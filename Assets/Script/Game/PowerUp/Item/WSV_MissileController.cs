using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Character;
using Mirror;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(Rigidbody))]
public class MissileController : NetworkBehaviour,IExplodable
{
    private enum EHomingMode
    {
        None = 0,       // 追尾しない
        StrongHoming,   // ずっとターゲットの元に移動する
        WeakHoming,     // ターゲットに近づけたら補間追尾(Lerp)
    }
    private Color _explodeColor = Color.clear;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _stopSearchInterval;
    [SerializeField]
    private float _searchRadius;

    [SerializeField]
    private float _chaseRadius;

    [SerializeField]
    [Range(0,1f)]
    private float _chaseRotationRate;
    private float _sqrChaseRadius;
    private float _sqrSearchRadius;
    private bool _isChasing;
    private float _targetLostTimeCnt;

    private float _explodeRadius = 0f;
    private int _ownerPlayerID = -1;

    private bool _canHoming;

    private EHomingMode _homingMode;
    private Transform _target;
    private Material _material;
    private Rigidbody _rigidbody;
    private MeshRenderer _meshRenderer;

    public float ExplodeRadius => _explodeRadius;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _canHoming = false;
        _homingMode = EHomingMode.None;
        _sqrSearchRadius = Mathf.Pow(_searchRadius,2f);
        _sqrChaseRadius = Mathf.Pow(_chaseRadius,2f);
        _targetLostTimeCnt = 0f;
        _rigidbody = GetComponent<Rigidbody>();
        _isChasing = false;
    }
    private void Start()
    {
        _material = new Material(_meshRenderer.sharedMaterial)
        {
            hideFlags = HideFlags.DontSave,
            color = _explodeColor
        };
        _meshRenderer.sharedMaterial = _material;
    }

    private void Update()
    {
        // 追尾しない場合終了
        if(_homingMode == EHomingMode.None)
            return;

        // ターゲットヘの距離を計算（二乗のまま）
        float distance = GetSqrDistanceToTarget();

        // ターゲットがない
        if(distance == float.PositiveInfinity)
            return;
        
        switch(_homingMode)
        {
            case EHomingMode.StrongHoming:
            {
                if(distance <= _sqrSearchRadius)
                    _homingMode = EHomingMode.WeakHoming;
                break;
            }
            case EHomingMode.WeakHoming:
            {
                break;
            }
        }
        // 第一形態かつターゲットヘの距離が検索範囲内、第二形態へ変更
        if(_homingMode == EHomingMode.StrongHoming && distance <= _sqrSearchRadius)
        {
            _homingMode = EHomingMode.WeakHoming;
        }

        if(_homingMode == EHomingMode.WeakHoming && distance <= _sqrChaseRadius)
        {
            if(!_isChasing)
            {
                _isChasing = true;
            }       
        }
        // 第二形態かつ検索範囲から離れる、追尾停止カウンターを加算する
        if(_homingMode == EHomingMode.WeakHoming && distance > _sqrChaseRadius)
        {
            if(_isChasing)
            {
                _targetLostTimeCnt += Time.deltaTime;

                // 一定時間がたったら追尾停止
                if(_targetLostTimeCnt >= _stopSearchInterval)
                {
                    _homingMode = EHomingMode.None;
                    _isChasing = false;
                }
            }
        }
    }

    // Update is called once per frame
    public void SetupExplode(int owner, Color color)
    {
        _ownerPlayerID = owner;
        _explodeColor = color;
        
        _meshRenderer.sharedMaterial.color = _explodeColor;

        LockOnNearestTarget();
    }

    private void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
        }
    }

    [Server]
    private void ExplodeMissile()
    {
        PaintExplodeArea();
        NetworkServer.Destroy(gameObject);
    }

    private void LockOnNearestTarget()
    {
        var networkIdentities = NetworkServer.connections;

        float targetDistance = float.PositiveInfinity;
        // サーバーに繋がっているオブジェクトから探す
        foreach(var identityKeyValuePair in networkIdentities)
        {
            GameObject identityObject = identityKeyValuePair.Value.identity.gameObject;

            // プレイヤーだった場合
            if(identityObject.TryGetComponent(out Player player))
            {
                // 見つかったプレイヤーがミサイルの使い手だった場合
                if(player.ID == _ownerPlayerID)
                    continue;

                // 一番近いプレイヤーを探す
                float tempDistance = Vector3.Distance(transform.position,identityObject.transform.position);
                if(tempDistance <= targetDistance)
                {
                    targetDistance = tempDistance;
                    _target = identityObject.transform;
                }
            }
        }
    }

    private void PaintExplodeArea()
    {
        if (_ownerPlayerID == -1)
        {
            return;
        }

        List<Vector3> explodeAreaVertexes = new List<Vector3>();
        for(int i = 0;i < 30;++i)
        {
            Quaternion angle = Quaternion.Euler(0, 12f * i, 0);
            Vector3 vert =  angle * Vector3.right ;
            explodeAreaVertexes.Add(vert.normalized * _explodeRadius + transform.position);
        }

        PaintAreaEvent paintEvent = new PaintAreaEvent
        {
            Verts = explodeAreaVertexes.ToArray(),
            PlayerID = _ownerPlayerID,
            PlayerAreaColor = _explodeColor
        };
        TypeEventSystem.Instance.Send(paintEvent);
    }

    private void OnCollisionEnter(Collision other) 
    {
        ExplodeMissile();
    }

    // ターゲットヘの距離を返す(二乗のまま)
    // ターゲットがない場合はfloat.PositiveInfinityを返す
    private float GetSqrDistanceToTarget()
    {
        // ターゲットがない
        if(_target == null)
            return float.PositiveInfinity;

        return (_target.position - transform.position).sqrMagnitude;
    }

    private void ChaseTarget()
    {
        if(_target == null)
            return;

        Vector3 nextMoveDirection = _target.position - transform.position;
        
        Quaternion rotation = Quaternion.LookRotation(nextMoveDirection, Vector3.up);

        #region Chase Rotation
        // TODO Need good implementation
        _rigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, _chaseRotationRate);
        #endregion
    }

}

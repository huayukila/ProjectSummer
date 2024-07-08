using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Character;
using Mirror;
using UnityEngine;


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
    private float _searchRadius;

    private float _sqrSearchRadius;

    private float _explodeRadius = 0f;
    private int _ownerPlayerID = -1;

    private bool _canHoming;

    private EHomingMode _homingMode;

    private Transform _target;
    private Material _material;

    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _canHoming = false;
        _homingMode = EHomingMode.None;
        _sqrSearchRadius = Mathf.Pow(_searchRadius,2f);
    }
    private void Start()
    {
        _material = new Material(_meshRenderer.sharedMaterial) { hideFlags = HideFlags.DontSave};
        _material.color = _explodeColor;
        _meshRenderer.sharedMaterial = _material;
    }

    private void Update()
    {
        if(_homingMode == EHomingMode.None)
            return;

        // ターゲットヘの距離を計算（二乗のまま）
        float distance = GetSqrDistanceToTarget();

        // ターゲットがない
        if(distance == float.PositiveInfinity)
            return;
        
        if(distance <= _sqrSearchRadius)
        {
            _homingMode = EHomingMode.WeakHoming;
        }
    }

    private void FixedUpdate()
    {
        // 追尾できない場合更新しない
        if(!_canHoming)
            return;

        switch(_homingMode)
        {
            // 第一段階
            // ターゲットの元に移動
            case EHomingMode.StrongHoming:
            {
                break;
            }
            // 第二段階
            // ターゲットに近づけたら補間追尾
            case EHomingMode.WeakHoming:
            {
                break;
            }
            // 第三段階
            // ターゲットを失ったら
            case EHomingMode.None:
            {
                break;
            }
        }
        
    }
    // Update is called once per frame
    public void SetExplodeProperty(int owner, float radius, Color color)
    {
        _ownerPlayerID = owner;
        _explodeRadius = radius;
        _explodeColor = color;

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

}

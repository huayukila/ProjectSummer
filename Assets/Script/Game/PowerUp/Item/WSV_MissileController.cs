using System.Collections;
using System.Collections.Generic;
using System.Timers;
using WSV.Character;
using Mirror;
using UnityEngine;
using UnityEngine.Analytics;

[RequireComponent(typeof(Rigidbody))]
public class MissileController : NetworkBehaviour,IExplodable
{
    private enum EHomingMode
    {
        None = 0,       // �ǔ����Ȃ�
        StrongHoming,   // �����ƃ^�[�Q�b�g�̌��Ɉړ�����
        WeakHoming,     // �^�[�Q�b�g�ɋ߂Â������Ԓǔ�(Lerp)
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
        // �ǔ����Ȃ��ꍇ�I��
        if(_homingMode == EHomingMode.None)
            return;

        // �^�[�Q�b�g�w�̋������v�Z�i���̂܂܁j
        float distance = GetSqrDistanceToTarget();

        // �^�[�Q�b�g���Ȃ�
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
        // ���`�Ԃ��^�[�Q�b�g�w�̋����������͈͓��A���`�Ԃ֕ύX
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
        // ���`�Ԃ������͈͂��痣���A�ǔ���~�J�E���^�[�����Z����
        if(_homingMode == EHomingMode.WeakHoming && distance > _sqrChaseRadius)
        {
            if(_isChasing)
            {
                _targetLostTimeCnt += Time.deltaTime;

                // ��莞�Ԃ���������ǔ���~
                if(_targetLostTimeCnt >= _stopSearchInterval)
                {
                    _homingMode = EHomingMode.None;
                    _isChasing = false;
                }
            }
        }
    }

    // Update is called once per frame
    public void Init(int owner, Color color)
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
        // �T�[�o�[�Ɍq�����Ă���I�u�W�F�N�g����T��
        foreach(var identityKeyValuePair in networkIdentities)
        {
            GameObject identityObject = identityKeyValuePair.Value.identity.gameObject;

            // �v���C���[�������ꍇ
            if(identityObject.TryGetComponent(out Player player))
            {
                // ���������v���C���[���~�T�C���̎g���肾�����ꍇ
                if(player.ID == _ownerPlayerID)
                    continue;

                // ��ԋ߂��v���C���[��T��
                float tempDistance = Vector3.Distance(transform.position,identityObject.transform.position);
                if(tempDistance <= targetDistance)
                {
                    targetDistance = tempDistance;
                    _target = identityObject.transform;
                }
            }
        }
    }

    [Server]
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

        IPaintSystem paintSystem = (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).GetFramework().GetSystem<IPaintSystem>();
        if(paintSystem != null)
        {
            paintSystem.Paint(explodeAreaVertexes.ToArray(),_ownerPlayerID,_explodeColor);
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        ExplodeMissile();
    }

    // �^�[�Q�b�g�w�̋�����Ԃ�(���̂܂�)
    // �^�[�Q�b�g���Ȃ��ꍇ��float.PositiveInfinity��Ԃ�
    private float GetSqrDistanceToTarget()
    {
        // �^�[�Q�b�g���Ȃ�
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

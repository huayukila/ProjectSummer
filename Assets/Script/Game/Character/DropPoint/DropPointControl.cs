using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

namespace Character
{
    public class DropPointControl : NetworkBehaviour
    {
        
        private struct PlayerDropPoints
        {
            // DropPoint���ۑ������List
            public List<GameObject> playerPoints;
            // DropPoint���Ǘ�����e
            public GameObject pointGroup;
        }

        private PlayerDropPoints _playerDropPoints;

        private TrailRenderer _tailTrailRenderer;      // DropPoint���q�����Ă��邱�Ƃ�\��TrailRenderer
        private GameObject _pointPrefab;             // DropPoint�̃v���n�u
        private float _tailFadeOutTimer;
        private Timer _dropPointTimer;           // DropPoint�̃C���X�^���X�����邱�Ƃ��Ǘ�����^�C�}�[

        private float trailOffset;

        //TODO refactorying
        private string _dropPointTag;
        private int _playerID;
        private Color _areaColor;

        private Player _player;

        private void Awake()
        {
            _pointPrefab = GameResourceSystem.Instance.GetPrefabResource("DropPoint");
            _tailFadeOutTimer = 0.0f;
            trailOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            _player = gameObject.GetOrAddComponent<Player>();
            _dropPointTag = "DropPoint";
            _playerID = -1;
            _areaColor = Color.clear;

            // �K����`�悷��GameObject�����
            GameObject trail = new GameObject(name + "Trail");
            // �v���C���[��e�ɂ���
            trail.transform.parent = transform;
            //todo take note
            // ���[���h���W�����[�J�����W�ɕϊ�����
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            trail.transform.localPosition = Vector3.down * 0.5f - localForward * trailOffset;
            trail.transform.localScale = Vector3.one;
            // TrailRenderer���A�^�b�`����
            _tailTrailRenderer = trail.gameObject.AddComponent<TrailRenderer>();

            _dropPointTimer = new Timer(Time.time, Global.DROP_POINT_INTERVAL,
                () =>
                {
                    // �^�C�}�[���I�������DropPoint��u��
                    CmdInstantiateDropPoint();
                }
                );
            _dropPointTimer.StartTimer(this);

            _playerDropPoints = new PlayerDropPoints
            {
                playerPoints = new List<GameObject>(),
                pointGroup = new GameObject("Player drop point group")
            };


        }
        // Update is called once per frame
        private void Update()
        {
            
            _tailFadeOutTimer += Time.deltaTime;
            // �v���C���[����Ɉ�莞�Ԃ��ړ�����������iDropPoint�̐������Ԃ̔����j
            if (_tailFadeOutTimer >= Global.DROP_POINT_ALIVE_TIME / 2.0f && _tailFadeOutTimer < Global.DROP_POINT_ALIVE_TIME)
            {
                // �s�����x���v�Z����@���@y = -1.9x + 1.95;
                float alpha = (-1.9f / Global.DROP_POINT_ALIVE_TIME) * _tailFadeOutTimer + 1.95f;
                // �s�����x�̍ŏ��l��0.05�ɐݒ肷��
                if (alpha < 0.05f)
                {
                    alpha = 0.05f;
                }
                SetTrailGradient(alpha);
            }

        }

        private void FixedUpdate()
        {
            DropNewPoint();
        }
        /// <summary>
        /// DropPoint���C���X�^���X������
        /// </summary>
        [Command]
        private void CmdInstantiateDropPoint()
        {
            GameObject pt = Instantiate(_pointPrefab, transform.position - transform.forward * trailOffset, transform.rotation);
            pt.tag = _dropPointTag;
            pt.GetComponent<DropPoint>().SetDestroyCallback(CmdRemovePoint);
            NetworkServer.Spawn(pt);
            AddPoint(pt);
        }

        /// <summary>
        /// TrailRenderer�̏����ݒ�s��
        /// </summary>
        public void Init()
        {
            _playerID = _player.ID;
            _dropPointTag = "DropPoint" + _playerID.ToString();
            _areaColor = _player.AreaColor;
            _tailTrailRenderer.material = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.DontSave};
            _tailTrailRenderer.startColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.endColor = Global.PLAYER_TRACE_COLORS[_playerID - 1];
            _tailTrailRenderer.startWidth = 1.0f;
            _tailTrailRenderer.endWidth = 1.0f;
            _tailTrailRenderer.time = Global.DROP_POINT_ALIVE_TIME;
        }

        /// <summary>
        /// DropPoint��u��
        /// </summary>    
        private void DropNewPoint()
        {
            if(_dropPointTimer.IsFinished())
            {
                _dropPointTimer.onReset();
                _dropPointTimer.StartTimer(this);
            }
        }

        /// <summary>
        /// TrailRenderer�̏�Ԃ����Z�b�g����
        /// </summary>
        public void ResetTrail()
        {
            _tailTrailRenderer.Clear();
            _tailFadeOutTimer = 0.0f;
            SetTrailGradient(1.0f);
        }

        /// <summary>
        /// TrailRenderer�̃O���f�B�G���g��ݒ肷��
        /// </summary>
        /// <param name="alpha">��Ԍ��̕s�����x</param>
        private void SetTrailGradient(float alpha)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(_areaColor, 0.0f), new GradientColorKey(_areaColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            _tailTrailRenderer.colorGradient = gradient;
        }


        /// <summary>
        /// List�ɂ���S�Ă�DropPoint(GameObject)�̃��[���h���W��Ԃ�
        /// </summary>
        /// <returns>List�̑S�Ă�GameObject�̃��[���h���W(Vector3�^)</returns>
        private Vector3[] DropPointsGameObjectToVector3()
        {
            // List�̃R�s�[�����
            //List<GameObject> retList = new List<GameObject>(list);
            // �߂�l�p�z������
            Vector3[] retPos = new Vector3[_playerDropPoints.playerPoints.Count];
            int index = 0;
            // List�̑S�Ă�GameObject�̃��[���h���W��߂�l�p�z��ɓ����
            foreach (GameObject ob in _playerDropPoints.playerPoints)
            {
                if(ob == null)
                    continue;
                
                retPos[index] = ob.transform.position;
                ++index;
            }
            return retPos;
            
        }

        /// <summary>
        /// ����̃v���C���[��DropPoint(GameObject)���Ǘ�����List��DropPoint(GameObject)������֐�
        /// </summary>
        /// <param name="ID">�v���C���[ID</param>
        /// <param name="dropPoint">List�ɓ����DropPoint</param>
        private void AddPoint(GameObject dropPoint)
        {
            // dropPoint�̐e��ݒ肵�āAList�ɓ����
            dropPoint.transform.parent = _playerDropPoints.pointGroup.transform;
            _playerDropPoints.playerPoints.Add(dropPoint);
            // ���݂��Ȃ��ꍇ�̓G���[���b�Z�[�W���o��
        }

        /// <summary>
        /// ������DropPoint(GameObject)��List��������֐�
        /// </summary>
        /// <param name="dropPoint">������DropPoint(GameObject)</param>
        [Command]
        private void CmdRemovePoint(GameObject dropPoint)
        {
            _playerDropPoints.playerPoints.Remove(dropPoint);
        }

        /// <summary>
        /// �v���C���[�̑S�Ă�DropPoint(GameObject)�������֐�
        /// </summary>
        /// <param name="ID">�v���C���[��ID</param>
        [Command]
        public void CmdClearDropPoints()
        {
            // �S�Ă�DropPoint(GameObject)��j������
            foreach (GameObject dropPoint in _playerDropPoints.playerPoints)
            {
                Destroy(dropPoint);
            }
            // List�ɂ��镨��S������
            _playerDropPoints.playerPoints.Clear();
        }

        /// <summary>
        /// �v���C���[�̑S�Ă�DropPoint�̃��[���h���W��߂��֐�
        /// </summary>
        /// <param name="ID">�v���C���[��ID</param>
        /// <returns>�S�Ă�DropPoint(GameObject)�̃��[���h���W�iVector3�^�j�A�v���C���[�����݂��Ȃ��ꍇ�͋�̔z���Ԃ�</returns>
        public Vector3[] GetPlayerDropPoints()
        {
            return DropPointsGameObjectToVector3();
        }
        /// <summary>
        /// DropPointSystem���f�C�j�V�����C�[�[�V��������֐�
        /// </summary>
        private void OnDestroy()
        {
            CmdClearDropPoints();
        }
    }
}
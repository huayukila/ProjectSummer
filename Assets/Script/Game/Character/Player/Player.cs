using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Gaming.PowerUp;
using Math;
using Mirror;
using Character;

public struct PaintAreaEvent
{
    public Vector3[] Verts;
    public int PlayerID;
    public Color32 PlayerAreaColor;
}

public struct PlayerItemContainer
{
    public IPlayer2ItemSystem Player2ItemSystem;
    public Player Player;
}

namespace Character
{
    [RequireComponent(typeof(ColorCheck), typeof(PlayerInput))]
    public class Player : Character, IPlayer2ItemSystem, IItemAffectable, IPlayerCommand, IPlayerInfo, IPlayerState, IPlayerInterfaceContainer,IPlayerBoost
    {
        // �v���C���[�̏��
        private enum State
        {
            None = 0,
            Fine,
            Dead,
            Invincible,
            Uncontrollable,
            Stun,
        }
        // �v���C���[�̏��
        private struct PlayerInfo
        {
            public int ID;          //�v���C���[��ID
            public Color AreaColor; //�v���C���[�̗̈�̐F
        }
        private struct PlayerSilkData
        {
            public int SilkCount;                   // �v���C���[�������Ă�����̎��̐�
            public GameObject SilkRenderer;         // �v���C���[�������Ă�����̎�����ʂɕ\������GameObject
        }
        public bool HadSilk => _silkData.SilkCount > 0;
        public IItem item { get; set; }
        private ColorCheck _colorCheck;                     // �J���[�`�F�b�N�R���|�l���g
        //TODO �����ɂ���
        private InputAction _boostAction;                   // �v���C���[�̃u�[�X�g����
        private InputAction _rotateAction;                  // �v���C���[�̉�]����
        private InputAction _itemAction;
        private PlayerInput _input;                         // playerInputAsset

        [field:SerializeField]
        private State _playerState;                         // �v���C���[�̃X�e�[�^�X

        private float _itemPlaceOffset;                     // �A�C�e���̕��u�ꏊ�ƃv���C���[���W�̊Ԃ̋����i�v���C���[�R���C�_�[�̕ӂ̒����i�����`�j�j
        private float _currentMoveSpeed;                    // �v���C���[�̌��ݑ��x
        private float _moveSpeedCoefficient;                // �v���C���[�̈ړ����x�̌W��
        private Rigidbody _rigidbody;                       // �v���C���[��Rigidbody
        private Vector3 _rotateDirection;                   // �v���C���[�̉�]����
        private bool _isBoostCooldown = false;              // �u�[�X�g�N�[���_�E�����Ă��邩�̃t���O
        private SpriteRenderer _imageSpriteRenderer;        // �v���C���[�摜��SpriteRenderer
        private PlayerAnim _playerAnim;
        private DropPointControl _dropPointCtrl;            // �v���C���[��DropPointControl
        private PlayerParticleSystemControl _particleSystemCtrl; // �v���C���[���g�ɂ������Ă���p�[�e�B�N���V�X�e�����Ǘ�����R���g���[���[
        //TODO refactoring           
        private PlayerSilkData _silkData;
        private float mBoostCoefficient;
        [SerializeField]
        private PlayerInfo _playerInfo = default;           // �v���C���[�̏��
        private float _returnToFineTimer = 0f;
        //TODO �e�X�g�p
        public Sprite[] silkCountSprites;
        private Dictionary<EItemEffect, Action> _itemAffectActions;
        private PlayerInterfaceContainer _playerInterface;

        private Vector3 _spawnPos;

        //TODO need refactorying(player should not know the existence of system)
        private IItemSystem _itemSystem;

        private GamePlayer _gamePlayer;

        private float _boostChargeTimeCnt;

        public Vector3 SpawnPos
        {
            get
            {
                return _spawnPos;
            }
            
            set
            {
                _spawnPos = value;
            }
        }
        private void Awake()
        {
            #region fuck mirror
            TypeEventSystem.Instance.Register<SendInitializedPlayerEvent>
            (
                eve =>
                {
                    IPlayerInfo info = eve.Player.GetComponent<IPlayerInfo>();
                    if(info != null)
                    {
                        eve.Player.GetComponent<DropPointControl>().Init();
                        eve.Player.transform.forward = Global.PLAYER_DEFAULT_FORWARD[info.ID - 1];
                        eve.Player.SpawnPos = (NetWorkRoomManagerExt.singleton as IRoomManager).GetRespawnPosition(info.ID - 1).position;
                    }
                    
                }
            );
            #endregion

            _playerInterface = new PlayerInterfaceContainer(this);
            // ����������
            Init();
            // Item affectable actions init
            InitItemAffect();

            {
                _boostChargeTimeCnt = Global.BOOST_COOLDOWN_TIME;
            }
        }
        private void Start()
        {
            _currentMoveSpeed = 0.0f;


            _particleSystemCtrl.Play();

            {
                TypeEventSystem.Instance.Register<PaintAreaEvent>
                (
                    e =>
                    {
                        IPaintSystem paintSystem = (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).GetFramework().GetSystem<IPaintSystem>();
                        if(paintSystem != null)
                        {
                            paintSystem.Paint(e.Verts,e.PlayerID,e.PlayerAreaColor);
                        }
                    }
                ).UnregisterWhenGameObjectDestroyed(gameObject);

            }

            {
                _itemSystem = (NetWorkRoomManagerExt.singleton as NetWorkRoomManagerExt).GetFramework().GetSystem<IItemSystem>();
            }
        }
        private void Update()
        {
            if (_playerState == State.Dead)
                return;

            // �v���C���[���u�ʏ�v��Ԃ���Ȃ��ƌ�قǂ̏��������s���Ȃ�
            else if(_playerState != State.Fine)
            {
                ReturnToFineCountDown();
                return;
            }
            UpdateFine();

        }

        private void FixedUpdate()
        {
            switch(_playerState)
            {
                case State.Fine:
                    // �v���C���[�̓���
                    PlayerMovement(Global.PLAYER_ACCELERATION);
                    // �v���C���[�̉�]
                    PlayerRotation();
                    break;
                case State.Uncontrollable:
                    float deceleration = GetOnSlipDeceleration();
                    PlayerMovement(deceleration);
                    break;
                case State.Stun:
                    _currentMoveSpeed = 0f;
                    break;
            }

        }

        private void LateUpdate()
        {
            switch(_playerState)
            {
                case State.Fine:
                    return;
                case State.Uncontrollable:
                    PlaySlipAnimation();
                    return;
            }
            
        }

        #region InternalLogic

        private void UpdateFine()
        {
            // �v���C���[�摜�������Ɠ��������Ɍ������Ƃɂ���
            //mImageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

            // �v���C���[�摜�̌�����ς���
            FlipCharacterImage();
            // �v���C���[�C���v�b�g���擾����
            Vector2 rotateInput = _rotateAction.ReadValue<Vector2>();
            // ��]���������߂�
            _rotateDirection = new Vector3(rotateInput.x, 0.0f, rotateInput.y);
            // �v���C���[������Ƃ���̒n�ʂ̐F���`�F�b�N����
            CheckGroundColor();
            // �̈��`�悵�Ă݂�
            TryPaintArea();

            if(_isBoostCooldown)
            {
                _boostChargeTimeCnt += Time.deltaTime;
            }

        }

        /// <summary>
        /// �v���C���[�̃v���p�e�B������������
        /// </summary>
        private void Init()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _colorCheck = GetComponent<ColorCheck>();
            // �v���C���[�����̉摜�̃����_���[���擾����
            _imageSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _particleSystemCtrl = gameObject.GetComponent<PlayerParticleSystemControl>();
            // DropPointControl�R���|�l���g��ǉ�����
            _dropPointCtrl = gameObject.GetComponent<DropPointControl>();
            // PlayerAnim�R���|�l���g��ǉ�����
            _playerAnim = gameObject.GetComponent<PlayerAnim>();


            _colorCheck.layerMask = LayerMask.GetMask("Ground");
            _moveSpeedCoefficient = 1.0f;
            _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
            _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED;
            _playerState = State.Fine;
            _itemPlaceOffset = GetComponent<BoxCollider>().size.x * transform.localScale.x * 0.5f;
            // �\�����ʂ�ϊ�����
            _imageSpriteRenderer.transform.localPosition = new Vector3(0.0f, -0.05f, 0.0f);
            _silkData.SilkCount = 0;
            _silkData.SilkRenderer = Instantiate(GameResourceSystem.Instance.GetPrefabResource("GoldenSilkImage"));
            _silkData.SilkRenderer.transform.parent = _imageSpriteRenderer.transform;
            _silkData.SilkRenderer.SetActive(false);
            mBoostCoefficient = 1f;

            SetPlayerInputProperties();

            _gamePlayer = GetComponent<GamePlayer>();


        }

        private void InitItemAffect()
        {
            _itemAffectActions = new Dictionary<EItemEffect, Action>
            {
                // Item Effect     Action
                { EItemEffect.Stun, OnStun },
                { EItemEffect.Slip, OnSlip }
            };
        }

        #region Player Move
        /// <summary>
        /// �v���C���[�̈ړ��𐧌䂷��
        /// </summary>
        private void PlayerMovement(in float acceleration)
        {
            // ���x���v�Z���A�͈͓��ɐ�������
            _currentMoveSpeed =  _currentMoveSpeed + acceleration * Time.fixedDeltaTime;
            _currentMoveSpeed = Mathf.Clamp(_currentMoveSpeed, 0f, _status.MaxMoveSpeed);

            // �O�����̈ړ�������
            Vector3 moveDirection = transform.forward * _currentMoveSpeed * _moveSpeedCoefficient * mBoostCoefficient;
            _rigidbody.velocity = moveDirection;
        }
        #endregion


        #region Player Rotate
        /// <summary>
        /// �v���C���[�̉�]�𐧌䂷��
        /// </summary>
        private void PlayerRotation()
        {
            // �������͂��Ȃ��ƏI��
            if (_rotateDirection == Vector3.zero)
                return;

            // ���͂��ꂽ�����։�]����
            {
                Quaternion rotation = Quaternion.LookRotation(_rotateDirection, Vector3.up);
                _rigidbody.rotation = Quaternion.Slerp(transform.rotation, rotation, _status.RotationSpeed * Time.fixedDeltaTime);
            }
        }
        #endregion

        /// <summary>
        /// �L�����N�^�[�̉摜�𔽓]����֐�
        /// </summary>
        private void FlipCharacterImage()
        {
            if (transform.forward.x < 0.0f)
            {
                _imageSpriteRenderer.flipX = false;
            }
            else
            {
                _imageSpriteRenderer.flipX = true;
            }
        }

        /// <summary>
        /// �v���C���[�̎��S��Ԃ�ݒ肷��
        /// </summary>
        private void StartDead()
        {
            _playerAnim.StartExplosionAnim();

            DropSilkEvent dropSilkEvent = new DropSilkEvent()
            {
                dropCount = _silkData.SilkCount,
                pos = transform.position
            };
            TypeEventSystem.Instance.Send(dropSilkEvent);
            TypeEventSystem.Instance.Send(new PlayerRespawnEvent(){ Player = gameObject});

            // �v���C���[�̏�Ԃ����Z�b�g����
            ResetStatus();
            // �v���C���[�̌��������Z�b�g����
            FlipCharacterImage();
            // �R���|�l���g�𖳌����ɂ���
            GetComponent<DropPointControl>().enabled = false;
            GetComponentInChildren<TrailRenderer>().enabled = false;
            GetComponent<Collider>().enabled = false;
            // �v���C���[�����C�x���g�����N����
            _particleSystemCtrl.Stop();
            SetPowerUpLevel();
        }

        /// <summary>
        /// �v���C���[�̃X�e�C�^�X�����Z�b�g����
        /// </summary>
        private void ResetStatus()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            _playerState = State.Dead;

            transform.localScale = Vector3.one;

            _currentMoveSpeed = 0.0f;

            _isBoostCooldown = false;

            _silkData.SilkCount = 0;

            transform.forward = Global.PLAYER_DEFAULT_FORWARD[(_playerInfo.ID - 1)];

            _gamePlayer.CmdOnClearAllDropPoints();

            _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;

            _dropPointCtrl.ResetTrail();

            _silkData.SilkRenderer.SetActive(false);

            _returnToFineTimer = 0f;
        }
        /// <summary>
        /// �n�ʂ̐F���`�F�b�N����
        /// </summary>
        private void CheckGroundColor()
        {
            // �����̗̈�ɂ�����
            if (_colorCheck.isTargetColor(Color.clear))
            {
                _moveSpeedCoefficient = 1.0f;
            }
            // �ʂ̃v���C���[�̗̈�ɂ�����
            else if (_colorCheck.isTargetColor(_playerInfo.AreaColor))
            {
                _moveSpeedCoefficient = Global.SPEED_UP_COEFFICIENT;
            }
            // �h���Ă��Ȃ��n�ʂɂ�����
            else
            {
                _moveSpeedCoefficient = Global.SPEED_DOWN_COEFFICIENT;
            }
        }

        /// <summary>
        /// �v���C���[�̈��`�悵�Ă݂�֐�
        /// </summary>
        private void TryPaintArea()
        {
            Vector3[] dropPoints = _dropPointCtrl.GetPlayerDropPoints();
            // DropPoint��4�ȏ゠��Ε`��ł���
            if (dropPoints.Length >= 4)
            {
                // �v���C���[�̐擪���W
                Vector3 endPoint1 = transform.position + transform.forward * _itemPlaceOffset;
                // �v���C���[�����O�ɃC���X�^���X������DropPoint�̍��W
                Vector3 endPoint2 = dropPoints[dropPoints.Length - 1];
                // endPoint1��endPoint2�ō�����x�N�g����endPoint2�ȊO��DropPoint��擪���珇�Ԃ�2���ō�����x�N�g����������Ă��邩�ǂ������`�F�b�N����
                for (int i = 0; i < dropPoints.Length - 2; ++i)
                {
                    // ��̃x�N�g�������s������continue
                    if (VectorMath.IsParallel(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1))
                    {
                        continue;
                    }
                    // ���ꂼ��̍��W�_�Ǝ������g�ȊO�̃x�N�g���̈ʒu�֌W���v�Z����(0���傫���Ȃ�x�N�g���̍����A0��菬�����Ȃ�x�N�g���̉E���A0�Ȃ�x�N�g���̒�)
                    float pointPos1 = VectorMath.PointOfLine(dropPoints[i], endPoint2, endPoint1);
                    float pointPos2 = VectorMath.PointOfLine(dropPoints[i + 1], endPoint2, endPoint1);
                    float pointPos3 = VectorMath.PointOfLine(endPoint2, dropPoints[i], dropPoints[i + 1]);
                    float pointPos4 = VectorMath.PointOfLine(endPoint1, dropPoints[i], dropPoints[i + 1]);
                    // ��̃x�N�g����������Ă�����`�悷��
                    if (pointPos1 * pointPos2 < 0 && pointPos3 * pointPos4 < 0)
                    {
                        // ��_���v�Z����
                        Vector3 crossPoint = VectorMath.GetCrossPoint(dropPoints[i], dropPoints[i + 1], endPoint2, endPoint1);
                        // �`�悷��̈�̒��_���擾����
                        List<Vector3> verts = new List<Vector3>();
                        for (int j = i + 1; j < dropPoints.Length; j++)
                        {
                            verts.Add(dropPoints[j]);
                        }
                        verts.Add(crossPoint);
                        // �`�悷��

                        #region Paint Area
                        PaintAreaEvent paintEvent = new PaintAreaEvent
                        {
                            Verts = verts.ToArray(),
                            PlayerID = _playerInfo.ID,
                            PlayerAreaColor = _playerInfo.AreaColor
                        };
                        TypeEventSystem.Instance.Send(paintEvent);
                        #endregion

                        TryCaptureObject(verts.ToArray());
                        // �S�Ă�DropPoint������
                        _gamePlayer.CmdOnClearAllDropPoints();
                        
                        // �K����TrailRenderer�̏�Ԃ����Z�b�g����
                        _dropPointCtrl.ResetTrail();
                        break;
                    }
                }
            }
        }

        private void TryCaptureObject(Vector3[] verts)
        {
            #region Pick Silk
            bool isPickedNew = false;
            List<Vector3> caputuredSilk = null;
            if(_itemSystem != null)
            {
                Vector3[] silkPos = _itemSystem.GetOnFieldSilkPos();
                caputuredSilk = new List<Vector3>();
                
                foreach (Vector3 pos in silkPos)
                {
                    if (VectorMath.InPolygon(pos, verts))
                    {
                        _silkData.SilkCount++;
                        caputuredSilk.Add(pos);
                        isPickedNew = true;
                    }
                }
            }
            
            // ���̎��̉摜��\��
            if (isPickedNew)
            {
                SilkCapturedEvent silkCapturedEvent = new SilkCapturedEvent()
                {
                    ID = _playerInfo.ID,
                    positions = caputuredSilk?.ToArray()
                };
                TypeEventSystem.Instance.Send(silkCapturedEvent);
                //AudioManager.Instance.PlayFX("SpawnFX", 0.7f);
                // �L�����N�^�[�摜�̏c�̑傫�����擾���ĉ摜�̏�ŕ\������
                _silkData.SilkRenderer.transform.localPosition = new Vector3(-_imageSpriteRenderer.bounds.size.x / 4f, _imageSpriteRenderer.bounds.size.z * 1.2f, 0);
                _silkData.SilkRenderer.SetActive(true);
                Transform silkCount = _silkData.SilkRenderer.transform.GetChild(0);
                if (silkCount != null)
                {
                    silkCount.GetComponent<SpriteRenderer>().sprite = silkCountSprites[_silkData.SilkCount];
                    silkCount.transform.localPosition = new Vector3(_imageSpriteRenderer.bounds.size.x, 0, 0);
                }
                SetPowerUpLevel();
            }
            #endregion
            #region Pick Item
            List<Vector3> capturedItemBoxPos = null;
            if(_itemSystem != null)
            {
                Vector3[] itemBoxPos = _itemSystem.GetOnFieldItemBoxPos();
                capturedItemBoxPos = new List<Vector3>();
                isPickedNew = false;
                foreach(var pos in itemBoxPos)
                {
                    if(VectorMath.InPolygon(pos,verts))
                    {
                        capturedItemBoxPos.Add(pos);
                        isPickedNew = true;
                    }
                }
            }
            
            if(isPickedNew)
            {
                TypeEventSystem.Instance.Send(new PlayerGetItem
                {
                    player = this,
                });
                TypeEventSystem.Instance.Send(new GetNewItem
                {
                    ItemBoxsPos = capturedItemBoxPos?.ToArray()
                });
            }
            #endregion
        }

        private void SetPlayerInputProperties()
        {
            _input = GetComponent<PlayerInput>();
            _input.defaultActionMap = "Player";
            _input.neverAutoSwitchControlSchemes = true;
            _input.SwitchCurrentActionMap("Player");
            _rotateAction = _input.actions["Rotate"];
            _boostAction = _input.actions["Boost"];
            _boostAction.performed += OnBoost;
            _itemAction = _input.actions["Item"];
            _itemAction.performed += OnUseItem;
        }

        private void SetPowerUpLevel()
        {
            if (_silkData.SilkCount > 0)
            {
                _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED + Global.POWER_UP_PARAMETER[_silkData.SilkCount - 1].SpeedUp;
                _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED + Global.POWER_UP_PARAMETER[_silkData.SilkCount - 1].RotateUp;
            }
            else
            {
                _status.MaxMoveSpeed = Global.PLAYER_MAX_MOVE_SPEED;
                _status.RotationSpeed = Global.PLAYER_ROTATION_SPEED;
            }
        }

        private void OnUseItem(InputAction.CallbackContext ctx)
        {
            if(_playerState == State.Fine && ctx.performed)
            {
                if(_gamePlayer != null)
                {
                    _gamePlayer.CmdOnItemSpawn(gameObject);
                    //this.UseItem(this);
                }
            }
        }

        private void ReturnToFineCountDown()
        {
            _returnToFineTimer -= Time.deltaTime;
            if(_returnToFineTimer <= 0f)
            {
                _imageSpriteRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, transform.forward);
                _playerState = State.Fine;
            }
        }

        private float GetOnSlipDeceleration()
        {
            if(_currentMoveSpeed >= _status.MaxMoveSpeed / 4f)
            {
                return -(_currentMoveSpeed - _status.MaxMoveSpeed / 4f) / (_returnToFineTimer - Global.ON_SLIP_TIME / 2f);  
            }
            if(_currentMoveSpeed <= Global.ON_SLIP_MIN_SPEED)
            {
                return 0f;
            }
            else
            {
                return -_currentMoveSpeed / _returnToFineTimer;
            }
        }

        private void PlaySlipAnimation()
        {
            //float rotationAngle = (-720f / Global.ON_SLIP_TIME * _returnToFineTimer + 720f) * Time.deltaTime;
            float rotationAngle = 720f / Global.ON_SLIP_TIME * Time.deltaTime;
            _imageSpriteRenderer.transform.Rotate(Vector3.back * rotationAngle);
        }
        private void OnEnable()
        {
            _input?.ActivateInput();
        }
        private void OnDisable()
        {
            _input?.DeactivateInput();
        }
        private void OnDestroy()
        {
            _boostAction.performed -= OnBoost;
            _itemAction.performed -= OnUseItem;
        }

        #region Boost Method
        // �u�[�X�g
        private void OnBoost(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_playerState == State.Fine && _isBoostCooldown == false)
                {
                    _boostChargeTimeCnt = 0f;
                    mBoostCoefficient = 1.5f;
                    _currentMoveSpeed = _status.MaxMoveSpeed;
                    _isBoostCooldown = true;
                    TypeEventSystem.Instance.Send(new BoostStart 
                    { 
                        Number = _playerInfo.ID
                    });
                    Timer stopBoostTimer = new Timer(Time.time, Global.BOOST_DURATION_TIME,
                        () =>
                        {
                            mBoostCoefficient = 1.0f;
                        }
                        );
                    Timer boostCoolDownTimer = new Timer(Time.time,Global.BOOST_COOLDOWN_TIME,
                        () =>
                        {
                            _isBoostCooldown = false;
                            _boostChargeTimeCnt = Global.BOOST_COOLDOWN_TIME;
                        }
                        );
                    stopBoostTimer.StartTimer(this);
                    boostCoolDownTimer.StartTimer(this);
                }

            }
        }
        #endregion

        #endregion // InternalLogic

        #region Item Effect

        private void OnSlip()
        {
            _playerState = State.Uncontrollable;
            _returnToFineTimer = Global.ON_SLIP_TIME;
        }

        private void OnStun()
        {
            _playerState = State.Stun;
            _currentMoveSpeed = 0f;
            _rigidbody.velocity = Vector3.zero;
            _returnToFineTimer = Global.ON_STUN_TIME;
        }

        #endregion //Item Effect

        #region Interface
        public int ID => _playerInfo.ID;
        public int SilkCount => _silkData.SilkCount;
        public Color AreaColor => _playerInfo.AreaColor;
        public void SetInfo(int ID, Color color)
        {
            if (_playerInfo.ID != 0)
                return;
            
            {
                _playerInfo.ID = ID;
                _playerInfo.AreaColor = color;
                name = "Player" + _playerInfo.ID.ToString();
            }
        }
        public bool IsDead => _playerState == State.Dead;
        public bool IsFine => _playerState == State.Fine;
        public bool IsInvincible => _playerState == State.Invincible;
        public bool IsUncontrollable => _playerState == State.Uncontrollable;
        public bool IsStuning => _playerState == State.Stun;

        public PlayerInterfaceContainer GetContainer()
        {
            return _playerInterface;
        }

        public void CallPlayerCommand(EPlayerCommand cmd)
        {
            Debug.Log("Command:" + cmd.ToString() + " get called");
            switch(cmd)
            {
                case EPlayerCommand.Respawn:
                    StartRespawn();
                    break;
                case EPlayerCommand.Dead:
                    StartDead();
                    break;
                default:
                    break;
            }
        }

        private void StartRespawn()
        {
            // ���S�ȊO�͍Đ��������Ȃ�
            if (_playerState != State.Dead)
                return;

            // �Đ�����
            {
                transform.position = _spawnPos;
                transform.forward = Global.PLAYER_DEFAULT_FORWARD[_playerInfo.ID - 1];
                _playerState = State.Fine;

                GetComponentInChildren<TrailRenderer>().enabled = true;
                GetComponent<DropPointControl>().enabled = true;
                GetComponent<Collider>().enabled = true;

                // ������̉��G�t�F�N�g�̍쐬
                GameObject smoke = Instantiate(GameResourceSystem.Instance.GetPrefabResource("Smoke"), transform.position, Quaternion.identity);
                smoke.transform.rotation = Quaternion.LookRotation(Vector3.up);
                smoke.transform.position -= new Vector3(0.0f, 0.32f, 0.0f);

                // �p�[�e�B�N���V�X�e���̍ĊJ
                _particleSystemCtrl.Play();
            }

        }

        public void OnEffect(string effectName)
        {
            effectName.ToTitleCast();

            object receivedEffect;

            if(Enum.TryParse(typeof(EItemEffect), effectName,out receivedEffect))
            {
                _itemAffectActions[(EItemEffect)receivedEffect].Invoke();
            }
            else
            {
                Debug.LogWarning("Can't find action of " + effectName + " effect." + "(In class " + GetType().Name + " )");
            }
        }
        public float ItemPlaceOffset => _itemPlaceOffset;

        //
        public float ChargeBarPercentage => Mathf.Clamp(_boostChargeTimeCnt / Global.BOOST_COOLDOWN_TIME, 0f, 1f);
        #endregion // Interface

        #region Obsolete Code
            /*
            /// <summary>
            /// �Փ˂��������Ƃ���������
            /// </summary>
            /// <param name="collision"></param>
            private void OnCollisionEnter(Collision collision)
            {
                // ���S�����v���C���[�͋��̖Ԃ������Ă�����
                if (_silkData.SilkCount > 0)
                {
                    DropSilkEvent dropSilkEvent = new DropSilkEvent()
                    {
                        pos = transform.position,
                    };
                    // ���̎��̃h���b�v�ꏊ��ݒ肷��
                    //HACK EventSystem temporary invalid
                    //TypeEventSystem.Instance.Send(dropSilkEvent);
                }
                // �Փ˂����玀�S��Ԃɐݒ肷��
                StartDead();
            }

            private void OnTriggerEnter(Collider other)
            {

                if (other.gameObject.tag.Contains("DropPoint"))
                {
                    // ������DropPoint�ȊO��DropPoint�ɓ���������
                    if (other.gameObject.tag.Contains(_playerInfo.ID.ToString()) == false)
                    {
                        if (_silkData.SilkCount > 0)
                        {
                            DropSilkEvent dropSilkEvent = new DropSilkEvent()
                            {
                                pos = transform.position,
                            };
                            TypeEventSystem.Instance.Send(dropSilkEvent);
                        }
                        // ���S��Ԃɐݒ肷��
                        StartDead();
                    }
                }
            }

                    */
        #endregion //Obsolete Code

    }

}

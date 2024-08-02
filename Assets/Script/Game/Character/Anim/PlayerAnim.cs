using UnityEngine;
using Mirror;
using UnityEngine.Assertions;

public class PlayerAnim : CharacterAnim
{
    private GameObject _respawnShadow; // �v���C���[���������鎞�̉e
    private SpriteRenderer _respawnShadowSpriteRenderer; // �v���C���[���������鎞�̉e��SpriteRenderer
    private GameObject _bigSpider; // �v���C���[���������邷�鎞�̑傫���w�
    private LineRenderer _respawnLineRenderer; // �v���C���[�������鎞�̋󒆓������鎞�Ɍq�����Ă��鎅
    private float _respawnAnimationTimer;
    private Vector3 _spawnPos;
    private GamePlayer _networkPlayer;

    private void Awake()
    {
        _bigSpider = Instantiate(GameResourceSystem.Instance.GetPrefabResource("BigSpider"), Vector3.zero,
            Quaternion.identity);
        _bigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        _bigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        _respawnLineRenderer = _bigSpider.GetComponentInChildren<LineRenderer>();
        _respawnLineRenderer.positionCount = 2;
        _respawnLineRenderer.startWidth = 0.2f;
        _respawnLineRenderer.endWidth = 0.2f;

        // ��������Ƃ������e�̃v���n�u���C���X�^���X������
        _respawnShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero,
            Quaternion.identity);
        _respawnShadow.transform.localScale = Vector3.zero;
        //TODO �e�̕�����ς���
        _respawnShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �e�𓧖��ɂ���
        _respawnShadowSpriteRenderer = _respawnShadow.GetComponent<SpriteRenderer>();
        _respawnShadowSpriteRenderer.color = Color.clear;

        _respawnAnimationTimer = Global.RESPAWN_TIME;

        TypeEventSystem.Instance.Register<SendInitializedPlayerEvent>
        (
            eve =>
            {
                PlayerInterfaceContainer playerInterfaces =
                    eve.Player.GetComponent<IPlayerInterfaceContainer>().GetContainer();
                int id = playerInterfaces.GetInterface<IPlayerInfo>().ID;
                eve.Player.GetComponent<PlayerAnim>()._spawnPos = (NetWorkRoomManagerExt.singleton as IRoomManager)
                    .GetRespawnPosition(id - 1).position;
            }
        );

        _networkPlayer = GetComponent<GamePlayer>();
    }

    [ClientRpc(includeOwner = false)]
    public override void RpcUpdateAnimation()
    {
        if (_bIsAnimationStopping)
            return;

        switch (_animationType)
        {
            case AnimType.Respawn:
                RpcUpdateRespawnAnimation();
                break;
        }

        _respawnAnimationTimer -= Time.deltaTime;
    }

    /// <summary>
    /// �����A�j���[�V���������Z�b�g����
    /// </summary>
    [ClientRpc]
    protected override void RpcResetAnimation()
    {
        _bigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        _respawnLineRenderer.positionCount = 0;
        _respawnShadow.transform.localScale = Vector3.zero;
        _respawnShadowSpriteRenderer.color = Color.clear;
        _bIsAnimationStopping = true;

        switch (_animationType)
        {
            case AnimType.None:
            {
                _respawnAnimationTimer = 0f;
            }
                break;
            case AnimType.Respawn:
            {
                _respawnAnimationTimer = Global.RESPAWN_TIME;
                _bigSpider.transform.position = _spawnPos + new Vector3(0.0f, 0.0f, 100.0f);
                _respawnShadow.transform.position = _spawnPos;
                // �����A�j���[�V����������������
                transform.position = _bigSpider.transform.position;
                _respawnLineRenderer.positionCount = 2;

                _bIsAnimationStopping = false;
            }
                break;

            // �����ɓ���Ȃ��͂�
            default:
            {
                Assert.IsTrue(true, $"Invalid Animation Type {_animationType}");
            }
                break;
        }
    }

    /// <summary>
    /// �����A�j���[�V�������X�V����֐�
    /// </summary>
    //TODO �J�������ɂ��鎞�ɕύX����\��
    [ClientRpc]
    public void RpcUpdateRespawnAnimation()
    {
        // �A�j���[�V�������I������珉����Ԃɖ߂�
        if (_respawnAnimationTimer <= 0)
        {
            SetAnimationType(AnimType.None);
            return;
        }

        // �����A�j���[�V�����Đ�

        #region Respawn Animation

        // �����A�j���[�V�����O�������̏���
        if (_respawnAnimationTimer >= Global.RESPAWN_TIME * 0.5f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // �����A�j���[�V�����㔼�����̏���
        else
        {
            //TODO
            transform.Translate(-(_bigSpider.transform.position - _spawnPos) * 0.4f * Time.deltaTime, Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            _respawnShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            _respawnShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] spiderThread = new Vector3[2];
            spiderThread[0] = _bigSpider.transform.position;
            spiderThread[1] = transform.position + new Vector3(0.0f, -0.5f, 0.0f);
            _respawnLineRenderer.SetPositions(spiderThread);
        }

        #endregion
    }

    public void StartExplosionAnim()
    {
        _networkPlayer.CmdSpawnDeadAnimation(transform.position);

        SetAnimationType(AnimType.Respawn);
        // �����̌��ʉ��𗬂�
        //AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }
}
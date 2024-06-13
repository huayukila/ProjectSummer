using UnityEngine;
using Character;
using Mirror;

public class PlayerAnim : CharacterAnim
{

    private GameObject mShadow;                        // �v���C���[���������鎞�̉e
    private SpriteRenderer mShadowSpriteRenderer;      // �v���C���[���������鎞�̉e��SpriteRenderer
    private GameObject mBigSpider;                      // �v���C���[���������邷�鎞�̑傫���w�
    private LineRenderer mBigSpiderLineRenderer;       // �v���C���[�������鎞�̋󒆓������鎞�Ɍq�����Ă��鎅
    private GameObject mExplosionPrefab;                // �����A�j���[�V�����v���n�u
    private Player mPlayer;
    private float _respawnAnimationTimer;

    private Vector3 _spawnPos;

    private void Awake()
    {
        mExplosionPrefab = GameResourceSystem.Instance.GetPrefabResource("Explosion");

        mBigSpider = Instantiate(GameResourceSystem.Instance.GetPrefabResource("BigSpider"), Vector3.zero, Quaternion.identity);
        mBigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        mBigSpider.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        mBigSpiderLineRenderer = mBigSpider.GetComponentInChildren<LineRenderer>();
        mBigSpiderLineRenderer.positionCount = 2;
        mBigSpiderLineRenderer.startWidth = 0.2f;
        mBigSpiderLineRenderer.endWidth = 0.2f;

        // ��������Ƃ������e�̃v���n�u���C���X�^���X������
        mShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        mShadow.transform.localScale = Vector3.zero;
        //TODO �e�̕�����ς���
        mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // �e�𓧖��ɂ���
        mShadowSpriteRenderer = mShadow.GetComponent<SpriteRenderer>();
        mShadowSpriteRenderer.color = Color.clear;

        mPlayer = GetComponent<Player>();
        _respawnAnimationTimer = Global.RESPAWN_TIME;

    }
    // Start is called before the first frame update
    void Start()
    {
        PlayerInterfaceContainer playerInterfaces = GetComponent<IPlayerInterfaceContainer>().GetContainer();
        int id = playerInterfaces.GetInterface<IPlayerInfo>().ID;
        _spawnPos = (NetWorkRoomManagerExt.singleton as IRoomManager).GetRespawnPosition(id - 1).position;
    }

    // Update is called once per frame
    void Update()
    {
        switch(mType)
        {
            case AnimType.None:
                break;
            case AnimType.Respawn:
                {
                    RpcUpdateRespawnAnimation();
                }
                break;
        }

    }

    /// <summary>
    /// �����A�j���[�V���������Z�b�g����
    /// </summary>
    [ClientRpc]
    private void RpcResetRespawnAnimation()
    {

        mBigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        mBigSpiderLineRenderer.positionCount = 0;
        mShadow.transform.localScale = Vector3.zero;
        mShadowSpriteRenderer.color = Color.clear;
        isStopped = true;
        mType = AnimType.None;
        _respawnAnimationTimer = Global.RESPAWN_TIME;
    }

    /// <summary>
    /// �����A�j���[�V�������X�V����֐�
    /// </summary>
    //TODO �J�������ɂ��鎞�ɕύX����\��
    [ClientRpc]
    private void RpcUpdateRespawnAnimation()
    {
        _respawnAnimationTimer -= Time.deltaTime;
        // �����A�j���[�V�����O�������̏���
        if (_respawnAnimationTimer >= Global.RESPAWN_TIME / 2.0f)
        {
            mBigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = mBigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // �����A�j���[�V�����㔼�����̏���
        else
        {
            //TODO
            transform.Translate(-(mBigSpider.transform.position - _spawnPos) * 0.4f * Time.deltaTime, Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] spiderThread = new Vector3[2];
            spiderThread[0] = mBigSpider.transform.position;
            spiderThread[1] = transform.position + new Vector3(0.0f, -0.5f, 0.0f);
            mBigSpiderLineRenderer.SetPositions(spiderThread);
        }
    }

    [ClientRpc]
    public void RpcStartRespawnAnim()
    {
        mType = AnimType.Respawn;
        int index = mPlayer.ID - 1;
        mBigSpider.transform.position = _spawnPos + new Vector3(0.0f, 0.0f, 100.0f);
        mShadow.transform.position = _spawnPos;
        // �����A�j���[�V����������������
        transform.position = mBigSpider.transform.position;
        mBigSpiderLineRenderer.positionCount = 2;
        Timer respawnAnimationTimer = new Timer(Time.time,Global.RESPAWN_TIME,
            () =>
            {
                RpcResetRespawnAnimation();
                
            }
            );
        respawnAnimationTimer.StartTimer(this);
        isStopped = false;
    }

    [ClientRpc]
    public void RpcStartExplosionAnim()
    {
        GameObject explosion = Instantiate(mExplosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        // �����̌��ʉ��𗬂�
        //AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }
}

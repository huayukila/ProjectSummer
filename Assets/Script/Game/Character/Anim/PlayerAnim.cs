using UnityEngine;
using Character;

public class PlayerAnim : CharacterAnim
{

    private GameObject mShadow;                        // �v���C���[���������鎞�̉e
    private SpriteRenderer mShadowSpriteRenderer;      // �v���C���[���������鎞�̉e��SpriteRenderer
    private GameObject mBigSpider;                      // �v���C���[���������邷�鎞�̑傫���w�
    private LineRenderer mBigSpiderLineRenderer;       // �v���C���[�������鎞�̋󒆓������鎞�Ɍq�����Ă��鎅
    private GameObject mExplosionPrefab;                // �����A�j���[�V�����v���n�u
    private Player mPlayer;
    private float _respawnAnimationTimer;

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
                    UpdateRespawnAnimation();
                }
                break;
        }

    }

    /// <summary>
    /// �����A�j���[�V���������Z�b�g����
    /// </summary>
    private void ResetRespawnAnimation()
    {

        mBigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        mBigSpiderLineRenderer.positionCount = 0;
        mShadow.transform.localScale = Vector3.zero;
        mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// �����A�j���[�V�������X�V����֐�
    /// </summary>
    //TODO �J�������ɂ��鎞�ɕύX����\��
    private void UpdateRespawnAnimation()
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
            transform.Translate(-(mBigSpider.transform.position - Global.PLAYER_START_POSITIONS[mPlayer.ID - 1]) * 0.4f * Time.deltaTime, Space.World);
            transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
            mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
            mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
            Vector3[] spiderThread = new Vector3[2];
            spiderThread[0] = mBigSpider.transform.position;
            spiderThread[1] = transform.position + new Vector3(0.0f, -0.5f, 0.0f);
            mBigSpiderLineRenderer.SetPositions(spiderThread);
        }
    }

    public void StartRespawnAnim()
    {
        mType = AnimType.Respawn;
        int index = mPlayer.ID - 1;
        mBigSpider.transform.position = Global.PLAYER_START_POSITIONS[index] + new Vector3(0.0f, 0.0f, 100.0f);
        mShadow.transform.position = Global.PLAYER_START_POSITIONS[index];
        // �����A�j���[�V����������������
        transform.position = mBigSpider.transform.position;
        mBigSpiderLineRenderer.positionCount = 2;
        Timer respawnAnimationTimer = new Timer(Time.time,Global.RESPAWN_TIME,
            () =>
            {
                ResetRespawnAnimation();
                isStopped = true;
                mType = AnimType.None;
                _respawnAnimationTimer = Global.RESPAWN_TIME;
            }
            );
        respawnAnimationTimer.StartTimer(this);
        isStopped = false;
    }

    public void StartExplosionAnim()
    {
        GameObject explosion = Instantiate(mExplosionPrefab, transform.position, Quaternion.identity);
        explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
        // �����̌��ʉ��𗬂�
        AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }
}

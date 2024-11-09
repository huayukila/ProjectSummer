using UnityEngine;
using Character;

public class PlayerAnim : CharacterAnim
{
  private readonly static float DARKNESS_RATE = 0.9f;
    private GameObject mShadow;                        
    private SpriteRenderer mShadowSpriteRenderer;      
    private GameObject mBigSpider;                      
    private LineRenderer mBigSpiderLineRenderer;      
    private GameObject mExplosionPrefab;               
    private Player mPlayer;
    private float _respawnAnimationTimer;
    private MPostProcess.MonochromeEffect _deadEffect;
    private MPostProcess.HideEffect _hideEffect;
    private float _deadEffectTimeCnt;
    private readonly float _deadEffectTimeInterval = 1f;

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

        
        mShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        mShadow.transform.localScale = Vector3.zero;

        mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        mShadowSpriteRenderer = mShadow.GetComponent<SpriteRenderer>();
        mShadowSpriteRenderer.color = Color.clear;

        mPlayer = GetComponent<Player>();
        _respawnAnimationTimer = Global.RESPAWN_TIME;

        _deadEffectTimeCnt = 0f;

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

    private void OnDestroy()
    {
      _deadEffect?.Dispose();
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
      if (_respawnAnimationTimer >= Global.RESPAWN_TIME / 2.0f)
      {
        mBigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
        transform.position = mBigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);

        _deadEffectTimeCnt += Time.deltaTime;
        if (_deadEffectTimeCnt >= _deadEffectTimeInterval)
        {
          _deadEffectTimeCnt = 1f;
        }

        _deadEffect?.SetRate(_deadEffectTimeCnt / _deadEffectTimeInterval);
        _hideEffect?.SetFogDense(_deadEffectTimeCnt / _deadEffectTimeInterval * DARKNESS_RATE);
      }
      else
      {
        //TODO
        transform.Translate(-(mBigSpider.transform.position - Global.PLAYER_START_POSITIONS[mPlayer.GetID() - 1]) * 0.4f * Time.deltaTime, Space.World);
        transform.localScale -= new Vector3(0.5f, 0.0f, 0.5f) * 0.4f * Time.deltaTime;
        mShadowSpriteRenderer.color += Color.white * 0.4f * Time.deltaTime;
        mShadow.transform.localScale += Vector3.one * 0.4f * Time.deltaTime * 0.8f;
        Vector3[] spiderThread = new Vector3[2];
        spiderThread[0] = mBigSpider.transform.position;
        spiderThread[1] = transform.position + new Vector3(0.0f, -0.5f, 0.0f);
        mBigSpiderLineRenderer.SetPositions(spiderThread);

        if (_deadEffect != null && _deadEffect.IsActive)
        {
          _deadEffectTimeCnt -= Time.deltaTime;
          if (_deadEffectTimeCnt <= 0f)
          {
            _deadEffectTimeCnt = 0f;
            _deadEffect.SetActive(false);
            _hideEffect?.SetActive(false);
          }

          _deadEffect.SetRate(_deadEffectTimeCnt / _deadEffectTimeInterval);
        }

        _hideEffect?.SetFogDense(_deadEffectTimeCnt / _deadEffectTimeInterval * DARKNESS_RATE);
      }
    }

    public void StartRespawnAnim()
    {
      mType = AnimType.Respawn;
      int index = mPlayer.GetID() - 1;
      mBigSpider.transform.position = Global.PLAYER_START_POSITIONS[index] + new Vector3(0.0f, 0.0f, 100.0f);
      mShadow.transform.position = Global.PLAYER_START_POSITIONS[index];
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

      _deadEffect?.SetActive(true);
      _deadEffectTimeCnt = 0f;

      _hideEffect?.SetActive(true);
      _hideEffect?.SetFogDense(0f);
    }

    public void StartExplosionAnim()
    {
      GameObject explosion = Instantiate(mExplosionPrefab, transform.position, Quaternion.identity);
      explosion.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);
      AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }

    public void SetDeadEffect(MPostProcess.MonochromeEffect effect, MPostProcess.HideEffect hideEffect)
    {
      _deadEffect = effect;
      _hideEffect = hideEffect;
    }
}

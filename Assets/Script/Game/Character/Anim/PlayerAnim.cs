using UnityEngine;
using Character;

public class PlayerAnim : CharacterAnim
{

    private GameObject mShadow;                        // プレイヤーが復活する時の影
    private SpriteRenderer mShadowSpriteRenderer;      // プレイヤーが復活する時の影のSpriteRenderer
    private GameObject mBigSpider;                      // プレイヤーが復活するする時の大きい蜘蛛
    private LineRenderer mBigSpiderLineRenderer;       // プレイヤー復活する時の空中投下する時に繋がっている糸
    private GameObject mExplosionPrefab;                // 爆発アニメーションプレハブ
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

        // 復活するとき現れる影のプレハブをインスタンス化する
        mShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero, Quaternion.identity);
        mShadow.transform.localScale = Vector3.zero;
        //TODO 影の方向を変える
        mShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 影を透明にする
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
    /// 復活アニメーションをリセットする
    /// </summary>
    private void ResetRespawnAnimation()
    {

        mBigSpider.transform.position = Global.GAMEOBJECT_STACK_POS;
        mBigSpiderLineRenderer.positionCount = 0;
        mShadow.transform.localScale = Vector3.zero;
        mShadowSpriteRenderer.color = Color.clear;
    }

    /// <summary>
    /// 復活アニメーションを更新する関数
    /// </summary>
    //TODO カメラを二つにする時に変更する予定
    private void UpdateRespawnAnimation()
    {
        _respawnAnimationTimer -= Time.deltaTime;
        // 復活アニメーション前半部分の処理
        if (_respawnAnimationTimer >= Global.RESPAWN_TIME / 2.0f)
        {
            mBigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = mBigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // 復活アニメーション後半部分の処理
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
        // 復活アニメーションを初期化する
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
        // 爆発の効果音を流す
        AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }
}

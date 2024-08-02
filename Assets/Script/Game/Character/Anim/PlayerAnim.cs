using UnityEngine;
using Mirror;
using UnityEngine.Assertions;

public class PlayerAnim : CharacterAnim
{
    private GameObject _respawnShadow; // プレイヤーが復活する時の影
    private SpriteRenderer _respawnShadowSpriteRenderer; // プレイヤーが復活する時の影のSpriteRenderer
    private GameObject _bigSpider; // プレイヤーが復活するする時の大きい蜘蛛
    private LineRenderer _respawnLineRenderer; // プレイヤー復活する時の空中投下する時に繋がっている糸
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

        // 復活するとき現れる影のプレハブをインスタンス化する
        _respawnShadow = Instantiate(GameResourceSystem.Instance.GetPrefabResource("PlayerShadow"), Vector3.zero,
            Quaternion.identity);
        _respawnShadow.transform.localScale = Vector3.zero;
        //TODO 影の方向を変える
        _respawnShadow.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.up);

        // 影を透明にする
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
    /// 復活アニメーションをリセットする
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
                // 復活アニメーションを初期化する
                transform.position = _bigSpider.transform.position;
                _respawnLineRenderer.positionCount = 2;

                _bIsAnimationStopping = false;
            }
                break;

            // ここに入らないはず
            default:
            {
                Assert.IsTrue(true, $"Invalid Animation Type {_animationType}");
            }
                break;
        }
    }

    /// <summary>
    /// 復活アニメーションを更新する関数
    /// </summary>
    //TODO カメラを二つにする時に変更する予定
    [ClientRpc]
    public void RpcUpdateRespawnAnimation()
    {
        // アニメーションが終わったら初期状態に戻す
        if (_respawnAnimationTimer <= 0)
        {
            SetAnimationType(AnimType.None);
            return;
        }

        // 復活アニメーション再生

        #region Respawn Animation

        // 復活アニメーション前半部分の処理
        if (_respawnAnimationTimer >= Global.RESPAWN_TIME * 0.5f)
        {
            _bigSpider.transform.Translate(new Vector3(0.0f, 0.0f, -20.0f * Time.deltaTime), Space.World);
            transform.position = _bigSpider.transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        }
        // 復活アニメーション後半部分の処理
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
        // 爆発の効果音を流す
        //AudioManager.Instance.PlayFX("BoomFX", 0.7f);
    }
}
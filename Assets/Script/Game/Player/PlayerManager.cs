using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    Timer _player1Timer;        // プレイヤー1の待機タイマー
    Timer _player2Timer;        // プレイヤー2の待機タイマー
    GameObject _playerPrefab;
    /// <summary>
    /// プレイヤーの復活タイミングをチェックする
    /// </summary>
    private void RespawnCheck()
    {
        // プレイヤー1が死んだら
        if (!GameManager.Instance.playerOne.activeSelf)
        {
            // 新しいタイマーを生成する
            if (_player1Timer == null)
            {
                _player1Timer = new Timer();
                _player1Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer1();
                    }
                    );
            }
            // 待機時間が終わったら
            else if (_player1Timer.IsTimerFinished())
            {
                _player1Timer = null;
            }
        }
        // プレイヤー2が死んだら
        if (!GameManager.Instance.playerTwo.activeSelf)
        {
            // 新しいタイマーを生成する
            if (_player2Timer == null)
            {
                _player2Timer = new Timer();
                _player2Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer2();
                    }
                    );
            }
            // 待機時間が終わったら
            else if (_player2Timer.IsTimerFinished())
            {
                _player2Timer = null;
            }
        }
    }    
    
    /// <summary>
    /// プレイヤー1を復活させる
    /// </summary>
    private void RespawnPlayer1()
    {
        GameManager.Instance.playerOne.GetComponent<Player1Control>().Respawn();
    }

    /// <summary>
    /// プレイヤー2を復活させる
    /// </summary>
    private void RespawnPlayer2()
    {
        GameManager.Instance.playerTwo.GetComponent<Player2Control>().Respawn();
    }

    protected override void Awake()
    {
        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
        GameObject player1 = Instantiate(_playerPrefab, Global.PLAYER1_START_POSITION, Quaternion.identity);
        player1.AddComponent<Player1Control>().SetAreaColor(Global.PLAYER_ONE_AREA_COLOR);
        player1.name = "Player1";
        GameObject player2 = Instantiate(_playerPrefab, Global.PLAYER2_START_POSITION, Quaternion.identity);
        player2.AddComponent<Player2Control>().SetAreaColor(Global.PLAYER_TWO_AREA_COLOR);
        player2.transform.forward = Vector3.back;
        player2.name = "Player2";
        TypeEventSystem.Instance.Register<AddScoreEvent>(e =>
        {
            ScoreItemManager.Instance.SetReachGoalProperties(e.playerID);

        }).UnregisterWhenGameObjectDestroyde(gameObject);

        TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
        {
            ScoreItemManager.Instance.DropGoldenSilk(e.dropMode);

        }).UnregisterWhenGameObjectDestroyde(gameObject);
        TypeEventSystem.Instance.Register<PickSilkEvent>(e =>
        {
            ScoreItemManager.Instance.SetGotSilkPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyde(gameObject);
    }

    private void Update()
    {
        RespawnCheck();
    }
}


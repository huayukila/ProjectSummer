using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    Timer _player1Timer;        // プレイヤー1の待機タイマー
    Timer _player2Timer;        // プレイヤー2の待機タイマー

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
        base.Awake();
    }

    private void Update()
    {
        RespawnCheck();
    }
}


using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    Timer _player1Timer;
    Timer _player2Timer;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        RespawnCheck();
    }

    private void RespawnCheck()
    {
        if (!GameManager.Instance.playerOne.activeSelf)
        {
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
            else if (_player1Timer.IsTimerFinished())
            {
                _player1Timer = null;
            }
        }
        if (!GameManager.Instance.playerTwo.activeSelf)
        {
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
            else if (_player2Timer.IsTimerFinished())
            {
                _player2Timer = null;
            }
        }
    }    
    
    private void RespawnPlayer1()
    {
        GameManager.Instance.playerOne.GetComponent<Player1Control>().Respawn();
    }

    private void RespawnPlayer2()
    {
        GameManager.Instance.playerTwo.GetComponent<Player2Control>().Respawn();
    }

}


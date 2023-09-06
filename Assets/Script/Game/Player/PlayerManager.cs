using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject player1;
    public GameObject player2;

    float _player1RespawnTimer;
    float _player2RespawnTimer;

    protected override void Awake()
    {
        base.Awake();
        _player1RespawnTimer = 0.0f;
        _player2RespawnTimer = 0.0f;

    }

    private void Update()
    {
        if(!player1.activeSelf)
        {
            _player1RespawnTimer += Time.deltaTime;
            if(_player1RespawnTimer >= 5.0f)
            {
                RespawnPlayer1();
                _player1RespawnTimer = 0.0f;
            }
        }
        if(!player2.activeSelf)
        {
            _player2RespawnTimer += Time.deltaTime;
            if (_player2RespawnTimer >= 5.0f)
            {
                RespawnPlayer2();
                _player2RespawnTimer = 0.0f;
            }
        }
    }

    private void RespawnPlayer1()
    {
        player1.GetComponent<Player1Control>().Respawn();
    }

    private void RespawnPlayer2()
    {
        player2.GetComponent<Player2Control>().Respawn();

    }
}


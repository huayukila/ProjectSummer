using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem:Singleton<ScoreSystem>
{
    private int player1Score;
    private int player2Score;
    void Start()
    {
        player1Score = 0;
        player2Score = 0;
    }

    [Obsolete]
    public void AddScore(int playerNumber) //ScoreSystem.AddScore(1or2)�ŃX�R�A�𑝉�����B
    {
        if (playerNumber == 1)
        {
            player1Score ++;

        }
        else if (playerNumber == 2)
        {
            player2Score ++;
        }
    }

    /// <summary>
    /// �w�肵���v���C���[�ɃX�R�A�����_����B
    /// </summary>
    /// <param name="playerNumber">�v���C���[�̔ԍ������</param>
    /// <param name="score">��������X�R�A�����</param>
    public void AddScore(int playerNumber, int score) 
    {
        if (playerNumber == 1)
        {
            player1Score += score;

        }
        else if (playerNumber == 2)
        {
            player2Score += score;
        }
    }
    public int GetPlayer1Score()
    {

        return player1Score;
    }
    public int GetPlayer2Score()
    {
        return player2Score;
    }

}

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
    public void AddScore(int playerNumber) //ScoreSystem.AddScore(1or2)でスコアを増加する。
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
    /// 指定したプレイヤーにスコアを加点する。
    /// </summary>
    /// <param name="playerNumber">プレイヤーの番号を入力</param>
    /// <param name="score">増加するスコアを入力</param>
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

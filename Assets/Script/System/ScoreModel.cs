using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ScoreModel : Singleton<ScoreModel>
{
    private float[] playerScore;

    public void SetScore()
    {
        playerScore = PolygonPaintManager.Instance.GetPlayersAreaPercent();
    }
    public float GetPlayer1Score()
    {
        if (playerScore != null && playerScore.Length > 0)
        {
            float value = Mathf.Round(playerScore[0] * 100f) / 100f;//小数点以下2桁に転換
            return value;
        }
        else
        {
            return 0f;
        }
    }
    public float GetPlayer2Score()
    {
        if (playerScore != null && playerScore.Length > 0)
        {
            float value = Mathf.Round(playerScore[1] * 100f) / 100f;//小数点以下2桁に転換
            return value;
        }
        else
        {
            return 0f;
        }
    }
    public void ResetScore()
    {
        if (playerScore != null)
        {
            for (int i = 0; i < playerScore.Length; i++)
            {
                playerScore[i] = 0f;
            }
        }
    }
}

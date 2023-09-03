using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
public class ScoreSystem
{
    private int player1Score;
    private int player2Score;

    public void InitializeScores()
    {
        player1Score = 0;
        player2Score = 0;
    }

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
    public int GetPlayer1Score()
    {
    
        return player1Score;
    }
    public int GetPlayer2Score()
    {
        return player2Score;
    }
}

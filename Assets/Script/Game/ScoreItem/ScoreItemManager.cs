using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class ScoreItemManager : Singleton<ScoreItemManager>
{   
    private Timer _goldenSilkSpawnTimer;        // 金の網を生成することを管理するタイマー
    private GameObject _gotSilkPlayer;          // 金の網を持っているプレイヤー

    public GameObject inSpaceSilk;              // 金の網
    public GameObject goalPoint;                // ゴール
    public Material goldMaterial;               // 金の網の材質

    /// <summary>
    /// 金の網の生成位置を決める関数
    /// 未完成のため、固定位置に生成している
    /// </summary>
    /// <returns></returns>
    private Vector3 GetInSpaceRandomPosition()
    {
        // temp pos
        return new Vector3(0.0f,0.64f,5.0f);
    }

    /// <summary>
    /// 金の網はゴールまで運搬された時の操作をする
    /// </summary>
    public void SetReachGoalProperties()
    {
        // プレイヤー1が運搬したら
        if(_gotSilkPlayer == GameManager.Instance.playerOne)
        {
            // player1 get 1 point
            Debug.Log("Player 1 get 1 point");
            ScoreSystem.Instance.AddScore(1);
        }
        // プレイヤー2が運搬したら
        if (_gotSilkPlayer == GameManager.Instance.playerTwo)
        {
            // player2 get 1 point
            Debug.Log("Player 2 get 1 point");
            ScoreSystem.Instance.AddScore(2);
        }

        inSpaceSilk.SetActive(false);
        goalPoint.SetActive(false);
        _gotSilkPlayer = null;

        _gotSilkPlayer = null;
        // 新しいタイマーを生成する
        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f, 
            () => 
            {   inSpaceSilk.transform.position = GetInSpaceRandomPosition();
                inSpaceSilk.SetActive(true); 
            }
            );
    }

    /// <summary>
    /// ゴールの位置を生成する
    /// 未完成のため、固定位置に生成している
    /// </summary>
    private void SetGoalPoint()
    {
        // temp pos
        goalPoint.transform.position = new Vector3(10.0f,0.64f,10.0f);
        goalPoint.SetActive(true);
    }

    /// <summary>
    /// 金の網を持っているプレイヤーの設定をする
    /// </summary>
    /// <param name="ob"></param>
    public void SetGotSilkPlayer(GameObject ob)
    {
        if(_gotSilkPlayer == null)
        {
            _gotSilkPlayer = ob;
        }
        // 持っているプレイヤーの材質を変える（区別するため）
        _gotSilkPlayer.GetComponent<Renderer>().material = goldMaterial;
        inSpaceSilk.SetActive(false);
        SetGoalPoint();
    }

    /// <summary>
    /// 金の網を持っていたプレイヤーが死んだら金の網をドロップする
    /// </summary>
    public void DropGoldenSilk()
    {
        if(_gotSilkPlayer != null)
        {
            inSpaceSilk.transform.position = _gotSilkPlayer.transform.position;
            _gotSilkPlayer = null;
            inSpaceSilk.SetActive(true);
            goalPoint.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤーが金の網を持っているかどうかをチェックする
    /// </summary>
    /// <param name="ob">プレイヤー</param>
    /// <returns>持っていたらtrueを返す、それ以外はfalseを返す</returns>
    public bool IsGotSilk(GameObject ob)
    {
        return _gotSilkPlayer == ob;
    }

    protected override void Awake()
    {
        base.Awake();
        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f, () => { inSpaceSilk.SetActive(true); });
        inSpaceSilk.SetActive(false);
        goalPoint.SetActive(false);
    }
    private void Update()
    {
        if (_goldenSilkSpawnTimer != null)
        {
            if (_goldenSilkSpawnTimer.IsTimerFinished())
            {
                _goldenSilkSpawnTimer = null;
            }
        }
    }

}

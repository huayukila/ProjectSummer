using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class ScoreItemManager : Singleton<ScoreItemManager>
{   
    Timer _goldenSilkSpawnTimer;
    GameObject _gotSilkPlayer;

    public GameObject inSpaceSilk;
    public GameObject goalPoint;
    public Material goldMaterial;

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
        if(_goldenSilkSpawnTimer != null)
        {
            if(_goldenSilkSpawnTimer.IsTimerFinished())
            {
                _goldenSilkSpawnTimer = null;
            }
        }
    }

    private Vector3 GetInSpaceRandomPosition()
    {
        // temp pos
        return new Vector3(0.0f,0.64f,5.0f);
    }

    public void SetReachGoalProperties()
    {
        if(_gotSilkPlayer == GameManager.Instance.playerOne)
        {
            // player1 get 1 point
            Debug.Log("Player 1 get 1 point");
        }

        if (_gotSilkPlayer == GameManager.Instance.playerTwo)
        {
            // player2 get 1 point
            Debug.Log("Player 2 get 1 point");
        }

        inSpaceSilk.SetActive(false);
        goalPoint.SetActive(false);

        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f, 
            () => 
            {   inSpaceSilk.transform.position = GetInSpaceRandomPosition();
                inSpaceSilk.SetActive(true); 
            }
            );
    }

    private void SetGoalPoint()
    {
        // temp pos
        goalPoint.transform.position = new Vector3(10.0f,0.64f,10.0f);
        goalPoint.SetActive(true);
    }
    public void SetGotSilkPlayer(GameObject ob)
    {
        if(_gotSilkPlayer == null)
        {
            _gotSilkPlayer = ob;
        }

        _gotSilkPlayer.GetComponent<Renderer>().material = goldMaterial;

        inSpaceSilk.SetActive(false);
        SetGoalPoint();
    }

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

    public bool IsGotSilk(GameObject ob)
    {
        return _gotSilkPlayer == ob;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndSceneUIDirector : MonoBehaviour
{
    public GameObject redScore;
    public GameObject yellowScore;
    public GameObject winner;
    public GameObject draw;
    public GameObject winYellow;
    public GameObject winRed;
    public GameObject pressAnyBtn;

    float timer;
    float alphaSetTMP;
    float alphaSet;
    float alphachangeTMP;
    float alphachange;
    float localScaleSetTMP;
    float localScaleChangeTMP;
    float localScaleSet;
    float localScaleChange;

    //Blink Initialization--------------------
    public float blinkSpeed = 0.02f;       //ボタンの点滅変化の速度
    public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    float blinkTimer = 0f;


    private void Start()
    {
        timer = 3f;

        alphaSetTMP = 0.0f;
        alphachangeTMP = 0.025f;
        localScaleSetTMP = 20f;
        localScaleChangeTMP = 0.5f;

        alphaSet = 0.0f;
        alphachange = 0.025f;
        localScaleSet = 20f;
        localScaleChange = 0.5f;


        UISystem.SetPos(redScore,1500f,0f);
        UISystem.SetPos(yellowScore, 1500f, 0f);

        UISystem.SetLocalScale(draw, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        UISystem.SetAlphaTMP(draw,alphaSet);

        UISystem.SetLocalScale(winner, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        UISystem.SetAlphaTMP(winner, alphaSet);

        UISystem.SetLocalScale(winRed, localScaleSet, localScaleSet, localScaleSet);
        UISystem.SetAlpha(winRed, alphaSet);

        UISystem.SetLocalScale(winYellow, localScaleSet, localScaleSet, localScaleSet);
        UISystem.SetAlpha(winYellow, alphaSet);

        UISystem.DisplayOff(pressAnyBtn);

        //UISystem.SetLocalScale(pressAnyBtn, 10f, 10f, 10f);
        //UISystem.SetAlphaTMP(pressAnyBtn, alphaSet);

        //Debug.Log(redScore.transform.position);


    }
    private void Update()
    {
        this.redScore.GetComponent<TextMeshProUGUI>().text = "RED SCORE: " + ScoreSystem.Instance.GetPlayer1Score().ToString();　  //テキストの内容
        this.yellowScore.GetComponent<TextMeshProUGUI>().text = "YELLOW SCORE: " + ScoreSystem.Instance.GetPlayer2Score().ToString();　  //テキストの内容 

        
    }
    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        UISystem.MoveToLeft(redScore, 900, 100);
        if (timer <= 2.6f)
        {
            UISystem.MoveToLeft(yellowScore, 900, 100);
        }
        if (timer <= 2.0f)
        {
            if (ScoreSystem.Instance.GetPlayer1Score() == ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppearTMP(draw);
            }
            else //if(ScoreSystem.Instance.GetPlayer1Score() >= ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppearTMP(winner);
                
            }      
        }
        if (timer <= 1.4f)
        {
            if(ScoreSystem.Instance.GetPlayer1Score() > ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppear(winRed);
            }
            if (ScoreSystem.Instance.GetPlayer1Score() < ScoreSystem.Instance.GetPlayer2Score())
            {
                TurnSmallAndAppear(winYellow);
            }         
        }
        if (timer <= 0.5f) 
        {
            UISystem.DisplayOn(pressAnyBtn);
            Blink();
            if (Input.anyKeyDown)          //TitleSceneへ切り替え
            {
                TypeEventSystem.Instance.Send<TitleSceneSwitch>();
            }
        }
    }
    void TurnSmallAndAppearTMP(GameObject ui)
    {
        alphaSetTMP += alphachangeTMP;
        UISystem.SetAlphaTMP(ui, alphaSetTMP);
        if (localScaleSetTMP > 1)
        {
            localScaleSetTMP -= localScaleChangeTMP;
            UISystem.SetLocalScale(ui, localScaleSetTMP, localScaleSetTMP, localScaleSetTMP);
        }
    }
    void TurnSmallAndAppear(GameObject ui)
    {
        alphaSet += alphachange;
        UISystem.SetAlpha(ui, alphaSet);
        if (localScaleSet > 1)
        {
            localScaleSet -= localScaleChange;
            UISystem.SetLocalScale(ui, localScaleSet, localScaleSet, localScaleSet);
        }
    }
    private void Blink()
    {
        blinkTimer += Time.fixedDeltaTime;

        Color newColor = pressAnyBtn.GetComponent<TextMeshProUGUI>().color;

        if (blinkTimer < blinkInterval * 0.5f)
        {
            newColor.a -= blinkSpeed;
        }
        else
        {
            newColor.a += blinkSpeed;
            if (newColor.a >= 1.0f)
            {
                newColor.a = 1.0f;
                blinkTimer = 0f;
            }
        }
        pressAnyBtn.GetComponent<TextMeshProUGUI>().color = newColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;
    public bool shineOnOff = true;         //ブタン点滅のスイッチ
    public float blinkSpeed = 0.02f;       //ボタンの点滅変化の速度
    public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    float blinkTimer = 0f;
    //float speed = 1f;　　　　　　　　　　//テスト用


    private void FixedUpdate()
    {
        if(shineOnOff==true)
        {
            //テスト用‐‐‐‐‐‐‐‐
            //blinkTimer += Time.fixedDeltaTime;
            //Debug.Log(CycleThroughTimer());
            // UISystem.BlinkTMP(pressBtn, blinkTimer, blinkSpeed, blinkInterval);

            blinkTimer += Time.fixedDeltaTime;

            Color newColor = pressBtn.GetComponent<TextMeshProUGUI>().color;

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
            pressBtn.GetComponent<TextMeshProUGUI>().color = newColor;
        }
    }
    void Update()
    {
        if (Input.anyKeyDown)          //MenuSceneへ切り替え
        {
            TypeEventSystem.Instance.Send<MenuSceneSwitch>();
        }
    }

    //private float CycleThroughTimer()　　　　　　　　　//テスト用‐‐‐‐‐‐‐‐
    //{
    //    blinkTimer += speed*Time.fixedDeltaTime;
    //    if (blinkTimer > 1f)
    //    {
    //        blinkTimer = 1f;
    //        speed = -1f;
    //    }
    //    else if (blinkTimer < 0f) 
    //    {
    //        blinkTimer = 0f;
    //        speed = 1f;
    //    }
    //    return blinkTimer;
    //}
}

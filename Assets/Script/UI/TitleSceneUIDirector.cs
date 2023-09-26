using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject startBtn;
    public bool shineOnOff = true;         //ブタン点滅のスイッチ
    public float blinkSpeed = 0.02f;       //ボタンの点滅変化の速度
    public float blinkInterval = 1.5f;       //ボタンの点滅一往復の時間
    float blinkTimer = 0f;

    private void FixedUpdate()
    {
        if(shineOnOff==true)
        {
            blinkTimer += Time.fixedDeltaTime;

            Color newColor = startBtn.GetComponent<Image>().color;

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
            startBtn.GetComponent<Image>().color = newColor;
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))          //MenuSceneへ切り替え
        {
            TypeEventSystem.Instance.Send<MenuSceneSwitch>();
        }
    }
}

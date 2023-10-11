using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject pressBtn;
    public bool shineOnOff = true;         //u^_ΕΜXCb`
    public float blinkSpeed = 0.02f;       //{^Μ_ΕΟ»Μ¬x
    public float blinkInterval = 1.5f;       //{^Μ_ΕκΜΤ
    float blinkTimer = 0f;
    //float speed = 1f;@@@@@@@@@@//eXgp


    private void FixedUpdate()
    {
        if(shineOnOff==true)
        {
            //eXgp]]]]]]]]
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
        if (Input.anyKeyDown)          //MenuSceneΦΨθΦ¦
        {
            TypeEventSystem.Instance.Send<MenuSceneSwitch>();
        }
    }

    //private float CycleThroughTimer()@@@@@@@@@//eXgp]]]]]]]]
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

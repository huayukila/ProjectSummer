using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartBtnCon : MonoBehaviour
{
    private Image image;
    private bool shineOnOff = true;
    public float blinkSpeed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Color newColor = image.color;

        if (shineOnOff)
        {
            newColor.a -= blinkSpeed;
            if (newColor.a <= 0.4f)
            {
                newColor.a = 0.4f;
                shineOnOff = false;
            }     
        }
        else
        {
            newColor.a += blinkSpeed;
            if (newColor.a >= 1.0f) 
            {
                newColor.a = 1.0f;
                shineOnOff = true;
            }
        }
        

        image.color = newColor;
    }
}

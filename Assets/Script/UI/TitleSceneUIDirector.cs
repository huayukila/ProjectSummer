using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneUIDirector : MonoBehaviour
{
    public GameObject startBtn;
    public bool shineOnOff = true;         //�u�^���_�ł̃X�C�b�`
    public float blinkSpeed = 0.02f;       //�{�^���̓_�ŕω��̑��x
    public float blinkInterval = 1.5f;       //�{�^���̓_�ňꉝ���̎���
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
        if (Input.GetKeyDown(KeyCode.Return))          //MenuScene�֐؂�ւ�
        {
            TypeEventSystem.Instance.Send<MenuSceneSwitch>();
        }
    }
}

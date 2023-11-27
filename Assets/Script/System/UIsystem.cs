using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UISystem
{
    public static void DisplayOn(GameObject ui)  //UI��\��
    {
        ui.SetActive(true);
    }
    public static void DisplayOff(GameObject ui)//UI���\���E�B��
    {
        ui.SetActive(false);
    }
    public static void MoveToRight(GameObject ui, float x, float speed)//UI���E�Ɉړ�(GameObject,�ړ��̏I�_��,�ړ��̃X�s�[�h)
    {
        if (ui.transform.position.x <= x)
        {
            ui.transform.Translate(speed, 0, 0);
        }
    }
    public static void MoveToLeft(GameObject ui,float x,float speed)//UI�����Ɉړ�(GameObject,�ړ��̏I�_��,�ړ��̃X�s�[�h)
    {
        if (ui.transform.position.x >= x)
        {
            ui.transform.Translate(-speed, 0, 0);
        }    
    }
    public static float GetPositionX(GameObject ui)//UI�̂����W���l��
    {
        float x1 = ui.transform.position.x;
        return x1;
    }
    public static float GetPositionY(GameObject ui)//UI�̂����W���l��
    {
        float y1 = ui.transform.position.y;
        return y1;
    }
    /// <summary>
    /// ui���݂̍��W(x1,y1)�Ɋ�Â���ui��V�������W(x2,y2)�ɏ���������
    /// </summary>
    /// <param name="ui">GameObject</param>
    /// <param name="x2">�V�������Wx2</param>
    /// <param name="y2">�V�������Wy2</param>
    public static void SetPos(GameObject ui, float x2, float y2)
    {
        ui.transform.position = new Vector3(GetPositionX(ui) + x2, GetPositionY(ui) + y2, 0f);
    }
    public static void SetLocalScale(GameObject ui,float x,float y,float z)//UI�̑傫����ύX
    {
        ui.transform.localScale=new Vector3(x,y,z);
    }
    public static void SetAlphaTMP(GameObject ui,float alpha)//UI�̓����x��ύX
    {
        Color colorUI = ui.GetComponent<TextMeshProUGUI>().color;
        colorUI.a = alpha;
        ui.GetComponent<TextMeshProUGUI>().color=colorUI;
    }
    public static void SetAlpha(GameObject ui, float alpha)//UI�̓����x��ύX
    {
        Color colorUI = ui.GetComponent<Image>().color;
        colorUI.a = alpha;
        ui.GetComponent<Image>().color = colorUI;
    }
}

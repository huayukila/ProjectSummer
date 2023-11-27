using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class UISystem
{
    public static void DisplayOn(GameObject ui)  //UIを表示
    {
        ui.SetActive(true);
    }
    public static void DisplayOff(GameObject ui)//UIを非表示・隠す
    {
        ui.SetActive(false);
    }
    public static void MoveToRight(GameObject ui, float x, float speed)//UIを右に移動(GameObject,移動の終点ｘ,移動のスピード)
    {
        if (ui.transform.position.x <= x)
        {
            ui.transform.Translate(speed, 0, 0);
        }
    }
    public static void MoveToLeft(GameObject ui,float x,float speed)//UIを左に移動(GameObject,移動の終点ｘ,移動のスピード)
    {
        if (ui.transform.position.x >= x)
        {
            ui.transform.Translate(-speed, 0, 0);
        }    
    }
    public static float GetPositionX(GameObject ui)//UIのｘ座標を獲得
    {
        float x1 = ui.transform.position.x;
        return x1;
    }
    public static float GetPositionY(GameObject ui)//UIのｙ座標を獲得
    {
        float y1 = ui.transform.position.y;
        return y1;
    }
    /// <summary>
    /// ui現在の座標(x1,y1)に基づいてuiを新しい座標(x2,y2)に初期化する
    /// </summary>
    /// <param name="ui">GameObject</param>
    /// <param name="x2">新しい座標x2</param>
    /// <param name="y2">新しい座標y2</param>
    public static void SetPos(GameObject ui, float x2, float y2)
    {
        ui.transform.position = new Vector3(GetPositionX(ui) + x2, GetPositionY(ui) + y2, 0f);
    }
    public static void SetLocalScale(GameObject ui,float x,float y,float z)//UIの大きさを変更
    {
        ui.transform.localScale=new Vector3(x,y,z);
    }
    public static void SetAlphaTMP(GameObject ui,float alpha)//UIの透明度を変更
    {
        Color colorUI = ui.GetComponent<TextMeshProUGUI>().color;
        colorUI.a = alpha;
        ui.GetComponent<TextMeshProUGUI>().color=colorUI;
    }
    public static void SetAlpha(GameObject ui, float alpha)//UIの透明度を変更
    {
        Color colorUI = ui.GetComponent<Image>().color;
        colorUI.a = alpha;
        ui.GetComponent<Image>().color = colorUI;
    }
}

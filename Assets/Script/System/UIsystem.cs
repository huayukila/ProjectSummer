using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIsystem : MonoBehaviour
{
    public static void DisplayOn(GameObject ui)  //UIを表示
    {
        if (ui.active == false)
        {
            ui.active = true;
        }
    }
    public static void DisplayOff(GameObject ui)//UIを非表示
    {
        if (ui.active == true)
        {
            ui.active = false;
        }
    }
    public static void MoveToRight(GameObject ui)//UIを右に移動
    {

    }
    public static void MoveToLeft(GameObject ui)//UIを左に移動
    {

    }
    public static void TurnGray(GameObject ui)
    {

    }
}

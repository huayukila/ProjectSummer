using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIsystem : MonoBehaviour
{
    public static void DisplayOn(GameObject ui)  //UI��\��
    {
        if (ui.active == false)
        {
            ui.active = true;
        }
    }
    public static void DisplayOff(GameObject ui)//UI���\��
    {
        if (ui.active == true)
        {
            ui.active = false;
        }
    }
    public static void MoveToRight(GameObject ui)//UI���E�Ɉړ�
    {

    }
    public static void MoveToLeft(GameObject ui)//UI�����Ɉړ�
    {

    }
    public static void TurnGray(GameObject ui)
    {

    }
}

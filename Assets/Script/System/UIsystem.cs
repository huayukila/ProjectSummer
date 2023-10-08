using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIsystem : MonoBehaviour
{
    public static void DisplayOn(GameObject ui)  //UI‚ğ•\¦
    {
        if (ui.active == false)
        {
            ui.active = true;
        }
    }
    public static void DisplayOff(GameObject ui)//UI‚ğ”ñ•\¦
    {
        if (ui.active == true)
        {
            ui.active = false;
        }
    }
    public static void MoveToRight(GameObject ui)//UI‚ğ‰E‚ÉˆÚ“®
    {

    }
    public static void MoveToLeft(GameObject ui)//UI‚ğ¶‚ÉˆÚ“®
    {

    }
    public static void TurnGray(GameObject ui)
    {

    }
}

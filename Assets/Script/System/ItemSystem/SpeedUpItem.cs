using UnityEngine;
public class SpeedUpItem : Item
{
    float speed = 10f;
    
    public override void Action()
    {
        durationTime = 2f;
        base.Action();
        Debug.Log("yeah");
    }

    public override void CallBack()
    {
        Debug.Log("hello");
    }
}

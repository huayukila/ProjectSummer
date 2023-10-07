using System.Collections;
using UnityEngine;

public interface IItem
{
    public void Action();
}

public abstract class Item :MonoBehaviour, IItem 
{
    protected float durationTime = 0f;
    public virtual void Action() {
        if(durationTime>0f)
        {
            StartCoroutine(CallBackEnum());
        }
    }

    public abstract void CallBack();
    IEnumerator CallBackEnum()
    {
        yield return new WaitForSeconds(durationTime);
        CallBack();
        yield return null;
    }
}

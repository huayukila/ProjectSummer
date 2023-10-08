using UnityEngine;

//使用kit的命名空间
using Kit;

public class ActionExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("StartTime:" + Time.time);
        //延时回调（高级计时器）,第一个参数输入时间，第二个参数输入委托
        ActionKit.Delay(3.0f, () =>
        {
            Debug.Log("ActionFinishTime:"+Time.time);
        }).Start(this);
        //一定记得使用.Start()
        //需要传入一个继承自monobehavior，并且是挂载在游戏场景中的脚本

        //动作序列,由Sequence开头，其余与延时回调无差
        ActionKit.Sequence().
            Delay(4.0f, () =>{
                Debug.Log("StartSequence:" + Time.time);
            }).Delay(6.0f, () =>
            {
                Debug.Log("SequenceFinishTime:" + Time.time);
            }).Start(this);
    }
}

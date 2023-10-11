using UnityEngine;

public class ItemExample : Item
{
    //在OnUse里写道具逻辑比如通过player给玩家加速，修改玩家状态之类
    public override void OnUse(Player player)
    {
        Debug.Log("this is a testitem");
    }
}

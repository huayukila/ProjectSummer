using UnityEngine;
using Character;
//创建资源文件用的属性标签，对准item文件夹右键点击对应按钮生成资源文件
//将资源文件拖入Resources文件夹内的ItemTable中对应的列表中
[CreateAssetMenu(menuName = "ItemSystem/ItemExample", fileName = "ItemExample")]
public class ItemExample : ThrowItem
{
    //在OnUse里写道具逻辑比如通过player给玩家加速，修改玩家状态之类
     public override void OnUse(Player player)
     {
         //    例如：
         //    player.MaxSpeed = 1.5;
         //    player.state = 无敌;
    
         Debug.Log("this is a testitem");
     }
}

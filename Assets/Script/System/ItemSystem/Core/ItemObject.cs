using UnityEngine;

/// <summary>
/// マップで生成するitem
/// </summary>
public class ItemObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        IPlayer2ItemSystem itemInterface = other.gameObject.GetComponent<IPlayer2ItemSystem>();
        //プレイヤーではない場合、何もしない
        if (itemInterface == null)
            return;
        //item持っている場合、何もしない
        if (itemInterface.HadItem())
            return;
        //
        TypeEventSystem.Instance.Send(new PlayerGetItem()
        {
            player = other.gameObject.GetComponent<IPlayer2ItemSystem>(),
        });

        //道具生成
       // ItemSystem.Instance.SpawnItem(Vector3.zero);
        
    }
}

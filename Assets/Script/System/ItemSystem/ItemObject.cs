using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        var Player = other.gameObject.GetComponent<TPlayer>();
        if (Player!=null)
        {
            if(!Player.isHadItem)
            {
                TypeEventSystem.Instance.Send<PlayerGetItem>(new PlayerGetItem()
                {
                    player = Player
                });
            }
        }
    }
}

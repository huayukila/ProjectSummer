using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.GetComponent<IPlayer2ItemSystem>().HadItem())
        {
            TypeEventSystem.Instance.Send(new PlayerGetItem()
            {
                player=other.gameObject.GetComponent<IPlayer2ItemSystem>(),
            });
        }
    }
}

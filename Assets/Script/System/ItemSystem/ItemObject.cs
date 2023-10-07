using UnityEngine;

public class ItemObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.gameObject.GetComponent<TPlayer>())
        {
            TypeEventSystem.Instance.Send<PlayerGetItem>(new PlayerGetItem()
            {
                player = other.gameObject.GetComponent<IPlayer>()
            });
        }
    }
}

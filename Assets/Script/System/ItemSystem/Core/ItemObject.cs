using UnityEngine;

/// <summary>
/// �}�b�v�Ő�������item
/// </summary>
public class ItemObject : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        IPlayer2ItemSystem itemInterface = other.gameObject.GetComponent<IPlayer2ItemSystem>();
        //�v���C���[�ł͂Ȃ��ꍇ�A�������Ȃ�
        if (itemInterface == null)
            return;
        //item�����Ă���ꍇ�A�������Ȃ�
        if (itemInterface.HadItem())
            return;
        //
        TypeEventSystem.Instance.Send(new PlayerGetItem()
        {
            player = other.gameObject.GetComponent<IPlayer2ItemSystem>(),
        });

        //�����
       // ItemSystem.Instance.SpawnItem(Vector3.zero);
        
    }
}

using Character;
using Unity.Mathematics;
using UnityEngine;
/// <summary>
/// ������A�C�e��
/// </summary>
public class ThrowItem: ItemBase
{
    //������Item��prefab
    public GameObject ThrowObj;

    //������Item�ɂ́A��ʓI����OnUse���\�b�h��override�̕K�v���Ȃ�
    public override void OnUse(Player player)
    {
        base.OnUse(player);
        Instantiate(ThrowObj, player.transform.position, 
            quaternion.identity);
    }
}
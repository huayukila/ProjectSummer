using System;
using UnityEngine;
using Character;

public interface IItem
{
    //IPlayer2ItemSystem�p�֐�
    void Use(Player player);
}

[Serializable]
public class ItemBase : ScriptableObject, IItem
{
    public string itemName;
    public int id;

    void IItem.Use(Player player)
    {
        OnUse(player);
    }

    //Item���ۂ̃��W�b�N�R�[�h�͂����ɏ����Ă�������
    public virtual void OnUse(Player player)
    {
        Debug.Log(itemName); //�ЂƂ܂��f�o�b�O
    }

    protected ItemBase()
    {
        itemName = GetType().Name;
    }
}
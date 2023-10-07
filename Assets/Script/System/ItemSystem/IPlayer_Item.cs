using UnityEngine;

public interface IPlayer_Item
{
    public void UseItem();
    public void GetItem(IItem item);
    public bool isHadSilk { get; }
    public bool isHadItem { get; }
}
public abstract class TPlayer :MonoBehaviour, IPlayer_Item
{
    public bool isHadSilk => false;

    public bool isHadItem => item != null;

    //“¹‹ï
    IItem item;
    //Ž…todo...

    public void GetItem(IItem item)
    {
        this.item = item;
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Action();
            item=null;
        }
    }
}
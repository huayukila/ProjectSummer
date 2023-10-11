using UnityEngine;

public interface IPlayer
{
    public void UseItem(TPlayer player);
    public void GetItem(IItem item);
    public bool isHadSilk { get; }
    public bool isHadItem { get; }
}
public abstract class TPlayer :MonoBehaviour, IPlayer
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

    public void UseItem(TPlayer player)
    {
        if(item != null)
        {
            item.Action(player);
            item=null;
        }
    }
}
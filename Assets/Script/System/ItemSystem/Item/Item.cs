public interface IItem
{
    void Use(Player player);
}
public abstract class Item :IItem 
{
    void IItem.Use(Player player)
    {
        OnUse(player);
    }
    public abstract void OnUse(Player player);
}

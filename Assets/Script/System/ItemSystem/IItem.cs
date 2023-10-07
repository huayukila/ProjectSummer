public interface IItem
{
    public void Action(TPlayer player);
}

public abstract class Item :IItem 
{
    public abstract void Action(TPlayer player);
}

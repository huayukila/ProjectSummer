public interface IPlayer2ItemSystem
{
    public bool HadSilk { get; }
    public bool HadItem() => item!=null;
    public IItem item { get; set; }
}

public static class IPlayer2ItemSystemExtension
{
    public static void UseItem(this IPlayer2ItemSystem self, Player player)
    {
        if (self.HadItem())
        {
            self.item.Use(player);
            self.item = null;
        }
    }

    public static void GetItem(this IPlayer2ItemSystem self, IItem item)
    {
        self.item = item;
    }
}
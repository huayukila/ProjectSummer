using Character;

public interface IPlayer2ItemSystem
{
    //糸持っていますか
    public bool HadSilk { get; }

    //アイテムもっていますか
    public bool HadItem() => item != null;

    //アイテム道具
    public IItem item { get; set; }
}

public static class IPlayer2ItemSystemExtension
{
    /// <summary>
    /// 道具使う
    /// </summary>
    /// <param name="self"></param>
    /// <param name="player"></param>
    public static void UseItem(this IPlayer2ItemSystem self, Player player)
    {
        if (self.HadItem())
        {
            self.item.Use(player);
            self.item = null;
        }
    }

    /// <summary>
    /// 道具ゲット
    /// </summary>
    /// <param name="self"></param>
    /// <param name="item"></param>
    public static void GetItem(this IPlayer2ItemSystem self, IItem item)
    {
        self.item = item;
    }
}
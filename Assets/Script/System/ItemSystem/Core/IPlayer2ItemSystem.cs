using Character;

public interface IPlayer2ItemSystem
{
    //�������Ă��܂���
    public bool HadSilk { get; }

    //�A�C�e�������Ă��܂���
    public bool HadItem() => item != null;

    //�A�C�e��
    public IItem item { get; set; }
}

public static class IPlayer2ItemSystemExtension
{
    /// <summary>
    /// �v���C���[��������g��
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
    /// ItemSystem���瓹��𕪔z����p�֐�
    /// </summary>
    /// <param name="self"></param>
    /// <param name="item"></param>
    public static void GetItem(this IPlayer2ItemSystem self, IItem item)
    {
        self.item = item;
    }
}

public enum EItemEffect
{
    None = 0,
    Stun,
    Slip,
}
public interface IItemAffectable
{
    void OnEffect(string effectName);
}
using UnityEngine;

public interface IPlayerControl
{

}

public enum EPlayerCommand
{
    None = 0,
    Respawn,
    Dead,
}
public interface IPlayerCommand : IPlayerControl
{

    void CallPlayerCommand(EPlayerCommand cmd);
}

public interface IPlayerInfo : IPlayerControl
{
    int ID { get; }
    int SilkCount { get; }
    Color AreaColor { get; }

    void SetInfo(int ID,Color color);

}


public interface IPlayerState : IPlayerControl
{

    bool IsFine { get; }
    bool IsDead { get; }
    bool IsInvincible { get; }
    bool IsUncontrollable { get; }
    bool IsStuning { get; }

}


public interface IPlayerBoost
{
    // Get percentage of Charge Bar
    float ChargeBarPercentage { get; }
}

public interface IPlayerInterfaceContainer
{
    PlayerInterfaceContainer GetContainer();
}

public class PlayerInterfaceContainer
{
    private IPlayerControl _playerControl;
    public PlayerInterfaceContainer(IPlayerControl playerControl)
    {
        _playerControl = playerControl;
    }

    public T GetInterface<T>() where T : IPlayerControl
    {
        return (T)_playerControl;
    }
}

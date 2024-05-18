using UnityEngine;

public interface ITestSystem : ISystem
{
    void RegisterManager(GameManager gameManager);
}

public class TestSystem : AbstractSystem, ITestSystem
{
    GameManager _gameManager;

    protected override void OnInit()
    {
    }

    public void RegisterManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}
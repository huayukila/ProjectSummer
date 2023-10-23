using UnityEngine;

public class GameResourceSystem : SingletonBase<GameResourceSystem>
{
    public GameObject[] playerPrefabs;

    public void Init()
    {
        if (playerPrefabs == null)
        {
            playerPrefabs = new GameObject[2] { Resources.Load("Prefabs/Player1") as GameObject, Resources.Load("Prefabs/Player2") as GameObject };
        }
    }

    public void onDeleteResource()
    {
        Resources.UnloadUnusedAssets();
    }
}

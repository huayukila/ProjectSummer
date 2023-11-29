using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IResourceManagement
{
    void Init();
    void Deinit();
    GameObject GetPrefabResource(string name);
    Sprite GetCharacterImage(string name);
}
public class GameResourceSystem : SingletonBase<GameResourceSystem>,IResourceManagement
{
    private Dictionary<string, GameObject> mPrefabs;
    private Dictionary<string, Sprite> mCharacterImages; 

    public void Init()
    {
        mPrefabs = new Dictionary<string, GameObject>();
        mCharacterImages = new Dictionary<string, Sprite>();
        CharacterImageDataBase characterImageDataBase = Resources.Load("CharacterImageDataBase") as CharacterImageDataBase;
        mCharacterImages = characterImageDataBase.GetCharacterImageList();
    }
    
    public void Deinit()
    {
        mPrefabs.Clear();
        mCharacterImages.Clear();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// プレハブを取得する関数
    /// </summary>
    /// <param name="name">プレハブの名前</param>
    /// <returns>プレハブ,見つからない場合はnullを返す</returns>
    public GameObject GetPrefabResource(string name)
    {
        GameObject gameObject = null;
        if(mPrefabs.TryGetValue(name,out GameObject value) == true)
        {
            gameObject = value;
        }
        else
        {
            gameObject = Resources.Load("Prefabs/" + name) as GameObject;
            if(gameObject != null)
            {
                mPrefabs.Add(name, gameObject);
            }
        }
        return gameObject;
    }

    public Sprite GetCharacterImage(string name) 
    {
        Sprite sprite = null;
        if(mCharacterImages.ContainsKey(name))
        {
            sprite = mCharacterImages[name];
        }
        else
        {
            Debug.LogError("Can't find Image of " + name);
        }
        return sprite;
    }
}

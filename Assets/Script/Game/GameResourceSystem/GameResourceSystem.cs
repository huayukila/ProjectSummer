using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IResourceManagement
{
    void Init();
    void Deinit();
    GameObject GetPrefabResource(string name);
}
public class GameResourceSystem : SingletonBase<GameResourceSystem>,IResourceManagement
{
    private Dictionary<string, GameObject> prefabs;

    public void Init()
    {
        prefabs = new Dictionary<string, GameObject>();
    }

    public void Deinit()
    {
        prefabs.Clear();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// �v���n�u���擾����֐�
    /// </summary>
    /// <param name="name">�v���n�u�̖��O</param>
    /// <returns>�v���n�u,������Ȃ��ꍇ��null��Ԃ�</returns>
    public GameObject GetPrefabResource(string name)
    {
        GameObject gameObject = null;
        if(prefabs.TryGetValue(name,out GameObject value) == true)
        {
            gameObject = value;
        }
        else
        {
            gameObject = Resources.Load("Prefabs/" + name) as GameObject;
            if(gameObject != null)
            {
                prefabs.Add(name, gameObject);
            }
        }
        return gameObject;
    }
}

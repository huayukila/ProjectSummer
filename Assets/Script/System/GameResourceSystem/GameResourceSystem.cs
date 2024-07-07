using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IResourceManagement
{
    GameObject GetPrefabResource(string name);
    Sprite GetCharacterImage(string name);
}
public class GameResourceSystem : SingletonBase<GameResourceSystem>,IResourceManagement,IDisposable
{
    private Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> _characterImages = new Dictionary<string, Sprite>(); 

    public GameResourceSystem()
    {
        Init();
    }
    /// <summary>
    /// �v���n�u���擾����֐�
    /// </summary>
    /// <param name="name">�v���n�u�̖��O</param>
    /// <returns>�v���n�u,������Ȃ��ꍇ��null��Ԃ�</returns>
    public GameObject GetPrefabResource(string name)
    {
        GameObject obj = null;
        if(_prefabs.TryGetValue(name,out obj) == true)
        {
            return obj;
        }

        obj = Resources.Load("Prefabs/" + name) as GameObject;
        if(obj != null)
        {
            _prefabs.Add(name, obj);
        }
     
        return obj;

    }

    public Sprite GetCharacterImage(string name) 
    {
        Sprite sprite = null;
        if(_characterImages.ContainsKey(name))
        {
            sprite = _characterImages[name];
        }
        else
        {
            Debug.LogError("Can't find Image of " + name);
        }
        return sprite;
    }

    public void Dispose()
    {
        Deinit();
    }

    private void Init()
    {
        CharacterImageDataBase characterImageDataBase = Resources.Load("CharacterImageDataBase") as CharacterImageDataBase;
        _characterImages = characterImageDataBase.GetCharacterImageList();
    }

    private void Deinit()
    {
        if (_prefabs != null)
        {
            _prefabs.Clear();
            _prefabs = null;
        }
        if (_characterImages != null)
        {
            _characterImages.Clear();
            _characterImages = null;
        }
        Resources.UnloadUnusedAssets();
    }


}

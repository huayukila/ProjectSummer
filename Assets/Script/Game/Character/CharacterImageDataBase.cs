using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterImageData/DataBase",fileName ="CharacterImageDataBase")]
public class CharacterImageDataBase : ScriptableObject
{
    public List<CharacterImageData> PlayerImageList;

    public Dictionary<string, Sprite> GetCharacterImageList()
    {
        Dictionary<string, Sprite> retList = new Dictionary<string, Sprite>();
        if (PlayerImageList != null)
        {
            foreach (CharacterImageData imageData in PlayerImageList)
            {
                if (!retList.ContainsKey(imageData.Name))
                {
                    retList.Add(imageData.Name, imageData.CharacterSprite);
                }
            }
        }
        return retList;

    }

}

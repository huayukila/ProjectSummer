using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParticleObject/DataBase",fileName = "New ParticleObject DataBase")]
[Serializable]
public class ParticleObjectDataBase : ScriptableObject
{
    public List<ParticleObjectData> particleObjectList;

    public Dictionary<string,GameObject> GetParticleObjectList()
    {
        Dictionary<string,GameObject> retList = new Dictionary<string,GameObject>();
        if (particleObjectList != null)
        { 
            foreach (ParticleObjectData objectData in particleObjectList)
            {
                if(retList.ContainsKey(objectData.particleObjectname) == false)
                {
                    retList.Add(objectData.particleObjectname, objectData.particleObject);
                }
            }
        }
        return retList;
    }
}

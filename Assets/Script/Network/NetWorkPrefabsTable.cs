using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Network/PrefabsTable",fileName = "PrefabsTable")]
public class NetWorkPrefabsTable : ScriptableObject
{
    [SerializeField] public List<GameObject> Prefabs;
    [SerializeField] public List<GameObject> PlayerPrefabs;
}

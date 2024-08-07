using System.Collections.Generic;
using System.Data;
using UnityEngine;


// // プレイヤーのDropPointに関する情報をまとめた構造体定義

// // システムインターフェース
// public interface IDropPointSystem
// {
//     // デイニシャライゼーション
//     void Deinit();
//     void InitPlayerDropPointGroup(int ID);
// }
// public class DropPointSystem : SingletonBase<DropPointSystem>, IDropPointSystem
// {
//     private struct PlayerDropPoints
//     {
//         // DropPointが保存されるList
//         public List<GameObject> playerPoints;
//         // DropPointを管理する親
//         public GameObject pointGroup;
//     }

//     // 各プレイヤーのPlayerDropPointsを管理する変数
//     private Dictionary<int, PlayerDropPoints> _playerDropPoints;

//     public DropPointSystem()
//     {
//         _playerDropPoints = new Dictionary<int, PlayerDropPoints>();
//     }

//     /// <summary>
//     /// Listにある全てのDropPoint(GameObject)のワールド座標を返す
//     /// </summary>
//     /// <returns>Listの全てのGameObjectのワールド座標(Vector3型)</returns>
//     private Vector3[] GameObjectToVector3(int id)
//     {
//         // Listのコピーを作る
//         //List<GameObject> retList = new List<GameObject>(list);
//         // 戻り値用配列を作る
//         PlayerDropPoints dropPoints;
        
//         if(_playerDropPoints.TryGetValue(id,out dropPoints))
//         {
//             Vector3[] retPos = new Vector3[dropPoints.playerPoints.Count];
//             int index = 0;
//             // Listの全てのGameObjectのワールド座標を戻り値用配列に入れる
//             foreach (GameObject ob in dropPoints.playerPoints)
//             {
//                 retPos[index++] = ob.transform.position;
//             }
//             return retPos;
//         }

//         return new Vector3[0];
        
//     }

//     /// <summary>
//     /// 新しいプレイヤーのPlayerDropPointsを作り、戻り値として返す関数
//     /// </summary>
//     /// <param name="ID">プレイヤーのID</param>
//     /// <returns>作ったPlayerDropPoints</returns>
//     private PlayerDropPoints CreatePlayerDropPoint(int ID)
//     {
//         PlayerDropPoints ret = new PlayerDropPoints();
//         ret.pointGroup = new GameObject("Player" + ID.ToString() + "DropPointGroup");
//         ret.playerPoints = new List<GameObject>();
//         return ret;
//     }


//     /// <summary>
//     /// 特定のプレイヤーのDropPoint(GameObject)を管理するListにDropPoint(GameObject)を入れる関数
//     /// </summary>
//     /// <param name="ID">プレイヤーID</param>
//     /// <param name="dropPoint">Listに入れるDropPoint</param>
//     public void AddPoint(int ID, GameObject dropPoint)
//     {
//         // プレイヤーのIDがDictionaryに存在したら
//         if (_playerDropPoints.ContainsKey(ID))
//         {
//             // dropPointの親を設定して、Listに入れる
//             dropPoint.transform.parent = _playerDropPoints[ID].pointGroup.transform;
//             _playerDropPoints[ID].playerPoints.Add(dropPoint);
//         }
//         // 存在しない場合はエラーメッセージを出力
//         else
//         {
//             //TODO バグがあるらしいが、要検討
//             PlayerDropPoints player = CreatePlayerDropPoint(ID);
//             dropPoint.transform.parent = player.pointGroup.transform;
//             player.playerPoints.Add(dropPoint);
//             _playerDropPoints.Add(ID, player);
//         }
//     }

//     /// <summary>
//     /// 消えたDropPoint(GameObject)をListから消す関数
//     /// </summary>
//     /// <param name="ID">プレイヤーID</param>
//     /// <param name="dropPoint">消えたDropPoint(GameObject)</param>
//     public void RemovePoint(int ID, GameObject dropPoint)
//     {
//         // プレイヤーのIDがDictionaryに存在したら
//         if (_playerDropPoints.ContainsKey(ID))
//         {
//             _playerDropPoints[ID].playerPoints.Remove(dropPoint);
//         }
//     }

//     /// <summary>
//     /// プレイヤーの全てのDropPoint(GameObject)を消す関数
//     /// </summary>
//     /// <param name="ID">プレイヤーのID</param>
//     public void ClearDropPoints(int ID)
//     {
//         // プレイヤーのIDがDictionaryに存在したら
//         if (_playerDropPoints.ContainsKey(ID))
//         {
//             // 全てのDropPoint(GameObject)を破棄する
//             foreach (GameObject dropPoint in _playerDropPoints[ID].playerPoints)
//             {
//                 Object.Destroy(dropPoint);
//             }
//             // Listにある物を全部消す
//             _playerDropPoints[ID].playerPoints.Clear();
//         }
//     }

//     /// <summary>
//     /// プレイヤーの全てのDropPointのワールド座標を戻す関数
//     /// </summary>
//     /// <param name="ID">プレイヤーのID</param>
//     /// <returns>全てのDropPoint(GameObject)のワールド座標（Vector3型）、プレイヤーが存在しない場合は空の配列を返す</returns>
//     public Vector3[] GetPlayerDropPoints(int ID)
//     {
//         // 戻り値用配列
//         Vector3[] ret = new Vector3[]{ };
//         // プレイヤーのIDがDictionaryに存在したら
//         if (_playerDropPoints.ContainsKey(ID))
//         {
//             ret = GameObjectToVector3(ID);
//         }
//         return ret;
//     }


//     #region interface

//     /// <summary>
//     /// DropPointSystemをデイニシャライゼーションする関数
//     /// </summary>
//     public void Deinit()
//     {
//         // Dictionaryを消す
//         if (_playerDropPoints != null)
//         {
//             _playerDropPoints.Clear();
//         }
//     }

//     /// <summary>
//     /// プレイヤーのPlayerDropPointsを初期化する関数
//     /// </summary>
//     /// <param name="ID">プレイヤーのID</param>
//     public void InitPlayerDropPointGroup(int ID)
//     {
//         // IDのプレイヤーが存在しない場合だったら
//         if (!_playerDropPoints.ContainsKey(ID))
//         {
//             // 新しいプレイヤーのPlayerDropPointsを作る
//             PlayerDropPoints player = CreatePlayerDropPoint(ID);
//             _playerDropPoints.Add(ID, player);
//         }
//     }
//     #endregion

//     #region test code
//     public void DebugFunc()
//     {
//         Debug.Log(_playerDropPoints.Count);
//         foreach(var k in _playerDropPoints)
//         {
//             Debug.Log("Key:");
//             Debug.Log(k.Key);
//             Debug.Log(k.Value.playerPoints.Count);
//         }

//     }
//     #endregion
// }

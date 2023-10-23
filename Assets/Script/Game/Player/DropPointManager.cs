using System.Collections.Generic;
using UnityEngine;


public struct PlayerDropPoints
{
    public List<GameObject> playerPoints;
    public GameObject pointGroup;
}
public class DropPointManager : Singleton<DropPointManager>
{
    private Dictionary<int, PlayerDropPoints> _playerDropPoints;
    List<GameObject> _player1Points;            // プレイヤー1が落としたDropPointをまとめて管理するリスト
    List<GameObject> _player2Points;            // プレイヤー2が落としたDropPointをまとめて管理するリスト
    GameObject _p1pointGroup;                   // プレイヤー1の生成したDropPointをHierarchyでまとめる空っぽのGameObject
    GameObject _p2pointGroup;                   // プレイヤー2の生成したDropPointをHierarchyでまとめる空っぽのGameObject

    /// <summary>
    /// Listにある全てのDropPointのワールド座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3[] GameObjectToVector3(List<GameObject> l)
    {
        List<GameObject> temp = new List<GameObject>(l);
        Vector3[] pos = new Vector3[temp.Count];
        int index = 0;
        foreach(GameObject ob in l)
        {
            pos[index++] = ob.transform.position;
        }
        return pos;
    }

    /// <summary>
    /// プレイヤー1が当たったDropPointから尻尾までのDropPointのワールド座標を返す
    /// </summary>
    /// <param name="pt">当たったDropPointのGameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPlayerOnePaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //戻り値を保存する変数
        List<Vector3> ret = new List<Vector3>();
        List<GameObject> temp = new List<GameObject>(_player1Points);
        foreach (GameObject ob in temp)
        {
            if (ob == pt)
            {
                addFlag = true;
            }
            //当たったDropPointからListの最後までのDropPointを戻り値の変数に入れる
            if (addFlag)
            {
                ret.Add(ob.transform.position);
            }
        }
        return ret;
    }

    /// <summary>
    /// プレイヤー2が当たったDropPointから尻尾までのDropPointのワールド座標を返す
    /// </summary>
    /// <param name="pt">当たったDropPointのGameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPlayerTwoPaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //戻り値を保存する変数
        List<Vector3> ret = new List<Vector3>();
        List<GameObject> currentPoints = new List<GameObject>(_player2Points);
        foreach(GameObject ob in currentPoints)
        {
            if(ob == pt)
            {
                addFlag = true;
            }
            //当たったDropPointからListの最後までのDropPointを戻り値の変数に入れる
            if(addFlag)
            {
                ret.Add(ob.transform.position);
            }
        }
        return ret;
    }

    /// <summary>
    /// プレイヤー1が落としたDropPointをリストに加える
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerOneAddPoint(GameObject ob)
    {
        // グループに入れる
        ob.transform.parent = _p1pointGroup.transform;
        _player1Points.Add(ob);
    }

    /// <summary>
    /// プレイヤー2が落としたDropPointをリストに加える
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerTwoAddPoint(GameObject ob)
    {
        ob.transform.parent = _p2pointGroup.transform;
        _player2Points.Add(ob);
    }

    /// <summary>
    /// プレイヤー1が落としたDropPointをリストから消す
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerOneRemovePoint(GameObject ob)
    {
        _player1Points.Remove(ob);
    }

    /// <summary>
    /// プレイヤー2が落としたDropPointをリストから消す
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerTwoRemovePoint(GameObject ob)
    {
        _player2Points.Remove(ob);
    }

    /// <summary>
    /// プレイヤー1が落としたDropPointを全て消す
    /// </summary>
    public void ClearPlayerOneDropPoints()
    {
        foreach(GameObject ob in _player1Points)
        {
            Destroy(ob);
        }
        _player1Points.Clear();
    }

    /// <summary>
    /// プレイヤー2が落としたDropPointを全て消す
    /// </summary>
    public void ClearPlayerTwoDropPoints()
    {
        foreach (GameObject ob in _player2Points)
        {
            Destroy(ob);
        }
        _player2Points.Clear();
    }

    /// <summary>
    /// プレイヤー1のDropPointの全てのワールド座標を返す
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPlayerOneDropPoints() => GameObjectToVector3(_player1Points);

    /// <summary>
    /// プレイヤー2のDropPointの全てのワールド座標を返す
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPlayerTwoDropPoints() => GameObjectToVector3(_player2Points);

    protected override void Awake()
    {
    }
    private void FixedUpdate()
    {

    }

    private void Update()
    {

    }

    public void Init()
    {
        _player1Points = new List<GameObject>();
        _player2Points = new List<GameObject>();
        _p1pointGroup = new GameObject("Player1DropPointGroup");
        _p2pointGroup = new GameObject("Player2DropPointGroup");
    }
}

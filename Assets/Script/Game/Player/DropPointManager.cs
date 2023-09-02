using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : Singleton<DropPointManager>
{
    List<GameObject> points;
    LineRenderer lr;            //※Trail Rendererに変更する予定

    /// <summary>
    /// DropPointのGameObjectをListに追加する
    /// </summary>
    /// <param name="ob"></param>
    public void AddPoint(GameObject ob)
    {
        points.Add(ob);
    }

    /// <summary>
    /// DropPointのGameObjectをListから削除する
    /// </summary>
    /// <param name="ob"></param>
    public void DeletePoint(GameObject ob)
    {
        points.Remove(ob);
    }

    /// <summary>
    /// LineRendererのプロパティを設定する
    /// </summary>
    private void SetRendererProperties()
    {
        if (lr != null)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.startWidth = 0.5f;
            lr.endWidth = 0.5f;
        }
    }

    /// <summary>
    /// Listにある全てのDropPointのワールド座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3[] GameObjectToVector3()
    {
        var temp = points;
        Vector3[] pos = new Vector3[temp.Count];
        for(int i = 0; i < temp.Count; ++i)
        {
            pos[i] = temp[i].transform.position;
        }
        return pos;
    }

    /// <summary>
    /// プレイヤーが当たったDropPointから尻尾までのDropPointのワールド座標を返す
    /// </summary>
    /// <param name="pt">当たったDropPointのGameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //戻り値を保存する変数
        List<Vector3> ret = new List<Vector3>();
        foreach(GameObject ob in points)
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
    /// 全てのDropPointを削除する
    /// </summary>
    public void Clear()
    {
        points.Clear();
    }    
    
    protected override void Awake()
    {
        base.Awake();
        points = new List<GameObject>();
        lr = gameObject.AddComponent<LineRenderer>();
        SetRendererProperties();
    }
    private void FixedUpdate()
    {
        //DropPointの数は一つより大きい場合はlineRendererを描く
        if (points.Count > 1)
        {
            lr.positionCount = points.Count;
            lr.SetPositions(GameObjectToVector3());
        }

    }

    private void Update()
    {

    }



}

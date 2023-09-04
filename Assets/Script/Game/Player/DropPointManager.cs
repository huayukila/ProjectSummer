using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : Singleton<DropPointManager>
{
    List<GameObject> _points;
    LineRenderer _lr;            //※Trail Rendererに変更する予定

    /// <summary>
    /// DropPointのGameObjectをListに追加する
    /// </summary>
    /// <param name="ob"></param>
    public void AddPoint(GameObject ob)
    {
        _points.Add(ob);
    }

    /// <summary>
    /// DropPointのGameObjectをListから削除する
    /// </summary>
    /// <param name="ob"></param>
    public void DeletePoint(GameObject ob)
    {
        _points.Remove(ob);
    }

    /// <summary>
    /// LineRendererのプロパティを設定する
    /// </summary>
    private void SetRendererProperties()
    {
        if (_lr != null)
        {
            _lr.material = new Material(Shader.Find("Sprites/Default"));
            _lr.startColor = Color.red;
            _lr.endColor = Color.red;
            _lr.startWidth = 0.5f;
            _lr.endWidth = 0.5f;
        }
    }

    /// <summary>
    /// Listにある全てのDropPointのワールド座標を返す
    /// </summary>
    /// <returns></returns>
    private Vector3[] GameObjectToVector3()
    {
        var temp = _points;
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
        foreach(GameObject ob in _points)
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
        foreach(GameObject ob in _points)
        {
            Destroy(ob.gameObject);
        }
        _points.Clear();
        _lr.enabled = false;
    }    
    
    protected override void Awake()
    {
        base.Awake();
        _points = new List<GameObject>();
        _lr = gameObject.AddComponent<LineRenderer>();
        _lr.enabled = false;
        SetRendererProperties();
    }
    private void FixedUpdate()
    {
        //DropPointの数は一つより大きい場合はlineRendererを描く
        if (_points.Count > 1)
        {
            _lr.enabled = true;
            _lr.positionCount = _points.Count;
            _lr.SetPositions(GameObjectToVector3());
        }

    }

    private void Update()
    {

    }



}

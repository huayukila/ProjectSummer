using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : Singleton<DropPointManager>
{
    List<GameObject> _points;
    LineRenderer _lr;            //��Trail Renderer�ɕύX����\��

    /// <summary>
    /// DropPoint��GameObject��List�ɒǉ�����
    /// </summary>
    /// <param name="ob"></param>
    public void AddPoint(GameObject ob)
    {
        _points.Add(ob);
    }

    /// <summary>
    /// DropPoint��GameObject��List����폜����
    /// </summary>
    /// <param name="ob"></param>
    public void DeletePoint(GameObject ob)
    {
        _points.Remove(ob);
    }

    /// <summary>
    /// LineRenderer�̃v���p�e�B��ݒ肷��
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
    /// List�ɂ���S�Ă�DropPoint�̃��[���h���W��Ԃ�
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
    /// �v���C���[����������DropPoint����K���܂ł�DropPoint�̃��[���h���W��Ԃ�
    /// </summary>
    /// <param name="pt">��������DropPoint��GameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //�߂�l��ۑ�����ϐ�
        List<Vector3> ret = new List<Vector3>();
        foreach(GameObject ob in _points)
        {
            if(ob == pt)
            {
                addFlag = true;
            }
            //��������DropPoint����List�̍Ō�܂ł�DropPoint��߂�l�̕ϐ��ɓ����
            if(addFlag)
            {
                ret.Add(ob.transform.position);
            }
        }
        return ret;
    }

    /// <summary>
    /// �S�Ă�DropPoint���폜����
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
        //DropPoint�̐��͈���傫���ꍇ��lineRenderer��`��
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

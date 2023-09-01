using System.Collections.Generic;
using UnityEngine;

public class DropPointManager : Singleton<DropPointManager>
{
    List<GameObject> points;
    LineRenderer lr;            //��Trail Renderer�ɕύX����\��

    /// <summary>
    /// DropPoint��GameObject��List�ɒǉ�����
    /// </summary>
    /// <param name="ob"></param>
    public void AddPoint(GameObject ob)
    {
        points.Add(ob);
    }

    /// <summary>
    /// DropPoint��GameObject��List����폜����
    /// </summary>
    /// <param name="ob"></param>
    public void DeletePoint(GameObject ob)
    {
        points.Remove(ob);
    }

    /// <summary>
    /// LineRenderer�̃v���p�e�B��ݒ肷��
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
    /// List�ɂ���S�Ă�DropPoint�̃��[���h���W��Ԃ�
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
    /// �v���C���[����������DropPoint����K���܂ł�DropPoint�̃��[���h���W��Ԃ�
    /// </summary>
    /// <param name="pt">��������DropPoint��GameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //�߂�l��ۑ�����ϐ�
        List<Vector3> ret = new List<Vector3>();
        foreach(GameObject ob in points)
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
        //DropPoint�̐��͈���傫���ꍇ��lineRenderer��`��
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

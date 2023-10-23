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
    List<GameObject> _player1Points;            // �v���C���[1�����Ƃ���DropPoint���܂Ƃ߂ĊǗ����郊�X�g
    List<GameObject> _player2Points;            // �v���C���[2�����Ƃ���DropPoint���܂Ƃ߂ĊǗ����郊�X�g
    GameObject _p1pointGroup;                   // �v���C���[1�̐�������DropPoint��Hierarchy�ł܂Ƃ߂����ۂ�GameObject
    GameObject _p2pointGroup;                   // �v���C���[2�̐�������DropPoint��Hierarchy�ł܂Ƃ߂����ۂ�GameObject

    /// <summary>
    /// List�ɂ���S�Ă�DropPoint�̃��[���h���W��Ԃ�
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
    /// �v���C���[1����������DropPoint����K���܂ł�DropPoint�̃��[���h���W��Ԃ�
    /// </summary>
    /// <param name="pt">��������DropPoint��GameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPlayerOnePaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //�߂�l��ۑ�����ϐ�
        List<Vector3> ret = new List<Vector3>();
        List<GameObject> temp = new List<GameObject>(_player1Points);
        foreach (GameObject ob in temp)
        {
            if (ob == pt)
            {
                addFlag = true;
            }
            //��������DropPoint����List�̍Ō�܂ł�DropPoint��߂�l�̕ϐ��ɓ����
            if (addFlag)
            {
                ret.Add(ob.transform.position);
            }
        }
        return ret;
    }

    /// <summary>
    /// �v���C���[2����������DropPoint����K���܂ł�DropPoint�̃��[���h���W��Ԃ�
    /// </summary>
    /// <param name="pt">��������DropPoint��GameObject</param>
    /// <returns></returns>
    public List<Vector3> GetPlayerTwoPaintablePointVector3(GameObject pt)
    {
        bool addFlag = false;
        //�߂�l��ۑ�����ϐ�
        List<Vector3> ret = new List<Vector3>();
        List<GameObject> currentPoints = new List<GameObject>(_player2Points);
        foreach(GameObject ob in currentPoints)
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
    /// �v���C���[1�����Ƃ���DropPoint�����X�g�ɉ�����
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerOneAddPoint(GameObject ob)
    {
        // �O���[�v�ɓ����
        ob.transform.parent = _p1pointGroup.transform;
        _player1Points.Add(ob);
    }

    /// <summary>
    /// �v���C���[2�����Ƃ���DropPoint�����X�g�ɉ�����
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerTwoAddPoint(GameObject ob)
    {
        ob.transform.parent = _p2pointGroup.transform;
        _player2Points.Add(ob);
    }

    /// <summary>
    /// �v���C���[1�����Ƃ���DropPoint�����X�g�������
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerOneRemovePoint(GameObject ob)
    {
        _player1Points.Remove(ob);
    }

    /// <summary>
    /// �v���C���[2�����Ƃ���DropPoint�����X�g�������
    /// </summary>
    /// <param name="ob"></param>
    public void PlayerTwoRemovePoint(GameObject ob)
    {
        _player2Points.Remove(ob);
    }

    /// <summary>
    /// �v���C���[1�����Ƃ���DropPoint��S�ď���
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
    /// �v���C���[2�����Ƃ���DropPoint��S�ď���
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
    /// �v���C���[1��DropPoint�̑S�Ẵ��[���h���W��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetPlayerOneDropPoints() => GameObjectToVector3(_player1Points);

    /// <summary>
    /// �v���C���[2��DropPoint�̑S�Ẵ��[���h���W��Ԃ�
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

using System.Collections.Generic;
using UnityEngine;


// �v���C���[��DropPoint�Ɋւ�������܂Ƃ߂��\���̒�`

// �V�X�e���C���^�[�t�F�[�X
public interface IDropPointSystem
{
    // ������
    void Init();
    // �f�C�j�V�����C�[�[�V����
    void Deinit();
    void InitPlayerDropPointGroup(int ID);
}
public class DropPointSystem : SingletonBase<DropPointSystem>, IDropPointSystem
{
    private struct PlayerDropPoints
    {
        // DropPoint���ۑ������List
        public List<GameObject> playerPoints;
        // DropPoint���Ǘ�����e
        public GameObject pointGroup;
    }

    // �e�v���C���[��PlayerDropPoints���Ǘ�����ϐ�
    private Dictionary<int, PlayerDropPoints> _playerDropPoints;

    /// <summary>
    /// List�ɂ���S�Ă�DropPoint(GameObject)�̃��[���h���W��Ԃ�
    /// </summary>
    /// <returns>List�̑S�Ă�GameObject�̃��[���h���W(Vector3�^)</returns>
    private Vector3[] GameObjectToVector3(List<GameObject> list)
    {
        // List�̃R�s�[�����
        //List<GameObject> retList = new List<GameObject>(list);
        // �߂�l�p�z������
        Vector3[] retPos = new Vector3[list.Count];
        int index = 0;
        // List�̑S�Ă�GameObject�̃��[���h���W��߂�l�p�z��ɓ����
        foreach (GameObject ob in list)
        {
            retPos[index++] = ob.transform.position;
        }
        return retPos;
    }

    /// <summary>
    /// �V�����v���C���[��PlayerDropPoints�����A�߂�l�Ƃ��ĕԂ��֐�
    /// </summary>
    /// <param name="ID">�v���C���[��ID</param>
    /// <returns>�����PlayerDropPoints</returns>
    private PlayerDropPoints CreatePlayerDropPoint(int ID)
    {
        PlayerDropPoints ret = new PlayerDropPoints();
        ret.pointGroup = new GameObject("Player" + ID.ToString() + "DropPointGroup");
        ret.playerPoints = new List<GameObject>();
        return ret;
    }


    /// <summary>
    /// ����̃v���C���[��DropPoint(GameObject)���Ǘ�����List��DropPoint(GameObject)������֐�
    /// </summary>
    /// <param name="ID">�v���C���[ID</param>
    /// <param name="dropPoint">List�ɓ����DropPoint</param>
    public void AddPoint(int ID, GameObject dropPoint)
    {
        // �v���C���[��ID��Dictionary�ɑ��݂�����
        if (_playerDropPoints.ContainsKey(ID))
        {
            // dropPoint�̐e��ݒ肵�āAList�ɓ����
            dropPoint.transform.parent = _playerDropPoints[ID].pointGroup.transform;
            _playerDropPoints[ID].playerPoints.Add(dropPoint);
        }
        // ���݂��Ȃ��ꍇ�͐V����PlayerDropPoints�����
        else
        {
            PlayerDropPoints player = CreatePlayerDropPoint(ID);
            dropPoint.transform.parent = player.pointGroup.transform;
            player.playerPoints.Add(dropPoint);
            _playerDropPoints.Add(ID, player);
        }
    }

    /// <summary>
    /// ������DropPoint(GameObject)��List��������֐�
    /// </summary>
    /// <param name="ID">�v���C���[ID</param>
    /// <param name="dropPoint">������DropPoint(GameObject)</param>
    public void RemovePoint(int ID, GameObject dropPoint)
    {
        // �v���C���[��ID��Dictionary�ɑ��݂�����
        if (_playerDropPoints.ContainsKey(ID))
        {
            _playerDropPoints[ID].playerPoints.Remove(dropPoint);
        }
    }

    /// <summary>
    /// �v���C���[�̑S�Ă�DropPoint(GameObject)�������֐�
    /// </summary>
    /// <param name="ID">�v���C���[��ID</param>
    public void ClearDropPoints(int ID)
    {
        // �v���C���[��ID��Dictionary�ɑ��݂�����
        if (_playerDropPoints.ContainsKey(ID))
        {
            // �S�Ă�DropPoint(GameObject)��j������
            foreach (GameObject dropPoint in _playerDropPoints[ID].playerPoints)
            {
                Object.Destroy(dropPoint);
            }
            // List�ɂ��镨��S������
            _playerDropPoints[ID].playerPoints.Clear();
        }
    }

    /// <summary>
    /// �v���C���[�̑S�Ă�DropPoint�̃��[���h���W��߂��֐�
    /// </summary>
    /// <param name="ID">�v���C���[��ID</param>
    /// <returns>�S�Ă�DropPoint(GameObject)�̃��[���h���W�iVector3�^�j�A�v���C���[�����݂��Ȃ��ꍇ�͋�̔z���Ԃ�</returns>
    public Vector3[] GetPlayerDropPoints(int ID)
    {
        // �߂�l�p�z��
        Vector3[] ret = new Vector3[]{ };
        // �v���C���[��ID��Dictionary�ɑ��݂�����
        if (_playerDropPoints.ContainsKey(ID))
        {
            ret = GameObjectToVector3(_playerDropPoints[ID].playerPoints);
        }
        return ret;
    }


    #region interface
    /// <summary>
    /// DropPointSystem������������֐�
    /// </summary>
    public void Init()
    {
        if (_playerDropPoints == null)
        {
            _playerDropPoints = new Dictionary<int, PlayerDropPoints>();
        }
    }

    /// <summary>
    /// DropPointSystem���f�C�j�V�����C�[�[�V��������֐�
    /// </summary>
    public void Deinit()
    {
        // Dictionary������
        if (_playerDropPoints != null)
        {
            _playerDropPoints.Clear();
        }
    }

    /// <summary>
    /// �v���C���[��PlayerDropPoints������������֐�
    /// </summary>
    /// <param name="ID">�v���C���[��ID</param>
    public void InitPlayerDropPointGroup(int ID)
    {
        // ID�̃v���C���[�����݂��Ȃ��ꍇ��������
        if (!_playerDropPoints.ContainsKey(ID))
        {
            // �V�����v���C���[��PlayerDropPoints�����
            PlayerDropPoints player = CreatePlayerDropPoint(ID);
            _playerDropPoints.Add(ID, player);
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class ScoreItemManager : Singleton<ScoreItemManager>
{   
    private Timer _goldenSilkSpawnTimer;        // ���̖Ԃ𐶐����邱�Ƃ��Ǘ�����^�C�}�[
    private GameObject _gotSilkPlayer;          // ���̖Ԃ������Ă���v���C���[

    public GameObject inSpaceSilk;              // ���̖�
    public GameObject goalPoint;                // �S�[��
    public Material goldMaterial;               // ���̖Ԃ̍ގ�

    /// <summary>
    /// ���̖Ԃ̐����ʒu�����߂�֐�
    /// �������̂��߁A�Œ�ʒu�ɐ������Ă���
    /// </summary>
    /// <returns></returns>
    private Vector3 GetInSpaceRandomPosition()
    {
        // temp pos
        return new Vector3(0.0f,0.64f,5.0f);
    }

    /// <summary>
    /// ���̖Ԃ̓S�[���܂ŉ^�����ꂽ���̑��������
    /// </summary>
    public void SetReachGoalProperties()
    {
        // �v���C���[1���^��������
        if(_gotSilkPlayer == GameManager.Instance.playerOne)
        {
            // player1 get 1 point
            Debug.Log("Player 1 get 1 point");
            ScoreSystem.Instance.AddScore(1);
        }
        // �v���C���[2���^��������
        if (_gotSilkPlayer == GameManager.Instance.playerTwo)
        {
            // player2 get 1 point
            Debug.Log("Player 2 get 1 point");
            ScoreSystem.Instance.AddScore(2);
        }

        inSpaceSilk.SetActive(false);
        goalPoint.SetActive(false);
        _gotSilkPlayer = null;

        _gotSilkPlayer = null;
        // �V�����^�C�}�[�𐶐�����
        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f, 
            () => 
            {   inSpaceSilk.transform.position = GetInSpaceRandomPosition();
                inSpaceSilk.SetActive(true); 
            }
            );
    }

    /// <summary>
    /// �S�[���̈ʒu�𐶐�����
    /// �������̂��߁A�Œ�ʒu�ɐ������Ă���
    /// </summary>
    private void SetGoalPoint()
    {
        // temp pos
        goalPoint.transform.position = new Vector3(10.0f,0.64f,10.0f);
        goalPoint.SetActive(true);
    }

    /// <summary>
    /// ���̖Ԃ������Ă���v���C���[�̐ݒ������
    /// </summary>
    /// <param name="ob"></param>
    public void SetGotSilkPlayer(GameObject ob)
    {
        if(_gotSilkPlayer == null)
        {
            _gotSilkPlayer = ob;
        }
        // �����Ă���v���C���[�̍ގ���ς���i��ʂ��邽�߁j
        _gotSilkPlayer.GetComponent<Renderer>().material = goldMaterial;
        inSpaceSilk.SetActive(false);
        SetGoalPoint();
    }

    /// <summary>
    /// ���̖Ԃ������Ă����v���C���[�����񂾂���̖Ԃ��h���b�v����
    /// </summary>
    public void DropGoldenSilk()
    {
        if(_gotSilkPlayer != null)
        {
            inSpaceSilk.transform.position = _gotSilkPlayer.transform.position;
            _gotSilkPlayer = null;
            inSpaceSilk.SetActive(true);
            goalPoint.SetActive(false);
        }
    }

    /// <summary>
    /// �v���C���[�����̖Ԃ������Ă��邩�ǂ������`�F�b�N����
    /// </summary>
    /// <param name="ob">�v���C���[</param>
    /// <returns>�����Ă�����true��Ԃ��A����ȊO��false��Ԃ�</returns>
    public bool IsGotSilk(GameObject ob)
    {
        return _gotSilkPlayer == ob;
    }

    protected override void Awake()
    {
        base.Awake();
        _goldenSilkSpawnTimer = new Timer();
        _goldenSilkSpawnTimer.SetTimer(10.0f, () => { inSpaceSilk.SetActive(true); });
        inSpaceSilk.SetActive(false);
        goalPoint.SetActive(false);
    }
    private void Update()
    {
        if (_goldenSilkSpawnTimer != null)
        {
            if (_goldenSilkSpawnTimer.IsTimerFinished())
            {
                _goldenSilkSpawnTimer = null;
            }
        }
    }

}

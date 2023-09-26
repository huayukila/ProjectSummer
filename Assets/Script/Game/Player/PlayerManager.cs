using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    Timer _player1Timer;        // �v���C���[1�̑ҋ@�^�C�}�[
    Timer _player2Timer;        // �v���C���[2�̑ҋ@�^�C�}�[
    GameObject _playerPrefab;
    /// <summary>
    /// �v���C���[�̕����^�C�~���O���`�F�b�N����
    /// </summary>
    private void RespawnCheck()
    {
        // �v���C���[1�����񂾂�
        if (!GameManager.Instance.playerOne.activeSelf)
        {
            // �V�����^�C�}�[�𐶐�����
            if (_player1Timer == null)
            {
                _player1Timer = new Timer();
                _player1Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer1();
                    }
                    );
            }
            // �ҋ@���Ԃ��I�������
            else if (_player1Timer.IsTimerFinished())
            {
                _player1Timer = null;
            }
        }
        // �v���C���[2�����񂾂�
        if (!GameManager.Instance.playerTwo.activeSelf)
        {
            // �V�����^�C�}�[�𐶐�����
            if (_player2Timer == null)
            {
                _player2Timer = new Timer();
                _player2Timer.SetTimer(Global.RESPAWN_TIME,
                    () =>
                    {
                        RespawnPlayer2();
                    }
                    );
            }
            // �ҋ@���Ԃ��I�������
            else if (_player2Timer.IsTimerFinished())
            {
                _player2Timer = null;
            }
        }
    }    
    
    /// <summary>
    /// �v���C���[1�𕜊�������
    /// </summary>
    private void RespawnPlayer1()
    {
        GameManager.Instance.playerOne.GetComponent<Player1Control>().Respawn();
    }

    /// <summary>
    /// �v���C���[2�𕜊�������
    /// </summary>
    private void RespawnPlayer2()
    {
        GameManager.Instance.playerTwo.GetComponent<Player2Control>().Respawn();
    }

    protected override void Awake()
    {
        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player");
        GameObject player1 = Instantiate(_playerPrefab, Global.PLAYER1_START_POSITION, Quaternion.identity);
        player1.AddComponent<Player1Control>().SetAreaColor(Global.PLAYER_ONE_AREA_COLOR);
        player1.name = "Player1";
        GameObject player2 = Instantiate(_playerPrefab, Global.PLAYER2_START_POSITION, Quaternion.identity);
        player2.AddComponent<Player2Control>().SetAreaColor(Global.PLAYER_TWO_AREA_COLOR);
        player2.transform.forward = Vector3.back;
        player2.name = "Player2";
    }

    private void Update()
    {
        RespawnCheck();
    }
}


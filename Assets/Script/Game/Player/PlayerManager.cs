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
        // player1�̃^�C�}�[�̑ҋ@���Ԃ��I�������
        if (_player1Timer != null && _player1Timer.IsTimerFinished())
        {
            _player1Timer = null;
        }
        // player2�̃^�C�}�[�̑ҋ@���Ԃ��I�������
        if (_player2Timer != null && _player2Timer.IsTimerFinished())
        {
            _player2Timer = null;
        }       
    }    
    
    private void RespawnPlayer(GameObject player)
    {
        if(player == GameManager.Instance.playerOne)
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

        }
        else if(player == GameManager.Instance.playerTwo)
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

        }
    }
    /// <summary>
    /// �v���C���[1�𕜊�������
    /// </summary>
    private void RespawnPlayer1()
    {
        GameObject respawnPlayer = GameManager.Instance.playerOne;
        respawnPlayer.GetComponent<Player>().ResetPlayerSpeed();
        respawnPlayer.transform.position = Global.PLAYER1_START_POSITION;
        respawnPlayer.transform.forward = Vector3.forward;
        respawnPlayer.SetActive(true);
        respawnPlayer.GetComponent<Renderer>().material.color = Color.black;
    }

    /// <summary>
    /// �v���C���[2�𕜊�������
    /// </summary>
    private void RespawnPlayer2()
    {
        GameObject respawnPlayer = GameManager.Instance.playerTwo;
        respawnPlayer.GetComponent<Player>().ResetPlayerSpeed();
        respawnPlayer.transform.position = Global.PLAYER2_START_POSITION;
        respawnPlayer.transform.forward = Vector3.back;
        respawnPlayer.SetActive(true);
        respawnPlayer.GetComponent<Renderer>().material.color = Color.black;
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
        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {
            RespawnPlayer(e.player);
        }).UnregisterWhenGameObjectDestroyde(gameObject);
    }

    private void Update()
    {
        RespawnCheck();
    }

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
    }
}


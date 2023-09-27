using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerOne;
    public GameObject playerTwo;
    private ItemSystem itemSystem;

    Timer _player1Timer;        // �v���C���[1�̑ҋ@�^�C�}�[
    Timer _player2Timer;        // �v���C���[2�̑ҋ@�^�C�}�[
    GameObject _playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        //�e�V�X�e���̎��ቻ�Ə�����
        itemSystem=ItemSystem.Instance;
        itemSystem.Init();

        //�V�[���̈ڍs���߂���
        TypeEventSystem.Instance.Register<TitleSceneSwitch>(e => { TitleSceneSwitch(); });
        TypeEventSystem.Instance.Register<MenuSceneSwitch>(e => { MenuSceneSwitch(); });
        TypeEventSystem.Instance.Register<GamingSceneSwitch>(e => { GamingSceneSwitch(); });
        TypeEventSystem.Instance.Register<EndSceneSwitch>(e => { EndSceneSwitch(); });
        TypeEventSystem.Instance.Register<GameOver>(e => { EndSceneSwitch(); });

        _playerPrefab = (GameObject)Resources.Load("Prefabs/Player");

        TypeEventSystem.Instance.Register<PlayerRespawnEvent>(e =>
        {

            RespawnPlayer(e.player);

        }).UnregisterWhenGameObjectDestroyde(gameObject);

        SceneManager.sceneLoaded += SceneLoaded;

    }

    private void Update()
    {
        //�e�V�X�e����update
        //�V�[���̈ڍs�Ȃ�
        RespawnCheck();
    }

    //�V�[���̈ڍs
    void TitleSceneSwitch()
    {
        SceneManager.LoadScene("Title");
    }
    void MenuSceneSwitch()
    {
        SceneManager.LoadScene("MenuScene");
    }
    void GamingSceneSwitch()
    {
        SceneManager.LoadScene("Gaming");
    }
    void EndSceneSwitch()
    {
        SceneManager.LoadScene("End");
    }

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
        if (player == playerOne)
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
        else if (player == playerTwo)
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

    private void OnDestroy()
    {
        Resources.UnloadUnusedAssets();
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene nextScene, LoadSceneMode mode)
    {
        if (nextScene.name == "Gaming")
        {
            GameObject player1 = Instantiate(_playerPrefab, Global.PLAYER1_START_POSITION, Quaternion.identity);
            player1.AddComponent<Player1Control>().SetAreaColor(Global.PLAYER_ONE_AREA_COLOR);
            player1.name = "Player1";
            GameObject player2 = Instantiate(_playerPrefab, Global.PLAYER2_START_POSITION, Quaternion.identity);
            player2.AddComponent<Player2Control>().SetAreaColor(Global.PLAYER_TWO_AREA_COLOR);
            player2.transform.forward = Vector3.back;
            player2.name = "Player2";

            GameObject scoreItemManager = new GameObject("ScoreItemManager");
            scoreItemManager.AddComponent<ScoreItemManager>();

            GameObject dropPointManager = new GameObject("DropPointManager");
            dropPointManager.AddComponent<DropPointManager>();
        }
        else
        {
            _player1Timer = null;
            _player2Timer = null;
        }
    }

}

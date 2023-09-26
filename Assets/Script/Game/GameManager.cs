using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public GameObject playerOne;
    public GameObject playerTwo;
    private ItemSystem itemSystem;
    protected override void Awake()
    {
        base.Awake();
        //各システムの実例化と初期化
        itemSystem=ItemSystem.Instance;
        itemSystem.Init();

        //シーンの移行命令を受け
        TypeEventSystem.Instance.Register<TitleSceneSwitch>(e => { TitleSceneSwitch(); });
        TypeEventSystem.Instance.Register<MenuSceneSwitch>(e => { MenuSceneSwitch(); });
        TypeEventSystem.Instance.Register<GamingSceneSwitch>(e => { GamingSceneSwitch(); });
        TypeEventSystem.Instance.Register<EndSceneSwitch>(e => { EndSceneSwitch(); });
        TypeEventSystem.Instance.Register<GameOver>(e => { EndSceneSwitch(); });
    }

    private void Update()
    {
        //各システムのupdate
        //シーンの移行など
    }

    //シーンの移行
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
}

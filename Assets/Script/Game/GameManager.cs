using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RenderTexture mapMaskTexture;
    public Paintable mapPaintable;
    public GameObject playerOne;
    public GameObject playerTwo;

    //道具システム
    private ItemSystem itemSystem;
    //得点システム
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        //各システムの実例化と初期化
        itemSystem=ItemSystem.Instance;
        itemSystem.Init();
    }

    private void Update()
    {
        //各システムのupdate
        //シーンの移行など
    }
}

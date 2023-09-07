using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RenderTexture mapMaskTexture;
    public Paintable mapPaintable;
    private ItemSystem itemSystem;
    protected override void Awake()
    {
        base.Awake();
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

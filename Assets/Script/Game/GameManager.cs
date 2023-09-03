using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RenderTexture mapMaskTexture;
    public Paintable mapPaintable;


    protected override void Awake()
    {
        base.Awake();
        //各システムの実例化と初期化

    }

    private void Update()
    {
        //各システムのupdate
        //シーンの移行など
    }
}

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
        //�e�V�X�e���̎��ቻ�Ə�����

    }

    private void Update()
    {
        //�e�V�X�e����update
        //�V�[���̈ڍs�Ȃ�
    }
}

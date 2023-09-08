using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RenderTexture mapMaskTexture;
    public Paintable mapPaintable;
    private ItemSystem itemSystem;
    protected override void Awake()
    {
        base.Awake();
        //�e�V�X�e���̎��ቻ�Ə�����
        itemSystem=ItemSystem.Instance;
        itemSystem.Init();
    }

    private void Update()
    {
        //�e�V�X�e����update
        //�V�[���̈ڍs�Ȃ�
    }
}

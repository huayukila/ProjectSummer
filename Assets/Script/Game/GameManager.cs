using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public RenderTexture mapMaskTexture;
    public Paintable mapPaintable;
    public GameObject playerOne;
    public GameObject playerTwo;

    //����V�X�e��
    private ItemSystem itemSystem;
    //���_�V�X�e��
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
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

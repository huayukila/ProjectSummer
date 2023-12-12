using UnityEngine;

public class Paintable : MonoBehaviour
{
    public bool debugUV = true;
    public Renderer GetRenderer() => rend;
    public RenderTexture GetMask() => maskTexture;
    public RenderTexture GetCopy() => copyTexture;
    public RenderTexture GetAreaMask() => areaMaskTexture;
    public RenderTexture GetAreaCopy() => areaCopyTexture;

    private int textureSize_x = Global.MAP_SIZE_WIDTH * 1000;
    private int textureSize_y = Global.MAP_SIZE_HEIGHT * 1000;
    private Renderer rend;
    private RenderTexture maskTexture;
    private RenderTexture copyTexture;
    private RenderTexture areaMaskTexture;
    private RenderTexture areaCopyTexture;

    private int maskTextureID = Shader.PropertyToID("_MaskTex");
    private int areaMaskTextureID = Shader.PropertyToID("_AreaMaskTex");

    private void Awake()
    {
        transform.localScale = new Vector3(Global.MAP_SIZE_WIDTH*10, 1, Global.MAP_SIZE_HEIGHT*10);

        maskTexture = new RenderTexture(textureSize_x, textureSize_y, 0, RenderTextureFormat.ARGBFloat);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.Create();

        areaMaskTexture = new RenderTexture(textureSize_x, textureSize_y, 0, RenderTextureFormat.ARGBFloat);
        areaMaskTexture.filterMode = FilterMode.Bilinear;
        areaMaskTexture.Create();

        copyTexture = new RenderTexture(textureSize_x, textureSize_y, 0, RenderTextureFormat.ARGBFloat);
        copyTexture.filterMode = FilterMode.Bilinear;
        copyTexture.enableRandomWrite = true;
        copyTexture.Create();

        areaCopyTexture = new RenderTexture(textureSize_x, textureSize_y, 0, RenderTextureFormat.ARGBFloat);
        areaCopyTexture.filterMode = FilterMode.Bilinear;
        areaCopyTexture.Create();

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, copyTexture);
        rend.material.SetTexture(areaMaskTextureID, areaCopyTexture);
        
        
        PolygonPaintManager.Instance.ClearRT(copyTexture);
        PolygonPaintManager.Instance.ClearRT(areaCopyTexture);
        PolygonPaintManager.Instance.SetPaintable(this);
    }
    void OnDisable()
    {
        maskTexture.Release();
        copyTexture.Release();
        areaMaskTexture.Release();
        areaCopyTexture.Release();
    }
}
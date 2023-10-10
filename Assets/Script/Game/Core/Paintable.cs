using UnityEngine;

public class Paintable : MonoBehaviour
{
    public bool debugUV = true;
    public int textureSize_x = 1024;
    public int textureSize_y= 1024;

    private Renderer rend;
    private RenderTexture maskTexture;
    private RenderTexture copyTexture;
    private RenderTexture areaMaskTexture;
    private RenderTexture areaCopyTexture;

    private int maskTextureID = Shader.PropertyToID("_MaskTex");
    private int areaMaskTextureID = Shader.PropertyToID("_AreaMaskTex");

    public Renderer GetRenderer() => rend;
    public RenderTexture GetMask() => maskTexture;
    public RenderTexture GetCopy() => copyTexture;
    public RenderTexture GetAreaMask() => areaMaskTexture;
    public RenderTexture GetAreaCopy() => areaCopyTexture;

    private void Awake()
    {
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

        areaCopyTexture = new RenderTexture(textureSize_x, textureSize_y, 0,RenderTextureFormat.ARGBFloat);
        areaCopyTexture.filterMode = FilterMode.Bilinear;
        areaCopyTexture.Create();

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, copyTexture);
        rend.material.SetTexture(areaMaskTextureID, areaCopyTexture);
    }
    void Start()
    {
        PolygonPaintManager.Instance.mapPaintable = this;
        PolygonPaintManager.Instance.ClearRT(copyTexture);
        PolygonPaintManager.Instance.ClearRT(areaCopyTexture);
        if (debugUV)
        {
            PolygonPaintManager.Instance.InitUVMask(this);
        }
    }
    void OnDisable()
    {
        maskTexture.Release();
        copyTexture.Release();
        areaMaskTexture.Release();
        areaCopyTexture.Release();
    }
}
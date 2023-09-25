using UnityEngine;

public class Paintable : MonoBehaviour
{
    public bool debugUV = true;
    public int textureSize = 1024;

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
    void Start()
    {

        maskTexture = new RenderTexture(textureSize, textureSize, 0);
        maskTexture.filterMode = FilterMode.Bilinear;

        areaMaskTexture = new RenderTexture(textureSize, textureSize, 0);
        areaMaskTexture.filterMode = FilterMode.Bilinear;

        copyTexture = new RenderTexture(textureSize, textureSize, 0);
        copyTexture.filterMode = FilterMode.Bilinear;

        areaCopyTexture = new RenderTexture(textureSize, textureSize, 0);
        areaCopyTexture.filterMode = FilterMode.Bilinear;
        PolygonPaintManager.Instance.ClearRT(copyTexture);
        PolygonPaintManager.Instance.ClearRT(areaCopyTexture);

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, copyTexture);
        rend.material.SetTexture(areaMaskTextureID, areaCopyTexture);

        if (debugUV)
        {
            PolygonPaintManager.Instance.InitUVMask(this);
        }
        PolygonPaintManager.Instance.mapPaintable = this;
    }

    void OnDisable()
    {
        maskTexture.Release();
        copyTexture.Release();
        areaMaskTexture.Release();
        areaCopyTexture.Release();
    }
}
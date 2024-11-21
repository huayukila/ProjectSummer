public sealed class RainbowOutlineEffect : System.IDisposable
{
  private static class OutlineShaderProp 
  {
    public readonly static int OUTLINE_COL = UnityEngine.Shader.PropertyToID("_OutlineColor");
    public readonly static int OUTLINE_WIDTH = UnityEngine.Shader.PropertyToID("_OutlineWidth");
    public readonly static int OUTLINE_INTENSE = UnityEngine.Shader.PropertyToID("_OutlineIntense");
    public readonly static int OUTLINE_ALPHA_THRESHOLD = UnityEngine.Shader.PropertyToID("_AlphaThreshold");

    }
  private UnityEngine.Material _mat;
  private float _timeCnt;
  private bool _disposedValue;

  public RainbowOutlineEffect(UnityEngine.Material material)
  {
    UnityEngine.Assertions.Assert.IsNotNull(material);

    _mat = material;
    _mat.SetColor(OutlineShaderProp.OUTLINE_COL, UnityEngine.Color.clear);
    _mat.SetFloat(OutlineShaderProp.OUTLINE_WIDTH, 1f);
    _mat.SetFloat(OutlineShaderProp.OUTLINE_INTENSE, 1f);
    _mat.SetFloat(OutlineShaderProp.OUTLINE_ALPHA_THRESHOLD, 1f);

    _timeCnt = 0f;
  }

  public void UpdateOutline(float deltaTime)
  {
    _timeCnt += deltaTime;
    _mat.SetColor(OutlineShaderProp.OUTLINE_COL, UnityEngine.Color.HSVToRGB(_timeCnt * 0.3f % 1f, 0.5f, 0.7f));
  }

  public void SetActive(bool value)
  {
    if (!value)
    {
      _mat.SetFloat(OutlineShaderProp.OUTLINE_ALPHA_THRESHOLD, 0f);
    }
    else
    {
      _mat.SetFloat(OutlineShaderProp.OUTLINE_ALPHA_THRESHOLD, 1f);
    }
  }

  private void Dispose(bool disposing)
  {
    if (!_disposedValue)
    {
      if (disposing)
      {
        UnityEngine.Object.Destroy(_mat);
        _mat = null;
      }

      _disposedValue = true;
    }
  }

  public void Dispose()
  {
    Dispose(disposing: true);
    System.GC.SuppressFinalize(this);
  }
}
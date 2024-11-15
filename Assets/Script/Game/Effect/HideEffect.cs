namespace MPostProcess
{
  public sealed class HideEffect : PostEffectBase
  {
    private static class HideShaderProp
    {
      public readonly static int FOG_COLOR_PROP = UnityEngine.Shader.PropertyToID("_FogColor");
      public readonly static int FOG_DENSE_PROP = UnityEngine.Shader.PropertyToID("_FogDense");
      public readonly static float MIN_FOG_DENSE = 0f;
      public readonly static float MAX_FOG_DENSE = 1f;
    }

    private UnityEngine.Material _material;
    private UnityEngine.Color _hideColor;
    private UnityEngine.Rendering.CommandBuffer _buffer;
    private int _tempTextureIdentifier;
    private bool _isEffectActive = false;
    public bool IsActive => _isEffectActive;

    public HideEffect(UnityEngine.Camera targetCam, UnityEngine.Color color)
      : base(targetCam)
    {
      var shader = UnityEngine.Shader.Find("MShaders/PostEffect/DimEffect");
      UnityEngine.Assertions.Assert.IsNotNull(shader, $"Cant find shader: MShaders/PostEffect/DimEffect ");

      _hideColor = color;

      _material = new UnityEngine.Material(shader);
      _material.SetColor(HideShaderProp.FOG_COLOR_PROP, _hideColor);
      _material.SetFloat(HideShaderProp.FOG_DENSE_PROP, 0f);

      _tempTextureIdentifier = UnityEngine.Shader.PropertyToID("_PostEffect");

      _buffer = new UnityEngine.Rendering.CommandBuffer { name = "DimEffect" };
      _buffer.GetTemporaryRT(_tempTextureIdentifier, -1, -1, 0);
      _buffer.Blit(UnityEngine.Rendering.BuiltinRenderTextureType.CameraTarget, _tempTextureIdentifier);
      _buffer.Blit(_tempTextureIdentifier, UnityEngine.Rendering.BuiltinRenderTextureType.CameraTarget, _material);
      _buffer.ReleaseTemporaryRT(_tempTextureIdentifier);
      
      _isEffectActive = false;
    }

    public override void SetActive(bool value)
    {
      if (_isEffectActive == value)
      {
        return;
      }
      
      if(_effectAttachCamera == null)
      {
        return;
      }

      _isEffectActive = value;

      if (_isEffectActive)
      {
        _effectAttachCamera.AddCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardAlpha, _buffer);
      }
      else
      {
        _effectAttachCamera.RemoveCommandBuffer(UnityEngine.Rendering.CameraEvent.AfterForwardAlpha, _buffer);
      }
    }

    public void SetFogDense(float rate)
    {
      rate = UnityEngine.Mathf.Clamp(rate, HideShaderProp.MIN_FOG_DENSE, HideShaderProp.MAX_FOG_DENSE);
      _material.SetFloat(HideShaderProp.FOG_DENSE_PROP, rate);
    }
    protected override void DisposeManagedRes()
    {
      base.DisposeManagedRes();
      if (_material != null)
      {
        UnityEngine.Object.Destroy(_material);
        _material = null;
      }

      if (_buffer != null)
      {
        _buffer.Clear();
        _buffer = null;
      }
    }
  }
}
// namespace MPostProcess

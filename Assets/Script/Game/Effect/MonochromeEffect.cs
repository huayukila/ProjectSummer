namespace MPostProcess
{
  public sealed class MonochromeEffect : PostEffectBase
  {
    private static class MonochromeShaderProp
    {
      public readonly static int EFFECT_RATE = UnityEngine.Shader.PropertyToID("_EffectRate");
      public readonly static int BRIGHTNESS = UnityEngine.Shader.PropertyToID("_BrightnessRate");
      public readonly static float MAX_RATE = 1.0f;
      public readonly static float MIN_RATE = 0.0f;
    }
    private UnityEngine.Material _material;
    private UnityEngine.Rendering.CommandBuffer _buffer;
    private int _tempTextureIdentifier;
    private bool _isEffectActive = false;
    public bool IsActive => _isEffectActive;

    public MonochromeEffect(UnityEngine.Camera targetCam)
      : base(targetCam)
    {
      var shader = UnityEngine.Shader.Find("MShaders/PostEffect/Monochrome");
      UnityEngine.Assertions.Assert.IsNotNull(shader, $"Cant find shader: MShaders/PostEffect/Monochrome ");
      _material = new UnityEngine.Material(shader);

      _tempTextureIdentifier = UnityEngine.Shader.PropertyToID("_PostEffect");

      _buffer = new UnityEngine.Rendering.CommandBuffer { name = "MonochromeEffect" };
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

    public void SetRate(float rate)
    {
      rate = UnityEngine.Mathf.Clamp(rate, MonochromeShaderProp.MIN_RATE, MonochromeShaderProp.MAX_RATE);
      _material.SetFloat(MonochromeShaderProp.EFFECT_RATE, rate);
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

namespace MPostProcess
{
  public abstract class PostEffectBase: System.IDisposable
  {
    protected UnityEngine.Camera _effectAttachCamera;
    private bool _isDisposed;

    protected PostEffectBase()
    {
      _isDisposed = false;
    }

    protected PostEffectBase(UnityEngine.Camera camera)
      :this()
    {
      _effectAttachCamera = camera;
    }

    ~PostEffectBase()
    {
      Dispose(false);
    }

    public abstract void SetActive(bool value);

    public void Dispose()
    {
      Dispose(true);
      System.GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
      if (_isDisposed)
      {
        return;
      }

      if (disposing)
      {
        DisposeManagedRes();
      }

      DisposeUnmanagedRes();

      _isDisposed = true;
    }

    protected virtual void DisposeManagedRes()
    {
      _effectAttachCamera = null;
    }   

    protected virtual void DisposeUnmanagedRes() {}
}
}
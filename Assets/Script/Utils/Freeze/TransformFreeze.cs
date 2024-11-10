namespace MLibrary
{
  [System.Flags]
  public enum FreezeConstraint
  {
    /// <summary>
    /// No constraints
    /// </summary>
    None = 0,                        // 0b000000000
    /// <summary>
    /// Freeze motion along the X-axis.
    /// </summary>
    FreezePositionX = 1,             // 0b000000001
    /// <summary>
    /// Freeze motion along the Y-axis.
    /// </summary>
    FreezePositionY = 2,             // 0b000000010
    /// <summary>
    /// Freeze motion along the Z-axis.
    /// </summary>
    FreezePositionZ = 4,             // 0b000000100
    /// <summary>
    /// Freeze motion along all axis.
    /// </summary>
    FreezePosition = FreezePositionX // 0b000000111
                   | FreezePositionY 
                   | FreezePositionZ,         
    /// <summary>
    /// Freeze rotation along the X-axis.
    /// </summary>
    FreezeRotationX = 8,             // 0b000001000
    /// <summary>
    /// Freeze rotation along the Y-axis.
    /// </summary>
    FreezeRotationY = 16,            // 0b000010000
    /// <summary>
    /// Freeze rotation along the Z-axis.
    /// </summary>
    FreezeRotationZ = 32,            // 0b000100000
    /// <summary>
    /// Freeze rotation along all axis.
    /// </summary>
    FreezeRotation = FreezeRotationX // 0b000111000
                   | FreezeRotationY
                   | FreezeRotationZ ,       
    /// <summary>
    /// Freeze scale along the X-axis.
    /// </summary>
    FreezeScaleX = 64,               // 0b001000000
    /// <summary>
    /// Freeze scale along the Y-axis.
    /// </summary>
    FreezeScaleY = 128,              // 0b010000000
    /// <summary>
    /// Freeze scale along the Z-axis.
    /// </summary>
    FreezeScaleZ = 256,              // 0b100000000
    /// <summary>
    /// Freeze scale along all axis.
    /// </summary>
    FreezeScale = FreezeScaleX       // 0b111000000
                | FreezeScaleY
                | FreezeScaleZ,
    /// <summary>
    /// Freeze motion rotation scale along the X-axis.
    /// </summary>
    FreezeAllX = FreezePositionX     // 0b001001001
               | FreezeRotationX
               | FreezeScaleX,
    /// <summary>
    /// Freeze motion rotation scale along the X-axis.
    /// </summary>
    FreezeAllY = FreezePositionY     // 0b010010010
               | FreezeRotationY
               | FreezeScaleY,
    /// <summary>
    /// Freeze motion rotation scale along the X-axis.
    /// </summary>
    FreezeAllZ = FreezePositionZ     // 0b100100100
               | FreezeRotationZ
               | FreezeScaleZ,
    /// <summary>
    /// Freeze motion rotation scale along all axis.
    /// </summary>
    FreezeAll = FreezePosition       // 0b111111111
              | FreezeRotation
              | FreezeScale,            
  }

  public class TransformFreeze
  {
    private UnityEngine.Transform _freezeTarget;

    /// <summary>
    /// 
    /// </summary>
    public FreezeConstraint Constraint
    {
      get;
      set;
    }
  }
}
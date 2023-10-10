using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    public LayerMask layerMask;
    public float raycastDistance = 10.0f;
    private NativeArray<Color> colorArray;
    private Color _CurrentColor = new Color();

    RenderTexture mapMaskTexture;
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, raycastDistance, layerMask))
        {
            if (mapMaskTexture == null)
            {
                mapMaskTexture = hit.transform.gameObject.GetComponent<Paintable>().GetCopy();
            }
            else
            {
                //ヒットポイントの世界座標をuv座標に変換する
                float uvX = hit.textureCoord.x;
                float uvY = hit.textureCoord.y;
                int x = Mathf.RoundToInt(uvX * mapMaskTexture.width);
                int y = Mathf.RoundToInt(uvY * mapMaskTexture.height);
                //GPUにヒットポイントところの色判別結果を請求する。
                AsyncGPUReadback.Request(mapMaskTexture, 0, x, 1, y, 1, 0, 1, TextureFormat.RGBAFloat,
                            (req) =>
                            {
                                colorArray = req.GetData<Color>();
                                _CurrentColor = colorArray[0];
                            });
            }
        }
    }
    /// <summary>
    /// 同じ色か？
    /// </summary>
    /// <param name="targetColor">目標色</param>
    /// <returns></returns>
    public bool isTargetColor(Color targetColor, float tolerance = 0.2f)
    {
        return
            Mathf.Abs(targetColor.r - _CurrentColor.r) < tolerance &&
            Mathf.Abs(targetColor.g - _CurrentColor.g) < tolerance &&
            Mathf.Abs(targetColor.b - _CurrentColor.b) < tolerance;
    }
}

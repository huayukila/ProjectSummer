using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    public LayerMask layerMask;
    public float raycastDistance = 10.0f;
    private NativeArray<Color32> colorArray;
    void Update()
    {
        RaycastHit hit;
        Vector3 position = Input.mousePosition;
        if (Physics.Raycast(gameObject.transform.position,Vector3.down, out hit, raycastDistance, layerMask))
        {
            //マネージャーからマップのRTをもらう
            RenderTexture mapMaskTexture = GameManager.Instance.mapMaskTexture;
            //ヒットポイントの世界座標をuv座標に変換する
            float uvX = hit.textureCoord.x;
            float uvY = hit.textureCoord.y;
            int x = Mathf.RoundToInt(uvX * mapMaskTexture.width);
            int y = Mathf.RoundToInt(uvY * mapMaskTexture.height);
            //GPUにヒットポイントところの色判別結果を請求する。
            AsyncGPUReadback.Request(mapMaskTexture, 0, x, 1, y, 1, 0, 1, TextureFormat.RGBA32,
                        (req) =>
                        {
                            colorArray = req.GetData<Color32>();
                        });
        }
    }
    /// <summary>
    /// 同じ色か？
    /// </summary>
    /// <param name="c1">目標色</param>
    /// <returns></returns>
    public bool IsSameColor(Color c1)
    {
        return Mathf.Abs(c1.r - colorArray[0].r) + Mathf.Abs(c1.g - colorArray[0].g) + Mathf.Abs(c1.b - colorArray[0].b) < 0.1f;
    }
}

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
                //�q�b�g�|�C���g�̐��E���W��uv���W�ɕϊ�����
                float uvX = hit.textureCoord.x;
                float uvY = hit.textureCoord.y;
                int x = Mathf.RoundToInt(uvX * mapMaskTexture.width);
                int y = Mathf.RoundToInt(uvY * mapMaskTexture.height);
                //GPU�Ƀq�b�g�|�C���g�Ƃ���̐F���ʌ��ʂ𐿋�����B
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
    /// �����F���H
    /// </summary>
    /// <param name="targetColor">�ڕW�F</param>
    /// <returns></returns>
    public bool isTargetColor(Color targetColor, float tolerance = 0.2f)
    {
        return
            Mathf.Abs(targetColor.r - _CurrentColor.r) < tolerance &&
            Mathf.Abs(targetColor.g - _CurrentColor.g) < tolerance &&
            Mathf.Abs(targetColor.b - _CurrentColor.b) < tolerance;
    }
}

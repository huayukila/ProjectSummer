using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    public LayerMask layerMask;
    public float raycastDistance = 10.0f;
    private NativeArray<Color32> colorArray;
    private Color32 _CurrentColor = new Color32();

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
                AsyncGPUReadback.Request(mapMaskTexture, 0, x, 1, y, 1, 0, 1, TextureFormat.RGBA32,
                            (req) =>
                            {
                                colorArray = req.GetData<Color32>();
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
    public bool isTargetColor(Color32 targetColor)
    {
        return
            Mathf.Abs(targetColor.r - _CurrentColor.r) +
            Mathf.Abs(targetColor.g - _CurrentColor.g) +
            Mathf.Abs(targetColor.b - _CurrentColor.b)
            < 0.1f;
    }
}

using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    public LayerMask layerMask;
    public float raycastDistance = 10.0f;
    private NativeArray<Color32> colorArray;
    private Color _CurrentColor=new Color();
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, raycastDistance, layerMask))
        {
            //�}�l�[�W���[����}�b�v��RT�����炤
            RenderTexture mapMaskTexture = GameManager.Instance.mapMaskTexture;
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
                            Debug.Log(colorArray[0]);
                            _CurrentColor = colorArray[0];
                        });
        }
        //Debug.Log(_CurrentColor);
    }
    /// <summary>
    /// �����F���H
    /// </summary>
    /// <param name="targetColor">�ڕW�F</param>
    /// <returns></returns>
    public bool isTargetColor(Color targetColor)
    {
        return 
            Mathf.Abs(targetColor.r - _CurrentColor.r) +
            Mathf.Abs(targetColor.g - _CurrentColor.g) + 
            Mathf.Abs(targetColor.b - _CurrentColor.b) 
            < 0.1f;
    }
}

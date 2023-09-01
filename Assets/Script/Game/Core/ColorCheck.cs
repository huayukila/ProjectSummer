using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    //public Vector3 checkOffset;
    //public float checkDistance;
    //public LayerMask checkLayer;
    //public RayCheck downCheck;

    private void Start()
    {
        //downCheck = new RayCheck();
        //downCheck.checkDistance = checkDistance;
        //downCheck.checkLayer = checkLayer;
        //downCheck.checkOffset = checkOffset;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            Vector3 position = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(position);
            //if (downCheck.Shoot(transform.position, -transform.up, out hit))
            if (Physics.Raycast(ray, out hit))
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
                                var colorArray = req.GetData<Color32>();
                                Debug.Log(IsSameColor(Color.red, colorArray[0]));
                            });
            }
        }
    }
    /// <summary>
    /// �����F���H
    /// </summary>
    /// <param name="c1">�ڕW�F</param>
    /// <param name="c2">�A���F</param>
    /// <returns></returns>
    private bool IsSameColor(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b) < 0.1f;
    }
}

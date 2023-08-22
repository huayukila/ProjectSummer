using UnityEngine;
using UnityEngine.Rendering;

public class ColorCheck : MonoBehaviour
{
    //public Vector3 checkOffset;
    //public float checkDistance;
    //public LayerMask checkLayer;
    //public RayCheck downCheck;

    Vector2 centerUV = new Vector2(0.5f, 0.5f);

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
                RenderTexture mapMaskTexture = GameManager.Instance.mapMaskTexture;
                float uvX = hit.textureCoord.x;
                float uvY = hit.textureCoord.y;
                int x = Mathf.RoundToInt(uvX * mapMaskTexture.width);
                int y = Mathf.RoundToInt(uvY * mapMaskTexture.height);
                Debug.Log(x + "," + y);
                AsyncGPUReadback.Request(mapMaskTexture, 0, x, 1, y, 1, 0, 1, TextureFormat.RGBA32,
                            (req) =>
                            {
                                var colorArray = req.GetData<Color32>();
                                Debug.Log(IsSameColor(Color.red, colorArray[0]));
                            });
            }
        }
    }

    private bool IsSameColor(Color c1, Color c2)
    {
        return Mathf.Abs(c1.r - c2.r) + Mathf.Abs(c1.g - c2.g) + Mathf.Abs(c1.b - c2.b) < 0.1f;
    }
}

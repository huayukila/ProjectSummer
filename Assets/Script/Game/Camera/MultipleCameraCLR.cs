using UnityEngine;

public class MultipleCameraCLR : MonoBehaviour
{
    public Camera mainCamera;
    public Camera subCamera;

    public int regionWidth = 960;
    public int regionHeight = 540;

    void Awake()
    {
        if (!GameManager.Instance.isMultipleScreen)
        {
            SetCamera();
        }
    }

    void SetCamera()
    {
        subCamera.targetDisplay = 0;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float viewportWidth = (float)regionWidth / screenWidth;
        float viewportHeight = (float)regionHeight / screenHeight;
        
        mainCamera.rect = new Rect(0f, 0.25f, viewportWidth, viewportHeight);

        subCamera.rect = new Rect(viewportWidth, 0.25f, viewportWidth, viewportHeight);
    }
}
using UnityEngine;

public class MultipleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void OpenMultScreen()
    {
        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
    }
}
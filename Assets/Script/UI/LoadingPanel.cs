using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPanel : MonoBehaviour
{
    private float countTime = 0f;

    private float durationTime = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        countTime += Time.deltaTime;
        if (countTime >= durationTime)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
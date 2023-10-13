using UnityEngine;

public class TitleSpiderShakeAnim : MonoBehaviour
{
    public float durationTime = 1.0f;
    float currentTime;
    Transform spider;
    private void Start()
    {
        spider = GetComponent<Transform>();
        currentTime = Time.time;
    }
    private void Update()
    {
        if(Time.time-currentTime>durationTime)
        {
            currentTime = Time.time;
            Vector3 rootA = spider.rotation.eulerAngles;
            spider.eulerAngles = rootA * -1;
        }
    }
}

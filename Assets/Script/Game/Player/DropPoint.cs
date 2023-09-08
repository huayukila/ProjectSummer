using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                Destroy(gameObject);
            }
            );
    }

    // Update is called once per frame
    void Update()
    {
        if(timer.IsTimerFinished())
        {
            if(gameObject.CompareTag("DropPoint1"))
            {
                DropPointManager.Instance.PlayerOneRemovePoint(gameObject);
            }
            else
            {
                DropPointManager.Instance.PlayerTwoRemovePoint(gameObject);
            }
        }
    }


}

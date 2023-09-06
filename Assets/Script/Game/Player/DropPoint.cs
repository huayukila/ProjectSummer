using UnityEngine;

public class DropPoint : MonoBehaviour
{
    float _destroyInterval;
    // Start is called before the first frame update
    void Start()
    {
        _destroyInterval = 3.0f;

    }

    // Update is called once per frame
    void Update()
    {
        _destroyInterval -= Time.deltaTime;
        if(_destroyInterval <= 0.0f)
        {
            if(gameObject.CompareTag("DropPoint1"))
            {
                DropPointManager.Instance.PlayerOneRemovePoint(gameObject);
            }
            else if (gameObject.CompareTag("DropPoint2"))
            {
                DropPointManager.Instance.PlayerTwoRemovePoint(gameObject);
            }
            Destroy(gameObject);
        }
    }


}

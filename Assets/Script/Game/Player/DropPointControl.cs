using UnityEngine;

public class DropPointControl : MonoBehaviour
{
    
    GameObject _pointPrefab;
    float _dropInterval;
    float _dropTimer;
    // Start is called before the first frame update
    void Start()
    {
        _pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        _dropInterval = 0.1f;
        _dropTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        _dropTimer += Time.deltaTime;
        if (_dropTimer >= _dropInterval)
        {
            GameObject pt = Instantiate(_pointPrefab, transform.position,transform.rotation);
            DropPointManager.Instance.AddPoint(pt);
            _dropTimer = 0.0f;
        }

    }

}



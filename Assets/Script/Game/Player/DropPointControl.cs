using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer TR;
    protected GameObject _pointPrefab;
    float _dropInterval;
    float _dropTimer;
    // Start is called before the first frame update

    protected abstract void SetDropPoint();
    protected abstract void SetTRProperties();

    private void TryDropPoint()
    {
        _dropTimer += Time.deltaTime;
        if (_dropTimer >= _dropInterval)
        {
            SetDropPoint();
            _dropTimer = 0.0f;
        }

    }

    private void Awake()
    {
        _pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
        _dropInterval = 0.1f;
        _dropTimer = 0.0f;
        TR = gameObject.AddComponent<TrailRenderer>();
        SetTRProperties();
    }

    // Update is called once per frame
    private void Update()
    {
        TryDropPoint();
    }

    private void FixedUpdate() { }

    public void ClearTrail()
    {
        TR.Clear();
    }
}



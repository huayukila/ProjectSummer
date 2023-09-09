using UnityEngine;

public abstract class DropPointControl : MonoBehaviour
{
    protected TrailRenderer TR;
    protected GameObject _pointPrefab;
    Timer _dropTimer;

    protected abstract void InstantiateDropPoint();
    protected abstract void SetTRProperties();

    private void TryDropPoint()
    {
        if(_dropTimer == null)
        {
            _dropTimer = new Timer();
            _dropTimer.SetTimer(Global.DROP_POINT_INTERVAL,
                () =>
                {
                    InstantiateDropPoint();
                }
                );
        }
        else if(_dropTimer.IsTimerFinished())
        {
            _dropTimer = null;
        }
    }

    private void Awake()
    {
        _pointPrefab = (GameObject)Resources.Load("Prefabs/DropPoint");
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



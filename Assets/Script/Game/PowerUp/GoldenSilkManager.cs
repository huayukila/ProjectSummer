using Gaming.PowerUp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnFieldSilk
{
    Vector3[] GetOnFieldSilkPos();
}
public class GoldenSilkManager : Singleton<GoldenSilkManager>, IOnFieldSilk
{

    private Timer mDropSilkTimer;               // 金の糸を落下させるタイマー
    private List<GameObject> mOnFieldSilk;
    private Dictionary<int,Stack<GameObject>> mCapturedSilk;
    // Start is called before the first frame update
    protected override void Awake()
    {
        mOnFieldSilk = new List<GameObject>();
        mCapturedSilk = new Dictionary<int,Stack<GameObject>>();
    }
    void Start()
    {
        // イベントを登録する
        TypeEventSystem.Instance.Register<SilkCapturedEvent>(e =>
        {

            foreach (var pos in e.positions)
            {
                int index = 0;
                while (index < mOnFieldSilk.Count)
                {
                    GameObject silk = mOnFieldSilk[index];
                    if (silk.transform.position == pos)
                    {
                        if (!mCapturedSilk.ContainsKey(e.ID))
                        {
                            mCapturedSilk.Add(e.ID, new Stack<GameObject>());
                        }
                        mCapturedSilk[e.ID].Push(silk);
                        mOnFieldSilk.Remove(silk);
                        silk.GetComponent<GoldenSilkControl>()?.StartInactive();
                        break;
                    }
                    ++index;
                }
            }

        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (mDropSilkTimer != null){
            mDropSilkTimer.IsTimerFinished();
        }
        SetSilkSpawnTimer();
    }

    private void SetSilkSpawnTimer()
    {
        if (mDropSilkTimer != null)
            return;
        
        if (GoldenSilkSystem.Instance.CurrentSilkCount >= Global.MAX_SILK_COUNT)
            return;

        mDropSilkTimer = new Timer();
        mDropSilkTimer.SetTimer(
            Global.SILK_SPAWN_TIME,
            () =>
            {
                GameObject obj = GoldenSilkSystem.Instance.DropNewSilk();
                if (obj != null)
                {
                    mOnFieldSilk.Add(obj);
                }
                mDropSilkTimer = null;
            }
        );
    }

    public Vector3[] GetOnFieldSilkPos()
    {
        List<Vector3> silkPos = new List<Vector3>();
        foreach(var silk in mOnFieldSilk)
        {
            silkPos.Add(silk.transform.position);
        }
        return silkPos.ToArray();

    }

}

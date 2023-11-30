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
    private Stack<GameObject> mCapturedSilk;
    // Start is called before the first frame update
    protected override void Awake()
    {
        mOnFieldSilk = new List<GameObject>();
        mCapturedSilk = new Stack<GameObject>();
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
                        mCapturedSilk.Push(silk);
                        mOnFieldSilk.Remove(silk);
                        silk.GetComponent<IGoldenSilk>()?.SetInactive();
                        break;
                    }
                    ++index;
                }
            }
        }).UnregisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Instance.Register<DropSilkEvent>(e =>
        {
            if(e.dropCount > 0)
            {
                GoldenSilkSystem.Instance.RecycleSilk(mCapturedSilk.Pop());
                --e.dropCount;
                while (e.dropCount > 0)
                {
                    if(mCapturedSilk.Count > 0)
                    {
                        GameObject dropSilk = mCapturedSilk.Pop();
                        dropSilk.GetComponent<IGoldenSilk>()?.StartDrop(e.pos,e.pos + GetDropSilkEndPos(e.pos));
                        mOnFieldSilk.Add(dropSilk);
                        --e.dropCount;
                    }
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

    private Vector3 GetDropSilkEndPos(Vector3 startPos)
    {
        Vector3 ret = Vector3.zero;
        float availableAreaWidth = Global.STAGE_WIDTH * 0.8f;
        float availableAreaHeight = Global.STAGE_HEIGHT * 0.8f;
        if (startPos.x > availableAreaWidth / 2f ||
            startPos.x < -availableAreaWidth / 2f ||
            startPos.z > availableAreaHeight / 2f ||
            startPos.z < -availableAreaHeight / 2f)
        {
            Vector3[] availableAreaVertexs = {  new Vector3(availableAreaWidth, 0.64f, availableAreaHeight),
                                                new Vector3(-availableAreaWidth, 0.64f, availableAreaHeight),
                                                new Vector3(availableAreaWidth, 0.64f, -availableAreaHeight),
                                                new Vector3(-availableAreaWidth, 0.64f, -availableAreaHeight)
                                             };
            Vector3 farVert = Vector3.zero;
            Vector3 nearVert = Vector3.positiveInfinity;

            foreach(Vector3 vert in availableAreaVertexs)
            {
                Vector3 toVertVec3 = startPos - vert;
                if((farVert - startPos).magnitude < toVertVec3.magnitude)
                {
                    farVert = vert;
                }
                if((nearVert - startPos).magnitude > toVertVec3.magnitude)
                {
                    nearVert = vert;
                }
            }
            Debug.Log("start: " + startPos);
            Debug.Log("far: " + farVert);
            Debug.Log("near: " + nearVert);
            ret = new Vector3( Random.Range(nearVert.x - startPos.x,farVert.x - startPos.x),
                                                                                        0 , 
                               Random.Range(nearVert.z - startPos.z,farVert.z - startPos.z)
                             ).normalized;
            ret *= Random.Range(15,25);
        }
        else
        {
            ret.x = Random.Range(-1f, 1f);
            ret.y = 0;
            ret.z = Random.Range(-1f, 1f);
            ret *= Random.Range(8,10);
        }
        return ret;
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

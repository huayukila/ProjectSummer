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

    private List<GameObject> mOnFieldSilk;
    private Stack<GameObject> mCapturedSilk;
    private bool mCanSpawnNewSilk = true;
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
            TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
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
        TypeEventSystem.Instance.Register<GameOver>(e => 
        {
            ResetGoldenSilkManager();
        }).UnregisterWhenGameObjectDestroyed(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        SetSilkSpawnTimer();
    }

    private void SetSilkSpawnTimer()
    {
        if (GoldenSilkSystem.Instance.CurrentSilkCount >= Global.MAX_SILK_COUNT)
            return;

        if (!mCanSpawnNewSilk)
            return;

        mCanSpawnNewSilk = false;
        Timer dropSilkTimer = new Timer(Time.time, Global.SILK_SPAWN_TIME,
            () =>
            {
                mCanSpawnNewSilk = true;
                GameObject obj = GoldenSilkSystem.Instance.DropNewSilk();
                //TODO リストに入れるタイミングを修正する
                if (obj != null)
                {
                    obj.GetComponent<IGoldenSilk>().SetActiveCallBack(obj =>
                    {
                        mOnFieldSilk.Add(obj);
                    });
                }
            });
        dropSilkTimer.StartTimer(this);
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
            Vector3[] availableAreaVertexs = {  new Vector3(availableAreaWidth / 3f, 0.64f, availableAreaHeight / 3f),
                                                new Vector3(-availableAreaWidth / 3f, 0.64f, availableAreaHeight / 3f),
                                                new Vector3(availableAreaWidth / 3f, 0.64f, -availableAreaHeight / 3f),
                                                new Vector3(-availableAreaWidth / 3f, 0.64f, -availableAreaHeight / 3f)
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
            //TODO refactorying
            float nearX = nearVert.x - startPos.x;
            float farX = farVert.x - startPos.x;
            float nearZ = nearVert.z - startPos.z;
            float farZ = farVert.z - startPos.z;
            float realX = Random.Range(nearX, farX);
            float realZ = Random.Range(nearZ, farZ);
            ret = new Vector3(realX,
                                                                                        0 ,
                              realZ
                             ).normalized;
            Debug.Log(ret);
            ret *= Random.Range(30,50);
        }
        else
        {
            ret.x = Random.Range(-1f, 1f);
            ret.y = 0;
            ret.z = Random.Range(-1f, 1f);
            ret *= Random.Range(10,15);
        }
        return ret;
    }

    private void ResetGoldenSilkManager()
    {
        mOnFieldSilk.Clear();
        mCapturedSilk.Clear();
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

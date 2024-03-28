using Gaming.PowerUp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnFieldSilk
{
    Vector3[] GetOnFieldSilkPos();
}

public interface IOnFieldItem
{
    Vector3[] GetOnFieldItemBoxPos();
}
public class ItemManager : Singleton<ItemManager>, IOnFieldSilk,IOnFieldItem
{
    private enum SpawnMode
    {
        Normal = 0,
        ItemFestival,
        SilkFestival,
        MegaFestival,
    }
    private readonly int MAX_ITEM_BOX_COUNT = 1;
    private List<GameObject> _onFieldSilks;
    private Stack<GameObject> _capturedSilks;
    private bool _canSpawnNewSilk = true;
    private List<GameObject> _onFieldItemBoxes;
    private SpawnMode _spawnMode = SpawnMode.Normal;
    // Start is called before the first frame update
    protected override void Awake()
    {
        _onFieldSilks = new List<GameObject>();
        _capturedSilks = new Stack<GameObject>();
        _onFieldItemBoxes = new List<GameObject>();
    }
    void Start()
    {
        // イベントを登録する
        #region Event Register
        TypeEventSystem.Instance.Register<SilkCapturedEvent>(e =>
        {

            foreach (var pos in e.positions)
            {
                int index = 0;
                while (index < _onFieldSilks.Count)
                {
                    GameObject silk = _onFieldSilks[index];
                    if (silk.transform.position == pos)
                    {
                        _capturedSilks.Push(silk);
                        _onFieldSilks.Remove(silk);
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
                GoldenSilkSystem.Instance.RecycleSilk(_capturedSilks.Pop());
                --e.dropCount;
                while (e.dropCount > 0)
                {
                    if(_capturedSilks.Count > 0)
                    {
                        GameObject dropSilk = _capturedSilks.Pop();
                        dropSilk.GetComponent<IGoldenSilk>()?.StartDrop(e.pos,e.pos + GetDropSilkEndPos(e.pos));
                        _onFieldSilks.Add(dropSilk);
                        --e.dropCount;
                    }
                }
            }
        }).UnregisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Instance.Register<GameOver>
            (e => 
            {
                DeinitItemManager();
            }
            ).UnregisterWhenGameObjectDestroyed(gameObject);

        TypeEventSystem.Instance.Register<GetNewItem>(e =>
        {
            foreach (var pos in e.ItemBoxsPos)
            {
                foreach(var itemBox in _onFieldItemBoxes)
                {
                    if(itemBox.transform.position == pos)
                    {
                        itemBox.GetComponent<ItemBoxController>().SetInactive();
                        break;
                    }
                }
            }
            TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
        }).UnregisterWhenGameObjectDestroyed(gameObject);

        #endregion
        for (int i = 0; i < MAX_ITEM_BOX_COUNT ; ++i)
        {
            _onFieldItemBoxes.Add(ItemSystem.Instance.SpawnItem(Global.ITEM_BOX_POS));
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch(_spawnMode)
        {
            case SpawnMode.Normal:
                SpawnNewSilk();
                break;
            case SpawnMode.ItemFestival:
                break;
            case SpawnMode.SilkFestival:
                break;
            case SpawnMode.MegaFestival:
                break;
        }

    }

    private void SpawnNewSilk()
    {
        if (GoldenSilkSystem.Instance.CurrentSilkCount >= Global.MAX_SILK_COUNT)
            return;

        if (!_canSpawnNewSilk)
            return;

        _canSpawnNewSilk = false;
        Timer dropSilkTimer = new Timer(Time.time, Global.SILK_SPAWN_TIME,
            () =>
            {
                _canSpawnNewSilk = true;
                GameObject obj = GoldenSilkSystem.Instance.DropNewSilk();
                //TODO リストに入れるタイミングを修正する
                obj.GetComponent<IGoldenSilk>().SetActiveCallBack(obj =>
                {
                    _onFieldSilks.Add(obj);
                });
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
            ret = new Vector3(realX, 0, realZ).normalized;
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

    private void DeinitItemManager()
    {
        _onFieldSilks.Clear();
        _capturedSilks.Clear();
        _onFieldItemBoxes.Clear();
    }
    public Vector3[] GetOnFieldSilkPos()
    {
        List<Vector3> silkPos = new List<Vector3>();
        foreach(var silk in _onFieldSilks)
        {
            silkPos.Add(silk.transform.position);
        }
        return silkPos.ToArray();

    }

    public Vector3[] GetOnFieldItemBoxPos() 
    {
        List<Vector3> itemBoxPos = new List<Vector3>();
        foreach(var itemBox in _onFieldItemBoxes)
        {
            itemBoxPos.Add(itemBox.transform.position);
        }
        return itemBoxPos.ToArray();
    }

}

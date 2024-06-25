using Gaming.PowerUp;
using Mirror;
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
public class ItemManager : View, IOnFieldSilk,IOnFieldItem
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
    private List<GameObject> _capturedSilks;
    private bool _canSpawnNewSilk = true;
    private List<GameObject> _onFieldItemBoxes;
    private List<GameObject> _unactivedItemBoxes;
    private SpawnMode _spawnMode = SpawnMode.Normal;

    private ItemSystem _itemSystem;

    private GameObject _silkPrefab;

    // Start is called before the first frame update
    private void Awake()
    {
        _onFieldSilks = new List<GameObject>();
        _capturedSilks = new List<GameObject>();
        _onFieldItemBoxes = new List<GameObject>();
        _unactivedItemBoxes = new List<GameObject>();

        _silkPrefab = GameResourceSystem.Instance.GetPrefabResource("GoldenSilk");
    }
    void Start()
    {
        // �C�x���g��o�^����
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
                        _capturedSilks.Add(silk);
                        _onFieldSilks.Remove(silk);
                        silk.GetComponent<GoldenSilkControl>().RpcSetInactive();
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
                CmdDestroySilk(_capturedSilks[0]);
                _capturedSilks.RemoveAt(0);
                --e.dropCount;
                while (e.dropCount > 0)
                {
                    if(_capturedSilks.Count > 0)
                    {
                        GameObject dropSilk = _capturedSilks[0];
                        _capturedSilks.RemoveAt(0);
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
                CmdDeinitItemManager();
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
                        _unactivedItemBoxes.Add(itemBox);
                        _onFieldItemBoxes.Remove(itemBox);
                        itemBox.SetActive(false);
                        Timer respawnItemBoxTimer = new Timer(Time.time,Global.ITEM_BOX_SPAWN_TIME,
                        () =>
                        {
                            _unactivedItemBoxes.Remove(itemBox);
                            _onFieldItemBoxes.Add(itemBox);
                            itemBox.SetActive(true);
                        });
                        respawnItemBoxTimer.StartTimer(this);

                        break;
                    }
                }
            }
            TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
        }).UnregisterWhenGameObjectDestroyed(gameObject);

        #endregion //Event Register


        _itemSystem = GetSystem<IItemSystem>() as ItemSystem;

        _itemSystem.RegisterManager(this);

        {
            CmdInitItemBox();
        }


    }

    // Update is called once per frame
    void Update()
    {
        switch(_spawnMode)
        {
            case SpawnMode.Normal:
                CmdSpawnNewSilk();
                break;
            case SpawnMode.ItemFestival:
                break;
            case SpawnMode.SilkFestival:
                break;
            case SpawnMode.MegaFestival:
                break;
        }
    }

    [Command]
    private void CmdSpawnNewSilk()
    {
        if (_onFieldSilks.Count >= Global.MAX_SILK_COUNT)
            return;

        if (!_canSpawnNewSilk)
            return;

        _canSpawnNewSilk = false;
        Timer dropSilkTimer = new Timer(Time.time, Global.SILK_SPAWN_TIME,
            () =>
            {
                _canSpawnNewSilk = true;
                RpcDropNewSilk();

            });
        dropSilkTimer.StartTimer(this);
    }

    [Command]
    private void CmdInitItemBox()
    {


        for (int i = 0; i < MAX_ITEM_BOX_COUNT ; ++i)
        {
            GameObject itemBox = _itemSystem.SpawnItem(Global.ITEM_BOX_POS);
            _onFieldItemBoxes.Add(itemBox);
            NetworkServer.Spawn(itemBox);
        }

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

    [Command]
    private void CmdDeinitItemManager()
    {
        ClearAllServerItems();
    }

    [Server]
    private void ClearAllServerItems()
    {
        ClearServerItems(_onFieldItemBoxes);
        ClearServerItems(_capturedSilks);
        ClearServerItems(_onFieldItemBoxes);
        ClearServerItems(_unactivedItemBoxes);
    }

    [Server]
    private void ClearServerItems(List<GameObject> items)
    {
        foreach(var item in items)
        {
            NetworkServer.Destroy(item);
        }
        items.Clear();
    }

    [ClientRpc]
    private void RpcDropNewSilk()
    {
        if(_onFieldSilks.Count + _capturedSilks.Count >= Global.MAX_SILK_COUNT)
            return;

            GameObject newSilk = Instantiate(_silkPrefab);

            //��������GoldenSilk�̃Z�b�g�A�b�v
            GoldenSilkControl ctrl = newSilk.GetComponent<GoldenSilkControl>();
            ctrl.StartSpawn(GetInSpaceRandomPosition());

            NetworkServer.Spawn(newSilk);

            //TODO ���X�g�ɓ����^�C�~���O���C������
            newSilk.GetComponent<IGoldenSilk>().SetActiveCallBack(obj =>
            {
                _onFieldSilks.Add(obj);
            });
    }
    [Command]
    private void CmdDestroySilk(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }

    private Vector3 GetInSpaceRandomPosition()
        {
            // �X�e�[�W�̈��͈͓��ɃC���X�^���X������
            float spawnAreaWidth = Global.STAGE_WIDTH / 2.5f;
            float spawnAreaHeight = Global.STAGE_HEIGHT / 2.5f;
            float posX = 0.0f;
            float posZ = 0.0f;
            while (posX == 0.0f || posZ == 0.0f)
            {
                posX = Random.Range(-spawnAreaWidth, spawnAreaWidth);
                posZ = Random.Range(-spawnAreaHeight, spawnAreaHeight);
            }
            return new Vector3(posX, 0.54f, posZ);
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

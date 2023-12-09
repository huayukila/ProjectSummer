using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class MiniMapController : MonoBehaviour
{
    //---------ミニマップ関連のコードです-----------
    public GameObject MiniMapSpider_Left;
    public GameObject MiniMapSpider_Right;

    public GameObject MiniMapSilkPrefab;
    private GameObject[] MiniMapSilkPrefabSave;// = new GameObject[3];

    private float miniMapSize;

    Vector3 silkPosInit;

    Vector3[] onFieldSilks;

    // Start is called before the first frame update
    void Start()
    {
        miniMapSize = 1.8f;
        if (MiniMapSilkPrefab != null)//GameProjectが入れているかどうか（以下同様）
        {
            silkPosInit = new Vector3(0, 1500, 0);
            MiniMapSilkPrefabSave = new GameObject[3];

            TypeEventSystem.Instance.Register<UpdataMiniMapSilkPos>(e =>
            {
                SetSilk();
            });
            //for (int i = 0; i < 3; i++)//黄金の糸を初期化
            //{
            //    MiniMapSilkPrefabSave[i] = Instantiate(MiniMapSilkPrefab, silkPosInit, Quaternion.identity);//黄金の糸3つ生成
            //    MiniMapSilkPrefabSave[i].transform.SetParent(transform.parent);//Canvasの中に生成
            //    //MiniMapSilkPrefabSave[i].SetActive(false);
            //    //testvector3s[i] = new Vector3(200.0f * i, 0, 200.0f * i);
            //}
        }
        RawImage miniMapImage = GetComponent<RawImage>();
        miniMapImage.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        if (MiniMapSpider_Left != null)
        {
            MiniMapSpider_Left.transform.position = silkPosInit;// GameManager.Instance.GetPlayerPos(1) + 
        }
        if (MiniMapSpider_Right != null)
        {
            MiniMapSpider_Right.transform.position = silkPosInit;// GameManager.Instance.GetPlayerPos(2) 
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (MiniMapSilkPrefab != null)
        {
            IOnFieldSilk iOnFieldSilk = GoldenSilkManager.Instance;
            onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();
            //for (int i = 0; i < 3; i++)
            //{
            //    if (isSilkFellOnTheFloor[i])
            //    {
            //        Vector3 tmp = onFieldSilks[i];
            //        MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
            //        MiniMapSilkPrefabSave[i].SetActive(true);
            //    }
            //}
            //if (onFieldSilks.Length != 0)
            //{
            //    for (int i = 0; i < onFieldSilks.Length; i++)
            //    {
            //        Vector3 tmp = onFieldSilks[i];
            //        MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
            //        MiniMapSilkPrefabSave[i].SetActive(true);
            //    }
            //}
        }

        if (MiniMapSpider_Left != null)
        {
            Vector3 Spider_Left = GameManager.Instance.GetPlayerPos(1) / Global.Map_Size_X * miniMapSize;
            MiniMapSpider_Left.transform.position = new Vector3(Spider_Left.x + transform.position.x, Spider_Left.z + transform.position.y, 0);
        }
        if (MiniMapSpider_Right != null)
        {
            Vector3 Spider_Right = GameManager.Instance.GetPlayerPos(2) / Global.Map_Size_X * miniMapSize;
            MiniMapSpider_Right.transform.position = new Vector3(Spider_Right.x + transform.position.x, Spider_Right.z + transform.position.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
        }

        //foreach (Vector3 Pos in onFieldSilks)
        //{
        //    silkPos.x = Pos.x;
        //    silkPos.y = Pos.z;
        //    Instantiate(MiniMapSilk, silkPos, Quaternion.identity);
        //}
    }

    //黄金の糸をスイッチon！
    private void SetSilk()
    {
        DestroySilk();
        Vector3 silkPos = new Vector3(0f, 0f, 0f);

        for (int i = 0; i < onFieldSilks.Length; i++) 
        {
            Vector3 tmp = onFieldSilks[i];
            tmp.y = onFieldSilks[i].z;
            tmp.z = 0.0f;
            tmp = tmp / Global.Map_Size_X * miniMapSize + transform.position;
            GameObject MiniMapSilkPrefabPut = Instantiate(MiniMapSilkPrefab, tmp, Quaternion.identity);
            MiniMapSilkPrefabPut.transform.SetParent(transform.parent);
            MiniMapSilkPrefabSave[i] = MiniMapSilkPrefabPut;
        }
    }
    private void DestroySilk()
    {
        for (int i = 0; i < MiniMapSilkPrefabSave.Length; i++)
        {
            if (MiniMapSilkPrefabSave[i] != null)
            {
                Destroy(MiniMapSilkPrefabSave[i]);
            }
        }
    }
}

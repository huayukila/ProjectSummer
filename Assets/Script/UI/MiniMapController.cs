using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class MiniMapController : MonoBehaviour
{
    //---------ミニマップ関連のコードです-----------

    //ミニマップのスパイダー
    public GameObject MiniMapSpider_Left;
    public GameObject MiniMapSpider_Right;

    //ミニマップの黄金の糸
    public GameObject MiniMapSilkPrefab;
    private GameObject[] MiniMapSilkPrefabSave;// = new GameObject[3];
    private Vector3[] onFieldSilks;

    //設定値
    private float miniMapSize;
    private Vector3 PosInit;

    IOnFieldSilk iOnFieldSilk;

    

    //テスト用
    //float testTimer = Global.SET_GAME_TIME;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        miniMapSize = rectTransform.sizeDelta.x / 100.0f;

        PosInit = new Vector3(0, 1500, 0);
        iOnFieldSilk = GoldenSilkManager.Instance;
        //IOnFieldSilk iOnFieldSilk = GoldenSilkManager.Instance;
        //onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();
        if (MiniMapSilkPrefab != null)//GameProjectが入れているかどうか（以下同様）
        {
            MiniMapSilkPrefabSave = new GameObject[3];

            TypeEventSystem.Instance.Register<UpdataMiniMapSilkPos>(e =>
            {
                SetSilk();
            });
            //for (int i = 0; i < 3; i++)//黄金の糸を初期化
            //{
            //    MiniMapSilkPrefabSave[i] = Instantiate(MiniMapSilkPrefab, PosInit, Quaternion.identity);//黄金の糸3つ生成
            //    MiniMapSilkPrefabSave[i].transform.SetParent(transform.parent);//Canvasの中に生成
            //    //MiniMapSilkPrefabSave[i].SetActive(false);
            //    //testvector3s[i] = new Vector3(200.0f * i, 0, 200.0f * i);
            //}
        }
        RawImage miniMapImage = GetComponent<RawImage>();
        miniMapImage.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        if (MiniMapSpider_Left != null)
        {
            MiniMapSpider_Left.transform.position = PosInit;// GameManager.Instance.GetPlayerPos(1) + 
        }
        if (MiniMapSpider_Right != null)
        {
            MiniMapSpider_Right.transform.position = PosInit;// GameManager.Instance.GetPlayerPos(2) 
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        #region 一時的不要なコード
        //if (MiniMapSilkPrefab != null)
        //{

        //    //for (int i = 0; i < 3; i++)
        //    //{
        //    //    if (isSilkFellOnTheFloor[i])
        //    //    {
        //    //        Vector3 tmp = onFieldSilks[i];
        //    //        MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
        //    //        MiniMapSilkPrefabSave[i].SetActive(true);
        //    //    }
        //    //}
        //    //if (onFieldSilks.Length != 0)
        //    //{
        //    //    for (int i = 0; i < onFieldSilks.Length; i++)
        //    //    {
        //    //        Vector3 tmp = onFieldSilks[i];
        //    //        MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
        //    //        MiniMapSilkPrefabSave[i].SetActive(true);
        //    //    }
        //    //}
        //}
        #endregion

        onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();

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

        #region　黄金の糸テスト用
        //TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();
        
        #endregion
    }

    //黄金の糸をスイッチon！
    public void SetSilk()
    {
        DestroySilk();

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

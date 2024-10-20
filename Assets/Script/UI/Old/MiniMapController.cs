using System;
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
    private GameObject[] MiniMapSilkPrefabSave; // = new GameObject[3];
    private Vector3[] onFieldSilks;

    //設定値
    //TODO 疑問点
    //miniMapSizeはVector2を使えば十分ですが、Vector3を使う理由は？
    private Vector2 miniMapSize;
    private Vector3 PosInit;

    IOnFieldSilk iOnFieldSilk;

    //テスト用
    //float testTimer = Global.SET_GAME_TIME;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        miniMapSize.x = rectTransform.sizeDelta.x / 100.0f;
        miniMapSize.y = rectTransform.sizeDelta.y / 100.0f;

        PosInit = new Vector3(0, 1500, 0);
        iOnFieldSilk = ItemManager.Instance;
        //IOnFieldSilk iOnFieldSilk = GoldenSilkManager.Instance;
        //onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();
        if (MiniMapSilkPrefab != null) //GameProjectが入れているかどうか（以下同様）
        {
            MiniMapSilkPrefabSave = new GameObject[3];

            TypeEventSystem.Instance.Register<UpdataMiniMapSilkPos>(e => { SetSilk(); });
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
            // MiniMapSpider_Left.transform.position = PosInit; // GameManager.Instance.GetPlayerPos(1) + 
            MiniMapSpider_Left.transform.localPosition = PosInit; // GameManager.Instance.GetPlayerPos(1) + 
        }

        if (MiniMapSpider_Right != null)
        {
            // MiniMapSpider_Right.transform.position = PosInit; // GameManager.Instance.GetPlayerPos(2) 
            MiniMapSpider_Right.transform.localPosition = PosInit; // GameManager.Instance.GetPlayerPos(2) 
        }
    }

    //TODO changed by Mai
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

        //TODO changed by Mai毎回取得する必要がない
        //onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();

        if (MiniMapSpider_Left != null)
        {
            //TODO changed by Mai プレイヤーが死亡した場合更新しません。
            if (!GameManager.Instance.IsPlayerDead(1))
            {
                Vector3 Spider_Left = GameManager.Instance.GetPlayerPos(1);
                // MiniMapSpider_Left.transform.position = new Vector3(Spider_Left.x / Global.MAP_SIZE_WIDTH * miniMapSize.x + transform.position.x,
                //                                     Spider_Left.z / Global.MAP_SIZE_HEIGHT * miniMapSize.y + transform.position.y,
                //                                     0.0f);
                MiniMapSpider_Left.transform.localPosition = new Vector3(
                    Spider_Left.x / Global.MAP_SIZE_WIDTH * miniMapSize.x + transform.localPosition.x,
                    Spider_Left.z / Global.MAP_SIZE_HEIGHT * miniMapSize.y + transform.localPosition.y,
                    0.0f);
            }
        }

        if (MiniMapSpider_Right != null)
        {
            //TODO changed by Mai プレイヤーが死亡した場合更新しません。
            if (!GameManager.Instance.IsPlayerDead(2))
            {
                Vector3 Spider_Right = GameManager.Instance.GetPlayerPos(2);
                // MiniMapSpider_Right.transform.position = new Vector3(Spider_Right.x / Global.MAP_SIZE_WIDTH * miniMapSize.x + transform.position.x,
                //                                                      Spider_Right.z / Global.MAP_SIZE_HEIGHT * miniMapSize.y + transform.position.y,
                //                                                      0.0f);
                MiniMapSpider_Right.transform.localPosition = new Vector3(
                    Spider_Right.x / Global.MAP_SIZE_WIDTH * miniMapSize.x + transform.localPosition.x,
                    Spider_Right.z / Global.MAP_SIZE_HEIGHT * miniMapSize.y + transform.localPosition.y,
                    0.0f);
            }
        }

        #region　黄金の糸テスト用

        //TypeEventSystem.Instance.Send<UpdataMiniMapSilkPos>();

        #endregion
    }

    //黄金の糸をスイッチon！
    //TODO　changed by Mai publicする必要がない、privateにした
    private void SetSilk()
    {
        //TODO changed by Mai拾った時に更新する
        onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();
        DestroySilk();
        if (onFieldSilks != null)
        {
            for (int i = 0; i < onFieldSilks.Length; i++)
            {
                GameObject MiniMapSilkPrefabPut = Instantiate(MiniMapSilkPrefab, Vector3.zero, Quaternion.identity);
                MiniMapSilkPrefabPut.transform.SetParent(transform.parent);
                Vector3 tmp = onFieldSilks[i];
                tmp.x = tmp.x / Global.MAP_SIZE_WIDTH * miniMapSize.x;
                tmp.y = onFieldSilks[i].z / Global.MAP_SIZE_HEIGHT * miniMapSize.y;
                tmp.z = 0.0f;
                tmp += transform.localPosition;
                MiniMapSilkPrefabPut.transform.localPosition = tmp;
                MiniMapSilkPrefabSave[i] = MiniMapSilkPrefabPut;
            }
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
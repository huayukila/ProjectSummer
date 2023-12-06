using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MiniMapController : MonoBehaviour
{
    //ミニマップ関連のGameObject-----------
    public GameObject MiniMapSpider_Left;
    public GameObject MiniMapSpider_Right;

    public GameObject MiniMapSilkPrefab;
    private GameObject[] MiniMapSilkPrefabSave = new GameObject[3];

    Vector3 silkPos = new Vector3(1500,1500,0);

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 3; i++)
        {
            MiniMapSilkPrefabSave[i] = Instantiate(MiniMapSilkPrefab, new Vector3(2500,2500,0), Quaternion.identity);
            MiniMapSilkPrefabSave[i].transform.SetParent(transform.parent);
            MiniMapSilkPrefabSave[i].SetActive(false);
        }
        RawImage miniMapImage = GetComponent<RawImage>();
        miniMapImage.texture = PolygonPaintManager.Instance.GetMiniMapRT();
        if (MiniMapSpider_Left != null)
        {
            MiniMapSpider_Left.transform.position = GameManager.Instance.GetPlayerPos(1) + silkPos;
        }
        if (MiniMapSpider_Right != null)
        {
            MiniMapSpider_Right.transform.position = GameManager.Instance.GetPlayerPos(2) + silkPos;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        IOnFieldSilk iOnFieldSilk = GoldenSilkManager.Instance;
        Vector3[] onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();

        if (MiniMapSpider_Left != null)
        {
            Vector3 Spider_Left = GameManager.Instance.GetPlayerPos(1);
            MiniMapSpider_Left.transform.position = new Vector3(Spider_Left.x + transform.position.x, Spider_Left.z + transform.position.y, 0);
        }
        if (MiniMapSpider_Right != null)
        {
            Vector3 Spider_Right = GameManager.Instance.GetPlayerPos(2);
            MiniMapSpider_Right.transform.position = new Vector3(Spider_Right.x + transform.position.x, Spider_Right.z + transform.position.y, 0);
        }
        if(onFieldSilks.Length != 0)
        {
            for (int i = 0; i < onFieldSilks.Length; i++) 
            {
                Vector3 tmp = onFieldSilks[i];
                MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
                MiniMapSilkPrefabSave[i].SetActive(true);
            }
        }
    
        //foreach (Vector3 Pos in onFieldSilks)
        //{
        //    silkPos.x = Pos.x;
        //    silkPos.y = Pos.z;
        //    Instantiate(MiniMapSilk, silkPos, Quaternion.identity);
        //}
    }
}

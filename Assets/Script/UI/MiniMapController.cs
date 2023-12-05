using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MiniMapController : MonoBehaviour
{
    //ミニマップ関連のGameObject-----------
    public GameObject MiniMapSpider_Left;
    public GameObject MiniMapSpider_Right;
    public GameObject MiniMapSilk;
    Vector3 silkPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
        if (MiniMapSpider_Left != null)
        {
            MiniMapSpider_Left.transform.position = GameManager.Instance.GetPlayerPos(1);
        }
        if (MiniMapSpider_Right != null)
        {
            MiniMapSpider_Right.transform.position = GameManager.Instance.GetPlayerPos(2);
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

        foreach (Vector3 Pos in onFieldSilks)
        {
            silkPos.x=Pos.x;
            silkPos.y=Pos.z;
            Instantiate(MiniMapSilk, silkPos, Quaternion.identity);   
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class MiniMapController : MonoBehaviour
{
    //---------�~�j�}�b�v�֘A�̃R�[�h�ł�-----------
    public GameObject MiniMapSpider_Left;
    public GameObject MiniMapSpider_Right;

    public GameObject MiniMapSilkPrefab;
    private GameObject[] MiniMapSilkPrefabSave = new GameObject[3];

    Vector3 silkPosInit = new Vector3(2500, 2500, 0);

    private bool[] isSilkFellOnTheFloor = new bool[3];//�����̎��̃X�C�b�`3�p��

    // Start is called before the first frame update
    void Start()
    {
        if (MiniMapSilkPrefab != null)//GameProject������Ă��邩�ǂ����i�ȉ����l�j
        {
            for (int i = 0; i < 3; i++)//�����̎��̃X�C�b�`��������
            {
                isSilkFellOnTheFloor[i] = false;
            }

            TypeEventSystem.Instance.Register<SilkFellOnTheFloor>(e =>
            {
                SetSilkAndActive(e.SilkNomber);
            }).UnregisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Instance.Register<ResetSilk>(e =>
            {
                ResetSilk(e.SilkNomber);
            }).UnregisterWhenGameObjectDestroyed(gameObject);

            for (int i = 0; i < 3; i++)//�����̎���������
            {
                MiniMapSilkPrefabSave[i] = Instantiate(MiniMapSilkPrefab, silkPosInit, Quaternion.identity);//�����̎�3����
                MiniMapSilkPrefabSave[i].transform.SetParent(transform.parent);//Canvas�̒��ɐ���
                MiniMapSilkPrefabSave[i].SetActive(false);
            }
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
            Vector3[] onFieldSilks = iOnFieldSilk.GetOnFieldSilkPos();
            for (int i = 0; i < 3; i++)
            {
                if (isSilkFellOnTheFloor[i])
                {
                    Vector3 tmp = onFieldSilks[i];
                    MiniMapSilkPrefabSave[i].transform.position = new Vector3(tmp.x + transform.position.x, tmp.z + transform.position.y, 0);
                    MiniMapSilkPrefabSave[i].SetActive(true);
                }
            }
            //if (onFieldSilks.Length != 0)
            //{
            //    for (int i = 0; i < onFieldSilks.Length; i++)
            //    {

            //    }
            //}
        }

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

        //foreach (Vector3 Pos in onFieldSilks)
        //{
        //    silkPos.x = Pos.x;
        //    silkPos.y = Pos.z;
        //    Instantiate(MiniMapSilk, silkPos, Quaternion.identity);
        //}
    }

    //�����̎����X�C�b�`on�I
    private void SetSilkAndActive(int silkNomber)
    {
        isSilkFellOnTheFloor[silkNomber] = true;
    }

    //�����̎������Z�b�g
    private void ResetSilk(int silkNomber)
    {
        isSilkFellOnTheFloor[silkNomber] = false;
        MiniMapSilkPrefabSave[silkNomber].SetActive(false);
        MiniMapSilkPrefabSave[silkNomber].transform.position = silkPosInit;
    }
}

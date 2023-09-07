using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUICon : MonoBehaviour
{
    public GameObject Spider01;
    public GameObject Spider02;
    public bool Spider01OnReght = true;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //this.Spider01 = GameObject.Find("Spider01");
        //this.Spider02 = GameObject.Find("Spider02");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void SpiderGethering()
    {
        float imagemove = 20.0f;
        this.Spider01.transform.Translate(-10, 0, 0);
        if (this.Spider01.transform.position.x <= 600.0f)
        {
            this.Spider01.transform.Translate(0, 0, 0);
        }
    }
}

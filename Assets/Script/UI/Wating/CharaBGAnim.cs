using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaBGAnim : MonoBehaviour
{
    public Sprite[] imgs;

    public float LoopTime;

    private float durationTime;
    private Image bg;

    private int index = 0;

    private void Awake()
    {
        bg = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        durationTime += Time.deltaTime;

        if (durationTime > LoopTime)
        {
            index++;
            bg.sprite = imgs[index % imgs.Length];
            durationTime = 0;
        }
    }
}
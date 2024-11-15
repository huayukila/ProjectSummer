using System;
using System.Collections;
using System.Collections.Generic;
using Es.InkPainter;
using UnityEngine;

public class testvanish : MonoBehaviour
{
    private DirtPlaneEffect dirtScreenComp;
    public Texture BrushTex;

    public float VanishTimeInterval;
    public float VanishCntDown;
    // Start is called before the first frame update
    void Start()
    {
        // target.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.down,
		//                  Camera.main.transform.rotation * Vector3.back);
        dirtScreenComp = new DirtPlaneEffect(Camera.main, BrushTex, 0.1f, Color.white);
        VanishCntDown = VanishTimeInterval;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     target.Paint(brush,target.transform.position);
        // }

        // target.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(new Vector3(90, Camera.main.transform.eulerAngles.y - 180, 0));

        // Debug.Log(target.transform.eulerAngles);

        // target.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 10f;

        if (VanishCntDown > 0f)
        {
            dirtScreenComp.ResetDirt();
            Span<Vector2> vectors = stackalloc Vector2[4];
            
            vectors[0].x = 0;
            vectors[0].y = 0;
            vectors[1].x = 1;
            vectors[1].y = 1;
            vectors[2].x = .5f;
            vectors[2].y = 0f;

            vectors[3].x = 0.5f;
            vectors[3].y = 0.5f;

            dirtScreenComp.PaintUV(vectors.ToArray());
        }

        VanishCntDown -= Time.deltaTime;
        if (VanishCntDown <= 0f)
        {
            VanishCntDown = 0f;
        }

        Color newColor = dirtScreenComp.DirtBrushColor;
        newColor.a = VanishCntDown / VanishTimeInterval;
        dirtScreenComp.DirtBrushColor = newColor;

        dirtScreenComp.Update();
    }

}

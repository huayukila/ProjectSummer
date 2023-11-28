using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class camtest : MonoBehaviour
{
    public GameObject[] cubes;
    public float smooth;
    Camera[] cams;
    // Start is called before the first frame update
    void Start()
    {
        cams = Camera.allCameras;
        for(int i = 0; i< cubes.Length; ++i)
        {
            if (i >= cams.Length)
                break;
            cams[i].transform.position = cubes[i].transform.position + Vector3.back * 5;
            cams[i].rect = new Rect((float)i/(float)cams.Length, 0.0f, 1.0f / cams.Length, 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        for(int i = 0;i < cubes.Length; ++i)
        {
            if (i >= cams.Length)
                break;
            Vector3 camPos = cubes[i].transform.position + Vector3.back * 5;
            cams[i].transform.position = Vector3.Lerp(cams[i].transform.position, camPos, smooth * Time.deltaTime);

        }
    }
}

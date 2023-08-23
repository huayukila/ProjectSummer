using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Player
{
    const int MAX_TAIL = 3;
    float movement;
    float rotate;
    public float dropVertexInterval;
    float dropVertexTimer;
    public GameObject tailPrefab;
    float tailSpawnTimer;
    public float tailSpawnInterval;
    int currentTailCount;
    GameObject lastTail;
    void Start()
    {
        movement = 0.0f;
        rotate = 0.0f;
        dropVertexTimer = 0.0f;
        tailSpawnTimer = 0.0f;
        currentTailCount = 0;
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        bool turn;
        turn = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        float rotate = Input.GetAxis("Horizontal");
        if (movement < 1.0f)
        {
            movement += acceleration * Time.deltaTime;
        }
        else
        {
            movement = 1.0f;
        }
        if(currentTailCount < MAX_TAIL)
        {
            tailSpawnTimer += Time.deltaTime;
            if(tailSpawnTimer >= tailSpawnInterval)
            {
                tailSpawnTimer = 0.0f;
                GameObject Tail = Instantiate(tailPrefab);
                if(transform.childCount <= 0)
                {    
                    Tail.transform.parent = transform;
                    Tail.transform.localPosition = new Vector3(0.0f, 0.0f, -1.5f);
                }
                else
                {
                    Tail.transform.parent = lastTail.transform;
                    Tail.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
                    Tail.transform.localScale = Vector3.one;
                }

                Tail.transform.localRotation = Quaternion.identity;
                lastTail = Tail;
                currentTailCount++;
            }
        }
        Vector3 movementDirection = new Vector3(0.0f, 0.0f, movement);
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime);
        if(movementDirection != Vector3.zero)
        {
            if(turn)
            {
                transform.Rotate(0.0f, rotate * rotationSpeed * Time.deltaTime, 0.0f);
            }
        }
    }
}

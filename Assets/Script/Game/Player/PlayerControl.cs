using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Player
{
    float movement;
    GameObject rootTail;
    public GameObject tailPrefab;
    void Start()
    {
        movement = 0.0f;
        SetRootTail();
    }

    // Update is called once per frame
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


    private void FixedUpdate()
    {
        
    }

    private void SetRootTail()
    {
        GameObject tail = Instantiate(tailPrefab, transform);

        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.parent = transform;
        tail.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);

        rootTail = tail;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailControl : MonoBehaviour
{
    const int MAX_TAIL = 50;
    public GameObject tailPrefab;
    private int tailsCount;
    GameObject[] tails;
    // Start is called before the first frame update
    void Awake()
    {
        tails = new GameObject[50];
        tails[0] = gameObject;
        tailsCount++;
        GenerateTails();
    }

    private void GenerateTails()
    {
        while(tailsCount < MAX_TAIL)
        {
            GameObject tail = Instantiate(tailPrefab);
            tail.transform.localScale = Vector3.one * 0.2f;
            tail.transform.rotation = transform.rotation;
            tail.transform.position = transform.position;
            tails[tailsCount] = tail;
            ++tailsCount;
        }
    }

    private void FixedUpdate()
    {
        for(int i = MAX_TAIL - 1; i > 0 ; --i)
        {
            tails[i].transform.position = tails[i-1].transform.position;
            tails[i].transform.rotation = tails[i-1].transform.rotation;
        }
    }

    public GameObject[] GetTailsObject()
    {
        return tails;
    }

    public GameObject GetTipTail()
    {
        return tails[tails.Length-1];
    }
}

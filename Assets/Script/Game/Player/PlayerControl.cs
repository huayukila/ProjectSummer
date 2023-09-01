<<<<<<< Updated upstream
=======
using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEngine;

public class PlayerControl : Player
{
    float currentMoveSpeed;
    GameObject rootTail;
    GameObject tipTail;
<<<<<<< Updated upstream

=======
    TailControl tc;
    public Paintable p;
>>>>>>> Stashed changes
    public GameObject tailPrefab;

    private void Awake()
    {
        SetTail();
        tipTail?.AddComponent<DropPointControl>();
        tc = rootTail.GetComponent<TailControl>();
    }
    void Start()
    {
        currentMoveSpeed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
    }


    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        currentMoveSpeed = currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : currentMoveSpeed + acceleration * Time.deltaTime;

        Vector3 movementDirection = Vector3.forward * currentMoveSpeed;
        transform.Translate(movementDirection * Time.fixedDeltaTime);

        Vector3 rotationDirection = new Vector3(horizontal, 0.0f, vertical);
        if (rotationDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab, transform);

        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.parent = transform;
        tail.transform.localPosition = new Vector3(0.0f, 0.0f, -2.5f);
        tail.AddComponent<TailControl>();

        rootTail = tail;

        TailControl temp = rootTail.GetComponent<TailControl>();
        tipTail = temp?.GetTipTail();

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DropPoint"))
        {
            List<Vector3> temp = new List<Vector3>();
            temp = DropPointManager.Instance.GetPaintablePoints(other.gameObject);

            GameObject[] tempTails = tc?.GetTails();
            if(tempTails != null)
            {
                for(int i=0;i<Global.iMAX_TAIL_COUNT;++i)
                {
                    temp.Add(tempTails[i].transform.position);
                }
            }
            temp.Add(transform.position);
            for(int i = 0;i<temp.Count;++i)
            {
                float x = temp[i].x;
                float z = temp[i].z;
                temp[i] = new Vector3(x,0.0f,z);
            }
            PolygonPaintManager.Instance.Paint(p, temp.ToArray());
        }
    }
}

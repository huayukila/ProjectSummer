using UnityEngine;

public class PlayerControl : Player
{
    float currentMoveSpeed;
    GameObject rootTail;
    GameObject tipTail;
    public GameObject tailPrefab;

    private void Awake()
    {
        SetTail();
        tipTail?.AddComponent<DropPointControl>();
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
        tail.transform.localPosition = new Vector3(0.0f, 0.0f, -0.5f);
        tail.AddComponent<TailControl>();
        tail.AddComponent<TailRender>();

        rootTail = tail;

        TailControl temp = rootTail.GetComponent<TailControl>();
        tipTail = temp?.GetTipTail();

    }


}

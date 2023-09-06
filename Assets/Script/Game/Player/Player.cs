using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public float maxMoveSpeed;
    [Min(0.0f)] public float acceleration;
    [Min(0.0f)] public float rotationSpeed;    
    public GameObject tailPrefab;
    public Color _areaColor;

    float _currentMoveSpeed;                    // プレイヤーの現在速度

    protected GameObject _rootTail;             // 尻尾の頭
    protected GameObject _tipTail;              // 尻尾の尾

    protected bool _isPainting;                 // 地面に描けるかどうかの信号
    float _timer;                               // 前回の描画が終わってからの経過時間
    protected Rigidbody _rigidBody;

    /// <summary>
    /// 尻尾をインスタンス化する
    /// </summary>
    private void SetTail()
    {
        GameObject tail = Instantiate(tailPrefab);
        tail.name = gameObject.name + "Tail";
        tail.transform.localScale = Vector3.one * 0.2f;
        tail.transform.localRotation = transform.rotation;
        tail.transform.position = transform.position;
        tail.AddComponent<TailControl>();

        _rootTail = tail;

        TailControl tc = _rootTail.GetComponent<TailControl>();
        _tipTail = tc.GetTipTail();

    }


    private void OnCollisionEnter(Collision collision)
    {
        bool isCollision = collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall");
        if (isCollision)
        {
            _rigidBody.Sleep();
            SetDeadStatus();
        }

    }

    protected virtual void Awake()
    {
        _isPainting = false;
        _timer = 0.0f;
        _currentMoveSpeed = 0.0f;
        SetTail();
        _rigidBody = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        // 描画を制限する（α版）
        if (_isPainting)
        {
            _timer += Time.deltaTime;
        }
        if (_timer >= 0.5f)
        {
            _isPainting = false;
            _timer = 0.0f;
        }

    }
    protected virtual void FixedUpdate()
    {
        PlayerMovement();
        _rootTail.transform.position = transform.position;
    }

    /// <summary>
    /// プレイヤーの移動を制御する
    /// </summary>
    private void PlayerMovement()
    {
        // 加速運動をして、最大速度まで加速する
        _currentMoveSpeed = _currentMoveSpeed >= maxMoveSpeed ? maxMoveSpeed : _currentMoveSpeed + acceleration * Time.deltaTime;
        Vector3 moveDirection = transform.forward * _currentMoveSpeed * Time.fixedDeltaTime;
        _rigidBody.velocity = moveDirection;
    }

    protected void SetDeadStatus()
    {
        gameObject.SetActive(false);
        _rootTail.GetComponent<TailControl>().SetDeactive();
        _rigidBody.velocity = Vector3.zero;
    }
    public virtual void Respawn()
    {

        ResetPlayerTransform();
        _rootTail.GetComponent<TailControl>().SetActive(transform.position);
        gameObject.SetActive(true);
        _rigidBody.WakeUp();
    }

    /// <summary>
    /// プレイヤーの回転を制御する
    /// </summary>
    protected abstract void PlayerRotation();

    protected abstract void ResetPlayerTransform();
}

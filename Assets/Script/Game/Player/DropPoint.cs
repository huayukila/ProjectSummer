using UnityEngine;

public class DropPoint : MonoBehaviour
{
    Timer _timer;        // DropPointのタイマー
    float _radius;
    bool _turnOn;
    void Awake()
    {
        // タイマーを初期化する
        _timer = new Timer();
        _timer.SetTimer(Global.DROP_POINT_ALIVE_TIME,
            () =>
            {
                // 時間が経ったらDropPointを消す
                Destroy(gameObject);
            }
            );
        _radius = GetComponent<SphereCollider>().radius;
        _turnOn = false;
    }

    void Update()
    {
        // タイマーが終わるまでチェック
        if(_timer.IsTimerFinished())
        {
            // どのプレイヤーが落としたDropPointをチェック
            if(gameObject.CompareTag("DropPoint1"))
            {
                DropPointManager.Instance.PlayerOneRemovePoint(gameObject);
            }
            else
            {
                DropPointManager.Instance.PlayerTwoRemovePoint(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if(!_turnOn)
        {
            CheckIsAbleCollide();
        } 
    }

    /// <summary>
    /// DropPointとプレイヤーは一定の距離以上離れたらDropPointのコライダーをアクティブに設定する
    /// </summary>
    private void CheckIsAbleCollide()
    {
        Vector3 playerPos = gameObject.CompareTag("DropPoint1") ? GameManager.Instance.playerOne.transform.position : GameManager.Instance.playerTwo.transform.position;
        Vector3 distance = playerPos - gameObject.transform.position;
        // 5.0fは後ほどプレイヤーのコライダーの半径に置き換える
        if (distance.magnitude > _radius + 5.0f)
        {
            _turnOn = true;
            GetComponent<SphereCollider>().enabled = true;
        }
    }

}

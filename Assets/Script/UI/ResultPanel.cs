using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    public RectTransform BindleTrs;
    public RectTransform LeftChara;
    public RectTransform RightChara;

    public float durationTime;
    public int LoopTimes = 3;
    public int MaxRange;
    public int MinRange;
    private Vector3 leftEndPoint;
    private Vector3 RightEndPoint;

    private void Awake()
    {
        TypeEventSystem.Instance.Register<GameOver>(e =>
            HandleShowUI()).UnregisterWhenGameObjectDestroyed(gameObject);
        leftEndPoint = BindleTrs.position + BindleTrs.right * -680;
        RightEndPoint = BindleTrs.position + BindleTrs.right * 680;
    }

    [Button]
    void HandleShowUI()
    {
        gameObject.SetActive(true);
        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(Vector3.one, 2f)).SetEase(Ease.OutBack);


        float moveDistance = Random.Range(MinRange, MaxRange);
        for (int i = 0; i < LoopTimes; i++)
        {
            sequence.Append(BindleTrs.DOMove(BindleTrs.position + BindleTrs.right * moveDistance, durationTime)
                .SetEase(Ease.OutElastic));
            sequence.Join(LeftChara.DOPunchScale(Vector3.one * 0.2f, 1f));
            sequence.Join(RightChara.DOPunchScale(Vector3.one * 0.2f, 1f));
            moveDistance -= Mathf.Abs(moveDistance) + Random.Range(MinRange, MaxRange);
        }

        // float[] values = PolygonPaintManager.Instance.GetPlayersAreaPercent();

        if (true)
        {
            sequence.Append(BindleTrs.DOMove(leftEndPoint, durationTime).SetEase(Ease.OutElastic))
                .AppendCallback(() => LeftChara.SetParent(LeftChara.parent.parent.parent));
            sequence.Join(LeftChara.DOLocalMove(Vector3.zero, 2.0f).SetEase(Ease.InBack));
        }
        else
        {
            sequence.Append(BindleTrs.DOMove(RightEndPoint, durationTime).SetEase(Ease.OutElastic))
                .AppendCallback(() => RightChara.SetParent(RightChara.parent.parent.parent));
            sequence.Join(RightChara.DOLocalMove(Vector3.zero, 2.0f).SetEase(Ease.InBack));
        }
        sequence.SetAutoKill();
    }

    [Button]
    private void Reset()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(BindleTrs.DOLocalMove(new Vector3(36, -43, 0), 2f));
        sequence.SetAutoKill();
    }
}
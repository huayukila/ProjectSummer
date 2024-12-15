using DG.Tweening;
using NaughtyAttributes;
using TMPro;
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
    public TextMeshProUGUI leftScore;
    public TextMeshProUGUI rightScore;

    private Vector3 leftEndPoint;
    private Vector3 RightEndPoint;

    private float rightNum = 80;
    private float currentRightNum = 0;
    private float leftNum = 20;
    private float currentLeftNum = 0;

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

        sequence.Append(transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack)).AppendCallback(StartScoreAnimation);

        Random.InitState((int)Time.realtimeSinceStartup);
        float moveDistance = GenerateRandomMoveDistance();
        for (int i = 0; i < LoopTimes; i++)
        {
            sequence.Append(BindleTrs.DOMove(BindleTrs.position + BindleTrs.right * moveDistance, durationTime)
                .SetEase(Ease.OutElastic));

            sequence.Join(LeftChara.DOPunchScale(Vector3.one * 0.2f, 1f));
            sequence.Join(RightChara.DOPunchScale(Vector3.one * 0.2f, 1f));
            moveDistance = GenerateRandomMoveDistance();
        }

        if (false)
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

    private void StartScoreAnimation()
    {
        var scoreSequence = DOTween.Sequence();

        scoreSequence.Append(DOTween.To(() => currentLeftNum, x => currentLeftNum = x, leftNum, 3f)
            .OnUpdate(() => { leftScore.text = currentLeftNum.ToString("F1"); })
            .SetEase(Ease.OutQuad));

        scoreSequence.Join(DOTween.To(() => currentRightNum, x => currentRightNum = x, rightNum, 3f)
            .OnUpdate(() => { rightScore.text = currentRightNum.ToString("F1"); })
            .SetEase(Ease.OutQuad));

        scoreSequence.SetAutoKill(true);
    }

    private float GenerateRandomMoveDistance()
    {
        float magnitude = Random.Range(MinRange, MaxRange);
        float sign = Random.value > 0.5f ? 1f : -1f;
        return magnitude * sign;
    }


    [Button]
    private void Reset()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(BindleTrs.DOLocalMove(new Vector3(36, -43, 0), 2f));
        sequence.SetAutoKill();
    }
}
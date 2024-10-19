using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingPanel : MonoBehaviour
{
    public Image leftOkImg;

    public Sprite leftOkSprite;

    public Image rightOkImg;

    public Sprite rightOkSprite;

    public Image LeftCharaBG;

    public Image RightCharaBG;

    public Transform Effect;

    private bool isLeftReady = false;
    private bool isRightReady = false;

    // Start is called before the first frame update
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (isRightReady && isLeftReady)
            return;
        if (Input.GetKeyDown(KeyCode.A) && !isLeftReady)
        {
            isLeftReady = true;
            leftOkImg.sprite = leftOkSprite;
            leftOkImg.rectTransform.DOScale(new Vector3(15, 10, 0), 0.2f).SetLoops(2, LoopType.Yoyo);
            HandleAllReady();
        }

        if (Input.GetKeyDown(KeyCode.S) && !isRightReady)
        {
            isRightReady = true;
            rightOkImg.sprite = rightOkSprite;
            rightOkImg.rectTransform.DOScale(new Vector3(15, 10, 0), 0.2f).SetLoops(2, LoopType.Yoyo);
            HandleAllReady();
        }
    }


    void HandleAllReady()
    {
        if (isRightReady && isLeftReady)
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(LeftCharaBG.rectTransform.DOAnchorPos(new Vector2(-960, 0), 0.5f).SetEase(Ease.OutQuad));
            sequence.Join(RightCharaBG.rectTransform.DOAnchorPos(new Vector2(960, 0), 0.5f).SetEase(Ease.OutQuad));

            sequence.Append(LeftCharaBG.rectTransform.DOAnchorPos(new Vector2(-421, 0), 0.2f).SetEase(Ease.InQuad));
            sequence.Join(RightCharaBG.rectTransform.DOAnchorPos(new Vector2(421, 0), 0.2f).SetEase(Ease.InQuad));

            sequence.Append(LeftCharaBG.rectTransform.DOAnchorPos(new Vector2(-234, 0), 0.2f).SetEase(Ease.InQuad));
            sequence.Join(RightCharaBG.rectTransform.DOAnchorPos(new Vector2(234, 0), 0.2f).SetEase(Ease.InQuad));

            sequence.Join(LeftCharaBG.rectTransform.DOScale(new Vector3(0.3f, 8.75f, 8.75f), 0.2f))
                .SetEase(Ease.InQuad);
            sequence.Join(RightCharaBG.rectTransform.DOScale(new Vector3(0.3f, 8.75f, 8.75f), 0.2f))
                .SetEase(Ease.InQuad);

            sequence.Append(LeftCharaBG.rectTransform.DOAnchorPos(new Vector2(-421, 0), 0.2f).SetEase(Ease.InQuad));
            sequence.Join(RightCharaBG.rectTransform.DOAnchorPos(new Vector2(421, 0), 0.2f).SetEase(Ease.InQuad));

            sequence.Join(LeftCharaBG.rectTransform.DOScale(new Vector3(8.75f, 8.75f, 8.75f), 0.2f))
                .SetEase(Ease.InQuad);
            sequence.Join(RightCharaBG.rectTransform.DOScale(new Vector3(8.75f, 8.75f, 8.75f), 0.2f))
                .SetEase(Ease.InQuad);

            sequence.Join(Effect.DOScale(Vector3.one, 0.6f)).SetEase(Ease.InQuad);


            sequence.AppendInterval(2f).AppendCallback(() => { SceneManager.LoadScene("Gaming"); });
            sequence.Play();
        }
    }
}
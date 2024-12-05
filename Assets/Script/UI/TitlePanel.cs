using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    public Button startBtn;
    public Button creditBtn;
    public Image blackImage;
    public Image BGImg;

    public Sprite noLightSprite;
    public Sprite lightOnSprite;

    public float minHoldTime = 1f;
    public float maxHoldTime = 5f;

    public int minFlickerCount = 2;
    public int maxFlickerCount = 5;

    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    void Start()
    {
        startBtn.onClick.AddListener(() =>
        {
            Sequence sequence = DOTween.Sequence();

            sequence.Append(blackImage.DOFade(1, 1f));

            sequence.onComplete += () => { SceneManager.LoadScene("Waiting"); };

            sequence.Play();
        });
        creditBtn.onClick.AddListener(() => { });

        StartLightSequence();
    }

    private bool isFlickering = true;

    void StartLightSequence()
    {
        if (BGImg == null || noLightSprite == null || lightOnSprite == null)
        {
            Debug.LogError("????ï–?åπê•î€?íuÅI");
            return;
        }

        if (!isFlickering) return;

        float holdTime = Random.Range(minHoldTime, maxHoldTime);

        BGImg.sprite = lightOnSprite;

        DOVirtual.DelayedCall(holdTime, StartFlickeringSequence);
    }

    void StartFlickeringSequence()
    {
        int flickerCount = Random.Range(minFlickerCount, maxFlickerCount);

        Flicker(flickerCount);
    }

    void Flicker(int count)
    {
        if (count <= 0)
        {
            StartLightSequence();
            return;
        }

        float interval = Random.Range(minFlickerInterval, maxFlickerInterval);


        BGImg.sprite = BGImg.sprite == noLightSprite ? lightOnSprite : noLightSprite;

        DOVirtual.DelayedCall(interval, () => Flicker(count - 1));
    }

    public void StopEffect()
    {
        isFlickering = false;
    }

    public void StartEffect()
    {
        isFlickering = true;
        StartLightSequence();
    }

    private void OnDestroy()
    {
        startBtn.onClick.RemoveAllListeners();
        creditBtn.onClick.RemoveAllListeners();
    }
}
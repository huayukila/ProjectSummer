using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    public Button startBtn;
    public Button creditBtn;
    public Image blackImage;

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
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        startBtn.onClick.RemoveAllListeners();
        creditBtn.onClick.RemoveAllListeners();
    }
}
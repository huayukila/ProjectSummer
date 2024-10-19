using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    private float countTime = 0f;
    private float waiteTime = 5;
    private float loadingAnimCount;
    private int animIndex = 0;

    private Vector3 moveDir = new Vector3(-1, -1, 0);

    [Header("LoadingImage")] public Image loadingImg;
    public Sprite[] LoadingAnima;
    public float LoadingAnimSpeed;

    [Header("BGMask")] public RectTransform[] BGMasks;

    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < BGMasks.Length; i++)
        {
            sequence.Insert(i * 0.1f, BGMasks[i].DOMove(BGMasks[i].position + moveDir * distance, 1.5f).SetEase(Ease.OutQuad));
        }

        sequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        loadingAnimCount += Time.deltaTime;

        if (loadingAnimCount > LoadingAnimSpeed)
        {
            animIndex++;
            loadingImg.sprite = LoadingAnima[animIndex % LoadingAnima.Length];
            loadingAnimCount = 0;
        }

        countTime += Time.deltaTime;
        if (countTime >= waiteTime)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
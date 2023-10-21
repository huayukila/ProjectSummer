using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class CountDownCtrler : MonoBehaviour
{
    public Image showCountImg;
    public Sprite[] countDownSprites;
    public float offsetY;
    [MinValue(0.2f), MaxValue(0.8f)] public float audioPlayTime;

    private GameObject _countDownUI;
    private bool _isStart;
    private bool _isAudioPlayed;
    private Vector3 _rootPosition;
    private Vector3 _offsetPosition;
    private float _durationTime;
    private float _audioDurationTime;
    private int _index;
    private float _startTime;
    private Action _callBack;
    
    /// <summary>
    /// 名前の通り
    /// </summary>
    /// <param name="callBack">CD終わった後での処理関数、なんもないならnullにすればよい</param>
    public void StartCountDown(Action callBack)
    {
        Application.targetFrameRate = 60;
        _isStart = true;
        _startTime = Time.realtimeSinceStartup;
        _callBack = callBack;
    }

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isStart) return;
        _durationTime = Time.realtimeSinceStartup - _startTime;
        _audioDurationTime = Time.realtimeSinceStartup - _startTime;
        if (!_isAudioPlayed)
        {
            if (_audioDurationTime > audioPlayTime)
            {
                AudioManager.Instance.PlayFX(_index < countDownSprites.Length - 1 ? "CDFX" : "TimeOutFX", 0.8f);
                _isAudioPlayed = true;
            }
        }

        if (_durationTime < 1)
        {
            Fade();
        }
        else
        {
            _isAudioPlayed = false;
            _startTime = Time.realtimeSinceStartup;
            _index++;
            if (_index > countDownSprites.Length - 1)
            {
                Application.targetFrameRate = -1;
                _callBack?.Invoke();
                Destroy(gameObject);
                return;
            }

            ResetToPre();
            if (_index == countDownSprites.Length - 1)
            {
                showCountImg.rectTransform.localScale = new Vector3(3, 1, 1);
            }
        }
    }

    #region 内部用

    void Fade()
    {
        Vector3 currentPosition = showCountImg.transform.position;
        showCountImg.transform.position = new Vector3(currentPosition.x,
            Mathf.Lerp(currentPosition.y, _rootPosition.y, 0.1f), currentPosition.z);
        showCountImg.color = new Color(1, 1, 1, Mathf.Lerp(showCountImg.color.a, 1, 0.1f));
    }

    void ResetToPre()
    {
        showCountImg.color = new Color(1, 1, 1, 0);
        showCountImg.transform.position = _offsetPosition;
        showCountImg.sprite = countDownSprites[_index];
    }

    void Init()
    {
        _countDownUI = showCountImg.gameObject;
        var position = _countDownUI.transform.position;
        _rootPosition = position;
        _offsetPosition = position + ((Vector3.down * offsetY));
        _countDownUI.transform.position = _offsetPosition;
        showCountImg.color = new Color(1f, 1, 1, 0);
        showCountImg.sprite = countDownSprites[_index];
    }

    #endregion
}
using System.Collections.Generic;
using UnityEngine;
using System;

interface AudioPlayer
{
    void StopAllMusic();

    /// <summary>
    /// BGM�Đ�
    /// </summary>
    /// <param name="audioName">BGM�̖��O</param>
    /// <param name="volume">����</param>
    void PlayBGM(string audioName, float volume);

    void StopBGM();
    void PauseBGM();
    void PauseAllMusic();
    void ContinueBGM();
    void ContinueAllMusic();

    /// <summary>
    /// ���ʂ�FX�Đ�
    /// </summary>
    /// <param name="audioName">���O</param>
    /// <param name="volume">����</param>
    void PlayFX(string audioName, float volume);

    /// <summary>
    /// �I�u�W�F�N�g�ɂ��Ă�����FX�B�܂��������ĂȂ����߁A���͗��p�ł��܂���
    /// </summary>
    /// <param name="audioName">���O</param>
    /// <param name="volume">����</param>
    /// <param name="obj">�I�u�W�F�N�g</param>
    /// <param name="isLoop">�J��Ԃ��čĐ��̂�</param>
    void PlayFX(string audioName, float volume, GameObject obj, bool isLoop);

    /// <summary>
    /// �����ꏊ�ɂ��̂܂܎c�邽�߂ɁB�������p�ł��܂���
    /// </summary>
    /// <param name="audioName">���O</param>
    /// <param name="volume">����</param>
    /// <param name="targetTransform">�����ʒu</param>
    void PlayFX(string audioName, float volume, Transform targetTransform);
}


public class AudioManager : Singleton<AudioManager>, AudioPlayer
{
    AudioSource _bgmAudioSource;
    Dictionary<string, AudioClip> _fxmAudioDic;
    Dictionary<string, AudioClip> _bgmAudioDic;
    readonly List<AudioSource> _playingMusicList=new List<AudioSource>();
    SimpleObjectPool<GameObject> _fxAudioSourcePool;

    protected override void Awake()
    {
        base.Awake();
        _fxAudioSourcePool = new SimpleObjectPool<GameObject>(() =>
        {
            GameObject obj = new GameObject();
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            obj.AddComponent<FXAudioPlayer>().Init(() =>
            {
                _fxAudioSourcePool.Recycle(obj);
                _playingMusicList.Remove(audioSource);
            });
            obj.transform.SetParent(gameObject.transform);
            return obj;
        }, obj =>
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            audioSource.volume = 1.0f;
            audioSource.clip = null;
            audioSource.loop = false;
        }, 10);
        AudioDataBase audioDatabase = (AudioDataBase)Resources.Load("AudioDataBase");
        _bgmAudioDic = audioDatabase.GetBGMAudioDic();
        _fxmAudioDic = audioDatabase.GetFXAudioDic();
        _bgmAudioSource = gameObject.AddComponent<AudioSource>();
        _bgmAudioSource.loop = true;
    }

    public void StopAllMusic()
    {
        StopBGM();
        foreach (var music in _playingMusicList)
        {
            music.Pause();
        }
    }

    public void PlayBGM(string audioName, float volume)
    {
        _bgmAudioSource.clip = _bgmAudioDic[audioName];
        _bgmAudioSource.volume = volume;
        _bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        _bgmAudioSource?.Stop();
    }

    public void PlayFX(string audioName, float volume)
    {
        GameObject fxObj = _fxAudioSourcePool.Allocate();
        fxObj.GetComponent<FXAudioPlayer>().PlayMusic(_fxmAudioDic[audioName], volume);
        _playingMusicList.Add(fxObj.GetComponent<AudioSource>());
    }

    public void PlayFX(string audioName, float volume, GameObject obj, bool isLoop)
    {
    }

    public void PlayFX(string audioName, float volume, Transform targetTransform)
    {
    }

    public void PauseBGM()
    {
        _bgmAudioSource?.Pause();
    }

    public void PauseAllMusic()
    {
        PauseBGM();
        foreach (var music in _playingMusicList)
        {
            music.Pause();
        }
    }

    public void ContinueBGM()
    {
        _bgmAudioSource?.UnPause();
    }

    public void ContinueAllMusic()
    {
        ContinueBGM();
        foreach (var music in _playingMusicList)
        {
            music.UnPause();
        }
    }
}

public class FXAudioPlayer : MonoBehaviour
{
    AudioSource _audioSource;
    Action _callBack;
    bool _isPlaying = false;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_audioSource == null || !_isPlaying) return;
        if (_audioSource.isPlaying) return;
        _isPlaying = false;
        _callBack?.Invoke();
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.Play();
        _isPlaying = true;
    }

    public void Init(Action callback)
    {
        _callBack = callback;
    }
}
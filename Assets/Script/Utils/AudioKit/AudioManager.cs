using System.Collections.Generic;
using UnityEngine;
using System;

interface AudioPlayer
{
    void StopAllMusic();

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="audioName">BGMの名前</param>
    /// <param name="volume">音量</param>
    void PlayBGM(string audioName, float volume);

    void StopBGM();
    void PauseBGM();
    void PauseAllMusic();
    void ContinueBGM();
    void ContinueAllMusic();

    /// <summary>
    /// 普通でFX再生
    /// </summary>
    /// <param name="audioName">名前</param>
    /// <param name="volume">音量</param>
    void PlayFX(string audioName, float volume);

    /// <summary>
    /// オブジェクトについていくのFX。まだ実装してないため、今は利用できません
    /// </summary>
    /// <param name="audioName">名前</param>
    /// <param name="volume">音量</param>
    /// <param name="obj">オブジェクト</param>
    /// <param name="isLoop">繰り返して再生のか</param>
    void PlayFX(string audioName, float volume, GameObject obj, bool isLoop);

    /// <summary>
    /// 発生場所にそのまま残るために。同じ利用できません
    /// </summary>
    /// <param name="audioName">名前</param>
    /// <param name="volume">音量</param>
    /// <param name="targetTransform">発生位置</param>
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
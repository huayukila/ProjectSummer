using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

interface AudioPlayer
{
    void StopAllMusic();
    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="name">BGMの名前</param>
    /// <param name="Volum">音量</param>
    void PlayBGM(string name, float Volum);
    void StopBGM();

    void PauseBGM();
    void PauseAllMusic();

    void ContinueBGM();

    void CountinueAllMusic();
    /// <summary>
    /// 普通でFX再生
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="Volum">音量</param>
    void PlayFX(string name, float Volum);
    /// <summary>
    /// オブジェクトについていくのFX。まだ実装してないため、今は利用できません
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="Volum">音量</param>
    /// <param name="Obj">オブジェクト</param>
    /// <param name="isLoop">繰り返して再生のか</param>
    void PlayFX(string name, float Volum, GameObject Obj, bool isLoop);
    /// <summary>
    /// 発生場所にそのまま残るために。同じ利用できません
    /// </summary>
    /// <param name="name">名前</param>
    /// <param name="Volum">音量</param>
    /// <param name="transform">発生位置</param>
    void PlayFX(string name, float Volum, Transform transform);
}


public class AudioManager : Singleton<AudioManager>, AudioPlayer
{
    AudioSource BGMAudioSource;

    Dictionary<string, AudioClip> FXM_audioDic;
    Dictionary<string, AudioClip> BGM_AudioDic;

    List<AudioSource> playingMusicList = new List<AudioSource>();

    SimpleObjectPool<GameObject> FX_audioSourcePool;


    protected override void Awake()
    {
        base.Awake();
        GameObject AudioPool=new GameObject("AudioPool");
        DontDestroyOnLoad(AudioPool);
        FX_audioSourcePool = new SimpleObjectPool<GameObject>(() =>
        {
            GameObject obj = new GameObject();
            obj.AddComponent<AudioSource>();
            obj.AddComponent<FX_audioPlayer>();
            obj.transform.SetParent(AudioPool.transform);
            return obj;
        }, (obj) =>
        {
            AudioSource audioSource = obj.GetComponent<AudioSource>();
            audioSource.volume = 1.0f;
            audioSource.clip = null;
            audioSource.loop = false;
        }, 10);
        AudioDataBase audioDatabase = (AudioDataBase)Resources.Load("AudioDataBase");
        BGM_AudioDic = audioDatabase.GetBGMAudioDic();
        FXM_audioDic = audioDatabase.GetFXAudioDic();
        BGMAudioSource = gameObject.AddComponent<AudioSource>();
        BGMAudioSource.loop = true;
    }

    public void StopAllMusic()
    {
        StopBGM();
        foreach(var music in playingMusicList)
        {
            music.Pause();
        }
    }

    public void PlayBGM(string name, float Volum)
    {
        BGMAudioSource.clip = BGM_AudioDic[name];
        BGMAudioSource.volume = Volum;
        BGMAudioSource.Play();
    }

    public void StopBGM()
    {
        BGMAudioSource?.Stop();
    }

    public void PlayFX(string name, float Volum)
    {
        GameObject fxObj = FX_audioSourcePool.Allocate();
        AudioSource audioSource = fxObj.GetComponent<AudioSource>();
        playingMusicList.Add(audioSource);
        fxObj.GetComponent<FX_audioPlayer>().Init(FXM_audioDic[name], Volum, () =>
        {
            FX_audioSourcePool.Recycle(fxObj);
            playingMusicList.Remove(audioSource);
        });
    }

    public void PlayFX(string name, float Volum, GameObject Obj, bool isLoop)
    {
    }

    public void PlayFX(string name, float Volum, Transform transform)
    {
    }

    public void PauseBGM()
    {
        BGMAudioSource?.Pause();
    }

    public void PauseAllMusic()
    {
        PauseBGM();
        foreach(var music in playingMusicList)
        {
            music.Pause();
        }
    }

    public void ContinueBGM()
    {
        BGMAudioSource?.UnPause();
    }

    public void CountinueAllMusic()
    {
        ContinueBGM();
        foreach (var music in playingMusicList)
        {
            music.UnPause();
        }
    }
}

public class FX_audioPlayer : MonoBehaviour
{
    AudioSource audioSource;
    Action callBack;
    private void Update()
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                callBack?.Invoke();
            }
        }
    }
    public void Init(AudioClip clip,float volum,Action callback)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volum;
        callBack = callback;

        audioSource.Play();
    }
}

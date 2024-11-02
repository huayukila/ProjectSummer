using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AudioKit/AudioDataBase", fileName = "NewAudioDataBase")]
[Serializable]
public class AudioDataBase : ScriptableObject
{
    public List<AudioData> BGMAudioDatasList;
    public List<AudioData> FXAudioDatasList;

    public Dictionary<string, AudioClip> GetFXAudioDic()
    {
        if (FXAudioDatasList != null)
        {
            Dictionary<string, AudioClip> m_AudioDic = new Dictionary<string, AudioClip>();
            foreach (var data in FXAudioDatasList)
            {
                if (!m_AudioDic.ContainsKey(data.audioName))
                {
                    m_AudioDic.Add(data.audioName, data.audioClip);
                }
            }
            return m_AudioDic;
        }
        return null;
    }
    public Dictionary<string, AudioClip> GetBGMAudioDic()
    {
        if(BGMAudioDatasList != null)
        {
            Dictionary<string, AudioClip> m_AudioDic = new Dictionary<string, AudioClip>();
            foreach (var data in BGMAudioDatasList)
            {
                if (!m_AudioDic.ContainsKey(data.audioName))
                {
                    m_AudioDic.Add(data.audioName, data.audioClip);
                }
            }
            return m_AudioDic;
        }
        return null;
    }
}

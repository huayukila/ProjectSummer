using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public interface IParticleSystem
{
    void Play();
    void Stop();
    void SetParticleObject(string name);
}
public abstract class ParticleSystemBase : MonoBehaviour,IParticleSystem
{
    protected Dictionary<string, GameObject> mParticleObjects;
    protected GameObject mParticleObject;                         // �p�[�e�B�N���V�X�e���������Ă���I�u�W�F�N�g
    protected ParticleSystem mParticleSystem;                     // �p�[�e�B�N���V�X�e��
    protected ParticleSystem.MainModule mMainModule;              // �p�[�e�B�N���V�X�e���̃v���p�e�B��ݒ肷�邽�߂̕ϐ�

    protected virtual void Awake() {}
    public void Play()
    {
        if (mParticleSystem.isStopped)
        {
            mParticleSystem.Play();
        }
    }

    public void Stop()
    {
        if (mParticleSystem.isPlaying)
        {
            mParticleSystem.Stop();
        }
    }

    public void SetParticleObject(string name)
    {
        if(mParticleObjects.ContainsKey(name))
        {
            Stop();
            mParticleObject = Instantiate(mParticleObjects[name],transform);
            mParticleSystem = mParticleObject.GetComponent<ParticleSystem>();
            mMainModule = mParticleSystem.main;
        }
    }
}


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
    protected GameObject mParticleObject;                         // パーティクルシステムが入っているオブジェクト
    protected ParticleSystem mParticleSystem;                     // パーティクルシステム
    protected ParticleSystem.MainModule mMainModule;              // パーティクルシステムのプロパティを設定するための変数

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


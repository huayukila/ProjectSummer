using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public interface IParticleSystem
{
    void Play();
    void Stop();
    void SetParticleMaterial(string name);
}
public class CharacterParticleSystem : MonoBehaviour,IParticleSystem
{ 
    private Dictionary<string, Material> mParticleObjects;
    private GameObject mParticleObject;                         // パーティクルシステムが入っているオブジェクト
    private ParticleSystem mParticleSystem;                     // パーティクルシステム
    private ParticleSystem.MainModule mMainModule;              // パーティクルシステムのプロパティを設定するための変数
    private Rigidbody mRigidbody;

    private void Awake()
    {
        mParticleObjects = new Dictionary<string, Material>();

        mParticleObject = Instantiate(GameResourceSystem.Instance.GetPrefabResource("DustParticlePrefab"), transform);
        mParticleObject.transform.localPosition = Vector3.zero;
        mParticleObject.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        mParticleSystem = mParticleObject.GetComponent<ParticleSystem>();
        mMainModule = mParticleSystem.main;
        mMainModule.startSize = 0.4f;
        mMainModule.startColor = Color.gray;
        mRigidbody = GetComponent<Rigidbody>();

        //mParticleObjects.Add("Dust", mParticleObject.GetComponent);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mParticleSystem.isPlaying)
        {
            mMainModule.startSpeed = mRigidbody.velocity.magnitude / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            mMainModule.simulationSpeed = mRigidbody.velocity.magnitude / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            mMainModule.startLifetime = mMainModule.simulationSpeed * 0.5f;
        }
    }

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

    public void SetParticleMaterial(string name)
    {
        if(mParticleObjects.TryGetValue(name,out Material vaule) == false)
        {
            
        }
    }
}

using UnityEngine;

public sealed class PlayerParticleSystemControl : ParticleSystemBase
{
    private Rigidbody mRigidbody;

    protected override void Awake()
    {
        ParticleObjectDataBase mParticleDatabase = Resources.Load("PlayerParticleDataBase") as ParticleObjectDataBase;
        mParticleObjects = mParticleDatabase.GetParticleObjectList();
        foreach (var particleObject in mParticleObjects)
        {
            particleObject.Value.transform.localPosition = Vector3.zero;
            particleObject.Value.transform.rotation = Quaternion.LookRotation(-transform.forward, Vector3.up);
        }
        if(mParticleObjects.ContainsKey("Dust"))
        {
            mParticleObject = Instantiate(mParticleObjects["Dust"], transform);
            mParticleSystem = mParticleObject.GetComponent<ParticleSystem>();
            mMainModule = mParticleSystem.main;
        }
        else
        {
            Debug.LogError(name + " doesn't have default particle system!");
        }
        mRigidbody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        if (mParticleSystem != null && mParticleSystem.isPlaying)
        {
            mMainModule.startSpeed = mRigidbody.velocity.magnitude / Global.PLAYER_MAX_MOVE_SPEED * 2.0f;
            mMainModule.simulationSpeed = mRigidbody.velocity.magnitude / Global.PLAYER_MAX_MOVE_SPEED * 4.0f + 1.0f;
            mMainModule.startLifetime = mMainModule.simulationSpeed * 0.5f;
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    void LockOnTarget(GameObject target);
    void StopLockOn();
}
namespace Gaming
{
    public class CameraControl : MonoBehaviour, ICameraController
    {
        private enum CamState
        {
            None = 0,
            OnTarget
        }
        [SerializeField]
        private float _smoothness;
        private GameObject _target;
        private CamState mState = CamState.None;
        private Timer _playerRespawnLockOnTimer;

        // Start is called before the first frame update
        void Start()
        {
            _smoothness = 2.5f;
        }

        private void FixedUpdate()
        {
            switch(mState)
            {
                case CamState.None:
                    break;
                case CamState.OnTarget:
                {
                    Vector3 camPos = _target.transform.position + Vector3.up * 36;
                    transform.position = Vector3.Lerp(transform.position, camPos, _smoothness * Time.fixedDeltaTime);
                    break;
                }

            }
        }

        public void LockOnTarget(GameObject target)
        {
            if (target != null)
            {
                _target = target;
                mState = CamState.OnTarget;
                transform.position = _target.transform.position + Vector3.up * 36;
            }
        }
        public void StopLockOn()
        {
            mState = CamState.None;
            _playerRespawnLockOnTimer = new Timer(Time.time,Global.RESPAWN_TIME / 2.0f,
                () =>
                {
                    mState = CamState.OnTarget;
                }
                );

            _playerRespawnLockOnTimer.StartTimer(this);
        }


    }
}


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
        private float m_Smoothness;
        private GameObject m_Target;
        private CamState m_State = CamState.None;

        // Start is called before the first frame update
        void Start()
        {
            m_Smoothness = 2.5f;
        }

        // Update is called once per frame

        private void FixedUpdate()
        {
            switch(m_State)
            {
                case CamState.None:
                    break;
                case CamState.OnTarget:
                {
                    Vector3 camPos = m_Target.transform.position + Vector3.up * 36;
                    transform.position = Vector3.Lerp(transform.position, camPos, m_Smoothness * Time.deltaTime);
                    break;
                }

            }
        }

        public void LockOnTarget(GameObject target)
        {
            if (target != null)
            {
                m_Target = target;
                m_State = CamState.OnTarget;
                transform.position = m_Target.transform.position + Vector3.up * 36;
            }
        }
        public void StopLockOn()
        {
            m_State = CamState.None;
            NewTimer lockOnNewTimer = new NewTimer  (                Time.time,
                                                        Global.RESPAWN_TIME/2f,
                                                        () =>
                                                        {
                                                            m_State = CamState.OnTarget;
                                                        }                               
                                                                            );
            lockOnNewTimer.StartTimer(this);
        }


    }
}


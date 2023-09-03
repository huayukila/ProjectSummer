using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimerCon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
public class Timer
{
    private float duration;
    private float startTime;
    private Action callback;

    public Timer()
    {
        duration = 0f;
        startTime = 0f;
        callback = null;
    }

    public void SetTimer(float duration,Action callback) //�^�C�}�[�̎��ԂƃR�[���o�b�N���Z�b�g����
    {
        this.duration = duration;
        this.callback = callback;
        startTime = Time.time;
    }
    public bool IsTimerFinished()�@�@�@�@�@�@�@�@�@�@�@�@//�^�C�}�[���I��邩�ǂ����̔��f
    {
        if(callback == null)
        {
            return false;
        }
        if (Time.time - startTime >= duration) 
        {
            callback.Invoke();
            return true;
        }
        return false;
    }
    public float GetTime()�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�^�C�}�[�̃��A���^�C���̃Q�b�g
    {
        float remainingTime = duration - (Time.time - startTime);
        return remainingTime;
    }
}



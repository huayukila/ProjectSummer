using System;
using UnityEngine;

namespace Kit
{
    internal class Delay : IAction
    {
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }
        public bool Deinited { get; set; }
        public bool Paused { get; set; }



        public float DelayTime;
        public Func<float> DelayTimeFactory = null;
        public System.Action OnDelayFinish { get; set; }
        public float CurrentSeconds { get; set; }

        private Delay() { }
        private static readonly SimpleObjectPool<Delay> mPool =
            new SimpleObjectPool<Delay>(() => new Delay(), null, 10);

        public static Delay Allocate(float delayTime, System.Action onDelayFinish = null)
        {
            var returnNode = mPool.Allocate();
            returnNode.ActionID = ActionKit.ID_GENERATOR++;
            returnNode.Deinited = false;
            returnNode.Reset();
            returnNode.DelayTime = delayTime;
            returnNode.OnDelayFinish = onDelayFinish;
            returnNode.CurrentSeconds = 0.0f;
            return returnNode;
        }
        public static Delay Allocate(Func<float> delayTimeFactory, System.Action onDelayFinish = null)
        {
            var returnNode = mPool.Allocate();
            returnNode.Deinited = false;
            returnNode.Reset();
            returnNode.DelayTimeFactory = delayTimeFactory;
            returnNode.OnDelayFinish = onDelayFinish;
            returnNode.CurrentSeconds = 0.0f;
            return returnNode;
        }

        public void OnStart()
        {
            if (DelayTimeFactory != null)
            {
                DelayTime = DelayTimeFactory();
            }
        }

        public void OnExecute(float deltaTime)
        {
            if (CurrentSeconds >= DelayTime)
            {
                this.Finish();
                OnDelayFinish?.Invoke();
            }
            CurrentSeconds += deltaTime;
        }

        public void OnFinish()
        {
        }

        public void Deinit()
        {
            OnDelayFinish = null;
            Deinited = true;
            mPool.Recycle(this);
        }

        public void Reset()
        {
            Status = ActionStatus.NotStart;
            Paused = false;
            CurrentSeconds = 0.0f;
        }
    }

    public static class DelayExtension
    {
        public static ISequence Delay(this ISequence self,float seconds,Action onDelayFinish = null)
        {
            return self.Append(Kit.Delay.Allocate(seconds, onDelayFinish));
        }

        public static ISequence Delay(this ISequence self, Func<float> delayTimeFactory, Action onDelayFinish = null)
        {
            return self.Append(Kit.Delay.Allocate(delayTimeFactory, onDelayFinish));
        }
    }
}

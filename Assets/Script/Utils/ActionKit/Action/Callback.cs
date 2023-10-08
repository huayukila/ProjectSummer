using System;
namespace Kit
{
    internal class Callback : IAction
    {
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }
        public bool Deinited { get; set; }
        public bool Paused { get; set; }

        private Action mCallback;
        private static SimpleObjectPool<Callback> mSimpleObjectPool =
            new SimpleObjectPool<Callback>(() => new Callback(), null, 10);

        private Callback() { }

        public static Callback Allocate(Action callback)
        {
            var callbackAction = mSimpleObjectPool.Allocate();
            callbackAction.ActionID = ActionKit.ID_GENERATOR++;
            callbackAction.Reset();
            callbackAction.Deinited = false;
            callbackAction.mCallback = callback;
            return callbackAction;
        }


        public void OnStart()
        {
            mCallback?.Invoke();
            this.Finish();
        }
        public void OnExecute(float deltaTime)
        {
        }

        public void OnFinish()
        {
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mCallback = null;
                mSimpleObjectPool.Recycle(this);
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }
    }

    public static class CallbackExtension
    {
        public static ISequence Callback(this ISequence self, Action callback)
        {
            return self.Append(Kit.Callback.Allocate(callback));
        }
    }
}
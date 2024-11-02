using System;
using System.Collections.Generic;

namespace Kit {

    public interface ISequence:IAction
    {
        ISequence Append(IAction action);
    }

    internal class Sequence : ISequence
    {
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }
        public bool Deinited { get; set; }
        public bool Paused { get; set; }

        private IAction mCurrentAction=null;
        private int mCurrentActionIndex = 0;
        private List<IAction> mActions = ListPool<IAction>.Get();

        private static SimpleObjectPool<Sequence> mSimpleObjectPool =
            new SimpleObjectPool<Sequence>(() => new Sequence(), null, 10);

        private Sequence() { }

        public ISequence Append(IAction action)
        {
            mActions.Add(action);
            return this;
        }

        public static Sequence Allocate()
        {
            var sequence = mSimpleObjectPool.Allocate();
            sequence.ActionID = ActionKit.ID_GENERATOR++;
            sequence.Reset();
            sequence.Deinited = false;
            return sequence;
        }


        public void OnStart()
        {
            if(mActions.Count > 0)
            {
                mCurrentActionIndex = 0;
                mCurrentAction = mActions[mCurrentActionIndex];
                mCurrentAction.Reset();
                TryExecuteUntilNextNotFinished();
            }
            else
            {
                this.Finish();
            }
        }

        public void OnExecute(float deltaTime)
        {
            if(mCurrentAction!=null)
            {
                if (mCurrentAction.Execute(deltaTime))
                {
                    mCurrentActionIndex++;
                    if (mCurrentActionIndex < mActions.Count)
                    {
                        mCurrentAction = mActions[mCurrentActionIndex];
                        mCurrentAction.Reset();

                        TryExecuteUntilNextNotFinished();
                    }
                    else
                    {
                        this.Finish();
                    }
                }
            }
            else
            {
                this.Finish();
            }
        }

        public void OnFinish()
        {
        }

        public void Reset()
        {
            mCurrentActionIndex = 0;
            Status = ActionStatus.NotStart;
            Paused = false;
            foreach (var action in mActions)
            {
                action.Reset();
            }
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;

                foreach (var action in mActions)
                {
                    action.Deinit();
                }
                mActions.Clear();
                mSimpleObjectPool.Recycle(this);
            }
        }


        void TryExecuteUntilNextNotFinished()
        {
            while (mCurrentAction != null && mCurrentAction.Execute(0))
            {
                mCurrentActionIndex++;

                if (mCurrentActionIndex < mActions.Count)
                {
                    mCurrentAction = mActions[mCurrentActionIndex];
                    mCurrentAction.Reset();
                }
                else
                {
                    mCurrentAction = null;
                    this.Finish();
                }
            }
        }
    }

    public static class SequenceExtension
    {
        public static ISequence Sequence(this ISequence self,Action<ISequence> sequenceSetting)
        {
            var repeat = Kit.Sequence.Allocate();
            sequenceSetting(repeat);
            return self.Append(repeat);
        }
    }
}
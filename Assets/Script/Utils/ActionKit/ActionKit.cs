using System;
using UnityEngine;
namespace Kit {
    public static class ActionKit
    {
        //アクションのマークID
        public static ulong ID_GENERATOR = 0;
        public static IAction Delay(float seconds, Action callback)
        {
            return Kit.Delay.Allocate(seconds, callback);
        }
        public static IAction CallBack(Action callback)
        {
            return Kit.Callback.Allocate(callback);
        }
        public static ISequence Sequence()
        {
            return Kit.Sequence.Allocate();
        }
    }
}
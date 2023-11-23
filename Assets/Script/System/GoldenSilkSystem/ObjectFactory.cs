using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaming
{
    public interface IGameObjectFactory
    {
        GameObject CreateObject();
    }
    //TODO ジェネリッククラスに変える
    public class GameObjectFactory : IGameObjectFactory
    {
        private Func<GameObject> mFactroyFunc;      // オブジェクトの作成方法

        public GameObjectFactory(Func<GameObject> FactroyFunc)
        {
            mFactroyFunc = FactroyFunc;
        }

        public GameObject CreateObject() 
        {
            return mFactroyFunc();
        }
    }

}

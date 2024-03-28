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
    //TODO �W�F�l���b�N�N���X�ɕς���
    public class GameObjectFactory : IGameObjectFactory
    {
        private Func<GameObject> mFactroyFunc;      // �I�u�W�F�N�g�̍쐬���@

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

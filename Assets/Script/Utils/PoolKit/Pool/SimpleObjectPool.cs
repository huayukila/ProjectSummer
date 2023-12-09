
using System;

public class SimpleObjectPool<T> : Pool<T>
{
    readonly Action<T> mResetMethod;

    /// <summary>
    /// poolストラック関数
    /// </summary>
    /// <param name="factoryMethod">object 取り組み関数</param>
    /// <param name="resetMethod">回収時自動行うreset関数</param>
    /// <param name="initCount">pool初期化時、含めてのobject個数</param>
    public SimpleObjectPool(Func<T> factoryMethod,Action<T> resetMethod=null,int initCount = 0)
    {
        mFactory=new CustomObjectFactory<T>(factoryMethod);
        mResetMethod=resetMethod;

        for(int i = 0; i < initCount; i++)
        {
            mCacheStack.Push(mFactory.Create());
        }
    }

    /// <summary>
    /// オブジェクトの回収関数(poolから取り出しのobjectだけ、使ってください)
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Recycle(T obj)
    {
        mResetMethod?.Invoke(obj);
        mCacheStack.Push(obj);

        return true;
    }
}

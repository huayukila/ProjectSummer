using System;
using System.Collections.Generic;

public abstract class Pool<T> : IPool<T>
{
    protected readonly Stack<T> mCacheStack=new Stack<T>();
    protected IObjectFactory<T> mFactory;
    protected int mMaxCount = 10;
    public int CurrentCount
    {
        get
        {
            return mCacheStack.Count;
        }
    }

    public virtual T Allocate()
    {
        return mCacheStack.Count == 0 ? mFactory.Create():mCacheStack.Pop();
    }

    public abstract bool Recycle(T obj);

    public void SetObjectFactory(IObjectFactory<T> factory)
    {
        mFactory = factory;
    }

    public void SetFactoryMethod(Func<T> factoryMethod)
    {
        mFactory = new CustomObjectFactory<T>(factoryMethod);
    }

    public void Clear(Action<T> onClearItem=null)
    {
        if (onClearItem != null)
        {
            foreach(var poolObj in mCacheStack)
            {
                onClearItem(poolObj);
            }
        }

        mCacheStack.Clear();
    }
}

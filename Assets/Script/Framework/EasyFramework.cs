using System;
using System.Collections.Generic;

public interface IArchitecture
{
    void RegisterSystem<T>(T system) where T : ISystem; // システムを登録する
    T GetSystem<T>() where T : class, ISystem; // システムを取得する
}

// システムのインターフェース
public interface ISystem
{
    void Init(); // 初期化する
    void SetArchitecture(IArchitecture architecture); // アーキテクチャを設定する
}

// 抽象システムクラス
public abstract class AbstractSystem : ISystem
{
    private IArchitecture _architecture; // アーキテクチャ

    public void Init() // 初期化メソッド
    {
        OnInit();
    }

    public void SetArchitecture(IArchitecture architecture) // アーキテクチャを設定するメソッド
    {
        _architecture = architecture;
    }

    protected abstract void OnInit(); // 初期化処理の抽象メソッド
}

public class EasyFramework :  IArchitecture
{
    private readonly HashSet<ISystem> mSystems = new HashSet<ISystem>(); // システムのハッシュセット
    private readonly IOCContainer mContainer = new IOCContainer(); // IOCコンテナ

    public void FrameworkInit() // フレームワークの初期化
    {
        foreach (var mSystem in mSystems)
        {
            mSystem.Init();
        }

        mSystems.Clear();
    }

    public void RegisterSystem<T>(T system) where T : ISystem // システムを登録する
    {
        system.SetArchitecture(this);
        mContainer.Register(system);
        mSystems.Add(system);
    }

    public T GetSystem<T>() where T : class, ISystem // システムを取得する
    {
        return mContainer.Get<T>();
    }

    
    private class IOCContainer // IOCコンテナクラス
    {
        private readonly Dictionary<Type, object> mInstances = new Dictionary<Type, object>(); // インスタンスの辞書

        public void Register<T>(T instance) // インスタンス登録
        {
            var key = typeof(T);
            if (mInstances.ContainsKey(key))
                mInstances[key] = instance;
            else
                mInstances.Add(key, instance);
        }

        public T Get<T>() where T : class // インスタンス取得
        {
            var key = typeof(T);
            if (mInstances.TryGetValue(key, out var reinstance)) return reinstance as T;

            return null;
        }
    }
    
}

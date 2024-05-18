using System;
using System.Collections.Generic;

public interface IArchitecture
{
    void RegisterSystem<T>(T system) where T : ISystem; // �V�X�e����o�^����
    T GetSystem<T>() where T : class, ISystem; // �V�X�e�����擾����
}

// �V�X�e���̃C���^�[�t�F�[�X
public interface ISystem
{
    void Init(); // ����������
    void SetArchitecture(IArchitecture architecture); // �A�[�L�e�N�`����ݒ肷��
}

// ���ۃV�X�e���N���X
public abstract class AbstractSystem : ISystem
{
    private IArchitecture _architecture; // �A�[�L�e�N�`��

    public void Init() // ���������\�b�h
    {
        OnInit();
    }

    public void SetArchitecture(IArchitecture architecture) // �A�[�L�e�N�`����ݒ肷�郁�\�b�h
    {
        _architecture = architecture;
    }

    protected abstract void OnInit(); // �����������̒��ۃ��\�b�h
}

public class EasyFramework :  IArchitecture
{
    private readonly HashSet<ISystem> mSystems = new HashSet<ISystem>(); // �V�X�e���̃n�b�V���Z�b�g
    private readonly IOCContainer mContainer = new IOCContainer(); // IOC�R���e�i

    public void FrameworkInit() // �t���[�����[�N�̏�����
    {
        foreach (var mSystem in mSystems)
        {
            mSystem.Init();
        }

        mSystems.Clear();
    }

    public void RegisterSystem<T>(T system) where T : ISystem // �V�X�e����o�^����
    {
        system.SetArchitecture(this);
        mContainer.Register(system);
        mSystems.Add(system);
    }

    public T GetSystem<T>() where T : class, ISystem // �V�X�e�����擾����
    {
        return mContainer.Get<T>();
    }

    
    private class IOCContainer // IOC�R���e�i�N���X
    {
        private readonly Dictionary<Type, object> mInstances = new Dictionary<Type, object>(); // �C���X�^���X�̎���

        public void Register<T>(T instance) // �C���X�^���X�o�^
        {
            var key = typeof(T);
            if (mInstances.ContainsKey(key))
                mInstances[key] = instance;
            else
                mInstances.Add(key, instance);
        }

        public T Get<T>() where T : class // �C���X�^���X�擾
        {
            var key = typeof(T);
            if (mInstances.TryGetValue(key, out var reinstance)) return reinstance as T;

            return null;
        }
    }
    
}

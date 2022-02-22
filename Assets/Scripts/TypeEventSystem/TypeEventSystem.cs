using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictionaryPool<TKey, TValue>
{
    static Stack<Dictionary<TKey, TValue>> mListStack = new Stack<Dictionary<TKey, TValue>>(8);

    public static Dictionary<TKey, TValue> Get()
    {
        if (mListStack.Count == 0)
        {
            return new Dictionary<TKey, TValue>(8);
        }

        return mListStack.Pop();
    }

    public static void Release(Dictionary<TKey, TValue> toRelease)
    {
        toRelease.Clear();
        mListStack.Push(toRelease);
    }
}
public static class ListPool<T>
{
    static Stack<List<T>> mListStack = new Stack<List<T>>(8);

    public static List<T> Get()
    {
        if (mListStack.Count == 0)
        {
            return new List<T>(8);
        }

        return mListStack.Pop();
    }

    public static void Release(List<T> toRelease)
    {
        toRelease.Clear();
        mListStack.Push(toRelease);
    }
}

public static class ListPoolExtensions
{
    public static void Release2Pool<T>(this List<T> toRelease)
    {
        ListPool<T>.Release(toRelease);
    }
}
public static class DictionaryPoolExtensions
{
    public static void Release2Pool<TKey, TValue>(this Dictionary<TKey, TValue> toRelease)
    {
        DictionaryPool<TKey, TValue>.Release(toRelease);
    }
}
public interface ITypeEventSystem : IDisposable
{
    IDisposable RegisterEvent<T>(Action<T> onReceive);
    void UnRegisterEvent<T>(Action<T> onReceive);

    void SendEvent<T>() where T : new();

    void SendEvent<T>(T e);
    void Clear();
}


public class TypeEventUnregister<T> : IDisposable
{
    public Action<T> OnReceive;

    public void Dispose()
    {
        TypeEventSystem.UnRegister(OnReceive);
    }
}

public class TypeEventSystem : ITypeEventSystem
{
    /// <summary>
    /// �ӿ� ֻ����洢���ֵ���
    /// </summary>
    interface IRegisterations : IDisposable
    {

    }


    /// <summary>
    /// ���ע��
    /// </summary>
    class Registerations<T> : IRegisterations
    {
        /// <summary>
        /// ��Ϊί�б���Ϳ���һ�Զ�ע��
        /// </summary>
        public Action<T> OnReceives = obj => { };

        public void Dispose()
        {
            OnReceives = null;
        }
    }

    /// <summary>
    /// ȫ��ע���¼�
    /// </summary>
    private static readonly ITypeEventSystem mGlobalEventSystem = new TypeEventSystem();

    /// <summary>
    /// 
    /// </summary>
    private Dictionary<Type, IRegisterations> mTypeEventDict = DictionaryPool<Type, IRegisterations>.Get();

    /// <summary>
    /// ע���¼�
    /// </summary>
    /// <param name="onReceive"></param>
    /// <typeparam name="T"></typeparam>
    public static IDisposable Register<T>(System.Action<T> onReceive)
    {
        return mGlobalEventSystem.RegisterEvent<T>(onReceive);
    }

    /// <summary>
    /// ע���¼�
    /// </summary>
    /// <param name="onReceive"></param>
    /// <typeparam name="T"></typeparam>
    public static void UnRegister<T>(System.Action<T> onReceive)
    {
        mGlobalEventSystem.UnRegisterEvent<T>(onReceive);
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="t"></param>
    /// <typeparam name="T"></typeparam>
    public static void Send<T>(T t)
    {
        mGlobalEventSystem.SendEvent<T>(t);
    }

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void Send<T>() where T : new()
    {
        mGlobalEventSystem.SendEvent<T>();
    }


    public IDisposable RegisterEvent<T>(Action<T> onReceive)
    {
        var type = typeof(T);

        IRegisterations registerations = null;

        if (mTypeEventDict.TryGetValue(type, out registerations))
        {
            var reg = registerations as Registerations<T>;
            reg.OnReceives += onReceive;
        }
        else
        {
            var reg = new Registerations<T>();
            reg.OnReceives += onReceive;
            mTypeEventDict.Add(type, reg);
        }

        return new TypeEventUnregister<T> { OnReceive = onReceive };
    }

    public void UnRegisterEvent<T>(Action<T> onReceive)
    {
        var type = typeof(T);

        IRegisterations registerations = null;

        if (mTypeEventDict.TryGetValue(type, out registerations))
        {
            var reg = registerations as Registerations<T>;
            reg.OnReceives -= onReceive;
        }
    }

    public void SendEvent<T>() where T : new()
    {
        var type = typeof(T);

        IRegisterations registerations = null;

        if (mTypeEventDict.TryGetValue(type, out registerations))
        {
            var reg = registerations as Registerations<T>;
            reg.OnReceives(new T());
        }
    }

    public void SendEvent<T>(T e)
    {
        var type = typeof(T);

        IRegisterations registerations = null;

        if (mTypeEventDict.TryGetValue(type, out registerations))
        {
            var reg = registerations as Registerations<T>;
            reg.OnReceives(e);
        }
    }

    public void Clear()
    {
        foreach (var keyValuePair in mTypeEventDict)
        {
            keyValuePair.Value.Dispose();
        }

        mTypeEventDict.Clear();
    }

    public void Dispose()
    {

    }
}

public interface IDisposableList : IDisposable
{
    void Add(IDisposable disposable);
}

public class DisposableList : IDisposableList
{
    List<IDisposable> mDisposableList = ListPool<IDisposable>.Get();

    public void Add(IDisposable disposable)
    {
        mDisposableList.Add(disposable);
    }

    public void Dispose()
    {
        foreach (var disposable in mDisposableList)
        {
            disposable.Dispose();
        }

        mDisposableList.Release2Pool();
        mDisposableList = null;
    }
}

public static class DisposableExtensions
{
    public static void AddTo(this IDisposable self, IDisposableList component)
    {
        component.Add(self);
    }
}

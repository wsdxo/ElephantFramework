using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// 不继承MonoBehavior的单例模式基类
/// 继承这个类需要写个私有无参构造函数
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T:class//,new()
{
    private static T instance;

    //用于加锁的对象
    protected static readonly object lockObj=new object();
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        //instance= new T();
                        //利用反射得到无参私有的构造函数进行实例化，避免外部创建破坏单例模式唯一性
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                            null,
                                            Type.EmptyTypes,
                                            null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("没有得到对应的无参构造函数");
                    }
                }
            }
            return instance;
        }
    }
}

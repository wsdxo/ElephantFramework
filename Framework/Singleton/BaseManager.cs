using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

/// <summary>
/// ���̳�MonoBehavior�ĵ���ģʽ����
/// �̳��������Ҫд��˽���޲ι��캯��
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseManager<T> where T:class//,new()
{
    private static T instance;

    //���ڼ����Ķ���
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
                        //���÷���õ��޲�˽�еĹ��캯������ʵ�����������ⲿ�����ƻ�����ģʽΨһ��
                        Type type = typeof(T);
                        ConstructorInfo info = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                            null,
                                            Type.EmptyTypes,
                                            null);
                        if (info != null)
                            instance = info.Invoke(null) as T;
                        else
                            Debug.LogError("û�еõ���Ӧ���޲ι��캯��");
                    }
                }
            }
            return instance;
        }
    }
}

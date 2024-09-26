using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̳�Mono�����Զ����صĵ���ģʽ����
/// �Ƽ�ʹ��
/// ��Ҫ�ֶ����ػ���ͨ��������ӣ�����
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject();//����������
                obj.name =typeof(T).ToString();//�õ�T�ű�����,�����ڱ༭������ȷ����
                instance = obj.AddComponent<T>();//�Զ�����
                DontDestroyOnLoad(obj);//���������Ƴ�����֤��������Ϸ�������ڶ�����
            }
            return instance;
        }
    }
}

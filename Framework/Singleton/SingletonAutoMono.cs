using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承Mono且能自动挂载的单例模式基类
/// 推荐使用
/// 不要手动挂载或是通过代码添加！！！
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
                GameObject obj = new GameObject();//创建空物体
                obj.name =typeof(T).ToString();//得到T脚本类名,可以在编辑器中明确看到
                instance = obj.AddComponent<T>();//自动挂载
                DontDestroyOnLoad(obj);//过场景不移除，保证在整个游戏生命周期都存在
            }
            return instance;
        }
    }
}

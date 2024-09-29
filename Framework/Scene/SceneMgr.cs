using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
/// 场景切换管理器
/// </summary>
public class SceneMgr :BaseManager<SceneMgr>
{
    private SceneMgr()
    {

    }
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    public void LoadScene(string name,UnityAction callBack=null)
    {
        SceneManager.LoadScene(name);
        callBack?.Invoke();
    }
    /// <summary>
    /// 异步切换场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    public void LoadSceneAsync(string name,UnityAction callBack = null)
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsync(name, callBack));
    }
    private IEnumerator ReallyLoadSceneAsync(string name, UnityAction callBack)
    {
        AsyncOperation ao=SceneManager.LoadSceneAsync(name);
        //每帧检测是否加载结束
        while(!ao.isDone)
        {
            //可以在这里利用事件中心 每一帧将进度分发到想要的地方
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,ao.progress);
            yield return 0;
        }
        //避免加载太快发送不出去
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,1);
        callBack?.Invoke();
    }
}

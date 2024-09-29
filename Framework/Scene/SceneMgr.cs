using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
/// <summary>
/// �����л�������
/// </summary>
public class SceneMgr :BaseManager<SceneMgr>
{
    private SceneMgr()
    {

    }
    /// <summary>
    /// ͬ���л�����
    /// </summary>
    /// <param name="name"></param>
    /// <param name="callBack"></param>
    public void LoadScene(string name,UnityAction callBack=null)
    {
        SceneManager.LoadScene(name);
        callBack?.Invoke();
    }
    /// <summary>
    /// �첽�л�����
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
        //ÿ֡����Ƿ���ؽ���
        while(!ao.isDone)
        {
            //���������������¼����� ÿһ֡�����ȷַ�����Ҫ�ĵط�
            EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,ao.progress);
            yield return 0;
        }
        //�������̫�췢�Ͳ���ȥ
        EventCenter.Instance.EventTrigger<float>(E_EventType.E_SceneLoadChange,1);
        callBack?.Invoke();
    }
}

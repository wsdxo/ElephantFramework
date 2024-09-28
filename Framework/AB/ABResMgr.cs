using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ������AB����Editor����������ʵ���п���
/// </summary>
public class ABResMgr :BaseManager<ABResMgr>
{
    private bool isDebug = false;//�����true��ͨ���༭������
    private ABResMgr()
    {

    }
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false)where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            //�Զ���ab������Դ����ʽ ��Ӧ�ļ��������ǰ���
            T res=EditorResMgr.Instance.LoadEditorRes<T>($"{abName}/{resName}");
            callBack?.Invoke(res);
        }
        else
        {
            ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
        }
#else
        ABMgr.Instance.LoadResAsync<T>(abName, resName, callBack, isSync);
#endif
    }
}

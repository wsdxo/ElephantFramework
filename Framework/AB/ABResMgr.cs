using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 整合了AB包和Editor管理器，其实可有可无
/// </summary>
public class ABResMgr :BaseManager<ABResMgr>
{
    private bool isDebug = false;//如果是true，通过编辑器加载
    private ABResMgr()
    {

    }
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack, bool isSync = false)where T:Object
    {
#if UNITY_EDITOR
        if(isDebug)
        {
            //自定义ab包中资源管理方式 对应文件夹名就是包名
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

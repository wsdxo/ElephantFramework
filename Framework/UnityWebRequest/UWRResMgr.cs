using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class UWRResMgr : SingletonAutoMono<UWRResMgr>
{
    /// <summary>
    /// 利用UnityWebRequest加载资源
    /// </summary>
    /// <typeparam name="T">类型T只能是string,byte[],Texture,AssetBundle</typeparam>
    /// <param name="path">资源路径,要自己加上协议 http、ftp、file</param>
    /// <param name="callBack">加载成功的回调函数</param>
    /// <param name="failCallBack">加载失败的回调函数</param>
    public void LoadRes<T>(string path,UnityAction<T>callBack,UnityAction failCallBack)where T : class
    {
        StartCoroutine(ReallyLoadRes<T>(path,callBack,failCallBack));
    }
    public IEnumerator ReallyLoadRes<T>(string path,UnityAction<T>callBack,UnityAction failCallBack)where T : class
    {
        //string
        //byte
        //Texture
        //AB包
        Type type = typeof(T);
        //用于加载的对象
        UnityWebRequest request = null;
        if(type==typeof(string)||
            type == typeof(byte[]))
        {
            request = UnityWebRequest.Get(path);
        }
        else if(type==typeof(Texture))
        {
            request=UnityWebRequestTexture.GetTexture(path);
        }
        else if (type == typeof(AssetBundle))
        {
            request=UnityWebRequestAssetBundle.GetAssetBundle(path);
        }
        else
        {
            failCallBack?.Invoke();
            yield break;
        }
        yield return request.SendWebRequest();
        //如果加载成功
        if(request.result==UnityWebRequest.Result.Success)
        {
            if (type == typeof(string))
            {
                callBack?.Invoke(request.downloadHandler.text as T);
            }
            else if(type== typeof(byte[]))
            {
                callBack?.Invoke(request.downloadHandler.data as T);
            }
            else if (type == typeof(Texture))
            {
                callBack?.Invoke(DownloadHandlerTexture.GetContent(request)as T);
            }
            else if (type == typeof(AssetBundle))
            {
                callBack?.Invoke(DownloadHandlerAssetBundle.GetContent(request) as T);
            }
        }
        else
        {
            failCallBack?.Invoke();
            Debug.LogError(request.error);

        }
        request.Dispose();
    }
}

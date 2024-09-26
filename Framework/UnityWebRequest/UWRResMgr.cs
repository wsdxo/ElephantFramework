using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class UWRResMgr : SingletonAutoMono<UWRResMgr>
{
    /// <summary>
    /// ����UnityWebRequest������Դ
    /// </summary>
    /// <typeparam name="T">����Tֻ����string,byte[],Texture,AssetBundle</typeparam>
    /// <param name="path">��Դ·��,Ҫ�Լ�����Э�� http��ftp��file</param>
    /// <param name="callBack">���سɹ��Ļص�����</param>
    /// <param name="failCallBack">����ʧ�ܵĻص�����</param>
    public void LoadRes<T>(string path,UnityAction<T>callBack,UnityAction failCallBack)where T : class
    {
        StartCoroutine(ReallyLoadRes<T>(path,callBack,failCallBack));
    }
    public IEnumerator ReallyLoadRes<T>(string path,UnityAction<T>callBack,UnityAction failCallBack)where T : class
    {
        //string
        //byte
        //Texture
        //AB��
        Type type = typeof(T);
        //���ڼ��صĶ���
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
        //������سɹ�
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

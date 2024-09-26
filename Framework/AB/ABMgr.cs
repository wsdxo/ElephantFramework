using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ABMgr : SingletonAutoMono<ABMgr>
{
    //����
    private AssetBundle mainAB=null;

    //��ȡ�����������ļ�
    AssetBundleManifest manifest=null;

    //���� �洢���ع���ab��
    private Dictionary<string,AssetBundle> abDic = new Dictionary<string,AssetBundle>();

    /// <summary>
    /// ab�����·���������޸�
    /// </summary>
    private string urlPath
    {
        get
        {
            return Application.streamingAssetsPath+"/";
        }
    }
    /// <summary>
    /// �������������޸�
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";

#elif UNITY_ANDROID
            return "ANDROID";
#else
            return "PC";
#endif
        }
    }
    /// <summary>
    /// ��������
    /// </summary>
    public void LoadMainAB()
    {
        //����ab����
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(urlPath + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    public void LoadAB(string abName)
    {
        //����ab����
        LoadMainAB();
        //��ȡ����
        AssetBundle ab = null;
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(urlPath + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        //����Ŀ���
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(urlPath + abName);
            abDic.Add(abName, ab);
        }
    }
    ///// <summary>
    ///// ͬ������
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName,string resName)
    //{
    //    LoadAB(abName);//����ab��
    //    return abDic[abName].LoadAsset(resName);
    //}

    ///// <summary>
    ///// ͬ������ ����type
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName,string resName,System.Type type)
    //{
    //    LoadAB(abName);
    //    return abDic[abName].LoadAsset(resName,type);
    //}
    ///// <summary>
    ///// ͬ�����أ����ݷ���
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public T LoadRes<T>(string abName,string resName)where T:Object
    //{
    //    LoadAB(abName);
    //    return abDic[abName].LoadAsset<T>(resName);
    //}
    /// <summary>
    /// �첽����
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName,string resName,UnityAction<Object>callBack,bool isSync=false)
    {
        StartCoroutine(ReallyLoadResAsync(abName,resName,callBack,isSync));
    }
    private IEnumerator ReallyLoadResAsync(string abName,string resName, UnityAction<Object> callBack,bool isSync=false)
    {
        LoadMainAB();
        //����������
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //�����û���ع�ab��
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(urlPath + strs[i]);
                    abDic.Add((strs[i]), ab);
                }
                else
                {
                    abDic.Add(strs[i], null);//��ռλ����ֹ֮�������ͬ�ĵļ���
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;//ֻҪ���ּ����У��͵ȴ�
                }
            }
        }
        //����Ŀ���
        if (!abDic.ContainsKey(abName))
        {
            if (isSync)
            {
                AssetBundle ab=AssetBundle.LoadFromFile(urlPath + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + abName);
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }
        }
        else
        {
            while (abDic[abName] == null)
            {
                yield return 0;//ֻҪ���ּ����У��͵ȴ�
            }
        }
        //���ذ�����Դ
        if (isSync)
        {
            Object obj = abDic[abName].LoadAsset(resName);
            callBack(obj);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);

            yield return abr;

            callBack(abr.asset);
        }
    }
    /// <summary>
    /// ����Type�첽����
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName,string resName,System.Type type,UnityAction<Object> callBack,bool isSync=false)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName,type,callBack,isSync));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack,bool isSync=false)
    {
        LoadMainAB();
        //����������
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(urlPath + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    abDic.Add(strs[i], null);//��ռλ����ֹ֮�������ͬ�ĵļ���
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;//ֻҪ���ּ����У��͵ȴ�
                }
            }
        }
        //����Ŀ���
        if (!abDic.ContainsKey(abName))
        {
            if(isSync)
            {
                AssetBundle ab=AssetBundle.LoadFromFile(urlPath + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + abName);
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }
        }
        else
        {
            while (abDic[abName] == null)
            {
                yield return 0;//ֻҪ���ּ����У��͵ȴ�
            }
        }
        //���ذ�����Դ
        if(isSync)
        {
            Object obj=abDic[abName].LoadAsset(resName,type);
            callBack(obj);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);

            yield return abr;

            callBack(abr.asset);
        }
    }
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack,bool isSync=false)where T:Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack,isSync));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack,bool isSync=false)where T: Object
    {
        LoadMainAB();
        //����������
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(urlPath + strs[i]);
                    abDic.Add(strs[i], ab);
                }
                else
                {
                    abDic.Add(strs[i], null);//��ռλ����ֹ֮�������ͬ�ĵļ���
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while(abDic[strs[i]] == null)
                {
                    yield return 0;//ֻҪ���ּ����У��͵ȴ�
                }
            }
        }
        //����Ŀ���
        if (!abDic.ContainsKey(abName))
        {
            if(isSync)
            {
                AssetBundle ab=AssetBundle.LoadFromFile(urlPath + abName);
                abDic.Add(abName, ab);
            }
            else
            {
                abDic.Add(abName, null);
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + abName);
                yield return abcr;
                abDic[abName] = abcr.assetBundle;
            }
        }
        else
        {
            while (abDic[abName] == null)
            {
                yield return 0;//ֻҪ���ּ����У��͵ȴ�
            }
        }
        //���ذ�����Դ
        if (isSync)
        {
            T res=abDic[abName].LoadAsset<T>(resName);
            callBack(res);
        }
        else
        {
            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);

            yield return abr;

            callBack(abr.asset as T);
        }
    }
    //������ж��
    public void UnLoad(string abName,UnityAction<bool>callBackResult) 
    {
        if (abDic.ContainsKey(abName))
        {
            //���ڼ���ab��������ж��
            if (abDic[abName]==null)
            {
                callBackResult(false);
                return;
            }
            abDic[abName].Unload(false);
            abDic.Remove(abName);
            callBackResult(true);
        }
    }
    //ȫ��ж��
    public void ClearAB()
    {
        //�����첽���أ���������֮ǰֹͣЭ��
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest=null;
    }
}

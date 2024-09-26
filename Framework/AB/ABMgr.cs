using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ABMgr : SingletonAutoMono<ABMgr>
{
    //主包
    private AssetBundle mainAB=null;

    //获取依赖的配置文件
    AssetBundleManifest manifest=null;

    //容器 存储加载过的ab包
    private Dictionary<string,AssetBundle> abDic = new Dictionary<string,AssetBundle>();

    /// <summary>
    /// ab包存放路径，方便修改
    /// </summary>
    private string urlPath
    {
        get
        {
            return Application.streamingAssetsPath+"/";
        }
    }
    /// <summary>
    /// 主包名，方便修改
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
    /// 加载主包
    /// </summary>
    public void LoadMainAB()
    {
        //加载ab主包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(urlPath + MainABName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
    }

    public void LoadAB(string abName)
    {
        //加载ab主包
        LoadMainAB();
        //获取依赖
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
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            ab = AssetBundle.LoadFromFile(urlPath + abName);
            abDic.Add(abName, ab);
        }
    }
    ///// <summary>
    ///// 同步加载
    ///// </summary>
    ///// <param name="abName"></param>
    ///// <param name="resName"></param>
    ///// <returns></returns>
    //public Object LoadRes(string abName,string resName)
    //{
    //    LoadAB(abName);//加载ab包
    //    return abDic[abName].LoadAsset(resName);
    //}

    ///// <summary>
    ///// 同步加载 根据type
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
    ///// 同步加载，根据泛型
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
    /// 异步加载
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
        //加载依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            //如果还没加载过ab包
            if (!abDic.ContainsKey(strs[i]))
            {
                if (isSync)
                {
                    AssetBundle ab = AssetBundle.LoadFromFile(urlPath + strs[i]);
                    abDic.Add((strs[i]), ab);
                }
                else
                {
                    abDic.Add(strs[i], null);//先占位，防止之后调用相同的的加载
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;//只要发现加载中，就等待
                }
            }
        }
        //加载目标包
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
                yield return 0;//只要发现加载中，就等待
            }
        }
        //加载包中资源
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
    /// 根据Type异步加载
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
        //加载依赖包
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
                    abDic.Add(strs[i], null);//先占位，防止之后调用相同的的加载
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while (abDic[strs[i]] == null)
                {
                    yield return 0;//只要发现加载中，就等待
                }
            }
        }
        //加载目标包
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
                yield return 0;//只要发现加载中，就等待
            }
        }
        //加载包中资源
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
        //加载依赖包
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
                    abDic.Add(strs[i], null);//先占位，防止之后调用相同的的加载
                    AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(urlPath + strs[i]);
                    yield return abcr;
                    abDic[strs[i]] = abcr.assetBundle;
                }
            }
            else
            {
                while(abDic[strs[i]] == null)
                {
                    yield return 0;//只要发现加载中，就等待
                }
            }
        }
        //加载目标包
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
                yield return 0;//只要发现加载中，就等待
            }
        }
        //加载包中资源
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
    //单个包卸载
    public void UnLoad(string abName,UnityAction<bool>callBackResult) 
    {
        if (abDic.ContainsKey(abName))
        {
            //正在加载ab包，不能卸载
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
    //全部卸载
    public void ClearAB()
    {
        //由于异步加载，所以清理之前停止协程
        StopAllCoroutines();
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest=null;
    }
}

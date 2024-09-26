using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class BaseResInfo 
{
    //引用计数
    public int refCount = 0;
}
/// <summary>
/// 资源信息对象
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : BaseResInfo
{
    //资源
    public T asset;
    //回调函数
    public UnityAction<T> callBack;
    //存储异步加载时开启的协同程序
    public Coroutine coroutine;
    //是否在引用计数为0时立刻移除
    public bool isDel = false;

    public void AddRefCount()
    {
        ++refCount;
    }
    public void SubRefCount()
    {
        --refCount;
        if (refCount < 0)
        {
            Debug.LogError("引用计数小于0了，请检查使用和卸载是否配对执行");
        }
    }
}
public class ResMgr :BaseManager<ResMgr>
{
    //用于存储加载过或者加载中的资源
    private Dictionary<string,BaseResInfo>resDic= new Dictionary<string,BaseResInfo>();
    private ResMgr()
    {

    }
    /// <summary>
    /// 同步加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public T Load<T>(string path)where T:UnityEngine.Object
    {
        string resName=path+"_"+typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            T res=Resources.Load<T>(path);
            info=new ResInfo<T>();
            info.asset = res;
            info.AddRefCount();//引用计数增加
            resDic.Add(resName, info);
            return info.asset;
        }
        else
        {
            info = resDic[resName]as ResInfo<T>;
            info.AddRefCount();//引用计数增加
            if (info.asset == null)
            {
                //存在异步加载，正在加载中
                MonoMgr.Instance.StopCoroutine(info.coroutine);
                T res = Resources.Load<T>(path);
                info.asset= res;
                info.callBack?.Invoke(info.asset);
                info.callBack = null;
                info.coroutine = null;
                return info.asset;
            }
            else
            {
                //如果加载结束，直接用
                return info.asset;
            }
        }
    }
    /// <summary>
    /// 异步加载资源的方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径（Resources下）</param>
    /// <param name="callBack">加载结束后的回调函数，异步加载结束后调用</param>
    public void LoadAsync<T>(string path,UnityAction<T> callBack)where T:UnityEngine.Object
    {
        //资源的唯一标识用 路径名_资源类型 拼接而成
        string resName=path+"_"+typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            info= new ResInfo<T>();
            info.AddRefCount();//引用计数增加
            resDic.Add(resName,info);
            info.callBack += callBack;
            info.coroutine=MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            info = resDic[resName]as ResInfo<T>;
            info.AddRefCount();//引用计数增加
            //如果资源没加载完
            //意味着还在进行异步加载
            if (info.asset==null)
            {
                info.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(info.asset);
            }
        }
        //通过协同程序异步加载资源
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path,callBack));
    }
    private IEnumerator ReallyLoadAsync<T>(string path)where T : UnityEngine.Object
    {
        ResourceRequest rq=Resources.LoadAsync<T>(path);//异步加载
        yield return rq;//加载结束之后才执行后面的代码
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            //资源加载结束 将资源取出并在字典中记录
            ResInfo<T> resInfo=(resDic[resName]as ResInfo<T>);
            resInfo.asset = rq.asset as T;
            if (resInfo.refCount == 0)
            {
                UnloadAsset<T>(path,resInfo.isDel,null,false);
            }
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //加载完毕后清空这些引用 避免内存的占用 带来的可能的内存泄漏
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    /// <summary>
    /// 资源异步加载的方法
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="type">资源类型</param>
    /// <param name="callBack">回调函数</param>
    [Obsolete("建议使用泛型加载方式，如果实在要用Type加载，一定不能和泛型混用去加载同类型同名资源")]
    public void LoadAsync(string path,Type type,UnityAction<UnityEngine.Object> callBack,bool isSub=true)
    {
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> info;
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<UnityEngine.Object>();
            info.AddRefCount();//引用计数增加
            resDic.Add(resName, info);
            info.callBack += callBack;
            MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            info = resDic[resName]as ResInfo<UnityEngine.Object>;
            info.AddRefCount();//引用计数增加
            if (info.asset == null)
            {
                info.callBack += callBack;
            }
            else
            {
                info.callBack?.Invoke(info.asset);
            }
        }
    }
    private IEnumerator ReallyLoadAsync(string path,Type type)
    {
        ResourceRequest rq=Resources.LoadAsync(path,type);
        yield return rq;
        string resName=path + "_" + type.Name;
        if(resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo=resDic[resName]as ResInfo<UnityEngine.Object>;
            resInfo.asset=rq.asset;
            if (resInfo.refCount==0)
            {
                UnloadAsset(path,type,resInfo.isDel,null,false);
            }
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);

                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
        //callBack(rq.asset);
    }
    /// <summary>
    /// 指定卸载某个资源
    /// </summary>
    /// <param name="assetToUnload"></param>
    public void UnloadAsset<T>(string path,bool isDel=false,UnityAction<T> callBack=null,bool isSub=true)
    {
        string resName=path+"_"+typeof(T).Name;
        if(resDic.ContainsKey (resName))
        {
            ResInfo<T> resInfo = resDic[resName]as ResInfo<T>;
            if(isSub)
                resInfo.SubRefCount();
            resInfo.isDel= isDel;
            //资源加载完卸载
            if(resInfo.asset != null&&resInfo.refCount==0&&resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //资源正在异步加载中
            else if(resInfo.asset==null)
            {
                //resInfo.isDel = true;
                resInfo.callBack-=callBack;
            }
        }
        //Resources.UnloadAsset(assetToUnload);
    }
    public void UnloadAsset(string path,Type type,bool isDel = false,UnityAction < UnityEngine.Object>callBack=null,bool isSub=true)
    {
        string resName = path + "_" + type.Name;
        if (resDic.ContainsKey(resName))
        {
            ResInfo<UnityEngine.Object> resInfo = resDic[resName] as ResInfo<UnityEngine.Object>;
            if(isSub)
                resInfo.SubRefCount();
            resInfo.isDel = isDel;
            //资源加载完
            if (resInfo.asset != null&&resInfo.refCount==0&&resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            //资源正在异步加载中
            else if(resInfo.asset==null)
            {
                //resInfo.isDel = true;
                resInfo.callBack-= callBack;
            }
        }
        //Resources.UnloadAsset(assetToUnload);
    }
    /// <summary>
    /// 异步卸载没有使用的资源
    /// </summary>
    /// <param name="callBack"></param>
    public void UnloadUnusedAsset(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyUnloadUnusedAsset(callBack));
    }
    private IEnumerator ReallyUnloadUnusedAsset(UnityAction callBack)
    {
        List<string>list= new List<string>();
        foreach(string path in resDic.Keys)
        {
            if (resDic[path].refCount==0)
            {
                list.Add(path);
            }
        }
        foreach(string path in list)
        {
            resDic.Remove(path);
        }
        AsyncOperation ao=Resources.UnloadUnusedAssets();
        yield return ao;

        //卸载完毕后调用
        callBack();
    }

    public int GetRefCount<T>(string path)
    {
        string resName=path+"_"+typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            return (resDic[resName]as ResInfo<T>).refCount;
        }
        return 0;
    }

    public void ClearDic(UnityAction callBack)
    {
        MonoMgr.Instance.StartCoroutine(ReallyClearDic(callBack));
    }
    private IEnumerator ReallyClearDic(UnityAction callBack)
    {
        resDic.Clear();
        AsyncOperation ao = Resources.UnloadUnusedAssets();
        yield return ao;

        //卸载完毕后调用
        callBack();
    }
}

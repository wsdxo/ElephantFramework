using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public abstract class BaseResInfo 
{
    //���ü���
    public int refCount = 0;
}
/// <summary>
/// ��Դ��Ϣ����
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResInfo<T> : BaseResInfo
{
    //��Դ
    public T asset;
    //�ص�����
    public UnityAction<T> callBack;
    //�洢�첽����ʱ������Эͬ����
    public Coroutine coroutine;
    //�Ƿ������ü���Ϊ0ʱ�����Ƴ�
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
            Debug.LogError("���ü���С��0�ˣ�����ʹ�ú�ж���Ƿ����ִ��");
        }
    }
}
public class ResMgr :BaseManager<ResMgr>
{
    //���ڴ洢���ع����߼����е���Դ
    private Dictionary<string,BaseResInfo>resDic= new Dictionary<string,BaseResInfo>();
    private ResMgr()
    {

    }
    /// <summary>
    /// ͬ������
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
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
            info.AddRefCount();//���ü�������
            resDic.Add(resName, info);
            return info.asset;
        }
        else
        {
            info = resDic[resName]as ResInfo<T>;
            info.AddRefCount();//���ü�������
            if (info.asset == null)
            {
                //�����첽���أ����ڼ�����
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
                //������ؽ�����ֱ����
                return info.asset;
            }
        }
    }
    /// <summary>
    /// �첽������Դ�ķ���
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·����Resources�£�</param>
    /// <param name="callBack">���ؽ�����Ļص��������첽���ؽ��������</param>
    public void LoadAsync<T>(string path,UnityAction<T> callBack)where T:UnityEngine.Object
    {
        //��Դ��Ψһ��ʶ�� ·����_��Դ���� ƴ�Ӷ���
        string resName=path+"_"+typeof(T).Name;
        ResInfo<T> info;
        if (!resDic.ContainsKey(resName))
        {
            info= new ResInfo<T>();
            info.AddRefCount();//���ü�������
            resDic.Add(resName,info);
            info.callBack += callBack;
            info.coroutine=MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path));
        }
        else
        {
            info = resDic[resName]as ResInfo<T>;
            info.AddRefCount();//���ü�������
            //�����Դû������
            //��ζ�Ż��ڽ����첽����
            if (info.asset==null)
            {
                info.callBack += callBack;
            }
            else
            {
                callBack?.Invoke(info.asset);
            }
        }
        //ͨ��Эͬ�����첽������Դ
        //MonoMgr.Instance.StartCoroutine(ReallyLoadAsync<T>(path,callBack));
    }
    private IEnumerator ReallyLoadAsync<T>(string path)where T : UnityEngine.Object
    {
        ResourceRequest rq=Resources.LoadAsync<T>(path);//�첽����
        yield return rq;//���ؽ���֮���ִ�к���Ĵ���
        string resName = path + "_" + typeof(T).Name;
        if (resDic.ContainsKey(resName))
        {
            //��Դ���ؽ��� ����Դȡ�������ֵ��м�¼
            ResInfo<T> resInfo=(resDic[resName]as ResInfo<T>);
            resInfo.asset = rq.asset as T;
            if (resInfo.refCount == 0)
            {
                UnloadAsset<T>(path,resInfo.isDel,null,false);
            }
            else
            {
                resInfo.callBack?.Invoke(resInfo.asset);
                //������Ϻ������Щ���� �����ڴ��ռ�� �����Ŀ��ܵ��ڴ�й©
                resInfo.callBack = null;
                resInfo.coroutine = null;
            }
        }
    }
    /// <summary>
    /// ��Դ�첽���صķ���
    /// </summary>
    /// <param name="path">��Դ·��</param>
    /// <param name="type">��Դ����</param>
    /// <param name="callBack">�ص�����</param>
    [Obsolete("����ʹ�÷��ͼ��ط�ʽ�����ʵ��Ҫ��Type���أ�һ�����ܺͷ��ͻ���ȥ����ͬ����ͬ����Դ")]
    public void LoadAsync(string path,Type type,UnityAction<UnityEngine.Object> callBack,bool isSub=true)
    {
        string resName = path + "_" + type.Name;
        ResInfo<UnityEngine.Object> info;
        if (!resDic.ContainsKey(resName))
        {
            info = new ResInfo<UnityEngine.Object>();
            info.AddRefCount();//���ü�������
            resDic.Add(resName, info);
            info.callBack += callBack;
            MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(path, type));
        }
        else
        {
            info = resDic[resName]as ResInfo<UnityEngine.Object>;
            info.AddRefCount();//���ü�������
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
    /// ָ��ж��ĳ����Դ
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
            //��Դ������ж��
            if(resInfo.asset != null&&resInfo.refCount==0&&resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset as UnityEngine.Object);
            }
            //��Դ�����첽������
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
            //��Դ������
            if (resInfo.asset != null&&resInfo.refCount==0&&resInfo.isDel)
            {
                resDic.Remove(resName);
                Resources.UnloadAsset(resInfo.asset);
            }
            //��Դ�����첽������
            else if(resInfo.asset==null)
            {
                //resInfo.isDel = true;
                resInfo.callBack-= callBack;
            }
        }
        //Resources.UnloadAsset(assetToUnload);
    }
    /// <summary>
    /// �첽ж��û��ʹ�õ���Դ
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

        //ж����Ϻ����
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

        //ж����Ϻ����
        callBack();
    }
}

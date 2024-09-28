using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
/// ui层级枚举
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System,
}
/// <summary>
/// UI管理器模块
/// 注意：面板类名和面板名字一致
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    /// <summary>
    /// 利用里氏替换原则原则 在字典中装载子类对象
    /// </summary>
    private abstract class BasePanelInfo
    {

    }
    private class PanelInfo<T> : BasePanelInfo where T : BasePanel
    {
        public T panel;
        public UnityAction<T> callBack;
        public bool isHide=false;
        public PanelInfo(UnityAction<T>callBack)
        {
            this.callBack += callBack;
        }
    }

    private Camera uiCamera;
    private Canvas uiCanvas;
    private EventSystem uiEventSystem;

    private Transform bottomLayer;
    private Transform middleLayer;
    private Transform topLayer;
    private Transform systemLayer;

    //规则 键：面板名字 值：面板
    private Dictionary<string,BasePanelInfo>panelDic = new Dictionary<string,BasePanelInfo>();

    private UIMgr()
    {
        uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

        uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        //找到层级父对象
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

        uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }
    /// <summary>
    /// 获取对应层级的父对象
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetFather(E_UILayer layer)
    {
        switch (layer)
        {
            case E_UILayer.Bottom:
                return bottomLayer;
            case E_UILayer.Top:
                return topLayer;
            case E_UILayer.Middle:
                return middleLayer;
            case E_UILayer.System:
                return systemLayer;
            default:
                return null;
        }
    }
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="layer">层级</param>
    /// <param name="callBack">回调函数</param>
    /// <param name="isSync">是否同步加载</param>
    public void ShowPanel<T>(E_UILayer layer=E_UILayer.Middle,UnityAction<T>callBack=null,bool isSync=false)where T:BasePanel
    {

        //存在
        string panelName=typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName]as PanelInfo<T>;
            //正在异步加载中
            if (panelInfo.panel==null)
            {
                panelInfo.isHide = false;
                if(callBack!=null)
                    panelInfo.callBack += callBack;
            }
            //已经加载结束
            else
            {
                //如果面板只是被失活 激活就行
                if(!panelInfo.panel.gameObject.activeSelf)
                {
                    panelInfo.panel.gameObject.SetActive(true);
                }
                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);
            }
            return;
        }
        //如果不存在面板 先存入字典 占个位置
        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        //加载面板
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "TestPanel", (res) =>
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //异步加载结束前 就想要隐藏了
            if (panelInfo.isHide)
            {
                panelDic.Remove(panelName);
                return;
            }

            Transform father=GetFather(layer);
            if(father==null)
            {
                father = middleLayer;
            }
            GameObject panelObj=GameObject.Instantiate(res, father,false);//创建面板，并保持原本缩放


            T panel=panelObj.GetComponent<T>();

            panel.ShowMe();

            panelInfo.callBack?.Invoke(panel);
            panelInfo.callBack = null;//回调执行完 清空 避免内存泄漏

            panelInfo.panel=panel;
        }, isSync);
    }
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public void HidePanel<T>(bool isDestory=false)where T:BasePanel
    {
        string panelName= typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;//隐藏了就不调用回调函数 直接置空
            }
            //加载结束
            else
            {
                //销毁面板
                if (isDestory)
                {
                    panelInfo.panel.HideMe();
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                //如果不销毁 那么失活就行 也不用从字典中移除
                //这样做可能会带来内存压力 但是可以减少频繁的GC带来的性能损耗
                else
                {
                    panelInfo.panel.gameObject.SetActive(false);
                }
            }
        }
        //if (panelDic.ContainsKey(panelName))
        //{
        //    panelDic[panelName].HideMe();
        //    GameObject.Destroy(panelDic[panelName].gameObject);
        //    panelDic.Remove(panelName);
        //    return;
        //}
    }
    /// <summary>
    /// 获取面板
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public void GetPanel<T>(UnityAction<T>callBack)where T :BasePanel
    {
        string panelName=typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //正在加载中
            if(panelInfo.panel == null)
            {
                panelInfo.callBack += callBack;
            }
            //加载结束 且没有隐藏
            else if(!panelInfo.isHide)
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }

    /// <summary>
    /// 为控件添加自定义事件
    /// </summary>
    /// <param name="control">对应的控件</param>
    /// <param name="type">事件的类型</param>
    /// <param name="callBack">对应的函数</param>
    public static void AddCustomEventTriggerListener(UIBehaviour control,EventTriggerType type,UnityAction<BaseEventData>callBack)
    {
        EventTrigger trigger=control.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger=control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry=new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);
        trigger.triggers.Add(entry);
    }
}

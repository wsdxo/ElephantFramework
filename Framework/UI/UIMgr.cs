using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
/// <summary>
/// ui�㼶ö��
/// </summary>
public enum E_UILayer
{
    Bottom,
    Middle,
    Top,
    System,
}
/// <summary>
/// UI������ģ��
/// ע�⣺����������������һ��
/// </summary>
public class UIMgr : BaseManager<UIMgr>
{
    /// <summary>
    /// ���������滻ԭ��ԭ�� ���ֵ���װ���������
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

    //���� ����������� ֵ�����
    private Dictionary<string,BasePanelInfo>panelDic = new Dictionary<string,BasePanelInfo>();

    private UIMgr()
    {
        uiCamera = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/UICamera")).GetComponent<Camera>();
        GameObject.DontDestroyOnLoad(uiCamera.gameObject);

        uiCanvas = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/Canvas")).GetComponent<Canvas>();
        uiCanvas.worldCamera = uiCamera;
        GameObject.DontDestroyOnLoad(uiCanvas.gameObject);

        //�ҵ��㼶������
        bottomLayer = uiCanvas.transform.Find("Bottom");
        middleLayer = uiCanvas.transform.Find("Middle");
        topLayer = uiCanvas.transform.Find("Top");
        systemLayer = uiCanvas.transform.Find("System");

        uiEventSystem = GameObject.Instantiate(ResMgr.Instance.Load<GameObject>("UI/EventSystem")).GetComponent<EventSystem>();
        GameObject.DontDestroyOnLoad(uiEventSystem.gameObject);
    }
    /// <summary>
    /// ��ȡ��Ӧ�㼶�ĸ�����
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
    /// ��ʾ���
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    /// <param name="layer">�㼶</param>
    /// <param name="callBack">�ص�����</param>
    /// <param name="isSync">�Ƿ�ͬ������</param>
    public void ShowPanel<T>(E_UILayer layer=E_UILayer.Middle,UnityAction<T>callBack=null,bool isSync=false)where T:BasePanel
    {

        //����
        string panelName=typeof(T).Name;
        if(panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName]as PanelInfo<T>;
            //�����첽������
            if (panelInfo.panel==null)
            {
                panelInfo.isHide = false;
                if(callBack!=null)
                    panelInfo.callBack += callBack;
            }
            //�Ѿ����ؽ���
            else
            {
                //������ֻ�Ǳ�ʧ�� �������
                if(!panelInfo.panel.gameObject.activeSelf)
                {
                    panelInfo.panel.gameObject.SetActive(true);
                }
                panelInfo.panel.ShowMe();
                callBack?.Invoke(panelInfo.panel);
            }
            return;
        }
        //������������ �ȴ����ֵ� ռ��λ��
        panelDic.Add(panelName, new PanelInfo<T>(callBack));
        //�������
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "TestPanel", (res) =>
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //�첽���ؽ���ǰ ����Ҫ������
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
            GameObject panelObj=GameObject.Instantiate(res, father,false);//������壬������ԭ������


            T panel=panelObj.GetComponent<T>();

            panel.ShowMe();

            panelInfo.callBack?.Invoke(panel);
            panelInfo.callBack = null;//�ص�ִ���� ��� �����ڴ�й©

            panelInfo.panel=panel;
        }, isSync);
    }
    /// <summary>
    /// �������
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    public void HidePanel<T>(bool isDestory=false)where T:BasePanel
    {
        string panelName= typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //���ڼ�����
            if (panelInfo.panel == null)
            {
                panelInfo.isHide = true;
                panelInfo.callBack = null;//�����˾Ͳ����ûص����� ֱ���ÿ�
            }
            //���ؽ���
            else
            {
                //�������
                if (isDestory)
                {
                    panelInfo.panel.HideMe();
                    GameObject.Destroy(panelInfo.panel.gameObject);
                    panelDic.Remove(panelName);
                }
                //��������� ��ôʧ����� Ҳ���ô��ֵ����Ƴ�
                //���������ܻ�����ڴ�ѹ�� ���ǿ��Լ���Ƶ����GC�������������
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
    /// ��ȡ���
    /// </summary>
    /// <typeparam name="T">����</typeparam>
    public void GetPanel<T>(UnityAction<T>callBack)where T :BasePanel
    {
        string panelName=typeof(T).Name;
        if (panelDic.ContainsKey(panelName))
        {
            PanelInfo<T> panelInfo = panelDic[panelName] as PanelInfo<T>;
            //���ڼ�����
            if(panelInfo.panel == null)
            {
                panelInfo.callBack += callBack;
            }
            //���ؽ��� ��û������
            else if(!panelInfo.isHide)
            {
                callBack?.Invoke(panelInfo.panel);
            }
        }
    }

    /// <summary>
    /// Ϊ�ؼ�����Զ����¼�
    /// </summary>
    /// <param name="control">��Ӧ�Ŀؼ�</param>
    /// <param name="type">�¼�������</param>
    /// <param name="callBack">��Ӧ�ĺ���</param>
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

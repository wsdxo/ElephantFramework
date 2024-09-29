using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventInfoBase
{
    
}
public class EventInfo<T>:EventInfoBase
{
    public UnityAction<T> actions=null;
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
public class EventInfo:EventInfoBase
{
    public UnityAction actions=null;
    public EventInfo(UnityAction action) 
    {
        actions += action;
    }
}
/// <summary>
/// �¼����Ĺ�����
/// ע�� �¼��ĺ����мӾ��м�
/// </summary>
public class EventCenter :BaseManager<EventCenter>
{
    private Dictionary<E_EventType,EventInfoBase>eventDic = new Dictionary<E_EventType,EventInfoBase>();
    private EventCenter()
    {

    }
    /// <summary>
    /// �����¼�
    /// �в�
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger<T>(E_EventType eventName,T info)
    {
        //�����¼���֪ͨ���˴���
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// �����¼�
    /// �޲�
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo).actions?.Invoke();
        }
    } 
    /// <summary>
    /// ����¼�������
    /// �в�
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions += func;
        }
        else
        {
            eventDic.Add(eventName, new EventInfo<T>(func));
        }
    }
    /// <summary>
    /// ����¼�������
    /// �޲�
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void AddEventListener(E_EventType eventName,UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo).actions += func;
        }
        else
        {
            eventDic.Add(eventName,new EventInfo(func));
        }
    }
    /// <summary>
    /// �Ƴ��¼�������
    /// �в�
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener<T>(E_EventType eventName,UnityAction<T> func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName] as EventInfo<T>).actions -= func;
        }
    }
    /// <summary>
    /// �Ƴ��¼�������
    /// �޲�
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="func"></param>
    public void RemoveEventListener(E_EventType eventName,UnityAction func)
    {
        if (eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo).actions -= func;
        }
    }
    /// <summary>
    /// ��������¼��ļ���
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    /// <summary>
    /// ���ָ���¼������м���
    /// </summary>
    /// <param name="eventName"></param>
    public void Clear(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            eventDic.Remove(eventName);
        }
    }
}

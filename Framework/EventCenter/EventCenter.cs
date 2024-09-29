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
/// 事件中心管理器
/// 注意 事件的函数有加就有减
/// </summary>
public class EventCenter :BaseManager<EventCenter>
{
    private Dictionary<E_EventType,EventInfoBase>eventDic = new Dictionary<E_EventType,EventInfoBase>();
    private EventCenter()
    {

    }
    /// <summary>
    /// 触发事件
    /// 有参
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger<T>(E_EventType eventName,T info)
    {
        //存在事件才通知别人处理
        if(eventDic.ContainsKey(eventName))
        {
            (eventDic[eventName]as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// 触发事件
    /// 无参
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
    /// 添加事件监听者
    /// 有参
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
    /// 添加事件监听者
    /// 无参
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
    /// 移除事件监听者
    /// 有参
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
    /// 移除事件监听者
    /// 无参
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
    /// 清空所有事件的监听
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
    /// <summary>
    /// 清除指定事件的所有监听
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

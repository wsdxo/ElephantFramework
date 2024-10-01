using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 计时器
/// 倒计时！！！
/// 倒计时！！！
/// 倒计时！！！
/// </summary>
public class TimerItem:IPoolObject
{
    public int keyID;
    /// <summary>
    /// 计时器总时间 单位毫秒 1s=1000ms
    /// </summary>
    public int allTime;
    /// <summary>
    /// 一开始的总时间 用于重置计时器 毫秒
    /// </summary>
    public int maxAllTime;
    /// <summary>
    /// 延时执行回调
    /// </summary>
    public UnityAction overCallBack;
    /// <summary>
    /// 间隔时间 毫秒
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// 一开始的间隔时间 毫秒 用于重置计时器
    /// </summary>
    public int maxIntervalTime;
    /// <summary>
    /// 间隔执行回调
    /// </summary>
    public UnityAction inervalCallBack;

    public bool isRunning;

    public void InitInfo(int keyID,int allTime,UnityAction overCallBack,int intervalAllTime=0,UnityAction intervalCallBack=null)
    {
        this.keyID = keyID;
        this.maxAllTime=this.allTime = allTime;
        this.overCallBack = overCallBack;
        this.maxIntervalTime=this.intervalTime = intervalAllTime;
        this.inervalCallBack = intervalCallBack;
        isRunning = true;
    }
    /// <summary>
    /// 重置计时器
    /// </summary>
    public void ResetTimer()
    {
        allTime = maxAllTime;
        intervalTime = maxIntervalTime;
        isRunning = true;
    }
    /// <summary>
    /// 缓存池回收时 清除相关的索引
    /// </summary>
    public void ResetInfo()
    {
        overCallBack = null;
        inervalCallBack = null;
    }
}

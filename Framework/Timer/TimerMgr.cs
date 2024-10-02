using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 计时器管理器
/// 用于开启 停止 重置等操作来管理计时器
/// </summary>
public class TimerMgr :BaseManager<TimerMgr>
{
    /// <summary>
    /// 存储受Time.Scale影响的计时器
    /// </summary>
    private Dictionary<int,TimerItem>timerDic= new Dictionary<int,TimerItem>();
    /// <summary>
    /// 不受Time.Scale影响的计时器
    /// </summary>
    private Dictionary<int, TimerItem> realTimerDic = new Dictionary<int, TimerItem>();
    //待移除列表
    private List<TimerItem>delList= new List<TimerItem>();
    Coroutine timer;
    Coroutine realTimer;
    //用于创建当前要创建的id
    private int availableID = 0;
    private const float intervalTime = 0.1f;
    private TimerMgr()
    {
        Start();
    }
    //开启计时器管理器
    public void Start()
    {
        timer=MonoMgr.Instance.StartCoroutine(StartTiming(false,timerDic));
        realTimer=MonoMgr.Instance.StartCoroutine(StartTiming(true,realTimerDic));
    }
    //关闭计时器管理器
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
        MonoMgr.Instance.StopCoroutine(realTimer);
    }
    WaitForSeconds waitForSeconds=new WaitForSeconds(intervalTime);
    WaitForSecondsRealtime waitForSecondsRealtime = new WaitForSecondsRealtime(intervalTime);
    private IEnumerator StartTiming(bool isRealTime, Dictionary<int, TimerItem> timerDic)
    {
        while(true)
        {
            if(isRealTime)
            {
                yield return waitForSecondsRealtime;
            }
            else
            {
                yield return waitForSeconds;
            }
            foreach(TimerItem item in timerDic.Values)
            {
                if(!item.isRunning)
                {
                    continue;
                }
                //判断是否有间隔时间执行的需求
                if(item.inervalCallBack != null)
                {
                    //减去对应的毫秒
                    item.intervalTime-=(int)(intervalTime*1000);
                    if (item.intervalTime <= 0)
                    {
                        //执行
                        item.inervalCallBack.Invoke();
                        //重置间隔时间
                        item.intervalTime=item.maxIntervalTime;
                    }
                }
                //更新总时间
                item.allTime-=(int)(intervalTime*1000);
                if(item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }
            for(int i=0; i<delList.Count; i++)
            {
                //从字典中移除
                timerDic.Remove(delList[i].keyID);
                //放入缓存池
                PoolMgr.Instance.PushObj(delList[i]);
            }
            //移除后清空列表
            delList.Clear();
        }
    }
    /// <summary>
    /// 创建单个计时器
    /// </summary>
    /// <param name="allTime">总时间 ms</param>
    /// <param name="overCallBack">总时间结束回调</param>
    /// <param name="intervalTime">间隔时间</param>
    /// <param name="intervalCallBack">间隔时间结束回调</param>
    /// <returns></returns>
    public int CreateTimer(bool isRealTime,int allTime,UnityAction overCallBack,int intervalTime=0,UnityAction intervalCallBack=null)
    {
        //构建唯一id
        int keyID=availableID++;
        //从缓存池取出计时器
        TimerItem timerItem=PoolMgr.Instance.GetObj<TimerItem>();
        //初始化
        timerItem.InitInfo(keyID,allTime,overCallBack,intervalTime,intervalCallBack);
        //记录到字典
        if(isRealTime )
        {
            realTimerDic.Add(keyID, timerItem);
        }
        else
        {
            timerDic.Add(keyID, timerItem);
        }
        return keyID;
    }
    /// <summary>
    /// 移除单个计时器
    /// </summary>
    /// <param name="id">想要移除的计时器的id 放入缓存池</param>
    public void RemoveTimer(int id)
    {
        if(timerDic.ContainsKey(id))
        {
            PoolMgr.Instance.PushObj(timerDic[id]);
            timerDic.Remove(id);
        }
        else if(realTimerDic.ContainsKey(id))
        {
            PoolMgr.Instance.PushObj(realTimerDic[id]);
            realTimerDic.Remove(id);
        }
    }
    /// <summary>
    /// 重置单个计时器
    /// </summary>
    /// <param name="id"></param>
    public void ResetTimer(int id)
    {
        if(timerDic.ContainsKey(id))
        {
            timerDic[id].ResetTimer();
        }
        else if (realTimerDic.ContainsKey(id))
        {
            realTimerDic[id].ResetTimer();
        }
    }
    /// <summary>
    /// 开启单个计时器
    /// </summary>
    /// <param name="id">计时器唯一id</param>
    public void StartTimer(int id)
    {
        if (timerDic.ContainsKey(id))
        {
            timerDic[id].isRunning = true;
        }
        else if(realTimerDic.ContainsKey(id))
        {
            realTimerDic[id].isRunning = true;
        }
    }
    /// <summary>
    /// 停止单个计时器
    /// </summary>
    /// <param name="id">计时器唯一id</param>
    public void StopTimer(int id)
    {
        if (timerDic.ContainsKey(id))
        {
            timerDic[id].isRunning=false;
        }
        else if (realTimerDic.ContainsKey(id))
        {
            realTimerDic[id].isRunning=false;
        }
    }
}

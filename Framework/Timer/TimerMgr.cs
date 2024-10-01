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
    private Dictionary<int,TimerItem>timerDic= new Dictionary<int,TimerItem>();
    //待移除列表
    private List<TimerItem>delList= new List<TimerItem>();
    Coroutine timer;
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
        timer=MonoMgr.Instance.StartCoroutine(StartTiming());
    }
    //关闭计时器管理器
    public void Stop()
    {
        MonoMgr.Instance.StopCoroutine(timer);
    }
    private IEnumerator StartTiming()
    {
        while(true)
        {
            yield return new WaitForSeconds(intervalTime);
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
    public int CreateTimer(int allTime,UnityAction overCallBack,int intervalTime=0,UnityAction intervalCallBack=null)
    {
        //构建唯一id
        int keyID=availableID++;
        //从缓存池取出计时器
        TimerItem timerItem=PoolMgr.Instance.GetObj<TimerItem>();
        //初始化
        timerItem.InitInfo(keyID,allTime,overCallBack,intervalTime,intervalCallBack);
        //记录到字典
        timerDic.Add(keyID, timerItem);
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
    }
}

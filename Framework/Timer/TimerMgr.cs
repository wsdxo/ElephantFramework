using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ��ʱ��������
/// ���ڿ��� ֹͣ ���õȲ����������ʱ��
/// </summary>
public class TimerMgr :BaseManager<TimerMgr>
{
    private Dictionary<int,TimerItem>timerDic= new Dictionary<int,TimerItem>();
    //���Ƴ��б�
    private List<TimerItem>delList= new List<TimerItem>();
    Coroutine timer;
    //���ڴ�����ǰҪ������id
    private int availableID = 0;
    private const float intervalTime = 0.1f;
    private TimerMgr()
    {
        Start();
    }
    //������ʱ��������
    public void Start()
    {
        timer=MonoMgr.Instance.StartCoroutine(StartTiming());
    }
    //�رռ�ʱ��������
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
                //�ж��Ƿ��м��ʱ��ִ�е�����
                if(item.inervalCallBack != null)
                {
                    //��ȥ��Ӧ�ĺ���
                    item.intervalTime-=(int)(intervalTime*1000);
                    if (item.intervalTime <= 0)
                    {
                        //ִ��
                        item.inervalCallBack.Invoke();
                        //���ü��ʱ��
                        item.intervalTime=item.maxIntervalTime;
                    }
                }
                //������ʱ��
                item.allTime-=(int)(intervalTime*1000);
                if(item.allTime <= 0)
                {
                    item.overCallBack.Invoke();
                    delList.Add(item);
                }
            }
            for(int i=0; i<delList.Count; i++)
            {
                //���ֵ����Ƴ�
                timerDic.Remove(delList[i].keyID);
                //���뻺���
                PoolMgr.Instance.PushObj(delList[i]);
            }
            //�Ƴ�������б�
            delList.Clear();
        }
    }
    /// <summary>
    /// ����������ʱ��
    /// </summary>
    /// <param name="allTime">��ʱ�� ms</param>
    /// <param name="overCallBack">��ʱ������ص�</param>
    /// <param name="intervalTime">���ʱ��</param>
    /// <param name="intervalCallBack">���ʱ������ص�</param>
    /// <returns></returns>
    public int CreateTimer(int allTime,UnityAction overCallBack,int intervalTime=0,UnityAction intervalCallBack=null)
    {
        //����Ψһid
        int keyID=availableID++;
        //�ӻ����ȡ����ʱ��
        TimerItem timerItem=PoolMgr.Instance.GetObj<TimerItem>();
        //��ʼ��
        timerItem.InitInfo(keyID,allTime,overCallBack,intervalTime,intervalCallBack);
        //��¼���ֵ�
        timerDic.Add(keyID, timerItem);
        return keyID;
    }
    /// <summary>
    /// �Ƴ�������ʱ��
    /// </summary>
    /// <param name="id">��Ҫ�Ƴ��ļ�ʱ����id ���뻺���</param>
    public void RemoveTimer(int id)
    {
        if(timerDic.ContainsKey(id))
        {
            PoolMgr.Instance.PushObj(timerDic[id]);
            timerDic.Remove(id);
        }
    }
    /// <summary>
    /// ���õ�����ʱ��
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
    /// ����������ʱ��
    /// </summary>
    /// <param name="id">��ʱ��Ψһid</param>
    public void StartTimer(int id)
    {
        if (timerDic.ContainsKey(id))
        {
            timerDic[id].isRunning = true;
        }
    }
    /// <summary>
    /// ֹͣ������ʱ��
    /// </summary>
    /// <param name="id">��ʱ��Ψһid</param>
    public void StopTimer(int id)
    {
        if (timerDic.ContainsKey(id))
        {
            timerDic[id].isRunning=false;
        }
    }
}

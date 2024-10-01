using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ��ʱ��
/// ����ʱ������
/// ����ʱ������
/// ����ʱ������
/// </summary>
public class TimerItem:IPoolObject
{
    public int keyID;
    /// <summary>
    /// ��ʱ����ʱ�� ��λ���� 1s=1000ms
    /// </summary>
    public int allTime;
    /// <summary>
    /// һ��ʼ����ʱ�� �������ü�ʱ�� ����
    /// </summary>
    public int maxAllTime;
    /// <summary>
    /// ��ʱִ�лص�
    /// </summary>
    public UnityAction overCallBack;
    /// <summary>
    /// ���ʱ�� ����
    /// </summary>
    public int intervalTime;
    /// <summary>
    /// һ��ʼ�ļ��ʱ�� ���� �������ü�ʱ��
    /// </summary>
    public int maxIntervalTime;
    /// <summary>
    /// ���ִ�лص�
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
    /// ���ü�ʱ��
    /// </summary>
    public void ResetTimer()
    {
        allTime = maxAllTime;
        intervalTime = maxIntervalTime;
        isRunning = true;
    }
    /// <summary>
    /// ����ػ���ʱ �����ص�����
    /// </summary>
    public void ResetInfo()
    {
        overCallBack = null;
        inervalCallBack = null;
    }
}

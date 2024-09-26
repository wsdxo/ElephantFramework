using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ͳһ����֡���»�ʱ����
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    /// <summary>
    /// ���Update֡���º���
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }
    /// <summary>
    /// ���fixedUpdate֡���º���
    /// </summary>
    /// <param name="fixedUpdateFun"></param>
    public void AddFixedUpdateListener(UnityAction fixedUpdateFun)
    {
        fixedUpdateEvent += fixedUpdateFun;
    }
    /// <summary>
    /// ���lateUpdate֡���º���
    /// </summary>
    /// <param name="lateUpdateFun"></param>
    public void AddLateUpdateListener(UnityAction lateUpdateFun)
    {
        lateUpdateEvent += lateUpdateFun;
    }
    /// <summary>
    /// �Ƴ�Update֡���º���
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }
    /// <summary>
    /// �Ƴ�fixedUpdate֡���º���
    /// </summary>
    /// <param name="fixedUpdateFun"></param>
    public void RemoveFixedUpdateListener(UnityAction fixedUpdateFun)
    {
        fixedUpdateEvent -= fixedUpdateFun;
    }
    /// <summary>
    /// �Ƴ�lateUpdate֡���º���
    /// </summary>
    /// <param name="lateUpdateFun"></param>
    public void RemoveLateUpdateListener(UnityAction lateUpdateFun)
    {
        lateUpdateEvent -= lateUpdateFun;
    }
    private void Update()
    {
        updateEvent?.Invoke();
    }
    private void FixedUpdate()
    {
        fixedUpdateEvent?.Invoke();
    }
    private void LateUpdate()
    {
        lateUpdateEvent?.Invoke();
    }
}

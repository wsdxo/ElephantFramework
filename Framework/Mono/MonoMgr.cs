using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 统一管理帧更新或定时更新
/// </summary>
public class MonoMgr : SingletonAutoMono<MonoMgr>
{
    private event UnityAction updateEvent;
    private event UnityAction fixedUpdateEvent;
    private event UnityAction lateUpdateEvent;

    /// <summary>
    /// 添加Update帧更新函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }
    /// <summary>
    /// 添加fixedUpdate帧更新函数
    /// </summary>
    /// <param name="fixedUpdateFun"></param>
    public void AddFixedUpdateListener(UnityAction fixedUpdateFun)
    {
        fixedUpdateEvent += fixedUpdateFun;
    }
    /// <summary>
    /// 添加lateUpdate帧更新函数
    /// </summary>
    /// <param name="lateUpdateFun"></param>
    public void AddLateUpdateListener(UnityAction lateUpdateFun)
    {
        lateUpdateEvent += lateUpdateFun;
    }
    /// <summary>
    /// 移除Update帧更新函数
    /// </summary>
    /// <param name="updateFun"></param>
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }
    /// <summary>
    /// 移除fixedUpdate帧更新函数
    /// </summary>
    /// <param name="fixedUpdateFun"></param>
    public void RemoveFixedUpdateListener(UnityAction fixedUpdateFun)
    {
        fixedUpdateEvent -= fixedUpdateFun;
    }
    /// <summary>
    /// 移除lateUpdate帧更新函数
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

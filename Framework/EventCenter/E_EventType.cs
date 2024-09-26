using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型枚举
/// 因为事件中心模块使用字符串作为键容易拼错，所以采用枚举，如果之后有需要的直接往这里面加，现有的两个是我自己测试用的
/// </summary>
public enum E_EventType
{
    /// <summary>
    /// 怪物死亡事件
    /// </summary>
    E_Monster_Dead,
    /// <summary>
    /// 玩家获取奖励
    /// </summary>
    E_Player_GetReward,
    E_Test,
}



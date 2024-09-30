using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 输入信息
/// </summary>
public class InputInfo
{
    /// <summary>
    /// 输入类型--键盘or鼠标
    /// </summary>
    public enum E_KeyOrMouse
    {
        /// <summary>
        /// 键盘
        /// </summary>
        Key,
        /// <summary>
        /// 鼠标
        /// </summary>
        Mouse,
    }
    /// <summary>
    /// 抬起、按下、长按
    /// </summary>
    public enum E_InputType
    {
        /// <summary>
        /// 按下
        /// </summary>
        Down,
        /// <summary>
        /// 抬起
        /// </summary>
        Up,
        /// <summary>
        /// 长按
        /// </summary>
        Always,
    }
    public E_KeyOrMouse keyOrMouse;

    public E_InputType inputType;
    //Keycode
    public KeyCode key;
    //MouseID
    public int mouseID;
    /// <summary>
    /// 键盘事件的构造函数
    /// </summary>
    /// <param name="inputType"></param>
    /// <param name="key"></param>
    public InputInfo(E_InputType inputType, KeyCode key)
    {
        this.keyOrMouse=E_KeyOrMouse.Key;
        this.inputType = inputType;
        this.key = key;
    }
    public InputInfo(E_InputType inputType,int mouseID)
    {
        this.keyOrMouse = E_KeyOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
}

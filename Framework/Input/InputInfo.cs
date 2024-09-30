using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������Ϣ
/// </summary>
public class InputInfo
{
    /// <summary>
    /// ��������--����or���
    /// </summary>
    public enum E_KeyOrMouse
    {
        /// <summary>
        /// ����
        /// </summary>
        Key,
        /// <summary>
        /// ���
        /// </summary>
        Mouse,
    }
    /// <summary>
    /// ̧�𡢰��¡�����
    /// </summary>
    public enum E_InputType
    {
        /// <summary>
        /// ����
        /// </summary>
        Down,
        /// <summary>
        /// ̧��
        /// </summary>
        Up,
        /// <summary>
        /// ����
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
    /// �����¼��Ĺ��캯��
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

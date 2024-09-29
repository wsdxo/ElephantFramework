using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyboardInputInfo
{
    public KeyCode key;
    /// <summary>
    /// 0 按下
    /// 1 抬起
    /// 2 按住
    /// </summary>
    public int inputType;
    public KeyboardInputInfo(KeyCode key, int inputType)
    {
        this.key = key;
        this.inputType = inputType;
    }
}
public class MouseInputInfo
{
    public int mouseID;
    /// <summary>
    /// 0 按下
    /// 1 抬起
    /// 2 按住
    /// </summary>
    public int inputType;
    public MouseInputInfo(int mouseID,int inputType)
    {
        this.mouseID = mouseID;
        this.inputType = inputType;
    }
}

public class InputMgr :BaseManager<InputMgr>
{
    private bool isKeyboardStart=false;//是否开启输入系统检测

    private bool isMouseStart=false;
    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }
    public void StartOrCloseInputMgr_Key(bool isStart)
    {
        this.isKeyboardStart=isStart;
    }
    public void StartOrCloseInputMgr_Mouse(bool isStart)
    {
        this.isMouseStart=isStart;
    }
    private void InputUpdate()
    {
        //如果没开启，就不要检测
        if (isKeyboardStart)
        {
            //控制移动
            CheckKeyCode(KeyCode.W);
            CheckKeyCode(KeyCode.A);
            CheckKeyCode(KeyCode.S);
            CheckKeyCode(KeyCode.D);
            //控制攻击
            CheckKeyCode(KeyCode.J);
            CheckKeyCode(KeyCode.K);
            CheckKeyCode(KeyCode.L);
        }
        if (isMouseStart)
        {
            CheckMouse(0);
            CheckMouse(1);
            CheckMouse(2);
        }

        EventCenter.Instance.EventTrigger(E_EventType.E_Horizontal, Input.GetAxis("Horizontal"));

        EventCenter.Instance.EventTrigger(E_EventType.E_Vertical, Input.GetAxis("Vertical"));

    }
    private void CheckKeyCode(KeyCode key)
    {
        if(Input.GetKeyDown(key))
        {
            KeyboardInputInfo info = new KeyboardInputInfo(key,0);
            EventCenter.Instance.EventTrigger(E_EventType.E_Keyboard,info);
        }
        if(Input.GetKeyUp(key))
        {
            KeyboardInputInfo info = new KeyboardInputInfo(key, 1);
            EventCenter.Instance.EventTrigger(E_EventType.E_Keyboard, info);
        }
        if (Input.GetKey(key))
        {
            KeyboardInputInfo info = new KeyboardInputInfo(key, 2);
            EventCenter.Instance.EventTrigger(E_EventType.E_Keyboard,info);
        }
    }
    private void CheckMouse(int mouseID)
    {
        if (Input.GetMouseButtonDown(mouseID))
        {
            MouseInputInfo info = new MouseInputInfo(mouseID, 0);
            EventCenter.Instance.EventTrigger(E_EventType.E_Mouse, info);
        }
        if (Input.GetMouseButtonUp(mouseID))
        {
            MouseInputInfo info = new MouseInputInfo(mouseID, 1);
            EventCenter.Instance.EventTrigger(E_EventType.E_Mouse, info);
        }
        if (Input.GetMouseButton(mouseID))
        {
            MouseInputInfo info = new MouseInputInfo(mouseID, 2);
            EventCenter.Instance.EventTrigger(E_EventType.E_Mouse, info);
        }
    }
}

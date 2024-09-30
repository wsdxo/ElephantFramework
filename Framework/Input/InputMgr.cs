using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class InputMgr :BaseManager<InputMgr>
{
    private Dictionary<E_EventType,InputInfo>inputDic=new Dictionary<E_EventType,InputInfo>();

    private InputInfo nowInputInfo;//当前遍历时取出的输入信息

    private bool isStart=false;//是否开启输入系统检测
    //用于在改键时获取输入信息的委托
    private UnityAction<InputInfo> getInputInfoCallBack;

    private bool isBeginCheckInput=false;//是否开始检测输入信息

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart=isStart;
    }
    /// <summary>
    /// 键盘改键或初始化的方法
    /// </summary>
    /// <param name="key"></param>
    /// <param name="inputType"></param>
    public void ChangeKeyboardInfo(E_EventType eventType,KeyCode key,InputInfo.E_InputType inputType)
    {
        ///不存在事件 初始化
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,key));
        }
        //改键
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].key = key;
            inputDic[eventType].inputType = inputType;
        }
    }
    /// <summary>
    /// 鼠标改键或初始化的方法
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="mouseID"></param>
    /// <param name="inputType"></param>
    public void ChangeMouseInfo(E_EventType eventType,int mouseID,InputInfo.E_InputType inputType)
    {
        if(!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType, new InputInfo(inputType,mouseID));
        }
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Mouse;
            inputDic[eventType].mouseID = mouseID;
            inputDic[eventType].inputType = inputType;
        }
    }
    public void RemoveInputInfo(E_EventType eventType)
    {
        if(inputDic.ContainsKey(eventType))
            inputDic.Remove(eventType);
    }
    public void GetInputInfo(UnityAction<InputInfo> callBack)
    {
        getInputInfoCallBack=callBack;
        MonoMgr.Instance.StartCoroutine(BeginCheckInput());
    }
    private IEnumerator BeginCheckInput()
    {
        yield return 0;
        //等一帧
        isBeginCheckInput = true;
    }
    private void InputUpdate()
    {
        if(isBeginCheckInput)
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;

                //监听所有键位的按下
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                //键盘
                foreach (KeyCode inputKey in keyCodes)
                {
                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                //鼠标
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                //把获取的信息传递给外部
                getInputInfoCallBack.Invoke(inputInfo);
                getInputInfoCallBack = null;
                isBeginCheckInput = false;//检测一次后就停止
            }
        }
        if (!isStart) return;

        foreach(E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //键盘输入
            if (nowInputInfo.keyOrMouse == InputInfo.E_KeyOrMouse.Key)
            {
                switch(nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if(Input.GetKeyDown(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetKeyUp(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetKey(nowInputInfo.key))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                }
            }
            //鼠标输入
            else
            {
                switch (nowInputInfo.inputType)
                {
                    case InputInfo.E_InputType.Down:
                        if(Input.GetMouseButtonDown(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Up:
                        if (Input.GetMouseButtonUp(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                    case InputInfo.E_InputType.Always:
                        if (Input.GetMouseButton(nowInputInfo.mouseID))
                            EventCenter.Instance.EventTrigger(eventType);
                        break;
                }
            }
        }

        EventCenter.Instance.EventTrigger(E_EventType.E_Horizontal, Input.GetAxis("Horizontal"));

        EventCenter.Instance.EventTrigger(E_EventType.E_Vertical, Input.GetAxis("Vertical"));

    }
}

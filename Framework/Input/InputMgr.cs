using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public class InputMgr :BaseManager<InputMgr>
{
    private Dictionary<E_EventType,InputInfo>inputDic=new Dictionary<E_EventType,InputInfo>();

    private InputInfo nowInputInfo;//��ǰ����ʱȡ����������Ϣ

    private bool isStart=false;//�Ƿ�������ϵͳ���
    //�����ڸļ�ʱ��ȡ������Ϣ��ί��
    private UnityAction<InputInfo> getInputInfoCallBack;

    private bool isBeginCheckInput=false;//�Ƿ�ʼ���������Ϣ

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }
    public void StartOrCloseInputMgr(bool isStart)
    {
        this.isStart=isStart;
    }
    /// <summary>
    /// ���̸ļ����ʼ���ķ���
    /// </summary>
    /// <param name="key"></param>
    /// <param name="inputType"></param>
    public void ChangeKeyboardInfo(E_EventType eventType,KeyCode key,InputInfo.E_InputType inputType)
    {
        ///�������¼� ��ʼ��
        if (!inputDic.ContainsKey(eventType))
        {
            inputDic.Add(eventType,new InputInfo(inputType,key));
        }
        //�ļ�
        else
        {
            inputDic[eventType].keyOrMouse = InputInfo.E_KeyOrMouse.Key;
            inputDic[eventType].key = key;
            inputDic[eventType].inputType = inputType;
        }
    }
    /// <summary>
    /// ���ļ����ʼ���ķ���
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
        //��һ֡
        isBeginCheckInput = true;
    }
    private void InputUpdate()
    {
        if(isBeginCheckInput)
        {
            if (Input.anyKeyDown)
            {
                InputInfo inputInfo = null;

                //�������м�λ�İ���
                Array keyCodes = Enum.GetValues(typeof(KeyCode));
                //����
                foreach (KeyCode inputKey in keyCodes)
                {
                    if (Input.GetKeyDown(inputKey))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, inputKey);
                        break;
                    }
                }
                //���
                for (int i = 0; i < 3; i++)
                {
                    if (Input.GetMouseButtonDown(i))
                    {
                        inputInfo = new InputInfo(InputInfo.E_InputType.Down, i);
                        break;
                    }
                }
                //�ѻ�ȡ����Ϣ���ݸ��ⲿ
                getInputInfoCallBack.Invoke(inputInfo);
                getInputInfoCallBack = null;
                isBeginCheckInput = false;//���һ�κ��ֹͣ
            }
        }
        if (!isStart) return;

        foreach(E_EventType eventType in inputDic.Keys)
        {
            nowInputInfo = inputDic[eventType];
            //��������
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
            //�������
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

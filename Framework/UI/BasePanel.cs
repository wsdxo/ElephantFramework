using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// ���ڴ洢�����õ���UI�ؼ�
    /// �����滻ԭ�� ����װ����
    /// </summary>
    protected Dictionary<string,UIBehaviour>controlDic= new Dictionary<string,UIBehaviour>();
    /// <summary>
    /// �ؼ�Ĭ������ ����õ��Ŀؼ����ִ������������ ��ζ�����ǲ���ͨ������ʹ���� ֻ������ʾ����
    /// </summary>
    private List<string>defaultNameList= new List<string>() {"Image", 
                                                         "Text (TMP)", 
                                                         "RawImage" ,
                                                         "Text (Legacy)",
                                                         "Checkmark",
                                                         "Label",
                                                         "Arrow",
                                                         "Placeholder",
                                                         "Fill",
                                                         "Handle",
                                                         "Viewport",
                                                         "Scrollbar Horizontal",
                                                         "Scrollbar Vertical",
                                                         "Background"};
    protected virtual void Awake()
    {
        //Ϊ�˱���һ���������ж�����
        //����Ѱ����Ҫ�����
        FindChildControl<Button>();
        FindChildControl<Toggle>();
        FindChildControl<Slider>();
        FindChildControl<ScrollRect>();
        FindChildControl<InputField>();
        FindChildControl<Dropdown>();
        
        FindChildControl<Text>();
        FindChildControl<Image>();
        FindChildControl<TextMeshProUGUI>();
    }
    /// <summary>
    /// �����ʾʱ���õ��߼�
    /// </summary>
    public abstract void ShowMe();

    /// <summary>
    /// �������ʱ���õ��߼�
    /// </summary>
    public abstract void HideMe();
    public T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            T control = controlDic[name]as T;
            if(control == null)
            {
                Debug.LogError($"��������Ϊ{name}������Ϊ{typeof(T)}�����");
                return null;
            }
            return control;
        }
            Debug.LogError($"��������Ϊ{name}�����");
            return null;
    }
    protected virtual void ClickBtn(string btnName)
    {

    }
    protected virtual void SliderValueChange(string sliderName,float value)
    {

    }
    protected virtual void ToggleValueChange(string toggleName,bool value)
    {

    }
    private void FindChildControl<T>()where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>(true);
        for (int i = 0; i < controls.Length; i++)
        {
            string controlName = controls[i].gameObject.name;
            if (!controlDic.ContainsKey(controlName))
            {
                if(!defaultNameList.Contains(controlName))
                { 
                    controlDic.Add(controlName, controls[i]);

                    if (controls[i] is Button)
                    {
                        (controls[i] as Button).onClick.AddListener(() =>
                        {
                            ClickBtn(controlName);
                        });
                    }
                    else if (controls[i] is Slider)
                    {
                        (controls[i] as Slider).onValueChanged.AddListener((value) =>
                        {
                            SliderValueChange(controlName,value);
                        });
                    }
                    else if (controls[i] is Toggle)
                    {
                        (controls[i] as Toggle).onValueChanged.AddListener((value) =>
                        {
                            ToggleValueChange(controlName,value);
                        });
                    }
                }
            }
        }
    }
}

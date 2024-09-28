using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 用于存储所有用到的UI控件
    /// 里氏替换原则 父类装子类
    /// </summary>
    protected Dictionary<string,UIBehaviour>controlDic= new Dictionary<string,UIBehaviour>();
    /// <summary>
    /// 控件默认名字 如果得到的控件名字存在于这个容器 意味着我们不会通过代码使用它 只是起到显示作用
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
        //为了避免一个对象上有多个组件
        //优先寻找重要的组件
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
    /// 面板显示时调用的逻辑
    /// </summary>
    public abstract void ShowMe();

    /// <summary>
    /// 面板隐藏时调用的逻辑
    /// </summary>
    public abstract void HideMe();
    public T GetControl<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            T control = controlDic[name]as T;
            if(control == null)
            {
                Debug.LogError($"不存在名为{name}，类型为{typeof(T)}的组件");
                return null;
            }
            return control;
        }
            Debug.LogError($"不存在名为{name}的组件");
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

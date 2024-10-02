using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using UnityEngine;
using static UnityEditor.LightingExplorerTableColumn;

public class PlayerPrefsDataMgr
{
    private static PlayerPrefsDataMgr instance;

    public static PlayerPrefsDataMgr Instance
    {
        get
        {
            if (instance == null) instance = new PlayerPrefsDataMgr();
            return instance;
        }
    }

    private PlayerPrefsDataMgr() { }

    public void SaveData(object data,string keyName)
    {
        //获取传入对象的所有字段
        Type dataType=data.GetType();
        FieldInfo[] infos = dataType.GetFields();
        //定义key的规则进行存储
        //keyname_数据类型_字段类型_字段名
        //存储
        string saveKeyName = "";
        FieldInfo info;
        for (int i = 0; i < infos.Length; i++)
        {
            info = infos[i];
            saveKeyName=keyName+"_"+dataType.Name+"_"+info.FieldType+"_"+info.Name;
            SaveValue(info.GetValue(data), saveKeyName);
        }
    }

    private void SaveValue(object value,string keyName)
    {
        Type valueType=value.GetType();
        if(valueType==typeof(int))
        {
            PlayerPrefs.SetInt(keyName, (int)value);
        }
        else if(valueType==typeof(float))
        {
            PlayerPrefs.SetFloat(keyName, (float)value);
        }
        else if(valueType==typeof(string))
        {
            PlayerPrefs.SetString(keyName, value.ToString());
        }
        else if(valueType==typeof(bool))
        {
            PlayerPrefs.SetInt(keyName, (bool)value?1:0);
        }
        //存List
        else if(typeof(IList).IsAssignableFrom(valueType))
        {
            //父类装子类
            IList list = (IList)value;
            //先存长度
            PlayerPrefs.SetInt(keyName , list.Count);
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                SaveValue(list[i], keyName+index++);
            }
        }
        //存dictionary，和List思路一样，先看能否父类装子类
        else if(typeof(IDictionary).IsAssignableFrom(valueType))
        {
            IDictionary dic=value as IDictionary;
            PlayerPrefs.SetInt(keyName, dic.Count);
            int index = 0;
            foreach(object key in dic.Keys)
            {
                SaveValue(key, keyName+"_key_"+index);
                SaveValue(dic[key], keyName+"_value_"+index);
                index++;
            }
        }
        else
        {
            SaveData(value, keyName);
        }
    }
    public object LoadData(Type type,string keyName)
    {
        //根据传入的类型创建一个对象
        object obj=Activator.CreateInstance(type);
        FieldInfo[] infos = type.GetFields();
        string loadKeyName = "";
        FieldInfo info;
        for(int i = 0;i<infos.Length;i++)
        {
            info = infos[i];
            loadKeyName= keyName + "_" + type.Name + "_" + info.FieldType + "_" + info.Name;

            info.SetValue(obj,LoadValue(info.FieldType,loadKeyName));

        }
        return obj;
    }
    //得到单个数据的方法
    private object LoadValue(Type fieldType, string keyName)
    {
        if (fieldType == typeof(int))
        {
            return PlayerPrefs.GetInt(keyName,0);
        }
        else if (fieldType == typeof(float))
        {
            return PlayerPrefs.GetFloat(keyName,0);
        }
        else if (fieldType == typeof(string))
        {
            return PlayerPrefs.GetString(keyName,"");
        }
        else if (fieldType == typeof(bool))
        {
            return PlayerPrefs.GetInt(keyName,0)==1?true:false;
        }
        else if(typeof(IList).IsAssignableFrom(fieldType))
        {
            int count=PlayerPrefs.GetInt(keyName,0);
            IList list= Activator.CreateInstance(fieldType)as IList;
            for(int i=0;i<count;i++)
            {
                list.Add(LoadValue(fieldType.GetGenericArguments()[0],keyName+i));
            }
            return list;
        }
        else if (typeof(IDictionary).IsAssignableFrom(fieldType))
        {
            int count=PlayerPrefs.GetInt(keyName,0);
            IDictionary dic= Activator.CreateInstance(fieldType) as IDictionary;
            for(int i=0;i<count; i++)
            {
                dic.Add(LoadValue(fieldType.GetGenericArguments()[0], keyName + "_key_" + i), LoadValue(fieldType.GetGenericArguments()[1],keyName+"_value_"+i));
            }
            return dic;
        }
        else
        {
            return LoadData(fieldType, keyName);
        }
    }
}

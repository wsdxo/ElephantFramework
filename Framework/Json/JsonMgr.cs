using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum JsonType
{
    JsonUtility,
    LitJson,
}
public class JsonMgr
{
    private static JsonMgr instance=new JsonMgr();
    private JsonMgr() { }

    public static JsonMgr Instance
    {
        get
        {
            return instance;
        }
    }
    //´æ
    public void SaveData(object data,string fileName,JsonType type=JsonType.LitJson)
    {
        string jsonStr = "";
        switch(type)
        {
            case JsonType.JsonUtility:
                jsonStr= JsonUtility.ToJson(data);
                break;
            case JsonType.LitJson:
                jsonStr=JsonMapper.ToJson(data);
                break;
        }
        File.WriteAllText(Application.persistentDataPath+"/"+fileName+".json", jsonStr);
    }
    //È¡
    public T LoadData<T>(string fileName,JsonType type=JsonType.LitJson)where T:new()
    {
        string path=Application.streamingAssetsPath+"/"+fileName+".json";
        if(!File.Exists(path))path=Application.persistentDataPath + "/" + fileName + ".json";
        if(!File.Exists(path))return new T();
        T data=default(T);
        string jsonStr=File.ReadAllText(path);
        switch (type)
        {
            case JsonType.JsonUtility:
                data=JsonUtility.FromJson<T>(jsonStr);
                break;
            case JsonType.LitJson:
                data=JsonMapper.ToObject<T>(jsonStr);
                break;
        }
        return data;
    }
}

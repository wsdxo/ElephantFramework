using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���루�����е����ݣ�����
/// </summary>
public class PoolData
{
    //�����洢�����еĶ��� ��¼����û��ʹ�õĶ���
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    //������¼ʹ���еĶ���� 
    private List<GameObject> usedList = new List<GameObject>();


    //��������� �������в��ֹ���Ķ���
    private GameObject rootObj;

    //��ȡ�������Ƿ��ж���
    public int Count => dataStack.Count;

    //��ȡ�Ƿ�������ʹ�õĶ���
    public int UsedCount => usedList.Count;

    private int maxNum;

    public bool NeedCreate => UsedCount < maxNum;

    /// <summary>
    /// ��ʼ�����캯��
    /// </summary>
    /// <param name="root">���ӣ�����أ�������</param>
    /// <param name="name">���븸���������</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        //��������ʱ �Żᶯ̬���� �������ӹ�ϵ
        if (PoolMgr.isOpenLayout)
        {
            //�������븸����
            rootObj = new GameObject(name);
            //�͹��Ӹ����������ӹ�ϵ
            rootObj.transform.SetParent(root.transform);
        }

        //��������ʱ �ⲿ�϶��ǻᶯ̬����һ�������
        //����Ӧ�ý����¼�� ʹ���еĶ���������
        PushUsedList(usedObj);
        PoolObj poolObj = usedObj.GetComponent<PoolObj>();
        if (poolObj == null)
        {
            Debug.LogError("��Ϊ��Ҫʹ�û���ع��ܵĶ������PoolObj�ű�");
            return;
        }
        this.maxNum=poolObj.maxNum;
    }

    /// <summary>
    /// �ӳ����е������ݶ���
    /// </summary>
    /// <returns>��Ҫ�Ķ�������</returns>
    public GameObject Pop()
    {
        //ȡ������
        GameObject obj;

        if (Count > 0)
        {
            //��û�е���������ȡ��ʹ��
            obj = dataStack.Pop();
            //����Ҫʹ���� Ӧ��Ҫ��ʹ���е�������¼��
            usedList.Add(obj);
        }
        else
        {
            //ȡ0�����Ķ��� ����ľ���ʹ��ʱ����Ķ���
            obj = usedList[0];
            //���Ұ�����ʹ���ŵĶ������Ƴ�
            usedList.RemoveAt(0);
            //��������Ҫ�ó�ȥ�ã���������Ӧ�ð����ּ�¼�� ʹ���е�������ȥ 
            //������ӵ�β�� ��ʾ �Ƚ��µĿ�ʼ
            usedList.Add(obj);
        }

        //�������
        obj.SetActive(true);
        //�Ͽ����ӹ�ϵ
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(null);

        return obj;
    }

    /// <summary>
    /// ��������뵽���������
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        //ʧ��������Ķ���
        obj.SetActive(false);
        //�����Ӧ����ĸ������� �������ӹ�ϵ
        if (PoolMgr.isOpenLayout)
            obj.transform.SetParent(rootObj.transform);
        //ͨ��ջ��¼��Ӧ�Ķ�������
        dataStack.Push(obj);
        //��������Ѿ�����ʹ���� Ӧ�ð����Ӽ�¼�������Ƴ�
        usedList.Remove(obj);
    }


    /// <summary>
    /// ������ѹ�뵽ʹ���е������м�¼
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}

/// <summary>
/// �����(�����)ģ�� ������
/// </summary>
public class PoolMgr : BaseManager<PoolMgr>
{
    //�������������г��������
    //ֵ ��ʵ����ľ���һ�� �������
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    //�Ƿ������ֹ���
    public static bool isOpenLayout = true;

    //���Ӹ�����
    private GameObject poolObj;

    private PoolMgr() { }

    /// <summary>
    /// �ö����ķ���
    /// </summary>
    /// <param name="name">��������������</param>
    /// <returns>�ӻ������ȡ���Ķ���</returns>
    public GameObject GetObj(string name)
    {
        if (isOpenLayout&&poolObj==null)
        {
            poolObj = new GameObject("Pool");
        }
        GameObject obj;
        
        #region �������������޺���߼��ж�
        if (!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            //��̬��������
            //û�е�ʱ�� ͨ����Դ���� ȥʵ������һ��GameObject
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //����ʵ���������Ķ��� Ĭ�ϻ������ֺ����һ��(Clone)
            //�������������� �����������
            obj.name = name;

            //��������
            if (!poolDic.ContainsKey(name))
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            else//ʵ���������Ķ��� ��Ҫ��¼��ʹ���еĶ���������
                poolDic[name].PushUsedList(obj);
        }
        //���������ж��� ���� ʹ���еĶ��������� ֱ��ȥȡ������
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion


        #region û�м��� ����ʱ���߼�
        ////�г��� ���� ������ �ж��� ��ȥֱ����
        //if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        //{
        //    //����ջ�еĶ��� ֱ�ӷ��ظ��ⲿʹ��
        //    obj = poolDic[name].Pop();
        //}
        ////���򣬾�Ӧ��ȥ����
        //else
        //{
        //    //û�е�ʱ�� ͨ����Դ���� ȥʵ������һ��GameObject
        //    obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
        //    //����ʵ���������Ķ��� Ĭ�ϻ������ֺ����һ��(Clone)
        //    //�������������� �����������
        //    obj.name = name;
        //}
        #endregion
        return obj;
    }


    /// <summary>
    /// ��������з������
    /// </summary>
    /// <param name="name">���루���󣩵�����</param>
    /// <param name="obj">ϣ������Ķ���</param>
    public void PushObj(GameObject obj)
    {
        #region ��Ϊʧ�� ���ӹ�ϵ�������� ��������д��� ���Բ���Ҫ�ٴ�����Щ������
        ////��֮��Ŀ�ľ���Ҫ�Ѷ�����������
        ////������ֱ���Ƴ����� ���ǽ�����ʧ�� һ������� �õ�ʱ���ټ�����
        ////�������ַ�ʽ�������԰Ѷ���ŵ���Ļ�⿴�����ĵط�
        //obj.SetActive(false);

        ////��ʧ��Ķ���Ҫ��������еĶ��� ������������Ϊ ���ӣ�����أ�������
        //obj.transform.SetParent(poolObj.transform);
        #endregion

        //û�г��� ��������
        //if (!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name, new PoolData(poolObj, obj.name));

        //�����뵱�зŶ���
        poolDic[obj.name].Push(obj);
        #region
        ////������ڶ�Ӧ�ĳ������� ֱ�ӷ�
        //if(poolDic.ContainsKey(name))
        //{
        //    //��ջ�����룩�з������
        //    poolDic[name].Push(obj);
        //}
        ////���� ��Ҫ�ȴ������� �ٷ�
        //else
        //{
        //    //�ȴ�������
        //    poolDic.Add(name, new Stack<GameObject>());
        //    //�������������
        //    poolDic[name].Push(obj);
        //}
        #endregion
    }

    /// <summary>
    /// ��������������ӵ��е����� 
    /// ʹ�ó��� ��Ҫ�� �г���ʱ
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
    }
}

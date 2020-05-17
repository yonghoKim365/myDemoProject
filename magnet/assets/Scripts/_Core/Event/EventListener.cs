using UnityEngine;
using System.Collections.Generic;

public class EventListner : Singleton<EventListner>
{
    #region				::	Member Variable - Event Map	::
    Dictionary<string, System.Action> EventMap_ = new Dictionary<string, System.Action>();
    Dictionary<string, System.Action<object>> EventMap1_ = new Dictionary<string, System.Action<object>>();
    Dictionary<string, System.Action<object, object>> EventMap2_ = new Dictionary<string, System.Action<object, object>>();
    #endregion

    #region				::	Public Function	::

    #region				::	Public Function - void Listner ::


    /// <summary>
    /// ��� �̺�Ʈ ���������͸� ���� �ϰ� �����̳ʸ� ����.
    /// </summary>
    public void Reset()
    {
        if (EventMap_.Count > 0)
        {
            foreach (KeyValuePair<string, System.Action> value in EventMap_)
            {
                System.Action action = value.Value as System.Action;
                System.Delegate.RemoveAll(action, action);
            }
        }
    }

    /// <summary>
    /// �̺�Ʈ�� �߻�.
    /// </summary>
    /// <param name="eventName">�߻� ��ų �̺�Ʈ��. ��ϵ� �̺�Ʈ ���� Ű������ ���ȴ�.</param>
    public void TriggerEvent(string eventName)
    {
        if (EventMap_.ContainsKey(eventName) && null != EventMap_[eventName])
            EventMap_[eventName]();
    }

    /// <summary>
    /// �̺�Ʈ �����ʸ� ����Ѵ�.
    /// �Լ����� �߻� �̺�Ʈ�� ���ڿ��� �����ϰ� ����Ѵ�.
    /// </summary>
    /// <param name="callback">void�� �ݹ� �Լ�.</param>
    public void RegisterListner(System.Action callback)
    {
        // ���ڿ��� Ű������ ��������� ���ڿ��� �ݹ��Լ��� ���� ������� �ʵ��� �Ѵ�.
        // �ݹ��Լ� �� ��ü�� �߻��� �̺�Ʈ�̴�.
        RegisterListner(callback.Method.Name, callback);
    }

    void RegisterListner(string eventName, System.Action callback)
    {        
        if (!EventMap_.ContainsKey(eventName))
            EventMap_.Add(eventName, callback);
        else EventMap_[eventName] += callback;
    }

    /// <summary>
    /// �̺�Ʈ ������ ��� ����.
    /// </summary>
    /// <param name="callback">������ �ݹ� �Լ�.</param>
    public void RemoveEvent(System.Action callback)
    {
        RemoveEvent(callback.Method.Name, callback);
    }

    void RemoveEvent(string eventName, System.Action callback)
    {
        if (EventMap_.ContainsKey(eventName) && null != EventMap_[eventName])
        {
            EventMap_[eventName] -= callback;
            if (null == EventMap_[eventName])
                EventMap_.Remove(eventName);
        }
    }
    #endregion

    #region				::	Public Function - one object Listner ::

    /// <summary>
    /// �̺�Ʈ�� �߻�.
    /// </summary>
    /// <param name="eventName">�߻� ��ų �̺�Ʈ��. ��ϵ� �̺�Ʈ ���� Ű������ ���ȴ�.</param>
    /// <param name="obj">�Ѱ��� ����Ÿ. ��� ��ü�� ������ �ִ�. �� �޴� �ʿ��� ĳ�����Ͽ� ����ؾ���.</param>
    public void TriggerEvent(string eventName, object obj)
    {       
        TriggerEvent<object>(EventMap1_, eventName, obj);
    }

    /// <summary>
    /// �̺�Ʈ �����ʸ� ����Ѵ�.
    /// �Լ����� �߻� �̺�Ʈ�� ���ڿ��� �����ϰ� ����Ѵ�.
    /// </summary>
    /// <param name="callback">object�� �ݹ� �Լ�. ��� ��ü�� �ü� �ִ�. ĳ���� �Ͽ� ����ؾ� �Ѵ�.</param>
    public void RegisterListner(System.Action<object> callback)
    {        
        RegisterListner(callback.Method.Name, callback);
    }

    public void RegisterListner(string eventName, System.Action<object> callback)
    {
        RemoveEvent(eventName, callback);
        RegisterListner<object>(EventMap1_, eventName, callback);
    }

    public void RemoveEvent(System.Action<object> callback)
    {
        RemoveEvent(callback.Method.Name, callback);
    }

    public void RemoveEvent(string eventName, System.Action<object> callback)
    {
        RemoveEvent<object>(EventMap1_, eventName, callback);
    }
    #endregion

    #region				::	Public Function - two object Listner ::
    public void TriggerEvent(string eventName, object obj1, object obj2)
    {
        TriggerEvent<object, object>(EventMap2_, eventName, obj1, obj2);
    }

    public void RegisterListner(System.Action<object, object> callback)
    {
        RegisterListner(callback.Method.Name, callback);
    }

    public void RegisterListner(string eventName, System.Action<object, object> callback)
    {
        RemoveEvent(eventName, callback);
        RegisterListner<object, object>(EventMap2_, eventName, callback);
    }

    public void RemoveEvent(System.Action<object, object> callback)
    {
        RemoveEvent(callback.Method.Name, callback);
    }

    public void RemoveEvent(string eventName, System.Action<object, object> callback)
    {
        RemoveEvent<object, object>(EventMap2_, eventName, callback);
    }
    #endregion

    #endregion

    #region				::	Private Templete Function	::

    private void TriggerEvent<T>(Dictionary<string, System.Action<T>> eventMap, string eventName, T obj)
    {
        if (eventMap.ContainsKey(eventName) && null != eventMap[eventName])
        {            
            eventMap[eventName](obj);
        }
    }

    private void RegisterListner<T>(Dictionary<string, System.Action<T>> eventMap, string key, System.Action<T> callback)
    {
        if (!eventMap.ContainsKey(key))
            eventMap.Add(key, callback);
        else eventMap[key] += callback;
    }

    private void RemoveEvent<T>(Dictionary<string, System.Action<T>> eventMap, string eventName, System.Action<T> callback)
    {
        if (eventMap.ContainsKey(eventName) && null != eventMap[eventName])
        {
            eventMap[eventName] -= callback;
            if (null == eventMap[eventName])
                eventMap.Remove(eventName);
        }
    }

    private void TriggerEvent<T1, T2>(Dictionary<string, System.Action<T1, T2>> eventMap, string eventName, T1 obj1, T2 obj2)
    {
        if (eventMap.ContainsKey(eventName) && null != eventMap[eventName])
            eventMap[eventName](obj1, obj2);
    }

    private void RegisterListner<T1, T2>(Dictionary<string, System.Action<T1, T2>> eventMap, string key, System.Action<T1, T2> callback)
    {
        if (!eventMap.ContainsKey(key))
            eventMap.Add(key, callback);
        else eventMap[key] += callback;
    }

    private void RemoveEvent<T1, T2>(Dictionary<string, System.Action<T1, T2>> eventMap, string eventName, System.Action<T1, T2> callback)
    {
        if (eventMap.ContainsKey(eventName) && null != eventMap[eventName])
        {
            eventMap[eventName] -= callback;
            if (null == eventMap[eventName])
                eventMap.Remove(eventName);
        }
    }

    #endregion
}

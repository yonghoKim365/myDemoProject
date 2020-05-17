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
    /// 모든 이벤트 딜리게이터를 삭제 하고 컨테이너를 비운다.
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
    /// 이벤트를 발생.
    /// </summary>
    /// <param name="eventName">발생 시킬 이벤트명. 등록된 이벤트 맵의 키값으로 사용된다.</param>
    public void TriggerEvent(string eventName)
    {
        if (EventMap_.ContainsKey(eventName) && null != EventMap_[eventName])
            EventMap_[eventName]();
    }

    /// <summary>
    /// 이벤트 리스너를 등록한다.
    /// 함수명은 발생 이벤트의 문자열과 동일하게 사용한다.
    /// </summary>
    /// <param name="callback">void형 콜백 함수.</param>
    public void RegisterListner(System.Action callback)
    {
        // 문자열을 키값으로 등록하지만 문자열과 콜백함수를 따로 사용하지 않도록 한다.
        // 콜백함수 명 자체가 발생될 이벤트이다.
        RegisterListner(callback.Method.Name, callback);
    }

    void RegisterListner(string eventName, System.Action callback)
    {        
        if (!EventMap_.ContainsKey(eventName))
            EventMap_.Add(eventName, callback);
        else EventMap_[eventName] += callback;
    }

    /// <summary>
    /// 이벤트 리스너 등록 해제.
    /// </summary>
    /// <param name="callback">해제할 콜백 함수.</param>
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
    /// 이벤트를 발생.
    /// </summary>
    /// <param name="eventName">발생 시킬 이벤트명. 등록된 이벤트 맵의 키값으로 사용된다.</param>
    /// <param name="obj">넘겨줄 데이타. 모든 객체를 넣을수 있다. 단 받는 쪽에서 캐스팅하여 사용해야함.</param>
    public void TriggerEvent(string eventName, object obj)
    {       
        TriggerEvent<object>(EventMap1_, eventName, obj);
    }

    /// <summary>
    /// 이벤트 리스너를 등록한다.
    /// 함수명은 발생 이벤트의 문자열과 동일하게 사용한다.
    /// </summary>
    /// <param name="callback">object형 콜백 함수. 모든 객체가 올수 있다. 캐스팅 하여 사용해야 한다.</param>
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

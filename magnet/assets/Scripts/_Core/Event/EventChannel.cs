using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Inspector에 표시되기 위해 public 설정
/// </remarks>
[System.Serializable]
public class EventChannel
{
    public bool     isSendable;
    public int      ID;
    public string   Name;

    public int HandlerCount
    {
        get
        {
            handlerCount = handlerList.Count;
            return handlerCount;
        }
    }

    [SerializeField]
    private int                 handlerCount;
    private List<IEventHandler> handlerList;

    /// <summary>
    /// 채널 활성화 여부
    /// </summary>
    public bool IsEnabled { 
        get { return isSendable; }
        set { isSendable = value; }
    }

    public EventChannel()
    {
        Init();
    }

    public EventChannel(int id, string channelName)
    {
        Init();

        ID = id;
        Name = channelName;
    }

    void Init()
    {
        IsEnabled = true;
        ID = 0;
        Name = "EventChannel";

        handlerList = new List<IEventHandler>();
    }

    public void Reset()
    {
        handlerList.Clear();
        handlerCount = HandlerCount;
    }

    public bool Contains(IEventHandler handler)
    {
        return handlerList.Contains(handler);
    }

    public IEventHandler AddHandler(IEventHandler handler)
    {
        handlerList.Add(handler);
        handlerCount = HandlerCount;
        handler.IsEnabled = true;

        return handler;
    }

    public void RemoveHandler(IEventHandler handler)
    {
        handlerList.Remove(handler);
        handlerCount = HandlerCount;
    }

    #region -------- SEND EVENT --------

    /// <summary>
    /// 대상에게 이벤트 전송
    /// </summary>
    /// <param name="to">대상 이벤트 핸들러</param>
    /// <param name="evt">전송할 이벤트</param>
    public void Send(IEventHandler to, IBaseEvent evt)
    {
        if (to.IsEnabled)
            to.OnEvent(evt);
    }

    /// <summary>
    /// 모든 이벤트 핸들러에 전송
    /// </summary>
    /// <param name="evt">전송할 이벤트</param>
    public void Broadcast(IBaseEvent evt)
    {
        foreach (IEventHandler handler in handlerList)
        {
            if (handler.IsEnabled)
                handler.OnEvent(evt);
        }
    }

    #endregion
}
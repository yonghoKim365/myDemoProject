using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 범용적인 이벤트 서비스 클래스
/// </summary>
/// <remarks>
/// EventService는 방송국이고,
/// EventChannel을 방송국이 송신해야할 채널들이라고 보면된다.
/// </remarks>
public class EventService : Immortal<EventService>
{
    public bool DebugMode;

    [SerializeField]
    private List<EventChannel> channelList;

    public int ChannelCount { get { return channelList.Count; } }

    public void ReInit()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        if (null != channelList)
            channelList.Clear();

        channelList = new List<EventChannel>();
    }
    
    /// <summary>
    /// 서비스 중인 정보들을 모두 제거한다.
    /// </summary>
    public void Reset()
    {
        channelList.RemoveAll( (channel) => {
                if (channel != null)
                {
                    channel.Reset();
                    return true;
                }
                return false;
            }
        );
    }

    /// <summary>
    /// 채널을 하나 등록한다.
    /// </summary>
    /// <returns>생성된 채널</returns>
    public EventChannel CreateChannel()
    {
        EventChannel channel = new EventChannel();
        channelList.Add(channel);

        if (DebugMode)
        {
            Debug.Log("EventService.CreateChannel : " + channel.ID + " . " + channel.Name + " :: ChannelCount : " + ChannelCount);
        }

        return channel;
    }

    /// <summary>
    /// 채널 이름을 정해서 등록한다.
    /// </summary>
    /// <param name="channelName"></param>
    /// <returns>생성된 채널</returns>
    public EventChannel CreateChannel(string channelName)
    {
        EventChannel channel = CreateChannel();
        channel.ID = channelList.Count;
        channel.Name = channelName;

        return channel;
    }

    /// <summary>
    /// 채널 제거
    /// </summary>
    /// <param name="channel">제거할 채널</param>
    public void RemoveChannel(EventChannel channel)
    {
        channelList.Remove(channel);
    }

    /// <summary>
    /// 이벤트 핸들러를 사용중인 모든 채널에서 제거한다.
    /// </summary>
    /// <param name="handler">제거할 이벤트 핸들러</param>
    public void RemoveHandler(IEventHandler handler)
    {
        foreach (EventChannel channel in channelList)
        {
            channel.RemoveHandler(handler);
        }
    }

    #region -------- SEND EVENT --------

    /// <summary>
    /// 해당 채널에 이벤트를 전송한다.
    /// </summary>
    /// <param name="to">대상 채널</param>
    /// <param name="evt">전송할 이벤트</param>
    public void Send(EventChannel to, IBaseEvent evt)
    {
        if (DebugMode)
        {
            Debug.Log("EventService.Send : " + evt);
        }

        if (null != to && to.isSendable)
            to.Broadcast(evt);
    }

    /// <summary>
    /// 주어진 이름의 채널에 이벤트를 전송한다.
    /// </summary>
    /// <param name="channelName">대상 채널 이름</param>
    /// <param name="evt">전송할 이벤트</param>
    public void Send(string channelName, IBaseEvent evt)
    {
        channelList.ForEach( (channel) =>
            {
                if (channel.Name == channelName)
                    Send(channel, evt);
            }
        );
    }

    /// <summary>
    /// 모든 채널에 이벤트를 전송한다.
    /// </summary>
    /// <param name="evt">전송할 이벤트</param>
    public void Broadcast(IBaseEvent evt)
    {
        if (DebugMode)
        {
            Debug.Log("EventService.Broadcast : " + evt);
        }

        foreach (EventChannel channel in channelList)
        {
            // 함수 호출 오버로드
            //Send(channel, evt); 
			
            if (null != channel && channel.isSendable)
                channel.Broadcast(evt);
        }
    }

    #endregion
}
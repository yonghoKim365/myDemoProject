using UnityEngine;
using System.Collections;

/// <summary>
/// EventHandler에서 보내질 Event들의 interface.
/// </summary>
public interface IBaseEvent
{
    int eventID { get; set; }
}

/** Example
public class DamageEvent : IBaseEvent 
{
    public int eventID { get; set; }

    public DamageEvent(int _evtId)
    {
        //base(_evtId);
        eventID = _evtId;
    }
}
*/
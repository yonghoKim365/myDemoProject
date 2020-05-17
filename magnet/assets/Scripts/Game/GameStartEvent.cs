using UnityEngine;
using System.Collections;

public class GameStartEvent : MonoBehaviour {

    public bool Live = false;
    void OnTriggerEnter(Collider col)
    {
        if (Live)
            return;

        //< 유닛이 아니면 패스한다
        if (col.gameObject.GetComponent<Unit>() == null)
            return;

        Live = true;
        EventListner.instance.TriggerEvent("InGameStart", true);
    }
}

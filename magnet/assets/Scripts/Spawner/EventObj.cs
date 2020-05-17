using UnityEngine;

/// <summary> 단순 이벤트 스크립트
/// 사용중인 곳은 튜토리얼의 화살표 객체의 끄는 용도. </summary>
public class EventObj : MonoBehaviour {

    public GameObject EventTarget;
    public EventObj NextEvent;

    public System.Action Callback;

    private bool IsEnter = false;
    public void OnTriggerEnter(Collider other)
    {
        if (IsEnter || Callback == null || other == null )
            return;

        IsEnter = true;

        if (EventTarget != null && other.gameObject != null && EventTarget == other.gameObject)
        {
            Callback();

            if (NextEvent != null)
                NextEvent.Show();
        }

        Hide();
    }

    public void OnTriggerExit(Collider other)
    {
        IsEnter = false;
    }

    public void OnTriggerStay(Collider other)
    {

    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

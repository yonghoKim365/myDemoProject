using UnityEngine;
using System.Collections;
using WellFired;

public class CutSceneTest : MonoBehaviour {

    public CutSceneEventData[] testlist;

	// Use this for initialization
	void Start () {

        //StartCoroutine(CutSceneForStart());

        Quaternion qua = Quaternion.Euler(new Vector3(11, -90f, 0));
        Debug.Log(qua.x + " , " + qua.y + " , " + qua.z + " , " + qua.w);
	}
	
    void PlayAni(string str)
    {
        Debug.Log("PlayAni " + str);
    }

    IEnumerator CutSceneForStart()
    {
        GameObject obj = GameObject.Find("Sequence");

        USSequencer seq = obj.GetComponent<USSequencer>();
        if (null == seq)
            yield break;

        //Transform timelineTrans = seq.transform.FindChild("Timelines for StartCameraRoot");
        //USTimelineContainer timeline = timelineTrans.GetComponent<USTimelineContainer>();
        //timeline.AffectedObject = startCamObj.transform;

        //USTimelineEvent evt = timeline.GetComponentInChildren<USTimelineEvent>();
        //USMatchObjectEvent matchEvt = evt.GetComponentInChildren<USMatchObjectEvent>();
       // matchEvt.objectToMatch = CameraManager.instance.mainCamera.gameObject;

        seq.PlaybackStarted = (sequence) =>
        {
        };
        seq.PlaybackFinished = (sequence) =>
        {
        };
        seq.Play();
    }
}

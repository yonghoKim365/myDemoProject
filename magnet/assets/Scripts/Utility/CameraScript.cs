using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	// 카메라가 보고 있을 타겟 
	public Transform target;
	public Quaternion   originalRot;
	public Quaternion   targetRot;
	public AnimationCurve rotcurve;
	GameObject targetobj;
	float time;
	public float inittime;
	// Path를 따라가는 Animation
	void Start () {

	}
	public void InitCamSet(GameObject target ,Quaternion _targetRot , float _time)
	{
		targetobj = target;
		time = _time;
		targetRot = _targetRot;
		originalRot = gameObject.transform.rotation;
        iTween.MoveTo(gameObject, iTween.Hash("path", iTweenPath.GetPath("InitCamPath")
                                                , "time", inittime
                                                , "easetype", iTween.EaseType.linear
                                                , "looktarget", gameObject
                                                , "looktime", 0f
                                                ));
        StartCoroutine(StartCamPath(inittime));

        SetCam();
	}
	IEnumerator StartCamPath(float _time)
	{
		yield return new WaitForSeconds(_time);
		SetCam();
	}
	public void SetCam()
	{
		//float time = _time;
		iTween.MoveTo ( gameObject, iTween.Hash( "path", iTweenPath.GetPath("CamPath") 
		                                        , "time", time
		                                        , "easetype", iTween.EaseType.linear
		                                        , "looktarget", targetobj
		                                        , "looktime", 0f
		                                        ));
		StartCoroutine(TransitionRoutine(time));
	}
	IEnumerator TransitionRoutine(float transitionDuration)
	{
		float time = 0f;
		float t = 0f;
		
		while (t <= 1f)
		{
			time += Time.deltaTime;
			t = Mathf.Clamp01(time / transitionDuration);

			transform.rotation = Quaternion.Lerp(originalRot, targetRot, rotcurve.Evaluate(t));
			
			yield return null;

			iTweenPath[] paths = gameObject.GetComponentsInChildren<iTweenPath>();
			for(int i =0; i<paths.Length; i++)
				Destroy(paths[i]);
			Destroy(gameObject.GetComponent<CameraScript>());
		}
	}
}
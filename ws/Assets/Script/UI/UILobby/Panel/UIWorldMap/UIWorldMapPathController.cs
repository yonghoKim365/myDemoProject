using UnityEngine;
using System.Collections;

public class UIWorldMapPathController : MonoBehaviour {

	public string pathName;

	public float time = 2.0f;

	public iTween.LoopType loopType = iTween.LoopType.loop;

	public iTween.EaseType easingType = iTween.EaseType.easeInOutQuad;

	public bool lookPath = false;

	public void play()
	{
		Vector3[] pathVectors = iTweenPath.GetPath(pathName);

		gameObject.transform.position = pathVectors[0];

		iTween.MoveTo( 
		              gameObject, 
		              iTween.Hash( "time", time, "path", pathVectors, "easetype", easingType, "looptype", loopType, "oncomplete","onCompleteMotion", "orienttopath",lookPath)
		              );
	}

	void onCompleteMotion()
	{
//		Debug.Log("complete!");
	}

	void Start () 
	{
		play();
	}
	
}

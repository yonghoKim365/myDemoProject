using UnityEngine;
using System.Collections;

public class SkyBoxHelper : MonoBehaviour {

	Transform _targetCam = null;
	Transform _tf;

	// Use this for initialization
	void Start () {

		if(_targetCam == null)
		{
			_targetCam = GameManager.me.gameCamera.transform;
		}

		if(_tf == null) _tf = transform;
	}

	void OnEnable()
	{
		if(_targetCam == null)
		{
			_targetCam = GameManager.me.gameCamera.transform;
		}
		
		if(_tf == null) _tf = transform;
	}


	Vector3 _v;

	// Update is called once per frame
	void Update () 
	{
		if(_targetCam != null && _tf != null)
		{
			_v = _tf.transform.position;
			_v.x = _targetCam.transform.position.x;
			_tf.transform.position = _v;
		}
	}

	void OnDestroy()
	{
		_targetCam = null;
		_tf = null;
	}

}

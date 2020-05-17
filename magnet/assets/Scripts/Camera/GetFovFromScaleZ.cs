using UnityEngine;
using System.Collections;

public class GetFovFromScaleZ : MonoBehaviour {
	public Camera _camera;
	// Use this for initialization
	void Start () {
		_camera = GetComponentInChildren<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate(){
		_camera.fieldOfView = this.transform.localScale.z;
		//Debug.Log (this.transform.localScale.z);
	}
}

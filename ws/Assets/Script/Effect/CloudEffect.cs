using UnityEngine;
using System.Collections;

sealed public class CloudEffect : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		// 952 ~ -1066
	}
	
	private Vector3 _v;
	
	// Update is called once per frame
	void Update () 
	{
		_v = transform.localPosition;
		
		if(_v.x < -1066.0f) _v.x = 952.0f;
		
		_v.x -= Time.smoothDeltaTime * 20.0f;
		
		transform.localPosition = _v;
		
	}
}

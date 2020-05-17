using UnityEngine;
using System.Collections;

sealed public class SimpleRotater : MonoBehaviour 
{
	public float speed = 50.0f;
	
	// Use this for initialization
	void Start () {
	
	}

	public enum Type
	{
		X, Y, Z
	}

	public Type type = Type.Z;

	private Quaternion _q;
	private Vector3 _v;
	
	// Update is called once per frame
	void Update () {

		if(GameManager.me == null || GameManager.me.isPaused) return;
		
		_q = transform.rotation;
		_v = _q.eulerAngles;

		switch(type)
		{
		case Type.Z:
			_v.z -= speed * GameManager.globalDeltaTime;
			break;
		case Type.X:
			_v.x -= speed * GameManager.globalDeltaTime;
			break;
		case Type.Y:
			_v.y -= speed * GameManager.globalDeltaTime;
			break;
		}


		_q.eulerAngles = _v;
		transform.rotation = _q;			
	}
}

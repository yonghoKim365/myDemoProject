using UnityEngine;
using System.Collections;

public class UIHandHeldEffect : MonoBehaviour {

	public Transform tf;

	public float speed = 0.0f;

	public int angle = 0;

	private Vector3 _v;

	public float limitX = 10.0f;
	public float limitY = 10.0f;

	private float _speed = 0.0f;

	public void start(Vector3 value)
	{
		angle = UnityEngine.Random.Range(0,360);
		enabled = true;

		limitX = value.x;
		limitY = value.y;

		speed = value.z;
		_speed = speed;
	}


	public void reset()
	{
		if(tf != null)
		{
			_v = tf.transform.localPosition;
			_v.x = 0.0f; _v.y = 0.0f; _v.z = 0.0f;
			tf.transform.localPosition = _v;
		}
	}

	void OnDisable()
	{
		reset();
	}

	Vector3 _v2;
	void Update()
	{
		if(tf != null)
		{
			_v = tf.transform.localPosition;
			_v2 = Util.getPositionByAngleAndDistance(angle, speed * Time.smoothDeltaTime);
			_v.x += _v2.x;
			_v.y += _v2.y;
			_v.z += _v2.x;

			bool resetAngle = false;

			if(_v.y > limitY)
			{
				_v.y = limitY;
				resetAngle = true;
			}
			else if(_v.y < -limitY)
			{
				_v.y = -limitY;
				resetAngle = true;
			}

			if(_v.x > limitX)
			{
				_v.x = limitX;
				
				resetAngle = true;
			}
			else if(_v.x < -limitX)
			{
				_v.x = -limitX;
				resetAngle = true;
			}

			if(resetAngle)
			{
				angle += 180;
				angle = UnityEngine.Random.Range(angle - 80,angle + 80);
				angle = angle % 360;

				speed = _speed * UnityEngine.Random.Range(0.85f, 1.15f);
			}

			tf.transform.localPosition = _v;
		}
	}


	void OnDestroy()
	{
		tf = null;
	}


}

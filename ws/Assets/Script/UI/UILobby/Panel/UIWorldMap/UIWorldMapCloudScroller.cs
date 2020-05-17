using UnityEngine;
using System.Collections;

public class UIWorldMapCloudScroller : MonoBehaviour {

	public Transform[] clouds = new Transform[3];

	public float width = 2798.0f;
	public float speed = 100.0f;

	void Update () 
	{
		_v = clouds[0].localPosition;
		_v.x -= speed * Time.smoothDeltaTime;
		clouds[0].localPosition = _v;

		int len = clouds.Length;

		for(int i = 1; i < len; ++i)
		{
			_v.x += width;
			clouds[i].localPosition = _v;
		}

		if(_v.x <= (-552.0f + (width * (len - 2))))
		{
			Transform tf = clouds[0];
			_v.x += width * (clouds.Length - 1);
			tf.localPosition = _v;


			for(int i = 0; i < len; ++i)
			{
				if(i == len - 1)
				{
					clouds[i] = tf;
				}
				else
				{
					clouds[i] = clouds[i+1];
				}
			}
		}
	}

	Vector3 _v;
	void OnEnabled()
	{
		_v = clouds[0].localPosition;
		_v.x = -552.0f;
		clouds[0].localPosition = _v;

		for(int i = 1; i < clouds.Length; ++i)
		{
			_v.x = -552.0f + width * i;
			clouds[i].localPosition = _v;
		}
	}

}

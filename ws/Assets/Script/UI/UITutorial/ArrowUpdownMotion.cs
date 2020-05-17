using UnityEngine;
using System.Collections;

public class ArrowUpdownMotion : MonoBehaviour {

	public Vector3 originalPos;

	public float yOffset = 50.0f;

	void Awake()
	{
		originalPos = transform.localPosition;
	}

	Vector3 _v;
	float value = 0.0f;
	void Update()
	{
		if(Time.timeScale >= 1)
		{
			value += Time.smoothDeltaTime * 6.0f;
		}
		else
		{
			value += RealTime.deltaTime * 6.0f;
		}


		_v = transform.localPosition;
		_v.y = originalPos.y + yOffset + Mathf.Sin(value) * 30.0f;
		transform.localPosition = _v;
	}

	void OnEnable()
	{
		transform.localPosition = originalPos;
		value = 0.0f;
	}

}

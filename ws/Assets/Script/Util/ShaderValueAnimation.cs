using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class ShaderValueAnimation : MonoBehaviour 
{
	public float speed = -0.5f;
	private float _time = 0.0f;
	public float limit = -1.0f;

	public bool useStartValue = false;
	public float startValue = -1.0f;

	public string targetShaderProperty = "_time";
	
	public enum ScrollType
	{
		Y_ONLY, X_ONLY, BOTH
	}
	
	public bool isShareMaterial = true;
	Material _tempMeterial = null;

	void Awake()
	{
		if(useStartValue)
		{
			_time = startValue;
		}
	}

	void Update () 
	{ 
		_time += Time.smoothDeltaTime * speed;

		if(useStartValue)
		{
			if(speed > 0 && startValue < 0 && _time > limit)
			{
				_time = startValue;
			}
			else if(speed < 0 && startValue > 0 && _time < limit)
			{
				_time = startValue;
			}
		}
		else
		{
			if(speed > 0 && _time > limit) _time %= limit;
			else if(speed < 0 && _time < limit) _time %= limit;
		}

		if(_time > 1.0f) _time -= 1.0f;

		if(renderer != null)
		{
			if(_tempMeterial == null)
			{
				if(isShareMaterial)
				{
					_tempMeterial = renderer.sharedMaterial;
				}
				else
				{
					_tempMeterial = renderer.material;
				}
			}

			if(_tempMeterial != null) _tempMeterial.SetFloat(targetShaderProperty, _time);
		}
	}

	void OnDestroy()
	{
		_tempMeterial = null;
	}
}




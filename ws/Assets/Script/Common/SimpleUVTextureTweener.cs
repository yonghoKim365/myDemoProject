using UnityEngine;
using System.Collections;

public class SimpleUVTextureTweener : MonoBehaviour 
{
	public Material mat;

	public float startValue = 0.0f;
	public float endValue = 0.0f;

	public float speed = 1.0f;

	public float nowValue = 0.0f;

	public void start()
	{
		nowValue = startValue;
		mat.SetTextureOffset ("_MainTex", new Vector2(0,startValue));
		enabled = true;
	}

	public void reset()
	{
		nowValue = startValue;
		mat.SetTextureOffset ("_MainTex", new Vector2(0,startValue));
	}


	void Update()
	{
		nowValue += speed * Time.deltaTime;

		if(speed > 0)
		{
			if(nowValue >= endValue)
			{
				nowValue = endValue;
				enabled = false;
			}
		}
		else if(speed < 0)
		{
			if(nowValue <= endValue)
			{
				nowValue = endValue;
				enabled = false;
			}
		}

		mat.SetTextureOffset ("_MainTex",new Vector2(0,nowValue));
	}

}

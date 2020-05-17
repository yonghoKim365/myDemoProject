using UnityEngine;
using System.Collections;

public class TextureSpriteAnimationTester : MonoBehaviour 
{
	public void init()
	{
	}
	
	public void Awake()
	{
	}

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;
	Vector3 _v;

	void Update () 
	{ 
		_v.x = xOffset;
		_v.y = yOffset;
		renderer.material.SetTextureOffset ("_MainTex",_v);
	}
}

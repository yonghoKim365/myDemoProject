using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextureSpriteAnimation : MonoBehaviour 
{
	private Vector2 _v;
	public int totalFrame = 3;
	public int nowFrame = 0;
	public float delay = 0.0f;

	float _delay = 0.0f;
	
	public void init()
	{
	}
	
	public void Awake()
	{
	}

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;

	public float x;
	public float y;

	FaceAnimationInfo data = null;

	public string nowAni = null;

	public void setData(FaceAnimationInfo info)
	{
#if UNITY_EDITOR
		nowAni = info.id;
#endif

		data = info;

		xOffset = info.xOffset;
		//yOffset = info.yOffset;
		yOffset = 0.0f;

		_delay = 0;

		totalFrame = data.frame.Length;
		nowFrame = 0;

		x = data.setup[ data.timeSheet[data.frame[nowFrame]].index ].x;
		y = data.setup[ data.timeSheet[data.frame[nowFrame]].index ].y;

		_v.x = x * xOffset;
		_v.y = y * yOffset;

		renderer.sharedMaterial.SetTextureOffset ("_MainTex",_v);

		_isEnabled = true;
	}

	public void clear()
	{
		data = null;
	}

	
	void Update () 
	{ 
		if(_isEnabled == false) return;
		if(data == null || PerformanceManager.isLowPc) return;
		if(data.timeSheet[data.frame[nowFrame]].delay < 0) return;

		_delay += Time.deltaTime;
		
		if(_delay > data.timeSheet[data.frame[nowFrame]].delay)
		{
			_delay = 0;
			++nowFrame;
			if(nowFrame >= totalFrame)
			{
				nowFrame = 0;
			}

			x = data.setup[ data.timeSheet[data.frame[nowFrame]].index ].x;
			y = data.setup[ data.timeSheet[data.frame[nowFrame]].index ].y;
			
			_v.x = x * xOffset;
			_v.y = y * yOffset;

			renderer.sharedMaterial.SetTextureOffset ("_MainTex",_v);
		}
	}

	bool _isEnabled = false;
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;
		}
	}

}


public class FaceAnimationInfo
{
	public string id;
	public string character;
	public string type;

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;

	public FaceAnimationPosition[] setup;
	public FaceAnimationTimeSheet[] timeSheet;
	public int[] frame;

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		character = (string)l[k["CHA"]];
		type = (string)l[k["TYPE"]];

		int x,y;
		Util.parseObject(l[k["X"]],out x, true, 0);
		Util.parseObject(l[k["Y"]],out y, true, 0);

		xOffset = 1.0f / (float)x;
		yOffset = 1.0f / (float)y;

		string[] t = ((string)l[k["SETUP"]]).Split('/');

		frame = Util.stringToIntArray(l[k["FRAME"]].ToString(),',');

		setup = new FaceAnimationPosition[t.Length];

		for(int i = 0; i < t.Length; ++i)
		{
			string[] t2 = t[i].Split(',');
			setup[i] = new FaceAnimationPosition();
			float.TryParse(t2[0],out setup[i].x);
			float.TryParse(t2[1],out setup[i].y);
		}

		t = ((string)l[k["SHEET"]]).Split('/');
		timeSheet = new FaceAnimationTimeSheet[t.Length];

		for(int i = 0; i < t.Length; ++i)
		{
			string[] t2 = t[i].Split(':');
			timeSheet[i] = new FaceAnimationTimeSheet();
			int.TryParse(t2[0],out timeSheet[i].index);
			float.TryParse(t2[1],out timeSheet[i].delay);
		}

	}
}

public struct FaceAnimationPosition
{
	public float x;
	public float y;
}

public struct FaceAnimationTimeSheet
{
	public int index;
	public float delay;
}
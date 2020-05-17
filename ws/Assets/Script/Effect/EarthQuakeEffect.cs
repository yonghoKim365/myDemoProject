using System;
using UnityEngine;

sealed public class EarthQuakeEffect  : MonoBehaviour 
{
	public EarthQuakeEffect ()
	{
	}
	
	void Start()
	{
	}
	
	private bool _isInit = false;
	private float _time = 0.0f;
	private float _size = 1.0f;
	private Transform _camera;
	private Vector3 _oriPos;
	private Vector3 _v;
	private float _value = 1.0f;
	
	public bool isReady = true;
	
	
	
	private float _endTime = 0.0f;
	private float _nowTime = 0.0f;
	
	public delegate void callback();
	private callback _callback = null;
	
	
	public enum Type { Vertical, Horiz, Mad };
	public enum MethodType { ByMath, ByNormal };
	
	private Type _type;
	private MethodType _method;
	
	
	public void init(Camera gameCamera, float time, float size, Type type = Type.Horiz, callback cb = null,  MethodType method = MethodType.ByMath)
	{
		if(isReady)
		{
			_type = type;
			_method = method; 
			
			_time = time;
			_camera = gameCamera.gameObject.transform.parent.transform;//gameCamera.transform;
			_size = size;
			
			_oriPos = _camera.localPosition;
			//_oriPos.x = GameManager.screenCenter.x;
			//_oriPos.y = GameManager.screenCenter.y;
			
			_isInit = true;
			isReady = false;
			_value = 1.0f;
			
			_prevTime = Time.realtimeSinceStartup;
			_nowTime = Time.realtimeSinceStartup;
			_endTime = Time.realtimeSinceStartup + time;
			
			_callback = cb;
		}
	}	
	
	
	
	private bool _isEnabled = false;
	
	public bool isEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
			gameObject.active = value;

			if(value == false)
			{
				_camera.localPosition = _oriPos;
				_isInit = false;		
				_callback = null;
				isReady = true;			
			}
		}
	}
	
	
	void Update()
	{
		if(GameManager.me.isPaused || _isEnabled == false || _isInit == false || GameManager.me.uiManager.currentUI != UIManager.Status.UI_PLAY) return;

		if(_method == MethodType.ByMath) update ();
		else if(_method == MethodType.ByNormal)  update2();
	}
	
	
	
	// 늦게 흔들리는 녀석...
	private void update()
	{
		_time -= GameManager.globalDeltaTime;
		_value += GameManager.globalDeltaTime*100.0f;
		
		if(_time <= 0.0f)
		{
			finish();
		}
		else
		{
			isReady = false;
			_v = _camera.localPosition;
			
			switch(_type)
			{
			case Type.Vertical:
				_v.y += Mathf.Sin(_value)*_size; //_oriPosY
				break;
			case Type.Horiz:
				_v.x += Mathf.Sin(_value)*_size;
				break;
			case Type.Mad:
				_v.y += Mathf.Sin(_value)*_size; //_oriPosY
				_v.x += Mathf.Sin(_value)*_size*-1.0f;
				break;
				
			}
			
			_camera.localPosition = _v;
		}		
	}
	
	
	private float _dirX = 1.0f;
	private float _dirY = -1.0f;
	
	private float _prevTime = 0.0f;
	
	int index = 0;
	// 빠르게 흔들리는 녀석...
	private void update2()
	{
		_dirX *= -1.0f;
		_dirY *= -1.0f;
		
		if(Time.realtimeSinceStartup >= _endTime)
		{
			finish();
		}
		else
		{
			isReady = false;
			_v = _camera.localPosition;
			
			
			switch(_type)
			{
			case Type.Vertical:
				_v.y += _size * _dirX; //_oriPos.y + 
				break;
			case Type.Horiz:
				_v.x += _size * _dirY; //_oriPos.x
				break;
			case Type.Mad:
				_v.y += _size * _dirX; //_oriPos.y
				_v.x += _size * _dirY; //_oriPos.x
				break;
			}			
			
			_camera.localPosition = _v;
		}
	}
	
	private void finish()
	{
		if(_callback != null) _callback();
		isEnabled = false;
	}
}


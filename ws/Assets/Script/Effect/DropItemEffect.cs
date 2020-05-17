using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public class DropItemEffect
{
	MapObject mobj;
	
	public DropItemEffect(MapObject mapObject)
	{
		mobj = mapObject;
	}	

	private bool _isEnabled = false;
	
	public float direction = 1.0f;
	
	bool isInit = false;
	
	bool startItemMotion = false;
	
	
	public string itemId = null;

	private float _gravity;
	
	private float _dx;
	private float _dz;
	private float _speedY;
	private float _landLine;
	
	private bool _startDisappear = false;
	
	private Vector3 _boundExtens;
	private Vector3 _boundCenter;

	private HitObject _hitObject = new HitObject();	
	
	private float _delay = 0.0f;
	
	private int _count = 0;
	
	public void start(Vector3 pos, float delay = 0.0f)
	{
		_time = 0.0f;
		_delay = delay;
		_v = pos;

		mobj.setPositionAndTransform(_v);

		_gravity = 1900.0f; // 중력
		_dx = GameManager.inGameRandom.Range(-100,30);
		_dz = GameManager.inGameRandom.Range(-50,50);
		_speedY = 900.0f - _dx; // y 속도

		_v = mobj.transformPosition;

		_landLine = _v.y + 100.0f;	

		startItemMotion = false;
		
		_hitObject = mobj.hitObject;
		_checkHitObject = mobj.hitObject;
		isEnabled = true;
	}
	

	// 포물선용
	public const float G = 2800.0f;//1980.0f;
	private float m_vx = 0;
	private float m_vy = 0;
	private float m_t = 0;	
	
	private float[] _duration = {0.5f,0.4f,0.2f,0.2f};
	private Vector2[] _targetPos = new Vector2[4];

	private HitObject _checkHitObject = new HitObject();
	
	
	public bool isEnabled
	{
		set
		{
			_isEnabled = value;
			if(value == false) isInit = false;
		}
		get
		{
			return _isEnabled;	
		}
	}	
	
	private Bounds _hitBounds;
	
	Vector3 _v;
	Vector3 _v2;
	
	private float _time = 0;
	private Color _color;
	
	public void update()
	{
		if(GameManager.me.isPaused) return;
		
		_time += GameManager.globalDeltaTime;
		
		if(_time >= _delay && _delay > -1.0f)
		{
			isEnabled = true;
			isInit = true;
			_delay = -1.0f;
		}

		if(startItemMotion)
		{
			updateItemMotion();
		}
		
		if(isInit == false) return;
		if(_isEnabled == false) return;
		
		// 일반 드랍 로직.
		
		_v = mobj.transformPosition;

		_speedY -= (_gravity * 1.05f)*GameManager.globalDeltaTime;
		
		_v.x += _dx*GameManager.globalDeltaTime;;
		_v.z += _dz*GameManager.globalDeltaTime;;
		_v.y += _speedY*GameManager.globalDeltaTime;
		
		
		
		if(_v.y <= _landLine && (_speedY*GameManager.globalDeltaTime < 0.0f)) // 기준선보다 y값이 작다...
		{
			_v.y = _landLine;
			isInit = false;
			isEnabled = false;
		}

		mobj.setPosition(_v);
	}
	
	
	void updateItemMotion()
	{
		_hitObject.setPosition(mobj.transformPosition);
		mobj.playMagnet();
	}

}


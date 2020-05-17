using System;
using System.Collections.Generic;
using UnityEngine;


sealed public class SimpleTweenManager
{
	private static List<SimpleTween> _tweenList = new List<SimpleTween>();
	
	public static SimpleTween addTween(Transform transform, Vector3 targetPos, float time, ITweenableObject targetObject = null, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		SimpleTween tw = new SimpleTween(transform, targetPos, time, targetObject, delay, callback, args);
		_tweenList.Add(tw);
		return tw;
	}

	
	public static bool removeTween(SimpleTween tw)
	{
		bool isRemove = _tweenList.Remove(tw);
		tw = null;
		return isRemove;
	}
	
	
	public static void clear()
	{
		int count = _tweenList.Count;
		int i;
		
		for(i = count - 1; i >= 0; --i)
		{
			_tweenList[i].method = null;
			_tweenList[i].parameters = null;
			_tweenList[i] = null;
		}
		
		_tweenList.Clear();
	}
	
	
	private SimpleTween tw;
	
	public void update()
	{
		if(GameManager.me.isPaused) return;
		
		for(int i = _tweenList.Count - 1; i >= 0; --i)
		{
			tw = _tweenList[i];
			
			if(tw.pause) continue;
			
			if(tw.delay > 0.0f)
			{
				tw.delay -= GameManager.globalDeltaTime;
				continue;
			}
			
			if(tw.time > 0.0f)
			{
				tw.update(GameManager.globalDeltaTime);
			}
			
			if(tw.time <= 0.0f)
			{
				_tweenList.RemoveAt(i);
				
				tw.complete();
				
				if(tw.method != null)
				{
					tw.method.DynamicInvoke(tw.parameters);
					if(_tweenList.Count == 0) break;
				}
				
				tw = null;
			}
		}
	}	
}


public class SimpleTween
{
	public object[] parameters;
	public float delay;
	public System.Delegate method;
	public Transform transform;
	public float time = 0.0f;	
	
	public bool pause = false;
	
	private float _speedX = 0.0f;
	private float _speedY = 0.0f;
	
	private Vector3 _v;
	private Vector3 _targetPos;
	
	public bool isComplete = false;
	
	private ITweenableObject _targetObject;
	
	private bool _isLocal = false;
	
	public SimpleTween(Transform transform, Vector3 targetPos, float time, ITweenableObject targetObject = null, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		_isLocal = false;
		
		_targetObject = targetObject;
		
		_targetPos = targetPos;
		
		_v = transform.position;
		
		_speedX = (targetPos.x - _v.x) / time;
		_speedY = (targetPos.y - _v.y) / time;
		
		this.transform = transform;
		this.time = time;			
		this.delay = delay;
		this.method = callback;
		this.parameters = args;
	}
	
	public bool isLocal
	{
		set
		{
			if(value)
			{
				_v = transform.localPosition;
				_speedX = (_targetPos.x - _v.x) / time;
				_speedY = (_targetPos.y - _v.y) / time;
				_isLocal = value;
			}
		}
	}
	
	
	public void update(float deltaTime)
	{
		if(_isLocal)
		{
			_v = transform.localPosition;
			_v.x += _speedX * deltaTime;
			_v.y += _speedY * deltaTime;
			transform.localPosition = _v;
		}
		else
		{
			_v = transform.position;
			_v.x += _speedX * deltaTime;
			_v.y += _speedY * deltaTime;
			transform.position = _v;
			
		}
		
		time -= deltaTime;
	}
	
	public void complete()
	{
		isComplete = true;
		transform.position = _targetPos;
		if(_targetObject != null) _targetObject.removeTween();
	}
}

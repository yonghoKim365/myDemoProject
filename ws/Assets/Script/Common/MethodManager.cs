using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed public class MethodManager
{
	private static List<DelayMethod> _methodList = new List<DelayMethod>();
	private static List<DelayMethod> _inPlayMethodList = new List<DelayMethod>();
	
	public static void addDelayFunc(float delay, System.Delegate method, params object[] args)
	{
		DelayMethod dm = new DelayMethod(delay, method, args);
		_methodList.Add(dm);
	}
	
	
	public static void addInGameDelayFunc(float delay, System.Delegate method, params object[] args)
	{
		DelayMethod dm = new DelayMethod(delay, method, args);
		_inPlayMethodList.Add(dm);
	}

	
	public static void clearInGameFunc()
	{
		int count = _inPlayMethodList.Count;
		int i;
		
		for(i = count - 1; i >= 0; --i)
		{
			_inPlayMethodList[i].method = null;
			_inPlayMethodList[i].parameters = null;
			_inPlayMethodList[i] = null;
		}
		
		_inPlayMethodList.Clear();		
	}
	
	
	public static void clear()
	{
		int count = _methodList.Count;
		int i;
		
		for(i = count - 1; i >= 0; --i)
		{
			_methodList[i].method = null;
			_methodList[i].parameters = null;
			_methodList[i] = null;
		}
		
		_methodList.Clear();
		clearInGameFunc();
	}
	
	

	public void update()
	{
		int i;
		for(i = _methodList.Count - 1; i >= 0; --i)
		{
			if(_methodList[i].delay <= 0.0f)
			{
				_methodList[i].method.DynamicInvoke(_methodList[i].parameters);
				if(_methodList.Count == 0) break;
				_methodList.RemoveAt(i);
				continue;
			}
			
			_methodList[i].delay -= GameManager.globalDeltaTime;
		}
		
		if(GameManager.me.isPaused) return;
		
		for(i = _inPlayMethodList.Count - 1; i >= 0; --i)
		{
			if(_inPlayMethodList[i].delay <= 0.0f)
			{
				_inPlayMethodList[i].method.DynamicInvoke(_inPlayMethodList[i].parameters);
				if(_inPlayMethodList.Count == 0) break;
				_inPlayMethodList.RemoveAt(i);
				continue;
			}
			
			_inPlayMethodList[i].delay -= GameManager.globalDeltaTime;
		}
	}
}


public class DelayMethod
{
	public object[] parameters;
	public float delay;
	public System.Delegate method;
	
	public DelayMethod(float delay, System.Delegate method, params object[] args)
	{
		this.delay = delay;
		this.method = method;
		this.parameters = args;
	}
}

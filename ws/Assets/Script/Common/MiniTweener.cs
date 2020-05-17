using System;
using UnityEngine;

public abstract class MiniTweener
{
	public MiniTweener ()
	{
	}
	
	public float time;
	public float progressTime = 0.0f;
	public bool isComplete = false;
	
	protected float _delay;
	protected object[] _parameters;	
	protected System.Delegate _method;
	
	public abstract void update(float deltaTime);
	
	public abstract void delete();
	
	protected void doComplete()
	{
		if(_method != null) _method.DynamicInvoke(_parameters);
	}
	
	protected void deleteBase()
	{
		_parameters = null;
		_method = null;
	}
}



sealed public class MiniTweenerDelayMethod : MiniTweener
{
	private float _speedX = 0.0f;
	private float fromValue;
	private float to;
	
	public static MiniTweenerDelayMethod start(float duration, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		MiniTweenerDelayMethod mt = new MiniTweenerDelayMethod();
		mt.init(duration, delay, callback, args);
		return mt;
	}
	
	public void init(float duration, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		time = duration;
		isComplete = false;
		_method = callback;
		_delay = delay;
		_parameters = args;
	}
	
	sealed public override void update (float deltaTime)
	{
		if(_delay >= 0)
		{
			_delay -= deltaTime;
			if(_delay < 0) progressTime += (-_delay);
			return;
		}
		
		progressTime += deltaTime;
		
		if(progressTime >= time)
		{
			isComplete = true;
			doComplete();
		}
	}
	
	sealed public override void delete()
	{
		base.deleteBase();
//		Debug.LogError("delete delay method tweener");
	}
	
	
}




sealed public class MiniTweenerFOV : MiniTweener
{
	private Camera _cam;
	private float _speedX = 0.0f;
	private float fromValue;
	private float to;
	
	public static MiniTweenerFOV start(Camera cam, float targetFov, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		MiniTweenerFOV mt = new MiniTweenerFOV();
		mt.init(cam, targetFov, duration, easingType, delay, callback, args);
		return mt;
	}
	
	public void init(Camera cam, float targetFov, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		time = duration;
		_cam = cam;
		fromValue = _cam.fieldOfView;
		to = targetFov;
		isComplete = false;
		progressTime = 0.0f;
		_speedX = (to - fromValue) / duration ;
		_method = callback;
		_delay = delay;
		_parameters = args;

		if(string.IsNullOrEmpty( easingType ) == false)
		{
			useEasing = true;
			string[] e = easingType.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e[1]);
			easingMethod = e[0];
		}
	}
	
	bool useEasing = false;
	string easingMethod = "";
	EasingType easingStyle;
	
	sealed public override void update (float deltaTime)
	{
		if(_delay >= 0)
		{
			_delay -= deltaTime;
			if(_delay < 0) progressTime += (-_delay);
			return;
		}
		
		progressTime += deltaTime;
		
		if(progressTime >= time)
		{
			_cam.fieldOfView = to;
			isComplete = true;
			doComplete();
		}
		else
		{
			//_cam.fieldOfView += _speedX * deltaTime;	
			float timeStep = progressTime/time;
			if(useEasing) timeStep = MiniTweenEasingType.getEasingValue(timeStep, easingMethod, easingStyle);
			_cam.fieldOfView = Mathf.Lerp(fromValue,to,timeStep);
		}
	}
	
	sealed public override void delete()
	{
		base.deleteBase();
		_cam = null;
	}
	
}


sealed public class MiniTweenerLocalPosition : MiniTweener
{
	private Transform _target;
	
	public float fromValue;
	public float to;
	
	private float _targetValueAmount = 0.0f;
	private float _speedX = 0.0f;
	private float _speedY = 0.0f;
	private float _speedZ = 0.0f;
	private Vector3 _v;
	private Vector3 _fromPos;
	private Vector3 _toPos;	
	
	public static MiniTweenerLocalPosition start(Transform tf, Vector3 targetPos, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		MiniTweenerLocalPosition mt = new MiniTweenerLocalPosition();
		mt.init(tf, targetPos, duration, easingType, delay, callback, args);
		return mt;
	}
	
	public void init(Transform tf, Vector3 targetPos, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		time = duration;
		_target = tf;
		
		_fromPos = tf.localPosition;
		_toPos = targetPos;
		
		isComplete = false;
		progressTime = 0.0f;
		//_speedX = (_toPos.x - _fromPos.x) / duration ;
		//_speedY = (_toPos.y - _fromPos.y) / duration ;
		//_speedZ = (_toPos.z - _fromPos.z) / duration ;
		
		_method = callback;
		_delay = delay;
		_parameters = args;		

		if(string.IsNullOrEmpty( easingType ) == false)
		{
			useEasing = true;
			string[] e = easingType.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e[1]);
			easingMethod = e[0];

		}
	}
	
	bool useEasing = false;
	string easingMethod = "";
	EasingType easingStyle;
	
	
	sealed public override void update (float deltaTime)
	{
		if(_delay >= 0)
		{
			_delay -= deltaTime;
			if(_delay < 0) progressTime += (-_delay);
			return;
		}
		
		progressTime += deltaTime;
		
		_v = _target.localPosition;
		//_v.x += _speedX * deltaTime;
		//_v.y += _speedY * deltaTime;
		//_v.z += _speedZ * deltaTime;
		
		if(progressTime >= time)
		{
			_target.localPosition = _toPos + GameManager.me.uiManager.uiPlay.getPlayerSkillEffectCamRuntimeOffsetPosition();

			if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Close)
			{
				GameManager.me.uiManager.uiPlay.gameCameraContainer.position = Vector3.Lerp(GameManager.me.uiManager.uiPlay.gameCameraContainer.position, GameManager.me.uiManager.uiPlay.cameraTarget.position, 0.9f);
			}

			isComplete = true;
			doComplete();
		}
		else
		{
			timeStep = progressTime/time;
			if(useEasing) timeStep = MiniTweenEasingType.getEasingValue(timeStep, easingMethod, easingStyle);
			_target.localPosition = Vector3.Lerp(_fromPos,_toPos, timeStep) + GameManager.me.uiManager.uiPlay.getPlayerSkillEffectCamRuntimeOffsetPosition();//_v;	

			if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Close)
			{
				GameManager.me.uiManager.uiPlay.gameCameraContainer.position = Vector3.Lerp(GameManager.me.uiManager.uiPlay.gameCameraContainer.position, GameManager.me.uiManager.uiPlay.cameraTarget.position, (timeStep > 0.7f)?timeStep*0.9f:timeStep);
			}

//			float step = progressTime/time;
//			step = Easing.EaseInOut(step,EasingType.Quadratic);
//			_target.localPosition = Vector3.Lerp(_fromPos,_toPos,step);//_v;	

		}
	}

	float timeStep = 0.0f;

	/*
	CurrentPosition = Lerp(OldPosition, NewPosition, CurrentTime / TotalDuration)
		
		If you want to use my easing class, you’d do instead :
			
			Step = CurrentTime / TotalDuration
			EasedStep = Easing.EaseIn(Step, EasingType.Quadratic)
			CurrentPosition = Lerp(OldPosition, NewPosition, EasedStep)
			*/
	
	sealed public override void delete()
	{
		base.deleteBase();
		_target = null;
	}
	
	
}







sealed public class MiniTweenerCamRotation : MiniTweener
{
	private Camera _cam;
	private Quaternion _targetRot;
	private float _speedX = 0.0f;
	private Quaternion fromRot;

	
	public static MiniTweenerCamRotation start(Camera cam, Vector3 targetRot, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		MiniTweenerCamRotation mt = new MiniTweenerCamRotation();
		mt.init(cam, targetRot, duration, easingType, delay, callback, args);
		return mt;
	}
	
	public void init(Camera cam, Vector3 targetRot, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		time = duration;
		_cam = cam;
		fromRot = _cam.transform.rotation;
		
		Quaternion q = cam.transform.rotation;
		q.eulerAngles = targetRot;
		_targetRot = q;
		isComplete = false;
		progressTime = 0.0f;
		
		_method = callback;
		_delay = delay;
		_parameters = args;

		if(string.IsNullOrEmpty( easingType ) == false)
		{
			useEasing = true;
			string[] e = easingType.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e[1]);
			easingMethod = e[0];
		}
	}

	bool useEasing = false;
	string easingMethod = "";
	EasingType easingStyle;


	sealed public override void update (float deltaTime)
	{
		if(_delay >= 0)
		{
			_delay -= deltaTime;
			if(_delay < 0) progressTime += (-_delay);
			return;
		}
		
		progressTime += deltaTime;
		
		if(progressTime >= time)
		{
			//_cam.transform.rotation = _targetRot;
			_cam.transform.rotation = Quaternion.Slerp(fromRot, _targetRot, 1.0f);
			isComplete = true;
			doComplete();
		}
		else
		{
			timeStep = progressTime/time;
			if(useEasing) timeStep = MiniTweenEasingType.getEasingValue(timeStep, easingMethod, easingStyle);
			_cam.transform.rotation = Quaternion.Slerp(fromRot, _targetRot, timeStep);			
		}
	}

	float timeStep;
	
	sealed public override void delete()
	{
		base.deleteBase();
		_cam = null;
	}
}





sealed public class MiniTweenerCamRoundMovement : MiniTweener
{
	private Transform _target;
	
	public float fromValue;
	public float to;
	
	private float _targetValueAmount = 0.0f;
	private float _speedX = 0.0f;
	private float _speedY = 0.0f;
	private float _speedZ = 0.0f;
	private Vector3 _v;
	private Vector3 _fromPos;
	private Vector3 _toPos;	
	
	public static MiniTweenerCamRoundMovement start(Transform tf, Vector3 targetPos, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		MiniTweenerCamRoundMovement mt = new MiniTweenerCamRoundMovement();
		mt.init(tf, targetPos, duration, easingType, delay, callback, args);
		return mt;
	}
	
	public void init(Transform tf, Vector3 targetPos, float duration, string easingType, float delay = 0.0f, System.Delegate callback = null, params object[] args)
	{
		time = duration;
		_target = tf;
		
		_fromPos = tf.localPosition;
		_toPos = targetPos;
		
		isComplete = false;
		progressTime = 0.0f;

		_method = callback;
		_delay = delay;
		_parameters = args;		
		
		if(string.IsNullOrEmpty( easingType ) == false)
		{
			useEasing = true;
			string[] e = easingType.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e[1]);
			easingMethod = e[0];
			
		}
	}
	
	bool useEasing = false;
	string easingMethod = "";
	EasingType easingStyle;
	
	
	sealed public override void update (float deltaTime)
	{
		if(_delay >= 0)
		{
			_delay -= deltaTime;
			if(_delay < 0) progressTime += (-_delay);
			return;
		}
		
		progressTime += deltaTime;
		
		_v = _target.localPosition;

		if(progressTime >= time)
		{
			_target.localPosition = _toPos;
			isComplete = true;
			doComplete();
		}
		else
		{
			timeStep = progressTime/time;
			if(useEasing) timeStep = MiniTweenEasingType.getEasingValue(timeStep, easingMethod, easingStyle);
			_target.localPosition = Vector3.Lerp(_fromPos,_toPos, timeStep);//_v;	
			
			//			float step = progressTime/time;
			//			step = Easing.EaseInOut(step,EasingType.Quadratic);
			//			_target.localPosition = Vector3.Lerp(_fromPos,_toPos,step);//_v;	
			
		}
	}
	
	float timeStep = 0.0f;

	sealed public override void delete()
	{
		base.deleteBase();
		_target = null;
	}
	

}











public class MiniTweenEasingType
{
	public const string EaseOut = "EaseOut";
	public const string EaseIn = "EaseIn";
	public const string EaseInOut = "EaseInOut";
	
	public const string Step = "Step";
	public const string Linear = "Linear";
	public const string Sine = "Sine";
	public const string Quadratic = "Quadratic";
	public const string Cubic = "Cubic";
	public const string Quartic = "Quartic";
	public const string Quintic = "Quintic";


	public static EasingType getEasingType(string easingType)
	{
		switch(easingType)
		{
		case Step:
			return EasingType.Step;
			break;
		case Linear:
			return EasingType.Linear;
			break;
		case Sine:
			return EasingType.Sine;
			break;
		case Quadratic:
			return EasingType.Quadratic;
			break;
		case Cubic:
			return EasingType.Cubic;
			break;
		case Quartic:
			return EasingType.Quartic;
			break;
		case Quintic:
			return EasingType.Quintic;
			break;
		}

		return EasingType.Linear;
	}

	public static float getEasingValue(float timeStep, string easingMethod, EasingType easingType)
	{
		switch(easingMethod)
		{
		case MiniTweenEasingType.EaseIn:
			timeStep = Easing.EaseIn(timeStep, easingType);
			break;
		case MiniTweenEasingType.EaseInOut:
			timeStep = Easing.EaseInOut(timeStep, easingType);
			break;
		case MiniTweenEasingType.EaseOut:
			timeStep = Easing.EaseOut(timeStep, easingType);
			break;
		}

		return timeStep;
	}


}
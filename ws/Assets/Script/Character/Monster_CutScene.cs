using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public partial class Monster : MonoBehaviour//, ITweenableObject
{
	//=======================================================================================//
	// 			컷씬 용		
	//=======================================================================================//	
	
	public Xbool isReady = false;
	
	
	
	private string _csAniState;
	public void playCutSceneAni(string aniId, int playNum, int leftPlayNum, int actionOnComplete)
	{
		WrapMode wrapMode = WrapMode.Loop;
		
		_csAniState = aniId;
		
		if(playNum == 0) // 무한
		{
#if UNITY_EDITOR
			Debug.Log(resourceId + " " + aniId);
#endif

			animation.clip = animation.GetClip(aniId);
			animation.GetClip(aniId).wrapMode = wrapMode;
			animation.CrossFade(aniId, aniFadeTime);
			//			animation.wrapMode = wrapMode;
			if(pet != null)
			{
				playPetCSAni(aniId, wrapMode);
				//				pet.animation.wrapMode = wrapMode;
			}			
		}
		else
		{
			wrapMode = WrapMode.Once;
			
//			Debug.Log("leftPlayNum : " + leftPlayNum);
			
			if(leftPlayNum > 1 )
			{
				animation.clip = animation.GetClip(aniId);
				animation.GetClip(aniId).wrapMode = wrapMode;
				animation.CrossFade(aniId, aniFadeTime);
				//				animation.wrapMode = wrapMode;
				if(pet != null)
				{
					playPetCSAni(aniId, wrapMode);
					//					pet.animation.wrapMode = wrapMode;
				}	
				
				--leftPlayNum;
				StartCoroutine(onCompleteCutSceneAni(animation.GetClip(aniId).length, aniId, playNum, leftPlayNum, actionOnComplete, aniId));
			}
			else
			{
				// 마지막 애니메이션..
				
//				Debug.Log("actionOnComplete : " + actionOnComplete);

				AnimationClip ac = animation.GetClip(aniId);
				if(ac != null)
				{
					animation.clip = ac;
					animation.GetClip(aniId).wrapMode = wrapMode;
					animation.CrossFade(aniId, aniFadeTime);
				}
				//				animation.wrapMode = wrapMode;
				if(pet != null)
				{
					playPetCSAni(aniId, wrapMode);
					//					pet.animation.wrapMode = wrapMode;
				}	
				
				if(deleteMotionEffect != null && deleteMotionEffect.effectChracter != null && (aniId == DEAD || aniId == SCENE_DEAD) )
				{
					StartCoroutine(onCompleteCutSceneAni(animation.GetClip(aniId).length, aniId, playNum, leftPlayNum, actionOnComplete, aniId));
				}
				else
				{
					if(actionOnComplete == 1)
					{
						StartCoroutine(onCompleteCutSceneAniIdle(animation.GetClip(aniId).length, aniId));
					}
					else if(actionOnComplete == 2)
					{
						_isEnabled.Set( false );
						StartCoroutine(onCompleteCutSceneAniVisible(animation.GetClip(aniId).length, false));
					}
				}
			}
		}
	}
	
	
	
	IEnumerator onCompleteCutSceneAni(float waitTime, string aniId, int playNum, int leftPlayNum, int actionOnComplete, string prevState)
	{
		if(prevState == _csAniState)
		{
			yield return new WaitForSeconds(waitTime);
			
			// 아주 특수한 경우로 죽는 애니가 2개로 나뉘어져있는 경우가 있다.
			if(deleteMotionEffect != null && deleteMotionEffect.effectChracter != null && (aniId == DEAD || aniId == SCENE_DEAD))
			{
				deleteMotionEffect.effectChracter.setPositionCtransform( cTransformPosition );
				deleteMotionEffect.effectChracter.tf.position = tf.position;
				deleteMotionEffect.effectChracter.tf.rotation = tf.rotation;				
				deleteMotionEffect.effectChracter.isEnabled = true;
				// 어차피 얘는 die용으로 쓰는 것. 
				deleteMotionEffect.effectChracter.playCSEffectAni(aniId, playNum, leftPlayNum, actionOnComplete, playCutSceneAni, cTransform.gameObject);
				cTransform.gameObject.SetActive(false);				
			}
			else
			{
				playCutSceneAni(aniId, playNum, leftPlayNum, actionOnComplete);	
			}
		}
		else yield return null;
	}
	
	
	IEnumerator onCompleteCutSceneAniIdle(float waitTime, string prevState)
	{
		if(prevState == _csAniState)
		{
			yield return new WaitForSeconds(waitTime);

			if(prevState == _csAniState)
			{
				
				WrapMode wrapMode = WrapMode.Loop;

				string normalId = getCutSceneNormal();

				animation.clip = animation.GetClip(normalId);
				animation.GetClip(normalId).wrapMode = wrapMode;
				animation.CrossFade(normalId, aniFadeTime);
				//			animation.wrapMode = wrapMode;
				
				if(pet != null)
				{
					playPetCSAni(NORMAL, wrapMode);
				}			
			}
		}
		else
		{
			yield return null;
		}
	}



	IEnumerator onCompleteCutSceneAniVisible(float waitTime, bool visible)
	{
		yield return new WaitForSeconds(waitTime);
		_isEnabled.Set( false );
		setVisible(visible);
	}



	
	public delegate void OnCutSceneAniCallback(string aniId, int playNum, int leftPlayNum, int actionOnComplete);
	
	public void playCSEffectAni(string aniId, int playNum, int leftPlayNum, int actionOnComplete, OnCutSceneAniCallback callback, GameObject parent)
	{
		animation.clip = animation.GetClip(aniId);
		animation.GetClip(aniId).wrapMode = WrapMode.Once;
		animation.CrossFade(aniId, aniFadeTime);
		// 현재는 die 하나만 쓰고 있으니까... 나머지는 무시한다.
		//StartCoroutine(onCompleteCutSceneChracterEffectAni(animation.GetClip(aniId).length, callback, aniId, playNum, leftPlayNum, actionOnComplete, parent));
	}
	
	IEnumerator onCompleteCutSceneChracterEffectAni(float waitTime, OnCutSceneAniCallback callback, string aniId, int playNum, int leftPlayNum, int actionOnComplete,  GameObject parent)
	{
		yield return new WaitForSeconds(waitTime);
		parent.SetActive(true);
		isEnabled = false;
		callback(aniId, playNum, leftPlayNum, actionOnComplete);
	}
	
	
	
	public void playCSAni(string ani, WrapMode wrapMode, bool must = false)
	{
		if(animation.GetClip(ani) != null)
		{
			animation.clip = animation.GetClip(ani);
			animation.GetClip(ani).wrapMode = wrapMode;

			if(must)
			{
				animation.Stop();
				animation.Play(ani);
			}
			else
			{
				animation.CrossFade(ani, aniFadeTime);
			}


			//			animation.wrapMode = wrapMode;
			
			if(pet != null)
			{
				playPetCSAni(ani, wrapMode);
			}			
		}
	}


	void playPetCSAni(string ani, WrapMode wrapMode)
	{
		if(ani == SCENE_DEAD) ani = Monster.DEAD;

		AnimationClip ac = pet.animation.GetClip(ani);

		if(ac != null)
		{
			ac.wrapMode = wrapMode;
			pet.animation.clip = ac;
			pet.animation.GetClip(ani).wrapMode = wrapMode;
			pet.animation.CrossFade(ani, aniFadeTime);
		}
	}




	public enum CutSceneTweenType
	{
		None, Position, PositionWithNoRotation
	}

	public CutSceneTweenType cutScenePositionTween = CutSceneTweenType.None;
	public int cutScenePositionTweenType = 1;

	public bool isCutSceneRotationTween = false;
	public bool isCutSceneUpdownMotion = false;

	public Vector3 csStartPos = new Vector3();
	public float csTargetDistance = 1.0f;
	
	public float csTweenSpeed = 100.0f;
	public float csTweenTime = 0.0f;
	private float _csProgressDeltaTime = 0.0f;
	private Quaternion _csStartRotation;
	private Quaternion _csTargetRotation;

	public float csUpDownCenter = 0.0f;
	public float csUpDownSpeed = 0.0f;
	public float csUpDownRange = 0.0f;
	private float _csUpDownDelta = 0.0f;

	public Vector3 csTargetPos = new Vector3();

	public int csAttr = 1;

	public void cutSceneInit()
	{
		cutScenePositionTween = CutSceneTweenType.None;
		isCutSceneRotationTween = false;
		isCutSceneUpdownMotion = false;
		csScaleTweener.init();
		csTweenSpeed = 0.0f;
		csTweenTime = 0.0f;
		_csProgressDeltaTime = 0.0f;
		csStartPos.x = 0;csStartPos.y = 0;csStartPos.z = 0;
		csTargetDistance = 0.0f;
		_csUpDownDelta = 0.0f;
		csUpDownCenter = 0.0f;
		csObjectColorTweener.isEnabled = false;
	}
	
	public void setCSRotation(float motionTime, Vector3 rotationValue)
	{
		csTweenTime = motionTime;
		_csProgressDeltaTime = 0.0f;
		isCutSceneRotationTween = true;
		_csStartRotation = tf.rotation;
		_csTargetRotation = tf.rotation;
		_csTargetRotation.eulerAngles = rotationValue;
	}

	public string getCutSceneNormal()
	{
		if(ani.GetClip(DEFAULT_NORMAL) != null) return DEFAULT_NORMAL;
		return NORMAL;
	}


	void onCompleteCutScenePositionTween()
	{
		if(csAttr == 1)
		{
			playCSAni(getCutSceneNormal(), WrapMode.Loop);
		}

		cutScenePositionTween = CutSceneTweenType.None;
	}

	public void cutSceneUpdate()
	{
		if(cutScenePositionTween == CutSceneTweenType.Position)
		{
			if(cutScenePositionTweenType == 1)
			{
				setPositionCtransform(cTransform.position + tf.forward * csTweenSpeed * CutSceneManager.cutSceneDeltaTime);
				if(VectorUtil.Distance3D(csStartPos, cTransformPosition) >= csTargetDistance) onCompleteCutScenePositionTween();
			}
			else if(cutScenePositionTweenType == 2)
			{
				if(csObjectMove2Tweener.update(CutSceneManager.cutSceneDeltaTime,this) == false) onCompleteCutScenePositionTween();
			}
		}
		else if(cutScenePositionTween == CutSceneTweenType.PositionWithNoRotation)
		{
			if(cutScenePositionTweenType == 1)
			{
				setPositionCtransform(Vector3.MoveTowards(cTransform.position, csTargetPos, csTweenSpeed * CutSceneManager.cutSceneDeltaTime));
				if(VectorUtil.Distance3D(csStartPos, cTransformPosition) >= csTargetDistance) onCompleteCutScenePositionTween();
			}
			else if(cutScenePositionTweenType == 2)
			{
				if(csObjectMove2Tweener.update(CutSceneManager.cutSceneDeltaTime,this) == false) onCompleteCutScenePositionTween();
			}
		}
		else if(isCutSceneUpdownMotion && Time.timeScale > 0)
		{
			_csUpDownDelta += CutSceneManager.cutSceneDeltaTime * csUpDownSpeed;

			_v = cTransform.position;
			_v.y = csUpDownCenter + Mathf.Sin(_csUpDownDelta) * csUpDownRange;
			setPositionCtransform(_v);
		}


		if(isCutSceneRotationTween)
		{
			_csProgressDeltaTime += CutSceneManager.cutSceneDeltaTime;
			
			if(_csProgressDeltaTime >= csTweenTime)
			{
				tf.rotation = _csTargetRotation;
				isCutSceneRotationTween = false;
			}
			else
			{
				tf.rotation = Quaternion.Slerp(_csStartRotation, _csTargetRotation, _csProgressDeltaTime/csTweenTime);	
			}
		}

		if(csObjectColorTweener.isEnabled)
		{
			csObjectColorTweener.update(CutSceneManager.cutSceneDeltaTime, this);
		}

		if(csScaleTweener.isEnabled)
		{
			csScaleTweener.update(CutSceneManager.cutSceneDeltaTime, cTransform);
		}

		// 데미지 모션용 ================ //
		updateCutSceneDamageMotion();
	}



	private bool _isLeftMonster = false;
	public void cutSceneDamageEffect(bool isLeft)
	{
		_isLeftMonster = isLeft;
		if(_damageMotionDuration > 100.0f)
		{
			_damageMotionDuration = 0.0f;
			_damageMotionStep = 0;
			
			_v2 = tf.localPosition;
			_v2.x += (isLeft)?-damageMotionValue:damageMotionValue;
			tf.localPosition = _v2;
		}
	
		setDamageFrame();
	}

	void updateCutSceneDamageMotion()
	{
		_currentDamageTime -= CutSceneManager.cutSceneDeltaTime;

		updateColor();

		if(_damageMotionDuration < 100.0f)
		{
			if(_damageMotionDuration < DAMAGE_MOTION_DURATION[0]) // 0.04f
			{
				_damageMotionDuration += CutSceneManager.cutSceneDeltaTime;
			}
			else if(_damageMotionDuration < DAMAGE_MOTION_DURATION[1]) // 0.04f
			{
				if(_damageMotionStep < 1)
				{
					_v2 = tf.localPosition;
					_v2.x = 0.0f;
					tf.localPosition = _v2;
					
					_damageMotionStep = 1;
				}
				
				_damageMotionDuration += CutSceneManager.cutSceneDeltaTime;
			}
			else if(_damageMotionDuration < DAMAGE_MOTION_DURATION[2]) // 0.04f
			{
				if(_damageMotionStep < 2)
				{
					_v2 = tf.localPosition;
					_v2.x += (_isLeftMonster)?-damageMotionStep2Value:damageMotionStep2Value;
					tf.localPosition = _v2;
					
					_damageMotionStep = 2;
				}
				
				_damageMotionDuration += CutSceneManager.cutSceneDeltaTime;
			}
			else
			{
				_v2 = tf.localPosition;
				_v2.x = 0.0f;
				tf.localPosition = _v2;
				
				_damageMotionDuration = 1000.0f;
			}
		}

	}





	public CutSceneScaleTweener csScaleTweener = new CutSceneScaleTweener();
	public CutSceneObjectMove2Tweener csObjectMove2Tweener = new CutSceneObjectMove2Tweener();
	public CutSceneObjectColorTweener csObjectColorTweener = new CutSceneObjectColorTweener();


}


public struct CutSceneScaleTweener
{
	public bool isEnabled;
	public float progressTime;
	public float motionTime;


	private string _easingMethod;
	private EasingType  _easingStyle;

	private float _startScale;
	private float _targetScale;
	private bool _useEasing;


	public void init()
	{
		isEnabled = false;
		progressTime = 0.0f;
		motionTime = 0.0f;
		_useEasing = false;
		_v = new Vector3();
	}

	public void start(float duration, float startScale, float targetScale, bool useEasing, string easingMethod, EasingType easingStyle)
	{
		isEnabled = true;
		progressTime = 0.0f;
		motionTime = duration;

		_startScale = startScale;
		_targetScale = targetScale;

		_useEasing = useEasing;
		_easingMethod = easingMethod;
		_easingStyle = easingStyle;

	}

	float _timeStep;
	Vector3 _v;

	public void update(float delta, Transform target)
	{
		progressTime += delta;
		if(progressTime >= motionTime)
		{
			isEnabled = false;
			_v.x = _targetScale;
		}
		else
		{
			_timeStep = progressTime/motionTime;
			if(_useEasing) _timeStep = MiniTweenEasingType.getEasingValue(_timeStep, _easingMethod, _easingStyle);
			_v.x = Mathf.Lerp(_startScale, _targetScale, _timeStep);
		}

		_v.y = _v.x; _v.z = _v.x;

		target.localScale = _v;
	}
}




public struct CutSceneObjectMove2Tweener
{
	public bool isEnabled;
	public float progressTime;
	public float motionTime;

	private string _easingMethod;
	private EasingType  _easingStyle;
	
	private Vector3 _startPos;
	private Vector3 _targetPos;
	private bool _useEasing;

	public void init()
	{
		isEnabled = false;
		progressTime = 0.0f;
		motionTime = 0.0f;
		_useEasing = false;
	}
	
	public void start(float duration, Vector3 startPos, Vector3 targetPos, bool useEasing, string easingMethod, EasingType easingStyle)
	{
		isEnabled = true;
		progressTime = 0.0f;
		motionTime = duration;
		
		_startPos = startPos;
		_targetPos = targetPos;
		
		_useEasing = useEasing;
		_easingMethod = easingMethod;
		_easingStyle = easingStyle;
		
	}
	
	float _timeStep;
	Vector3 _v;
	
	public bool update(float delta, Monster mon)
	{
		progressTime += delta;
		if(progressTime >= motionTime)
		{
			isEnabled = false;
			mon.setPositionCtransform(_targetPos);
			return false;
		}
		else
		{
			_timeStep = progressTime/motionTime;
			if(_useEasing) _timeStep = MiniTweenEasingType.getEasingValue(_timeStep, _easingMethod, _easingStyle);
			mon.setPositionCtransform(Vector3.Lerp(_startPos,_targetPos,_timeStep));
			return true;
		}
	}
}





public struct CutSceneObjectColorTweener
{
	public bool isEnabled;
	public float progressTime;
	public float motionTime;
	
	private string _easingMethod;
	private EasingType  _easingStyle;
	
	private Color _startColor;
	private Color _targetColor;
	private bool _useEasing;
	
	public void init()
	{
		isEnabled = false;
		progressTime = 0.0f;
		motionTime = 0.0f;
		_useEasing = false;
	}
	
	public void start(float duration, Color startColorCode, Color targetColorCode, bool useEasing, string easingMethod, EasingType easingStyle)
	{
		isEnabled = true;
		progressTime = 0.0f;
		motionTime = duration;

		_startColor = startColorCode;
		_targetColor = targetColorCode;
		
		_useEasing = useEasing;
		_easingMethod = easingMethod;
		_easingStyle = easingStyle;
		
	}
	
	float _timeStep;
	Vector3 _v;
	
	public bool update(float delta, Monster mon)
	{
		progressTime += delta;
		if(progressTime >= motionTime)
		{
			isEnabled = false;
			mon.setColor(_targetColor);
			return false;
		}
		else
		{
			_timeStep = progressTime/motionTime;
			if(_useEasing) _timeStep = MiniTweenEasingType.getEasingValue(_timeStep, _easingMethod, _easingStyle);
			mon.setColor(   Util.colorLerp( _startColor, _targetColor, _timeStep) );
			return true;
		}
	}
}
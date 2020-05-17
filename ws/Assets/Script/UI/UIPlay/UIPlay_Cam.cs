using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public partial class UIPlay : UIBase 
{

	[HideInInspector]
	public Vector3 csCamPos = new Vector3();
	[HideInInspector]
	public Vector2 csCamCenter = new Vector2();
	[HideInInspector]
	public float csCamFov;
	public int csCamMoveType;
	public Vector2 csCamOffset = new Vector2();
	private Vector3 csCamPositionCalcVector = new Vector3();
	
	const int CAM_MOVE_FREEZE = 100; 
	
	public const int CAM_MOVE_STOP = 0;
	public const int CAM_MOVE_POSITION = 1;
	public const int CAM_MOVE_ROTATION = 2;


	public static Vector3 nowSkillEffectCamCenterPosition = new Vector3();
	public static Vector3 nowPlayerSkillTargetPosition = new Vector3();


	public enum SKILL_EFFECT_CAM_STATUS
	{
		None, Play, Close
	}

	public enum SKILL_EFFECT_CAM_TYPE
	{
		None, HeroSkill, UnitSkill, ChaserAttack
	}


	public static SKILL_EFFECT_CAM_STATUS nowSkillEffectCamStatus = SKILL_EFFECT_CAM_STATUS.None;
	public static SKILL_EFFECT_CAM_TYPE nowSkillEffectCamType = SKILL_EFFECT_CAM_TYPE.None;

	public static bool isPlayerSkillType = false;

	private static Vector3 _playerPositionAtTime2 = new Vector3();
	private static Vector3 _camPosAtTime2 = new Vector3();
	static bool _isFollowPlayerWhenSkillEffectCamIdle = false;
	public static bool isFollowPlayerWhenSkillEffectCamIdle
	{
		set
		{
			if(value && UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill && UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play)
			{
				value = true;
			}
			else value = false;
			
			_isFollowPlayerWhenSkillEffectCamIdle = value;
			
			if(value)
			{
				_playerPositionAtTime2 = GameManager.me.player.cTransform.position;
				_camPosAtTime2 = GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition;
			}
		}
		get
		{
			return _isFollowPlayerWhenSkillEffectCamIdle;
		}
	}
	
	public void setPlayerSkillEffectCamRuntimeOffsetPositionWhenIdle()
	{
		if(nowSkillEffectCamStatus == SKILL_EFFECT_CAM_STATUS.Play && targetChangeTweening == false && camAround.isEnabled == false && _isFollowPlayerWhenSkillEffectCamIdle && UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill && useUnitSkillCamTargetPosition == false)
		{
			gameCameraPosContainer.localPosition = _camPosAtTime2 + GameManager.me.player.cTransform.position - _playerPositionAtTime2;
		}
	}





	
	public void setCutSceneCameraRot(Transform target, bool setCamCenter, Vector2 centerPoint, float fov, bool setCamRot, Vector3 newCamRot)
	{
		useHandHeld = false; 
		camAround.isEnabled = false;
		
		clearTweener();
		
		if(target != null) cameraTarget = target;
		if(setCamCenter)
		{
			csCamPositionCalcVector.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * (centerPoint.x /GameManager.screenSize.x);
			csCamPositionCalcVector.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * (centerPoint.y /GameManager.screenSize.y);
			
			csCamOffset.x = (GameManager.screenSize.x * 0.5f - centerPoint.x)/GameManager.screenSize.x;
			csCamOffset.y = (GameManager.screenSize.y * 0.5f - centerPoint.y)/GameManager.screenSize.y;
			csCamOffset.x = GameManager.screenWidth * csCamOffset.x * gameCamera.rect.width;
			csCamOffset.y = GameManager.screenHeight * csCamOffset.y * gameCamera.rect.height;
		}
		
		if(fov > 0)
		{
			csCamFov = fov;
			gameCamera.fieldOfView = fov;
		}
		
		if(setCamRot)
		{
			_q = gameCamera.transform.rotation;
			_q.eulerAngles = newCamRot;
			gameCamera.transform.rotation = _q;
			
			_v = gameCamera.WorldToScreenPoint(cameraTarget.position);		
			_v.x += csCamOffset.x;
			_v.y += csCamOffset.y;
			_v = gameCamera.ScreenToWorldPoint(_v);			
			
			gameCamera.transform.rotation = Util.getLookRotationQuaternion(_v - gameCamera.transform.position);
		}
	}




	
	public void setCutSceneCamera(Transform target, bool setCamCenter, Vector2 centerPoint, bool setCamPos, Vector3 newCamPos, float fov, bool setCamRot, Vector3 newCamRot, int newMotionType = -1)
	{
		useHandHeld = false;
		camAround.isEnabled = false;

		usePlayerPositionOffsetWhenSkillEffectCam = false;

		clearTweener();
		
		if(target != null) cameraTarget = target;
		if(setCamCenter)
		{
			csCamPositionCalcVector.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * (centerPoint.x /GameManager.screenSize.x);
			csCamPositionCalcVector.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * (centerPoint.y /GameManager.screenSize.y);
			
			csCamOffset.x = (GameManager.screenSize.x * 0.5f - centerPoint.x)/GameManager.screenSize.x;
			csCamOffset.y = (GameManager.screenSize.y * 0.5f - centerPoint.y)/GameManager.screenSize.y;
			csCamOffset.x = GameManager.screenWidth * csCamOffset.x * gameCamera.rect.width;
			csCamOffset.y = GameManager.screenHeight * csCamOffset.y * gameCamera.rect.height;
		}
		
		if(fov > 0)
		{
			csCamFov = fov;
			gameCamera.fieldOfView = fov;
		}
		if(setCamPos)
		{
			if(nowSkillEffectCamStatus == SKILL_EFFECT_CAM_STATUS.Play)
			{
				newCamPos += getUnitSkillCamOffsetPosition();
			}

			csCamPos = newCamPos;
			gameCameraPosContainer.localPosition = newCamPos;
		}
		
		if(setCamRot)
		{
			_q = gameCamera.transform.rotation;
			_q.eulerAngles = newCamRot;
			gameCamera.transform.rotation = _q;
		}
		
		//		Debug.LogError("==== : " + CutSceneManager.cutScenePlayTime);
		//		Debug.LogError("target : " + target + "setCamPos : " + setCamPos + " setCamRot : " + setCamRot);
		if(target != null && setCamPos && setCamRot == false)
		{
			_v = gameCamera.WorldToScreenPoint(cameraTarget.position);		
			_v.x += csCamOffset.x;
			_v.y += csCamOffset.y;
			_v = gameCamera.ScreenToWorldPoint(_v);	
			gameCamera.transform.rotation = Util.getLookRotationQuaternion(_v - gameCamera.transform.position);
			
			//			Debug.LogError( gameCamera.transform.position + "    " + gameCamera.transform.rotation.eulerAngles);
		}
		
		if(newMotionType > -1)
		{
			csCamMoveType = newMotionType;
		}

		isFollowPlayerWhenSkillEffectCamIdle = true;
	}	
	
	
	
	
	bool targetChangeTweening = false;
	bool startCameraMove = false;
	int nextMotionType = 0;
	float motionStartTime;
	float motionTime;
	
	private Vector3 _startCameraPos;
	private Quaternion _startCameraRot;

	public bool useHandHeldAfterCameraMoving = false;
	public Vector3 handHeldAfterCameraMovingValue = new Vector3();



	private static Vector3 _playerPositionAtTime = new Vector3();
	static bool _usePlayerPositionOffsetWhenSkillEffectCam = false;
	public static bool usePlayerPositionOffsetWhenSkillEffectCam
	{
		set
		{
			if(value && UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill && UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play)
			{
				value = true;
			}
			else value = false;

			_usePlayerPositionOffsetWhenSkillEffectCam = value;

			if(value) _playerPositionAtTime = GameManager.me.player.cTransform.position;
		}
	}

	public Vector3 getPlayerSkillEffectCamRuntimeOffsetPosition()
	{
		if(nowSkillEffectCamStatus == SKILL_EFFECT_CAM_STATUS.Play && _usePlayerPositionOffsetWhenSkillEffectCam && UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill && useUnitSkillCamTargetPosition == false)
		{
			return GameManager.me.player.cTransform.position - _playerPositionAtTime;
		}
		_v.x = 0; _v.y = 0; _v.z = 0;
		return _v;
	}


	public static bool useUnitSkillCamTargetPosition = false;

	public Vector3 getUnitSkillCamOffsetPosition()
	{
		_v = gameCamera.transform.rotation.eulerAngles;
		_v2.x = 0; _v2.y = 0; _v2.z = 0;
		_q.eulerAngles = _v2;
		gameCameraContainer.rotation = _q;

		_q.eulerAngles = _v;
		gameCamera.transform.rotation = _q;

		_v.x = cameraTarget.position.x - gameCameraContainer.position.x;
		_v.y = 0;
		_v.z = 0;

		if(useUnitSkillCamTargetPosition == false)
		{
			return nowSkillEffectCamCenterPosition - GameManager.me.uiManager.uiPlay.gameCameraContainer.position;// + _v;
		}
		else
		{
			return nowPlayerSkillTargetPosition - GameManager.me.uiManager.uiPlay.gameCameraContainer.position;// + _v;
		}
	}

	public void setCutSceneCameraMove(Transform target, bool setCamCenter, Vector2 centerPoint, bool setCamPos, Vector3 newCamPos, bool setCamRot, Vector3 newCamRot, float fov, float motionTime, int newMotionType = -1, int nowMotionType = -1, string easingType = null)
	{
		useHandHeldAfterCameraMoving = false;

		usePlayerPositionOffsetWhenSkillEffectCam = true;
		isFollowPlayerWhenSkillEffectCamIdle = false;

		if(useHandHeld)
		{
			_v = handHeldEffect.tf.localPosition;
			gameCameraPosContainer.localPosition += _v;
			handHeldEffect.tf.localPosition = Vector3.zero;
		}

		useHandHeld = false;
		camAround.isEnabled = false;
		
		clearTweener();
		
		_motionDelta = 0.0f;
		
		if(target != null) cameraTarget = target;
		if(setCamCenter)
		{
			csCamPositionCalcVector.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * (centerPoint.x /GameManager.screenSize.x);
			csCamPositionCalcVector.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * (centerPoint.y /GameManager.screenSize.y);
			
			csCamOffset.x = (GameManager.screenSize.x * 0.5f - centerPoint.x)/GameManager.screenSize.x;
			csCamOffset.y = (GameManager.screenSize.y * 0.5f - centerPoint.y)/GameManager.screenSize.y;
			csCamOffset.x = GameManager.screenWidth * csCamOffset.x * gameCamera.rect.width;
			csCamOffset.y = GameManager.screenHeight * csCamOffset.y * gameCamera.rect.height;
		}
		
		this.motionTime = motionTime;
		
		if((target != null || setCamCenter)) //&& setCamPos == false)
		{
			motionStartTime = Time.time;
			targetChangeTweening = true;
			_startCameraPos = gameCamera.transform.localPosition;
			_startCameraRot = gameCamera.transform.rotation;
		}
		else targetChangeTweening = false;

		if(nowSkillEffectCamStatus == SKILL_EFFECT_CAM_STATUS.Play)
		{
			newCamPos += getUnitSkillCamOffsetPosition();
		}

		if(fov > 0) tweener.Add(MiniTweenerFOV.start(gameCamera, fov, motionTime, easingType));
		if(setCamPos) tweener.Add(MiniTweenerLocalPosition.start(gameCameraPosContainer, newCamPos, motionTime,easingType));
		if(setCamRot) tweener.Add(MiniTweenerCamRotation.start(gameCamera, newCamRot, motionTime, easingType));

		nextMotionType = csCamMoveType;
		if(newMotionType > -1) nextMotionType = newMotionType;
		if(nowMotionType > -1) csCamMoveType = nowMotionType;
		
		System.Action<int> callbackFunc = onCompleteCameraMove;
		tweener.Add(MiniTweenerDelayMethod.start(motionTime, 0.0f, callbackFunc, nextMotionType));
		
	}
	
	void onCompleteCameraMove(int newMotionType)
	{
		csCamMoveType = newMotionType;

		targetChangeTweening = false;

		if(newMotionType >= 100)
		{
			GameManager.me.cutSceneManager.closeOpenCutScene();
		}
		else
		{
			if(useHandHeldAfterCameraMoving)
			{
				setHandHeldEffect(handHeldAfterCameraMovingValue);
			}
		}

		isFollowPlayerWhenSkillEffectCamIdle = true;
	}
	
	
	List<MiniTweener> tweener = new List<MiniTweener>();
	
	
	private Vector3 _camTargetPos;	
	private int tweenCount = 0;
	private float _motionDelta = 0.0f;
	private Vector3 _targetCameraPos;
	
	private float _newFov;
	private float prevPlayTime = 0.0f;
	
	private float fov;
	
	private Vector3 _v;
	private Quaternion _q;
	private Quaternion _q2;
	private Vector3 _v2;
	private Vector3 _v3;
	
	
	float _csDelta = 0.0f;


	bool _useHandHeld = false;
	public bool useHandHeld
	{
		set
		{
			_useHandHeld = value;
			handHeldEffect.enabled = value;
			if(value == false) handHeldEffect.reset();
		}
		get
		{
			return _useHandHeld;
		}
	}
	
	public UIHandHeldEffect handHeldEffect;
	
	public void setHandHeldEffect(Vector3 eff)
	{
		useHandHeld = true;
		handHeldEffect.start(eff);
	}



	public CamAroundPlayer camAround = new CamAroundPlayer();




	public void playCSCam()
	{
		if(camAround.isEnabled)
		{
			camAround.update();
			return;
		}

		if(cameraTarget != null)
		{
			tweenCount = tweener.Count;
			
			if(CutSceneManager.nowOpenCutScene) _csDelta = CutSceneManager.cutSceneDeltaTime;
			else _csDelta = Time.smoothDeltaTime;
			
			for(i = tweener.Count -1; i >= 0; --i)
			{
				tweener[i].update(_csDelta);
				
				if(tweener[i].isComplete == true)
				{
					if(tweenCount == 0) targetChangeTweening =  false;
					tweener[i].delete();
					tweener.RemoveAt(i);
				}
			}

			setPlayerSkillEffectCamRuntimeOffsetPositionWhenIdle();

			switch(csCamMoveType)
			{
			case CAM_MOVE_POSITION:
				
				_camTargetPos = cameraTarget.position;
				
				_v = gameCamera.WorldToScreenPoint(_camTargetPos);
				csCamPositionCalcVector.z = _v.z;
				
				_v2 = gameCamera.ScreenToWorldPoint(csCamPositionCalcVector);
				
				_v3 = gameCamera.transform.localPosition;
				_v3.x -= (_v2.x - _camTargetPos.x);
				_v3.y -= (_v2.y - _camTargetPos.y);			
				
				if(targetChangeTweening)
				{
					_motionDelta += _csDelta;///motionTime;
					_v = gameCamera.transform.localPosition = Vector3.Lerp(_startCameraPos, _v3, _motionDelta/motionTime);//(Time.time-motionStartTime));//_v3;		motionDelta);//			
				}
				else
				{
					_v = Vector3.Lerp(gameCamera.transform.localPosition, _v3, Time.deltaTime * 7.0f);//_v3;					
				}
				
				gameCameraPosContainer.localPosition += _v;
				_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;
				gameCamera.transform.localPosition = _v;				
				
				break;
				
			case CAM_MOVE_ROTATION:
				
				_v = gameCamera.WorldToScreenPoint(cameraTarget.position);		
				_v.x += csCamOffset.x;
				_v.y += csCamOffset.y;
				_v = gameCamera.ScreenToWorldPoint(_v);			
				
				if(targetChangeTweening)
				{
					_motionDelta += _csDelta;///motionTime;
					gameCamera.transform.rotation = Quaternion.Slerp(_startCameraRot, Util.getLookRotationQuaternion(_v - gameCamera.transform.position), _motionDelta/motionTime);//(Time.time-motionStartTime));
				}
				else
				{
					//gameCamera.transform.rotation = Quaternion.LookRotation((_v) - gameCamera.transform.position);
					gameCamera.transform.rotation = Quaternion.Slerp(gameCamera.transform.rotation, Util.getLookRotationQuaternion(_v - gameCamera.transform.position), Time.deltaTime * 7.0f);//(Time.time-motionStartTime));
				}	
				
				break;
				
			case CAM_MOVE_FREEZE:
				break;
				
			default:
				_v = gameCamera.transform.localPosition;
				gameCameraPosContainer.localPosition += _v;
				_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;
				gameCamera.transform.localPosition = _v;
				
				//				gameCameraPosContainer.transform.localPosition = Vector3.Lerp(gameCameraPosContainer.transform.localPosition, 
				//				                                                              gameCameraPosContainer.transform.localPosition + gameCamera.transform.localPosition, 0.8f);
				//				_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;
				//				gameCamera.transform.localPosition = Vector3.Lerp(gameCamera.transform.localPosition, _v, 0.9f);
				break;
			}
		}
	}	



	public Vector3 _cameraContainerPosAtBackToGameCamStarting = new Vector3();
	public static bool showMapAfterBackToGameCamFromSkillCam = false;

	public void backToGameCamFromSkillCam(float moveTime, string easing, bool showMap)
	{
		isFollowPlayerWhenSkillEffectCamIdle = false;
		showMapAfterBackToGameCamFromSkillCam = false;

		if(moveTime <= 0.0f)
		{
			GameManager.me.cutSceneManager.useCutSceneCamera = false;
			resetCamera();
			changeCamera(true);

			if(showMap)
			{
				if(GameManager.me.mapManager.inGameMap != null)
				{
					GameManager.me.mapManager.createBackground(GameManager.me.mapManager.inGameMap.id,true);
				}
			}
		}
		else
		{
			//_v = getCameraCenterPosition(GameManager.me.player.cTransform, 0.22f, 0.4f, 15);
			_v.x = 296.5178f;
			_v.y = 1165.279f;
			_v.z = -2168.0f;
			nowSkillEffectCamStatus =  SKILL_EFFECT_CAM_STATUS.Close;
			showMapAfterBackToGameCamFromSkillCam = showMap;
			
			_cameraContainerPosAtBackToGameCamStarting = gameCameraContainer.transform.position;
			
			setCutSceneCameraMove(GameManager.me.player.cTransform,false,Vector2.zero,true,_v,true,new Vector3(27,0,0),15,moveTime,100,-1,easing);
		}
	}


	
	public void playCam()
	{
		if(_minimapTouched)
		{
			_v = cameraTarget.position;
			_v.x = _minimapPointPosition;
			_v.z = 0.0f;
			
			// ==== 카메라가 지연되어 캐릭터를 쫓아가는 효과. ==== //
			_v2 = gameCameraContainer.position;
			
			_tempF = (_v.x - _v2.x);
			
			_v2.x += _tempF;
			_v.x = _v2.x;			
			gameCameraContainer.position = Vector3.Lerp(gameCameraContainer.position,_v,0.95f);//_v;
			return;
		}
		
		if(cameraTarget == null) return;
		_v = cameraTarget.position;
		_v.z = 0.0f;
		
		// ==== 카메라가 지연되어 캐릭터를 쫓아가는 효과. ==== //
		_v2 = gameCameraContainer.position;
		
		_tempF = (_v.x - _v2.x);
		
		if(GameManager.me.player.moveState != Player.MoveState.Stop)
		{
			_v2.x += _tempF * (GameManager.globalDeltaTime) * GameManager.info.setupData.defaultPlayCamSpringValue[0];
			_v.x = _v2.x;			
			gameCameraContainer.position = _v;
		}
		
		else
		{
			_v2.x += _tempF * (GameManager.globalDeltaTime) * GameManager.info.setupData.defaultPlayCamSpringValue[1];
			_v.x = _v2.x;
			gameCameraContainer.position = _v;
		}
		
		// ==== 캐릭터 이동시 화면 움직이는 효과 ==== //
		
		_v = cameraTarget.position;
		_v2 = gameCameraContainer.position;		
		
		_tempF = (_v.x - _v2.x);
		
		_q = gameCameraContainer.rotation;
		_q2 = gameCameraContainer.rotation;
		_v2 =  _q.eulerAngles;		
		_v2.x = rx;
		
		_tempF *= GameManager.globalDeltaTime * 2.0f;
		
		_v2.y = ry + _tempF;
		
		// 최대 회전각도를 제한한다.
		if(_v2.y < 180.0f && _v2.y > 8.0f) _v2.y = 8.0f;
		else if(_v2.y > 180.0f && _v2.y < 356.0f) _v2.y = 352.0f;
		else if(_v2.y < -8.0f) _v2.y = -8.0f;
		
		_q.eulerAngles = _v2;		
		
		if(GameManager.me.player.moveState != Player.MoveState.Stop)
		{
			_v = Quaternion.Slerp(_q2, _q, Time.smoothDeltaTime * 1.5f * (MapManager.useZoomCamera?0.3f:1.0f)).eulerAngles;
		}
		else
		{
			_v = Quaternion.Slerp(_q2, _defaultRotation, Time.smoothDeltaTime * (MapManager.useZoomCamera?0.3f:1.0f)).eulerAngles;
		}

		_q.eulerAngles = _v;
		gameCameraContainer.rotation = _q;

		//==== ZOOM IN - OUT ====//
		
		//float dist = VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, GameManager.me.stageManager.heroMonster[0].cTransformPosition.x);
		//dist *= 0.01f;
		//if(dist > 8.0f) dist = 8.0f;
		//GameManager.me.persCamera.fieldOfView = fov + dist - 5.0f;

		if(MapManager.useZoomCamera)
		{

			_newFov = VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, MapManager.zoomCameraTargetX);
			_newFov /= 55.0f;
			if(_newFov <= 15.0f) _newFov = 15.0f;
			else if(_newFov >= 21.3f) _newFov = 21.3f;
			{
				gameCamera.fieldOfView = Mathf.Lerp(gameCamera.fieldOfView, _newFov, Time.smoothDeltaTime * 10.0f);

				_v = gameCameraPosContainer.transform.localPosition;
				_v.x = 296.5178f;

				if(_newFov > 15.0f)
				{
					_v.x += (VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, MapManager.zoomCameraTargetX))*0.1f;
				}

				gameCameraPosContainer.localPosition = Vector3.Lerp(gameCameraPosContainer.localPosition,_v, Time.smoothDeltaTime * 3.0f);
			}

			/*
			float dist = VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, MapManager.zoomCameraTarge);
			dist *= 0.005f;
			if(dist > 4.0f) dist = 4.0f;
		
			_newFov = fov + dist - 2.0f;
		
			if(gameCamera.fieldOfView - _newFov > Time.smoothDeltaTime * 2.0f)
			{
				gameCamera.fieldOfView -= Time.smoothDeltaTime * 2.0f;
			}
			else if(gameCamera.fieldOfView - _newFov < -Time.smoothDeltaTime * 2.0f)
			{
				gameCamera.fieldOfView += Time.smoothDeltaTime * 2.0f;
			}
			else
			{
				gameCamera.fieldOfView = _newFov;		
			}
			*/
		}

	}
	


	public void resetToChallengeModeZoomDefaultCamera()
	{
		if(MapManager.useZoomCamera)
		{
//			Debug.LogError(MapManager.useZoomCamera);

			_newFov = VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, MapManager.zoomCameraTargetX);
			_newFov /= 55.0f;
			if(_newFov <= 15.0f) _newFov = 15.0f;
			else if(_newFov >= 21.3f) _newFov = 21.3f;

			gameCamera.fieldOfView = _newFov;
			
			_v = gameCameraPosContainer.localPosition;
			_v.x = 296.5178f;
			
			if(_newFov > 15.0f)
			{
				_v.x += (VectorUtil.Distance(GameManager.me.player.cTransformPosition.x, MapManager.zoomCameraTargetX))*0.1f;
			}

			gameCameraPosContainer.localPosition = _v;
		}
	}


	
	
	

	
	private float _setCameraCenterPosContainerPosX = 0.0f; // 줌인 줌아웃때 기준점으로 쓸 녀석이다...
	Vector3 calc = new Vector3();
	private void setCameraCenter(bool settingCamRightNow = false, float xPos = 0.24f, float yPos = 0.4f)
	{
		if(cameraTarget == null) return;
		
		if(settingCamRightNow)
		{
			_v = cameraTarget.position;
			gameCameraContainer.position = _v;
		}		


		calc.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * xPos;//(centerPoint.x /GameManager.screenSize.x);
		calc.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * yPos;//(centerPoint.y /GameManager.screenSize.y);
		
		_camTargetPos = cameraTarget.position;
		// 어차피 화면 가운데에 놓으려고 하니까.. 캐릭터 z값은 무시해주자.
		_camTargetPos.z = 0.0f;
		
		_v = gameCamera.WorldToScreenPoint(_camTargetPos);
		calc.z = _v.z;
		
		_v2 = gameCamera.ScreenToWorldPoint(calc);
		// 화면의 특정점의 위치를 월드 좌표로 변환.
		
		_v3 = gameCameraPosContainer.localPosition;
		_v3.x -= (_v2.x - _camTargetPos.x);
		_v3.y -= (_v2.y - _camTargetPos.y);
		gameCameraPosContainer.localPosition = _v3;	
		_setCameraCenterPosContainerPosX = _v3.x;

	}	
	
	
	public Vector3 getCameraCenterPosition(Transform target, float xPos, float yPos, float fov)
	{
		calc.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * xPos;//(centerPoint.x /GameManager.screenSize.x);
		calc.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * yPos;//(centerPoint.y /GameManager.screenSize.y);
		_camTargetPos = target.position;
		
		// 어차피 화면 가운데에 놓으려고 하니까.. 캐릭터 z값은 무시해주자.
		_camTargetPos.z = 0.0f;
		
		
		float oriFov = gameCamera.fieldOfView;
		if(fov > 0) gameCamera.fieldOfView = fov;
		
		_v = gameCamera.WorldToScreenPoint(_camTargetPos);
		calc.z = _v.z;
		_v2 = gameCamera.ScreenToWorldPoint(calc);
		_v3 = gameCameraPosContainer.localPosition;
		_v3.x -= (_v2.x - _camTargetPos.x);
		_v3.y -= (_v2.y - _camTargetPos.y);
		
		gameCamera.fieldOfView = oriFov;
		
		return _v3;
	}
	
	
	public Vector3 getCameraCenterPosition(Vector3 targetPos, float xPos, float yPos, float fov)
	{
		calc.x = GameManager.screenWidth * gameCamera.rect.x + GameManager.screenWidth * gameCamera.rect.width * xPos;//(centerPoint.x /GameManager.screenSize.x);
		calc.y = GameManager.screenHeight * gameCamera.rect.y + GameManager.screenHeight * gameCamera.rect.height * yPos;//(centerPoint.y /GameManager.screenSize.y);
		
		// 어차피 화면 가운데에 놓으려고 하니까.. 캐릭터 z값은 무시해주자.
		targetPos.z = 0.0f;
		
		float oriFov = gameCamera.fieldOfView;
		if(fov > 0) gameCamera.fieldOfView = fov;
		
		_v = gameCamera.WorldToScreenPoint(targetPos);
		calc.z = _v.z;
		_v2 = gameCamera.ScreenToWorldPoint(calc);
		_v3 = gameCameraPosContainer.localPosition;
		_v3.x -= (_v2.x - targetPos.x);
		_v3.y -= (_v2.y - targetPos.y);
		
		gameCamera.fieldOfView = oriFov;
		
		return _v3;
	}
	
	
	
	
	public int resetCamBlock = 0;
	
	public void resetCamera()
	{
		useHandHeld = false;
		camAround.isEnabled = false;

		clearTweener();
		csCamMoveType = CAM_MOVE_STOP;
		
		if(resetCamBlock > 0 && GameManager.me.cutSceneManager.useCutSceneCamera)
		{
			Debug.LogError("BLOCK RESET!");
			resetCamBlock = 0;
			return;
		}
		
		//Debug.LogError("==== resetCamera!!! ==== ");

		cameraTarget = GameManager.me.player.cTransform;
		
		_v = gameCameraContainer.position;
		_v.x = 0.0f; _v.y = 0.0f; _v.z = 0.0f;
		gameCameraContainer.position = _v;		
		
		_q = gameCameraContainer.transform.rotation;
		_v = _q.eulerAngles;_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;_q.eulerAngles = _v;
		gameCameraContainer.transform.rotation = _q;	
		
		_v = GameManager.me.gameCamera.transform.parent.transform.localPosition;
		_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;
		GameManager.me.gameCamera.transform.parent.transform.localPosition = _v;		
		
		_v = gameCameraPosContainer.localPosition;
		_v.x = 296.5178f;_v.y = 1165.279f;_v.z = -2168.0f;
		gameCameraPosContainer.localPosition = _v;		

		
		_v = gameCamera.transform.localPosition;
		_v.x = 0;_v.y = 0;_v.z = 0;
		gameCamera.transform.localPosition = _v;				
		
		//GameManager.me.gameCamera.fieldOfView = 10;
		
		camNumber = 0;
		
		if(CutSceneManager.nowOpenCutScene == false)
		{
			changeCamera(true);
			//playCam();	
		}
		else
		{
			changeCamera(false);
		}
	}	
	
	int camNumber = 0;
	
	private float rx = 0;
	private float ry = 0;
	private Quaternion _defaultRotation;
	
	public void changeCamera(bool settingCameraRightNow = false)
	{
		_v.x = 296.5178f;
		_v.y = 1165.279f;
		_v.z = -2168.0f;

		gameCameraPosContainer.localPosition = _v;
		
		_q = gameCameraContainer.rotation;
		_v = GameManager.info.setupData.cameraPreset[camNumber].rotation;
		_q.eulerAngles = _v;
		gameCameraContainer.rotation = _q;
		_defaultRotation = _q;
		
		
		_q = GameManager.me.gameCamera.transform.localRotation;
		_v = _q.eulerAngles;			
		_v.x = 27.0f;_v.y = 0.0f;_v.z = 0.0f;_q.eulerAngles = _v;
		GameManager.me.gameCamera.transform.localRotation = _q;			
		
		//fov = GameManager.info.setupData.cameraPreset[camNumber].fov;
		GameManager.me.gameCamera.fieldOfView = 15;


		if(CutSceneManager.nowOpenCutScene)
		{
			setCameraCenter(settingCameraRightNow);	
		}
		else
		{
			if(settingCameraRightNow && cameraTarget != null)
			{
				_v = cameraTarget.position;
				gameCameraContainer.position = _v;
			}
		}

	}		
}





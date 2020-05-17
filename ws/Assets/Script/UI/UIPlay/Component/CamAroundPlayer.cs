using UnityEngine;

public class CamAroundPlayer
{
	public CamAroundPlayer ()
	{
	}


	Vector3 _targetStartPos = new Vector3();
	Vector3 _targetEndPos = new Vector3();
	bool _useTargetEndPos = false;

	Vector3 _targetStartScreenPos = new Vector3();
	Vector3 _targetEndScreenPos = new Vector3();
	bool _useTargetEndScreenPos = false;


	float _startAngle = 0.0f;
	float _nowAngle = 0.0f;
	float _startYpos = 0.0f;
	float _startDist = 0.0f;

	float _progressTime = 0.0f;
	float _time = 0.0f;

	Vector3 csCamOffset;

	bool useEasing = false;
	string easingMethod = "";
	EasingType easingStyle;

	bool useYPosMove = false;
	float _endYpos = 0.0f;

	float _distTweeningValue = 1.0f;

	float _endAngle = 0.0f;

	Vector3 _v;

	public void start(Vector3 targetStartPos,  // 속성1> 기준점좌표 [x,y,z(/x,y,z)] : 기준점 시작좌표 / 기준점 끝좌표(생략가능)
	                  Vector3 targetEndPos,
	                  bool useTargetEndPos,

	                  Vector3 targetStartScreenPos,  // 속성2> 화면좌표A [x,y(/x,y)] : 기준점을 위치시킬 화면좌표(시작) / 기준점을 위치시킬 화면좌표(끝)
	                  Vector3 targetEndScreenPos,
	                  bool useTargetEndScreenPos, 

	                  float duration, // 속성3> 카메라이동시간 (sec) : 설정된 시간동안 카메라 이동

	                  int finalCamYpos, //속성4> 최종카메라 높이(y)값 : 현재 높이값에서 설정된 높이값으로 변경 (0 세팅시 높이 변화 없음)
	                  float distanceTweeningValue, //속성5> 기준점과의 거리변경(%) : 기준점과의 현재거리 대비 N%만큼 거리를 줄이거나 늘림 (y축은 무시, x,z 값만 가지고 기준점과의 거리를 계산)
	                  int rotValue, //속성6> 회전각도 : 오른쪽 (or 왼쪽) 이동시 양수 / 반대쪽 이동은 음수로 지정 (예 : -540 세팅시 왼쪽으로 한바퀴반 돌기)
	                  string easingType)  // 속성7> 가속도 : CAM_MOVE 와 동일
	{

		isEnabled = true;


		if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play)
		{
			targetStartPos += GameManager.me.uiManager.uiPlay.getUnitSkillCamOffsetPosition() + GameManager.me.uiManager.uiPlay.gameCameraContainer.position;
			targetEndPos += GameManager.me.uiManager.uiPlay.getUnitSkillCamOffsetPosition() + GameManager.me.uiManager.uiPlay.gameCameraContainer.position;
		}

		UIPlay.isFollowPlayerWhenSkillEffectCamIdle = false;


		_targetStartPos = targetStartPos;
		_targetEndPos = targetEndPos;
		_useTargetEndPos = useTargetEndPos;

		_targetStartScreenPos = targetStartScreenPos;
		_targetEndScreenPos = targetEndScreenPos;
		_useTargetEndScreenPos = useTargetEndScreenPos;


		if(targetStartScreenPos.z < 0)
		{
			_targetStartScreenPos = GameManager.me.gameCamera.WorldToScreenPoint(_targetStartPos);
			_targetStartScreenPos = Util.screenPositionWithCamViewRect(_targetStartScreenPos);
		}


		_v = GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition;

		if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play)
		{
			_v += GameManager.me.uiManager.uiPlay.gameCameraContainer.position;
		}

		_startAngle = Util.getFloatAngleBetweenXZ(_v, targetStartPos);
		_startDist = VectorUtil.DistanceXZ(_v, targetStartPos);


		csCamOffset.x = (GameManager.screenSize.x * 0.5f - _targetStartScreenPos.x)/GameManager.screenSize.x;
		csCamOffset.y = (GameManager.screenSize.y * 0.5f - _targetStartScreenPos.y)/GameManager.screenSize.y;
		csCamOffset.x = GameManager.screenWidth * csCamOffset.x * GameManager.me.uiManager.uiPlay.gameCamera.rect.width;
		csCamOffset.y = GameManager.screenHeight * csCamOffset.y * GameManager.me.uiManager.uiPlay.gameCamera.rect.height;

		useEasing = false;

		_progressTime = 0.0f;
		_time = duration;

		_startYpos = GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition.y;

		_endYpos = finalCamYpos;

		useYPosMove = (finalCamYpos != 0);

		_distTweeningValue = distanceTweeningValue;

		_endAngle = _startAngle + rotValue;

		if(string.IsNullOrEmpty( easingType ) == false)
		{
			useEasing = true;
			string[] e = easingType.Split(',');
			easingStyle = MiniTweenEasingType.getEasingType(e[1]);
			easingMethod = e[0];
		}

	}
	

	float _nowDist = 0.0f;
	Vector3 _nowTargetPosition = new Vector3();
	Vector3 _nowScreenTargetPosition = new Vector3();
	float timeStep;
	float _csDelta = 0.0f;

	public void update()
	{
		if(CutSceneManager.nowOpenCutScene) _csDelta = CutSceneManager.cutSceneDeltaTime;
		else _csDelta = Time.smoothDeltaTime;

		_progressTime += _csDelta;///motionTime;

		timeStep = _progressTime/_time;

		if(timeStep >= 1.0f)
		{
			timeStep = 1.0f;
			isEnabled = false;
		}

		if(useEasing) timeStep = MiniTweenEasingType.getEasingValue(timeStep, easingMethod, easingStyle);

		_nowDist = _startDist * Mathf.Lerp(1.0f, _distTweeningValue, timeStep);
		_nowAngle = Mathf.Lerp(_startAngle, _endAngle, timeStep);

		while(_nowAngle < 0)
		{
			_nowAngle += 360.0f;
		}

		if(_useTargetEndPos)
		{
			_nowTargetPosition = Vector3.Lerp(_targetStartPos, _targetEndPos, timeStep);
		}
		else _nowTargetPosition = _targetStartPos;


//		Debug.Log(_nowTargetPosition);

		_nowTargetPosition += GameManager.me.uiManager.uiPlay.getPlayerSkillEffectCamRuntimeOffsetPosition();


		_v = Util.getPositionByAngleAndDistanceXZWithoutTable( _nowAngle , _nowDist);

		if(useYPosMove)
		{
			_v.y = Mathf.Lerp(_startYpos, _endYpos, timeStep);
		}
		else _v.y = _startYpos;

		_v.x = _nowTargetPosition.x - _v.x;
		_v.z = _nowTargetPosition.z - _v.z;

		if(UIPlay.nowSkillEffectCamStatus == UIPlay.SKILL_EFFECT_CAM_STATUS.Play)
		{
			GameManager.me.uiManager.uiPlay.gameCameraPosContainer.position = _v;
		}
		else
		{
			GameManager.me.uiManager.uiPlay.gameCameraPosContainer.localPosition = _v;
		}


		if(_useTargetEndScreenPos)
		{
			_nowScreenTargetPosition = Vector3.Lerp(_targetStartScreenPos, _targetEndScreenPos, timeStep);

			csCamOffset.x = (GameManager.screenSize.x * 0.5f - _nowScreenTargetPosition.x)/GameManager.screenSize.x;
			csCamOffset.y = (GameManager.screenSize.y * 0.5f - _nowScreenTargetPosition.y)/GameManager.screenSize.y;
			csCamOffset.x = GameManager.screenWidth * csCamOffset.x * GameManager.me.uiManager.uiPlay.gameCamera.rect.width;
			csCamOffset.y = GameManager.screenHeight * csCamOffset.y * GameManager.me.uiManager.uiPlay.gameCamera.rect.height;
		}

		
		_v = GameManager.me.uiManager.uiPlay.gameCamera.WorldToScreenPoint(_nowTargetPosition);		
		_v.x += csCamOffset.x;
		_v.y += csCamOffset.y;
		_v = GameManager.me.uiManager.uiPlay.gameCamera.ScreenToWorldPoint(_v);			
		

		GameManager.me.uiManager.uiPlay.gameCamera.transform.rotation = Util.getLookRotationQuaternion(_v - GameManager.me.uiManager.uiPlay.gameCamera.transform.position);
		
		//gameCamera.transform.rotation = Util.getLookRotationQuaternion( Vector3.zero - gameCameraPosContainer.position);

		if(isEnabled == false && timeStep >= 1.0f) UIPlay.isFollowPlayerWhenSkillEffectCamIdle = true;
	}


	void clear()
	{

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
			if(value == false)
			{
				clear();
			}
			else
			{
				GameManager.me.uiManager.uiPlay.useHandHeld = false;
				GameManager.me.uiManager.uiPlay.clearTweener();
			}
		}
	}

}


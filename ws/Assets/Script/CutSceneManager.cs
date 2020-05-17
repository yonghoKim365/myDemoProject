using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class CutSceneManager : MonoBehaviour 
{
	public bool isCutSceneRecordMode = false;
	public int recordAct = 1;
	public int recordStage = 1;
	public int recordRound = 1;

	public void autoCutScenePlayer()
	{
		StartCoroutine(startAutoCutScene());
	}

	IEnumerator startAutoCutScene()
	{
		yield return new WaitForSeconds(5.0f);

		string testId = "A"+recordAct+"S"+recordStage+"R"+recordRound;

		if(GameManager.info.roundData.ContainsKey(testId) == false || (GameManager.me.stageManager.nowRound != null && GameManager.me.stageManager.nowRound.id == testId))
		{
			yield break;
		}
		else
		{
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[testId],GameType.Mode.Epic);
			GameManager.me.uiManager.showLoading();
			GameManager.me.startGame();
			
			++recordRound;
			
			if(recordStage < 3)
			{
				if(recordRound > 3)
				{
					++recordStage;
					recordRound = 1;
				}
			}
			else if(recordStage < 5)
			{
				if(recordRound > 4)
				{
					++recordStage;
					recordRound = 1;
				}		
			}
			else
			{
				if(recordRound > 5)
				{
					++recordStage;
					recordRound = 1;
				}				
			}
			
			if(recordStage > 5)
			{
				recordStage = 1;
				++recordAct;
			}

		}

	}




	public CharacterDialogBox dialogBox;

	public UIButton btnCutSceneSkip;

	public UIButton btnCutSceneSpeed, btnCutSceneSpeedPress;
	public UISprite spReplaySpeed;
	public int nowCutSceneSpeed = 1;


	public bool blockControl = false;

	public static float introCutSceneStartTime = 0.0f;
	public static float cutScenePlayTime = 0.0f;
	public float cutSceneEventEndTime = 0.0f;

	public float cutSceneEventFrameEndTime = 0.0f;
	
	public enum Status
	{
		NONE, PREPARE, CUTSCENE, INGAME_CUTSCENE
	}
	
	public Status status;
	
	public CutSceneData[] readyCutScene = null;
	public CutSceneData currentCutScene = null;
	
	int cutSceneCount = 0;

	public bool hasCutScene = false;
	public bool isSlowMode = false;

	// 0이면 슬로우에 관계없이 컷씬 이벤트 시간은 정상 작동. 1이면 따라서 느려진다.
	public int slowModeTimeProgressType = 0;

	public static float cutSceneDeltaTime = 0.0f;

	public UILabel lbCutSceneId;
	public UILabel tfPlayTime;
	private float _prevCutSceneTime = 0.0f;
	private Vector2 _touchPoint;
	
	
	public Dictionary<string, CutSceneCharacterData> cutSceneCharacterDic = new Dictionary<string, CutSceneCharacterData>(StringComparer.Ordinal);
	public Dictionary<string, List<Monster>> cutScenePlayCharacter = new Dictionary<string, List<Monster>>(StringComparer.Ordinal);	
	
	
	public Dictionary<string, CutSceneEffectData> cutSceneEffectDic = new Dictionary<string, CutSceneEffectData>(StringComparer.Ordinal);
	public Dictionary<string, List<Effect>> cutScenePlayEffect = new Dictionary<string, List<Effect>>(StringComparer.Ordinal);		
	
	
	private float _pauseTime = 0.0f;
	private float _pauseOriginalTimeScale = 1.0f;
	public static float realDeltaTime = 0.0f;
	private float _prevTime = 0.0f;	
	
	
	public static bool nowOpenCutScene = false;

	public enum CutSceneType
	{
		Pre,After,UnitCam,Close
	}

	public static CutSceneType nowOpenCutSceneType = CutSceneType.Pre;

	
	private bool _useCutSceneCamera = false;
	public bool useCutSceneCamera
	{
		set
		{
			_useCutSceneCamera = value;

			if(UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None && 
			   UIPlay.nowSkillEffectCamType != UIPlay.SKILL_EFFECT_CAM_TYPE.None)
			{
				GameManager.me.gameCamera.nearClipPlane = (value)?1.5f:1800.0f;//1200.0f; // 
				GameManager.me.gameCamera.farClipPlane = (value)?100000.0f:5000.0f;
			}
			else
			{
				GameManager.me.gameCamera.nearClipPlane = (value)?1.5f:1800.0f;//1200.0f; // 
				GameManager.me.gameCamera.farClipPlane = (value)?100000.0f:5000.0f;
			}


#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				GameManager.me.gameCamera.nearClipPlane = (value)?1.5f:1800.0f;//1200.0f; // 
				GameManager.me.gameCamera.farClipPlane = (value)?100000.0f:5000.0f;
			}
#endif
			
			//GameManager.me.gameCamera.transparencySortMode = TransparencySortMode.Perspective;
			//GameManager.me.gameCamera.backgroundColor = new Color();
		}
		get
		{
			return _useCutSceneCamera;
		}		
	}






	public bool useUnitCamCamra
	{
		set
		{
			_useCutSceneCamera = value;
			
			GameManager.me.gameCamera.nearClipPlane = (value)?1.5f:1800.0f;//1200.0f; // 
			GameManager.me.gameCamera.farClipPlane = (value)?100000.0f:5000.0f;
			
			
			#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				GameManager.me.gameCamera.nearClipPlane = 1.5f;
				GameManager.me.gameCamera.farClipPlane = 100000.0f;
			}
			#endif


		}
		get
		{
			return _useCutSceneCamera;
		}		
	}





	public void useCutSceneCameraWithoutClipSetting()
	{
		_useCutSceneCamera = true;
		GameManager.me.gameCamera.nearClipPlane = 1800.0f;//1200.0f; // 
		GameManager.me.gameCamera.farClipPlane = 5000.0f;
	}

	
	// 얘가 켜져있으면 인게임시라도 캐릭터들이 동작을 하지 않는다.
	public bool stopLoopInGame = false;
	
	
	public void initCutScene()
	{
		nowOpenCutScene = false;
		currentCutScene = null;
		cutSceneCount = 0;
		readyCutScene = null;
		hasCutScene = false;
		blockControl = false;
		nowOpenCutSceneType = CutSceneManager.CutSceneType.Close;

		init();
		
		if(string.IsNullOrEmpty( GameManager.me.stageManager.getCutSceneId() ) == false)
		{
			string[] str = GameManager.me.stageManager.getCutSceneId().Split(',');
			readyCutScene = new CutSceneData[str.Length];
			
			cutSceneCount = str.Length;
			for(int i = 0; i < cutSceneCount; ++i)
			{
				readyCutScene[i] = GameManager.info.cutSceneData[str[i]];
				readyCutScene[i].init();
				hasCutScene = true;	
				status = Status.PREPARE;
			}
			return;
			
		}
		
		status = Status.NONE;
	}
	
	
	void init()
	{
		spReplaySpeed.spriteName = "img_replay_speed_1";
		isSkipMode = false;
		useCutSceneCamera = false;
		tfPlayTime.text = ""; // 임시코드.
		cutScenePlayTime = 0.0f;
		cutSceneEventEndTime = 0.0f;
		_prevCutSceneTime = 0.0f;		
		_prevTime = Time.realtimeSinceStartup;
		isSlowMode = false;
		slowModeTimeProgressType = 0;
		stopLoopInGame = false;
		cutScenePlaySpeed = 1.0f;
		cutSceneEventEndTime = 0.0f;
		btnCutSceneSkip.gameObject.SetActive(false);
		btnCutSceneSpeed.gameObject.SetActive(false);
		dialogBox.gameObject.SetActive(false);

		introCutSceneStartTime = 0.0f;

		nowCutSceneSpeed = 1;
		prevCutSceneSpeed = 1;

		GameManager.me.uiManager.uiPlay.clearTweener();

	}
	

	void Awake()
	{
		UIEventListener.Get(btnCutSceneSkip.gameObject).onClick = onSkipCutScene;
		UIEventListener.Get(btnCutSceneSpeed.gameObject).onClick = onChangeCutSceneSpeed;

		UIEventListener.Get(btnCutSceneSpeedPress.gameObject).onPress = onChangeCutSceneSpeedPress;
	}


	public bool isSkipMode = false;

	public void onSkipCutScene(GameObject go)
	{
		isSkipMode = true;
		GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_OUT, cutSceneSkipFadeOutTime);
		btnCutSceneSkip.gameObject.SetActive(false);
		btnCutSceneSpeed.gameObject.SetActive(false);
		checkSkipTime = Time.realtimeSinceStartup + cutSceneSkipFadeOutTime;
		StartCoroutine(checkFadeoutAndSkipCutScene());
		// 페이드 아웃 후 컷씬 넘기기를 시작해야함....
	}


	void onChangeCutSceneSpeed(GameObject go)
	{
		++nowCutSceneSpeed;
		if(nowCutSceneSpeed > 3) nowCutSceneSpeed = 1;

		switch(nowCutSceneSpeed)
		{
		case 1:
			spReplaySpeed.spriteName = "img_replay_speed_1";
			skipCutScene(1.0f);
			break;
		case 2:
			spReplaySpeed.spriteName = "img_replay_speed_2";
			skipCutScene(2.0f);
			break;
		case 3:
			spReplaySpeed.spriteName = "img_replay_speed_4";
			skipCutScene(4.0f);
			break;
		}
	}


	int prevCutSceneSpeed = 1;
	void onChangeCutSceneSpeedPress(GameObject go, bool state)
	{
		if(nowOpenCutScene && btnCutSceneSpeedPress.gameObject.activeInHierarchy)
		{
			if(state)
			{
				if(Input.touchCount > 1) return;

				prevCutSceneSpeed = nowCutSceneSpeed;
				nowCutSceneSpeed = 4;
			}
			else
			{
				nowCutSceneSpeed = prevCutSceneSpeed;
			}
			
			switch(nowCutSceneSpeed)
			{
			case 1:
				spReplaySpeed.spriteName = "img_replay_speed_1";
				skipCutScene(1.0f);
				break;
			case 2:
				spReplaySpeed.spriteName = "img_replay_speed_2";
				skipCutScene(2.0f);
				break;
			case 4:
				spReplaySpeed.spriteName = "img_replay_speed_4";
				skipCutScene(4.0f);
				break;
			}
		}
	}




	float checkSkipTime = 0.0f;
	IEnumerator checkFadeoutAndSkipCutScene()
	{
		while(true)
		{
			yield return null;
			if(Time.realtimeSinceStartup >= cutSceneSkipFadeOutTime && GameManager.me.uiManager.uiLayoutEffect.spFade.color.a >= 0.9f)
			{
				skipCutScene(20.0f);
				break;
			}
		}
	}

	void startSkipCutScene()
	{
		skipCutScene(20.0f);
	}


	public bool canSkipCutScene(bool isClearCutScene = true)
	{
		if( (isClearCutScene && GameManager.me.stageManager.isMaxRound) || GameManager.me.stageManager.isIntro) return false;
		return !GameDataManager.instance.canCutScenePlay;
	}


	// 컷씬 발동 조건을 체크한다...
	public bool roundStateCheck(bool isClearCutScene = false)
	{
#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker) return false;
		Debug.LogError("HellModeManager.instance.isOpen : " + HellModeManager.instance.isOpen);
		#endif

		if(HellModeManager.instance.isOpen) return false;

#if UNITY_EDITOR
		if(CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker)
		{

		}
		else
#endif
		{


			// 클리어 컷씬이고 맥스라운드면... 혹은 인트로면 컷씬보기 확인에 상관없이 무조건 컷씬 보여준다.
			if(canSkipCutScene(isClearCutScene))
			{
				return false;
			}
		}




		for(int i = 0; i < cutSceneCount; ++i)
		{
			if(readyCutScene[i].roundStateChecker != null && 
			   readyCutScene[i].status == CutSceneData.STATUS.PREPARE && readyCutScene[i].roundStateChecker.check() && 
			   GameManager.me.recordMode == GameManager.RecordMode.record)
			{



				openAndLoadCutScene( readyCutScene[i] );

				return true;
			}
			#if UNITY_EDITOR
			else if(CutSceneMakerForDesigner.instance.useCutSceneMaker || CutSceneMaker.instance.useCutSceneMaker)
			{
				if(readyCutScene[i].roundStateChecker != null && readyCutScene[i].status == CutSceneData.STATUS.PREPARE )
				{
					
					openAndLoadCutScene( readyCutScene[i] );
					
					return true;
				}
			}
			#endif


		}
		
		return false;			
	}
	

	// 컷씬 발동 조건이 충족됐으면 컷씬을 연다.
	public void openAndLoadCutScene(CutSceneData selectCutScene)
	{
		nowOpenCutScene = true;

#if UNITY_EDITOR
		Debug.LogError("openAndLoadCutScene selectCutScene : " + selectCutScene.id);
#endif

		currentCutScene = selectCutScene;

		#if UNITY_EDITOR
		Debug.LogError("openAndLoadCutScene CutSceneManager.nowOpenCutSceneType : " + CutSceneManager.nowOpenCutSceneType);

		if(DebugManager.instance.useDebug && CutSceneMakerForDesigner.instance.useCutSceneMaker)
		{
			CutSceneManager.nowOpenCutSceneType = CutSceneManager.CutSceneType.Pre;
		}

#endif

		if(CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.Pre)
		{
			currentCutScene.load();
			GameManager.me.cutSceneManager.startCutScene();	

			#if UNITY_EDITOR
			GameManager.me.cutSceneManager.lbCutSceneId.text = currentCutScene.id;
			#endif

		}
	}




	Quaternion _q;
	public void startCutScene()
	{
		GameManager.me.mapManager.pvpGradeSlotManager.gameObject.SetActive(false);

		UILoading.nowLoading = false;

		UIPlay.nowSkillEffectCamStatus = UIPlay.SKILL_EFFECT_CAM_STATUS.None;
		UIPlay.isFollowPlayerWhenSkillEffectCamIdle = false;
		UIPlay.usePlayerPositionOffsetWhenSkillEffectCam = false;
		UIPlay.showMapAfterBackToGameCamFromSkillCam = false;
		UIPlay.nowSkillEffectCamType = UIPlay.SKILL_EFFECT_CAM_TYPE.None;

		init();

		//===========
		#if UNITY_EDITOR
		Debug.Log("CutScene 발동!! 카메라 리셋 테스트!");
#endif

		_v = GameManager.me.uiManager.uiPlay.gameCameraContainer.position;
		_v.x = 0.0f; _v.y = 0.0f; _v.z = 0.0f;
		GameManager.me.uiManager.uiPlay.gameCameraContainer.position = _v;		
		
		_q = GameManager.me.uiManager.uiPlay.gameCameraContainer.transform.rotation;
		_v = _q.eulerAngles;_v.x = 0.0f;_v.y = 0.0f;_v.z = 0.0f;_q.eulerAngles = _v;
		GameManager.me.uiManager.uiPlay.gameCameraContainer.transform.rotation = _q;	
		//===========

		GameManager.me.uiManager.changeUI(UIManager.Status.UI_PLAY, true);	
		currentCutScene.status = CutSceneData.STATUS.PLAY; 
		currentCutScene.update(); // 0초 이하인 이벤트는 여기에서 바로 실행시켜준다.	


		if(GameManager.me.stageManager.nowRound.id == "INTRO")
		{
			btnCutSceneSpeed.gameObject.SetActive(false);
		}
		else
		{
			btnCutSceneSpeed.gameObject.SetActive(true);
		}


		nowCutSceneSpeed = 1;
		prevCutSceneSpeed = 1;

#if UNITY_EDITOR

		if(CutSceneMaker.useSearchMode && CutSceneMaker.targetTime > 1.0f)
		{
			skipCutScene(20.0f);
		}
#endif
	}

	public static int usingSkillSlotIndex = -1;

	public void startUnitSkillCamScene(string id, Vector3 centerPosition, UIPlay.SKILL_EFFECT_CAM_TYPE camType, int skillSlotIndex = -1, bool changeCameraClip = true)
	{

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;

//		if(camType == UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill && UnitSkillCamMaker.instance.useUnitSkillCamMaker == false) return;
#else
		//if(camType == UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill) return;
#endif

		if(string.IsNullOrEmpty(id) || GameManager.info.unitSkillCamData.ContainsKey(id) == false)
		{
#if UNITY_EDITOR
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				foreach(KeyValuePair<string, CutSceneData> kv in GameManager.info.unitSkillCamData)
				{
					id = kv.Key;
				}				
			}
			else
#endif
			{
				return;
			}
		}

		if(camType != UIPlay.SKILL_EFFECT_CAM_TYPE.ChaserAttack )
		{
			GameManager.me.effectManager.skillCamParticleEffectVisibleIsHide(true);
		}

		GameManager.me.characterManager.restoreMonsterVisibleForSkillCam();

		UIPlay.nowSkillEffectCamCenterPosition = centerPosition;
		UIPlay.nowSkillEffectCamStatus = UIPlay.SKILL_EFFECT_CAM_STATUS.Play;
		UIPlay.showMapAfterBackToGameCamFromSkillCam = false;
		UIPlay.nowSkillEffectCamType = camType;

		UIPlay.isFollowPlayerWhenSkillEffectCamIdle = false;

		_q.eulerAngles = new Vector3(0,0,0);
		GameManager.me.gameCameraContainer.transform.localRotation = _q;

		if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill )
		{
			UIPlayUnitSlot.lastActiveSkillUseSlotIndex = -1;
			usingSkillSlotIndex = skillSlotIndex;
		}
		else
		{
			usingSkillSlotIndex = -1;
		}

		if(camType != UIPlay.SKILL_EFFECT_CAM_TYPE.ChaserAttack )
		{
			initUnitSkillCamMonsterVisible();
		}

		nowOpenCutScene = true;

		nowOpenCutSceneType = CutSceneType.UnitCam;

		currentCutScene = GameManager.info.unitSkillCamData[id];

		currentCutScene.load(true);

		currentCutScene.init();

		init();

		if(changeCameraClip) useUnitCamCamra = true;// useCutSceneCameraWithoutClipSetting();
		else useCutSceneCameraWithoutClipSetting();

		currentCutScene.status = CutSceneData.STATUS.PLAY; 
		currentCutScene.update(); // 0초 이하인 이벤트는 여기에서 바로 실행시켜준다.	
		
		btnCutSceneSpeed.gameObject.SetActive(false);
		nowCutSceneSpeed = 1;
		prevCutSceneSpeed = 1;
	}




	void initUnitSkillCamMonsterVisible()
	{
		// 시전자를 빼고는 모두 보이지 않게 만든다.

		foreach(Monster mon in GameManager.me.characterManager.playerMonster)
		{
			if(mon.hp > 0 && mon.isEnabled)
			{
				if(UIPlay.nowSkillEffectCamType == UIPlay.SKILL_EFFECT_CAM_TYPE.HeroSkill)
				{
					if(mon.isPlayer)
					{
						mon.setVisibleForSkillCam(true,1.0f);
						continue;
					}

					mon.setVisibleForSkillCam(false);
				}
				else
				{
					if(UIPlayUnitSlot.lastActiveSkillUseSlotIndex != mon.monsterUISlotIndex)
					{
						mon.setVisibleForSkillCam(false);
					}
				}
			}
		}
		
		
		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			if(mon.hp > 0 && mon.isEnabled)
			{
				mon.setVisibleForSkillCam(false);
			}
		}
	}









	static float LOOP_INTERVAL = 0.05f;
	float currentTime = 0.0f;
	float _updateLoopLeftTime = 0.0f; 

	void LateUpdate()
	{
		if(hasCutScene || UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None || UIPlay.isPlayingPreCam) // 컷씬이 없으면 동작 자체를 하지 않아야 함.
		{
			//if(GameManager.me.stageManager.isIntro == false)
			{
				realDeltaTime = (Time.realtimeSinceStartup - _prevTime);
				_prevTime = Time.realtimeSinceStartup;	
				cutSceneDeltaTime = Time.deltaTime;

				#if UNITY_EDITOR
				if(CutSceneMaker.useSearchMode && CutSceneMaker.targetTime <= cutScenePlayTime)
				{
					CutSceneMaker.useSearchMode = false;
					skipCutScene(1.0f);

					//GameManager.setTimeScale = 0.0f;

					UnityEditor.EditorApplication.isPaused = true;

					return;
				}
				#endif

				update ();
			}
			/*
			else
			{
				if(introCutSceneStartTime > 0 && (GameManager.me.isPlaying || GameManager.me.stageManager.playTime >= 0) ) return;

				float newTime = currentTime + Time.smoothDeltaTime;
				float frameTime = newTime - currentTime; 
				if ( frameTime > LOOP_INTERVAL * 10.0f) frameTime = LOOP_INTERVAL * 10.0f;
				
				currentTime = newTime;
				_updateLoopLeftTime += frameTime; 
				float useLoopUpdateTime = 0.0f;
				
				float updateDeltaValue = 0.0f;

				if(_updateLoopLeftTime < LOOP_INTERVAL)
				{
					introLogicUpdate(0.0f);
				}
				else
				{
					while ( _updateLoopLeftTime >= LOOP_INTERVAL)      
					{           
						updateDeltaValue += LOOP_INTERVAL;
						useLoopUpdateTime += updateDeltaValue;
						_updateLoopLeftTime -= updateDeltaValue;  
						introLogicUpdate(LOOP_INTERVAL);
					} 
				}

				cutSceneDeltaTime = Time.deltaTime;
				introRenderingUpdate (updateDeltaValue);
			}
			*/
		}
	}









	void checkClickUI(bool isPress = true)
	{
		if(_clickUIId != null)
		{
			switch(_clickUIId)
			{
			case UnitSlot.U1:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[0].onPress(null,true);
				break;
			case UnitSlot.U2:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[1].onPress(null,true);
				break;
			case UnitSlot.U3:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[2].onPress(null,true);
				break;
			case UnitSlot.U4:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[3].onPress(null,true);
				break;
			case UnitSlot.U5:
				GameManager.me.uiManager.uiPlay.UIUnitSlot[4].onPress(null,true);
				break;
			case SkillSlot.S1:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[0].onPress(null,isPress);
				break;
			case SkillSlot.S2:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[1].onPress(null,isPress);
				break;
			case SkillSlot.S3:
				GameManager.me.uiManager.uiPlay.uiSkillSlot[2].onPress(null,isPress);
				break;
			}
		}
		
		_clickUIId = null;
	}




	void renderObject()
	{
		foreach(KeyValuePair<string, List<Monster>> kv in cutScenePlayCharacter)
		{
			foreach(Monster cha in kv.Value)
			{
				cha.cutSceneUpdate();
			}
		}	
		
		foreach(KeyValuePair<string, List<Effect>> kv in cutScenePlayEffect)
		{
			foreach(Effect eff in kv.Value)
			{
				eff.cutSceneUpdate();
			}
		}
	}



	public void updateFrame()
	{
		if(currentCutScene == null) // 현재 열려있는 컷씬이 없으면 
		{
			for(int i = 0; i < cutSceneCount; ++i)
			{
				if(currentCutScene != null)
				{
					return; // readycutscene에서 업데이트 중에 컷씬이 발동될 수 있음.
				}
				
				if(readyCutScene[i].status != CutSceneData.STATUS.PREPARE)
				{
					continue; // 이미 사용된 컷씬이면 검사하지 않는다.
				}
				
				readyCutScene[i].checkFrameEvent();
			}
		}
		else
		{

#if UNITY_EDITOR
			Debug.Log("currentCutScene.status : " + currentCutScene.status);
#endif


			if(currentCutScene.status == CutSceneData.STATUS.PLAY)
			{
				currentCutScene.checkFrameEvent();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE_TOUCH)
			{
				if(Input.GetMouseButtonDown(0) == true)
				{
					_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);
					if(_touchRect.contains(_touchPoint))
					{
						currentCutScene.status = CutSceneData.STATUS.PLAY;
						GameManager.setTimeScale = 1.0f;//_pauseOriginalTimeScale;
						GameManager.me.uiManager.uiTutorial.hide();
					}
				}

				currentCutScene.checkFrameEvent();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE_LONG_TOUCH)
			{
				currentCutScene.checkFrameEvent();

				if(currentCutScene.status != CutSceneData.STATUS.PAUSE_LONG_TOUCH) return;

				if(GameManager.me.stageManager.playTime >= cutSceneEventFrameEndTime)
				{
					GameManager.me.uiManager.uiTutorial.hide();
					currentCutScene.status = CutSceneData.STATUS.PLAY;
					GameManager.setTimeScale = 1.0f;//_pauseOriginalTimeScale;
				}
				else
				{
					if(Input.GetMouseButton(0) == true || Input.GetMouseButtonDown(0) )
					{
						_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);
						if(_touchRect.contains(_touchPoint))
						{
							GameManager.setTimeScale = 1.0f;//_pauseOriginalTimeScale;
							GameManager.me.uiManager.uiTutorial.gameObject.SetActive(false);
							GameManager.me.setMouseDown();
						}
					}
					else
					{
						GameManager.me.uiManager.uiTutorial.gameObject.SetActive(true);
						GameManager.setTimeScale = 0.0f;
					}
				}
			}
			else
			{
				currentCutScene.checkFrameEvent();
			}
		}
	}




	void update()
	{
		if(currentCutScene == null) // 현재 열려있는 컷씬이 없으면 
		{
			for(int i = 0; i < cutSceneCount; ++i)
			{
				if(currentCutScene != null)
				{
					return; // readycutscene에서 업데이트 중에 컷씬이 발동될 수 있음.
				}
				
				if(readyCutScene[i].status != CutSceneData.STATUS.PREPARE)
				{
					continue; // 이미 사용된 컷씬이면 검사하지 않는다.
				}
				
				readyCutScene[i].update();
			}
		}
		else
		{

			if(currentCutScene.status == CutSceneData.STATUS.PLAY)
			{
				currentCutScene.update();

				if(isSlowMode == false || slowModeTimeProgressType == 1) cutScenePlayTime += cutSceneDeltaTime;
				else cutScenePlayTime += realDeltaTime;

				renderObject();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE)
			{
				_pauseTime -= realDeltaTime;
				if(_pauseTime <= 0.0f)
				{
					currentCutScene.status = CutSceneData.STATUS.PLAY;
					GameManager.setTimeScale = 1.0f;//GameManager.setTimeScale = _pauseOriginalTimeScale;
				}
				
				renderObject();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE_TOUCH)
			{
				if(Input.GetMouseButtonDown(0) == true)
				{
					_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);
					if(_touchRect.contains(_touchPoint))
					{
						currentCutScene.status = CutSceneData.STATUS.PLAY;
						GameManager.setTimeScale = 1.0f;//GameManager.setTimeScale = _pauseOriginalTimeScale;
						GameManager.me.uiManager.uiTutorial.hide();
					}
				}
				
				renderObject();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE_LONG_TOUCH)
			{
				if(cutScenePlayTime >= cutSceneEventEndTime)
				{
					GameManager.me.uiManager.uiTutorial.hide();
					currentCutScene.status = CutSceneData.STATUS.PLAY;
					GameManager.setTimeScale = 1.0f;//_pauseOriginalTimeScale;
				}
				else
				{
					if(Input.GetMouseButton(0) == true && Input.GetMouseButtonDown(0) == false)
					{
						_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);
						if(_touchRect.contains(_touchPoint))
						{
							//currentCutScene.status = CutSceneData.STATUS.PLAY;
							GameManager.setTimeScale = 1.0f;//GameManager.setTimeScale = _pauseOriginalTimeScale;
							cutScenePlayTime += cutSceneDeltaTime;
							GameManager.me.uiManager.uiTutorial.gameObject.SetActive(false);
						}
					}
					else
					{
						GameManager.me.uiManager.uiTutorial.gameObject.SetActive(true);
						GameManager.setTimeScale = 0.0f;
					}
				}

				renderObject();
			}
			else if(currentCutScene.status == CutSceneData.STATUS.PAUSE_TOUCH_OFF)
			{

#if UNITY_EDITOR
				Debug.LogError("PAUSE_TOUCH_OFF : " + Input.GetMouseButton(0) + "    down: " + Input.GetMouseButtonDown(0));
#endif

				if(Input.GetMouseButton(0) == false && Input.GetMouseButtonDown(0) == false)
				{
					currentCutScene.status = CutSceneData.STATUS.PLAY;
					GameManager.setTimeScale = 1.0f;//GameManager.setTimeScale = _pauseOriginalTimeScale;
					GameManager.me.uiManager.uiTutorial.hide();
				}
				
				renderObject();
			}



#if UNITY_EDITOR

#else
			if(EpiServer.instance != null && EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
#endif
			{
			tfPlayTime.text = cutScenePlayTime.ToString("#.##") + "\nf:" + GameManager.replayManager.nowFrame;	
			_prevCutSceneTime = cutScenePlayTime;
			}

		}
	}
	


	// 컷씬에 사용한 캐릭터들을 한번에 모두 제거할 때 사용한다.
	// 다만 컷씬용으로 생성한 캐릭터들만 삭제한다.
	
	List<string> _tempKeys = new List<string>();
	public void clearCutSceneAsset(bool cleanAll = true)
	{
		clearCutSceneObjectAsset(cleanAll);
		clearCutSceneEffectAsset();
	}



	public void clearCutSceneObjectAsset(bool cleanAll = true)
	{
		if(cleanAll) cutSceneCharacterDic.Clear();

		_tempKeys.Clear();
		
		foreach(KeyValuePair<string, List<Monster>> kv in cutScenePlayCharacter)
		{
			foreach(Monster cha in kv.Value)
			{
				cha.cutSceneInit();

				if(cha.isCutSceneOnlyCharacter)
				{
					GameManager.me.characterManager.cleanMonster(cha);
				}
				else cha.removeTooltip();

				_tempKeys.Add(kv.Key);
			}
		}
		
		GameManager.me.player.removeTooltip();
		
		foreach(String key in _tempKeys)
		{
			cutScenePlayCharacter[key].Clear();
		}		
	}

	public void clearCutSceneEffectAsset(bool clearAll = true)
	{
		foreach(KeyValuePair<string, List<Effect>> kv in cutScenePlayEffect)
		{
			foreach(Effect eff in kv.Value)
			{
				if(eff == null) continue;
				eff.cutSceneInit();
				GameManager.me.effectManager.setCutSceneEffect(eff);
			}
			cutScenePlayEffect[kv.Key].Clear();
		}
		
		if(clearAll) cutScenePlayEffect.Clear();
	}





	
	public void removeCutSceneCharacter(string id)
	{
		if(cutScenePlayCharacter.ContainsKey(id) == false) return;

		foreach(Monster cha in cutScenePlayCharacter[id])
		{
			cha.cutSceneInit();

			if(cha.isCutSceneOnlyCharacter)
			{
				GameManager.me.characterManager.cleanMonster(cha, true);
			}
			else cha.removeTooltip();
		}
		
		cutScenePlayCharacter[id].Clear();
	}
	
	
	
	
	public void removeCutSceneEffect(string id)
	{
		if(cutScenePlayEffect.ContainsKey(id) == false) return;

		foreach(Effect eff in cutScenePlayEffect[id])
		{
			GameManager.me.effectManager.setCutSceneEffect(eff);
			eff.cutSceneInit();
		}
		cutScenePlayEffect[id].Clear();
	}	
	
	
	
	

	// 컷씬용 캐릭터를 입력.
	private string _tempMonType;
	private Vector3 _v = new Vector3();
	public Monster addMonsterToStage(bool isPlayerMon, Monster.TYPE type, string monId)
	{
		_v.x = -1000.0f;
		_v.y = 0.0f;
		_v.z = 0.0f;
		Monster mon;

//		Debug.Log(monId);

		bool isPlayer = (type == Monster.TYPE.PLAYER);

		if(type == Monster.TYPE.UNIT)
		{
			_tempMonType = GameManager.info.unitData[monId].resource;
		}
		else if(type == Monster.TYPE.HERO)
		{
			_tempMonType = GameManager.info.heroMonsterData[monId].resource;
		}
		else if(type == Monster.TYPE.NPC)
		{
			_tempMonType = GameManager.info.npcData[monId].resource;
		}
		else
		{
			_tempMonType = monId;
		}

		mon = GameManager.me.characterManager.getMonster(isPlayer, isPlayerMon, _tempMonType, false);
		
		mon.cutSceneInit();
		
		if(type == Monster.TYPE.UNIT)
		{
			CharacterUtil.setRare(GameManager.info.unitData[monId].rare, mon);
		}
		else if(type == Monster.TYPE.HERO)
		{
			mon.removeRareLine();
		}
		else if(type == Monster.TYPE.NPC)
		{
			if(mon.monsterData.hasFaceAni == false)
			{
				mon.removeRareLine();
				//CharacterUtil.setRare(RareType.NORMAL, mon);
			}
		}

		mon.init(null, null, monId, isPlayerMon, type);

		mon.isCutSceneOnlyCharacter = true;
		
		//if(isPlayerMon == false) mon.isFlipX = true;
		
		mon.cTransform.position = _v;
		
		mon.isEnabled = true;

		return mon;
	}	


	
	
	public float cutScenePlaySpeed = 1.0f;
	public float cutSceneSkipFadeOutTime = 1.0f;
	
	public void skipCutScene(float skipSpeed = 5.0f)
	{
		isSlowMode = false;
		slowModeTimeProgressType = 0;
		cutScenePlaySpeed = skipSpeed;
		GameManager.setTimeScale = cutScenePlaySpeed;

		if(skipSpeed > 1.0f)
		{
			SoundManager.instance.stopLoopChargingEffect();
			//SoundManager.instance.stopTutorialVoice();
		}
	}
	
	
	public void setPause(float pauseTime, int pauseType)
	{
		currentCutScene.status = CutSceneData.STATUS.PAUSE;
		_pauseTime = pauseTime;
		_pauseOriginalTimeScale = Time.timeScale;
		
		if(pauseType == 0) // stop Time only!
		{
			
		}
		else if(pauseType == 1) // stop All!
		{
			GameManager.setTimeScale = 0.0f;
		}
	}
	
	private Rectangle _touchRect;
					
	private string _clickUIId = null;

	public void setPauseAndTouch(Rectangle touchRect, int pauseType, string clickUIId)
	{
//		Debug.LogError("cutScenePlayTime : " + CutSceneManager.cutScenePlayTime + "   playTime: " + GameManager.me.stageManager.playTime);

		currentCutScene.status = CutSceneData.STATUS.PAUSE_TOUCH;
		_pauseOriginalTimeScale = Time.timeScale;
		_touchRect = touchRect;

		_clickUIId = null;
		if(string.IsNullOrEmpty( clickUIId ) != null) _clickUIId = clickUIId;
						
		if(pauseType == 0) // stop Time only!
		{
			
		}
		else if(pauseType == 1) // stop All!
		{
			GameManager.setTimeScale = 0.0f;
		}
	}
	
	public void setPauseAndLongTouch(Rectangle touchRect, int pauseType)
	{
		currentCutScene.status = CutSceneData.STATUS.PAUSE_LONG_TOUCH;
		_pauseOriginalTimeScale = Time.timeScale;
		_touchRect = touchRect;
		
		if(pauseType == 0) // stop Time only!
		{
			
		}
		else if(pauseType == 1) // stop All!
		{
			GameManager.setTimeScale = 0.0f;
		}
	}	
	


	public bool setPauseAndTouchOff(Rectangle touchRect, int pauseType)
	{
		if(Input.GetMouseButton(0))
		{
			_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);

			#if UNITY_EDITOR
			Debug.LogError("setPauseAndTouchOff mouse ok & tp : x:" + _touchPoint.x + "   y:" + _touchPoint.y + "    rx:" + _touchRect.x + " ry:" + _touchRect.y + "   rw:" + _touchRect.width + "  rh:" + _touchRect.height);
			#endif


			if(touchRect.contains(_touchPoint))
			{

				#if UNITY_EDITOR
				Debug.LogError("setPauseAndTouchOff mouse ok & touchrect ok! ");
				#endif

				currentCutScene.status = CutSceneData.STATUS.PAUSE_TOUCH_OFF;
				_pauseOriginalTimeScale = Time.timeScale;
				_touchRect = touchRect;
				
				if(pauseType == 0) // stop Time only!
				{
					
				}
				else if(pauseType == 1) // stop All!
				{
					GameManager.setTimeScale = 0.0f;
				}

				return true;
			}
		}

		#if UNITY_EDITOR
		Debug.LogError("setPauseAndTouchOff : false ");
		#endif

		return false;
	}


	
	// 준비된 컷씬 이벤트가 모두 종료되었을때...
	
	/*
	public void completeCutScene()
	{
#if UNITY_EDITOR		
		Debug.Log("======= CUT SCENE COMPLETE !!! ==========");
#endif
		if(status == Status.INGAME_CUTSCENE) // 인 게임 컷신 일때는 게임은 그냥 계속 하면 된다...
		{
			
		}
		else if(status == Status.CUTSCENE) // 단순 컷씬이라면 이후 뭔가를 해야한다....
		{
			//clearCutSceneAsset();
		}
		
		status = Status.NONE;
	}
	*/

	public void clearOpenEffectCam()
	{
		if(UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None)
		{
			if(UIPlay.showMapAfterBackToGameCamFromSkillCam && GameManager.me.mapManager.inGameMap != null)
			{
				GameManager.me.mapManager.createBackground(GameManager.me.mapManager.inGameMap.id,true);
			}

			GameManager.me.effectManager.skillCamParticleEffectVisibleIsHide(false);
			GameManager.me.characterManager.restoreMonsterVisibleForSkillCam();
			UIPlay.nowSkillEffectCamStatus = UIPlay.SKILL_EFFECT_CAM_STATUS.None;
			UIPlay.nowSkillEffectCamType = UIPlay.SKILL_EFFECT_CAM_TYPE.None;
			UIPlay.usePlayerPositionOffsetWhenSkillEffectCam = false;
			UIPlay.isFollowPlayerWhenSkillEffectCamIdle = false;
			UIPlay.showMapAfterBackToGameCamFromSkillCam = false;
			GameManager.me.uiManager.uiPlay.csCamMoveType = UIPlay.CAM_MOVE_STOP;
			clearCutSceneEffectAsset(false);
			useCutSceneCamera = false;
			GameManager.me.uiManager.uiPlay.resetToChallengeModeZoomDefaultCamera();

			if(GameManager.me.playMode == GameManager.PlayMode.replay)
			{
				GameManager.setTimeScale = GameManager.me.uiManager.uiPlay.replaySpeed;
			}
			else if(GameManager.me.uiManager.uiPlay.btnFastPlay.gameObject.activeInHierarchy && GameManager.me.isFastPlay)
			{
				GameManager.setTimeScale = UIPlay.FAST_PLAY_SPEED;
			}
			else
			{
				GameManager.setTimeScale = 1.0f;
			}
		}
	}

	
	// 현재 열린 컷씬을 그냥 닫는다.
	public void closeOpenCutScene(bool closeAll = false)
	{
		isSkipMode = false;
		GameManager.me.characterManager.inGameGUITooltipContainer.gameObject.SetActive(false);
		status = Status.PREPARE;
		nowOpenCutScene = false;
		currentCutScene = null;
		stopLoopInGame = false;
		btnCutSceneSkip.gameObject.SetActive(false);
		btnCutSceneSpeed.gameObject.SetActive(false);
		nowCutSceneSpeed = 1;
		prevCutSceneSpeed = 1;

		clearOpenEffectCam();

		if(closeAll)
		{
			status = Status.PREPARE;
			close();

			if(CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.After)
			{
				bool isDebug = false;

#if UNITY_EDITOR
				isDebug = DebugManager.instance.useDebug;
#endif

				if(isDebug || DebugManager.useTestRound || GameManager.me.successType != WSDefine.GAME_SUCCESS)
				{
					if(GameManager.me.successType != WSDefine.GAME_SUCCESS && isDebug == false)
					{
						AdviceData.checkAdvice(GameManager.me.returnToSelectScene);
					}
					else
					{
						GameManager.me.returnToSelectScene();
					}
				}
				else
				{
					GameManager.me.cutSceneManager.useCutSceneCamera = true;

					GameManager.me.checkMissionNoticeAndOpenRoundClearPopup();
				}
			}
			else
			{
				useCutSceneCamera = false;
				GameManager.me.returnToSelectScene();
			}
		}
	}
	
	// 다음 컷씬을 연다.
	public void openNextCutScene(string nextCutSceneId, bool deletePrevAssets = true)
	{
		status = Status.NONE;

		if(deletePrevAssets) clearCutSceneAsset();

		nowOpenCutScene = true;

		#if UNITY_EDITOR
		Debug.LogError("openNextCutScene nextCutSceneId : " + nextCutSceneId);
#endif

		currentCutScene = GameManager.info.cutSceneData[nextCutSceneId];

		currentCutScene.load();
		GameManager.me.cutSceneManager.startCutScene();

	}

	
	// 모든 컷씬이 끝나거나 게임이 종료될때 완전 초기화.
	public void close()
	{
		isSkipMode = false;
		if(currentCutScene != null) currentCutScene.init();
		cutSceneCount = 0;
		status = Status.NONE;
		readyCutScene = null;
		hasCutScene = false;
		init();	
		clearCutSceneAsset();

		if(nowOpenCutSceneType == CutSceneType.Pre) GameManager.me.uiManager.uiLayoutEffect.hide();
		nowOpenCutScene = false;
	}
	
	
	
	
	void OnDestroy()
	{

	}
	
	
	
}


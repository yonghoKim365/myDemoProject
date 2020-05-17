using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public partial class UIPlay : UIBase {

#if UNITY_EDITOR
	public const float FAST_PLAY_SPEED = 10.5f;
#else
	public const float FAST_PLAY_SPEED = 1.5f;
#endif

	public UIPlayWarningAlpha warningAlpha;

	public PhotoDownLoader playerFace;
	public PhotoDownLoader pvpFace;

	public UISprite spPvpEmptyFace;

	public Transform tfMiniMapStart;
	public Transform tfMiniMapEnd;

	public Transform tfMiniMapTop;
	public Transform tfMiniMapBottom;

	public Transform tfMiniMapContainer;

	public Transform tfMiniMapBg;

	public UIChallangeModeInfo challangeModeInfo;
	public UIHellModeInfo hellModeInfo;

	public UIGameResultString gameResultWord;

	public GameObject roundInfoContainer;
	
	public UILabel lbRoundInfo, lbRoundTime, lbRoundLeftNum, lbChaser;
	
	public UISprite spRoundInfoBg, spRoundInfoLeftNumIcon;

	public UILabel lbSP,lbMP;
	public UISprite pbSP,pbMP;

	public UISprite spHurryup, spPlayerDamage;



	public UIPanel playerChangeControlPanelEffect;



	public static UIPlayUnitSlot getUnitSlot(int monsterUISlotIndex)
	{
		if(monsterUISlotIndex >= 10)
		{
			return GameManager.me.uiManager.uiPlay.UIUnitSlot2[monsterUISlotIndex % 10];
		}

		return GameManager.me.uiManager.uiPlay.UIUnitSlot1[monsterUISlotIndex];
	}


	public UIPlayUnitSlot[] UIUnitSlot 
	{
		get
		{
			if(GameManager.me.battleManager.selectPlayerIndex == 0)
			{
				return UIUnitSlot1;
			}

			return UIUnitSlot2;
		}
	}

	public UIPlaySkillSlot[] uiSkillSlot
	{
		get
		{
			if(GameManager.me.battleManager.selectPlayerIndex == 0)
			{
				return uiSkillSlot1;
			}
			
			return uiSkillSlot2;
		}
	}


	public UISprite[] uiUnitEmptySlot
	{
		get
		{
			if(GameManager.me.battleManager.selectPlayerIndex == 0)
			{
				return uiUnitEmptySlot1;
			}
			
			return uiUnitEmptySlot2;
		}
	}

	public UISprite[] uiSkillEmptySlot
	{
		get
		{
			if(GameManager.me.battleManager.selectPlayerIndex == 0)
			{
				return uiSkillEmptySlot1;
			}
			
			return uiSkillEmptySlot2;
		}
	}

	public GameObject[] playerControlSlots = new GameObject[2];

	public UIPlayUnitSlot[] UIUnitSlot1 = new UIPlayUnitSlot[5];
	public UIPlaySkillSlot[] uiSkillSlot1 = new UIPlaySkillSlot[3];
	public UISprite[] uiUnitEmptySlot1 = new UISprite[5];
	public UISprite[] uiSkillEmptySlot1 = new UISprite[3];

	public UIPlayUnitSlot[] UIUnitSlot2 = new UIPlayUnitSlot[5];
	public UIPlaySkillSlot[] uiSkillSlot2 = new UIPlaySkillSlot[3];
	public UISprite[] uiUnitEmptySlot2 = new UISprite[5];
	public UISprite[] uiSkillEmptySlot2 = new UISprite[3];


	public UIPlayTagSlot[] playerTagSlot;
	public UIPlayTagSlot[] pvpTagSlot;

	
	public UIButton btnPause, btnSkipCutScene, btnClearGame;

	public UIButton btnReplayClose, btnReplaySpeed;
	public UISprite spReplaySpeed;

	public UIButton btnAutoPlay;
	public UISprite spAutoPlay;

	public UIButton btnFastPlay;
	public UISprite spFastPlay;


	public UIBattleStartAnimation readyBattle;

	public GameObject goHellGoAni;
	public UISprite spHellState;
	public ParticleSystem psHellSuccess;


	public GameObject pvpContainer;

	public Transform gameCameraContainer;
	public Transform gameCameraPosContainer;

	public Transform panelTop;
	public Transform panelBottom;
	
	public BoxCollider bcIgnoreGameTouch;

	public UILabel lbGameTimer;
	public UILabel tfPlayTime;
	//public UILabel tfLeftTime;
	public bool updateLeftTime = false;

	//   - 제한시간 일 경우(섬멸,이동,파괴,획득), 10초 남았을 때 빨강색으로 깜박거리기
	bool _isLimitTimeMode = false;



	public GameObject goEpicPreCamInfo;
	public UISprite spEpicRoundIcon, spEpicRoundText;
	public UILabel lbEpicRoundInfo;

	void onClickPause(GameObject go)
	{
		onOpenPause();
	}


	void Awake()
	{
		gameObject.SetActive(false);
		UIEventListener.Get (btnPause.gameObject).onClick = onClickPause;

		UIEventListener.Get (btnReplayClose.gameObject).onClick = onClickReplayClose;
		UIEventListener.Get (btnReplaySpeed.gameObject).onClick = onClickReplaySpeed;
		UIEventListener.Get (btnClearGame.gameObject).onClick = onClickClearGame;

		UIEventListener.Get (btnAutoPlay.gameObject).onClick = onClickAutoPlay;
		UIEventListener.Get (btnFastPlay.gameObject).onClick = onClickFastPlay;
	}


	void onClickAutoPlay(GameObject go)
	{
		GameManager.me.isAutoPlay = !GameManager.me.isAutoPlay;
		setAutoPlay(GameManager.me.isAutoPlay);
		if(GameManager.me.isAutoPlay)
		{
			GameManager.me.player.changeAutoPlayDelay = 5;
		}
	}

	void setAutoPlay(bool isAuto)
	{
		for(int i = 0; i < 3; ++i)
		{
			if(GameDataManager.instance.skills[i].isOpen)
			{
				uiSkillSlot[i].releasePressByTurnOffAutoPlay();
			}
		}

		if(isAuto)
		{
			spAutoPlay.spriteName = "ibtn_auto_onidle";
			btnFastPlay.gameObject.SetActive(true);
			setFastPlay(GameManager.me.isFastPlay);
		}
		else
		{
			spAutoPlay.spriteName = "ibtn_auto_offidle";
			btnFastPlay.gameObject.SetActive(false);
			setFastPlay(false);
		}
	}


	void onClickFastPlay(GameObject go)
	{
		GameManager.me.isFastPlay = !GameManager.me.isFastPlay;
		setFastPlay(GameManager.me.isFastPlay);
	}
	
	public void setFastPlay(bool isFast)
	{
		if(isFast)
		{
			spFastPlay.spriteName = "ibtn_2x_onidle";
			GameManager.setTimeScale = FAST_PLAY_SPEED;
		}
		else
		{
			spFastPlay.spriteName = "ibtn_2x_offidle";
			GameManager.setTimeScale = 1.0f;
		}
	}




	void onClickClearGame(GameObject go)
	{
#if UNITY_EDITOR

		if( (GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic && GameManager.me.stageManager.nowRound.id != "INTRO") || GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong )
		{
			GameManager.me.stageManager.nowPlayingGameResult = Result.Type.Win;
			GameManager.me.mapManager.clearRound();
		}
		
		btnClearGame.gameObject.SetActive(false);

		return;

#endif

		if(EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
		{

#if UNITY_ANDROID
			GameManager.me.OnApplicationQuit();
#else
			GameManager.setTimeScale = 0.0f;
#endif
			return;
		}


#if UNITY_EDITOR
		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic && GameManager.me.stageManager.nowRound.id != "INTRO")
		{
			GameManager.me.stageManager.nowPlayingGameResult = Result.Type.Win;
			GameManager.me.mapManager.clearRound();
		}
#endif

		btnClearGame.gameObject.SetActive(false);
	}

	
	void Start()
	{
		UIEventListener.Get(btnSkipCutScene.gameObject).onClick = onSkipCutScene;
	}
	
	public Camera gameCamera; // game perspective camera
	
	public override void hide ()
	{
		_minimapTouched = false;
		_minimapTouchFingerId = -10000;
		resetCamera();		
		clearTweener();
		cameraTarget = null;
		targetChangeTweening = false;
		csCamMoveType = 0;
		base.hide ();
		
	}
	
	public override void show ()
	{
		base.show();
		_minimapTouched = false;
		_minimapTouchFingerId = -10000;
		//init();
	}
	
	
	public void clearTweener()
	{

		int len = tweener.Count;
		for(int i = 0; i < len; ++i)
		{
			tweener[i].delete();
		}
		tweener.Clear();
	}
	

	public void onOpenPause()
	{
		
		#if !UNITY_EDITOR
		if(GameManager.me.stageManager.nowRound != null &&
		   GameManager.me.stageManager.nowRound.id == "INTRO")
		{
			return;
		}
		#endif

		if(GameManager.me.isPlaying == false) return;

		GameManager.me.uiManager.popupPaused.show();
	}


	void onSkipCutScene(GameObject go)
	{

		if(EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
		{
			
			#if UNITY_ANDROID
			GameManager.me.OnApplicationQuit();
			#else
			GameManager.setTimeScale = 0.0f;
			#endif
			return;
		}


		Debug.LogError("차후 막아야한다.");

		if(Time.timeScale <= 1.0f) GameManager.setTimeScale = 1.2f;
		else if(Time.timeScale <= 1.2f) GameManager.setTimeScale = 2.0f;
		else if(Time.timeScale <= 2.0f) GameManager.setTimeScale = 10.0f;
		else if(Time.timeScale <= 10.0f) GameManager.setTimeScale = 1.0f;
	}
	
	
	public Transform cameraTarget = null;


	public void init()
	{
		cameraTarget = GameManager.me.player.cTransform;

		readyBattle.gameObject.SetActive(false);
		
		if(CutSceneManager.nowOpenCutScene == false) resetCamera();
		
		bcIgnoreGameTouch.gameObject.SetActive(false);
		
		prevPlayTime = 0.0f;
		tfPlayTime.text = "";
		
		clearTweener();

		GameManager.me.battleManager.init();

		resetUI();
	}

	public static string playerImageUrl = "";
	public static string pvpImageUrl = "";


	public static int playerLeagueGrade = 1;
	public static Xint pvpleagueGrade = 1;


	private Vector3 ROUND_INFO_LABEL_SINGLE_POSITION = new Vector3(-378.3f, -14.8f, 91);


	void onClickReplaySpeed(GameObject go)
	{
		switch(replaySpeed)
		{
		case 1:
			replaySpeed = 2;
			GameManager.setTimeScale = 2.0f;
			spReplaySpeed.spriteName = "img_replay_speed_2";
			break;
		case 2:
		case 10:
			replaySpeed = 4;
			GameManager.setTimeScale = 4.0f;
			#if UNITY_EDITOR
			GameManager.setTimeScale = 20.0f;
			#endif

			spReplaySpeed.spriteName = "img_replay_speed_4";
			break;
		case 4:
			replaySpeed = 1;
			GameManager.setTimeScale = 1.0f;
			spReplaySpeed.spriteName = "img_replay_speed_1";
			break;
		}
	}


	void onClickReplayClose(GameObject go)
	{
		if(GameManager.me.playMode == GameManager.PlayMode.replay && GameManager.me.isPlaying.Get())
		{
			btnReplayClose.gameObject.SetActive(false);
			GameManager.me.returnToSelectScene();
		}
	}

	public int replaySpeed = 1;


	public void initAutoPlay()
	{
		if(DebugManager.useTestRound)
		{
			GameManager.me.canUseAutoPlay = !StageManager.instance.isIntro;
			setAutoPlay(GameManager.me.isAutoPlay);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic && 
		   GameManager.me.stageManager.nowRoundId.StartsWith("A") &&
		   GameManager.me.stageManager.nowRoundId != ("A" + GameDataManager.instance.maxAct + "S" + GameDataManager.instance.maxStage + "R" + GameDataManager.instance.maxRound)) 
		{
			GameManager.me.canUseAutoPlay = true;
			setAutoPlay(GameManager.me.isAutoPlay);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Championship)
		{
			GameManager.me.canUseAutoPlay = true;
			setAutoPlay(GameManager.me.isAutoPlay);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Hell)
		{
			GameManager.me.canUseAutoPlay = true;
			setAutoPlay(GameManager.me.isAutoPlay);
		}
		else if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong)
		{
			if(GameManager.me.stageManager.sigongData != null)
			{
				GameManager.me.canUseAutoPlay = (GameManager.me.stageManager.sigongData.autoPlay == WSDefine.YES);
			}
			else
			{
				GameManager.me.canUseAutoPlay = false;
			}

			setAutoPlay(GameManager.me.isAutoPlay);
		}
		else
		{
			GameManager.me.canUseAutoPlay = false;
		}
	}


	public bool didShowHurryUp = false;

	public void resetUI()
	{
		spPlayerDamage.cachedGameObject.gameObject.SetActive(false);
		didShowHurryUp = false;
		spHurryup.gameObject.SetActive(false);

		warningAlpha.stop();

		playerChangeControlPanelEffect.gameObject.SetActive(false);

		hideMenu(0.0f);

		replaySpeed = 1;
		spReplaySpeed.spriteName = "img_replay_speed_1";

		lbGameTimer.color = Color.white;
		lbGameTimer.text = GameManager.me.stageManager.settingTime.ToString();

		if(GameManager.me.playMode == GameManager.PlayMode.replay)
		{
			btnPause.gameObject.SetActive(false);
			btnReplayClose.gameObject.SetActive(false);
			btnReplaySpeed.gameObject.SetActive(false);

			GameManager.me.canUseAutoPlay = false;
		}
		else
		{
			btnPause.gameObject.SetActive(true);
			btnReplayClose.gameObject.SetActive(false);
			btnReplaySpeed.gameObject.SetActive(false);

			initAutoPlay();
		}

//#if UNITY_EDITOR
//		GameManager.me.canUseAutoPlay = true;
//#endif


		pvpContainer.gameObject.SetActive(false);
		playerHpBar.gameObject.SetActive(true);

		tfMiniMapContainer.gameObject.SetActive(true);

		_v = tfMiniMapContainer.localPosition;

		if(StageManager.instance.nowPlayingGameType == GameType.Mode.Championship)
		{
			_v.x = -60;
		}
		else
		{
			_v.x = 0;
		}

		tfMiniMapContainer.localPosition = _v;

		_v = tfMiniMapBg.localPosition;
		_v.y = 310;
		tfMiniMapBg.localPosition = _v;


		lbRoundTime.text = "";
		lbRoundLeftNum.text = "";
		lbRoundTime.color = Color.white;

		lbChaser.text = "";
		lbChaser.color = Color.white;
		lbChaser.gameObject.SetActive(false);

		_isLimitTimeMode = false;


		if(StageManager.instance.nowPlayingGameType != GameType.Mode.Hell)
		{
			updateLeftTime = (GameManager.me.stageManager.nowRound.settingTime > -1);
			hellModeInfo.gameObject.SetActive(false);
		}
		else
		{
			updateLeftTime = true;
			hellModeInfo.gameObject.SetActive(true);
		}

		if(GameManager.me.stageManager.nowRoundMode == RoundData.MODE.PVP)
		{
			_v = btnAutoPlay.transform.localPosition;
			_v.x = 331f;
			_v.y = 233f;
			btnAutoPlay.transform.localPosition = _v;
			
			_v = btnFastPlay.transform.localPosition;
			_v.x = 227f;
			_v.y = 233f;
			btnFastPlay.transform.localPosition = _v;
		}
		else
		{
			_v = btnAutoPlay.transform.localPosition;
			_v.x = 460f;
			_v.y = 219.84f;
			btnAutoPlay.transform.localPosition = _v;
			
			_v = btnFastPlay.transform.localPosition;
			_v.x = 355f;
			_v.y = 219.84f;
			btnFastPlay.transform.localPosition = _v;
		}


		if(GameManager.me.stageManager.nowRoundId == "INTRO")
		{
			challangeModeInfo.gameObject.SetActive(false);
			roundInfoContainer.SetActive(false);
			lbRoundTime.gameObject.SetActive(false);
			lbRoundLeftNum.gameObject.SetActive(false);
			btnPause.gameObject.SetActive(false);
		}
		else
		{

			switch(GameManager.me.stageManager.nowRoundMode)
			{
			case RoundData.MODE.KILLEMALL :
				lbRoundInfo.text = Util.getUIText("KILLEMALL");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				lbRoundTime.transform.localPosition = ROUND_INFO_LABEL_SINGLE_POSITION;
				lbRoundTime.gameObject.SetActive(GameManager.me.stageManager.nowRound.settingTime > -1);
				lbRoundLeftNum.gameObject.SetActive(false);			
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;
				break;
				
			case RoundData.MODE.SURVIVAL :
				lbRoundInfo.text = Util.getUIText("SURVIVAL");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.transform.localPosition = ROUND_INFO_LABEL_SINGLE_POSITION;
				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));
				lbRoundLeftNum.gameObject.SetActive(false);
				
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;

				didShowHurryUp = true;

				break;
				
			case RoundData.MODE.PROTECT :
				lbRoundInfo.text = Util.getUIText("PROTECT");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.transform.localPosition = ROUND_INFO_LABEL_SINGLE_POSITION;
				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));
				lbRoundLeftNum.gameObject.SetActive(false);
				
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;

				didShowHurryUp = true;

				break;
				
			case RoundData.MODE.SNIPING :
				lbRoundInfo.text = Util.getUIText("SNIPING");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);

				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));

				lbRoundLeftNum.gameObject.SetActive(false);

				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;



				break;
				
			case RoundData.MODE.KILLCOUNT :
				lbRoundInfo.text = Util.getUIText("KILLCOUNT");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.gameObject.SetActive(false);
				
				lbRoundLeftNum.transform.localPosition = new Vector3(-351,-13,91);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_monster";
				lbRoundLeftNum.gameObject.SetActive(true);
				
				break;
			case RoundData.MODE.KILLCOUNT2:
				lbRoundInfo.text = Util.getUIText("KILLCOUNT");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.gameObject.SetActive(false);
				
				lbRoundLeftNum.transform.localPosition =new Vector3(-351,-13,91);
				lbRoundLeftNum.gameObject.SetActive(true);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_monster";
				
				break;
				
			case RoundData.MODE.ARRIVE :
				lbRoundInfo.text = Util.getUIText("ARRIVE");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));
				lbRoundTime.transform.localPosition = new Vector3(-440,-13,91);
				
				
				lbRoundLeftNum.gameObject.SetActive(true);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_distance";
				
				if(lbRoundTime.gameObject.activeSelf)
				{
					lbRoundLeftNum.transform.localPosition = new Vector3(-259,-13,91);
				}
				else
				{
					lbRoundLeftNum.transform.localPosition = new Vector3(-337,-13,91);
				}
				
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;

				if(GameManager.me.stageManager.nowRound.chaser != null)
				{
					GameManager.me.uiManager.uiPlay.lbChaser.gameObject.SetActive(true);
					GameManager.me.uiManager.uiPlay.lbChaser.text =  (-(int)((GameManager.me.stageManager.nowRound.playerStartPosX - GameManager.me.stageManager.nowRound.chaser.posX) * 0.01f)) + "m";
					GameManager.me.uiManager.uiPlay.lbChaser.color = Color.white;
				}

				break;
				
			case RoundData.MODE.DESTROY :
				lbRoundInfo.text = Util.getUIText("DESTROY");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.transform.localPosition = new Vector3(-476.6f, -14.8f, 91);
				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));
				
				if(lbRoundTime.gameObject.activeSelf)
				{
					lbRoundLeftNum.transform.localPosition =new Vector3(-259,-13,91);
				}
				else
				{
					lbRoundLeftNum.transform.localPosition =new Vector3(-351,-13,91);
				}
				
				lbRoundLeftNum.gameObject.SetActive(true);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_object";
				
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;


				if(GameManager.me.stageManager.nowRound.chaser != null)
				{
					GameManager.me.uiManager.uiPlay.lbChaser.gameObject.SetActive(true);
					GameManager.me.uiManager.uiPlay.lbChaser.text =  (-(int)((GameManager.me.stageManager.nowRound.playerStartPosX - GameManager.me.stageManager.nowRound.chaser.posX) * 0.01f)) + "m";
					GameManager.me.uiManager.uiPlay.lbChaser.color = Color.white;
				}

				break;
				
			case RoundData.MODE.GETITEM:
				lbRoundInfo.text = Util.getUIText("GETITEM");
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);
				
				lbRoundTime.transform.localPosition = new Vector3(-476.6f, -14.8f, 91);
				lbRoundTime.gameObject.SetActive((GameManager.me.stageManager.nowRound.settingTime > -1));
				
				if(lbRoundTime.gameObject.activeSelf == false)
				{
					lbRoundLeftNum.transform.localPosition =new Vector3(-351,-13,91);
				}
				else
				{
					lbRoundLeftNum.transform.localPosition =new Vector3(-259,-13,91);
				}
				
				lbRoundLeftNum.gameObject.SetActive(true);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_item";
				
				_isLimitTimeMode = lbRoundTime.gameObject.activeSelf;
				_isLimitTimeMode = true;
				break;
				
			case RoundData.MODE.PVP:
				pvpContainer.gameObject.SetActive(true);
				playerHpBar.gameObject.SetActive(false);
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(false);

				playerFace.init(playerImageUrl);
				playerFace.down(playerImageUrl);

				pvpFace.init(pvpImageUrl);
				pvpFace.down(pvpImageUrl);


				_v = tfMiniMapBg.localPosition;
				_v.y = 260;
				tfMiniMapBg.localPosition = _v;

				if(pvpImageUrl != null)
				{
					if(pvpImageUrl.ToUpper().Contains("HTTP") == false)
					{
						switch(pvpImageUrl.ToUpper())
						{
						case Character.LEO:
							spPvpEmptyFace.spriteName = Character.LEO_IMG;
							break;
						case Character.CHLOE:
							spPvpEmptyFace.spriteName = Character.CHLOE_IMG;
							break;
						case Character.KILEY:
							spPvpEmptyFace.spriteName = Character.KILEY_IMG;
							break;
						default:
							spPvpEmptyFace.spriteName = Character.EMPTY_IMG;
							break;
						}
						
					}
				}

				break;
				
			case RoundData.MODE.C_RUN:
				playerHpBar.gameObject.SetActive(true);
				tfMiniMapContainer.gameObject.SetActive(false);
				challangeModeInfo.gameObject.SetActive(true);
				roundInfoContainer.SetActive(false);
				break;
				
			case RoundData.MODE.C_SURVIVAL:
				playerHpBar.gameObject.SetActive(true);
				tfMiniMapContainer.gameObject.SetActive(false);
				challangeModeInfo.gameObject.SetActive(true);
				roundInfoContainer.SetActive(false);
				break;
				
			case RoundData.MODE.C_HUNT:
				playerHpBar.gameObject.SetActive(true);
				tfMiniMapContainer.gameObject.SetActive(false);
				challangeModeInfo.gameObject.SetActive(true);
				challangeModeInfo.progress = 0.0f;
				challangeModeInfo.rank = 0;
				roundInfoContainer.SetActive(false);
				break;

			case RoundData.MODE.B_TEST:
				playerHpBar.gameObject.SetActive(true);
				tfMiniMapContainer.gameObject.SetActive(false);
				challangeModeInfo.gameObject.SetActive(true);
				challangeModeInfo.progress = 0.0f;
				challangeModeInfo.rank = 0;
				challangeModeInfo.update(0);
				roundInfoContainer.SetActive(false);
				break;



			case RoundData.MODE.HELL :
//				lbRoundInfo.text = "WAVE 1";//"지옥의 균열";
				challangeModeInfo.gameObject.SetActive(false);
				roundInfoContainer.SetActive(true);

				lbRoundTime.gameObject.SetActive(false);
				lbRoundTime.transform.localPosition = new Vector3(-440,-13,91);

				lbRoundLeftNum.gameObject.SetActive(false);
				spRoundInfoLeftNumIcon.spriteName = "img_mark_distance";
				
				lbRoundLeftNum.transform.localPosition = new Vector3(-259,-13,91);

				_isLimitTimeMode = true;

				break;

				
			default:
				lbRoundInfo.text = GameManager.me.stageManager.nowRound.mode;	
				break;
			}


		}

		if(StageManager.instance.nowPlayingGameType == GameType.Mode.Hell)
		{
			didShowHurryUp = false;
		}


		_v = panelTop.localPosition;
		_v.x = 0.0f;
		_v.z = 0.0f;
		_v.y = 190.0f;
		panelTop.localPosition = _v;
		
		_v = panelBottom.localPosition;
		_v.x = 0.0f;
		_v.z = 0.0f;		
		_v.y = -190.0f;
		panelBottom.localPosition = _v;		

#if UNITY_EDITOR
//		if(epi.GAME_DATA.localUser.userID == "88377728244541937")
//		{
//
//		}
//		else
#endif
		{
			bool showClearButton = false;

#if UNITY_EDITOR
			switch(PandoraManager.instance.localUser.userID)
			{
			case "91278939938273137":
			case "88377728244541937":
			case "88148076135084144":
				showClearButton = true;
				break;
			}
#endif

			if(DebugManager.useTestRound)
			{
				showClearButton = true;
			}

			if(EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
			{
				showClearButton = false;
			}

#if UNITY_EDITOR
			showClearButton = true;
#endif

			btnClearGame.gameObject.SetActive(showClearButton);

			try
			{
				btnSkipCutScene.gameObject.collider.enabled = showClearButton;
			}
			catch(System.Exception e)
			{

			}
		}


		initMiniMap(false);
	}

	
	
	// show, hide 메뉴는 시간이 있으면 트위닝 되어서 나타나거나 사라진다.
	// 시간이 없으면 바로 나타남.
	// ignoreGameUI를 설정하면 ui 클릭을 막을 수 있다.
	public void showMenu(float time = 0.3f, bool ignoreGameUI = false)
	{
		bcIgnoreGameTouch.gameObject.SetActive(ignoreGameUI);
		
		if(time > 0)
		{
			_v = panelTop.position;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.z = 0.0f;
			TweenPosition.Begin(panelTop.gameObject, time, _v).method = UITweener.Method.EaseInOut;		
			_v = panelBottom.position;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.z = 0.0f;
			TweenPosition tp = TweenPosition.Begin(panelBottom.gameObject, time, _v);
			tp.method = UITweener.Method.EaseInOut;		
			tp.eventReceiver = gameObject;
			tp.callWhenFinished = "showMiniMap";
		}
		else
		{
			TweenPosition tp = panelTop.gameObject.GetComponent<TweenPosition>();
			if(tp != null) tp.enabled = false;
			tp = panelBottom.gameObject.GetComponent<TweenPosition>();
			if(tp != null) tp.enabled = false;

			_v = panelTop.position;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.z = 0.0f;	
			panelTop.localPosition = _v;
			
			_v = panelBottom.position;
			_v.x = 0.0f;
			_v.y = 0.0f;
			_v.z = 0.0f;	
			panelBottom.localPosition = _v;	
			initMiniMap();
		}

		GameManager.me.battleManager.visibleTagSlots();

	}

	void showMiniMap()
	{
		initMiniMap(true);
		if(GameManager.me.playMode == GameManager.PlayMode.replay)
		{
			if(GameManager.me != null && GameManager.me.stageManager.nowRound != null)
			{
				if(GameManager.me.stageManager.nowRound.id != "INTRO")
				{
					btnReplaySpeed.gameObject.SetActive(true);
					btnReplayClose.gameObject.SetActive(true);
				}
			}
		}
	}



	public void hideTagSlot()
	{
		playerTagSlot[0].setVisible(false);
		playerTagSlot[1].setVisible(false);
		pvpTagSlot[0].setVisible(false);
		pvpTagSlot[1].setVisible(false);
	}

	
	public void hideMenu(float time = 0.3f)
	{
		if(time > 0)
		{
			_v = panelTop.position;
			
			if(_v.x != 0.0f && _v.y != 190.0f && _v.z != 0.0f)
			{
				_v.x = 0.0f;
				_v.z = 0.0f;
				_v.y = 190.0f;
				TweenPosition.Begin(panelTop.gameObject, time, _v).method = UITweener.Method.EaseInOut;		
				_v = panelBottom.position;
				_v.x = 0.0f;
				_v.z = 0.0f;		
				_v.y = -190.0f;
				TweenPosition.Begin(panelBottom.gameObject, time, _v).method = UITweener.Method.EaseInOut;				
			}
			else
			{
				hideMenu(0.0f);
			}
		}
		else
		{
			TweenPosition tp = panelTop.gameObject.GetComponent<TweenPosition>();
			if(tp != null) tp.enabled = false;
			tp = panelBottom.gameObject.GetComponent<TweenPosition>();
			if(tp != null) tp.enabled = false;

			_v = panelTop.localPosition;
			_v.x = 0.0f;
			_v.z = 0.0f;
			_v.y = 190.0f;
			panelTop.localPosition = _v;
			
			_v = panelBottom.localPosition;
			_v.x = 0.0f;
			_v.z = 0.0f;		
			_v.y = -190.0f;
			panelBottom.localPosition = _v;	
		}

		hideTagSlot();
	}


	// 배틀 애니메이션.
	public void showReadyBattleAnimation()
	{
		SoundData.play("uibt_start");

		readyBattle.play();
		//readyBattle.SetActive(true);

//		_v.x = 3.0f;
//		_v.y = 3.0f;
//		_v.z = 1.0f;
//		readyBattle.transform.localScale = _v;
//
//		_v.x = 1.0f;_v.y = 1.0f;_v.z = 1.0f;
//
//		TweenAlpha.Begin(readyBattle.gameObject, 0.5f, 1.0f).method = UITweener.Method.BounceIn;
//		TweenScale.Begin(readyBattle.gameObject, 0.5f, _v).method = UITweener.Method.BounceIn;

		//aniReadyBattle.Play();
	}

	
	public void hideReadyBattleAnimation()
	{
		readyBattle.onCompleteEffect();
		//aniReadyBattle.Stop();
		//readyBattle.SetActive(false);
		goHellGoAni.gameObject.SetActive(false);
	}

	public void showHellRoundStartAnimation()
	{
		spHellState.spriteName = "img_start_go";
		spHellState.MakePixelPerfect();
		psHellSuccess.gameObject.SetActive(false);
		goHellGoAni.gameObject.SetActive(true);
		goHellGoAni.animation.Play("BattleGo");
	}



	public void showHellRoundSuccessAnimation()
	{

		spHellState.spriteName = "img_result_success";
		spHellState.MakePixelPerfect();
		psHellSuccess.gameObject.SetActive(true);
		goHellGoAni.gameObject.SetActive(true);
		goHellGoAni.animation.Play("BattleSuccess");
		psHellSuccess.Play();
//		SoundData.play( "m_sucess" );
	}



	private StringBuilder _sb = new StringBuilder();

	public PlayerHpBar playerHpBar;
	public PlayerHpBar playerPVPMeHpBar;
	public PlayerHpBar playerPVPEnemyHpBar;

	public void updateHP(float hpPer, float hp, float maxHp, bool useAniEffect = true)
	{
		playerPVPMeHpBar.updateHP(hpPer, hp, maxHp, useAniEffect);
		playerHpBar.updateHP(hpPer, hp, maxHp, useAniEffect);
	}

	public void updateMP(float mp, float maxMp)
	{
//		_sb.Length = 0;
//		_sb.Append((int)mp);
//		_sb.Append("/");
//		_sb.Append((int)maxMp);
		lbMP.text = Mathf.RoundToInt( mp / maxMp * 100.0f) + "%";//_sb.ToString();
		pbMP.fillAmount = mp/maxMp;
	}

	public void updateSP(float sp, float maxSp)
	{
//		_sb.Length = 0;
//		_sb.Append((int)sp);
//		_sb.Append("/");
//		_sb.Append((int)maxSp);
		lbSP.text = Mathf.RoundToInt( sp / maxSp * 100.0f) + "%";//_sb.ToString();
		pbSP.fillAmount = sp/maxSp;
	}	
	
	int i=0;
//	public void updateUnitSlot()
//	{
//		for(i = 0; i < 5; ++i)
//		{
//			UIUnitSlot[i].update();
//		}
//	}
//
//	
//	public void updateSkillSlot()
//	{
//		for(i = 0; i < 3; ++i)
//		{
//			uiSkillSlot[i].update();
//		}
//	}


	float _tempF;
	float _tempF2;	


	public Camera uiPlayCamera;
	Touch _t;
	private int _minimapTouchFingerId = -10000;
	private bool _minimapTouched = false;

	private float _minimapPointPosition = 0;


	public void initMiniMap(bool visible = true)
	{
		CharacterMinimapPointer.startX = tfMiniMapStart.localPosition.x;			
		CharacterMinimapPointer.endX = tfMiniMapEnd.localPosition.x;
		CharacterMinimapPointer.yPos = tfMiniMapBg.localPosition.y - 45;
		CharacterMinimapPointer.width = CharacterMinimapPointer.endX - CharacterMinimapPointer.startX ;

		CharacterMinimapPointer.fingerStartX = tfMiniMapStart.position.x;			
		CharacterMinimapPointer.fingerEndX = tfMiniMapEnd.position.x;


		foreach(CharacterMinimapPointer mp in GameManager.me.characterManager.miniMapPointers)
		{
			mp.cachedTransform = mp.transform;
			if(mp.canInitVisibleAtStartTime) mp.visible = visible;
		}
	}


	public void setCameraClipPlane(float nearClip, float farClip)
	{
		GameManager.me.gameCamera.nearClipPlane = 1700.0f;
		GameManager.me.gameCamera.farClipPlane = 5000.0f;
	}


	void LateUpdate()
	{
#if UNITY_EDITOR

		if(Input.GetKeyUp(KeyCode.Space))
		{
			onClickClearGame(null);
		}
		else if(Input.GetKeyUp(KeyCode.Keypad0))
		{
			onClickAutoPlay(null);
		}
#endif


		if(GameManager.me.uiManager.currentUI != UIManager.Status.UI_PLAY || GameManager.me.isPaused) return;
		
		if(GameManager.me.cutSceneManager.useCutSceneCamera == true) //CutSceneManager.nowOpenCutScene == false)// || 
		{
			playCSCam();

			if(isPlayingPreCam)
			{
				if(Input.GetMouseButton(0))
				{
					GameManager.setTimeScale = 4.0f;
				}
				else if(Input.GetMouseButtonUp(0))
				{
					GameManager.setTimeScale = 1.0f;
				}
			}
		}
		else if(GameManager.me.isPlaying)// 컷씬 진행중일때는 컷씬의 카메라 셋팅이 우선된다.
		{
			bool resetCamPos = _minimapTouched;

			//initMiniMap();

#if UNITY_EDITOR

			if(_minimapTouched == false && Input.GetMouseButtonDown(0))
			{
				_v = uiPlayCamera.ScreenToWorldPoint(Input.mousePosition);
				
				if(_v.x > CharacterMinimapPointer.fingerStartX
				   && _v.x < CharacterMinimapPointer.fingerEndX
				   && _v.y < tfMiniMapTop.position.y
				   && _v.y > tfMiniMapBottom.position.y)
				{
					_minimapTouched = true;
					_minimapPointPosition= StageManager.mapStartPosX + ((StageManager.mapEndPosX - StageManager.mapStartPosX)/CharacterMinimapPointer.width*(_v.x - CharacterMinimapPointer.fingerStartX));
					if(_minimapPointPosition < StageManager.mapStartPosX) _minimapPointPosition = StageManager.mapStartPosX;
					else if(_minimapPointPosition > StageManager.mapEndPosX ) _minimapPointPosition = StageManager.mapEndPosX ;
				}
			}
			else if(_minimapTouched && Input.GetMouseButton(0))
			{
				_v = uiPlayCamera.ScreenToWorldPoint(Input.mousePosition);
				_minimapPointPosition = StageManager.mapStartPosX + ((StageManager.mapEndPosX - StageManager.mapStartPosX)/CharacterMinimapPointer.width*(_v.x - CharacterMinimapPointer.fingerStartX));
				if(_minimapPointPosition < StageManager.mapStartPosX) _minimapPointPosition = StageManager.mapStartPosX;
				else if(_minimapPointPosition > StageManager.mapEndPosX ) _minimapPointPosition = StageManager.mapEndPosX ;
			}
			else
			{
				_minimapTouched = false;
				_minimapTouchFingerId = -10000;
			}
#else
			if(Input.touchCount > 0)
			{
				for(int i = 0; i < Input.touchCount; ++i)
				{
					_t = Input.GetTouch(i);
					
					if(_minimapTouched == false && _t.phase == TouchPhase.Began)
					{
						_v = uiPlayCamera.ScreenToWorldPoint(_t.position);
						
						if(_v.x > CharacterMinimapPointer.fingerStartX
						   && _v.x < CharacterMinimapPointer.fingerEndX
						   && _v.y < tfMiniMapTop.position.y
						   && _v.y > tfMiniMapBottom.position.y)
						{

							_minimapTouchFingerId = _t.fingerId;
							_minimapTouched = true;
							_minimapPointPosition= StageManager.mapStartPosX + ((StageManager.mapEndPosX - StageManager.mapStartPosX)/CharacterMinimapPointer.width*(_v.x - CharacterMinimapPointer.fingerStartX));
							if(_minimapPointPosition < StageManager.mapStartPosX) _minimapPointPosition = StageManager.mapStartPosX;
							else if(_minimapPointPosition > StageManager.mapEndPosX ) _minimapPointPosition = StageManager.mapEndPosX ;
						}
					}
					else if(_minimapTouched && _minimapTouchFingerId == _t.fingerId)
					{
						if(_t.phase == TouchPhase.Moved)
						{
							_v = uiPlayCamera.ScreenToWorldPoint(Input.mousePosition);

							_minimapPointPosition= StageManager.mapStartPosX + ((StageManager.mapEndPosX - StageManager.mapStartPosX)/CharacterMinimapPointer.width*(_v.x - CharacterMinimapPointer.fingerStartX));
							if(_minimapPointPosition < StageManager.mapStartPosX) _minimapPointPosition = StageManager.mapStartPosX;
							else if(_minimapPointPosition > StageManager.mapEndPosX ) _minimapPointPosition = StageManager.mapEndPosX ;

						}
						if(_t.phase == TouchPhase.Ended || _t.phase == TouchPhase.Canceled)
						{
							_minimapTouched = false;
							_minimapTouchFingerId = -10000;
						}

						break;
					}
				}
			}
			else
			{
				_minimapTouched = false;
				_minimapTouchFingerId = -10000;
			}
#endif

			if(resetCamPos && _minimapTouched == false)
			{
				_v = cameraTarget.position;
				_v.z = 0.0f;
				_v2 = gameCameraContainer.position;
				_tempF = (_v.x - _v2.x);
				_v2.x += _tempF;
				_v.x = _v2.x;			
				gameCameraContainer.position = _v;
			}

			playCam();
			
			if(GameManager.me.stageManager.playTime - prevPlayTime >= 0.1f)
			{
				if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PVP)
				{
					int lt = (int)(GameManager.me.stageManager.settingTime - GameManager.me.stageManager.playTime);
					lbGameTimer.text = lt.ToString();

					if(lt <= 10)
					{
						if(lt % 2 == 0) lbGameTimer.color = Color.white;
						else lbGameTimer.color = Color.red;
					}
				}

				float currentPTime = GameManager.me.stageManager.playTime.Get();
//#if UNITY_EDITOR
				tfPlayTime.text = currentPTime.ToString("#.#");	
//#endif

				prevPlayTime = GameManager.me.stageManager.playTime;
				
				if(updateLeftTime)
				{
					if(HellModeManager.instance.isOpen)
					{
						HellModeManager.instance.updateTime();
					}
					if(challangeModeInfo.gameObject.activeInHierarchy)
					{
						challangeModeInfo.setLeftTime((int)(GameManager.me.stageManager.nowRound.settingTime - GameManager.me.stageManager.playTime));
					}
					else if(lbRoundTime.cachedGameObject.activeInHierarchy)
					{
						if(_isLimitTimeMode)
						{
							int leftTime = (int)(GameManager.me.stageManager.nowRound.settingTime - GameManager.me.stageManager.playTime);
							lbRoundTime.text = Util.secToHourMinuteSecondString(leftTime);

							if(leftTime <= 10)
							{
								if(leftTime % 2 == 0) lbRoundTime.color = Color.white;
								else lbRoundTime.color = Color.red;
							}

							if(leftTime <= 20)
							{
								if(didShowHurryUp == false)
								{
									spHurryup.gameObject.SetActive(true);
									didShowHurryUp = true;
								}
							}
						}
						else
						{
							lbRoundTime.text = Util.secToHourMinuteSecondString((int)(GameManager.me.stageManager.playTime));
						}
					}
					else
					{
						//tfLeftTime.text = ((int)(GameManager.me.stageManager.nowRound.settingTime - GameManager.me.stageManager.playTime))+"";
					}
				}
			}			
		}
		else if(GameManager.me.playGameOver)
		{
			// 게임이 종료 됐을 때의 카메라 연출.

			if(gameCamera.fieldOfView > 8)
			{
				gameCamera.fieldOfView -= Time.smoothDeltaTime * 1.5f;	
			}

			/*
			float sv = GameManager.me.playCamRenderImage.saturationAmount;
			sv -= Time.smoothDeltaTime * 0.8f;
			if(sv >= 0) GameManager.me.playCamRenderImage.saturationAmount = sv;
			*/
		}
	}



	public void initTimeUI()
	{
		if(_isLimitTimeMode)
		{
			int leftTime = (int)(GameManager.me.stageManager.nowRound.settingTime - GameManager.me.stageManager.playTime);
			lbRoundTime.text = Util.secToHourMinuteSecondString(leftTime);
			
			if(leftTime <= 10)
			{
				if(leftTime % 2 == 0) lbRoundTime.color = Color.white;
				else lbRoundTime.color = Color.red;
			}
		}
		else
		{
			lbRoundTime.text = Util.secToHourMinuteSecondString((int)(GameManager.me.stageManager.playTime));
		}
	}




	public void setPlayerDamageEffect()
	{
		StartCoroutine(startPlayerDamageEffect());
	}

	IEnumerator startPlayerDamageEffect()
	{
		spPlayerDamage.cachedGameObject.gameObject.SetActive(true);
		yield return Util.ws01;
		spPlayerDamage.cachedGameObject.gameObject.SetActive(false);
	}


}

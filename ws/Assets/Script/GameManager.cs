using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour {

	public static bool isDebugBuild = false;


	public const int FRAME_RATE_LOW = 30;
	public const int FRAME_RATE_HIGH = 50;

	private Vector3 _v = new Vector3(0.0f, 0.0f, 0.0f); // 임시로 쓸 vector3.	
	private Vector3 _v2;	

	public const int MAX_ACT = 6;

	public bool isGuest = false;

	public bool showOpening = false;

	public bool showWindRunner = true;

	public bool useAssetBundleMapFile = true;

	public static GameManager me;

	public UINetworkLock networkLock;
	public EffectManager effectManager;	
	public BulletManager bulletManager;
	public MapManager normalMapManager;
	public MapManager cutSceneMapManager;

	public BattleManager battleManager;


	public static NetworkManager networkManager;	
	public static GameDataManager gameDataManager;
	public static SoundManager soundManager;
	public static AndroidManager androidManager;
	public static ResourceManager resourceManager;	
	public static GameInfoManager info;
	public static ReplayManager replayManager;

	public SpecialPackageManager specialPackageManager = new SpecialPackageManager();

	public PerformanceManager performanceManager;
	public ClientDataLoader clientDataLoader;
	
	public MapManager mapManager;
	public StageManager stageManager;
	public CharacterManager characterManager = null;
	public UIManager uiManager;
	public MethodManager methodManager = new MethodManager();

	public Camera gameCamera;
	public FlareLayerSetter flareLayer;

	public GameObject gameCameraContainer;
	public Camera baseUICamera; // 메인 UI용 카메라 (NGUI)

	public Camera inGameGUICamera;
	public Camera inGameGUIGameCamera;
	public Camera inGameGUIPreviewCamera;


	public static Xfloat globalDeltaTime = 0.0f;
	public static float renderDeltaTime = 0.0f;
	
	public c_tk2dGameCamera tk2dGameCamera;
	
	public Transform stage;
	
	public Transform assetPool;
	
	public float globalTime = 0;
	
	public static float globalGamePassTime = 0.0f;
	
	public bool isInit = false;

	public static Vector2 screenSize = Vector2.zero;
	public static Vector2 screenCenter = Vector2.zero;

	public static float screenWidth = 1024;
	public static float screenHeight = 1024;

	public Player player;
	public Player pvpPlayer;

//	public Player[] myPlayers = new Player[2];
//	public Player[] enemyPlayers = new Player[2];

	public GameObject monster;
	public GameObject mapObject;
	
	public Transform deletePool;

	private bool _canUseAutoPlay = false;
	public bool canUseAutoPlay
	{
		set
		{
			_canUseAutoPlay = value;
			uiManager.uiPlay.btnAutoPlay.gameObject.SetActive(_canUseAutoPlay);
			uiManager.uiPlay.btnFastPlay.gameObject.SetActive(_canUseAutoPlay);
		}
		get
		{
			return _canUseAutoPlay;
		}
	}



	public bool isAutoPlay
	{
		set
		{
#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation) return;
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker) return;

			if(DebugManager.instance.useDebug)
			{
				DebugManager.instance.isAutoPlay = value;
				return;
			}
#endif
			PlayerPrefs.SetInt("AUTOPLAY",(value)?WSDefine.YES:WSDefine.NO);
			DebugManager.instance.isAutoPlay = value;
		}
		get
		{
#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation)
			{
				return false;
			}


			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker) return false;

			if(DebugManager.instance.useDebug)
			{
				return DebugManager.instance.isAutoPlay;
			}
#endif

			if(recordMode != RecordMode.record)
			{
				return replayManager.isAutoPlayContinueGame;
			}
			
			if(_canUseAutoPlay)
			{
				return DebugManager.instance.isAutoPlay;
			}

			return false;
			
		}
	}



	private Xbool _isFastPlay = false;
	public bool isFastPlay
	{
		set
		{
			PlayerPrefs.SetInt("FASTPLAY",(value)?WSDefine.YES:WSDefine.NO);
			_isFastPlay= value;
		}
		get
		{

			if(_canUseAutoPlay)
			{
				return _isFastPlay;
			}

			return false;
			
		}
	}






	private bool _isMuteSound = false;
	public bool isMuteBgm
	{
		set
		{
			_isMuteSound = value;

			PlayerPrefs.SetInt("MUSIC",(value)?1:0);

			if(soundManager != null) soundManager.muteBGM(value);
		}
		get
		{
			return _isMuteSound;
		}
	}


	private bool _isMuteSFX = false;
	public bool isMuteSFX
	{
		set
		{
			_isMuteSFX = value;

			PlayerPrefs.SetInt("SFX",(value)?1:0);

			if(soundManager != null) soundManager.muteSFX(value);
		}
		get
		{
			return _isMuteSFX;
		}
	}


	void loadLocalData()
	{
		_isMuteSFX = (PlayerPrefs.GetInt("SFX") == 0)?false:true;
		_isMuteSound = (PlayerPrefs.GetInt("MUSIC") == 0)?false:true;

		isAutoPlay = ( PlayerPrefs.GetInt("AUTOPLAY",WSDefine.NO) == WSDefine.YES);

		UIMission.loadLocalData();

		UIPopupInstantDungeon.loadLocalData();

	}

	public Xbool isPlaying = false;
	
	private Xbool _isPaused = false;

	private Vector2 _touchRate = Vector2.zero;
	
	public CutSceneManager cutSceneManager;
	
	public static bool isLoadedGame	= false;	
	
	public c_tk2dGameCamera tk2dCam;
	
	public static string guiLog = "";
	
	public bool showLog = true;
	
	public const int TOAST_SHORT = 0;
	public const int TOAST_LONG = 1;	
	
	private static string _logString = string.Empty;
	public UILabel logger;	

	private Vector3 _tempVector;
	
	private float _touchPosX = 0;
	private int _fingerId = -1000;	

	public Xbool playGameOver = false;
	
	// 주인공이 총알을 쏘는 코드.
	private bool _isMouseDown = false;
	private Vector2 _prevMousePosition = Vector2.zero;
	private Vector2 _touchDeltaPosition = Vector2.zero;	

	public static float camRatioY;


	public Scene.IntroStep introStep = Scene.IntroStep.IntroProgress;


	void Awake()
	{


		_waitForTouchToStartGame = false;

		isPlaying = false;

		isStartGame = false;
//		Debug.LogError(Application.dataPath);
//		QualitySettings.masterTextureLimit = 1;
//		QualitySettings.pixelLightCount = 0;
//		QualitySettings.shadowDistance = 0;
//		QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;


#if UNITY_EDITOR
		Application.targetFrameRate = FRAME_RATE_HIGH;
		QualitySettings.vSyncCount = 0; 
#endif



#if !UNITY_EDITOR
		showOpening = false;
#endif

//		DontDestroyOnLoad(gameObject);
//		if(me==null){
			
//		}else{
//			DestroyImmediate(this.gameObject);
//			return;
//		}

		if(me != null)
		{
			GameObject.DestroyImmediate(me.gameObject);
		}

		me = this;

		loadLocalData();
		
		Resolution resolution = Screen.currentResolution;
		
		screenSize = tk2dGameCamera.targetResolution;
		
		screenCenter.x = screenSize.x * 0.5f;
		screenCenter.y = screenSize.y * 0.5f;

//		Log.log("screen width : " + Screen.width , " height: " + Screen.height + "  resolution : " + resolution);		
//		Log.log("screenSize.x : " + screenSize.x + " screenSize.y: " + screenSize.y);
		
		_touchRate.x = screenSize.x / Screen.width / tk2dGameCamera.mainCamera.rect.width;
		_touchRate.y = screenSize.y / Screen.height / tk2dGameCamera.mainCamera.rect.height;	
		
		setAngleData();
		
		mapManager = normalMapManager;

		if(Monster.ATK_INDEX == null)
		{
			Monster.ATK_INDEX = new Dictionary<string, int>();
			Monster.ATK_INDEX.Add("atk",0);

			for(int i = 0; i < 20; ++i)
			{
				Monster.ATK_INDEX.Add("atk"+i,i);
			}
		}

		camRatioY = screenSize.y / Screen.height;
	}



	public static Well512Random inGameRandom = new Well512Random(4989);

	void Start () 
	{
//		Debug.LogError("d46e2a : " + Util.HexToColor("d46e2a"));

		// IOS에서는 persistentDataPath 를 쓰면 안된다.

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false) Log.log(Application.persistentDataPath);
#endif
		
#if UNITY_ANDROID && !UNITY_EDITOR
		//androidManager = AndroidManager.instance;
#endif
		// 기본적인 게임 데이터 지정
		gameDataManager = GameDataManager.instance;
		
		// 리소스 관리를 위한 매니저
		resourceManager = ResourceManager.instance;
		
		effectManager.init();
		
		info = GameInfoManager.instance;
		
		soundManager = SoundManager.instance;

		characterManager.init();
		 
		stageManager = StageManager.instance;

//		Debug.LogError("stageManager : " + stageManager);

		replayManager = ReplayManager.instance;

		bulletManager.init();

		specialPackageManager.init();

		// 네트웍 초기화
		networkManager = NetworkManager.instance;

		restartAppGame();

//#if !UNITY_EDITOR
//		isMuteSound = false;
//
//		soundManager.mute(false);
//#endif
	}



	public void restartAppGame()
	{
//		EpiServer.instance.InitLogin();
		isStartGame = false;

		soundManager.stopAllSounds();

		initCamera();
		
		uiManager.init();
		uiManager.changeUI(UIManager.Status.UI_TITLE);

		soundManager.clearSound();
		characterManager.clearStage();

		isPaused = false;

		GameManager.setTimeScale = 1.0f;

		StartCoroutine(initNetwork());
	}





	private IEnumerator initNetwork()
	{

		bool initSDK = true;

		yield return null;

		float lastCheckTime = Time.realtimeSinceStartup;
		while(WemeManager.Instance == null || PandoraManager.instance == null || PandoraManager.instance.isStart == false || EpiServer.instance == null || PlatformManager.instance == null)
		{
			yield return null;
			if(Time.realtimeSinceStartup - lastCheckTime > 5.0f)
			{
				initSDK = false;
				break;
			}
		}

		if(initSDK == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("SDK_FAILED"), NetworkManager.RestartApplication, NetworkManager.RestartApplication);
			yield break;
		}

		// if 동의를 했으면 
		// 얘는 말 그대로 단순 서버 체크.
		//EpiServer.instance.serverCheck();
		// else 동의를 받는다.
		
#if UNITY_EDITOR
		
		if(DebugManager.instance.useDebug)
		{
			DebugManager.instance.autoLogin();
		}
		else
		#endif		
		{
			if(Weme.mainSceneLoadingAsync != null)
			{
				while(Weme.mainSceneLoadingAsync.isDone == false)
				{
					yield return null;
				}

				Weme.mainSceneLoadingAsync = null;
			}

			yield return new WaitForSeconds(0.1f);

			UIManager.setGameState( Util.getUIText("CHECK_WEMEGATE") , 0.1f);

			yield return new WaitForSeconds(0.1f);

			EpiServer.instance.serverStart();
		}
	}


	// TOC_INIT 까지 끝난 상태다.
	public void onCompleteInitKakao()
	{
		Debug.Log("onCompleteInitKakao");
		StartCoroutine(preLoad ());
	}

	private IEnumerator preLoad()
	{
		resourceManager.initInGameUI();

		#if UNITY_ANDROID 
		
		while(resourceManager.uiImageLoadComplete == false)
		{
			yield return null;
		}
		#endif


		#if UNITY_ANDROID
		if(resourceManager.resourceLoaderGo == null)
		{
			UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("UIINIT_FAILED"), NetworkManager.RestartApplication, NetworkManager.RestartApplication);
			yield break;
		}
		#endif



		AssetBundleManager.instance.startLoadNGUIAsset();
		
		while(AssetBundleManager.instance.completeLoadingNGUIAsset == false)
		{ 
			yield return null;
		}
		
		
		AssetBundleManager.instance.startLoadPlayerTextureAsset();
		
		while(AssetBundleManager.instance.completeLoadingPlayerTextre == false)
		{ 
			yield return null;
		}



		AssetBundleManager.instance.startLoadCharacterSound();
		
		while(AssetBundleManager.instance.completeLoadingCharacterSound == false)
		{ 
			yield return null;
		}




//		Debug.LogError("=========== preLoad ====");

		UIManager.setGameState( Util.getUIText("CHAR_LOADING") + " 0%" , 0.85f);

		playGameOver = false;
		
		// 최초 시작시 기본 데이터들을 불러오는 곳.. 프리로딩 부분.
		gameDataManager.loadDefaultModelData();
		
		while(gameDataManager.isCompleteLoadModel == false)
		{ 
			yield return new WaitForSeconds(0.2f);	
			UIManager.setGameState( Util.getUIText("CHAR_LOADING") + " " + (  Mathf.RoundToInt(GameDataManager.progress * 99.0f)  ) + "%", (0.85f) +  ( GameDataManager.progress * 0.1f) );
		}

		while(gameDataManager.isCompleteLoadMap == false)
		{ 
			yield return null;
		}

		while(gameDataManager.isCompleteLoadLobbyMap == false) { yield return null; }



		effectManager.addDefaultEffect();
		effectManager.startLoadEffects(true);
		while(effectManager.isCompleteLoadEffect == false) { yield return null; };

		effectManager.initChargingEffect();


		// 이펙트도 미리 불러온다..

#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.gameResourceErrorCheck)
		{

			foreach(KeyValuePair<string, EffectData> kv in GameManager.info.effectData)
			{
//				if(kv.Value.preloadNum > 0)
				{
					switch(kv.Value.type)
					{
					case EffectData.ResourceType.PARTICLE:
					case EffectData.ResourceType.BULLET:
					case EffectData.ResourceType.CHAIN:
					case EffectData.ResourceType.PREFAB:
					case EffectData.ResourceType.SCENE:
						effectManager.addLoadEffectData(kv.Value.id, true );
						break;
					}

					GameManager.info.effectData[kv.Key].preloadNum = 1;
				}
			}


			effectManager.startLoadEffects(true);
			while(effectManager.isCompleteLoadEffect == false)
			{
				yield return null;
			}
		}
#endif


		foreach(KeyValuePair<string, EffectData> kv in GameManager.info.effectData)
		{
			if(kv.Value.type == EffectData.ResourceType.PARTICLE)
			{
				GameManager.me.effectManager.preloadParticleEffects(kv.Value.id, kv.Value.resource, kv.Value.preloadNum);
			}
		}

		while(effectManager.isCompleteLoadEffect == false) { yield return null; };

#if UNITY_EDITOR

		if(DebugManager.instance.useDebug)
		{
			changeMainPlayer( null, DebugManager.instance.defaultHero, "FENRIR");
		}
		else
		{
			changeMainPlayer(null, "LEO", "FENRIR");
		}

#else
		changeMainPlayer(null, "LEO", "FENRIR");
#endif
		clearMemory();

		//GameManager.me.player.setPositionCtransform( new Vector3(1000,1000,0) );		

		UIManager.setGameState( Util.getUIText("CHAR_LOADING") + " 100%", 0.95f);

		yield return null;		

		UIManager.setGameState( Util.getUIText("INIT_UI"), 0.96f, 0.4f, 0.04f);

		yield return null;

		effectManager.startLoadEffects(true);
		while(effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}

		SoundManager.instance.addLoadSoundData("bgm_maintheme");

		SoundManager.instance.startLoadSounds(true);

		while(SoundManager.instance.isCompleteLoadSound == false)
		{
			yield return null;
		}



		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
		
		}
		else
			#endif
		{

#if UNITY_IPHONE
			if(GameDataManager.instance.serviceMode == GameDataManager.ServiceMode.IOS_SUMMIT)
			{
				uiManager.uiMenu.uiLobby.btnEvent.gameObject.SetActive(false);				
			}
			else
#endif
			{
				GameDataManager.instance.receivedEventUrl = false;
				EpiServer.instance.sendGetEventUrl();
				
				while(GameDataManager.instance.receivedEventUrl == false)
				{
					yield return null;
				}
			}
		}


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			onCompleteGetReinforceData();
		}
		else
#endif
		{
			EpiServer.instance.sendGetRecom();
		}
	}



	public void onCompleteGetReinforceData()
	{

		// load title image 는 사실은 apk hash 값을 서버로 보내는 코드다.

#if !UNITY_EDITOR && UNITY_ANDROID
		resourceManager.loadTitleImageComplete();
#else

		#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker)
		{
			_canStartGame = true;
			preLoadComplete();	
		}
		else
			#endif
		{
			Application.LoadLevelAdditiveAsync("scene_runemakestudio_ver2");
		}

#endif
	}


	bool _canStartGame = false;
	public void onCompleteInitRunemakeStudio()
	{
		_canStartGame = true;

		introStep = Scene.IntroStep.MissingPaymentCheck;

		if(EpiServer.instance.confirmMissingPurchase())
		{
			UIManager.setGameState(Util.getUIText("CHECK_CONSUME"));

			Invoke("preLoadComplete", 5.0f);
		}
		else
		{
			preLoadComplete();
		}
	}


	public void setCanStartGame(bool value)
	{
		_canStartGame = value;
	}

	// init -> kakao -> 기본 모델링 파일까지 다 읽어온 상태.
	public void preLoadComplete()
	{
		UINetworkLock.instance.hide();

		if(_canStartGame == false) return;

		_canStartGame = false;

		#if UNITY_EDITOR
		if(showOpening)
		{
			startPlayMainGame();
			return;
		}
		else if(PandoraManager.instance.localUser.userID == GameDataManager.instance.name )
		{
			needCheckNickName = true;
		}
		#endif


		#if !UNITY_EDITOR
		if(PandoraManager.instance.localUser.userID == GameDataManager.instance.name )
		{
			string introKey = "";
			
			try
			{
				introKey = PandoraManager.instance.localUser.userID.Substring(PandoraManager.instance.localUser.userID.Length - 2);
			}
			catch(Exception e)
			{
			}

			if((PlayerPrefs.GetInt("COMPLETE_INTRO"+introKey,0) == 0) && GameDataManager.instance.maxAct < 2)
			{
				showOpening = true;
			}
			else
			{
				needCheckNickName = true;							
			}
		}
		
		#endif
		//		Debug.LogError("오프닝은 강제 삭제중.");
		
		startPlayMainGame();
	}



	bool _waitForTouchToStartGame = false;

	// 서버에서 데이터를 받고 인증도 끝나면 메인 게임 로직을 시작한다.
	// 게임 데이터에 따라 모델링을 부르고 로비 화면으로 진입한다.
	public void startPlayMainGame()
	{

		#if UNITY_EDITOR
		
		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			if(uiManager.uiTitle != null) uiManager.uiTitle.hide();
			CutSceneMaker.instance.play();
			return;
		}

		if(CutSceneMakerForDesigner.instance.useCutSceneMaker)
		{
			if(uiManager.uiTitle != null) uiManager.uiTitle.hide();
			CutSceneMakerForDesigner.instance.play();
			return;
		}


		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker)
		{
			if(uiManager.uiTitle != null) uiManager.uiTitle.hide();
			UnitSkillCamMaker.instance.play();
			return;
		}

		if(DebugManager.instance.useDebug == false)

		#endif
		{
			TutorialManager.instance.uiInit();
		}

		playGameOver = false;

		if(Weme.instance != null && Weme.instance.loadingEndFsm != null) Weme.instance.loadingEndFsm.enabled = true;

		_waitForTouchToStartGame = true;

		StartCoroutine(startPlayMainGameCT());


	}
















	IEnumerator startPlayMainGameCT()
	{
		yield return new WaitForSeconds(1.5f);

		UIManager.setGameState(Util.getUIText("TOUCH_SCREEN"));
		if(uiManager.uiTitle != null) uiManager.uiTitle.showTouchNotice(true);

		SoundData.play("bgm_maintheme", true);

		GameManager.me.clearMemory(2.5f);
	}

	void closeTitleAndPlayGame()
	{
		GameManager.setTimeScale = 1.0f;

		uiManager.uiTitle.showTouchNotice(false);

		_waitForTouchToStartGame = false;

		if(showOpening)
		{
			StartCoroutine(startOpening());
			return;
		}

		introStep = Scene.IntroStep.AttendanceCheck;

		uiManager.changeUI(UIManager.Status.UI_MENU, true);

		if(uiManager.uiTitle != null) uiManager.uiTitle.close();
		GameManager.me.clearMemory();

		StartCoroutine(checkNoticeAndAttendance(true));
	}


	// isAfterGame <= 얘가 true면 게임이 한판 끝난뒤 체크하는 것. 아니면 정말 처음부터 게임을 시작하는 것. (타이틀 보고나서 여기로...)
	// 게임이 끝난뒤에 여기로 오는 이유는 인트로를 보고나서 닉네임도 보기 때문.

	IEnumerator checkNoticeAndAttendance(bool checkingContinueGame = false, bool isAfterGame = false)
	{

		if(isAfterGame == false)
		{
			introStep = Scene.IntroStep.AttendanceCheck;
			
			if(GameDataManager.instance.attendData != null)
			{
				GameManager.me.uiManager.popupAttend.show();
			}
			
			while(GameManager.me.uiManager.popupAttend.isPlaying)
			{
				yield return null;
			}

			// 프로모션 추가.
			PandoraManager.instance.showWemePromotion();


			if(checkSystemPopup())
			{
				while(nowCheckingSystemPopup)
				{
					yield return null;
				}
			}

			introStep = Scene.IntroStep.PlayGame;
		}

		if(needCheckNickName)
		{
			needCheckNickName = false;
			GameManager.me.uiManager.popupNickName.show();
			GameDataManager.instance.lastPlayStatus = WSDefine.LAST_PLAY_STATUS_NONE;
		}
		else if(checkingContinueGame)
		{
			if(checkContinueGame() == false) // 이어하기가 아닐때만 바로가기.
			{
				checkDirectGo();
			}
		}
		else 
		{
			checkDirectGo();
		}



		
		#if UNITY_EDITOR
		DebugManager.instance.startAutoCombatMode();
		#endif
	}




	void checkDirectGo()
	{
		if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None) return;

		switch(uiManager.uiMenu.directGoIndex)
		{
		case UIMenu.UIPosition.Hero:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.HERO)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.HERO);
			}
			
			break;
		case UIMenu.UIPosition.Summon:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.SUMMON)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.SUMMON);
			}
			
			break;
		case UIMenu.UIPosition.Skill:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.SKILL)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.SKILL);
			}
			
			break;
		case UIMenu.UIPosition.Lobby:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.LOBBY)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.LOBBY);
			}
			
			break;
		case UIMenu.UIPosition.Friend:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.FRIEND)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.FRIEND);
			}
			
			break;
		case UIMenu.UIPosition.Mission:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.MISSION)
			{
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.MISSION);
			}
			
			break;
		case UIMenu.UIPosition.Shop:
			
			if(GameManager.me.uiManager.popupShop.gameObject.activeSelf != null)
			{
				GameManager.me.uiManager.popupShop.showItemShop();
				//GameManager.me.uiManager.popupShop.
			}
			
			break;
		case UIMenu.UIPosition.WorldMap:
			
			if(uiManager.uiMenu.currentPanel != UIMenu.WORLD_MAP)
			{
				
			}
			
			break;
		}

		uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.None;
	}



	bool checkContinueGame()
	{
		bool hasContinueGame = replayManager.hasContinueGame();

		// 이어하기 체크

		//		* 서버에는 전투시작 기록이 존재하나, 폰 변경 or 삭제 등의 이유로 중간저장 파일이 존재하지 않을때 → 해당 전투는 패배처리					
		//		- <탐험모드> 해당 라운드가 2라운드 이상인 경우, 라운드 초기화					
		//		- <도전모드> 무효처리					
		//		- <챔피언십> 해당 경기 패배처리					
		//		- <친선경기> 패배처리 					

		switch(GameDataManager.instance.lastPlayStatus)
		{
		case WSDefine.LAST_PLAY_STATUS_NONE:
			
			if(TutorialManager.instance.check("T1") == false)
			{
				if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None) // 이어하기가 없고 바로가기가 없을때만 스페셜 패키지 노출.
				{
					if(GameManager.me.specialPackageManager.check())
					{
						return true;
					}
				}
			}
			replayManager.deleteTempFile();
			break;

		case WSDefine.LAST_PLAY_STATUS_EPIC:
			if(hasContinueGame)
			{
				if(VersionData.isCompatibilityVersion( replayManager.continueVersion , true) == false)
				{
					EpiServer.instance.sendInvalidateGame();
				}
				else
				{
					char[] roundData = replayManager.continueRoundId.ToCharArray();

					bool canContinue = true;

					if(canContinue && roundData.Length == 6)
					{

						UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText( "CONTINUE_EPIC", (roundData[1]).ToString() , roundData[3].ToString() , roundData[5].ToString() )
						                   , GameManager.me.startContinueGame, EpiServer.instance.sendDeniedContinueEpic);
						GameManager.me.uiManager.uiMenu.changePanel(UIMenu.WORLD_MAP);

						return true;
					}
					else
					{
						EpiServer.instance.sendDeniedContinueEpic();
					}
				}
			}
			else
			{
				EpiServer.instance.sendDeniedContinueEpic();
			}

			break;


		case WSDefine.LAST_PLAY_STATUS_CHAMPIONSHIP:
			if(hasContinueGame)
			{
				if(VersionData.isCompatibilityVersion( replayManager.continueVersion , true) == false || GameDataManager.instance.championshipStatus != WSDefine.CHAMPIONSHIP_OPEN)
				{
					EpiServer.instance.sendInvalidateGame();
					GameDataManager.instance.lastPlayStatus = WSDefine.LAST_PLAY_STATUS_NONE;
				}
				else
				{
					EpiServer.instance.sendChampionShipData(true);
					return true;
				}
			}
			else
			{
				EpiServer.instance.sendDeniedContinuePVP();
				// 패배 처리.
			}

			break;

		case WSDefine.LAST_PLAY_STATUS_HELL:
			if(hasContinueGame)
			{
				if(VersionData.isCompatibilityVersion( replayManager.continueVersion , true) == false)
				{
					EpiServer.instance.sendInvalidateGame();
				}
				else
				{
					char[] roundData = replayManager.continueRoundId.ToCharArray();
					
					bool canContinue = true;

//					Debug.LogError("HellModeManager.instance.continueHellWave : " + HellModeManager.instance.continueHellWave);
//					Debug.LogError(HellModeManager.instance.continueHellWave == 1);

					if(canContinue && 

					   // 웨이브 1일때는 서버에 저장한 데이터가 없다.
					   (HellModeManager.instance.continueHellWave == 1 || 
					 (
						// 서버에 저장한 데이터가 있다면 모두 비교한다.
						GameDataManager.instance.lastHellWaveInfo != null && 
						Convert.ToInt32(GameDataManager.instance.lastHellWaveInfo[0]) == HellModeManager.instance.continueHellWave &&
						GameDataManager.instance.lastHellWaveInfo.Length >= 19) )
					   )
					   
					{

						UISystemPopup.open(UISystemPopup.PopupType.YesNo,Util.getUIText("CONTINUE_HELL")
						                   , GameManager.me.startContinueGame, EpiServer.instance.sendInvalidateGame);
						GameManager.me.uiManager.uiMenu.changePanel(UIMenu.WORLD_MAP);
						return true;
					}
					else
					{
						EpiServer.instance.sendInvalidateGame();
					}
					
					
					//					EpiServer.instance.sendPrepareEpic(true);
				}
			}
			else
			{
				EpiServer.instance.sendInvalidateGame();
			}
			break;


		case WSDefine.LAST_PLAY_STATUS_FRIENDPVP:

			if(hasContinueGame)
			{
				if(VersionData.isCompatibilityVersion( replayManager.continueVersion , true) == false || epi.GAME_DATA.appFriendDic.ContainsKey(GameManager.me.uiManager.popupChampionshipResult.enemyId) == false)
				{
					EpiServer.instance.sendInvalidateGame();
					GameDataManager.instance.lastPlayStatus = WSDefine.LAST_PLAY_STATUS_NONE;
				}
				else
				{
					UISystemPopup.open(UISystemPopup.PopupType.YesNo,Util.getUIText("CONTINUE_FRIENDLY")+(epi.GAME_DATA.appFriendDic[GameManager.me.uiManager.popupChampionshipResult.enemyId].f_Nick), startContinueGame, EpiServer.instance.sendDeniedContinueFriendPVP);
					GameManager.me.uiManager.uiMenu.changePanel(UIMenu.WORLD_MAP);
					return true;
				}
			}
			else
			{
				EpiServer.instance.sendDeniedContinueFriendPVP();
				// 패배 처리.
			}
			break;
		}

		return false;
	}



	public void startContinueGame()
	{
		GameDataManager.instance.lastPlayStatus = WSDefine.LAST_PLAY_STATUS_NONE;

		switch(replayManager.continueGameType)
		{
		case RoundData.TYPE.EPIC:

			char[] roundData = replayManager.continueRoundId.ToCharArray();

			int playAct = 1; 
			int.TryParse(roundData[1].ToString(), out playAct);

			int playStage = 1; 
			int.TryParse(roundData[3].ToString(), out playStage);

			int playRound = 1; 
			int.TryParse(roundData[5].ToString(), out playRound);

			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[replayManager.continueRoundId], GameType.Mode.Epic, playAct, playStage, playRound);
			break;

		case RoundData.TYPE.CHAMPIONSHIP:
			if(GameDataManager.instance.championshipData != null && 
			   GameDataManager.instance.championshipData.champions.ContainsKey(GameManager.me.uiManager.popupChampionshipResult.enemyId))
			{
				UIPlay.playerImageUrl = PandoraManager.instance.localUser.image_url;
				if(UIPlay.playerImageUrl == null) UIPlay.playerImageUrl = "";
				UIPlay.pvpImageUrl = GameDataManager.instance.championshipData.champions[GameManager.me.uiManager.popupChampionshipResult.enemyId].imageUrl;
				if(UIPlay.pvpImageUrl == null) UIPlay.pvpImageUrl = "";
			}
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[replayManager.continueRoundId],GameType.Mode.Championship);
			break;

		case RoundData.TYPE.FRIENDLY:
			UIPlay.playerImageUrl = PandoraManager.instance.localUser.image_url;
			UIPlay.pvpImageUrl = epi.GAME_DATA.appFriendDic[GameManager.me.uiManager.popupChampionshipResult.enemyId].image_url;
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[replayManager.continueRoundId],GameType.Mode.Friendly);
			break;

		case RoundData.TYPE.CHALLENGE:
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData[replayManager.continueRoundId],GameType.Mode.Challenge);
			break;

		case RoundData.TYPE.HELL:
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData["HELL"],GameType.Mode.Hell);
			break;
		}


		recordMode = RecordMode.continueGame;
		GameManager.me.uiManager.showLoading();
		GameManager.me.startGame(0.5f, null, true);
	}



	public IEnumerator startOpening()
	{
		UIManager.setGameState( Util.getUIText("LOADING_OPENING") );
		UIManager.setTitleProgress(0.95f);

		Debug.LogError("startOpening!!!");

		stageManager.setNowRound(GameManager.info.roundData["INTRO"],GameType.Mode.Epic);
		loadRoundMonsterModelData();
		while(gameDataManager.isCompleteLoadMap == false){ yield return null; };
		while(gameDataManager.isCompleteLoadModel == false){ yield return null; };
		while(SoundManager.instance.isCompleteLoadSound == false){ yield return null; };
		while(effectManager.isCompleteLoadEffect == false){ yield return null; };

		SoundManager.instance.loadCutSceneSoundAsset(null);
		while(SoundManager.nowLoadingCutSceneAsset) { yield return null;  };

		#if UNITY_IOS
		if(iPhone.generation == iPhoneGeneration.iPhone4 ||
		   iPhone.generation == iPhoneGeneration.iPhone4S ||
		   iPhone.generation == iPhoneGeneration.iPodTouch1Gen ||
		   iPhone.generation == iPhoneGeneration.iPodTouch2Gen ||
		   iPhone.generation == iPhoneGeneration.iPodTouch3Gen  ||
		   iPhone.generation == iPhoneGeneration.iPodTouch4Gen ||
		   iPhone.generation == iPhoneGeneration.iPodTouch5Gen ||
		   iPhone.generation == iPhoneGeneration.iPodTouchUnknown
		   )
		{
			startOpeningCutScene();
		}
		else 
		#endif
		{

			if(GameDataManager.instance.lobbyMapResource.ContainsKey( UILobby.LOBBY_MAP_NAMES[0] ))
			{
				if(GameDataManager.instance.lobbyMapResource[ UILobby.LOBBY_MAP_NAMES[0] ] != null)
				{
					GameDataManager.instance.lobbyMapResource[ UILobby.LOBBY_MAP_NAMES[0] ].SetActive(false);
				}
			}

#if UNITY_EDITOR
			if(showWindRunner == false)
			{
				startOpeningCutScene();
				yield break;
			}
#endif

			if(PerformanceManager.isLowPc)
			{
				startOpeningCutScene();
			}
			else
			{
				yield return Application.LoadLevelAdditiveAsync("windrunner");
			}
		}
	}


	public void changeMainPlayer(GamePlayerData gpd , string heroResourceName , string vehicleResourceName , bool loadPlayerEffectOnly = false)
	{
//		Debug.LogError("changeMainPlayer : " + player);

		bool needCSCamTargetChange = false;

		if(GameManager.me.uiManager.uiPlay.cameraTarget != null && player != null && GameManager.me.uiManager.uiPlay.cameraTarget == player.cTransform)
		{
			needCSCamTargetChange = true;
		}

		if(player != null)
		{
//			Debug.LogError("changeMainPlayer clean: " + player);
			GameManager.me.characterManager.cleanMonster(player);
		}

		player = (Player)characterManager.getMonster(true,true, heroResourceName,false);

		if(gpd == null) gpd = GameDataManager.instance.heroes[heroResourceName];

		player.init(gpd, true, true, 0);

		player.pet = (Pet)characterManager.getMonster(false, true,vehicleResourceName,false);
		player.pet.init(player);

		player.isEnabled = false;

		if(needCSCamTargetChange)
		{
			GameManager.me.uiManager.uiPlay.cameraTarget = player.cTransform;
		}

		effectManager.loadEffectFromPlayerData(gpd, loadPlayerEffectOnly);

	}

	public Player getCurrentPlayer(bool isIngamePlayer = false, bool includePet = true)
	{
		Player p = (Player)characterManager.getMonster(true,true,GameDataManager.instance.selectHeroId,isIngamePlayer);
		p.init(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId],true,false);

		if(includePet)
		{
			p.pet = (Pet)characterManager.getMonster(false,true,GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId].partsVehicle.parts.resource.ToUpper(),false);
			p.pet.init(p);
		}

		p.isEnabled = true;
		return p;
	}


	bool needCheckNickName = false;

	public void startOpeningCutScene()
	{
		StartCoroutine(openingCutSceneCT());
	}

	IEnumerator openingCutSceneCT()
	{
		Debug.LogError("Start Opening!");
		uiManager.changeUI(UIManager.Status.UI_OPENING);

		if(replayManager != null) replayManager.init(0);
		
		if(DebugManager.useTestRound == false) needCheckNickName = true;

		uiManager.uiPlay.resetCamera();

		player.isEnabled = true;

		player.state = Monster.NORMAL;		

		normalMapManager.createBackground(stageManager.getMapId(), true);

		mapManager.visible = true;

		currentScene = Scene.STATE.PLAY_INTRO;

		GameManager.me.cutSceneManager.initCutScene();

		GameManager.me.cutSceneManager.roundStateCheck();

		yield return null;
//		Debug.LogError("destroyWindRunner " + WindRunnerMain.instance);
		if(WindRunnerMain.instance != null)
		{
			WindRunnerMain.instance.destroy();
		}

		if(uiManager.uiTitle != null) uiManager.uiTitle.close();

		yield return null;

	}

	
	public bool isPaused
	{
		get
		{
			return _isPaused;
		}
		set
		{
			GameManager.setTimeScale = (value)?0.0f:1.0f;
			_isPaused = value;
		}
	}

	public RenderImage playCamRenderImage;
	
	public void startGameOver(float waitTime = 1.5f)
	{
#if UNITY_EDITOR
		Debug.LogError("== Start Game Over!! == : " + GameManager.me.stageManager.playTime);
#endif

		if(playGameOver == false)
		{
			playGameOver = true;

			//playCamRenderImage.saturationAmount = 1.0f;
			//playCamRenderImage.enabled = true;

			StartCoroutine( gameOver(waitTime) );
		}
	}



	public XString successType = "";




	private float _lastSendCompleteTime = -1000.0f;

	public void onCompleteRound(string type)
	{
		successType.Set(type);

		soundManager.stopGroanVoice();

		uiManager.uiPlay.warningAlpha.gameObject.SetActive(false);

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug || DebugManager.useTestRound )
#else
		if(DebugManager.useTestRound )
#endif
		{
			StartCoroutine(showAndSendGameResult(true));
		}
		else
		{
			if(recordMode == RecordMode.replay)
			{
				if(replayManager.isPVPReplayIsSurrenderGame)
				{
					replayManager.checkReplayResult(replayManager.isPVPReplayIsAttackGame == false);
				}
				else
				{
					bool replayResultIsMyWinning = ( (successType == WSDefine.GAME_SUCCESS && replayManager.isPVPReplayIsAttackGame) || 
					                                (  (successType == WSDefine.GAME_FAILED || successType == WSDefine.GAME_GIVEUP) && replayManager.isPVPReplayIsAttackGame == false) 
					                                );

					replayManager.checkReplayResult(replayResultIsMyWinning);
				}
			}
			else
			{
				if(recordMode == RecordMode.record && stageManager.nowPlayingGameType == GameType.Mode.Championship)
				{
					replayManager.saveReplayData(successType);
				}
				
				GameManager.replayManager.tempSave();

				if(RealTime.time - _lastSendCompleteTime < 5.0f) return;

				_lastSendCompleteTime = RealTime.time;

				if(successType == WSDefine.GAME_SUCCESS)
				{
					GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Music,"", AudioFader.State.FadeOut, 2.0f);
					SoundData.play("bgm_win_c");
				}
				else
				{
					GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Music,"", AudioFader.State.FadeOut, 2.0f);
					SoundData.play("bgm_lose_c");
				}

				StartCoroutine(showAndSendGameResult(false));
			}
		}
	}


	IEnumerator showAndSendGameResult(bool isDebug)
	{
		if(player.state == Monster.WIN)
		{
			yield return new WaitForSeconds(2.5f);
		}

		uiManager.uiPlay.hideMenu();


#if UNITY_EDITOR
		if(isDebug)
		{
			//			if(CutSceneManager.nowOpenCutScene == false)
			{
				// 디버그일대는 바로 게임 엔딩으로...
				uiManager.uiPlay.gameResultWord.callback = onCompleteRecieveRoundResult;
				uiManager.uiPlay.gameResultWord.init(stageManager.nowPlayingGameResult, stageManager.nowPlayingGameType);
			}
		}
		else
#endif
		{
			if(DebugManager.useTestRound && EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
			{
				uiManager.uiPlay.gameResultWord.callback = onCompleteRecieveRoundResult;
				uiManager.uiPlay.gameResultWord.init(stageManager.nowPlayingGameResult, stageManager.nowPlayingGameType);

			}
			else
			{
				onFinishRound();
			}

			// 실제 게임일때는 서버에 결과 패킷을 보내는 것으로...
			//uiManager.uiPlay.gameResultWord.callback = onFinishRound;
			//uiManager.uiPlay.gameResultWord.init(stageManager.nowPlayingGameResult, stageManager.nowPlayingGameType);
		}
	}


	public void playReplayResult(bool isWin)
	{
		uiManager.uiPlay.gameResultWord.callback = returnToSelectScene;
		
		if( isWin )
		{
			GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Music,"", AudioFader.State.FadeOut, 2f);
			SoundData.play("bgm_win_c");
			uiManager.uiPlay.gameResultWord.init(Result.Type.Win, stageManager.nowPlayingGameType, 3.5f);
		}
		else
		{
			GameManager.soundManager.fadePlay(SoundManager.SoundPlayType.Music,"", AudioFader.State.FadeOut, 2f);
			SoundData.play("bgm_lose_c");
			uiManager.uiPlay.gameResultWord.init(Result.Type.Lose, stageManager.nowPlayingGameType, 3.5f);
		}

		uiManager.uiPlay.btnReplayClose.gameObject.SetActive (false);
		uiManager.uiPlay.btnReplaySpeed.gameObject.SetActive (false);

		uiManager.uiPlay.hideMenu();
	}



	// 서버에 결과 패킷을 보낸다.
	void onFinishRound()
	{
		EpiServer.instance.sendGetTimeStamp();
	}


	public Xint timeStamp = 0;

	public void onFinishRoundProcess2(int serverTimeStamp)
	{
		timeStamp.Set(serverTimeStamp);

#if UNITY_IOS

		if(Dkaghghk.eCount > 0)
		{
			EpiServer.instance.sendSetCheater();
			return;
		}

		switch(stageManager.nowPlayingGameType)
		{
		case GameType.Mode.Epic:
			EpiServer.instance.sendRoundFinishEpic(successType ); 
			break;
			
		case GameType.Mode.Hell:
			EpiServer.instance.sendFinishHell( HellModeManager.instance.roundIndex, HellModeManager.instance.totalScore, HellModeManager.instance.killUnitCount, HellModeManager.instance.getTotalDistance() );
			break;
			
		case GameType.Mode.Championship:
			string replayData = replayManager.getSavedReplayData();
			EpiServer.instance.sendFinishPVP( GameManager.me.stageManager.nowPlayingGameResult, successType, replayData );
			break;
		case GameType.Mode.Friendly:
			EpiServer.instance.sendFinishFriendPVP( GameManager.me.stageManager.nowPlayingGameResult, successType );
			break;

		case GameType.Mode.Sigong:
			EpiServer.instance.sendFinishSigong( successType );
			break;
			
		}

#else
		resourceManager.unloadUnusedUIImage();
#endif

	}


	// 순서도 : 게임 결과 -> 컷씬 발동 여부 체크 -> 결과 이펙트 화면에 뿌림 -> 서버에 패킷 보냄 -> 컷씬이 있으면 컷씬 발동 or 없으면 보상 팝업 -> 컷신 발동후 보상 팝업.

	// 서버에 승패 결과를 보내고 결과를 받은 순간.
	// 여기에서 결과 컷씬을 발동시켜준다.
	public void onCompleteRecieveRoundResult()
	{
//		Debug.LogError("onCompleteRecieveRoundResult : " + GameManager.me.successType);
		if(CutSceneManager.nowOpenCutScene && CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.After)
		{
			UIPlaySkillSlot.hideAllSlotEffect();

			GameManager.me.characterManager.hideStageAssets();
			GameManager.me.cutSceneManager.currentCutScene.load();
			GameManager.me.cutSceneManager.startCutScene();
		}
		else
		{
			GameManager.setTimeScale = 1.0f;

#if UNITY_EDITOR
			if(DebugManager.instance.useDebug || DebugManager.useTestRound || GameManager.me.successType != WSDefine.GAME_SUCCESS)
#else
			if(DebugManager.useTestRound || GameManager.me.successType != WSDefine.GAME_SUCCESS)
#endif
			{

#if UNITY_EDITOR
				if(GameManager.me.successType != WSDefine.GAME_SUCCESS && uiManager.currentUI == UIManager.Status.UI_PLAY  && DebugManager.instance.useDebug == false)
#else
				if(GameManager.me.successType != WSDefine.GAME_SUCCESS && uiManager.currentUI == UIManager.Status.UI_PLAY  )
#endif

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
				GameManager.me.uiManager.stageClearEffectManager.play();
			}
		}
	}

	public void checkMissionNoticeAndOpenRoundClearPopup()
	{
		GameManager.setTimeScale = 1.0f;
		StartCoroutine(checkMissionNoticeAndOpenRoundClearPopupCT());
	}

	IEnumerator checkMissionNoticeAndOpenRoundClearPopupCT()
	{
		GameDataManager.instance.checkMissionNotice(false);

		while(UIMissionClearNotice.instance.isPlaying)
		{
			yield return null;
		}

		GameManager.me.uiManager.popupRoundClear.show();

	}












	
	public IEnumerator gameOver(float waitTime = 0.0f)
	{
#if UNITY_EDITOR
		Debug.Log("DEAD!!!!!!!" + Time.timeSinceLevelLoad);
#endif
		
		isPlaying = false;
		
		GameManager.setTimeScale = 1.0f;
		
		if(player.hp <= 0)
		{
			_v = player.cTransformPosition;
			_v.z -= 10.0f;

			effectManager.stopQuakeEffect();			
			effectManager.quakeEffect(0.5f, 4.0f, EarthQuakeEffect.Type.Mad, onCompleteDeadQuakeEffect, EarthQuakeEffect.MethodType.ByNormal);				
			
			yield return new WaitForSeconds(0.2f);
		}
		
		yield return new WaitForSeconds(waitTime);
		GameManager.setTimeScale = 1.0f;
		
		onCompleteRound(WSDefine.GAME_FAILED);
	}

	void onCompleteDeadQuakeEffect()
	{
		GameManager.setTimeScale = 1.0f;
	}

	[HideInInspector]
	public bool isStartGame = false;

	
	// 어떤 라운드가 시작될때 최초 시작.
	// startGame => waitAndResrtartGame => (cutscene or game) => ready_battle => startBattleCoroutine 순서다.
	public void startGame(float loadingTime = 1.0f, GamePlayerData testModeData = null, bool isContinueGame = false)
	{
		UIPlayUnitSlot.useTouchTutorial = PlayerPrefs.GetInt("TOUCHTUTORIAL", WSDefine.YES);

		UILoading.nowLoading = true;

#if UNITY_EDITOR
		if(RuneStudioMain.instance != null) RuneStudioMain.instance.hideCardStudio();
#else
		if(RuneStudioMain.instance == null) 
		{
			return;
		}
		
		RuneStudioMain.instance.hideCardStudio();
#endif

		isStartGame = true;

//		Debug.LogError("**** startGame");

		SoundData.play("uibt_battlein");

		Monster.uniqueIndex = 0;

		if(recordMode == RecordMode.replay)
		{
			playMode = PlayMode.replay;

			if(stageManager.isIntro == false)
			{
#if UNITY_EDITOR
//				if(DebugManager.instance.useDebug)
//				{
//					replayManager.loadZip(ReplayManager.REPLAY_NAME,true);
//					GameDataManager.replayAttackerData = GameDataManager.selectedPlayerData;
//					replayManager.replaySeed = BattleSimulator.instance.randomSeed;
//				}
#endif
				changeMainPlayer(GameDataManager.replayAttackerData,GameDataManager.replayAttackerData.id,GameDataManager.replayAttackerData.partsVehicle.parts.resource.ToUpper());
			}
		}
		else
		{
			playMode = PlayMode.normal;

			if(testModeData == null)
			{
				GamePlayerData nowPlayerData = GameDataManager.selectedPlayerData;
				changeMainPlayer(GameDataManager.selectedPlayerData,nowPlayerData.id,nowPlayerData.partsVehicle.parts.resource.ToUpper());
			}
			else
			{
				changeMainPlayer(testModeData, testModeData.id, testModeData.partsVehicle.parts.resource.ToUpper());
			}
		}

		needClearWork = false;
		_openContinuePopup = false;

		cutSceneMapManager.visible = false;
		mapManager = normalMapManager;

		gameCameraContainer.SetActive(true);
		
		tk2dCam.useTargetResolution = false;
		tk2dCam.gameObject.SetActive(false);




		uiManager.uiPlay.init();

		if(recordMode == RecordMode.replay)
		{
			GameManager.me.player.init(GameDataManager.replayAttackerData, true, true, 0);
		}
		else
		{
			if(testModeData == null)
			{
				GameManager.me.player.init(GameDataManager.selectedPlayerData, true, true, 0);
			}
			else
			{
				GameManager.me.player.init(testModeData, true, true, 0);
			}

			if(isContinueGame && replayManager.continueGameType == RoundData.TYPE.HELL && recordMode == RecordMode.continueGame)
			{
				HellModeManager.instance.loadContinueHellMode();
			}
		}

		GameManager.me.player.setParent( GameManager.me.mapManager.waitZone );

		mapManager.isSetStage = false;
		playGameOver = false;


		if(recordMode == RecordMode.record && playMode == PlayMode.normal && isContinueGame == false)
		{
			replayManager.init(UnityEngine.Random.Range(1000,99999));
			replayManager.tempSave(DebugManager.instance.pvpPlayerData, DebugManager.instance.pvpPlayerData2);
		}

		StartCoroutine( waitAndRestartGame(loadingTime) );
	}


	public void returnToSelectScene()
	{
		returnToSelectScene(2.5f);
	}

	public void returnToSelectScene(float waitTime)
	{


		stageManager.isIntro = false;

//		HellModeManager.instance.lastPlayingHellModePlayingTime = 0;

		isStartGame = false;

		TutorialManager.waitStartBattle = false;
//		Debug.LogError("returnToSelectScene : " + waitTime);

		needClearWork = false;

		GameManager.me.uiManager.uiLayoutEffect.hide();
		GameManager.me.cutSceneManager.useCutSceneCamera = false;

		GameDataManager.instance.resetPlayerInfo();

		GameManager.me.characterManager.inGameGUIContinaer.gameObject.SetActive(false);

		GameManager.soundManager.stopLoopChargingEffect();

		GameManager.setTimeScale = 1.0f;
#if UNITY_EDITOR
		MapTesterHelper.instance.showOriginal();
		Log.saveFileLog();
#else
//		if(GameManager.isDebugBuild) Log.saveFileLog();
#endif

		TutorialManager.instance.gameObject.SetActive(true);

		StartCoroutine(returnToSelectSceneCoroutine(waitTime));
	}


	public bool nowCheckingSystemPopup = false;


	bool checkSystemPopup()
	{
		if(GameDataManager.instance.popups.Count > 0)
		{
			nowCheckingSystemPopup = true;

			P_Popup pd = GameDataManager.instance.popups.Dequeue();

			if(needCheckNickName || 
			   TutorialManager.instance.clearCheck ("T1") == false || 
			   GameDataManager.instance.lastPlayStatus != WSDefine.LAST_PLAY_STATUS_NONE)
			{
				if(string.IsNullOrEmpty(pd.actionType) == false && pd.actionType == UIInGameNoticePopup.GAMEUI)
				{
					return checkSystemPopup();
				}
			}

			UISystemPopup.open(UISystemPopup.PopupType.InGameNotice, "", reCheckSystemPopup, reCheckSystemPopup, pd);
			return true;
		}

		nowCheckingSystemPopup = false;
		return false;
	}

	void reCheckSystemPopup()
	{
		checkSystemPopup();
	}



	public bool skipReturnLoadingScene = false;
	IEnumerator returnToSelectSceneCoroutine(float waitTime = 2.0f)
	{
		float returnStartTime = RealTime.time;
		
		isInit = false;
		
		GameManager.me.player.isEnabled = false;
		
		//		if(playMode == PlayMode.record) replayManager.save();
		HellModeManager.instance.isOpen = false;
		
		GameManager.me.mapManager.isSetStage = false;		

		GameManager.me.gameCamera.gameObject.SetActive(false);


		checkSystemPopup();

		while(nowCheckingSystemPopup)
		{
			yield return null;
		}

		GameManager.me.cutSceneManager.close();

		UILoading.nowLoading = true;

		GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.canCheckTutorial48_50 = false;

		gameCamera.gameObject.SetActive(false);

		GameManager.me.uiManager.showLoading();

		foreach(UIPlayUnitSlot us in uiManager.uiPlay.UIUnitSlot)
		{
			if(us != null) us.clear();
		}

		foreach(UIPlaySkillSlot ss in uiManager.uiPlay.uiSkillSlot)
		{
			if(ss != null) ss.clear();
		}

		effectManager.deleteExceptionList.Clear();

		if(skipReturnLoadingScene == false)
		{
			clearStage();

			battleManager.clearStage();

			// 안쓰는 몬스터 리소스들을 삭제한다.
			characterManager.clearUnusedResource(false);
			
			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false)
			{
				uiManager.uiLoading.showLoadingTipMonster();
				uiManager.uiLoading.isCloseLoadingSecene = true;
				
				while(GameManager.me.uiManager.uiLoading.gameObject.activeSelf && GameManager.me.uiManager.uiLoading.ready == false)
				{
					yield return null;
				}
			}
			#else
			uiManager.uiLoading.showLoadingTipMonster();
			uiManager.uiLoading.isCloseLoadingSecene = true;
			
			while(GameManager.me.uiManager.uiLoading.gameObject.activeSelf && GameManager.me.uiManager.uiLoading.ready == false)
			{
				yield return null;
			}
			#endif
			
			
			SoundManager.instance.clearSound();
			
			GameManager.setTimeScale = 1.0f;

#if UNITY_EDITOR
			Debug.Log("waitTime : " + waitTime);
#endif

			yield return null;
			
			yield return Resources.UnloadUnusedAssets();
			
			yield return null;
			
			System.GC.Collect();
			
			yield return null;
			
			float minWaitTime = 4.0f;
			
			if(RealTime.time - returnStartTime < minWaitTime)
			{
				yield return new WaitForSeconds( minWaitTime - (RealTime.time - returnStartTime) );
			}
		}

		skipReturnLoadingScene = false;

		// 로딩 화면을 끝냄.
		uiManager.changeUI(UIManager.Status.UI_MENU);	

		if(GameManager.me.successType.Get() != null)
		{
			if(  (GameManager.me.successType == WSDefine.GAME_FAILED || GameManager.me.successType == WSDefine.GAME_GIVEUP)  && DebugManager.useTestRound == false)
			{
				if(stageManager.nowPlayingGameType == GameType.Mode.Epic)
				{
					GameManager.me.specialPackageManager.canCheckRunePack = true;

					if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None || uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.WorldMap)
					{
						GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.canCheckTutorial48_50 = true;
						
						string stageId = null;
						#if UNITY_EDITOR
						if(DebugManager.instance.useDebug)
						{
							GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.open(1,1,1);
						}
						else
							#endif
						{
							ActData ad;
							
							if(stageManager.isRepeatGame)
							{
								ad = GameManager.info.actData[stageManager.playAct];
								stageId = ad.stages[stageManager.playStage-1];
								uiManager.uiMenu.uiWorldMap.stageDetail.open(stageManager.playAct, stageManager.playStage, stageManager.playRound);
							}
							else
							{
								uiManager.uiMenu.uiWorldMap.stageDetail.open(GameDataManager.instance.maxActWithCheckingMaxAct,GameDataManager.instance.currentStageWithCheckingMaxAct,GameDataManager.instance.maxRound);
							}


							if(TutorialManager.instance.check("T1") == false)
							{
								GameManager.me.specialPackageManager.check();
							}
						}
					}
					else
					{
						if(uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeSelf)
						{
							uiManager.uiMenu.uiWorldMap.stageDetail.hide();
						}
					}


				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Championship)
				{
					if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None || uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.WorldMap)
					{
						#if UNITY_EDITOR
						if(DebugManager.instance.useDebug) {}
						else
							#endif
						{
							uiManager.popupChampionship.show();					
						}
					}
					else if(uiManager.popupChampionship.gameObject.activeSelf)
					{
						uiManager.popupChampionship.hide();
					}
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Hell)
				{
					if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None || uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.WorldMap)
					{
						#if UNITY_EDITOR
						if(DebugManager.instance.useDebug) {}
						else
							#endif
						{
							uiManager.popupHell.showWithoutAni();
						}
					}
					else
					{
						uiManager.popupChampionship.hide();
					}
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Friendly)
				{
					uiManager.uiMenu.uiFriend.refresh();
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Sigong)
				{
					if(uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.None || uiManager.uiMenu.directGoIndex == UIMenu.UIPosition.WorldMap)
					{
						#if UNITY_EDITOR
						if(DebugManager.instance.useDebug) {}
						else
							#endif
						{
							uiManager.popupInstantDungeon.open();
						}
					}
					else
					{
						if(uiManager.popupInstantDungeon.gameObject.activeSelf)
						{
							uiManager.popupInstantDungeon.hide();
						}
					}
				}
			}
			else if(GameManager.me.successType == WSDefine.GAME_SUCCESS && DebugManager.useTestRound == false)
			{
				if(stageManager.nowPlayingGameType == GameType.Mode.Epic)
				{
					string stageId = null;
					#if UNITY_EDITOR
					if(DebugManager.instance.useDebug)
					{
						stageId = DebugManager.instance.debugStageId;
					}
					else
#endif
					{
						ActData ad = GameManager.info.actData[GameDataManager.instance.maxActWithCheckingMaxAct];
						stageId = ad.stages[GameDataManager.instance.currentStageWithCheckingMaxAct-1];
					}

					if(stageManager.isRepeatGame)
					{
						uiManager.uiMenu.uiWorldMap.stageDetail.open(stageManager.playAct, stageManager.playStage, stageManager.playRound);
					}
					else
					{
						if(gameDataManager.needWorldMapAnimation && GameDataManager.instance.maxAct <= GameManager.MAX_ACT)
						{
							gameDataManager.needWorldMapAnimation = false;
							// 들어가서 레벨업 팝업 체크함.
							uiManager.uiMenu.uiWorldMap.startNextStageAnimation();
						}
						else
						{
							uiManager.uiMenu.uiWorldMap.refresh();
							if(stageId != null && GameDataManager.instance.maxAct <= GameManager.MAX_ACT)
							{
								uiManager.uiMenu.uiWorldMap.stageDetail.open(GameDataManager.instance.maxActWithCheckingMaxAct,GameDataManager.instance.currentStageWithCheckingMaxAct, GameDataManager.instance.maxRound );
							}

							else if(GameDataManager.instance.maxAct > GameManager.MAX_ACT)
							{
								if(TutorialManager.instance.isTutorialMode == false)
								{
									GameManager.me.uiManager.popupUpdateWatingNotice.show();
								}
							}
						}
					}
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Championship)
				{
#if UNITY_EDITOR
					if(DebugManager.instance.useDebug)
					{
					}
					else
#endif
					{
					uiManager.popupChampionship.show();					
					}
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Friendly)
				{
					uiManager.uiMenu.uiFriend.refresh();
				}
				else if(stageManager.nowPlayingGameType == GameType.Mode.Sigong)
				{
					uiManager.popupInstantDungeon.open(false,false,true);
				}

//				Debug.LogError("씬으로 돌아간다!!! 성공이다!!!!");
			}

			GameManager.me.successType = "";
		}

		if(GameManager.me.uiManager.popupRoundClear.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupRoundClear.hide();
		}

		recordMode = RecordMode.record;
		playMode = PlayMode.normal;

		DebugManager.useTestRound = false;

		replayManager.deleteTempFile();

		string introKey = "";

		introKey = PandoraManager.instance.localUser.userID;

		if( string.IsNullOrEmpty( introKey ) == false && introKey.Length > 3)
		{
			introKey = introKey.Substring(PandoraManager.instance.localUser.userID.Length - 2);
			PlayerPrefs.SetInt("COMPLETE_INTRO"+introKey,1);
		}

		CutSceneData.clearCutSceneData();

		characterManager.clearUnusedResource(true);

		clearMemory();


		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug && GameManager.me.cutSceneManager.isCutSceneRecordMode)
		{
			GameManager.me.cutSceneManager.autoCutScenePlayer();
		}
		#endif

		UILoading.nowLoading = false;


		StartCoroutine(checkNoticeAndAttendance(false, true));

	}
	
	
	
	public void clearStage(bool flag = true)
	{
#if UNITY_EDITOR
		Debug.LogError("clearStage");
#endif

		needClearWork = false;
		isInit = false;
		isPlaying = false;

		// 게임이 완전 종료됐으면 당연히 컷씬도 종료다...
		cutSceneManager.close();
		mapManager.clearStage();
		bulletManager.clearStage();

		effectManager.clearWordEffect();

		characterManager.clearStage();

		effectManager.clearStage();

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false || currentSimulatorMatchIsFirst)
#endif
		{
			effectManager.destroyAssets();
			resourceManager.destroyAllPrefab();
		}

		MethodManager.clearInGameFunc();	
		player.resetPosition();

		int len = deletePool.childCount;
		
		for(int i = len-1; i >= 0; --i) UnityEngine.Object.Destroy(deletePool.GetChild(i).gameObject);
		
		GameManager.me.stageManager.heroMonster = null;
		
		// 게임이 완전 종료됐으면 당연히 컷씬도 종료다...
		//cutSceneManager.close();
		
		stageManager.playTime = 0.0f;
		characterManager.longestWalkedTargetZonePlayerLine.Set( -9999.0f );

		clearMemory();
	}

	
	public IEnumerator waitAndRestartGame(float waitTime = 0.0f)
	{
		effectManager.destroyAllUnitBodyEffect();


		GameDataManager.instance.clearPlayData();


		needClearWork = false;

//		Debug.LogError("waitAndRestartGame 재시작! : " + Time.timeSinceLevelLoad);

		clearStage();

		yield return null;

		if(UINetworkLock.instance != null) UINetworkLock.instance.hide();

		uiManager.uiLoading.showLoadingTipMonster();



		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
			while(GameManager.me.uiManager.uiLoading.isWaitingForLoading == true)
			{
				yield return null;
			}
		}
		#else
		while(GameManager.me.uiManager.uiLoading.isWaitingForLoading == true)
		{
			yield return null;
		}
		#endif

		yield return Resources.UnloadUnusedAssets();
		System.GC.Collect();

//		Debug.Log("loadRoundMonsterModelData start 0 ");
		// PreLoad Model Data!!!


		loadRoundMonsterModelData();

		while(gameDataManager.isCompleteLoadMap == false){ yield return null; };
		while(gameDataManager.isCompleteLoadModel == false){ yield return null; };
		while(SoundManager.instance.isCompleteLoadSound == false){ yield return null; };
		while(effectManager.isCompleteLoadEffect == false){ yield return null; };

		if(stageManager.nowPlayingGameType == GameType.Mode.Epic)
		{
			SoundManager.instance.loadCutSceneSoundAsset(null);

			while(SoundManager.nowLoadingCutSceneAsset) { yield return null;  };
		}
		else if(stageManager.nowPlayingGameType == GameType.Mode.Sigong && string.IsNullOrEmpty( stageManager.getCutSceneId() ) == false)
		{
			SoundManager.instance.loadCutSceneSoundAsset(null);
			
			while(SoundManager.nowLoadingCutSceneAsset) { yield return null;  };
		}

#if UNITY_EDITOR

		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			SoundManager.instance.loadCutSceneSoundAsset(null);
			while(SoundManager.nowLoadingCutSceneAsset) { yield return null;  };

		}
#endif


		yield return Resources.UnloadUnusedAssets();
		yield return null;
		System.GC.Collect();
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false) characterManager.startPreLoading();
		else CharacterManager.isCompletePreloading = true;
#else
		characterManager.startPreLoading();
#endif
		while(CharacterManager.isCompletePreloading == false){ yield return null; };

//		Debug.Log("loadRoundMonsterModelData COMPLETE");

		player.isEnabled = true;
		player.state = Monster.NORMAL;		

		currentScene = Scene.STATE.PLAY_INTRO;

		// 일단 화면은 인트로.... 검은 화면이 되겠다... 로딩바가 계속 뜨고 있겠지...
		// 이때 컷씬이 있으면 초기화를 해준다.
		GameManager.me.cutSceneManager.initCutScene();

		// 기본적으로 스테이지에서 쓸 배경은 만드는데...
		normalMapManager.createBackground( stageManager.getMapId(UIPlay.playerLeagueGrade), true);

		if(stageManager.nowRound.mode == RoundData.MODE.PVP)
		{
			GameManager.me.mapManager.pvpGradeSlotManager.init(UIPlay.playerLeagueGrade, UIPlay.pvpleagueGrade);

			while(GameManager.me.mapManager.pvpGradeSlotManager.isCompleteLoadingGradeSymbol == false)
			{
				yield return null;
			}
		}

		while(uiManager.uiLoading.gameObject.activeSelf && uiManager.uiLoading.checkCloseTime() == false)
		{
			yield return null;
		}

		// intro에서 컷씬 발동하는지 체크.
		GameManager.me.cutSceneManager.roundStateCheck();

		mapManager.visible = true;


#if UNITY_EDITOR

		if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useEffectSkillCamEditor )
		{
			player.setVisible(false);
		}
#endif


		// 컷씬이 아닐때는 몬스터를 세팅하고 게임을 그냥 시작한다.
		if(CutSceneManager.nowOpenCutScene == false)
		{
#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false) yield return new WaitForSeconds(waitTime);
#else
			yield return new WaitForSeconds(waitTime);
#endif
			// ready는 사실상 실제 게임 시작을 알리는 것과 동일하다.
			// ready에서 startBattle()을 호출하기 때문!
			currentScene = Scene.STATE.PLAY_READY;
		}
	}
	
	
	private float _showResultDelay = 0.0f;
	private Xint _currentScene = Scene.STATE.SPLASH;
	
	public Xint currentScene
	{
		set
		{
			if(value == _currentScene) return;
			_currentScene = value;
			
			switch(_currentScene)
			{
				
			case Scene.STATE.PLAY_INTRO:
				uiManager.uiPlay.resetCamera();
				isPaused = false;
				break;
			case Scene.STATE.PLAY_READY:
//				Debug.LogError("==== Scene.PLAY_READY");
				isPlaying = false;
				player.state = Monster.NORMAL;
				startBattle();
				break;				
			case Scene.STATE.PLAY_BATTLE:
//				Debug.LogError("==== Scene.PLAY_BATTLE");
				break;

			case Scene.STATE.PLAY_CLEAR_SUCCESS: // clearboss!
			case Scene.STATE.PLAY_CLEAR_FAILED:

#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation == false)
#endif
				{
					GameManager.setTimeScale = 1.0f;
				}


				if(uiManager.uiLoading.gameObject.activeSelf)
				{
					uiManager.uiLoading.hide();
				}

				if(_currentScene == Scene.STATE.PLAY_CLEAR_SUCCESS)
				{
					_showResultDelay = 2.0f;				


#if UNITY_EDITOR
					Debug.LogError("=== PLAY_CLEAR_SUCCESS : " + GameManager.me.stageManager.playTime);
					Log.log("PLAY_CLEAR_SUCCESS : " + GameManager.me.stageManager.playTime);
					GameManager.me.player.state = Monster.NORMAL;
#else
//					if(GameManager.isDebugBuild)
//					{
//						Debug.LogError("=== PLAY_CLEAR_SUCCESS : " + GameManager.me.stageManager.playTime);
//						Log.log("PLAY_CLEAR_SUCCESS : " + GameManager.me.stageManager.playTime);
//						GameManager.me.player.state = Monster.NORMAL;
//					}
#endif
				}
				else
				{
#if UNITY_EDITOR
					Debug.LogError("=== PLAY_CLEAR_FAILED : " + GameManager.me.stageManager.playTime);
					Log.log("PLAY_CLEAR_FAILED : " + GameManager.me.stageManager.playTime);
#else
//					if(GameManager.isDebugBuild)
//					{
//						Debug.LogError("=== PLAY_CLEAR_FAILED : " + GameManager.me.stageManager.playTime);
//						Log.log("PLAY_CLEAR_FAILED : " + GameManager.me.stageManager.playTime);
//					}

					#endif
				}

				GameManager.soundManager.stopLoopChargingEffect();

				isPlaying = false;
				needClearWork = true;

				// 경우에 따라 다르겠지만... 일단 클리어 후 캐릭터들은 일반 동작 상태로 바꾸어 준다.

				GameManager.me.player.clearEffect();
				
				for(int j = GameManager.me.characterManager.playerMonster.Count -1 ; j >= 0; --j)
				{
					if(GameManager.me.characterManager.playerMonster[j].isEnabled == false) continue;
					GameManager.me.characterManager.playerMonster[j].setIdleAndFreeze();
				}
				
				for(int j = GameManager.me.characterManager.monsters.Count -1 ; j >= 0; --j)
				{
					if(GameManager.me.characterManager.monsters[j].isEnabled == false || GameManager.me.characterManager.monsters[j].isHero) continue;
					GameManager.me.characterManager.monsters[j].setIdleAndFreeze();
				}				
				
				if(GameManager.me.stageManager.nowRound.mode != RoundData.MODE.PVP)
				{
					uiManager.uiPlay.hideMenu(0.1f);
				}

				break;

			case Scene.STATE.PLAY_CLEAR_DRAW:
				#if UNITY_EDITOR
				Debug.LogError("=== PLAY_CLEAR_DRAW : " + GameManager.me.stageManager.playTime);
				Log.log("PLAY_CLEAR_DRAW : " + GameManager.me.stageManager.playTime);
				#endif
				
				GameManager.soundManager.stopLoopChargingEffect();
				isPlaying = false;
				needClearWork = true;
				
				for(int j = GameManager.me.characterManager.playerMonster.Count -1 ; j >= 0; --j)
				{
					if(GameManager.me.characterManager.playerMonster[j].isEnabled == false) continue;
					GameManager.me.characterManager.playerMonster[j].setIdleAndFreeze(true);
				}
				
				for(int j = GameManager.me.characterManager.monsters.Count -1 ; j >= 0; --j)
				{
					if(GameManager.me.characterManager.monsters[j].isEnabled == false) continue;
					GameManager.me.characterManager.monsters[j].setIdleAndFreeze(true);
				}	
				
				break;
			}
		}
		get
		{
			return _currentScene;
		}
	}









	

	// 게임 배틀 애니메이션과 함께 실제 게임이 시작된다.
	public void startBattle(bool useFixedRandomSeed = false, int seed = -1)
	{
		Debug.LogError("**** startBattle");
		GameManager.me.uiManager.uiPlay.hideMenu(0.0f);
		StartCoroutine(startBattleCoroutine(useFixedRandomSeed, seed));
	}



	
	IEnumerator startBattleCoroutine(bool useFixedRandomSeed, int seed)
	{
//		Debug.LogError("**** startBattleCoroutine");

		if(stageManager.nowRound != null && stageManager.nowRound.id != "INTRO")
		{
			while(CutSceneManager.nowOpenCutScene)
			{
				yield return new WaitForSeconds(0.1f);
			}

			if(stageManager.nowRound.mode == RoundData.MODE.PVP && GameManager.me.mapManager.pvpGradeSlotManager.gameObject.activeSelf == false)
			{
				GameManager.me.mapManager.pvpGradeSlotManager.gameObject.SetActive(true);
			}
		}

		GameManager.me.uiManager.uiPlay.goEpicPreCamInfo.SetActive(false);

		_updateLoopLeftTime = 0.0f;
		GameManager.setTimeScale = 1.0f;

		if(Monster.uniqueIndex < 1000)
		{
			Monster.uniqueIndex = 1000;
		}

		_fingerId = -1000;
		_isMouseDown = false;

		// 화면에 있을 몬스터 캐릭터 세팅.
		mapManager.setStage(stageManager.nowRound);

		mapManager.createBackground( stageManager.getMapId( UIPlay.playerLeagueGrade) , true);

		yield return Resources.UnloadUnusedAssets();
		System.GC.Collect();

		GameManager.me.player.stat.uniqueId = 0;
		GameManager.me.player.state = Monster.NORMAL;
		GameManager.me.player.render();
		GameManager.me.player.container.gameObject.SetActive(true);
		GameManager.me.player.pet.container.gameObject.SetActive(true);
		GameManager.me.player.setParent( GameManager.me.mapManager.mapStage );

		battleManager.initPlayerUniqueId();


		if(GameManager.me.pvpPlayer != null)
		{
			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false) yield return null;
			#else
			yield return null;
			#endif
			GameManager.me.pvpPlayer.state = Monster.NORMAL;
			GameManager.me.pvpPlayer.render();
			yield return null;
			GameManager.me.pvpPlayer.container.gameObject.SetActive(true);
			GameManager.me.pvpPlayer.pet.container.gameObject.SetActive(true);
			GameManager.me.pvpPlayer.setParent( GameManager.me.mapManager.mapStage );
		}


		if(GameManager.me.cutSceneManager.useCutSceneCamera == false)
		{
			uiManager.uiPlay.resetCamera();
			uiManager.uiPlay.resetToChallengeModeZoomDefaultCamera();
		}

		float waitTime = 0.1f;
		while(CutSceneManager.nowOpenCutScene && stageManager.nowRound.id != "INTRO")
		{
			yield return new WaitForSeconds(0.1f);
			waitTime += 0.1f;
			if(waitTime > 2.0f) break;
		}

		// 여기까지는 로딩화면 or 컷씬 종료 화면이 되겠다...

		// ui는 일단 플레이 화면으로 간다....
		uiManager.changeUI(UIManager.Status.UI_PLAY);




		// 일반적으로 bgm을 틀지만 이계던전에서는 다음 라운드가 시작할때 새로 트는게 아니라 기존것을 쭉 이어서 튼다.
		bool canPlayBgm = true;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation)
		{
			uiManager.uiPlay.showMenu(0.0f);
			//yield return null;	
			uiManager.uiPlay.hideReadyBattleAnimation();	
		}
		else
#endif
		{
			if(recordMode != RecordMode.continueGame)
			{
				uiManager.uiPlay.hideMenu(0.0f);

				if((stageManager.nowPlayingGameType != GameType.Mode.Challenge) && CutSceneManager.nowOpenCutScene == false && stageManager.nowRound.id != "INTRO")
				{
#if UNITY_EDITOR
					if(DebugManager.instance.skipBattlePreviewCam || UnitSkillCamMaker.instance.useUnitSkillCamMaker)
					{
						GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_IN, 0, true);
					}
					else
#endif
					{
						GameManager.me.uiManager.uiPlay.startPreCam();
						while(UIPlay.isPlayingPreCam)
						{
							yield return null;
						}
						GameManager.setTimeScale = 1.0f;

						if(HellModeManager.instance.isOpen && HellModeManager.instance.roundIndex > 1)
						{
							// 이계던전에서는 이전라운드에서 틀었던 음악을 계손 튼다.
						}
						else
						{
							SoundManager.instance.fadePlay(SoundManager.SoundPlayType.Music, "", AudioFader.State.FadeOut, 2.0f);
						}

						yield return new WaitForSeconds(0.3f);
					}
				}

				if(stageManager.nowRound.id != "INTRO")
				{

					GameManager.me.cutSceneManager.useCutSceneCamera = false;

					if(stageManager.nowRound.id == "PVP")
					{
						GameManager.me.uiManager.uiPlay.setCameraClipPlane(1700,5000);
					}

				}

				GameManager.me.uiManager.uiPlay.goEpicPreCamInfo.SetActive(false);
				uiManager.uiPlay.cameraTarget = GameManager.me.player.cTransform;
				uiManager.uiPlay.resetCamera();
				uiManager.uiPlay.resetToChallengeModeZoomDefaultCamera();
				yield return null;

//				GameManager.me.uiManager.uiPlay.btnReplayClose.isEnabled = true;

#if UNITY_EDITOR
				if(UnitSkillCamMaker.instance.useUnitSkillCamMaker == false)
#endif

				{

					if(HellModeManager.instance.isOpen == false || HellModeManager.instance.roundIndex <= 1)
					{
						uiManager.uiPlay.showReadyBattleAnimation();

						if(stageManager.nowPlayingGameType == GameType.Mode.Championship)
						{
//							SoundData.play("f_battle");
						}
						else if(stageManager.nowPlayingGameType == GameType.Mode.Hell)
						{
//							SoundData.play("m_battle");
						}

						yield return new WaitForSeconds(3.0f);		
					}
					else if(HellModeManager.instance.isOpen && HellModeManager.instance.roundIndex > 1)
					{
						yield return new WaitForSeconds(0.5f);

						uiManager.uiLayoutEffect.sideFade.gameObject.SetActive(false);

						uiManager.uiPlay.showHellRoundStartAnimation();

//						SoundData.play("m_go");
						if(HellModeManager.instance.roundIndex <= 20 && HellModeManager.instance.roundIndex % 5 == 0)
						{
							canPlayBgm = true;
						}
						else
						{
							canPlayBgm = false;
						}

						yield return new WaitForSeconds(1.5f);
					}
				}
			}

			if(stageManager.nowRound.id != "INTRO")
			{
				GameManager.me.cutSceneManager.useCutSceneCamera = false;
				if(stageManager.nowRound.id == "PVP")
				{
					GameManager.me.uiManager.uiPlay.setCameraClipPlane(1700,5000);
				}
			}

			uiManager.uiPlay.showMenu();
			uiManager.uiPlay.hideReadyBattleAnimation();
		}


		GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;


		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation)
		{
		}
		else
		#endif
		{
			if(canPlayBgm) SoundData.playBattleBGM();
		}

		// 시드값을 고정시키면 게임도 고정된다고 생각하자!
		//inGameRandom.init(4989);// = new Well512Random(4989);
		
		int randomeSeed = 4989;
		
		#if UNITY_EDITOR
		if(BattleSimulator.instance.wantSameResult)
		{
			randomeSeed = BattleSimulator.instance.randomSeed;
		}
		else
		{
			if(useFixedRandomSeed)
			{
				randomeSeed = seed;
			}
			else
			{
				randomeSeed = UnityEngine.Random.Range(1000,99999);
			}
		}

		BattleSimulator.instance.nowSeed = randomeSeed;

		#else

		if(useFixedRandomSeed)
		{
			randomeSeed = seed;
		}
		else
		{
			randomeSeed = UnityEngine.Random.Range(1000,99999);
		}
		#endif

		if(recordMode == RecordMode.replay || GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PVP)
		{
			if(recordMode == RecordMode.replay)
			{
				inGameRandom = new Well512Random((uint)replayManager.replaySeed);
				replayManager.init(replayManager.replaySeed);
			}
			else
			{
				inGameRandom = new Well512Random((uint)replayManager.pvpSeed);
				replayManager.init(replayManager.pvpSeed);
			}

			if(recordMode == RecordMode.replay)
			{
				replayManager.loadReplayGameData();
			}
			else if(recordMode == RecordMode.continueGame)
			{
				inGameRandom = new Well512Random((uint)replayManager.continueSeed);
				replayManager.init(replayManager.continueSeed);
				replayManager.loadContinueGameData();
			}
			
		}
		else if(recordMode == RecordMode.continueGame)
		{
			inGameRandom = new Well512Random((uint)replayManager.continueSeed);
			replayManager.init(replayManager.continueSeed);
			replayManager.loadContinueGameData();
		}
		else
		{
#if UNITY_EDITOR
			Debug.LogError("random seed : " + randomeSeed);
#endif

			inGameRandom = new Well512Random((uint)randomeSeed);
			replayManager.init(randomeSeed);
		}


		GameManager.me.characterManager.updatePlayerFirst = (inGameRandom.Range(0,101) > 50);


		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
			Log.log("updatePlayerFirst : " + GameManager.me.characterManager.updatePlayerFirst); //ff
		}
#else
//		if(GameManager.isDebugBuild)
//		{
//			Log.log("updatePlayerFirst : " + GameManager.me.characterManager.updatePlayerFirst); //ff
//		}
		#endif



		characterManager.inGameGUITooltipContainer.gameObject.SetActive(false);

		stageManager.playTime = 0.0f;		

		UnitSlot.summonPosIndex = inGameRandom.Range(0,12);

		UIPlayUnitSlot.summonPosIndex = inGameRandom.Range(0,12);

		characterManager.longestWalkedTargetZonePlayerLine.Set( -9999.0f );

		if(stageManager.nowRound.id != "INTRO") TutorialManager.instance.roundCheck(stageManager.nowRound.mode);

		GameManager.me.uiManager.uiPlay.initTimeUI();

		while(TutorialManager.waitStartBattle)
		{
			yield return null;
		}

		TutorialManager.instance.gameObject.SetActive(false);

		if(CutSceneManager.introCutSceneStartTime > 0)
		{
			CutSceneManager.cutScenePlayTime =  Mathf.Round( CutSceneManager.introCutSceneStartTime * 10 ) * 0.1f;
			_v = GameManager.me.player.cTransformPosition;
			_v.x = GameManager.me.stageManager.nowRound.playerStartPosX;
			_v.z = 0.0f;
			GameManager.me.player.setPositionCtransform( _v );	
			CutSceneManager.introCutSceneStartTime = -1;
		}

		// 스타트에서 라운드 발동 체크. 
		// 컨디션 중 1번 스타트면 여기에서 체크한다.

		if(recordMode == RecordMode.record && (stageManager.nowPlayingGameType == GameType.Mode.Epic || stageManager.nowPlayingGameType == GameType.Mode.Sigong))
		{
			GameManager.me.cutSceneManager.roundStateCheck();
		}


//		currentTime = 0.0f;
		_updateLoopLeftTime = 0.0f; 

		isPaused = false;
		isInit = true;
		isPlaying = true;

		replayManager.resetMemoryStreamPosition();


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug && cutSceneManager.isCutSceneRecordMode)
		{
			yield return new WaitForSeconds(2.0f);
			mapManager.clearRound();
		}
#endif


		if(canUseAutoPlay && recordMode == RecordMode.record)
		{
			uiManager.uiPlay.setFastPlay(GameManager.me.isFastPlay && isAutoPlay);
		}


		UILoading.nowLoading = false;
	}








	void OnApplicationPause(bool flag)
	{
		if(flag && 
		   GameManager.me != null &&  
		   recordMode == RecordMode.record && 
		   GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY && 
		   isPlaying )
		   //&& HellModeManager.instance.gameObject.activeInHierarchy == false)
		{
			GameManager.me.uiManager.uiPlay.onOpenPause();
		}
	}





//
	public void setGuiLog(string str)
	{
#if UNITY_IPHONE
		Debug.LogError(str);

		guiLog = str + "\n" + guiLog;
		if(guiLog.Length >= 2000) guiLog = guiLog.Substring(0,2000);
#endif
	}
//

	void OnGUI()
	{
#if UNITY_IPHONE
		GUI.color = Color.red;
		GUI.skin.box.fontSize = 40;
		GUI.skin.label.fontSize = 40;
		GUI.Label(new Rect(30,30,900,600), guiLog); 	
#endif
	}




}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class RuneStudioMain : MonoBehaviour 
{
	public const string onClickEvent = "OnClick";

	public RuneStudioInit resetter;

	public Transform moveablePanelParent;

	public Transform reinforceMoveablePanelParent;
	public Transform composeMovablePanelParent;

	public Transform[] evolveMovablePanelParent;
	public Transform transcendMovablePanelParent;

	public GameObject rootUnitMake;
	public GameObject rootPreUnitMake;

	public GameObject[] rootEvolve;
	public GameObject rootReinforce;
	public GameObject rootCompose;
	public GameObject rootTranscend;

	public UICardFrame composeCardFrame1, composeCardFrame2;

	public Camera cam256;
	public Camera cam512;


	public UISprite spSkillIcon;
	public UISprite spSkillIconFrame;
	public GameObject[] skillIconRareBg = new GameObject[6];


	public Material[] smallCardBgMaterial = new Material[6];


	public GameObject[] cardStudio256;
	public GameObject[] cardStudio512;


	public Transform tfUnit256Container;
	public Transform tfUnit512Container;

	[HideInInspector]
	public GameObject goMakeCompleteGuidePanel;

	public MegaMorphAnim megaMorpeAnim;

	public Material gachaBgMaterial;

	public enum Type
	{
		UnitMake, Reinforce, None, Evolve, Compose, Transcend
	}

	public Type type = Type.None;

	public PlayMakerFSM[] makeController;
	public PlayMakerFSM[] reinforceController;
	public PlayMakerFSM[] evolveController;
	public PlayMakerFSM[] composeController;
	public PlayMakerFSM[] transcendController;

//	public PlayMakerFSM[] challengeResultSlotFSM = new PlayMakerFSM[9];

	public GameObject card10Container;

	public StudioCardBgSlot[] card10BgSlot = new StudioCardBgSlot[10];
	public StudioCardCoverSlot[] card10Cover = new StudioCardCoverSlot[10];

	public GameObject card2Container;
	public StudioCardBgSlot[] card2BgSlot = new StudioCardBgSlot[2];

	public UICardFrame[] evolutionCardFrame;
	public Animation[] evolutionRotationAni;


	public GameObject transcendStartDoor;



	public GameObject blackBackground;


	public static RuneStudioMain instance;


	public enum Step
	{
		Start, Progress, Finish
	}


	public Step step = Step.Finish;


	void OnDestroy()
	{
		if(instance !=null)
		{
			if(instance.gameObject != null)
			{
				GameObject.Destroy(instance.gameObject);
			}
		}
		//instance
	}


	void Awake()
	{
		if(instance != null)
		{
			GameObject.DestroyImmediate(instance.gameObject);
		}

		instance = this;

		rootPreUnitMake.SetActive(false);
		rootUnitMake.gameObject.SetActive(false);
//		rootChallengeResult.gameObject.SetActive(false);
		rootReinforce.gameObject.SetActive(false);

		cam256.gameObject.SetActive(false);
		cam512.gameObject.SetActive(false);

		resetter.setupCam.useTargetResolution = true;
		resetter.setupCam.UpdateCameraMatrix();
		resetter.setupCam.gameObject.SetActive(false);

		gachaBgMaterial.SetColor("_Color", Color.black);

		isPlayingMaking = false;
	}


	void Start()
	{
		StartCoroutine(completeInitRuneMakeStudioCT());
	}

	IEnumerator completeInitRuneMakeStudioCT()
	{
		yield return new WaitForSeconds(0.8f);

		GameManager.me.onCompleteInitRunemakeStudio();
		isPlayingMaking = false;
	}


	public void showCard512Studio(int rare)
	{
		if(rare > RareType.SS && rare < 50)
		{
			rare = RareType.SS;
		}

		for(int i = 0; i < 6; ++i)
		{
			cardStudio512[i].SetActive( (i == rare) );
		}
	}


	private PlayMakerFSM _lastFsm;

	public void sendEvent(PlayMakerFSM fsm, bool blockMenuCam = true)
	{
		if(blockMenuCam)
		{
			GameManager.me.uiManager.menuCamera2.enabled = false;
			GameManager.me.uiManager.uiMenuCamera.enabled = false;
		}
		//uirf_start : 연출 시작시 (합성/뽑기에도 동일하게 적용) <WMV>
		SoundData.play("uirf_start");

		_lastFsm = fsm;	
		fsm.gameObject.SetActive(true);
		fsm.enabled = true;
		fsm.SendEvent(onClickEvent);

		state = State.Playing;
	}


	public enum State
	{
		Idle, Playing, Waiting
	}

	public State state = State.Idle;
	Rectangle _touchRect = new Rectangle(1010,560,173,70);

	Vector2 _touchPoint;

	void Update()
	{
		if(Input.GetMouseButtonUp(0))
		{
			if(step == Step.Start && GameManager.me.uiManager.goBtnRuneStudioSkip.activeSelf )
			{
				if(TutorialManager.instance.isTutorialMode == false)
				{
					_touchPoint = Util.screenPositionWithCamViewRect(Input.mousePosition);
					if(_touchRect.contains(_touchPoint))
					{
						GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);
						GameManager.setTimeScale = 1.0f;	

						switch(type)
						{
						case Type.UnitMake:
							nextMakeResult(true);
							break;
						case Type.Reinforce:
							showReinforceResultCard(true);
							break;
						case Type.Compose:
							showComposeResultCard(true);
							break;
						case Type.Evolve:
							showEvolveResultCard(true);
							break;
						case Type.Transcend:
							showTranscendResultCard(true);
							break;
						}
						return;
					}
				}
			}
		}

		if(state == State.Playing)
		{
			if(Input.GetMouseButton(0))
			{
				if(Time.timeScale < 4.0f)
				{
					GameManager.setTimeScale = 4.0f;
				}
			}
			else if(Input.GetMouseButtonUp(0))
			{
				if(Time.timeScale > 1.0f)
				{
					GameManager.setTimeScale = 1.0f;
				}
			}
		}
		else if(state == State.Waiting)
		{
			if(Input.GetMouseButtonDown(0) && _isMouseDown == false)
			{
				_isMouseDown = true;
			}
			else if(Input.GetMouseButtonUp(0) && _isMouseDown)
			{
				_isMouseDown = false;
				state = State.Playing;

				if(step != Step.Finish)
				{
					nextMakeResult();
				}
			}
		}
	}


	public void resetPlaySpeed()
	{
		GameManager.setTimeScale = 1.0f;
		state = State.Idle;
	}


	public void hideCardStudio()
	{
		for(int i = 0; i < cardStudio512.Length; ++i)
		{
			cardStudio512[i].SetActive( false );
		}
		
		for(int i = 0; i < cardStudio256.Length; ++i)
		{
			cardStudio256[i].SetActive( false );
		}
	}



	public void endProcess(bool isSkipMode = false)
	{
		Debug.LogError("========= endProcess ======== now Delay Mode ? : " + _isDelayEndProcessMode);

		if(_isDelayEndProcessMode)
		{
			return;
		}

		GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);

		goMakeCompleteGuidePanel.gameObject.SetActive(false);

		GameManager.me.uiManager.menuCamera2.enabled = true;
		GameManager.me.uiManager.uiMenuCamera.enabled = true;

		GameManager.me.uiManager.popupShop.goShopBlocker.gameObject.SetActive(false);

		evolveSourceBgSlot.hide();
		evolveSourceBgSlot.gameObject.SetActive(false);

		for(int i = 0; i < 10; ++i)
		{
			card10BgSlot[i].hide();
		}

		for(int i = 0; i < 2; ++i)
		{
			card2BgSlot[i].hide();
		}

		card10Container.gameObject.SetActive(false);
		card2Container.gameObject.SetActive(false);

		reinforceStarter.enabled = false;
		makeStarter.enabled = false;
		transcendStarter.enabled = false;

		for(int i = 0; i < evolveController.Length ; ++i)
		{
			evolveController[i].enabled = false;
		}

		//evolveStarter.enabled = false;

		blackBackground.gameObject.SetActive(false);

		reinforceSlotContainer.SetActive(false);

		resetPlaySpeed();

		cam256.gameObject.SetActive(false);
		cam512.gameObject.SetActive(false);

		hideCardStudio();

		spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);

		switch(type)
		{
		case Type.None:
			break;
		case Type.Evolve:
			for(int i = 0; i < rootEvolve.Length; ++i)
			{
				rootEvolve[i].SetActive(false);
			}
			break;
		case Type.UnitMake:
			rootUnitMake.SetActive(false);
			rootPreUnitMake.SetActive(false);
			break;
		case Type.Reinforce:
			rootReinforce.SetActive(false);

			if(isSkipMode == false)
			{
				if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) GameManager.me.uiManager.popupSummonDetail.hide();
				if(GameManager.me.uiManager.popupSkillPreview.isEnabled) GameManager.me.uiManager.popupSkillPreview.isEnabled = false;
				if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();
			}
			break;
		}

		if(_lastFsm != null)
		{
#if UNITY_EDITOR
			Debug.Log(_lastFsm);
#endif
			_lastFsm.enabled = false;
			_lastFsm.gameObject.SetActive(false);
		}

		megaMorpeAnim.gameObject.SetActive(false);

		gachaBgMaterial.SetColor("_Color", Color.black);

		resetter.reset(type);

		GameManager.setTimeScale = 1.0f;

		// 소환수 완료.
		if(TutorialManager.nowPlayingTutorial("T44",10))
		{
			//EpiServer.instance.sendCompleteTutorial("T44");
			TutorialManager.instance.subStep = 11;
			TutorialManager.uiTutorial.hide();
			TutorialManager.instance.openDialog(200,400,true,true);
		}
		// 소환수 완료.
		else if(TutorialManager.nowPlayingTutorial("T44",14))
		{
			//14번도 생략.
			TutorialManager.instance.subStep = 15;
			TutorialManager.uiTutorial.hide();
			TutorialManager.uiTutorial.setArrowAndDim(19,538,false);
		}
		else if(TutorialManager.nowPlayingTutorial("T48") || TutorialManager.nowPlayingTutorial("T49") || TutorialManager.nowPlayingTutorial("T50"))
		{
			TutorialManager.instance.subStep = 8;
			TutorialManager.instance.openDialog(200,400,true,true,true);
			TutorialManager.uiTutorial.showBigSizeCharacter();
		}

		if(string.IsNullOrEmpty(GameDataManager.instance.stageClearRewardItem) == false )
		{
//			Debug.LogError("GameDataManager.instance.stageClearRewardItem : " + GameDataManager.instance.stageClearRewardItem);
			GameDataManager.instance.stageClearRewardItem = null;
			GameManager.me.uiManager.uiMenu.uiWorldMap.startNextStageAnimationStep2();
		}

		isPlayingMaking = false;
	}

	List<GameIDData> _leftDisplayItems = new List<GameIDData>();

	public GameObject reinforceSlotContainer;
	public UIChallengeItemSlot[] reinforceSlots = new UIChallengeItemSlot[5];
	public Renderer[] reinforceRenderingSlot = new Renderer[5];

	public Renderer evolutionRuneRenderer;

	public string reinforceResultId;
	public GameIDData.Type reinforceType;

	private GameIDData _reinforceOriginalData = null;
	public GameIDData reinforceOriginalData
	{
		set
		{
			if(value == null) _reinforceOriginalData = null;
			else
			{
				if(_reinforceOriginalData == null) _reinforceOriginalData = new GameIDData();
				_reinforceOriginalData.parse(value.serverId, value.type);
			}
		}
		get
		{
			return _reinforceOriginalData;
		}
	}

	public GameIDData reinforceResultData = new GameIDData();

	public void playReinforceResult(string newId, string[] sourceIds, GameIDData.Type rType)
	{
		reinforceMoveablePanelParent.transform.localScale = new Vector3(1,1,1);

		reinforceResultId = newId;
		type = Type.Reinforce;
		reinforceType = rType;

		GameManager.me.uiManager.uiMenu.uiHero.selectedSlot = null;
		GameManager.me.uiManager.uiMenu.uiSkill.selectSlot = null;
		GameManager.me.uiManager.uiMenu.uiSummon.selectSlot = null;

		reinforceSlotContainer.SetActive(true);

		for(int i = 0; i < 5; ++i)
		{
			Renderer ren;

			if(i < sourceIds.Length)
			{
				GameIDData temp = new GameIDData();
				temp.parse(sourceIds[i], reinforceType);

				switch(temp.type)
				{
				case GameIDData.Type.Equip:
					reinforceSlots[i].setData(UIChallengeItemSlot.Type.Equip,temp);
					break;
				case GameIDData.Type.Unit:
					reinforceSlots[i].setData(UIChallengeItemSlot.Type.Unit,temp);
					break;
				case GameIDData.Type.Skill:
					reinforceSlots[i].setData(UIChallengeItemSlot.Type.Skill,temp);
					break;
				}

				reinforceRenderingSlot[i].enabled = true;
			}
			else
			{
				ren = reinforceRenderingSlot[i].GetComponent<Renderer>();
				reinforceRenderingSlot[i].enabled = false;
			}
		}

		reinforceResultData.parse(newId, reinforceType);


		// 강화되기 전 데이터를 먼저 보여준다.
		switch(reinforceResultData.type)
		{
		case GameIDData.Type.Equip:

			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( reinforceOriginalData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false);
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
			break;
		case GameIDData.Type.Skill:

			GameManager.me.uiManager.popupSkillPreview.show( reinforceOriginalData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false, true);

			break;

		case GameIDData.Type.Unit:

			GameManager.me.uiManager.popupSummonDetail.show( reinforceOriginalData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false);
			spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
			break;
		}

		cam256.gameObject.SetActive(true);

		sendEvent(reinforceStarter);

		if(TutorialManager.instance.isTutorialMode == false)
		{
			GameManager.me.uiManager.activeRuneStudioSkipButton();
			step = Step.Start;
		}
	}




	public void showReinforceResultCard(bool isSkipMode = false)
	{

		GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);

		// uirf_result : 강화 완료시 <WMV>
		SoundData.play("uirf_result");

		if(isSkipMode)
		{
			refreshInventory(reinforceResultData.type);
			endProcess();

			switch(reinforceResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( reinforceResultData, RuneInfoPopup.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, null, reinforceOriginalData);
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.spSkipModeBackground.gameObject.SetActive(true);
//				if(UIReinforceBarPanel.isTabSlot) GameManager.me.uiManager.uiMenu.uiHero.setCharacterData();
				break;
			case GameIDData.Type.Skill:
				GameManager.me.uiManager.popupSkillPreview.show( reinforceResultData, RuneInfoPopup.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, null, reinforceOriginalData);
				GameManager.me.uiManager.popupSkillPreview.spSkipModeBackground.gameObject.SetActive(true);
				break;
			case GameIDData.Type.Unit:
				GameManager.me.uiManager.popupSummonDetail.show( reinforceResultData, RuneInfoPopup.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, reinforceOriginalData);
				GameManager.me.uiManager.popupSummonDetail.spSkipModeBackground.gameObject.SetActive(true);
				break;
			}
		}
		else
		{
			switch(reinforceResultData.type)
			{
			case GameIDData.Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show( reinforceResultData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false, false, null, reinforceOriginalData);
//				if(UIReinforceBarPanel.isTabSlot) GameManager.me.uiManager.uiMenu.uiHero.setCharacterData();
				break;
			case GameIDData.Type.Skill:
				GameManager.me.uiManager.popupSkillPreview.show( reinforceResultData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false, false, null, reinforceOriginalData);
				break;
			case GameIDData.Type.Unit:
				GameManager.me.uiManager.popupSummonDetail.show( reinforceResultData, RuneInfoPopup.Type.Reinforce, UIReinforceBarPanel.isTabSlot == false, false, reinforceOriginalData);
				break;
			}
			
			reinforceMoveablePanelParent.transform.localScale = new Vector3(1,0,1);
			
			iTween.ScaleTo(reinforceMoveablePanelParent.gameObject, iTween.Hash(
				"scale", new Vector3(1,1,1),
				"delay", 0.0f,
				"time",0.3f,
				"easetype", iTween.EaseType.easeInSine,
				"looptype", iTween.LoopType.none,
				"oncomplete", "iTweenOnComplete",
				"islocal",true,
				"onCompleteTarget",gameObject
				)); 
		}
	}












	public void iTweenOnComplete()
	{
//		Debug.LogError("iTweenOnComplete!");

		switch(reinforceResultData.type)
		{
		case GameIDData.Type.Equip:

			foreach(UIPanelRefresher p in GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.panelRefresher)
			{
				p.draw();
			}

			break;
		case GameIDData.Type.Skill:
			
			foreach(UIPanelRefresher p in GameManager.me.uiManager.popupSkillPreview.panelRefresher)
			{
				p.draw();
			}

			break;
			
		case GameIDData.Type.Unit:
			
			foreach(UIPanelRefresher p in GameManager.me.uiManager.popupSummonDetail.panelRefresher)
			{
				p.draw();
			}

			break;
		}

		StartCoroutine(onCompleteReinforceProcess());
	}


	IEnumerator onCompleteReinforceProcess()
	{
		yield return new WaitForSeconds(0.5f);

		refreshInventory(reinforceType);
		
		switch(reinforceType)
		{
		case GameIDData.Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.btnClose.gameObject.SetActive(true);
			
			if(TutorialManager.nowPlayingTutorial("T46"))
			{
				TutorialManager.uiTutorial.setArrowAndDim(997,530,false);
			}
			break;
		case GameIDData.Type.Skill:
			GameManager.me.uiManager.popupSkillPreview.btnClose.gameObject.SetActive(true);
			GameManager.me.uiManager.popupSkillPreview.reinforcePlayScene();
			
			if(TutorialManager.nowPlayingTutorial("T45"))
			{
				TutorialManager.instance.openDialog(200,450,true,true);
				TutorialManager.uiTutorial.skillGuide.start();
			}
			
			break;
		case GameIDData.Type.Unit:
			GameManager.me.uiManager.popupSummonDetail.btnClose.gameObject.SetActive(true);
			
			if(TutorialManager.nowPlayingTutorial("T44",20))
			{
				TutorialManager.uiTutorial.hide();
				TutorialManager.instance.openDialog(200,400,true,true);
			}
			break;
		}

		step = Step.Finish;

	}






	private bool _makingModeIsShop = false;



	public bool isPlayingMaking = false;

	public void playMakeResult(string[] results, bool isShopMode = true)
	{
		if(results == null || results.Length == 0) return;

		bool canStartMake = false;

		for(int i = 0; i < results.Length; ++i)
		{
			if( UIRewardNoticePanel.checkReward( results[i] , false) )
			{

			}
			else if(canStartMake == false)
			{
				GameIDData.Type t = GameIDData.getItemTypeById(results[i]);
				switch(t)
				{
				case GameIDData.Type.Equip:
				case GameIDData.Type.Skill:
				case GameIDData.Type.Unit:
					canStartMake = true;
					break;
				}
			}
		}

		if(canStartMake)
		{
			_makingModeIsShop = isShopMode;

			isPlayingMaking = true;

			step = Step.Start;
			
			if(_makingModeIsShop) GameManager.me.uiManager.popupShop.goShopBlocker.gameObject.SetActive(true);

			StartCoroutine(playMakeCt(results));

		}
		else
		{
			if(GameManager.me.uiManager.rewardNotice.isPlaying == false)
			{
				GameManager.me.uiManager.rewardNotice.next();
			}
		}
	}




	IEnumerator playMakeCt(string[] results)
	{
		_leftDisplayItems.Clear();

		card10Container.gameObject.SetActive(false);
		reinforceSlotContainer.SetActive(false);

		type = Type.UnitMake;

		GameIDData.Type itemType = GameIDData.Type.Equip;

		while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

		bool hasLoadingModel = false;

		for(int i = 0; i < results.Length; ++i)
		{
			itemType = GameIDData.getItemTypeById(results[i]);
			if(itemType == GameIDData.Type.None)
			{
				continue;
			}

			GameIDData d = new GameIDData();
			d.parse(results[i], itemType);
			_leftDisplayItems.Add(d);

			switch(itemType)
			{

			case GameIDData.Type.Unit:
				GameDataManager.instance.addLoadModelData(GameManager.info.monsterData[d.unitData.resource]);
				hasLoadingModel = true;
				break;
			}
		}


		_nowDisplayIndex = 0;
		isFirstCard = true;

		if(hasLoadingModel) 
		{
			GameDataManager.instance.startModelLoad();
			while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;
		}

		sendEvent(makeStarter);

		yield return new WaitForSeconds(0.4f);

		if((_leftDisplayItems.Count == 5 || _leftDisplayItems.Count == 10) && TutorialManager.instance.isTutorialMode == false)
		{
			GameManager.me.uiManager.activeRuneStudioSkipButton();
			step = Step.Start;
		}
		else
		{
			GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);
		}
	}


	public void refreshInventory(GameIDData.Type refreshType)
	{
		switch(refreshType)
		{
		case GameIDData.Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.refreshList();
			break;
		case GameIDData.Type.Skill:
			GameManager.me.uiManager.uiMenu.uiSkill.refreshMySkills();
			GameManager.me.uiManager.uiMenu.uiSkill.refreshSkillInven();
			break;
		case GameIDData.Type.Unit:
			GameManager.me.uiManager.uiMenu.uiSummon.refreshMyUnits();
			GameManager.me.uiManager.uiMenu.uiSummon.refreshUnitInven();
			break;
		}
	}




	int _nowDisplayIndex = 0;
	bool isFirstCard = false;


	private bool _isMouseDown = false;

	public void ShowNextButton()
	{
		state = State.Waiting;
		_isMouseDown = false;
		GameManager.setTimeScale = 1.0f;

		goMakeCompleteGuidePanel.gameObject.SetActive(true);

		if(moveablePanelParent.localScale.y < 1.0f)
		{
			Vector3 vs = moveablePanelParent.localScale;
			vs.y = 1.0f;
			moveablePanelParent.localScale = vs;
		}
	}


	public void nextMakeResult(bool skipSingleCardDisplay = false)
	{
//		Debug.LogError("=== nextMakeResult!");

		goMakeCompleteGuidePanel.gameObject.SetActive(false);

		if(_nowDisplayIndex < _leftDisplayItems.Count && skipSingleCardDisplay == false)
		{
			GameIDData nowData = _leftDisplayItems[_nowDisplayIndex];

			++_nowDisplayIndex;

			resetter.reset(Type.UnitMake);
			rootUnitMake.gameObject.SetActive(true);

			blackBackground.SetActive(true);

			isFirstCard = false;

			switch(nowData.type)
			{
			case GameIDData.Type.Equip:
				if(GameManager.me.uiManager.popupSkillPreview.gameObject.activeSelf) GameManager.me.uiManager.popupSkillPreview.isEnabled = false;
				if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) GameManager.me.uiManager.popupSummonDetail.hide();

				spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
				GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show(nowData, RuneInfoPopup.Type.Make, false);
				break;


			case GameIDData.Type.Skill:
				if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();
				if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) GameManager.me.uiManager.popupSummonDetail.hide();


				spSkillIcon.cachedTransform.parent.gameObject.SetActive(true);
				GameManager.me.uiManager.popupSkillPreview.showMakeResult (nowData);
				break;


			case GameIDData.Type.Unit:
				if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();
				if(GameManager.me.uiManager.popupSkillPreview.gameObject.activeSelf) GameManager.me.uiManager.popupSkillPreview.isEnabled = false;

				spSkillIcon.cachedTransform.parent.gameObject.SetActive(false);
				GameManager.me.uiManager.popupSummonDetail.showMakeResult (nowData);
				break;
			}

			switch( nowData.rare )
			{
			case RareType.D:
				playState( MAKE_D );
				//Invoke("nextMakeResult",2.5f);
				break;
			case RareType.C:
				playState( MAKE_C );
				//Invoke("nextMakeResult",2.5f);
				break;
			case RareType.B:
				playState( MAKE_C );
				//Invoke("nextMakeResult",2.5f);
				//playState( MAKE_SUPERRARE );
				//Invoke("nextMakeResult",7.5f);
				break;
			case RareType.A:
				//uigt_cast3 : A급이상 연출들어갈 때 (뽑기에도 동일하게 적용) <WMV>
				SoundData.play("uigt_cast3");

				playState( MAKE_A );

				break;
			case RareType.S:
			case RareType.SS:

				//uigt_cast3 : A급이상 연출들어갈 때 (뽑기에도 동일하게 적용) <WMV>
				SoundData.play("uigt_cast3");

				playState( MAKE_S );

				break;
			}
		}
		else
		{
			step = Step.Finish;

			if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) GameManager.me.uiManager.popupSummonDetail.hide();
			if(GameManager.me.uiManager.popupSkillPreview.isEnabled) GameManager.me.uiManager.popupSkillPreview.isEnabled = false;
			if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();

			GameManager.me.uiManager.goBtnRuneStudioSkip.SetActive(false);

			resetter.reset(Type.UnitMake);
			rootUnitMake.gameObject.SetActive(true);

			blackBackground.SetActive(true);

			card10Container.gameObject.SetActive(true);
			cam512.gameObject.SetActive(true);

			showCard512Studio(100);

			int totalOpenCard = _leftDisplayItems.Count;

			if(_makingModeIsShop == false)
			{
				totalOpenCard = 0;
				GameManager.me.uiManager.rewardNotice.next();
			}

			for(int i = 0; i < totalOpenCard && i < 10; ++i)
			{
				card10Cover[i].setData(_leftDisplayItems[i]);
				card10BgSlot[i].setData(_leftDisplayItems[i]);
			}

			if(totalOpenCard == 10)
			{
				playState ( MAKE_10CARD );

				Debug.LogError("==== 10 cards! delay endprocess");
				_isDelayEndProcessMode = true;
				StartCoroutine(delayEndProcess(8.0f));

			}
			else if(totalOpenCard == 5)
			{
				playState( MAKE_5CARD );

				Debug.LogError("==== 5 cards! delay endprocess");

				_isDelayEndProcessMode = true;
				StartCoroutine(delayEndProcess(5.0f));
			}
			else
			{
				Debug.LogError("==== normal endProcess!");
				endProcess();
				GameManager.me.uiManager.rewardNotice.start(true);
			}

			_leftDisplayItems.Clear();

			isPlayingMaking = false;

		}
	}

	private bool _isDelayEndProcessMode = false;
	IEnumerator delayEndProcess(float delay)
	{
		Debug.LogError("delayEndProcess : " + delay);
		yield return new WaitForSeconds(delay);

//		Debug.LogError("end delayEndProcess");
		if(isPlayingMaking || _isDelayEndProcessMode)
		{
			_isDelayEndProcessMode = false;
			endProcess();		
		}

		GameManager.me.uiManager.rewardNotice.start(true);
	}






	/*
	public GameObject challengeResultContainer;
	public GameObject challengeResult1Container;
	public GameObject challengeResult2Container;

	public GameObject[] challengeResult2StarGameObject = new GameObject[3];

	public PlayMakerFSM[] challengeResult1Star = new PlayMakerFSM[3];
	public PlayMakerFSM[] challengeResult2Star = new PlayMakerFSM[3];
	
	public Material[] challengeResult2SlotMaterial;
	
	public void initChallnegeResultMaterial()
	{
		for(int i = challengeResult2SlotMaterial.Length - 1; i >= 0; --i)
		{
			challengeResult2SlotMaterial[i].SetFloat("_DissolvePower", 0.65f);
		}
	}
	*/


}



public class Rune
{
	public enum Type
	{
		Unit, Equipment, Skill
	}

}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UILobby : UIBase {

	public UIButton btnChangeToMain, btnChangeToSub;

	public UIButton btnRubyShop;
	public UIButton btnGoldShop;
	public UIButton btnEnergyShop;

	public UIButton btnLevelup;

	public UIButton btnHero;
	public UIButton btnSummon;
	public UIButton btnSkill;
	public UIButton btnWorld;
	public UIButton btnShop;
	public UIButton btnMessage;
	public UIButton btnSetting;
	public UIButton btnMission;
	public UIButton btnFriend;

	public UIButton btnEvent;

	public UIButton btnStart;

	public UISprite spHasMissionIcon;
	public UISprite spHasTutorialForMission, spHasTutorialForFriend;

	public float nowZoomSize = 0.0f;

	public float lobbyCharacterZoomMin = 0.5f;
	public float lobbyCharacterZoomDefault = 0.7f;
	public float lobbyCharacterZoomMax = 1.2f;
	
	public float characterStageCenterPosX = 0;

	public GameObject goNewMsgContainer;

	public UILabel lbLevel, lbName, lbExp, lbEnergy, lbNextEnergyTime, lbGold, lbRuby, lbNewMsgNum;

	public UISprite spExp;


	public GameObject goHasNewSkillRune, goHasNewEquipRune, goHasNewUnitRune, goHasNewMission;

	public Transform heroParent;
	public Transform[] unitParent;
	
	public Player hero;
	public Monster[] units = new Monster[5];

	public UILobbySkillSlot[] nowSkills = new UILobbySkillSlot[3];

	public PhotoDownLoader playerPhoto;

	public LobbyCharacterPosition lobbyCharacterPositionSetter= new LobbyCharacterPosition();


	public Camera uiCamera;
	public Camera chracterCamera;
	
	private RaycastHit uiCheckHitInfo;

	const string HERO = "HERO";
	const string PET = "PET";
	const string UNIT0 = "UNIT0";
	const string UNIT1 = "UNIT1";
	const string UNIT2 = "UNIT2";
	const string UNIT3 = "UNIT3";
	const string UNIT4 = "UNIT4";


	public GameObject[] _lobbyBackground = new GameObject[LOBBY_MAP_NAMES.Length];
	public static readonly string[] LOBBY_MAP_NAMES = new string[]{"act1_lobby","act2_lobby","act3_lobby","act4_lobby","act5_lobby","act6_lobby"};

	void Awake()
	{
		_lobbyBackground = new GameObject[LOBBY_MAP_NAMES.Length];

		characterStageCenterPosX = characterCameraTransform.transform.localPosition.x;
		
		lobbyTreeContainer.transform.localScale = new Vector3(lobbyCharacterZoomDefault,lobbyCharacterZoomDefault,lobbyCharacterZoomDefault);
		nowZoomSize = lobbyCharacterZoomDefault;


		UIEventListener.Get(btnRubyShop.gameObject).onClick = onClickRubyShop;
		UIEventListener.Get(btnGoldShop.gameObject).onClick = onClickGoldShop;
		UIEventListener.Get(btnEnergyShop.gameObject).onClick = onClickEnergyShop;

		UIEventListener.Get(btnHero.gameObject).onClick = onClickHero;
		UIEventListener.Get(btnSummon.gameObject).onClick = onClickSummon;
		UIEventListener.Get(btnSkill.gameObject).onClick = onClickSkill;
		UIEventListener.Get(btnWorld.gameObject).onClick = onClickWorld;
		UIEventListener.Get(btnShop.gameObject).onClick = onClickShop;
		UIEventListener.Get(btnMessage.gameObject).onClick = onClickMessage;
		UIEventListener.Get(btnSetting.gameObject).onClick = onClickSetting;

		UIEventListener.Get(btnMission.gameObject).onClick = onClickMission;
		UIEventListener.Get(btnFriend.gameObject).onClick = onClickFriend;
		UIEventListener.Get(btnStart.gameObject).onClick = onClickStart;

		UIEventListener.Get(btnEvent.gameObject).onClick = onClickEvent;


		UIEventListener.Get(btnHero.gameObject).onPress = onPressHero;
		UIEventListener.Get(btnSummon.gameObject).onPress = onPressSummon;
		UIEventListener.Get(btnSkill.gameObject).onPress = onPressSkill;
		UIEventListener.Get(btnWorld.gameObject).onPress = onPressWorld;
		UIEventListener.Get(btnShop.gameObject).onPress = onPressShop;
		UIEventListener.Get(btnMessage.gameObject).onPress = onPressMessage;
		UIEventListener.Get(btnSetting.gameObject).onPress = onPressSetting;
		UIEventListener.Get(btnMission.gameObject).onPress = onPressMission;
		UIEventListener.Get(btnFriend.gameObject).onPress = onPressFriend;
		UIEventListener.Get(btnStart.gameObject).onPress = onPressStart;

		UIEventListener.Get(btnEvent.gameObject).onPress = onPressEvent;

//		GameDataManager.levelDispatcher -= updateLevel;
//		GameDataManager.levelDispatcher += updateLevel;

		GameDataManager.nameDispatcher -= updateName;
		GameDataManager.nameDispatcher += updateName;

//		GameDataManager.expDispatcher -= updateExp;
//		GameDataManager.expDispatcher += updateExp;

		GameDataManager.energyDispatcher -= updateEnergy;
		GameDataManager.energyDispatcher += updateEnergy;

		GameDataManager.chargeTimeDispatcher -= updateNextEnergy;
		GameDataManager.chargeTimeDispatcher += updateNextEnergy;

		GameDataManager.goldDispatcher -= updateGold;
		GameDataManager.goldDispatcher += updateGold;

		GameDataManager.rubyDispatcher -= updateRuby;
		GameDataManager.rubyDispatcher += updateRuby;


		goHasNewSkillRune.gameObject.SetActive(false);
		goHasNewEquipRune.gameObject.SetActive(false);
		goHasNewUnitRune.gameObject.SetActive(false);
		goHasNewMission.gameObject.SetActive(false);


		UIEventListener.Get(btnChangeToMain.gameObject).onClick = onClickChangeToMain;
		UIEventListener.Get(btnChangeToSub.gameObject).onClick = onClickChangeToSub;

		//lbLevel, lbName, lbExp, lbEnergy, lbNextEnergyTime, lbGold, lbRuby, lbNewMsgNum;
	}


	public static void setHasNewSkillRune(bool isVisible)
	{
		GameManager.me.uiManager.uiMenu.uiLobby.goHasNewSkillRune.SetActive(isVisible);
	}

	public static void setHasNewUnitRune(bool isVisible)
	{
		GameManager.me.uiManager.uiMenu.uiLobby.goHasNewUnitRune.SetActive(isVisible);
	}

	public static void setHasNewEquipRune(bool isVisible)
	{
		GameManager.me.uiManager.uiMenu.uiLobby.goHasNewEquipRune.SetActive(isVisible);
	}


	public static void setHasNewMission(bool isVisible)
	{
		GameManager.me.uiManager.uiMenu.uiLobby.goHasNewMission.SetActive(isVisible);
	}


	void onPressHero(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_hero");
		}
	}

	void onPressSummon(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_unitrune");
		}
	}

	void onPressSkill(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_skillrune");
		}
	}

	void onPressWorld(GameObject go, bool state)
	{
		if(state == true)
		{
			
		}
	}

	void onPressShop(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_shop");
		}
	}

	void onPressMessage(GameObject go, bool state)
	{
		if(state == true)
		{
			
		}
	}

	void onPressSetting(GameObject go, bool state)
	{
		if(state == true)
		{
			
		}
	}

	void onPressMission(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_ms");
		}
	}

	void onPressFriend(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_friend");
		}
	}



	void onPressStart(GameObject go, bool state)
	{
		if(state == true)
		{
			SoundData.play("uibt_world");
		}
	}


	void onPressEvent(GameObject go, bool state)
	{
		if(state == true)
		{
			//SoundData.play("uibt_world");
		}
	}



	void updateName()
	{
		lbName.text = Util.GetShortID( GameDataManager.instance.name, 10);
	}

//	void updateExp()
//	{
//		lbExp.text = Mathf.RoundToInt(GameDataManager.instance.expGauge * 100.0f) + "%";
//		float per = (float)GameDataManager.instance.expGauge;
//		spExp.fillAmount = per;
//	}
	
	void updateEnergy()
	{
		lbEnergy.text = "[fef9a8]" + GameDataManager.instance.energy + "[-][b0ada4] / " + GameDataManager.instance.maxEnergy + "[-]";
	}
	
	void updateNextEnergy(string time)
	{
		lbNextEnergyTime.text = time;
	}
	
	void updateGold()
	{
		lbGold.text = Util.GetCommaScore(GameDataManager.instance.gold);
	}
	
	void updateRuby()
	{
		lbRuby.text = Util.GetCommaScore(GameDataManager.instance.ruby);
	}

	void updateNewMsg()
	{
		//lbNewMsgNum.text = GameDataManager.instance.level.ToString();
	}

	public void refreshWaitFbPointNum()
	{
		if(GameDataManager.instance.waitFBPointNum > 0)
		{
			
		}
		else
		{
			
		}
	}



	
//	public Vector3 defaultCameraTransformLocalPosition = new Vector3(37.5f, 662.0f, 1020.0f);
//	public Vector3 defaultCameraTransformLocalRotation = new Vector3(29.5f, 181.0f, 0.0f);

	public Vector3 defaultCameraTransformLocalPosition = new Vector3(0, 662.0f, 1020.0f);
	public Vector3 defaultCameraTransformLocalRotation = new Vector3(29.5f, 179.0f, 0.0f);//-6.7f);


	Quaternion _q;

	public override void show ()
	{

#if UNITY_EDITOR
		PandoraManager.instance.localUser.image_url = "http://office.linktomorrow.com/common/windsoul/resource/empty1.png";
#endif

		base.show ();

		_currentCharacterIsMain = true;
		btnChangeToMain.gameObject.SetActive(false);
		btnChangeToSub.gameObject.SetActive(GameDataManager.instance.selectSubHeroId != null);

		startUpdateCharacter();
		refreshInfo();

		chracterCamera.fieldOfView = 15.0f * nowZoomSize;

		//characterCameraTransform.localPosition = defaultCameraTransformLocalPosition;

		_q.eulerAngles = defaultCameraTransformLocalRotation;
		chracterCamera.transform.localRotation = _q;


//		Debug.LogError("PandoraManager.instance.localUser.image_url : " + PandoraManager.instance.localUser.image_url);

		//playerPhoto.init(PandoraManager.instance.localUser.image_url);
		playerPhoto.down(PandoraManager.instance.localUser.image_url,70);

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) 
		{
			GameDataManager.instance.maxStage = 1;
			GameDataManager.instance.maxRound = 1;

			++GameDataManager.instance.maxAct;

			if(GameDataManager.instance.maxAct > 5) GameDataManager.instance.maxAct = 1;
		}
#endif
		updateLobbyMap();

		spHasMissionIcon.gameObject.SetActive(GameDataManager.instance.hasClearMission);

		updateNewMsgAni();

		setHasNewMission(UIMission.checkHasNewMission());

		updateCamera();

		checkInventoryLimit();

		btnMission.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,1,3));
		btnFriend.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,1,3));


		if(GameManager.me.introStep == Scene.IntroStep.PlayGame)
		{
			GameManager.me.specialPackageManager.check();
		}
	}


	public void updateLobbyMap()
	{
		StartCoroutine(updateLobbyMapCT());
	}


	public void visibleLobbyMap(bool isVisible)
	{
		try
		{
			int index = GameDataManager.instance.maxActWithCheckingMaxAct - 1;
			
			if(_lobbyBackground.Length > index)
			{
				if(_lobbyBackground[index] != null)
				{
					_lobbyBackground[index].gameObject.SetActive(isVisible);
				}
			}
		}
		catch
		{

		}
	}

	public void checkLobbyMap()
	{
		int index = GameDataManager.instance.maxActWithCheckingMaxAct - 1;

		if(_lobbyBackground.Length > index)
		{
			if(_lobbyBackground[index] == null)
			{
				updateLobbyMap();
			}
			else
			{
				if(_lobbyBackground[index].activeInHierarchy == false || _lobbyBackground[index].transform.parent != lobbyTreeContainer.transform)
				{
					_lobbyBackground[index].transform.parent = lobbyTreeContainer.transform;
					_v.x = 0; _v.y = 0; _v.z = 0; _lobbyBackground[index].transform.localPosition = _v; 
					
					switch(index)
					{
					case 1: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[index].transform.localScale = _v;  break;
					case 2: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[index].transform.localScale = _v; break;
					default: _v.x = 1; _v.y = 1; _v.z = 1; _lobbyBackground[index].transform.localScale = _v; break;
					}
					
					_lobbyBackground[index].gameObject.SetActive(true);
				}
			}
		}
	}
	
	
	
	IEnumerator updateLobbyMapCT()
	{
		int checkActIndex = GameDataManager.instance.maxActWithCheckingMaxAct;
		
		for(int i = 0; i < _lobbyBackground.Length; ++i)
		{
			//"Act1_Lobby"
			if(i == checkActIndex-1)
			{
				if(i < GameManager.MAX_ACT)
				{
					if(_lobbyBackground[i] != null)
					{
						if(_lobbyBackground[i].activeInHierarchy == false || _lobbyBackground[i].transform.parent != lobbyTreeContainer.transform)
						{
							_lobbyBackground[i].transform.parent = lobbyTreeContainer.transform;
							_v.x = 0; _v.y = 0; _v.z = 0; _lobbyBackground[i].transform.localPosition = _v; 
							
							switch(i)
							{
							case 1: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v;  break;
							case 2: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v; break;
							default: _v.x = 1; _v.y = 1; _v.z = 1; _lobbyBackground[i].transform.localScale = _v; break;
							}

							_lobbyBackground[i].gameObject.SetActive(true);
						}

						continue;
					}

					GameDataManager.instance.loadLobbyMap(LOBBY_MAP_NAMES[i]);
					while(GameDataManager.instance.isCompleteLoadLobbyMap == false) { yield return null; }

					GameDataManager.instance.lobbyMapResource.TryGetValue( LOBBY_MAP_NAMES[i] , out _lobbyBackground[i] );

					if(_lobbyBackground[i] != null)
					{
						_lobbyBackground[i].transform.parent = lobbyTreeContainer.transform;
						_v.x = 0; _v.y = 0; _v.z = 0; _lobbyBackground[i].transform.localPosition = _v; 
						
						switch(i)
						{
						case 1: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v;  break;
						case 2: _v.x = 10; _v.y = 10; _v.z = 10; _lobbyBackground[i].transform.localScale = _v; break;
						default: _v.x = 1; _v.y = 1; _v.z = 1; _lobbyBackground[i].transform.localScale = _v; break;
						}

						_lobbyBackground[i].gameObject.SetActive(true);
					}
				}
			}
			else
			{
				if(_lobbyBackground[i] != null)
				{
					GameObject.Destroy(_lobbyBackground[i].gameObject);
					_lobbyBackground[i] = null;
				}

				if(GameDataManager.instance.lobbyMapResource.ContainsKey(LOBBY_MAP_NAMES[i]))
				{
					if(GameDataManager.instance.lobbyMapResource[LOBBY_MAP_NAMES[i]] != null)
					{
						GameObject.Destroy(GameDataManager.instance.lobbyMapResource[LOBBY_MAP_NAMES[i]].gameObject);
					}

					GameDataManager.instance.lobbyMapResource[name] = null;
					GameManager.me.clearMemory();
				}
			}
		}
	}





	public void updateNewMsgAni()
	{
		if(lbNewMsgNum == null || goNewMsgContainer == null) return;

		if(lbNewMsgNum.gameObject.activeSelf && lbNewMsgNum.text.Length > 0)
		{
			goNewMsgContainer.animation.enabled = true;
		}
		else
		{
			goNewMsgContainer.animation.enabled = false;
			goNewMsgContainer.transform.localScale = Vector3.one;
		}
	}



	public void refreshInfo()
	{
//		Debug.Log("Should Modify!!!! == Guest Mode!!! ");

//		updateLevel();
		updateName();
//		updateExp();
		updateEnergy();
		//updateNextEnergy("00:00");
		updateGold();
		updateRuby();
		updateNewMsg();


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false)
#endif
		{
			if(TutorialManager.instance.isTutorialMode == false && GameManager.me.isGuest == false)
			{
				if(GameDataManager.instance.friendDatas == null)
				{
					EpiServer.instance.sendGetFriends(true);
				}
			}
		}


		refreshWaitFbPointNum();
	}





	void cleanLobbyCharacter()
	{
		if(hero != null) GameManager.me.characterManager.cleanMonster(hero);
		hero = null;

		for(int i = 0; i < 5; ++i)
		{
			if(units[i] != null)
			{
				GameManager.me.effectManager.removeBodyEffect(units[i]);
				GameManager.me.characterManager.cleanMonster(units[i]);
			}

			units[i] = null;
		}

		_playerUnitSlots = null;
		_playerSkillSlots = null;
	}


	private bool _currentCharacterIsMain = false;
	private string _selectHeroId = Character.LEO;

	private PlayerUnitSlot[] _playerUnitSlots;
	private PlayerSkillSlot[] _playerSkillSlots;
	private GamePlayerData _currentGamePlayerData;
	private bool _updateCharacterComplete = false;

	void startUpdateCharacter()
	{
		_updateCharacterComplete = false;

		_selectHeroId = GameDataManager.instance.selectHeroId;



		if(GameDataManager.instance.selectSubHeroId == null)
		{
			btnChangeToSub.gameObject.SetActive(false);
			btnChangeToMain.gameObject.SetActive(false);
			
			_currentCharacterIsMain = true;
		}
		else 
		{
			if(_currentCharacterIsMain == false)
			{
				_selectHeroId = GameDataManager.instance.selectSubHeroId;
			}
			
		}

		_currentGamePlayerData = GameDataManager.instance.heroes[_selectHeroId];

		_playerUnitSlots = GameDataManager.instance.playerUnitSlots[_selectHeroId];
		_playerSkillSlots = GameDataManager.instance.playerSkillSlots[_selectHeroId];

		for(int i = 0; i < 3; ++i)
		{
			if(_playerSkillSlots[i].isOpen) nowSkills[i].setData(_playerSkillSlots[i].infoData, _currentGamePlayerData);
			else nowSkills[i].setData(null);
		}

		StartCoroutine(updateCharacter());
	}




	IEnumerator updateCharacter()
	{

		if(hero != null) GameManager.me.characterManager.cleanMonster(hero);
		for(int i = 0; i < 5; ++i)
		{
			if(units[i] != null) GameManager.me.characterManager.cleanMonster(units[i]);
			units[i] = null;
		}

		GameDataManager.instance.loadDefaultModelData();
		
		while(GameDataManager.instance.isCompleteLoadModel == false)
		{ 
			yield return null;
		}

		while(GameDataManager.instance.isCompleteLoadLobbyMap == false)
		{ 
			yield return null;
		}


		hero = (Player)GameManager.me.characterManager.getMonster(true,true,_selectHeroId,false);//  (Player)Instantiate(GameManager.me.player);
		hero.init(_currentGamePlayerData,true,false);

		hero.pet = (Pet)GameManager.me.characterManager.getMonster(false,true,_currentGamePlayerData.partsVehicle.parts.resource.ToUpper(),false);
		hero.pet.init(hero);

		hero.setParent(heroParent);
		hero.cTransform.localScale = new Vector3(1,1,1);		
		hero.cTransform.localPosition = new Vector3(0,0,0);	

		hero.setPositionCtransform(heroParent.position);

		Quaternion q = hero.tf.localRotation;
		Vector3 v = q.eulerAngles;
		v.x = 0; v.y = 0; v.z = 0;
		q.eulerAngles = v;
		hero.state = Monster.NORMAL;

		hero.pet.collider.enabled = true;
		hero.pet.collider.name = "PET";

		hero.pet.tf.localRotation = q;	
		hero.pet.cTransform.localRotation = q;


		for(int i = 0; i < 5; ++i)
		{
			if(_playerUnitSlots[i].isOpen == false)
			{
				continue;
			}
			
			units[i] = GameManager.me.characterManager.getMonster(false, true, _playerUnitSlots[i].unitData.resource, false);
			units[i].isEnabled = true;
			CharacterUtil.setRare(_playerUnitSlots[i].unitData.rare, units[i]);
			units[i].setParent( unitParent[i] );

			units[i].cTransform.localPosition = new Vector3(0,0,0);	

			PlayerUnitSlot pus = _playerUnitSlots[i];
			units[i].init(pus.unitInfo.transcendData, pus.unitInfo.transcendLevel, pus.unitData.id,true,Monster.TYPE.UNIT);

			units[i].normalCollider.enabled = true;
			units[i].name = "UNIT"+i;
			units[i].shadow.gameObject.SetActive( true );
			units[i].initShadowAndEffectSize();
			units[i].startAction();
			units[i].setIdleAndFreeze(true);

			if(units[i].ani.GetClip(Monster.NORMAL_LOBBY) != null)
			{
				units[i].state = Monster.NORMAL_LOBBY;
			}
			else
			{
				units[i].state = Monster.NORMAL;
			}

			units[i].renderAniRightNow();

			 q = units[i].transform.localRotation;
			 v = q.eulerAngles;

			v.x = 0; v.y = 0; v.z = 0;
			q.eulerAngles = v;
			units[i].transform.localRotation = q;		
			units[i].cTransform.localRotation = q;

			GameManager.me.effectManager.checkUnitBodyEffect(units[i], true);

		}

		lobbyCharacterPositionSetter.changePosition(units);

		nowZoomSize = LobbyCharacterPosition.defaultZoomSize;

//		if(LobbyCharacterPosition.hasOutsidePosition) nowZoomSize = 1.0f;
//		else nowZoomSize = 0.72f;

		chracterCamera.fieldOfView = 15.0f * nowZoomSize;

		_updateCharacterComplete = true;
	}
	
	
	public override void hide ()
	{
		base.hide ();		
		cleanLobbyCharacter();
	}

	private int _tempIndex = 0;
	void checkAdvice()
	{
		UISystemPopup.open(UISystemPopup.PopupType.Advice, "", checkAdvice, checkAdvice, _tempIndex);
		if(_tempIndex == 0) _tempIndex = 1;
		else _tempIndex = 0;
	}

	int i = 0;
	public void onClickStart(GameObject go)
	{
//		UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("CHAMP_WAITING"));
//		UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("HELL_WAITING"));


//		UISystemPopup.open(UISystemPopup.PopupType.Default, "fuck", null, null);
//		UISystemPopup.open(UISystemPopup.PopupType.Advice, "", checkAdvice, checkAdvice, _tempIndex);
//		UISystemPopup.open(UISystemPopup.PopupType.YesNo, "", null, null);


//		UISystemPopup.open(UISystemPopup.PopupType.Advice, "", checkAdvice, checkAdvice, 0);

//		return;

//		UIMissionClearNotice.instance.start(GameDataManager.instance.missions);
//
//		return;

//		RuneStudioMain.instance.playMakeResult(new string[]{"SK_1101_1_0","SK_5401_20_0"});





		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;
		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.WORLD_MAP);
		
		return;


		P_Popup p = new P_Popup();

		p.type = "TXT";
		p.size = "L";
		p.title = "리뷰 이벤트";
		p.text = "afdasf님 재미있게 플레이하고 계신가요?\n지금 리뷰를 작성하시면 20루비를 드립니다!\n리뷰를 작성하시겠습니까?";
		p.image = "image";
		p.delay = 1;
		p.buttonType = "OK:CLOSE,ACTION:DETAIL";
		p.eventId = "EVT_REVIEW";
		p.eventValue = null;
		p.actionType = "REVIEW";
		p.actionData = "market://details?id=com.linktomorrow.windsoul";
		p.options = null;

		UISystemPopup.open(UISystemPopup.PopupType.InGameNotice, "", null, null, p);

		return;

		//RuneStudioMain.instance.playMakeResult(new string[]{"UN50100103_10","LEO_HD1_11_0_1","UN50100103_10","LEO_HD1_11_0_1","UN50100103_10","SK_3401_20_1","UN50100103_10","LEO_HD1_11_0_1","UN50100103_10"});

		/*
		if(i > 2)
		{
			i = 0;
		}

		GameIDData nd = new GameIDData();
		nd.parse("SK_3202_20_0", GameIDData.Type.Skill);

		GameIDData nd2 = new GameIDData();
		nd2.parse("SK_3202_2_0", GameIDData.Type.Skill);

		if(i == 0)
		{
			GameManager.me.uiManager.popupSkillPreview.show( nd, UIPopupSkillPreview.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, null);
		}
		else if(i == 1)
		{
			GameManager.me.uiManager.popupSkillPreview.show( nd2, UIPopupSkillPreview.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, null);
		}
		else if(i == 2)
		{
			GameManager.me.uiManager.popupSkillPreview.show( nd, UIPopupSkillPreview.Type.Normal, UIReinforceBarPanel.isTabSlot == false, false, null, nd2);
		}

		++i;

		return;
*/

//


		GameManager.me.stageManager.setNowRound(GameManager.info.roundData["A2S2R2"],GameType.Mode.Epic);

		GameManager.me.uiManager.popupRoundClear.show();

		return;


		//HellModeManager.instance.totalScore = 123414;
		HellModeManager.instance.currentDistance = 894;
		HellModeManager.instance.killUnitCount = 150;
		HellModeManager.instance.roundIndex = 8;
		
		if(i == 0)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100","SK_1105_1_0",
				"UN10700101_0",
				"UN11000101_0",
				"SK_1301_1_0",
				"SK_1106_1_0",
				"UN10700101_0",
				"UN11000101_0",
				"SK_1301_1_0",
				"SK_1106_1_0",
				"UN10700101_0",
				"UN11000101_0",
				"SK_1301_1_0",
				"SK_1106_1_0",
				"UN19800101_0",
				"SK_1109_1_0"});
		}
		else if(i == 1)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100"});
		}
		else if(i == 2)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100", "SK_1105_1_0"});
		}
		else if(i == 3)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100", "SK_1105_1_0","SK_1105_1_0"});
		}
		else if(i == 4)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100", "SK_1105_1_0","SK_1105_1_0","SK_1105_1_0"});
		}
		else if(i == 5)
		{
			GameManager.me.uiManager.popupHellResult.show(new string[]{"GOLD_100", "SK_1105_1_0","SK_1105_1_0","SK_1105_1_0","SK_1105_1_0"});
			i = -1;
		}
		
		++i;
		
		return;



	}



	public void onClickEvent(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		if(string.IsNullOrEmpty( GameDataManager.instance.eventUrl ))
		{
			UISystemPopup.open("현재 준비된 이벤트가 없습니다.");
		}

		PandoraManager.instance.showWebView(GameDataManager.instance.eventUrl, onCloseEventPage);

	}


	void onCloseEventPage(string resultString)
	{
		if(EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
		{
			Debug.LogError("onCloseCouponPage : " + resultString);
		}
		
		EpiServer.instance.sendRefreshEvent("EVENT");

		GameManager.me.clearMemory();
	}



	public void onClickHero(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;
		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.HERO);
	}

	public void onClickSummon(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.SUMMON);		
	}

	
	public void onClickSkill(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.SKILL);		
	}

	
	public void onClickWorld(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;


	}
	
	public void onClickShop(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		// 300 sec is refresh interval.
		float intvTime = 300.0f;
		try{
			intvTime = float.Parse(Util.getUIText("SHOP_REFRESH_SEC"));
		}catch(Exception e){
		}

		if (GameDataManager.instance.lastTimeOfShopData + intvTime < Time.realtimeSinceStartup){
			EpiServer.instance.sendRefreshShop();
		}
		else{
			GameManager.me.uiManager.popupShop.showSpecialShop();
		}


	}


	public void onClickRubyShop(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showRubyShop();
	}

	public void onClickGoldShop(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showGoldShop();
	}


	public void onClickEnergyShop(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showEnergyShop();
	}


	public void onClickMessage(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;
		GameManager.me.uiManager.popupMessage.show();
	}


	public void onClickSetting(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;
		GameManager.me.uiManager.popupOption.show();
	}	


	public void onClickMission(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;



		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.MISSION);
	}


	public void onClickFriend(GameObject go)
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;





		GameManager.me.uiManager.uiMenu.changePanel(UIMenu.FRIEND);
	}



	
	
	public Transform characterCameraTransform;
	
	
	// --------------------------- 캐릭터 줌 관련 --------------------------- //
	
	float zoomSize = 0.0f;
	void LateUpdate()
	{
		if(GameManager.me == null || GameManager.me.uiManager.currentUI != UIManager.Status.UI_MENU || GameManager.me.uiManager.uiMenu.currentPanel != UIMenu.LOBBY) return;

		updateCamera();

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(UICamera.hoveredObject != UICamera.fallThrough)
			{
				if(GameManager.me.uiManager.uiMenu.rayCast(GameManager.me.uiManager.uiMenuCamera.camera, btnStart.gameObject) == false) return;
			}
			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;


			UIEndGamePopup.showEndPopup();

			return;
		}





	}


	void updateCamera()
	{
		_v = characterCameraTransform.transform.localPosition;
		_v.x = characterStageCenterPosX - transform.position.x;
		
		//zoomSize = lobbyTreeContainer.localScale.x;
		zoomSize = nowZoomSize;
		
		if(zoomSize <= 0.0f) zoomSize = 0.01f;
		
		float ratio = (zoomSize - lobbyCharacterZoomMin) / (lobbyCharacterZoomMax - lobbyCharacterZoomMin);
		
		ratio = 1 - ratio;
		
		// ratio가 1일때 y는 419. rx는 18.
		// ratio가 0일때 y는 662, rx는 29.5
		
		float targetY = defaultCameraTransformLocalPosition.y - (ratio)*(641.0f-340.0f);//(662.0f-419.0f);
		
		//_v.y = targetY;
		_v.y = Mathf.Lerp(_v.y, targetY, 0.9f + ratio);
		
		characterCameraTransform.transform.localPosition = _v; // 화면 고정을 위해 주석처리.
		
		_v = defaultCameraTransformLocalRotation;
		//_v.x -= (ratio)*(29.5f-18.0f);
		_v.x -= (ratio)*(29.5f-17.0f);
		
		_v.y = 179.0f; _v.z = 0.0f;
		
		_q.eulerAngles = _v;
		chracterCamera.transform.localRotation = _q; // 화면 고정을 위해 주석처리.
		
		// 오브젝트를 클릭하지 않았으면.
		if(finchAndSelectObject() == false && currentSelectObject == null)
		{
		}
	}


	
	
	// 화면 특정 좌표를 눌렀을때 반응 하는 것.	
	// ngui ui를 눌렀다면 반응을 안하고 아니라면 캐릭터는 그 위치로 이동한다.
	private float _draggingDistance = 0.0f;
	private bool _isTouchDragging = false;

	
	private float _dragginDistance2 = 0.0f;
	private bool _isTouchDragging2 = false;
	private bool _checkTouchStart = false;	
	
	private float touchDistance;	
	private Vector2 prevMousePosition = Vector2.zero;	
	
	public Transform characterStage;
	public Transform lobbyTreeContainer;
	
	private Vector3 _v;
	
	private GameObject currentSelectObject;
	
	private bool _isMouseDown = false;
	private bool _isMouseDownStart = false;


	private void zoomStage()
	{
		if(UICamera.hoveredObject != UICamera.fallThrough) return;
		//lobbyTreeContainer.localScale = _v;
		nowZoomSize = _v.x;
		chracterCamera.fieldOfView = 15.0f * nowZoomSize;
	}

	// Update is called once per frame
	private bool finchAndSelectObject () {
		
		Vector2 mousePos = Vector2.zero;
		
		bool isMouseUp = false;
		bool isDrag = false;
		
#if UNITY_EDITOR
		
			if(_isMouseDown == false) _isMouseDown = Input.GetMouseButtonDown(0);
			isMouseUp = Input.GetMouseButtonUp(0);
			if(isMouseUp) _isMouseDown = false;
		
			mousePos = Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IPHONE
			if(Input.touchCount >= 1)
			{
				Touch t = Input.GetTouch(0);
				_isMouseDown = (t.phase == TouchPhase.Began);
				isMouseUp = (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled);
				mousePos = t.position;
				isDrag = (t.phase == TouchPhase.Moved);
			}
#endif		
		
#if UNITY_EDITOR



		// 확대 축소. 핀치 줌.
		if(Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			//_v = lobbyTreeContainer.localScale;
			_v.x = nowZoomSize;
			_v.x = Mathf.Min(_v.x*1.2f, lobbyCharacterZoomMax);
			_v.y = _v.z = _v.x;
			zoomStage();

			//lobbyTreeContainer.localScale = _v;
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//_v = lobbyTreeContainer.localScale;
			_v.x = nowZoomSize;
			_v.x = Mathf.Max(_v.x/1.2f, lobbyCharacterZoomMin);			
			_v.y = _v.z =_v.x;			
			zoomStage();
			//lobbyTreeContainer.localScale = _v;
		}

		/*

		// 터치가 시작한 순간.
		if(Input.GetMouseButtonDown(0))
		{
			Debug.Log("터치가 시작.");
			
			prevMousePosition = mousePos;
			_draggingDistance = 0.0f;
			_isTouchDragging = false;
			_isMouseDown = true;
			_isMouseDownStart = true;
		}
		
		// 화면 이동. 마우스가 눌린 상태.
		if(Input.GetMouseButton(0) && _isMouseDownStart)
		{

			characterStage.Rotate(0,((prevMousePosition.x)-Input.mousePosition.x)*10.0f*Time.smoothDeltaTime, 0);
			
			float deltaX = prevMousePosition.x - mousePos.x;
			float deltaY = prevMousePosition.y - mousePos.y;
			_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
			if(_draggingDistance > 20.0f) _isTouchDragging = true;
			
			prevMousePosition = mousePos;
		}
*/

		
		if(Input.GetMouseButtonDown(0))
		{
			// 캐릭터 개체 하나씩 회전시키기위한 값들...
			GameObject sgo = getCharacterByScreenPosition(mousePos);

			prevMousePosition = mousePos;
			_draggingDistance = 0.0f;
			_isTouchDragging = false;
			_isMouseDown = true;
			_isMouseDownStart = true;

			if(sgo!=null)
			{
				currentSelectObject = sgo;			
			}
			//==================================//
		}
		else if(Input.GetMouseButtonUp(0))
		{
			_isMouseDownStart = false;

			getCharacterByScreenPosition(mousePos);

			if(currentSelectObject != null)
			{
				// 화면 이동이 이루어진 상태가 아니라면....
				// 해당 타일에 맞는 작업을 시작한다.
				if(_isTouchDragging == false)
				{
					playDefaultAnimation();
				}
				
				currentSelectObject = null;
		
				_draggingDistance = 0.0f;
				_isTouchDragging = false;			
			}
		}


		// 화면 이동. 마우스가 눌린 상태.
		if(Input.GetMouseButton(0) && _isMouseDownStart && currentSelectObject != null)
		{
			currentSelectObject.transform.Rotate(0,((prevMousePosition.x)-Input.mousePosition.x)*30.0f*Time.smoothDeltaTime, 0);
			
			float deltaX = prevMousePosition.x - mousePos.x;
			float deltaY = prevMousePosition.y - mousePos.y;
			_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
			if(_draggingDistance > 20.0f) _isTouchDragging = true;
			
			prevMousePosition = mousePos;
		}



#elif UNITY_IPHONE || UNITY_ANDROID

		if(Input.touchCount > 0)mousePos = Input.GetTouch(0).position;
		
		if(Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			
			if(touch.phase == TouchPhase.Began)
			{
				prevMousePosition = (Vector3)touch.position;
				
				GameObject sgo = getCharacterByScreenPosition(mousePos);
				
				if(sgo!=null)
				{
				   _draggingDistance = 0.0f;
				  _isTouchDragging = false;	
					
					currentSelectObject = sgo;
					return true;
				}
			}
			else if(touch.phase == TouchPhase.Moved)
			{
				float deltaX = prevMousePosition.x - mousePos.x;
				float deltaY = prevMousePosition.y - mousePos.y;
				_draggingDistance += ((deltaX>0)?deltaX:-deltaX)+((deltaY>0)?deltaY:-deltaY);
				if(_draggingDistance > 20.0f)
				{
					_isTouchDragging = true;
				}					
				
				//characterStage.Rotate(0,((prevMousePosition.x)-touch.position.x)*10.0f*Time.smoothDeltaTime,0);

				if(currentSelectObject != null) currentSelectObject.transform.Rotate(0,((prevMousePosition.x)-touch.position.x)*30.0f*Time.smoothDeltaTime,0);

				prevMousePosition = (Vector3)touch.position;
			}
			else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				if(currentSelectObject != null)
				{
					// 화면 이동이 이루어진 상태가 아니라면....
					// 해당 타일에 맞는 작업을 시작한다.
					if(_isTouchDragging == false)
					{
//						Debug.LogError("클릭 오브젝트 : " + currentSelectObject.name);
						playDefaultAnimation();
						//currentSelectObject.animation.Play("attack");	
					}
					
					currentSelectObject = null;
					_draggingDistance = 0.0f;
					_isTouchDragging = false;	
					
					return true;
				}
				
				_draggingDistance = 0.0f;
				_isTouchDragging = false;				
			}
		}


		else if(Input.touchCount > 1)
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(1).phase == TouchPhase.Began)
			{
				touchDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			}
			else if((Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) //!uiManager.isUIOver 
			{
				float dis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
			
				if(touchDistance>dis)
				{
					//_v = lobbyTreeContainer.localScale;
					_v.x = nowZoomSize;
					_v.x = Mathf.Min(_v.x*1.02f, lobbyCharacterZoomMax);
					_v.y = _v.z = _v.x;
					zoomStage();
					//lobbyTreeContainer.localScale = _v;
				}
				else if(touchDistance<dis)
				{
					//_v = lobbyTreeContainer.localScale;
					_v.x = nowZoomSize;
					_v.x = Mathf.Max(_v.x/1.02f, lobbyCharacterZoomMin);
					_v.y = _v.z =_v.x;			
					zoomStage();
					//lobbyTreeContainer.localScale = _v;
				}
				touchDistance = dis;
			}
			
			if(Input.GetTouch(1).phase == TouchPhase.Ended)
			{
				prevMousePosition = (Vector3)Input.GetTouch(0).position;
			}else if(Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				prevMousePosition = (Vector3)Input.GetTouch(1).position;
			}
			
			currentSelectObject = null;
		}


#endif

		return false;
	}	

	public GameObject getCharacterByScreenPosition(Vector3 pos)
	{
		if(UICamera.hoveredObject != UICamera.fallThrough) return null;

		if(Physics.Raycast(chracterCamera.ScreenPointToRay(Input.mousePosition), out uiCheckHitInfo))
		{
//			Debug.Log("getCharacterByScreenPosition : " + uiCheckHitInfo.collider.gameObject.name);
			switch( uiCheckHitInfo.collider.gameObject.name )
			{
			case HERO:
			case PET:
				return hero.cTransform.gameObject;
				break;
			case UNIT0:
			case UNIT1:
			case UNIT2:
			case UNIT3:
			case UNIT4:
				return uiCheckHitInfo.collider.gameObject;
				break;
			}
		}
		return null;
	}





	void playDefaultAnimation()
	{
		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		if(UICamera.hoveredObject != UICamera.fallThrough || TutorialManager.instance.isTutorialMode ) return;

		if(uiCheckHitInfo.collider == null || uiCheckHitInfo.collider.gameObject == null) return;

		if(RuneStudioMain.instance.isPlayingMaking) return;

		switch( uiCheckHitInfo.collider.gameObject.name )
		{
		case HERO:
		case PET:
			if(btnHero.gameObject.activeSelf) onClickHero( null );
			else 
			{
				hero.state = Monster.SHOOT;
				hero.render();
			}
			break;
		case UNIT0:
			GameManager.me.uiManager.popupSummonDetail.show(_playerUnitSlots[0].unitInfo, RuneInfoPopup.Type.PreviewOnly, false);
			break;
		case UNIT1:
			GameManager.me.uiManager.popupSummonDetail.show(_playerUnitSlots[1].unitInfo, RuneInfoPopup.Type.PreviewOnly, false);
			break;
		case UNIT2:
			GameManager.me.uiManager.popupSummonDetail.show(_playerUnitSlots[2].unitInfo, RuneInfoPopup.Type.PreviewOnly, false);
			break;
		case UNIT3:
			GameManager.me.uiManager.popupSummonDetail.show(_playerUnitSlots[3].unitInfo, RuneInfoPopup.Type.PreviewOnly, false);
			break;
		case UNIT4:
			GameManager.me.uiManager.popupSummonDetail.show(_playerUnitSlots[4].unitInfo, RuneInfoPopup.Type.PreviewOnly, false);
			break;
		}

	}
	

	void OnEnable()
	{
		goHasNewSkillRune.animation["UpDown"].time = 0;
		goHasNewUnitRune.animation["UpDown"].time = 0;
		goHasNewEquipRune.animation["UpDown"].time = 0;
		goHasNewMission.animation["UpDown"].time = 0;

		goHasNewSkillRune.animation.Play();
		goHasNewUnitRune.animation.Play();
		goHasNewEquipRune.animation.Play();
		goHasNewMission.animation.Play();

	}


	public void checkInventoryLimit()
	{
		if(gameObject.activeSelf == false || gameObject.activeInHierarchy == false ) return;

		if(TutorialManager.instance.isTutorialMode == false && TutorialManager.instance.isReadyTutorialMode == false)
		{
			if(GameDataManager.instance.partsInventoryList.Count > GameDataManager.INVENTORY_LIMIT)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("OVER_INVEN_LIMIT",Util.getUIText("HERO_EQUIP")));
			}
			else if(GameDataManager.instance.unitInventoryList.Count > GameDataManager.INVENTORY_LIMIT)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("OVER_INVEN_LIMIT",Util.getUIText("UNIT_RUNE")));
			}
			else if(GameDataManager.instance.skillInventoryList.Count > GameDataManager.INVENTORY_LIMIT)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("OVER_INVEN_LIMIT",Util.getUIText("SKILL_RUNE")));
			}
		}
	}




	void onClickChangeToMain(GameObject go)
	{
		_currentCharacterIsMain = true;
		cleanLobbyCharacter();
		startUpdateCharacter();
		btnChangeToMain.gameObject.SetActive(false);
		btnChangeToSub.gameObject.SetActive(true);
	}
	
	
	void onClickChangeToSub(GameObject go)
	{
		_currentCharacterIsMain = false;
		cleanLobbyCharacter();
		startUpdateCharacter();
		btnChangeToMain.gameObject.SetActive(true);
		btnChangeToSub.gameObject.SetActive(false);
	}

}

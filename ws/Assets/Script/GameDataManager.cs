using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class GameDataManager : MonoBehaviour {

	public const string PERIOD = "PERIOD";


	int i=0,j=0,len=0;
	
	private static GameDataManager _instance;
	
	public static GameDataManager instance
	{
		get
		{
			if(_instance == null) _instance = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();

			return _instance;
		}
	}
	
	void OnDestroy()
	{
		unloadAllMapAssetBundle();
		loadedBundle = null;

		nameDispatcher = null;
//		expDispatcher = null;

		energyDispatcher = null;
		friendPointDispatcher = null;
		hellTicketDispatcher = null;
		goldDispatcher = null;
		rubyDispatcher = null;
		runeStoneDispatcher = null;

		chargeTimeDispatcher = null;
		priceDataDispatcher = null;
		replayAttackerData = null;

		playData.Clear();
		playSubData.Clear();

		sigongList = null;

		if(playerUnitSlots != null) playerUnitSlots.Clear();
		playerUnitSlots = null;

		if(playerSkillSlots != null) playerSkillSlots.Clear();
		playerSkillSlots = null;

		_instance = null;
	}

	const string EXP = "EXP";
	const string ENERGY = "ENERGY";
	const string GOLD = "GOLD";
	const string RUBY = "RUBY";	
	const string SKILLRUNE = "SKILLRUNE";
	const string UNITRUNE = "UNITRUNE";
	const string NEXT_ENERGY_SEC = "NEXT_ENERGY_SEC";
	const string CHANGED = "CHANGED";	

	public const int MAX_UNIT_INVENTORY = 15;

	public const int INVENTORY_LIMIT = 700;


	public enum ServiceMode
	{
		Normal, IOS_SUMMIT, CBT
	}

	public ServiceMode serviceMode = ServiceMode.Normal;


	private string _deviceId = null;
	public string deviceId
	{
		set
		{
			_deviceId = value;
			if(string.IsNullOrEmpty(value))
			{
				PlayerPrefs.DeleteKey("DEVICE");
			}
			else
			{
				PlayerPrefs.SetString("DEVICE",value);
			}
		}
		get
		{
			return _deviceId;
		}
	}

	public string savedDeviceId
	{
		get
		{
			if(PlayerPrefs.HasKey("DEVICE"))
			{
				return PlayerPrefs.GetString("DEVICE");
			}
			return null;
		}
	}



	public Dictionary<string, GameIDData> unitInventory = new Dictionary<string, GameIDData>();
	public List<GameIDData> unitInventoryList = new List<GameIDData>();

	public Dictionary<string, GameIDData> skillInventory = new Dictionary<string, GameIDData>();
	public List<GameIDData> skillInventoryList = new List<GameIDData>();

	public Dictionary<string, GameIDData> partsInventory = new Dictionary<string, GameIDData>();
	public List<GameIDData> partsInventoryList = new List<GameIDData>();

	public Dictionary<string, int> historyUnitRunes = null;
	public Dictionary<string, int> historySkillRunes = null;

	public delegate void PlayerInfoUpdater();

	public static PlayerInfoUpdater nameDispatcher;
	private string _name = "";
	public string name
	{
		get{ return _name; }
		set{ _name = value; if(GameDataManager.nameDispatcher != null) GameDataManager.nameDispatcher(); }
	}

//	public static PlayerInfoUpdater levelDispatcher;
//	private int _level = 1;
	public int level = 1;
//	{
//		get{ return _level; }
//		set{
//			_level = value; 
//			if(GameDataManager.levelDispatcher != null) GameDataManager.levelDispatcher(); 
//
//			if(heroes != null)
//			{
//				foreach(KeyValuePair<string, GamePlayerData> kv in heroes)
//				{
//					if(kv.Value != null) kv.Value.level = level;
//				}
//			}
//		}
//	}

	public static PlayerInfoUpdater energyDispatcher;
	private int _energy = 0;
	public int energy
	{
		get{ return _energy; }
		set{ _energy = value; if(GameDataManager.energyDispatcher != null) GameDataManager.energyDispatcher(); }
	}

	public Dictionary<string, int> maxEnergies = null;

	public int maxEnergy
	{
		get
		{
			if(maxEnergies != null && maxEnergies.ContainsKey(champLeague.ToString()))
			{
				return maxEnergies[champLeague.ToString()];
			}
			else return 5;
		}
	}

	public static PlayerInfoUpdater friendPointDispatcher;
	private int _friendPoint = 0;
	public int friendPoint
	{
		get{ return _friendPoint; }
		set{ _friendPoint = value; if(GameDataManager.friendPointDispatcher != null) GameDataManager.friendPointDispatcher(); }
	}


	public static PlayerInfoUpdater hellTicketDispatcher;
	private int _hellTicket = 0;
	public int hellTicket
	{
		get{ return _hellTicket; }
		set{ _hellTicket = value; if(GameDataManager.hellTicketDispatcher != null) GameDataManager.hellTicketDispatcher(); }
	}

	public int maxHellTicket = 3;



	public static PlayerInfoUpdater runeStoneDispatcher;
	private int _runeStone = 0;
	public int runeStone
	{
		get{ return _runeStone; }
		set{ 
			_runeStone = value; 
			if(GameDataManager.runeStoneDispatcher != null)
			{
				GameDataManager.runeStoneDispatcher(); 
			}
		}
	}



	public static PlayerInfoUpdater goldDispatcher;
	private int _gold = 0;
	public int gold
	{
		get{ return _gold; }
		set{ 
			_gold = value; 
			if(GameDataManager.goldDispatcher != null)
			{
				GameDataManager.goldDispatcher(); 
			}
		}
	}

	public static PlayerInfoUpdater rubyDispatcher;
	private int _ruby = 0;
	public int ruby 
	{
		get{ return _ruby; }
		set{ _ruby = value; if(GameDataManager.rubyDispatcher != null) GameDataManager.rubyDispatcher(); }
	}



	public bool needUpdateEnergyTick = false;
	public DateTime energyChargeCheckTime;
	public delegate void PlayerInfoTimeUpdater(string time);
	public static PlayerInfoTimeUpdater chargeTimeDispatcher;
	private int _chargeTime = 0;
	public int chargeTime
	{
		get{ return _chargeTime; }
		set
		{ 
			_chargeTime = value; 

			energyChargeCheckTime = DateTime.Now; 
			needUpdateEnergyTick = true; 
		}
	}


	public static PlayerInfoUpdater priceDataDispatcher;
	private P_Shop _priceData;
	public P_Shop shopData
	{
		get { return _priceData; }
		set
		{
			_priceData = value;
			if(priceDataDispatcher != null) GameDataManager.priceDataDispatcher();
		}
	}




	public ToC_CHAMPIONSHIP_DATA championshipData = null;
	public ToC_LAST_CHAMPIONSHIP_DATA lastChampionshipData = null;

	public DateTime championShipCheckTime = DateTime.Now;

	public Xint champLeague;
	public int champGroupId;

	public int inviteCount;

	public bool hasClearMission = false;

	private int _champMemberId = -100;
	public int champMemberId
	{
		set
		{
			if(_champMemberId != value)
			{
				_champMemberId = value;
				
				if(GameManager.me != null && GameManager.me.uiManager != null)
				{
					GameManager.me.uiManager.uiMenu.uiWorldMap.updateChampionshipStatus();
				}
			}
		}
		get
		{
			return _champMemberId;
		}
	}


	private int _championshipStatus = -100;
	public int championshipStatus
	{
		set
		{
			if(_championshipStatus != value)
			{
				_championshipStatus = value;
				
				if(GameManager.me != null && GameManager.me.uiManager != null)
				{
					GameManager.me.uiManager.uiMenu.uiWorldMap.updateChampionshipStatus();
				}
			}
		}
		get
		{
			return _championshipStatus;
		}
	}

	private int _championshipDefence = WSDefine.NO;

	public int championshipDefence
	{
		set
		{
			_championshipDefence = value;

			if(GameManager.me != null && GameManager.me.uiManager != null)
			{
				GameManager.me.uiManager.uiMenu.uiWorldMap.spChampionshipExclamation.gameObject.SetActive(value == WSDefine.YES && TutorialManager.instance.clearCheck("T24"));
			}
		}
		get
		{
			return _championshipDefence;
		}
	}


	public ToC_PREPARE_HELL hellInfo = null;
	public DateTime hellModeCheckTime = DateTime.Now;



	// 에픽스테이지 후 재경기에 대한 확답을 받았는지.
	// 안받았다면 화면 들어가서 바로 뿌려야한다.

	
	
	// 게임에서 지면 무조건 라운드는 리셋된다.
	// 하지만 이어하기를 하려면 해당 라운드가 필요하다.
	// failedRound는 실패해서 다시시작해야할 라운드 정보.


	public int prevPlayRound = 0;

	
	// 튜토리얼이 완전 종료되었는지 확인.
	public int completeTutorial = 0;

	public Dictionary<string, int> tutorialHistory = new Dictionary<string, int>();

	public Dictionary<string, Dictionary<string, string>> selectUnitRunes = new Dictionary<string, Dictionary<string, string>>();
	public Dictionary<string, Dictionary<string, string>> selectSkillRunes = new Dictionary<string, Dictionary<string, string>>();

	public Dictionary<string, int> composePrices;

	public Dictionary<string, int> evolvePrices;

	public Dictionary<string, int> reforgePrices;

	public string[] getSelectUnitRunes(string heroId)
	{
		if(heroId == null) heroId = selectHeroId;

		string[] u = new string[5];

		u[0] = selectUnitRunes[heroId][UnitSlot.U1];
		u[1] = selectUnitRunes[heroId][UnitSlot.U2];
		u[2] = selectUnitRunes[heroId][UnitSlot.U3];
		u[3] = selectUnitRunes[heroId][UnitSlot.U4];
		u[4] = selectUnitRunes[heroId][UnitSlot.U5];

		return u;
	}

	public string[] getSelectSkillRunes(string heroId)
	{
		if(heroId == null) heroId = selectHeroId;
		
		string[] s = new string[3];
		
		s[0] = selectSkillRunes[heroId][SkillSlot.S1];
		s[1] = selectSkillRunes[heroId][SkillSlot.S2];
		s[2] = selectSkillRunes[heroId][SkillSlot.S3];

		return s;
	}



	public Dictionary<string, P_Sigong> sigongList;

	private int _showPhoto = WSDefine.YES;

	public int showPhoto
	{
		set
		{
			_showPhoto = value;
			GameManager.me.uiManager.popupOption.setShowPhoto(_showPhoto == WSDefine.YES);
			PlayerPrefs.SetInt("SHOWPHOTO",value);
		}
		get
		{
			return _showPhoto;
		}
	}


	private int _messageBlock = WSDefine.NO;
	public int messageBlock
	{
		set
		{
			_messageBlock = value;
			GameManager.me.uiManager.popupOption.setMessageBlock(_messageBlock == WSDefine.YES);
			PlayerPrefs.SetInt("MESSAGEBLOCK",value);
		}
		get
		{
			return _messageBlock;
		}
	}

	public string selectHeroId = Character.LEO;

	private string _selectSubHeroId = null;
	public string selectSubHeroId
	{
		set
		{
			if(string.IsNullOrEmpty(value)) _selectSubHeroId = null;
			else _selectSubHeroId = value;
		}
		get
		{
			return _selectSubHeroId;
		}
	}


	public string skillRuneBase = "";
	public string unitRuneBase = "";

	// 기존 방식에서는 얘를 사용.
	public PlayerSkillSlot[] skills
	{
		set
		{
			playerSkillSlots[selectHeroId] = value;
		}
		get
		{
			return playerSkillSlots[selectHeroId];
		}
	}

		
	public PlayerUnitSlot[] unitSlots
	{
		set
		{
			playerUnitSlots[selectHeroId] = value;
		}
		get
		{
			return playerUnitSlots[selectHeroId];
		}
	}


	// 덱 분리후에는 얘를 쓰게 될거다.
	public Dictionary<string, PlayerSkillSlot[]> playerSkillSlots = new Dictionary<string, PlayerSkillSlot[]>();
	public Dictionary<string, PlayerUnitSlot[]> playerUnitSlots = new Dictionary<string, PlayerUnitSlot[]>();


	void initPlayerSlots()
	{
#if UNITY_EDITOR
		Debug.LogError("== initPlayerSlots");
#endif
		initSinglePlayerSlots(Character.LEO);
		initSinglePlayerSlots(Character.KILEY);
		initSinglePlayerSlots(Character.CHLOE);
		initSinglePlayerSlots(Character.LUKE);
	}

	void initSinglePlayerSlots(string character)
	{
		if(playerUnitSlots.ContainsKey(character))
		{
			playerUnitSlots.Add(character, new PlayerUnitSlot[Player.SUMMON_SLOT_NUM]);
		}
		else
		{
			playerUnitSlots[character] =  new PlayerUnitSlot[Player.SUMMON_SLOT_NUM];
		}

		if(playerSkillSlots.ContainsKey(character))
		{
			playerSkillSlots.Add(character, new PlayerSkillSlot[Player.SKILL_SLOT_NUM]);
		}
		else
		{
			playerSkillSlots[character] =  new PlayerSkillSlot[Player.SKILL_SLOT_NUM];
		}


		for(i = 0; i < 5; ++i)
		{
			playerUnitSlots[character][i] = new PlayerUnitSlot();
		}
		for(i = 0; i < 3; ++i)
		{
			playerSkillSlots[character][i] = new PlayerSkillSlot();
		}
	}






	public Dictionary<string, GamePlayerData> heroes = new Dictionary<string, GamePlayerData>(StringComparer.Ordinal);

	public Dictionary<string, Dictionary<string, HeroPartsData>> defaultParts = new Dictionary<string, Dictionary<string, HeroPartsData>>(StringComparer.Ordinal);


	public Dictionary<string, P_FriendData> friendDatas = null;

	public int goldForRuby = 250;


	public Dictionary<string, int> playData = new Dictionary<string, int>();
	public Dictionary<string, int> playSubData = new Dictionary<string, int>();

	public Dictionary<string, int> getPlayData()
	{
		playData[GameDataManager.PERIOD] =  Mathf.CeilToInt( GameManager.me.stageManager.playTime );
		return playData;
	}


	public Dictionary<string, int> getPlaySubData()
	{
		playSubData[GameDataManager.PERIOD] =  Mathf.CeilToInt( GameManager.me.stageManager.playTime );
		
		return playSubData;
	}



	public string[] lastHellWaveInfo = null;


	public Dictionary<string, int> heroPrices;



	void Awake()
	{
		initPlayerSlots();

		unitInventory.Clear();
		unitInventoryList.Clear();

		skillInventory.Clear();
		skillInventoryList.Clear();

		partsInventory.Clear();
		partsInventoryList.Clear();

		playData.Add(UnitSlot.U1, 0);
		playData.Add(UnitSlot.U2, 0);
		playData.Add(UnitSlot.U3, 0);
		playData.Add(UnitSlot.U4, 0);
		playData.Add(UnitSlot.U5, 0);

		playData.Add(SkillSlot.S1, 0);
		playData.Add(SkillSlot.S2, 0);
		playData.Add(SkillSlot.S3, 0);

		playData.Add(PERIOD,0);

//		playData.Add("HERO","");

		playSubData.Add(UnitSlot.U1, 0);
		playSubData.Add(UnitSlot.U2, 0);
		playSubData.Add(UnitSlot.U3, 0);
		playSubData.Add(UnitSlot.U4, 0);
		playSubData.Add(UnitSlot.U5, 0);
		
		playSubData.Add(SkillSlot.S1, 0);
		playSubData.Add(SkillSlot.S2, 0);
		playSubData.Add(SkillSlot.S3, 0);
		
		playSubData.Add(PERIOD,0);
//		playSubData.Add("HERO","");

	}



	void Start()
	{
		if(DebugManager.instance.useDebug)
		{
			#if UNITY_EDITOR
				selectHeroId = DebugManager.instance.defaultHero;
			#endif
		}
	}


	// 현재 선택한 캐릭터.
	public string id = "LEO";

	string[] _defaultParts = new string[4];

	// This is Init when model load complete;
	public void init(string id, int level)
	{
		// 디버그 혹은 게스트 모드일때만 쓰자.
#if !UNITY_EDITOR
		return;
#endif

		if(DebugManager.instance.useDebug == false) return;

//		Debug.LogError("=== GameDataManager Init! === : " + level);
		if(heroes.ContainsKey(id) == false) heroes.Add(id,new GamePlayerData(id));
		heroes[id].level = level;

		SetupData sd = GameManager.info.setupData;

	}



	public Dictionary<string, GamePlayerData> defaultHeroData = new Dictionary<string, GamePlayerData>();

	public int[] reforgeLogic;

	// ui 용 기본 캐릭터 데이터를 준비한다.
	public void makeDefaultGamePlayerData()
	{
		reforgeLogic = Util.stringToIntArray(Util.getUIText("REFORGE_LOGIC"), ',');

#if UNITY_EDITOR

		Debug.Log("==========================================="+ Util.getUIText("REFORGE_LOGIC"));

#endif

		defaultHeroData.Clear();
		defaultHeroData.Add(Character.KILEY, new GamePlayerData(Character.KILEY));
		defaultHeroData.Add(Character.LEO, new GamePlayerData(Character.LEO));
		defaultHeroData.Add(Character.CHLOE, new GamePlayerData(Character.CHLOE));
		
		defaultHeroData[Character.LEO] = DebugManager.instance.setPlayerData(defaultHeroData[Character.LEO], false, Character.LEO, 
		                                                                     GameManager.info.setupData.defaultLeo[0],
		                                                                     GameManager.info.setupData.defaultLeo[1],
		                                                                     GameManager.info.setupData.defaultLeo[2],
		                                                                     GameManager.info.setupData.defaultLeo[3]);
		                                                                     
		defaultHeroData[Character.KILEY] = DebugManager.instance.setPlayerData(defaultHeroData[Character.KILEY], false, Character.KILEY,  
		                                                                       GameManager.info.setupData.defaultKiley[0],
		                                                                       GameManager.info.setupData.defaultKiley[1],
		                                                                       GameManager.info.setupData.defaultKiley[2],
		                                                                       GameManager.info.setupData.defaultKiley[3]);	


		defaultHeroData[Character.CHLOE] = DebugManager.instance.setPlayerData(defaultHeroData[Character.CHLOE], false, Character.CHLOE,  
		                                                                       GameManager.info.setupData.defaultChloe[0],
		                                                                       GameManager.info.setupData.defaultChloe[1],
		                                                                       GameManager.info.setupData.defaultChloe[2],
		                                                                       GameManager.info.setupData.defaultChloe[3], null, null, "", "pc_chloe_face01");	


		DebugManager.instance.setDebugTagPVPData();
	}





	public bool checkIsIsThisSelectedParts(HeroPartsData pd)
	{
		if(heroes.ContainsKey(pd.character))
		{
			if(heroes[pd.character].partsHead.partsId == pd.id ||
			   heroes[pd.character].partsWeapon.partsId == pd.id ||
			   heroes[pd.character].partsBody.partsId == pd.id ||
			   heroes[pd.character].partsVehicle.partsId == pd.id)
			{
				return true;
			}
		}

		return false;
	}



	public P_Hero getMainHeroData()
	{
		return serverHeroData[selectHeroId];
	}

	public P_Hero getSubHeroData()
	{
		if(selectSubHeroId != null && serverHeroData.ContainsKey(selectSubHeroId))
		{
			return serverHeroData[selectSubHeroId];
		}

		return null;
	}


	public GamePlayerData getSubGamePlayerData()
	{
		if(selectSubHeroId != null && serverHeroData.ContainsKey(selectSubHeroId) && heroes.ContainsKey(selectSubHeroId))
		{
			return heroes[selectSubHeroId];
		}

		return null;
	}

	public static GamePlayerData replayAttackerData;

	public static GamePlayerData replayAttacker2Data;


	public static GamePlayerData selectedPlayerData
	{
		get
		{
#if UNITY_EDITOR
			if(DebugManager.instance.useDebug) return (instance.heroes.ContainsKey(DebugManager.instance.defaultHero))?instance.heroes[DebugManager.instance.defaultHero]:null;
#endif

			return (instance.heroes.ContainsKey(instance.selectHeroId))?instance.heroes[instance.selectHeroId]:null;
		}
	}


	public void resetPlayerInfo()
	{
#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false)
#endif
		{
			parseHeroInven(serverHeroData);

			List<string> heroList = new List<string>();
			heroList.Add(Character.LEO);
			heroList.Add(Character.CHLOE);
			heroList.Add(Character.KILEY);

			parseSelectUnits( selectHeroId, selectUnitRunes[selectHeroId] );
			parseSelectSkills( selectHeroId, selectSkillRunes[selectHeroId] );

			heroList.Remove(selectHeroId);

			if(selectSubHeroId != null && selectUnitRunes.ContainsKey(selectSubHeroId))
			{
				parseSelectUnits( selectSubHeroId, selectUnitRunes[selectSubHeroId] );
				parseSelectSkills( selectSubHeroId, selectSkillRunes[selectSubHeroId] );
				heroList.Remove(selectSubHeroId);
			}

			foreach(string name in heroList){
				parseSelectUnits( name, selectUnitRunes[name] );
				parseSelectSkills( name, selectSkillRunes[name] );
			}


			heroList.Clear();
		}
	}


	float _prevTime = 0.0f;
	public void Update()//IEnumerator  updateTimeTick()
	{
		//if(needUpdateEnergyTick)
		{
			if(Time.realtimeSinceStartup - _prevTime >= 0.5f)
			{
				_prevTime = Time.realtimeSinceStartup;
			}
			else return;

			//while(true)
			{
				//yield return new WaitForSeconds(1.0f);z

				if(_chargeTime > 0)
				{
					TimeSpan ts = (DateTime.Now - energyChargeCheckTime);
					int leftTime = _chargeTime - (int)ts.TotalSeconds;
					
					if(leftTime < 0 && GameManager.me.uiManager.currentUI == UIManager.Status.UI_MENU && GameManager.me.uiManager.uiLoading.gameObject.activeSelf == false)
					{
						if(GameDataManager.chargeTimeDispatcher != null) GameDataManager.chargeTimeDispatcher("00:00");
						needUpdateEnergyTick = false;
						_chargeTime = -1;
						EpiServer.instance.sendEnergyTick();
					}
					else
					{
						if(GameDataManager.chargeTimeDispatcher != null) GameDataManager.chargeTimeDispatcher(Util.secToHourMinuteSecondString(leftTime)); 
					}
				}
				else if(_energy >= 5)
				{
					if(GameDataManager.chargeTimeDispatcher != null) GameDataManager.chargeTimeDispatcher(MAX);
				}
			}
		}
	}

	const string MAX = "MAX";

	public static bool priceCheck(ShopItem.Type type, float price, bool openShop = true)
	{
		return priceCheck(type, Mathf.RoundToInt(price), openShop);
	}


	public static bool priceCheck(ShopItem.Type type, int price, bool openShop = true)
	{
		switch(type)
		{
		case ShopItem.Type.gold:
			if(GameDataManager.instance.gold < price)
			{
				if(openShop)
				{
					UISystemPopup.open(UISystemPopup.PopupType.GoToGoldShop);
				}
				return false;
			}
			break;
		case ShopItem.Type.ruby:
			if(GameDataManager.instance.ruby < price)
			{
				if(openShop)
				{
					UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
				}
				return false;
			}
			break;
		}
		
		return true;
	}



	public bool roundClearStatusCheck(int checkAct, int checkStage, int checkRound)
	{
		if(maxAct > checkAct) return true;
		else if(maxAct == checkAct)
		{
			if(maxStage > checkStage) return true;
			else if(maxStage == checkStage)
			{
				if(maxRound > checkRound ) return true;
			}
		}
		
		return false;
	}




	public bool canCutScenePlay
	{
		set
		{
			PlayerPrefs.SetInt("CSPLAY",(value)?WSDefine.YES:WSDefine.NO);
		}
		get
		{
			if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong)
			{
				return true;
			}

			if(UILobbyStageDetailPanel.mustPlayCutScene)
			{
				return true;
			}

			if( PlayerPrefs.GetInt("CSPLAY",WSDefine.NO) == WSDefine.YES )
			{
				return true;
			}

			return false;
		}
	}




	public bool hasCharacter(string character)
	{
		switch(character)
		{
		case Character.KILEY:
			if(GameDataManager.instance.maxAct < 3) return false;
			break;
		case Character.CHLOE:
			if(GameDataManager.instance.maxAct < 5) return false;
			break;
		}

		return serverHeroData.ContainsKey(character);
	}



	public string getCharacterSelectedRune(string runeId, GameIDData.Type runeType, int didLoad)
	{
		if(didLoad == WSDefine.NO) return string.Empty;

		switch(runeType)
		{

		case GameIDData.Type.Equip:
			return getCharacterSelectedEquip(runeId);
		case GameIDData.Type.Skill:
			return getCharacterSelectedSkill(runeId);
		case GameIDData.Type.Unit:
			return getCharacterSelectedUnit(runeId);
		}

		return string.Empty;
	}


	public string getCharacterSelectedEquip(string runeId)
	{
		foreach(KeyValuePair<string, P_Hero> kv in serverHeroData)
		{
			if( string.IsNullOrEmpty( kv.Value.selEqts[ HeroParts.HEAD ] ) == false && kv.Value.selEqts[ HeroParts.HEAD ] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty( kv.Value.selEqts[ HeroParts.BODY ] ) == false && kv.Value.selEqts[ HeroParts.BODY ] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty( kv.Value.selEqts[ HeroParts.WEAPON ] ) == false && kv.Value.selEqts[ HeroParts.WEAPON ] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty( kv.Value.selEqts[ HeroParts.VEHICLE ] ) == false && kv.Value.selEqts[ HeroParts.VEHICLE ] == runeId)
			{
				return kv.Key;
			}
		}

		return string.Empty;
	}


	public string getCharacterSelectedUnit(string runeId)
	{
		foreach(KeyValuePair<string, Dictionary<string, string>> kv in selectUnitRunes)
		{
			if( string.IsNullOrEmpty(kv.Value[UnitSlot.U1]) == false && kv.Value[UnitSlot.U1] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[UnitSlot.U2]) == false && kv.Value[UnitSlot.U2] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[UnitSlot.U3]) == false && kv.Value[UnitSlot.U3] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[UnitSlot.U4]) == false && kv.Value[UnitSlot.U4] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[UnitSlot.U5]) == false && kv.Value[UnitSlot.U5] == runeId)
			{
				return kv.Key;
			}
		}

		return string.Empty;

	}
	
	public string getCharacterSelectedSkill(string runeId)
	{
		foreach(KeyValuePair<string, Dictionary<string, string>> kv in selectSkillRunes)
		{
			if( string.IsNullOrEmpty(kv.Value[SkillSlot.S1]) == false && kv.Value[SkillSlot.S1] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[SkillSlot.S2]) == false && kv.Value[SkillSlot.S2] == runeId)
			{
				return kv.Key;
			}
			else if( string.IsNullOrEmpty(kv.Value[SkillSlot.S3]) == false && kv.Value[SkillSlot.S3] == runeId)
			{
				return kv.Key;
			}
		}
		
		return string.Empty;
	}
	











	public void clearPlayData()
	{
		playData[UnitSlot.U1] = 0;
		playData[UnitSlot.U2] = 0;
		playData[UnitSlot.U3] = 0;
		playData[UnitSlot.U4] = 0;
		playData[UnitSlot.U5] = 0;

		playData[SkillSlot.S1] = 0;
		playData[SkillSlot.S2] = 0;
		playData[SkillSlot.S3] = 0;

		playData[PERIOD] = 0;

		playSubData[UnitSlot.U1] = 0;
		playSubData[UnitSlot.U2] = 0;
		playSubData[UnitSlot.U3] = 0;
		playSubData[UnitSlot.U4] = 0;
		playSubData[UnitSlot.U5] = 0;
		
		playSubData[SkillSlot.S1] = 0;
		playSubData[SkillSlot.S2] = 0;
		playSubData[SkillSlot.S3] = 0;
		
		playSubData[PERIOD] = 0;
	}



	public static string getPVPAI(bool isPlayerSide)
	{
		int grade = 1;

		if(isPlayerSide)
		{
			grade = GameDataManager.instance.champLeague;
		}
		else
		{
			grade = UIPlay.pvpleagueGrade;
		}

		switch(grade)
		{
		case 1:
			return "AI_PVP1";
			break;
		case 2:
			return "AI_PVP2";
			break;
		case 3:
			return "AI_PVP3";
			break;
		case 4:
			return "AI_PVP4";
			break;
		case 5:
			return "AI_PVP5";
			break;
		case 6:
			return "AI_PVP6";
			break;
		default:
			return "AI_PVP1";
			break;
		}
	}




}
























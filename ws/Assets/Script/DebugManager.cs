using UnityEngine;
using System.Collections;
using System;
using epi;
using System.Collections.Generic;

public class DebugManager : MonoBehaviour {

	public static DebugManager instance;

	public static Xbool useTestRound = false;

	public string userId;

	public bool useDebug;
	public bool ignoreCoolTime = false;
	public bool runInBackground = true;

	public bool useTagMatchMode = false;

	public enum NetworkSimulationType
	{
		None, Timeout, Error, Disconnected
	}

	public NetworkSimulationType networkSimuationType = NetworkSimulationType.None;

	public bool skipBattlePreviewCam = false;

	public float rareAniSpeed = 0.2f;

	public Xbool isAutoPlay = false;

	public bool isAutoCombat = false;

	public ToC_INIT initData = null;

	// 기본 캐릭터 ==============================
	public string defaultHero = "LEO";

	public string equipHead = "";
	public string equipBody = "";
	public string equipWeapon = "";
	public string equipVehicle = "";
	
	public string debugStageId = "STG011";
	public string debugRoundId = "RND011";

	public string debugChallengeId = "HUNT";

	public string[] debugUnitId = new string[Player.SUMMON_SLOT_NUM];	
	public string[] debugSkillId = new string[Player.SKILL_SLOT_NUM];

	public string ai;



	// PVP용 적 캐릭터 정보 =================================
	public int pvpStartPoint = 2000;

	public float pvpSummonAIUpdateDelay = 0.8f;
	public bool pvpDrawAfterTimeOver = false;
	public float pvpDrawTime = 10.0f;

	public string pvpDefaultHero = "LEO";
	

	public string pvpEquipHead = "";
	public string pvpEquipBody = "";
	public string pvpEquipWeapon = "";
	public string pvpEquipVehicle = "";
	
	public string[] pvpDebugUnitId = new string[Player.SUMMON_SLOT_NUM];	
	public string[] pvpDebugSkillId = new string[Player.SKILL_SLOT_NUM];

	public string pvpAi;

	public GamePlayerData pvpPlayerData = null;




	//player tag =================================
	public string defaultHero2 = "LEO";
	
	public string equipHead2 = "";
	public string equipBody2 = "";
	public string equipWeapon2 = "";
	public string equipVehicle2 = "";

	public string[] debugUnitId2 = new string[Player.SUMMON_SLOT_NUM];	
	public string[] debugSkillId2 = new string[Player.SKILL_SLOT_NUM];
	
	public string ai2;

	public GamePlayerData playerData2 = null;

	//pvp tag =================================

	public string pvpDefaultHero2 = "LEO";

	public string pvpEquipHead2 = "";
	public string pvpEquipBody2 = "";
	public string pvpEquipWeapon2 = "";
	public string pvpEquipVehicle2 = "";
	
	public string[] pvpDebugUnitId2 = new string[Player.SUMMON_SLOT_NUM];	
	public string[] pvpDebugSkillId2 = new string[Player.SKILL_SLOT_NUM];
	
	public string pvpAi2;

	public GamePlayerData pvpPlayerData2 = null;

	//=================================

//#if UNITY_EDITOR			
	void Awake()
	{
		if(instance != null && instance != this) Destroy(instance);
		instance = this;

//		for(int i = 0; i < Player.SUMMON_SLOT_NUM)
//		{
//			debugUnitId[i] = debugUnitId[i] + "_0";
//			pvpDebugUnitId[i] = pvpDebugUnitId[i] + "_0";
//		}
//
//		for(int i = 0; i < Player.SKILL_SLOT_NUM)
//		{
//			debugSkillId[i] = debugSkillId[i] + "_0";
//			pvpDebugSkillId[i] = pvpDebugSkillId[i] + "_0";
//		}



#if UNITY_EDITOR
		if(runInBackground)
		{
			Application.runInBackground = runInBackground;
		}

		if(useDebug)
		{
			parseInitData();
		}
#else


		useDebug = false;
		ignoreCoolTime = false;
#endif

#if UNITY_EDITOR
		Player.SUMMON_DELAY = pvpSummonAIUpdateDelay;
#endif

		useTestRound = false;
	}
//#endif			


	void Start()
	{
#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			ignoreCoolTime = true;
		}
#endif
	}

	void parseInitData()
	{
		string initText = (Resources.Load(ClientDataLoader.CLIENT_DATA_PATH + "debug/data_debuginit") as TextAsset).ToString();

#if UNITY_EDITOR
		Debug.Log(initText);
#endif
		
		string[] parseText = initText.Split('|');
		if(parseText.Length == 2)
		{
			BaseJsonObj jObj = null;
			string jStr = parseText[1];
			jObj = JsonFx.Json.JsonReader.Deserialize<BaseJsonObj>(jStr);
			jObj.cmd = parseText[0];
			
#if UNITY_IPHONE
			iphoneJsonData ijd = new iphoneJsonData();
			ijd.jsonStr = jStr;
			initData = JsonFx.Json.JsonReader.Deserialize<ToC_INIT>(ijd.jsonStr);
#else
			Type type = Type.GetType(jObj.cmd);
			initData = (ToC_INIT)JsonFx.Json.JsonReader.Deserialize(jStr,type);
#endif		
		}
	}
	


	
	
	public void setDebugInitData()
	{
		int i = 0;

		GameDataManager.instance.parseInitPacket(initData);

		if(GameDataManager.instance.heroes.ContainsKey(Character.KILEY) == false) GameDataManager.instance.heroes.Add(Character.KILEY, new GamePlayerData(Character.KILEY));
		if(GameDataManager.instance.heroes.ContainsKey(Character.LEO) == false) GameDataManager.instance.heroes.Add(Character.LEO, new GamePlayerData(Character.LEO));
		if(GameDataManager.instance.heroes.ContainsKey(Character.CHLOE) == false) GameDataManager.instance.heroes.Add(Character.CHLOE, new GamePlayerData(Character.CHLOE));

		GameDataManager.instance.heroes[Character.KILEY] = setPlayerData(GameDataManager.instance.heroes[Character.KILEY], true, Character.KILEY, 
		                                                                 "KILEY_HD1_11_0_0","KILEY_BD1_11_0_0","KILEY_WP1_11_0_0","KILEY_RD1_11_0_0", 
		                                                                 debugUnitId, debugSkillId, ai);
		GameDataManager.instance.heroes[Character.LEO] = setPlayerData(GameDataManager.instance.heroes[Character.LEO], true, Character.LEO, 
		                                                               "LEO_HD1_11_0_0","LEO_BD1_11_0_0","LEO_WP1_11_0_0","LEO_RD1_11_0_0", 
		                                                               debugUnitId, debugSkillId, ai);

		GameDataManager.instance.heroes[Character.CHLOE] = setPlayerData(GameDataManager.instance.heroes[Character.CHLOE], true, Character.CHLOE, 
		                                                                 "CHLOE_HD4_11_0_0","CHLOE_BD4_11_0_0","CHLOE_WP4_11_0_0","CHLOE_RD4_11_0_0", 
		                                                                 debugUnitId, debugSkillId, ai, "pc_chloe_face01");


		GameDataManager.instance.selectSubHeroId = defaultHero2;

		GameDataManager.instance.heroes[defaultHero] = setPlayerData(GameDataManager.instance.heroes[defaultHero], true, defaultHero, equipHead,equipBody,equipWeapon,equipVehicle,debugUnitId,debugSkillId,ai);
		pvpPlayerData = setPlayerData(pvpPlayerData, false, pvpDefaultHero,pvpEquipHead,pvpEquipBody,pvpEquipWeapon,pvpEquipVehicle,pvpDebugUnitId,pvpDebugSkillId,pvpAi);

		setDebugTagPVPData();

		GameDataManager.instance.unitInventory.Clear();
		GameDataManager.instance.unitInventoryList.Clear();
		GameDataManager.instance.skillInventory.Clear();
	}

	public void setDebugPVPData()
	{
		pvpPlayerData = setPlayerData(pvpPlayerData, false, pvpDefaultHero,pvpEquipHead,pvpEquipBody,pvpEquipWeapon,pvpEquipVehicle,pvpDebugUnitId,pvpDebugSkillId,pvpAi);
	}


	public void setDebugTagPVPData()
	{
		if(useTagMatchMode == false)
		{
			return;
		}
#if UNITY_EDITOR
		playerData2 = new GamePlayerData(defaultHero2);
		playerData2 = setPlayerData(playerData2, true, true, defaultHero2,equipHead2,equipBody2,equipWeapon2,equipVehicle2,debugUnitId2,debugSkillId2,ai2);

		pvpPlayerData2 = new GamePlayerData(pvpDefaultHero2);
		pvpPlayerData2 = setPlayerData(pvpPlayerData2, false, true, pvpDefaultHero2,pvpEquipHead2,pvpEquipBody2,pvpEquipWeapon2,pvpEquipVehicle2,pvpDebugUnitId2,pvpDebugSkillId2,pvpAi2);

#else

		SetupData sd = GameManager.info.setupData;
		playerData2 = new GamePlayerData(sd.tagTest1Hero);
		playerData2 = setPlayerData(playerData2, true, true, sd.tagTest1Hero,sd.tagTest1Equips[0],sd.tagTest1Equips[1],sd.tagTest1Equips[2],sd.tagTest1Equips[3],sd.tagTest1Unit,sd.tagTest1Skill,ai2);
		
		
		pvpPlayerData2 = new GamePlayerData(sd.tagTest2Hero);
		pvpPlayerData2 = setPlayerData(pvpPlayerData2, false, true, sd.tagTest2Hero,sd.tagTest2Equips[0],sd.tagTest2Equips[1],sd.tagTest2Equips[2],sd.tagTest2Equips[3],sd.tagTest2Unit,sd.tagTest2Skill,pvpAi2);


#endif
	}



	public GamePlayerData setPlayerData(GamePlayerData playerData, bool isPlayerSide, bool isSecondPlayer, string characterId, string hd, string bd, string wp, string rd, string[] units = null, string[] skills = null, string ai = "", string faceTexture = null)
	{
		if(playerData == null)
		{
			playerData = new GamePlayerData(characterId);
		}
		else
		{
			playerData.id = characterId;
		}
		
		playerData.level = 1;
		
		string[] tstr = new string[2];
		
		rd = rd.Trim().Replace("\"","");
		hd = hd.Trim().Replace("\"","");
		wp = wp.Trim().Replace("\"","");
		bd = bd.Trim().Replace("\"","");
		
		playerData.partsHead = new HeroPartsItem(characterId,hd);
		playerData.partsWeapon = new HeroPartsItem(characterId,wp);
		playerData.partsBody = new HeroPartsItem(characterId,bd);
		
		if(string.IsNullOrEmpty(rd)) playerData.partsVehicle = null;
		else playerData.partsVehicle = new HeroPartsItem(characterId,rd);
		
		if(string.IsNullOrEmpty(faceTexture))
		{
			if(characterId == Character.CHLOE)
			{
				playerData.faceTexture = "pc_chloe_face01";
			}
			else
			{
				playerData.faceTexture = null;
			}
			
		}
		else
		{
			playerData.faceTexture = faceTexture;
		}
		
		_tempStringList.Clear();
		
		int i = 0;
		
		if(units != null)
		{
			for(i = 0; i < units.Length; ++i)
			{
				if(string.IsNullOrEmpty(units[i]) == false) _tempStringList.Add(units[i].Trim());
				else if(isPlayerSide) _tempStringList.Add("");
			}
			
			playerData.units = new string[5]; 
			
			for(i = 0; i < 5; ++i)
			{
				if(_tempStringList.Count > i)
				{
					playerData.units[i] = _tempStringList[i];
				}
				else
				{
					playerData.units[i] = null;
				}
			}
			
			playerData.units = playerData.units;
			
		}
		
		if(skills != null)
		{
			_tempStringList.Clear();
			for(i = 0; i < skills.Length; ++i)
			{
				if(string.IsNullOrEmpty(skills[i]) == false) _tempStringList.Add(skills[i].Trim());
				else if(isPlayerSide) _tempStringList.Add("");
			}
			
			playerData.skills = new string[3];
			
			for(i = 0; i < 3  ; ++i)
			{
				if(_tempStringList.Count > i)
				{
					playerData.skills[i] = _tempStringList[i];
				}
				else
				{
					playerData.skills[i] = null;
				}
			}
			
			playerData.skills = playerData.skills;
			
		}
		
		
		if(isPlayerSide)
		{
			if(isSecondPlayer == false)
			{
				for(i = 0; i < 5; ++i)
				{
					if(playerData.units != null && i < playerData.units.Length)
					{
						GameDataManager.instance.playerUnitSlots[characterId][i].setData(playerData.units[i]);
					}
					else
					{
						GameDataManager.instance.playerUnitSlots[characterId][i].setData(null);
					}
				}
				
				for(i = 0; i < 3; ++i)
				{
					if(playerData.skills != null && i < playerData.skills.Length)
					{
						GameDataManager.instance.playerSkillSlots[characterId][i].setData(playerData.skills[i]);
					}
					else
					{
						GameDataManager.instance.playerSkillSlots[characterId][i].setData(null);
					}
				}
			}
			else
			{
				for(i = 0; i < 5; ++i)
				{
					if(playerData.units != null && i < playerData.units.Length)
					{
						GameDataManager.instance.playerUnitSlots[characterId][i].setData(playerData.units[i]);
					}
					else
					{
						GameDataManager.instance.playerUnitSlots[characterId][i].setData(null);
					}
				}
				
				for(i = 0; i < 3; ++i)
				{
					if(playerData.skills != null && i < playerData.skills.Length)
					{
						GameDataManager.instance.playerSkillSlots[characterId][i].setData(playerData.skills[i]);
					}
					else
					{
						GameDataManager.instance.playerSkillSlots[characterId][i].setData(null);
					}
				}
			}
		}
		
		playerData.ai = ai.Replace("\"","");
		
		return playerData;

	}



	public GamePlayerData setPlayerData(bool isPlayerSide, P_Hero heroData, Dictionary<string, string> units, Dictionary<string, string> skills )
	{
		if(heroData == null) return null;

		string[] u = new string[5];
		u[0] = units[UnitSlot.U1];
		u[1] = units[UnitSlot.U2];
		u[2] = units[UnitSlot.U3];
		u[3] = units[UnitSlot.U4];
		u[4] = units[UnitSlot.U5];
		
		string[] s = new string[3];
		s[0] = skills[SkillSlot.S1];
		s[1] = skills[SkillSlot.S2];
		s[2] = skills[SkillSlot.S3];
		
		GamePlayerData gpd = new GamePlayerData(heroData.name);

		DebugManager.instance.setPlayerData(gpd,isPlayerSide,heroData.name, 
		                                    heroData.selEqts[HeroParts.HEAD],
		                                    heroData.selEqts[HeroParts.BODY],
		                                    heroData.selEqts[HeroParts.WEAPON],
		                                    heroData.selEqts[HeroParts.VEHICLE],
		                                    u,s,DebugManager.instance.pvpAi);

		return gpd;
	}



	public GamePlayerData setPlayerData(GamePlayerData playerData, bool isPlayerSide, string characterId, string hd, string bd, string wp, string rd, string[] units = null, string[] skills = null, string ai = "", string faceTexture = null)
	{
		return setPlayerData(playerData, isPlayerSide, false, characterId, hd, bd, wp, rd, units, skills, ai, faceTexture);
	}


	List<string> _tempStringList = new List<string>();
	
	public void autoLogin()
	{
		string[] fArr = new string[PandoraManager.instance.appFriendDic.Count];
		PandoraManager.instance.appFriendDic.Keys.CopyTo(fArr,0);

		GameManager.me.clientDataLoader.init(onCompleteDebugAutoLogin);
	}



	void onCompleteDebugAutoLogin()
	{

		setDebugInitData();
		GameManager.me.onCompleteInitKakao();
	}



	void OnDestroy()
	{
		instance = null;
	}






	//===============================//

	public void startAutoCombatMode()
	{
#if UNITY_EDITOR
		if(DebugManager.instance.useDebug == false && DebugManager.instance.isAutoPlay && DebugManager.instance.isAutoCombat && BattleSimulator.nowSimulation == false)
		{
			StartCoroutine(updateAutoCombatMode());
		}
		else if(DebugManager.instance.useDebug && GameManager.me.cutSceneManager.isCutSceneRecordMode)
		{
			GameManager.me.cutSceneManager.autoCutScenePlayer();
		}
#endif
	}


	WaitForSeconds ws1 = new WaitForSeconds(2.0f);
	IEnumerator updateAutoCombatMode()
	{
		yield return null;
		bool fromCombat = false;

		while(true)
		{
			if(GameManager.me.isPlaying)
			{
				yield return ws1;
				if(GameManager.me.stageManager.playTime > 1.0f && GameManager.me.player != null && Time.timeScale < 2.0f)
				{
					GameManager.setTimeScale = 3.0f;
				}
			}
			else
			{
				yield return ws1;

//				if(GameManager.me.uiManager.popupRetry.gameObject.activeInHierarchy)
//				{
//					GameManager.me.uiManager.popupRetry.gameObject.SetActive(false);
//					EpiServer.instance.sendRetryPlayRound(WSDefine.YES);
//				}
//				else 
				if(GameManager.me.uiManager.uiLoading.gameObject.activeInHierarchy)
				{

				}
				else if(UISystemPopup.instance.popupAdvice.gameObject.activeInHierarchy)
				{
					UISystemPopup.instance.popupAdvice.hide();
				}
				else if(UISystemPopup.instance.popupLevelup.gameObject.activeInHierarchy)
				{
					UISystemPopup.instance.popupLevelup.hide();
				}
				else if(UISystemPopup.instance.popupDefault.gameObject.activeInHierarchy)
				{
					UISystemPopup.instance.popupDefault.hide();
				}
				else if(GameManager.me.uiManager.popupRoundClear.gameObject.activeInHierarchy)
				{
					GameManager.me.uiManager.popupRoundClear.hide();
					//UISystemPopup.checkLevelupPopupWithCallback(GameManager.me.returnToSelectScene);
					GameManager.me.returnToSelectScene();
				}
				else if(GameManager.me.uiManager.uiMenu.uiLobby.gameObject.activeInHierarchy)
				{
					fromCombat = false;
					GameManager.me.uiManager.uiMenu.uiLobby.onClickStart(null);
				}
				else if(GameManager.me.uiManager.uiMenu.uiWorldMap.gameObject.activeInHierarchy)
				{
					if(GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.activeInHierarchy)
					{
						if(fromCombat)
						{
							GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.gameObject.SetActive(false);
						}
						else
						{
							fromCombat = true;
							GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.onStartRound(null);
						}
					}
					else if(fromCombat == false)
					{
						GameManager.me.uiManager.uiMenu.uiWorldMap.onStartGame(null);
					}
					else
					{
						GameManager.me.uiManager.uiMenu.uiWorldMap.onClickBackToMainMenu(null);
					}
				}
				else if(CutSceneManager.nowOpenCutScene && GameManager.me.cutSceneManager.btnCutSceneSkip.gameObject.activeSelf && GameManager.me.cutSceneManager.isSkipMode == false && CutSceneManager.cutScenePlayTime > 1.0f)
				{
					GameManager.me.cutSceneManager.onSkipCutScene(null);
				}


			}
		}
	}



	public void loadDebugUnit(string units, int type)
	{
		units = units.Trim();
		string[] lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);
		
		if(lines[0].StartsWith("SK")) return;
		
		for(int i = 0; i < lines.Length ; ++i)
		{
			if(string.IsNullOrEmpty( lines[i] ) == false && lines[i].StartsWith("UN") && lines[i].Length < 6)
			{
				lines[i] += "02001";
			}
		}

		if(type == 0) debugUnitId = lines;
		else if(type == 1) debugUnitId2 = lines;
		else if(type == 2) pvpDebugUnitId = lines;
		else if(type == 3) pvpDebugUnitId2 = lines;
		
		
		Debug.LogError(lines.Length + " 입력완료");
	}


	public void loadDebugSkill(string units, int type)
	{
		units = units.Trim();
		string[] lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);
		
		for(int i = 0; i < lines.Length; ++i)
		{
			lines[i] += "_20";
			if(lines[i].StartsWith("UN") || lines[i].StartsWith("EU")) return;
		}

		if(type == 0) debugSkillId = lines;
		else if(type == 1) debugSkillId2 = lines;
		else if(type == 2) pvpDebugSkillId = lines;
		else if(type == 3) pvpDebugSkillId2 = lines;
	}


	public void loadDebugParts(string units, int type)
	{
		units = units.Trim();
		string[] lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);

		switch(type)
		{
		case 0:
			equipHead = lines[0];
			equipBody = lines[1];
			equipWeapon = lines[2];
			equipVehicle = lines[3];
			break;

		case 1:
			equipHead2 = lines[0];
			equipBody2 = lines[1];
			equipWeapon2 = lines[2];
			equipVehicle2 = lines[3];
			break;

		case 2:
			pvpEquipHead = lines[0];
			pvpEquipBody = lines[1];
			pvpEquipWeapon = lines[2];
			pvpEquipVehicle = lines[3];
			break;

		case 3:
			pvpEquipHead2 = lines[0];
			pvpEquipBody2 = lines[1];
			pvpEquipWeapon2 = lines[2];
			pvpEquipVehicle2 = lines[3];
			break;
		}
	}


}

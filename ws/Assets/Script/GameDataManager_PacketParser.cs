using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class GameDataManager : MonoBehaviour 
{

	/////////////////////////////////////////////////////////////////////////////////////////////		
	//
	// GAME_DATA!!!!!	
	//	
	/////////////////////////////////////////////////////////////////////////////////////////////	
	
	//== BASE PACKET START//
	public Dictionary<string, string> C_PlayerDatas;
	public Dictionary<string, P_Hero> C_HeroDatas;
	public Dictionary<string, int> C_Equipments;
	public Dictionary<string, int> C_UnitRunes;
	public Dictionary<string, int> C_SkillRunes;
	
	//== PACKET END//
	
	//public string echo;

	//== INIT PACKET START//
	public Xint maxAct = 1;
	public Xint maxStage = 1;
	public Xint maxRound = 1;
	//public Dictionary<string, P_FriendData> friendDatas;
	//public int totalPetSlot;
	public float lastTimeOfShopData;

	public int maxActWithCheckingMaxAct
	{
		get
		{
			if(maxAct > GameManager.MAX_ACT)
			{
				return GameManager.MAX_ACT;
			}

			return maxAct;
		}
	}

	public int currentStageWithCheckingMaxAct
	{
		get
		{
			if(maxAct > GameManager.MAX_ACT)
			{
				return 5;
			}
			
			return maxStage;
		}
	}


//	public P_Popup[] notices;
	public Dictionary<string, string> initOptions;	

	public Dictionary<string, P_Message> messages = new Dictionary<string, P_Message>();

	public int lastPlayStatus = WSDefine.LAST_PLAY_STATUS_NONE;



	public Dictionary<string, P_Package> packages = new Dictionary<string, P_Package>();



	public P_Attend attendData;

	public Dictionary<string, P_Annuity> annuityProducts = null;


	public void parseInitPacket(ToC_INIT p)
	{
		if(p == null) return;

		parseBasePacket(p);

		checkNewMsgNum();

		name = p.nickname;

		energy = p.energy;

		maxEnergies = p.maxEnergies;

		chargeTime = p.chargeTime;

		friendPoint = p.friendPoint;

		gold = p.gold;
		ruby = p.ruby;

		runeStone = p.rstone;

		messageBlock = p.blockMessage;

		selectHeroId = p.selectHeroId;
		selectSubHeroId = p.selectSubHeroId;

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			selectHeroId = p.selectHeroId;
			selectSubHeroId = null;
		}
#endif

		if(p.heroes != null)
		{
			parseHeroInven(p.heroes);
		}

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);
		
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);

		parsePartsInven(p.equipments);
		

		parseUnitInven(p.unitRunes);


		parseSkillInven(p.skillRunes);
		


		maxAct = p.maxAct;
#if UNITY_EDITOR
		//maxAct = 6;
#endif
		maxStage = p.maxStage;
		maxRound = p.maxRound;


		champLeague = p.champLeague; 	// 1:BRONZE, 2:SILVER, 3:GOLD, 4:MASTER, 5:PLATINUM, 6:LEGEND
		champGroupId = p.champGroupId;
		champMemberId = p.champMemberId;
		inviteCount = p.inviteCount;

		completeTutorial = p.completeTutorial;

		if(p.tutorialHistory == null) tutorialHistory.Clear();
		else tutorialHistory = p.tutorialHistory;

		lastPlayStatus = p.lastPlayStatus;

		goldForRuby = p.goldForRuby;

		if(string.IsNullOrEmpty(p.hellSave) == false)
		{
			lastHellWaveInfo = p.hellSave.Split(',');
//			Debug.LogError("lastHellWaveInfo : " + p.hellSave);
		}
		else
		{
			lastHellWaveInfo = null;
		}


		switch(p.serviceMode)
		{
		case 0: serviceMode = ServiceMode.Normal; break;
		case 1: serviceMode = ServiceMode.IOS_SUMMIT; break;
		case 2: serviceMode = ServiceMode.CBT; break;
		}

		timeDiffBetweenServerAndClient = Util.getTimeDiffBetweenServerAndClient(p.timestamp);

		attendData = p.attendEvent;

		parseAnnuityProducts(p.annuityProducts, false);

		heroPrices = p.heroPrices;

	}


	public int timeDiffBetweenServerAndClient = -1;

	public void parseGetRecomPacket(ToC_GET_RECOM p)
	{

		equipReinforceData = p.eqtReinforce;
		skillReinforceData = p.skillReinforce;
		unitReinforceData = p.unitReinforce;
		
		composePrices = p.composePrices;

		evolvePrices = p.evolvePrices;

		reforgePrices = p.reforgePrices;

		GameManager.me.onCompleteGetReinforceData();

	}


	public string eventUrl = null;

	public bool receivedEventUrl = true;

	public void parseEventUrlPacket(ToC_EVENT_URL p)
	{
		eventUrl = p.url;

		receivedEventUrl = true;

		if(EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
		{
//			Debug.LogError("ToC_EVENT_URL : " + p.url);
		}

		GameManager.me.uiManager.uiMenu.uiLobby.btnEvent.gameObject.SetActive( string.IsNullOrEmpty(eventUrl) == false );
	}




	public void parseFinishSigongPacket(ToC_FINISH_SIGONG p)
	{
		parseBasePacket(p);

		energy = p.energy;

		chargeTime = p.chargeTime;

		gold = p.gold;

		ruby = p.ruby;

		runeStone = p.rstone;

		roundClearLevelUpInvenItems.Clear();
		
		if(GameManager.me.successType == WSDefine.GAME_SUCCESS)
		{
			switch(GameManager.me.battleManager.mainCharacterId)
			{
			case Character.LEO:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes, p.selectSkillRunes);
				break;
			case Character.KILEY:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes_Kiley, p.selectSkillRunes_Kiley);
				break;
			case Character.CHLOE:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes_Chloe, p.selectSkillRunes_Chloe);
				break;
			}

		}

		parseHeroInven(p.heroes);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);
		
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		
		parsePartsInven(p.equipments, true);
		
		parseUnitInven(p.unitRunes, true);
		
		parseSkillInven(p.skillRunes, true);

		parseSigongList(p.sigongList);

		roundItems = p.rewardItems;

		GameManager.me.uiManager.uiPlay.gameResultWord.callback = GameManager.me.onCompleteRecieveRoundResult;

		if( (CutSceneManager.nowOpenCutScene && CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.After) || GameManager.me.successType != WSDefine.GAME_SUCCESS)
		{
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType);
		}
		else
		{
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType, 2f, 0.25f);
		}

	}






	public int normalPlayNeedRuby = -1;
	public int normalCountUsingRuby = 0;



	public void parsePlayEpicPacket(ToC_PLAY_EPIC p, bool isTutorialMode = false)
	{
		parseBasePacket(p);
		energy = p.energy;
		chargeTime = p.chargeTime;

		if(isTutorialMode)
		{
			EpiServer.instance.sendCompleteRewardTutorial("T26");
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.hide();
			GameManager.me.uiManager.showLoading();
			GameManager.me.startGame(0.5f);
		}
	}
	

	public string stageClearRewardItem = null;
	public bool needWorldMapAnimation = false;

	public string[] roundItems = null;

	public List<RoundClearLevelupItemData> roundClearLevelUpInvenItems = new List<RoundClearLevelupItemData>();

	public void parseFinishEpicPacket(ToC_FINISH_EPIC p, bool isDeniedMode = false)
	{
		parseBasePacket(p);

		gold = p.gold;

		needWorldMapAnimation = (p.maxStage > maxStage || p.maxAct > maxAct) && (GameManager.me.stageManager.isRepeatGame == false);


		if(p.maxAct > maxAct)
		{
			AdPluginManager.reportGoogleRemarketing(new string[]{ "action_type", "act_clear", "value", maxAct.Get().ToString() });
		}


		maxAct = p.maxAct;
#if UNITY_EDITOR
		//maxAct = 6;
#endif
		maxStage = p.maxStage;
		maxRound = p.maxRound;

		roundClearLevelUpInvenItems.Clear();

		if(isDeniedMode == false && GameManager.me.successType == WSDefine.GAME_SUCCESS)
		{
			switch( GameManager.me.battleManager.mainCharacterId )
			{
			case Character.LEO:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes, p.selectSkillRunes);
				break;
			case Character.KILEY:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes_Kiley, p.selectSkillRunes_Kiley);
				break;
			case Character.CHLOE:
				parseRoundClearRewardData(GameManager.me.battleManager.mainCharacterId, p.heroes, p.selectUnitRunes_Chloe, p.selectSkillRunes_Chloe);
				break;
			}


		}

		parseHeroInven(p.heroes);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);

		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);


		stageClearRewardItem = p.stageItem;

		parsePartsInven(p.equipments, true);

		parseUnitInven(p.unitRunes, true);

		parseSkillInven(p.skillRunes, true);

		if(isDeniedMode) return;

		roundItems = p.roundItems;

		GameManager.me.uiManager.uiPlay.gameResultWord.callback = GameManager.me.onCompleteRecieveRoundResult;
		
		if( (CutSceneManager.nowOpenCutScene && CutSceneManager.nowOpenCutSceneType == CutSceneManager.CutSceneType.After) || GameManager.me.successType != WSDefine.GAME_SUCCESS)
		{
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType);
		}
		else
		{
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType, 2f, 0.25f);
		}
		
	}



	void parseRoundClearRewardData(string mainCharacter, Dictionary<string, P_Hero> heroes, Dictionary<string, string> unitRunes, Dictionary<string, string> skillRunes)
	{
		GameIDData g1 = new GameIDData();
		GameIDData g2 = new GameIDData();

		parseRoundClearRewardEquipData(mainCharacter, HeroParts.HEAD, g1, g2, heroes);
		parseRoundClearRewardEquipData(mainCharacter, HeroParts.BODY, g1, g2, heroes);
		parseRoundClearRewardEquipData(mainCharacter, HeroParts.VEHICLE, g1, g2, heroes);
		parseRoundClearRewardEquipData(mainCharacter, HeroParts.WEAPON, g1, g2, heroes);

		// skill
		parseRoundClearRewardSkillData(mainCharacter, SkillSlot.S1, g1, g2, skillRunes);
		parseRoundClearRewardSkillData(mainCharacter, SkillSlot.S2, g1, g2, skillRunes);
		parseRoundClearRewardSkillData(mainCharacter, SkillSlot.S3, g1, g2, skillRunes);
		// unit

		parseRoundClearRewardUnitData(mainCharacter, UnitSlot.U1, g1, g2, unitRunes);
		parseRoundClearRewardUnitData(mainCharacter, UnitSlot.U2, g1, g2, unitRunes);
		parseRoundClearRewardUnitData(mainCharacter, UnitSlot.U3, g1, g2, unitRunes);
		parseRoundClearRewardUnitData(mainCharacter, UnitSlot.U4, g1, g2, unitRunes);
		parseRoundClearRewardUnitData(mainCharacter, UnitSlot.U5, g1, g2, unitRunes);
	}

	private void parseRoundClearRewardEquipData(string mainCharacter, string partsName, GameIDData g1, GameIDData g2, Dictionary<string, P_Hero> heroes)
	{
		if(serverHeroData.ContainsKey(mainCharacter) == false || heroes.ContainsKey(mainCharacter) == false)
		{
			return;
		}

		g1.parse(serverHeroData[mainCharacter].selEqts[partsName], GameIDData.Type.Equip);
		
		if(g1.level < GameIDData.MAX_LEVEL)
		{
			g2.parse(heroes[mainCharacter].selEqts[partsName], GameIDData.Type.Equip);
			roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(g2.level-g1.level, g1.level, g2.getReinforceProgressPercent(), g1.getReinforceProgressPercent(), heroes[mainCharacter].selEqts[partsName] , g2.rare));
		}
	}



	private void parseRoundClearRewardSkillData(string mainCharacter, string slotIndex, GameIDData g1, GameIDData g2, Dictionary<string, string> skillRunes)
	{
		if(selectSkillRunes.ContainsKey(mainCharacter) == false)
		{
			return;
		}

		if(string.IsNullOrEmpty(selectSkillRunes[mainCharacter][slotIndex]) == false && string.IsNullOrEmpty(skillRunes[slotIndex]) == false)
		{
			g1.parse(selectSkillRunes[mainCharacter][slotIndex], GameIDData.Type.Skill);
			
			if(g1.level < GameIDData.MAX_LEVEL)
			{
				g2.parse(skillRunes[slotIndex], GameIDData.Type.Skill);
				roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(g2.level-g1.level, g1.level, g2.getReinforceProgressPercent(), g1.getReinforceProgressPercent(), skillRunes[slotIndex], g2.rare));
			}
		}

	}



	private void parseRoundClearRewardUnitData(string mainCharacter, string slotIndex, GameIDData g1, GameIDData g2, Dictionary<string, string> unitRunes)
	{
		if(selectUnitRunes.ContainsKey(mainCharacter) == false)
		{
			return;
		}

		if(string.IsNullOrEmpty(selectUnitRunes[mainCharacter][slotIndex]) == false && string.IsNullOrEmpty(unitRunes[slotIndex]) == false)
		{
			g1.parse(selectUnitRunes[mainCharacter][slotIndex] , GameIDData.Type.Unit);
			
			if(g1.level < GameIDData.MAX_LEVEL)
			{
				g2.parse(unitRunes[slotIndex] , GameIDData.Type.Unit);
				roundClearLevelUpInvenItems.Add(new RoundClearLevelupItemData(g2.level-g1.level, g1.level, g2.getReinforceProgressPercent(), g1.getReinforceProgressPercent(), unitRunes[slotIndex], g2.rare));
			}
		}

	}




	public void parsePlaySigongPacket(ToC_PLAY_SIGONG p)
	{
		parseBasePacket(p);

		energy = p.energy;
		chargeTime = p.chargeTime;
		gold = p.gold;
		ruby = p.ruby;
		runeStone = p.rstone;


		GameManager.me.uiManager.popupInstantDungeon.onCompleteReceivePlaySigong();
	}



	public void parsePrepareSigongPacket(ToC_PREPARE_SIGONG p)
	{
		parseBasePacket(p);
		parseSigongList(p.sigongList);

		GameManager.me.uiManager.popupInstantDungeon.onCompleteReceiveSigongData();
	}




	public Dictionary<string, DateTime> sigongTimer = new Dictionary<string, DateTime>();

	void parseSigongList(Dictionary<string, P_Sigong> receivedSigongList)
	{
		sigongList = receivedSigongList;

		if(sigongList == null)
		{
			sigongList = new Dictionary<string, P_Sigong>();
		}

		foreach(KeyValuePair<string, P_Sigong> kv in sigongList)
		{
			if(sigongTimer.ContainsKey(kv.Key) == false)
			{
				sigongTimer.Add(kv.Key, DateTime.Now);
			}
			else
			{
				sigongTimer[kv.Key] = DateTime.Now;
			}
		}
	}




	public void parsePrepareHellPacket(ToC_PREPARE_HELL p)
	{
		parseBasePacket(p);

		hellInfo = p;

		maxHellTicket = p.maxTicket;

		hellTicket = p.hellTicket;

		hellModeCheckTime = DateTime.Now;

		if(p.fRankData != null)
		{
			hellFriendRanks = p.fRankData;
		}
		else
		{
			hellFriendRanks.Clear();
		}
		
		GameManager.me.uiManager.popupHell.canRequestMoreFriendRank = (p.fRankData != null && p.fRankData.Count > 0);

		GameManager.me.uiManager.popupHell.refreshList(true);

		GameManager.me.uiManager.popupHell.refreshInfo();

		TutorialManager.instance.check("T51");

	}

	public Dictionary<string, P_FriendRank> hellFriendRanks = new Dictionary<string, P_FriendRank>();

	public void parseGetHellFriendRankPacket(ToC_GET_HELL_FRIEND_RANK p, int requestStartIndex)
	{
		parseBasePacket(p);

		if(requestStartIndex == 1)
		{
			if(p.fRankData != null)
			{
				hellFriendRanks = p.fRankData;
			}
		}
		else
		{
			if(p.fRankData != null)
			{
				foreach(KeyValuePair<string, P_FriendRank> kv in p.fRankData)
				{
					if(hellFriendRanks.ContainsKey(kv.Key) == false)
					{
						hellFriendRanks.Add(kv.Key, kv.Value);
					}
				}
			}
		}

		GameManager.me.uiManager.popupHell.canRequestMoreFriendRank = (p.fRankData != null && p.fRankData.Count > 0);
		//GameManager.me.uiManager.popupHell.canRequestMoreFriendRank = (p.fRankData.Count > 0 && p.fRankData.Count % UIPopupHell.ITEM_NUM_PER_PAGE == 0);

		if(GameManager.me.uiManager.popupHell.lastGetHellFriendRankRequestStartIndex == requestStartIndex)
		{
			GameManager.me.uiManager.popupHell.canRequestMoreFriendRank = false;
		}
		
		GameManager.me.uiManager.popupHell.lastGetHellFriendRankRequestStartIndex = requestStartIndex;


		GameManager.me.uiManager.popupHell.refreshList(requestStartIndex == 1);

	}

	public Dictionary<string, P_UserRank> hellUserRanks = new Dictionary<string, P_UserRank>();

	public int lastGetHellUserRankRequestStartIndex = 0;

	public void parseGetHellUserRankPacket(ToC_GET_HELL_USER_RANK p, int requestStartIndex)
	{
		parseBasePacket(p);

		if(requestStartIndex == 1)
		{
			if(p.uRankData != null) 
			{
				hellUserRanks = p.uRankData;
			}
		}
		else
		{
			if(p.uRankData != null)
			{
				foreach(KeyValuePair<string, P_UserRank> kv in p.uRankData)
				{
					if(hellUserRanks.ContainsKey(kv.Key) == false)
					{
						hellUserRanks.Add(kv.Key, kv.Value);
					}
				}
			}
		}

		GameManager.me.uiManager.popupHell.canRequestMoreUserRank = (p.uRankData != null && p.uRankData.Count > 0);

		if(GameManager.me.uiManager.popupHell.lastGetHellUserRankRequestStartIndex == requestStartIndex)
		{
			GameManager.me.uiManager.popupHell.canRequestMoreUserRank = false;
		}

		GameManager.me.uiManager.popupHell.lastGetHellUserRankRequestStartIndex = requestStartIndex;

//		GameManager.me.uiManager.popupHell.canRequestMoreUserRank = (p.uRankData.Count > 0 && p.uRankData.Count % UIPopupHell.ITEM_NUM_PER_PAGE == 0);

		GameManager.me.uiManager.popupHell.refreshList(requestStartIndex == 1);
		//public Dictionary<string, P_UserRank> uRankData;
	}

	public void parseGetHellUserInfoPacket(ToC_GET_HELL_USER_INFO p, P_UserRank userData)
	{
		parseBasePacket(p);
		GameManager.me.uiManager.popupFriendDetail.setData(p, userData);

//		public P_Hero hero;
//		public Dictionary<string, string> selectUnitRunes;
//		public Dictionary<string, string> selectSkillRunes;
	}





	public void parsePlayHellPacket(ToC_PLAY_HELL p)
	{
		parseBasePacket(p);
		hellTicket = p.hellTicket;

		GameManager.me.uiManager.popupHell.updateHellTicket();
		GameManager.me.uiManager.popupHell.startHell();

	}


	public void parseSaveHellPacket(ToC_SAVE_HELL p)
	{
		parseBasePacket(p);

//		Debug.LogError("parseSaveHellPacket");

		EpiServer.instance.waitForSaveWaveToServer = false;
		
	}


	public void parseFinishHellPacket(ToC_FINISH_HELL p, bool isDeniedContinueGame = false)
	{
		parseBasePacket(p);

		int rewardGold = p.gold - gold;

		gold = p.gold;

		parsePartsInven(p.equipments, true);
		parseUnitInven(p.unitRunes, true);
		parseSkillInven(p.skillRunes, true);

		if(hellInfo != null)
		{
			hellInfo.dScore = p.dScore;
			hellInfo.dWave = p.dWave;
			hellInfo.dMonster = p.dMonster;
			hellInfo.dDistance = p.dDistance;
			hellInfo.wRank = p.wRank;
			hellInfo.wScore = p.wScore;
			hellInfo.wWave = p.wWave;
			hellInfo.wMonster = p.wMonster;
			hellInfo.wDistance = p.wDistance;
			hellInfo.wGroup = p.wGroup;
			hellInfo.tRank = p.tRank;
			hellInfo.tScore = p.tScore;
			hellInfo.tWave = p.tWave;
			hellInfo.tMonster = p.tMonster;
			hellInfo.tDistance = p.tDistance;

			hellInfo.fRank = p.fRank;
		}

		if(p.fRankData != null)
		{
			hellFriendRanks = p.fRankData;
		}
		else
		{
			hellFriendRanks.Clear();
		}

		GameManager.me.uiManager.popupHell.canRequestMoreFriendRank = (p.fRankData != null && p.fRankData.Count > 0);

		if(isDeniedContinueGame) return;

		GameManager.me.uiManager.popupHellResult.rewardGold = rewardGold;
		GameManager.me.uiManager.popupHellResult.rewardItems = p.rewardItems;

		GameManager.me.uiManager.uiPlay.gameResultWord.callback = GameManager.me.uiManager.stageClearEffectManager.play;  //GameManager.me.uiManager.popupHellResult.showHellResult;
		GameManager.me.uiManager.uiPlay.gameResultWord.init(Result.Type.Finish, GameManager.me.stageManager.nowPlayingGameType, 2, 0.25f);

	}





	/*
	public void parseRetryPlayRoundPacket(ToC_RETRY_PLAY_EPIC p, bool isTutorialRetry)
	{
		parseBasePacket(p);
		ruby = p.ruby;
		maxRound = p.maxRound;



		string stageId = null;
		if(DebugManager.instance.useDebug)
		{
			stageId = DebugManager.instance.debugStageId;
		}
		else
		{
			ActData ad = GameManager.info.actData[GameDataManager.instance.maxAct];
			stageId = ad.stages[GameDataManager.instance.maxStage-1];
		}
		
		if(stageId != null)
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.open(stageId);
		}

		if(isTutorialRetry) EpiServer.instance.sendCompleteTutorial("T11");
	}
	*/


	public void parseInvalidateGame(ToC_INVALIDATE_GAME p)
	{
		parseBasePacket(p);
		//gameMode; // 0:EPIC, 1:CHALLENGE, 2:CHAMPIONSHIP
		
	}
	
	public void parseChangeHeroPacket(ToC_CHANGE_HERO p)
	{
		parseBasePacket(p);
		selectHeroId = p.heroId ;
		selectSubHeroId = p.subHeroId;
		//Debug.LogError("isMain??? : " + p.isMain);

		GameManager.me.uiManager.popupHeroInfo.onCompleteChangeHero();
		//	public string heroId;
		
	}
	
	public void parseChangeEquipmentPacket(ToC_CHANGE_EQUIPMENT p)
	{
		parseBasePacket(p);

		//		public string heroId;
		//		public string type;
		//		public string equipmentId;

		parseHeroInven(p.heroes);
		//parsePartsInven(p.equipments);

		GameManager.me.uiManager.uiMenu.uiHero.refreshList();
	}
	
	public void parseSellEquipmentPacket(ToC_SELL_EQUIPMENT p)
	{
		parseBasePacket(p);
//		parseHeroInven(p.heroes);
		parsePartsInven(p.equipments);
		

		int prevGold = gold;
		gold = p.gold;

		UIRewardNoticePanel.addGold(gold-prevGold);

		GameManager.me.uiManager.uiMenu.uiHero.refreshList();
	}
	

	
	public void parseReinforceEquipRunePacket(ToC_REINFORCE_EQUIPMENT p, string[] reinforceSourceIds)
	{
		parseBasePacket(p);
		
		parseHeroInven(p.heroes);
		parsePartsInven(p.equipments);
		
		
		gold = p.gold;
		ruby = p.ruby;
		
		RuneStudioMain.instance.playReinforceResult(p.newEquipmentId, reinforceSourceIds, GameIDData.Type.Equip);
	}



	public void parseReinforceUnitRunePacket(ToC_REINFORCE_UNITRUNE p, string[] reinforceSourceIds)
	{
		parseBasePacket(p);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);


		parseUnitInven(p.unitRunes);
		
		gold = p.gold;
		ruby = p.ruby;

		RuneStudioMain.instance.playReinforceResult(p.newUnitRuneId, reinforceSourceIds, GameIDData.Type.Unit);
	}


	public void parseReinforceSkillRunePacket(ToC_REINFORCE_SKILLRUNE p, string[] reinforceSourceIds)
	{
		parseBasePacket(p);

		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		parseSkillInven(p.skillRunes);
		
		gold = p.gold;
		ruby = p.ruby;
		
		RuneStudioMain.instance.playReinforceResult(p.newSkillRuneId, reinforceSourceIds, GameIDData.Type.Skill);

	}




	public void parseComposeEquipmentPacket(ToC_COMPOSE_EQUIPMENT p, string lastComposeEquipmentId, string sourceId)
	{
		parseBasePacket(p);

		parseHeroInven(p.heroes);
		parsePartsInven(p.equipments);

		RuneStudioMain.instance.playComposeResult(p.newEquipmentId, new string[]{lastComposeEquipmentId,sourceId}, GameIDData.Type.Equip);

		gold = p.gold;
		ruby = p.ruby;
	}



	public void parseComposeSkillRunePacket(ToC_COMPOSE_SKILLRUNE p, string lastComposeSkillId, string sourceId)
	{
		parseBasePacket(p);
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		parseSkillInven(p.skillRunes);
		
		RuneStudioMain.instance.playComposeResult(p.newSkillRuneId, new string[]{lastComposeSkillId,sourceId}, GameIDData.Type.Skill);

		gold = p.gold;
		ruby = p.ruby;
	}


	public void parseComposeUnitRunePacket(ToC_COMPOSE_UNITRUNE p, string lastComposeUnitId, string sourceId)
	{
		parseBasePacket(p);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);


		parseUnitInven(p.unitRunes);
		gold = p.gold;
		ruby = p.ruby;

		RuneStudioMain.instance.playComposeResult(p.newUnitRuneId, new string[]{lastComposeUnitId,sourceId}, GameIDData.Type.Unit);

	}

















	public void parseEvolveEquipmentPacket(ToC_EVOLVE_EQUIPMENT p, string lastEvolveEquipmentId)
	{
		parseBasePacket(p);
		
		parseHeroInven(p.heroes);
		parsePartsInven(p.equipments);

		gold = p.gold;
		ruby = p.ruby;
		runeStone = p.rstone;

		GameManager.me.uiManager.popupEvolution.hide();
		GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();

		RuneStudioMain.instance.playEvolveResult(p.newEquipmentId, lastEvolveEquipmentId, GameIDData.Type.Equip);
	}
	
	
	
	public void parseEvolveSkillRunePacket(ToC_EVOLVE_SKILLRUNE p, string lastEvolveSkillId)
	{
		parseBasePacket(p);
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		parseSkillInven(p.skillRunes);
		
		gold = p.gold;
		ruby = p.ruby;
		runeStone = p.rstone;

		GameManager.me.uiManager.popupEvolution.hide();
		GameManager.me.uiManager.popupSkillPreview.isEnabled = false;

		RuneStudioMain.instance.playEvolveResult(p.newSkillRuneId, lastEvolveSkillId, GameIDData.Type.Skill);
	}
	
	
	public void parseEvolveUnitRunePacket(ToC_EVOLVE_UNITRUNE p, string lastEvolveUnitId)
	{
		parseBasePacket(p);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);

		parseUnitInven(p.unitRunes);

		gold = p.gold;
		ruby = p.ruby;
		runeStone = p.rstone;

		GameManager.me.uiManager.popupEvolution.hide();
		GameManager.me.uiManager.popupSummonDetail.hide();
		
		RuneStudioMain.instance.playEvolveResult(p.newUnitRuneId, lastEvolveUnitId, GameIDData.Type.Unit);
		
	}




	public void parseReforgeRunePacket(ToC_REFORGE_RUNE p, string lastReforgeId, GameIDData.Type lastReforgeType)
	{
		parseBasePacket(p);

		gold = p.gold;
		ruby = p.ruby;

		switch(lastReforgeType)
		{
		case GameIDData.Type.Unit:

			parseSelectUnits(Character.LEO, p.selectRunes); 
			parseSelectUnits(Character.KILEY, p.selectRunes_Kiley); 
			parseSelectUnits(Character.CHLOE, p.selectRunes_Chloe);

			parseUnitInven(p.runes);
			break;
		case GameIDData.Type.Skill:

			parseSelectSkills(Character.LEO,p.selectRunes);
			parseSelectSkills(Character.KILEY,p.selectRunes_Kiley);
			parseSelectSkills(Character.CHLOE,p.selectRunes_Chloe);

			parseSkillInven(p.runes);
			break;
		case GameIDData.Type.Equip:
			parseHeroInven(p.heroes);
			parsePartsInven(p.runes);
			break;
		}

		GameManager.me.uiManager.popupReforege.hide(false);

		RuneStudioMain.instance.playTranscendResult(p.newRuneId, lastReforgeId, lastReforgeType);
	}














	public void parseChangeSkillPacket(ToC_CHANGE_SKILLRUNE p)
	{
		parseBasePacket(p);

		parseSkillInven(p.skillRunes);
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		GameManager.me.uiManager.uiMenu.uiSkill.onCompleteChangeSkill();
	}	
	
	
	public void parseUnloadSkillPacket(ToC_UNLOAD_SKILLRUNE p)
	{
		parseBasePacket(p);
		parseSkillInven(p.skillRunes);
		parseSelectSkills(Character.LEO,p.selectSkillRunes);
		parseSelectSkills(Character.KILEY,p.selectSkillRunes_Kiley);
		parseSelectSkills(Character.CHLOE,p.selectSkillRunes_Chloe);
		GameManager.me.uiManager.uiMenu.uiSkill.onCompleteChangeSkill();
	}	

	
	// unit
	
	public void parseChangeUnitPacket(ToC_CHANGE_UNITRUNE p)
	{
		parseBasePacket(p);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);

		parseUnitInven(p.unitRunes);
		GameManager.me.uiManager.uiMenu.uiSummon.onCompleteChangeUnit();
	}		
	
	
	public void parseUnloadUnitPacket(ToC_UNLOAD_UNITRUNE p)
	{
		parseBasePacket(p);

		parseUnitInven(p.unitRunes);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectUnits(Character.KILEY, p.selectUnitRunes_Kiley); 
		parseSelectUnits(Character.CHLOE, p.selectUnitRunes_Chloe);

		GameManager.me.uiManager.uiMenu.uiSummon.onCompleteChangeUnit();
	}	


	public void parseSellUnitRune(ToC_SELL_UNITRUNE p)
	{
		parseBasePacket(p);

		int reward = p.gold - gold;

		gold = p.gold;

		parseUnitInven(p.unitRunes);

		UIRewardNoticePanel.addGold(reward);

		GameManager.me.uiManager.uiMenu.uiSummon.refreshUnitInven();
	}



	public void parseSellSkillRune(ToC_SELL_SKILLRUNE p)
	{
		parseBasePacket(p);

		int reward = p.gold - gold;
		gold = p.gold;

		parseSkillInven(p.skillRunes);

		UIRewardNoticePanel.addGold(reward);

		GameManager.me.uiManager.uiMenu.uiSkill.refreshMySkills();
		GameManager.me.uiManager.uiMenu.uiSkill.refreshSkillInven();
	}



	
	
	public void parseUnitRuneHistoryPacket(ToC_UNITRUNE_HISTORY p)
	{
		parseBasePacket(p);
		historyUnitRunes = p.historyUnitRunes;
		GameManager.me.uiManager.popupRuneBook.onCompleteReceiveUnitRuneHistory();
	}
	

	
	public void parseChangeEquipPacket(ToC_CHANGE_EQUIPMENT p)
	{
		parseBasePacket(p);

		parseHeroInven(p.heroes);
		parsePartsInven(p.equipments);
		

		GameManager.me.uiManager.uiMenu.uiHero.onCompleteChangeEquip();
	}			

	
	public void parseSkillRuneHistoryPacket(ToC_SKILLRUNE_HISTORY p)
	{
		parseBasePacket(p);
		historySkillRunes = p.historySkillRunes;

		GameManager.me.uiManager.popupRuneBook.onCompleteRecieveSkillRuneHistory();
	}
	
	

	public void parseEquipRuneHistoryPacket(ToC_EQUIPMENT_HISTORY p)
	{
		parseBasePacket(p);

		GameManager.me.uiManager.popupEquipRuneBook.onCompleteRecieveEquipRuneHistory(p.historyEquipments);
	}




	public void parseMessageListPacket(ToC_MESSAGE_LIST p)
	{
		parseBasePacket(p);

		GameManager.me.uiManager.popupMessage.refresh();
	}
	
	public void parseConfirmMessagePacket(ToC_CONFIRM_MESSAGE p, string msgId, string[] itemList)
	{
		parseBasePacket(p);
		energy = p.energy;
		chargeTime = p.chargeTime;
		gold = p.gold;
		ruby = p.ruby;
		runeStone = p.rstone;

		friendPoint = p.friendPoint;

		// 보상 받은 것 보여주기.

		parsePartsInven(p.equipments, true);
		parseUnitInven(p.unitRunes, true);
		parseSkillInven(p.skillRunes, true);

		if(messages.ContainsKey(msgId))
		{
			messages.Remove(msgId);
		}

		GameManager.me.uiManager.popupMessage.refresh();

		if(itemList != null)
		{
			RuneStudioMain.instance.playMakeResult(itemList, false);
		}

	}
	


	public Dictionary<string, DateTime> friendPointRefreshTimer = new Dictionary<string, DateTime>();
	public Dictionary<string, DateTime> missionCloseTime = new Dictionary<string, DateTime>();
	public Dictionary<string, DateTime> msgCloseTime = new Dictionary<string, DateTime>();

	public Dictionary<string, DateTime> championshipCoolTime = new Dictionary<string, DateTime>();

	public int waitFBPointNum = 0;

	public int slotMachinePrice = 0;

	// 하루 최대  pvp 횟수.
	public int friendlyPVPMax = 0;

	// 현재 플레이한 pvp id 목록.
	// 여기 포함된 애들과는 더이상 대전할 수 없다.
	public string[] friendlyPVPIds = null;

	public Dictionary<int, Dictionary<int, List<P_FriendData>>> friendDataByActRound = null;

	private void resetFriendDataByActRound()
	{
		if(friendDataByActRound == null) friendDataByActRound = new Dictionary<int, Dictionary<int, List<P_FriendData>>>();
		friendDataByActRound.Clear();

		for(int i = 1; i <= GameManager.MAX_ACT + 1; ++i)
		{
			friendDataByActRound.Add(i,new Dictionary<int, List<P_FriendData>>());

			for(int j = 1; j <= 5; ++j)
			{
				friendDataByActRound[i].Add(j, new List<P_FriendData>());
			}
		}
	}


	public Dictionary<string, P_FriendRewardRow> friendRewardTable;


	public void parseGetFriendsPacket(ToC_GET_FRIENDS p)
	{
		parseBasePacket(p);

		slotMachinePrice = p.slotMachinePrice;

		slotMachineRewardList = p.slotRewardList;

		friendlyPVPMax = p.fpvpMax;
		friendlyPVPIds = p.fpvpIds;

		waitFBPointNum = 0;

		if(friendDatas == null) friendDatas = new Dictionary<string, P_FriendData>();

		if(p.friendDatas == null)
		{
			if(friendDatas == null) friendDatas = new Dictionary<string, P_FriendData>();
			friendDatas.Clear();
			resetFriendDataByActRound();
		}
		else
		{
			friendDatas.Clear();
			resetFriendDataByActRound();

			foreach(KeyValuePair<string, P_FriendData> kv in p.friendDatas)
			{
				if(epi.GAME_DATA.appFriendDic.ContainsKey(kv.Key) == false) continue;


				if(friendPointRefreshTimer.ContainsKey(kv.Key) == false)
				{
					friendPointRefreshTimer.Add(kv.Key, DateTime.Now);
				}
				else
				{
					friendPointRefreshTimer[kv.Key] = DateTime.Now;
				}

				if(kv.Value.receivedFP == WSDefine.YES) ++waitFBPointNum;

				friendDatas.Add(kv.Key, kv.Value);

				friendDataByActRound[kv.Value.maxAct][kv.Value.maxStage].Add(kv.Value);

			}
		}

		friendRewardTable = p.fRewardTable;

		GameManager.me.uiManager.uiMenu.uiFriend.refresh();
		GameManager.me.uiManager.uiMenu.uiLobby.refreshWaitFbPointNum();
	}










	public void parseGetFriendsForGiftListPacket(ToC_GET_FRIENDS p)
	{
		parseBasePacket(p);
		
		slotMachinePrice = p.slotMachinePrice;
		
		slotMachineRewardList = p.slotRewardList;
		
		friendlyPVPMax = p.fpvpMax;
		friendlyPVPIds = p.fpvpIds;
		
		waitFBPointNum = 0;
		
		if(friendDatas == null) friendDatas = new Dictionary<string, P_FriendData>();
		
		if(p.friendDatas == null)
		{
			if(friendDatas == null) friendDatas = new Dictionary<string, P_FriendData>();
			friendDatas.Clear();
			resetFriendDataByActRound();
		}
		else
		{
			friendDatas.Clear();
			resetFriendDataByActRound();
			
			foreach(KeyValuePair<string, P_FriendData> kv in p.friendDatas)
			{
				if(epi.GAME_DATA.appFriendDic.ContainsKey(kv.Key) == false) continue;
				
				
				if(friendPointRefreshTimer.ContainsKey(kv.Key) == false)
				{
					friendPointRefreshTimer.Add(kv.Key, DateTime.Now);
				}
				else
				{
					friendPointRefreshTimer[kv.Key] = DateTime.Now;
				}
				
				if(kv.Value.receivedFP == WSDefine.YES) ++waitFBPointNum;
				
				friendDatas.Add(kv.Key, kv.Value);
				
				friendDataByActRound[kv.Value.maxAct][kv.Value.maxStage].Add(kv.Value);
				
			}
		}
		
		friendRewardTable = p.fRewardTable;

		GameManager.me.uiManager.uiMenu.uiLobby.refreshWaitFbPointNum();

		GameManager.me.uiManager.popupShop.popupGift.list.draw();

	}








	public void parseGetFriendDetailPacket(ToC_GET_FRIEND_DETAIL p, string friendId)
	{
//		parseBasePacket(p);

		//if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.FRIEND)
		{
			GameManager.me.uiManager.uiMenu.uiFriend.onCompleteGetFriendDetailInfo(p, friendId);
		}
	}
	
	public void parseSendFriendPointPacket(ToC_SEND_FRIEND_POINT p, string friendId, bool sendKakaoMessage = false)
	{
		parseBasePacket(p);
		friendPoint = p.friendPoint;

		if(friendPointRefreshTimer.ContainsKey(friendId) == false) friendPointRefreshTimer.Add(friendId,DateTime.Now);
		else friendPointRefreshTimer[friendId] = DateTime.Now;

		friendDatas[friendId].fpWaitingTime = p.fpWaitingTime;
		friendDatas[friendId].fLevel = p.fLevel;
		friendDatas[friendId].fExpGauge = p.fExpGauge;

		GameManager.me.uiManager.uiMenu.uiFriend.onCompleteSendFriendPoint(sendKakaoMessage, friendId);
	}
	
	public void parseRecieveFriendPointPacket(ToC_RECEIVE_FRIEND_POINT p, string friendId)
	{
		parseBasePacket(p);
		friendPoint = p.friendPoint;

		friendDatas[friendId].receivedFP = WSDefine.FALSE;
		friendDatas[friendId].fLevel = p.fLevel;
		friendDatas[friendId].fExpGauge = p.fExpGauge;


		GameManager.me.uiManager.uiMenu.uiFriend.onCompleteReceiveFriendPoint(friendId);
		GameManager.me.uiManager.uiMenu.uiWorldMap.refreshFriendInfo();
	}
	

	
//	public void parseConvertFpEnergyPacket(ToC_CONVERT_FP_ENERGY p)
//	{
//		parseBasePacket(p);
//		energy = p.energy;
//		chargeTime = p.chargeTime;
//		friendPoint = p.friendPoint;
//		
//	}
	
	public void parseInviteFriendPacket(ToC_INVITE_FRIEND p, string invitedId)
	{
		parseBasePacket(p);
		friendPoint = p.friendPoint;
		inviteCount = p.inviteCount;

		if(inviteFriendIds.Contains(invitedId) == false) inviteFriendIds.Add(invitedId);

		GameManager.me.uiManager.popupFriendInvite.onCompleteInvite();
	}
	


	public List<string> inviteFriendIds = null;


	public void parseInviteFriendList(ToC_INVITE_FRIEND_LIST p)
	{
		parseBasePacket(p);
		if(inviteFriendIds == null) inviteFriendIds = new List<string>();
		inviteFriendIds.Clear();
		if(p.inviteFriendIds != null) inviteFriendIds.AddRange(p.inviteFriendIds);

		GameManager.me.uiManager.popupFriendInvite.refresh();
	}



	public void parsePlayFriendPVPPacket(ToC_PLAY_FRIEND_PVP p)
	{
		parseBasePacket(p);


		GameManager.me.uiManager.popupFriendlyPVPAttack.onReceivePVPData(p);
		
//		// FRIEND DATA
//		public int fLevel;
//		public P_Hero fHero;
//		public Dictionary<string, string> fSelectUnitRunes;
//		public Dictionary<string, string> fSelectSkillRunes;
//		public string[] fpvpIds;
	}


	public void parseFinishFriendPVPPacket(ToC_FINISH_FRIEND_PVP p, bool isContinueMode = false)
	{
		parseBasePacket(p);

		friendlyPVPIds = p.fpvpIds;

		int rewardGold = p.gold - gold;

		gold = p.gold;

		if(isContinueMode == false)
		{
			GameManager.me.uiManager.popupChampionshipResult.prizeGold = rewardGold;  

			GameManager.me.uiManager.uiPlay.gameResultWord.callback = GameManager.me.uiManager.stageClearEffectManager.play;  //GameManager.me.uiManager.popupHellResult.showHellResult;
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType, 2, 0.25f);
		}
	}


	public Dictionary<string, P_Reward> slotMachineRewardList = null;

//	public void parseSlotMachineRewardPacket(ToC_SLOT_MACHINE_REWARD p)
//	{
//		parseBasePacket(p);
//		slotMachineRewardList = p.rewardList;
//	}


	public void parsePullSlotMachinePacket(ToC_PULL_SLOT_MACHINE p)
	{
		parseBasePacket(p);

		slotMachinePrice = p.slotMachinePrice;

		friendPoint = p.friendPoint;

		energy = p.energy;
		chargeTime = p.chargeTime;
		gold = p.gold;
		ruby = p.ruby;

		parsePartsInven(p.equipments, true);
		parseUnitInven(p.unitRunes, true);
		parseSkillInven(p.skillRunes, true);


		GameManager.me.uiManager.uiMenu.uiFriend.onCompletePullSlotMachine(p);

	}



	public void parseGetReplayPacket(ToC_GET_REPLAY p)
	{
		parseBasePacket(p);

		if(string.IsNullOrEmpty(p.replayData) || p.replayData.Length < 10 || ReplayManager.instance.convertServerReplayData(p.replayData) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "리플레이 데이터가 유효하지 않습니다.");
			return;
		}
		else if( VersionData.isCompatibilityVersion( GameManager.replayManager.replayGameVersion , false) == false )
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "호환되지 않는 리플레이 데이터입니다.\n(ver."+ GameManager.replayManager.replayGameVersion + ")");
			return;
		}

		GameManager.me.stageManager.setNowRound(GameManager.info.roundData["PVP"], GameType.Mode.Championship);

		ReplayManager.instance.replaySeed = Convert.ToInt32(p.aiId);

		//GamePlayerData gpd = new GamePlayerData(p.hero.name);

		GameDataManager.replayAttackerData = DebugManager.instance.setPlayerData(true, p.hero, p.selectUnitRunes, p.selectSkillRunes);

		if(p.subHero == null)
		{
			GameDataManager.replayAttacker2Data = null;
		}
		else
		{
			GameDataManager.replayAttacker2Data = DebugManager.instance.setPlayerData(true, p.subHero, p.selectSubUnitRunes, p.selectSubSkillRunes);
		}

		//=============== pvp ===================//

		DebugManager.instance.pvpPlayerData = DebugManager.instance.setPlayerData(false, p.eHero, p.eSelectUnitRunes, p.eSelectSkillRunes);

		if(p.eSubHero == null)
		{
			DebugManager.instance.pvpPlayerData2 = null;
		}
		else
		{
			DebugManager.instance.pvpPlayerData2 = DebugManager.instance.setPlayerData(false, p.eSubHero, p.eSubSelectUnitRunes, p.eSubSelectSkillRunes);
		}

		GameManager.me.recordMode = GameManager.RecordMode.replay;
		GameManager.me.uiManager.showLoading();

		GameManager.me.startGame(0.5f);
	}


	public void parsePrepareTutorialPacket(ToC_PREPARE_TUTORIAL p)
	{
		TutorialManager.instance.getTutorialPrepareData(p);
	}




	public void parseStartTutorialPacket(ToC_START_TUTORIAL p)
	{
		parseBasePacket(p);

		TutorialManager.instance.prepareTutorial(TutorialManager.instance.readyTutorialId);
	}


	public void parseCompleteTutorialPacket(ToC_COMPLETE_TUTORIAL p, bool isSkipTutorial)
	{
		parseBasePacket(p);

		if(p.tutorialHistory == null) tutorialHistory.Clear();
		else tutorialHistory = p.tutorialHistory;

		TutorialManager.instance.readyTutorialId = "";

		if(isSkipTutorial == false) TutorialManager.instance.onCompleteTutorial();
		else TutorialManager.instance.uiInit();
	}


	public void parseRewardTutorialPacket(ToC_REWARD_TUTORIAL p)
	{
		parseBasePacket(p);

		int getGold = p.gold - gold;
		int getRuby = p.ruby - ruby;
		int getEnergy = p.getEnergy;

		energy = p.energy;
		chargeTime = p.chargeTime;
		gold = p.gold;
		ruby = p.ruby;

		parseSelectHero(p.hero);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectSkills(Character.LEO,p.selectSkillRunes);

		TutorialManager.instance.onCompleteGetReward(getGold, getRuby, getEnergy);
	}



	public void parseCompleteRewardTutorialPacket(ToC_COMPLETE_REWARD_TUTORIAL p)
	{
		parseBasePacket(p);

		if(p.tutorialHistory == null) tutorialHistory.Clear();
		else tutorialHistory = p.tutorialHistory;

		chargeTime = p.chargeTime;


		int getGold = p.gold - gold;
		int getRuby = p.ruby - ruby;
		int getEnergy = p.getEnergy;
		
		energy = p.energy;
		gold = p.gold;
		ruby = p.ruby;

		parseSelectHero(p.hero);

		parseSelectUnits(Character.LEO, p.selectUnitRunes); 
		parseSelectSkills(Character.LEO,p.selectSkillRunes);

		TutorialManager.instance.onCompleteGetReward(getGold, getRuby, getEnergy);
	}




	//========================
	/*
	public void parsePrepareChallengePacket(ToC_PREPARE_CHALLENGE p)
	{
		parseBasePacket(p);

		challengeRoundInfo = p;

		GameManager.me.uiManager.popupChallenge.refresh();
	}

	public void parseChangeChallengeItemsPacket(ToC_CHANGE_CHALLENGE_ITEMS p)
	{
		parseBasePacket(p);

		ruby = p.ruby;

		GameDataManager.instance.challengeRoundInfo.items = p.items;

		GameDataManager.instance.challengeRoundInfo.waitingTime = p.waitingTime;
		GameDataManager.instance.challengeModeWaitCheckTime = DateTime.Now;

		GameManager.me.uiManager.popupChallenge.refresh();
	}


	public void parseBuyChallengeItemPacket(ToC_BUY_CHALLENGE_ITEM p)
	{
		parseBasePacket(p);
		ruby = p.ruby;

		parsePartsInven(p.equipments);
		

		parseUnitInven(p.unitRunes);
		

		parseSkillInven(p.skillRunes);
		

		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) GameManager.me.uiManager.popupSummonDetail.hide();

		if(GameManager.me.uiManager.popupSkillPreview.isEnabled) GameManager.me.uiManager.popupSkillPreview.isEnabled = false;

		if(GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.gameObject.activeSelf) GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();

		RuneStudioMain.instance.playMakeResult(new string[]{p.itemId}, false);

	}



	public void parsePlayChallengePacket(ToC_PLAY_CHALLENGE p, bool isTutorial = false)
	{
		parseBasePacket(p);

		energy = p.energy;
		chargeTime = p.chargeTime;
		ruby = p.ruby;

		challengeRoundInfo.playNeedRuby = p.playNeedRuby;
		challengeRoundInfo.countUsingRuby = p.countUsingRuby;

		if(isTutorial) EpiServer.instance.sendCompleteRewardTutorial("T27");
		else GameManager.me.uiManager.popupChallenge.onCompleteReceivePlay();
	}

	public void parseFinishChallengePacket(ToC_FINISH_CHALLENGE p, bool isContinueCheckMode = false)
	{
		parseBasePacket(p);

		if(challengeRoundInfo != null) challengeRoundInfo.currentMode = p.maxMode;

		if(p.level > level) UISystemPopup.needLevelupPopup = true;

		gold = p.gold;
		ruby = p.ruby;
		level = p.level;
		exp = p.exp;
		expFull = p.fullExp;



		if(isContinueCheckMode == false)
		{
			GameManager.me.uiManager.popupChallengeResult.show();
			GameManager.me.uiManager.popupChallengeResult.init(p.rewardItems);
		}

		parsePartsInven(p.equipments);
		

		parseUnitInven(p.unitRunes);
		

		parseSkillInven(p.skillRunes);
		


		//GameManager.me.uiManager.popupChallenge.refresh();

//		public int currentMode; // 1:RUSH, 2:SURVIVE, 3:HUNT
//		public string rewardItemId;
//		public Dictionary<string, int> equipments;
//		public Dictionary<string, int> equipmentPrices;
		
	}
	*/


	public void parseCompleteAllTutorialPacket(ToC_COMPLETE_ALL_TUTORIAL p)
	{
		parseBasePacket(p);
		
		if(p.tutorialHistory == null) tutorialHistory.Clear();
		else tutorialHistory = p.tutorialHistory;
	}




	public void parseChampionshipDataPacket(ToC_CHAMPIONSHIP_DATA p, bool isContinueCheckMode)
	{
		parseBasePacket(p);
		championshipData = p;
		refreshChampionshipCoolTime(p.champions);

		championShipCheckTime = DateTime.Now;
		GameManager.me.uiManager.popupChampionship.refresh();
		GameManager.me.uiManager.popupChampionshipReplayPanel.refresh();
		if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.WORLD_MAP) GameManager.me.uiManager.uiMenu.uiWorldMap.refresh(true);

		if(isContinueCheckMode)
		{
			if(p.champions.ContainsKey(GameManager.me.uiManager.popupChampionshipResult.enemyId))
			{

				P_Champion pd = p.champions[GameManager.me.uiManager.popupChampionshipResult.enemyId];
				UISystemPopup.open(UISystemPopup.PopupType.YesNo,"진행중인 전투가 존재합니다.\n이어서 진행하시겠습니까?\n\n[챔피언십] VS "+ pd.nickname + " " + GameManager.me.uiManager.popupChampionshipResult.matchNumber + "차전", GameManager.me.startContinueGame, EpiServer.instance.sendDeniedContinuePVP);
				GameManager.me.uiManager.uiMenu.changePanel(UIMenu.WORLD_MAP);
			}
			else
			{
				EpiServer.instance.sendInvalidateGame();
			}
		}
	}


	public void parseLastChampionshipDataPacket(ToC_LAST_CHAMPIONSHIP_DATA p)
	{
		parseBasePacket(p);
		lastChampionshipData = p;
		GameManager.me.uiManager.popupChampionshipLastWeekResult.show();
	}



	public void parseRejoinChampionshipPacket(ToC_REJOIN_CHAMPIONSHIP p)
	{

		parseBasePacket(p);
		champGroupId = p.champGroupId;
		champMemberId = p.champMemberId;

		EpiServer.instance.sendChampionShipData();
	}





	public void parseRequestMissionReward(ToC_REQUEST_MISSION_REWARD p, string requestMissionId)
	{
		parseBasePacket(p);

		energy = p.energy;
		chargeTime = p.chargeTime;
		gold = p.gold;
		ruby = p.ruby;

		runeStone = p.rstone;

		friendPoint = p.friendPoint;

		parsePartsInven(p.equipments, true);
		
		parseUnitInven(p.unitRunes, true);
		
		parseSkillInven(p.skillRunes, true);
		

		if(missions.ContainsKey(requestMissionId))
		{
			missions[requestMissionId].state = WSDefine.CLOSE;
		}

		hasClearMission = false;

		foreach(KeyValuePair<string, P_Mission> kv in missions)
		{
			if(kv.Value.state == WSDefine.CLEAR)
			{
				hasClearMission = true;
				break;
			}
		}

		if(GameManager.me != null)
		{
			GameManager.me.uiManager.uiMenu.uiLobby.spHasMissionIcon.gameObject.SetActive(hasClearMission);
		}

		
		GameManager.me.uiManager.uiMenu.uiMission.refresh();


		if(p.gachaRewards != null)
		{
			if(missions.ContainsKey(requestMissionId))
			{
				GameManager.me.uiManager.rewardNotice.start(false, missions[requestMissionId].rewards);
			}

			RuneStudioMain.instance.playMakeResult(p.gachaRewards, false);

		}
		else
		{
			if(missions.ContainsKey(requestMissionId))
			{
				GameManager.me.uiManager.rewardNotice.start(true, missions[requestMissionId].rewards);
			}
		}

		//gachaRewards = p.gachaRewards;

	}



	public void parseEnemyData(ToC_ENEMY_DATA p, P_Champion data)
	{
		parseBasePacket(p);

		GameManager.me.uiManager.popupFriendDetail.setData(p, data);
		//GameManager.me.uiManager.popupChampionshipAttack.onReceiveEnemyData(p);
	}



	public void parsePlayPVP(ToC_PLAY_PVP p, string tutorialPVPId = null)
	{
		parseBasePacket(p);

		ruby = p.ruby;

		RoundData rd = GameManager.info.roundData["PVP"];
		GameManager.me.stageManager.setNowRound(rd, GameType.Mode.Championship);

		DebugManager.instance.pvpPlayerData = DebugManager.instance.setPlayerData(false, p.eHero, p.eSelectUnitRunes, p.eSelectSkillRunes);



		if(p.eSubHero == null)
		{
			DebugManager.instance.pvpPlayerData2 = null;
		}
		else
		{
			DebugManager.instance.pvpPlayerData2 = DebugManager.instance.setPlayerData(false, p.eSubHero, p.eSelectUnitRunes_Sub, p.eSelectSkillRunes_Sub);
		}


		if(tutorialPVPId != null)
		{
			EpiServer.instance.sendCompleteRewardTutorial(tutorialPVPId);
		}
		else GameManager.me.uiManager.popupChampionshipAttack.onReceivePVPData();
	}

	public void parseFinishPVP(ToC_FINISH_PVP p, bool isContinueFailMode = false)
	{
		parseBasePacket(p);

		if(championshipData != null)
		{
			championshipData.champions = p.champions;

			refreshChampionshipCoolTime(p.champions);
		}

		int priceGold = gold;
		priceGold = p.gold - gold;
		gold = p.gold;

		if(isContinueFailMode == false)
		{
			GameManager.me.uiManager.popupChampionshipResult.prizeGold = priceGold;  
			GameManager.me.uiManager.popupChampionshipResult.score = p.score;
			GameManager.me.uiManager.uiPlay.gameResultWord.callback = GameManager.me.uiManager.stageClearEffectManager.play;  //GameManager.me.uiManager.popupHellResult.showHellResult;
			GameManager.me.uiManager.uiPlay.gameResultWord.init(GameManager.me.stageManager.nowPlayingGameResult, GameManager.me.stageManager.nowPlayingGameType, 2, 0.25f);
		}
	}


	void refreshChampionshipCoolTime(Dictionary<string, P_Champion> champs)
	{
		foreach(KeyValuePair<string, P_Champion> kv in champs)
		{
			if(championshipCoolTime.ContainsKey(kv.Value.userId))
			{
				championshipCoolTime[kv.Value.userId] = DateTime.Now;
			}
			else
			{
				championshipCoolTime.Add( kv.Value.userId , DateTime.Now);
			}
		}
	}




	public void parseBuyGoldPacket(ToC_BUY_GOLD p)
	{
		parseBasePacket(p);

		if(string.IsNullOrEmpty(p.message) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, p.message);
		}
		
		switch(p.result)
		{
		case WSDefine.REFRESH: //-1:REFRESH
			GameManager.me.uiManager.popupShop.refresh();
			break;
		case WSDefine.FAIL: //0:FAIL
			break;
		case WSDefine.SUCCESS: //1:SUCCESS
			int buyGold = p.gold - gold;
			
			gold = p.gold;
			ruby = p.ruby;
			
			if(buyGold > 0)
			{
				P_Reward boughtShopItem = new P_Reward();
				boughtShopItem.count = buyGold;
				boughtShopItem.code = WSDefine.GOLD;
				GameManager.me.uiManager.rewardNotice.start(true, boughtShopItem);
			}

			break;
		}
	}


	public void parseBuyEnergyPacket(ToC_BUY_ENERGY p)
	{
		parseBasePacket(p);

		if(string.IsNullOrEmpty(p.message) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, p.message);
		}

		switch(p.result)
		{
		case WSDefine.REFRESH: //-1:REFRESH
			GameManager.me.uiManager.popupShop.refresh();
			break;

		case WSDefine.FAIL: //0:FAIL
			break;

		case WSDefine.SUCCESS: //1:SUCCESS

			energy = p.energy;
			chargeTime = p.chargeTime;
			gold = p.gold;
			ruby = p.ruby;
			
			parsePartsInven(p.equipments, true);
			parseUnitInven(p.unitRunes, true);
			parseSkillInven(p.skillRunes, true);
			
			RuneStudioMain.instance.playMakeResult(p.newItems);

			break;
		}
	}


	public Dictionary<string, P_Package> packagePriceInfo = new Dictionary<string, P_Package>();
	
	public void parseBuyByMoneyPacket(ToC_BUY_BY_MONEY p, bool isMissingBuying = false)
	{
		parseBasePacket(p);
	
		int buyRuby = p.ruby - ruby;

		ruby = p.ruby;

		parseAnnuityProducts(p.annuityProducts, isMissingBuying == false);

		if(string.IsNullOrEmpty( p.paymentId ) == false)
		{
			#if !UNITY_EDITOR
			try
			{
				PandoraIAB.instance.confirmPurchase(p.paymentId);
			}
			catch(Exception e)
			{
				
			}
			#endif
		}


		if(p.result == WSDefine.SUCCESS)
		{
			if(AdPluginManager.useAdbrix)
			{
#if UNITY_ANDROID
				IgaworksUnityPluginAOS.Adbrix.buy(p.productId);
#elif UNITY_IPHONE
//				IgaworksADPluginIOS.AdbrixBuy(p.productId);
#endif
			}


			string pId = p.productId;

			if(pId.Contains("_gift")) pId = pId.Replace("_gift","");

			string productPrice = getMoneyProductPrice(pId, packagePriceInfo);

			if(productPrice != null)
			{
				AdPluginManager.reportGoogleConversion(pId, productPrice);

				AdPluginManager.reportGoogleRemarketing(new string[]{"action_type","purchase","product_id",p.productId,"value",productPrice});
			}

		}
		else if(p.result == WSDefine.REFRESH) // REFRESH
		{

		}




		if(isMissingBuying == false)
		{

			if(string.IsNullOrEmpty( p.message ) == false)
			{
				string msg = p.message;

				if(msg.Contains("/"))
				{
					string[] m = msg.Split('/');

					if(epi.GAME_DATA.appFriendDic.ContainsKey(m[0]))
					{
						msg = epi.GAME_DATA.appFriendDic[m[0]].f_Nick ;

						for(int i = 1; i < m.Length; ++i)
						{
							msg += m[i];
						}
					}
				}

				UISystemPopup.open( UISystemPopup.PopupType.Default, msg);
			}
		}
		else
		{
			if(EpiServer.instance.confirmMissingPurchase() == false)
			{
				if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_TITLE)
				{
					GameManager.me.preLoadComplete();
				}
			}
		}
	}




	public void parseBuyItemPacket(ToC_BUY_ITEM p, string buyProductName, string buyProductId)
	{
		parseBasePacket(p);


		if(string.IsNullOrEmpty(p.message) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, p.message);
		}
		
		switch(p.result)
		{
		case WSDefine.REFRESH: //-1:REFRESH
			GameManager.me.uiManager.popupShop.refresh();
			break;
		case WSDefine.FAIL: //0:FAIL
			break;
		case WSDefine.SUCCESS: //1:SUCCESS

			gold = p.gold;
			ruby = p.ruby;
			runeStone = p.rstone;

			parsePartsInven(p.equipments, true);
			parseUnitInven(p.unitRunes, true);
			parseSkillInven(p.skillRunes, true);
			
			if(p.newItems != null)
			{
				foreach(string str in p.newItems)
				{
					GameManager.me.uiManager.popupShop.jackpotManager.addMyJackPot(str, buyProductName);
				}

				/* 화폐 상품만 로그 기록함.
				if(AdPluginManager.useAdbrix)
				{
					#if UNITY_ANDROID
					IgaworksUnityPluginAOS.Adbrix.buy(buyProductId);
					#elif UNITY_IPHONE
//					IgaworksADPluginIOS.AdbrixBuy(buyProductId);
					#endif
				}
*/

				RuneStudioMain.instance.playMakeResult(p.newItems);
			}

			break;
		}


//#if UNITY_EDITOR
//
//		Debug.LogError("테스트 코드!!!");
//
//		List<string> fuck = new List<string>();
//
//		string h = "";
//		foreach(string str in p.newItems)
//		{
//			fuck.Add(str);
//			h = str;
//		}
//
//		while(fuck.Count < 9)
//		{
//			fuck.Add(h);
//		}
//
//		RuneStudioMain.instance.playMakeResult(fuck.ToArray());
//
//		return;
//
//#endif



	}







	public Dictionary<string, P_Reinforce> equipReinforceData = null;
	public Dictionary<string, P_Reinforce> skillReinforceData = null;
	public Dictionary<string, P_Reinforce> unitReinforceData = null;





	public void parseBuyHeroDataPacket(ToC_BUY_HERO p, GameIDData.Type fromUIType)
	{
		parseBasePacket(p);
		ruby = p.ruby;

		parseHeroInven(p.heroes);

		selectHeroId = p.selectHeroId;
		selectSubHeroId = p.selectSubHeroId;

		switch(fromUIType)
		{
		case GameIDData.Type.None:
			GameManager.me.uiManager.popupHeroInfo.refreshTab();

			GameManager.me.uiManager.uiMenu.uiSummon.tabPlayer.refreshUnlock();
			GameManager.me.uiManager.uiMenu.uiSkill.tabPlayer.refreshUnlock();
			GameManager.me.uiManager.uiMenu.uiHero.tabPlayer.refreshUnlock();

			break;
		case GameIDData.Type.Unit:
			GameManager.me.uiManager.uiMenu.uiSummon.tabPlayer.onCompleteBuyHero();
			break;
		case GameIDData.Type.Skill:
			GameManager.me.uiManager.uiMenu.uiSkill.tabPlayer.onCompleteBuyHero();
			break;
		case GameIDData.Type.Equip:
			GameManager.me.uiManager.uiMenu.uiHero.tabPlayer.onCompleteBuyHero();
			break;

		}
	}



	
	public void parseGetSettingPacket(ToC_GET_SETTINGS p)
	{
		parseBasePacket(p);
		showPhoto = (p.showPhoto);

		if(PandoraManager.instance.localUser.messageBlock && p.blockMessage == WSDefine.NO)
		{
			messageBlock = WSDefine.YES;
			EpiServer.instance.sendSetSetting();
		}
		else
		{
			messageBlock = (p.blockMessage);
		}
	}

	public void parseSetSettingPacket(ToC_SET_SETTINGS p)
	{
		parseBasePacket(p);
		showPhoto = (p.showPhoto);
		messageBlock = (p.blockMessage);
	}


	public void parseGetJackPotUsers(ToC_GET_JACKPOT_USERS p)
	{
//		parseBasePacket(p);

		if(p.jackpots != null)
		{
			GameManager.me.uiManager.popupShop.jackpotManager.parseJackpotServerData(p);
		}
	}
	
	
	public void parseGetOtherData(ToC_GET_OTHER_DATA p)
	{
//		parseBasePacket(p);

		GameManager.me.uiManager.popupShop.popupFriendDetail.setData(p);

		//GameManager.me.uiManager.popupFriendDetail.setData(p);
	}



	
	public void parseRemoveUserPacket(ToC_REMOVE_USER p)
	{
//		parseBasePacket(p);

		GameManager.me.restartAppGame();
	}
		


//	public Dictionary<string, string> serverText = new Dictionary<string, string>();
//	public Dictionary<string, P_Popup> systemAlert = new Dictionary<string, P_Popup>();

//	public P_Popup[] notifyPopups = null;
//	public P_Popup actionPopup = null;
//	public P_PromotionPopup specialProduct = null;

//	public Dictionary<string, Dictionary<string, string>> eventDatas = new Dictionary<string, Dictionary<string, string>>();

	public Dictionary<string, P_Mission> missions = new Dictionary<string, P_Mission>();
	public Dictionary <string, P_MissionNotification> missionNotifications = new Dictionary<string, P_MissionNotification>();




//	public class P_MessengerData
//	{
//		public string to;
//		public string text;
//		public string templateId;
//		public string image;
//		public string param;
//	}
	/*
 * 카톡으로 보낼 메세지 데이터
 * to => 친구ID
 * text => 메세지
 * templateId => 친구초대	1432, 자랑하기	1516, 열쇠 메시지	1515, 신발 보내기	1433
 * image => 이미지(필요한 경우만)
 * param => 카톡 param
*/
	public void parsePaymentPacket(ToC_PAYMENT p)
	{
//		if (retData.echo != "MISSING_ITEM")
//		{
//			if (retData.messengerData != null)
//			{
//				PandoraManager.instance.kakaoSendMessage(retData.messengerData);
//			}
//		}
		
//		if (Application.loadedLevelName == "stageselect"){
//			StageSelectSceneManager.instance.refreshGoldRubyShoeStarUI();
//			if (SubPanelShop.instance != null){
//				SubPanelShop.instance.refreshGoldRuby();
//			}
//			
//		}
	}








	public void parseRefreshEventPacket(ToC_REFRESH_EVENT p)
	{
		parseBasePacket(p);

		//public Dictionary<string, string> resultData; // <= 현재는 안쓴다.
		// 여기 갔다오면 메시지 함에 아이템이 들어올 수 있다.

	}



	public void parseActionEventPacket(ToC_ACTION_EVENT p, string actionUrl, string actionType)
	{
		parseBasePacket(p);
		
		if(string.IsNullOrEmpty( actionUrl ) == false)
		{
			if(actionUrl.ToLower().StartsWith("market") || actionUrl.ToLower().StartsWith("itunes") || (actionType != null && actionType == "REVIEW"))
			{
				Application.OpenURL(actionUrl);
			}
			else
			{
				PandoraManager.instance.showWebView(actionUrl);
			}
		}
		//public Dictionary<string, string> resultData;
	}





	public void parseMissionDataPacket(ToC_MISSION_DATA p)
	{
		parseBasePacket(p);
		
		GameManager.me.uiManager.uiMenu.uiMission.refresh();

	}



	public void parseCheckProductPacket(ToC_CHECK_PRODUCT p, string productId, bool isGift, string friendId)
	{
		parseBasePacket(p);

		if(string.IsNullOrEmpty(p.message) == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, p.message);
		}

		switch(p.result)
		{
		case WSDefine.REFRESH: //-1:REFRESH
			GameManager.me.uiManager.popupShop.refresh();
			break;
		case WSDefine.FAIL: //0:FAIL
			break;
		case WSDefine.SUCCESS: //1:SUCCESS

			if(string.IsNullOrEmpty(productId) == false)
			{
				EpiServer.instance.buyItem( productId, isGift, friendId);
			}

			break;
		}

	}




	public void parseBasePacket(ToC_BASE p)
	{
		if(p == null) return;


		if(p.C_Missions != null)
		{
			hasClearMission = false;

			missions = p.C_Missions;

			foreach(KeyValuePair<string, P_Mission> kv in missions)
			{
				if(missionCloseTime.ContainsKey(kv.Key) == false)
				{
					missionCloseTime.Add(kv.Key, DateTime.Now);
				}
				else
				{
					missionCloseTime[kv.Key] = DateTime.Now;
				}

				if(kv.Value.state == WSDefine.CLEAR)
				{
					hasClearMission = true;
				}
			}
		}

		if(p.C_MissionNotifications != null)
		{
			foreach(KeyValuePair<string, P_MissionNotification> kv in p.C_MissionNotifications)
			{
				if(missionNotifications.ContainsKey(kv.Key))
				{
					missionNotifications[kv.Key] = kv.Value;
				}
				else
				{
					missionNotifications.Add(kv.Key, kv.Value);
				}

				if(kv.Value.state == WSDefine.CLEAR)
				{
					hasClearMission = true;
				}
			}

			//missionNotifications = 
		}

		if(GameManager.me != null)
		{
			GameManager.me.uiManager.uiMenu.uiLobby.spHasMissionIcon.gameObject.SetActive(hasClearMission);
		}

	

		if(p.C_Shop != null)
		{
			shopData = p.C_Shop;

			lastTimeOfShopData = Time.realtimeSinceStartup;

		}


		if(p.C_Messages != null)
		{
			messages = p.C_Messages;

			foreach(KeyValuePair<string, P_Message  > kv in messages)
			{
				if(msgCloseTime.ContainsKey(kv.Key) == false)
				{
					msgCloseTime.Add(kv.Key, DateTime.Now);
				}
				else
				{
					msgCloseTime[kv.Key] = DateTime.Now;
				}
			}

			checkNewMsgNum();
		}

		championshipStatus = p.C_ChampStatus; // 1:OPEN, 2:CLOSE

		championshipDefence = p.C_ChampDefence; 	// 1:YES, 0:NO


		if(p.C_Packages != null)
		{
			packages = p.C_Packages;
		}


		checkMissionNotice();


		/*
#if UNITY_EDITOR

		Debug.LogError("테스트!!!!");
		p.C_Popups = new Dictionary<string, P_Popup>();

		P_Popup p0 = new P_Popup();
		p0.size = "L";
		p0.title = null;
		p0.text = "fadskjfl;kajsf;klasjkflja\nfasdklfja;lsjkfl";
		p0.delay = 1;
		p0.image = "http://office.linktomorrow.com/common/windsoul/sample.png";
		p0.buttonType = "CLOSE:NO";
		p0.actionType = "";
		p0.actionData = "";
		p0.options = new Dictionary<string, string>();
		p0.options.Add("OFF_TODAY","");
		p.C_Popups.Add("fdas1",p0);


		P_Popup p2 = new P_Popup();
		p2.size = "S";
		p2.title = null;
		p2.text = "fadskjfl;kajsf;klasjkflja\nfasdklfja;lsjkfl";
		p2.delay = 0;
		p2.image = "http://office.linktomorrow.com/common/windsoul/endpopup.png";
		p2.buttonType = "OK:CONFIRM";
		p2.actionType = "";
		p2.actionData = "";
		p2.options = new Dictionary<string, string>();
		p2.options.Add("OFF_TODAY","");
		p.C_Popups.Add("fd1",p2);


		P_Popup p1 = new P_Popup();
		p1.size = "M";
		p1.title = "mini title";
		p1.text = "mini body";
		p1.delay = 2;
		p1.buttonType = "OK:DETAIL,CLOSE:NO";
		p1.actionType = "GAMEUI";
		p1.actionData = "HERO";
		p1.options = new Dictionary<string, string>();
		p1.options.Add("OFF_TODAY","");


		p.C_Popups.Add("fdas",p1);

#endif
		*/

		if(p.C_Popups != null)
		{
			foreach(KeyValuePair<string, P_Popup> kv in p.C_Popups)
			{
				if(kv.Value.options != null)
				{
					// 오늘 하루 보지 않기.
					if(kv.Value.options.ContainsKey("OFF_TODAY"))
					{
						// 오늘 하루 보지 않기 자동으로.
						if(PlayerPrefs.GetInt("P"+UIInGameNoticePopup.getPopupHash(kv.Value), -1000) == UIInGameNoticePopup.getToday())
						{
							continue;
						}
					}

					if(kv.Value.options.ContainsKey("MOVE_TO"))
					{
						if(kv.Value.options["MOVE_TO"] == "HEAD")
						{
							_tempfirstPopups.Insert(0, kv.Value);
							continue;
						}
						else if(kv.Value.options["MOVE_TO"] == "TAIL")
						{
							_tempEndPopups.Insert(0, kv.Value);
							continue;
						}
					}
				}

				_tempPopups.Add(kv.Value);
			}


			for(int i = 0; i < _tempfirstPopups.Count; ++i)
			{
				popups.Enqueue(_tempfirstPopups[i]);
			}
			
			for(int i = 0; i < _tempPopups.Count; ++i)
			{
				popups.Enqueue(_tempPopups[i]);
			}
			
			for(int i = 0; i < _tempEndPopups.Count; ++i)
			{
				popups.Enqueue(_tempEndPopups[i]);
			}
			
			_tempfirstPopups.Clear();
			_tempPopups.Clear();
			_tempEndPopups.Clear();
			
			
		}
	}


	private List<P_Popup> _tempfirstPopups = new List<P_Popup>();
	private List<P_Popup> _tempEndPopups = new List<P_Popup>();
	private List<P_Popup> _tempPopups = new List<P_Popup>();


	public Queue<P_Popup> popups = new Queue<P_Popup>();



	public void checkServerPopup(P_Popup p)
	{
//		if(TutorialManager.instance == null || TutorialManager.instance.clearCheck("T13") == false) return;
//		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_TITLE) return;
//		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_LOADING) return;
//		if(checkUI && (GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY )) return;



	}








	public void checkMissionNotice(bool checkUI = true)
	{
		if(TutorialManager.instance == null || TutorialManager.instance.clearCheck("T13") == false) return;
		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_TITLE) return;
		if(GameManager.me.uiManager.currentUI == UIManager.Status.UI_LOADING) return;

		if(checkUI && (GameManager.me.uiManager.currentUI == UIManager.Status.UI_PLAY )) return;

		if(missionNotifications.Count > 0 && UIMissionClearNotice.instance != null)
		{
			UIMissionClearNotice.instance.start(missionNotifications);
			missionNotifications.Clear();
		}
	}









	













	void checkNewMsgNum()
	{
		int newMsg = 0;
		
		foreach(KeyValuePair<string, P_Message> kv in messages)
		{
			if(kv.Value.isNew == WSDefine.YES) ++newMsg;
		}
		
		if(newMsg > 0)
		{
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.gameObject.SetActive(true);
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.text = newMsg.ToString();
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.gameObject.SetActive(false);
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.text = newMsg.ToString();
		}

		GameManager.me.uiManager.uiMenu.uiLobby.updateNewMsgAni();
	}




	public string getMoneyProductPrice(string id, Dictionary<string, P_Package> prevPackageData = null)
	{
		float price = -1000;

		if(shopData != null && shopData.rubyProducts != null && shopData.rubyProducts.ContainsKey(id))
		{
			price = shopData.rubyProducts[id].price;
		}
		else if(shopData != null && shopData.giftProducts != null && shopData.giftProducts.ContainsKey(id))
		{
			price = shopData.giftProducts[id].price;
		}
		else if(packages != null && packages.ContainsKey(id))
		{
			price = packages[id].price;
		}
		else if(prevPackageData != null && prevPackageData.ContainsKey(id))
		{
			price = prevPackageData[id].price;
		}

		if(price > 0)
		{
			#if UNITY_ANDROID
			return Util.GetCommaScore( MathUtil.RoundToInt( price ) );
			#else
			return string.Format("{0:F2}", price);
			#endif
		}

		return null;
	}



	public Dictionary<string, DateTime> annuityUpdateTime = new Dictionary<string, DateTime>();

	private void parseAnnuityProducts(Dictionary<string, P_Annuity> data, bool refreshList)
	{
		annuityProducts = data;

		if(annuityProducts != null)
		{
			foreach(KeyValuePair<string, P_Annuity> kv in annuityProducts)
			{
				if(annuityUpdateTime.ContainsKey(kv.Value.annuityId))
				{
					annuityUpdateTime[kv.Value.annuityId] = DateTime.Now;
				}
				else
				{
					annuityUpdateTime.Add( kv.Value.annuityId , DateTime.Now);
				}
			}
		}

		if(refreshList)
		{
			if(GameManager.me.uiManager.popupShop.gameObject.activeSelf)
			{
				if(GameManager.me.uiManager.popupShop.listSpecial.gameObject.activeSelf)
				{
					GameManager.me.uiManager.popupShop.listSpecial.draw(false);
				}
			}
		}
	}


	public void parseConsumeAnnuityPacket(ToC_CONSUME_ANNUITY p)
	{
		parseBasePacket(p);

		parseAnnuityProducts(p.annuityProducts, true);
	}


	public void parseRefreshAnnuityPacket(ToC_REFRESH_ANNUITY p)
	{
		parseBasePacket(p);

		parseAnnuityProducts(p.annuityProducts, true);
	}


}



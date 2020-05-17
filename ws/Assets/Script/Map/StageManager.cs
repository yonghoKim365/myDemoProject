using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager
{
	public RoundData demoRound = new RoundData();

	private StageManager ()
	{
		demoRound.mode = RoundData.MODE.DEMO;
	}

	public void setDemoRound()
	{
		clearChecker = GameManager.me.stageManager.dontCheck;
		failChecker = GameManager.me.stageManager.dontCheck;
		nowRound = demoRound;
	}

	private static StageManager _instance;
	
	public static StageManager instance
	{
		get
		{
			if(_instance == null) _instance = new StageManager();
			return _instance;
		}
	}
	
	public void OnDestroy()
	{
		_instance = null;
	}
	
	public int stageNumber = 1;
	public StageData nowStageData;
	
	private int _stageLevel = 1;
	private int _clearSetNum = 0;

	public Xfloat playTime = 0.0f;

	public Monster[] heroMonster;
	
	public Monster protectNPC = null;
	public Monster chaser = null;
	
	public Monster[] playerProtectObjectMonster;
	public int protectObjMonCount = 0;
	
	public Monster[] playerDestroyObjectMonster;
	public int destroyObjMonCount = 0;	

	public void init()
	{
		nowSelectStage = "STAGE1";
	}
	
	public string nowSelectStage = "STAGE1";
	



	public int settingTime
	{
		get
		{
			if(_nowRound == null)
			{
				return 120;
			}
			else
			{
				if(_nowRound.mode == RoundData.MODE.HELL)
				{
					return HellModeManager.instance.nowRound.settingTime;
				}
				else
				{
					return _nowRound.settingTime;
				}
			}
		}
	}



	private RoundData _nowRound = null;
	public RoundData nowRound
	{
		set
		{
			_nowRound = value;
		}
		get
		{
			if(_nowRound.mode == RoundData.MODE.HELL)
			{
				return HellModeManager.instance.nowRound;
			}
			else
			{
				return _nowRound;
			}
		}
	}

	public string nowRoundMode
	{
		get
		{
			return _nowRound.mode;
		}
	}

	public string nowRoundId
	{
		get
		{
			return _nowRound.id;
		}
	}


	public static Xint mapStartPosX = 0;
	public static Xint mapEndPosX = 1000;
	public static Xint mapPlayerEndPosX = 1000;
	
	public delegate bool GameClearChecker(int attr = -1, Monster mon = null);
	public GameClearChecker clearChecker = null;
	
	public delegate bool GameFailChecker(int attr = -1, Monster mon = null);
	public GameFailChecker failChecker = null;	
	

	public GameType.Mode nowPlayingGameType = GameType.Mode.Epic;
	public Xint nowPlayingGameResult = Result.Type.Win;

	public bool isPVPMode = false;

	public bool isSurrenderGame = false;

	public Xbool isIntro = false;

	public Xint playAct = 1;
	public Xint playStage = 1;
	public Xint playRound = 1;

	public bool isRepeatGame = false;

	public bool isMaxRound = false;

	public P_Sigong sigongData;

	public string getCutSceneId()
	{
		if(nowPlayingGameType == GameType.Mode.Sigong)
		{
			if(sigongData != null)
			{
				return sigongData.cutscene;
			}
		}
		else
		{
			return nowRound.cutSceneId;
		}

		return null;
	}


	public int getMapId(int grade = 1)
	{
		if(nowPlayingGameType == GameType.Mode.Sigong)
		{
			if(sigongData != null)
			{
				if(string.IsNullOrEmpty(sigongData.map) == false)
				{
					int sigongMapId = -1;
					int.TryParse(sigongData.map, out sigongMapId);
					if(sigongMapId > 0)
					{
						return sigongMapId;
					}
				}
			}
		}

		if(nowRound.id != "PVP")
		{
			return nowRound.mapId[0];
		}
		
		if(grade >= 5 && nowRound.mapId.Length > 2)
		{
			return nowRound.mapId[2];
		}
		else if(grade >= 3 && nowRound.mapId.Length > 1)
		{
			return nowRound.mapId[1];
		}
		
		return nowRound.mapId[0];
	}





	public void setNowRound(RoundData rd, GameType.Mode gameType = GameType.Mode.Epic, int inputAct = 1, int inputStage = 1, int inputRound = 1)
	{
//		Debug.LogError("**** StageManager setNowRound");

		sigongData = null;

		playAct = inputAct;
		playStage = inputStage;
		playRound = inputRound;

		DebugManager.instance.useTagMatchMode = (gameType == GameType.Mode.Friendly || gameType == GameType.Mode.Championship);


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			DebugManager.instance.useTagMatchMode = (rd.mode == RoundData.MODE.PVP);
		}
#endif


		isMaxRound = (inputAct == GameDataManager.instance.maxAct && inputStage == GameDataManager.instance.maxStage && inputRound == GameDataManager.instance.maxRound);

		isRepeatGame = (GameDataManager.instance.roundClearStatusCheck(playAct, playStage, playRound));

		isSurrenderGame = false;

		// 라운드 게임 정보를 입력.
		isIntro = (rd.id == "INTRO");

		nowRound = rd;
		mapStartPosX = rd.mapStartEndPosX[0];
		mapEndPosX = rd.mapStartEndPosX[1];
		mapPlayerEndPosX = mapEndPosX - 100;

		if(rd.mode == RoundData.MODE.HELL)
		{
			HellModeManager.instance.setStage(rd);
		}
		else
		{
			HellModeManager.instance.setStage(null);
		}

		isPVPMode = false;

		nowPlayingGameType = gameType;


		switch(rd.mode)
		{
		case RoundData.MODE.PVP:
			clearChecker = checkGameClearPVP;
			failChecker = checkGameFailPVP;
			isPVPMode = true;
			break;
		case RoundData.MODE.KILLEMALL:
			clearChecker = checkGameClearKillemAll;
			failChecker = checkGameFailKillemAll;
			break;
		case RoundData.MODE.SURVIVAL:
			clearChecker = checkGameClearSurvival;
			failChecker = checkGameFailSurvival;
			break;
		case RoundData.MODE.PROTECT:
			clearChecker = checkGameClearProtect;
			failChecker = checkGameFailProtect;
			break;
		case RoundData.MODE.SNIPING:
			clearChecker = checkGameClearSniping;
			failChecker = checkGameFailSniping;
			break;
		case RoundData.MODE.KILLCOUNT:
			clearChecker = checkGameClearKillCount;
			failChecker = checkGameFailKillCount;
			break;
		case RoundData.MODE.KILLCOUNT2:
			clearChecker = checkGameClearKillCount2;
			failChecker = checkGameFailKillCount;
			break;
		case RoundData.MODE.ARRIVE:
			clearChecker = checkGameClearArrive;
			failChecker = checkGameFailArrive;
			_prevDist = 99999;
			break;
		case RoundData.MODE.DESTROY:
			clearChecker = checkGameClearDestroy;
			failChecker = checkGameFailDestroy;
			break;			
		case RoundData.MODE.GETITEM:
			clearChecker = checkGameClearGetItem;
			failChecker = checkGameFailGetItem;
			break;

		case RoundData.MODE.B_TEST:
			clearChecker = checkGameClearBtest;
			failChecker = checkGameFailKillemAll;
			break;

		case RoundData.MODE.HELL:
			clearChecker = checkGameClearHell;
			failChecker = checkGameFailHell;
			nowPlayingGameType = GameType.Mode.Hell;
			break;

		default:
			clearChecker = checkGameClear;
			failChecker = checkGameFail;
			break;
		}


		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				clearChecker = GameManager.me.stageManager.dontCheck;
				failChecker = GameManager.me.stageManager.dontCheck;
				return;
			}
		}
		#endif

	}


	public bool dontCheck(int attr = -1, Monster mon = null)
	{
		return false;
	}



	public bool checkGameClearPVP(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(DebugManager.instance.useTagMatchMode == false)
		{
			if(GameManager.me.pvpPlayer.isEnabled == false)
			{
				nowPlayingGameResult = Result.Type.Win;
				GameManager.me.mapManager.clearRound();
				return true;
			}
			else if(attr == ClearChecker.CHECK_TIME && playTime > GameManager.me.stageManager.settingTime)//nowRound.settingTime)
			{
				//			Debug.LogError("PVP 테이블에 시간 체크해봐야 한다!!!");
				
				if(GameManager.me.player.hp > GameManager.me.pvpPlayer.hp)
				{
					nowPlayingGameResult = Result.Type.Win;
					GameManager.me.mapManager.clearRound();
					//return true;
				}
				else
				{
					nowPlayingGameResult = Result.Type.Lose;
					GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_FAILED;
				}
				
				return true;
			}
			
			
			#if UNITY_EDITOR
			else
			{
				if(BattleSimulator.nowSimulation)
				{
					if(DebugManager.instance.pvpDrawAfterTimeOver && GameManager.me.stageManager.playTime >= DebugManager.instance.pvpDrawTime)
					{
						GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_DRAW;
					}
				}
			}
			#endif
		}
		else //========== TAG MATCH 일때 ============= //
		{
			if(GameManager.me.pvpPlayer.isEnabled == false && GameManager.me.battleManager.checkPVPPlayerDead())
			{
				nowPlayingGameResult = Result.Type.Win;
				GameManager.me.mapManager.clearRound();
				return true;
			}
			else if(attr == ClearChecker.CHECK_TIME && playTime > GameManager.me.stageManager.settingTime)//nowRound.settingTime)
			{
				//			Debug.LogError("PVP 테이블에 시간 체크해봐야 한다!!!");
				
				if(GameManager.me.battleManager.checkWinWhenTimeOver())
				{
					nowPlayingGameResult = Result.Type.Win;
					GameManager.me.mapManager.clearRound();
					//return true;
				}
				else
				{
					nowPlayingGameResult = Result.Type.Lose;
					GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_FAILED;
				}
				
				return true;
			}
			
			
			#if UNITY_EDITOR
			else
			{
				if(BattleSimulator.nowSimulation)
				{
					if(DebugManager.instance.pvpDrawAfterTimeOver && GameManager.me.stageManager.playTime >= DebugManager.instance.pvpDrawTime)
					{
						GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_DRAW;
					}
				}
			}
			#endif
		}





		return false;
	}

	
	int leftMonCount = 0;
	public bool checkGameClearKillemAll(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		leftMonCount = GameManager.me.mapManager.bossNum + GameManager.me.mapManager.monUnitNum;

		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			if(leftMonCount == 0)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
			}
		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(leftMonCount <= 1)
//			{
//				nowPlayingGameResult = Result.Type.Clear;
//				GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//				GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//				GameManager.me.cutSceneManager.roundStateCheck();
//				if(CutSceneManager.nowOpenCutScene)return true;
//				GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
//			}
//		}
		
		return false;	
	}

	
	public bool checkGameClearSurvival(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(attr == ClearChecker.CHECK_TIME && playTime > nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Clear;
			GameManager.me.mapManager.clearRound();
			return true;
		}
		
		return false;	
	}	
	
	public bool checkGameClearProtect(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		//* 일정 시간 경과 후 목표 오브젝트 잔존
		if(attr == ClearChecker.CHECK_TIME && playTime > nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Clear;
			GameManager.me.mapManager.clearRound();
			return true;
		}	
		
		return false;	
	}
	
	public bool checkGameClearSniping(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(nowRound.targetHpPer >= 0)
		{
			if(heroMonster[nowRound.targetIndex].hpPer * 100.0f <= nowRound.targetHpPer)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
				return true;
			}
		}
		else if(heroMonster[nowRound.targetIndex].hp <= 0)
		{
			nowPlayingGameResult = Result.Type.Clear;
			GameManager.me.mapManager.clearRound();
			return false;
		}

		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			if(heroMonster[nowRound.targetIndex].isEnabled == false)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
			}
		}

		// 안쓴다.
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(heroMonster[nowRound.targetIndex].isEnabled == false)
//			{
//				nowPlayingGameResult = Result.Type.Clear;
//				GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//				GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//				GameManager.me.cutSceneManager.roundStateCheck();
//				if(CutSceneManager.nowOpenCutScene) return true;
//				GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
//			}
//		}		
		
		return false;	
	}
	
	int leftNum = 0;
	int count = 0;
	public bool checkGameClearKillCount(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		leftNum = 0;
		
		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			foreach(KeyValuePair<string, Xint> kv in GameManager.me.mapManager.leftKilledMonsterNum)
			{
				count = kv.Value;
				if(count < 0) count = 0;
				leftNum += count;
//				Debug.Log("delete now Left: " + kv.Key + " " + kv.Value);
			}
			
//			Debug.Log("leftNum : " + leftNum);
			
			if(leftNum == 0)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
			}
		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(mon.isEnabled == false)
//			{
//				foreach(KeyValuePair<string, Xint> kv in GameManager.me.mapManager.leftKilledMonsterNum)
//				{
//					count = kv.Value;
//					if(count < 0) count = 0;
//					leftNum += count;
////					Debug.Log("imme now Left: " + kv.Key + " " + kv.Value);
//				}
//				
//				if(leftNum == 1)
//				{
//					if(GameManager.me.mapManager.leftKilledMonsterNum.ContainsKey(mon.unitData.id))
//					{
//						if(GameManager.me.mapManager.leftKilledMonsterNum[mon.unitData.id] == 1)
//						{
//							nowPlayingGameResult = Result.Type.Clear;
//							GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//							GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//							GameManager.me.cutSceneManager.roundStateCheck();
//							if(CutSceneManager.nowOpenCutScene) return true;
//							GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
//						}
//					}
//				}
//			}
//		}			
		
		return false;	
	}
	
	
	public bool checkGameClearKillCount2(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		leftNum = 0;
		
		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
//			Debug.LogError("GameManager.me.mapManager.leftKilledMonster : " + GameManager.me.mapManager.leftKilledMonster);
			
			if(GameManager.me.mapManager.leftKilledMonster <= 0)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
			}
		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(mon.isEnabled == false)
//			{
//				if(GameManager.me.mapManager.leftKilledMonster <= 1)
//				{
//					nowPlayingGameResult = Result.Type.Clear;
//					GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//					GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//					GameManager.me.cutSceneManager.roundStateCheck();
//					if(CutSceneManager.nowOpenCutScene) return true;
//					GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;					
//				}
//			}
//		}			
		
		return false;	
	}	
	
	
	private int _tempDist = 99999;
	private int _prevDist = 99999;
	public void updateDistance()
	{
		_tempDist = Mathf.CeilToInt(((float)nowRound.targetPos - GameManager.me.player.cTransformPosition.x) * 0.01f);
		
		if(_tempDist != _prevDist)
		{
			_prevDist = _tempDist;
			GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = _tempDist + "m";
		}
	}

	public const int ARRIVE_DISTANCE_BUFFER = 50;

	public bool checkGameClearArrive(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(attr == ClearChecker.CHECK_TIME)
		{
			updateDistance();
			if(Xfloat.greatEqualThan(  (GameManager.me.player.cTransformPosition.x + StageManager.ARRIVE_DISTANCE_BUFFER).AsFloat() , nowRound.targetPos.Get() ))
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
				GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = "0m";
				return true;	
			}
		}
		
		return false;	
	}
	
	public bool checkGameClearDestroy(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		leftNum = 0;
		
		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			foreach(KeyValuePair<string, Xint> kv in GameManager.me.mapManager.leftDestroyObjectNum)
			{
				count = kv.Value;
				if(count < 0) count = 0;
				leftNum += count;
//				Debug.Log("delete now Left: " + kv.Key + " " + kv.Value);
			}
			
//			Debug.Log("leftNum : " + leftNum);
			
			if(leftNum == 0)
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
			}
		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(mon.npcData != null && mon.isEnabled == false)
//			{
//				foreach(KeyValuePair<string, Xint> kv in GameManager.me.mapManager.leftDestroyObjectNum)
//				{
//					count = kv.Value;
//					if(count < 0) count = 0;
//					leftNum += count;
//				}
//				
//				if(leftNum == 1)
//				{
//					if(GameManager.me.mapManager.leftDestroyObjectNum.ContainsKey(mon.npcData.id))
//					{
//						if(GameManager.me.mapManager.leftDestroyObjectNum[mon.npcData.id] == 1)
//						{
//							nowPlayingGameResult = Result.Type.Clear;
//							GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//							GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//							GameManager.me.cutSceneManager.roundStateCheck();
//							if(CutSceneManager.nowOpenCutScene) return true;
//							GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
//						}
//					}
//				}
//			}
//		}			
		
		return false;	

	}
	
	public bool checkGameClearGetItem(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		leftNum = 0;
		foreach(KeyValuePair<string, Xint> kv in GameManager.me.mapManager.leftGetItemNum)
		{
			count = kv.Value;
			if(count < 0) count = 0;
			leftNum += count;
		}

		GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = leftNum + "";

//		Debug.Log("leftNum : " + leftNum);
		
		if(leftNum == 0)
		{
			nowPlayingGameResult = Result.Type.Clear;
			GameManager.me.mapManager.clearRound();		
		}
		
		return false;	
	}	






	public bool checkGameClearChallengeRun(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;


		// 2분 경과.
		if(attr == ClearChecker.CHECK_TIME)
		{
			GameManager.me.uiManager.uiPlay.challangeModeInfo.updateDistance();

			if(playTime > nowRound.settingTime)
			{
				nowPlayingGameResult = Result.Type.TimeOver;
				GameManager.me.mapManager.clearRound();
				return true;
			}
			else if(GameManager.me.player.cTransformPosition.x >= GameManager.me.mapManager.rankData[2])
			{
				GameManager.me.uiManager.uiPlay.challangeModeInfo.update( GameManager.me.mapManager.rankData[2], GameManager.me.mapManager.rankData[2] + "m");
				GameManager.me.uiManager.uiPlay.challangeModeInfo.rank = 3;
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
				return true;
			}

			// 랭크 거리.
//			else if(GameManager.me.player.cTransformPosition.x >= 4000.0f)
//			{
//				GameManager.me.mapManager.clearRound();
//				return true;	
//			}
		}

		return false;	
	}	


	public bool checkGameClearChallengeSurvival(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(attr == ClearChecker.CHECK_TIME)
		{
			GameManager.me.uiManager.uiPlay.challangeModeInfo.update((int)playTime);

			if(playTime >= GameManager.me.mapManager.rankData[2])
			{
				GameManager.me.uiManager.uiPlay.challangeModeInfo.update( GameManager.me.mapManager.rankData[2]);
				GameManager.me.uiManager.uiPlay.challangeModeInfo.rank = 3;
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.mapManager.clearRound();
				return true;
			}
		}
		
		return false;	
	}	

	public bool checkGameClearChallengeHunt(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		// 2분 경과.
		if(attr == ClearChecker.CHECK_TIME)
		{
			if(playTime > nowRound.settingTime)
			{
				nowPlayingGameResult = Result.Type.TimeOver;
				GameManager.me.mapManager.clearRound();
				return true;
			}
		}

		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			if(GameManager.me.mapManager.killedUnitCount >= GameManager.me.mapManager.rankData[2])
			{
				nowPlayingGameResult = Result.Type.Clear;
				GameManager.me.uiManager.uiPlay.challangeModeInfo.update(GameManager.me.mapManager.killedUnitCount);
				GameManager.me.uiManager.uiPlay.challangeModeInfo.rank = 3;
				GameManager.me.mapManager.clearRound();
			}

		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			if(GameManager.me.mapManager.killedUnitCount + 1 >= GameManager.me.mapManager.rankData[2])
//			{
//				nowPlayingGameResult = Result.Type.Clear;
//				GameManager.me.uiManager.uiPlay.challangeModeInfo.update(GameManager.me.mapManager.killedUnitCount);
//				GameManager.me.uiManager.uiPlay.challangeModeInfo.rank = 3;
//
//				GameManager.me.onCompleteRound(WSDefine.GAME_SUCCESS);
//				GameManager.me.currentScene = Scene.STATE.PLAY_LAST_MONSTER_DIE;
//				GameManager.me.cutSceneManager.roundStateCheck();
//				if(CutSceneManager.nowOpenCutScene) return true;
//				GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
//			}
//		}			

		return false;	
	}	



	public bool checkGameClearBtest(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;
		
		// 2분 경과.
		if(attr == ClearChecker.CHECK_TIME)
		{
		}
		
		if(attr == ClearChecker.CHECK_AFTER_DELETE)
		{
			GameManager.me.uiManager.uiPlay.challangeModeInfo.update(GameManager.me.mapManager.killedUnitCount);
		}
//		else if(attr == ClearChecker.CHECK_IMMEDIATELY)
//		{
//			GameManager.me.uiManager.uiPlay.challangeModeInfo.update(GameManager.me.mapManager.killedUnitCount);
//		}			
		
		return false;	
	}	



	public bool checkGameClearHell(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		HellModeManager.instance.checkClear(attr);

		return false;	
	}



	
	public bool checkGameClear(int attr = -1, Monster mon = null)
	{
		return false;	
	}		


	// ===========//

	public bool checkGameFailPVP(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		//* 아군 히어로 사망
		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Lose;
			return true;
		}
		
		return false;	
	}	


	public bool checkGameFailKillemAll(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		//* 아군 전멸
		//* 제한시간 경과 (옵션)
		
		if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		
		return false;	
	}
	
	public bool checkGameFailSurvival(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		//* 아군 히어로 사망
		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		
		return false;	
	}	
	
	
	bool isFail = false;
	public bool checkGameFailProtect(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}

		//* 오브젝트 파괴		
		isFail = true;
		for(int i = 0; i < protectObjMonCount; ++i)
		{
			if(playerProtectObjectMonster[i].hp > 0)
			{
				isFail = false;
				break;
			}
			else
			{
//				Debug.Log(playerProtectObjectMonster[i] + "  " + playerProtectObjectMonster[i].hp);
			}
		}

		if(isFail) nowPlayingGameResult = Result.Type.Fail;

		return isFail;	
	}
	
	public bool checkGameFailSniping(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}
	
	public bool checkGameFailKillCount(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		//* 아군 전멸
		//* 제한시간 경과 (옵션)		
		
		if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		
		return false;	
	}
	
	public bool checkGameFailArrive(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.stageManager.protectNPC != null && GameManager.me.stageManager.protectNPC.isEnabled == false)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}
	
	public bool checkGameFailDestroy(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.stageManager.protectNPC != null && GameManager.me.stageManager.protectNPC.isEnabled == false)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}
	
	public bool checkGameFailGetItem(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}

		return false;	
	}	


	public bool checkGameFailChallengeRun(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}


	public bool checkGameFailChallengeSurvival(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}

	public bool checkGameFailChallengeHunt(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}



	public bool checkGameFailHell(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;
		
		if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			HellModeManager.instance.hellClearType = Result.Type.Fail;
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(HellModeManager.instance.timeFailCheck())
		{
			HellModeManager.instance.hellClearType = Result.Type.TimeOver;
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		return false;	
	}


	
	public bool checkGameFail(int attr = -1, Monster mon = null)
	{
		if(GameManager.me.isPlaying == false) return false;

		if(nowRound.settingTime > -1 && playTime >= nowRound.settingTime)
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}
		else if(GameManager.me.player.hp <= 0 && GameManager.me.battleManager.checkMyPlayerDead())
		{
			nowPlayingGameResult = Result.Type.Fail;
			return true;
		}

		return false;	
	}		
}


public class ClearChecker
{
	public const int CHECK_AFTER_DELETE = 0;
	public const int CHECK_IMMEDIATELY = 1;
	
	public const int CHECK_TIME = 2;
	
}



public class GameType
{
	public enum Mode
	{
		Epic, Challenge, Championship, Friendly, Hell, Sigong
	}

	public enum Result
	{
		Win, Lose, Clear, Fail, TimeOver
	}

}


public class Result
{
	public class Type
	{
		public const int GameOver = 3150;
		public const int TimeOver = 3151;
		public const int Clear = 3152; 
		public const int Lose = 3153;
		public const int Win = 3154;
		public const int Fail = 3155;
		public const int Perfect = 3156;
		public const int Finish = 3157;
		public const int Go = 3158;
		public const int Success = 3159;
		public const int GiveUp = 3160;
	}
}

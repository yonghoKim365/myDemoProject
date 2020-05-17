using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour {


	public List<string> prevLoadingMonsterResource = new List<string>();

	public List<string> clearExceptionEffects = new List<string>();

	void loadPlayerUnits(GamePlayerData gpd, bool isResourceType = false)
	{
		MonsterData md;

		if(gpd == null) return;

		if(isResourceType == false)
		{
			if(gpd.units != null)
			{
				foreach(string unitId in gpd.units)
				{
					if(string.IsNullOrEmpty( unitId ) == false)
					{
						GameIDData eUnitInfo = new GameIDData();
						eUnitInfo.parse(unitId, GameIDData.Type.Unit);
						md = GameManager.info.monsterData[eUnitInfo.unitData.resource];
						gameDataManager.addLoadModelData(md);
					}
				}
			}
		}
		else
		{
			
			if( gpd.unitResourceId != null)
			{
				foreach(string unitId in gpd.unitResourceId)
				{
					if(string.IsNullOrEmpty( unitId ) == false)
					{
						md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
						gameDataManager.addLoadModelData(md);
						
					}
				}
			}
		}

	}

	//PreLoad Model Data!!!
	public void loadRoundMonsterModelData(bool nowCheckExceptionEffect = false)
	{
		MonsterData md;

		prevLoadingMonsterResource.Clear();
		
#if UNITY_EDITOR
		Debug.Log("== preload: enemy heromonsters");
#endif
//		setGuiLog("== preload: enemy heromonsters");

		if(player != null)
		{
			loadPlayerUnits(player.playerData, true);
		}

		if(stageManager.nowRound.mode == RoundData.MODE.PVP)
		{
			if(playMode == PlayMode.replay)
			{
				loadPlayerUnits(GameDataManager.replayAttackerData);
				loadPlayerUnits(GameDataManager.replayAttacker2Data);
			}
			else
			{
				if(DebugManager.instance.useTagMatchMode && GameDataManager.instance.getSubHeroData() != null)
				{
					loadPlayerUnits(GameDataManager.instance.heroes[GameDataManager.instance.selectSubHeroId]);
				}
			}

			loadPlayerUnits(DebugManager.instance.pvpPlayerData, true);
			loadPlayerUnits(DebugManager.instance.pvpPlayerData2, true);
		}

		gameDataManager.loadUnitSlotModelData(false);

		if(stageManager.nowRound.heroMonsters != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.heroMonsters)
			{
#if UNITY_EDITOR
				Debug.Log(smd.id);

				try
				{
					md = GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource];
				}
				catch
				{
					Debug.Log(GameManager.info.heroMonsterData[smd.id].resource);
					Debug.Log(GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource]);
				}

#endif
				md = GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource];
				gameDataManager.addLoadModelData(md);


				if(smd.units != null)
				{
					foreach(string unitId in smd.units)
					{

						#if UNITY_EDITOR
						try
						{
							md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
						}
						catch
						{
							Debug.Log(unitId);
							Debug.Log(GameManager.info.unitData[unitId].resource);
						}
						
						#endif


						md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
						gameDataManager.addLoadModelData(md.resource,md,md);

					}
				}
			}
		}
		
#if UNITY_EDITOR
		Debug.Log("== preload: enemy units");
#endif		
//		setGuiLog("== preload: enemy units");

		if(stageManager.nowRound.unitMonsters != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.unitMonsters)
			{
				md = GameManager.info.monsterData[GameManager.info.unitData[smd.id].resource];
				gameDataManager.addLoadModelData(md.resource,md,md);

			}		
		}
		
#if UNITY_EDITOR
		Debug.Log("== preload: protect object");
#endif				
//		setGuiLog("== preload: protect object");

		if(stageManager.nowRound.protectObject != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.protectObject)
			{
				md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
				gameDataManager.addLoadModelData(md);

			}		
		}	
		
#if UNITY_EDITOR
		Debug.Log("== preload: destroyObject");
#endif				
//		setGuiLog("== preload: destroyObject");
		
		if(stageManager.nowRound.destroyObject != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.destroyObject)
			{
				md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
				gameDataManager.addLoadModelData(md);

			}		
		}		
		
#if UNITY_EDITOR
		Debug.Log("== preload: decoObject");
#endif				
//		setGuiLog("== preload: decoObject");
		
		if(stageManager.nowRound.decoObject != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.decoObject)
			{
				md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
				gameDataManager.addLoadModelData(md);

			}		
		}	


#if UNITY_EDITOR
Debug.Log("== preload: blockObject");
#endif				
		
		
		if(stageManager.nowRound.blockObject != null)
		{
			foreach(StageMonsterData smd in stageManager.nowRound.blockObject)
			{
				md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
				gameDataManager.addLoadModelData(md);

			}		
		}

		
#if UNITY_EDITOR
		Debug.Log("== preload: invisibleHeroMonster");
#endif				
//		setGuiLog("== preload: invisibleHeroMonster");
		
		if(stageManager.nowRound.invisibleHeroMonster != null)
		{
			
			md = GameManager.info.monsterData[GameManager.info.heroMonsterData[stageManager.nowRound.invisibleHeroMonster.id].resource];
			gameDataManager.addLoadModelData(md);

			
			if(stageManager.nowRound.invisibleHeroMonster.units != null)
			{
				foreach(string unitId in stageManager.nowRound.invisibleHeroMonster.units)
				{
					md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
					gameDataManager.addLoadModelData(md.resource,md,md);

				}
			}			
		}			
		
		
#if UNITY_EDITOR
		Debug.Log("== preload: chaser");
#endif				
//		setGuiLog("== preload: chaser");
		
		if(stageManager.nowRound.chaser != null)
		{
			md = GameManager.info.monsterData[GameManager.info.npcData[stageManager.nowRound.chaser.id].resource];
			gameDataManager.addLoadModelData(md);

		}			



		#if UNITY_EDITOR
		Debug.Log("== preload: challange mode");
		#endif				
//		setGuiLog("== preload: challange mode");
		
		if(stageManager.nowRound.challengeData != null)
		{
			if(stageManager.nowRound.challengeData != null)
			{
				switch(stageManager.nowRound.challengeData.type)
				{
				case StageMonsterData.Type.NPC:
					md = GameManager.info.monsterData[GameManager.info.npcData[stageManager.nowRound.challengeData.id].resource];
					gameDataManager.addLoadModelData(md);

					break;
				case StageMonsterData.Type.UNIT:
					md = GameManager.info.monsterData[GameManager.info.unitData[stageManager.nowRound.challengeData.id].resource];
					gameDataManager.addLoadModelData(md.resource, md, md);

					break;
				case StageMonsterData.Type.HERO:
					md = GameManager.info.monsterData[GameManager.info.heroMonsterData[stageManager.nowRound.challengeData.id].resource];
					gameDataManager.addLoadModelData(md);

					break;
				}
			}

			if(stageManager.nowRound.units != null)
			{
				foreach(string un in stageManager.nowRound.units)
				{
					//				Debug.LogError("un : " + un);
					md = GameManager.info.monsterData[GameManager.info.unitData[un].resource];
					gameDataManager.addLoadModelData(md.resource, md , md);

				}
			}
		}	

		
#if UNITY_EDITOR
		Debug.Log("== preload: cutScene monsters");
#endif			
//		setGuiLog("== preload: cutScene monsters");

		bool loadCutScene = !cutSceneManager.canSkipCutScene();

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
#endif
		{

#if UNITY_EDITOR
			if(DebugManager.instance.useDebug)
			{
				if(UnitSkillCamMaker.instance.useUnitSkillCamMaker || CutSceneMaker.instance.useCutSceneMaker || CutSceneMakerForDesigner.instance.useCutSceneMaker) loadCutScene = true;
			}
#endif

			{
				if(string.IsNullOrEmpty(stageManager.getCutSceneId()) == false)
				{
					string[] cids = stageManager.getCutSceneId().Split(',');
					
					CutSceneData rcd;
					
					foreach(string id in cids)
					{
						#if UNITY_EDITOR
						Debug.Log(id);
#endif
						clientDataLoader.loadAndAddCutSceneData(id);

						if(loadCutScene)
						{
							rcd = GameManager.info.cutSceneData[id];
							foreach(string mid in rcd.loadMonsterIds)
							{
//													Debug.LogError(mid);
								gameDataManager.addLoadModelData(GameManager.info.monsterData[mid]);

								effectManager.loadEffectFromMonsterData(mid);
							}
							
							if(useAssetBundleMapFile)
							{
								foreach(int mapId in rcd.loadMapIds)
								{
									if(GameManager.info.mapData.ContainsKey(mapId) == false)
									{
										Debug.LogError(mapId + "////////////////////////");

										foreach(KeyValuePair<int, MapData> kv in GameManager.info.mapData)
										{
											Debug.Log("id:"+kv.Key);
										}
									}

									gameDataManager.addLoadMapData( GameManager.info.mapData[mapId].resource );
								}
							}
							
							
							foreach(string sid in rcd.loadSoundIds)
							{
//								Debug.LogError(sid);
								SoundManager.instance.addLoadSoundData(sid);
							}


							foreach(string eid in rcd.loadEffectIds)
							{
//								Debug.LogError(eid);
								effectManager.addLoadEffectData(eid);
							}


						}
					}
				}

			}
		}





//		setGuiLog(" useAssetBundleMapFile : " + useAssetBundleMapFile);

		if(useAssetBundleMapFile)
		{
			int loadingMapId = stageManager.getMapId( UIPlay.playerLeagueGrade);

			if(loadingMapId > 0 && GameManager.info.mapData.ContainsKey(loadingMapId))
			{
				gameDataManager.addLoadMapData( GameManager.info.mapData[loadingMapId].resource );
			}
		}

		System.GC.Collect();

		loadBattleBGM();

		effectManager.preloadRoundEffect(stageManager.nowRound, nowCheckExceptionEffect);

		gameDataManager.startModelLoad(true);
		gameDataManager.startMapLoad(true);
		SoundManager.instance.startLoadSounds(true);
		effectManager.startLoadEffects(true);
	}



	void loadBattleBGM()
	{
		if(stageManager.nowPlayingGameType == GameType.Mode.Epic)
		{
			switch( stageManager.playRound )
			{
			case 1:
				SoundData.nowPlayingBattleBgm = "bgm_battle_a";
				break;
			case 2:
				SoundData.nowPlayingBattleBgm = "bgm_battle_b";
				break;
			case 3:
				SoundData.nowPlayingBattleBgm = "bgm_battle_c";
				break;
			case 4:
				SoundData.nowPlayingBattleBgm = "bgm_battle_d";
				break;
			case 5:
				SoundData.nowPlayingBattleBgm = "bgm_battle_e";
				break;
			}
		}
		else if(stageManager.nowPlayingGameType == GameType.Mode.Hell)
		{
			int r = HellModeManager.instance.roundIndex;

			if(recordMode ==  GameManager.RecordMode.continueGame)
			{
				if(r < 5) SoundData.nowPlayingBattleBgm = "bgm_battle_a";
				else if(r < 10) SoundData.nowPlayingBattleBgm = "bgm_battle_b";
				else if(r < 15) SoundData.nowPlayingBattleBgm = "bgm_battle_c";
 				else if(r < 20) SoundData.nowPlayingBattleBgm = "bgm_battle_d";
				else SoundData.nowPlayingBattleBgm = "bgm_battle_e";
			}
			else
			{
				switch(r)
				{
				case 1:
					SoundData.nowPlayingBattleBgm = "bgm_battle_a";
					break;
				case 6:
					SoundData.nowPlayingBattleBgm = "bgm_battle_b";
					break;
				case 11:
					SoundData.nowPlayingBattleBgm = "bgm_battle_c";
					break;
				case 16:
					SoundData.nowPlayingBattleBgm = "bgm_battle_d";
					break;
				case 21:
					SoundData.nowPlayingBattleBgm = "bgm_battle_e";
					break;
				}

			}
		}
		else if(stageManager.nowPlayingGameType == GameType.Mode.Championship)
		{
			switch(uiManager.popupChampionshipResult.matchNumber)
			{
			case 1:
				if( UnityEngine.Random.Range(0,10) >= 5) SoundData.nowPlayingBattleBgm = "bgm_battle_e";
				else SoundData.nowPlayingBattleBgm = "bgm_battle_d";
				break;
			case 2:
				if( UnityEngine.Random.Range(0,10) >= 5) SoundData.nowPlayingBattleBgm = "bgm_battle_c";
				else SoundData.nowPlayingBattleBgm = "bgm_battle_b";
				break;
			case 3:
				SoundData.nowPlayingBattleBgm = "bgm_battle_a";
				break;
			}
		}
		else  if(stageManager.nowPlayingGameType == GameType.Mode.Friendly)
		{
			SoundData.nowPlayingBattleBgm = "bgm_battle_a";
		}
		else if(stageManager.nowPlayingGameType == GameType.Mode.Sigong)
		{
			if(stageManager.sigongData != null)
			{
				SoundData.nowPlayingBattleBgm = stageManager.sigongData.bgm;
			}
		}
		else
		{
			SoundData.nowPlayingBattleBgm = null;
		}

		if(string.IsNullOrEmpty( SoundData.nowPlayingBattleBgm ) == false)
		{
			SoundManager.instance.addLoadSoundData(SoundData.nowPlayingBattleBgm);
		}
	}

	
}


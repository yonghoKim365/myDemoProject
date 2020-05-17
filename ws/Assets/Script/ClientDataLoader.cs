using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using LitJson;
#endif

using System.IO;


public class ClientDataLoader : MonoBehaviour
{
	public const string NAME = "name";
	public const string CLIENT_DATA_PATH = "clientdata/";
	
	public ClientDataLoader ()
	{
	}
	
	public delegate void Callback();
	Callback _callback;
	
	public void init(Callback cb)
	{
		_callback = cb;
		AssetBundleManager.instance.makeDownloadFileList();
	}

	public void onCompleteResourceCheck()
	{
		UIManager.setGameState( Util.getUIText("DRAW_BG") , 0.82f);
		StartCoroutine(loadDatas());
	}
	
	
	IEnumerator loadDatas()
	{
		System.GC.Collect();
		yield return null;

		loadSoundData("data_sound_client");
		loadModelData("data_modeldata_client");

		loadLoadingScreenData("data_loading_client");

		loadAniData("data_anidata_client");
		loadSkillEffectSetupData("data_skilleffectsetup_client");		
		loadSetupData("data_setup_client");
		UIManager.setGameState( Util.getUIText("DRAW_BG")+" 12%" );
		loadLoadingTipData("data_tip_client");

		loadUnitIconIndexData("data_uniticon_client");
		loadSkillIconIndexData("data_skillicon_index_client");
		loadEquipIconIndexData("data_equipicon_index_client");


		loadBaseUnitData("data_baseunitrune_client");
		loadRareUnitData("data_unitrune_client");
//		UIManager.setGameState( "게임데이터 로딩중 24%" );
		loadBaseMonsterUnitData("data_e_unit_client");
		loadRareMonsterUnitData("data_e_unit_rare_client");
		loadSkillIconData("data_skillicon_client");
		loadUnitSkillData("data_unit_skill_client");
		loadHeroSkillData("data_baseskillrune_client");

		loadActData("data_act_client");
		loadStageData("data_stage_client");
		loadRoundData("data_round_client");
		System.GC.Collect();
		yield return null;

		UIManager.setGameState( Util.getUIText("DRAW_BG") + " 48%" , 0.83f);
		loadBulletPattern("data_bulletpattern_client");
		loadMonsterData("data_monster_client");
		loadHeroMonsterData("data_undead_hero_client");
		loadHeroMonsterAI("data_heromon_ai_client");
		loadMapObjectData("data_mapobject_client");
		loadPlayerData("data_hero_leo_client");
		loadTextData("data_text_client");
		loadUITextData("data_uitext_client");

		loadTutorialInfoData("data_tutorialinfo_client");
		loadTutorialData("data_tutorial_client");
		loadBulletData("data_bullet_client");
		loadBaseHeroPartsData("data_equip_base_client");
		loadHeroPartsData("data_equipment_client");
		loadMapData("data_map_client");
		System.GC.Collect();
		yield return null;
		UIManager.setGameState( Util.getUIText("DRAW_BG") + "85%" );
		loadNPCData("data_npc_client");
		loadEffectData("data_effect_client");
		loadCutSceneData("data_cs_client");
		loadUnitSkillCamData("data_us_client");
		loadPlayerAiData("data_playerai_client");
		loadAiGroupData("data_ai_group_client");
		loadFaceAniData("data_faceani_client");
		loadLobbyPosition("data_lobby_client");
		loadAdviceData("data_advice_client");
		loadTestModeData("data_testmode_client");

		loadVersionData("data_version_client");

		loadTranscendData("data_reforge_client");

		loadHellSetupData("data_hellsetup_client");

		loadTestSigongData("data_sigong_test_client");

		if(GameManager.info.dataStreamDic != null)
		{
			GameManager.info.dataStreamDic.Clear();
		}

		yield return null;
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		UIManager.setGameState( Util.getUIText("DRAW_BG") + " 100%" , 0.84f);
		CharacterEffect.knockBackData = SkillEffectData.getSkillEffectData(25,300,0);

		CharacterEffect.stunData = SkillEffectData.getSkillEffectData(30,GameManager.info.setupData.tagStunTime.Get());

		//ErrorTest
#if UNITY_EDITOR	
		yield return StartCoroutine( checkError() );
		Debug.LogError("== errorCheck Complete ==");
#endif	

		GameDataManager.instance.makeDefaultGamePlayerData();

		if(_callback != null) _callback();


//		Debug.LogError("cutSceneData : " + Util.checkObjectMemorySize(GameManager.info.cutSceneData));

	}
	
	
	

#if UNITY_EDITOR	
	private List<string> checkLogics = new List<string>();
	
	IEnumerator checkError()
	{
		Util.stringBuilder.Length = 0;

		foreach(string str in HeroSkillData.error)
		{
			Util.stringBuilder.Append(str + "\n");
		}

		Debug.Log(Util.stringBuilder.ToString());

		Util.stringBuilder.Length = 0;

		HeroSkillData.error.Clear();


		foreach(KeyValuePair<string, MonsterData> kv in GameManager.info.monsterData)
		{
			if(kv.Value.effectData != null)
			{
				foreach(AttachedEffectData ade in kv.Value.effectData)
				{
					if(string.IsNullOrEmpty(ade.id) == false && GameManager.info.effectData.ContainsKey(ade.id) == false)
					{
						Debug.LogError(kv.Key + " monsterData Effect Error!   " + ade.id);
					}
				}
			}
		}

		foreach(KeyValuePair<string, BulletData> kv in GameManager.info.bulletData)
		{
			BulletData bd = kv.Value;

			if(bd.effectData == null) continue;

			foreach(AttachedEffectData aed in bd.effectData)
			{
				if(string.IsNullOrEmpty(aed.id) == false && GameManager.info.effectData.ContainsKey(aed.id) == false)
				{
					Debug.LogError(bd.id + " Bullet Effect Error!   " + aed.id);
				}
			}

			if(string.IsNullOrEmpty(bd.destroyEffectId) == false && GameManager.info.effectData.ContainsKey(bd.destroyEffectId) == false)
			{
				Debug.LogError(bd.id + "destroyEffectId Effect Error!  " + bd.destroyEffectId);
			}

			if(string.IsNullOrEmpty(bd.hitEffect) == false && GameManager.info.effectData.ContainsKey(bd.hitEffect) == false)
			{
				Debug.LogError(bd.id + "hitEffect Effect Error!  " + bd.hitEffect);
			}
		}


		foreach(KeyValuePair<string, Dictionary<string,AniData>> kv in GameManager.info.aniData)
		{
			Dictionary<string, AniData> d = kv.Value;

			foreach(KeyValuePair<string, AniData> kv2 in d)
			{
				AniData ad = kv2.Value;

				if(ad.effect != null)
				{
					foreach(AniDataEffect aed in ad.effect)
					{
						if(string.IsNullOrEmpty(aed.id) == false && GameManager.info.effectData.ContainsKey(aed.id) == false)
						{
							Debug.LogError(ad.id + " AniDataEffect Effect Error!  " + aed.id);
						}
					}
				}

			}
		}



		foreach(KeyValuePair<string, UnitSkillData> kv in GameManager.info.unitSkillData)
		{
			UnitSkillData ud = kv.Value;

			if(string.IsNullOrEmpty(ud.activeSkillEffect) == false && GameManager.info.effectData.ContainsKey(ud.activeSkillEffect) == false)
			{
				Debug.LogError(ud.id + " ud.activeSkillEffect Error!  " + ud.activeSkillEffect);
			}
		}


		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.gameResourceErrorCheck)
		{
			foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
			{
				if(string.IsNullOrEmpty(kv.Value.cutSceneId)) continue;
				string[] rds = kv.Value.cutSceneId.Split(',');

				for(int i = 0; i < rds.Length; ++i)
				{
//					Debug.Log(kv.Key + " : " + kv.Value.cutSceneId);
					loadAndAddCutSceneData(rds[i]);
				}
			}

			foreach(KeyValuePair<string, SkillIconData> sd in GameManager.info.skillIconData)
			{
				if(string.IsNullOrEmpty(sd.Value.soundId) == false && GameManager.info.soundData.ContainsKey(sd.Value.soundId) == false)
				{
					Debug.LogError("== 스킬 사운드 없음 : " + sd.Value.soundId + "   " + sd.Value.id);
				}
			}

			string serror = "";
			string merror = "";
			string cerror = "";
			string eerror = "";

			foreach(KeyValuePair<string, CutSceneData> cd in GameManager.info.cutSceneData)
			{
				foreach(string s in cd.Value.loadSoundIds)
				{
					if(GameManager.info.soundData.ContainsKey(s) == false)
					{
						serror += ("=== 사운드 없음 : " + s + "   " + cd.Value.id + "\r\n");
					}
				}

				foreach(int m in cd.Value.loadMapIds)
				{
					if(GameManager.info.mapData.ContainsKey(m) == false)
					{
						merror += ("=== 맵 없음 : " + m + "   " + cd.Value.id + "\r\n");
					}
				}

				foreach(string m in cd.Value.loadMonsterIds)
				{
					if(GameManager.info.monsterData.ContainsKey(m) == false)
					{
						merror += ("=== 몬스터 없음 : " + m + "   " + cd.Value.id + "\r\n");
					}
				}

				foreach(string e in cd.Value.loadEffectIds)
				{
					if(GameManager.info.effectData.ContainsKey(e) == false)
					{
						eerror += ("=== 이펙트 없음 : " + e + "   " + cd.Value.id + "\r\n");
					}
				}
			}

			foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseUnitData)
			{
				if(GameManager.info.monsterData.ContainsKey(kv.Value.resource) == false)
				{
					Debug.LogError(" 몬스터데이터가 없음 : " + kv.Value.resource);
				}
				else if( GameManager.info.unitIconIndexData.ContainsKey( GameManager.info.monsterData[kv.Value.resource].resource ) == false)
				{
					Debug.LogError(" 아이콘이 없음!@ : " + GameManager.info.monsterData[kv.Value.resource].resource);
				}
			}

			foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseMonsterUnitData)
			{
				if(GameManager.info.monsterData.ContainsKey(kv.Value.resource) == false)
				{
					Debug.LogError(" 몬스터데이터가 없음 : " + kv.Value.resource);
				}
			}


			foreach(KeyValuePair<string, RoundData> kv  in GameManager.info.roundData)
			{

				RoundData nowRound = kv.Value;

				if(nowRound.mode == RoundData.MODE.ARRIVE)
				{
					if(nowRound.targetPos.Get() + 300f > nowRound.mapStartEndPosX[1].Get())
					{
						Debug.LogError(nowRound.id+" Arrive 모드는 맵 길이가 300이 더 길어야함!");
					}
				}


				if(nowRound.heroMonsters != null && kv.Value.mode != RoundData.MODE.PVP && kv.Key != "CS_TEST")
				{
					MonsterData md;
					
					foreach(StageMonsterData smd in nowRound.heroMonsters)
					{
						try
						{
							md = GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource];
						}
						catch
						{
							Debug.LogError(nowRound.id +  "error : " + smd.id);
						}


						if(smd.skills != null)
						{
							HeroSkillData hsd;

							foreach(string str in smd.skills)
							{
								if(GameManager.info.heroSkillData.TryGetValue(str, out hsd))
								{
									if(GameManager.info.bulletPatternData.ContainsKey(hsd.resource) == false)
									{
										if(hsd.exeData.type == 15)
										{
											Debug.LogError( nowRound.id + "error : " + smd.id + " - " + str + "  doesn't have bullet resource"   );
										}
										else if(hsd.exeData.type > 2)
										{
											Debug.Log( nowRound.id + "error : " + smd.id + " - " + str + "  doesn't have bullet resource"   );
										}
									}
								}
								else
								{
									GameIDData.dummySkill.parse(str);

									try
									{
										if(GameManager.info.bulletPatternData.ContainsKey(GameIDData.dummySkill.skillData.resource) == false)
										{
											if(GameIDData.dummySkill.skillData.exeType == 15)
											{
												Debug.LogError( nowRound.id + "error : " + smd.id + " - " +GameIDData.dummySkill.skillData.resource + " - " + str + "  doesn't have bullet resource"   );
											}
											else if(GameIDData.dummySkill.skillData.exeType > 2)
											{
												Debug.Log( nowRound.id + "error : " + smd.id + " - " +GameIDData.dummySkill.skillData.resource + " - " + str + "  doesn't have bullet resource"   );
											}
										}
									}
									catch
									{
										Debug.LogError(nowRound.id + "error : " + smd.id + " - " + str + " : heroskill doesn't exist  - Monster AI");
									}

								}
							}
						}

						foreach(string smdAi in smd.ai)
						{
							MonsterHeroAI ai;

							if(GameManager.info.heroMonsterAI.TryGetValue(smdAi, out ai))
							{
								if(ai.actionSkill != null && ai.actionSkill[0] > -1)
								{
									try
									{
										string hsk = smd.skills[ai.actionSkill[0]];
									}
									catch
									{
										Debug.LogError(nowRound.id + "error : " + smd.id + " - " + ai.actionSkill[0] + " : ai.actionSkill - Monster AI");
									}
								}
								
								if(ai.actionSkill2 > -1)
								{
									try
									{
										string hsk = smd.skills[ai.actionSkill2];
									}
									catch
									{
										Debug.LogError(nowRound.id + "error : " + smd.id + " - " + ai.actionSkill2 + " : ai.actionSkill2 - Monster AI");
									}
								}

							}
							else
							{
								Debug.LogError(nowRound.id + "error : " + smd.id + " - " + smdAi + " : heroskilldata doesn't exist  - Monster AI");
							}


							//ai.skillNum
						}

						if(smd.units != null)
						{
							foreach(string unitId in smd.units)
							{
								try
								{
									md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
								}
								catch
								{
									Debug.LogError(nowRound.id +  "error : " + smd.id + "  unit error: " + unitId);
								}
							}
						}
					}


					if(nowRound.unitMonsters != null)
					{
						foreach(StageMonsterData smd in nowRound.unitMonsters)
						{
							try
							{
								md = GameManager.info.monsterData[GameManager.info.unitData[smd.id].resource];
							}
							catch
							{
								Debug.LogError(nowRound.id +  "unitMonsters error : " + smd.id);
							}
						}		
					}



					if(nowRound.protectObject != null)
					{
						foreach(StageMonsterData smd in nowRound.protectObject)
						{
							try
							{
								md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
							}
							catch
							{
								Debug.LogError(nowRound.id +  "protectObject error : " + smd.id);
							}
						}		
					}	


					if(nowRound.destroyObject != null)
					{
						foreach(StageMonsterData smd in nowRound.destroyObject)
						{
							try
							{
								md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
							}
							catch
							{
								Debug.LogError(nowRound.id +  "destroyObject error : " + smd.id);
							}

						}		
					}	


					if(nowRound.decoObject != null)
					{
						foreach(StageMonsterData smd in nowRound.decoObject)
						{


							try
							{
								md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
							}
							catch
							{
								Debug.LogError(nowRound.id +  "decoObject error : " + smd.id);
							}

							
						}		
					}	


					if(nowRound.blockObject != null)
					{
						foreach(StageMonsterData smd in nowRound.blockObject)
						{
							try
							{
								md = GameManager.info.monsterData[GameManager.info.npcData[smd.id].resource];
							}
							catch
							{
								Debug.LogError(nowRound.id +  "blockObject error : " + smd.id);
							}
						}		
					}


					if(nowRound.invisibleHeroMonster != null)
					{

						try
						{
							md = GameManager.info.monsterData[GameManager.info.heroMonsterData[nowRound.invisibleHeroMonster.id].resource];
						}
						catch
						{
							Debug.LogError(nowRound.id +  "invisibleHeroMonster error : " + nowRound.invisibleHeroMonster.id);
						}


						
						if(nowRound.invisibleHeroMonster.units != null)
						{
							foreach(string unitId in nowRound.invisibleHeroMonster.units)
							{

								try
								{
									md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
								}
								catch
								{
									Debug.LogError(nowRound.id +  "invisibleHeroMonster unit error : " + unitId);
								}
							}
						}			
					}


					if(nowRound.chaser != null)
					{
						try
						{
							md = GameManager.info.monsterData[GameManager.info.npcData[nowRound.chaser.id].resource];
						}
						catch
						{
							Debug.LogError(nowRound.id +  "chaser error : " + nowRound.chaser.id);
						}
					}	

				}
			}


			GameIDData gd = new GameIDData();

			foreach(KeyValuePair<string, TestModeData> kv in GameManager.info.testModeData)
			{
				TestModeData td = kv.Value;

				if(td.hero != Character.LEO && td.hero != Character.KILEY && td.hero != Character.CHLOE)
				{
					Debug.LogError(td.id + "   없는 히어로 : " + td.hero);
				}

				try
				{
					gd.parse(td.head);
					string hid = gd.partsData.id;
				}
				catch
				{
					Debug.LogError(td.head + " 없는 장비");
				}

				try
				{
					gd.parse(td.body);
					string bid = gd.partsData.id;
				}
				catch
				{
					Debug.LogError(td.body + " 없는 장비");
				}

				try
				{
					gd.parse(td.weapon);
					string hid = gd.partsData.id;
				}
				catch
				{
					Debug.LogError(td.weapon + " 없는 장비");
				}

				try
				{
					gd.parse(td.vehicle);
					string vid = gd.partsData.id;
				}
				catch
				{
					Debug.LogError(td.vehicle + " 없는 장비");
				}


				try
				{
					gd.parse(td.u1);
					string u1 = gd.unitData.id;
				}
				catch
				{
					Debug.LogError(td.u1 + " 없는 u1");
				}

				try
				{
					gd.parse(td.u2);
					string u2 = gd.unitData.id;
				}
				catch
				{
					Debug.LogError(td.u2 + " 없는 u2");
				}

				try
				{
					gd.parse(td.u3);
					string u3 = gd.unitData.id;
				}
				catch
				{
					Debug.LogError(td.u3 + " 없는 u3");
				}

				try
				{
					gd.parse(td.u4);
					string u4 = gd.unitData.id;
				}
				catch
				{
					Debug.LogError(td.u4 + " 없는 u4");
				}

				try
				{
					gd.parse(td.u5);
					string u5 = gd.unitData.id;
				}
				catch
				{
					Debug.LogError(td.u5 + " 없는 u5");
				}


				try
				{
					gd.parse(td.s1);
					string s1 = gd.skillData.id;
				}
				catch
				{
					Debug.LogError(td.s1 + " 없는 s1");
				}

				try
				{
					gd.parse(td.s2);
					string s2 = gd.skillData.id;
				}
				catch
				{
					Debug.LogError(td.s2 + " 없는 s2");
				}

				try
				{
					gd.parse(td.s3);
					string s3 = gd.skillData.id;
				}
				catch
				{
					Debug.LogError(td.s3 + " 없는 s3");
				}

			}


			foreach(KeyValuePair<string, HeroSkillData> kv in GameManager.info.heroSkillData)
			{
				if(GameManager.info.skillIconData.ContainsKey(kv.Value.resource) == false && kv.Value.isBook)
				{
					Debug.LogError(kv.Key + " 스킬 아이콘이 없음." );
				}
			}

			//return GameManager.info.heroPartsDic[resourceId].iconByRare(rare);

			//MonsterData.setUnitIcon( GameManager.info.monsterData[GameManager.info.unitData[resourceId].resource], targetSprite, defaultDepth );

			//GameManager.info.skillIconData[GameManager.info.heroSkillData[resourceId].resource].icon;



			if(serror.Length > 0) Debug.LogError(serror);
			if(merror.Length > 0) Debug.LogError(merror);
			if(cerror.Length > 0) Debug.LogError(cerror);
			if(eerror.Length > 0) Debug.LogError(eerror);

			yield return null;
		}



		yield return null;
	}
#endif	
	
	
	
	
	
	public void loadSetupData(string fileName)
	{
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.

		SetupData data = new SetupData();
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				switch( ((string)list[k["ID"]]) )
				{
					case "PLAYER_DAMAGE_COOLTIME":
					Util.parseObject(list[k["VALUE"]], out data.playerHitCooltime, true, 0.0f);
					break;


					
					case "HP_BAR_SHOWTIME":
					Util.parseObject(list[k["VALUE"]], out data.hpBarShowTime, true, 1.0f);				
					break;			

					case "RECOVERY_DELAY":
					float rd = 0;
					Util.parseObject(list[k["VALUE"]], out rd, true, 1.3f);				
					data.recoveryDelay = rd;

					break;		

					case "TAG_RECOVERY_DELAY_SP":
					float wsp = 0;
					Util.parseObject(list[k["VALUE"]], out wsp, true, 2.6f);				
					data.waitPlayerRecoveryDelaySp = wsp;
					break;	

					case "TAG_RECOVERY_DELAY_MP":
					float wmp = 0;
					Util.parseObject(list[k["VALUE"]], out wmp, true, 2.6f);				
					data.waitPlayerRecoveryDelayMp = wmp;
					break;	

					case "TAG_RECOVERY_HPPER":
					float whpper = 0;
					Util.parseObject(list[k["VALUE"]], out whpper, true, 0.01f);				
					data.waitPlayerRecoveryHpPer = whpper;
					break;	

					
					case "DEFAULT_HP":
					Util.parseObject(list[k["VALUE"]], out data.defaultHp, true, 200);				
					break;	
					
					case "DEFAULT_MP":
					Util.parseObject(list[k["VALUE"]], out data.defaultMp, true, 0);				
					break;	
					
					case "DEFAULT_SP":
					Util.parseObject(list[k["VALUE"]], out data.defaultSp, true, 200);				
					break;	
					
					case "USE_HP_AS_SP":
						data.useHpAsSp = (((string)list[k["VALUE"]]).Equals("Y"));							
					break;
				
					
					case "PLAY_CAM_SPRING_VALUE":
						data.defaultPlayCamSpringValue = Util.stringToFloatArray((string)list[k["VALUE"]],',');
						break;	
					case "DEFAULT_CAMERA":
					
						string[] dc = ((string)list[k["VALUE"]]).Split('&');
						data.cameraPreset = new CameraInfo[dc.Length];
						for(int i = 0; i < dc.Length ; ++i)
						{
							data.cameraPreset[i] = new CameraInfo();
							string[] l = dc[i].Split('|');
							float[] pos = Util.stringToFloatArray( l[0],',');
							data.cameraPreset[i].position.x = pos[0];
							data.cameraPreset[i].position.y = pos[1];
							data.cameraPreset[i].position.z = pos[2];
							float[] rot = Util.stringToFloatArray( l[1],',');
							data.cameraPreset[i].rotation.x = rot[0];
							data.cameraPreset[i].rotation.y = rot[1];
							data.cameraPreset[i].rotation.z = rot[2];
						
							float.TryParse(l[2],out data.cameraPreset[i].fov);
						}
						break;





				case "LEO_DEFAULT":
					data.defaultLeo = list[k["VALUE"]].ToString().Split(',');
					break;

				case "LEO_DEFAULT2":
					data.defaultLeo2 = list[k["VALUE"]].ToString().Split(',');
					break;

				case "KILEY_DEFAULT":
					data.defaultKiley = list[k["VALUE"]].ToString().Split(',');
					break;
				case "KILEY_DEFAULT2":
					data.defaultKiley2 = list[k["VALUE"]].ToString().Split(',');
					break;

				case "CHLOE_DEFAULT":
					data.defaultChloe = list[k["VALUE"]].ToString().Split(',');
					break;
				case "CHLOE_DEFAULT2":
					data.defaultChloe2 = list[k["VALUE"]].ToString().Split(',');
					break;

				case "DEFAULT_EFFECTS":
					data.defaultLoadEffects = list[k["VALUE"]].ToString().Split(',');
					break;


				case "TAG_KNOCK_BACK":
					data.tagKnuckBackValue = Util.stringToXIntArray( list[k["VALUE"]].ToString(), ',');
					break;


				case "TAG_STUN_TIME":
					Util.parseObject( list[k["VALUE"]], out data.tagStunTime, true ,2500);
					break;

				case "TAG_COOLTIME":
					Util.parseObject( list[k["VALUE"]], out data.tagCoolTime, true, 20.0f);

#if UNITY_EDITOR
					if(DebugManager.instance.useDebug)
					{
						data.tagCoolTime = 3f;
					}
#endif

					break;


				case "TAG_TEST_DECK1_HERO":
					data.tagTest1Hero = list[k["VALUE"]].ToString();
					break;
				case "TAG_TEST_DECK1_EQUIP":
					data.tagTest1Equips = list[k["VALUE"]].ToString().Split(',');
					break;
				case "TAG_TEST_DECK1_UNIT":
					data.tagTest1Unit = list[k["VALUE"]].ToString().Split(',');
					break;
				case "TAG_TEST_DECK1_SKILL":
					data.tagTest1Skill = list[k["VALUE"]].ToString().Split(',');
					break;
							
				case "TAG_TEST_DECK2_HERO":
					data.tagTest2Hero = list[k["VALUE"]].ToString();
					break;
				case "TAG_TEST_DECK2_EQUIP":
					data.tagTest2Equips = list[k["VALUE"]].ToString().Split(',');
					break;
				case "TAG_TEST_DECK2_UNIT":
					data.tagTest2Unit = list[k["VALUE"]].ToString().Split(',');
					break;
				case "TAG_TEST_DECK2_SKILL":
					data.tagTest2Skill = list[k["VALUE"]].ToString().Split(',');
					break;
				}
				
				list = null;
			}
		}
		
		GameManager.info.setupData = data;


		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	


	public void loadBaseHeroPartsData(string fileName)
	{
		Dictionary<string, BaseHeroPartsData> heroPartsData = new Dictionary<string, BaseHeroPartsData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);

		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				BaseHeroPartsData data = new BaseHeroPartsData();
				data.setData(list, k);
				heroPartsData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.heroBasePartsData = heroPartsData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}



	
	public void loadHeroPartsData(string fileName)
	{
		Dictionary<string, HeroPartsData> heroPartsDic = new Dictionary<string, HeroPartsData>(StringComparer.Ordinal);

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;

				bool skip = false;

				BaseHeroPartsDataInfo bpd = new BaseHeroPartsDataInfo();
				bpd.setData(list, k);

				HeroPartsData data = new HeroPartsData();
				data.setData(list, k, bpd);

				data.id = (string)list[k["ID"]];

				heroPartsDic[data.id] = data;

				list = null;
			}
		}
		
		GameManager.info.heroPartsDic = heroPartsDic;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		

	}
	
	
	public void loadCutSceneData(string fileName)
	{
#if UNITY_EDITOR
		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			CutSceneMaker.instance.parseSource();
			return;
		}
#endif
	}	






	public void loadAndAddCutSceneData(string cutSceneId)
	{
		#if UNITY_EDITOR
		if(CutSceneMaker.instance.useCutSceneMaker)
		{
			CutSceneMaker.instance.parseSource();
			return;
		}
		#endif

		if(GameManager.info.cutSceneData.ContainsKey(cutSceneId)) return;

//		Debug.Log(cutSceneId);

		// ---- 고정 
		string source = AssetBundleManager.getCutSceneTextAssetDataFromLocal(cutSceneId);

//		Debug.Log(source);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.

		CutSceneData cutSceneData = new CutSceneData();
		cutSceneData.id = cutSceneId.ToUpper();

		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				cutSceneData.setData(list, k);
				list = null;
			}
		}

		cutSceneData.setDataFinallize();

		GameManager.info.cutSceneData.Add(cutSceneData.id,cutSceneData);
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

	}	







	public void loadUnitSkillCamData(string fileName)
	{
		Dictionary<string, CutSceneData> cutSceneData = new Dictionary<string, CutSceneData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				string id = (string)list[k["ID"]];
				
				if(cutSceneData.ContainsKey(id) == false)
				{
					cutSceneData.Add(id, new CutSceneData());
//					cutSceneData[id].id = id;
				}
				cutSceneData[id].setData(list, k, CutSceneData.CutSceneType.UnitCam);
				list = null;
			}
		}
		
		foreach(KeyValuePair<string, CutSceneData> kv in cutSceneData)
		{
			kv.Value.setDataFinallize();
		}

		GameManager.info.unitSkillCamData = cutSceneData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	







	public void loadPlayerAiData(string fileName)
	{
		Dictionary<string, PlayerAiData> aiDatas = new Dictionary<string, PlayerAiData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				PlayerAiData data = new PlayerAiData();
				data.setData(list, k);
				aiDatas[data.id] = data;
				list = null;
			}
		}

		GameManager.info.playerAiData = aiDatas;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	




	public void loadAiGroupData(string fileName)
	{
		Dictionary<string, AiGroupData> aiGroupData = new Dictionary<string, AiGroupData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				AiGroupData data = new AiGroupData();
				data.setData(list, k);
				aiGroupData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.aiGroupData = aiGroupData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	




	public void loadFaceAniData(string fileName)
	{
		Dictionary<string, Dictionary<string, FaceAnimationInfo>> faceAniData = new Dictionary<string, Dictionary<string, FaceAnimationInfo>>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				FaceAnimationInfo data = new FaceAnimationInfo();
				data.setData(list, k);
				string cha = data.character.ToLower();
				if(faceAniData.ContainsKey(cha) == false) faceAniData.Add(cha, new Dictionary<string, FaceAnimationInfo>());
				faceAniData[cha][data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.faceAniData = faceAniData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	



	public void loadLobbyPosition(string fileName)
	{
		Dictionary<string, LobbyPositionData> lobbyPosition = new Dictionary<string, LobbyPositionData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				LobbyPositionData data = new LobbyPositionData();

				data.setData(list, k);
				lobbyPosition[(string)list[k["ID"]]] = data;

				list = null;
			}
		}

		GameManager.info.lobbyPosition = lobbyPosition;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	



	public void loadAdviceData(string fileName)
	{
		List<AdviceData> adviceData = new List<AdviceData>();

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				AdviceData data = new AdviceData();
				data.setData(list, k);
				adviceData.Add(data);
				list = null;
			}
		}
		
		GameManager.info.adviceData = adviceData;

		adviceData.Sort(AdviceData.sortByScoreFromLow);


		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	




	
	
	public void loadMapData(string fileName)
	{
		Dictionary<int, MapData> mapData = new Dictionary<int, MapData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				MapData data = new MapData();
				data.setData(list, k);
				mapData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.mapData = mapData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
	
	
	
	public void loadNPCData(string fileName)
	{
		Dictionary<string, NPCData> npcData = new Dictionary<string, NPCData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				NPCData data = new NPCData();
				data.setData(list, k);
				npcData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.npcData = npcData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
	
	

	//loadEffectData("");

	public void loadEffectData(string fileName)
	{
		Dictionary<string, EffectData> effectData = new Dictionary<string, EffectData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				EffectData data = new EffectData();
				data.setData(list, k);
				effectData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.effectData = effectData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}			




	public static Dictionary<string, EffectData> getEffectDataDic()
	{
		Dictionary<string, EffectData> effectData = new Dictionary<string, EffectData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal("data_effect_client");
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				EffectData data = new EffectData();
				data.setData(list, k);
				effectData[data.id] = data;
				list = null;
			}
		}
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

		return effectData;

	}	



	
	
	public void loadAniData(string fileName)
	{
		Dictionary<string, Dictionary<string, AniData>> aniData = new Dictionary<string, Dictionary<string, AniData>>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				AniData data = new AniData();
				data.setData(list, k);

				if(aniData.ContainsKey(data.id) == false) aniData.Add(data.id, new Dictionary<string, AniData>(StringComparer.Ordinal));
				
				aniData[data.id][data.ani] = data;
				
				list = null;
			}
		}
		
		GameManager.info.aniData = aniData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}		
		
		
	public void loadSoundData(string fileName)
	{
		Dictionary<string, SoundData> soundData = new Dictionary<string, SoundData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.

		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				SoundData data = new SoundData();
				data.setData(list, k);

//				Debug.LogError("sd: " + data.id);

				if(data.type == SoundData.Type.Chracter || data.type == SoundData.Type.Player)
				{
					string str = list[k["PARSING"]].ToString().Trim();

					if(string.IsNullOrEmpty(str) == false)
					{
						string[] ss = str.Split(',');
						int len = ss.Length;


						data.atkNum = 0;
						data.dieNum = 0;
						data.dmgNum = 0;
						data.grnNum = 0;


						for(int i = 0; i < len ; ++i)
						{
							SoundData d = new SoundData();
							d.id = data.id + "_" + ss[i];

							d.fileName = d.id;

							d.path = data.path + d.id;

							d.type = SoundData.Type.Effect;

							soundData[d.id] = d;

							if(data.type == SoundData.Type.Player)
							{

								if(ss[i].Contains("atk"))
								{
									++data.atkNum;
								}
								else if(ss[i].Contains("die"))
								{
									++data.dieNum;
								}
								else if(ss[i].Contains("dmg"))
								{
									++data.dmgNum;
								}
								else if(ss[i].Contains("grn"))
								{
									++data.grnNum;
								}
							}
						}
					}
				}

				soundData[data.id] = data;

				list = null;
			}
		}


//		foreach(KeyValuePair<string, SoundData> kv in soundData)
//		{
//			Debug.LogError(kv.Key + "   fileName: " + kv.Value.fileName + "   path: " + kv.Value.path);
//		}


		GameManager.info.soundData = soundData;

		jd.Clear();
		jd = null;

		k.Clear();
		k = null;
	}	




	
	public void loadModelData(string fileName)
	{
		Dictionary<string, ModelData> modelData = new Dictionary<string, ModelData>(StringComparer.Ordinal);

		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = ClientDataLoader.getKeyIndexDic((List<object>)jd[NAME]);
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				ModelData data = new ModelData();
				
				data.fileName = (string)list[k["FILENAME"]];
				data.shader = (string)list[k["SHADER"]];

				if(string.IsNullOrEmpty(data.shader) == false && data.shader == "Rim/Rimlight_rampL")
				{
					data.useRimShader = true;
				}
				else
				{
					data.useRimShader = false;
				}


				data.hasCollider = !((string)list[k["HAS_COLLIDER"]]).Equals("N");
				
				Util.parseObject(list[k["SCALE"]], out data.scale, 100);

				float scale = data.scale / 100.0f;//* 0.01f;

				Util.parseObject(list[k["DAMAGE_RANGE"]], out data.damageRange, 100);

				data.damageRange *= scale;

				switch(((string)list[k["MERGE_WITHOUT_ATLAS"]]))
				{
				case "Y":
					data.mergeWithoutAtlas = ModelData.MergeType.All;
					break;
				case "P":
					data.mergeWithoutAtlas = ModelData.MergeType.Parts;
					break;
				default:
					data.mergeWithoutAtlas = ModelData.MergeType.None;
					break;
					
				}

				Util.parseObject(list[k["PX"]],out data.px, true);
				Util.parseObject(list[k["PY"]],out data.py, true);
				Util.parseObject(list[k["PZ"]],out data.pz, true);
				Util.parseObject(list[k["SX"]],out data.sx, true);
				Util.parseObject(list[k["SY"]],out data.sy, true);
				Util.parseObject(list[k["SZ"]],out data.sz, true);

				Util.parseObject(list[k["SHADOW"]], out data.shadowSize, true, 0.0f);
				data.shadowSize *= scale;

				if(k.ContainsKey("SUMMONSIZE"))
				{
					Util.parseObject(list[k["SUMMONSIZE"]], out data.summonEffectSize, true, 0.0f);
					Util.parseObject(list[k["EFFECTSIZE"]], out data.effectSize, true, 0.0f);
				}
				else
				{
					Util.parseObject(list[k["SHADOW"]], out data.summonEffectSize, true, 0.0f);
				}

				data.summonEffectSize *= scale;

				data.effectSize *= scale;

				Util.parseObject(list[k["W"]], out data.width, true, 0.0f);
				Util.parseObject(list[k["H"]], out data.height, true, 0.0f);

				Util.parseObject(list[k["SHOT_SCALE"]], out data.shotScale, true, 1.0f);

				data.width *= scale;
				data.height *= scale;

				data.shotScale *= scale;

				Util.parseObject(list[k["SHOT_YPOS"]], out data.shotYPos , true, 0.0f);

				data.shotYPos *= scale;

				float frame = 0.0f;
				Util.parseObject(list[k["SHOT"]], out frame, true, 0.0f);
				data.poseTime = frame/30.0f;



				switch((list[k["LOBBYSIZE"]]).ToString())
				{
				case "L":
					data.lobbySize = ModelData.LobbySize.Big;
					break;
				case "M":
					data.lobbySize = ModelData.LobbySize.Medium;
					break;
				case "S":
					data.lobbySize = ModelData.LobbySize.Small;
					break;
				}

				string cr = (list[k["COLOR"]]).ToString();

				if(string.IsNullOrEmpty(cr))
				{
					data.particleColorLength = 1;
					data.particleColors = new Color[1]{new Color(1,1,1)};
				}
				else
				{
					string[] c = cr.Split(',');
					data.particleColors = new Color[c.Length];
					data.particleColorLength = c.Length;

					for(int i = 0; i < c.Length; ++i)
					{
						data.particleColors[i] = Util.HexToColor(c[i]);
					}
				}

				cr = (list[k["BCOLOR"]]).ToString();
				
				if(string.IsNullOrEmpty(cr))
				{
					data.useDefaultColor = false;
				}
				else
				{
					data.useDefaultColor = true;
					data.defaultColor = Util.HexToColor(cr);
				}

				data.hasDeleteTime = (((string)list[k["DELTIME"]]).Equals("N") == false);

				cr = (list[k["TEXTURE"]]).ToString();

				if(string.IsNullOrEmpty(cr))
				{
					data.textures = null;
				}
				else
				{
					data.textures = cr.Split(',');
				}


				Util.parseObject(list[k["LOBBY_EFFECT_SIZE"]], out data.lobbyEffectSize, true, 1.0f);


				modelData[data.fileName] = data;
				list = null;
			}
		}
		GameManager.info.modelData = modelData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		

	}
	
	
	public void loadSkillEffectSetupData(string fileName)
	{
		
		Dictionary<int, SkillEffectSetupData> skillEffectSetupData = new Dictionary<int, SkillEffectSetupData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				SkillEffectSetupData data = new SkillEffectSetupData();
				data.setData(list, k);
				skillEffectSetupData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.skillEffectSetupData = skillEffectSetupData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
		
		
	public void loadLoadingTipData(string fileName)
	{
		List<LoadingTipData> tipData = new List<LoadingTipData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				LoadingTipData data = new LoadingTipData();
				data.setData(list, k);
				tipData.Add(data);
				list = null;
			}
		}
		
		GameManager.info.tipData = tipData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}




	public void loadLoadingScreenData(string fileName)
	{
		Dictionary<string, LoadingScreenData> loadingScreenData = new Dictionary<string, LoadingScreenData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				LoadingScreenData data = new LoadingScreenData();
				data.setData(list, k);
				loadingScreenData[data.unitId] = data;
				list = null;
			}
		}
		
		GameManager.info.loadingScreenData = loadingScreenData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

		
	}




	public void loadUnitIconIndexData(string fileName)
	{
		Dictionary<string, IconIndexData> unitIconIndexData = new Dictionary<string, IconIndexData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				IconIndexData data = new IconIndexData();
				data.setData(list, k);
				unitIconIndexData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.unitIconIndexData = unitIconIndexData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}






	public void loadSkillIconIndexData(string fileName)
	{
		Dictionary<string, IconIndexData> skillIconIndexData = new Dictionary<string, IconIndexData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				IconIndexData data = new IconIndexData();
				data.setData(list, k);
				skillIconIndexData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.skillIconIndexData = skillIconIndexData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}


	public void loadEquipIconIndexData(string fileName)
	{
		Dictionary<string, IconIndexData> equipIconIndexData = new Dictionary<string, IconIndexData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				IconIndexData data = new IconIndexData();
				data.setData(list, k);
				equipIconIndexData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.equipIconIndexData = equipIconIndexData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}









	public void loadBaseUnitData(string fileName)
	{
		Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				UnitData data = new UnitData();
				data.setData(list, k);

				data.baseIdWithoutRare = data.baseUnitId.Substring(3);

				unitData[data.id] = data;

				GameManager.info.unitData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.baseUnitData = unitData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	

	
	public void loadRareUnitData(string fileName)
	{
		Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		

		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				UnitData data = new UnitData();
				data.setData(list, k, true);
				GameManager.info.unitData[data.id] = data.getRareUnitData(GameManager.info.baseUnitData[data.baseUnitId], data);
				if(GameManager.info.rareUnitDataByBaseId.ContainsKey(data.baseUnitId) == false) GameManager.info.rareUnitDataByBaseId.Add(data.baseUnitId , new List<UnitData>());
				GameManager.info.rareUnitDataByBaseId[data.baseUnitId].Add(GameManager.info.unitData[data.id]);
				list = null;
			}
		}
		
//		GameManager.info.rareUnitData = unitData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	
	
	
	public void loadBaseMonsterUnitData(string fileName)
	{
		Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				UnitData data = new UnitData();
				data.setData(list, k, false, false, false);
				unitData[data.id] = data;
				
				// 유닛 데이터에도 넣어줌...
				GameManager.info.unitData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.baseMonsterUnitData = unitData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	
	
	
	public void loadRareMonsterUnitData(string fileName)
	{
		Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				UnitData data = new UnitData();
				data.setData(list, k, true);
//				unitData[data.id] = data;
				
				// 레어와 베이스를 합친것을 베이스 유닛 데이터에 넣어준다.
				GameManager.info.unitData[data.id] = data.getRareUnitData(GameManager.info.baseMonsterUnitData[data.baseUnitId], data);

				list = null;
			}
		}
		
//		GameManager.info.rareMonsterUnitData = unitData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	
	

	public void loadSkillIconData(string fileName)
	{
		Dictionary<string, SkillIconData> iconData = new Dictionary<string, SkillIconData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				SkillIconData data = new SkillIconData();
				data.setData(list, k);
				iconData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.skillIconData = iconData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}



	public void loadUnitSkillData(string fileName)
	{
		Dictionary<string, UnitSkillData> unitSkillData = new Dictionary<string, UnitSkillData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				UnitSkillData data = new UnitSkillData();
				data.setData(list, k);
				unitSkillData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.unitSkillData = unitSkillData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
	
	
	public void loadHeroSkillData(string fileName)
	{
		Dictionary<string, HeroSkillData> heroSkillData = new Dictionary<string, HeroSkillData>(StringComparer.Ordinal);
		Dictionary<string, HeroSkillData> heroBaseSkillData = new Dictionary<string, HeroSkillData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				HeroSkillData data = new HeroSkillData();
				data.isBase = true;
				data.setData(list, k);	
				heroBaseSkillData[data.id] = data;


				HeroSkillData d = new HeroSkillData();
				d.isBase = false;
				d.setData(list, k);		
				d.baseId = d.id;

				heroSkillData[d.id] = d;

				list = null;
			}
		}
		GameManager.info.heroBaseSkillData = heroBaseSkillData;
		GameManager.info.heroSkillData = heroSkillData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	
	


	public void loadTutorialInfoData(string fileName)
	{
		Dictionary<string, TutorialInfoData> tutorialInfoData = new Dictionary<string, TutorialInfoData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				TutorialInfoData data = new TutorialInfoData();				
				data.setData(list,k);
				tutorialInfoData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.tutorialInfoData = tutorialInfoData;
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		

	}



	
	public void loadTutorialData(string fileName)
	{
		Dictionary<string, Dictionary<int, TutorialData>> tutorialData = new Dictionary<string, Dictionary<int, TutorialData>>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				TutorialData data = new TutorialData();				
				data.setData(list,k);

				if(tutorialData.ContainsKey(data.id) == false) tutorialData.Add(data.id, new Dictionary<int, TutorialData>());
				tutorialData[data.id][data.step] = data;
				list = null;
			}
		}
		
		GameManager.info.tutorialData = tutorialData;
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}


	
	
	public void loadTextData(string fileName)
	{
		Dictionary<string, TextData> textData = new Dictionary<string, TextData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;

				TextData td = new TextData();
				td.setData(list, k);
				textData[(string)list[k["ID"]]] = td;
				list = null;
			}
		}
		
		GameManager.info.textData = textData;
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	

	public void loadUITextData(string fileName)
	{
		Dictionary<string, TextData> uiTextData = new Dictionary<string, TextData>(StringComparer.Ordinal);

		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);

		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				TextData td = new TextData();
				td.setData(list, k);
				uiTextData[(string)list[k["ID"]]] = td;
				list = null;
			}
		}

		GameManager.info.uiTextData = uiTextData;
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}





	
	
	public void loadActData(string fileName)
	{
		Dictionary<int, ActData> actData = new Dictionary<int, ActData>();

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				ActData data = new ActData();				
				data.setData(list,k);
				actData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.actData = actData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	
	
	

	public void loadStageData(string fileName)
	{
		Dictionary<string, StageData> stageData = new Dictionary<string, StageData>(StringComparer.Ordinal);

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				StageData data = new StageData();				
				data.setData(list,k);
				stageData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.stageData = stageData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	
	
	
	public void loadRoundData(string fileName)
	{
		Dictionary<string, RoundData> roundData = new Dictionary<string, RoundData>(StringComparer.Ordinal);

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				RoundData data = new RoundData();				
				data.setData(list,k);
				roundData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.roundData = roundData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
	

	
	
	
	
	public void loadHeroMonsterAI(string fileName)
	{
		Dictionary<string, MonsterHeroAI> heroMonsterAI = new Dictionary<string, MonsterHeroAI>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				MonsterHeroAI data = new MonsterHeroAI();
				data.setData(list, k);
				heroMonsterAI[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.heroMonsterAI = heroMonsterAI;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
		
	
	public void loadHeroMonsterData(string fileName)
	{
		Dictionary<string, HeroMonsterData> heroMonsterData = new Dictionary<string, HeroMonsterData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				HeroMonsterData data = new HeroMonsterData();
				data.setData(list, k);
				heroMonsterData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.heroMonsterData = heroMonsterData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		

	}
	
	
	
	
	
	
	
	public void loadMonsterData(string fileName)
	{
		Dictionary<string, MonsterData> monsterData = new Dictionary<string, MonsterData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				MonsterData data = new MonsterData();
				
				data.id = (string)list[k["ID"]];
				//data.type = (string)list[k["TYPE"]];
				data.name = (string)list[k["NAME"]];
#if UNITY_EDITOR
//				Debug.Log(" loadMonsterData,name:"+data.name);
#endif
				
				data.damageSound = (string)list[k["DAMAGESOUND"]];
				if(data.damageSound.Trim().Length == 0) data.damageSound = null;
				
				data.category = MonsterCategory.getCategory( ((string)list[k["CATEGORY"]]).Trim() );

				data.hasFaceAni = (((string)list[k["FACEANI"]]) == "Y");

				data.hasParticle = (((string)list[k["PARTICLE"]]) == "Y");

				//data.invincible = (data.hp == -1);
				//data.setBulletPattern((string)list[k["BULLETPATTERN"]]);
				//data.hitReward = new Reward((string)list[k["HITREWARD"]]);
				//data.reward = new Reward((string)list[k["REWARDITEM"]]);
				
#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["REWARDITEM"]]);
#endif						
				data.resource = (string)list[k["RESOURCE"]];

				data.icon = (string)list[k["ICON"]];

				data.defaultTexture = (string)list[k["TEXTURE"]];

				if(string.IsNullOrEmpty(data.defaultTexture))
				{
					data.defaultTexture = null;
				}

				Util.parseObject(list[k["SCALE"]], out data.scale, true, 1.0f);

#if UNITY_EDITOR
//				Debug.Log(data.resource);
#endif
				
				data.damageRange = GameManager.info.modelData[data.resource].damageRange;
				data.damageRange *= (data.scale * (((float)GameManager.info.modelData[data.resource].scale)/100.0f));	//data.damageRange *= (data.scale * (((float)GameManager.info.modelData[data.resource].scale)*0.01f));	

				
				data.deleteBulletAfterDead = (((string)list[k["DELETE_BULLET"]]).Equals("Y"));


				string dparts = ((string)list[k["DELETE_PARTS"]]).Trim();
				if(string.IsNullOrEmpty(dparts)) data.deleteParts = null;
				else data.deleteParts = dparts.Split(',');

				//Util.parseObject(list[k["SHADOWSIZE"]],  out data.shadowSize, 1.0f);



				data.isBlockMonster = ((string)list[k["BLOCKMONSTER"]]).Equals("Y");
				
				data.setCreateMotionType((string)list[k["CREATEMOTION"]]);
				
				data.setDeleteMotionType((string)list[k["DELETEMOTION"]]);
				
				
				if(list[k["EXPLOSION_EFFECT"]] != null && list[k["EXPLOSION_EFFECT"]] is string)
				{
					data.explosionEffect = (string)list[k["EXPLOSION_EFFECT"]];
				}
				
				data.setEffect((string)list[k["EFFECT"]]);
				
				data.visible = ((string)list[k["VISIBLE"]]).Equals("N")?false:true;
				
				data.summonSound = (string)list[k["SUMMON_SOUND"]];
				
				if(data.summonSound != null) data.summonSound = data.summonSound.Trim();
				if(data.summonSound.Length < 1) data.summonSound = null;

				data.canChangeShader = ((list[k["CHANGE_SHADER"]]).ToString() != "N");

//				if(k.ContainsKey("UV_ANI"))
				{
					string uv = (string)list[k["UV_ANI"]];
					
					if(string.IsNullOrEmpty(uv) == false)
					{
						string[] uvAnis = uv.Split('/');
						
						data.partsUV = new PartsUVAnimation[uvAnis.Length];
						
						for(int i = 0; i < uvAnis.Length; ++i)
						{
							data.partsUV[i] = new PartsUVAnimation();
							data.partsUV[i].setData(uvAnis[i]);
						}
						
					}
				}



				monsterData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.monsterData = monsterData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	
		
	
	
	public void loadPlayerData(string fileName)
	{
		Dictionary<string, PlayerHeroData[]> playerHeroData = new Dictionary<string, PlayerHeroData[]>(StringComparer.Ordinal);

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				PlayerHeroData data = new PlayerHeroData();
				data.setData(list, k);
				
				if(playerHeroData.ContainsKey(data.id) == false || playerHeroData[data.id] == null)
				{
					playerHeroData[data.id] = new PlayerHeroData[ PlayerHeroData.LEVEL_MAX ];
				}

				playerHeroData[data.id][data.level - 1] = data;

				list = null;
			}
		}
		
		GameManager.info.playerHeroData = playerHeroData;

		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}
	
	
	
	
	
	
	public void loadRewardLogic(string fileName)
	{
		Dictionary<string, RewardLogicData> rewardLogicData = new Dictionary<string, RewardLogicData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				RewardLogicData data = new RewardLogicData();
				
				data.id = (string)list[k["ID"]];
				data.reward = new Reward((string)list[k["LOGIC"]]);
				
#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["LOGIC"]]);
#endif						
				
				rewardLogicData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.rewardLogicData = rewardLogicData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}		
	
	
	
	
	

	public void loadMapObjectData(string fileName)
	{
		Dictionary<string, MapObjectData> mapObjectData = new Dictionary<string, MapObjectData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				MapObjectData mod = new MapObjectData();
				
				
				mod.id = (string)list[k["ID"]];
				mod.type = (string)list[k["TYPE"]];
				mod.name = (string)list[k["NAME"]];

				string[] tempAttr = ((string)list[k["ATTR"]]).Split(':');



				switch(tempAttr.Length)
				{
				case 1:
					mod.attr = tempAttr[0];					
					break;
				case 2:
					mod.attr = tempAttr[0];
					mod.attrValue = Convert.ToInt32(tempAttr[1]);
					break;
				}


				
				mod.reward = new Reward((string)list[k["REWARDITEM"]]);



#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["REWARDITEM"]]);
#endif				
				
				mod.afterLimitAction = new Reward((string)list[k["AFTER_LIMIT_ACTION"]]);
				
#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["AFTER_LIMIT_ACTION"]]);
#endif						
				if(list[k["TIMELIMIT"]] is long) mod.timeLimit = (float)(long)list[k["TIMELIMIT"]];
				else if(list[k["TIMELIMIT"]] is double) mod.timeLimit = (float)(double)list[k["TIMELIMIT"]];
				else if(list[k["TIMELIMIT"]] is float) mod.timeLimit = (float)list[k["TIMELIMIT"]];
				else if(list[k["TIMELIMIT"]] is int) mod.timeLimit = (float)(int)list[k["TIMELIMIT"]];
				
				if(list[k["SCALE"]] is long) mod.scale = (float)(long)list[k["SCALE"]];
				else if(list[k["SCALE"]] is double) mod.scale = (float)(double)list[k["SCALE"]];
				else if(list[k["SCALE"]] is float) mod.scale = (float)list[k["SCALE"]];
				else if(list[k["SCALE"]] is int) mod.scale = (float)(int)list[k["SCALE"]];				
				
				
				mod.setEffect((string)list[k["EFFECT"]]);
				
				mapObjectData[mod.id] = mod;
				
				list = null;
			}
		}
		
		GameManager.info.mapObjectData = mapObjectData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}	
	
	
	
	
	

	
	
	
	
	
	
	
	
	
	public void loadBulletData(string fileName)
	{
		Dictionary<string, BulletData> bulletData = new Dictionary<string, BulletData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				BulletData data = new BulletData();
				
				data.id = (string)list[k["ID"]];
				
				//data.name = (string)list[k[NAME]];
				
//				data.isAnimationBullet = (list[k["ANI"]] is string && ((string)list[k["ANI"]]).Equals("Y"));
				
				//data.resourceId = (string)list[k["RESOURCE"]];
				
//				if(list[k["R_SPEED"]] is long) data.rotateSpeed = (float)(long)list[k["R_SPEED"]];
//				else if(list[k["R_SPEED"]] is double) data.rotateSpeed = (float)(double)list[k["R_SPEED"]];
//				else if(list[k["R_SPEED"]] is int) data.rotateSpeed = (float)(int)list[k["R_SPEED"]];
//				else if(list[k["R_SPEED"]] is float) data.rotateSpeed = (float)list[k["R_SPEED"]];
				
				
				data.hitEffect = (string)list[k["HITEFFECT"]];

				if(data.hitEffect.Contains(","))
				{
					string[] heOption = data.hitEffect.Split(',');
					data.hitEffect = heOption[0];

					switch(heOption[1])
					{
					case "IS":
						data.hitEffectOption = BulletData.HitEffectOptionType.IgnoreSize;
						break;
					case "IP":
						data.hitEffectOption = BulletData.HitEffectOptionType.IgnoreParent;
						break;
					case "IA":
						data.hitEffectOption = BulletData.HitEffectOptionType.IgnoreAll;
						break;
					}
				}


				data.hitActionReward = new Reward((string)list[k["HITACTION"]]);
				data.deadActionReward = new Reward((string)list[k["HITDEADACTION"]]);
				

#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["HITACTION"]]);
				checkLogics.Add((string)list[k["HITDEADACTION"]]);
#endif						
				
				/*
				if(list[k["SPEED"]] is long) data.speed = (float)(long)list[k["SPEED"]];	
				else if(list[k["SPEED"]] is double) data.speed = (float)(double)list[k["SPEED"]];	
				else if(list[k["SPEED"]] is int) data.speed = (float)(int)list[k["SPEED"]];	
				else if(list[k["SPEED"]] is float) data.speed = (float)list[k["SPEED"]];	
				*/
				
				/*
				if(list[k["POWER"]] is long) data.power = (int)(long)list[k["POWER"]];	
				else if(list[k["POWER"]] is int) data.power = (int)list[k["POWER"]];	
				else data.power = 0;
				*/
				
//				if(list[k["HP"]] is long) data.hp = (int)(long)list[k["HP"]];	
//				else if(list[k["HP"]] is int) data.hp = (int)list[k["HP"]];	
//				else data.hp = 0;
				
				data.destroy = (list[k["DESTROY"]] is string && ((string)list[k["DESTROY"]]).Equals("Y"));

				data.attachEffectToShotPoint = (list[k["EFF2POINT"]] is string && ((string)list[k["EFF2POINT"]]).Equals("Y"));

				data.destroyEffectId = (string)list[k["USE_DESTROY_EFFECT"]];
				data.destroyEffectId = data.destroyEffectId.Trim();
				data.useDestroyEffect = (string.IsNullOrEmpty(data.destroyEffectId))?false:true;
					
				if(k.ContainsKey("DESTROY_EFFECT_OPTION"))
				{

					// 폭파 이펙트가 총알의 회전값을 사용함.
					switch(list[k["DESTROY_EFFECT_OPTION"]].ToString())
					{
					case "BR":
						data.destroyEffectOption = BulletData.DestroyEffectOptionType.UseBulletRotation;
						break;
					}
				}

//				if(list[k["DISTANCELIMIT"]] is long) data.limitDistance = (float)(long)list[k["DISTANCELIMIT"]];	
//				else if(list[k["DISTANCELIMIT"]] is int) data.limitDistance = (float)(int)list[k["DISTANCELIMIT"]];	
//				else if(list[k["DISTANCELIMIT"]] is float) data.limitDistance = (float)list[k["DISTANCELIMIT"]];	
//				else data.limitDistance = -1;				
//				
//				
//				if(list[k["TIMELIMIT"]] is long) data.limitTime = (float)(long)list[k["TIMELIMIT"]];	
//				else if(list[k["TIMELIMIT"]] is double) data.limitTime = (float)(double)list[k["TIMELIMIT"]];	
//				else if(list[k["TIMELIMIT"]] is int) data.limitTime = (float)(int)list[k["TIMELIMIT"]];	
//				else if(list[k["TIMELIMIT"]] is float) data.limitTime = (float)list[k["TIMELIMIT"]];	
//				else data.limitTime = -1;
//				
//			
				if(list[k["INVINCIBLE"]] is long) data.invincibleValue = (int)(long)list[k["INVINCIBLE"]];	
				else if(list[k["INVINCIBLE"]] is int) data.invincibleValue = (int)list[k["INVINCIBLE"]];	
				
				
				data.destoryAction = new Reward((string)list[k["DESTROY_ACTION"]]); //data.setSecondBulletAction((string)jd[keyName][15]);
				
#if UNITY_EDITOR				
				checkLogics.Add((string)list[k["DESTROY_ACTION"]]);
#endif						

				data.ignoreHitObjectRotate = (list[k["ROT"]].ToString()).Equals("Y")?true:false;

				data.setEffect((string)list[k["EFFECT"]]);
				
				data.visible = ((string)list[k["VISIBLE"]]).Equals("N")?false:true;


				data.startSound = list[k["START_SOUND"]].ToString();
				data.destroySound = list[k["DESTROY_SOUND"]].ToString();

				if(string.IsNullOrEmpty(data.startSound)) data.startSound = null;
				if(string.IsNullOrEmpty(data.destroySound)) data.destroySound = null;

				if(k.ContainsKey("PRELOAD_EFFECT"))
				{
					data.preloadEffect = list[k["PRELOAD_EFFECT"]].ToString();
					if(string.IsNullOrEmpty(data.preloadEffect)) data.preloadEffect = null;
				}
				else
				{
					data.preloadEffect = null;
				}


				if(list[k["SCALE"]] is long) data.scale = (float)(long)list[k["SCALE"]];
				else if(list[k["SCALE"]] is double) data.scale = (float)(double)list[k["SCALE"]];
				else if(list[k["SCALE"]] is float) data.scale = (float)list[k["SCALE"]];
				else if(list[k["SCALE"]] is int) data.scale = (float)(int)list[k["SCALE"]];					

				Util.parseObject(list[k["PIVOT"]], out data.pivot, true, -1);

				

				
				bulletData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.bulletData = bulletData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}	
	

	
	public void loadBulletPattern(string fileName)
	{
		Dictionary<string, BulletPatternData> bulletPatternData = new Dictionary<string, BulletPatternData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				BulletPatternData data = new BulletPatternData();
				
				data.id = (string)list[k["ID"]];
				
				data.ids = ((string)list[k["IDS"]]).Split(',');
				data.bulletIdLength = data.ids.Length;
				
				if(  ((string)list[k["IDS"]]).Length == 0 || ((string)list[k["IDS"]]).Trim() == "")
				{
					data.bulletIdLength = 0;
				}
				
				bulletPatternData[data.id] = data;
				
				list = null;
			}
		}
		
		GameManager.info.bulletPatternData = bulletPatternData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}	
	
	

	public void loadTestModeData(string fileName)
	{
		Dictionary<string, TestModeData> testModeData = new Dictionary<string, TestModeData>(StringComparer.Ordinal);

		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				TestModeData data = new TestModeData();
				data.setData(list, k);

				testModeData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.testModeData = testModeData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}	




	public void loadTranscendData(string fileName)
	{
		Dictionary<string, TranscendData> transcendData = new Dictionary<string, TranscendData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				TranscendData data = new TranscendData();
				data.setData(list, k);
				
				transcendData[data.id] = data;
				list = null;
			}
		}
		
		GameManager.info.transcendData = transcendData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
		
	}




	public void loadHellSetupData(string fileName)
	{
		Dictionary<int, HellSetupData> hellSetupData = new Dictionary<int, HellSetupData>();
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				HellSetupData data = new HellSetupData();
				data.setData(list, k);
				
				hellSetupData[data.roundIndex] = data;
				list = null;
			}
		}
		
		GameManager.info.hellSetupData = hellSetupData;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;

	}

	public void loadTestSigongData(string fileName)
	{
		Dictionary<string, P_Sigong> testSigong = new Dictionary<string, P_Sigong>(StringComparer.Ordinal);
		
		// ---- 고정 

		string source = (Resources.Load(ClientDataLoader.CLIENT_DATA_PATH + "data_sigong_test_client") as TextAsset).ToString();

		if( string.IsNullOrEmpty( source )) return;

		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;

				P_Sigong  p = new P_Sigong();

				p.id = (string)list[k["DUNGEON_ID"]];

				p.roundId = (string)list[k["ROUND_ID"]];

				p.handicap = list[k["HANDICAP_TYPE"]].ToString();

				p.forcedDeck = (string)list[k["FORCED_DECK"]];

				testSigong[p.id] = p;
				list = null;
			}
		}
		
		GameManager.info.testSigong = testSigong;
		
		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}	



	public void loadVersionData(string fileName)
	{
		Dictionary<string, VersionData> versionData = new Dictionary<string, VersionData>(StringComparer.Ordinal);
		
		// ---- 고정 
		string source = AssetBundleManager.getTextAssetDataFromLocal(fileName);
		
		Dictionary<string, object> jd = MiniJSON.Json.Deserialize(source) as Dictionary<string, object>;
		Dictionary<string, int> k = getKeyIndexDic((List<object>)jd[NAME]);
		// -- 여기까지.
		
		foreach(KeyValuePair<string, object> kv in jd)
		{
			if(kv.Key != NAME)
			{
				List<object> list = kv.Value as List<object>;
				
				VersionData data = new VersionData();
				data.setData(list, k);

				if(data.versions != null && data.versions.Length > 0)
				{
					versionData[data.id] = data;
				}

				list = null;
			}
		}
		
		GameManager.info.versionData = versionData;

		if(versionData.ContainsKey(GameManager.info.clientFullVersion))
		{
			if(versionData[GameManager.info.clientFullVersion].codePatch == null)
			{
				VersionData.codePatchVer = null;
				VersionData.codePathVerNum = 0;
			}
			else
			{
				VersionData.codePathVerNum = versionData[GameManager.info.clientFullVersion].codePatch.Length;
				VersionData.codePatchVer = new int[versionData[GameManager.info.clientFullVersion].codePatch.Length];

				for(int i = 0; i < versionData[GameManager.info.clientFullVersion].codePatch.Length; ++i)
				{
					VersionData.codePatchVer[i] = versionData[GameManager.info.clientFullVersion].codePatch[i];
				}
			}



		}

		jd.Clear();
		jd = null;
		
		k.Clear();
		k = null;
	}	


	
	
	
	
	//--------------- UTIL ---------------- //
	
	public static Dictionary<string, int> getKeyIndexDic(List<object> list)
	{
		Dictionary<string, int> keyIndexer = new Dictionary<string, int>(StringComparer.Ordinal);
		
		int len = list.Count;
		
		for(int i = 0; i < len ; ++i)
		{
			keyIndexer[(string)list[i]] = i;
		}
		
		return keyIndexer;
	}	

}



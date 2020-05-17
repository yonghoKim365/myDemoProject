using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public partial class UnitSkillCamMaker : MonoBehaviour {

	public static UnitSkillCamMaker instance;

	public bool usePrefabEffect = false;

	public bool disablePlayerAttack = false;

	public bool gameResourceErrorCheck = false;

	[HideInInspector]public bool useUnitSkillCamMaker = false;

	[HideInInspector]public bool useEffectSkillCamEditor = false;
	[HideInInspector]public bool useUnitSkillCamEditor = false;


	public bool usePause = false;

	public string nowId = "";

	public string source;

	public GameObject[] gameObjectForDebugging;

	void Awake () {
		instance = this;
#if UNITY_EDITOR

#else
		useUnitSkillCamMaker = false;
#endif
	}

	public void play()
	{
		GameManager.me.stageManager.setNowRound(GameManager.info.roundData[DebugManager.instance.debugRoundId], GameType.Mode.Epic);
		GameManager.me.startGame(0);
	}


	public float playerPosX = -50.0f;

	public float unitPosX = 150.0f;

	public float monsterPosX = 300.0f;


	public void resetCharacterPosition()
	{
		foreach(Monster mon in GameManager.me.characterManager.playerMonster)
		{
			mon.setPositionCtransform(new Vector3(unitPosX, 0, 0));
			if(mon.isPlayer == false) mon.stat.speed = 0.0f;
		}

		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			mon.setPositionCtransform(new Vector3(monsterPosX, 0, 0));
			mon.stat.speed = 0.0f;
		}

		GameManager.me.player.setPositionCtransform(new Vector3(playerPosX, 0, 0));
	}

	public void deleteUnit()
	{
		foreach(Monster mon in GameManager.me.characterManager.playerMonster)
		{
			if(mon.isPlayer == false)
			{
				mon.dead();
			}
		}
	}


	public void playSkill()
	{
		for(int i = 0; i < 5; ++i)
		{
			if(GameDataManager.instance.unitSlots[i].isOpen)
			{
				if(GameManager.me.uiManager.uiPlay.UIUnitSlot[i].isLocked == false)
				{
					if(GameManager.me.uiManager.uiPlay.UIUnitSlot[i].unitSlot != null && GameManager.me.uiManager.uiPlay.UIUnitSlot[i].unitSlot.activeSkillData != null && GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon != null
					   && GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon.isEnabled)
					{
						GameManager.me.uiManager.uiPlay.UIUnitSlot[i].useDebugActiveSkill();

#if UNITY_EDITOR
						if(usePause)
						{
							UnityEditor.EditorApplication.isPaused = true;
						}
#endif
					}
				}
			}
		}
	}



	public void playAttack()
	{
		for(int i = 0; i < 5; ++i)
		{
			if(GameDataManager.instance.unitSlots[i].isOpen)
			{
				if(GameManager.me.uiManager.uiPlay.UIUnitSlot[i].isLocked == false)
				{
					if(GameManager.me.uiManager.uiPlay.UIUnitSlot[i].unitSlot != null && GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon != null
					   && GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon.isEnabled)
					{

						if(GameManager.me.characterManager.monsters.Count > 0)
						{
							GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon.setTarget(GameManager.me.characterManager.monsters[0]);

							Monster mon = GameManager.me.uiManager.uiPlay.UIUnitSlot[i].mon;

							mon.unitData.attackType.lookTargetAndAttack(mon, GameManager.me.characterManager.monsters[0], mon.unitData.attackType.isShortType);

//							mon.action.setAttackDelay();
//							mon.action.nowUnitSkillData = null;
//							mon.state = Monster.ATK_IDS[ mon.unitData.attackType.type]; //Monster.SHOOT;
//							mon.action.state = CharacterAction.STATE_WAIT;

						}

						#if UNITY_EDITOR
						if(usePause)
						{
							UnityEditor.EditorApplication.isPaused = true;
						}
						#endif
					}
				}
			}
		}
	}


	public void resetAniData()
	{
		GameManager.me.clientDataLoader.loadBulletPattern("data_bulletpattern_client");
		GameManager.me.clientDataLoader.loadBulletData("data_bullet_client");
		GameManager.me.clientDataLoader.loadAniData("data_anidata_client");

		for(int i = 0; i < GameManager.me.characterManager.playerMonster.Count; ++i)
		{
			GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(GameManager.me.characterManager.playerMonster[i]);				
		}

		for(int i = 0; i < GameManager.me.characterManager.monsters.Count; ++i)
		{
			GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(GameManager.me.characterManager.monsters[i]);				
		}

	}



	public void resetCamera()
	{
		GameManager.me.uiManager.uiPlay.resetCamera();
	}
	
	
	public bool parseSource()
	{
		Dictionary<string, CutSceneData> cutSceneData = new Dictionary<string, CutSceneData>(StringComparer.Ordinal);
		Dictionary<string, int> k = new Dictionary<string, int>();
		k["ID"] = 0;
		k["HEADER"] = 1;
		k["COMMAND"] = 2;
		k["ATTR1"] = 3;
		k["ATTR2"] = 4;
		k["ATTR3"] = 5;
		k["ATTR4"] = 6;
		k["ATTR5"] = 7;
		k["ATTR6"] = 8;
		k["ATTR7"] = 9;
		k["ATTR8"] = 10;

		if(string.IsNullOrEmpty(source)) return false;

		string[] lines = source.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);

		int len = lines.Length;

		List<object> list = new List<object>();

		bool isFirst = true;

		for(int i = 0; i < len; ++i)
		{
			if(string.IsNullOrEmpty(lines[i])) continue;

			string[] singleLines = Util.CsvParser(lines[i]);

			if(singleLines[0] != "US") continue;

			list.Clear();

			int len2 = singleLines.Length;

			for(int j = 1; j < 12; ++j)
			{
				if(j < len2)
				{
					list.Add( singleLines[j] );
				}
				else
				{
					list.Add("");
				}
			}


			string id = (string)list[k["ID"]];
			
			if(cutSceneData.ContainsKey(id) == false)
			{
				cutSceneData.Add(id, new CutSceneData());

				if(isFirst)
				{
					nowId = id;
					isFirst = false;
				}
			}

//			cutSceneData[id].id = id;
			cutSceneData[id].setData(list, k);
		}

		foreach(KeyValuePair<string, CutSceneData> kv in cutSceneData)
		{
			kv.Value.setDataFinallize();
		}

		if(cutSceneData == null || cutSceneData.Count < 1) return false;

		if(Application.isEditor && Application.isPlaying)
		{
			GameManager.info.unitSkillCamData = cutSceneData;
		}
		else return false;

		return true;
	}



	[HideInInspector] public DebugManager debugManager;

	[HideInInspector] public ResourceManager resourceManager;


	public void loadDebugUnit(string units)
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

		debugManager.debugUnitId =lines;

		Debug.LogError(lines.Length + " 입력완료");



		if(Application.isPlaying)
		{
			debugManager.setDebugInitData();
			GameManager.me.player.init(GameDataManager.instance.heroes[DebugManager.instance.defaultHero],true,true,0);

			if(GameManager.me.player != null)
			{
				if(GameManager.me.player.playerData != null && GameManager.me.player.playerData.units != null)
				{
					foreach(string unitId in GameManager.me.player.playerData.unitResourceId)
					{
						if(string.IsNullOrEmpty( unitId ) == false)
						{
							MonsterData md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];

							GameManager.me.effectManager.loadEffectFromUnitData(unitId);
							GameDataManager.instance.addLoadModelData(md);
							
						}
					}
				}
			}

			GameManager.me.effectManager.startLoadEffects(true);
			GameDataManager.instance.startModelLoad(true);
		}
	}




	public void loadDebugSkill(string units)
	{
		units = units.Trim();
		string[] lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);

		for(int i = 0; i < lines.Length; ++i)
		{
			lines[i] += "_20";
			if(lines[i].StartsWith("UN") || lines[i].StartsWith("EU")) return;
		}

		debugManager.debugSkillId =lines;
		
		Debug.LogError(lines.Length + " 입력완료");
		
		for(int i = 0; i < lines.Length ; ++i)
		{
			Debug.Log(lines[i]);
		}

		if(Application.isPlaying)
		{
			try
			{
				debugManager.setDebugInitData();
				GameManager.me.player.init(GameDataManager.instance.heroes[DebugManager.instance.defaultHero],true,true,0);
				GameManager.me.effectManager.loadEffectFromPlayerData(GameDataManager.instance.heroes[DebugManager.instance.defaultHero]);
				GameManager.me.effectManager.startLoadEffects(true);

			}
			catch
			{
				Debug.LogError("입력 오류!");
			}

		}
	}



	public void createHeroMonster(string heroMonsterID)
	{
		if(Application.isPlaying)
		{
			heroMonsterID = heroMonsterID.Trim();
			string[] lines = heroMonsterID.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);

			foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
			{
				RoundData rd = kv.Value;

				if(rd.heroMonsters != null)
				{
					for(int i = 0; i < lines.Length; ++i)
					{
						for(int j = 0; j < rd.heroMonsters.Length; ++j)
						{
							if(rd.heroMonsters[j].id == lines[i] && rd.heroMonsters[j].attr == null)
							{
								StartCoroutine(createHeroMonster(rd.heroMonsters[j]));
								return;
							}
						}
					}
				}
			}



			foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
			{
				RoundData rd = kv.Value;
				
				if(rd.heroMonsters != null)
				{
					for(int i = 0; i < lines.Length; ++i)
					{
						for(int j = 0; j < rd.heroMonsters.Length; ++j)
						{
							if(rd.heroMonsters[j].attr == null)
							{
								if(  GameManager.info.monsterData[ GameManager.info.heroMonsterData[ rd.heroMonsters[j].id ].resource ].resource  == lines[i])
								{
									StartCoroutine(createHeroMonster(rd.heroMonsters[j]));
									return;

								}
							}
						}
					}
				}
			}



		}
	}



	public void killHeroMonster()
	{
		if(GameManager.me.stageManager.heroMonster != null)
		{
			foreach(Monster mon in GameManager.me.stageManager.heroMonster)
			{
				mon.damageDead();
			}
		}

		GameManager.me.stageManager.heroMonster = null;
	}


	IEnumerator createHeroMonster(StageMonsterData smd)
	{
		Vector3 _v;

		MonsterData md;

		if(GameManager.me.stageManager.heroMonster != null)
		{
			foreach(Monster mon in GameManager.me.stageManager.heroMonster)
			{
				mon.damageDead();
			}
		}

		try
		{
			md = GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource];
		}
		catch
		{
			Debug.Log(GameManager.info.heroMonsterData[smd.id].resource);
			Debug.Log(GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource]);
		}
		
		md = GameManager.info.monsterData[GameManager.info.heroMonsterData[smd.id].resource];
		GameManager.gameDataManager.addLoadModelData(md);
		
		
		if(smd.units != null)
		{
			foreach(string unitId in smd.units)
			{
				try
				{
					md = GameManager.info.monsterData[GameManager.info.unitData[unitId].resource];
					GameManager.gameDataManager.addLoadModelData(md.resource,md,md);

				}
				catch
				{
					Debug.LogError(unitId);
				}
			}
		}
		
		
		
		GameManager.me.effectManager.loadEffectFromHeroMonsterData( GameManager.info.heroMonsterData[smd.id] );
			
		if(smd.skills != null)
		{
			for(int i = smd.skills.Length - 1; i >= 0; --i)
			{
				GameManager.me.effectManager.loadEffectFromHeroSkillData(smd.skills[i]);
			}
		}
			
		if(smd.units != null)
		{
			for(int i = smd.units.Length - 1; i >= 0; --i)
			{
				GameManager.me.effectManager.loadEffectFromUnitData(smd.units[i]);
			}
		}

		GameManager.me.effectManager.startLoadEffects(true);
		GameDataManager.instance.startModelLoad(true);

		yield return null;

		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}
		
		while(GameDataManager.instance.isCompleteLoadModel == false)
		{
			yield return null;
		}


		_v.x = monsterPosX;
		_v.y = 0;
		_v.z = 0;
		
		GameManager.me.stageManager.heroMonster = new Monster[1];
		GameManager.me.stageManager.heroMonster[0] =  GameManager.me.mapManager.addMonsterToStage(null, null, false, smd, smd.id ,_v);

	}











	List<GameIDData> unitData = new List<GameIDData>();

	public void loadDebugAllUnit(string units, bool isMirrorType = false, string[] inputLines = null)
	{
		if(Application.isPlaying)
		{

			string[] lines = null;

			if(inputLines == null)
			{
				units = units.Trim();
				lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);
			}
			else
			{
				lines = inputLines;
			}

			if(lines == null) return;

			if(lines[0].StartsWith("SK")) return;

			unitData.Clear();

			for(int i = 0; i < lines.Length ; ++i)
			{
				if(string.IsNullOrEmpty( lines[i] ) == false && lines[i].Length < 6)
				{
					if(lines[i].StartsWith("UN"))
					{
						lines[i] += "02001";
					}
					else if(lines[i].StartsWith("EU"))
					{
						lines[i] += "06001";
					}


				}

				if(string.IsNullOrEmpty( lines[i] ) == false && (lines[i].StartsWith("UN") || lines[i].StartsWith("EU")))
				{
					GameIDData gd = new GameIDData();
					gd.parse(lines[i], GameIDData.Type.Unit);
					unitData.Add(gd);

					GameManager.me.effectManager.loadEffectFromUnitData(gd.unitData.id);

					MonsterData md = GameManager.info.monsterData[gd.unitData.resource];
					GameDataManager.instance.addLoadModelData(md);

				}
			}

			GameManager.me.effectManager.startLoadEffects(true);
			GameDataManager.instance.startModelLoad(true);

			if(isMirrorType)
			{

				StartCoroutine(createMirror());
			}
			else
			{
				StartCoroutine(checkUnits());
			}
		}
	}



	IEnumerator createMirror()
	{
		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}
		
		while(GameDataManager.instance.isCompleteLoadModel == false)
		{
			yield return null;
		}

		killMonsterUnits();
		deleteUnit();

		for(int i = 0; i < unitData.Count; ++i)
		{
			GameManager.me.mapManager.addMonsterToStage(null, null, true, null, unitData[i].id , new Vector3(unitPosX, 0, 0  ));
		}

		for(int i = 0; i < unitData.Count; ++i)
		{
			GameManager.me.mapManager.addMonsterToStage(null, null, false, null, unitData[i].id , new Vector3(monsterPosX, 0, 0  ));
		}

		unitData.Clear();
	}






	List<Monster> nowMonster = new List<Monster>();

	IEnumerator checkUnits()
	{
		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}

		while(GameDataManager.instance.isCompleteLoadModel == false)
		{
			yield return null;
		}

		for(int i = 0; i < unitData.Count; ++i)
		{
			 GameManager.me.mapManager.addMonsterToStage(null, null, true, null, unitData[i].id , new Vector3(100 + i*150, 0, 0  ));
		}
	}


	public void debugAllAttack(bool isPlayerSide = true)
	{
		if(isPlayerSide)
		{
			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				if(mon.isPlayer) continue;
				if(GameManager.me.characterManager.monsters.Count > 0)
				{
					mon.setTarget(GameManager.me.characterManager.monsters[0]);
					mon.unitData.attackType.lookTargetAndAttack(mon, GameManager.me.characterManager.monsters[0], mon.unitData.attackType.isShortType);
				}
			}
		}
		else
		{
			foreach(Monster mon in GameManager.me.characterManager.monsters)
			{
				if(mon.isPlayer || mon.isHero) continue;
				if(GameManager.me.characterManager.playerMonster.Count > 0)
				{
					mon.setTarget(GameManager.me.characterManager.playerMonster[0]);
					mon.unitData.attackType.lookTargetAndAttack(mon, GameManager.me.characterManager.playerMonster[0], mon.unitData.attackType.isShortType);
				}
			}
		}

	}


	public void debugAllSkill(bool isPlayerSide = true)
	{
		if(isPlayerSide)
		{
			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				if(mon.isPlayer) continue;
				int slen = 0;
				if(mon.unitData.skill != null) slen = mon.unitData.skill.Length;
				
				for(int i = 0; i < slen ; ++i)
				{
					if(GameManager.info.unitSkillData[mon.unitData.skill[i]].activeSkillCooltime > -1)
					{
						GameManager.info.unitSkillData[mon.unitData.skill[i]].doActiveSkill(mon);
					}
				}
			}

			return;
		}

		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			if(mon.isPlayer || mon.isHero) continue;
			int slen = 0;
			if(mon.unitData.skill != null) slen = mon.unitData.skill.Length;
			
			for(int i = 0; i < slen ; ++i)
			{
				if(GameManager.info.unitSkillData[mon.unitData.skill[i]].activeSkillCooltime > -1)
				{
					GameManager.info.unitSkillData[mon.unitData.skill[i]].doActiveSkill(mon);
				}
			}
		}

	}


	public void debugAllRemove()
	{
		foreach(Monster mon in GameManager.me.characterManager.playerMonster)
		{
			if(mon.isPlayer == false)
			{
				mon.dead();
			}
		}
	}




	public void playMonsterHeroAttack(bool isDefaultAttack, int skillIndex)
	{
		string t = (isDefaultAttack)?"0":"1";
		t += ",";
		t += skillIndex;

		if(GameManager.me.stageManager.heroMonster != null)
		{
			foreach(Monster mon in GameManager.me.stageManager.heroMonster)
			{
				if(mon != null) mon.action.setData(t);
			}
		}
	}



	public void playMonsterHeroAni(int index)
	{
		if(GameManager.me.stageManager.heroMonster != null)
		{
			foreach(Monster mon in GameManager.me.stageManager.heroMonster)
			{
				if(mon != null)
				{
					mon.action.setData("ANI,atk"+index);
					//mon.playAni("atk"+index);
				}
			}
		}
	}



	public void setActive(bool isActive)
	{

		foreach(GameObject go in gameObjectForDebugging)
		{
			if(go == null) continue;
			go.SetActive(isActive);
		}

	}



	public void soundOnOff()
	{
		GameManager.me.isMuteSFX = !GameManager.me.isMuteSFX;
		GameManager.soundManager.muteSFX(GameManager.me.isMuteSFX);
	}



	private List<MapData> _map = new List<MapData>();
	private int _mapIndex = 0;
	public string map = "";
	public int mapId = -1;

	public string currentRoundId = "";

	public void nextMap()
	{
		if(_map.Count == 0)
		{
			foreach(KeyValuePair<int, MapData> kv in GameManager.info.mapData)
			{
				_map.Add(kv.Value);
			}

			_mapIndex = 0;
		}


		if(mapId > 0 && GameManager.info.mapData.ContainsKey(mapId))
		{
			for(int i = 0 ; i < _map.Count; ++i)
			{
				if(_map[i].id == mapId)
				{
					_mapIndex = i;
					break;
				}
			}
		}


		GameManager.me.mapManager.clearStage();

		StartCoroutine(loadMap());
	}


	IEnumerator loadMap()
	{
		GameDataManager.instance.addLoadMapData(_map[_mapIndex].resource);

		GameDataManager.instance.startMapLoad(true);

		while(GameDataManager.instance.isCompleteLoadMap == false)
		{
			yield return null;
		}
		
		GameManager.me.mapManager.createBackground(_map[_mapIndex].id, true);

		GameManager.me.mapManager.visible = true;

		map = _map[_mapIndex].id + "  " + _map[_mapIndex].resource;

		++_mapIndex;
		if(_mapIndex >= _map.Count) _mapIndex = 0;

		GameManager.me.clearMemory();

	}


	private List<RoundData> _rounds = new List<RoundData>();

	int _epicIndex = 0;
	public void nextStage()
	{
		if(_rounds.Count == 0)
		{
			foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
			{
				if(kv.Value.id != "INTRO" && 
				   kv.Value.id.Contains("B_TEST") == false &&
				   kv.Value.id != "TEST" &&
				   kv.Value.id.StartsWith("C_") == false &&
				   kv.Value.id.StartsWith("CS_") == false)
				{
					_rounds.Add(kv.Value);
				}
			}

			_epicIndex = 0;
		}

		GameManager.me.characterManager.clearStage(true);
		GameManager.me.mapManager.clearStage();

		GameManager.me.clearMemory();

		if(_rounds.Count > _epicIndex)
		{
			StartCoroutine(nextStageCT(_rounds[_epicIndex]));
			
			++_epicIndex;
		}
		else
		{
			Debug.LogError("COMPLETE!!!");
		}


	}


	IEnumerator nextStageCT(RoundData rd)
	{
		currentRoundId = rd.id;

		GameManager.me.stageManager.setNowRound(rd, GameType.Mode.Epic);

		GameManager.me.loadRoundMonsterModelData();
		
		while(GameManager.gameDataManager.isCompleteLoadMap == false){ yield return null; };
		while(GameManager.gameDataManager.isCompleteLoadModel == false){ yield return null; };
		while(SoundManager.instance.isCompleteLoadSound == false){ yield return null; };
		while(GameManager.me.effectManager.isCompleteLoadEffect == false){ yield return null; };

		GameManager.me.mapManager.isSetStage = false;

		GameManager.me.mapManager.setStage(rd);


		SoundManager.instance.loadCutSceneSoundAsset(null);
		while(SoundManager.nowLoadingCutSceneAsset) { yield return null;  };

		GameManager.me.mapManager.createBackground( StageManager.instance.getMapId(UIPlay.playerLeagueGrade), true);

		GameManager.me.stageManager.playTime = 1;

		Debug.LogError(rd.id + " COMPLETE!!");

		yield return new WaitForSeconds(0.5f);

		nextStage();
	}


















	public void loadAllModel()
	{
		if(Application.isPlaying)
		{
			foreach(KeyValuePair<string, MonsterData> kv in GameManager.info.monsterData)
			{

				GameDataManager.instance.addLoadModelData(kv.Value);
			}

			GameDataManager.instance.startModelLoad(true);

			StartCoroutine(checkAllModel());

		}
	}


	IEnumerator checkAllModel()
	{
		while(GameManager.me.effectManager.isCompleteLoadEffect == false)
		{
			yield return null;
		}
		
		while(GameDataManager.instance.isCompleteLoadModel == false)
		{
			yield return null;
		}


		int index = 0;

		foreach(KeyValuePair<string, MonsterData> kv in GameManager.info.monsterData)
		{
			Monster mon = GameManager.me.characterManager.getMonster(false,true,kv.Value.id,false);

			Vector3 v = new Vector3(( index / 3 ) * 100, 0, -200 + (index%3) * 200 );
			Debug.Log(index + "   " + v);
			mon.setPositionCtransform(v);

			++index;
		}
	}




	public void killMonsterUnits(bool killAllUnits = false)
	{

		bool hasHero = false;

		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			if(mon.heroMonsterData != null )
			{
				hasHero = true;
				break;
			}
		}

		if(hasHero == false && killAllUnits) hasHero = true;

		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			if(mon.unitData != null && mon.heroMonsterData == null && mon.isHero == false)
			{
				if(hasHero == false)
				{
					hasHero = true;
					continue;
				}
				mon.dead();
			}
		}
	}


	public void repositionAll()
	{
		Vector3 v;

		foreach(Monster mon in GameManager.me.characterManager.monsters)
		{
			v = mon.cTransform.position;
			v.y = mon.cTransformPosition.y;
			mon.setPositionCtransform(v);
			mon.waitEnemy = false;
			if(v.x >= 90000)
			{
				mon.damageDead();
			}
		}


		foreach(Monster mon in GameManager.me.characterManager.playerMonster)
		{
			v = mon.cTransform.position;
			v.y = mon.cTransformPosition.y;
			mon.setPositionCtransform(v);
		}

	}



	public void convertClipboardDataToAniData(string clipboard)
	{
		if(Application.isPlaying)
		{
			clipboard = clipboard.Trim();

			string[] lines = clipboard.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);
			
			int len = lines.Length;
			
			for(int i = 0; i < len; ++i)
			{
				if(string.IsNullOrEmpty(lines[i])) continue;
				
				string[] singleLines = Util.CsvParser(lines[i]);
				
				if(singleLines[0] != "ANIDATA")
				{
					Debug.LogError("잘못된 데이터입니다.");
				}
				else
				{
					AniData aniData = new AniData();
					aniData.setData(singleLines);
					
					GameManager.info.aniData[aniData.id][aniData.ani] = aniData;
				}
			}


			for(int i = 0; i < GameManager.me.characterManager.playerMonster.Count; ++i)
			{
				GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(GameManager.me.characterManager.playerMonster[i]);
			}

			for(int i = 0; i < GameManager.me.characterManager.monsters.Count; ++i)
			{
				GameManager.me.characterManager.setCharacterShootingPointAndEffectContainer(GameManager.me.characterManager.monsters[i]);
			}


		}
	}
		













	public void loadModel(string units)
	{
		units = units.Trim();
		string[] lines = units.Split( new string[]{"\r\n","\n"}, System.StringSplitOptions.None);
		
		if(lines[0].StartsWith("SK")) return;

		bool isUnit = false;

		List<string> tempUnits = new List<string>();
		List<string> tempHeroMonsters = new List<string>();

		for(int i = 0; i < lines.Length ; ++i)
		{
			string fileName = lines[i];

			if(GameManager.info.modelData.ContainsKey(lines[i]) == false)
			{
				continue;
			}

			UnitData ud;

			foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseUnitData)
			{
				ud = kv.Value;

				if(  GameManager.info.modelData[ GameManager.info.monsterData[ud.resource].resource ].fileName == lines[i] )
				{
					isUnit = true;
					lines[i] = ud.id;
					break;
				}
			}


			foreach(KeyValuePair<string, UnitData> kv in GameManager.info.baseMonsterUnitData)
			{
				ud = kv.Value;

				if(  GameManager.info.modelData[ GameManager.info.monsterData[ud.resource].resource ].fileName == lines[i] )
				{
					isUnit = true;
					lines[i] = ud.id;
					break;
				}
			}


			if(isUnit)
			{
				if(string.IsNullOrEmpty( lines[i] ) == false && lines[i].Length < 6 && lines[i].Length > 2)
				{
					if(lines[i].StartsWith("UN"))
					{
						lines[i] += "02001";
					}
					else if(lines[i].StartsWith("EU"))
					{
						lines[i] += "02001";
					}
				}

				tempUnits.Add(lines[i]);
			}
			else
			{
				tempHeroMonsters.Add(lines[i]);
			}
		}

		if(tempUnits.Count > 0)
		{
			loadDebugAllUnit(null,false,tempUnits.ToArray());
		}

		if(tempHeroMonsters.Count > 0)
		{
			foreach(string s in tempHeroMonsters)
			{
				createHeroMonster(s);
			}

		}
	}




}

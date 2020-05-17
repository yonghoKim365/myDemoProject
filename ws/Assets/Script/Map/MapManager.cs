using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MapManager : MonoBehaviour , IManagerBase
{

	public tk2dSlicedSprite[] spHuntZone = new tk2dSlicedSprite[5];


	public ChangeStageTransition changeThemeAnimationStart;
	
	public Transform mapStage;

	public Transform waitZone;

	
	public HitObject playerHitObject;

	public HitObject playerNewHitObjectX = new HitObject();
	public HitObject playerNewHitObjectY = new HitObject();
	
	private Vector3 _v;
	
	public static int top = 200;

	public static int bottom = -200;

	public static int summonTop = 150;
	public static int summonBottom = -150;
	
	public static float mapHeight = 400.0f;
	public static float mapSummonHeight = 300.0f;
	
	[HideInInspector]
	public bool isCreated = false;
	
	public MapData loadedMap = null;
	public MapData inGameMap = null;

	public Transform mapFileParent;



	public Stack<MapData> loadedMapList = new Stack<MapData>();

	public PVPGradeSlotManager pvpGradeSlotManager;

	public ParticleEffect effectArrive = null;


	public void hideMap()
	{
		if(inGameMap != null && inGameMap.mapFile != null)
		{
			inGameMap.setVisible(false);
		}
		
		if(loadedMap != null)
		{
			if(loadedMapList.Contains(loadedMap) == false)
			{
				loadedMapList.Push(loadedMap);
			}
			
			loadedMap.setVisible(false);
		}

		GameManager.me.gameCamera.backgroundColor = Color.black;

		loadedMap = null;
	}




	public void createBackground(int mapId, bool gameMap = false, bool preLoad = false)
	{
		if(GameManager.info.mapData.ContainsKey(mapId) == false)// || PerformanceManager.isLowPc)
		{
			return;
		}

		if(gameMap)
		{
			if(loadedMap != null)
			{
				if(loadedMapList.Contains(loadedMap) == false)
				{
					loadedMapList.Push(loadedMap);
				}
				
				loadedMap.setVisible(false);
				loadedMap = null;
			}

			if(inGameMap != null)
			{
				if(loadedMapList.Contains(inGameMap) == false)
				{
					loadedMapList.Push(inGameMap);
				}
				
				inGameMap.setVisible(false);
				inGameMap = null;
			}

			inGameMap = GameManager.info.mapData[mapId];
			inGameMap.loadMap(mapFileParent);
			inGameMap.mapFile.gameObject.name = "IN_GAME_MAP : "+mapId;

			loadedMap = inGameMap;
		}
		else
		{
			if(inGameMap != null && inGameMap.mapFile != null)
			{
				inGameMap.setVisible(false);
			}

			if(loadedMap != null)
			{
				if(loadedMapList.Contains(loadedMap) == false)
				{
					loadedMapList.Push(loadedMap);
				}

				loadedMap.setVisible(false);
			}

			loadedMap = null;
			loadedMap = GameManager.info.mapData[mapId];
			loadedMap.loadMap(mapFileParent);

#if UNITY_EDITOR
			loadedMap.mapFile.gameObject.name = "LOADMAP : " + mapId;
#endif

			if(preLoad)
			{
				loadedMap.setVisible(false);
			}
		}

		if(loadedMap.mapFile.gameObject.activeSelf)
		{
			GameManager.me.gameCamera.backgroundColor = Color.black;
			//GameManager.me.gameCamera.backgroundColor = new Color((float)loadedMap.skyColor[0]/255.0f, (float)loadedMap.skyColor[1]/255.0f, (float)loadedMap.skyColor[2]/255.0f );
		}

		if(GameManager.info.mapData[mapId].useFlare && PerformanceManager.isLowPc == false)
		{
			GameManager.me.flareLayer.isEnabled = true;
		}
	}


	private int _distanceSettingDelay = 0;
	private int _distanceDisplayValue = 0;
	private float _specialSpeedDuration = 0.0f;
	
	
	public bool stopScroll = false;
	
	
	private float _tempSS;
	
	public void update()
	{
		// 맵 오브젝트들을 갱신. 화면 아래로 내려가면 삭제하고, 주인공이랑 부딪치면 통과시킬지 여부를 결정한다.
		// 굳이 주인공을 여기서 갱신시키는건 조금이라서 연상을 줄이기 위함.
		// 주인공 충돌을 한번이 가로, 세로로 하는 이유는 올라가다 막혔을때 좌우로는 움직이게 하기 위함이다.
		mapObjectUpdate();
	}

	public void render()
	{
		for(int i = currentMapObjects.Count - 1; i >= 0; --i)
		{
			currentMapObjects[i].LateUpdate();
		}
	}


	private bool _isPlayerHitCheck = false;
	
	
	private void mapObjectUpdate()
	{
		//Log.log("isPlayerMovable : " + isPlayerMovable + " moveDirection: " + moveDirection + " deltaPlayerPosY : " + deltaPlayerPosY);
		
		_isPlayerHitCheck = false;
		
		playerHitObject = GameManager.me.player.getHitObject();
		
		for(int i = currentMapObjects.Count - 1; i >= 0; --i)
		{
			currentMapObjects[i].update();
			
			if( currentMapObjects[i].isEnabled)
			{
				// 그냥 통과한다. 단 item인 경우는 충돌검사후 아이템을 먹은 효과를 보여준다.
				if(currentMapObjects[i].mapObjectData.type.Equals("ITEM"))
				{
					if(currentMapObjects[i].hitObject.intersects(playerHitObject))
					{
						currentMapObjects[i].isDeleteObject = true;
					}

					// 아이템을 먹은 상태다.
					if(currentMapObjects[i].isDeleteObject)
					{
						_v = currentMapObjects[i].transform.position;
						_v.x += currentMapObjects[i].hitObject.width / 2.0f;
						_v.y += currentMapObjects[i].hitObject.height / 2.0f;
						
						doGetItem(_v, currentMapObjects[i].mapObjectData.id);
					}
				}
			}
			
			// 화면 아래로 내려간 녀석들은 삭제를 해야한다.
			if(currentMapObjects[i].isDeleteObject)
			{
				setMapObject(currentMapObjects[i], i);				
			}
		}
	}
	
	
	
	public void doGetItem(Vector3 getItemPosition, string itemId)
	{
		GameManager.me.effectManager.getGetItemEffect().start(getItemPosition, false);

		//GameManager.soundManager.playEffect("sounds/get_coin");			

		SoundData.play("uibt_countdown");

		
		if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.GETITEM)
		{
			if(leftGetItemNum.ContainsKey(itemId))
			{
				--leftGetItemNum[itemId];	
				GameManager.me.effectManager.startGetUIItemEffect(GameManager.me.player.cTransform.position, itemId);
			}
			
			GameManager.me.stageManager.clearChecker();
		}		
		
	}
	
	
	

	
	
	
	public Vector3 hitObjectPosition;
	private bool _isHitWithBullet = false;

	
	public MapObject addMapObjectToStage(string mapObjectId, IVector3 position)
	{
		MapObjectData mobjData = GameManager.info.mapObjectData[mapObjectId];

		MapObject mobj = getMapObject();
		
		mobj.init(mobjData, position);	
	
		currentMapObjects.Add(mobj);	
		
		return mobj;
	}
	
	
	


	
	public Xint monUnitNum = 0;
	public Xint bossNum = 0;
	
	public Xint leftKilledMonster = -999;

	public Xint killedUnitCount = 0;

	
	public Dictionary<string, Xint> leftKilledMonsterNum = new Dictionary<string, Xint>(StringComparer.Ordinal);
	public Dictionary<string, Xint> leftDestroyObjectNum = new Dictionary<string, Xint>(StringComparer.Ordinal);
	public Dictionary<string, Xint> leftGetItemNum = new Dictionary<string, Xint>(StringComparer.Ordinal);

	public int[] rankData;

	
	// 보스를 죽였다면!!!!
	public void clearRound()
	{
		GameManager.me.characterManager.restoreMonsterVisibleForSkillCam();
		GameManager.me.currentScene = Scene.STATE.PLAY_CLEAR_SUCCESS;
	}

	
	private MonsterCategory.Category _tempMonCategory = MonsterCategory.Category.UNIT;	
	private string _tempMonResource = "";	
	private Monster.TYPE _tempMonType;

	public Monster addMonsterToStage(TranscendData td, int[] transcendLevel, bool isPlayerMon, StageMonsterData stageMonData, string monId, Vector3 position, UnitData ud = null, bool addList = true)
	{
//		Log.log("addMonsterToStage : " + monId, " pos: " + position);
		
		Monster mon;
		
		bool isUnitMonster = (stageMonData == null || stageMonData.type == StageMonsterData.Type.UNIT);
		
		if(isUnitMonster)
		{
			_tempMonResource = GameManager.info.unitData[monId].resource;
			_tempMonType = Monster.TYPE.UNIT;
		}
		else if(stageMonData.type == StageMonsterData.Type.HERO)
		{
			_tempMonResource = GameManager.info.heroMonsterData[monId].resource;
			_tempMonType = Monster.TYPE.HERO;
		}
		else if(stageMonData.type == StageMonsterData.Type.NPC) 
		{
			_tempMonResource = GameManager.info.npcData[monId].resource;
			_tempMonType = Monster.TYPE.NPC;
		}

#if UNITY_EDITOR
		//Debug.Log(_tempMonCategory);
#endif

		if(_tempMonType == Monster.TYPE.UNIT)
		{
			mon = GameManager.me.characterManager.getMonster(false, isPlayerMon, _tempMonResource, addList);
			CharacterUtil.setRare(GameManager.info.unitData[monId].rare, mon);
		}
		else if(_tempMonType == Monster.TYPE.HERO)
		{
			if(GameManager.info.heroMonsterData[monId].isPlayerResourceType)
			{
				HeroMonsterData hd = GameManager.info.heroMonsterData[monId];
				mon = (Monster)GameManager.me.characterManager.getPlayerCharacter(hd.resource,hd.head,hd.body,hd.weapon,hd.vehicle,hd.faceTexture,false,addList);

				if(mon.action != null) GameManager.me.characterManager.setCharacterAction(mon.action, mon.category);
				mon.action = null;
				mon.category = MonsterCategory.Category.HEROMONSTER;
			}
			else
			{
				mon = GameManager.me.characterManager.getMonster(false, isPlayerMon, _tempMonResource, addList);
				mon.removeRareLine();
			}
		}
		else
		{
			mon = GameManager.me.characterManager.getMonster(false, isPlayerMon, _tempMonResource, addList);

			if(mon.monsterData.hasFaceAni == false)
			{
				//CharacterUtil.setRare(RareType.NORMAL, mon);
				mon.removeRareLine();
			}
		}

		//position.x += (float)(GameManager.inGameRandom.Range(0,101))*0.01f;//(float)GameManager.getRandomNum()*0.01f;

		//mon.setPositionCtransform( position );
		
		mon.init(td, transcendLevel, monId, isPlayerMon, _tempMonType, stageMonData);

		mon.setPositionCtransform( position );

//		Log.log("addMonsterToStage" + mon , mon.resourceId);

		mon.isCutSceneOnlyCharacter = false;
		
		_tempMonCategory = GameManager.info.monsterData[_tempMonResource].category;

		if(addList)
		{
			switch(_tempMonCategory)
			{
				case MonsterCategory.Category.HEROMONSTER:
					++bossNum;
					//Debug.LogError("bossNum : " + bossNum);
					break;
				case MonsterCategory.Category.UNIT:
					if(isPlayerMon == false)
					{
						monUnitNum.Set(monUnitNum+1);
					}
					//Debug.LogError("monUnitNum : " + monUnitNum);
					break;
			}
		}

		// 인트로 컷씬 게임용.
		if(GameManager.me.stageManager.isIntro) mon.hp = 100000000;

		mon.action.setFirstPosition(mon.cTransformPosition);	
		mon.setVisible(true);
		mon.isEnabled = true;
		
		return mon;
	}
	


	public Monster addPVPPlayerUnitToStage(TranscendData td, int[] transcendLevel, string monId, Vector3 position)
	{
		Monster mon;
		
		_tempMonResource = GameManager.info.unitData[monId].resource;
		_tempMonType = Monster.TYPE.UNIT;

		mon = GameManager.me.characterManager.getMonster(false, false, _tempMonResource, true);
		CharacterUtil.setRare(GameManager.info.unitData[monId].rare, mon);

		position.x -= (float)(GameManager.inGameRandom.Range(0,101))/100.0f;

//		Log.log("addPVPPlayerUnitToStage : " + mon.resourceId);

		mon.init(td, transcendLevel, true, monId, false, _tempMonType);

		mon.setPositionCtransform(  position );

		mon.isCutSceneOnlyCharacter = false;
		
		_tempMonCategory = GameManager.info.monsterData[_tempMonResource].category;
		
		mon.action.setFirstPosition(mon.cTransformPosition);	
		
		mon.setVisible(true);
		mon.isEnabled = true;
		
		return mon;
	}





	public Player addPlayerToStage(bool isPlayerMon, GamePlayerData pd, Vector3 position, bool addList = true, int playerTagIndex = 0)
	{
		Player player;

#if UNITY_EDITOR
		Debug.Log(pd.id);
#endif

		player = (Player)GameManager.me.characterManager.getMonster(true,false, pd.id, addList);

//		Log.log("addPlayerToStage : " + player, player.name);

		player.setPositionCtransform( position );
		
		player.isCutSceneOnlyCharacter = false;

		player.init(pd,isPlayerMon,true, playerTagIndex);

		player.setPositionCtransform( position );

		player.setVisible(true);
		player.isEnabled = true;


		player.pet = (Pet)GameManager.me.characterManager.getMonster(false,false,pd.partsVehicle.parts.resource.ToUpper(),false);
		player.pet.init(player);
		player.container.name = "PVP";
		if(player.hpBar != null)
		{
			player.hpBar.isEnabled = true;
			player.hpBar.visible = false;
		}



		return player;
	}



	public CharacterMinimapPointer targetPosMinimapPointer;

	public bool isSetStage = false;

	public static bool useZoomCamera = false;
	public static float zoomCameraTargetX = 0;

	public void setStage(RoundData rd)
	{
		// 실제 게임이 시작하기 바로 전에 호출.
		useZoomCamera = false;

		int index = 0;
		int i = 0;
		int len = 0;
		
		if(isSetStage)
		{
#if UNITY_EDITOR
			Debug.LogError("### Already Setting Stage!!!");
#endif
			return;
		}

		StageManager.mapStartPosX = rd.mapStartEndPosX[0];
		StageManager.mapEndPosX = rd.mapStartEndPosX[1];

		killedUnitCount = 0;
		bossNum = 0;
		monUnitNum = 0;
		GameManager.me.stageManager.protectNPC = null;
		GameManager.me.stageManager.chaser = null;
		GameManager.me.stageManager.playerDestroyObjectMonster = null;
		GameManager.me.stageManager.heroMonster = null;

		_v = GameManager.me.player.cTransformPosition;
		_v.x = rd.playerStartPosX;
		_v.z = 0.0f;
		GameManager.me.player.setPositionCtransform( _v );		

		GameManager.me.characterManager.playerMonster.Add(GameManager.me.player);

		if(rd.mode == RoundData.MODE.PVP)
		{
			GameManager.me.battleManager.initPlayer(_v);

			_v = GameManager.me.player.cTransformPosition;
			//_v.x = DebugManager.instance.pvpStartPoint;
			_v.x = 2450.0f;
			GameManager.me.pvpPlayer = addPlayerToStage(false,DebugManager.instance.pvpPlayerData, _v, true);

			GameManager.me.battleManager.initPVPPlayer(_v);

			BaseSkillData.enemyHero = GameManager.me.pvpPlayer;
			isSetStage = true;

			MapManager.zoomCameraTargetX = GameManager.me.pvpPlayer.cTransformPosition.x;

			useZoomCamera = true;

			return;
		}
		else
		{
			DebugManager.instance.useTagMatchMode = false;
			GameManager.me.battleManager.initPlayer(_v);
		}


#if UNITY_EDITOR

		if(UnitSkillCamMaker.instance.useEffectSkillCamEditor)
		{

		}
		else		
#endif
		{


			if(rd.heroMonsters.Length > 0)
			{
				
				index = 0;
				foreach(StageMonsterData data in rd.heroMonsters)
				{
					if(data.attr == null) ++index;
				}			
				
				if(index > 0)
				{
					GameManager.me.stageManager.heroMonster = new Monster[index];		
				}
				else GameManager.me.stageManager.heroMonster = null;
				
				
				index = 0;
				foreach(StageMonsterData data in rd.heroMonsters)
				{
					_v.x = data.posX;
					_v.y = data.posY;
					_v.z = data.posZ;
					
					if(data.attr == null)
					{
						GameManager.me.stageManager.heroMonster[index] = addMonsterToStage(null, null, false, data, data.id ,_v);
						++index;
					}
					else
					{
						// attr이 "H" 면 히든임...
						Monster mon = addMonsterToStage(null, null, false, data, data.id , _v, null, false);
						mon.setVisible(false);
						if(mon.miniMapPointer != null) mon.miniMapPointer.isEnabled = false;
						GameManager.me.characterManager.decoMonsters.Add( mon );
					}
				}
			}



		}
		


		if(rd.unitMonsters != null)
		{
			foreach(StageMonsterData data in rd.unitMonsters)
			{
				_v.x = data.posX;
				_v.y = data.posY;
				_v.z = data.posZ;
				
				if(data.attr != null)
				{
					if(data.attr == "V") // 적에게 쳐맞거나 상대가 위치에 왔을때 동작.
					{
						Monster mon = addMonsterToStage(null, null, false, null, data.id , _v);
						if(data.checkLine > -1000)
						{
							mon.waitEnemy = true;
							mon.waitLine = data.checkLine;
						}
					}
					else if(data.attr == "H") // 화면에 안보이다가 스폰...
					{
						
					}
				}
				else
				{
					addMonsterToStage(null, null, false, null, data.id , _v);	
				}
			}		
		}
		
		
		if(rd.decoObject != null)
		{
			foreach(StageMonsterData data in rd.decoObject)
			{
				_v.x = data.posX;
				_v.y = data.posY;
				_v.z = data.posZ;
				
				Monster mon = addMonsterToStage(null, null, false, data, data.id , _v, null, false);
				GameManager.me.characterManager.decoMonsters.Add( mon );

				if(data.angle > -1000)
				{
					Quaternion q = mon.tf.localRotation;
					_v = q.eulerAngles;
					_v.y = data.angle;
					q.eulerAngles = _v;
					mon.tf.localRotation = q;
				}

				if(data.attr != null)
				{
					string[] attr = data.attr.Split('-');
					if(string.IsNullOrEmpty(attr[0]) == false && mon.ani != null) mon.playAni(attr[0]);
					
					if(attr.Length > 1)
					{
						float.TryParse(attr[2], out _v.x);
						float.TryParse(attr[3], out _v.y);
						float.TryParse(attr[4], out _v.z);
						GameManager.info.effectData[attr[1]].getEffect(-1000,_v);
					}
				}
			}				
		}

		if(rd.blockObject != null)
		{
			foreach(StageMonsterData data in rd.blockObject)
			{
				_v.x = data.posX;
				_v.y = data.posY;
				_v.z = data.posZ;

				Monster dm = null;

				switch(data.attr)
				{
				case "0":
					dm = addMonsterToStage(null, null, false, data, data.id , _v);
					dm.isBlockMonster = false;
					dm.specialType = Monster.SpecialType.SKIP_DESTORY_CHECK;
					break;
				case "1":
					dm = addMonsterToStage(null, null, false, data, data.id , _v);
					dm.isBlockMonster = true;
					dm.specialType = Monster.SpecialType.SKIP_DESTORY_CHECK;
					break;
				case "2":
					dm = addMonsterToStage(null, null, true, data, data.id , _v);
					dm.isBlockMonster = false;
					dm.specialType = Monster.SpecialType.SKIP_DESTORY_CHECK;
					break;
				case "3":
					dm = addMonsterToStage(null, null, true, data, data.id , _v);
					dm.isBlockMonster = true;
					dm.specialType = Monster.SpecialType.SKIP_DESTORY_CHECK;
					break;
				}

				if(dm != null && data.angle > -1000)
				{
					Quaternion q = dm.tf.localRotation;
					_v = q.eulerAngles;
					_v.y = data.angle;
					q.eulerAngles = _v;
					dm.tf.localRotation = q;
				}

				dm = null;
			}	
		}



		if(rd.mode == RoundData.MODE.KILLCOUNT)
		{
			leftKilledMonsterNum.Clear();

			len = rd.killMonsterCount;
			for(i = 0; i < len; ++i)
			{
#if UNITY_EDITOR
				Debug.Log(rd.killMonsterIds[i] + "   " +  rd.killMonsterNum[i]);
#endif

				if(leftKilledMonsterNum.ContainsKey(rd.killMonsterIds[i]))
				{
					leftKilledMonsterNum[rd.killMonsterIds[i]] =  rd.killMonsterNum[i] ;
				}
				else
				{
					leftKilledMonsterNum.Add(rd.killMonsterIds[i] , rd.killMonsterNum[i] );
				}
			}
			
			int num = 0;
			foreach(KeyValuePair<string,Xint> kv in GameManager.me.mapManager.leftKilledMonsterNum)
			{
				num += kv.Value;
			}

			GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = num + "M";			
		}
		else if(rd.mode == RoundData.MODE.KILLCOUNT2)
		{
			leftKilledMonster = rd.killMonsterNum[0];
			GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = GameManager.me.mapManager.leftKilledMonster + "M";
		}				
		else if(rd.mode == RoundData.MODE.GETITEM)
		{
			if(rd.settingAttr2 != null) createDeadzoneMonster(rd);

			leftGetItemNum.Clear();
			int totalNum = 0;
			foreach(KeyValuePair<string, Xint> kv in rd.getItemData.needCount)
			{
				leftGetItemNum[kv.Key] = kv.Value;
				totalNum += kv.Value;
			}

			GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = totalNum + "";
			
		}
		else if(rd.mode == RoundData.MODE.SURVIVAL)
		{
			if(rd.settingAttr2 != null) createDeadzoneMonster(rd);
		}
		else if(rd.mode == RoundData.MODE.ARRIVE)
		{
			if(rd.chaser != null)
			{
				_v.x = rd.chaser.posX; _v.y = 0.0f; _v.z = rd.chaser.posZ;
				GameManager.me.stageManager.chaser = addMonsterToStage(null, null, false, rd.chaser, rd.chaser.id ,_v, null, false);
				GameManager.me.stageManager.chaser.miniMapPointer = GameManager.me.characterManager.getMinimapPointer(false,GameManager.me.stageManager.chaser.cTransform);
				GameManager.me.stageManager.chaser.initMiniMap();
				GameManager.me.stageManager.chaser.stat.speed = rd.chaser.speed;
				GameManager.me.stageManager.chaser.lookDirection(1000);
			}	
			
			if(rd.protectObject != null)
			{
				_v = GameManager.me.player.cTransformPosition;
				_v.x -= 150.0f;
				_v.z = rd.protectObject[0].posZ;

				if(GameManager.info.npcData[rd.protectObject[0].id].type == NPCData.Type.PLAYER)
				{
					GameManager.me.stageManager.protectNPC = (Monster)GameManager.me.characterManager.getNPCPlayerCharacter(rd.protectObject[0].id, true);// addMonsterToStage(true, rd.protectObject[0], rd.protectObject[0].id ,_v);
					GameManager.me.stageManager.protectNPC.setPositionCtransform( _v );
				}
				else
				{
					GameManager.me.stageManager.protectNPC = addMonsterToStage(null, null, true, rd.protectObject[0], rd.protectObject[0].id ,_v);
				}

				GameManager.me.stageManager.protectNPC.miniMapPointer.init(CharacterMinimapPointer.PROTECT, GameManager.me.stageManager.protectNPC.cTransform, 12);

				GameManager.me.characterManager.setCharacterAction(GameManager.me.stageManager.protectNPC.action,GameManager.me.stageManager.protectNPC.category);
				GameManager.me.stageManager.protectNPC.action = null;
				GameManager.me.stageManager.protectNPC.category = MonsterCategory.Category.PROTECT;
				GameManager.me.stageManager.protectNPC.action = GameManager.me.characterManager.getCharacterAction(GameManager.me.stageManager.protectNPC.category);
				GameManager.me.stageManager.protectNPC.action.init(GameManager.me.stageManager.protectNPC);

				GameManager.me.stageManager.protectNPC.action.targetPos.x = rd.protectObject[0].checkLine.Get();
			}


			targetPosMinimapPointer = GameManager.me.characterManager.getMinimapPointer(false,null);
			targetPosMinimapPointer.pointer.spriteName = CharacterMinimapPointer.DISTANCE;
			targetPosMinimapPointer.init(CharacterMinimapPointer.DISTANCE,null, 16, true);
			targetPosMinimapPointer.setPosition(rd.targetPos);
			targetPosMinimapPointer.isEnabled = true;

			_v.x = rd.targetPos + 50; _v.y = 5; _v.z = 0;
			effectArrive = GameManager.info.effectData["E_EVENT_ARRIVE_EFF"].getParticleEffect(-1000,_v,null,null);
		}
		else if(rd.mode == RoundData.MODE.PROTECT)
		{
			if(rd.settingAttr2 != null) createDeadzoneMonster(rd);

			index = 0;
			GameManager.me.stageManager.playerProtectObjectMonster = new Monster[rd.protectObject.Length];	
			GameManager.me.stageManager.protectObjMonCount = rd.protectObject.Length;
			foreach(StageMonsterData data in rd.protectObject)
			{
				_v.x = data.posX;
				_v.y = data.posY;
				_v.z = data.posZ;	

				Monster pMon; 

				if(GameManager.info.npcData[data.id].type == NPCData.Type.PLAYER)
				{
					pMon = (Monster)GameManager.me.characterManager.getNPCPlayerCharacter(data.id, true);
					pMon.setPositionCtransform( _v );
				}
				else
				{
					pMon = addMonsterToStage(null, null, true, data, data.id ,_v);
				}

				GameManager.me.stageManager.playerProtectObjectMonster[index] = pMon;
				pMon.miniMapPointer.init(CharacterMinimapPointer.PROTECT, pMon.cTransform, 12);


				GameManager.me.characterManager.setCharacterAction(pMon.action,pMon.category);
				pMon.action = null;
				pMon.category = MonsterCategory.Category.NPC;
				pMon.action = GameManager.me.characterManager.getCharacterAction(pMon.category);
				pMon.action.init(pMon);


				if(data.angle > -1000)
				{
					Quaternion q = pMon.tf.localRotation;
					_v = q.eulerAngles;
					_v.y = data.angle;
					q.eulerAngles = _v;
					pMon.tf.localRotation = q;
				}


				if(data.attr != null)
				{
					string[] attr = data.attr.Split('-');
					if(string.IsNullOrEmpty(attr[0]) == false && pMon.ani != null) pMon.playAni(attr[0]);

					if(attr.Length > 1)
					{
						float.TryParse(attr[2], out _v.x);
						float.TryParse(attr[3], out _v.y);
						float.TryParse(attr[4], out _v.z);
						GameManager.info.effectData[attr[1]].getEffect(-1000,_v);
					}
				}

//				Debug.LogError("pMon type! : " + pMon.stat.monsterType);

				++index;
			}
		}
		else if(rd.mode == RoundData.MODE.SNIPING)
		{
			#if UNITY_EDITOR
			
			if(UnitSkillCamMaker.instance.useEffectSkillCamEditor)
			{
				
			}
			else		
			#endif
			{
				GameManager.me.stageManager.heroMonster[rd.targetIndex].miniMapPointer.init(CharacterMinimapPointer.BOSS, GameManager.me.stageManager.heroMonster[rd.targetIndex].cTransform, 7);
			}
		}
		else if(rd.mode == RoundData.MODE.DESTROY)
		{
			if(rd.chaser != null)
			{
				_v.x = rd.chaser.posX; _v.y = 0.0f; _v.z = rd.chaser.posZ;
				GameManager.me.stageManager.chaser = addMonsterToStage(null, null, false, rd.chaser, rd.chaser.id ,_v, null, false);
				GameManager.me.stageManager.chaser.miniMapPointer = GameManager.me.characterManager.getMinimapPointer(false,GameManager.me.stageManager.chaser.cTransform);
				GameManager.me.stageManager.chaser.initMiniMap();
				GameManager.me.stageManager.chaser.stat.speed = rd.chaser.speed;
				GameManager.me.stageManager.chaser.lookDirection(1000);
			}
			
			leftDestroyObjectNum.Clear();
			index = 0;
			GameManager.me.stageManager.playerDestroyObjectMonster = new Monster[rd.destroyObject.Length];	
			
			foreach(StageMonsterData data in rd.destroyObject)
			{
				if(leftDestroyObjectNum.ContainsKey(data.id) == false)
				{
					leftDestroyObjectNum[data.id] = 1;
				}
				else
				{
					++leftDestroyObjectNum[data.id];	
				}
				
				_v.x = data.posX;
				_v.y = data.posY;
				_v.z = data.posZ;				
				GameManager.me.stageManager.playerDestroyObjectMonster[index] = addMonsterToStage(null, null, false, data, data.id ,_v);
				GameManager.me.stageManager.playerDestroyObjectMonster[index].stat.speed = data.speed;


				if(data.angle > -1000)
				{
					Quaternion q = GameManager.me.stageManager.playerDestroyObjectMonster[index].tf.localRotation;
					_v = q.eulerAngles;
					_v.y = data.angle;
					q.eulerAngles = _v;
					GameManager.me.stageManager.playerDestroyObjectMonster[index].tf.localRotation = q;
				}

				++index;
			}
			
			int num = 0;
			foreach(KeyValuePair<string,Xint> kv in leftDestroyObjectNum)
			{
				num += kv.Value;
			}
			
			GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = num + "";
		}
		else if(rd.mode == RoundData.MODE.C_RUN) // 무한질주
		{


			string[] cData = rd.settingAttr.Split('/');
			string[] cRun = cData[0].Split(',');
			int cBossNum = 0;
			float startX = 0;
			float xOffset = 0;
			float checkDistance = 0;
			float.TryParse(cRun[1],out startX);
			float.TryParse(cRun[2],out xOffset);
			int.TryParse(cRun[3],out cBossNum);
			float.TryParse(cRun[4],out checkDistance);

			float[] constValue = Util.stringToFloatArray(cData[1],',');

			for(i = 0; i < cBossNum; ++i)
			{
				_v.x = startX + (i*xOffset);
				_v.y = 0.0f;
				_v.z = 0.0f;
				Monster mon = addMonsterToStage(null, null, false,rd.challengeData,cRun[0],_v);
				GameManager.me.characterManager.setCharacterAction(mon.action,mon.category);
				mon.action = null;
				mon.category = MonsterCategory.Category.CHALLENGE_RUN_BOSS;
				mon.action = GameManager.me.characterManager.getCharacterAction(mon.category);
				mon.action.init(mon);
				mon.name = "RUN_BOSS";

				ChallengeInfinityRun cr = new ChallengeInfinityRun();
				cr.checkDistance = checkDistance;
				cr.summonDelay = (float)(cBossNum-i)/(float)cBossNum * constValue[0] + constValue[1];
				cr.summonDelay *= 0.001f;
				cr.summonUnits = rd.units;
				cr.rankData = Util.stringToIntArray(cData[2],',');
				rankData = cr.rankData;
				mon.action.setData((object)cr);
				mon.initMiniMap();
			}

			GameManager.me.uiManager.uiPlay.challangeModeInfo.init(rd.mode,rankData[0],rankData[1],rankData[2]);
		}
		else if(rd.mode == RoundData.MODE.C_SURVIVAL) // 무한 생존
		{
			string[] cData = rd.settingAttr.Split('/');
			float[] csData = Util.stringToFloatArray(cData[0],',');
			_v.x = csData[0];
			_v.y = 0.0f;
			_v.z = 0.0f;
			Monster mon = addMonsterToStage(null, null, false,null,rd.challengeData.id,_v,null,false);
			GameManager.me.characterManager.setCharacterAction(mon.action,mon.category);
			mon.action = null;
			mon.category = MonsterCategory.Category.CHALLENGE_SURVIVAL_BOSS;
			mon.action = GameManager.me.characterManager.getCharacterAction(mon.category);
			mon.action.init(mon);
			mon.name = "BOSS";

			ChallengeInfinitySurvival cs = new ChallengeInfinitySurvival();
			cs.constValue = Util.stringToFloatArray(cData[1],',');
			cs.rankData = Util.stringToIntArray(cData[2],',');
			rankData = cs.rankData;
			cs.summonUnits = rd.units;
			cs.hitRadius = csData[1];
			mon.action.setData((object)cs);
			mon.setVisible(false);
			if(mon.miniMapPointer != null) mon.miniMapPointer.isEnabled = false;
			GameManager.me.characterManager.decoMonsters.Add( mon );

			GameManager.me.uiManager.uiPlay.challangeModeInfo.init(rd.mode,rankData[0],rankData[1],rankData[2]);


			useZoomCamera = true;
			zoomCameraTargetX = csData[0];

		}
		else if(rd.mode == RoundData.MODE.C_HUNT) // 무한 사냥.
		{
			string[] cData = rd.settingAttr.Split('/');

			ChallengeInfinityHunt ch = new ChallengeInfinityHunt();
			ch.summonDelays = Util.stringToFloatArray(cData[2],',');
			ch.rankData = Util.stringToIntArray(cData[3],',');
			rankData = ch.rankData;
			ch.summonUnits = rd.units;
			float[] distanceData = Util.stringToFloatArray(cData[1],',');

			float startPosX = 0.0f;

			float.TryParse(cData[0],out startPosX);

			ch.checkLevelDistance = new float[5];
			ch.checkLevelDistance[0] = startPosX;

			for(int li = 0; li < 4; ++li)
			{
				ch.checkLevelDistance[li+1] = distanceData[li] + ch.checkLevelDistance[li];
			}

			_v.x = startPosX;
			foreach(float w in distanceData)
			{
				_v.x += w;
			}

			_v.y = 0.0f;
			_v.z = 0.0f;
			Monster mon = addMonsterToStage(null, null, false,null,rd.challengeData.id,_v,null,false);
			GameManager.me.characterManager.setCharacterAction(mon.action,mon.category);
			mon.action = null;
			mon.category = MonsterCategory.Category.CHALLENGE_HUNT_BOSS;
			mon.action = GameManager.me.characterManager.getCharacterAction(mon.category);
			mon.action.init(mon);
			mon.name = "BOSS";

			mon.action.setData((object)ch);
			mon.setVisible(false);
			if(mon.miniMapPointer != null) mon.miniMapPointer.isEnabled = false;
			GameManager.me.characterManager.decoMonsters.Add( mon );	

			GameManager.me.uiManager.uiPlay.challangeModeInfo.init(rd.mode,rankData[0],rankData[1],rankData[2]);

			useZoomCamera = true;
			zoomCameraTargetX = _v.x;

		}


		else if(rd.mode == RoundData.MODE.B_TEST) // 무한질주
		{
			Monster mon = addMonsterToStage(null, null, false,null,rd.challengeData.id,_v,null,false);
			GameManager.me.characterManager.setCharacterAction(mon.action,mon.category);
			mon.action = null;
			mon.category = MonsterCategory.Category.B_TEST_BOSS;
			mon.action = GameManager.me.characterManager.getCharacterAction(mon.category);
			mon.action.init(mon);
#if UNITY_EDITOR
			mon.name = "BOSS";
#endif
			
			mon.action.setData((object)rd.settingAttr);

			mon.setVisible(false);
			if(mon.miniMapPointer != null) mon.miniMapPointer.isEnabled = false;
			GameManager.me.characterManager.decoMonsters.Add( mon );	

			GameManager.me.uiManager.uiPlay.challangeModeInfo.init(rd.mode,9999,9999,9999);
		}

		isSetStage = true;
	}
	
	

	void createDeadzoneMonster(RoundData rd)
	{
		if(rd.challengeData != null)
		{
			float[] csData = Util.stringToFloatArray(rd.settingAttr2, ',');

			_v.x = csData[0];
			_v.y = 0.0f;
			_v.z = 0.0f;

			Monster mon = addMonsterToStage(null, null, false,null,rd.challengeData.id,_v,null,false);
			GameManager.me.characterManager.setCharacterAction(mon.action,mon.category);
			mon.action = null;
			mon.category = MonsterCategory.Category.MONSTER_DEAD_ZONE;
			mon.action = GameManager.me.characterManager.getCharacterAction(mon.category);
			mon.action.init(mon);
			mon.name = "BOSS";

			ChallengeInfinitySurvival cs = new ChallengeInfinitySurvival();
			cs.hitRadius = csData[1];
			mon.action.setData((object)cs);
			mon.setVisible(false);

			if(mon.miniMapPointer == null)
			{
				mon.miniMapPointer = GameManager.me.characterManager.getMinimapPointer(false,null);
			}

			if(mon.miniMapPointer != null)
			{
				mon.miniMapPointer.pointer.spriteName = CharacterMinimapPointer.CHASER;
				mon.miniMapPointer.init(CharacterMinimapPointer.CHASER,null, 15, true);
				mon.miniMapPointer.setPosition(_v.x);
				mon.miniMapPointer.isEnabled = true;
			}

			GameManager.me.characterManager.decoMonsters.Add( mon );
		}
	}

	
	/*	
	private void startChangeStage(float waitTime = 5.0f)
	{
		System.Action delayFunc = startChangeStageThemeAnimation;
		MethodManager.addInGameDelayFunc(waitTime, delayFunc );		
	}
	
	
	private void startChangeStageThemeAnimation()
	{
		changeThemeAnimationStart.animation.Play();		
	}
	
	
	public void startChangeStageTheme()
	{
		StartCoroutine(stageCleaning());
	}
	
	
	public IEnumerator stageCleaning()
	{
		foreach(AnimationState state in changeThemeAnimationStart.animation)
		{
			state.speed = 0.0f;
		}	
		yield return new WaitForEndOfFrame();
		
		yield return null;
		
		GameManager.me.clearStage();	
		
		yield return null;
		
		yield return new WaitForEndOfFrame();

		foreach(AnimationState state in changeThemeAnimationStart.animation)
		{
			state.speed = 1.0f;
		}	
	}

	
	
	public void onCompleteChangeStageTheme()
	{
		GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
	}
	
	
	private void changeThemeAndStartNewStage(bool veryFirst, string backgroundId)
	{
		//changeBackground(backgroundId);
		GameManager.me.clearStage();		
		GameManager.me.currentScene = Scene.STATE.PLAY_BATTLE;
	}
*/
	
	
	public List<MapObject> currentMapObjects = new List<MapObject>();


	private Stack<MapObject> _mapObjectPool = new Stack<MapObject>();	
	
	public MapObject getMapObject()
	{
		MapObject mop;
		
		if(_mapObjectPool.Count > 0) mop = _mapObjectPool.Pop();
		else mop = (new MapObject((GameObject)Instantiate(GameManager.me.mapObject)));
		
		mop.nowUsing = true;
		
		return mop;
	}
	
	
	public void setMapObject(MapObject mobj)
	{
		_v.x = -99999.0f;
		mobj.setPositionAndTransform(_v);
		mobj.isEnabled = false;
		
		if(mobj.deleteCompleteCallback != null) mobj.deleteCompleteCallback(mobj);
		
		mobj.clearEffect();
		mobj.nowUsing = false;
		_mapObjectPool.Push(mobj);
		currentMapObjects.Remove(mobj);
	}		
	
	
	public void setMapObject(MapObject mobj, int index)
	{
		_v.x = -99999.0f;
		mobj.setPositionAndTransform(_v);
		mobj.isEnabled = false;
		
		if(mobj.deleteCompleteCallback != null) mobj.deleteCompleteCallback(mobj);
			
		mobj.clearEffect();
		mobj.nowUsing = false;
		_mapObjectPool.Push(mobj);
		currentMapObjects.RemoveAt(index);
	}	
	
	
	
	public bool visible
	{
		set
		{
			/*
			for(int i = currentMapObjects.Count - 1; i >= 0; --i)
			{
				currentMapObjects[i].gameObject.SetActiveRecursively(value);
				currentMapObjects[i].visibleEffect(value);
			}
			*/
			
			if(loadedMap != null)
			{
#if UNITY_EDITOR
				Debug.Log("===============  MAPMANAGER SETTING!!!! ========================");
#endif


				loadedMap.setVisible(value);

				if(value == true)
				{
					top = (int)loadedMap.top;
					bottom = (int)loadedMap.bottom;
					summonTop = top - 50;
					summonBottom = bottom + 50;
					mapHeight = top - bottom;
					mapSummonHeight = summonTop - summonBottom;
				}
			}
		}
	}
	
	
	public void clearStage(bool flag = true)
	{
		int len = currentMapObjects.Count;
		
		for(int i = len -1; i >= 0; --i)
		{
			setMapObject(currentMapObjects[i], i);
		}	


		GameManager.me.player.isFreeze.Set( false );

		if(inGameMap != null)
		{
			inGameMap.unloadAndDestroyMap();
		}

		if(loadedMap != null)
		{
			loadedMap.unloadAndDestroyMap();
		}

		while(loadedMapList.Count > 0)
		{
			loadedMapList.Pop().unloadAndDestroyMap();
		}

		foreach(KeyValuePair<int, MapData> kv in GameManager.info.mapData)
		{
			kv.Value.unloadAndDestroyMap();
		}

		if(GameManager.me.useAssetBundleMapFile)
		{
			foreach(KeyValuePair<string, GameObject> kv in GameDataManager.instance.mapResource)
			{
				if(kv.Value != null && (MapData.destroyExceptionResource == null || MapData.destroyExceptionResource != kv.Key))
				{
//					MapData.destroyComponents(kv.Value);
					GameObject.DestroyImmediate(kv.Value, true);
				}
			}

			if(MapData.destroyExceptionResource != null)
			{
				if(GameDataManager.instance.mapResource.ContainsKey(MapData.destroyExceptionResource))
				{
					GameObject g = GameDataManager.instance.mapResource[MapData.destroyExceptionResource];
					GameDataManager.instance.mapResource.Clear();
					GameDataManager.instance.mapResource.Add(MapData.destroyExceptionResource, g);
				}
				else
				{
					GameDataManager.instance.mapResource.Clear();
				}
			}
			else
			{
				GameDataManager.instance.mapResource.Clear();
			}
		}


		loadedMapList.Clear();
		inGameMap = null;
		loadedMap = null;

		GameDataManager.instance.unloadAllMapAssetBundle();

		if(targetPosMinimapPointer != null)
		{
			GameManager.me.characterManager.setMinimapPointer(targetPosMinimapPointer);
			targetPosMinimapPointer = null;
		}


		pvpGradeSlotManager.destroy();


		GameManager.me.flareLayer.isEnabled = false;

		//GameManager.soundManager.stopBG();
	}

	
	


}

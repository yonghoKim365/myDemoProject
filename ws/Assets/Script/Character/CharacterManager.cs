using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public class CharacterManager : MonoBehaviour, IManagerBase
{
	public Dictionary<string, AssetBundle> loadedBundle = new Dictionary<string, AssetBundle>(StringComparer.Ordinal);

	public Transform monsterResourcePool; // 원본 몬스터.

	public Transform preloadingMonsterPool; // 복사한 녀석들.
	
	private MonsterSorterByPosX _sortMonster = new MonsterSorterByPosX();
	private PlayerMonsterSorterByPosX _sortPlayerMonster = new PlayerMonsterSorterByPosX();
	private SorterByDistance _sortByDistance = new SorterByDistance();
	private SorterFloat _sortByFloat = new SorterFloat();
	
	public static SorterByDistanceFromHitPointCharacter sortByDistHitPointCharacter = new SorterByDistanceFromHitPointCharacter();
	public static SorterByDistanceFromHitPoint sortByDistHitPoint = new SorterByDistanceFromHitPoint();
	
	public Dictionary<string, Monster> monsterResource = new Dictionary<string, Monster>(StringComparer.Ordinal);

	public TargetingDecal[] targetingDecal;

	public ChargingGauge[] chargingGauge;

	public static IFloat lineOffset = 1.0f;

	void Awake()
	{
	}

	public void init()
	{
	}


	void OnDestroy()
	{
		if(_tempAniData != null) _tempAniData.Clear();
		_tempAniData = null;

		if(monsterResource != null) monsterResource.Clear();
		monsterResource = null;
		targetingDecal = null;
		chargingGauge = null;
		if(loadedBundle != null) loadedBundle.Clear();
		loadedBundle = null;
	}


	private Stack<string> _preloadingMonster = new Stack<string>();
	public void addPreloadingData(string str)
	{
		return;

//#if !UNITY_EDITOR
		isCompletePreloading = false;
		_preloadingMonster.Push(str);
//#endif
	}

	public void startPreLoading()
	{
		return;
//#if UNITY_EDITOR
//		isCompletePreloading = true;
//#else
		isCompletePreloading = false;
		StartCoroutine(startPreLoadingCoroutine());
//#endif
	}
	
	public static bool isCompletePreloading = true;

	IEnumerator startPreLoadingCoroutine()
	{
		string _tempId;
		while(_preloadingMonster.Count > 0)
		{
			_tempId = _preloadingMonster.Pop();

			while(_monsterPool.ContainsKey(_tempId) == false || _monsterPool[_tempId].Count < 5)
			{
				cleanMonster(createMonsterAsset(false,false,_tempId,false));
			}

			yield return null;
			System.GC.Collect();
		}

		yield return new WaitForSeconds(0.5f);

		yield return Resources.UnloadUnusedAssets();

		yield return null;

		System.GC.Collect();

		yield return null;

		isCompletePreloading = true;
	}


	//targetingDecal
	public List<TargetingDecal> activeMonsterTargetingDecal = new List<TargetingDecal>();
	private Stack<TargetingDecal> _monsterTargetingDecalPool = new Stack<TargetingDecal>();

	public TargetingDecal getMonsterHeroTargetingDecal()
	{
		TargetingDecal t;

		if(_monsterTargetingDecalPool.Count > 0)
		{
			t = _monsterTargetingDecalPool.Pop();
		}
		else
		{
			t = (TargetingDecal)UnityEngine.Object.Instantiate(GameManager.me.characterManager.targetingDecal[1]);
		}

		activeMonsterTargetingDecal.Add(t);

		return t;
	}

	public void setMonsterHeroTargetingDecal(TargetingDecal t)
	{
		t.isEnabled = false;

		if(activeMonsterTargetingDecal.Contains(t))
		{
			activeMonsterTargetingDecal.Remove(t);
		}

		_monsterTargetingDecalPool.Push(t);
	}




	private Dictionary<MonsterCategory.Category, Stack<CharacterAction>> _CharacterActionPool = new Dictionary<MonsterCategory.Category, Stack<CharacterAction>>();

	public CharacterAction getCharacterAction(MonsterCategory.Category category)
	{
		CharacterAction ma;

		if(_CharacterActionPool.ContainsKey(category) == false)
		{
			_CharacterActionPool[category] = new Stack<CharacterAction>();
		}
		
		if(_CharacterActionPool[category].Count > 0) ma = _CharacterActionPool[category].Pop();
		else
		{
			ma = createCharacterAction(category);
		}
		
		return ma;
	}
	
	
	public CharacterAction createCharacterAction(MonsterCategory.Category categoryName)
	{
		switch(categoryName)
		{
		case MonsterCategory.Category.UNIT:
			return new MonsterUnitAction();
		case MonsterCategory.Category.HEROMONSTER:
			return new MonsterHeroMonster();
		case MonsterCategory.Category.PLAYER:
			return new PlayerAction();
		case MonsterCategory.Category.NPC:
			return new CharacterAction();
		case MonsterCategory.Category.EFFECT:
			return new CharacterAction();
		case MonsterCategory.Category.OBJECT:
			return new CharacterAction();				
		case MonsterCategory.Category.PROTECT:
			return new ProtectNpcAction();				
		case MonsterCategory.Category.CHASER:
			return new MonsterChaser();				
		case MonsterCategory.Category.CHALLENGE_RUN_BOSS:
			return new ChallengeBossRun();
		case MonsterCategory.Category.CHALLENGE_SURVIVAL_BOSS:
			return new ChallengeBossSurvival();
		case MonsterCategory.Category.CHALLENGE_HUNT_BOSS:
			return new ChallengeBossHunt();
		case MonsterCategory.Category.B_TEST_BOSS:
			return new BTestBoss();
		case MonsterCategory.Category.MONSTER_DEAD_ZONE:
			return new MonsterDeadZone();
			break;
		}
		return new CharacterAction();
	}
	
	
	
	public void setCharacterAction(CharacterAction characterAction, MonsterCategory.Category category)
	{
		_CharacterActionPool[category].Push(characterAction);
		characterAction.clear();
	}


	//--- monster pool --- //
	private Stack<CharacterEffect> _characterEffectPool = new Stack<CharacterEffect>();
	
	public CharacterEffect getCharacterEffect(Monster mon)
	{
		CharacterEffect ce;
		if(_characterEffectPool.Count > 0)
		{
			ce = _characterEffectPool.Pop();
		}
		else ce = new CharacterEffect();

		ce.cha = mon;

		return ce;
	}

	public void setCharacterEffect(CharacterEffect ce)
	{
		ce.clear();
		ce.cha = null;
		_characterEffectPool.Push(ce);
	}

	private Stack<CharacterShadow> _characterShadows = new Stack<CharacterShadow>();
	
	public CharacterShadow getCharacterShadow()
	{
		if(_characterShadows.Count > 0) return _characterShadows.Pop();
		return Instantiate(shadow) as CharacterShadow;
	}
	
	public void setCharacterShadow(CharacterShadow shadow)
	{
		shadow.tf.parent = monsterResourcePool;
		shadow.gameObject.SetActive(false);
		_characterShadows.Push(shadow);
	}


	private Dictionary<string, Stack<Monster>> _monsterPool = new Dictionary<string, Stack<Monster>>(StringComparer.Ordinal);	
	
	private Stack<WaitMonster> _waitMonsterPool = new Stack<WaitMonster>();

	public CharacterShadow shadow;

	public List<WaitMonster> waitMonsters = new List<WaitMonster>();
	
	public WaitMonster getWaitMonster()
	{
		WaitMonster mon;
		
		if(_waitMonsterPool.Count > 0)
		{
			mon = _waitMonsterPool.Pop();	
		}
		else
		{
			mon = new WaitMonster();
		}
		
		waitMonsters.Add(mon);
		
		return mon;
	}
	
	
	
	public void removeAndSetWaitMonster(WaitMonster mon)
	{
		waitMonsters.Remove(mon);
		_waitMonsterPool.Push(mon);
	}
	
	
	
	public Monster getMonster(bool isPlayer, bool isPlayerMon = false, string monsterId = null, bool isInGameMon = true)
	{
#if UNITY_EDITOR

		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			Debug.Log("getMonster : " + monsterId);
		}
#endif
		Monster mon;

		if(_monsterPool.ContainsKey(monsterId) == false) _monsterPool.Add(monsterId, new Stack<Monster>());

		if(GameManager.info.monsterData[monsterId].category == MonsterCategory.Category.PLAYER || 
		   GameManager.info.monsterData[monsterId].category == MonsterCategory.Category.PET ||  
		   _monsterPool[monsterId].Count == 0)
		{
			mon = createMonsterAsset(isPlayer, isPlayerMon, monsterId, isInGameMon);
		}
		else
		{
			mon = _monsterPool[monsterId].Pop();
			mon.gameObject.SetActive(true);
			mon.monsterData = GameManager.info.monsterData[monsterId];
			mon.category =  mon.monsterData.category ;
		}

		mon.isVisible = true;
		mon.isVisibleForSkillCam = false;

		if(mon.aiUnitSlot != null) mon.aiUnitSlot.mon = null;
		mon.aiUnitSlot = null;
		mon.monsterUISlotIndex = -1;

		_v.x = 0.0f; _v.y = 0.0f; _v.z = 0.0f;

		mon.setParent(GameManager.me.mapManager.mapStage);

		mon.setPositionCtransform(_v);
		mon.cTransform.localPosition = _v;

		mon.characterEffect = getCharacterEffect(mon);

		mon.shadow = getCharacterShadow();

//		Log.log("get mondata category: " + mon.monsterData.category);

		if(isInGameMon)
		{
			if(isPlayerMon == false) monsters.Add(mon);
			else playerMonster.Add(mon);

			if(UIPopupSkillPreview.isOpen == false)
			{
				if(GameManager.me.stageManager.isIntro == false) mon.hpBar = getHpBar(mon.cTransform);			
				mon.miniMapPointer = getMinimapPointer(isPlayerMon, mon.cTransform);
			}
		}

		return mon;
	}



	public Player getNPCPlayerCharacter(string id, bool isInGame)
	{
		NPCData data = GameManager.info.npcData[id];
		GamePlayerData gpd = new GamePlayerData(data.resource);
		gpd = DebugManager.instance.setPlayerData(gpd,false, data.resource, data.head, data.body, data.weapon, data.vehicle);
		gpd.faceTexture = data.faceTexture;

//		UnityEngine.Debug.LogError( data.resource );
		
		Player p = (Player)GameManager.me.characterManager.getMonster(true,true,data.resource,false);
		p.init(gpd,true,false);

		data.setDataToCharacter(p);

		p.pet = (Pet)GameManager.me.characterManager.getMonster(true,false,gpd.partsVehicle.parts.resource.ToUpper(),false);
		p.pet.init(p);
		p.isEnabled = true;

		p.isPlayer = false;
		p.isHero = false;
		p.npcData = data;

		p.isPlayersPlayer = false;

		if(isInGame)
		{
			p.hpBar = getHpBar(p.cTransform);	
			p.isPlayerSide = true;
			p.initHpBar();
			p.hpBar.isEnabled = true;
			p.hpBar.visible = false;
			p.miniMapPointer = getMinimapPointer(true, p.cTransform);
			playerMonster.Add((Monster)p);
		}

		p.stat.monsterType = Monster.TYPE.NPC;

		return p;
	}




	public Player getPlayerCharacter(string resource, string head, string body, string weapon, string vehicle, string faceTexture, bool isPlayerSide, bool isInGame)
	{
		GamePlayerData gpd = new GamePlayerData(resource);

#if UNITY_EDITOR

		if(DebugManager.instance.isAutoCombat && BattleSimulator.nowSimulation == false)
		{
			gpd = DebugManager.instance.setPlayerData(gpd,false, resource, head, body, weapon, vehicle, null, null, DebugManager.instance.ai);
		}
		else
#endif
		{
			gpd = DebugManager.instance.setPlayerData(gpd,false, resource, head, body, weapon, vehicle);
		}

		gpd.faceTexture = faceTexture;
		
		Player p = (Player)GameManager.me.characterManager.getMonster(true,true,resource,false);
		p.init(gpd,true,false);
		p.pet = (Pet)GameManager.me.characterManager.getMonster(true,false,gpd.partsVehicle.parts.resource.ToUpper(),false);
		p.pet.init(p);
		p.isEnabled = true;
		
		p.isPlayer = false;
		p.isHero = true;

		p.isPlayersPlayer = false;

		if(isInGame)
		{
			p.hpBar = getHpBar(p.cTransform);	
			p.isPlayerSide = isPlayerSide;
			p.initHpBar();
			p.hpBar.isEnabled = true;
			p.hpBar.visible = false;
			p.miniMapPointer = getMinimapPointer(true, p.cTransform);

			if(isPlayerSide)
			{
				playerMonster.Add((Monster)p);
			}
			else
			{
				monsters.Add((Monster)p);
			}
		}
		
		p.stat.monsterType = Monster.TYPE.HERO;
		
		return p;
	}








	public Monster createMonsterAsset(bool isPlayer, bool isPlayerMon = false, string monsterId = null, bool isInGameMon = true)
	{
		if(_monsterPool.ContainsKey(monsterId) == false) _monsterPool.Add(monsterId, new Stack<Monster>());
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Debug.Log("monsterId : " + monsterId);
//			Debug.Log(GameManager.info.monsterData[monsterId].resource);
		}
#endif
		Monster mon = (Monster)Instantiate(monsterResource[GameManager.info.monsterData[monsterId].resource]);

		mon.gameObject.SetActive(true);

		mon.tf = mon.transform;
		mon.monsterData = GameManager.info.monsterData[monsterId];
		mon.category = mon.monsterData.category;
		#if UNITY_EDITOR			
		GameObject container = new GameObject();
		container.name = mon.monsterData.id;
		#else
		GameObject container = new GameObject("");
		#endif				

		mon.ani = mon.animation;
		mon.container = container;
		mon.cTransform = container.transform;

		mon.setParent( GameManager.me.mapManager.mapStage );
		mon.tf.parent = mon.cTransform;

		if(GameManager.info.modelData.ContainsKey(mon.resourceId))
		{
			mon.useRimShader = GameManager.info.modelData[mon.resourceId].useRimShader;
		}

		mon.rendererInit();

		if(mon.monsterData.canChangeShader == false)
		{
			mon.setOriginalShader();
		}


		if(isPlayer == false)
		{
			isPlayer = GameManager.info.monsterData[monsterId].category == MonsterCategory.Category.PLAYER;
		}

		if(GameManager.info.monsterData[monsterId].hasParticle)
		{
			mon.particle = mon.GetComponentInChildren<ParticleSystem>();
			mon.particle.Play();
		}

		if(isPlayer)
		{
			if(mon.heads == null) mon.heads = new Dictionary<string, GameObject>(StringComparer.Ordinal);
			if(mon.bodies == null) mon.bodies = new Dictionary<string, GameObject>(StringComparer.Ordinal);
			if(mon.weapons == null) mon.weapons = new Dictionary<string, GameObject>(StringComparer.Ordinal);

			Transform[] parts = mon.GetComponentsInChildren<Transform>();

			int partsLen = parts.Length;

			for(int pi = 0; pi < partsLen; ++pi)
			{
				if(parts[pi].name.Contains(Monster.head))
				{
					mon.heads[parts[pi].name] = parts[pi].gameObject;
				}
				else if(parts[pi].name.Contains(Monster.body))
				{
					mon.bodies[parts[pi].name] = parts[pi].gameObject;
				}
				else if(parts[pi].name.Contains(Monster.weapon))
				{
					mon.weapons[parts[pi].name] = parts[pi].gameObject;
				}
			}
		}


		if(mon.monsterData.category == MonsterCategory.Category.PET)
		{
			Transform[] rt = mon.GetComponentsInChildren<Transform>();
			foreach(Transform r in rt)
			{
				if(r.name.Equals(Monster.ride_01))
				{
					((Pet)mon).tfRidePoint = r;
					break;
				}
			}
		}


		if(mon.monsterData.hasFaceAni)
		{
			mon.faceAniEye = null;
			mon.faceAniEye2 = null;
			mon.faceAniMouth = null;
			mon.faceAniMouth2 = null;

			for(int i = 0; i < mon.smrNumber; ++i)
			{
				if(mon.smrs[i].gameObject.name.Contains(ModelData.OUTLINE_NAME) == false)
				{
					if(mon.smrs[i].gameObject.name.Contains(ModelData.eye))
					{
						TextureSpriteAnimation tsa = mon.smrs[i].gameObject.GetComponent<TextureSpriteAnimation>();
						if(tsa == null) tsa = mon.smrs[i].gameObject.AddComponent<TextureSpriteAnimation>();
						if(mon.faceAniEye == null) mon.faceAniEye = tsa;
						else mon.faceAniEye2 = tsa;
					}
					else if(mon.smrs[i].gameObject.name.Contains(ModelData.mouth))
					{
						TextureSpriteAnimation tsa = mon.smrs[i].gameObject.GetComponent<TextureSpriteAnimation>();
						if(tsa == null) tsa = mon.smrs[i].gameObject.AddComponent<TextureSpriteAnimation>();
						if(mon.faceAniMouth == null) mon.faceAniMouth = tsa;
						else mon.faceAniMouth2 = tsa;
					}
				}
			}
		}


		if(mon.monsterData.deleteParts != null)
		{
			int delPartsLen = mon.monsterData.deleteParts.Length;

//			SkinnedMeshRenderer[] smrs = mon.GetComponentsInChildren<SkinnedMeshRenderer>();
//			int sl = smrs.Length;
			
			for(int i = 0; i < mon.smrNumber; ++i)
			{
				for(int j = 0; j < delPartsLen; ++j)
				{
					if(mon.smrs[i].gameObject.name.Contains(mon.monsterData.deleteParts[j]))
					{
						mon.smrs[i].gameObject.SetActive(false);
					}
				}
			}

			for(int i = 0; i < mon.mrNumber; ++i)
			{
				for(int j = 0; j < delPartsLen; ++j)
				{
					if(mon.mrs[i].gameObject.name.Contains(mon.monsterData.deleteParts[j]))
					{
						mon.mrs[i].gameObject.SetActive(false);
					}
				}
			}
		}



		if(mon.monsterData.partsUV != null )
		{
			int uvLen = mon.monsterData.partsUV.Length;
			
			for(int i = 0; i < mon.smrNumber; ++i)
			{
				for(int j = 0; j < uvLen; ++j)
				{
					if(mon.smrs[i].gameObject.name.Contains(mon.monsterData.partsUV[j].targetName))
					{
						TextureAnimationLoopType t = mon.smrs[i].gameObject.GetComponent<TextureAnimationLoopType>();
						if(t == null) t = mon.smrs[i].gameObject.AddComponent<TextureAnimationLoopType>();
						mon.monsterData.partsUV[j].Apply(t);
					}
				}
			}
			
			for(int i = 0; i < mon.mrNumber; ++i)
			{
				for(int j = 0; j < uvLen; ++j)
				{
					if(mon.mrs[i].gameObject.name.Contains(mon.monsterData.partsUV[j].targetName))
					{
						TextureAnimationLoopType t = mon.mrs[i].gameObject.GetComponent<TextureAnimationLoopType>();
						if(t == null) t = mon.mrs[i].gameObject.AddComponent<TextureAnimationLoopType>();
						mon.monsterData.partsUV[j].Apply(t);
					}
				}
			}
		}




		setCharacterShootingPointAndEffectContainer(mon);

		mon.ani.cullingType = AnimationCullingType.BasedOnClipBounds;

		return mon;
	}


	int attackerLength = 0;

	public void checkDeadMonsterEvent(Monster monster)
	{
		if(GameManager.me.isPlaying)
		{
			// -- 적 캐릭터가 사라진 순간 -- //
			switch(monster.category)
			{
			case MonsterCategory.Category.HEROMONSTER:
				--GameManager.me.mapManager.bossNum;
				break;
			case MonsterCategory.Category.UNIT:
				if(monster.isPlayerSide == false)
				{
					if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLEMALL)
					{
						--GameManager.me.mapManager.monUnitNum;
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLCOUNT)
					{
						if(GameManager.me.mapManager.leftKilledMonsterNum.ContainsKey(monster.unitData.id))
						{
							--GameManager.me.mapManager.leftKilledMonsterNum[monster.unitData.id];
							
							int num = 0;
							foreach(KeyValuePair<string,Xint> kv in GameManager.me.mapManager.leftKilledMonsterNum)
							{
								num += kv.Value;
							}
							
							GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = num + "M";		
							
						}
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLCOUNT2)
					{
						--GameManager.me.mapManager.leftKilledMonster;
						GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = GameManager.me.mapManager.leftKilledMonster + "M";
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.C_HUNT)
					{
						int killUnitNum = GameManager.me.mapManager.killedUnitCount.Get() + 1;
						GameManager.me.mapManager.killedUnitCount.Set(killUnitNum);
						GameManager.me.uiManager.uiPlay.challangeModeInfo.update( killUnitNum );
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.B_TEST)
					{
						int killUnitNum = GameManager.me.mapManager.killedUnitCount.Get() + 1;
						GameManager.me.mapManager.killedUnitCount.Set(killUnitNum);
						GameManager.me.uiManager.uiPlay.challangeModeInfo.update( killUnitNum );
					}

					if(HellModeManager.instance.isOpen)
					{
						HellModeManager.instance.setKillUnit(monster.unitData);
					}
				}
				break;
			case MonsterCategory.Category.OBJECT:
				if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.DESTROY)
				{
					if(monster.isPlayerSide == false && monster.specialType != Monster.SpecialType.SKIP_DESTORY_CHECK)
					{
						--GameManager.me.mapManager.leftDestroyObjectNum[monster.npcData.id];	
						int num = 0;
						foreach(KeyValuePair<string,Xint> kv in GameManager.me.mapManager.leftDestroyObjectNum)
						{
							num += kv.Value;
						}
						
						GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = num + "";
					}
				}
				break;
			}
			
			GameManager.me.stageManager.clearChecker(ClearChecker.CHECK_AFTER_DELETE);
		}

	}

	Quaternion _q = new Quaternion();
	IVector3 _cleanPosition = IVector3.zero;
	public void cleanMonster(Monster monster, bool isDestroy = false)
	{
		if(monster == null) return;

		if(monster.animation != null && monster.animation.GetClip(Monster.NORMAL) != null) monster.animation.Play(Monster.NORMAL);

		if(monster.aiUnitSlot != null) monster.aiUnitSlot.mon = null;
		monster.aiUnitSlot = null;
		monster.monsterUISlotIndex = -1;

		monster.stat.uniqueId = -1;
		monster.prevUniqueId = -1;

		monster.cTransform = monster.container.transform;
		monster.tf = monster.transform;

		monster.setParent( preloadingMonsterPool );

		monster.cleanPosition();
		monster.cTransform.localPosition = _cleanPosition;

		monster.receiveDamageMonstersByMe.Clear();
		monster.clearEffect();
		monster.setVisible(true);
		monster.isVisibleForSkillCam = false;
		monster.isEnabled = false;

		_v = monster.container.transform.localScale;
		_v.x = 1; _v.y = 1; _v.z = 1;
		monster.container.transform.localScale = _v;

		monster.cutSceneInit();

		_v.x = 0; _v.y = 0; _v.z = 0;
		_q.eulerAngles = _v;
		monster.cTransform.rotation = _q;
		monster.tf.rotation = _q;

		monster.nowAniId = string.Empty;
		monster.setState(Monster.NORMAL);

		//monster.removeTween();

		if(monster.deleteMotionEffect != null && monster.deleteMotionEffect.effectChracter != null) 
		{
			cleanMonster(monster.deleteMotionEffect.effectChracter, isDestroy);
		}

		if(monster.toolTip != null)
		{
			setTooltip(monster.toolTip);
			monster.toolTip = null;
		}
		
		if(monster.hpBar != null)
		{
			setHpBar(monster.hpBar);
			monster.hpBar = null;		
		}
		
		
		if(monster.miniMapPointer != null)
		{
			setMinimapPointer(monster.miniMapPointer);
			monster.miniMapPointer = null;		
		}
		
		attackerLength = monster.attackers.Count;
		
		if(attackerLength > 0) // 나를 때리던 놈들이 있으면 걔들한테서 내가 죽었다고 그만 때리라고 알려준다.
		{
			for(int i = 0; i < attackerLength; ++i)
			{
				if(monster.attackers[i] != null)
				{
					monster.attackers[i].target = null;
					monster.attackers[i].targetAngle = 0;
				}
			}
			
			monster.attackers.Clear();
		}
		
		monster.setTarget(null);
		monster.setSkillTarget(null);

		if(monster.action != null) setCharacterAction(monster.action, monster.category);
		monster.action = null;

		if(monster.characterEffect != null)
		{
			setCharacterEffect(monster.characterEffect);
			monster.characterEffect = null;
		}

		if(monster.shadow != null)
		{
			setCharacterShadow(monster.shadow);
			monster.shadow = null;
		}

		monster.setParent( preloadingMonsterPool );

		if(monster.pet != null)
		{
			cleanMonster(monster.pet, true);
			destroyMonster(monster);
			monster.pet = null;
			monster = null;
		}
		else if(isDestroy)
		{
			destroyMonster(monster);
			monster = null;
		}
		else
		{
			if(_monsterPool.ContainsKey(monster.monsterData.id) == false) _monsterPool.Add(monster.monsterData.id, new Stack<Monster>());
			_monsterPool[monster.monsterData.id].Push(monster);
		}
		//CharacterUtil.removeRareLineMaterial(monster);
	}
	// --------------------------------------------------------------------------------------------

	void destroyMonster(Monster monster, bool isOriginal = false)
	{
//		Debug.LogError("destroy: " + monster.cTransform);
		GameObject container = monster.container;
		monster.destroy(isOriginal);
		if(monster != null && monster.gameObject != null) GameObject.DestroyImmediate(monster.gameObject, true);
		if(container != null) GameObject.DestroyImmediate(container, true);
	}


	private Dictionary<string, AniData> _tempAniData = null;


	public void setCharacterShootingPointAndEffectContainer(Monster cha)
	{
		if(cha != null && cha.resourceId != null)
		{
			Transform[] trs = cha.transform.GetComponentsInChildren<Transform>();

			if(GameManager.info.aniData.TryGetValue(cha.resourceId, out _tempAniData))
			{
				foreach(KeyValuePair<string, AniData> kv in _tempAniData)
				{
					if(( kv.Key.Contains(Monster.SHOOT_HEADER) || kv.Key.Contains(Monster.SKILL_HEAD) ))
					{
						if(cha.shootingPositions == null) cha.shootingPositions = new Dictionary<string, int[][]>(StringComparer.Ordinal);

						cha.shootingPositions[kv.Key] = kv.Value.shootingPositions;

						if(kv.Value.shootingPoint != null)
						{
							int len = kv.Value.shootingPoint.Length;
							if(cha.shootingTransforms == null) cha.shootingTransforms = new Dictionary<string, Transform[]>(StringComparer.Ordinal);
							cha.shootingTransforms[kv.Key] = new Transform[len];

							for(int i = 0; i < len; ++i)
							{
								foreach(Transform ct in trs)
								{
									if(ct.name.Equals(kv.Value.shootingPoint[i]))
									{
										cha.shootingTransforms[kv.Key][i]  = ct;
										break;
									}
								}							
							}
						}



						if(cha.effectParents == null) cha.effectParents = new Dictionary<string, Transform>(StringComparer.Ordinal);
						
						foreach(Transform ct in trs)
						{
							if(kv.Value.effect != null)
							{
								foreach(AniDataEffect efd in kv.Value.effect)
								{
									if(string.IsNullOrEmpty(efd.parent) == false)
									{
										if(ct.name.Equals(efd.parent))
										{
											cha.effectParents[efd.parent] = ct;
											break;
										}
									}
								}
							}
						}	
					}
				}
			}




			if(cha.monsterData != null)
			{
				if(cha.monsterData.effectData != null)
				{
					int targetNum = 0;

					foreach(AttachedEffectData efd in cha.monsterData.effectData)
					{
						if(string.IsNullOrEmpty(efd.parentName) == false)
						{
							++targetNum;
						}
					}

					if(cha.effectParents == null) cha.effectParents = new Dictionary<string, Transform>(StringComparer.Ordinal);

					foreach(Transform ct in trs)
					{
						foreach(AttachedEffectData efd in cha.monsterData.effectData)
						{
							if(string.IsNullOrEmpty(efd.parentName) == false)
							{
								if(ct.name.Equals(efd.parentName))
								{
									cha.effectParents[efd.parentName] = ct;
									break;
								}
							}
						}
					}							
				}
			}
			
			
			

		}		
	}	
	


	
	

	
	
	
	
	// --------------------------------------------------------------------------------------------
	
	public delegate void LoadBaseCharacterCompleteCallback(GameObject go, Monster cha);
	
	public void loadBaseCharacter(Monster.ResourceType type, string name, LoadBaseCharacterCompleteCallback callback, Transform root )
	{
		StartCoroutine(loadCharacterAsset(type, name, root, callback));
	}
	
	
	IEnumerator loadCharacterAsset(Monster.ResourceType type, string loadAssetsNames, Transform root, LoadBaseCharacterCompleteCallback callback )
	{
		string[] loadAssets = loadAssetsNames.Split(',');

		string path = string.Empty;

#if UNITY_EDITOR
		if(ResourceManager.instance.useAssetDownload == false)
		{
			path = AssetBundleManager.getResourceName( loadAssets[0] +"_base" , AssetBundleManager.ResourceType.Model);
		}
		else
#endif
		{
			path = AssetBundleManager.getResourceName( loadAssets[0] , AssetBundleManager.ResourceType.Model);//+"_base" );
		}


#if UNITY_EDITOR && UNITY_IPHONE
		path = ResourceManager.getLocalFilePath("I/" + path, AssetBundleManager.bundleExtension);
#elif UNITY_EDITOR && UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("A/" + path, AssetBundleManager.bundleExtension);
#elif UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath("I/" + path, AssetBundleManager.bundleExtension);
#else
		path = ResourceManager.getLocalFilePath("A/" + path, AssetBundleManager.bundleExtension);
#endif
		
		using(WWW asset = new WWW(path))
		{
//	#if UNITY_EDITOR		
//			Debug.Log("path: " + path);
//	#endif	
			bool success = true;
			
			yield return asset;
				
		   if(asset == null || asset.error != null || asset.isDone == false)
			{
				if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
				{
					Debug.LogError("err: " + asset.error.ToString());
				}

				success = false;
			}
		    else if(asset != null && asset.assetBundle != null) success = true;
			else success = false;
			
			if(success)
			{
				GameObject gob = (GameObject)Instantiate(asset.assetBundle.mainAsset);

//				GameObject gob = (GameObject)(asset.assetBundle.mainAsset as GameObject) ;

//				Debug.Log("gob : " + (gob == null));

				if(gob.animation == null)
				{
					gob.AddComponent<Animation>();
				}
				gob.animation.Stop();
				gob.animation.playAutomatically = false;			
				
				if(type == Monster.ResourceType.PLAYER)
				{
					Player p = gob.AddComponent<Player>();
					p.resourceId = loadAssets[0];
					addResource(loadAssets[0],p);
					gob.transform.parent = monsterResourcePool;

					p.rendererInit(true);

					// 파츠 혹은 아웃라인을 쪼개서 불렀다면 아래것을 쓴다.
					//if(loadAssets.Length > 1)  p.partsLoader.loadModel(loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1), callback);	
					//else if(callback != null) callback(gob, p);

					// 통짜모델이기 때문에 이걸 쓴다.
					p.partsLoader.initSingleModel(p, gob, callback); 
				}
				else if(type == Monster.ResourceType.MONSTER)
				{
//					Debug.Log("loadAssets[0]: " + loadAssets[0]);
					Monster m = gob.AddComponent<Monster>();
					m.resourceId = loadAssets[0];
					addResource(loadAssets[0],m);
					gob.transform.parent = monsterResourcePool;

					m.rendererInit(true);

					//if(loadAssets.Length > 1)  m.partsLoader.loadModel(loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1), callback);	
					//else if(callback != null) callback(gob, m);
					m.partsLoader.initSingleModel(m, gob, callback);

				}
				else if(type == Monster.ResourceType.EFFECT)
				{
					Monster m = gob.AddComponent<Monster>();
					m.resourceId = loadAssets[0];
					addResource(loadAssets[0],m);
					gob.transform.parent = monsterResourcePool;
					
					//if(loadAssets.Length > 1)  m.partsLoader.loadModel(loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1), callback);	
					//else if(callback != null) callback(gob, m);

					m.rendererInit(true);

					m.partsLoader.initSingleModel(m, gob, callback);
				}			
				else if(type == Monster.ResourceType.PET)
				{
					Pet p = gob.AddComponent<Pet>();
					p.resourceId = loadAssets[0];
					addResource(loadAssets[0],p);
					gob.transform.parent = monsterResourcePool;
					
					//if(loadAssets.Length > 1)  p.partsLoader.loadModel(loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1), callback);	
					//else if(callback != null) callback(gob, p);

					p.rendererInit(true);

					p.partsLoader.initSingleModel(p, gob, callback);
				}

//				saveAssetBundle(loadAssetsNames, asset.assetBundle);
				asset.assetBundle.Unload(false);
			}

			asset.Dispose();
		}

//		asset.Dispose();
//		asset = null;	
		
	}	


	public void saveAssetBundle(string assetName, AssetBundle asset)
	{
		if(loadedBundle.ContainsKey(assetName))
		{
			loadedBundle[assetName] = asset;
		}
		else
		{
			loadedBundle.Add( assetName , asset );
		}
	}


	
	void addResource(string name, Monster resource)
	{
		name = name.ToLower();

#if UNITY_EDITOR
//		Debug.Log("== Add resrouce : " + name);
#endif
		if(resource.animation != null) resource.animation.Play();

		if(monsterResource.ContainsKey(name))
		{
#if UNITY_EDITOR
			Debug.LogError("====== ALREADY HAS RESOURCE : CHECK THIS!!! =====");
#endif
		}
		else
		{
			monsterResource.Add(name, resource);
		}


	}


	public void clearResource(bool includeOriginal = false)
	{
		foreach(KeyValuePair<string, Stack<Monster>> kv in _monsterPool)
		{
			if(kv.Value.Count > 0)
			{
				GameObject.DestroyImmediate(kv.Value.Pop().container.gameObject,true);
			}
		}


		if(includeOriginal)
		{
			foreach(KeyValuePair<string, Monster> kv in monsterResource)
			{
				GameObject.DestroyImmediate(kv.Value.gameObject,true);
			}
			monsterResource.Clear();

			foreach(KeyValuePair<string, AssetBundle> kv in loadedBundle)
			{
				if(kv.Value != null)
				{
					kv.Value.Unload(true);
					Destroy(kv.Value);
				}
			}

			loadedBundle.Clear();
		}
	}


	public string nowWorldMapMonsterHero = string.Empty;

	public void clearUnusedResource(bool includeLoadingSampleMonster = false, List<string> exceiptionList = null)
	{
		List<string> deleteAssets = new List<string>();

		foreach(KeyValuePair<string, Stack<Monster>> kv in _monsterPool)
		{
			while(kv.Value.Count > 0)
			{
				if(deleteAssets.Contains(kv.Key) == false)
				{
					deleteAssets.Add(kv.Key);
				}

				Monster mon = kv.Value.Pop();
				destroyMonster(mon);
			}
		}
		
		for(int i = deleteAssets.Count - 1; i >= 0; --i)
		{
			_monsterPool.Remove(deleteAssets[i]);
		}
		
		deleteAssets.Clear();


		for(int i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
		{
			if(GameDataManager.instance.unitSlots[i].isOpen == false) continue;

			MonsterData umd = GameManager.info.monsterData[GameDataManager.instance.unitSlots[i].unitData.resource];

			if(umd.deleteMotionType == ChracterDeleteMotionType.EFFECT)
			{
				EffectData deleteMotionEffect = GameManager.info.effectData[umd.deleteMotionValue];
				
				if(deleteMotionEffect.type == EffectData.ResourceType.CHARACTER)
				{
					if(exceiptionList == null) exceiptionList = new List<string>();
					exceiptionList.Add(GameManager.info.monsterData[deleteMotionEffect.resource].resource.ToLower());
				}
			}			
		}

		// should fix!!!
		foreach(KeyValuePair<string, PlayerUnitSlot[]> kv2 in GameDataManager.instance.playerUnitSlots)
		{
			for(int i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
			{
				if(kv2.Value[i].isOpen == false) continue;
				
				MonsterData umd = GameManager.info.monsterData[kv2.Value[i].unitData.resource];
				
				if(umd.deleteMotionType == ChracterDeleteMotionType.EFFECT)
				{
					EffectData deleteMotionEffect = GameManager.info.effectData[umd.deleteMotionValue];
					
					if(deleteMotionEffect.type == EffectData.ResourceType.CHARACTER)
					{
						if(exceiptionList == null) exceiptionList = new List<string>();
						exceiptionList.Add(GameManager.info.monsterData[deleteMotionEffect.resource].resource.ToLower());
					}
				}	
			}
		}






		if(exceiptionList == null) exceiptionList = new List<string>();


		if(string.IsNullOrEmpty(nowWorldMapMonsterHero) == false && GameManager.info.monsterData.ContainsKey(nowWorldMapMonsterHero) && exceiptionList.Contains(GameManager.info.monsterData[nowWorldMapMonsterHero].resource ) == false)
		{
			exceiptionList.Add( GameManager.info.monsterData[nowWorldMapMonsterHero].resource );
		}


		foreach(KeyValuePair<string, Monster> kv in monsterResource)
		{
			if( 
			   kv.Key.StartsWith("pc_") == false && 
			   kv.Key.StartsWith("pet_") == false &&

			   kv.Key.StartsWith("mob_skeletonwarrior01") == false &&
			   kv.Key.StartsWith("mob_skeletonsaber01") == false   
			   )
			{

				bool canDeleteThis = true;

				if(includeLoadingSampleMonster == false)
				{
					if(GameManager.me.uiManager.uiLoading.sample != null)
					{
						if(GameManager.me.uiManager.uiLoading.sample.resourceId != null && kv.Key == GameManager.me.uiManager.uiLoading.sample.resourceId)
						{
							canDeleteThis = false;
							break;
						}
					}
				}

				if(exceiptionList != null)
				{
					if(exceiptionList.Contains(kv.Key))
					{
						canDeleteThis = false;
					}
				}


				for(int i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
				{
					if(GameDataManager.instance.unitSlots[i].isOpen)
					{
						if(kv.Key == GameManager.info.monsterData[GameDataManager.instance.unitSlots[i].unitData.resource].resource)
						{
							canDeleteThis = false;
							break;
						}
					}
				}


				// should fix!!!
				foreach(KeyValuePair<string, PlayerUnitSlot[]> kv2 in GameDataManager.instance.playerUnitSlots)
				{
					for(int i = 0; i < Player.SUMMON_SLOT_NUM; ++i)
					{
						if(kv2.Value[i].isOpen)
						{
							if(kv.Key == GameManager.info.monsterData[kv2.Value[i].unitData.resource].resource)
							{
								canDeleteThis = false;
								break;
							}
						}
					}
				}


				if(canDeleteThis == false) continue;

				if(deleteAssets.Contains(kv.Key) == false)
				{
					deleteAssets.Add(kv.Key);
				}

				destroyMonster(kv.Value, true);

				if(loadedBundle.ContainsKey(kv.Key) && loadedBundle[kv.Key] != null)
				{
					loadedBundle[kv.Key].Unload(true);
					Destroy(loadedBundle[kv.Key]);
					loadedBundle[kv.Key] = null;
					loadedBundle.Remove(kv.Key);
				}
			}
		}

		for(int i = deleteAssets.Count - 1; i >= 0; --i)
		{
			monsterResource.Remove(deleteAssets[i]);
		}

		deleteAssets.Clear();
	}



	// -------------------------------------------------------------------------------------------
	
	public List<CharacterHpBar> hpBars = new List<CharacterHpBar>();
	public CharacterHpBar hpBarAsset;
	public Transform inGameGUIContinaer;
	private Stack<CharacterHpBar> _hpBarPool = new Stack<CharacterHpBar>();	
	
	public CharacterHpBar getHpBar(Transform pointer,float yPos = 100.0f)
	{
		CharacterHpBar hpBar;
		
		if(_hpBarPool.Count > 0) hpBar = _hpBarPool.Pop();
		else
		{
			hpBar = ((CharacterHpBar)Instantiate(hpBarAsset));

		}
		 
		hpBar.transform.parent = inGameGUIHpBarContainer;//inGameGUIContinaer;

		hpBar.yScreenLimit = (int) ( 490.0f / GameManager.camRatioY );

		hpBar.init(pointer,0,yPos);
		hpBars.Add(hpBar);
		return hpBar;
	}
	
	
	
	public void setHpBar(CharacterHpBar hpBar)
	{
		if(hpBar == null) return;
		hpBars.Remove(hpBar);
		hpBar.isEnabled = false;
		_hpBarPool.Push(hpBar);
	}	
	
	
	
	
	// -------------------------------------------------------------------------------------------
	
	public List<CharacterMinimapPointer> miniMapPointers = new List<CharacterMinimapPointer>();
	public CharacterMinimapPointer miniMapPointerAsset;
	private Stack<CharacterMinimapPointer> _miniMapPointerPool = new Stack<CharacterMinimapPointer>();	
	
	public CharacterMinimapPointer getMinimapPointer(bool isPlayer, Transform character)
	{
		CharacterMinimapPointer mp;
		
		if(_miniMapPointerPool.Count > 0) mp = _miniMapPointerPool.Pop();
		else
		{
			mp = ((CharacterMinimapPointer)Instantiate(miniMapPointerAsset));
		}
		 
		mp.cachedTransform = mp.transform;
		mp.transform.parent = GameManager.me.uiManager.uiPlay.tfMiniMapContainer;//inGameGUIContinaer;
		//mp.visible = false;
		miniMapPointers.Add(mp);
		return mp;
	}
	
	public void setMinimapPointer(CharacterMinimapPointer mp)
	{
		miniMapPointers.Remove(mp);
		mp.isEnabled = false;
		_miniMapPointerPool.Push(mp);
	}	
	
	
	
	// -------------------------------------------------------------------------------------------
	
	public List<CharacterTooltip> toolTips = new List<CharacterTooltip>();
	public CharacterTooltip toolTipAsset;
	public Transform inGameGUITooltipContainer;
	public Transform inGameGUIHpBarContainer;
	private Stack<CharacterTooltip> _toolTipPool = new Stack<CharacterTooltip>();	
	
	public CharacterTooltip getTooltip(Transform pointer)
	{
		CharacterTooltip toolTip;
		
		if(_toolTipPool.Count > 0) toolTip = _toolTipPool.Pop();
		else
		{
			toolTip = ((CharacterTooltip)Instantiate(toolTipAsset));
		}
		 
		toolTip.transform.parent = inGameGUITooltipContainer;
		
		toolTip.init(pointer);
		toolTips.Add(toolTip);
		return toolTip;
	}
	
	public void setTooltip(CharacterTooltip toolTip)
	{
		toolTips.Remove(toolTip);
		toolTip.isEnabled = false;
		_toolTipPool.Push(toolTip);
	}		
	
	
	
	// -------------------------------------------------------------------------------------------

	public void clearStage(bool includePlayerSide = true)
	{
//		Debug.Log("clearStage!");
//		Log.log("clearStage");

#if UNITY_EDITOR
		Monster.randomNum = 0;
#endif
		Monster.uniqueIndex = 0;

		int len = monsters.Count;
		int i = 0;
		
		for(i = len -1; i >= 0; --i)
		{
#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false && DebugManager.instance.useDebug == false)
			{
#endif
				if(monsters[i].isPlayer)
				{
					if(GameManager.me.pvpPlayer != null && ((Player)monsters[i]) == GameManager.me.pvpPlayer)
					{
						try
						{
							GameManager.me.pvpPlayer.clearPlayerEffect();

							GameManager.me.battleManager.removePlayerFromSlots(GameManager.me.pvpPlayer, false, false);
						}
						catch(Exception e)
						{
							
						}
					}
					else
					{
						GameManager.me.battleManager.removePlayerFromSlots((Player)monsters[i], false);
					}
				}
#if UNITY_EDITOR
			}
#endif
			cleanMonster(monsters[i],true);
		}	


		monsters.Clear();

		if(includePlayerSide)
		{
			len = playerMonster.Count;
			
			for(i = len -1; i >= 0; --i)
			{
				if(playerMonster[i].isPlayersPlayer == false)
				{
					cleanMonster(playerMonster[i],true);
				}
				else
				{
					if( ((Player)playerMonster[i]) == GameManager.me.player)
					{
						GameManager.me.battleManager.removePlayerFromSlots(((Player)playerMonster[i]), true, false);
					}
					else
					{
						GameManager.me.battleManager.removePlayerFromSlots(((Player)playerMonster[i]), true, true);
					}
				}

			}	
			playerMonster.Clear();
		}


		len = deadEffectPlayingMonsters.Count;
		
		for(i = len - 1; i >= 0; --i)
		{
			deadEffectPlayingMonsters[i].isDeleteObject = false;
			cleanMonster(deadEffectPlayingMonsters[i],true);
		}

		deadEffectPlayingMonsters.Clear();

		if(GameManager.me.stageManager.chaser != null)
		{
			cleanMonster(GameManager.me.stageManager.chaser, true);
			GameManager.me.stageManager.chaser = null;
		}
		
		len = decoMonsters.Count;
		
		for(i = len-1; i >=0; --i)
		{
			cleanMonster(decoMonsters[i], true);
		}
		
		decoMonsters.Clear();
		
		len = waitMonsters.Count;

		for(i = len-1; i >=0; --i)
		{
			removeAndSetWaitMonster(waitMonsters[i]);
		}
		waitMonsters.Clear();
		
		changeSideMonsters.Clear();


		if(GameManager.me.stageManager.protectNPC != null)
		{
			cleanMonster(GameManager.me.stageManager.protectNPC, true);
			GameManager.me.stageManager.protectNPC = null;
		}

		if(GameManager.me.stageManager.chaser != null)
		{
			cleanMonster(GameManager.me.stageManager.chaser, true);
			GameManager.me.stageManager.chaser = null;
		}

		GameManager.me.stageManager.playerDestroyObjectMonster = null;

		GameManager.me.stageManager.heroMonster = null;

		targetingDecal[0].isEnabled = false;
		targetingDecal[1].isEnabled = false;

		attackerLength = 0;

		//clearDeadPets(false);

		monsterLeftLine.Set( 0 );
		monsterRightLine.Set( 0 );
		
		playerMonsterLeftLine.Set( 0 );
		playerMonsterRightLine.Set( 0 );
		
		totalMonsterUnitHp.Set( 0 );
		totalPlayerUnitHp.Set( 0 );
		
		playerUnitInMonsterTargetZone.Set ( 0 );
		monterUnitInPlayerTargetZone.Set ( 0 );
		
		
		playerUnitHPInMonsterTargetZone.Set( 0 );
		monterUnitHPInPlayerTargetZone.Set( 0 );
		
		
		totalAliveMonsterUnitNum.Set( 0 );
		totalAlivePlayerMonsterUnitNum.Set( 0 );
		
		totalAliveMonsterNum = 0;
		
		tempModelCount = 0;
		
		targetZoneDistance.Set( 50.0f );
		targetZoneMonsterLine.Set( 0 );
		targetZonePlayerLine.Set( 0 );
		longestWalkedTargetZonePlayerLine.Set( -99999.0f );
		
		_playerCharacterInTargetZone.Clear();
		_monsterCharacterInTargetZone.Clear();

		aliveMonUnit.Clear();
		alivePlayerUnit.Clear();
		alivePlayerUnit.Clear();

		blockObjectRightLines.Clear();
		blockObjectLeftLines.Clear();

	}	
	

	
	public void hideStageAssets()
	{
		int len = monsters.Count;
		int i = 0;
		
		for(i = len -1; i >= 0; --i)
		{
			monsters[i].setPositionCtransform(new IVector3(0,-1000,0));
			monsters[i].setVisible(false);
			if(monsters[i].hpBar != null) monsters[i].hpBar.visible = false;

			if(monsters[i].deleteMotionEffect != null && monsters[i].deleteMotionEffect.effectChracter != null) monsters[i].deleteMotionEffect.effectChracter.isEnabled = false;

		}	
		
		len = playerMonster.Count;
		
		for(i = len -1; i >= 0; --i)
		{
			if(playerMonster[i].isPlayer == false)
			{
				playerMonster[i].setPositionCtransform(new IVector3(0,-1000,0));
				playerMonster[i].setVisible(false);
				if(playerMonster[i].hpBar != null) playerMonster[i].hpBar.visible = false;
			}
		}	
		
		len = deadEffectPlayingMonsters.Count;
		
		for(i = len - 1; i >= 0; --i)
		{
			deadEffectPlayingMonsters[i].isDeleteObject = false;
			deadEffectPlayingMonsters[i].setVisible(false);
			cleanMonster(deadEffectPlayingMonsters[i],true);
		}
		
		deadEffectPlayingMonsters.Clear();
		
		if(GameManager.me.stageManager.chaser != null)
		{
			cleanMonster(GameManager.me.stageManager.chaser, true);
			GameManager.me.stageManager.chaser = null;
		}
		
		len = decoMonsters.Count;
		
		for(i = len-1; i >=0; --i)
		{
			decoMonsters[i].setPositionCtransform(new IVector3(0,-1000,0));
			decoMonsters[i].setVisible(false);
		}
		
		len = waitMonsters.Count;
		
		for(i = len-1; i >=0; --i)
		{
			removeAndSetWaitMonster(waitMonsters[i]);
		}
		
		if(GameManager.me.stageManager.protectNPC != null)
		{
			cleanMonster(GameManager.me.stageManager.protectNPC, true);
			GameManager.me.stageManager.protectNPC = null;
		}
		
		if(GameManager.me.stageManager.chaser != null)
		{
			cleanMonster(GameManager.me.stageManager.chaser, true);
			GameManager.me.stageManager.chaser = null;
		}
		
		targetingDecal[0].isEnabled = false;
		targetingDecal[1].isEnabled = false;
		targetingDecal[2].isEnabled = false;

		chargingGauge[0].visible = false;
		chargingGauge[1].visible = false;

		if(GameManager.me.player != null)
		{
			if(GameManager.me.player.hpBar != null) GameManager.me.player.hpBar.visible = false;

			GameManager.me.player.isVisibleForSkillCam  = false;
			GameManager.me.player.changeShader(false,true);
			GameManager.me.player.setColor(new Color(1,1,1,1));
			GameManager.me.player.setVisible(false);
		}


		if(GameManager.me.mapManager.effectArrive != null)
		{
			GameManager.me.mapManager.effectArrive.setVisible(false);
		}

		//GameManager.me.characterManager.inGameGUIContinaer.gameObject.SetActive(false);
	}	






	
	public List<Monster> monsters = new List<Monster>();
	public List<Monster> playerMonster = new List<Monster>();
	public List<Monster> decoMonsters = new List<Monster>();
	
	public List<Monster> deadEffectPlayingMonsters = new List<Monster>();


	private void setMonsterVisible(List<Monster> list, bool visible)
	{
		if(list == null) return;
		foreach(Monster mon in list)
		{
			mon.container.SetActive(visible);
			if(visible == true)
			{
				mon.shadow.gameObject.active = (mon.shadowSize > 0.0f);
			}
		}
	}


	public void restoreMonsterVisibleForSkillCam()
	{
		foreach(Monster mon in monsters)
		{
			if(mon.isEnabled && mon.hp > 0 && mon.isVisibleForSkillCam)
			{
				mon.setVisibleForSkillCam(true, 1.0f);
			}
		}

		foreach(Monster mon in playerMonster)
		{
			if(mon.isEnabled && mon.hp > 0 && mon.isVisibleForSkillCam)
			{
				mon.setVisibleForSkillCam(true, 1.0f);
			}
		}
	}




	private IVector3 _v, _v2;
	
	
	
	public Stack<Monster> changeSideMonsters = new Stack<Monster>();
	


	public IFloat getTeamUnitAverageHPPer(bool isPlayerSide)
	{
		IFloat hpPer = 0;
		int len = 0;
		int checkCount = 0;

		if(isPlayerSide)
		{
			len = playerMonster.Count;

			for(int i = 0; i < len ; ++i)
			{
				if(playerMonster[i].unitData != null && playerMonster[i].hp > 0)
				{
					++checkCount;
					hpPer += playerMonster[i].hpPer;
				}
			}
			
			hpPer /= checkCount;

		}
		else
		{
			len = monsters.Count;
			
			for(int i = 0; i < len ; ++i)
			{
				if(monsters[i].unitData != null && monsters[i].hp > 0)
				{
					++checkCount;
					hpPer += monsters[i].hpPer;
				}
			}

			hpPer /= checkCount;
		}

		return hpPer;
	}


	public void render()
	{
		render(playerMonster);
		render(monsters);
		render(decoMonsters);

		if(GameManager.me.player != null) GameManager.me.player.render();
		if(GameManager.me.stageManager.chaser != null) GameManager.me.stageManager.chaser.render();
		//if(GameManager.me.stageManager.protectNPC != null) GameManager.me.stageManager.protectNPC.render();
	}

	public void render(List<Monster> list)
	{
		int len = list.Count;

		for(int i = len -1; i >= 0; --i)
		{
			if(list[i].isPlayersPlayer == false) list[i].render();
		}		
	}

	public void updateAnimationDelayMethod(List<Monster> list)
	{
		int len = list.Count;
		
		for(int i = len -1; i >= 0; --i)
		{
			list[i].updateAnimationMethod();
		}		
	}


	void updatePlayerMonster()
	{
		int i,len;
		len = playerMonster.Count;

		for(i = len -1; i >= 0; --i)
		{
			if(playerMonster[i].isDeleteObject)
			{
				if(playerMonster[i].isPlayersPlayer == false)
				{
					playerMonster[i].isDeleteObject = false;
#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation)
					{
						checkDeadMonsterEvent(playerMonster[i]);
						cleanMonster(playerMonster[i]);
					}
					else
					{
						playerMonster[i].startDeadEffect();
						deadEffectPlayingMonsters.Add(playerMonster[i]);
					}
#else
					playerMonster[i].startDeadEffect();
					deadEffectPlayingMonsters.Add(playerMonster[i]);
#endif
				}
				playerMonster.RemoveAt(i);
			}
			else// if(playerMonster[i].enabled && playerMonster[i].hp > 0)
			{	
				playerMonster[i].update();

				playerMonster[i].receiveDamageMonstersByMe.Clear();
			}
		}		
	}

	void updateMonster()
	{
		int i,len;
		len = monsters.Count;

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
			Log.log("updateMonster len : " + len); //ff
		}
		#endif

		for(i = len -1; i >= 0; --i)
		{
#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false)
			{
				Log.log(" i : " + i + "   " + monsters[i].resourceId + "    " + monsters[i].lineLeft); //ff
			}
#endif

			if(monsters[i].isDeleteObject)
			{
				monsters[i].isDeleteObject = false;

#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation)
				{
					if(monsters[i].isPlayerSide == false)
					{
						monsters[i].startDeadReward();	
					}

					checkDeadMonsterEvent(monsters[i]);
					cleanMonster(monsters[i]);
				}
				else
				{
					monsters[i].startDeadEffect();
					deadEffectPlayingMonsters.Add(monsters[i]);
				}
				
#else
				monsters[i].startDeadEffect();
				deadEffectPlayingMonsters.Add(monsters[i]);
#endif
				monsters.RemoveAt(i);
			}
			else
			{
				monsters[i].update();

				monsters[i].receiveDamageMonstersByMe.Clear();
			}
		}
	}



	public Monster getMonsterByUniqueId(bool targetIsPlayerSide, int uniqueId, int attackerUniqueId)
	{
		int len = 0;
		if(targetIsPlayerSide)
		{
			len = playerMonster.Count;
			for(int i = len -1; i >= 0; --i)
			{
				if(playerMonster[i].isEnabled)
				{
					if( playerMonster[i].stat.uniqueId == uniqueId && playerMonster[i].stat.uniqueId != attackerUniqueId) return playerMonster[i];
				}
			}
		}
		else
		{
			len = monsters.Count;
			for(int i = len -1; i >= 0; --i)
			{
				if(monsters[i].isEnabled)
				{
					if( monsters[i].stat.uniqueId == uniqueId && monsters[i].stat.uniqueId != attackerUniqueId) return monsters[i];
				}
			}
		}

		return null;
	}



	public Monster getVeryLeftMonster(bool isPlayerSide)
	{
		Monster m = null;

		int len;
		if(isPlayerSide)
		{
			len = playerMonster.Count;
			for(int i = len -1; i >= 0; --i)
			{
				if(playerMonster[i].isDeleteObject == false)
				{
					if(m == null || playerMonster[i].cTransform.position.x < m.cTransform.position.x) m = playerMonster[i];
				}
			}

		}
		else
		{
			len = monsters.Count;
			for(int i = len -1; i >= 0; --i)
			{
				if(monsters[i].isDeleteObject == false)
				{
					if(m == null || monsters[i].cTransform.position.x < m.cTransform.position.x) m = monsters[i];
				}
			}
		}

		return m;
	}


	public bool updatePlayerFirst = false;

	public void update()
	{
		int len = 0;
		int i = 0;

		// 죽은 애들은 찾아서 죽음 처리를 해줌.
//		_v = GameManager.me.player.cTransformPosition;
//		_v.x += GameManager.me.player.moveDirection;
		//_v.y += GameManager.me.player.deltaPlayerPosY;
//		GameManager.me.player.getHitObject(_v);
		if(updatePlayerFirst)
		{
			updatePlayerMonster();
			updateMonster();
		}
		else
		{
			updateMonster();
			updatePlayerMonster();
		}


		
		len = decoMonsters.Count;
		for(i = len -1; i >= 0; --i)
		{
			decoMonsters[i].update();
		}


		//updatePosition();


		// 기다렸다 출력하는 녀석들...
		len = waitMonsters.Count;
		for(i = len -1; i >= 0; --i)
		{
			waitMonsters[i].update();
		}		



		// 죽은뒤 깜빡 거리는 애들.
		len = deadEffectPlayingMonsters.Count;

		for(i = len - 1; i >= 0; --i)
		{
			if(deadEffectPlayingMonsters[i].isDeleteObject)
			{
				deadEffectPlayingMonsters[i].isDeleteObject = false;
				cleanMonster(deadEffectPlayingMonsters[i]); // 다 깜빡거렸으면...
				deadEffectPlayingMonsters.RemoveAt(i);
			}
			else
			{
				deadEffectPlayingMonsters[i].playDeadEffect();
			}
		}

		
		len = changeSideMonsters.Count;

		if(len > 0) SoundData.playHitSound(29,true);

		for(i = len - 1; i >= 0; --i)
		{
			Monster mon = changeSideMonsters.Pop();
			
			if(mon.isPlayerSide)
			{
				if(playerMonster.Remove(mon))
				{
					++GameManager.me.mapManager.monUnitNum;
					mon.isPlayerSide = !mon.isPlayerSide;
					mon.fowardDirectionValue = (mon.isPlayerSide?1000.0f:-1000.0f);

					monsters.Add(mon);	
					
					//mon.isFlipX = true;
					mon.action.startAction();
					
					_v = mon.cTransformPosition;

					if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.PVP)
					{
						_v.x = GameManager.me.pvpPlayer.cTransformPosition.x - 100.0f;
					}
					else
					{
						if( GameManager.me.stageManager.heroMonster != null && GameManager.me.stageManager.heroMonster[0] != null &&  GameManager.me.stageManager.heroMonster[0].hp > 0)
						{
							_v.x = GameManager.me.stageManager.heroMonster[0].cTransformPosition.x - 100.0f;
						}
						else
						{
							_v.x = GameManager.me.characterManager.playerMonsterRightLine + 100.0f;
						}

						
						if(_v.x <= GameManager.me.characterManager.playerMonsterRightLine)
						{
							_v.x += 100.0f;
						}					

					}

					mon.setPositionCtransform(_v);

#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation == false)
#endif
					{
						GameManager.info.effectData[UnitSlot.SUMMON_EFFECT_ENEMY].getEffect(-1000,_v, null, null, mon.summonEffectSize); //GameManager.resourceManager.getInstantPrefabs("Effect/virtical 14");
					}
				}
				
			}
			else
			{
				if(monsters.Remove(mon))
				{
					mon.isPlayerSide = !mon.isPlayerSide;
					mon.fowardDirectionValue = (mon.isPlayerSide?1000.0f:-1000.0f);
					playerMonster.Add(mon);
					
					//mon.isFlipX = false;
					mon.action.startAction();
					
					_v = mon.cTransformPosition;
					_v.x = GameManager.me.player.cTransformPosition.x + 100.0f;

					if(UIPopupSkillPreview.isOpen)
					{
						_v.x += 150.0f;
					}
					else if(_v.x >= GameManager.me.characterManager.monsterLeftLine)
					{
						_v.x -= 100.0f;
					}
					
					GameManager.info.effectData[UnitSlot.getSummonEffectByRare(mon.unitData.rare)].getEffect(-1000,_v, null, null, mon.summonEffectSize); //GameManager.resourceManager.getInstantPrefabs("Effect/virtical 14");
					mon.setPositionCtransform(_v);
					
					
					if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLEMALL)
					{
						--GameManager.me.mapManager.monUnitNum;
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLCOUNT)
					{

						if(GameManager.me.mapManager.leftKilledMonsterNum.ContainsKey(mon.unitData.id))
						{
							GameManager.me.mapManager.leftKilledMonsterNum[mon.unitData.id] = GameManager.me.mapManager.leftKilledMonsterNum[mon.unitData.id]-1;
							
							int num = 0;
							foreach(KeyValuePair<string,Xint> kv in GameManager.me.mapManager.leftKilledMonsterNum)
							{
								num += kv.Value;
							}
							
							GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = num + "M";							
							
						}
					}
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.KILLCOUNT2)
					{
						GameManager.me.mapManager.leftKilledMonster = GameManager.me.mapManager.leftKilledMonster-1;
						GameManager.me.uiManager.uiPlay.lbRoundLeftNum.text = GameManager.me.mapManager.leftKilledMonster + "M";		
					}						
					else if(GameManager.me.stageManager.nowRound.mode == RoundData.MODE.B_TEST)
					{
						int killUnitNum = GameManager.me.mapManager.killedUnitCount.Get() + 1;
						GameManager.me.mapManager.killedUnitCount.Set(killUnitNum);
						GameManager.me.uiManager.uiPlay.challangeModeInfo.update( killUnitNum );
					}
					
					if(HellModeManager.instance.isOpen)
					{
						HellModeManager.instance.setKillUnit(mon.unitData);
					}

					GameManager.me.stageManager.clearChecker(ClearChecker.CHECK_AFTER_DELETE);

				}
			}

			if(mon.miniMapPointer != null) mon.miniMapPointer.changeSide(mon.isPlayerSide);

			mon.addAttachedParticleEffectByCharacterSize(GameManager.info.skillEffectSetupData[29].effUp);
			
			mon.removeTarget();
			mon.removeAttacker();

			// 현재 버전에서는 현혹해서 상대방이 바뀌어도 내 UI에서 스킬 사용이 가능하다.. 버그니까 이것을 적용해야한다.
			mon.changeSide();

			// 혹시 몰라서 넣게되는 방어코드 되겠다...
			if(mon.isPlayerSide)
			{
				foreach(Monster m in playerMonster)
				{
					if(m.target == mon)
					{
						m.target = null;
					}
				}
			}
			else
			{
				foreach(Monster m in monsters)
				{
					if(m.target == mon)
					{
						m.target = null;
					}
				}
			}


		}		
	}
	

	// 컷씬에서 게임 중 죽은 애들을 삭제하기 위함.
	public void updateDeadMonster()
	{
		// 죽은뒤 깜빡 거리는 애들.
		int len = deadEffectPlayingMonsters.Count;
		int i = 0;

		for(i = len - 1; i >= 0; --i)
		{
			if(deadEffectPlayingMonsters[i].isDeleteObject)
			{
				deadEffectPlayingMonsters[i].isDeleteObject = false;
				cleanMonster(deadEffectPlayingMonsters[i]); // 다 깜빡거렸으면...
				deadEffectPlayingMonsters.RemoveAt(i);
			}
			else
			{
				deadEffectPlayingMonsters[i].playDeadEffect();
			}
		}
	}


	
	
	public Xfloat monsterLeftLine = 0;
	public Xfloat monsterRightLine = 0;
	
	public Xfloat playerMonsterLeftLine = 0;
	public Xfloat playerMonsterRightLine = 0;


	public Xfloat monsterLimitLine = 0;
	public Xfloat playerLimitLine = 0;



	public bool checkMonsterBlockLine(Monster mon)
	{
		int len = blockObjectRightLines.Count;
		int i = 0;
		
		for(i = 0; i < len; ++i)
		{
			if(mon.lineLeft > blockObjectRightLines[i] || mon.lineRight < blockObjectLeftLines[i] )
			{
				continue;
			}
			else return false;
		}
		
		return true;
	}


	public bool checkPlayerMonsterBlockLine(Monster mon)
	{
		int len = blockPlayerObjectRightLines.Count;
		int i = 0;
		
		for(i = 0; i < len; ++i)
		{
			if(mon.lineLeft > blockPlayerObjectRightLines[i] || mon.lineRight < blockPlayerObjectLeftLines[i] )
			{
				continue;
			}
			else return false;
		}
		
		return true;
	}

	
	
	private HitObject _checkMonsterHitObject;
	private HitObject _tempHitObject;
	
	public Xfloat totalMonsterUnitHp = 0;
	public Xfloat totalPlayerUnitHp = 0;

	public Xfloat lowestMonsterUnitHpPer = 0;
	public Xfloat lowestPlayerUnitHpPer = 0;

	
	public Xint playerUnitInMonsterTargetZone = 0;
	public Xint monterUnitInPlayerTargetZone = 0;
	
	
	public Xfloat playerUnitHPInMonsterTargetZone = 0;
	public Xfloat monterUnitHPInPlayerTargetZone = 0;
	
	
	public Xint totalAliveMonsterUnitNum = 0;
	public Xint totalAlivePlayerMonsterUnitNum = 0;
	
	public int totalAliveMonsterNum = 0;
	
	public Transform enemyCircle;
	
	int tempModelCount = 0;
	
	public Xfloat targetZoneDistance = 50.0f;
	public Xfloat targetZoneMonsterLine = 0.0f;
	public Xfloat targetZonePlayerLine = 0.0f;
	public Xfloat longestWalkedTargetZonePlayerLine = -99999.0f;
	
	
	private List<Monster> _playerCharacterInTargetZone = new List<Monster>();
	private List<Monster> _monsterCharacterInTargetZone = new List<Monster>();
	
	public Dictionary<string, int> aliveMonUnit = new Dictionary<string, int>(StringComparer.Ordinal);
	public Dictionary<string, int> alivePlayerUnit = new Dictionary<string, int>(StringComparer.Ordinal);
	
	public List<IFloat> blockObjectRightLines = new List<IFloat>();
	public List<IFloat> blockObjectLeftLines = new List<IFloat>();
	
	public List<IFloat> blockPlayerObjectRightLines = new List<IFloat>();
	public List<IFloat> blockPlayerObjectLeftLines = new List<IFloat>();

	public Xfloat playerPosX = 0;
	public Xfloat pvpPlayerPosX = 0;

	public bool hasPlayerDamageTanker = false;
	public bool hasPVPDamageTanker = false;

	// 환각 중인 애들을 갖고 있는지.
	public bool hasSideChangePlayerUnit = false;
	public bool hasSideChangePVPUnit = false;


	public void sort()
	{
		playerMonster.Sort(_sortPlayerMonster);
		monsters.Sort(_sortMonster);
	}

	public void updateLineInformation()
	{

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
			Log.log("updateLineInformation"); //ff 
		}
		#endif

		aliveMonUnit.Clear();
		alivePlayerUnit.Clear();
		blockObjectRightLines.Clear();
		blockObjectLeftLines.Clear();
		blockPlayerObjectRightLines.Clear();
		blockPlayerObjectLeftLines.Clear();

		
		playerMonster.Sort(_sortPlayerMonster);
		monsters.Sort(_sortMonster);
		
		//_tempHitObject = GameManager.me.player.getHitObject();
		
		tempModelCount = 1;
		
		totalPlayerUnitHp.Set( 0 );
		totalMonsterUnitHp.Set( 0 );
		totalAlivePlayerMonsterUnitNum.Set( 0 );
		totalAliveMonsterUnitNum.Set( 0 );	
		playerUnitInMonsterTargetZone.Set( 0 ); 
		monterUnitInPlayerTargetZone = 0;

		lowestMonsterUnitHpPer.Set( 1000 );
		lowestPlayerUnitHpPer.Set( 1000 );

		totalAliveMonsterNum = 0;
		
		playerUnitHPInMonsterTargetZone.Set( 0 );
		monterUnitHPInPlayerTargetZone.Set( 0 );
		
		playerMonsterRightLine.Set( StageManager.mapStartPosX );
		playerMonsterLeftLine.Set( StageManager.mapStartPosX );
		monsterLeftLine.Set( 99000 );
		monsterRightLine.Set( 99000 );		

		monsterLimitLine.Set( 99999 );
		playerLimitLine.Set( -99999 );

		playerPosX.Set( -1000 );
		pvpPlayerPosX.Set( 99999 );

		hasPlayerDamageTanker = false;
		hasPVPDamageTanker = false;

		hasSideChangePlayerUnit = false;
		hasSideChangePVPUnit = false;


		int len = playerMonster.Count;
		int i = 0;
		tempModelCount += len;		
		
		if(len > 0)
		{
			for(i = 0; i < len; ++i)
			{
				if(playerMonster[i].isEnabled)
				{
					if(playerMonster[i].lineRight > playerMonsterRightLine) playerMonsterRightLine.Set( playerMonster[i].lineRight );	
					break;
				}
			}

			targetZonePlayerLine.Set( playerMonsterRightLine - targetZoneDistance );
		}


		if(GameManager.me.player != null) playerPosX.Set( GameManager.me.player.lineLeft );
		if(GameManager.me.pvpPlayer != null) pvpPlayerPosX.Set( GameManager.me.pvpPlayer.lineRight );


		len = monsters.Count;
		
		if(len > 0)
		{
			for(i = 0; i < len; ++i)
			{
				if(monsters[i].isEnabled)
				{
#if UNITY_EDITOR
					if(BattleSimulator.nowSimulation == false) Log.log (i, monsters[i].stat.uniqueId + "   " + monsters[i].resourceId , monsters[i].lineRight); //ff
#endif

					if(monsters[i].lineLeft < monsterLeftLine) monsterLeftLine.Set( monsters[i].lineLeft );
					break;
				}
			}
			
			targetZoneMonsterLine.Set( monsterLeftLine + targetZoneDistance );
		}

			
		len = playerMonster.Count;
		if(len > 0)
		{
			// 얘는 i가 낮을수록 우측.

			for(i = len -1; i >= 0; --i)
			{

				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation == false)
				{
					Log.log(playerMonster[i].stat.uniqueId + "   " + "playermonster " + " i : " + playerMonster[i].lineRight +   "     " + playerMonster[i].resourceId);
				}
				#endif



//				playerMonster[i].isInTargetZone.Set( false );
				
				if(playerMonster[i].isEnabled)//.hp > 0 ) // && playerMonster[i].isDeleteObject == false )
				{
					playerMonster[i].lineIndex = i;

					if(playerMonster[i].unitData != null)
					{
						totalPlayerUnitHp.Set( totalPlayerUnitHp + playerMonster[i].hp );
						totalAlivePlayerMonsterUnitNum.Set(totalAlivePlayerMonsterUnitNum + 1);

						if(lowestPlayerUnitHpPer > playerMonster[i].hpPer)
						{
							lowestPlayerUnitHpPer.Set( playerMonster[i].hpPer.Get() );
						}

						if(alivePlayerUnit.ContainsKey( playerMonster[i].unitData.id ) == false) alivePlayerUnit.Add(playerMonster[i].unitData.id,1);
						else ++alivePlayerUnit[ playerMonster[i].unitData.id ];	

						if(playerMonster[i].characterEffect.isPlayingEffect(36))
						{
							hasPlayerDamageTanker = true;
						}

						if(playerMonster[i].characterEffect.isPlayingEffect(29))
						{
							hasSideChangePlayerUnit = true;
						}

					}
					
					if(playerMonster[i].lineRight >= targetZonePlayerLine)
					{
						if(playerMonster[i].unitData != null)
						{
							playerUnitInMonsterTargetZone.Set( playerUnitInMonsterTargetZone + 1 );
							playerUnitHPInMonsterTargetZone.Set(playerUnitHPInMonsterTargetZone.Get()  + playerMonster[i].hp );
						}
						
//						playerMonster[i].isInTargetZone = true;
					}


					if(playerMonster[i].isBlockMonster)
					{
						//if(monsters[i].isBlockMonster)
						{
							blockPlayerObjectRightLines.Add(playerMonster[i].lineRight);
							blockPlayerObjectLeftLines.Add(playerMonster[i].lineLeft);
						}
					}

				}
			}				
		}
		
		if(targetZonePlayerLine > longestWalkedTargetZonePlayerLine) longestWalkedTargetZonePlayerLine = targetZonePlayerLine;
		
//		GameManager.me.player.isInTargetZone = (GameManager.me.player.lineLeft >= targetZonePlayerLine);
		
		
		// 적 몬스터...
		len = monsters.Count;
		
		if(len > 0)
		{
			// 인덱스가 0이면 좌측. 올라갈 수록 우측이다.

			// 타겟존에 있는 애들.
			for(i = len -1; i >= 0; --i)
			{

				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation == false)
				{
					Log.log("monster i "+ i + "   " + monsters[i].lineLeft +   "     " + monsters[i].resourceId); //ff 
				}
				#endif

//				monsters[i].isInTargetZone = false;
				if(monsters[i].isEnabled)//.hp > 0)// && monsters[i].isDeleteObject == false )
				{
					monsters[i].lineIndex = i;

					if(monsters[i].unitData != null)
					{
						totalMonsterUnitHp.Set( totalMonsterUnitHp + monsters[i].hp );
						totalAliveMonsterUnitNum.Set(totalAliveMonsterUnitNum + 1);

						if(lowestMonsterUnitHpPer > monsters[i].hpPer)
						{
							lowestMonsterUnitHpPer.Set( monsters[i].hpPer.Get() );
						}

						if(aliveMonUnit.ContainsKey( monsters[i].unitData.id ) == false) aliveMonUnit.Add(monsters[i].unitData.id,1);
						else ++aliveMonUnit[ monsters[i].unitData.id ];		

						if(monsters[i].characterEffect.isPlayingEffect(36))
						{
							hasPVPDamageTanker = true;
						}


						if(monsters[i].characterEffect.isPlayingEffect(29))
						{
							hasSideChangePVPUnit = true;
						}

					}
					
					if(monsters[i].lineLeft <= targetZoneMonsterLine)
					{
						if(monsters[i].unitData != null)
						{
							monterUnitInPlayerTargetZone.Set(monterUnitInPlayerTargetZone + 1 );
							monterUnitHPInPlayerTargetZone.Set( monterUnitHPInPlayerTargetZone + monsters[i].hp);
						}
						
//						monsters[i].isInTargetZone = true;
					}
					
					
					//if(monsters[i].monsterData.category == MonsterCategory.OBJECT)
					if(monsters[i].isBlockMonster)
					{
						//if(monsters[i].isBlockMonster)
						{
							blockObjectRightLines.Add(monsters[i].lineRight);
							blockObjectLeftLines.Add(monsters[i].lineLeft);
						}
					}
					
					++totalAliveMonsterNum;
					
				}
			}				
		}		

		lowestMonsterUnitHpPer.Set( lowestMonsterUnitHpPer.Get() * 100.0f );
		lowestPlayerUnitHpPer.Set( lowestPlayerUnitHpPer.Get() * 100.0f );

		blockObjectRightLines.Sort(_sortByFloat);
		blockObjectLeftLines.Sort(_sortByFloat);

		blockPlayerObjectRightLines.Sort(_sortByFloat);
		blockPlayerObjectLeftLines.Sort(_sortByFloat);

		if(GameManager.me.player.hp > 0)
		{
			GameManager.me.player.prevMoveState = Monster.MoveState.Stop;

			if((Xfloat.greaterThan( GameManager.me.player.lineRight + GameManager.me.player.damageRange * 0.5f + 10 , monsterLeftLine )) && GameManager.me.player.moveState == Player.MoveState.Forward)
			{
				GameManager.me.player.setMovingDirection(Monster.MoveState.Stop);	
			}
		}

		if(GameManager.me.pvpPlayer != null && GameManager.me.pvpPlayer.hp > 0)
		{
			MapManager.zoomCameraTargetX = GameManager.me.pvpPlayer.cTransformPosition.x;

			GameManager.me.pvpPlayer.prevMoveState = Monster.MoveState.Stop;

			if((Xfloat.lessThan( GameManager.me.pvpPlayer.lineLeft - GameManager.me.pvpPlayer.damageRange * 0.5f - 10 , playerMonsterRightLine) ) && GameManager.me.player.moveState == Player.MoveState.Forward)
			{
				GameManager.me.pvpPlayer.setMovingDirection(Monster.MoveState.Stop);	

			}
		}

		tempModelCount += len;

//		
//		monsterRightLine = (Mathf.RoundToInt(monsterRightLine+1));
//		monsterLeftLine = monsterLeftLine;
//		playerMonsterRightLine = playerMonsterRightLine+lineOffset;
//		playerMonsterLeftLine = (Mathf.RoundToInt(playerMonsterLeftLine-1));

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false) Log.logError("[[[[[[[ ", monsterLeftLine + playerMonsterRightLine, "monsterLeftLine:",monsterLeftLine,"playerMonsterRightLine:",playerMonsterRightLine); //ff
#endif

		//GameManager.guiLog = tempModelCount.ToString();
	}


	
	// 시전자 편에서 가장 에너지가 작은 애.
	public Monster getLowestHPTeamTarget(bool attackerIsPlayer, Monster player, BaseSkillData.CheckTargetingType targetingTypeChecker = null, bool includeHero = true)
	{
		Monster cha = null;
		int len = 0;
		int i = 0;
		
		if(attackerIsPlayer == false) // 몬스터중에서 시전자를 제외하고 가장 체력 낮은 놈을...
		{
			len = monsters.Count;
			for(i = 0; i < len; ++i)
			{
				if(monsters[i] == player) continue;
				if(includeHero == false && monsters[i].isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(monsters[i]) == false) continue;
				if(monsters[i].hp > 0)
				{
					if(cha == null || cha.hpPer > monsters[i].hpPer) cha = monsters[i];
				}
			}
		}
		else
		{
			len = playerMonster.Count;
			
			for(i = len-1; i >= 0; --i)
			{
				if(playerMonster[i] == player) continue;
				if(includeHero == false && playerMonster[i].isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(playerMonster[i]) == false) continue;
				if(playerMonster[i].hp > 0)
				{
					if(cha == null || cha.hpPer > playerMonster[i].hpPer) cha = playerMonster[i];
				}
			}			
			
//			if(cha == null || (GameManager.me.player.hp > 0 && cha.hp > GameManager.me.player.hp))
//			{
//				if(GameManager.me.player != player)
//				{
//					return GameManager.me.player;
//				}
//			}
		}
		
		return cha;
	}
	
	
	// 적중에서 가장 에너지가 작은 애.
	public Monster getLowestHPEnemyTarget(bool attackerIsPlayer, BaseSkillData.CheckTargetingType targetingTypeChecker = null, bool includeHero = true)
	{
		Monster cha = null;
		int len = 0;
		int i = 0;
		
		if(attackerIsPlayer) // 몬스터중에서 가장 체력 낮은 놈을...
		{
			len = monsters.Count;
			for(i = 0; i < len; ++i)
			{
				if(targetingTypeChecker != null && targetingTypeChecker(monsters[i]) == false) continue;
				if(monsters[i].hp > 0)
				{
					if(includeHero == false && monsters[i].isHero) continue;
					if(cha == null || cha.hpPer > monsters[i].hpPer) cha = monsters[i];
				}
			}
			
			return cha;
		}
		else
		{
			len = playerMonster.Count;
			for(i = len-1; i >= 0; --i)
			{
				if(targetingTypeChecker != null && targetingTypeChecker(playerMonster[i]) == false) continue;
				if(playerMonster[i].hp > 0)
				{
					if(includeHero == false && playerMonster[i].isHero) continue;
					if(cha == null || cha.hpPer > playerMonster[i].hpPer) cha = playerMonster[i];
				}
			}			
			
//			if(cha == null || (GameManager.me.player.hp > 0 && cha.hp > GameManager.me.player.hp))
//			{
//				return GameManager.me.player;	
//			}
		}
		
		return cha;
	}
	



	
	private List<Monster> _chaByRangeDistance = new List<Monster>();
	


	// 거리 상으로 가장 가까이있는 동료을 찾는다.
	public Monster getCloseTeamTarget(bool attackerIsPlayerSide, Monster player, BaseSkillData.CheckTargetingType targetingTypeChecker = null, bool includeHero = true)
	{
		int len = 0, i = 0;
		
		_chaByRangeDistance.Clear();
		Monster mon;
		
		if(attackerIsPlayerSide == false)
		{
			len = GameManager.me.characterManager.monsters.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.monsters[i];
				if(mon.isEnabled == false || mon == player) continue; 
				if(includeHero == false && mon.isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(mon) == false) continue;
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(player.cTransformPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.playerMonster[i];
				if(mon.isEnabled == false || mon == player) continue; 
				if(includeHero == false && mon.isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(mon) == false) continue;
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(player.cTransformPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		
		_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);				
		
		len = _chaByRangeDistance.Count;
		
		if(len > 0)
		{
			mon = _chaByRangeDistance[0];
			_chaByRangeDistance.Clear();
			return mon;
		}

		return null;
	}	
	
	
	// 적중 나와 가장 가까이에 있는 적을 찾는다.
	public Monster getCloseEnemyTarget(bool attackerIsPlayer, Monster player, BaseSkillData.CheckTargetingType targetingTypeChecker = null, bool includeHero = true)
	{
		int len = 0, i = 0;
		
		_chaByRangeDistance.Clear();
		Monster mon;
		
		// 시전자가 플레이어쪽이라면.
		if(attackerIsPlayer)
		{
			// 몬스터 중에서 나와 가장 가까운 애를 찾는다.
			len = GameManager.me.characterManager.monsters.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = monsters[i];
				if(mon.isEnabled == false) continue; 
				if(includeHero == false && mon.isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(mon) == false) continue;
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(player.cTransformPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = playerMonster[i];
				if(mon.isEnabled == false) continue; 	
				if(includeHero == false && mon.isHero) continue;
				if(targetingTypeChecker != null && targetingTypeChecker(mon) == false) continue;
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(player.cTransformPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		
		_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);				
		
		len = _chaByRangeDistance.Count;
		
		if(len > 0)
		{
			mon = _chaByRangeDistance[0];
			_chaByRangeDistance.Clear();
			 return mon; // 아니면 그냥 반환.
		}

		return null;
	}		




	// 특정 점에서 가장 가까이있는 타겟을 찾는다.
	public Monster getCloseTargetByPosition(bool isPlayerBulletList, Vector3 checkPosition, int shooterUniqueId)
	{
		int len = 0, i = 0;
		
		_chaByRangeDistance.Clear();
		Monster mon;
		
		if(isPlayerBulletList)
		{
			len = GameManager.me.characterManager.monsters.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.monsters[i];
				if(mon.isEnabled == false || mon.stat.uniqueId == shooterUniqueId) continue; 
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(checkPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		else
		{
			len = GameManager.me.characterManager.playerMonster.Count;
			
			for(i = len -1; i >= 0 ; --i)
			{	
				mon = GameManager.me.characterManager.playerMonster[i];
				if(mon.isEnabled == false || mon.stat.uniqueId == shooterUniqueId) continue; 
				mon.distanceFromHitPoint = VectorUtil.DistanceXZ(checkPosition, mon.cTransformPosition);
				_chaByRangeDistance.Add(mon);
			}
		}
		
		_chaByRangeDistance.Sort(CharacterManager.sortByDistHitPoint);				
		
		len = _chaByRangeDistance.Count;
		
		if(len > 0)
		{
			mon = _chaByRangeDistance[0];
			_chaByRangeDistance.Clear();
			return mon;
		}
		
		return null;
	}	
	

	




	// 직선 상으로 가장 가까이에 있는 몇번째 녀석..
	public Monster getCloseEnemyTargetByIndexForHeroMonster(bool attackerIsPlayer, int index)
	{
		int len = 0;

		// index = 0은 상대 히어로다.
		// index = 1을 실제로는 앞부터 라고 보면 되겠다..

		if(attackerIsPlayer) // 주인공에게 적 중 앞에서 있는 몇 번째....
		{
			len = GameManager.me.characterManager.monsters.Count;

			if(index == 0)
			{
				for(int i = 0; i < len; ++i)
				{
					if(GameManager.me.characterManager.monsters[i].hp > 0 && monsters[i].isHero)
					{
						return monsters[i];
					}
				}

				return null;
			}


			if(len - index < 0) return null;
			
			for(int i = 0; i < len; ++i)
			{
				if(GameManager.me.characterManager.monsters[i].hp > 0)
				{
					--index;
					if(index == 0) return GameManager.me.characterManager.monsters[i];
				}
			}
		}
		else // 적 몬스터에게 우리편 검사...
		{
			len = GameManager.me.characterManager.playerMonster.Count;

			if(index == 0)
			{
				if(GameManager.me.player.hp > 0) return GameManager.me.player;
				else return null;
			}


			if(len - index < 0) return null;
			
			for(int i = 0; i < len; ++i)
			{
				if(GameManager.me.characterManager.playerMonster[i].hp > 0)
				{
					--index;
					if(index == 0) return GameManager.me.characterManager.playerMonster[i];
				}
			}					
		}
		
		return null;
	}		
	
	
	
	
	
	// 직선 상으로 가장 가까이에 있는 몇번째 녀석..
	public Monster getCloseEnemyTargetByIndex(bool attackerIsPlayer, int index)
	{
		int len = 0;
		
		if(attackerIsPlayer) // 주인공에게 적 중 앞에서 있는 몇 번째....
		{
			len = GameManager.me.characterManager.monsters.Count;
			
			if(len - index < 0) return null;
			
			for(int i = 0; i < len; ++i)
			{
				if(GameManager.me.characterManager.monsters[i].hp > 0)
				{
					--index;
					if(index == 0) return GameManager.me.characterManager.monsters[i];
				}
			}
		}
		else // 적 몬스터에게 우리편 검사...
		{
			len = GameManager.me.characterManager.playerMonster.Count;

			if(len - index < 0) return null;

			for(int i = 0; i < len; ++i)
			{
				if(GameManager.me.characterManager.playerMonster[i].hp > 0)
				{
					--index;

					// 최전방에서 x 번째 애를 찾는데 이미 애들이 다 죽어서 없다. 그럼 맨 뒤에 녀석을 선택한다.
					if(index == 0 || ((i+1) == len)) 
					{
						return GameManager.me.characterManager.playerMonster[i];
					}

				}
			}					
		}
		
		return null;
	}		
	
	
	// 직선 상으로 가장 가까이에 있는 몇번째 녀석..
	public Monster getCloseMonsterTeamTargetByIndex(int index, Monster attacker)
	{
		int len = 0;
		
		len = GameManager.me.characterManager.monsters.Count;
		
		if(len - index < 0) return null;
		
		for(int i = 0; i < len; ++i)
		{
			if(GameManager.me.characterManager.monsters[i] != attacker && GameManager.me.characterManager.monsters[i].hp > 0)
			{
				--index;
				if(index == 0) return GameManager.me.characterManager.monsters[i];
			}
		}
		return null;
	}		
	
	
	
	// 스테이지 세팅에 의해서 적이 접근하기 전까지는 대기하고 있던 캐릭터를 작동시킨다.
	public void wakeUpMonster(IVector3 targetPos)
	{
		int l = monsters.Count;		
		
		for(int i = l - 1; i >= 0; --i)
		{
			if(monsters[i].waitEnemy)
			{
				if(VectorUtil.Distance(monsters[i].cTransformPosition.x, targetPos.x) < 200.0f)
				{
					monsters[i].wakeUp();
				}
			}
		}
	}
	
	
	[HideInInspector]
	public List<Monster> distanceSorter = new List<Monster>();
		

	private Monster _tempChar;
	private Monster _tempChar2;
	
	// 근접 타입인 적이 우리편 중 공격 대상을 찾는것이다.
	public Monster getShortTargetCharacter(Monster mon)
	{
		distanceSorter.Clear();
		
		int count = 0;
		int i = 0;
		int j = 0;
		_tempChar = null;
		_tempChar2 = null;
		
		// 플레이어 몬스터 세팅...
		// 1,2 번 몬스터를 세팅함.
		
		if(mon.isPlayerSide)
		{
			count = monsters.Count;

			_v2 = mon.cTransformPosition;
			_v2.x = mon.lineRight;

			for(i = 0; i < count; ++i)
			{
				if(monsters[i].isEnabled)
				{
					if(j == 0)
					{
						distanceSorter.Add(monsters[i]);	
						//monsters[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(mon.cTransformPosition, monsters[i].cTransformPosition);
						_v = monsters[i].cTransformPosition;
						_v.x = monsters[i].lineLeft;
						monsters[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(_v2, _v);
						++j;
					}
					else if(distanceSorter[0].lineLeft + targetZoneDistance >= monsters[i].lineLeft)
					{
						distanceSorter.Add(monsters[i]);
						_v = monsters[i].cTransformPosition;
						_v.x = monsters[i].lineLeft;
						monsters[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(_v2, _v);
						//monsters[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(mon.cTransformPosition, monsters[i].cTransformPosition);
					}
					else break;
				}
			}	
			
		}
		else
		{
			count = playerMonster.Count;

			_v2 = mon.cTransformPosition;
			_v2.x = mon.lineLeft;

			for(i = 0; i < count; ++i)
			{
				if(playerMonster[i].isEnabled)
				{
					if(j == 0)
					{
						distanceSorter.Add(playerMonster[i]);	
//						playerMonster[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(mon.cTransformPosition, playerMonster[i].cTransformPosition);

						_v.x.Set( playerMonster[i].lineRight );
						_v.y = playerMonster[i].cTransformPosition.y;
						_v.z = playerMonster[i].cTransformPosition.z;

						playerMonster[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(_v2, _v);
						++j;
					}
					else if(distanceSorter[0].lineRight - targetZoneDistance <= playerMonster[i].lineRight)
					{
						distanceSorter.Add(playerMonster[i]);

						_v.x.Set( playerMonster[i].lineRight );
						_v.y = playerMonster[i].cTransformPosition.y;
						_v.z = playerMonster[i].cTransformPosition.z;

						playerMonster[i].distanceBetweenAttacker = VectorUtil.DistanceXZ(_v2, _v);
					}
					else break;
				}
			}	
		}
		
		distanceSorter.Sort(_sortByDistance);
		
		count = distanceSorter.Count;
		
		if(count == 1) _tempChar = distanceSorter[0];
		else if(count >= 2)
		{
			_tempChar = distanceSorter[0];
			_tempChar2 = distanceSorter[1];
		}

		distanceSorter.Sort(_sortByDistance);

		distanceSorter.Clear();
		
		if(_tempChar != null)
		{
			if(_tempChar2 != null)
			{
				int attacker = _tempChar.attackers.Count;
				
				if(attacker <= 3) return _tempChar;
				else
				{
					attacker -= _tempChar2.attackers.Count;
					
					if(attacker <= 0) return _tempChar;
					else return _tempChar2;
				}
			}
			else
			{
				return _tempChar;
			}
		}			
		
		return _tempChar;
	}	



	public bool checkAliveShortUnitNum(bool isPlayerSide, int checkCount)
	{
		int len = 0;
		if(isPlayerSide)
		{
			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
				{
					++len;
					if(len > checkCount) return false;
				}
			}
		}
		else
		{
			foreach(Monster mon in GameManager.me.characterManager.monsters)
			{
				if(mon.unitData != null && mon.unitData.attackType.isShortType && mon.isEnabled)
				{
					++len;
					if(len > checkCount) return false;
				}
			}
		}

		return true;
	}





	public int getAliveSideChangeUnitNum(bool isPlayerSide, string unitId)
	{
		int len = 0;
		if(isPlayerSide)
		{
			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				if(mon.unitData != null && mon.isEnabled && mon.unitData.id == unitId && mon.characterEffect.isPlayingEffect(29))
				{
					++len;
				}
			}
		}
		else
		{
			foreach(Monster mon in GameManager.me.characterManager.monsters)
			{
				if(mon.unitData != null && mon.isEnabled && mon.unitData.id == unitId && mon.characterEffect.isPlayingEffect(29))
				{
					++len;
				}
			}
		}

		// 도일 캐릭터가 한번에 환각 걸리는것은 5마리로 제한.
		if(len > 5) len = 5;

		return len;
	}







}



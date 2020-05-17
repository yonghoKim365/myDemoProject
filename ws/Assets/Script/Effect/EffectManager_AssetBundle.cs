using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class EffectManager : MonoBehaviour , IManagerBase
{

	public void addDefaultEffect()
	{
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_ENEMY, true);
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_RARE, true);
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_SUPER, true);
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_LEGEND, true);
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_SS, true);
		addLoadEffectData(UnitSlot.SUMMON_EFFECT_NORMAL, true);

		addLoadEffectData(BaseSkillData.E_SHOT_BLUE01, true);
		addLoadEffectData(BaseSkillData.E_SHOT_YELLOW01, true);
		addLoadEffectData(BaseSkillData.E_SHOT_VIOLET01, true);
		addLoadEffectData(BaseSkillData.E_SHOT_GREEN01, true);
		addLoadEffectData(BaseSkillData.E_SHOT_RED01, true);

		addLoadEffectData(Effect.E_HIT_PARTICLE01, true);

		addLoadEffectData(EffectData.E_P_START, true);
		addLoadEffectData(EffectData.E_P_HIT, true);

		addLoadEffectData(UIPlayUnitSlot.DEFAULT_UNIT_SKILL_EFFECT, true);

		addLoadEffectData("E_FX_CHARGING_SS", true);

		addLoadEffectData("E_HIT_STRIKE", true);
		addLoadEffectData("E_METEOR_HIT", true);
		addLoadEffectData("E_KNOCKBACK_01", true);


		if(GameManager.info.setupData.defaultLoadEffects != null)
		{
			foreach(string dle in GameManager.info.setupData.defaultLoadEffects)
			{
				addLoadEffectData(dle, true);
			}
		}


		addLoadEffectData(Player.LEO_ATTACK_EFFECT, true);

		loadEffectFromMonsterData( Character.LEO , true);
		loadEffectFromMonsterData( Character.KILEY , true);
		loadEffectFromMonsterData( Character.CHLOE , true);
		loadEffectFromMonsterData( Character.SCARLETT , true);



	}




	public bool isCompleteLoadEffect = true;
//
//	public bool isCompleteLoadEffect
//	{
//		set
//		{
//			_isCompleteLoadEffect = value;
//			Debug.LogError("_isCompleteLoadEffect : " + value);
//		}
//		get
//		{
//			return _isCompleteLoadEffect;
//		}
//	}



	public Queue<string> _effectLoaderId = new Queue<string>();
	public Queue<string> _effectLoaderFileName = new Queue<string>();

	public List<string> defaultEffectList = new List<string>();

	public List<string> deleteExceptionList = new List<string>();

	public void addLoadEffectData(string effectDataId, bool isDefault = false)
	{
		if(string.IsNullOrEmpty(effectDataId)) return;

		if(GameManager.info.effectData.ContainsKey(effectDataId) == false)
		{
			#if UNITY_EDITOR
			Debug.LogError(effectDataId + " 가 info에 없음 " );
			#endif
			return;
		}


#if UNITY_EDITOR
//		Debug.Log("effectDataId : " + effectDataId);
#endif

		EffectData ed = GameManager.info.effectData[effectDataId];

		if(_nowCheckExceptionEffect)
		{
			if(deleteExceptionList.Contains(ed.resource) == false) deleteExceptionList.Add(ed.resource);
		}

		if(ResourceManager.instance.hasPrefabs(ed.resource)) return;

		//		Debug.LogError("ready add: " + effectDataId);
		
		if(ed.type != EffectData.ResourceType.CHARACTER  && _effectLoaderFileName.Contains(ed.resource) == false)
		{
			if(isDefault && defaultEffectList.Contains(effectDataId) == false)
			{
				defaultEffectList.Add(ed.resource);
			}

			isCompleteLoadEffect = false;
			_effectLoaderId.Enqueue(ed.id);
			_effectLoaderFileName.Enqueue(ed.resource);
		}
	}
	
	private int _totalEffectNum = 0;
	int _leftEffectNum = 0;

	public bool didStartLoadEffect = false;

	public void startLoadEffects(bool veryFirst = false)
	{
		_leftEffectNum =  _effectLoaderId.Count;
		
		#if UNITY_EDITOR
		Debug.Log("startLoadEffects : " + _leftEffectNum);
		#endif
		if(veryFirst) _totalEffectNum = _leftEffectNum;
		
		if(_leftEffectNum> 0)
		{
			didStartLoadEffect = true;

			//			progress = 1.0f - (float)_leftEffectNum / (float)_totalEffectNum;
			_effectLoaderFileName.Dequeue();
			StartCoroutine(loadEffectAsset( GameManager.info.effectData[ _effectLoaderId.Dequeue() ] ));
		}
		else
		{
			isCompleteLoadEffect = true;
			didStartLoadEffect = false;
		}
	}
	
	
	IEnumerator loadEffectAsset(EffectData ed)
	{
		while(_leftEffectNum > 0)
		{

#if UNITY_EDITOR

			bool usePrefabGameobject = false;

			if(string.IsNullOrEmpty(ed.path) == false)
			{
				if(  (UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.usePrefabEffect) || (CutSceneMaker.instance.useCutSceneMaker && CutSceneMaker.instance.usePrefabEffect ) )
				{
					GameObject pgo = (GameObject)(Resources.LoadAssetAtPath("Assets/"+ed.path+".prefab", typeof(GameObject)));

					if(pgo != null)
					{
						ResourceManager.instance.setPrefabFromAssetBundle( ed, null, pgo );
						usePrefabGameobject = true;
					}
				}
			}

			if(usePrefabGameobject == false)
#endif
			{

				string path = AssetBundleManager.getResourceName(ed.resource, AssetBundleManager.ResourceType.Effect);
				
				#if UNITY_IPHONE		
				path = ResourceManager.getLocalFilePath(ResourceManager.EFFECT_IPHONE + path, AssetBundleManager.bundleExtension);
				#else
				path = ResourceManager.getLocalFilePath(ResourceManager.EFFECT_ANDROID + path, AssetBundleManager.bundleExtension);
				#endif
				
				using(WWW asset = new WWW( path))
				{
					yield return asset;

					if(asset == null || asset.error != null || asset.isDone == false)
					{
						#if UNITY_EDITOR
						Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
						#endif
					}
					else if(asset != null && asset.assetBundle != null)
					{
						//				Debug.LogError("save: " + sd.fileName);
						ResourceManager.instance.setPrefabFromAssetBundle( ed, asset.assetBundle );
						
						asset.assetBundle.Unload(false);
					}
					
					if(asset != null) asset.Dispose();
				}
			
			}

			_leftEffectNum =  _effectLoaderId.Count;
			
			#if UNITY_EDITOR
//			Debug.Log("startLoadEffects : " + _leftEffectNum);
			#endif

			if(_leftEffectNum> 0)
			{
				_effectLoaderFileName.Dequeue();
				ed = GameManager.info.effectData[ _effectLoaderId.Dequeue() ];
			}
			else
			{
				didStartLoadEffect = false;
				isCompleteLoadEffect = true;
				break;
			}
		}
	}	












	public void loadEffectFromUnitData(string unitDataId)
	{
		if(string.IsNullOrEmpty(unitDataId) == false && GameManager.info.unitData.ContainsKey(unitDataId))
		{
			loadEffectFromUnitData( GameManager.info.unitData[unitDataId] );
		}
	}


	private void loadEffectFromUnitData(UnitData ud)
	{
#if UNITY_EDITOR
		//Debug.LogError("loadEffectFromUnitData : " + ud.resource);
#endif

		loadEffectByBulletPatternIdAndAttackType(ud.resource, ud.attackType.type);

		//
		//GameManager.info.skillEffectSetupData[_attacker.atkEffectType].effUp, 
		loadEffectFromAtkEffectNum( ud.effectType );

		loadEffectFromMonsterData(  ud.resource );

		if( ud.skill != null)
		{
			for(int i = ud.skill.Length -1; i >= 0; --i)
			{
				if(string.IsNullOrEmpty( ud.skill[i] ) == false && GameManager.info.unitSkillData.ContainsKey(ud.skill[i]) )
				{
					loadEffectFromUnitSkillData( GameManager.info.unitSkillData[ud.skill[i]] );
				}
			}
		}

	}


	private void loadEffectFromUnitSkillData(UnitSkillData us)
	{
		addLoadEffectData(us.colorEffectId);

		addLoadEffectData(us.activeSkillEffect);

		loadEffectByBulletPatternIdAndAttackType((us.resourceId == null)?us.id:us.resourceId, us.exeData.type);

		loadEffectFromSkillEffect(us.skillEffects);
	}



	private void loadEffectFromAtkEffectNum(int type)
	{
		if(GameManager.info.skillEffectSetupData.ContainsKey( type ))
		{
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].effUp );
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].effUpLoop );
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].effDown );
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].effDownLoop );
			
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].upIcon );
			addLoadEffectData(GameManager.info.skillEffectSetupData[ type ].downIcon );
		}
	}





	private void loadEffectFromSkillEffect(SkillEffectData[] sed)
	{
		if(sed != null)
		{
			for(int i = sed.Length - 1; i >= 0; --i)
			{
				if(GameManager.info.skillEffectSetupData.ContainsKey( sed[i].type ))
				{
					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].effUp );
					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].effUpLoop );
					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].effDown );
					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].effDownLoop );

					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].upIcon );
					addLoadEffectData(GameManager.info.skillEffectSetupData[ sed[i].type ].downIcon );
				}
			}
		}
	}


	public void loadEffectFromMonsterData(string monsterId, bool isDefault = false)
	{
		if(string.IsNullOrEmpty(monsterId) == false && GameManager.info.monsterData.ContainsKey(monsterId))
		{
			if(GameManager.info.monsterData[monsterId].effectData != null)
			{
				foreach(AttachedEffectData aed in GameManager.info.monsterData[monsterId].effectData)
				{
					if(aed.type == AttachedEffect.TYPE_PREFAB || aed.type == AttachedEffect.TYPE_EFFECT || aed.type == AttachedEffect.TYPE_INDY_EFFECT)
					{
						addLoadEffectData ( aed.id );
					}
				}
			}



			addLoadEffectData(GameManager.info.monsterData[monsterId].explosionEffect);

			loadEffectFromAnimationData(GameManager.info.monsterData[monsterId].resource);
		}
	}


	private Dictionary<string, AniData> _tempAniDic = null;
	private void loadEffectFromAnimationData(string resourceId, bool isDefault = false)
	{
		_tempAniDic = null;
		if(GameManager.info.aniData.TryGetValue(resourceId, out _tempAniDic))
		{
			foreach(KeyValuePair<string, AniData> kv in _tempAniDic)
			{
				loadEffectFromAnimationData(kv.Value, isDefault);
			}

			_tempAniDic = null;
		}
	}


	private void loadEffectFromAnimationData(AniData ani, bool isDefault = false)
	{
		if(ani.effectNum <= 0) return;
		for(int i = ani.effectNum -1 ; i >= 0; --i)
		{
			addLoadEffectData(ani.effect[i].id, isDefault);
		}
	}

	public void loadEffectFromHeroMonsterData(HeroMonsterData hd)
	{
		loadEffectFromMonsterData(  hd.resource );

		loadEffectFromAtkEffectNum( hd.atkEffectType );

		loadEffectByBulletPatternIdAndAttackType(hd.bulletPatternId, hd.attackType.type);
	}


	GameIDData _tempIdData = new GameIDData();

	public void loadEffectFromHeroSkillData(string id)
	{
		if(string.IsNullOrEmpty(id) == false)
		{
			_tempIdData.parse(id);

			if(GameManager.info.heroSkillData.ContainsKey(_tempIdData.resourceId))
			{
				loadEffectFromHeroSkillData(_tempIdData.skillData);
			}
		}
	}


	public void loadEffectFromHeroSkillData(HeroSkillData hd)
	{
		loadEffectByBulletPatternIdAndAttackType(hd.resource, hd.exeData.type);
		loadEffectFromSkillEffect(hd.skillEffects);
	}


	BulletPatternData bpd = null;
	BulletData bd;
	public void loadEffectByBulletPatternIdAndAttackType(string bulletPatternId, int attackType)
	{
		bpd = null;
		GameManager.info.bulletPatternData.TryGetValue(bulletPatternId, out bpd);

		if(attackType == 15)
		{
			if(bpd != null && bpd.ids != null)
			{
				for(int i = bpd.ids.Length - 1; i >= 0; --i)
				{
					addLoadEffectData ( bpd.ids[i] );
				}
			}
		}
		else
		{
			if(bpd != null && bpd.ids != null)
			{
				for(int i = bpd.ids.Length - 1; i >= 0; --i)
				{
					bd = null;

					GameManager.info.bulletData.TryGetValue(bpd.ids[i], out bd);

					if(bd != null)
					{
						if(bd.effectData != null)
						{
							foreach(AttachedEffectData aed in bd.effectData)
							{
								if(aed.type == AttachedEffect.TYPE_PREFAB || aed.type == AttachedEffect.TYPE_EFFECT || aed.type == AttachedEffect.TYPE_INDY_EFFECT)
								{
									addLoadEffectData ( aed.id );
								}
							}
						}

						addLoadEffectData ( bd.hitEffect );
						if(bd.useDestroyEffect) addLoadEffectData(bd.destroyEffectId);

						if(bd.preloadEffect != null)
						{
							addLoadEffectData(bd.preloadEffect);
						}

					}
				}
			}
		}
	}



	public void loadEffectFromPlayerData(GamePlayerData pd, bool loadPlayerEffectOnly = false)
	{
		if(pd == null) return;
		loadEffectByBulletPatternIdAndAttackType(pd.partsWeapon.parts.baseId, pd.partsWeapon.parts.attackType.type);

		if(loadPlayerEffectOnly == false)
		{
			if(pd.unitResourceId != null)
			{
				foreach(string u in pd.unitResourceId)
				{
					loadEffectFromUnitData(u);
				}
			}
			
			if(pd.skills != null)
			{
				foreach(string s in pd.skills)
				{
					loadEffectFromHeroSkillData(s);
				}
			}
		}
	}


	private bool _nowCheckExceptionEffect = false;

	public void preloadRoundEffect(RoundData rd, bool nowCheckExceptionEffect)
	{
		_nowCheckExceptionEffect = nowCheckExceptionEffect;

		addDefaultEffect();

		if( GameManager.me.playMode == GameManager.PlayMode.replay)
		{
			loadEffectFromPlayerData(GameDataManager.replayAttackerData);
			loadEffectFromPlayerData(GameDataManager.replayAttacker2Data);
		}

		loadEffectFromPlayerData(GameManager.me.player.playerData);

		loadEffectFromPlayerData(GameDataManager.selectedPlayerData);

		if(DebugManager.instance.useTagMatchMode && GameDataManager.instance.selectSubHeroId != null)
		{
			loadEffectFromPlayerData( GameDataManager.instance.heroes[GameDataManager.instance.selectSubHeroId]);
		}

		if(rd == null) return;

		if(rd.mode == RoundData.MODE.PVP)
		{
			loadEffectFromPlayerData(DebugManager.instance.pvpPlayerData);

			if(DebugManager.instance.useTagMatchMode && DebugManager.instance.pvpPlayerData2 != null) 
			{
				loadEffectFromPlayerData(DebugManager.instance.pvpPlayerData2);
			}

			return;
		}
		
		
		if(rd.heroMonsters.Length > 0)
		{
			foreach(StageMonsterData data in rd.heroMonsters)
			{
				loadEffectFromHeroMonsterData( GameManager.info.heroMonsterData[data.id] );

				if(data.skills != null)
				{
					for(int i = data.skills.Length - 1; i >= 0; --i)
					{
						loadEffectFromHeroSkillData(data.skills[i]);
					}
				}

				if(data.units != null)
				{
					for(int i = data.units.Length - 1; i >= 0; --i)
					{
						loadEffectFromUnitData(data.units[i]);
					}
				}
			}
		}
		
		
		if(rd.unitMonsters != null)
		{
			foreach(StageMonsterData data in rd.unitMonsters)
			{
				loadEffectFromUnitData( data.id );
			}		
		}
		
		
		if(rd.decoObject != null)
		{
			foreach(StageMonsterData data in rd.decoObject)
			{
				loadEffectFromMonsterData( GameManager.info.npcData[data.id].resource );
			}				
		}
		
		if(rd.blockObject != null)
		{
			foreach(StageMonsterData data in rd.blockObject)
			{
				loadEffectFromMonsterData( GameManager.info.npcData[data.id].resource );
			}	
		}

		if(rd.destroyObject != null)
		{
			foreach(StageMonsterData data in rd.destroyObject)
			{
				loadEffectFromMonsterData(  GameManager.info.npcData[data.id].resource );
			}
		}


		if(rd.protectObject != null)
		{
			foreach(StageMonsterData data in rd.protectObject)
			{
				loadEffectFromMonsterData(  GameManager.info.npcData[data.id].resource );

				if(data.attr != null)
				{
					string[] attr = data.attr.Split('-');
					
					if(attr.Length > 1)
					{
						addLoadEffectData(attr[1]);
					}
				}
			}
		}



		if(rd.chaser != null)
		{
			loadEffectFromMonsterData( GameManager.info.npcData[rd.chaser.id].resource );
		}

		if(rd.mode == RoundData.MODE.GETITEM)
		{
			foreach(string id in rd.getItemData.itemIds)
			{
				if(GameManager.info.mapObjectData.ContainsKey(id))
				{
					MapObjectData md = GameManager.info.mapObjectData[id];
					
					foreach(AttachedEffectData aed in md.effectData)
					{
						if(aed.type == AttachedEffect.TYPE_PREFAB || aed.type == AttachedEffect.TYPE_EFFECT || aed.type == AttachedEffect.TYPE_INDY_EFFECT)
						{
							addLoadEffectData ( aed.id );
						}
					}
				}
			}
		}

		if(rd.mode == RoundData.MODE.ARRIVE)
		{
			addLoadEffectData("E_EVENT_ARRIVE_EFF");
		}












		if(rd.challengeData != null)
		{
			switch(rd.challengeData.type)
			{
			case StageMonsterData.Type.NPC:
				loadEffectFromMonsterData(GameManager.info.npcData[rd.challengeData.id].resource);
				break;
			case StageMonsterData.Type.UNIT:
				loadEffectFromUnitData(GameManager.info.unitData[rd.challengeData.id]);

				break;
			case StageMonsterData.Type.HERO:
				loadEffectFromHeroMonsterData(GameManager.info.heroMonsterData[rd.challengeData.id]);
				break;
			}
		}
		
		if(rd.units != null)
		{
			foreach(string un in rd.units)
			{
				loadEffectFromUnitData(GameManager.info.unitData[un]);
			}
		}
			


	}


	[HideInInspector]
	public bool loadAllEffects = false;

	public void loadAllSkillEffect()
	{
		foreach(KeyValuePair<int, SkillEffectSetupData> kv in GameManager.info.skillEffectSetupData)
		{
			addLoadEffectData(kv.Value.effDown);
			addLoadEffectData(kv.Value.effUp);

			addLoadEffectData(kv.Value.effDownLoop);
			addLoadEffectData(kv.Value.effUpLoop);

			addLoadEffectData(kv.Value.downIcon);
			addLoadEffectData(kv.Value.upIcon);
		}

		startLoadEffects(true);

		loadAllEffects = true;
	}


}

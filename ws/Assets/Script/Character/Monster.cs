using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class Monster : MonoBehaviour
{

	public int lineIndex = 0;

	public CharacterAction action = null;
	
	protected float _damageMotionDuration = 1000.0f;
	
	[HideInInspector]
	public Monster parent = null;
	
	[HideInInspector]
	public bool isPet = false;
	
	private Color _tempColor;
	
	public HeroMonsterData heroMonsterData;

	protected int _unitSlotsNum = 0;
	protected int _skillSlotsNum = 0;
	private int _aiSlotsNum = 0;

	public UnitSlot[] unitSlots = null;
	public SkillSlot[] skillSlots = null;
	public AISlot[] aiSlots = null;

	public UnitSlot aiUnitSlot = null;

//	public float distance = 0.0f; // 특정 캐릭터와의 거리.
	
	protected bool _showFirst = false;

	// 스테이지 유닛 세팅시 특정 지점에 가야 활성화 되는 녀석들이 있다.
	public Xbool waitEnemy = false;
	public Xfloat waitLine = 0.0f;
	
	public void wakeUp()
	{
		waitEnemy = false;
		waitLine = 0.0f;
	}


	public void init( GameIDData inputIdData, bool isPlayerMon = false, TYPE type = Monster.TYPE.UNIT, StageMonsterData sMonData = null, bool isPetMonster = false)
	{
		init(inputIdData.transcendData, inputIdData.transcendLevel, isPlayerMon, inputIdData.unitData.id ,isPlayerMon,type,sMonData,isPetMonster);
	}


	public void init(TranscendData td, int[] transcendLevel, string id, bool isPlayerMon = false, TYPE type = Monster.TYPE.UNIT, StageMonsterData sMonData = null, bool isPetMonster = false)
	{
		init(td, transcendLevel, isPlayerMon,id,isPlayerMon,type,sMonData,isPetMonster);
	}

	public static int randomNum = 0;
	public void init(TranscendData transcendData, int[] transcendLevel, bool isPlayerUnitData, string id, bool isPlayerMon = false, TYPE type = Monster.TYPE.UNIT, StageMonsterData sMonData = null, bool isPetMonster = false)
	{
		
#if UNITY_EDITOR		
		cTransform.gameObject.name = ((isPlayerMon)?"pm":"em")+ randomNum++;
		tf.gameObject.name = cTransform.gameObject.name ;
#endif		
		resetDefaultVals();

		if(particleEffect != null) GameManager.me.effectManager.setParticleEffect(particleEffect);

		isPet = isPetMonster;
		isPlayerSide = isPlayerMon;
		fowardDirectionValue = (isPlayerSide?1000.0f:-1000.0f);
		isDeleteObject = false;
		
		_v = tf.localScale;
		_v.x = 1.0f;
		_v.y = 1.0f;
		_v.z = 1.0f;
		tf.localScale = _v;
		
		_q = tf.rotation;
		_v = _q.eulerAngles;
		_v.x = 0.0f;
		
		if(isPlayerSide)
		{
			_v.y = 90.0f;	
		}
		else
		{
			_v.y = 270.0f;
		}
		
		_v.z = 0.0f;
		_q.eulerAngles = _v;
		tf.rotation = _q;
		
		isMonster = true;
		
		npcData = null;
		unitData = null;
		heroMonsterData = null;		
		stageMonsterData = sMonData;
		
		stat.monsterType = type;

		bool needToSetRareEffect = false;

		if(type == TYPE.UNIT)
		{
			isHero = false;

			unitData = GameManager.info.unitData[id];
			unitData.setDataToCharacter(this, transcendData, transcendLevel);

			if(isPlayerMon)
			{
			// GameManager.me.player.unitHpUp 소환생명력증가		
			// GameManager.me.player.unitDefUp

				float tempF = maxHp;
				tempF += tempF * GameManager.me.player.unitHpUp(unitData);//  stat.unitHpUp;
				maxHp = tempF;
				hp = tempF;
				
				tempF = stat.defMagic; 
				tempF += tempF * GameManager.me.player.unitDefUp(unitData);//.stat.unitDefUp;
				stat.defMagic = tempF;
				
				tempF = stat.defPhysic; 
				tempF += tempF * GameManager.me.player.unitDefUp(unitData);//.stat.unitDefUp;
				stat.defPhysic = tempF;		

				if(unitData.rare > 0)
				{
					needToSetRareEffect = true;
				}

			}
			else if(isPlayerUnitData) // 적인데 주인공 소환수 데이터를 갖고 있으면 걔는 무조건 PVP!
			{
				float tempF = maxHp;
				tempF += tempF * GameManager.me.pvpPlayer.unitHpUp(unitData);//.stat.unitHpUp;
				maxHp = tempF;
				hp = tempF;
				
				tempF = stat.defMagic; 
				tempF += tempF * GameManager.me.pvpPlayer.unitDefUp(unitData);//..stat.unitDefUp;
				stat.defMagic = tempF;
				
				tempF = stat.defPhysic; 
				tempF += tempF * GameManager.me.pvpPlayer.unitDefUp(unitData);//..stat.unitDefUp;
				stat.defPhysic = tempF;	

				if(unitData.rare > 0)
				{
					needToSetRareEffect = true;
				}
			}


			#if UNITY_EDITOR		
			cTransform.gameObject.name += "_"+ unitData.rare;
			tf.gameObject.name = cTransform.gameObject.name ;
			#endif		

			
			setAniData(Monster.ATK_IDS[unitData.attackType.type]);
			setDefaultHitRange(_tempAniData, false);//(unitData.attackType.type == 1 || unitData.attackType.type == 2));

				
			if(unitData.skill != null && unitData.skill.Length > 0)
			{
				skillSlots = new UnitSkillSlot[unitData.skill.Length];
				_skillSlotsNum = skillSlots.Length;

				for(i =0; i < _skillSlotsNum; ++i)
				{
					skillSlots[i] = new UnitSkillSlot();					
					skillSlots[i].setData(this, GameManager.info.unitSkillData[unitData.skill[i]]);
				}	
			}
			else
			{
				skillSlots = null;
			}
			
		}
		else if(type == TYPE.HERO)// 히어로....
		{
			isHero = true;
			
			heroMonsterData = GameManager.info.heroMonsterData[id];			
			heroMonsterData.setDataToCharacter(this);
			
			setAniData(Monster.ATK_IDS[heroMonsterData.attackType.type]);
			setDefaultHitRange(_tempAniData, false);//(heroMonsterData.attackType.type == 1 || heroMonsterData.attackType.type == 2));

			setUpdateWhenOffscreen(true);

			int len = 0;
			
			if(stageMonsterData != null)
			{
				if(stageMonsterData.units != null)
				{
					_unitSlotsNum = stageMonsterData.units.Length;
					unitSlots = new UnitSlot[_unitSlotsNum];
					
					len = unitSlots.Length;
					for(i =0; i < len; ++i)
					{
						unitSlots[i] = new UnitSlot();
						unitSlots[i].setData(this, GameManager.info.unitData[stageMonsterData.units[i]]);
					}
				}

				_skillSlotsNum = stageMonsterData.skills.Length;
				skillSlots = new HeroSkillSlot[_skillSlotsNum];
				len = _skillSlotsNum;

				for(i =0; i < len; ++i)
				{
					skillSlots[i] = new HeroSkillSlot();
					GameIDData skillInfo = new GameIDData();
					skillInfo.parse(stageMonsterData.skills[i], GameIDData.Type.Skill);
					skillSlots[i].setData(this, skillInfo);
				}

				_aiSlotsNum = stageMonsterData.ai.Length;
				aiSlots = new AISlot[_aiSlotsNum];
				
				len = _aiSlotsNum;
				for(i =0; i < len; ++i)
				{
					aiSlots[i] = new AISlot();

#if UNITY_EDITOR
					try
					{
#endif
						aiSlots[i].setData(this, GameManager.info.heroMonsterAI[stageMonsterData.ai[i]]);
#if UNITY_EDITOR
					}
					catch
					{
						Debug.LogError("==== ERRORRRRRR : " +  stageMonsterData.ai[i]);
					}
#endif


				}				
			}
		}
		else if(type == TYPE.NPC)
		{
			isHero = false;
			
			npcData = GameManager.info.npcData[id];
			GameManager.info.npcData[id].setDataToCharacter(this);
		}
		else if(type == TYPE.EFFECT)
		{
			isHero = false;
		}

		baseInit();
		
		damageRange = monsterData.damageRange;

		isBlockMonster = monsterData.isBlockMonster;

		initShadowAndEffectSize();
		
		_showFirst = false;

		_recoveryDelay.Set(0.0f);
		_recoveryDelayMp.Set(0.0f);
		_recoveryDelaySp.Set(0.0f);
		_hpRecoveryDelay.Set( 0.0f );
		//_monsterShowTime = 0.0f;
		
		invincible = false;



		//Log.log("monsterData.category : " + monsterData.category);

		action = GameManager.me.characterManager.getCharacterAction(category);
		action.init(this);
		
		hasAni = !(category == MonsterCategory.Category.OBJECT && ani.GetClip(DEAD) == null);

		_v = tf.localPosition;
		_v.y = 1.0f;
		tf.localPosition = _v;
		
		_damageMotionDuration = 1000.0f;
		
		if(monsterData.deleteMotionType == ChracterDeleteMotionType.EFFECT)
		{
			deleteMotionEffect = GameManager.info.effectData[monsterData.deleteMotionValue].clone();
			if(deleteMotionEffect.type == EffectData.ResourceType.CHARACTER)
			{
#if UNITY_EDITOR				
				Debug.Log ("deleteMotionEffect.effectChracter : " + (deleteMotionEffect.effectChracter == null));
#endif
				
				if(deleteMotionEffect.effectChracter == null)
				{
					deleteMotionEffect.effectChracter = GameManager.me.characterManager.getMonster(false, isPlayerMon, deleteMotionEffect.resource, false);
					if(unitData != null) CharacterUtil.setRare( unitData.rare , deleteMotionEffect.effectChracter);
					else if(isHero) deleteMotionEffect.effectChracter.removeRareLine();
					deleteMotionEffect.effectChracter.init(null, null, monsterData.deleteMotionValue, isPlayerMon, Monster.TYPE.EFFECT);
					deleteMotionEffect.effectChracter.isEnabled = false;
					deleteMotionEffect.effectChracter.cTransform.localScale = cTransform.localScale;
				}
			}
		}
		else deleteMotionEffect = null;
		
		if(ani[Monster.NORMAL] != null) state = Monster.NORMAL;
		
		initHpBar();
		
		initMiniMap();
		
		saveCharacterOriginalValue();

//		if(needToSetRareEffect)
//		{
		 	//GameManager.info.effectData[UnitData.rareEffectId[unitData.rare-1]].getEffect(cTransform.position,null,cTransform); //getParticleEffectByCharacterSize(this,null,cTransform);
			//particleEffect = GameManager.info.effectData[UnitData.rareEffectId[unitData.rare-1]].getParticleEffectByCharacterSize(this,null,cTransform,10000,0,0,0,0.5f);
//		}


		if(GameManager.me.stageManager.isIntro)
		{
			maxHp = 10000000;
			_hp = 10000000;
		}

		stat.maxHp = MathUtil.RoundToInt(maxHp);

		if(GameManager.info.modelData.ContainsKey(resourceId)) useRimShader = GameManager.info.modelData[resourceId].useRimShader;
		

	}


	public void initMiniMap()
	{
		if(miniMapPointer != null)
		{
			switch(category)
			{
			case MonsterCategory.Category.CHASER:
				miniMapPointer.init(CharacterMinimapPointer.CHASER,cTransform, 15);
				break;
			case MonsterCategory.Category.HEROMONSTER:
				miniMapPointer.init(CharacterMinimapPointer.E_HERO,cTransform, 7);
				break;
			case MonsterCategory.Category.PROTECT:
				miniMapPointer.init(CharacterMinimapPointer.PROTECT,cTransform, 12);
				break;
			case MonsterCategory.Category.OBJECT:
				miniMapPointer.init(CharacterMinimapPointer.OBJECT,cTransform, 13);
				break;
			case MonsterCategory.Category.UNIT:
				if(isPlayerSide) miniMapPointer.init(CharacterMinimapPointer.P_UNIT,cTransform, 6);
				else miniMapPointer.init(CharacterMinimapPointer.E_UNIT,cTransform, 5);
				break;
			}
			
			miniMapPointer.isEnabled = true;
		}
	}


	public void setAniData(string aniId)
	{
#if UNITY_EDITOR

		if(DebugManager.instance.useDebug && UnitSkillCamMaker.instance.useEffectSkillCamEditor)
		{
			try
			{
				if(GameManager.info.aniData[resourceId].ContainsKey(aniId))
				{
					_tempAniData = GameManager.info.aniData[resourceId][aniId];
				}
				else
				{
					_tempAniData = GameManager.info.aniData[resourceId][ATK];
				}
				
				if(string.IsNullOrEmpty( _tempAniData.link ) == false)
				{
					_tempAniData = GameManager.info.aniData[resourceId][_tempAniData.link];
				}	
			}
			catch
			{
				_tempAniData = new AniData();
			}
		}
		else
#endif
		{

			if(GameManager.info.aniData[resourceId].TryGetValue(aniId, out _tempAniData) == false)
			{
				_tempAniData = GameManager.info.aniData[resourceId][ATK];
			}

			if(string.IsNullOrEmpty( _tempAniData.link ) == false)
			{
				_tempAniData = GameManager.info.aniData[resourceId][_tempAniData.link];
			}	
		}

	}


	protected void setDefaultHitRange(AniData aniData, bool useHitRangeAsAttackRange)
	{
		hitRange = aniData.hitRange * monsterData.scale;	
		if(useHitRangeAsAttackRange) stat.atkRange = hitRange;
	}


	
	public void onCompleteEffectDeadAni()
	{
		isDeleteObject = true;
		_isEnabled = false;
		_isThisEffectCharacter = true;
		//StartCoroutine(startDeadEffect(true));
	}

	bool _isThisEffectCharacter = false;

	protected bool _playingDeadEffect = false;
	protected int _deadEffectNum = 0;
	protected IFloat _deadEffectDelay = 0.0f;

	public void startDeadEffect()
	{
		_deadEffectNum = 0;


		if(GameManager.info.modelData[resourceId].hasDeleteTime == false)
		{
			_deadEffectDelay = 10.0f;
			playDeadEffect();
			isDeleteObject = true;
		}
		else
		{
			_deadEffectDelay = 0.0f;
			playDeadEffect();
			if(gameObject.activeInHierarchy) StartCoroutine(playDeadEffectCoroutine());
		}
	}


	protected static WaitForSeconds wait05 = new WaitForSeconds(0.5f);
	protected static WaitForSeconds wait008 = new WaitForSeconds(0.08f);

	IEnumerator playDeadEffectCoroutine()
	{
		yield return wait05;

		int i = 5;
		while(i > 0)
		{
			if(UIPlay.nowSkillEffectCamStatus != UIPlay.SKILL_EFFECT_CAM_STATUS.None)
			{
				setVisible(false);
			}
			else
			{
				setVisible(!isVisible);
			}
			yield return wait008;
			--i;
		}

		if(_isThisEffectCharacter)
		{
			deleteMotionEffect.effectChracter.isEnabled = false;
			deleteMotionEffect.effectChracter = null;
		}

	}





	// 깜빡 거리면서 죽는 이펙트...
	// 여기에서 라운드 클리어 체크를 한다.
	public void playDeadEffect()
	{
		_deadEffectDelay += GameManager.globalDeltaTime;

		if(_deadEffectDelay <= 0.8f) return;

		if(isPlayerSide == false) startDeadReward();		

		GameManager.me.characterManager.checkDeadMonsterEvent(this);

		_deadEffectDelay = -9999.0f;

		isDeleteObject = true;
	}

	
	
	public void startAction()
	{
		_showFirst = true;	
		_isEnabled.Set( true );	
	
		if(action != null) action.startAction();

		if(shadow != null)
		{
			shadow.renderer.enabled = isVisible;
		}
	}



	protected void doMotion()
	{
		action.delay.Set( action.delay - GameManager.globalDeltaTime );

		if(isFreeze) return; // 임시 

		if(_damageMotionDuration < 100.0f)
		{
			if(_damageMotionDuration < DAMAGE_MOTION_DURATION[0]) // 0.04f
			{
				_damageMotionDuration += GameManager.globalDeltaTime;
			}
			else if(_damageMotionDuration < DAMAGE_MOTION_DURATION[1]) // 0.04f
			{
				if(_damageMotionStep < 1)
				{
					_v2 = tf.localPosition;
					_v2.x = 0.0f;
					tf.localPosition = _v2;

					_damageMotionStep = 1;
				}

				_damageMotionDuration += GameManager.globalDeltaTime;
			}
			else if(_damageMotionDuration < DAMAGE_MOTION_DURATION[2]) // 0.04f
			{
				if(_damageMotionStep < 2)
				{
					_v2 = tf.localPosition;
					_v2.x += (isPlayerSide)?-damageMotionStep2Value:damageMotionStep2Value;
					tf.localPosition = _v2;

					_damageMotionStep = 2;
				}

				_damageMotionDuration += GameManager.globalDeltaTime;
			}
			else
			{
				_v2 = tf.localPosition;
				_v2.x = 0.0f;
				tf.localPosition = _v2;
				
				_damageMotionDuration = 1000.0f;
			}
		}

		if(UIPopupSkillPreview.isOpen == false)
		{
			action.doMotion();
		}
	}	
	
	
	private float _tSp;
	private float _getScore;
	
	public void startDeadReward()
	{
		if(isPlayerSide == false && unitData != null && GameManager.me.stageManager.nowRound.getItemData != null)
		{
			cTransformPosition.z = 0.0f;
			GameManager.me.stageManager.nowRound.getItemData.createItem(unitData.id, cTransformPosition);
		}
	}

	
	public void initHpBar()
	{
		if(hpBar != null)
		{
			hpBar.isEnabled = true;	
			hpBar.visible = false;
			hpBar.yPos = hitObject.height;
			if(isPlayerSide)
			{
				hpBar.spEnergy.SetSprite(CharacterHpBar.PLAYER_HP_NAME);
			}
			else
			{
				hpBar.spEnergy.SetSprite(CharacterHpBar.ENEMY_HP_NAME);
			}
		}	
	}	


	public void setEffectHpBar(bool isSet)
	{
		if(hpBar != null)
		{
			if(isSet)
			{
				hpBar.spEnergy.SetSprite(CharacterHpBar.HP_MAX_EFFECT_HP_NAME);

				if(monsterUISlotIndex > -1)
				{
					UIPlay.getUnitSlot(monsterUISlotIndex).spHpBar.spriteName = UIPlayUnitSlot.SPECIAL_GAUGE_COLOR;
				}

			}
			else if(isPlayerSide)
			{
				hpBar.spEnergy.SetSprite(CharacterHpBar.PLAYER_HP_NAME);

				if(monsterUISlotIndex > -1)
				{
					UIPlay.getUnitSlot(monsterUISlotIndex).spHpBar.spriteName = UIPlayUnitSlot.NORMAL_GAUGE_COLOR;
				}
			}
			else
			{
				hpBar.spEnergy.SetSprite(CharacterHpBar.ENEMY_HP_NAME);
			}
		}

	}

	


	public void setIdleAndFreeze(bool setEnabledToFalse = false)
	{
		isFreeze.Set( true );
		state = NORMAL;

//		_invincibleTime = 0.0f;

		changeShader(false);

		setColor(_normalColor);

		if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);

		if(setEnabledToFalse)
		{
			_isEnabled.Set( false );
			_currentDamageTime = 1000;
		}

		if(hpBar != null) hpBar.visible = false;
		_energyBarShowTime = -100.0f;
	}

	
	
	

	
	
//============= OVERRIDE METHODS =========================///	

	public virtual void update()
	{


		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
//			Log.log(resourceId + " isReady " + isReady + "  waitEnemy : " + waitEnemy + "  _showFirst : " + _showFirst); //ff
		}
		#endif



		if(isReady == false) return;


		updateAnimationMethod();

		if(waitEnemy)
		{
			if(GameManager.me.characterManager.playerMonsterRightLine >= waitLine) wakeUp();
			return;
		}

		if(_showFirst == false)
		{

			startAction();
		}
		
		baseUpdate();
		
		if(characterEffect != null && characterEffect.check() == false)
		{
			if(stat.monsterType == TYPE.UNIT)
			{

				receiveDamageMonstersByMe.Clear();
			}
			return;
		}

		//Debug.LogError(this + "   action.delay : " + action.delay);

		_v = cTransformPosition;

		doMotion();
		
		if(unitSlots != null) for(int i = 0 ; i < _unitSlotsNum; ++i) unitSlots[i].update();
		if(skillSlots != null) for(int i = 0 ; i < _skillSlotsNum; ++i) skillSlots[i].update();
		if(aiSlots != null) for(int i = 0 ; i < _aiSlotsNum; ++i) { aiSlots[i].update(); };

		hitObject.setPosition(_v);
		
		if(_hp <= 0)
		{
			if(isPlayerSide) lineRight = -99999.0f;
			else lineLeft = 99999.0f;;
			
			if(_state != DEAD) dead ();
		}
		else
		{
			lineRight = hitObject.right;
			lineLeft = hitObject.x;
		}

		if(stat.monsterType == TYPE.UNIT)
		{
			receiveDamageMonstersByMe.Clear();

		}
	}	

	public IVector3 prevTransformPosition = Vector3.zero;
	protected bool _needPositionRender = false;
	public void setPosition(IVector3 pos)
	{
		#if UNITY_EDITOR
//		if(isPlayerSide && isPlayer)
//		{
//			Log.log("== set pos : " + pos, this);
//		}
		#endif

		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame)
		{
			setPositionCtransform(pos);
			return;
		}

		if(GameManager.loopIndex == 0) prevTransformPosition = cTransformPosition;
		//ff Log.log("== set pos : " + pos, this);

		cTransformPosition.x = pos.x;
		cTransformPosition.y = pos.y;
		cTransformPosition.z = pos.z;

		_needPositionRender = true;
	}

	public void setPositionCtransform(IVector3 pos)
	{
//		pos.x = Mathf.Round(pos.x * 100.0f) * 0.01f;
//		pos.y = Mathf.Round(pos.y * 100.0f) * 0.01f;
//		pos.z = Mathf.Round(pos.z * 100.0f) * 0.01f;

		prevTransformPosition.x = pos.x;
		prevTransformPosition.y = pos.y;
		prevTransformPosition.z = pos.z;

		cTransformPosition.x = pos.x;
		cTransformPosition.y = pos.y;
		cTransformPosition.z = pos.z;


		cTransform.position = pos;
//		lineLeftPos = pos; lineLeftPos.x -= damageRange;
//		lineRightPos = pos; lineRightPos.x += damageRange;

		_needPositionRender = false;
	}


	protected Xfloat _recoveryDelaySp = 0f;
	protected Xfloat _recoveryDelayMp = 0f;

	protected Xfloat _recoveryDelay = 0f;
	protected Xfloat _hpRecoveryDelay = 0f;
	
	protected void updateRecoveryData()
	{
#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
		{
			_recoveryDelay = 5;
		}
#endif

		_recoveryDelay.Set(_recoveryDelay + GameManager.globalDeltaTime);
		if(_recoveryDelay >= GameManager.info.setupData.recoveryDelay)
		{
			_recoveryDelay.Set(_recoveryDelay - GameManager.info.setupData.recoveryDelay);
			//if(isPlayer) hp += hpRecovery;
			//else hpWithoutDisplay += hpRecovery;
			mp += stat.mpRecovery;
			sp += stat.spRecovery;
		}
	}		

	

	protected Dictionary<string, float> _useEffectRecords = new Dictionary<string, float>(StringComparer.Ordinal);
	const string HIT_STRIKE_EFFECT = "E_HIT_STRIKE";

	public void playDamageSoundAndEffect(int attackerUniqueId, bool useHitEffect = true, string soundId = null, string effectId = HIT_STRIKE_EFFECT, bool useCharacterSizeEffect = true, bool useCustomPos = false, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, bool useRoration = false)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

		if(useCustomPos)
		{
			_v.x = posX;
			_v.y = posY;
			_v.z = posZ;
		}
		else
		{
			_v = cTransformPosition;
			_v.y += hitObject.height * 0.5f;
			_v.z -= 5.0f;//hitObject.distance * 0.5f;
		}

		if(soundId != null) SoundData.play(soundId);
		//GameManager.soundManager.playEffect("critical");

		if(isPlayer)
		{
			SoundData.playDamageSound( monsterData.resource , playerData.characterId);
		}

		if(useHitEffect)
		{
			if(string.IsNullOrEmpty(effectId) == false)
			{
				if(_useEffectRecords.ContainsKey(effectId) == false)
				{
					_useEffectRecords.Add(effectId ,Time.realtimeSinceStartup);
				}
				else
				{
					if(_useEffectRecords[effectId] + 0.3f > Time.realtimeSinceStartup) return;

					_useEffectRecords[effectId] = Time.realtimeSinceStartup;
				}

				if(useCharacterSizeEffect)
				{
					if(useRoration)
					{
						GameManager.info.effectData[effectId].getParticleEffectByCharacterSize(attackerUniqueId, this).tf.localRotation = tf.localRotation;

					}
					else
					{
						GameManager.info.effectData[effectId].getParticleEffectByCharacterSize(attackerUniqueId, this);
					}

				}
				else
				{
					if(useRoration)
					{
						GameManager.info.effectData[effectId].getEffect(attackerUniqueId, _v, null, null).transform.localRotation = tf.localRotation;	
					}
					else
					{
						GameManager.info.effectData[effectId].getEffect(attackerUniqueId, _v, null, null);	
					}
				}
			}
		}
	}		






	public void playHitEffect(int attackerUniqueId, string effectId = HIT_STRIKE_EFFECT, bool useCharacterSizeEffect = true, bool useCustomPos = false, float posX = 0.0f, float posY = 0.0f, float posZ = 0.0f, bool useRoration = false)
	{
		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;
		
		if(useCustomPos)
		{
			_v.x = posX;
			_v.y = posY;
			_v.z = posZ;
		}
		else
		{
			_v = cTransformPosition;
			_v.y += hitObject.height * 0.5f;
			_v.z -= 5.0f;//hitObject.distance * 0.5f;
		}
		

		if(string.IsNullOrEmpty(effectId) == false)
		{
			if(_useEffectRecords.ContainsKey(effectId) == false)
			{
				_useEffectRecords.Add(effectId ,Time.realtimeSinceStartup);
			}
			else
			{
				if(_useEffectRecords[effectId] + 0.3f > Time.realtimeSinceStartup) return;
				
				_useEffectRecords[effectId] = Time.realtimeSinceStartup;
			}
			
			if(useCharacterSizeEffect)
			{
				if(useRoration)
				{
					GameManager.info.effectData[effectId].getParticleEffectByCharacterSize(attackerUniqueId, this).tf.localRotation = tf.localRotation;
					
				}
				else
				{
					GameManager.info.effectData[effectId].getParticleEffectByCharacterSize(attackerUniqueId, this);
				}
				
			}
			else
			{
				if(useRoration)
				{
					GameManager.info.effectData[effectId].getEffect(attackerUniqueId, _v, null, null).transform.localRotation = tf.localRotation;	
				}
				else
				{
					GameManager.info.effectData[effectId].getEffect(attackerUniqueId, _v, null, null);	
				}
			}
		}
	}		








	public float preDamageCalc(int damageIndex, float attackersAtkPhysic, float attackersAtkMagic, float damagePer = 1.0f, float minimumDamagePer = 1.0f)
	{
		//Log.log("preDamageCalc : "+ damageIndex);
		return MathUtil.getDamage(attackersAtkPhysic, stat.defPhysic ,attackersAtkMagic, stat.defMagic, damagePer, minimumDamagePer, 1.0f/(float)damageIndex);
	}


	protected void addDamageCharacter(Monster shooter, int checkUniqueId)
	{
		if(shooter != null && shooter.stat.uniqueId == checkUniqueId &&  shooter.stat.monsterType == Monster.TYPE.UNIT && shooter.isEnabled)
		{
			prevUniqueId = stat.uniqueId;
			shooter.receiveDamageMonstersByMe.Add(this);
//			Debug.LogError("adddamage : " + shooter.name);
		}
	}


	public virtual bool damage(TYPE shooterType, Monster shooter, int checkUnituqeId, bool isSkillType, int damageIndex, Transform hitter, float attackersAtkPhysic, float attackersAtkMagic, float damagePer = 1.0f, float minimumDamagePer = 1.0f, bool useHitEffect = true, string effectId = "E_HIT_STRIKE", string soundId = null, float tempDiscountDamageValue = 1.0f)
	{
#if UNITY_EDITOR
//		Log.log("===================================");
//		try
//		{
//			if(unitData != null) Log.log("Attacker : " + hitter + "     " + "  target: " + unitData.id + "    targetHp: " + hp);
//			else if(heroMonsterData != null) Log.log("Attacker : " + hitter + "     " + "  target: " + heroMonsterData.id + "    targetHp: " + hp);
//			else Log.log("Attacker : " + hitter + "     " + "  target: " + target + "    targetHp: " + hp);
//
//		}catch(Exception e)
//		{
//
//		}
#endif


		_damage.Set( MathUtil.getDamage(attackersAtkPhysic,stat.defPhysic ,attackersAtkMagic,stat.defMagic, damagePer, minimumDamagePer, 1.0f/(float)damageIndex, tempDiscountDamageValue) );
		
		//if(_isEnabled == false || _invincibleTime > 0.0f) return false;
		if(_isEnabled == false ) return false;
		
		playDamageSoundAndEffect((shooter != null)?shooter.stat.uniqueId:-1000, useHitEffect, soundId, effectId);
		
		if(_damage < 1 || (GameManager.me.isPlaying == false && UIPopupSkillPreview.isOpen == false)) return false;
		
		if(invincible.Get() == false)
		{
			// 회피 확률.
			if(canAvoid()) return false; // shooterType != TYPE.UNIT
			
			_v = cTransformPosition;
			_v.z -= hitObject.depth;
			//GameManager.me.effectManager.getWordEffect().start(("-"+damage), _v);
			

			checkDamageReflection(_damage,shooter,checkUnituqeId);

			if(checkDamageTanker(_damage))
			{
				return false;
			}

			hp -= _damage;
			hpEffect(-_damage);

			if(_hp <= 0)
			{
				dead();
				return true;			
			}
			else 
			{	
				action.damage(_damage);
				if(isSkillType == false) addDamageCharacter(shooter, checkUnituqeId);
				//if(isMonster == false) _invincibleTime = 1.5f;
				//&& category == MonsterCategory.UNIT
				if(_damage > 0 && _damageMotionDuration > 100.0f) damageMotionEffect(tf);
			}

			setDamageFrame();
			return true;
		}

		return false;
	}	
	


	
	
	public virtual bool damage(TYPE shooterType, Monster shooter, int checkUnituqeId, bool isSkillType, IFloat damageValue, bool useHitEffect = true, string effectId = "E_HIT_STRIKE", string soundId = null)
	{
		if(_isEnabled == false) return false;
		
		if(waitEnemy)
		{
			GameManager.me.characterManager.wakeUpMonster(cTransformPosition);
			waitEnemy = false;
		}
		
		playDamageSoundAndEffect((shooter != null)?shooter.stat.uniqueId:-1000,useHitEffect, soundId, effectId);				
		
		//if(damageValue < 1 || _invincibleTime > 0.0f ) return false;
		if( damageValue < 1 ) return false;

		if(GameManager.me.isPlaying == false && UIPopupSkillPreview.isOpen == false) return false;

		if(invincible.Get() == false)
		{
			// 회피 확률.
			if(canAvoid()) return false; // shooterType != TYPE.UNIT			
			
			checkDamageReflection(damageValue,shooter,checkUnituqeId);

			if(checkDamageTanker(damageValue))
			{
				return false;
			}

			hp -= damageValue;
			hpEffect(-damageValue);


			if(_hp <= 0)
			{
				dead();
				return true;			
			}
			else
			{
				if(isSkillType == false) addDamageCharacter(shooter, checkUnituqeId);
				if(damageValue > 0 && _damageMotionDuration > 100.0f) damageMotionEffect(tf);
			}

			setDamageFrame();
			return true;

		}
		
		return false;
	}		


	// 히어로 맞을때는 노란색.
	static Color playerPlayerMinusHpColor = Color.yellow;

	static Color playerSideMinusHpColor = Color.red;
	static Color monsterSideMinusHpColor = Color.white;

	static Color playerSidePlusHpColor = Color.green;
	static Color monsterSidePlusHpColor = new Color(164.0f/255.0f,17.0f/255.0f,218/255.0f);


	int damageParticleNum = 20;

	private static int fuck = 0;
	public void hpEffect(float value)
	{
		if(GameManager.me.recordMode == GameManager.RecordMode.continueGame) return;

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0.5f) return;
#endif

		//- 커졌다 작아지며 위로 올라가면서 사라지게   
		//- HP감소시 : 아군 (빨강색) -00 
		//			- 적 (하얀색) 00  

		//- HP증가시 : 아군 (노랑색) +00 
		//			/ 적 (보라색) +00

		if(value >= 1)
		{

			if(isPlayer) WordEffect.showWordEffect( ((int)value).ToString(), playerSidePlusHpColor, cTransform, _boundExtens.y * 0.7f);
			else if(isPlayerSide)
			{
			}
			else WordEffect.showWordEffect( ((int)value).ToString(), monsterSidePlusHpColor, cTransform, _boundExtens.y * 0.7f);
		}
		else if(value <= -1)
		{
			//데미지를 입을 때, 데미지 양에 따라 파편의 개수가 결정   - 전체 HP를 기준으로..  데미지가 N%라고 했을 때, N*A 개 파티클이 튐
			//q.eulerAngles =  tf.rotation.eulerAngles;//  new Vector3(0,(effect.cha.isPlayerSide)?0:180,0);

			float absValue = MathUtil.abs(value);
			float per = absValue / maxHp * 100.0f;
			damageParticleNum = (int)(per * DAMAGE_PARTICLE_NUM_RATIO);

			if(damageParticleNum < DAMAGE_PARTICLE_MIN_NUM) damageParticleNum = DAMAGE_PARTICLE_MIN_NUM;
			else if(damageParticleNum > DAMAGE_PARTICLE_MAX_NUM) damageParticleNum = DAMAGE_PARTICLE_MAX_NUM;

			if(isPlayer)
			{
				WordEffect.showWordEffect( "-"+((int)absValue), playerPlayerMinusHpColor , cTransform, _boundExtens.y * 0.7f);
				if(isPlayerSide && hpPer < 0.15f) GameManager.me.uiManager.uiPlay.setPlayerDamageEffect();
			}
			else if(isPlayerSide)
			{
				//WordEffect.showWordEffect( "-"+((int)absValue), playerSideMinusHpColor, cTransform, _boundExtens.y * 0.7f);
			}
			else
			{
				WordEffect.showWordEffect( ""+((int)absValue), monsterSideMinusHpColor, cTransform, _boundExtens.y * 0.7f);
			}


			#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation) return;
			#endif

			if(PerformanceManager.isLowPc == false)
			{
				GameManager.info.effectData[Effect.E_HIT_PARTICLE01].getDamageParticleEffectByCharacterSize(this,null,tf, damageParticleNum);
			}
		}
	}


	static List<Monster> _tankers = new List<Monster>();

	public bool checkDamageTanker(IFloat damageValue)
	{
		if(isPlayerSide)
		{
			if(GameManager.me.characterManager.hasPlayerDamageTanker == false)
			{
				return false;
			}

			foreach(Monster mon in GameManager.me.characterManager.playerMonster)
			{
				if(mon.unitData != null && mon != this && mon.isEnabled && mon.characterEffect.check36(this) )
				{
					_tankers.Add(mon);
				}
			}
		}
		else
		{
			if(GameManager.me.characterManager.hasPVPDamageTanker == false)
			{
				return false;
			}

			foreach(Monster mon in GameManager.me.characterManager.monsters)
			{
				if(mon.unitData != null && mon != this && mon.isEnabled && mon.characterEffect.check36(this) )
				{
					_tankers.Add(mon);
				}				
			}
		}

		if(_tankers.Count > 0)
		{
			IFloat tankerDamage =  damageValue / (float)_tankers.Count;

			for(int i = _tankers.Count - 1; i >= 0; --i)
			{
				_tankers[i].damage(Monster.TYPE.NONE, null, -1000, false, tankerDamage);
			}

			_tankers.Clear();
		}

		return false;
	}



	public void checkDamageReflection(IFloat damageValue, Monster shooter = null, int attackerUniqueId = -1000)
	{
		if(shooter != null && shooter.stat.uniqueId == attackerUniqueId)
		{
			shooter.characterEffect.check35(damageValue);
		}
		else if(attackerUniqueId > -1)
		{

			if(isPlayerSide)
			{
				foreach(Monster mon in GameManager.me.characterManager.monsters)
				{
					if(mon.isEnabled && mon.stat.uniqueId == attackerUniqueId)
					{
						mon.characterEffect.check35(damageValue);
						return;
					}
				}
			}
			else
			{
				foreach(Monster mon in GameManager.me.characterManager.playerMonster)
				{
					if(mon.isEnabled && mon.stat.uniqueId == attackerUniqueId)
					{
						mon.characterEffect.check35(damageValue);
						return;
					}
				}
			}
		}
	}


	
	public virtual void damageDead(bool useSound = true)
	{
		if(_isEnabled == false) return;
		
		if(useSound) playDamageSoundAndEffect(-1000);

		hp -= hp;
		hpEffect(-hp);
		dead(useSound);
	}		
	

	
	int _damageMotionStep = 0;
	protected virtual void damageMotionEffect(Transform cha)
	{
		_damageMotionDuration = 0.0f;
		_damageMotionStep = 0;

		_v2 = cha.localPosition;
		_v2.x += (isPlayerSide)?-damageMotionValue:damageMotionValue;
		cha.localPosition = _v2;
	}
	
	public bool needToSetDead = false;
//	public void dead()
//	{
//		needToSetDead = true;
//	}
	
	public virtual void dead(bool useSound = true)
	{
		if(UIPopupSkillPreview.isOpen)
		{
			hp = 1000;
			return;
		}

		if(monsterDeadCallback != null)
		{
			monsterDeadCallback(stat.uniqueId);
			monsterDeadCallback = null;
		}

		if(shadow != null) shadow.renderer.enabled = false;

		_isEnabled.Set( false );
		isFreeze.Set( true );

		if(hpBar != null)
		{
			hpBar.visible = false;
		}
		
		removeAttacker();
		removeTarget();
		
		switch(category)
		{
//		case MonsterCategory.Category.HEROMONSTER:
//			if(GameManager.me.stageManager.clearChecker(ClearChecker.CHECK_IMMEDIATELY, this)) return;
//			break;
		
		case MonsterCategory.Category.UNIT:
			
			if(aiUnitSlot != null)
			{
				aiUnitSlot.deadCreatedMonster();
			}
			else if(monsterUISlotIndex > -1)
			{
				UIPlay.getUnitSlot(monsterUISlotIndex).deadCreatedMonster();
			}

//			if(isPlayerSide == false)
//			{
//				if(GameManager.me.stageManager.clearChecker(ClearChecker.CHECK_IMMEDIATELY, this)) return;
//			}
			break;	
		}


		if(string.IsNullOrEmpty(monsterData.explosionEffect) == false)
		{
			if(GameManager.info.effectData.ContainsKey(monsterData.explosionEffect))
			{
				_v = cTransformPosition;
				_v.z -= 5.0f;
				GameManager.info.effectData[monsterData.explosionEffect].getEffect(attackerUniqueId,_v);
			}
		}

		_v = cTransformPosition;
		_v.y = bodyYCenter;


		characterEffect.clear();

		clearAnimationMethod();

		if(useSound) SoundData.playDieSound(  resourceId );

		setVisibleForSkillCam(true, 1.0f);

		setColor(_normalColor);

		if(monsterUISlotIndex > -1) UIPlay.getUnitSlot(monsterUISlotIndex).spDamageEffect.cachedGameObject.SetActive(false);

		state = Monster.DEAD;

		if(action != null) action.dead();
	}


	public void changeSide()
	{
		if(aiUnitSlot != null)
		{
			aiUnitSlot.deadCreatedMonster();
		}
		else if(monsterUISlotIndex > -1)
		{
			UIPlay.getUnitSlot(monsterUISlotIndex).deadCreatedMonster();
		}

		aiUnitSlot = null;
		monsterUISlotIndex = -1;
	}



	public void skillPreviewDead()
	{
		if(monsterDeadCallback != null)
		{
			monsterDeadCallback(stat.uniqueId);
			monsterDeadCallback = null;
		}
		
		if(shadow != null) shadow.renderer.enabled = false;
		
		_isEnabled.Set( false );
		isFreeze.Set( true );

		removeAttacker();
		removeTarget();

		_v = cTransformPosition;
		_v.y = bodyYCenter;

		characterEffect.clear();
		
		clearAnimationMethod();
		
		SoundData.playDieSound(  resourceId );
		
		setColor(_normalColor);
		
		state = Monster.DEAD;
	}



	
	
	public virtual string state
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			
			if(_state == Monster.DEAD)
			{
				stat.uniqueId = -1;
				prevUniqueId = -1;
				clearEffect();
				invincible = true;
			}

#if UNITY_EDITOR
			//ff Log.log(resourceId, "state : " + _state); //ff
#endif
			playAni(_state);
		}
	}
	
	
	
	public void clearEffect()
	{
		if(attacker != null)
		{
			if(attacker.target == this) attacker.target = null;
		}
		attacker = null;
		attackerUniqueId = -1;
		
		for(i = _effects.Count - 1; i >= 0; --i)
		{
			GameManager.me.effectManager.setAttachedEffect(_effects[i]);
		}
		
		_effects.Clear();
		
		for(i = attachedBullet.Count - 1; i >= 0; --i)
		{
			attachedBullet[i].setDelete();
		}

		attachedBullet.Clear();

		deleteSkillAttachedEffect();

		if(particleEffect != null)
		{
			GameManager.me.effectManager.setParticleEffect(particleEffect);
			particleEffect = null;
		}
	}


	// 유닛 스킬을 썼을때 몸에 붙는 이펙트.
	public void deleteSkillAttachedEffect()
	{
		if(skillAttachedEffect != null)
		{
			GameManager.me.effectManager.setParticleEffect(skillAttachedEffect);
			skillAttachedEffect = null;
		}
	}


	public void cleanPosition()
	{
		_v.x = 0.0f; _v.y = -1000.0f; _v.z = 0.0f;
		setPositionCtransform(_v);
		_v.y = 0.0f;
		tf.localPosition = _v;
	}

	public virtual void resetPosition(bool isHide = false)
	{
//		Log.log("resetPosition : " + name, this);

		_v.x = 0.0f; _v.y = -1000.0f; _v.z = 0.0f;
		setPositionCtransform(_v);
		
		_v.x = 0.0f; _v.y = 0.0f; _v.z = 0.0f;
		tf.localPosition = _vector3Zero;	
		tf.rotation = _defaultQ;
	}	



}







using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class UnitSlot : BaseSlot
{
	public const string U1 = "U1";
	public const string U2 = "U2";
	public const string U3 = "U3";	
	public const string U4 = "U4";
	public const string U5 = "U5";

	public const string SUMMON_EFFECT_ENEMY = "E_SUMMON_ENEMY";
	public const string SUMMON_EFFECT_SS = "E_SUMMON_SS";
	public const string SUMMON_EFFECT_LEGEND = "E_SUMMON_LEGEND";
	public const string SUMMON_EFFECT_NORMAL = "E_SUMMON_NORMAL";
	public const string SUMMON_EFFECT_RARE = "E_SUMMON_RARE";
	public const string SUMMON_EFFECT_SUPER = "E_SUMMON_SUPER";

	public const string SUMMON_SOUND_NORMAL = "btl_summon_normal";
	public const string SUMMON_SOUND_RARE = "btl_summon_rare";
	public const string SUMMON_SOUND_SUPER = "btl_summon_srare";
	public const string SUMMON_SOUND_LEGEND = "btl_summon_legend";

	public static string getSummonSoundByRare(int rare)
	{
		switch(rare)
		{
		case RareType.B:
			return SUMMON_SOUND_RARE;
		case RareType.A:
			return SUMMON_SOUND_SUPER;
		case RareType.S:
		case RareType.SS:
			return SUMMON_SOUND_LEGEND;
		default:
			return SUMMON_SOUND_NORMAL;
		}
	}



	public static string getSummonEffectByRare(int rare)
	{
		switch(rare)
		{
		case RareType.B:
			return SUMMON_EFFECT_RARE;
			break;
		case RareType.A:
			return SUMMON_EFFECT_SUPER;
			break;
		case RareType.S:
			return SUMMON_EFFECT_LEGEND;
		case RareType.SS:
			return SUMMON_EFFECT_SS;
		default:
			return SUMMON_EFFECT_NORMAL;
			break;
		}
	}

	int leftNum = 0;

	public UnitSlot ()
	{
	}

	private Player _player;
	private Monster _heroMon;
	public UnitData unitData;
	public UnitSkillData activeSkillData;

	public Monster mon = null;



	Dictionary<string, int> aliveUnitDic;

	public Xfloat useSp = 0.0f;

	UIPlayUnitSlot uiUnitSlot = null;


	public override void destroy ()
	{
		base.destroy ();
		_player = null;
		_heroMon = null;
		unitData = null;
		activeSkillData = null;
		mon = null;
		aliveUnitDic.Clear();
		aliveUnitDic = null;
		if(uiUnitSlot != null) uiUnitSlot.mon = null;
		uiUnitSlot = null;
		infoData = null;
	}

	private GameIDData infoData = null;
	public void setData(Monster heroMon, GameIDData idData, UIPlayUnitSlot uiSlot = null)
	{
		infoData = idData;
		setData(heroMon,infoData.unitData,uiSlot);
	}


	public void setData(Monster heroMon, UnitData mud, UIPlayUnitSlot uiSlot = null)
	{
		_heroMon = heroMon;
		unitData = mud;

		uiUnitSlot = uiSlot;


		maxCoolTime = unitData.cooltime;

		#if UNITY_EDITOR
		if(uiSlot != null)
		{
			if(DebugManager.instance.ignoreCoolTime) maxCoolTime = 0;
		}
		#endif



		coolTime = maxCoolTime;

		useSp = unitData.sp;

		mon = null;
		canUseOneShotUnitSkill = false;
		activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME;
//		if(uiUnitSlot != null) uiUnitSlot.activeSkillState = activeSkillState;


		if(heroMon.isPlayer)
		{
			_player = (Player)heroMon;

			useSp -= (useSp * _player.summonSpPercent(mud));

			activeSkillData = null;

			coolTime.Set(2.0f); // player 들은 기본 쿨타임이 2초다.

			if(mud.skill != null)
			{
				foreach(string skillId in mud.skill)
				{
					#if UNITY_EDITOR
					Debug.Log(mud.id + " " + skillId);
					#endif
					
					if(GameManager.info.unitSkillData[skillId].activeSkillCooltime > -1)
					{
						activeSkillData = GameManager.info.unitSkillData[skillId];
						break;
					}
				}
			}
			
			if(activeSkillData != null)
			{
				resetActiveSkillCoolTime();
			}
		}

		if(heroMon.isPlayerSide)
		{
			aliveUnitDic = GameManager.me.characterManager.alivePlayerUnit;
		}
		else
		{
			aliveUnitDic = GameManager.me.characterManager.aliveMonUnit;
		}

		leftNum = 0;


		if(uiSlot != null)
		{
			uiSlot.unitSlot = this;
			uiSlot.init(unitData, infoData.transcendData, infoData.transcendLevel, _player);
		}
	}



	public int activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME;

	public bool checkActiveSkillDuration()
	{
		return (activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_DURATION && nowSkillCoolTime > 0);
	}

	public void resetActiveSkillCoolTime(bool useSkill = false)
	{
		if(uiUnitSlot != null)
		{
			uiUnitSlot.resetActiveSkillCoolTime(useSkill);
			return;
		}

		if(activeSkillData == null) return;
		
		maxSkillCoolTime = activeSkillData.activeSkillCooltime;
		nowSkillCoolTime = 0.0f;

		if(useSkill && activeSkillData.activeSkillDuration > 0)
		{
			nowSkillCoolTime = activeSkillData.activeSkillDuration;
			activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_DURATION;

			if(mon  != null)
			{
				if(string.IsNullOrEmpty(activeSkillData.activeSkillEffect) == false)
				{
					if(mon.isPlayerSide) 
					{
						ParticleEffect.SKILL_EFFECT_SHOOTER_ID = mon.stat.uniqueId;
						GameManager.me.cutSceneManager.startUnitSkillCamScene(UIPlayUnitSlot.nowReadySkillCamId, mon.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.UnitSkill);
					}

					mon.skillAttachedEffect = GameManager.info.effectData[activeSkillData.activeSkillEffect].getParticleEffect(mon.stat.uniqueId, mon, null, mon.tf);
				}

				GameManager.info.effectData[UIPlayUnitSlot.DEFAULT_UNIT_SKILL_EFFECT].getParticleEffectByCharacterSize(mon.stat.uniqueId, mon, null, mon.tf);
			}
		}
		else
		{
			activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME;

			if(useSkill && mon  != null)
			{
				GameManager.info.effectData[UIPlayUnitSlot.DEFAULT_UNIT_SKILL_EFFECT].getParticleEffectByCharacterSize(mon.stat.uniqueId, mon, null, mon.tf);
				
				if(string.IsNullOrEmpty(activeSkillData.activeSkillEffect) == false)
				{
					GameManager.info.effectData[activeSkillData.activeSkillEffect].getParticleEffect(mon.stat.uniqueId, mon, null, mon.tf);
				}
			}
			else if(!useSkill & mon != null) mon.deleteSkillAttachedEffect();
		}

	}


	public void clearAllCooltime()
	{
		if(uiUnitSlot != null)
		{
			uiUnitSlot.clearAllCooltime();
			return;
		}

		coolTime.Set(0);

		if(activeSkillData != null)
		{
			resetActiveSkillCoolTime();
		}
	}





	sealed public override void update()
	{
		if(uiUnitSlot != null)
		{
			uiUnitSlot.update(true);
			return;
		}

		if(_heroMon.isPlayer)
		{
			if(aliveUnitDic.ContainsKey(unitData.id))
			{
				leftNum = (unitData.maxSummonAtOnce - aliveUnitDic[unitData.id]);

				if(_heroMon.isPlayerSide)
				{
					if(GameManager.me.characterManager.hasSideChangePlayerUnit)
					{
						leftNum += GameManager.me.characterManager.getAliveSideChangeUnitNum(true,unitData.id);
					}

				}
				else
				{
					if(GameManager.me.characterManager.hasSideChangePVPUnit)
					{
						leftNum += GameManager.me.characterManager.getAliveSideChangeUnitNum(false,unitData.id);
					}

				}
			}
			else leftNum = unitData.maxSummonAtOnce;

			if(leftNum > 0)
			{
				coolTime -= GameManager.globalDeltaTime;
			}

			if(activeSkillData != null)
			{
				// 소환중이면...
				if(leftNum == 0)
				{
					if(activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME)
					{
						nowSkillCoolTime += GameManager.globalDeltaTime;
						
						if(nowSkillCoolTime >= maxSkillCoolTime)
						{
							activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_READY;
						}
						else
						{
							activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME;
						}
					}
					else if(activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_DURATION)
					{
						nowSkillCoolTime -= GameManager.globalDeltaTime;
						
						if(nowSkillCoolTime > 0)
						{
						}
						else
						{
							resetActiveSkillCoolTime();
						}
					}
					else if(activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_READY && mon != null)
					{
						useActiveSkill();
					}
				}
				else
				{
					if(activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_DURATION)
					{
						resetActiveSkillCoolTime();
					}
				}
			}

		}
		else
		{
			coolTime -= GameManager.globalDeltaTime;
		}
	}






	sealed public override bool canUse()
	{
		//Debug.Log("**  " + unitData.id  + "  : canUse?? ");

		if(aliveUnitDic.ContainsKey(unitData.id) == false)
		{
			if(unitData.maxSummonAtOnce <= 0)
			{
				//Debug.Log("unitData.maxSummonAtOnce <= 0  ");
				return false;
			}
		}
		else
		{
			if(unitData.maxSummonAtOnce <= aliveUnitDic[unitData.id])
			{
				//Debug.Log("unitData.maxSummonAtOnce <= GameManager.me.characterManager.aliveMonUnit[unitData.id]  maxOnce: " + unitData.maxSummonAtOnce + "  aliveUnit:" + GameManager.me.characterManager.aliveMonUnit[unitData.id]);
				return false;
			}
		}

		//Debug.Log("_coolTime: " + _coolTime);
		//Debug.Log("_heroMon.sp: " + _heroMon.sp + "   unitData.sp :"+ unitData.sp);

		return (coolTime <= 0.0f && _heroMon.sp >= useSp);
	}


	public bool canUseAfterCoolTime(IFloat useSp)
	{
		IFloat leftTime = coolTime;
		IFloat needSpTime = 0.0f;

		if(_heroMon.sp < useSp)
		{
			needSpTime = (useSp - _heroMon.sp) / _heroMon.stat.spRecovery * GameManager.info.setupData.recoveryDelay;
		}

		if(needSpTime < leftTime) leftTime = needSpTime;

		if( _heroMon.sp - useSp + (_heroMon.stat.spRecovery * (needSpTime / GameManager.info.setupData.recoveryDelay))
		   > useSp)
		{
			return true;
		}
		return false;
	}


	private int _returnPVPPoint = 0;
	public IFloat pvpPoint()
	{
		if(aliveUnitDic.ContainsKey(unitData.id) == false)
		{
			if(unitData.maxSummonAtOnce <= 0)
			{
				return 0;
			}
		}
		else
		{
			if(unitData.maxSummonAtOnce <= aliveUnitDic[unitData.id])
			{
				return 0;
			}
		}

		return (_player.getUnitRuneSelectPoint(infoData) + _player.getUnitRunePoint(this));
	}

	private IVector3 _v;


	IFloat _posX;
	// 히어로 몬스터가 쓰는 메서드.
	public void create(float targetPosX = 0.0f, int nowIndex = 0, int totalIndex = 1)
	{
		// 쿨타임 무시!!!
		//if(canUse())
		{
			_v = _heroMon.cTransformPosition;					
			_posX = _heroMon.hitObject.x;
			_v.x = _posX + targetPosX;
			
			if(totalIndex > 1)
			{
				//Log.logError("===",MapManager.mapSummonHeight , (float)nowIndex , (float)totalIndex , MapManager.mapSummonHeight , (float)(nowIndex + 1)  , (float)totalIndex, "MapManager.summonBottom : " + MapManager.summonBottom);
				//Log.log(MapManager.mapSummonHeight * ((float)nowIndex / (float)totalIndex), MapManager.mapSummonHeight * ((float)(nowIndex + 1)  * (float)totalIndex));
				
				_v.z = (float)GameManager.inGameRandom.Range((int)(MapManager.mapSummonHeight * ((float)nowIndex / (float)totalIndex)),(int)( MapManager.mapSummonHeight * ((float)(nowIndex + 1)  / (float)totalIndex)));
				_v.z += MapManager.summonBottom;
				
				if(_v.z >= 0.0f && _v.z < 30) _v.z = 30;
				else if(_v.z <= 0.0f && _v.z > -30) _v.z = -30;
			}
			else
			{
				_v.z = (float)GameManager.inGameRandom.Range((int)MapManager.summonBottom, (int)MapManager.summonTop);
				if(_v.z >= 0.0f && _v.z < 80) _v.z = 80;
				else if(_v.z <= 0.0f && _v.z > -80) _v.z = -80;
			}
					
			////Debug.LogError("=== CREATE POSITION: " + _v);
			
			
			if(_v.x < GameManager.me.characterManager.playerMonsterRightLine)
			{
				_v.x = GameManager.me.characterManager.playerMonsterRightLine + 200.0f;
			}


			if(_v.x >= _posX) _v.x = _posX;
			
			Monster summonMon = GameManager.me.mapManager.addMonsterToStage(null, null, false, null, unitData.id, _v, unitData);
			_heroMon.sp -= useSp;
			_v.y = 0.5f;

			SoundData.play(UnitSlot.getSummonSoundByRare(unitData.rare));

#if UNITY_EDITOR
			if(BattleSimulator.nowSimulation == false)
#endif
			{
				GameManager.info.effectData[SUMMON_EFFECT_ENEMY].getEffect(-1000,_v, null, null, summonMon.summonEffectSize); 
			}
			//go.transform.parent = _heroMon.tf;
		}
		
		resetCoolTime();
	}

	public static int summonPosIndex = 0;


	public override void resetCoolTime ()
	{
		if(uiUnitSlot != null)
		{
			uiUnitSlot.resetCoolTime();
			return;
		}

		base.resetCoolTime ();
	}


	public void createPVPUnit()
	{
		//GameManager.me.player.state = Character.SHOOT;

		if(uiUnitSlot != null)
		{
			GameManager.replayManager.unitButtons[uiUnitSlot.slotIndex % 10] = true;
			return;
		}

		activeSkillState = UIPlayUnitSlot.STATE_ACTIVE_SKILL_COOLTIME;
//		if(uiUnitSlot != null) uiUnitSlot.activeSkillState = activeSkillState;

		if(_heroMon.isPlayerSide)
		{
			_v = _heroMon.cTransformPosition;
			_v.x += 100;
			_v.y = 1;
			
			++UIPlayUnitSlot.summonPosIndex;
			if(UIPlayUnitSlot.summonPosIndex >= 12) UIPlayUnitSlot.summonPosIndex = 0;

			_v.z = MapManager.summonBottom + MapManager.mapSummonHeight * UIPlayUnitSlot.summonPos[UIPlayUnitSlot.summonPosIndex]/11.0f;

			if(_v.x + 10 > GameManager.me.characterManager.monsterLeftLine) _v.x = GameManager.me.characterManager.monsterLeftLine - 100;
			if(_v.x <= StageManager.mapStartPosX.Get()) _v.x = StageManager.mapStartPosX + 50;

			mon = GameManager.me.mapManager.addMonsterToStage(infoData.transcendData, infoData.transcendLevel, true, null, unitData.id ,_v);
			mon.aiUnitSlot = this;

			_heroMon.sp -= useSp;

			_v.y = 0.5f;

			resetCoolTime();

			if(uiUnitSlot != null)
			{
				mon.monsterUISlotIndex = uiUnitSlot.slotIndex;
				uiUnitSlot.resetCoolTime();
			}
		}
		else
		{
			_v = _heroMon.cTransformPosition;
			_v.x -= 100;
			_v.y = 1;
			
			++summonPosIndex;
			if(summonPosIndex >= 12) summonPosIndex = 0;

			_v.z = MapManager.summonBottom + MapManager.mapSummonHeight * UIPlayUnitSlot.summonPos[summonPosIndex]/11.0f;
			
			if( (_v.x - 10) < GameManager.me.characterManager.playerMonsterRightLine)
			{
				_v.x = GameManager.me.characterManager.playerMonsterRightLine + 100;
			}

			if(_v.x >= StageManager.mapEndPosX.Get())
			{
				_v.x = StageManager.mapEndPosX - 50;
			}

			mon = GameManager.me.mapManager.addPVPPlayerUnitToStage(infoData.transcendData, infoData.transcendLevel, unitData.id ,_v);
			mon.aiUnitSlot = this;
			_heroMon.sp -= useSp;

			_v.y = 0.5f;

			resetCoolTime();
		}

		#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 0) return;
		#endif

		SoundData.play(UnitSlot.getSummonSoundByRare(unitData.rare));
		SoundData.playSummonSound( mon.monsterData.resource );

		GameManager.info.effectData[UnitSlot.getSummonEffectByRare(unitData.rare)].getEffect(-1000,_v, null, null, mon.summonEffectSize); //GameManager.resourceManager.getInstantPrefabs("Effect/virtical 14");
	}

	public Xfloat maxSkillCoolTime = 30.0f;
	public Xfloat nowSkillCoolTime = 0.0f;
	public bool canUseOneShotUnitSkill = false;


	public void deadCreatedMonster()
	{
		if(uiUnitSlot != null)
		{
			uiUnitSlot.deadCreatedMonster();
			return;
		}

		if(mon != null)
		{
			mon = null;

			if(activeSkillState == UIPlayUnitSlot.STATE_ACTIVE_SKILL_DURATION)
			{
				resetActiveSkillCoolTime();
			}
		}
	}


	void useActiveSkill()
	{
		// 쓸 수 있는지 검사...
		if(_player.unitActiveSkillChecker == null || _player.unitActiveSkillChecker.checkActiveSkill(mon,activeSkillData) == false)
		{
			return;
		}

		if(uiUnitSlot != null) //&& activeSkillData.activeSkillCamId != null)
		{
			uiUnitSlot.playSkillEffectCam(activeSkillData.activeSkillCamId);
		}

		if(activeSkillData.activeSkillDuration <= 0)
		{
			canUseOneShotUnitSkill = true;
			activeSkillData.doActiveSkill(mon);
		}
		
		canUseOneShotUnitSkill = false;
		resetActiveSkillCoolTime(true);
	}










}


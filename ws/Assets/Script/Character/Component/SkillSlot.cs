using System;
using UnityEngine;

public abstract class SkillSlot : BaseSlot
{


	public const string S1 = "S1";
	public const string S2 = "S2";
	public const string S3 = "S3";

	public int state = 0;
//
//	public int state
//	{
//		set
//		{
//			_state = value;
//
//			switch(value)
//			{
//			case 0:
//				if(uiSlot != null) Debug.LogError(uiSlot.slotIndex + "   READY");
//				break;
//			case 1:
//				if(uiSlot != null) Debug.LogError(uiSlot.slotIndex + "   COOLTIME");
//				break;
//			case 2:
//				if(uiSlot != null) Debug.LogError(uiSlot.slotIndex + "   PRESS");
//				else Debug.LogError("   PRESS");
//				break;
//			}
//
//
//		}
//		get
//		{
//			return _state;
//		}
//	}


	public SkillSlot ()
	{
	}

	public Xfloat useMp = 0.0f;

	protected Monster mon;
	protected Player player;

	public HeroSkillData skillData;
	protected UnitSkillData _unitSkillData;
	
	public int exeCount = 0;
//	public int runeSelectPoint = 0;

	public bool isB1_3Skill = false;
	public bool isB7_8Skill = false;

	// 현재 스킬의 강화도에 따른 최대 차징 시간.
	public Xfloat chargingTimeLimit = 1.0f;

	public GameIDData skillInfo;

	public bool hasSkillAni = false;

	public virtual void setData(Monster monster, GameIDData heroSkillInfo,  UIPlaySkillSlot inputUiSlot = null)
	{
		uiSlot = inputUiSlot;
		mon = monster;
		exeCount = 0;
		player = null;

		if(heroSkillInfo != null)
		{
			skillInfo = heroSkillInfo;

			if(mon.isPlayer && heroSkillInfo.transcendData != null)
			{
				skillData = heroSkillInfo.skillData.clone();
				heroSkillInfo.transcendData.apply(skillData, heroSkillInfo.transcendLevel);

			}
			else
			{
				skillData = heroSkillInfo.skillData;
				skillData.exeData.init(AttackData.AttackerType.Hero, AttackData.AttackType.Skill, skillData);
			}

			useMp = skillData.mp;

			if( skillData.coolTime <= 0.1f)
			{
				maxCoolTime = 0.0f;
			}
			else
			{
				maxCoolTime = skillData.coolTime;
			}
			
			coolTime = maxCoolTime;

			chargingTimeLimit = skillData.getChargingTime(heroSkillInfo.reinforceLevel);
		}
	}


	public UIPlaySkillSlot uiSlot;
	
	public void setData(Monster monster, UnitSkillData sd)
	{
		player = null;
		exeCount = 0;
		mon = monster;
		maxCoolTime = sd.coolTime;
		coolTime = maxCoolTime + sd.coolTimeStartDelay;
		_unitSkillData = sd;

		useMp = 0.0f;

		hasSkillAni = sd.hasSkillAni;

		sd.exeData.init(AttackData.AttackerType.Unit, AttackData.AttackType.Skill, sd);
	}	


	public override void destroy ()
	{
		base.destroy ();
		skillData = null;
		_unitSkillData = null;
		uiSlot = null;
		skillInfo = null;
		player = null;
		mon = null;
	}

	
	// 그러니까 결론적으로 이 클래스는 히어로 몬스터가 쓰는 클래스란거지...
	public abstract bool canTargetingByHeroMonster(int target);

	public abstract void canSkillMove();

	public abstract void doSkill(int totalDamageNum);

	public abstract void doAlternativeSkillAttack(int totalDamageNum);

	public abstract string getBulletPatternId();



	//========= PVP Point ============//
	
	private IFloat _returnPVPPoint = 0;
	public IFloat pvpPoint()
	{
		_returnPVPPoint = player.getSkillRuneSelectPoint(skillInfo) + player.getSkillRunePoint(this);

		if(mon.isPlayerSide)
		{
			// 아군에게 쓰는 스킬은 대상이 하나라도 있어야한다.
			if(skillData.targetType == Skill.TargetType.ME)
			{
				if(GameManager.me.characterManager.playerMonster.Count <= 1)
				{
					_returnPVPPoint = -10000;
				}
			}
			// 적군에게 쓰는 스킬인데 유닛이 한마디로 없다...
			else if(GameManager.me.stageManager.isPVPMode == false)
			{
				if(skillData.isChangeSideSkill && GameManager.me.characterManager.totalAliveMonsterUnitNum.Get() <= 0)
				{
					_returnPVPPoint = -10000;
				}
			}
		}
		else
		{
			if(skillData.targetType == Skill.TargetType.ME)
			{
				if(GameManager.me.characterManager.monsters.Count <= 1)
				{
					_returnPVPPoint = -10000;
				}
			}
			// 적군에서 쓰는 스킬인데 유닛이 한마디로 없다...
			else if(GameManager.me.stageManager.isPVPMode == false)
			{
				if(skillData.isChangeSideSkill && GameManager.me.characterManager.totalAlivePlayerMonsterUnitNum.Get() <= 0)
				{
					_returnPVPPoint = -10000;
				}
			}
		}

		return _returnPVPPoint;
	}
	
	
	public bool canUseAfterCoolTime(float nowUseMp)
	{
		float leftTime = coolTime;
		float needMpTime = 0.0f;
		
		if(mon.mp < useMp)
		{
			needMpTime = (useMp - mon.mp) / mon.stat.mpRecovery * GameManager.info.setupData.recoveryDelay;
		}
		
		if(needMpTime < leftTime) leftTime = needMpTime;
		
		if( mon.mp - nowUseMp + (mon.stat.mpRecovery * (needMpTime / GameManager.info.setupData.recoveryDelay))
		   > useMp)
		{
			return true;
		}
		return false;
	}



	public virtual void use()
	{
	}

	public virtual void onPress(bool isPress)
	{
	}


	public virtual void cancelChargingAndClearAllCooltime()
	{

	}

	
}


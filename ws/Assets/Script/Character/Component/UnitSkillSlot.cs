using System;
using UnityEngine;

sealed public class UnitSkillSlot : SkillSlot
{
	public UnitSkillSlot ()
	{
	}

	sealed public override bool canTargetingByHeroMonster (int target)
	{
		return true;
	}

	sealed public override void canSkillMove ()
	{
	}

	
	sealed public override bool canUse()
	{
		return _unitSkillData.check(mon, this);		
	}	
	
	
	sealed public override string getBulletPatternId()
	{
		return _unitSkillData.id;
	}
	
	sealed public override void doSkill (int totalDamageNum)
	{
#if UNITY_EDITOR		
//		Debug.Log("Use Skill!!!");
#endif		
		
		if(_unitSkillData.unitMonsterTargetingChecker(mon) == false)
		{
#if UNITY_EDITOR			
//			Debug.Log("== TARGETING FAILED!" + mon.name);
#endif			
			return;
		}

		++exeCount; // 발동 횟수를 늘린다.		
		resetCoolTime(); // 스킬을 썼으면 쿨타임을 초기화한다.		

		if(_unitSkillData.hasSkillAni)
		{
//			if(VersionData.codePatchVer >= 2)
			{
				// 액티브 스킬이 발사될때는 패시브를 쓸 수 없다.
				if(_unitSkillData.isPassiveSkill == false)
				{
					mon.action.canUsePassiveSkill = false;
				}
			}

			mon.state = Monster.ATK_IDS[ _unitSkillData.exeType];
			mon.action.nowUnitSkillData = _unitSkillData;
			mon.action.state = MonsterUnitAction.STATE_WAIT;
		}
		else
		{
			mon.nowBulletPatternId = (_unitSkillData.resourceId == null)?_unitSkillData.id:_unitSkillData.resourceId;
//			Debug.LogError("_unitSkillData.id : " + mon.nowBulletPatternId);
			//Log.log("unit skill slot : " + mon.resourceId);
			_unitSkillData.exeData.monsterShoot(mon, totalDamageNum);
		}

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation && BattleSimulator.instance.skipTime > 1) return;
#endif

		if(_unitSkillData.isPassiveSkill && _unitSkillData.usePassiveSkillEffect && _unitSkillData.conAct != UnitSkillData.Condition.AfterAttack)
		{
			GameManager.info.effectData[EffectData.E_P_START].getParticleEffectByCharacterSize(mon.stat.uniqueId, mon, null, mon.tf);
		}
	}
	
	sealed public override void doAlternativeSkillAttack (int totalDamageNum)
	{
//		Debug.Log("Use AlternativeSkillAttack!!!");
		doSkill(totalDamageNum);
	}
	
}

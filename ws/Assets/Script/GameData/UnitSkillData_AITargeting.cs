using System;
using System.Collections.Generic;
using UnityEngine;

sealed public partial class UnitSkillData : BaseSkillData
{

	//==================================

	public bool unitAiTargeting0(Monster attacker, float dist)
	{
		// 0 : 본인
		if(attacker.isPlayerSide)
		{
			if(dist < MathUtil.abs(attacker.cTransformPosition.x , GameManager.me.characterManager.targetZoneMonsterLine))
			{
				return false;
			}
		}
		else if(attacker.isPlayerSide)
		{
			if(dist < MathUtil.abs(attacker.cTransformPosition.x , GameManager.me.characterManager.targetZonePlayerLine))
			{
				return false;
			}
		}

		attacker.setSkillTarget( attacker );
		attacker.targetPosition = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		return true;
	}
	
	public bool unitAiTargeting1(Monster attacker, float dist)
	{
		// 0 : 본인
		if(attacker.isPlayerSide)
		{
			if(dist < MathUtil.abs(attacker.cTransformPosition.x , GameManager.me.characterManager.targetZoneMonsterLine))
			{
				return false;
			}
		}
		else if(attacker.isPlayerSide)
		{
			if(dist < MathUtil.abs(attacker.cTransformPosition.x , GameManager.me.characterManager.targetZonePlayerLine))
			{
				return false;
			}
		}

		if(attacker.attacker != null && attacker.attacker.stat.uniqueId == attacker.attackerUniqueId && attacker.attacker.hp > 0 && attacker.attacker.stat.monsterType == Monster.TYPE.UNIT)
		{
			attacker.setSkillTarget( attacker.attacker );
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}
		
		return false;
	}
	
	
	public bool unitAiTargeting2(Monster attacker)
	{
		_targetIndexSorter.Clear();
		
		for(int i = attacker.receiveDamageMonstersByMe.Count - 1; i >= 0; --i)
		{
			if(attacker.receiveDamageMonstersByMe[i].isSameMonster()) _targetIndexSorter.Add(i);
		}

//		Debug.LogError("unitAiTargeting2 : " + _targetIndexSorter.Count);

		if(_targetIndexSorter.Count > 0)
		{
			attacker.setSkillTarget( attacker.receiveDamageMonstersByMe[_targetIndexSorter[GameManager.inGameRandom.Range(0,_targetIndexSorter.Count)]] );
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			attacker.receiveDamageMonstersByMe.Clear();
			_targetIndexSorter.Clear();
			return true;
		}
		attacker.receiveDamageMonstersByMe.Clear();
		_targetIndexSorter.Clear();
		
		//2 : 자신이 공격한 상대 (적 유닛 or 히어로)
		//   - 범위공격의 경우, 그 중 1마리 랜덤선택	
		return false;
	}
	
	public bool unitAiTargeting3(Monster attacker)
	{
		//3 : 자신과 가장 가까운 적
		attacker.setSkillTarget( GameManager.me.characterManager.getCloseEnemyTarget(attacker.isPlayerSide, attacker) );
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}	
		
		return false;
	}
	
	public bool unitAiTargeting4(Monster attacker)
	{
		//4 : 자신과 가장 가까운 아군 유닛
		attacker.setSkillTarget( GameManager.me.characterManager.getCloseTeamTarget(attacker.isPlayerSide, attacker, monsterTypeChecker) );
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}	
		
		return false;
	}
	
	public bool unitAiTargeting5_1(Monster attacker)
	{
		//5 : 전방 N거리(속성1) 지점
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += targetAttr[0].Get(); 
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= targetAttr[0].Get();
			attacker.targetPosition = _v;
		}
		
		return true;
	}	
	
	
	public bool unitAiTargeting5_2(Monster attacker)
	{
		//5 : 전방 N거리(속성1) 지점
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += targetAttr[0].Get(); 
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= targetAttr[0].Get();
			attacker.targetPosition = _v;
		}
		
		return true;
	}	
	
	
	public bool unitAiTargeting5_3(Monster attacker)
	{
		//5 : 전방 N거리(속성1) 지점
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;

		if(attacker.isPlayerSide)
		{
			_v.x += targetAttr[0].Get(); 
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= targetAttr[0].Get();
			attacker.targetPosition = _v;
		}
		
		return true;
	}	
	
	
	public bool unitAiTargeting5_4(Monster attacker)
	{
		//5 : 전방 N거리(속성1) 지점
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += targetAttr[0].Get(); 
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= targetAttr[0].Get();
			attacker.targetPosition = _v;
		}
		
		return true;
	}	
	
	public bool unitAiTargeting6(Monster attacker)
	{
		//6 : HP가 가장 낮은 아군 유닛
		attacker.setSkillTarget(GameManager.me.characterManager.getLowestHPTeamTarget(attacker.isPlayerSide, attacker, monsterTypeChecker));
		if(attacker.skillTarget != null)
		{
			attacker.targetPosition = attacker.skillTarget.cTransformPosition;
			attacker.targetHeight = attacker.skillTarget.hitObject.height;	
			attacker.targetUniqueId = attacker.skillTarget.stat.uniqueId;
			return true;
		}
		
		return false;
	}
	
	
	public bool unitAiTargeting7_1(Monster attacker)
	{
		//7 : 정면 방향
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;

		if(attacker.isPlayerSide)
		{
			_v.x += 1000.0f;
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= 1000.0f;
			attacker.targetPosition = _v;
		}
		
		return true;
	}
	
	
	public bool unitAiTargeting7_2(Monster attacker)
	{
		//7 : 정면 방향
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += 1000.0f;
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= 1000.0f;
			attacker.targetPosition = _v;
		}
		
		return true;
	}
	
	
	public bool unitAiTargeting7_3(Monster attacker)
	{
		//7 : 정면 방향
		isTargetingForward = true;
		_v = attacker.cTransformPosition;
		attacker.targetHeight = attacker.hitObject.height;
		attacker.targetUniqueId = -1;
		
		if(attacker.isPlayerSide)
		{
			_v.x += 1000.0f;
			attacker.targetPosition = _v;
		}
		else
		{
			_v.x -= 1000.0f;
			attacker.targetPosition = _v;
		}
		
		return true;
	}
	
	
	
	
	//======================================


}
using System;
using UnityEngine;
using System.Collections.Generic;

sealed public partial class AttackData
{
	// 기대값을 구할때 원래는 스킬이펙트의 성공확률이라는 것은
	// Miss냐 아니냐를 판단하는 기준이었다.
	// 그런데 데미지 기대값을 구할때는 그냥 데미지값에 그 확률을 곱해버리면 된다.
	// 이를테면 성공확률이 40%다. 그러면 최종 데미지 값에 0.4를 곱해버리면 기대값이라고 볼 수 있다.

	float damageCalc0(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}
	
	
	float damageCalc1(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}
	
	float damageCalc2(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, ((float)attr[0]) / 100.0f);		
	}	

	
	float damageCalc3(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 3. 직선발사 단일공격  : 최대비행거리	비행속도
		// 그냥 구하면 됨.
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}
	
	float damageCalc4(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 4. 직선발사 범위공격 : 최대비행거리	비행속도	데미지범위	최소데미지비율	최대타겟유닛수			
		////b.targetRange = attr[2];
		// 최소데미지 비율이란 것은 특정 지점에서 데미지가 떨어졌다.
		// 그럼 반경에 있는 애들한테 스플래시 데미지가 입혀지는데..
		// 최초 데미지가 100이라고 하면 가장 멀리있는 애는 100에서 데미지 비율(50) = 50%의 데미지를 바는다는 뜻이다.
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, ((float)attr[3]) / 100.0f);	
	}
	
	float damageCalc5(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}


	float damageCalc6(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 6. 곡선발사 범위공격 : 타임리밋	데미지범위	최소데미지비율	최대타겟유닛수
		//b.targetRange = attr[1];
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, (float)attr[2] / 100.0f);	
	}
	
	float damageCalc7(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		//b.targetRange = attr[1];
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, (float)attr[2] / 100.0f);	
	}
	
	float damageCalc8(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		//b.targetRange = attr[0];
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, (float)attr[1] / 100.0f);	

	}
	
	float damageCalc9(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 9. 순간 위치고정 이펙트 충돌공격 : 최대타겟유닛수	이펙트타입	이펙트Z/R	이펙트X
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}


	float damageCalc10(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}
	
	float damageCalc11(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 11. 곡선발사 후 지속 위치고정 이펙트 충돌공격 : 타임리밋	최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}	
	
	float damageCalc12(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 12. 지속 캐릭터어태치 이펙트 충돌공격 : 최대타겟유닛수	지속시간	이펙트타입	이펙트Z/R	이펙트X
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}		
	
	float damageCalc13(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// 안 씀.
		// TYPE 13. 시한폭탄 : 폭발대기시간	데미지범위	최소데미지비율	최대타겟유닛수
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}		
	
	float damageCalc14(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 14. 데미지범위	최소데미지비율	최대타겟유닛수
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);//, getDamagePer(attacker.cTransformPosition, target.cTransformPosition),  attr[0]), (float)attr[1] * 0.01f);	
	}		

	float damageCalc15(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 15. 최대전체거리	최대연결거리A	  최대연결거리B	최대연결유닛수	연결딜레이
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}	
	
	
	float damageCalc16(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 16. 최대데미지거리
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker);	
	}	
	
	
	float damageCalc17(int heroSkillLevel, int applyReinforceLevel, Monster attacker, Monster target)
	{
		// TYPE 17. 타임리밋	  데미지범위	최소데미지비율	최대피격유닛수	사선 각도	낙하범위	낙하횟수/간격
		//b.targetRange = attr[1];
		return _skillData.preDamageCalc(target, heroSkillLevel, applyReinforceLevel,  attacker, 1.0f, (float)attr[2] / 100.0f);	
	}

	float getDamagePer(Vector3 centerPos, Vector3 targetPos, float radius)
	{
		float damagePer = 1.0f - ((VectorUtil.DistanceXZ(centerPos, targetPos)) / ( radius * 0.5f));
		if(damagePer < 0.0f) damagePer = 0.1f;
		else if(damagePer > 1.0f) damagePer = 1.0f;
		
		return damagePer;
	}

}

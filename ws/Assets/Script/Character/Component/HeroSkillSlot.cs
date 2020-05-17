using System;

sealed public class HeroSkillSlot : SkillSlot
{
	public HeroSkillSlot ()
	{
	}

	sealed public override bool canUse()
	{
		return (coolTime <= 0.0f && mon.mp >= useMp);
	}
	
	
	// 그러니까 결론적으로 이 클래스는 히어로 몬스터가 쓰는 클래스란거지...
	public override bool canTargetingByHeroMonster(int target)
	{
		return skillData.heroMonsterTargetingChecker(mon, target);
	}
	
	sealed public override void canSkillMove()
	{
		skillData.exeData.skillMove(mon);
	}
	
	sealed public override void doSkill(int totalDamageNum)
	{
		//Log.log(mon.resourceId + "  doSkill");
		// 적용 강화 레벨은 무조건 최대치. 
		// 레어레벨도 몬스터히어로는 차징이 없으므로 해당 스킬의 레어도를 바로 적용한다.
		skillData.exeData.monsterShoot(mon, totalDamageNum, skillInfo.reinforceLevel + skillData.baseLevel   , 20);	
	}

	public override void doAlternativeSkillAttack (int totalDamageNum)
	{
	}

	
	
	sealed public override string getBulletPatternId()
	{
		return skillData.resource;
	}


// =========== PVP 캐릭터 ============== //



}


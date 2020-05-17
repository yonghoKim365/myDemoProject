using System;
using UnityEngine;

sealed public partial class SkillEffectData
{
	public delegate float PreSkillDamageCalc(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Monster attacker = null, float damagePer = 1.0f, float minimumDamagePer = 1.0f);
	public PreSkillDamageCalc preSkillDamageCalc = null;

	//============= PRE DAMAGE 계산기 1~8번, 33번. =================//
	float physicAtkDamagePreCalc(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Monster attacker = null, float damagePer = 1.0f, float minimumDamagePer = 1.0f)
	{
		if(levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel,null,attacker) == false) return 0.0f;
		return skillTarget.preDamageCalc(1, getApplyValue(0, applyReinforceLevel, attacker.stat), 0, damagePer, minimumDamagePer);
	}

	float magicAtkDamagePreCalc(BaseSkillData skillData, int skillLevel, Monster skillTarget, int applyReinforceLevel, Monster attacker = null, float damagePer = 1.0f, float minimumDamagePer = 1.0f)
	{
		if(levelCorrection && skillData.checkLevelCorrection(skillLevel, skillTarget.stat.skillTargetLevel,null,attacker) == false) return 0.0f;	
		
		return skillTarget.preDamageCalc(1, 0, getApplyValue(0, applyReinforceLevel, attacker.stat), damagePer, minimumDamagePer);
	}		
}


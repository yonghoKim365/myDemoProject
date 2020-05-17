using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class _LowDataMgr
{
	
	public float GetFormula(FormulaType type, byte ValueNum)
	{
		Formula.formulaInfo info = null;
		if (FormulaLowData.formulaInfoDic.TryGetValue((uint)type, out info))
		{
			if (ValueNum == 1)
			{
				return info.Value1;
			}
			else if (ValueNum == 2)
			{
				return info.Value2;
			}
			else if (ValueNum == 3)
			{
				return info.Value3;
			}
		}
		return 0;
	}

	// #장비 강화 상승 배율 #장비강화
	// 2017.9.6. kim yong ho
	public float ItemRefineCalc(uint RefineCount)
	{
		// "장비 강화단계 = 0 일 때, 장비 강화 상승 비율 = 0
		// 장비 강화 상승 배율 = (EquipReinf_f.Value1_f * 장비 부위 강화단계) 
		// 장비 능력치 공식 = ( 장비 테이블값 * 0.1 ) * ( 1+ (장비 승급 상승 비율) + (장비 강화 상승 비율) )"
		return GetFormula (FormulaType.EQUIP_REFINE, 1) * RefineCount;
	}

	// #장비 승급 상승 배율 #장비승급
	// 2017.9.6. kim yong ho
	public float ItemGradeCalc(uint GradeCount, uint MinorGrade)
	{

		// 장비 승급 단계 = 0 일 때, 장비 승급 상승 비율 = 0
		// 장비 승급 상승 비율 =(EquipUp_f.Value1_f * 장비 부위 승급단계)
		// 장비 능력치 공식 = ( 장비 테이블값 * 0.1 ) * ( 1+ (장비 승급 상승 비율) + (장비 강화 상승 비율) )

		return GetFormula (FormulaType.EQUIP_UP, 1) * ((GradeCount * 10) + MinorGrade);
	}

	int myRoundToInt2(float v){
		return Mathf.FloorToInt (v += 0.51f);
	}
	
	// 2017.11.2 kyh
	// 장비 능력치 공식 = ( 장비 테이블값 * 0.1 ) * (1 + 장비 강화 상승 비율) 
	// item의 능력치를 계산하고 소수점이하 반올림하여 리턴한다.
	public int GetItemAbilityValueToInt(float itemAbilityValue, uint RefineCount){
		// old
		//return (itemAbilityValue * 0.1f) * ( 1f + ItemGradeCalc (GradeCount, _MinorGrade) + ItemRefineCalc (RefineCount));
		// new 171101 kyh
		return myRoundToInt2 ((itemAbilityValue * 0.1f) * (1f + ItemRefineCalc (RefineCount)));
	}

	// item의 능력치를 계산하고 리턴한다. 반올림 안함.
	public float GetItemAbilityValue(float itemAbilityValue, uint enchantCount){
		return (itemAbilityValue * 0.1f) * ( 1f + ItemRefineCalc (enchantCount));		
	}
	
	// #파트너 강화, #파트너강화
	public float PartnerRefineCalc(uint RefineCount)
	{
		// 파트너 강화 단계 = 0 일 때，파트너 강화 상승 배율 = 0
		// 파트너 강화 단계 > 0 일 때，파트너 강화 상승 배율 = (ParReinf_f.Value1_f * 파트너 강화 단계) 
		// 파트너 능력치 공식 = ( 각 능력치 테이블값 * 0.1 ) * (1 + 파트너 강화 상승 비율)"
		// 2017.10.12. kim yong ho
		return 1f + (GetFormula (FormulaType.PARTNER_REFINE, 1) * RefineCount);
	}
	
//	public float PartnerGradeCalc(uint GradeCount)
//	{
//		if (GradeCount == 0)
//		{
//			return 0f;
//		}
//		
//		return GetFormula(FormulaType.EQUIP_UP, 1) * GradeCount + GetFormula(FormulaType.EQUIP_UP, 2);
//	}
	

	
	// 등급과 포뮬러값을 연산하여 코스튬의 ability value를 리턴한다.
	// costumeAbilityValue : 테이블에서 읽어온 코스튬의 능력치 값. * 0.1 해줘야한다.
	// costumeGrade : 코스튬의 등급. 0부터 시작.
	public float GetCostumeAbilityValue(uint Grade, uint MinorGrade, float costumeAbilityValue){

		// 2017.9.6. kim yong ho
		float v = 0;
		v = (costumeAbilityValue * 0.1f) * (1 + (_LowDataMgr.instance.CostumeGradeCalc (Grade, MinorGrade)));
		return v;
	}

	public float GetSubCharUpValue(){
		Dictionary<ulong, NetData.CharacterInfo>.ValueCollection characters = NetData.instance.GetCharacters().Values;

		if (characters.Count == 1)
			return 1f;

		int totLevel = 0;
		for (int i = 0; i < characters.Count; i++) {
			NetData.CharacterInfo charInfo = characters.ElementAt(i);
			totLevel += charInfo.level;
		}

		float rate = 1f + ((float)totLevel * GetFormula (FormulaType.SUB_CHAR_UP, 1));

		return rate;

	}

	public float CostumeGradeCalc(uint Grade, uint MinorGrade)
	{
		return GetFormula(FormulaType.COSTUME_UP, 1) * ((Grade * 10) + MinorGrade)  + GetFormula(FormulaType.COSTUME_UP, 2);
	}


	// 크리티컬 확률 
	// ( (공격자의 크리티컬 수치 –  피격자의 크리티컬 저항 수치) / (Critical_Rate_f.Value2_f +  피격자의 크리티컬 저항 수치) ) + Critical_Rate_f.Value1_f
	// 만약 (공격자의 크리티컬 수치 –  피격자의 크리티컬 저항 수치) < 0일때 0을 취함.
	// 만약 크리티컬 확률 < 0.05 일때 , 크리티컬 확률 = 5%
	// 만약 크리티컬 확률 > CriticalRC_f.Value1_f 일때, 크리티컬 확률 = CriticalRC_f.Value1_f
	// 2017.8.24. kyh
	public float CalcCriticalRate(float attacker_CriticalChance, float defender_CriticalRes){
		float criticalChange = 0f;
		if( attacker_CriticalChance - defender_CriticalRes < 0 )
		{
			criticalChange = 0;
		}
		else
		{
			// old
			//criticalChange = (attacker.CharInfo.CriticalChance - CharInfo.CriticalRes) / (_LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_RATE, 2)  + CharInfo.CriticalRes);
			
			// ( (공격자의 크리티컬 수치 –  피격자의 크리티컬 저항 수치) / (Critical_Rate_f.Value2_f +  피격자의 크리티컬 저항 수치) ) + Critical_Rate_f.Value1_f
			// 2017.8.24. kyh
			float Critical_Rate1 = _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_RATE, 1);
			float Critical_Rate2 = _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_RATE, 2);
			criticalChange = ((attacker_CriticalChance - defender_CriticalRes) / (Critical_Rate2  + defender_CriticalRes)) + Critical_Rate1;
		}
		
		if(criticalChange < 0.05f)
		{
			criticalChange = 0.05f;
		}
		else if(criticalChange > _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_RATE_CAP, 1) )
		{
			criticalChange = _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_RATE_CAP, 1);
		}

		return criticalChange;
	}

	// 크리티컬 피해량 배율
	// 크리티컬 데미지 * CriticalDmg_f.Value1_f / CriticalDmg_f.Value2_f
	// 2017.8.24. kyh
	public float CalcCriticalDamage(float criticalDamage){

		return criticalDamage * _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_DAMAGE, 1) / _LowDataMgr.instance.GetFormula(FormulaType.CRITICAL_DAMAGE, 2);
	}


	// 생명력 흡수배율
	// ( ( 생명력 흡수 * LifeSteal_f.Value1_f ) / (현재 캐릭터 레벨 + LifeSteal_f.Value2_f ) ) / 100
	// 만약 생명력 흡수배율＞LifeSC_f.Value1_f 일때，생명력 흡수배율 = LifeSC_f.Value1_f
	// 생명력 흡수량 = 최종 데미지 * 생명력 흡수 배율
	// 2017.8.24. kyh
	public float CalcDrainHpRate(float drainHP, int level){
		float drainHpRate = (drainHP * _LowDataMgr.instance.GetFormula (FormulaType.LIFE_STEAL, 1)) / (level + _LowDataMgr.instance.GetFormula (FormulaType.LIFE_STEAL, 2)) / 100f;
		if (drainHpRate > _LowDataMgr.instance.GetFormula (FormulaType.LIFE_STEAL_CAP, 1)) {
			drainHpRate = _LowDataMgr.instance.GetFormula (FormulaType.LIFE_STEAL_CAP, 1);
		}
		return drainHpRate;
	}


	// 데미지 감소 배율 
	//( ( ( 피격자의 총 데미지 감소율 * ReduceDmg_f.Value1_f ) / ReduceDmg_f.Value2_f ) + ReduceDmg_f.Value3_f ) / 100
	// 만약 데미지 감소 배율 > ReduceC_f.Value1_f 일때, 데미지 감소 배율은 ReduceC_f.Value1_f 이다.
	// 2017.8.24. kyh
	public float CalcDamageDecreaseRate(float targetDamageDecreaseRate){

		float reduceDmg_val1 = _LowDataMgr.instance.GetFormula (FormulaType.REDUCE_DAMAGE, 1);
		float reduceDmg_val2 = _LowDataMgr.instance.GetFormula (FormulaType.REDUCE_DAMAGE, 2);
		float reduceDmg_val3 = _LowDataMgr.instance.GetFormula (FormulaType.REDUCE_DAMAGE, 3);
		float val = (((targetDamageDecreaseRate * reduceDmg_val1) / reduceDmg_val2 ) + reduceDmg_val3) / 100f;
		if (val > _LowDataMgr.instance.GetFormula (FormulaType.REDUCE_CAP, 1)) {
			val =  _LowDataMgr.instance.GetFormula (FormulaType.REDUCE_CAP, 1);
		}
		return val;

	}

	// 공격속도 증가 배율
	// ( ( 공격속도 * AttackSpeed_f.Value1_f ) + AttackSpeed_f.Value2_f ) / 100
	// 공격속도 배율 > AttackSC_f.Value1_f 일때, 공격속도 배율 = AttackSC_f.Value1_f
	// 2017.8.25 kyh
	public float CalcAttackSpeedIncreaseRate(float atkSpeed){
		float val = ((atkSpeed * _LowDataMgr.instance.GetFormula (FormulaType.ATTACK_SPEED, 1)) + _LowDataMgr.instance.GetFormula (FormulaType.ATTACK_SPEED, 2)) / 100f;

		if (val > _LowDataMgr.instance.GetFormula (FormulaType.ATTACK_SPEED_CAP, 1)) {
			val = _LowDataMgr.instance.GetFormula (FormulaType.ATTACK_SPEED_CAP, 1);
		}

		return val;
	}


	public float GetAttackSpeed(float atkSpeed){
		return atkSpeed * (1 + CalcAttackSpeedIncreaseRate(atkSpeed));
	}
}




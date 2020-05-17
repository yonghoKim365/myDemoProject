using UnityEngine;
using System.Collections;

static public class GK_DamageCalculator
{
    /// <summary>
    /// 공격자가 대상에 가할 데미지 계산
    /// </summary>
    /// <remarks>
    /// 대미지 공식 적용
    /// A. 기본 스탯(능력) + 아이템(절대치) + 아이템 옵션(절대치)
    /// B. {A} * ( 1+ (아이템% + 패시브% + 아이템 옵션%))
    /// C. 버프 적용 : {B} * (1 + (건물 버프 + 스킬 버프))
    /// D. 대상 속성 공격 적용 : {C} * ( 1 +( 속성 수치 )) 대상 속성 비교
    /// E. 액티브 적용 : {C or D} * ( 1 + (액티브 적용))
    /// G. 크리티컬 대미지 적용 : {C or D or E } * (1+ (크.확 - 크.확 감소 디버프) * (크.댐 - 크.댐 감소 디버프))
    /// H. 방어력 무시 % 확률 계산

    /// 방어력 공식 적용
    /// A. 기본 스탯(능력) + 아이템(절대치) + 아이템 옵션(절대치)  
    /// B. {A} * (1+ (아이템% + 패시브% + 아이템 옵션%))
    /// C. 버프 적용 : {B} * (1+ (건물 버프% + 스킬 버프%))
    /// D. 총 방어력 / ((총방어력+Defense_DefendVariable)+((레벨-1)*Defense_LevelVariable))
    /// </remarks>
    /// <param name="attacker">공격자</param>
    /// <param name="target">대상</param>
    /// <param name="damage">초기정해진 데미지</param>
    /// <param name="atkType">공격종류</param>
    /// <returns>대상에게 적용되어야할 실 데미지</returns>
    static public int CalcDamage(Unit attacker, Unit target, float damage, float AddDamage, bool isCritical, eAttackType atkType)
    { 
		//GameCharacterInfo CharInfo = target.CharInfo;

		// ======== 공격력 계수가 적용된 초기 데미지 ========
		float calcDamage = damage;

		/*
        //공식
        //=========================================================================================
        // 치명타 일경우 = 방어력 무시+[(공격력-데미지 감소)-((공격력-데미지 감소)*대미지 감소율*0.01*%)]*(2+치명타 대미지*0.01*%)
        // 치명타가 아닐경우 = 방어력 무시+[(공격력-데미지 감소)-((공격력-데미지 감소)*대미지 감소율*0.01*%)]
        //=========================================================================================
        // *방어력 무시 공격력
        float DefIgnoreDam = attacker.CharInfo.DefIgnoreAtk;
	    // **공격력 = 데미지 감소 수치가 적용된 공격력
	    float CalcDamageDec = (calcDamage - CharInfo.Def);
	    // ***감소될 공격력 = 데미지 감소율에 의해 나온 수치
	    float CalcDamageDecRate = CalcDamageDec * CharInfo.DefRate *0.01f;
	    // 최종 공격력 = (*방무공) + (**공격력) + (***감소될공격력)
        float totalDam = DefIgnoreDam + (CalcDamageDec - CalcDamageDecRate);

        if( isCritical )
        {
            totalDam = totalDam * ( 2 + attacker.CharInfo.CriticalDmgRate * 0.01f);
        }
        calcDamage = Mathf.Clamp(totalDam, 1, calcDamage);
        return (int)calcDamage;
        */

		float targetDefRate = target.CharInfo.DefRate * 0.001f;

		if (targetDefRate > 1) {
			targetDefRate = 1f;
		}

		if (isCritical) {
			//크리시
			if (damage - target.CharInfo.Def > 0) {
				float totalDam = (attacker.CharInfo.DefIgnoreAtk + AddDamage + (damage - target.CharInfo.Def) - ((damage - target.CharInfo.Def) * targetDefRate));
				totalDam = totalDam * (2 + attacker.CharInfo.CriticalDmgRate * 0.001f);
				calcDamage = Mathf.Clamp (totalDam, 0, totalDam);
				return (int)calcDamage;
			} else {
				float totalDam = (attacker.CharInfo.DefIgnoreAtk + AddDamage + 0);
				totalDam = totalDam * (2 + attacker.CharInfo.CriticalDmgRate * 0.001f);
				calcDamage = Mathf.Clamp (totalDam, 0, totalDam);
				return (int)calcDamage;
			}            
		} else {
			//크리가 아닐시
			if (damage - target.CharInfo.Def > 0) {
				float totalDam = attacker.CharInfo.DefIgnoreAtk + AddDamage + (damage - target.CharInfo.Def) - ((damage - target.CharInfo.Def) * targetDefRate);
				calcDamage = Mathf.Clamp (totalDam, 0, totalDam);
				return (int)calcDamage;
			} else {
				float totalDam = attacker.CharInfo.DefIgnoreAtk + AddDamage + 0;
				calcDamage = Mathf.Clamp (totalDam, 0, totalDam);
				return (int)calcDamage;
			}
		}        
    }

	/// <summary>
	/// damage를 계산한다.
	/// </summary>
	/// <returns>The damage new.</returns>
	/// <param name="attacker">공격자.</param>
	/// <param name="target">Target.</param>
	/// <param name="damage">스킬계수 * 공격자데미지를 연산한 값.</param>
	/// <param name="AddDamage">스킬 부가 데미지</param>
	/// <param name="isCritical">If set to <c>true</c> is critical.</param>
	/// <param name="targetDefRate">피격자 데미지 감소 비율</param>
	/// <param name="criticalRate">크리티컬 피해량 백분율</param>
	static public int CalcDamageNew(Unit attacker, Unit target, float damage, float AddDamage, bool isCritical, float targetDefRate, float criticalRate)
	{ 

		// new 
		// new - 아직 설명만
		//1、회피 발생의 경우, 최종 데미지는  0
		//2、회피 미발생의 경우, 크리티컬 확률에 따라 크리티컬 발생 여부 체크
		//  2.1 크리 발생의 경우，
		//		최종 데미지 = (방어력 무시 + 스킬 부가 데미지 + (스킬계수 * 공격자 데미지 – 피격자 데미지 감소) - ((스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) X 피격자 데미지 감소 비율)) X ( 2 + 크리티컬 피해량 백분율) * (1 + 데미지 변량 백분율) ,
		//      이중 만약 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) < 0 라면 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) = 0   
		//		만약 최종 데미지가 < 0 이라면, 최종 데미지 = 0
		//  2.2 크리 미발생의 경우, 
		//		최종 데미지 = (방어력 무시 +스킬 부가 데미지 + (스킬계수 * 공격자 데미지 – 피격자 데미지 감소) - ((스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) X 피격자 데미지 감소 비율)) * (1 + 데미지 변량 백분율), 
		//		이중 만약 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) < 0 라면 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) = 0
		// 		만약 최종 데미지가 < 0 이라면, 최종 데미지 = 0 
		// 스킬 계수는 스킬의 레벨에 따라skill테이블의 skilllevel테이블의 factorRate_f수치를 읽어 옴
		// 스킬 부가 데미지는 스킬 레벨에 따라 skilllevel테이블의 ignorEef_f수치를 읽어 옴"

		// 이중 만약 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) < 0 라면 (스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) = 0   
		// damage = 스킬계수 * 공격자데미지를 이미 연산한 값.

		// 방어력 무시 							: attacker.CharInfo.DefIgnoreAtk
		// 스킬 부가 데미지 						: AddDamage
		// 스킬계수 * 공격자 데미지 – 피격자 데미지 감소 	: calcDamage
		// 피격자 데미지 감소 						: target.CharInfo.Def
		// 피격자 데미지 감소 비율 					: targetDefRate
		// 크리티컬 피해량 백분율 					: criticalRate
		// 데미지 변량 백분율 						: damageChangeRate

		float calcDamage = damage - target.CharInfo.Def; // 스킬계수 * 공격자 데미지 – 피격자 데미지 감소
		if (calcDamage < 0)
			calcDamage = 0;
		
		float totDam = 0f;
		if (isCritical) {
		// 최종 데미지 = (방어력 무시 + 스킬 부가 데미지 + (스킬계수 * 공격자 데미지 – 피격자 데미지 감소) - ((스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) X 피격자 데미지 감소 비율)) X ( 2 + 크리티컬 피해량 백분율)
			totDam = (attacker.CharInfo.DefIgnoreAtk + AddDamage + calcDamage - (calcDamage * targetDefRate)) * ( 2 + criticalRate);
		} else {
		// 최종 데미지 = (방어력 무시 + 스킬 부가 데미지 + (스킬계수 * 공격자 데미지 – 피격자 데미지 감소) - ((스킬계수 * 공격자 데미지 – 피격자 데미지 감소 ) X 피격자 데미지 감소 비율))
			totDam = attacker.CharInfo.DefIgnoreAtk + AddDamage + calcDamage - (calcDamage * targetDefRate);
		}

		// 데미지 변량  // between -5% ~ 5%
		totDam = Random.Range ((int)(totDam * 0.95f), (int)(totDam * 1.05f)); 

		if (totDam < 0)
			totDam = 0;
		
		return (int)totDam;
		
	}
}
using UnityEngine;
using System.Collections;

/// <summary>
/// 유닛들 관련 계산기
/// </summary>
public class UnitCalculator
{
    /// <summary>
    /// 지역에 따른 공격력 상승률 계산
    /// </summary>
    public static float CalcAddingAtkRateFromLand(LandType landType, GameCharacterInfo charInfo)
    {
        float addingRate = 0f;
        //switch (landType)
        //{
        //    case LandType.PREAH:
        //        addingRate = charInfo[AbilityType.IncreasePreaAtkDmg].FinalValue;                
        //        break;
        //    case LandType.LATINA:
        //        addingRate = charInfo[AbilityType.IncreaseLatinaAtkDmg].FinalValue;
        //        break;
        //    case LandType.AZEN:
        //        addingRate = charInfo[AbilityType.IncreaseAzenAtkDmg].FinalValue;
        //        break;
        //    case LandType.AKRA:
        //        addingRate = charInfo[AbilityType.IncreaseAkraAtkDmg].FinalValue;
        //        break;
        //}

        return addingRate;
    }

    /// <summary>
    /// 지역에 따른 방어력 상승률 계산
    /// </summary>
    public static float CalcAddingDefRateFromLand(LandType landType, GameCharacterInfo charInfo)
    {
        float addingRate = 0f;
        //switch (landType)
        //{
        //    case LandType.PREAH:
        //        addingRate = charInfo[AbilityType.IncreasePreaDefence].FinalValue;                
        //        break;
        //    case LandType.LATINA:
        //        addingRate = charInfo[AbilityType.IncreaseLatinaDefence].FinalValue;
        //        break;
        //    case LandType.AZEN:
        //        addingRate = charInfo[AbilityType.IncreaseAzenDefence].FinalValue;
        //        break;
        //    case LandType.AKRA:
        //        addingRate = charInfo[AbilityType.IncreaseAkraDefence].FinalValue;
        //        break;
        //}

        return addingRate;
    }

    /// <summary>
    /// 보스에 대한 공격력 상승률 계산
    /// </summary>
    public static float CalcAtkRateTowardBoss(Unit attacker, Unit target)
    {
        float addRate = 0f;
        //if (target.UnitType == UnitType.Boss)
        //    addRate = attacker.CharInfo[AbilityType.DAMAGEDamagToBoss].FinalValue;
        return addRate;
    }

    /// <summary>
    /// 보스에 대한 방어력 상승률 계산
    /// </summary>
    public static float CalcDefRateFromBoss(Unit attacker, Unit target)
    {
        float addRate = 0f;
        //if (attacker.UnitType == UnitType.Boss)
        //    addRate = target.CharInfo[AbilityType.DAMAGE_DECREASERateOfBoss].FinalValue;
        return addRate;
    }
}

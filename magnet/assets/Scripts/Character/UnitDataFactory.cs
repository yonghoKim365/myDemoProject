using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// InGame에서 Unit에 사용할 애니메이션,Skill 리스트를 구성해주는 클래스
/// </summary>
public static class UnitDataFactory
{
    
    /*
    public static void SetupDefaultAnim(GameCharacterInfo charInfo)
    {
        Unit ownerUnit = charInfo.Owner;

        switch (ownerUnit.UnitType)
        {
            case UnitType.Unit:
                {
                    Pc pc = ownerUnit as Pc;
                    SetupDefaultAnimForUnit(pc.syncData.LowID, pc.syncData.Grade, pc.syncData.AwakenType, out charInfo.animDatas);
                }
                break;

            case UnitType.Npc:
            case UnitType.Boss:
                {
                    Npc npc = ownerUnit as Npc;
                    SetupDefaultAnimForNpc((uint)npc.NpcLowID, out charInfo.animDatas);
                }
                break;

            case UnitType.Prop:
                {
                    Prop prop = ownerUnit as Prop;
                    SetupDefaultAnimForNpc((uint)prop.NpcLowID, out charInfo.animDatas);
                }
                break;

            case UnitType.Trap:
                {
                }
                break;
        }
    }

    public static void SetupDefaultAnimForUnit(uint unitId, byte grade, uint awakenPhase, out _ResourceLowData.AniTableInfo[] animDatas)
    {
        UnitLowData.GradeInfo gradeInfo = LowDataMgr.GetUnitGradeData( unitId, grade, awakenPhase );
        SetupDefaultAnim(  gradeInfo.resource, out animDatas );
        //animDatas = _LowDataMgr.instance.GetUnitAniInfo((ushort)gradeInfo.resource);
    }

    public static void SetupDefaultAnimForNpc(uint unitId, out _ResourceLowData.AniTableInfo[] animDatas)
    { 
        UnitLowData.EnemyInfo enemyInfo = LowDataMgr.GetUnitOfEnemyData( unitId );
        SetupDefaultAnim(  enemyInfo.resource, out animDatas );
        //animDatas = _LowDataMgr.instance.GetUnitAniInfo((ushort)enemyInfo.resource);
    }

    */
    public static int CountingMaxAnimCombo(Resource.AniInfo[] animDatas)
    {
        int maxAnimCombo = 0;
        maxAnimCombo += null == animDatas[(int)eAnimName.Anim_attack1] || "0" == animDatas[(int)eAnimName.Anim_attack1].aniName ? 0 : 1;
        maxAnimCombo += null == animDatas[(int)eAnimName.Anim_attack2] || "0" == animDatas[(int)eAnimName.Anim_attack2].aniName ? 0 : 1;
        maxAnimCombo += null == animDatas[(int)eAnimName.Anim_attack3] || "0" == animDatas[(int)eAnimName.Anim_attack3].aniName ? 0 : 1;
        maxAnimCombo += null == animDatas[(int)eAnimName.Anim_attack4] || "0" == animDatas[(int)eAnimName.Anim_attack4].aniName ? 0 : 1;
        
        return maxAnimCombo;
    }
}
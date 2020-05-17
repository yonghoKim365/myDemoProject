using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ForwardTriggerEvent : MonoBehaviour {

    public class StayUnitData
    {
        public Unit unit;
        public float CallTime;
    }

    Unit Caster;
    public int Idx;

    public List<AbilityData> SkillAbilityLists = new List<AbilityData>();
    int TargetLimitCount = 100;

    public System.Action<Collider, ForwardTriggerEvent> TriggerEnter;
    public System.Action<Unit, ForwardTriggerEvent> TriggerEnter_Unit;
    public System.Action<Collider, ForwardTriggerEvent> TriggerStay;
    public System.Action<Unit, ForwardTriggerEvent> TriggerStay_Unit;
    public System.Action<Collider, ForwardTriggerEvent> TriggerExit;

    public void Setup(Unit _Caster, AbilityData _ability)
    {
        Caster = _Caster;
        TargetLimitCount = _ability.targetCount;
        SkillAbilityLists.Add(_ability);
    }

    public void Setup(Unit _Caster, List<AbilityData> _abilitys)
    {
        Caster = _Caster;
        TargetLimitCount = _abilitys[0].targetCount;
        SkillAbilityLists = _abilitys;
    }

    public void Setup(Unit _Caster)
    {
        Caster = _Caster;
    }

	void OnTriggerEnter(Collider other)
    {
        if (TargetLimitCount <= 0)
            return;

        if (other.GetType().ToString().Equals("UnityEngine.CapsuleCollider"))
        {
            return;
        }        

        Unit target = other.GetComponent<Unit>();
        if(target == null)
            return;

        //< 정상이 아니라면 패스
        if (!target.Usable)
            return;

        //< 대상에 따라 조건을 검사한다.
        if (SkillAbilityLists.Count > 0)
        {
            if(SceneManager.instance.IsRTNetwork)
            {
                if(G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
                {
                    if ((SkillAbilityLists[0].applyTarget == 1 || SkillAbilityLists[0].applyTarget == 2) && Caster.m_rUUID == target.m_rUUID)
                        return;

                    if (SkillAbilityLists[0].applyTarget == 3 && Caster.TeamID != target.TeamID)
                        return;
                }
                else if (G_GameInfo.GameMode == GAME_MODE.ARENA)
                {
                    if ((SkillAbilityLists[0].applyTarget == 1 || SkillAbilityLists[0].applyTarget == 2) && Caster.TeamID == target.TeamID)
                        return;

                    if ((SkillAbilityLists[0].applyTarget == 1 || SkillAbilityLists[0].applyTarget == 2) && Caster.m_rUUID == target.m_rUUID)
                        return;

                    if (SkillAbilityLists[0].applyTarget == 3 && Caster.TeamID != target.TeamID)
                        return;
                }
                
                
            }
            else
            {
                if ((SkillAbilityLists[0].applyTarget == 1 || SkillAbilityLists[0].applyTarget == 2) && Caster.TeamID == target.TeamID)
                    return;

                if (SkillAbilityLists[0].applyTarget == 3 && Caster.TeamID != target.TeamID)
                    return;
            }            
        }

        TargetLimitCount--;

        if (null != TriggerEnter)
            TriggerEnter( other , this);

        if (TriggerEnter_Unit != null)
            TriggerEnter_Unit(target, this);
    }

    Dictionary<Collider, StayUnitData> StayUnitDic;
    public float StayDelay = 0;
    void OnTriggerStay(Collider other)
    {
        if (null != TriggerStay)
            TriggerStay(other, this);

        if (TriggerStay_Unit == null)
            return;

        if (StayUnitDic == null)
            StayUnitDic = new Dictionary<Collider, StayUnitData>();

        //< 대상에 들어있다는것은 유닛이라는것이므로 시간을 체크해줌
        if(StayUnitDic.ContainsKey(other))
        {
            if (StayUnitDic[other].CallTime > Time.time)
                return;
        }
        else
        {
            Unit target = other.GetComponent<Unit>();
            if (target == null)
                return;

            //< 정상이 아니라면 패스
            if (!target.Usable)
                return;

            StayUnitData nData = new StayUnitData();
            nData.unit = target;
            StayUnitDic.Add(other, nData);
        }

        StayUnitDic[other].CallTime = Time.time + StayDelay;
        TriggerStay_Unit(StayUnitDic[other].unit, this);
    }

    void OnTriggerExit(Collider other)
    {
        if (null != TriggerExit)
            TriggerExit(other, this);
    }
}

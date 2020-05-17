using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill_AI : MonoBehaviour {

    List<Skill> Heals = new List<Skill>();
    List<Skill> Buffs = new List<Skill>();
    List<Skill> Attacks = new List<Skill>();

    List<Skill> AllSkills = new List<Skill>();

    Unit parentUnit;

    public void Setup(Unit _parent)
    {
        parentUnit = _parent;

        //< 선별한다. - 평타는 스킬이아니다 그래서 1부터
        for(int i=1; i<_parent.SkillCtlr.SkillList.Length; i++)
        {
            if (_parent.SkillCtlr.SkillList[i] == null)
                continue;

            //9번스킬부터는 체인관련스킬들이다.무시
            if (i >= 5)
                continue;

            if (_parent.SkillCtlr.SkillList[i]._SkillType == eActiveSkillType.Heal)
                Heals.Add(_parent.SkillCtlr.SkillList[i]);
            else if (_parent.SkillCtlr.SkillList[i]._SkillType == eActiveSkillType.Buff)
                Buffs.Add(_parent.SkillCtlr.SkillList[i]);
            else
                Attacks.Add(_parent.SkillCtlr.SkillList[i]);

            //파트너의 궁극기는 패스하자 - 수동으로만 사용가능
            if (_parent.IsPartner && i == 4)
                continue;

            AllSkills.Add(_parent.SkillCtlr.SkillList[i]);
        }
    }

    public float UseSkillEndTime = 0;
    List<int> useableSkill = new List<int>();
    public bool SkillUpdate()
    {
        if (!ActiveCheck())
            return false;

        useableSkill.Clear();

        for (int i = 0; i < AllSkills.Count; i++)
        {
            if (AllSkills[i].IsPatten)//패턴의 경우 무시
                continue;

            //사용가능 
            if (AllSkills[i].GetCondition() == SkillActiveCondition.eAvailable)
            {
                useableSkill.Add(i);
                continue;
            }

            if (AllSkills[i]._SkillType == eActiveSkillType.Attack)
            {
                //공격스킬일경우 타겟이 멀리에 있는지까지체크
                if(AllSkills[i].GetCondition() == SkillActiveCondition.eFarFromTarget)
                {
                    useableSkill.Add(i);
                    continue;
                }
            }
        }

        //사용가능한게 없다
        if (useableSkill.Count == 0)
            return false;

        int RandomIndex = useableSkill[Random.Range(0, useableSkill.Count)];

        if (AllSkills[RandomIndex] == null)
        {
            Debug.LogError("들어오면 안됨");
            return false;
        }

        bool useCheck = false;
        float GCD = 4;// _LowDataMgr.G_AutoSkillDelay();

        if (AllSkills[RandomIndex]._SkillType == eActiveSkillType.Attack)
        {
            SkillActiveCondition condition = AllSkills[RandomIndex].GetCondition();
            if( condition == SkillActiveCondition.eAvailable || condition == SkillActiveCondition.eFarFromTarget) 
            {
                //그냥사용
                if (parentUnit.UseSkill(AllSkills[RandomIndex].slot) && parentUnit.CurrentState == UnitState.Skill)
                {
                    //GCD = AllSkills[RandomIndex].GetSkillActionInfo().GlobalCooltime;
                    useCheck = true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if(AllSkills[RandomIndex]._SkillType == eActiveSkillType.Buff)
        {
            if (AllSkills[RandomIndex].GetCondition() == SkillActiveCondition.eAvailable)
            {
                //< 공격할 대상이 화면안에 들어올때 버프를 사용함 - 버프가 왜??? 이건 버프 = 디버프라는 구조에서나 이렇게
                //if (parentUnit.GetTarget() == null || (parentUnit.transform.position - parentUnit.GetTarget().transform.position).magnitude > 20)
                //    continue;

                //< 스킬 시전!
                if (parentUnit.UseSkill(AllSkills[RandomIndex].slot) && parentUnit.CurrentState == UnitState.Skill )
                {
                    useCheck = true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (AllSkills[RandomIndex]._SkillType == eActiveSkillType.Heal)
        {
            //힐은 일단 없다긴 하지만
            if (AllSkills[RandomIndex].GetCondition() == SkillActiveCondition.eAvailable)
            {
                //< 스킬 시전!
                if (parentUnit.UseSkill(AllSkills[RandomIndex].slot) && parentUnit.CurrentState == UnitState.Skill)
                {
                    useCheck = true;
                }
                else
                {
                    return false;
                }
            }
        }

        if (useCheck)
            UseSkillEndTime = Time.time + GCD;

        return useCheck;
    }
    
    //public bool AttackUpdate()
    //{
    //    if (!ActiveCheck())
    //        return false;

    //    //< 현재 바라보는 대상이 프롭이면 패스
    //    if (parentUnit.GetTarget() == null || parentUnit.GetTarget().UnitType == UnitType.Prop)
    //        return false;

    //    bool useCheck = false;
    //    float GCD = _LowDataMgr.G_AutoSkillDelay();

    //    for (int i = 0; i < Attacks.Count; i++)
    //    {
    //        if (Attacks[i].GetCondition() == SkillActiveCondition.eAvailable)
    //        {
    //            //< 스킬을 시전
    //            if (parentUnit.UseSkill(Attacks[i].slot) && parentUnit.CurrentState == UnitState.Skill )
    //            {
    //                GCD = Attacks[i].GetSkillActionInfo().cooltime;
    //                useCheck = true;
    //                break;
    //            }
    //            else
    //                continue;
    //        }
    //    }

    //    if (useCheck)
    //        UseSkillEndTime = Time.time + GCD;

    //    return useCheck;
    //}

    //public bool HealUpdate()
    //{
    //    //< 힐은 바로 사용하도록 시간체크안함
    //    if (!ActiveCheck(true))
    //        return false;

    //    if (Heals.Count == 0)
    //        return false;

    //    //< 체력이 60%이하일때만 시전
    //    if (((float)parentUnit.CharInfo.Hp / (float)parentUnit.CharInfo.MaxHp) > 0.6f)
    //        return false;

    //    float GCD = _LowDataMgr.G_AutoSkillDelay();

    //    bool useCheck = false;
    //    for (int i = 0; i < Heals.Count; i++ )
    //    {
    //        if (Heals[i].GetCondition() == SkillActiveCondition.eAvailable)
    //        {
    //            //< 스킬 시전!
    //            if (parentUnit.UseSkill(Heals[i].slot) && parentUnit.CurrentState == UnitState.Skill)
    //            {
    //                GCD = Attacks[i].GetSkillActionInfo().cooltime;
    //                useCheck = true;
    //                break;
    //            }
    //            else
    //                continue;
    //        }
    //    }

    //    if (useCheck)
    //        UseSkillEndTime = Time.time + GCD;

    //    return false;
    //}

    //public bool BuffUpdate()
    //{
    //    if (!ActiveCheck())
    //        return false;

    //    if (Buffs.Count == 0)
    //        return false;

    //    //< 사용할수있는 버프가 있는지 체크
    //    bool useCheck = false;

    //    float GCD = _LowDataMgr.G_AutoSkillDelay();

    //    for (int i = 0; i < Buffs.Count; i++ )
    //    {
    //        if (Buffs[i].GetCondition() == SkillActiveCondition.eAvailable)
    //        {
    //            //< 공격할 대상이 화면안에 들어올때 버프를 사용함 - 버프가 왜??? 이건 버프 = 디버프라는 구조에서나 이렇게
    //            //if (parentUnit.GetTarget() == null || (parentUnit.transform.position - parentUnit.GetTarget().transform.position).magnitude > 20)
    //            //    continue;

    //            //< 스킬 시전!
    //            if (parentUnit.UseSkill(Buffs[i].slot) && parentUnit.CurrentState == UnitState.Skill)
    //            {
    //                GCD = Attacks[i].GetSkillActionInfo().cooltime;
    //                useCheck = true;
    //                break;
    //            }
    //            else
    //                continue;
    //        }
    //    }

    //    if (useCheck)
    //        UseSkillEndTime = Time.time + GCD;

    //    return false;
    //}

    //< 스킬을 사용할수있는 대상인지 검사
    bool ActiveCheck(bool NotTimeCheck = false)
    {
        //< 시간제한이 걸려있을시 패스
        if (!NotTimeCheck && UseSkillEndTime > Time.time)
            return false;

        //< 플레이어인데, 현재 카메라 이벤트 중이라면 패스
        if (parentUnit.UnitType == global::UnitType.Unit && parentUnit.isLeader && SkillEventMgr.EventUpdate)
            return false;

        //< 이미 스킬상태라면 패스
        if (parentUnit.CurrentState == UnitState.Skill)
            return false;

        /*
        if (G_GameInfo.GameInfo.AutoMode || parentUnit.IsHelper || parentUnit.IsPartner || parentUnit.UnitType != UnitType.Unit)
        {
            return true;
        }

        return false;
        */
        return true;
    }

    //< 스킬이 끝났을시 호출
    public void EndSkill()
    {
        //< 연속으로 사용을 안하기위해 후딜을 줌
        if( !(parentUnit is Pc) )
            UseSkillEndTime = Time.time + Random.Range(1, 4);
    }
}

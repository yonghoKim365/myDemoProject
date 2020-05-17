using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SkillActiveCondition 
{
    eNone,          //없는 스킬이잖아!!
    eActive,        //활성화 중이다
    eCoolTime,      //쿨타임 걸려 있다
    eNeedTarget,    //타겟이 필요 하다.
    eFarFromTarget, //타겟에서 멀다
    eAvailable,     //사용 가능하다.
}

public enum eActiveSkillType
{
    Heal,
    Buff,
    Attack,
    None,
}
 
public class Skill
{
    public bool Usable { get; set;}

    public int slot;
    protected uint _skillIndex;
    public bool PassiveSkill = false;

    public bool IsMainSkill;

    protected Unit Caster;
    public bool bActive;
    public bool IsPatten = false;
    float coolTime = 0;

    //SecurityValue<int> NowCoolTime = new SecurityValue<int>(0);
    public float CoolTime
    {
        get
        {
            //NowCoolTime.val = (int)coolTime;
            return coolTime;//NowCoolTime.val;
        }
        set
        {
            coolTime = value;
        }
    }

    protected SkillController SkillCtr;

    public eActiveSkillType _SkillType = eActiveSkillType.Attack;

    //< 해당 스킬의 정보를 리턴해준다.
    /*
    public SkillTablesLowData.ActionInfo GetSkillActionInfo()
    {
        return SkillCtr._SkillGroupInfo.GetActionData((uint)(slot+1));
    }
    */

    public uint GetSkillIndex()
    {
        return _skillIndex;
    }

    public ActionInfo GetSkillActionInfo()
    {
        return SkillCtr.__SkillGroupInfo.GetAction((uint)_skillIndex);
    }

    // 컨트롤러에서 오는 초기화 작업
    byte needtarget;
    public ushort range;
    public float OrgCooltime;
    public virtual void Init(int _slot, uint skillIndex, Unit owner, SkillController _parent, bool _normal = false)
    {
        slot = _slot;
        Caster = owner;
        SkillCtr = _parent;
        _skillIndex = skillIndex;

        //체인스킬도 일단 무시
        if (slot == 0 || slot == 5 || slot == 6 || slot == 7)
            IsMainSkill = true;

        //< 처음엔 모두 사용할수 있도록
        coolTime = 0;

        //< 자주사용하는값들은 미리 저장
        ActionInfo actionInfo = GetSkillActionInfo();
        if (actionInfo == null)
        {
            Debug.LogError(string.Format("Action Info null error skillIndex = {0}, npcName={1}", _skillIndex, owner.gameObject.name));
            return;
        }

        needtarget = actionInfo.needtarget;
        range = (ushort)(actionInfo.range * 1);
        OrgCooltime = actionInfo.cooltime;

        //일단 일반스킬만 살려둠
        _SkillType = eActiveSkillType.Attack;
        /*
        if (ActionInfo.skilltype == 1)
            _SkillType = eActiveSkillType.Attack;
        else if (ActionInfo.skilltype == 2)
                _SkillType = eActiveSkillType.Buff;
        else if (ActionInfo.skilltype == 3)
            _SkillType = eActiveSkillType.Heal;
        */
    }

    /// <summary> 보스 레이드 스킬 </summary>
    public virtual void RaidSkillInit(int _slot, uint skillIndex, Unit owner, SkillController _parent, bool _normal = false)
    {
        slot = _slot;
        Caster = owner;
        SkillCtr = _parent;
        _skillIndex = skillIndex;

        IsPatten = true;

        if (slot == 0)
            IsMainSkill = true;

        //< 처음엔 모두 사용할수 있도록
        coolTime = 0;

        //< 자주사용하는값들은 미리 저장
        needtarget = GetSkillActionInfo().needtarget;
        range = (ushort)(GetSkillActionInfo().range * 1);
        OrgCooltime = GetSkillActionInfo().cooltime;

        //< 스킬 타입을 설정한다
        ActionInfo ActionInfo = GetSkillActionInfo();
        if (ActionInfo == null)
            return;

        //일단 일반스킬만 살려둠
        _SkillType = eActiveSkillType.Attack;
        /*
        if (ActionInfo.skilltype == 1)
            _SkillType = eActiveSkillType.Attack;
        else if (ActionInfo.skilltype == 2)
                _SkillType = eActiveSkillType.Buff;
        else if (ActionInfo.skilltype == 3)
            _SkillType = eActiveSkillType.Heal;
        */
    }

    //< 스킬을 사용할수있는 상태로 변경(게임 시작시 한번 호출)
    public void SetSkillActive()
    {
        coolTime = 0;
        bActive = false;
    }

    public void SkillUpdate()
    {
        coolTime -= Time.deltaTime;
        if (coolTime < 0)
            coolTime = 0;
    }

    //< 쿨타임 체크 :: -1-불가, 0-가능, 기타 - 남은 쿨타임
    public int IsUseAbleSkill()
    {
        //if (bActive)
        //    return -1;

        return (int)coolTime;
    }

    //< 타겟이 필요한 스킬인지 검사
    public bool NeedTarget(bool AutoModeforceUserUse = false)
    {
        //< 마을일때에는 시뮬레이션이므로 무조건 타겟 있을때만 하도록
        if (TownState.TownActive && _SkillType == eActiveSkillType.Attack)
            return true;

        //< 자동일경우에는 무조건 대상이 있을때만 사용하도록 처리
        if (AutoModeforceUserUse)
            return false;

        if (G_GameInfo.GameInfo.AutoMode && _SkillType == eActiveSkillType.Attack)
            return true;
        else
            return needtarget == 1 ? true : false;
    }

    //< 해당 스킬의 사용이 가능한지 검사한다.
    public SkillActiveCondition GetCondition(bool AutoModeforceUserUse = false)
    {
        //사용중이다
        if (bActive)
            return SkillActiveCondition.eActive;

        //< 쿨타임 걸려 있다
        if (coolTime > 0)
            return SkillActiveCondition.eCoolTime;
        
        //< 무조건 대상에게 가서 스킬을 쓰도록
        if (NeedTarget(AutoModeforceUserUse))
        {
            Unit newTarget = Caster.GetTarget();

            //타겟이 필요하다
            if (newTarget == null)
                return SkillActiveCondition.eNeedTarget;

            //타겟 상태가 유효하지 않다( 타겟이 죽거나 죽는중이다 )
            if (newTarget != null && !newTarget.Usable)
                return SkillActiveCondition.eNeedTarget;

            //사거리가 멀다
            if ((newTarget.transform.position - Caster.transform.position).magnitude > range)
                return SkillActiveCondition.eFarFromTarget;
        }

        return SkillActiveCondition.eAvailable;
    }


    /// 스킬을 활성화 상태로 만든다.
    public void ActiveSkill()
    {
        //coolTime = SkillCtr._SkillGroupInfo.GetActionData((uint)slot+1, (Caster is Pc) ? (Caster as Pc).syncData.SkillLvDatas : null).cooltime;
        //coolTime = SkillCtr._SkillGroupInfo.GetActionData((uint)slot + 1, (Caster is Pc) ? (Caster as Pc).syncData.SkillLvDatas : null).cooltime;
        //coolTime = SkillCtr.__SkillGroupInfo.GetAction((uint)slot).cooltime;

        //coolTime = GetSkillActionInfo().GlobalCooltime;
        //쿨타임 감소 적용
        //coolTime = GetSkillActionInfo().cooltime * (1 - Caster.CharInfo.CooltimeReduce);

		coolTime = NetData.instance.CalcSkillCoolTime(GetSkillActionInfo ().cooltime, Caster.CharInfo.CooltimeReduce);

        bActive = true;

        if (IsMainSkill)//평타 무시.
            return;
        
        if( Caster.UnitType == UnitType.Unit)//플레어, 파트너 일 경우에만!
        {
            if ( !Caster.IsPartner && NetData.instance._userInfo._charUUID != Caster.m_rUUID)
                return;

            if (!SceneManager.instance.IsRTNetwork && InGameHUDPanel.ZeroSkillCoolTime)
            {
                coolTime = (Caster.Animator.GetAnimLength(Caster.GetAniData(eAnimName.Anim_skill1 + (slot - 1)).aniName) * (1 - Caster.CharInfo.CooltimeReduce));
                bActive = false;
            }

            if (!Caster.IsPartner || (Caster.IsPartner && slot == 4))//파트너는 필살기만 해당함.
            {
                //UIBasePanel InGameHUDPanel = UIMgr.GetHUDBasePanel(); //GetUIBasePanel("UIPanel/InGameHUDPanel");
                if (G_GameInfo.GameInfo != null && G_GameInfo.GameInfo.HudPanel != null)
                    G_GameInfo.GameInfo.HudPanel.AddUseSkill(Caster.IsPartner, this);

                //스킬사용횟수 체크
                if (G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
                {
                    if (Caster.IsPartner)
                        (G_GameInfo.GameInfo)._AchieveFightData.parterSkillCnt++;
                    else
                        (G_GameInfo.GameInfo)._AchieveFightData.SkillCount++;
                }

            }
        }
    }

    public virtual void UseSkill(ushort abilityIndex, bool normal)
    {
        //< 내 스킬의 어빌리티 인지 검증(평타일경우 스킬인덱스는 0이므로, +1을 안시켜줌)
        //if (!normal)
        //    _slot = (uint)(slot + 1);

        //if (!SkillCtr._SkillGroupInfo.GetAbilityContainsKey(_slot, abilityIndex))
        //    return;

        //< 스킬 사용
        //List<SkillTablesLowData.AbilityInfo> skillList = null;

        //if (normal)
        //    skillList = SkillCtr._SkillGroupInfo.GetAbility(_slot, abilityIndex);
        //else

        /*
        if(Caster.UnitType  == UnitType.Unit)
        {
            byte temp = (Caster as Pc).syncData.SkillData[_slot]._SkillLevel;
            skillList = SkillCtr._SkillGroupInfo.GetAbility(_slot, abilityIndex, (uint)temp);
        }
        else
        {
            skillList = SkillCtr._SkillGroupInfo.GetAbility(_slot, abilityIndex, null );
        }
        */


        //skillList = SkillCtr._SkillGroupInfo.GetAbility(_slot, abilityIndex, Caster.UnitType == UnitType.Unit ? (Caster as Pc).syncData.SkillLvDatas : null);
        //skillList = SkillCtr._SkillGroupInfo.GetAbility(_slot, abilityIndex, Caster.UnitType == UnitType.Unit ? (Caster as Pc).syncData.SkillLvDatas : null);

        //< 플레이어에 리더라면 화면에 띄워줌
        //if (!normal && Caster.UnitType == UnitType.Unit && Caster.isLeader)
        //{
        //LocaleLowData.DataInfo localeName = LowDataMgr.GetLocale(GetSkillActionInfo().name);
        //LocaleLowData.DataInfo localedesc = LowDataMgr.GetLocale(skillList[0].skillDec);

        //if (localeName != null && localedesc != null)
        //{
        ////스킬사용시 해당 스킬의 설명과 아이콘에 관한 정보를 HUD로 전달
        ////EventListner.instance.TriggerEvent("HUD_MSG", string.Format("{0}\n{1}", localeName.title, localedesc.title));
        ////EventListner.instance.TriggerEvent("HUD_MSG_ICON", GetSkillActionInfo().icon);
        //}
        //}

        //for (int i = 0; i < skillList.Count; i++)
        //SkillAbility.ActiveSkill(SkillCtr.__SkillGroupInfo.AbilityDic[_slot][abilityIndex], Caster, true);

        AbilityData ability = SkillCtr.__SkillGroupInfo.GetAbility(GetSkillActionInfo().idx, (uint)abilityIndex);

        if (ability != null)
            SkillAbility.ActiveSkill(ability, Caster, normal);
        else
            Debug.Log(string.Format("AbilityNotFound:skillIdx:{0},AbilityIdx:{1}", GetSkillActionInfo().idx, abilityIndex));
    }

    public float IsSkillCoolTimePecent()
    {
        return ((coolTime) / (OrgCooltime));
    }

    /// 스킬 사용 상태가 끝날때 호출 된다.
    public void EndSkill()
    {
        bActive = false;
    }

    /// <summary> InGameHUDPanel에서 파트너의 필살기를 사용한 객체가 누구인지를 알기 위해 추가.</summary>
    public Unit GetCaster()
    {
        return Caster;
    }
}

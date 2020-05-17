using UnityEngine;
using System.Collections;

public class UnitStateBase : FSM.BaseState<Unit>
{
    protected bool startedPush = false;

    float nextSearchT = 0;

    void Update()
    {
        CachedUpdate();
    }

    public virtual void CachedUpdate()
    {
        if (parent.StopState)
            return;

        if (TownState.TownActive || G_GameInfo._GameInfo == null)
            return;

        GAME_MODE mode = G_GameInfo.GameMode;

        //< 오토가 아니라면 패스
        if (mode == GAME_MODE.SINGLE || mode == GAME_MODE.RAID || 
            mode == GAME_MODE.SPECIAL_EXP || mode == GAME_MODE.SPECIAL_GOLD ||
            mode == GAME_MODE.TOWER || mode == GAME_MODE.FREEFIGHT ||
            mode == GAME_MODE.TUTORIAL || mode == GAME_MODE.ARENA)
        {
            if (parent.TeamID == 0 && parent.IsLeader && !G_GameInfo.GameInfo.AutoMode)
                return;

        }

        if (G_GameInfo.GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        {
            if (parent.IsLeader == true || parent.UnitType == UnitType.Npc)  //parent.TeamID == (byte)eTeamType.Team2) //GAME_MODE.FREEFIGHT에서  parent.TeamID == (byte)eTeamType.Team2 NPC를 의미함
            {
                return;  //난투전 모드에서는 우선 새로운 타겟을 잡지 않는다.(안그러면 SearchTarget()에 의해 계속 AttackState 로 들어가려고 할 것이다.)
            }

            if (parent.IsPartner == true && parent.TeamID != G_GameInfo.PlayerController.Leader.TeamID)
                return;
        }
        
        SearchTarget(); //<-- Npc 는 주로 여기서 평타공격을 하고 있다. 
    }

    /// 인지범위내에 적이 있다면, 타겟을 자동으로 잡아준다.
    public Unit SearchTargetUnit;
    void SearchTarget()
    {
        if (Time.time < nextSearchT || parent.IsStun)
            return;
        
        //타겟이 없다면!! 새로운 타겟 설정

        if (parent.GetTarget() == null && parent.ForcedAttackTarget == null)
        {
            Unit newTarget = null;

            if(G_GameInfo.GameMode == GAME_MODE.ARENA)
            {
                if( parent.IsLeader )
                {
                    //1:1에서 내 캐릭이면 전체검색
                    newTarget = G_GameInfo.CharacterMgr.NearestTarget(parent.TeamID, (eAttackType)parent.AttackType, parent.cachedTransform.position, 1000, true);
                }
                else
                {
                    //아니면 - 내파트너
                    newTarget = G_GameInfo.CharacterMgr.NearestTarget(parent.TeamID, (eAttackType)parent.AttackType, parent.cachedTransform.position, parent.CharInfo.AtkRecognition, true);
                }
            }
            else if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)// 난투장에서는 캐릭의 공격범위 상관없이 전체 검색
                newTarget = G_GameInfo.CharacterMgr.NearestTarget(parent.TeamID, (eAttackType)parent.AttackType, parent.cachedTransform.position, 1000, true);
            //< 일반 모드에서는 사정거리에 따라서 대상을 검색함
            else
            {
                //< PC일경우에는 사정거리 크게 잡음
                if (parent.UnitType == UnitType.Unit)
                    newTarget = G_GameInfo.CharacterMgr.NearestTarget(parent.TeamID, (eAttackType)parent.AttackType, parent.cachedTransform.position, parent.CharInfo.AtkRecognition, true);
                else
                {
                    newTarget = G_GameInfo.CharacterMgr.FindTargetWithAggro(parent, parent.CharInfo.AtkRecognition, parent, true);

                    //Debug.LogWarning("2JW : newTarget - " + newTarget + " : parent - " + parent + " : " + parent.CharInfo + " : " + parent.CharInfo.AtkRecognition, parent);
                }
            }

            SearchTargetUnit = newTarget;
            if (null != newTarget)
                parent.AttackTarget(newTarget.GetInstanceID());
            else
                parent.SetTarget(GameDefine.unassignedID);
        }
        else
        {
            if (parent.ForcedAttackTarget != null)
                parent.AttackTarget(parent.ForcedAttackTarget.GetInstanceID());
            else
                parent.AttackTarget(parent.TargetID); //<---- 난투전에서 자꾸 이쪽으로 들어가려고 할 것이다.
        }

        nextSearchT = Time.time + GameDefine.SearchTargetCheckDelay;
    }

    /// <summary>
    /// 주어진 시간 동안 velocity 방향으로 pushPower 만큼 민다.
    /// </summary>
    public virtual void Push(eAnimName animName, Vector3 _velocity, float pushPower, float animTime, float stiffenTime, UnitState _next = UnitState.Idle)
    {
    }
}

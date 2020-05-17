using UnityEngine;
using System.Collections;

/// <summary>
/// 빠른 공격 사거리 까지 이동
/// 공격 사거리 이내는 빠른이동후 공격
/// 애니메이션 체크 해서 자동 콤보
/// </summary>
public class AttackState : UnitStateBase
{
    //float ComboKeepTime;
    //float nextMoveTime = 0f;
    float nextAttackTime = 0f;
    float atkAnimLength = 0f;
    //float fadeTime = 0.01f;
    //eAnimName attackAnim = eAnimName.Anim_attack1;

    Unit target = null;
    bool invalidTarget = true;
    
    bool goNear = false;
    //bool isDashAtk = false;
    //System.Action ExecuteDashFunc;  // 대시공격 수행 대리자

    NavMeshAgent navAgent;
    //ObstacleAvoidanceType saveAvoidType;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        //if (SceneManager.instance.IsRTNetwork && parent.UnitType == UnitType.Npc)
        //{
        //    parent.CurCombo = 0;                // Npc에서는 콤보가 없다고 했는데... 나중에 생기면 다시 수정해 주어야 한다.

        //    ExecuteDashFunc = CheckDashAttack;
        //    goNear = false;                     // goNear == true면 대상과 붙을 수 있는 최소한 까지 이동하기.

        //    // 유닛은 공격중에는 충돌 검사 되도록 해서, 유닛끼리 안겹쳐지도록 하자.
        //    if (navAgent == null)
        //        navAgent = parent.GetComponent<NavMeshAgent>();                                            //이건 필요할 듯

        //    return;
        //}

        /*
        if (Time.time > ComboKeepTime)
            parent.CurCombo = 0;
        */

        //ExecuteDashFunc = CheckDashAttack;

        //< 처음 들어오면 퍼스트어택체크
        //NewCheckTargetDelay = 0;
        goNear = false;

        // 유닛은 공격중에는 충돌 검사 되도록 해서, 유닛끼리 안겹쳐지도록 하자.
        if (navAgent == null)
            navAgent = parent.GetComponent<NavMeshAgent>();
        
        if (null != navAgent )//&& parent.UnitType == UnitType.Unit)
        {
            //saveAvoidType = navAgent.obstacleAvoidanceType;
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
        }
    }

    public override void OnExit(System.Action callback)
    {
        if (SceneManager.instance.IsRTNetwork)
        {
            parent.IsMovingLastLocation = true;
        }

        //< 스킬 이펙트 종료
        parent.SkillEffectClear(Time.time < atkAnimLength);

        //nextMoveTime = 0;
        atkAnimLength = nextAttackTime = 0;

        parent.ActiveAttack = false;

        /*
        //< 플레이어만 적용
        if (parent.UnitType == UnitType.Unit)
            ComboKeepTime = Time.time + 1;
        else
            ComboKeepTime = float.MaxValue;
            */

        base.OnExit(callback);
    }

    public override void CachedUpdate()
    {
        if (parent.StopState)
            return;
	    
        //if (parent.CheckAndFlying())
        //    return;

	    ////< 대쉬중이 아닐떄에만 타겟을 재갱신
	    //if (!isDashAtk)
            CheckTarget();  //<--- 현제 타겟이 공격 가능한지...?? 상태를 알아오는 함수

	   // 타겟이 없으면 Idle or PathMove
	    if (invalidTarget || target == null || target.CharInfo.IsDead)
        {
            // 공격애니메이션이 플레이 중이라면 애니메이션이 끝날 때까지 기다리도록 한다.
            if (Time.time > atkAnimLength)
                parent.ChangeState( UnitState.Idle );

            ////< 혹시나 대쉬상태였다면 패스
            //isDashAtk = false;

		    //Debug.LogWarning("2JW : AttackState 333 ");
		    return;
        }

       if (parent.CurrentState != UnitState.Attack)
           return;

        Vector3 targetDist = target.cachedTransform.position - parent.cachedTransform.position;

        //< 상대가 공중에 떠있을때에는 공중에 떠있는 높이값을 빼준다
        /*
        if(target.isAir)
        {
            Vector3 targetpos = target.cachedTransform.position;
            targetpos.y -= target.CharInfo.AirHeight;
            targetDist = targetpos - parent.cachedTransform.position;
        }
        */

        //공격 가능한 시간이다.
	    float atkRange = parent.CharInfo.AtkRange; // 첫 공격일때, 정해진 비율만큼 더 가까이 이동한 다음에 공격.
	    if (Time.time > nextAttackTime)
        {
            parent.ActiveAttack = false;

            // 공격 사거리내에 들어왔다면,
            bool canAttack = MathHelper.IsInRange(targetDist, atkRange, parent.Radius, target.Radius);
            
            if (canAttack)
            {
                if (parent.skill_AI.SkillUpdate())
                    return;

                parent.LookAt(target.transform.position);
                parent.ManualAttack();
            }
                
            else
            {
                goNear = true;
            }
        }
        else
        {
            //< 애니메이션이 끝났다면 아이들로 설정
            //if (!parent.Animator.Animation.isPlaying || atkAnimLength <= Time.time)
		    //{
			//    parent.PlayAnim(eAnimName.Anim_battle_idle, true, fadeTime);
            //    parent.UnitStop = true;
            //}
        }

        if (goNear)
        {
            // 대상과 붙을 수 있는 최소한 까지 이동하기.
            goNear = !MathHelper.IsInRange(targetDist, atkRange, parent.Radius, target.Radius);
            //goNear = targetDist.magnitude > (parent.Radius + target.Radius);

            //< 대쉬 체크
            //if (null != ExecuteDashFunc)
            //    ExecuteDashFunc();

            //if (isDashAtk)
            //{
            //    if (parent.CalculatePath(target.cachedTransform.position))
            //        parent.PlayAnim( eAnimName.Anim_dash, true, fadeTime );

            //    bool isEnd = parent.MoveToPath(3f);
            //    if (!isEnd)
            //        ExecuteDashFunc = CheckDashAttack;
            //}
            //else
            {
                /*
                if (!invalidTarget && target != null && Time.time > nextMoveTime)
                {
                    if (parent.CalculatePath(target.cachedTransform.position))
                    { 
                        parent.PlayAnim( eAnimName.Anim_move, true, fadeTime );
                        SetObstacleAvoidance( true );
                    }
                    nextMoveTime = Time.time + GameDefine.NextMoveDelta;
                }
                parent.MoveToPath( 1f );
                */
                //if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.transform.position) > 5)

                /*
                 * if (!invalidTarget && target != null && Time.time > nextMoveTime)
                {
                    parent.CalculatePath(target.cachedTransform.position);
                    //parent.ChangeState(UnitState.Move);
                    parent.MoveToPath(1f);
                    nextMoveTime = Time.time + GameDefine.NextMoveDelta;
                }*/
                //if (!invalidTarget && target != null && Time.time > nextMoveTime)
                {
                    bool success = parent.MovePosition(target.cachedTransform.position);
                    /*
                    if (success)
                    {
                        bool changeState = parent.ChangeState(UnitState.Move);
                        if (changeState)
                            (parent.FSM.Current() as MoveState).moveSpeedRatio = 1.0f;
                    }
                    */

                    //nextMoveTime = Time.time + GameDefine.NextMoveDelta;
                }                
            }
	   }
	   //Debug.LogWarning("2JW : AttackState 555 ");
    }


    //private void Attack()
    //{
    //    parent.SetObstacleAvoidance( true );
    //    parent.UnitStop = true;

    //    //< 타겟을 바라본다.
    //    parent.LookAt(target.cachedTransform.position);

    //    eAnimName aniName;        
    //    if(parent is Pc && !parent.IsPartner )
    //        aniName = (eAnimName)((int)eAnimName.Anim_attack1 + parent.CurCombo);
    //    else
    //        aniName = (eAnimName)((int)eAnimName.Anim_attack1);

    //    //나중에는 FreeFight 모드에서도 고려해야 할 것 같다. 여기서 한번 공격하고... 
    //    parent.CurCombo++;

    //    //< 실제 시간은 아래에서 대입(이펙트 호출할때 어셋번들로 인하여 딜레이가 생길수있기때문)
    //    atkAnimLength = Time.time + parent.Animator.GetAnimLength(aniName) - fadeTime;
    //    nextAttackTime = 90;

    //    int a_OldCombo = parent.CurCombo;

    //    if (SceneManager.instance.IsRTNetwork)
    //    {
    //        if (parent.IsLeader == true ) 
    //        {
    //            uint a_Combo = 10000;
    //            a_Combo = parent.GetNormalAttackComboSkillID(a_OldCombo);

    //            if (a_Combo == uint.MaxValue)
    //            {
    //                Debug.Log("NotFound NormalAttack ID");
    //                return;
    //            }

    //            iFunClient.instance.ReqSkillStart( parent.m_rUUID, a_Combo, parent.transform.position, parent.transform.eulerAngles.y); //--> 피격 대상인 놈들을 모두 찾는다.
    //        }
    //        else if( parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID)
    //        {
    //            uint a_Combo = parent.GetNormalAttackComboSkillID(0);

    //            iFunClient.instance.ReqSkillStart(parent.m_rUUID, a_Combo, parent.transform.position, parent.transform.eulerAngles.y); //--> 피격 대상인 놈들을 모두 찾는다.
    //        }
    //    }

    //    //< 이펙트 실행
    //    string StartEffect = "0";
    //    uint StartSoundID = 0;
    //    if (parent.GetAniData(aniName) != null)
    //    {
    //        StartEffect = parent.GetAniData(aniName).effect;
    //        StartSoundID = parent.GetAniData(aniName).seSkill;
    //    }

    // if ("0" != StartEffect && null != parent.Animation[parent.Animator.GetAnimName(aniName)])
    // {
    //        parent.SpawnSkillEffect(StartEffect, parent.Animation[parent.Animator.GetAnimName(aniName)].speed, parent.cachedTransform, parent.GetAniData(aniName).childEffect == 0 ? null : parent.transform, (skillCastingEff) =>
    //        {
    //            SoundManager.instance.PlaySfxSound(StartSoundID, parent.cachedTransform);
    //            //< 애니메이션 실행
    //            parent.PlayAnim(aniName, true, fadeTime);
    //            nextAttackTime = atkAnimLength;

    //            //황비홍 프로젝트 - 어택딜레이 유저는 없고 몬스터는 지정된 수치로 딜레이
    //            //if (parent.CurCombo == 0)
    //            {
    //                if (parent as Pc && !parent.IsPartner)
    //                    parent.CharInfo.AtkDelay = 0.0f;
    //                else
    //                    parent.CharInfo.AtkDelay = 0.5f;// _LowDataMgr.G_AttackDelay();

    //                nextAttackTime = atkAnimLength + parent.CharInfo.AtkDelay;
    //            }

    //            SkillTables.ActionInfo actionData = parent.SkillCtlr.__SkillGroupInfo.GetAction(parent.UseSkillIdx);

    //            if (actionData != null)
    //            {

    //                if (actionData.effectCallNotiIdx != 0)
    //                {
    //                    Collider[] existCol = skillCastingEff.GetComponentsInChildren<Collider>(true);
    //                    for (int j = 0; j < existCol.Length; j++)
    //                    {
    //                        //EffectCallAbility = true;

    //                        //< 있을경우는 삭제
    //                        ForwardTriggerEvent OrgtriggerEvt = existCol[j].gameObject.GetComponent<ForwardTriggerEvent>();
    //                        if (OrgtriggerEvt != null)
    //                            UIMgr.Destroy(OrgtriggerEvt);

    //                        existCol[j].isTrigger = false;
    //                        existCol[j].isTrigger = true;
    //                        ForwardTriggerEvent triggerEvt = existCol[j].gameObject.AddComponent<ForwardTriggerEvent>();
    //                        triggerEvt.TriggerEnter_Unit = FireToTarget;

    //                        uint skillIdx = parent.SkillCtlr.GetSkillIndex((int)parent.UseSkillSlot);

    //                        //황비홍 프로젝트 - 프로젝트에 맞게 수정
    //                        triggerEvt.Setup(parent, parent.SkillCtlr.__SkillGroupInfo.GetAbility(skillIdx, 1));
    //                        //triggerEvt.Setup(parent, parent.SkillCtlr._SkillGroupInfo.GetAbility((uint)parent.UseSkillSlot + 1, actionData.effectCallNotiIdx, parent.UnitType == UnitType.Unit ? (parent as Pc).syncData.SkillLvDatas : null));
    //                        //triggerEvt.Setup(parent, parent.SkillCtlr._SkillGroupInfo.GetAbility((uint)parent.UseSkillSlot + 1, actionData.effectCallNotiIdx, parent.UnitType == UnitType.Unit ? (parent as Pc).syncData.SkillLvDatas : null));
    //                    }
    //                }

    //            }                
    //        });
    // }
    // else
    // {
    //        SoundManager.instance.PlaySfxSound(StartSoundID, parent.cachedTransform);
    //        //< 시전이펙트가 없을시 바로 애니메이션 실행
    //        parent.PlayAnim(aniName, true, fadeTime);
    //        nextAttackTime = atkAnimLength;

    //        //황비홍 프로젝트 - 어택딜레이 유저는 없고 몬스터는 지정된 수치로 딜레이
    //        //if (parent.CurCombo == 0)
    //        {
    //            if (parent as Pc && !parent.IsPartner)
    //                parent.CharInfo.AtkDelay = 0.0f;
    //            else
    //                parent.CharInfo.AtkDelay = 0.5f;// _LowDataMgr.G_AttackDelay();

    //            nextAttackTime = atkAnimLength + parent.CharInfo.AtkDelay;
    //        }
    //    }

    //    goNear = false;
    //    //isDashAtk = false;
    //    parent.PreEndAttackAnim = false;

    //    parent.ActiveAttack = true;

    //    //< 리더가 아니라면 공격할때 대상을 바라본다.
    //    if (parent.UnitType == UnitType.Npc || !parent.IsLeader)
    //        parent.transform.LookAt(target.cachedTransform);

    //    ////< 누구든 첫 공격이면 배틀시작
    //    //if (!PVPGameInfo.BattleStartCheck && G_GameInfo.GameMode == GAME_MODE.PVP)
    //    //    PVPGameInfo.BattleStartCheck = true;

    //    //< PVP일때 적 AI를 위한 처리
    //    if (G_GameInfo.GameMode == GAME_MODE.PVP && parent.TeamID == 0)
    //        EventListner.instance.TriggerEvent("PC_USE_ATTACK", parent);
    //}

    float NewCheckTargetDelay = 0.2f;
    void CheckTarget()
    {
        //< 일정시간마다 제일 가까운 타겟으로 다시 검사한다.(PC일경우에만 검사한다. 몬스터는 PC만 타겟이기때문)
        if (parent.UnitType == UnitType.Unit)
        {
            NewCheckTargetDelay -= Time.deltaTime;
            if (NewCheckTargetDelay <= 0)
            {
                NewCheckTargetDelay = 0.2f;

                //< 일점사 대상이 없을경우에만 새로운 대상을 검색한다.
                if (parent.ForcedAttackTarget == null)
                {
                    Unit newCheckTarget = G_GameInfo.CharacterMgr.FindTargetWithAggro(parent, parent.CharInfo.AtkRecognition, parent, true);
                    if (null != newCheckTarget)
                        parent.TargetID = newCheckTarget.GetInstanceID();
                }
            }
        }

        Unit newTarget = null;
        invalidTarget = G_GameInfo.CharacterMgr.InvalidTarget(parent.TargetID, ref newTarget);
        if (!invalidTarget && target != newTarget)
            target = newTarget;

        // 공격 못하는 타겟인지 체크
        bool cantAtkType = false;
        if (target != null)
            cantAtkType = target.CurrentState == UnitState.Dead || target.CurrentState == UnitState.Dying || target.CharInfo.Hp <= 0;//|| (target.CurrentState == UnitState.Event && RaidBossAIBase.NotTargeting);

        if (cantAtkType || invalidTarget)
        {
            parent.SetTarget(GameDefine.unassignedID);
            target = null;
        }
    }

    ///*
    //// 대시 공격 가능 여부에 따라 작동함
    //Unit PrevTargetUnit;
    //void CheckDashAttack()
    //{
    //    if (!parent.CharInfo.CanDashAttack)
    //        return;

    //    // 첫번째 공격이고, 근접무기라면 대시공격 여부 검사
    //    Vector3 targetDist = target.cachedTransform.position - parent.cachedTransform.position;
    //    float distance = targetDist.magnitude - (parent.Radius + target.Radius);
    //    attackAnim = FirstAtkAnimName( distance );
    //    //isDashAtk = attackAnim != eAnimName.Anim_attack1;
    //    //if (isDashAtk)
    //    //{
    //    //    //< 어차피 대상에게 이동하는 도중에 거리체크를 계속 하므로 대상위치로 잡아도된다
    //    //    if (PrevTargetUnit == null || PrevTargetUnit != target)
    //    //    {
    //    //        parent.CalculatePath(target.cachedTransform.position);
    //    //        PrevTargetUnit = target;
    //    //    }
    //    //    ExecuteDashFunc = null;
    //    //}
    //}

    ///// 주어진 거리에 맞는 공격 애니메이션 이름을 알려준다. (영웅전용)
    //public eAnimName FirstAtkAnimName(float distance)
    //{
    //    GameCharacterInfo charInfo = parent.CharInfo;

    //    // 원거리
    //    if (!charInfo.CanDashAttack)
    //        return eAnimName.Anim_attack1;

    //    // 근거리
    //    if (distance <= charInfo.AtkRange)
    //    {
    //        return eAnimName.Anim_attack1;
    //    }
    //    //else if (distance <= (charInfo.RushAtkRange)) 
    //    //{
    //    //    return eAnimName.Anim_attack1_f;
    //    //}

    //    return eAnimName.Anim_attack1;
    //}
    //*/

    /////// NavMeshAgent의 AvoidanceType을 변경하기. (Unit이 이동중일때는 충돌검사 안하도록 하기 위함)
    ////void SetObstacleAvoidance(bool isEnable)
    ////{
    ////    if (null != navAgent)/* && parent.UnitType == UnitType.Unit)*/
    ////    {
    ////        if (isEnable)
    ////            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.GoodQualityObstacleAvoidance;
    ////        else
    ////            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    ////    }
    ////}

    ///// 스킬 이펙트에 존재하는 충돌체의 TriggerEnter에 연결되어 액션을 수행할 함수
    //void FireToTarget(Unit _Target, ForwardTriggerEvent _TriggerEvent)
    //{
    //    //황비홍 프로젝트 - 발사체 적용중
    //    //if (!EffectCallAbility)
    //    //    return;

    //    if (null != _Target)
    //    {
    //        for (int i = 0; i < _TriggerEvent.SkillAbilityLists.Count; i++)
    //        {
    //            //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
    //            if (SceneManager.instance.IsRTNetwork)
    //            {
    //                if( parent.IsLeader || (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID))
    //                {
    //                    System.Collections.Generic.List<ulong> a_list = new System.Collections.Generic.List<ulong>();
    //                    a_list.Add(_Target.m_rUUID);

    //                    if (a_list.Count <= 0)
    //                        a_list.Add(ulong.MaxValue);

    //                    iFunClient.instance.ReqDamage( parent.m_rUUID, _LowDataMgr.GetSkillAction(_TriggerEvent.SkillAbilityLists[i].Idx).idx, _TriggerEvent.SkillAbilityLists[i].notiIdx, parent.transform.position, parent.transform.eulerAngles.y, a_list); //--> 피격 대상인 놈들을 모두 찾는다.
    //                }                    
    //            }
    //            else
    //            {
    //                //< 공격
    //                SkillAbility.ApplySkill(SkillType.Attack, _TriggerEvent.SkillAbilityLists[i], parent, _Target, false);

    //                //< 버프 호출이있을시 버프호출
    //                if (_TriggerEvent.SkillAbilityLists[i].callBuffIdx != 0 && _Target.BuffCtlr != null)
    //                    _Target.BuffCtlr.AttachBuff(parent, _Target, parent.SkillCtlr.__SkillGroupInfo.GetBuff(_TriggerEvent.SkillAbilityLists[i].callBuffIdx, _TriggerEvent.SkillAbilityLists[i].Idx), _TriggerEvent.SkillAbilityLists[i]);
    //                //_Target.BuffCtlr.AttachBuff(parent, _Target, parent.SkillCtlr._SkillGroupInfo.GetBuff(_TriggerEvent.SkillAbilityLists[i].callBuffIdx, _TriggerEvent.SkillAbilityLists[i].unitIdx, _TriggerEvent.SkillAbilityLists[i].skillIdx, parent.UnitType == UnitType.Unit ? (parent as Pc).syncData.SkillLvDatas : null), _TriggerEvent.SkillAbilityLists[i]);
    //            }
    //        }
    //    }
    //}
}
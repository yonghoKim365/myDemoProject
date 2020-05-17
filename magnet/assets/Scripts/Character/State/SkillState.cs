using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillState : UnitStateBase
{
    public static bool SkillCameraEvent = true;
    public float moveSpeedRatio = 1f;

    public float endTime = 0.0f;

    protected uint _NowSkillIndex = 0;

    //< 이펙트 박스 안에서 어빌리티를 호출하는 구조인지?
    public bool EffectCallAbility = false;
    public ActionInfo actionData;

    public bool MoveableSkill = false;

    public override void OnEnter(System.Action callback)
    {
	    base.OnEnter(callback);
        parent.SetObstacleAvoidance(true);

        parent.CurrentState = UnitState.Skill;
	    EffectCallAbility = false;

        parent.SkillBlend = false;
        MoveableSkill = parent.MoveableSkill;

        _NowSkillIndex = parent.UseSkillSlot;

        //< 스킬 활성화
        if (!parent.SkillCtlr.ActiveSkill(_NowSkillIndex))
	    {
		    //parent.ChangeState(UnitState.Idle);
            //parent.SkillCtlr.SkillList[_NowSkillIndex].bActive = false;
            return;
	    }

        //맹장버프 예외처리
        if (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID))
        {
            //네트워크로 버프제거 요청
        }
        else
        {
            parent.BuffCtlr.DetachBuff(BuffDurationType.P_Extra);
        }

        //< 스킬 이름 표시(메인 리더만 사용)
        actionData = parent.SkillCtlr.GetActionInfo(_NowSkillIndex);

        eAnimName aniName;

        if (_NowSkillIndex >= 5)
        {
            aniName = (eAnimName)((int)eAnimName.Anim_Chain);
        }
        else
        {
            aniName = (eAnimName)((int)eAnimName.Anim_skillStart + _NowSkillIndex);
        }

        if (!SceneManager.instance.IsRTNetwork || (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID)))
        {
            if (parent.UnitType == UnitType.Unit)
            {
                parent.FindTarget();
                // ** 회전 조절 우선순위 **
                // 1. 조이스틱 회전값
                // 2. 타겟존재 여부
                if (parent.inputCtlr != null && Quaternion.identity != parent.inputCtlr.RotationFromInterJoyStick)
                    parent.cachedTransform.rotation = parent.inputCtlr.RotationFromInterJoyStick;
                else if (parent.GetTarget() != null)
                    parent.LookAt(parent.GetTarget().cachedTransform.position);
            }
        }

        if (SceneManager.instance.IsRTNetwork)
        {
            //if (parent.IsLeader == true || (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID))
            if (parent.m_rUUID == NetData.instance._userInfo._charUUID)
            {
                if (null != parent.Animation[parent.Animator.GetAnimName(aniName)])
                {
                    ActionInfo actionIfno = parent.SkillCtlr.SkillList[_NowSkillIndex].GetSkillActionInfo();

                    //일단 서버에 위치보정
                    parent.MoveNetworkCalibrate(parent.cachedTransform.position);
                    //pc의 경우 8방향이 아닌 360도를 쓰게 수정
                    //NetworkClient.instance.SendPMsgBattleAttackPrepareC((int)actionIfno.idx, 0, (int)UnitModelHelper.RotateTo8Way(parent.transform.eulerAngles.y));
                    NetworkClient.instance.SendPMsgBattleAttackPrepareC((int)actionIfno.idx, 0, (int)parent.cachedTransform.eulerAngles.y);
                }
            }
        }

        // 애니메이션 
        endTime = 90;
        // 시전 이펙트 재생
        string StartEffect = "0";
        uint StartSoundID = 0;

        Resource.AniInfo aniData = parent.GetAniData(aniName);

        if( actionData.callChainIdx != 0 )
        {
            if (!SceneManager.instance.IsRTNetwork || (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID)))
            {
                if( parent.CharInfo.CharIndex == 13000 && parent.ChainSkill )
                {
                    //의사이고 체인이 스킬에서 시작된경우만 체인연결
                    parent.ChainEndTime = Time.time + actionData.callChainTime;
                    parent.ChainLevel = 0;
                    switch (_NowSkillIndex)
                    {
                        case 5:
                            parent.ChainLevel = 0;
                            break;
                        case 6:
                            parent.ChainLevel = 1;
                            break;
                        case 7:
                            parent.ChainLevel = 2;
                            break;

                    }
                }
                else
                {
                    //체인스킬 활성화
                    parent.ChainEndTime = Time.time + actionData.callChainTime;
                    parent.ChainLevel = 0;
                }
            }
        }

        if (aniData != null)
        {
            if( parent.SkillCtlr.isBuffSkill(_NowSkillIndex) )
            {
                //스킬이 버프스킬일경우 애니메이션에서 이펙트를 가져오는게아님
                //강제로 1번노티의 스킬아이디에서 연결된 버프ID를 가져옴
                //버프일단 비활성
                //uint tempBuffID = parent.SkillCtlr.__SkillGroupInfo.GetAbility(parent.UseSkillIdx, 1).callBuffIdx;

                //if(tempBuffID != 0)
                //{
                //    StartEffect = parent.SkillCtlr.__SkillGroupInfo.GetBuff(tempBuffID, parent.UseSkillIdx).CastingEffect;
                //    StartSoundID = aniData.seSkill;
                //}
            }
            else
            {
                StartEffect = aniData.effect;
                StartSoundID = aniData.seSkill;
            }
        }

        if ("0" != StartEffect && null != parent.Animation[parent.Animator.GetAnimName(aniName)])
        {
            parent.SpawnSkillEffect(StartEffect, parent.Animation[parent.Animator.GetAnimName(aniName)].speed, parent.cachedTransform, aniData.childEffect == 0 ? null : parent.cachedTransform, (skillCastingEff) =>
            //parent.SpawnSkillEffect(StartEffect, parent.Animation[parent.Animator.GetAnimName(aniName)].speed, parent.transform, parent.transform, (skillCastingEff) =>
            {
                //< 애니메이션 실행
                //parent.PlayAnim(aniName, true, 0.5f);
                parent.PlayAnim(aniName, true);
                //SoundManager.instance.PlaySfxSound(StartSoundID, parent.cachedTransform);
                SoundManager.instance.PlaySfxUnitSound(StartSoundID, parent._audioSource, parent.cachedTransform);
                SoundManager.instance.PlayUnitVoiceSound(parent.GetVoiceID(aniName), parent._audioSource, parent.cachedTransform);
                endTime = parent.Animator.GetAnimLength(aniName);

                //< 플레이어라면 스킬이벤트 시작
                if (SkillCameraEvent && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
                {
                    if(actionData.camera != 0)
                        SkillEventMgr.instance.SetEvent(true, parent, actionData);
                }
                    

                // 1. 이펙트 자식중에 충돌체가 있는지 찾도록 한다.
                if (actionData.effectCallNotiIdx != 0)
                {
                    Collider[] existCol = skillCastingEff.GetComponentsInChildren<Collider>(true);
                    for (int j = 0; j < existCol.Length; j++)
                    {
                        EffectCallAbility = true;

                        //< 있을경우는 삭제
                        ForwardTriggerEvent OrgtriggerEvt = existCol[j].gameObject.GetComponent<ForwardTriggerEvent>();
                        if (OrgtriggerEvt != null)
                            UIMgr.Destroy(OrgtriggerEvt);

                        existCol[j].isTrigger = false;
                        existCol[j].isTrigger = true;
                        ForwardTriggerEvent triggerEvt = existCol[j].gameObject.AddComponent<ForwardTriggerEvent>();
                        triggerEvt.TriggerEnter_Unit = FireToTarget;

                        uint skillIdx = parent.SkillCtlr.GetSkillIndex((int)_NowSkillIndex);

                        triggerEvt.Setup(parent, parent.SkillCtlr.__SkillGroupInfo.GetAbility(skillIdx, 1));
                    }
                }
            });
        }
        else if(null != parent.Animation[parent.Animator.GetAnimName(aniName)])
        {
            //< 시전이펙트가 없을시 바로 애니메이션 실행
            parent.PlayAnim(aniName, true);
            SoundManager.instance.PlaySfxUnitSound(StartSoundID, parent._audioSource, parent.cachedTransform);
            endTime = parent.Animator.GetAnimLength(aniName);

            //< 플레이어라면 스킬이벤트 시작
            if (SkillCameraEvent && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
                SkillEventMgr.instance.SetEvent(true, parent, actionData);
        }
        else
        {
            //애니메이션 이없으면 소리와 함께 바로 스킬종료
            SoundManager.instance.PlaySfxUnitSound(StartSoundID, parent._audioSource, parent.cachedTransform);
            endTime = 0;
        }

        //여기까지 왔다면 액션인포에 스킬유지 버프가 있다면 버프를 켜자
        if (actionData.callCastingBuffIdx != 0)
        {
            //버프가 있다 버프추가
            if (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID))
            {
                //네트워크로 버프활성요청

            }
            else
            {
                parent.BuffCtlr.AttachBuff(parent, parent, parent.SkillCtlr.__SkillGroupInfo.GetBuff(actionData.callCastingBuffIdx, actionData.idx), 10000, 3);
            }
        }

        if (parent.UnitType == UnitType.Unit && parent.IsLeader)
            EventListner.instance.TriggerEvent("UseSkill_Start", parent);

        //< PVP일때 적 AI를 위한 처리
        //G_GameInfo.GameMode == GAME_MODE.PVP && 
        if (parent.TeamID == 0)
            EventListner.instance.TriggerEvent("PC_USE_SKILL", parent);
    }

    public override void OnExit(System.Action callback)
    {
        //if(MoveableSkill)
        //    parent.MoveNetwork(parent.cachedTransform.position);

        if (SkillCameraEvent && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            if (actionData.camera != 0)
                SkillEventMgr.instance.SetEvent(false, parent, actionData);
        }
            
        
        EffectCallAbility = false;

        //< 혹시 제대로된 종료가 아니라면 이펙트및 사운드 강제종료
        parent.SkillEffectClear(endTime > 0.1f);

        //< 스킬 종료처리
        //if (parent.SkillCtlr.SkillList.Length > _NowSkillIndex && parent.SkillCtlr.SkillList[_NowSkillIndex] != null)
            parent.SkillCtlr.SkillList[_NowSkillIndex].EndSkill();

        parent.SkillBlend = false;
        parent.MoveableSkill = false;

        if (parent.UnitType == UnitType.Unit && parent.IsLeader)
            EventListner.instance.TriggerEvent("UseSkill_End", parent);

        if (parent.skill_AI != null)
            parent.skill_AI.EndSkill();

        if (parent is Npc && parent.UnitType == UnitType.Boss)
        {
            if ((parent as Npc).BossPatten != null && parent.SkillCtlr.SkillList[_NowSkillIndex].IsPatten)//G_GameInfo.GameMode == GAME_MODE.RAID 
            {
                (parent as Npc).BossPatten.EndPattenSkill(_NowSkillIndex);
            }
        }

        if (SceneManager.instance.IsRTNetwork)
        {
            parent.SetTarget(GameDefine.unassignedID); 
        }

        //스킬발동 버프가 있을경우 버프를 끄자
        if (actionData.callCastingBuffIdx != 0)
        {
            //버프가 있다 버프추가
            if (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID))
            {
                //네트워크로 버프제거 요청
            }
            else
            {
                if( parent.BuffCtlr.CheckBuffType(actionData.callCastingBuffIdx, BuffDurationType.SkillAttach) )
                {
                    parent.BuffCtlr.DetachBuff(actionData.callCastingBuffIdx);
                }
            }
        }

        parent.ReservedSkillIdx = 0;
        parent.ReservedSkillTime = 0;

        //스킬이 종료한후 타일의 이동이 있었다면 체크
        //if(SceneManager.instance.IsRTNetwork)
        //{
        //    Vector3 MinPos = NaviTileInfo.instance.GetMinPos();

        //    float PointX = Mathf.Abs(MinPos.x - transform.position.x) / NaviTileInfo.instance.GetTileSize();
        //    float PointY = Mathf.Abs(MinPos.z - transform.position.z) / NaviTileInfo.instance.GetTileSize();

        //    if(PointX != parent.BeforePosX || PointY != parent.BeforePosY)
        //    {
        //        Debug.Log("스킬종료후 위치가 바뀜");
        //    }
        //}

        base.OnExit( callback );
    }

    //Vector3 a_Tgetpos = Vector3.zero;
    public override void CachedUpdate()
    {
        parent.UnitStop = true;

        endTime -= Time.deltaTime;
        if (endTime <= 0)
        {
            parent.ChangeState(UnitState.Idle);
        }

        if(parent.MoveableSkill)
        {
            //if (parent.inputCtlr != null && Quaternion.identity != parent.inputCtlr.RotationFromJoyStick)
            //{
            //    parent.UnitStop = false;
            //    if (!parent.MoveToPath(moveSpeedRatio, true))
            //    {
            //        {
            //            //< 조이스틱으로 움직이고 있다면 패스
            //            //if (!parent.StopState)
            //            //    parent.ChangeState(UnitState.Idle);
            //        }
            //    }
            //}
            //else //if (parent.GetTarget() != null)
            //{
            //    parent.StopState = parent.MovePosition(parent.cachedTransform.position + (parent.cachedTransform.forward * 0.5f), moveSpeedRatio);
            //}
            if (parent.inputCtlr != null && Quaternion.identity != parent.inputCtlr.RotationFromInterJoyStick)
            {
                parent.cachedTransform.rotation = parent.inputCtlr.RotationFromInterJoyStick;
            }
            else if (parent.GetTarget() != null)
            {
                parent.LookAt(parent.GetTarget().cachedTransform.position);
            }
            
            parent.StopState = parent.MovePosition(parent.cachedTransform.position + (parent.cachedTransform.forward * 0.5f), moveSpeedRatio);

            //parent.MoveNetwork(parent.cachedTransform.position);

            parent.UnitStop = false;
            if (!parent.MoveToPath(moveSpeedRatio, true))
            {
                {
                    //< 조이스틱으로 움직이고 있다면 패스
                    //if (!parent.StopState)
                    //    parent.ChangeState(UnitState.Idle);
                }
            }
        }
    }

    /// 스킬 이펙트에 존재하는 충돌체의 TriggerEnter에 연결되어 액션을 수행할 함수
    void FireToTarget(Unit _Target, ForwardTriggerEvent _TriggerEvent)
    {
        if (!EffectCallAbility)
            return;

        if (null != _Target)
        {
            for (int i = 0; i < _TriggerEvent.SkillAbilityLists.Count; i++ )
            {
                if (SceneManager.instance.IsRTNetwork)
                {
                    if(parent.IsLeader || (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID))
                    {
                        System.Collections.Generic.List<ulong> a_list = new System.Collections.Generic.List<ulong>();
                        a_list.Add(_Target.m_rUUID);

                        if (a_list.Count <= 0)
                            a_list.Add(ulong.MaxValue);

                        //iFunClient.instance.ReqDamage(parent.m_rUUID, _LowDataMgr.GetSkillAction(_TriggerEvent.SkillAbilityLists[i].Idx).idx, _TriggerEvent.SkillAbilityLists[i].notiIdx, parent.transform.position, parent.transform.eulerAngles.y, a_list); //--> 피격 대상인 놈들을 모두 찾는다.
                    }                    
                }
                else
                {
                    //< 공격
                    SkillAbility.ApplySkill(SkillType.Attack, _TriggerEvent.SkillAbilityLists[i], parent, _Target, false);

                    //< 버프 호출이있을시 버프호출
                    /*
                    if (_TriggerEvent.SkillAbilityLists[i].callBuffIdx != 0 && _Target.BuffCtlr != null)
                        _Target.BuffCtlr.AttachBuff(parent, _Target, parent.SkillCtlr.__SkillGroupInfo.GetBuff(_TriggerEvent.SkillAbilityLists[i].callBuffIdx, _TriggerEvent.SkillAbilityLists[i].Idx), _TriggerEvent.SkillAbilityLists[i]);
                    */
                }
            }  
        }
    }
}

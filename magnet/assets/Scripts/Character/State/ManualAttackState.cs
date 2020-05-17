using UnityEngine;
using System.Collections;

public class ManualAttackState : UnitStateBase
{
    float   fadeTime = 0.08f;

    float attackEndTime = 0f;
    bool isCurEndAnim = true;
    float ComboKeepTime;

    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize( _parent );

    }

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.SkillBlend = false;

        Attack();
    }

    public override void OnExit(System.Action callback)
    {
        if(parent.isLeader)
        {
            if (parent.UnitType == UnitType.Unit)
            {
                //if(G_GameInfo.GameInfo.AutoMode)
                //    ComboKeepTime = Time.time + 0.2f;
                //else
                    ComboKeepTime = Time.time + 0.1f;
            }                
            else
                ComboKeepTime = float.MaxValue;
        }

        parent.SkillBlend = false;

        parent.ActiveAttack = false;

        //< 스킬 이펙트 종료
        parent.SkillEffectClear(Time.time < (atkAnimLength));

        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        if (SceneManager.instance.IsRTNetwork)
        {
            parent.SetTarget(GameDefine.unassignedID); //타겟 초기화 
        }

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        if (parent == null)
            return;

        float   curTime = Time.time;
        isCurEndAnim = curTime > attackEndTime;
        parent.ActiveAttack = !isCurEndAnim;

        // 애니메이션이 끝날때 까지 아무일도 없으면 상태전환.
        if (isCurEndAnim)
        {
            parent.ChangeState( UnitState.Idle );
            return;
        }
        else
        {
            // 공격대기시간동안은 Idle
            if (!parent.Animation.isPlaying)
                parent.PlayAnim( eAnimName.Anim_battle_idle );
        }
    }

    float atkAnimLength;
    private void Attack()
    {
        parent.UnitStop = true;

        if (parent.isLeader)
        {
            if (Time.time > ComboKeepTime)
                parent.CurCombo = 0;
        }

        if (!SceneManager.instance.IsRTNetwork || (SceneManager.instance.IsRTNetwork && (parent.m_rUUID == NetData.instance._userInfo._charUUID)))
        {
            if(parent.UnitType == UnitType.Unit)
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
            if (0 < parent.m_SvComboIdx) //중계 받아온 입장에서만...
            {
                parent.CurCombo = parent.m_SvComboIdx; //평타 인덱스 동기화 
                parent.m_SvComboIdx = -1;
            }
        }

        eAnimName aniName;
        if (parent is Pc && !parent.IsPartner)
            aniName = (eAnimName)((int)eAnimName.Anim_attack1 + parent.CurCombo);
        else
            aniName = (eAnimName)((int)eAnimName.Anim_attack1);

        atkAnimLength = Time.time + parent.Animator.GetAnimLength(aniName) - fadeTime;
        attackEndTime = 90;

        int a_OldCombo = parent.CurCombo;

        if (parent is Pc && !parent.IsPartner)
        {
            parent.UseSkillIdx = parent.GetNormalAttackComboSkillID(parent.CurCombo);
            parent.CurCombo++;
        }
        else
        {
            parent.UseSkillIdx = parent.GetNormalAttackComboSkillID(0);
        }
        
        if (SceneManager.instance.IsRTNetwork)
        {
            //if (parent.IsLeader == true || (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID)) 
            if(parent.m_rUUID == NetData.instance._userInfo._charUUID)
            {
                uint a_Combo;
                a_Combo = parent.GetNormalAttackComboSkillID(a_OldCombo);

                if(a_Combo == uint.MaxValue)
                {
                    Debug.Log("NotFound NormalAttack ID");
                    return;
                }
                /*
                ////루트 가져오기
                a_RootMoPos = 0.0f;
                if (parent.UnitType == UnitType.Unit)
                {
                    _ResourceLowData.AniTableInfo aniInfo = parent.CharInfo.animDatas[(int)aniName];
                    if (parent is Pc && aniInfo.rootMotion == true)
                    {
                        Pc a_refPc = (Pc)parent;
                        string animName = a_refPc.Animator.GetAnimName(aniName);
                        Vector3 a_CacVec = a_refPc.rootMotion.CalcTotalMovingDistance(animName);
                        a_RootMoPos = a_CacVec.z * 0.7f;
                    }
                }
                else if(parent.UnitType == UnitType.Npc)
                {
                    _ResourceLowData.AniTableInfo aniInfo = parent.CharInfo.animDatas[(int)aniName];
                    if (parent is Npc && aniInfo.rootMotion == true)
                    {
                        Npc a_refNpc = (Npc)parent;
                        string animName = a_refNpc.Animator.GetAnimName(aniName);
                        Vector3 a_CacVec = a_refNpc.rootMotion.CalcTotalMovingDistance(animName);
                        a_RootMoPos = a_CacVec.z * 0.7f;
                    }
                }

                parent.g_CacDamPos = parent.transform.position + (parent.transform.forward * a_RootMoPos);  //이미 캐릭터간 간격은 충분히 멀다. 충돌박스로 인하여... 1.8 ~ 2.1 m
                //parent.g_CacDamRot = Quaternion.LookRotation(parent.transform.forward).eulerAngles;   <--타겟을 이미 바라보고 있기 때문에 이 부분은 않해도 될 것 같다.

                parent.g_AniEvent.ForGetList_SkillUse(1, UnitState.ManualAttack); //<-- 이렇게 해서 타겟의 리스트를 얻어온다.
                System.Collections.Generic.List<ulong> a_list = new System.Collections.Generic.List<ulong>();
                a_list.Clear();

                for (int a_ii = 0; a_ii < parent.g_targetList.Count; a_ii++)
                {
                    a_list.Add(parent.g_targetList[a_ii].m_rUUID);
                }

                if (a_list.Count <= 0)
                    a_list.Add(ulong.MaxValue);
                */
                //iFunClient.instance.ReqSkillStart( parent.m_rUUID, a_Combo, parent.transform.position, parent.transform.eulerAngles.y); 
                //NetworkClient.instance.SendPMsgBattleAttackPrepareC((int)parent.UseSkillIdx, 0, (int)UnitModelHelper.RotateTo8Way(parent.transform.eulerAngles.y));

                //일단 서버에 위치보정을 하자
                parent.MoveNetworkCalibrate(parent.cachedTransform.position);

                //pc의 경우 8방향이 아닌 360도를 쓰게 수정
                NetworkClient.instance.SendPMsgBattleAttackPrepareC((int)parent.UseSkillIdx, 0, (int)parent.cachedTransform.eulerAngles.y);
            }
            //else if (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID)
            //{
            //    uint a_Combo = parent.GetNormalAttackComboSkillID(0);
            //    NetworkClient.instance.SendPMsgBattleAttackPrepareC((int)parent.UseSkillIdx, 0, 1);
            //}
        }

        //< 이펙트 실행
        string StartEffect = "0";
        uint StartSoundID = 0;

        Resource.AniInfo aniData = parent.GetAniData(aniName);

        if (aniData != null)
        {
		    StartEffect = aniData.effect;
            StartSoundID = aniData.seSkill;
        }

	    if ("0" != StartEffect && null != parent.Animation[parent.Animator.GetAnimName(aniName)])
	    {
            parent.SpawnSkillEffect(StartEffect, parent.Animation[parent.Animator.GetAnimName(aniName)].speed, parent.cachedTransform, aniData.childEffect == 0 ? null : parent.cachedTransform, (skillCastingEff) =>
            //parent.SpawnSkillEffect(StartEffect, parent.Animation[parent.Animator.GetAnimName(aniName)].speed, parent.transform, parent.transform, (skillCastingEff) =>
            {
                //SoundManager.instance.PlaySfxSound(StartSoundID, parent.cachedTransform);
                SoundManager.instance.PlaySfxUnitSound(StartSoundID, parent._audioSource, parent.cachedTransform);
                SoundManager.instance.PlayUnitVoiceSound(parent.GetVoiceID(aniName), parent._audioSource, parent.cachedTransform);
                //< 애니메이션 실행
                //parent.PlayAnim(aniName, true, fadeTime, true);
                parent.PlayAnim(aniName, true);
                attackEndTime = atkAnimLength + parent.CharInfo.AtkDelay;

                ActionInfo actionData = parent.SkillCtlr.__SkillGroupInfo.GetAction(parent.UseSkillIdx);
                
                if(actionData != null)
                {
                    if (actionData.effectCallNotiIdx != 0)
                    {
                        Collider[] existCol = skillCastingEff.GetComponentsInChildren<Collider>(true);
                        for (int j = 0; j < existCol.Length; j++)
                        {
                            //EffectCallAbility = true;

                            //< 있을경우는 삭제
                            ForwardTriggerEvent OrgtriggerEvt = existCol[j].gameObject.GetComponent<ForwardTriggerEvent>();
                            if (OrgtriggerEvt != null)
                                UIMgr.Destroy(OrgtriggerEvt);

                            existCol[j].isTrigger = false;
                            existCol[j].isTrigger = true;
                            ForwardTriggerEvent triggerEvt = existCol[j].gameObject.AddComponent<ForwardTriggerEvent>();
                            triggerEvt.TriggerEnter_Unit = FireToTarget;

                            uint skillIdx = parent.SkillCtlr.GetSkillIndex((int)parent.UseSkillSlot);

                            //황비홍 프로젝트 - 프로젝트에 맞게 수정
                            triggerEvt.Setup(parent, parent.SkillCtlr.__SkillGroupInfo.GetAbility(skillIdx, 1));
                        }
                    }
                }
            });
        }
        else
        {
            //SoundManager.instance.PlaySfxSound(StartSoundID, parent.cachedTransform);
            SoundManager.instance.PlaySfxUnitSound(StartSoundID, parent._audioSource, parent.cachedTransform);

            //< 시전이펙트가 없을시 바로 애니메이션 실행
            parent.PlayAnim(aniName, true, fadeTime, true);
            attackEndTime = atkAnimLength + parent.CharInfo.AtkDelay;
        }

        parent.PreEndAttackAnim = false;
        parent.ActiveAttack = true;

    }

    /// 스킬 이펙트에 존재하는 충돌체의 TriggerEnter에 연결되어 액션을 수행할 함수
    void FireToTarget(Unit _Target, ForwardTriggerEvent _TriggerEvent)
    {
        //황비홍 프로젝트 - 발사체 적용중
        //if (!EffectCallAbility)
        //    return;

        if (null != _Target)
        {
            for (int i = 0; i < _TriggerEvent.SkillAbilityLists.Count; i++)
            {
                if (SceneManager.instance.IsRTNetwork)
                {
                    if (parent.IsLeader || (parent.IsPartner == true && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID))
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
                    SkillAbility.ApplySkill(SkillType.Attack, _TriggerEvent.SkillAbilityLists[i], parent, _Target, true);

                    /*
                    //< 버프 호출이있을시 버프호출
                    if (_TriggerEvent.SkillAbilityLists[i].callBuffIdx != 0 && _Target.BuffCtlr != null)
                        _Target.BuffCtlr.AttachBuff(parent, _Target, parent.SkillCtlr.__SkillGroupInfo.GetBuff(_TriggerEvent.SkillAbilityLists[i].callBuffIdx, _TriggerEvent.SkillAbilityLists[i].Idx), _TriggerEvent.SkillAbilityLists[i]);
                    */
                }
            }
        }
    }
}

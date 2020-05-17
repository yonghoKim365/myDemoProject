using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillAbility
{
    struct SelectUnitData
    {
        public float magnitude;
        public Unit Owner;
    }

    /// 스킬에 적용 시킬 값을 계산한다.
    public static float CalcAffectValue(AbilityData _ability, Unit _Caster, Unit _Target, float prefix = 1f)
    {
        return CalcAffectValue(_ability.baseFactor, _ability.factorRate, _ability.factor, _Caster, _Target, prefix);
    }

    public static float CalcAffectValue(SkillTables.BuffInfo _buff, Unit _Caster, Unit _Target, float prefix = 1f)
    {
        //if (SimulationGameInfo.SimulationGameCheck)
        //    Debug.Log(string.Format("BuffCalcAffectValue  BuffIdx[{0}], baseFactor[{1}], factorRate[{2}]", _buff.Indx, _buff.baseFactor, _buff.factorRate));

        //< 아래의 값들은 전부다 수치가 %로만 저장되야하므로 베이스팩터는 0이어야함
        //< 그렇기때문에 따로 계산할필요없이 팩터레이트값만 리턴해주면됨
        switch ((BuffType)_buff.buffType)
        {
            /*
            case BuffType.Attack:
            case BuffType.Def:
            case BuffType.CriticalRate:
            case BuffType.CriticalDam:

            case BuffType.NatureAttack:
            case BuffType.PoisonAttack:
            case BuffType.WaterAttack:
            case BuffType.MetalAttack:
            case BuffType.FireAttack:
            case BuffType.HollyAttack:
            case BuffType.DarkAttack:
            
            case BuffType.AttackSpeed:
            case BuffType.IgnoreDef:
            case BuffType.MaxHp:
            */
            case BuffType.DAMAGE_INCREASE:
            case BuffType.ATTACKSPEED_INCREASE:
            case BuffType.DAMAGEREDUCE_INCRESE:
            case BuffType.DAMAGEREDUCEPER_INCREASE:
            case BuffType.CRITICALRATE_INCREASE:
            case BuffType.CRITICALDAMAGE_INCREASE:
            case BuffType.CRITICALRES_INCREASE:
            case BuffType.HITRATE_INCREASE:
            case BuffType.DODGERATE_INCREASE:
            case BuffType.LIFESTEAL_INCREASE:
            case BuffType.ANGLEDEFUP:
                return _buff.factorRate * prefix;
        }

        return CalcAffectValue(_buff.baseFactor, _buff.factorRate, 0f, _Caster, _Target, prefix);
    }

    static float CalcAffectValue(byte baseFactor, float factorRate, float addfactor, Unit _Caster, Unit _Target, float prefix = 1f)
    {
        float result = 0;
        switch (baseFactor)
        {
            case 0:     //< 기본
                result = factorRate;
                break;

            case 1:     //< 시전자 공격력
                result = (_Caster.CharInfo.Atk * factorRate * prefix) + addfactor;
                break;

            case 2:     //< 시전자 방어력
                result = (_Caster.CharInfo.Def * factorRate * prefix);
                break;

            case 3:     //< 시전자 전체체력
                result = (_Caster.CharInfo.MaxHp * factorRate * prefix);
                break;

            case 4:     //< 시전자 현재체력
                result = (_Caster.CharInfo.Hp * factorRate * prefix);
                break;

            case 21:    //< 타겟 공격력
                result = (_Target.CharInfo.Atk * factorRate * prefix);
                break;

            case 22:    //< 타겟 방어력
                result = (_Target.CharInfo.Def * factorRate * prefix);
                break;

            case 23:    //< 타겟 전체체력
                result = (_Target.CharInfo.MaxHp * factorRate * prefix);
                break;

            case 24:    //< 타겟 현재체력
                result = (_Target.CharInfo.Hp * factorRate * prefix);
                break;
        }

        //if(SimulationGameInfo.SimulationGameCheck)
        //    Debug.Log("CalcAffectValue " + baseFactor + " , " + result);

        return result;
    }

    // 설정한 타겟에 맞게 타겟을 넘겨 준다.
    public static List<Unit> GetTargetList(byte ApplyTarget, Unit _Caster)
    {
        byte teamIndex = 0;
        if (ApplyTarget == 1 || ApplyTarget == 2)//적만
        {
            teamIndex = (byte)(_Caster.TeamID == 0 ? 1 : 0);
            List<Unit> list = new List<Unit>();

            //< 나의 현재타겟은 맨처음으로 넣어준다.
            Unit CasterTarget = _Caster.GetTarget();
            if (CasterTarget != null)
                list.Add(CasterTarget);

            if (G_GameInfo.CharacterMgr.allTeamDic.ContainsKey(teamIndex))
            {
                for (int i = 0; i < G_GameInfo.CharacterMgr.allTeamDic[teamIndex].Count; i++)
                {
                    Unit target = G_GameInfo.CharacterMgr.allTeamDic[teamIndex][i];
                    if (target != null && target != CasterTarget && target.Usable)
                        list.Add(target);
                }
            }

            if(G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
            {
                if (_Caster != null)
                {
                    //exceptUnit.TeamID //같은 팀중에서 OtherPc놈들만 찾는다.
                    List<Unit> MyTeamList = G_GameInfo.CharacterMgr.allTeamDic[_Caster.TeamID];
                    for (int i = 0; i < MyTeamList.Count; i++)
                    {
                        Unit target = MyTeamList[i];

                        if (_Caster == target)                     //캐스터만 빼고 모두 리스트에 넣어야 한다.
                            continue;

                        if (_Caster.Owner == target.Owner)         //유저 소유 캐릭터들도 모두 빼고
                            continue;

                        if (target != null && target != CasterTarget && target.Usable)
                            list.Add(target);
                    }
                }
            }

            return list;
        }
        else if (ApplyTarget == 3 || ApplyTarget == 4)//아군만
        {
            teamIndex = _Caster.TeamID;

            List<Unit> list = new List<Unit>();
            if (G_GameInfo.CharacterMgr.allTeamDic.ContainsKey(teamIndex))
            {
                for (int i = 0; i < G_GameInfo.CharacterMgr.allTeamDic[teamIndex].Count; i++)
                {
                    Unit target = G_GameInfo.CharacterMgr.allTeamDic[teamIndex][i];

                    //< 현재 활성화된 아군만
                    if (ApplyTarget == 3)
                    {
                        if (target != null && target.Usable)
                            list.Add(target);
                    }
                    //< 슬롯에있는 아군도
                    else if (ApplyTarget == 4)
                    {
                        if (target != null && !target.IsDie)
                            list.Add(target);
                    }
                }
            }
            return list;
        }
        
        return null;
    }

    //< 스킬 시전(외부에서 스킬 사용시 호출)
    public static bool SkillDebugMode = true;
    public static void ActiveSkill(AbilityData _ability, Unit _Caster, bool normal = false)
    {
        //if (SkillDebugMode && _Caster.UnitType == UnitType.Unit && SimulationGameInfo.SimulationGameCheck)
        //    Debug.Log("ActiveSkill  applyTarget " + _ability.applyTarget + " , skillIdx " + _ability.skillIdx + " , notiIdx " + _ability.notiIdx);

        if (_ability.applyTarget == 0)// 나만
        {
            if (_Caster.Usable)
                ApplySkill((SkillType)_ability.skillType, _ability, _Caster, _Caster, normal);
        }
        else if (_ability.applyTarget == 2)  //< 논타겟(발사체에서 주로사용)
        {
            ApplySkill((SkillType)_ability.skillType, _ability, _Caster, null, normal);
        }
        else //< 그외 모두
        {
            ushort radius = 0;

            radius = (ushort)(_ability.radius);/* + _LowDataMgr.GetSkillAction(_ability.Idx).range)*/
            
            //< 뭐든간에 프로젝트타일이라면 조금더 늘려줌
            if ((SkillType)_ability.skillType == SkillType.Projecttile)
                radius += (ushort)((float)radius * 2);

            //< 조건이 되는 모든 리스트를 구한다.
            List<SelectUnitData> _SelectUnitDatas = new List<SelectUnitData>();
            List<Unit> targetList = GetTargetList(_ability.applyTarget, _Caster);
            for (int i = 0; i < targetList.Count; i++)
            {
                //< 각도 체크
                if (_ability.angle > 0 && _ability.angle <= 360)
                {
                    if(SceneManager.instance.IsRTNetwork && (SkillType)_ability.skillType == SkillType.Projecttile)
                    {
                        //네트워크 임시 프로젝타일
                        if (!MathHelper.InAngle(_Caster.transform.forward.normalized, _Caster.transform.position, targetList[i].transform.position, 60))
                            continue;
                    }
                    else
                    {
                        if (!MathHelper.InAngle(_Caster.transform.forward.normalized, _Caster.transform.position, targetList[i].transform.position, _ability.angle))
                            continue;
                    }                    
                }

                //< 범위 체크
                //if (_ability.radius > 0)
                if (radius > 0)
                {
                    if (!MathHelper.IsInRange((targetList[i].transform.position - _Caster.transform.position), radius, _Caster.Radius, targetList[i].Radius))
                        continue;
                }

                SelectUnitData newData = new SelectUnitData();
                newData.magnitude = (targetList[i].transform.position - _Caster.transform.position).magnitude;
                newData.Owner = targetList[i];
                _SelectUnitDatas.Add(newData);
            }

            //< 제일 가까운 대상에게 사용한다.
            int limitTarget = _ability.targetCount == 0 ? 100 : (int)_ability.targetCount;

            //< 이펙트스킬이라면 이펙트카운트로 대입
            if ((SkillType)_ability.skillType == SkillType.EffectSkill)
                limitTarget = _ability.eCount;

            //멀티플레이가 아닐때만 여기서 처리 - 멀티플레이일경우 미스처리는 서버에서 한다
            if ( !SceneManager.instance.IsRTNetwork)
            {
                //일단 버프는 빗나가지않게만 처리 나중에 어케할지결정
                if( _ability.callBuffIdx == 0)
                {
                    //선택된애들중에 명중률 계산해서 미스나면 삭제
                    for (int i = _SelectUnitDatas.Count - 1; i >= 0; i--)
                    {
                        //float HitRate = _Caster.CharInfo.HitRate - _SelectUnitDatas[i].Owner.CharInfo.DodgeRate;
                        //bool isHit = (HitRate * 0.01f) >= Random.value;

						// ( (피격자의 회피 수치 – 공격자의 명중 수치) * Evade_f.Value1_f / (공격자의 명중 수치) ) + Evade_f.Value2_f
						// 만약 (피격자의 회피 수치 – 공격자의 명중 수치) < 0일때 0을 취함.
						// 만약 회피 확률 < 0 일때，회피 확율 = 0
						// 만약 회피 확률 > EvadeC_f.Value1_f 일떄 회피확률 = EvadeC_f.Value1_f
						// 2017.8.24. kyh

                        //새 명중률 계산
                        float DodgeRate = 0;
                        if(_SelectUnitDatas[i].Owner.CharInfo.DodgeRate - _Caster.CharInfo.HitRate < 0)
                        {
                            DodgeRate = 0;
                        }
                        else
                        {
							// old
                            //DodgeRate = ((_SelectUnitDatas[i].Owner.CharInfo.DodgeRate - _Caster.CharInfo.HitRate) / (500 + _Caster.CharInfo.HitRate));
							// new
							// ( (피격자의 회피 수치 – 공격자의 명중 수치) * Evade_f.Value1_f / (공격자의 명중 수치) ) + Evade_f.Value2_f
							// 2017.8.24. kyh
							float evade1 = _LowDataMgr.instance.GetFormula(FormulaType.EVADE, 1);
							float evade2 = _LowDataMgr.instance.GetFormula(FormulaType.EVADE, 2);
							DodgeRate = ((_SelectUnitDatas[i].Owner.CharInfo.DodgeRate - _Caster.CharInfo.HitRate) * evade1 / _Caster.CharInfo.HitRate) + evade2;
                        }

                        if(DodgeRate > _LowDataMgr.instance.GetFormula(FormulaType.EVADE_CAP, 1))
                        {
                            DodgeRate = _LowDataMgr.instance.GetFormula(FormulaType.EVADE_CAP, 1);
                        }

                        bool isEvade = DodgeRate >= Random.value;

                        //회피했다 
                        if (isEvade)
                        {
                            //현재 값을 지우고 머리위에 회피를 띄우자
                            G_GameInfo.GameInfo.BoardPanel.ShowBuff(_SelectUnitDatas[i].Owner.gameObject, _Caster.gameObject, _LowDataMgr.instance.GetStringCommon(21));
                            _SelectUnitDatas.Remove(_SelectUnitDatas[i]);

                        }
                    }
                }
            }

            _SelectUnitDatas.Sort(delegate (SelectUnitData tmp1, SelectUnitData tmp2) { return tmp1.magnitude.CompareTo(tmp2.magnitude); });

            List<Unit> targetResult = new List<Unit>();

            //타겟제한에 따라 공격을 하니 네트워크 모드 일경우 여기부터 리스트를 뽑아서 따로 보내주자
            for (int i = 0; i < _SelectUnitDatas.Count; i++)
            {
                //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
                if (SceneManager.instance.IsRTNetwork)
                {
                    targetResult.Add(_SelectUnitDatas[i].Owner); //<---단순 리스트만 받아간다.
                }
                else
                {
                    //< 스킬 사용
                    ApplySkill((SkillType)_ability.skillType, _ability, _Caster, _SelectUnitDatas[i].Owner, normal);
                }

                limitTarget--;
                if (limitTarget <= 0)
                    break;
            }

            if (SceneManager.instance.IsRTNetwork)
            {
                //멀티플레이이고 이게 내 유닛이라면 서버로 데미지 계산 요청
                if (_Caster.m_rUUID == G_GameInfo.PlayerController.Leader.m_rUUID || _Caster.IsPartner == true && _Caster.TeamID == G_GameInfo.PlayerController.Leader.TeamID)
                {
                    if (targetResult.Count == 0)
                        return;
                    
                    List<Sw.TargetInfo> targets = new List<Sw.TargetInfo>();
                    targets.Clear();

                    for(int i=0;i<targetResult.Count;i++)
                    {
                        var Target = new Sw.TargetInfo();

                        if (targetResult[i].UnitType == UnitType.Npc || targetResult[i].UnitType == UnitType.Boss)
                            Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_NPC;
                        else if (targetResult[i].UnitType == UnitType.Unit && !targetResult[i].IsPartner)
                            Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_USER;
                        else if (targetResult[i].UnitType == UnitType.Unit && targetResult[i].IsPartner)
                            Target.UnTargetType = (int)Sw.ROLE_TYPE.ROLE_TYPE_HERO;

                        Target.UllTargetId = (long)targetResult[i].m_rUUID;

                        Vector3 my = _Caster.cachedTransform.position;
                        Vector3 target = targetResult[i].transform.position;
                        my.y = target.y = 0;

                        Vector3 pushingDir = (target - my).normalized * 3f; //(transform.position - attackerGO.transform.position).normalized;
                        target = target + pushingDir;

                        Vector3 MinPos = NaviTileInfo.instance.GetMinPos();
                        Target.UnPosX = (int)(Mathf.Abs(MinPos.x - target.x) / NaviTileInfo.instance.GetTileSize());
                        Target.UnPosY = (int)(Mathf.Abs(MinPos.z - target.z) / NaviTileInfo.instance.GetTileSize());
                        Target.UnPush = 1;

                        targets.Add(Target);

                    }

                    //어택일경우는 데미지 체크를 보냄
                    if ((SkillType)_ability.skillType == SkillType.Attack || (SkillType)_ability.skillType == SkillType.AttackBuff)
                    {
                        //데미지일 경우
                        //iFunClient.instance.ReqDamage(_Caster.m_rUUID, _LowDataMgr.GetSkillAction(_ability.Idx).idx, _ability.notiIdx, _Caster.transform.position, _Caster.transform.eulerAngles.y, a_list);
                        NetworkClient.instance.SendPMsgRoleAttackC((int)_LowDataMgr.GetSkillAction(_ability.Idx).idx, 0, (int)_ability.notiIdx, 1, ref targets);
                    }
                    else if ((SkillType)_ability.skillType == SkillType.Projecttile)
                    {
                        //프로젝타일 일경우
                        //iFunClient.instance.ReqDamage(_Caster.m_rUUID, _LowDataMgr.GetSkillAction(_ability.Idx).idx, _ability.notiIdx, _Caster.transform.position, _Caster.transform.eulerAngles.y, a_list);
                    }
                    else if ((SkillType)_ability.skillType == SkillType.Buff)
                    {
                        //버프일 경우
                        //iFunClient.instance.ReqBuff(_Caster.m_rUUID, _LowDataMgr.GetSkillAction(_ability.Idx).idx, _ability.notiIdx);
                    }
                    else if ((SkillType)_ability.skillType == SkillType.EffectSkill)
                    {
                        //장판스킬일경우 - 없다는데 봐야알지
                    }
                    else if ((SkillType)_ability.skillType == SkillType.Heal)
                    {
                        //힐스킬일경우 - 없다는데 봐야알지
                    }
                }
            }                
        }
    }

    //< 실질적 스킬 적용
    public static void ApplySkill(SkillType skillType, AbilityData _ability, Unit _Caster, Unit _Target, bool normal = false)
    {
        float affectValue = CalcAffectValue(_ability, _Caster, _Target);

        switch (skillType)
        {
            case SkillType.Attack:      //일반 공격
            case SkillType.AttackBuff:
                if (_Target != null)
                {
                    //SkillTables.ActionInfo _action = _LowDataMgr.GetSkillAction(_ability.Idx);

                    //슈퍼아머 계산은 먼저 해야함
                    //if (_ability != null)
                    //{
                    //    uint beforeSuperArmor = _Target.CharInfo.SuperArmor;
                    //    //슈퍼아머값 감소
                    //    if (_Target.CharInfo.SuperArmor <= _ability.superArmorDmg)
                    //    {
                    //        _Target.CharInfo.SuperArmor = 0;

                    //        //0일경우만 회복을 시키고 아닐경우는 무시하자 
                    //        if (_Target.SuperRecoveryTick == 0f)
                    //        {
                    //            if(beforeSuperArmor!=0)
                    //            {
                    //                Transform effTrans = G_GameInfo.SpawnEffect("Fx_SuperArmor_Delete", _Target.cachedTransform.position, Quaternion.Euler(Vector3.zero));

                    //                if (effTrans != null)
                    //                {
                    //                    FxMakerPoolItem poolItem = effTrans.GetComponent<FxMakerPoolItem>();
                    //                    if (poolItem == null)
                    //                    {
                    //                        poolItem = effTrans.gameObject.AddComponent<FxMakerPoolItem>();
                    //                        poolItem.destroyTime = 10;
                    //                    }

                    //                    poolItem.Owner = _Target;
                    //                    poolItem.SetAttach(_Target.transform);
                    //                }

                    //                SoundManager.instance.PlaySfxUnitSound(0, _Target._audioSource, _Target.cachedTransform, true);
                    //            }

                    //            _Target.SuperRecoveryTick = _Target.CharInfo.SuperArmor_RecoveryTime;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        _Target.CharInfo.SuperArmor = _Target.CharInfo.SuperArmor - _ability.superArmorDmg;
                    //    }

                    //    //공격자측의 슈퍼아머 회복
                    //    _Caster.CharInfo.SuperArmor += _ability.superArmorRecovery;
                    //    _Caster.CharInfo.SuperArmor = (uint)Mathf.Clamp(_Caster.CharInfo.SuperArmor, 0, _Caster.CharInfo.MaxSuperArmor);
                    //}
                    //if (_ability.callBuffIdx != 0)
                    //{
                    //    if (_Target != null && _Target.BuffCtlr != null)
                    //    {
                    //        _Target.BuffCtlr.AttachBuff(_Caster, _Target, _Caster.SkillCtlr.__SkillGroupInfo.GetBuff(_ability.callBuffIdx, _ability.Idx), _ability.rate, _ability.durationTime);
                    //    }
                    //}

                    _Target.TakeDamage(_Caster, 1.0f, affectValue, _ability.ignoreDef, eAttackType.All, !normal, _ability, false, false);
                }
                break;

            case SkillType.Projecttile: //발사체 발사  

                SkillTables.ProjectTileInfo _projecttiledata = _LowDataMgr.GetSkillProjectTileData(_ability.callAbilityIdx);
                Vector3 dirVec = _Caster.transform.forward;
                if (_projecttiledata.startType == 3)
                {
                    Transform[] temp = _Caster.GetComponentsInChildren<Transform>(true);
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i] != null && temp[i].name.Equals("cast_dummy_01"))
                        {
                            Vector3 temp1 = _Caster.cachedTransform.position;
                            Vector3 temp2 = temp[i].position;

                            temp1.y = 0;
                            temp2.y = 0;

                            Vector3 temppos = temp1 - temp2;
                            float distance = temppos.magnitude;
                            dirVec = temppos / distance;
                            break;
                        }
                    }
                }

                //< 발사체 생성
                if (SceneManager.instance.IsRTNetwork && (_Caster.m_rUUID == NetData.instance._userInfo._charUUID))
                {
                    //발사체 생성 서버로 보내주자
                    NetworkClient.instance.SendPMsgBattleAddProjectTileC((int)_ability.Idx, _ability.notiIdx, (int)_ability.callAbilityIdx, _Caster.BeforePosX, _Caster.BeforePosY, _Caster.BeforePosX, _Caster.BeforePosY, 10f, 10f, dirVec);
                    break;
                }
                else
                {
                    _Caster.SpawnProjectile(_LowDataMgr.GetSkillProjectTileData(_ability.callAbilityIdx), (int)affectValue, _Caster.TeamID, dirVec, _Caster, _Target, _ability, 0, normal);
                    break;
                }

            case SkillType.Buff:        //< 버프 생성
                //황비홍 프로젝트 - 버프일단 무시
                if (_Target != null && _Target.BuffCtlr != null)
                {
                    _Target.BuffCtlr.AttachBuff(_Caster, _Target, _Caster.SkillCtlr.__SkillGroupInfo.GetBuff(_ability.callBuffIdx, _ability.Idx), _ability.rate, _ability.durationTime);

                    //< 주변적들에게 모두 넉백(자신에게 사용하는 버프만)
                    if (_Target == _Caster)
                    {
                        List<Unit> targetList = GetTargetList(1, _Caster);
                        SkillTables.ActionInfo _action = _LowDataMgr.GetSkillAction(_ability.Idx);
                        float radius = (ushort)(_ability.radius + _action.range);
                        for (int i = 0; i < targetList.Count; i++)
                        {
                            if (targetList[i] == null || targetList[i].IsDie)
                                continue;

                            if (!MathHelper.IsInRange((targetList[i].transform.position - _Caster.transform.position), radius, _Caster.Radius, targetList[i].Radius))
                                continue;

                            targetList[i].SetPush(_ability.pushpower, _Caster.gameObject, 1);
                        }
                    }
                }
                break;
                

            case SkillType.Heal:        //체력 회복 또는 감소
                if (affectValue > 0 && _Target != null)
                {
                    _Target.SetHp(_Caster, affectValue);
                }
                break;

            case SkillType.EffectSkill: //< 해당 적 위치에 이펙트 바로 호출하여 공격
                //황비홍 프로젝트 - 이펙트 스킬 일단 무시
                /*
                if (_Target != null && _ability.targetEffect != "0")
                {
                    //if (SimulationGameInfo.SimulationGameCheck)
                    //    UnitSimulation.UseSkillNames.Add(_ability.targetEffect);

                    _Target.SpawnEffect(_ability.targetEffect, 1, _Target.transform, null, false, (skillCastingEff) =>
                    {
                        //< 사운드 실행
                        if (_ability.targetSound > 0)
                            SoundHelper.PlaySfxSound(_ability.targetSound, 1.5f);

                        //< 각도만 캐스터 방향으로 보정해준다
                        skillCastingEff.transform.localRotation = _Caster.transform.localRotation;

                        // 1. 이펙트 자식중에 충돌체가 있는지 찾도록 한다.
                        Collider[] existCol = skillCastingEff.GetComponentsInChildren<Collider>(true);
                        if (null != existCol)
                        {
                            for (int j = 0; j < existCol.Length; j++)
                                SetTriggerEvent(existCol[j], _Caster, affectValue, _ability);
                        }

                        //< 이펙트에 듀플리케이트가 있을경우 연결해줌
                        NcDuplicator _NcDuplicator = skillCastingEff.GetComponentInChildren<NcDuplicator>();
                        if (_NcDuplicator != null)
                        {
                            _NcDuplicator._CallBack = (trn) =>
                            {
                                Collider[] existCol2 = trn.gameObject.GetComponentsInChildren<Collider>(true);
                                if (null != existCol2)
                                {
                                    for (int j = 0; j < existCol2.Length; j++)
                                        SetTriggerEvent(existCol2[j], _Caster, affectValue, _ability);
                                }
                            };
                        }
                    }, _Caster.Model.Main.transform.localScale.x);
                }
                */
                break;
        }
    }

    static void SetTriggerEvent(Collider existCol, Unit _Caster, float affectValue, AbilityData _ability)
    {
        //< 있을경우는 삭제
        ForwardTriggerEvent OrgtriggerEvt = existCol.gameObject.GetComponent<ForwardTriggerEvent>();
        if (OrgtriggerEvt != null)
            UIMgr.Destroy(OrgtriggerEvt);

        existCol.isTrigger = false;
        existCol.isTrigger = true;
        ForwardTriggerEvent triggerEvt = existCol.gameObject.AddComponent<ForwardTriggerEvent>();
        triggerEvt.Setup(_Caster, _ability);
        triggerEvt.TriggerEnter_Unit = (target, ForwardTriggerEvent) =>
        {
            if (target != null)
            {
                //< 대미지
                target.TakeDamage(_Caster, 1.0f, affectValue, 0, eAttackType.All, true, null);


                //황비홍 프로젝트에 맞게 수정 버프스킬은 일단 무시
                /*
                //< 버프 호출이있을시 버프호출
                if (_ability.callBuffIdx != 0 && target.BuffCtlr != null)
                    //황비홍 프로젝트에 맞게 수정
                    target.BuffCtlr.AttachBuff(_Caster, target, _Caster.SkillCtlr._SkillGroupInfo.GetBuff(_ability.callBuffIdx, _ability.unitIdx, _ability.skillIdx, _Caster.UnitType == UnitType.Unit ? (_Caster as Pc).syncData.SkillLvDatas : null), _ability);
                */
                
            }
        };
    }

    //< 버프로 인한 스킬 적용
    public static bool BuffApplySkill(bool Apply, SkillTables.BuffInfo _info, Unit _Caster, Unit _Target, float _affectValue, Buff _buff)
    {
        if(SceneManager.instance.IsRTNetwork)
        {
            //네트워크상태에선 캐스터가 없다
            if ( _Target == null || !_Target.gameObject.activeSelf)
                return false;
        }
        else
        {
            if (_Caster == null || _Target == null || !_Caster.gameObject.activeSelf || !_Target.gameObject.activeSelf)
                return false;
        }

        if (!_Target.CharInfo.BuffValue.ContainsKey((BuffType)_info.buffType))
            _Target.CharInfo.BuffValue.Add((BuffType)_info.buffType, 0);

        switch((BuffType)_info.buffType)
        {
            //< 수치 타입
            case BuffType.DAMAGE_INCREASE:
            case BuffType.ATTACKSPEED_INCREASE:
            case BuffType.DAMAGEREDUCE_INCRESE:
            case BuffType.DAMAGEREDUCEPER_INCREASE:
            case BuffType.CRITICALRATE_INCREASE:
            case BuffType.CRITICALDAMAGE_INCREASE:
            case BuffType.CRITICALRES_INCREASE:
            case BuffType.HITRATE_INCREASE:
            case BuffType.DODGERATE_INCREASE:
            case BuffType.LIFESTEAL_INCREASE:
            case BuffType.ANGLEDEFUP:
                if (Apply) _Target.CharInfo.BuffValue[(BuffType)_info.buffType] += _affectValue;
                else _Target.CharInfo.BuffValue[(BuffType)_info.buffType] -= _affectValue;
                break;

            case BuffType.Knockback:
            //case BuffType.Freeze:
            //case BuffType.StoneCurse:
            case BuffType.Stun:
            case BuffType.Down:
                bool success = _Target.SetUnitState(Apply, _info.buffType, _info.factorRate, _Caster);
                if (Apply && !success)
                    return false;

                break;

            //< 즉시적용
            case BuffType.BurnDot:
            case BuffType.PoisoningDot:
                //카메라 푸쉬등의 어빌리티에서 가져오는 데이터가 없으므로 그냥 적용
                if (!SceneManager.instance.IsRTNetwork)
                    if (Apply) _Target.TakeDamage(_Caster, 1, _affectValue, 0, eAttackType.All, true, null );
                break;

                /*
                case BuffType.Attack:
                case BuffType.Def:
                case BuffType.CriticalRate:
                case BuffType.CriticalDam:

                case BuffType.NatureAttack:
                case BuffType.PoisonAttack:
                case BuffType.WaterAttack:
                case BuffType.MetalAttack:
                case BuffType.FireAttack:
                case BuffType.HollyAttack:
                case BuffType.DarkAttack:

                case BuffType.AttackSpeed:
                case BuffType.IgnoreDef:
                    if (Apply)  _Target.CharInfo.BuffValue[(BuffType)_info.buffType] += _affectValue;
                    else        _Target.CharInfo.BuffValue[(BuffType)_info.buffType] -= _affectValue;
                    break;

                    //< 얘는 버프벨류로 체크안함.
                case BuffType.Shield:
                    break;

                    //< MaxHP 적용
                case BuffType.MaxHp:
                    //< 현재 MaxHP를 얻음
                    int NowMaxHp = _Target.CharInfo.MaxHp;

                    //< 값 적용
                    if (Apply)  _Target.CharInfo.BuffValue[(BuffType)_info.buffType] += _affectValue;
                    else        _Target.CharInfo.BuffValue[(BuffType)_info.buffType] -= _affectValue;

                    //< 증가된후의 차액을 얻음
                    int MaxHpValue = _Target.CharInfo.MaxHp - NowMaxHp;

                    //< 맥스HP가 증가되는거라면 증가되는 수치만큼 현재체력도 증가시켜줌
                    if (MaxHpValue > 0)
                        _Target.CharInfo.Hp += (int)MaxHpValue;
                    else
                    {
                        if (_Target.CharInfo.Hp > _Target.CharInfo.MaxHp)
                            _Target.CharInfo.Hp = _Target.CharInfo.MaxHp;
                    }

                    _Target.hp = _Target.CharInfo.Hp;
                    break;


                case BuffType.AllImmune:
                case BuffType.AttackImmune:
                case BuffType.SkillImmune:
                    _Target.CharInfo.BuffValue[(BuffType)_info.buffType] = Apply ? 1 : 0;
                    break;

                    //< 즉시적용(힐)
                case BuffType.Heal:
                case BuffType.HealDot:
                    if(Apply)   _Target.SetHp(_Caster, _affectValue);
                    break;

              //< 상태이상
                

                    //< 분신소환
                //case BuffType.Clone:
                //    if (Apply)  _buff.AddUnit(_Caster, (int)_buff.buffInfo.eventValue1);
                //    else        _buff.DeleteUnit();
                //    break;

                //    //< 유닛 소환
                //case BuffType.Summon:
                //    if (Apply)  _buff.AddEnemy(_Caster, (int)_buff.buffInfo.eventValue1, _buff.buffInfo.eventValue2);
                //    else        _buff.DeleteUnit();
                //    break;

                    //< 광폭화
                case BuffType.Berserker:
                    break;

                    //< 스플래쉬 대미지
                case BuffType.Splash:
                    if (Apply)
                    {
                        if (_buff.ability != null)
                        {
                            //< 적의 리스트를 얻어온다.(스플래쉬는 자신을 주변으로 공격하는것이므로 타겟 기준으로 가져옴)
                            List<Unit> targetList = GetTargetList(1, _Target);
                            for (int i = 0; i < targetList.Count; i++)
                            {
                                if (targetList[i] == null || targetList[i].IsDie)
                                {
                                    //< 범위를 체크한다.
                                    Vector3 caster = _Target.transform.position;
                                    Vector3 target = targetList[i].transform.position;
                                    if ((target - caster).magnitude <= _buff.ability.radius)
                                        targetList[i].TakeDamage(_Target, 1, _affectValue, eAttackType.All, true);
                                }
                            }
                        }
                    }
                    break;
                    */
        }

        return true;
    }
}

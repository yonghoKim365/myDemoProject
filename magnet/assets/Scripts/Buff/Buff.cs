using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buff
{
    public int AccrueCount = 0;     //< 현재 버프 누적개수

    BuffController controller;
    public SkillTables.BuffInfo buffInfo;
    //public AbilityData ability;
    public BuffDurationType BuffDurType;
    float BuffDuration;

    float affectValue;                  //< 적용시켜야할 값
    public float affectedValue = 0;     //< 버프로 토탈 인해 적용시킨 값

    public Unit Caster, Target = null;
    public Transform Effgo;


    public string BuffIcon = "";
    float LifeTime = 0, TickTime = 0;

    public List<Unit> UnitList; //< 분신, 소환등에 사용되는 리스트객체

    bool Live = false;
    
    public void OnAttach(BuffController _controller, Unit _Caster, Unit _Target, SkillTables.BuffInfo _buffInfo, float _affectValue, BuffDurationType _Type, float _BuffDuration)
    {
        controller = _controller;
        buffInfo = _buffInfo;
        //ability = _ability;
        BuffDurType = _Type;
        BuffDuration = _BuffDuration;
        Caster = _Caster;
        Target = _Target;

        byte overLapCount = _buffInfo.overLapCount;
        if (overLapCount == 0)
            overLapCount = 1;

        //< 중첩 카운트 체크
        if (overLapCount <= AccrueCount)
        {
            //BuffController.BuffInfoStringShow(_Caster, _Target, buffInfo, "(Max)");
            return;
        }

        //< 능력치부터 적용
        AffectBuff(_affectValue, (type) =>
        {
            if (!type)
            {
                //< 적용부터 실패했다면 바로 버프를 삭제시켜줌
                controller.DetachBuff(this, true);
                BuffController.BuffInfoStringShow(_Caster, _Target, buffInfo, "抵抗");
                return;
            }

            //< 머리위에 띄워주기위함
            BuffController.BuffInfoStringShow(_Caster, _Target, buffInfo, "");

            //< 값 누적
            affectedValue += _affectValue;

            //< 데이터 적용
            AccrueCount++;
            affectValue = _affectValue;

            //< 무적관련 버프일경우에는 이미 버프중에 무적이 걸려있을때에 이전 이펙트를 삭제시켜준다.
            switch ((BuffType)_buffInfo.buffType)
            {
                case BuffType.AllImmune:
                case BuffType.AttackImmune:
                case BuffType.SkillImmune:
                    controller.SetImmuneEffect(false);
                    break;
            }

            Live = true;
            
            //< 버프에따른 이펙트를 붙인다
            SetEffect(true);

            //< 버프 아이콘 이름 대입
            if(BuffIcon == "")
            {
                UnitStateBase _UnitStateBase = null;
                if (Caster != null && Caster.FSM.GetState(UnitState.Skill, out _UnitStateBase) && _UnitStateBase is SkillState && (_UnitStateBase as SkillState).actionData != null)
                    BuffIcon = _LowDataMgr.instance.GetLowDataIcon( (_UnitStateBase as SkillState).actionData.Icon );
                else
                    BuffIcon = buffInfo.icon;
            }
            
            //< 버프 유지시간 대입
            LifeTime = _BuffDuration;

            //< 틱 시간 대입
            TickTime = buffInfo.tic;

            //if (SkillAbility.SkillDebugMode && _Caster.UnitType == UnitType.Unit && SimulationGameInfo.SimulationGameCheck)
            //    Debug.Log("버프 생성 : " + (BuffType)_buffInfo.buffType + " , LifeTime " + LifeTime + " , TickTime" + TickTime, Target.gameObject);
        });
    }

    //< 외부에서 업데이트를 시켜준다.
    public void BuffUpdate()
    {
        if (!Live)
            return;

        //맹장특수스킬 예외처리
        if(BuffDurType == BuffDurationType.Normal || BuffDurType == BuffDurationType.P_Extra)
        {
            //< 버프 라이프시간 체크
            LifeTime -= Time.deltaTime;
            if (LifeTime <= 0)
            {
                DetachBuff();

                if(BuffDurType == BuffDurationType.P_Extra)
                {
                    //맹장의 경우 임시 아이들로 처리
                    if(Target.CurrentState == UnitState.Idle)
                    {
                        Target.PlayAnim(eAnimName.Anim_battle_idle, true, 0.08f, true);
                    }
                    else
                    {
                        Target.ChangeState(UnitState.Idle);
                    }
                }

                return;
            }

        }

        //< 틱 업데이트
        if (TickTime > 0)
        {
            TickTime -= Time.deltaTime;
            if(TickTime <= 0)
            {
                //어빌리티 적용
                AffectBuff(affectValue, (type) =>
                {
                    if (type)
                    {
                        //< 값 누적
                        affectedValue += affectValue;

                        //< 다시 갱신
                        TickTime = buffInfo.tic;
                    }
                });
            }
        }
    }

    //< 버프 적용   
    void AffectBuff(float affectValue, System.Action<bool> _callback)
    {
	   //< 스턴일경우에는 체크
	   if ((BuffType)buffInfo.buffType == BuffType.Stun)
	   {
		  //< 상대가 푸쉬 상태라면 대기하고있음
		  //if (Target.FSM.Current_State == UnitState.Push)
		  //{
			 //Caster.StartCoroutine(PushReadyUpdate(() =>
			 //{
				//_callback(SkillAbility.BuffApplySkill(true, buffInfo, Caster, Target, affectValue, this));
			 //}));
		  //}
		  //else
			 _callback(SkillAbility.BuffApplySkill(true, buffInfo, Caster, Target, affectValue, this));
	   }
	   else
		  _callback(SkillAbility.BuffApplySkill(true, buffInfo, Caster, Target, affectValue, this));
    }

    IEnumerator PushReadyUpdate(System.Action call)
    {
        while(true)
        {
            if (Target.FSM.Current_State != UnitState.Push)
                break;

            yield return null;
        }

        call();
    }

    //< 버프를 완전히 제거 한다.
    public void DetachBuff()
    {
        Live = false;

        //< 적용된 능력 제거
        SkillAbility.BuffApplySkill(false, buffInfo, Caster, Target, affectedValue, this);
        DeleteUnit();

        //< 버프컨트롤러에서 삭제
        controller.DetachBuff(this);

        //< 이펙트가 있을시 삭제
        SetEffect(false);

        //< 무적관련 버프일경우에는 남아있는 무적 버프가있을시 이펙트를 켜준다.
        switch ((BuffType)buffInfo.buffType)
        {
            case BuffType.AllImmune:
            case BuffType.AttackImmune:
            case BuffType.SkillImmune:
                controller.SetImmuneEffect(true);
                break;
        }
    }

    public void SetEffect(bool type)
    {
        //< 이펙트를 켜준다.
        if(type)
        {
            string prefabName = buffInfo.effect;
            if (prefabName != null && prefabName != "" && prefabName != "0" && Target.Usable && Effgo == null)
            {
                Target.SpawnEffect(prefabName, 1, Target.transform, Target.transform, false, (eff) =>
                {
                    if (eff != null && Live && Target != null && !Target.IsDie)
                    {
                        Effgo = eff;

                        //< 타입이 스턴이라면 머리위로 보정
                        //if(buffInfo.buffType == (byte)BuffType.Stun)
                        if(buffInfo.startType == 1)
                        {
                            Collider _Collider = Target.gameObject.GetComponent<Collider>();
                            if(_Collider != null)
                                Effgo.transform.position = new Vector3(
                                    Target.transform.position.x, 
                                    _Collider.bounds.max.y + 0.5f, 
                                    Target.transform.position.z);
                        }
                        else if (buffInfo.startType == 2)
                        {
                            Collider _Collider = Target.gameObject.GetComponent<Collider>();
                            if (_Collider != null)
                                Effgo.transform.position = new Vector3(
                                    Target.transform.position.x,
                                    Target.transform.position.y + _Collider.bounds.max.y / 2f,
                                    Target.transform.position.z);
                        }
                    }
                    else
                    {
                        Effgo = eff;
                        SetEffect(false);
                    }
                });
            }
        }
        //< 이펙트를 꺼준다
        else if (!type)
        {
            if (Effgo != null)
            {
                FxMakerPoolItem poolItem = Effgo.GetComponent<FxMakerPoolItem>();
                if (poolItem == null)
                {
                    poolItem = Effgo.gameObject.AddComponent<FxMakerPoolItem>();
                    poolItem.Owner = Caster;
                    poolItem.destroyTime = 1;
                }
                else
                    poolItem.ManualDespawn();
            }
        }
        
    }

    //< 나와 같은 유닛을 추가해준다.(분신)
    public void AddUnit(Unit _Caster, int count)
    {
        //황비홍 프로젝트 - 소환물 없음
        //if (count == 0)
        //    return;

        //if (_Caster.UnitType == UnitType.Unit)
        //{
        //    TempCoroutine.instance.FrameDelay(0.05f, () =>
        //    {
        //        GameObject npcGo = G_GameInfo.GameInfo.SpawnUnit(G_GameInfo._GameInfo.FindOwnerPlayerController(), 0, ((Pc)_Caster).syncData.CopyData(), _Caster.transform.position, _Caster.transform.rotation);

        //        Unit unit = npcGo.GetComponent<Unit>();
        //        unit.CharInfo.SetLevel(_Caster.CharInfo.Level);
        //        unit.CharInfo.SetStatusPercent(affectValue);
        //        unit.SetClone(_Caster);
        //        unit.StaticState(false);
        //        UnitList.Add(unit);

        //        TempCoroutine.instance.StartCoroutine(G_GameInfo._GameInfo.FindOwnerPlayerController().SetUnitSpawnPos(_Caster, unit, () =>
        //        {
        //            //< 이펙트 띄워줌
        //            G_GameInfo.SpawnEffect("Fx_Monster_Regen_04", 1, npcGo.transform, npcGo.transform, Vector3.one);

        //            //< 다시소환
        //            AddUnit(_Caster, --count);
        //        }));
        //    });
        //}
        //else
        //{
        //    Npc enemy = (Npc)_Caster;
        //    AddEnemy(_Caster, count, enemy.NpcLowID);
        //}
        
    }

    //< 새로운 유닛을 소환한다.
    public void AddEnemy(Unit _Caster, int count, uint LowID)
    {
        //황비홍 프로젝트 - 소환물 없음
        //if (count == 0)
        //    return;

        ////< NPC일경우에는 그냥 아이디로 바로 생성해주면 된다.
        //TempCoroutine.instance.FrameDelay(0.05f, () =>
        //{

        //    //레벨은 임의로 1을 넣어줌 
        //    GameObject npcGo = G_GameInfo.GameInfo.SpawnNpc((uint)LowID, (eTeamType)_Caster.TeamID, 1, _Caster.GroupNo, _Caster.transform.position, _Caster.transform.rotation, false);

        //    Npc _Npc = npcGo.GetComponent<Npc>();
        //    _Npc.CharInfo.SetLevel(_Caster.CharInfo.Level);
        //    _Npc.CharInfo.SetStatusPercent(affectValue);
        //    _Npc.SetClone(_Caster);
        //    _Npc.StaticState(false);
        //    UnitList.Add(_Npc);

        //    TempCoroutine.instance.StartCoroutine(G_GameInfo._GameInfo.FindOwnerPlayerController().SetUnitSpawnPos(_Caster, _Npc, () =>
        //    {
        //        //< 이펙트 띄워줌
        //        G_GameInfo.SpawnEffect("Fx_Monster_Regen_04", 1, npcGo.transform, npcGo.transform, Vector3.one);

        //        //< 다시소환
        //        AddEnemy(_Caster, --count, LowID);
        //    }));
        //});
    }

    //< 소환되어진 유닛이 있다면 파괴하기위함
    public void DeleteUnit()
    {
        if (UnitList == null)
            return;

        for (int i = 0; i < UnitList.Count; i++)
        {
            if (UnitList[i] != null)
            {
                UnitList[i].EffectClear(true);
                G_GameInfo.SpawnEffect("Fx_Monster_RegenEnd_01", UnitList[i].transform.position, Quaternion.identity);
                G_GameInfo.CharacterMgr.RemoveUnit(UnitList[i]);
                TempCoroutine.Destroy(UnitList[i].gameObject);
            }
        }

        UnitList.Clear();
    }

    //< 보호막등에 사용하기위해 공격을 받을시 여길 들어옴
    public void TakeDamage(Unit unit, ref float Dam, bool IsSkill)
    {
    }

    public float GetBuffLifeTimePencent()
    {
        //< 현재 남은시간을 리턴해준다( Max 1 )
        if(BuffDurType == BuffDurationType.Normal)
        {
            float Pencent = LifeTime / BuffDuration;
            return 1 - Pencent;
        }
        else
        {
            return 1;
        }        
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EvasionState : UnitStateBase  //회피
{
    Vector3 TargetPos;

    //List<Unit> Unitlist;
    //Transform EffectObj;
    float EvasionLiveTime = 2;
    float MoveSpeed = 0;
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        //황비홍 프로젝트엔 EvasionState를 안씀

        ////< 혹시나 다른 이펙트가 있을수도있으므로 이펙트삭제
        //parent.EffectClear();
        //parent.PlayAnim(eAnimName.Anim_Evasion, false, 0, true);

        ////< 타 유닛들의 네브메쉬를 꺼준다
        //Unitlist = G_GameInfo.CharacterMgr.AliveList();
        //for (int i = 0; i < Unitlist.Count; i++)
        //{
        //    if (Unitlist[i].CurrentState != UnitState.Evasion && !Unitlist[i].StopState && Unitlist[i].CurrentState != UnitState.Evasion)
        //        Unitlist[i].ZeroNavAgentRedius(true);
        //}

        ////< 목표 위치를 구한다
        //GetTargetPos();

        ////< 애니메이션의 재생속도를 제어한다
        //AnimationState state = parent.Animator.Animation[parent.Animator.GetAnimName(eAnimName.Anim_Evasion)];
        //if (state != null)
        //{
        //    if (parent.resInfo.dodgeType == 2)
        //        MoveSpeed = state.speed = 1.5f;
        //    else
        //        MoveSpeed = state.speed = 1.2f;
        //}

        ////< 이펙트를 붙여준다
        //if (parent.resInfo.effectDodge != "" && parent.resInfo.effectDodge != "0")
        //{
        //    parent.SpawnEffect(parent.resInfo.effectDodge, 1, parent.transform, parent.transform, true, (eff) =>
        //    {
        //        if (eff == null || parent == null || Camera.main == null)
        //            return;

        //        //< 위치를 보정해준다
        //        if (parent.resInfo.dodgeType == 2)
        //        {
        //            //< 워프이므로 유닛의 중간값으로 보정
        //            eff.transform.position = new Vector3(
        //                eff.transform.position.x, 
        //                eff.transform.position.y + ((parent.Height * 0.5f) + (parent.isAir ? parent.CharInfo.AirHeight : 0)), 
        //                eff.transform.position.z);
        //        }

        //        eff.transform.position += (Camera.main.transform.position - parent.transform.position).normalized * 2f;

        //        EffectObj = eff;
        //    });
        //}

        ////< 이시간이 지나면 풀리도록처리
        //EvasionLiveTime = parent.Animator.GetAnimLength(eAnimName.Anim_Evasion);
        //EvasionLiveTime = EvasionLiveTime - (EvasionLiveTime * 0.2f);
        //EvasionLiveTime = Time.time + EvasionLiveTime - 0.1f;
    }

    IEnumerator UpdateMeshBreak()
    {
        while(true)
        {
            parent.Model.BakeMeshObject(0.2f);
            yield return new WaitForSeconds(0.15f);
        }
    }

    public override void CachedUpdate()
    {
        //< 앞으로 이동시켜준다.
        parent.MoveToPath(MoveSpeed);

        //< 시간이 지나면 종료
        if(EvasionLiveTime < Time.time)
            parent.ChangeState(UnitState.Idle);
    }

    void GetTargetPos()
    {
        TargetPos = parent.transform.position + (parent.transform.forward * 6);//구르기시 이동거리가 6(나중에 구르기가 들어간다면 테이블로 값 빼야하고 안들어간다면 다 지워도 무방한 스테이트)
        parent.CalculatePath(TargetPos);
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        //if (EffectObj != null)
        //    EffectObj.parent = null;

        ////< 네브메쉬를 켜준다
        ////parent.navAgent.radius = OrgNavMeshRadius;

        //for (int i = 0; i < Unitlist.Count; i++)
        //{
        //    if (!Unitlist[i].StopState && Unitlist[i].CurrentState != UnitState.Event)
        //        Unitlist[i].ZeroNavAgentRedius(false);
        //        //Unitlist[i].navAgent.enabled = true;
        //}

    }
}

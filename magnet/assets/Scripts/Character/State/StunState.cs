using UnityEngine;
using System.Collections;

public class StunState : UnitStateBase
{
    public BuffType NowSkillType;
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.EffectClear();

        startedPush = false;

        if (parent.BuffCtlr.GetTypeBuffCheck(BuffType.Down) || parent.BuffCtlr.GetTypeBuffCheck(BuffType.Knockback))
	    {
		    NowSkillType = parent.BuffCtlr.GetTypeBuffCheck(BuffType.Down) ? BuffType.Down : BuffType.Knockback;

		    if (NowSkillType == BuffType.Knockback)
		    {
			    //넉백이면 캐스터를 바라보도록 한다.
			    Unit caster = parent.BuffCtlr.GetTypeBuffCasterCheck(BuffType.Knockback);

                //네트워크용 임시
                if (caster != null)
                    parent.cachedTransform.LookAt(caster.cachedTransform);
		    }
		    //< 넘어져있는 애니메이션을 실행한다.

            if(NowSkillType == BuffType.Knockback)
		        parent.PlayAnim(eAnimName.Anim_down, true, 0.1f, true);
            else
                parent.PlayAnimNoRootMation(eAnimName.Anim_down, true, 0.1f, true);

            if (parent.Animator.GetAnimName(eAnimName.Anim_down) != "0" && parent.Animation[parent.Animator.GetAnimName(eAnimName.Anim_down)] != null)
			    parent.Animation[parent.Animator.GetAnimName(eAnimName.Anim_down)].speed = 1;

        }
        else
	    {
		    NowSkillType = BuffType.Stun;

		    //< 스턴 애니메이션을 실행한다.
		    parent.PlayAnim(eAnimName.Anim_stun, true, 0.1f, true);

            Resource.AniInfo aniInfo = parent.GetAniData(eAnimName.Anim_stun);
            SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, parent._audioSource, parent.cachedTransform);
        }

        if (parent.UnitType == UnitType.Unit && parent.IsLeader)
            EventListner.instance.TriggerEvent("UseSkill_Start", parent);

    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        EventListner.instance.TriggerEvent("UseSkill_End", parent);

    }

    public override void CachedUpdate()
    {
        if (!startedPush)
            return;

        if (Vector3.Distance(parent.cachedTransform.position, ArrivalPos) < (PushSpeed * Time.deltaTime))
        {

        }
        else
        {
            ArrivalLook = ArrivalPos - parent.cachedTransform.position;
            ArrivalLook.Normalize();
            parent.cachedTransform.Translate(ArrivalLook * (PushSpeed * Time.deltaTime), Space.World);
        }
    }

    Vector3 ArrivalPos, ArrivalLook;
    float PushSpeed;
    public void Push(float power)
    {
        if (NowSkillType != BuffType.Knockback)
            return;
        //{
            //Push(PushPower);
        //}

        Vector3 EndPos = parent.cachedTransform.position + ((-parent.cachedTransform.forward) * power);

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(EndPos, out navHit, Vector3.Distance(EndPos, parent.cachedTransform.position), 9))
        {
            ArrivalPos = navHit.position;
            Debug.Log("StunState StartPos:" + parent.cachedTransform.position + " EndPos:" + EndPos + " AdjustPos:" + ArrivalPos + " power: " + power);
        }
        else
        {
            ArrivalPos = parent.cachedTransform.position;
            Debug.LogError("StunState Faild Position StartPos:" + parent.cachedTransform.position + " EndPos:" + EndPos + " AdjustPos:" + ArrivalPos + " power: " + power);
        }

        //ArrivalPos = EndPos;
        ArrivalLook = ArrivalPos - parent.cachedTransform.position;
        ArrivalLook.Normalize();

        PushSpeed = power * 3f;
        startedPush = true;

    }


    //테스트용 위치로 날리기
    public void Push(float power, Vector3 targetPos)
    {
        if (NowSkillType != BuffType.Knockback)
            return;
        //{
        //Push(PushPower);
        //}

        //Vector3 EndPos = this.transform.position + ((-transform.forward) * power);

        Vector3 EndPos = targetPos;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(EndPos, out navHit, Vector3.Distance(EndPos, parent.cachedTransform.position), 9))
            EndPos = navHit.position;

        ArrivalPos = EndPos;
        ArrivalLook = ArrivalPos - parent.cachedTransform.position;
        ArrivalLook.Normalize();

        PushSpeed = power * 3f;
        startedPush = true;

    }
}

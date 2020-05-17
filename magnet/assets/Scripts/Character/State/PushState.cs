using UnityEngine;
using System.Collections;

public class PushState : UnitStateBase
{
    protected Vector3 velocity = Vector3.zero;
    protected float stateEndTime = 0;
    protected float pushEndTime = 0;
    protected UnitState nextState = UnitState.Idle;
    //protected Transform dustFx;

    protected float _stiffenTime = 0.15f;

    public override void OnEnter(System.Action callback)
    {
//	   Debug.LogWarning("222222222222222222222222222222222222 : " + parent.CurrentState + " : " + parent, parent);

	   base.OnEnter( callback );

        SetIdleAni = false;
        parent.GetComponent<NavMeshAgent>().updateRotation = false;

	   //< 이리 들어왔으면 이펙트가 시전중이었을수도있기에 다 꺼줌
	   parent.EffectClear();
    }

    /// <summary>
    /// 주어진 시간 동안 velocity 방향으로 pushPower 만큼 민다.
    /// </summary>
    public override void Push(eAnimName animName, Vector3 _velocity, float pushPower, float duration, float stiffenTime, UnitState _next = UnitState.Idle)
    {
        float pushDur = duration;
        _stiffenTime = stiffenTime;

        // 애니메이션 유무에 따라 그냥 밀리는지 설정
        if (animName != eAnimName.Anim_none)
        {
            if (parent.PlayAnim(animName, false, 0, true))
            {
                float speed = 0.7f;
                if (animName == eAnimName.Anim_damage)
                {
                    AnimationState state = parent.Animator.Animation[parent.Animator.GetAnimName(animName)];
                    state.speed = speed;
                }

                // 밀리는 시간만 조절하도록 한다.
                //pushDur = (parent.Animator.GetAnimLength(animName) + ((1 - speed) * parent.Animator.GetAnimLength(animName)));
            }
            else
            {
                pushDur = 0.25f; // 애니메이션이 없거나 플레이 실패시 고정시간 밀리기.
            }
        }

        if (animName == eAnimName.Anim_damage)
        {
            //데미지 표시일경우
            Resource.AniInfo aniInfo = parent.GetAniData(eAnimName.Anim_damage);
            SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, parent._audioSource, parent.cachedTransform);
        }
        else if (animName == eAnimName.Anim_down)
        {
            //데미지 표시일경우
            Resource.AniInfo aniInfo = parent.GetAniData(eAnimName.Anim_down);
            SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, parent._audioSource, parent.cachedTransform);
        }

        //< 이동하는 방향을 자신의 뒤로 설정
        Vector3 newForward = -_velocity;
        newForward.y = 0;
        parent.cachedTransform.forward = newForward.normalized;

        //< 도착할 위치 지정
        pushPower = pushPower * (1 / pushDur);
        velocity = _velocity.normalized * pushPower;

        //< 종료 시점 지정
        stateEndTime = pushEndTime = Time.time + pushDur;
        stateEndTime += stiffenTime;

        nextState = _next;
        startedPush = true;
    }

    public virtual bool EndState()
    {
        if (Time.time > stateEndTime)
        {
            parent.ChangeState( nextState );
            startedPush = false;
            return true;
        }

        return false;
    }

    bool SetIdleAni = false;
    public override void CachedUpdate()
    {
        if (!startedPush)
            return;

        if (EndState())
            return;

        if (Time.time < pushEndTime)
            parent.GetComponent<NavMeshAgent>().velocity = velocity;
        else if (!SetIdleAni)
            SetIdleAni = true;
    }
    
    public override void OnExit(System.Action callback)
    {
        parent.GetComponent<NavMeshAgent>().updateRotation = true;
        parent.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
        startedPush = false;

        base.OnExit( callback );
    }
}

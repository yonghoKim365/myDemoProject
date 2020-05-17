using UnityEngine;
using System.Collections;

public class FloatingState : PushState
{
    enum State
    {
        Begin,
        Floating,
        End,
        Done,
    }

    Transform targetTrans;
    AnimationState floatAnimState;    
    eAnimName animName;    

    State curInternalState = State.Begin;
    float curHeight;
    float floatingHeight; // 공중부양 높이
    float floatDuration; // 애니메이션 플레이시간을 제외한 공중부양 시간
    //new float stateEndTime = 0f;

    float animLength;    
    float halfAnimLength; // 애니메이션 길이의 반
    float time = 0f; // 시간 비율 계산용
    readonly float extraTime = 0.2f; // 바닥으로 떨어지고 나서 약간의 대기 시간.
    readonly float landingTime = 0.28f;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        targetTrans = parent.Model.Transform;
        curHeight = targetTrans.localPosition.y;

        curInternalState = State.Done;

        parent.SetMasteryEvent(4, parent);
    }

    public override void OnExit(System.Action callback)
    {
        animLength = 0;
        stateEndTime = 0;
        curHeight = 0;

        targetTrans.SetLocalY( 0f );

        parent.SetMasteryEvent(5, parent);

        base.OnExit( callback );
    }

    /// <summary>
    /// 뜨는시간, 떨어지는 시간을 제외한 체공시간 계산하기
    /// </summary>
    public void Floating(Vector3 direction, float power, float _height = 1.5f, float _duration = 0.0f)
    {
        if (curInternalState == State.Begin)
            return;

        animName = eAnimName.Anim_floating;

        parent.PlayAnim( animName, true, 0.08f, true );
        floatAnimState = parent.Animation[parent.Animator.GetAnimName( animName )];

        if (floatAnimState == null)
        {
            parent.ChangeState(UnitState.Idle);
            return;
        }

        //< 들어왔다면 이펙트는 삭제시켜줌
        parent.EffectClear(true);
        
        animLength = floatAnimState.length;
        halfAnimLength = animLength * 0.5f;
        stateEndTime = Time.time + animLength + _duration;

        // 회전시키기
        //direction.y = 0;
        //parent.transform.forward = -direction.normalized;

        // 푸시관련
        Push( Mathf.Max( 1f, power ), animLength - landingTime );
        
        // 상태 시작.
        curInternalState = State.Begin;
        floatingHeight = _height;
        floatDuration = _duration;
        curHeight = targetTrans.localPosition.y;
        time = 0f;
        nextState = UnitState.StandUp;
    }

    void Push(float power, float duration)
    {
        float force = power / duration; // 뜨는시간만큼만 날아가게
        velocity = -transform.forward * force;
        pushEndTime = Time.time + duration;
        startedPush = true;
    }
        
    public override bool EndState()
    {
        if (Time.time > stateEndTime)
        {
            parent.ChangeState( nextState );
            startedPush = false;
            floatAnimState = null;
            return true;
        }

        return false;
    }

    public override void CachedUpdate()
    {
        float t = time / landingTime;
        if (floatAnimState != null && (curInternalState == State.Begin || curInternalState == State.Floating))
        {
            time += Time.deltaTime;
            floatAnimState.normalizedTime = floatAnimState.normalizedTime >= 0.5f ? 0.5f : floatAnimState.normalizedTime;

            targetTrans.SetLocalY(Mathf.Lerp(curHeight, floatingHeight, t));
        }

        switch (curInternalState)
        {
            case State.Begin:
                {
                    // t = 빨리 띄우기 위한 factor
                    // normalizedTime은 애니메이션 반까지는 플레이하기 위한 조건
                    if (t >= 1 || floatAnimState.normalizedTime >= 0.25f)
                    {
                        //parent.Animation.Stop( floatAnimState.name );
                        curInternalState = State.Floating;

                        // 체공시간 + 추락시간동안 천천히 밀리도록함. (보류)
                        //Push( pushPower * 0.6f, floatDuration + halfAnimLength );

                        //< 애니메이션 변경(최소 유지시간은 있어야 변경하도록함)
                        if (floatDuration > 0.2f)
                            parent.PlayAnim(eAnimName.Anim_floating2, true, 0.2f, true);
                    }

                    //Debug.Log( curInternalState + " : " + floatAnimState.normalizedTime + " : " + floatAnimState.time + " : " + t );
                }
                break;

            case State.Floating:
                {
                    floatDuration -= Time.deltaTime;
                    if (floatDuration <= 0f)
                    {
                        curInternalState = State.End;
                        stateEndTime = Time.time + halfAnimLength + extraTime;
                        time = 0f;

                        if (parent.Animator.IsPlaying(parent.Animator.GetAnimName(eAnimName.Anim_floating2)))
                        {
                            floatAnimState.normalizedTime = 0.5f;
                            parent.PlayAnim(animName, true, 0.08f, true);
                        }
                    }

                    //Debug.Log( curInternalState + " : " + floatDuration );
                }
                break;

            case State.End:
                {
                    time += Time.deltaTime;
                    float t2 = time / (landingTime - 0.1f);

                    targetTrans.SetLocalY(Mathf.Lerp(floatingHeight, 0, t2));

                    //dustFx = G_GameInfo.EffectPool.Spawn("Fx_down_01", transform.position, transform.rotation, transform);

                    //Debug.Log( curInternalState + " : " + time + " : " + t );
                }
                break;
        }

        base.CachedUpdate();
    }
}

using UnityEngine;
using System.Collections;

//< 연출등에 사용되는 스테이트로 아무것도 안하는데 사용함
public class EventState : UnitStateBase
{
    float AniPlayTime = 0;
    //float RunTime = 0;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        //< 내가 사용했던 모든 이펙트 삭제
        //parent.EffectClear();
        parent.PlayAnim(eAnimName.Anim_intro);
        parent.Animator.PlayAnimationSound(eAnimName.Anim_intro);
        string effName = parent.Animator.GetAnimationEffect(eAnimName.Anim_intro);
        if(!string.IsNullOrEmpty(effName) )
            G_GameInfo.SpawnEffect(effName, parent.cachedTransform.position, Quaternion.Euler(parent.cachedTransform.eulerAngles) );
        
        AniPlayTime = parent.Animator.GetAnimLength(eAnimName.Anim_intro);
    }

    public override void CachedUpdate()
    {
        if(0 < AniPlayTime)
        {
            AniPlayTime -= Time.deltaTime;
            if (AniPlayTime <= 0)
                parent.ChangeState(UnitState.Idle);
        }
    }

    public override void OnExit(System.Action callback)
    {
        //bool changeIdle = parent.ChangeState(UnitState.Idle);
        base.OnExit(callback);
    }

}

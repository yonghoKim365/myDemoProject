using UnityEngine;
using System.Collections;

public class StandUpState : UnitStateBase
{
    float endTime = 0f;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.PlayAnim( eAnimName.Anim_stand, true, 0.4f );
        endTime = Time.time + parent.Animator.GetAnimLength( eAnimName.Anim_stand );
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        if (endTime <= Time.time)
        {
            parent.ChangeState( UnitState.Idle );
        }
    }
}
using UnityEngine;
using System.Collections;

public class Prop_IdleState : UnitStateBase
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.PlayAnim( eAnimName.Anim_idle, true, 0.1f );
    }

    public override void CachedUpdate()
    {
    }
}

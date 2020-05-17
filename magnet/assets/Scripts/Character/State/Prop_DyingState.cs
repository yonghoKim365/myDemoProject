using UnityEngine;
using System.Collections;

public class Prop_DyingState : UnitStateBase
{
    private float endTime = 0.0f;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.PlayAnim( eAnimName.Anim_die );        
        parent.CharInfo.Hp = 0;

        BoxCollider[] collider = this.gameObject.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < collider.Length; i++)
            collider[i].enabled = false;

        if (GameDefine.skillPushTest)
        {
            CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
            for (int i = 0; i < colider2.Length; i++)
                colider2[i].enabled = false;
        }

        endTime = parent.Animator.GetAnimLength( eAnimName.Anim_die ) + 0.5f;
    }

    public override void OnExit(System.Action callback)
    {
        if (parent.UsableNavAgent)
        {
            parent.GetComponent<NavMeshAgent>().Stop();
            parent.GetComponent<NavMeshAgent>().enabled = false;
        }

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        endTime -= Time.deltaTime;
        if (endTime <= 0)
        {
            parent.ChangeState( UnitState.Dead );
        }
    }
}

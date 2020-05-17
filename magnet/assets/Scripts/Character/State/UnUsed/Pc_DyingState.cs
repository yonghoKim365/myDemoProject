using UnityEngine;
using System.Collections;

public class Pc_DyingState : UnitStateBase
{
    private float endTime = 0.0f;
    private NavMeshAgent navAgent;
    private ObstacleAvoidanceType storedAvoidanceType;
    private int storedAvoidancePriority;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        //parent.PlayAnim( eAnimName.Anim_die, true, 0.08f );
        parent.PlayAnim(eAnimName.Anim_die, true, 0.2f, true);
        parent.CharInfo.Hp = 0;

        navAgent = parent.GetComponent<NavMeshAgent>();
        storedAvoidanceType = navAgent.obstacleAvoidanceType;
        storedAvoidancePriority = navAgent.avoidancePriority;
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navAgent.avoidancePriority = 99;

        //< 죽었을때 컬리더박스는 꺼줌
        BoxCollider[] collider = this.gameObject.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < collider.Length; i++)
            collider[i].enabled = false;

        if (GameDefine.skillPushTest)
        {
            CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
            for (int i = 0; i < colider2.Length; i++)
                colider2[i].enabled = false;
        }

        endTime = Time.time + parent.Animator.GetAnimLength( eAnimName.Anim_die ) + 0.5f;

        //< 혹시모르니 체크
        if (parent.dirIndicator != null)
            Destroy(parent.dirIndicator);
    }

    public override void OnExit(System.Action callback)
    {
        if (parent.UsableNavAgent)
        {
            navAgent.obstacleAvoidanceType = storedAvoidanceType;
            navAgent.avoidancePriority = storedAvoidancePriority;
            navAgent.Stop();
            navAgent.enabled = false;
        }

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        if (Time.time > endTime)
        {
            parent.ChangeState( UnitState.Dead );
        }
    }
}
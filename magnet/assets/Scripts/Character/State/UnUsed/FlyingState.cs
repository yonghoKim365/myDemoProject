using UnityEngine;
using System.Collections;

public class FlyingState : UnitStateBase
{
    float lerpDuration = 0.5f;
    float fromHeight;
    float targetHeight;
    float t = 0;

    UnitState nextState = UnitState.Idle;
    //NavMeshAgent navAgent;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        //navAgent = parent.GetComponent<NavMeshAgent>();
        //fromHeight = navAgent.baseOffset;
        fromHeight = parent.Model.Main.transform.localPosition.y;
        t = 0;
    }

    public override void OnExit(System.Action callback)
    {   
        //if (navAgent.baseOffset == 0f)
        if (parent.Model.Main.transform.localPosition.y == 0 )
        {
            parent.CharInfo.CurMoveType = MoveType.Ground;
        }
        else
        {
            parent.CharInfo.CurMoveType = MoveType.Air;
        }

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        if (!parent.UsableNavAgent)
            return;

        if (t < 1)
        {
            t += Time.deltaTime * ( 1f / lerpDuration );

            parent.Model.Main.transform.localPosition = new Vector3(0, Mathf.Lerp(fromHeight, targetHeight, Mathf.Clamp01(t)), 0);
        }
        else
            parent.ChangeState( nextState );
    }

    public virtual void Flying(float targetHeight, float lerpDuration = 0.5f, UnitState _next = UnitState.Idle)
    {
        this.targetHeight = targetHeight;
        this.lerpDuration = lerpDuration;
        this.nextState = _next;

        if (targetHeight > 0.0f)
            parent.CharInfo.CurMoveType = MoveType.Air;
    }
}

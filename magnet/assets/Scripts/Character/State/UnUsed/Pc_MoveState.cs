using UnityEngine;
using System.Collections;

public class Pc_MoveState : UnitStateBase
{
    Pc pc;
    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize( _parent );

        if (!(_parent is Pc))
            Debug.LogError( "you must have Pc Component!" );

        pc = _parent as Pc;
    }

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        parent.PlayAnim( eAnimName.Anim_move, true, 0.1f, true );
    }

    public override void OnExit(System.Action callback)
    {
        pc.IsMovingLastLocation = false;

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        if (pc.IsLeader && pc.IsMovingLastLocation)
        {
            // 수동모드 일때도 공격대상을 찾는 작업을 한다.
            base.CachedUpdate();

            if (parent.TargetID != GameDefine.unassignedID)
                return;

            // 수동모드라면 이동지우기
            if (!G_GameInfo.GameInfo.AutoMode)
            {
                parent.ClearPath();
                parent.ChangeState( UnitState.Idle );
                return;
            }
        }

        if (!parent.MoveToPath())
        {
            parent.ChangeState( UnitState.Idle );
        }
    }
}

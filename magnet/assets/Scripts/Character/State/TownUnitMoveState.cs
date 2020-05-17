using UnityEngine;
using System.Collections;
using System;

public class TownUnitMoveState : MoveState {

    //MyTownUnit mtu;
    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize(_parent);
        //base.OnEnter(callback);

        //if (!(_parent is MyTownUnit))
        //    Debug.LogError("you must have MyTownUnit Component!");

        //mtu = _parent as MyTownUnit;
    }

    public override void OnEnter(Action callback)
    {
        base.OnEnter(callback);
        //parent.PlayAnim(eAnimName.Anim_move, true, 0.1f, true);
    }

    public override void OnExit(Action callback)
    {
        //mtu.IsMovingLastLocation = false;
        //parent.MoveNetwork(parent.cachedTransform.position);

        base.OnExit(callback);
    }

    public override void CachedUpdate()
    {
        if (!TownState.TownActive)
            return;

        parent.UnitStop = false;

        //parent.MoveNetwork(parent.cachedTransform.position);

        if (!parent.MoveToPath(moveSpeedRatio))
        {
            if (!parent.StopState)
                parent.ChangeState(UnitState.Idle);

            // 수동모드라면 이동지우기
            if (parent.IsMovingLastLocation)
            {
                parent.ClearPath();
                parent.ChangeState(UnitState.Idle);
                return;
            }
        }

    }
}

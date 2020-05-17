using UnityEngine;
using System.Collections;

public class Pc_IdleState : IdleState
{
    Pc pc;

    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize( _parent );

        if (!(_parent is Pc))
            Debug.LogError( "you must have Pc Component!" );
        pc = _parent as Pc;
    }

    public override void CachedUpdate()
    {
        base.CachedUpdate();

        if (pc.IsLeader && G_GameInfo.GameInfo.AutoMode)
        {
            //< 타겟이 없을때에는 마지막으로 이동한다.
            if(parent.GetTarget() == null)
                pc.GoToLastLocation();
        }
    }
}

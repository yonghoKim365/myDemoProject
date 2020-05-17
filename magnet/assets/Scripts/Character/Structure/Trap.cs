using UnityEngine;
using System.Collections;

public class Trap : Unit
{
    public AnimationClip startAnim;
    public AnimationClip endAnim;

    protected override void Init_SyncData(params object[] args)
    {
        base.Init_SyncData(args);

        switch (UnitType)
        {
            case UnitType.Trap:
                //NpcLowID = (uint)args[3];
                break;
        }
    }

    protected override void Init_Controllers() { }
    protected override void SetupComponents() { }
}

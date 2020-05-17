using UnityEngine;
using System.Collections;

/// <summary>
/// 보스 죽고나서 후처리
/// </summary>
public class Boss_DeadState : DeadState
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );
    }

    protected override void EndDead()
    {
        parent.CharInfo.Hp = 0;

        G_GameInfo.CharacterMgr.RemoveUnit( parent );

        parent.Model.Main.SetActive( false );
    }
}

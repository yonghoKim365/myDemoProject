using UnityEngine;
using System.Collections;

/// <summary>
/// Pc죽고나서 후처리
/// </summary>
public class Pc_DeadState : DeadState
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

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
    }

    protected override void EndDead()
    {
        G_GameInfo.CharacterMgr.RemoveUnit( parent );

        parent.Model.Main.SetActive( false );
    }
}

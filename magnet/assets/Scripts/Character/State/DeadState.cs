using UnityEngine;
using System.Collections;

public class DeadState : UnitStateBase
{
    protected float duration = 0;
    float EndDelay = 9999;
    override public void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        parent.SpawnEffect("Fx_Monstor_Death_01", 2, parent.cachedTransform, null, false, (deadFx) => 
        {
            duration = SelfDespawn.DespawnPartileSystem("Effect", deadFx, 1.5f) * 1.2f;
            
        });
        
        parent.DeleteShadow();
        parent.DeadEffect(1.5f, () => 
        {
            EndDelay = 0.5f;
        });

        EndDelay = float.MaxValue;
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
    }

    protected virtual void EndDead()
    { 
        enabled = false;

        G_GameInfo.CharacterMgr.RemoveUnit( parent );

        if (parent.UnitType != UnitType.Unit)
            Destroy( parent.gameObject );

        parent.Model.Main.SetActive(false);
    }

    public override void CachedUpdate()
    {
        EndDelay -= Time.deltaTime;
        if(EndDelay <= 0)
        {
            EndDelay = 99999;
            EndDead();
        }
    }
}
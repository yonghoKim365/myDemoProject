using UnityEngine;
using System.Collections;

public class WanderState : UnitStateBase
{
    float time = 0f;
    protected float wanderDistance = 3;
    protected float intervalNewDest = 5f;
    Vector3 destination;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        if (time == 0 && G_GameInfo.GameInfo.GameMode != GAME_MODE.NONE)
            NewDestination();
    }

    public override void OnExit(System.Action callback)
    {
        time = 0;

        base.OnExit( callback );        
    }

    public override void CachedUpdate()
    {
        if (G_GameInfo.GameInfo.GameMode == GAME_MODE.NONE)
            return;

        if (parent.CheckAndFlying(UnitState.Wander))
            return;

        //base.CachedUpdate();

        //if (parent.TargetID != GameDefine.unassignedID)
        //{
        //    if (parent.ChangeState( UnitState.Attack ))
        //        return;
        //}

        UpdateWander();
    }

    protected virtual void UpdateWander()
    {
        // 현재 속도의 1/3 속도로 이동하도록 한다.
        if (time > intervalNewDest)
        {
            NewDestination();
        }
        
        // 다 이동했다면, idle모션으로 대기하기
        if (!parent.MoveToPath( .33f ))
            parent.PlayAnim(TownState.TownActive ? eAnimName.Anim_idle : eAnimName.Anim_battle_idle, true, 0.1f );

        time += Time.deltaTime;
    }

    protected virtual void NewDestination()
    {
        time = 0;
        intervalNewDest = Random.Range( 3f, 10f );

        destination = parent.cachedTransform.position + Random.onUnitSphere * wanderDistance * Random.Range( 0.3f, 1f );
        
        if (parent.CalculatePath( destination ))
        { 
            Debug.DrawLine( parent.cachedTransform.position, destination, Color.red, intervalNewDest );
            // TODO : 걷기애니메이션 생기면 그때 넣기
            parent.PlayAnim( eAnimName.Anim_move, true, 0.1f );
        }
    }
}

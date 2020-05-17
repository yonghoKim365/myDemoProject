using UnityEngine;
using System.Collections;

public class IdleState : UnitStateBase
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

	    TownState _townstate = SceneManager.instance.CurrentStateBase() as TownState;

        if( _townstate == null )
        {
            //마을아니다
            parent.SetObstacleAvoidance(true);
            
            //명장 예외처리 
            //if (parent.CharInfo.GetBuffValue(BuffType.ANGLEDEFUP) > 0f)
            if(parent.BuffCtlr != null && (parent.BuffCtlr.GetTypeBuffCheck(BuffType.ANGLEDEFUP) || parent.BuffCtlr.GetTypeBuffCheck(BuffType.CHARGEATTACK) ))
            {
                parent.PlayAnim(eAnimName.Anim_Extra, true, 0.1f, true);
            }
            else
            {
                parent.PlayAnim(eAnimName.Anim_battle_idle, true, 0.08f, true);
            }
        }
        else
        {
            //마을이다
            if( parent is InteractionNPC)
            {
                if (Random.Range(0, 100) > 50)
                    parent.PlayAnim(eAnimName.Anim_idle, true, 0.1f);
                else
                    parent.PlayAnim(eAnimName.Anim_intro, true, 0.1f);
            }
            else
            {
                parent.PlayAnim(eAnimName.Anim_idle, true, 0.08f, true);
            }
        }        
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
    }

    public override void CachedUpdate()
    {
        if (parent.IsStun)
        {
            parent.ChangeState( UnitState.Stun );
            return;
        }

        if (parent.CheckAndFlying())
            return;

        //if(G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
        //{
        //    if( parent.UnitType == UnitType.Npc && 0 < (G_GameInfo.GameInfo as TutorialGameInfo).DummyUnitNum )
        //    {
        //        return;
        //    }
        //}

        base.CachedUpdate();

        if(SceneManager.instance.IsRTNetwork)
        {
            //< 헬퍼인데, 타겟이 없다면..
            if(G_GameInfo.PlayerController.Leader.TeamID == parent.TeamID)
            {
                if (parent.IsHelper && parent.TargetID == GameDefine.unassignedID)
                {
                    //< 대상 플레이어와의 거리를 체크해준다
                    if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.cachedTransform.position) > 5)
                    {
                        //parent.CalculatePath(parent.HelperParnetUnit.transform.position - (parent.HelperParnetUnit.transform.forward * 1));
                        parent.ChangeState(UnitState.Move);
                        parent.MovePosition(parent.HelperParnetUnit.transform.position);
                    }
                }
            }            
        }
        else
        {
            //< 헬퍼인데, 타겟이 없다면..
            if (parent.IsHelper && parent.TargetID == GameDefine.unassignedID)
            {
                //< 대상 플레이어와의 거리를 체크해준다
                if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.cachedTransform.position) > 5)
                {
                    //parent.CalculatePath(parent.HelperParnetUnit.transform.position - (parent.HelperParnetUnit.transform.forward * 1));
                    parent.ChangeState(UnitState.Move);
                    parent.MovePosition(parent.HelperParnetUnit.transform.position);
                }
            }
        }

        //실시간 네트워크 모드에서 타겟이 없다면.. 항상  마지막 위치로 이동하려고 한다. 
        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        //if (SceneManager.instance.IsRTNetwork)
        //{
            //if (parent.UnitType == UnitType.Unit)
            {
                parent.UnitStop = true;
            }
        //}//if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
    }
}

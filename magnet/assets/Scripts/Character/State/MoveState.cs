using UnityEngine;
using System.Collections;

public class MoveState : UnitStateBase
{
    public float moveSpeedRatio = 1f;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        TownState _townstate = SceneManager.instance.CurrentStateBase() as TownState;

        if (_townstate == null)
        {
            parent.SetObstacleAvoidance(false);
        }
        if(parent.CharInfo.GetBuffValue(BuffType.ANGLEDEFUP) > 0f)
        {
            moveSpeedRatio = 0.6f;
            parent.PlayAnim(eAnimName.Anim_walk, true, 0.1f, true);
        }
        else
        {
            moveSpeedRatio = 1f;
            parent.PlayAnim(eAnimName.Anim_move, true, 0.1f, true);
        }
    }

    public override void OnExit(System.Action callback)
    {
        parent.IsMovingLastLocation = false;
        parent.ClearPath();
        //moveSpeedRatio = 1f;
        //parent.MoveNetwork(parent.cachedTransform.position);

        base.OnExit( callback );
    }

    public override void CachedUpdate()
    {
        //일단 임시... 피곤하다
        if(G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        {
            parent.UnitStop = false;

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

            //parent.MoveNetwork(parent.cachedTransform.position);
        }
        else
        {
            parent.UnitStop = false;
            if (!parent.MoveToPath(moveSpeedRatio))
            {
                /*
                if (parent.IsHelper && parent.HelperParnetUnit != null)
                {
                    if (parent.IsPartner && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID)
                    {
                        float dist = Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.transform.position);
                        if (dist > 3)
                        {
                            moveSpeedRatio = 1 * (dist / 3);
                            parent.CalculatePath(parent.HelperParnetUnit.transform.position - (parent.HelperParnetUnit.transform.forward * 1));
                        }
                        else
                            parent.ChangeState(UnitState.Idle);
                    }
                }
                else*/
                {
                    //< 조이스틱으로 움직이고 있다면 패스
                    if (!parent.StopState)
                        parent.ChangeState(UnitState.Idle);
                }
            }

            //< 헬퍼인데, 길이 막혔거나해서 타겟과 멀어졌을시 순간이동 시켜줌
            if (parent.IsHelper && parent.HelperParnetUnit != null)
            {
                //if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.transform.position) > 10)
                //{
                //    parent.transform.position = parent.HelperParnetUnit.transform.position - parent.HelperParnetUnit.transform.forward * 0.2f;
                //    parent.transform.rotation = parent.HelperParnetUnit.transform.rotation;
                //    parent.ChangeState(UnitState.Idle);
                //    return;
                //}
                //else
                //{
                //    //< 혹여나 한곳에서 0.5초이상 머물러있다면 그것도 이동시켜줌
                //    if (Vector3.Distance(NowPos,parent.transform.position) < 0.1f)
                //    {
                //        if ((Time.time - MoveTime) > 0.5f)
                //        {
                //            parent.transform.position = parent.HelperParnetUnit.transform.position - parent.HelperParnetUnit.transform.forward * 0.2f;
                //            parent.transform.rotation = parent.HelperParnetUnit.transform.rotation;
                //            parent.ChangeState(UnitState.Idle);
                //        }
                //    }
                //    else
                //    {
                //        NowPos = parent.transform.position;
                //        MoveTime = Time.time;
                //    }
                //}
            }

            if (parent.IsLeader)
            {
                // 수동모드라면 이동지우기
                if (parent.IsMovingLastLocation && !G_GameInfo.GameInfo.AutoMode)
                {
                    parent.ClearPath();
                    parent.ChangeState(UnitState.Idle);
                    return;
                }


                //< 수동이 아니라면 이동하면서 타겟을 계속 검사함
                if (!parent.StopState)
                    base.CachedUpdate();
            }
            else
            {
                //네트워크가 아니라면
                if (G_GameInfo.GameMode != GAME_MODE.FREEFIGHT)
                {
                    Unit Target = parent.GetTarget();

                    if (Target == null)
                    {
                        if (parent.IsHelper)
                        {
                            if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.cachedTransform.position) > 5)
                            {
                                parent.CalculatePath(parent.HelperParnetUnit.transform.position - (parent.HelperParnetUnit.transform.forward * 1));
                                parent.ChangeState(UnitState.Move);
                                parent.MovePosition(parent.HelperParnetUnit.transform.position);
                            }
                            else
                            {
                                parent.ChangeState(UnitState.Idle);
                            }
                        }
                        else
                        {
                            parent.ChangeState(UnitState.Idle);
                        }
                    }
                }
                else
                {
                    //네트워크 이고 내 파트너 일경우
                    if (parent.IsPartner && parent.TeamID == G_GameInfo.PlayerController.Leader.TeamID)
                    {
                        Unit Target = parent.GetTarget();

                        if (Target == null)
                        {
                            if (parent.IsHelper)
                            {
                                if (Vector3.Distance(parent.HelperParnetUnit.transform.position, parent.cachedTransform.position) > 5)
                                {
                                    parent.CalculatePath(parent.HelperParnetUnit.transform.position - (parent.HelperParnetUnit.transform.forward * 1));
                                    parent.ChangeState(UnitState.Move);
                                    parent.MovePosition(parent.HelperParnetUnit.transform.position);
                                }
                                else
                                {
                                    parent.ChangeState(UnitState.Idle);
                                }
                            }
                            else
                            {
                                parent.ChangeState(UnitState.Idle);
                            }


                        }
                    }
                }

                base.CachedUpdate();

            }
        }
    }
}

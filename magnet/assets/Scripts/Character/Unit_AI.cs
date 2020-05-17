using UnityEngine;
using System.Collections;

public class Unit_AI : MonoBehaviour {

    bool goNear = false;
    Unit target = null;
    bool invalidTarget = true;
    Unit parent = null;

    // Use this for initialization
    void Start () {
        parent = GetComponent<Unit>();
	}

    // Update is called once per frame
    void Update()
    {
        if(!TownState.TownActive)
        {
            if (parent.UnitType == UnitType.Unit && !parent.IsPartner)
            {
                if (!(parent.CurrentState == UnitState.Idle || parent.CurrentState == UnitState.Move || parent.CurrentState == UnitState.Wander || parent.CurrentState == UnitState.ManualAttack))
                {
                    return;
                }

                if (parent.CurrentState == UnitState.ManualAttack && !parent.SkillBlend)
                {
                    return;
                }
            }
            else
            {
                if (!(parent.CurrentState == UnitState.Idle || parent.CurrentState == UnitState.Move || parent.CurrentState == UnitState.Wander))
                    return;
            }
            //if (!(parent.CurrentState == UnitState.Idle || parent.CurrentState == UnitState.Move || parent.CurrentState == UnitState.Wander))
            //    return;

            bool bActive = false;
            if (G_GameInfo.GameMode == GAME_MODE.SINGLE || G_GameInfo.GameMode == GAME_MODE.TOWER || G_GameInfo.GameMode == GAME_MODE.FREEFIGHT || G_GameInfo.GameMode == GAME_MODE.RAID
                || G_GameInfo.GameMode == GAME_MODE.SPECIAL_EXP || G_GameInfo.GameMode == GAME_MODE.SPECIAL_GOLD || G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
            {
                if ((parent.TeamID == 1 && G_GameInfo.GameInfo.EnemyAutoMode) || (parent.TeamID == 0 && G_GameInfo.GameInfo.AutoMode) || (parent.TeamID == 0 && parent.IsPartner))
                {
                    if( G_GameInfo.GameMode == GAME_MODE.FREEFIGHT )
                    {
                        if(NetData.instance._userInfo._charUUID == parent.m_rUUID)
                        {
                            bActive = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        bActive = true;
                    }
                }
            }
            else if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT) //난투장일경우
            {
                if (parent.TeamID == 0 && G_GameInfo.GameInfo.AutoMode && parent.IsLeader)
                {
                    bActive = true;
                }
            }
            else if (G_GameInfo.GameMode == GAME_MODE.ARENA) //아레나일경우
            {
                if ((G_GameInfo.PlayerController.Leader.TeamID == parent.TeamID && parent.IsPartner) || //내팀애들이고 파트너일경우 - 자동
                    ((G_GameInfo.PlayerController.Leader.TeamID == parent.TeamID) && G_GameInfo.GameInfo.AutoMode) && !parent.IsPartner)//내팀이고 나고 자동일경우 - 자동
                {
                    bActive = true;
                }

                if(G_GameInfo.PlayerController.Leader.TeamID != parent.TeamID)
                {
                    //다른팀이면 무조건 자동
                    bActive = true;
                }
            }
            //else if (G_GameInfo.GameMode == GAME_MODE.TUTORIAL) //튜토리얼
            //{
            //    if ((parent.TeamID == 1 && G_GameInfo.GameInfo.EnemyAutoMode) || //적 유닛
            //        (G_GameInfo.PlayerController.Leader.TeamID == parent.TeamID && G_GameInfo.GameInfo.AutoMode))//플레이어
            //    {
            //        bActive = true;
            //    }

            //}


            if (!bActive)
                return;

            CheckTarget();

            // 타겟이 없으면 Idle or PathMove
            if (invalidTarget || target == null || target.CharInfo.IsDead)
            {
                //난투장에서는 타겟이 없으면대기 다른데는 끝지점으로 이동
                if (!SceneManager.instance.IsRTNetwork)
                {
                    if (parent.m_rUUID == NetData.instance._userInfo._charUUID)
                    {
                        if(G_GameInfo.GameMode == GAME_MODE.SINGLE)
                        {
                            GameObject nextWall = (G_GameInfo.GameInfo as SingleGameInfo).spawnCtlr.NextWall;

                            if (nextWall == null )
                            {
                                Vector3 lastPos = G_GameInfo.GameInfo.CurNavMeshGroup().end.position;
                                Vector3 offset = parent.cachedTransform.position - lastPos;
                                if (offset.sqrMagnitude < 4f)
                                    return;

                                Vector3 dest;
                                if(CalcPosition(G_GameInfo.GameInfo.CurNavMeshGroup().end.position, out dest))
                                {
                                    bool success = parent.MovePosition(dest, 1f);
                                }
                                //parent.MovePosition(G_GameInfo.GameInfo.CurNavMeshGroup().end.position, 1f, true);
                            }
                            else
                            {
                                Vector3 dest;
                                if(CalcPosition(nextWall.transform.position, out dest))
                                {
                                    bool success = parent.MovePosition(dest, 1f);
                                }
                                //parent.MovePosition(nextWall.transform.position, 1f, true);
                            }
                            return;
                        }
                        else
                        {
                            Vector3 lastPos = G_GameInfo.GameInfo.CurNavMeshGroup().end.position;
                            Vector3 offset = parent.cachedTransform.position - lastPos;
                            if (offset.sqrMagnitude < 4f)
                                return;

                            Vector3 dest;
                            if(CalcPosition(G_GameInfo.GameInfo.CurNavMeshGroup().end.position, out dest))
                            {
                                bool success = parent.MovePosition(dest, 1f);
                            }
                            //parent.MovePosition(G_GameInfo.GameInfo.CurNavMeshGroup().end.position, 1f, true);
                        }                        
                    }
                }

                return;
            }

            //대상과의 거리 체크
            Vector3 targetDist = target.cachedTransform.position - parent.cachedTransform.position;
            float atkRange = parent.CharInfo.AtkRange;

            if(parent.UnitType == UnitType.Unit && !parent.IsPartner )
            {
                if( parent.CurCombo >= 1 )
                {
                    atkRange = atkRange + 2f;
                }
            }

            // 공격 사거리내에 들어왔다면,
            bool canAttack = MathHelper.IsInRange(targetDist, atkRange, parent.Radius, target.Radius);

            if (canAttack)
            {
                if (parent.inputCtlr != null)
                    parent.inputCtlr.RotationFromInterJoyStick = Quaternion.identity;

                if (parent.UnitType == UnitType.Boss && (parent as Npc).BossPatten != null && (parent as Npc).BossPatten.PattenUpdate())// G_GameInfo.GameMode == GAME_MODE.RAID &&
                {
                    return;
                }
                else
                //if (parent.CurCombo == 0)
                {
                    if (parent.skill_AI != null && parent.skill_AI.SkillUpdate())
                        return;
                }

                //대상보고 해야되는데...일단 임시로 여기서 대상보기
                parent.LookAt(target.cachedTransform.position);
                parent.ManualAttack();
                return;
            }
            else
            {
                goNear = true;
                //parent.PlayAnim(eAnimName.Anim_move, true, 0.1f);
                //parent.UnitStop = false;
            }

            if (goNear)
            {
                // 대상과 붙을 수 있는 최소한 까지 이동하기.
                goNear = !MathHelper.IsInRange(targetDist, atkRange, parent.Radius, target.Radius);

                Vector3 dest;
                if(CalcPosition(target.cachedTransform.position, out dest))
                {
                    bool success = parent.MovePosition(dest, 1f);
                }
            }
        }
        else
        {
            //일단 내위치를 찾는다

        }        
    }

    public bool CalcPosition(Vector3 dest, out Vector3 result)
    {
        bool success;
        success = CalculatePath(dest, false);

        if(success)
        {
            if (movePath.corners.Length > 1)
            {
                Vector3 targetPos = movePath.corners[movePathIndex];
                Vector3 curPos = parent.cachedTransform.position;

                if ((dest - curPos).sqrMagnitude < 1f)
                {
                    result = Vector3.zero;
                    return false;
                }

                targetPos.y = curPos.y = 0; // x, z값은 0이고, y만 값만 존재할때 이동문제 발생 (높낮이 맵에 의한)

                Vector3 offset = targetPos - curPos;
                float movespeed = parent.CharInfo.MoveSpeed;// 8.5f;// CharInfo != null ? CharInfo.MoveSpeed : 8.5f;
                                                            //Debug.LogWarning("2JW : " + Owner + " : " + movespeed + " : " + CharInfo.MoveSpeed);
                //float speed = movespeed * speedRatio;

                // 기본으로 이동해야할 힘 = 방향 * 속도
                Vector3 velocity = offset.normalized;

                result = parent.cachedTransform.position + velocity;

                return true;
            }
            else
            {
                //이동불가
            }
        }

        result = Vector3.zero;

        return false;
    }

    protected NavMeshPath movePath = new NavMeshPath();
    protected int movePathIndex = 0;

    public void ClearPath()
    {
        movePath.ClearCorners();
        movePathIndex = 1;
    }

    public bool CalculatePath(Vector3 TargetPos, bool end = false)
    {
        if (!parent.gameObject.activeSelf || !parent.UsableNavAgent || float.IsNaN(TargetPos.x) || movePath == null)
            return false;

        ClearPath();

        // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 NavMesh가능 위치를 찾아준다.
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(TargetPos, out navHit, Vector3.Distance(TargetPos, parent.transform.position), 9))
        {
            // 9 == Terrain
            TargetPos = navHit.position;
        }

        bool Find = parent.navAgent.CalculatePath(TargetPos, movePath);

        // 시작점과 끝점은 계산에서 제외시킴
        for (int i = 1; i < movePath.corners.Length - 1; i++)
        {
            // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
            if (NavMesh.FindClosestEdge(movePath.corners[i], out navHit, 1))
            {
                if ((navHit.position - movePath.corners[i]).sqrMagnitude < 1f)
                {
                    movePath.corners[i] = movePath.corners[i] + navHit.normal * 1f;
                }
            }
        }

        // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
        if (movePath.corners.Length >= 2)
        {
            // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
            for (int i = 1; i < movePath.corners.Length - 1; ++i)
            {
                if ((movePath.corners[i] - movePath.corners[i + 1]).sqrMagnitude < 2f)
                    movePath.corners[i + 1] = movePath.corners[i];
            }
        }

        return Find;
    }

    float NewCheckTargetDelay = 0.2f;
    void CheckTarget()
    {
        //< 일정시간마다 제일 가까운 타겟으로 다시 검사한다.(PC일경우에만 검사한다. 몬스터는 PC만 타겟이기때문)
        if (parent.UnitType == UnitType.Unit)
        {
            NewCheckTargetDelay -= Time.deltaTime;
            if (NewCheckTargetDelay <= 0)
            {
                NewCheckTargetDelay = 0.2f;

                //< 일점사 대상이 없을경우에만 새로운 대상을 검색한다.
                if (parent.ForcedAttackTarget == null)
                {
                    float radius = 0f;

                    if(SceneManager.instance.IsRTNetwork)
                    {
                        if(FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                        {
                            radius = 1000f;
                        }
                        else
                        {
                            radius = parent.CharInfo.AtkRecognition;
                        }
                    }
                    else
                    {
                        radius = parent.CharInfo.AtkRecognition;
                    }

                    Unit newCheckTarget = G_GameInfo.CharacterMgr.FindTargetWithAggro(parent, radius, parent, true);
                    if (null != newCheckTarget)
                        parent.TargetID = newCheckTarget.GetInstanceID();
                }
            }
        }

        Unit newTarget = null;
        invalidTarget = G_GameInfo.CharacterMgr.InvalidTarget(parent.TargetID, ref newTarget);
        if (!invalidTarget && target != newTarget)
            target = newTarget;

        // 공격 못하는 타겟인지 체크
        bool cantAtkType = false;
        if (target != null)
            cantAtkType = target.CurrentState == UnitState.Dead || target.CurrentState == UnitState.Dying || target.CharInfo.Hp <= 0;// || (target.CurrentState == UnitState.Event && RaidBossAIBase.NotTargeting);

        if (cantAtkType || invalidTarget)
        {
            parent.SetTarget(GameDefine.unassignedID);
            target = null;
        }
    }
}

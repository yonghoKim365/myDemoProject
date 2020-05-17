using UnityEngine;
using System.Collections;

public class MoveToSkillState : UnitStateBase
{
    Skill curSkill;
    Unit target = null;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        curSkill = parent.SkillCtlr.SkillList[parent.MoveToSkillSlot];
        FirstRush = true;
        if (!GoTarget())
            parent.ChangeState(UnitState.Idle);
    }

    public bool UseSkillReady = false;
    public float RushDistance;
    bool FirstRush = true;
    public override void CachedUpdate()
    {
        if (null == target || null == curSkill)
        {
            parent.ChangeState( UnitState.Idle );
            return;
        }

        parent.UnitStop = false;

        if (UseSkillReady)
        {
            parent.UseSkillSlot = (uint)parent.MoveToSkillSlot;
            parent.UseSkillIdx = (uint)parent.SkillCtlr.GetSkillIndex(parent.MoveToSkillSlot);
            parent.ChangeState(UnitState.Skill);
            UseSkillReady = false;
        }
        else
        {
            // 스킬사거리 안에 들어오는지
            if (MathHelper.IsInRange(target.transform.position - parent.transform.position, curSkill.range, parent.Radius, target.Radius))
            {
                parent.UseSkillSlot = (uint)parent.MoveToSkillSlot;
                parent.UseSkillIdx = (uint)parent.SkillCtlr.GetSkillIndex(parent.MoveToSkillSlot);
                parent.ChangeState(UnitState.Skill);
            }
            else
            {
                RushDistance = Vector3.Distance(parent.transform.position, target.transform.position);
                if (RushDistance < parent.CharInfo.RushAtkRange)
                {
                    parent.PlayAnim(eAnimName.Anim_dash, true, 0.08f, true);
                    if (!parent.MoveToPath(3))
                        GoTarget();

                    //< 첫 러쉬 들어갈때 패스를 갱신
                    if (FirstRush)
                    {
                        GoTarget();
                        FirstRush = false;
                    }
                }
                else
                {
                    // 이동을 끝냈다면, 한번더 체크
                    if (!parent.MoveToPath())
                        GoTarget();
                }
            }
        }

        //< 대상을 향해 가는 도중에 더 가까운 대상이 있는지 검사
        if (G_GameInfo._GameInfo.AutoMode)
            SearchTarget();
    }

    float SearchTime = 0;
    void SearchTarget()
    {
        if((Time.time - SearchTime) > 0.5f)
        {
            SearchTime = Time.time;

            Unit newCheckTarget = G_GameInfo.CharacterMgr.FindTargetWithAggro(parent, parent.CharInfo.AtkRecognition, parent);
            if (newCheckTarget != null)
                target = newCheckTarget;
        }
    }

    public override void OnExit(System.Action callback)
    {
        UseSkillReady = false;
        parent.MoveToSkillSlot = 0;
        base.OnExit( callback );
    }

    bool GoTarget()
    {
        if (G_GameInfo.CharacterMgr.InvalidTarget( parent.TargetID, ref target ))
            return false;

        parent.CalculatePath( target.transform.position );
        parent.PlayAnim( eAnimName.Anim_move, true, 0.08f, true );
        
        return true;
    }
}

using UnityEngine;

/// <summary>
/// 상태 머신 설정용 클래스
/// </summary>
public static class GK_FSMFactory
{
    /// <summary>
    /// 원하는 타입에 맞게 FSM을 생성 및 설정해준다.
    /// </summary>
    /// <param name="target">설정하고자 하는 대상</param>
    /// <param name="wantUnitType">유닛타입</param>
    public static void SetupFSM(Unit parent, UnitType wantUnitType, out FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm = new FSM.FSM<UnitEvent, UnitState, Unit>( parent );

        switch (wantUnitType)
        {
            case UnitType.TownUnit:
                {
                    SetupStateForTownUnit(parent, ref fsm);
                }
                break;
            case UnitType.TownNpc:
                {
                    SetupStateForTownNPC(parent, ref fsm);
                }
                break;

            case UnitType.TownNINPC:
                {
                    SetupStateForTownNINPC(parent, ref fsm);
                }
                break;

            case UnitType.Unit:
                {
                    // Unit전용 상태머신 셋팅해야됨.
                    SetupStateForPc( parent, ref fsm );
                }
                break;

            case UnitType.Npc:
                {
                    SetupStateForNpc(parent, ref fsm);
                }
                break;

            case UnitType.Boss:
                {
                    SetupStateForBoss( parent, ref fsm );
                }
                break;

            case UnitType.Prop:
                { 
                    SetupStateForProp( parent, ref fsm );
                }
                break;

            case UnitType.Trap:
                { 
                    SetupStateForTrap( parent, ref fsm );
                }
                break;
        }

    }

    /// <summary>
    /// Pc를 위한 기본 스테이트 생성 및 전이 설정
    /// </summary>
    static void SetupStateForPc(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        //fsm.AddState(UnitState.Wander, parent.gameObject.AddComponent<WanderState>());
        fsm.AddState(UnitState.Idle, parent.gameObject.AddComponent<IdleState>());
        fsm.AddState(UnitState.Move, parent.gameObject.AddComponent<MoveState>());
        fsm.AddState(UnitState.Attack, parent.gameObject.AddComponent<AttackState>());
        //일단 당분간 보류
        fsm.AddState(UnitState.ManualAttack, parent.gameObject.AddComponent<ManualAttackState>());
        //fsm.AddState(UnitState.Evasion, parent.gameObject.AddComponent<EvasionState>());
        fsm.AddState(UnitState.Skill, parent.gameObject.AddComponent<SkillState>());
        //fsm.AddState(UnitState.MoveToSkill, parent.gameObject.AddComponent<MoveToSkillState>());
        fsm.AddState(UnitState.Push, parent.gameObject.AddComponent<PushState>());
        fsm.AddState(UnitState.Dying, parent.gameObject.AddComponent<DyingState>());
        fsm.AddState(UnitState.Dead, parent.gameObject.AddComponent<DeadState>());
        fsm.AddState(UnitState.Stun, parent.gameObject.AddComponent<StunState>());
        fsm.AddState(UnitState.Floating, parent.gameObject.AddComponent<FloatingState>());
        fsm.AddState(UnitState.StandUp, parent.gameObject.AddComponent<StandUpState>());
        //fsm.AddState( UnitState.Flying, parent.gameObject.AddComponent<FlyingState>() );
        fsm.AddState(UnitState.Event, parent.gameObject.AddComponent<EventState>());

        // for IDLE
        fsm.RegistEvent(UnitState.Idle, UnitEvent.Move, UnitState.Move);

        // for MOVE
        fsm.RegistEvent(UnitState.Move, UnitEvent.Idle, UnitState.Idle);
    }

    static void SetupStateForTownUnit(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState(UnitState.Idle, parent.gameObject.AddComponent<TownUnitIdleState>());
        fsm.AddState(UnitState.Move, parent.gameObject.AddComponent<TownUnitMoveState>());

        // for IDLE
        fsm.RegistEvent(UnitState.Idle, UnitEvent.Move, UnitState.Move);

        // for MOVE
        fsm.RegistEvent(UnitState.Move, UnitEvent.Idle, UnitState.Idle);
    }

    static void SetupStateForTownNPC(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState(UnitState.Idle, parent.gameObject.AddComponent<TownUnitIdleState>());
        fsm.AddState(UnitState.Move, parent.gameObject.AddComponent<TownUnitMoveState>());

        // for IDLE
        fsm.RegistEvent(UnitState.Idle, UnitEvent.Move, UnitState.Move);

        // for MOVE
        fsm.RegistEvent(UnitState.Move, UnitEvent.Idle, UnitState.Idle);
    }

    static void SetupStateForTownNINPC(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState(UnitState.Idle, parent.gameObject.AddComponent<TownUnitIdleState>());
        fsm.AddState(UnitState.Wander, parent.gameObject.AddComponent<TownUnitWanderState>());

        // for IDLE
        fsm.RegistEvent(UnitState.Idle, UnitEvent.Wander, UnitState.Wander);

        // for MOVE
        fsm.RegistEvent(UnitState.Wander, UnitEvent.Idle, UnitState.Idle);
    }    

    static void SetupStateForNpc(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState( UnitState.Wander, parent.gameObject.AddComponent<WanderState>() );
        fsm.AddState( UnitState.Idle, parent.gameObject.AddComponent<IdleState>() );
        fsm.AddState( UnitState.Move, parent.gameObject.AddComponent<MoveState>() );
        //fsm.AddState( UnitState.Attack, parent.gameObject.AddComponent<AttackState>() );
        fsm.AddState(UnitState.ManualAttack, parent.gameObject.AddComponent<ManualAttackState>());
        //fsm.AddState(UnitState.Evasion, parent.gameObject.AddComponent<EvasionState>());
        fsm.AddState( UnitState.Skill, parent.gameObject.AddComponent<SkillState>() );
        //fsm.AddState( UnitState.MoveToSkill, parent.gameObject.AddComponent<MoveToSkillState>() );
        fsm.AddState( UnitState.Push, parent.gameObject.AddComponent<PushState>() );
        fsm.AddState( UnitState.Dying, parent.gameObject.AddComponent<DyingState>() );
        fsm.AddState( UnitState.Dead, parent.gameObject.AddComponent<DeadState>() );
        fsm.AddState( UnitState.Stun, parent.gameObject.AddComponent<StunState>() );
        fsm.AddState( UnitState.Floating, parent.gameObject.AddComponent<FloatingState>() );
        fsm.AddState( UnitState.StandUp, parent.gameObject.AddComponent<StandUpState>() );
        //fsm.AddState( UnitState.Flying, parent.gameObject.AddComponent<FlyingState>() );
        fsm.AddState( UnitState.Event, parent.gameObject.AddComponent<EventState>());
        
        // for IDLE
        fsm.RegistEvent( UnitState.Idle, UnitEvent.Move, UnitState.Move );

        // for MOVE
        fsm.RegistEvent( UnitState.Move, UnitEvent.Idle, UnitState.Idle );
    }

    static void SetupStateForBoss(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState( UnitState.Idle, parent.gameObject.AddComponent<IdleState>() );
        fsm.AddState( UnitState.Move, parent.gameObject.AddComponent<MoveState>() );
        //fsm.AddState( UnitState.Attack, parent.gameObject.AddComponent<AttackState>() );
        fsm.AddState(UnitState.ManualAttack, parent.gameObject.AddComponent<ManualAttackState>());
        fsm.AddState( UnitState.Skill, parent.gameObject.AddComponent<SkillState>() );
        //fsm.AddState( UnitState.MoveToSkill, parent.gameObject.AddComponent<MoveToSkillState>() );
        fsm.AddState( UnitState.Push, parent.gameObject.AddComponent<PushState>() );
        fsm.AddState( UnitState.Dying, parent.gameObject.AddComponent<DyingState>() );
        fsm.AddState( UnitState.Dead, parent.gameObject.AddComponent<Boss_DeadState>() );
        fsm.AddState( UnitState.Stun, parent.gameObject.AddComponent<StunState>() );
        fsm.AddState( UnitState.Floating, parent.gameObject.AddComponent<FloatingState>() );
        fsm.AddState( UnitState.StandUp, parent.gameObject.AddComponent<StandUpState>() );
        //fsm.AddState( UnitState.Flying, parent.gameObject.AddComponent<FlyingState>() );
        fsm.AddState( UnitState.Event, parent.gameObject.AddComponent<EventState>());
        
        // for IDLE
        fsm.RegistEvent( UnitState.Idle, UnitEvent.Move, UnitState.Move );

        // for MOVE
        fsm.RegistEvent( UnitState.Move, UnitEvent.Idle, UnitState.Idle );
    }

    static void SetupStateForProp(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState( UnitState.Idle, parent.gameObject.AddComponent<Prop_IdleState>() );
        fsm.AddState( UnitState.Dying, parent.gameObject.AddComponent<Prop_DyingState>() );
        fsm.AddState( UnitState.Dead, parent.gameObject.AddComponent<DeadState>() );
    }

    static void SetupStateForTrap(Unit parent, ref FSM.FSM<UnitEvent, UnitState, Unit> fsm)
    {
        fsm.AddState( UnitState.Idle, parent.gameObject.AddComponent<Trap_IdleState>() );
    }
}
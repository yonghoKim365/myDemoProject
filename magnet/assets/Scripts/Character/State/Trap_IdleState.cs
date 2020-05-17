using UnityEngine;
using System.Collections;

public class Trap_IdleState : UnitStateBase
{
    new Trap parent;
    string trapAttackName = "FireTrap";

    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize( _parent );

        parent = _parent as Trap;
    }

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter( callback );

        if (null != parent.collider)
            parent.collider.enabled = false;

        InvokeRepeating( trapAttackName, 1f, parent.CharInfo.AtkDelay );
    }

    public override void OnExit(System.Action callback)
    {
        if (IsInvoking( trapAttackName ))
            CancelInvoke( trapAttackName );

        base.OnExit( callback );
    }

    void FireTrap()
    {
        if (null != parent.startAnim)
        { 
            parent.Animator.PlayAnim( parent.startAnim.name );

            // 다음 공격전에는 EndAnim이 작동되도록 하기.
            if (null != parent.endAnim)
            { 
                float endAnimStart = Mathf.Clamp( parent.startAnim.length, 0, parent.CharInfo.AtkDelay - 0.1f );
                StartCoroutine( EndAttackFunc( endAnimStart ) );
            }
        }

        // 충돌체를 가지고 있다면, 충돌체 기반으로 공격이 적용되도록 한다.
        if (null != parent.collider)
            parent.collider.enabled = true;
        else
            parent.AttackEvent( 1f );
    }

    IEnumerator EndAttackFunc(float delay)
    {
        if (null == parent.endAnim)
            yield break;

        yield return new WaitForSeconds( delay );

        if (null != parent.collider)
            parent.collider.enabled = false;

        parent.Animator.PlayAnim( parent.endAnim.name );
    }

    void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.transform.GetComponent<Unit>();
        if (null == otherUnit)
            return;

        bool canAttack = G_GameInfo.CharacterMgr.CanTarget(parent.CharInfo.AttackType, otherUnit ) && parent.TeamID != otherUnit.TeamID;
        if (canAttack)
            otherUnit.TakeDamage( parent, 1f, parent.CharInfo.Atk, 0, parent.CharInfo.AttackType, false, null );
    }

    public override void CachedUpdate()
    {
    }
}

using UnityEngine;
using System.Collections;

public class TownUnitIdleState : IdleState {
    float _time = 0f;

    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize(_parent);

        //if (_parent is TownUnit)
        //    return;

        //if (!(_parent is MyTownUnit))
        //    Debug.LogError("you must have MyTownUnit Component!");
        //mtu = _parent as MyTownUnit;

    }

    public override void CachedUpdate()
    {
        //base.CachedUpdate();
        if (parent is InteractionNPC)
        {
            _time = _time - Time.deltaTime;

            if(_time < 0f)
            {
                (parent as InteractionNPC).NewDestination();
            }
        }
    }

    public override void OnExit(System.Action callback)
    {
        //mtu.IsMovingLastLocation = false;
        base.OnExit(callback);
    }

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        //if (time == 0)
        //parent.NewDestination();

        if (parent is InteractionNPC)
        {
            if (parent.Animator.CurrentAnim == eAnimName.Anim_intro)
            {
                _time = parent.Animator.GetAnimLength(eAnimName.Anim_intro);
            }
            else
            {
                _time = Random.Range(1f, 3f);
            }
        }

        //bool playani = parent.PlayAnim(eAnimName.Anim_idle, true, 0.08f, true);
        //Debug.LogWarning("2JW : In TownUnitIdleState PlayAnim = " + playani);
    }
}

using UnityEngine;
using System.Collections;

public class TownUnitWanderState : UnitStateBase
{
    TownState _townState = null;
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        _townState = SceneManager.instance.CurrentStateBase() as TownState;
        //parent.PlayAnim(_townState != null ? eAnimName.Anim_idle : eAnimName.Anim_battle_idle, true, 0.1f );

		prevPos = parent.cachedTransform.position;
		samePosCnt = 0;
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
    }

    public override void CachedUpdate()
    {
        UpdateWander();
    }

	Vector3 prevPos;
	int samePosCnt;
    protected virtual void UpdateWander()
    {
        // 현재 속도의 1/3 속도로 이동하도록 한다.
        //if (time > intervalNewDest)
        //{
        //    NewDestination();
        //}

        // 다 이동했다면, idle모션으로 대기하기
        if (!parent.MoveToPath ((parent as InteractionNPC).npcData.MoveSpeed)) {
			//if(Random.Range(0,100) > 50)
			//    parent.PlayAnim(eAnimName.Anim_idle, true, 0.1f);
			//else
			//    parent.PlayAnim(eAnimName.Anim_intro, true, 0.1f);
			parent.ChangeState (UnitState.Idle);
		} else {
			// npc가 구석에 박혀 움직이지 못할때가 있어 추가.
			// 제자리에 50카운트 이상 있으면 상태 바꿔준다. 
			if (prevPos == parent.cachedTransform.position){
				samePosCnt++;
				if (samePosCnt > 50){
					parent.ChangeState (UnitState.Idle);
				}
			}
			prevPos = parent.cachedTransform.position;
		}

    }
}

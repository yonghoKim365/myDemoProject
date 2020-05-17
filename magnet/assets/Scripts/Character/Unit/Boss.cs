using UnityEngine;

public class Boss : Npc
{
    protected override void Init_Controllers()
    {
        base.Init_Controllers();

        ////< 보스는 모두 스킬쓰도록 처리
        //if (G_GameInfo.GameMode == GAME_MODE.INFINITE || G_GameInfo.GameMode == GAME_MODE.SINGLE)
        //{
        //    skill_AI = gameObject.AddComponent<Skill_AI>();
        //    skill_AI.Setup(this);
        //}
    }
    protected override void Init_Datas()
    {
        base.Init_Datas();
    }

    //< 보스일경우 플레이어가 구르기를 했을시 충돌하면 취소시킴
    // not using evasion
    void OnTriggerEnter(Collider col)
    {
        //CancelEvasion(col);
    }

    float OnTriggerStayDelay = 0.1f;
    void OnTriggerStay(Collider col)
    {
        OnTriggerStayDelay -= Time.deltaTime;
        if (OnTriggerStayDelay > 0)
            return;

        OnTriggerStayDelay = 0.1f;
        // CancelEvasion(col);
    }

	/*
    void CancelEvasion(Collider col)
    {
        Unit unit = col.GetComponent<Unit>();
        if (unit == null)
            return;

        if (unit.UnitType == global::UnitType.Unit)
        {
            if (unit.CurrentState == UnitState.Evasion)
            {
                //< 플레이어의 구르기 진행 방향과 자신과의 각도를 검사한다.
                if(Vector3.Distance(unit.transform.forward, (this.transform.position - unit.transform.position).normalized) < 1)
                    unit.ChangeState(UnitState.Idle);
            }
        }
    }
    */
}

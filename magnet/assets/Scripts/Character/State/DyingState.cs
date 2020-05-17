using UnityEngine;
using System.Collections;

///// <summary>
///// 죽을때 기본적으로 뒤로 밀리면서 죽음
///// </summary>
//public class DyingState : UnitStateBase
//{
//    protected Vector3 velocity = Vector3.zero;

//    public override void OnInitialize(Unit _parent)
//    {
//        base.OnInitialize(_parent);
//    }

//    public override void OnEnter(System.Action callback)
//    {
//        base.OnEnter(callback);

//        parent.CharInfo.Hp = 0;

//        //G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING ||
//        if ((parent.UnitType == UnitType.Boss || parent.UnitType == UnitType.Unit || parent.NotDeadPush))
//            parent.PlayAnim(eAnimName.Anim_die, true, 0.2f, true);
//        else
//            parent.PlayAnim(eAnimName.Anim_down, true, 0.2f, true);

//        Resource.AniInfo aniInfo = parent.GetAniData(eAnimName.Anim_die);
//        SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, parent._audioSource, parent.cachedTransform);
//        SoundManager.instance.PlayUnitVoiceSound(parent.GetVoiceID(eAnimName.Anim_die), parent._audioSource, parent.cachedTransform);

//        //< 죽었을때 컬리더박스는 꺼줌
//        BoxCollider[] collider = this.gameObject.GetComponentsInChildren<BoxCollider>();
//        for (int i = 0; i < collider.Length; i++)
//            collider[i].enabled = false;

//        if (GameDefine.skillPushTest)
//        {
//            CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
//            for (int i = 0; i < colider2.Length; i++)
//                colider2[i].enabled = false;
//        }

//        if (parent.UsableNavAgent)
//            parent.GetComponent<NavMeshAgent>().enabled = false;

//        //< 사망시
//        parent.SetMasteryEvent(3, parent);

//        //< 이펙트 삭제
//        parent.EffectClear();

//        if (parent.ExtraModels != null)
//        {
//            for (int i = 0; i < parent.ExtraModels.Count; i++)
//                parent.ExtraModels[i].gameObject.SetActive(false);
//        }

//        parent.GetComponent<NavMeshAgent>().updateRotation = false;
//    }

//    public override void CachedUpdate()
//    {
//        PushUpdate();
//    }

//    //< 죽었을때 뒤로 밀려나는 처리
//    void PushUpdate()
//    {
//        if (!startedPush)
//            return;

//        if (!parent.Model.Main.animation.isPlaying)
//            parent.ChangeState(UnitState.Dead);

//        //G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING ||
//        if ((parent.UnitType == UnitType.Boss || parent.UnitType == UnitType.Unit || parent.NotDeadPush))
//        {

//        }
//        else
//        {
//            parent.GetComponent<NavMeshAgent>().velocity = velocity;
//        }
//    }

//    public override void OnExit(System.Action callback)
//    {
//        if (parent.UnitType == UnitType.Boss)
//            Time.timeScale = GameDefine.DefaultTimeScale;

//        parent.GetComponent<NavMeshAgent>().updateRotation = true;
//        parent.GetComponent<NavMeshAgent>().velocity = Vector3.zero;

//        //< 이펙트 삭제
//        parent.EffectClear();

//        startedPush = false;

//        base.OnExit(callback);
//    }

//    Vector3 ArrivalPos, ArrivalLook;
//    float PushSpeed;
//    public void PushForDie(float power)
//    {
//        Vector3 newForward = -parent.cachedTransform.forward;
//        newForward.y = 0;
//        //parent.cachedTransform.forward = newForward;

//        //< 도착할 위치 지정
//        float pushPower = 5;
//        velocity = newForward * pushPower;

//        startedPush = true;
//    }
//}

/// <summary>
/// 죽을때 기본적으로 뒤로 밀리면서 죽음
/// </summary>
public class DyingState : UnitStateBase
{
    public override void OnInitialize(Unit _parent)
    {
        base.OnInitialize(_parent);
    }

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        parent.CharInfo.Hp = 0;

        //G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING ||
        if ((parent.UnitType == UnitType.Boss || parent.UnitType == UnitType.Unit || parent.NotDeadPush))
            parent.PlayAnim(eAnimName.Anim_die, true, 0.2f, true);
        else
            parent.PlayAnim(eAnimName.Anim_down, true, 0.2f, true);

        Resource.AniInfo aniInfo = parent.GetAniData(eAnimName.Anim_die);
        SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, parent._audioSource, parent.cachedTransform);
        SoundManager.instance.PlayUnitVoiceSound(parent.GetVoiceID(eAnimName.Anim_die), parent._audioSource, parent.cachedTransform);

        //< 죽었을때 컬리더박스는 꺼줌
        BoxCollider[] collider = this.gameObject.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < collider.Length; i++)
            collider[i].enabled = false;

        if (GameDefine.skillPushTest)
        {
            CapsuleCollider[] colider2 = this.gameObject.GetComponentsInChildren<CapsuleCollider>();
            for (int i = 0; i < colider2.Length; i++)
                colider2[i].enabled = false;
        }

        if (parent.UsableNavAgent)
            parent.GetComponent<NavMeshAgent>().enabled = false;

        //< 사망시
        parent.SetMasteryEvent(3, parent);

        //< 이펙트 삭제
        parent.EffectClear();

        if (parent.ExtraModels != null)
        {
            for (int i = 0; i < parent.ExtraModels.Count; i++)
                parent.ExtraModels[i].gameObject.SetActive(false);
        }
    }

    public override void CachedUpdate()
    {
        PushUpdate();
    }

    //< 죽었을때 뒤로 밀려나는 처리
    void PushUpdate()
    {
        //G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING ||
        if ((parent.UnitType == UnitType.Boss || parent.UnitType == UnitType.Unit || parent.NotDeadPush))
        {
            if (!parent.Model.Main.animation.isPlaying)
                parent.ChangeState(UnitState.Dead);
        }
        else
        {
            //< 해당 위치까지 이동시킨다
            if (Vector3.Distance(parent.cachedTransform.position, ArrivalPos) < (PushSpeed * Time.deltaTime))
            {
                if (!parent.Model.Main.animation.isPlaying)
                    parent.ChangeState(UnitState.Dead);
            }
            else
            {

                //올바른 방향으로 계속 조절
                ArrivalLook = ArrivalPos - parent.cachedTransform.position;
                ArrivalLook.Normalize();

                parent.cachedTransform.Translate(ArrivalLook * (PushSpeed * Time.deltaTime), Space.World);
            }
        }
    }

    public override void OnExit(System.Action callback)
    {
        if (parent.UnitType == UnitType.Boss)
            Time.timeScale = GameDefine.DefaultTimeScale;

        //< 이펙트 삭제
        parent.EffectClear();

        base.OnExit(callback);
    }

    Vector3 ArrivalPos, ArrivalLook;
    float PushSpeed;
    public void PushForDie(float power)
    {
        //Push(eAnimName.Anim_none, -transform.forward, power, parent.Animator.GetAnimLength(eAnimName.Anim_down), UnitState.Dead);

        //< 도착 위치를 먼저 얻는다.
        Vector3 EndPos = parent.cachedTransform.position + ((-parent.cachedTransform.forward) * power);

        //Debug.LogWarning("2JW : 111 " + this.transform.position + " : " + transform.forward + " : " + power + " : " + EndPos);
        //< 해당 위치가 이동 가능한 위치인지 검사한다.
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(EndPos, out navHit, Vector3.Distance(EndPos, parent.cachedTransform.position), 9))
        {
            ArrivalPos = navHit.position;
            Debug.Log("PushForDie StartPos:" + parent.cachedTransform.position + " EndPos:" + EndPos + " AdjustPos:" + ArrivalPos + " power: " + power);
        }
        else
        {
            ArrivalPos = parent.cachedTransform.position;
            Debug.LogError("PushForDie Faild Position StartPos:" + parent.cachedTransform.position + " EndPos:" + EndPos + " AdjustPos:" + ArrivalPos + " power: " + power);
        }

        //ArrivalPos = EndPos;
        ArrivalLook = ArrivalPos - parent.cachedTransform.position;
        ArrivalLook.Normalize();

        PushSpeed = power * 3f;
        //PushSpeed = EndPos.magnitude / (power * 2);
    }
}
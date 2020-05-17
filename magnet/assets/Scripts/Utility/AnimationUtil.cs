using UnityEngine;
using System.Collections;

public class AnimationUtil
{
    /// 해당 애니메이션의 플레이 시간을 구해준다.(sec)
    static public float GetAnimLength(Animation targetAnim, string animName, bool relativeSpeed = true)
    {
        AnimationState animState = targetAnim[animName];
        if (null == animState)
            return 0f;

        return relativeSpeed ? (targetAnim[animName].length / animState.speed) : targetAnim[animName].length;
    }


    /// 대상 객체의 애니메이션 속도를 조절한다
    static public void SetAnimationSpeed(GameObject targetGO, float newSpeed)
    {
        foreach (AnimationState state in targetGO.animation)
            state.speed = newSpeed;

        int childCnt = targetGO.transform.childCount;
        for (int i = 0; i < childCnt; i++)
        {
            if (targetGO.transform.GetChild(i).animation != null)
            {
                foreach (AnimationState state in targetGO.transform.GetChild(i).animation)
                {
                    state.speed = newSpeed;
                }
            }
        }
    }

    /// 주어진 애니메이션 리스트들의 속도를 조절한다.
    static public void SetAnimationSpeed(Animation animation, float newSpeed, params string [] animNames)
    {
        foreach (string animName in animNames)
        {
            AnimationState state = animation[animName];

            //일단은 스킬 적용되게
            if (null != state /*&& !animName.Contains("skill")*/)
                state.speed = newSpeed;
        }
    }

    /// 에러.
    static public void SetAnimationPause(Animation animation, string animName, bool pause, float setAnimTime = 0)
    {
        if (animation == null)
            return;

        AnimationState state = animation[animName];
        if (null != state)
        {
            if (setAnimTime > 0)
            {
                state.time = setAnimTime;
                animation.Sample();
            }

            //state.enabled = !pause;
            state.speed = pause ? 0f : 1f;
            animation.Sample();
        }
    }
}

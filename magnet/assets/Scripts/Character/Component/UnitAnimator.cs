using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 현 게임을 위한 클래스
/// </summary>
public class UnitAnimator
{
    public bool                     IsReady { private set; get; }

    public GameObject               Owner;
    public Animation                Animation { private set; get; }    
    public eAnimName                CurrentAnim { set; get; }
    public string                   CurrentAnimName { set; get; }
    public float                    CurrentAnimSpeed { set; get; }
    public AnimationState           CurrentAnimState { set; get; }

    public event System.Action<Animation> OnChangedAnimator;
    public event System.Action<Animation> OnPlayAnimation;

    Resource.AniInfo[]              AnimDatas;

    Unit parent;

    //< 애니메이션 재생 속도
    Dictionary<string, float> AnimLength = new Dictionary<string, float>();
    public Dictionary<string, eAnimName> eAnimNameDic = new Dictionary<string, eAnimName>();

    public UnitAnimator()
    {
        Reset();
    }

    public virtual void Init(GameObject owner, Animation animation, Resource.AniInfo[] _animDatas)
    { 
        if (null == animation)
        { 
            Debug.LogError( "Null 객체로 생성자를 호출할 수 없습니다!" );
            return;
        }
        
        Owner = owner;
        Animation = animation;
        AnimDatas = _animDatas;

        parent = Owner.GetComponent<Unit>();
        
        if (null != OnChangedAnimator)
            OnChangedAnimator( Animation );

        IsReady = true;

        //< 애니메이션 속도를 저장시킨다
        foreach (eAnimName eEnum in eAnimName.GetValues(typeof(eAnimName)))
        {
            if (eEnum == eAnimName.Anim_none || eEnum == eAnimName.Anim_Max)
                continue;

            string aniname = GetAnimName(eEnum);
            if (aniname == "0")
                continue;

            if (Animation.GetClip(aniname) == null)
                continue;

            if (!AnimLength.ContainsKey(aniname))
                AnimLength.Add(aniname, AnimationUtil.GetAnimLength(Animation, aniname));

            if (!eAnimNameDic.ContainsKey(aniname))
            {
                eAnimNameDic.Add(aniname, eEnum);
            }
        }
    }

    public virtual void Reset()
    {
        IsReady = false;

        Animation = null;

        OnChangedAnimator = null;
        OnPlayAnimation = null;

        ClearAnimState();
    }

    public void ClearAnimState()
    {
        CurrentAnim = eAnimName.Anim_none;
        CurrentAnimName = "0";
        CurrentAnimSpeed = 1f;
        CurrentAnimState = null;
    }

    /// <summary>
    /// 실제 애니메이션 플레이
    /// </summary>
    [HideInInspector]
    public bool ChangeAniCheck = false;
    
    public virtual bool PlayAnim(eAnimName animEvent, bool crossFade = false, float crossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false, bool StopCheck = false)
    {
        if (IsPlaying( animEvent ) && !canPlaySameAnim)
            return false;

        // die Animation 중에는 Animation 변경 불가
        if (!canPlaySameAnim && CurrentAnim == eAnimName.Anim_die && animEvent != eAnimName.Anim_die && !parent.Usable)
            return false;

        if (null == Animation)
        {
            Debug.LogWarning( "애니메이션을 플레이할 수 있는 객체가 존재하지 않습니다." );
            return false;
        }

        if (!StopCheck)
        {
            string animName = GetAnimName(animEvent);
            //if (parent != null)
            //    Debug.LogWarning(parent.CurCombo + " : " + animName);

            if (!PlayAnim(animName, crossFade, crossTime, queued))
                return false;
        }
        else
            ChangeAniCheck = true;

        return true;
    }

    public bool PlayAnim(string animName, bool isCrossFade = false, float CrossTime = 0.1f, bool queued = false)
    {
        if (Animation == null || animName == "0" || null == Animation[animName])
            return false;

        if (isCrossFade)
        {
            if (Animation.IsPlaying( animName ))
                Animation[animName].time = 0f;
            
            if (queued && Animation.isPlaying)
                Animation.CrossFadeQueued( animName, CrossTime );
            else
                Animation.CrossFade( animName, CrossTime );
        }
        else
        {
            if (Animation.IsPlaying( animName ))
                Animation[animName].time = 0f;

            if (queued && Animation.isPlaying)
                Animation.PlayQueued(animName);
            else
                Animation.Play(animName);
        }

        CurrentAnim = eAnimNameDic[animName];
        CurrentAnimName = animName;
        CurrentAnimSpeed = Animation[animName].speed;
        CurrentAnimState = Animation[animName];

        if (null != OnPlayAnimation)
            OnPlayAnimation( Animation );

        return true;
    }

    public float GetAnimLength(eAnimName animEvent)
    {
        string animName = GetAnimName( animEvent );
        if (AnimLength.ContainsKey(animName))
            return AnimLength[animName];

        return 0.5f;
    }

    public float GetAnimLength(string animName)
    {
        //return AnimationUtil.GetAnimLength( Animation, animName );

        if (AnimLength.ContainsKey(animName))
            return AnimLength[animName];
        else
        {
            if (GameDefine.TestMode)
                Debug.Log("해당 애니메이션 없음 " + animName);
        }

        return 0.5f;
    }

    public string GetAnimName(eAnimName animEvent)
    {
        if (AnimDatas == null)
            return "1";

        if (AnimDatas.Length <= (int)animEvent || AnimDatas[(int)animEvent] == null)
            return "1";

        return AnimDatas[(int)animEvent].aniName;
    }
    
    public bool IsPlaying(eAnimName animEvent)
    {
        if (Animation == null)
            return false;

        return Animation.IsPlaying( GetAnimName( animEvent ) );
    }

    public bool IsPlaying(string animName)
    {
        return Animation.IsPlaying( animName );
    }

    /// <summary> 애니메이션에 맞는 사운드 </summary>
    public void PlayAnimationSound(eAnimName anim)
    {
        if (AnimDatas.Length <= (int)anim || AnimDatas[(int)anim] == null)
        {
            Debug.LogError("Play Animation Sound error " + anim);
            return;
        }

        SoundManager.instance.PlaySfxSound(AnimDatas[(int)anim].seSkill);
    }

    /// <summary> 애니메이션에 맞는 이펙트 이름 </summary>
    public string GetAnimationEffect(eAnimName anim)
    {
        if (AnimDatas.Length <= (int)anim || AnimDatas[(int)anim] == null)
        {
            Debug.LogError("animation effect index error " + anim);
            return null;
        }

        return AnimDatas[(int)anim].effect;
    }

    public uint GetAnimationSound(eAnimName anim)
    {
        if (AnimDatas.Length <= (int)anim || AnimDatas[(int)anim] == null)
        {
            //Debug.LogError("animation effect index error " + anim);
            return 0;
        }

        return AnimDatas[(int)anim].seSkill;
    }

    public bool PlayAnimationVoice(eAnimName anim)
    {
        if (AnimDatas.Length <= (int)anim || AnimDatas[(int)anim] == null)
        {
            //Debug.LogError("animation effect index error " + anim);
            return false;
        }

        uint voiceSoundID;
        Resource.AniInfo aniData = AnimDatas[(int)anim];

        if (aniData.voice.Count >= 1)
        {
            int voiceSoundArrayIdx = Random.Range(0, aniData.voice.Count);
            voiceSoundID = uint.Parse(aniData.voice[voiceSoundArrayIdx].ToString());
        }
        else
        {
            voiceSoundID = uint.Parse(aniData.voice[0].ToString());
        }

        SoundManager.instance.PlayVoiceSound(voiceSoundID);

        return true;
    }
}
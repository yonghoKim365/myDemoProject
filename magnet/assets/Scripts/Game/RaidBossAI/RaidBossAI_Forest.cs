using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaidBossAI_Forest : RaidBossAIBase
{
    /*
    //<======================================
    //<     레이드에서 사용할 정보들
    //<======================================
    //< 애니메이션 이름
    string[] AniNames = { "hecaton_pt_01", "hecaton_pt_02", "hecaton_pt_03" };
    public override void SetPatternData()
    {
        //AddPattern(new PatternData(1, false, true, 6, ActivePattern));
        AddPattern(new PatternData(0.8f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.6f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.4f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.2f, true, false, 0, ActiveRandomPattern));
    }

    public override void SetGameData()
    {
        //< 이펙트를 미리 로드해놓는다.
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hecaton_pt_jump", 2);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hecaton_pt_landing", 2);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hecaton_pt_4att", 1);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hecaton_pt_rolling", 4);

        //< 애니메이션을 넣어준다.
        AssetbundleLoader.AddAnimationClip(RaidBoss.Animator.Animation, AniNames);
    }

    //< 랜덤 패턴 실행
    public override void ActiveRandomPattern(PatternData _PatternData)
    {
        base.ActiveRandomPattern(_PatternData);

        //< 패턴 실행
        int idx = Random.Range(0, 2);
        if (idx == 1)
            StartCoroutine(ActiveRandomPattern_1());
        else
            StartCoroutine(ActiveRandomPattern_2());
    }

    //< 점프해서 공격
    IEnumerator ActiveRandomPattern_1()
    {
        CameraManager.instance.RtsCamera.Distance = 20;
        NotTargeting = true;

        RaidBoss.ZeroNavAgentRedius(true);

        //< 이벤트상태로 변경
        //RaidBoss.ChangeState(UnitState.Event, true);
        
        int count = 3;
        while(true)
        {
            SpawnEffect("Fx_hecaton_pt_jump", RaidBoss.transform, null, (effect) =>
            {
                RaidBoss.Animator.Animation.CrossFade(AniNames[0], 0.5f);
                AddCameraShake(0, 2, 0);
            });
            
            yield return new WaitForSeconds(1);

            //< 랜덤한 위치에 공격
            SpawnEffect("Fx_hecaton_pt_landing", null, null, (effects) =>
            {
                effects.transform.position = G_GameInfo.PlayerController.Leader.transform.position;
                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                effects.transform.position += effects.transform.forward * Random.Range(0, 4);

                RaidBoss.Model.Main.transform.position = effects.transform.position;
                RaidBoss.Model.Main.transform.rotation = effects.transform.rotation;
                SetTriggerEvent(effects.gameObject, (target, cb) =>
                {
                    //< 대미지를 줌
                    PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 1.5f);
                });

                AddCameraShake(1.8f, 3, 0);
            });

            yield return new WaitForSeconds(1.5f);
            RaidBoss.Animator.Animation.CrossFade(AniNames[1], 0.5f);
            
            yield return new WaitForSeconds(1);

            count--;
            if (count == 0)
                break;

            yield return null;
        }

        //< 마지막은 위치를 보정하기위해 한번더
        SpawnEffect("Fx_hecaton_pt_jump", RaidBoss.Model.Main.transform, null, (effect) =>
        {
            RaidBoss.Animator.Animation.CrossFade(AniNames[0], 0.5f);
            AddCameraShake(0, 2, 0);
        });

        yield return new WaitForSeconds(1);

        //< 랜덤한 위치에 공격
        SpawnEffect("Fx_hecaton_pt_landing", null, null, (effects) =>
        {
            effects.transform.position = G_GameInfo.PlayerController.Leader.transform.position;
            effects.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            effects.transform.position += effects.transform.forward * Random.Range(1, 6);

            RaidBoss.transform.position = effects.transform.position;
            RaidBoss.transform.rotation = effects.transform.rotation;

            RaidBoss.Model.Main.transform.localPosition = Vector3.zero;
            RaidBoss.Model.Main.transform.localRotation = Quaternion.identity;

            SetTriggerEvent(effects.gameObject, (target, cb) =>
            {
                //< 대미지를 줌
                PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 1.5f);
            });

            AddCameraShake(1.8f, 3, 0);
        });

        yield return new WaitForSeconds(1.5f);
        RaidBoss.Animator.Animation.CrossFade(AniNames[1], 0.5f);

        yield return new WaitForSeconds(1);

        NotTargeting = false;
        ActionLive = false;
        RaidBoss.ChangeState(UnitState.Idle);
        RaidBoss.ZeroNavAgentRedius(false);

        if (G_GameInfo.GameInfo != null)
            G_GameInfo.GameInfo.SetCameraUpdate();

        yield return null;
    }

    //< 돌 소환
    IEnumerator ActiveRandomPattern_2()
    {
        //< 이벤트상태로 변경
        //RaidBoss.ChangeState(UnitState.Event, true);

        SpawnEffect("Fx_hecaton_pt_4att", RaidBoss.transform, null, (effects) =>
        {
            RaidBoss.Animator.Animation.CrossFade(AniNames[2], 0.5f);
        });

        StartCoroutine(SpawnStone());

        yield return new WaitForSeconds(2.5f);

        RaidBoss.Animator.Animation.CrossFade(RaidBoss.Animator.GetAnimName(eAnimName.Anim_idle), 0.5f);
        yield return new WaitForSeconds(4);

        ActionLive = false;
        RaidBoss.ChangeState(UnitState.Idle);

        UseEffectClear();

        yield return null;
    }

    IEnumerator SpawnStone()
    {
        int count = 4;
        while(true)
        {
            SpawnEffect("Fx_hecaton_pt_rolling", RaidBoss.transform, null, (effects) =>
            {
                if (G_GameInfo.PlayerController.Leader != null)
                {
                    effects.transform.LookAt(G_GameInfo.PlayerController.Leader.transform);
                    effects.transform.localRotation = Quaternion.Euler(new Vector3(0, effects.transform.localEulerAngles.y, 0));
                }
                else
                    effects.transform.localRotation = Quaternion.Euler(new Vector3(0, RaidBoss.transform.localEulerAngles.y, 0));

                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                effects.transform.position += effects.transform.forward * Random.Range(0, 2);

                //< 컬리더박스 연결
                SetTriggerEvent_Stay(effects.gameObject, 0.5f, (target, cb) =>
                {
                    //< 대미지를 줌
                    PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 0.5f);
                });

                for (int i = 0; i < 5; i++)
                    AddCameraShake(2 + (i * 0.25f), 2);
            });

            count--;
            if (count == 0)
                break;

            yield return new WaitForSeconds(1);
        }
    }
    */
}

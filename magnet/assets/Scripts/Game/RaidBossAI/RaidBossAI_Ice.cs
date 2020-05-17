using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaidBossAI_Ice : RaidBossAIBase
{
    /*
    //<======================================
    //<     레이드에서 사용할 정보들
    //<======================================
    //< 애니메이션 이름
    string[] AniNames = { "akkad_pt_01", "akkad_pt_02" };
    public override void SetPatternData()
    {
        //AddPattern(new PatternData(1, false, true, 6, ActivePattern));
        AddPattern(new PatternData(0.8f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.6f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.4f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.2f, true, false, 0, ActiveRandomPattern));
    }

    List<SpawnUnitData> CreateObjects = new List<SpawnUnitData>();
    public override void SetGameData()
    {
        //< 이펙트를 미리 로드해놓는다.
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_akkad_pt_shield", 2);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_akkad_pt_fallingICE", 5);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_regen_jump_01", 8);

        //< 애니메이션을 넣어준다.
        AssetbundleLoader.AddAnimationClip(RaidBoss.Animator.Animation, AniNames);

        //< 늑대를 미리 생성해둔다.
        for(int i=0; i<12; i++)
        {
            SpawnUnitData nData = new SpawnUnitData();

            GameObject unit = G_GameInfo._GameInfo.SpawnNpc(30104, eTeamType.Team2, 1, RaidBoss.transform.position, RaidBoss.transform.rotation);
            nData.SpawnUnit = unit;
            unit.SetActive(false);

            CreateObjects.Add(nData);
        }
    }

    //< 고정 패턴 실행
    public override void ActivePattern(PatternData _PatternData)
    {
        base.ActivePattern(_PatternData);

        //< 빙석 소환
        StartCoroutine(SpawnIce());

        //< 이건 그냥 시간마다 소환이므로 다른 패턴을 방해하지않음
        ActionLive = false;
    }

    //< 랜덤 패턴 실행
    public override void ActiveRandomPattern(PatternData _PatternData)
    {
        base.ActiveRandomPattern(_PatternData);

        //< 패턴 실행
        int idx = Random.Range(0, 2);

        //< 0일경우 늑대 소환
        if (idx == 0 && CreateObjects.Count > 0)
            StartCoroutine(ActiveRandomPattern_1());
        //< 1일경우 방어막
        else
            StartCoroutine(ActiveRandomPattern_2());
    }

    //< 늑대 소환
    IEnumerator ActiveRandomPattern_1()
    {
        //< 상태 변경
        //RaidBoss.ChangeState(UnitState.Event, true);
        RaidBoss.Animator.Animation.CrossFade(AniNames[1], 0.5f);

        //< 늑대소환
        StartCoroutine(SpawnWolf());

        yield return new WaitForSeconds(2);

        ActionLive = false;
        RaidBoss.ChangeState(UnitState.Idle);

        yield return null;
    }

    IEnumerator SpawnWolf()
    {
        EventListner.instance.TriggerEvent("HUD_RAIDMSG", string.Format("아카드가 늑대들을 소환합니다."));

        int count = 4;
        Vector3 CenterPos = G_GameInfo.PlayerController.Leader.transform.position;
        for (int i = 0; i < CreateObjects.Count; i++)
        {
            NavMeshHit hit;
            while (true)
            {
                Vector3 randomDir = (Vector3.one * 5) + (Random.insideUnitSphere * 5f);
                randomDir.y = 0f;

                NavMesh.SamplePosition(randomDir + CenterPos, out hit, 10f, 1);

                if (float.IsInfinity(hit.position.x))
                    continue;
                if (float.IsInfinity(hit.position.y))
                    continue;
                if (float.IsInfinity(hit.position.z))
                    continue;

                break;
            }

            CreateObjects[i].SpawnUnit.transform.position = hit.position;
            CreateObjects[i].SpawnUnit.transform.LookAt(CenterPos);
            CreateObjects[i].SpawnUnit.SetActive(true);

            Unit target = CreateObjects[i].SpawnUnit.GetComponent<Unit>();
            target.CharInfo.SetTargetStatus(RaidBoss.CharInfo.Stats, 0.1f);

            AssetbundleLoader.AddAnimationClip(target.Animator.Animation, new string[] { "icewolf_regen_01" });
            target.SetIntroEvent(new List<string>() { "icewolf_regen_01" }, "Fx_regen_jump_01");

            CreateObjects.RemoveAt(i);
            i--;

            count--;
            if (count <= 0)
                break;

            yield return new WaitForSeconds(0.5f);
        }
    }

    //< 방어막 처리 + 빙석소환
    IEnumerator ActiveRandomPattern_2()
    {
        //< 이벤트상태로 변경
        //RaidBoss.ChangeState(UnitState.Event, true);
        RaidBoss.Animator.Animation.CrossFade(AniNames[0], 0.5f);

        float LiveTime = 8;
        EventListner.instance.TriggerEvent("HUD_RAIDMSG", string.Format("아카드가 보호막을 시전합니다.\n{0}초간 공격시 대미지를 돌려받습니다", LiveTime));

        //< 이펙트를 실행함
        GameObject Effects = null;
        SpawnEffect("Fx_akkad_pt_shield", null, RaidBoss.transform, (effect) =>
        {
            Effects = effect.gameObject;
            Effects.transform.localPosition = Vector3.zero;
            Effects.transform.localRotation = Quaternion.identity;
        });

        //< 대미지를 받았을경우 모든 플레이어에게 대미지를 준다
        System.Action<Unit, float> call = (attacker, dam) => 
        {
            AllPlayerTakeDamage(dam, true);
        };
        RaidBoss.TakeDamageCallBack += call;

        //< 빙석소환 시작
        StartCoroutine(SpawnIce());

        yield return new WaitForSeconds(LiveTime);

        if (RaidBoss != null)
            RaidBoss.TakeDamageCallBack -= call;

        if (Effects != null)
            UseEffectClear(Effects);

        ActionLive = false;
        RaidBoss.ChangeState(UnitState.Idle, true);
        yield return null;
    }

    //< 빙석 소환
    IEnumerator SpawnIce()
    {
        Vector3 CenterPos = G_GameInfo.PlayerController.Leader.transform.position;

        int count = 6;
        while(true)
        {
            //< 빙석 소환
            SpawnEffect("Fx_akkad_pt_fallingICE", null, null, (effects) =>
            {
                //< 위치는 리더 플레이어 기준으로 랜덤하게 잡는다.
                if (G_GameInfo.PlayerController.Leader != null)
                    effects.transform.position = G_GameInfo.PlayerController.Leader.transform.position;
                else
                    effects.transform.position = CenterPos;

                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                float forwardValue = Random.Range(3, 10);
                effects.transform.position += effects.transform.forward * forwardValue;

                SetTriggerEvent(effects.gameObject, (target, cb) =>
                {
                    //< 대미지를 줌
                    PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 0.4f);
                });

                //< 카메라 쉐이크 추가
                AddCameraShake(2, 3, 0);
            });

            count--;
            if (count <= 0)
                break;

            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
    */
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaidBossAI_Poison : RaidBossAIBase
{
    /*
    //<======================================
    //<     레이드에서 사용할 정보들
    //<======================================
    //< 애니메이션 이름
    public override void SetPatternData()
    {
        AddPattern(new PatternData(1, false, true, 10, ActivePattern));
        AddPattern(new PatternData(0.9f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.75f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.5f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.3f, true, false, 0, ActiveRandomPattern));
    }

    List<SpawnUnitData> CreateObjects = new List<SpawnUnitData>();
    List<SpawnUnitData> CreateUnits = new List<SpawnUnitData>();
    public override void SetGameData()
    {
        //< 이펙트를 미리 로드해놓는다.
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_tyrant_pt_heal");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_tyrant_pt_burst");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_ray");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_egg");

        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_tyrant_pt_swamp_01", 4);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_tyrant_pt_swamp_02", 4);

        //< 소환될 위치에 오브젝트를 미리 생성해둠
        GameObject PatternGroup = GameObject.Find("PatternGroup");
        Transform[] trn = PatternGroup.GetComponentsInChildren<Transform>();
        for (int i = 1; i < trn.Length; i++ )
        {
            SpawnUnitData nData = new SpawnUnitData();
            EventUnit _EventUnit = G_GameInfo._GameInfo.SpawnEventUnit(4800102, "Props/Forest_Prop_001_Orc", trn[i].transform.position, trn[i].transform.rotation, null);
            nData.FirstObj = _EventUnit.gameObject;
            _EventUnit.StaticState(true);
            NGUITools.SetLayer(nData.FirstObj, 13);

            CreateObjects.Add(nData);
        }

        //< 슬라임을 미리 생성해둔다.
        for (int i = 0; i < 20; i++)
        {
            SpawnUnitData nData = new SpawnUnitData();

            EventUnit _EventUnit = G_GameInfo._GameInfo.SpawnEventUnit(4800102, "tyrant_pp_01", RaidBoss.transform.position, RaidBoss.transform.rotation, new string[] { "tyrant_pp_run_01" });
            nData.SpawnUnit = _EventUnit.gameObject;
            _EventUnit.gameObject.SetActive(false);

            CreateUnits.Add(nData);
        }
    }

    //< 고정 패턴 실행
    public override void ActivePattern(PatternData _PatternData)
    {
        base.ActivePattern(_PatternData);

        //< 슬라임 소환
        if (CreateUnits.Count > 0)
            StartCoroutine(SpawnUnitUpdate());

        //< 이건 그냥 시간마다 소환이므로 다른 패턴을 방해하지않음
        ActionLive = false;
    }

    public override void ActiveRandomPattern(PatternData _PatternData)
    {
        base.ActiveRandomPattern(_PatternData);
        ActionLive = false;

        //< 독웅덩이 소환
        StartCoroutine(SpawnPoison());
    }

    IEnumerator SpawnPoison()
    {
        //< 자리를 완전 랜덤하게 선택한다.
        EventListner.instance.TriggerEvent("HUD_RAIDMSG", "어딘가에 독 웅덩이가 발생하였습니다");

        NavMeshHit hit;
        while (true)
        {
            Vector3 randomDir = (Vector3.one * 5) + (Random.insideUnitSphere * 15f);
            randomDir.y = 0f;

            NavMesh.SamplePosition(randomDir + G_GameInfo.PlayerController.Leader.transform.position, out hit, 10f, 1);

            if (float.IsInfinity(hit.position.x))
                continue;
            if (float.IsInfinity(hit.position.y))
                continue;
            if (float.IsInfinity(hit.position.z))
                continue;

            break;
        }

        SpawnEffect("Fx_tyrant_pt_swamp_01", null, null, (effect) =>
        {
            effect.transform.position = hit.position;
            effect.transform.localScale = Vector3.one;
        });

        yield return new WaitForSeconds(2.5f);

        SpawnEffect("Fx_tyrant_pt_swamp_02", null, null, (effect) =>
        {
            effect.transform.position = hit.position;
            effect.transform.localScale = Vector3.one;

            SetTriggerEvent_Stay(effect.gameObject, 3, (targetunit, ter) =>
            {
                if (targetunit.TeamID != 0)
                    return;

                //< 대미지를 줌
                PlayerTakeDamage(targetunit, RaidBoss.CharInfo.Atk * 0.25f);
            });
        });

        yield return null;
    }

    IEnumerator SpawnUnitUpdate()
    {
        //< 어디서 소환할지 랜덤으로
        SpawnUnitData TargetData = CreateObjects[Random.Range(0, CreateObjects.Count)];

        EventListner.instance.TriggerEvent("HUD_RAIDMSG", "슬라임이 생성되고 있습니다.");

        //< 화면에 표시
        ShowTargetCamera(true, TargetData.FirstObj, new Vector3(2.5f, 2.5f, 1));

        //< 해당 유닛에게 이펙트 띄워줌
        GameObject ShowEffet = null;
        SpawnEffect("Fx_vulcanus_pt_ray", TargetData.FirstObj.transform, null, (effect) =>
        {
            ShowEffet = effect.gameObject;
        });

        //< 2초간 대기
        yield return new WaitForSeconds(3);

        if (ShowEffet != null)
            Destroy(ShowEffet);

        //< 죽었으면 패스
        if (RaidBoss == null || RaidBoss.IsDie)
            yield break;

        //< 게임 종료상태라면 대기(혹시 부활할수도있으니)
        while (true)
        {
            if (!G_GameInfo._GameInfo.isEnd)
                break;

            yield return null;
        }

        int buffType = Random.Range(0, 5);

        //< 슬라임 소환
        Unit target = null;
        SpawnEffect("Fx_vulcanus_pt_egg", TargetData.FirstObj.transform, null, (effect) =>
        {
            target = CreateUnits[0].SpawnUnit.GetComponent<Unit>();
            target.gameObject.SetActive(true);

            target.Model.Main.animation.Play("tyrant_pp_run_01");

            CreateUnits.RemoveAt(0);
            NGUITools.SetLayer(target.gameObject, 13);

            //< 위치설정
            target.transform.position = TargetData.FirstObj.transform.position;

            //< 능력치 설정
            (target as EventUnit).ShowUnit(RaidBoss.CharInfo.MaxHp * 0.2f);
            target.StaticState(false);
        });

        yield return new WaitForSeconds(1);

        //< 화면을 꺼줌
        ShowTargetCamera(false, TargetData.FirstObj, Vector3.zero);

        while(true)
        {
            if (target != null)
                break;

            yield return null;
        }

        //< 이제 보스를 향해서 이동한다
        while(true)
        {
            if (RaidBoss == null || RaidBoss.IsDie)
                break;

            if (target.IsDie)
                break;

            Vector3 look = (RaidBoss.transform.position - target.transform.position).normalized;
            target.transform.LookAt(RaidBoss.transform);
            target.transform.Translate(look * (1.2f * Time.deltaTime), Space.World);

            if ((RaidBoss.transform.position - target.transform.position).magnitude < 4.8f)
                break;

            yield return null;
        }

        //< 이녀석이 죽은거라면?
        if (target.IsDie)
        {
            //< 주변 플레이어에게 대미지를 주고, 버프를 걸어준다.
            SpawnEffect("Fx_tyrant_pt_burst", target.transform, null, (effects) =>
            {
                SetTriggerEvent(effects.gameObject, (targetunit, cb) =>
                {
                    if (targetunit.TeamID != 0)
                        return;

                    //< 대미지를 줌
                    PlayerTakeDamage(targetunit, RaidBoss.CharInfo.Atk * 0.5f);

                    //< 버프를 걸어줌
                    AddBuff(targetunit, buffType);

                    SpawnEffect("Fx_tyrant_pt_heal", targetunit.transform, null, (effect) => { });
                });

                AddBuff(G_GameInfo.PlayerController.Leader, buffType);
            });

            //< 타입에 따라서 플레이어에게 버프를 건다.
        }
        else if (!RaidBoss.IsDie)
        {
            //< 그냥 걸어나온거라면 보스에게 버프를 걸어준다.
            SpawnEffect("Fx_tyrant_pt_heal", RaidBoss.transform, null, (effect) =>
            {
                AddBuff(RaidBoss, buffType);
            });

            //< 슬라임은 쥬금
            target.Model.Main.SetActive(false);
            target.ChangeState(UnitState.Dying);
        }

        yield return null;
    }

    void AddBuff(Unit Target, int idx)
    {
        string targetName = Target.TeamID == 0 ? "플레이어" : "타일런트";
        string BuffInfo = "";
        //
        //SkillTablesLowData.BuffInfo nBuff = new SkillTablesLowData.BuffInfo();
        //switch(idx)
        //{
        //    case 0: //< 공격력 증가 
        //        nBuff.Indx = 400000000;
        //        nBuff.name = 40001;
        //        nBuff.rate = 10000000;
        //        nBuff.icon = "icon_golden_spirit";
        //        nBuff.buffType = (byte)BuffType.Attack;
        //        nBuff.factorRate = Target.TeamID == 0 ? 0.05f : 0.2f;
        //        nBuff.overLapCount = 99;
        //        nBuff.durationTime = 999;

        //        BuffInfo = "(일정시간동안 공격력이 증가합니다)";
        //        break;

        //    case 1: //< 공격력 감소
        //        nBuff.Indx = 400000001;
        //        nBuff.name = 40001;
        //        nBuff.rate = 10000000;
        //        nBuff.icon = "icon_provoke_cry";
        //        nBuff.buffType = (byte)BuffType.Attack;
        //        nBuff.factorRate = Target.TeamID == 0 ? -0.1f : -0.05f;
        //        nBuff.overLapCount = 99;
        //        nBuff.durationTime = 999;

        //        BuffInfo = "(일정시간동안 공격력이 감소합니다)";
        //        break;

        //    case 2: //< 방어력 감소 
        //        nBuff.Indx = 400000002;
        //        nBuff.name = 40002;
        //        nBuff.rate = 10000000;
        //        nBuff.icon = "icon_hawk_eye";
        //        nBuff.buffType = (byte)BuffType.Attack;
        //        nBuff.factorRate = Target.TeamID == 0 ? -0.2f : -0.1f;
        //        nBuff.overLapCount = 99;
        //        nBuff.durationTime = 999;

        //        BuffInfo = "(일정시간동안 방어력이 감소합니다)";
        //        break;

        //    case 3: //< 체력 회복
        //        nBuff.Indx = 400000003;
        //        nBuff.name = 40018;
        //        nBuff.rate = 10000000;
        //        nBuff.icon = "icon_heal";
        //        nBuff.buffType = (byte)BuffType.HealDot;
        //        nBuff.baseFactor = 3;
        //        nBuff.factorRate = Target.TeamID == 0 ? 0.03f : 0.035f;
        //        nBuff.overLapCount = 99;
        //        nBuff.tic = 2;
        //        nBuff.durationTime = 10;

        //        BuffInfo = "(일정시간동안 체력이 회복됩니다)";
        //        break;

        //    case 4: //< 무적
        //        nBuff.Indx = 400000004;
        //        nBuff.name = 40018;
        //        nBuff.rate = 10000000;
        //        nBuff.icon = "icon_shield_charge";
        //        nBuff.buffType = (byte)BuffType.AllImmune;
        //        nBuff.baseFactor = 3;
        //        nBuff.factorRate = 0.05f;
        //        nBuff.overLapCount = 99;
        //        nBuff.durationTime = Target.TeamID == 0 ? 6 : 8;

        //        BuffInfo = "(일정시간동안 무적효과를 얻습니다)";
        //        break;
        //}
        //Target.BuffCtlr.AttachBuff(Target, Target, nBuff, null);
        //

        EventListner.instance.TriggerEvent("HUD_RAIDMSG", string.Format("{0}에게 버프가 적용되었습니다\n{1}", targetName, BuffInfo));
    }
        */
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//<=================================================
//<     허큘러스 레이드 패턴 업데이트 클래스
//<=================================================
public class RaidBossAI_Metal : RaidBossAIBase
{
    /*
    //<======================================
    //<     레이드에서 사용할 정보들
    //<======================================
    //< 애니메이션 이름
    string[] AniNames = { "hercules_pt_01" };           //< 허큘러스에 추가할 애니
    string[] ObjectAniNames = { "hercules_pt_02" };     //< 소환되는 버프 검에 추가할 애니

    public override void SetPatternData()
    {
        AddPattern(new PatternData(1, false, true, 20, ActivePattern));
        AddPattern(new PatternData(0.8f, true, false, 0, ActivePattern));
        AddPattern(new PatternData(0.6f, true, false, 0, ActivePattern));
        AddPattern(new PatternData(0.4f, true, false, 0, ActivePattern));
        AddPattern(new PatternData(0.2f, true, false, 0, ActivePattern));
    }

    List<Unit> CreateObjects = new List<Unit>();

    List<SpawnUnitData> CreateUnits = new List<SpawnUnitData>();
    public override void SetGameData()
    {
        //< 이펙트를 미리 로드해놓는다.
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hercules_pt_01");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hercules_pt_buff_01");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hercules_pt_buff_02");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hercules_pt_buff_03");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_hercules_pt_buff_04");

        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_ray");

        //< 애니메이션을 넣어준다.
        AssetbundleLoader.AddAnimationClip(RaidBoss.Animator.Animation, AniNames);

        //< 검은 미리 생성해둔다
        GameObject PatternGroup = GameObject.Find("PatternSwordGroup");
        Transform[] trn = PatternGroup.GetComponentsInChildren<Transform>();
        for (int i = 1; i < trn.Length; i++)
        {
            EventUnit _EventUnit = G_GameInfo._GameInfo.SpawnEventUnit(4800102, "hercules_pt_02", trn[i].transform.position, trn[i].transform.rotation, ObjectAniNames);
            _EventUnit.gameObject.SetActive(false);
            CreateObjects.Add(_EventUnit);
        }

        //< 석상 유닛도 미리 생성해둔다
        GameObject PatternUnitGroup = GameObject.Find("PatternUnitGroup");
        trn = PatternUnitGroup.GetComponentsInChildren<Transform>();
        for(int i=1; i<trn.Length; i++)
        {
            uint NpcLowID = 0;
            if (i == 1) NpcLowID = 401107;
            else if (i == 2) NpcLowID = 601508;
            else if (i == 3) NpcLowID = 801007;
            else if (i == 4) NpcLowID = 701407;

            SpawnUnitData nData = new SpawnUnitData();
            EventUnit _EventUnit = G_GameInfo._GameInfo.SpawnEventUnit(NpcLowID, "hercules_ptm_0" + i.ToString(), trn[i].transform.position, trn[i].transform.rotation, null);
            nData.FirstObj = _EventUnit.gameObject;

            //< 타겟이 안되도록 처리해준다.
            _EventUnit.StaticState(true);

            //< 소환될 유닛도 미리 만들어놓는다
            GameObject unit = G_GameInfo.GameInfo.SpawnNpc(NpcLowID, eTeamType.Team2, 1, trn[i].transform.position, trn[i].transform.rotation);
            nData.SpawnUnit = unit;
            unit.SetActive(false);

            nData.idx = i;

            CreateUnits.Add(nData);

            NGUITools.SetLayer(nData.FirstObj, 13);
            NGUITools.SetLayer(nData.SpawnUnit, 13);
        }
    }

    //< 고정 패턴 실행
    public override void ActivePattern(PatternData _PatternData)
    {
        base.ActivePattern(_PatternData);

        //< 타임 반복이라면 유닛소환
        if (_PatternData.TimeReplay)
        {
            if (CreateUnits.Count > 0)
                StartCoroutine(SpawnUnitUpdate());
        }
        //< 아니라면 검 소환
        else
        {
            if (CreateObjects.Count > 0)
                StartCoroutine(SpawnBuffSword());
        }
    }

    IEnumerator SpawnUnitUpdate()
    {
        SpawnUnitData TargetData = CreateUnits[Random.Range(0, CreateUnits.Count)];
        Unit TargetUnit = TargetData.FirstObj.GetComponent<Unit>();
        CreateUnits.Remove(TargetData);

        EventListner.instance.TriggerEvent("HUD_RAIDMSG", "석상이 깨어나기 시작합니다.");

        //< 해당 석상에게 이펙트를 뿌려줌
        GameObject ShowEffet = null;
        SpawnEffect("Fx_vulcanus_pt_ray", TargetUnit.transform, null, (effect) =>
        {
            ShowEffet = effect.gameObject;
        });

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

        //< 유닛 소환
        SpawnEffect("Fx_vulcanus_pt_egg", TargetUnit.transform, null, (effect) =>
        {
            TargetData.SpawnUnit.SetActive(true);
            NGUITools.SetLayer(TargetData.SpawnUnit, 13);

            Unit target = TargetData.SpawnUnit.GetComponent<Unit>();
            target.CharInfo.SetTargetStatus(RaidBoss.CharInfo.Stats, 0.2f);
            target.StaticState(false);

            if (G_GameInfo.PlayerController.Leader != null)
                target.SetTarget(G_GameInfo.PlayerController.Leader.GetInstanceID());

            TargetUnit.ChangeState(UnitState.Dying);

            //< 인덱스에 맞게 능력치를 다시셋팅한다.
            switch(TargetData.idx)
            {
                case 0: //< 공격력 높음
                    target.CharInfo.Stats[AbilityType.DAMAGE].Value = RaidBoss.CharInfo.Stats[AbilityType.DAMAGE].Value * 0.5f;
                    break;

                case 1: //< 방어력 높음
                    target.CharInfo.Stats[AbilityType.DAMAGE_DECREASE].Value = RaidBoss.CharInfo.Stats[AbilityType.DAMAGE_DECREASE].Value * 1;
                    break;

                case 2: //< 속성 방어력 높음
                    //target.CharInfo.Stats[AbilityType.AttriButeDcreAtk].Value = RaidBoss.CharInfo.Stats[AbilityType.AttriButeDcreAtk].Value * 2;
                    break;

                case 3: //< 셋다 높음
                    target.CharInfo.Stats[AbilityType.DAMAGE].Value = RaidBoss.CharInfo.Stats[AbilityType.DAMAGE].Value * 0.5f;
                    target.CharInfo.Stats[AbilityType.DAMAGE_DECREASE].Value = RaidBoss.CharInfo.Stats[AbilityType.DAMAGE_DECREASE].Value * 1;
                    //target.CharInfo.Stats[AbilityType.AttriButeDcreAtk].Value = RaidBoss.CharInfo.Stats[AbilityType.AttriButeDcreAtk].Value * 2;
                    break;
            }
        });

    }

    IEnumerator SpawnBuffSword()
    {
        CameraManager.instance.RtsCamera.Distance = 30;

        //< 버프 타입을 랜덤으로 얻는다.
        byte _BuffType = (byte)Random.Range(1, 5);

        //< 이벤트상태로 변경
        //RaidBoss.ChangeState(UnitState.Event, true);
        SpawnEffect("Fx_hercules_pt_01", RaidBoss.transform, null, (effect) =>
        {
            RaidBoss.Animator.Animation.CrossFade(AniNames[0], 0.5f);
        });
        yield return new WaitForSeconds(1.5f);

        ActionLive = false;
        RaidBoss.ChangeState(UnitState.Idle, true);

        //< 위치를 랜덤으로 구한다
        Dictionary<float, Unit> ChoiceUnit = new Dictionary<float, Unit>();
        Vector3 target = G_GameInfo.PlayerController.Leader.transform.position;
        for(int i=0; i<CreateObjects.Count; i++)
        {
            float dis = Vector3.Distance(target, CreateObjects[i].transform.position);
            if(!ChoiceUnit.ContainsKey(dis))
                ChoiceUnit.Add(dis, CreateObjects[i]);
        }

        //< 두번쨰로 가까운 대상으로 소환시켜준다.
        EventUnit _EventUnit = null;
        foreach (KeyValuePair<float, Unit> dic in ChoiceUnit)
        {
            _EventUnit = dic.Value as EventUnit;
            break;
        }
        CreateObjects.Remove(_EventUnit);

        //< 화면에 표시
        ShowTargetCamera(true, _EventUnit.gameObject, new Vector3(3.5f, 3.5f, 1.5f));

        //< 플레이
        GameObject SwordEffect = null;
        SpawnEffect("Fx_hercules_pt_buff_0" + _BuffType.ToString(), _EventUnit.transform, null, (effect) =>
        {
            _EventUnit.gameObject.SetActive(true);
            NGUITools.SetLayer(_EventUnit.gameObject, 13);
            _EventUnit.SetSizeData(new Vector2(2,3));
            _EventUnit.Model.Main.transform.localScale = Vector3.one;
            _EventUnit.Model.Main.animation.Play(ObjectAniNames[0]);
            SwordEffect = effect.gameObject;
        });

        _EventUnit.ShowUnit(RaidBoss.CharInfo.MaxHp * 0.15f);

        //< 버프 적용
        SkillTablesLowData.BuffInfo orgBuff = SetBuff(true, _BuffType);

        string buffinfo = "";
        if (_BuffType == 1) buffinfo = "유지되는동안 보스의 공격력과 방어력이 증가합니다";
        else if (_BuffType == 2) buffinfo = "유지되는동안 보스의 공격력이 증가합니다";
        else if (_BuffType == 3) buffinfo = "유지되는동안 보스의 방어력이 증가합니다";
        else if (_BuffType == 4) buffinfo = "유지되는동안 일정시간마다 보스의 체력이 회복됩니다";

        EventListner.instance.TriggerEvent("HUD_RAIDMSG", "검을 파괴하세요!\n" + buffinfo);

        if (G_GameInfo.GameInfo != null)
            G_GameInfo.GameInfo.SetCameraUpdate();

        //< 살아있는 동안 꾸준히 대미지를 준다.
        WaitForSeconds wait = new WaitForSeconds(1);
        while(true)
        {
            yield return wait;

            if (_EventUnit == null || _EventUnit.IsDie)
                break;

            yield return wait;

            //< 대미지는 여기서 줌
            AllPlayerTakeDamage(RaidBoss.CharInfo.Atk * 0.2f, true);
        }

        //< 버프 해제
        SetBuff(false, _BuffType, orgBuff);

        if (SwordEffect != null)
            Destroy(SwordEffect);
        yield return null;
    }

    int[] BuffCount = new int[5];
    SkillTablesLowData.BuffInfo SetBuff(bool type, byte _BuffType, SkillTablesLowData.BuffInfo orgBuff = null)
    {
        
        if (RaidBoss == null || RaidBoss.BuffCtlr == null)
            return null;

        if (GameDefine.TestMode)
            Debug.Log("SetBuff " + _BuffType);

        SkillTablesLowData.BuffInfo nBuff = null;

        
        ////< 타입 1은 공격력, 방어력 둘다 추가이므로 방식이 조금 다름
        //if(_BuffType == 1)
        //{
        //    if (type)
        //    {
        //        nBuff = AddBuff(0);
        //        nBuff.Indx = 400000000;
        //        RaidBoss.BuffCtlr.AttachBuff(RaidBoss, RaidBoss, nBuff, null);

        //        nBuff = AddBuff(1);
        //        nBuff.Indx = 400000001;
        //        RaidBoss.BuffCtlr.AttachBuff(RaidBoss, RaidBoss, nBuff, null);
        //    }
        //    else
        //    {
        //        if (RemoveBuff(0))
        //            RaidBoss.BuffCtlr.DetachBuff(400000000);
        //        if (RemoveBuff(1))
        //            RaidBoss.BuffCtlr.DetachBuff(400000001);
        //    }
        //}
        //else
        //{
        //    if (type)
        //    {
        //        nBuff = AddBuff(_BuffType);
        //        RaidBoss.BuffCtlr.AttachBuff(RaidBoss, RaidBoss, nBuff, null);
        //    }
        //    else
        //    {
        //        //< 누적 되었을수도 있기때문에 카운트를 감소시켜줌
        //        if (RemoveBuff(_BuffType))
        //            RaidBoss.BuffCtlr.DetachBuff(orgBuff.Indx);
        //    }
        //}
        

        return nBuff;
    }

    SkillTablesLowData.BuffInfo AddBuff(int type)
    {
        SkillTablesLowData.BuffInfo nBuff = new SkillTablesLowData.BuffInfo();
        BuffCount[type]++;

        //< 공격력 증가
        if(type == 2 || type == 0)
        {
            nBuff.Indx = 400000002;
            nBuff.name = 40001;
            nBuff.rate = 10000000;
            nBuff.icon = "icon_golden_spirit";
            nBuff.buffType = (byte)BuffType.Attack;
            nBuff.factorRate = 0.2f;
            nBuff.overLapCount = 99;
            nBuff.durationTime = 999;
        }

        //< 방어력 증가
        else if (type == 3 || type == 1)
        {
            nBuff.Indx = 400000003;
            nBuff.name = 40002;
            nBuff.rate = 10000000;
            nBuff.icon = "icon_soul_cry";
            nBuff.buffType = (byte)BuffType.Def;
            nBuff.factorRate = 0.2f;
            nBuff.overLapCount = 99;
            nBuff.durationTime = 999;
        }

        //< 체력 회복
        else if(type == 4)
        {
            nBuff.Indx = 400000004;
            nBuff.name = 40018;
            nBuff.rate = 10000000;
            nBuff.icon = "icon_heal";
            nBuff.buffType = (byte)BuffType.HealDot;
            nBuff.baseFactor = 3;
            nBuff.factorRate = 0.02f;
            nBuff.overLapCount = 99;
            nBuff.tic = 2;
            nBuff.durationTime = 999;
        }

        return nBuff;
    }

    bool RemoveBuff(int type)
    {
        BuffCount[type]--;
        if (BuffCount[type] == 0)
        {
            BuffCount[type] = 0;
            return true;
        }

        return false;
    }
    */
}

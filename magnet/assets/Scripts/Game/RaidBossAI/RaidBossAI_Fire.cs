using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//<=================================================
//<     불카누스 레이드 패턴 업데이트 클래스
//<=================================================
public class RaidBossAI_Fire : RaidBossAIBase
{
    /*
    //<======================================
    //<     레이드에서 사용할 정보들
    //<======================================
    //< 애니메이션 이름
    string[] AniNames = { "vulcanus_pt_01", "vulcanus_pt_02" };
    string[] EggAniNames = { "vulcanus_pt_03" };

    public override void SetPatternData()
    {
        AddPattern(new PatternData(1, false, true, 20, ActivePattern));
        AddPattern(new PatternData(0.8f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.6f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.4f, true, false, 0, ActiveRandomPattern));
        AddPattern(new PatternData(0.2f, true, false, 0, ActiveRandomPattern));
    }

    List<SpawnUnitData> CreateObjects = new List<SpawnUnitData>();
    public override void SetGameData()
    {
        //< 이펙트를 미리 로드해놓는다.
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_breath");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_meteor", 5);
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_egg");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_depart");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_landing");
        G_GameInfo._GameInfo.FillSpawnPool(G_GameInfo._GameInfo.effectPool, "Fx_vulcanus_pt_ray");

        //< 애니메이션을 넣어준다.
        AssetbundleLoader.AddAnimationClip(RaidBoss.Animator.Animation, AniNames);

        //< 알을 미리 생성해둔다.
        GameObject PatternGroup = GameObject.Find("PatternGroup");
        Transform[] trn = PatternGroup.GetComponentsInChildren<Transform>();
        for (int i = 1; i < trn.Length; i++)
        {
            SpawnUnitData nData = new SpawnUnitData();
            EventUnit _EventUnit = G_GameInfo._GameInfo.SpawnEventUnit(4800102, "vulcanus_pt_03", trn[i].transform.position, trn[i].transform.rotation, EggAniNames);
            nData.FirstObj = _EventUnit.gameObject;
            
            //< 타겟이 안되도록 처리해준다.
            _EventUnit.StaticState(true);

            //< 소환될 유닛도 미리 만들어놓는다
            GameObject unit = G_GameInfo.GameInfo.SpawnNpc(50204, eTeamType.Team2, 1, trn[i].transform.position, trn[i].transform.rotation);
            nData.SpawnUnit = unit;
            unit.SetActive(false);

            CreateObjects.Add(nData);

            NGUITools.SetLayer(nData.FirstObj, 13);
            NGUITools.SetLayer(nData.SpawnUnit, 13);
        }
    }

    //< 고정 패턴 실행
    public override void ActivePattern(PatternData _PatternData)
    {
        base.ActivePattern(_PatternData);

        if (CreateObjects.Count == 0)
            return;

        //<=======================================
        //<         알을 소환하기 시작한다.
        //<=======================================
        StartCoroutine(ActivePattern());
    }

    //< 랜덤 패턴 실행
    public override void ActiveRandomPattern(PatternData _PatternData)
    {
        base.ActiveRandomPattern(_PatternData);

        //< 패턴 실행
        int idx = Random.Range(0, 2);

        //< 0일경우 메테오
        if(idx == 0)
            StartCoroutine(ActiveRandomPattern_1());
        //< 1일경우 브레스
        else
            StartCoroutine(ActiveRandomPattern_2());
    }

    IEnumerator ActivePattern()
    {
        SpawnUnitData TargetData = CreateObjects[Random.Range(0, CreateObjects.Count)];
        Unit TargetUnit = TargetData.FirstObj.GetComponent<Unit>();
        CreateObjects.Remove(TargetData);

        EventListner.instance.TriggerEvent("HUD_RAIDMSG", "알이 깨어나기 시작합니다.\n알을 파괴하세요!");

        //< 화면에 표시
        ShowTargetCamera(true, TargetData.FirstObj, new Vector3(2.5f, 2.5f, 1));

        //< 해당 유닛에게 이펙트 띄워줌
        GameObject ShowEffet = null;
        SpawnEffect("Fx_vulcanus_pt_ray", TargetUnit.transform, null, (effect) =>
        {
            //< 활성화
            ShowEffet = effect.gameObject;
            TargetUnit.StaticState(false);
            (TargetUnit as EventUnit).ShowUnit(RaidBoss.CharInfo.MaxHp * 0.1f);
            TargetUnit.Model.Main.animation.Play(EggAniNames[0]);
        });

        yield return new WaitForSeconds(10);

        if (ShowEffet != null)
            Destroy(ShowEffet);

        //< 죽었으면 패스
        if (TargetUnit == null || TargetUnit.IsDie || RaidBoss == null || RaidBoss.IsDie)
            yield break;

        //< 게임 종료상태라면 대기(혹시 부활할수도있으니)
        while(true)
        {
            if (!G_GameInfo._GameInfo.isEnd)
                break;

            yield return null;
        }

        //< 안죽었다면 유닛 소환
        SpawnEffect("Fx_vulcanus_pt_egg", TargetUnit.transform, null, (effect) =>
        {
            TargetData.SpawnUnit.SetActive(true);
            NGUITools.SetLayer(TargetData.SpawnUnit, 13);

            Unit target = TargetData.SpawnUnit.GetComponent<Unit>();
            target.CharInfo.SetTargetStatus(RaidBoss.CharInfo.Stats, 0.1f);
            target.StaticState(false);

            if (G_GameInfo.PlayerController.Leader != null)
                target.SetTarget(G_GameInfo.PlayerController.Leader.GetInstanceID());

            TargetUnit.ChangeState(UnitState.Dying);
        });

        yield return new WaitForSeconds(2);
    }

    IEnumerator FlyUpdate(bool up)
    {
        CameraManager.instance.RtsCamera.Distance = 24;

        if (up)
        {
            //< 이벤트상태로 변경
            //RaidBoss.ChangeState(UnitState.Event, true);
            RaidBoss.ZeroNavAgentRedius(true);
            NotTargeting = true;

            //< 날아가는 애니메이션 먼저 실행
            RaidBoss.Animator.Animation.CrossFade(AniNames[0], 0.5f);
            SpawnEffect("Fx_vulcanus_pt_depart", RaidBoss.transform, null,  (effects) =>
            {
                SetTriggerEvent(effects.gameObject, (target, cb) =>
                {
                    //< 푸쉬
                    target.SetPush(6, RaidBoss.gameObject, 2);
                });
            });
            yield return new WaitForSeconds(1);

            AddCameraShake(0, 3, 0);
            yield return new WaitForSeconds(2);
        }
        else
        {
            //< 다시 착륙
            RaidBoss.Animator.Animation.Play(AniNames[1]);
            yield return new WaitForSeconds(1.6f);

            SpawnEffect("Fx_vulcanus_pt_landing", RaidBoss.transform, null, (effects) =>
            {
                SetTriggerEvent(effects.gameObject, (target, cb) =>
                {
                    //< 푸쉬
                    target.SetPush(6, RaidBoss.gameObject, 2);

                    //< 대미지를 줌
                    PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 1.5f);
                });
            });

            AddCameraShake(0, 3, 0);
            yield return new WaitForSeconds(1);

            ActionLive = false;
            NotTargeting = false;
            RaidBoss.ChangeState(UnitState.Idle, true);
            RaidBoss.ZeroNavAgentRedius(false);

            if (G_GameInfo.GameInfo != null)
                G_GameInfo.GameInfo.SetCameraUpdate();

            yield return new WaitForSeconds(2);

            //< 사용했던 이펙트 초기화
            UseEffectClear();
        }

        yield return null;
    }

    //< 메테오 업데이트
    IEnumerator ActiveRandomPattern_1()
    {
        //< 이륙
        yield return StartCoroutine(FlyUpdate(true));

        Vector3 CenterPos = G_GameInfo.PlayerController.Leader.transform.position;
        int CreateCount = 6;
        while(true)
        {
            //< 메테오 생성
            SpawnEffect("Fx_vulcanus_pt_meteor", null, null, (effects) =>
            {
                //< 위치는 리더 플레이어 기준으로 랜덤하게 잡는다.
                if (G_GameInfo.PlayerController.Leader != null)
                    effects.transform.position = G_GameInfo.PlayerController.Leader.transform.position;
                else
                    effects.transform.position = CenterPos;

                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                float forwardValue = Random.Range(2, 10);
                effects.transform.position += effects.transform.forward * forwardValue;

                SetTriggerEvent(effects.gameObject, (target, cb) =>
                {
                    //< 대미지를 줌
                    PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 1.5f);
                });

                //< 카메라 쉐이크 추가
                AddCameraShake(2, 3, 0);
            });

            CreateCount--;
            if (CreateCount <= 0)
                break;
            
            //< 대기
            yield return new WaitForSeconds(0.8f);
        }

        //< 마지막 메테오 떨어지는 시간 대기
        yield return new WaitForSeconds(2);

        //< 착륙
        yield return StartCoroutine(FlyUpdate(false));
    }

    //< 브레스 업데이트
    IEnumerator ActiveRandomPattern_2()
    {
        //< 이륙
        yield return StartCoroutine(FlyUpdate(true));

        //< 이펙트 실행
        SpawnEffect("Fx_vulcanus_pt_breath",  RaidBoss.transform, null, (effects) =>
        {
            if(G_GameInfo.PlayerController.Leader != null)
            {
                effects.transform.LookAt(G_GameInfo.PlayerController.Leader.transform);
                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, effects.transform.localEulerAngles.y, 0));
            }
            else
                effects.transform.localRotation = Quaternion.Euler(new Vector3(0, RaidBoss.transform.localEulerAngles.y, 0));

            //< 컬리더박스 연결
            SetTriggerEvent_Stay(effects.gameObject, 0.5f, (target, cb) =>
            {
                //< 대미지를 줌
                PlayerTakeDamage(target, RaidBoss.CharInfo.Atk * 0.5f);
            });

            for (int i = 0; i < 10; i++ )
                AddCameraShake(2 + (i * 0.25f), 2);
        });
        yield return new WaitForSeconds(3);
        yield return StartCoroutine(FlyUpdate(false));
    }
    */
}

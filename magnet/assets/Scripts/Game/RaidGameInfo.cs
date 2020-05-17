using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaidGameInfo : GameInfoBase
{
    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.RAID; } }
    public ulong verifyToken { set; get; }

    //public DungeonTable.StageTableInfo playStageInfo { set; get; }

    public int BossDropGold;
    public int NormalDropGold;

    public static bool DisableHit = false;
    /// <summary>
    /// 프로토 타입 몬스터 모두 죽이기 플래그
    /// </summary>
    public bool ProtoKillAllFlag = false;

    protected override void InitDatas()
    {
        base.InitDatas();

        //< 컷씬 정보 미리 로드
        CutSceneMgr.LoadCutScene();
    }

    protected override void InitManagers()
    {
        base.InitManagers();

        G_GameInfo.CharacterMgr.allrUUIDDic.Clear();
        
        _PlayerSyncData syncData = NetData.instance._playerSyncData;

        CreatePlayerController(syncData.playerSyncDatas[0], syncData.partnerSyncDatas);

        InitSpawnCtlr();
    }

    protected override void InitUI()
    {
        base.InitUI();

        //인게임 HUD패널 생성
        HudPanel = UIMgr.Open("UIPanel/InGameHUDPanel").GetComponent<InGameHUDPanel>();
        //	   Debug.LogWarning("2JW : InitUI() In - " + HudPanel);
    }

    void InitSpawnCtlr()
    {
        // 난이도 선택 및 NPCSpawn 시작!
        SpawnController[] spawnCtlrs = GameObject.FindObjectsOfType<SpawnController>();
        if (null == spawnCtlrs || 0 == spawnCtlrs.Length)
            spawnCtlr = ResourceMgr.InstAndGetComponent<SpawnController>("MapComponent/SpawnController");
        else
            spawnCtlr = spawnCtlrs[0];

        spawnCtlr.Init(G_GameInfo.PlayerController.Leader.gameObject);

        //<==========================================
        //          스폰될 NPC들 설정해주기
        //<==========================================
        //Dictionary<uint, StageLowData.NpcPostingInfo> placementDic = LowDataMgr.GetSingleStagePlacementDatas(StageInfo.stageId);
        spawnGroup = spawnCtlr.GetList<SpawnGroup>();

        int totalMonster = 0;
        List<string> SpawnEffects = new List<string>();
        for (int i = 0; i < spawnGroup.Count; i++)
        {
            SpawnGroup group = spawnGroup[i];
            totalMonster += group.spawnerList.Count;
            for (int spawnerNo = 0; spawnerNo < group.spawnerList.Count; spawnerNo++)
            {
                // NPCSpawner만 골라서 처리해주도록 한다.
                NPCSpawner npcSpawner = group.spawnerList[spawnerNo] as NPCSpawner;
                if (null == npcSpawner)
                    continue;

                npcSpawner.Owner = group;

                // NPCSpawner에서 SpawnNo를 얻어와서.
                //if (!placementDic.ContainsKey((uint)npcSpawner.UnitNum))
                //{
                //    if (GameDefine.TestMode)
                //        Debug.Log("UnitLowData.DataInfo Index Error UnitNum : " + npcSpawner.UnitNum + " , StageId : " + StageInfo.stageId);

                //    // 없는 데이터는 스폰안되도록하기.
                //    npcSpawner.unitId = 0;
                //    continue;
                //}

                //// 스테이지 정보 데이터 기반으로 스폰될 유닛 셋팅해주기.
                //npcSpawner.unitId = (int)placementDic[(uint)npcSpawner.UnitNum].unitId;
                //npcSpawner.GroupNo = (int)placementDic[(uint)npcSpawner.UnitNum].groupNo;
                //npcSpawner.SetEventData(placementDic[(uint)npcSpawner.UnitNum]);

                ////< 출현할때 이펙트 리스트를 저장
                //if (placementDic[(uint)npcSpawner.UnitNum].intro == 1 && !SpawnEffects.Contains(placementDic[(uint)npcSpawner.UnitNum].introEffect))
                //    SpawnEffects.Add(placementDic[(uint)npcSpawner.UnitNum].introEffect);
            }

            // 스폰불가능한 Spawner는 지우도록 한다.
            group.spawnerList.RemoveAll((spawner) =>
            {
                bool removed = false;
                if (spawner is NPCSpawner)
                    removed = (spawner as NPCSpawner).unitId == 0;

                if (removed)
                    DestroyImmediate((spawner as NPCSpawner).gameObject);

                return removed;
            });
        }

        spawnCtlr.StartController();

        //< 트랩 스폰 
        //TrapSpawnController[] TrapspawnCtlrs = GameObject.FindObjectsOfType<TrapSpawnController>();
        //if (TrapspawnCtlrs != null && TrapspawnCtlrs.Length > 0)
        //{
        //    TrapSpawnController TrapspawnCtlr = TrapspawnCtlrs[0];
        //    TrapspawnCtlr.Init();
        //}

        //< 이펙트를 풀에 생성해둔다
        for (int i = 0; i < SpawnEffects.Count; i++)
            FillSpawnPool(effectPool, SpawnEffects[i], 3);

        int dropGold = NetData.instance._RewardData.GetCoin;
        NormalDropGold = Mathf.CeilToInt(dropGold / (totalMonster + 5));
        BossDropGold = dropGold - (NormalDropGold * (totalMonster - 1));
    }

    protected override IEnumerator GameStartReady()
    {
        while (true)
        {
            if (RaidGameState.IsMapLoad)
                break;

            yield return null;
        }

        SceneManager.instance.ShowLoadingTipPanel(false);

        float delay = 1f + Time.time;
        while (true)
        {
            if (delay < Time.time)
                break;

            yield return null;
        }

        DungeonTable.SingleBossRaidInfo lowData = null;
        _LowDataMgr.instance.RefLowDataBossRaid(StageId, ref lowData);
        TimeLimit = lowData.limitTime;

        GameStart();
    }

    protected override void OnDieUnit(Unit deadUnit)
    {
        base.OnDieUnit(deadUnit);

        if (NetData.instance._RewardData == null)
        {
            Debug.LogError("RewardData null error");
            return;
        }

        int addGold = 0;
        if (deadUnit is Boss)
        {
            addGold = BossDropGold;//나머지 다줌
        }
        else if (deadUnit is Npc)
        {
            if ((deadUnit as Npc).isMiddleBoss)
                addGold = NormalDropGold;
            else
                addGold = NormalDropGold;
        }
        else
        {
            return;
        }

        //GetGold += addGold;
        base.AddGold(deadUnit, addGold);
    }

    public override void EndGame(bool isSuccess)
    {
        base.EndGame(isSuccess);

        if (RaidGameState.IsTest)
        {
            SceneManager.instance.ActionEvent(_ACTION.GO_TOWN);
            RaidGameState.IsTest = false;
            return;
        }

        HudPanel.GameEnd(isSuccess);
        StartCoroutine(ResultAction(isSuccess));
    }

    IEnumerator ResultAction(bool isSuccess)
    {
        yield return new WaitForSeconds(1);
        
        UIMgr.instance.UICamera.enabled = true;

        HudPanel.Hide();
        UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();
        NetworkClient.instance.SendPMsgBossBattleCompleteC(StageId, isSuccess, NetData.instance._RewardData.GetExp);
        
        //if (!isSuccess)
        //{
        //    //< 모든 유닛을 숨김
        //    Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
        //    characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);
        //    foreach (Unit unit in allUnits)
        //        unit.gameObject.SetActive(false);
        //}

        yield return null;
    }

    /// <summary>
    /// InGameHUDPanel에서 초기화 할 것이 있어서 재정의함.
    /// </summary>
    public override void RevivePlayer()
    {
        base.RevivePlayer();

        HudPanel.Revive();
    }
    /*
    public override float PlayTimeUpdate()
    {
        float time = base.PlayTimeUpdate();
        if (HudPanel != null)
            HudPanel.TimeUpdate(time);

        return time;
    }
    */
}
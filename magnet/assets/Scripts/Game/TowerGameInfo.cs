using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerGameInfo : GameInfoBase {

    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.TOWER; } }

    public int BossDropGold;
    public int NormalDropGold;

    protected override void InitDatas()
    {
        //_STATE state = SceneManager.instance.CurrState();

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
        DungeonTable.TowerInfo tLowData = _LowDataMgr.instance.GetLowDataTower(StageId);
        //TimeLimit = tLowData.limitTime;

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
        uint[] Group1_UnitId = new uint[] {
            tLowData.MobId1,
            tLowData.MobId2,
            tLowData.MobId3,
        };

		uint[] Group2_UnitId = new uint[] {
			tLowData.MobId4,
			tLowData.MobId5,
			tLowData.MobId6,
		};

		uint[] Group3_UnitId = new uint[] {
			tLowData.MobId7,
			tLowData.MobId8,
			tLowData.MobId9,
			tLowData.MobId10,
			tLowData.MobId11//얘가 보스.
		};
		
		
		spawnGroup = spawnCtlr.GetList<SpawnGroup>();

        int totalMonster = 0;
        List<string> SpawnEffects = new List<string>();
        for (int i = 0; i < spawnGroup.Count; i++)
        {
            SpawnGroup group = spawnGroup[i];
            totalMonster += group.spawnerList.Count;
			uint[] unitId = Group1_UnitId;
			if (group.groupNo == 2){
				unitId = Group2_UnitId;
			}
			if (group.groupNo == 3){
				unitId = Group3_UnitId;
			}

            for (int spawnerNo = 0; spawnerNo < group.spawnerList.Count; spawnerNo++)
            {
				//if (unitId[spawnerNo] == 0)continue;

                // NPCSpawner만 골라서 처리해주도록 한다.
                NPCSpawner npcSpawner = group.spawnerList[spawnerNo] as NPCSpawner;
                if (null == npcSpawner)
                    continue;

                npcSpawner.unitId = (int)unitId[spawnerNo];
                npcSpawner.Owner = group;
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
            if (TowerGameState.IsMapLoad)
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

        DungeonTable.TowerInfo lowData = _LowDataMgr.instance.GetLowDataTower(StageId);
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

        if (TowerGameState.IsTest)
        {
            SceneManager.instance.ActionEvent(_ACTION.GO_TOWN);
            TowerGameState.IsTest = false;
            return;
        }

        HudPanel.GameEnd(isSuccess, true);

        PlayerController playerCtlr = G_GameInfo.PlayerController;
        StartCoroutine(ResultAction(isSuccess, playerCtlr ));
    }

    IEnumerator ResultAction(bool isSuccess, PlayerController playerCtlr)
    {
        yield return new WaitForSeconds(2);
        
        HudPanel.Hide();
        UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();
        
        uint totalKillCount = _AchieveFightData.KillBossCount + _AchieveFightData.KillMonsterCount, totalDieCount = _AchieveFightData.DieCount;
        NetworkClient.instance.SendPMsgTowerBattleCompleteC(StageId, totalDieCount, totalKillCount, PlayTime, isSuccess);

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
}

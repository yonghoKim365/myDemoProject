using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum InGameTutorialType
{
    None,
    //TouchQuest,
    Joystick,
    Attack,
    Skill_01,
    Skill_02,
    //Skill_03,
    //Skill_04,
    //FreeTalk,
    Auto,
    SuperArmor,
    EndGame,

    Max,
}

public class TutorialGameInfo : GameInfoBase
{
    /// <summary>
    /// 진행한 스테이지 ID
    /// </summary>
    //public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.TUTORIAL; } }
    //public ulong verifyToken { set; get; }

    public TutorialPopup TutorialUI;
    //public bool IsDummyAttack;
    //public int DummyAttackCount;
    //public int DummyUnitNum;

    public List<EventObj> ArrowList = new List<EventObj>();

    protected override void InitDatas()
    {
        base.InitDatas();

        //< 컷씬 정보 미리 로드
        CutSceneMgr.LoadCutScene();

        if (!TutorialGameState.IsTutorial)
            return;
        
    }

    protected override void InitManagers()
    {
        base.InitManagers();

        G_GameInfo.CharacterMgr.allrUUIDDic.Clear();
        //NetData.instance.MakePlayerSyncData(true);     //우선 난투전에서는 싱크데이터를 만들어 준다.

        _PlayerSyncData syncData = NetData.instance._playerSyncData;
        CreatePlayerController(syncData.playerSyncDatas[0], syncData.partnerSyncDatas);

        InitSpawnCtlr();
    }

    protected override void InitUI()
    {
        base.InitUI();
        
        //인게임 HUD패널 생성
        HudPanel = UIMgr.Open("UIPanel/InGameHUDPanel").GetComponent<InGameHUDPanel>();

        if( TutorialGameState.IsTutorial)
        {
            //ChatPopup chat = UIMgr.OpenChatPopup();
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
            if (chat != null)
                chat.Hide();

            TutorialUI = UIMgr.OpenTutorialPopup((int)TutorialType.INGAME);
            //OnTutorial((InGameTutorialType)1);

        }

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

        if (!TutorialGameState.IsTutorial)
        {
            spawnCtlr.gameObject.SetActive(false);
            return;
        }

        spawnCtlr.Init(G_GameInfo.PlayerController.Leader.gameObject);
        CameraManager.instance.mainCamera.enabled = true;
        //G_GameInfo.PlayerController.Leader.SuperRecoveryTick = float.MaxValue;
        //G_GameInfo.PlayerController.Leader.CharInfo.SuperArmor = 0;

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

        GameObject arrowGo = GameObject.Find("Arrow");
        if (arrowGo == null)
            return;

        Transform arrowTf = arrowGo.transform;
        for(int i=0; i < arrowTf.childCount; i++)
        {
            Transform tf = arrowTf.GetChild(i);
            if (tf == null)
                continue;

            EventObj eventObj = tf.GetComponent<EventObj>();
            if(eventObj == null)
                eventObj = tf.gameObject.AddComponent<EventObj>();
            
            eventObj.EventTarget = G_GameInfo.PlayerController.Leader.gameObject;
            eventObj.Callback = delegate () {

                ArrowList.RemoveAt(0);
            };

            eventObj.Hide();
            ArrowList.Add(eventObj);
        }

        ArrowList[0].Show();
    }

    protected override IEnumerator GameStartReady()
    {
        while (true)
        {
            if (TutorialGameState.IsMapLoad)
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

        //CameraAdjustManager를 따로 인스턴스화 해놓지 않았기때문에 그냥찾자 게임시작부분이라 퍼포먼스 신경안씀
        GameObject go = GameObject.Find("CameraAdjustManager");
        if (go != null )
            go.GetComponent<CameraAdjustManager>().CameraAdjust("", new Vector3(23f, 167f, 0f), 7);

        GameStart();

    }

    protected override void OnDieUnit(Unit deadUnit)
    {
        base.OnDieUnit(deadUnit);

        if (deadUnit is Boss)
        {
            Transform leaderTrn = G_GameInfo.PlayerController.Leader.transform;
            //for (int i = 0; i < 10; i++)
            //    DropItem.DropAssets(unit.transform, leaderTrn, i, (value * 0.1f), true);

            for (int i = 0; i < 6; i++)
                DropItem.DropItems(deadUnit.transform, leaderTrn, i, i == 0 ? DropItem.DropType.treasureChest : DropItem.DropType.WoodBox, true);
        }
    }

    public override void EndGame(bool isSuccess)
    {
        if (TutorialGameState.IsTutorial)
        {
            UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();
            UIMgr.instance.UICamera.enabled = true;
            OnTutorial(InGameTutorialType.EndGame);
        }
        else
        {
            TempCoroutine.instance.FrameDelay(3f, () =>
            {
                //HudPanel.Hide();
                //UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();
                UIMgr.instance.UICamera.enabled = true;
                TutorialGameState.IsTutorial = false;
                SceneManager.instance.TownAction();
            });
        }
    }

    //< 최종 종료전에 수행되어야할 작업(보스가 죽는순간 등)
    //< 모든 유닛의 움직임을 정지, 승리했을시 몬스터 모두 죽이기등
    public override void PrepareEndGame(bool win = false)
    {
        // 모든 유닛정보 얻기
        Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
        characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);

        //PlayerController playerCtlr = G_GameInfo.PlayerController;
        //if (null != playerCtlr && playerCtlr.CurLeaderUnit())
        //{
        //    //< 타겟팅 꺼줌
        //    //playerCtlr.CurLeaderUnit().IsLeader = false;
        //    playerCtlr.CurLeaderUnit().ChangeState(UnitState.Idle);
        //}

        AutoMode = false;

        //< 남은 몬스터들 죽임
        foreach (Unit unit in allUnits)
        {
            if (null == unit || (null != unit && !unit.Usable))
                continue;

            //< 이겼을때만 죽이고 그게아니라면 모두 정지만 시켜놓는다.
            if (win && unit.TeamID != 0)
                unit.UnitKill(G_GameInfo.PlayerController.Leader);
        }

        // 슬로우 효과 주기
        if (win)
        {
            Time.timeScale = 0.1f;
            float endSlowDuration = 0.25f;
            if (G_GameInfo.CharacterMgr != null && G_GameInfo.CharacterMgr.BossUnit != null)
                G_GameInfo.CharacterMgr.BossUnit.BossEndShaderChange();
            
            StartCoroutine(RestoreTimeScale(endSlowDuration));
            
        }
    }

    public void EndTutorial()
    {
        TempCoroutine.instance.FrameDelay(0.5f, () => { 
            base.EndGame(true);
            UIMgr.instance.UICamera.enabled = false;
            HudPanel.GameEnd(true);
            //UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
            //if (basePanel != null)
            //    basePanel.Close();

            SceneManager.instance.CurTutorial = TutorialType.INGAME;
            NetworkClient.instance.SendPMsgRoleGuideC((int)1);
            NetworkClient.instance.SendPMsgLoginOverC();
        
            TutorialGameState.IsTutorial = false;
        });
    }

    /// <summary>
    /// InGameHUDPanel에서 초기화 할 것이 있어서 재정의함.
    /// </summary>
    public override void RevivePlayer()
    {
        base.RevivePlayer();

        HudPanel.Revive();
    }

    public void OnTutorial(InGameTutorialType type)
    {
        if (TutorialUI == null)
            return;

        if (type == InGameTutorialType.Skill_02)
        {
            if (TutorialUI.Condition != 1 && TutorialUI.CurInGameTuto != InGameTutorialType.Skill_01)
                return;
        }
        
        TutorialUI.OnInGameTutorial(type);
    }

    public void EndZone()
    {
        if (0 < ArrowList.Count)
            ArrowList[0].Show();

        TutorialUI.EndZone();
    }
}

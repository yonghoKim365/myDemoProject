using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleGameInfo : GameInfoBase {
    /// <summary> 진행한 스테이지 ID </summary>
    public uint StageId { set; get; }
    public override GAME_MODE GameMode { get { return GAME_MODE.SINGLE; } }
    public ulong verifyToken { set; get; }
    
    public uint QuestTalkId;//NetworkClient에서 선처리를 하려고 만들어놓음.
    public byte UiState;//NetworkClient에서 먼저 처리되면 1, 여기가 먼저 처리되면 2
    public bool IsQuestClear;

    public int BossDropGold;
    public int NormalDropGold;

	public uint prevQuestID;
    //public static bool DisableHit = false;

    /// <summary> 프로토 타입 몬스터 모두 죽이기 플래그 </summary>
    //public bool ProtoKillAllFlag = false;

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
//	    Debug.LogWarning("2JW : InitUI() In - " + HudPanel);
    }

    void InitSpawnCtlr()
    {

		DungeonTable.StageInfo stage = _LowDataMgr.instance.GetStageInfo(StageId);
		GameObject spawnCtrlObj = null;
		if (stage.type == 1) { // normal
			spawnCtrlObj = GameObject.Find("SpawnController");
			if (GameObject.Find("SpawnController_Hard")){
				GameObject.Find("SpawnController_Hard").SetActive(false);
			}
		} 
		else if (stage.type == 2) {
			GameObject.Find("SpawnController").SetActive(false); 
			spawnCtrlObj = GameObject.Find("SpawnController_Hard");
		}

        // 난이도 선택 및 NPCSpawn 시작!
        //SpawnController[] spawnCtlrs = GameObject.FindObjectsOfType<SpawnController>();
        SpawnController[] spawnCtlrs = spawnCtrlObj.GetComponentsInChildren<SpawnController> ();

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

        int totalMonster = 0, propCount =0;
        List<string> SpawnEffects = new List<string>();
        for (int i = 0; i < spawnGroup.Count; i++)
        {
            SpawnGroup group = spawnGroup[i];
            totalMonster += group.spawnerList.Count;
            for (int spawnerNo = 0; spawnerNo < group.spawnerList.Count; spawnerNo++)
            {
                // NPCSpawner만 골라서 처리해주도록 한다.
                //NPCSpawner npcSpawner = group.spawnerList[spawnerNo] as NPCSpawner;
                if (group.spawnerList[spawnerNo] is Prop)
                    ++propCount;
                else if(group.spawnerList[spawnerNo] is NPCSpawner)
                    group.spawnerList[spawnerNo].Owner = group;

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
        NormalDropGold = Mathf.CeilToInt(dropGold / (totalMonster));
        BossDropGold = dropGold-(NormalDropGold*(totalMonster));
    }

    protected override IEnumerator GameStartReady()
    {
        while (true)
        {
#if UNITY_EDITOR
			if (GameObject.Find("SpawnTestManager")){
				SingleGameState.IsMapLoad = true;
			}
#endif
            if (SingleGameState.IsMapLoad)
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

        //SearchPathEffect();

        //싱글게임 진입 플래그 만족이 아닐경우
        Quest.QuestInfo info = QuestManager.instance.CheckSubQuest(QuestSubType.SINGLEGAMEPLAY, StageId);
        if (info == null) {

            IsQuestClear = false;
            GameStart();
		}
        else if (0 < info.QuestTalkSceneID)
        {
            IsQuestClear = true;

            UIMgr.OpenMissionPanel(info.ID);
            HudPanel.Hide();//꺼놓는다.
        }

        DungeonTable.StageInfo stage = _LowDataMgr.instance.GetStageInfo(StageId);
		if (stage != null) {
			TimeLimit = stage.LimitTime;
		} else {
			Debug.LogError(" stage info is null!");
		}

    }

    public void OverrideGameStart(uint questID)
    {

		//QuestInfo quest = QuestManager.instance.GetLastQuestInfo();
		prevQuestID = questID;
		if (GameObject.Find ("CinemaSceneManager") != null){
			CinemaSceneManager csm = GameObject.Find ("CinemaSceneManager").GetComponent<CinemaSceneManager> ();

			if (csm.getCinemaSceneStartType(questID) == CinemaSceneManager.StartType.BEFORE_GAME_START){
				int startSeqIdx = csm.getCinemaSceneStartingSeqIndex(questID);
				int endSeqIdx = csm.getCinemaSceneEndingSeqIndex(questID);
				CutSceneMgr.StartCinemaScene(true, startSeqIdx, endSeqIdx, ()=>{
					HudPanel.Show();
					GameStart();
				});
				return;
			}
		}

        HudPanel.Show();
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

        //int totalGold = NetData.instance._RewardData.GetCoin;
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
        else if (deadUnit is Prop)
        {
            addGold = (int)(deadUnit as Prop).PropInfo.RewardGold;
        }

        else
        {
            return;
        }

        //GetGold += addGold;
        base.AddGold(deadUnit, addGold);
    }

    public void OverrideGameEnd(bool isSuccess)
    {
        if (SingleGameState.IsTest)
        {
            SceneManager.instance.ActionEvent(_ACTION.GO_TOWN);
            SingleGameState.IsTest = false;
        }

        if (isSuccess)
        {
            if (UiState == 1)//넷트워크에서 처리가 잘됨. 좋은거
            {
#if UNITY_EDITOR
				if (SceneManager.instance.testData.bCutSceneTest && GameObject.Find ("CinemaSceneManager") != null){
					CinemaSceneManager csm = GameObject.Find ("CinemaSceneManager").GetComponent<CinemaSceneManager> ();
					
					SingleGameInfo stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;
					bool bPlayCutScene = false;
					if (csm.getCinemaSceneStartTypeByStageID(stageinfo.StageId) == CinemaSceneManager.StartType.AFTER_CLEAR){
						bPlayCutScene = true;
					}
					if (stageinfo.StageId == 310){ // 3-10 has two conditions, BEFORE_GAME_START and AFTER_CLEAR,
						if (csm.getCinemaSceneStartTypeByStageID(stageinfo.StageId) == CinemaSceneManager.StartType.BEFORE_GAME_START){
							bPlayCutScene = true;
						}
					}
					
					if (bPlayCutScene){
						int idx = 0;
						if (stageinfo.StageId == 310){
							idx = 2;
						}
						CutSceneMgr.StartCinemaScene(false, idx, idx, ()=>{
							OpenResultPanel(true);
						});
						return;
					}
				}
#endif
                OpenResultPanel(true);
            }
            else//클라가 먼저 실행됨 네트워크에서 처리.
                UiState = 2;
        }
        else
        {
            UiState = 2;
            NetworkClient.instance.SendPMsgStageCompleteC((int)StageId, 0, new int[] { 0, 0, 0 });
        }
    }

    public override void EndGame(bool isSuccess)
    {
        if (!IsStartGame)//이미 처리중 중복처리 막는다
            return;

        base.EndGame(isSuccess);

        HudPanel.GameEnd(isSuccess);

        TempCoroutine.instance.FrameDelay(1f, () =>
        {
            UIMgr.instance.UICamera.enabled = true;

            HudPanel.Hide();
            UIMgr.GetUI("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>().Hide();
            
            OverrideGameEnd(isSuccess);
        });
    }
    
    public override void PrepareEndGame(bool win = false)
    {
        base.PrepareEndGame(win);

        //튜토리얼 챕터 클리어 강제로나갈 수도 있으니 저장시켜준다.
        //if (G_GameInfo.GameMode == GAME_MODE.SINGLE && StageId == 101 && UIMgr.instance.CurTutorial == TutorialType.CHAPTER)
        //    PlayerPrefs.SetInt(string.Format("Tutorial{0}", NetData.instance.GetUserInfo()._charUUID), (int)TutorialType.CHAPTER_REWARD);
        
        if (win && 0 < SingleGameState.StageQuestList.Count)
        {
            //3이 별갯수
            int[] TempStar = new int[] {
                SingleGameState.StageQuestList[0].IsClear ? 1 : 0,
                SingleGameState.StageQuestList[1].IsClear ? 1 : 0,
                SingleGameState.StageQuestList[2].IsClear ? 1 : 0
            };

            int stageNum = (int)(StageId % 100);
            if (10 <= stageNum)//챕터 클리어.
            {
                NetData.ClearSingleStageData clearData = null;
                if (NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(StageId, out clearData))
                {
                    if (clearData.Clear_0 + clearData.Clear_1 + clearData.Clear_2 <= 0)
                    {
                        SingleGameState.IsChapterClear = true;
                    }
                }
                else//없는것이 맞다.
                    SingleGameState.IsChapterClear = true;
            }

            NetworkClient.instance.SendPMsgStageCompleteC((int)StageId, win ? 1 : 0, TempStar);
        }

        Debug.Log("SingleGameState.IsChapterClear = " + SingleGameState.IsChapterClear);
    }


    /// <summary> InGameHUDPanel에서 초기화 할 것이 있어서 재정의함. </summary>
    public override void RevivePlayer()
    {
        base.RevivePlayer();

        HudPanel.Revive();
    }

    //GUIStyle txtColStyle;
    //void OnEnable()
    //{
    //    txtColStyle = new GUIStyle();
    //    txtColStyle.normal.textColor = Color.white;
    //}

    //void OnGUI()
    //{
    //    if (Debug.isDebugBuild)
    //    {
    //        GUILayout.BeginVertical("box");

    //        GUILayout.Label("<b>SuperArmorValue</b>", txtColStyle);

    //        //var enumerator = G_GameInfo.CharacterMgr.allrUUIDDic.GetEnumerator();

    //        //while (enumerator.MoveNext())
    //        //{
    //        //    Unit unit = enumerator.Current.Value;

    //        //    if (unit != null)
    //        //    {
    //        //        if (unit.m_rUUID == NetData.instance._userInfo._charUUID)
    //        //        {
    //        //            GUILayout.Label("<b>" + enumerator.Current.Key + ": " + (unit as Pc).CharInfo.UnitName + " HP:" + unit.CharInfo.Hp + "/" + unit.CharInfo.MaxHp + "</b>", txtColStyle);
    //        //        }
    //        //        else
    //        //        {
    //        //            GUILayout.Label(enumerator.Current.Key + ": " + (unit as Pc).CharInfo.UnitName + " HP:" + unit.CharInfo.Hp + "/" + unit.CharInfo.MaxHp, txtColStyle);
    //        //        }
    //        //    }
    //        //}

    //        GUILayout.EndVertical();
    //    }
    //}
}
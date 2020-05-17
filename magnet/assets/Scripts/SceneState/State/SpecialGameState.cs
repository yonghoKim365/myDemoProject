using UnityEngine;
using System.Collections.Generic;

public class SpecialGameState : SceneStateBase {
    public static uint lastSelectStageId;
    public static bool IsGoldStage;
    public static string StageName = null;
    //public static SpecialGameType SpecialType;
    public static List<NetData.MonsterData> MonList;

    public string StageMapName;
    public uint stageId = 0;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        lastSelectStageId = 1;
        if (0 < lastSelectStageId)
        {
            if (IsGoldStage)
            {
                DungeonTable.EquipInfo lowData = _LowDataMgr.instance.GetLowDataEquipBattle((byte)lastSelectStageId);
                StageName = lowData.StageName;
            }
            else
            {
                DungeonTable.SkillInfo lowData = _LowDataMgr.instance.GetLowDataSkillBattle((byte)lastSelectStageId);
                StageName = lowData.StageName;
            }
        }
        else
        {
            if (IsGoldStage)
                StageName = "Gold_Dungeon_001";
            else
                StageName = "Exp_Dungeon_001";
        }
        
        LoadLevelAsync(StageName);

        GameReadyState.NextAction = _ACTION.SPECIAL_STAGE;

        // xray 일단 주석처리 
        //  CameraManager.instance.XRayComponent.DefualtCameraMode();

    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        Time.timeScale = 1f;

        //임시로 해당 스테이 빠져나갈때 초기화 시켜주기 위함
        //NetData.instance.PlayerSyncData.Init(0, "Player");

        // xray 일단 주석처리 
        //  CameraManager.instance.XRayComponent.XRaCameraMode();
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName != StageName) return;
        IsMapLoad = true;

        CameraManager.instance.RtsCamera.Reset();
        SceneSetting();
        MapEnvironmentSetting(Application.loadedLevelName);
        InitGame();

        //SoundManager.instance.PlayBgmSound(_LowDataMgr.instance.GetBGMFile(Application.loadedLevelName));
    }

    void InitGame()
    {
        //SceneManager.GameMode = GAME_MODE.INFINITE;

        SetupMainCamera(true);
        
        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>("GameInfo/SpecialGameInfo");
        (infoBase as SpecialGameInfo).StageId = lastSelectStageId;
        //(infoBase as GoldGameInfo).playStageInfo = _LowDataMgr.instance.GetStageInfo(lastSelectStageId);
    }

    /// <summary> 다르게 사용함 카운트 다운 후 사운드 재생 </summary>
    public override void SceneSetting()
    {
        //UIMgr.ClearUI();
        CameraManager.instance.SceneInit();
    }
}

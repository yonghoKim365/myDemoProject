using UnityEngine;
using System.Collections.Generic;

public enum RAID_TYPE
{
    None,
    //Fire,
    //Water,
    //Metal,
    Raid_1,
    Raid_2,
    Raid_3,
}


public class RaidGameState : SceneStateBase
{
    public static bool IsTest = false;
    /// <summary>
    /// 마지막으로 플레이한 스테이지 번호
    /// </summary>
    public static uint lastSelectStageId;
    public static ulong verifyToken;

    public static string StageName = null;
    //public static RAID_TYPE RaidType;
    public string StageMapName;
    public uint stageId = 0;

    public override void OnEnter(System.Action callback)
    {
        //StageLowData.DataInfo curStageLowData = LowDataMgr.GetStage(stageId);

        //SceneManager.eLoadingTipType type = SceneManager.eLoadingTipType.NONE;
        //if (curStageLowData.property == 1 || curStageLowData.property == 9) type = SceneManager.eLoadingTipType.SINGLE_FOREST;
        //else if (curStageLowData.property == 2 || curStageLowData.property == 8) type = SceneManager.eLoadingTipType.SINGLE_POISON;
        //else if (curStageLowData.property == 3 || curStageLowData.property == 6) type = SceneManager.eLoadingTipType.SINGLE_ICE;
        //else if (curStageLowData.property == 4 || curStageLowData.property == 7) type = SceneManager.eLoadingTipType.SINGLE_REMAINS;
        //else if (curStageLowData.property == 5 || curStageLowData.property == 10) type = SceneManager.eLoadingTipType.SINGLE_VOLCANO;

        //SceneManager.instance.ShowLoadingTipPanel(true, () => 
        //{
        //    base.OnEnter(callback);
        //    LoadLevelAsync(StageName);

        //    GameReadyState.NextAction = _ACTION.PLAY_SINGLE;
        //}, type);
        /*
        StageName = "";
        switch (lastSelectStageId)
        {
            case 0:
                StageName = "single_test";
                break;

            default:
                //StageName = string.Format("single_f{0}", lastSelectStageId.ToString("00#"));
                //StageName = (_LowDataMgr.instance.GetStageInfo(lastSelectStageId)).StageName;
                StageName = "Raid_Dungeon_001";

                break;
        }
        */
        if (lastSelectStageId == 0)
            StageName = "single_test";
        else
        {
            DungeonTable.SingleBossRaidInfo lowData = null;
            _LowDataMgr.instance.RefLowDataBossRaid(lastSelectStageId, ref lowData);
            StageName = lowData.stageName;
        }


        base.OnEnter(callback);
        LoadLevelAsync(StageName);

        GameReadyState.NextAction = _ACTION.PLAY_RAID;

        // xray 일단 주석처리 
        // CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        Time.timeScale = 1f;

        //임시로 해당 스테이 빠져나갈때 초기화 시켜주기 위함
        //NetData.instance.PlayerSyncData.Init(0, "Player");

        // xray 일단 주석처리 
        // CameraManager.instance.XRayComponent.XRaCameraMode();
    }

    void OnLevelWasLoaded(int level)
    {
        //if (Application.loadedLevelName == StageName )
        //{
        //    IsMapLoad = false;
        //    InGameMapMgr.instance.LoadMap(StageMapName, LowDataMgr.GetStage(stageId).lightmapType, "", (obj) => 
        //    {
        //        IsMapLoad = true;
        //    });

        //    CameraManager.instance.RtsCamera.Reset();
        //    SceneSetting();
        //    InitGame();
        //    SoundHelper.PlayBgmSound(LowDataMgr.GetStage(stageId).bGM);
        //}
        if (Application.loadedLevelName != StageName) return;
        if (SceneManager.instance.CurrState() != _STATE.RAID_GAME) return;

        IsMapLoad = true;

        CameraManager.instance.RtsCamera.Reset();
        SceneSetting();
        MapEnvironmentSetting(Application.loadedLevelName);
        InitGame();

        //SoundManager.instance.PlayBgmSound(_LowDataMgr.instance.GetBGMFile(Application.loadedLevelName));
    }
    /*
    public void SetStageNum(uint stageid)
    {
        StageName = LowDataMgr.GetStage(stageid).stageName;
        StageMapName = LowDataMgr.GetStage(stageid).mapName;
        stageId = stageid;
    }
    */
    void InitGame()
    {
        //SceneManager.GameMode = GAME_MODE.SINGLE;

        SetupMainCamera(true);

        // 게임시작시 필요한 플레이어 정보들을 준비하도록 한다.
        //if (!TutorialMgr.TutorialActive)
        //    NetData.instance.SetSyncData( 0, DataManager.GetEquipedUnits());

        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>("GameInfo/RaidGameInfo");
        (infoBase as RaidGameInfo).StageId = lastSelectStageId;
        //(infoBase as SingleGameInfo).playStageInfo = _LowDataMgr.instance.GetStageInfo(lastSelectStageId);
        (infoBase as RaidGameInfo).verifyToken = verifyToken;
    }
}

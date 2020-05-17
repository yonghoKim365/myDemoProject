using UnityEngine;
using System.Collections;

public class TowerGameState : SceneStateBase {
    public static uint lastSelectStageId;
    public static bool IsTest = false;
    public static string StageName = null;
    public string StageMapName;

    public override void OnEnter(System.Action callback)
    {

        if (0 < lastSelectStageId)
        {
            //DungeonTable.GoldBattleInfo gLowData = _LowDataMgr.instance.GetLowDataGoldBattle((byte)lastSelectStageId);
            DungeonTable.TowerInfo tLowData = _LowDataMgr.instance.GetLowDataTower(lastSelectStageId);
            StageName = tLowData.mapName;
        }
        else
            StageName = "Devildom_001";

        base.OnEnter(callback);
        LoadLevelAsync(StageName);

        GameReadyState.NextAction = _ACTION.PLAY_TOWER;
        /*
        Bloom bloomdata = CameraManager.instance.mainCamera.GetComponentInChildren<Bloom>();
        bloomdata.enabled = true;
        bloomdata.bloomIntensity = 0.2f;
        bloomdata.bloomThreshhold = 0.25f;
        bloomdata.bloomBlurIterations = 2;
        */
        // xray 일단 주석처리 
        // CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        //CameraManager.instance.mainCamera.GetComponentInChildren<Bloom>().enabled = false;
        base.OnExit(callback);

        Time.timeScale = 1f;

        //임시로 해당 스테이 빠져나갈때 초기화 시켜주기 위함
        //NetData.instance.PlayerSyncData.Init(0, "Player");

        // xray 일단 주석처리 
        // CameraManager.instance.XRayComponent.XRaCameraMode();
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName != StageName) return;
        IsMapLoad = true;
        CameraManager.instance.RtsCamera.Reset();
        SceneSetting();
        MapEnvironmentSetting(Application.loadedLevelName);
        InitGame();
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
        //SceneManager.GameMode = GAME_MODE.INFINITE;

        SetupMainCamera(true, GAME_MODE.TOWER);

        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>("GameInfo/TowerGameInfo");
        (infoBase as TowerGameInfo).StageId = lastSelectStageId;
        //(infoBase as TowerGameInfo).playStageInfo = _LowDataMgr.instance.GetStageInfo(lastSelectStageId);

        CameraManager.instance.mainCamera.backgroundColor = new Color(0, 0, 0, 1);
    }
}

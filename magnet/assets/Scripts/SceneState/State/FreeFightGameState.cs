using UnityEngine;
using System.Collections;

public class FreeFightGameState : SceneStateBase
{

    public static uint lastSelectStageId;     /// 마지막으로 플레이한 스테이지 번호

    public static string StageName = null;
    public static string MaskFileName = null;
    public static GAME_MODE GameMode;

    public static ulong selectedRoomNo;
    public static bool StateActive = false;

    public override void OnEnter(System.Action callback)
    {
        /*
        StageName = "";
        switch (lastSelectStageId)
        {
            //case 10000:    //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
            //    StageName = "PVP_Stage_001";
            //    lastSelectStageId = 100; //<----- 지금은 임시로 싱글 첫번째 실행 시켰을 때 테이블에서 얻어온 100을 대입했다. 
            //    break;

            default:
                {
                    
                    StageName = "battle_field";
                }
                break;
        }
        */

        Map.MapDataInfo mapData = _LowDataMgr.instance.GetMapData((uint)NetworkClient.instance.GetMapID());
        StageName = mapData.scene;
        MaskFileName = mapData.maskfile;
        
        base.OnEnter(callback);
        LoadLevelAsync(StageName);

        switch(mapData.type)
        {
            case 3:
                GameMode = GAME_MODE.FREEFIGHT;
                break;
            case 5:
                GameMode = GAME_MODE.MULTI_RAID;
                break;
            case 6:
                GameMode = GAME_MODE.COLOSSEUM;
                break;
        }

        GameReadyState.NextAction = _ACTION.PLAY_FREEFIGHT;
        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
        StateActive = false;
        GameMode = GAME_MODE.NONE;
        Time.timeScale = 1f;
        //임시로 해당 스테이 빠져나갈때 초기화 시켜주기 위함
        //NetData.instance.PlayerSyncData.Init(0, "Player");

        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.XRaCameraMode();
    }

    void OnLevelWasLoaded(int level)
    {
        if (Application.loadedLevelName != StageName) return;
        if (SceneManager.instance.CurrState() != _STATE.FREEFIGHT_GAME) return;

        IsMapLoad = true;

        //타일정보 읽어옴
        NaviTileInfo.instance.LoadTile(MaskFileName);

        CameraManager.instance.RtsCamera.Reset();
        SceneSetting();
        MapEnvironmentSetting(Application.loadedLevelName);
        InitGame();


        StateActive = true;
        NetworkClient.instance.SendPMsgBattleMapEnterMapReadyC();
    }

    //public static void StateStart()
    //{
    //    StateActive = true;
    //    NetworkClient.instance.SendPMsgBattleMapEnterMapReadyC();
    //}

    void InitGame()
    {
        //SceneManager.GameMode = GAME_MODE.FREEFIGHT;   //<-- 지금은 사용 않하는 듯 함
        SetupMainCamera(true, GAME_MODE.FREEFIGHT);

        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>("GameInfo/FreeFightGameInfo");
        //G_GameInfo.GameMode = GAME_MODE.FREEFIGHT;
        (infoBase as FreeFightGameInfo).StageId = lastSelectStageId;
        //(infoBase as FreeFightGameInfo).playStageInfo = _LowDataMgr.instance.GetLowDataDogFight(lastSelectStageId);
        (infoBase as FreeFightGameInfo).RoomNo = selectedRoomNo;
        
        if (Application.loadedLevelName.Contains("Colosseum"))
        {
            CameraManager.instance.RtsCamera.Rotation = -150f;
        }        
    }

    //public hfh_server_info.RoomInfo GetThisRoomInfo()
    //{
    //    if (SceneManager.isRTNetworkMode != GAME_MODE.FREEFIGHT)
    //        return null;

    //    if (SceneManager.g_FrFtInfo == null)
    //        return null;

    //    if (SceneManager.g_FrFtInfo.roomList.Count <= 0)
    //        return null;

    //    hfh_server_info.RoomInfo a_RoomInfo = null;
    //    for (int i = 0; i < SceneManager.g_FrFtInfo.roomList.Count; i++)
    //    {
    //        if (FreeFightGameState.selectedRoomNo == SceneManager.g_FrFtInfo.roomList[i].roomNo)
    //        {
    //            a_RoomInfo = SceneManager.g_FrFtInfo.roomList[i];
    //            break;
    //        }
    //    }

    //    return a_RoomInfo;
    //}
}
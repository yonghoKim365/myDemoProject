using UnityEngine;
using System.Collections;

public class ArenaGameState : SceneStateBase
{
    public static uint lastSelectStageId;     /// 마지막으로 플레이한 스테이지 번호

    public static string StageName = null;

    public static ulong selectedRoomNo;
    public static int MyRank;
    public static int TargetRank;
    public static int MyTopRank;    //나의 최고등수
    public static int TargetAttack; //적의 전투력


    public override void OnEnter(System.Action callback)
    {
        StageName = "PPVP_Stage_001";
        base.OnEnter(callback);


        float delayTime = SceneManager.instance.LoadingTipPanel.prevLoadingTime;

        TempCoroutine.instance.FrameDelay(delayTime , () =>
        {
            LoadLevelAsync(StageName);
            GameReadyState.NextAction = _ACTION.PLAY_ARENA;
        });
      
        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        Time.timeScale = 1f;

        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.XRaCameraMode();
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
        //SceneManager.GameMode = GAME_MODE.ARENA;
        //G_GameInfo.GameMode = GAME_MODE.ARENA;

        SetupMainCamera(true, GAME_MODE.ARENA);

        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>("GameInfo/ArenaGameInfo");
        (infoBase as ArenaGameInfo).StageId = lastSelectStageId;
        //(infoBase as ArenaGameInfo).playStageInfo = _LowDataMgr.instance.GetLowDataDogFight(lastSelectStageId);
        (infoBase as ArenaGameInfo).RoomNo = selectedRoomNo;
    }
}

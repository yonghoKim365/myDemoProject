using UnityEngine;
using System.Collections.Generic;

public class SingleGameState : SceneStateBase
{
    /// <summary> 마지막으로 플레이한 스테이지 번호 </summary>
    public static uint lastSelectStageId=0;
    public static ulong verifyToken;
    public static string CurStageName;

    public static bool IsFirstReward = false;   //하드모드최초보상관련
    public static bool IsChapterClear = false;
    public static bool IsTest = false;
    //public static int CurrentChapter = 0;//스테이지 어디 챕터인지 저장용

    public static List<NetData.StageClearData> StageQuestList;

    string StageName = null;

    public override void OnEnter(System.Action callback)
    {
	    StageName = "";
        switch (lastSelectStageId)
	    {
		  case 0:
			 StageName = "single_test";
			 break;

		  default:
             StageName = (_LowDataMgr.instance.GetStageInfo(lastSelectStageId)).StageName;
			 break;
	    }
        base.OnEnter(callback);
        LoadLevelAsync(StageName);

        GameReadyState.NextAction = _ACTION.PLAY_SINGLE;
        /*
        //블럼 셋팅 강제로 넣어줌
        if (lastSelectStageId == 105 || lastSelectStageId == 107)
        {
            Bloom bloomdata = CameraManager.instance.mainCamera.GetComponentInChildren<Bloom>();
            bloomdata.enabled = true;
            bloomdata.bloomIntensity = 0.3f;
            bloomdata.bloomThreshhold = 0.25f;
            bloomdata.bloomBlurIterations = 3;
        }
        */
        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        Time.timeScale = 1f;

        //블럼 셋팅 강제로
        /*
        if (lastSelectStageId == 105 || lastSelectStageId == 107)
            CameraManager.instance.mainCamera.GetComponentInChildren<Bloom>().enabled = false;
        */
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
        IsMapLoad = true;
	   
        CameraManager.instance.RtsCamera.Reset();
        SceneSetting();
        MapEnvironmentSetting(Application.loadedLevelName);
        InitGame();

        //SoundManager.instance.PlayBgmSound(_LowDataMgr.instance.GetBGMFile(Application.loadedLevelName));
    }

    void InitGame()
    {
        //bool tempRotate = false;
        //if (lastSelectStageId == 302 || lastSelectStageId == 303 || lastSelectStageId == 306)
        //    tempRotate = true;

        SetupMainCamera( true , GAME_MODE.SINGLE, false);

        // 게임시작시 필요한 플레이어 정보들을 준비하도록 한다.
        //if (!TutorialMgr.TutorialActive)
        //    NetData.instance.SetSyncData( 0, DataManager.GetEquipedUnits());

        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>( "GameInfo/SingleGameInfo" );
        (infoBase as SingleGameInfo).StageId = lastSelectStageId;
        //(infoBase as SingleGameInfo).playStageInfo = _LowDataMgr.instance.GetStageInfo(lastSelectStageId);
        (infoBase as SingleGameInfo).verifyToken = verifyToken;

        IsFirstReward = false;


        if (SceneManager.instance.testData.bSingleSceneTestStart) {
			Time.timeScale = 5f;
		}
		if (SceneManager.instance.testData.bQuestTestStart) {
			Time.timeScale = 5f;
		}
    }
    
}

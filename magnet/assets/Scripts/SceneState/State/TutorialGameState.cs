using UnityEngine;
using System.Collections;

public class TutorialGameState : SceneStateBase {

    public static bool IsTutorial;//튜토리얼 중인지
    public static uint lastSelectStageId;
    public static ulong verifyToken;

    public static string StageName = null;

    public override void OnEnter(System.Action callback)
    {
	    //StageName = "prologue_shrine";
        /*
        switch (lastSelectStageId)
	    {
		    case 0:
			    StageName = "single_test";
			    break;

		    default:
                //StageName = string.Format("single_f{0}", lastSelectStageId.ToString("00#"));
                StageName = (_LowDataMgr.instance.GetStageInfo(lastSelectStageId)).StageName;
			    break;
	    }
        */
        base.OnEnter(callback);
        LoadLevelAsync(StageName);

        GameReadyState.NextAction = _ACTION.PLAY_TUTORIAL;

        // xray 일단 주석처리 
        //CameraManager.instance.XRayComponent.DefualtCameraMode();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);

        Time.timeScale = 1f;

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
        InitGame(Application.loadedLevelName);

        //SoundManager.instance.PlayBgmSound(_LowDataMgr.instance.GetBGMFile(Application.loadedLevelName));
    }

    void InitGame(string loadSceneName)
    {
        GAME_MODE mode = GAME_MODE.SINGLE;
        if (StageName.Contains("tower"))
            mode = GAME_MODE.TOWER;
        else if (StageName.Contains("PPVP"))
            mode = GAME_MODE.ARENA;
        else if(StageName.Contains("battle_field"))
            mode = GAME_MODE.FREEFIGHT;

        SetupMainCamera(true, mode, false);
        //SetupMainCamera( true );
        
        // GameInfoBase 생성
        GameInfoBase infoBase = ResourceMgr.InstAndGetComponent<GameInfoBase>( "GameInfo/TutorialGameInfo" );

        if (IsTutorial)
            infoBase.AutoMode = false;

        if (loadSceneName.Equals("1zone"))
        {
            CameraManager.instance.RtsCamera.MaxDistance = 50f;
            CameraManager.instance.RtsCamera.Tilt = 23f;
            CameraManager.instance.RtsCamera.Rotation = 167f;
            CameraManager.instance.RtsCamera.Distance = 13;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class StartState : SceneStateBase
{
    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);

        StartInit();

        //if (Application.loadedLevelName != "LoginScene")
        //{
        //    Destroy(AtlasMgr.instance.gameObject);
        //    Destroy(TaskManager.instance.gameObject);
        //    Destroy(GK_PoolManager.instance.gameObject);
        //    Destroy(TempCoroutine.instance.gameObject);
        //    Destroy(NativeManager.instance.gameObject);
        //    Destroy(CameraManager.instance.gameObject);
        //    Destroy(UIMgr.instance.gameObject);
        //    Destroy(QualityManager.instance.gameObject);
        //    Destroy(InputManager.instance.gameObject);
        //    Destroy(_LowDataMgr.instance.gameObject);
        //    Destroy(SoundManager.instance.gameObject);
        //    Destroy(GameObject.Find("AudioController"));
        //    Destroy(GameObject.Find("UIRoot(Clone)"));
        //    Destroy(GameObject.Find("Network"));

        //    LoadLevelAsync("LoginScene");
        //    //Destroy(SceneManager.instance.gameObject);
        //}
        //else
        //    StartInit();
    }

    public override void OnExit(System.Action callback)
    {
        base.OnExit(callback);
    }

    /// <summary>
    /// 게임에 필요한 데이터 로드 및 다운로드 시작!
    /// </summary>
    void StartInit()
    {
        //< 제이슨, 어셋파일 다운로드
        //parent.StartToCheckAssetData();
        //NextSceneCheck = false;

        GameDefine.ScreenOrgSize.x = Screen.width;
        GameDefine.ScreenOrgSize.y = Screen.height;

        Resolution[] Res = Screen.resolutions;

        for(int i=0;i<Res.Length;i++)
        {
            Debug.Log(Res[i].width + "x" + Res[i].height);
        }

        //Screen.SetResolution(1280, 720, true);

		int scrWidth = Screen.width;
#if UNITY_ANDROID
		if (scrWidth >= 1920){
			scrWidth = 1920;
		}
#endif

		Screen.SetResolution(scrWidth, scrWidth * 9 / 16, true);

        Debug.Log("CurrentResolution " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);

        ////인트로
        //TempCoroutine.instance.FrameDelay(0.3f, () =>
        //{
        //    UIMgr.Open("UIPanel/LogoPanel").GetComponent<LogoPanel>().EndCallBack = () =>
        //    {
        //        //parent.ShowLoadingPanel(true);
        //        //StartInit();

        //        //SoundHelper.PlayBgmSound(9010, true, "BGM_Main_Tittle");

        //        //로고가 끝나면 넘어간다.
        //        NextSceneCheck = false;
        //        AssetDownLoad.instance.isLoad = true;
        //    };
        //});

        UIMgr.Open("UIPanel/LogoPanel");

        //< 플랫폼 별로 처리
#if UNITY_EDITOR
        GameDefine.PlatformType = ePlatformType.PC;
#elif UNITY_ANDROID
            //< 현재 연동된 계정이 무엇인지에 따라 처리
            GameDefine.PlatformType = ePlatformType.GOOGLEPLAY;
#elif UNITY_IPHONE
            GameDefine.PlatformType = ePlatformType.GAMECENTER;
#endif

        //if (GameDefine.PlatformType != ePlatformType.PC)
        //    MinNativeMgr.instance.Setup();

        //< 임시로 넣어둠
        //GameDefine.PlatformType = ePlatformType.PC;
        //parent.ActionEvent(_ACTION.GO_NEXT);
    }

    //bool NextSceneCheck = true;
    //void Update()
    //{
    //    //if (AssetDownLoad.instance.isLoad && !NextSceneCheck)
    //    if(!NextSceneCheck)
    //    {
    //        NextSceneCheck = true;
    //        Invoke("NextScene", 1);
    //    }
    //}

    //void NextScene()
    //{
    //    //parent.LoadingPanel().ShowLoadingInfo(false);
    //    parent.ActionEvent(_ACTION.GO_NEXT);
    //}

    void OnLevelWasLoaded(int level)
    {
        /*
        if (Application.loadedLevelName == "LoginScene")
        {
            QuestManager.instance.CleanUp();
            CameraManager.instance.EnableRPGCamera(false);

            CameraManager.instance.transform.position = Vector3.zero;
            CameraManager.instance.transform.eulerAngles = Vector3.zero;
            //CameraManager.instance.RtsCamera.gameObject.transform.position = new Vector3(4.02f, 1.98f, 20.85f);
            //CameraManager.instance.RtsCamera.gameObject.transform.eulerAngles = new Vector3(6.827481f, -116f, 2f);
            //CameraManager.instance.mainCamera.transform.position = Vector3.zero;
            //CameraManager.instance.mainCamera.transform.eulerAngles = Vector3.zero;
            //CameraManager.instance.RtsCamera.gameObject.transform.position = Vector3.zero;
            //CameraManager.instance.RtsCamera.gameObject.transform.eulerAngles = Vector3.zero;

            CameraManager.instance.mainCamera.transform.position = new Vector3(4.02f, 1.98f, 20.85f);
            CameraManager.instance.mainCamera.transform.eulerAngles = new Vector3(6.827481f, -116f, 2f);
            CameraManager.instance.mainCamera.fieldOfView = 47;

            StartInit();
        }
        */
    }
}

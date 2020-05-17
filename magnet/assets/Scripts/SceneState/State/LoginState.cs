using UnityEngine;

public class LoginState : SceneStateBase
{
    public static bool FirstLogin = true;

    public override void OnEnter(System.Action callback)
    {
        base.OnEnter(callback);
        
		SceneManager.instance.showStopWatchTimer ("LoginState, start LoadLevelAsync");
        LoadLevelAsync("LoginScene", null, true);
    }

    public override void OnExit(System.Action callback)
    {
        //CameraManager.instance.mainCamera.transform.position = new Vector3(0f, 8.5f, 15f);
        //CameraManager.instance.mainCamera.transform.eulerAngles = new Vector3(37f, 180f, 0f);
        //CameraManager.instance.mainCamera.fieldOfView = 40;
        //CameraManager.instance.mainCamera.GetComponentInChildren<Bloom>().enabled = false;

        base.OnExit(callback);

        //FirstLogin = false;
        //MinNativeMgr.instance.SetLocalPush(0, 1, NetData.instance.Nickname + "님 접속을 환영합니다", string.Format("게임 서버 버전 [{0}] : 클라이언트버전 [{1}] : ID[{2}]", GameDefine.BuildVersion, GameDefine.NowBuildVersion, NetData.instance.PlatformID));

        //< 처음 데이터 처리
        //NetData.instance.FirstLoadData();
    }

    public void SelectSceneProcess()
    {
        UIMgr.Close("UIPanel/LoginPanel");

        iTween.RotateTo(CameraManager.instance.mainCamera.gameObject, iTween.Hash("time", 2f
                                                                                    , "easetype", iTween.EaseType.linear
		                                                                          	, "rotation", new Vector3(-0.6000061f, -155.66f, 2.49f)
                                                                                    //, "rotation", new Vector3(6.73f, -155.66f, 2.49f)
                                                                                    //, "rotation", new Vector3(11.77262f, -159.8964f, 2.135164f)
                                                                                    , "oncomplete", "RotateSelectHeroEnd"
                                                                                    , "oncompletetarget", this.gameObject));
        iTween.MoveTo(CameraManager.instance.mainCamera.gameObject, iTween.Hash("time", 2f
                                                                                    , "easetype", iTween.EaseType.linear
		                                                                        	, "position", new Vector3(5.47f, 1.35f, 21.05f)
		                                                                        	//, "position", new Vector3(4.8f, 1.18f, 20.38f)
                                                                                    //, "position", new Vector3(4.58f, 2.35f, 20.38f)
                                                                                    //, "position", new Vector3(5.16f, 3.51f, 23.54f)
                                                                                    ));
    }

    private Vector3 loginSceneCameraPos = new Vector3(4.02f, 1.98f, 20.85f);
    private Vector3 loginSceneCameraAngles = new Vector3(6.827481f, -116f, 2f);

    public void LoginSceneProcess()
    {
        //UIMgr.Close("UIPanel/SelectHeroPanel");

        iTween.RotateTo(CameraManager.instance.mainCamera.gameObject, iTween.Hash("time", 2f
                                                                                    , "easetype", iTween.EaseType.linear
                                                                                    , "rotation", loginSceneCameraAngles // new Vector3(6.827481f, -116, 2f)
                                                                                    , "oncomplete", "RotateLoginEnd"
                                                                                    , "oncompletetarget", this.gameObject));
        iTween.MoveTo(CameraManager.instance.mainCamera.gameObject, iTween.Hash("time", 2f
                                                                                    , "easetype", iTween.EaseType.linear
                                                                                    , "position", loginSceneCameraPos // new Vector3(4.02f, 1.98f, 20.85f)
                                                                                    ));
    }

    void RotateSelectHeroEnd()
    {
        NetData.instance.GetUserInfo().ClearData();
        //캐릭터 선택 패널 띄우기
        UIMgr.Open("UIPanel/SelectHeroPanel", true);
        //선택페널에서 캐릭터 선택이 이뤄지면 
    }

    void RotateLoginEnd()
    {
        NetworkClient.instance.DisconnectGameServer();
        NetData.instance.InitUserData();
        NetData.instance.ClearCharIdc();

        //GameObject go = 
        UIMgr.Open("UIPanel/LoginPanel", false);
        //loginPanel.StartPanel();

        //if (go != null)
        //{
        //    go.GetComponent<LoginPanel>().setLogoEff();
        //}
        
    }
    
    void LoginPhase()
    {
        QuestManager.instance.CleanUp();
        CameraManager.instance.EnableRPGCamera(false);

        CameraManager.instance.transform.position = Vector3.zero;
        CameraManager.instance.transform.eulerAngles = Vector3.zero;
        CameraManager.instance.mainCamera.transform.position = loginSceneCameraPos;
        CameraManager.instance.mainCamera.transform.eulerAngles = loginSceneCameraAngles;
        CameraManager.instance.mainCamera.fieldOfView = 47;

        //UIBasePanel logoPanel = UIMgr.GetUIBasePanel("UIPanel/LogoPanel");
        //if (logoPanel != null)
        //{
        //    GameObject go = UIMgr.Open("UIPanel/LoginPanel", false);
        //    if (go != null)
        //        go.GetComponent<LoginPanel>().EndLoading();

        //    logoPanel.Close();
        //}
        //else
        //{
        //    UIMgr.Open("UIPanel/LoginPanel", false);
        //}

       

        if (SceneManager.instance.IsShowLoadingPanel())
            SceneManager.instance.ShowLoadingTipPanel(false);
    }


    void OnLevelWasLoaded(int level)
    {
		//SceneManager.instance.showStopWatchTimer (" LoginState OnLevelWasLoaded start");
        if (Application.loadedLevelName == "LoginScene")
        {
            CameraManager.instance.transform.position = Vector3.zero;
            CameraManager.instance.transform.eulerAngles = Vector3.zero;
            CameraManager.instance.mainCamera.transform.position = loginSceneCameraPos;
            CameraManager.instance.mainCamera.transform.eulerAngles = loginSceneCameraAngles;
            CameraManager.instance.mainCamera.fieldOfView = 47;

            //MapEnvironmentSetting(Application.loadedLevelName);
            //여기는 로딩전이라 손으로 셋팅하자
            FastBloom bloom = CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<FastBloom>();
            bloom.enabled = false;
            bloom.threshhold = 0.47f;
            bloom.intensity = 0.05f;
            bloom.blurSize = 2.47f;
            bloom.blurIterations = 1;

            if (!FirstLogin)//로그아웃했을경우 이쪽
            {
                LoginPhase();
                UIMgr.Open("UIPanel/LoginPanel", false);
                //LoadingPhase();
            }
            else//최초 실행시 이쪽부터
            {
                //UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPanel/LogoPanel");
                //if (basePanel != null)
                //{
                //(basePanel as LogoPanel).EndLogoStay();

                //(basePanel as LogoPanel).EndCallBack = () =>
                //{
                //    FirstLogin = false;
                //    AssetDownLoad.instance.isLoad = true;

                //    LoginPhase();
                //};
                //}

                LoginPanel loginPanel = UIMgr.Open("UIPanel/LoginPanel", true).GetComponent<LoginPanel>();
                loginPanel.EndCallBack = () => {
                    
                    //AssetDownLoad.instance.isLoad = true;

                    LoginPhase();
                };
                //LoginPhase();
            }
        }

		//SceneManager.instance.showStopWatchTimer (" LoginState OnLevelWasLoaded end");
		//SceneManager.instance.sw.Stop ();
    }
    #region 네트워크 로딩관련(현재안슴)
    //    IEnumerator UILoadUpdate()
    //    {
    //float value = 0.4f, runTime = 0;
    //UIBasePanel netPopup = UIMgr.GetUIBasePanel("UIObject/NetProcess");
    //if (netPopup != null && netPopup is NetProcess)
    //{
    //    NetProcess net = netPopup as NetProcess;
    //    int count = net.GetProcessCount();
    //    float addValue = 0.05f * count;
    //    while (0 < count)
    //    {
    //        runTime += Time.deltaTime;
    //        if (5f < runTime)
    //            break;

    //        if (net.GetProcessCount() == count)
    //        {
    //            yield return new WaitForSeconds(0.1f);
    //            continue;
    //        }

    //        value += addValue;
    //        count = net.GetProcessCount();
    //        SceneManager.instance.LoadingTipPanel.changeLoadingBar(value);

    //        if (0.9f <= value)
    //            break;

    //        yield return null;
    //    }
    //}

    //        float value =0, runTime = 0;
    //        while (value <= 0.9f)
    //        {
    //            runTime += Time.deltaTime;
    //            if (5f < runTime)
    //                break;
    //
    //            value += 0.05f;
    //
    //            if(SceneManager.instance.LoadingTipPanel !=null)
    //                SceneManager.instance.LoadingTipPanel.changeLoadingBar(value);
    //
    //            yield return new WaitForSeconds(0.1f);
    //        }

    //        if (SceneManager.instance.IsShowLoadingPanel())
    //            SceneManager.instance.ShowLoadingTipPanel(false);
    //    }
    #endregion
}

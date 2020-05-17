using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WellFired;

public class CutSceneMgr : MonoSingleton<CutSceneMgr> 
{
    //< 컷씬에 들어갈 타겟 대상 리스트
    static string DefaultPath = "CutScene/DefaultScene/Sequence";
    public static Dictionary<int, GameObject> CutSceneEventDic = new Dictionary<int, GameObject>();

	public static Thinksquirrel.Utilities.CameraShake cutSceneCamShaker;

	public static hoMove cameraMover;

	static SpawnController spawnCtlr;

    public static void AddCutSceneEventTarget(GameObject obj, int Index)
    {
        if (!CutSceneEventDic.ContainsKey(Index))
            CutSceneEventDic.Add(Index, obj);
        else
        {
            if (GameDefine.TestMode)
                Debug.Log("AddCutSceneEventTarget CutSceneEventDic Index Error " + Index, obj);
        }
    }

    public static void LoadCutScene(string path = "")
    {
        if (seq != null)
            DestroyImmediate(seq.gameObject);

        if (path == "")
            seq = ResourceMgr.InstAndGetComponent<USSequencer>(DefaultPath);
        else
            seq = ResourceMgr.InstAndGetComponent<USSequencer>(path);

        CutSceneSendMessage[] list = seq.GetComponentsInChildren<CutSceneSendMessage>(true);
        for (int i = 0; i < list.Length; i++)
        {
            //<==============================================
            //<         미리 선작업을 진행해놓는다.
            //<==============================================
            //if (list[i]._CutSceneEventData != null)
            //{
            //    for (int j = 0; j < list[i]._CutSceneEventData.Length; j++)
            //    {
            //        for (int p = 0; p < list[i]._CutSceneEventData[j].EventList.Length; p++)
            //        {
            //            //< 이펙트 호출이있다면 미리 로드해놓는다
            //            if (list[i]._CutSceneEventData[j].EventList[p].ActionType == eCutSceneEvent.SpawnEffect)
            //                AssetbundleLoader.GetEffect(list[i]._CutSceneEventData[j].EventList[p].ActionString, (obj) => { });
            //        }
            //    }
            //}
        }
    }

    public void OpenCutSceneSkipPanel()
    {
        //UIMgr.Open("CutSceneSkipPanel").GetComponent<CutSceneSkipPanel>().SkipCallBack = SkipEvent;
    }

    //< 한 스테이지당 무조건 한번만 실행된다.
    public static bool StartCheck = false;
    static System.Action EndCall;
    public static void StartCutScene(System.Action _EndCall = null)
    {
        if (StartCheck)
            return;

        EndCall = _EndCall;
        GameInfoBase.NotTimeUpdate = true;

        Time.timeScale = 1;
        StartCheck = true;

        CutSceneSendMessage.PauseAllUnits(true);
		spawnCtlr = getSpawnController ();
		spawnCtlr.setNpcActiveExceptBoss (false);

        G_GameInfo.GameInfo.StartCoroutine(SetCutScene());

        //< 기본 UI들을 숨겨준다
		UIMgr.setHudPanelVisible (false);
		UIMgr.setInGameBoardPanelVisible (false);

	   TempCoroutine.instance.StartCoroutine(BossAnimationPlayed());
    }

	static CinemaSceneManager cinemaSceneManager;
	public static void StartCinemaScene(bool bShowPanelAfterFinish = true, int seqIdx = 0, int endSeqIdx = 0, System.Action _EndCall = null){

		if (StartCheck)
			return;
		
		EndCall = _EndCall;
		GameInfoBase.NotTimeUpdate = true;
		
		Time.timeScale = 1;
		StartCheck = true;
		
		CutSceneSendMessage.PauseAllUnits(true);

		cinemaSceneManager = GameObject.Find ("CinemaSceneManager").GetComponent<CinemaSceneManager> ();

		cinemaSceneManager.storeAliveCamera ();
		cinemaSceneManager.setActiveSequences (true);
		cinemaSceneManager.InActiveOtherCharsExceptMe (NetData.instance.GetUserInfo ()._userCharIndex);

		cinemaSceneManager.seqs[endSeqIdx].PlaybackFinished = (sequence) =>
		{
			EndCinemaScene();
			cinemaSceneManager.setActiveSequences (false);
			cinemaSceneManager.restoreUnactivedCamera();
			cinemaSceneManager.closeDialogPanel();

			// endScene에서 UIPanel들을 살려주기 때문에 컷씬 종료후 UIPanel을 보지 않으려면 revicePanel을 false로 해준다.
			if (bShowPanelAfterFinish == false){
				CutSceneSendMessage.PauseAllUnits(true);
				UIMgr.setHudPanelVisible(false);
				UIMgr.setInGameBoardPanelVisible(false);
                ChatPopup chat = UIMgr.OpenChatPopup();
                if (chat != null)
                    chat.Hide();
                //UIMgr.setChatPopupVisible(false);
            }
		};

		cinemaSceneManager.seqs[seqIdx].Play ();


	}
	
	static IEnumerator bossCutSceneCoroutine;
	static IEnumerator forceFinishCutSceneCoroutine;
	public static void StartBossCutScene(System.Action _EndCall = null)
	{
		if (StartCheck)
			return;
		
		EndCall = _EndCall;
		GameInfoBase.NotTimeUpdate = true;

		//< 기본 UI들을 숨겨준다
		//GameObject InGameHUDPanelGO = UIMgr.GetUI("UIPanel/InGameHUDPanel");
		UIMgr.setHudPanelVisible (false);
		UIMgr.setInGameBoardPanelVisible (false);
		
		Time.timeScale = 1;
		StartCheck = true;
		
		CutSceneSendMessage.PauseAllUnits(true);
		spawnCtlr = getSpawnController ();
		spawnCtlr.setNpcActiveExceptBoss (false);
		
		bossCutSceneCoroutine = DoBossCutScene2 ();
		G_GameInfo.GameInfo.StartCoroutine(bossCutSceneCoroutine);

	}



    public static IEnumerator BossAnimationPlayed()
    {
	    yield return new WaitForSeconds(1.35f);

        UIMgr.OpenAppearBoss();
        InGameHUDPanel hudpanel = UIMgr.GetUI("UIPanel/InGameHUDPanel").GetComponent<InGameHUDPanel>();
	    if (hudpanel == null) yield break;

	    //hudpanel.BossUnit.PlayAnim(eAnimName.Anim_intro);
        hudpanel.BossUnit.ChangeState(UnitState.Event);
        //hudpanel.BossUnit.PlayAnim(eAnimName.Anim_battle_idle, true, 0.1f, true, true);
    }

	public static void EndCinemaScene()
	{
		UIHelper.SetMainCameraActive(true);
		if (spawnCtlr != null)
			spawnCtlr.setNpcActiveExceptBoss(true);
		
		if (EndCall != null)
			EndCall ();
		EndCall = null;
		
		StartCheck = false;
		GameInfoBase.NotTimeUpdate = false;
		Time.timeScale = GameDefine.DefaultTimeScale;
		CutSceneSendMessage.PauseAllUnits(false);
		UIMgr.setHudPanelVisible(true);
		UIMgr.setInGameBoardPanelVisible(true);

	}
	
	//< 씬이 종료될때 호출되는곳
	public static void EndScene( bool InGame = true)
	{
		UIHelper.SetMainCameraActive(true);
		if (spawnCtlr != null)
			spawnCtlr.setNpcActiveExceptBoss(true);
		
		if (EndCall != null)
			EndCall ();
		EndCall = null;
		
		StartCheck = false;
		CutSceneEventDic.Clear();
		
		if(InGame)
			GameInfoBase.NotTimeUpdate = false;

        Time.timeScale = GameDefine.DefaultTimeScale;

        for (int i = 0; i < EffectList.Count; i++)
        {
            if (EffectList[i] != null)
                DestroyImmediate(EffectList[i]);
        }
        EffectList.Clear();

//        GameObject panel = UIMgr.GetUI("CutSceneSkipPanel");
//        if (panel != null)
//            panel.GetComponent<UIBasePanel>().Close();

        if (InGame)
        {
            CutSceneSendMessage.PauseAllUnits(false);

            GameObject appearPanel = UIMgr.GetUI("UIPanel/AppearBossPanel");
            if (appearPanel != null)
                appearPanel.GetComponent<AppearBossPanel>().Close();

			UIMgr.setHudPanelVisible(true);
			UIMgr.setInGameBoardPanelVisible(true);
	   }

	   if (seq != null)
            Destroy(seq.gameObject);

        if (CutSceneSendMessage._CameraFadeEventObj != null)
            Destroy(CutSceneSendMessage._CameraFadeEventObj.gameObject);

        if (startCamObj != null)
            Destroy(startCamObj);

        if (startRootObj != null)
            Destroy(startRootObj);

        if(InGame)
        {
            TempCoroutine.instance.FrameDelay(0.1f, () =>
            {
                //< 혹시나 포커싱카메라가 없을수도있기에 체크
                if (G_GameInfo.GameInfo.FocusingCam == null)
                {
                    G_GameInfo.GameInfo.FocusingCam = ResourceMgr.InstAndGetComponent<FocusingCamera>("Camera/FocusingCamera");
                    G_GameInfo.GameInfo.FocusingCam.SetTargetCamera(CameraManager.instance.mainCamera);

                    if (SkillEventMgr.Live)
                        SkillEventMgr.instance.Setup(CameraManager.instance.RtsCamera, G_GameInfo.GameInfo.FocusingCam);
                }

                //< 포커싱카메라가 안꺼져있을수있으므로 꺼줌
                if (G_GameInfo.GameInfo.FocusingCam != null && G_GameInfo.GameInfo.FocusingCam.gameObject.activeSelf)
                    G_GameInfo.GameInfo.FocusingCam.EndUpdate();
            });
        }
		GameObject UIShadowLight= GameObject.Find ("UI_ShadowLight"); //cyoung 추가
        if(UIShadowLight != null )
		    UIShadowLight.GetComponent<Light> ().enabled = true;
    }

    //< 씬연출을 스킵할때 호출되는 부분
    public static void SkipEvent()
    {
        if (seq != null)
            Destroy(seq.gameObject);

        //< 일단 페이드아웃을 먼저 해준다
        G_GameInfo._GameInfo.StartCoroutine(CutSceneSendMessage._CameraFadeEventObj.FadeOutUpdate(() => 
        {
            //< EndScene를 호출하여 마무리시켜준다.
            EndScene(true);
        }));
    }

	void setCameraFirstPos(){
	}



	static void initStartCamAndShakeCam(){
		//< 카메라 정보 대입
		startRootObj = new GameObject("StartCameraRoot");
		startRootObj.transform.position = CameraManager.instance.RtsCamera.transform.position;
		startRootObj.transform.rotation = CameraManager.instance.RtsCamera.transform.rotation;
		
		startCamObj = new GameObject("StartCamera", typeof(Camera));
		startCamObj.camera.backgroundColor = CameraManager.instance.mainCamera.backgroundColor;
		startCamObj.camera.farClipPlane = CameraManager.instance.mainCamera.farClipPlane;
		//startCamObj.transform.localPosition = startRootObj.transform.position;
		//startCamObj.transform.localRotation = startRootObj.transform.rotation;
		startCamObj.transform.position = CameraManager.instance.RtsCamera.transform.position;
		startCamObj.transform.rotation = CameraManager.instance.RtsCamera.transform.rotation;
		startCamObj.camera.fieldOfView = CameraManager.instance.mainCamera.fieldOfView;

		cutSceneCamShaker = startCamObj.AddComponent<Thinksquirrel.Utilities.CameraShake>();
		cutSceneCamShaker.cameras.Add(startCamObj.GetComponent<Camera>());
		
		CameraManager.instance.cutSceneCamShaker = cutSceneCamShaker;

//		Debug.Log (" startCamObj.transform :" + startCamObj.transform.position);
//
//		Debug.Break ();

	}

	static public bool isBossCutSceneProgress;
	static bool isTestScene = false;
	static IEnumerator DoBossCutScene2()
	{
		isTestScene = false;
		if (GameObject.Find ("CutSceneTestManager")) {
			isTestScene = true;
		}

		isBossCutSceneProgress = true;

		yield return new WaitForSeconds(0.1f);
		
		if (cameraMover == null) {
			if (GameObject.Find("CutSceneCameraMover") != null){
				cameraMover = GameObject.Find("CutSceneCameraMover").GetComponent<hoMove>();
			}

			//cameraMover = new GameObject("CameraMover");
			//cameraMover.AddComponent<hoMove>();
		}

		UIHelper.SetMainCameraDepth(100);

		initStartCamAndShakeCam ();
		
		//startCamObj.transform.parent = cameraMover.gameObject.transform;
		startCamObj.transform.parent = cameraMover.cameraContainer;

		Unit targetUnit = CutSceneEventDic [0].GetComponent<Unit> (); 

		GameObject targetObject = new GameObject ("lookAtTarget");
		targetObject.transform.position = targetUnit.transform.position + Vector3.up * 1.23f;

		cameraMover.bLookAtTarget = true;
		//if (cameraMover.transformForLookAtTarger == null) {
			//cameraMover.transformForLookAtTarger = targetUnit.transform;
			cameraMover.transformForLookAtTarger = targetObject.transform;
		//}


		CutSceneSeqHelper csHelper = GameObject.Find ("CutSceneCameraMover").GetComponent<CutSceneSeqHelper> ();

		GameObject abp = null;
		AppearBossPanel bossPanel = null;
		
		bool isPanelInited = false;
		// fot test loop

		bool bLoop = true;

		while (bLoop) {

			isBossCutSceneProgress = true;
			

			// set first pos
			//startCamObj.transform.localPosition = Vector3.zero;
			//startCamObj.transform.localRotation = Quaternion.identity;
			//startCamObj.transform.localPosition = startRootObj.transform.position;
			//startCamObj.transform.localRotation = startRootObj.transform.rotation;

			//UIHelper.SetMainCameraActive (false);

			//Debug.Break();

			UIHelper.SetMainCameraDepth(100);

			yield return new WaitForSeconds (csHelper.start_delay);
			
			G_GameInfo.GameInfo.FocusingCam.ChangeParent (startCamObj.transform);
			G_GameInfo.GameInfo.FocusingCam.StartEffect (0, true);
			G_GameInfo.GameInfo.FocusingCam.AddObject (targetUnit.gameObject);
			cutSceneCamShaker.cameras.Add(G_GameInfo.GameInfo.FocusingCam.GetComponent<Camera>() );

			// camera move to first path's position.
			cameraMover.transform.position = cameraMover.pathContainer.transform.FindChild("WaypointStart").transform.position;
			cameraMover.transform.LookAt(targetUnit.transform.position);
			cameraMover.cameraContainerLookat();

			startCamObj.transform.localPosition = Vector3.zero;
			startCamObj.transform.localRotation = Quaternion.identity;


			cameraMover.StartMove ();

			// camera change
			UIHelper.SetMainCameraActive (false);
			UIHelper.SetMainCameraDepth(0);

			// wait until cameraMover finish
//			while(cameraMover.bReachedTweenMove==false){
//				yield return null;
//			}

			TempCoroutine.instance.FrameDelay (csHelper.delay_for_name_ani, delegate() {
				if (isPanelInited) {
					bossPanel.DoNameAniAlphaSound ();
				} else {
					UIMgr.OpenAppearBoss ();
					abp = GameObject.Find ("UIPanel/AppearBossPanel");
					bossPanel = abp.GetComponent<AppearBossPanel> ();
					isPanelInited = true;
				}
			});

			TempCoroutine.instance.FrameDelay (csHelper.delay_for_boss_ani, delegate() {
				// boss npc animation
				InGameHUDPanel hudpanel = UIMgr.GetUI ("UIPanel/InGameHUDPanel").GetComponent<InGameHUDPanel> ();
				if (hudpanel != null) {
					hudpanel.BossUnit.ChangeState (UnitState.Event);
				}
			});

			// boss npc 의 애니메이션에서 stopBossCutScene()이 호출되어  coroutine이 종료되므로 실제 이 yield를 기다려서 종료하지 않고 loop도 돌지 않는다. . only for test.
			//yield return new WaitForSeconds (csHelper.sequence_total_time);

			// for safe. prevent loop dead lock.
			//G_GameInfo.GameInfo.StartCoroutine("forceFinishCutScene");
			forceFinishCutSceneCoroutine = forceFinishCutScene();
			G_GameInfo.GameInfo.StartCoroutine(forceFinishCutSceneCoroutine);

			while(isBossCutSceneProgress){
				yield return null;
			}

			bLoop = false;
			if (isTestScene) {
				// delay for repeatly loop test
				bLoop = true;
				yield return new WaitForSeconds (1.0f);
			}
		}

		isBossCutSceneProgress = false;
		
		yield return null;
	}

	public static void stopBossCutScene(){

		if (!isTestScene) {
            if(bossCutSceneCoroutine != null){
                G_GameInfo.GameInfo.StopCoroutine (bossCutSceneCoroutine);
				G_GameInfo.GameInfo.StopCoroutine (forceFinishCutSceneCoroutine);
			}

			EndScene(true);
		}
		
		resetFocusCamAndShaker ();

		isBossCutSceneProgress = false;

		GameObject UIShadowLight= GameObject.Find ("UI_ShadowLight"); //cyoung 추가
		UIShadowLight.GetComponent<Light> ().enabled = true;
	}

	// for safe. prevent loop dead lock.
	static IEnumerator forceFinishCutScene(){

		yield return new WaitForSeconds (10f);

		if (isBossCutSceneProgress) {
			EndScene(true);
		}
	}

	public static void resetFocusCamAndShaker(){
		G_GameInfo.GameInfo.FocusingCam.ChangeParent (CameraManager.instance.mainCamera.transform);
		
		if (G_GameInfo.GameInfo.FocusingCam != null && G_GameInfo.GameInfo.FocusingCam.gameObject.activeSelf){
			G_GameInfo.GameInfo.FocusingCam.EndEffect (false);
		}

		UIHelper.SetMainCameraActive (true);
        if (cutSceneCamShaker != null) {
			cutSceneCamShaker.cameras.Remove (G_GameInfo.GameInfo.FocusingCam.GetComponent<Camera> ());
		}
        //else
        //    Debug.LogError("not found 'curSCeneCamShaker' error ");
		// 원래 컷씬용으로만 쓰려고 만든 이벤트인데 애니메이션에서 이 함수를 콜하게 되어있으나
		// 이 오브젝트가 컷씬 아닌 다른곳에서도 쓰이기때문에 컷씬아닐때에도 이쪽으로 들어오는 경우가 있다. 에러는 아님.

		Time.timeScale = GameDefine.DefaultTimeScale;
	}
	
    public static USSequencer seq;
    public static GameObject startRootObj;
    public static GameObject startCamObj;
    public static Thinksquirrel.Utilities.CameraShake Shaker;
    //static private Dictionary<byte, CameraShakeData> shakeDataDic;
    public static List<GameObject> EffectList = new List<GameObject>();
    static IEnumerator SetCutScene()
    {
        yield return new WaitForSeconds(0.1f);

        //< 카메라 정보 대입
        startRootObj = new GameObject("StartCameraRoot");
        //startRootObj.transform.position = CameraManager.instance.RtsCamera.transform.position;
        //startRootObj.transform.rotation = CameraManager.instance.RtsCamera.transform.rotation;
		Unit targetUnit = CutSceneEventDic[0].GetComponent<Unit>();
		startRootObj.transform.position =new Vector3 (targetUnit.transform.position.x,targetUnit.transform.position.y+targetUnit.UnitCollider.size.y*2.0f,targetUnit.transform.position.z)+targetUnit.transform.forward*35.0f;
		startRootObj.transform.rotation = Quaternion.LookRotation ((targetUnit.transform.position+Vector3.up*(targetUnit.UnitCollider.size.y*0.8f))-startRootObj.transform.position);


        startCamObj = new GameObject("StartCamera", typeof(Camera));
        startCamObj.camera.backgroundColor = CameraManager.instance.mainCamera.backgroundColor;
        startCamObj.camera.farClipPlane = CameraManager.instance.mainCamera.farClipPlane;
        startCamObj.transform.localPosition = startRootObj.transform.position;
        startCamObj.transform.localRotation = startRootObj.transform.rotation;
        startCamObj.camera.fieldOfView = CameraManager.instance.mainCamera.fieldOfView;
		startCamObj.AddComponent<Thinksquirrel.Utilities.CameraShake>();


		//Debug.Break ();
        //int idx = 0;
        bool SetEndPos = false;
        CutSceneSendMessage[] list = seq.GetComponentsInChildren<CutSceneSendMessage>(true);
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i]._CutSceneEventData != null && !list[i].EventSystem)
            {
            }
            else
            {
                for (int j = 0; j < list[i]._CutSceneEventData.Length; j++)
                {
                    for (int p = 0; p < list[i]._CutSceneEventData[j].EventList.Length; p++ )
                    {
                        //< 스타트 위치를 보정해준다
                        if(list[i]._CutSceneEventData[j].EventList[p].ActionType == eCutSceneEvent.SetStartPos)
				    	{
					   		yield return G_GameInfo.GameInfo.StartCoroutine(ResetPosUpdate(list[i]._CutSceneEventData[j].EventList[p].ActionPos, false));
                        }
                        else if (list[i]._CutSceneEventData[j].EventList[p].ActionType == eCutSceneEvent.SetDefaultPos)
				    	{
						   //< 카메라 위치를 조정함
							Vector3 center = Vector3.zero;
                            Vector3 TargetPos = Vector3.zero;
                            foreach (KeyValuePair<int, GameObject> obj in CutSceneEventDic)
                            {
                                if (obj.Value != null)
                                    center += obj.Value.transform.position;
                            }

                            center /= CutSceneEventDic.Count;
                            TargetPos = center;

                            //center += (CutSceneEventDic[0].transform.forward * 7.7f);
							//center += (CutSceneEventDic[0].transform.forward * 9f);
							center += (CutSceneEventDic[0].transform.forward *targetUnit.UnitCollider.size.y * 2.5f );

							/*USTimelineProperty Infolist = seq.GetComponentInChildren<USTimelineProperty>();

							Infolist.Properties [0].curves [0].Keys[3].Time =seq.Duration+10;
							Infolist.Properties [0].curves [1].Keys [3].Time = seq.Duration+10;
							Infolist.Properties [0].curves [2].Keys [3].Time =seq.Duration+10;
							Infolist.Properties [1].curves [0].Keys[3].Time = seq.Duration+10;
							Infolist.Properties [1].curves [1].Keys [3].Time = seq.Duration;
							Infolist.Properties [1].curves [2].Keys [3].Time = seq.Duration+10;
							Infolist.Properties [1].curves [3].Keys [3].Time = seq.Duration+10;

							Infolist.Properties [0].curves[0].Keys[2].Time=seq.Duration;
							Infolist.Properties [0].curves [1].Keys [2].Time = seq.Duration;
							Infolist.Properties [0].curves [2].Keys [2].Time = seq.Duration;
							Infolist.Properties [1].curves [0].Keys[2].Time = seq.Duration;
							Infolist.Properties [1].curves [1].Keys [2].Time = seq.Duration;
							Infolist.Properties [1].curves [2].Keys [2].Time = seq.Duration;
							Infolist.Properties [1].curves [3].Keys [2].Time = seq.Duration;*/

							SetTimeLinePos(center+new Vector3(0,0.0f,0), TargetPos);
                            SetEndPos = true;
					    }
                    }
                }
            }

            //<==============================================
            //<         미리 선작업을 진행해놓는다.
            //<==============================================
            if (list[i]._CutSceneEventData != null)
            {
                for (int j = 0; j < list[i]._CutSceneEventData.Length; j++)
                {
                    for (int p = 0; p < list[i]._CutSceneEventData[j].EventList.Length; p++)
                    {
                        if (list[i]._CutSceneEventData[j].EventList[p].ActionType == eCutSceneEvent.CameraShake && Shaker == null)
                        {
                            Camera _camera = startRootObj.gameObject.AddComponent<Camera>();
                            _camera.clearFlags = CameraClearFlags.Nothing;
                            _camera.cullingMask = 0;

                            Shaker = startRootObj.AddComponent<Thinksquirrel.Utilities.CameraShake>();
                            Shaker.cameras.Add(_camera);
                        }
                    }
                }
            }
        }

        //< 위치 보정을 안했다면 여기서 해준다. (레이드가 아닐경우에만)
        if(!SetEndPos)
            SetTimeLinePos(Vector3.zero, Vector3.zero, true);

        USTimelineContainer[] list2 = seq.GetComponentsInChildren<USTimelineContainer>();
        for (int i = 0; i < list2.Length; i++)
            list2[i].AffectedObject = startCamObj.transform;

        //< 카메라 위치를 변경함(포커스 카메라)
        G_GameInfo.GameInfo.FocusingCam.ChangeParent(startCamObj.transform);

        seq.PlaybackFinished = (sequence) =>
        {
            G_GameInfo.GameInfo.StartCoroutine(ResetPosUpdate(CameraManager.instance.RtsCamera.transform.position, true));
			//G_GameInfo.GameInfo.StartCoroutine(SetCutScene());
        };


        seq.Play();

        //< 메인 카메라는 꺼줌
        UIHelper.SetMainCameraActive(false);

        yield return null;
    }

	static SpawnController getSpawnController(){

		GameObject obj = GameObject.Find("SpawnController");

        if(G_GameInfo.GameMode == GAME_MODE.SINGLE)
        { 
		    //SingleGameInfo stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;
		    DungeonTable.StageInfo stage = _LowDataMgr.instance.GetStageInfo(SingleGameState.lastSelectStageId);
		    if (stage.type == 2) {
			    obj = GameObject.Find("SpawnController_Hard");
		    }
        }

		SpawnController[] spawnCtlrs = obj.GetComponentsInChildren<SpawnController> ();

		//SpawnController[] spawnCtlrs = GameObject.FindObjectsOfType<SpawnController>();

		SpawnController _spawnCtlr;
		if (null == spawnCtlrs || 0 == spawnCtlrs.Length)
			_spawnCtlr = ResourceMgr.InstAndGetComponent<SpawnController>("MapComponent/SpawnController");
		else
			_spawnCtlr = spawnCtlrs[0];

		return _spawnCtlr;
	}

    //< 타임라인의 시작부터 끝까지 위치, 각도를 다시 재설정해준다(현재 위치에 맞게 보정)
    static void SetTimeLinePos(Vector3 center, Vector3 TargetPos, bool Default = false)
    {
        Vector3 localEulerAngles = Vector3.zero;
        Quaternion localQua = Quaternion.identity;
        if (!Default)
        {
            //< 각도를 뽑아내기위해서 도착할 위치로 이동및, 원하는 방향을 바라보도록 처리
            Unit targetUnit = CutSceneEventDic[0].GetComponent<Unit>();
            startCamObj.transform.position = center + (Vector3.up * (targetUnit.UnitCollider.size.y * 0.8f));
            startCamObj.transform.LookAt(TargetPos + (Vector3.up * (targetUnit.UnitCollider.size.y * 1.0f)));

            //< 원하는 각도를 뽑아냄
            localEulerAngles = startCamObj.transform.localEulerAngles;
            localEulerAngles.x = localEulerAngles.x >= 180 ? localEulerAngles.x - 360 : localEulerAngles.x;
            localEulerAngles.y = localEulerAngles.y >= 180 ? localEulerAngles.y - 360 : localEulerAngles.y;
            localEulerAngles.z = localEulerAngles.z >= 180 ? localEulerAngles.z - 360 : localEulerAngles.z;
            localQua = Quaternion.Euler(localEulerAngles);

            if (startRootObj.transform.localRotation.y < 0 && localQua.y > 0)
                localQua.y *= -1;
            else if (startRootObj.transform.localRotation.y > 0 && localQua.y < 0)
                localQua.y *= -1;

            //< 스테이지별로 보정해준다...
            //if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
            //{
            //    switch((G_GameInfo._GameInfo as SingleGameInfo).StageID)
            //    {
            //        case 210:
            //            localQua.z = -0.1f;
            //            break;
            //    }
            //}
            

            //< 복구
            //Debug.Log("목표 카메라 위치 : " + center);
            //Debug.Log("목표 오일러값 : " + startCamObj.transform.localEulerAngles + " , 보정후 오일러값 : " + localEulerAngles);
        }
        
        USTimelineProperty Infolist = seq.GetComponentInChildren<USTimelineProperty>();


        for (int q = 0; q < Infolist.Properties.Count; q++)
        {
            for (int g = 0; g < Infolist.Properties[q].curves.Count; g++)
            {
                for (int s = 0; s < Infolist.Properties[q].curves[g].Keys.Count; s++)
                {
                    if (Infolist.Properties[q].propertyName == "localPosition")
                    {
                        //< 시작과 끝에는 초기위치를 대입
                        //if (G_GameInfo.GameMode != GAME_MODE.RAID)
                        {
                            if (s == 0 || s == Infolist.Properties[q].curves[g].Keys.Count - 1)
                            {
                                if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.x;
                                else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.y;
                                else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.z;
                            }
                            else
                            {
                                //< 카메라 이동 위치이므로, 위에서 최종 위치로 카메라 위치를 변경했었기때문에
                                //< 현재 카메라 위치로 타겟을 잡아줌
                                if (!Default)
                                {
									if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startCamObj.transform.localPosition.x;
									else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startCamObj.transform.localPosition.y;
									else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startCamObj.transform.localPosition.z;
                                }
                            }
                        }
                        /*
                        else if(G_GameInfo.GameMode == GAME_MODE.RAID)
                        {
                            if (s == 0 || s == 1)
                            {
                                //< 레이드일때에는 시작위치값만 대입시켜줌
                                if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.x;
                                else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.y;
                                else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localPosition.z;
                            }
                        }
                        */
                        
                    }
                    else if (Infolist.Properties[q].propertyName == "localRotation" || Infolist.Properties[q].propertyName == "rotation")
                    {
                        //< 시작과 끝에는 초기각도를 대입
                        //if (G_GameInfo.GameMode != GAME_MODE.RAID)
                        {
                            if (s == 0 || s == Infolist.Properties[q].curves[g].Keys.Count - 1)
                            {
                                //< 레이드일때에는 시작위치값만 대입시켜줌
                                if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.x;
                                else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.y;
                                else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.z;
                                else if (g == 3) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.w;
                            }
                            else
                            {
                                //< 디폴트값이 아닐경우는 위에서 뽑아낸 목표 각도를 대입
                                if (!Default)
                                {
									if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.x;//localQua.x;
									else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.y;//localQua.y;
									else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.z; //localQua.z;
									else if (g == 3) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.w;//localQua.w;
                                }
                            }
                        }
                        /*
                        else if(G_GameInfo.GameMode == GAME_MODE.RAID)
                        {
                            if (s == 0 || s == 1)
                            {
                                //< 레이드일때에는 시작위치값만 대입시켜줌
                                if (g == 0) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.x;
                                else if (g == 1) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.y;
                                else if (g == 2) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.z;
                                else if (g == 3) Infolist.Properties[q].curves[g].Keys[s].Value = startRootObj.transform.localRotation.w;
                            }
                        }
                        */
                    }
                }
            }
        }

        //< 다시 시작할 위치로 보정
        startCamObj.transform.localPosition = startRootObj.transform.position;
        startCamObj.transform.localRotation = startRootObj.transform.rotation;
    }

    //< 해당 위치까지 카메라 위치를 이동시켜준다.
    static IEnumerator ResetPosUpdate(Vector3 TargetPos, bool EndCheck = false, float speed = 25)
    {
        //< 최고높이까지 올려주고 시작
        Vector3 pos = startRootObj.transform.position;

        while (true)
        {
            Vector3 look = (TargetPos - pos).normalized;
            pos += look * (speed * Time.deltaTime);
            if (Vector3.Distance(pos, TargetPos) < 0.5f)
            {
                startRootObj.transform.position = TargetPos;
                break;
            }

            startRootObj.transform.position = pos;
            yield return null;
        }

        if (EndCheck)
        {
            //< 포커싱 카메라 위치를 다시 옮겨줌
            G_GameInfo.GameInfo.FocusingCam.ChangeParent(CameraManager.instance.mainCamera.transform);

            EndScene();
        }
    }

	static IEnumerator ResetCutSceneCam(Vector3 TargetPos, float speed = 25)
	{
		//< 최고높이까지 올려주고 시작
		Vector3 pos = startCamObj.transform.position;
		
		while (true)
		{
			Vector3 look = (TargetPos - pos).normalized;
			pos += look * (speed * Time.deltaTime);
			if (Vector3.Distance(pos, TargetPos) < 0.5f)
			{
				startCamObj.transform.position = TargetPos;
				break;
			}
			
			startCamObj.transform.position = pos;
			yield return null;
		}
		
		//if (EndCheck)
		//{
			//< 포커싱 카메라 위치를 다시 옮겨줌
			G_GameInfo.GameInfo.FocusingCam.ChangeParent(CameraManager.instance.mainCamera.transform);
			
			//EndScene();
		//}
	}

    static public Vector3 WorldToUIPosition(Vector3 worldPos)
    {
        return startCamObj.camera.WorldToScreenPoint(worldPos) - MathHelper.UI_Center_Pos;
    }

    public static void Shake(byte shakeType = 1, System.Action callback = null)
    {
        CameraShakeData data;
        if (CameraManager.instance.shakeDataDic.TryGetValue(shakeType, out data))
            Shaker.Shake(data.shakeType, data.numberOfShakes, data.shakeAmount, data.rotationAmount, data.distance, data.speed, data.decay, 0, data.multiplyByTimeScale, () => 
            {
                startRootObj.transform.position = Vector3.zero;
                startRootObj.transform.rotation = Quaternion.identity;
            });
        else
            Shaker.Shake();
    }


}

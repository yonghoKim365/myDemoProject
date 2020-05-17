using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneStateBase : FSM.BaseState<SceneManager>{
	
	public static GameObject GameCamera = null;

    private Color32 _mapLightColor;
    private  float _mapIntensity;
    public static bool IsMapLoad;

    public Color32 GetMapLightColor()
    {
        return _mapLightColor;
    }

    public float GetMapIntensity()
    {
        return _mapIntensity;
    }

    public override void OnEnter(System.Action callback)
	{
        IsMapLoad = false;

        base.OnEnter(callback);
    }

    public override void OnExit(System.Action callback)
    {
        IsMapLoad = false;
        CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<FastBloom>().enabled = false;
        base.OnExit(callback);
    }

    public virtual void SceneSetting()
    {
		string bgmName = Application.loadedLevelName;

		if (bgmName == "maintown_cutscene")
		{
			bgmName = "maintown";
		}

		PlayMapBGM (bgmName);
        //UIMgr.ClearUI();
        CameraManager.instance.SceneInit();
    }

    public void MapEnvironmentSetting(string MapName)
    {
        GameObject mainlightobj = GameObject.Find("main light");

        if (MapName == "tutorial_0101")
            MapName = "single_0101";
        /*else if (MapName == "1zone")
        {
            MapName = "maintown";
        }
		else*/ if (MapName == "maintown_cutscene")
		{
			MapName = "maintown";
		}

        Map.MapDataInfo mapdata = _LowDataMgr.instance.GetMapData(MapName);

        if (mainlightobj != null)
        {
            Light light = mainlightobj.GetComponent<Light>();

            light.cullingMask = 0;
            light.cullingMask = 1 << LayerMask.NameToLayer("Map");
            light.cullingMask ^= 1 << LayerMask.NameToLayer("Unit");
            light.cullingMask ^= 1 << LayerMask.NameToLayer("Focus");

            if (mapdata != null)
            {
                //if(mapdata.AddIntensity != 0)
                //    light.intensity = mapdata.AddIntensity;
                //else
                //    light.intensity = 0.7f;

                light.shadowStrength = mapdata.ShadowStrength;
                light.shadows = LightShadows.Hard;
                light.shadowBias = 0.05f;
                light.shadowSoftness = 4;
                light.shadowSoftnessFade = 1;

                //만약을 위해 예외처리
                if (mapdata.AddColorR == 0 &&
                    mapdata.AddColorG == 0 &&
                    mapdata.AddColorB == 0)
                {
                    //_mapLightColor = light.color;
                    _mapLightColor = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    _mapLightColor = new Color32(mapdata.AddColorR, mapdata.AddColorG, mapdata.AddColorB, 255);
                }

                if (mapdata.AddIntensity == 0)
                {
                    _mapIntensity = 1f;
                }
                else
                {
                    _mapIntensity = mapdata.AddIntensity;
                }
            }
        }

        //로우퀄리티에는 필요없다
        if(QualityManager.instance.GetQuality() == QUALITY.QUALITY_LOW)
        {
            mainlightobj.SetActive(false);
        }

        if (mapdata != null)
        {
            //블룸셋팅
            if(mapdata.Bloom_Enable == 1)
            {
                FastBloom bloom = CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<FastBloom>();
                bloom.enabled = true;
                bloom.threshhold = mapdata.Bloom_Threshhold;
                bloom.intensity = mapdata.Bloom_Intensity;
                bloom.blurSize = mapdata.Bloom_BlurSize;
                bloom.blurIterations = (int)mapdata.Bloom_BlurIterations;
            }
            else
            {
                CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<FastBloom>().enabled = false;
            }
        }
        else
        {
            CameraManager.instance.mainCamera.gameObject.GetComponentInChildren<FastBloom>().enabled = false;
        }
    }

    public void LoadLevelAsync( string scenename, System.Action callback = null, bool noProgress = false )
	{
        //SceneManager.instance.StartCoroutine( LoadLevelScene(scenename, callback) );
        StartCoroutine(LoadLevelScene(scenename, callback, noProgress) );
	}

    /// <summary>
    /// 비동기로 해당 씬을 로드하도록 합니다.
    /// </summary>
    /// <param name="callback">로딩 완료후 호출될 함수</param>
	IEnumerator LoadLevelScene( string scenename, System.Action callback, bool noProgress = false )
	{
        yield return new WaitForEndOfFrame();

        if (!Application.loadedLevelName.Equals("StartScene"))
        {
            UIMgr.ClearUI();
        }

        if (Application.loadedLevelName == scenename)
		{
            Debug.LogWarning("LoadLevel() : 같은 씬(" + scenename + ")을 로드하려고 합니다.");
			yield return null;
		}
		else
		{
            if (noProgress)
            {
                Application.backgroundLoadingPriority = ThreadPriority.High;
                AsyncOperation async = Application.LoadLevelAsync(scenename);
                
                while (!async.isDone)
                    yield return null;
            }
            else
            {
                Application.backgroundLoadingPriority = ThreadPriority.High;

                int displayProgress = 0;
                int toProgress = 0;

                //Debug.Log("op.progress:" + toProgress);

                AsyncOperation op = Application.LoadLevelAsync(scenename);
                op.allowSceneActivation = false;
                while (op.progress < 0.9f)
                {
                    toProgress = (int)op.progress * 100;

                    //Debug.Log("toProgress:" + toProgress);

                    while (displayProgress < toProgress)
                    {
                        ++displayProgress;
                        //SetLoadingPercentage(displayProgress);

                        if (SceneManager.instance.LoadingTipPanel != null)
                        {
                            float value = (float)displayProgress / 100f;
                            SceneManager.instance.LoadingTipPanel.changeLoadingBar(value);
                        }
                        //Debug.Log("displayProgress:"+displayProgress);
                        yield return null;
                    }

                    yield return null;
                }

                toProgress = 100;
                while (displayProgress < toProgress)
                {
                    ++displayProgress;
                    //SetLoadingPercentage(displayProgress);

                    if (SceneManager.instance.LoadingTipPanel != null)
                    {
                        float value = (float)displayProgress / 100f;
                        SceneManager.instance.LoadingTipPanel.changeLoadingBar(value);
                    }

                    //Debug.Log("displayProgress:"+displayProgress);
                    yield return null;
                }

                yield return new WaitForEndOfFrame();

                if (!Application.loadedLevelName.Equals("StartScene"))
                {
                    //AtlasMgr.instance.DeleteAllPanelData();
                    //Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }

                op.allowSceneActivation = true;
            }            
        }
				
		Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
		
        if (null != callback)
		    callback();
	}

    /// <summary>
    /// 싱글스테이지 일때, 카메라 셋팅 변경
    /// </summary>
    /// <param name="isSingle">true라면 싱글스테이지용 카메라로 셋팅</param>
    public void SetupMainCamera(bool isRps = true, GAME_MODE mode = GAME_MODE.SINGLE, bool rotate = false)
    {
        if (null == CameraManager.instance.mainCamera)
            return;

        CameraManager.instance.EnableRPGCamera(isRps);
        if (isRps && mode != GAME_MODE.NONE)
        {
            // InGame용 카메라 셋팅값
            RtsCamera rts = CameraManager.instance.RtsCamera;
            rts.Distance = 16;
            rts.Smoothing = false;
            rts.Rotation = -150;
            rts.Tilt = 36;
            rts.LookAtHeightOffset = 1f;
            rts.MoveDampening = 10f;
            rts.RotationDampening = 5f;
            CameraManager.instance.mainCamera.fieldOfView = 40;

            if (mode == GAME_MODE.ARENA)
            {
                rts.Rotation = 180;
                rts.Distance = 32f;
                rts.MaxDistance = 32f;
                rts.Tilt = 27;
            }
            else if (mode == GAME_MODE.FREEFIGHT)
            {
				rts.Distance = 18;//20;
                rts.Rotation = 0;
				rts.MaxDistance = 18;
				if (FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID){
					rts.Distance = 20;
					rts.Rotation = -150;
				}
            }
            else if (mode == GAME_MODE.TOWER)
            {
                rts.Distance = 16;
                rts.Rotation = -55;
                rts.Tilt = 25;
                CameraManager.instance.mainCamera.fieldOfView = 35;
            }
            //else if (mode == GAME_MODE.SHOOT)
            //{
            //    rts.Rotation = 180;
            //    rts.Distance = 24;
            //    rts.MaxDistance = 24;
            //    rts.Tilt = 32;
            //}

            if (rotate )
            {
                rts.Rotation = -55;
            }
                
        }
        else if( mode == GAME_MODE.NONE)
        {
            RtsCamera rts = CameraManager.instance.RtsCamera;
            rts.Distance = 20f;
            rts.Smoothing = false;
            rts.Rotation = -150;
            rts.Tilt = 36;
            rts.LookAtHeightOffset = 1f;
            rts.MoveDampening = 10f;
            rts.RotationDampening = 5f;
            rts.MaxDistance = 20f;
            //rts.MinDistance = 3f;

            rts.MinDistance = 3.4f;
            rts.MaxDistance = 20f;
            //rts.MinTilt = 4.25f;
            rts.MinTilt = 20f;

            //마을일경우 
            CameraManager.instance.mainCamera.transform.localPosition = new Vector3(0f, 0.8f, 0f);
            CameraManager.instance.mainCamera.fieldOfView = 40;

            //CameraMouseZoom zoom = CameraManager.instance.mainCamera.gameObject.GetComponent<CameraMouseZoom>();
            //zoom.InitSetrot = new Vector3
        }

        Camera mainCam = CameraManager.instance.mainCamera;
        // 안개 색깔을 카메라 clear color로 설정
        mainCam.backgroundColor = RenderSettings.fogColor;
        // 안개로 안덮힌 부분까지만 보이기

        if (TownState.TownActive)
			mainCam.farClipPlane = 100f;
		else
			mainCam.farClipPlane = 1000f;
        //    mainCam.farClipPlane = RenderSettings.fogEndDistance;
    }

    /// <summary> 외부에서 사용하려고 뺏음 </summary>
    public virtual void PlayMapBGM(string bgmName)
    {
        bgmName = _LowDataMgr.instance.GetBGMFile(bgmName);
        //Debug.Log(string.Format("play bgm : <color=blue>{0}</color>", bgmName) );
        if (!string.IsNullOrEmpty(bgmName))
            SoundManager.instance.PlayBgmSound(bgmName);
    }
}

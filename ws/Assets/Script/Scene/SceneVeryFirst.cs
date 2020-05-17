using System;
using UnityEngine;
using System.Collections;
using WmWemeSDK.JSON;
using System.Collections.Generic;
using JsonFx.Json;

public class SceneVeryFirst : MonoBehaviour
{
	public static int shaderMaxLod = 99999;

	public Camera cam;

	public SceneVeryFirst ()
	{
	}
	
	void Awake()
	{

		UILoading.nowLoading = false;

//		GameManager.checkAutoLandScape();

		shaderMaxLod = Shader.globalMaximumLOD;

//		if(SystemInfo.deviceModel.Contains("IM-A870S") || SystemInfo.deviceModel.Contains("LG-F240L"))
//		{
//
//		}

		GameManager.isDebugBuild = Debug.isDebugBuild;

	}
	
	void Start()
	{

		GameManager.initDeviceOrientation();


		PandoraManager.nowCrash = false;
		PandoraManager.startWeme = false;
		PandoraManager.initComplete = false;
		PandoraManager.startDeviceLogin = false;


		StartCoroutine(goToGame());
	}
	
	
	IEnumerator goToGame()
	{
		QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

		#if UNITY_IPHONE
		switch(iPhone.generation)
		{
		case iPhoneGeneration.iPhone4:
		case iPhoneGeneration.iPhone4S:
		case iPhoneGeneration.iPad1Gen:
		case iPhoneGeneration.iPad2Gen:
		case iPhoneGeneration.iPadMini1Gen:		
		case iPhoneGeneration.iPodTouch1Gen:
		case iPhoneGeneration.iPodTouch2Gen:
		case iPhoneGeneration.iPodTouch3Gen:
		case iPhoneGeneration.iPodTouch4Gen:
		case iPhoneGeneration.iPodTouch5Gen:
			ResourceManager.useLowResource = true;
			PerformanceManager.isLowPc = true;
			PlayerPrefs.SetInt("LOWPC",WSDefine.YES);
			Application.targetFrameRate = GameManager.FRAME_RATE_LOW;
			break;
		default:
			Application.targetFrameRate = GameManager.FRAME_RATE_HIGH;
			break;
		}
		#else
		Application.targetFrameRate = GameManager.FRAME_RATE_HIGH;
		#endif

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		QualitySettings.vSyncCount = 0; 

		yield return new WaitForSeconds(1.5f);

		cam.enabled = false;

		bool isMuteSound = (PlayerPrefs.GetInt("SFX") == 0)?false:true;

		if(isMuteSound)
		{
			Handheld.PlayFullScreenMovie("weme_mute.mp4", Color.black, FullScreenMovieControlMode.Hidden);
		}
		else
		{
			Handheld.PlayFullScreenMovie("weme.mp4", Color.black, FullScreenMovieControlMode.Hidden);
		}


		yield return new WaitForSeconds(0.5f);


		Application.LoadLevel(1);
	}
}


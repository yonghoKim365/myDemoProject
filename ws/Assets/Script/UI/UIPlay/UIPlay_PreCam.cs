using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public partial class UIPlay : UIBase 
{


	// ================================================================================
	
	public static bool isPlayingPreCam = false;
	
	public UIPlayCameraEditor camShotEditor;
	List<PlayCamShotData> camShotList = new List<PlayCamShotData>();


	List<Monster> _sorter = new List<Monster>();
	MonsterSorterByTransformPosX _sortByTransformX = new MonsterSorterByTransformPosX();

	public void startPreCam()
	{
		if(HellModeManager.instance.isOpen) return;

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Sigong) return;

		if(GameDataManager.instance.canCutScenePlay == false && GameManager.me.stageManager.nowRound.mode != RoundData.MODE.PVP)
		{
			return;
		}

		if(GameManager.me.playMode == GameManager.PlayMode.replay)
		{
//			GameManager.me.uiManager.uiPlay.btnReplayClose.isEnabled = false;
			//return;
		}

		GameManager.setTimeScale = 1.0f;
		nowSkillEffectCamStatus = SKILL_EFFECT_CAM_STATUS.None;
		isFollowPlayerWhenSkillEffectCamIdle = false;
		usePlayerPositionOffsetWhenSkillEffectCam = false;

		isPlayingPreCam = false;
		camShotList.Clear();
		GameManager.me.characterManager.sort();
		List<Monster> l = GameManager.me.characterManager.monsters;
		
		PlayCamShotData d0 = new PlayCamShotData();
		PlayCamShotData d1 = new PlayCamShotData();
		PlayCamShotData d2 = new PlayCamShotData();
		PlayCamShotData d3 = new PlayCamShotData();
		PlayCamShotData d4 = new PlayCamShotData();

		if(l.Count == 0) return;

		cameraTarget = l[l.Count-1].cTransform;
		
		switch(GameManager.me.stageManager.nowRound.mode)
		{
		case RoundData.MODE.KILLEMALL:
			
			//KILLEMALL  -> 먼 곳에서 서서히 히어로쪽으로 줌인하며 원래 카메라 위치로
			
			if(MathUtil.abs (cameraTarget.position.x , GameManager.me.player.cTransform.position.x) > 1500.0f)
			{
				isPlayingPreCam = true;
				GameManager.setTimeScale = 1.0f;
				GameManager.me.cutSceneManager.useCutSceneCamera = true;
				
				d0.init();
				d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
				d0.time = 0.5f;
				d0.setCamPos = true;
				d0.newCamPos = getCameraCenterPosition(cameraTarget, 0.6f, 0.4f, 15);//20);
				d0.motionTime = 1.5f;
				d0.fov = 15;//20;
				d0.easingType = "EaseOut,Cubic";
				
				d1.init();
				d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
				d1.time = d0.motionTime + 0.01f;
				d1.setCamPos = true;
				d1.newCamPos = getCameraCenterPosition(cameraTarget, 0.4f, 0.5f, 10);
				d1.fov = 10;
				d1.motionTime = 0.6f;
				d1.easingType = "EaseOut,Sine";

				Monster lm = GameManager.me.characterManager.getVeryLeftMonster(false);
				_v.x = -842 + ((lm != null)?lm.cTransform.localPosition.x:0) - GameManager.me.gameCameraContainer.transform.position.x;
				_v.y = 1230;
				_v.z = -2168;
				lm = null;

				d2.init();
				d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
				d2.time = d1.motionTime + 0.01f;
				d2.setCamPos = true;
				d2.newCamPos = _v;//getCameraCenterPosition(GameManager.me.player.cTransform, 0.24f, 0.4f, 15);
				d2.fov = 15;
				d2.setCamRot = true;
				d2.newCamRot = new Vector3(27,25,0);
				d2.motionTime = 3.0f;
				d2.easingType = "EaseOut,Cubic";
				
				d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM;
				d3.time = d2.motionTime + 0.01f;
				
				camShotList.Add(d0);
				camShotList.Add(d1);
				camShotList.Add(d2);
				camShotList.Add(d3);
				
				camShotEditor.playCodes(camShotList.ToArray());
			}

			break;
		case RoundData.MODE.SURVIVAL:
			//		SURVIVAL -> 히어로 뒤쪽에서 원래 위치로	
			isPlayingPreCam = true;
			GameManager.setTimeScale = 1.0f;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;
			
			Vector3 startPos = GameManager.me.player.cTransform.position;
			startPos.x += -1358;
			startPos.y += 411;
			startPos.z += 6;
			
			_v2 = startPos;
			_v2.x += 1360;
			_v2.y -= 250.0f;
			d0.init();
			d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d0.time = 0.5f;
			d0.setCamPos = true;
			d0.newCamPos = _v2;
			d0.fov = 7;
			d0.setCamRot = true;
			d0.newCamRot = new Vector3(1,90,0);
			d0.motionTime = 1.5f;
			d0.easingType = "EaseOut,Sine";
			
			_v2 = startPos;
			_v2.x -= 400.0f;
			d1.init();
			d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d1.time = d0.motionTime + 0.5f;
			d1.setCamPos = true;
			d1.newCamPos = _v2;
			d1.fov = 18;
			d1.setCamRot = true;
			d1.newCamRot = new Vector3(5,90,0);
			d1.motionTime = 0.5f;
			d1.easingType = "EaseOut,Sine";
			
			d2.init();
			d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d2.time = d1.motionTime + 0.5f;
			d2.setCamPos = true;
			d2.newCamPos = new Vector3(296.5178f,1165.279f,-2168.0f);
			d2.fov = 15;
			d2.setCamRot = true;
			d2.newCamRot = new Vector3(27,0,0);
			d2.motionTime = 1.0f;
			d2.easingType = "EaseOut,Cubic";

			d3.init();
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM_WITHOUT_FADE;
			d3.time = d1.motionTime + 0.1f;
			
			gameCameraPosContainer.localPosition = startPos;
			_v.x = 16.0f; _v.y = 90.0f; _v.z = 0.0f;
			_q = gameCamera.transform.localRotation;
			_q.eulerAngles = _v;
			gameCamera.transform.localRotation = _q;
			
			gameCamera.fieldOfView = 16.0f;
			
			
			camShotList.Add(d0);
			camShotList.Add(d1);
			camShotList.Add(d2);
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			
			break;
		case RoundData.MODE.PROTECT:
			//		PROTECT -> 보호 대상과 히어로를 투샷으로 비추고 원래위치로			
			isPlayingPreCam = true;
			GameManager.setTimeScale = 1.0f;
			GameManager.me.cutSceneManager.useCutSceneCameraWithoutClipSetting();
			
			_v = GameManager.me.stageManager.playerProtectObjectMonster[0].cTransform.position;
			
			d0.init();
			d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d0.time = 0.0f;
			d0.setCamPos = true;
			d0.newCamPos = getCameraCenterPosition(_v, 0.55f, 0.4f, 8);
			d0.motionTime = 2.0f;
			d0.fov = 8;
			d0.easingType = "EaseOut,Cubic";
			
			d2.init();
			d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d2.time = d2.motionTime + 2.0f;
			d2.setCamPos = true;
			d2.newCamPos = new Vector3(296.5178f,1165.279f,-2168.0f);
			d2.fov = 15;
			d2.motionTime = 1.5f;
			d2.easingType = "EaseOut,Cubic";
			
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM_WITHOUT_FADE;
			d3.time = d2.motionTime + 0.01f;
			
			camShotList.Add(d0);
			camShotList.Add(d2);
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			break;
		case RoundData.MODE.SNIPING:
			//		SNIPING -> 적 보스를 비춰주다가 원래위치로			
			isPlayingPreCam = true;
			GameManager.me.cutSceneManager.useCutSceneCameraWithoutClipSetting();
			
			d0.init();
			d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d0.time = 0.5f;
			d0.setCamPos = true;
			d0.newCamPos = getCameraCenterPosition(GameManager.me.stageManager.heroMonster[0].cTransform, 0.6f, 0.3f, 20);
			d0.motionTime = 1.5f;
			d0.setCamRot = true;
			d0.newCamRot = new Vector3(27,5,0);
			d0.fov = 22;
			d0.easingType = "EaseOut,Cubic";
			
			
			d1.init();
			d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d1.time = d0.motionTime + 0.01f;
			d1.setCamPos = true;
			d1.newCamPos = getCameraCenterPosition(GameManager.me.stageManager.heroMonster[0].cTransform, 0.4f, 0.3f, 10);
			d1.fov = 10;
			d0.newCamRot = new Vector3(27,0,0);
			d1.motionTime = 1.5f;
			d1.easingType = "EaseOut,Sine";
			
			
			d2.init();
			d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d2.time = d1.motionTime + 0.01f;
			d2.setCamPos = true;
			d2.newCamPos = new Vector3(296.5178f,1165.279f,-2168.0f);
			d2.fov = 15;
			d2.motionTime = 3.0f;
			d2.easingType = "EaseOut,Cubic";
			
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM_WITHOUT_FADE;
			d3.time = d2.motionTime + 0.01f;
			
			camShotList.Add(d0);
			camShotList.Add(d1);
			camShotList.Add(d2);
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			
			break;
		case RoundData.MODE.KILLCOUNT:
			//		KILLCOUNT -> 해당 몬스터 3마리를 히어로와 먼 순서대로 샷으로 때려주고 원래위치로			
			isPlayingPreCam = true;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;


			_v.x = 18; _v.y = 90; _v.z = 0;
			_q.eulerAngles = _v;
			gameCamera.transform.localRotation = _q;

			_v.x = -1079;
			_v.y = 411;
			_v.z = 0;
			gameCameraPosContainer.transform.localPosition = _v;

			gameCamera.fieldOfView = 18;


			string kid = GameManager.me.stageManager.nowRound.killMonsterIds[0];//mapManager.leftKilledMonsterNum

			l.Sort(_sortByTransformX);

			int nowIndex = 0;
			for(int i = l.Count - 1; i >= 0; --i)
			{
				if(l[i].unitData != null && l[i].unitData.id == kid)
				{
					_v = l[i].cTransform.position;
					_v.x += -1358;
					_v.y += 411;
					_v.z += -100;
					
					switch(nowIndex)
					{
					case 0:
						
						d0.init();
						d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d0.time = 0.5f;
						d0.setCamPos = true;
						d0.newCamPos = _v;
						d0.motionTime = 1.2f;
						d0.setCamRot = true;
						d0.newCamRot = new Vector3(18,84,0);
						d0.fov = 18;
						d0.easingType = "EaseOut,Cubic";
						camShotList.Add(d0);
						break;
					case 1:
						
						d1.init();
						d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d1.time = 1.5f;
						d1.setCamPos = true;
						d1.newCamPos = _v;
						d1.motionTime = 0.3f;
						d1.setCamRot = true;
						d1.newCamRot = new Vector3(18,84,0);
						d1.fov = 18;
						d1.easingType = "EaseOut,Cubic";
						camShotList.Add(d1);
						break;
					case 2:
						
						d2.init();
						d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d2.time = 0.6f;
						d2.setCamPos = true;
						d2.newCamPos = _v;
						d2.motionTime = 0.3f;
						d2.setCamRot = true;
						d2.newCamRot = new Vector3(18,84,0);
						d2.fov = 18;
						d2.easingType = "EaseOut,Cubic";
						camShotList.Add(d2);
						break;
					}
					
					++nowIndex;
				}
			}
			
			
			d3.init();
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM;
			d3.time = 0.6f;
			
			camShotList.Add(d3);
			
			//			gameCameraPosContainer.localPosition = startPos;
			//			_v.x = 16.0f; _v.y = 90.0f; _v.z = 0.0f;
			//			_q = gameCamera.transform.localRotation;
			//			_q.eulerAngles = _v;
			//			gameCamera.transform.localRotation = _q;
			//			
			//			gameCamera.fieldOfView = 16.0f;
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			
			
			
			break;
		case RoundData.MODE.ARRIVE:
			//		ARRIVE -> 목적지점에서 그대로 왼쪽으로 (추격몬스터 보여주고) 원래 위치로 이동			
			isPlayingPreCam = true;
			GameManager.me.cutSceneManager.useCutSceneCameraWithoutClipSetting();
			
			_v = getCameraCenterPosition(new Vector3(GameManager.me.stageManager.nowRound.targetPos,0,0), 0.5f, 0.4f, 15);//18);
			
			gameCameraPosContainer.localPosition = _v;
			
			
			
			if(GameManager.me.stageManager.chaser != null)
			{
				d0.init();
				d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
				d0.time = 1.0f;
				d0.setCamPos = true;
				d0.newCamPos = getCameraCenterPosition(GameManager.me.stageManager.chaser.transform, 0.2f, 0.4f, 15);//18);
				d0.motionTime = 0.5f;
				d0.fov = 15;//18;
				d0.easingType = "EaseOut,Cubic";
				camShotList.Add(d0);
				
			}
			
			d2.init();
			d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d2.time = 1.5f;
			d2.setCamPos = true;
			d2.newCamPos = new Vector3(296.5178f,1165.279f,-2168.0f);
			d2.fov = 15;
			d2.motionTime = 0.5f;
			d2.easingType = "EaseOut,Cubic";
			camShotList.Add(d2);
			
			
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM_WITHOUT_FADE;
			d3.time = 0.5f;
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			break;
		case RoundData.MODE.DESTROY:
			//		DESTROY -> 목표물 전부 샷으로 보여주고, (추격몬스터 보여주고) 원래위치로 이동
			isPlayingPreCam = true;
			GameManager.me.cutSceneManager.useCutSceneCameraWithoutClipSetting();
			
			
//			d0.init();
//			d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
//			d0.time = 0.0f;
//			d0.setCamPos = true;
//			d0.newCamPos = getCameraCenterPosition(cameraTarget, 0.6f, 0.4f, 20);
//			d0.motionTime = 0.5f;
//			d0.fov = 20;
//			d0.easingType = "EaseOut,Cubic";
//			
//			camShotList.Add(d0);
			
			// 샷.
			float prevTile = d0.motionTime;

			for(int i = GameManager.me.stageManager.playerDestroyObjectMonster.Length - 1; i >= 0; --i)
			{
				_sorter.Add(GameManager.me.stageManager.playerDestroyObjectMonster[i]);
			}

			_sorter.Sort(_sortByTransformX);

			int q = 0;
			for(int i = _sorter.Count - 1; i >= 0; --i)
			{
				PlayCamShotData pd = new PlayCamShotData();
				pd.init();
				pd.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
				pd.time = 0.8f;
				pd.setCamPos = true;
				pd.newCamPos = getCameraCenterPosition(_sorter[i].cTransform, 0.4f, 0.3f, 11);
				pd.fov = 11;
				pd.motionTime = 0.3f;
				pd.easingType = "EaseOut,Sine";
				camShotList.Add(pd);
				++q;
			}

			_sorter.Clear();

			// 샷.
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM;
			d3.time = camShotList[camShotList.Count-1].motionTime + 0.5f;
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());

			
			break;
		case RoundData.MODE.GETITEM:
			//		GETITEM -> 앞에서 목표 유닛 3마리를 샷으로 때리면서 원래위치로 이동
			isPlayingPreCam = true;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;

			_v.x = 18; _v.y = 90; _v.z = 0;
			_q.eulerAngles = _v;
			gameCamera.transform.localRotation = _q;
			
			_v.x = -1079;
			_v.y = 411;
			_v.z = 0;
			gameCameraPosContainer.transform.localPosition = _v;
			
			gameCamera.fieldOfView = 18;

			l.Sort(_sortByTransformX);

			int j = 0;

			int mlen = l.Count;

			for(int i = 0; i < mlen; ++i)
			{
				if(l[i].unitData != null && GameManager.me.stageManager.nowRound.getItemData.isCreateItemMonster(l[i].unitData.id))
				{
					_v = l[i].cTransform.position;
					_v.x += -1358;
					_v.y += 411;
					_v.z += -100;
					
					switch(j)
					{
					case 0:
						
						d0.init();
						d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d0.time = 1.0f;
						d0.setCamPos = true;
						d0.newCamPos = _v;
						d0.motionTime = 1.2f;
						d0.setCamRot = true;
						d0.newCamRot = new Vector3(18,84,0);
						d0.fov = 18;
						d0.easingType = "EaseOut,Cubic";
						camShotList.Add(d0);
						break;
					case 1:
						
						d1.init();
						d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d1.time = 1.5f;
						d1.setCamPos = true;
						d1.newCamPos = _v;
						d1.motionTime = 0.3f;
						d1.setCamRot = true;
						d1.newCamRot = new Vector3(18,84,0);
						d1.fov = 18;
						d1.easingType = "EaseOut,Cubic";
						camShotList.Add(d1);
						break;
					case 2:
						
						d2.init();
						d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
						d2.time = 0.6f;
						d2.setCamPos = true;
						d2.newCamPos = _v;
						d2.motionTime = 0.3f;
						d2.setCamRot = true;
						d2.newCamRot = new Vector3(18,84,0);
						d2.fov = 18;
						d2.easingType = "EaseOut,Cubic";
						camShotList.Add(d2);
						break;
					}
					
					++j;
				}
			}
			
			
			d3.init();
			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM;
			d3.time = 0.6f;
			
			camShotList.Add(d3);
			
			//			gameCameraPosContainer.localPosition = startPos;
			//			_v.x = 16.0f; _v.y = 90.0f; _v.z = 0.0f;
			//			_q = gameCamera.transform.localRotation;
			//			_q.eulerAngles = _v;
			//			gameCamera.transform.localRotation = _q;
			//			
			//			gameCamera.fieldOfView = 16.0f;
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			break;



		case RoundData.MODE.PVP:

			isPlayingPreCam = true;
			//GameManager.me.cutSceneManager.useCutSceneCameraWithoutClipSetting();

			GameManager.me.cutSceneManager.useCutSceneCamera = true;

			_v.x = 767.6f; _v.y = 2135.43f; _v.z = -3127.116f;
			gameCameraPosContainer.localPosition = _v;
			_q.eulerAngles = new Vector3(31.08f,0,0);
			gameCamera.transform.localRotation = _q;
			gameCamera.fieldOfView = 16.2f;

			d1.init();
			d1.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d1.time = 1.0f;
			d1.setCamPos = true;
			d1.newCamPos = new Vector3(767.6f,854.3f,-2153.043f);
			d1.setCamRot = true;
			d1.newCamRot = new Vector3(15.2f,20.85f,0.0f);

			d1.fov = 14.2f;
			d1.motionTime = 1.0f;
			d1.easingType = "EaseOut,Cubic";
			camShotList.Add(d1);


			d2.init();
			d2.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d2.time = d1.time + 0.1f;
			d2.setCamRot = true;
			d2.newCamRot = new Vector3(15.2f,-18.16f,0.0f);
			
			d2.fov = 14.2f;
			d2.motionTime = 0.5f;
			d2.easingType = "EaseOut,Cubic";
			camShotList.Add(d2);


			d0.init();
			d0.playType = UIPlayCameraEditor.PLAY_TYPE_MOVE;
			d0.time = 0.6f;
			d0.setCamPos = true;
			_v.x = 767.6f; _v.y = 1630.052f; _v.z = -2616.303f;
			d0.newCamPos = _v;

			d0.setCamRot = true;
			d0.newCamRot = new Vector3(29.12f,0.0f,0.0f);

			d0.motionTime = 0.5f;
			d0.fov = 20.6f; // 22.6f
			d0.easingType = "EaseOut,Cubic";
			camShotList.Add(d0);



			d3.playType = UIPlayCameraEditor.PLAY_TYPE_RESETCAM;
			d3.time = 0.51f;
			camShotList.Add(d3);
			
			camShotEditor.playCodes(camShotList.ToArray());
			
			break;

		default:
			isPlayingPreCam = false;
			GameManager.me.cutSceneManager.useCutSceneCamera = false;
			break;
		}

		goEpicPreCamInfo.SetActive(isPlayingPreCam && (GameManager.me.stageManager.nowRound.mode != RoundData.MODE.PVP));


		cameraTarget = GameManager.me.player.cTransform;

		SoundManager.instance.stopTutorialVoice();
		SoundData.play("precam_bgm");
	}
	
	
	static Transform player
	{
		get
		{
			return GameManager.me.player.cTransform;
		}
	}



}
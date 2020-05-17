using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPlayCameraEditor : MonoBehaviour {

	public const int PLAY_TYPE_SHOT = 0;// shot 1 move 2 rot 3 resetcam
	public const int PLAY_TYPE_MOVE = 1;
	public const int PLAY_TYPE_ROTATE = 2;

	public const int PLAY_TYPE_RESETCAM = 3;
	public const int PLAY_TYPE_RESETCAM_WITHOUT_FADE = 4;



	public string[] codeArray;
	private int nowIndex = 0;
	public List<CamShotPlayTimeAndCode> _list = new List<CamShotPlayTimeAndCode>();

	public void startPlayCodes()
	{
		++nowIndex;

		_list.Clear();

		for(int i = 0; i < codeArray.Length; ++i)
		{
			CamShotPlayTimeAndCode tc = new CamShotPlayTimeAndCode();
			string[] t = codeArray[i].Split('|');
			float.TryParse(t[0], out tc.time);
			int.TryParse(t[1], out tc.playType);
			tc.code = t[2];
			_list.Add(tc);
		}

		StartCoroutine(playCodesEditor(_list.ToArray(), nowIndex));
		_list.Clear();
	}

	IEnumerator playCodesEditor(CamShotPlayTimeAndCode[] codes, int index)
	{
		for(int i = 0; i < codes.Length; ++i)
		{
			if(index != nowIndex) break;
			yield return new WaitForSeconds(codes[i].time);
			codeToParam(codes[i].code);

			switch(codes[i].playType)
			{
			case PLAY_TYPE_SHOT: playShot(); break;
			case PLAY_TYPE_MOVE: playMove(); break;
			case PLAY_TYPE_ROTATE: playRotate(); break;
			case PLAY_TYPE_RESETCAM: resetCamera(); break;
			case PLAY_TYPE_RESETCAM_WITHOUT_FADE: resetCamera(); break;
			}

		}
		yield return null;
	}


	public void playCodes(PlayCamShotData[] codes)
	{
		++nowIndex;
		StartCoroutine(playCodesPlay(codes, nowIndex));
	}


	IEnumerator playCodesPlay(PlayCamShotData[] codes, int index)
	{
		GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_IN, 0, true);

		for(int i = 0; i < codes.Length; ++i)
		{
			if(index != nowIndex) break;
			yield return new WaitForSeconds(codes[i].time);
			dataToParam(codes[i]);
			
			switch(codes[i].playType)
			{
			case PLAY_TYPE_SHOT: playShot(); break;
			case PLAY_TYPE_MOVE: playMove(); break;
			case PLAY_TYPE_ROTATE: playRotate(); break;
			case PLAY_TYPE_RESETCAM: 

				GameManager.me.uiManager.uiPlay.goEpicPreCamInfo.SetActive(false);

				GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_OUT, 0.2f, true);
				yield return new WaitForSeconds(0.2f);
				GameManager.me.cutSceneManager.useCutSceneCamera = false;
				resetCamera(); 
				GameManager.me.uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_IN, 0.2f, true);
				yield return new WaitForSeconds(0.2f);
				UIPlay.isPlayingPreCam = false;
				break;

			case PLAY_TYPE_RESETCAM_WITHOUT_FADE:

				GameManager.me.uiManager.uiPlay.goEpicPreCamInfo.SetActive(false);
				yield return new WaitForSeconds(0.3f);
				GameManager.me.cutSceneManager.useCutSceneCamera = false;
				resetCamera(); 
				UIPlay.isPlayingPreCam = false;
				break;
			}
		}
		yield return null;

	}



	public string code = "";

	public bool hideBG = true;

	public UIPlay play;

	public Transform target;

	public bool setCamCenter;
	public Vector2 centerPoint;
	public bool setCamPos;
	public Vector3 newCamPos;
	public bool setCamRot;
	public Vector3 newCamRot;
	public float fov;
	public float motionTime;

//	const int CAM_MOVE_STOP = 0;
//	const int CAM_MOVE_POSITION = 1;
//	const int CAM_MOVE_ROTATION = 2;

	public enum camType
	{
		CAM_MOVE_STOP, CAM_MOVE_POSITION, CAM_MOVE_ROTATION
	}

	public camType newMotionType;
	public camType nowMotionType;

	private int _newMotionType = -1;
	private int _nowMotionType = -1;

	public string easingType = null;

	public void resetCamera()
	{
		GameManager.me.mapManager.inGameMap.setVisible(true);
		GameManager.me.uiManager.uiPlay.resetCamera();

		if(GameManager.me.stageManager.nowRound != null && GameManager.me.stageManager.nowRound.id == "PVP")
		{
			GameManager.me.uiManager.uiPlay.resetToChallengeModeZoomDefaultCamera();
		}
	}

	public void playShot()
	{
		if(hideBG)
		{
			GameManager.me.mapManager.inGameMap.setVisible(false);
			GameManager.me.gameCamera.backgroundColor = Color.black;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;// useCutSceneCameraWithoutClipSetting();
		}

		switch(nowMotionType)
		{
		case camType.CAM_MOVE_STOP: _nowMotionType = 100; break;
		case camType.CAM_MOVE_POSITION: _nowMotionType = 1; break;
		case camType.CAM_MOVE_ROTATION: _nowMotionType = 2; break;
		}

		switch(newMotionType)
		{
		case camType.CAM_MOVE_STOP: _newMotionType = 100; break;
		case camType.CAM_MOVE_POSITION: _newMotionType = 1; break;
		case camType.CAM_MOVE_ROTATION: _newMotionType = 2; break;
		}

		if(transform == null) target = GameManager.me.player.cTransform;
		if(string.IsNullOrEmpty(easingType)) easingType = null;
		play.setCutSceneCamera( target,  setCamCenter,  centerPoint,  setCamPos,  newCamPos,  fov,  setCamRot,  newCamRot,  _newMotionType );
	}

	public void playMove()
	{
		if(hideBG)
		{
			GameManager.me.mapManager.inGameMap.setVisible(false);
			GameManager.me.gameCamera.backgroundColor = Color.black;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;// useCutSceneCameraWithoutClipSetting();
		}


		switch(nowMotionType)
		{
		case camType.CAM_MOVE_STOP: _nowMotionType = 100; break;
		case camType.CAM_MOVE_POSITION: _nowMotionType = 1; break;
		case camType.CAM_MOVE_ROTATION: _nowMotionType = 2; break;
		}
		
		switch(newMotionType)
		{
		case camType.CAM_MOVE_STOP: _newMotionType = 100; break;
		case camType.CAM_MOVE_POSITION: _newMotionType = 1; break;
		case camType.CAM_MOVE_ROTATION: _newMotionType = 2; break;
		}

		if(transform == null) target = GameManager.me.player.cTransform;
		if(string.IsNullOrEmpty(easingType)) easingType = null;
		play.setCutSceneCameraMove(target, setCamCenter, centerPoint, setCamPos, newCamPos, setCamRot, newCamRot, fov, motionTime, _newMotionType, _nowMotionType, easingType);
	}

	public void playRotate()
	{
		if(hideBG)
		{
			GameManager.me.mapManager.inGameMap.setVisible(false);
			GameManager.me.gameCamera.backgroundColor = Color.black;
			GameManager.me.cutSceneManager.useCutSceneCamera = true;// useCutSceneCameraWithoutClipSetting();
		}


		if(transform == null) target = GameManager.me.player.cTransform;
		play.setCutSceneCameraRot( target,  setCamCenter,  centerPoint,  fov,  setCamRot,  newCamRot);
	}

	void dataToParam(PlayCamShotData data)
	{
		setCamCenter = data.setCamCenter;
		centerPoint = data.centerPoint;
		setCamPos = data.setCamPos;
		newCamPos = data.newCamPos;
		setCamRot = data.setCamRot;
		newCamRot = data.newCamRot;
		fov = data.fov;
		motionTime = data.motionTime;
		newMotionType = data.newMotionType;
		nowMotionType = data.nowMotionType;
		easingType = data.easingType;
	}



	public void codeToParam(string source = null)
	{
		string[] c = (source == null)?code.Split(','):source.Split(',');

		if(c.Length < 16) return;

		setCamCenter = (c[0]=="1");

		float.TryParse(c[1], out centerPoint.x);
		float.TryParse(c[2], out centerPoint.y);

		setCamPos = (c[3]=="1");

		float.TryParse(c[4], out newCamPos.x);
		float.TryParse(c[5], out newCamPos.y);
		float.TryParse(c[6], out newCamPos.z);

		setCamRot = (c[7]=="1");

		float.TryParse(c[8], out newCamRot.x);
		float.TryParse(c[9], out newCamRot.y);
		float.TryParse(c[10], out newCamRot.z);

		float.TryParse(c[11], out fov);
		float.TryParse(c[12], out motionTime);

		switch(c[13])
		{
		case "1":
			newMotionType =  camType.CAM_MOVE_POSITION;
			break;
		case "2":
			newMotionType = camType.CAM_MOVE_ROTATION;
			break;
		case "100":
			newMotionType = camType.CAM_MOVE_STOP;
			break;
		}

		switch(c[14])
		{
		case "1":
			nowMotionType =  camType.CAM_MOVE_POSITION;
			break;
		case "2":
			nowMotionType = camType.CAM_MOVE_ROTATION;
			break;
		case "100":
			nowMotionType = camType.CAM_MOVE_STOP;
			break;
		}

		if(string.IsNullOrEmpty(c[15]) == false && c.Length >= 17)
		{
			easingType = c[15]+","+c[16];
		}
		else easingType = "";



	}

	public void paramToCode()
	{
		string settingCode = "";

		settingCode += ((setCamCenter)?"1":"0") + ",";

		settingCode += centerPoint.x+","+ centerPoint.y+ ",";

		settingCode += ((setCamPos)?"1":"0") + ",";

		settingCode += newCamPos.x+","+ newCamPos.y+ ","+ newCamPos.z+ ",";

		settingCode += ((setCamRot)?"1":"0") + ",";
		
		settingCode += newCamRot.x+","+ newCamRot.y+ ","+ newCamRot.z+ ",";

		settingCode += fov.ToString() + ",";

		settingCode += (motionTime.ToString()) + ",";

		switch(newMotionType)
		{
		case camType.CAM_MOVE_POSITION:
			settingCode += "1,";
			break;
		case camType.CAM_MOVE_ROTATION:
			settingCode += "2,";
			break;
		case camType.CAM_MOVE_STOP:
			settingCode += "100,";
			break;
		}

		switch(nowMotionType)
		{
		case camType.CAM_MOVE_POSITION:
			settingCode += "1,";
			break;
		case camType.CAM_MOVE_ROTATION:
			settingCode += "2,";
			break;
		case camType.CAM_MOVE_STOP:
			settingCode += "100,";
			break;
		}

		settingCode += easingType;

		code = settingCode;
	}


	public Vector3 targetScreenPosition = new Vector3(); // 1,2는 비율. 3은 목표 fov
	public Vector3 cameraLocalPositionResult = new Vector3();

	public void getPositionCameraLocalPosition()
	{
		cameraLocalPositionResult = GameManager.me.uiManager.uiPlay.getCameraCenterPosition(target,targetScreenPosition.x, targetScreenPosition.y, targetScreenPosition.z);
	}


	public void copyCamLocalPositionToValue()
	{
		newCamPos = cameraLocalPositionResult;
		fov = targetScreenPosition.z;
	}


}


public struct CamShotPlayTimeAndCode
{
	public float time;
	public int playType; // 0 shot 1 move 2 rot
	public string code;
}



public struct PlayCamShotData
{
	public float time;
	public int playType;  // 0 shot 1 move 2 rot 3 resetcam

	public bool setCamCenter;
	public Vector2 centerPoint;
	public bool setCamPos;
	public Vector3 newCamPos;
	public bool setCamRot;
	public Vector3 newCamRot;
	public float fov;
	public float motionTime;

	public UIPlayCameraEditor.camType newMotionType;
	public UIPlayCameraEditor.camType nowMotionType;
	
	public string easingType;

	public void init()
	{
		setCamCenter = false;
		setCamPos = false;
		setCamRot = false;
		motionTime = 0.0f;
		fov = -1;
		nowMotionType = UIPlayCameraEditor.camType.CAM_MOVE_STOP;
		newMotionType = UIPlayCameraEditor.camType.CAM_MOVE_STOP;
		easingType = null;
	}

}




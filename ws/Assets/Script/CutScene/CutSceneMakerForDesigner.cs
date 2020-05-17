using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CutSceneMakerForDesigner : MonoBehaviour {

	public static CutSceneMakerForDesigner instance;

	public bool useCutSceneMaker = false;

	public string cutSceneId = "";

	public int targetSearchTime = 5;

	void Awake () {
		instance = this;
#if UNITY_EDITOR
#else
		useCutSceneMaker = false;
#endif
	}

	public void play()
	{
		if(GameManager.info.cutSceneData.ContainsKey(cutSceneId) == false)
		{
			GameManager.me.clientDataLoader.loadAndAddCutSceneData(cutSceneId);
		}

		if(cutSceneId != null && GameManager.info.cutSceneData.ContainsKey(cutSceneId))
		{
			CutSceneMaker.useSearchMode = false;

			GameManager.info.roundData["PVP"].cutSceneId = cutSceneId;
			GameManager.me.stageManager.setNowRound(GameManager.info.roundData["PVP"],GameType.Mode.Championship);
			GameManager.me.startGame(0);
		}
	}


	public void goToTime(float time)
	{
		if(time > 1)
		{
			CutSceneMaker.useSearchMode = true;
			CutSceneMaker.targetTime = time;
		}
		
		GameManager.me.startQuickRestart();
	}

}

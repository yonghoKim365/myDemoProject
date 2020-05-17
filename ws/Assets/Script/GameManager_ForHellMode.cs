using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour 
{
	public IEnumerator nextHellMode(float waitTime = 0.0f)
	{
		Monster.uniqueIndex = 0;

		needClearWork = false;
		_openContinuePopup = false;
		
		cutSceneMapManager.visible = false;
		mapManager = normalMapManager;

		mapManager.isSetStage = false;
		playGameOver = false;
		needClearWork = false;

		yield return new WaitForSeconds(0.5f);

		uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.SIDE_FADE_IN, 1.0f);

		yield return new WaitForSeconds(0.6f);

		uiManager.uiPlay.warningAlpha.stop();

		float t = RealTime.time;

		loadRoundMonsterModelData(true);

		while(gameDataManager.isCompleteLoadMap == false){ yield return null; };
		while(gameDataManager.isCompleteLoadModel == false){ yield return null; };
		while(SoundManager.instance.isCompleteLoadSound == false){ yield return null; };
		while(effectManager.isCompleteLoadEffect == false){ yield return null; };

//		t = RealTime.time - t;
//		t = 1.0f - t;
//
//		if(t > 0) yield return  new WaitForSeconds(t);

		MapData.destroyExceptionResource = GameManager.info.mapData[stageManager.getMapId()].resource;

		int prevHp = Mathf.CeilToInt(player.maxHp);
		int prevSp = Mathf.CeilToInt(player.maxSp);
		int prevMp = Mathf.CeilToInt(player.mp);

		// 이계던전에서는 플레이타임은 매 웨이브마다 초기화(0)되고
		// total 시간이 줄어둔다. 
		int prevTime = Mathf.CeilToInt(GameManager.me.stageManager.playTime);
		HellModeManager.instance.leftTime -= prevTime;

		HellModeManager.instance.totalPlayTime += Mathf.CeilToInt(GameManager.me.stageManager.playTime.Get());


		mapManager.hideMap();
		clearStage();

		effectManager.deleteExceptionList.Clear();

		uiManager.uiPlay.goHellGoAni.SetActive(false);

		GameManager.me.stageManager.playTime = prevTime;

		inGameRandom = new Well512Random((uint)UnityEngine.Random.Range(1000,99999));

		if(DebugManager.useTestRound == false)
		{
			EpiServer.instance.sendSaveHell(prevHp, prevSp, prevMp, prevTime, HellModeManager.instance.totalPlayTime);
			
			while(EpiServer.instance.waitForSaveWaveToServer)
			{
				yield return null;
			}
		}

		player.clearAnimationMethod();
		player.onCompleteAttackAni();	
		player.onCompleteSkillAni(null);
		player.isEnabled = true;
		player.state = Monster.NORMAL;
		player.renderAniRightNow();


		GamePlayerData nowPlayerData = GameDataManager.selectedPlayerData;
		changeMainPlayer(GameDataManager.selectedPlayerData,nowPlayerData.id,nowPlayerData.partsVehicle.parts.resource.ToUpper());

		player.isEnabled = true;
		player.state = Monster.NORMAL;		

		player.hp = prevHp;
		player.sp = prevSp;
		player.mp = prevMp;

		player.hpBar.visible = false;

		MapData.destroyExceptionResource = null;
		yield return null;
		characterManager.clearUnusedResource(false, prevLoadingMonsterResource);

		yield return new WaitForSeconds(0.2f);

		yield return Resources.UnloadUnusedAssets();
		yield return null;
		System.GC.Collect();
		// 기본적으로 스테이지에서 쓸 배경은 만드는데...
		normalMapManager.createBackground(stageManager.getMapId(), true);
		mapManager.visible = true;

		//uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.FADE_IN, 1.0f);
		uiManager.uiLayoutEffect.start(UILayoutEffect.LayoutTransitionEffect.SIDE_FADE_OUT, 1.0f);

		uiManager.uiPlay.init();

		currentScene = Scene.STATE.PLAY_READY;

		yield return new WaitForSeconds(2.5f);

		uiManager.uiLayoutEffect.sideFade.gameObject.SetActive(false);

		yield return new WaitForSeconds(2.5f);
		
		uiManager.uiLayoutEffect.sideFade.gameObject.SetActive(false);
	}


}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour {

	public bool currentSimulatorMatchIsFirst = false;

	public void startSimulator(bool isFirst = false)
	{
		StartCoroutine(startSimulatorCt(isFirst));
	}

	int sindex = 0;

	public IEnumerator startSimulatorCt(bool isFirst = false)
	{
		currentSimulatorMatchIsFirst = isFirst;

		isInit = false;
		isPlaying = false;
		playGameOver = false;

		clearStage();

		if(isFirst)
		{
			sindex = 0;
			cutSceneMapManager.visible = false;
			mapManager = normalMapManager;
			gameCameraContainer.SetActive(true);
			tk2dCam.useTargetResolution = false;
			tk2dCam.gameObject.SetActive(false);
			mapManager.isSetStage = false;
		}
		else
		{
			++sindex;
		}
		
		uiManager.uiPlay.init();
		GameManager.me.player.init(GameDataManager.selectedPlayerData, true, true, 0);
		GameManager.me.player.pet.init(player);

		mapManager.clearStage();
		bulletManager.clearStage();
		effectManager.clearStage();
		characterManager.clearStage();
		MethodManager.clearInGameFunc();	

		player.isEnabled = true;
		player.state = Monster.NORMAL;		
		player.resetPosition();
		
		int len = deletePool.childCount;
		for(int i = len-1; i >= 0; --i) UnityEngine.Object.Destroy(deletePool.GetChild(i).gameObject);
		
		GameManager.me.stageManager.heroMonster = null;
		
		stageManager.playTime = 0.0f;
		characterManager.longestWalkedTargetZonePlayerLine.Set( -9999.0f );

		Debug.LogError("simulator isfirst : " + isFirst);

		if(isFirst)
		{
			loadRoundMonsterModelData();
			while(gameDataManager.isCompleteLoadMap == false){ yield return null; };
			while(gameDataManager.isCompleteLoadModel == false){ yield return null; };
			while(SoundManager.instance.isCompleteLoadSound == false){ yield return null; };
			while(effectManager.isCompleteLoadEffect == false){ yield return null; };

			characterManager.startPreLoading();
			while(CharacterManager.isCompletePreloading == false){ yield return null; };
			characterManager.inGameGUITooltipContainer.gameObject.SetActive(false);
		}

		if(BattleSimulator.instance.viewBackground)
		{
			normalMapManager.createBackground(stageManager.getMapId(), true);
			mapManager.visible = true;
		}

		//

		_updateLoopLeftTime = 0.0f;
		GameManager.setTimeScale = 1.0f;
		
		// 화면에 있을 몬스터 캐릭터 세팅.
		mapManager.setStage(stageManager.nowRound);
		
		if(GameManager.me.pvpPlayer != null)
		{
			GameManager.me.pvpPlayer.state = Monster.NORMAL;
		}
		
		uiManager.changeUI(UIManager.Status.UI_PLAY);
		uiManager.uiPlay.showMenu(0.0f);
		uiManager.uiPlay.hideReadyBattleAnimation();	

		int randomeSeed = 4989;
		
		#if UNITY_EDITOR
		if(BattleSimulator.instance.wantSameResult)
		{
			randomeSeed = BattleSimulator.instance.randomSeed;
		}
		else
		{
			randomeSeed = UnityEngine.Random.Range(1000,999999);//Guid.NewGuid().GetHashCode();//UnityEngine.Random.Range(1000,99999);
		}

		BattleSimulator.instance.nowSeed = randomeSeed;

		#else
		randomeSeed = UnityEngine.Random.Range(1000,99999);
		#endif

		inGameRandom = new Well512Random((uint)randomeSeed);
		replayManager.init(randomeSeed);

		GameManager.me.characterManager.updatePlayerFirst = (inGameRandom.Range(0,101) > 50);

		stageManager.playTime = 0.0f;		
		UnitSlot.summonPosIndex = inGameRandom.Range(0,12);
		UIPlayUnitSlot.summonPosIndex = inGameRandom.Range(0,12);
		characterManager.longestWalkedTargetZonePlayerLine.Set( -9999.0f );

		yield return null;

		_currentScene = Scene.STATE.PLAY_BATTLE;

		isPaused = false;
		isInit = true;
		isPlaying = true;



	}





/// ============================================= ///

	public void startQuickRestart()
	{
		if(CutSceneManager.nowOpenCutScene)
		{
			GameManager.me.cutSceneManager.closeOpenCutScene(false);
			GameManager.me.cutSceneManager.status = CutSceneManager.Status.PREPARE;
			GameManager.me.cutSceneManager.close();
		}

		GameManager.me.characterManager.inGameGUITooltipContainer.gameObject.SetActive(false);

		isInit = false;
		isPlaying = false;
		
		GameManager.me.player.isEnabled = false;
		GameManager.me.mapManager.isSetStage = false;		
		
		isInit = false;
		isPlaying = false;
		playGameOver = false;
		
		
		startGame(0.01f);
	}
	



}
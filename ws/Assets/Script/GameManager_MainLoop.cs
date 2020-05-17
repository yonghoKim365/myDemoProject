using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

sealed public partial class GameManager : MonoBehaviour {

	LoopType _loopType = LoopType.inGame;
	
	public enum LoopType
	{
		preview, inGame
	}

	public static Xfloat LOOP_INTERVAL = 0.1f;//0.05f; //0.1f;

	Xfloat currentTime = 0.0f;
	Xfloat _updateLoopLeftTime = 0.0f; 


	public static bool renderSkipFrame = false;


	void Update()
	{
#if UNITY_EDITOR
		if(stageManager.isIntro)
		{
			if(Input.GetMouseButtonDown(2))
			{
				Time.timeScale = 10.0f;
			}
			else if(Input.GetMouseButtonUp(2))
			{
				Time.timeScale = 1.0f;
			}
		}
#endif


//		Debug.Log("timescale: " + Time.timeScale);


		if(_waitForTouchToStartGame && Input.GetMouseButtonUp(0))
		{
			_waitForTouchToStartGame = false;
			closeTitleAndPlayGame();
		}


		if(uiManager.uiLoading.gameObject.activeInHierarchy == true && recordMode != RecordMode.continueGame && _openContinuePopup == false) return;

#if UNITY_EDITOR
		if(Input.GetKeyUp(KeyCode.Q)) Time.timeScale -= 0.1f;
		else if(Input.GetKeyUp(KeyCode.W)) Time.timeScale *= 2.0f;
		else if(Input.GetKeyUp(KeyCode.S)) Log.saveFileLog();
#endif
		if(Time.smoothDeltaTime <= 0.0f || Time.timeScale <= 0)
		{
			globalDeltaTime = 0.0f;
			if(stageManager.isIntro) cutSceneManager.updateFrame();
			return;
		}

		if(uiManager.currentUI != UIManager.Status.UI_PLAY)
		{
			if(uiManager.currentUI == UIManager.Status.UI_MENU)
			{
//				checkAutoLandScape();
			}

			LOOP_INTERVAL.Set( Time.smoothDeltaTime );
			_loopType = LoopType.preview;
			if (Time.frameCount % 500 == 0)  System.GC.Collect();
			if (Time.frameCount % 3600 == 0)  clearMemory();
		}
		else
		{

			if(_isPaused)
			{
				if (Time.frameCount % 30 == 0)  System.GC.Collect();
				return;
			}

			_loopType = LoopType.inGame;


			LOOP_INTERVAL.Set( 0.05f ); ////(1.0f/20.0f); // 초당 20번 연산.

			if(isAutoPlay == false && BattleSimulator.nowSimulation == false)
			{
				player.moveState = Player.MoveState.Stop;
			}

			if(recordMode == RecordMode.record && isPlaying)
			{
				checkPlayerTouchForMove();
			}
		}

		float newTime = currentTime + Time.smoothDeltaTime;//((float)(Mathf.RoundToInt(Time.smoothDeltaTime*100.0f))*0.01f);
		float frameTime = newTime - currentTime; 
		
		if ( frameTime > LOOP_INTERVAL * 10.0f)
		{
			frameTime = LOOP_INTERVAL * 10.0f;
		}
		
		#if UNITY_EDITOR
		if( BattleSimulator.nowSimulation && 
		   BattleSimulator.instance.skipTime > 0 && 
		   BattleSimulator.instance.skipTime > LOOP_INTERVAL) 
		{
			frameTime = BattleSimulator.instance.skipTime;
		}
		#endif
		
		//if(playMode == Play_Mode.REPLAY) frameTime = 10.0f;
		if(recordMode == RecordMode.continueGame)
		{
			frameTime = 30.0f;
		}
		
		currentTime.Set( newTime );
		_updateLoopLeftTime.Set( _updateLoopLeftTime.Get() + frameTime ); 
		
		float useLoopUpdateTime = 0.0f;

		loopIndex = 0;

		while ( _updateLoopLeftTime >= LOOP_INTERVAL)      
		{           
			globalDeltaTime = LOOP_INTERVAL;//Time.smoothDeltaTime;//Time.smoothDeltaTime;	//LOOP_INTERVAL;
			updateLoop();
			globalGamePassTime += globalDeltaTime;    
			useLoopUpdateTime += globalDeltaTime;
			_updateLoopLeftTime.Set( _updateLoopLeftTime - globalDeltaTime );      

			if(stageManager.isIntro)
			{
#if UNITY_EDITOR
				Debug.Log("intro loop : " + _updateLoopLeftTime);
#endif

				cutSceneManager.updateFrame();

				if(Time.timeScale <= 0)
				{
					_updateLoopLeftTime = 0;
				}
			}

			++loopIndex;
		} 

		if(useLoopUpdateTime > 0)
		{
			renderSkipFrame = false;
		}
		else
		{
			renderSkipFrame = (isPlaying && uiManager.currentUI == UIManager.Status.UI_PLAY);
		}

		renderRatio = _updateLoopLeftTime / LOOP_INTERVAL;

		render();

		globalTime = Time.realtimeSinceStartup;
	}


	public static float renderRatio = 0.0f;
	public static int loopIndex = 0;

	void updateLoop()
	{
		switch(_loopType)
		{
		case LoopType.inGame:
			updateInGameLoop();
			break;
		case LoopType.preview:
			updatePreviewLoop();
			break;
		}
	}



	void updatePreviewLoop()
	{
		if(player == null || GameManager.me == null) return;

		if(uiManager.popupSkillPreview.isEnabled && 
		   uiManager.popupSkillPreview.gameObject.activeSelf && 

		   (uiManager.popupShop.gameObject.activeSelf == false || uiManager.popupSkillPreview.isJackpotMode ) )
		{
			characterManager.update();
			player.update(); 		
			bulletManager.update();
			stageManager.playTime += globalDeltaTime;
		}
		else if(uiManager.uiMenu.currentPanel == UIMenu.LOBBY)
		{
			if( uiManager.uiMenu.uiLobby.hero == null) return;

			if( uiManager.uiMenu.uiLobby.gameObject.activeSelf )
			{
				uiManager.uiMenu.uiLobby.hero.update();
				uiManager.uiMenu.uiLobby.hero.render();
				
				for(int i = 0; i < 5; ++i)
				{
					if(uiManager.uiMenu.uiLobby.units[i] != null)
					{
						uiManager.uiMenu.uiLobby.units[i].update();
						uiManager.uiMenu.uiLobby.units[i].render();
					}
				}
			}
		}
	}


	void render()
	{
		characterManager.render();
		mapManager.render();
	}


	public enum RecordMode
	{
		record, replay, continueGame
	}

	public enum PlayMode
	{
		normal, replay
	}


	public RecordMode recordMode = RecordMode.record;

	public PlayMode playMode = PlayMode.normal;

	public Xbool needClearWork = false;

	bool gameEndCheck()
	{
		if(needClearWork)
		{
			int _cs = _currentScene;
			switch(_cs)
			{
			case Scene.STATE.PLAY_CLEAR_SUCCESS:

				if(uiManager.uiLoading.gameObject.activeSelf)
				{
					uiManager.uiPlay.resetCamera();
					uiManager.uiLoading.hide();
				}

				needClearWork = false;
				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation)
				{
					++BattleSimulator.instance.win;
					BattleSimulator.instance.endSimulation();
					_currentScene = Scene.STATE.TITLE;
					return true;
				}
				#endif


				if(HellModeManager.instance.isOpen)
				{
					player.changeShader(false,true);
					player.setColor(new Color(1,1,1,1));


					// 다음 라운드를 진행 할 수 있다면 계속 진행한다.
					if(HellModeManager.instance.checkNextRound())
					{
						uiManager.uiPlay.showHellRoundSuccessAnimation();
					}
					// 진행할 수 없다면 끝! (25 웨이브가 끝이다.)
					else
					{
						player.clearPlayerEffect();
						player.changeShader(false,true);
						player.setColor(new Color(1,1,1,1));

						if(GameManager.replayManager.isPVPReplayIsSurrenderGame == false && recordMode != RecordMode.replay)
						{
							//cutSceneManager.startUnitSkillCamScene("WIN", player.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.ChaserAttack);
							player.state = Monster.WIN;
							player.playAni(Monster.WIN);
							player.renderAniRightNow();
						}
						
						onCompleteRound(WSDefine.GAME_SUCCESS);
					}

					return true;
				}

				// 클리어 컷씬 발동 체크.
				cutSceneManager.roundStateCheck(true);

				player.clearPlayerEffect();
				player.changeShader(false,true);
				player.setColor(new Color(1,1,1,1));

				if(pvpPlayer != null) pvpPlayer.clearPlayerEffect();

				// 컷씬이 발동됐으면 그냥 백단에서 패킷을 임시로 보낸다.
				// 아니면 success 처리를 한다...

				if(CutSceneManager.nowOpenCutScene == false)
				{
					if(GameManager.replayManager.isPVPReplayIsSurrenderGame == false && recordMode != RecordMode.replay)
					{
						cutSceneManager.startUnitSkillCamScene("WIN", player.cTransform.position, UIPlay.SKILL_EFFECT_CAM_TYPE.ChaserAttack, -1);
						GameManager.me.gameCamera.nearClipPlane = 1000.0f;//1200.0f; // 

						player.state = Monster.WIN;
						player.playAni(Monster.WIN);
						player.renderAniRightNow();
					}

					onCompleteRound(WSDefine.GAME_SUCCESS);
				}
				else
				{
					onCompleteRound(WSDefine.GAME_SUCCESS);
				}

				return true;
				break;


			case Scene.STATE.PLAY_CLEAR_FAILED:

				if(uiManager.uiLoading.gameObject.activeSelf)
				{
					uiManager.uiPlay.resetCamera();
					uiManager.uiLoading.hide();
				}

				needClearWork = false;
				
				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation)
				{
					++BattleSimulator.instance.lose;
					//BattleSimulator.log("#GAME: " + BattleSimulator.instance.nowGameNum +  " 패배. 시간: ",stageManager.playTime + " player hp:" + player.hp, "sp:" + player.sp, "mp:"+player.mp + " | pvp hp:" + pvpPlayer.hp, "sp:" + pvpPlayer.sp, "mp:"+pvpPlayer.mp);
					isInit = false;
					isPlaying = false;
					BattleSimulator.instance.endSimulation();
					_currentScene = Scene.STATE.TITLE;
					return true;
				}
				#endif
				
				// 실패할때 컷씬 시작 체크. 그런건 이제 없음.
				cutSceneManager.roundStateCheck();

				player.clearPlayerEffect();
				if(pvpPlayer != null) pvpPlayer.clearPlayerEffect();

				if(CutSceneManager.nowOpenCutScene == false)
				{
					startGameOver();
					return true;
				}
				else
				{
					onCompleteRound(WSDefine.GAME_FAILED);
				}
				
				return true;
				
				break;


			case Scene.STATE.PLAY_CLEAR_DRAW:
				needClearWork = false;
				#if UNITY_EDITOR
				if(BattleSimulator.nowSimulation)
				{
					++BattleSimulator.instance.draw;
					isInit = false;
					isPlaying = false;
					BattleSimulator.instance.endSimulation();
					_currentScene = Scene.STATE.TITLE;
				}
				#endif

				return true;

				break;
	
			}
		}

		return false;
	}


	bool _openContinuePopup = false;
	public float replayPrepareDelay = 2.0f;

	void updateInGameLoop()
	{
		if(_openContinuePopup)
		{
			if(replayPrepareDelay > 0)
			{
				replayPrepareDelay -= RealTime.deltaTime;
				return;
			}

			_openContinuePopup = false;
			uiManager.uiPlay.resetCamera();
			uiManager.uiPlay.onOpenPause();
			uiManager.uiLoading.hide();
			return;
		}

		if(gameEndCheck()) return;

		// 게임 초기화가 안됐으면 업데이트를 하지 않음.
		if(isInit == false) return;

		_openContinuePopup = false;

		// 마우스가 ui 위에 있는지를 체크한다.
		//uiManager.uiOverCheck();

//#if UNITY_EDITOR
//		Log.log(replayManager.nowFrame + "  : " + isPlaying);
//#endif

		if(isPlaying)
		{
			// 녹화 모드일때.
			if(recordMode == RecordMode.record)
			{
				checkControl(true);

				float pTime = stageManager.playTime.Get();

				if( pTime > 2)
				{
					if(replayManager.tempSaveCount == 0)
					{
						++replayManager.tempSaveCount;
						replayManager.tempSave();
					}
					else if((pTime / 30.0f) > replayManager.tempSaveCount)
					{
						++replayManager.tempSaveCount;
						replayManager.tempSave();
					}

				}

			}
			else if(recordMode == RecordMode.replay)
			{
				if(replayManager.getNextRecord() == false)
				{
					_updateLoopLeftTime = 0.0f;
				}
				else
				{
					checkControl(false);
				}

			}
			else if(recordMode == RecordMode.continueGame)
			{
				if(replayManager.getNextRecord() == false)
				{
					_updateLoopLeftTime = 0.0f;
					replayPrepareDelay = 1.0f;
					_openContinuePopup = true;

					uiManager.uiPlay.initAutoPlay();

					return;
				}
				else
				{
					checkControl(false);
				}
			}



			characterManager.updateLineInformation();
		}


		#if UNITY_EDITOR
		//ff Log.log("delta : " + globalDeltaTime);
		#endif

		// 맵 업데이트.
		// 이곳에서 맵 오브젝트와 주인공의 충돌을 검사한다.
		mapManager.update();
		
		// 몬스터 캐릭터 업데이트.
		// 몬스터가 밖으로 나갔는지 여부도 여기에서 검사한다.
		// 몬스터 총알을 여기서 쏜다.
		
		if(currentScene == Scene.STATE.PLAY_BATTLE && cutSceneManager.stopLoopInGame == false && stageManager.playTime.Get() > 0)
		{
			characterManager.update();
			// 주인공 캐릭터 업데이트.	

			if(stageManager.chaser != null) stageManager.chaser.update();

			battleManager.update();

		}
		else if(CutSceneManager.nowOpenCutScene && cutSceneManager.stopLoopInGame == false)
		{
			characterManager.updateDeadMonster();

			if(currentScene == Scene.STATE.PLAY_CLEAR_SUCCESS && cutSceneManager.stopLoopInGame == false && stageManager.playTime.Get() > 0)
			{
				characterManager.updateAnimationDelayMethod(characterManager.monsters);
			}
		}

		
		bulletManager.update();
		
		methodManager.update();
		
		if(_currentScene == Scene.STATE.PLAY_READY)
		{
			player.state = Monster.NORMAL;
		}
		else if(isPlaying)
		{
			float pt = stageManager.playTime + globalDeltaTime;
			stageManager.playTime.Set( Mathf.Round(pt * 100.0f) * 0.01f );

#if UNITY_EDITOR
//			if(CutSceneManager.introCutSceneStartTime > 0) Log.log("stageManager.playTime : " + stageManager.playTime + "   cs : " + CutSceneManager.cutScenePlayTime);

			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				return;
			}

#endif
			if(stageManager.failChecker()) currentScene = Scene.STATE.PLAY_CLEAR_FAILED;
			else stageManager.clearChecker(ClearChecker.CHECK_TIME);

			if(uiManager.uiPlay.warningAlpha.gameObject.activeSelf == false && player.hpPer.Get() <= 0.15f)
			{
				uiManager.uiPlay.warningAlpha.start();
			}
			else if(uiManager.uiPlay.warningAlpha.gameObject.activeSelf && player.hpPer.Get() > 0.15f){
				uiManager.uiPlay.warningAlpha.stop();
			}
		}
	}


	void checkControl(bool saveMode)
	{
		bool hasEvent = false;

#if UNITY_EDITOR
		string fuck = "";
		if(player.moveState == Player.MoveState.Backward) fuck = "LEFT ";
		else if(player.moveState == Player.MoveState.Stop) fuck = "STOP ";
		else if(player.moveState == Player.MoveState.Forward) fuck = "RIGHT ";
#endif

		for(int i = 0; i < 5; ++i)
		{
			if(replayManager.unitButtons[i])
			{
				uiManager.uiPlay.UIUnitSlot[i].isClicked = true;
				hasEvent = true;
#if UNITY_EDITOR
				fuck += " UNIT " + i + "    ";
#endif
			}
		}
		
		for(int i = 0; i < 3; ++i)
		{
			if(replayManager.skillButtons[i])
			{
				uiManager.uiPlay.uiSkillSlot[i].isClicked = true;
#if UNITY_EDITOR
				fuck += " SKILL " + i + "    ";
#endif
				hasEvent = true;
			}
		}

#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation == false)
		{
			Log.logError("====== Auto: " + GameManager.me.isAutoPlay  + "      nowFrame : " + replayManager.nowFrame + "   " + fuck );
		}
#else
//		if(GameManager.isDebugBuild )
//		{
//			Log.logError("====== Auto: " + GameManager.me.isAutoPlay  + "      nowFrame : " + replayManager.nowFrame + "   " + fuck );
//		}
#endif


		if(replayManager.changePlayer)
		{
			GameManager.me.battleManager.clickMyPlayerChange = true;
		}

		if(replayManager.changePVPPlayer)
		{
			GameManager.me.battleManager.clickPVPPlayerChange = true;
		}

		if(saveMode) replayManager.addData(hasEvent, player.moveState);

		replayManager.resetButtonState();

		++replayManager.nowFrame;

		#if UNITY_EDITOR
		if(stageManager.isIntro)
		{
			Debug.LogError("[ replayManager.nowFrame : " + replayManager.nowFrame);
		}
		#endif
	}


	bool _needToFindNewTouchFinger = false;

	Vector3 _playerPosWhenTouchStart = new Vector3();

	const float TOUCH_MOVE_OFFSET = 50.0f;


	// 인트로 컷씬용....
	public void setMouseDown()
	{
		_isMouseDown = true;
		_playerPosWhenTouchStart = player.cTransformPosition;
		_touchPosX = -99999.0f;
	}



	public void setMouseTouchOff()
	{
		_fingerId = -1000;
		_isMouseDown = false;
	}

	
	void checkPlayerTouchForMove()
	{
		//_touchPosX = 0;
	
#if UNITY_EDITOR
		if(BattleSimulator.nowSimulation) return;
#endif

		bool mobile = true;

		if(isAutoPlay) return;

		if(battleManager.waitingForPlayerChange) return;

		if((CutSceneManager.nowOpenCutScene))
		{
			if(cutSceneManager.blockControl) return;
			else mobile = false;
		}


		if(isPlaying && player.isFreeze == false)
		{
			//-- 터치 관련 코드.--//		
			//guiLog = "";		

#if UNITY_EDITOR || UNITY_WEBPLAYER	
			mobile = false;
#elif UNITY_IPHONE || UNITY_ANDROID 

#endif

			if(mobile)
			{
				_needToFindNewTouchFinger = false;
				if(Input.touchCount > 0)
				{
					for(int i = 0; i < Input.touchCount; ++i)
					{
						Touch t = Input.GetTouch(i);
						
						if(t.phase == TouchPhase.Began) 
						{
							if(uiOverCheck(t.position) == false) 
							{
								_isMouseDown = true;	
								_fingerId = t.fingerId;
								_playerPosWhenTouchStart = player.cTransformPosition;
								_touchPosX = -99999.0f;
								break;
							}
						}
						else if(t.phase == TouchPhase.Ended) 
						{
							_isMouseDown = false;
							
							for(int j = 0; j < Input.touchCount; ++j)
							{
								Touch t1 = Input.GetTouch(j);
								
								if(t.fingerId != t1.fingerId)
								{
									if(uiOverCheck(t1.position) == false) 
									{
										_isMouseDown = true;
										_fingerId = t1.fingerId;
										_playerPosWhenTouchStart = player.cTransformPosition;
										_touchPosX = -99999.0f;
										break;
									}
								}
							}	
							
							
							
							break;
						}
						else if(_isMouseDown && t.fingerId == _fingerId)
						{
							//_touchPosX = t.position.x;  // tk2dGameCamera.resolutionScale;
							
							if(MathUtil.abs(_touchPosX, t.position.x) > TOUCH_MOVE_OFFSET)
							{
								_touchPosX = t.position.x;
								_playerPosWhenTouchStart = player.cTransformPosition;
							}
							
							break;
						}
					}
				}
				else
				{
					_fingerId = -1000;
					_isMouseDown = false;
				}
			}
			else
			{
				if(Input.GetMouseButtonDown(0))
				{
					if(uiOverCheck() == false)
					{
						_isMouseDown = true;
						_playerPosWhenTouchStart = player.cTransformPosition;
						_touchPosX = -99999.0f;
					}
				}
				else if(Input.GetMouseButtonUp(0))
				{
					_isMouseDown = false;
				}
				
				if(_isMouseDown && Input.GetMouseButton(0))
				{
					if(MathUtil.abs(_touchPosX, Input.mousePosition.x) > TOUCH_MOVE_OFFSET)
					{
						_touchPosX = Input.mousePosition.x;
						_playerPosWhenTouchStart = player.cTransformPosition;
					}
					
					//Debug.Log(_touchPosX);
					
					//_touchPosX = Input.mousePosition.x;// / tk2dGameCamera.resolutionScale;
				}
			}








			if(_isMouseDown && _touchPosX > -9999.0f)
			{
				_v = gameCamera.WorldToScreenPoint(_playerPosWhenTouchStart);//player.cTransformPosition);

				if(_touchPosX < _v.x - 15)
				{
					player.moveState = Player.MoveState.Backward;
				}
				else if(_touchPosX > _v.x + 15)
				{
					player.moveState = Player.MoveState.Forward;
				}
			}


			#if UNITY_EDITOR
			
			if(Input.GetKey(KeyCode.LeftArrow))
			{
				player.moveState = Player.MoveState.Backward;
			}
			else if(Input.GetKey(KeyCode.RightArrow))
			{
				player.moveState = Player.MoveState.Forward;
			}
			#endif
			
			//-- 터치 관련 코드 끝.--//				
		}
	}

	


	
}

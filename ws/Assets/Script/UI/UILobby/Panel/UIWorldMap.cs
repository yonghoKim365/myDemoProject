using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public partial class UIWorldMap : UIBase {

	public UIWorldMapStageClearRewardPopup stageClearRewardPopup;
	public UIDebugRoundList debugRoundList;
	public UIButton btnDebug;

	public UIDebugTestModeList debugTestModeList;

	public UILobbyStageDetailPanel stageDetail;	

	public UISprite spChampionship, spHell, spInstantDungeon;
	public UIButton btnStartEpicStage, btnShop, btnChampionship, btnHell, btnInstantDungeon;

	public UISprite spChampionshipMarkResult, spChampionshipDormancy, spChampionshipExclamation;

	public UISprite spHasTutorialForHell, spHasTutorialForChampionship;


	public GameObject goWorldMapContainer;
	public Camera camWorldCamera;

	public UIButton btnDrag;

	public UIWorldMapFriendButton friendButton;

	public UIWorldMapFriendList friendList;

	public UIWorldMapFriendDetail friendDetailButton;


	const float MAP_CAMERA_START_X = 0.0f;
	const float MAP_CAMERA_END_X = 2642.0f;//4530.0f;

	Vector3 _v;
	Quaternion _q = new Quaternion();

	public UIWorldMapRoad mapRoad;

	public Player mapPlayer = null;
	public Monster mapHeroMonster = null;

	public Transform mapPlayerContainer;


	public WorldMapEffectManager mapEffectManager;


	void Awake()
	{
		setBackButton(UIMenu.LOBBY);
		UIEventListener.Get(btnStartEpicStage.gameObject).onClick = onStartGame;
		UIEventListener.Get(btnChampionship.gameObject).onClick = onClickChampionship;
		UIEventListener.Get(btnHell.gameObject).onClick = onClickHell;
		UIEventListener.Get(btnInstantDungeon.gameObject).onClick = onClickInstantDungeon;


		UIEventListener.Get(btnShop.gameObject).onClick = onOpenShop;


		UIEventListener.Get(btnStartEpicStage.gameObject).onPress = onPressStartGame;
		UIEventListener.Get(btnChampionship.gameObject).onPress = onPressChampionship;
		UIEventListener.Get(btnHell.gameObject).onPress = onPressHell;
		
		UIEventListener.Get(btnShop.gameObject).onPress = onPressOpenShop;



		UIEventListener.Get(btnDrag.gameObject).onPress = OnPress;
		UIEventListener.Get(btnDrag.gameObject).onDrag = onDragStage;

		UIEventListener.Get(btnDebug.gameObject).onClick = onClickDebugButton;
	}


	void onPressStartGame(GameObject go, bool state)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(state)
		{
			SoundData.play("uibt_gamemode");
		}
	}

	void onPressChampionship(GameObject go, bool state)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(state)
		{
			SoundData.play("uibt_gamemode");
		}
	}

	void onPressHell(GameObject go, bool state)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(state)
		{
			SoundData.play("uibt_gamemode");
		}
	}

	void onPressOpenShop(GameObject go, bool state)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(state)
		{
			SoundData.play("uibt_shop");
		}
	}


	public override void onClickBackToMainMenu (GameObject go)
	{
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		//if(GameManager.me.uiManager.popupcha
		base.onClickBackToMainMenu (go);
	}


	void onClickDebugButton(GameObject go)
	{
		if(debugRoundList.gameObject.activeSelf)
		{
			debugRoundList.gameObject.SetActive(false);
		}
		else
		{
			debugRoundList.gameObject.SetActive(true);
			debugRoundList.draw(false);
		}
	}


	public void updateChampionshipStatus()
	{
		spChampionshipDormancy.gameObject.SetActive(GameDataManager.instance.champMemberId == -1 && TutorialManager.instance.clearCheck("T24"));
	}


	public void onClickChampionship(GameObject go)
	{
		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
		#endif

		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;



		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;



		if(GameDataManager.instance.roundClearStatusCheck(1,4,5) == false)
		{
//			UISystemPopup.open(UISystemPopup.PopupType.Default, "4스테이지를 클리어하면 입장하실 수 있습니다.");
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("CHAMP_JOIN_CNDT"));
			return;
		}




		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;



		if((GameDataManager.instance.championshipStatus != WSDefine.CHAMPIONSHIP_OPEN))
		{
			DateTime checkTime = System.DateTime.Now;
			
			if(checkTime.DayOfWeek == DayOfWeek.Monday && (checkTime.Hour >= 12))
			{
				
			}
			else
			{
//				UISystemPopup.open("준비 중입니다.");
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("CHAMP_WAITING"));
				return;
			}
		}

		GameManager.me.uiManager.popupChampionship.show();
	}







	public void onClickInstantDungeon(GameObject go)
	{
		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
		#endif

		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;
		
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;


		if(GameDataManager.instance.maxAct <= 2)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("SIGONG_CONDITION"));
			return;
		}


		GameManager.me.uiManager.popupInstantDungeon.open(true);
	}



	public void onClickHell(GameObject go)
	{

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
#endif

		if(GameManager.me.uiManager.popupSummonDetail.gameObject.activeSelf) return;

		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;

		if(GameDataManager.instance.roundClearStatusCheck(1,3,4) == false)
		{
//			UISystemPopup.open(UISystemPopup.PopupType.Default, "3스테이지를 클리어하면 입장하실 수 있습니다.");
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("HELL_JOIN_CNDT"));

			return;
		}

		if((GameDataManager.instance.championshipStatus != WSDefine.CHAMPIONSHIP_OPEN))
		{
			DateTime checkTime = System.DateTime.Now;

			if(checkTime.DayOfWeek == DayOfWeek.Monday && (checkTime.Hour >= 12))
			{

			}
			else
			{
//				UISystemPopup.open(UISystemPopup.PopupType.Default, "순위를 집계중입니다.");
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("HELL_WAITING"));
				return;
			}
		}

		GameManager.me.uiManager.popupHell.show();
	}



	void onOpenShop(GameObject go)
	{

		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;

		GameManager.me.uiManager.popupShop.showSpecialShop();
	}


	public void onStartGame(GameObject go)
	{

		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;
		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;

		debugRoundList.gameObject.SetActive(false);

		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			stageDetail.open (1, 1, 1, true);
		}
		else
#endif
		{
			if(GameDataManager.instance.maxAct > GameManager.MAX_ACT)
			{
				if(TutorialManager.instance.isTutorialMode == false)
				{
					GameManager.me.uiManager.popupUpdateWatingNotice.show();
				}
				return;
			}

			stageDetail.open (GameDataManager.instance.maxActWithCheckingMaxAct, GameDataManager.instance.maxStage, GameDataManager.instance.maxRound, true);
		}

	}


	public void openEpicPopup(bool selectCurrentRound, int selectAct = 1, int selectStage = 1, int selectRound = 1)
	{
		if(GameManager.me.uiManager.uiLoading.gameObject.activeSelf) return;

		if(nowPlayingWalkAnimation) return;
		if(stageClearRewardPopup.gameObject.activeSelf) return;



		if(selectCurrentRound)
		{
			stageDetail.open (GameDataManager.instance.maxActWithCheckingMaxAct, GameDataManager.instance.currentStageWithCheckingMaxAct, GameDataManager.instance.maxRound, true);
		}
		else
		{
			stageDetail.open (selectAct, selectStage, selectRound, true);
		}
	}




	void onCompleteHideMapHeroMonster()
	{
		if(mapHeroMonster != null)
		{
			GameManager.me.characterManager.cleanMonster(mapHeroMonster);
			GameManager.me.characterManager.nowWorldMapMonsterHero = "";
			mapHeroMonster = null;
		}
	}

	void hideHeroMonster()
	{
		if(mapHeroMonster != null)
		{
			iTween.ScaleTo(mapHeroMonster.cTransform.gameObject, iTween.Hash(
				//"path", getPath(targetAct,targetRound,10.0f).ToArray(),	
				"time", 0.8f,
				"x",0.01f,
				"y",0.01f,
				"z",0.01f,
				"islocal",true,
				"easetype", iTween.EaseType.easeInCubic,
				"oncomplete","onCompleteHideMapHeroMonster",
				"oncompletetarget",this.gameObject
				) );
			
			//					GameManager.me.characterManager.cleanMonster(mapHeroMonster);
			GameManager.me.characterManager.nowWorldMapMonsterHero = "";
			//					mapHeroMonster = null;
		}
	}


	public bool nowPlayingWalkAnimation = false;
	IEnumerator startWalk(int targetAct, int targetStage)
	{
		if(mapPlayer != null)
		{
			if(targetStage == 1)
			{
				createMapMonster(false, true);
			}
			else if(targetStage == 5)
			{
				hideHeroMonster();
			}

			nowPlayingWalkAnimation = true;

//			yield return new WaitForSeconds(0.5f);

			if(mapPlayer.ani.GetClip(Monster.MWALK))
			{
				mapPlayer.ani[Monster.MWALK].speed = 1.0f;
			}
			
			mapPlayer.ani.CrossFade(Monster.MWALK, 0.2f);

			Vector3[] path = iTweenPath.GetPath("a"+targetAct + "s"+(targetStage));
			if(path == null)
			{
				onCompletePathAnimation();
				yield break;
			}

			iTween.MoveTo(mapPlayer.cTransform.gameObject, 
			              iTween.Hash(
				//"path", getPath(targetAct,targetRound,10.0f).ToArray(),	
				"path", path,
				"time", 1f,
				//"islocal",true,
				"easetype", iTween.EaseType.linear,
				//"movetopath", false,
				"movetopath", true,
				"oncomplete","onCompletePathAnimation",
				"oncompletetarget",this.gameObject
				) );
		}
	}





	public void onCompletePathAnimation()
	{
		GameManager.setTimeScale = 1.0f;

		mapPlayer.cTransform.position = getCurrentPosition(GameDataManager.instance.maxAct, GameDataManager.instance.maxStage);
		mapPlayer.ani.Play("idle");
		checkTutorialStart();
		nowPlayingWalkAnimation = false;
		System.GC.Collect();

	}





	public void checkTutorialStart(bool checkSpecialPackage = true)
	{
		if(GameDataManager.instance.roundClearStatusCheck(1,4,4))
		{
			spChampionship.color = btnChampionship.defaultColor;
			spChampionshipExclamation.enabled = true;
			spChampionshipDormancy.enabled = true;
		}
		else
		{
			spChampionship.color = btnChampionship.disabledColor;
			spChampionshipExclamation.enabled = false;
			spChampionshipDormancy.enabled = false;
		}

		if(GameDataManager.instance.roundClearStatusCheck(1,3,4))
		{
			spHell.color = btnHell.defaultColor;
		}
		else
		{
			spHell.color = btnHell.disabledColor;
		}

		spHasTutorialForHell.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,3,4) && TutorialManager.instance.clearCheck("T51") == false);
		spHasTutorialForChampionship.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,4,4) && TutorialManager.instance.clearCheck("T24") == false);


		if(GameManager.me.uiManager.uiMenu.currentPanel != UIMenu.WORLD_MAP) return;

		if(GameManager.me.uiManager.popupRetry.gameObject.activeSelf || 
		   GameManager.me.uiManager.popupShop.gameObject.activeSelf || 
		   GameManager.me.uiManager.popupChampionship.gameObject.activeSelf || 
		   stageDetail.gameObject.activeSelf 
		   ) return;


		if(GameManager.me.uiManager.popupSpecialPack.gameObject.activeSelf || GameManager.me.uiManager.popupSpecialSinglePack.gameObject.activeSelf )
		{
			return;
		}

		// 1.1.3 [액트1 스테이지1 라운드3] 이상 클리어 & 월드맵 페이지 & 스테이지 이동 연출 완료 후
		if(TutorialManager.instance.check("T45")) 
		{

		}
		// 1.4.2. [액트1 스테이지4 라운드2] 이상 클리어 & <히어로 페이지> 튜토리얼 완료 & 월드맵페이지 & 가방 2칸 여유
		else if(TutorialManager.instance.check("T46")) 
		{

		}
		else if(checkSpecialPackage)
		{
			GameManager.me.specialPackageManager.check();
		}
	}



	public void startNextStageAnimation()
	{
		int prevStage = GameDataManager.instance.maxStage - 1;
		int prevAct = GameDataManager.instance.maxActWithCheckingMaxAct;
		
		if(prevStage < 1)
		{
			--prevAct;
			prevStage = 5;
		}
		
		mapPlayer.cTransform.position = getCurrentPosition(prevAct, prevStage);


		if(prevStage == 4)
		{
			createMapMonster(true);
		}


		if(string.IsNullOrEmpty(GameDataManager.instance.stageClearRewardItem) == false )
		{
			playStageClearRewardItemScene();
		}
		else
		{
			startNextStageAnimationStep2();
		}



	}

	void playStageClearRewardItemScene()
	{
		stageClearRewardPopup.show();
	}




	// 클리어 애니메이션이 이루어지는 곳.
	private bool _isActClearScene = false;
	public void startNextStageAnimationStep2()
	{
		if(GameManager.me.stageManager.nowPlayingGameType != GameType.Mode.Epic) return;

		int prevStage = GameDataManager.instance.maxStage - 1;
		int prevAct = GameDataManager.instance.maxActWithCheckingMaxAct;
		
		if(prevStage < 1 )	
		{
			--prevAct;
			prevStage = 5;
		}



		if(prevStage == 5 && GameDataManager.instance.maxActWithCheckingMaxAct-2 >= 0)
		{
			setCameraPositionByAct(prevAct);
			// 맵이 열리는 애니메이션도 한다.
			_isActClearScene = true;

			mapRoad.refresh(prevAct, prevStage, UIWorldMapRoad.RefreshType.ActOpen);

			mapEffectManager.spCurrentActLock = mapRoad.spActLockCover[GameDataManager.instance.maxActWithCheckingMaxAct-2];
			mapRoad.spActLock[GameDataManager.instance.maxActWithCheckingMaxAct-2].gameObject.SetActive(false);

			mapEffectManager.play(true);



			Hashtable hash = new Hashtable();
			hash.Add("islocal", true);
			hash.Add("time", 0.8f);
			hash.Add("easetype", iTween.EaseType.linear);

			Vector3 pos = camWorldCamera.transform.localPosition;


			// 새 액트가 열릴때 그 액트를 화면 중앙으로 옮기는데 그때의 좌표다.
			switch(prevAct)
			{
			case 1:
				pos.x = 256;
				break;
			case 2:
				pos.x = 731;
				break;
			case 3:
				pos.x = 1268;
				break;
			case 4:
				pos.x = 1868;
				break;
			case 5:
				pos.x = 2353;//2442;
				break;
				/*
			case 6:
				pos.x = 2379;
				break;
			case 7:
				
				break;

			case 8:
				
				break;

			case 9:
				
				break;
				*/

			}

			if(prevAct < GameManager.MAX_ACT)
			{
				hash.Add("position", pos);
				iTween.MoveTo(camWorldCamera.gameObject, hash);
			}

		}
		else
		{
			setCameraPositionByAct(prevAct); // 이건 테스트용..

			mapRoad.refresh(prevAct, prevStage, UIWorldMapRoad.RefreshType.StageOpen);

			// 단순히 걷기만 한다.
			_isActClearScene = false;
			mapEffectManager.play(false);
		}
	}



	public void onCompleteFSMEffect()
	{
		StartCoroutine( startWalk(GameDataManager.instance.maxActWithCheckingMaxAct, GameDataManager.instance.maxStage) );
	}


	public void refresh(bool checkTutorialOnly = false, bool stageAnimation = false)
	{
		#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
#endif

//		Debug.LogError("refresh : " + checkTutorialOnly);

		if(friendList.gameObject.activeInHierarchy) friendList.listGrid.itemClear();
		friendList.gameObject.SetActive(false);
		friendDetailButton.gameObject.SetActive(false);

		if(checkTutorialOnly == false)
		{
			mapPlayer.init(GameDataManager.instance.heroes[GameDataManager.instance.selectHeroId],true,false);
			_q.eulerAngles = new Vector3(0,0,0);
			mapPlayer.cTransform.localRotation = _q;
			_q.eulerAngles = new Vector3(0,145,0);
			mapPlayer.tf.localRotation = _q;
			mapPlayer.tf.localPosition = new Vector3(0,0,-50);


			// 현재 위치. 여기서는 애니메이션이 된다는 것을 가정하지 않는다.
			mapRoad.refresh(GameDataManager.instance.maxAct, GameDataManager.instance.maxStage, UIWorldMapRoad.RefreshType.Refresh);

			mapPlayer.cTransform.position = getCurrentPosition(GameDataManager.instance.maxAct, GameDataManager.instance.maxStage);

			if(mapPlayer.ani.GetClip(Monster.MWALK) == true)
			{
				mapPlayer.ani[Monster.MWALK].time = 0.0f;
				mapPlayer.ani[Monster.MWALK].speed = 0.0f;
				mapPlayer.ani.Play(Monster.MWALK);
			}

			mapPlayer.ani.Play("idle");
			//mapPlayer.ani.Play("memo0"+UnityEngine.Random.Range(1,3));
		}
		else if(stageAnimation == false)
		{
			onCompletePathAnimation();
		}

		setCameraPositionByAct(GameDataManager.instance.maxActWithCheckingMaxAct);
	}


	void setCameraPositionByAct(int act)
	{
//		Debug.Log("=== setCameraPositionByAct : " + act);

		_v = camWorldCamera.transform.localPosition;

		// 월드맵에 들어갔을때 현재 액트에 따라 카메라 위치를 수정한다.
		switch(act)
		{
		case 1:
			_v.x = 0;
			break;
		case 2:
			_v.x = 401;
			break;
		case 3:
			_v.x = 838;
			break;
		case 4:
			_v.x = 1375;
			break;
		case 5:
			_v.x = 2017;
			break;
		case 6:
			_v.x = 2334;
			break;
			/*
		case 7:
			
			break;

		case 8:
			
			break;

		case 9:
			
			break;
*/


		}
		
		camWorldCamera.transform.localPosition = _v;
	}




	public override void show ()
	{
		base.show ();

		if(TutorialManager.isTutorialOpen("T48") || TutorialManager.isTutorialOpen("T49") || TutorialManager.isTutorialOpen("T50"))
		{

		}
		else
		{
			stageDetail.hide();
		}

		stageClearRewardPopup.hide();

		nowPlayingWalkAnimation = false;

		GameManager.setTimeScale = 1.0f;

		createMapPlayer();

		if(GameDataManager.instance.maxAct >= 3)
		{
			spInstantDungeon.color = Color.white;
		}
		else
		{
			spInstantDungeon.color = btnHell.disabledColor;
		}

		if(GameManager.me.stageManager.isRepeatGame == false && GameManager.me.stageManager.playStage == 5 && GameDataManager.instance.maxStage == 1)
		{

		}
		else
		{
			createMapMonster();
		}

		if(GameManager.me.successType.Get() != null && 
		   GameManager.me.successType == WSDefine.GAME_SUCCESS && 
		   DebugManager.useTestRound == false && 
		   GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic)
		{
		}
		else
		{
			refresh();
		}

		showFriends();
	}


	void createMapPlayer()
	{
		if(mapPlayer != null && mapPlayer.playerData.id != GameDataManager.instance.selectHeroId)
		{
			GameManager.me.characterManager.cleanMonster(mapPlayer);
			mapPlayer = null;
		}
		
		if(mapPlayer == null)
		{
			mapPlayer = GameManager.me.getCurrentPlayer(false,false);

#if UNITY_EDITOR
			mapPlayer.container.name = "MAPPLAYER";
#endif
			mapPlayer.setParent(mapPlayerContainer);
			
			_q.eulerAngles = new Vector3(0,0,0);
			mapPlayer.cTransform.localRotation = _q;
			_q.eulerAngles = new Vector3(0,145,0);
			mapPlayer.tf.localRotation = _q;
			mapPlayer.tf.localPosition = new Vector3(0,0,-50);
		}
	}




	void createMapMonster(bool mustCreate = false, bool useScaleTween = false)
	{
		//act 1 stage 5 - 파멸의 스켈레톤 킹 //mob_skeletonking02
		//act 2 stage 5 - 좀비 오우거 // mob_zombieogre01
		// act 3 stage 5 - 블러드 데몬 // mob_blooddemon01
		// act 4 stage 5 - 미노타우로스  // mob_minotaur01


		float size = 1.0f;

		string nowTargetMonster = "";
		
		if(GameDataManager.instance.maxStage < 5 || mustCreate)
		{
			switch(GameDataManager.instance.maxAct)
			{
			case 1:
				nowTargetMonster = "MOB_SKELETONKING02";
				size = 0.5f;
				//0.5f
				break;
			case 2:
				nowTargetMonster = "MOB_BOSS_ZOMBIEOGRE01";
				size = 0.5f;
				break;
			case 3:
				nowTargetMonster = "MOB_BOSS_DEMON";
				size = 0.36f;
				//0.36
				break;
			case 4:
				nowTargetMonster = "MOB_MINOTAUR01";
				size = 0.7f;
				break;
			case 5:
				nowTargetMonster = "MOB_BOSS_IBRAM01";
				size = 0.7f;
				break;
			case 6:
				nowTargetMonster = "MOB_BOSS_ARCDAM02";
				size = 0.6f;
				break;
			case 7:
				break;
			}
		}
		
		
		if( mapHeroMonster != null)
		{
			if(string.IsNullOrEmpty(nowTargetMonster) ||  nowTargetMonster != GameManager.me.characterManager.nowWorldMapMonsterHero)
			{
				GameManager.me.characterManager.cleanMonster(mapHeroMonster);
				mapHeroMonster = null;
				GameManager.me.characterManager.nowWorldMapMonsterHero = "";
			}
		}
		
		if(string.IsNullOrEmpty(nowTargetMonster) == false)
		{
			GameManager.me.characterManager.nowWorldMapMonsterHero = nowTargetMonster;
		}
		

//		Debug.Log("mapHeroMonster : " + (mapHeroMonster == null));
//		Debug.Log("GameManager.me.characterManager.nowWorldMapMonsterHero : " + (GameManager.me.characterManager.nowWorldMapMonsterHero));

		if(mapHeroMonster == null && string.IsNullOrEmpty(GameManager.me.characterManager.nowWorldMapMonsterHero) == false)
		{
			StartCoroutine(getMonster(GameManager.me.characterManager.nowWorldMapMonsterHero, size));
		}
	}





	IEnumerator getMonster(string monsterDataId, float size)
	{
		while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

		GameDataManager.instance.addLoadModelData( GameManager.info.monsterData[monsterDataId].resource, null, null );

		GameDataManager.instance.startModelLoad();

		while(GameDataManager.instance.isCompleteLoadModel == false) { yield return null; } ;

		mapHeroMonster = GameManager.me.characterManager.getMonster(false,false, monsterDataId ,false);

		mapHeroMonster.setParent(mapPlayerContainer);

		_q.eulerAngles = new Vector3(0,0,0);
		mapHeroMonster.cTransform.localRotation = _q;
		_q.eulerAngles = new Vector3(0,180,0);
		mapHeroMonster.tf.localRotation = _q;
		mapHeroMonster.tf.localPosition = new Vector3(0,0,-50);

		mapHeroMonster.cTransform.localScale = new Vector3(size, size, size);

		mapHeroMonster.cTransform.position = getCurrentPosition(GameDataManager.instance.maxActWithCheckingMaxAct, 5);

		mapHeroMonster.isEnabled = true;

		if(mapHeroMonster.ani.GetClip("memo") != null)
		{
			mapHeroMonster.ani.GetClip("memo").wrapMode = WrapMode.Loop;
			mapHeroMonster.ani.Play("memo");
		}

	}





	List<UIWorldMapFriendButton> friends = null;


	IEnumerator showFriendsCT()
	{
		if(GameDataManager.instance.friendDataByActRound != null && gameObject.activeInHierarchy)
		{
			if(friends == null)
			{
				friends = new List<UIWorldMapFriendButton>();
				
				for(int i = 1; i <= GameManager.MAX_ACT + 1; ++i)
				{
					for(int j = 1; j <= 5; ++j)
					{
						int count = GameDataManager.instance.friendDataByActRound[i][j].Count;
						
						if(count > 0)
						{
							UIWorldMapFriendButton fb = Instantiate(friendButton) as UIWorldMapFriendButton;
							friends.Add(fb);
							fb.transform.parent = friendButton.transform.parent;
							fb.gameObject.SetActive(true);
							fb.name = i + " - " + j + " c" + count;
							fb.setData(GameDataManager.instance.friendDataByActRound[i][j]);
						}
					}
					yield return null;
				}
			}
			else
			{
				if(GameManager.me.uiManager.uiMenu.uiFriend.didRefreshFriendList)
				{
					GameManager.me.uiManager.uiMenu.uiFriend.didRefreshFriendList = false;
					
					int fbLen = friends.Count;
					int fbIndex = 0;
					
					for(int i = 1; i <= GameManager.MAX_ACT + 1; ++i)
					{
						for(int j = 1; j <= 5; ++j)
						{
							int count = GameDataManager.instance.friendDataByActRound[i][j].Count;
							
							if(count > 0)
							{
								UIWorldMapFriendButton fb; 
								
								if(fbIndex < fbLen)
								{
									fb = friends[fbIndex];
									++fbIndex;
								}
								else
								{
									fb = Instantiate(friendButton) as UIWorldMapFriendButton;
									friends.Add(fb);
									fb.transform.parent = friendButton.transform.parent;
								}
								
								fb.gameObject.SetActive(true);
								fb.name = i + " - " + j + " c" + count;
								fb.setData(GameDataManager.instance.friendDataByActRound[i][j]);
							}
						}
						yield return null;
					}
					
					for(int i = fbIndex; i < fbLen; ++i)
					{
						friends[i].gameObject.SetActive(false);
					}
				}
				else
				{
					for(int i = friends.Count - 1; i >= 0; --i)
					{
						for(int j = friends[i].data.Count - 1; j >= 0; --j)
						{
							if(GameDataManager.instance.friendDatas.ContainsKey(friends[i].data[j].userId))
							{
								friends[i].data[j] = GameDataManager.instance.friendDatas[friends[i].data[j].userId];
							}
						}

						if(i % 5 == 0) yield return null;

						friends[i].refreshInfo();
						friends[i].refreshPhoto();
					}
				}
			}
		}

	}

	void showFriends()
	{
		if(GameDataManager.instance.friendDataByActRound != null && gameObject.activeInHierarchy)
		{
			StartCoroutine(showFriendsCT());
		}
	}


	public void refreshFriendInfo()
	{
		if(gameObject.activeSelf == false) return;
		for(int i = friends.Count -1; i >= 0; --i)
		{
			friends[i].refreshInfo();
		}
	}


	public void refreshFriendPhoto()
	{
		if(PhotoDownLoader.destroyOverLimitPhoto && friends != null)
		{
			for(int i = friends.Count -1; i >= 0; --i)
			{
				friends[i].refreshPhoto();
			}

			PhotoDownLoader.destroyOverLimitPhoto = false;
		}
	}



	public void onClickStageButton(GameObject go)
	{

	}


	
	public override void hide ()
	{
		base.hide ();		
		stageDetail.hide();
	}
	



	void OnEnable()
	{
		if(mapPlayer != null) mapPlayer.ani.Play("idle");

		if(mapHeroMonster != null)
		{
			if(mapHeroMonster.ani.GetClip("memo") != null)
			{
				mapHeroMonster.ani.GetClip("memo").wrapMode = WrapMode.Loop;
				mapHeroMonster.ani.Play("memo");
			}

		}

		StartCoroutine(clearMemory());
	}


	WaitForSeconds ws1s = new WaitForSeconds(3.0f);

	IEnumerator clearMemory()
	{
		yield return Util.ws1;

		if(mapPlayer != null)
		{
			mapPlayer.ani.Play("idle");
			//mapPlayer.ani.Play("memo0"+UnityEngine.Random.Range(1,3));
		}


		GameManager.me.clearMemory();

		int i = 0;

		while(true)
		{
			yield return ws1s;
			System.GC.Collect();
		}
	}


	void OnDisable()
	{
//		Debug.Log("onDisable");

		if(mapPlayer != null && mapPlayer.cTransform != null)
		{
			iTween.Stop(mapPlayer.cTransform.gameObject);
		}
	}










}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class TutorialManager : MonoBehaviour {

	public string prevReceivedRewardTutorialId = "";

	public string readyTutorialId = "";
	public bool isReadyTutorialMode = false;

	private bool _isTutorialMode = false;
	public bool isTutorialMode
	{
		get
		{
			return _isTutorialMode;
		}
		set
		{
			_isTutorialMode = value;
			if(value == false)
			{
				if(GameManager.me != null && GameManager.me.uiManager)
				{
					GameManager.me.uiManager.uiTutorial.camera.depth = 13.0f;
				}
			}
		}
	}

	[HideInInspector] public bool isTutorialSkipCheckMode = false;

	public string nowTutorialId = "";
	public int subStep = 0;

	public bool useTutorialDebugMode = false;

	public string checkTutorialId = "";

	public static TutorialManager instance = null;


	public static bool isTutorialOpen(string tId)
	{
		if(TutorialManager.instance.isTutorialMode && (TutorialManager.instance.nowTutorialId == tId || TutorialManager.instance.readyTutorialId == tId ))
		{
			return true;
		}
		
		return false;
	}


	public static bool nowPlayingTutorial(string tId, int subStep = -1)
	{
		if(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == tId)
		{
			if(subStep < 0) return true;
			else return TutorialManager.instance.subStep == subStep;
		}

		return false;
	}

	void Awake()
	{
		if(instance != null)
		{
			instance = null;
		}

		instance = this;
		/*
		if(instance == null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
		*/

		nowTutorialId = "";
		waitStartBattle = false;
		isTutorialSkipCheckMode = false;
		isStart = false;

	}


	void OnDestroy()
	{
		instance = null;
	}



	public static bool waitStartBattle = false;

	IEnumerator minmapPointerVisibleAnimation(int startSubStep, int endSubStep, string target = null)
	{
		bool visible = false;

		while(subStep >= startSubStep && subStep <= endSubStep)
		{
			yield return new WaitForSeconds(0.3f);

			visible = !visible;

			foreach(CharacterMinimapPointer mp in GameManager.me.characterManager.miniMapPointers)
			{
				if(target != null && target != mp.type) continue;

				mp.visible = visible;
			}

			if(visible) yield return new WaitForSeconds(0.5f);
		}

		foreach(CharacterMinimapPointer mp in GameManager.me.characterManager.miniMapPointers)
		{
			mp.visible = true;
		}
	}


	IEnumerator uiVisibleAnimation(int startSubStep, int endSubStep, GameObject go)
	{
		while(subStep >= startSubStep && subStep <= endSubStep)
		{
			yield return new WaitForSeconds(0.3f);

			go.SetActive(!go.activeSelf);

			if(go.activeSelf)
			{
				yield return new WaitForSeconds(0.5f);
			}
		}

		go.SetActive(true);
	}





	Dictionary<string, int> _tutorialReward = new Dictionary<string, int>();

	public void onCompleteGetReward(int getGold, int getRuby, int getEnergy)
	{
		_tutorialReward.Clear();

//		if(getExp > 0) _tutorialReward.Add(WSDefine.EXP, getExp);
		if(getGold > 0)  _tutorialReward.Add(WSDefine.GOLD, getGold);
		if(getRuby > 0)  _tutorialReward.Add(WSDefine.RUBY, getRuby);
		if(getEnergy > 0)  _tutorialReward.Add(WSDefine.ENERGY, getEnergy);
		uiTutorial.rewardPopup.open(_tutorialReward);
	}


	public bool clearCheck(string id) { return (GameDataManager.instance.completeTutorial == WSDefine.YES|| (GameDataManager.instance.tutorialHistory.ContainsKey(id)) ); }


	public void roundCheck(string mode)
	{

#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) GameDataManager.instance.completeTutorial = (useTutorialDebugMode == false)?WSDefine.YES:WSDefine.NO;
#endif
		if(GameDataManager.instance.completeTutorial == WSDefine.YES) return; 

		if(GameManager.me.stageManager.nowPlayingGameType == GameType.Mode.Epic && HellModeManager.instance.isOpen == false)
		{
			switch(mode)
			{
			case RoundData.MODE.KILLEMALL: if(check ("T2")) return; break;
			case RoundData.MODE.SURVIVAL: if(check("T3")) return; break;
			case RoundData.MODE.PROTECT: if(check ("T4")) return; break;
			case RoundData.MODE.SNIPING: if(check ("T5")) return; break;
			case RoundData.MODE.KILLCOUNT: if(check ("T6")) return; break;
			case RoundData.MODE.ARRIVE: if(check("T7")) return; break;
			case RoundData.MODE.DESTROY: if(check ("T8")) return; break;
			case RoundData.MODE.GETITEM: if(check ("T9")) return; break;
				//		case RoundData.MODE.C_RUN:  if(check ("T18")) return; break;
				//		case RoundData.MODE.C_SURVIVAL:  if(check ("T19")) return; break;
				//		case RoundData.MODE.C_HUNT:  if(check ("T20")) return; break;
			}
			
			if(GameManager.me.stageManager.nowRound.chaser != null) if(check ("T10")) return; 

		}
	}





	public bool skipThisTime = false;
	
	// 튜토리얼 모드일때는 모든 버튼의 클릭을 막은 뒤 검사한 후 튜토리얼을 진행한다.
	public bool checkButton(GameObject go)
	{
		// 에러 팝업이면 무조건 클릭 가능하게!
		// 레벨업 팝업이면 레벨업 튜토리얼 제외하고 무조건 클릭 가능하게!

		if(skipThisTime)
		{
			skipThisTime = false;

			if(go == GameManager.me.uiManager.uiTutorial.btnDialog.gameObject || go == GameManager.me.uiManager.uiTutorial.btnBigDialog)
			{
				return false;
			}
		}

		if(isStart)
		{
			if(checkTutorialIgnoreButtons(go)) return true;

			return false;
		}
		else if(isTutorialMode)
		{
#if UNITY_EDITOR
			Debug.LogError(go + "    " + go.name + "  nowTutorialId : " + nowTutorialId + "   subStep : " + subStep);
#endif

			if(checkTutorialIgnoreButtons(go)) return true;

			switch(nowTutorialId)
			{
			case "T1": return check1 (go); break;
			case "T2": return check2_8 (go); break;
			case "T3": return check2_8 (go); break;
			case "T4": return check2_8 (go); break;
			case "T5": return check2_8 (go); break;
			case "T6": return check2_8 (go); break;
			case "T7": return check2_8 (go); break;
			case "T8": return check2_8 (go); break;
			case "T9": return check9 (go); break;
			case "T10": return check10 (go); break;
//			case "T11": return check11 (go); break;
//			case "T12": return check12 (go); break;
			case "T13": return check13 (go); break;
//			case "T14": return check14 (go); break;
			case "T15": return check15 (go); break;
//			case "T16": return check16 (go); break;
//			case "T17": return check17 (go); break;
//			case "T18": return check18_20 (go); break;
//			case "T19": return check18_20 (go); break;
//			case "T20": return check18_20 (go); break;
//			case "T21": return check21 (go); break;
//			case "T22": return check22 (go); break;
//			case "T23": return check23 (go); break;
			case "T24": return check24 (go); break;
//			case "T25": return check25 (go); break;
//			case "T26": return check26 (go); break;
//			case "T27": return check27 (go); break;
//			case "T28": return check28 (go); break;
//			case "T29": return check29 (go); break;
//			case "T30": return check30 (go); break;
//			case "T31": return check31 (go); break;
//			case "T32": return check32 (go); break;
//			case "T33": return check33 (go); break;
//			case "T34": return check34 (go); break;
//
//			case "T35": return check35 (go); break;
//			case "T36": return check36 (go); break;
			case "T37": return check37 (go); break;

//			case "T38": return check38 (go); break;
//			case "T39": return check39 (go); break;
//			case "T40": return check40 (go); break;
//			case "T41": return check41 (go); break;
//			case "T42": return check42 (go); break;
//			case "T43": return check43 (go); break;

			case "T44": return check44 (go); break;
			case "T45": return check45 (go); break;
			case "T46": return check46 (go); break;
//			case "T47": return check47 (go); break;

			case "T48": return check48 (go); break;
			case "T49": return check49 (go); break;
			case "T50": return check50 (go); break;
			case "T51": return check51 (go); break;
			case "T52": return check52 (go); break;
				
			}
		}
		else if(isTutorialSkipCheckMode)
		{
			if(checkTutorialIgnoreButtons(go)) return true;

			if(go == uiTutorial.btnDialogYes.gameObject)
			{
				onConfirmStartTutorial();
			}
			else if(go == uiTutorial.btnDialogNo.gameObject)
			{
				onSkipTutorial();
			}

			return false;
		}

		return true;
	}


	bool checkTutorialIgnoreButtons(GameObject go)
	{
		if(go == UISystemPopup.instance.popupLevelup.btnClose.gameObject)
		{
			return true;
		}
		else if(go == UISystemPopup.instance.popupDefault.btnClose.gameObject && UISystemPopup.instance.popupDefault.popupData.popupType == UISystemPopup.PopupType.SystemError)
		{
			return true;
		}
		else if(go == GameManager.me.uiManager.popupNickName.btnClose.gameObject)
		{
			return true;
		}

		return false;
	}



	// 시작시에 버튼을 막는다.
	public void uiInit()
	{
		//return;

		#if UNITY_EDITOR
		if(useTutorialDebugMode == false) return;
		#endif

		//Debug.LogError("여기 수정해야한다.");

		//"친구 버튼, 미션 버튼, 챔피언십 버튼 튜토리얼 후 활성화 하기 히어로, 소환룬, 스킬룬 튜토리얼 전 비활성화 하기"

		uiManager.uiMenu.uiLobby.btnFriend.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,1,3) || GameManager.me.isGuest);
		uiManager.uiMenu.uiLobby.spHasTutorialForFriend.gameObject.SetActive(clearCheck("T15") == false && !GameManager.me.isGuest);
		

		



		uiManager.uiMenu.uiLobby.btnMission.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,1,3));
		uiManager.uiMenu.uiLobby.spHasTutorialForMission.gameObject.SetActive(clearCheck("T13") == false);


		uiManager.uiMenu.uiLobby.btnHero.gameObject.SetActive (clearCheck("T46"));
		uiManager.uiMenu.uiLobby.btnSummon.gameObject.SetActive (clearCheck("T44"));
		uiManager.uiMenu.uiLobby.btnSkill.gameObject.SetActive (clearCheck("T45"));


		if(GameDataManager.instance.roundClearStatusCheck(1,4,4))
		{
			uiManager.uiMenu.uiWorldMap.spChampionship.color = uiManager.uiMenu.uiWorldMap.btnChampionship.defaultColor;
			uiManager.uiMenu.uiWorldMap.spChampionshipExclamation.enabled = true;
			uiManager.uiMenu.uiWorldMap.spChampionshipDormancy.enabled = true;
		}
		else
		{
			uiManager.uiMenu.uiWorldMap.spChampionship.color = uiManager.uiMenu.uiWorldMap.btnChampionship.disabledColor;
			uiManager.uiMenu.uiWorldMap.spChampionshipExclamation.enabled = false;
			uiManager.uiMenu.uiWorldMap.spChampionshipDormancy.enabled = false;
		}

		uiManager.uiMenu.uiWorldMap.spHasTutorialForChampionship.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,4,4) && clearCheck("T24") == false);


		if(GameDataManager.instance.roundClearStatusCheck(1,3,4))
		{
			uiManager.uiMenu.uiWorldMap.spHell.color = uiManager.uiMenu.uiWorldMap.btnHell.defaultColor;
		}
		else
		{
			uiManager.uiMenu.uiWorldMap.spHell.color = uiManager.uiMenu.uiWorldMap.btnHell.disabledColor;
		}

		uiManager.uiMenu.uiWorldMap.spHasTutorialForHell.gameObject.SetActive(GameDataManager.instance.roundClearStatusCheck(1,3,4) && clearCheck("T51") == false);

	}




	// 단축키 

	public static UITutorial uiTutorial
	{
		get { return GameManager.me.uiManager.uiTutorial; }
	}
	
	static UIManager uiManager
	{
		get { return GameManager.me.uiManager; }
	}
	
	public void openDialog(float posX, float posY, bool backgroundShadow, bool buttonVisible, bool includeName = false)
	{
		if(includeName) uiTutorial.openTutorialDialog(posX, posY, buttonVisible, nowTutorialId, subStep, GameDataManager.instance.name);
		else uiTutorial.openTutorialDialog(posX, posY, buttonVisible,nowTutorialId, subStep);

		GameManager.me.uiManager.uiTutorial.spBackground.gameObject.SetActive(backgroundShadow);
	}
	
	public void setArrowAndDim(float posX, float posY)
	{
		GameManager.me.uiManager.uiTutorial.setArrowAndDim(posX,posY);
	}



	public void closeTutorial()
	{
		isTutorialMode = false;
		nowTutorialId = "";

		if(GameManager.me.uiManager.currentUI != UIManager.Status.UI_PLAY)
		{
			SoundManager.instance.clearTutorialVoiceAsset();
		}
		else
		{
			SoundManager.instance.stopTutorialVoice();
		}

	}


}

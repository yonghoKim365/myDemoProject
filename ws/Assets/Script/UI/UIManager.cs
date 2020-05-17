using UnityEngine;
using System.Collections;
using System.Text;

public class UIManager : MonoBehaviour 
{
	public UILayoutEffect uiLayoutEffect;

	static UIManager instance;

	public UIVisitingLobby uiVisitingLobby;

	public UIPlay uiPlay;
	public UIMenu uiMenu;
	public UITitle uiTitle;
	public UILoading uiLoading;

	public UISystemPopup uiSystemPopup;

//	public UIPopupChallenge popupChallenge;
//	public UIPopupChallengeResult popupChallengeResult;
	public UIPopupChampionship popupChampionship;
	public UIPopupChampionshipAttack popupChampionshipAttack;
	public UIPopupTableGuide popupTableGuide;
	public UIPopupChampionshipResult popupChampionshipResult;
	public UIPopupChampionshipLastWeekResult popupChampionshipLastWeekResult;
	public UIChampionshipReplayPanel	popupChampionshipReplayPanel;

	public UIRoundClearPopup popupRoundClear;

	public UIPopupHell popupHell;
	public UIPopupHellResult popupHellResult;
	public UIPopupHellModeGuide popupHellGuide;

	public UIPopupDefenceResult popupDefenceResult;
	public UIPopupFriendDetail popupFriendDetail;
	public UIPopupFriendInvite popupFriendInvite;
	public UIPopupNickName popupNickName;
	public UIPopupOption popupOption;
	public UIPopupMessage popupMessage;
	public UIPopupReward popupReward;
	public UIPopupRoundStart popupRoundStart;

	public UIPopupShop popupShop;

	public UIPopupRuneBook popupRuneBook;
	public UIPopupEquipRuneBook popupEquipRuneBook;


	public UIPopupHeroInfo popupHeroInfo;

	


	public UIRetryPopup popupRetry;

	public UIGoldForRubyPopup popupGoldForRuby;

	public UIPopupFriendlyAttack popupFriendlyPVPAttack;

	public UIPopupSkillPreview popupSkillPreview;
	public UISummonDetailPopup popupSummonDetail;

	public UIHeroItemDetailPopup popupHeroDetail
	{
		get
		{
			return GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup;
		}
	}


	public UISpecialPackPopup popupSpecialPack;
	public UISpecialPackPopup popupSpecialSinglePack;


	public UIPopupAttend popupAttend;
	public UIPopupAttendReward popupAttendReward;

	public UIPopupPaused popupPaused;

	public UIPopupNoticeUpdateWaitingArea popupUpdateWatingNotice;

	public UIRewardNoticePanel rewardNotice;

	public UITutorial uiTutorial;

	public UIPlayStageClear stageClearEffectManager;


	public UIPopupEvolution popupEvolution;


	public UIPopupInstantDungeon popupInstantDungeon;

	public UIRuneReforegePopup popupReforege;


	// 진화용 카메라.
	public Camera menuCamera3;

	// 룬, 장비 미리보기용 카메라.
	public Camera menuCamera2;

	// 메인 UI 이벤트 카메라.
	public UICamera uiMenuCamera;

	public GameObject goMakeCompleteGuidePanel;

	public GameObject goBtnRuneStudioSkip;

	public enum Status
	{
		UI_TITLE, UI_MENU, UI_LOADING, UI_PLAY, UI_OPENING
	}

	public Status currentUI = Status.UI_TITLE;
	
	public void changeUI(Status ui, bool ignoreSameUI = false)
	{
#if UNITY_EDITOR
		Debug.LogError("====== " + currentUI + "   " + ui + "   ignore sameui: " + ignoreSameUI);
#endif

		if(currentUI == ui && ignoreSameUI) return;
		
		switch(ui)
		{
		case Status.UI_TITLE:
			GameManager.setTimeScale = 1;
			if(uiTitle != null) uiTitle.show();
			uiMenu.hide();
			uiLoading.hide();
			GameManager.me.gameCamera.gameObject.SetActive(false);
			break;

		case Status.UI_OPENING:
			if(uiTitle != null) uiTitle.hide();
			uiLoading.hide();
			uiMenu.hide();
			uiPlay.hide();
			uiPlay.hideMenu(0);

			GameManager.me.gameCamera.gameObject.SetActive(false);
			break;
		case Status.UI_MENU:
			if(uiTitle != null) uiTitle.hide();

			GameManager.me.isPaused = false;
			Vector3 v = uiMenu.uiLobby.transform.localPosition;
			v.x = 0.0f;
			uiMenu.uiLobby.transform.localPosition = v;			
			uiMenu.show();
			uiPlay.hide();
			uiLoading.hide();
			GameManager.me.gameCamera.gameObject.SetActive(false);
			SoundData.play("bgm_maintheme", true);

			break;
			
		case Status.UI_LOADING:
			uiLoading.show();
			uiPlay.hide();
			uiPlay.hideReadyBattleAnimation();
			break;
			
		case Status.UI_PLAY:

			GameManager.me.characterManager.inGameGUIContinaer.gameObject.SetActive(true);
			//GameManager.me.inGameGUICamera.depth = 8;
			CharacterAttachedUI.gameViewCamera = GameManager.me.gameCamera;
			GameManager.me.gameCamera.gameObject.SetActive(true);
			uiMenu.hide();			
			uiPlay.show();	
			uiPlay.gameObject.SetActive(true);
			uiPlay.resetCamera();

			if(GameManager.me.recordMode != GameManager.RecordMode.continueGame)
			{
				uiLoading.hide();
			}
			break;
		}

#if UNITY_EDITOR
		Debug.Log("ChangeUI Complete");
#endif
		currentUI = ui;
	}



	public void showLoading()
	{
		currentUI = Status.UI_LOADING;
		uiLoading.show();
		uiPlay.hide();
		uiPlay.hideReadyBattleAnimation();
	}




	
	IEnumerator goToMenu(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		changeUI(Status.UI_MENU);
	}

	
	void Awake()
	{
		instance = this;
	}
	
	void OnDestroy()
	{
		uiPlay = null;
		uiMenu = null;
		uiTitle = null;
		uiLoading = null;
		uiSystemPopup = null;
		popupPaused = null;
		instance = null;
	}	


	public void init()
	{
		uiSystemPopup.hide();
		hideAllPopup();

		rewardNotice.hide();

		goBtnRuneStudioSkip.SetActive(false);

		menuCamera3.gameObject.SetActive(false);
	}

	void hideAllPopup()
	{

		popupChampionship.hide(true);
		popupTableGuide.hide(true);
		popupChampionshipResult.hide(true);
		popupChampionshipLastWeekResult.hide(true);
		popupDefenceResult.hide(true);
		popupFriendDetail.hide(true);
		popupFriendInvite.hide(true);
		popupNickName.hide(true);
		popupOption.hide(true);
		popupMessage.hide(true);
		popupReward.hide(true);
		popupRoundStart.hide(true);

		popupHellResult.hide(true);
		popupHellGuide.hide(true);

		popupShop.hide (true);
		popupRuneBook.hide(true);

		popupChampionshipAttack.hide(true);
		popupFriendlyPVPAttack.hide(true);

		popupPaused.hide(true);

		popupSpecialPack.hide(true);
		popupSpecialSinglePack.hide(true);

		popupEvolution.hide(true);

		popupInstantDungeon.hide(true);

		popupReforege.hide(true);

		if(popupRoundClear.gameObject.activeSelf) popupRoundClear.gameObject.SetActive(false);

	}



	void onSelectPause(GameObject go)
	{
		pause();
	}
	
	public void pause()
	{

#if !UNITY_EDITOR
		if(GameManager.me.stageManager.nowRound != null &&
		   GameManager.me.stageManager.nowRound.id == "INTRO")
		{
			return;
		}
#endif



		if(Time.timeScale > 0)
		{
			//SoundManager.GetInstance().bgm_play.Pause();
			GameManager.setTimeScale = 0;
			GameManager.me.clearMemory();
			popupPaused.show();
		}
	}


	public static bool checkRubyPopup(int ruby)
	{
		if(GameDataManager.instance.ruby < ruby)
		{
			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
			return false;
		}

		return true;
	}

	private static int _index = 0;
	public static void setGameState(string str, float value = -1f, float duration = -1, float perValue = 0.1f)
	{

		try
		{
			if(GameManager.me != null && GameManager.me.uiManager != null)
			{
				if(GameManager.me.uiManager.uiTitle != null)
				{
					if(Weme.instance != null)
					{
						Weme.instance.stopSceneLoadingBar = true;
						
						if(Weme.instance.lbText != null) Weme.instance.lbText.text = str;
						
						if(value >= 0)
						{
							if(Weme.instance.spProgressBar != null) Weme.instance.spProgressBar.fillAmount = value;
							
							++_index;
							
							if(duration > 0)
							{
								GameManager.me.uiManager.updateTitleProgressBar(value, duration, perValue);
							}
						}
						
					} 
				}
			}

		}
		catch
		{

		}
	}

	public void updateTitleProgressBar(float startValue, float duration, float perValue)
	{
		StartCoroutine(updateTitleProgressBarCT(startValue, duration, perValue, _index));
	}


	private WaitForSeconds _ws005 = new WaitForSeconds(0.05f);
	IEnumerator updateTitleProgressBarCT(float startValue, float duration, float perValue, int index)
	{
		float totalTime = 0;
		while(_index == index && totalTime < duration)
		{
			setTitleProgress( startValue + (totalTime / duration ) * perValue);
			yield return _ws005;
			totalTime += 0.05f;
		}
	}


	public void activeRuneStudioSkipButton()
	{
		if(GameManager.me.uiManager.popupMessage.gameObject.activeSelf) return;
		StartCoroutine(activeRuneStudioSkipButtonCT());
	}
	
	IEnumerator activeRuneStudioSkipButtonCT()
	{
		yield return new WaitForSeconds(0.5f);
		goBtnRuneStudioSkip.SetActive(true);
	}




	public static void setTitleProgress(float value)
	{
		if(GameManager.me != null && GameManager.me.uiManager != null && GameManager.me.uiManager.uiTitle != null)
		{
			if(Weme.instance != null && Weme.instance.spProgressBar != null) Weme.instance.spProgressBar.fillAmount = value;
		}
	}



	public enum PhotoDownLoadType
	{
		Init, DownLoad, InitAndDownload
	}
	public static bool setPlayerPhoto(string imageUrl, UISprite spEmptyFace, PhotoDownLoader face, PhotoDownLoadType type, int photoSize = -1)
	{
		if(imageUrl == null || imageUrl.ToLower().Contains("http") == false)
		{
			face.gameObject.SetActive( false );
			if(imageUrl == null) imageUrl = "";
			switch(imageUrl.ToUpper())
			{
			case Character.LEO:
				spEmptyFace.spriteName = Character.LEO_IMG;
				break;
			case Character.CHLOE:
				spEmptyFace.spriteName = Character.CHLOE_IMG;
				break;
			case Character.KILEY:
				spEmptyFace.spriteName = Character.KILEY_IMG;
				break;
			default:
				spEmptyFace.spriteName = Character.EMPTY_IMG;
				break;
			}
		}
		else
		{
			face.gameObject.SetActive( true );

			if(type != PhotoDownLoadType.DownLoad) face.init(imageUrl);

			if(type != PhotoDownLoadType.Init)
			{
				if(photoSize > 0)
				{
					face.down(imageUrl, photoSize);
				}
				else
				{
					face.down(imageUrl);
				}
			}

			return true;
		}

		return false;
	}




	public static void setPlayerPhoto(int showPhoto, string imageUrl, UISprite spEmptyFace, PhotoDownLoader face, bool downloadNow = false, int photoSize = -1)
	{
		if(showPhoto == WSDefine.TRUE)
		{
			if(imageUrl == null || imageUrl.ToLower().Contains("http") == false)
			{
				face.gameObject.SetActive( false );
				if(imageUrl == null) imageUrl = "";
				switch(imageUrl.ToUpper())
				{
				case Character.LEO:
					spEmptyFace.spriteName = Character.LEO_IMG;
					break;
				case Character.CHLOE:
					spEmptyFace.spriteName = Character.CHLOE_IMG;
					break;
				case Character.KILEY:
					spEmptyFace.spriteName = Character.KILEY_IMG;
					break;
				default:
					spEmptyFace.spriteName = Character.EMPTY_IMG;
					break;
				}

			}
			else
			{
				face.gameObject.SetActive( true );
				face.init(imageUrl);
				if(downloadNow)
				{
					if(photoSize > 0)
					{
						face.down(imageUrl, photoSize);
					}
					else
					{
						face.down(imageUrl);
					}
				}
			}
		}
		else
		{
			face.gameObject.SetActive( false );
			if(imageUrl == null) imageUrl = "";
			switch(imageUrl.ToUpper())
			{
			case Character.LEO:
				spEmptyFace.spriteName = Character.LEO_IMG;
				break;
			case Character.CHLOE:
				spEmptyFace.spriteName = Character.CHLOE_IMG;
				break;
			case Character.KILEY:
				spEmptyFace.spriteName = Character.KILEY_IMG;
				break;
			default:
				spEmptyFace.spriteName = Character.EMPTY_IMG;
				break;
			}
		}
	}

}


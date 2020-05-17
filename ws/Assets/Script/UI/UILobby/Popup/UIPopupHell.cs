using UnityEngine;
using System.Collections;
using System;

public class UIPopupHell : UIPopupBase {

	public GameObject goHasMyRecordContainer, goDontHaveRecordContainer, goLeftTimeToUpdateRanking;

	public UIButton btnStart, btnOpenRewardList;

	public UIButton tabFriend, tabAll;

	public UIButton btnOpenTooltipToday, btnOpenTooltipWeek, btnOpenTooltipBest;

	public UITabButton tabButtonFriend, tabButtonAll;

	public UILabel lbLeftTime, lbTodayScore, lbTodayWave, lbWeeklyScore, lbWeeklyRanking, lbBestScore, lbBestRanking;

	public UILabel lbFriendRanking, lbMyName, lbMyScore;

	public UISprite spMyRankIcon;

	public UIHellRankingList list;

	public UILabel lbPlayTicket;

	public UILabel lbLefTimeToUpdateRanking;

	public UIIconTooltip recordTooltip;

	int _rankingUpdateTime = 100;
	DateTime _rankingUpdateCheckTime = DateTime.Now;

//	public const int ITEM_NUM_PER_PAGE = 25;

	public bool lastTimeActionIsOpenWorldRanking = false;

	protected override void onClickClose (GameObject go)
	{
		if(UINetworkLock.instance != null) UINetworkLock.instance.hide();
		base.onClickClose (go);
		recordTooltip.hide();
		GameManager.me.uiManager.uiMenu.uiWorldMap.checkTutorialStart(false);
		GameManager.me.uiManager.uiMenu.uiWorldMap.refreshFriendPhoto();
	}

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnStart.gameObject).onClick = onStart;
		UIEventListener.Get(btnOpenRewardList.gameObject).onClick = onOpenRewardList;

		UIEventListener.Get(tabFriend.gameObject).onClick = onClickTabFriend;
		UIEventListener.Get(tabAll.gameObject).onClick = onClickTabAll;
		list.listGrid.callbackAfterDrawFinalItem = onDrawFinalListItem;

		UIEventListener.Get(btnOpenTooltipToday.gameObject).onClick = onClickOpenTooltipToday;
		UIEventListener.Get(btnOpenTooltipWeek.gameObject).onClick = onClickOpenTooltipWeek;
		UIEventListener.Get(btnOpenTooltipBest.gameObject).onClick = onClickOpenTooltipBest;

		GameDataManager.hellTicketDispatcher -= updateHellTicket;
		GameDataManager.hellTicketDispatcher += updateHellTicket;

	}


	void onClickOpenTooltipToday(GameObject go)
	{
		string killNum = GameDataManager.instance.hellInfo.dMonster.ToString();
		string distance = GameDataManager.instance.hellInfo.dDistance.ToString();
		string wave = GameDataManager.instance.hellInfo.dWave.ToString();
		string score = Util.GetCommaScore(GameDataManager.instance.hellInfo.dScore);

		recordTooltip.start( Util.getUIText("HELL_RECORD_DETAIL",killNum, distance, wave, score), 51, -307, 2, true, false);
	}

	void onClickOpenTooltipWeek(GameObject go)
	{
		string killNum = GameDataManager.instance.hellInfo.wMonster.ToString();
		string distance = GameDataManager.instance.hellInfo.wDistance.ToString();
		string wave = GameDataManager.instance.hellInfo.wWave.ToString();
		string score = Util.GetCommaScore(GameDataManager.instance.hellInfo.wScore);
		
		recordTooltip.start( Util.getUIText("HELL_RECORD_DETAIL",killNum, distance, wave, score), 51, -167, 2, true, false);
	}

	void onClickOpenTooltipBest(GameObject go)
	{
		string killNum = GameDataManager.instance.hellInfo.tMonster.ToString();
		string distance = GameDataManager.instance.hellInfo.tDistance.ToString();
		string wave = GameDataManager.instance.hellInfo.tWave.ToString();
		string score = Util.GetCommaScore(GameDataManager.instance.hellInfo.tScore);
		
		recordTooltip.start( Util.getUIText("HELL_RECORD_DETAIL",killNum, distance, wave, score), 51, -245, 2, true, false);
	}


	private bool _friendListLock = false;
	private bool _userListLock = false;
	void onDrawFinalListItem(int finalListItemIndex)
	{
		if(list.listGrid.dragScrollView != null)
		{
			if(list.isFriendType)
			{
				if(canRequestMoreFriendRank && _friendListLock == false) //&& GameDataManager.instance.hellFriendRanks.Count % ITEM_NUM_PER_PAGE == 0)
				{
					Debug.LogError("request more list : " + (GameDataManager.instance.hellFriendRanks.Count+1));

					_friendListLock = true;
					list.listGrid.dragScrollView.DisableSpring();
					list.listGrid.dragScrollView.enabled = false;
					EpiServer.instance.sendGetHellFriendRank(GameDataManager.instance.hellFriendRanks.Count+1);
				}
			}
			else
			{
				if(canRequestMoreUserRank && _userListLock == false) // && GameDataManager.instance.hellUserRanks.Count % ITEM_NUM_PER_PAGE == 0)
				{
					Debug.LogError("request more list : " + (GameDataManager.instance.hellUserRanks.Count+1));

					_userListLock = true;
					list.listGrid.dragScrollView.DisableSpring();
					list.listGrid.dragScrollView.enabled = false;

					EpiServer.instance.sendGetHellUserRank(GameDataManager.instance.hellUserRanks.Count+1);
				}
			}

			Debug.LogError("last item index: " + finalListItemIndex);
		}
	}


	public void setTabFriend()
	{
		tabButtonFriend.isEnabled = true;
		tabButtonAll.isEnabled = false;
		list.isFriendType = true;
		
		lastTimeActionIsOpenWorldRanking = false;

		refreshInfo();
	}

	void onClickTabFriend(GameObject go)
	{
		tabButtonFriend.isEnabled = true;
		tabButtonAll.isEnabled = false;
		list.isFriendType = true;

		lastTimeActionIsOpenWorldRanking = false;

		if(GameDataManager.instance.hellFriendRanks.Count > 0)
		{
			refreshList(true);
		}
		else
		{
			EpiServer.instance.sendGetHellFriendRank(1);
		}

		refreshInfo();
	}

	void onClickTabAll(GameObject go)
	{
		tabButtonFriend.isEnabled = false;
		tabButtonAll.isEnabled = true;
		list.isFriendType = false;

		if(GameDataManager.instance.hellUserRanks.Count > 0)
		{
			refreshList(true);
		}
		else
		{
			lastTimeActionIsOpenWorldRanking = true;
			EpiServer.instance.sendGetHellUserRank(1);
		}

		refreshInfo();
	}



	public override void show ()
	{
		if(TutorialManager.isTutorialOpen("T44") ||
		   TutorialManager.isTutorialOpen("T45") ||
		   TutorialManager.isTutorialOpen("T13") ||
		   TutorialManager.isTutorialOpen("T15") ||
		   TutorialManager.isTutorialOpen("T46") ||
		   TutorialManager.isTutorialOpen("T24") )
		{
			return;
		}

		base.show ();

		list.clear();
		list.isFriendType = true;
		tabButtonFriend.isEnabled = true;
		tabButtonAll.isEnabled = false;

		canRequestMoreUserRank = true;
		canRequestMoreFriendRank = true;
		lastGetHellUserRankRequestStartIndex = -1;
		lastGetHellFriendRankRequestStartIndex = -1;

		_friendListLock = false;
		_userListLock = false;

		GameDataManager.instance.hellFriendRanks.Clear();
		GameDataManager.instance.hellUserRanks.Clear();

		updateHellTicket();

		_lastOpenDay = DateTime.Now.Day;
		_updateTicketDelay = 2.0f;


		setRankingUpdateTimeInfo();

		goLeftTimeToUpdateRanking.SetActive(false);

		StartCoroutine(sendPreparePacketCT());


	}


	void setRankingUpdateTimeInfo()
	{
		_rankingUpdateCheckTime = DateTime.Now;

		if(_rankingUpdateCheckTime.Minute >= 2)
		{
			_rankingUpdateCheckTime = Weme.ChangeTime(DateTime.Now, DateTime.Now.Hour, 2).AddHours(1);
		}
		else
		{
			_rankingUpdateCheckTime = Weme.ChangeTime(DateTime.Now, DateTime.Now.Hour, 2);
		}

		TimeSpan ts = (_rankingUpdateCheckTime - DateTime.Now);

		_rankingUpdateTime = (int)ts.TotalSeconds;

		_rankingUpdateCheckTime = DateTime.Now;
	}


	public void updateHellTicket()
	{
		if(GameDataManager.instance.hellTicket < 1)
		{
			lbPlayTicket.text = "[ba0000]"+GameDataManager.instance.hellTicket +"[-][fef9a8]/"+GameDataManager.instance.maxHellTicket+"[-]";
		}
		else
		{
			lbPlayTicket.text = "[fef9a8]"+GameDataManager.instance.hellTicket+"/"+GameDataManager.instance.maxHellTicket+"[-]";
		}

	}


	public void showWithoutAni ()
	{
		if(TutorialManager.isTutorialOpen("T44") ||
		   TutorialManager.isTutorialOpen("T45") ||
		   TutorialManager.isTutorialOpen("T13") ||
		   TutorialManager.isTutorialOpen("T15") ||
		   TutorialManager.isTutorialOpen("T46") ||
		   TutorialManager.isTutorialOpen("T24") )
		{
			return;
		}

		popupPanel.transform.localPosition = new Vector3(-3.98f,-0.36f,0.0f);
		gameObject.SetActive(true);

		_lastOpenDay = DateTime.Now.Day;
		_updateTicketDelay = 2.0f;

		list.clear();
		list.isFriendType = true;
		tabButtonFriend.isEnabled = true;
		tabButtonAll.isEnabled = false;
		
		canRequestMoreUserRank = true;
		canRequestMoreFriendRank = true;
		lastGetHellUserRankRequestStartIndex = -1;
		lastGetHellFriendRankRequestStartIndex = -1;
		
		_friendListLock = false;
		_userListLock = false;
		
		GameDataManager.instance.hellFriendRanks.Clear();
		GameDataManager.instance.hellUserRanks.Clear();
		
		updateHellTicket();

		setRankingUpdateTimeInfo();

		goLeftTimeToUpdateRanking.SetActive(false);

		EpiServer.instance.sendPrepareHell();

	}


	IEnumerator sendPreparePacketCT()
	{
		while(ani != null && ani.isPlaying)
		{
			yield return null;
		}

		yield return null;

		EpiServer.instance.sendPrepareHell();
	}


	void onOpenRewardList(GameObject go)
	{
		GameManager.me.uiManager.popupHellGuide.show(UIPopupHellModeGuide.Type.Week);
	}



	private float _lastTouchTime = -1;

	void onStart(GameObject go)
	{
#if UNITY_EDITOR
		if(DebugManager.instance.useDebug)
		{
			hide();
			startHell();
		}
		else
#endif
		{
			if(GameDataManager.instance.hellTicket > 0)
			{
//				if(Time.realtimeSinceStartup - _lastTouchTime > 1.0f)
				{
					EpiServer.instance.sendPlayHell();
					_lastTouchTime = Time.realtimeSinceStartup;
				}
			}
			else 
			{
				if(GameDataManager.instance.serviceMode == GameDataManager.ServiceMode.CBT)
				{
					UISystemPopup.open( UISystemPopup.PopupType.Default, Util.getUIText("NO_HELLTICKET", "10"));
				}
				else
				{
					UISystemPopup.open( UISystemPopup.PopupType.Default, Util.getUIText("NO_HELLTICKET", "3"));
				}

				//UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("CHALLENGE_BY_RUBY",GameDataManager.instance.challengeRoundInfo.playNeedRuby.ToString()), onConfirnRubyChallenge, null, GameDataManager.instance.challengeRoundInfo.playNeedRuby.ToString());
			}
		}
	}


//	void onConfirnRubyChallenge()
//	{
//
//		if(GameDataManager.instance.ruby < GameDataManager.instance.challengeRoundInfo.playNeedRuby)
//		{
//			GameManager.me.uiManager.popupShop.showRubyShop();
//		}
//		else
//		{
//			EpiServer.instance.sendPlayChallenge();
//		}
//	}


	public void startHell()
	{
		hide();
		GameManager.me.stageManager.setNowRound(GameManager.info.roundData["HELL"], GameType.Mode.Hell);
		GameManager.me.uiManager.showLoading();
		GameManager.me.startGame(0.5f);
	}


//	void onRubyStart(GameObject go)
//	{
//		if(GameDataManager.instance.ruby >= GameDataManager.instance.challengeRoundInfo.changeItemRuby)
//		{
//			EpiServer.instance.sendChangeChallengeItems();
//		}
//		else
//		{
//			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
//		}
//	}

	public bool canRequestMoreFriendRank = true;
	public bool canRequestMoreUserRank = true;

	public int lastGetHellFriendRankRequestStartIndex = -1;
	public int lastGetHellUserRankRequestStartIndex = -1;

	public void refreshList(bool rePosition)
	{
		if(rePosition == false)
		{
			UINetworkLock.instance.show();

//			if(list.isFriendType)
//			{
//				_friendListLock = false;
//			}
//			else
//			{
//				_userListLock = false;
//			}
		}
		else
		{
			if(list.isFriendType == false) setRankingUpdateTimeInfo();
		}

		list.draw(rePosition);

		StartCoroutine(onCompleteRefreshList(rePosition));
	}


	IEnumerator onCompleteRefreshList(bool rePosition)
	{
		yield return new WaitForSeconds(0.1f);
		list.listGrid.dragScrollView.enabled = true;

		if(rePosition == false)
		{
			if(list.isFriendType)
			{
				_friendListLock = false;
			}
			else
			{
				_userListLock = false;
			}

			UINetworkLock.instance.hide();
		}
	}



	public void refreshInfo()
	{
		lbTodayScore.text = Util.GetCommaScore(GameDataManager.instance.hellInfo.dScore);
		lbTodayWave.text = "( "+GameDataManager.instance.hellInfo.dWave+" Wave )";
		
		lbWeeklyScore.text = Util.GetCommaScore(GameDataManager.instance.hellInfo.wScore);

		if(GameDataManager.instance.hellInfo.wRank == 0)
		{
			lbWeeklyRanking.text = Util.getUIText("HELL_RANK_TEXT",GameDataManager.instance.hellInfo.fRank.ToString(),"-", Mathf.CeilToInt( GameDataManager.instance.hellInfo.wGroup ).ToString());
		}
		else
		{
			lbWeeklyRanking.text = Util.getUIText("HELL_RANK_TEXT",GameDataManager.instance.hellInfo.fRank.ToString(),GameDataManager.instance.hellInfo.wRank.ToString(), Mathf.CeilToInt( GameDataManager.instance.hellInfo.wGroup ).ToString());
		}


		lbBestScore.text = Util.GetCommaScore(GameDataManager.instance.hellInfo.tScore);;


		if(GameDataManager.instance.hellInfo.tRank == 0)
		{
			lbBestRanking.text = Util.getUIText("MYBESTRANK","-");
		}
		else
		{
			lbBestRanking.text = Util.getUIText("MYBESTRANK",GameDataManager.instance.hellInfo.tRank.ToString());
		}

		if(list.isFriendType)
		{
			goHasMyRecordContainer.SetActive(GameDataManager.instance.hellInfo.fRank > 0);
			goDontHaveRecordContainer.SetActive(!goHasMyRecordContainer.activeSelf);
			goLeftTimeToUpdateRanking.SetActive(false);

			lbFriendRanking.text = GameDataManager.instance.hellInfo.fRank + "";
			spMyRankIcon.enabled = (GameDataManager.instance.hellInfo.fRank <= 3);
		}
		else
		{
			goHasMyRecordContainer.SetActive(GameDataManager.instance.hellInfo.wRank > 0);
			goDontHaveRecordContainer.SetActive(!goHasMyRecordContainer.activeSelf);
			goLeftTimeToUpdateRanking.SetActive(true);

			lbFriendRanking.text = GameDataManager.instance.hellInfo.wRank + "";
			spMyRankIcon.enabled = (GameDataManager.instance.hellInfo.wRank <= 3);
		}
		
		lbMyName.text = GameDataManager.instance.name;
		
		lbMyScore.text = Util.GetCommaScore(GameDataManager.instance.hellInfo.wScore);

	}




	float _delayTime = 0;
	float _updateTicketDelay = 2.0f;

	void LateUpdate()
	{
		if(GameDataManager.instance.hellInfo == null) return;
		
		if(_delayTime > 0)
		{
			_delayTime -= Time.smoothDeltaTime;

		}
		else
		{
			_delayTime = 0.5f;
		}

		int time = GameDataManager.instance.hellInfo.remainedTime;
		TimeSpan ts;
		int leftTime = 0;

		if(time > 0)
		{
			ts = (DateTime.Now - GameDataManager.instance.hellModeCheckTime);
			leftTime = time - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = "종료";
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString(leftTime);
			}
		}




		if(list.isFriendType == false && _rankingUpdateTime > 0 && _userListLock == false)
		{
			ts = (DateTime.Now - _rankingUpdateCheckTime);
			leftTime = _rankingUpdateTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				setRankingUpdateTimeInfo();
				lbLefTimeToUpdateRanking.text = "-";
				_rankingUpdateTime = -1;
				EpiServer.instance.sendGetHellUserRank(1);

			}
			else
			{
				lbLefTimeToUpdateRanking.text = Util.secToHourMinuteSecondString(leftTime);
			}
		}



		if(_lastOpenDay != DateTime.Now.Day)
		{
			_updateTicketDelay -= 0.5f;

			if(_updateTicketDelay <= 0)
			{
				_lastOpenDay = DateTime.Now.Day;
				
				GameDataManager.instance.hellTicket = 3;
				
				updateHellTicket();
			}
		}
	}


	private int _lastOpenDay = -1;


}

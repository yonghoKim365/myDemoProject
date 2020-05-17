using UnityEngine;
using System.Collections;

public class UIFriend : UIBase {

	public UIFriendList list;

	public UILabel lbFriendPoint, lbSlotMachinePrice;

	public UIButton btnHelp, btnInvite;

	public UIButton btnReceiveBonus, btnLogin;

	public UIFriendSlotMachine slotMachine;

	public GameObject goNoFriendContainer;

	public UILabel lbNoFriend;


	public GameObject goLoading;


	void Awake()
	{
		setBackButton(UIMenu.LOBBY);

		UIEventListener.Get(btnHelp.gameObject).onClick = onClickHelp;
		UIEventListener.Get(btnInvite.gameObject).onClick = onClickInvite;
		UIEventListener.Get(btnReceiveBonus.gameObject).onClick = onClickReceiveBonus;
		UIEventListener.Get(btnLogin.gameObject).onClick = onClickLogin;


		GameDataManager.friendPointDispatcher -= updateFriendPoint;
		GameDataManager.friendPointDispatcher += updateFriendPoint;
	}

	void onClickHelp(GameObject go)
	{
		GameManager.me.uiManager.popupTableGuide.show(UIPopupTableGuide.GuideType.Friend);
//		btnReceiveBonus.isEnabled = !btnReceiveBonus.isEnabled;
	}

	void onClickInvite(GameObject go)
	{
		if(GameManager.me.isGuest)
		{

		}
		else
		{
			GameManager.me.uiManager.popupFriendInvite.show();
		}
	}

	void onClickReceiveBonus(GameObject go)
	{
		if(GameDataManager.instance.friendPoint >= GameDataManager.instance.slotMachinePrice || TutorialManager.instance.nowTutorialId == "T15")
		{
			EpiServer.instance.sendPullSlotMachine();
		}

	}

	void onClickLogin(GameObject go)
	{
		if(GameManager.me.isGuest)
		{
			NetworkManager.RestartApplication();
			//PandoraManager.instance.loginKakao();
		}
	}



	void updateFriendPoint()
	{

		if(GameManager.me.isGuest)
		{
			lbFriendPoint.text = "0";
			btnReceiveBonus.isEnabled = false;
		}
		else
		{
			lbFriendPoint.text = Util.GetCommaScore(GameDataManager.instance.friendPoint);
			
			if(slotMachine != null && slotMachine.isReady)
			{
				btnReceiveBonus.isEnabled = (GameDataManager.instance.slotMachinePrice <= GameDataManager.instance.friendPoint || TutorialManager.instance.isTutorialMode);
			}
		}
	}

	public override void onClickBackToMainMenu (GameObject go)
	{
		list.clear();
		GameManager.soundManager.stopLoopEffect();
		base.onClickBackToMainMenu (go);
	}


	public UISprite spMachineBg, spMachineIcon;


	public bool didRefreshFriendList = false;

	public override void show ()
	{
		_selectPanel = null;
		_selectWorldMapPanel = null;

		base.show ();

		slotMachine.refresh();

		switch(GameDataManager.instance.champLeague)
		{
		case WSDefine.LEAGUE_BRONZE:
			spMachineBg.spriteName = "img_gacha_bronz_bg";
			spMachineIcon.spriteName = "img_gacha_bronz";
			break;
		case WSDefine.LEAGUE_SILVER:
			spMachineBg.spriteName = "img_gacha_bronz_bg";
			spMachineIcon.spriteName = "img_gacha_silver";
			break;
		case WSDefine.LEAGUE_GOLD:
			spMachineBg.spriteName = "img_gacha_silver_bg";
			spMachineIcon.spriteName = "img_gacha_gold";
			break;
		case WSDefine.LEAGUE_MASTER:
			spMachineBg.spriteName = "img_gacha_silver_bg";
			spMachineIcon.spriteName = "img_gacha_master";
			break;
		case WSDefine.LEAGUE_PLATINUM:
			spMachineBg.spriteName = "img_gacha_gold_bg";
			spMachineIcon.spriteName = "img_gacha_platinum";
			break;
		case WSDefine.LEAGUE_LEGEND:
			spMachineBg.spriteName = "img_gacha_gold_bg";
			spMachineIcon.spriteName = "img_gacha_legend";
			break;
		}

		goNoFriendContainer.SetActive(false);

		if(GameManager.me.isGuest)
		{
			goLoading.SetActive(false);

			btnLogin.gameObject.SetActive(true);
			list.gameObject.SetActive(false);

			btnHelp.gameObject.SetActive(false);

			btnInvite.gameObject.SetActive(false);

			updateFriendPoint();

		}
		else
		{
			goLoading.SetActive(true);

			btnHelp.isEnabled = false;

			list.gameObject.SetActive(true);
			btnLogin.gameObject.SetActive(false);
			EpiServer.instance.sendGetFriends(true);



			didRefreshFriendList = true;
		}

		TutorialManager.instance.check("T15");

		//[액트1 스테이지2 라운드2] 이상 클리어 & 월드맵 라운드 정보 팝업 & 카톡로그인유저


	}


	public void refresh()
	{
		_selectPanel = null;
		_selectWorldMapPanel = null;

		lbSlotMachinePrice.text = Util.GetCommaScore(GameDataManager.instance.slotMachinePrice);

		updateFriendPoint();

		if(slotMachine.isReady)
		{
			btnReceiveBonus.isEnabled = (GameDataManager.instance.slotMachinePrice <= GameDataManager.instance.friendPoint || TutorialManager.instance.isTutorialMode);
		}

		if(gameObject.activeInHierarchy == false) return;
		list.draw();
		UINetworkLock.instance.hide();
		System.GC.Collect();

		btnHelp.isEnabled = true;

		goLoading.SetActive(false);

		if(GameDataManager.instance.friendDatas == null || GameDataManager.instance.friendDatas.Count <= 0)
		{
			goNoFriendContainer.gameObject.SetActive(true);
			lbNoFriend.text = Util.getUIText("NOFD"+UnityEngine.Random.Range(0,3));
		}
		else
		{
			goNoFriendContainer.gameObject.SetActive(false);
		}
	}


	private UIFriendListPanel _selectPanel;
	private UIWorldMapFriendDetail _selectWorldMapPanel;

	public void onClickPointButton(UIFriendListPanel panel, UIFriendListPanel.PointButtonType type)
	{
		switch(type)
		{
		case UIFriendListPanel.PointButtonType.Get:
			_selectPanel = panel;
			SoundData.play("uifr_pointget");
			EpiServer.instance.sendReceiveFriendPoint(panel.data.userId);
			break;
		case UIFriendListPanel.PointButtonType.Send:
			_selectPanel = panel;
			SoundData.play("uifr_pointsnd");
			EpiServer.instance.sendSendFriendPoint(panel.data.userId, panel.data.blockMessage);
			break;
		default:
			_selectWorldMapPanel = null;
			_selectPanel = null;
			break;
		}
	}


	public void onClickPointButton(UIWorldMapFriendDetail panel, UIFriendListPanel.PointButtonType type)
	{
		switch(type)
		{
		case UIFriendListPanel.PointButtonType.Get:
			_selectWorldMapPanel = panel;
			EpiServer.instance.sendReceiveFriendPoint(panel.data.userId);
			break;
		case UIFriendListPanel.PointButtonType.Send:
			_selectWorldMapPanel = panel;
			EpiServer.instance.sendSendFriendPoint(panel.data.userId, panel.data.blockMessage);
			break;
		default:
			_selectWorldMapPanel = null;
			break;
		}
	}



	public void onCompleteSendFriendPoint(bool sendKakaoMessage, string userId)
	{
		if(sendKakaoMessage)
		{
			//PandoraManager.instance.kakaoSendMessage(userId, msg, "");
		}

		if(_selectPanel != null && _selectPanel.data != null && _selectPanel.data.userId == userId)
		{
			_selectPanel.data = GameDataManager.instance.friendDatas[userId];
			_selectPanel.refresh();
		}
		else
		{
			list.draw(false);
		}

		if(_selectWorldMapPanel != null)
		{
			_selectWorldMapPanel.refresh();
		}

		_selectPanel = null;
		_selectWorldMapPanel = null;
	}


	public void onCompleteReceiveFriendPoint(string userId)
	{
		if(_selectPanel != null && _selectPanel.data != null && _selectPanel.data.userId == userId)
		{
			_selectPanel.data = GameDataManager.instance.friendDatas[userId];
			_selectPanel.refresh();
		}
		else
		{
			list.draw(false);
		}
		
		if(_selectWorldMapPanel != null)
		{
			_selectWorldMapPanel.refresh();
		}
		
		_selectPanel = null;
		_selectWorldMapPanel = null;
	}



	public void onCompletePullSlotMachine(ToC_PULL_SLOT_MACHINE reward)
	{
		slotMachine.start(reward);
		btnReceiveBonus.isEnabled = false;

	}


	public void onCompleteGetFriendDetailInfo(ToC_GET_FRIEND_DETAIL p, string friendId)
	{
		GameManager.me.uiManager.popupFriendDetail.setData(p, friendId);
	}


	void LateUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(btnBack.isEnabled == false) return;
			if(GameManager.me.uiManager.uiMenu.rayCast(GameManager.me.uiManager.uiMenuCamera.camera, btnBack.gameObject) == false) return;
			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;
			onClickBackToMainMenu(null);
			return;
		}
	}


}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using WmWemeSDK.JSON;

public class UIPopupFriendInvite : UIPopupBase {

	//public UIButton

	public UIInviteList list;

	/*
	public UILabel lbInviteNum;
	public UISprite[] spInviteClear;
	public UILabel[] lbInviteReward;
	public UISprite[] spInviteDouble;
*/
	public UIPopupFriendInviteReward inviteRewardIconListNormal;
	public UIPopupFriendInviteReward inviteRewardIconListKakao;
	private UIPopupFriendInviteReward currentRewardIconList;


	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
		inviteRewardIconListNormal.gameObject.SetActive(false);
		inviteRewardIconListKakao.gameObject.SetActive(false);
		if(PlatformManager.instance != null){
#if UNITY_ANDROID
			if (PlatformManager.instance.type == PlatformManager.Platform.Kakao){
				inviteRewardIconListKakao.gameObject.SetActive(true);
				currentRewardIconList = inviteRewardIconListKakao;
			}
			else{
				inviteRewardIconListNormal.gameObject.SetActive(true);
				currentRewardIconList = inviteRewardIconListNormal;
			}
#else
			inviteRewardIconListNormal.gameObject.SetActive(true);
			currentRewardIconList = inviteRewardIconListNormal;
#endif
		}
	}

	private bool _initInviteFriendList = false;

	public override void show ()
	{
		base.show ();

		if(GameDataManager.instance.inviteFriendIds == null || _initInviteFriendList == false)
		{
			_initInviteFriendList = true;
			EpiServer.instance.sendInviteFriendList();
			return;
		}

		refresh();
	}

	public void refresh()
	{
		if(gameObject.activeInHierarchy)
		{
			list.draw();
		}

		currentRewardIconList.refreshInviteNum();

		System.GC.Collect();
	}


	private UIInviteListPanel _invitedFriend;
	private string _invitedFriendId;

	public void onClickInvite(UIInviteListPanel panel)
	{
		_invitedFriendId = panel.userId;
		_invitedFriend = panel;
		UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("SEND_INVITE", epi.GAME_DATA.friendDic[_invitedFriendId].f_Nick), onOkInvite, onCancelInvite);
	}

	public void onOkInvite()
	{
#if UNITY_EDITOR
		Debug.Log("_invitedFriend : " + _invitedFriendId);
		EpiServer.instance.sendInviteFriend(_invitedFriendId);
		return;
#endif

		Debug.LogError("invite : " + _invitedFriendId);

		NetworkManager.instance.nowRequestedInvitingKakaoId = _invitedFriendId;

		Dictionary<string, string> dic = new Dictionary<string, string>();
		dic.Add("sender_name",PandoraManager.instance.localUser.nickname);
		dic.Add("game_name","윈드소울");

		if (PlatformManager.instance != null && PlatformManager.instance.type == PlatformManager.Platform.Kakao){
			PandoraManager.instance.kakaoSendLinkInviteMessage("-"+_invitedFriendId, "invite", "2912", dic, "");
		}
		else{
			PandoraManager.instance.kakaoSendLinkInviteMessage("-"+_invitedFriendId, "invite", "2272", dic, "");
		}
	}





	void onCancelInvite()
	{
		_invitedFriend = null;
		_invitedFriendId = null;
	}

	public void onCompleteInvite()
	{
		if(_invitedFriend != null)
		{
			_invitedFriend.refresh();
			_invitedFriend = null;
			_invitedFriendId = null;

			currentRewardIconList.refreshInviteNum();
		}
	}

	/*
	bool _checkRewardDouble = false;
	bool _showRewardDouble = true;

	void refreshInviteNum()
	{
		lbInviteNum.text = GameDataManager.instance.inviteCount.ToString();
		
		spInviteClear[0].enabled = GameDataManager.instance.inviteCount >= 10;
		spInviteClear[1].enabled = GameDataManager.instance.inviteCount >= 20;
		spInviteClear[2].enabled = GameDataManager.instance.inviteCount >= 30;
		spInviteClear[3].enabled = GameDataManager.instance.inviteCount >= 40;

		string inviteColor = "bf9086";
		string defaultColor = "854c1b";

		lbInviteReward[0].text = "["+((GameDataManager.instance.inviteCount >= 10)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD10")+"[-]";
		lbInviteReward[1].text = "["+((GameDataManager.instance.inviteCount >= 20)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD20")+"[-]";
		lbInviteReward[2].text = "["+((GameDataManager.instance.inviteCount >= 30)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD30")+"[-]";
		lbInviteReward[3].text = "["+((GameDataManager.instance.inviteCount >= 40)?inviteColor:defaultColor)+"]"+Util.getUIText("INVITE_REWARD40")+"[-]";

		if(_checkRewardDouble == false)
		{
			_checkRewardDouble = true;
			_showRewardDouble = Util.getUIText("INVITE_DOUBLE").Equals("Y");

			spInviteDouble[0].enabled = _showRewardDouble;
			spInviteDouble[1].enabled = _showRewardDouble;
			spInviteDouble[2].enabled = _showRewardDouble;
			spInviteDouble[3].enabled = _showRewardDouble;
		}

		if(_showRewardDouble)
		{
			spInviteDouble[0].alpha = ((GameDataManager.instance.inviteCount >= 10)?0.65f:1.0f);
			spInviteDouble[1].alpha = ((GameDataManager.instance.inviteCount >= 20)?0.65f:1.0f);
			spInviteDouble[2].alpha = ((GameDataManager.instance.inviteCount >= 30)?0.65f:1.0f);
			spInviteDouble[3].alpha = ((GameDataManager.instance.inviteCount >= 40)?0.65f:1.0f);
		}
	}

*/

	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);

		_invitedFriend = null;
		_invitedFriendId = null;

		if(GameManager.me.uiManager.uiMenu.uiFriend.list.listGrid.itemList != null)
		{
			foreach(UIListGridItemPanelBase p in GameManager.me.uiManager.uiMenu.uiFriend.list.listGrid.itemList)
			{
				p.setPhotoLoad();
			}
		}
	}


}

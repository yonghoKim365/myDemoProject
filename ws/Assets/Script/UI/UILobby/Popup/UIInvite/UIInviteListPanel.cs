using UnityEngine;
using System.Collections;
using System;

public class UIInviteListPanel : UIListGridItemPanelBase {

	public UIButton btnInvite;
	public UIButton btnInvite2;
	
	public UILabel lbName;
	
	public PhotoDownLoader face;
	
//	public UISprite spInvited;
	

	public UIDragScrollView scrollView;
	
	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnInvite.gameObject).onClick = onClickInvite;
		UIEventListener.Get(btnInvite2.gameObject).onClick = onClickInvite;

		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.popupFriendInvite.list.panel;
	}



	void OnDestroy()
	{
		btnInvite = null; 
		btnInvite2 = null;
		userId = null;

		scrollView = null;
	}

	public string userId = null;

	public override void setPhotoLoad()
	{
		if(userId == null) return;
		face.down(epi.GAME_DATA.friendDic[userId].image_url);
	}	
	
	public enum PointButtonType
	{
		None, Get, Send
	}
	
	PointButtonType pointType = PointButtonType.None;
	
	public override void setData(object obj)
	{
		string str = (string)obj;

		if(userId != null && userId == str) return;

		userId = str;
		
		face.init(epi.GAME_DATA.friendDic[userId].image_url);

		lbName.text = Util.GetShortID(epi.GAME_DATA.friendDic[userId].f_Nick, 10);
		
		refresh();
		
	}

	
	public void refresh()
	{
		bool canInvite = true;

		if(GameDataManager.instance.inviteFriendIds != null && GameDataManager.instance.inviteFriendIds.Contains(userId))
		{
			canInvite = false;
		}

		if( epi.GAME_DATA.friendDic[userId].supported_device == false )
		{
			canInvite = false;
		}

		if(canInvite)
		{
			btnInvite.gameObject.SetActive(true);
			btnInvite2.gameObject.SetActive(true);
		}
		else
		{
			btnInvite.gameObject.SetActive(false);
			btnInvite2.gameObject.SetActive(false);

		}
	}

	void onClickInvite(GameObject go)
	{
		GameManager.me.uiManager.popupFriendInvite.onClickInvite(this);
	}


	
}

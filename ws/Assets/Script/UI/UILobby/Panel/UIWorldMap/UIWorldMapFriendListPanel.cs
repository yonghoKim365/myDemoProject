using UnityEngine;
using System.Collections;
using System;

public class UIWorldMapFriendListPanel : UIListGridItemPanelBase {
	
	public UIButton btn;
	
	public PhotoDownLoader face;

	public UISprite spHart;
	
	public UIDragScrollView scrollView;
	
	protected override void initAwake ()
	{
		UIEventListener.Get(btn.gameObject).onClick = onClick;
		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.uiMenu.uiWorldMap.friendList.panel;
	}

	void OnDestroy()
	{
		btn = null; 
		scrollView = null;
		data = null;
	}

	public override void setPhotoLoad()
	{
		if(data == null) return;
		face.down(epi.GAME_DATA.appFriendDic[data.userId].image_url);
	}	
	
	public P_FriendData data;


	Vector3 _v;
	void onClick(GameObject go)
	{
		_v = GameManager.me.uiManager.uiMenu.uiWorldMap.camWorldCamera.WorldToScreenPoint(transform.position);
		GameManager.me.uiManager.uiMenu.uiWorldMap.friendDetailButton.show(data, _v, UIWorldMapFriendList.isDownArrow);
		//EpiServer.instance.sendGetFriendDetail(data.userId);
	}

	public override void setData(object obj)
	{
		//int rank = _idx+1;
		data = (P_FriendData)obj;
		face.init(epi.GAME_DATA.appFriendDic[data.userId].image_url);
		spHart.enabled = (data.receivedFP == WSDefine.YES);
	}


	public void refresh()
	{
		spHart.enabled = (data.receivedFP == WSDefine.YES);
	}


}

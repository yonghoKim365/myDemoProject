using UnityEngine;
using System.Collections;

public class UIGiftListSlotPanel : UIListGridItemPanelBase {

	public UIButton btn;
	public PhotoDownLoader face;

	public UILabel lbMessage;

	public P_FriendData data;

	public UIDragScrollView scrollView;

	protected override void initAwake ()
	{
		UIEventListener.Get(btn.gameObject).onClick = onSelect;
		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.popupShop.popupGift.list.panel;
	}
	
	void onSelect(GameObject go)
	{
		P_Product p = GameManager.me.uiManager.popupShop.popupGift.selectedGiftItem;

		UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, 
		                   Util.getUIText("CONFIRM_SENDGIFT",p.count.ToString(),epi.GAME_DATA.appFriendDic[data.userId].f_Nick),
		                   onConfirmToSendGift, null, p.price.ToString(), p.priceType);
	}


	void onConfirmToSendGift()
	{
		EpiServer.instance.sendCheckProduct("SHOP", GameManager.me.uiManager.popupShop.popupGift.selectedGiftItem.id, GameManager.me.uiManager.popupShop.popupGift.selectedGiftItem.revision, true, data.userId);
	}


	public override void setPhotoLoad()
	{
		if(data == null) return;
		face.down(epi.GAME_DATA.appFriendDic[data.userId].image_url);
	}	

	public override void setData(object obj)
	{
		P_FriendData d = (P_FriendData)obj;
		
		data = d;
		
		face.init(epi.GAME_DATA.appFriendDic[data.userId].image_url);
		
		lbMessage.text = Util.getUIText("SEND_GIFT", epi.GAME_DATA.appFriendDic[data.userId].f_Nick, Util.getUIText("RUBY"));

	}

}

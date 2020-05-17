using System.Collections;
using UnityEngine;

public class UIRetryPopup : UIPopupBase {
	
	public UILabel lbMsg;
	public UILabel lbPrice, lbLeftTicket;
	public UIButton btnOk;

	public UISprite spPriceType;

	public static bool viewRetryPopup = false;
	
	public override void show ()
	{
		base.show ();

//		if(GameDataManager.instance.ticket > 0)
//		{
//			lbMsg.text = Util.getUIText("RETRY_ROUND",(GameDataManager.instance.failedRound+1)+"");
//			spPriceType.spriteName = WSDefine.ICON_TICKET;
//			spPriceType.MakePixelPerfect();
//			lbPrice.text = "1";
//			lbLeftTicket.text = "(남은 도전권:"+GameDataManager.instance.ticket+")";
//
//		}
//		else
//		{
//			lbMsg.text = Util.getUIText("RETRY_ROUND",(GameDataManager.instance.failedRound+1)+"");
//			lbPrice.text = GameDataManager.instance.retryNeedRuby + "";
//			lbLeftTicket.text = "";
//			spPriceType.spriteName = "img_icn_cash";
//			spPriceType.MakePixelPerfect();
//		}
	}
	
	protected override void awakeInit ()
	{
		UIEventListener.Get(btnOk.gameObject).onClick = onYes;
	}
	
	
	
	protected void onYes (GameObject go)
	{
//		if(GameDataManager.instance.ticket <= 0 && GameDataManager.instance.retryNeedRuby > GameDataManager.instance.ruby)
//		{
//			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
//		}
//		else
//		{
//			gameObject.SetActive(false);
//			retryRound();
//		}
	}
	
	
	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		deniedRetry();
	}
	
	
	void retryRound()
	{
//		EpiServer.instance.sendRetryPlayRound(WSDefine.YES);
	}
	
	void deniedRetry()
	{
//		EpiServer.instance.sendRetryPlayRound(WSDefine.NO);
	}
	
}



using UnityEngine;
using System.Collections;

public class UIGoldForRubyPopup : UIPopupBase {
	
	public UILabel lbMsg;
	public UILabel lbPrice;
	public UIButton btnOk;
	
	
	public PopupData.PopupAction yesCallback = null;
	public PopupData.PopupAction cancelCallback = null;
	
	public override void show ()
	{
		base.show ();
	}
	
	protected override void awakeInit ()
	{
		UIEventListener.Get(btnOk.gameObject).onClick = onYes;
	}
	

	bool _openRubyShop = false;

	private int _gold;

	public void show(int gold, PopupData.PopupAction okCallback)
	{
		GameManager.me.uiManager.menuCamera3.gameObject.SetActive(true);

		//		* 환전은 기준 '루비->골드' 환율 적용 (1루비 = 250골드)
		//		* 환율 적용 계산 후 루비개수의 소수점은 무조건 올림으로 맞춤 (예: 필요 골드 200일 때, 0.8루비 -> 1루비)
		//		* 보유 루비가 부족한 경우 루비 상점 팝업
		_gold = gold;

		int needRuby = Mathf.CeilToInt((float)(_gold - GameDataManager.instance.gold)/GameDataManager.instance.goldForRuby);
		string goldDiff = Util.GetCommaScore(_gold - GameDataManager.instance.gold);

		yesCallback = okCallback;

		_openRubyShop = false;

		if(GameDataManager.instance.gold >= _gold)
		{
			okCallback();
		}
		else if(GameDataManager.instance.ruby < needRuby)
		{
			if(TutorialManager.instance.isTutorialMode) return;

			show();
			lbMsg.text = Util.getUIText("NOTENOUGH_GOLD_BUYRUBY",goldDiff);
			lbPrice.text = needRuby+"";
			lbPrice.color = Color.red;
			_openRubyShop = true;
		}
		else
		{
			if(TutorialManager.instance.isTutorialMode) return;

			show();
			lbMsg.text = Util.getUIText("NOTENOUGH_GOLD_BUYRUBY",goldDiff);
			lbPrice.text = needRuby+"";
			lbPrice.color = Color.white;
		}
	}


	void refreshAfterBuyRuby()
	{
		if(_openRubyShop)
		{
			gameObject.SetActive(true);
		}

		int needRuby = Mathf.CeilToInt((float)(_gold - GameDataManager.instance.gold)/GameDataManager.instance.goldForRuby);
		string goldDiff = Util.GetCommaScore(_gold - GameDataManager.instance.gold);
		
		_openRubyShop = false;
		
		if(GameDataManager.instance.ruby < needRuby)
		{
			lbMsg.text = Util.getUIText("NOTENOUGH_GOLD_BUYRUBY",goldDiff);
			lbPrice.text = needRuby+"";
			lbPrice.color = Color.red;
			_openRubyShop = true;
		}
		else
		{
			lbMsg.text = Util.getUIText("NOTENOUGH_GOLD_BUYRUBY",goldDiff);
			lbPrice.text = needRuby+"";
			lbPrice.color = Color.white;
		}
	}



	protected void onYes (GameObject go)
	{
		if(_openRubyShop)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("GOTO_RUBY_SHOP"), goToRubyShop, null);
		}
		else
		{
			gameObject.SetActive(false);
			if(yesCallback != null) yesCallback();
		}
	}

	void goToRubyShop()
	{
		gameObject.SetActive(false);
		
		if(GameManager.me.uiManager.popupEvolution.gameObject.activeSelf)
		{
			GameManager.me.uiManager.popupEvolution.hideAll();
			
			GameManager.me.uiManager.popupShop.callbackAfterShopClose = null;
		}
		else
		{
			GameManager.me.uiManager.popupShop.callbackAfterShopClose = refreshAfterBuyRuby;
		}

		GameManager.me.uiManager.popupShop.showRubyShop();
	}

	
	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		
	}
	
}

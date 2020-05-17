using UnityEngine;
using System.Collections;

public class UIInvenPlayerTab : MonoBehaviour {

	public UITabButton tabLeo, tabKiley, tabChloe;
	public UISprite spKileyUnlock, spChloeUnlock;

	public UISprite spLeoSelect, spKileySelect, spChloeSelect;

	public delegate void ChangeInvenPlayerTab(string chaId);
	public ChangeInvenPlayerTab tabChangeDispatcher;

	public string currentTab = Character.LEO;

	public GameIDData.Type tabType;

	public UISprite spActLock3, spActLock5;

	void Awake()
	{
		UIEventListener.Get(tabLeo.btn.gameObject).onClick = onClickLeo;
		UIEventListener.Get(tabKiley.btn.gameObject).onClick = onClickKiley;
		UIEventListener.Get(tabChloe.btn.gameObject).onClick = onClickChloe;
	}

	public void init(GameIDData.Type invenType)
	{
		tabType = invenType;
		setSelect();
		changeTab(GameDataManager.instance.selectHeroId);
		refreshUnlock();
	}


	public void refreshUnlock()
	{
		spKileyUnlock.enabled = (GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY) == false);
		spChloeUnlock.enabled = (GameDataManager.instance.serverHeroData.ContainsKey(Character.CHLOE) == false);

		spActLock3.enabled = (GameDataManager.instance.maxAct < 3);
		spActLock5.enabled = (GameDataManager.instance.maxAct < 5);

	}


	UISprite spMain,spSub;

	public void setSelect()
	{
		spMain = null;
		spSub = null;

		switch(GameDataManager.instance.selectHeroId)
		{
		case Character.LEO:
			spMain = spLeoSelect;
			break;
		case Character.KILEY:
			spMain = spKileySelect;
			break;
		case Character.CHLOE:
			spMain = spChloeSelect;
			break;
		}

		if(GameDataManager.instance.selectSubHeroId != null)
		{
			switch(GameDataManager.instance.selectSubHeroId)
			{
			case Character.LEO:
				spSub = spLeoSelect;
				break;
			case Character.KILEY:
				spSub = spKileySelect;
				break;
			case Character.CHLOE:
				spSub = spChloeSelect;
				break;
			}
		}

		if(spMain != null)
		{
			spMain.spriteName = "img_mark_cha_main";
		}

		if(spSub != null)
		{
			spSub.spriteName = "img_mark_cha_sub";
		}

		spLeoSelect.cachedGameObject.SetActive( (spLeoSelect == spMain) || (spLeoSelect == spSub ) );
		spChloeSelect.cachedGameObject.SetActive( (spChloeSelect == spMain) || (spChloeSelect == spSub ) );
		spKileySelect.cachedGameObject.SetActive( (spKileySelect == spMain) || (spKileySelect == spSub ) );
	}

	void onClickLeo(GameObject go)
	{
		if(UIReinforceBarPanel.isReinforceMode || UIMultiSellPanel.isMultiSell || UIComposePanel.isComposeMode)
		{
			return;
		}

		changeTab(Character.LEO);
	}

	void onClickKiley(GameObject go)
	{
		if(UIReinforceBarPanel.isReinforceMode || UIMultiSellPanel.isMultiSell || UIComposePanel.isComposeMode)
		{
			return;
		}



		if(GameDataManager.instance.maxAct >= 3 )
		{
			if(GameDataManager.instance.serverHeroData.ContainsKey(Character.KILEY))
			{
				changeTab(Character.KILEY);
			}
			else
			{
				UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUY_HERO", Util.getUIText("KILEY")), onBuyKiley, null, GameDataManager.instance.heroPrices[Character.KILEY].ToString());
			}
		}
	}


	void onBuyKiley()
	{
		if(GameDataManager.instance.ruby < GameDataManager.instance.heroPrices[Character.KILEY])
		{
			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
		}
		else
		{
			_buyHero = Character.KILEY;
			EpiServer.instance.sendBuyHero(_buyHero, tabType);
		}
	}





	void onClickChloe(GameObject go)
	{
		if(UIReinforceBarPanel.isReinforceMode || UIMultiSellPanel.isMultiSell || UIComposePanel.isComposeMode)
		{
			return;
		}


		if(GameDataManager.instance.maxAct >= 5)
		{
			if(GameDataManager.instance.serverHeroData.ContainsKey(Character.CHLOE))
			{
				changeTab(Character.CHLOE);
			}
			else
			{
				UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUY_HERO", Util.getUIText("CHLOE")), onBuyChloe, null, GameDataManager.instance.heroPrices[Character.CHLOE].ToString());
			}
		}		
	}



	void onBuyChloe()
	{
		if(GameDataManager.instance.ruby < GameDataManager.instance.heroPrices[Character.CHLOE])
		{
			UISystemPopup.open(UISystemPopup.PopupType.GoToRubyShop);
		}
		else
		{
			_buyHero = Character.CHLOE;
			EpiServer.instance.sendBuyHero(_buyHero, tabType);
		}
	}



	private string _buyHero = null;
	public void onCompleteBuyHero()
	{
		if(_buyHero == null) return;
		changeTab(_buyHero);
		refreshUnlock();
	}





	private Vector3 _v;
	public void changeTab(string chaId, bool useCallback = true)
	{
		_v.y = 18; _v.z = 0;

		switch(chaId)
		{
		case Character.LEO:
			tabLeo.isEnabled = true;
			tabKiley.isEnabled = false;
			tabChloe.isEnabled = false;

			_v.x = -3;
			spLeoSelect.transform.localPosition = _v;
			_v.x = 4;
			spChloeSelect.transform.localPosition = _v;
			spKileySelect.transform.localPosition = _v;

			break;

		case Character.KILEY:
			tabLeo.isEnabled = false;
			tabKiley.isEnabled = true;
			tabChloe.isEnabled = false;

			_v.x = -3;
			spKileySelect.transform.localPosition = _v;

			_v.x = 4;
			spChloeSelect.transform.localPosition = _v;
			spLeoSelect.transform.localPosition = _v;
			break;

		case Character.CHLOE:
			tabLeo.isEnabled = false;
			tabKiley.isEnabled = false;
			tabChloe.isEnabled = true;

			_v.x = -3;
			spChloeSelect.transform.localPosition = _v;

			_v.x = 4;
			spLeoSelect.transform.localPosition = _v;
			spKileySelect.transform.localPosition = _v;

			break;
		}

		currentTab = chaId;

		if(useCallback && tabChangeDispatcher != null)
		{
			tabChangeDispatcher(chaId);
		}
	}

}

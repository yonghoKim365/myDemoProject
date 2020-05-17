using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupShop : UIPopupBase 
{
	public JackPotDisplayManager jackpotManager;


	public UILabel lbRuby, lbGold, lbEnergy, lbRuneStone;

	public UIButton btnSpecial;

	public UIButton btnEnergy;
	public UIButton btnEquip;
	public UIButton btnGold;
	public UIButton btnRuby;
	public UIButton btnGift;

	public UISprite spTabSpecial, spTabEnergy, spTabEquip, spTabGold, spTabRuby, spTabGift;
	public UISprite spTabSpecialFont, spTabEnergyFont, spTabEquipFont, spTabGoldFont, spTabRubyFont, spTabGiftFont;

	public UIPopupShopGift popupGift;

	public UIPopupFriendDetail popupFriendDetail;

	public UIShopItemList list;

	public UIShopItemList listEquip;

	public UIShopSpecialItemList listSpecial;

	public UIScrollView scrollView;
	public UIScrollView equipScrollView;


	public UILabel lbTip;
	public UILabel lbCurrentWorld;

	public GameObject goJackpotContainer;

	public UILabel lbMoneyBuyWarning;


	public GameObject goShopBlocker;

	public GameObject prevActiveObject = null;

	public Callback.Default callbackAfterShopClose = null;

	public bool showItemTabBeforeClose = false;

	protected override void awakeInit ()
	{

		UIEventListener.Get(btnSpecial.gameObject).onClick = onClickSpecial;

		UIEventListener.Get(btnEnergy.gameObject).onClick = onClickEnergy;
		UIEventListener.Get(btnEquip.gameObject).onClick = onClickEquip;
		UIEventListener.Get(btnGold.gameObject).onClick = onClickGold;
		UIEventListener.Get(btnRuby.gameObject).onClick = onClickRuby;
		UIEventListener.Get(btnGift.gameObject).onClick = onClickGift;

#if UNITY_ANDROID
		if(PlatformManager.instance != null && PlatformManager.instance.type == PlatformManager.Platform.Kakao)
		{
			btnGift.gameObject.SetActive(false);
		}
#endif



		GameDataManager.goldDispatcher -= updateGold;
		GameDataManager.goldDispatcher += updateGold;
		
		GameDataManager.rubyDispatcher -= updateRuby;
		GameDataManager.rubyDispatcher += updateRuby;

		GameDataManager.priceDataDispatcher -= updateItem;
		GameDataManager.priceDataDispatcher += updateItem;


		GameDataManager.energyDispatcher -= updateEnergy;
		GameDataManager.energyDispatcher += updateEnergy;

		GameDataManager.runeStoneDispatcher -= updateRuneStone;
		GameDataManager.runeStoneDispatcher += updateRuneStone;

		showItemTabBeforeClose = false;

		// for apple review
		if (GameDataManager.instance.annuityProducts == null){
			btnGift.transform.localPosition = btnEnergy.transform.localPosition;
			btnEnergy.transform.localPosition = btnGold.transform.localPosition;
			btnGold.transform.localPosition = btnRuby.transform.localPosition;
			btnRuby.transform.localPosition = btnEquip.transform.localPosition;
			btnEquip.transform.localPosition = btnSpecial.transform.localPosition;
			btnSpecial.gameObject.SetActive(false);
		}

#if UNITY_IPHONE
		btnGift.gameObject.SetActive(false);
#endif

	}




	bool lobbyCharacterCameraEnable = true;

	public static bool SHOUL_REFRESH_INVEN_LIST = false;

	bool _fromLobby = false;

	public override void show ()
	{

		if(GameManager.me.isGuest && GameDataManager.instance.serviceMode != GameDataManager.ServiceMode.IOS_SUMMIT)
		{
			btnRuby.gameObject.SetActive(false);
			btnSpecial.gameObject.SetActive(false);

			btnEquip.transform.localPosition = new Vector3(102.3992f, -8.326538f, 166.7005f);
			btnGold.transform.localPosition = new Vector3(241.2111f, -8.326538f, 166.7005f);
			btnEnergy.transform.localPosition = new Vector3(379.0906f, -8.326538f, 166.7005f);
		}


		if(string.IsNullOrEmpty(lbMoneyBuyWarning.text))
		{
#if UNITY_IOS
			lbMoneyBuyWarning.text = Util.getUIText("MONEY_BUY_WARNING_I");
#else
			lbMoneyBuyWarning.text = Util.getUIText("MONEY_BUY_WARNING_A");
#endif
		}


		// 연산을 조금이라도 줄이기위한...
		GameManager.me.uiManager.uiMenuCamera.enabled = false;
		GameManager.me.uiManager.uiMenu.camera.enabled = false;
		GameManager.me.uiManager.uiMenu.uiLobby.chracterCamera.enabled = false;

		if(_fromLobby == false)
		{
			_fromLobby = (GameManager.me.uiManager.uiMenu.uiLobby.gameObject.activeSelf);
		}

		if(_fromLobby) GameManager.me.uiManager.uiMenu.uiLobby.gameObject.SetActive(false);


		SHOUL_REFRESH_INVEN_LIST = false;

		base.show ();
		goShopBlocker.SetActive(false);

		popupGift.hide();
		popupFriendDetail.hide();

		updateGold();
		updateRuby();
		updateItem();
		updateEnergy();
		updateRuneStone();

		lbCurrentWorld.text = Util.getUIText("SHOP_CURRENT_WORLD",GameDataManager.instance.maxAct.ToString(),GameDataManager.instance.maxStage.ToString(),(GameDataManager.instance.maxRound).ToString());


		EpiServer.instance.sendGetJackPotUsers();

		GameManager.me.specialPackageManager.check();

	}


	protected override void onClickClose (GameObject go)
	{
		if(showItemTabBeforeClose && UIShopItemList.type == ShopItem.Type.ruby)
		{
			onClickEquip(null);
			showItemTabBeforeClose = false;
			return;
		}

		GameManager.me.uiManager.uiMenuCamera.enabled = true;
		GameManager.me.uiManager.uiMenu.camera.enabled = true;
		GameManager.me.uiManager.uiMenu.uiLobby.chracterCamera.enabled = true;

		base.onClickClose (go);

		if(SHOUL_REFRESH_INVEN_LIST)
		{
			if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.HERO)
			{
				GameManager.me.uiManager.uiMenu.uiHero.refreshList();
			}
			else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SKILL)
			{
				GameManager.me.uiManager.uiMenu.uiSkill.refreshSkillInven();
			}
			else if(GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.SUMMON)
			{
				GameManager.me.uiManager.uiMenu.uiSummon.refreshUnitInven();
			}
		}

		if(callbackAfterShopClose != null)
		{
			callbackAfterShopClose();
			callbackAfterShopClose = null;
		}

	}


	public void showGoldShop()
	{
		show ();
		onClickGold(null);
	}

	public void showRubyShop()
	{

#if UNITY_IOS
		if(GameManager.me.isGuest && GameDataManager.instance.serviceMode != GameDataManager.ServiceMode.IOS_SUMMIT)
		{
			UISystemPopup.open("게스트 모드일 때는 루비구매를 하실 수 없습니다.");
			return;
		}
#endif

		show ();
		onClickRuby(null);
	}

	public void showGiftShop()
	{
		#if UNITY_IOS
		if(GameManager.me.isGuest && GameDataManager.instance.serviceMode != GameDataManager.ServiceMode.IOS_SUMMIT)
		{
			UISystemPopup.open("게스트 모드일 때는 루비구매를 하실 수 없습니다.");
			return;
		}
		#endif

		show ();
		onClickGift(null);
	}


	public void showItemShop()
	{
		show ();
		onClickEquip(null);
	}


	public void showEnergyShop()
	{
		show ();
		onClickEnergy(null);
	}

	public void showSpecialShop()
	{

//		>> 비활성 상태
//		* UI : 구매 버튼 표시 (패키지 상품처럼 버튼 위에 가격 표시하기)
//		* 구입 완료시, 활성 & 수령 가능 상태로 즉시 변경
//
//		>> 활성 & 수령 가능 상태
//		* UI : 루비 수령 버튼(루비수 넣기), 최대 획득 가능일 표시
//		* 수령 완료시, 활성 & 수령 불가 상태로 즉시 변경
//		(마지막날이었다면 비활성 상태로 즉시 변경)
//
//		>> 활성 & 수령 불가 상태
//		* UI : 금일 자정까지의 남은 시간, 최대 획득 가능일 표시


//		- 스페셜탭으로 진입 : 비활성 상태 또는 활성 & 수령 가능 상태
//		- 뽑기탭으로 진입 :  튜토리얼 중 또는 활성 & 수령 불가 상태

		if(GameManager.me.isGuest == false && TutorialManager.instance.isTutorialMode == false && GameDataManager.instance.annuityProducts != null)
		{
			int index = 0;
			foreach(KeyValuePair<string, P_Annuity> kv in GameDataManager.instance.annuityProducts)
			{
				if(kv.Value.didBuy == WSDefine.YES && kv.Value.didConsume == WSDefine.FALSE)
				{
					show ();
					clickSpecial();
					updateSpecial(index);
					return;
				}

				++index;
			}
		}

		showItemShop();
	}




	public override void hide (bool isInit = false)
	{
		if( (GameManager.me != null && GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.LOBBY && GameManager.me.uiManager.uiMenu.uiLobby.gameObject.activeSelf == false) ||  _fromLobby)
		{
			if(isInit == false)
			{
				GameManager.me.uiManager.uiMenu.uiLobby.gameObject.SetActive(true);
				GameManager.me.uiManager.uiMenu.uiLobby.checkLobbyMap();
			}
		}

		_fromLobby = false;

		base.hide (isInit);
		goShopBlocker.gameObject.SetActive(false);
		onClickGold(null);

		if(isInit == false && GameManager.me != null && GameManager.me.uiManager.uiMenu.currentPanel == UIMenu.LOBBY)
		{
			GameManager.me.uiManager.uiMenu.uiLobby.checkInventoryLimit();
		}
	}


	public void initHide()
	{
		_fromLobby = false;
		
		base.hide ();
		goShopBlocker.gameObject.SetActive(false);
		onClickGold(null);
	}




	void updateGold()
	{
		lbGold.text = Util.GetCommaScore(GameDataManager.instance.gold);
	}
	
	void updateRuby()
	{
		lbRuby.text = Util.GetCommaScore(GameDataManager.instance.ruby);
	}

	void updateEnergy()
	{
		lbEnergy.text = Util.GetCommaScore(GameDataManager.instance.energy);
	}


	void updateRuneStone()
	{
		lbRuneStone.text = Util.GetCommaScore(GameDataManager.instance.runeStone);
	}


	void updateItem()
	{
		if(GameDataManager.instance.shopData == null || gameObject.activeInHierarchy == false) return;

		list.gameObject.SetActive(true);
		listEquip.gameObject.SetActive(false);
		listSpecial.gameObject.SetActive(false);

		if(UIShopItemList.type == ShopItem.Type.ruby || UIShopItemList.type == ShopItem.Type.gift)
		{
			lbMoneyBuyWarning.gameObject.SetActive(true);
			goJackpotContainer.SetActive(false);
		}
		else
		{
			lbMoneyBuyWarning.gameObject.SetActive(false);
			goJackpotContainer.SetActive(true);
		}

		list.draw();
	}


	void updateSpecial(int index = -1)
	{
		if(GameDataManager.instance.shopData == null || gameObject.activeInHierarchy == false) return;
		list.gameObject.SetActive(false);
		listEquip.gameObject.SetActive(false);

		lbMoneyBuyWarning.gameObject.SetActive(true);
		goJackpotContainer.SetActive(false);

		listSpecial.gameObject.SetActive(true);

		if(index > 0)
		{
			listSpecial.draw(true,index);
		}
		else
		{
			listSpecial.draw();
		}
	}


	void updateEquipItem()
	{
		if(GameDataManager.instance.shopData == null || gameObject.activeInHierarchy == false) return;
		list.gameObject.SetActive(false);
		listEquip.gameObject.SetActive(true);
		listSpecial.gameObject.SetActive(false);

		lbMoneyBuyWarning.gameObject.SetActive(false);
		goJackpotContainer.SetActive(true);


		// 장비탭 프리미엄 1장 상품 페이지로 바로 보내기
		if(TutorialManager.nowPlayingTutorial("T44") )
		{
			listEquip.draw(true,getPageNumFromItemList("ITEM_P1_UNIT"));
		}
		//// 장비탭 프리미엄 1장 상품 페이지로 바로 보내기
		else if(TutorialManager.nowPlayingTutorial("T45"))
		{
			listEquip.draw(true,getPageNumFromItemList("ITEM_P1_SKILL"));
		}
		else
		{
			listEquip.draw();
		}
	}



	int getPageNumFromItemList(string id)
	{
		int i = 0;

		foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.itemProducts)
		{
			if(kv.Value.id == id)
			{
				break;
			}

			++i;
		}

		return (i/3);
	}




	public void refresh()
	{
		if(gameObject.activeInHierarchy)
		{
			if(list.gameObject.activeSelf) list.draw();
			else listEquip.draw();
		}
	}


	const string TAB_ON_IDLE = "ibtn_tab_onidle";
	const string TAB_OFF_IDLE = "ibtn_tab_offidle";



	public void onClickSpecial(GameObject go)
	{
		clickSpecial();
		updateSpecial();
	}

	void clickSpecial()
	{
		showItemTabBeforeClose = false;
		
		spTabSpecial.spriteName = TAB_ON_IDLE;
		spTabEnergy.spriteName = TAB_OFF_IDLE;
		spTabEquip.spriteName = TAB_OFF_IDLE;
		spTabGold.spriteName = TAB_OFF_IDLE;
		spTabRuby.spriteName = TAB_OFF_IDLE;
		spTabGift.spriteName = TAB_OFF_IDLE;
		
		spTabSpecialFont.spriteName = "ibtn_tab_special_onidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_offidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_offidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_offidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_offidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_offidle";
		
		btnSpecial.enabled = false;
		btnEnergy.enabled = true;
		btnEquip.enabled = true;
		btnGold.enabled = true;
		btnRuby.enabled = true;
	}



	public void onClickEnergy(GameObject go)
	{
		showItemTabBeforeClose = false;

		spTabSpecial.spriteName = TAB_OFF_IDLE;
		spTabEnergy.spriteName = TAB_ON_IDLE;
		spTabEquip.spriteName = TAB_OFF_IDLE;
		spTabGold.spriteName = TAB_OFF_IDLE;
		spTabRuby.spriteName = TAB_OFF_IDLE;
		spTabGift.spriteName = TAB_OFF_IDLE;

		spTabSpecialFont.spriteName = "ibtn_tab_special_offidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_onidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_offidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_offidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_offidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_offidle";

		btnSpecial.enabled = true;
		btnEnergy.enabled = false;
		btnEquip.enabled = true;
		btnGold.enabled = true;
		btnRuby.enabled = true;


		UIShopItemList.type = ShopItem.Type.energy;
		
		updateItem();
	}


	public void onClickEquip(GameObject go)
	{
		showItemTabBeforeClose = false;

		spTabSpecial.spriteName = TAB_OFF_IDLE;
		spTabEnergy.spriteName = TAB_OFF_IDLE;
		spTabEquip.spriteName = TAB_ON_IDLE;
		spTabGold.spriteName = TAB_OFF_IDLE;
		spTabRuby.spriteName = TAB_OFF_IDLE;
		spTabGift.spriteName = TAB_OFF_IDLE;

		spTabSpecialFont.spriteName = "ibtn_tab_special_offidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_offidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_onidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_offidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_offidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_offidle";

		btnSpecial.enabled = true;
		btnEnergy.enabled = true;
		btnEquip.enabled = false;
		btnGold.enabled = true;
		btnRuby.enabled = true;
		
		UIShopItemList.type = ShopItem.Type.item;
		
		updateEquipItem();
	}


	public void onClickGold(GameObject go)
	{
		showItemTabBeforeClose = false;

		spTabSpecial.spriteName = TAB_OFF_IDLE;
		spTabEnergy.spriteName = TAB_OFF_IDLE;
		spTabEquip.spriteName = TAB_OFF_IDLE;
		spTabGold.spriteName = TAB_ON_IDLE;
		spTabRuby.spriteName = TAB_OFF_IDLE;
		spTabGift.spriteName = TAB_OFF_IDLE;

		spTabSpecialFont.spriteName = "ibtn_tab_special_offidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_offidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_offidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_onidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_offidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_offidle";

		btnSpecial.enabled = true;
		btnEnergy.enabled = true;
		btnEquip.enabled = true;
		btnGold.enabled = false;
		btnRuby.enabled = true;

		UIShopItemList.type = ShopItem.Type.gold;

		updateItem();
	}

	public void onClickRuby(GameObject go)
	{
		if(go != null) showItemTabBeforeClose = false;

		spTabSpecial.spriteName = TAB_OFF_IDLE;
		spTabEnergy.spriteName = TAB_OFF_IDLE;
		spTabEquip.spriteName = TAB_OFF_IDLE;
		spTabGold.spriteName = TAB_OFF_IDLE;
		spTabRuby.spriteName = TAB_ON_IDLE;
		spTabGift.spriteName = TAB_OFF_IDLE;

		spTabSpecialFont.spriteName = "ibtn_tab_special_offidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_offidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_offidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_offidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_onidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_offidle";

		btnSpecial.enabled = true;
		btnEnergy.enabled = true;
		btnEquip.enabled = true;
		btnGold.enabled = true;
		btnRuby.enabled = false;

		UIShopItemList.type = ShopItem.Type.ruby;

		updateItem();
	}

	void onClickGift(GameObject go)
	{
		if(go != null) showItemTabBeforeClose = false;

		spTabSpecial.spriteName = TAB_OFF_IDLE;
		spTabEnergy.spriteName = TAB_OFF_IDLE;
		spTabEquip.spriteName = TAB_OFF_IDLE;
		spTabGold.spriteName = TAB_OFF_IDLE;
		spTabRuby.spriteName = TAB_OFF_IDLE;
		spTabGift.spriteName = TAB_ON_IDLE;

		spTabSpecialFont.spriteName = "ibtn_tab_special_offidle";
		spTabEnergyFont.spriteName = "ibtn_tab_energy_offidle";
		spTabEquipFont.spriteName = "ibtn_tab_item_offidle";
		spTabGoldFont.spriteName = "ibtn_tab_gold_offidle";
		spTabRubyFont.spriteName = "ibtn_tab_ruby_offidle";
		spTabGiftFont.spriteName = "ibtn_tab_gift_onidle";

		btnSpecial.enabled = true;
		btnEnergy.enabled = true;
		btnEquip.enabled = true;
		btnGold.enabled = true;
		btnRuby.enabled = false;
		
		UIShopItemList.type = ShopItem.Type.gift;
		
		updateItem();
	}




	void LateUpdate()
	{

		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(btnClose.isEnabled == false) return;
			if(GameManager.me.uiManager.uiMenu.rayCast(transform.parent.camera, btnClose.gameObject) == false) return;
			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;

			onClickClose(null);

			return;
		}
	}

}

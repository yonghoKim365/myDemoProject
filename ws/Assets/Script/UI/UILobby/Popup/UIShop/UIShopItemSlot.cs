using UnityEngine;
using System.Collections;

public class UIShopItemSlot : UIListGridItemPanelBase {

	public GameObject goNormalContainer;
	public GameObject goItemContainer;

	// 사진이 아닐때는 얘가 활성화.
	public GameObject goDefaultContainer;

	// 사진일때는 얘가 활성화.
	public PhotoDownLoader photo;

	public UISprite spCenterIcon, spRibon;
	public UISprite spPriceIcon, spBonus, spGiftWord;

	public UILabel lbPrice, lbBonusNum, lbCount, lbPrefix, lbItemName, lbSubTitle, lbItemCount, lbNormalItemName;

	public UIButton btn;

	public UIDragScrollView scrollView;


	string _currentItemName = "";

	const float _bonusNumberYPos = 87.0f;


	protected override void initAwake ()
	{
		UIEventListener.Get(btn.gameObject).onClick = onClickSlot;

		if(scrollView.scrollView == null)
		{
			scrollView.scrollView = GameManager.me.uiManager.popupShop.scrollView;
		}
	}


	public override void setPhotoLoad ()
	{
		if(_imgUrl != null)
		{
			photo.down( _imgUrl );	
		}
	}


	void onClickSlot(GameObject go)
	{
		switch(_type)
		{

		case  ShopItem.Type.energy:

			if(_title.Length == 1)
			{
				UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYSHOPITEM",_nowItem.price.ToString(),_title[0]), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);
			}
			else if(_title.Length > 1)
			{

				if(GameDataManager.instance.roundClearStatusCheck(1,2,3) == false)
				{
					UISystemPopup.open( UISystemPopup.PopupType.Default,Util.getUIText("BUY_AFTER_STAGE2"));
					return;
				}
				if(_title[1].ToLower().Contains("bonus"))
				{
					UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYSHOPITEM_ENERGY",_nowItem.price.ToString(),_title[0]), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);
				}
				else
				{
					UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYSHOPITEM",_nowItem.price.ToString(),_title[0]+","+_title[1].Replace("+","")), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);
				}

			}
			break;

		case  ShopItem.Type.gold:
			UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYGOLDRUBY",_nowItem.price.ToString(),_nowItem.count.ToString()), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);
			break;

		case  ShopItem.Type.ruby:

			#if UNITY_IOS
			if(GameManager.me.isGuest && GameDataManager.instance.serviceMode != GameDataManager.ServiceMode.IOS_SUMMIT)
			{
				UISystemPopup.open("게스트 모드일 때는 루비구매를 하실 수 없습니다.");
				return;
			}
			#endif

			UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUYRUBY",_nowItem.count.ToString()), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);
			break;


		case  ShopItem.Type.gift:

			GameManager.me.uiManager.popupShop.popupGift.selectedGiftItem = _nowItem;

			GameManager.me.uiManager.popupShop.popupGift.show();
			break;


		case ShopItem.Type.item:

			switch(_nowItem.subcategory)
			{
			case "UNITRUNE":

				if(TutorialManager.nowPlayingTutorial("T44") || TutorialManager.nowPlayingTutorial("T48"))
				{
					onConfirmBuy2();
					return;
				}

#if UNITY_EDITOR
				else if(TutorialManager.instance.useTutorialDebugMode == false)
				{

				}
#endif
				else if( (TutorialManager.instance.clearCheck("T44") == false && !(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == "T44")) || GameDataManager.instance.roundClearStatusCheck(1,1,2) == false)
				{
					UISystemPopup.open ( UISystemPopup.PopupType.Default, Util.getUIText("BUY_AFTER_SUMMON_TUTO"));
					return;
				}

				break;

			case "SKILLRUNE":

				if(TutorialManager.nowPlayingTutorial("T45",11) || TutorialManager.nowPlayingTutorial("T49"))
				{
					onConfirmBuy2();
					return;
				}
				#if UNITY_EDITOR
				else if(TutorialManager.instance.useTutorialDebugMode == false)
				{
					
				}
				#endif
				else if( (TutorialManager.instance.clearCheck("T45") == false && !(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == "T45")) || GameDataManager.instance.roundClearStatusCheck(1,1,3) == false)
				{
					UISystemPopup.open ( UISystemPopup.PopupType.Default, Util.getUIText("BUY_AFTER_SKILL_TUTO"));
					return;
				}

				break;

			case "EQUIPMENT":

				if(TutorialManager.nowPlayingTutorial("T50"))
				{
					onConfirmBuy2();
					return;
				}
				#if UNITY_EDITOR
				else if(TutorialManager.instance.useTutorialDebugMode == false)
				{
					
				}
				#endif
				else if( (TutorialManager.instance.clearCheck("T46") == false && !(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == "T46")) || GameDataManager.instance.roundClearStatusCheck(1,2,3) == false)
				{
					UISystemPopup.open ( UISystemPopup.PopupType.Default, Util.getUIText("BUY_AFTER_EQUIP_TUTO"));
					return;
				}

				break;
			}




			if(_nowItem.priceType == WSDefine.GOLD)
			{
				UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, 
				                   Util.getUIText("BUY_ITEM_SHOP_GOLD",_nowItem.price.ToString(), (_title[0] + ((_title.Length > 1)?(" " + _title[1]):"") )), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);

			}
			else
			{

				UISystemPopup.open( UISystemPopup.PopupType.YesNoPrice, Util.getUIText("BUY_ITEM_SHOP_RUBY",_nowItem.price.ToString(),(_title[0] + ((_title.Length > 1)?(" " + _title[1]):"") )), onConfirmBuy, null, _nowItem.price.ToString(), _nowItem.priceType);

			}

			break;
		}
	}


	void onConfirmBuy()
	{

		if(_nowItem.priceType == WSDefine.GOLD)
		{
			if(GameDataManager.priceCheck(ShopItem.Type.gold, _nowItem.price) == false)
			{
				return;
			}
		}
		else if(_nowItem.priceType == WSDefine.RUBY)
		{
			if(GameDataManager.priceCheck(ShopItem.Type.ruby, _nowItem.price) == false)
			{

				if(_type == ShopItem.Type.item)
				{
					GameManager.me.uiManager.popupShop.showItemTabBeforeClose = true;
				}

				return;
			}
		}

		onConfirmBuy2();
	}

	void onConfirmBuy2()
	{
		switch(_type)
		{
		case ShopItem.Type.energy:

			EpiServer.instance.sendBuyEnergy(_nowItem.id, _nowItem.revision);
			break;

		case  ShopItem.Type.gold:
			EpiServer.instance.sendBuyGold(_nowItem.id, _nowItem.revision);
			break;
		case  ShopItem.Type.ruby:

			EpiServer.instance.sendCheckProduct("SHOP", _nowItem.id, _nowItem.revision, false, "");

			break;

		case ShopItem.Type.item:

			EpiServer.instance.sendBuyItem(_nowItem.id, _nowItem.revision, null, _currentItemName);
			break;
		}
	}


	public UISprite spEquipTitleRibon, spEquipIcon;
	public UILabel lbEquipMiddleMsg;


	private ShopItem.Type _type;
	private P_Product _nowItem;
	private string[] _title;

	private Vector3 _v = new Vector3();
	private string _imgUrl = null;

	public override void setData (object obj)
	{
		if(obj == null)
		{
			this.gameObject.SetActive(false);
			return;
		}
		else
		{
			this.gameObject.SetActive(true);
		}

		lbBonusNum.enabled = false;

		_type = UIShopItemList.type;
		_nowItem = (P_Product)obj;

		if(string.IsNullOrEmpty(_nowItem.imgUrl) == false && _nowItem.imgUrl.StartsWith("http"))
		{
			photo.gameObject.SetActive(true);
			goDefaultContainer.SetActive(false);
			goNormalContainer.SetActive(false);
			goItemContainer.SetActive(false);

			_imgUrl = _nowItem.imgUrl;

			photo.init(_nowItem.imgUrl);

			_title = _nowItem.desc.Split('/');

			switch(_type)
			{
			case ShopItem.Type.item:

				if(TutorialManager.instance.isTutorialMode)
				{
					if(TutorialManager.nowPlayingTutorial("T44") && _nowItem.id == "ITEM_P1_UNIT")
					{
						btn.gameObject.name = "P";
					}
					else if(TutorialManager.nowPlayingTutorial("T45") && _nowItem.id == "ITEM_P1_SKILL")
					{
						btn.gameObject.name = "P";
					}
					else
					{
						btn.gameObject.name = string.Empty;
					}
				}

				if(_title.Length == 3 || _title.Length == 4)
				{
					_currentItemName = _title[0] + " " + _title[1];
				}

				break;
			}


			return;
		}

		_imgUrl = null;

		_title = _nowItem.desc.Split('/');

		goDefaultContainer.SetActive(true);
		photo.gameObject.SetActive(false);


		if(_nowItem.priceType == PRICE_TYPE_MONEY)
		{
			#if UNITY_ANDROID
			lbPrice.text = Util.GetCommaScore( Mathf.RoundToInt( _nowItem.price ) );
			#else
			lbPrice.text = string.Format("{0:F2}", _nowItem.price);
			#endif
		}
		else
		{
			lbPrice.text = Util.GetCommaScore( Mathf.RoundToInt( _nowItem.price ) );
		}

		spPriceIcon.spriteName = getPriceType(_nowItem.priceType);
		spPriceIcon.MakePixelPerfect();

		spGiftWord.enabled = _type == ShopItem.Type.gift;

		switch(_type)
		{

		case  ShopItem.Type.energy:

			goNormalContainer.gameObject.SetActive(true);
			goItemContainer.gameObject.SetActive(false);
			
			spRibon.spriteName = "img_buy_ruby";
			
			spCenterIcon.spriteName = "img_buy_energy_0"+((index+1 < 5)?index+1:5);
			spCenterIcon.MakePixelPerfect();

			lbNormalItemName.enabled = true;
			lbNormalItemName.text = _title[0];//Util.getUIText("ENERGY",_nowItem.count.ToString());
			lbCount.enabled = false;


			_v = spPriceIcon.transform.localPosition;
			_v.x = 7; _v.y = -147;
			spPriceIcon.transform.localPosition = _v;

			_v = lbPrice.transform.localPosition;
			_v.x = 68; _v.y = -149;
			lbPrice.transform.localPosition = _v;

			setBonusLabelAndSprite();

			break;
		case  ShopItem.Type.gold:

			goNormalContainer.gameObject.SetActive(true);
			goItemContainer.gameObject.SetActive(false);

			spRibon.spriteName = "img_buy_ruby";

			spCenterIcon.spriteName = "img_buy_gold_0"+((index+1 < 5)?index+1:5);
			spCenterIcon.MakePixelPerfect();

			lbNormalItemName.enabled = false;
			lbCount.enabled = true;

			lbCount.text = Util.GetCommaScore(_nowItem.count);

			setBonusLabelAndSprite();

			_v = spPriceIcon.transform.localPosition;
			_v.x = 7; _v.y = -147;
			spPriceIcon.transform.localPosition = _v;

			_v = lbPrice.transform.localPosition;
			_v.x = 68; _v.y = -149;
			lbPrice.transform.localPosition = _v;

			break;

		case ShopItem.Type.ruby:
		case ShopItem.Type.gift:

			goNormalContainer.gameObject.SetActive(true);
			goItemContainer.gameObject.SetActive(false);

			spRibon.spriteName = "img_buy_ruby";

			spCenterIcon.spriteName = "img_buy_ruby_0"+((index+1 < 5)?index+1:5);
			spCenterIcon.MakePixelPerfect();

			lbNormalItemName.enabled = false;
			lbCount.enabled = true;

			lbCount.text = Util.GetCommaScore(_nowItem.count);


			
			_v = lbPrice.transform.localPosition;
			_v.x = 68; _v.y = -149;
			lbPrice.transform.localPosition = _v;


			_v = spPriceIcon.transform.localPosition;
			_v.x =  lbPrice.transform.localPosition.x - lbPrice.printedSize.x * 0.5f - 30f ;//7; 
			_v.y = -147;
			spPriceIcon.transform.localPosition = _v;

			setBonusLabelAndSprite();

			break;

		case ShopItem.Type.item:

			if(TutorialManager.instance.isTutorialMode)
			{
				btn.gameObject.name = string.Empty;
			}

			goNormalContainer.gameObject.SetActive(false);
			goItemContainer.gameObject.SetActive(true);

			char[] ta = _nowItem.id.ToCharArray();
			bool isNormal = (ta[5] == 'N');
			bool isPackage = (ta[6] != '1');

			if(TutorialManager.nowPlayingTutorial("T44"))
			{
				btn.gameObject.name = "";
			}

			switch(ta[8])
			{

			case 'U':
				if(isPackage && !isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premiumset_01";
				}
				else if(!isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premium_01";

					if(TutorialManager.nowPlayingTutorial("T44") && _nowItem.id == "ITEM_P1_UNIT")
					{
						btn.gameObject.name = "P";
					}
				}
				else
				{
					spEquipIcon.spriteName = "img_buy_normal_01";
					if(TutorialManager.nowPlayingTutorial("T44") )
					{
						btn.gameObject.name = "N";
					}
				}
				break;

			case 'S':
				if(isPackage && !isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premiumset_02";
				}
				else if(!isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premium_02";

					if(TutorialManager.nowPlayingTutorial("T45") && _nowItem.id == "ITEM_P1_SKILL")
					{
						btn.gameObject.name = "P";
					}
				}
				else
				{
					spEquipIcon.spriteName = "img_buy_normal_02";
				}
				break;



			case 'E':
				if(isPackage && !isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premiumset_03";
				}
				else if(!isNormal)
				{
					spEquipIcon.spriteName = "img_buy_premium_03";

					if(TutorialManager.nowPlayingTutorial("T50"))
					{
						btn.gameObject.name = "P";
					}

				}
				else spEquipIcon.spriteName = "img_buy_normal_03";
				break;

			}


			switch(_title.Length)
			{
			case 3:
				lbPrefix.text = _title[0];
				lbItemName.text = _title[1];
				lbEquipMiddleMsg.text = _title[2];

				_currentItemName = _title[0] + " " + _title[1];

				break;
			case 4: 
				lbPrefix.text = _title[0];

				lbItemName.text = _title[1];
				lbItemCount.text = _title[2];
				lbEquipMiddleMsg.text = _title[3];

				_currentItemName = _title[0] + " " + _title[1];
				break;
			}

			lbItemCount.enabled = (isPackage && !isNormal);

			_v = lbPrefix.transform.localPosition;
			_v.x = 52; _v.y = 148;
			lbPrefix.transform.localPosition = _v;
			
			_v = lbPrefix.transform.localScale;
			_v.x = 1.1f; _v.y = 1.1f;
			lbPrefix.transform.localScale = _v;
			
			_v = lbItemName.transform.localPosition;
			_v.x = 53; _v.y = 118;
			lbItemName.transform.localPosition = _v;



			spEquipTitleRibon.spriteName = (isNormal)?"img_buy_normal":"img_buy_premium";

			spEquipIcon.MakePixelPerfect();


			_v = spPriceIcon.transform.localPosition;
			_v.x = 22; _v.y = -147;
			spPriceIcon.transform.localPosition = _v;

			_v = lbPrice.transform.localPosition;
			_v.x = 77; _v.y = -149;
			lbPrice.transform.localPosition = _v;

			break;
		}
	}



	void setBonusLabelAndSprite(bool headerColorCheck = false)
	{
		if(_title.Length == 2 )
		{
			if(_title[1].ToUpper().Contains("BONUS"))
			{
				lbBonusNum.enabled = false;
				spBonus.enabled = true;
				
				_v = spBonus.cachedTransform.localPosition;
				_v.y = 87;
				spBonus.cachedTransform.localPosition = _v;
			}
			else
			{
				lbBonusNum.enabled = true;

				if(headerColorCheck)
				{
					if(_title[1].Contains(WSDefine.PREMIUM_HEADER))
					{
						lbBonusNum.text = "[8d5699]"+_title[1]+"[-]";
					}
					else if(_title[1].Contains(WSDefine.NORMAL_HEADER))
					{
						lbBonusNum.text = "[b7ef56]"+_title[1]+"[-]";
					}
					else
					{
						lbBonusNum.text = _title[1];
					}
				}
				else
				{
					lbBonusNum.text = _title[1];
				}



				spBonus.enabled = false;
				
				_v = lbBonusNum.cachedTransform.localPosition;
				_v.y = 87;
				lbBonusNum.cachedTransform.localPosition = _v;
			}
			
		}
		else if(_title.Length == 3 )
		{
			lbBonusNum.enabled = true;
			spBonus.enabled = true;
			
			if(_title[1].ToUpper().Contains("BONUS"))
			{
				if(headerColorCheck)
				{
					if(_title[2].Contains(WSDefine.PREMIUM_HEADER))
					{
						lbBonusNum.text = "[8d5699]"+_title[2]+"[-]";
					}
					else if(_title[2].Contains(WSDefine.NORMAL_HEADER))
					{
						lbBonusNum.text = "[b7ef56]"+_title[2]+"[-]";
					}
					else
					{
						lbBonusNum.text = _title[2];
					}
				}
				else
				{
					lbBonusNum.text = _title[2];
				}
				
				_v = lbBonusNum.cachedTransform.localPosition;
				_v.y = 62;
				lbBonusNum.cachedTransform.localPosition = _v;
				
				_v = spBonus.cachedTransform.localPosition;
				_v.y = 87;
				spBonus.cachedTransform.localPosition = _v;
			}
			else
			{
				if(headerColorCheck)
				{
					if(_title[1].Contains(WSDefine.PREMIUM_HEADER))
					{
						lbBonusNum.text = "[8d5699]"+_title[1]+"[-]";
					}
					else if(_title[1].Contains(WSDefine.NORMAL_HEADER))
					{
						lbBonusNum.text = "[b7ef56]"+_title[1]+"[-]";
					}
					else
					{
						lbBonusNum.text = _title[1];
					}
				}
				else
				{
					lbBonusNum.text = _title[1];
				}
				
				_v = lbBonusNum.cachedTransform.localPosition;
				_v.y = 87;
				lbBonusNum.cachedTransform.localPosition = _v;
				
				_v = spBonus.cachedTransform.localPosition;
				_v.y = 62;
				spBonus.cachedTransform.localPosition = _v;
			}
		}
		else
		{
			spBonus.enabled = false;
		}
	}




	const string PRICE_TYPE_MONEY = "MONEY";
	const string PRICE_TYPE_GOLD = "GOLD";
	const string PRICE_TYPE_RUBY = "RUBY";

	public const string ICON_WON = "icn_won";
	public const string ICON_DOLLAR = "icn_dollar";

	public static string getPriceType(string priceType)
	{
		switch(priceType)
		{
		case PRICE_TYPE_MONEY:
			#if UNITY_ANDROID
			return "icn_won";
			#else
			return "icn_dollar";
			#endif

			break;
		case PRICE_TYPE_GOLD:
			return WSDefine.ICON_GOLD;

		case PRICE_TYPE_RUBY:
			return WSDefine.ICON_RUBY;
		}

		return string.Empty;
	}

}


public class ShopItem
{
	public enum Type
	{
		item, gold, ruby, energy, gift
	}
}
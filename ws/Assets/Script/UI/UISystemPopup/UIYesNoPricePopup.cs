using UnityEngine;
using System.Collections;

public class UIYesNoPricePopup : UISystemPopupBase {

	public UILabel lbPrice;

	public UISprite spPriceType;

	public override void show (PopupData pd, string msg)
	{
		base.show (pd, msg);
		lbPrice.text = pd.data[0].ToString();
	}

	public void setPriceType(string priceType)
	{
		if(priceType == null) priceType = "RUBY";
		spPriceType.spriteName = UIShopItemSlot.getPriceType(priceType);
	}
}

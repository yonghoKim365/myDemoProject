using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UITutorialComposePanel : MonoBehaviour 
{
	public UIChallengeItemSlot slotOriginal;
	public UIChallengeItemSlot slotSource;
	public UIChallengeItemSlot slotResult;

	public UILabel lbPrice;

	public void setData (string original, string source)
	{
		GameIDData originalData = new GameIDData();
		GameIDData slotData = new GameIDData();

		originalData.parse(original);
		slotData.parse(source);

		slotOriginal.setData(original);
		slotSource.setData(source);

		slotSource.setData(UIChallengeItemSlot.Type.Unit, slotData);
		int price = GameDataManager.instance.composePrices["UNIT" + RareType.SERVER_CHARACTER[originalData.rare]];

		slotResult.gameObject.SetActive(true);
		slotResult.setRare(originalData.rare + 1);
		slotResult.setLevel(1);
		slotResult.spBackground.gameObject.SetActive(true);
		slotResult.spBackground.spriteName = UIHeroInventorySlot.SLOT_BG_QUESTION;

		lbPrice.text = Util.GetCommaScore(price);
	}

}


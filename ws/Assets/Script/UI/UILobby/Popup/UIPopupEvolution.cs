using UnityEngine;
using System.Collections;

public class UIPopupEvolution : UIPopupBase 
{
	public UIChallengeItemSlot itemslot;
	public UISprite spRuneStoneFull;


	public UILabel lbRuneStoneCount;
	public UILabel lbText, lbPrice;

	public UIButton btnOk;

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnOk.gameObject).onClick = onClickOk;
	}

	void onClickOk(GameObject go)
	{
		if(GameDataManager.instance.runeStone < GameDataManager.instance.evolvePrices["RSTONE"])
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("LACK_RUNESTONE"), closeThis, closeThis);
		}
		else if(GameDataManager.instance.gold < GameDataManager.instance.evolvePrices["GOLD"])
		{
			GameManager.me.uiManager.popupGoldForRuby.show (GameDataManager.instance.evolvePrices["GOLD"], onConfirmEvolution);
		}
		else
		{
			onConfirmEvolution();
		}
	}

	void onConfirmEvolution()
	{
		switch(_type)
		{
		case Rune.Type.Equipment:
			EpiServer.instance.sendEvolveEquipment(_didLoad, _sourceId);
			break;
		case Rune.Type.Unit:
			EpiServer.instance.sendEvolveUnitRune(_didLoad, _sourceId);
			break;
		case Rune.Type.Skill:
			EpiServer.instance.sendEvolveSkillRune(_didLoad, _sourceId);
			break;
		}
	}

	public void hideAll()
	{
		closeThis();

		switch(_type)
		{
		case Rune.Type.Equipment:
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.hide();
			break;
		case Rune.Type.Unit:
			GameManager.me.uiManager.popupSummonDetail.hide();
			break;
		case Rune.Type.Skill:
			GameManager.me.uiManager.popupSkillPreview.isEnabled = false;
			break;
		}


	}


	void closeThis()
	{
		base.hide();
	}


	public override void show ()
	{
		GameManager.me.uiManager.menuCamera3.gameObject.SetActive(true);

		if(GameDataManager.instance.gold < GameDataManager.instance.evolvePrices["GOLD"])
		{
			lbPrice.text = "[b82b00]"+GameDataManager.instance.evolvePrices["GOLD"]+"[-]";
		}
		else
		{
			lbPrice.text = "[fff1c9]"+GameDataManager.instance.evolvePrices["GOLD"]+"[-]";
		}

		if(GameDataManager.instance.runeStone < GameDataManager.instance.evolvePrices["RSTONE"])
		{
			spRuneStoneFull.enabled = false;
		}
		else
		{
			spRuneStoneFull.enabled = true;
		}

		lbRuneStoneCount.text = GameDataManager.instance.evolvePrices["RSTONE"].ToString();

		switch(_type)
		{
		case Rune.Type.Equipment:
			lbText.text = Util.getUIText("POPUP_EVOLVE_E", itemslot.infoData.itemName);
			break;
		case Rune.Type.Unit:
			lbText.text = Util.getUIText("POPUP_EVOLVE_U", itemslot.infoData.itemName);
			break;
		case Rune.Type.Skill:
			lbText.text = Util.getUIText("POPUP_EVOLVE_S", itemslot.infoData.itemName);
			break;
		}



		base.show();
	}

	private Rune.Type _type;
	private int _didLoad = WSDefine.NO;
	private string _sourceId;

	public void show(Rune.Type type, string sourceId, int didLoad)
	{
		_type = type;
		_didLoad = didLoad;
		_sourceId = sourceId;

		itemslot.setData(sourceId);
		show();
	}


}

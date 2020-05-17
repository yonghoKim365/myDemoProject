using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIComposePanel : MonoBehaviour 
{
	public UIChallengeItemSlot slotOriginal;
	public UIChallengeItemSlot slotSource;
	public UIChallengeItemSlot slotResult;

	public UILabel lbDefaultMessage, lbPrice;

	public UIButton btnCompose;
	public UIButton btnClose;


	void Awake()
	{
		UIEventListener.Get( btnCompose.gameObject ).onClick = onClickCompose;
		UIEventListener.Get( btnClose.gameObject ).onClick = onClickClose;
	}

	void onClickCompose(GameObject go)
	{
		if(GameDataManager.instance.gold < _currentComposePrice && TutorialManager.instance.isTutorialMode == false)
		{
			GameManager.me.uiManager.popupGoldForRuby.show (_currentComposePrice, onConfirmCompose);
		}
		else
		{
			onConfirmCompose();
		}

	}

	void onConfirmCompose()
	{
		RuneStudioMain.instance.reinforceOriginalData = originalData;

		switch(_type)
		{
		case Type.Unit:
			EpiServer.instance.sendComposeUnitRune(isTabSlot?WSDefine.YES:WSDefine.NO, originalData.serverId, source.serverId);
			break;
		case Type.Skill:
			EpiServer.instance.sendComposeSkillRune(isTabSlot?WSDefine.YES:WSDefine.NO, originalData.serverId, source.serverId);
			break;
		case Type.Equip:
			EpiServer.instance.sendComposeEquipment(isTabSlot?WSDefine.YES:WSDefine.NO, originalData.serverId, source.serverId);
			break;
		}
		
		hide ();
	}



	void onClickClose(GameObject go)
	{
		hide ();
	}

	public void hide()
	{
		if(isComposeMode)
		{
			isComposeMode = false;

			switch(_type)
			{
			case Type.Unit:
				GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(true);
				GameManager.me.uiManager.uiMenu.uiSummon.invenList.draw(false);
				break;
			case Type.Skill:
				GameManager.me.uiManager.uiMenu.uiSkill.btnSort.gameObject.SetActive(true);
				GameManager.me.uiManager.uiMenu.uiSkill.invenList.draw(false);
				break;
			case Type.Equip:
				GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(true);
				GameManager.me.uiManager.uiMenu.uiHero.invenList.draw(false);
//				GameManager.me.uiManager.uiMenu.uiHero.btnShowCharacterInfo.isEnabled = true;
				break;
			}
		}

		sourceIndex = -999;
		source = null;

		gameObject.SetActive(false);

		originalData = null;
		originalSlotIndex = -1;
	}

	public static int originalSlotIndex = -1;
	public static bool isComposeMode = false;

	public static GameIDData originalData;

	public static bool isTabSlot = false;

	public enum Type
	{
		Unit, Skill, Equip
	}

	Type _type = Type.Unit;


	void init()
	{
		gameObject.SetActive(true);

		isComposeMode = true;
		
		sourceIndex = -999;
		source = null;

		slotSource.gameObject.SetActive(false);
		slotResult.gameObject.SetActive(false);

		btnCompose.isEnabled = false;
		lbPrice.text = "0";

		refresh(true);
	}

	public void startFromSummon(GameIDData originalItemData, int slotIndex)
	{
		_type 	= Type.Unit;

//		isTabSlot = !originalSlot.isInventorySlot;

		lbDefaultMessage.text = Util.getUIText("COMPOSE_UNIT");

		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);

		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse( originalItemData.serverId,  originalItemData.type);

		isTabSlot = (slotIndex < 0);

		slotOriginal.setData(UIChallengeItemSlot.Type.Unit, originalData);

		init();

		GameManager.me.uiManager.uiMenu.uiSummon.invenList.draw(false);
	}



	public void startFromSkill(GameIDData originalItemData, int slotIndex)
	{
		GameManager.me.uiManager.uiMenu.uiSkill.btnSort.gameObject.SetActive(false);

		_type = Type.Skill;

//
//		if(originalSlot.isInventorySlot)
//		{
//			isTabSlot = (originalSlot.isChecked);
//		}
//		else
//		{
//			isTabSlot = true;
//		}

//		isTabSlot = !originalSlot.isInventorySlot;

		lbDefaultMessage.text = Util.getUIText("COMPOSE_SKILL");
		
		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);
		
		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse( originalItemData.serverId,  originalItemData.type);

		isTabSlot = (slotIndex < 0);

		slotOriginal.setData(UIChallengeItemSlot.Type.Skill, originalData);


		init();

		GameManager.me.uiManager.uiMenu.uiSkill.invenList.draw(false);
	}

	public void startFromHeroTab(GameIDData originalItemData)
	{
		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);

		_type = Type.Equip;

		isTabSlot = true;
		
		lbDefaultMessage.text = Util.getUIText("COMPOSE_EQUIP");

		originalSlotIndex = -1;
		originalData = new GameIDData();
		originalData.parse( originalItemData.serverId,  originalItemData.type);

//		GameIDData test = new GameIDData();
//		test.parse("LEO_HD2_1_10_0", GameIDData.Type.Equip);
//		originalData = test;
//
//		GameIDData test2 = new GameIDData();
//		test2.parse("LEO_BD2_1_10_0", GameIDData.Type.Equip);
//		GameIDData test3 = new GameIDData();
//		test3.parse("LEO_HD2_1_5_0", GameIDData.Type.Equip);
//		originalItemData = test;


		slotOriginal.setData(UIChallengeItemSlot.Type.Equip, originalData);

		init();
		
		GameManager.me.uiManager.uiMenu.uiHero.invenList.draw(false);
//		GameManager.me.uiManager.uiMenu.uiHero.btnShowCharacterInfo.isEnabled = false;

//		sourceList.Add(test2);
//		sourceList.Add(test3);
//		refresh();


	}

	public void startFromHeroInven(GameIDData originalItemData, int slotIndex)
	{
		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);

		_type = Type.Equip;
		
		isTabSlot = false;
		
		lbDefaultMessage.text = Util.getUIText("COMPOSE_EQUIP");

		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse( originalItemData.serverId,  originalItemData.type);
		
		slotOriginal.setData(UIChallengeItemSlot.Type.Equip, originalData);

		
		init();
		
		GameManager.me.uiManager.uiMenu.uiHero.invenList.draw(false);
//		GameManager.me.uiManager.uiMenu.uiHero.btnShowCharacterInfo.isEnabled = false;

	}








	public static int sourceIndex = -999;
	public GameIDData source = null;


	public void onClick(UISummonInvenSlot invenSlot)
	{
		if(invenSlot.data == null) return;

		switch(onClick(invenSlot.index, invenSlot.data))
		{
		case 1: invenSlot.select = true; break;
		case -1: invenSlot.select = false; break;
		}

		refresh();
	}

	public void onClick(UISkillInvenSlot invenSlot)
	{
		switch(onClick(invenSlot.index, invenSlot.data))
		{
		case 1: invenSlot.select = true; break;
		case -1: invenSlot.select = false; break;
		}

		refresh();
	}

	public void onClick(UIHeroInventorySlot invenSlot)
	{
		switch(onClick(invenSlot.index, invenSlot.data))
		{
		case 1: invenSlot.select = true; break;
		case -1: invenSlot.select = false; break;
		}

		refresh();
	}


	public void onClick(UIHeroInventoryTab invenSlot)
	{
		onClick(-1000, invenSlot.data);

		refresh();
	}


	int onClick (int slotIndex, GameIDData slotData)
	{
		if(sourceIndex == -999)
		{
			sourceIndex = slotIndex;
			source = slotData;

			slotSource.gameObject.SetActive(true);

			int price = 0;

			switch(_type)
			{
			case Type.Equip:
				slotSource.setData(UIChallengeItemSlot.Type.Equip, source);
				price = GameDataManager.instance.composePrices[source.partsData.character + source.partsData.type + RareType.SERVER_CHARACTER[originalData.rare]];
				break;
			case Type.Skill:
				slotSource.setData(UIChallengeItemSlot.Type.Skill, source);
				price = GameDataManager.instance.composePrices["SKILL" + RareType.SERVER_CHARACTER[originalData.rare]];
				break;
			case Type.Unit:
				slotSource.setData(UIChallengeItemSlot.Type.Unit, source);
				price = GameDataManager.instance.composePrices["UNIT" + RareType.SERVER_CHARACTER[originalData.rare]];
				break;
			}

			slotResult.gameObject.SetActive(true);
			slotResult.setRare(originalData.rare + 1);
			slotResult.setLevel(1);
			slotResult.spBackground.gameObject.SetActive(true);
			slotResult.spBackground.spriteName = UIHeroInventorySlot.SLOT_BG_QUESTION;


			btnCompose.isEnabled = true;
			lbPrice.text = Util.GetCommaScore(price);
			_currentComposePrice = price;
			return 1;
		}
		else if(slotIndex == sourceIndex)
		{
			sourceIndex = -999;
			source = null;

			slotSource.infoData = null;
			slotSource.gameObject.SetActive(false);
			slotResult.gameObject.SetActive(false);

			btnCompose.isEnabled = false;
			lbPrice.text = "0";
			return -1;
		}
		return 0;
	}


	private int _currentComposePrice = 0;
	private Vector3 _v;

	bool _enoughSelection = false;

	void refresh(bool displayRightNow = false)
	{

	}


	void setAllData(bool calcLabel = false, bool includeOriginal = false)
	{
	}



}


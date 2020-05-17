using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UIHero : UIBase {

	public UIInvenPlayerTab tabPlayer;

	public UIButton btnSort, btnBuyRuby, btnBuyGold, btnOpenShop, btnOpenRuneBook, btnOpenHeroInfo;

	public UIHeroInvenList invenList;
	public UIInvenSort popupSort;

	public UIHeroItemDetailPopup itemDetailPopup;

	public UIHeroInventoryTab[] slotCategory;
	public static string _nowHero = null;
	public static string nowHero
	{
		get
		{
#if UNITY_EDITOR
			if(DebugManager.instance.useDebug)
			{
				return DebugManager.instance.defaultHero;
			}
#endif

			if(_nowHero == null) return Character.LEO;
			return _nowHero;
		}
		set
		{

			_nowHero = value;
		}
	}


	public UILabel lbGold, lbRuby, lbRuneStone;

	void Awake()
	{

		setBackButton(UIMenu.LOBBY);

		UIEventListener.Get(btnOpenShop.gameObject).onClick = onClickOpenShop;

		UIEventListener.Get(btnBuyGold.gameObject).onClick = onBuyGold;
		UIEventListener.Get(btnBuyRuby.gameObject).onClick = onBuyRuby;

		UIEventListener.Get(btnSort.gameObject).onClick = onClickSort;

		UIEventListener.Get(btnOpenRuneBook.gameObject).onClick = onClickOpenRuneBook;

		UIEventListener.Get(btnOpenHeroInfo.gameObject).onClick = onClickOpenHeroInfo;


		popupSort.invenType = UIInvenSort.InvenType.equip;
		popupSort.list = invenList;

		GameDataManager.goldDispatcher -= updateGold;
		GameDataManager.goldDispatcher += updateGold;
		
		GameDataManager.rubyDispatcher -= updateRuby;
		GameDataManager.rubyDispatcher += updateRuby;

		GameDataManager.runeStoneDispatcher -= updateRuneStone;
		GameDataManager.runeStoneDispatcher += updateRuneStone;
	}

	public override void onClickBackToMainMenu (GameObject go)
	{
		if(reinforcePanel.gameObject.activeSelf)
		{
			reinforcePanel.hide();
			return;
		}
		else if(composePanel.gameObject.activeSelf)
		{
			composePanel.hide();
			return;
		}
		else if(sellPanel.gameObject.activeSelf)
		{
			sellPanel.hide();
			return;
		}


		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.partsInventory)
		{
			kv.Value.isNew = false;
		}

		UILobby.setHasNewEquipRune(false);


		base.onClickBackToMainMenu (go);

		TutorialManager.uiTutorial.tutorialEndCircleEffect.hide();
	}


	void onClickOpenRuneBook(GameObject go)
	{
		GameManager.me.uiManager.popupEquipRuneBook.init();
	}

	void onClickOpenHeroInfo(GameObject go)
	{
		GameManager.me.uiManager.popupHeroInfo.init(tabPlayer.currentTab);
	}

	void onClickOpenRuneInfo(GameObject go)
	{
		//GameManager.me.uiManager.popupRuneStoneInfo.show();
	}


	void onClickSort(GameObject go)
	{
		popupSort.open();
	}


	void onClickOpenShop(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showSpecialShop();
	}

	void onBuyGold(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showGoldShop();
	}

	void onBuyRuby(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showRubyShop();
	}


	void updateGold()
	{
		lbGold.text = Util.GetCommaScore(GameDataManager.instance.gold);
	}
	
	void updateRuby()
	{
		lbRuby.text = Util.GetCommaScore(GameDataManager.instance.ruby);
	}


	void updateRuneStone()
	{
		lbRuneStone.text = Util.GetCommaScore(GameDataManager.instance.runeStone);
	}


	void changePlayerTab(string chaId)
	{
		setCharacter(chaId);
	}


	public const int LEO = 0;
	public const int KILEY = 1;
	public const int CHLOE = 2;
	public const int LUKE = 3;



	void setCharacter(string chaId)
	{
		switch(chaId)
		{
		case Character.LEO: setCharacter(LEO); break;
		case Character.KILEY: if(GameDataManager.instance.maxAct >=3) setCharacter(KILEY); break;
		case Character.CHLOE: if(GameDataManager.instance.maxAct >=5) setCharacter(CHLOE); break;
		case Character.LUKE: if(GameDataManager.instance.maxAct >=8) setCharacter(LUKE); break;
		}
	}

	public static string heroIdByIndex(int index)
	{
		switch(index)
		{
		case LEO: return Character.LEO; break;
		case CHLOE: return Character.CHLOE; break;
		case KILEY: return Character.KILEY; break;
		case LUKE: return Character.LUKE; break;
		}

		return "";
	}

	public static bool canOpenCharacter(int character)
	{
		switch(character)
		{
		case LEO:
			return true;
			break;
		case KILEY:

			if(GameDataManager.instance.maxAct >= 3)
			{
				return true;
			}
			break;
		case CHLOE:
			if(GameDataManager.instance.maxAct >= 5)
			{
				return true;
			}
			break;
		case LUKE:
			if(GameDataManager.instance.maxAct >= 8)
			{
				return true;
			}

			break;
		}

		return false;
	}


	void setCharacter(int index)
	{
		string heroId = Character.LEO;

		if(GameDataManager.instance.heroes.ContainsKey(Character.KILEY) == false)
		{
			if(GameDataManager.instance.maxAct >= 3)
			{

			}
		}

		if(GameDataManager.instance.heroes.ContainsKey(Character.CHLOE) == false)
		{
			if(GameDataManager.instance.maxAct >= 5)
			{

			}
		}

		heroId = heroIdByIndex(index);

		if(nowHero != heroId)
		{
			nowHero = heroId;
			refreshList();
		}
	}





	public UIHeroInventorySlot selectedSlot;
	UIHeroInventoryTab _selectedTab;

	public void onClickSlot(UIHeroInventorySlot s, GameIDData data)
	{
		if(UIReinforceBarPanel.isReinforceMode)
		{
			if(s.data == null) return;

			reinforcePanel.onClick(s);
		}
		else if(UIComposePanel.isComposeMode)
		{
			if(s.data == null) return;
			composePanel.onClick(s);
		}
		else if(UIMultiSellPanel.isMultiSell)
		{
			if(s.data == null) return;
			sellPanel.onClick(s);
		}
		else
		{
			selectedSlot = s;
			_selectedTab = null;

			itemDetailPopup.show(data, RuneInfoPopup.Type.Normal, true);
		}

	}


	public void onClickTabSlot(UIHeroInventoryTab t, GameIDData data)
	{
		if(UIReinforceBarPanel.isReinforceMode)
		{
			if(t.data == null) return;
			reinforcePanel.onClick(t);
		}
		else if(UIComposePanel.isComposeMode)
		{
			if(t.data == null) return;
			composePanel.onClick(t);
		}
		else
		{
			selectedSlot = null;
			_selectedTab = t;
			
			itemDetailPopup.show(data, RuneInfoPopup.Type.Normal, false);
		}
	}


	
	public override void show()
	{
		base.show();

		popupSort.hide();

		itemDetailPopup.hide();

		btnSort.gameObject.SetActive(true);

		nowHero = "";

		setCharacter(GameDataManager.instance.selectHeroId);

		updateGold();
		updateRuby();
		updateRuneStone();


		tabPlayer.tabChangeDispatcher -= changePlayerTab;
		tabPlayer.tabChangeDispatcher += changePlayerTab;
		tabPlayer.init(GameIDData.Type.Equip);

		refreshSort();

		refreshList(true);

		reinforcePanel.hide();
		composePanel.hide();
		sellPanel.hide();

		TutorialManager.instance.check("T37");


	}

	void refreshSort()
	{
		invenList.sortType = LocalDataManager.getInvenSortType(UIInvenSort.InvenType.equip);
		invenList.sortFromHigh = LocalDataManager.isInvenSortDirectionHigh(UIInvenSort.InvenType.equip);
	}


	public override void hide ()
	{
		base.hide ();
		inventorySlots = null;
	}

	const int BD = 0;
	const int HD = 1;
	const int WP = 2;
	const int RD = 3;

	void refreshSelectedParts()
	{
		slotCategory[BD].setData(GameDataManager.instance.heroes[nowHero].partsBody.itemInfo);
		slotCategory[HD].setData(GameDataManager.instance.heroes[nowHero].partsHead.itemInfo);
		slotCategory[WP].setData(GameDataManager.instance.heroes[nowHero].partsWeapon.itemInfo);
		slotCategory[RD].setData(GameDataManager.instance.heroes[nowHero].partsVehicle.itemInfo);
	}
/*
	public void setCharacterData()
	{

		sb1.Length = 0;
		sb2.Length = 0;

		if(hero == null) return;

		hero.changeGamePlayerData(GameDataManager.instance.heroes[nowHero]);

		sb1.Append("[f0b938]- "+Util.getUIText("HERO")+" -[-]\n[e0c3a1]");
		sb2.Append("\n");

		setStatLine(hero.maxHp,"MAXHP","{0:0}");
		setStatLine(hero.maxSp,"MAXSP","{0:0}");
		setStatLine(hero.stat.spRecovery,"SPRECOVERY","{0:0.0}");
		setStatLine(hero.maxMp,"MAXMP","{0:0}");
		setStatLine(hero.stat.mpRecovery,"MPRECOVERY","{0:0.0}");

		if(hero.stat.atkPhysic > hero.stat.atkMagic) setStatLine(hero.stat.atkPhysic,"ATKPHYSIC","{0:0.0}");
		else setStatLine(hero.stat.atkMagic,"ATKMAGIC","{0:0.0}");

		setStatLine(hero.stat.defPhysic,"DEFPHYSIC","{0:0.0}");
		setStatLine(hero.stat.defMagic,"DEFMAGIC","{0:0.0}");
		setStatLine(hero.stat.speed,"MOVESPEED","{0:0}");

		sb1.Append("[-]");
		sb1.Append("[f0b938]- " + Util.getUIText("UNIT_RUNE") + " -[-]\n[e0c3a1]");
		sb2.Append("\n");


		setStatLine(hero.stat.summonSpPercent*100,"SUMMON_SP_PERCENT","{0:0}","%\n");
		setStatLine(hero.stat.unitHpUp*100,"UNIT_HP_UP","{0:0}","%\n");
		setStatLine(hero.stat.unitDefUp*100,"UNIT_DEF_UP","{0:0}","%\n");

		sb1.Append("[-]");
		sb1.Append("[f0b938]- "+Util.getUIText("SKILL_RUNE")+" -[-]\n[e0c3a1]");
		sb2.Append("\n");

		setStatLine(hero.stat.skillMpDiscount*100,"SKILL_MP_DISCOUNT","{0:0}","%\n");
		setStatLine(hero.stat.skillAtkUp*100,"SKILL_ATK_UP","{0:0}","%\n");
		setStatLine(hero.stat.skillUp*100,"SKILL_UP","{0:0}","%\n");
		setStatLine(hero.stat.skillTimeUp*100,"SKILL_TIME_UP","{0:0}","%\n");

		sb1.Append("[-]");

		lbInfoName.text = sb1.ToString();
		lbInfoConetnt.text = sb2.ToString();
	}
	
	StringBuilder sb1 = new StringBuilder();
	StringBuilder sb2 = new StringBuilder();
	void setStatLine(float value, string nameId, string format, string lastWord = "\n")
	{
		sb1.Append(Util.getUIText(nameId));
		sb1.Append("\n");
		sb2.Append(string.Format(format,value));
		sb2.Append(lastWord);
	}
*/


	public void onCompleteChangeEquip()
	{
		refreshList();
//		refreshHeroModel();
	}


	public UIHeroInventorySlot[] inventorySlots = new UIHeroInventorySlot[1];


	public void refreshList(bool reposition = false)
	{
		refreshSelectedParts();

		invenList.draw(reposition);

		inventorySlots = invenList.listGrid.gameObject.GetComponentsInChildren<UIHeroInventorySlot>();
	}





	public UIMultiSellPanel sellPanel;
	
	
	public void startSellMode(GameIDData sellingItemData)
	{
		if( selectedSlot == null)
		{
			resetSelectSlot(sellingItemData, true);
		}
		// 방어 코드
		if(selectedSlot == null )
		{
			int len = invenList.listGrid.dataList.Count;
			
			for(int i = 0; i < len ; ++i)
			{
				GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];
				
				for(int j = 0; j < invenList.maxPerLine; ++j)
				{
					if(data[j] == null) return;
					
					if(sellingItemData.serverId == data[j].serverId)
					{
						sellPanel.startFromHeroInven(sellingItemData, invenList.maxPerLine * i + j);
						return;
					}
				}
			}
		}

		if(selectedSlot != null)
		{
			selectedSlot.select = true;
			sellPanel.startFromHeroInven(selectedSlot.data, selectedSlot.index);
		}
	}


	public UIReinforceBarPanel reinforcePanel;


	public void startReinforceMode(GameIDData reinforceItemData, bool isFromInventorySlot)
	{
		if(isFromInventorySlot && selectedSlot == null)
		{
			resetSelectSlot(reinforceItemData, isFromInventorySlot);
		}
		else if(isFromInventorySlot == false && _selectedTab == null)
		{
			resetSelectSlot(reinforceItemData, isFromInventorySlot);
		}

		// 방어 코드
		if(selectedSlot == null && _selectedTab == null)
		{
			int len = invenList.listGrid.dataList.Count;

			for(int i = 0; i < len ; ++i)
			{
				GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];

				for(int j = 0; j < invenList.maxPerLine; ++j)
				{
					if(data[j] == null) return;

					if(reinforceItemData.serverId == data[j].serverId)
					{
						reinforcePanel.startFromHeroInven(reinforceItemData, invenList.maxPerLine * i + j);
						return;
					}
				}
			}
		}
		//============


		if(selectedSlot != null)
		{
			reinforcePanel.startFromHeroInven(selectedSlot.data, selectedSlot.index);
		}
		else if(_selectedTab != null)
		{
			reinforcePanel.startFromHeroTab(_selectedTab.data);
		}
	}



	public UIComposePanel composePanel;
	
	
	public void startComposeMode(GameIDData composeItemData, bool isFromInventorySlot)
	{
		if(isFromInventorySlot && selectedSlot == null)
		{
			resetSelectSlot(composeItemData, isFromInventorySlot);
		}
		else if(isFromInventorySlot == false && _selectedTab == null)
		{
			resetSelectSlot(composeItemData, isFromInventorySlot);
		}
		
		// 방어 코드
		if(selectedSlot == null && _selectedTab == null)
		{
			int len = invenList.listGrid.dataList.Count;
			
			for(int i = 0; i < len ; ++i)
			{
				GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];
				
				for(int j = 0; j < invenList.maxPerLine; ++j)
				{
					if(data[j] == null) continue;
					
					if(composeItemData.serverId == data[j].serverId)
					{
						composePanel.startFromHeroInven(composeItemData, invenList.maxPerLine * i + j);
						return;
					}
				}
			}
		}
		//============
		
		
		if(selectedSlot != null)
		{
			composePanel.startFromHeroInven(selectedSlot.data, selectedSlot.index);
		}
		else if(_selectedTab != null)
		{
			composePanel.startFromHeroTab(_selectedTab.data);
		}
	}
















	void resetSelectSlot(GameIDData d, bool isInventorySlot)
	{
		if(d == null) return;

		if(isInventorySlot)
		{
			if(selectedSlot == null)
			{
				UIHeroInventorySlot[] ss = invenList.GetComponentsInChildren<UIHeroInventorySlot>();
				foreach(UIHeroInventorySlot ts in ss)
				{
					if(ts.data != null && d.serverId == ts.data.serverId)
					{
						selectedSlot = ts;
						break;
					}
				}
			}
		}
		else
		{
			if(_selectedTab == null)
			{
				for(int i = 0; i < slotCategory.Length; ++i)
				{
					if(slotCategory[i].data.serverId == d.serverId)
					{
						_selectedTab = slotCategory[i];
						break;
					}
				}
			}
		}
	}


	void LateUpdate()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			if(btnBack.isEnabled == false) return;
			if(GameManager.me.uiManager.uiMenu.rayCast(GameManager.me.uiManager.uiMenuCamera.camera, btnBack.gameObject) == false) return;

			if(TutorialManager.instance.isTutorialMode || TutorialManager.instance.isReadyTutorialMode) return;
			if(UILoading.nowLoading) return;
			onClickBackToMainMenu(null);
			return;
		}
	}


}

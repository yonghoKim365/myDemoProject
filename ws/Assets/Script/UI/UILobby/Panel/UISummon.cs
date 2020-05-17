using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class UISummon : UIBase {

	public UIInvenPlayerTab tabPlayer;

	public UISummonInvenSlot[] nowSlots;

	public enum Mode
	{
		Normal, PutOn, Break
	}
	
	public Mode mode = Mode.Normal;
	
	public UISummonInvenSlot selectSlot;

	public UISummonInvenList invenList;
	public UIButton btnSort, btnCancelPutOnMode;
	public UIInvenSort popupSort;

	public GameObject putOnPanel;
	
	public UILabel lbRuby, lbGold, lbRuneStone, lbPutOnModeMsg;

	public UIButton btnBuyRuby, btnBuyGold, btnOpenShop, btnOpenBook, btnOpenHeroInfo;//, btnOpenRuneInfo;




	void Awake()
	{
		setBackButton(UIMenu.LOBBY);

		UIEventListener.Get(btnSort.gameObject).onClick = onClickSort;


		UIEventListener.Get(btnCancelPutOnMode.gameObject).onClick = onClickCancelPutOnMode;

		UIEventListener.Get(btnBuyGold.gameObject).onClick = onBuyGold;
		UIEventListener.Get(btnBuyRuby.gameObject).onClick = onBuyRuby;

		UIEventListener.Get(btnOpenShop.gameObject).onClick = onClickShop;
		UIEventListener.Get(btnOpenBook.gameObject).onClick = onClickBook;

		UIEventListener.Get(btnOpenHeroInfo.gameObject).onClick = onClickOpenHeroInfo;

		//UIEventListener.Get(btnOpenRuneInfo.gameObject).onClick = onClickOpenRuneInfo;


		popupSort.invenType = UIInvenSort.InvenType.unit;
		popupSort.list = invenList;



		int i = 0;
		foreach(UISummonInvenSlot s in nowSlots)
		{
			mySlotDic[s.gameObject.name] = s;
			s.selectSlotListener = onSelectSlot;
			UIEventListener.Get(s.btn.gameObject).onClick = s.onSelectSlot;
			s.isInventorySlot = false;
			s.index = i;
			s.spSlotName.spriteName = "img_slot"+(i+1);
			++i;
		}

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

		if(composePanel.gameObject.activeSelf)
		{
			composePanel.hide();
			return;
		}

		if(sellPanel.gameObject.activeSelf)
		{
			sellPanel.hide();
			return;
		}

		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.unitInventory)
		{
			kv.Value.isNew = false;
		}

		UILobby.setHasNewUnitRune(false);

		base.onClickBackToMainMenu (go);

		TutorialManager.uiTutorial.tutorialEndCircleEffect.hide();
	}


	void onBuyGold(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showGoldShop();
	}
	
	void onBuyRuby(GameObject go)
	{
		GameManager.me.uiManager.popupShop.showRubyShop();
	}


	void onClickShop(GameObject go)
	{
		selectSlot = null;
		GameManager.me.uiManager.popupShop.showSpecialShop();
	}

	void onClickBook(GameObject go)
	{
		GameManager.me.uiManager.popupRuneBook.init(UIPopupRuneBook.Type.UnitBook);
	}


	void onClickOpenHeroInfo(GameObject go)
	{
		GameManager.me.uiManager.popupHeroInfo.init(tabPlayer.currentTab);
	}

	void onClickOpenRuneInfo(GameObject go)
	{
		//GameManager.me.uiManager.popupRuneStoneInfo.show();
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
		invenList.onTabChange();
		refreshMyUnits();
	}




	void onClickCancelPutOnMode(GameObject go)
	{
		setMode(Mode.Normal);
	}


	void onClickSort(GameObject go)
	{
		if(mode == Mode.PutOn) return;
		popupSort.open();
	}


	public void onCompleteChangeUnit(bool refresh = true)
	{
		selectSlot = null;
		setMode(Mode.Normal);

		if(refresh)
		{
			refreshMyUnits();
			refreshUnitInven();
		}
	}


	void resetSelectSlot(GameIDData d, bool isInventorySlot)
	{
		if(selectSlot == null && d != null)
		{
			UISummonInvenSlot[] ts = invenList.GetComponentsInChildren<UISummonInvenSlot>();
			foreach(UISummonInvenSlot t in ts)
			{
				if(t.data != null && d.serverId == t.data.serverId)// && t.isChecked == !isInventorySlot)
				{
					selectSlot = t;
					break;
				}
			}
		}
	}


	public void startPutOn(GameIDData d)
	{
		resetSelectSlot(d, true);
		setMode(Mode.PutOn, d);
	}
	
	public void startPutOff( GameIDData gd )
	{
		for(int i = 0; i < 5; ++i)
		{
			if(GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].isOpen && gd != null)
			{
				if(gd.serverId == GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].unitInfo.serverId)
				{
					switch(i)
					{
					case 0: _slotId = UnitSlot.U1; break;
					case 1: _slotId = UnitSlot.U2; break;
					case 2: _slotId = UnitSlot.U3; break;
					case 3: _slotId = UnitSlot.U4; break;
					case 4: _slotId = UnitSlot.U5; break;
					}
				}
			}
		}


		EpiServer.instance.sendUnloadUnitRune(tabPlayer.currentTab, _slotId);
	}

	public void startBreak(UnitData unitData)
	{

	}


	public UISprite spSelectSlotBorder;
	public GameObject[] spNowSelectedSlots;

	public GameIDData _selectSlotData = null;

	void setMode(Mode m, GameIDData gd = null)
	{
		mode = m;
		
		if(m == Mode.Normal)
		{
			if(selectSlot != null) selectSlot.select = false;
			refreshMyUnits();
			selectSlot = null;
			putOnPanel.SetActive(false);
		}
		else if(m == Mode.PutOn)
		{
			putOnPanel.SetActive(true);
			//_selectSlot.select = true;

			_selectSlotData = null;

			bool alreadyPutOnItem = false;

			if(selectSlot != null && selectSlot.data != null)
			{
				spSelectSlotBorder.enabled = true;
				spSelectSlotBorder.cachedTransform.position = selectSlot.spSelectBorder.cachedTransform.position;
				alreadyPutOnItem = isSettingSameBaseUnit(selectSlot.data.unitData, tabPlayer.currentTab);
				_selectSlotData = selectSlot.data;
			}
			else if(gd != null)
			{
				spSelectSlotBorder.enabled = false;
				alreadyPutOnItem = isSettingSameBaseUnit(gd.unitData, tabPlayer.currentTab);
				_selectSlotData = gd;
			}


			if(alreadyPutOnItem)
			{
				lbPutOnModeMsg.text = Util.getUIText("CAN_PUTON_SAMERUNE");
			}
			else
			{
				lbPutOnModeMsg.text = Util.getUIText("SELECT_SLOT");
			}

			for(int i = 0; i < 5; ++i)
			{
				if(GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].isOpen)
				{
					if(alreadyPutOnItem)
					{
						if(TutorialManager.instance.isTutorialMode) spNowSelectedSlots[i].SetActive(false);
						else spNowSelectedSlots[i].SetActive(_selectSlotData.unitData.baseUnitId == GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].unitData.baseUnitId);

						nowSlots[i].buttonEnabled = (_selectSlotData.unitData.baseUnitId == GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].unitData.baseUnitId);
					}
					else
					{
						if(TutorialManager.instance.isTutorialMode) spNowSelectedSlots[i].SetActive(false);
						else spNowSelectedSlots[i].SetActive(true);
						nowSlots[i].buttonEnabled = true;
					}
				}
				else
				{
					if(TutorialManager.instance.isTutorialMode) spNowSelectedSlots[i].SetActive(false);
					else spNowSelectedSlots[i].SetActive(!alreadyPutOnItem);
					nowSlots[i].buttonEnabled = (!alreadyPutOnItem);
				}

				if(TutorialManager.nowPlayingTutorial("T44"))
				{
					nowSlots[i].buttonEnabled = true;
				}
			}


		}

	}

	

	
	


	void refreshSort()
	{
		invenList.sortType = LocalDataManager.getInvenSortType(UIInvenSort.InvenType.unit);
		invenList.sortFromHigh = LocalDataManager.isInvenSortDirectionHigh(UIInvenSort.InvenType.unit);
	}

	
	public override void show()
	{
		base.show();
		popupSort.hide();
		btnSort.gameObject.SetActive(true);

		setMode( Mode.Normal );

		selectSlot = null;

		updateGold();
		updateRuby();
		updateRuneStone();


		tabPlayer.tabChangeDispatcher -= changePlayerTab;
		tabPlayer.tabChangeDispatcher += changePlayerTab;
		tabPlayer.init(GameIDData.Type.Unit);


		refreshSort();
		refreshMyUnits();
		refreshUnitInven(true);

		reinforcePanel.hide();
		composePanel.hide();
		sellPanel.hide();

//		if(GameDataManager.instance.historyUnitRunes == null) EpiServer.instance.sendUnitRuneHistory();

		checkTutorial();

	}	


	public void checkTutorial()
	{
		if(TutorialManager.instance.check("T37") == false)
		{
			TutorialManager.instance.check("T52");
		}
	}



	private Dictionary<string, UISummonInvenSlot> mySlotDic = new Dictionary<string, UISummonInvenSlot>(StringComparer.Ordinal);
	
	public void refreshMyUnits()
	{
		for(int i = 0; i < 5; ++i)
		{
			if(GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].isOpen)
			{
				nowSlots[i].setData(GameDataManager.instance.playerUnitSlots[tabPlayer.currentTab][i].unitInfo);	
				nowSlots[i].buttonEnabled = true;
			}
			else
			{
				nowSlots[i].setData(null);	
				nowSlots[i].buttonEnabled = false;
			}


			nowSlots[i].select = false;
		}
	}
	


	public void refreshUnitInven(bool isReposition = false)
	{
		invenList.draw(isReposition);

		System.GC.Collect();
	}

	
	
	bool isSettingSameBaseUnit(UnitData ud, string character)
	{
		foreach(PlayerUnitSlot pd in GameDataManager.instance.playerUnitSlots[character])
		{
			if(pd.isOpen == false) continue;
			if(pd.unitData.baseUnitId == ud.baseUnitId)
			{
				return true;
			}
		}		
		return false;
	}



	// 현재 캐릭터가 아닌 다른 캐릭터가 이 스킬을 차고 있는지 확인해본다.
	public string lastCheckPutOnCharacter;
	public bool checkCanPutOn(UnitData ud)
	{
		int count = 0;

		foreach(KeyValuePair<string, PlayerUnitSlot[]> kv  in GameDataManager.instance.playerUnitSlots)
		{
			if(tabPlayer.currentTab == kv.Key || GameDataManager.instance.serverHeroData.ContainsKey(kv.Key) == false)
			{
				continue;
			}
			
			foreach(PlayerUnitSlot pd in kv.Value)
			{
				if(pd.isOpen == true && pd.unitData.baseUnitId == ud.baseUnitId)
				{
					lastCheckPutOnCharacter = kv.Key;
					return false;
				}
			}
		}
		
		return true;
	}








	string _slotId;
	public void onSelectSlot(UISummonInvenSlot s, GameIDData data)
	{
		if(UIReinforceBarPanel.isReinforceMode)
		{
			selectSlot = null;
			if(s.data == null) return;
			reinforcePanel.onClick(s);
		}
		else if(UIComposePanel.isComposeMode)
		{
			selectSlot = null;
			if(s.data == null) return;
			composePanel.onClick(s);
		}
		else if(UIMultiSellPanel.isMultiSell)
		{
			selectSlot = null;
			if(s.data == null) return;
			sellPanel.onClick(s);
		}
		else if(mode == Mode.Normal)
		{
			if(data != null)
			{
				selectSlot = s;
				s.select = false;

				GameManager.me.uiManager.popupSummonDetail.show(data, RuneInfoPopup.Type.Normal, s.isInventorySlot);
			}
		}
		else if(mode == Mode.PutOn)
		{
			if(s.isInventorySlot || _selectSlotData == null) return;
			
			switch(s.index)
			{
			case 0: _slotId = UnitSlot.U1; break;
			case 1: _slotId = UnitSlot.U2; break;
			case 2: _slotId = UnitSlot.U3; break;
			case 3: _slotId = UnitSlot.U4; break;
			case 4: _slotId = UnitSlot.U5; break;
			}

			SoundData.play("uirn_runeattach");
			EpiServer.instance.sendChangeUnitRune(tabPlayer.currentTab, _slotId, _selectSlotData.serverId);			
		}
	}







	public UIMultiSellPanel sellPanel;
	
	
	public void startSellMode(GameIDData sellingItemData)
	{
		resetSelectSlot(sellingItemData, true);
		
		int slotIndex = -1;
		if(selectSlot != null)
		{
			slotIndex = selectSlot.index;
			sellPanel.startFromSummon(sellingItemData, slotIndex);
		}
		else
		{

			int len = invenList.listGrid.dataList.Count;
			
			for(int i = 0; i < len ; ++i)
			{
				GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];
				
				for(int j = 0; j < invenList.maxPerLine; ++j)
				{
					if(data[j] == null) continue;
					
					if(sellingItemData.serverId == data[j].serverId)
					{
						sellPanel.startFromSummon(sellingItemData, invenList.maxPerLine * i + j);
						return;
					}
				}
			}

		}
	}







	public UIReinforceBarPanel reinforcePanel;

	public void startReinforceMode(GameIDData reinforceItemData, bool isFromInventorySlot)
	{
		resetSelectSlot(reinforceItemData, isFromInventorySlot);
		
		int slotIndex = -1;
		if(selectSlot != null)
		{
			slotIndex = selectSlot.index;
			if(isFromInventorySlot == false) slotIndex = -1;
			reinforcePanel.startFromSummon(reinforceItemData, slotIndex);
		}
		else
		{
			if(isFromInventorySlot == false)
			{
				reinforcePanel.startFromSummon(reinforceItemData, -1);
			}
			else
			{
				int len = invenList.listGrid.dataList.Count;
				
				for(int i = 0; i < len ; ++i)
				{
					GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];
					
					for(int j = 0; j < invenList.maxPerLine; ++j)
					{
						if(data[j] == null) continue;

						if(reinforceItemData.serverId == data[j].serverId)
						{
							reinforcePanel.startFromSummon(reinforceItemData, invenList.maxPerLine * i + j);
							return;
						}
					}
				}
			}
		}
	}




	public UIComposePanel composePanel;
	public UITutorialComposePanel tutorialComposePanel;

	
	public void startComposeMode(GameIDData composeItemData, bool isFromInventorySlot)
	{
		resetSelectSlot(composeItemData, isFromInventorySlot);
		
		int slotIndex = -1;
		if(selectSlot != null)
		{
			slotIndex = selectSlot.index;
			if(isFromInventorySlot == false) slotIndex = -1;
			composePanel.startFromSummon(composeItemData, slotIndex);
		}
		else
		{
			if(isFromInventorySlot == false)
			{
				composePanel.startFromSummon(composeItemData, -1);
			}
			else
			{
				int len = invenList.listGrid.dataList.Count;
				
				for(int i = 0; i < len ; ++i)
				{
					GameIDData[] data = (GameIDData[])invenList.listGrid.dataList[i];
					
					for(int j = 0; j < invenList.maxPerLine; ++j)
					{
						if(data[j] == null) return;
						
						if(composeItemData.serverId == data[j].serverId)
						{
							composePanel.startFromSummon(composeItemData, invenList.maxPerLine * i + j);
							return;
						}
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

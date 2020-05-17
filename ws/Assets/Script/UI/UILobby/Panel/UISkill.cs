using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class UISkill : UIBase {

	public UIInvenPlayerTab tabPlayer;

	public UISkillInvenSlot[] nowSlots;

	public enum Mode
	{
		Normal, PutOn, Break
	}

	public Mode mode = Mode.Normal;

	public UISkillInvenSlot selectSlot;

	public UISkillInvenList invenList;
	public UIButton btnSort, btnCancelPutOnMode;
	public UIInvenSort popupSort;

	public GameObject putOnPanel;

	public UILabel lbRuby, lbGold, lbRuneStone, lbPutOnModeMsg;

	public UIButton btnBuyRuby, btnBuyGold, btnOpenShop, btnOpenBook, btnOpenHeroInfo;


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



		popupSort.invenType = UIInvenSort.InvenType.skill;
		popupSort.list = invenList;

		int i = 0;
		foreach(UISkillInvenSlot s in nowSlots)
		{
			mySlotDic[s.gameObject.name] = s;
			s.isInventorySlot = false;
			s.index = i;
			++i;
		}

		if(GameManager.me != null) GameManager.me.uiManager.popupSkillPreview.gameObject.SetActive(false);
		



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

		foreach(KeyValuePair<string, GameIDData> kv in GameDataManager.instance.skillInventory)
		{
			kv.Value.isNew = false;
		}

		UILobby.setHasNewSkillRune(false);

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
		GameManager.me.uiManager.popupRuneBook.init(UIPopupRuneBook.Type.SkillBook);
	}

	void onClickOpenHeroInfo(GameObject go)
	{
		GameManager.me.uiManager.popupHeroInfo.init(tabPlayer.currentTab);
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
		refreshMySkills();
	}




	void onClickSort(GameObject go)
	{
		if(mode == Mode.PutOn) return;
		popupSort.open();
	}



	void onClickCancelPutOnMode(GameObject go)
	{
		setMode(Mode.Normal);
	}

	
	public void onCompleteChangeSkill()
	{
		selectSlot = null;
		setMode(Mode.Normal);
		refreshMySkills();
		refreshSkillInven();
	}


	void resetSelectSlot(GameIDData d, bool isInventorySlot)
	{
		if(selectSlot == null && d != null)
		{
			UISkillInvenSlot[] ss = invenList.GetComponentsInChildren<UISkillInvenSlot>();
			foreach(UISkillInvenSlot ts in ss)
			{
				if(ts.data != null && d.serverId == ts.data.serverId)// && ts.isChecked == !isInventorySlot)
				{
					selectSlot = ts;
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

	public void startPutOff(GameIDData gd)
	{
		for(int i = 0; i < 3; ++i)
		{
			if(GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].isOpen && gd != null)
			{
				if(gd.serverId == GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].infoData.serverId)
				{
					switch(i)
					{
					case 0: _slotId = SkillSlot.S1; break;
					case 1: _slotId = SkillSlot.S2; break;
					case 2: _slotId = SkillSlot.S3; break;
					}
				}
			}
		}

		EpiServer.instance.sendUnloadSkillRune(tabPlayer.currentTab, _slotId);
	}


	public UISprite spSelectSlotBorder;
	public GameObject[] spNowSelectedSlots;
	private GameIDData _selectSlotData = null;

	void setMode(Mode m, GameIDData gd = null)
	{
		mode = m;
		
		if(m == Mode.Normal)
		{
			if(selectSlot != null) selectSlot.select = false;
			refreshMySkills();
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
				alreadyPutOnItem = checkSameBaseSkillInMySlot(selectSlot.data.skillData.baseId, tabPlayer.currentTab);
				_selectSlotData = selectSlot.data;
			}
			else if(gd != null)
			{
				spSelectSlotBorder.enabled = false;
				alreadyPutOnItem = checkSameBaseSkillInMySlot(gd.skillData.baseId, tabPlayer.currentTab);
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


			for(int i = 0; i < 3; ++i)
			{
				if(GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].isOpen)
				{
					if(alreadyPutOnItem)
					{
						if(TutorialManager.instance.isTutorialMode) spNowSelectedSlots[i].SetActive(false);
						else spNowSelectedSlots[i].SetActive(_selectSlotData.skillData.baseId == GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].infoData.skillData.baseId);
						nowSlots[i].buttonEnabled = (_selectSlotData.skillData.baseId == GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].infoData.skillData.baseId);
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
					nowSlots[i].buttonEnabled = !alreadyPutOnItem;
				}

			}
		}

	}



	void refreshSort()
	{
		invenList.sortType = LocalDataManager.getInvenSortType(UIInvenSort.InvenType.skill);
		invenList.sortFromHigh = LocalDataManager.isInvenSortDirectionHigh(UIInvenSort.InvenType.skill);
	}

	
	public override void show()
	{
		base.show();

		SoundData.play("uirn_runepopup");

		btnSort.gameObject.SetActive(true);

		popupSort.hide();

		setMode( Mode.Normal );

		selectSlot = null;

		updateGold();
		updateRuby();
		updateRuneStone();

		tabPlayer.tabChangeDispatcher -= changePlayerTab;
		tabPlayer.tabChangeDispatcher += changePlayerTab;
		tabPlayer.init(GameIDData.Type.Skill);


		refreshSort();
		refreshMySkills();
		refreshSkillInven(true);

		reinforcePanel.hide();
		composePanel.hide();
		sellPanel.hide();

		TutorialManager.instance.check("T37");

//		if(GameDataManager.instance.historySkillRunes == null) EpiServer.instance.sendSkillRuneHistory();
	}	
	
	private Dictionary<string, UISkillInvenSlot> mySlotDic = new Dictionary<string, UISkillInvenSlot>(StringComparer.Ordinal);
	
	public void refreshMySkills()
	{
		for(int i = 0; i < 3; ++i)
		{
			if(GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].isOpen)
			{
				nowSlots[i].setData( GameDataManager.instance.playerSkillSlots[tabPlayer.currentTab][i].infoData );
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



	public int refreshSkillInven(bool reposition = false)
	{
		invenList.draw(reposition);

		System.GC.Collect();
		return 0;
	}

	
	int getSettingSkillNum(string id)
	{
		int count = 0;
		foreach(PlayerSkillSlot ps in GameDataManager.instance.skills)
		{
			if(ps.isOpen == true && ps.id.Equals(id)) ++count;
		}

		return count;
	}


	public bool checkSameBaseSkillInMySlot(string baseId, string character)
	{
		int count = 0;
		foreach(PlayerSkillSlot ps in GameDataManager.instance.playerSkillSlots[character])
		{
			if(ps.isOpen == true && ps.infoData.skillData.baseId == baseId)
			{
				return true;
			}
		}
		
		return false;
	}


	// 현재 캐릭터가 아닌 다른 캐릭터가 이 스킬을 차고 있는지 확인해본다.
	public string lastCheckPutOnCharacter;
	public bool checkCanPutOn(string baseId)
	{
		int count = 0;


		foreach(KeyValuePair<string, PlayerSkillSlot[]> kv  in GameDataManager.instance.playerSkillSlots)
		{
			if(tabPlayer.currentTab == kv.Key || GameDataManager.instance.serverHeroData.ContainsKey(kv.Key) == false)
			{
				continue;
			}

			foreach(PlayerSkillSlot ps in kv.Value)
			{
				if(ps.isOpen == true && ps.infoData.skillData.baseId == baseId)
				{
					lastCheckPutOnCharacter = kv.Key;
					return false;
				}
			}
		}

		return true;
	}





	string _slotId;
	public void onSelectSlot(UISkillInvenSlot s, GameIDData data)
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

				GamePlayerData selectHeroData = null;

				GameDataManager.instance.heroes.TryGetValue(tabPlayer.currentTab, out selectHeroData);

				GameManager.me.uiManager.popupSkillPreview.show(data, RuneInfoPopup.Type.Normal, s.isInventorySlot, true, selectHeroData);
			}

		}
		else if(mode == Mode.PutOn)
		{
			if(s.isInventorySlot || _selectSlotData == null) return;

			switch(s.index)
			{
			case 0: _slotId = SkillSlot.S1; break;
			case 1: _slotId = SkillSlot.S2; break;
			case 2: _slotId = SkillSlot.S3; break;
			}

			SoundData.play("uirn_runeattach");
			EpiServer.instance.sendChangeSkillRune(tabPlayer.currentTab, _slotId, _selectSlotData.serverId);
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
			sellPanel.startFromSkill(sellingItemData, slotIndex);
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
						sellPanel.startFromSkill(sellingItemData, invenList.maxPerLine * i + j);
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
			reinforcePanel.startFromSkill(reinforceItemData, slotIndex);
		}
		else
		{
			if(isFromInventorySlot == false)
			{
				reinforcePanel.startFromSkill(reinforceItemData, -1);
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
							reinforcePanel.startFromSkill(reinforceItemData, invenList.maxPerLine * i + j);
							return;
						}
					}
				}
			}
		}
	}


	public UIComposePanel composePanel;

	public void startComposeMode(GameIDData composeItemData, bool isFromInventorySlot)
	{
		resetSelectSlot(composeItemData, isFromInventorySlot);
		
		int slotIndex = -1;
		if(selectSlot != null)
		{
			slotIndex = selectSlot.index;
			if(isFromInventorySlot == false) slotIndex = -1;
			composePanel.startFromSkill(composeItemData, slotIndex);
		}
		else
		{
			if(isFromInventorySlot == false)
			{
				composePanel.startFromSkill(composeItemData, -1);
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
							composePanel.startFromSkill(composeItemData, invenList.maxPerLine * i + j);
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

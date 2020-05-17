using UnityEngine;
using System.Collections;

public class UISummonInvenSlot : MonoBehaviour {
	
	public BoxCollider boxCollider;

	public UISprite spSelectBorder;

	public UISprite spIcon, spCheck, spRareBorder, spSlot, spSlotName, spLock, spLabelBg, spNew;

	public UISprite spCantUse;

	public GameIDData data;
	public UIButton btn;
	
	public UILabel lbItemLevel, lbTranscendLevel;

	public int index = 0;
	public bool isInventorySlot = false;
	public BaseSlot.InventorySlotType slotType = BaseSlot.InventorySlotType.Normal;

	public bool checkCantUse = false;

	public UIDragScrollView scrollView;

	public int iconDepth;

	void Awake ()
	{
		UIEventListener.Get(btn.gameObject).onClick = onClickButton;
	}
	
	// Use this for initialization
	void Start () 
	{
		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.uiMenu.uiSummon.invenList.panel;
	}

	public bool buttonEnabled
	{
		set
		{
			boxCollider.enabled = value;
		}
	}





	public bool select
	{
		set
		{
			if(spSelectBorder != null)
			{
				spSelectBorder.enabled = value;
			}
		}
	}




	public void setData(GameIDData gd, int slotIndex = -1)
	{
		slotType = BaseSlot.InventorySlotType.Normal;

		if(isInventorySlot)
		{
			index = slotIndex;

			setInventorySlot(gd);

			bool isLock = false;


			if(UIReinforceBarPanel.isReinforceMode && gd != null)
			{
				if(UIReinforceBarPanel.isTabSlot == false && index == UIReinforceBarPanel.originalSlotIndex) isLock = true;

				if(isLock == false)
				{
					select = (UIReinforceBarPanel.sourceListIndex.Contains(index));
				}
			}
			else if(UIMultiSellPanel.isMultiSell && gd != null)
			{
				select = (UIMultiSellPanel.sourceListIndex.Contains(index));
			}
			else if(UIComposePanel.isComposeMode && gd != null)
			{
				if(UIComposePanel.isTabSlot == false && index == UIComposePanel.originalSlotIndex) isLock = true;

				else if(UIComposePanel.originalData.rare != data.rare || data.reinforceLevel < GameIDData.MAX_LEVEL)
				{
					isLock = true;
				}

				if(isLock == false)
				{
					select = (UIComposePanel.sourceIndex == index);
				}
			}
			else
			{
				spLock.enabled = false;
			}

			spLock.enabled = isLock;
			btn.isEnabled = !isLock;

			if(spNew != null) spNew.enabled = (data != null && data.isNew);

		}
		else
		{
			setCategorySlot(gd);

		}
	}

	void setCategorySlot(GameIDData gd)
	{
		if(spCantUse != null) spCantUse.enabled = false;

		if(gd == null)
		{
			data = null;
			spIcon.enabled = false;
			spRareBorder.enabled = false;
			spSlot.enabled = false;
			spSlotName.enabled = true;

			btn.isEnabled = false;

			lbItemLevel.enabled = false;
			if(lbTranscendLevel != null) lbTranscendLevel.enabled = false;

			if(spLabelBg != null) spLabelBg.enabled = false;
		}
		else
		{
//			if(str.StartsWith("#"))
//			{
//				str = str.Substring(1);
//				isChecked = true;
//			}

			data = gd;
		
			spIcon.enabled = true;
			spRareBorder.enabled = true;
			spSlot.enabled = true;
			spSlotName.enabled = false;

			data.setUnitIcon(spIcon, iconDepth);

			spIcon.MakePixelPerfect();
			spIcon.width = 102;

			lbItemLevel.enabled = true;
			lbItemLevel.text = "l"+data.level;

			Util.setTranscendLevel(lbTranscendLevel, data.totalPLevel);

			btn.isEnabled = true;

			spSlot.spriteName = RareType.getRareBgSprite(data.rare);
			spRareBorder.spriteName = RareType.getRareLineSprite(data.rare);



			if(spLabelBg != null)
			{
//				spGrade.spriteName = data.getGradeSprite();
				spLabelBg.enabled = true;
			}
		}
	}







	void setInventorySlot(GameIDData gd)
	{
//		isChecked = false;
		
		gameObject.name = "";
		
		if(gd == null)
		{
			data = null;
			spIcon.enabled = false;
			spSlot.spriteName = UIHeroInventorySlot.SLOT_BG_GRADE_NORMAL;
			spRareBorder.spriteName = UIHeroInventorySlot.SLOT_LINE_GRADE_NORMAL;
			spRareBorder.enabled = true;
			btn.isEnabled = false;

			lbItemLevel.enabled = false;
			Util.setTranscendLevel(lbTranscendLevel);

			if(spLabelBg != null) spLabelBg.enabled = false;

			if(spCantUse != null) spCantUse.enabled = false;
		}
		else
		{
			data = gd;

			lbItemLevel.enabled = true;

			spIcon.enabled = true;

//			Debug.Log("data.unitData.resource : " + data.unitData.resource + "   " + data.serverId);


			data.setUnitIcon(spIcon, iconDepth);

			spIcon.MakePixelPerfect();
			spIcon.width = 102;

			lbItemLevel.text = "l"+data.level;

			Util.setTranscendLevel(lbTranscendLevel, data.totalPLevel);

			btn.isEnabled = true;

			spRareBorder.spriteName = RareType.getRareLineSprite(data.rare);
			spSlot.spriteName = RareType.getRareBgSprite(data.rare);

			spRareBorder.enabled = true;

			if(TutorialManager.instance.isTutorialMode && TutorialManager.instance.nowTutorialId == "T44")
			{
				gameObject.name = data.serverId;
			}

			if(spLabelBg != null)
			{
//				spGrade.enabled = true;
//				spGrade.spriteName = data.getGradeSprite();
				spLabelBg.enabled = true;
			}

			refreshCanUse();

		}

		if(spSelectBorder != null) spSelectBorder.enabled = false;
	}


	public void refreshCanUse()
	{
		if(spCantUse != null)
		{
			if(checkCantUse && data != null && data.unitData != null && isInventorySlot && !UIReinforceBarPanel.isReinforceMode && !UIMultiSellPanel.isMultiSell && !UIComposePanel.isComposeMode)
			{
				spCantUse.enabled = !GameManager.me.uiManager.uiMenu.uiSummon.checkCanPutOn(data.unitData);
			}
			else
			{
				spCantUse.enabled = false;
			}
		}
	}



	
//	bool _isChecked = false;
//	
//	public bool isChecked
//	{
//		get
//		{
//			return _isChecked;
//		}
//		set
//		{
//			_isChecked = value;
//			spCheck.gameObject.SetActive(value);
//		}
//	}
	
	
	public delegate void SelectSlot(UISummonInvenSlot s, GameIDData data);
	public SelectSlot selectSlotListener;
	
	public void onSelectSlot(GameObject go)
	{
		selectSlotListener(this, data);
	}
	
	void onClickButton(GameObject go)
	{
		if(slotType == BaseSlot.InventorySlotType.FriendDetailSlot)
		{
			if(data == null) return;

			if(GameManager.me.uiManager.popupShop.popupFriendDetail.gameObject.activeSelf)
			{
				GameManager.me.uiManager.popupShop.popupFriendDetail.showSlotTooltip(spSelectBorder, data.getTooltipDescription() ,transform.localPosition);
			}
			else
			{
				GameManager.me.uiManager.popupFriendDetail.showSlotTooltip(spSelectBorder, data.getTooltipDescription() ,transform.localPosition);
			}
		}
		else if(slotType == BaseSlot.InventorySlotType.HeroInfoSlot)
		{
			GameManager.me.uiManager.popupSummonDetail.show(data, RuneInfoPopup.Type.PreviewOnly, true);
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiSummon.onSelectSlot(this, data);
		}
	}


	void OnDestroy()
	{
		data = null;
	}
	
}

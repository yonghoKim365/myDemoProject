using UnityEngine;
using System.Collections;

public class UISkillInvenSlot : MonoBehaviour {

	public int defaultDepth = -1000;

	public BoxCollider boxCollider;
	
	public UISprite spSlotName, spIcon, spRareBorder, spLock, spSelectBorder, spLabelBg, spNew;//, spSkillNameNum; //spCheck

	public UISprite spCantUse;

	public UILabel lbItemLevel, lbTranscendLevel;
	
	public GameIDData data;
	
	public UIButton btn;

	public int index = 0;
	public bool isInventorySlot = false;
	public bool isMyInven = true;

	public bool checkCantUse = false;


	public BaseSlot.InventorySlotType slotType = BaseSlot.InventorySlotType.Normal;

	public UIDragScrollView scrollView;

	void Awake ()
	{
		UIEventListener.Get(btn.gameObject).onClick = onClickButton;
	}

	// Use this for initialization
	void Start () 
	{
		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.uiMenu.uiSkill.invenList.panel;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public bool buttonEnabled
	{
		set
		{
			boxCollider.enabled = value;
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

				else if(UIComposePanel.originalData.rare != data.rare || data.reinforceLevel < GameIDData.MAX_LEVEL) isLock = true;

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
			if(spLock != null) spLock.enabled = false;
		}



	}


	void setCategorySlot(GameIDData gd)
	{
		if(spCantUse != null) spCantUse.enabled = false;

		gameObject.name = string.Empty;
		
		if(gd == null)
		{
			data = null;


			btn.isEnabled = false;

			spSlotName.enabled = true;		

			spIcon.enabled = false;
			spRareBorder.enabled = false;
//			spSkillNameNum.enabled = false;
			lbItemLevel.enabled = false;

			Util.setTranscendLevel(lbTranscendLevel);

			if(spLabelBg != null) spLabelBg.enabled = false;

		}
		else
		{

			btn.isEnabled = true;

//			if(d.StartsWith("#"))
//			{
//				d = d.Substring(1);
//				isChecked = true;
//			}

			data = gd;

			spSlotName.enabled = false;


			spRareBorder.enabled = true;
//			spSkillNameNum.enabled = true;
			lbItemLevel.enabled = true;



			Icon.setSkillIcon(data.getSkillIcon(), spIcon, defaultDepth);


			spIcon.MakePixelPerfect();
			spIcon.enabled = true;

			spRareBorder.spriteName = RareType.getRareLineSprite(data.rare);


//			if(spSkillNameNum != null)
//			{
//				//				spSkillNameNum.spriteName = data.getGradeSprite();
//				spSkillNameNum.enabled = false;
//			}


			lbItemLevel.text = "l"+data.level;

			Util.setTranscendLevel(lbTranscendLevel, data.totalPLevel);

			if(spLabelBg != null) spLabelBg.enabled = true;

		}
		
		if(spSelectBorder != null) spSelectBorder.enabled = false;


	}





	void setInventorySlot(GameIDData gd)
	{

		gameObject.name = string.Empty;

//		isChecked = false;

		if(gd == null)
		{
			data = null;

			btn.isEnabled = false;

			spLock.enabled = false;
			spIcon.enabled = false;

			spRareBorder.enabled = true;

//			spSkillNameNum.enabled = false;

			lbItemLevel.enabled = false;

			Util.setTranscendLevel(lbTranscendLevel);

			spRareBorder.spriteName = UIHeroInventorySlot.SLOT_LINE_GRADE_NORMAL;

			if(spLabelBg != null) spLabelBg.enabled = false;

			if(spCantUse != null) spCantUse.enabled = false;

		}
		else
		{
			btn.isEnabled = true;

//			if(d.StartsWith("#"))
//			{
//				d = d.Substring(1);
//				isChecked = true;
//			}

			data = gd;

			spLock.enabled = false;

			spRareBorder.enabled = true;
//			spSkillNameNum.enabled = true;

			lbItemLevel.enabled = true;




			Icon.setSkillIcon(data.getSkillIcon(), spIcon);


			spIcon.MakePixelPerfect();
			spIcon.enabled = true;

			spRareBorder.spriteName = RareType.getRareLineSprite(data.rare);


//			if(spSkillNameNum != null)
//			{
////				spSkillNameNum.spriteName = data.getGradeSprite();
//				spSkillNameNum.enabled = false;
//			}

			lbItemLevel.text = "l"+data.level;
			Util.setTranscendLevel(lbTranscendLevel, data.totalPLevel);

			if(spLabelBg != null) spLabelBg.enabled = true;

			refreshCanUse();
		}

		if(spSelectBorder != null) spSelectBorder.enabled = false;
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
			GamePlayerData selectHeroData = null;
			GameDataManager.instance.heroes.TryGetValue( GameManager.me.uiManager.uiMenu.uiSkill.tabPlayer.currentTab, out selectHeroData);
			GameManager.me.uiManager.popupSkillPreview.show(data, RuneInfoPopup.Type.PreviewOnly, true, true, selectHeroData);

		}
		else if(isMyInven == false)
		{
			if(data == null) return;
			GameManager.me.uiManager.uiVisitingLobby.onClickSkill(data);
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiSkill.onSelectSlot(this, data);
		}

	}



	public void refreshCanUse()
	{
		if(spCantUse != null)
		{
			if(checkCantUse && data != null && data.skillData != null && isInventorySlot && !UIReinforceBarPanel.isReinforceMode && !UIMultiSellPanel.isMultiSell && !UIComposePanel.isComposeMode)
			{
				spCantUse.enabled = !GameManager.me.uiManager.uiMenu.uiSkill.checkCanPutOn(data.baseId);
			}
			else
			{
				spCantUse.enabled = false;
			}
		}
	}
	
	
}

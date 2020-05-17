using UnityEngine;
using System.Collections;

public class UIHeroInventorySlot : MonoBehaviour {
	
	public UISprite spCantUse, spPartsIcon, spRareBorder, spSelect, spSlotRareBg, spLock, spLabelBg, spNew;

	public UILabel lbInforceLevel, lbTranscendLevel;

	public GameIDData data;
	
	public UIButton btn;

	public int iconDepth = -1000;

	public BaseSlot.InventorySlotType slotType = BaseSlot.InventorySlotType.Normal;


	void Awake()
	{
		UIEventListener.Get(btn.gameObject).onClick = onSelectSlot;
	}

	public const string LOCK_NOICON = "img_list_slot_lock";//"img_list_slot_lock_noicon";
	public const string LOCK_ICON = "img_list_slot_lock";

	public const string SLOT_BG_QUESTION = "mark_question";

	public const string SLOT_BG_GRADE_D = "img_list_slotbg_d";
	public const string SLOT_LINE_GRADE_D = "img_list_slotline_d";

	public const string SLOT_BG_GRADE_C = "img_list_slotbg_c";
	public const string SLOT_LINE_GRADE_C = "img_list_slotline_c";

	public const string SLOT_BG_GRADE_B = "img_list_slotbg_b";
	public const string SLOT_LINE_GRADE_B = "img_list_slotline_b";

	public const string SLOT_BG_GRADE_A = "img_list_slotbg_a";
	public const string SLOT_LINE_GRADE_A = "img_list_slotline_a";

	public const string SLOT_BG_GRADE_S = "img_list_slotbg_s";
	public const string SLOT_LINE_GRADE_S = "img_list_slotline_s";

	public const string SLOT_BG_GRADE_SS = "img_list_slotbg_ss";
	public const string SLOT_LINE_GRADE_SS = "img_list_slotline_ss";

	public const string SLOT_BG_GRADE_NORMAL = "img_list_slotbg_normal";
	public const string SLOT_LINE_GRADE_NORMAL = "img_list_slotline_normal";


	public void setStringData(string str, bool useCantUseCover = true, int slotIndex = -1)
	{
		GameIDData gd = new GameIDData();
		gd.parse(str, GameIDData.Type.Equip);
		setData(gd, useCantUseCover, slotIndex);
	}

	public void setData(GameIDData gd, bool useCantUseCover = true, int slotIndex = -1)
	{
		slotType = BaseSlot.InventorySlotType.Normal;

		//gameObject.name = string.Empty;

		if(gd == null)
		{
			btn.isEnabled = false;
			data = null;

			spCantUse.gameObject.SetActive(false);

			spPartsIcon.gameObject.SetActive(false);

			spSelect.gameObject.SetActive(false);
			spRareBorder.gameObject.SetActive(true);
			spSlotRareBg.gameObject.SetActive(true);

			lbInforceLevel.gameObject.SetActive(false);

			spRareBorder.spriteName = SLOT_LINE_GRADE_NORMAL;
			spSlotRareBg.spriteName = SLOT_BG_GRADE_NORMAL;

			if(spLock != null) spLock.enabled = false;
			if(spLabelBg != null) spLabelBg.enabled = false;

			if(spNew != null) spNew.enabled = false;

			Util.setTranscendLevel(lbTranscendLevel);

		}
		else
		{
			btn.isEnabled = true;

			data = gd;

			if(UIReinforceBarPanel.isReinforceMode)
			{
				spCantUse.gameObject.SetActive(false);
			}
			else
			{
				spCantUse.gameObject.SetActive(data.partsData.character != UIHero.nowHero && useCantUseCover);
			}

			spSelect.gameObject.SetActive(false);

			spPartsIcon.gameObject.SetActive(true);
			spSlotRareBg.gameObject.SetActive(true);
			spRareBorder.gameObject.SetActive(true);

			spRareBorder.spriteName = RareType.getRareLineSprite(data.rare);
			spSlotRareBg.spriteName = RareType.getRareBgSprite(data.rare);


			Icon.setEquipIcon(data.partsData.getIcon(), spPartsIcon, iconDepth);


			lbInforceLevel.gameObject.SetActive(true);

			if(data.level > 0)
			{
				lbInforceLevel.text = "l"+data.level;
			}
			else
			{
				lbInforceLevel.text = "-";
			}

			Util.setTranscendLevel(lbTranscendLevel, data.totalPLevel);

			index = slotIndex;
			
			bool isLock = false;
			
			if(UIReinforceBarPanel.isReinforceMode)
			{
				if(index == UIReinforceBarPanel.originalSlotIndex) isLock = true;

				if(isLock == false)
				{
					select = (UIReinforceBarPanel.sourceListIndex.Contains(index));
				}
			}
			else if(UIMultiSellPanel.isMultiSell)
			{
				select = (UIMultiSellPanel.sourceListIndex.Contains(index));
			}
			else if(UIComposePanel.isComposeMode)
			{
				if(index == UIComposePanel.originalSlotIndex) isLock = true;

				else if(UIComposePanel.originalData.rare != data.rare || 
				        data.reinforceLevel < GameIDData.MAX_LEVEL
				        )
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



			if(spLabelBg != null) spLabelBg.enabled = true;



		}

		if(spNew != null) spNew.enabled = (data != null && data.isNew);
	}


	bool _isUse = false;
	
	public bool isUse
	{
		get
		{
			return _isUse;
		}
		set
		{
			_isUse = value;
			btn.enabled = !value;
		}
	}


	public void onSelectSlot(GameObject go)
	{
		if(data == null) return;

		if(slotType == BaseSlot.InventorySlotType.FriendDetailSlot)
		{
			if(GameManager.me.uiManager.popupShop.popupFriendDetail.gameObject.activeSelf)
			{
				GameManager.me.uiManager.popupShop.popupFriendDetail.showSlotTooltip(spSelect, data.getTooltipDescription() ,transform.localPosition);
			}
			else
			{
				GameManager.me.uiManager.popupFriendDetail.showSlotTooltip(spSelect, data.getTooltipDescription() ,transform.localPosition);
			}

		}
		else if(slotType == BaseSlot.InventorySlotType.HeroInfoSlot)
		{
			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show(data, RuneInfoPopup.Type.PreviewOnly, true);
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiHero.onClickSlot(this, data);
		}

	}

	public int index = -1;

	public bool select
	{
		set
		{
			spSelect.gameObject.SetActive(value);
		}
	}


}

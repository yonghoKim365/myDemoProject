using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIChallengeItemSlot : MonoBehaviour {

	public ParticleSystem particle;

	public UISprite spRareBorder, spBackground, spSelect, spSlotBlackLock, spSlotCheck, spLevelBg;

	// 챌린지 용으로 썼음... 이제 안씀...
	public UISprite spMarkConfirm;

	public UISprite spEquipIcon, spUnitIcon, spSkillIcon, spDefaultIcon;

	public UIButton btnInfo;

	public UILabel lbInforceLevel, lbTranscendLevel;

	public int index = 0;

	public int unitSpriteDepth = 41;

	public int count;

	public delegate void UIChallengeItemSlotCallback(UIChallengeItemSlot slot);
	public UIChallengeItemSlotCallback callback = null;

	public UIDragScrollView scrollView;

//	public Collider cachedCollider;

	void Awake()
	{
		if(btnInfo != null) UIEventListener.Get(btnInfo.gameObject).onClick = onClickSlot;
//		if(cachedCollider == null) cachedCollider = collider;
	}

	public bool useButton = false;


	public void onClickSlot(GameObject go)
	{
//		Debug.LogError(go.name);

		if(useButton == false)
		{
			return;
		}

		if(callback != null)
		{
			callback(this);
			return;
		}

		switch(type)
		{
//		case Type.Equip:
//			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show(infoData, UIHeroItemDetailPopup.Type.InstantBuy, false, itemData);
//			break;
//
//		case Type.Skill:
//			GameManager.me.uiManager.popupSkillPreview.showInstantBuy(itemData, infoData);
//			break;
//
//		case Type.Unit:
//			GameManager.me.uiManager.popupSummonDetail.showChallengeItem( itemData, infoData);
//			break;

//		case Type.SelectChampionshipResultSlot:
//			GameManager.me.uiManager.popupChallengeResult.resultSelector.onClick(this);
//			break;

		case Type.StageRewardPreviewItem:
			if(infoData.type == GameIDData.Type.None) return;
			GameManager.me.uiManager.uiMenu.uiWorldMap.stageDetail.onClickRewardItem(this);
			//infoData.getTooltipDescription();
			break;

		case Type.Reforge:
			if(infoData.type == GameIDData.Type.None) return;
			GameManager.me.uiManager.popupReforege.onClickSlot(this);
			//infoData.getTooltipDescription();
			break;

		case Type.BookHiddenType:

			break;

		case Type.BookSkillType:
			GameManager.me.uiManager.popupSkillPreview.show( infoData, RuneInfoPopup.Type.Book, false);
			break;

		case Type.BookUnitType:
			GameManager.me.uiManager.popupSummonDetail.show( infoData, RuneInfoPopup.Type.Book, infoData.rare, infoData.level );
			break;

		case Type.BookEquipType:

			GamePlayerData compareHero = null;
			GameIDData compareData = null;

			if(infoData.partsData.character == Character.LEO)
			{
				GameDataManager.instance.heroes.TryGetValue(Character.LEO, out compareHero);
			}
			else if(infoData.partsData.character == Character.KILEY)
			{
				GameDataManager.instance.heroes.TryGetValue(Character.KILEY, out compareHero);
			}
			else if(infoData.partsData.character == Character.CHLOE)
			{
				GameDataManager.instance.heroes.TryGetValue(Character.CHLOE, out compareHero);
			}

			if(compareHero != null)
			{
				if(infoData.partsData.type == HeroParts.HEAD)
				{
					compareData = compareHero.partsHead.itemInfo;
				}
				else if(infoData.partsData.type == HeroParts.BODY)
				{
					compareData = compareHero.partsBody.itemInfo;
				}
				else if(infoData.partsData.type == HeroParts.WEAPON)
				{
					compareData = compareHero.partsWeapon.itemInfo;
				}
				else if(infoData.partsData.type == HeroParts.VEHICLE)
				{
					compareData = compareHero.partsVehicle.itemInfo;
				}
			}

			GameManager.me.uiManager.uiMenu.uiHero.itemDetailPopup.show ( infoData, RuneInfoPopup.Type.Book, false, false, null, compareData);
			break;

		}
	}

//	P_ChallengeItem itemData;

	public enum Type
	{
		Skill, Unit, Equip, GameItem, SelectChampionshipResultSlot, StageRewardPreviewItem, BookSkillType, BookUnitType, BookEquipType, BookHiddenType,
		SelectTranscendStuff, SelectEvolutionStuff, EvolutionRuneStone, EvolutionRune, Reforge
	}

	public Type type;
	public GameIDData infoData = new GameIDData();
	public string inputItemId;

	string _iconResourceId;

	public void setData(string itemId)
	{
		inputItemId = itemId;

		switch (itemId.Substring(0,2))
		{
		case "EN":
			type = Type.GameItem;
			infoData.type = GameIDData.Type.None;
			_iconResourceId = WSDefine.ICON_REWARD_ENERGY;
			lbInforceLevel.text = itemId.Substring(itemId.LastIndexOf("_")+1);
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			break;
		case "GO":
			type = Type.GameItem;
			infoData.type = GameIDData.Type.None;
			_iconResourceId = WSDefine.ICON_REWARD_GOLD;
			lbInforceLevel.text = itemId.Substring(itemId.LastIndexOf("_")+1);
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			break;
		case "RU":
			type = Type.GameItem;
			infoData.type = GameIDData.Type.None;
			_iconResourceId = WSDefine.ICON_REWARD_RUBY;
			lbInforceLevel.text = itemId.Substring(itemId.LastIndexOf("_")+1);
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			break;

		case "RS":
			type = Type.GameItem;
			infoData.type = GameIDData.Type.None;
			_iconResourceId = WSDefine.ICON_REWARD_RUNESTONE;
			lbInforceLevel.text = itemId.Substring(itemId.LastIndexOf("_")+1);
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			break;


		case "EX":
			type = Type.GameItem;
			_iconResourceId = WSDefine.ICON_REWARD_EXP;
			infoData.type = GameIDData.Type.None;
			lbInforceLevel.text = itemId.Substring(itemId.LastIndexOf("_")+1);
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			break;
			
		case "LE":
		case "KI":
		case "CH":
			type = Type.Equip;
			infoData.parse(itemId, GameIDData.Type.Equip);
			_iconResourceId = infoData.resourceId;
			lbInforceLevel.pivot = UIWidget.Pivot.BottomRight;
			break;
			
		case "SK":
			type = Type.Skill;
			infoData.parse(itemId, GameIDData.Type.Skill);
			_iconResourceId = infoData.resourceId;
			lbInforceLevel.pivot = UIWidget.Pivot.BottomRight;
			break;
			
		case "UN":
			type = Type.Unit;
			infoData.parse(itemId, GameIDData.Type.Unit);
			_iconResourceId = infoData.resourceId;
			lbInforceLevel.pivot = UIWidget.Pivot.BottomRight;
			break;
		}

		draw();


		useButton = false;
		select = false;
	}


	public void setTransendLevel()
	{
		if(infoData != null)
		{
			Util.setTranscendLevel(lbTranscendLevel, infoData.totalPLevel);
		}
	}



	private string _temp;

	private static string[] _countParser;

	public void setSigongData(string itemId, int chance)
	{
		inputItemId = itemId;

		type = Type.GameItem;
		infoData.type = GameIDData.Type.None;

		bool isRuneType = true;
		int runeRare = -1;

		_temp = itemId.Substring(0,2);

		switch (_temp)
		{
		case "EN":
			_iconResourceId = WSDefine.ICON_REWARD_ENERGY;
			isRuneType = false;
			break;
		case "GO":
			_iconResourceId = WSDefine.ICON_REWARD_GOLD;
			isRuneType = false;
			break;
		case "RU":
			_iconResourceId = WSDefine.ICON_REWARD_RUBY;
			isRuneType = false;
			break;
			
		case "RS":
			_iconResourceId = WSDefine.ICON_REWARD_RUNESTONE;
			isRuneType = false;
			break;
		case "EX":
			_iconResourceId = WSDefine.ICON_REWARD_EXP;
			isRuneType = false;
			break;
		}

		if(isRuneType == true)
		{
			_temp = itemId.Substring(0,1);

			switch(_temp)
			{
			case "E":
				_iconResourceId = "mark_question_herol";
				break;
				
			case "S":
				_iconResourceId = "mark_question_skill";
				break;
				
			case "U":
				_iconResourceId = "mark_question_animal";
				break;
			}
		}

		spMarkConfirm.enabled = (chance == 100);

		if(isRuneType)
		{
			_temp = itemId.Substring(1,1);

			switch(_temp)
			{
			case "X":
				runeRare = RareType.SS;
				break;
			case "S":
				runeRare = RareType.S;
				break;
			case "A":
				runeRare = RareType.A;
				break;
			case "B":
				runeRare = RareType.B;
				break;
			case "C":
				runeRare = RareType.C;
				break;
			case "D":
				runeRare = RareType.D;
				break;
			}

			draw(false, runeRare);
		}
		else
		{
			_countParser = itemId.Substring(itemId.IndexOf("_")+1).Split('_');

			if(_countParser[0] == _countParser[1])
			{
				lbInforceLevel.text = _countParser[0];
			}
			else
			{
				lbInforceLevel.text = _countParser[0] + "~" + _countParser[1];
			}

			lbInforceLevel.enabled = true;
			spLevelBg.enabled = true;
			lbInforceLevel.pivot = UIWidget.Pivot.Bottom;
			draw(true);
		}

		if(spLevelBg.enabled)
		{
			spDefaultIcon.cachedTransform.localPosition = new Vector3(0,7,0);
		}
		else
		{
			spDefaultIcon.cachedTransform.localPosition = Vector3.zero;
		}

		useButton = false;
		select = false;

		Util.setTranscendLevel(lbTranscendLevel);
	}




	public void setChallengeResultSlot()
	{
		type = Type.SelectChampionshipResultSlot;
		useButton = true;
	}


	public void setBookType(Type bookType, RuneBookData bookData)
	{
		type = bookType;

		useButton = true;

		lbInforceLevel.pivot = UIWidget.Pivot.BottomRight;

		if(bookData.nowCount > 0)
		{
			check = true;
			lbInforceLevel.text = bookData.nowCount.ToString();
		}
		else if(bookData.hasCount > 0)
		{
			check = false;
		}
		else
		{
			spSlotBlackLock.enabled = true;
			check = false;
		}

		lbInforceLevel.text = "l20";
	}







	public void setReforgeData(GameIDData idData, bool useTransLevelLabel)
	{
		if(scrollView == null)
		{
			scrollView = gameObject.AddComponent<UIDragScrollView>();
		}
		
		if(idData == null)
		{
			useButton = false;
			
			//if(cachedCollider != null) cachedCollider.enabled = false;
			
			spRareBorder.spriteName = UIHeroInventorySlot.SLOT_LINE_GRADE_NORMAL;
			spBackground.spriteName = UIHeroInventorySlot.SLOT_BG_GRADE_NORMAL;
			
			spDefaultIcon.enabled = false;
			
			spEquipIcon.enabled = false;
			spSkillIcon.enabled = false;
			spUnitIcon.enabled = false;
			
			if(spSlotBlackLock != null) spSlotBlackLock.enabled = false;
			
			check = false;
			
			showLevelBar = false;
			Util.setTranscendLevel(lbTranscendLevel);
			
			
		}
		else
		{
			inputItemId = idData.serverId;
			infoData = idData;
			
			switch(infoData.type)
			{
			case GameIDData.Type.Equip: type = Type.Equip; break;
			case GameIDData.Type.Unit: type = Type.Unit; break;
			case GameIDData.Type.Skill: type = Type.Skill; break;
			}
			
			draw();
			type = Type.Reforge;
			useButton = true;

			if(useTransLevelLabel && infoData.totalPLevel > 0)
			{
				lbTranscendLevel.enabled = true;
				Util.setTranscendLevel(lbTranscendLevel, infoData.totalPLevel);

			}
			else
			{
				lbTranscendLevel.enabled = false;
			}


			check = false;
			select = false;

		}
	}











	public void setTranscendStuffData(GameIDData idData, int inputIndex = -1)
	{
		if(scrollView == null)
		{
			scrollView = gameObject.AddComponent<UIDragScrollView>();
//			scrollView.scrollView = GameManager.me.uiManager.popupSelectStuff.list.panel;
		}

		if(idData == null)
		{
			useButton = false;

			//if(cachedCollider != null) cachedCollider.enabled = false;

			spRareBorder.spriteName = UIHeroInventorySlot.SLOT_LINE_GRADE_NORMAL;
			spBackground.spriteName = UIHeroInventorySlot.SLOT_BG_GRADE_NORMAL;

			spDefaultIcon.enabled = false;

			spEquipIcon.enabled = false;
			spSkillIcon.enabled = false;
			spUnitIcon.enabled = false;

			if(spSlotBlackLock != null) spSlotBlackLock.enabled = false;

			check = false;

			showLevelBar = false;
			Util.setTranscendLevel(lbTranscendLevel);


		}
		else
		{
//			if(cachedCollider != null) cachedCollider.enabled = true;

			index = inputIndex;
			inputItemId = idData.serverId;
			infoData = idData;

			switch(infoData.type)
			{
			case GameIDData.Type.Equip: type = Type.Equip; break;
			case GameIDData.Type.Unit: type = Type.Unit; break;
			case GameIDData.Type.Skill: type = Type.Skill; break;
			}

			draw();
			type = Type.SelectTranscendStuff;
			useButton = true;

			//bool isCheck = (UIPopupStuff.selectRuneIndex > -1 && UIPopupStuff.selectRuneIndex == index);

			//check = isCheck;

			//if(isCheck)
			{
//				GameManager.me.uiManager.popupSelectStuff.slotSelected = this;
			}
		}
	}













	public void setData(Type slotType, GameIDData idData)
	{
		inputItemId = idData.serverId;
		type = slotType;
		infoData = idData;
		draw();

		if(slotType == Type.Skill || slotType == Type.Equip || slotType == Type.Unit)
		{
			setTransendLevel();
		}
		else if(lbTranscendLevel != null)
		{
			lbTranscendLevel.enabled = false;
		}

	}


	void draw(bool visibleLevelUpBar = true, int rare = -1)
	{

		switch(type)
		{
		case Type.GameItem:
			spDefaultIcon.spriteName = _iconResourceId;
			spDefaultIcon.enabled = true;
			spDefaultIcon.MakePixelPerfect();
			spEquipIcon.enabled = false;
			spSkillIcon.enabled = false;
			spUnitIcon.enabled = false;


			if(rare > -1)
			{
				spBackground.spriteName = RareType.getRareBgSprite(rare);
				spRareBorder.spriteName = RareType.getRareLineSprite(rare);
			}
			else
			{
				spRareBorder.spriteName = UIHeroInventorySlot.SLOT_LINE_GRADE_NORMAL;
				spBackground.spriteName = UIHeroInventorySlot.SLOT_BG_GRADE_NORMAL;
			}
			break;
		case Type.Equip:

			Icon.setEquipIcon(infoData.getHeroPartsIcon(), spEquipIcon);

			spEquipIcon.enabled = true;
			spEquipIcon.MakePixelPerfect();
			spEquipIcon.width = 102;
			spEquipIcon.height = 102;
//			spEquipIcon.cachedTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			
			spDefaultIcon.enabled = false;
			spSkillIcon.enabled = false;
			spUnitIcon.enabled = false;
			
			spBackground.spriteName = RareType.getRareBgSprite(infoData.rare);
			spRareBorder.spriteName = RareType.getRareLineSprite(infoData.rare);
			lbInforceLevel.text = "l"+infoData.level;
			break;
		case Type.Unit:

			//spUnitIcon.atlas = 
			//spUnitIcon.spriteName = infoData.getUnitIcon();

			infoData.setUnitIcon(spUnitIcon, unitSpriteDepth);

			spUnitIcon.enabled = true;
			spUnitIcon.MakePixelPerfect();
			spUnitIcon.width = 102;
			spEquipIcon.height = 102;
			//spUnitIcon.cachedTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			
			spDefaultIcon.enabled = false;
			spSkillIcon.enabled = false;
			spEquipIcon.enabled = false;
			
			spBackground.spriteName = RareType.getRareBgSprite(infoData.rare);
			spRareBorder.spriteName = RareType.getRareLineSprite(infoData.rare);
			lbInforceLevel.text = "l"+infoData.level;
			break;
		case Type.Skill:

			Icon.setSkillIcon(infoData.getSkillIcon(), spSkillIcon);

			spSkillIcon.enabled = true;
			spSkillIcon.MakePixelPerfect();

			spDefaultIcon.enabled = false;
			spUnitIcon.enabled = false;
			spEquipIcon.enabled = false;
			
			spBackground.spriteName = RareType.getRareBgSprite(infoData.rare);
			spRareBorder.spriteName = RareType.getRareLineSprite(infoData.rare);
			lbInforceLevel.text = "l"+infoData.level;
			break;
		}

//		Util.setTranscendLevel(lbTranscendLevel, infoData.transcendLevel.Get());

		showLevelBar = visibleLevelUpBar;

		check = false;
		if(spSlotBlackLock != null) spSlotBlackLock.enabled = false;
	}



	public void setLevel(int newLevel)
	{
		lbInforceLevel.text = "l"+newLevel;
	}

	public void setRare(int newRare)
	{
		if(newRare > RareType.SS) newRare = RareType.SS;

		spBackground.spriteName = RareType.getRareBgSprite(newRare);
		spRareBorder.spriteName = RareType.getRareLineSprite(newRare);
	}



//	public void setData(P_ChallengeItem item)
//	{
//		itemData = item;
//		setData(itemData.id);
//		useButton = true;
//	}

	public bool select
	{
		set
		{
			spSelect.enabled = value;
		}
	}

	private bool _check = false;
	public bool check
	{
		set
		{
			if(spSlotCheck != null) spSlotCheck.enabled = value;
			_check = value;
		}
		get
		{
			return _check;
		}
	}


	public bool showLevelBar
	{
		set
		{
			lbInforceLevel.enabled = value;
			if(spLevelBg != null) spLevelBg.enabled = value;
		}
	}



}

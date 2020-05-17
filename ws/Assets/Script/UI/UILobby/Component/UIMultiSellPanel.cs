using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMultiSellPanel : MonoBehaviour 
{
	public UILabel lbDefaultMessage, lbPrice;
	
	public UIButton btnSell;
	public UIButton btnClose;
	
	bool nowPlayingDisplayEffect = false;
	
	
	void Awake()
	{
		UIEventListener.Get( btnSell.gameObject ).onClick = onClickSell;
		UIEventListener.Get( btnClose.gameObject ).onClick = onClickClose;
	}
	
	void onClickSell(GameObject go)
	{
		if(sourceList.Count <= 0)
		{
			return;
		}

		string[] sourceIds = new string[sourceList.Count];
		for(int i = sourceList.Count - 1; i >= 0; --i)
		{
			sourceIds[i] = sourceList[i].serverId;
		}
		
		switch(_type)
		{
		case Type.Unit:
			EpiServer.instance.sendSellUnitRune(sourceIds);
			break;
		case Type.Skill:
			EpiServer.instance.sendSellSkillRune(sourceIds);
			break;
		case Type.Equip:
			EpiServer.instance.sendSellEquip(sourceIds);
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
		if(isMultiSell)
		{
			isMultiSell = false;
			
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
		
		nowPlayingDisplayEffect = false;
		
		sourceListIndex.Clear();
		sourceList.Clear();
		
		gameObject.SetActive(false);
		
	
	
	}
	

	public static bool isMultiSell = false;
	public static GameIDData originalData;
	

	
	public enum Type
	{
		Unit, Skill, Equip
	}
	
	Type _type = Type.Unit;
	
	
	void init()
	{
		gameObject.SetActive(true);
		
		isMultiSell = true;
		
		sourceListIndex.Clear();
		sourceList.Clear();

	}
	
	public void startFromSummon(GameIDData originalItemData, int slotIndex)
	{
		_type 	= Type.Unit;
		
		lbDefaultMessage.text = Util.getUIText("MULTISELL_UNIT");
		
		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);

		init();
		
		sourceListIndex.Add(slotIndex);
		sourceList.Add(originalItemData);
		
		refresh(true);
		
		GameManager.me.uiManager.uiMenu.uiSummon.invenList.draw(false);
	}
	
	
	
	public void startFromSkill(GameIDData originalItemData, int slotIndex)
	{
		GameManager.me.uiManager.uiMenu.uiSkill.btnSort.gameObject.SetActive(false);
		
		_type = Type.Skill;

		lbDefaultMessage.text = Util.getUIText("MULTISELL_SKILL");
		
		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);

		init();
		
		sourceListIndex.Add(slotIndex);
		sourceList.Add(originalItemData);
		
		refresh(true);
		
		GameManager.me.uiManager.uiMenu.uiSkill.invenList.draw(false);
	}

	public void startFromHeroInven(GameIDData originalItemData, int slotIndex)
	{
		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);

		_type = Type.Equip;
		lbDefaultMessage.text = Util.getUIText("MULTISELL_EQUIP");

		init();

		sourceListIndex.Add(slotIndex);
		sourceList.Add(originalItemData);

		refresh(true);

		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);
//		GameManager.me.uiManager.uiMenu.uiHero.btnShowCharacterInfo.isEnabled = false;

		GameManager.me.uiManager.uiMenu.uiHero.invenList.draw(false);
	}

	
	
	public static List<int> sourceListIndex = new List<int>();
	public List<GameIDData> sourceList = new List<GameIDData>();
	
	
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

	
	int onClick (int slotIndex, GameIDData slotData)
	{
		if(sourceListIndex.Contains(slotIndex) == false)
		{
			if(sourceListIndex.Count < 10)
			{
				sourceListIndex.Add(slotIndex);
				sourceList.Add(slotData);
				return 1;
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, "한번에 10개까지 판매할 수 있습니다.");
			}
		}
		else
		{
			int tempI = sourceListIndex.IndexOf(slotIndex);
			sourceListIndex.RemoveAt(tempI);
			sourceList.RemoveAt(tempI);
			return -1;
		}
		return 0;
	}
	
	
	private int _currentPrice = 0;
	private int _prevPrice = 0;

	private Vector3 _v;

	
	void refresh(bool isInit = false)
	{
		int resultReinforceLevel = 0;
		float resultPer = 0;
		_currentPrice = 0;

		for(int i = sourceList.Count - 1; i >= 0; --i)
		{
			_currentPrice += sourceList[i].getSellPrice();
		}

		++_workIndex;
		
		if(isInit)
		{
			setPrice(_currentPrice);
		}
		else
		{
			StartCoroutine(valueUpdater(_workIndex));
		}
		
		btnSell.isEnabled = (sourceList.Count > 0);
	}
	
	
	void setPrice(int setPrice)
	{
		lbPrice.text = Util.GetCommaScore(setPrice);
		_prevPrice = setPrice;
	}
	
	
	int _workIndex = 0;
	WaitForSeconds ws005 = new WaitForSeconds(0.05f);
	
	
	IEnumerator valueUpdater(int workIndex)
	{
		float duration = 0.5f;
		float time = 0.0f;
		bool isComplete = false;

		int startPrice = _prevPrice;

		while(workIndex == _workIndex && isComplete == false)
		{
			time += 0.05f;
			float step = time / duration;
			step = Easing.EaseOut( time / duration, EasingType.Cubic);
			
			if(time >= duration) 
			{
				step = 1.0f;
				isComplete = true;
			}
			
			if(isComplete)
			{
				setPrice(_currentPrice);
			}
			else
			{
				setPrice(Mathf.RoundToInt( Mathf.Lerp( startPrice, _currentPrice , step) ));
			}
			
			yield return ws005;
		}

	}




}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIReinforceBarPanel : MonoBehaviour 
{
	public UIChallengeItemSlot slotOriginal;

	public UIReinforceBarPanelProgressBar originalProgressBar;
	public UIReinforceBarPanelProgressBar resultProgressBar;

	public UILabel lbDefaultMessage, lbPrice;

	public UIButton btnReinforce;
	public UIButton btnClose;

	bool nowPlayingDisplayEffect = false;


	void Awake()
	{
		UIEventListener.Get( btnReinforce.gameObject ).onClick = onClickReinforce;
		UIEventListener.Get( btnClose.gameObject ).onClick = onClickClose;
	}

	void onClickReinforce(GameObject go)
	{
		if(GameDataManager.instance.gold < _currentReinforcePrice && TutorialManager.instance.isTutorialMode == false)
		{
			GameManager.me.uiManager.popupGoldForRuby.show (_currentReinforcePrice, onConfirmReinforce);
		}
		else
		{
			onConfirmReinforce();
		}

	}

	void onConfirmReinforce()
	{
		string[] sourceIds = new string[sourceList.Count];
		for(int i = sourceList.Count - 1; i >= 0; --i)
		{
			sourceIds[i] = sourceList[i].serverId;
		}

		RuneStudioMain.instance.reinforceOriginalData = originalData;

		switch(_type)
		{
		case Type.Unit:
			EpiServer.instance.sendReinforceUnitRune(originalData.serverId, sourceIds);
			break;
		case Type.Skill:
			EpiServer.instance.sendReinforceSkillRune(originalData.serverId, sourceIds);
			break;
		case Type.Equip:
			EpiServer.instance.sendReinforceEquip(originalData.serverId, sourceIds);
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
		if(isReinforceMode)
		{
			isReinforceMode = false;

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

		originalData = null;
		originalSlotIndex = -1;
	}

	public static int originalSlotIndex = -1;
	public static bool isReinforceMode = false;
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

		isReinforceMode = true;
		
		sourceListIndex.Clear();
		sourceList.Clear();

		originalDisplayData.progressBarValue = originalData.getReinforceProgressPercent();
		originalDisplayData.progressLabelValue = Mathf.RoundToInt(originalDisplayData.progressBarValue * 100.0f);

		targetDisplayData.progressBarValue = originalDisplayData.progressBarValue; 
		targetDisplayData.progressLabelValue = originalDisplayData.progressLabelValue;

		refresh(true);
	}

	public void startFromSummon(GameIDData originalItemData, int slotIndex)
	{
		_type 	= Type.Unit;

//		isTabSlot = !originalSlot.isInventorySlot;

		lbDefaultMessage.text = Util.getUIText("REINFORCE_UNIT");

		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);

		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse(originalItemData.serverId, originalItemData.type);

		isTabSlot = (slotIndex < 0);

		slotOriginal.setData(UIChallengeItemSlot.Type.Unit, originalData);
		slotOriginal.setTransendLevel();

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

		lbDefaultMessage.text = Util.getUIText("REINFORCE_SKILL");
		
		GameManager.me.uiManager.uiMenu.uiSummon.btnSort.gameObject.SetActive(false);
		
		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse(originalItemData.serverId, originalItemData.type);

		isTabSlot = (slotIndex < 0);

		slotOriginal.setData(UIChallengeItemSlot.Type.Skill, originalData);

		init();

		GameManager.me.uiManager.uiMenu.uiSkill.invenList.draw(false);
	}

	public void startFromHeroTab(GameIDData originalItemData)
	{
		_type = Type.Equip;

		isTabSlot = true;
		
		lbDefaultMessage.text = Util.getUIText("REINFORCE_EQUIP");

		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);

		originalSlotIndex = -1;
		originalData = new GameIDData();
		originalData.parse(originalItemData.serverId, originalItemData.type);

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
		_type = Type.Equip;
		
		isTabSlot = false;
		
		lbDefaultMessage.text = Util.getUIText("REINFORCE_EQUIP");

		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);

		originalSlotIndex = slotIndex;
		originalData = new GameIDData();
		originalData.parse(originalItemData.serverId, originalItemData.type);
		
		slotOriginal.setData(UIChallengeItemSlot.Type.Equip, originalData);

		init();

		GameManager.me.uiManager.uiMenu.uiHero.btnSort.gameObject.SetActive(false);
		GameManager.me.uiManager.uiMenu.uiHero.invenList.draw(false);
//		GameManager.me.uiManager.uiMenu.uiHero.btnShowCharacterInfo.isEnabled = false;
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


	public void onClick(UIHeroInventoryTab invenSlot)
	{
		onClick(-1000, invenSlot.data);

		refresh();
	}


	int onClick (int slotIndex, GameIDData slotData)
	{
		if(sourceListIndex.Contains(slotIndex) == false)
		{
			if(sourceList.Count > 0 && _enoughSelection)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REINFORCE_UNNECESSARRY"));
				return 0;
			}

			if(sourceListIndex.Count < 5)
			{
				sourceListIndex.Add(slotIndex);
				sourceList.Add(slotData);
				return 1;
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("REINFORCE_SELECT_LIMIT"));
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


	private int _currentReinforcePrice = 0;
	private Vector3 _v;

	bool _enoughSelection = false;



	void refresh(bool isInit = false)
	{
		int resultReinforceLevel = 0;
		float resultPer = 0;
		_currentReinforcePrice = 0;

		originalData.reinforceCalc( out resultReinforceLevel, out resultPer, out _currentReinforcePrice, originalData, sourceList);

		targetDisplayData.price = _currentReinforcePrice;
		targetDisplayData.level = resultReinforceLevel;

//		nowDisplayData.price = _currentReinforcePrice;

		if(targetDisplayData.level >= GameIDData.MAX_LEVEL)
		{
			_enoughSelection = true;
			targetDisplayData.progressBarValue = 1.0f;
			targetDisplayData.progressLabelValue = 100;
		}
		else
		{
			_enoughSelection = false;
			targetDisplayData.progressLabelValue = Mathf.FloorToInt(resultPer * 100.0f);
			targetDisplayData.progressBarValue =  resultPer;
		}

		++_workIndex;

		// 강화 창 열었을때 기본 세팅. 
		if(isInit)
		{
			originalDisplayData.progressBarValue = originalData.getReinforceProgressPercent();
			originalDisplayData.progressLabelValue = Mathf.FloorToInt(originalDisplayData.progressBarValue * 100.0f);
			originalDisplayData.level = originalData.level;

			nowDisplayData = targetDisplayData;
			startDisplayData = targetDisplayData;
			startDisplayData.tweenExpValue = startDisplayData.progressBarValue+startDisplayData.level;

			setAllData(true);
		}
		else
		{
			startDisplayData = nowDisplayData;
			startDisplayData.tweenExpValue = startDisplayData.progressBarValue+startDisplayData.level;
			StartCoroutine(valueUpdater(_workIndex));

//			setAllData(false);
		}

		btnReinforce.isEnabled = (sourceList.Count > 0);


	}


	void setAllData(bool includeOriginal = false)
	{
		lbPrice.text = Util.GetCommaScore(nowDisplayData.price);

		if(includeOriginal)
		{
			originalProgressBar.lbProgress.text = originalDisplayData.progressLabelValue + "%";
			originalProgressBar.spProgressBar.fillAmount = originalDisplayData.progressBarValue;
			originalProgressBar.lbLevel.text = "l"+originalDisplayData.level;

			resultProgressBar.lbProgress.text = originalProgressBar.lbProgress.text;
			resultProgressBar.spProgressBar.fillAmount = originalProgressBar.spProgressBar.fillAmount;
			resultProgressBar.lbLevel.text = originalProgressBar.lbLevel.text;
		}
		else
		{
			if((nowDisplayData.level == 19 && nowDisplayData.progressBarValue >= 1.0f) || nowDisplayData.level == 20)
			{
				resultProgressBar.lbProgress.text = "MAX";
				resultProgressBar.spProgressBar.fillAmount = 1.0f;
				resultProgressBar.lbLevel.text = "l"+20;
			}
			else
			{
				resultProgressBar.lbProgress.text = nowDisplayData.progressLabelValue + "%";
				resultProgressBar.spProgressBar.fillAmount = nowDisplayData.progressBarValue;
				resultProgressBar.lbLevel.text = "l"+nowDisplayData.level;
			}
		}
	}


	int _workIndex = 0;
	WaitForSeconds ws005 = new WaitForSeconds(0.05f);


	IEnumerator valueUpdater(int workIndex)
	{
		float targetValue = 0;

		nowDisplayData.tweenExpValue = nowDisplayData.progressBarValue+nowDisplayData.level;
		targetDisplayData.tweenExpValue = targetDisplayData.progressBarValue+targetDisplayData.level;

		float duration = (MathUtil.abs(nowDisplayData.tweenExpValue, targetDisplayData.tweenExpValue));
		if(duration > 1.2f) duration = 1.2f;
		float time = 0.0f;
		bool isComplete = false;

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
				nowDisplayData = targetDisplayData;
				setAllData();
			}
			else
			{
				nowDisplayData.level = Mathf.RoundToInt( Mathf.Lerp( startDisplayData.level, targetDisplayData.level , step) );

				float calcValue = Mathf.Lerp( startDisplayData.tweenExpValue, targetDisplayData.tweenExpValue , step);

				nowDisplayData.level = (int)calcValue;// Mathf.RoundToInt( Mathf.Lerp( startDisplayData.level, targetDisplayData.level , step) );
				if(nowDisplayData.level > GameIDData.MAX_LEVEL) nowDisplayData.level = GameIDData.MAX_LEVEL;

				if(originalData.level != nowDisplayData.level && Mathf.Approximately(calcValue,targetDisplayData.tweenExpValue) && step >= 1.0f && calcValue % 1.0f <= 0.0f)
				{
					nowDisplayData.progressBarValue = 1.0f;
				}
				else
				{
					nowDisplayData.progressBarValue = calcValue % 1.0f;
				}


				nowDisplayData.progressLabelValue = Mathf.FloorToInt(nowDisplayData.progressBarValue * 100.0f);
				nowDisplayData.price = Mathf.RoundToInt( Mathf.Lerp( startDisplayData.price, targetDisplayData.price , step) );

				setAllData();
			}

			yield return ws005;
		}
	}


	ReinforceDisplayData originalDisplayData = new ReinforceDisplayData();
	ReinforceDisplayData nowDisplayData = new ReinforceDisplayData();

	ReinforceDisplayData startDisplayData = new ReinforceDisplayData();
	ReinforceDisplayData targetDisplayData = new ReinforceDisplayData();
}





public struct ReinforceDisplayData
{
	public int level;
	public float progressBarValue;
	public int progressLabelValue;
	public int price;

	public float tweenExpValue;

	public void copyFrom(ReinforceDisplayData pd)
	{
		level = pd.level;
		progressBarValue = pd.progressBarValue;
		progressLabelValue = pd.progressLabelValue;
		price = pd.price;
		tweenExpValue = pd.tweenExpValue;
	}

}

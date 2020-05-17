using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISummonInvenList : UIListBase {

	public List<GameIDData[]> rankers = new List<GameIDData[]>();
	protected List<GameIDData> _tempList = new List<GameIDData>();

	public float itemPerLine =  8.0f;

	protected override void refreshData ()
	{
		rankers.Clear();
		_tempList.Clear();

		_tempList.AddRange(GameDataManager.instance.unitInventoryList);

		sort();

		len = _tempList.Count;

		/*
		if(len > 20 && TutorialManager.instance.isTutorialMode && TutorialManager.nowPlayingTutorial("T44") && TutorialManager.instance.subStep >= 13)
		{
			//UN21900101

			for(i = 0; i < len; ++i)
			{
				if(_tempList[i].serverId.Contains("UN21900101"))
				{
					GameIDData gd = _tempList[i];
					_tempList.RemoveAt(i);
					_tempList.Insert(0,gd);
					break;
				}
			}
		}
		*/

		int totalLine = (Mathf.CeilToInt((float)len / (float)maxPerLine) + 1);
		if(totalLine < 3) totalLine = 3;

		bool lockSlot = true;

		string id;
//		bool u1 = false;
//		bool u2 = false;
//		bool u3 = false;
//		bool u4 = false;
//		bool u5 = false;

		for(i = 0; i < totalLine; ++i)
		{
			GameIDData[] gds = new GameIDData[maxPerLine];

			for(j = 0; j < maxPerLine; ++j)
			{
				if(i*maxPerLine+j < len)
				{
					gds[j] = _tempList[i*maxPerLine+j];
				}
				else
				{
					gds[j] = null;
				}
			}

			rankers.Add(gds);
		}

		_tempList.Clear();

		rankerLen = rankers.Count;
	}


	int getSettingUnitNum(string id)
	{
		count = 0;
		foreach(PlayerUnitSlot pd in GameDataManager.instance.unitSlots)
		{
			if(pd.isOpen == true && pd.unitData.id.Equals(id)) ++count;
		}
		
		return count;
	}	


	protected override void sort()
	{
		switch(sortType)
		{
		case SORT_TYPE_RARE:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByRareFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByRareFromLow);
			break;

		case SORT_TYPE_LEVEL:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByLevelFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByLevelFromLow);
			break;

		case SORT_TYPE_NAME:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByNameFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByNameFromLow);
			break;

		case SORT_TYPE_USE_SP:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByUseSpFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByUseSpFromLow);
			break;

		case SORT_TYPE_ATK:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByAtkFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByAtkFromLow);
			break;

		case SORT_TYPE_DEF:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByDefFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByDefFromLow);
			break;

		case SORT_TYPE_HP:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortUnitDataByHpFromHigh);
			else _tempList.Sort(GameIDData.sortUnitDataByHpFromLow);
			break;
		}

		if(TutorialManager.nowPlayingTutorial("T44") && TutorialManager.instance.subStep < 20)
		{
			if(_tempList.Count > 24)
			{
				List<GameIDData> tl = new List<GameIDData>();

				for(int i = _tempList.Count -1; i >= 0; --i)
				{
					if(_tempList[i].serverId.Contains("UN21900101") || _tempList[i].serverId.Contains("UN10100101"))
					{
						tl.Add(_tempList[i]);
						_tempList.RemoveAt(i);
						break;
					}
				}

				tl.AddRange(_tempList);
				_tempList.Clear();
				_tempList.AddRange(tl);

			}
		}
	}



	protected override void setPassData ()
	{
		foreach(GameIDData[] ids in rankers)
		{
			rankers_obj.Add ((object)ids);	
		}
	}



	public void onTabChange()
	{
		if(listGrid.gameObject.activeSelf && listGrid.itemList.Count > 0)
		{
			listGrid.gameObject.BroadcastMessage("refreshCanUse");
		}
	}

}




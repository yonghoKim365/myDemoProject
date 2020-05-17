using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UISkillInvenList : UIListBase {

	public List<GameIDData[]> rankers = new List<GameIDData[]>();
	protected List<GameIDData> _tempList = new List<GameIDData>();

	public float itemPerLine = 8.0f;

	protected override void refreshData ()
	{
		rankers.Clear();
		_tempList.Clear();

		_tempList.AddRange(GameDataManager.instance.skillInventoryList);

		sort();

		len = _tempList.Count;
		
		int totalLine = (Mathf.CeilToInt((float)len / (float)maxPerLine) + 1);
		if(totalLine < 3) totalLine = 3;

		string id;
//		bool s1 = false;
//		bool s2 = false;
//		bool s3 = false;

		for(i = 0; i < totalLine; ++i)
		{
			GameIDData[] gds = new GameIDData[maxPerLine];
			
			for(j = 0; j < maxPerLine; ++j)
			{
				if(i*maxPerLine+j < len)
				{
					gds[j] =  _tempList[i*maxPerLine+j];
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



	protected override void setPassData ()
	{
		foreach(GameIDData[] ids in rankers)
		{
			rankers_obj.Add ((object)ids);	
		}
	}


	protected override void sort()
	{
		switch(sortType)
		{
		case SORT_TYPE_RARE:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortSkillDataByRareFromHigh);
			else _tempList.Sort(GameIDData.sortSkillDataByRareFromLow);
			break;
			
		case SORT_TYPE_LEVEL:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortSkillDataByLevelFromHigh);
			else _tempList.Sort(GameIDData.sortSkillDataByLevelFromLow);
			break;
			
		case SORT_TYPE_NAME:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortSkillDataByNameFromHigh);
			else _tempList.Sort(GameIDData.sortSkillDataByNameFromLow);
			break;
			
		case SORT_TYPE_USE_MP:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortSkillDataByUseMpFromHigh);
			else _tempList.Sort(GameIDData.sortSkillDataByUseMpFromLow);
			break;
			
		case SORT_TYPE_SKILLTYPE:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortSkillDataBySkillTypeFromHigh);
			else _tempList.Sort(GameIDData.sortSkillDataBySkillTypeFromLow);
			break;
		}
		

		if(TutorialManager.nowPlayingTutorial("T45") && TutorialManager.instance.subStep < 15)
		{
			if(_tempList.Count > 24)
			{
				List<GameIDData> tl = new List<GameIDData>();
				
				for(int i = _tempList.Count -1; i >= 0; --i)
				{
					if(_tempList[i].serverId.Contains("SK_1105_1") || _tempList[i].serverId.Contains("SK_2110_1"))
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


	public void onTabChange()
	{
		if(listGrid.gameObject.activeSelf && listGrid.itemList.Count > 0)
		{
			listGrid.gameObject.BroadcastMessage("refreshCanUse");
		}
	}



}

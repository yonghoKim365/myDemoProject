using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIHeroInvenList : UIListBase {

	public List<GameIDData[]> rankers = new List<GameIDData[]>();
	protected List<GameIDData> _tempList = new List<GameIDData>();

	protected override void refreshData()
	{
		rankers.Clear();
		_tempList.Clear();
		int i,j,len;



#if UNITY_EDITOR

	 	if(DebugManager.instance.useDebug)
		{
			if(GameDataManager.instance.partsInventoryList.Count < 10)
			{
				GameDataManager.instance.partsInventoryList.Clear();

				foreach(KeyValuePair<string, HeroPartsData> kv in GameManager.info.heroPartsDic)
				{
					if(kv.Key.StartsWith("CH"))
					{
						if(kv.Value.type == HeroParts.WEAPON)
						{
							GameIDData gd = new GameIDData();
							gd.parse(kv.Key, GameIDData.Type.Equip);
							GameDataManager.instance.partsInventoryList.Add(gd);
						}
					}
				}
			}
		}

#endif



		_tempList.AddRange(GameDataManager.instance.partsInventoryList);

		sort();

		len = _tempList.Count;

		int totalLine = (Mathf.CeilToInt((float)len / 5.0f) + 1);
		if(totalLine < 3) totalLine = 3;
//		int totalLine = Mathf.CeilToInt( (float)capacity /5.0f );

		
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
		case SORT_TYPE_HERO:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortEquipDataByHeroFromHigh);
			else _tempList.Sort(GameIDData.sortEquipDataByHeroFromLow);
			break;
			
		case SORT_TYPE_RARE:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortEquipDataByRareFromHigh);
			else _tempList.Sort(GameIDData.sortEquipDataByRareFromLow);
			break;
			
		case SORT_TYPE_LEVEL:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortEquipDataByLevelFromHigh);
			else _tempList.Sort(GameIDData.sortEquipDataByLevelFromLow);
			break;
			
		case SORT_TYPE_PARTS:
			if(sortFromHigh) _tempList.Sort(GameIDData.sortEquipDataByTypeFromHigh);
			else _tempList.Sort(GameIDData.sortEquipDataByTypeFromLow);
			break;
		}








		if(TutorialManager.nowPlayingTutorial("T46") && TutorialManager.instance.subStep < 12)
		{
			if(_tempList.Count > 15)
			{
				List<GameIDData> tl = new List<GameIDData>();
				
				for(int i = _tempList.Count -1; i >= 0; --i)
				{
					if(_tempList[i].serverId.Contains("LEO_RD2_11_1") || _tempList[i].serverId.Contains("LEO_RD1_11"))
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





	public int debugIndex = 0;
	public int defaultDebugNum = 10;

	public void addTest()
	{
		rankers.Clear();
		_tempList.Clear();
		int i,j,len;

		for(i = 0; i < debugIndex + defaultDebugNum; ++i)
		{
			_tempList.Add(GameDataManager.instance.partsInventoryList[0]);
		}

		len = _tempList.Count;
		
		int totalLine = (Mathf.CeilToInt((float)len / 5.0f) + 1);
		if(totalLine < 3) totalLine = 3;
		//		int totalLine = Mathf.CeilToInt( (float)capacity /5.0f );
		bool lockSlot = true;
		
		for(i = 0; i < totalLine; ++i)
		{
			GameIDData[] str = new GameIDData[maxPerLine];
			
			for(j = 0; j < maxPerLine; ++j)
			{
				if(i*maxPerLine+j < len)
				{
					str[j] = _tempList[i*maxPerLine+j];
				}
				else// if(i*maxPerLine+j >= len)
				{
					str[j] = null;
				}
			}
			
			rankers.Add(str);
		}
		
		_tempList.Clear();
		
		rankerLen = rankers.Count;
		
		rankers_obj.Clear();
		
		setPassData();
		
		
		listGrid.setData(panel, rankers_obj, false, 0);

	}
	


}

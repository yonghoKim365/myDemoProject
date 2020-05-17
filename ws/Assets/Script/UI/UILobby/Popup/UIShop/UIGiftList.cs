using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIGiftList : UIListBase {

	public List<string[]> rankers = new List<string[]>();
	protected List<string> _tempList = new List<string>();

	protected override void refreshData ()
	{
		rankers.Clear();
		_tempList.Clear();

//		foreach(KeyValuePair<string, int> kv in GameDataManager.instance.skillInventory)
//		{
//			len = kv.Value - getSettingSkillNum(kv.Key);
//			for(i = 0; i < len; ++i)
//			{
//				_tempList.Add(kv.Key);
//			}
//		}

		sort();

		len = _tempList.Count;

		for(int i = 0; i < len; i+=maxPerLine)
		{
			string[] str = new string[maxPerLine];

			for(j = 0; j < maxPerLine; ++j)
			{
				if(i+j < len) str[j] = _tempList[i+j];
				else str[j] = null;
			}

			rankers.Add(str);
		}

		_tempList.Clear();

		rankerLen = rankers.Count;
	}

	protected override void sort ()
	{

	}

	
	protected override void setPassData ()
	{
		foreach(string[] ids in rankers)
		{
			rankers_obj.Add ((object)ids);	
		}
	}
	
}

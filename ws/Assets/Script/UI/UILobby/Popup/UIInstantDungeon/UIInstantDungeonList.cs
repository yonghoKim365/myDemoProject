using UnityEngine;
using System.Collections.Generic;

public class UIInstantDungeonList : UIListBase {
	
	public List<P_Sigong> rankers = new List<P_Sigong>();

	protected override void refreshData ()
	{
		rankers_obj.Clear();
		rankers.Clear();

		if(GameDataManager.instance.sigongList != null)
		{
			foreach(KeyValuePair<string, P_Sigong> kv in GameDataManager.instance.sigongList)
			{
				if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Easy)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EASY) continue;
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Normal)
				{
					if(kv.Value.category != UIPopupInstantDungeon.NORMAL) continue;
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Hard)
				{
					if(kv.Value.category != UIPopupInstantDungeon.HARD) continue;
				}
				else if(UIPopupInstantDungeon.type == UIPopupInstantDungeon.Type.Event)
				{
					if(kv.Value.category != UIPopupInstantDungeon.EVENT) continue;
				}

				rankers.Add(kv.Value);
			}
		}
	}
	

	protected override void setPassData ()
	{
		int len = rankers.Count;

		for(int i = 0; i < len; ++i)
		{
			rankers_obj.Add ((object)rankers[i]);
		}

		rankers.Clear();
	}

	protected override void sort()
	{
	}
}

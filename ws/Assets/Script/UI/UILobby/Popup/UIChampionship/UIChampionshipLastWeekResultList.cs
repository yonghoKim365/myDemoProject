using UnityEngine;
using System.Collections.Generic;

public class UIChampionshipLastWeekResultList : UIListBase {
	
	public List<P_Champion> rankers = new List<P_Champion>();
//	protected List<HeroSkillData> _tempList = new List<HeroSkillData>();



	protected override void refreshData ()
	{
		rankers.Clear();

		foreach(KeyValuePair<string, P_Champion> kv in GameDataManager.instance.lastChampionshipData.lastChampions)
		{
			rankers.Add(kv.Value);
		}
	}

	protected override void setPassData ()
	{
		int len = rankers.Count;
		int lastScore = 0;
		int lastNum = 0;

		for(int i = 0; i < len; ++i)
		{
			rankers_obj.Add ((object)rankers[i]);
		}
	}

	 
	protected override void sort()
	{
	}
}



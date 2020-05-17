using UnityEngine;
using System.Collections.Generic;

public class UIPopupDefenceResultList : UIListBase {
	
	public List<P_DefenceRecord> rankers = new List<P_DefenceRecord>();

	protected override void refreshData ()
	{
		rankers.Clear();

		if(GameDataManager.instance.championshipData != null && GameDataManager.instance.championshipData.defenceRecords != null)
		{
			foreach(P_DefenceRecord pd in GameDataManager.instance.championshipData.defenceRecords)
			{
				rankers.Add(pd);
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
	}

//
//	 P_ChampionSorter _sort = new P_ChampionSorter();
//	
	protected override void sort()
	{
//		rankers.Sort(_sort);
	}
}

//
//public class P_ChampionSorter  : IComparer<P_Champion>
//{
//	public int Compare(P_Champion x, P_Champion y)
//	{
//		if(x.rank > y.rank) return 1;
//		else if(x.rank < y.rank) return -1;
//		return 0;
//	}	
//}
//
//
//
//
//public struct ChampionListData
//{
//	public P_Champion championData;
//	public int number;
//
//	public ChampionListData(P_Champion data)
//	{
//		championData = data;
//		number = 1;
//	}
//}


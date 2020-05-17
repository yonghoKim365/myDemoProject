using UnityEngine;
using System.Collections.Generic;

public class UIFriendList : UIListBase {
	
	public List<P_FriendData> rankers = new List<P_FriendData>();

	protected override void refreshData ()
	{
		rankers.Clear();

		if(GameDataManager.instance.friendDatas == null) return;

		foreach(KeyValuePair<string, P_FriendData> kv in GameDataManager.instance.friendDatas)
		{
			rankers.Add(kv.Value);
		}
	}

	
	protected override void setPassData ()
	{
		int len = rankers.Count;
		
		sort();
		
		for(int i = 0; i < len; ++i)
		{
			rankers_obj.Add ((object)rankers[i]);
		}
	}
	
	
	P_FriendDataSorter _sort = new P_FriendDataSorter();
	
	protected override void sort()
	{
		rankers.Sort(_sort);
	}
}


public class P_FriendDataSorter  : IComparer<P_FriendData>
{
	public int Compare(P_FriendData x, P_FriendData y)
	{
//		1> 챔피언십 등급
//		2>  우정레벨
//		3> 유저레벨
//		4> 닉네임(가나다)

		int i = y.league.CompareTo(x.league);
		if (i == 0) i = y.fLevel.CompareTo(x.fLevel);
//		if (i == 0) i = y.level.CompareTo(x.level);
		if (i == 0) i = x.nickname.CompareTo(y.nickname);

		return i;
	}	
}


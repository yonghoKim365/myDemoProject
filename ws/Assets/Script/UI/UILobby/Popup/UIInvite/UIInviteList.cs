using UnityEngine;
using System.Collections.Generic;

public class UIInviteList : UIListBase {
	
	public List<string> rankers = new List<string>();
	
	protected override void refreshData ()
	{
		rankers.Clear();

		foreach(KeyValuePair<string, epi.GameUser> kv in epi.GAME_DATA.friendDic)
		{
#if !UNITY_EDITOR
			if(epi.GAME_DATA.appFriendDic.ContainsKey(kv.Key) == false)
				#endif
			{
				if(kv.Value.isKakaoBlock == false)
				{
					rankers.Add(kv.Key);
				}
			}
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
	
	
	//P_FriendDataSorter _sort = new P_FriendDataSorter();
	
	protected override void sort()
	{
		//rankers.Sort(_sort);
	}
}
	

//public class P_FriendDataSorter  : IComparer<P_FriendData>
//{
//	public int Compare(P_FriendData x, P_FriendData y)
//	{
//		//		1> 챔피언십 등급
//		//		2>  우정레벨
//		//		3> 유저레벨
//		//		4> 닉네임(가나다)
//		
//		int i = y.league.CompareTo(x.league);
//		if (i == 0) i = y.friendLevel.CompareTo(x.friendLevel);
//		if (i == 0) i = y.level.CompareTo(x.level);
//		if (i == 0) i = x.nickname.CompareTo(y.nickname);
//		
//		return i;
//	}	
//}


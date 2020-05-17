using UnityEngine;
using System.Collections.Generic;

public class UIHellRankingList : UIListBase {
	
	public List<P_FriendRank> friendRankers = new List<P_FriendRank>();
	public List<P_UserRank> allRankers = new List<P_UserRank>();

	public bool isFriendType = false;

	protected override void refreshData ()
	{
		friendRankers.Clear();
		allRankers.Clear();

		if(isFriendType)
		{
			if(GameDataManager.instance.hellFriendRanks == null) return;

			foreach(KeyValuePair<string, P_FriendRank> kv in GameDataManager.instance.hellFriendRanks)
			{
				friendRankers.Add(kv.Value);
			}
		}
		else
		{
			if(GameDataManager.instance.hellUserRanks == null) return;

			foreach(KeyValuePair<string, P_UserRank> kv in GameDataManager.instance.hellUserRanks)
			{
				allRankers.Add(kv.Value);
			}
		}
	}
	

	protected override void setPassData ()
	{
		int len = 0;


		if(isFriendType)
		{
			len = friendRankers.Count;

			for(int i = 0; i < len; ++i)
			{
				rankers_obj.Add ((object)friendRankers[i]);
			}
		}
		else
		{
			len = allRankers.Count;

			for(int i = 0; i < len; ++i)
			{
				rankers_obj.Add ((object)allRankers[i]);
			}
		}

		friendRankers.Clear();
		allRankers.Clear();
	}

	protected override void sort()
	{
	}
}

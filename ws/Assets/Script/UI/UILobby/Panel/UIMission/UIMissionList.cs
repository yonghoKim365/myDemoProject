using UnityEngine;
using System.Collections.Generic;

public class UIMissionList : UIListBase {
	
	public List<string> rankers = new List<string>();

	public List<P_Mission> sortList = new List<P_Mission>();

	public List<P_Mission> unclearList = new List<P_Mission>();


	protected override void refreshData ()
	{
		rankers.Clear();

		if(GameDataManager.instance.missions != null)
		{
			sortList.Clear();
			unclearList.Clear();

			foreach(KeyValuePair<string, P_Mission > kv in GameDataManager.instance.missions)
			{
				if(kv.Value.state == WSDefine.MISSION_CLOSE) continue;

				if(UIMission.type == UIMission.Type.Main)
				{
					if(kv.Value.kind == WSDefine.MISSION_KIND_MAIN)
					{
						if(kv.Value.state == WSDefine.MISSION_CLEAR)
						{
							sortList.Add(kv.Value);
						}
						else
						{
							unclearList.Add(kv.Value);
						}
					}
				}
				else if(UIMission.type == UIMission.Type.Sub)
				{
					if(kv.Value.kind == WSDefine.MISSION_KIND_SUB)
					{
						if(kv.Value.state == WSDefine.MISSION_CLEAR)
						{
							sortList.Add(kv.Value);
						}
						else
						{
							unclearList.Add(kv.Value);
						}
					}
				}
				else if(UIMission.type == UIMission.Type.Event)
				{
					if(kv.Value.kind == WSDefine.MISSION_KIND_EVENT)
					{
						if(kv.Value.state == WSDefine.MISSION_CLEAR)
						{
							sortList.Add(kv.Value);
						}
						else
						{
							unclearList.Add(kv.Value);
						}
					}
				}
				else if(UIMission.type == UIMission.Type.Play)
				{
					if(kv.Value.kind == WSDefine.MISSION_KIND_PLAY)
					{
						if(kv.Value.state == WSDefine.MISSION_CLEAR)
						{
							sortList.Add(kv.Value);
						}
						else
						{
							unclearList.Add(kv.Value);
						}
					}
				}
			}

		
			sortList.AddRange(unclearList);


			int len = sortList.Count;

			if(TutorialManager.nowPlayingTutorial("T13") || TutorialManager.instance.clearCheck("T13") == false)
			{
				for(int i = 0; i < len ; ++i)
				{
					if(sortList[i].id == "M_EP_001_01")
					{
						rankers.Add(sortList[i].id);
						sortList.RemoveAt(i);
						break;
					}
				}
			}

			len = sortList.Count;

			for(int i = 0; i < len ; ++i)
			{
				rankers.Add(sortList[i].id);
			}
		}

		sortList.Clear();
		unclearList.Clear();
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


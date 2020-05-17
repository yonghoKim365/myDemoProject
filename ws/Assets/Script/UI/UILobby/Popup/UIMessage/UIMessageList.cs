using UnityEngine;
using System.Collections.Generic;

public class UIMessageList : UIListBase {
	
	public List<string> rankers = new List<string>();
	public List<P_Message> sortList = new List<P_Message>();

	public int nowMsg = 0;


	MessageSorter _sorter = new MessageSorter();

	protected override void refreshData ()
	{
		rankers.Clear();

		int newMsg = 0;
		nowMsg = 0;


		if(GameDataManager.instance.messages != null)
		{

			foreach(KeyValuePair<string, P_Message> kv in GameDataManager.instance.messages)
			{
				if(kv.Value.isNew == WSDefine.YES) ++newMsg;
				rankers.Add(kv.Key);
				++nowMsg;
			}

//			foreach(KeyValuePair<string, P_Message> kv in GameDataManager.instance.messages)
//			{
//				if(kv.Value.isNew == WSDefine.YES) ++newMsg;
//				sortList.Add(kv.Value);
//				++nowMsg;
//			}
//
//			int len = sortList.Count;
//			sortList.Sort(_sorter);
//
//			for(int i = 0; i < len; ++i)
//			{
//				rankers.Add(sortList[i].id);
//			}
		}

		if(newMsg > 0)
		{
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.gameObject.SetActive(true);
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.text = newMsg.ToString();
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.gameObject.SetActive(false);
			GameManager.me.uiManager.uiMenu.uiLobby.lbNewMsgNum.text = newMsg.ToString();
		}

		GameManager.me.uiManager.uiMenu.uiLobby.updateNewMsgAni();
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


public class MessageSorter  : IComparer<P_Message>
{
	public int Compare(P_Message x, P_Message y)
	{
		return (x.expiredTime < 0)?1:1;
	}	
}


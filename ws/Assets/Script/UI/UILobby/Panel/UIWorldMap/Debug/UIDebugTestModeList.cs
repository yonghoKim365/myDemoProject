using UnityEngine;
using System.Collections.Generic;

public class UIDebugTestModeList : UIListBase 
{

	public UIButton btnClose;

	void Awake()
	{
		UIEventListener.Get(btnClose.gameObject).onClick = onClickClose;
	}

	void onClickClose(GameObject go)
	{
		gameObject.SetActive(false);
	}


	public List<string> rankers = null;

	protected override void refreshData ()
	{
		if(rankers == null) rankers = new List<string>();

		if(rankers.Count <= 0)
		{
			foreach(KeyValuePair<string, TestModeData  > kv in GameManager.info.testModeData)
			{
				rankers.Add(kv.Value.id);
			}
		}
	}
	
	
	protected override void setPassData ()
	{
		rankers_obj.Clear();

		int len = rankers.Count;
		
		for(int i = 0; i < len; ++i)
		{
			rankers_obj.Add ((object)rankers[i]);
		}
	}
	
	protected override void sort()
	{
	}
}


using UnityEngine;
using System.Collections.Generic;

public class UIDebugRoundList : UIListBase {
	
	public List<string> rankers = null;

	public UIDebugRoundPopup askPopup;

	protected override void refreshData ()
	{
		if(rankers == null) rankers = new List<string>();

		if(rankers.Count <= 0)
		{
			foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
			{
				rankers.Add(kv.Value.id);
			}


			foreach(KeyValuePair<string, P_Sigong > kv in GameManager.info.testSigong)
			{
				rankers.Add("SIGONG/"+kv.Value.id);
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


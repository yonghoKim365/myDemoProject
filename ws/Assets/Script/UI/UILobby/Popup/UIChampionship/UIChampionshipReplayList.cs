using UnityEngine;
using System.Collections.Generic;

public class UIChampionshipReplayList : UIListBase {

	public List<P_ChampionResult> rankers = new List<P_ChampionResult>();

	private Dictionary<string, P_ChampionResult> championshipRoundResults;

	public int revengeCoolTime;
	
	protected override void refreshData ()
	{
		rankers.Clear();

		foreach(KeyValuePair<string, P_ChampionResult> kv in championshipRoundResults)
		{
			//Debug.Log(" kv.key:"+kv.Key);
			rankers.Add(kv.Value);
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
	
	protected override void sort()
	{

	}

	public void SetData(Dictionary<string, P_ChampionResult> _rounds, int _revengeCoolTime){
		championshipRoundResults = _rounds;
		revengeCoolTime  = _revengeCoolTime;
	}
}

using System;
using System.Collections.Generic;

sealed public class StageData : BaseData
{
	public string id;	
	public string type;
	public string title;
	public string[] rounds;
	
	public StageData ()
	{
	}

	
	sealed public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		type = (string)l[k["TYPE"]];		
		title = (string)l[k["TITLE"]];
		rounds = ((string)l[k["ROUNDS"]]).Split(',');
	}
}


using System;
using System.Collections.Generic;

sealed public class TutorialInfoData : BaseData
{
	public string id;

	public string title;

	public bool canSkip = false;
	
	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = "T"+l[k["ID"]].ToString();
		title = (string)l[k["TITLE"]];


		if( l[k["CANSKIP"]] is String && ((string)l[k["CANSKIP"]]).ToLower() == "o" )
		{
			canSkip = true;
		}
		else
		{
			canSkip = false;
		}
	}
	
}


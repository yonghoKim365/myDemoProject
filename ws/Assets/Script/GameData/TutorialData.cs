using System;
using System.Collections.Generic;

sealed public class TutorialData : BaseData
{
	public string id;
	public int step;

	public string text;

	public TutorialData ()
	{
		
	}
	
	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		id = "T"+l[k["ID"]].ToString();
		Util.parseObject(l[k["STEP"]], out step, true, 0);
		text = ((string)l[k["TEXT"]]).Replace("\\n","\n");
	}
	
}


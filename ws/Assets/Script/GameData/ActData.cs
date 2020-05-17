using System;
using System.Collections.Generic;

sealed public class ActData : BaseData
{
	public int id;
	public string[] stages;
	public string description;
	
	public ActData ()
	{
		
	}
	
	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		Util.parseObject(l[k["ID"]], out id, true, 0);
		description = ((string)l[k["DESCRIPTION"]]).Replace("\\n","\n");
		stages = ((string)l[k["EPICSTAGES"]]).Split(',');
	}
	
}


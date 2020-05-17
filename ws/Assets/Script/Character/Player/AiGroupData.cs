using System;
using UnityEngine;
using System.Collections.Generic;

sealed public class AiGroupData : BaseData
{

	public string data;
	public string id;

	public override void setData (List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		data = (string)l[k["DATA"]];
		data = data.Replace("\"","");

	}

}
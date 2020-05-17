using System;
using System.Collections.Generic;

public struct SkillIconData
{
	public string id;
	public string icon;
	public string soundId;

	public void setData(List<object> l, Dictionary<string, int> k)
	{
		id = (string)l[k["ID"]];
		icon = (string)l[k["ICON"]];
		soundId = (string)l[k["SOUND"]];
	}

}


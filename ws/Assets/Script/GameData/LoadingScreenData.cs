using System;
using System.Collections.Generic;

sealed public class LoadingScreenData
{
	public string id;

	public string unitId;

	public string name;
	public string type;
	public string attackType;
	public string skill;
	public string code;
	public int weight;

	
	public void setData(List<object> l, Dictionary<string, int> k)
	{
				
		id = (l[k["FILENAME"]]).ToString();

		unitId = (l[k["UID"]]).ToString();

		name = (l[k["NAME"]]).ToString();
		type = (l[k["TYPE"]]).ToString();
		attackType = (l[k["ATTACK_TYPE"]]).ToString();
		skill = (l[k["SKILL"]]).ToString();
		code = (l[k["CODE"]]).ToString();

		Util.parseObject(l[k["WEIGHT"]], out weight, true, 1);
	}
	
}

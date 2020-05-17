using System;
using System.Collections.Generic;

sealed public class HellSetupData : BaseData
{
	public int roundIndex;

	public Xint killPoint = 0;
	public Xint clearBonus = 0;
	public Xint timeBonus = 0;

	public override void setData(List<object> l, Dictionary<string, int> k)
	{
		Util.parseObject(l[k["ID"]], out roundIndex, true);

		Util.parseObject(l[k["KILLPOINT"]], out killPoint, true);
		Util.parseObject(l[k["CLEAR_BONUS"]], out clearBonus, true);
		Util.parseObject(l[k["TIME_BONUS"]], out timeBonus, true);
	}
}
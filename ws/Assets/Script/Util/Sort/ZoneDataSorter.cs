using System;
using System.Collections.Generic;

public class ZoneDataSorter : IComparer<ZoneData>
{
	public int Compare(ZoneData x, ZoneData y)
	{
		if(x.sort > y.sort)
		{
			return 1;
		}
		else if(x.sort < y.sort)
		{
			return -1;
		}
		
		return 0;
	}
}

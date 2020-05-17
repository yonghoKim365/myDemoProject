using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIShopSpecialItemList : UIListBase 
{
	protected override void refreshData ()
	{
		rankers_obj.Clear();
	}

	protected override void setPassData ()
	{
		if(GameDataManager.instance.annuityProducts != null)
		{
			foreach(KeyValuePair<string, P_Annuity> kv in GameDataManager.instance.annuityProducts)
			{
				rankers_obj.Add((object)kv.Value);
			}
		}
	}


	protected override void sort()
	{
	}

}

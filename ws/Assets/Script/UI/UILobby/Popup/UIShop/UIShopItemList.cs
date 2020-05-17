using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIShopItemList : UIListBase 
{
	public static ShopItem.Type type;

	public List<P_Product> sortList = new List<P_Product>();

	private P_PriceSorter _sorter = new P_PriceSorter();


	protected override void refreshData ()
	{
		sortList.Clear();

		if(type == ShopItem.Type.gold)
		{
			if(GameDataManager.instance.shopData.goldProducts != null)
			{
				foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.goldProducts)
				{
					sortList.Add(kv.Value);
				}
			}
		}
		else if(type == ShopItem.Type.ruby)
		{
			if(GameDataManager.instance.shopData.rubyProducts != null)
			{
				foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.rubyProducts)
				{
					sortList.Add(kv.Value);
				}
			}
		}
		else if(type == ShopItem.Type.item)
		{
			if(GameDataManager.instance.shopData.itemProducts != null)
			{
				foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.itemProducts)
				{
					sortList.Add(kv.Value);
				}
			}
		}
		else if(type == ShopItem.Type.energy)
		{
			if(GameDataManager.instance.shopData.energyProducts != null)
			{
				foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.energyProducts)
				{
					sortList.Add(kv.Value);
				}
			}
		}
		else if(type == ShopItem.Type.gift)
		{
			if(GameDataManager.instance.shopData.giftProducts != null)
			{
				foreach(KeyValuePair<string, P_Product> kv in GameDataManager.instance.shopData.giftProducts)
				{
					sortList.Add(kv.Value);
				}
			}
		}


	}

	protected override void setPassData ()
	{
		int len = sortList.Count;

		if(isPageType)
		{
			int totalPage =  Mathf.CeilToInt((float)len / (float)maxPerLine) ;
			for(int i = 0; i < totalPage; ++i)
			{
				P_Product[] p = new P_Product[maxPerLine];

				for(int j = 0; j < maxPerLine; ++j)
				{
					if(i*maxPerLine+j < len)
					{
						p[j] = sortList[i*maxPerLine+j];
					}
					else
					{
						p[j] = null;
					}
				}
				
				rankers_obj.Add((object)p);
			}
		}
		else
		{
			for(int i = 0; i < len ; ++i)
			{
				rankers_obj.Add((object)sortList[i]);
			}
		}


		sortList.Clear();
	}


	protected override void sort()
	{
	}

}



public class P_PriceSorter  : IComparer<P_Product>
{
	public int Compare(P_Product x, P_Product y)
	{
		return x.price.CompareTo(y.price);
	}	
}

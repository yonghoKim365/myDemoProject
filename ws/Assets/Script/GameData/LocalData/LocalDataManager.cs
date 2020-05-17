using UnityEngine;
using System.Collections;

public class LocalDataManager 
{
	public const string UNIT_SORT_DIRECTION = "USD";
	public const string UNIT_SORT_TYPE = "UST";

	public const string SKILL_SORT_DIRECTION = "SSD";
	public const string SKILL_SORT_TYPE = "SST";

	public const string PARTS_SORT_DIRECTION = "PSD";
	public const string PARTS_SORT_TYPE = "PST";

	public const int SORT_HIGH = 1;
	public const int SORT_LOW = 0;

	public static void setInvenSortType(UIInvenSort.InvenType type, string sortType)
	{
		switch(type)
		{
		case UIInvenSort.InvenType.unit:
			PlayerPrefs.SetString(UNIT_SORT_TYPE, sortType);
			break;
		case UIInvenSort.InvenType.skill:
			PlayerPrefs.SetString(SKILL_SORT_TYPE, sortType);
			break;
		case UIInvenSort.InvenType.equip:
			PlayerPrefs.SetString(PARTS_SORT_TYPE, sortType);
			break;
		}
	}

	public static void setInvenSortDirection(UIInvenSort.InvenType type, bool fromHigh)
	{
		switch(type)
		{
		case UIInvenSort.InvenType.unit:
			PlayerPrefs.SetInt(UNIT_SORT_DIRECTION, fromHigh?SORT_HIGH:SORT_LOW);
			break;
		case UIInvenSort.InvenType.skill:
			PlayerPrefs.SetInt(SKILL_SORT_DIRECTION, fromHigh?SORT_HIGH:SORT_LOW);
			break;
		case UIInvenSort.InvenType.equip:
			PlayerPrefs.SetInt(PARTS_SORT_DIRECTION, fromHigh?SORT_HIGH:SORT_LOW);
			break;
		}
	}


	public static string getInvenSortType(UIInvenSort.InvenType type)
	{
		switch(type)
		{
		case UIInvenSort.InvenType.unit:
			return PlayerPrefs.GetString(UNIT_SORT_TYPE, "RARE");
			break;
		case UIInvenSort.InvenType.skill:
			return PlayerPrefs.GetString(SKILL_SORT_TYPE, "RARE");
			break;
		case UIInvenSort.InvenType.equip:
			return PlayerPrefs.GetString(PARTS_SORT_TYPE, "RARE");
			break;
		}

		return "RARE";
	}

	public static bool isInvenSortDirectionHigh(UIInvenSort.InvenType type)
	{
		switch(type)
		{
		case UIInvenSort.InvenType.unit:
			return (PlayerPrefs.GetInt(UNIT_SORT_DIRECTION, SORT_HIGH) == SORT_HIGH)?true:false;
			break;
		case UIInvenSort.InvenType.skill:
			return (PlayerPrefs.GetInt(SKILL_SORT_DIRECTION, SORT_HIGH) == SORT_HIGH)?true:false;
			break;
		case UIInvenSort.InvenType.equip:
			return (PlayerPrefs.GetInt(PARTS_SORT_DIRECTION, SORT_HIGH) == SORT_HIGH)?true:false;
			break;
		}

		return true;
	}

}

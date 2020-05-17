using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class UIListBase : MonoBehaviour {

	public const string SORT_TYPE_NONE = "NONE";
	public const string SORT_TYPE_RARE = "RARE";
	public const string SORT_TYPE_LEVEL = "LEVEL";
	public const string SORT_TYPE_NAME = "NAME";
	public const string SORT_TYPE_USE_SP = "SP";
	public const string SORT_TYPE_USE_MP = "MP";
	public const string SORT_TYPE_ATK = "ATK";
	public const string SORT_TYPE_DEF = "DEF";
	public const string SORT_TYPE_HP = "HP";
	public const string SORT_TYPE_SKILLTYPE = "SKILLTYPE";
	public const string SORT_TYPE_HERO = "HERO";
	public const string SORT_TYPE_PARTS = "PARTS";


	public bool isPageType = false;

	public string sortType = SORT_TYPE_NONE;
	public bool sortFromHigh = true;

	public ListGrid listGrid;
	public UIScrollView panel;
	public UIPanel uiPanel;

	protected int i,j,len,count;

	public int maxPerLine = 5;

	protected abstract void refreshData();

	protected abstract void setPassData();

	protected abstract void sort();

	protected List<object> rankers_obj = new List<object>();

	protected int rankerLen = 0;

	public virtual void draw(bool isResetPos_p = true, int startIndex = -1){
		
		refreshData();

		rankers_obj.Clear();

		setPassData();


		listGrid.setData(panel, rankers_obj,isResetPos_p,startIndex);
	}


	public virtual void clear(bool isResetPos_p = true)
	{
		rankers_obj.Clear();

		listGrid.setData(panel, rankers_obj,isResetPos_p);
	}
	
}

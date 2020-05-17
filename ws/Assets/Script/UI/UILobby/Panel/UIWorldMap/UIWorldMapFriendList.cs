using UnityEngine;
using System.Collections.Generic;

public class UIWorldMapFriendList : UIListBase {
	
	public List<P_FriendData> rankers = new List<P_FriendData>();

	public UISprite spArrowUp, spArrowDown;

	public static bool isDownArrow = true;

	protected override void refreshData ()
	{
	}

	public override void draw(bool isResetPos_p = true, int startIndex = -1)
	{
		rankers_obj.Clear();
		setPassData();
		
		listGrid.setData(panel, rankers_obj,isResetPos_p,startIndex);
	}


	Vector3 _v;
	public void draw(List<P_FriendData> fdList, Vector3 screenPosition)
	{
		gameObject.SetActive(true);
		GameManager.me.uiManager.uiMenu.uiWorldMap.friendDetailButton.gameObject.SetActive(false);
		_v = GameManager.me.uiManager.uiMenu.camera.ScreenToWorldPoint(screenPosition);	
		_v.x -= 10.0f;
		_v.z = 1626;

		if(_v.y < 2617)
		{
			_v.y += 70.0f;
			isDownArrow = true;
			spArrowUp.enabled = false;
			spArrowDown.enabled = true;
		}
		else
		{
			_v.y -= 70.0f;
			isDownArrow = false;
			spArrowUp.enabled = true;
			spArrowDown.enabled = false;
		}

		transform.position = _v;

		rankers.Clear();
		rankers.AddRange(fdList);
		draw();
	}

	protected override void setPassData ()
	{
		int len = rankers.Count;
		
		sort();
		
		for(int i = 0; i < len; ++i)
		{
			rankers_obj.Add ((object)rankers[i]);
		}
	}
	
	
	P_FriendDataWorldMapSorter _sort = new P_FriendDataWorldMapSorter();
	
	protected override void sort()
	{
		rankers.Sort(_sort);
	}
}


public class P_FriendDataWorldMapSorter  : IComparer<P_FriendData>
{
	public int Compare(P_FriendData x, P_FriendData y)
	{
		//		1> 챔피언십 등급
		//		2>  우정레벨
		//		3> 유저레벨
		//		4> 닉네임(가나다)

		int i = y.receivedFP.CompareTo(x.receivedFP);
		if (i == 0) i = y.fLevel.CompareTo(x.fLevel);
		if (i == 0) i = y.league.CompareTo(x.league);

		return i;
	}	
}


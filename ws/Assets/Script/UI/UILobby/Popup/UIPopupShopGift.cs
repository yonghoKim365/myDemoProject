using UnityEngine;
using System.Collections;
using System;

public class UIPopupShopGift : UIPopupBase 
{
	public P_Product selectedGiftItem;

	public UIFriendList list;

	protected override void awakeInit ()
	{
	}



	public override void show ()
	{
		base.show ();

		list.draw();

		EpiServer.instance.sendGetFriendsForGiftList();

	}

}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupTableGuide : UIPopupBase {


	public GameObject goChampionshipInfoContainer;
	public GameObject goFriendInfoContainer;

	public UISprite spChampionshipTitle;
	public UILabel lbChampionshipDescription, lbChampionshipWarning;

	public UILabel lbFriendTitle, lbFriendDescription, lbFriendDescription2, lbFriendWarning, lbEnergyFriend, lbRuneFriend, lbRubyFriend, lbGoldFriend;
	public UILabel lbChampionshipWinningGold;

	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public override void show ()
	{
		base.show ();
	}

	public enum GuideType
	{
		Championship, Friend
	}


	bool _setFriendDesc = false;
	public void show(GuideType type)
	{
		show ();

		switch(type)
		{
		case GuideType.Championship:
			goChampionshipInfoContainer.SetActive(true);
			goFriendInfoContainer.SetActive(false);

			lbChampionshipDescription.text = Util.getUIText("CHAMP_INFO_DESCRIPTION");
			lbChampionshipWarning.text = Util.getUIText("CHAMP_INFO_WARNING");

			lbChampionshipWinningGold.text = Util.getUIText("CHAMP_WINNING_GOLD");

			break;
		case GuideType.Friend:
			goChampionshipInfoContainer.SetActive(false);
			goFriendInfoContainer.SetActive(true);

			if(_setFriendDesc == false)
			{
				_setFriendDesc = true;
				lbFriendTitle.text = Util.getUIText("FRIEND_INFO_TITLE");
				lbFriendDescription.text = Util.getUIText("FRIEND_INFO_DESCRIPTION");
				lbFriendDescription2.text = Util.getUIText("FRIEND_INFO_DESCRIPTION2");
				lbFriendWarning.text = Util.getUIText("FRIEND_INFO_WARNING");
			}

			refreshFriend();

			break;
		}
	}


	public void refreshFriend()
	{
		if(GameDataManager.instance.friendRewardTable == null) return;

		string energy = "";
		string ruby = "";
		string gold = "";
		string rune = "";

		int index = 0;
		foreach(KeyValuePair<string, P_FriendRewardRow> kv in GameDataManager.instance.friendRewardTable)
		{
			energy += ((index > 0)?"\n":"") + kv.Value.energy1 + "~" + kv.Value.energy3;

			ruby += ((index > 0)?"\n":"") + kv.Value.ruby1 + "~" + kv.Value.ruby3;

			gold += ((index > 0)?"\n":"") + Util.GetCommaScore(kv.Value.gold1) + "~" + Util.GetCommaScore(kv.Value.gold3);

			if(string.IsNullOrEmpty( kv.Value.rune3 ) == false)
			{
				rune += ((index > 0)?"\n":"") + kv.Value.rune1.Substring(kv.Value.rune1.Length - 1) + "~" + kv.Value.rune3.Substring(kv.Value.rune3.Length - 1) + Util.getUIText("GRADE");
			}
			else
			{
				rune += ((index > 0)?"\n":"") + kv.Value.rune1.Substring(kv.Value.rune1.Length - 1) + Util.getUIText("GRADE");
			}


			++index;
		}

		lbEnergyFriend.text = energy;
		lbRubyFriend.text = ruby;
		lbGoldFriend.text = gold;
		lbRuneFriend.text = rune;
	}


}

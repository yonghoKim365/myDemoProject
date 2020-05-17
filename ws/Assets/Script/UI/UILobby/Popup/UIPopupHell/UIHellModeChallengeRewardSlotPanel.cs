using UnityEngine;
using System.Collections;

public class UIHellModeChallengeRewardSlotPanel : MonoBehaviour {

	public UILabel lbRank, lbGold, lbRune, lbEquip;

	public UISprite spEquipIcon;

	public void setData(string rank, string gold, string rune, string equip)
	{
		lbRank.text = rank;
		lbGold.text = gold;
		lbRune.text = rune;
		lbEquip.text = equip;

		spEquipIcon.cachedGameObject.SetActive( !string.IsNullOrEmpty(equip) );
	}

}

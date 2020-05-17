using UnityEngine;
using System.Collections;

public class UIHellModeWeekRewardSlotPanel : MonoBehaviour {

	public UISprite spIcon, spTopRankBg;
	public UILabel lbRank, lbCount;

	public void setData(string rankText, string icon, string count, float yPos = -69, bool showTopRankBg = false)
	{
		lbRank.text = rankText;

		Vector3 v = lbRank.cachedTransform.localPosition;
		v.y = yPos;
		lbRank.cachedTransform.localPosition = v;

		spIcon.spriteName = icon;

		spIcon.MakePixelPerfect();

		lbCount.text = count;

		spTopRankBg.enabled = spTopRankBg;


	}

}

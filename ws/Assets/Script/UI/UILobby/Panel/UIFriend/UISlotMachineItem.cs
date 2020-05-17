using UnityEngine;
using System.Collections;

public class UISlotMachineItem : MonoBehaviour 
{
	public UISprite spIcon;
	public UISprite spGrade;
	public UILabel lbCount;

	public Transform tf;


	void OnDestroy()
	{
		tf = null;
		spIcon = null;
		lbCount = null;
	}

	public Vector3 finalPosition = new Vector3();


	public void setData(string code, int count, string gatch)
	{
		switch(code)
		{
		case WSDefine.REWARD_TYPE_ENERGY:
			spIcon.spriteName = WSDefine.ICON_REWARD_ENERGY;
			break;
		case WSDefine.REWARD_TYPE_GOLD:
			spIcon.spriteName = WSDefine.ICON_REWARD_GOLD;
			break;
		case WSDefine.REWARD_TYPE_RUBY:
			spIcon.spriteName = WSDefine.ICON_REWARD_RUBY;
			break;
		case WSDefine.REWARD_TYPE_FRIENDPOINT:
			spIcon.spriteName = WSDefine.ICON_HART;
			break;
		case WSDefine.REWARD_TYPE_TICKET:
			spIcon.spriteName = WSDefine.ICON_TICKET;
			break;
		case WSDefine.REWARD_TYPE_EXP:
			spIcon.spriteName = WSDefine.ICON_REWARD_EXP;
			break;

		case WSDefine.REWARD_TYPE_GACHA:
		case WSDefine.REWARD_TYPE_RUNE:
		case WSDefine.REWARD_TYPE_ITEM:
			spIcon.spriteName = WSDefine.ICON_REWARD_RUNE;

//			Debug.LogError("==== 수정 필요~!! 아이콘 없음 ====");
			break;
		}

//		spIcon.MakePixelPerfect();

		if(code == WSDefine.REWARD_TYPE_RUNE)
		{
			int gradeIndex = gatch.LastIndexOf("_");
			if(gatch != null && gradeIndex > 0)
			{
				lbCount.enabled = false;
				spGrade.enabled = true;
				spGrade.spriteName = "img_runegrade_" + gatch.Substring(gradeIndex + 1).ToLower();
				return;
			}
		}

	   lbCount.text = count.ToString();
	   lbCount.enabled = true;
	   spGrade.enabled = false;

	}

}

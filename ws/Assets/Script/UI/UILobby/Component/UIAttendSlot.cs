using UnityEngine;
using System.Collections;

public class UIAttendSlot : MonoBehaviour 
{
	public UILabel lbDay, lbItemName;

	public UISprite spIcon, spGrade, spCheck;

	public string data;

	public UILabel lbNewSRune;

	public void setData(string inputData)
	{
		if(string.IsNullOrEmpty(inputData)) return;

		data = inputData;

		lbNewSRune.gameObject.SetActive(false);

		if(data.StartsWith("RU"))
		{
			spIcon.spriteName = WSDefine.ICON_REWARD_RUBY;
			spGrade.cachedGameObject.SetActive(false);
			
			lbItemName.text = Util.getUIText("RUBY_NUM", data.Substring(data.LastIndexOf("_")+1));
		}
		else if(data.StartsWith("GO"))
		{
			spIcon.spriteName = WSDefine.ICON_REWARD_GOLD;
			spGrade.cachedGameObject.SetActive(false);
			
			lbItemName.text = Util.getUIText("GOLD_NUM", data.Substring(data.LastIndexOf("_")+1));
		}
		else if(data.StartsWith("EN"))
		{
			spIcon.spriteName = WSDefine.ICON_REWARD_ENERGY;
			spGrade.cachedGameObject.SetActive(false);
			
			lbItemName.text = Util.getUIText("ENG_NUM", data.Substring(data.LastIndexOf("_")+1));
		}
		else if(data.StartsWith("EV"))
		{
			spIcon.spriteName = WSDefine.ICON_REWARD_RUNE;
			
			spGrade.cachedGameObject.SetActive(true);
			spGrade.spriteName = "img_runegrade_"+data.Substring(data.LastIndexOf("_")+1).ToLower();
			
			if(data.StartsWith("EVENT_U"))
			{
				spIcon.spriteName = WSDefine.ICON_REWARD_ANIMAL;
				lbItemName.text = data.Substring(data.LastIndexOf("_")+1) + Util.getUIText("GRADE") + " " + Util.getUIText("UNIT_RUNE");

				if (data == "EVENT_U_NEW_S"){
					lbNewSRune.gameObject.SetActive(true);
				}
			}
			else if(data.StartsWith("EVENT_S"))
			{
				spIcon.spriteName = WSDefine.ICON_REWARD_RUNE;
				lbItemName.text = data.Substring(data.LastIndexOf("_")+1) + Util.getUIText("GRADE") + " " + Util.getUIText("SKILL_RUNE");
			}
			else if(data.StartsWith("EVENT_E"))
			{
				spIcon.spriteName = WSDefine.ICON_REWARD_EQUIP;
				lbItemName.text = data.Substring(data.LastIndexOf("_")+1) + Util.getUIText("GRADE") + " " + Util.getUIText("EQUIP");
			}
		}
		else if(data.StartsWith("RSTONE"))
		{
			spIcon.spriteName = WSDefine.ICON_REWARD_RUNESTONE;
			lbItemName.text = Util.getUIText("STR_N_RUNESTONE",data.Substring(data.LastIndexOf("_")+1));

			spGrade.cachedGameObject.SetActive(false);
		}
		
		spIcon.MakePixelPerfect();

	}

}
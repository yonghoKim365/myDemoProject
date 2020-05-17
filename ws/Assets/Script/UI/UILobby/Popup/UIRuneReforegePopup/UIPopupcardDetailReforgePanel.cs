using UnityEngine;
using System.Collections;

public class UIPopupcardDetailReforgePanel : MonoBehaviour 
{
	public UIRuneReforegePopupATTRSlot[] slots;

	public void setData(GameIDData gd)
	{
		if(gd.totalPLevel <= 0)
		{
			gameObject.SetActive(false);
			return;
		}
		else
		{
			gameObject.SetActive(true);
		}

		TranscendData td = gd.transcendData;

		for(int i = 0; i < 4; ++i)
		{
			slots[i].lbName.text = td.description[i];


			if(GameManager.me.uiManager.popupReforege.step == 2)
			{
				int newValue = td.getApplyRateValue(gd.transcendLevel[i],  i);
				int diff = newValue - GameManager.me.uiManager.popupReforege.currentEffectValue[i];

				string resultType = td.getApplyRateTypeString(i);
				string result = td.getApplyRateValueString(gd.transcendLevel[i],  i);
				
				if(diff > 0)
				{
					result += " [ffe400](↑" + diff  + resultType +")[-]";
				}
				else if(diff < 0)
				{
					result += " [be1010](↓" + diff  + resultType +")[-]";
				}
				
				slots[i].lbValue.text = result;


			}
			else
			{
				slots[i].lbValue.text = td.getApplyRateValueString(gd.transcendLevel[i], i);
			}
		}
	}

}

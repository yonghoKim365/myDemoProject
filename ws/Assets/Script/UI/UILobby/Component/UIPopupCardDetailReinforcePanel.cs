using UnityEngine;
using System.Collections;

public class UIPopupCardDetailReinforcePanel : MonoBehaviour {

	public UILabel lbReinforceLevel, lbReinforceProgressPercent;
	public UISprite spReinforeceProgress;

	Vector3 _v;
	
	public void setReinforceData(GameIDData data)
	{
		float reinforcePercent = data.getReinforceProgressPercent();
		int tempLevel =  data.reinforceLevel;

		lbReinforceLevel.text = "l"+tempLevel;
		
		if(tempLevel >= GameIDData.MAX_LEVEL)
		{
			lbReinforceProgressPercent.text = "MAX";//"100%";
			spReinforeceProgress.fillAmount = 1;
		}
		else
		{
			lbReinforceProgressPercent.text = Mathf.FloorToInt(reinforcePercent * 100.0f) + "%";
			spReinforeceProgress.fillAmount = reinforcePercent;
		}
	}
}

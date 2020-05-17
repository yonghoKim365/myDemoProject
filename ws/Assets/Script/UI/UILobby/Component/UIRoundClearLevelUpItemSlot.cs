using UnityEngine;
using System.Collections;

public class UIRoundClearLevelUpItemSlot : MonoBehaviour 
{
	public UISprite spOldExpPer, spNewExpPer, spLevelup;

	public ParticleSystem particleLevelup;

	public UILabel lbPlusLevel;

	public UIChallengeItemSlot itemSlot;


	public RoundClearLevelupItemData itemData;

	public void setData(RoundClearLevelupItemData data)
	{
		itemData = data;

		itemSlot.setData(data.id);

		particleLevelup.gameObject.SetActive(false);

		if(data.plusLevel > 0)
		{
			spOldExpPer.fillAmount = 0;
			lbPlusLevel.text = data.plusLevel.ToString();
			lbPlusLevel.cachedGameObject.SetActive(true);
			spLevelup.enabled = true;
		}
		else
		{
			lbPlusLevel.cachedGameObject.SetActive(false);
			spOldExpPer.fillAmount = data.oldPercent;
			spLevelup.enabled = false;

		}

		if(itemSlot.infoData.level >= 20)
		{
			if(data.plusLevel == 0)
			{
				spNewExpPer.fillAmount = 0;
				spOldExpPer.fillAmount = 1;
			}
			else
			{
				spNewExpPer.fillAmount = 1.0f;
			}
		}
		else
		{
			spNewExpPer.fillAmount = itemSlot.infoData.getReinforceProgressPercent();
		}
	}


	void OnDisable()
	{
	}

}

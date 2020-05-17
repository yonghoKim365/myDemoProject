using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIReinforceBarPanelProgressBar : MonoBehaviour 
{
	public UISprite spProgressBar, spProgressBar2;
	public UILabel lbProgress, lbLevel;//, lbPlusLevel;

	Vector3 _v;

	public void updatePercentLabel(int settingLevel)
	{
//		_v = lbProgress.cachedTransform.localPosition;
//		if(settingLevel >= 5)
//		{
//			_v.x = 97 + 4 * 81;
//		}
//		else
//		{
//			_v.x = 97 + settingLevel * 81;
//		}
//		lbProgress.cachedTransform.localPosition = _v;
	}


}

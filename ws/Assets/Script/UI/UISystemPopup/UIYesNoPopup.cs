using UnityEngine;
using System.Collections;

public class UIYesNoPopup : UISystemPopupBase {


	Vector3 _leftButtonPos = new Vector3(-102f, -108f, -233f);
	Vector3 _rightButtonPos = new Vector3(98f, -108f, -233f);

	protected override void awakeInit ()
	{
		base.awakeInit ();

//		_leftButtonPos = btnClose.transform.localPosition;
//		_rightButtonPos = btnYes.transform.localPosition;
	}


//	public void setButton(bool yesPositionIsRight)
//	{
//		if(yesPositionIsRight)
//		{
//			btnClose.transform.localPosition = _leftButtonPos;
//			btnYes.transform.localPosition = _rightButtonPos;
//		}
//		else
//		{
//			btnYes.transform.localPosition = _leftButtonPos;
//			btnClose.transform.localPosition = _rightButtonPos;
//		}
//	}

}

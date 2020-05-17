using UnityEngine;
using System.Collections;
using System;

public class UIPopupNoticeUpdateWaitingArea : UIPopupBase {


	public UILabel lbText;

	public UISprite spCharacter;

	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public override void show ()
	{
		base.show ();

		switch(GameDataManager.instance.maxAct)
		{
		case 5:
			lbText.text = Util.getUIText("WAIT_UPDATE_ACT5");
			spCharacter.spriteName = "img_characimg1";
			break;
		case 6:
			lbText.text = Util.getUIText("WAIT_UPDATE_ACT6");
			spCharacter.spriteName = "img_characimg3";
			break;
		case 7:
			lbText.text = Util.getUIText("WAIT_UPDATE_ACT7");
			spCharacter.spriteName = "img_characimg3";
			break;
		}
	}

}

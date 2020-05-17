using UnityEngine;
using System.Collections;

public class UILevelupPopup : UISystemPopupBase 
{
	public UILabel lbLevel;

	public ParticleSystem effect;

	public override void show (PopupData pd, string msg = "")
	{
		GameManager.setTimeScale = 1.0f;
		base.show (pd, msg);
		lbMsg.text = Util.getUIText("MSG_HERO_LEVELUP");
		lbLevel.text = msg;

		effect.Play();
	}

	public override void hide ()
	{
		base.hide ();
	}
}

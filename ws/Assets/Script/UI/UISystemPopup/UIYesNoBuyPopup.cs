using UnityEngine;
using System.Collections;

public class UIYesNoBuyPopup : UISystemPopupBase {


	public UILabel lbMsg2;

	protected override void awakeInit ()
	{
		base.awakeInit ();
	}


	public override void show (PopupData pd, string msg)
	{
		base.show (pd, msg);

		if(pd.data != null && pd.data.Length > 0)
		{
			lbMsg2.text = pd.data[0].ToString();
		}
	}

}

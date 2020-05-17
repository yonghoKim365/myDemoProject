using UnityEngine;
using System.Collections;

public class UIAdvicePopup : UISystemPopupBase 
{
	public GameObject goOneButtonContainer;
	public GameObject goTwoButtonContainer;

	public UIButton btnOneButtonGo;


	protected override void awakeInit ()
	{
		base.awakeInit ();

		UIEventListener.Get( btnOneButtonGo.gameObject ).onClick = onYes;
	}



	public override void show (PopupData pd, string msg)
	{
		base.show (pd, msg);

		if((int)pd.data[0] == 0) 
		{
			goTwoButtonContainer.gameObject.SetActive(false);
			goOneButtonContainer.gameObject.SetActive(true);
		}
		else
		{
			goTwoButtonContainer.gameObject.SetActive(true);
			goOneButtonContainer.gameObject.SetActive(false);
		}

	}


}

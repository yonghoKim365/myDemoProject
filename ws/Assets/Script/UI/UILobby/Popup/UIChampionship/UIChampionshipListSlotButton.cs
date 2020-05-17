using UnityEngine;
using System.Collections;

public class UIChampionshipListSlotButton : MonoBehaviour {

	public UISprite spImage;
	public UISprite spSelect;
	public UIButton btn;

	public UIDragScrollView scrollView;

	public bool isDefaultAttackButton = false;

	void Start()
	{
		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.popupChampionship.list.panel;
	}

	public string type;

	public void setData(string result, bool blockButton)
	{
		type = result;

		if(btn != null) btn.isEnabled = true;

		switch(result)
		{
		case "N":
			spImage.spriteName = "ibtn_mark_emptyidle";
			if(blockButton) btn.isEnabled = false;
			else
			{
				if(btn != null)  btn.name = "0";
			}
			break;
		case "L":
			spImage.spriteName = "ibtn_mark_loseidle";
			if(btn != null) btn.name = "l";
			break;
		case "W":
			spImage.spriteName = "ibtn_mark_winidle";
			if(btn != null) btn.name = "1";
			break;
		}


		if(spSelect != null) spSelect.enabled = false;
	}

}

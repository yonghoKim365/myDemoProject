using UnityEngine;
using System.Collections;
using System;

public class UIDebugRoundListSlotPanel : UIListGridItemPanelBase {

	public UIButton btnStart;
	public UILabel lbTitle;




	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnStart.gameObject).onClick = onClickRound;
	}
	
	void onClickRound(GameObject go)
	{
		GameManager.me.uiManager.uiMenu.uiWorldMap.debugRoundList.askPopup.gameObject.SetActive(true);
		GameManager.me.uiManager.uiMenu.uiWorldMap.debugRoundList.askPopup.show(_id, isSigong);
	}











	public override void setPhotoLoad()
	{
//		if(data == null) return;
	}	

	string _id;
	bool isSigong = false;

	public override void setData(object obj)
	{
		_id = (string)obj;
		lbTitle.text = _id;


		if(_id.StartsWith("SIG"))
		{
			string[] f = _id.Split('/');
			_id = f[1];
			isSigong = true;
		}
		else
		{
			isSigong = false;
		}

	}

}

using UnityEngine;
using System.Collections;
using System;
using System.Text;

public class UIPopupNickName : UIPopupBase {


	public UILabel lbText;
	public UIInput inputField;

	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public override void show ()
	{
		base.show ();
		lbText.text = Util.getUIText("UI_NICKNAME");
		inputField.defaultText = Util.getUIText("UI_NICKNAME_DEFAULT");

		IgaworksUnityPluginAOS.Adbrix.firstTimeExperience("join_try");

	}


	protected override void onClickClose (GameObject go)
	{
		inputField.label.text = inputField.label.text.Trim();

		if(Encoding.UTF8.GetByteCount(inputField.label.text) > 24)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NICKNAME_OVER"));
		}
		else
		{
			if(checkNickNameCharacter(inputField.label.text) == false)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NICK_SPACE_ERORR"));
				return;
			}

			if(inputField.label.text.Contains(" "))
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("NICK_CHAR_ERROR"));
				return;
			}


			EpiServer.instance.sendSetNickName(inputField.label.text);
		}
	}


	public bool checkNickNameCharacter(string name)
	{
		char[] ca = name.ToCharArray();

		int len = ca.Length;

		for(int i = 0; i < len; ++i)
		{
			if(GameManager.me.uiManager.uiMenu.uiLobby.lbName.trueTypeFont.HasCharacter(ca[i]) == false)
			{
				Debug.Log(ca[i].ToString());
				return false;
			}
		}

		return true;
	}



}

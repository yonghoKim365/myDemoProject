using UnityEngine;
using System.Collections;
using System;

public class UIPopupMessage : UIPopupBase {

	public UIMessageList list;

	public UILabel lbNoMessage;

	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public override void show ()
	{
		base.show ();

		list.clear();

		EpiServer.instance.sendMessageList();
	}


	public void refresh()
	{
		list.draw();
		lbNoMessage.enabled = !(list.nowMsg > 0);
		System.GC.Collect();
	}

	private UIMessageListSlotPanel _panel;

	public void onReceiveItem(UIMessageListSlotPanel panel, string[] itemList)
	{
		_panel = panel;
		EpiServer.instance.sendConfirmMessasge(panel.data.id, itemList);
	}

}

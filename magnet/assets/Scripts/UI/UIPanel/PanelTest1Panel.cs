using UnityEngine;
using System.Collections;

public class PanelTest1Panel : UIBasePanel {

	public override void Init()
	{
		base.Init();
	}

	public override void LateInit()
	{
		base.LateInit ();
	}
	
	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}
	
	public override void Hide()
	{
		base.Hide();		
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
		UIMgr.OpenTown();
	}    

	public override void Close()
	{
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
		base.Close();
		UIMgr.OpenTown();
		
	}

}

using UnityEngine;
using System.Collections;

public class ChangeStageTransition : MonoBehaviour {

	public Animation animation;
	
	public void start()
	{
		animation.Play();
	}
	
	public void onStartChange()
	{
//		GameManager.me.mapManager.startChangeStageTheme();
	}
	
	public void onCompleteChange()
	{
//		GameManager.me.mapManager.onCompleteChangeStageTheme();
	}
}

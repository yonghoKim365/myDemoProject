
using UnityEngine;
using System.Collections;

public class UIWorldMapStageClearRewardPopup : MonoBehaviour 
{
	
	public UIButton btnClose;
	public UILabel lbMsg;

	protected Vector3 _v;
	
	void Awake()
	{
		UIEventListener.Get(btnClose.gameObject).onClick = onClose;
	}
	
	protected void onClose(GameObject go)
	{
		RuneStudioMain.instance.playMakeResult(new string[]{GameDataManager.instance.stageClearRewardItem}, false);
		hide();
	}
	
	public virtual void show()
	{
		gameObject.SetActive(true);

		int prevAct = GameDataManager.instance.maxAct;
		int prevStage = GameDataManager.instance.maxStage - 1;

		if(prevStage == 0)
		{
			--prevAct;
			prevStage = 5;
		}

		lbMsg.text = Util.getUIText("ACTCLEAR_GIFT",prevAct.ToString(), prevStage.ToString());
	}
	
	public virtual void hide()
	{
		gameObject.SetActive(false);
	}
}

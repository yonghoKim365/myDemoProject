using UnityEngine;
using System.Collections;
using System;

public class UIPopupDefenceResult : UIPopupBase {

	public UIPopupDefenceResultList list;


	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public override void show ()
	{
		base.show ();
	}

	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);

		TutorialManager.instance.check("T25");
	}


	public bool checkShowDefenceResult()
	{
		bool hasResult = false;

		if(TutorialManager.nowPlayingTutorial("T24")) return false;

		if(GameManager.me.uiManager.popupChampionship.gameObject.activeSelf)
		{
			if(GameDataManager.instance.championshipData != null)
			{
				if(GameDataManager.instance.championshipData.defenceRecords != null)
				{
					if(GameDataManager.instance.championshipData.defenceRecords.Length > 0)
					{
						hasResult = true;
						show ();
						//list.clear();
						list.draw();

						// 한번 뿌렸으면 지움.
						GameDataManager.instance.championshipData.defenceRecords = null;
						GameDataManager.instance.championshipDefence = WSDefine.NO;
					}
				}
			}
		}

		return hasResult;
	}
}

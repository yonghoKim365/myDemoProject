using UnityEngine;
using System.Collections;
using System;

public class UIPopupChampionshipLastWeekResult : UIPopupBase 
{

	public UISprite spGradeIcon, spGradeText, spGradeBg;

	public UILabel lbLeftTime;

	public UIChampionshipLastWeekResultList list;


	protected override void awakeInit ()
	{
	}


	public override void show ()
	{
		base.show ();
		refresh();
	}

	public void refresh()
	{
		if(GameManager.me.uiManager.popupChampionship.gameObject.activeSelf == false) return;

		switch(GameDataManager.instance.lastChampionshipData.league)
		{
		case WSDefine.LEAGUE_BRONZE:
			spGradeIcon.spriteName = "img_gacha_bronz";
			spGradeText.spriteName = "icn_levelmedal_bronz_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg1";
			break;
		case WSDefine.LEAGUE_SILVER:
			spGradeIcon.spriteName = "img_gacha_silver";
			spGradeText.spriteName = "icn_levelmedal_silver_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg1";
			break;
		case WSDefine.LEAGUE_GOLD:
			spGradeIcon.spriteName = "img_gacha_gold";
			spGradeText.spriteName = "icn_levelmedal_gold_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg2";
			break;
		case WSDefine.LEAGUE_MASTER:
			spGradeIcon.spriteName = "img_gacha_master";
			spGradeText.spriteName = "icn_levelmedal_master_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg2"; 
			break;
		case WSDefine.LEAGUE_PLATINUM:
			spGradeIcon.spriteName = "img_gacha_platinum";
			spGradeText.spriteName = "icn_levelmedal_platinum_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg3"; // gold
			break;
		case WSDefine.LEAGUE_LEGEND:
			spGradeIcon.spriteName = "img_gacha_legend";
			spGradeText.spriteName = "icn_levelmedal_legend_title";
			spGradeBg.spriteName = "img_levelmedal_title_bg3"; // gold
			break;
		}

		list.draw();

		System.GC.Collect();
	}

	/*
	float _delayTime = 0;
	void Update()
	{
		if(GameDataManager.instance.championshipData == null) return;

		if(_delayTime > 0)
		{
			_delayTime -= Time.smoothDeltaTime;
		}
		else _delayTime = 0.5f;

		int time = GameDataManager.instance.championshipData.remainedTime;
		if(time > 0)
		{
			TimeSpan ts = (DateTime.Now - GameDataManager.instance.championShipCheckTime);
			int leftTime = time - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = "종료";
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString(leftTime);
			}
		}
	}
	*/
	
	
}

using UnityEngine;
using System.Collections;
using System;

public class UIPopupChampionship : UIPopupBase 
{
	public UIButton btnChampionshipGuide, btnLastWeekResult;

	public UISprite spGradeIcon, spGradeText, spGradeBg;

	public UILabel lbLeftTime;

	public UIChampionshipList list;

	public UIChampionshipActionTooltip actionTooltip;

	public UIButton btnDummy;


	protected override void awakeInit ()
	{
		UIEventListener.Get(btnChampionshipGuide.gameObject).onClick = onClickChampionshipGuide;
		UIEventListener.Get(btnLastWeekResult.gameObject).onClick = onClickChampionshipLastWeekResult;


		UIEventListener.Get(btnDummy.gameObject).onPress = onPressDummy;
	}

	void onPressDummy(GameObject go, bool isPress)
	{
		actionTooltip.hide();
	}


	void onClickChampionshipGuide(GameObject go)
	{
		GameManager.me.uiManager.popupTableGuide.show(UIPopupTableGuide.GuideType.Championship);
	}


	void onClickChampionshipLastWeekResult(GameObject go)
	{
		EpiServer.instance.sendLastChampionshipData();
	}


	public override void show ()
	{
		base.show ();

		actionTooltip.hide();

		list.clear();

		setLeagueData();

		if(GameDataManager.instance.champMemberId == -1 || TutorialManager.instance.clearCheck("T24") == false)
		{
			EpiServer.instance.sendRejoinChampionship();
		}
		else
		{
			EpiServer.instance.sendChampionShipData();
		}
	}

	public void refresh()
	{
		if(GameManager.me.uiManager.uiMenu.currentPanel != UIMenu.WORLD_MAP || gameObject.activeSelf == false) return;

		setLeagueData();

		list.draw();

		if(TutorialManager.instance.check("T24"))
		{

		}
		else
		{
			GameManager.me.uiManager.popupDefenceResult.checkShowDefenceResult();
		}

		System.GC.Collect();

	}

	void setLeagueData()
	{
		if(GameManager.me.uiManager.uiMenu.currentPanel != UIMenu.WORLD_MAP || gameObject.activeSelf == false) return;
		
		switch(GameDataManager.instance.champLeague)
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
	}


	float _delayTime = 0;
	private float _delay = 0.0f;
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
				lbLeftTime.text = Util.getUIText("END");
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString(leftTime);
			}
		}
		
	}


	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		GameManager.me.uiManager.uiMenu.uiWorldMap.refreshFriendPhoto();
	}

	
}

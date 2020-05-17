using UnityEngine;
using System.Collections;
using System;

public class UIPopupHellModeGuide : UIPopupBase {


	public UIButton tabWeek,tabChallenge, btnHelpChallengeReward;
	public UISprite spTabWeek, spTabChallenge;
	public UISprite spTabWeekFont, spTabChallengeFont;

	public GameObject goWeekPanel, goChallengePanel;

	public UIHellModeChallengeRewardSlotPanel[] challengeRewards;
	public UIHellModeWeekRewardSlotPanel[] weekRewards;

	public UIIconTooltip challengeRewardTooltip;

	public UILabel lbRewardListWord;

	protected override void awakeInit ()
	{
		UIEventListener.Get(tabWeek.gameObject).onClick = onClickTabWeek;
		UIEventListener.Get(tabChallenge.gameObject).onClick = onClickTabChallenge;

		UIEventListener.Get(btnHelpChallengeReward.gameObject).onClick = onClickChallengeRewardTooltip;
	}

	private string _weekTooltip = null;
	private string _challengeTooltip = null;

	void onClickChallengeRewardTooltip(GameObject go)
	{
		if(type == Type.Week)
		{
			if(_weekTooltip == null) _weekTooltip = Util.getUIText("HELL_REWARD_WEEK_TOOLTIP");
			challengeRewardTooltip.lbText.text = _weekTooltip;
			challengeRewardTooltip.start(3.0f);
		}
		else if(type == Type.Challenge)
		{
			if(_challengeTooltip == null) _challengeTooltip = Util.getUIText("HELL_REWARD_CHALLENGE_TOOLTIP");
			challengeRewardTooltip.lbText.text = _challengeTooltip;
			challengeRewardTooltip.start(3.0f);
		}
	}

	bool _setRewardText = false;

	public override void show ()
	{
		base.show ();

		challengeRewardTooltip.hide();

		if(_setRewardText == false)
		{
			_setRewardText = true;

			string[] f;

			for(int i = 0; i <= 4; ++i)
			{
				f = Util.getUIText("HELL_CR"+i).Split('/');
				challengeRewards[i].setData(f[0],f[1],f[2],f[3]);
			}

			f = Util.getUIText("HELL_WR0").Split('/');
			weekRewards[0].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -69, true);


			f = Util.getUIText("HELL_WR1").Split('/');
			weekRewards[1].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -74, true);


			f = Util.getUIText("HELL_WR2").Split('/');
			weekRewards[2].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -57);


			f = Util.getUIText("HELL_WR3").Split('/');
			weekRewards[3].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1],-60);


			f = Util.getUIText("HELL_WR4").Split('/');
			weekRewards[4].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -71);


			f = Util.getUIText("HELL_WR5").Split('/');
			weekRewards[5].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -74);


			f = Util.getUIText("HELL_WR6").Split('/');
			weekRewards[6].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -60);


			f = Util.getUIText("HELL_WR7").Split('/');
			weekRewards[7].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -60);


			f = Util.getUIText("HELL_WR8").Split('/');
			weekRewards[8].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -60);


			f = Util.getUIText("HELL_WR9").Split('/');
			weekRewards[9].setData(f[0],WSDefine.ICON_REWARD_RUBY, f[1], -60);
		}



	}


	public enum Type
	{
		Week, Challenge, None
	}

	public Type type = Type.None;

	public void show(Type openType)
	{
		show();
		type = openType;
		refresh();
	}


	void refresh()
	{
		switch(type)
		{
		case Type.Challenge:

			spTabWeek.spriteName = "ibtn_tab_offidle";
			spTabChallenge.spriteName = "ibtn_tab_onidle";
			
			spTabWeekFont.spriteName = "ibtn_tab_reward1_offidle";
			spTabChallengeFont.spriteName = "ibtn_tab_reward2_onidle";
			
			goWeekPanel.SetActive(false);
			goChallengePanel.SetActive(true);

			lbRewardListWord.text = Util.getUIText("HELL_PLAY_R_LIST");

			break;
		case Type.Week:

			spTabWeek.spriteName = "ibtn_tab_onidle";
			spTabChallenge.spriteName = "ibtn_tab_offidle";
			
			spTabWeekFont.spriteName = "ibtn_tab_reward1_onidle";
			spTabChallengeFont.spriteName = "ibtn_tab_reward2_offidle";
			
			goWeekPanel.SetActive(true);
			goChallengePanel.SetActive(false);

			lbRewardListWord.text = Util.getUIText("HELL_RANK_R_LIST");

			break;
		}
	}


	void onClickTabWeek(GameObject go)
	{
		if(type == Type.Week) return;
		type = Type.Week;
		refresh();
		challengeRewardTooltip.hide();
	}

	void onClickTabChallenge(GameObject go)
	{
		if(type == Type.Challenge) return;
		type = Type.Challenge;
		refresh();
		challengeRewardTooltip.hide();
	}


}

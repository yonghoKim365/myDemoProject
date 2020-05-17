using UnityEngine;
using System.Collections;
using System;

public class UIPopupChampionshipResult : UIPopupBase {

	public GameObject goWinContainer;

	public UILabel lbPrize, lbName, lbMatchNumber, lbWinPoint, lbRewardType;

	public UISprite spResultType, spPrizeIcon;

	public GameObject goChampionshipPanel;

	public PhotoDownLoader face;

	public UILabel lbFriendlyResult, lbFriendlyGuide;


	protected override void awakeInit ()
	{
		//UIEventListener.Get(inputField.gameObject).
	}

	public string enemyId = "";
	public int matchNumber = 1;

	public string pvpName = "";

	public string enemyName = "";

	//public UIChampionshipListSlotButton[] btns;

	public GameObject goMoveContainer;

	bool needGoToMenu = false;

	public int prizeGold = 0;
	public int score = 0;

	public UILabel lbWinCnt;
	public UILabel lbLoseCnt;

	public void open(bool isFriendly = false)
	{
		if(isFriendly)
		{
			if(GameManager.me.stageManager.nowPlayingGameResult != Result.Type.Win)
			{
				AdviceData.checkAdvice(UISystemPopup.checkLevelupPopupAndReturnToScene);
				return;
			}
		}

		needGoToMenu = true;

		base.show ();


		face.init(UIPlay.pvpImageUrl);
		face.down(UIPlay.pvpImageUrl);

		lbName.text = PlayerPrefs.GetString("PVPNAME","");

		if(string.IsNullOrEmpty(lbName.text))
		{
			lbName.text = pvpName;
		}

//		lbRewardType.text = "";

		if(GameManager.me.stageManager.nowPlayingGameResult == Result.Type.Win)
		{
			spResultType.spriteName = "img_result_win";
			goWinContainer.SetActive(true);
//			goMoveContainer.transform.localPosition = new Vector3(-226.3281f,136.8f, -191.1182f);

			GameManager.soundManager.stopBG();
			SoundData.play("bgm_win_b");
		}
		else
		{
			spResultType.spriteName = "img_result_lose";
			goWinContainer.SetActive(false);
//			goMoveContainer.transform.localPosition = new Vector3(-226.3281f,104.6f, -191.1f);

			GameManager.soundManager.stopBG();
			SoundData.play("bgm_lose_a");
		}

		spResultType.MakePixelPerfect();

		lbWinCnt.gameObject.SetActive(false);
		lbLoseCnt.gameObject.SetActive(false);

		if(isFriendly)
		{

			goChampionshipPanel.gameObject.SetActive(false);
			lbWinPoint.gameObject.SetActive(false);
//			spPrizeIcon.spriteName = WSDefine.ICON_GOLD;
			//lbRewardType.text = Util.getUIText("RECEIVE_EXP");

			lbFriendlyResult.gameObject.SetActive(true);

			string pName = PlayerPrefs.GetString("CURRENT_FRIENDLY_ENEMY_NAME","");
			if(string.IsNullOrEmpty(pName))
			{
				pName = enemyName;
			}


			if(GameManager.me.stageManager.nowPlayingGameResult == Result.Type.Win)
			{
				lbFriendlyResult.text = Util.getUIText("WINNING_FRIENDLY_PVP",pName, Util.getUIText("WIN"));
//				lbFriendlyGuide.text = Util.getUIText("FRIENDLY_REWARD_GUIDE");
				lbFriendlyGuide.enabled = false;
			}
			else
			{
				lbFriendlyResult.text = Util.getUIText("WINNING_FRIENDLY_PVP",pName, Util.getUIText("LOSE"));
				lbFriendlyGuide.enabled = false;
			}
		}
		else
		{
			lbFriendlyResult.gameObject.SetActive(false);

//			spPrizeIcon.spriteName = WSDefine.ICON_GOLD;
			lbWinCnt.gameObject.SetActive(true);
			lbLoseCnt.gameObject.SetActive(true);

			goChampionshipPanel.gameObject.SetActive(true);

			int winCnt = 0;
			int loseCnt = 0;
			for(int i=0;i< UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
				if (GameDataManager.instance.championshipData.champions[enemyId].attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "W"){
					winCnt++;
				}
				else if (GameDataManager.instance.championshipData.champions[enemyId].attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "L"){
					loseCnt++;
				}
			}

			lbWinCnt.text = winCnt.ToString();
			lbLoseCnt.text = loseCnt.ToString();

//			lbRewardType.text = Util.getUIText("WINNING_PRIZE");
		}

		spPrizeIcon.MakePixelPerfect();

		lbWinPoint.gameObject.SetActive(false);

		if(isFriendly)
		{
			lbMatchNumber.text = Util.getUIText("FRIENDLY_MATCH");

		}
		else
		{
			lbMatchNumber.text = Util.getUIText("MATCH_NUMBER",matchNumber +"");


			//int point = 0;
			
			//_v = lbWinPoint.transform.localPosition;

			/*
			if(GameManager.me.stageManager.nowPlayingGameResult == Result.Type.Win) point = 3;
			
			switch(matchNumber)
			{
			case 1:
				_v = btns[0].transform.localPosition;
				break;
			case 2:
				_v = btns[1].transform.localPosition;
				break;
			case 3:
				_v = btns[2].transform.localPosition;
				break;
			}
			*/

			//_v = lbWinCnt.transform.localPosition;

			if(score > 0)
			{
				//lbWinPoint.gameObject.SetActive(true);
				//lbWinPoint.text = "+"+score.ToString();
				/*
				_v.y += 30.0f;
				lbWinPoint.transform.localPosition = _v;
				lbWinPoint.text = "+"+score.ToString();
				_v.y += 60;
				Color c = lbWinPoint.color;
				c.a = 1.0f;
				lbWinPoint.color = c;
				c.a = 0.0f;
				StartCoroutine(playWinPointEffect(c));
				*/
			}
			else
			{
				//lbWinPoint.gameObject.SetActive(false);
			}
		}

		lbPrize.text = Util.GetCommaScore( prizeGold );
	}


	IEnumerator playWinPointEffect(Color c)
	{
		yield return Util.ws1;
		TweenPosition.Begin(lbWinPoint.gameObject,1.3f, _v).method = UITweener.Method.EaseOut;
		TweenColor.Begin(lbWinPoint.gameObject,1.3f, c);

	}




	public override void hide (bool isInit = false)
	{
		base.hide(isInit);

		if(isInit == false && needGoToMenu)
		{
			if(GameManager.me.stageManager.nowPlayingGameResult != Result.Type.Win)
			{
				AdviceData.checkAdvice(UISystemPopup.checkLevelupPopupAndReturnToScene);
			}
			else
			{
				UISystemPopup.checkLevelupPopupAndReturnToScene();
			}
		}

		needGoToMenu = false;
	}


}

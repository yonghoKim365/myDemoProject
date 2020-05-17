using UnityEngine;
using System.Collections;
using System;

public class UIPopupFriendlyAttack : UIPopupBase {

	public PhotoDownLoader playerFace;
	public PhotoDownLoader pvpFace;

	public UILabel lbPlayerName, lbPVPName;

	public UILabel lbTitle;

	public UILabel lbReward, lbRubyExploreWord, lbRubyUsingNum, lbResetWord, lbRewardWord;

	public UIButton btnAttack;


	public static string friendlyPVPId = "";

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnAttack.gameObject).onClick = onClickAttack;
	}

	public void onClickAttack(GameObject go)
	{
		GameManager.me.uiManager.popupChampionshipResult.enemyId = _friendId;
		GameManager.me.uiManager.popupChampionshipResult.matchNumber = 1;
		EpiServer.instance.sendPlayFriendPVP(_friendId);
	}


	public void onReceivePVPData(ToC_PLAY_FRIEND_PVP p)
	{

		RoundData rd = GameManager.info.roundData["PVP"];
		GameManager.me.stageManager.setNowRound(rd, GameType.Mode.Friendly);

		DebugManager.instance.pvpPlayerData = DebugManager.instance.setPlayerData(false,p.fHero, p.fSelectUnitRunes, p.fSelectSkillRunes);

		if(p.fSubHero == null)
		{
			DebugManager.instance.pvpPlayerData2 = null;
		}
		else
		{
			DebugManager.instance.pvpPlayerData2 = DebugManager.instance.setPlayerData(false,p.fSubHero, p.fSubSelectUnitRunes, p.fSubSelectSkillRunes);
		}

		hide();

		GameManager.me.uiManager.showLoading();

		GameManager.me.startGame();
	}


	public override void show ()
	{
		base.show ();
	}


	private string _friendId;
	public void setData(string friendId, int leftNum, int reward)
	{
		_friendId = friendId;

		friendlyPVPId = friendId;

		UIPlay.playerImageUrl = PandoraManager.instance.localUser.image_url;
		UIPlay.pvpImageUrl = epi.GAME_DATA.appFriendDic[friendId].image_url;

		UIPlay.playerLeagueGrade = GameDataManager.instance.champLeague;

		if(GameDataManager.instance.friendDatas != null) UIPlay.pvpleagueGrade = GameDataManager.instance.friendDatas[friendId].league;
		else
		{
			Debug.LogError("Can't find friend's league information");
		}

		playerFace.init(PandoraManager.instance.localUser.image_url);
		pvpFace.init(UIPlay.pvpImageUrl);

		playerFace.down(PandoraManager.instance.localUser.image_url);
		pvpFace.down(UIPlay.pvpImageUrl);

		lbPlayerName.text = Util.GetShortID( PandoraManager.instance.localUser.f_Nick , 10);
		lbPVPName.text = Util.GetShortID( epi.GAME_DATA.appFriendDic[friendId].f_Nick, 10);
		PlayerPrefs.SetString("PVPNAME",lbPVPName.text);
		GameManager.me.uiManager.popupChampionshipResult.pvpName = lbPVPName.text;

		PlayerPrefs.SetString("CURRENT_FRIENDLY_ENEMY_NAME",epi.GAME_DATA.appFriendDic[friendId].f_Nick);
		GameManager.me.uiManager.popupChampionshipResult.enemyName = epi.GAME_DATA.appFriendDic[friendId].f_Nick;

		lbTitle.text = Util.getUIText("FRIENDLY_MATCH");

//		본인 액트	상대 챔피언십 등급별 획득 경험치					
//			브론즈	실버	골드	마스터	플래티넘	레전드
//				1	3	6	9	12	15	19
//				2	7	14	20	27	34	41
//				3	14	28	42	56	70	84
//				4	24	49	73	97	122	146



		lbReward.text = Util.GetCommaScore(reward); // 승리시 획득 경험치.
		lbRubyUsingNum.text = (leftNum).ToString(); // 남은 친선경기 횟수.

		lbRewardWord.text = Util.getUIText("FRIENDLY_WINNING_GOLD");

	}


}

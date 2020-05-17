using UnityEngine;
using System.Collections;
using System;

public class UIChampionshipListSlotPanel : UIListGridItemPanelBase {

	public UIButton btn, btnInfo;

	public UIDragScrollView btnInfoScrollView;

	public PhotoDownLoader face;

	public UILabel lbName, lbWinPoint, lbMyWinPoint,lbRanking, lbCoolTime;

	public UISprite spBorder, spIcon, spEmptyFace, spHighRankerIcon, spHighRankBg, spDefaultAttack;

	//public UILabel lbAtkWin, lbAtkLose, lbDefWin, lbDefLose;

	public UIChampionshipListSlotButton btnDefaultAttack;

	//public UIChampionshipListSlotButton[] btnAttacks = new UIChampionshipListSlotButton[3];
	//public UIChampionshipListSlotButton[] btnDefences = new UIChampionshipListSlotButton[3];

	public UIChampionshipResultContainer othersResultContainer;
	public UIChampionshipResultContainer myResultContainer;

	static Color _myColor = new Color(254f/255f,240f/255f,193f/255f);
	static Color _enemyColor = new Color(157.0f/255.0f, 216f/255f, 36f/255f);

	static public readonly string[] ROUND_IDS = new string[]{"R0","R1","R2","R3","R4","R5","R6"};

	P_Champion data;

	// Use this for initialization
	protected override void initAwake ()
	{
		//UIEventListener.Get(btn.gameObject).onClick = onSelect;
		UIEventListener.Get(btnDefaultAttack.btn.gameObject).onClick = onDefaultAttack;
		UIEventListener.Get(btnInfo.gameObject).onClick = onClickInfo;
	}

	void Start()
	{

		if(btnInfoScrollView != null) btnInfoScrollView.scrollView = GameManager.me.uiManager.popupChampionship.list.panel;
	}

	void onClickInfo(GameObject go)
	{

		if(PandoraManager.instance.localUser.userID != data.userId)
		{
			if(data.attackRounds[ROUND_IDS[0]].result == "N")
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default, Util.getUIText("INFO_AFTER_ATK"));
			}
			else
			{
				EpiServer.instance.sendEnemyData(data);
			}
		}
		else
		{
			GameManager.me.uiManager.popupFriendDetail.myData();
		}

	}

	bool checkCanFight()
	{

		for(int i = 0; i < 7; ++i)
		{
			if(data.attackRounds[ROUND_IDS[i]].result == "N" || data.attackRounds[ROUND_IDS[i]].result != "W")
			{
				return true;
			}
		}

		return false;
	}

	int numOfWinRound(){

		int winCnt = 0;
		for(int i = 0; i < 7; ++i)
		{
			if(data.attackRounds[ROUND_IDS[i]].result == "W")
			{
				winCnt++;
			}
		}
		
		return winCnt;
	}

	int numOfRemainRound(){
		
		int remainCnt = 0;
		for(int i = 0; i < 7; ++i)
		{
			if(data.attackRounds[ROUND_IDS[i]].result == "N")
			{
				remainCnt++;
			}
		}
		
		return remainCnt;
	}


	void onDefaultAttack(GameObject go)
	{

//		> 일반 공격이 가능한 경우 (7차전까지 치르지 않은 상태)				
//		* 가장 빠른 차전 공격 팝업 띄우기				


		if (numOfRemainRound() == 0){
#if UNITY_EDITOR
			Debug.LogError(" ERROR, logic error ======================");
			// 남은 라운드가 없으면 버튼이 비활성화되어 눌러질수가 없으므로 여기 들어오면 에러. //
#endif
			return;
		}

		int matchIndex = 0;

		for(int i=ROUND_IDS.Length-1;i>=0;i--){
			if(data.attackRounds[ROUND_IDS[i]].result == "N")
			{
				matchIndex = i;
			}
		}

		if(matchIndex > 0)
		{
			DateTime dt = DateTime.Now;
			
			GameDataManager.instance.championshipCoolTime.TryGetValue(data.userId, out dt);
			
			TimeSpan ts = (DateTime.Now - dt);
			int leftTime = data.coolTime - (int)ts.TotalSeconds;
			
			if(leftTime > 0)
			{
				UISystemPopup.open(UISystemPopup.PopupType.Default,  Util.getUIText("PVP_COOLTIME", Util.secToHourMinuteString(leftTime, Util.getUIText("TIME") + " ", Util.getUIText("MINUTE"), false) , (matchIndex+1).ToString()   ));
				return;
			}
		}

		GameManager.me.uiManager.popupChampionshipAttack.show();
		GameManager.me.uiManager.popupChampionshipAttack.setData(data, false, matchIndex);

	}


	
	void onRevengeAtkButton(GameObject go)
	{
		//if (UIChampionshipReplayPanel.getWinCnt(data.attackRounds) + UIChampionshipReplayPanel.getLoseCnt(data.attackRounds) == 0)return;
		//if (UIChampionshipReplayPanel.getLoseCnt(data.attackRounds) == 0 && UIChampionshipResultContainer.getNumOfOnePointEarnedRound(data.attackRounds)==0)return;
		//if (data.revengeCoolTime > 0)return;

		if (UIChampionshipResultContainer.isEnableReBattle(data)==false)return;

		TimeSpan ts = (DateTime.Now - GameDataManager.instance.championShipCheckTime);
		if (data.revengeCoolTime - (int)ts.TotalSeconds > 0)return;
		
		//GameManager.me.uiManager.popupChampionshipReplayPanel.setData(data);
		//GameManager.me.uiManager.popupChampionshipReplayPanel.show();

		bool isRematch = false;
		//int matchIndex = 0;

		/*
		[재도전 우선 순위].
		1. 플레이 하지 않은 날 또는 패배한 경기.
		2. 2캐릭 이상 보유 상태이면서 2점을 획득하지 않은 경기.
		*1->2 항목 순으로 가장 오래된 경기를 대상으로 재경기 도전 팝업을 표시함.
*/

		for(int i=0;i<ROUND_IDS.Length;i++){
			bool bOpenPopup = false;
			isRematch = false;

			if((data.attackRounds[ROUND_IDS[i]].result == "N" && data.coolTime == 0) || data.attackRounds[ROUND_IDS[i]].result == "L")
			{
				if(data.attackRounds[ROUND_IDS[i]].result == "L"){
					isRematch = true;
				}
				GameManager.me.uiManager.popupChampionshipAttack.show();
				GameManager.me.uiManager.popupChampionshipAttack.setData(data, isRematch, i);
				return;
			}
		}
		for(int i=0;i<ROUND_IDS.Length;i++){
			if(data.attackRounds[ROUND_IDS[i]].result == "W" && UIChampionshipReplayPanel.getWinPoint(data.attackRounds[ROUND_IDS[i]]) != 2){
				GameManager.me.uiManager.popupChampionshipAttack.show();
				GameManager.me.uiManager.popupChampionshipAttack.setData(data, true, i);
				return;
			}
		}
	}


	void onReplayButton(GameObject go)
	{
		if (UIChampionshipReplayPanel.isPvpBattleRecordExist(data.attackRounds, data.defenceRounds)==false)return;

		GameManager.me.uiManager.popupChampionshipReplayPanel.setData(data);
		GameManager.me.uiManager.popupChampionshipReplayPanel.show();
	}

	public override void setPhotoLoad()
	{
		if(data == null) return;

		if(data.showPhoto == WSDefine.TRUE)
		{
			face.down(data.imageUrl);
		}
	}	




	public const string ICON_DOWN = "icn_mark_skull";
	public const string ICON_BRONZ = "icn_levelmedal_bronz";
	public const string ICON_GOLD = "icn_levelmedal_gold";
	public const string ICON_LEGEND = "icn_levelmedal_legend";
	public const string ICON_MASTER = "icn_levelmedal_master";
	public const string ICON_PLATINUM = "icn_levelmedal_platinum";
	public const string ICON_SILVER = "icn_levelmedal_silver";

	static Vector3 _downgradeIconPosition = new Vector3(-370.4f,222.46f,220);
	static Vector3 _medalIconPosition = new Vector3(-341,226,220);

	public override void setData(object obj)
	{
		data = (P_Champion)obj;

		UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, face);

		lbName.text = Util.GetShortID(data.nickname,10);

		lbWinPoint.text = data.score + "";
		lbMyWinPoint.text = data.score + "";

		lbRanking.text = data.rank.ToString();

		Vector3 _v = lbRanking.cachedTransform.localPosition;

		switch(data.rank)
		{
		case 1:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_gold";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			break;
		case 2:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_silver";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			break;
		case 3:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_bronz";
			spHighRankBg.enabled = true;
			lbRanking.text = data.rank.ToString();
			break;
		default:
			spHighRankerIcon.enabled = false;
			spHighRankBg.enabled = false;
			lbRanking.text = Util.getUIText("WORD_RANK", data.rank.ToString());
			break;
		}

		switch(GameDataManager.instance.champLeague)
		{
		case WSDefine.LEAGUE_LEGEND:
			if(data.rank >= 8)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_PLATINUM:

			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_LEGEND; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_MASTER:

			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_PLATINUM; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_GOLD:

			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_MASTER; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;

			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_SILVER:

			if(data.rank >= 11)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_DOWN; // 강등.
				spIcon.MakePixelPerfect();
				spIcon.transform.localPosition = _downgradeIconPosition;
			}
			else if(data.rank <= 5 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_GOLD; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		case WSDefine.LEAGUE_BRONZE:
			if(data.rank <= 7 && data.rank > 3)
			{
				spIcon.enabled = true;
				spIcon.spriteName = ICON_SILVER; // 승격
				spIcon.width = 36;
				spIcon.height = 40;
				spIcon.transform.localPosition = _medalIconPosition;
			}
			else spIcon.enabled = false;
			break;
		}

//	{"memberId":"0","userId":"88148076135900375",
//			"attackWin":0,
//			"attackLose":0,
//			"defenceWin":0,"defenceLose":0,"score":0,"attackRounds":{"R0":"N","R1":"N","R2":"N"},"defenceRounds":{"R0":"N","R1":"N","R2":"N"},

		if(PandoraManager.instance.localUser.userID == data.userId)
		{
			myResultContainer.gameObject.SetActive(true);
			othersResultContainer.gameObject.SetActive(false);
			myResultContainer.setData(data);

			_needUpdateTick = false;
			spBorder.enabled = true;
			lbWinPoint.enabled = false;
			lbMyWinPoint.enabled = true;
			lbName.color = _myColor;
			lbCoolTime.enabled = false;
		}
		else{
			myResultContainer.gameObject.SetActive(false);
			othersResultContainer.gameObject.SetActive(true);
			othersResultContainer.setData(data);
			UIEventListener.Get(othersResultContainer.btnPVP.gameObject).onClick = onRevengeAtkButton;
			UIEventListener.Get(othersResultContainer.btnViewSp.gameObject).onClick = onReplayButton;

			//data.coolTime = 1000;

			refreshDefaultAttackButton();

			/*
			if(checkCanFight()) // data.attackRounds == N,L , 내가 이 유저를 공격하여 패한 경기가 있거나 아직 안한 경기가 있음. 경기 가능. 
			{
				spDefaultAttack.spriteName = "ibtn_vs3_idle";
				refreshDefaultAttackButton();
				lbCoolTime.enabled = true;
			}
			else // 안한 경기가 없고, 모두 Win. 경기 불가. 
			{
				//spDefaultAttack.gameObject.SetActive(false);
				spDefaultAttack.spriteName = "ibtn_vs2_idle";
				refreshDefaultAttackButton();
			}
			*/

			_needUpdateTick = (data.coolTime > 0);
			spBorder.enabled = false;
			lbWinPoint.enabled = true;
			lbMyWinPoint.enabled = false;
			lbName.color = _enemyColor;

		}

		/*
		if(PandoraManager.instance.localUser.userID != data.userId)
		{
			_needUpdateTick = (data.coolTime > 0);

			spBorder.enabled = false;
			resultContainer.SetActive(false);
			progressContainer.SetActive(true);

			btnAttacks[0].setData(data.attackRounds["R0"],false);

			btnDefences[0].setData(data.defenceRounds["R0"],true);
			btnDefences[1].setData(data.defenceRounds["R1"],true);
			btnDefences[2].setData(data.defenceRounds["R2"],true);

			// 좌측 공격 조작 승,패,무 수정.
			// 우측 방어전 조작 수정.

			lbWinPoint.enabled = true;
			lbMyWinPoint.enabled = false;

			lbName.color = _enemyColor;

			btnDefaultAttack.gameObject.SetActive(true);

			if(checkCanFight())
			{
				spDefaultAttack.color = btnDefaultAttack.btn.defaultColor;
				refreshAttackButton(false);
				lbCoolTime.enabled = true;
			}
			else
			{
				spDefaultAttack.color = btnDefaultAttack.btn.disabledColor;
				refreshAttackButton(true);
			}
		}
		else
		{
			_needUpdateTick = false;

			spBorder.enabled = true;
			resultContainer.SetActive(true);
			progressContainer.SetActive(false);

			lbAtkWin.text = "" + data.attackWin;
			lbAtkLose.text = "" + data.attackLose; 
			lbDefWin.text = "" + data.defenceWin; 
			lbDefLose.text = "" + data.defenceLose; 

			lbWinPoint.enabled = false;
			lbMyWinPoint.enabled = true;

			lbName.color = _myColor;

			lbCoolTime.enabled = false;

			btnDefaultAttack.gameObject.SetActive(false);
		}

*/
		_delay = 0;
	}



	private bool _needUpdateTick = false;
	private float _delay = 0.0f;


	void Update()
	{
		if(_needUpdateTick == false) return;
		
		if(_delay > 0)
		{
			_delay -= RealTime.deltaTime;
			return;
		}
		
		_delay = 0.5f;
		
		if(data.coolTime > 0)
		{
			TimeSpan ts = (DateTime.Now - GameDataManager.instance.championshipCoolTime[data.userId]);
			int leftTime = data.coolTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				_needUpdateTick = false;
				refreshDefaultAttackButton();
			}
			else
			{
				lbCoolTime.text = Util.secToHourMinuteSecondString(leftTime); 
			}
		}
	}


	const string COOLTIME_ATK_SPRITE = "ibtn_vs3_idle";
	const string NORMAL_ATK_SPRITE = "ibtn_vs2_idle";

	void refreshDefaultAttackButton()
	{

		if (numOfRemainRound() == 0){
			spDefaultAttack.gameObject.SetActive(false);
			lbCoolTime.gameObject.SetActive(false);
			return;
		}
		else{
			spDefaultAttack.gameObject.SetActive(true);
		}

		//if((data.attackRounds["R0"].result != "N" && data.coolTime > 0 && isSetDefault == false) || (numOfWinRound() == 7 && data.coolTime > 0 && isSetDefault == true))
		if(data.attackRounds["R0"].result != "N" && data.coolTime > 0)
		{

			DateTime dt = DateTime.Now;
			
			GameDataManager.instance.championshipCoolTime.TryGetValue(data.userId, out dt);
			
			TimeSpan ts = (DateTime.Now - dt);
			int leftTime = data.coolTime - (int)ts.TotalSeconds;
			
			if(leftTime > 0)
			{
				lbCoolTime.text = Util.secToHourMinuteSecondString(leftTime); 
				lbCoolTime.gameObject.SetActive(true);
				lbCoolTime.enabled = true;
				spDefaultAttack.spriteName = COOLTIME_ATK_SPRITE;
				return;
			}
		}

		spDefaultAttack.spriteName = NORMAL_ATK_SPRITE;
		lbCoolTime.gameObject.SetActive(false);
	}




}

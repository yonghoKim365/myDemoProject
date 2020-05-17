using UnityEngine;
using System.Collections.Generic;

public class UIChampionshipReplayPanel : UIPopupBase
{

	public UIChampionshipReplayList list;


	public UILabel lbAttackWin;
	public UILabel lbAttackLose;
	public UILabel lbDefenceWin;
	public UILabel lbDefenceLose;
	public UILabel lbName;
	public UILabel lbScore;
	public UILabel lbRank;

	public GameObject tabAttack;
	public GameObject tabDefence;

	public PhotoDownLoader face;
	public UISprite spEmptyFace;

	public P_Champion data;
	public int tabKind = 0; // 0 == atk, 1 == def

	public UISprite spHighRankerIcon;

	protected override void awakeInit ()
	{
		UIEventListener.Get(tabAttack.gameObject).onClick = onClickAttackTab;
		UIEventListener.Get(tabDefence.gameObject).onClick = onClickDefenceTab;
	}

	public void setData(P_Champion _data)
	{
		data = _data;
		//UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, face);


	}
	public override void show ()
	{

		base.show ();
		list.clear();
		tabKind = 0;

		UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, face);
		/*
		if(data.showPhoto == WSDefine.TRUE)
		{
			Debug.Log("10. data.imageUrl :"+data.imageUrl);
			
			face.down(data.imageUrl);
		}
		*/

		refresh();

	}

	public void refresh()
	{
		if(GameManager.me.uiManager.uiMenu.currentPanel != UIMenu.WORLD_MAP || gameObject.activeSelf == false) return;

		if (GameDataManager.instance.championshipData.champions.ContainsKey(data.userId)){
			data = GameDataManager.instance.championshipData.champions[data.userId];
		}

		setTabAndRefreshList(tabKind);

		lbAttackWin.text = data.attackWin.ToString();
		lbAttackLose.text = data.attackLose.ToString();
		lbDefenceWin.text = data.defenceWin.ToString();
		lbDefenceLose.text = data.defenceLose.ToString();
		
		if(PandoraManager.instance.localUser.userID != data.userId){
			lbAttackWin.text = UIChampionshipReplayPanel.getWinCnt(data.attackRounds).ToString();
			lbAttackLose.text = UIChampionshipReplayPanel.getLoseCnt(data.attackRounds).ToString();
			lbDefenceWin.text = UIChampionshipReplayPanel.getWinCnt(data.defenceRounds).ToString();
			lbDefenceLose.text = UIChampionshipReplayPanel.getLoseCnt(data.defenceRounds).ToString();
		}
		
		lbName.text = data.nickname;
		lbScore.text = data.score.ToString();
		lbRank.text = data.rank.ToString();
		
		switch(data.rank)
		{
		case 1:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_gold";
			lbRank.text = data.rank.ToString();
			break;
		case 2:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_silver";
			lbRank.text = data.rank.ToString();
			break;
		case 3:
			spHighRankerIcon.enabled = true;
			spHighRankerIcon.spriteName = "img_rank_bronz";
			lbRank.text = data.rank.ToString();
			break;
		default:
			spHighRankerIcon.enabled = false;
			lbRank.text = Util.getUIText("WORD_RANK", data.rank.ToString());
			break;
		}

		System.GC.Collect();
		
	}

	float _delayTime = 0;
	void Update()
	{
		if(GameDataManager.instance.championshipData == null) return;
		
	}

	void onClickAttackTab(GameObject go){
		if (tabKind == 0)return;

		setTabAndRefreshList(0);
	}

	void onClickDefenceTab(GameObject go){
		if (tabKind == 1)return;

		setTabAndRefreshList(1);
	}

	void setTabAndRefreshList(int newTab){

		tabKind = newTab;

		if (tabKind == 0){
			tabAttack.transform.GetComponent<UISprite>().spriteName = "ibtn_tap_attack_on";
			tabDefence.transform.GetComponent<UISprite>().spriteName = "ibtn_tap_defense_off";
		}
		else{
			tabAttack.transform.GetComponent<UISprite>().spriteName = "ibtn_tap_attack_off";
			tabDefence.transform.GetComponent<UISprite>().spriteName = "ibtn_tap_defense_on";
		}

		list.clear();
		if (tabKind == 0){
			list.SetData(data.attackRounds, data.revengeCoolTime);
		}
		else{
			list.SetData(data.defenceRounds, data.revengeCoolTime);
		}
		list.draw();
	}


	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		GameManager.me.uiManager.uiMenu.uiWorldMap.refreshFriendPhoto();
	}

	static public bool isPvpBattleRecordExist(Dictionary<string, P_ChampionResult> AttackRounds,Dictionary<string, P_ChampionResult> DefenceRounds){
		int aw,al,dw,dl;
		aw = al = dw = dl = 0;
		for(int i=0;i<UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
			if(AttackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "W"){
				aw++;
			}
			else if(AttackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "L"){
				al++;
			}
		}
		for(int i=0;i<UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
			if(DefenceRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "W"){
				dw++;
			}
			else if(DefenceRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "L"){
				dl++;
			}
		}

		if (aw + al + dw + dl == 0)return false;

		return true;

	}

	static public int getWinCnt(Dictionary<string, P_ChampionResult> rounds){
		int winCnt = 0;
		for(int i=0;i<UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
			if(rounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "W")
			{
				winCnt++;
			}
		}
		return winCnt;
	}
	
	static public int getLoseCnt(Dictionary<string, P_ChampionResult> rounds){
		int LoseCnt = 0;
		for(int i=0;i<UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
			if(rounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "L")
			{
				LoseCnt++;
			}
		}
		return LoseCnt;
	}

	public static int getWinPoint(P_ChampionResult _data){
		int wp = 0;
		if (_data.result == "W"){
			if (_data.attackHeroState.ContainsKey(Character.CHLOE)){
				if (_data.attackHeroState[Character.CHLOE] == 1){
					wp++;
				}
			}
			if (_data.attackHeroState.ContainsKey(Character.KILEY)){
				if (_data.attackHeroState[Character.KILEY] == 1){
					wp++;
				}
			}
			if (_data.attackHeroState.ContainsKey(Character.LEO)){
				if (_data.attackHeroState[Character.LEO] == 1){
					wp++;
				}
			}
		}
		return wp;
	}
}
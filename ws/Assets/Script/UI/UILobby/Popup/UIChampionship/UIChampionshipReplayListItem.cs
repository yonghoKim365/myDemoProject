using UnityEngine;
using System.Collections.Generic;
using System;

public class UIChampionshipReplayListItem : UIListGridItemPanelBase {

	public GameObject 	btnPVP;
	public GameObject 	btnViewSp;

	public UISprite[] attackHero;
	public GameObject[] attackHero_Death;

	public UILabel lbBattleNum;
	public UILabel lbCoolTime;

	public UISprite winIcon;
	public UISprite atkOrDefIcon;
	P_ChampionResult data;

	int remainCoolTime;
	protected override void initAwake ()
	{
		UIEventListener.Get(btnPVP).onClick = onClickBattle;
		UIEventListener.Get(btnViewSp).onClick = onClickReplay;
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

		if(remainCoolTime > 0)
		{
			TimeSpan ts = (DateTime.Now - GameDataManager.instance.championShipCheckTime);
			remainCoolTime = GameManager.me.uiManager.popupChampionshipReplayPanel.data.revengeCoolTime - (int)ts.TotalSeconds;


			if(remainCoolTime <= 0)
			{
				_needUpdateTick = false;
				if (btnPVP.activeInHierarchy){
					btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle";
					lbCoolTime.gameObject.SetActive(false);
				}
			}
			else
			{
				lbCoolTime.text = Util.secToHourMinuteSecondString(remainCoolTime); 
			}
		}


	}

	public override void setPhotoLoad()
	{
		//if(data == null) return;		
	}	
	

	public override void setData(object obj)
	{
		data = (P_ChampionResult)obj;

		initItem();

	}

	void onClickBattle(GameObject go)
	{
		if (remainCoolTime > 0)return;

		GameManager.me.uiManager.popupChampionshipAttack.show();
		GameManager.me.uiManager.popupChampionshipAttack.setData(GameManager.me.uiManager.popupChampionshipReplayPanel.data, true, index);
	}

	void onClickReplay(GameObject go)
	{
#if UNITY_IPHONE
		UISystemPopup.open (UISystemPopup.PopupType.Default, Util.getUIText("REPLAY_TEMPPOPUP"), onClickReplay2);
#else
		bool isAttackGame = true;
		if (GameManager.me.uiManager.popupChampionshipReplayPanel.tabKind == 1){
			isAttackGame = false;
		}
		
		EpiServer.instance.sendGetReplay(isAttackGame,GameManager.me.uiManager.popupChampionshipReplayPanel.data.userId, "R"+index.ToString());
#endif
	}

#if UNITY_IPHONE
	void onClickReplay2()
	{
        bool isAttackGame = true;
        if (GameManager.me.uiManager.popupChampionshipReplayPanel.tabKind == 1){
			isAttackGame = false;
		}
		
		EpiServer.instance.sendGetReplay(isAttackGame,GameManager.me.uiManager.popupChampionshipReplayPanel.data.userId, "R"+index.ToString());
	}
#endif

	void initItem(){
		TimeSpan ts = (DateTime.Now - GameDataManager.instance.championShipCheckTime);
		remainCoolTime = GameManager.me.uiManager.popupChampionshipReplayPanel.data.revengeCoolTime - (int)ts.TotalSeconds;
		
		if (remainCoolTime > 0){
			_needUpdateTick = true;
		}

		btnViewSp.SetActive(true);

		lbBattleNum.text = (this.index+1).ToString() + "차전";
		if (data.result == "W"){
			atkOrDefIcon.spriteName = "ibtn_mark_winidle";
			// replay - o
			// 1 point == re-battle o , 2 point == rebattle x

			if (GameManager.me.uiManager.popupChampionshipReplayPanel.tabKind == 1){ // defence tab
				btnPVP.SetActive(false);
				_needUpdateTick = false;
			}
			else{
				if (UIChampionshipReplayPanel.getWinPoint(data) == 2){
					btnPVP.SetActive(false);
				}
				else{
					if (remainCoolTime > 0){
						setPvpCoolTime(remainCoolTime);
					}
					else{
						btnPVP.SetActive(true);
						btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle";
						lbCoolTime.gameObject.SetActive(false);
					}
				}
			}
		}
		else if (data.result == "L"){
			atkOrDefIcon.spriteName = "ibtn_mark_loseidle";
			// replay - o
			// re-battle - o
			if (GameManager.me.uiManager.popupChampionshipReplayPanel.tabKind == 1){ // defence tab
				btnPVP.SetActive(false);
				_needUpdateTick = false;
			}
			else{
				btnPVP.SetActive(true);
				if (remainCoolTime > 0){
					setPvpCoolTime(remainCoolTime);
				}
				else{
					btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle";
					lbCoolTime.gameObject.SetActive(false);
				}
			}
		}
		else if (data.result == "N"){
			atkOrDefIcon.spriteName = "ibtn_mark_emptyidle";
			// replay - x
			// re-battle - x
			btnPVP.SetActive(false);
			btnViewSp.SetActive(false);
		}

		int heroCnt = 0;

		attackHero[0].gameObject.SetActive(false);
		attackHero[1].gameObject.SetActive(false);

		if (data.attackHeroState == null){
			attackHero_Death[0].SetActive(false);
			attackHero_Death[1].SetActive(false);
		}
		else{

			foreach(KeyValuePair<string, int> kv in data.attackHeroState)
			{
				if (kv.Key == null){
					attackHero[heroCnt].gameObject.SetActive(false);
					heroCnt++;
					continue;
				}

				attackHero[heroCnt].gameObject.SetActive(true);

				switch(kv.Key){
				case "CHLOE":
					attackHero[heroCnt].spriteName = "img_hero_03";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				case "KILEY":
					attackHero[heroCnt].spriteName = "img_hero_02";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				case "LEO":
					attackHero[heroCnt].spriteName = "img_hero_01";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				}
				heroCnt++;
			}
		}
		heroCnt = 2;

		attackHero[2].gameObject.SetActive(false);
		attackHero[3].gameObject.SetActive(false);

		if (data.defenceHeroState == null){
			attackHero_Death[2].SetActive(false);
			attackHero_Death[3].SetActive(false);
		}
		else{
			foreach(KeyValuePair<string, int> kv in data.defenceHeroState)
			{
				if (kv.Key == null){
					attackHero[heroCnt].gameObject.SetActive(false);
					heroCnt++;
					continue;
				}

				attackHero[heroCnt].gameObject.SetActive(true);

				switch(kv.Key){
				case "CHLOE":
					attackHero[heroCnt].spriteName = "img_hero_03";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				case "KILEY":
					attackHero[heroCnt].spriteName = "img_hero_02";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				case "LEO":
					attackHero[heroCnt].spriteName = "img_hero_01";
					if (kv.Value == 1){
						attackHero_Death[heroCnt].SetActive(false);
					}
					else if (kv.Value == 0){
						attackHero_Death[heroCnt].SetActive(true);
					}
					break;
				}
				heroCnt++;
			}
		}

		if (GameManager.me.uiManager.popupChampionshipReplayPanel.tabKind == 1){ // defence tab
			btnPVP.SetActive(false);
			_needUpdateTick = false;
		}
	}

	void setPvpCoolTime(int ct){
		btnPVP.SetActive(true);
		btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle_off";
		lbCoolTime.gameObject.SetActive(true);
		lbCoolTime.text = Util.secToHourMinuteSecondString(ct); 
	}
	

}

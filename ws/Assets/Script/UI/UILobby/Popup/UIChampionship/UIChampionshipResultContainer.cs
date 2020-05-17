using UnityEngine;
using System.Collections.Generic;
using System;

public class UIChampionshipResultContainer : MonoBehaviour {


	public UILabel lbAttackWin;
	public UILabel lbAttackLose;
	public UILabel lbDefenceWin;
	public UILabel lbDefenceLose;

	public GameObject 	btnPVP;
	public GameObject 	btnViewSp;
	//public UILabel 		lbWaitTime;
	public UILabel 		lbRevengeCoolTime;
	P_Champion data;

	
	int remainCoolTime;
	private bool _needUpdateTick = false;
	private float _delay = 0.0f;

	void Update(){

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
			remainCoolTime = data.revengeCoolTime - (int)ts.TotalSeconds;
			
			//Debug.Log(" remainCoolTime :"+remainCoolTime);
			
			if(remainCoolTime <= 0)
			{
				_needUpdateTick = false;
				if (btnPVP.activeInHierarchy){
					btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle";
					lbRevengeCoolTime.gameObject.SetActive(false);
				}
			}
			else
			{
				lbRevengeCoolTime.text = Util.secToHourMinuteSecondString(remainCoolTime); 
			}
		}
	}

	int attackWin,attackLose, defenceWin, defenceLose;

	public void setData(P_Champion _data){

		data = _data;

		remainCoolTime = 0;
		remainCoolTime = data.revengeCoolTime;

		attackWin = data.attackWin;
		attackLose = data.attackLose;
		defenceWin = data.defenceWin;
		defenceLose = data.defenceLose;

		if(PandoraManager.instance.localUser.userID != data.userId){
			attackWin = UIChampionshipReplayPanel.getWinCnt(data.attackRounds);
			attackLose = UIChampionshipReplayPanel.getLoseCnt(data.attackRounds);
			defenceWin = UIChampionshipReplayPanel.getWinCnt(data.defenceRounds);
			defenceLose = UIChampionshipReplayPanel.getLoseCnt(data.defenceRounds);
		}

		lbAttackWin.text = attackWin.ToString();
		lbAttackLose.text = attackLose.ToString();
		lbDefenceWin.text = defenceWin.ToString();
		lbDefenceLose.text = defenceLose.ToString();


		//if (btnPVP != null && btnViewSp != null){
		if(PandoraManager.instance.localUser.userID != data.userId){

				// 공격 성공 + 공격실패 == 0 // 재도전 불가
				// 1점 공격 / 재도전 가능, but 쿨타임 있다면 불가
				// 2점공격 / 재도전 불가


			if (attackLose + attackWin + defenceWin + defenceLose == 0){
				btnViewSp.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_replay_idle_off";
			}
			else{
				btnViewSp.transform.GetComponentInChildren<UISprite>().spriteName = "ibnt_replay_idle";
			}

			if (attackWin + attackLose == 0){
				btnPVP.SetActive(false);
			}
			else{
				if (getNumOfOnePointEarnedRound(data.attackRounds) > 0 || attackLose > 0){
					if (data.revengeCoolTime > 0){
						// disable re-battle
						btnPVP.SetActive(true);
						btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle_off";
						
						lbRevengeCoolTime.gameObject.SetActive(true);
						lbRevengeCoolTime.text = Util.secToHourMinuteSecondString(data.revengeCoolTime);
					}
					else{
						// enable re-battle
						btnPVP.SetActive(true);
						btnPVP.transform.GetComponentInChildren<UISprite>().spriteName = "ibtn_vs_idle";
						lbRevengeCoolTime.gameObject.SetActive(false);
					}
				}
				else{
					// disable re-battle
					btnPVP.SetActive(false);
				}
			}

			if (data.revengeCoolTime > 0){
				_needUpdateTick = true;
			}

		}
	}
	public static bool isEnableReBattle(P_Champion _data){
		int aw, al, dw,dl;
		aw = al = dw = dl = 0;
		aw = _data.attackWin;
		al = _data.attackLose;
		dw = _data.defenceWin;
		dl = _data.defenceLose;
		
		if(PandoraManager.instance.localUser.userID != _data.userId){
			aw = UIChampionshipReplayPanel.getWinCnt(_data.attackRounds);
			al = UIChampionshipReplayPanel.getLoseCnt(_data.attackRounds);
			dw = UIChampionshipReplayPanel.getWinCnt(_data.defenceRounds);
			dl = UIChampionshipReplayPanel.getLoseCnt(_data.defenceRounds);
		}

		if (aw + al == 0){
			return false;
		}
		else{
			if (getNumOfOnePointEarnedRound(_data.attackRounds) > 0 || al > 0){
				return true;
			}
			else{
				return false;
			}
		}
		return true;
	}
	
	// 공격하여 승리한  pvp중에서 1점을 얻었던 라운드수를 리턴한다.
	public static int getNumOfOnePointEarnedRound(Dictionary<string, P_ChampionResult> _attackRounds){
		int onePointGameCnt = 0;
		for(int i=0;i<UIChampionshipListSlotPanel.ROUND_IDS.Length;i++){
			if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].result == "W"){
				int winPnt =0;

				if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState.ContainsKey(Character.CHLOE)){
					if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState[Character.CHLOE] == 1){
						winPnt++;
					}
				}
				if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState.ContainsKey(Character.KILEY)){
					if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState[Character.KILEY] == 1){
						winPnt++;
					}
				}
				if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState.ContainsKey(Character.LEO)){
					if (_attackRounds[UIChampionshipListSlotPanel.ROUND_IDS[i]].attackHeroState[Character.LEO] == 1){
						winPnt++;
					}
				}

				if (winPnt == 1){
					onePointGameCnt++;
				}
			}
		}
		return onePointGameCnt;
	}
}

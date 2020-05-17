using UnityEngine;
using System.Collections;
using System;

public class UIPopupChampionshipAttack : UIPopupBase {

	public PhotoDownLoader playerFace;
	public PhotoDownLoader pvpFace;

	public GameObject goReMatchContainer;

	public UILabel lbPlayerName, lbPVPName;
	 
	public UILabel lbTitle, lbMatchWord, lbMatchDescription;

	public UILabel lbLeftTime, lbEnergy, lbStartPrice, lbRubyExploreWord, lbRubyUsingNum, lbResetWord;

	public UIButton btnAttack, btnRematch;

	public UISprite spEmptyFace; // spStartPriceType

	protected override void awakeInit ()
	{
		UIEventListener.Get(btnAttack.gameObject).onClick = onClickAttack;
		UIEventListener.Get(btnRematch.gameObject).onClick = onClickAttack;

//		GameDataManager.energyDispatcher -= updateEnergy;
//		GameDataManager.energyDispatcher += updateEnergy;

//		GameDataManager.chargeTimeDispatcher -= updateNextEnergy;
//		GameDataManager.chargeTimeDispatcher += updateNextEnergy;
	}

	public void onClickAttack(GameObject go)
	{
//		int ruby = GameDataManager.instance.ruby;
//		int energy = GameDataManager.instance.energy;

		int needRuby =  GameDataManager.instance.championshipData.reattackRuby;

		if(_isRematch)
		{
			if(UIManager.checkRubyPopup(needRuby) == false)
			{
				return;
			}
		}

		GameManager.me.uiManager.popupChampionshipResult.enemyId = _data.userId;
		GameManager.me.uiManager.popupChampionshipResult.matchNumber = matchNumber;

		EpiServer.instance.sendPlayPVPData(_data.userId,_round, "AI_PVP1");
	}





//	public void startTutorialAttack()
//	{
//		GameManager.me.uiManager.popupChampionshipResult.enemyId = _data.userId;
//		GameManager.me.uiManager.popupChampionshipResult.matchNumber = matchNumber;
//
//		EpiServer.instance.sendPlayPVPData(_data.userId, _round, "AI_PVP1", TutorialManager.instance.nowTutorialId);
//	}






	public void onReceivePVPData()
	{
		hide();

		GameManager.me.uiManager.showLoading();

		GameManager.me.startGame();
	}


	public override void show ()
	{
		base.show ();
//		updateEnergy();

//		TutorialManager.instance.check("T28");
	}

	P_Champion _data;
	bool _isRematch = false;

	string _round;
	int matchNumber;

	public void setData(P_Champion data,  bool isRematch, int slotIndex)
	{
		_data = data;
		_isRematch = isRematch;

		UIPlay.playerImageUrl = PandoraManager.instance.localUser.image_url;
		UIPlay.pvpImageUrl = (data.imageUrl == null)?"":data.imageUrl;

		playerFace.init(PandoraManager.instance.localUser.image_url);
		playerFace.down(PandoraManager.instance.localUser.image_url);

		UIManager.setPlayerPhoto(data.showPhoto, data.imageUrl, spEmptyFace, pvpFace, true);

		// 같은 리그니까...
		UIPlay.playerLeagueGrade = GameDataManager.instance.champLeague;
		UIPlay.pvpleagueGrade = GameDataManager.instance.champLeague;

		lbPlayerName.text = Util.GetShortID( GameDataManager.instance.name , 10);
		lbPVPName.text = Util.GetShortID( data.nickname , 10);
		PlayerPrefs.SetString("PVPNAME",lbPVPName.text);
		GameManager.me.uiManager.popupChampionshipResult.pvpName = lbPVPName.text;

		switch(slotIndex)
		{
		case 0: _round = "R0"; matchNumber = 1; break;
		case 1: _round = "R1"; matchNumber = 2; break;
		case 2: _round = "R2"; matchNumber = 3; break; 
		case 3: _round = "R3"; matchNumber = 4; break;
		case 4: _round = "R4"; matchNumber = 5; break;
		case 5: _round = "R5"; matchNumber = 6; break; 
		case 6: _round = "R6"; matchNumber = 7; break; 
		}

//		if(isRematch)
//		{
//			spStartPriceType.spriteName = "img_icn_cash";
//		}
//		else
//		{
//			spStartPriceType.spriteName = "img_icn_energe";
//		}

		lbTitle.text = (1+slotIndex) + "차전";

		goReMatchContainer.SetActive(isRematch);

//		updateEnergy();
//		updateNextEnergy("-");

		if(isRematch)
		{
			lbStartPrice.text = GameDataManager.instance.championshipData.reattackRuby + "";
//			spStartPriceType.gameObject.SetActive(true);
			//spStartPriceType.spriteName = "img_icn_cash";
			btnRematch.gameObject.SetActive(true);
			btnAttack.gameObject.SetActive(false);
		}
		else
		{
			btnRematch.gameObject.SetActive(false);
			btnAttack.gameObject.SetActive(true);
		}

	}

//	void updateEnergy()
//	{
//		lbEnergy.text = GameDataManager.instance.energy.ToString();
//	}
//
//	void updateNextEnergy(string timeStr)
//	{
//		if(gameObject.activeInHierarchy == false) return;
//		lbLeftTime.text = timeStr;
//	}

}

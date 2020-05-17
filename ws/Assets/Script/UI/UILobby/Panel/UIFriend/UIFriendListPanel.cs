using UnityEngine;
using System.Collections;
using System;

public class UIFriendListPanel : UIListGridItemPanelBase {
	
	public UIButton btnInfo, btnPVP, btnPoint;

	public UILabel lbUserLevel, lbName, lbFriendLevel, lbFriendExpPer, lbFBWaitTime;
	
	public PhotoDownLoader face;
	
	public UISprite spGradeIcon, spFriendPointProgressBar, spBtnPointBackground;

	private bool _needUpdateTick = false;
	private float _delay = 0.0f;

	public UIDragScrollView scrollView;
	
	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnInfo.gameObject).onClick = onClickInfo;
		UIEventListener.Get(btnPVP.gameObject).onClick = onClickPVP;
		UIEventListener.Get(btnPoint.gameObject).onClick = onClickPoint;

		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.uiMenu.uiFriend.list.panel;
	}


	void OnDestroy()
	{
		btnInfo = null; 
		btnPVP = null;
		btnPoint = null;

		scrollView = null;
		data = null;
		spGradeIcon = null;
		spFriendPointProgressBar = null;
		spBtnPointBackground = null;
		lbUserLevel = null;
		lbFriendLevel = null;
		lbFriendExpPer = null;
		lbFBWaitTime = null;

	}


	void onClickInfo(GameObject go)
	{
		if(GameManager.me.uiManager.uiMenu.uiFriend.slotMachine.isReady == false) return;

		EpiServer.instance.sendGetFriendDetail(data.userId);
	}

	void onClickPVP(GameObject go)
	{
		if(GameManager.me.uiManager.uiMenu.uiFriend.slotMachine.isReady == false) return;

		int leftNum = GameDataManager.instance.friendlyPVPMax;

		if(GameDataManager.instance.friendlyPVPIds != null)
		{
			leftNum = GameDataManager.instance.friendlyPVPMax - GameDataManager.instance.friendlyPVPIds.Length;

			if(leftNum <= 0)
			{
				UISystemPopup.open("오늘의 친선경기 횟수를 초과했습니다.");
				return;
			}
			else
			{
				foreach(string id in GameDataManager.instance.friendlyPVPIds)
				{
					if(id == data.userId)
					{
						UISystemPopup.open("오늘 이미 대전한 친구입니다.");
						return;
					}
				}
			}
		}

		GameManager.me.uiManager.popupFriendlyPVPAttack.show();
		GameManager.me.uiManager.popupFriendlyPVPAttack.setData(data.userId, leftNum, data.pvpReward);
	}

	void onClickPoint(GameObject go)
	{
		GameManager.me.uiManager.uiMenu.uiFriend.onClickPointButton(this, pointType);
	}

	
	public override void setPhotoLoad()
	{
		if(data == null) return;
		face.down(epi.GAME_DATA.appFriendDic[data.userId].image_url);
	}	

	public P_FriendData data;

	public enum PointButtonType
	{
		None, Get, Send
	}

	PointButtonType pointType = PointButtonType.None;

	public override void setData(object obj)
	{
		//int rank = _idx+1;
		P_FriendData d = (P_FriendData)obj;

		data = d;

		face.init(epi.GAME_DATA.appFriendDic[data.userId].image_url);

		Util.stringBuilder.Length = 0;

		Util.stringBuilder.Append(epi.GAME_DATA.appFriendDic[data.userId].f_Nick);

		if(data.nickname != data.userId)
		{
			Util.stringBuilder.Append(" (");
			Util.stringBuilder.Append(data.nickname);
			Util.stringBuilder.Append(")");
		}

		lbName.text = Util.GetShortID( Util.stringBuilder.ToString() , 18 );

		Util.stringBuilder.Length = 0;

		refresh();
	}


	void Update()
	{
		if(_needUpdateTick == false) return;

		if(data!=null)
		{
			if(_delay > 0)
			{
				_delay -= RealTime.deltaTime;
				return;
			}

			_delay = 0.5f;

			if(data.fpWaitingTime > 0)
			{
				TimeSpan ts = (DateTime.Now - GameDataManager.instance.friendPointRefreshTimer[data.userId]);
				int leftTime = data.fpWaitingTime - (int)ts.TotalSeconds;
				
				if(leftTime < 0)
				{
					_needUpdateTick = false;
					refreshFriendPointButton();
				}
				else
				{
					lbFBWaitTime.text = Util.secToHourMinuteSecondString(leftTime); 
				}
			}
		}
	}


	bool canFight()
	{
		int leftNum = GameDataManager.instance.friendlyPVPMax;
		
		if(GameDataManager.instance.friendlyPVPIds != null)
		{
			leftNum = GameDataManager.instance.friendlyPVPMax - GameDataManager.instance.friendlyPVPIds.Length;
			
			if(leftNum <= 0)
			{
				return false;
			}
			else
			{
				foreach(string id in GameDataManager.instance.friendlyPVPIds)
				{
					if(id == data.userId)
					{
						return false;
					}
				}
			}
		}

		return true;
	}



	public void refresh()
	{
//		lbUserLevel.text = "l"+data.level;
		lbUserLevel.enabled = false;
		lbFriendLevel.text = data.fLevel.ToString();

		if(data.fLevel >= 5)
		{
			lbFriendExpPer.text = "MAX";
		}
		else
		{
			lbFriendExpPer.text = data.fExpGauge+"%";
		}

		switch(data.league)
		{
		case 1:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_BRONZ;
			break;
		case 2:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_SILVER;
			break;
		case 3:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_GOLD;
			break;
		case 4:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_MASTER;
			break;
		case 5:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_PLATINUM;
			break;
		case 6:
			spGradeIcon.spriteName = UIChampionshipListSlotPanel.ICON_LEGEND;
			break;
		}

		btnPVP.isEnabled = canFight();

		spFriendPointProgressBar.fillAmount = (float)data.fExpGauge * 0.01f;
		
		_needUpdateTick = (data.fpWaitingTime > 0);
		
		_delay = 0;
		
		refreshFriendPointButton();
	}


	void refreshFriendPointButton()
	{
		if(data.receivedFP == WSDefine.YES)
		{
			spBtnPointBackground.spriteName = "ibtn_get_spidle";
			lbFBWaitTime.gameObject.SetActive(false);
			pointType = PointButtonType.Get;
			btnPoint.isEnabled = true;
		}
		else
		{
			if(data.fpWaitingTime > 0)
			{
				DateTime dt = DateTime.Now;

				GameDataManager.instance.friendPointRefreshTimer.TryGetValue(data.userId, out dt);

				TimeSpan ts = (DateTime.Now - dt);
				int leftTime = data.fpWaitingTime - (int)ts.TotalSeconds;
				
				if(leftTime < 0)
				{
					_needUpdateTick = false;
					spBtnPointBackground.spriteName = "ibtn_view_spidle";
					lbFBWaitTime.gameObject.SetActive(false);
					pointType = PointButtonType.Send;
					btnPoint.isEnabled = true;
				}
				else
				{
					lbFBWaitTime.text = Util.secToHourMinuteSecondString(leftTime); 
					spBtnPointBackground.spriteName = "ibtn_send_spidle";
					lbFBWaitTime.gameObject.SetActive(true);
					pointType = PointButtonType.None;
					btnPoint.isEnabled = false;
				}
			}
			else
			{
				spBtnPointBackground.spriteName = "ibtn_view_spidle";
				lbFBWaitTime.gameObject.SetActive(false);		
				pointType = PointButtonType.Send;
				btnPoint.isEnabled = true;
			}
		}
	}


	/*
	//한글나오나//

	public void setData(object Obj)
	{
		if(type == "GIFT"){
		}else if(type=="INVITE"){
			string tmpName = UIManager.GetShortID(data.nickname);
			face.init(data.image_url);
			face.down(data.image_url);
			//nameLabel.text = string.Format(TextDatas.POPUP_INVITE_PANEL,tmpName);
			string str = epi.GAMEdata.inviteFriendText;
			str = str.Replace("{to}",tmpName);
			nameLabel.text = str;
			nameLabel.RenderText();
			
			UIEventListener.Get(shoesBtn.gameObject).onClick -= OnClickSendShoes;
			UIEventListener.Get(shoesBtn.gameObject).onClick += OnClickSendShoes;
			
			if(data.isInvited==false){
				shoesBtn.isEnabled = true;
			}else{
				shoesBtn.isEnabled = false;
			}
		}else if(type=="LASTRANKING"){
			string tmpName = UIManager.GetShortID(data.nickname);
			if(tmpName.Length>7){
				tmpName = tmpName.Substring(0,7)+"...";
			}
			
			//nameLabel.text = data.userID;
			
			face.init(data.image_url);
			
			scoreLabel.text = UIManager.GetCommaScore(data.myLastWeekScore);
			nameLabel.text = tmpName;
			nameLabel.RenderText();
			
			if(rank==1){
				rank1.gameObject.SetActiveRecursively(true);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else if(rank==2){
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(true);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else if(rank==3){
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(true);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else{
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(true);
				rankLabel.text = rank.ToString();
			}
			
			
			
			pnlWait.gameObject.SetActiveRecursively(false);
			
			if(GAMEdata.localUser.userID == data.userID){
				myBackPanel.gameObject.SetActiveRecursively(true);
				defaultBackPanel.gameObject.SetActiveRecursively(false);
				
				shoesBtn.gameObject.SetActiveRecursively(false);
				shoesCantBtn.gameObject.SetActiveRecursively(false);
				
			}else{
				myBackPanel.gameObject.SetActiveRecursively(false);
				defaultBackPanel.gameObject.SetActiveRecursively(true);
				
				shoesBtn.gameObject.SetActiveRecursively(false);
				shoesCantBtn.gameObject.SetActiveRecursively(false);
			}
			
			charIcon.gameObject.SetActiveRecursively(false);
			if(data.myLastWeekOptsList[1]!="" && data.myLastWeekOptsList[1]!=null){
				//vehicle
				switch(data.myLastWeekOptsList[1]){
				case PlayerInfo.TYPE_VEHICLE_FENRIR:
					charIcon.spriteName = "ranking_P4";
					break;
				case PlayerInfo.TYPE_VEHICLE_GRIFFIN:
					charIcon.spriteName = "ranking_P1";
					break;
				case PlayerInfo.TYPE_VEHICLE_UNICON:
					charIcon.spriteName = "ranking_P2";
					break;
				case PlayerInfo.TYPE_VEHICLE_WHITETIGER:
					charIcon.spriteName = "ranking_P3";
					break;
				default:
					charIcon.gameObject.SetActiveRecursively(false);
					break;
				}
			}else{
				//character
				switch(data.myLastWeekOptsList[0]){
				case PlayerInfo.TYPE_CHASER:
					charIcon.spriteName = "ranking_C2";
					break;
				case PlayerInfo.TYPE_RISKTAKER:
					charIcon.spriteName = "ranking_C3";
					break;
				case PlayerInfo.TYPE_KYLIE:
					charIcon.spriteName = "ranking_C4";
					break;
				case PlayerInfo.TYPE_TREASUREHUNTER:
					charIcon.spriteName = "ranking_C1";
					break;
				default:
					charIcon.gameObject.SetActiveRecursively(false);
					break;
				}
			}
		}else if(type=="RANKING"){
			//string tmpName = data.nickname;
			string tmpName = UIManager.GetShortID(data.nickname);
			if(tmpName.Length>7){
				tmpName = tmpName.Substring(0,7)+"...";
			}
			//face.url = data.image_url;
			face.init(data.image_url);
			//face.ready();
			scoreLabel.text = UIManager.GetCommaScore(data.weekScore);
			nameLabel.text = tmpName;
			nameLabel.RenderText();
			
			if(rank==1){
				rank1.gameObject.SetActiveRecursively(true);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else if(rank==2){
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(true);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else if(rank==3){
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(true);
				rankLabel.gameObject.SetActiveRecursively(false);
			}else{
				rank1.gameObject.SetActiveRecursively(false);
				rank2.gameObject.SetActiveRecursively(false);
				rank3.gameObject.SetActiveRecursively(false);
				rankLabel.gameObject.SetActiveRecursively(true);
				rankLabel.text = rank.ToString();
			}
			
			if(data.weekScore>0){
				UIEventListener.Get(btnPreview.gameObject).onClick -= OnClickPreview;
				UIEventListener.Get(btnPreview.gameObject).onClick += OnClickPreview;
				btnPreview.gameObject.SetActiveRecursively(true);
			}else{
				btnPreview.gameObject.SetActiveRecursively(false);
			}
			
			pnlWait.gameObject.SetActiveRecursively(false);
			
			if(GAMEdata.localUser.userID == data.userID){
				myBackPanel.gameObject.SetActiveRecursively(true);
				defaultBackPanel.gameObject.SetActiveRecursively(false);
				PlayerInfo.GetInstance().rank = rank;
				
				shoesBtn.gameObject.SetActiveRecursively(false);
				
				if(data.isBlockMessage==true || data.isKakaoBlock==true){
					shoesBtn.gameObject.SetActiveRecursively(false);
					shoesCantBtn.gameObject.SetActiveRecursively(true);
					
					UIEventListener.Get(shoesCantBtn.gameObject).onClick -= OnClickDontSendShoes;
					UIEventListener.Get(shoesCantBtn.gameObject).onClick += OnClickDontSendShoes;
				}else{
					shoesBtnSprite.spriteName = "button_main_shoesON2";
					shoesBtn.gameObject.SetActiveRecursively(true);
					shoesCantBtn.gameObject.SetActiveRecursively(false);
				}
				UIEventListener.Get(shoesCantBtn.gameObject).onClick -= OnClickDontSendShoes;
				UIEventListener.Get(shoesCantBtn.gameObject).onClick += OnClickDontSendShoes;
				UIEventListener.Get(shoesBtn.gameObject).onClick -= OnClickSendShoes;
				UIEventListener.Get(shoesBtn.gameObject).onClick += OnClickSendShoes;
			}else{
				myBackPanel.gameObject.SetActiveRecursively(false);
				defaultBackPanel.gameObject.SetActiveRecursively(true);
				
				if(data.isBlockMessage==true || data.isKakaoBlock==true){
					shoesBtn.gameObject.SetActiveRecursively(false);
					shoesCantBtn.gameObject.SetActiveRecursively(true);
					
					UIEventListener.Get(shoesCantBtn.gameObject).onClick -= OnClickDontSendShoes;
				}else{
					shoesBtn.gameObject.SetActiveRecursively(true);
					shoesBtnSprite.spriteName = "button_main_shoesON";
					shoesCantBtn.gameObject.SetActiveRecursively(false);
					
					UIEventListener.Get(shoesBtn.gameObject).onClick -= OnClickSendShoes;
					UIEventListener.Get(shoesBtn.gameObject).onClick += OnClickSendShoes;
				}
			}
			
			if(data.weekScore>0){
				charIcon.gameObject.SetActiveRecursively(true);
				if(data.weekOptionsList[1]!="" && data.weekOptionsList[1]!=null){
					string rideName = "";
					//vehicle
					switch(data.weekOptionsList[1]){
					case PlayerInfo.TYPE_VEHICLE_FENRIR:
						rideName = "ranking_btn_p4";
						break;
					case PlayerInfo.TYPE_VEHICLE_GRIFFIN:
						rideName = "ranking_btn_p1";
						break;
					case PlayerInfo.TYPE_VEHICLE_UNICON:
						rideName = "ranking_btn_p2";
						break;
					case PlayerInfo.TYPE_VEHICLE_WHITETIGER:
						rideName = "ranking_btn_p3";
						break;
					default:
						charIcon.gameObject.SetActiveRecursively(false);
						break;
					}
					if(data.weekOptionsList[5]=="1"){
						rideName += "_e";
					}
					charIcon.spriteName = rideName;
				}else{
					//character
					switch(data.weekOptionsList[0]){
					case PlayerInfo.TYPE_CHASER:
						charIcon.spriteName = "ranking_btn_c2";
						break;
					case PlayerInfo.TYPE_RISKTAKER:
						charIcon.spriteName = "ranking_btn_c3";
						break;
					case PlayerInfo.TYPE_KYLIE:
						charIcon.spriteName = "ranking_btn_c4";
						break;
					case PlayerInfo.TYPE_TREASUREHUNTER:
						charIcon.spriteName = "ranking_btn_c1";
						break;
					default:
						charIcon.gameObject.SetActiveRecursively(false);
						break;
					}
				}
			}else{
				charIcon.gameObject.SetActiveRecursively(false);
			}
		}
		
	}
	
	public void setLoad()
	{
		face.down(data.image_url);
		//if(uid == "addUserSecretUid")return;
		//if(face.url==null || face.url=="")return;
		//face.addStack();
	}
	void OnClickSendGift(GameObject go){
		UISystemPopup.OpenPopupShopGiftGold(data.userID,data.nickname);
	}
	void OnClickSendShoes(GameObject go){
		if(data.userID == GAMEdata.localUser.userID){
			UISystemPopup.OpenPopup("",TextDatas.POPUP_BLOCK_SHOES,UISystemPopup.TYPE_OK_CANCEL,OnBlockShoes);
		}else{
			if(type=="INVITE"){
				string str = epi.GAMEdata.invitePopupText;
				str = str.Replace("{from}",data.nickname);
				str = str.Replace("{leftfspoint}",PlayerInfo.GetInstance().friendLeftTodayPoint.ToString());
				UISystemPopup.OpenPopup("",str,UISystemPopup.TYPE_OK_CANCEL,SendInvite);
			}else if(type=="RANKING"){
				//send life
				
				string str = TextDatas.GetInstance().GetTextFromServer("SENDMSGPOPUP_LIFE",null,data.nickname);
				str = str.Replace("{leftfspoint}",PlayerInfo.GetInstance().friendLeftTodayPoint.ToString());
				UISystemPopup.OpenPopup("",str,UISystemPopup.TYPE_OK_CANCEL,SendShoes);

//				UISystemPopup.ViewLoadingProgress();
//				EpiServer.instance.giftItem("wr_gold_1000",data.userID);

			}
		}
	}
	void OnClickDontSendShoes(GameObject go){
		if(data.userID == GAMEdata.localUser.userID){
			UISystemPopup.OpenPopup("",TextDatas.POPUP_UNBLOCK_SHOES,UISystemPopup.TYPE_OK_CANCEL,OnUnBlockShoes);
		}
	}
	void OnClickPreview(GameObject go){
		UIManager.GetInstance().previewPanel.Open(data.nickname,data.weekOptionsList);
	}
	void OnUnBlockShoes(){
		shoesBtn.gameObject.SetActiveRecursively(true);
		shoesBtnSprite.spriteName = "button_main_shoesON2";
		shoesCantBtn.gameObject.SetActiveRecursively(false);
		NetworkManager.GetInstance().SendMessageBlock(false);
	}
	void OnBlockShoes(){
		shoesBtn.gameObject.SetActiveRecursively(false);
		shoesCantBtn.gameObject.SetActiveRecursively(true);
		NetworkManager.GetInstance().SendMessageBlock(true);
	}
	void SendShoes(){
		NetworkManager.GetInstance().SendShoes(data.userID);
	}
	void SendInvite(){
		shoesBtn.isEnabled = false;
		data.isInvited = true;
		epi.GAMEdata.invitedCount++;
		NetworkManager.GetInstance().SendInvite(data.userID);
	}
	private int _leftTick;
	private int _leftHour;
	private int _leftMin;
	private int _leftSec;
	void Update(){
		if(data!=null){
			if(type=="RANKING"){
				if(data.userID!=GAMEdata.localUser.userID){
					_leftTick = NetworkManager.GetInstance().GetLeftTick(data.userID);
					if(_leftTick > 0){
						shoesCantBtn.gameObject.SetActiveRecursively(false);
						shoesBtn.gameObject.SetActiveRecursively(false);
						pnlWait.gameObject.SetActiveRecursively(true);
						_leftSec = _leftTick%60;
						_leftTick = (_leftTick - _leftSec) / 60;
						
						_leftMin = _leftTick%60;
						
						_leftHour = (_leftTick - _leftMin) / 60;
						
						if(_leftHour<=0 && _leftMin<=0){
							lblLeftTime.text = string.Format(TextDatas.TIME_SEC,_leftSec);
						}else if(_leftHour<=0){
							lblLeftTime.text = string.Format(TextDatas.TIME_MIN_SEC,_leftMin,_leftSec);
						}else{
							lblLeftTime.text = string.Format(TextDatas.TIME_HOUR_MIN,_leftHour,_leftMin);
						}
					}else if(pnlWait.active==true){
						pnlWait.SetActiveRecursively(false);
						if(data.isBlockMessage==true || data.isKakaoBlock==true){
							shoesCantBtn.gameObject.SetActiveRecursively(true);
							shoesBtn.gameObject.SetActiveRecursively(false);
						}else{
							shoesCantBtn.gameObject.SetActiveRecursively(false);
							shoesBtn.gameObject.SetActiveRecursively(true);
						}
					}
				}
			}
		}
	}
	*/
	
	
}

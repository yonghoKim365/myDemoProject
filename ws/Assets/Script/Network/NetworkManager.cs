
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NetworkManager : MonoBehaviour {
	
	public static bool isGuest;
	public static NetworkManager instance;


	public static void RestartApplication()
	{
		Debug.Log("RestartApplication");

		UINetworkLock.instance.hide();

		PandoraManager.instance.setRelogin ();

		if(EpiServer.instance != null)
		{
			EpiServer.instance.clearValue();
		}

		if(NetworkManager.instance != null)
		{

			NetworkManager.instance.clearValue();
		}

		Application.LoadLevel(3);
	}

	void ShowProgress()
	{
		UINetworkLock.instance.show();
	}
	
	void HideProgress()
	{
		UINetworkLock.instance.hide();
	}

	void Awake(){

//		instance = this;
//		isGuest = false;
//		isInitialized = false;
		if(instance==null)
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
			isGuest = false;
			isInitialized = false;
		}else{
			DestroyImmediate(this.gameObject);
		}
	}


	public void clearValue()
	{
		isUnregistEventAtTitle = false;
		nowRequestedInvitingKakaoId = "";
	}

	
	public float gameStartLimitTime;
	
	private List<string> _systemAlertKeyList = new List<string>();
	//private P_ActionPopup _savedActionPopup;
	static bool isInitialized = false;
	void Start () {
		if(isInitialized==false)
		{

			isInitialized = true;

//			Debug.Log("network manager init()");
			EpiServer.instance.onServerCheck -= OnServerCheck;
			EpiServer.instance.onServerCheck += OnServerCheck;
			
//			EpiServer.instance.onServerStart -= OnServerStart;
//			EpiServer.instance.onServerStart += OnServerStart;
			
			EpiServer.instance.onServerReqError-=OnServerReqError;
			EpiServer.instance.onServerReqError+=OnServerReqError;
			
			EpiServer.instance.onServerReqComplete -= OnServerReqComplete;
			EpiServer.instance.onServerReqComplete += OnServerReqComplete;
			
//			EpiServer.instance.onServerState -= OnServerStatus;
//			EpiServer.instance.onServerState += OnServerStatus;
			
			PandoraManager.instance.onPandoraHandler = onPandoraHandler;
			PandoraManager.instance.onKakaoHandler = onKakaoHandler;
		}
	}





//	public void OnServerStatus(string msg)
//	{		
//		string str = msg.Replace('\n' , ' ');
//		GameManager.me.uiManager.uiTitle.tfConnectingState.text = str;
//	}
	
	
	
	public void OnServerCheck(int mode_p, string noticeMsg_p, string url_p){
		/*
		  *  ToC_CHECK
		  * MODE
		  * URL
		  * MSG
		  * MODE => 0,1,2
		  * 0 URL => start.php
		  * 1 MSG popup, URL => start.php
		  * 2 MSG popup => if URL applicationurl , not RESTART game
		  * */
		//url_p=url_p.Replace("office.linktomorrow.com","192.168.1.201");
		switch(mode_p){
		case 0:
			//ServerStart(url_p);
			break;
		case 1:
			//serverStartURL = url_p;
			//UISystemPopup.OpenPopup("",noticeMsg_p,UISystemPopup.TYPE_OK,OnServerStartAfterPopup);
			break;
		case 2:
			/*	if(string.IsNullOrEmpty(url_p)){
				UISystemPopup.OpenPopup("",noticeMsg_p,UISystemPopup.TYPE_OK,OnRestartApplication);	
			}else{
				applicationURL = url_p;
				UISystemPopup.OpenPopup("",noticeMsg_p,UISystemPopup.TYPE_OK,OnApplicationOpenAfterPopup);
			}*/
			break;
		}
	}

	

	public void OnServerReqError(string cmd, string msg, int errCode, int errLv)
	{
	}


	public bool isUnregistEventAtTitle = false;
	public bool isCompleteUnregistEvent = false;


	public bool isLockKakaoUnregistEvent = false;
	public void onPandoraHandler(PandoraManager.PandoraState ps)
	{
		//EpiServer.instance.sendLinkServerCheck();

		if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
		{
			Debug.Log("networkManager.onPandoraHandler  "+ps);
		}

//		GameManager.me.setGuiLog("networkManager.onPandoraHandler  "+ps);

		//1. weme -> start


		//progress kakao login
		switch(ps)
		{
		case PandoraManager.PandoraState.READY_WEME_NOTICE:
			Debug.Log("Ready Weme Notice");
			UIManager.setGameState( "READY_WEME_NOTICE", 0.13f );
			showWemeNotice();
			break;

			// 자동으로 뜨는 것....
		case PandoraManager.PandoraState.SHOW_LOGIN_BTN :
			Debug.Log("SHOW_LOGIN_BTN");

			//show kakao Login Btn
			UIManager.setGameState( Util.getUIText( "SHOW_LOGIN_BTN" ), 0.13f );
#if UNITY_EDITOR
			PandoraManager.instance.loginKakao();
#else
			if(GameManager.me.uiManager.uiTitle != null) GameManager.me.uiManager.uiTitle.showButton(); // ==> 카톡인 episerver.kakaologin...
#endif
			break;
		case PandoraManager.PandoraState.KAKAO_LOGIN : 
			//start kakao login // 자동 과정..
			Debug.Log("kakao login start");
			UIManager.setGameState(Util.getUIText( "KAKAO_LOGIN" ), 0.15f );
			break;
		case PandoraManager.PandoraState.KAKAO_AUTH :
			//weme auth // 과정...
			Debug.Log("weme auth");
			UIManager.setGameState(Util.getUIText(  "KAKAO_AUTH" ), 0.17f );
			if(GameManager.me.uiManager.uiTitle != null) GameManager.me.uiManager.uiTitle.hideButton();
			break;
		case PandoraManager.PandoraState.KAKAO_MYINFO :
			//get kakao myinfo
			Debug.Log("get kakao myInfo");
			UIManager.setGameState(Util.getUIText(  "KAKAO_MYINFO" ), 0.2f);
			break;
		case PandoraManager.PandoraState.KAKAO_FRIENDLIST :
			//get friend info
			Debug.Log("get kakao friendInfo");
			UIManager.setGameState( Util.getUIText(  "KAKAO_FRIENDLIST" ) , 0.22f);
			break;
		case PandoraManager.PandoraState.KAKAO_LOGIN_COMPLETE :
			//complete login
			Debug.Log("kakao login complete");

			UIManager.setGameState(Util.getUIText(  "KAKAO_LOGIN_COMPLETE" ), 0.24f );

			epi.GAME_DATA.kakaoDataInit();
			
			string[] fArr = new string[PandoraManager.instance.appFriendDic.Count];
			PandoraManager.instance.appFriendDic.Keys.CopyTo(fArr,0);
			string uid = PandoraManager.instance.localUser.userID;
			string kakaoToken = PandoraManager.instance.kakaoToken;

			if(GameManager.me.isGuest)
			{
				EpiServer.instance.sendMigrateKakaoId(PandoraManager.instance.localUser.userID, WSDefine.NO);
				return;
			}

			GameDataManager.instance.deviceId = null;

			EpiServer.instance.convertPandoraFriendsListToGameFriendList();

				// 실제 게임 서버로.
			EpiServer.instance.getAuth("KAKAO",uid,kakaoToken,fArr);

			break;
		case PandoraManager.PandoraState.DEVICE_LOGIN :
			//complete login
			Debug.Log("Device login complete");
			string[] emptyArr = new string[]{};

			string deviceID = PandoraManager.instance.localUser.userID;
			string deviceSeq = PandoraManager.instance.kakaoToken;

			if(GameManager.isDebugBuild)
			{
				Debug.LogError("DEVICE : " + PandoraManager.PandoraState.DEVICE_LOGIN);
			}

			EpiServer.instance.getAuth("DEVICE",deviceID,deviceSeq,emptyArr);
			//ResourceManager.instance.LoadMainScene();
			UIManager.setGameState( Util.getUIText("DEVICE_LOGIN") );
			break;

		case PandoraManager.PandoraState.KAKAO_LOGOUT:

			Debug.Log("KAKAO_LOGOUT");

			UIManager.setGameState( Util.getUIText("KAKAO_LOGOUT") );
			RestartApplication();
			break;


		case PandoraManager.PandoraState.KAKAO_UNREGIST:

			Debug.Log("KAKAO_UNREGIST");
			UIManager.setGameState( "KAKAO_UNREGIST" );

			isCompleteUnregistEvent = true;

			if(isUnregistEventAtTitle == false)
			{
				NetworkManager.RestartApplication();
			}

			break;

		default:

			Debug.Log("default");

			UIManager.setGameState("Unkown ps state: " + ps.ToString());
			break;
		}
	}



	public void showWemeNotice()
	{
		bool isUsedPopupLoginAtClose = false;

		//-------check maintenance
		WemeMaintenance maintenance = PandoraManager.instance.getMaintenance();

		if(maintenance != null)
		{
			string txt = maintenance.reason;

			bool canUseMaintenance = true;

			if(string.IsNullOrEmpty(txt) == false && txt.StartsWith("AOS^"))
			{
				txt = txt.Substring(4);

#if UNITY_IOS
				canUseMaintenance = false;
#endif

			}
			else if(string.IsNullOrEmpty(txt) == false && txt.StartsWith("IOS^"))
			{
				txt = txt.Substring(4);

#if UNITY_ANDROID
				canUseMaintenance = false;
#endif
			}

			if(canUseMaintenance)
			{
				txt += "\n( " + Util.parseWemeSDKDate(maintenance.begin);
				txt += "~ "  + Util.parseWemeSDKDate(maintenance.end) +" )";
				isUsedPopupLoginAtClose = true;
				
				UISystemPopup.open(UISystemPopup.PopupType.SystemError, txt , NetworkManager.RestartApplication, NetworkManager.RestartApplication);
			}
		}

		//---------checkClientState
		if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
		{
			Debug.LogError("PandoraManager.instance.clientState : " + PandoraManager.instance.clientState);
		}

		switch(PandoraManager.instance.clientState)
		{

		case "prepare":
			isUsedPopupLoginAtClose = true;
			break;

		case "test":
			break;

		case "service":
			break;
		case "upgraderecommend":

			if(isUsedPopupLoginAtClose == false)
			{
				UISystemPopup.openFirst(UISystemPopup.PopupType.YesNo, Util.getUIText("UPGRADENEED"), GoMarket,PandoraManager.instance.loginProcess);
			}
			else
			{
				UISystemPopup.openFirst(UISystemPopup.PopupType.YesNo, Util.getUIText("UPGRADENEED"), GoMarket, null);
			}

			isUsedPopupLoginAtClose = true;
			break;
		case "upgradeneed":

			UISystemPopup.openFirst(UISystemPopup.PopupType.SystemError, Util.getUIText("UPGRADERECOMMEND"), GoMarket, GoMarket);

			isUsedPopupLoginAtClose = true;
			break;

		case "finish":
			isUsedPopupLoginAtClose = true;
			break;

		}

		WemeNotice noti = PandoraManager.instance.getNextNotice();
		noti = checkNoticeRepeatType(noti);

		if(noti==null)
		{
			if(isUsedPopupLoginAtClose==false)
			{
				PandoraManager.instance.loginProcess();
			}
		}
		else
		{
			saveNoticeRecord(noti);

			if(string.IsNullOrEmpty(noti.detailLink))
			{
				if(isUsedPopupLoginAtClose==false)
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents, PandoraManager.instance.loginProcess, PandoraManager.instance.loginProcess);
				}
				else
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents);
				}
			}
			else
			{
				if(isUsedPopupLoginAtClose==false)
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents , PandoraManager.instance.loginProcess, PandoraManager.instance.loginProcess, UISystemPopup.getUrlLinkString(noti.detailLink));
				}
				else
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents , null, null, UISystemPopup.getUrlLinkString(noti.detailLink));
				}
			}
		}

		
		do{
			noti = PandoraManager.instance.getNextNotice();
			if(noti!=null)
			{
				if(string.IsNullOrEmpty(noti.detailLink)==false)
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents , null, null, UISystemPopup.getUrlLinkString(noti.detailLink));
				}
				else
				{
					UISystemPopup.openFirst(getNoticePopupType(noti), noti.contents);
				}
			}
		}while(noti!=null);

	}


	public WemeNotice checkNoticeRepeatType(WemeNotice noti)
	{
		if(noti == null) return null;
		if(noti.showType == "once")
		{
			if(PlayerPrefs.GetInt("NOTI"+noti.id,WSDefine.NO) == WSDefine.YES)
			{
				return null;
			}
		}

		return noti;
	}

	public void saveNoticeRecord(WemeNotice noti)
	{
		if(noti == null) return;
		if(noti.showType == "once")
		{
			PlayerPrefs.SetInt("NOTI"+noti.id,WSDefine.YES);
		}
	}



	public UISystemPopup.PopupType getNoticePopupType(WemeNotice noti)
	{
		if( (noti.contents != null && noti.contents.Length > 10 && noti.contents.Substring(0,5).ToLower().StartsWith("http")) ||  
		   noti.contents.Length > 128 || (string.IsNullOrEmpty(noti.detailLink) != null && noti.detailLink.Length > 10 ) )
		{
			return UISystemPopup.PopupType.Image;
		}
		else
		{
			return UISystemPopup.PopupType.Default;
		}
	}





	public void GoMarket()
	{
		PandoraManager.instance.goMarket();
		UIManager.setGameState(Util.getUIText("UPGRADE_NEWVERSION"));

		if(GameManager.me.uiManager.uiTitle != null) 
		{
			if(GameManager.me.uiManager.uiTitle.btnLogin.gameObject.activeSelf)
			{
				GameManager.me.uiManager.uiTitle.btnLogin.gameObject.SetActive(false);
			}
			
			if(GameManager.me.uiManager.uiTitle.btnGuest.gameObject.activeSelf)
			{
				GameManager.me.uiManager.uiTitle.btnGuest.gameObject.SetActive(false);
			}
		}
	}
	
	public void OnServerReqComplete(string cmd){
		
		//-------check ChangeData
		//checkChangedData(reqData);
		
		switch(cmd)
		{
		case epi.COMM.RESTART:
			RestartApplication();
			break;
		case epi.COMM.KAKAO_LOGIN:
			isGuest = false;
			break;
		case epi.COMM.KAKAO_LOGINCHECK:
			break;
		case epi.COMM.AUTH:
//			isLogined = true;
//			Debug.LogError("epi.COMM.AUTH");

			UIManager.setGameState( Util.getUIText("REC_USER_INFO") );


			PandoraManager.instance.getWemePromotion();

			break;

		case epi.COMM.KAKAO_LOGOUT:
			UINetworkLock.instance.hide();
			RestartApplication();
			break;

		case epi.COMM.INIT:
			GameManager.me.onCompleteInitKakao();
			break;

		default:

			break;
		}
	}



	public string nowRequestedInvitingKakaoId = "";
	void onKakaoHandler(string URI, int code, string errorDesc)
	{
		if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
		{
			Debug.Log("networkManager.onKakaoHandler : "+URI+"   code: "+code ); //+"   id : "+temKakaoID); 
		}
		
		switch(code)
		{
			/**< @memberof KaError error code means success.(no error) */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_SUCCESS :						  		

			if( (URI == "sendinvitelinkmessage" || URI == "sendlinkmessage" ) && nowRequestedInvitingKakaoId!="")
			{
				//EpiServer.instance.sendInviteFriend(temKakaoID);
				//EpiServer.instance.RequestInviteFriend(temKakaoID, code.ToString());
				EpiServer.instance.sendInviteFriend(nowRequestedInvitingKakaoId);
				nowRequestedInvitingKakaoId = "";
			}

			break;


			/**< @memberof KaError error code means cannot use on guest login. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_SUPPORTED_IN_GUEST_MODE  :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "게스트 모드에서는 사용할 수 없는 기능입니다.");
			break;				

			/**< @memberof KaError error code means login success but not activate email address. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_SUCCESS_NOT_VERIFIED  :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "이메일 인증이 되지 않았습니다.");
			break;				

			/**< @memberof KaError error code means common error see desc. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_ERROR :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "카카오톡 연동 오류가 발생했습니다.");
			break;								

			/**< @memberof KaError error code means permission denied. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_INSUFFICIENT_PERMISSION :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "허가되지 않은 기능을 호출했습니다.");
			break;			

			/**< @memberof KaError error code means send message failed, invite message is once of 30 days. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_EXCEED_INVITE_CHAT_LIMIT :		

			UISystemPopup.open(UISystemPopup.PopupType.Default, "동일한 친구에게는 1달에 1번만 초대메시지를 보낼 수 있습니다.");

			break;			

			/**< @memberof KaError error code means send message failed, message block userself (not installed). */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_INVITE_MESSAGE_BLOCKED  :			
			UISystemPopup.open(UISystemPopup.PopupType.Default, "초대 메시지가 거부되었습니다.");
			break;				

			/**< @memberof KaError error code means send message failed, message block userself (installed). */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_MESSAGE_BLOCK_USER :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "메시지가 거부되었습니다.");
			break;			

			/**< @memberof KaError error code means cannot found chatroom infomation (cattingplus). */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_CHAT_NOT_FOUND  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "채팅 플러스 오류.");
			break;

			/**< @memberof KaError error code means send message failed, message block userself (installed). */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_UNSUPPORTED_DEVICE  :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "지원하지 않는 기기입니다.");
			break;				

			/**< @memberof KaError error code means unregisterd user. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_UNREGISTERD_USER  :	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "등록되지 않은 사용자입니다.");
			break;	

			/**< @memberof KaError error code means invalid parameter. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_INVALID_REQUEST  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "잘못된 요청입니다.");
			break;	

			/**< @memberof KaError error code means withdraw user. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_DEACTIVATED_USER  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "탈퇴 과정에서 오류가 발생했습니다.");
			break;	

			/**< @memberof KaError error code means not found user info. */	
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_NOT_FOUND :		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "사용자 정보를 찾을 수 없습니다.");
			break;	

			/**< @memberof KaError error code means maintenance kakao server. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_UNDER_MAINTENANCE  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "카카오톡 서버 점검 중입니다.");
			break;	

			/**< @memberof KaError error code means user age too low. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_LOWER_AGE_LIMIT  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "연령제한에 걸렸습니다.");
			break;	

			/**< @memberof KaError error code means not avalable push token. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_INVALID_PUSH_TOKEN  :		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "푸쉬 오류가 발생했습니다.");
			break;	

			/**< @memberof KaError error code means invite message full for game. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_EXCEED_CHAT_LIMIT :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "게임에 허가된 초대메시지 횟수를 초과하였습니다.");
			break;
		
			/**< @memberof KaError error code means invite message full for day. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_INVITE_LIMIT  :						  	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "오늘은 더 이상 초대하실 수 없습니다.");
			break;				

			/**< @memberof KaError error code means access_token not avalable. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_KAServerErrorNotAuthorized  :	
			UISystemPopup.open(UISystemPopup.PopupType.Default, "접근 오류가 발생했습니다.");
			break; 		

			/**< @memberof KaError error code means refresh_token not avalable. */
		case (int)KakaoManager.KakaoErrType.KAKAO_ERR_KAServerErrorInvaildGrant  :						  		
			UISystemPopup.open(UISystemPopup.PopupType.Default, "카카오톡 인증 오류가 발생했습니다.");
			break;      
		default:


			UISystemPopup.open(UISystemPopup.PopupType.Default, "error: " + code + "\n" + errorDesc);

			break;
		}

	}



	public void kakaoLogin()
	{
		PandoraManager.instance.loginKakao();
	}

	public void KakaoLogout(){
		StartCoroutine(KakaoLogoutProcess());
	}

	IEnumerator KakaoLogoutProcess()
	{
		ShowProgress();
		yield return new WaitForSeconds(1f);
		PandoraManager.instance.kakaoLogOut();
	}



	




}
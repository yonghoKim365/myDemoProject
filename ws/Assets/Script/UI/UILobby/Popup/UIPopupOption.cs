using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UIPopupOption : UIPopupBase {

	public string testSoundId = "";
	public SoundManager.SoundPlayType testSoundPlayType = SoundManager.SoundPlayType.Music;
	public AudioFader.State testFadeType = AudioFader.State.FadeIn;
	public float testSoundFadeTime = 1.0f;


	public UISprite spKakaoLoginOut;

	public UIButton btnLogout, btnCenter, btnQuitGame, btnHelpShowPhoto, btnCoupon, btnShowTerm, btnCafe, btnShowGuestId;
	public UIToggle cbBGM, cbSFX, cbPushNotice, cbLowPc, cbShowMyPic;

	public UILabel lbUserIdWord, lbUserId, lbClientVersion;

	public UIButton btnLoadTestData, btnClearDebugData, btnTestCompose, btnTestMake;


	protected override void awakeInit ()
	{


		UIEventListener.Get(btnLogout.gameObject).onClick = onClickLogout;
		UIEventListener.Get(btnCenter.gameObject).onClick = onClickCenter;
		UIEventListener.Get(btnQuitGame.gameObject).onClick = onClickQuitGame;
		
		UIEventListener.Get(cbBGM.gameObject).onClick = onClickBgm;
		UIEventListener.Get(cbSFX.gameObject).onClick = onClickSFX;
		
		UIEventListener.Get(cbPushNotice.gameObject).onClick = onClickNotice;
		UIEventListener.Get(cbLowPc.gameObject).onClick = onClickLowPc;
		UIEventListener.Get(cbShowMyPic.gameObject).onClick = onClickShowMyPic;		


		UIEventListener.Get(btnCoupon.gameObject).onClick = onClickCoupon;		
		UIEventListener.Get(btnShowTerm.gameObject).onClick = onClickShowTerm;		
		UIEventListener.Get(btnCafe.gameObject).onClick = onClickCafe;		

		UIEventListener.Get(btnShowGuestId.gameObject).onClick = onClickGuestId;		



		UIEventListener.Get(btnHelpShowPhoto.gameObject).onClick = onClickHelpShowMyPic;		

		UIEventListener.Get(btnLoadTestData.gameObject).onClick = onClickTestData;
		UIEventListener.Get(btnClearDebugData.gameObject).onClick = onClickDeleteData;

		UIEventListener.Get(btnTestCompose.gameObject).onClick = onClickTestCompose;
		UIEventListener.Get(btnTestMake.gameObject).onClick = onClickTestMake;

	}


	void onClickGuestId(GameObject go)
	{
		if(GameManager.me.isGuest)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "[ "+ Util.getUIText("GUIEST_ID") +" ]\n"+PandoraManager.instance.guestUserId + ".device\n" + PandoraManager.instance.localUser.userID);
		}
	}


	protected override void onClickClose (GameObject go)
	{
		base.onClickClose (go);
		//EpiServer.instance.sendSetSetting();
	}

	bool _clickCoupon = false;

	void onClickCoupon(GameObject go)
	{
//		UISystemPopup.open(UISystemPopup.PopupType.Default, "준비중입니다.", null, null);

		EpiServer.instance.sendCouponEvent();

	}


	public void onCompleteReceiveCouponUrl(string url)
	{
		if(string.IsNullOrEmpty(url) == false)
		{
			PandoraManager.instance.showWebView(url, onCloseCouponPage);
		}
		else
		{
			UISystemPopup.open(Util.getUIText("COUPON_URL_ERR"));
		}
	}



	void onCloseCouponPage(string resultString)
	{
		if(EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
		{
			Debug.LogError("onCloseCouponPage : " + resultString);
		}

		EpiServer.instance.sendRefreshEvent("COUPON");
		GameManager.me.clearMemory();
	}



	void onClickShowTerm(GameObject go)
	{
		PandoraManager.instance.showTermsView(onCloseTerm);
	}


	void onCloseTerm(string result)
	{
		GameManager.me.clearMemory();
	}



	void onClickCafe(GameObject go)
	{
		//UISystemPopup.open(UISystemPopup.PopupType.Default, "준비중입니다.", null, null);
		//return;

		PandoraManager.instance.showWebView("http://cafe.naver.com/windsoulweme", onCloseCafe);
	}


	void onCloseCafe(string result)
	{
		GameManager.me.clearMemory();
	}



	void onSendHartTemplet()
	{
//		Dictionary<string, string> dic = new Dictionary<string, string>();
//		dic.Add("sender_name",PandoraManager.instance.localUser.nickname);
//		PandoraManager.instance.kakaoSendLinkMessage("88377728244541937",  "초대 테스트", "2273", "", dic, "");
	}

	void onSendInvite()
	{
//		Dictionary<string, string> dic = new Dictionary<string, string>();
//		dic.Add("sender_name",PandoraManager.instance.localUser.nickname);
//		dic.Add("game_name","윈드소울");
//
//		PandoraManager.instance.kakaoSendLinkMessage("88377728244541937",  "초대 테스트", "2272",  "", dic, "");
	}

	void onConfirmPurchase()
	{
		EpiServer.instance.confirmMissingPurchase();
	}

	void onConfirmPurchase2()
	{
		EpiServer.instance.confirmMissingPurchase();
	}


	int i = 0;
	void onClickTestCompose(GameObject go)
	{
		AssetBundleManager.instance.deleteAllDownloadAssets();

		Debug.LogError("리소스 삭제");

		/*
		if(i == 0)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, "Hard Templet Test", onSendHartTemplet, null);
		}
		else if(i == 1)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, "Invite Test", onSendInvite, null);
		}
		else if(i == 2)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, "Confirm Missing Purchase", onConfirmPurchase, null);
		}
		else if(i == 3)
		{
			UISystemPopup.open(UISystemPopup.PopupType.YesNo, "Confirm Missing Purchase MUST", onConfirmPurchase2, null);
		}

		++i;

		if(i > 3) i = 0;



		return;

		if(i == 0)
		{
			RuneStudioMain.instance.playComposeResult("LEO_HD4_31_20_0", new string[]{"LEO_BD4_11_20","LEO_WP4_11_20_0"}, GameIDData.Type.Equip);
		}
		else if(i == 1)
		{
			RuneStudioMain.instance.playComposeResult("UN21202001_0", new string[]{"UN31202001_0","UN40602001_0"}, GameIDData.Type.Unit);
		}
		else if(i == 2)
		{
			RuneStudioMain.instance.playComposeResult("SK_5201_20_0", new string[]{"SK_1402_20","SK_3402_20_0"}, GameIDData.Type.Skill);
		}

		++i;
		if(i > 2) i = 0;
		*/
	}



	void onClickTestMake(GameObject go)
	{

		string[] purchaseArr = EpiServer.instance.pIab.getMissingPurchase();

		Debug.LogError("purchaseArr : " + purchaseArr.Length);

		if(purchaseArr.Length>0)
		{
			EpiServer.instance.pIab.confirmPurchase(EpiServer.instance.pIab.getOrderID(purchaseArr[0]));
		}

		return;

		WemeManager.Instance.showPopUpPromotion(
			WemeManager.WmPromotionDisplayOrientation.WM_PROMOTION_DISPLAY_ORIENTATION_LANDSCAPE, 
			WemeManager.WmPromotionPopupExposeStyle.WM_PROMOTION_POPUP_EXPOSE_STYLE_SINGLE_POPUP, 
			testWemeCallback);
	}


	public void testWemeCallback(string jsonResult)
	{
		Debug.LogError("testWemeCallback jsonResult : " + jsonResult);
	}



	void onClickHelpShowMyPic(GameObject go)
	{
		UISystemPopup.open( UISystemPopup.PopupType.Default, Util.getUIText("MSG_SHOW_PHOTO_HELP"));
	}


	void onClickDeleteData(GameObject go)
	{
		PlayerPrefs.DeleteAll();

		string[] fuck = AssetBundleManager.instance.deleteTempData();

		if(fuck != null && fuck.Length > 0)
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, string.Join(",",fuck) + " Delete!");
		}
		else
		{
			UISystemPopup.open(UISystemPopup.PopupType.Default, "지울 파일이 없습니다.");
		}
	}



	void onClickTestData(GameObject go)
	{
		string[] loadFiles = {"data_monster_client","data_skilleffectsetup_client","data_setup_client","data_tip_client","data_baseunitrune_client","data_unitrune_client","data_e_unit_client","data_e_unit_rare_client","data_skillicon_client","data_unit_skill_client","data_baseskillrune_client","data_act_client","data_stage_client","data_round_client","data_item_client","data_undead_hero_client","data_heromon_ai_client","data_hero_leo_client","data_text_client","data_equip_base_client","data_equipment_client","data_npc_client","data_effect_client","data_playerai_client","data_testmode_client","data_us_client","data_bulletpattern_client","data_sigong_test_client"};

		List<string> fuck = new List<string>();

		fuck.AddRange(loadFiles);

		foreach(KeyValuePair<string, RoundData> kv in GameManager.info.roundData)
		{
			string[] cids = kv.Value.cutSceneId.Split(',');
			if(cids == null) continue;
			foreach(string c in cids)
			{
				if(string.IsNullOrEmpty(c) == false)
				{
					if(fuck.Contains(c) == false) fuck.Add("cs/"+c.ToLower());
				}
			}
		}

		AssetBundleManager.instance.startDownloadTempDataFile(fuck.ToArray());
	}


	void onClickLogout(GameObject go)
	{
//#if UNITY_EDITOR
//		PlayerPrefs.DeleteAll();
//#endif
		if(GameManager.me.isGuest)
		{
			NetworkManager.RestartApplication();
			//PandoraManager.instance.loginKakao();
		}
		else
		{

#if UNITY_EDITOR
			NetworkManager.RestartApplication();
#else
			EpiServer.instance.sendLogout();
#endif
		}
	}

	void onClickCenter(GameObject go)
	{

		#if UNITY_EDITOR
		PlayerPrefs.DeleteAll();
		#endif


#if UNITY_EDITOR
		NetworkManager.RestartApplication();
#else
		PandoraManager.instance.showMainPlatformView();
#endif
	}

	void onClickQuitGame(GameObject go)
	{
		UISystemPopup.open(UISystemPopup.PopupType.YesNo,Util.getUIText("QUIT1"),QuitProcess1);
	}

	void QuitProcess1()
	{
		UISystemPopup.open(UISystemPopup.PopupType.YesNo,Util.getUIText("QUIT2"),QuitProcess2);
	}

	void QuitProcess2()
	{
		UISystemPopup.open(UISystemPopup.PopupType.YesNo,Util.getUIText("QUIT3"),EpiServer.instance.sendRemoveUser);
	}



	void onClickSFX(GameObject go)
	{
		GameManager.me.isMuteSFX = !cbSFX.value;
		GameManager.soundManager.muteSFX(!cbSFX.value);
	}

	
	void onClickBgm(GameObject go)
	{
		GameManager.me.isMuteBgm = !cbBGM.value;
		GameManager.soundManager.muteBGM(!cbBGM.value);
	}
	
	void onClickLowPc(GameObject go)
	{
		PerformanceManager.GetInstance().lowPc = cbLowPc.value;
	}



	void onClickNotice(GameObject go)
	{
		if(PandoraManager.instance.localUser.messageBlock && cbPushNotice.value == true)
		{
			UISystemPopup.open( UISystemPopup.PopupType.Default, Util.getUIText("CHECK_MSG_ALLOW"));
			setMessageBlock(true);
			return;
		}

		Debug.LogError(cbPushNotice.value);
		GameDataManager.instance.messageBlock = (cbPushNotice.value?WSDefine.NO:WSDefine.YES);
		EpiServer.instance.sendSetSetting();
		
	}
	
	void onClickShowMyPic(GameObject go)
	{
		Debug.LogError(cbShowMyPic.value);
		GameDataManager.instance.showPhoto = (cbShowMyPic.value?WSDefine.YES:WSDefine.NO);
		EpiServer.instance.sendSetSetting();
	}


	public void setShowPhoto(bool isShow)
	{
		cbShowMyPic.value = isShow;
	}


	public void setMessageBlock(bool isBlock)
	{
		cbPushNotice.value = !isBlock;
	}


	private bool _didInit = false;

	public override void show()
	{
		if(_didInit == false)
		{
			setShowPhoto(PlayerPrefs.GetInt("SHOWPHOTO", WSDefine.YES) == WSDefine.YES);

			int isBlockMsg = PlayerPrefs.GetInt("MESSAGEBLOCK", WSDefine.NO);

			if(PandoraManager.instance.localUser.messageBlock) isBlockMsg = WSDefine.YES;

			setMessageBlock( isBlockMsg == WSDefine.YES );

			_didInit = true;
		}

		if(PandoraManager.instance.localUser.messageBlock)
		{
			GameDataManager.instance.messageBlock = WSDefine.YES;
		}

		cbBGM.value = !GameManager.me.isMuteBgm;

		cbSFX.value = !GameManager.me.isMuteSFX;

		cbLowPc.value = PerformanceManager.isLowPc;	

		if(GameManager.me.isGuest)
		{
			lbUserIdWord.text = Util.getUIText("GUIEST_ID") + "(" + Util.getUIText("READ_DETAIL")  +")";

			lbUserId.text = Util.GetShortID(PandoraManager.instance.guestUserId + ".device " + PandoraManager.instance.localUser.userID, 18);

			spKakaoLoginOut.spriteName = "img_text_login";

		}
		else
		{
			lbUserIdWord.text = Util.getUIText("SNS_MEMNUMBER");
			lbUserId.text = PandoraManager.instance.localUser.userID;

			spKakaoLoginOut.spriteName = "img_text_logout";
		}


		#if UNITY_IPHONE
		if(GameDataManager.instance.serviceMode == GameDataManager.ServiceMode.IOS_SUMMIT || GameManager.me.isGuest)
		{
			btnCoupon.gameObject.SetActive(false);
			btnCenter.gameObject.SetActive(false);
			btnCafe.gameObject.SetActive(false);
		}
		else
			#endif
		{
			btnCoupon.gameObject.SetActive(true);
			btnCenter.gameObject.SetActive(true);
			btnCafe.gameObject.SetActive(true);
		}


		lbClientVersion.text = Util.getUIText("CURRENT_VER",GameManager.info.clientFullVersion);

		base.show();


#if UNITY_EDITOR
		if(DebugManager.instance.useDebug) return;
#endif

		EpiServer.instance.sendGetSetting();



	}


}

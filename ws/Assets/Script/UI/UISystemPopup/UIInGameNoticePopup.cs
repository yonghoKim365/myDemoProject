using UnityEngine;
using System.Collections;

public class UIInGameNoticePopup : UISystemPopupBase 
{

	public PhotoDownLoader bigPhoto;
	public PhotoDownLoader middlePhoto;

	public UIScrollView middleScrollView;
	public UIScrollView bigScrollView;

	public UILabel lbSmallText, lbMiddleText, lbBigText;

	public UILabel lbSmallLeftTime, lbMiddleLeftTime, lbBigLeftTime;
	public UILabel lbSmallTitle, lbMiddleTitle, lbBigTitle;

	public GameObject spMiddleLoading, spBigLoading;
	public GameObject goSmall, goMiddle, goBig;

	public UIButton[] btnClosePopups;
	public UIButton[] btnDirectGos;
	public UIButton[] btnDetails;
	public UIButton[] btnBuys;
	public UIButton[] btnConfirms;

	public UIButton[] btnNoMoreTodays;
	public UISprite[] spNoMoreTodays;

	bool _isImageType = false;


	P_Popup _data;


	public const string OFF_TODAY = "OFF_TODAY";

	public const string SMALL = "S";
	public const string MIDDLE = "M";
	public const string BIG = "L";

	public const string REVIEW = "REVIEW";
	public const string GAMEUI = "GAMEUI";
	public const string WEBLINK = "WEBLINK";
	public const string WEME_WEBLINK = "WEME_WEBLINK";
	public const string RESTART = "RESTART";
	public const string QUIT = "QUIT";

	public string currentSize = MIDDLE;

	protected override void awakeInit ()
	{
		base.awakeInit ();
		middlePhoto.isLockMemoryUnload = true;
		bigPhoto.isLockMemoryUnload = true;

		for(int i = btnDirectGos.Length - 1; i >= 0 ; --i)
		{
			UIEventListener.Get( btnClosePopups[i].gameObject ).onClick = onClickClose;

			UIEventListener.Get( btnDirectGos[i].gameObject ).onClick = onClickOk;
			UIEventListener.Get( btnDetails[i].gameObject ).onClick = onClickOk;
			UIEventListener.Get( btnBuys[i].gameObject ).onClick = onClickOk;
			UIEventListener.Get( btnConfirms[i].gameObject ).onClick = onClickOk;

			UIEventListener.Get( btnNoMoreTodays[i].gameObject ).onClick = onClickNoMoreToday;

			btnNoMoreTodays[i].transform.parent.gameObject.SetActive(false);

			spNoMoreTodays[i].enabled = false;

		}
	}


	void onClickClose(GameObject go)
	{
		if(spNoMoreToday.enabled)
		{
			PlayerPrefs.SetInt("P"+getPopupHash(_data),getToday());
		}

		base.onClose (null);
	}


	void onClickNoMoreToday(GameObject go)
	{
		spNoMoreToday.enabled = !spNoMoreToday.enabled;
	}


	public static int getToday()
	{
		return System.DateTime.Today.Month * 100 + System.DateTime.Today.Day;
	}


	public static int getPopupHash(P_Popup popupData)
	{
		if( string.IsNullOrEmpty( popupData.text ) == false)
		{
			return popupData.text.GetHashCode(); 
		}
		else if( string.IsNullOrEmpty( popupData.image ) == false)
		{
			return popupData.image.GetHashCode();
		}

		return -1;
	}


	void onClickOk(GameObject go)
	{
		if(spNoMoreToday.enabled)
		{
			PlayerPrefs.SetInt("P"+getPopupHash(_data),getToday());
		}

		if(string.IsNullOrEmpty(_data.actionType) == false && string.IsNullOrEmpty(_data.actionData) == false)
		{
			switch(_data.actionType)
			{
			case REVIEW:
				EpiServer.instance.sendActionEvent(_data.eventId, null, _data.actionData, _data.actionType );
				break;
				
			case GAMEUI:
				setMoveUI();
				break;
				
			case WEBLINK:
				Application.OpenURL(_data.actionData);
				break;
				
			case WEME_WEBLINK:
				PandoraManager.instance.showWebView(_data.actionData);
				break;
				
			case RESTART:
				NetworkManager.RestartApplication();
				break;
				
			case QUIT:
				#if UNITY_ANDROID
				GameManager.me.OnApplicationQuit();
				#endif
				break;
			}
		}


		base.onYes(null);

	}


	void setMoveUI()
	{
		GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.None;
		
		switch(_data.actionData)
		{
		case "HERO":
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Hero;
			break;
		case "UNIT":
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Summon;
			break;
		case "SKILL":
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Skill;
			break;
		case "MISSION":
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Mission;
			break;
		case "FRIEND":
			GameManager.me.uiManager.uiMenu.directGoIndex = UIMenu.UIPosition.Friend;
			break;
			
		case "SHOP_SPECIAL":
			GameManager.me.uiManager.popupShop.showSpecialShop();
			break;
			
		case "SHOP_GOLD":
			GameManager.me.uiManager.popupShop.showGoldShop();
			break;
			
		case "SHOP_RUBY":
			GameManager.me.uiManager.popupShop.showRubyShop();
			break;
			
		case "SHOP_ENERGY":
			GameManager.me.uiManager.popupShop.showEnergyShop();
			break;
			
		case "SHOP_ITEM":
			GameManager.me.uiManager.popupShop.showItemShop();
			break;
			
		case "SHOP_GIFT":
			GameManager.me.uiManager.popupShop.showGiftShop();
			break;
		}

	}



	public override void show (PopupData pd, string msg)
	{
		base.show (pd, "");

		_data = (P_Popup)pd.data[0];

		bool isReviewPopup = false;

		if(string.IsNullOrEmpty(_data.actionType) == false && _data.actionType == REVIEW)
		{
			isReviewPopup = true;
			_data.size = MIDDLE;
		}

		switch(_data.size)
		{
		case SMALL:
			currentSize = SMALL;

			goSmall.SetActive(true);
			goMiddle.SetActive(false);
			goBig.SetActive(false);

			break;
		case BIG:
			currentSize = BIG;

			goSmall.SetActive(false);
			goMiddle.SetActive(false);
			goBig.SetActive(true);

			break;
		default:
			currentSize = MIDDLE;

			goSmall.SetActive(false);
			goMiddle.SetActive(true);
			goBig.SetActive(false);

			if(isReviewPopup)
			{
				lbText.pivot = UIWidget.Pivot.Center;
			}
			else
			{
				lbText.pivot = UIWidget.Pivot.Left;
			}

			break;
		}


		btnClosePopup.gameObject.SetActive(false);
		btnDirectGo.gameObject.SetActive(false);
		btnDetail.gameObject.SetActive(false);
		btnBuy.gameObject.SetActive(false);
		btnConfirm.gameObject.SetActive(false);

		scrollView.ResetPosition();


		if(string.IsNullOrEmpty(_data.title))
		{
			lbTitle.cachedTransform.parent.gameObject.SetActive(false);
		}
		else
		{
			lbTitle.cachedTransform.parent.gameObject.SetActive(true);
			lbTitle.text = _data.title.Replace("\\n","\n");
		}

		if( string.IsNullOrEmpty(_data.image) == false && _data.image.Length > 5 && _data.image.Substring(0,4).ToLower().StartsWith("http"))
		{
			_isImageType = true;
			photo.gameObject.SetActive(true);
			lbText.enabled = false;
			spLoading.gameObject.SetActive( true );
			StartCoroutine(startDownload(_data.image));
		}
		else
		{
			_isImageType = false;

			spLoading.gameObject.SetActive( false );
			photo.gameObject.SetActive(false);

			if(string.IsNullOrEmpty(_data.text) == false)
			{
				lbText.enabled = true;
				lbText.text = _data.text.Replace("\\n","\n");
			}
			else
			{
				lbText.enabled = false;
			}
		}

		/*
		"C_Popups":{
			"1":{
				"type":"TXT",
				"size":null,
				"title":"리뷰 이벤트",
				"text":"afdasf님 재미있게 플레이하고 계신가요?\n지금 리뷰를 작성하시면 20루비를 드립니다!\n리뷰를 작성하시겠습니까?",
				"image":null,
				"delay":1,
				"buttonType":"OK:YES,CANCEL:NO",  // OK:YES,CLOSE:NO
				"eventId":"EVT_REVIEW",
				"eventValue":null,
				"actionType":"REVIEW",
				"actionData":"market://details?id=com.linktomorrow.windsoul",
				"options":null
			}
		},
		*/

		if(_data.options != null && _data.options.ContainsKey("OFF_TODAY"))
		{
			if(_data.options["OFF_TODAY"] == "AUTO")
			{
				btnNoMoreToday.gameObject.SetActive(false);
				spNoMoreToday.enabled = true;
			}
			else
			{
				btnNoMoreToday.gameObject.SetActive(true);
				spNoMoreToday.enabled = false;
			}
		}
		else
		{
			btnNoMoreToday.gameObject.SetActive(false);
		}

		lbLeftTime.gameObject.SetActive(false);

//		if(_data.actionType == REVIEW)
//		{
//			setReviewPopup();
//		}

		StartCoroutine(refreshButton());

	}


	void setReviewPopup()
	{
		// 여기는 임시 코드들이다... 안드로이드에서 실수로 title과 bodyText가 거꾸로 되어있어서... 나중에 서버 패치하고 난 뒤에 여기도 확실히 수정해야한다.
		
		try
		{
			if(string.IsNullOrEmpty(_data.text) == false && _data.text.Length > 30)
			{
				lbText.text = _data.text.Replace("\\n","\n");
				lbTitle.text = _data.title.Replace("\\n","\n");
			}
			else
			{
				lbText.text = _data.title.Replace("\\n","\n");
				lbTitle.text = _data.text.Replace("\\n","\n");
			}
		}
		catch
		{
			
		}
	}



	const string OK = "OK"; // 하단 초록색 버튼.
	const string CLOSE = "CLOSE"; // 우측 상단 닫기 버튼.

	const string BUY = "BUY"; // 구매.
	const string GO = "GO"; // 바로가기 
	const string DETAIL = "DETAIL"; // 자세히 보기
	const string CONFIRM = "CONFIRM"; // 확인



	IEnumerator refreshButton()
	{
		if(_data.delay > 0)
		{
			yield return new WaitForSeconds(_data.delay);
		}



		if(_data.actionType == REVIEW)
		{
			btnDirectGo.gameObject.SetActive(true);
			btnClosePopup.gameObject.SetActive(true);
		}
		else
		{
			string[] b1 = _data.buttonType.Split(',');
			
			for(int i = b1.Length - 1; i >= 0; --i)
			{
				string[] b2 = b1[i].Split(':');
				
				switch(b2[0])
				{
				case OK:
					
					if(b2[1] == BUY)
					{
						btnBuy.gameObject.SetActive(true);
					}
					else if(b2[1] == GO)
					{
						btnDirectGo.gameObject.SetActive(true);
					}
					else if(b2[1] == DETAIL)
					{
						btnDetail.gameObject.SetActive(true);
					}
					else if(b2[1] == CONFIRM)
					{
						btnConfirm.gameObject.SetActive(true);
					}
					
					break;
					
				case CLOSE:
					btnClosePopup.gameObject.SetActive(true);
					break;
				}
			}
		}

//		"buttonType":"OK:CLOSE,ACTION:DETAIL",
//		"eventId":"EVT_REVIEW",
//		"eventValue":null,
//		"actionType":"REVIEW",
//		"actionData":"market://details?id=com.linktomorrow.windsoul",

		/*
		- 타입은 OK, CLOSE, ACTION 3종류만 있음
			- 모양은 정하는 대로 많음(OK,CLOSE,DETAIL,CANCEL,YES,NO,HELP,PURCHASE,GIVEUP,REPLAY 등등...)
				- 예1) OK:DETAIL,CLOSE:CANCEL
				- 예2) OK:YES,CLOSE:NO
				- 예3) CLOSE:CLOSE
				- 예4) OK:OK
				- 예5) OK:CLOSE
				- 예6) ACTION:DETAIL => action을 수행=>OK와 동일, 다만 팝업을 닫지는 않음
				*/
	}


	IEnumerator startDownload(string msg)
	{
		float timeLimit = 0;
		if(useScaleTween && ani != null )
		{
			while(ani.isPlaying && timeLimit < 1.5f)
			{
				timeLimit += 0.1f;
				yield return Util.ws01;
			}
		}
		
		yield return Util.ws01;

		photo.init(msg);
		photo.down(msg);

		float timeout = 10.0f;
		while(timeout > 0 && ( photo.mainTexture == null || photo.mainTexture.enabled == false ) )
		{ 
			timeout -= 0.05f;
			yield return new WaitForSeconds(0.05f);
		}

		spLoading.gameObject.SetActive( false );
	}







	UIButton btnClosePopup
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnClosePopups[0];
				break;
			case BIG:
				return btnClosePopups[2];
				break;
			default:
				return btnClosePopups[1];
				break;
			}
		}
	}


	UIButton btnDirectGo
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnDirectGos[0];
				break;
			case BIG:
				return btnDirectGos[2];
				break;
			default:
				return btnDirectGos[1];
				break;
			}
		}
	}


	UIButton btnDetail
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnDetails[0];
				break;
			case BIG:
				return btnDetails[2];
				break;
			default:
				return btnDetails[1];
				break;
			}
		}
	}



	UIButton btnBuy
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnBuys[0];
				break;
			case BIG:
				return btnBuys[2];
				break;
			default:
				return btnBuys[1];
				break;
			}
		}
	}



	UIButton btnConfirm
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnConfirms[0];
				break;
			case BIG:
				return btnConfirms[2];
				break;
			default:
				return btnConfirms[1];
				break;
			}
		}
	}


	
	UIScrollView scrollView
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return middleScrollView;
				break;
			case BIG:
				return bigScrollView;
				break;
			default:
				return middleScrollView;
				break;
			}
		}
	}



	PhotoDownLoader photo
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return middlePhoto;
				break;
			case BIG:
				return bigPhoto;
				break;
			default:
				return middlePhoto;
				break;
			}
		}
	}
	
	UILabel lbText
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return lbSmallText;
				break;
			case BIG:
				return lbBigText;
				break;
			default:
				return lbMiddleText;
				break;
			}
		}
	}
	
	UILabel lbLeftTime
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return lbSmallLeftTime;
				break;
			case BIG:
				return lbBigLeftTime;
				break;
			default:
				return lbMiddleLeftTime;
				break;
			}
		}
	}


	UILabel lbTitle
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return lbSmallTitle;
				break;
			case BIG:
				return lbBigTitle;
				break;
			default:
				return lbMiddleTitle;
				break;
			}
		}
	}

	
	GameObject spLoading
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return spMiddleLoading;
				break;
			case BIG:
				return spBigLoading;
				break;
			default:
				return spMiddleLoading;
				break;
			}
		}
	}




	UISprite spNoMoreToday
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return spNoMoreTodays[0];
				break;
			case BIG:
				return spNoMoreTodays[2];
				break;
			default:
				return spNoMoreTodays[1];
				break;
			}
		}
	}



	UIButton btnNoMoreToday
	{
		get
		{
			switch(currentSize)
			{
			case SMALL:
				return btnNoMoreTodays[0];
				break;
			case BIG:
				return btnNoMoreTodays[2];
				break;
			default:
				return btnNoMoreTodays[1];
				break;
			}
		}
	}


}

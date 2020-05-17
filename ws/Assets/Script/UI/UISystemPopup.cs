using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class UISystemPopup : MonoBehaviour{


	public static string getUrlLinkString(string address)
	{
		return WEB_LINK_HEADER + address;
	}

	public enum PopupType
	{
		Default, YesNo, YesNoLeft, YesNoPrice, YesNoBuy, LevelUp, ClearRound, GoToRubyShop, GoToGoldShop, Restart, SystemError, Advice, SpecialPack, SpecialSinglePack, Image, Sell, InGameNotice, EndGame, None
	}

	public static UISystemPopup instance;

	public UISprite spBackground;

	public BoxCollider blockCollider;


	public static bool nowNetworkLock = false;
	public static bool nowOpenPopup = false;


	public const string WEB_LINK_HEADER = "L:";


	void Awake()
	{
		if(instance != null) GameObject.Destroy(instance.gameObject);

		spBackground.gameObject.SetActive(true);
		instance = this;
	}

	public static void init()
	{
		for(int i = _popups.Count - 1; i >= 0; --i)
		{
			setPopupDataToPool(_popups[i]);
		}

		_popups.Clear();

		if(nowPopupData != null) nowPopupData.reset();
		nowPopupData = null;
		_closePopupData = null;
		nowNetworkLock = false;
		nowOpenPopup = false;
	}


	public List<PopupData> popups = new List<PopupData>();


	public UILevelupPopup popupLevelup;
	public UISystemPopupBase popupMessage;


	public UISystemPopupBase popupDefault;

	public UIBigSizePopup popupBigSize;

	public UIYesNoPopup popupYesNo;
	public UIYesNoPricePopup popupYesNoPrice;

	public UIYesNoBuyPopup popupYesNoBuy;

	public UISellRunePopup popupSell;

	public UIAdvicePopup popupAdvice;

	public UIInGameNoticePopup popupInGameNotice;
	public UIEndGamePopup popupEndGame;

	public void hide()
	{
		popupLevelup.gameObject.SetActive(false);
		popupMessage.gameObject.SetActive(false);

		popupDefault.gameObject.SetActive(false);
		popupYesNo.gameObject.SetActive(false);
		popupYesNoPrice.gameObject.SetActive(false);
		popupYesNoBuy.gameObject.SetActive(false);

		popupAdvice.gameObject.SetActive(false);

		popupSell.gameObject.SetActive(false);

		popupInGameNotice.gameObject.SetActive(false);
		popupEndGame.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}


	public static void open(string msg, PopupType type = PopupType.Default)
	{
		UISystemPopup.open(type, msg);
	}



	public bool hasPopup()
	{
		return (nowPopupData != null || _popups.Count > 0);
	}



	private static List<PopupData> _popups = new List<PopupData>();
	public static PopupData nowPopupData = null;
	private static PopupData _closePopupData = null;
	static Stack<PopupData> _popupDataPool = new Stack<PopupData>();

	public static void closeNowPopup(PopupData closePopupData, bool isYesAction)
	{
		nowOpenPopup = false;

		_closePopupData = closePopupData;

		if(closePopupData != null)
		{
			if(closePopupData.popupType == UISystemPopup.PopupType.SystemError)
			{
				if(UISystemPopup.nowNetworkLock)
				{
					UISystemPopup.nowNetworkLock = false;
					UINetworkLock.instance.show();
				}
			}
		}

		nowOpenPopup = false;

		if(closePopupData != null)
		{
			if(isYesAction)
			{
				if(closePopupData.yesAction != null)
				{
					closePopupData.yesAction();
				}

				if(closePopupData.closeLink != null)
				{
					PandoraManager.instance.showWebView(closePopupData.closeLink);
				}
			}
			else
			{
				if(closePopupData.closeAction != null)
				{
					closePopupData.closeAction();
				}
			}
		}

		_closePopupData = null;

		if(nowOpenPopup == false)
		{
			if(_popups.Count > 0)
			{
				PopupData pd = _popups[0];
				_popups.Remove(pd);
				open(pd);
			}
		}

		if(nowOpenPopup == false)
		{
			nowPopupData = null;
		}

		if(closePopupData != null)
		{
			if(_popups.Contains(closePopupData))
			{
				_popups.Remove(closePopupData);
			}

			if(nowPopupData != null && closePopupData == nowPopupData) nowPopupData = null;

			setPopupDataToPool(closePopupData);
		}
	}



	static PopupData getPopupDataFromPool()
	{
		//if(_popupDataPool.Count > 0) return _popupDataPool.Pop();
		return new PopupData();
	}
	
	static void setPopupDataToPool(PopupData pd)
	{
//		if(pd == null) return;
//		pd.reset();
//		_popupDataPool.Push(pd);
	}



	public static void openFirst(PopupType type, string msg = "", PopupData.PopupAction yesAction = null, PopupData.PopupAction closeAction = null, params object[] data)
	{
		PopupData pd = getPopupDataFromPool();
		pd.setData(type, msg, yesAction, closeAction, data);
		
		if(_closePopupData != null && (nowPopupData != null && nowPopupData == _closePopupData))
		{
			nowPopupData = null;
		}
		
		if(nowPopupData != null)
		{
			_popups.Insert(0,nowPopupData);

			if(nowPopupData.popupType != pd.popupType)
			{
				UISystemPopupBase p = getPopupByType(nowPopupData.popupType);
				if(p != null) p.gameObject.SetActive(false);
			}

			nowPopupData = null;
		}
		
		open(pd);
	}



	public static UISystemPopupBase getPopupByType(PopupType type)
	{
		switch(type)
		{
		case PopupType.Default:
			return UISystemPopup.instance.popupDefault;
			break;
			
		case PopupType.Image:

			return UISystemPopup.instance.popupBigSize;
			break;

		case PopupType.InGameNotice:
			return UISystemPopup.instance.popupInGameNotice;
			break;


		case PopupType.EndGame:
			return UISystemPopup.instance.popupEndGame;
			break;

			
		case PopupType.SystemError:
			return UISystemPopup.instance.popupDefault;
			break;
			
		case PopupType.Restart:
			return UISystemPopup.instance.popupDefault;
			break;
			
		case PopupType.YesNoBuy:
			return UISystemPopup.instance.popupYesNoBuy;
			break;
			
		case PopupType.YesNoLeft:
			return UISystemPopup.instance.popupYesNo;
			break;
			
		case PopupType.YesNoPrice:
			return UISystemPopup.instance.popupYesNoPrice;
			break;
			
		case PopupType.LevelUp:
			return UISystemPopup.instance.popupLevelup;
			break;

		case PopupType.Sell:
			return UISystemPopup.instance.popupSell;
			break;


			
		case PopupType.GoToRubyShop:
			return UISystemPopup.instance.popupYesNo;
			break;
		case PopupType.GoToGoldShop:
			return UISystemPopup.instance.popupYesNo;
			break;
			
		case PopupType.Advice:
			return UISystemPopup.instance.popupAdvice;
			break;
		}

		return null;

	}





	public static void open(PopupType type, string msg = "", PopupData.PopupAction yesAction = null, PopupData.PopupAction closeAction = null, params object[] data)
	{
		PopupData pd = getPopupDataFromPool();
		pd.setData(type, msg, yesAction, closeAction, data);

		if(_closePopupData != null && (nowPopupData != null && nowPopupData == _closePopupData))
		{
			nowPopupData = null;
		}

		// 새로 띄울 팝업이 시스템 에러면 얘가 최우선 순위 되겠다. 
		// 기존에 떠있는 팝업이 있으면 저장해놓고 얘를 먼저 보여준다.
		if(type == PopupType.SystemError) 
		{
			if(nowPopupData != null)
			{
				_popups.Insert(0,nowPopupData);
				nowPopupData = null;
			}

			open(pd);
		}
		// 일반 팝업일때는 현재 보여지는 팝업이 있으면 뒤에 쌓아놓고 아니면 바로 보여준다.
		else
		{
			if(nowPopupData != null)
			{
				_popups.Add(pd);
			}
			else
			{
				open(pd);
			}
		}
	}


	static void open(PopupData pd)
	{
		nowNetworkLock = false;

		if(pd == null) return;
//		Debug.LogError(pd.popupType + "  " + pd.message);

		switch(pd.popupType)
		{
		case PopupType.Default:
			UISystemPopup.instance.camera.depth = 51;
			UISystemPopup.instance.popupDefault.show(pd, pd.message);
			nowPopupData = pd;
			break;

		case PopupType.Image:
			UISystemPopup.instance.camera.depth = 51;
			UISystemPopup.instance.popupBigSize.show(pd, pd.message);
			nowPopupData = pd;
			break;


		case PopupType.SystemError:
			UISystemPopup.instance.camera.depth = 51;
			if(UINetworkLock.instance.gameObject.activeSelf)
			{
				nowNetworkLock = true;
				UINetworkLock.instance.hide();
			}
			else
			{
				nowNetworkLock = false;
			}
			UISystemPopup.instance.popupDefault.show(pd, pd.message);
			nowPopupData = pd;
			break;

		case PopupType.EndGame:

			UISystemPopup.instance.camera.depth = 51;
			UISystemPopup.instance.popupEndGame.show(pd, pd.message);
			nowPopupData = pd;

			break;

		case PopupType.InGameNotice:
			UISystemPopup.instance.camera.depth = 51;
			UISystemPopup.instance.popupInGameNotice.show(pd, pd.message);
			nowPopupData = pd;
			break;			

		case PopupType.Restart:
			UISystemPopup.instance.camera.depth = 51;
			UISystemPopup.instance.popupDefault.show(pd, pd.message);
			pd.yesAction = NetworkManager.RestartApplication;
			pd.closeAction = NetworkManager.RestartApplication;
			nowPopupData = pd;
			break;

		case PopupType.YesNo:
			if(GameManager.me.uiManager.uiTitle != null && GameManager.me.uiManager.uiTitle.gameObject.activeInHierarchy) UISystemPopup.instance.camera.depth = 51;
			else UISystemPopup.instance.camera.depth = 11.05f;
//			UISystemPopup.instance.popupYesNo.setButton(true);
			UISystemPopup.instance.popupYesNo.show(pd, pd.message);
			nowPopupData = pd;
			break;


		case PopupType.YesNoBuy:
			if(GameManager.me.uiManager.uiTitle != null && GameManager.me.uiManager.uiTitle.gameObject.activeInHierarchy) UISystemPopup.instance.camera.depth = 51;
			else UISystemPopup.instance.camera.depth = 11.05f;
			//			UISystemPopup.instance.popupYesNo.setButton(true);
			UISystemPopup.instance.popupYesNoBuy.show(pd, pd.message);
			nowPopupData = pd;
			break;


		case PopupType.YesNoLeft:
			if(GameManager.me.uiManager.uiTitle != null && GameManager.me.uiManager.uiTitle.gameObject.activeInHierarchy) UISystemPopup.instance.camera.depth = 51;
			else UISystemPopup.instance.camera.depth = 11.05f;
//			UISystemPopup.instance.popupYesNo.setButton(false);
			UISystemPopup.instance.popupYesNo.show(pd, pd.message);
			nowPopupData = pd;
			break;

		case PopupType.YesNoPrice:
			UISystemPopup.instance.camera.depth = 11.05f;
			UISystemPopup.instance.popupYesNoPrice.show(pd, pd.message);

			if(pd.data != null && pd.data.Length > 1)
			{
				UISystemPopup.instance.popupYesNoPrice.setPriceType((string)pd.data[1]);
			}
			else
			{
				UISystemPopup.instance.popupYesNoPrice.setPriceType(null);
			}

			nowPopupData = pd;
			break;


		case PopupType.Sell:
			UISystemPopup.instance.camera.depth = 11.05f;
			UISystemPopup.instance.popupSell.show(pd, pd.message);

			nowPopupData = pd;
			break;

		case PopupType.LevelUp:
			UISystemPopup.instance.camera.depth = 11.05f;
			SoundData.play("uiet_userlvup");
			UISystemPopup.instance.popupLevelup.show(pd, pd.message);
			nowPopupData = pd;
			break;




		case PopupType.GoToRubyShop:
			UISystemPopup.instance.camera.depth = 11.05f;
			UISystemPopup.instance.popupYesNo.show(pd, Util.getUIText("GOTO_RUBY_SHOP"));
			pd.yesAction = GameManager.me.uiManager.popupShop.showRubyShop;
			pd.closeAction = null;
			nowPopupData = pd;
			break;
		case PopupType.GoToGoldShop:
			UISystemPopup.instance.camera.depth = 11.05f;
			UISystemPopup.instance.popupYesNo.show(pd, Util.getUIText("GOTO_GOLD_SHOP"));
			pd.yesAction = GameManager.me.uiManager.popupShop.showGoldShop;
			pd.closeAction = null;
			nowPopupData = pd;
			break;

		case PopupType.Advice:
			UISystemPopup.instance.camera.depth = 11.05f;
			UISystemPopup.instance.popupAdvice.show(pd, pd.message);

			nowPopupData = pd;
			break;
		}

		if(pd.popupType == PopupType.ClearRound)
		{
			UISystemPopup.instance.spBackground.color = new Color(0,0,0,0.01f);
		}
		else
		{
			UISystemPopup.instance.spBackground.color = new Color(0,0,0,133f/255f);
		}

		UISystemPopup.instance.blockCollider.enabled = true;

		nowOpenPopup = true;
	}









	// 임시코드.
	public static bool needLevelupPopup = false;


	public static void checkLevelupPopupWithoutCallback(PopupData.PopupAction popupAction = null)
	{
		if(needLevelupPopup)
		{
			needLevelupPopup = false;
			UISystemPopup.open(UISystemPopup.PopupType.LevelUp, GameDataManager.instance.level + "", popupAction, popupAction);
		}
	}

	public static void checkLevelupPopupAndReturnToScene()
	{
		GameManager.me.uiManager.stageClearEffectManager.resetPlayMaker();
		GameManager.me.returnToSelectScene();
	}





	public static void openLevelUpPopup()
	{
		needLevelupPopup = false;
		UISystemPopup.open(UISystemPopup.PopupType.LevelUp, GameDataManager.instance.level + "");
	}

}




public class PopupData
{
	public delegate void PopupAction();

	public PopupAction yesAction;
	public PopupAction closeAction;
	public string message = string.Empty;
	public object[] data;
	public string closeLink = null;

	public UISystemPopup.PopupType popupType;

	public void setData(UISystemPopup.PopupType type, string msg = "", PopupAction yesAction = null, PopupAction closeAction = null, params object[] dataParmas)
	{
		popupType = type;
		message = msg;
		this.yesAction = yesAction;
		this.closeAction = closeAction;
		data = dataParmas;

		if(data != null && data.Length > 0)
		{
			if(data[0].ToString().StartsWith(UISystemPopup.WEB_LINK_HEADER))
			{
				closeLink = data[0].ToString().Substring(2);
			}
			else closeLink = null;
		}
		else
		{
			closeLink = null;
		}
	}

	public void reset()
	{
		message = string.Empty;
		yesAction = null;
		closeAction = null;
		data = null;
		closeLink = null;
	}
}

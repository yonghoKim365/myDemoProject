using UnityEngine;
using System.Collections;
using System;

public class UISpecialItemSlot : UIListGridItemPanelBase
{
	public UIDragScrollView scrollView;

	public PhotoDownLoader photo;

	private Vector3 _v = new Vector3();
	private P_Annuity _data;

	public UIButton btnConsume;
	public UIButton btnBuy;

	public UILabel lbConsumeCount, lbEndDate, lbLeftTimeWord, lbLeftTime;

	public UISprite spRewardIcon;



	protected override void initAwake ()
	{
		UIEventListener.Get(btnBuy.gameObject).onClick = onClickBuy;
		UIEventListener.Get(btnConsume.gameObject).onClick = onClickConsume;
		
		if(scrollView.scrollView == null)
		{
			scrollView.scrollView = GameManager.me.uiManager.popupShop.scrollView;
		}

	}


	public override void setPhotoLoad ()
	{
		if(_data != null &&_data.imageUrl != null)
		{
			photo.down(_data.imageUrl);
		}

	}


	void onClickConsume(GameObject go)
	{
		EpiServer.instance.sendConsumeAnnuity(_data.annuityId);

	}

	
	void onClickBuy(GameObject go)
	{
		#if UNITY_IOS
		if(GameManager.me.isGuest && GameDataManager.instance.serviceMode != GameDataManager.ServiceMode.IOS_SUMMIT)
		{
			UISystemPopup.open("게스트 모드일 때는 구매를 하실 수 없습니다.");
			return;
		}
		#endif


		string price = "";

#if UNITY_ANDROID
		price = Util.GetCommaScore( Mathf.RoundToInt( _data.price )) + "원";
#else
		price = "$" + string.Format("{0:F2}", _data.price);
#endif

		UISystemPopup.open( UISystemPopup.PopupType.YesNoBuy, Util.getUIText("SPECIAL_BUY_CONFIRM", _data.title, Util.getHangulJosa(_data.title), price), onConfirmBuy, null, Util.getUIText("SPECIAL_BUY_CONFIRM2"));
	}

	
	void onConfirmBuy()
	{

#if UNITY_EDITOR
		EpiServer.instance.sendBuyByMoney(_data.productId, null, null, null);			
#else
		EpiServer.instance.buyItem(_data.productId, false, "");
#endif
	}




	public override void setData (object obj)
	{

		if(obj == null)
		{
			this.gameObject.SetActive(false);
			return;
		}
		else
		{
			this.gameObject.SetActive(true);
		}

		_data = (P_Annuity)obj;

		photo.init(_data.imageUrl);

		if( string.IsNullOrEmpty( lbLeftTimeWord.text ) )
		{
			lbLeftTimeWord.text = Util.getUIText("SPECIALITEM_LEFTITME");
		}



		if(_data.didBuy == WSDefine.YES)
		{
			btnBuy.gameObject.SetActive(false);

			if(_data.didConsume == WSDefine.YES)
			{
				btnConsume.gameObject.SetActive(false);
				lbLeftTimeWord.enabled = true;
				lbLeftTime.enabled = true;

				StartCoroutine(updateLeftTime(true));
			}
			else
			{
				btnConsume.gameObject.SetActive(true);
				lbLeftTimeWord.enabled = false;
				lbLeftTime.enabled = false;

				StartCoroutine(updateLeftTime(false));
			}

			lbEndDate.text = Util.getUIText("SPECIALITEM_ENDDATE", _data.endDate.Substring(4,2) , _data.endDate.Substring(6,2) );

			lbEndDate.enabled = true;
		}
		else
		{
			btnBuy.gameObject.SetActive(true);
			btnConsume.gameObject.SetActive(false);

			lbEndDate.enabled = false;
			lbLeftTimeWord.enabled = false;
			lbLeftTimeWord.enabled = false;
		}

		if(btnConsume.gameObject.activeSelf)
		{
			spRewardIcon.cachedGameObject.SetActive(true);

			for(int i = 0; i < _data.product.Length; ++i)
			{
				if(_data.product[i].StartsWith("RU"))
				{
					spRewardIcon.spriteName = WSDefine.ICON_REWARD_RUBY;
					_data.product[i].Substring(_data.product[i].IndexOf("_")+1);
					break;
				}
				else if(_data.product[i].StartsWith("GO"))
				{
					spRewardIcon.spriteName = WSDefine.ICON_REWARD_GOLD;
					_data.product[i].Substring(_data.product[i].IndexOf("_")+1);
					break;
				}
			}
		}
		else
		{
			spRewardIcon.cachedGameObject.SetActive(false);
		}


	}

	WaitForSeconds _ws1 = new WaitForSeconds(1.0f);

	IEnumerator updateLeftTime(bool updateText)
	{
		while(true)
		{
			DateTime dt = DateTime.Now;
			
			GameDataManager.instance.annuityUpdateTime.TryGetValue(_data.annuityId, out dt);
			
			TimeSpan ts = (DateTime.Now - dt);
			int leftTime = _data.remainedTime - (int)ts.TotalSeconds;

			if(leftTime < 0)
			{
				EpiServer.instance.sendRefreshAnnuity();
				break;
			}
			else
			{
				if(updateText) lbLeftTime.text = Util.secToHourMinuteSecondString(leftTime);
			}			

			yield return _ws1;
		}
	}

}


using UnityEngine;
using System.Collections;
using System;

public class UIMessageListSlotPanel : UIListGridItemPanelBase {

	public UIButton btnReceive;
	public UISprite spReceive;

	public PhotoDownLoader face;

	public UISprite spGiftIcon;
	public UISprite spMoneyIcon;

	public UILabel lbMessage;
	public UILabel lbGiftText;
	public UILabel lbLeftTime;

	public Transform tfLine1Container;


	// Use this for initialization
	protected override void initAwake ()
	{
		UIEventListener.Get(btnReceive.gameObject).onClick = onClickReceive;
	}
	
	void onClickReceive(GameObject go)
	{
		GameManager.me.uiManager.popupMessage.onReceiveItem(this, data.itemList);
	}


	static Vector3 _line1Pos = new Vector3(137,-10,0);
	static Vector3 _line2Pos = new Vector3(137,4,0);


	void setReceiveButton(bool isEnable, bool isUpdateType = false)
	{
		btnReceive.isEnabled = isEnable;
		if(isEnable)
		{
			spReceive.color = btnReceive.defaultColor;

			if(isUpdateType == false && (data.expiredTime <= 0 ))
			{
				tfLine1Container.localPosition = _line1Pos;
			}
			else
			{
				tfLine1Container.localPosition = _line2Pos;
			}
		}
		else
		{
			spReceive.color = btnReceive.disabledColor;
			tfLine1Container.localPosition = _line1Pos;
		}

	}


	public override void setPhotoLoad()
	{
		if(data == null) return;
		if(epi.GAME_DATA.appFriendDic.ContainsKey(data.senderId))
		{
			face.down(epi.GAME_DATA.appFriendDic[data.senderId].image_url);
		}
		else if(epi.GAME_DATA.friendDic.ContainsKey(data.senderId))
		{
			face.down(epi.GAME_DATA.friendDic[data.senderId].image_url);
		}
	}	

	public P_Message data;

	public override void setData(object obj)
	{
		data = GameDataManager.instance.messages[(string)obj];

		lbLeftTime.text = "";

		if(data.senderId == WSDefine.ADMIN_ID)
		{
			face.gameObject.SetActive(false);
		}
		else
		{
			face.gameObject.SetActive(true);

			if(epi.GAME_DATA.appFriendDic.ContainsKey(data.senderId))
			{
				face.init(epi.GAME_DATA.appFriendDic[data.senderId].image_url);
			}
			else if(epi.GAME_DATA.friendDic.ContainsKey(data.senderId))
			{
				face.init(epi.GAME_DATA.friendDic[data.senderId].image_url);
			}
			else
			{
				face.gameObject.SetActive(false);
			}
		}

		lbMessage.text = data.text;//Util.getText("UI_SEND_GIFT","홍길동");

		//public string[] itemList;

		bool isGift = true;

		if(data.itemList != null)
		{


			if(data.itemList.Length == 1)
			{
				switch(data.itemList[0].Substring(0,2))
				{
				case "EN":
					spGiftIcon.gameObject.SetActive(false);
					spMoneyIcon.gameObject.SetActive(true);
					spMoneyIcon.spriteName = WSDefine.ICON_ENERGY;
					lbGiftText.text = data.itemList[0].Substring(data.itemList[0].LastIndexOf("_")+1);
					isGift = false;
					break;
				case "RU":
					spGiftIcon.gameObject.SetActive(false);
					spMoneyIcon.gameObject.SetActive(true);
					spMoneyIcon.spriteName = WSDefine.ICON_RUBY;
					lbGiftText.text = data.itemList[0].Substring(data.itemList[0].LastIndexOf("_")+1);
					isGift = false;
					break;

				case "RS":
					spGiftIcon.gameObject.SetActive(false);
					spMoneyIcon.gameObject.SetActive(true);
					spMoneyIcon.spriteName = WSDefine.ICON_RUNESTONE;
					lbGiftText.text = data.itemList[0].Substring(data.itemList[0].LastIndexOf("_")+1);
					isGift = false;
					break;

				case "GO":
					spGiftIcon.gameObject.SetActive(false);
					spMoneyIcon.gameObject.SetActive(true);
					spMoneyIcon.spriteName = WSDefine.ICON_GOLD;

					int gold = 0;
					int.TryParse(data.itemList[0].Substring(data.itemList[0].LastIndexOf("_")+1), out gold);
					lbGiftText.text = Util.GetCommaScore(gold);
					isGift = false;
					break;
				}
			}

			if(isGift)
			{
				spGiftIcon.gameObject.SetActive(true);
				spMoneyIcon.gameObject.SetActive(false);
				lbGiftText.text = "";
			}

		}



		spMoneyIcon.MakePixelPerfect();

		if(data.expiredTime < 0)
		{
			check = false;
			setReceiveButton(true);
		}
		else
		{
			check = true;

			if(data.expiredTime <= 0)
			{
				setReceiveButton(false);
			}
			else
			{
				setReceiveButton(true);
			}
		}

		if(check == false || data.expiredTime <= 0)
		{
			if(isGift)
			{

			}
			else
			{

			}
		}


	}

	bool check = true;

	void Update()
	{
		if(check && data.expiredTime > 0)
		{
			TimeSpan ts = (DateTime.Now - GameDataManager.instance.msgCloseTime[data.id]);
			int leftTime = data.expiredTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = "-";
				check = false;
				setReceiveButton(false, true);
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime);
			}
		}
	}




}

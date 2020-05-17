using UnityEngine;
using System.Collections;
using System;

public class UIWorldMapFriendDetail : MonoBehaviour {
	
	public UIButton btnInfo, btnPVP, btnPoint;

	public UILabel lbFBWaitTime;
	
	public UISprite spBtnPointBackground, spArrowUp, spArrowDown;

	private bool _needUpdateTick = false;
	private float _delay = 0.0f;

	void Awake () 
	{
		UIEventListener.Get(btnInfo.gameObject).onClick = onClickInfo;
		UIEventListener.Get(btnPVP.gameObject).onClick = onClickPVP;
		UIEventListener.Get(btnPoint.gameObject).onClick = onClickPoint;
	}


	void OnDestroy()
	{
		btnInfo = null; 
		btnPVP = null;
		btnPoint = null;

		data = null;
		spBtnPointBackground = null;

		lbFBWaitTime = null;

	}


	void onClickInfo(GameObject go)
	{
		EpiServer.instance.sendGetFriendDetail(data.userId);
	}

	void onClickPVP(GameObject go)
	{
		int leftNum = GameDataManager.instance.friendlyPVPMax;

		if(GameDataManager.instance.friendlyPVPIds != null)
		{
			leftNum = GameDataManager.instance.friendlyPVPMax - GameDataManager.instance.friendlyPVPIds.Length;

			if(leftNum <= 0)
			{
				UISystemPopup.open(Util.getUIText("FRIENDLY_OVERLIMIT"));
				return;
			}
			else
			{
				foreach(string id in GameDataManager.instance.friendlyPVPIds)
				{
					if(id == data.userId)
					{
						UISystemPopup.open(Util.getUIText("FRIENDLY_ALREADY"));
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


	public P_FriendData data;



	UIFriendListPanel.PointButtonType pointType = UIFriendListPanel.PointButtonType.None;

	Vector3 _v;
	public void show(P_FriendData d, Vector3 screenPos, bool isDown = false)
	{


		gameObject.SetActive(true);

		_v = GameManager.me.uiManager.uiMenu.camera.ScreenToWorldPoint(screenPos);	
		_v.z = 0;

		if(isDown == false)
		{
			_v.y += 70.0f;
			spArrowUp.enabled = false;
			spArrowDown.enabled = true;
		}
		else
		{
			_v.y -= 70.0f;
			spArrowUp.enabled = true;
			spArrowDown.enabled = false;
		}

		transform.position = _v;

		data = d;

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
		btnPVP.isEnabled = canFight();

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
			pointType = UIFriendListPanel.PointButtonType.Get;
			btnPoint.isEnabled = true;
		}
		else
		{
			if(data.fpWaitingTime > 0)
			{
				TimeSpan ts = (DateTime.Now - GameDataManager.instance.friendPointRefreshTimer[data.userId]);
				int leftTime = data.fpWaitingTime - (int)ts.TotalSeconds;
				
				if(leftTime < 0)
				{
					_needUpdateTick = false;
					spBtnPointBackground.spriteName = "ibtn_view_spidle";
					lbFBWaitTime.gameObject.SetActive(false);
					pointType = UIFriendListPanel.PointButtonType.Send;
					btnPoint.isEnabled = true;
				}
				else
				{
					lbFBWaitTime.text = Util.secToHourMinuteSecondString(leftTime); 
					spBtnPointBackground.spriteName = "ibtn_send_spidle";
					lbFBWaitTime.gameObject.SetActive(true);
					pointType = UIFriendListPanel.PointButtonType.None;
					btnPoint.isEnabled = false;
				}
			}
			else
			{
				spBtnPointBackground.spriteName = "ibtn_view_spidle";
				lbFBWaitTime.gameObject.SetActive(false);		
				pointType = UIFriendListPanel.PointButtonType.Send;
				btnPoint.isEnabled = true;
			}
		}
	}

}

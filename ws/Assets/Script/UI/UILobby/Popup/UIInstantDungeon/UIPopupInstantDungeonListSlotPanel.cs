using UnityEngine;
using System.Collections;
using System;

public class UIPopupInstantDungeonListSlotPanel : UIListGridItemPanelBase {

	public UIButton btnInfo;

	public UIDragScrollView btnInfoScrollView;


	public UILabel lbTitle, lbGameMode, lbEnterNum, lbClearNum, lbAutoEnable, lbLeftTime;
	public UISprite spNew, spGameMode, spBackground, spAutoEnable, spSelectBorder;

	bool _setUIText = false;

	public UIDragScrollView scrollView;


	protected override void initAwake ()
	{
		UIEventListener.Get(btnInfo.gameObject).onClick = onClick;

		if(scrollView != null) scrollView.scrollView = GameManager.me.uiManager.popupInstantDungeon.list.panel;
	}

	public override void setPhotoLoad()
	{
	}	


	private P_Sigong _data;

	const string BACKGROUND_EASY = "img_listbox_dungeon1";
	const string BACKGROUND_NORMAL = "img_listbox_dungeon2";
	const string BACKGROUND_HARD = "img_listbox_dungeon3";
	const string BACKGROUND_EVENT = "img_listbox_dungeon4";

	public override void setData(object obj)
	{
		if(obj == null) return;
		_data = (P_Sigong)obj;

		switch(UIPopupInstantDungeon.type)
		{
		case UIPopupInstantDungeon.Type.Easy:
			spBackground.spriteName = BACKGROUND_EASY;
			break;
		case UIPopupInstantDungeon.Type.Normal:
			spBackground.spriteName = BACKGROUND_NORMAL;
			break;
		case UIPopupInstantDungeon.Type.Hard:
			spBackground.spriteName = BACKGROUND_HARD;
			break;
		case UIPopupInstantDungeon.Type.Event:
			spBackground.spriteName = BACKGROUND_EVENT;
			break;
		}


		if(_setUIText == false)
		{
			_setUIText = true;
			lbAutoEnable.text = Util.getUIText("CANT_AUTOPLAY");
		}

		lbTitle.text = _data.title;

		if(_data.enterMax == 0)
		{
			lbEnterNum.text = Util.getUIText("ENTER_NUM") + " -";
			lbEnterNum.alpha = 0.5f;
		}
		else
		{
			lbEnterNum.text = Util.getUIText("ENTER_NUM") +  " " + _data.enterCount + "/" + _data.enterMax;
			lbEnterNum.alpha = 1.0f;
		}


		if(_data.clearMax == 0)
		{
			lbClearNum.text = Util.getUIText("CLEAR_NUM") + " -";
			lbClearNum.alpha = 0.5f;
		}
		else
		{
			lbClearNum.text = Util.getUIText("CLEAR_NUM") + " " + _data.clearCount + "/" + _data.clearMax;
			lbClearNum.alpha = 1.0f;
		}


		lbAutoEnable.cachedGameObject.SetActive( _data.autoPlay == WSDefine.NO );

		spAutoEnable.cachedGameObject.SetActive( _data.autoPlay == WSDefine.NO );


		if(UIPopupInstantDungeon.checkNewDic.ContainsKey(_data.id))
		{
			spNew.enabled = false;
		}
		else
		{
			spNew.enabled = true;
			UIPopupInstantDungeon.checkNewDic.Add(_data.id, true);
		}

		refreshPanel();

		if(_data.roundId.StartsWith("PVP"))
		{
			lbGameMode.text = Util.getUIText("HERO_PVP");
			spGameMode.spriteName = "img_epicmode_boss";
		}
		else
		{
			switch( GameManager.info.roundData[_data.roundId].mode )
			{
			case RoundData.MODE.KILLEMALL: 
				lbGameMode.text = Util.getUIText("KILLEMALL");
				spGameMode.spriteName = "img_epicmode_kill";
				break;

			case RoundData.MODE.SURVIVAL: 
				lbGameMode.text = Util.getUIText("SURVIVAL"); 
				spGameMode.spriteName = "img_epicmode_survival";
				break;

			case RoundData.MODE.PROTECT: 
				lbGameMode.text = Util.getUIText("PROTECT"); 
				spGameMode.spriteName = "img_epicmode_protect";
				break;

			case RoundData.MODE.SNIPING: 
				lbGameMode.text = Util.getUIText("SNIPING");
				spGameMode.spriteName = "img_epicmode_boss";
				break;

			case RoundData.MODE.KILLCOUNT: 
				lbGameMode.text = Util.getUIText("KILLCOUNT");
				spGameMode.spriteName = "img_epicmode_killcount";
				break;

			case RoundData.MODE.KILLCOUNT2: 
				lbGameMode.text = Util.getUIText("KILLCOUNT");
				spGameMode.spriteName = "img_epicmode_killcount";
				break;

			case RoundData.MODE.ARRIVE: 
				lbGameMode.text = Util.getUIText("ARRIVE");
				spGameMode.spriteName = "img_epicmode_arrive";
				break;

			case RoundData.MODE.DESTROY: 
				lbGameMode.text = Util.getUIText("DESTROY");
				spGameMode.spriteName = "img_epicmode_destroy";
				break;

			case RoundData.MODE.GETITEM: 
				lbGameMode.text = Util.getUIText("GETITEM");
				spGameMode.spriteName = "img_epicmode_getitem";
				break;
			}
		}

		++_updateTimeIndex;

		lbLeftTime.text = "";

		if(string.IsNullOrEmpty(_data.enterDepId) == false && GameDataManager.instance.sigongList[_data.enterDepId].clearCount <= 0)
		{
			lbLeftTime.text = "이전 단계 던전을 클리어하시면 입장하실 수 있습니다.";
		}
		else
		{
			checkUpdateTime();
			StartCoroutine(updateTime(_updateTimeIndex));
		}
	}

	private int _updateTimeIndex = 0;


	Vector3 _v;
	private int _playIndex = 0;
	public override void refreshPanel ()
	{
		lbTitle.enabled = true;

		if(GameManager.me.uiManager.popupInstantDungeon.selectSigongData != null && GameManager.me.uiManager.popupInstantDungeon.selectSigongData.id == _data.id)
		{
			spSelectBorder.enabled = true;
			_v.x = 0;
			lbTitle.cachedTransform.localPosition = _v;

			++_playIndex;
			StartCoroutine(playTextAnimation(_playIndex));
		}
		else
		{
			++_playIndex;
			spSelectBorder.enabled = false;
			_v.x = 0;
			lbTitle.cachedTransform.localPosition = _v;
		}
	}

	//14
	private const int _titleWidth = 413;

	IEnumerator playTextAnimation(int playIndex)
	{
		yield return null;

		float printSize = lbTitle.printedSize.x;
		
		yield return Util.ws1;

		if(printSize > _titleWidth)
		{
			while(_playIndex == playIndex)
			{
				_v.x = lbTitle.cachedTransform.localPosition.x;
				_v.x -= 4.0f; 
				_v.y = 0;
				_v.z = 0;
				
				if(_v.x < -(printSize - _titleWidth))
				{
					yield return Util.ws1;
					if(_playIndex == playIndex) _v.x = 0;
					if(_playIndex == playIndex)lbTitle.enabled = false;
					yield return Util.ws02;
					if(_playIndex == playIndex)lbTitle.cachedTransform.localPosition = _v;
					if(_playIndex == playIndex)lbTitle.enabled = true;
					yield return Util.ws1;
				}
				else
				{
					yield return Util.ws005;
					if(_playIndex == playIndex)lbTitle.transform.localPosition = _v;
				}
			}
		}
	}




	public void onClick(GameObject go)
	{
		if(GameManager.me.uiManager.popupInstantDungeon.selectSigongData != null && GameManager.me.uiManager.popupInstantDungeon.selectSigongData.id == _data.id)
		{
			return;
		}

#if UNITY_EDITOR
		Debug.LogError("onClick index: " + index);
#endif

		GameManager.me.uiManager.popupInstantDungeon.setDungeon(_data, index);
	}


	IEnumerator updateTime(int updateTimeIndex)
	{
		bool stop = false;
		while(_updateTimeIndex == updateTimeIndex)
		{
			if(checkUpdateTime() == false && stop == false)
			{
				stop = true;
				yield break;
			}

			yield return Util.ws1;
		}
	}


	bool checkUpdateTime()
	{
		TimeSpan ts = (DateTime.Now - GameDataManager.instance.sigongTimer[_data.id]);
		
		int leftTime = 0;
		
		if(_data.enterWaitingTime > 0)
		{
			leftTime = _data.enterWaitingTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = string.Empty;
				return false;
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime, " 후 입장 가능");
			}
			
		}
		else if(_data.enterCoolTime > 0)
		{
			leftTime = _data.enterCoolTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = string.Empty;
				return false;
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime, " 후 재입장 가능");
			}
		}
		else if(_data.closingTime > 0)
		{
			leftTime = _data.closingTime - (int)ts.TotalSeconds;
			
			if(leftTime < 0)
			{
				lbLeftTime.text = string.Empty;
				GameManager.me.uiManager.popupInstantDungeon.requestSeverData();
				return false;
			}
			else
			{
				lbLeftTime.text = Util.secToDayHourMinuteSecondString2(leftTime);
			}
		}
		else
		{
			lbLeftTime.text = string.Empty;
			return false;
		}

		return true;
	}
	
}

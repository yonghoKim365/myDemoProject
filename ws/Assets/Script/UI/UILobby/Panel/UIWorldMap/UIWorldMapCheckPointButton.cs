using UnityEngine;
using System.Collections;

public class UIWorldMapCheckPointButton : MonoBehaviour {

	public UILabel lbStageNumber;
	public UISprite spButtonBackground;

	public UIButton btnButton;

	public int act = 1;
	public int stage = 1;


	void Awake()
	{
		btnButton = GetComponent<UIButton>();
		if(btnButton != null)
		{
			UIEventListener.Get(btnButton.gameObject).onClick = onClickStageButton;
		}
	}


	void onClickStageButton(GameObject go)
	{
		if(GameManager.me.uiManager.uiMenu.uiWorldMap.nowPlayingWalkAnimation) return;

		if(act > GameDataManager.instance.maxAct)
		{
			return;
		}
		else if(act == GameDataManager.instance.maxAct && stage > GameDataManager.instance.maxStage)
		{
			return;
		}

		if(act == GameDataManager.instance.maxAct && stage == GameDataManager.instance.maxStage)
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.openEpicPopup(true);
		}
		else
		{
			GameManager.me.uiManager.uiMenu.uiWorldMap.openEpicPopup(false, act,stage, 1);
		}

	}


	public enum Status
	{
		Current, Clear, Lock
	}

	Status _status;

	private bool _isInit = false;

	public Status status
	{
		set
		{
			_status = value;

			switch(value)
			{
			case Status.Clear:
				spButtonBackground.spriteName = "img_round_clear";
				lbStageNumber.gameObject.SetActive(true);
				lbStageNumber.color = new Color(0.898f,0.81177f,0.341f);
				break;
			case Status.Current:
				spButtonBackground.spriteName = "img_round_myhero";
				lbStageNumber.gameObject.SetActive(false);
				break;
			case Status.Lock:
				spButtonBackground.spriteName = "img_round_lock";
				lbStageNumber.gameObject.SetActive(true);
				lbStageNumber.color = new Color(0.58f,0.695f,0.7568f);
				break;
			}

			if(_isInit == false)
			{
				if(btnButton != null)
				{
					UIEventListener.Get(btnButton.gameObject).onDrag = GameManager.me.uiManager.uiMenu.uiWorldMap.onDragStage;
					UIEventListener.Get(btnButton.gameObject).onPress = GameManager.me.uiManager.uiMenu.uiWorldMap.OnPress;
				}
				_isInit = true;
			}

		}
		get
		{
			return _status;
		}
	}

}

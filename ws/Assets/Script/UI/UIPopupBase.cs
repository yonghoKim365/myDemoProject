using UnityEngine;
using System.Collections;

public abstract class UIPopupBase : MonoBehaviour {

	protected Vector3 _v;
	public Transform popupPanel;

	public UIButton btnClose;

	public bool useScaleTween = true;

	public Animation ani = null;

	public enum PopupSize
	{
		Big, Middle, Small, Unknown
	}

	public PopupSize popupSize = PopupSize.Middle;


	void Awake()
	{
		if(btnClose != null)
		{
			UIEventListener.Get(btnClose.gameObject).onClick = onClickClose;
		}

		awakeInit();
	}

	protected abstract void awakeInit();

	protected virtual void onClickClose(GameObject go)
	{
		switch(popupSize)
		{
		case PopupSize.Big:
			SoundData.play("uicm_close_big");
			break;
		case PopupSize.Middle:
			SoundData.play("uicm_close_mid");
			break;
		case PopupSize.Small:
			SoundData.play("uicm_close_sml");
			break;
		}

		hide();
	}



	private bool _isInitUI = false;
	protected void checkInitUI()
	{
		if(_isInitUI == false)
		{
			initUI();
			_isInitUI = true;
		}
	}

	protected virtual void initUI()
	{

	}

	public virtual void show()
	{
		checkInitUI();

		gameObject.SetActive(true);

		if(popupPanel != null && useScaleTween && ani != null && Time.timeScale >= 1.0f)
		{
			ani.Play();
		}

		switch(popupSize)
		{
		case PopupSize.Big:
			SoundData.play("uicm_popup_big");
			break;
		case PopupSize.Middle:
			SoundData.play("uicm_popup_mid");
			break;
		case PopupSize.Small:
			SoundData.play("uicm_popup_sml");
			break;
		}
	}
	
	public virtual void hide(bool isInit = false)
	{
		gameObject.SetActive(false);
	}


}

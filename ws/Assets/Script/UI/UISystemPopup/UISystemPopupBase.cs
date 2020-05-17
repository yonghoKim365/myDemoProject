using UnityEngine;
using System.Collections;

public abstract class UISystemPopupBase : MonoBehaviour {

	public UISystemPopup.PopupType popupType = UISystemPopup.PopupType.None;

	public enum PopupSize
	{
		Big, Middle, Small
	}
	
	public PopupSize popupSize = PopupSize.Middle;


	public UIButton btnClose;
	public UIButton btnYes;
	public UILabel lbMsg;

	public bool useScaleTween = true;
	public Transform popupPanel;
	
	public PopupData popupData = new PopupData();


	public GameObject container;
	
	protected Vector3 _v;

	public Animation ani = null;

	void Awake()
	{
		if(btnClose != null) UIEventListener.Get(btnClose.gameObject).onClick = onClose;
		if(btnYes != null) UIEventListener.Get(btnYes.gameObject).onClick = onYes;
		awakeInit();
	}

	protected virtual void awakeInit()
	{

	}

	protected virtual void onClose(GameObject go)
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
	
	public virtual void show(PopupData pd, string msg = "")
	{
		container.SetActive(true);
		gameObject.SetActive(true);

		popupData = pd;

		if(lbMsg != null) lbMsg.text = msg;

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

		if(popupPanel != null && useScaleTween && Time.timeScale >= 1.0f)
		{
			if(ani != null)
			{
				ani.Play();
			}
		}
	}

	
	public virtual void hide()
	{
		container.SetActive(false);
		gameObject.SetActive(false);
		UISystemPopup.closeNowPopup(popupData, false);
	}
	
	protected virtual void onYes(GameObject go)
	{
		container.SetActive(false);
		gameObject.SetActive(false);
		UISystemPopup.closeNowPopup(popupData, true);
	}
}

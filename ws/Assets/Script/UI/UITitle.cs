using UnityEngine;
using System.Collections;

public class UITitle : UIBase 
{
	public UIButton btnLogin;
	public UIButton btnGuest;




	public GameObject btnTouchNotice;


	void Awake()
	{
		showTouchNotice(false);

		UIEventListener.Get(btnLogin.gameObject).onClick = onClickLogin;
		UIEventListener.Get(btnGuest.gameObject).onClick = onClickGuest;
		hideButton();



	}


	public void showTouchNotice(bool visible)
	{
		btnTouchNotice.SetActive(visible);
		if(Weme.instance != null && Weme.instance.goProgressBarContainer != null) Weme.instance.goProgressBarContainer.SetActive(!visible);
	}



	public override void show ()
	{
		base.show ();
	}

	public override void hide ()
	{
		base.hide ();
		if(Weme.instance != null) Weme.instance.gameObject.SetActive(false);
	}

	
	public void hideButton()
	{
		btnLogin.gameObject.SetActive(false);
		btnGuest.gameObject.SetActive(false);
	}
	
	public void showButton()
	{
		btnLogin.gameObject.SetActive(true);



#if UNITY_IPHONE
		btnGuest.gameObject.SetActive(true);
#endif

//
//
//		Debug.LogError("여기 수정====");
//		Debug.LogError("여기 수정====");
//		Debug.LogError("여기 수정====");
//		Debug.LogError("여기 수정====");
//		Debug.LogError("여기 수정====");
//		Debug.LogError("여기 수정====");
//		btnGuest.gameObject.SetActive(true);

	}
	
	
	void onClickLogin(GameObject go)
	{
		GameManager.me.isGuest = false;

		GameDataManager.instance.deviceId = "";

		PandoraManager.instance.loginKakao();
	}
	
	
	void onClickGuest(GameObject go)
	{
		UISystemPopup.open( UISystemPopup.PopupType.YesNo, Util.getUIText("GUEST_START"), startGuestMode);
	}

	void startGuestMode()
	{
		hideButton();


		Debug.LogError("start Guest Mode");

		UIManager.setGameState(Util.getUIText("GUEST_LOGIN"));

		GameManager.me.isGuest = true;

		NativeManager.instance.setGuestMode();

		PandoraManager.instance.loginDevice();
	}


	public void close()
	{
		GameObject.Destroy(gameObject);
		GameManager.me.uiManager.uiTitle = null;
	}


	void OnDestroy()
	{
		if(Weme.instance != null)
		{
			GameObject.Destroy(Weme.instance.gameObject);
		}
	}

	
}


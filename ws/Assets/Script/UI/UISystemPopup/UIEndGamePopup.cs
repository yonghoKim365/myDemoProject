using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEndGamePopup : UISystemPopupBase 
{

	private static List<EndPopupData> _endPoupData = null;
	private static bool _initEndPopupData = false;
	private static int _totalChance = 100;

	public static void showEndPopup()
	{
		if(_initEndPopupData == false)
		{
			initEndPopupData();
			_initEndPopupData = true;
		}

		if(_endPoupData != null && _endPoupData.Count > 0)
		{

			int chance = UnityEngine.Random.Range(0, _totalChance) + 1;
			int nowChance = 0;

			for(int i = 0; i < _endPoupData.Count; ++i)
			{
				nowChance += _endPoupData[i].chance;

				if(nowChance >= chance)
				{
					UISystemPopup.open(UISystemPopup.PopupType.EndGame, _endPoupData[i].imgUrl, null, null, _endPoupData[i].packageName);
					return;
				}
			}
		}

		UISystemPopup.open(UISystemPopup.PopupType.YesNo, Util.getUIText("END_GAME"), GameManager.me.OnApplicationQuit, null);
	}



	static void initEndPopupData()
	{
		string data = Util.getUIText("ENDPOPUP");
		_totalChance = 0;

		if(string.IsNullOrEmpty(data) == false)
		{
			_endPoupData = new List<EndPopupData>();
			
			string[] d1 = data.Trim().Split('~');
			
			for(int i = 0; i < d1.Length; ++i)
			{
				string[] d2 = d1[i].Split('^');
				
				if(d2.Length == 3)
				{
					EndPopupData epd = new EndPopupData();
					epd.chance = -1;
					
					int.TryParse(d2[0], out epd.chance);
					epd.packageName = d2[1];
					epd.imgUrl = d2[2];
					
					if(epd.chance > 0 && string.IsNullOrEmpty(epd.packageName) == false && string.IsNullOrEmpty(epd.imgUrl) == false && epd.imgUrl.ToLower().StartsWith("http"))					
					{
						_totalChance += epd.chance;
						_endPoupData.Add(epd);
					}
				}
			}
		}
	}





	public PhotoDownLoader photo;

	public UIScrollView scrollView;

	public UILabel lbText;

	public GameObject spLoading;

	public UIButton btnClosePopup;

	public UIButton btnDetail;

	public UIButton btnConfirm;

	bool _isImageType = false;

	protected override void awakeInit ()
	{
		base.awakeInit ();

		photo.isLockMemoryUnload = true;

		UIEventListener.Get( btnClosePopup.gameObject ).onClick = onClickClose;
		UIEventListener.Get( btnDetail.gameObject ).onClick = onClickDetail;
		UIEventListener.Get( btnConfirm.gameObject ).onClick = onClickOk;
	}


	void onClickClose(GameObject go)
	{
		base.onClose (null);
	}




	void onClickDetail(GameObject go)
	{
		Util.openAppOrGoToMarket(_marketUrl);
	}



	void onClickOk(GameObject go)
	{
		GameManager.me.OnApplicationQuit();
	}

	private string _marketUrl = "com.linktomorrow.windrunner";

	public override void show (PopupData pd, string msg)
	{
		base.show (pd, "");

		_marketUrl = (string)pd.data[0];

		scrollView.ResetPosition();
		
		_isImageType = true;
		photo.gameObject.SetActive(true);
		lbText.enabled = false;
		spLoading.gameObject.SetActive( true );
		
		StartCoroutine(startDownload(msg));
	}



	IEnumerator startDownload(string msg)
	{
		yield return new WaitForSeconds(0.05f);

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
}


public class EndPopupData
{
	public string packageName;
	public string imgUrl;
	public int chance = 100;
}


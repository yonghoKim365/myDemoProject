using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpecialPackSlot : MonoBehaviour {

	public UILabel lbItemTitle, lbRubyCount, lbDescription, lbPrice;

	public UISprite spItemIcon, spItemPriceIcon, spMoneyType;

	public GameObject uiContainer;

	public PhotoDownLoader photo;

	public UIButton btn;

	void Awake () 
	{
		photo.isLockMemoryUnload = true;
		if (spMoneyType != null){
#if UNITY_IPHONE
		spMoneyType.spriteName = UIShopItemSlot.ICON_DOLLAR;
#else
		spMoneyType.spriteName = UIShopItemSlot.ICON_WON;
#endif
		}

		UIEventListener.Get(  btn.gameObject ).onClick = onClickButton;
	}

	Stack<string> _deleteKeys = new Stack<string>();

	void onClickButton(GameObject go)
	{
		EpiServer.instance.sendCheckProduct("PACKAGE", _data.id, _data.revision, false, "");

		if(GameDataManager.instance.packages != null)
		{
			string deleteKey = "";
			
			foreach(KeyValuePair<string, P_Package> kv in GameDataManager.instance.packages)
			{
				if(kv.Value.category == _data.category)
				{
					_deleteKeys.Push(kv.Key);
				}
			}
		}


		while(_deleteKeys.Count > 0)
		{
			string str = _deleteKeys.Pop();

			if(GameDataManager.instance.packages != null && GameDataManager.instance.packages.ContainsKey(str))
			{
				if(GameDataManager.instance.packagePriceInfo.ContainsKey(str) == false)
				{
					GameDataManager.instance.packagePriceInfo.Add(str, GameDataManager.instance.packages[str]);
				}
				else
				{
					GameDataManager.instance.packagePriceInfo[str] = GameDataManager.instance.packages[str];
				}

				GameDataManager.instance.packages.Remove(str);
			}
		}


		if(_closeCallback != null) _closeCallback();
		_closeCallback = null;
	}

	P_Package _data;

	Callback.Default _closeCallback;


	public bool isImageType
	{
		get
		{
			return (_data != null && string.IsNullOrEmpty( _data.imageUrl ) == false);
		}
	}

	public void setData(P_Package pData, Callback.Default closeCallback)
	{
		_data = pData;

		_closeCallback = closeCallback;

		if (lbPrice != null){
		#if UNITY_ANDROID
		lbPrice.text = Util.GetCommaScore( Mathf.RoundToInt( _data.price ) );
		#else
		lbPrice.text = string.Format("{0:F2}", _data.price);
		#endif
		}

		if(string.IsNullOrEmpty( _data.imageUrl ) == false)
		{
			uiContainer.gameObject.SetActive(false);
			photo.gameObject.SetActive(true);
			return;
		}

		photo.gameObject.SetActive(false);
		uiContainer.gameObject.SetActive(true);

		switch(_data.category)
		{
		case SpecialPackage.RUNE:

			if(_data.subcategory != null)
			{
				switch(_data.subcategory)
				{
				case SpecialPackage.UNIT:
					spItemIcon.spriteName = "img_package_animal";
					break;
				case SpecialPackage.SKILL:
					spItemIcon.spriteName = "img_package_skill";
					break;
				case SpecialPackage.EQUIP:
					spItemIcon.spriteName = "img_package_hero";
					break;
				}
			}

			break;
		case SpecialPackage.SPECIAL:

			if (spItemIcon != null){ 
				switch(_data.showWeight)
				{
				case 0:
					spItemIcon.spriteName = "img_package_special2";
					break;
				case 1:
					spItemIcon.spriteName = "img_package_special1";
					break;
				}
			}

			break;
		case SpecialPackage.STARTER:

			switch(_data.showWeight)
			{
			case 0:
				spItemIcon.spriteName = "img_package_start2";
				break;
			case 1:
				spItemIcon.spriteName = "img_package_start1";
				break;
			}

			break;
		}

	}



	IEnumerator startDownload(string url)
	{
		yield return Util.ws01;
		
		photo.init(url);
		photo.down(url);
		
		float timeout = 10.0f;
		while(timeout > 0 && ( photo.mainTexture == null || photo.mainTexture.enabled == false ) )
		{ 
			timeout -= 0.05f;
			yield return new WaitForSeconds(0.05f);
		}
	}


	public void loadImage()
	{
		if(_data == null || string.IsNullOrEmpty(_data.imageUrl) || photo.gameObject.activeSelf == false) return;
		StartCoroutine(startDownload(_data.imageUrl));
	}



}

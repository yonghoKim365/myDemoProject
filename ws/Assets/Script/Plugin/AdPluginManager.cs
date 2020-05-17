using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class AdPluginManager : MonoBehaviour {

	public static AdPluginManager instance = null;

	public static bool useLog = true;

	public static bool useAdbrix = true;

	public static bool useAdfit = true;

	public static bool useAppang = true;

	public static bool useGoogleTracking = true;



	private const string ADDFITID = "457487671879977";

	private const string APPANGID_ANDROID = "633d6847c630c3a1d293e13889bc5ab9";

	private const string APPANGID_IOS = "5de699035536919b1d9a55150463fb36";




	public const string GOOGLE_CONVERSION_ID = "960724292";



	public const string AdbrixAppKey = "518011218";
	public const string AdbrixHashKey = "fa1dad62ab0e4f3a";


	void Awake ()
	{
		useLog = Debug.isDebugBuild;

#if UNITY_IOS
		useAdbrix = false;
		useAdfit = false;
		useAppang = false;
		useGoogleTracking = false;
#endif


		if(instance == null)
		{
			GameObject.DontDestroyOnLoad(this.gameObject);
			instance = this;

			if(useGoogleTracking)
			{
				initGoogleTracking();
			}

			if(useAdbrix)
			{
#if UNITY_ANDROID
				IgaworksUnityPluginAOS.InitPlugin ();
				IgaworksUnityPluginAOS.Common.startApplication ();
#endif
			}
		}
		else
		{
			GameObject.Destroy(this.gameObject);
		}

	}

	// Use this for initialization
	void Start () 
	{

		if(useAdbrix)
		{
			#if UNITY_ANDROID
			IgaworksUnityPluginAOS.Common.startSession();
			#endif

#if UNITY_IPHONE
//			IgaworksADPluginIOS.SetCallbackHandler("AdPluginManager");						
//			IgaworksADPluginIOS.IgaworksADWithAppKey(AdbrixAppKey, AdbrixHashKey, false);
#endif
		}

#if UNITY_ANDROID
		if(useAdfit)
		{
			PandoraAdfitManager.instance.init(ADDFITID);
			PandoraAdfitManager.instance.actionComplete();
		}
#endif

		reportGoogleConversion(GoogleConversionData.INSTALL);
		reportGoogleConversion(GoogleConversionData.PLAY);
	}



	void OnApplicationPause(bool pauseStatus)
	{
		if(useAdbrix)
		{

	#if UNITY_ANDROID
			if (pauseStatus) {
//				Debug.Log ("go to Background");
				IgaworksUnityPluginAOS.Common.endSession();
			} else {
//				Debug.Log ("go to Foreground");
				IgaworksUnityPluginAOS.Common.startSession();
			}
	#endif
		}


		if(useGoogleTracking)
		{
			enableGoogleUsage();
		}
	} 






	#if UNITY_ANDROID

	AndroidJavaObject pluginObject;

	#endif

	public void getPluginClass()
	{
		#if UNITY_EDITOR
		
		#elif UNITY_ANDROID

		if(pluginObject != null) return;

		try
		{
			AndroidJavaClass ac = new AndroidJavaClass( "com.linktomorrow.windsoulplugin.WindSoulPlugin" );
			if(ac == null) return;
			pluginObject = ac.CallStatic<AndroidJavaObject>("getInstance");
		}
		catch
		{
			
		}
		#endif
	}


	public static void startAppang()
	{
		if(instance == null) return;

		instance.initAppang();
	}

	public void initAppang()
	{
		if(useAppang == false) return;

#if UNITY_EDITOR

#elif UNITY_ANDROID
		try
		{
			getPluginClass();
			if(pluginObject == null) return;
			pluginObject.Call ("initAppang", APPANGID_ANDROID, useLog);
		}
		catch
		{

		}
#endif
	}


	Dictionary<string, GoogleConversionData> _googleData = new Dictionary<string, GoogleConversionData>();

	public void initGoogleTracking()
	{
#if UNITY_ANDROID

		// 설치
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),960724292, "qxTfCK6-1VcQxPqNygM", "0.00", false);
		_googleData[GoogleConversionData.INSTALL] = new GoogleConversionData("qxTfCK6-1VcQxPqNygM", "0.00", false); 

		//실행
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),960724292, "pG2-CKvL6VcQxPqNygM", "0.00", true);	
		_googleData[GoogleConversionData.PLAY] = new GoogleConversionData( "pG2-CKvL6VcQxPqNygM", "0.00", true);

		//가입
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "AjeYCMit6FcQxPqNygM", "0.00", true);
		_googleData[GoogleConversionData.JOIN] = new GoogleConversionData( "AjeYCMit6FcQxPqNygM", "0.00", true);

		//루비27
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "eCxCCO726FcQxPqNygM", "3300.00", true);
		_googleData["ws_ruby_27"] = new GoogleConversionData( "eCxCCO726FcQxPqNygM", "3300.00", true);

		//루비90
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "6hzaCPH26FcQxPqNygM", "11000.00", true);
		_googleData["ws_ruby_90"] = new GoogleConversionData( "6hzaCPH26FcQxPqNygM", "11000.00", true);

		//루비300
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "w9FzCOWF6VcQxPqNygM", "33000.00", true);
		_googleData["ws_ruby_300"] = new GoogleConversionData( "w9FzCOWF6VcQxPqNygM", "33000.00", true);

		//루비540
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "D_G4CNez6FcQxPqNygM", "55000.00", true);
		_googleData["ws_ruby_540"] = new GoogleConversionData( "D_G4CNez6FcQxPqNygM", "55000.00", true);

		//루비1170  ws_ruby_1170  / 
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "Tun5CK6E6VcQxPqNygM", "110000.00", true);
		_googleData["ws_ruby_1170"] = new GoogleConversionData( "Tun5CK6E6VcQxPqNygM", "110000.00", true);

		//소울팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "XjfACPOh6VcQxPqNygM", "11000.00", true);
		_googleData["ws_package_starter"] = new GoogleConversionData( "XjfACPOh6VcQxPqNygM", "11000.00", true);

		//클로이팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "87GoCLmz6FcQxPqNygM", "33000.00", true);
		_googleData["ws_package_pwstarter"] = new GoogleConversionData( "87GoCLmz6FcQxPqNygM", "33000.00", true);

		//레오팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "8OdVCLyz6FcQxPqNygM", "55000.00", true);
		_googleData["ws_package_special"] = new GoogleConversionData( "8OdVCLyz6FcQxPqNygM", "55000.00", true);

		//카일리팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "e4tNCIGk6VcQxPqNygM", "110000.00", true);
		_googleData["ws_package_pwspecial"] = new GoogleConversionData( "e4tNCIGk6VcQxPqNygM", "110000.00", true);

		//소환팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "N8ghCJaE6VcQxPqNygM", "19800.00", true);
		_googleData["ws_package_summons"] = new GoogleConversionData( "N8ghCJaE6VcQxPqNygM", "19800.00", true);

		//스킬팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "8sMHCM2F6VcQxPqNygM", "19800.00", true);
		_googleData["ws_package_skill"] = new GoogleConversionData( "8sMHCM2F6VcQxPqNygM", "19800.00", true);

		//장비팩
		//AdWordsConversionReporter.reportWithConversionId(this.getApplicationContext(),"960724292", "Pgw3COiF6VcQxPqNygM", "19800.00", true);
		_googleData["ws_package_equip"] = new GoogleConversionData( "Pgw3COiF6VcQxPqNygM", "19800.00", true);

#elif UNITY_IPHONE
		/*
		// 설치
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"YDXgCPuM6VcQxPqNygM" value:@"0.00" isRepeatable:NO];
		_googleData[GoogleConversionData.INSTALL] = new GoogleConversionData("YDXgCPuM6VcQxPqNygM", "0.00", false); 
		
		
		//실행
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"njVLCI_X6FcQxPqNygM" value:@"0.00" isRepeatable:YES];
		_googleData[GoogleConversionData.PLAY] = new GoogleConversionData( "njVLCI_X6FcQxPqNygM", "0.00", true);
		
		
		//가입
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"Q77qCMuJ6VcQxPqNygM" value:@"0.00" isRepeatable:YES];
		_googleData[GoogleConversionData.JOIN] = new GoogleConversionData( "Q77qCMuJ6VcQxPqNygM", "0.00", true);
		
		
		//루비27
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"6Xd4CJmE6VcQxPqNygM" value:@"2.99" isRepeatable:YES];
		_googleData["ws_ruby_tier3"] = new GoogleConversionData("6Xd4CJmE6VcQxPqNygM","2.99", true);
		
		
		//루비90
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"uE-mCJyE6VcQxPqNygM" value:@"9.99" isRepeatable:YES];
		_googleData["ws_ruby_tier10"] = new GoogleConversionData( "uE-mCJyE6VcQxPqNygM","9.99",true);
		
		
		//루비300
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"jW3ECNOF6VcQxPqNygM" value:@"29.99" isRepeatable:YES];
		_googleData["ws_ruby_tier30"] = new GoogleConversionData( "jW3ECNOF6VcQxPqNygM" , "29.99", true);
		
		//루비540
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"h6elCOT-6FcQxPqNygM" value:@"49.99" isRepeatable:YES];
		_googleData["ws_ruby_tier50"] = new GoogleConversionData( "h6elCOT-6FcQxPqNygM" , "49.99", true);
		
		//루비1170  ws_ruby_1170  / 
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"dELSCLGE6VcQxPqNygM" value:@"99.99" isRepeatable:YES];
		_googleData["ws_ruby_tier60"] = new GoogleConversionData( "dELSCLGE6VcQxPqNygM" , "99.99", true);
		
		//소울팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"2UdpCOCz6FcQxPqNygM" value:@"9.99" isRepeatable:YES];
		_googleData["ws_package_ios_starter"] = new GoogleConversionData("2UdpCOCz6FcQxPqNygM" ,"9.99", true);
		
		//클로이팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"7ARkCJS16FcQxPqNygM" value:@"29.99" isRepeatable:YES];
		_googleData["ws_package_ios_pwstarter"] = new GoogleConversionData("7ARkCJS16FcQxPqNygM" ,"29.99", true);
		
		//레오팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"vw1aCLWN6VcQxPqNygM" value:@"49.99" isRepeatable:YES];
		_googleData["ws_package_ios_special"] = new GoogleConversionData("vw1aCLWN6VcQxPqNygM" ,"49.99", true);
		
		//카일리팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"WmQ3CJ2i6VcQxPqNygM" value:@"99.99" isRepeatable:YES];
		_googleData["ws_package_ios_pwspecial"] = new GoogleConversionData("WmQ3CJ2i6VcQxPqNygM" ,"99.99", true);
		
		//소환팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"iHcoCKiE6VcQxPqNygM" value:@"17.99" isRepeatable:YES];
		_googleData["ws_package_ios_summons"] = new GoogleConversionData("iHcoCKiE6VcQxPqNygM","17.99", true);
		
		//스킬팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"Euv5CPud6VcQxPqNygM" value:@"17.99" isRepeatable:YES];
		_googleData["ws_package_ios_skill"] = new GoogleConversionData("Euv5CPud6VcQxPqNygM" ,"17.99", true);
		
		//장비팩
		//[ACTConversionReporter reportWithConversionID:@"960724292" label:@"y6HCCLiN6VcQxPqNygM" value:@"17.99" isRepeatable:YES];
		_googleData["ws_package_ios_equip"] = new GoogleConversionData("y6HCCLiN6VcQxPqNygM" ,"17.99", true);
*/

#endif


	}


	public static void reportGoogleConversion(string id, string value = null)
	{
		if(instance != null)
		{
			instance._reportGoogleConversion( id,  value );
		}
	}

	public void _reportGoogleConversion(string id, string value = null)
	{
#if UNITY_EDITOR
		return;
#endif
		if(useGoogleTracking == false) return;

		if(_googleData.ContainsKey(id))
		{
			if(value != null) _googleData[id].value = value;

			
			try
			{
#if UNITY_ANDROID
				getPluginClass();
				if(pluginObject == null) return;

				if(Debug.isDebugBuild)
				{
					Debug.Log("reportGoogleRemarketing : " + GOOGLE_CONVERSION_ID + "  lb:" +  _googleData[id].label + "   v:" +  _googleData[id].value + "   rt:" + _googleData[id].isRepeat);
				}

				pluginObject.Call ("reportGoogleConversion", GOOGLE_CONVERSION_ID, _googleData[id].label, _googleData[id].value, _googleData[id].isRepeat, useLog);

#elif UNITY_IPHONE

//				_reportWithConversionID(GOOGLE_CONVERSION_ID, _googleData[id].label, _googleData[id].value, _googleData[id].isRepeat, useLog);

#endif

			}
			catch
			{
				Debug.LogError("report error....");
			}




		}
	}


	public static void reportGoogleRemarketing(string[] values)
	{
		if(instance != null)
		{
			instance._reportGoogleRemarketing( values );
		}
	}


	public void _reportGoogleRemarketing(string[] values)
	{
		if(values == null || values.Length % 2 != 0) return;

		if(useGoogleTracking == false) return;


		try
		{
			#if UNITY_ANDROID
			getPluginClass();
			if(pluginObject == null) return;

			if(Debug.isDebugBuild)
			{
				Debug.Log("reportGoogleRemarketing" + useLog + "  " +  GOOGLE_CONVERSION_ID + "   " +  values);
			}

			pluginObject.Call ("reportGoogleRemarketing", useLog, GOOGLE_CONVERSION_ID, values);

#else
			/*
			if(values.Length == 2)
			{
				_reportRemarketing1(GOOGLE_CONVERSION_ID, values[0], values[1], useLog);
			}
			if(values.Length == 4)
			{
				_reportRemarketing2(GOOGLE_CONVERSION_ID, values[0], values[1], values[2], values[3], useLog);
			}
			if(values.Length == 6)
			{
				_reportRemarketing3(GOOGLE_CONVERSION_ID, values[0], values[1], values[2], values[3], values[4], values[5], useLog);
			}
			if(values.Length == 8)
			{
				_reportRemarketing4(GOOGLE_CONVERSION_ID, values[0], values[1], values[2], values[3], values[4], values[5], values[6], values[7], useLog);
			}
			*/

			#endif

		}
		catch
		{
			Debug.LogError("re-report error....");
		}

	}


	void enableGoogleUsage()
	{

#if UNITY_
		try
		{
			getPluginClass();
			if(pluginObject == null) return;
			pluginObject.Call ("enableGoogleConversionUsage", GOOGLE_CONVERSION_ID, true);
		}
		catch
		{
			
		}
#endif
	}

	/*
#if UNITY_IPHONE

	[DllImport("__Internal")]
	private static extern void _reportWithConversionID(string conversionId, string label, string value, bool isRepeat, bool useLog);

	[DllImport("__Internal")]
	private static extern void _reportRemarketing1( string conversionId, string key, string value, bool useLog);

	[DllImport("__Internal")]
	private static extern void _reportRemarketing2( string conversionId, string key, string value, string key2, string value2, bool useLog);

	[DllImport("__Internal")]
	private static extern void _reportRemarketing3( string conversionId, string key, string value, string key2, string value2, string key3, string value3, bool useLog);

	[DllImport("__Internal")]
	private static extern void _reportRemarketing4( string conversionId, string key, string value, string key2, string value2, string key3, string value3, string key4, string value4, bool useLog);

#endif
*/
}




public class GoogleConversionData
{
	public const string INSTALL = "install";
	public const string PLAY = "play";
	public const string JOIN = "join";


	public string label;
	public string value;
	public bool isRepeat = true;

	public GoogleConversionData(string inputLabel, string inputValue, bool inputRepeat)
	{
		label = inputLabel;
		value = inputValue;
		isRepeat = inputRepeat;
	}

}


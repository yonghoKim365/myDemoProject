//#define TESTMODE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using WmWemeSDK.JSON;


public partial class AssetBundleManager : MonoBehaviour {

	public bool useLowResource = false;

	const float networkWaitTime = 0.3f;


	// 리소스 파일 루트 주소.
	public string resourcePath = "";//"http://office.linktomorrow.com/common/windsoul/";

	public const int MD5_CHECKSIZE = 10240;
	public const int CRC_CHECKSIZE = 10240;

	// 컷씬 팩.
	public const string CSP_NAME = "cs";

	// 게임 데이터 팩.
	public const string GD_PACKAGE_NAME = "gd";

	// 더미 데이터 
	public const string GDDM_PACKAGE_NAME = "dm.p12";
	
	public const string TUTORIAL_VOICE_PACKAGE_NAME = "st";
	public const string CUTSCENE_VOICE_PACKAGE_NAME = "sc";
	
	public const string GAME_MUSIC_PACKAGE_NAME = "b";
	public const string CUTSCENE_MUSIC_PACKAGE_NAME = "cb";


	public const string GAME_EFFECT_ZIP_PACKAGE_NAME = "ep";

	public const string NGUI_PACKAGE_NAME = "ng";
	public const string PLAYER_TEXTURE_PACKAGE_NAME = "pt";
	public const string GAME_EFFECT_PACKAGE_NAME = "ge";
	public const string CHARACTER_VOICE_PACKAGE_NAME = "cv";

	public const string ASSETBUNDLE = ".assetbundle";
	public const string DOWNLOAD_BUNDLE_EXTENSION_NAME = ".p12";
	public const string NORMAL_BUNDLE_EXTENSION_NAME = ".assetbundle";
	
	public const string P = "D`hnCP_2mX`Ssh3_X";
	
	public const int P_LENGH = 20811;
	
	public enum FOLDER_TYPE
	{
		Character, Map, Data, Pack, TutorialSound, CutSceneSound, GameMusic, CutSceneMusic, Sound, 
		NGUITexture, PlayerTexture, GameEffect, CharacterVoice, ETCPack
	}
	
	public static AssetBundleManager instance;

	public static string CURRENT_DATA_VERSION = "0";


	private static string _assetSavePath = null;

	public static string assetSavePath
	{
		get
		{
			if(Application.isPlaying)
			{
				if(_assetSavePath == null)
				{
					TextAsset info = Resources.Load("pandora/serverinfo") as TextAsset;
					JSONObject serverObj = JSONObject.Parse(info.ToString());
					string serverZoneStr = serverObj.GetString("server");
					JSONObject currentServer = serverObj.GetObject("epiServerdata").GetObject(serverZoneStr);
					_assetSavePath = Application.dataPath + "/BundleAsset/" + currentServer.GetString("version").Replace(".","_") + "/";
				}

				return _assetSavePath;
			}
			else
			{
				TextAsset info = Resources.Load("pandora/serverinfo") as TextAsset;
				JSONObject serverObj = JSONObject.Parse(info.ToString());
				string serverZoneStr = serverObj.GetString("server");
				JSONObject currentServer = serverObj.GetObject("epiServerdata").GetObject(serverZoneStr);
				return Application.dataPath + "/BundleAsset/" + currentServer.GetString("version").Replace(".","_") + "/";
			}
		}
	}


	public static string getAssetSavePath(string subPath, string fileName, bool useVersionPath = true)
	{
		string path = assetSavePath + subPath;

		if(useVersionPath == false)
		{
			path = Application.dataPath + "/BundleAsset/";
		}

		if(!Directory.Exists(path)) Directory.CreateDirectory(path);
		return path + fileName;
	}



	public static string getTutorialSoundBundlePath(string tutorialId, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath("STI/" , tutorialId + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath("STA/" , tutorialId + ".assetbundle");
			#endif
		}
		else
		{
#if UNITY_IOS
			return ResourceManager.getLocalFilePath("STI/" + tutorialId, ".assetbundle");
#else
			return ResourceManager.getLocalFilePath("STA/" + tutorialId, ".assetbundle");
#endif
		}
	}




	public static string getCutSceneSoundBundlePath(string csId, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath("SCI/" , csId + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath("SCA/" , csId + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath("SCI/" + csId, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath("SCA/" + csId, ".assetbundle");
			#endif
		}
	}


	public static string getCutSceneMusicBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath("CBI/" , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath("CBA/" , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath("CBI/" + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath("CBA/" + id, ".assetbundle");
			#endif
		}
	}


	public static string getGameMusicBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath("BI/" , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath("BA/" , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath("BI/" + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath("BA/" + id, ".assetbundle");
			#endif
		}
	}



	public static string getNGUIBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath("NGI/" , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath("NGA/" , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath("NGI/" + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath("NGA/" + id, ".assetbundle");
			#endif
		}
	}


	public static string getPlayerTextureBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath(ResourceManager.TEXTURE_IPHONE , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath(ResourceManager.TEXTURE_ANDROID , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_IPHONE + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_ANDROID + id, ".assetbundle");
			#endif
		}
	}




	public static string getEffectBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath(ResourceManager.EFFECT_IPHONE , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath(ResourceManager.EFFECT_ANDROID , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath(ResourceManager.EFFECT_IPHONE + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath(ResourceManager.EFFECT_ANDROID + id, ".assetbundle");
			#endif
		}
	}




	public static string getCharacterVoiceSoundBundlePath(string id, bool isLoadMode = false)
	{
		if(isLoadMode == false)
		{
			#if UNITY_IOS
			return AssetBundleManager.getAssetSavePath(ResourceManager.CHARACTERSOUND_IPHONE , id + ".assetbundle");
			#else
			return AssetBundleManager.getAssetSavePath(ResourceManager.CHARACTERSOUND_ANDROID , id + ".assetbundle");
			#endif
		}
		else
		{
			#if UNITY_IOS
			return ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_IPHONE + id, ".assetbundle");
			#else
			return ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_ANDROID + id, ".assetbundle");
			#endif
		}
	}




	
	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Destroy (gameObject);
		}
	}


	public static string bundleExtension
	{
		get
		{
			if(ResourceManager.instance != null && ResourceManager.instance.useAssetDownload)
			{
				return DOWNLOAD_BUNDLE_EXTENSION_NAME;
			}
			else
			{
				return NORMAL_BUNDLE_EXTENSION_NAME;
			}
		}
	}


	public enum ResourceType
	{
		Model, Effect, Map, Sound, Texture, UI, ETC
	}

	public static string getResourceName(string name, ResourceType resourceType)
	{
		if(ResourceManager.instance != null && ResourceManager.instance.useAssetDownload)
		{
			switch(resourceType)
			{
			case ResourceType.Model:
				name = GameManager.info.modelFileNames[name];
				break;

			case ResourceType.Effect:
				#if UNITY_EDITOR
//				Debug.Log("getResourceName : " + name);
				#endif

				if(EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA && GameManager.info.effectFileNames.ContainsKey(name) == false)
				{
					Debug.Log(name);
				}

				name = GameManager.info.effectFileNames[name];
				break;

			case ResourceType.Map:
				name = GameManager.info.mapFileNames[name];
				break;

			case ResourceType.Sound:
				name = GameManager.info.soundFileNames[name];
				break;

			case ResourceType.Texture:
				name = GameManager.info.textureFileNames[name];
				break;

			case ResourceType.UI:
				name = GameManager.info.uiFileNames[name];
				break;

			case ResourceType.ETC:
				name = GameManager.info.etcFileNames[name];
				break;
			}
		}




		return name;
	}



	Dictionary<string, string> fileList = new Dictionary<string, string>();

	bool loadFileList(byte[] bytes)
	{

		if(bytes == null) return false;


		try
		{
			using(MemoryStream m = new MemoryStream(bytes))
			{
				fileList = new Dictionary<string, string>();
				
				ZipInputStream zis = new ZipInputStream(m); // zip 파일에 입력하고..
				
				ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
				
				while ((ze = zis.GetNextEntry()) != null)
				{
					MemoryStream ms = new MemoryStream();

					if (!ze.IsDirectory)
					{
						byte[] buffer = new byte[2048];
						int len;

						while ((len = zis.Read(buffer, 0, buffer.Length)) > 0) 
						{ 
							ms.Write(buffer, 0, len); 
						};
					}
					
					ms.Position = 0;
					
					fileList.Add(ze.Name, Encoding.UTF8.GetString(ms.ToArray()));
				}
				
				zis.Close(); // 입력받은 녀석은 지우고...
				zis.Dispose();
				zis = null;
			}
		}
		catch(IOException e)
		{
			return false;
		}


		if(fileList == null || fileList.Count < 5 || fileList.ContainsKey("v") == false)
		{
			return false;
		}

		if(fileList.TryGetValue("v", out CURRENT_DATA_VERSION) == false)
		{
			return false;
		}

		return true;

	}

	bool _hasDownloadAsset = false;

	void onConfirmDownload()
	{
		_hasDownloadAsset = false;
	}

	public void makeDownloadFileList(int tryCount = 0)
	{
		UIManager.setGameState( Util.getUIText("CHECK_UPDATE") , 0.3f);
		_listDownloadTryCount = tryCount;
		StartCoroutine(startToMakeDownloadFileList());
	}

	void retryMakeDownloadFileList()
	{
		++_listDownloadTryCount;
		StartCoroutine(startToMakeDownloadFileList());
	}



	public float totalDownloadSize = 0;
	public float timeOut = 5.0f;
	int _listDownloadTryCount = 0;
	const int RETRY_LIMIT = 3;


	private int disconnectCheckCount = 0;
	void onRecheckNetworkConnection()
	{
#if UNITY_EDITOR
		if(Application.internetReachability == NetworkReachability.NotReachable || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Disconnected) 
#else
		if(Application.internetReachability == NetworkReachability.NotReachable)
#endif
		{
			disconnectCheckCount = 2;
			UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED"), onRecheckNetworkConnection, onRecheckNetworkConnection);
		}
		else disconnectCheckCount = 0;
	}




	// 패치 업데이트용 파일을 서버에서 다운로드 받는다.
	// fa는 안드로이드, fi는 아이폰용.
	IEnumerator startToMakeDownloadFileList()
	{
		GameManager.setTimeScale = 1.0f;

		while(Caching.ready == false) yield return null;

#if UNITY_EDITOR
		if(ResourceManager.instance.useAssetDownload == false) { GameManager.me.clientDataLoader.onCompleteResourceCheck(); yield break; }
#endif


#if UNITY_EDITOR
		if(Application.internetReachability == NetworkReachability.NotReachable || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Disconnected) 
#else
		if(Application.internetReachability == NetworkReachability.NotReachable)
#endif
		{
			disconnectCheckCount = 1;
			while(disconnectCheckCount > 0) { yield return new WaitForSeconds(1.0f); if(disconnectCheckCount == 1) onRecheckNetworkConnection(); }
		}

#if UNITY_ANDROID
		WWW www = new WWW(getResourcePath()+EpiServer.instance.clientVer.Replace(".","_")+"/fa" + EpiServer.instance.clientVer.Replace(".","_") + "");
#else
		WWW www = new WWW(getResourcePath()+EpiServer.instance.clientVer.Replace(".","_")+"/fi" + EpiServer.instance.clientVer.Replace(".","_") + "");
#endif
		float progressTime = 0.0f;

		float lastProgress = 0.0f;

		while(!www.isDone && www.error==null && progressTime<timeOut)
		{
			if(Mathf.Approximately( www.progress, lastProgress) ) progressTime += networkWaitTime;
			else progressTime = 0;

			lastProgress = www.progress;

			yield return new WaitForSeconds(networkWaitTime);
		}
#if UNITY_EDITOR
		if(progressTime>=timeOut || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Timeout)
#else
		if(progressTime>=timeOut)
#endif
		{
			www.Dispose();
			www = null;

			if(_listDownloadTryCount<RETRY_LIMIT)
			{
				retryMakeDownloadFileList();
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED")+"\n(error : 1)", retryMakeDownloadFileList, retryMakeDownloadFileList);
			}
			yield break;
		}
		
#if UNITY_EDITOR
		else if(www.error != null || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Error) 
#else
		else if(www.error != null)
#endif
		{
			www.Dispose();
			www = null;

			if(_listDownloadTryCount<RETRY_LIMIT)
			{
				retryMakeDownloadFileList();
			}
			else
			{
				UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED")+"\n(error : 2)", retryMakeDownloadFileList, retryMakeDownloadFileList);
			}
			yield break;
		}
		else if( www.isDone)
		{
			_hasDownloadAsset = false;
			totalDownloadSize = 0;

			ResourceManager.checkResourceFolderExists();


			if(loadFileList(www.bytes) == false)
			{
				// 패치 목록 파일을 받다가 에러가 났다.

				if(_listDownloadTryCount<RETRY_LIMIT)
				{
					retryMakeDownloadFileList();
				}
				else
				{
					UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED")+"\n(error : 5)", retryMakeDownloadFileList, retryMakeDownloadFileList);
				}

				yield break;
			}

			yield return null;


			//=========== 묶음 다운 로드 =============//

			UIManager.setGameState( Util.getUIText("CHECK_UPDATEPACK") , 0.4f);

			yield return null;

			checkStartPackDownloadAsset(_needPackDownloadAssetList);

			bool downloadPack = false;

			// 묶음 파일은 3개 이상일때만...
			if(_needPackDownloadAssetList.Count > 3)
			{
				yield return StartCoroutine( checkDownloadAsset(_needDataDownloadAssetList,FOLDER_TYPE.Data) );

				yield return StartCoroutine( checkDownloadAsset(_needSoundDownloadAssetList,FOLDER_TYPE.Sound));

				yield return StartCoroutine(checkDownloadAsset(_needNguiDownloadAssetList,FOLDER_TYPE.NGUITexture));

				yield return StartCoroutine(checkDownloadAsset(_needPlayerTextureDownloadAssetList,FOLDER_TYPE.PlayerTexture));

				yield return StartCoroutine(checkDownloadAsset(_needEffectDownloadAssetList,FOLDER_TYPE.ETCPack));

				yield return StartCoroutine(checkDownloadAsset(_needCharacterSoundDownloadAssetList,FOLDER_TYPE.CharacterVoice));

				downloadPack = true;

				_hasDownloadAsset = true;

#if UNITY_EDITOR
				Debug.LogError("totalDownloadSize : " + totalDownloadSize);
#endif

				if(totalDownloadSize >= 1.0f)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default,Mathf.RoundToInt(totalDownloadSize)+Util.getUIText("DOWNLOAD_RESOURCE"),onConfirmDownload, onConfirmDownload);
				}
				else
				{
					if(EpiServer.instance != null && EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
					{
						totalDownloadSize = (float)System.Math.Round(totalDownloadSize,2);
						UISystemPopup.open(UISystemPopup.PopupType.Default,string.Format("{0:0.00}",totalDownloadSize)+Util.getUIText("DOWNLOAD_RESOURCE"),onConfirmDownload, onConfirmDownload);
					}
					else
					{
						_hasDownloadAsset = false;
					}

				}

				while(_hasDownloadAsset)
				{
					yield return null;
				}

				if(_needPackDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needPackDownloadAssetList,FOLDER_TYPE.Pack,Util.getUIText("CHECK_RS"),true));
				}

				if(_needDataDownloadAssetList.Count > 0)
				{
					bool hasDownloadDummy = _needDataDownloadAssetList.Contains("dm");
					yield return StartCoroutine(downloadAsset(_needDataDownloadAssetList,FOLDER_TYPE.Data,Util.getUIText("CHECK_RS")+"2"));
					if(hasDownloadDummy) unzipDummy();
				}

				if(_needSoundDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needSoundDownloadAssetList,FOLDER_TYPE.Sound,Util.getUIText("CHECK_SD")));
				}



				if(_needNguiDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needNguiDownloadAssetList,FOLDER_TYPE.NGUITexture,Util.getUIText("CHECK_IC")));
				}
				
				
				if(_needPlayerTextureDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needPlayerTextureDownloadAssetList,FOLDER_TYPE.PlayerTexture,Util.getUIText("CHECK_WR")));
				}
				
				
				if(_needEffectDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needEffectDownloadAssetList,FOLDER_TYPE.ETCPack,Util.getUIText("CHECK_EF")));
				}


				if(_needCharacterSoundDownloadAssetList.Count > 0)
				{
					yield return StartCoroutine(downloadAsset(_needCharacterSoundDownloadAssetList,FOLDER_TYPE.CharacterVoice,Util.getUIText("CHECK_VC")));
				}

			}




			//=========== 개별 다운 로드 ============//
			_hasDownloadAsset = false;
			totalDownloadSize = 0;

			UIManager.setGameState(Util.getUIText("FIND_DATA") , 0.5f);

			yield return StartCoroutine( checkDownloadAsset(_needDataDownloadAssetList,FOLDER_TYPE.Data) );
			UIManager.setGameState( Util.getUIText("FIND_CHARACTER") , 0.6f, 1.4f);

			yield return StartCoroutine(checkDownloadAsset(_needCharacterDownloadAssetList,FOLDER_TYPE.Character));
			UIManager.setGameState( Util.getUIText("FIND_MAP") , 0.7f, 1.4f);

			yield return StartCoroutine(checkDownloadAsset(_needMapDownloadAssetList,FOLDER_TYPE.Map));

			if(downloadPack == false)
			{
				UIManager.setGameState( Util.getUIText("FIND_SOUND") , 0.8f);
				yield return StartCoroutine(checkDownloadAsset(_needSoundDownloadAssetList,FOLDER_TYPE.Sound));
				
				
				UIManager.setGameState( Util.getUIText("FIND_ICON") , 0.8f);
				yield return StartCoroutine(checkDownloadAsset(_needNguiDownloadAssetList,FOLDER_TYPE.NGUITexture));
				
				UIManager.setGameState( Util.getUIText("FIND_TEXTURE") , 0.8f);
				yield return StartCoroutine(checkDownloadAsset(_needPlayerTextureDownloadAssetList,FOLDER_TYPE.PlayerTexture));
				
				UIManager.setGameState( Util.getUIText("FIND_EFFECT") , 0.8f);
				yield return StartCoroutine(checkDownloadAsset(_needEffectDownloadAssetList,FOLDER_TYPE.ETCPack));
				
				UIManager.setGameState( Util.getUIText("FIND_VOICE") , 0.8f);
				yield return StartCoroutine(checkDownloadAsset(_needCharacterSoundDownloadAssetList,FOLDER_TYPE.CharacterVoice));
			}


			if(_needCharacterDownloadAssetList.Count > 0 || 
			   _needMapDownloadAssetList.Count > 0 || 
			   _needDataDownloadAssetList.Count > 0 ||
			   _needSoundDownloadAssetList.Count > 0 ||
			   _needNguiDownloadAssetList.Count > 0 ||
			   _needPlayerTextureDownloadAssetList.Count > 0 ||
			   _needEffectDownloadAssetList.Count > 0 ||
			   _needCharacterSoundDownloadAssetList.Count > 0 
			   )
			{
				_hasDownloadAsset = true;

				if(totalDownloadSize >= 1.0f)
				{
					UISystemPopup.open(UISystemPopup.PopupType.Default,Mathf.RoundToInt(totalDownloadSize)+Util.getUIText("DOWNLOAD_RESOURCE"),onConfirmDownload, onConfirmDownload);
				}
				else
				{
					if(EpiServer.instance != null && EpiServer.instance.targetServer == EpiServer.SERVER.ALPHA)
					{
						totalDownloadSize = (float)System.Math.Round(totalDownloadSize,2);
						UISystemPopup.open(UISystemPopup.PopupType.Default,string.Format("{0:0.00}",totalDownloadSize)+Util.getUIText("DOWNLOAD_RESOURCE"),onConfirmDownload, onConfirmDownload);
					}
					else
					{
						_hasDownloadAsset = false;
					}
				}
			}

			while(_hasDownloadAsset)
			{
				yield return null;
			}


			if(_needDataDownloadAssetList.Count > 0)
			{
				bool hasDownloadDummy = _needDataDownloadAssetList.Contains("dm");
				yield return StartCoroutine(downloadAsset(_needDataDownloadAssetList,FOLDER_TYPE.Data,Util.getUIText("DOWNLOAD_DATA")));
				if(hasDownloadDummy) unzipDummy();
			}

			if(_needCharacterDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needCharacterDownloadAssetList,FOLDER_TYPE.Character,Util.getUIText("DOWNLOAD_CHARACTER")));
			}

			if(_needMapDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needMapDownloadAssetList,FOLDER_TYPE.Map,Util.getUIText("DOWNLOAD_MAP")));
			}


			if(_needSoundDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needSoundDownloadAssetList,FOLDER_TYPE.Sound,Util.getUIText("DOWNLOAD_SOUND")));
			}



			if(_needNguiDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needNguiDownloadAssetList,FOLDER_TYPE.NGUITexture,Util.getUIText("DOWNLOAD_ICON")));
			}


			if(_needPlayerTextureDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needPlayerTextureDownloadAssetList,FOLDER_TYPE.PlayerTexture,Util.getUIText("DOWNLOAD_TEXTURE")));
			}


			if(_needEffectDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needEffectDownloadAssetList,FOLDER_TYPE.ETCPack,Util.getUIText("DOWNLOAD_EFFECT")));
			}


			if(_needCharacterSoundDownloadAssetList.Count > 0)
			{
				yield return StartCoroutine(downloadAsset(_needCharacterSoundDownloadAssetList,FOLDER_TYPE.CharacterVoice,Util.getUIText("DOWNLOAD_VOICE")));
			}
		}
		
		www.Dispose();
		www = null;


		// 컷씬 데이터는 매번 풀어준다...
		unzipCutSceneData();

		yield return null;

		if(unzipData() == false)
		{
			UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("DATA_ERROR","1"), NetworkManager.RestartApplication, NetworkManager.RestartApplication);
			yield break;
		}

		yield return null;

		if(GameManager.info.version != CURRENT_DATA_VERSION)
		{
			int patchErrorCount = PlayerPrefs.GetInt("PATCH_ERROR_COUNT", 0);

			if(patchErrorCount > 3)
			{
				deleteAllDownloadAssets();
				deleteTempData();
			}

			++patchErrorCount;

			PlayerPrefs.GetInt("PATCH_ERROR_COUNT", patchErrorCount);

			UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("DATA_ERROR","2"), NetworkManager.RestartApplication, NetworkManager.RestartApplication);
			yield break;
		}

		PlayerPrefs.SetInt("PATCH_ERROR_COUNT", 0);

		GameManager.me.clientDataLoader.onCompleteResourceCheck();
	}


	bool _waitingDownloadComplete = true;
	bool _waitingConfirmErrorPopup = false;


	private string _nowStateText = "";

	IEnumerator downloadAsset(Stack<string> stack, FOLDER_TYPE folderType, string statusWord = null, bool isPack = false)
	{
		GameManager.setTimeScale = 1.0f;

		int totalCount = stack.Count;
		int nowCount = 0;
		while(stack.Count > 0)
		{
			string n = stack.Pop();
			
			_waitingDownloadComplete = true;
			
			_listDownloadTryCount = 0;

			if(statusWord == null) _nowStateText = null;
			else _nowStateText = statusWord + " " +  (nowCount+1) +"/"+ totalCount;

			while(_waitingDownloadComplete)
			{
				if(isPack)
				{
					yield return StartCoroutine( startDownloadAsset(folderType, n, ".p12",false,true) );
				}
				else if(folderType == FOLDER_TYPE.Sound || folderType == FOLDER_TYPE.ETCPack)
				{
					yield return StartCoroutine( startDownloadAsset(folderType, n, ".p12",true,true,false) );
				}
				else
				{
					yield return StartCoroutine( startDownloadAsset(folderType, n, ".p12") );
				}

				++_listDownloadTryCount;
			}

			++nowCount;
		}
	}




	void retryDownloadAsset()
	{
		_waitingConfirmErrorPopup = false;
	}

	IEnumerator startDownloadAsset(FOLDER_TYPE folderType, string n, string webExtensionName, bool useMD5FileNameDic = true, bool unzipAndDelete = false, bool saveUnzipTrace = true)
	{
		GameManager.setTimeScale = 1.0f;

		_waitingConfirmErrorPopup = false;
		//string fileName = ResourceManager.getFolderName(folderType, false) + n + (folderType == FOLDER_TYPE.Character?"_base":"") + webExtensionName;

		string fileNameWithoutPath = "";

		if(useMD5FileNameDic == false)
		{
			fileNameWithoutPath = n;
		}
		else
		{
			switch(folderType)
			{
			case FOLDER_TYPE.Character:
				fileNameWithoutPath  = GameManager.info.modelFileNames[n];
				break;
			case FOLDER_TYPE.Map:
				fileNameWithoutPath  = GameManager.info.mapFileNames[n];
				break;
			case FOLDER_TYPE.Data:
				fileNameWithoutPath  = GameManager.info.etcFileNames[n];
				break;
			case FOLDER_TYPE.Pack:
				fileNameWithoutPath  = GameManager.info.etcFileNames[n];
				break;
			case FOLDER_TYPE.TutorialSound:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.CutSceneSound:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.GameMusic:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.CutSceneMusic:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.Sound:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.NGUITexture:
				fileNameWithoutPath  = GameManager.info.uiFileNames[n];
				break;
			case FOLDER_TYPE.PlayerTexture:
				fileNameWithoutPath  = GameManager.info.textureFileNames[n];
				break;
			case FOLDER_TYPE.GameEffect:
				fileNameWithoutPath  = GameManager.info.effectFileNames[n];
				break;
			case FOLDER_TYPE.CharacterVoice:
				fileNameWithoutPath  = GameManager.info.soundFileNames[n];
				break;
			case FOLDER_TYPE.ETCPack:
				fileNameWithoutPath  = GameManager.info.etcFileNames[n];
				break;
			}

		}



		string fileName = ResourceManager.getFolderName(folderType, false) + fileNameWithoutPath + webExtensionName;

		#if UNITY_EDITOR
		if(Application.internetReachability == NetworkReachability.NotReachable || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Disconnected) 
		#else
		if(Application.internetReachability == NetworkReachability.NotReachable)
		#endif
		{
			disconnectCheckCount = 1;
			
			while(disconnectCheckCount > 0)
			{
				yield return new WaitForSeconds(1.0f);
				if(disconnectCheckCount == 1) onRecheckNetworkConnection();
			}
		}

		string url = getResourcePath() +  fileName;

//		Debug.Log(resourcePath + "resource/" +  fileName + "   n: " + n);

		WWW www = new WWW(url);

		float progressTime = 0.0f;

		float _lastProgress = 0;
		while(!www.isDone && www.error==null && progressTime<timeOut)
		{
			if(_nowStateText != null)
			{
				UIManager.setGameState( _nowStateText + " (" + Mathf.Ceil(www.progress * 100.0f) + "%)" );
			}

			if(Mathf.Approximately(_lastProgress,www.progress))  progressTime += 0.1f;
			else progressTime = 0;

			_lastProgress = www.progress;
			yield return new WaitForSeconds(0.1f);
		}
#if UNITY_EDITOR
		if(progressTime>=timeOut || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Timeout)
#else
		if(progressTime>=timeOut)
#endif
		{
			www.Dispose();
			www = null;
			
			if(_listDownloadTryCount >= RETRY_LIMIT)
			{
#if UNITY_EDITOR
				Debug.Log(getResourcePath() +  fileName + "   n: " + n);
#endif

				_waitingConfirmErrorPopup = true;
				UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED")+"\n(error : 3)", retryDownloadAsset, retryDownloadAsset);
			}

			while(_waitingConfirmErrorPopup)
			{
				yield return new WaitForSeconds(networkWaitTime);
			}

			yield break;
		}
		
#if UNITY_EDITOR
		else if(www.error != null || DebugManager.instance.networkSimuationType == DebugManager.NetworkSimulationType.Error) 
#else
		else if(www.error != null)
#endif
		{
			www.Dispose();
			www = null;
			
			if(_listDownloadTryCount >= RETRY_LIMIT)
			{
				#if UNITY_EDITOR
				Debug.Log(getResourcePath() +  fileName + "   n: " + n);
				#endif

				_waitingConfirmErrorPopup = true;
				UISystemPopup.open(UISystemPopup.PopupType.SystemError, Util.getUIText("NET_DISCONNECTED")+"\n(error : 4)", retryDownloadAsset, retryDownloadAsset);
			}

			while(_waitingConfirmErrorPopup)
			{
				yield return new WaitForSeconds(networkWaitTime);
			}

			yield break;
		}
		else if( www.isDone)
		{
			fileName = ResourceManager.getFolderName(folderType,true) + fileNameWithoutPath + bundleExtension;

			if(unzipAndDelete == false)
			{
				File.WriteAllBytes( ResourceManager.getLocalFilePath( fileName, "", false), www.bytes);
			}
			else
			{
				if(_nowStateText != null)
				{
					UIManager.setGameState(_nowStateText + "( "+Util.getUIText("EXTRACT")+")");
				}

				yield return null;

				n = n.ToUpper();

				// 모델링 파일.
				if(n.StartsWith("A") || n.StartsWith("I") )
				{
					Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Character,true),"",false));
				}
				// 맵 파일.
				else if(n.StartsWith("M"))
				{
					Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Map,true),"",false));
				}
				// 사운드 파일.
				else if(n.StartsWith("S") || n.StartsWith("B") || n.StartsWith("CB") )
				{
					// 사운드 파일은 원본을 복사해서 다운로드 해시 체크를 하고
					// 압축도 풀어놔야한다.
					if(n.StartsWith("SC")) // 컷씬.
					{
						Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.CutSceneSound,true),"",false));
					}
					else if(n.StartsWith("ST")) // 튜토리얼
					{
						Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.TutorialSound,true),"",false));
					}
					else if(n.StartsWith("B")) // 게임음악
					{
						Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.GameMusic,true),"",false));
					}
					else if(n.StartsWith("CB")) // 컷씬음악
					{
						Util.extractZipByteFile( www.bytes, null, ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.CutSceneMusic,true),"",false));
					}

					fileName = ResourceManager.getFolderName(FOLDER_TYPE.Sound,true) + fileNameWithoutPath + bundleExtension;
					File.WriteAllBytes( ResourceManager.getLocalFilePath( fileName, "", false), www.bytes);
					
				}
				else if( n.StartsWith("GE"))
				{
					if(n.StartsWith("GE")) // 게임이펙트
					{
						string effectPath = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.GameEffect,true),"",false);

						// 이미 파일들이 존재한다면 얘들은 구버전인거다.. 그러니까 싹 다 지운다.
						// 어차피 개별파일을 다운로드하는게 아니라 압축 팩을 푸는거니까 다 지워도 상관없다.
						if(Directory.Exists(effectPath))
						{
							string[] deleteFiles = Directory.GetFiles(effectPath);

							if(deleteFiles != null)
							{
								for(int i = deleteFiles.Length - 1; i >= 0; --i)
								{
									if(File.Exists(deleteFiles[i]))
									{
										File.Delete(deleteFiles[i]);
									}
								}
							}
						}

						Util.extractZipByteFile( www.bytes, null, effectPath);
					}

					fileName = ResourceManager.getFolderName(FOLDER_TYPE.ETCPack,true) + fileNameWithoutPath + bundleExtension;
					File.WriteAllBytes( ResourceManager.getLocalFilePath( fileName, "", false), www.bytes);
				}



				string traceName = ResourceManager.getFolderName(folderType,true) + fileNameWithoutPath ;
				traceName = ResourceManager.getLocalFilePath(traceName,".zzz",false);
				PlayerPrefs.SetInt("PACK"+fileNameWithoutPath,1);
				if(saveUnzipTrace) File.WriteAllText(traceName,"");
			}

			_waitingDownloadComplete = false;
		}

		www.Dispose();
		www = null;
	}


	Stack<string> _needCharacterDownloadAssetList = new Stack<string>();
	Stack<string> _needMapDownloadAssetList = new Stack<string>();
	Stack<string> _needDataDownloadAssetList = new Stack<string>();
	Stack<string> _needPackDownloadAssetList = new Stack<string>();
	Stack<string> _needSoundDownloadAssetList = new Stack<string>();

	Stack<string> _needNguiDownloadAssetList = new Stack<string>();
	Stack<string> _needPlayerTextureDownloadAssetList = new Stack<string>();
	Stack<string> _needEffectDownloadAssetList = new Stack<string>();
	Stack<string> _needCharacterSoundDownloadAssetList = new Stack<string>();



	IEnumerator checkDownloadAsset(Stack<string> list, FOLDER_TYPE folderType)
	{
		GameManager.setTimeScale = 1.0f;

		list.Clear();
		
		string[] files = Directory.GetFiles(ResourceManager.getLocalFilePath(ResourceManager.getFolderName(folderType,true),"",false),"*"+bundleExtension);
		string[] fileNames = new string[files.Length];
		
		for(int i = files.Length - 1; i >= 0; --i)
		{
			fileNames[i] = Path.GetFileNameWithoutExtension( files[i] );
		}

		string listKey = "";

		switch(folderType)
		{
		case FOLDER_TYPE.Character:
			listKey = "cd";
			break;
		case FOLDER_TYPE.Map:
			listKey = "md";
			break;
		case FOLDER_TYPE.Data:
			listKey = "dd";
			break;

		case FOLDER_TYPE.Sound:
			listKey = "sd";
			break;





		case FOLDER_TYPE.TutorialSound:
			listKey = TUTORIAL_VOICE_PACKAGE_NAME;
			break;

		case FOLDER_TYPE.CutSceneSound:
			listKey = CUTSCENE_VOICE_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.CutSceneMusic:
			listKey = CUTSCENE_MUSIC_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.GameMusic:
			listKey = GAME_MUSIC_PACKAGE_NAME;
			break;




		case FOLDER_TYPE.ETCPack:
			listKey = GAME_EFFECT_ZIP_PACKAGE_NAME;
			break;


		case FOLDER_TYPE.NGUITexture:
			listKey = NGUI_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.PlayerTexture:
			listKey = PLAYER_TEXTURE_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.GameEffect:
			listKey = GAME_EFFECT_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.CharacterVoice:
			listKey = CHARACTER_VOICE_PACKAGE_NAME;
			break;

		}

		Dictionary<string, object> cd = MiniJSON.Json.Deserialize(fileList[listKey]) as Dictionary<string, object>;
		
		string checkFileName = "";

		int checkIndex = 0;

#if UNITY_EDITOR
		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		sw.Start();
#endif

		Dictionary<string, string> fileNameDic = getFileNameDic(folderType);



		foreach(KeyValuePair<string, object> kv in cd)
		{
			++ checkIndex;

			switch(folderType)
			{
			case FOLDER_TYPE.Character: if(checkIndex % 20 == 0) { System.GC.Collect();  yield return null; }; break;
			case FOLDER_TYPE.Map: if(checkIndex % 10 == 0) { System.GC.Collect(); yield return null; }; break;
			case FOLDER_TYPE.Sound: yield return null; break;
			}

			long assetSize = 0;
			string[] checkData = ((string)kv.Value).Split('&');
			long.TryParse(checkData[0], out assetSize);
			bool hasFile = false;

			fileNameDic[kv.Key] = checkData[1].Replace("/","_");

			for(int i = fileNames.Length - 1; i >= 0; --i)
			{
				if(fileNames[i] == fileNameDic[kv.Key])
				{
					hasFile = true;
					checkFileName = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(folderType,true)+checkData[1],AssetBundleManager.bundleExtension,false);
					break;
				}
			}


			if(folderType == FOLDER_TYPE.Sound)
			{
				if(kv.Key.ToLower().StartsWith(TUTORIAL_VOICE_PACKAGE_NAME))
				{
					if(checkDownloadAssetBySize(FOLDER_TYPE.TutorialSound) == false) hasFile = false;
				}
				else if(kv.Key.ToLower().StartsWith(CUTSCENE_VOICE_PACKAGE_NAME))
				{
					if(checkDownloadAssetBySize(FOLDER_TYPE.CutSceneSound) == false) hasFile = false;
				}
				else if(kv.Key.ToLower().StartsWith(GAME_MUSIC_PACKAGE_NAME))
				{
					if(checkDownloadAssetBySize(FOLDER_TYPE.GameMusic) == false) hasFile = false;
				}
				else if(kv.Key.ToLower().StartsWith(CUTSCENE_MUSIC_PACKAGE_NAME))
				{
					if(checkDownloadAssetBySize(FOLDER_TYPE.CutSceneMusic) == false) hasFile = false;
				}
			}
			else if(folderType == FOLDER_TYPE.ETCPack)
			{
				if(kv.Key.ToLower().StartsWith(GAME_EFFECT_PACKAGE_NAME))
				{
					if(checkDownloadAssetBySize(FOLDER_TYPE.GameEffect, ".p12", true) == false)
					{
						setHashFileName(FOLDER_TYPE.GameEffect);
						hasFile = false;
					}
				}
			}


			if(hasFile)
			{
				// 사운드 팩은 has 체크 안함. 파일이 있는지만 본다. 어차피 개별 파일도 다 체크하기 때문.
				if(folderType == FOLDER_TYPE.Sound) continue;

				// 이펙트는 md5 검사로 바꾸었다.
				if(folderType == FOLDER_TYPE.ETCPack && kv.Key.ToLower().StartsWith(GAME_EFFECT_PACKAGE_NAME) == false ) continue;

				string localHash = Util.getMD5HashFromFile(checkFileName);

				if(assetSize == Util.getFileSize(checkFileName) && checkData[1] == localHash)
				{
					continue;
				}
			}
			
			if(list.Contains(kv.Key) == false)
			{
				list.Push(kv.Key);
				totalDownloadSize += ((float)assetSize)/1048576.0f;
			}
		}

		#if UNITY_EDITOR

		Debug.Log(listKey + " " + sw.Elapsed);

		#endif

		System.GC.Collect();

	}




	string getListKey(FOLDER_TYPE folderType)
	{
		string listKey = "";

		switch(folderType)
		{
		case FOLDER_TYPE.Character:
			listKey = "cd";
			break;
		case FOLDER_TYPE.Map:
			listKey = "md";
			break;
		case FOLDER_TYPE.Data:
			listKey = "dd";
			break;
		case FOLDER_TYPE.Sound:
			listKey = "sd";
			break;
		case FOLDER_TYPE.TutorialSound:
			listKey = TUTORIAL_VOICE_PACKAGE_NAME;
			break;
		case FOLDER_TYPE.CutSceneSound:
			listKey = CUTSCENE_VOICE_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.CutSceneMusic:
			listKey = CUTSCENE_MUSIC_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.GameMusic:
			listKey = GAME_MUSIC_PACKAGE_NAME;
			break;
			
			
		case FOLDER_TYPE.ETCPack:
			listKey = GAME_EFFECT_ZIP_PACKAGE_NAME;
			break;
			
			
		case FOLDER_TYPE.NGUITexture:
			listKey = NGUI_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.PlayerTexture:
			listKey = PLAYER_TEXTURE_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.GameEffect:
			listKey = GAME_EFFECT_PACKAGE_NAME;
			break;
			
		case FOLDER_TYPE.CharacterVoice:
			listKey = CHARACTER_VOICE_PACKAGE_NAME;
			break;
		}

		return listKey;

	}


	void setHashFileName(FOLDER_TYPE folderType, bool useHashName = true)
	{
		string listKey = getListKey(folderType);
		
		Dictionary<string, string> fileNameDic = getFileNameDic(folderType);

		Dictionary<string, object> cd = MiniJSON.Json.Deserialize(fileList[listKey]) as Dictionary<string, object>;
		
		string checkFileName = "";
		
		foreach(KeyValuePair<string, object> kv in cd)
		{
			long assetSize = 0;
			string[] checkData = ((string)kv.Value).Split('&');
			long.TryParse(checkData[0], out assetSize);
			bool hasFile = false;
			
			// 일단 해쉬이름을 얘는 안쓰는데... 쓸까 말까...
			if(useHashName)
			{
				fileNameDic[kv.Key] = checkData[1].Replace("/","_");
			}
			else
			{
				fileNameDic[kv.Key] = kv.Key;//checkData[1].Replace("/","_");
			}
		}
	}



	bool checkDownloadAssetBySize(FOLDER_TYPE folderType, string extensionName = ".assetbundle", bool useHashName = false)
	{
		string tn = ResourceManager.getFolderName(folderType,true);

//		Debug.Log(tn);

		tn = ResourceManager.getLocalFilePath(tn,"",false);

		string[] files = Directory.GetFiles(tn,"*"+extensionName);
		string[] fileNames = new string[files.Length];
		
		for(int i = files.Length - 1; i >= 0; --i)
		{
			fileNames[i] = Path.GetFileNameWithoutExtension( files[i] );
		}
		
		string listKey = getListKey(folderType);

		Dictionary<string, string> fileNameDic = getFileNameDic(folderType);


		Dictionary<string, object> cd = MiniJSON.Json.Deserialize(fileList[listKey]) as Dictionary<string, object>;
		
		string checkFileName = "";
		
		foreach(KeyValuePair<string, object> kv in cd)
		{
			long assetSize = 0;
			string[] checkData = ((string)kv.Value).Split('&');
			long.TryParse(checkData[0], out assetSize);
			bool hasFile = false;

			// 일단 해쉬이름을 얘는 안쓰는데... 쓸까 말까...
			if(useHashName)
			{
				fileNameDic[kv.Key] = checkData[1].Replace("/","_");
			}
			else
			{
				fileNameDic[kv.Key] = kv.Key;//checkData[1].Replace("/","_");
			}


			for(int i = fileNames.Length - 1; i >= 0; --i)
			{
				if(fileNames[i] == fileNameDic[kv.Key])
				{
					hasFile = true;

					if(useHashName)
					{
						checkFileName = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(folderType,true)+checkData[1],extensionName,false);
					}
					else
					{
						checkFileName = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(folderType,true)+kv.Key,extensionName,false);
					}

					break;
				}
			}

			if(hasFile)
			{
				if(assetSize == Util.getFileSize(checkFileName))
				{
					continue;
				}
			}

#if UNITY_EDITOR
			Debug.Log("hasFile : " + hasFile + "   " + fileNameDic[kv.Key] + "  " + checkFileName + "   " + assetSize + "   " + folderType);
#endif

			return false;

		}

		return true;
	}



	public void deleteAllDownloadAssets()
	{
		List<string> deleteFiles = new List<string>();

		string[] files = Directory.GetFiles(ResourceManager.getLocalFilePath("","",false),"*.p12", SearchOption.AllDirectories);


		for(int i = 0; i < 20; ++i)
		{
			PlayerPrefs.SetInt("PACK"+"A"+i,-1);
			PlayerPrefs.SetInt("PACK"+"M"+i,-1);
		}


		if(files != null)
		{
			deleteFiles.AddRange(files);
		}

		files = Directory.GetFiles(ResourceManager.getLocalFilePath("","",false),"*.assetbundle", SearchOption.AllDirectories);

		if(files != null)
		{
			deleteFiles.AddRange(files);
		}


		if(files != null)
		{
			deleteFiles.AddRange(files);
		}
		
		files = Directory.GetFiles(ResourceManager.getLocalFilePath("","",false),"*.zzz", SearchOption.AllDirectories);


		if(files != null)
		{
			deleteFiles.AddRange(files);
		}


		for(int i = deleteFiles.Count - 1; i >= 0; --i)
		{
			File.Delete(deleteFiles[i]);
		}

	}




	void checkStartPackDownloadAsset(Stack<string> list)
	{
		list.Clear();
		
		string[] files = Directory.GetFiles(ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Pack,true),"",false),"*"+".zzz");
		string[] fileNames = new string[files.Length];
		
		for(int i = files.Length - 1; i >= 0; --i)
		{
			fileNames[i] = Path.GetFileNameWithoutExtension( files[i] );
		}
		
		string listKey = "pd";

		Dictionary<string, object> cd = MiniJSON.Json.Deserialize(fileList[listKey]) as Dictionary<string, object>;
		
		string checkFileName = "";
		
		foreach(KeyValuePair<string, object> kv in cd)
		{
			long assetSize = 0;
			string[] checkData = ((string)kv.Value).Split('&');
			long.TryParse(checkData[0], out assetSize);
			bool hasFile = false;
			
			for(int i = fileNames.Length - 1; i >= 0; --i)
			{
				if(fileNames[i] == kv.Key)
				{
					hasFile = true;
					break;
				}
			}

			if(hasFile == false)
			{
				hasFile = (PlayerPrefs.GetInt("PACK"+kv.Key,-1) == 1);

				if(list.Contains(kv.Key) == false && hasFile == false)
				{
					list.Push(kv.Key);
					totalDownloadSize += ((float)assetSize)/1048576.0f;
				}
			}
		}
	}











	static string _dataPath = "";
	
	public static string getTextAssetDataFromLocal(string fileName, bool ignoreAssetDownload = false, string prefix = "", string extension = ".txt")
	{
		_dataPath = string.Empty;

#if TESTMODE
		if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.REAL)
		{
			_dataPath = getLocalFilePath() + prefix;
			if(Directory.Exists(_dataPath) == false)
			{
				Directory.CreateDirectory(_dataPath);
			}

			if(File.Exists(_dataPath + "/" + fileName + extension))
			{
				return File.ReadAllText(_dataPath + "/" + fileName + extension);
			}
		}
#endif
		
		if(ResourceManager.instance != null && ResourceManager.instance.useAssetDownload && ignoreAssetDownload == false)
		{
//			Debug.Log(fileName);

			string keyName = Util.getEncryptFileName(fileName);

//			Debug.Log(keyName);

			string data = Util.decByteAndConvertToString(GameManager.info.dataStreamDic[keyName]);

			GameManager.info.dataStreamDic[keyName] = null;
			GameManager.info.dataStreamDic.Remove(keyName);

			if(data != null) return data;
		}
#if UNITY_EDITOR
		else
		{
			_dataPath = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+fileName,extension,false);
			
			if(File.Exists(_dataPath))
			{
				return File.ReadAllText(_dataPath);
			}
		}
#endif



		return null;
	}



	public static string getCutSceneTextAssetDataFromLocal(string fileName, bool ignoreAssetDownload = false, string prefix = "", string extension = ".txt")
	{
		_dataPath = string.Empty;
		
		if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.REAL)
		{
			_dataPath = getLocalFilePath() + prefix;
			if(Directory.Exists(_dataPath) == false)
			{
				Directory.CreateDirectory(_dataPath);
			}
			
			
			if(File.Exists(_dataPath + "/" + fileName + extension))
			{
				return File.ReadAllText(_dataPath + "/" + fileName + extension);
			}
		}
		
		if(ResourceManager.instance != null && ResourceManager.instance.useAssetDownload && ignoreAssetDownload == false)
		{
			string name = Util.getEncryptFileName(fileName.ToLower());

			_dataPath = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+name,bundleExtension,false);
			
			if(File.Exists(_dataPath))
			{
				byte[] bytes = Util.decByte( File.ReadAllBytes(_dataPath) );

				using(MemoryStream resultStream = new MemoryStream())
				{
					using(Stream stm = new MemoryStream(bytes))
					{
						ZipInputStream zis = new ZipInputStream(stm);
						
						ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
						
						while ((ze = zis.GetNextEntry()) != null)
						{
							if (!ze.IsDirectory)
							{
#if UNITY_EDITOR
//								Debug.Log(ze.Name);
#endif
								
								byte[] buffer = new byte[2048];
								int len;
								
								if(ze.Name == name)
								{
									while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
									{
										resultStream.Write(buffer, 0, len);
									}
								}
							}
						}
						
						zis.Close(); // 입력받은 녀석은 지우고...
						zis.Dispose();
						zis = null;
						
						resultStream.Position = 0;
						
						return System.Text.Encoding.UTF8.GetString(resultStream.ToArray());
					}
				}
			}
		}
		else
		{
			_dataPath = ResourceManager.getLocalFilePath(ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+"cs/"+fileName,extension,false);
			
			if(File.Exists(_dataPath))
			{
				return File.ReadAllText(_dataPath);
			}
		}
		
		return null;
	}








	void unzipCutSceneData()
	{
		string achievePath = ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+ GameManager.info.etcFileNames["cs" + fileList["v"]] ,bundleExtension, false);

		Util.extractZipByteFile(Util.decByte(File.ReadAllBytes(achievePath)), null, ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true),"",false));

		//Util.extractZipFile(achievePath,null,ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true),"",false));
	}

	bool unzipData()
	{
		try
		{
			string achievePath = ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+ GameManager.info.etcFileNames["gd" + fileList["v"]] ,bundleExtension, false);
			
			using(MemoryStream tms = new MemoryStream(File.ReadAllBytes(achievePath)))
			{
				byte[] d = new byte[tms.Length-P_LENGH];
				
				tms.Position = P_LENGH;
				tms.Read(d, 0, d.Length);
				
				d = Util.decByte(d);
				
				GameManager.info.dataStreamDic.Clear();
				
				using(Stream stm = new MemoryStream(d))
				{
					ZipInputStream zis = new ZipInputStream(stm);
					if(P != null)
					{
						zis.Password = P;
					}
					
					ICSharpCode.SharpZipLib.Zip.ZipEntry ze;
					
					while ((ze = zis.GetNextEntry()) != null)
					{
						if (!ze.IsDirectory)
						{
							byte[] buffer = new byte[4096];
							int len;
							
							MemoryStream tempMs = new MemoryStream();
							
							while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
							{
								tempMs.Write(buffer, 0, len);
							}
							
							tempMs.Position = 0;
							GameManager.info.dataStreamDic.Add(ze.Name.Replace(bundleExtension,""), tempMs.ToArray());
							tempMs.Close();
							tempMs.Dispose();
						}
					}
					
					GameManager.info.version = Util.decByteAndConvertToString(GameManager.info.dataStreamDic[Util.getEncryptFileName("v")]);
					
					zis.Close(); // 입력받은 녀석은 지우고...
					zis.Dispose();
					zis = null;
					
					d = null;
					
				}
			}
		}
		catch
		{
			return false;
		}

		return true;
	}



	void unzipDummy()
	{
		string achievePath = ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true)+ GameManager.info.etcFileNames["dm"] ,bundleExtension, false);
		Util.extractZipFile(achievePath, null,ResourceManager.getLocalFilePath( ResourceManager.getFolderName(FOLDER_TYPE.Data,true),"",false),false);
	}







	public bool completeLoadingNGUIAsset = false;

	public void startLoadNGUIAsset()
	{
		completeLoadingNGUIAsset = false;
		StartCoroutine(loadNGUIAsset(NGUI_PACKAGE_NAME));
	}


	IEnumerator loadNGUIAsset(string loadAssetsNames)
	{
		string path = AssetBundleManager.getResourceName(loadAssetsNames, ResourceType.UI);

		#if UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath(ResourceManager.NGUI_IPHONE + path, AssetBundleManager.bundleExtension);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.NGUI_ANDROID + path, AssetBundleManager.bundleExtension);
		#endif
		
		//		using(WWW asset = new WWW(path))
		WWW asset = new WWW(path);
		{
			#if UNITY_EDITOR		
			Debug.Log("path: " + path);
			#endif	
			
			bool success = true;
			
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
				#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
				#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				UnityEngine.Object[] clips = asset.assetBundle.LoadAll( typeof( UIAtlas ));
				
				foreach(UnityEngine.Object at in clips)
				{
					string n = at.name.ToUpper();

					if(n == ("NGUI_EQUIP_ICON"))
					{
						ResourceManager.instance.equipIconAtlas[0] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON2"))
					{
						ResourceManager.instance.equipIconAtlas[1] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON3"))
					{
						ResourceManager.instance.equipIconAtlas[2] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON4"))
					{
						ResourceManager.instance.equipIconAtlas[3] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON5"))
					{
						ResourceManager.instance.equipIconAtlas[4] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON6"))
					{
						ResourceManager.instance.equipIconAtlas[5] = at as UIAtlas;
					}
					else if(n == ("NGUI_EQUIP_ICON7"))
					{
						ResourceManager.instance.equipIconAtlas[6] = at as UIAtlas;
					}


					else if(n == ("NGUI_SKILL_ICON"))
					{
						ResourceManager.instance.skillIconAtlas[0] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON2"))
					{
						ResourceManager.instance.skillIconAtlas[1] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON3"))
					{
						ResourceManager.instance.skillIconAtlas[2] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON4"))
					{
						ResourceManager.instance.skillIconAtlas[3] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON5"))
					{
						ResourceManager.instance.skillIconAtlas[4] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON6"))
					{
						ResourceManager.instance.skillIconAtlas[5] = at as UIAtlas;
					}
					else if(n == ("NGUI_SKILL_ICON7"))
					{
						ResourceManager.instance.skillIconAtlas[6] = at as UIAtlas;
					}



					else if(n == ("NGUI_UNIT_ICON"))
					{
						ResourceManager.instance.unitIconAtlas[0] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON2"))
					{
						ResourceManager.instance.unitIconAtlas[1] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON3"))
					{
						ResourceManager.instance.unitIconAtlas[2] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON4"))
					{
						ResourceManager.instance.unitIconAtlas[3] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON5"))
					{
						ResourceManager.instance.unitIconAtlas[4] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON6"))
					{
						ResourceManager.instance.unitIconAtlas[5] = at as UIAtlas;
					}
					else if(n == ("NGUI_UNIT_ICON7"))
					{
						ResourceManager.instance.unitIconAtlas[6] = at as UIAtlas;
					}

				}
				
				asset.assetBundle.Unload(false);
			}
		}
		
		if(asset != null) asset.Dispose();
		asset = null;	

		completeLoadingNGUIAsset = true;

		#if UNITY_EDITOR
		Debug.LogError("completeLoadingNGUIAsset");
		#endif

	}	







	public bool completeLoadingPlayerTextre = false;
	
	public void startLoadPlayerTextureAsset()
	{
		completeLoadingPlayerTextre = false;
		StartCoroutine(loadPlayerTextureAsset(PLAYER_TEXTURE_PACKAGE_NAME));
	}


	public AssetBundle playerTextureBundle;
	
	IEnumerator loadPlayerTextureAsset(string loadAssetsNames)
	{
		string path = AssetBundleManager.getResourceName(loadAssetsNames, ResourceType.Texture);
		
		#if UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_IPHONE + path, AssetBundleManager.bundleExtension);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_ANDROID + path, AssetBundleManager.bundleExtension);
		#endif
		
		//		using(WWW asset = new WWW(path))
		WWW asset = new WWW(path);
		{
			#if UNITY_EDITOR		
			Debug.Log("path: " + path);
			#endif	
			
			bool success = true;
			
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
				#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
				#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				ResourceManager.instance.loadAllTextureFromBundle(asset.assetBundle);

				//playerTextureBundle = asset.assetBundle;
				asset.assetBundle.Unload(false);
			}
		}
		
		if(asset != null) asset.Dispose();
		asset = null;	
		
		completeLoadingPlayerTextre = true;

#if UNITY_EDITOR
		Debug.LogError("completeLoadingPlayerTextre");
#endif
	}


















	
	public bool completeLoadingCharacterSound = false;
	
	public void startLoadCharacterSound()
	{
		completeLoadingCharacterSound = false;
		StartCoroutine(loadCharacterSoundAsset(CHARACTER_VOICE_PACKAGE_NAME));
	}
	
	IEnumerator loadCharacterSoundAsset(string loadAssetsNames)
	{
		string path = AssetBundleManager.getResourceName(loadAssetsNames, ResourceType.Sound);
		
		#if UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_IPHONE + path, AssetBundleManager.bundleExtension);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_ANDROID + path, AssetBundleManager.bundleExtension);
		#endif
		
		//		using(WWW asset = new WWW(path))
		WWW asset = new WWW(path);
		{
			#if UNITY_EDITOR		
			Debug.Log("path: " + path);
			#endif	
			
			bool success = true;
			
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
				#if UNITY_EDITOR
				Debug.LogError("err: " + asset.error.ToString() + "   path : " + path);
				#endif
			}
			else if(asset != null && asset.assetBundle != null)
			{
				SoundManager.instance.setSfxSoundFromAssetBundle(asset.assetBundle);

				asset.assetBundle.Unload(false);
			}
		}
		
		if(asset != null) asset.Dispose();
		asset = null;	
		
		completeLoadingCharacterSound = true;
		
		#if UNITY_EDITOR
		Debug.LogError("completeLoadingCharacterSound");
		#endif
	}

















	void OnDestroy()
	{
		if(playerTextureBundle != null)
		{
			try
			{
				playerTextureBundle.Unload(true);
			}
			catch
			{

			}

			playerTextureBundle = null;
		}
	}




	public Dictionary<string, string> getFileNameDic(FOLDER_TYPE folderType)
	{
		switch(folderType)
		{
		case FOLDER_TYPE.Character:
			return GameManager.info.modelFileNames;
			break;
		case FOLDER_TYPE.Map:
			return GameManager.info.mapFileNames;
			break;
		case FOLDER_TYPE.Data:
			return GameManager.info.etcFileNames;
			break;
		case FOLDER_TYPE.Pack:
			return GameManager.info.etcFileNames;
			break;
		case FOLDER_TYPE.TutorialSound:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.CutSceneSound:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.GameMusic:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.CutSceneMusic:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.Sound:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.NGUITexture:
			return GameManager.info.uiFileNames;
			break;
		case FOLDER_TYPE.PlayerTexture:
			return GameManager.info.textureFileNames;
			break;
		case FOLDER_TYPE.GameEffect:
			return GameManager.info.effectFileNames;
			break;
		case FOLDER_TYPE.CharacterVoice:
			return GameManager.info.soundFileNames;
			break;
		case FOLDER_TYPE.ETCPack:
			return GameManager.info.etcFileNames;
			break;
		default:
			return GameManager.info.etcFileNames;
			break;
		}
	}



	string getResourcePath()
	{
		if(useLowResource)
		{
			if(ResourceManager.useLowResource)
			{
				return resourcePath + "resource/low/";
			}
		}

		return resourcePath + "resource/";
	}
	
	
}

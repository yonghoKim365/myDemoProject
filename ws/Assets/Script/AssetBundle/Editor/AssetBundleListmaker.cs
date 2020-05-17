using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;
using ICSharpCode.SharpZipLib.Zip;
using System;

public class AssetBundleListMaker
{
	public static int dataVersion
	{
		get
		{
			return AssetBundleMakerManager.dataVersion;
		}
	}

	public const int PACK_FILE_SIZE = 8;

	public static void CutSceneBundleList()
	{
#if UNITY_IOS
		EditorUtility.DisplayDialog("경고","안드로이드에서 만드세요.","닫기");
		Debug.LogError("안드로이드에서 만드세요!");
		return;
#endif

		List<byte[]> data = new List<byte[]>();
		Dictionary<string, string> dic = new Dictionary<string, string>();

		string path = "ver"+dataVersion+"/cs";

		path = ResourceManager.getLocalFilePath(path,"",false,true);
		
		string tempPath = ResourceManager.getLocalFilePath("UPLOAD/C","",false,true);
		
		if(Directory.Exists(tempPath) == false) Directory.CreateDirectory(tempPath);
		
		string[] files = Directory.GetFiles(path,"*.txt");
		
		string[] csNames = new string[files.Length];

		List<Byte[]> tempSave = new List<byte[]>();

		for(int i = 0; i < files.Length; ++i)
		{
			csNames[i] = Util.getEncryptFileName(Path.GetFileNameWithoutExtension(files[i])) + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME;

			tempSave.Clear();

			tempSave.Add(File.ReadAllBytes(files[i]));

			Byte[] zip = Util.saveZipToByteArray( new string[]{Util.getEncryptFileName(Path.GetFileNameWithoutExtension(files[i]))}, tempSave);

			data.Add(Util.encByte( zip ));
		}

		byte[] outBytes = Util.saveZipToByteArray(csNames, data);

		File.WriteAllBytes(tempPath + "/" + AssetBundleManager.CSP_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME, Util.encByte(outBytes));

		//Util.saveZip(tempPath + "/" + AssetBundleManager.CSP_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME, csNames, data);

		Debug.LogError("CutSceneBundleList 완료!.");
	}



	public static void PackageGameData()
	{

		#if UNITY_IOS
		EditorUtility.DisplayDialog("경고","안드로이드에서 만드세요.","닫기");
		Debug.LogError("안드로이드에서 만드세요!");
		return;
		#endif

		string dataSourceFolder = "ver"+dataVersion; //PlayerSettings.bundleVersion.Replace(".","_")+"/

		List<byte[]> data = new List<byte[]>();
		Dictionary<string, string> dic = new Dictionary<string, string>();
		
		string path = ResourceManager.getLocalFilePath(dataSourceFolder,"",false,true);
		
		string tempPath = ResourceManager.getLocalFilePath("UPLOAD/C","",false,true);
		
		if(Directory.Exists(tempPath) == false) Directory.CreateDirectory(tempPath);

		File.WriteAllText(ResourceManager.getLocalFilePath(dataSourceFolder+"/v",".txt",false,true),dataVersion.ToString());

		string[] files = Directory.GetFiles(path,"*.txt");
		
		string[] dataNames = new string[files.Length];
		
		for(int i = 0; i < files.Length; ++i)
		{
			dataNames[i] = Util.getEncryptFileName(Path.GetFileNameWithoutExtension(files[i])) + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME;
			data.Add(Util.encByte(File.ReadAllBytes(files[i])));
		}
		
		byte[] dataBytes = Util.saveZipToByteArray(dataNames, data, AssetBundleManager.P);
		dataBytes = Util.encByte(dataBytes);

		byte[] dummyBytes = File.ReadAllBytes(ResourceManager.getLocalFilePath("dummy/dummy",".jpg",false,true));

		File.WriteAllBytes(tempPath + "/" + AssetBundleManager.GD_PACKAGE_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME, Util.mergeByteArray(dummyBytes, dataBytes));

		//=== dummy ===//

		if(File.Exists( tempPath + "/" + AssetBundleManager.GDDM_PACKAGE_NAME)) return;

		if(File.Exists( ResourceManager.getLocalFilePath("datadummy/dm",".p12",false,true)))
		{
			File.Copy(ResourceManager.getLocalFilePath("datadummy/dm",".p12",false,true),  tempPath + "/" + AssetBundleManager.GDDM_PACKAGE_NAME);
			return;
		}


		path = ResourceManager.getLocalFilePath("datadummy","",false,true);
		
		files = Directory.GetFiles(path,"*.p12");
		
		dataNames = new string[files.Length];
		
		for(int i = 0; i < files.Length; ++i)
		{
			dataNames[i] = Util.getEncryptFileName(Path.GetFileNameWithoutExtension(files[i])) + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME;
			data.Add( File.ReadAllBytes(files[i]) );
		}
		
		dataBytes = Util.saveZipToByteArray(dataNames, data, null);

		File.WriteAllBytes(tempPath + "/" + AssetBundleManager.GDDM_PACKAGE_NAME, dataBytes);

		Debug.LogError("PackageGameData 완료!.");
	}



	public static void createDummyData()
	{
		string localPath = ResourceManager.getLocalFilePath("datadummy","",false,true);
		
		string header = "BrainBuster    4.x.x 0.4.0f1       <         ?        4 ]           g";

		string end = "";

		Byte[] fuck = System.Text.Encoding.UTF8.GetBytes(header);

		for(int i = 0; i < 200; ++i)
		{
			byte[] array = new byte[32];
			System.Random random = new System.Random();
			
			int limitSize = UnityEngine.Random.Range(1024,50230);
			
			MemoryStream ms = new MemoryStream();

			ms.Write(fuck,0,fuck.Length);

			while(limitSize > 0)
			{
				if(UnityEngine.Random.Range(0,10) > 8)
				{
					random.NextBytes(array);
				}
				else
				{
					for(int j = 0; j < array.Length; ++j)
					{
						array[j] = new byte();
					}
				}

				ms.Write(array,0,32);
				limitSize -= 32;
			}

			Byte[] enc = Util.encByte(ms.ToArray());

			ms = new MemoryStream();

			if(UnityEngine.Random.Range(0,100) < 3)
			{
				ms.Write(fuck,0,fuck.Length);
			}

			ms.Write(enc,0,enc.Length);

			random.NextBytes(array);
			ms.Write(array,0,32);

			File.WriteAllBytes(localPath + "/" + Util.getMD5HashFromStream(ms, 500,100) + ".p12", ms.ToArray());
		}

		for(int i = 0; i < 5; ++i)
		{
			byte[] array = new byte[32];
			System.Random random = new System.Random();
			
			int limitSize = UnityEngine.Random.Range(430000,640640);
			
			MemoryStream ms = new MemoryStream();

			ms.Write(fuck,0,fuck.Length);

			float l = limitSize;

			while(limitSize > 0)
			{
				if(limitSize > l * 0.98f )
				{
					random.NextBytes(array);
				}
				else
				{
					{
						for(int j = 0; j < array.Length; ++j)
						{
							array[j] = new byte();
						}
					}
				}

				ms.Write(array,0,32);
				limitSize -= 32;
			}

			Byte[] enc = Util.encByte(ms.ToArray());
			
			ms = new MemoryStream();
			ms.Write(enc,0,enc.Length);
			random.NextBytes(array);
			ms.Write(array,0,32);
			
			File.WriteAllBytes(localPath + "/" + Util.getMD5HashFromStream(ms, 500,100) + ".p12", ms.ToArray());
		}

	}



	public static void CreateBundleList()
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		string path = "";
		string uploadPath = "";
		string[] files;

// =========  NGUI 텍스쳐
		dic.Clear();
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.NGUI_ANDROID,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/"+ResourceManager.NGUI_ANDROID,"",false, true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.NGUI_IPHONE,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/"+ResourceManager.NGUI_IPHONE,"",false, true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");

		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);
			
			string saveMd5Name = "";
			
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path, uploadPath, f, true);
			}
			
			saveMd5Name = md5Name + ".p12";
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 .");
			}
		}

		string ng = MiniJSON.Json.Serialize(dic);




// =========  캐릭터 텍스쳐
		dic.Clear();
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_ANDROID,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/"+ResourceManager.TEXTURE_ANDROID,"",false, true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.TEXTURE_IPHONE,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/"+ResourceManager.TEXTURE_IPHONE,"",false, true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);
			
			string saveMd5Name = "";
			
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path, uploadPath, f, true);
			}
			
			saveMd5Name = md5Name + ".p12";
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 .");
			}
		}
		
		string pt = MiniJSON.Json.Serialize(dic);












// =========  캐릭터 사운드
		dic.Clear();
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_ANDROID,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/"+ResourceManager.CHARACTERSOUND_ANDROID,"",false, true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.CHARACTERSOUND_IPHONE,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/"+ResourceManager.CHARACTERSOUND_IPHONE,"",false, true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);
			
			string saveMd5Name = "";
			
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path, uploadPath, f, true);
			}
			
			saveMd5Name = md5Name + ".p12";
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 .");
			}
		}
		
		string cv = MiniJSON.Json.Serialize(dic);



// ========= 캐릭터 모델 정보 생성   ======================================================== //
		dic.Clear();

#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("A/","",false, true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/A/","",false, true);
#else
		path = ResourceManager.getLocalFilePath("I/","",false, true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/I/","",false, true);
#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);

			dic.Add(Path.GetFileNameWithoutExtension(f).Replace("_base",""),Util.getFileSize(f) + "&" + md5Name);

			string saveMd5Name = "";

			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path, uploadPath, f, true);
			}

			saveMd5Name = md5Name + ".p12";

			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 .");
			}
		}

		Dictionary<string, ModelData> mData = ImportUtil.loadModelData();
		foreach(KeyValuePair<string, ModelData> kv in mData)
		{
			if(kv.Key.EndsWith("_weapon")) continue;
			if(kv.Value.modelType == ModelData.ModelType.Weapon) continue;

			if(dic.ContainsKey(kv.Key) == false) Debug.LogError(kv.Key + " 캐릭터 모델링이 없어요!");
		}

		string cd = MiniJSON.Json.Serialize(dic);


// ========= MAP 정보 생성   ======================================================== //

		dic.Clear();

		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("MA/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/MA/","",false,true);
		#else
		path = ResourceManager.getLocalFilePath("MI/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/MI/","",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");

		Dictionary<int, MapData> mapData = ImportUtil.loadMapData();

		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);

			string dicKey = Path.GetFileNameWithoutExtension(f);

			bool hasDicKey = false;

			foreach(KeyValuePair<int, MapData> kv in mapData)
			{
				if(kv.Value.resource == dicKey) hasDicKey = true;
			}

			if(hasDicKey == false)
			{
				Debug.LogError("== 만들 필요가 없는 map : " + dicKey);
				continue;
			}

			dic.Add(dicKey,Util.getFileSize(f) + "&" + md5Name);

			string saveMd5Name = "";
			
			saveMd5Name = md5Name + ".p12";
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path,uploadPath,f, true);
			}
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 .");
			}
		}
		

		foreach(KeyValuePair<int, MapData> kv in mapData)
		{
			if(dic.ContainsKey(kv.Value.resource) == false) Debug.LogError(kv.Value.resource + "맵 모델링이 없어요!");
		}
		
		string md = MiniJSON.Json.Serialize(dic);


//================== 데이터 정보 생성  ======================================================== //
		List<byte[]> data = new List<byte[]>();

		dic.Clear();

		// == 컷씬 수집 == //
//		path = ResourceManager.getLocalFilePath("C/cs/","",false,true);
		string tempPath = ResourceManager.getLocalFilePath("UPLOAD/C","",false,true);

		string cutscenePackName = tempPath + "/" + AssetBundleManager.CSP_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME;
		string tempMd5Name = Util.getMD5HashFromFile(cutscenePackName);
		dic.Add(Path.GetFileNameWithoutExtension(cutscenePackName),Util.getFileSize(cutscenePackName) + "&" + tempMd5Name);
		Util.fileCopy(tempPath,tempPath,AssetBundleManager.CSP_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME,true);
		if(tempMd5Name != Util.getMD5HashFromFile(tempPath  + "/" +  tempMd5Name + ".p12")) Debug.LogError("cs : 복사한 md5 이름이 다름 ");

		string dataPackName = tempPath + "/" + AssetBundleManager.GD_PACKAGE_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME;
		tempMd5Name = Util.getMD5HashFromFile(dataPackName);
		dic.Add(Path.GetFileNameWithoutExtension(dataPackName),Util.getFileSize(dataPackName) + "&" + tempMd5Name );
		Util.fileCopy(tempPath,tempPath,AssetBundleManager.GD_PACKAGE_NAME + dataVersion + AssetBundleManager.DOWNLOAD_BUNDLE_EXTENSION_NAME,true);
		if(tempMd5Name != Util.getMD5HashFromFile(tempPath  + "/" +  tempMd5Name + ".p12")) Debug.LogError("gd : 복사한 md5 이름이 다름 ");

		string dummyPackName = tempPath + "/" + AssetBundleManager.GDDM_PACKAGE_NAME;
		tempMd5Name = Util.getMD5HashFromFile(dummyPackName);
		dic.Add(Path.GetFileNameWithoutExtension(dummyPackName),Util.getFileSize(dummyPackName) + "&" + tempMd5Name);
		Util.fileCopy(tempPath,tempPath,AssetBundleManager.GDDM_PACKAGE_NAME,true);
		if(tempMd5Name != Util.getMD5HashFromFile(tempPath + "/" + tempMd5Name + ".p12")) Debug.LogError("dm : 복사한 md5 이름이 다름 ");


		string dd = MiniJSON.Json.Serialize(dic);






		// ========= 사운드 정보 생성   ======================================================== //
		
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("SA/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/SA/","",false,true);
		#else
		path = ResourceManager.getLocalFilePath("SI/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/SI/","",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);
			
			string saveMd5Name = "";
			
			saveMd5Name = md5Name + ".p12";
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path,uploadPath,f, true);
			}
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 ");
			}
		}
		
		if(dic.ContainsKey(AssetBundleManager.CUTSCENE_VOICE_PACKAGE_NAME) == false) Debug.LogError( "컷씬 사운드가 없어요!");
		if(dic.ContainsKey(AssetBundleManager.TUTORIAL_VOICE_PACKAGE_NAME) == false) Debug.LogError( "튜토리얼 사운드가 없어요!");
		if(dic.ContainsKey(AssetBundleManager.GAME_MUSIC_PACKAGE_NAME) == false) Debug.LogError( "게임뮤직 사운드가 없어요!");
		if(dic.ContainsKey(AssetBundleManager.CUTSCENE_MUSIC_PACKAGE_NAME) == false) Debug.LogError( "컷신뮤직 사운드가 없어요!");

		string sd = MiniJSON.Json.Serialize(dic);



		// tutorial sound list
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.TUTORIAL_VOICE_ANDROID,"",false,true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.TUTORIAL_VOICE_IPHONE,"",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&");
		}
		
		string st = MiniJSON.Json.Serialize(dic);


		// cutscene sound list
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.CS_VOICE_ANDROID,"",false,true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.CS_VOICE_IPHONE,"",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&");
		}
		
		string sc = MiniJSON.Json.Serialize(dic);






		// =========  이펙트 팩.
		dic.Clear();
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.ETCPACK_ANDROID,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/"+ResourceManager.ETCPACK_ANDROID,"",false, true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.ETCPACK_IPHONE,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/"+ResourceManager.ETCPACK_IPHONE,"",false, true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);
			
			string saveMd5Name = "";
			
			if(File.Exists(path + saveMd5Name) == false)
			{
				Util.fileCopy(path, uploadPath, f, true);
			}
			
			saveMd5Name = md5Name + ".p12";
			
			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
			{
				Debug.LogError(f + " : 복사한 md5 이름이 다름 ");
			}
		}
		
		string ep = MiniJSON.Json.Serialize(dic);




		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.EFFECT_ANDROID,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/"+ResourceManager.EFFECT_ANDROID,"",false, true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.EFFECT_IPHONE,"",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/"+ResourceManager.EFFECT_IPHONE,"",false, true);
		#endif

		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			string saveMd5Name = "";

//			Debug.Log(f + "   " + Util.getFileSize(f));
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&" + md5Name);

//			if(File.Exists(path + saveMd5Name) == false)
//			{
//				Util.fileCopy(path, uploadPath, f, true);
//			}
//			
//			saveMd5Name = md5Name + ".p12";
//			
//			if(md5Name != Util.getMD5HashFromFile(uploadPath + saveMd5Name))
//			{
//				Debug.LogError(f + " : 복사한 md5 이름이 다름 ");
//			}

		}

		
		string ge = MiniJSON.Json.Serialize(dic);



		// =========  게임음악
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.GAMEMUSIC_ANDROID,"",false,true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.GAMEMUSIC_IPHONE,"",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&");
		}
		
		string b = MiniJSON.Json.Serialize(dic);




		// 컷씬 음악
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath(ResourceManager.CS_MUSIC_ANDROID,"",false,true);
		#else
		path = ResourceManager.getLocalFilePath(ResourceManager.CS_MUSIC_IPHONE,"",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		foreach(string f in files)
		{
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&");
		}
		
		string cb = MiniJSON.Json.Serialize(dic);














		// ========= 스타터팩 정보 생성   ======================================================== //
		
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("UPLOAD/PA/","",false,true);
		#else
		path = ResourceManager.getLocalFilePath("UPLOAD_IOS/PI/","",false,true);
		#endif
		files = Directory.GetFiles(path,"*.p12");
		
		foreach(string f in files)
		{
			dic.Add(Path.GetFileNameWithoutExtension(f),Util.getFileSize(f) + "&");
		}

		string pd = MiniJSON.Json.Serialize(dic);

//================== 목록 생성.  ======================================================== //

		data.Clear();

		List<string> names = new List<string>();

		data.Add(System.Text.Encoding.UTF8.GetBytes(dataVersion.ToString()));
		names.Add("v");

		data.Add(System.Text.Encoding.UTF8.GetBytes(cd));
		names.Add("cd");

		data.Add(System.Text.Encoding.UTF8.GetBytes(md));
		names.Add("md");

		data.Add(System.Text.Encoding.UTF8.GetBytes(dd));
		names.Add("dd");

		data.Add(System.Text.Encoding.UTF8.GetBytes(pd));
		names.Add("pd");

		data.Add(System.Text.Encoding.UTF8.GetBytes(sd)); // total sound
		names.Add("sd");

		data.Add(System.Text.Encoding.UTF8.GetBytes(st)); // tutorial list
		names.Add(AssetBundleManager.TUTORIAL_VOICE_PACKAGE_NAME); // tutorial

		data.Add(System.Text.Encoding.UTF8.GetBytes(sc)); // cs list
		names.Add(AssetBundleManager.CUTSCENE_VOICE_PACKAGE_NAME); 

		data.Add(System.Text.Encoding.UTF8.GetBytes(b)); // cs list
		names.Add( AssetBundleManager.GAME_MUSIC_PACKAGE_NAME); 

		data.Add(System.Text.Encoding.UTF8.GetBytes(cb)); // cs list
		names.Add(AssetBundleManager.CUTSCENE_MUSIC_PACKAGE_NAME); 

		data.Add(System.Text.Encoding.UTF8.GetBytes(ng)); // ngui list
		names.Add(AssetBundleManager.NGUI_PACKAGE_NAME); 


		data.Add(System.Text.Encoding.UTF8.GetBytes(pt)); // 
		names.Add(AssetBundleManager.PLAYER_TEXTURE_PACKAGE_NAME); 


		data.Add(System.Text.Encoding.UTF8.GetBytes(ep)); // ngui list
		names.Add(AssetBundleManager.GAME_EFFECT_ZIP_PACKAGE_NAME); 


		data.Add(System.Text.Encoding.UTF8.GetBytes(ge)); // ngui list
		names.Add(AssetBundleManager.GAME_EFFECT_PACKAGE_NAME); 


		data.Add(System.Text.Encoding.UTF8.GetBytes(cv)); // ngui list
		names.Add(AssetBundleManager.CHARACTER_VOICE_PACKAGE_NAME); 


#if UNITY_ANDROID
		Util.saveZip(ResourceManager.getLocalFilePath("UPLOAD/fa"+PlayerSettings.bundleVersion.Replace(".","_"),"",false,true),names.ToArray(),data);
#else
		Util.saveZip(ResourceManager.getLocalFilePath("UPLOAD_IOS/fi"+PlayerSettings.bundleVersion.Replace(".","_"),"",false,true),names.ToArray(),data);
#endif
		Debug.LogError("정보 저장 완료!");
	}








////=================== 스타터 팩. =============================////

	// 최초 사용자를 위한 압축 팩.

	public static void CreateStarterPack()
	{
		Dictionary<string, string> dic = new Dictionary<string, string>();
		
		// ========= 캐릭터 ======================================================== //
		#if UNITY_ANDROID
		string path = ResourceManager.getLocalFilePath("A/","",false, true);
		string uploadPath = ResourceManager.getLocalFilePath("UPLOAD/A/","",false, true);
		#else
		string path = ResourceManager.getLocalFilePath("I/","",false, true);
		string uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/I/","",false, true);
		#endif
		string[] files = Directory.GetFiles(path,"*.assetbundle");


		long packSize = 0;
		int packIndex = 0;

		Dictionary<int, List<string>> packingFileList = new Dictionary<int, List<string>>();

		packingFileList.Clear();
		packSize = 0;
		packIndex = 0;

#if UNITY_ANDROID
		string saveDirectoryName = ResourceManager.getLocalFilePath("UPLOAD/PA/","",false, true) ;
#else
		string saveDirectoryName = ResourceManager.getLocalFilePath("UPLOAD_IOS/PI/","",false, true) ;
#endif


		if(Directory.Exists(saveDirectoryName) == false) Directory.CreateDirectory(saveDirectoryName);

		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			string saveMd5Name = uploadPath + md5Name + ".p12";
			if(File.Exists(saveMd5Name) == false) Debug.LogError("에러! : " + f);
			if(md5Name != Util.getMD5HashFromFile(saveMd5Name)) Debug.LogError(f + " : 복사한 md5 이름이 다름 " + saveMd5Name);

			packSize += Util.getFileSize(saveMd5Name);

			if(packingFileList.ContainsKey(packIndex) == false) packingFileList.Add(packIndex, new List<string>());
			packingFileList[packIndex].Add(saveMd5Name);

			if(packSize >= 1048576 * PACK_FILE_SIZE)
			{
				packSize = 0;
				++packIndex;
			}
		}

		foreach(KeyValuePair<int, List<string>> kv in packingFileList)
		{
			Util.saveZip(saveDirectoryName + "A"+kv.Key + ".p12", kv.Value.ToArray());
		}



		// ========= 맵 ======================================================== //
		
		dic.Clear();
		
		#if UNITY_ANDROID
		path = ResourceManager.getLocalFilePath("MA/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD/MA/","",false,true);
		#else
		path = ResourceManager.getLocalFilePath("MI/","",false,true);
		uploadPath = ResourceManager.getLocalFilePath("UPLOAD_IOS/MI/","",false,true);
		#endif
		files = Directory.GetFiles(path,"*.assetbundle");
		
		packingFileList.Clear();
		packSize = 0;
		packIndex = 0;
		
		foreach(string f in files)
		{
			string md5Name = Util.getMD5HashFromFile(f);
			string saveMd5Name = uploadPath + md5Name + ".p12";
			if(File.Exists(saveMd5Name) == false) Debug.LogError("에러! : " + f);
			if(md5Name != Util.getMD5HashFromFile(saveMd5Name)) Debug.LogError(f + " : 복사한 md5 이름이 다름 ");
			
			packSize += Util.getFileSize(saveMd5Name);
			
			if(packingFileList.ContainsKey(packIndex) == false) packingFileList.Add(packIndex, new List<string>());
			packingFileList[packIndex].Add(saveMd5Name);
			
			if(packSize >= 1048576 * PACK_FILE_SIZE)
			{
				packSize = 0;
				++packIndex;
			}
		}

		foreach(KeyValuePair<int, List<string>> kv in packingFileList)
		{
			Util.saveZip(saveDirectoryName + "M"+kv.Key + ".p12", kv.Value.ToArray());
		}






		Debug.LogError("묶음 ZIP 파일 생성 완료!");

	}
}



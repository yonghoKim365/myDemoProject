using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using LitJson;
#endif

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Security.Cryptography;
using System.Reflection;
using System.Text;

public class ResourceManager : MonoBehaviour 
{

	public static bool useLowResource = false;

	public bool useAssetDownload = false;

	public UIAtlas[] unitIconAtlas;
	public UIAtlas[] skillIconAtlas;
	public UIAtlas[] equipIconAtlas;

	public Material rareMaterial;
	public Material superRareMaterial;
	public Material legendMaterial;

	public Texture rareTexture;
	public Texture superRareTexture;
	public Texture legendTexture;

	public Texture lobbyShaderTexture;

	public static ResourceManager instance = null;

	const string prefabPath = "prefabs/";


	void OnDestroy()
	{
//		Debug.Log("OnDestroy : ResourceManager");
		destroyAllPrefab();

		skillTransparent = null;
		_nguiIconAtlas.Clear();
		_collections = null;
		_collectionList = null;
		_prefabs = null;


		instance = null;
	}
	

	private Dictionary<string, GameObject> _prefabs;


	private Dictionary<string, UIAtlas> _nguiIconAtlas = new Dictionary<string, UIAtlas>(StringComparer.Ordinal);
	private Dictionary<string, tk2dSpriteCollectionData> _collections;
	private Dictionary<string, string> _collectionList;

	public Dictionary<string, string> collectionList
	{
		set
		{
			_collectionList = value;
		}
	}

	private Dictionary<string, Texture> _texture = new Dictionary<string, Texture>();


	public Shader skillTransparent;// = Shader.Find("Custom/SkillTransparent");
	public Shader normalShader;// = Shader.Find("Unlit/Texture Color");
	public Shader damageShader;// = Shader.Find("Custom/CharacterDamageEffect2");
	public Shader lobbyShader;

	public Shader rimShader;


	void Awake()
	{
		if(instance==null){
			_collections = new Dictionary<string, tk2dSpriteCollectionData>();
			_prefabs = new Dictionary<string, GameObject>();

			instance = this;

			skillTransparent = Shader.Find("Custom/SkillTransparent");
			normalShader = Shader.Find("Unlit/Texture Color");
			damageShader = Shader.Find("Custom/CharacterDamageEffect2");

			rimShader = Shader.Find("Rim/Rimlight_rampL");


		}else{
			DestroyImmediate(this.gameObject);
		}

#if !UNITY_EDITOR
		useAssetDownload = true;
#endif
	}



	public void loadAllTextureFromBundle(AssetBundle ab)
	{
		if(ab == null) return;

		UnityEngine.Object[] obs = ab.LoadAll(typeof(Texture));

		if(obs != null)
		{
			foreach(UnityEngine.Object o in obs)
			{
				_texture[o.name] = (Texture)o;
			}
		}
	}



	public Texture getPartsTexture(string name)
	{
		if(!_texture.ContainsKey(name) || _texture[name] == null)
		{
//			_texture[name] = (Texture)AssetBundleManager.instance.playerTextureBundle.Load(name, typeof(Texture));

			_texture[name] = (Texture)Resources.Load("texture/player/"+name,typeof(Texture));
		}

#if UNITY_EDITOR
//		Debug.LogError("get parts name : " + name);
#endif

		return _texture[name];			
	}















	private Dictionary<string, Stack<GameObject>> _prefabPool = new Dictionary<string, Stack<GameObject>>(StringComparer.Ordinal);
	
	public GameObject getPrefabFromPool(string name)
	{
		if(!_prefabPool.ContainsKey(name) || _prefabPool[name] == null)
		{
			_prefabPool[name] = new Stack<GameObject>();
		}
		
		if(_prefabPool[name].Count == 0)
		{
			return getInstantPrefabs(name);
		}
		
		GameObject go = _prefabPool[name].Pop();

		if(go == null) return getInstantPrefabs(name);

		return go;
	}
	
	public void setPrefabToPool(string name, GameObject gobj)
	{
		if(!_prefabPool.ContainsKey(name) || _prefabPool[name] == null)
		{
			_prefabPool[name] = new Stack<GameObject>();
		}
		
		_prefabPool[name].Push(gobj);
	}



	public GameObject getInstantPrefabs(string name)
	{
		GameObject go = (GameObject)Instantiate(getPrefabs(name));
		go.SetActive(true);
		return go;
	}


//	public List<string> bundlePrefabNames = new List<string>();


	Stack<GameObject> _resetShaderEffects = new Stack<GameObject>();
	public void setPrefabFromAssetBundle(EffectData ed, AssetBundle ab, GameObject prefab = null)
	{
		string name = ed.resource;

		if(!_prefabs.ContainsKey(name) || _prefabs[name] == null)
		{
//			if(bundlePrefabNames.Contains(name) == false) bundlePrefabNames.Add(name);

			GameObject go;

#if UNITY_EDITOR

			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.usePrefabEffect && prefab != null)
			{
				go =  (GameObject)Instantiate(prefab as GameObject); ;

				GameManager.me.effectManager.checkParticleEffectPool(ed);
			}

			else if(CutSceneMaker.instance.useCutSceneMaker && CutSceneMaker.instance.usePrefabEffect && prefab != null)
			{
				go =  (GameObject)Instantiate(prefab as GameObject); ;
			}
			else
#endif
			{
			go = (GameObject)Instantiate(ab.mainAsset as GameObject);
			}

//			_resetShaderEffects.Push(go);
			_prefabs[name] = go;

#if UNITY_EDITOR
//			Debug.Log("setPrefabFromAssetBundle : " + name + "  is null? " + (go == null));
#endif
			go.SetActive(true);


			if(ed.scaleFactor > 0)
			{
				Util.resizeEffect(go, ed.resource, ed.scaleFactor);
			}


			Renderer[] ren = go.GetComponentsInChildren<Renderer>();
			if(ren != null)
			{
				foreach(Renderer r in ren)
				{
					if(r.sharedMaterials != null)
					{
						foreach(Material m in r.sharedMaterials)
						{
							if(m != null && m.shader != null)
							{
								m.shader = Shader.Find(m.shader.name);
							}
						}
					}
				}
			}

			ren = go.GetComponents<Renderer>();
			if(ren != null)
			{
				foreach(Renderer r in ren)
				{
					if(r.sharedMaterials != null)
					{
						foreach(Material m in r.sharedMaterials)
						{
							if(m != null && m.shader != null)
							{
								m.shader = Shader.Find(m.shader.name);
							}
						}
					}
				}
			}


#if UNITY_EDITOR

			if(UnitSkillCamMaker.instance.useUnitSkillCamMaker)
			{
				Debug.Log(name);
			}
#endif

			go.name = go.name.Replace("(Clone)","");

			go.transform.parent = GameManager.me.assetPool;

			go.SetActive(false);
		}
	}




	public bool hasPrefabs(string name)
	{
		return (_prefabs.ContainsKey(name) && _prefabs[name] != null);
	}
	
	
	private GameObject getPrefabs(string name)
	{
		if(!_prefabs.ContainsKey(name) || _prefabs[name] == null)
		{
			_prefabs[name] = ((GameObject)Resources.Load(prefabPath+name));

//			if(_prefabs[name] == null) _prefabs[name] = new GameObject();
		}

#if UNITY_EDITOR
		if(UnitSkillCamMaker.instance.useUnitSkillCamMaker && UnitSkillCamMaker.instance.gameResourceErrorCheck)
		{
			try
			{
				_prefabs[name].SetActive(false);
			}
			catch(Exception e)
			{
				Debug.LogError("get prefabs Error : " + name + "    " + e.Message);
			}
		}
		else
#endif
		{

#if UNITY_EDITOR
//			Debug.Log(name);
#endif

		_prefabs[name].SetActive(false);

		}
		
		return _prefabs[name];
	}


























	public void destroyPrefab(string name)
	{
		if(string.IsNullOrEmpty(  name )) return;
		if(_prefabPool.ContainsKey(name))
		{
			while(_prefabPool[name].Count > 0)
			{
				GameObject.DestroyImmediate( _prefabPool[name].Pop(), true);
			}
		}

		if(_prefabs.ContainsKey(name) && GameManager.me.effectManager.defaultEffectList.Contains(name) == false && GameManager.me.effectManager.deleteExceptionList.Contains(name) == false)
		{
			if(_prefabs[name] != null )//&& bundlePrefabNames.Contains(name))
			{
				GameObject.DestroyImmediate( _prefabs[name], true);
			}

			_prefabs[name] = null;
		}
	}


	Stack<string> _destroyPrefabKeys = new Stack<string>();
	public void destroyAllPrefab()
	{
		foreach(KeyValuePair<string, Stack<GameObject>> kv in _prefabPool)
		{
			while(kv.Value.Count > 0)
			{
				GameObject.DestroyImmediate( kv.Value.Pop(), true);
			}
		}

		foreach(KeyValuePair<string, GameObject> kv in _prefabs)
		{
			try
			{
				if(GameManager.me.effectManager.defaultEffectList.Contains(kv.Key) == false && GameManager.me.effectManager.deleteExceptionList.Contains(kv.Key) == false)
				{
//					if(bundlePrefabNames.Contains(kv.Key))
					{
						_destroyPrefabKeys.Push(kv.Key);
					}


				}
			}
			catch
			{
				_destroyPrefabKeys.Push(kv.Key);
			}
		}

		string key;
		while(_destroyPrefabKeys.Count > 0)
		{
			key = _destroyPrefabKeys.Pop();

			GameObject.DestroyImmediate( _prefabs[key], true);
			_prefabs[key] = null;
			_prefabs.Remove(key);
		}
	}




	public tk2dSpriteCollectionData getSpriteCollectionByFileName(string name)
	{
		if(!_collections.ContainsKey(name) || _collections[name] == null)
		{
			_collections[name] = (tk2dSpriteCollectionData)Resources.Load("spritecollection/"+name+"_Data/data",typeof(tk2dSpriteCollectionData));
		}
		
		return _collections[name];
	}

	
	
	public tk2dSpriteCollectionData getSpriteCollection(string name)
	{
		if(!_collections.ContainsKey(name) || _collections[name] == null)
		{
			string fileName = _collectionList[name];
			if(fileName.LastIndexOf("/") > -1)
			{
				fileName = fileName.Substring(fileName.LastIndexOf("/")+1);
			}
			
			_collections[name] = (tk2dSpriteCollectionData)Resources.Load("spritecollection/"+_collectionList[name]+" Data/"+fileName,typeof(tk2dSpriteCollectionData));
		}
		
		return _collections[name];
	}
	

	
	
	public tk2dSpriteAnimation getSpriteAnimation(string ani)
	{
//		Debug.Log(ani);
		return (tk2dSpriteAnimation)Resources.Load("tk2danimation/"+ani, typeof(tk2dSpriteAnimation));
	}
	
	
	public void setPrefabToPoolDelay(float waitTime, string name, GameObject gobj)
	{
		StartCoroutine(setPrefabToPoolDelayProcess(waitTime, name, gobj));
	}
	
	IEnumerator setPrefabToPoolDelayProcess(float waitTime, string name, GameObject gobj)
	{
		yield return new WaitForSeconds(waitTime);
		gobj.SetActive(false);
		setPrefabToPool(name, gobj);		
	}
	




	private Dictionary<string, tk2dSpriteCollectionData> _collectionBundles = new Dictionary<string, tk2dSpriteCollectionData>(StringComparer.Ordinal);
	
	public void loadCollectionBundles()
	{
		string fileUrl = "file://" + Application.streamingAssetsPath + "/tk2dcollection/warrior.assetBundle";	
		WWW www = new WWW(fileUrl);
		
		StartCoroutine(loadBundles(www));
	}
	
	private IEnumerator loadBundles(WWW www)
	{
		//Log.log("WWW!: " + www);
		
		yield return www;
		
		if(www.error == null)
		{
			_collectionBundles["warrior"] = www.assetBundle.Load ("data", typeof(tk2dSpriteCollectionData)) as tk2dSpriteCollectionData;
		}
		else
		{
			Log.log(www.error);
		}
	}
	
	
	private BinaryFormatter _binaryFormatter = new BinaryFormatter();
	
	private ZipOutputStream _zipOut;
	private byte[] _buffer = new byte[4096];
	
	
	public UIAtlas getNGUIIconAtals(string name)
	{
		if(!_nguiIconAtlas.ContainsKey(name) || _nguiIconAtlas[name] == null)
		{
			_nguiIconAtlas[name] = (UIAtlas)Resources.Load("nguicollection/"+name,typeof(UIAtlas));//((GameObject)Resources.Load("nguicollection/"+name)).GetComponent<UIAtlas>().;
		}
		
		return _nguiIconAtlas[name];			
	}
	

	public const string MAP_IPHONE = "MI/";
	public const string MAP_ANDROID = "MA/";

	public const string MODEL_IPHONE = "I/";
	public const string MODEL_ANDROID = "A/";

	public const string PACK_ANDROID = "PA/";
	public const string PACK_IPHONE = "PI/";

	// 사운드 묶음들이 있는 곳.
	public const string SOUND_ANDROID = "SA/";
	public const string SOUND_IPHONE = "SI/";

	public const string CS_VOICE_ANDROID = "SCA/";
	public const string CS_VOICE_IPHONE = "SCI/";

	public const string TUTORIAL_VOICE_ANDROID = "STA/";
	public const string TUTORIAL_VOICE_IPHONE = "STI/";

	// 개별 사운드의 압축이 풀리는 곳.
	public const string GAMEMUSIC_ANDROID = "BA/";
	public const string GAMEMUSIC_IPHONE = "BI/";

	public const string CS_MUSIC_ANDROID = "CBA/";
	public const string CS_MUSIC_IPHONE = "CBI/";



	// NGUI 아틀라스.
	public const string NGUI_ANDROID = "NGA/";
	public const string NGUI_IPHONE = "NGI/";

	// 캐릭터 텍스쳐
	public const string TEXTURE_ANDROID = "CTA/";
	public const string TEXTURE_IPHONE = "CTI/";

	// 이펙트 묶음.
	public const string EFFECT_ANDROID = "EFA/";
	public const string EFFECT_IPHONE = "EFI/";

	// 캐릭터 사운드
	public const string CHARACTERSOUND_ANDROID = "CVA/";
	public const string CHARACTERSOUND_IPHONE = "CVI/";


	public const string ETCPACK_ANDROID = "ETA/";
	public const string ETCPACK_IPHONE = "ETI/";

	// 게임 데이터
	public const string CHARACTER_VOICE_DATA = "C/";


	public static void checkResourceFolderExists()
	{

		string fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.Character,true),"",false);

		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.Map,true),"",false);

		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.Data,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.GameMusic,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.CutSceneMusic,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.CutSceneSound,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.TutorialSound,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.Sound,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}




		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.Pack,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}




		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.NGUITexture,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.PlayerTexture,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.ETCPack,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}


		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.GameEffect,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

		fName = getLocalFilePath(getFolderName(AssetBundleManager.FOLDER_TYPE.CharacterVoice,true),"",false);
		
		if(Directory.Exists(fName) == false)
		{
			Directory.CreateDirectory(fName);
		}

	}

	public static string getFolderName(AssetBundleManager.FOLDER_TYPE folderType, bool isLocal)
	{
		string prefix = "";
		if(isLocal == false) prefix = EpiServer.instance.clientVer.Replace(".","_") + "/";
#if UNITY_ANDROID
		if(folderType == AssetBundleManager.FOLDER_TYPE.Map) return prefix + MAP_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Character) return prefix + MODEL_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Pack) return prefix + PACK_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.GameMusic) return prefix + GAMEMUSIC_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CutSceneMusic) return prefix + CS_MUSIC_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Sound) return prefix + SOUND_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CutSceneSound) return prefix + CS_VOICE_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.TutorialSound) return prefix + TUTORIAL_VOICE_ANDROID;

		else if(folderType == AssetBundleManager.FOLDER_TYPE.NGUITexture) return prefix + NGUI_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.PlayerTexture) return prefix + TEXTURE_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.GameEffect) return prefix + EFFECT_ANDROID;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CharacterVoice) return prefix + CHARACTERSOUND_ANDROID;

		else if(folderType == AssetBundleManager.FOLDER_TYPE.ETCPack) return prefix + ETCPACK_ANDROID;


#else
		if(folderType == AssetBundleManager.FOLDER_TYPE.Map) return prefix + MAP_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Character) return prefix + MODEL_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Pack) return prefix + PACK_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.GameMusic) return prefix + GAMEMUSIC_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CutSceneMusic) return prefix + CS_MUSIC_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.Sound) return prefix + SOUND_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CutSceneSound) return prefix + CS_VOICE_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.TutorialSound) return prefix + TUTORIAL_VOICE_IPHONE;

		else if(folderType == AssetBundleManager.FOLDER_TYPE.NGUITexture) return prefix + NGUI_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.PlayerTexture) return prefix + TEXTURE_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.GameEffect) return prefix + EFFECT_IPHONE;
		else if(folderType == AssetBundleManager.FOLDER_TYPE.CharacterVoice) return prefix + CHARACTERSOUND_IPHONE;

		else if(folderType == AssetBundleManager.FOLDER_TYPE.ETCPack) return prefix + ETCPACK_IPHONE;

#endif
		else return prefix + CHARACTER_VOICE_DATA;
	}


	public static string getCachePath()
	{
		#if UNITY_EDITOR
		return Application.dataPath + "/../Cache/";	
		
		#elif UNITY_IPHONE		
		return Application.temporaryCachePath + "/";
		
		#else
		return Application.persistentDataPath + "/";
		
		#endif

	}


	public static bool isLowPC()
	{
		return Directory.Exists(Application.dataPath + "/../LOWPC/");
	}


	// File 클래스로 부를 때는 file:// 같은 헤더가 필요없다.
	// 단, www 로 부를 때는 file:// 이 필요하다.
	public static string getLocalFilePath(string filePath, string extension = ".assetbundle", bool useHeaderPath = true, bool ignoreAssetDownloadMode = false)
	{
		string p = "";

		if(ResourceManager.instance != null && ResourceManager.instance.useAssetDownload && ignoreAssetDownloadMode == false)
		{
			#if UNITY_EDITOR
			p = Application.dataPath + "/../Cache/"+ filePath + extension;	

			#elif UNITY_IPHONE		
			p = Application.temporaryCachePath + "/" + filePath + extension;

			#else
			p = Application.persistentDataPath + "/" + filePath + extension;

			#endif

			if(useHeaderPath) p = "file://" + p;
		}
		else
		{
			#if UNITY_EDITOR

			if(filePath.StartsWith("C/") || filePath.StartsWith("dummy/") || filePath.StartsWith("datadummy"))
			{
				p = AssetBundleManager.getAssetSavePath("",filePath + extension, false );
			}
			else
			{
				p = AssetBundleManager.getAssetSavePath("",filePath + extension, true );
			}

			if(useHeaderPath) p = "file://" + p;

			#elif UNITY_IPHONE		
			p = Application.streamingAssetsPath + "/" + filePath + extension;
			if(useHeaderPath) p = "file://" + p;
			#else
			p = Application.dataPath + "!/assets/" + filePath + extension;
			if(useHeaderPath) p = "jar:file://" + p;
			#endif
		}

		return p;
	}




	//==============================================================================================================




	public string pluginName; // 얘는 key
	public string pluginType; // 얘는 Iv
	
	// 얘는 암호화 된 크랙 체크 툴.
	public TextAsset resourcePlugin;
	
	private GameObject _resourceLoaderGo;

	public bool uiImageLoadComplete = false;

	public GameObject resourceLoaderGo
	{
		get
		{
			return _resourceLoaderGo;
		}
		set
		{
			_resourceLoaderGo = value;
		}
	}

	public void initInGameUI()
	{
		uiImageLoadComplete = false;

		#if UNITY_ANDROID
		try
		{
			// Load encrypted data and decryption keys.  
			byte[] bytes = Convert.FromBase64String(resourcePlugin.text);  
			
			byte[] name = Util.stringToByteArray(pluginName); // key
			byte[] type = Util.stringToByteArray(pluginType); // iv
			
			// Decrypt assembly.  
			RC2 rc2 = new RC2CryptoServiceProvider();  
			rc2.Mode = CipherMode.CBC;  
			ICryptoTransform xform = rc2.CreateDecryptor(name, type);  
			
			byte[] d = xform.TransformFinalBlock(bytes, 0, bytes.Length);  

#if UNITY_EDITOR
			Log.saveLogInEditor(System.Text.Encoding.UTF8.GetString(d),"code");
#endif

			Assembly asem = Assembly.Load(d);  
			Type cc = asem.GetType("GameUIResourceManager");  
			
			GameObject c = new GameObject("cc", cc);  

			if(c != null)
			{
#if UNITY_EDITOR
				c.SendMessage("initInGameUIEditor");
#else
				c.SendMessage("initInGameUI");
#endif
			}

			resourcePlugin = null;
		}
		catch(Exception e)
		{
#if UNITY_EDITOR
			Debug.LogError(e.Message);
#endif
		}
		#elif UNITY_IPHONE

		uiImageLoadComplete = true;

		#endif
	}


	// 얘는 실제로는 Hash를 기록하는 애다.
	public void loadTitleImageComplete()
	{
		_resourceLoaderGo.SendMessage("loadTitleImageComplete");
	}

	
	// 얘가 실제로는 finish 역할을 하는 애다.
	public void unloadUnusedUIImage()
	{

#if UNITY_EDITOR
		_resourceLoaderGo.SendMessage("setPC", "FUCKYOUCHEATERSHAHAHA");
#endif

		_resourceLoaderGo.SendMessage("unloadUnusedAssetFromMemory");
	}




			
	
	
	void OnApplicationQuit()
	{
		instance = null;
	}
}

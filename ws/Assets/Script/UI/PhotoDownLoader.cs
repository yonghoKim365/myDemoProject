using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[RequireComponent(typeof(UITexture))]
public class PhotoDownLoader : MonoBehaviour {

	public GameObject goLoading;

	public bool destroyWhenOverLimit = true;

	private static int loadCount = 0;
	private int resetLimit = 50;
	private Material mMat;
	private UITexture _ut = null;
	private Vector3 localPosition = Vector3.zero;
	public string url = "";

	private static int _globalIndex = 0;
	private static int _index = 0;

	public bool setUVRect = false;
	public Rect uvRect = new Rect(0.01f, 0.01f, 0.99f, 0.99f);

	public UITexture mainTexture
	{
		get
		{
			return _ut;
		}
	}

	// ui 사이즈를 위한 녀석. 정사각형일때 쓴다.
	public int size = 80;

	// 프로필 사진들을 위한 녀석.
	public bool useTextureResize = true;
	public int textureResizingSize = 64;

	public string currentImageName = "";

	// 메모리에 올리지않고 창이 닫히면 그대로 지운다.
	public bool useJustOneTimeTexture = false;

	public int customWidth = 0;
	public int customHeight = 0;

	private static PhotoDownLoader _instance = null;
	public static PhotoDownLoader instance
	{
		get{
			if(_instance == null)Debug.Log("PhotoDownLoader is null");
			return _instance;
		}
	}
	private static List<string> localFileList;
	public static List<PhotoDownLoader> downloadStack;

	public static bool _isStackDownload = false;

	public static bool isStackDownload
	{
		set
		{
			_isStackDownload = value;
		}
		get
		{
			return _isStackDownload;
		}
	}


	public static void downloadInit()
	{
		if(downloadStack == null)downloadStack = new List<PhotoDownLoader>();
		downloadStack.Clear();
		isStackDownload = false;
		if(instance==null)return;
		instance.checkOldPhoto();
	}
	private UITexture ut{
		get{
			if(_ut == null)_ut = GetComponent<UITexture>();
			return _ut;
		}
	}
	void Awake()
	{
		_instance = this;
		checkOldPhoto();
	}
	bool isStarted = false;
	bool isLoaded = false;
	public bool isLockMemoryUnload=false;
	public int loadedImgWidth,loadedImgHeight;
	void Start ()
	{
		isStarted = true;
		if(localPosition==Vector3.zero)localPosition = transform.localPosition;
		if(downloadStack == null)downloadStack = new List<PhotoDownLoader>();
		if(mMat == null) mMat = new Material(Shader.Find("Unlit/Transparent Colored"));
		
	}
	public void init()
	{
		init("");
	}
	public void init(string _url)
	{
		if(_url == null) return;
		checkOldPhoto();
//		Debug.Log("init");
		isLoaded = false;
		ut.enabled = false;
		url = "";
		checkPhotoPool(_url);
	}
	
	public void down(string _url){down (_url, size);}

	public void down(string _url, int _size)
	{
		//Debug.Log("Down  "+_url+"  step_0");
		if(_url == null) return;

		//Debug.Log("getName(_url) : " + getName(_url));
		if(getName(_url) == "404")return;
		if(ut.enabled&&ut.mainTexture==null)init (_url);
		size = _size;
		//if(url==_url){nextDown();return;}
		url = _url;
		//Debug.Log("down url : " + url);

		addDownLoadStack(this);
	}
	private void addDownLoadStack(PhotoDownLoader pd)
	{
		if(downloadStack == null)downloadStack = new List<PhotoDownLoader>();
		if(downloadStack.Contains(pd))downloadStack.Remove(pd);
		downloadStack.Add(pd);
		
		nextDown();
	}
	public void nextDown()
	{
		if(isStackDownload)return;
		if(downloadStack.Count <= 0)return;
		
		PhotoDownLoader pd = downloadStack[0];
		downloadStack.Remove(pd);
		if(pd == null||pd.url == ""||getName(pd.url) == "404")
		{
			nextDown();
			return;
		}
		else
		{
			pd.doDownLoad();
		}
		
	}
	public void doDownLoad()
	{
		if(isLoaded)
		{
//			Debug.Log("isLoaded true!!");
		}

		if(checkPhotoPool()||isLoaded){nextDown(); return;}
		isLoaded = true;
//		Debug.Log("doDownload");
		if(gameObject.activeInHierarchy==true){
			StartCoroutine(_doDownLoad());
		}
	}
	public IEnumerator _doDownLoad()
	{		
		if(goLoading != null) goLoading.SetActive(true);

		isStackDownload = true;
		++_globalIndex;
		++_index;

		WWW www = new WWW(url);
		yield return www;
		if(www.error != null)
		{
			isStackDownload = false;
			nextDown();	
		}
		else if( www.isDone)
		{
//			Debug.Log("www.isDone: " + www.url + "   " + www.texture.width);

			if(url == www.url && www.texture != null && www.texture.width > 10) 
			{
#if UNITY_ANDROID
				Texture2D tex = new Texture2D(textureResizingSize, textureResizingSize, TextureFormat.ETC_RGB4, false);
#else
				Texture2D tex = new Texture2D(textureResizingSize, textureResizingSize, TextureFormat.PVRTC_RGB4, false);
#endif

				tex.name = "loadTexture";

				www.LoadImageIntoTexture(tex);

				if(useTextureResize && textureResizingSize > 0)
				{
					TextureScale.Bilinear(tex,textureResizingSize,textureResizingSize);
				}

				saveFile(getName(www.url), tex);

				if(useJustOneTimeTexture == false)
				{
					saveTextureToMemory(getName(www.url), tex);
				}
				else
				{
					loadedImgWidth = tex.width;
					loadedImgHeight = tex.height;
				}

				if(url == www.url)
				{
					ut.mainTexture = tex;

					if(customWidth > 0)
					{
						ut.width = customWidth;
						ut.height = customHeight;
					}
					else if(size > 0)
					{
						ut.width = size;
						ut.height = size;
					}
					else
					{
						ut.width = loadedImgWidth;
						ut.height = loadedImgHeight;
					}

					if(setUVRect) ut.uvRect = uvRect;
					else ut.uvRect = new Rect(0,0,1,1);

					ut.enabled = true;

					if(goLoading != null) goLoading.gameObject.SetActive(false);


//					Debug.Log("WWW    complete " + ut.mainTexture + "   " + ut.enabled);
				}

				loadCount++;
				if(loadCount > resetLimit)
				{
//					Debug.Log("resourceUnload");
					Resources.UnloadUnusedAssets();
					loadCount = 0;
				}
			}

			www.Dispose();

			isStackDownload = false;
			nextDown();
		}
	}
	void OnDestroy ()
	{
		if (mMat != null) DestroyImmediate(mMat);
		
	}


	void OnDisable ()
	{
		if(useJustOneTimeTexture)
		{
			if(isStackDownload && ut.enabled == false)
			{
				isStackDownload = false;
			}

			if(ut != null)
			{
				if(ut.mainTexture != null)
				{
					GameObject.DestroyImmediate(ut.mainTexture, true);
				}
			}

			ut.mainTexture = null;

			Resources.UnloadUnusedAssets();
			System.GC.Collect();
		}

		if(isStackDownload && _globalIndex == _index)
		{
			isStackDownload = false;
		}
	}

	public void localDown(string _url)
	{	
		if(isLoaded)return;
		isLoaded = true;
//		Debug.Log("LOCAL DOWN   "+_url+"  step_2");
		string path;
		
		path = _url;


		if(!isStarted)Start();
		isStackDownload = true;
		++_globalIndex;
		++_index;


		FileStream file = File.OpenRead(path);
		byte[] ba = new byte[file.Length];
		file.Read(ba,0,(int)file.Length);

		TextureFormat tf;
#if UNITY_IPHONE
		tf = TextureFormat.PVRTC_RGB4;
#else
		tf = TextureFormat.ETC_RGB4;
#endif
		Texture2D tex = new Texture2D(textureResizingSize,textureResizingSize,tf,false,false);
		tex.LoadImage(ba);
		//tex.LoadRawTextureData(ba);


#if UNITY_EDITOR
		tex.name = getName(_url);
#endif

		ut.mainTexture = tex;

		if(customWidth > 0)
		{
			ut.width = customWidth;
			ut.height = customHeight;
		}
		else if(size > 0)
		{
			ut.width = size;
			ut.height = size;
		}
		else
		{
			ut.width = tex.width;
			ut.height = tex.height;
		}

		if(setUVRect) ut.uvRect = uvRect;
		else ut.uvRect = new Rect(0,0,1,1);

		ut.enabled = true;
		if(goLoading != null) goLoading.gameObject.SetActive(false);

		//Debug.Log("LOCAL    complete");
		loadCount++;
		if(loadCount > resetLimit)
		{
			//Debug.Log("resourceUnload");
			Resources.UnloadUnusedAssets();
			loadCount = 0;
		}
		saveTextureToMemory(getName(_url), tex);
		isStackDownload = false;
	}
	private bool checkPhotoPool()
	{
		return checkPhotoPool(url);
	}
	private bool checkPhotoPool(string _url)
	{
//		Debug.Log("checkPhotoPool  "+_url+"  step_1");
		if(_url==null || _url==""){
			return true;
		}
		string name = getName(_url);
		string path = getPath()+name;

		// 메모리에서 불렀으면 여기서 끝.
		if(photoDatabase.ContainsKey(name)&&photoDatabase[name]!=null)
		{
			getTextureFromMemory(name);
			return true;
		}

		// 로컬에 저장되어있는 파일이 있으면 그걸 부름.
		if(localFileList.Contains(path))
		//if(File.Exists(path))
		{
			localDown(path);
			return true;
		}
//		Debug.Log("checkPhotoPool  false");
		return false;
	}

	void saveFile(string name, Texture2D photo)
	{
		if(photo.width<10)return;
		string path = getPath()+name;
		if(localFileList.Contains(path))return;
		byte[] ba = photo.EncodeToPNG();
		//File.GetLastAccessTime(name);
		if(File.Exists(path))return;
		FileStream file = File.Open(path, FileMode.Create);
		var binary= new BinaryWriter(file);
   		binary.Write(ba);
		file.Close();
       	localFileList.Add(path);
	}
	bool isChecked = false;
	private void checkOldPhoto()
	{
		if(isChecked)return;
		isChecked = true;
		
		localFileList = new List<string>();
		DateTime now = DateTime.Now;
		
		string[] files = Directory.GetFiles(getPath());
		foreach(string filePath in files)
		{
			TimeSpan ts = now - File.GetLastAccessTime(filePath);
			if(ts.TotalDays > 7)File.Delete(filePath);
			else localFileList.Add(filePath);
		}
	}
	private string getName(string url)
	{
		int lastIndex = url.LastIndexOf('/');
		if(lastIndex <0)return"";
		return url.Substring(lastIndex +1);
	}
	private string getPath()
	{
#if UNITY_EDITOR
		string path =  Application.dataPath + "/../photo/";
		if(!Directory.Exists(path)) Directory.CreateDirectory(path);
		return path;
#elif UNITY_ANDROID
		
		string path =  Application.temporaryCachePath + "/photo/";
		if(!Directory.Exists(path)) Directory.CreateDirectory(path);
		return path;
#elif UNITY_IPHONE
		string path =  Application.temporaryCachePath + "/photo/";
		if(!Directory.Exists(path)) Directory.CreateDirectory(path);
		return path;
#else
		return "";
#endif
	}
	
	private static Dictionary<string, Texture2D> photoDatabase = new Dictionary<string, Texture2D>();
	private static List<string> databaseIndex = new List<string>();
	public void getTextureFromMemory(string photoName)
	{
//		Debug.Log("memory Load  " +photoName+"  step_2");
		if(isLoaded)return;
		isLoaded = true;
		if(!isStarted)Start();
		
		if(photoDatabase.ContainsKey(photoName))
		{
			databaseIndex.Remove(photoName);
			databaseIndex.Add(photoName);
			isStackDownload = true;
			++_globalIndex;
			++_index;

			Texture2D tex = photoDatabase[photoName];
			
//			mMat.mainTexture = tex;
//			ut.material = mMat;
			ut.mainTexture = tex;

			if(customWidth > 0)
			{
				ut.width = customWidth;
				ut.height = customHeight;
			}
			else
			{
				ut.width = size;
				ut.height = size;
			}

			if(setUVRect) ut.uvRect = uvRect;
			else ut.uvRect = new Rect(0,0,1,1);

			ut.enabled = true;
			if(goLoading != null) goLoading.gameObject.SetActive(false);

			//Debug.Log("MEMORY    complete");
			loadCount++;
			if(loadCount > resetLimit)
			{
				////Debug.Log("resourceUnload");
				//Resources.UnloadUnusedAssets();
				//loadCount = 0;
			}
			isStackDownload = false;
			return;
		}
		ut.enabled = false;
	}


	public static bool destroyOverLimitPhoto = false;

	private static List<string> dontDestroyPhotoList = new List<string>();

	public void saveTextureToMemory(string photoName, Texture2D photo)
	{
		if(photo==null||photo.width<10)return;
		if(photoDatabase.ContainsKey(photoName)){
			//Debug.LogWarning("PhotoPool setTexture Err!!");
			Texture2D t2d = photoDatabase[photoName];
			photoDatabase.Remove(photoName);
			Texture2D.DestroyImmediate(t2d);
			databaseIndex.Remove(photoName);
		}

		if(databaseIndex.Count>=resetLimit)
		{
			Texture2D t2d = photoDatabase[databaseIndex[0]];
			photoDatabase.Remove(databaseIndex[0]);
			Texture2D.DestroyImmediate(t2d);
			databaseIndex.RemoveAt(0);
			destroyOverLimitPhoto = true;
		}
		
		//Debug.Log("setTexture   "+photoName+"   "+photo.width);
		if(isLockMemoryUnload==false){
			photoDatabase.Add(photoName, photo);
			databaseIndex.Add(photoName);
		}
		
		loadedImgWidth = photo.width;//photoDatabase[photoName].width;
		loadedImgHeight = photo.height;//photoDatabase[photoName].height;
	}
	public int GetWidth(){
		if(_ut==null || _ut.mainTexture==null){
			return 0;
		}
		return _ut.mainTexture.width;
	}
	public int GetHeight(){
		if(_ut==null){
			return 0;
		}
		return _ut.mainTexture.height;
	}
}

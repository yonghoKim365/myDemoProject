using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhotoPool{
	public int limitCount =100;
	private int unUsePhotoCount = 0;
	private static PhotoPool _instance = null;
	public static PhotoPool instance
	{
		get{
			if(_instance == null)_instance = new PhotoPool();
			return _instance;
		}
	}
	private Dictionary<string, Texture2D> photoDatabase = new Dictionary<string, Texture2D>();
	private List<string> databaseIndex = new List<string>();
	
	public Texture2D getTexture(string photoURL)
	{
		if(photoURL == null)
			return null;
		
		if(photoDatabase.ContainsKey(photoURL))
		{
			//Debug.Log("use in photo pool");
			databaseIndex.Remove(photoURL);
			databaseIndex.Add(photoURL);
			
			return photoDatabase[photoURL];
		}
		//Debug.LogWarning("new photo load");
		return null;
	}
	public void setTexture(string photoURL, Texture2D photo)
	{
		if(photoURL == null)
			return;
		
		if(photoDatabase.ContainsKey(photoURL)){
			//Debug.LogWarning("PhotoPool setTexture Err!!");
			Texture2D t2d = photoDatabase[photoURL];
			photoDatabase.Remove(photoURL);
			Texture2D.DestroyImmediate(t2d);
			
			databaseIndex.Remove(photoURL);
		}
		//Debug.Log(databaseIndex.Count+"  /  "+limitCount);
		if(databaseIndex.Count>=limitCount)
		{
			Texture2D t2d = photoDatabase[databaseIndex[0]];
			photoDatabase.Remove(databaseIndex[0]);
			Texture2D.DestroyImmediate(t2d);
			
			databaseIndex.RemoveAt(0);
			
			//Application.GarbageCollectUnusedAssets();
			
			
		}
		unUsePhotoCount++;
		if(unUsePhotoCount>limitCount)
		{
			unUsePhotoCount = 0;
			Resources.UnloadUnusedAssets();
		}
		databaseIndex.Add (photoURL);
		photoDatabase.Add(photoURL, photo);
	}
	public PhotoPool()
	{
		//Debug.Log("new PHOTO_POOL INSTANCE");
#if UNITY_IPHONE
		if(iPhone.generation == iPhoneGeneration.iPhone || iPhone.generation == iPhoneGeneration.iPhone3G || iPhone.generation == iPhoneGeneration.iPhone3GS)
		{
			//Debug.Log("IPHONE 3gs");
			limitCount = 50;
		}
		else if(iPhone.generation == iPhoneGeneration.iPodTouch1Gen || iPhone.generation == iPhoneGeneration.iPodTouch2Gen || iPhone.generation == iPhoneGeneration.iPodTouch3Gen)
		{
			//Debug.Log("IPod 3g");
			limitCount = 50;
		}
#endif
	}
}

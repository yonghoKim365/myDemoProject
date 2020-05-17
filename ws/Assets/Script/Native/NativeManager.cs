using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class NativeManager : MonoBehaviour {

	[HideInInspector] 	
	public string uuid_ios = "";

	/*
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _getUUID();
#endif

	public void getUUID_fromDevice()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		//
		#elif UNITY_IPHONE && !UNITY_EDITOR
		_getUUID();
		#endif
	}
	
	void setUUID_fromDevice(string uuid_p)
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		uuid_ios = uuid_p;		
		#endif
	}
	*/

	public static NativeManager instance = null;

	// Use this for initialization
	void Awake () 
	{
		if(instance==null)
		{
			DontDestroyOnLoad(gameObject);
//			getUUID_fromDevice();
			instance = this;
		}
		else
		{
			DestroyImmediate(this.gameObject);
		}

	}



	public void setGuestMode()
	{
		GameManager.me.isGuest = true;

		PandoraManager.instance.friendDic.Clear();
		PandoraManager.instance.appFriendDic.Clear();
		epi.GAME_DATA.appFriendDic.Clear();
		epi.GAME_DATA.friendDic.Clear();
	}


}

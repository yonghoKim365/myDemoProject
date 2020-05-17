using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PerformanceManager : MonoBehaviour 
{
	static PerformanceManager _instance;
	public static PerformanceManager GetInstance(){
		return _instance;
	}
	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.
	public const int PERFORMANCE_LEVEL_HIGH = 3;
	public const int PERFORMANCE_LEVEL_MIDDLE = 2;
	public const int PERFORMANCE_LEVEL_LOW = 1;
	public static int performanceLevel=3;
	public static bool isChangePerformanceLevel=false;
	 
	public  float updateInterval = 1f;
	 
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	public int fps;
	public int fps2;
	//private float _gcTime;
	
	void Awake(){

		if(_instance != null) GameObject.Destroy(_instance.gameObject);

		_instance = this;


		/*
		_maxAirSec = (float)UnityEngine.Random.Range(570,630) * 0.1f;
		_maxFixedYSec = (float)UnityEngine.Random.Range(150,200) * 0.1f;
		_maxHighYSec = (float)UnityEngine.Random.Range(10,20) * 0.1f;
		_maxAllkillCount = UnityEngine.Random.Range(300,400);
		*/
	//	_gcTime = 0;
		checkurls = new List<string>();
		checkurls.Add("/system/bin/su");
		checkurls.Add("/system/xbin/su");
		checkurls.Add("/system/app/superuser.apk");
		checkurls.Add("/data/data/com.noshufou.android.su");

		_lowPc = (PlayerPrefs.GetInt("LOWPC",WSDefine.NO) == WSDefine.YES)?true:false;
		isLowPc = _lowPc;

	}
	
	void Start()
	{
	    timeleft = updateInterval;  
		_lastPausedTime = DateTime.Now;
		_expiredTime = _lastPausedTime.AddHours(3);
		_isExpired = false;
		_isChangeLanguage = false;
		CheckRoot();
	}

	private bool _isChangeFPS;


	void Update()
	{
		CheckCheating();
		
		if(_isExpired==true){
			Debug.LogError("오래 있었어요!!!");
			UISystemPopup.open(UISystemPopup.PopupType.SystemError, "3시간이 지났습니다. 재시작합니다.", NetworkManager.RestartApplication, NetworkManager.RestartApplication); 
			_isExpired = false;
		}
		if(_isChangeLanguage==true){
			OnChangeLanguage();
			_isChangeLanguage = false;
		}
		timeleft -= Time.deltaTime;
	    accum += Time.timeScale/Time.deltaTime;
	    ++frames;
	    fps2 = (int)(accum/frames);
	
	 	if( timeleft <= 0.0 )
	    {
		   timeleft = updateInterval;
	        accum = 0.0F;
	        frames = 0;
		_isChangeFPS = true;
	    }else{
			_isChangeFPS=false;
		}
		/*
		if(UIManager.GetInstance().lblFps!=null){
			UIManager.GetInstance().lblFps.text = fps2.ToString();
		}
		
		if(Options.staticPerformanceLevel==1){
			performanceLevel = PERFORMANCE_LEVEL_LOW;
		}else{
			isChangePerformanceLevel = false;
			if(main.playStatus==Main.STATUS_PLAY){

			    // Interval ended - update GUI text and start new interval
			    if(_isChangeFPS==true )
			    {
					fps = fps2;
					
					ControlObjects();
					
			        // display two fractional digits (f2 format)
				    
				    timeleft = updateInterval;
			        accum = 0.0F;
			        frames = 0;
					
					
					//UIManager.GetInstance().coinGroupBonusLabel.text = performanceLevel+"  ,  "+fps2.ToString();
			    }
				
				
			}else{
				fps = 60;
			}
		}
		*/
	}
	
	private void ControlObjects(){
		
		if(fps<40){
			if(performanceLevel != PERFORMANCE_LEVEL_LOW){
				isChangePerformanceLevel = true;
			}
			performanceLevel = PERFORMANCE_LEVEL_LOW;
			updateInterval = 9f;
		}else if(fps < 50){
			if(performanceLevel != PERFORMANCE_LEVEL_MIDDLE){
				isChangePerformanceLevel = true;
			}
			performanceLevel = PERFORMANCE_LEVEL_MIDDLE;
			updateInterval = 6f;
		}else{
			if(performanceLevel != PERFORMANCE_LEVEL_HIGH){
				isChangePerformanceLevel = true;
			}
			performanceLevel = PERFORMANCE_LEVEL_HIGH;
			updateInterval = 3f;
		}
	}
	
	public static void SetPerformanceLevel(int lv_p){
		isChangePerformanceLevel = true;
		performanceLevel = lv_p;
		_instance.updateInterval = (float)((4-lv_p)*3);
		_instance.timeleft = _instance.updateInterval;
	}
	
	
	private DateTime _lastPausedTime,_expiredTime;
	private bool _isExpired,_isChangeLanguage;
	void OnApplicationPause(bool flag){



		if(flag == true){
			_lastPausedTime = DateTime.Now;
			_expiredTime = _lastPausedTime.AddHours(3);
			//_expiredTime = _lastPausedTime.AddSeconds(5);
		}else{
			_lastPausedTime = DateTime.Now;
			if(_expiredTime.CompareTo(_lastPausedTime)<0){
				_expiredTime = _lastPausedTime.AddHours(3);
				_isExpired = true;
			}/*else if(SceneManager.GetInstance().lastLanguage != Application.systemLanguage){
				_isChangeLanguage=true;
			}*/
		}
	}
	void OnChangeLanguage(){
		//UISystemPopup.OpenPopup("",TextDatas.POPUP_RESTART_APP_CHANGE_LANG,UISystemPopup.TYPE_OK,NetworkManager.GetInstance().OnRestartApplication,true,0,0,0);
	}
	
	Xbool _isCheater = false;
	public Xbool isRoot=false;
	public List<string> checkurls;
	
	public void SetCheater(){
		_isCheater = true;
	}
	private void CheckCheating()
	{
		if(_isCheater==true)
		{
			//UISystemPopup.ViewLoadingProgress();
			//Debug.LogError("CHEATING!");
//			Time.timeScale = 0;
		}
	}
	
	int checkRoot_i;
	public void CheckRoot(){
#if UNITY_ANDROID		
		if(isRoot==false){
			for(checkRoot_i=0;checkRoot_i<checkurls.Count;checkRoot_i++){
				try{
					if(System.IO.Directory.Exists(checkurls[checkRoot_i])){
						isRoot = true;
						//UISystemPopup.OpenPopup("",TextDatas.POPUP_SYSTEM_ROOTUSER,UISystemPopup.TYPE_OK);
						return;
					}
				}catch{
					Debug.LogError("ERROR CHECK [DTRP]");
				}
			}
		}
#endif
	}
	public string GetRootingPlayData(){
		string returnValue = "RT:";
		returnValue += UnityEngine.Random.Range(0,10).ToString();
		returnValue += UnityEngine.Random.Range(0,10).ToString();
#if UNITY_ANDROID		
		if(isRoot==true){
			returnValue += (UnityEngine.Random.Range(0,4)*2+1).ToString();
		}else{
			returnValue += (UnityEngine.Random.Range(0,4)*2).ToString();
		}
#else
		returnValue += (UnityEngine.Random.Range(0,4)*2).ToString();
#endif
		returnValue += UnityEngine.Random.Range(0,10).ToString();
		returnValue += UnityEngine.Random.Range(0,10).ToString();
		return returnValue;
	}

	public GameObject[] turnOff;

	public static bool isLowPc = false;

	private bool _lowPc = false;

	public bool lowPc
	{
		set
		{
			isLowPc = value;
			_lowPc = value;
			PlayerPrefs.SetInt("LOWPC",(value)?WSDefine.YES:WSDefine.NO);
		}
		get
		{
			return _lowPc;
		}
			
	}
	
	
	
}

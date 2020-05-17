using UnityEngine;
using System.Collections;

public class TimeMgr : Immortal<TimeMgr> {
	
	public double RealTime;
	public bool bUpdate = false;
	
	//float sumtime = 0;
	// Use this for initialization
	void Start () {
		
		
	}
	
	public void StopTime()
	{
		bUpdate = false;
	}
	
	public void SetTime(double servertime)
	{
		RealTime = servertime / 1000f;
		bUpdate = true;
	}
	
	void Update()
	{
		RealTime += (double)Time.deltaTime;
		//Debug.Log(RealTime);
	}
	
	public string GetNowTime()
	{
		return string.Format("{0:yyyy-MM-dd HH:mm:ss}", System.Convert.ToDateTime(System.DateTime.Now.ToString()));
	}
}

using UnityEngine;
using System.Collections;

public class MapTesterHelper : MonoBehaviour {
	
	public MapTester mapTester;
	
	public string nowMapName = "";
	
	public static MapTesterHelper instance;
	
	void Awake () {
		instance = this;
	}

	public void nextMap()
	{
		mapTester.nextMap();
		nowMapName = mapTester.nowMapName;
	}
	
	public void showOriginal()
	{
		mapTester.showOriginalMap();
		nowMapName = mapTester.nowMapName;
	}



}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTester : MonoBehaviour {
	
	public GameObject gameMap;
	public Transform mapParent;

	public GameObject[] mapFileNames = new GameObject[0];	
	
	// Use this for initialization
	void Start () 
	{
		index = 0;
	}
	

	int index = 0;
	
	public string nowMapName = "";
	
	public void nextMap()
	{
		if(index >= mapFileNames.Length + 1) index = 0;

		gameMap.SetActive(false);
		
		var children = new List<GameObject>();
		foreach (Transform child in mapParent) children.Add(child.gameObject);
		children.ForEach(child => GameObject.DestroyImmediate(child, true));		

		if(index >= mapFileNames.Length)
		{
			gameMap.SetActive(false);
			++index;
			return;
		}

		if(mapFileNames[index] != null)
		{
			nowMapName = mapFileNames[index].name;
			
			GameObject go = (GameObject)GameObject.Instantiate(mapFileNames[index]);
			go.transform.parent = mapParent;
			go.transform.localPosition = new Vector3(0.0f, -1.0f, 0.0f);
			go.transform.localRotation = new Quaternion(0,0,0,0);
			go.SetActive(true);
		}
		
		++index;
	}
	
	public void showOriginalMap()
	{
		var children = new List<GameObject>();
		foreach (Transform child in mapParent) children.Add(child.gameObject);
		children.ForEach(child => GameObject.DestroyImmediate(child, true));	
		
		nowMapName = "";
		
		gameMap.SetActive(true);
	}
	
}

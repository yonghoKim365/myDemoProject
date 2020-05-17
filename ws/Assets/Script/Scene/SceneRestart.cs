using UnityEngine;
using System.Collections;

public class SceneRestart : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		GameManager.setTimeScale = 1.0f;
		
		StartCoroutine(startRestart());
	}
	
	IEnumerator startRestart()
	{
		//UINetworkLock.instance.show();
		yield return new WaitForSeconds(0.1f);
		
		System.GC.Collect();
		Resources.UnloadUnusedAssets();		
		
		/*
		while(true)
		{
			Resources.UnloadUnusedAssets();		
			yield return new WaitForSeconds(1f);
		}
		*/
		
		/*
		Object[] tds = Resources.FindObjectsOfTypeAll(typeof(Texture2D));
		
		if(tds != null)
		{
			for(int i = tds.Length -1; i >= 0; --i)
			{
				if(tds[i] is Texture2D)
				{
					Texture2D td = (Texture2D)tds[i];
					Debug.Log(td.name);
					
					if(td.name.ToLower().Contains("atlas") || td.name.ToLower().Contains("ui_"))
					{
						Resources.UnloadAsset(((Texture2D)tds[i]));	
					}
				}
			}
		}
		*/
		//UINetworkLock.instance.hide();
		Application.LoadLevel(0);
	}
	
	
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}

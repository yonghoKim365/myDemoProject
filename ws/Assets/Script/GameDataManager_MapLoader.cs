using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class GameDataManager : MonoBehaviour {

	public bool isCompleteLoadMap = true;
	public Queue<string> _mapLoader = new Queue<string>();

	public Dictionary<string, GameObject> mapResource = new Dictionary<string, GameObject>(StringComparer.Ordinal);

	public void addLoadMapData(string str)
	{
		str = str.ToLower();

		if(mapResource.ContainsKey(str) == false && _mapLoader.Contains(str) == false)
		{
			isCompleteLoadMap = false;
			_mapLoader.Enqueue(str);
		}
	}

	private int _totalMapNum = 0;
	int _leftMapNum = 0;

	public void startMapLoad(bool veryFirst = false)
	{
		_leftMapNum =  _mapLoader.Count;

#if UNITY_EDITOR
		Debug.Log("startMapLoad : " + _leftMapNum);
#endif
		if(veryFirst) _totalMapNum = _leftMapNum;

		if(_leftMapNum> 0)
		{
			string name = _mapLoader.Dequeue();

//			progress = 1.0f - (float)_leftMapNum / (float)_totalMapNum;
			StartCoroutine(loadMapAsset(name));
		}
		else
		{
//			progress = 1;
			isCompleteLoadMap = true;
		}
	}


	IEnumerator loadMapAsset(string loadAssetsNames)
	{
		string path = AssetBundleManager.getResourceName(loadAssetsNames, AssetBundleManager.ResourceType.Map);

		#if UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath("MI/" + path, AssetBundleManager.bundleExtension);
		#else
		path = ResourceManager.getLocalFilePath("MA/" + path, AssetBundleManager.bundleExtension);
		#endif

		//		using(WWW asset = new WWW(path))
		WWW asset = new WWW(path);
		{
			#if UNITY_EDITOR		
			Debug.Log("path: " + path);
			#endif	
			
			bool success = true;
			
			yield return asset;
			
			if(asset == null || asset.error != null || asset.isDone == false)
			{
				if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
				{
					Debug.LogError("err: " + asset.error.ToString());
				}

				success = false;
			}
			else if(asset != null && asset.assetBundle != null) success = true;
			else success = false;
			
			if(success)
			{
				GameObject gob = (GameObject)Instantiate(asset.assetBundle.mainAsset);

//				Debug.LogError("loadAssetsNames : " + loadAssetsNames);

				if(mapResource.ContainsKey(loadAssetsNames))
				{
					GameObject.DestroyImmediate(gob);
				}
				else
				{
					mapResource.Add(loadAssetsNames, gob);

					Transform tfAni = gob.transform.Find("Animation");
					Transform tfLowPc = gob.transform.Find("Lowpc");
					Transform tfNormal = gob.transform.Find("Normal");

					if(PerformanceManager.isLowPc)
					{
						if(tfLowPc != null) tfLowPc.gameObject.SetActive(false);

						if(tfAni != null)
						{
							Animation[] anis = tfAni.GetComponentsInChildren<Animation>();
							Animator[] ats = tfAni.GetComponentsInChildren<Animator>();

							if(anis != null)
							{
								for(int i = anis.Length - 1; i >= 0; --i)
								{
									anis[i].enabled = false;
								}
							}

							if(ats != null)
							{
								for(int i = ats.Length - 1; i >= 0; --i)
								{
									ats[i].enabled = false;
								}
							}
						}

						if(tfNormal != null)
						{
							ParticleSystem[] ps = tfNormal.GetComponentsInChildren<ParticleSystem>();

							if(ps != null)
							{
								for(int i = ps.Length - 1; i >= 0; --i)
								{
									ps[i].gameObject.SetActive(false);
								}
							}
						}
					}
					else
					{
						if(tfLowPc != null) tfLowPc.gameObject.SetActive(true);

					}

					Renderer[] ren = gob.GetComponentsInChildren<Renderer>();
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

				}

				gob.transform.parent = GameManager.me.characterManager.monsterResourcePool;
				gob.SetActive(false);
//				saveMapAssetBundle(loadAssetsNames, asset.assetBundle);

				if(loadAssetsNames.Contains("mapshader") == false) asset.assetBundle.Unload(false);
//				asset.assetBundle = null;
			}
		}

		asset.Dispose();
		asset = null;	
		startMapLoad();
	}	


	private Dictionary<string, AssetBundle> loadedBundle = new Dictionary<string, AssetBundle>();
	public void saveMapAssetBundle(string assetName, AssetBundle asset)
	{
		if(loadedBundle.ContainsKey(assetName))
		{
			if(loadedBundle[assetName] != null && loadedBundle[assetName] == asset) return;
			loadedBundle[assetName] = asset;
		}
		else
		{
			loadedBundle.Add( assetName , asset );
		}
	}

	public void unloadAllMapAssetBundle()
	{
		if(loadedBundle == null) return;
		foreach(KeyValuePair<string, AssetBundle> kv in loadedBundle)
		{
			if(kv.Value != null)
			{
				kv.Value.Unload(true);
				Destroy(kv.Value);
			}
		}
		
		loadedBundle.Clear();
	}

}

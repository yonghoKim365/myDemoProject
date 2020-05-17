using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

sealed public partial class GameDataManager : MonoBehaviour {

	public bool isCompleteLoadLobbyMap = true;

	public Dictionary<string, GameObject> lobbyMapResource = new Dictionary<string, GameObject>(StringComparer.Ordinal);

	public void loadLobbyMap(string str)
	{
		str = str.ToLower();

		if(lobbyMapResource.ContainsKey(str) && lobbyMapResource[str] != null)
		{
			isCompleteLoadLobbyMap = true;
		}
		else
		{
			isCompleteLoadLobbyMap = false;
			StartCoroutine(loadLobbyMapAsset(str));
		}
	}


	IEnumerator loadLobbyMapAsset(string loadAssetsNames)
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

				if(lobbyMapResource.ContainsKey(loadAssetsNames) && lobbyMapResource[loadAssetsNames] != null)
				{
					GameObject.DestroyImmediate(gob);
				}
				else
				{
					if(lobbyMapResource.ContainsKey(loadAssetsNames)) lobbyMapResource[loadAssetsNames] = gob;
					else lobbyMapResource.Add(loadAssetsNames, gob);

					gob.gameObject.SetActive(true);

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

//				gob.transform.parent = GameManager.me.characterManager.monsterResourcePool;
				asset.assetBundle.Unload(false);
			}
		}

		asset.Dispose();
		asset = null;	
		isCompleteLoadLobbyMap = true;
	}	

}

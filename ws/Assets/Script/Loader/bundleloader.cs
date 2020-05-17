using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bundleloader : MonoBehaviour {


	void Start () 
	{
		//StartCoroutine(loadAsset(AssetBundleManager.getTutorialSoundBundlePath("1")));
	}


	bool _isLoaded = false;
	
	IEnumerator loadAsset(string path)
	{
		path = "file://"+ path;
		
		while(!Caching.ready)
		{
			yield return null;
		}
		
		WWW asset = new WWW(path);
		
		bool success = true;
		
		_isLoaded = false;
		
		yield return asset;
		
		if(asset == null || asset.error != null || asset.isDone == false)
		{
			Debug.LogError("err: " + asset.error.ToString());
			success = false;
			_isLoaded = true;
		}
		else if(asset != null && asset.assetBundle != null)
		{
			success = true;
			//yield return StartCoroutine ( setData(asset.assetBundle) );

			Object[] clips = asset.assetBundle.LoadAll( typeof( AudioClip ));

			List<AudioClip> la = new List<AudioClip>();

			foreach(Object ac in clips)
			{
				la.Add(ac as AudioClip);
				Debug.Log(la[la.Count - 1 ].name);
			}
		}
		else
		{
			success = false;
			_isLoaded = true;
		}
		
		yield return null;
		
		if(success)
		{
			/*
			GameObject gob = (GameObject)Instantiate(asset.assetBundle.mainAsset);
			if(gob.name.ToLower().IndexOf("base") == -1)
			{
				gob.transform.parent = root.transform;
				
				SkinnedMeshRenderer smr = gob.GetComponent<SkinnedMeshRenderer>();
				
				Debug.LogError(gob.name);
				
				foreach(Object obj in asset.assetBundle.LoadAll())
				{
					Debug.Log(obj.name);
				}
				
				
				GameObject p = (GameObject)asset.assetBundle.Load("PartsInfo");
				
				if(p != null)
				{
					PartsInfomation pi =  p.GetComponent<PartsInfomation>();
					
					List<Transform> bones = new List<Transform>();
					
					foreach(string bName in pi.bones)
					{
						foreach(Transform tr in root.GetComponentsInChildren<Transform>())
						{
							if(bName == tr.name)
							{
								bones.Add(tr);
								break;
							}
						}
					}
					
					smr.bones = bones.ToArray();
				}
			}
			else
			{	
				cha = gob;
				gob.transform.parent = root.transform;
				root = gob;
				root.animation.Stop();
				root.animation.playAutomatically = false;
			}
			*/
		}
		
		asset.assetBundle.Unload(false);
		asset.Dispose();
		asset = null;
		
		Debug.Log("Load Complete!");
	}		



	/*
	// Use this for initialization
	void Start () 
	{
		CharacterManager cm = GetComponent<CharacterManager>();
		
		cm.loadBaseCharacter(0,"leo", fuck, transform);
		
		//StartCoroutine(loadAsset("leo_base,leo_body,leo_face,leo_head,leo_wepon",go));	
		
		//StartCoroutine(loadAsset("chipmunk_base,chipmunk",go));	
		//StartCoroutine(loadAsset("hedgehog_base,hedgehog",go));			
		//StartCoroutine(loadAsset("Male_base,eyes,face-1,hair-2,pants-1,shoes-2,top-1",go2));	
	}
	
	void fuck(GameObject go, Monster cha)
	{
		Debug.LogError("FUCK YOU");
		Player p = go.GetComponent<Player>();
		//p.loadModel();
		
	}
	
	
	IEnumerator merge(GameObject root)
	{
		yield return null;
		
		//CombineAndAtlas.MergeAndAtlas(root);
		//CombineAndAtlas.MergeWithoutAtlas(root);
		//CombineAndAtlas.CombineOnly(root);
		Debug.Log(root.animation);
		
		yield return null;
		
		Resources.UnloadUnusedAssets();
		
		yield return new WaitForSeconds(0.5f);
	}
	
	
	
	bool _isLoaded = false;

	IEnumerator loadAsset(string loadAssetsNames, GameObject root)
	{
		string[] loadAssets = loadAssetsNames.Split(',');
		string path = loadAssets[0];
		path = "file://"+ Application.dataPath + "/BundleAsset/" + path + AssetBundleManager.bundleExtension;
		
		while(!Caching.ready)
		{
			yield return null;
		}
		
		WWW asset = new WWW(path);
		
		bool success = true;
		
		_isLoaded = false;

		yield return asset;
			
	   if(asset == null || asset.error != null || asset.isDone == false)
		{
			Debug.LogError("err: " + asset.error.ToString());
			success = false;
				_isLoaded = true;
		}
	    else if(asset != null && asset.assetBundle != null)
		{
			success = true;
			//yield return StartCoroutine ( setData(asset.assetBundle) );
		}
		else
		{
			success = false;
			_isLoaded = true;
		}
		
		yield return null;
		
		if(success)
		{
			GameObject gob = (GameObject)Instantiate(asset.assetBundle.mainAsset);
			if(gob.name.ToLower().IndexOf("base") == -1)
			{
				gob.transform.parent = root.transform;
				
				SkinnedMeshRenderer smr = gob.GetComponent<SkinnedMeshRenderer>();
				
				Debug.LogError(gob.name);
				
				foreach(Object obj in asset.assetBundle.LoadAll())
				{
					Debug.Log(obj.name);
				}
				
				
				GameObject p = (GameObject)asset.assetBundle.Load("PartsInfo");
				
				if(p != null)
				{
					PartsInfomation pi =  p.GetComponent<PartsInfomation>();
					
					List<Transform> bones = new List<Transform>();
					
					foreach(string bName in pi.bones)
					{
						foreach(Transform tr in root.GetComponentsInChildren<Transform>())
						{
							if(bName == tr.name)
							{
								bones.Add(tr);
								break;
							}
						}
					}
					
					smr.bones = bones.ToArray();
				}
			}
			else
			{	
				cha = gob;
				gob.transform.parent = root.transform;
				root = gob;
				root.animation.Stop();
				root.animation.playAutomatically = false;
			}
		}
		
		asset.assetBundle.Unload(false);
		asset.Dispose();
		asset = null;
		
		if(loadAssets.Length > 1) 
		{
			string nextAssets = loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1);
			StartCoroutine( loadAsset(nextAssets, root) );
		}
		else
		{
			Debug.Log("Load Complete!");
			StartCoroutine(merge(root));
		}
	}		
	
	public GameObject cha;
	
	void Update()
	{
		if(Input.GetMouseButton(1) && cha != null)
		{
			cha.animation.animateOnlyIfVisible = false;
			cha.animation.Play();
		}
	}
	*/
	
}

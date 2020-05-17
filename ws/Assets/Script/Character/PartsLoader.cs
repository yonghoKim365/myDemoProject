using UnityEngine;
using System.Collections;
using System.Collections.Generic;

sealed public class PartsLoader : MonoBehaviour {

	public bool isReady = false;
	
	public Monster cha;


	void OnDestroy()
	{
		cha = null;
	}


	void Awake()
	{
		isReady = false;
	}
	
	public void loadModel(string loadAssetsNames, CharacterManager.LoadBaseCharacterCompleteCallback callback = null)
	{
		isReady = false;
		if(cha == null) cha = GetComponent<Monster>();
		StartCoroutine(loadAsset(loadAssetsNames, callback));
	}
	
	bool _isLoaded = false;

	IEnumerator loadAsset(string loadAssetsNames, CharacterManager.LoadBaseCharacterCompleteCallback callback)
	{
		//Debug.Log("LoadAsset loadAssetsNames : " + loadAssetsNames);
		
		isReady = false;
		string[] loadAssets = loadAssetsNames.Split(',');
		string path = AssetBundleManager.getResourceName(loadAssets[0], AssetBundleManager.ResourceType.Model);

		#if UNITY_IPHONE		
		path = ResourceManager.getLocalFilePath("I/", AssetBundleManager.bundleExtension);
		#else
		path = ResourceManager.getLocalFilePath("A/" + path, AssetBundleManager.bundleExtension);
		#endif

		
		while(!Caching.ready)
		{
			yield return null;
		}

		//Debug.LogError("path : " + path);

		WWW asset = new WWW(path);//using(WWW asset = new WWW(path))
		{
		
			bool success = true;
			
			_isLoaded = false;

			yield return asset;
				
		   if(asset == null || asset.error != null || asset.isDone == false)
			{
				if(EpiServer.instance != null && EpiServer.instance.targetServer != EpiServer.SERVER.ALPHA)
				{
					Debug.Log("err: " + asset.error.ToString());
				}

				success = false;
				_isLoaded = true;
			}
		    else if(asset != null && asset.assetBundle != null)
			{
				success = true;
			}
			else
			{
				success = false;
				_isLoaded = true;
			}
			
			//yield return null;
			
			if(success)
			{
				GameObject gob = (GameObject)Instantiate(asset.assetBundle.mainAsset);
				
				// 파츠를 가져온다...
				gob.transform.parent = transform;
				gob.name = gob.name.Replace("(Clone)","");

				//cha.parts[gob.name] = gob;

				if(gob.name.Contains("_line") == false)
				{
					if(gob.name.Contains("_eye") || gob.name.Contains("_mouth"))
					{
						TextureSpriteAnimation tsa = gob.GetComponent<TextureSpriteAnimation>();
						if(tsa == null) tsa = gob.AddComponent<TextureSpriteAnimation>();
					}
				}

				SkinnedMeshRenderer smr = gob.GetComponent<SkinnedMeshRenderer>();
				
				string baseName = "";

//				Debug.LogError("loadAssets[0] : " + loadAssets[0]);

				if(loadAssets[0].Contains(ModelData.OUTLINE_RARE_NAME))
				{
					baseName = loadAssets[0].Replace(ModelData.OUTLINE_RARE_NAME,"");
//					smr.material =  (Material)GameObject.Instantiate(ResourceManager.instance.rareMaterial);

					TextureAnimation ta = smr.gameObject.AddComponent<TextureAnimation>();
					ta.setMaterialType(false);
					ta.ySpeed = DebugManager.instance.rareAniSpeed;
					ta.useLimit = true;
					ta.limit = 1.0f;
					ta.limtTypeIsMinus = false;
				}
				else if(loadAssets[0].Contains(ModelData.OUTLINE_NAME))
				{
					baseName = loadAssets[0].Replace(ModelData.OUTLINE_NAME,"");
					
//					if(GameManager.me.characterManager.monsterResource.ContainsKey(baseName))
//					{
//						SkinnedMeshRenderer[] smrs = GameManager.me.characterManager.monsterResource[baseName].gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
//						foreach(SkinnedMeshRenderer s in smrs)
//						{
//							if(s.name.Equals(baseName))
//							{
//								smr.material = s.material;
//								break;
//							}
//						}	
//					}
				}
				
				GameObject p = (GameObject)asset.assetBundle.Load("PartsInfo");
				
				if(p != null)
				{
					PartsInfomation pi =  p.GetComponent<PartsInfomation>();
					List<Transform> bones = new List<Transform>();
					
					foreach(string bName in pi.bones)
					{
						foreach(Transform tr in GetComponentsInChildren<Transform>())
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

				//GameManager.me.characterManager.saveAssetBundle(loadAssetsNames, asset.assetBundle);
				asset.assetBundle.Unload(false);	
				asset.Dispose();
				asset = null;	
			}
		}
		
		
		if(loadAssets.Length > 1) 
		{
			string nextAssets = loadAssetsNames.Substring(loadAssetsNames.IndexOf(",")+1);
			yield return StartCoroutine( loadAsset(nextAssets, callback) );
		}
		else
		{
//			Debug.Log("Load Complete!");
			//StartCoroutine(onCompleteLoadModel(callback));

			isReady = false;
//			yield return Resources.UnloadUnusedAssets();
			System.GC.Collect();
			cha.onCompleteLoadModel();		
			if(callback != null) callback(gameObject, cha);		
			isReady = true;
		}
	}
	
	/*	
	IEnumerator onCompleteLoadModel(CharacterManager.LoadBaseCharacterCompleteCallback callback)
	{
		isReady = false;
		//yield return null;
		//CombineAndAtlas.MergeAndAtlas(root);
		//CombineAndAtlas.MergeWithoutAtlas(root);
		//CombineAndAtlas.CombineOnly(root);
		//yield return null;
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
		cha.onCompleteLoadModel();		
		if(callback != null) callback(gameObject, cha);		
		isReady = true;
		yield return null;
	}
	*/












//// ==== 상단은 캐릭터를 쪼개서 불러올때 적용하는 코드고 아래는 현재 그냥 몬스터 하나당 한번씩 통채로 부를때 쓰는 코드다.

	public void initSingleModel(Monster baseCha, GameObject modelBase, CharacterManager.LoadBaseCharacterCompleteCallback callback = null)
	{
		isReady = false;

		StartCoroutine(initLoadedAsset(baseCha, modelBase, callback));
	}


	IEnumerator initLoadedAsset(Monster baseCha, GameObject model, CharacterManager.LoadBaseCharacterCompleteCallback callback)
	{
		yield return null;
		SkinnedMeshRenderer[] smrs = model.GetComponentsInChildren<SkinnedMeshRenderer>();

		foreach(SkinnedMeshRenderer smr in smrs)
		{
			smr.name = smr.name.Replace("(Clone)","");

			if(smr.gameObject.name.Contains("_line") == false)
			{
				if(smr.gameObject.name.Contains("_eye") || smr.gameObject.name.Contains("_mouth"))
				{
					TextureSpriteAnimation tsa = smr.gameObject.GetComponent<TextureSpriteAnimation>();
					if(tsa == null) tsa = smr.gameObject.AddComponent<TextureSpriteAnimation>();
				}
			}

			if(smr.name.Contains(ModelData.OUTLINE_RARE_NAME))
			{
//				smr.material = (Material)GameObject.Instantiate(ResourceManager.instance.rareMaterial);
				
				TextureAnimation ta = smr.gameObject.AddComponent<TextureAnimation>();
				ta.setMaterialType(false);
				ta.ySpeed = DebugManager.instance.rareAniSpeed;		
				ta.useLimit = true;
				ta.limit = 1.0f;
				ta.limtTypeIsMinus = false;
			}
			else if(smr.name.Contains(ModelData.OUTLINE_NAME))
			{
				string baseName = smr.name.Replace(ModelData.OUTLINE_NAME,"");
				
//				if(GameManager.me.characterManager.monsterResource.ContainsKey(baseName))
//				{
//					SkinnedMeshRenderer[] smrs2 = GameManager.me.characterManager.monsterResource[baseName].gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
//					foreach(SkinnedMeshRenderer s in smrs2)
//					{
//						if(s.name.Equals(baseName))
//						{
//							smr.material = s.material;
//							break;
//						}
//					}	
//				}
			}
		}
				
		isReady = false;
		baseCha.onCompleteLoadModel();		
		if(callback != null) callback(gameObject, baseCha);		
		isReady = true;
	}


}

using System;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
	public MapData ()
	{
	}
	
	
	public int id;
	public string resource;
	public float top;
	public float bottom;

	public string effect;




	public bool useFlare = false;
	
	public void setData(List<object> l, Dictionary<string, int> k)
	{
		Util.parseObject(l[k["ID"]], out id, true, 1);

		resource = (string)l[k["RESOURCE"]];
		resource = resource.ToLower();
		
		Util.parseObject(l[k["TOP"]], out top, true, 200.0f);
		Util.parseObject(l[k["BOTTOM"]], out bottom, true, -200.0f);

		effect = (string)l[k["EFFECT"]];


		if(k.ContainsKey("USE_FLARE"))
		{
			useFlare =  l[k["USE_FLARE"]].ToString().Equals("Y");
		}
	}
	
	public GameObject mapFile = null;
	
	public void loadMap(Transform parent)
	{
		if(mapFile == null)
		{
			if(GameManager.me.useAssetBundleMapFile)
			{
#if UNITY_EDITOR

				Debug.LogError("loadMap : " + resource);

				if(BattleSimulator.nowSimulation)
				{
					if( GameDataManager.instance.mapResource.ContainsKey( resource ) == false)
					{
						GameDataManager.instance.mapResource.Add( resource, new GameObject());
					}
				}
#endif

				mapFile = GameDataManager.instance.mapResource[resource];

			}
			else
			{
				mapFile = GameManager.resourceManager.getPrefabFromPool("Map/"+resource);
				//(GameObject)GameObject.Instantiate((GameObject)Resources.Load("prefabs/Map/"+resource));
			}
		}
		else
		{
			mapFile.transform.parent = parent;
			mapFile.transform.localPosition = new Vector3(0.0f, -5.0f, 0.0f);
			mapFile.transform.localRotation = new Quaternion(0,0,0,0);
			mapFile.SetActive(true);
		}
	}

	public void setVisible(bool isShow)
	{
		if(mapFile != null)
		{
			mapFile.SetActive(isShow);

			if(isShow)
			{
				GameManager.me.gameCamera.backgroundColor = Color.black;

			}
		}
	}


	public static string destroyExceptionResource = null;

	public void unloadAndDestroyMap()
	{
		if(mapFile != null)
		{
			if(GameManager.me.useAssetBundleMapFile == false)
			{
//				destroyComponents(mapFile);
				GameObject.DestroyImmediate(mapFile,true);
				GameManager.resourceManager.destroyPrefab("Map/"+resource);
			}
			else
			{
				if(destroyExceptionResource == null || resource != destroyExceptionResource)
				{
					GameObject.DestroyImmediate(mapFile,true);

					if(GameDataManager.instance.mapResource.ContainsKey(resource))
					{
						if(GameDataManager.instance.mapResource != null)
						{
							GameObject.DestroyImmediate(GameDataManager.instance.mapResource[resource], true);
							GameDataManager.instance.mapResource.Remove(resource);
						}
					}
				}
			}
		}

		mapFile = null;
	}


	public static void destroyComponents(GameObject go)
	{
		return;

		if(go == null) return;
		if(go.activeInHierarchy == false)
		{
			go.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
			go.SetActive(true);
		}

		MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>();

		if(mfs != null)
		{
			for(int i = mfs.Length - 1; i >= 0; --i)
			{
				GameObject.DestroyImmediate( mfs[i].sharedMesh, true );
			}
		}

		Renderer[] r = go.GetComponentsInChildren<Renderer>();

		if(r != null)
		{
			for(int i = r.Length - 1; i >= 0; --i)
			{
				GameObject.DestroyImmediate( r[i].sharedMaterial, true );
			}
		}
	}


}


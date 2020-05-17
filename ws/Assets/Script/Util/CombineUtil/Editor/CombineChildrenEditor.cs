using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

class CombineChildrenEditor 
{
	public static int frameToWait = 0;
	public static bool generateTriangleStrips = true, combineOnStart = true, destroyAfterOptimized = true, castShadow = false, receiveShadow = false, keepLayer = true, addMeshCollider = false;
	public static bool checkSubUVByMaterialName = true;
	
	static string[] checkMaterialName = new string[]{"hardlight","blends/"};
	
	public static string fileStartName = "";

	static bool checkHasSecondUV(string shaderName)
	{
		if(shaderName == null) return false;
		
		foreach(string str in checkMaterialName)
		{
			if(shaderName.Contains(str))
			{
				return true;
			}
		}
		
		return false;
	}


	[MenuItem ("Windsoul Bundle/Optimize Map") ]
	static void Execute()
	{
		foreach (Object o in Selection.GetFiltered(typeof (GameObject), SelectionMode.Unfiltered))
		{
			if(o is GameObject)
			{
				int i = -1;
				int.TryParse(o.name, out i);
				if(i > 0)
				{
					Merge((GameObject)o);
				}

				o.name = o.name + "_" + "optimized";
			}
		}
	}	


	static void Merge(GameObject target)
	{
		Component[] filters  = target.GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = target.transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();

		Dictionary<Material, MapTextureChangeData> mcd = new Dictionary<Material, MapTextureChangeData>();

		for (int i=0;i<filters.Length;i++) 
		{
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].renderer;
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance ();
			instance.mesh = filter.sharedMesh;

			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) 
			{
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;

				MapTextureChangeObject o = curRenderer.gameObject.GetComponent<MapTextureChangeObject>();

				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
					
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];

					if (objects != null) 
					{
						objects.Add(instance);

						if(o != null && mcd.ContainsKey(materials[m]) == false)
						{
							MapTextureChangeData d = new MapTextureChangeData();
							d.effectMat = o.effect;
							d.originalMat = o.original;
							d.type = o.type;

							mcd.Add(materials[m], d);
						}
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);

						if(o != null && mcd.ContainsKey(materials[m]) == false)
						{
							MapTextureChangeData d = new MapTextureChangeData();
							d.effectMat = o.effect;
							d.originalMat = o.original;
							d.type = o.type;
							
							mcd.Add(materials[m], d);
						}
					}
				}


				if (Application.isPlaying && destroyAfterOptimized && combineOnStart)
				{
					GameObject.Destroy(curRenderer.gameObject);
				}
				else if (destroyAfterOptimized) 
				{
					GameObject.DestroyImmediate(curRenderer.gameObject);
				}
				else
				{
					curRenderer.enabled = false;
				}
			}
		}

		int randomIndex = 0;

		foreach (DictionaryEntry de  in materialToMesh) 
		{
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));
			
			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
				// Make sure we have a mesh filter & renderer
				if (target.GetComponent(typeof(MeshFilter)) == null)
					target.gameObject.AddComponent(typeof(MeshFilter));
				if (!target.GetComponent("MeshRenderer"))
					target.gameObject.AddComponent("MeshRenderer");
				
				
				Material mat = (Material)de.Key;
				MeshFilter filter = (MeshFilter)target.GetComponent(typeof(MeshFilter));

				filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()));

				target.renderer.material = mat;
				target.renderer.enabled = true;
				if (addMeshCollider) target.gameObject.AddComponent<MeshCollider>();
				target.renderer.castShadows = false;
				target.renderer.receiveShadows = false;

				if(mcd.ContainsKey(mat))
				{
					MapTextureChangeObject mco = target.AddComponent<MapTextureChangeObject>();
					mco.effect = mcd[mat].effectMat;
					mco.original = mcd[mat].originalMat;
					mco.type = mcd[mat].type;
				}

			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{
				
				string n = "";
				
				n = de.Key.ToString();
				n = n.Substring(0, n.LastIndexOf("(")-1);
				
				GameObject go = new GameObject("Combined mesh " + n);
				if (keepLayer) go.layer = target.gameObject.layer;
				go.transform.parent = target.transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");
				
				Material mat = (Material)de.Key;

				if(mcd.ContainsKey(mat))
				{
					MapTextureChangeObject mco = go.AddComponent<MapTextureChangeObject>();
					mco.effect = mcd[mat].effectMat;
					mco.original = mcd[mat].originalMat;
					mco.type = mcd[mat].type;
				}

				go.renderer.material = mat;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));
				
				if(Application.isPlaying)filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, (checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()) == false)?false:true);
				else filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()));
				go.renderer.castShadows = false;
				go.renderer.receiveShadows = false;
				if (addMeshCollider) go.AddComponent<MeshCollider>();
				
				#if UNITY_EDITOR
				if(Application.isPlaying == false)
				{
					GameObject tg = target.gameObject;
					
					string name = tg.name;
					string rootName = "";
					
					while(tg.transform.parent != null)
					{
						name = tg.transform.parent.name + "_" + name + n;
						tg = tg.transform.parent.gameObject;
						rootName = tg.name;
					}
					
					if(Directory.Exists("Assets/00_CombineMeshes/"+rootName) == false)
					{
						Directory.CreateDirectory("Assets/00_CombineMeshes/"+rootName);
					}

					if(name.StartsWith(rootName))
					{
						name = name.Substring(rootName.Length);
						if(name.StartsWith("_Normal_"))
						{
							try
							{
								name = name.Substring(8);
							}
							catch
							{

							}
						}
					}

					UnityEditor.AssetDatabase.CreateAsset(filter.sharedMesh, "Assets/00_CombineMeshes/"+rootName+"/"+((fileStartName.Length > 0)?(fileStartName+"_"):"")+name+(++randomIndex)+".asset");
				}
				#endif
			}
		}	

	}
}


public class MapTextureChangeData
{
	public Material originalMat;
	public Material effectMat;

	public MapTextureChangeObject.Type type;
}
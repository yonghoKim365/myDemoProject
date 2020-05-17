using UnityEngine;
using System.Collections;


/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/
//[ExecuteInEditMode()]
using System.IO;


[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildrenNew : MonoBehaviour {
	
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
    public int frameToWait = 0;
	public bool generateTriangleStrips = true, combineOnStart = true, destroyAfterOptimized = false, castShadow = false, receiveShadow = false, keepLayer = true, addMeshCollider = false;
	public bool checkSubUVByMaterialName = true;

	string[] checkMaterialName = new string[]{"hardlight","blends/"};

	public string fileStartName = "";

    void Start()
    {
#if UNITY_EDITOR
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
		Debug.LogError("ERRORRR!!!!!");
#endif
		return;

        if (combineOnStart && frameToWait == 0) Combine();
        else StartCoroutine(CombineLate());
    }

    IEnumerator CombineLate()
    {
        for (int i = 0; i < frameToWait; i++ ) yield return 0;
        Combine();
    }

    [ContextMenu("Combine Now on Childs")]
    public void CallCombineOnAllChilds()
    {
		CombineChildrenNew[] c = gameObject.GetComponentsInChildren<CombineChildrenNew>();
        int count = c.Length;
        for (int i = 0; i < count; i++) if(c[i] != this)c[i].Combine();
        combineOnStart = enabled = false;
    }

	bool checkHasSecondUV(string shaderName)
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


	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    [ContextMenu ("Combine Now")]
	public void Combine () {
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh= new Hashtable();
		
		for (int i=0;i<filters.Length;i++) {
			MeshFilter filter = (MeshFilter)filters[i];
			Renderer curRenderer  = filters[i].renderer;
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance ();
			instance.mesh = filter.sharedMesh;
			if (curRenderer != null && curRenderer.enabled && instance.mesh != null) {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;
				for (int m=0;m<materials.Length;m++) {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) {
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList ();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);
					}
				}
                if (Application.isPlaying && destroyAfterOptimized && combineOnStart) Destroy(curRenderer.gameObject);
                else if (destroyAfterOptimized) DestroyImmediate(curRenderer.gameObject);
				else curRenderer.enabled = false;
			}
		}
	
		foreach (DictionaryEntry de  in materialToMesh) {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
				// Make sure we have a mesh filter & renderer
				if (GetComponent(typeof(MeshFilter)) == null)
					gameObject.AddComponent(typeof(MeshFilter));
				if (!GetComponent("MeshRenderer"))
					gameObject.AddComponent("MeshRenderer");
	

				Material mat = (Material)de.Key;
				MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));
				if (Application.isPlaying) filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, (checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()) == false)?false:true);
				else filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()));
				renderer.material = mat;
				renderer.enabled = true;
                if (addMeshCollider) gameObject.AddComponent<MeshCollider>();
                renderer.castShadows = castShadow;
                renderer.receiveShadows = receiveShadow;
			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{

				string n = "";
				
				n = de.Key.ToString();
				n = n.Substring(0, n.LastIndexOf("(")-1);

				GameObject go = new GameObject("Combined mesh " + n);
                if (keepLayer) go.layer = gameObject.layer;
				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent("MeshRenderer");

				Material mat = (Material)de.Key;

				go.renderer.material = mat;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));

				if(Application.isPlaying)filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, (checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()) == false)?false:true);
				else filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips, checkSubUVByMaterialName && checkHasSecondUV(mat.shader.name.ToLower()));
                go.renderer.castShadows = castShadow;
                go.renderer.receiveShadows = receiveShadow;
                if (addMeshCollider) go.AddComponent<MeshCollider>();

#if UNITY_EDITOR
				if(Application.isPlaying == false)
				{
					GameObject tg = gameObject;

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

					UnityEditor.AssetDatabase.CreateAsset(filter.sharedMesh, "Assets/00_CombineMeshes/"+rootName+"/"+((fileStartName.Length > 0)?(fileStartName+"_"):"")+name+".asset");
				}
#endif
            }
		}	
	}	
}
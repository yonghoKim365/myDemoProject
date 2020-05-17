using UnityEngine;

using System.Collections;

using System.IO;
using System.Collections.Generic;
using UnityEditor;

 
[ExecuteInEditMode]
public class CombineAndAtlas : MonoBehaviour {


	void Start()
	{
		CombineMaterialAndAtlas(gameObject);
		MergeWithoutAtlas(gameObject);
	}

	public static void CombineMaterialAndAtlas(GameObject root, int maxAtlasSize = 2048, bool useAlpha = false)
	{
		int vertCount = 0;
		int normCount = 0;
		int tanCount = 0;
		int triCount = 0;
		int uvCount = 0;
		int boneCount = 0;
		int bpCount = 0;
		int bwCount = 0;
		
		Transform[] bones;
		Matrix4x4[] bindPoses;
		BoneWeight[] weights;
		
		Vector3[] verts;
		Vector3[] norms;
		Vector4[] tans;
		
		int[] tris;
		Vector2[] uvs;
		Texture2D[] textures;

		int vertOffset = 0;
		int normOffset = 0;
		int tanOffset = 0;
		int triOffset = 0;
		int uvOffset = 0;
		int meshOffset = 0;		
		int  boneSplit = 0;
		int bNum = 0;
		int[] bCount;

		SkinnedMeshRenderer[] SMRs  = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			uvCount += smr.sharedMesh.uv.Length;
			bNum++;
		}
		
		textures = new Texture2D[bNum];

		uvs = new Vector2[uvCount];


		Shader s = Shader.Find("");
		if(useAlpha)
		{
			s = Shader.Find("Unlit/Transparent Colored");
		}
		else
		{
			s = Shader.Find("Unlit/Texture Color");
		}

		// 텍스쳐 사이즈를 줄이는 일을 해본다...
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			Texture2D sourceTx = (Texture2D) smr.sharedMaterial.GetTexture("_MainTex");

			if(smr.sharedMaterial.shader != null)
			{
				s = smr.sharedMaterial.shader;
			}

			string sourceTexture = UnityEditor.AssetDatabase.GetAssetPath( sourceTx );

			string newFileName = Path.GetFileNameWithoutExtension(sourceTexture);

			string newPath = sourceTexture.Substring(0,sourceTexture.LastIndexOf("/")) + "/lowpc/" + newFileName + Path.GetExtension(sourceTexture);

			Texture2D newTx = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));

			if(newTx == null)
			{
				Debug.LogError( "경로에 텍스쳐가 없습니다. : " + newPath  );
			}

			Material m = new Material( s );
			m.SetTexture("_MainTex",newTx);

			smr.sharedMaterial = m;
		}


		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			foreach (Vector2 uv in smr.sharedMesh.uv) 
			{
				uvs[uvOffset++] = uv;
			}
			
			textures[meshOffset] = (Texture2D) smr.sharedMaterial.GetTexture("_MainTex");

			meshOffset++;
		}

		Texture2D tx = new Texture2D (1,1,TextureFormat.RGBA32,false);

		tx.name = root.name;

		string savePackFileName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + tx.name + "_pack_atlas.png";

		Rect[] r = tx.PackTextures (textures, 0, maxAtlasSize);
		
		//tx.Compress(true);
		tx.Apply();

		byte[] bytes = tx.EncodeToPNG();
		FileStream file = File.Open(savePackFileName,FileMode.Create);
		var binary= new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Texture2D loadTx = (Texture2D)AssetDatabase.LoadAssetAtPath(savePackFileName, typeof(Texture2D));

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Material mat = new Material( s );

		string matName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + root.name + "lowMat.asset";
		AssetDatabase.CreateAsset(mat, matName);

		mat.SetTexture("_MainTex", loadTx);

		uvOffset = 0;
		meshOffset = 0;

		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			Vector2[] newUv = new Vector2[smr.sharedMesh.uv.Length];
			int index = 0;

			foreach (Vector2 uv in smr.sharedMesh.uv)
			{
				uvs[uvOffset].x = Mathf.Lerp (r[meshOffset].xMin, r[meshOffset].xMax, uv.x);//uv.x % 1);
				uvs[uvOffset].y = Mathf.Lerp (r[meshOffset].yMin, r[meshOffset].yMax, uv.y);//uv.y % 1);
				newUv[index] = uvs[uvOffset];
				index++;
				uvOffset++;
			}

			Mesh me = new Mesh();

			string meshName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + root.name + "_" + smr.sharedMesh.name + "_lowMesh.asset";
			AssetDatabase.CreateAsset(me, meshName);

			me.name = smr.sharedMesh.name;
			me.vertices = smr.sharedMesh.vertices;
			me.normals = smr.sharedMesh.normals;
			me.tangents = smr.sharedMesh.tangents;
			me.boneWeights = smr.sharedMesh.boneWeights;
			me.triangles = smr.sharedMesh.triangles;
			me.bindposes = smr.sharedMesh.bindposes;
			me.uv = newUv;

			smr.sharedMesh = me;
			smr.sharedMaterial = mat;
			meshOffset++;
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}



	public static void CombineMultipleMaterialAndAtlas(GameObject root, int maxAtlasSize = 2048)
	{
		int vertCount = 0;
		int normCount = 0;
		int tanCount = 0;
		int triCount = 0;
		int uvCount = 0;
		int boneCount = 0;
		int bpCount = 0;
		int bwCount = 0;
		
		Transform[] bones;
		Matrix4x4[] bindPoses;
		BoneWeight[] weights;
		
		Vector3[] verts;
		Vector3[] norms;
		Vector4[] tans;
		
		int[] tris;
		Vector2[] uvs;
		Texture2D[] textures;
		
		int vertOffset = 0;
		int normOffset = 0;
		int tanOffset = 0;
		int triOffset = 0;
		int uvOffset = 0;
		int meshOffset = 0;		
		int  boneSplit = 0;
		int bNum = 0;
		int[] bCount;
		
		SkinnedMeshRenderer[] SMRs  = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
		
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			uvCount += smr.sharedMesh.uv.Length;
			bNum++;
		}
		
		textures = new Texture2D[bNum];
		
		uvs = new Vector2[uvCount];
		
		
		Shader s = null;

		List<Material> materials = new List<Material>();

		// 텍스쳐 사이즈를 줄이는 일을 해본다...
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			foreach(Material sm in smr.sharedMaterials)
			{
				Texture2D sourceTx = (Texture2D) sm.GetTexture("_MainTex");
				
				if(sm.shader != null)
				{
					s = sm.shader;
				}
				
				string sourceTexture = UnityEditor.AssetDatabase.GetAssetPath( sourceTx );
				
				string newFileName = Path.GetFileNameWithoutExtension(sourceTexture);
				
				string newPath = sourceTexture.Substring(0,sourceTexture.LastIndexOf("/")) + "/lowpc/" + newFileName + Path.GetExtension(sourceTexture);
				
				Texture2D newTx = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath(newPath, typeof(Texture2D));
				
				if(newTx == null)
				{
					Debug.LogError( "경로에 텍스쳐가 없습니다. : " + newPath  );
				}
				
				Material m = new Material( s );
				m.SetTexture("_MainTex",newTx);

				materials.Add(m);
			}

			smr.sharedMaterials = materials.ToArray();
			materials.Clear();
		}
		
		
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			foreach (Vector2 uv in smr.sharedMesh.uv) 
			{
				uvs[uvOffset++] = uv;
			}
			
			textures[meshOffset] = (Texture2D) smr.sharedMaterial.GetTexture("_MainTex");
			
			meshOffset++;
		}
		
		Texture2D tx = new Texture2D (1,1,TextureFormat.RGBA32,false);
		
		tx.name = root.name;
		
		string savePackFileName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + tx.name + "_pack_atlas.png";
		
		Rect[] r = tx.PackTextures (textures, 0, maxAtlasSize);
		
		//tx.Compress(true);
		tx.Apply();
		
		byte[] bytes = tx.EncodeToPNG();
		FileStream file = File.Open(savePackFileName,FileMode.Create);
		var binary= new BinaryWriter(file);
		binary.Write(bytes);
		file.Close();
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Texture2D loadTx = (Texture2D)AssetDatabase.LoadAssetAtPath(savePackFileName, typeof(Texture2D));
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		Material mat = new Material( s );
		
		string matName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + root.name + "lowMat.asset";
		AssetDatabase.CreateAsset(mat, matName);
		
		mat.SetTexture("_MainTex", loadTx);
		
		uvOffset = 0;
		meshOffset = 0;
		
		foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			Vector2[] newUv = new Vector2[smr.sharedMesh.uv.Length];
			int index = 0;
			
			foreach (Vector2 uv in smr.sharedMesh.uv)
			{
				uvs[uvOffset].x = Mathf.Lerp (r[meshOffset].xMin, r[meshOffset].xMax, uv.x);//uv.x % 1);
				uvs[uvOffset].y = Mathf.Lerp (r[meshOffset].yMin, r[meshOffset].yMax, uv.y);//uv.y % 1);
				newUv[index] = uvs[uvOffset];
				index++;
				uvOffset++;
			}
			
			Mesh me = new Mesh();
			
			string meshName = CreateCharacterAssetBundleBase.bundleDummyPrefabPath + smr.sharedMesh.name + "_lowMesh.asset";
			AssetDatabase.CreateAsset(me, meshName);
			
			me.name = smr.sharedMesh.name;
			me.vertices = smr.sharedMesh.vertices;
			me.normals = smr.sharedMesh.normals;
			me.tangents = smr.sharedMesh.tangents;
			me.boneWeights = smr.sharedMesh.boneWeights;
			me.triangles = smr.sharedMesh.triangles;
			me.bindposes = smr.sharedMesh.bindposes;
			me.uv = newUv;
			
			smr.sharedMesh = me;
			smr.sharedMaterial = mat;
			meshOffset++;
		}
		
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}



    
    public static void MergeAndAtlas(GameObject root, bool hasNormalMaps = false, int maxAtlasSize = 2048)
	{

        SkinnedMeshRenderer[] SMRs;

        int vertCount = 0;
        int normCount = 0;
        int tanCount = 0;
        int triCount = 0;
        int uvCount = 0;
        int boneCount = 0;
        int bpCount = 0;
        int bwCount = 0;

        Transform[] bones;
        Matrix4x4[] bindPoses;
        BoneWeight[] weights;

        Vector3[] verts;
        Vector3[] norms;
        Vector4[] tans;

        int[] tris;
        Vector2[] uvs;
        Texture2D[] textures;
        Texture2D[] normalmaps;

        int vertOffset = 0;
        int normOffset = 0;
        int tanOffset = 0;
        int triOffset = 0;
        int uvOffset = 0;
        int meshOffset = 0;		
        int  boneSplit = 0;
        int bNum = 0;
        int[] bCount;

        SMRs = root.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            vertCount += smr.sharedMesh.vertices.Length;
            normCount += smr.sharedMesh.normals.Length;
            tanCount += smr.sharedMesh.tangents.Length;
            triCount += smr.sharedMesh.triangles.Length;
            uvCount += smr.sharedMesh.uv.Length;
            boneCount += smr.bones.Length;
            bpCount += smr.sharedMesh.bindposes.Length;
            bwCount += smr.sharedMesh.boneWeights.Length;
            bNum++;
        }

        bCount = new int[3];
        bones = new Transform[boneCount];
        weights = new BoneWeight[bwCount];
        bindPoses = new Matrix4x4[bpCount];
        textures = new Texture2D[bNum];
        normalmaps = new Texture2D[bNum];

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            for(int b1 = 0; b1 < smr.bones.Length; b1++) 
			{
                bones[bCount[0]] = smr.bones[b1];
                bCount[0]++;
            }

            for(int b2 = 0; b2 < smr.sharedMesh.boneWeights.Length; b2++) 
			{
                weights[bCount[1]] = smr.sharedMesh.boneWeights[b2];
                weights[bCount[1]].boneIndex0 += boneSplit;
                weights[bCount[1]].boneIndex1 += boneSplit;
                weights[bCount[1]].boneIndex2 += boneSplit;
                weights[bCount[1]].boneIndex3 += boneSplit;

                bCount[1]++;
            }

            for(int b3 = 0; b3 < smr.sharedMesh.bindposes.Length; b3++) 
			{
                bindPoses[bCount[2]] = smr.sharedMesh.bindposes[b3];
                bCount[2]++;
            }
            boneSplit += smr.bones.Length;
        }

        verts = new Vector3[vertCount];
        norms = new Vector3[normCount];
        tans = new Vector4[tanCount];
        tris = new int[triCount];
        uvs = new Vector2[uvCount];

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            foreach (int i in smr.sharedMesh.triangles) {
                tris[triOffset++] = i + vertOffset;
            }

            foreach (Vector3 v in smr.sharedMesh.vertices) {
                verts[vertOffset++] = v;
            }

            foreach (Vector3 n in smr.sharedMesh.normals) {
                norms[normOffset++] = n;
            }

            foreach (Vector4 t in smr.sharedMesh.tangents) {
                tans[tanOffset++] = t;
            }

            foreach (Vector2 uv in smr.sharedMesh.uv) {
                uvs[uvOffset++] = uv;
            }

            textures[meshOffset] = (Texture2D) smr.sharedMaterial.GetTexture("_MainTex");

            if( hasNormalMaps ) normalmaps[meshOffset] = (Texture2D) smr.sharedMaterial.GetTexture("_BumpMap");

            meshOffset++;

            Destroy( smr.gameObject );
        }

		Texture2D tx = new Texture2D (1,1,TextureFormat.ETC_RGB4,false);
		
		tx.name = "MainTextureAtlas";
		
        Rect[] r = tx.PackTextures (textures, 0, maxAtlasSize);
		
		tx.Compress(true);
		
		tx.Apply();
		
		Debug.LogError(tx.format);
		
		for(int i = 0; i < textures.Length; ++i)
		{
			DestroyImmediate(textures[i], true);
			textures[i] = null;
		}
		textures = null;
		
        Texture2D nm = new Texture2D (1,1);

        if( hasNormalMaps ) 
		{
            nm.PackTextures (normalmaps, 0, maxAtlasSize);
        }

        uvOffset = 0;
        meshOffset = 0;
		
		Shader shader = null;
		
        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			shader = smr.sharedMaterial.shader;
			
            foreach (Vector2 uv in smr.sharedMesh.uv) {
                uvs[uvOffset].x = Mathf.Lerp (r[meshOffset].xMin, r[meshOffset].xMax, uv.x % 1);
                uvs[uvOffset].y = Mathf.Lerp (r[meshOffset].yMin, r[meshOffset].yMax, uv.y % 1);
                uvOffset ++;
            }
            meshOffset ++;
        }

        Material mat;
		
		if(shader != null) mat = new Material(shader);
        else if( hasNormalMaps ) mat = new Material( Shader.Find("Bumped Diffuse") );
        else mat = new Material( Shader.Find("Unlit/Texture") );

        
        mat.SetTexture("_MainTex", tx);

        if( hasNormalMaps ) mat.SetTexture("_BumpMap", nm);

        //New Mesh

        Mesh me = new Mesh();
        me.name = root.gameObject.name;
        me.vertices = verts;
        me.normals = norms;
        me.tangents = tans;
        me.boneWeights = weights;
        me.uv = uvs;
        me.triangles = tris;
        me.bindposes = bindPoses;

        SkinnedMeshRenderer newSMR = root.AddComponent<SkinnedMeshRenderer>();

        newSMR.sharedMesh = me;
        newSMR.bones = bones;
        newSMR.updateWhenOffscreen = true;

        root.renderer.material = mat;

        // reset animator culling mode so that Mecanim recognizes renderer bounds again

        Animator animator = root.GetComponent<Animator>();

        if( animator ) 
		{
            animator.cullingMode = AnimatorCullingMode.BasedOnRenderers;
        }
    }
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	// Use Only 1 Texture.
    public static void MergeWithoutAtlas(GameObject root, bool deleteOldSmr = false)
	{
        SkinnedMeshRenderer[] SMRs;

        int vertCount = 0;
        int normCount = 0;
        int tanCount = 0;
        int triCount = 0;
        int uvCount = 0;
        int boneCount = 0;
        int bpCount = 0;
        int bwCount = 0;

        Transform[] bones;
        Matrix4x4[] bindPoses;
        BoneWeight[] weights;

        Vector3[] verts;
        Vector3[] norms;
        Vector4[] tans;

        int[] tris;
        Vector2[] uvs;
        Texture2D[] textures;
        Texture2D[] normalmaps;

        int vertOffset = 0;
        int normOffset = 0;
        int tanOffset = 0;
        int triOffset = 0;
        int uvOffset = 0;
        int meshOffset = 0;		
        int  boneSplit = 0;
        int bNum = 0;
        int[] bCount;

        SMRs = root.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            vertCount += smr.sharedMesh.vertices.Length;
            normCount += smr.sharedMesh.normals.Length;
            tanCount += smr.sharedMesh.tangents.Length;
            triCount += smr.sharedMesh.triangles.Length;
            uvCount += smr.sharedMesh.uv.Length;
            boneCount += smr.bones.Length;
            bpCount += smr.sharedMesh.bindposes.Length;
            bwCount += smr.sharedMesh.boneWeights.Length;
            bNum++;
        }

        bCount = new int[3];
        bones = new Transform[boneCount];
        weights = new BoneWeight[bwCount];
        bindPoses = new Matrix4x4[bpCount];
        textures = new Texture2D[bNum];
        normalmaps = new Texture2D[bNum];

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            for(int b1 = 0; b1 < smr.bones.Length; b1++) 
			{
                bones[bCount[0]] = smr.bones[b1];
                bCount[0]++;
            }

            for(int b2 = 0; b2 < smr.sharedMesh.boneWeights.Length; b2++) 
			{
                weights[bCount[1]] = smr.sharedMesh.boneWeights[b2];
                weights[bCount[1]].boneIndex0 += boneSplit;
                weights[bCount[1]].boneIndex1 += boneSplit;
                weights[bCount[1]].boneIndex2 += boneSplit;
                weights[bCount[1]].boneIndex3 += boneSplit;

                bCount[1]++;
            }

            for(int b3 = 0; b3 < smr.sharedMesh.bindposes.Length; b3++) 
			{
                bindPoses[bCount[2]] = smr.sharedMesh.bindposes[b3];
                bCount[2]++;
            }
            boneSplit += smr.bones.Length;
        }

        verts = new Vector3[vertCount];
        norms = new Vector3[normCount];
        tans = new Vector4[tanCount];
        tris = new int[triCount];
        uvs = new Vector2[uvCount];

        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
            foreach (int i in smr.sharedMesh.triangles) {
                tris[triOffset++] = i + vertOffset;
            }

            foreach (Vector3 v in smr.sharedMesh.vertices) {
                verts[vertOffset++] = v;
            }

            foreach (Vector3 n in smr.sharedMesh.normals) {
                norms[normOffset++] = n;
            }

            foreach (Vector4 t in smr.sharedMesh.tangents) {
                tans[tanOffset++] = t;
            }

            foreach (Vector2 uv in smr.sharedMesh.uv) {
                uvs[uvOffset++] = uv;
            }

            textures[meshOffset] = (Texture2D) smr.sharedMaterial.GetTexture("_MainTex");

            meshOffset++;

            Destroy( smr.gameObject );
        }
		

        uvOffset = 0;
        meshOffset = 0;
		
		Shader shader = null;
        foreach (SkinnedMeshRenderer smr in SMRs) 
		{
			shader = smr.sharedMaterial.shader;
            foreach (Vector2 uv in smr.sharedMesh.uv) {
                uvs[uvOffset].x = uv.x;
                uvs[uvOffset].y = uv.y;
                uvOffset ++;
            }
            meshOffset ++;
        }
		
		Material mat;
		if(shader != null) mat = new Material(shader);		
		else mat = new Material( Shader.Find("Unlit/Texture") );
        
        mat.SetTexture("_MainTex", textures[0]);

		SkinnedMeshRenderer newSMR = root.GetComponent<SkinnedMeshRenderer>();
		if(newSMR == null) newSMR = root.AddComponent<SkinnedMeshRenderer>();

        //New Mesh
		Mesh me = newSMR.sharedMesh;
		if(me == null) me = new Mesh();
		else me.Clear();

        me.name = root.gameObject.name;
        me.vertices = verts;
        me.normals = norms;
        me.tangents = tans;
        me.boneWeights = weights;
        me.uv = uvs;
        me.triangles = tris;
        me.bindposes = bindPoses;

        newSMR.sharedMesh = me;
        newSMR.bones = bones;
        newSMR.updateWhenOffscreen = false;

        root.renderer.material = mat;

        Animator animator = root.GetComponent<Animator>();

        if( animator ) 
		{
			GameObject.Destroy(animator);
        }

		if(deleteOldSmr)
		{
			for(int i = SMRs.Length - 1; i >= 0; --i)
			{
				GameObject.Destroy(SMRs[i].gameObject);
			}
		}

    }	
	
	
	
	
	
	
	
    public static void CombineOnly(GameObject root){

        SkinnedMeshRenderer[] SMRs;

        int vertCount = 0;
        int normCount = 0;
        int tanCount = 0;
        int triCount = 0;
        int uvCount = 0;
        int boneCount = 0;
        int bpCount = 0;
        int bwCount = 0;

        Transform[] bones;
        Matrix4x4[] bindPoses;
        BoneWeight[] weights;

        Vector3[] verts;
        Vector3[] norms;
        Vector4[] tans;
        Vector2[] uvs;

//      Texture2D[] textures;

        List<int[]> subMeshes;

        Material[] mats;

 

        int vertOffset = 0;

        int normOffset = 0;

        int tanOffset = 0;

        int triOffset = 0;

        int uvOffset = 0;

        int meshOffset = 0;

 

        int boneSplit = 0;

        int bNum = 0;

 

        int[] bCount;

 

        SMRs = root.GetComponentsInChildren<SkinnedMeshRenderer>();

 

        foreach (SkinnedMeshRenderer smr in SMRs) {

            vertCount += smr.sharedMesh.vertices.Length;

            normCount += smr.sharedMesh.normals.Length;

            tanCount += smr.sharedMesh.tangents.Length;

            triCount += smr.sharedMesh.triangles.Length;

            uvCount += smr.sharedMesh.uv.Length;

            boneCount += smr.bones.Length;

            bpCount += smr.sharedMesh.bindposes.Length;

            bwCount += smr.sharedMesh.boneWeights.Length;

            bNum++;

        }

 

        bCount = new int[3];

        

        bones = new Transform[boneCount];

        weights = new BoneWeight[bwCount];

        bindPoses = new Matrix4x4[bpCount];

//      textures = new Texture2D[bNum];

        mats = new Material[bNum];

 

        foreach (SkinnedMeshRenderer smr in SMRs) {

            

            // Load bone transforms

            for(int b1 = 0; b1 < smr.bones.Length; b1++) {

                

                bones[bCount[0]] = smr.bones[b1];

                bCount[0]++;

            }

 

            // Load bone weights

            for(int b2 = 0; b2 < smr.sharedMesh.boneWeights.Length; b2++){

 

                weights[bCount[1]] = smr.sharedMesh.boneWeights[b2];

                weights[bCount[1]].boneIndex0 += boneSplit;

                weights[bCount[1]].boneIndex1 += boneSplit;

                weights[bCount[1]].boneIndex2 += boneSplit;

                weights[bCount[1]].boneIndex3 += boneSplit;

 

                bCount[1]++;

            }

 

            // Load bone bindposes

            for(int b3 = 0; b3 < smr.sharedMesh.bindposes.Length; b3++){

 

                bindPoses[bCount[2]] = smr.sharedMesh.bindposes[b3];

                bCount[2]++;

            }

 

            boneSplit += smr.bones.Length;

        }

 

        verts = new Vector3[vertCount];

        norms = new Vector3[normCount];

        tans = new Vector4[tanCount];

        subMeshes = new List<int[]>();

        uvs = new Vector2[uvCount];

 

        foreach (SkinnedMeshRenderer smr in SMRs){

            

            int[] theseTris = new int[smr.sharedMesh.triangles.Length];

            foreach (int i in smr.sharedMesh.triangles){

                theseTris[triOffset++] = i + vertOffset;

            }

            

            subMeshes.Add(theseTris);

            triOffset = 0;

 

            foreach (Vector3 v in smr.sharedMesh.vertices){

                verts[vertOffset++] = v;

            }

 

            foreach (Vector3 n in smr.sharedMesh.normals){

                norms[normOffset++] = n;

            }

 

            foreach (Vector4 t in smr.sharedMesh.tangents){

                tans[tanOffset++] = t;

            }

 

            foreach (Vector2 uv in smr.sharedMesh.uv){

                uvs[uvOffset++] = uv;

            }

            

            mats[meshOffset] = smr.sharedMaterial;

            

 

//            textures[meshOffset] = (Texture2D) smr.sharedMaterial.mainTexture;

//            string path = AssetDatabase.GetAssetPath (smr.sharedMaterial.mainTexture);

//            TextureImporter imp = (TextureImporter) AssetImporter.GetAtPath (path);

//

//            if (!imp.isReadable) {

//                imp.isReadable = false;

//                AssetDatabase.Refresh ();

//                AssetDatabase.ImportAsset (path);

//            }

            

            meshOffset++;

 

            smr.enabled = false;

        }

 

 

 

//        Texture2D tx = new Texture2D (1,1);

//        Rect[] r = tx.PackTextures (textures, 0, maxAtlasSize);

//        File.WriteAllBytes (Application.dataPath + "/" + Selection.activeGameObject.name + ".png", tx.EncodeToPNG());

//        AssetDatabase.Refresh ();

//        tx = (Texture2D) AssetDatabase.LoadAssetAtPath ("Assets/" + Selection.activeGameObject.name + ".png", typeof(Texture2D)); 

 

//        uvOffset = 0;

//        meshOffset = 0;

//        foreach (SkinnedMeshRenderer smr in SMRs) {

//

//            foreach (Vector2 uv in smr.sharedMesh.uv) {

//              

//                uvs[uvOffset].x = Mathf.Lerp (r[meshOffset].xMin, r[meshOffset].xMax, uv.x);

//                uvs[uvOffset].y = Mathf.Lerp (r[meshOffset].yMin, r[meshOffset].yMax, uv.y);

//

//                uvOffset ++;

//            }

//

//            meshOffset ++;

//        }

 

 

 

//        Material mat = new Material (Shader.Find("Diffuse"));

//        mat.mainTexture = tx;

//

//        AssetDatabase.CreateAsset(mat, "Assets/" + Selection.activeGameObject.name + ".mat");

 

 

 

        //New Mesh

 

        Mesh me = new Mesh();

        me.name = root.name;

        me.vertices = verts;

        me.normals = norms;

        me.tangents = tans;

        me.boneWeights = weights;

        me.uv = uvs;

        me.subMeshCount = subMeshes.Count;

        

        for( int subMesh = 0; subMesh < subMeshes.Count; subMesh++ ) {

            me.SetTriangles( subMeshes[subMesh], subMesh );

        }

 
        me.bindposes = bindPoses;

 

        SkinnedMeshRenderer newSMR = root.AddComponent<SkinnedMeshRenderer>();

 
        newSMR.sharedMesh = me;
        newSMR.bones = bones;
        newSMR.updateWhenOffscreen = true;

        root.renderer.sharedMaterials = mats;

    }
	
	
}
using UnityEngine;

using UnityEditor;

using System.Collections;

using System.Collections.Generic;

using System.IO;

/*
Here is another adaptation of duhprey's adaptation. 
This version of the script doesn't do any atlasing; 
it just puts all original materials under the same combined renderer, 
which is useful if you want to be able to swap the texture of, 
say, a shirt or skin color without changing the rest. 
The atlasing code is just commented in case someone wants to add it back in.
*/ 

public class EditorCombine : MonoBehaviour {

 

//    static public int maxAtlasSize = 2048;

 

    [MenuItem ("Assets/CombineSkinnedMeshes")]

 

    static void CombineTheMeshes(){

 

        if (Selection.activeGameObject == null) return;

 

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

 

        SMRs = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

 

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

//                imp.isReadable = true;

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

        me.name = Selection.activeGameObject.name;

        me.vertices = verts;

        me.normals = norms;

        me.tangents = tans;

        me.boneWeights = weights;

        me.uv = uvs;

        me.subMeshCount = subMeshes.Count;

        

        for( int subMesh = 0; subMesh < subMeshes.Count; subMesh++ ) {

            me.SetTriangles( subMeshes[subMesh], subMesh );

        }

 

        AssetDatabase.CreateAsset(me, "Assets/" + Selection.activeGameObject.name + "mesh.asset");

 

        me.bindposes = bindPoses;

 

        SkinnedMeshRenderer newSMR = Selection.activeGameObject.AddComponent<SkinnedMeshRenderer>();

 

        newSMR.sharedMesh = me;

        newSMR.bones = bones;

        newSMR.updateWhenOffscreen = true;

        

        Selection.activeGameObject.renderer.sharedMaterials = mats;

 

//        Selection.activeGameObject.renderer.material = mat;

    }

 

}
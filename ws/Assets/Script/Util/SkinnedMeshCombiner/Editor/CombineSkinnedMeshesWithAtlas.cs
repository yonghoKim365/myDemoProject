using UnityEngine;

using UnityEditor;

using System.Collections;

using System.IO;

 

public class CombineSkinnedMeshesWithAtlas : MonoBehaviour {

 

    public int maxAtlasSize = 2048; 

    private bool initFlag;

    

    void Start () {

        initFlag = true;    

    }

    

    void Update () {

        if (initFlag) {

            CombineTheMeshes(gameObject);

            initFlag = false;

        }

        else animation.CrossFade("Run");

    }

    

    void CombineTheMeshes(GameObject source){

        if (source == null) return;

 

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

 

        int vertOffset = 0;

        int normOffset = 0;

        int tanOffset = 0;

        int triOffset = 0;

        int uvOffset = 0;

        int meshOffset = 0;

 

        int  boneSplit = 0;

        int bNum = 0;

 

        int[] bCount;

 

        SMRs = source.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer smr in SMRs){

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

 

        foreach (SkinnedMeshRenderer smr in SMRs){

            for(int b1 = 0; b1 < smr.bones.Length; b1++){

                bones[bCount[0]] = smr.bones[b1];

                bCount[0]++;

            }

            for(int b2 = 0; b2 < smr.sharedMesh.boneWeights.Length; b2++){

                weights[bCount[1]] = smr.sharedMesh.boneWeights[b2];

                weights[bCount[1]].boneIndex0 += boneSplit;

                weights[bCount[1]].boneIndex1 += boneSplit;

                weights[bCount[1]].boneIndex2 += boneSplit;

                weights[bCount[1]].boneIndex3 += boneSplit;

                bCount[1]++;

            }

            for(int b3 = 0; b3 < smr.sharedMesh.bindposes.Length; b3++){

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

 

        foreach (SkinnedMeshRenderer smr in SMRs){

            foreach (int i in smr.sharedMesh.triangles){

                tris[triOffset++] = i + vertOffset;

            }

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

            textures[meshOffset] = (Texture2D) smr.sharedMaterial.mainTexture;

            string path = AssetDatabase.GetAssetPath (smr.sharedMaterial.mainTexture);

            TextureImporter imp = (TextureImporter) AssetImporter.GetAtPath (path);

            if (!imp.isReadable) {

                imp.isReadable = true;

                AssetDatabase.Refresh ();

                AssetDatabase.ImportAsset (path);

            }

            meshOffset++;

            smr.enabled = false;

        }

 

        Texture2D tx = new Texture2D (1,1);

        Rect[] r = tx.PackTextures (textures, 0, maxAtlasSize);

        File.WriteAllBytes (Application.dataPath + "/" + source.name + ".png", tx.EncodeToPNG());

        AssetDatabase.Refresh ();

        tx = (Texture2D) AssetDatabase.LoadAssetAtPath ("Assets/" + source.name + ".png", typeof(Texture2D)); 

 

        uvOffset = 0;

        meshOffset = 0;

        foreach (SkinnedMeshRenderer smr in SMRs) {

            foreach (Vector2 uv in smr.sharedMesh.uv) {

                Vector2 uvClamped = new Vector2();

                

                uvClamped = uv;

                

                while (uvClamped.x > 1)

                    uvClamped.x = uvClamped.x - 1;

                

                while (uvClamped.x < 0)

                    uvClamped.x = uvClamped.x + 1;

                

                while (uvClamped.y > 1)

                    uvClamped.y = uvClamped.y - 1;

                

                while (uvClamped.x < 0)

                    uvClamped.y = uvClamped.y + 1;

                

                uvs[uvOffset].x = Mathf.Lerp (r[meshOffset].xMin, r[meshOffset].xMax, uvClamped.x);             

                uvs[uvOffset].y = Mathf.Lerp (r[meshOffset].yMin, r[meshOffset].yMax, uvClamped.y);             

                uvOffset ++;

            }

            meshOffset ++;

        }

 

        Material mat = new Material (Shader.Find("Diffuse"));

        mat.mainTexture = tx;

        AssetDatabase.CreateAsset(mat, "Assets/" + source.name + ".mat");

 

        //New Mesh

        Mesh me = new Mesh();

        me.name = source.name;

        me.vertices = verts;

        me.normals = norms;

        me.tangents = tans;

        me.boneWeights = weights;

        me.uv = uvs;

        me.triangles = tris;

        AssetDatabase.CreateAsset(me, "Assets/" + source.name + "mesh.asset");

        me.bindposes = bindPoses;

        SkinnedMeshRenderer newSMR = source.AddComponent<SkinnedMeshRenderer>();

 

        newSMR.sharedMesh = me;

        newSMR.bones = bones;

        newSMR.updateWhenOffscreen = true;

        source.renderer.material = mat;

    }

}
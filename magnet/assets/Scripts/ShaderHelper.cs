using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ShaderHelper
{

    /// <summary> 메테리얼 인스턴트화.</summary>
    public static void InitMapPostProcess(int iLayer, string newShaderName, string propertyName)
    {
        //Obstacle처리
        GameObject[] AllObject = Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.hideFlags == HideFlags.None).ToArray();

        string TransCutOut = "Transparent/Cutout/Diffuse"; //<-Transparent/Cutout/Diffuse  제외시켜야할 쉐이더 
        string TransDiffuse = "Transparent/Diffuse"; //<-Transparent/Diffuse  제외시켜야할 쉐이더 
        string Foliage = "Custom/Foliage"; //<-제외시켜야할 쉐이더 
		string DiffuseShake = "Transparent/Diffuse/Shake"; //<-제외시켜야할 쉐이더 


        //string newShaderName = "Custom/DiffuseAlpha";
        for (int i = 0; i < AllObject.Length; i++)
        {
            if (AllObject[i].layer.CompareTo(iLayer) != 0)
                continue;

            //Layer가 Obstacle일경우 해당 오브젝트의 메쉬 렌더러의 쉐이더를 교체한다
            Renderer[] renderers = AllObject[i].GetComponents<Renderer>();

            if (renderers == null)
                continue;

            int renderCount = renderers.Length;
            for (int j = 0; j < renderCount; j++)
            //foreach (MeshRenderer renderer in renderers)
            {
                if (renderers[j] == null)
                    continue;

                Material[] materials = renderers[j].materials;
                int materialCount = materials.Length;
                for (int k = 0; k < materialCount; k++)
                //foreach (Material M in renderer.materials)
                {
                    if (materials[k] == null)
                        continue;
                    if (materials[k].shader.name == newShaderName)
                        continue;
                    // TransParenCutOut은 제외시킨다
                    if (materials[k].shader.name == TransCutOut)
                        continue;
                    // TransParenDiffuse은 제외시킨다
                    if (materials[k].shader.name == TransDiffuse)
                        continue;
                    if (materials[k].shader.name == Foliage)
                        continue;
					if (materials[k].shader.name == DiffuseShake)
						continue;

                    Texture MainTex = materials[k].mainTexture;
                    materials[k].shader = Shader.Find(newShaderName);
                    materials[k].SetTexture(propertyName, MainTex);
                    // 컬러값 1,1,1,1
                    Color col = materials[k].color;
                    col.r = 1f;
                    col.g = 1f;
                    col.b = 1f;
                    materials[k].color = col;

                    // 혹시모를자식체크 

                    if (AllObject[i].gameObject.transform.childCount >= 1)
                    {
                        List<Renderer> ChildRender = new List<Renderer>();

                        for (int idx = 0; idx < AllObject[i].transform.childCount; idx++)
                        {
                            if (AllObject[i].transform.GetChild(idx).GetComponent<MeshRenderer>() == null)
                            {
                                if (AllObject[i].transform.GetChild(idx).GetComponent<SkinnedMeshRenderer>() != null)
                                    ChildRender.Add(AllObject[i].transform.GetChild(idx).GetComponent<SkinnedMeshRenderer>());
                            }
                            else
                            {
                                ChildRender.Add(AllObject[i].transform.GetChild(idx).GetComponent<MeshRenderer>());
                            }
                        }


                        for (int cIdx = 0; cIdx < ChildRender.Count; cIdx++)
                        {
                            Material[] mat = ChildRender[cIdx].materials;
                            int matCount = mat.Length;
                            for (int mIdx = 0; mIdx < matCount; mIdx++)
                            {
                                if (mat[mIdx] == null)
                                    continue;
                                if (mat[mIdx].shader.name == newShaderName)
                                    continue;
                                // TransParenCutOut은 제외시킨다
                                if (mat[mIdx].shader.name == TransCutOut)
                                    continue;
                                // TransParenDiffuse은 제외시킨다
                                if (mat[mIdx].shader.name == TransDiffuse)
                                    continue;
                                if (mat[mIdx].shader.name == Foliage)
                                    continue;
								if (mat[mIdx].shader.name == DiffuseShake)
									continue;

                                Texture _MainTex = mat[mIdx].mainTexture;
                                mat[mIdx].shader = Shader.Find(newShaderName);
                                mat[mIdx].SetTexture(propertyName, _MainTex);
                                // 컬러값 1,1,1,1
                                Color _col = mat[mIdx].color;
                                _col.r = 1f;
                                _col.g = 1f;
                                _col.b = 1f;
                                mat[mIdx].color = _col;
                            }

                        }





                    }
                }




            }


        }

    }

    //void CheckChild(GameObject Allobject[i])
    //{
    //    List<Renderer> ChildRender = new List<Renderer>();
    //    //자식도 있다면 찾아서바꿔줘...
    //    if (AllObject[i].gameObject.transform.childCount >= 1)
    //    {
    //        for (int k = 0; k < AllObject[i].transform.childCount; k++)
    //        {
    //            if (AllObject[i].transform.GetChild(k).GetComponent<MeshRenderer>() == null)
    //            {
    //                if (AllObject[i].transform.GetChild(k).GetComponent<SkinnedMeshRenderer>() != null)
    //                    ChildRender.Add(AllObject[i].transform.GetChild(k).GetComponent<SkinnedMeshRenderer>());
    //            }
    //            else
    //            {
    //                ChildRender.Add(AllObject[i].transform.GetChild(k).GetComponent<MeshRenderer>());
    //            }
    //        }

    //        for (int j = 0; j < ChildRender.Count; j++)
    //        {
    //            Material[] mat = ChildRender[j].materials;
    //            int matCount = mat.Length;
    //            for (int k = 0; k < matCount; k++)
    //            {
    //                if (mat[k] == null)
    //                    continue;
    //                if (mat[k].shader.name == newShaderName)
    //                    continue;
    //                // TransParenCutOut은 제외시킨다
    //                if (mat[k].shader.name == TransCutOut)
    //                    continue;
    //                // TransParenDiffuse은 제외시킨다
    //                if (mat[k].shader.name == TransDiffuse)
    //                    continue;

    //                Texture MainTex = mat[k].mainTexture;
    //                mat[k].shader = Shader.Find(newShaderName);
    //                mat[k].SetTexture(propertyName, MainTex);
    //                // 컬러값 1,1,1,1
    //                Color col = mat[k].color;
    //                col.r = 1f;
    //                col.g = 1f;
    //                col.b = 1f;
    //                mat[k].color = col;
    //            }

    //        }



    //    }
}


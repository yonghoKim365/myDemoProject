using UnityEngine;
using System.Collections;

public class FoliageParameter : MonoBehaviour {

    //public Renderer[] MeshRenderer;
    public bool IsLightMap = true;
    public float Power = 0.1f;
    public float Angle = 0;
    public Vector3 WindDirection = new Vector3(0.2f, -1, 0.2f);

    void Start()
    {
        FoliageParameter[] param = gameObject.GetComponents<FoliageParameter>();
        if (1 < param.Length)
        {
            //Debug.Log(gameObject.name + "  " + transform.parent.name);
            //Destroy(this);
            return;
        }

        string shaderName = null;
        if(IsLightMap)
            shaderName = "Custom/Foliage";
        else
            shaderName = "Custom/FoliageLight";

        Material[] materials = this.renderer.materials;
        int length = materials.Length;
        for(int i=0; i < length; i++)
        {
            Material m = materials[i];
            if (m == null)
                continue;

            Color prevColor = Color.white;
            Texture mainTex = m.mainTexture;
            if (m.shader.name != shaderName)
            {
                prevColor = m.color;
                m.shader = Shader.Find(shaderName);
            }

            Vector4 dir = WindDirection;

            if (IsLightMap)
            {
                m.SetTexture("_MainTexture", mainTex);
                m.SetFloat("_TimeValue", Power);
                m.SetFloat("_VectorX", dir.x);
                m.SetFloat("_VectorY", dir.y);
                m.SetFloat("_VectorZ", dir.z);
                m.color = prevColor;
            }
            else
            {
                m.SetTexture("_MainTexture", mainTex);
                m.SetFloat("_TimeValue", Power);
                m.SetFloat("_Angle", Angle);
                m.SetVector("_Direction", dir);
            }
        }

        //할 것이 없으므로 삭제.
        Destroy(this);
    }
    
}

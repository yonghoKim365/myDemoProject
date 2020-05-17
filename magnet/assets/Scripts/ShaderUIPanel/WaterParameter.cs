using UnityEngine;
using System.Collections;

public class WaterParameter : MonoBehaviour {
    
    public Vector4 WaveSpeed = new Vector4(-0.1f, -0.05f, -0.1f, -0.8f);
    public float Exposure = 2f;
    public float Distortion = 0.2f;
    public float Reflection = 0.5f;
    //public string Texture_1 = "Waterbump";
    //public string Texture_2 = "Water fallback";
    //public string Cubmap = "oceangradientcube";
    public float PhaseOffset = 1;
    public float Speed = 0;
    public float Depth = 0;
    public float Smoothing = 0;
    public float XDrift = 0.1f;
    public float ZDrift = 0.1f;
    public float Scale = 0;
    public float Alpha = 0.7f;

    private string ShaderName = "Water/ReflectiveBlendAnimated";

    void Start()
    {
        Material[] materials = this.renderer.materials;
        int length = materials.Length;
        for (int i = 0; i < length; i++)
        {
            Material m = materials[i];
            if (m == null)
                continue;

            if (m.shader.name != ShaderName)
                continue;//m.shader = Shader.Find(ShaderName);

            //m.SetTexture(ProertyName, mainTex);
            m.SetVector("_WaveSpeed", WaveSpeed);
            m.SetFloat("_Exposure", Exposure);
            m.SetFloat("_Distortion", Distortion);
            m.SetFloat("_Reflection", Reflection);
            m.SetFloat("_PhaseOffset", PhaseOffset);
            m.SetFloat("_Speed", Speed);
            m.SetFloat("_Depth", Depth);
            m.SetFloat("_Smoothing", Smoothing);
            m.SetFloat("_XDrift", XDrift);
            m.SetFloat("_ZDrift", ZDrift);
            m.SetFloat("_Scale", Scale);
            m.SetFloat("_Alpha", Alpha);
            //}
        }

        //할 것이 없으므로 삭제.
        Destroy(this);
    }
}

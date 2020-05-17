using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomClipAlpha : MonoBehaviour {

    public UIPanel panel = null;
    float widthScale;
    public List<Material> matList = new List<Material>();
    public float CenterX;
    public float CenterY;
    public float SizeX;
    public float SizeY;
    public Vector4 clipRange;

    public float OffSetX;
    public float OffSetY;

    public float LeftOffSet, RightOffSet, UpOffSet, DownOffSet;

    public bool bActive = false;

    Shader Clipshader;

    // Use this for initialization

    public void SetPanel(UIPanel _panel)
    {
        panel = _panel;
    }

    public void Init( string shader )
    {
        int Width = 1280;

        Clipshader = Shader.Find(shader);

        widthScale = (float)(Screen.width / (float)Width);
        clipRange = panel.baseClipRegion;

        clipRange.z *= widthScale;
        clipRange.w *= widthScale;
        
        OffSetX *= widthScale;
        OffSetY *= widthScale;
        SetMaterial();

    }

    public void SetMaterial()
    {
        matList.Clear();

        MeshRenderer[] meshrender = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer render in meshrender)
        {
            foreach (Material mat in render.materials)
            {
                mat.shader = Clipshader;
                mat.SetVector("_ScreenSize", new Vector2(Screen.width, Screen.height));
                matList.Add(mat);
            }
        }

        SkinnedMeshRenderer[] skinnedmeshrender = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer render in skinnedmeshrender)
        {
            foreach (Material mat in render.materials)
            {
                mat.shader = Clipshader;
                mat.SetVector("_ScreenSize", new Vector2(Screen.width, Screen.height));
                matList.Add(mat);
            }
        }
        
    }

    //void Update()
    //{
    //    if (matList.Count < 1)
    //        SetMaterial();
    //}

    void LateUpdate()
    {
        if (panel == null)
            return;

        clipRange = panel.baseClipRegion;
        
        clipRange.z *= widthScale;
        clipRange.w *= widthScale;

        float minX = ((Screen.width * 0.5f) + clipRange.x + OffSetX) - (clipRange.z * 0.5f);
        float minY = ((Screen.height * 0.5f) + clipRange.y + OffSetY) - (clipRange.w * 0.5f);

        Vector4 range = new Vector4(minX + LeftOffSet, minY + UpOffSet, minX + clipRange.z + RightOffSet, minY + clipRange.w + DownOffSet);
        
        foreach (Material mat in matList)
        {
            mat.SetVector("_ClipRange", range);
            
            if (bActive == true)
                mat.renderQueue = panel.startingRenderQueue + 10;
            else
                mat.renderQueue = panel.startingRenderQueue;
          
        }
    }

}

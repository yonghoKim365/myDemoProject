using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UiOpenEvent : MonoBehaviour {

    //List<UIPanel> panels = new List<UIPanel>();
	UIPanel BasePanel = new UIPanel();
    List<GameObject> EffectList = new List<GameObject>();
    //List<SkinnedMeshRenderer> units = new List<SkinnedMeshRenderer>();
    //List<MeshRenderer> models = new List<MeshRenderer>();
    //List<SkinnedMeshRenderer> models2 = new List<SkinnedMeshRenderer>();

    public float alpha = 1;
    public float Speed = 5;
    public float DelayCallback = 0.1f;

    public System.Action EndCallBack;

    float ModelAddAlpha = 0.1f;
    float DelayTime;

    bool OpenEvent = false;
    bool Live = false;
    public void InitOpenEvent()
    {
        //UIPanel[] panel = this.gameObject.GetComponentsInChildren<UIPanel>();
        //for (int i = 0; i < panel.Length; i++)
        //    panels.Add(panel[i]);
        BasePanel = this.GetComponent<UIPanel>();

        List<ParticleSystem> parList = UIHelper.FindComponents<ParticleSystem>(transform);
        for(int i=0; i < parList.Count; i++)
        {
            if (parList[i].gameObject.layer == LayerMask.NameToLayer("UI"))
                continue;

            EffectList.Add(parList[i].gameObject);
        }

        List<MeshRenderer> meshList = UIHelper.FindComponents<MeshRenderer>(transform);
        for (int i = 0; i < meshList.Count; i++)
        {
            if (meshList[i].gameObject.layer == LayerMask.NameToLayer("UI"))
                continue;

            EffectList.Add(meshList[i].gameObject);
        }
    }

	public void setBasePanel(UIPanel p){
		BasePanel = p;
	}

    //public void AddUnits(GameObject obj)
    //{   
    //    SkinnedMeshRenderer[] skinnedmeshRender = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
    //    for (int i = 0; i < skinnedmeshRender.Length; i++ )
    //    {
    //        if (skinnedmeshRender[i].material.shader.name == "Custom/ClipShader Colored (AlphaClip)")
    //            models2.Add(skinnedmeshRender[i]);
    //        else
    //        {
    //            //< 셰이더부터 변경해줌
    //            skinnedmeshRender[i].material.shader = Shader.Find("Custom/RimSpec2_Queue");
    //            units.Add(skinnedmeshRender[i]);
    //        }
    //    }
            
    //    MeshRenderer[] meshRender = obj.GetComponentsInChildren<MeshRenderer>();
    //    for (int i = 0; i < meshRender.Length; i++)
    //        models.Add(meshRender[i]);

    //    if(Live)
    //        DataUpdate(alpha == 0 ? (alpha - ModelAddAlpha) : alpha);
    //}

    //public void AddPanel(UIPanel panel)
    //{
        //panels.Add(panel);
    //}

    public void SetReset()
    {
        //StopCoroutine("StartEvent");
        DataUpdate(1);
    }
    
    public void SetEvent(bool type, System.Action _EndCallBack = null)
    {
        if (Live)
        {
            DataUpdate(alpha);
            EndCallBack = null;
            //StopCoroutine("StartEvent");
        }

        if (TownState.TownActive && SceneManager.instance.GetState<TownState>().IsEndSceneLoad)
            UIMgr.instance.UICamera.enabled = false;

        EndCallBack = _EndCallBack;
        
        OpenEvent = type;
        alpha = OpenEvent ? 0 : 1;

        DataUpdate(alpha);

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Live = true;
        //StartCoroutine("StartEvent");
    }
    /*
    IEnumerator StartEvent()
    {
        yield return null;

        
        DataUpdate(alpha == 0 ? (alpha - ModelAddAlpha) : alpha);
        //if (OpenEvent)
        //    yield return new WaitForSeconds(0.2f);

        while(true)
        {
            alpha = OpenEvent ? (alpha + (Speed * Time.deltaTime)) : (alpha - (Speed * Time.deltaTime));
            if (alpha > 1)
                alpha = 1;
            else if (alpha < 0)
                alpha = 0;

            DataUpdate(alpha);

            if ((OpenEvent && alpha == 1) || (!OpenEvent && alpha == 0))
                break;

            yield return null;
        }
        
        UIMgr.instance.UICamera.enabled = true;

        //yield return new WaitForSeconds(0.1f);//약간의 텀

        if (EndCallBack != null)
            EndCallBack();
        
        Live = false;
    }
    */
    
    void Update()
    {
        if ( !Live)
            return;

        if(OpenEvent && DelayTime < DelayCallback)
        {
            DelayTime += Time.deltaTime;
            return;
        }
        
        if ((OpenEvent && alpha == 1) || (!OpenEvent && alpha == 0))
        {
            //if (!OpenEvent && alpha == 0)
            //{
            //    DelayTime += Time.deltaTime;
            //    if (DelayTime < DelayCallback)
            //        return;
            //}
            
            DelayTime = 0;
            if(TownState.TownActive && SceneManager.instance.GetState<TownState>().IsEndSceneLoad)
                UIMgr.instance.UICamera.enabled = true;
            if (EndCallBack != null)
                EndCallBack();

            Live = false;
        }
        else
        {
            alpha = OpenEvent ? (alpha + (Speed * Time.deltaTime)) : (alpha - (Speed * Time.deltaTime));
            if (alpha > 1)
                alpha = 1;
            else if (alpha < 0)
                alpha = 0;

            DataUpdate(alpha);
        }
    }


    void DataUpdate(float alpha)
    {
        //for (int i = 0; i < panels.Count; i++)
        //{
        //    if (panels[i] != null)
        //        panels[i].alpha = alpha;
        //}
        BasePanel.alpha = alpha;

        //float colorAlpha = alpha + ModelAddAlpha;
        //if (colorAlpha > 1)
        //    colorAlpha = 1;
        //else if (colorAlpha < 0)
        //    colorAlpha = 0;

        //Color32 color2 = new Color32(0, 0, 0, (byte)(alpha >= 0 ? (byte)(255 * (alpha)) : (byte)0));
        for (int i=0; i < EffectList.Count; i++)
        {
            if (EffectList[i] != null)
            {
                if (OpenEvent)
                {
                    if(0.5f <= alpha && !EffectList[i].activeSelf)
                        EffectList[i].SetActive(true);
                }
                else
                {
                    if (alpha <= 0.5f && EffectList[i].activeSelf)
                        EffectList[i].SetActive(false);
                }
            }
        }

        //Color32 color = new Color32(0, 0, 0, (byte)(255 * (colorAlpha)));
        //for (int i = 0; i < units.Count; i++)
        //{
        //    if (units[i] != null)
        //    {
        //        for (int j = 0; j < units[i].materials.Length; j++)
        //            units[i].materials[j].SetColor("_AddColor", color);
        //    }
        //}

        //if (models.Count > 0)
        //{
        //    Color32 color2 = new Color32(0, 0, 0, (byte)(alpha >= 0 ? (byte)(255 * (alpha)) : (byte)0));
        //    for (int i = 0; i < models.Count; i++)
        //    {
        //        if (models[i] != null)
        //        {
        //            for (int j = 0; j < models[i].materials.Length; j++)
        //                models[i].materials[j].SetColor("_AddColor", color2);
        //        }
        //    }
        //}

        //if (models2.Count > 0)
        //{
        //    Color32 color2 = new Color32(0, 0, 0, (byte)(alpha >= 0 ? (byte)(255 * (alpha)) : (byte)0));
        //    for (int i = 0; i < models2.Count; i++)
        //    {
        //        if (models2[i] != null)
        //        {
        //            for (int j = 0; j < models2[i].materials.Length; j++)
        //                models2[i].materials[j].SetColor("_AddColor", color2);
        //        }
        //    }
        //}
        
    }

    public bool IsHiding
    {
        get {
            return !OpenEvent && Live;
        }
    }

    public bool IsShowing
    {
        get
        {
            return OpenEvent && Live;
        }
    }
}

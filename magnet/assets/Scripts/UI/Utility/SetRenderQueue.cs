using UnityEngine;
//[ExecuteInEditMode]
public class SetRenderQueue : MonoBehaviour
{
    public int renderQueue = 3000;

    private Material mMat;
    private Material sMat;

    private bool IsSetting;
    
    void Awake ()
    {
        if (IsSetting)
            return;

        IsSetting = true;
        SetRenderQ(renderQueue);
    }

    void SetRenderQ(int renerQ)
    {
        Renderer ren = renderer;

        if (ren == null)
        {
            ParticleSystem sys = GetComponent<ParticleSystem>();
            if (sys != null) ren = sys.renderer;
        }

        if (ren != null && ren.sharedMaterial != null)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if (trail != null && sMat == null)
            {
                sMat = new Material(trail.sharedMaterial);
                sMat.renderQueue = renderQueue;
                trail.material = sMat;
            }
            else if(sMat != null)
                sMat.renderQueue = renderQueue;

            if (mMat == null)
                mMat = new Material(ren.sharedMaterial);

            mMat.renderQueue = renerQ;
            ren.material = mMat;
        }
    }

    void OnDestroy () { if (mMat != null) Destroy(mMat); }

    public void ResetRenderQ(int renderQ)
    {
        gameObject.SetActive(false);
        IsSetting = true;

        if (renderQ == -1)//원본으로 셋팅 명령 값
            renderQ = renderQueue;
        renderQueue = renderQ;

        SetRenderQ(renderQ);

        gameObject.SetActive(true);
    }
}
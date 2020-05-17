using UnityEngine;
using System.Collections;

public class CameraFadeEvent : MonoBehaviour {

    public MeshRenderer coverMeshRender;
    Material CoverMaterial { get { return coverMeshRender.materials[0]; } }

    float Alpha = 0;
    float FadeSpeed = 400;

	void Awake () 
    {
	    //< 처음 생성될때 색상은 알파 0으로
        Alpha = 0;
        CoverMaterial.color = new Color32(255, 255, 255, (byte)Alpha);
	}

    public void SetFadeOut()
    {
        StartCoroutine(FadeOutUpdate());
    }

    public IEnumerator FadeOutUpdate(System.Action endcall = null)
    {
        Alpha = 0;

        while(true)
        {
            Alpha += FadeSpeed * Time.deltaTime;
            if (Alpha >= 255)
            {
                Alpha = 255;
                CoverMaterial.color = new Color32(255, 255, 255, (byte)Alpha);
                break;
            }
            CoverMaterial.color = new Color32(255, 255, 255, (byte)Alpha);

            yield return null;
        }

        if (endcall != null)
            endcall();
    }

    public void SetFadeIn()
    {
        StartCoroutine(FadeInUpdate());
    }

    public IEnumerator FadeInUpdate()
    {
        yield return new WaitForSeconds(0.2f);

        Alpha = 255;
        while (true)
        {
            Alpha -= FadeSpeed * Time.deltaTime;
            if (Alpha <= 0)
            {
                Alpha = 0;
                CoverMaterial.color = new Color32(255, 255, 255, (byte)Alpha);
                break;
            }
            CoverMaterial.color = new Color32(255, 255, 255, (byte)Alpha);

            yield return null;
        }
    }
	
}

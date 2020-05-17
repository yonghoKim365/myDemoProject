using UnityEngine;
using System.Collections;

public class SmoothDestroy : MonoBehaviour 
{
    
    Material targetMat;
    Color Color;
    float lifeTime;

    public void Init(Material mat, Color startColor, float _lifeTime = 1.5f)
    {
        targetMat = mat;
        Color = startColor;
        lifeTime = _lifeTime;

        StartCoroutine( Routine() );
    }

    IEnumerator Routine()
    {
        float t = 0;
        Color col = Color;

        while (t <= 1f)
        {
            col.a = Mathf.Clamp01( 1f - t );

            targetMat.SetColor( "_Color", col );

            t += (Time.deltaTime / lifeTime);

            yield return null;
        }

        Destroy( gameObject );
    }
}

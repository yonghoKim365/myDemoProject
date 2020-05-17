using UnityEngine;
using System.Collections;

public class UnitEffectHelper
{
    #region :: Shader 이펙트 관련 ::

    /// <summary>
    /// 죽었을때 투명하게 만들기
    /// </summary>
    public static IEnumerator DeadEffect(GameObject go, float duration, System.Action call = null)
    {
        SkinnedMeshRenderer[] SMR = go.GetComponentsInChildren<SkinnedMeshRenderer>();

        NGUITools.SetLayer(go, 0);

        float ratio = 0;
        float startT = Time.time;
        while (ratio < 0.8)
        {
            ratio = Mathf.Clamp01((Time.time - startT) / duration);
            for (int i = 0; i < SMR.Length; i++)
                for (int j = 0; j < SMR[i].materials.Length; j++)
                    SMR[i].materials[j].SetFloat("_Blend", ratio);

            yield return null;
        }

        if (call != null)
            call();

        go.SetActive(false);
    }

    /// <summary>
    /// 현재 사용안함
    /// </summary>
    public static IEnumerator FlickerEffect(GameObject go, float startTime, float duration)
    {
        SkinnedMeshRenderer[] SMRs = ChangeShader( go, "Unlit/Texture_AddColor" );
        if (SMRs == null)
            yield break;

        float ratio = 0;
        while (ratio < 1f)
        {
            ratio = Mathf.Clamp01((Time.time - startTime) / duration);
            float col = (1 - ratio) * GameDefine.DamageColor.r;
            Color CalcColor = new Color(col, col, col);
            
            foreach (SkinnedMeshRenderer renderer in SMRs)
                foreach (Material M in renderer.materials)
                    M.SetColor( "_AddColor", CalcColor );

            yield return null;
        }
    }

    public static SkinnedMeshRenderer[] ChangeShader(GameObject go, string newShaderName)
    {
        SkinnedMeshRenderer[] SMRs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (SMRs == null)
            return null;

        // Shader 체인지
        foreach (SkinnedMeshRenderer renderer in SMRs)
        {
            foreach (Material M in renderer.materials)
            {
                if (M.shader.name != newShaderName)
                {
                    Texture MainTex = M.mainTexture;
                    M.shader = Shader.Find( newShaderName );
                    M.SetTexture( "_Texture", MainTex );
                }
            }
        }

        return SMRs;
    }

    /// <summary>
    /// 해당 객체내의 Material들의 Color Property를 변경하게 해준다.
    /// </summary>
    public static void ChangeColors(GameObject go, string propertyName, Color changingColor)
    {
        SkinnedMeshRenderer[] SMRs = UnitEffectHelper.GetHasProperty( go, propertyName );
        if (SMRs == null)
            return;

        foreach (SkinnedMeshRenderer renderer in SMRs)
            foreach (Material M in renderer.materials)
                M.SetColor( propertyName, changingColor );
    }

    /// <summary>
    /// Material에 주어진 Property가 존재하는 Renderer Component를 찾아준다.
    /// </summary>
    public static SkinnedMeshRenderer[] GetHasProperty(GameObject go, string propertyName)
    {
        return System.Array.FindAll(go.GetComponentsInChildren<SkinnedMeshRenderer>(true), (r) => 
        {  
            foreach (Material M in r.materials)
            {
                return M.HasProperty( propertyName );
            }

            return false;
        });
    }

    #endregion

    /// <summary>
    /// 대상 객체를 흔들도록 한다.
    /// </summary>
    public static IEnumerator ShakeModelRoutine(Transform targetTrans)
    {
        float shakeIntensity = 0.2f;
        float shakeDecoy = 0.02f;
        float time = 0.2f;
        
        while (time > 0f)
        {
            Vector3 newPos = Random.insideUnitSphere * shakeIntensity;
            newPos.z = newPos.y = 0f;
            targetTrans.localPosition = newPos;

            shakeIntensity -= shakeDecoy;
            time -= Time.deltaTime;
            
            yield return null;
        }

        targetTrans.localPosition = Vector3.zero;
    }
}

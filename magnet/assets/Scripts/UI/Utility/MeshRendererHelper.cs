using UnityEngine;
using System.Collections;

public class MeshRendererHelper : MonoBehaviour {

    public CheatPopup Cheat;

    void OnSetColor(Color color)
    {
        Unit[] unit = FindObjectsOfType<Unit>();

        foreach(Unit thisunit in unit)
        {
            if(thisunit.UnitType == UnitType.Unit || thisunit.UnitType == UnitType.Npc || thisunit.UnitType == UnitType.TownUnit || thisunit.UnitType == UnitType.TownNpc)
            {
                SkinnedMeshRenderer[] mrs = thisunit.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (SkinnedMeshRenderer renderer in mrs)
                {
                    foreach (Material M in renderer.materials)
                    {
                        M.SetColor("_MainColor", color);
                    }
                }
            }            
        }

        if(Cheat != null)
        {
            Cheat.SetColor(color);
        }
    }

    public void OnGetColor(ColorPicker picker)
    {
        Unit[] unit = FindObjectsOfType<Unit>();

        foreach (Unit thisunit in unit)
        {
            if (thisunit.UnitType == UnitType.Unit || thisunit.UnitType == UnitType.TownUnit )
            {
                SkinnedMeshRenderer[] mrs = thisunit.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (SkinnedMeshRenderer renderer in mrs)
                {
                    foreach (Material M in renderer.materials)
                    {
                        picker.NotifyColor(M.GetColor("_MainColor"));
                        return;
                    }
                }
            }
        }
    }

    public float GetIntensity()
    {
        float intensity = 0f;
        Unit[] unit = FindObjectsOfType<Unit>();

        foreach (Unit thisunit in unit)
        {
            if (thisunit.UnitType == UnitType.Unit || thisunit.UnitType == UnitType.TownUnit)
            {
                SkinnedMeshRenderer[] mrs = thisunit.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (SkinnedMeshRenderer renderer in mrs)
                {
                    foreach (Material M in renderer.materials)
                    {
                        intensity = M.GetFloat("_Intensity");
                        return intensity;
                    }
                }
            }
        }

        return intensity;
    }

    public void SetIntensity(float intensity)
    {
        Unit[] unit = FindObjectsOfType<Unit>();

        foreach (Unit thisunit in unit)
        {
            if (thisunit.UnitType == UnitType.Unit || thisunit.UnitType == UnitType.Npc || thisunit.UnitType == UnitType.TownUnit || thisunit.UnitType == UnitType.TownNpc)
            {
                SkinnedMeshRenderer[] mrs = thisunit.GetComponentsInChildren<SkinnedMeshRenderer>();

                foreach (SkinnedMeshRenderer renderer in mrs)
                {
                    foreach (Material M in renderer.materials)
                    {
                        M.SetFloat("_Intensity", intensity);
                    }
                }
            }
        }
    }
}

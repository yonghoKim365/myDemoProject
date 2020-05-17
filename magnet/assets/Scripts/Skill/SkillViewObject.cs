using UnityEngine;
using System.Collections;

public class SkillViewObject : MonoBehaviour {

    public GameObject BarViewObj, CylinderObj;

    public void SetBarView(float width, float length)
    {
        BarViewObj.SetActive(true);
        CylinderObj.SetActive(false);

        BarViewObj.transform.localScale = new Vector3(width, length, 0);
        BarViewObj.transform.localPosition = new Vector3(0, 0, (length / 2));
    }

    public void SetEllipse(float range)
    {
        BarViewObj.SetActive(false);
        CylinderObj.SetActive(true);

        CylinderObj.transform.localScale = new Vector3(range, 0, range);
    }

    public void SetEllipse(float range, float angle)
    {
        BarViewObj.SetActive(false);
        CylinderObj.SetActive(true);

        CylinderObj.transform.localScale = new Vector3(range, 0, range);
    }
}

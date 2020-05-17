using UnityEngine;
using System.Collections;

public class CameraAdjustArea : MonoBehaviour {
    public Vector3 AdjustEulerAngles;
    private CameraAdjustManager manager;
    public float CameraDistance = 16f;

    public bool CameraRollBackFlag = false;
    public Vector3 CameraRollBackEulerAngles;
    public float CameraRollBackDistance = 16f;

    private void Awake()
    {
        manager = GameObject.Find("CameraAdjustManager").GetComponent<CameraAdjustManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.CameraAdjust("", AdjustEulerAngles, CameraDistance );
    }

    private void OnTriggerExit(Collider other)
    {
        if(CameraRollBackFlag)
        {
            manager.CameraAdjust("", CameraRollBackEulerAngles, CameraRollBackDistance);
        }
    }
}

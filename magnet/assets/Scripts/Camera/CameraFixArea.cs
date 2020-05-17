using UnityEngine;
using System.Collections;

public class CameraFixArea : MonoBehaviour
{
    public Vector3 AdjustEulerAngles;
    private CameraAdjustManager manager;
    public float CameraDistance = 16f;
    public GameObject FixTarget;

    private void Awake()
    {
        manager = GameObject.Find("CameraAdjustManager").GetComponent<CameraAdjustManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        manager.AdjustFixCamera(AdjustEulerAngles, CameraDistance);

        CameraManager.instance.Follow(FixTarget.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        CameraManager.instance.Follow(G_GameInfo.PlayerController.Leader.transform);
    }
}

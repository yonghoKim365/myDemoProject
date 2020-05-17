using UnityEngine;
using System.Collections;

public class CameraAdjustManager : MonoBehaviour {
    public Vector3 StartCameraEulerAngles;
    IEnumerator co=null;

    public void CameraAdjust(string ColliderName, Vector3 ViewEulerAngles, float CameraDistance, float LerpSpeed = 0.01f)
    {
        if( co != null )
            StopCoroutine(co);

        co = AdjustCamera(ViewEulerAngles, CameraDistance, LerpSpeed);
        StartCoroutine(co);
    }

    IEnumerator AdjustCamera(Vector3 Angle, float CameraDistance, float LerpSpeed)
    {
        while (!(CameraManager.instance.RtsCamera.Tilt == Angle.x && CameraManager.instance.RtsCamera.Rotation == Angle.y))
        {
            CameraManager.instance.RtsCamera.Tilt = Mathf.Lerp(CameraManager.instance.RtsCamera.Tilt, Angle.x, LerpSpeed);
            CameraManager.instance.RtsCamera.Rotation = Mathf.Lerp(CameraManager.instance.RtsCamera.Rotation, Angle.y, LerpSpeed);
            CameraManager.instance.RtsCamera.Distance = Mathf.Lerp(CameraManager.instance.RtsCamera.Distance, CameraDistance, LerpSpeed);
            yield return null;
        }

        yield return null;
    }

    public void AdjustFixCamera(Vector3 Angle, float CameraDistance)
    {

        if (co != null)
            StopCoroutine(co);

        CameraManager.instance.RtsCamera.Tilt = Angle.x;
        CameraManager.instance.RtsCamera.Rotation = Angle.y;
        CameraManager.instance.RtsCamera.Distance = CameraDistance;
    
    }
}

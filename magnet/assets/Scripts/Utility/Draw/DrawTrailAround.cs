using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TrailRenderer))]
public class DrawTrailAround : MonoBehaviour
{
    public Transform Target;

    public TrailRenderer TrailDrawer;

    public float RotateSpeed = 1f;
    public float Distance = 1f;

    private float rotate = 0;
    private Vector3 heightVec3 = Vector3.zero;
    private Vector3 addedPos = Vector3.zero;

    void Awake()
    {
        TrailDrawer = GetComponent<TrailRenderer>();
    }

    void Update()
    {
        if (null == Target)
            return;

        rotate += RotateSpeed * Time.deltaTime;
        
        Vector3 newDirPos = (Quaternion.Euler(0, rotate, 0) * ( Target.transform.forward * Distance ));

        transform.position = Target.transform.position + heightVec3 + addedPos + newDirPos;
    }

    public void Setup(Transform target, float rotSpeed, float roundDistance, float yPos = 5f, Vector3 addPos = default(Vector3))
    {
        StartCoroutine( "ClearRoutine" );
        
        Target = target;
        RotateSpeed = rotSpeed;
        Distance = roundDistance;
        heightVec3 = new Vector3(0, yPos, 0);
        addedPos = addPos;
    }

    IEnumerator ClearRoutine()
    {
        float saveTime = TrailDrawer.time;
        TrailDrawer.time = 0f;

        yield return null;

        TrailDrawer.time = saveTime;
    }
}

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PathPoint : MonoBehaviour
{
    public Transform CachedTrans;
    public Vector3 Bezier = Vector3.zero;
    public Vector3 BezierPosition
    {
        get { return Bezier + transform.position;}
        set { Bezier = value - transform.position; }
    }
 
	public Vector3 mOffset = Vector3.zero;
	public Vector3 Offset
	{
		get { return transform.position + mOffset;}
		set { mOffset = value - transform.position;}		
	}
	
    public float MoveDamping = 0.3f;

    void Start()
    {
        CachedTrans = transform;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathData : MonoBehaviour {

    public float Distance = 7.0f;

    // 카메라 점들
    private List<PathPoint> mPoints = new List<PathPoint>();
    public List<PathPoint> Points
    {
        get { return mPoints; }
    }

    public GameObject Target;
    public Color LineColor = Color.white;

	// Use this for initialization
	void Start () 
    {
        Target = null;
        Points.Clear();
        int ChildCount = transform.childCount;
        for (int i = 0; i < ChildCount; i++)
        {
            PathPoint CP = transform.GetChild(i).GetComponent<PathPoint>();
            Points.Add(CP);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // index뒤에 pt위치에 추가
    public void AddCameraPoint(int NewIndex = -1)
    {
        if (NewIndex == -1)
            NewIndex = mPoints.Count;

        GameObject newPoint = new GameObject("Control Point");
        newPoint.transform.parent = transform;

        PathPoint pt = newPoint.AddComponent<PathPoint>();
        mPoints.Insert(NewIndex, pt);

        NewIndex = Mathf.Max(0, NewIndex - 1);
        // 마지막 인덱스에 추가

        pt.transform.position = mPoints[NewIndex].transform.position + mPoints[NewIndex].transform.forward * 5;
        pt.BezierPosition = pt.transform.position;
    }

    public bool DeleteCameraPoint(int index)
    {
        if (mPoints.Count <= index)
            return false;

        if (mPoints[index] != null && mPoints[index].gameObject)
            DestroyImmediate(mPoints[index].gameObject);

        mPoints.RemoveAt(index);
        Debug.Log("Points Count = " + mPoints.Count);
        return true;
    }

}

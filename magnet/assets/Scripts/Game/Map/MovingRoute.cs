using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 이동 경로 관리 클래스
/// </summary>
public class MovingRoute : MonoBehaviour 
{
    /// <summary>
    /// 제일 첫번째 노드
    /// </summary>
    public GameObject   Start { get { return GetNode(0); } }

    /// <summary>
    /// 가장 마지막 노드
    /// </summary>
    public GameObject   End { get { return GetNode( Count - 1 ); } }
    public int          Count { get { return routeLink.Count; } }
    public bool         IsEmpty { get { return 0 == routeLink.Count; } }

    public List<GameObject> RouteLink { get { return routeLink; } }

    [SerializeField][HideInInspector]
    List<GameObject>        routeLink = new List<GameObject>();

    public GameObject CreatePrev(GameObject baseNode = null)
    {
        GameObject newGo = CreateEmptyNode( baseNode );
        AddPrev( newGo, baseNode );

        return newGo;
    }

    public GameObject CreateNext(GameObject baseNode = null)
    {
        GameObject newGo = CreateEmptyNode( baseNode );
        AddNext( newGo, baseNode );

        return newGo;
    }

    public void AddPrev(GameObject newNode, GameObject node = null)
    {
        if (CreateStartPosition())
            return;

        int foundIndex = RouteLink.FindIndex( (go) => { return go == node; } );
        if (-1 != foundIndex)
            RouteLink.Insert( Mathf.Clamp( foundIndex, 0, int.MaxValue ), newNode );
        else
            RouteLink.Add( newNode );

        RenameChildren();
    }

    public void AddNext(GameObject newNode, GameObject node = null)
    {
        if (CreateStartPosition())
            return;

        int foundIndex = RouteLink.FindIndex( (go) => { return go == node; } );
        if (-1 != foundIndex)
            RouteLink.Insert( Mathf.Clamp( foundIndex + 1, 0, int.MaxValue ), newNode );
        else
            RouteLink.Add( newNode );

        RenameChildren();
    }

    public bool Remove(GameObject node)
    {
        bool removed = RouteLink.Remove( node );
        if (removed)
            DestroyImmediate( node );

        RenameChildren();

        return removed;
    }

    Vector3 ClosestPointOnLine(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 v1 = point - a;
        Vector3 v2 = ( b - a ).normalized;

        float d = Vector3.Distance( a, b );
        float t = Vector3.Dot( v2, v1 );

        if (t <= 0)
            return a;

        if (t >= d)
            return b;

        Vector3 v3 = b * t;
        Vector3 closestPoint = a + v3;

        return closestPoint;
    }

    public List<Vector3> GetPositions()
    {
        List<Vector3> posList = new List<Vector3>();
        for (int i = 0; i < routeLink.Count; i++)
            posList.Add(routeLink[i].transform.position);

        return posList;
    }

    public List<Vector3> GetReversePositions()
    {
        List<Vector3> posList = GetPositions();
        posList.Reverse();
        return posList;
    }
    
    public GameObject GetNode(int index)
    {
        index = Mathf.Clamp(index, 0, routeLink.Count - 1);
        return routeLink[index];
    }

    /// <summary>
    /// 루트에 아무것도 없는지 검사해서 최소한 1개를 루트에 넣어준다.
    /// </summary>
    /// <returns>루트가 하나도 없어서 새로 추가했다면 true</returns>
    public bool CreateStartPosition()
    {
        if (0 == RouteLink.Count)
        {
            GameObject newGo = new GameObject(RouteLink.Count.ToString());
            newGo.transform.AttachTo( transform );
            RouteLink.Add( newGo );

            return true;
        }

        return false;
    }

    GameObject CreateEmptyNode(GameObject baseNode = null)
    {
        GameObject newGo = new GameObject();
        newGo.transform.AttachTo( transform );

        Vector3 addPos = Vector3.forward;
        if (null != baseNode)
            addPos = baseNode.transform.position + baseNode.transform.forward * 2f;
        else
            addPos = transform.forward * 2f;

        newGo.transform.localPosition += addPos;

        return newGo;
    }

    /// <summary>
    /// 자식들 이름 재설정
    /// </summary>
    void RenameChildren()
    {
        for (int i = 0; i < routeLink.Count; i++)
        {
            routeLink[i].name = i.ToString();
        }
    }
}

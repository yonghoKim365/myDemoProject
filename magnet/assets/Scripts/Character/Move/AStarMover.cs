using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 방향마다의 비용을 저장, 연산시켜주는 클래스
/// </summary>
public class PathVertex
{
    public float Coast { get { return F; } }
    public Vector2 myTilePos { get { return curTilePos; } }
    public PathVertex Parent { get { return _prevVertex; } }
    PathVertex _prevVertex = null;
    Vector2 curTilePos;
    float F = 0f;                   // 들어가는 최종 비용으로써, 나중에 길찾기의 기준으로 사용된다.
    float G = 0f;                   // 시작점부터 이동할 방향까지 걸리는 거리
    float H = 0f;                   // 모든 장애물을 무시하고 목적지까지 가는 데 걸리는 거리

    public PathVertex(PathVertex prevVertex, Vector2 curPos, Vector2 endPos)
    {
        curTilePos = curPos;
        _prevVertex = prevVertex;
        if (prevVertex == null)
            G = 1;
        else
            G = prevVertex.G + 1;
        H = Mathf.Abs(endPos.x - curPos.x) + Mathf.Abs(endPos.y - curPos.y);
        F = G + H;
    }
}

public class AStarMover : MonoSingleton<AStarMover>
{
    // 이동 방향 정보
    enum Direction { left, right, up, down, leftup, rightup, leftdown, rightdown };
    static Vector2[] DirectionList = {
            new Vector2(1f, 0f), new Vector2(-1f, 0f),
            new Vector2(0f, 1f), new Vector2(0f, -1f),
            new Vector2(1f, 1f), new Vector2(-1f, 1f),
            new Vector2(1f, -1f), new Vector2(-1f, -1f)};

    List<PathVertex> _openList
        = new List<PathVertex>();
    List<PathVertex> _closeList
        = new List<PathVertex>();
    List<PathVertex> _path
        = new List<PathVertex>();

    public Vector2 _myCurPos = Vector2.zero;                   // 자신이 현제 있는 타일
    Vector2 _destination = Vector2.zero;                // 최종 목적지
    //Vector2 _nextMovePos = Vector2.zero;                // 다음에 실질적으로 움직여줄 공간

    public List<PathVertex> CalculatePath(Vector2 Start, Vector2 End)
    {
        _myCurPos = Start;
        Move(End);

        return _path;
    }

    /*
    void Awake()
    {
        Owner = GetComponent<Unit>();
        OwnerTrans = Owner.transform;

        _myCurPos = Owner.Position;
        Move(new Vector2(49, 19));
        TestPreview();
        bUse = true;
        TargetPos = gameObject.transform.position;
    }


    public void TestPreview()
    {
        for (int i = 0; i < _path.Count; i++)
        {
            GameObject ret = GameObject.Find("TiledObj_" + (int)_path[i].myTilePos.x + "_" + (int)_path[i].myTilePos.y);

            if (ret != null)
            {
                MeshRenderer temprend = ret.GetComponent<MeshRenderer>();
                temprend.material.SetColor("_Color", Color.yellow);
            }
        }

        //if (!_myCurPos.Equals(_destination) && _path.Count > _pathPrograss)
        //{
        //    _tick = 0f;
        //    _nextMovePos = _path[_pathPrograss].myTilePos;
        //    _myCurPos = _nextMovePos;
        //    _pathPrograss += 1;
        //}
    }
    */

    /// <param name="destination">목적지 좌표</param>
    public Vector2 Move(Vector2 destination)
    {
        // 초기화
        _destination = destination;

        _path.Clear();
        _closeList.Clear();
        _openList.Clear();

        // 길 구하기
        PathVertex latestVertex = Simulate();
        PathVertex traceVertex = latestVertex;
        while (traceVertex != null)
        {
            _path.Add(traceVertex);
            traceVertex = traceVertex.Parent;
        }
        _path.Reverse();

        // 로그 출력
        /*
        Debug.Log("Finish");
        for (int i = 0; i < _path.Count; i++)
            Debug.Log("Path: " + _path[i].myTilePos.ToString());
        */
        if (_path.Count >= 2)
            return _path[1].myTilePos;
        else
            return _myCurPos;

    }

    /// <summary>
    /// 어느 경로가 가능한가를 판단
    /// </summary>
    /// <returns>맨 마지막 좌표</returns>
    PathVertex Simulate()
    {
        PathVertex latestVertex = new PathVertex(null, _myCurPos, _destination);
        while (!latestVertex.myTilePos.Equals(_destination))
        {
            foreach (var direction in DirectionList)
            {
                Vector2 nextLocation = latestVertex.myTilePos + direction;
                if (!Exception(nextLocation))
                {
                    PathVertex nextVertex = new PathVertex(latestVertex, nextLocation, _destination);
                    AddinOpenList(nextVertex);
                }
            }

            PathVertex bestVertex = null;
            if (!_openList.Count.Equals(0))
                bestVertex = _openList[0];
            foreach (var vertex in _openList)
                if (bestVertex.Coast >= vertex.Coast)
                    bestVertex = vertex;
            _openList.Remove(bestVertex);
            _closeList.Add(bestVertex);

            latestVertex = bestVertex;

            if (latestVertex == null)
            {
                //길을 찾을수없다 
                return null;
            }
        }
        return latestVertex;
    }

    /// <summary>
    /// 오픈 리스트에 더하기 전,
    /// 몇가지 사항들을 비교함
    /// </summary>
    /// <param name="target"></param>
    void AddinOpenList(PathVertex target)
    {
        foreach (var vertex in _closeList)
            if (target.myTilePos.Equals(vertex.myTilePos))
                return;

        foreach (var vertex in _openList)
            if (target.myTilePos.Equals(vertex.myTilePos))
                if (vertex.Coast > target.Coast)
                {
                    _openList.Remove(vertex);
                    _openList.Add(target);
                    return;
                }

        _openList.Add(target);
    }

    bool Exception(Vector2 point)
    {
        // 맵 배열 인덱스 바깥의 영역을 가리키고 있는가
        if (NaviTileInfo.instance.GetTilePos((int)point.x, (int)point.y) == Vector3.zero)
            return true;

        if (!NaviTileInfo.instance.GetMoveablePos((int)point.x, (int)point.y))
            return true;

        /*
        if (point.x < 0f || point.x >= BoardManager.Instance.GetWidth() ||
           point.y < 0f || point.y >= BoardManager.Instance.GetHeight())
            return true;

        // 가리키고 있는 타일은 지나갈 수 있는 타일인가
        //bool isPassable = TileDirector.Instance.MapData[(int)point.x, (int)point.y].isPassable;
        bool isPassable = BoardManager.Instance.MoveablePos((int)point.x, (int)point.y);
        if (!isPassable)
            return true;
        */

        return false;
    }
}

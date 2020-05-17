using UnityEngine;
using System.Collections;

public class TownUnitMoveHelper : MonoBehaviour {
    Unit parent = null;
    public bool _active = false;
    public Vector3 targetPos = Vector3.zero;

    // Use this for initialization
    void Start () {
        parent = GetComponent<Unit>();
    }

    public void MovePosition(Vector3 pos)
    {
        _active = true;
        NavMeshHit navHit;
        targetPos = pos;
        if (NavMesh.SamplePosition(targetPos, out navHit, Vector3.Distance(targetPos, parent.cachedTransform.position), 9))
        {
            // 9 == Terrain
            targetPos = navHit.position;
        }
    }

    public void StopMove()
    {
        _active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (TownState.TownActive)
        {
            if(_active)
            {
                Vector3 dest;
                if(!CalcPosition(targetPos, out dest) )
                {
                    _active = false;
                    return;
                }

                if(!parent.MovePosition(dest, 1f))
                {
                    _active = false;
                }
            }
        }
    }

    public bool CalcPosition(Vector3 dest, out Vector3 result)
    {
        bool success;
        success = CalculatePath(dest, false);

        if (success)
        {
            if (movePath.corners.Length > 1)
            {
                Vector3 targetPos = movePath.corners[movePathIndex];
                Vector3 curPos = parent.cachedTransform.position;

                if ((dest - curPos).sqrMagnitude < 1f)
                {
                    result = Vector3.zero;
                    return false;
                }
                    

                targetPos.y = curPos.y = 0; // x, z값은 0이고, y만 값만 존재할때 이동문제 발생 (높낮이 맵에 의한)

                Vector3 offset = targetPos - curPos;
                float movespeed = parent.CharInfo.MoveSpeed;// 8.5f;// CharInfo != null ? CharInfo.MoveSpeed : 8.5f;
                                                            //Debug.LogWarning("2JW : " + Owner + " : " + movespeed + " : " + CharInfo.MoveSpeed);
                                                            //float speed = movespeed * speedRatio;

                // 기본으로 이동해야할 힘 = 방향 * 속도
                Vector3 velocity = offset.normalized;

                result = parent.cachedTransform.position + velocity;

                return true;
            }
            else
            {
                //이동불가
            }
        }

        result = Vector3.zero;

        return false;
    }

    protected NavMeshPath movePath = new NavMeshPath();
    protected int movePathIndex = 0;

    public void ClearPath()
    {
        movePath.ClearCorners();
        movePathIndex = 1;
    }

    public bool CalculatePath(Vector3 TargetPos, bool end = false)
    {
        if (!parent.gameObject.activeSelf || !parent.UsableNavAgent || float.IsNaN(TargetPos.x) || movePath == null)
            return false;

        ClearPath();

        // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 NavMesh가능 위치를 찾아준다.
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(TargetPos, out navHit, Vector3.Distance(TargetPos, parent.cachedTransform.position), 9))
        {
            // 9 == Terrain
            TargetPos = navHit.position;
        }

        bool Find = parent.navAgent.CalculatePath(TargetPos, movePath);

        // 시작점과 끝점은 계산에서 제외시킴
        for (int i = 1; i < movePath.corners.Length - 1; i++)
        {
            // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
            if (NavMesh.FindClosestEdge(movePath.corners[i], out navHit, 1))
            {
                if ((navHit.position - movePath.corners[i]).sqrMagnitude < 1f)
                {
                    movePath.corners[i] = movePath.corners[i] + navHit.normal * 1f;
                }
            }
        }

        // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
        if (movePath.corners.Length >= 2)
        {
            // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
            for (int i = 1; i < movePath.corners.Length - 1; ++i)
            {
                if ((movePath.corners[i] - movePath.corners[i + 1]).sqrMagnitude < 2f)
                    movePath.corners[i + 1] = movePath.corners[i];
            }
        }

        return Find;
    }
}

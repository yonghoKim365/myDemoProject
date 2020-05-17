using UnityEngine;
using System.Collections;

/// <summary>
/// NavMeshAgent를 가진 객체의 위한 이동 제어
/// </summary>
/// <remarks>
/// 
/// NavMeshAgent를 가지고 있는 객체를 이용해서 이동을 제어하는 클래스이다.
/// 
/// NavMeshAgent고유 기능을 이용해서 이동하지 않고, 
/// NavMesh에 의해 생성된 경로를 수정하여 게임에 맞게 사용함.
/// 
/// </remarks>
[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentMover : MonoBehaviour
{
    public bool             IsReady { private set; get; }
    public GameObject       Owner;
    public NavMeshAgent     NavAgent;

    public bool             Enabled 
    {
        set { 
            if (null != NavAgent)
                NavAgent.enabled = value; 
        }
        get { return IsReady && NavAgent.enabled; }
    }

    public bool             UsableNavAgent { get { return NavAgent != null && NavAgent.enabled; } }    
    public Vector3          DestPosition { set; get; }

    public float            Speed
    {
        set 
        {
            if (null != NavAgent)
                NavAgent.speed = value;
            speed = value;
        }

        get 
        {
            return speed;
        }
    }

    public NavMeshPath CurMovePath { get { return movePath; } }

    protected NavMeshPath   movePath = new NavMeshPath();
    protected int           movePathIndex = 0;

    float       speed;
    Transform   ownerTrans;


    public void Init(GameObject _agentOwner)
    {
        if (null == _agentOwner)
        {
            Debug.LogWarning( GetType() + ".Init() Error" );
            return;
        }

        NavMeshAgent agent = _agentOwner.GetComponent<NavMeshAgent>();
        if (null == agent)
        {
            Debug.LogWarning( GetType() + ".Init() NavMeshAgent is not exist!!" );
            return;
        }

        Reset();

        Owner = _agentOwner;
        NavAgent = agent;
        Speed = NavAgent.speed;
        ownerTrans = _agentOwner.transform;

        IsReady = true;
    }

    public virtual void Reset()
    {
        ClearPath();

        IsReady = false;

        Owner = null;
        NavAgent = null;
        ownerTrans = null;
        DestPosition = Vector3.zero;
    }

    public void AutoMove()
    {
        if (movePath.corners.Length <= 0)
            return;

        NavAgent.SetPath( movePath );
    }
        
    /// <summary>
    /// 구해진 Path따라 자신의 속도에 맞게 일정량 이동하게 한다.
    /// </summary>
    /// <param name="speedRatio">현 속도에 배속을 설정한다.</param>
    public bool MoveToPath(float speedRatio = 1.0f)
    {
        if (!UsableNavAgent)
            return false;

        bool endPath = movePathIndex >= movePath.corners.Length;
        if (endPath)
            return false;

        Vector3 targetPos = movePath.corners[movePathIndex];
        Vector3 currentPos = ownerTrans.position;
        targetPos.y = currentPos.y = 0; // x, z값은 0이고, y만 값만 존재할때 이동문제 발생 (높낮이 맵에 의한)

        Vector3 offset = targetPos - currentPos;
        
        // 기본으로 이동해야할 힘 = 방향 * 속도
        Vector3 velocity = (Speed * speedRatio) * offset.normalized;

        float destDistance = offset.magnitude;  // 남은거리
        float velDistance = velocity.magnitude * Time.deltaTime;    // 현 프레임에서 이동해야할 거리

        // 이동
        if (destDistance <= velDistance)
        {
            // 목표지점의 마지막 정확한 이동거리
            NavAgent.velocity = velocity * ( destDistance / velDistance );
        }
        else
            NavAgent.velocity = velocity;

        // 회전
        LookAt( targetPos, Time.deltaTime * 10f * speedRatio );

        int skipIdxCnt = 1;
        if (destDistance <= velDistance)
        {
            // 다음 지점 설정을 위한 코드 : (프레임당 이동거리)보다 (다음지점)거리가 짦다면 스킵되도록 한다.
            for (skipIdxCnt = 1; movePathIndex + skipIdxCnt < movePath.corners.Length; ++skipIdxCnt)
            {
                // 포인트 무시해야함
                if (( movePath.corners[movePathIndex + skipIdxCnt] - ownerTrans.position ).magnitude > velDistance)
                    break;
            }

            movePathIndex += skipIdxCnt;
        }

        return true;
    }

    /// <summary>
    /// 해당 지점까지의 네비게이션 패스 생성 함수
    /// </summary>
    /// <returns></returns>
    public bool CalculatePath(Vector3 targetPos)
    {
        if (!UsableNavAgent)
            return false;

        ClearPath();

        // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 위치를 찾아준다.
        NavMeshHit navHit;
        if (NavMesh.SamplePosition( targetPos, out navHit, Vector3.Distance( targetPos, ownerTrans.position ), -1 ))
        {
            targetPos = navHit.position;
        }

        bool Find = NavAgent.CalculatePath( targetPos, movePath );
        if (Find)
        {
            int cornerCnt = movePath.corners.Length;
            // 시작점과 끝점은 계산에서 제외시킴
            for (int i = 1; i < cornerCnt - 1; i++)
            {
                // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
                if (NavMesh.FindClosestEdge( movePath.corners[i], out navHit, 1 ))
                {
                    if (( navHit.position - movePath.corners[i] ).sqrMagnitude < 1f)
                        movePath.corners[i] = movePath.corners[i] + navHit.normal * 1f;
                }
            }

            // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
            if (cornerCnt >= 2)
            {
                // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
                for (int i = 1; i < cornerCnt - 1; ++i)
                {
                    if (( movePath.corners[i] - movePath.corners[i + 1] ).sqrMagnitude < 2f)
                        movePath.corners[i + 1] = movePath.corners[i];
                }
            }

            DestPosition = targetPos;
        }

        return Find;
    }

    public void ClearPath()
    {
        movePath.ClearCorners();
        movePathIndex = 1;
        DestPosition = null != ownerTrans ? ownerTrans.position : Vector3.zero;

        if (null != NavAgent)
            NavAgent.ResetPath();
    }

    public void LookAt(Vector3 target, float delta = 1f)
    {
        ownerTrans.rotation = MathHelper.GetYRotation( target, ownerTrans, delta );
    }

    /// <summary>
    /// 지정한 지점으로 즉시 이동하도록 한다.
    /// </summary>
    /// <param name="newPos">즉시 이동할 위치</param>
    /// <param name="safety">NavMeshAgent가 갈수없는 곳이라면 안전한 위치로 보정유무</param>
    public void Warp(Vector3 newPos, bool safety = true)
    {
        if (safety)
        { 
            NavMeshHit navHit;
            if (NavMesh.SamplePosition( newPos, out navHit, Vector3.Distance( newPos, ownerTrans.position ), -1 ))
            {   
                NavAgent.Warp( navHit.position );
            }
        }
        else
            // NavMesh와 너무 먼 곳에 Warp시, 의도하지 않은 동작을함.
            NavAgent.Warp( newPos );
    }
        
    #region :: for Debug ::
    
    public void DrawDebug()
    {
        if (null != movePath)
        {
            for (int i = 1; i < movePath.corners.Length; ++i)
            {
                Debug.DrawLine( movePath.corners[i - 1], movePath.corners[i], Color.cyan );
                Debug.DrawLine( movePath.corners[i], movePath.corners[i] + Vector3.up * i, Color.blue );
            }
        }

        if (null != NavAgent)
        {
            Debug.DrawLine( ownerTrans.position, ownerTrans.position + NavAgent.velocity, Color.magenta );
            Debug.DrawLine( ownerTrans.position, ownerTrans.position + NavAgent.desiredVelocity, Color.red );
        }
    }

    #endregion
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RMCurve 데이터 기반으로 작동하는 RootMotion
/// </summary>
public class KRootMotionRM : MonoBehaviour 
{
    /// <summary>
    /// 실제 이동하는 객체
    /// </summary>
    public Transform mover;
    NavMeshAgent moverAgent;

    /// <summary>
    /// mover의 애니메이션 Component
    /// </summary>
    public Animation moverAnim;

    /// <summary>
    /// 애니메이션에 종속되어 움직이는 Biped Root (루트모션 핵심 Tranform)
    /// </summary>
    public Transform animatedRoot;

    public bool IsPlayingMotion { get { return doRootMotion; } }

    protected bool              doRootMotion = false;
    protected AnimationState    playingAnimState;
    protected string            playingAnimName = string.Empty;
    protected float             playingAnimLength = 0;
    protected RMCurve           playingCurve;
    protected bool              _ignoreEnemy = false;

    Vector3                     prevFramePos = Vector3.zero;            // 애니메이션에 의해 이동한 마지막 위치값 저장

    /// <summary>
    /// 애니메이션별 이동량이 담겨있는 데이터
    /// </summary>
    Dictionary<string, RMCurve> curveDic;
    
    public void Init(Dictionary<string, RMCurve> curveData, Transform _mover, Animation _animator, Transform _animRootBone)
    {
        if (null == curveData || curveData.Count == 0)
        {
            //Debug.LogWarning("RMCurve 데이터가 존재하지 않습니다. : " + transform, _mover.gameObject);
            return;
        }

        mover = _mover;
        moverAgent = mover.GetComponent<NavMeshAgent>();
        moverAnim = _animator;
        animatedRoot = _animRootBone;

        curveDic = curveData;
    }

    /// <summary>
    /// 루트모션 적용 시작 (주의! 애니메이션은 다른곳에서 플레이하던지 해야함. 여기서는 루트모션에 이동값만 적용시킴)
    /// </summary>
    /// <param name="animState">루트모션을 수행할 애니메이션</param>
    public void Play(AnimationState animState, bool rootMotion = false, bool ignoreEnemy = false)
    {
        if (null == curveDic || !curveDic.ContainsKey(animState.name))
        {
            if (rootMotion)
            {
                if(GameDefine.TestMode)
                {
                    if (null == curveDic)
                        Debug.Log("curveDic 가 없습니다");
                    else if (!curveDic.ContainsKey(animState.name))
                        Debug.Log("curveDic 에 애니가 없습니다 " + animState.name);
                }
            }
            return;
        }

        //if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        //    rootMotion = false;

        _ignoreEnemy = ignoreEnemy;
        doRootMotion = rootMotion;
        playingAnimState = animState;
        playingAnimName = animState.name;
        playingAnimLength = animState.length;
        playingCurve = curveDic[playingAnimName];
        
        prevFramePos = playingCurve.GetOffset(playingAnimState.time);
    }

    public void End()
    {
        doRootMotion = false;

        if (null != playingAnimState)
        {
            playingAnimState = null;
            playingCurve = null;
        }
    }

    void LateUpdate()
    {
        if (!doRootMotion)
            return;

        float nextAnimTime = (playingAnimState.time + (Time.deltaTime * playingAnimState.speed));

        // 다음 프레임에 애니메이션이 끝나는지 체크하기 위함.
        // 애니메이션 길이를 1이라고 봤을때, 0.9999라면, 다음 프레임은 내부적으로만 처리되기 때문에, 
        // 정확한 위치 값 적용을 위해서 마지막도 계산필요.
        if (nextAnimTime >= playingAnimLength || !playingAnimState.enabled)
        {
            End();

            return;
        }

        Vector3 newPos = playingCurve.GetOffset(playingAnimState.time);
        Vector3 deltaPos = newPos - prevFramePos;

        if (GameDefine.skillPushTest)
        {
            //if(mover.gameObject.GetComponent<Unit>().UnitType == UnitType.Unit || mover.gameObject.GetComponent<Unit>().UnitType == UnitType.Boss)
            if(!_ignoreEnemy)
            {
                Vector3 nextRealPos = mover.position + (Vector3.Dot(deltaPos, Vector3.forward) * mover.transform.forward);
                Vector3 dir = (nextRealPos - mover.position).normalized;

                //Debug.DrawRay(mover.position, dir, Color.red, 0.5f);

                RaycastHit hit;
                LayerMask mask = 1 << LayerMask.NameToLayer("Unit");

                if (Physics.Raycast(mover.position, dir, out hit, 1f, mask))
                //if (Physics.Raycast(mover.position, dir, out hit, 0.5f))
                {
                    //Debug.Log(hit);
                    doRootMotion = false;
                    return;
                }
            }            
        }

        // 프레임당 이동값을 더해준다.
        if (null != moverAgent && moverAgent.enabled)
            moverAgent.Move((Vector3.Dot(deltaPos, Vector3.forward) * mover.transform.forward));
        else
            mover.transform.localPosition += (Vector3.Dot(deltaPos, Vector3.forward) * mover.transform.forward);

        // 현재 로컬 위치를 저장해둔다.
        prevFramePos = newPos;

        // 로컬 위치 초기화
        // TODO : die애니메이션의 x, z값 변동이 커서 눈에 띄어서 일단 주석.
        //animatedRoot.localPosition = new Vector3( 0, animatedRoot.localPosition.y, 0 );
    }

    /// <summary> 루트모션으로 이동할 총거리를 구해준다. </summary>
    public Vector3 CalcTotalMovingDistance(string targetAnimName)
    {
        if (!curveDic.ContainsKey(targetAnimName))
            return Vector3.zero;

        return curveDic[targetAnimName].CalcTotalDistance();
    }
}

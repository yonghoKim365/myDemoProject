using UnityEngine;
using System.Collections;

public class KRootMotion : MonoBehaviour 
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
    /// 애니메이션에 종속되어 움직이는 Biped Root
    /// </summary>
    public Transform animatedRoot;

    public bool IsPlayingMotion { get { return doRootMotion; } }

    protected bool              doRootMotion = false;
    protected AnimationState    playingAnimState;
    protected string            playingAnimName = string.Empty;
    protected float             playingAnimLength = 0;

    Vector3     prevFramePos = Vector3.zero;            // 애니메이션에 의해 이동한 마지막 위치값 저장
    //Quaternion  prevFrameRot = Quaternion.identity;  // 애니메이션에 의해 이동한 마지막 회전값 저장

    public void Init(Transform _mover, Animation _animator, Transform _animRootBone)
    {
        mover = _mover;
        moverAgent = mover.GetComponent<NavMeshAgent>();
        moverAnim = _animator;
        animatedRoot = _animRootBone;
    }

    /// <summary>
    /// 루트모션 적용 시작 (주의! 애니메이션은 다른곳에서 플레이하던지 해야함. 여기서는 루트모션에 이동값만 적용시킴)
    /// </summary>
    /// <param name="animState">루트모션을 수행할 애니메이션</param>
    public void Play(AnimationState animState, bool rootMotion = false)
    {
        doRootMotion = rootMotion;
        playingAnimState = animState;
        playingAnimName = animState.name;
        playingAnimLength = animState.length;
        
        prevFramePos = animatedRoot.localPosition;
        //prevFrameRot = animatedRoot.localRotation;
        
        //if (mover.GetComponent<Pc>())
        //    Debug.Log("RootMotionPlay : [" + doRootMotion + "] : " +  animState.name + " : [AnimState] : " + animState.enabled + " [AnimTime] : " + animState.time + " : " + animState.normalizedTime + " : [LocalPos] : " + prevFramePos.ToString( "f4" ) );
    }

    public void End()
    {
        doRootMotion = false;

        if (null != playingAnimState)
        { 
            playingAnimState.time = playingAnimLength;
            moverAnim.Sample();
            playingAnimState.enabled = false;
            playingAnimState.normalizedTime = 0f;
        }

        //if (mover.GetComponent<Pc>())
        //    Debug.Log("RootMotionEnd : " +  playingAnimState.name + " : [AnimState] : " + playingAnimState.enabled + " [AnimTime] : " + playingAnimState.time + " : " + playingAnimState.normalizedTime + " : [LocalPos] : " + prevFramePos.ToString( "f4" ) );
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
        }
        
        Vector3 newPos = animatedRoot.localPosition;
        //Quaternion newRot = animatedRoot.localRotation;

        Vector3 deltaPos = newPos - prevFramePos;

        // 프레임당 이동값을 더해준다.
        if (moverAgent)
            moverAgent.Move( (Vector3.Dot( deltaPos, Vector3.forward ) * mover.transform.forward) );
        else
            mover.transform.localPosition += (Vector3.Dot( deltaPos, Vector3.forward ) * mover.transform.forward);
        //mover.transform.localRotation = newRot * savedLocalRot;
        
        // 현재 로컬 위치를 저장해둔다.
        prevFramePos = animatedRoot.localPosition;
        //prevFrameRot = animatedRoot.localRotation;
        
        // 로컬 위치 초기화
        animatedRoot.localPosition = new Vector3( 0, prevFramePos.y, 0 );
        //animatedRoot.localRotation = Quaternion.identity;
    }
}

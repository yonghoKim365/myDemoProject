using UnityEngine;
using System.Collections;

public class DirectionIndicator : MonoBehaviour 
{
    enum TargetType
    {
        NONE,
        POSITION,
        TRANSFORM,
    }

    protected Transform   Parent;
	protected GameObject  EffectObj;

    private Vector3     TargetPos;
    private Transform   TargetTrans;
    private TargetType  targetType = TargetType.NONE;

    private System.Action OnUpdate;
    private float UpdateInterval = 0.15f;
    private float LastUpdateT = 0;

    public DirectionIndicator Init(GameObject parent)
    {
        if (null == parent)
        {
            Debug.LogError( "you must have Parent!" );
            return this;
        }

        Parent = parent.transform;

        CreateEffect( parent.transform );

        Hide();

        return this;
    }

    void OnDestroy()
    {
        if (null != EffectObj)
            Destroy( EffectObj );
    }

    public void SetOnUpdate(System.Action func, float interval = 0.15f)
    {
        OnUpdate = func;
        UpdateInterval = interval;
    }

    /// <summary>
    /// 정해진 위치를 바라보도록 한다.
    /// </summary>
    public void Show(Vector3 target)
    {
        TargetPos = target;
        targetType = TargetType.POSITION;
        if (null != EffectObj)
            EffectObj.SetActive( true );
    }

    public void Show(Transform target)
    {
        Show( Vector3.zero );

        TargetTrans = target;
        targetType = TargetType.TRANSFORM;        
    }

    public void Hide()
    {
        if (null != EffectObj)
            EffectObj.SetActive( false );

        targetType = TargetType.NONE;
    }
    
    void LateUpdate()
    {
        if (Time.time > LastUpdateT + UpdateInterval)
        {
            if (null != OnUpdate)
                OnUpdate();

            LastUpdateT = Time.time;
        }

        if (TargetType.NONE == targetType || EffectObj == null)
            return;

        Vector3 nextPos;
        switch (targetType)
        {
            case TargetType.POSITION:
                nextPos = TargetPos;
                break;
            case TargetType.TRANSFORM:
                if (TargetTrans == null)
                {
                    Hide();
                    return;
                }

                nextPos = TargetTrans.position;
                break;

            default:
                nextPos = Parent.position + Parent.forward;
                break;
        }

        Vector3 dir = nextPos - transform.position;
        EffectObj.transform.localRotation = Quaternion.Euler( 
            0,
            SignedAngle( Parent.forward, dir.normalized ),
            0 );

        // 타겟위치에 일정거리내로 들어가면 끄기.
        if (dir.sqrMagnitude < 3f)
            Hide();
    }

    float SignedAngle(Vector3 a, Vector3 b)
    {
        float angle = Vector3.Angle( a, b );

        return angle * Mathf.Sign( Vector3.Cross( a, b ).y );
    }

    void CreateEffect(Transform parent)
    {
        //AssetbundleLoader.GetEffect("Fx_exit_direction_01", (effect) => 
        //{
        //    EffectObj = Instantiate(effect) as GameObject;
        //    EffectObj.transform.AttachTo(parent, new Vector3(0f, 0.02f, 0f), Vector3.one, Quaternion.identity);
        //});
    }
}

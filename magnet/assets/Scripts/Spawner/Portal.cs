using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Portal : SpawnGroup
{
    /*
    /// <summary>
    /// 이동될 지점
    /// </summary>
    public Transform destination;
    
    public ParticleSystem beginEffect;

    public bool destroyAfterMove = true;

    bool canTeleport;

    public override void Ready(SpawnController parent)
    {
        base.Ready(parent);
        
        if (null != beginEffect)
            beginEffect.Stop( true );

        canTeleport = false;
    }
    
    protected override void Execute()
    {
        if (null != beginEffect)
            beginEffect.Play( true );

        StartCoroutine( WaitFor(1.5f) );
    }

    IEnumerator WaitFor(float delay)
    {
        yield return new WaitForSeconds( delay );

        canTeleport = true;
    }
    */

    private bool IsEnd=false;

    /// <summary>
    /// Agent가 포탈에 접촉했다면, 조건 체크후 다음 지점으로 이동시켜준다.
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter(Collider other)
    {
        //GoDestination();
        if( !IsEnd )
            OnEnter(other);
    }

    protected void OnTriggerStay(Collider other)
    {
        //GoDestination();
        if (!IsEnd)
            OnEnter(other);
    }

    void OnEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Unit"))
            return;

        Pc pc = other.gameObject.GetComponent<Pc>();
        if (pc == null)
            return;
        
        IsEnd = true;
        if(G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
        {
            (G_GameInfo.GameInfo as TutorialGameInfo).EndTutorial();
        }
    }
    /*
    void GoDestination()
    {
        if (null == destination || canTeleport == false)
            return;

        // 의존성 없애기 위해 SendMessage로 처리함.
        if (null != mainAgent)
            mainAgent.SendMessage( "Teleport", destination.position );

        if (destroyAfterMove)
            Destroy( gameObject );

        canTeleport = false;
    }
    */
    protected override void ClearComponent()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (null != collider)
        {
            Gizmos.DrawWireCube( collider.bounds.center, collider.bounds.size );
        }
        Gizmos.color = Color.white;
    }
}

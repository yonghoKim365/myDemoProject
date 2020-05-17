using UnityEngine;
using System.Collections;
using PathologicalGames;

public class SpawnWall : SpawnGroup
{
    public Animation animator;
    public AnimationClip animOpen;
    public AnimationClip animClose;
    
    //float animDuration;
    public string fxEffectName;

    public override void Init(GameObject _mainAgent, bool preload = false)
    {
        base.Init( _mainAgent, preload );

        fxEffectName = "Fx_" + animator.name;

    }

    protected override void ClearComponent()
    {
    }

    void Update()
    {
        if (OpenCheck)
            return;

        if (MainUnit == null)
            return;

        //if (MainUnit.CurrentState == UnitState.Evasion)
        //    MainUnit.ChangeState(UnitState.Idle, true);
    }

    protected override void OnEvent(object obj)
    {
        SpawnEventArgs args = obj as SpawnEventArgs;
        if (null == args)
        {
            Debug.LogWarning( "OnEvent : 이 메시지가 호출되면 인자로 잘못된 값을 보낸것입니다." );
            return;
        }

        switch (args.eventType)
        {
            case eEvent.AllSpawned:
                {   
                    // 다음 그룹의 스폰이 끝났다면, 못돌아가도록 막기
                    if (args.sender == nextGroup)
                    {
                        //Close();
                        //Invoke("Close", 0.2f);
                    }                    
                }
                break;

            case eEvent.AllDead:
            case eEvent.OpenDoor:
                {
                    // 전 그룹의 객체가 다 죽었다면 벽을 없애준다.
                    if (args.sender == prevGroup)
                    {
                        Open();
                    }
                }
                break;
        }
    }

    Unit MainUnit;
    void OnTriggerEnter(Collider other)
    {
        if (OpenCheck)
            return;

        if (mainAgent == null)
            return;

        if (mainAgent != null && other.gameObject != mainAgent)
            return;

        if(MainUnit == null || MainUnit.gameObject != mainAgent)
            MainUnit = mainAgent.GetComponent<Unit>();
    }

    void OnTriggerExit(Collider other)
    {
        if (MainUnit != null && MainUnit.gameObject == other.gameObject)
            MainUnit = null;
    }

    bool OpenCheck = false;
    void Open()
    {
        OpenCheck = true;
        animator.Play( animOpen.name );

        SetObstacles( false );

        SendPrevAndNext( eEvent.OpenDoor );
    }

    void OpenAni()
    {
        OpenCheck = true;
        animator.Play(animOpen.name);

        SetObstacles(false);
    }

    void Close()
    { 
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive( true );
        }

        animator.Play( animClose.name );

        SetObstacles( true );

        SendPrevAndNext( eEvent.CloseDoor );
    }

    void CloseAni()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        animator.Play(animClose.name);
        SetObstacles(true);
    }

    void SetObstacles(bool _active)
    {
        NavMeshObstacle[] obstacles = GetComponentsInChildren<NavMeshObstacle>();
        if (null == obstacles)
            return;

        for (int i = 0; i < obstacles.Length; i++)
            obstacles[i].enabled = _active;
    }
}

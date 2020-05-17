using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEventArgs
{
    public SpawnGroup sender;
    public SpawnGroup.eEvent eventType;
}

public class SpawnGroup : MonoBehaviour
{
    //< 현재 활성화된 그룹을 저장하기위함
    public static GameObject ActiveSpawnGroup;

    public enum eSpawnTiming
    {
        Immediately,
        TriggerEnter,
        PrevAllSpawned,
        PrevAllDead,

        /// <summary>
        /// 스폰타이밍이 따로 없는 상태
        /// </summary>
        Manual,
    }

    public enum eEvent
    {
        Spawned,    
        Dead,
        AllSpawned, // 모두 스폰했다
        AllDead,    // 스폰된게 모두 죽었다
        OpenDoor,
        CloseDoor,
    }

    public int              groupNo;    
    public eSpawnTiming     spawnTiming;
    protected SpawnGroup    prevGroup;
    public SpawnGroup       nextGroup;

    public List<ISpawner>   spawnerList = new List<ISpawner>();
    public List<NPCSpawner> npcspawnerList = new List<NPCSpawner>();
    public InGameTutorialType TutorialType = InGameTutorialType.None;
    
/// <summary>
/// 이벤트에 감지되는 주체
/// </summary>
protected GameObject mainAgent;

    private int deadCount = 0;

	// 몬스터 소환 확률. 
	// 기존 npc들을 모두 0 이상으로 수정해줘야하는 불편함이 있어 100-n = 확률이 되도록 함.
	public int reverseProbability;
	public float DelayTime;

	int numOfNpcsIncludeRegen;

    public virtual void Ready(SpawnController parent)
    {
        // 충돌 박스를 사용한다면, InputManager에 의한 Raycast는 무시되도록 하기 위함.
        gameObject.layer = LayerMask.NameToLayer( "Ignore Raycast" );

        spawnerList.Clear();
        foreach (Transform trans in GetComponentsInChildren<Transform>())
        {
            ISpawner spawner = (ISpawner)trans.GetComponent( "ISpawner" );
            if (null != spawner)
            {
                //테스트용으로 프랍은 뺀다
                //if (spawner is PropSpawner) continue;

                spawnerList.Add( spawner );

                if (spawner is PropSpawner)
                    parent.AddProp();
            }
        }

        // 링크 연결을 위함
        if (null != nextGroup)
            nextGroup.SetPrev( this );
    }

    public virtual void Init(GameObject _mainAgent, bool preload = false)
    {
        mainAgent = _mainAgent;

        foreach (ISpawner spawner in spawnerList)
        {
            if (null != spawner)
                spawner.Preload();

            if (spawner is NPCSpawner)
                npcspawnerList.Add((spawner as NPCSpawner));
        }

        if (spawnTiming == eSpawnTiming.Immediately)
            Execute();
        else if (spawnTiming == eSpawnTiming.TriggerEnter)
        {
            if (null == collider)
                Debug.LogError( "충돌체를 가지고 있어야만 합니다." );
        }
        else
        {
            ClearComponent();
        }

		numOfNpcsIncludeRegen = NumOfNpcsInSpawnGroupIncludeRegen ();
    }

    public void ChangeAgent(GameObject _mainAgent)
    {
        if (null == _mainAgent)
            return;

        mainAgent = _mainAgent;
    }

    protected void SetPrev(SpawnGroup group)
    {
        prevGroup = group;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != mainAgent ||
            spawnTiming != eSpawnTiming.TriggerEnter)
            return;

        if(G_GameInfo.GameMode == GAME_MODE.TUTORIAL && TutorialType != InGameTutorialType.None)
        {
            //if(groupNo == 7)
            //{
            //    (G_GameInfo.GameInfo as TutorialGameInfo).OnTutorial(InGameTutorialType.Auto);
            //}
            //else if(groupNo == 1)
            //{
            //    (G_GameInfo.GameInfo as TutorialGameInfo).OnTutorial(InGameTutorialType.Attack);
            //}
            
            (G_GameInfo.GameInfo as TutorialGameInfo).OnTutorial(TutorialType);
        
            //if ((G_GameInfo.GameInfo as TutorialGameInfo).TutorialUI.CurInGameTuto == InGameTutorialType.Joystick)
            //(G_GameInfo.GameInfo as TutorialGameInfo).OnTutorial(InGameTutorialType.Attack);
        }

        Execute();
    }

    /// <summary>
    /// SendMessage에 의해 호출될 이벤트 Receiver
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void OnEvent(object obj)
    {   
        SpawnEventArgs args = obj as SpawnEventArgs;
        if (null == args)
        {
            Debug.LogWarning( "OnEvent : 이 메시지가 호출되면 인자로 잘못된 값을 보낸것입니다." );
            return;
        }

        //Debug.LogWarning( "OnEvent : " + this + " : " + args.sender + " : " + args.eventType );
        
        switch (args.eventType)
        {
            case eEvent.AllSpawned:
                {
                    // 이전그룹에서 모두 Spawn이 완료되었다면,
                    if (null != prevGroup && args.sender == prevGroup && spawnTiming == eSpawnTiming.PrevAllSpawned)
                    {
                        Execute();
                    }
                }
                break;

            case eEvent.Dead:
                {
                    if (args.sender == this)
                    { 
                        ++deadCount;

                        //if (deadCount >= spawnerList.Count)
 						if (deadCount >= numOfNpcsIncludeRegen)
                        {
                            SendPrevAndNext( eEvent.AllDead );
                        }
                    }
                }
                break;

            case eEvent.AllDead:
                {
                    if(G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
                    {
                        (G_GameInfo.GameInfo as TutorialGameInfo).EndZone();
                    }
					// 안들어오는듯.
                    // 이전그룹의 Spawn된 객체가 모두 죽었다면,
                    if (null != prevGroup && args.sender == prevGroup && spawnTiming == eSpawnTiming.PrevAllDead)
                    {
                        Execute();
                    }
                }
                break;

            case eEvent.OpenDoor:

                if (TutorialType != InGameTutorialType.None && G_GameInfo.GameMode == GAME_MODE.TUTORIAL && G_GameInfo.GameInfo is TutorialGameInfo)
                {
                    (G_GameInfo.GameInfo as TutorialGameInfo).EndZone();
                }

                break;
        }
    }

	int NumOfNpcsInSpawnGroupIncludeRegen(){
		int n = spawnerList.Count;

		for (int i = 0; i < npcspawnerList.Count; i++)
		{
			if (npcspawnerList[i] == null)
				continue;
			
			if (npcspawnerList[i].RegenLive){
				n += npcspawnerList[i].OriginRegenCount;
			}
		}

		return n;

	}
    protected virtual void Execute()
    {
        if (SpawnActive)
            return;

        SpawnActive = true;
        StartCoroutine("SpawnRoutine");
    }

    //< 생성한 유닛이 모두 살아있는지 검사
    public bool AllUnitLive()
    {
        if (!SpawnActive)
            return true;

        for (int i = 0; i < npcspawnerList.Count; i++)
        {
            if (npcspawnerList[i] == null)
                continue;

            if (npcspawnerList[i].UnitLive())
                return true;
        }

        return false;
    }

	public bool IsUnitLive(int nUnitNum){
		for (int i = 0; i < npcspawnerList.Count; i++)
		{
			if (npcspawnerList[i] == null)
				continue;

			if (npcspawnerList[i].UnitNum == nUnitNum){
				if (npcspawnerList[i].UnitLive()){
					return true;
				}
 				else return false;
			}
		}

		return false;
	}

    bool SpawnActive = false;
    IEnumerator SpawnRoutine()
    {
		if (reverseProbability > 100)
			reverseProbability = 100;

		int probability = 100 - reverseProbability;
#if UNITY_EDITOR
		if (probability == 0) {
			Debug.LogError("probability of SpawnGroup is zero!!. npc will never spawn");
		}
#endif
		if (Random.Range (1, 100) <= probability) {

			yield return new WaitForSeconds (DelayTime);

			// 현 그룹에 존재하는 Spawner들을 실행시킨다.
			for (int i = 0; i < spawnerList.Count; i++) {
				////테스트로 1마리만 띄워보자
				//if (i > 0) break;

				if (null != spawnerList [i])
					spawnerList [i].Spawn ();

				yield return null;
			}

			SendPrevAndNext (eEvent.AllSpawned);

		} else {
			SendPrevAndNext (eEvent.AllDead);
		}

        ClearComponent();
    }

    protected virtual void ClearComponent()
    {
        // 필요없는 객체 제거
        Destroy( GetComponent<Collider>() );
    }

    protected void SendPrevAndNext(eEvent evt)
    {
        if (null != prevGroup)
            prevGroup.SendMessage( "OnEvent", new SpawnEventArgs() { sender = this, eventType = evt}, SendMessageOptions.DontRequireReceiver );
        if (null != nextGroup)
            nextGroup.SendMessage( "OnEvent", new SpawnEventArgs() { sender = this, eventType = evt}, SendMessageOptions.DontRequireReceiver );
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube( transform.position, Vector3.one );
        Gizmos.color = Color.red;
        if (null != collider)
        {
            Gizmos.DrawWireCube( collider.bounds.center, collider.bounds.size );
        }
        Gizmos.color = Color.white;

		GUIStyle labelStyle = new GUIStyle (UnityEditor.EditorStyles.label);
		if (reverseProbability > 0) {
			labelStyle.fontSize = 20;
			labelStyle.normal.textColor = Color.black;
			//UnityEditor.Handles.Label (transform.position + Vector3.up, (100 - reverseProbability).ToString () + "%", labelStyle);
			if (collider != null){
				UnityEditor.Handles.Label (collider.bounds.center, (100 - reverseProbability).ToString () + "%", labelStyle);
			}
		}
    }
#endif

    /// <summary> 메뉴얼용 스폰은 없는거 같아서 추가 </summary>
    public void MenualSpawn()
    {
        Execute();
    }

	public void setActiveSpawnersExceptBoss(bool _bActive){
		for (int i = 0; i < npcspawnerList.Count; i++)
		{
			if (npcspawnerList[i] == null)
				continue;

			if (npcspawnerList[i].spawnedGO == null)
				continue;
			
			if (npcspawnerList[i].isBoss)
				continue;
				
			npcspawnerList[i].spawnedGO.gameObject.SetActive(_bActive);
		}
	}


}

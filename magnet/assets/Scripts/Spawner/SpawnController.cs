using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnController : MonoBehaviour 
{
    public eDifficulty difficulty = eDifficulty.Normal;

    public GameObject MainAgent;

    public GameObject NextWall=null;
    
    List<SpawnGroup> groupList = new List<SpawnGroup>();
    public List<SpawnGroup> NextGroups = new List<SpawnGroup>();

    /// <summary>
    /// Spawn System 초기화
    /// </summary>
    /// <param name="mainAgent">게임내 주 이동체 </param>
    public void Init(GameObject mainAgent)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        MainAgent = mainAgent;

        SpawnGroup[] groups = GameObject.FindObjectsOfType<SpawnGroup>();
        foreach (SpawnGroup group in groups)
        { 
            group.Ready(this);
            AddGroup( group );
        }
    }

    public int PropCount = 0;
    public void AddProp()
    {
        PropCount++;
    }

    float NextCheckTime = float.MaxValue;
    void Update()
    {
        if(G_GameInfo._GameInfo != null &&  G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            NextCheckTime -= Time.deltaTime;
            if (NextCheckTime <= 0)
            {
                NextCheckTime = 0.5f;

                if (!NextGroups[0].AllUnitLive())
                {
                    //< 다음 그룹으로 설정해준다.
                    NextGroups.RemoveAt(0);
                    SetNextSpawnWall();
                    if (NextGroups.Count == 0)
                    {
                        SpawnGroup.ActiveSpawnGroup = null;
                        NextCheckTime = float.MaxValue;
                    }
                    else
                    {
                        //< 그룹 자체는 위치가 이상하게 박혀있을수 있기때문에 배치된 스포너 위치로 대입한다.
                        for (int i = 0; i < NextGroups[0].npcspawnerList.Count; i++)
                        {
                            if (NextGroups[0].npcspawnerList[i] != null)
                            {
                                SpawnGroup.ActiveSpawnGroup = NextGroups[0].npcspawnerList[i].gameObject;
                                break;
                            }
                        }
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// Spawn System이 작동하도록 한다.
    /// </summary>
    /// <param name="preload"></param>
    public void StartController(bool preload = true)
    {
        //Debug.LogWarning("2JW : SpawnController in StartContorller(" + preload + ")");
        foreach (SpawnGroup group in groupList)
        { 
            group.Init( MainAgent, preload );
        }

        //< 첫번째 그룹을 대상으로 설정해준다
        Dictionary<int, SpawnGroup> SaveGroup = new Dictionary<int, SpawnGroup>();
        foreach (SpawnGroup group in groupList)
        {
            if (group.spawnTiming == SpawnGroup.eSpawnTiming.TriggerEnter)
            {
                int idx = 0;
                if (int.TryParse(group.name.Replace("NpcGroup", ""), out idx))
                {
                    group.gameObject.name = idx.ToString();
                    NextGroups.Add(group);
                }
            }
        }

        if (NextGroups.Count > 0)
        {
            //< 정렬
            NextGroups.Sort(delegate(SpawnGroup tmp1, SpawnGroup tmp2) { return int.Parse(tmp1.gameObject.name).CompareTo(int.Parse(tmp2.gameObject.name)); });

            SpawnGroup.ActiveSpawnGroup = NextGroups[0].gameObject;
            NextCheckTime = 5;
        }

        SetNextSpawnWall();
    }

    public void AddGroup(SpawnGroup group)
    {
        if (null == group)
            return;        
        groupList.Add( group );
    }

    /// <summary>
    /// 이벤트 발생 주체 변경하기.
    /// </summary>
    public void ChangeAgent(GameObject mainAgent)
    {
        if (null == mainAgent)
            return;

        MainAgent = mainAgent;

        foreach (SpawnGroup group in groupList)
            group.ChangeAgent( mainAgent );
    }

    public List<T> GetList<T>() where T : SpawnGroup
    {
        List<T> list = new List<T>();

        foreach(SpawnGroup group in groupList)
        {
            if (group is T)
                list.Add( group as T );
        }

        return list;
    }

    public SpawnGroup GetReadySpawner()
    {
        for(int i=0; i<groupList.Count; i++)
        {
            if(groupList[i].enabled)
                return groupList[i];
        }

        return null;
    }
    /// <summary> 메뉴얼용 스폰. </summary>
    public bool MenualSpawnGroup(int groupNum)
    {
        bool isSpawn = false;
        for (int i = 0; i < groupList.Count; i++)
        {
            if (groupList[i].groupNo != groupNum)
                continue;

            groupList[i].MenualSpawn();
            isSpawn = true;
        }

        return isSpawn;
    }

	public void setNpcActiveExceptBoss(bool _bActive){
        if(0 < NextGroups.Count)
		    NextGroups [0].setActiveSpawnersExceptBoss (_bActive);
	}

    public void SetNextSpawnWall()
    {
        if (0 < NextGroups.Count)
        {
            for (int i = 0; i < NextGroups.Count; i++)
            {
                if (NextGroups[i].nextGroup is SpawnWall)
                {
                    NextWall = NextGroups[i].nextGroup.gameObject;
                    return;
                }
            }
        }

        NextWall = null;
    }

    //public Vector3 GetNextSpawnWallPos()
    //{
    //    if (0 < NextGroups.Count)
    //    {
    //        for(int i=0;i<NextGroups.Count;i++ )
    //        {
    //            if(NextGroups[i].nextGroup is SpawnWall)
    //            {
    //                return NextGroups[i].nextGroup.gameObject.transform.position;
    //            }
    //        }
    //    }

    //    return Vector3.zero;
    //}
}

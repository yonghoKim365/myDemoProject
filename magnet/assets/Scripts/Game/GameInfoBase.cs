using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using System.Linq;

public enum eTeamType
{
    Min = 0,
    Team1 = Min,    // 팀 1
    Team2 = 1,      // 팀 2
    AllEnemy = 2,   // 모두에게 적
    AllAlly = 3,    // 모둑에게 아군
    Max = 4,
}

public class GameInfoBase : MonoBehaviour
{
    public bool GameLive = false;
    public bool IsStartGame;//게임 시작했는지

    ///  현재 게임 모드
    protected GAME_MODE _GAME_MODE = GAME_MODE.NONE;  //< 외부에서 사용할수도있기때문에 따로 제어가능하도록 함
    public virtual GAME_MODE GameMode { set { _GAME_MODE = value; } get { return _GAME_MODE; } }

    //< 자동 스킬 사용인지 체크
    public bool AutoSkillMode = true;

    /// 네비게이션 메시에서 이동을 시작할 시작 -> 끝점 저장
    public class NavigationPosGroup
    {
        public Transform start;
        public Transform end;
    }

    public float ChangeLeaderDelay;

    public List<SpawnGroup> spawnGroup;
    List<NavigationPosGroup> navPosGroups;
    int navGroupIndex;

    Transform team1StartTrans;
    Transform team2StartTrans;

    public CharacterMgr characterMgr = null;
    public SpawnController spawnCtlr = null;
    public InGameBoardPanel BoardPanel = null;
    public UnitTouchController UnitCtlr = null;

    // PoolManage
    public SpawnPool effectPool = null;
    public SpawnPool projectilePool = null;
    public SpawnPool OrgprojectilePool = null;
    public SpawnPool InGameObjPool = null;
    
    protected bool loadedPool = false;
    private bool _IsStopDamage;

    public float PlayTime = 0;
    public float TimeLimit;

    bool isAutoMode;
    bool _isEnd = false;
    public bool isEnd
    {
        get { return _isEnd; }
        set
        {
            _isEnd = value;

            //< 게임이 끝났을경우에는 타임 업데이트도 막는다
            NotTimeUpdate = _isEnd;
        }
    }

    public bool IsStopDamage {
        set {
            _IsStopDamage = value;
        }
        get {
            return IsStopDamage;
        }
    }

    public System.Action LoadingCompleted;

    public FocusingCamera FocusingCam;
    public GameEndEventPanel _GameEndEventPanel;//자식에서도 사용하자 

    //< 현재 Single, Raid에서 같이 사용하기때문에 여기에 넣어둠
    public InGameHUDPanel HudPanel = null;

    public static bool NotTimeUpdate = true;   //< 타임 업데이트를 하지않아야할때 사용
    public bool IsEndForced;//강제종료

    public bool IsEndForcedMap = false; //맵으로 강제종료인가 ? ?
    /// 자동 전투 모드 유무
    public bool AutoMode
    {
        set
        {
            isAutoMode = value;
            PlayerPrefs.SetInt(GameDefine.GameName + "_" + GameMode.ToString() + GameDefine.AutoMode_Suffix, isAutoMode ? 1 : 0);
        }

        get { return isAutoMode; }
    }

    //< 적군의 자동전투 유무
    public bool EnemyAutoMode = true;

    /// 내 유저 관리용
    public PlayerController playerCtrl = null;
    public void SetPlayerCtrl(PlayerController ctrl) { playerCtrl = ctrl; }

    //전투관련 업적데이터 
    public class AchieveFightData
    {
        public uint MaxCombo = 0;//최대연타
        public uint MaxDamaga = 0;//최대뎀지
        public uint SkillCount = 0;//스킬사용횟수
        public uint KillMonsterCount = 0;   //일반본스터 킬수
        public uint KillBossCount = 0;  //보스몬스터킬수
        public uint killPkCount = 0;    //다른유저킬수
        public uint DieCount = 0;   //사망횟수
        public uint ReviveCount = 0;    //부활횟수
        public uint parterSkillCnt = 0;    //파트너스킬횟수
    }
    public AchieveFightData _AchieveFightData = new AchieveFightData();

    // 게임시작할때 전부리셋해줌
    public void ResetAchieveFightData()
    {
        _AchieveFightData.MaxCombo = 0;
        _AchieveFightData.MaxDamaga = 0;
        _AchieveFightData.SkillCount = 0;
        _AchieveFightData.KillMonsterCount = 0;
        _AchieveFightData.KillBossCount = 0;
        _AchieveFightData.killPkCount = 0;
        _AchieveFightData.DieCount = 0;
        _AchieveFightData.ReviveCount = 0;
        _AchieveFightData.parterSkillCnt = 0;
    }

    public void SendAchieveFightData()
    {
        NetworkClient.instance.SendPMsgAchieveSynFightDataTotalValueC(
            _AchieveFightData.MaxCombo,
            _AchieveFightData.MaxDamaga,
            _AchieveFightData.SkillCount,
            _AchieveFightData.KillMonsterCount,
            _AchieveFightData.KillBossCount,
            _AchieveFightData.killPkCount,
            _AchieveFightData.DieCount,
            _AchieveFightData.ReviveCount,  
            _AchieveFightData.parterSkillCnt

            );
    }


    #region //<========== GameInfoBase ==========>//

    #region 데이터로딩부분 (안씀)
    //로딩데이터
    //public List<System.Action> LoadingData = new List<System.Action>();
    //public void LoadLoadingData()
    //{
    //    LoadingData.Add(InitDatas);
    //    LoadingData.Add(InitManagers);
    //    LoadingData.Add(InitUI);

    //    StartCoroutine(LoadingDataUpdate());
    //}

    //public bool IsEndLoad;

    //IEnumerator LoadingDataUpdate()
    //{
    //    IsEndLoad = false;

    //    int count = 0;
    //    while (true)
    //    {
    //        float value = (float)(count + 1) / (float)LoadingData.Count;

    //        LoadingData[count]();

    //        yield return new WaitForSeconds(0.1f);

    //        if (SceneManager.instance.IsShowLoadingPanel())
    //            SceneManager.instance.LoadingTipPanel.changeLoadingBar(value);


    //        if (value < 0.9f)
    //        {
    //            yield return new WaitForSeconds(0.1f);
    //        }


    //        Debug.Log(string.Format("value {0}", value));

    //        yield return null;
    //        count++;

    //        if (count >= LoadingData.Count)
    //            break;
    //    }

    //    if (GameMode == GAME_MODE.FREEFIGHT)
    //        FreeFightGameState.StateStart();

    //    yield return new WaitForSeconds(0.1f);
    //    IsEndLoad = true;
    //    SceneManager.instance.ShowLoadingTipPanel(false);

    //    yield return null;
    //}
    #endregion


    //< 외부에서 게임인포를 사용할때 처리
    public bool NotInGame = false;
    protected virtual void Start()
    {
        IsStartGame = false;

        // 다른 게임이 이미 플레이중인데, 다시 시도할 경우 에러
        if (null == G_GameInfo.GameInfo)
            return;

        if (!PoolManager.Pools.ContainsKey("Effect") || null == GameObject.Find("InGamePoolMgr"))
            ResourceMgr.Instantiate<GameObject>("GameInfo/InGamePoolMgr");

        effectPool = PoolManager.Pools["Effect"];
        projectilePool = PoolManager.Pools["Projectile"];
        OrgprojectilePool = PoolManager.Pools["Orgprojectile"];
        InGameObjPool = PoolManager.Pools["InGameObj"];

        InitDatas();
        InitManagers();
        InitUI();

        ShaderHelper.InitMapPostProcess(LayerMask.NameToLayer("Obstacle"), "Custom/DiffuseAlpha", "_Texture");

        //< 로딩이 완료되었을때 호출하기위함
        LoadingCompleted += CallLoadingCompleted;

        if (rtsCamera == null)
            rtsCamera = CameraManager.instance.RtsCamera;

        if (!NotInGame)
        {
            //< 카메라 연결
            UIHelper.SetMainCameraActive(true);

            _GameEndEventPanel = ResourceMgr.InstAndGetComponent<GameEndEventPanel>("UI/UIPanel/GameEndEventPanel");
            _GameEndEventPanel.SetTargetCamera(Camera.main);

            //< 스킬 이벤트 매니져 생성
            if (GameMode == GAME_MODE.SINGLE || GameMode == GAME_MODE.RAID || GameMode == GAME_MODE.SPECIAL_EXP || GameMode == GAME_MODE.SPECIAL_GOLD || GameMode == GAME_MODE.TOWER)// || SimulationGameInfo.SimulationGameCheck)
            {
                FocusingCam = ResourceMgr.InstAndGetComponent<FocusingCamera>("Camera/FocusingCamera");
                FocusingCam.SetTargetCamera(Camera.main);

                SkillEventMgr.instance.Setup(rtsCamera, FocusingCam);
            }
        }

        ResetAchieveFightData(); //게임시작전 데이터초기화해줌

    }


    bool NotUpdate = false;
    protected virtual void Update()
    {
	   if (NotUpdate || _isEnd)
		  return;

	   if (!SkillEventMgr.ActiveEvent && !NotInGame)
		  CameraUpdate();

        if (!NotTimeUpdate && GameMode != GAME_MODE.FREEFIGHT)
            PlayTimeUpdate();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
        {
            List<Unit> allUnits = characterMgr.allUnitList;
            //foreach (Unit unit in allUnits)
            for(int i=0; i < allUnits.Count; i++)
            {
                Unit unit = allUnits[i];
                if (null == unit || unit.IsPartner || unit.IsLeader || !unit.Usable )
                    continue;

                unit.CharInfo.Hp = 0;// (G_GameInfo.PlayerController.Leader, 0);
                unit.Die(G_GameInfo.PlayerController.Leader);//SetHp(G_GameInfo.PlayerController.Leader, -1);
                //unit.ChangeState(UnitState.Idle);
                //unit.SetTarget(GameDefine.unassignedID);
                //unit.StaticState(true);
            }
        }
#endif
    }

    protected RtsCamera rtsCamera;
    protected Unit ActiveUnit_1, ActiveUnit_2;
    protected virtual void CameraUpdate()
    {
        if (ActiveUnit_1 == null)// || RaidBossAIBase.ActionLive)
            return;

        if (_isEnd)
            return;

    }

    public void SetCameraUpdate()
    {
	   CameraUpdate();
    }

    protected virtual void OnDestroy()
    {
	   if (null != FocusingCam)
		  Destroy(FocusingCam.gameObject);

	   //< 컷씬 이벤트 해제(중간에 나가는 경우가 있을수도있기때문에 해제시켜줌)
	   CutSceneMgr.EndScene(false);

	   effectPool.SetDestroy();
	   projectilePool.SetDestroy();
	   OrgprojectilePool.SetDestroy();
	   InGameObjPool.SetDestroy();

	   //< 삭제
	   if (SkillEventMgr.Live)
		  Destroy(SkillEventMgr.instance.gameObject);
    }

    //< 외부에서 풀을 초기화하기위함
    public void ClearPool()
    {
        effectPool.SetDestroy();
        projectilePool.SetDestroy();
        OrgprojectilePool.SetDestroy();
        InGameObjPool.SetDestroy();
        LoadUnitList.Clear();
    }

    protected virtual void InitDatas()
    {
	   isAutoMode = PlayerPrefs.GetInt(GameDefine.GameName + "_" + GameMode.ToString() + GameDefine.AutoMode_Suffix) == 1;

	   // 플레이어 스폰지점 관련 데이터 로드
	   if (!NotInGame)
	   {
			GameObject team1 = GameObject.Find("Team1Start");
		  	GameObject team2 = GameObject.Find("Team2Start");

			if( G_GameInfo.GameInfo.GameMode == GAME_MODE.SINGLE ){
				SingleGameInfo stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;
				DungeonTable.StageInfo stage = _LowDataMgr.instance.GetStageInfo(stageinfo.StageId);
				if (stage.type == 2){
					team1 = GameObject.Find("Team1Start_Hard");
					team2 = GameObject.Find("Team2Start_Hard");
				}
			}

		  	if (team1 != null)
			 	team1StartTrans = team1.transform;

		  	if (team2 != null)
			 	team2StartTrans = team2.transform;

		  // 네비게이션 메시 기반 시작 -> 끝 위치.
		  navPosGroups = new List<NavigationPosGroup>();
		  GameObject navPosGroup = GameObject.Find("NavigationPosGroup");
		  if (null != navPosGroup)
		  {
			 Transform posGroupTrans = navPosGroup.transform;

			 int groupCnt = posGroupTrans.childCount / 2;
			 for (int i = 1; i <= groupCnt; i++)
			 {
				Transform startTrans = posGroupTrans.FindChild("_StartPosition" + i.ToString());
				Transform endTrans = posGroupTrans.FindChild("_EndPosition" + i.ToString());

				navPosGroups.Add(new NavigationPosGroup() { start = startTrans, end = endTrans });
			 }
		  }
		  else
			 navPosGroups.Add(new NavigationPosGroup() { start = team1StartTrans, end = team2StartTrans });

		  navGroupIndex = 0;
	   }
    }

    /// 게임상 필요한 매니저 등을 생성 및 초기화.
    protected virtual void InitManagers()
    {
	   characterMgr = ResourceMgr.Instantiate<GameObject>("GameInfo/CharacterMgr").GetComponent<CharacterMgr>();
	   characterMgr.transform.AttachTo(transform);
	   characterMgr.RemovUnitCallback += OnRemoveUnit;
    }

    /// 모든 게임 준비가 완료됐을 때 호출되도록 한다.
    protected virtual void InitUI()
    {
	   BoardPanel = UIMgr.Open("UIPanel/InGameBoardPanel").GetComponent<InGameBoardPanel>();
    }

    //< 하나의 유닛이라도 로드가 완료되었을시 호출된다.
    //< 유닛의 모델 로드 자체가 비동기이므로, 거의 유닛의 개수만큼 호출된다고 봐도 무방
    //< 즉 현재 최종적으로 로드가 되는 시점을 알아낼수있는 방법이 없음(플레이어와 몬스터의 스폰 시점이 다르기때문)
    protected virtual void OnLoadingComplete()
    {
        if (GameLive)
	        return;

        // 캐릭터들도 다 로드되었는지 체크한후 완료.
        /*
        foreach (PlayerController playerCtlr in playerControllers.Values)
        {
	        if (false == playerCtlr.LoadingDone)
		        return;
        }
        */

        GameLive = true;
        //< 풀을 로드해놓는다.
        StartCoroutine(PoolLoadRoutine());

        if (null != LoadingCompleted)
	        LoadingCompleted();

        if (GameMode != GAME_MODE.ARENA)
        {
	        //< PVP 모드일경우에는 따로 리더를 설정해준다.
	        ChangeLeader(0, false);
        }

        //< 모두 이동 불가처리
        if (G_GameInfo.PlayerController != null)
        {
	        foreach (Unit unit in G_GameInfo.PlayerController.Units)
		        unit.StaticState(true);
        }

		#if UNITY_EDITOR

		if (SceneManager.instance.testData.bCutSceneTest){
	        if (GameObject.Find("CinemaSceneManager") != null)
	        {
	            CinemaSceneManager csm = GameObject.Find("CinemaSceneManager").GetComponent<CinemaSceneManager>();

	            SingleGameInfo stageinfo = (SingleGameInfo)G_GameInfo.GameInfo;
	            if (csm.getCinemaSceneStartTypeByStageID(stageinfo.StageId) == CinemaSceneManager.StartType.BEFORE_GAME_START)
	            {
	                int endIdx = 0;
	                if (stageinfo.StageId == 310)
	                {
	                    endIdx = 1;
	                }
	                CutSceneMgr.StartCinemaScene(true, 0, endIdx, () => {
	                    StartCoroutine(GameStartReady());
	                });
	                return;
	            }
	        }
		}
		
		#endif
		StartCoroutine(GameStartReady());
    }

    protected virtual IEnumerator GameStartReady() {
        //while( !IsEndLoad)
        //{
        //    yield return new WaitForSeconds(0.1f);
        //}

        GameStart();
        yield return null;
    }

    protected virtual void GameStart()
    {
        IsStartGame = true;

		// 1-7 컷씬후 UIcamera가 꺼져버려서 여기서 다시 켠다. 
		UIMgr.instance.UICamera.gameObject.GetComponent<Camera> ().enabled = true;
		UIMgr.instance.UICamera.enabled = true;

        // 모든 캐릭터들을 행동가능한 상태로 만든다.
	   
        if (null != HudPanel)
        {
            HudPanel.GameStart();
            HudPanel.SetAutoMode(AutoMode);
        }

        //startTime = Time.time;
        PlayTime = 0;
		
        Time.timeScale = GameDefine.DefaultTimeScale;

		if (SceneManager.instance.testData.bSingleSceneTestStart) {
			Time.timeScale = 5f;
		}
		if (SceneManager.instance.testData.bQuestTestStart) {
			Time.timeScale = 5f;
		}

        NotTimeUpdate = false;

        {
            Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
            characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);
            foreach (Unit unit in allUnits)
            {
                if (null == unit)
                    continue;

                unit.StaticState(false);
                unit.SetTarget(GameDefine.unassignedID);
            }
        }
    }

    //< 하나의 유닛이라도 모델로드까지 모두 완료되었을시 호출
    protected virtual void CallLoadingCompleted()
    {
	   LoadingCompleted -= CallLoadingCompleted;
    }

    #region Pool Manager

    List<uint> LoadUnitList = new List<uint>();
    IEnumerator PoolLoadRoutine()
    {
        //< 모든 유닛을 돌면서 이펙트, 프로젝트타일을 로드해놓는다.
        //foreach (KeyValuePair<int, Unit> dic in characterMgr.allUnitDic)
        //{
        //Unit unit = dic.Value;
        //if (unit == null || unit.UnitType == UnitType.Prop || unit.UnitType == UnitType.Trap)
        //continue;

        ////< 이미 로드했던 유닛은 패스한다.
        //if (LoadUnitList.Contains(unit.SkillCtlr.UnitId))
        //continue;

        //LoadUnitList.Add(unit.SkillCtlr.UnitId);

        ////< 해당 유닛의 일반 노멀 스킬값을 가져온다.
        //GetAbilityData(unit, unit.SkillCtlr.UnitId, unit.UnitType);

        ////< 해당 유닛의 사운드 로드(밀리지않게 로드만 해놓는다)
        //SoundSpawnPool(unit.SkillCtlr.UnitId);
        //}

        ////< 기본 피격 이펙트관련해서는 따로 로드해놓는다
        //FillSpawnPool(effectPool, "Fx_beshot_02", GameMode == GAME_MODE.SKILLTOOL ? 0 : 12);            //< 평타 이펙트
        //FillSpawnPool(effectPool, "Fx_beshot_04", GameMode == GAME_MODE.SKILLTOOL ? 3 : 6);             //< 스킬 이펙트
        //FillSpawnPool(effectPool, "Fx_beshot_03", GameMode == GAME_MODE.SKILLTOOL ? 0 : 3);             //< 마법형프로젝트타일 이펙트
        //FillSpawnPool(effectPool, "Fx_beshot_01", GameMode == GAME_MODE.SKILLTOOL ? 0 : 3);             //< 물리형프로젝트타일 이펙트
        //FillSpawnPool(effectPool, "Fx_Monstor_Death_01", GameMode == GAME_MODE.SKILLTOOL ? 0 : 6);     //< 죽을때 이펙트

        ////< 동전 풀 추가
        if (GameMode == GAME_MODE.SINGLE || GameMode == GAME_MODE.SPECIAL_EXP || GameMode == GAME_MODE.SPECIAL_GOLD ||
            GameMode == GAME_MODE.TOWER || GameMode == GAME_MODE.RAID || GameMode == GAME_MODE.TUTORIAL)//&& GameMode != GAME_MODE.SKILLTOOL)
        {
            //Resource.AniInfo[] animDatas = new Resource.AniInfo[5];
            //for (int i = 0; i < animDatas.Length; i++) animDatas[i] = new Resource.AniInfo();

            //animDatas[0].aniName = "Coin_Drop_Idle_01";
            //for (int i = 0; i < 4; i++)
            //    animDatas[i + 1].aniName = "Coin_Drop_Start_0" + (i + 1).ToString();
            FillSpawnPool_Model(InGameObjPool, "InGameObject/prefab/Coin_Drop_02", null, 10);
            FillSpawnPool_Model(InGameObjPool, "InGameObject/prefab/Item_Drop_01", null, 5);

            //FillSpawnPool(effectPool, "Fx_GoldDrop_01", 10);
        }

        //< 프로젝트타일 오리지널 프리팹 풀로 생성
        PrefabPool prefabPool = new PrefabPool((Resources.Load("GameInfo/Projectile") as GameObject).transform);
	    prefabPool.preloadAmount = GameMode == GAME_MODE.SKILLTOOL ? 5 : 10;
	    prefabPool.AddSpawnCount = 3;
	    prefabPool.limitInstances = false;
	    prefabPool.limitAmount = 30;
	    OrgprojectilePool.CreatePrefabPool(prefabPool, true);

	    EffectLoadPoolNames.Clear();
	    ProjectTileLoadPoolNames.Clear();
	    //SoundLoadPoolNames.Clear();
	    ModelLoadPoolNames.Clear();

	    yield return null;
    }

    void GetAbilityData(Unit unit, uint unitIdx, UnitType unittype)
    {

        ////< 시전 이펙트 로드
        //for (int i = 0; i < 4; i++)
        //{
        //        _ResourceLowData.AniTableInfo attackData = unit.GetAniData((eAnimName)((int)eAnimName.Anim_attack1 + i));
        //if (attackData != null && attackData.effect != "0")
        //{
        //if (GameMode == GAME_MODE.SKILLTOOL && unittype == UnitType.Unit)
        //FillSpawnPool(effectPool, attackData.effect, 2);
        //else if (GameMode != GAME_MODE.SKILLTOOL)
        //FillSpawnPool(effectPool, attackData.effect, unittype == UnitType.Unit ? 1 : 3); //< 플레이어라면 한명이기에 1개씩, 그외라면 몬스터기에 여러개

        //}

        ////< 스킬은 플레이어 or 보스만 생성해준다.
        //if (unittype == UnitType.Unit || unittype == UnitType.Boss)
        //{
        //            _ResourceLowData.AniTableInfo skillData = unit.GetAniData((eAnimName)((int)eAnimName.Anim_skill1 + i));
        //if (skillData != null && skillData.effect != "0")
        //FillSpawnPool(effectPool, skillData.effect, 1); //< 스킬은 무조건 1개씩
        //}
        //}

        ////< 충돌 이펙트및 프로젝트타일 로드
        //Dictionary<uint, List<SkillTablesLowData.AbilityInfo>> abilityDic = LowDataMgr.GetSkillAbilitys(unitIdx);
        //if (abilityDic != null)
        //{
        //foreach (KeyValuePair<uint, List<SkillTablesLowData.AbilityInfo>> dic in abilityDic)
        //{
        //for (int i = 0; i < dic.Value.Count; i++)
        //{
        //if (dic.Value[i].targetEffect != "0")
        //    FillSpawnPool(effectPool, dic.Value[i].targetEffect, 1);

        ////< 사운드도 처리
        //if (dic.Value[i].targetSound > 0)
        //{
        //    if (!SoundLoadPoolNames.Contains((int)dic.Value[i].targetSound))
        //    {
        //	   SoundLoadPoolNames.Add((int)dic.Value[i].targetSound);
        //	   SoundHelper.SetSoundLoad(dic.Value[i].targetSound);
        //    }
        //}

        ////< 프로젝트타일도 생성해놓는다.
        //if (dic.Value[i].callAbilityIdx != 0)
        //{
        //    SkillTablesLowData.ProjectTileInfo projecttile = LowDataMgr.GetSkillProjectTileData(dic.Value[i].callAbilityIdx);
        //    if (projecttile != null && projecttile.colideEffect != null)
        //	   FillSpawnPool(effectPool, projecttile.colideEffect, 1);

        //    if (projecttile != null && projecttile.prefab != "0")
        //	   ProjectTileSpawnPool(projectilePool, projecttile.prefab);

        //    if (projecttile != null && projecttile.ProjectSound != 0)
        //    {
        //	   if (!SoundLoadPoolNames.Contains((int)projecttile.ProjectSound))
        //	   {
        //		  SoundLoadPoolNames.Add((int)projecttile.ProjectSound);
        //		  SoundHelper.SetSoundLoad(projecttile.ProjectSound);
        //	   }
        //    }
        //}
        //}
        //}
        //}
    }

    List<string> EffectLoadPoolNames = new List<string>();
    public bool FillSpawnPool(SpawnPool targetPool, string filename, int preloadAmount = 1)
    {
        //if (!EffectLoadPoolNames.Contains(filename))
        //{
        //    EffectLoadPoolNames.Add(filename);

        //    //< 이펙트를 가져온다.
        //    AssetbundleLoader.GetEffect(filename, (effect) =>
        //    {
        //        PrefabPool prefabPool = new PrefabPool(effect.transform);
        //        prefabPool.preloadAmount = preloadAmount;
        //        prefabPool.AddSpawnCount = 2;
        //        prefabPool.limitInstances = false;
        //        prefabPool.limitAmount = 30;
        //        targetPool.CreatePrefabPool(prefabPool, true);
        //    }, false);
        //}
        return true;
    }

    List<string> ModelLoadPoolNames = new List<string>();
    bool FillSpawnPool_Model(SpawnPool targetPool, string filename, Resource.AniInfo[] animDatas, int preloadAmount = 1)
    {
	    if (!ModelLoadPoolNames.Contains(filename))
	    {
		    ModelLoadPoolNames.Add(filename);
		    GameObject model = ResourceMgr.Load(filename) as GameObject;
		    //Debug.LogError("<color=red>2JW : FillSpawnPool_Model model - " + model + "</color>", model);
		    //< 모델을 가져온다.
		    //AssetbundleLoader.AssetLoad(filename, (model) =>
		    {
			    GameObject obj = Instantiate(model) as GameObject;
                if(obj.GetComponent<TweenAsset>() == null )
			        obj.AddComponent<TweenAsset>();

			    obj.name = obj.name.Replace("(Clone)", "");

                //if (animDatas != null)
                //{
                //    //< 애니메이션을 결합해준다.
                //    AssetbundleLoader.AddAnimationClips(obj.animation, animDatas, () =>
                //    {
                        
                //    });
                //}

                PrefabPool prefabPool = new PrefabPool(obj.transform);
                prefabPool.preloadAmount = preloadAmount;
                prefabPool.AddSpawnCount = 2;
                prefabPool.limitInstances = false;
                prefabPool.limitAmount = 30;
                targetPool.CreatePrefabPool(prefabPool, true);

                obj.SetActive(false);
            }//);
	    }
	    return true;
    }

    void LocalFillSpawnPool(SpawnPool targetPool, string path, string name, int preloadAmount = 1)
    {
	   GameObject obj = Resources.Load(path) as GameObject;
	   //obj.name = name;

	   PrefabPool prefabPool = new PrefabPool(obj.transform);
	   prefabPool.preloadAmount = preloadAmount;
	   prefabPool.AddSpawnCount = 2;
	   prefabPool.limitInstances = false;
	   prefabPool.limitAmount = 30;
	   targetPool.CreatePrefabPool(prefabPool, true);
    }

    List<string> ProjectTileLoadPoolNames = new List<string>();
    bool ProjectTileSpawnPool(SpawnPool targetPool, string filename)
    {
	   //if (!ProjectTileLoadPoolNames.Contains(filename))
	   //{
		  //ProjectTileLoadPoolNames.Add(filename);

		  ////< 프로젝트타일을 가져온다.
		  //AssetbundleLoader.GetProjectTile(filename, (projecttile) =>
		  //{
			 ////< 한번더 체크한다.
			 //PrefabPool prefabPool = new PrefabPool(projecttile.transform);
			 //prefabPool.preloadAmount = 2;
			 //prefabPool.AddSpawnCount = 2;
			 //prefabPool.limitInstances = false;
			 //prefabPool.limitAmount = 30;
			 //targetPool.CreatePrefabPool(prefabPool, true);

		  //}, false);
	   //}
	   return true;
    }

    //List<int> SoundLoadPoolNames = new List<int>();
    //void SoundSpawnPool(uint idx)
    //{
	   //UnitLowData.GradeInfo grade = LowDataMgr.GetUnitGradeInfo(idx);
	   //if (grade == null)
		  //return;

	   //ResourceLowData.UnitInfo info = LowDataMgr.GetUnitResourceData(grade.resource);
	   //if (info == null)
		  //return;

	   //string[] Modelname = info.modelFile.Split('_');
	   //List<int> sounds = LowDataMgr.GetUnitSoundList(Modelname[0]);
	   //for (int i = 0; i < sounds.Count; i++)
	   //{
		  //if (SoundLoadPoolNames.Contains(sounds[i]))
			 //continue;

		  //SoundLoadPoolNames.Add(sounds[i]);

		  ////< 로드 해놓는다.
		  ////SoundHelper.SetSoundLoad((uint)sounds[i]);
	   //}
    //}

    #endregion

    //< 부활할시 호출되는 함수
    public virtual void RevivePlayer()
    {
        ReviveCount++;

        isEnd = false;

        PlayerController controller = G_GameInfo.PlayerController;
        Vector3 spawnPos = Vector3.zero;
        if (null != controller.Leader)
	        spawnPos = controller.Leader.transform.position;
        else
	        spawnPos = controller.Leader.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 20, -1))
	        spawnPos = hit.position;

        foreach (Unit unit in controller.Units)
	        unit.Revive();

        foreach (Unit unit in controller.Partners)
        {
            unit.Revive();
            if( unit.HelperParnetUnit != null )
            {
                NavMeshHit pos;
                Vector3 partnerpos = unit.transform.position;
                if (NavMesh.SamplePosition(unit.HelperParnetUnit.transform.position, out pos, 20, -1))
                    partnerpos = hit.position;

                if (!unit.GetComponent<NavMeshAgent>().Warp(partnerpos))
                    unit.transform.position = partnerpos;
            }

        }
	        

        if (null != controller.Leader)
        {
            Vector3 newSpawnPos = spawnPos + new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * 1.5f;
            if (NavMesh.SamplePosition(newSpawnPos, out hit, 20, -1))
                newSpawnPos = hit.position;
            if (!controller.Leader.GetComponent<NavMeshAgent>().Warp(newSpawnPos))
	            controller.Leader.transform.position = newSpawnPos;
        }

        //if (HudPanel != null)
        //    HudPanel.Revive();

        //< 다시 부활했을시 이벤트를 쏴줌
        EventListner.instance.TriggerEvent("Revive", "");
    }

    /// 유닛 죽으면 바로 호출될 함수
    int ReviveCount = 0;
    protected virtual void OnDieUnit(Unit deadUnit)
    {
	   switch (deadUnit.UnitType)
	   {
		  case UnitType.Unit:
			 {
                //< 쿨타임이 돌고있다면 실패!
                //if (HudPanel.PCInfos.GetCoolTimeLive())
                //{
                //    PlayerController ctlr = FindOwnerPlayerController();
                //    for (int i = 0; i < ctlr.Units.Count; i++)
                //    {
                //        if (!ctlr.Units[i].CharInfo.IsDead)
                //            ctlr.Units[i].StaticState(true);
                //    }

                //    isEnd = true;
                //    TempCoroutine.instance.FrameDelay(3, () =>
                //    {
                //        if (ReviveCount == 0)
                //            UIMgr.Open("InGame/InGameRevivePanel");
                //        else
                //            ForcedGameEnd();

                //        EventListner.instance.TriggerEvent("UnitAllDead", "");
                //    });
                //}
                //else
                //{
                //    PlayerController ctlr = FindOwnerPlayerController();
                //    Unit controllUnit = null;
                //    for (int i = 0; i < ctlr.Units.Count; i++)
                //    {
                //        if (!ctlr.Units[i].CharInfo.IsDead && ctlr.Units[i] != ctlr.Leader && !ctlr.Units[i].IsHelper)
                //        {
                //            controllUnit = ctlr.Units[i];
                //            break;
                //        }
                //    }

                //    if (null != controllUnit && !isEnd)
                //    {
                //        ChangeLeader(controllUnit);
                //        HudPanel.PCInfos.NextCool();
                //    }
                //}
             }
			 break;

		  case UnitType.Boss:
			 {
				// 모든 보스를 잡아야지만 게임 종료가 되도록한다.
				if (characterMgr.allTypeDic.ContainsKey(UnitType.Boss))
				{
				    List<Unit> bossList = characterMgr.allTypeDic[UnitType.Boss];
				    bool allDead = true;
				    foreach (Unit boss in bossList)
				    {
					   if (null == boss)
						  continue;

					   if (!boss.CharInfo.IsDead)
					   {
						  allDead = false;
						  break;
					   }
				    }

				    if (0 == bossList.Count || allDead)
				    {
                        PrepareEndGame(true);
					   //Invoke("PrepareEndGame", 0.2f);
				    }
				}
			 }
			 break;
	   }
    }

    /// 몬스터가 죽을때 호출될 콜백함수
    protected virtual void OnRemoveUnit(Unit removedUnit)
    {
	    if (null == removedUnit || isEnd)
		    return;

	    switch (removedUnit.UnitType)
	    {
		    case UnitType.Unit:
			    {
			        PlayerController ctlr = G_GameInfo.PlayerController;

                    //아레나 모드의 경우 내가 다죽든 상대방이 다죽든 결과는 서버에서 보냄
                    if (G_GameInfo.GameMode == GAME_MODE.ARENA || G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
                        return;

			        // 플레이어의 모든 유닛이 매니저에서 지워져야지만 끝나도록한다.
			        bool notExist = true;
			        if (notExist && ctlr.AliveCount() <= 0)
			        {
				        isEnd = true;
                        if (GameMode == GAME_MODE.SPECIAL_GOLD || GameMode == GAME_MODE.SPECIAL_EXP)//이쪽은 부활 관련 횟수가 무제한.
                        {
                            UIMgr.OpenRevivePopup(OnClickRevive);
                        }
                        else
                        {
                            string gameMode = string.Format("{0}_ReviveCount", G_GameInfo.GameMode);
                            int maxReviveCount = _LowDataMgr.instance.GetEtcTableValue<int>((EtcID)System.Enum.Parse(typeof(EtcID), gameMode));

                            //if (ReviveCount == 0 && G_GameInfo.GameMode == GAME_MODE.SINGLE )
                            if (ReviveCount < maxReviveCount)
                            {
                                UIMgr.OpenRevivePopup(OnClickRevive);
                            }
                            else
                            {
                                ForcedGameEnd();
                            }
                        }
			        }
			    }
			    break;

		    case UnitType.Boss:
			    {
			        // 모든 보스를 잡아야지만 게임 종료가 되도록한다.
			        if (characterMgr.allTypeDic.ContainsKey(UnitType.Boss))
			        {
				        List<Unit> bossList = characterMgr.allTypeDic[UnitType.Boss];
				        if (0 == bossList.Count)
					        EndGame(true);
			        }
			    }
			    break;
	    }
    }

    // 게임 패배
    public void ForcedGameEnd()
    {
        if (GameMode == GAME_MODE.FREEFIGHT)
        {
            if(FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                NetworkClient.instance.SendPMsgMessRoomLeaveC((long)FreeFightGameState.selectedRoomNo);

            if (FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM)
                NetworkClient.instance.SendPMsgColosseumLeaveRoomC((long)FreeFightGameState.selectedRoomNo);

            if (FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID)
                NetworkClient.instance.SendPMsgMultiBossLeaveRoomC((long)FreeFightGameState.selectedRoomNo);
        }
	    else
        {
            PrepareEndGame();
            
            TempCoroutine.instance.FrameDelay(1, () =>
            {
                bool unConditionalVictory = GameMode == GAME_MODE.SPECIAL_EXP || GameMode == GAME_MODE.SPECIAL_GOLD;//조건없이 무조건 승리인 던전.
                EndGame(unConditionalVictory);
            });
        }

    }

    //< 최종 종료전에 수행되어야할 작업(보스가 죽는순간 등)
    //< 모든 유닛의 움직임을 정지, 승리했을시 몬스터 모두 죽이기등
    public virtual void PrepareEndGame(bool win = false)
    {
	    NotUpdate = true;

        HudPanel.StopWarning();

        //< 혹시나 돌고있는 AI가있다면 종료
        //RaidBossAIBase.EndRaidAI();

        //< UI터치 막음
        UIMgr.instance.UICamera.enabled = false;

	    // 모든 유닛정보 얻기
	    Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
	    characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);
        
        //< PVP모드가 아닐경우 추가 처리
        //if (GameMode != GAME_MODE.PVP)
	    {
            //if (null != HudPanel)
            //HudPanel.Joystick.ActivateJoystick(false);

            PlayerController playerCtlr = G_GameInfo.PlayerController;
		    if (null != playerCtlr && playerCtlr.CurLeaderUnit())
		    {
			    //< 타겟팅 꺼줌
			    playerCtlr.CurLeaderUnit().IsLeader = false;
		    }

		    //< 남은 몬스터들 죽임
		    foreach (Unit unit in allUnits)
		    {
			    if (null == unit || (null != unit && !unit.Usable))
			        continue;

			    //< 이겼을때만 죽이고 그게아니라면 모두 정지만 시켜놓는다.
			    if (win && unit.TeamID != 0)
                    unit.UnitKill(G_GameInfo.PlayerController.Leader);
                //unit.UnitKill(playerControllers[0].Leader);
			    else
			    {
			    //unit.ChangeState(UnitState.Idle);
			        unit.StaticState(true, 3);
			        unit.SetTarget(GameDefine.unassignedID);
			    }
		    }

		    // 슬로우 효과 주기
		    if (win)
		    {
			    Time.timeScale = 0.1f;
			    float endSlowDuration = 0.25f;
                if(G_GameInfo.CharacterMgr != null && G_GameInfo.CharacterMgr.BossUnit != null)
			        G_GameInfo.CharacterMgr.BossUnit.BossEndShaderChange();

                //RadialBlurEffect blur = CameraManager.instance.GetComponentInChildren<RadialBlurEffect>();
                //blur.enabled = true;

                StartCoroutine(RestoreTimeScale(endSlowDuration));

                if (_GameEndEventPanel != null)
			        _GameEndEventPanel.StartEvent(1, rtsCamera);
		    }
	    }
	    //else
	    //{
     //       //< 모든 유닛을 멈춘다.
     //       foreach (Unit unit in allUnits)
		   // {
			  //  if (null == unit || (null != unit && !unit.Usable))
			  //  continue;

			  //  unit.ChangeState(UnitState.Idle);
			  //  unit.SetTarget(GameDefine.unassignedID);
			  //  unit.StaticState(true);
		   // }
	    //}

	   //< 카메라 쉐이크 없앰
	   CameraManager.instance.ShakeClose();
    }

    protected IEnumerator RestoreTimeScale(float duration, System.Action _call = null)
    {
	   yield return new WaitForSeconds(duration);

	   Time.timeScale = GameDefine.DefaultTimeScale;

        //RadialBlurEffect blur = CameraManager.instance.GetComponentInChildren<RadialBlurEffect>();
        //blur.enabled = false;

        if (_call != null)
		  _call();
    }

    /// <summary>
    /// 최종 게임종료처리(게임이 완전 끝났을때 처리)
    /// 각게임인포마다 재정의해서 필요한부분 구현 필요
    /// <param name="isSuccess"> 성공여부 </param>
    /// </summary>
    public virtual void EndGame(bool isSuccess)
    {
        if (!IsStartGame)//이미 처리중 중복처리 막는다
            return;

            /*
        if (isSuccess)
        {
            Time.timeScale = 0.1f;
            float endSlowDuration = 0.25f;
            if (G_GameInfo.CharacterMgr != null && G_GameInfo.CharacterMgr.BossUnit != null)
                G_GameInfo.CharacterMgr.BossUnit.BossEndShaderChange();

            //RadialBlurEffect blur = CameraManager.instance.GetComponentInChildren<RadialBlurEffect>();
            //blur.enabled = true;

            StartCoroutine(RestoreTimeScale(endSlowDuration));

            if (_GameEndEventPanel != null)
                _GameEndEventPanel.StartEvent(1, rtsCamera);
        }
        */

                NotTimeUpdate = false;//끝났으니 더이상 타입을 업데이트 하지 않는다.
        IsStartGame = false;

        // 게임 플레이 시간
        //playingTime = Time.time - startTime;
        Time.timeScale = GameDefine.DefaultTimeScale;

	    if (GameDefine.TestMode)
		    Debug.Log("플레이 시간 " + PlayTime);

	    // 종료처리동안 UI인풋 막기
	    UIMgr.instance.UICamera.enabled = false;
        
        isEnd = true;

        /*
	    //< 모든 유닛을 숨겨준다.
	    Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
	    characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);
	    for (int i = 0; i < allUnits.Length; i++)
	    {
		    if (allUnits[i] != null)
			    allUnits[i].gameObject.SetActive(false);
	    }
        */
        Unit[] allUnits = new Unit[characterMgr.allUnitDic.Values.Count];
        characterMgr.allUnitDic.Values.CopyTo(allUnits, 0);

        foreach (Unit unit in allUnits)
        {
            if (null == unit || (null != unit && !unit.Usable))
                continue;

            //unit.ChangeState(UnitState.Idle);
            unit.SetTarget(GameDefine.unassignedID);
            unit.StaticState(true);
        }

        if (SkillEventMgr.Live)
		    SkillEventMgr.instance.SetEnd();

	    //if (isSuccess)
		//    SoundHelper.PlayBgmSound(9011, false, "", false);
	    //else
		//    SoundHelper.PlayBgmSound(9012, false, "", false);

        if(isSuccess)
            NetData.instance.GetUserInfo().MissionUpdate(MissionType.INSTANCE_CLEAR, (byte)GameMode , 1);

        if( G_GameInfo.GameInfo.GameMode == GAME_MODE.SINGLE )
        {
            PlayerController ctlr = G_GameInfo.PlayerController;

            if( ctlr.NpcKillCount > 0 )
                NetData.instance.GetUserInfo().MissionUpdate(MissionType.MONSTER_KILL, 0, (uint)ctlr.NpcKillCount);
        }
            

        if (_GameEndEventPanel != null)
		    Destroy(_GameEndEventPanel.gameObject);
        /*
        //< 혹시 채팅창이 켜져있다면 꺼준다.
        GameObject chattingpanel = UIMgr.GetUI("Chatting/ChattingPanel");
        if (chattingpanel != null && chattingpanel.gameObject.activeSelf)
           chattingpanel.GetComponent<UIBasePanel>().Hide();
        */


        if(G_GameInfo.GameMode !=GAME_MODE.TUTORIAL)
        {
            SendAchieveFightData();
            Debug.Log(string.Format("MaxCombo:{0}\nMaxDamage:{1}\nSkillCnt:{2}\nKillMob:{3}\nKillBoss:{4}\nPK:{5}:\nDie:{6}\nRevive:{7}\nPartnerSkillcnt:{8}",
                 G_GameInfo.GameInfo._AchieveFightData.MaxCombo,
                  G_GameInfo.GameInfo._AchieveFightData.MaxDamaga,
                   G_GameInfo.GameInfo._AchieveFightData.SkillCount,
                    G_GameInfo.GameInfo._AchieveFightData.KillMonsterCount,
                     G_GameInfo.GameInfo._AchieveFightData.KillBossCount,
                      G_GameInfo.GameInfo._AchieveFightData.killPkCount,
                       G_GameInfo.GameInfo._AchieveFightData.DieCount,
                        G_GameInfo.GameInfo._AchieveFightData.ReviveCount,
                        G_GameInfo.GameInfo._AchieveFightData.parterSkillCnt));

        }

    }

    /// 유저 컨트롤러에 해당하는 유닛만 리더로 설정가능하도록 한다. ([유저_PlayerController]의 유닛만됨)
    public virtual Unit ChangeLeader(Unit newLeader, bool coexist = true)
    {
	   if (null == newLeader)
		  return null;

	   PlayerController ctlr = G_GameInfo.PlayerController;

	   //< 기존 리더는 2로, 새로운 리더가 1로 설정
	   ActiveUnit_2 = ctlr.Leader;
	   ActiveUnit_1 = newLeader;

	   //< 둘이 같다면 보정
	   if (ActiveUnit_1 == ActiveUnit_2)
		  ActiveUnit_2 = null;

	   //< 기존 리더가 이동중이었었다면 해제
	   if (ActiveUnit_2 != null)
		  ActiveUnit_2.StopState = false;

	   //ctlr.SetLeader(newLeader, coexist);
	   newLeader.inputCtlr = StartLookAndControl(newLeader);

	   if (null != spawnCtlr)
		  spawnCtlr.ChangeAgent(newLeader.gameObject);

	   if (null != newLeader && null != HudPanel)
	   {
		  //Debug.LogWarning("2JW : GameInfoBase ChangeLeader() - " + newLeader);
		  HudPanel.LeaderUnit = newLeader;
	   }

	   if (!GameLive)
		  return newLeader;

	   //< 연출
	   SkillEventMgr.instance.SpawnEvent(true, newLeader);

	   //< 사운드 실행
	   //SoundHelper.UIClickSound((uint)eUIClickSoundType.UnitSpawn);

	   //< 이펙트를 띄워준다
	   newLeader.SpawnEffect("Fx_Monster_Regen_04", 1, newLeader.transform, newLeader.transform);

        if(!SceneManager.instance.IsRTNetwork)
        {
            //< 주변에 있는 모든 적들에게 대미지 + 밀리게하기
            List<Unit> list = characterMgr.FindTargetsByTeam(1, newLeader);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //< 레이드에서 에러가 날수도있기에 가까운 적만 공격하도록 처리
                    if ((newLeader.transform.position - list[i].transform.position).magnitude < 20)
                    {
                        list[i].SetPush(2, newLeader.gameObject, 0);
                        list[i].TakeDamage(newLeader, 1, newLeader.CharInfo.Atk * 0.2f, 0, eAttackType.All, true, null);
                    }
                }
            }
        }

	   return newLeader;
    }
    public Unit ChangeLeader(int unitIndex, bool coexist = true)
    {
	   if (0 > unitIndex)
		  return null;

	   PlayerController ctlr = G_GameInfo.PlayerController;
	   if (ctlr == null || unitIndex >= ctlr.Units.Count)
		  return null;
	   return ChangeLeader(ctlr.Units[unitIndex], coexist);
    }

    #endregion


    #region :: related PlayerControll ::

    /// 플레이어 생성
    /// 
    public PlayerController CreatePlayerController(PlayerUnitData player, List<PlayerUnitData> partnerList)
    {
        GameObject controllerGO = ResourceMgr.Instantiate<GameObject>("NetWork/PlayerController");

        if (!SceneManager.instance.IsRTNetwork)
        {
            controllerGO.transform.position = GetSpawnTransform((eTeamType)player._TeamID) != null ? GetSpawnTransform((eTeamType)player._TeamID).position : Vector3.zero;
            controllerGO.transform.rotation = GetSpawnTransform((eTeamType)player._TeamID) != null ? GetSpawnTransform((eTeamType)player._TeamID).rotation : Quaternion.identity;
        }

        PlayerController playerController = controllerGO.GetComponent<PlayerController>();
        playerController.Init(player, partnerList);

        if (SceneManager.instance.IsRTNetwork)
        {
            int PosX, PosY;
            NetworkClient.instance.GetRegenPos(out PosX, out PosY);
            Vector3 pos = NaviTileInfo.instance.GetTilePos(PosX, PosY);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(pos, out navHit, 20, -1))
            {
                pos = navHit.position;
            }

            //controllerGO.transform.position = pos;
            PlayerController playerCtrl = G_GameInfo.PlayerController;
            if (playerCtrl != null && playerCtrl.Leader != null)
            {
                playerCtrl.Leader.GetComponent<NavMeshAgent>().Warp(pos);
                playerCtrl.Leader.BeforePosX = PosX;
                playerCtrl.Leader.BeforePosY = PosY;
                playerCtrl.Leader.MoveNetworkCalibrate(pos);
                //playerCtrl.Leader.inputCtlr.TargetPos = playerCtrl.Leader.transform.position;
            }
        }

        return playerController;
    }

    #endregion

    #region :: Spawner ::

    /// 아군 유닛를 소환하도록 한다.
    public GameObject SpawnUnit(ulong unitUUID, byte teamID, PlayerUnitData syncData, Vector3 spawnLoaction, Quaternion spawnRotation, bool isPartner = false, Unit leader = null)
    {
        object[] initialData = new object[4] { unitUUID, teamID, (int)UnitType.Unit, syncData };

        GameObject pcGo = new GameObject(string.Format(isPartner ? "PARTNER_{0}" : "UNIT_{0}", syncData._SlotNo));
        Pc pc = pcGo.AddComponent<Pc>();
        pc.transform.position = spawnLoaction;
        pc.transform.rotation = spawnRotation;
        pc.IsPartner = isPartner;
        pc.m_rUUID = unitUUID;
        pc.Init(initialData);
        if (pc.IsPartner == true && leader != null)
            pc.SetHelper(leader);
        return pcGo;
    }

    public GameObject SpawnPartner(ulong unitUUID, byte teamID, PlayerUnitData syncData, Vector3 spawnLoaction, Quaternion spawnRotation, bool isPartner = false, Unit leader = null)
    {
        object[] initialData = new object[4] { unitUUID, teamID, (int)UnitType.Unit, syncData };

        GameObject pcGo = new GameObject(string.Format(isPartner ? "PARTNER_{0}" : "UNIT_{0}", syncData._SlotNo));
        Pc pc = pcGo.AddComponent<Pc>();
        pc.transform.position = spawnLoaction;
        pc.transform.rotation = spawnRotation;
        pc.IsPartner = isPartner;
        pc.Init(initialData);
        if (pc.IsPartner == true && leader != null)
            pc.SetHelper(leader);
        return pcGo;
    }

    /// 각종 NPC 소환용 (적군, 보스 등등)
    public GameObject SpawnNpc(uint npcLowID, eTeamType teamType, int groupNo, Vector3 spawnLoaction, Quaternion spawnRotation, bool isBoss = false, bool isMiddleBoss = false)
    {
        UnitType npcType = isBoss ? UnitType.Boss : UnitType.Npc;

        // 일반 NPC인지, 보스인지, 건물인지 유무 파악해서 UnitType설정하기
        object[] initialData = new object[5] { GameDefine.unassignedID, (byte)teamType, (int)npcType, npcLowID, groupNo };

        string modelName = (npcType == UnitType.Npc ? "NPC_" : "BOSS_") + npcLowID.ToString();

        GameObject npcGo = new GameObject(modelName);
        Npc npc = isBoss ? npcGo.AddComponent<Boss>() : npcGo.AddComponent<Npc>();
        npc.transform.position = spawnLoaction;
        npc.transform.rotation = spawnRotation;
	    npc.Init(initialData);
        npc.isMiddleBoss = isMiddleBoss;

        return npc.gameObject;
    }

    /// 각종 Prop
    public GameObject SpawnProp(uint npcLowID, eTeamType teamType, int groupNo, Vector3 spawnLoaction, Quaternion spawnRotation)
    {
        string prefabPath = "PROP_" + npcLowID.ToString();
        UnitType npcType = UnitType.Prop;

        // 일반 NPC인지, 보스인지, 건물인지 유무 파악해서 UnitType설정하기
        object[] initialData = new object[5] { GameDefine.unassignedID, (byte)teamType, (int)npcType, npcLowID, groupNo };

        GameObject propGO = new GameObject(prefabPath);
        Unit prop = propGO.AddComponent<Prop>();
        prop.transform.position = spawnLoaction;
        prop.transform.rotation = spawnRotation;
        prop.Init(initialData);

        return prop.gameObject;
    }

    public EventUnit SpawnEventUnit(uint npcLowID, string ModelName, Vector3 spawnLoaction, Quaternion spawnRotation, string[] ObjectAniNames)
    {
        string prefabPath = "EVENT_" + ModelName;
        UnitType npcType = UnitType.Prop;

        // 일반 NPC인지, 보스인지, 건물인지 유무 파악해서 UnitType설정하기
        object[] initialData = new object[7] { GameDefine.unassignedID, (byte)eTeamType.Team2, (int)npcType, npcLowID, 0, ModelName, ObjectAniNames };

        GameObject propGO = new GameObject(prefabPath);
        EventUnit _EventUnit = propGO.AddComponent<EventUnit>();
        _EventUnit.transform.position = spawnLoaction;
        _EventUnit.transform.rotation = spawnRotation;
        _EventUnit.Init(initialData);

        return _EventUnit;
    }

    //네트워크용 유닛 생성
    public GameObject SpawnNetworkUnit(ulong accountID, byte teamID, PlayerUnitData syncData, Vector3 spawnLoaction, Quaternion spawnRotation, bool isPartner = false, Unit leader = null)
    {
        object[] initialData = new object[4] { accountID, teamID, (int)UnitType.Unit, syncData };

        GameObject pcGo = new GameObject(string.Format(isPartner ? "PARTNER_{0}" : "UNIT_{0}", syncData._SlotNo));
        Pc pc = pcGo.AddComponent<Pc>();
        pc.transform.position = spawnLoaction;
        pc.transform.rotation = spawnRotation;
        pc.IsPartner = isPartner;
        pc.Init(initialData);
        pc.m_rUUID = accountID;

        //네트워크 파트너는 헬퍼필요없음
        //if (pc.IsPartner == true && leader != null)
        //    pc.SetHelper(leader);

        characterMgr.AddRoomUnit(pc);

        return pcGo;
    }

    //네트워크용 몬스터 생성
    public GameObject SpawnNetworkNpc(ulong netID, uint npcLowID, eTeamType teamType, int groupNo, Vector3 spawnLoaction, Quaternion spawnRotation, bool isBoss = false, bool isMiddleBoss = false)
    {
        UnitType npcType = isBoss ? UnitType.Boss : UnitType.Npc;

        // 일반 NPC인지, 보스인지, 건물인지 유무 파악해서 UnitType설정하기
        object[] initialData = new object[5] { GameDefine.unassignedID, (byte)teamType, (int)npcType, npcLowID, groupNo };

        string modelName = (npcType == UnitType.Npc ? "NPC_" : "BOSS_") + npcLowID.ToString() + "_" + netID.ToString();

        GameObject npcGo = new GameObject(modelName);
        Npc npc = isBoss ? npcGo.AddComponent<Boss>() : npcGo.AddComponent<Npc>();
        npc.transform.position = spawnLoaction;
        npc.transform.rotation = spawnRotation;
        npc.Init(initialData);
        npc.m_rUUID = netID;
        npc.isMiddleBoss = isMiddleBoss;

        characterMgr.AddRoomNPC(npc);

        return npc.gameObject;
    }

    /// 발사체 생성
    Dictionary<int, Transform[]> UnitCastDummys = new Dictionary<int, Transform[]>();
    public GameObject SpawnProjectile(SkillTables.ProjectTileInfo _projecttile, AbilityData ability, int _Damage, int teamIndex, Vector3 dir, Unit owner, Unit target, Vector3 spawnLoaction, Quaternion spawnRotation, ulong ProjectTileId, bool normal = false)
    {
        if (_projecttile == null)
            return null;

        Transform go = PoolManager.Pools["Orgprojectile"].Spawn("Projectile");

        NetProjectile projectile = null;
        if (go == null)
            projectile = ResourceMgr.InstAndGetComponent<NetProjectile>("GameInfo/Projectile");
        else
            projectile = go.GetComponent<NetProjectile>();

        if(_projecttile.Type == 0 )
        {
            //발사체
            bool bFindcastDummy = false;
            //< 생성되는 위치가 따로 필요하다면 해당위치로 대입
            if (_projecttile.startType == 1 && _projecttile.castDummy != "")
            {
                if (!UnitCastDummys.ContainsKey(owner.GetInstanceID()))
                    UnitCastDummys.Add(owner.GetInstanceID(), owner.GetComponentsInChildren<Transform>(true));

                for (int i = 0; i < UnitCastDummys[owner.GetInstanceID()].Length; i++)
                {
                    if (UnitCastDummys[owner.GetInstanceID()][i] != null && _projecttile.castDummy.Contains(UnitCastDummys[owner.GetInstanceID()][i].name))
                    {
                        projectile.transform.position = UnitCastDummys[owner.GetInstanceID()][i].position;
                        bFindcastDummy = true;
                        break;
                    }
                }
            }

            if (!bFindcastDummy)
            {
                //< 중앙에서 발사하도록 처리
                //BoxCollider collide = owner.GetComponent<BoxCollider>();
                //float height = (collide != null ? (collide.bounds.max.y / 2) : 1);

                //if (height < 0)
                //    height *= -1;

                //spawnLoaction.y += height;
                //projectile.transform.position = spawnLoaction;

                //그냥 원점에서 나가게 처리
                //spawnLoaction.y += _projecttile.yLocation;
                projectile.transform.position = spawnLoaction;
                //projectile.transform.position = new Vector3(spawnLoaction.x, spawnLoaction.y + _projecttile.yLocation , spawnLoaction.z);
            }

            if (_projecttile.startType == 3)
                projectile.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            else
                projectile.transform.rotation = spawnRotation;

            //< 데이터 대입
            projectile.GetComponent<NetProjectile>().Setup(_projecttile, ability, _Damage, teamIndex, dir, owner, target, ProjectTileId,  normal);

            //< 레이어 설정
            NGUITools.SetChildLayer(projectile.transform, owner.gameObject.layer);
        }
        else if (_projecttile.Type == 1)
        {
            //바로앞에 
            Vector3 targetPos = owner.transform.position + ((owner.transform.forward) * _projecttile.startLocation);

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(targetPos, out navHit, Vector3.Distance(targetPos, transform.position), 9))
                targetPos = navHit.position;

            projectile.transform.position = targetPos;
            projectile.transform.rotation = spawnRotation;

            //< 데이터 대입
            projectile.GetComponent<NetProjectile>().Setup(_projecttile, ability, _Damage, teamIndex, dir, owner, target, ProjectTileId, normal);

            //< 레이어 설정
            NGUITools.SetChildLayer(projectile.transform, owner.gameObject.layer);
        }

        return projectile.gameObject;
    }

    #endregion

    public Transform GetSpawnTransform(eTeamType teamType)
    {
        Transform trans = null;
        if (teamType == eTeamType.Team1)
            trans = team1StartTrans;
        else if (teamType == eTeamType.Team2)
            trans = team2StartTrans;

        return trans;
    }

    /// 현재 NavMesh의 시작 -> 끝 위치를 가지는 그룹을 리턴해준다.
    public NavigationPosGroup CurNavMeshGroup()
    {
        return navPosGroups[Mathf.Clamp(navGroupIndex, 0, navPosGroups.Count - 1)];
    }

    public NavigationPosGroup EndNavMeshGroup()
    {
        return navPosGroups[navPosGroups.Count - 1];
    }
    public void NextNavMeshGroup()
    {
        navGroupIndex = Mathf.Clamp(++navGroupIndex, 0, navPosGroups.Count - 1);
    }

    /// 선택한 유닛을 컨트롤 가능하도록 한다.
    public GKUnitController StartLookAndControl(Unit target)
    {
        if (null == target)
            return null;

        CameraManager.instance.Follow(target.transform);

        GKUnitController ctlr = target.GetComponent<GKUnitController>();
        if (null == ctlr)
            ctlr = target.gameObject.AddComponent<GKUnitController>();

        //ctlr.StartJoystick(CameraManager.instance.mainCamera);

        // 인풋에 따른 제어 설정
        return ctlr;
    }
    
    /// <summary>
    /// 타임 업데이트 용 단순히 시간만 업데이트한다
    /// </summary>
    public virtual void PlayTimeUpdate()
    {
        if (TimeLimit <= 0)
            return;

        PlayTime += Time.deltaTime;
        //float time = base.PlayTimeUpdate();
        float time = Mathf.Round(PlayTime - 0.5f);
        time = TimeLimit - time;

        if (time < 0)// 타임 어택
        {
            if (GameMode == GAME_MODE.SPECIAL_EXP || GameMode == GAME_MODE.SPECIAL_GOLD)
                EndGame(true);//게임 끝
            else
                EndGame(false);//게임 끝

            return;
        }

        if (HudPanel != null)
            HudPanel.TimeUpdate(time);

        //PlayTime += Time.deltaTime;
        //return Mathf.Round(PlayTime - 0.5f);
    }

    void OnClickRevive()
    {
        HudPanel.Revive();
        G_GameInfo.GameInfo.RevivePlayer();
        //< 부활했을시 처리
        EventListner.instance.TriggerEvent("Revive");

        //UIBasePanel panel = UIMgr.GetHUDBasePanel();
        //if (panel != null)
        //    panel.Show();

        SoundManager.instance.PlaySfxSound(eUISfx.UI_revival, false);
        SceneManager.instance.CurrentStateBase().PlayMapBGM(Application.loadedLevelName);
    }

    /// <summary> 골드 획득시 연출할 것. </summary>
    public virtual void AddGold(Unit unit, float value)
    {
        //골드획득연출삭제, 코인만올라가게
        //EventListner.instance.TriggerEvent("COIN_LOOTING", value);

        if (unit is Boss)
        {
            Transform leaderTrn = G_GameInfo.PlayerController.Leader.transform;
            for (int i = 0; i < 5; i++)
                DropItem.DropAssets(unit.transform, leaderTrn, i, (value * 0.1f), true);

            int itemCnt = Random.Range(2, 5);

            for (int i = 0; i < itemCnt; i++)
                DropItem.DropItems(unit.transform, leaderTrn, i, i == 0 ? DropItem.DropType.treasureChest : DropItem.DropType.WoodBox, true);
        }
        else if (unit is Npc)
        {
            if ((unit as Npc).isMiddleBoss)
            {
                for (int i = 0; i < 5; i++)
                    DropItem.DropAssets(unit.transform, G_GameInfo.PlayerController.Leader.transform, i, (value * 0.2f), true);
            }
            else
                DropItem.DropAssets(unit.transform, G_GameInfo.PlayerController.Leader.transform, 0, (value));
        }
        else if (unit is Prop)
        {
            DropItem.DropAssets(unit.transform, G_GameInfo.PlayerController.Leader.transform, 0, (value));
        }


    }

    public virtual void OpenResultPanel(params object[] param)
    {
        bool isSuccess = (bool)param[0];
        if (IsEndForced)//바로 종료
        {
            //if (!isSuccess && UIMgr.instance.CurTutorial == TutorialType.CHAPTER)
            //    UIMgr.instance.CurTutorial = TutorialType.CLICK_CHAPTER;
            
            NetData.instance.ClearRewardData();
            TempCoroutine.instance.StopAllCoroutines();
            
            if(IsEndForcedMap)
            {
                //스테이지선택버튼?
                _STATE state = _STATE.SINGLE_GAME;
                if (GameMode == GAME_MODE.RAID)
                    state = _STATE.RAID_GAME;
                else if (GameMode == GAME_MODE.TOWER)
                    state = _STATE.TOWER_GAME;
                else if (GameMode == GAME_MODE.ARENA)
                    state = _STATE.ARENA_GAME;
                else if (GameMode == GAME_MODE.FREEFIGHT)
                {
                    if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                    {
                        state = _STATE.FREEFIGHT_GAME;
                    }
                    else if (FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM)
                    {
                        state = _STATE.COLOSSEUM;
                    }
                    else
                    { 
                        state = _STATE.MULTI_RAID;
                    }                    
                }

                IsEndForcedMap = false;
                SceneManager.instance.LobbyActionEvent(state, _ACTION.GO_MAP, UI_OPEN_TYPE.NONE);
            }
            else
            {
                SceneManager.instance.LobbyActionEvent(_STATE.SINGLE_GAME, _ACTION.GO_TOWN, UI_OPEN_TYPE.NONE);
            }


            return;
        }

		// for test

        if (GameMode == GAME_MODE.SINGLE)
        {
            if ((this as SingleGameInfo).UiState == 2)
            {
                if (isSuccess)
                {//클라이언트가 먼저 실행됬다.

                    //if (GameMode == GAME_MODE.SINGLE && (this as SingleGameInfo).StageId == 101 && UIMgr.instance.CurTutorial == TutorialType.CHAPTER)//튜토리얼 챕터 클리어 강제로나갈 수도 있으니 저장시켜준다.
                    //    PlayerPrefs.SetInt(string.Format("Tutorial{0}", NetData.instance.GetUserInfo()._charUUID), (int)TutorialType.CHAPTER_REWARD);

                    Quest.QuestInfo questInfo = QuestManager.instance.CheckSubQuest(QuestSubType.SINGLEGAMECLEAR, (this as SingleGameInfo).StageId);
                    if (questInfo != null && 0 < questInfo.QuestTalkSceneID)
                    {
                        (this as SingleGameInfo).IsQuestClear = true;
                        UIMgr.OpenMissionPanel(questInfo.ID);
                    }

                    return;
                }
            }
            else if((this as SingleGameInfo).UiState == 1)
            {
                if (0 < (this as SingleGameInfo).QuestTalkId)
                {
                    ///clear 컷씬이 있으면 missionPanel (캐릭터간 대화창 퀘스트)를 열지않고 컷씬으로 대체한다.
					if (GameObject.Find("CinemaSceneManager") != null)
                    {
                        CinemaSceneManager csm = GameObject.Find("CinemaSceneManager").GetComponent<CinemaSceneManager>();

                        uint questID = (this as SingleGameInfo).QuestTalkId;
                        if (csm.getCinemaSceneStartType(questID) == CinemaSceneManager.StartType.AFTER_CLEAR)
                        {
                            int startSeqIdx = csm.getCinemaSceneStartingSeqIndex(questID);
                            int endSeqIdx = csm.getCinemaSceneEndingSeqIndex(questID);
                            CutSceneMgr.StartCinemaScene(false, startSeqIdx, endSeqIdx, () => {
                                QuestManager.instance.QuestComplet(questID);
                                //UIMgr.OpenMissionPanel((this as SingleGameInfo).QuestTalkId);
                            });
                            return;
                        }
                    }
                    // 없으면 missionPanel연다. 
                    UIMgr.OpenMissionPanel((this as SingleGameInfo).QuestTalkId);
                    return;
                }
            }
        }
        
        UIMgr.Open("UIPanel/ResultRewardStarPanel", param);
        if (!UIMgr.instance.UICamera.enabled)
            UIMgr.instance.UICamera.enabled = true;

        if (GameMode == GAME_MODE.TOWER && isSuccess)
        {
            DungeonTable.TowerInfo towerInfo = _LowDataMgr.instance.GetLowDataTower( (this as TowerGameInfo).StageId);
            NetworkClient.instance.SendPMsgTowerUseTimeQueryC(towerInfo.level);//최단시간 클리어 타임 받아오기
            NetworkClient.instance.SendPMsgTowerRankQueryC(towerInfo.level);//현재 랭킹 받아오기
        }
    }
}
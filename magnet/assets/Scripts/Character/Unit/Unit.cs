using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary></summary>
/// <remarks>
/// ==== 기본 캐릭터 생성 방식 ====
/// 
/// 1. Root 게임오브젝트 생성 (껍데기만 준비된 프리팹 로드)
/// 
///     - Mesh Model 생성
///         - 애니메이션 추가
///         - 그림자 추가
///     
///     - FSM 상태머신 설정
///     
///     - enum으로 애니메이션 플레이를 위한 AnimTable 설정
/// </remarks>


public class Unit : MonoBehaviour
{
    public ulong m_rUUID = 0;           //실시간 네트워크 모드 Room에 들어갔을 때 구분하기 위한 UUID  //네트웍에서 채널(방)별로 발급되는 UUID
    public uint SpwanUnitId = 0;
    public static bool ShowUnitDebugView = false;
    public bool showDebug = false;

    /// 기본적은 Data들 모두 초기화 완료했는지 여부
    public bool InitDone = false;
    public Transform cachedTransform;

    //< 자신을 포함하여 모델까지의 모든 트랜스폼
    Transform[] _Transforms;
    public Transform[] Transforms
    {
        get
        {
            if (_Transforms == null)
                _Transforms = new Transform[1];

            return _Transforms; 
        }
        set
        {
            _Transforms = value;
        }
    }

    /// 현 객체의 유닛종류
    public UnitType UnitType { set; get; }

    /// 속한 팀 번호 (아군 적군 판별용). Inspector에서 볼려고 Property속성안함
    byte _TeamID;
    public byte TeamID
    {
        get
        {
            return _TeamID;
        }
        set{_TeamID = value;}
    }

    //public Vector2 Position;

    /// 유닛들 그룹화 관리를 위한 변수
    public int GroupNo { protected set; get; }

    /// 현재 선택되어 있는 스테이트
    public UnitState CurrentState { set; get; }

    public Animation Animation { 
        get {
            if (!Animator.IsReady)
                return null;
            return Animator.Animation;
        } 
    }

    /// 현재 스턴상태인지 여부
    public bool IsStun  { get { return CurrentState == UnitState.Stun; } }

    /// 현재 캐릭터가 사용가능한지 (죽었는지 등등)
    public bool Usable
    {
        get 
        {
            if (CharInfo == null || gameObject == null || FSM == null)
                return false;
            //if(UnitType == UnitType.Unit)
            //    Debug.LogWarning("2JW : " + gameObject.activeSelf + " : " + CharInfo.IsDead + " : " + FSM.IsEnable + " : " + (CurrentState != UnitState.Dead));
            return !CharInfo.IsDead && gameObject.activeSelf && FSM.IsEnable && CurrentState != UnitState.Dead; 
        }
    }

    //< 죽었는지 여부
    public bool IsDie
    {
        get
        {
            if (CharInfo.IsDead || CurrentState == UnitState.Dead || CurrentState == UnitState.Dying)
                return true;

            return false;
        }
    }

    public void SetObstacleAvoidance(bool isEnable)
    {
        if (null != navAgent)
        {
            if(G_GameInfo.GameMode == GAME_MODE.SINGLE || G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
            {
                if (isLeader)
                    navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
                else
                    navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }
            else
            {
                navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }
        }
    }

    /// 현재 NavAgent가 사용가능한지 여부
    public bool UsableNavAgent { get { return navAgent != null && navAgent.enabled; } }

    /// 유닛의 자체 유효 영역 (NavMeshAgent.Radius * Scale.Max)
    public float Radius { set { radius = value; } get { return radius; } }
    public float Height;

    public float SuperRecoveryTick = 0;

    public int CurCombo
    {
        set { curComboNo = value != 0 ? curComboNo = (int)Mathf.Repeat(value, CharInfo.MaxAnimCombo) : curComboNo = value; }
        get { return curComboNo; }
    }

    public bool PreEndAttackAnim = false;

    /// 내가 액션을 취해야할 대상 (Unit InstanceID)
    public int TargetID
    {
        set { targetID = value; }
        get { return targetID; }
    }

    public float ChainEndTime;
    public bool isChainTime()
    {
        if (ChainEndTime != float.MaxValue && ChainEndTime >= Time.time)
        {
            return true;
        }

        return false;
    }
    public int ChainLevel = 0;
    public bool ChainSkill = false;

    //< 강제로 일점사할 대상
    Unit _ForcedAttackTarget;
    public Unit ForcedAttackTarget
    {
        get 
        {
            if (_ForcedAttackTarget == null || !_ForcedAttackTarget.Usable)
                _ForcedAttackTarget = null;

            return _ForcedAttackTarget; 
        }
        set
        {
            _ForcedAttackTarget = value;
            SetTarget(_ForcedAttackTarget.GetInstanceID());
        }
    }


    /// 현 객체 소유주
    public PlayerController Owner { set; get; }

    /// 인게임에 필요한 캐릭터 정보들
    public GameCharacterInfo        CharInfo;

    public bool                     SkillBlend = false;
    public bool                     MoveableSkill = false;

    public uint                     UseSkillSlot;
    public uint                     UseSkillIdx;

    public uint                     ReservedSkillIdx = 0;
    public float                    ReservedSkillTime = 0f;


    public int                      MoveToSkillSlot;
    public SkillController          SkillCtlr;
    public BuffController           BuffCtlr;

    public GKUnitController         inputCtlr;
    public Skill_AI                 skill_AI;

    public FSM.FSM<UnitEvent, UnitState, Unit>   FSM;

    public NavMeshAgent             navAgent;

    public AudioSource              _audioSource;
    protected float                 radius;

    BoxCollider                     _UnitCollider;
    public BoxCollider              UnitCollider
    {
        get
        {
            if(_UnitCollider == null)
                _UnitCollider = this.gameObject.GetComponent<BoxCollider>();

            return _UnitCollider;
        }

        set
        {
            _UnitCollider = value;
        }
    }
    
    [HideInInspector]
    public GameObject               shadowGO;

    protected NavMeshPath           movePath = new NavMeshPath();
    protected int                   movePathIndex = 0;

    // 타겟팅을 위한 (Unit InstanceID)
    private int                 targetID = GameDefine.unassignedID;

    // 콤보 카운팅을 위한
    private int                 curComboNo;

    //< 현재 어택 중인지
    public bool                 ActiveAttack;    

    // 피격 이펙트 (깜박임)
    private float               flickeringStartT= 0;
    private float               flickeringTime = 0.3f;
    //private Transform           prevEffect;
    
    // 모델 흔들기 이펙트
    private IEnumerator         shakeRoutine;

    public UnitModelObject      Model;
    public UnitAnimator         Animator;

    public List<Transform>      ExtraModels;

    // 리더를 위한
    public DirectionIndicator   dirIndicator;

    public int AttackType;      //< 어택 타입( 1 : 근접, 2 : 원거리 , 3 : 원거리(마법) )

    #region :: DamageSync ::

    private Queue<Sw.PMsgRoleAttackRecvS> _damageQueue = new Queue<Sw.PMsgRoleAttackRecvS>();  //멀티플레이 데미지 표시를 위한 데미지큐
    public int DamageQueueReset()
    {
        int remainData = _damageQueue.Count;
        _damageQueue.Clear();

        return remainData;
    }

    public void DamageEnqueue(Sw.PMsgRoleAttackRecvS attackData)
    {
        _damageQueue.Enqueue(attackData);
    }

    public Sw.PMsgRoleAttackRecvS DamageDequeue()
    {
        if (_damageQueue.Count > 0)
        {
            return _damageQueue.Dequeue();
        }

        return null;
    }
    #endregion

    #region :: MonoBehaviour ::
    
    [HideInInspector] public int m_SvComboIdx = -1;

    public virtual void Awake() { }
    public virtual void Start() { }

    public int BeforePosX;
    public int BeforePosY;

    //void Update()

    float recentSendTime = 0.3f;
    void FixedUpdate()
    {
        if (FreeFightGameState.StateActive || TownState.TownActive)
        {
            //이전의 좌표와 현재의 좌표를 비교하여 달라졌다면 이동메세지를 보낸다
            if (m_rUUID == NetData.instance._userInfo._charUUID)
            {
                //recentSendTime = recentSendTime - Time.deltaTime;
                MoveNetwork(dest_back);

                ////if (recentSendTime <= 0)
                //{
                //    recentSendTime = 0.3f;

                //    Vector3 MinPos = NaviTileInfo.instance.GetMinPos();
                //    float PointX = Mathf.Abs(MinPos.x - transform.position.x) / NaviTileInfo.instance.GetTileSize();
                //    float PointY = Mathf.Abs(MinPos.z - transform.position.z) / NaviTileInfo.instance.GetTileSize();

                //    //if (NaviTileInfo.instance.GetMoveablePos((int)PointX, (int)PointY))
                //    {
                //        //if (BeforePosX == (int)PointX && BeforePosY == (int)PointY)
                //        //{

                //        //}
                //        //else
                //        {
                //            if (Mathf.Abs(BeforePosX - (int)PointX) > 1 || Mathf.Abs(BeforePosY - (int)PointY) > 1)
                //            {
                //                //많이크면 Astar로 타일 순서를 찾자
                //                List<PathVertex> path = AStarMover.instance.CalculatePath(new Vector2(BeforePosX, BeforePosY), new Vector2((int)PointX, (int)PointY));

                //                //다수의 칸이동
                //                if (FreeFightGameState.StateActive)
                //                {
                //                    NetworkClient.instance.SendPMsgBattleMapMoveCS(path);
                //                }
                //                else
                //                {
                //                    NetworkClient.instance.SendPMsgMapMoveCS(path, transform.position.x, transform.position.z);
                //                }

                //                BeforePosX = (int)PointX;
                //                BeforePosY = (int)PointY;
                //            }
                //            else
                //            {
                //                //한칸만 이동
                //                if (FreeFightGameState.StateActive)
                //                {
                //                    NetworkClient.instance.SendPMsgBattleMapMoveCS((int)PointX, (int)PointY);
                //                }
                //                else
                //                {
                //                    NetworkClient.instance.SendPMsgMapMoveCS((int)PointX, (int)PointY, transform.position.x, transform.position.z);
                //                }

                //                BeforePosX = (int)PointX;
                //                BeforePosY = (int)PointY;
                //            }
                //        }
                //    }
                //}

                //Vector3 MinPos = NaviTileInfo.instance.GetMinPos();
                //float PointX = Mathf.Abs(MinPos.x - transform.position.x) / NaviTileInfo.instance.GetTileSize();
                //float PointY = Mathf.Abs(MinPos.z - transform.position.z) / NaviTileInfo.instance.GetTileSize();

                //if (NaviTileInfo.instance.GetMoveablePos((int)PointX, (int)PointY))
                //{
                //    if (BeforePosX == (int)PointX && BeforePosY == (int)PointY)
                //    {

                //    }
                //    else
                //    {
                //        if (Mathf.Abs(BeforePosX - (int)PointX) > 1 || Mathf.Abs(BeforePosY - (int)PointY) > 1)
                //        {
                //            //많이크면 Astar로 타일 순서를 찾자
                //            List<PathVertex> path = AStarMover.instance.CalculatePath(new Vector2(BeforePosX, BeforePosY), new Vector2((int)PointX, (int)PointY));

                //            //다수의 칸이동
                //            if (FreeFightGameState.StateActive)
                //            {
                //                NetworkClient.instance.SendPMsgBattleMapMoveCS(path, dest_back.x, dest_back.z);
                //            }
                //            else
                //            {
                //                NetworkClient.instance.SendPMsgMapMoveCS(path, dest_back.x, dest_back.z);
                //            }

                //            BeforePosX = (int)PointX;
                //            BeforePosY = (int)PointY;
                //        }
                //        else
                //        {
                //            //한칸만 이동
                //            if (FreeFightGameState.StateActive)
                //            {
                //                NetworkClient.instance.SendPMsgBattleMapMoveCS((int)PointX, (int)PointY, dest_back.x, dest_back.z);
                //            }
                //            else
                //            {
                //                NetworkClient.instance.SendPMsgMapMoveCS((int)PointX, (int)PointY, dest_back.x, dest_back.z);
                //            }

                //            BeforePosX = (int)PointX;
                //            BeforePosY = (int)PointY;
                //        }
                //    }
                //}
            }
        }

        if (G_GameInfo._GameInfo == null)
            return;

    }

    void Update()
    {
        if (!TownState.TownActive)
        {
            if (!SceneManager.instance.IsRTNetwork || (SceneManager.instance.IsRTNetwork && (m_rUUID == NetData.instance._userInfo._charUUID)))
            {
                if (G_GameInfo.GameInfo != null  && ChainEndTime != float.MaxValue)
                {
                    if (isChainTime())
                    {
                        //아직 체인가능한시간이다
                        if (m_rUUID == NetData.instance._userInfo._charUUID)
                        {
                            if (G_GameInfo.GameInfo.HudPanel != null)
                                G_GameInfo.GameInfo.HudPanel.ChangeAttackImg(1, SkillCtlr.GetSkillIndex(5), SkillCtlr.SkillList[5].GetSkillActionInfo().callChainTime);
                        }
                    }
                    else
                    {
                        //체인불가시간 리셋하자
                        ChainEndTime = float.MaxValue;

                        if (m_rUUID == NetData.instance._userInfo._charUUID)
                        {
                            if (G_GameInfo.GameInfo.HudPanel != null)
                                G_GameInfo.GameInfo.HudPanel.ChangeAttackImg(0, 0, 0);
                        }
                        //스킬 아이콘 변경
                    }
                }
            }

            //슈퍼아머 관련 연산은 멀티가 아닌경우만 - 멀티일시 서버가
            if (!SceneManager.instance.IsRTNetwork)
            {
                if(FSM != null)
                {
                    if (!IsDie)
                    {
                        if (SuperRecoveryTick > 0)
                        {
                            //틱이 활성화 된경우만
                            SuperRecoveryTick -= Time.deltaTime;

                            if (SuperRecoveryTick <= 0)
                            {
                                //시간이 다되었으면 회복
                                Transform effTrans = G_GameInfo.SpawnEffect("Fx_SuperArmor_Apply", cachedTransform.position, Quaternion.Euler(Vector3.zero));

                                if(effTrans != null)
                                {
                                    FxMakerPoolItem poolItem = effTrans.GetComponent<FxMakerPoolItem>();
                                    if (poolItem == null)
                                    {
                                        poolItem = effTrans.gameObject.AddComponent<FxMakerPoolItem>();
                                        poolItem.destroyTime = 10;
                                    }

                                    poolItem.Owner = this;
                                    poolItem.SetAttach(transform);
                                }

                                SoundManager.instance.PlaySfxUnitSound(0, _audioSource, cachedTransform, true);

                                CharInfo.SuperArmor += ((uint)((float)CharInfo.MaxSuperArmor * (CharInfo.SuperArmor_RecoveryRate * 0.01f))) + (uint)CharInfo.SuperArmor_Recovery;
                                CharInfo.SuperArmor = (uint)Mathf.Clamp(CharInfo.SuperArmor, 0, CharInfo.MaxSuperArmor);
                                SuperRecoveryTick = 0f;
                            }
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        if (m_rUUID == NetData.instance._userInfo._charUUID && !TownState.TownActive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                ManualAttack();
            }

            if (CharInfo.CharIndex != 13000)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    UseSkill(1, true);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    ChargeStart();
                if (Input.GetKeyUp(KeyCode.Alpha1))
                    ChargeEnd();

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
                UseSkill(2, true);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                UseSkill(3, true);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                UseSkill(4, true);
        }
#endif
    }

    protected virtual void OnEnable()
    {
        // active false -> true로 변경시, 애니메이션 재생안되는 문제 해결
        if (null != Animator && Animator.IsReady)
        {
            Animator.Animation.enabled = false;
            Animator.Animation.enabled = true;
        }
    }

    protected virtual void OnDisable() {}

    protected virtual void OnDestroy()  
    {
        if(BuffCtlr != null)
            BuffCtlr.AllBuffDestroy();

    }

    #endregion

    #region :: Initialize ::

    /// 작동을 위한 초기화의 시작!
    public void Init(params object[] args)
    {
        Model = new UnitModelObject();
        Animator = new UnitAnimator();

        cachedTransform = this.transform;
        Init_SyncData(args);

        Init_FSM();

        Init_Datas();

        Init_Controllers();

        Init_Model();

		Init_Part2 ();
    }

	public IEnumerator InitAsync(params object[] args)
	{
		Model = new UnitModelObject();
		Animator = new UnitAnimator();
		
		cachedTransform = this.transform;
		Init_SyncData(args);
		
		Init_FSM();
		
		Init_Datas();
		
		Init_Controllers();
		
		yield return StartCoroutine (Init_ModelAsync ());
		
		Init_Part2 ();
	}

	void Init_Part2(){
		//공속적용을 위해 한번ATKSpeed를 가져옴
		float atkspeed = CharInfo.AtkSpeed;
		
		SetupComponents(); //NaviAgent 추가와 라이트 추가
		
		// 생성된 유닛 정보 저장
		if (Owner != null && UnitType != global::UnitType.Trap)
			Owner.AddUnit( this );
		
		if (G_GameInfo.CharacterMgr != null && UnitType != global::UnitType.Trap)
			G_GameInfo.CharacterMgr.AddUnit( this );
		
		InitDone = true;
		
		LoadingComplete();
	}

	public void SetTrailRenderer(GameObject tu, bool b){
		//if (tu.transform.GetComponentInChildren<TrailRenderer> () != null) {
		TrailRenderer[] tr = tu.transform.GetComponentsInChildren<TrailRenderer> (true);
		for (int i = 0; i < tr.Length; i++)
		{
			tr[i].enabled = b;
			tr[i].time = 0f;
			 	
				//tr[i].gameObject.name = "trail_1";
				//tr[i].gameObject.SetActive(false);
		}
		//}
	}

	/// 받아온 데이터들을 셋팅하도록 한다.
    protected virtual void Init_SyncData(params object[] args)
    {
        if (Owner == null)
        {
            if (this is Pc)
            {
                //ulong controllerID = (ulong)args[0];
                // 수정해야함
                if(TeamID != (byte)eTeamType.Team2)
                    Owner = G_GameInfo.PlayerController;
            }            
        }

        TeamID = (byte)args[1];
        UnitType = (UnitType)args[2];
    }

    /// 필요한 FSM를 추가하도록 한다. 상속받아서 호출하되 Base함수 필요없으면, 호출하지 않도록 한다.
    protected virtual void Init_FSM()
    { }

    /// 현 객체 필요한 데이터들을 설정한다.
    protected virtual void Init_Datas()
    {
        CharInfo = new GameCharacterInfo();
        CharInfo.Init( this );

        if (SceneManager.instance.IsRTNetwork)
        {
            float a_value = CharInfo.Stats[AbilityType.HP].FinalValue; 
            if (a_value < 0 || a_value > int.MaxValue * 0.9f)
                a_value = 10;
            CharInfo.Hp = CharInfo.MaxHp = (int)a_value;

            CharInfo.MaxSuperArmor = (uint)CharInfo.Stats[AbilityType.SUPERARMOR].FinalValue;
        }
    }

    /// 어셋번들로 부터 메쉬를 로드하면됨.
    protected virtual void Init_Model()
    { }

	protected virtual IEnumerator Init_ModelAsync()
	{ 
		yield return null;
	}

    /// 스킬 및 버프관련 컨트롤러 초기화
    protected virtual void Init_Controllers()
    {
        if (UnitType == global::UnitType.Prop)
            return;

        if (BuffCtlr == null)
            BuffCtlr = G_GameInfo.CharacterMgr.AddBuffCtrl(this);

        if (SkillCtlr == null)
            SkillCtlr = G_GameInfo.CharacterMgr.AddSkillCtrl(this);
    }

    /// 객체 구성에 필요한 기본 컴포넌트들 셋팅하기
    protected virtual void SetupComponents()
    {
        UnitModelHelper.SetupStandardComponent(transform);

        navAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();

		//매터리얼에서 넣어주는게 좋은데...
        SkinnedMeshRenderer[] SMR = Model.GetSkinnedMeshRenders();

        Texture noiseTex = ResourceMgr.Load("Etc/Noise") as Texture;

        for (int i = 0; i < SMR.Length; i++)
            for (int j = 0; j < SMR[i].materials.Length; j++)
                SMR[i].materials[j].SetTexture("_DisolveTexture", noiseTex);        
    }

    ///// 컴포넌트 비례해서 재계산이 필요한 값들을 계산하는 함수. (현 게임에서는 실모델 로드되고 나서 호출필요)
    protected virtual void ComputeComponents()
    {
	   SetAnimationSpeed();
    }

    public void SetAnimationSpeed()
    {
    //    //< 애니메이션 스피드 설정
    //    AnimationUtil.SetAnimationSpeed(Animator.Animation, CharInfo.AtkSpeed, CharInfo.GetAttackAnims());

        //< 애니메이션을 컬링 없이 무조건 재생되도록 처리
        //Animation.cullingType = AnimationCullingType.AlwaysAnimate;
    }

    protected virtual void SetupForGameMode(GAME_MODE gameMode)
    {        
    }

    /// Unit의 모든 준비가 끝나고 나서 불려야 될 함수. (여러군데서 불릴수 있음. 비동기 로딩하는 곳이 있기 때문에)
    /// 비동기 로드를 사용하는 부분도 존재할수 있기 때문에, 알맞은 곳에서 호출되도록 해준다.
    protected virtual bool LoadingComplete()
    {
        if (!InitDone || !Model.IsReady)
            return false;

        ComputeComponents();                        // navAgent,  BoxCollider  사이즈를 재조정 해 준다. 
        SetupForGameMode( G_GameInfo.GameMode );

        if (null != Owner)
            Owner.CheckLoadingComplete();           // 모든 유닛들이 로드가 완료되었다고 GameInfoBase에 알려준다.

        //< 체력 저장
        //hp = CharInfo.Hp;

        //< 피격당했을시 처리를 위해 미리 로드해놓음
        //if(UnitType != global::UnitType.Boss)
        SMRs = UnitEffectHelper.GetHasProperty(Model.Main, "_FlashAmount");

        return true;
    }

    public GameObject LightObj;
    protected void AddLight()
    {
        ////< 지역에 따라 처리
        //if(G_GameInfo.GameMode == GAME_MODE.SINGLE)
        //    LightObj = Instantiate(Resources.Load("Light/UnitLight_" + (G_GameInfo._GameInfo as SingleGameInfo).StageInfo.property.ToString())) as GameObject;
        //else
            LightObj = Instantiate(Resources.Load("Light/UnitLight_1")) as GameObject;

        //if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
        //{
        //    if((G_GameInfo._GameInfo as SingleGameInfo).StageInfo.AreaType == 3)
        //        LightObj.GetComponent<Light>().color = new Color32(6, 55, 75, 255);
        //}
        //else if (G_GameInfo.GameMode == GAME_MODE.PVP)
        //{
        //    if(TeamID == 0)
        //        LightObj.GetComponent<Light>().color = new Color32(7, 49, 69, 255);
        //    else
        //        LightObj.GetComponent<Light>().color = new Color32(51, 13, 13, 255);
        //}
        //else if(G_GameInfo.GameMode == GAME_MODE.RAID)
        //{
        //    if ((G_GameInfo._GameInfo as RaidGameInfo).areaType == 3)
        //        LightObj.GetComponent<Light>().color = new Color32(50, 50, 50, 255);
        //}
        
        LightObj.transform.parent = this.transform;
        LightObj.transform.localPosition = new Vector3(0, 1.1f, 0);

        if(QualityManager.instance.GetQuality() == QUALITY.QUALITY_MID || QualityManager.instance.GetQuality() == QUALITY.QUALITY_HIGH )
        {
            LightObj.SetActive(true);
        }
        else
        {
            LightObj.SetActive(false);
        }

        float CharLightValue;
        float ShadowStrength;

        _LowDataMgr.instance.GetMapShadowData(Application.loadedLevelName, out CharLightValue, out ShadowStrength);

        //라이트셋팅
        if (LightObj != null)
        {
            Light li = LightObj.GetComponent<Light>();
            li.intensity = CharLightValue;
        }

    }

    #endregion

    #region :: Logic ::

    #region :: Move ::

    public void MoveNetworkCalibrate(Vector3 Dest)
    {
        dest_back = Dest;
        MoveNetwork(Dest);
    }

    public void MoveNetwork(Vector3 Dest)
    {
        if (FreeFightGameState.StateActive || TownState.TownActive)
        {
            //이전의 좌표와 현재의 좌표를 비교하여 달라졌다면 이동메세지를 보낸다
            if (m_rUUID == NetData.instance._userInfo._charUUID)
            {

                Vector3 MinPos = NaviTileInfo.instance.GetMinPos();
                float PointX = Mathf.Abs(MinPos.x - Dest.x) / NaviTileInfo.instance.GetTileSize();
                float PointY = Mathf.Abs(MinPos.z - Dest.z) / NaviTileInfo.instance.GetTileSize();

                if (NaviTileInfo.instance.GetMoveablePos((int)PointX, (int)PointY))
                {
                    if (BeforePosX == (int)PointX && BeforePosY == (int)PointY)
                    {

                    }
                    else
                    {
                        if (Mathf.Abs(BeforePosX - (int)PointX) > 1 || Mathf.Abs(BeforePosY - (int)PointY) > 1)
                        {
                            //많이크면 Astar로 타일 순서를 찾자
                            List<PathVertex> path = AStarMover.instance.CalculatePath(new Vector2(BeforePosX, BeforePosY), new Vector2((int)PointX, (int)PointY));

                            //다수의 칸이동
                            if (FreeFightGameState.StateActive)
                            {
                                NetworkClient.instance.SendPMsgBattleMapMoveCS(path, Dest.x, Dest.z);
                            }
                            else
                            {
                                NetworkClient.instance.SendPMsgMapMoveCS(path, Dest.x, Dest.z);
                            }

                            BeforePosX = (int)PointX;
                            BeforePosY = (int)PointY;
                        }
                        else
                        {
                            //한칸만 이동
                            if (FreeFightGameState.StateActive)
                            {
                                NetworkClient.instance.SendPMsgBattleMapMoveCS((int)PointX, (int)PointY, Dest.x, Dest.z);
                            }
                            else
                            {
                                NetworkClient.instance.SendPMsgMapMoveCS((int)PointX, (int)PointY, Dest.x, Dest.z);
                            }

                            BeforePosX = (int)PointX;
                            BeforePosY = (int)PointY;
                        }
                    }
                }
            }
        }
    }

    Vector3 dest_back;
    public virtual bool MovePosition(Vector3 dest, float speedRatio = 1f, bool end = false)
    {
        bool success;

        if (CurrentState == UnitState.Skill)
        {
            if(MoveableSkill)
            {
                success = CalculatePath(dest, end);

                dest_back = dest;

                //if (success)
                //    MoveNetwork(dest);
            }
                        
            return false;
        }
            

        success = CalculatePath(dest, end);

        if (success && CurrentState != UnitState.Skill)
        {
            bool changeState = ChangeState( UnitState.Move );

            dest_back = dest;
            //if (changeState)
            //    ( FSM.Current() as MoveState ).moveSpeedRatio = speedRatio;

            //if (success)
            //    MoveNetwork(dest);
        }

        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
        //{
        //    m_NicName = Owner.GetPlayerNickName();  //테스트용 Unit에서 UUID를 보기 위해서...
        //}

        return success;
    }

    /// <summary>
    /// 구해진 Path따라 자신의 속도에 맞게 일정량 이동하게 한다.
    /// </summary>
    /// <param name="speedRatio">현 속도에 배속을 설정한다.</param>
    public bool MoveToPath(float speedRatio = 1.0f, bool NotLook = false)
    {
        bool sucessed = true;
        if (movePath.corners.Length > movePathIndex)
        {
            Vector3 targetPos = movePath.corners[movePathIndex];
            Vector3 curPos = cachedTransform.position;
            targetPos.y = curPos.y = 0; // x, z값은 0이고, y만 값만 존재할때 이동문제 발생 (높낮이 맵에 의한)

            Vector3 offset = targetPos - curPos;
		    float movespeed = CharInfo.MoveSpeed;// 8.5f;// CharInfo != null ? CharInfo.MoveSpeed : 8.5f;
            //Debug.LogWarning("2JW : " + Owner + " : " + movespeed + " : " + CharInfo.MoveSpeed);
            float speed = movespeed * speedRatio;

            // 기본으로 이동해야할 힘 = 방향 * 속도
            Vector3 velocity = speed * offset.normalized;

            if (GameDefine.skillPushTest)
            {
                if(IsLeader)
                {
                    //레이어 문제 일단 임시처리 시도

                    //난투장의 경우 이동시 밀치지않음
                    if(FreeFightGameState.StateActive && !( UnitType == UnitType.Unit && m_rUUID == NetData.instance._userInfo._charUUID))
                    {
                        //난투장의 경우 이동은 처리안함

                    }
                    else
                    {
                        //비난투장의 경우 
                        RaycastHit hit;
                        LayerMask mask = 1 << LayerMask.NameToLayer("Unit");
                        if (Physics.Raycast(cachedTransform.position, velocity, out hit, 1f, mask))
                        {
                            //부딛힌 대상이 파트너일경우만 무시함
                            if (hit.collider == null)
                                return false;

                            Unit hitUnit = hit.collider.gameObject.GetComponent<Unit>();
                            if (hitUnit == null || !hitUnit.IsPartner ) 
                                return false;
                        }
                    }
                }                
            }

            // 남은거리
            float destDist = offset.magnitude;
            float velDist = velocity.magnitude * Time.deltaTime;

            //Debug.Log( "MoveToPath : " + velocity.ToString( "f4" ) + " -> " + (velocity + (transform.right * speed)).ToString( "f4" ) );
            //velocity += (transform.right);

            // 이동
            if (destDist <= velDist)
                navAgent.velocity = velocity; // 목표지점의 마지막 정확한 이동거리 == offset.normalized * (destDist / velDist * speed);
            else
                navAgent.velocity = velocity;

            // 회전
            if (!NotLook)
            {
                LookAt(targetPos, Time.deltaTime * 10f * speedRatio);
            }

            int passIndex = 1;
            if (destDist <= velDist)
            {
                // 다음 지점 설정을 위한 코드 : (프레임당 이동거리)보다 (다음지점)거리가 짦다면 스킵되도록 한다.
                for (passIndex = 1; movePathIndex + passIndex < movePath.corners.Length; ++passIndex)
                {
                    // 포인트 무시해야함
                    if ((movePath.corners[movePathIndex + passIndex] - cachedTransform.position).magnitude > velDist)
                        break;
                }

                movePathIndex += passIndex;
            }//if (destDist <= velDist)
        }
        else
            sucessed = false;

        return sucessed;
    }

    /// <summary>
    /// 해당 지점까지의 네비게이션 패스 생성 함수
    /// </summary>
    /// <returns></returns>
    public bool CalculatePath(Vector3 TargetPos, bool end = false)
    {
        if ( !gameObject.activeSelf || !UsableNavAgent || float.IsNaN(TargetPos.x) || movePath == null)
            return false;

        ClearPath();

        // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 NavMesh가능 위치를 찾아준다.
        NavMeshHit navHit;
        if (NavMesh.SamplePosition( TargetPos, out navHit, Vector3.Distance( TargetPos, cachedTransform.position ), 9 ))
        {
            // 9 == Terrain
            TargetPos = navHit.position;
        }

        bool Find = navAgent.CalculatePath( TargetPos, movePath );

        // 시작점과 끝점은 계산에서 제외시킴
        for (int i = 1; i < movePath.corners.Length - 1; i++)
        {
            // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
            if (NavMesh.FindClosestEdge( movePath.corners[i], out navHit, 1 ))
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

    ////By Seo
    //public NavMeshPath m_ForLenPath = new NavMeshPath();
    ////네비 메쉬를 고려한 거리를 찾아주는 함수, 네트웍 위치 보정을 위해
    //public float GetNaviPathLength(Vector3 TargetPos)
    //{
    //    if (!UsableNavAgent || float.IsNaN(TargetPos.x))
    //    {
    //        return (TargetPos - this.transform.position).magnitude;
    //    }

    //    m_ForLenPath.ClearCorners();

    //    // NavMesh 영역 바깥 클릭인지 검사해서, 바깥이면 가장가까운 NavMesh가능 위치를 찾아준다.
    //    NavMeshHit navHit;
    //    if (NavMesh.SamplePosition(TargetPos, out navHit, Vector3.Distance(TargetPos, transform.position) + 2, 9))
    //    {
    //        // 9 == Terrain
    //        TargetPos = navHit.position;
    //    }

    //    bool Find = navAgent.CalculatePath(TargetPos, m_ForLenPath);

    //    if (Find == false)
    //    {
    //        return (TargetPos - this.transform.position).magnitude;
    //    }

    //    // 시작점과 끝점은 계산에서 제외시킴
    //    for (int i = 1; i < m_ForLenPath.corners.Length - 1; i++)
    //    {
    //        // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
    //        if (NavMesh.FindClosestEdge(m_ForLenPath.corners[i], out navHit, 1))
    //        {
    //            if ((navHit.position - m_ForLenPath.corners[i]).sqrMagnitude < 1f)
    //            {
    //                m_ForLenPath.corners[i] = m_ForLenPath.corners[i] + navHit.normal * 1f;
    //            }
    //        }
    //    }

    //    float a_Length = 0.0f;
    //    // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
    //    if (m_ForLenPath.corners.Length >= 2)
    //    {
    //        // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
    //        for (int i = 1; i < m_ForLenPath.corners.Length - 1; ++i)
    //        {
    //            if ((m_ForLenPath.corners[i] - m_ForLenPath.corners[i + 1]).sqrMagnitude < 2f)
    //                m_ForLenPath.corners[i + 1] = m_ForLenPath.corners[i];
    //        }

    //        // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
    //        for (int i = 0; i < m_ForLenPath.corners.Length - 1; ++i)
    //        {
    //            a_Length = a_Length + (m_ForLenPath.corners[i] - m_ForLenPath.corners[i + 1]).magnitude;
    //        }
    //    }
    //    else
    //    {
    //        return (TargetPos - this.transform.position).magnitude;
    //    }

    //    return a_Length;
    //}


    public void ClearPath()
    {
        movePath.ClearCorners();
        movePathIndex = 1;
    }

    #endregion

    #region :: Attack ::

    public void AttackEvent(float damageRatio)
    {
        DamageToTarget( damageRatio, CharInfo.AtkAngle );
    }

    /// 현재 공격에 대한 데미지를 타겟에게 가한다.
    void DamageToTarget(float damageRatio, float angle)
    {
        if (angle <= 0f)
        {   
            // 단일 공격
            Unit target = null;
            if (!G_GameInfo.CharacterMgr.InvalidTarget( TargetID, ref target ))
            {
                // 데미지
                target.TakeDamage( this, damageRatio, CharInfo.Atk, 0, CharInfo.AttackType, false, null);
            }
        }
        else
        {
            // 범위 공격
            foreach (KeyValuePair<byte, List<Unit>> pair in G_GameInfo.CharacterMgr.allTeamDic)
            {
                // 같은팀은 제외
                if (TeamID == pair.Key)
                    continue;

                for (int i = 0; i < pair.Value.Count; i++)
                {
                    Unit target = pair.Value[i];
                    if (!G_GameInfo.CharacterMgr.CanTarget(CharInfo.AttackType, target))
                        continue;

                    Vector3 targetDir = target.cachedTransform.position - cachedTransform.position;

                    // 정해진 타겟과 같지 않고, 공격범위에도 안들어 온다면, 타격주지않음.
                    // TODO : 수동 공격 빠져서 다시 이렇게함.
                    //if (TargetID != target.GetInstanceID() &&
                    if (!MathHelper.IsInRange(targetDir, CharInfo.AtkRange, cachedTransform.forward, angle, Radius, target.Radius))
                        continue;

                    // TODO : 수동 공격이 들어가서 다시 이렇게함.
                    //if (!MathHelper.IsInRange( targetDir, CharInfo.AtkSqrRange, transform.forward, angle, SqrRadius * 2f, navSqrMagnitude ))
                    //    continue;
                    
                    // 데미지
                    target.TakeDamage( this, damageRatio, CharInfo.Atk, 0, CharInfo.AttackType, false, null );
                }
            }
        }
    }

    /// <summary>
    /// 프로토 타입용 데미지에 의해 누적된 히트파워값.
    /// </summary>
    /// 

    /// 현 캐릭터에게 데미지를 입힌다.
    public System.Action<Unit, float> TakeDamageCallBack;
    public virtual int TakeDamage(Unit attacker, float damageRatio, float damage, float AddDamage, eAttackType atkType, bool isSkillDamage, AbilityData _ability, bool through = false, bool projecttile = false)
    {
        if (navAgent == null || !navAgent.enabled)
            return 0;
        
        if (CharInfo.IsDead)
            return 0;

        //< 무적모드 체크
        if (CharInfo.GetBuffValue(BuffType.AllImmune) > 0)
            return 0;
        else if (CharInfo.GetBuffValue(BuffType.AttackImmune) > 0 && !isSkillDamage)
            return 0;
        else if (CharInfo.GetBuffValue(BuffType.SkillImmune) > 0 && isSkillDamage)
            return 0;


        if (_ability != null)
        {
            uint beforeSuperArmor = CharInfo.SuperArmor;
            //슈퍼아머값 감소
            if (CharInfo.SuperArmor <= _ability.superArmorDmg)
            {
                CharInfo.SuperArmor = 0;

                //0일경우만 회복을 시키고 아닐경우는 무시하자 
                if (SuperRecoveryTick == 0f)
                {
                    if (beforeSuperArmor != 0)
                    {
                        Transform effTrans = G_GameInfo.SpawnEffect("Fx_SuperArmor_Delete", cachedTransform.position, Quaternion.Euler(Vector3.zero));

                        if (effTrans != null)
                        {
                            FxMakerPoolItem poolItem = effTrans.GetComponent<FxMakerPoolItem>();
                            if (poolItem == null)
                            {
                                poolItem = effTrans.gameObject.AddComponent<FxMakerPoolItem>();
                                poolItem.destroyTime = 10;
                            }

                            poolItem.Owner = this;
                            poolItem.SetAttach(transform);
                        }

                        SoundManager.instance.PlaySfxUnitSound(0, _audioSource, cachedTransform, true);
                    }

                    SuperRecoveryTick = CharInfo.SuperArmor_RecoveryTime;
                }
            }
            else
            {
                CharInfo.SuperArmor = CharInfo.SuperArmor - _ability.superArmorDmg;
            }

            //공격자측의 슈퍼아머 회복
            attacker.CharInfo.SuperArmor += _ability.superArmorRecovery;
            attacker.CharInfo.SuperArmor = (uint)Mathf.Clamp(attacker.CharInfo.SuperArmor, 0, attacker.CharInfo.MaxSuperArmor);
        }

        if (_ability != null)
        {
            if (_ability.callBuffIdx != 0)
            {
                if (this != null && BuffCtlr != null)
                {
                    BuffCtlr.AttachBuff(attacker, this, attacker.SkillCtlr.__SkillGroupInfo.GetBuff(_ability.callBuffIdx, _ability.Idx), _ability.rate, _ability.durationTime);
                }
            }
        }


        // ======== 공격력 계수에 의한 계산 ========
        float calcDamage = damage * damageRatio;
		
		float criticalRate = _LowDataMgr.instance.CalcCriticalRate (attacker.CharInfo.CriticalChance, CharInfo.CriticalRes);

		bool isCritical = criticalRate >= Random.value;

        if (!through) {
			// old
			//calcDamage = GK_DamageCalculator.CalcDamage (attacker, this, calcDamage, AddDamage, isCritical, atkType);
			// new . 2017.8.25. kyh
			// 데미지 감소 배율 계산
			float targetDefRate = _LowDataMgr.instance.CalcDamageDecreaseRate(CharInfo.DefRate);
			calcDamage = GK_DamageCalculator.CalcDamageNew ( attacker, this, calcDamage, AddDamage, isCritical, targetDefRate, criticalRate);
		}

        // Min ~ Max 데미지를 위한 (+-) 10% 적용
        // calcDamage += Random.Range((int)(calcDamage * -0.05f), (int)(calcDamage * 0.05f));
		// CalcDamageNew 안에서 계산함

        ///////////////////////////////////////////////////////////////////////////////////////
        // 주의 - 임시로 명장 버프시 데미지감소적용
        if(BuffCtlr != null && BuffCtlr.GetTypeBuffCheck(BuffType.ANGLEDEFUP))
        {
            if( MathHelper.InAngle(attacker.cachedTransform.forward.normalized, attacker.cachedTransform.position, cachedTransform.position, 180) )
            {
                Debug.Log("DamageReduce");
                calcDamage = calcDamage * 0.7f;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////

        //< 스킬 시전중이거나 구르기중이라면 감소
        eDamResistType DamResist = eDamResistType.None;

        if (TakeDamageCallBack != null)
            TakeDamageCallBack(attacker, calcDamage);

        //< 이벤트스테이트라면 1씩 닳게
        if (CurrentState == UnitState.Event)
            calcDamage = 1;

        if (UnitType == UnitType.Prop)
        {
            if((this as Prop).PropInfo.DestroyType == 2)
                calcDamage = 1;
        }
            

        calcDamage = Mathf.Clamp(calcDamage, 1, calcDamage);

        //황비홍 프로젝트 - 버프는 나중에
       
        if (BuffCtlr != null)
            calcDamage = BuffCtlr.TakeDam((int)calcDamage, ref DamResist);
#if UNITY_EDITOR

        if (IsLeader && InGameHUDPanel.ZeroDamagePlay_01)
            calcDamage = 1;
        else if (IsPartner && InGameHUDPanel.ZeroDamagePlay_02)
            calcDamage = 1;
        else if (InGameHUDPanel.ZeroDamagePlay)
	    {
		    calcDamage = 1;
	    }
#endif
        CharInfo.Hp -= (int)calcDamage;

        //생명력흡수 처리
        if(attacker.CharInfo.DrainHP > 0f)
        {
			// old
            //if( 0.7f >= Random.value ) 
            //    attacker.SetHp(attacker, calcDamage * attacker.CharInfo.DrainHP * 0.001f);
			// new
			// 2017.8.24. kyh
			attacker.SetHp(attacker,  calcDamage * _LowDataMgr.instance.CalcDrainHpRate(attacker.CharInfo.DrainHP, CharInfo.Level));
        }

        //어빌리티 없을때의 예외처리
        float pushPower = 1f;
        float pushTime = 1.5f;
        float DiePushPower = 2f;
        byte cameraShake = 0;
        float stiffenTime = 0f;

        if (_ability != null)
        {
            cameraShake = _ability.cameraShake;
            pushPower = _ability.pushpower;
            pushTime = _ability.pushTime;
            DiePushPower = Random.Range(_ability.dieMinDistance, _ability.dieMaxDistance);
            stiffenTime = _ability.stiffenTime;
        }

        bool SuperArmorActive = true;
        if(CharInfo.SuperArmor == 0)
        {
            SuperArmorActive = false;
        }

        if (G_GameInfo.GameMode == GAME_MODE.TUTORIAL && this is Pc && (this as Pc).IsLeader)
        {
            if (CharInfo.Hp <= 0)//채력의 반보다 적으면 데미지 무시함.
                CharInfo.Hp = 1;
        }

        bool DieCheck = !(CharInfo.Hp > 0);
        bool isHitSound = true;
        if (!DieCheck )
        {
            //< 구르기, 스킬 상태가 아닐때에만 푸쉬처리
            //if (CurrentState != UnitState.Evasion || CurrentState != UnitState.Skill)
            {
                if (SceneManager.instance.IsRTNetwork)
                {
                }
                else
                {
                    //플레이어/파트너는 밀리지않는다 - 몬스터는 밀림
                    if(!SuperArmorActive && _ability != null)
                    {
                        //슈퍼아머 비활성화 상태이면 무조건밀림
                        //SetPush(pushPower, attacker.gameObject, pushTime, stiffenTime);

                        SetPush(pushPower / CharInfo.Weight, attacker.gameObject, pushTime / CharInfo.Weight, stiffenTime);
                    }
                }
            }
            
        }
        else
        {
            // 죽이는 실제 처리는 함수에서 하도록 한다.
            Die(attacker, DiePushPower);
        }

        if (this.UnitType == UnitType.Prop)
            isHitSound = false;

        if (isHitSound)//일반몹 제외하고 pc, boss는 따로 재생
        {
            Resource.AniInfo aniInfo = GetAniData(eAnimName.Anim_damage);
            SoundManager.instance.PlaySfxUnitSound(aniInfo.seSkill, _audioSource, cachedTransform);
        }

        C_TakeDamage(attacker, (int)calcDamage, isSkillDamage, isCritical, CharInfo.IsDead, projecttile, DamResist, cameraShake);

        return (int)calcDamage;
    }

    //public int hp = 0;
    protected virtual void C_TakeDamage(Unit attacker, int Damage, bool IsSkillDamage, bool IsCritical, bool isDying, bool projecttile = false, eDamResistType DamResist = eDamResistType.None, byte cameraShake = 0)
    {
        GameObject attackerObj = attacker.gameObject;
        if (attackerObj != null)
        {
            byte shakeType = (byte)((IsCritical || IsSkillDamage) ? 1 : 0);

            if(cameraShake != 0 || !IsCritical)
            {
                shakeType = cameraShake;
            }

            //< 어택커가 메인리더일때만 체크
            if (shakeType != 0 && attacker.UnitType == UnitType.Unit && (IsCritical || IsSkillDamage))// && attacker == G_GameInfo.PlayerController.Leader)
            {
                if (IsSkillDamage || IsCritical)
                    ShakeCamera(attacker.cachedTransform.forward, 2);
                else
                    ShakeCamera(attacker.cachedTransform.forward, shakeType);
            }

            if (!SceneManager.instance.IsRTNetwork)
            {
                if(TeamID == 0 )
                {
                    //맞는유저의 팀이 0일경우 - 노멀게임에선 아군이다
                    ShowDamagedFx(attacker, Damage, true, IsCritical, projecttile, DamResist);
                }
                else
                {
                    ShowDamagedFx(attacker, Damage, false, IsCritical, projecttile, DamResist);
                }
                
            }
        }

        if(!isDying)
            StartFlickerFx();
    }

    public void SetPush(float pushPower, GameObject attacker, float stiffenTime, float animTime = 1.5f)
    {
        if (Mathf.Abs(pushPower) >= 0.0f)
        {
            // 공격자 방향대로 밀리도록 하기
            Vector3 my = this.cachedTransform.position;
            Vector3 target = attacker.transform.position;
            my.y = target.y = 0;

            Vector3 pushingDir = (target - my).normalized * -1; //(transform.position - attackerGO.transform.position).normalized;

            switch (CurrentState)
            {
                case UnitState.Floating:
                case UnitState.Dead:
                case UnitState.Dying:
                //case UnitState.Evasion:
                case UnitState.Event:
                    break;

                // 스킬사용중일때, 일반공격 받으면 밀리기는 무시하자
                //case UnitState.Skill:
                //    if( UseSkillSlot == 0 )
                //    {
                //        //스킬슬롯 0은 평타다 평타는 밀림
                //        {
                //            ChangeState(IsStun ? UnitState.Stun : UnitState.Push);
                //            Push(eAnimName.Anim_damage, pushingDir, pushPower, animTime);
                //        }
                //    }
                //    break;

                //< 스턴 중일떄에는 패스
                case UnitState.Stun:
                    break;

                default:
                    {/*
                        if (G_GameInfo.GameMode == GAME_MODE.PVP && (PVPGameInfo.PvpType == ePVPTYPE.MINE || PVPGameInfo.PvpType == ePVPTYPE.MINE_MATCH))
                        {

                        }
                        else*/
                        {
                            ChangeState(IsStun ? UnitState.Stun : UnitState.Push);
                            Push(eAnimName.Anim_damage, pushingDir, pushPower, stiffenTime, animTime);
                        }
                    }
                    break;
            }
        }
    }

    //< 현 캐릭터를 그냥 한방에 죽인다
    public bool NotDeadPush = false;
    public virtual void UnitKill(Unit attacker)
    {
        //< 여기서 그냥 죽인것은 따로 푸쉬연출을 안넣는다.
        NotDeadPush = true;

        int calcDamage = (int)CharInfo.Hp * 2;
        CharInfo.Hp -= calcDamage;

        Die(attacker, 0);

        if (attacker != null && gameObject.activeSelf)
            C_TakeDamage(attacker, 0, true, true, CharInfo.IsDead);
    }

    /// 방어력 같은거 무시하고 바로 적용
    public System.Action<int> SetHpCallBack;
    public void SetHp(Unit attacker, float addHp) //버프나 힐 적용시
    {
        if (addHp == 0 || CharInfo.IsDead)
            return;

        // 최종 데미지 적용
        CharInfo.Hp += (int)addHp; //(int)Mathf.Clamp(addHp, 1, addHp);

        if (SetHpCallBack != null)
            SetHpCallBack((int)addHp);

        if (CharInfo.Hp <= 0)
        {
            Die(attacker);
        }
        else
        {
            if (addHp < 0)
                ShowDamagedFx(attacker, (int)addHp, false, false, false, eDamResistType.None);
            else
            {
			 if (G_GameInfo.GameInfo.BoardPanel != null)
				G_GameInfo.GameInfo.BoardPanel.ShowDamage(gameObject, attacker.gameObject, (int)addHp, false, false, true);
		  }
        }
    }

    public virtual void Die(Unit attacker, float pushPower = 1f)
    {
        if (null != BuffCtlr)
            BuffCtlr.AllBuffDestroy();

        // 공격자도 피격자가 죽었기 때문에, 타겟팅 초기화 or 어그로 업데이트를 해서 새로 타겟팅해주기.
        if (null != attacker)
        {
            attacker.SetTarget( GameDefine.unassignedID );
        }
        SetTarget( GameDefine.unassignedID );

        if(G_GameInfo.GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            //싱글일때만 킬카운트를 올려주자
            if (attacker.Owner != null)
                attacker.Owner.NpcKillCount++;


			// 파트너도 죽여줌.
			if (this is Pc){
				foreach(Pc partner in G_GameInfo.PlayerController.Partners){
					partner.SetHp(attacker, -partner.CharInfo.MaxHp);
				}
			}
        }
        //PK킬수
        if(this is Pc && attacker.UnitType == UnitType.Unit && attacker.m_rUUID == NetData.instance.GetUserInfo()._charUUID && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            (G_GameInfo.GameInfo)._AchieveFightData.killPkCount++;
            //난투장도 따르 추가
            if(G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                (G_GameInfo.GameInfo as FreeFightGameInfo).SetFreeFightUserKillData((G_GameInfo.GameInfo as FreeFightGameInfo)._FreeFightUserKillData.KillCount + 1, this.CharInfo.UnitName);

            }
        }
        // 킬수를 세준다
        if (this is Npc && attacker.m_rUUID == NetData.instance.GetUserInfo()._charUUID  && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            if (UnitType == UnitType.Boss)
            {
                (G_GameInfo.GameInfo)._AchieveFightData.KillBossCount++;
            }
            else 
            {
                (G_GameInfo.GameInfo)._AchieveFightData.KillMonsterCount++;

            }
        }
        // 나의죽은횟수
        if (this is Pc && m_rUUID == NetData.instance.GetUserInfo()._charUUID && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            (G_GameInfo.GameInfo)._AchieveFightData.DieCount++;
        }

        if(SceneManager.instance.IsRTNetwork)
        {
            //일단 임시로 이전과같이
            if (ChangeState(UnitState.Dying, true) && (FSM.Current() is DyingState))
            {
                //타격감용 테스트
                if (Random.Range(0, 2) == 0)
                    (FSM.Current() as DyingState).PushForDie(7f);
                else
                    (FSM.Current() as DyingState).PushForDie(3f);
            }
        }
        else
        {
            if (ChangeState(UnitState.Dying, true) && (FSM.Current() is DyingState))
            {
                (FSM.Current() as DyingState).PushForDie(pushPower);
            }
        }
        

        G_GameInfo.GameInfo.SendMessage( "OnDieUnit", this, SendMessageOptions.DontRequireReceiver );

        if (LightObj != null)
            LightObj.SetActive(false);
    }

    #endregion

    #region :: Skill And Buff ::

    /// 현재 스킬을 사용가능한지 검사
    public bool CanCastingSkill(bool overrideSkillState = false)
    {
        if(overrideSkillState)
        {
            switch (CurrentState)
            {
                case UnitState.Dying:
                case UnitState.Dead:
                case UnitState.Stun:
                    return false;
            }

            return true;
        }
        else
        {
            switch (CurrentState)
            {
                case UnitState.Skill:
                {
                    if (SkillBlend)
                        return true;
                    else
                        return false;
                }
                    
                case UnitState.Dying:
                case UnitState.Dead:
                case UnitState.Stun:
                    return false;
            }

            return true;
        }
        
    }

    public int SkillToSlotID(uint skillID) //스킬인덱스를 가지고 슬롯 번호를 가져오는 함수 
    {
        for (int i=0;i< SkillCtlr.SkillList.Length;i++)
        {
            if (SkillCtlr.SkillList[i] == null) //평타일때도 확인하려고 할테니...
                continue;

            if (SkillCtlr.SkillList[i].GetSkillActionInfo().idx == skillID)
                return i;
        }

        return -1;
    }

    /*
    public bool isBuffSkill(uint SkillID)
    {
        for(int i=0;i<SkillCtlr.__SkillGroupInfo.skillData.Length;i++)
        {
            if (SkillCtlr.__SkillGroupInfo.skillData[i] != null)
            {
                if (SkillCtlr.__SkillGroupInfo.skillData[i]._SkillID != 0 )
                {
                    if(SkillCtlr.__SkillGroupInfo.skillData[i]._SkillID == SkillID)
                    {
                        List<SkillTables.AbilityInfo> abilityList = SkillCtlr.__SkillGroupInfo.GetAbilityList(SkillCtlr.__SkillGroupInfo.skillData[i]._SkillID);

                        if (abilityList == null)
                            return false;

                        for (int j = 0; j < abilityList.Count; j++)
                        {
                            if (abilityList[j].callBuffIdx != 0)
                            {
                                return true;
                            }
                        }
                    }                    
                }
            }
        }

        return false;
    }
    */

    //이게 평타일경우 몇번째 콤보인지를 얻어온다 평타가 아닐경우 -1
    public int GetNormalAttackComboCount(uint skillID)
    {
        //가능하면 직접 억세스 하는건 별론데...
        for(int i=0;i<SkillCtlr.__SkillGroupInfo.normalAttackData.Length;i++)
        {
            if(SkillCtlr.__SkillGroupInfo.normalAttackData[i] != null)
            {
                if( SkillCtlr.__SkillGroupInfo.normalAttackData[i]._SkillID == skillID)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    //콤보 카운트로 스킬의 ID를 가져온다 없을경우 -1
    public uint GetNormalAttackComboSkillID(int comboCount)
    {
        if(SkillCtlr.__SkillGroupInfo.normalAttackData[comboCount] != null)
        {
            return SkillCtlr.__SkillGroupInfo.normalAttackData[comboCount]._SkillID;
        }

        return uint.MaxValue;
    }

    /// <summary>
    /// 의사 때문에 예외처리
    /// </summary>
    private float PressedTime = 0;
    public void ChargeStart()
    {
        if (!(CurrentState == UnitState.Move || CurrentState == UnitState.Idle))
            return;

        if (!SkillCtlr.IsAbleSkill(1))
            return;

        SkillCtlr.ActiveSkill(1);
        ActionInfo actionData = SkillCtlr.GetActionInfo(1);

        PressedTime = Time.time;
        BuffCtlr.AttachBuff(this, this, SkillCtlr.__SkillGroupInfo.GetBuff(actionData.callCastingBuffIdx, 0), 10000, 3);

        if(CurrentState == UnitState.Idle)
            PlayAnim(eAnimName.Anim_Extra, true, 0.1f, true);
    }

    //차지스킬관련
    public void ChargeEnd()
    {
        if (CharInfo.CharIndex != 13000)
            return;

        {
            if(BuffCtlr.GetTypeBuffCheck(BuffType.CHARGEATTACK))
            {
                Debug.Log("Released:" + (Time.time - PressedTime));
                float presstime = Time.time - PressedTime;
                uint iCount = (uint)(presstime / 1.5f);
                Debug.Log("ChargedPower:" + iCount);

                ActionInfo actionData = SkillCtlr.GetActionInfo(1);
                BuffCtlr.DetachBuff(actionData.callCastingBuffIdx);

                switch (iCount)
                {
                    case 0:
                        {
                            UseSkillSlot = (uint)5;
                            UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(5);
                            ChangeState(UnitState.Skill);
                            ChainSkill = true;
                        }
                        break;
                    case 1:
                        {
                            UseSkillSlot = (uint)6;
                            UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(6);
                            ChangeState(UnitState.Skill);
                            ChainSkill = true;
                        }
                        break;
                    default:
                        {
                            UseSkillSlot = (uint)7;
                            UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(7);
                            ChangeState(UnitState.Skill);
                            ChainSkill = true;
                        }
                        break;

                }

                SkillCtlr.SkillList[1].EndSkill();
            }
        }
    }

    /// 유닛이 보유한 스킬을 사용
    public virtual bool UseSkill(int slot, bool AutoModeforceUserUse = false)
    {
        // 지금 안전 지대인지에 있는지? 검사
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        {
            if (m_rUUID == NetData.instance._userInfo._charUUID)
            {
                //안전지대 체크          
            }

            //멀티 컨텐츠에서는 타겟을 잡지않는다.
            SetTarget(GameDefine.unassignedID);

            SkillActiveCondition skillConditoion = SkillCtlr.GetSkillCondition(slot);
            UseSkillSlot = (uint)slot;
            UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(slot);

            if (UnitType == UnitType.Npc)
            {
                if (CurrentState == UnitState.Dying || CurrentState == UnitState.Dead || CurrentState == UnitState.Stun)
                    return false;

            }
            else
            {
                if (!SkillCtlr.IsAbleSkill(slot))
                    return false;

                if (m_rUUID == NetData.instance._userInfo._charUUID)
                {
                    if (!CanCastingSkill(false))
                    {
                        if(skillConditoion == SkillActiveCondition.eAvailable || SkillBlend != false)
                        {
                            //예약
                            ReservedSkillIdx = (uint)slot;
                            ReservedSkillTime = Time.time;
                        }

                        return false;
                    }
                }
                else
                {
                    if (!CanCastingSkill(true))
                    {
                        return false;
                    }
                }
            }

            if (UnitType == UnitType.Npc)
            {
                skillConditoion = SkillActiveCondition.eAvailable; //무조건 발동시킨다.
                ChangeState(UnitState.Idle);
            }

            if( UnitType == UnitType.Unit && m_rUUID != NetData.instance._userInfo._charUUID)
            {
                //다른 일반 유저일경우 무조건 발동
                skillConditoion = SkillActiveCondition.eAvailable; //무조건 발동시킨다.
                ChangeState(UnitState.Idle);
            }

            switch (skillConditoion)
            {
                case SkillActiveCondition.eFarFromTarget: //타겟으로 부터 멀다
                case SkillActiveCondition.eAvailable:
                case SkillActiveCondition.eNeedTarget: //이래도 나감
                    ChangeState(UnitState.Idle);
                    ChangeState(UnitState.Skill); 
                    return true;
                case SkillActiveCondition.eActive:  //활성화 중이다
                    if(SkillBlend)
                    {
                        ChangeState(UnitState.Idle);
                        ChangeState(UnitState.Skill);
                        return true;
                    }
                    return false;
                case SkillActiveCondition.eCoolTime://쿨타임이다
                    return false;
            }

            return false;
        }
        else
        {
            //< 플레이어인데, 현재 카메라 이벤트 중이라면 패스
            //if (UnitType == global::UnitType.Unit && isLeader && SkillEventMgr.EventUpdate)
            //    return false;

            SkillActiveCondition skillConditoion = SkillCtlr.GetSkillCondition(slot, AutoModeforceUserUse);
            UseSkillSlot = (uint)slot;
            UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(slot);

            if (!SkillCtlr.IsAbleSkill(slot))
                return false;

            if (!CanCastingSkill())
            {
                if (skillConditoion == SkillActiveCondition.eAvailable || SkillBlend != false)
                {
                    //예약
                    ReservedSkillIdx = (uint)slot;
                    ReservedSkillTime = Time.time;
                }

                return false;
            }

            switch (skillConditoion)
            {
                case SkillActiveCondition.eFarFromTarget:
                case SkillActiveCondition.eAvailable:
                case SkillActiveCondition.eNeedTarget:
                    ChangeState(UnitState.Idle);
                    ChangeState(UnitState.Skill);
                    return true;
                case SkillActiveCondition.eActive:
                    if (SkillBlend)
                    {
                        ChangeState(UnitState.Idle);
                        ChangeState(UnitState.Skill);
                        return true;
                    }
                    return false;
                case SkillActiveCondition.eCoolTime:
                    return false;
            }
            return false;
        }        
    }

    //< 실질적으로 스킬 시전
    public void StartSkill(uint verifyId, bool normal)
    {
        if (normal)
            SkillCtlr.UseSkill(0, (int)verifyId, true);
        else
            SkillCtlr.UseSkill(UseSkillSlot, (int)verifyId, false);
    }

    //< 상태이상에 걸릴때 호출될 함수
    public bool SetUnitState(bool _set, byte _type, float successrate, Unit caster)
    {
        //< 구르기상태라면 패스
        //if (_set && CurrentState == UnitState.Evasion)
        //    return false;

        //< 보스라면 그냥 패스 -> 보스도 이제걸림
        //if ((this is Boss))
        //    return false;

        //< 저항력을 체크한다.
        if(_set)
        {
            //황비홍 프로젝트에서 수정 - 기본 저항력같은데 다른데서 가져와야될듯
            //uint Resistance = 0;
            //uint Resistance = (this is Pc) ? (this as Pc).comData.gradeInfo.Resistance : (this as Npc).enemyInfo.Resistance;
            //Resistance += (uint)(Resistance * CharInfo.Stats[AbilityType.AllResist].FinalValue);

            //< 100%는 없다.
            //if (Resistance > 900000)
            //    Resistance = 900000;

            /*
            if ((BuffType)_type == BuffType.Freeze)
            {
                if (Random.Range(0, 1000000) <= Resistance)
                    return false;
            }
            */
        }
        
        //< 상태이상 시전
        if (_set)
        {
            switch((BuffType)_type)
		    {
			    case BuffType.Knockback:
			    //case BuffType.Freeze:
                //case BuffType.StoneCurse:
                case BuffType.Stun:
                case BuffType.Down:
                    //< 해당 스테이트에는 상태이상 안걸림
                    switch (CurrentState)
                    {
                        case UnitState.Dead:
                        case UnitState.Dying:
                        case UnitState.Stun:
                            return false;
                    }

                    //슈퍼아머 관련 처리 - 다운과 넉백은 슈퍼아머 상태면 걸리지않는다.
                    if((BuffType)_type == BuffType.Down || (BuffType)_type == BuffType.Knockback)
                    {
                        //슈퍼 아머가 0이 아닐경우는 무시된다.
                        if( CharInfo.SuperArmor != 0 )
                        {
                            return false;
                        }
                    }

                    //< 스턴처리
                    ChangeState(UnitState.Stun);

                    if ((BuffType)_type == BuffType.Knockback)
                    {
                        (FSM.Current() as StunState).Push(successrate);
                    }
                    break;
            }
        }
        else
        {
            switch ((BuffType)_type)
            {
                //< 스턴 해제
                //case BuffType.Freeze:
                //case BuffType.StoneCurse:
                case BuffType.Stun:
                    //< 죽었거나, 숨겨져있을때에는 아이들로 변환을 시키지 않는다.
                    if (Usable)
                        ChangeState(UnitState.Idle);
                    break;
			    case BuffType.Knockback:
			    case BuffType.Down:
                    if(Usable && !ChangeState(UnitState.StandUp))
                        ChangeState(UnitState.Idle);
                    break;
            }
        }

        return true;
    }

    /// 죽은 상태에서 다시 살려내기. (살아있는 상태면 FullHP, 무적상태만 적용)
    public virtual void Revive()
    {
        // 죽어서 유닛 리스트에서 사라졌다면, 다시 추가
        if (!G_GameInfo.CharacterMgr.ContainsUnit( this ))
            G_GameInfo.CharacterMgr.AddUnit( this );

        //< 버프 컨트롤러 다시 적용
        if (BuffCtlr == null)
            BuffCtlr = G_GameInfo.CharacterMgr.AddBuffCtrl(this);

        //< 스킬 초기화
        if (null != SkillCtlr)
            SkillCtlr.ResetSkill();

        Model.Main.SetActive( true );

		NGUITools.SetLayer(Model.Main, 14);
        Animator.ClearAnimState();
        CharInfo.Hp = CharInfo.MaxHp;
        navAgent.enabled = true;

        if (_DeadEffectCor != null)
            StopCoroutine(_DeadEffectCor);

        if (gameObject.activeSelf)
            StartFlickerFx(); // 부활 효과 (원래 메시 색상 복구를 위해서 호출)

        if (gameObject.activeSelf)
        {
            TempCoroutine.instance.FrameDelay(0.5f, () =>
            {
                SpawnSkillEffect("Fx_IN_revival", 0.5f, cachedTransform, cachedTransform, (skillCastingEff) =>
                {

                });
            });            
        }

        //< 셰이더 수치 복구
        foreach (SkinnedMeshRenderer renderer in Model.GetSkinnedMeshRenders())
        foreach (Material M in renderer.materials)
        {
            //M.SetFloat("_RimPower", 3);
            //M.SetFloat("_SliceAmount", 0);
            //M.SetColor("_RimColor", GameDefine.UnitRimColor);
            M.SetFloat("_FlashAmount", 0);
            M.SetFloat("_Blend", 0);
        }

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.enabled = true;

        CapsuleCollider collider2 = gameObject.GetComponent<CapsuleCollider>();
        collider2.enabled = true;

        AddShadow( collider ? new Vector3( collider.size.x, collider.size.z, 1 ) : Model.ScaleVec3 );
        
        StaticState(false);
        ClearPath();
        ChangeState( UnitState.Idle );

        //부활자체는 애니가 없기에 추가해야한다
        //Resource.AniInfo aniInfo = GetAniData(eAnimName.revival);
        //int voiceSoundArrayIdx = Random.Range(0, aniInfo.voice.Count);
        //uint voiceSoundID = uint.Parse(aniInfo.voice[voiceSoundArrayIdx].ToString());
        //SoundManager.instance.PlayUnitVoiceSound(voiceSoundID, parent._audioSource, parent.cachedTransform);

        if (LightObj != null)
            LightObj.SetActive(true);

        // 업적데이터동기화
        if (this is Pc && m_rUUID == NetData.instance.GetUserInfo()._charUUID && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            (G_GameInfo.GameInfo)._AchieveFightData.ReviveCount++;
        }
    }

    //< 프로젝트타일 발사(유닛에게 귀속되게하기위함)
    public List<NetProjectile> ProjecttileList = new List<NetProjectile>();
    public GameObject SpawnProjectile(SkillTables.ProjectTileInfo _projecttile, int _Damage, int teamIndex, Vector3 dir, Unit owner, Unit target, AbilityData ability, ulong projectTileId, bool normal = false)
    {
        if (_projecttile == null)
            return null;

        GameObject obj = G_GameInfo.GameInfo.SpawnProjectile(_projecttile, ability, _Damage, teamIndex, dir, owner, target, cachedTransform.position, cachedTransform.localRotation, projectTileId, normal);
        if (obj != null)
        {
            ProjecttileList.Add(obj.GetComponent<NetProjectile>());

            //< 레이어 변경
            if (Model != null)
                NGUITools.SetChildLayer(obj.transform, Model.Transform.gameObject.layer);
        }

        obj.transform.localScale = owner.cachedTransform.lossyScale;

        return obj;
    }

    public void DespawnProjectile(NetProjectile obj)
    {
        if (ProjecttileList.Contains(obj))
            ProjecttileList.Remove(obj);
    }

    public void AllClearProjecttile()
    {
        for(int i=0; i<ProjecttileList.Count; i++)
        {
            if (ProjecttileList[i] != null)
                ProjecttileList[i].Clear();
        }

        ProjecttileList.Clear();
    }

    #endregion

    #region :: Leader System ::

    /// 맵 마지막 지점으로 이동중인지 여부
    public bool IsMovingLastLocation { set; get; }

    /// 현재 리더인지 유무
    public virtual bool IsLeader
    {
        set {
            if (isLeader != value)
            {
                isLeader = value;
                
                if (isLeader)
                {
                    IsHelper = false;

                    storeRecognition = CharInfo.AtkRecognition;

                    if (null == dirIndicator && !IsDie &&(G_GameInfo.GameMode == GAME_MODE.SINGLE || G_GameInfo.GameMode == GAME_MODE.RAID))
                    { 
                        //< 혹시나 존재하는지 체크한다
                        dirIndicator = gameObject.GetComponent<DirectionIndicator>();
                        if (dirIndicator != null)
                            DestroyImmediate(dirIndicator);
                        
                        dirIndicator = gameObject.AddComponent<DirectionIndicator>();

                        if (G_GameInfo.GameMode == GAME_MODE.SINGLE && UnitType == global::UnitType.Unit)
                        {
                            dirIndicator.Init(gameObject).SetOnUpdate(OnUpdateIndicator);

                            //if (navAgent != null && navAgent.enabled)
                            //    CalculatePath(G_GameInfo.GameInfo.CurNavMeshGroup().end.position, true);
                        }
                        else if(G_GameInfo.GameMode == GAME_MODE.RAID)
                            dirIndicator.Init(gameObject).SetOnUpdate(OnUpdateTargetIndicator);
                    }
                }
                else
                { 
                    CharInfo.AtkRecognition = storeRecognition;

                    if (null != dirIndicator)
                        Destroy( dirIndicator );
                }
            }
        }

        get { return isLeader; }
    }
    
    public bool isLeader = false;
    private float storeRecognition;
    
    /// 현 스테이지 마지막 위치로 이동하게 만든다.
    public bool GoToLastLocation()
    {
        // 최소 이동 거리 검사
        Vector3 lastPos = G_GameInfo.GameInfo.CurNavMeshGroup().end.position;
        Vector3 offset = cachedTransform.position - lastPos;
        if (offset.sqrMagnitude < 4f)
            return false;

        IsMovingLastLocation = MovePosition( G_GameInfo.GameInfo.CurNavMeshGroup().end.position , 1f, true);

        return IsMovingLastLocation;
    }

    #endregion

    #endregion

    public Unit HelperParnetUnit;
    public bool IsHelper = false;   //< 도우미 역할(메인 캐릭을 따라다니면서 자동공격을 지원)

    public void SetHelper(Unit _parent)
    {
        IsLeader = false;
        IsHelper = true;
        HelperParnetUnit = _parent;
    }

    public bool IsClone = false;
    public void SetClone(Unit _parent)
    {
        IsLeader = false;
        IsClone = IsHelper = true;
        HelperParnetUnit = _parent;
    }

    public void SetMasteryEvent(int targetAction, Unit target = null)
    {
        //if (unitMastery == null)
        //    return;

        ////< 2번째 조건(상태)
        //int targetAction2 = 0;

        ////< 공격시에는 상대방의 2번째 조건을 체크한다.
        //if (targetAction == 1)
        //    targetAction2 = GetUnitStatusIdx(target);
        ////< 그외에는 내 기준으로 체크
        //else
        //    targetAction2 = GetUnitStatusIdx(this);

        ////< 마스터리 호출
        //unitMastery.SetEvent(UnitMastery.GetBitValue(targetAction), targetAction2, target);
    }

    /// <summary>
    /// 파트너인지 확인
    /// </summary>
    public bool IsPartner = false;

    //< 상대의 상태에 따라서 인덱스를 리턴해준다(비트연산용)
    int GetUnitStatusIdx(Unit unit)
    {
        int targetAction2 = 0;

        //< 내가 상태이상에 걸렸을시에 공격하는것일수도 있기때문에 체크
        //if (unit.CurrentState == UnitState.Stun)
        //{
        //    switch ((unit.FSM.Current() as StunState).NowSkillType)
        //    {
        //            //< 기절시
        //        case eSkillType.Skill_Buff_Stun:
        //            targetAction2 |= UnitMastery.GetBitValue(1);
        //            break;

        //            //< 다운시
        //        case eSkillType.Skill_Buff_Down:
        //            targetAction2 |= UnitMastery.GetBitValue(3);
        //            break;

        //            //< 석화
        //        case eSkillType.Skill_Buff_StoneCurse:
        //            targetAction2 |= UnitMastery.GetBitValue(5);
        //            break;

        //            //< 빙결시
        //        case eSkillType.Skill_Buff_Freeze:
        //            targetAction2 |= UnitMastery.GetBitValue(6);
        //            break;
        //    }
        //}

        //< 플라이 상태일시
        //else if (unit.CurrentState == UnitState.Floating)
        //    targetAction2 |= UnitMastery.GetBitValue(4);

        //< 혼란 상태일시
        //else if (unit.Buff_Confusion)
        //    targetAction2 |= UnitMastery.GetBitValue(2);

        //< 원거리 일경우
        //if (unit.AttackType == 2)
        //    targetAction2 |= UnitMastery.GetBitValue(7);

        //< 근거리 일경우
        //else if (unit.AttackType == 1)
        //    targetAction2 |= UnitMastery.GetBitValue(8);

        //< 속성별 대입
        //switch((eUnitProperty)unit.CharInfo.Property)
        //{
        //    case eUnitProperty.Nature: // 자연
        //        targetAction2 |= UnitMastery.GetBitValue(9);
        //        break;
        //    case eUnitProperty.Poison: // 독
        //        targetAction2 |= UnitMastery.GetBitValue(10);
        //        break;
        //    case eUnitProperty.Ice: // 물
        //        targetAction2 |= UnitMastery.GetBitValue(11);
        //        break;
        //    case eUnitProperty.Metal: // 금속
        //        targetAction2 |= UnitMastery.GetBitValue(12);
        //        break;
        //    case eUnitProperty.Fire: // 불
        //        targetAction2 |= UnitMastery.GetBitValue(13);
        //        break;
        //    case eUnitProperty.Holy: // 빛
        //        targetAction2 |= UnitMastery.GetBitValue(14);
        //        break;
        //    case eUnitProperty.Dark: // 암흑
        //        targetAction2 |= UnitMastery.GetBitValue(15);
        //        break;
        //}

        return targetAction2;
    }

    Coroutine BakeMeshObjectCoroutine;
    public void BakeMeshObject(int count, float retime, float lifetime)
    {
        if (BakeMeshObjectCoroutine != null)
            StopCoroutine(BakeMeshObjectCoroutine);

        BakeMeshObjectCoroutine = StartCoroutine(BakeMeshObjectUpdate(count, retime, lifetime));
    }

    public void StopBakeMeshObject()
    {
        if (BakeMeshObjectCoroutine != null)
            StopCoroutine(BakeMeshObjectCoroutine);
    }

    IEnumerator BakeMeshObjectUpdate(int count, float retime, float lifetime)
    {
        while(true)
        {
            //< 시간동안 대기
            yield return new WaitForSeconds(retime);

            //< 생성을 해준다
            Model.BakeMeshObject(lifetime);
            count--;
            if (count <= 0)
                break;
        }
        yield return null;
    }

    #region :: ChangeState ::

    /// 상태 변경 함수
    public bool ChangeState(UnitState newState, bool Coercion = false)
    {
        if (CurrentState == newState)
            return false;

        if (UsableNavAgent && navAgent.hasPath)
            navAgent.Stop();

        // 스턴 상태거나, 상태 불가능한지 검사
        if (!Coercion && IsStun && !ChangeStateInStun(newState))
            return false;

        if (FSM.ChangeState( newState ))
        {
            CurrentState = newState;
        }
        else
        {
            CurrentState = FSM.Current_State;
            return false;
        }

        return true;
    }

    /// 스턴 상태에서 변경가능한 State인지 여부
    bool ChangeStateInStun(UnitState newState)
    {
        // 현재 상태가 스턴상태가 아니라면, 아무 상태로 변경가능하다.
        if (CurrentState != UnitState.Stun)
            return true;

        switch (newState)
        { 
            case UnitState.Idle:
            case UnitState.Push:
            case UnitState.StandUp:
            case UnitState.Dying:
            case UnitState.Dead:
                return true;
        }

        return false;
    }

    //< 구르기 또는 이동키를 사용할수있는지 여부
    public bool NotChangeStateCehck()
    {
		//if (CurrentState == UnitState.Evasion || 
		if (CurrentState == UnitState.Dying || CurrentState == UnitState.Flying || CurrentState == UnitState.Push || 
		    CurrentState == UnitState.Floating || CurrentState == UnitState.Stun || CurrentState == UnitState.StandUp)
            return false;

        return true;
    }

    /// <summary>
    /// 주어진 방향으로 강제 이동
    /// </summary>
    /// <param name="direction">방향</param>
    /// <param name="power">밀리게할 힘</param>
    public void Push(eAnimName animName, Vector3 direction, float power, float animTime, float stiffenTime, UnitState nextState = UnitState.Idle)
    {
        (FSM.Current() as UnitStateBase).Push( animName, direction, power, animTime, stiffenTime, nextState);
    }

    /// 뜨면서 넘어지는 상태로 전환
    public void Floating(Vector3 direction, float pushPower, float height = 1.5f, float _duration = 0)
    {
        ChangeState( UnitState.Floating );
        if (FSM.Current() is FloatingState)
            (FSM.Current() as FloatingState).Floating(direction, pushPower, height, _duration);
    }

    float stayFlyingT;
    float stayGroundT;
    public bool isAir;

    /// 검사후, 착륙이냐 이륙이냐 판단해서 수행
    public bool CheckAndFlying(UnitState _next = UnitState.Idle)
    {
        if (Model == null || Model.Main == null)
            return false;

        // 공중유닛이 아니라면 검사할 필요없음.
        if (CharInfo.OrignalMoveType != MoveType.Air)
            return false;

        if (isAir)
        {
            if (stayFlyingT > Time.time)
                return false;

            if (Flying( 0, 0.1f, _next ))
            {
                isAir = false;
                // 지상에서 유지할 시간
                stayGroundT = Time.time + 4f;

                return true;
            }
        }
        else
        {
            if (stayGroundT > Time.time)
                return false;

            if (Flying( CharInfo.AirHeight, 0.1f, _next ))
            {
                isAir = true;

                // 공중에서 유지할 시간
                stayFlyingT = Time.time + CharInfo.AirFlyTime;
                return true;
            }
        }

        return false;
    }

    /// 위로 올라가던가, 내려가던가
    public bool Flying(float targetHeight, float lerpDuration = 0.5f, UnitState _next = UnitState.Idle)
    {
        ChangeState( UnitState.Flying );
        if (CurrentState == UnitState.Flying && FSM.Current() is FlyingState)
        {
            ( FSM.Current() as FlyingState ).Flying( targetHeight, lerpDuration, _next );
            return true;
        }
        return false;
    }

    #endregion

    // 실제 애니메이션 플레이
    public virtual bool PlayAnim(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        if (Animator == null ||  !Animator.IsReady)
            return false;

        bool isSuccess = Animator.PlayAnim(animEvent, CrossFade, CrossTime, canPlaySameAnim, queued);
        return isSuccess;
    }

    public virtual bool PlayAnimNoRootMation(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        
        return true;
    }

    //< 애니메이션 데이터 리턴
    public Resource.AniInfo GetAniData(eAnimName anim)
    {
        if (CharInfo.animDatas[(int)anim] != null)
            return CharInfo.animDatas[(int)anim];

        return null;
    }

    public uint GetVoiceID(eAnimName anim)
    {
        uint voiceSoundID = 0;
        if (CharInfo.animDatas[(int)anim] != null)
        {
            Resource.AniInfo aniData = CharInfo.animDatas[(int)anim];

            if (aniData.voice.Count >= 1)
            {
                int voiceSoundArrayIdx = Random.Range(0, aniData.voice.Count);

                if (aniData.voice[voiceSoundArrayIdx] != null)
                {
                    voiceSoundID = uint.Parse(aniData.voice[voiceSoundArrayIdx].ToString());
                }
                else
                {
                    voiceSoundID = 0;
                }
            }
            else
            {
                voiceSoundID = uint.Parse(aniData.voice[0].ToString());
            }
        }

        return voiceSoundID;
    }

    #region :: Target System (추후 시스템으로 분리할 가능성) ::

    /// <summary>
    /// 타겟 셋팅후 바로 공격! uLink.NetworkViewID.unassigned 셋팅은 SetTarget 함수로 변경
    /// </summary>
    /// <returns>공격상태 전환 성공 유무</returns>
    public virtual bool AttackTarget(int instID)
    {
        SetTarget( instID );

        if (CurrentState == UnitState.Skill)
            return false;

        Unit Target = null;
        if (!G_GameInfo.CharacterMgr.InvalidTarget( instID, ref Target ))
        {
            if (CalculatePath(Target.cachedTransform.position))
            {
                //RaycastHit[] hits = Physics.RaycastAll( Target.transform.position, Target.transform.forward, 3f );                
                //Debug.Log( gameObject + ".AttackTarget() : " + Target + " : HitCount : " + hits.Length, gameObject );
                //ChangeState( UnitState.Attack );
            }

            return true;
        }

        return false;
    }

    public void FindTarget()
    {
        float a_Angle = 360;

        byte teamIndex = (byte)(TeamID == 0 ? 1 : 0);
        Unit newTarget = G_GameInfo.CharacterMgr.FindTargetByTeam(teamIndex, this, this.CharInfo.AtkRecognition, a_Angle, true);

        if (null != newTarget)
        {
            if (TargetID != newTarget.GetInstanceID())
            {
                SetTarget(newTarget.GetInstanceID());
            }
        }
    }

    // 타겟 설정만 하는 함수
    public virtual bool SetTarget(int instanceID)
    {
        if (targetID == instanceID)
            return false;

        //Debug.Log( gameObject + ".SetTarget() : " + instanceID, gameObject );

        TargetID = instanceID;

        return true;
    }

    //< 타겟을 리턴해줌
    public Unit TargetUnit;
    public Unit GetTarget()
    {
        Unit newTarget = null;
        if (!G_GameInfo.CharacterMgr.InvalidTarget(TargetID, ref newTarget))
        {
            TargetUnit = newTarget;
            return newTarget;
        }

        return null;
    }
    #endregion

    #region :: Components Utils ::

    /// 행동불가 상태로 만들기. 스폰될때나, 게임 종료될때 사용
    public bool _StopState = false;
    public bool StopState
    {
        get{return _StopState;}
        set
        {
            _StopState = value;
        }
    }

    public bool _UnitStop = false;
    public bool UnitStop
    {
        get { return _UnitStop; }
        set
        {
            _UnitStop = value;
        }
    }

    public virtual void StaticState(bool doStatic = true, float fadeDelay = 0.1f)
    {
        if (!gameObject.activeSelf)
            return;

        if (null != navAgent)
        {
            navAgent.enabled = !doStatic;
        }

        if (null != FSM)
        {
            //if (UnitType == UnitType.Npc && !doStatic)
            //{
                //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT)
                //if (SceneManager.instance.IsRTNetwork)
                //{
                //    FSM.Enable(UnitState.Idle);
                //}
                //else
            //    {
            //        FSM.Enable(UnitState.Idle);//(UnitState.Wander);
            //    }

            //    CurrentState = FSM.Current_State;
            //}
            //else
                FSM.Enable(!doStatic);
        }

        if (doStatic && Model.IsReady && (Animator.CurrentAnim != eAnimName.Anim_idle && Animator.CurrentAnim != eAnimName.Anim_battle_idle))
        {
            bool queued = (Animator.CurrentAnim == eAnimName.Anim_none || Animator.CurrentAnimState.wrapMode != WrapMode.Loop);

            //PlayAnim( eAnimName.Anim_idle, false, 0.1f, false, queued );
            if (Animator.IsReady)
                Animator.PlayAnim((SceneManager.instance.CurrentStateBase() as TownState) != null ? eAnimName.Anim_idle : eAnimName.Anim_battle_idle, true, fadeDelay, true, queued);
        }
    }

    public void LookAt(Vector3 target, float delta = 1f)
    {
        cachedTransform.rotation = MathHelper.GetYRotation( target, cachedTransform, delta );
    }

    /// 상황에 맞게 카메라 흔들어 주기.
    static float ShakeTime = 0;
    public virtual void ShakeCamera(Vector3 shakeDir, byte type = 1)
    {
        //< 마을일경우 패스
        if (TownState.TownActive)
            return;

        //< 한순간에 막 떨리는걸 막기위함
        if ((Time.time - ShakeTime) < 0.5f)
            return;

        ShakeTime = Time.time;

        //< 1로 왔을시에는 0으로 보정해줌
        if (type == 1)
            type = 0;

        CameraManager.instance.Shake( shakeDir, type );
    }

    #region :: Animation Freezing ::

    string pauseAnimName;
    float pauseAnimDuration;

    public void PauseAnimation(eAnimName animName, float duration = 0.1f)
    {
        pauseAnimName = Animator.GetAnimName( animName );
        pauseAnimDuration = duration;
        StopCoroutine( "PauseAnimRoutine" );
        StartCoroutine( "PauseAnimRoutine" );
    }

    IEnumerator PauseAnimRoutine()
    {
        string freezeName = pauseAnimName;

        AnimationUtil.SetAnimationPause( Animator.Animation, freezeName, true );

        yield return new WaitForSeconds( pauseAnimDuration );

        if (Animator.IsPlaying(freezeName))
            AnimationUtil.SetAnimationPause( Animator.Animation, freezeName, false );
    }

    #endregion

    public void ShakeModel()
    {
        if (null != shakeRoutine)
            StopCoroutine( shakeRoutine );

        shakeRoutine = UnitEffectHelper.ShakeModelRoutine( Model.Transform );
        StartCoroutine( shakeRoutine );
    }

    public virtual void AddShadow(Vector3 scale)
    {
        if (QualityManager.instance.GetQuality() == QUALITY.QUALITY_LOW)
        {
            if (null != shadowGO)
            {
                shadowGO.transform.localScale = scale;
                shadowGO.SetActive(true);
                return;
            }

            shadowGO = ResourceMgr.Instantiate<GameObject>("Etc/Shadow");
            shadowGO.transform.parent = transform;
            shadowGO.transform.localPosition = new Vector3(0f, 0.06f, 0f);
            shadowGO.transform.localRotation = Quaternion.Euler(new Vector3(270, 0, 0));
            shadowGO.transform.localScale = scale;
        }
        else
        {
            if (null != shadowGO)
            {
                shadowGO.transform.localScale = scale;
                shadowGO.SetActive(false);
                return;
            }
        }
    }

    public virtual void ShadowPosition(Vector3 localPos)
    {
        if (null != shadowGO)
            shadowGO.transform.localPosition = localPos;
    }

    public virtual void DeleteShadow()
    {
        if (shadowGO != null)
        {
            shadowGO.SetActive(false);
            //shadowGO.transform.parent = null;
            //Destroy( shadowGO );
            //shadowGO = null;
        }
    }

    // 피격 적용시, 보여즐 효과.
    public void ShowDamagedFx(Unit attacker, int damage, bool isMyUnit, bool isCritical, bool projecttile, eDamResistType DamResist)
    {
        Vector3 dir = attacker.cachedTransform.position - cachedTransform.position;
        Vector3 spawnPos = cachedTransform.position + (dir.normalized * 0.8f);
        spawnPos.y += (Height * 0.5f); // 캐릭터 높이 비례해서 계산

        if(attacker.UnitType == global::UnitType.Unit || attacker.UnitType == global::UnitType.Boss || attacker.UnitType == global::UnitType.Npc)
        {
            //< 프로젝트 타일일경우는 캐릭 중심에 띄워줘야함
            if(!projecttile)
            {
                Ray ra = new Ray(attacker.cachedTransform.position, attacker.cachedTransform.forward);
                RaycastHit hit;
                if (UnitCollider.Raycast(ra, out hit, 100))
                {
                    //< 나의 컬리더박스에 가장 외곽부분값을 얻은후, 공격자의 방향벡터로 수치를 더해줌
                    spawnPos = hit.point;

                    //< 충돌 위치는 공격자 높이보다 내가 더 크다면 공격자의 높이기준, 반대라면 나의 기준
                    spawnPos.y = this.cachedTransform.position.y;
                    spawnPos.y += Height > attacker.Height ? (attacker.Height * 0.5f) : (Height * 0.5f);
                }
            }
        }


        float EffectSize = Model.Transform.localScale.x;

        
        //< 발사체라면 마법형인지 물리형인지 체크
        if (projecttile)
        {
            //< 마법형
            if (attacker.AttackType == 3)
                SpawnEffect("Fx_beshot_01", 1, this.cachedTransform, null, false, (trn) => { trn.transform.position = spawnPos; trn.transform.LookAt(spawnPos + (dir.normalized * 5)); }, EffectSize);
            //< 물리형
            else
                SpawnEffect("Fx_beshot_01", 1, this.cachedTransform, null, false, (trn) => { trn.transform.position = spawnPos; trn.transform.LookAt(spawnPos + (dir.normalized * 5)); }, EffectSize);
        }
        else
        {
            SpawnEffect("Fx_beshot_01", 1, this.cachedTransform, null, false, (trn) => { trn.transform.position = spawnPos; }, EffectSize);        }
            
        

	   //SoundHelper.TempPlayIngameSound(SpwanUnitId, isCritical ? SoundHelper.eSoundType.critical : SoundHelper.eSoundType.hit);

        if (G_GameInfo.GameInfo.BoardPanel == null)
            return;

        //< 대미지가 0일경우는 UI표시는 안해줌
        if (damage <= 0)
            return;

        //뎀지측정
        if(attacker is Pc && attacker.m_rUUID == NetData.instance.GetUserInfo()._charUUID && G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            if(damage >= (G_GameInfo.GameInfo)._AchieveFightData.MaxDamaga)
                (G_GameInfo.GameInfo)._AchieveFightData.MaxDamaga = (uint)damage;
        }

        G_GameInfo.GameInfo.BoardPanel.ShowDamage(gameObject, attacker.gameObject, damage, isMyUnit, isCritical, false, DamResist);

        /*
        if (null != attacker && attacker.IsLeader)
        {
            if (null != attacker.Owner)
            {
                //if(G_GameInfo.GameMode != GAME_MODE.PVP)
                    G_GameInfo.GameInfo.BoardPanel.ShowCombo(attacker.Owner.AddCombo(), attacker.Owner.ComboTieCheckTime);
            }
        }
        */
    }

    /// 피격 반짝임 효과
    public void StartFlickerFx()
    {
        if (!this.gameObject.activeSelf)
            return;

        StopCoroutine( "FlickerEffect" );
        StartCoroutine( "FlickerEffect" );

        if (IsLeader)
            EventListner.instance.TriggerEvent( "TAKE_DAMAGE", this );
    }
       
    //< 죽었을시 이펙트처리
    Coroutine _DeadEffectCor;
    public void DeadEffect(float duration, System.Action _EndCB)
    {
        _DeadEffectCor = StartCoroutine(UnitEffectHelper.DeadEffect(Model.Main, duration, () => 
        {
            if (_EndCB != null)
                _EndCB();
        }));
    }

    //< 피격 당했을시 하얘지는 처리
    SkinnedMeshRenderer[] SMRs;
    WaitForSeconds FlickerDelay = new WaitForSeconds(0.15f);
    IEnumerator FlickerEffect()
    {
        if (Model == null || this.gameObject == null || !this.gameObject.activeSelf || !Model.IsReady || Model.IsNotShader || SMRs == null)
            yield break;

        #region :: for Unlit/Transparent Cutout_Custom 을 위한 피격 효과 ::

        float startFlashAmount = 0.7f;

        //< 시작하자마자 바로 하얗게 바꿈
        for (int i = 0; i < SMRs.Length; i++)
        {
            for (int j = 0; j < SMRs[i].materials.Length; j++)
            {
                SMRs[i].materials[j].SetFloat("_FlashAmount", startFlashAmount);
            }
        }

        yield return FlickerDelay;

        float ratio = 0;
        while (ratio < 1f)
        {
            ratio = Mathf.Clamp01((Time.time - flickeringStartT) / flickeringTime);

            foreach (SkinnedMeshRenderer renderer in SMRs)
                foreach (Material M in renderer.materials)
                    M.SetFloat("_FlashAmount", Mathf.Clamp01(startFlashAmount - ratio));

            yield return null;
        }

        #endregion

        #region :: for RimShader (Obsolete) ::
        ////< 시작하자마자 바로 하얗게 바꿈
        //for (int i = 0; i < SMRs.Length; i++)
        //{
        //    for (int j = 0; j < SMRs[i].materials.Length; j++)
        //    {
        //        if (UnitType != global::UnitType.Unit)
        //            SMRs[i].materials[j].SetColor("_RimColor", new Color32(255, 255, 255, 0));

        //        SMRs[i].materials[j].SetFloat("_RimPower", MinValue);
        //    }
        //}

        ////< 일정시간 대기
        //yield return FlickerDelay;

        ////< 다시 하얘지게
        //float ratio = 0;
        //flickeringStartT = Time.time;
        //while (ratio < 1f)
        //{
        //    if (Model == null || this.gameObject == null || !this.gameObject.activeSelf)
        //        yield break;

        //    ratio = Mathf.Clamp01((Time.time - flickeringStartT) / flickeringTime);
        //    if (ratio > 1)   ratio = 1;

        //    float col = ratio * 3;
        //    col += MinValue;
        //    if (col > 3)
        //        col = 3;

        //    for (int i = 0; i < SMRs.Length; i++)
        //        for (int j = 0; j < SMRs[i].materials.Length; j++)
        //            SMRs[i].materials[j].SetFloat("_RimPower", col);

        //    yield return null;
        //}

        ////< 다시 원상복귀 시켜줌
        //if (UnitType != global::UnitType.Unit)
        //{
        //    for (int i = 0; i < SMRs.Length; i++)
        //        for (int j = 0; j < SMRs[i].materials.Length; j++)
        //            SMRs[i].materials[j].SetColor("_RimColor", GameDefine.UnitRimColor);
        //}
        #endregion
    }

    //< 림 셰이더로 변경해준다.
    public void ChangeRimShader()
    {
        SkinnedMeshRenderer[] temMaterial = Model.Main.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer material in temMaterial)
            material.materials[0].shader = Shader.Find("Custom/RimSpec2");
        SMRs = UnitEffectHelper.GetHasProperty(Model.Main, "_RimPower");
    }

	public void SetShaderEnvironment()
    {
        SkinnedMeshRenderer[] temMaterial = Model.Main.GetComponentsInChildren<SkinnedMeshRenderer>();
        SceneStateBase ssb = SceneManager.instance.CurrentStateBase();

        if(ssb != null)
        {
            for (int i = 0; i < temMaterial.Length; i++)
            {
                for (int j = 0; j < temMaterial[i].materials.Length; j++)
                {
                    temMaterial[i].materials[j].SetColor("_MainColor", ssb.GetMapLightColor());
                    temMaterial[i].materials[j].SetFloat("_Intensity", ssb.GetMapIntensity());
                }
            }
        }
    }
    //< 자신에게 사용한 이펙트를 저장하기위함
    public Dictionary<bool, List<Transform>> EffectDic = new Dictionary<bool, List<Transform>>();
    public void SpawnEffect(string prefabName, float speed = 1f, Transform posAndRot = null, Transform parent = null, bool MyEffect = false, System.Action<Transform> call = null, float _scale = 1)
    {
	   //if (SimulationGameInfo.SimulationGameCheck && UnitType == global::UnitType.Unit)
	   //    UnitSimulation.UseSkillNames.Add(prefabName);

	   //UnitState UseCurrentState = CurrentState;

        G_GameInfo.SpawnEffect(prefabName, speed, posAndRot, parent, Vector3.one * _scale, (tempTrans) => 
        {
            if (tempTrans != null)
            {
                if (!EffectDic.ContainsKey(MyEffect))
                    EffectDic.Add(MyEffect, new List<Transform>());

                EffectDic[MyEffect].Add(tempTrans);

                FxMakerPoolItem poolItem = tempTrans.GetComponent<FxMakerPoolItem>();
                if (poolItem == null)
                {
                    poolItem = tempTrans.gameObject.AddComponent<FxMakerPoolItem>();
                    poolItem.destroyTime = 6;
                }

                poolItem.Owner = this;

                //< 부모가 없다면 현재 자신위치로 설정
                if (posAndRot == null && parent == null)
                {
                    tempTrans.position = this.transform.position;
                    tempTrans.rotation = this.transform.rotation;
                }

                //< 레이어 설정
                NGUITools.SetChildLayer(tempTrans, gameObject.layer);

                if (call != null)
                    call(tempTrans);
            }
        });
    }

    //< 스킬을 사용할때 이펙트(스킬 벗어날시 삭제해줘야하기때문)
    public List<Transform> SkillEffects = new List<Transform>();
    public void SpawnSkillEffect(string prefabName, float speed = 1f, Transform posAndRot = null, Transform parent = null, System.Action<Transform> call = null)
    {
        //if (SimulationGameInfo.SimulationGameCheck && UnitType == global::UnitType.Unit && UnitSimulation.UseSkillNames != null)
        //    UnitSimulation.UseSkillNames.Add(prefabName);

        G_GameInfo.SpawnEffect(prefabName, speed, posAndRot, parent, Model.Transform.localScale, (tempTrans) =>
        {
            if (tempTrans != null)
            {
                if (!EffectDic.ContainsKey(true))
                    EffectDic.Add(true, new List<Transform>());

                EffectDic[true].Add(tempTrans);

                //< 스킬 이펙트 리스트에 추가
                SkillEffects.Add(tempTrans);

                FxMakerPoolItem poolItem = tempTrans.GetComponent<FxMakerPoolItem>();
                if (poolItem == null)
                {
                    poolItem = tempTrans.gameObject.AddComponent<FxMakerPoolItem>();
                    poolItem.destroyTime = 10;
                }

                poolItem.Owner = this;

                if(parent != null)
                {
                    poolItem.SetAttach(parent);
                }

                //< 부모가 없다면 현재 자신위치로 설정
                if (posAndRot == null && parent == null)
                {
                    tempTrans.position = this.transform.position;
                    tempTrans.rotation = this.transform.rotation;
                }

                //< 레이어 설정
                NGUITools.SetChildLayer(tempTrans, gameObject.layer);

			 if (call != null)
			 {
				call(tempTrans);
			 }
            }
        });
    }
    
    //< 이펙트 모두 삭제
    public void EffectClear(bool all = false)
    {
        //< 모든 이펙트 삭제
        if (!EffectDic.ContainsKey(true))
            return;

        for(int i=0; i<EffectDic[true].Count; i++)
        {
            if(EffectDic[true][i] != null)
            {
                FxMakerPoolItem obj = EffectDic[true][i].gameObject.GetComponent<FxMakerPoolItem>();
                if(obj != null)
                    obj.ManualDespawn();
                else
                    Destroy(EffectDic[true][i].gameObject);
            }
        }

        if(all)
        {
            if (!EffectDic.ContainsKey(false))
                return;

            for (int i = 0; i < EffectDic[false].Count; i++)
            {
                if (EffectDic[false][i] != null)
                {
                    FxMakerPoolItem obj = EffectDic[false][i].gameObject.GetComponent<FxMakerPoolItem>();
                    if (obj != null)
                        obj.ManualDespawn();
                    else
                        Destroy(EffectDic[false][i].gameObject);
                }
            }
        }
    }

    public void EffectClear(Transform effect)
    {
        FxMakerPoolItem obj = effect.gameObject.GetComponent<FxMakerPoolItem>();
        if (obj != null)
            obj.ManualDespawn();
        else
            Destroy(effect.gameObject);
    }

    //< 스킬 이펙트 모두 삭제
    public void SkillEffectClear(bool destroy)
    {
        if (destroy)
        {
            for (int i = 0; i < SkillEffects.Count; i++)
            {
                if (SkillEffects[i] != null)
                {
                    FxMakerPoolItem obj = SkillEffects[i].gameObject.GetComponent<FxMakerPoolItem>();
                    if (obj != null)
                        obj.ManualDespawn();
                    else
                        Destroy(SkillEffects[i].gameObject);
                }
            }
        }
        
        SkillEffects.Clear();
    }

    void SetEffectActive(GameObject model, bool type)
    {
        ParticleSystem[] Particles = model.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem par in Particles)
        {
            if (type)
            {
                par.Pause(true);
                par.Stop(true);
            }
            else
            //tempTrans = G_GameInfo.SpawnEffect( prefabName, speed, posAndRot, parent, Model.Transform.localScale );
            {
                par.Play(true);
            }
        }

        //return tempTrans;

        NcEffectBehaviour[] ncComs = model.GetComponentsInChildren<NcEffectBehaviour>(true);
        foreach (NcEffectBehaviour ncCom in ncComs)
        {
            ncCom.enabled = !type;
        }
    }

    Shader Additive, Alpha_Blended;
    Shader CustomAlphaShader_Additive, CustomAlphaShader_Object;
    public void SetAllObjectLayer(bool type)
    {
        // type == true일경우에는 스킬연출 대상 false에는 비대상

        //<=====================================
        //<             이펙트 변경
        //<=====================================            
        foreach (KeyValuePair<bool, List<Transform>> obj in EffectDic)
        {
            List<Transform> _EffectList = obj.Value;
            for (int i = 0; i < _EffectList.Count; i++)
            {
                if (_EffectList[i] == null)
                    continue;

                //< 셰이더를 직접 변경해준다.
                SetShader(_EffectList[i].gameObject, type);
            }
        }

        //<=====================================
        //<         프로젝트타일 변경
        //<=====================================
        for (int i = 0; i < ProjecttileList.Count; i++)
        {
            if (ProjecttileList[i] != null)
                NGUITools.SetChildLayer(ProjecttileList[i].transform, type ? 11 : 0);
        }
    }

    //< 셰이더를 변경한다.
    void SetShader(GameObject obj, bool type)
    {
        if (Additive == null)
        {
            Additive = Shader.Find("Particles/Additive");
            Alpha_Blended = Shader.Find("Particles/Alpha Blended");
            CustomAlphaShader_Additive = Shader.Find("Custom/CustomAlphaShader_Additive");
            CustomAlphaShader_Object = Shader.Find("Custom/CustomAlphaShader_Object");
        }

        MeshRenderer[] list = obj.GetComponentsInChildren<MeshRenderer>();
        if (list != null)
        {
            for (int j = 0; j < list.Length; j++)
            {
                //< 셰이더를 직접 변경해준다.
                ChangeShader(list[j].sharedMaterial.shader, type);
            }
        }

        SkinnedMeshRenderer[] list2 = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (list2 != null)
        {
            for (int j = 0; j < list2.Length; j++)
            {
                //< 셰이더를 직접 변경해준다.
                ChangeShader(list2[j].sharedMaterial.shader, type);
            }
        }

        ParticleSystem[] list3 = obj.GetComponentsInChildren<ParticleSystem>();
        if (list3 != null)
        {
            for (int j = 0; j < list3.Length; j++)
            {
                //< 셰이더를 직접 변경해준다.
                ChangeShader(list3[j].renderer.materials[0].shader, type);
            }
        }
    }

    void ChangeShader(Shader shader, bool type)
    {
        if (type)
        {
            if (shader.name == "Particles/Additive")
                shader = CustomAlphaShader_Additive;
            else if (shader.name == "Particles/Alpha Blended")
                shader = CustomAlphaShader_Object;
        }
        else
        {
            if (shader.name == "Custom/CustomAlphaShader_Additive")
                shader = Additive;
            else if (shader.name == "Custom/CustomAlphaShader_Object")
                shader = Alpha_Blended;
        }
    }

    //< 자신의 포메이션에 맞게 위치를 셋팅한다.
    public void SetPosition(int allyCount, int FormationNo, Unit ReaderUnit)
    {
        Vector3 dir = Vector3.zero;

        if (allyCount == 4)
        {
            float angle = 120f - FormationNo * 45f;
            dir = Quaternion.Euler(0, angle, 0) * -ReaderUnit.transform.forward;
        }
        else if (allyCount == 3)
        {
            float angle = 130f - FormationNo * 65f;
            dir = Quaternion.Euler(0, angle, 0) * -ReaderUnit.transform.forward;
        }
        else if (allyCount == 2)
        {
            float angle = 70f - FormationNo * 50f;
            dir = Quaternion.Euler(0, angle, 0) * -ReaderUnit.transform.forward;
        }
        else if (allyCount == 1)
        {
            dir = -ReaderUnit.transform.forward;
        }

        dir = (dir * (ReaderUnit.Radius + ReaderUnit.Radius)) * 1.5f;

        this.transform.position += dir;
    }

    public float DefaultAniSpeed = 1;
    public void SetAniSpeed(float speed)
    {
        //< 속도 저장
        if (speed != 0)
            DefaultAniSpeed = speed;

        //<==============================
        //<         애니메이션 처리
        //<==============================
        if (Animator != null)
        {
            foreach (eAnimName eEnum in eAnimName.GetValues(typeof(eAnimName)))
            {
                if (eEnum == eAnimName.Anim_none || eEnum == eAnimName.Anim_Max)
                    continue;

                if (Animator.GetAnimName(eEnum) == null || Animator.GetAnimName(eEnum) == "0")
                    continue;

                if (Animator.Animation.GetClip(Animator.GetAnimName(eEnum)) == null)
                    continue;

                AnimationState state = Animator.Animation[Animator.GetAnimName(eEnum)];
                if (state != null)
                {
                    if (speed < 1)
                        state.speed = speed;
                    else
                    {
                        if (eEnum == eAnimName.Anim_move || eEnum == eAnimName.Anim_damage)
                            state.speed = 1;
                        else
                            state.speed = speed;
                    }
                }
            }
        }
    }

    #endregion

    //< 인트로 연출이있을시 처리
    public void SetIntroEvent(List<string> aniList, string effect)
    {
        //< 업데이트 실행
        StartCoroutine(IntroEventUpdate(aniList, effect));
    }

    IEnumerator IntroEventUpdate(List<string> aniList, string effect)
    {
        //< 일단 못움직이게 막아줌
        StaticState(true);

        //< 애니메이션 실행
        int idx = 0;
        float LimitTime = 0;

        while(true)
        {
            if (!Model.Main.animation.GetClip(aniList[idx]))
                break;

            if (idx == 0)
            {
                //< 보스라면 컷씬 이벤트로 인해 잠깐 스톱해놓음
                if (UnitType == global::UnitType.Boss)
                {
                    Model.Main.SetActive(false);
                    yield return new WaitForSeconds(0.5f);
                    Model.Main.SetActive(true);
                }

                //< 이펙트도 띄워줌
                if (effect != "")
                    SpawnEffect(effect);

                Model.Main.animation.Play(aniList[idx]);
            }
            else
                Model.Main.animation.CrossFade(aniList[idx], 0.5f);

            LimitTime = Time.time + Animator.GetAnimLength(aniList[idx]);
            idx++;

            while (true)
            {
                if (Time.time >= LimitTime)
                    break;

                yield return null;
            }

            if (idx >= aniList.Count)
                break;
        }

        //< 아이들 애니메이션 실행
        PlayAnim((SceneManager.instance.CurrentStateBase() as TownState) != null ? eAnimName.Anim_idle : eAnimName.Anim_battle_idle, true, 1, true);

        StaticState(false);
        ChangeState(UnitState.Idle);
        
        yield return null;
    }

    public void DetachBuffType(BuffType type)
    {
        if (CharInfo.GetBuffValue(type) > 0f)
        {
            if (SceneManager.instance.IsRTNetwork && (m_rUUID == NetData.instance._userInfo._charUUID))
            {
                //네트워크로 버프삭제요청

            }
            else
            {
                BuffCtlr.DetachBuff(type);
            }
        }
    }

    /// 수동 공격시 사용.
    public void ManualAttack()
    {
        //if (!Usable || CurrentState == UnitState.Skill || CurrentState == UnitState.Evasion)
		if (!Usable || CurrentState == UnitState.Skill)
            return;

        // 지금 안전 지대인지에 있는지? 검사
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        {
            if (IsLeader == true)
            {
                //안전지대 설정
            }
        }

        if (isChainTime())
        {
            //체인스킬로 변경
            switch (ChainLevel)
            {
                case 0:
                    {
                        UseSkillSlot = (uint)5;
                        UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(5);
                        ChangeState(UnitState.Skill);
                        ChainEndTime = float.MaxValue;
                    }
                    break;
                case 1:
                    {
                        UseSkillSlot = (uint)6;
                        UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(6);
                        ChangeState(UnitState.Skill);
                        ChainEndTime = float.MaxValue;
                    }
                    break;
                default:
                    {
                        UseSkillSlot = (uint)7;
                        UseSkillIdx = (uint)SkillCtlr.GetSkillIndex(7);
                        ChangeState(UnitState.Skill);
                        ChainEndTime = float.MaxValue;
                    }
                    break;

            }

            if (G_GameInfo.GameInfo != null && G_GameInfo.GameInfo.HudPanel != null && m_rUUID == NetData.instance._userInfo._charUUID)
                G_GameInfo.GameInfo.HudPanel.ChangeAttackImg(0, SkillCtlr.GetSkillIndex(5), SkillCtlr.SkillList[5].GetSkillActionInfo().callChainTime );
            return;
        }

        if (CurrentState == UnitState.ManualAttack)
        {
            if(SkillBlend)
            {
                ChangeState(UnitState.Idle);
                ChangeState(UnitState.ManualAttack);
            }
        }
        else
        {
            ChangeState(UnitState.ManualAttack);
        }
    }
    
    /// [방향표시기] 화면 밖에 존재하는 적의 위치를 알려준다.
    void OnUpdateIndicator()
    {
        if (G_GameInfo.GameMode != GAME_MODE.SINGLE)
            return;

        List<Unit> unitList = G_GameInfo.CharacterMgr.AliveList( 1 );

        int outsideCount = unitList.Count;

        //if (outsideCount > 0 && insideCount == 0)
        if (outsideCount > 0)
        {
            if (this.TargetID == GameDefine.unassignedID)
            {
                // 플레이어와 가까운 최대 5마리까지 찾고, 나머지는 제거
                unitList.Sort( (u1, u2) =>
                {
                    float u1Dist = Vector3.Distance( transform.position, u1.transform.position );
                    float u2Dist = Vector3.Distance( transform.position, u2.transform.position );
                    return u1Dist.CompareTo( u2Dist );
                } );

                dirIndicator.Show( unitList[0].transform.position );
            }
            else
            {
                Unit target = null;
                if (!G_GameInfo.CharacterMgr.InvalidTarget( this.TargetID, ref target ))
                    dirIndicator.Show( target.transform.position );
            }                    
        }
        else
        {
            //< 유닛이 없을경우에는 내가 가야할 위치를 얻는다
            if(SpawnGroup.ActiveSpawnGroup != null)
                dirIndicator.Show(SpawnGroup.ActiveSpawnGroup.transform.position);
            else
                dirIndicator.Hide();
        }
    }

    void OnUpdateTargetIndicator()
    {
        //< 레이드라면 타겟이 있을경우만 활성화
        if(G_GameInfo.GameMode == GAME_MODE.RAID)
        {
            //if (RaidBossAIBase.SelectCameraTarget != null && !RaidBossAIBase.SelectCameraTarget.IsDie)
            //    dirIndicator.Show(RaidBossAIBase.SelectCameraTarget.transform.position);
            //else 
                dirIndicator.Hide();
        }
    }

    //< 승리포즈 처리를 위한 코루틴
    public static IEnumerator VictoryAniUpdate(GameObject modelGO, string victoryAni, string idleAni)
    {
        if (!modelGO.animation.GetClip(victoryAni))
        {
            modelGO.animation.CrossFade(idleAni, 0.2f);
            yield break;
        }

        modelGO.animation.CrossFade(victoryAni, 0.2f);
        while(true)
        {
            if (modelGO == null || !modelGO.animation.isPlaying)
                break;

            yield return null;
        }

        if (modelGO != null)
            modelGO.animation.CrossFade(idleAni, 0.2f);
    }

    //< 네브메쉬 크기를 제어한다(연출용)
    public float OrgNavRadius = 0;
    public void ZeroNavAgentRedius(bool type)
    {
        if (type)
            navAgent.radius = 0;
        else
            navAgent.radius = Radius;
    }

    public virtual void WeaponEffectsUpdate(object arg)
    {

    }

    protected GameObject[] WeaponEffects = { null, null };
    /// <summary>
    /// 무기 이펙트를 달아준다.
    /// 추후에는 테이블 정보등을 참조해야 함
    /// </summary>
    /// 


    public virtual void AttachWeaponEffect()
    {
        
    }

    /// <summary>
    /// 다른곳에서도 쓰기위해서
    /// </summary>
    /// <param name="EffectRootName"></param>
    /// <param name="modelGo"></param>
    public static GameObject AttachedWeaponEffect(string EffectRootName, string dummyname, GameObject modelGo, bool LayerChange = false)
    {
	   Transform dummyTrs = null;
	   GameObject oriEffect = Resources.Load(EffectRootName) as GameObject;
	   if (oriEffect == null) Debug.LogError("2JW : Effect Not Find - " + EffectRootName);
	   //Debug.LogWarning("2JW : -------------------------- " + modelGo, modelGo);
	   Transform[] trs = modelGo.GetComponentsInChildren<Transform>(true);
	   for (int i = 0; i < trs.Length; i++)
	   {
		  if (trs[i].name == dummyname)
		  {
			 dummyTrs = trs[i];
		  }
	   }
	   GameObject go = null;
	   //Debug.LogWarning("2JW : dummyTrs - " + dummyTrs[i], dummyTrs[i]);
	   if (dummyTrs != null && oriEffect != null)
	   {
		  go = Instantiate(oriEffect) as GameObject;
		  go.transform.parent = dummyTrs;
		  go.transform.localScale = Vector3.one;
		  go.transform.localPosition = Vector3.zero;
		  go.transform.localRotation = Quaternion.identity;
		  if (LayerChange)
			 NGUITools.SetLayer(go, modelGo.layer);
	   }
	   return go;
    }

    public virtual void ShowHeadObj(bool unShow)
    {
        // 생성
        //if(SceneManager.instance.optionData.ShowHpBar == "1111" && SceneManager.instance.optionData.ShowName == " 111")

    }

    #region :: Debug ::

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        //if (!SimulationGameInfo.SimulationGameCheck)
            DisplayGizmos();
    }

    void OnDrawGizmos()
    {
        //if (SimulationGameInfo.SimulationGameCheck)
            DisplayGizmos();
    }
    void DisplayGizmos()
    {
        if (null != movePath)
        {
            for (int i = 1; i < movePath.corners.Length; ++i)
            {
                Debug.DrawLine( movePath.corners[i - 1], movePath.corners[i], Color.cyan );
                Debug.DrawLine( movePath.corners[i], movePath.corners[i] + Vector3.up * i, Color.blue );
            }
        }

        if (this is TownUnit)
            return;

        if (this is MyTownUnit)
            return;

        //if (null != CharInfo)
        //{ 
        //    UnityEditor.Handles.color = Color.red;
        //    UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.up, CharInfo.AtkRecognition );
        //    UnityEditor.Handles.Label( transform.position + (transform.right * CharInfo.AtkRecognition), "적 인지영역" );
            
        //    UnityEditor.Handles.color = Color.cyan;
        //    UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.up, Radius );
        //    UnityEditor.Handles.Label( transform.position + (transform.right * Radius), "피격가능거리" );

        //    UnityEditor.Handles.color = Color.white;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, CharInfo.RushAtkRange + Radius);
        //    UnityEditor.Handles.Label(transform.position + (transform.right * (CharInfo.RushAtkRange + Radius)), "Dash공격가능거리");

        //    UnityEditor.Handles.color = Color.grey;
        //    UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, CharInfo.AtkRange + Radius);
        //    UnityEditor.Handles.Label(transform.position + (transform.right * (CharInfo.AtkRange + Radius)), "최소공격사거리");

        //    UnityEditor.Handles.color = Color.black;
        //    UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.up, CharInfo.FirstAtkRange + Radius );
        //    UnityEditor.Handles.Label( transform.position + (transform.right * (CharInfo.FirstAtkRange + Radius)), "첫공격사거리" );

        //    UnityEditor.Handles.color = new Color( 1, 1, 1, 0.2f );
        //    float minAtkAngle = CharInfo.AtkAngle != 0 ? CharInfo.AtkAngle : 1f;
        //    Vector3 startVec = Quaternion.Euler( 0, minAtkAngle * 0.5f, 0 ) * transform.forward;
        //    UnityEditor.Handles.DrawSolidArc( transform.position, transform.up, startVec, -minAtkAngle, CharInfo.AtkRange + Radius );
            
        //    UnityEditor.Handles.color = Color.white;
        //}

        //if (null != SkillCtlr && null != SkillCtlr.GetDataInfo(UseSkillSlot) )
        { 
            //UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.up, SkillCtlr.GetDataInfo( UseSkillSlot ).Range );
            //UnityEditor.Handles.Label( transform.position + (transform.forward * SkillCtlr.GetDataInfo( UseSkillSlot ).Range), UseSkillSlot + ":SkillRange" );
        }
    }

    Vector3 ScreenPoint;
    Rect    WorldToScreenRt;
    Vector2 scrollPos;
    public bool showSkillInfo = false;

    void OnGUI()
    {
        if (!ShowUnitDebugView)
            return;

        Vector3 screenpoint = Camera.main.WorldToScreenPoint(transform.position);
        screenpoint.y = Camera.main.pixelHeight - screenpoint.y;
        ScreenPoint = screenpoint;
        WorldToScreenRt.width = Camera.main.pixelWidth;
        WorldToScreenRt.height = Camera.main.pixelHeight - 72f;
        WorldToScreenRt.x = ScreenPoint.x - Camera.main.pixelWidth * 0.5f;
        WorldToScreenRt.y = ScreenPoint.y - Camera.main.pixelHeight * 0.5f;

        GUILayout.BeginArea(WorldToScreenRt);
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(showDebug ? "Hide" : "Show"))
                showDebug = !showDebug;

            scrollPos = GUILayout.BeginScrollView(scrollPos, "box");

            OnGUIStatus();

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    void OnGUIStatus()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.richText = true;

        if (showDebug)
        {
            if (null != CharInfo)
                GUILayout.TextArea( CharInfo.ToString(), style, GUILayout.Width(500) );
        }
        if (showSkillInfo)
            ShowSkillInfo();
    }

    void ShowSkillInfo()
    {
        Color saveColor = GUI.contentColor;
        GUI.contentColor = Color.red;
        if (null != SkillCtlr)
            GUILayout.Label( SkillCtlr.ToString() );
        GUI.contentColor = saveColor;
    }

#endif

    #endregion
}
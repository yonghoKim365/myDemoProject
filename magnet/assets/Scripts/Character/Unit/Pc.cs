using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class Pc : Unit
{
    /// <summary>
    /// 서버로 부터 받은 캐릭터 초기화에 필요한 초기 데이터들
    /// </summary>
    public PlayerUnitData syncData;
    
    // RootMotion
    public KRootMotionRM   rootMotion;
    
    //public LowDataMgr.UnitCompositionData  comData;

    public GameObject  ringShadow;

    public override bool IsLeader
    {
        get
        {
            return base.IsLeader;
        }
        set
        {
            base.IsLeader = value;

            if (!IsLeader && null != inputCtlr)
            {
                inputCtlr.Stop();
            }
        }
    }
    
    #region :: MonoBehaviour ::
    
    public override void Awake()
    {
        base.Awake();

        //EventListner.instance.RegisterListner( LevelUpEffect );
    }

    protected override void OnDestroy()
    {
        //EventListner.instance.RemoveEvent( LevelUpEffect );

        base.OnDestroy();
    }

    #endregion

    /// <summary>
    /// NetworkView로 부터 받아온 초기화 데이터들을 셋팅하도록 한다.
    /// </summary>
    protected override void Init_SyncData(params object[] args)
    {
        base.Init_SyncData(args);

        //syncData = (UnitSyncData)args[3];
        syncData = (PlayerUnitData)args[3];

        AttackType = 1;// LowDataMgr.GetUnitAbilityData(syncData.LowID).attackType;
    }

    protected override void Init_FSM()
    {
        GK_FSMFactory.SetupFSM( this, UnitType, out FSM );
    }

    protected override void Init_Datas()
    {
        base.Init_Datas();

    }

    protected override void Init_Model()
    {
        //AssetbundleLoader.AssetLoad( AssetbundleLoader.CreateSuffixForPlatform( resInfo.modelFile ), OnLoadedModelGO );

        //추후 다 어셋번들에서 읽어오게 해야함
        GameObject _unit = null;
        if (syncData._isPartner)
        {
            Partner.PartnerDataInfo partner = _LowDataMgr.instance.GetPartnerInfo(syncData._partnerID);
            _unit = UnitModelHelper.PartnerModelLoad(   syncData._partnerID, 
                                                        ref WeaponEffects, 
                                                        true,
                                                        partner.RightWeaDummy,
                                                        partner.LeftWeaDummy, QualityManager.instance.GetModelQuality());

            CharInfo.animDatas = _LowDataMgr.instance.AniInfoSetting( partner.prefab,
                                                                      _unit,
                                                                      partner.AniId);
        }
        else
        {
            _unit = UnitModelHelper.PCModelLoad( syncData._charIdx,
                                                 syncData._HeadItem,
                                                 syncData._CostumeItem,
                                                 syncData._ClothItem,
                                                 syncData._WeaponItem,
                                                 syncData._HideCostume, 
                                                 ref WeaponEffects, QualityManager.instance.GetModelQuality());


            //무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
            CharInfo.animDatas = _LowDataMgr.instance.AniInfoSetting(_LowDataMgr.instance.GetLowDataSkillSet(syncData._SkillSetId).AniPath,//_LowDataMgr.instance.GetLowDataCostumeInfo(syncData._CostumeItem).Bodyprefab,
                                                                      _unit,
                                                                      _LowDataMgr.instance.GetLowDataSkillSet(syncData._SkillSetId).AniId);
                                                                      //_LowDataMgr.instance.GetLowDataCostumeInfo(syncData._CostumeItem).AniId);
        }
        
                                                        

        CharInfo.MaxAnimCombo = UnitDataFactory.CountingMaxAnimCombo(CharInfo.animDatas);

        OnLoadedModelGO(_unit);
        NGUITools.SetLayer(gameObject, 14);
		
		SetShaderEnvironment();
    }

    public override void AttachWeaponEffect()
    {
        //어차피 몬스터랑 유닛이랑 다르게 처리
        //base.AttachWeaponEffect();
        string EffectRootName = "";

        if (syncData._isPartner)
        {
            Partner.PartnerDataInfo partner = _LowDataMgr.instance.GetPartnerInfo(syncData._partnerID);

            if(partner!=null)
            {
                if (!partner.RightWeaDummy.Equals("none"))
                {
                    EffectRootName = string.Format("Effect/_PARTNER/{0}", partner.RightWeaDummy);
                    WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", gameObject);
                }

                if (!partner.LeftWeaDummy.Equals("none"))
                {
                    EffectRootName = string.Format("Effect/_PARTNER/{0}", partner.LeftWeaDummy);
                    WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", gameObject);
                }
            }            
        }
        else//Hero?
        {
            if (syncData._HideCostume)
            {
                Item.EquipmentInfo weaponItem = _LowDataMgr.instance.GetLowDataEquipItemInfo(syncData._WeaponItem);

                if (weaponItem != null)
                {
                    if (!weaponItem.RightWeaDummy.Equals("none"))
                    {
                        EffectRootName = string.Format("Effect/_PC/{0}", weaponItem.RightWeaDummy);
                        WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", gameObject);
                    }
                    if (!weaponItem.LeftWeaDummy.Equals("none"))
                    {
                        EffectRootName = string.Format("Effect/_PC/{0}", weaponItem.LeftWeaDummy);
                        WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", gameObject);
                    }
                }                
            }
            else
            {
                Item.CostumeInfo costume = _LowDataMgr.instance.GetLowDataCostumeInfo(syncData._CostumeItem);

                if (costume != null)
                {
                    if (!costume.RightWeaDummy.Equals("none"))
                    {
                        EffectRootName = string.Format("Effect/_PC/{0}", costume.RightWeaDummy);
                        WeaponEffects[0] = AttachedWeaponEffect(EffectRootName, "wp_dummy_01", gameObject);
                    }

                    if (!costume.LeftWeaDummy.Equals("none"))
                    {
                        EffectRootName = string.Format("Effect/_PC/{0}", costume.LeftWeaDummy);
                        WeaponEffects[1] = AttachedWeaponEffect(EffectRootName, "wp_dummy_02", gameObject);
                    }
                }
            }
        }
    }

    protected override void Init_Controllers()
    {
        base.Init_Controllers();

        //< 스킬 AI 대입시켜줌(자동스킬을 사용했을시에만)
        //if (IsPartner || G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING || NetData.instance.GetDoubleBuffCheck(InGameBuffItemType.AutoSkill) != 1)
        //if (IsPartner || G_GameInfo.GameMode == GAME_MODE.PVP || G_GameInfo.GameMode == GAME_MODE.SPARRING )
        {
            skill_AI = gameObject.AddComponent<Skill_AI>();
            skill_AI.Setup(this);
        }
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        navAgent.avoidancePriority = 45;

        // RootMotion 추가
        //if(G_GameInfo.GameMode != GAME_MODE.PVP)
            rootMotion = gameObject.AddComponent<KRootMotionRM>();

        IsLeader = true;

        //< 라이트 추가
        if(!syncData._isPartner)
            AddLight();
    }

    protected override void ComputeComponents()
    {
        base.ComputeComponents();
        
        float modelX = 1.43f;// comData.dataInfo.modelX;
        float modelY = 1.9f;// comData.dataInfo.modelY;
        float modelZ = 1.43f;// comData.dataInfo.modelZ;

        navAgent.radius = (modelX / 2) * Model.OriginalScale;
        navAgent.height = modelY * Model.OriginalScale;
        //navAgent.baseOffset = CharInfo.AirHeight;
        navAgent.baseOffset = 0f;

        UnitCollider = gameObject.GetComponent<BoxCollider>();
        UnitCollider.size = Vector3.Scale(new Vector3(modelX, modelY, modelZ), Model.ScaleVec3);
        UnitCollider.center = new Vector3(UnitCollider.center.x, UnitCollider.size.y * 0.5f, UnitCollider.center.z);

        if(GameDefine.skillPushTest)
        {
            if(syncData._isPartner)
            {
                //파트너면 CapsuleColider를 꺼줌
                CapsuleCollider colider = gameObject.GetComponent<CapsuleCollider>();
                colider.enabled = false;

            }
        }

        if (syncData._isPartner)
        {
            gameObject.layer = LayerMask.NameToLayer("Unit");
        }

        Radius = navAgent.radius;
        Height = UnitCollider.size.y;

        AddShadow(new Vector3(UnitCollider.size.x, UnitCollider.size.z, 1));

	   // RootMotion을 위한 셋팅
	   if (rootMotion != null)
	   {
            string AniRMName = "";

            if (syncData._isPartner)
            {
                AniRMName = _LowDataMgr.instance.GetPartnerInfo(syncData._partnerID).prefab;
            }
            else
            {
                AniRMName = _LowDataMgr.instance.GetLowDataCostumeInfo(syncData._CostumeItem).Bodyprefab;
            }            

            AssetbundleLoader.GetRMCurves( AniRMName , (dic) =>
            {
                rootMotion.Init(dic, transform, Animator.Animation, Model.FindAndCaching("Bip001"));
            });
        }

    }

    protected override void SetupForGameMode(GAME_MODE gameMode)
    {
        /*
        if (gameMode == GAME_MODE.PVP)
        {
            // Material에 직접 접근해서 색상변경시 Instance된다.
            if (TeamID != 0 && null != ringShadow)
                ringShadow.renderer.material.SetColor( "_AddColor", Color.red );
        }
        else if (gameMode == GAME_MODE.SPARRING)
        {
            if (null != inputCtlr)
                inputCtlr.StopJoystick();
        }
        */
    }
    
    /// 모델 생성 준비 완료시 호출할 콜백
    protected void OnLoadedModelGO(GameObject modelGO)//GameObject prefabObj)
    {
        if (Model.IsReady)
            Model.DeleteModel();

        //< 모델 생성//Init_Model함수에서 생성함.
        //GameObject modelGO = GameObject.Instantiate( prefabObj ) as GameObject;

        //< 애니메이션 대입
        //AssetbundleLoader.AddAnimationClips(modelGO.animation, CharInfo.animDatas, () =>
        //{
            AniEvent ani = modelGO.AddComponent<AniEvent>();
            ani.MyUnit = this;


		    Model.Init(this.gameObject, modelGO, 1.3f);
            if (null != modelGO.animation)
            {
                Animator.Init(this.gameObject, modelGO.animation, CharInfo.animDatas);
                Animator.Animation.playAutomatically = false;  //기본 애니메이션이(Animation.clip) 프로그램이 시작될때 자동으로 재생되어야 하는지의 대한 여부.

                Animator.PlayAnim(eAnimName.Anim_battle_idle);
            }

            //ExtraModels = GK_ModelCreator.SetupExtraModels(Model, resInfo.dummyId);

            //////////////////////////
            LoadingComplete();
	   //});

	   //< 각성 셋팅
	   //UIHelper.SetAwakenModel(modelGO, syncData.AwakenType, syncData.LowID);

	   //무기 이펙트 설정
	   AttachWeaponEffect();

	   //< 모든 트랜스폼을 미리 저장해놓는다
	   Transforms = modelGO.GetComponentsInChildren<Transform>(true);
    }
    
    public override void Die(Unit attacker, float pushPower = 1f)
    {
        base.Die( attacker, pushPower );

        if (null != inputCtlr)
            inputCtlr.Stop();

        if (dirIndicator != null)
        {
            dirIndicator.Hide();
            dirIndicator.enabled = false;
        }
    }

    public override bool PlayAnim(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        bool doPlaying = base.PlayAnim( animEvent, CrossFade, CrossTime, canPlaySameAnim, queued );
        if (false == doPlaying)
            return doPlaying;

        //일단 임시!!!
        bool ignoreEnemy = false;

        if(CurrentState == UnitState.Skill)
        {
            ActionInfo actionIfno = SkillCtlr.SkillList[UseSkillSlot].GetSkillActionInfo();

            if (actionIfno != null)
            {
                if (actionIfno.skillpass == 0)
                {
                    ignoreEnemy = false;
                }
                else
                {
                    ignoreEnemy = true;
                }
            }
            else
            {
                ignoreEnemy = false;
            }
        }
        else
        {
            ignoreEnemy = false;
        }
        
        Resource.AniInfo aniInfo = CharInfo.animDatas[(int)animEvent];
        if (UsableNavAgent && rootMotion != null)
            rootMotion.Play( Animator.CurrentAnimState, aniInfo.rootMotion, ignoreEnemy);

        return true;
    }

    public override bool PlayAnimNoRootMation(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        return base.PlayAnim(animEvent, CrossFade, CrossTime, canPlaySameAnim, queued);
    }

    public override bool SetTarget(int instanceID)
    {
        if (null != inputCtlr)
            inputCtlr.ShowTargetFx(instanceID);

        return base.SetTarget(instanceID);
    }
    public override void ShakeCamera(Vector3 shakeDir, byte type = 1)
    {
        base.ShakeCamera(shakeDir, type);
    }

    public override void StaticState(bool doStatic = true, float fadeDelay = 0.1f)
    {
        if (null != rootMotion && rootMotion.IsPlayingMotion)
            rootMotion.End();

        if (!doStatic)
        {
            ShowHeadObj(true);
        }
        base.StaticState(doStatic, fadeDelay);
    }

    public override void Revive()
    {
        base.Revive();

        if (m_rUUID == NetData.instance._userInfo._charUUID)
        //if (null != Owner)
        {
            if (IsLeader)
                G_GameInfo.GameInfo.ChangeLeader(this);

            if (dirIndicator)
			    dirIndicator.enabled = true;

            if (inputCtlr != null)//살아 났으니 조이스틱 다시 연결
                inputCtlr.ActivateJoystick(true);

        }
    }

    public override void AddShadow(Vector3 scale)
    {
        base.AddShadow( scale );

        if (null != ringShadow)
            return;

	   //< 해당 프리팹을 생성해서 메테리얼을 새로 생성후 색상 변경보다, 차라리 두개의 프리팹을 만들고 한번만 생성하는게
	   //< 효율적일것 같아서 두개중에 불러오도록 처리
	   //if (TeamID != 0 && G_GameInfo.GameMode == GAME_MODE.PVP)
		  //ringShadow = ResourceMgr.Instantiate<GameObject>("Character/RingShadow_Target");
	   //else
		  //ringShadow = ResourceMgr.Instantiate<GameObject>("Character/Shadow");

	   //ringShadow.transform.AttachTo(transform, new Vector3(0f, 0.15f, 0f), scale, Quaternion.Euler(new Vector3(90, 0, 0)));
	   //ringShadow.transform.localScale = new Vector3(scale.x, scale.x, scale.x);
    }

    public override void DeleteShadow()
    {
        base.DeleteShadow();

        if (ringShadow == null)
            return;

        ringShadow.transform.parent = null;
        Destroy( ringShadow );
        ringShadow = null;
    }

    public override int TakeDamage(Unit attacker, float damageRatio, float damage, float AddDamage, eAttackType atkType, bool isSkillDamage, AbilityData _ability, bool through = false, bool projecttile = false)
    {
        
        if (G_GameInfo.GameMode == GAME_MODE.SINGLE && !IsPartner)//플레이어만 체크
        {
            int count = SingleGameState.StageQuestList.Count;
            for(int i=0; i < count; i++)
            {
                if (SingleGameState.StageQuestList[i].CheckCondition(ClearQuestType.MINIMUM_HIT, 1))
                {
                    if (!SingleGameState.StageQuestList[i].IsClear)//다 맞았음 (망했음)
                    {
                        UIBasePanel panel = UIMgr.GetHUDBasePanel();
                        if (panel != null) {
                            (panel as InGameHUDPanel).SetQuestStringColor(i, false);
                        }
                    }
                    break;//횟수 증가함 더이상 조건문 돌지않는다.
                }
            }
        }

        int val = base.TakeDamage(attacker, damageRatio, damage, AddDamage, atkType, isSkillDamage, _ability, through, projecttile);

        return val;
    }

    /// <summary>
    /// 순간이동을 시키며, 유닛들까지 같이 이동시킨다 (Portal을 위해 만듬)
    /// </summary>
    /// <param name="position"></param>
    public void Teleport(Vector3 position)
    {
        if (null == Owner)
            return;
        
        navAgent.enabled = false;
        transform.position = position;
        navAgent.enabled = true;

        foreach (Pc unit in Owner.Units)
        {
            if (null == unit)
                continue;
            
            NavMeshAgent na = unit.GetComponent<NavMeshAgent>();
            if (null != na)
            {
                na.enabled = false;
                unit.transform.position = position;
                na.enabled = true;
            }
        }

        CheckNavMesh();
    }

    void CheckNavMesh()
    {
        G_GameInfo.GameInfo.NextNavMeshGroup();

        //if (autoMode)
        //    GoToLastLocation();
    }

    public override void ShowHeadObj(bool unShow)
    {
        base.ShowHeadObj(unShow);
        if (gameObject.activeSelf && G_GameInfo.GameInfo.BoardPanel != null)
        {
            bool isMy = false;
            if (SceneManager.instance.IsRTNetwork)
            {
                if (m_rUUID == NetData.instance.GetUserInfo().GetCharUUID())//나 자신인지!IsPartner && 
                    isMy = true;
            }
            else
            {
                if (syncData._TCPUUID == 0)//내꺼임
                    isMy = true;
            }

            G_GameInfo.GameInfo.BoardPanel.ShowHead(gameObject, syncData._Name, syncData._Prefix, syncData._Suffix, isMy);
        }

    }

    ////진입시
    private void OnTriggerEnter(Collider other)
    {
        //난투장에서만체크
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)

            if (!(G_GameInfo.GameInfo as FreeFightGameInfo).IsSafetyZone &&
                m_rUUID == NetData.instance.GetUserInfo()._charUUID)
            {
                if (other.gameObject.tag.Contains("SafetyZone"))
                    (G_GameInfo.GameInfo as FreeFightGameInfo).IsSafetyZone = true;
            }
        
    }


    //나갈떄
    private void OnTriggerExit(Collider other)
    {
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
        {
            if (m_rUUID == NetData.instance.GetUserInfo()._charUUID)
             //난투장에서만체크
            {
                if (other.gameObject.tag.Contains("SafetyZone"))
                    (G_GameInfo.GameInfo as FreeFightGameInfo).IsSafetyZone = false;
            }
        }
    }

    #region :: Effect ::

    /// <summary>
    /// 레벨업 효과 처리 해주는 곳
    /// </summary>
    public void LevelUpEffect()
    {
        //G_GameInfo.GameInfo.BoardPanel.ShowLevelUp();
        
        //Transform effTrans = SpawnEffect( "Fx_LevelUp_01", 1f, transform, transform, false );
        //effTrans.AttachTo( G_GameInfo.GameInfo.BoardPanel.transform, true );
        //effTrans.Rotate( 0, 180, 0 );
        //effTrans.localScale = new Vector3( 300, 300, 300 )

        //G_GameInfo.GameInfo.BoardPanel.ShowLevelUp();
        if (this.gameObject.activeSelf)
            SpawnEffect("Fx_LevelUp_01_sub", 1f, transform, transform, false, (effTrans) =>
            {
            });
        else
        {
            //SingleGameInfo gameInfo = ( G_GameInfo.GameInfo as SingleGameInfo );
            //if (null == gameInfo)
            //    return;

            //gameInfo.HudPanel.LevelUpEff( this );
        }
    }

    #endregion
}
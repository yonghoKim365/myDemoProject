using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Npc : Unit
{
    // NPCSpawner에 의해 생성됐을시, 연결됨.
    public ISpawner         spawnner;

    public uint             NpcLowID { private set;  get; }
    public Mob.MobInfo      npcInfo { private set; get; }
    //public uint           NpcLevel { private set; get; }

    // RootMotion
    public KRootMotionRM rootMotion;
    public RaidBossAIBase BossPatten;

    public bool isMiddleBoss = false;

    GameObject ringShadow;

    #region :: MonoBehaviour ::

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Start()
    {
        if (UnitType == UnitType.Boss )//|| isMiddleBoss)
        {
            if (G_GameInfo.GameMode == GAME_MODE.SINGLE || G_GameInfo.GameMode == GAME_MODE.RAID 
                || G_GameInfo.GameMode == GAME_MODE.TOWER)
                EventListner.instance.TriggerEvent("HUD_SHOWBOSS", true, this);

            if(G_GameInfo.GameMode == GAME_MODE.RAID)
            {
                Mob.MobInfo monInfo = _LowDataMgr.instance.GetMonsterInfo(NpcLowID);
                if(0 < monInfo.Pattenid)
                    BossPatten = gameObject.AddComponent<RaidBossAIBase>();
            }
        }
    }

    #endregion

    //< NPC 스포너 테이블 대입용
    //public StageLowData.NpcPostingInfo _NpcPostingInfo;
    public uint SpawnUnitNum;
    public void SetNpcPostingInfo(uint unitNum)
    {
        //if (G_GameInfo.GameMode != GAME_MODE.SINGLE)
        if(SceneManager.instance.IsRTNetwork)
            return;

        SpawnUnitNum = unitNum;
        //Dictionary<uint, StageLowData.NpcPostingInfo> placementDic = LowDataMgr.GetSingleStagePlacementDatas((G_GameInfo._GameInfo as SingleGameInfo).StageID);

        //if (placementDic.ContainsKey(unitNum))
        //    _NpcPostingInfo = placementDic[unitNum];
        //else
        //    Debug.LogWarning("StageLowData.NpcPostingInfo 가 없습니다 : " + unitNum);
    }

    protected override void Init_SyncData(params object[] args)
    {
        base.Init_SyncData(args);

        switch (UnitType)
        {
            case UnitType.Npc:
            case UnitType.Boss:
                NpcLowID = (uint)args[3];

                npcInfo = _LowDataMgr.instance.GetMonsterInfo(NpcLowID);
                if(npcInfo == null)
                {
                    Debug.LogError(string.Format("not found MobTable error {0}", NpcLowID) );
                    npcInfo = _LowDataMgr.instance.GetMonsterInfo(10100);
                }

                AttackType = 1;// LowDataMgr.GetUnitAbilityData(NpcLowID).attackType;
                break;
        }

        GroupNo = (int)args[4];
    }

    protected override void Init_FSM()
    {
        GK_FSMFactory.SetupFSM( this, UnitType, out FSM );
    }

    protected override void Init_Datas()
    {
        base.Init_Datas();
    }

    protected override void Init_Controllers()
    {
        base.Init_Controllers();

        skill_AI = gameObject.AddComponent<Skill_AI>();
        skill_AI.Setup(this);
    }

    protected override void Init_Model()
    {
        //AssetbundleLoader.AssetLoad( AssetbundleLoader.CreateSuffixForPlatform( resInfo.modelFile ), OnLoadedModelGO );

        //추후 어셋번들에서 읽어오게 수정
        //MobTableLowData.MobInfo mosterInfo = _LowDataMgr.instance.GetMonsterInfo(npcInfo.Id);

        GameObject _unit = UnitModelHelper.NPCModelLoad(npcInfo.Id, QualityManager.instance.GetModelQuality());


        CharInfo.animDatas = _LowDataMgr.instance.AniInfoSetting(   npcInfo.prefab,
                                                                    _unit,
                                                                    npcInfo.AniId);//_LowDataMgr.instance.GetUnitAniInfo(mosterInfo.AniId);

        if(CharInfo.animDatas != null)
        {
            CharInfo.MaxAnimCombo = UnitDataFactory.CountingMaxAnimCombo(CharInfo.animDatas);
        }
        else
        {
            Debug.LogError(string.Format("NotFound AniData MonsterID:{0}", npcInfo.Id));
        }
        
        OnLoadedModelGO(_unit);
        NGUITools.SetLayer(gameObject, 14);

		SetShaderEnvironment();
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        switch (UnitType)
        {
            case global::UnitType.Npc:
            case global::UnitType.Boss:
                { 
                    //몬스터도 안밀림
                    navAgent.avoidancePriority = 20;
                }
                break;
        }

        if (SceneManager.instance.IsRTNetwork)
        {
            navAgent.avoidancePriority = 50;
        }

        rootMotion = gameObject.AddComponent<KRootMotionRM>();
    }

    protected override void ComputeComponents()
    {
        base.ComputeComponents();

        //UnitLowData.DataInfo _data = LowDataMgr.GetUnitData(comData.Info.UnitId);
        float ModelX = 1.43f;// _data.modelX;
        float ModelY = 1.9f;//  _data.modelY;
        float ModelZ = 1.43f;// _data.modelZ;

        navAgent.radius = (ModelX > ModelZ ? ModelX / 2 : ModelZ / 2) * Model.OriginalScale;
        navAgent.height = ModelY * Model.OriginalScale;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        UnitCollider = collider;
        collider.size = Vector3.Scale(new Vector3(ModelX * 0.9f, ModelY, ModelZ * 0.9f), Model.ScaleVec3);
        collider.center = new Vector3(UnitCollider.center.x, (collider.size.y * 0.5f), UnitCollider.center.z);

        Radius = navAgent.radius;
        Height = collider.size.y;

        //< 보스라면 안밀리도록 처리
        if (UnitType == global::UnitType.Boss || isMiddleBoss)
        {
            navAgent.avoidancePriority = 20;
            collider.isTrigger = true;
        }

        AddShadow( new Vector3( Mathf.Max(collider.size.x, Model.OriginalScale), Mathf.Max(collider.size.z, Model.OriginalScale), 1 ) );


	   // RootMotion을 위한 셋팅
	   if (rootMotion != null)
	   {
            AssetbundleLoader.GetRMCurves(npcInfo.prefab, (dic) =>
            {
                rootMotion.Init(dic, transform, Animator.Animation, Model.FindAndCaching("Bip001"));
            });
        }
    }

    protected override void SetupForGameMode(GAME_MODE gameMode)
    {
        if (UnitType == UnitType.Unit)
        {
            if (null != navAgent)
                navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;

            // Material에 직접 접근해서 색상변경시 Instance된다.
            if (TeamID != 0 && null != ringShadow)
                ringShadow.renderer.material.SetColor( "_AddColor", Color.red );
        }
    }

    public void BossEndShaderChange()
    {
        // 보스 사망연출시에 호출되어
        if (UnitType == global::UnitType.Boss && G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {

        }
    }

    /// 모델 생성 준비 완료시 호출할 콜백
    protected void OnLoadedModelGO(GameObject prefabObj)
    {
        if (Model.IsReady)
            Model.DeleteModel();
        
        GameObject modelGO = GameObject.Instantiate( prefabObj ) as GameObject;

        //< 보스일경우에는 셰이더를 변경해준다.(보스 연출을 위해..)
        if (UnitType == global::UnitType.Boss && G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            //쉐이더 통일로 삭제
            /*
            SkinnedMeshRenderer[] temMaterial = modelGO.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer material in temMaterial)
            {
                if (material.materials[0].shader.name == "Unlit/Transparent Cutout")    //"Custom/ClipSahderRimSpec2")
                    material.materials[0].shader = Shader.Find("Custom/RimSpec3_Queue");
            }
            */
        }

        //AssetbundleLoader.AddAnimationClips(modelGO.animation, CharInfo.animDatas, () => 
        //{
            AniEvent ani = modelGO.AddComponent<AniEvent>();
            ani.MyUnit = this;

            //if(G_GameInfo.GameMode == GAME_MODE.SPECIAL)
                //Model.Init(this.gameObject, modelGO, 1f);
            //else
            //{
                //m_jpsoldier 작은거 문제로 임시로 키워줌
                if (G_GameInfo.GameMode == GAME_MODE.SINGLE && npcInfo.prefab.Contains("m_jpsoldier_"))
                {
                    Model.Init(this.gameObject, modelGO, 1.5f * npcInfo.Scale);
                }
                else
                {
                    Model.Init(this.gameObject, modelGO, 1.3f * npcInfo.Scale);
                }
            //}

            if (null != modelGO.animation)
            {
                Animator.Init(this.gameObject, modelGO.animation, CharInfo.animDatas);
                Animator.Animation.playAutomatically = false;
            }

            //ExtraModels = GK_ModelCreator.SetupExtraModels(Model, resInfo.dummyId);

            if (UnitType == global::UnitType.Boss)
                modelGO.animation.cullingType = AnimationCullingType.AlwaysAnimate;

	   //< 보스일경우 인트로가있을경우에는 처리해줌
	   if (UnitType == UnitType.Boss && Animator.GetAnimName(eAnimName.Anim_intro) != "0" && Model.Main.animation.GetClip(Animator.GetAnimName(eAnimName.Anim_intro)) != null)
		  Model.Main.animation.Play(Animator.GetAnimName(eAnimName.Anim_intro));

	   //Model.Main.animation.Stop();

            //////////////////////////
            LoadingComplete();
        //});

        
        //< 모든 트랜스폼을 미리 저장해놓는다
        Transforms = modelGO.GetComponentsInChildren<Transform>(true);
    }

    public bool AllyCheck = false;
    public override int TakeDamage(Unit attacker, float damageRatio, float damage, float AddDamage, eAttackType atkType, bool isSkillDamage, AbilityData _ability, bool through = false, bool projecttile = false)
    {
        int resultDmg = base.TakeDamage(attacker, damageRatio, damage, AddDamage, atkType, isSkillDamage, _ability, through, projecttile);

        //< 인게임 상태가 아니라면 패스
        if (G_GameInfo.GameInfo.GameMode == GAME_MODE.NONE)
            return resultDmg;

        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            int count = SingleGameState.StageQuestList.Count;
            for (int i = 0; i < count; i++)
            {
                //if (SingleGameState.StageQuestList[i].Type != ClearQuestType.MAX_DAMAGE)
                //    continue;
                //else if (SingleGameState.StageQuestList[i].IsClear)//이미 클리어
                //    continue;
                
                if( SingleGameState.StageQuestList[i].CheckCondition(ClearQuestType.MAX_DAMAGE, resultDmg) )
                {
                    UIBasePanel panel = UIMgr.GetHUDBasePanel();
                    if(panel != null) {
                        (panel as InGameHUDPanel).SetQuestStringColor(i, true);
                    }

                    break;
                }
            }
        }
        /*
        if (G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
        {
            if ((G_GameInfo.GameInfo as TutorialGameInfo).DummyUnitNum == SpawnUnitNum)
            {
                if(IsDie)
                {
                    (G_GameInfo.GameInfo as TutorialGameInfo).EndDummyUnit();//더미 교육 끝
                }
                //if (attacker.CurrentState == UnitState.Attack
                //    || attacker.CurrentState == UnitState.ManualAttack)
                //    (G_GameInfo.GameInfo as TutorialGameInfo).AddCondition(InGameTutorialType.Attack);
                //else if (attacker.CurrentState == UnitState.Skill)
                //{
                //    CharInfo.Hp = CharInfo.MaxHp;
                //    if (attacker.UseSkillSlot == 1)
                //        (G_GameInfo.GameInfo as TutorialGameInfo).AddCondition(InGameTutorialType.Skill_01);
                //    else if (attacker.UseSkillSlot == 2)
                //        (G_GameInfo.GameInfo as TutorialGameInfo).AddCondition(InGameTutorialType.Skill_02);
                //    else if (attacker.UseSkillSlot == 3)
                //        (G_GameInfo.GameInfo as TutorialGameInfo).AddCondition(InGameTutorialType.Skill_03);
                //    else if (attacker.UseSkillSlot == 4)
                //    {
                //        //IsDie = true;
                //        CharInfo.Hp = 0;
                //        Die(attacker);
                //        (G_GameInfo.GameInfo as TutorialGameInfo).AddCondition(InGameTutorialType.Skill_04);
                //    }
                //}

                //base.TakeDamage(attacker, 0, 0, 0, atkType, isSkillDamage, 0, pushPower, through, projecttile);
                //return 0;
            }
        }
        */
        //< 이미 한번 호출을 해줬다면 패스한다.
        if (AllyCheck)
            return resultDmg;

        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT) //네트웍모드에서는 동맹에게 나를 공격한 공격자를 알려주지 않는다.
        if (SceneManager.instance.IsRTNetwork)
            return resultDmg;

        
        // 동맹들에게 나를 공격한 공격자를 알려주도록 한다.
        if (G_GameInfo.CharacterMgr.unitGroupDic.ContainsKey(GroupNo))
        {
            List<Unit> allyList = G_GameInfo.CharacterMgr.unitGroupDic[GroupNo];
            for (int i = 0; i < allyList.Count; i++)
            {
                Unit ally = allyList[i];
                if (null == ally || !ally.Usable || !ally.UsableNavAgent)
                    continue;

                if (ally.TargetID == GameDefine.unassignedID)
                {
                    Vector3 targetDist = attacker.cachedTransform.position - ally.cachedTransform.position;
                    float distance = Mathf.Clamp(targetDist.magnitude - (1f + ally.Radius + CharInfo.FirstAtkRange), 0, 100f);
                    Vector3 addPos = targetDist.normalized * distance;
                    ally.MovePosition(ally.cachedTransform.position + addPos);
                    ally.TargetID = attacker.GetInstanceID();
                }

                if ((ally is Npc))
                    (ally as Npc).AllyCheck = true;
            }
        }
         
        return resultDmg;
    }

    public override void Die(Unit attacker, float pushPower = 1f)
    {
        base.Die( attacker, pushPower );

        if (null == attacker)
            return;

        if (null != spawnner)
            spawnner.SendEvent( (int)SpawnGroup.eEvent.Dead );

        // Pc, Npc만 경험치를 획득하도록 되어있다.
        if (attacker.UnitType != UnitType.Unit)
            return;

    }

    public override void Revive()
    {
        if (null != Owner && null != Owner.Leader)
        { 
            Vector3 newPos = Owner.Leader.transform.position;
            transform.position = newPos;
        }

        base.Revive();
    }

    public override bool PlayAnim(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        bool doPlaying = base.PlayAnim( animEvent, CrossFade, CrossTime, canPlaySameAnim, queued );
        if (false == doPlaying)
            return doPlaying;
		
        //몬스터의 경우 일단 난투장에선 RootMotion을 안씀
		bool bRootMotion = CharInfo.animDatas[(int)animEvent].rootMotion;

        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
            bRootMotion = false;

        if (UsableNavAgent && rootMotion != null)
            rootMotion.Play( Animator.CurrentAnimState, bRootMotion);

        return true;
    }

    public override bool PlayAnimNoRootMation(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1f, bool canPlaySameAnim = false, bool queued = false)
    {
        return base.PlayAnim(animEvent, CrossFade, CrossTime, canPlaySameAnim, queued);
    }

    public override void StaticState(bool doStatic = true, float fadeDelay = 0.1f)
    {
        if (null != rootMotion && rootMotion.IsPlayingMotion)
            rootMotion.End();
       
        if(!doStatic )
        {
            //if (SceneManager.instance.optionData.ShowNpcHead)//Npc 이름, HpBar 표기 하는 것이 True면 실행
            //if(SceneManager.instance.optionData.ShowHpBar)
            //if()
            ShowHeadObj(true);
        }

        base.StaticState(doStatic, fadeDelay);
    }

    public override void AddShadow(Vector3 scale)
    {
        base.AddShadow( scale );

        if (UnitType != UnitType.Unit || null != ringShadow)
        {
            if (ringShadow)
                ringShadow.transform.localScale = new Vector3(scale.x, scale.x, scale.x); 
            return;
        }

        //< 해당 프리팹을 생성해서 메테리얼을 새로 생성후 색상 변경보다, 차라리 두개의 프리팹을 만들고 한번만 생성하는게
        //< 효율적일것 같아서 두개중에 불러오도록 처리
        //if(TeamID != 0 && G_GameInfo.GameMode == GAME_MODE.PVP)
        //    ringShadow = ResourceMgr.Instantiate<GameObject>("Character/RingShadow_Target");
        //else
        //    ringShadow = ResourceMgr.Instantiate<GameObject>("Character/Shadow");

        //ringShadow.transform.AttachTo( transform, new Vector3( 0f, 0.15f, 0f ), scale, Quaternion.Euler( new Vector3( 90, 0, 0 ) ) );
        //ringShadow.transform.localScale = new Vector3(scale.x, scale.x, scale.x); 
        //ringShadow.renderer.material.SetColor( "_AddColor" );
    }

    public override void DeleteShadow()
    {
        base.DeleteShadow();

        if (UnitType != UnitType.Unit || ringShadow == null)
            return;

        ringShadow.transform.parent = null;
        Destroy( ringShadow );
        ringShadow = null;
    }

    public override void ShowHeadObj(bool unShow)
    {
        base.ShowHeadObj(unShow);


        // NPC에 HPSlider 표시
        if (unShow && gameObject.activeSelf &&  G_GameInfo.GameInfo.BoardPanel != null)//if(UnitType == global::UnitType.Npc && !doStatic && G_GameInfo.GameInfo.BoardPanel != null)
        {
            Mob.MobInfo mLowData = _LowDataMgr.instance.GetMonsterInfo(NpcLowID);
            string name = null;
            if (mLowData != null)
                name = _LowDataMgr.instance.GetStringUnit(mLowData.NameId);
            else
                name = string.Format("NotFound ID={0}", NpcLowID);
            
            G_GameInfo.GameInfo.BoardPanel.ShowHead(gameObject, name, 0, 0, false);
        }
    }
}

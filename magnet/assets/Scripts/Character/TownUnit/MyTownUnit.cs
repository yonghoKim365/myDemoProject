using UnityEngine;
using System.Collections;

public class MyTownUnit : Unit {

    public KRootMotionRM rootMotion;
    Resource.AniInfo[] ads = null;
    public TownUnitMoveHelper townUnitHelper;

    public GameObject _myModel;
    /// <summary>
    /// 스킨이 변경되었을때 true
    /// </summary>
    private bool IsChangeSkin;


    public uint moveTargetStage;
    public uint moveTargetNPC;
    public bool moveTargetPotal;
    public bool IsMiniMapMove;

    public static MyTownUnit CreateTownControllUnit( int PosX, int PosY )
    {
        GameObject _townUnit = new GameObject();
        _townUnit.name = "MyTownUnit";

        MyTownUnit mtu = _townUnit.AddComponent<MyTownUnit>();

        mtu.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);

        Vector3 pos = NaviTileInfo.instance.GetTilePos(PosX, PosY);

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(pos, out navHit, 10, -1))  //<--- 네비게이션 메쉬 높이 때문에 이것도 계산해 주었어야 한다.(몬스터의 값은 비슷하게 온다.)
        {
            pos = navHit.position;
        }

        mtu.transform.position = pos;

        mtu.Height = 2.47f;
        mtu.Init();

        mtu.BeforePosX = PosX;
        mtu.BeforePosY = PosY;

        return mtu;
    }

	public static IEnumerator CreateTownControllUnitAsync( int PosX, int PosY, System.Action<MyTownUnit> callback )
	{
		GameObject _townUnit = new GameObject();
		_townUnit.name = "MyTownUnit";
		
		MyTownUnit mtu = _townUnit.AddComponent<MyTownUnit>();
		
		mtu.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
		
		Vector3 pos = NaviTileInfo.instance.GetTilePos(PosX, PosY);
		
		NavMeshHit navHit;
		if (NavMesh.SamplePosition(pos, out navHit, 10, -1))  //<--- 네비게이션 메쉬 높이 때문에 이것도 계산해 주었어야 한다.(몬스터의 값은 비슷하게 온다.)
		{
			pos = navHit.position;
		}
		
		mtu.transform.position = pos;
		
		mtu.Height = 2.47f;
		yield return SceneManager.instance.StartCoroutine (mtu.InitAsync ());
		
		mtu.BeforePosX = PosX;
		mtu.BeforePosY = PosY;

		callback (mtu);
	}
	    
    public void ControllerSetting(Joystick joy)
    {

        CameraManager.instance.Follow(transform);
//	   Debug.LogWarning("2JW : mytownunit.Controllersetting() In - " + CameraManager.instance.mainCamera + " : " + CameraManager.instance.mainCamera.transform.localEulerAngles + " : " + CameraManager.instance.mainCamera.transform.eulerAngles, CameraManager.instance.mainCamera);

        inputCtlr = gameObject.GetComponent<GKUnitController>();
        if (null == inputCtlr)
            inputCtlr = gameObject.AddComponent<GKUnitController>();

        inputCtlr.StartJoystick(joy, CameraManager.instance.mainCamera);
        
    }

    protected override void Init_SyncData(params object[] args)
    {
	    UnitType = UnitType.TownUnit;
    }

    protected override void Init_Datas()
    {
	    base.Init_Datas();

        moveTargetStage = uint.MaxValue;
        moveTargetNPC = uint.MaxValue;
        moveTargetPotal = false;
        IsMiniMapMove = false;
    }

    //타운이동관련 함수들
    public void RunToNPC(uint id = 0, bool isMiniMap=false)
    {
        moveTargetNPC = id;
        IsMiniMapMove = isMiniMap;
        TownNpcMgr.instance.RunTownUnit(TownRunTargetType.Npc, id);
    }

    public void RunToCurrentQuestNpc()
    {
        Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
        if (quest != null)
            RunToNPC(quest.ParamId);
        
    }

    public void RunToPotal(uint stageId=uint.MaxValue)
    {
        //SceneManager.instance.GetState<TownState>().MyHero.
        ResetMoveTarget();
        moveTargetStage = stageId;
        moveTargetPotal = true;
        IsMiniMapMove = false;
        //TownNpcMgr.instance.RunTownUnit(TownRunTargetType.Potal);

        int npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.SINGLE_NPC);

        if (npcID != int.MaxValue)
        {
            //SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
            //SceneManager.instance.GetState<TownState>().MyHero.
            RunToNPC((uint)npcID);
        }
    }
	
    public void ResetMoveTarget()
    {
        moveTargetStage = uint.MaxValue;
        moveTargetNPC = uint.MaxValue;
        moveTargetPotal = false;
        IsMiniMapMove = false;

        townUnitHelper.StopMove();

        ChangeState(UnitState.Idle);
    }

    public bool CheckPotal()
    {
        return moveTargetPotal;
    }

    public uint CheckNpc()
    {
        return moveTargetNPC;
    }

    protected override void Init_Model()
    {
        UnitModelLoad();

		Init_ModelPart2 ();
    }

	protected override IEnumerator Init_ModelAsync()
	{
		yield return  StartCoroutine (UnitModelLoadAsync ());
		
		Init_ModelPart2 ();
	}

	void Init_ModelPart2()
	{

		BoxCollider _collider = gameObject.GetComponent<BoxCollider>();
		if (_collider == null)
		{
			_collider = gameObject.AddComponent<BoxCollider>();
			_collider.size = Vector3.one * 1.8f;
		}
		_collider.enabled = true;
		AddShadow(collider ? new Vector3(_collider.size.x, _collider.size.z, 1) : Model.ScaleVec3);
	}

    public void Set_XRayShader()
    {
        GameObject a_ChildCamObj = null;
        SkinnedMeshRenderer[] SMRs = _myModel.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (SMRs != null)
        {
            Material a_material = Resources.Load("Shaders/XRayMaterial", typeof(Material)) as Material;
            for (int j = 0; j < SMRs.Length; j++)
            {
                //< 셰이더를 직접 변경해준다.
                //Debug.LogWarning(SMRs[j].gameObject.name);

                a_ChildCamObj = GameObject.Instantiate(SMRs[j].gameObject) as GameObject;
                a_ChildCamObj.name = "XRay_" + SMRs[j].gameObject.name;
                a_ChildCamObj.layer = LayerMask.NameToLayer("Unit");   //<-- 적용할 
                a_ChildCamObj.transform.parent = _myModel.transform;

                a_ChildCamObj.transform.localPosition = SMRs[j].transform.localPosition;
                a_ChildCamObj.transform.localRotation = SMRs[j].transform.localRotation;
                a_ChildCamObj.transform.localScale = SMRs[j].transform.localScale;

                a_ChildCamObj.renderer.material = a_material;

                //2, 기존에 피격 Shader 제외되도록...

            }
        }
    }

	void getModelInfo(NetData._UserInfo inven, ref uint HELMETID, ref uint CLOTHID, ref uint WEAPONID, ref uint CostumeID){

		Item.EquipmentInfo tempInfo = inven.GetEquipPartsLowData(ePartType.HELMET);
		if (tempInfo == null)
			HELMETID = 0;
		else
			HELMETID = tempInfo.Id;
		
		tempInfo = inven.GetEquipPartsLowData(ePartType.CLOTH);
		if (tempInfo == null)
			CLOTHID = 0;
		else
			CLOTHID = tempInfo.Id;
		
		tempInfo = inven.GetEquipPartsLowData(ePartType.WEAPON);
		if (tempInfo == null)
			WEAPONID = 0;
		else
			WEAPONID = tempInfo.Id;
        
		NetData._CostumeData tempCostume = inven.GetEquipCostume();
		
		if (tempCostume == null)
		{
			if(inven.GetCharIdx() == 11000)
			{
				CostumeID = 100;
			}
			else if (inven.GetCharIdx() == 12000)
			{
				CostumeID = 110;
			}
			else if (inven.GetCharIdx() == 13000)
			{
				CostumeID = 120;
			}
		}
		else
		{
			CostumeID = tempCostume._costmeDataIndex;
		}
    }

    protected void UnitModelLoad()
    {
        NetData._UserInfo inven = NetData.instance.GetUserInfo();

        uint HELMETID = 0;
        uint CLOTHID = 0;
        uint WEAPONID = 0;
        uint CostumeID = 0;

		getModelInfo (inven, ref HELMETID, ref CLOTHID, ref WEAPONID, ref CostumeID);

        //예외처리는 어케하냐 
        GameObject _myUnit = UnitModelHelper.PCModelLoad( inven.GetCharIdx(), HELMETID, CostumeID, CLOTHID, WEAPONID, inven.isHideCostum, 
		                                                 ref WeaponEffects, QualityManager.instance.GetModelQuality());

		UnitModelLoadPart2 (_myUnit, CostumeID);

    }

	protected IEnumerator UnitModelLoadAsync()
	{
		NetData._UserInfo inven = NetData.instance.GetUserInfo();
		
		uint HELMETID = 0;
		uint CLOTHID = 0;
		uint WEAPONID = 0;
		uint CostumeID = 0;
		
		getModelInfo (inven, ref HELMETID, ref CLOTHID, ref WEAPONID, ref CostumeID);

		GameObject _myUnit = null;
		yield return StartCoroutine(UnitModelHelper.PCModelLoadAsync(inven.GetCharIdx(),HELMETID,CostumeID,CLOTHID,WEAPONID,inven.isHideCostum, 
		                                                        (retVal1,retVal2) =>{
																	_myUnit = retVal1;
																	WeaponEffects = retVal2;
																},
																QualityManager.instance.GetModelQuality()));

        uint skillSetId = inven.GetEquipSKillSet().SkillSetId;
        if (skillSetId <= 0)
        {
            switch (inven._userCharIndex)
            {
                case 11000:
                    skillSetId = 100;
                    break;
                case 12000:
                    skillSetId = 200;
                    break;
                case 13000:
                    skillSetId = 300;
                    break;
            }
        }
        UnitModelLoadPart2 (_myUnit, skillSetId);
	}

	protected void UnitModelLoadPart2(GameObject _myUnit, uint skillSetId)
	{

		SetTrailRenderer (_myUnit, false);

        SkillTables.SkillSetInfo skillSetInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
		//Item.CostumeInfo costume = _LowDataMgr.instance.GetLowDataCostumeInfo(CostumeID);
		//무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
		ads = _LowDataMgr.instance.TownAniInfoSetting(skillSetInfo.AniPath,
		                                              _myUnit,
                                                      skillSetInfo.AniId,
		                                              false);
		
		_myUnit.transform.parent = transform;
		_myModel = _myUnit;        
		
		Animator.Init(_myUnit, _myUnit.animation, ads);
		Animator.Animation.playAutomatically = false;
		
		Model.Init(gameObject, _myUnit);

		AniEvent aniEvent = _myUnit.AddComponent<AniEvent>();
		aniEvent.MyUnit = this;
		
		Animator.PlayAnim(eAnimName.Anim_idle);
		NGUITools.SetLayer(gameObject, 14);

		//Set_XRayShader();
		SetShaderEnvironment();
	}

    protected override void Init_Controllers()
    {
    }

    protected override void Init_FSM()
    {
        GK_FSMFactory.SetupFSM(this, UnitType.TownUnit, out FSM);
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        navAgent.avoidancePriority = 40;
        navAgent.baseOffset = 0f;
        rootMotion = gameObject.AddComponent<KRootMotionRM>();
		townUnitHelper = gameObject.AddComponent<TownUnitMoveHelper>();
        AddLight();
    }

    public override bool PlayAnim(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1F, bool canPlaySameAnim = false, bool queued = false)
    {
        bool doPlaying = base.PlayAnim(animEvent, CrossFade, CrossTime, canPlaySameAnim, queued);
        if (false == doPlaying)
            return doPlaying;

        Resource.AniInfo aniInfo = ads[(int)animEvent];
        if (UsableNavAgent && rootMotion != null)
            rootMotion.Play(Animator.CurrentAnimState, aniInfo.rootMotion);

        return true;
    }

    public override void StaticState(bool doStatic = true, float fadeDelay = 0.1f)
    {
        if (null != rootMotion && rootMotion.IsPlayingMotion)
            rootMotion.End();

        base.StaticState(doStatic, fadeDelay);
    }

    /// <summary>
    /// 장착중인 아이템에 맞게 스킨 설정 해줌 추후에 삭제해야함.
    /// </summary>
    /// 
    public bool ChangeSkin()
    {

        if (!IsChangeSkin)
            return false;
        //내 모델을 날리고 새로 읽어오자


        Destroy(_myModel);

        UnitModelLoad();

        IsChangeSkin = false;

        return true;
    }
    
    /// <summary>
    /// 스킨이 변경되었을때 호출한다.
    /// </summary>
    /// <param name="isChange"></param>
    public void SetChangeSkin(bool isChange)
    {
        IsChangeSkin = isChange;
    }
    
    public NavMeshPath GetMovePath()
    {
        return SceneManager.instance.GetState<TownState>().MyHero.movePath;
    }
}

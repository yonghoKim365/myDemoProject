using UnityEngine;
using System.Collections;

public class TownUnit : Unit, IInputObject
{
    public KRootMotionRM rootMotion;
    Resource.AniInfo[] ads = null;
    public NetData.RecommandFriendData townunitData;

    public GameObject _myModel;

    public static TownUnit CreateTownUnit(int x, int y, params object[] args)
    {
        GameObject _townUnit = new GameObject();
        _townUnit.name = "TownUnit";

        TownUnit tu = _townUnit.AddComponent<TownUnit>();
        Vector3 pos = NaviTileInfo.instance.GetTilePos(x, y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 10, -1))
        {
            pos = hit.position;
        }

        tu.transform.position = pos;

        tu.Height = 2.47f;
        tu.Init(args);

		tu.SetTrailRenderer (tu.gameObject, false);

        // 마을에서 스케일 1.2정도 크게해줘
        tu.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);

        NGUITools.SetLayer(_townUnit, 14);

        return tu;
    }

	public static IEnumerator CreateTownUnitAsync(int x, int y, System.Action<TownUnit> callback,  params object[] args)
	{
		GameObject _townUnit = new GameObject();
		_townUnit.name = "TownUnit";
		
		TownUnit tu = _townUnit.AddComponent<TownUnit>();
		Vector3 pos = NaviTileInfo.instance.GetTilePos(x, y);
		
		NavMeshHit hit;
		if (NavMesh.SamplePosition(pos, out hit, 10, -1))
		{
			pos = hit.position;
		}
		
		tu.transform.position = pos;
		
		tu.Height = 2.47f;
		yield return SceneManager.instance.StartCoroutine (tu.InitAsync (args));

		tu.SetTrailRenderer (tu.gameObject, false);

		
		// 마을에서 스케일 1.2정도 크게해줘
		tu.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
		
		NGUITools.SetLayer(_townUnit, 14);

		callback (tu);
	}



    protected override void Init_SyncData(params object[] args)
    {
        UnitType = (UnitType)args[2];
        townunitData = (NetData.RecommandFriendData)args[3];
    }

    public void InputEvent(POINTER_INFO ptr)
    {
        /*
        //조이스틱 끄고 키기
        if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.PRESS)
        {
            //Debug.Log("OnPress");
            if (EasyTouch.instance.enable)
                EasyTouch.SetEnabled(false);
        }
        else if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.RELEASE)
        {
            //Debug.Log("OnPress");
            if (!EasyTouch.instance.enable)
                EasyTouch.SetEnabled(true);
        }
        else if (EasyTouch.instance != null && ptr.evt == POINTER_INFO.INPUT_EVENT.DRAG)
        {
            //Debug.Log("OnDrag");
            if (!EasyTouch.instance.enable)
                EasyTouch.SetEnabled(true);
        }
        */

        if (ptr.evt == POINTER_INFO.INPUT_EVENT.CLICK)
        {
            UIMgr.OpenUserInfoPopup((long)townunitData.c_usn, townunitData.nickname, (int)townunitData.character_id, townunitData.vipLevel, (int)townunitData.level);
        }
    }

    protected override void Init_Datas()
    {
        base.Init_Datas();
    }

    protected override void Init_Model()
    {
        UnitModelLoad();

        BoxCollider _collider = gameObject.GetComponent<BoxCollider>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.size = Vector3.one * 1.8f;
        }
        _collider.enabled = true;
        AddShadow(collider ? new Vector3(_collider.size.x, _collider.size.z, 1) : Model.ScaleVec3);
    }

	protected override IEnumerator Init_ModelAsync()
	{
		yield return StartCoroutine (UnitModelLoadAsync ());
		
		BoxCollider _collider = gameObject.GetComponent<BoxCollider>();
		if (_collider == null)
		{
			_collider = gameObject.AddComponent<BoxCollider>();
			_collider.size = Vector3.one * 1.8f;
		}
		_collider.enabled = true;
		AddShadow(collider ? new Vector3(_collider.size.x, _collider.size.z, 1) : Model.ScaleVec3);
	}

    protected void UnitModelLoad()
    {
        //추후 다시한번 수정 요망
        GameObject _myUnit = UnitModelHelper.PCModelLoad( townunitData.character_id,
                                                          townunitData.EquipHeadItemIdx,
                                                          townunitData.costume_id,
                                                          townunitData.EquipClothItemIdx,
                                                          townunitData.EquipWeaponItemIdx,
                                                          townunitData.isHideCostume, ref WeaponEffects, QualityManager.instance.GetModelQuality());

        //Item.CostumeInfo cosinfo = _LowDataMgr.instance.GetLowDataCostumeInfo(townunitData.costume_id);
        uint skillSetId = townunitData.SkillSetID;//애니메이션을 코스튬이 아닌 스킬셋트에서 가져온다.
        if (skillSetId <= 0) {
            switch (townunitData.character_id)
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

        SkillTables.SkillSetInfo skillSetInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        //무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
        ads = _LowDataMgr.instance.TownAniInfoSetting(skillSetInfo.AniPath,
                                                      _myUnit,
                                                      skillSetInfo.AniId,
                                                      false);
        //inven.GetEquipCostume().GetLowData().AniId);

        _myUnit.transform.parent = transform;
        _myModel = _myUnit;

        Animator.Init(_myUnit, _myUnit.animation, ads);
        Animator.Animation.playAutomatically = false;

        Model.Init(gameObject, _myUnit);

        AniEvent aniEvent = _myUnit.AddComponent<AniEvent>();
        aniEvent.MyUnit = this;

        Animator.PlayAnim(eAnimName.Anim_idle);

		SetShaderEnvironment();
    }

	protected IEnumerator UnitModelLoadAsync()
	{
		//추후 다시한번 수정 요망
		GameObject _myUnit = null;
		yield return StartCoroutine(UnitModelHelper.PCModelLoadAsync( townunitData.character_id,
		                                                        townunitData.EquipHeadItemIdx,
		                                                        townunitData.costume_id,
		                                                        townunitData.EquipClothItemIdx,
		                                                        townunitData.EquipWeaponItemIdx,
		                                                        townunitData.isHideCostume,
		                                                        (retval, retval2)=>{ 
																	_myUnit = retval;
																	WeaponEffects = retval2;
																}, QualityManager.instance.GetModelQuality()));
		 
		//Item.CostumeInfo cosinfo = _LowDataMgr.instance.GetLowDataCostumeInfo(townunitData.costume_id);
		
        if(_myUnit == null)
        {
            Debug.LogError(string.Format("charId={0}, headIdx={1}, costumeId={2}, clothIdx={3}, weaponIdx={4}, isHide={5}, roleId={6}, nickName={7}",
                townunitData.character_id,
                townunitData.EquipHeadItemIdx,
                townunitData.costume_id,
                townunitData.EquipClothItemIdx,
                townunitData.EquipWeaponItemIdx,
                townunitData.isHideCostume,
                townunitData.c_usn,
                townunitData.nickname
                ));
            yield return null;
        }

        uint skillSetId = townunitData.SkillSetID;//애니메이션을 코스튬이 아닌 스킬셋트에서 가져온다.
        if (skillSetId <= 0)
        {
            switch (townunitData.character_id)
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

        SkillTables.SkillSetInfo skillSetInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillSetId);
        //무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
        ads = _LowDataMgr.instance.TownAniInfoSetting(skillSetInfo.AniPath,
                                                      _myUnit,
                                                      skillSetInfo.AniId,
                                                      false);
        
		//inven.GetEquipCostume().GetLowData().AniId);
		
		_myUnit.transform.parent = transform;
		_myModel = _myUnit;
		
		Animator.Init(_myUnit, _myUnit.animation, ads);
		Animator.Animation.playAutomatically = false;
		
		Model.Init(gameObject, _myUnit);
		
		AniEvent aniEvent = _myUnit.AddComponent<AniEvent>();
		aniEvent.MyUnit = this;
		
		Animator.PlayAnim(eAnimName.Anim_idle);
		
		SetShaderEnvironment();
	}

    protected override void Init_Controllers()
    {
    }

    protected override void Init_FSM()
    {
        GK_FSMFactory.SetupFSM(this, UnitType.TownNpc, out FSM);
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navAgent.avoidancePriority = 50;
        rootMotion = gameObject.AddComponent<KRootMotionRM>();
        //AddLight();
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

    public void ChangeSkin(bool isHide, uint costume, uint head, uint cloth, uint weapon)
    {
        townunitData.costume_id = costume;
        townunitData.EquipHeadItemIdx = head;
        townunitData.EquipClothItemIdx = cloth;
        townunitData.EquipWeaponItemIdx = weapon;
        townunitData.isHideCostume = isHide;
        //내 모델을 날리고 새로 읽어오자
        Destroy(_myModel);

        UnitModelLoad();

        NGUITools.SetLayer(_myModel, 14);
    }

    public void ChangeTitle(uint left, uint right)
    {
        townunitData.Prefix = left;
        townunitData.Sufffix = right;

        UIBasePanel basePanel = UIMgr.GetTownBasePanel();
        if (basePanel != null)
            (basePanel as TownPanel).ChangeUnitTitle(townunitData.c_usn, left, right);
    }

    public ulong GetUID()
    {
        return townunitData.c_usn;
    }
}

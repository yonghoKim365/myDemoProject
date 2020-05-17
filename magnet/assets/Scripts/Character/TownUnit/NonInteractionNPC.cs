using UnityEngine;
using System.Collections;


public struct NonInteractionNPCInfo
{
    public uint npcType;
    public string NPCPref;
    public uint AniID;
    public float MoveSpeed;
}

public class NonInteractionNPC : Unit
{
    //public KRootMotionRM rootMotion;
    Resource.AniInfo[] _ads = null;
    public NonInteractionNPCInfo _npcData;
    public GameObject _myModel;

    public static NonInteractionNPC CreateTownUnit(int x, int y, params object[] args)
    {
        GameObject _townNPCUnit = new GameObject();
        _townNPCUnit.name = "NonInteractionNPC";

        NonInteractionNPC tu = _townNPCUnit.AddComponent<NonInteractionNPC>();
        Vector3 pos = NaviTileInfo.instance.GetTilePos(x, y);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 10, -1))
        {
            pos = hit.position;
        }

        tu.transform.position = pos;

        tu.Height = 2.47f;
        tu.Init(args);

        // 마을에서 스케일 1.2정도 크게해줘
        tu.gameObject.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);

        NGUITools.SetLayer(_townNPCUnit, 14);

        return tu;
    }

    protected override void Init_SyncData(params object[] args)
    {
        UnitType = (UnitType)args[2];
        _npcData = (NonInteractionNPCInfo)args[3];
        //townunitData = (NetData.RecommandFriendData)args[3];
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

    protected void UnitModelLoad()
    {
        //추후 다시한번 수정 요망
        GameObject _myUnit = UnitModelHelper.ModelLoadtoString(_npcData.NPCPref);



        //GameObject _myUnit = UnitModelHelper.PCModelLoad(townunitData.character_id,
        //                                                  townunitData.EquipHeadItemIdx,
        //                                                  townunitData.costume_id,
        //                                                  townunitData.EquipClothItemIdx,
        //                                                  townunitData.EquipWeaponItemIdx,
        //                                                  townunitData.isHideCostume, ref WeaponEffects);

        //Item.CostumeInfo cosinfo = _LowDataMgr.instance.GetLowDataCostumeInfo(townunitData.costume_id);

        ////무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
        _ads = _LowDataMgr.instance.TownAniInfoSetting(_npcData.NPCPref,
                                                      _myUnit,
                                                      _npcData.AniID,
                                                      false);
        ////inven.GetEquipCostume().GetLowData().AniId);

        _myUnit.transform.parent = transform;
        _myModel = _myUnit;

        Animator.Init(_myUnit, _myUnit.animation, _ads);
        Animator.Animation.playAutomatically = false;

        Model.Init(gameObject, _myUnit);

        AniEvent aniEvent = _myUnit.AddComponent<AniEvent>();
        aniEvent.MyUnit = this;

        Animator.PlayAnim(eAnimName.Anim_idle);
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
        //rootMotion = gameObject.AddComponent<KRootMotionRM>();
        AddLight();
    }

    public override bool PlayAnim(eAnimName animEvent, bool CrossFade = false, float CrossTime = 0.1F, bool canPlaySameAnim = false, bool queued = false)
    {
        bool doPlaying = base.PlayAnim(animEvent, CrossFade, CrossTime, canPlaySameAnim, queued);
        if (false == doPlaying)
            return doPlaying;

        //Resource.AniInfo aniInfo = ads[(int)animEvent];
        //if (UsableNavAgent && rootMotion != null)
        //    rootMotion.Play(Animator.CurrentAnimState, aniInfo.rootMotion);

        return true;
    }

    public override void StaticState(bool doStatic = true, float fadeDelay = 0.1f)
    {
        //if (null != rootMotion && rootMotion.IsPlayingMotion)
        //    rootMotion.End();

        base.StaticState(doStatic, fadeDelay);
    }
}

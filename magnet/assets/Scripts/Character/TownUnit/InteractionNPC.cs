using UnityEngine;
using System.Collections;


public struct InteractionNPCInfo
{
    public uint npcType;
    public string NPCPref;
    public uint AniID;
    public float MoveSpeed;
	public float scale;
	public uint moveRange;
}

public class InteractionNPC : Unit
{
    //public KRootMotionRM rootMotion;
    Resource.AniInfo[] ads = null;
    public InteractionNPCInfo npcData;

    public GameObject _myModel;

    public static InteractionNPC CreateTownUnit(Vector3 pos, params object[] args)
    {
        GameObject _townNPCUnit = new GameObject();
        _townNPCUnit.name = "InteractionNPC";

        InteractionNPC tu = _townNPCUnit.AddComponent<InteractionNPC>();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 10, -1))
        {
            pos = hit.position;
        }

        tu.transform.position = pos;

        tu.Height = 2.47f;
        tu.Init(args); // load

		InteractionNPCInfo n = (InteractionNPCInfo)args [3];

		tu.gameObject.transform.localScale = new Vector3 (n.scale, n.scale, n.scale);

        NGUITools.SetLayer(_townNPCUnit, 14);

        return tu;
    }

	public static IEnumerator CreateTownUnitAsync(Vector3 pos, System.Action<InteractionNPC> callback, params object[] args)
	{
		GameObject _townNPCUnit = new GameObject();
		_townNPCUnit.name = "InteractionNPC";
		
		InteractionNPC tu = _townNPCUnit.AddComponent<InteractionNPC>();
		
		NavMeshHit hit;
		if (NavMesh.SamplePosition(pos, out hit, 10, -1))
		{
			pos = hit.position;
		}
		
		tu.transform.position = pos;
		
		tu.Height = 2.47f;

		yield return SceneManager.instance.StartCoroutine (tu.InitAsync (args));
		
		InteractionNPCInfo n = (InteractionNPCInfo)args [3];
		
		tu.gameObject.transform.localScale = new Vector3 (n.scale, n.scale, n.scale);
		
		NGUITools.SetLayer(_townNPCUnit, 14);

		callback (tu);
	}

    protected override void Init_SyncData(params object[] args)
    {
        UnitType = (UnitType)args[2];
        npcData = (InteractionNPCInfo)args[3];
        //townunitData = (NetData.RecommandFriendData)args[3];
    }

    protected override void Init_Datas()
    {
        base.Init_Datas();
    }

    //protected override void Init_Model()
	protected override IEnumerator Init_ModelAsync()
    {
		yield return  StartCoroutine (UnitModelLoadAsync ());

        BoxCollider _collider = gameObject.GetComponent<BoxCollider>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider>();
            _collider.size = Vector3.one * 1.8f;
        }
        _collider.enabled = true;
        AddShadow(collider ? new Vector3(_collider.size.x, _collider.size.z, 1) : Model.ScaleVec3);

		yield return null;
    }

    protected void UnitModelLoad()
    {
        //추후 다시한번 수정 요망
        GameObject _myUnit = UnitModelHelper.ModelLoadtoString(npcData.NPCPref);

        ////무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
        ads = _LowDataMgr.instance.TownAniInfoSetting(npcData.NPCPref,
                                                      _myUnit,
                                                      npcData.AniID,
                                                      false );
        ////inven.GetEquipCostume().GetLowData().AniId);

        _myUnit.transform.parent = transform;
        _myModel = _myUnit;

        Animator.Init(_myUnit, _myUnit.animation, ads);
        Animator.Animation.playAutomatically = false;

        Model.Init(gameObject, _myUnit);

        AniEvent aniEvent = _myUnit.AddComponent<AniEvent>();
        aniEvent.MyUnit = this;

        Animator.PlayAnim(eAnimName.Anim_idle);
    }

	protected IEnumerator UnitModelLoadAsync()
	{
		//추후 다시한번 수정 요망
		GameObject _myUnit = null;

		yield return StartCoroutine( UnitModelHelper.ModelLoadAsync (npcData.NPCPref, (retVal)=> { _myUnit = retVal;}));
		
		////무조건 플레이어의 애니메이션 ID는 코스튬에서 읽어와야한다
		ads = _LowDataMgr.instance.TownAniInfoSetting(npcData.NPCPref,
		                                              _myUnit,
		                                              npcData.AniID,
		                                              false );
		////inven.GetEquipCostume().GetLowData().AniId);
		
		_myUnit.transform.parent = transform;
		_myModel = _myUnit;
		
		Animator.Init(_myUnit, _myUnit.animation, ads);
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
        GK_FSMFactory.SetupFSM(this, UnitType.TownNINPC, out FSM);
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        navAgent.avoidancePriority = 50;
        //rootMotion = gameObject.AddComponent<KRootMotionRM>();
        //AddLight();
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

    protected float wanderDistance = 3;
    protected float intervalNewDest = 3f;
    Vector3 destination;
    public virtual void NewDestination()
    {
        //time = 0;
		intervalNewDest = Random.Range (1f, npcData.moveRange);

		destination = transform.position + Random.onUnitSphere * wanderDistance * intervalNewDest;
			
		NavMeshHit hit;
        if (NavMesh.SamplePosition(destination, out hit, 10, -1))
        {
            destination = hit.position;
        }


        if (CalculatePath(destination))
        {
            Debug.DrawLine(transform.position, destination, Color.red, intervalNewDest);
            PlayAnim(eAnimName.Anim_move, true, 0.1f);
            ChangeState(UnitState.Wander);
        }
    }
}

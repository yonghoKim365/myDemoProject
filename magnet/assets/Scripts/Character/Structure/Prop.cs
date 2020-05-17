using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prop : Unit
{
    // NPCSpawner에 의해 생성됐을시, 연결됨.
    public ISpawner         spawnner;

    public uint             PropLowID { private set; get; }
    public Mob.PropInfo     PropInfo { private set; get; }

    protected override void Init_SyncData(params object[] args)
    {
        base.Init_SyncData(args);

        switch (UnitType)
        {
            case UnitType.Prop:
                PropLowID = (uint)args[3];
                PropInfo = _LowDataMgr.instance.GetPropInfo(PropLowID);
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

    protected override void Init_Model()
    {
        GameObject modelGO = ResourceMgr.Load(string.Format("Character/Prefab/{0}", PropInfo.Prefab)) as GameObject;
        CharInfo.animDatas = _LowDataMgr.instance.PropAniInfoSetting( PropInfo.Prefab, modelGO, PropInfo.ResourceUnitId);
        OnLoadedModelGO( modelGO );
        NGUITools.SetLayer(gameObject, 14);
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();

        if (UnitType == global::UnitType.Prop)
            navAgent.avoidancePriority = 20;

        SetObstacleAvoidance(false);
    }

    /// <summary>
    /// 생성된 모델 받기
    /// </summary>
    /// <param name="createdObj">생성되어온 객체</param>
    protected void OnLoadedModelGO(GameObject prefabObj)
    {
        if (Model.IsReady)
            Model.DeleteModel();
        
        GameObject modelGO = GameObject.Instantiate( prefabObj ) as GameObject;
        //AssetbundleLoader.AddAnimationClips(modelGO.animation, CharInfo.animDatas, () => 
        //{
            AniEvent ani = modelGO.AddComponent<AniEvent>();
            ani.MyUnit = this;

		    Model.Init(this.gameObject, modelGO, 1.3f);
            if (null != modelGO.animation)
            {
                Animator.Init(this.gameObject, modelGO.animation, CharInfo.animDatas);
                Animator.Animation.playAutomatically = false;  //기본 애니메이션이(Animation.clip) 프로그램이 시작될때 자동으로 재생되어야 하는지의 대한 여부.

                //Animator.PlayAnim(eAnimName.Anim_battle_idle);
            }

            //ExtraModels = GK_ModelCreator.SetupExtraModels(Model, resInfo.dummyId);

        //////////////////////////
        LoadingComplete();
        //});
    }

    protected override void ComputeComponents()
    {
        base.ComputeComponents();

        float lowNavRadius = 1f;
        float lowNavHeight = 1f;
        
        navAgent.radius = lowNavRadius * Model.OriginalScale;
        navAgent.height = lowNavHeight * Model.OriginalScale;
        //navAgent.baseOffset = CharInfo.AirHeight;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.size = Vector3.Scale(new Vector3(lowNavRadius, lowNavHeight, lowNavRadius), Model.Transform.localScale);
        collider.center = new Vector3( collider.center.x, ( Model.Transform.localScale.y * 0.5f ) * lowNavHeight, collider.center.z );

        Radius = navAgent.radius;
        Height = collider.size.y;
    }

    protected override bool LoadingComplete()
    {
        bool state = base.LoadingComplete();
        
        if (state)
        {
            // 모든 로드가 완료된 시점에서 호출되어야 하겠다.
            FSM.Enable( UnitState.Idle );
        }
        return state;        
    }

    public override void Die(Unit attacker, float pushPower = 1f)
    {
        base.Die( attacker, pushPower );

        //< 프롭이 부서졌을시에 보상 리스트가 있을시 추가
        if (this is EventUnit)
            return;

        //if(G_GameInfo.GameMode == GAME_MODE.SINGLE)
        //{
        //    List<NetData.GachaDataGroup> list = (G_GameInfo._GameInfo as SingleGameInfo).propDropList;
        //    if (list == null || list.Count == 0)
        //        return;

        //    //< 누적
        //    (G_GameInfo._GameInfo as SingleGameInfo).OnDieProp();

        //    //< 꽝
        //    if(list[0].listGacha == null || list[0].listGacha.Count == 0)
        //    {
        //        list.RemoveAt(0);
        //        return;
        //    }

        //    Transform leaderTrn = G_GameInfo.PlayerController.Leader.transform;
        //    for (int i = 0; i < 5; i++)
        //        DropItem.DropAssets(this.transform, leaderTrn, i, ((float)list[0].listGacha[0].gold * 0.2f), true);

        //    attacker.Owner.TotalGold += (uint)((float)list[0].listGacha[0].gold * NetData.instance.GetDoubleBuffCheck(InGameBuffItemType.GoldUp));
        //    attacker.Owner.OrgTotalGold += list[0].listGacha[0].gold;
        //    list.RemoveAt(0);
        //}
    }
}

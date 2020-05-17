using UnityEngine;
using System.Collections;

public class EventUnit : Prop
{
    public GameObject ringShadow;

    string ModelName = "";
    string[] ObjectAniNames;
    protected override void Init_SyncData(params object[] args)
    {
        base.Init_SyncData(args);

        ModelName = (string)args[5];

        if (args[6] != null)
            ObjectAniNames = (string[])args[6];
    }

    public void ShowUnit(float hp)
    {
        //G_GameInfo.GameInfo.BoardPanel.ShowHpSlider(gameObject);
        ringShadow.SetActive(true);

        CharInfo.Stats[AbilityType.HP].Value = hp;
        CharInfo.Hp = (int)hp;
    }

    protected override void Init_Model()
    {
        //GameObject modelGO = ResourceMgr.Load(ModelName) as GameObject;
        //if (null == modelGO)
        //    AssetbundleLoader.AssetLoad(AssetbundleLoader.CreateSuffixForPlatform(ModelName), OnLoadedModelGO);
        //else
        //    OnLoadedModelGO(modelGO);
    }

    protected override void SetupComponents()
    {
        base.SetupComponents();
        navAgent.avoidancePriority = 1;
    }

    //protected void OnLoadedModelGO(GameObject prefabObj)
    //{
    //    if (Model.IsReady)
    //        Model.DeleteModel();

    //    GameObject modelGO = GameObject.Instantiate(prefabObj) as GameObject;
    //    NGUITools.SetLayer(modelGO, this.gameObject.layer);
    //    if (ObjectAniNames != null)
    //    {
    //        AssetbundleLoader.AddAnimationClip(modelGO.animation, ObjectAniNames, () =>
    //        {
    //            SetModel(modelGO);
    //        });
    //    }
    //    else
    //        SetModel(modelGO);
    //}

    //void SetModel(GameObject modelGO)
    //{
    //    Model.Init(this.gameObject, modelGO, comData.Info.battleSize);
    //    if (null != modelGO.animation)
    //    {
    //        Animator.Init(this.gameObject, modelGO.animation, CharInfo.animDatas);
    //        Animator.Animation.playAutomatically = false;
    //    }

    //    //////////////////////////
    //    LoadingComplete();
    //}

    public void SetSizeData(Vector2 size)
    {
        navAgent.radius = size.x / 2;
        navAgent.height = size.y;

        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.size = new Vector3(size.x, size.y, size.x);
        collider.center = new Vector3(collider.center.x, size.y / 2, collider.center.z);

        Radius = navAgent.radius;
        Height = collider.size.y;
    }

    protected override bool LoadingComplete()
    {
        bool state = base.LoadingComplete();

        if (state)
        {
            ringShadow = ResourceMgr.Instantiate<GameObject>("Actor/RingShadow_Target");
            ringShadow.transform.AttachTo(transform, new Vector3(0f, 0.15f, 0f), Vector3.one * 3, Quaternion.Euler(new Vector3(90, 0, 0)));
            ringShadow.SetActive(false);

            StaticState(true);
        }
        return state;
    }

    public override void DeleteShadow()
    {
        base.DeleteShadow();

        Destroy(ringShadow);
    }
}
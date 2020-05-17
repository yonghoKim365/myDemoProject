using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class PartnerPanel : UIBasePanel
{
    public Transform ListGridTf;//파트너의 스크롤뷰 어미 슬롯 셋팅용
    public Transform SpawnEffectRoot;//소환시 나오는 이펙트의 자리 어미

    enum InfoViewType { ListInfo, SpawnInfo, OwnInfo }//InfoViewObj의 배열값
    public GameObject[] InfoViewObj;//'0 ListInfo, 1 DetailInfo' InfoViewType 배열사용

    public GameObject SpawnEffectPrefab;
    public GameObject SpawnBGPrefab;

    public PartnerOwnView OwnPartnerView;//OwnView Script

    public UITabGroup TabListGroup;

    public float ParticleScale = 0.4f;

    private NetData._UserInfo CharInven;//캐릭터가 가지고 있는 인벤토리
    private InfoViewType CurInfoView;//현제 어디를 보고있는지 알기 위한값
    private int CurPnDataArray;//보고있는 파트너의 배열번호
    private int CurPartnerTabArr;//보고있는 파트너 탭 번호
    private bool IsDuringEffect;//이펙트 중인지 확인하는 변수

    public override void Init()
    {
        SceneManager.instance.sw.Reset();
        SceneManager.instance.sw.Start();
        SceneManager.instance.showStopWatchTimer(" PartnerPanel Init() start");

        base.Init();

        CharInven = NetData.instance.GetUserInfo();
               
        Transform spawntf = InfoViewObj[(int)InfoViewType.SpawnInfo].transform;
        GameObject bg = Instantiate(SpawnBGPrefab) as GameObject;
        bg.transform.parent = spawntf.FindChild("background_root");
        bg.transform.localPosition = Vector3.zero;
        bg.transform.localScale = Vector3.one;
        bg.transform.localEulerAngles = Vector3.zero;

        SceneManager.instance.showStopWatchTimer(" PartnerPanel init 10");

        ChangeInfoView(InfoViewType.ListInfo);
        CreatePartnerSlot();
        SceneManager.instance.showStopWatchTimer(" PartnerPanel init 20");

        OwnPartnerView.Init();
        
        
        //TempCoroutine.instance.FrameDelay(1.0f, () => {
            TabListGroup.Initialize(OnClickBuffTypeList);
        //});
		
        SceneManager.instance.showStopWatchTimer(" PartnerPanel init end");
        SceneManager.instance.sw.Stop();
    }

    public override void LateInit()
    {
        base.LateInit();
        
        if(mStarted && parameters != null && 0 < parameters.Length && (bool)parameters[0])//초기화 
        {
            parameters[0] = false;
            ChangeInfoView(InfoViewType.ListInfo);
            OnTabPartnerList(CurPartnerTabArr);
            uiMgr.SetTopMenuTitleName(7);

            ListGridTf.parent.GetComponent<UIScrollView>().ResetPosition();
        }

        List<NetData._PartnerData> dataList = GetSortPartnerList();//CharInven.GetPartnerList();
        NetData._PartnerData partnerData = dataList[CurPnDataArray];
        if (CurInfoView == InfoViewType.SpawnInfo )
        {
            ChangeInfoView(InfoViewType.OwnInfo);
            OwnPartnerView.SetPartnerInfo(partnerData);
        }
        //else if(CurInfoView == InfoViewType.OwnInfo)
        //{
        //    ChangeInfoView(InfoViewType.ListInfo);
        //    OnTabPartnerList(CurPartnerTabArr);
        //    uiMgr.SetTopMenuTitleName(7);

        //    ListGridTf.parent.GetComponent<UIScrollView>().ResetPosition();
        //}
        
        if (CurInfoView == InfoViewType.ListInfo)
            OnSubTutorial();

    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    /// <summary> 파트너 리스트에서 상단 탭버튼 </summary>
    void OnClickBuffTypeList(int arr)
    {
        CurPnDataArray = 0;
        CurPartnerTabArr = arr;
        int gridSlotCount = OnTabPartnerList(arr);

        //List<NetData._PartnerData> dataList = GetSortPartnerList();

        UIScrollView scroll = ListGridTf.parent.GetComponent<UIScrollView>();
        scroll.enabled = true;
        scroll.ResetPosition();
        //스크롤 예외처리
        if (gridSlotCount <= 12)
        {
            scroll.enabled = false;
        }
    }

    /// <summary>
    /// View타입에 맞게 끄고 켜준다
    /// </summary>
    /// <param name="type">키고자 하는 타입</param>
    void ChangeInfoView(InfoViewType type)
    {
        CurInfoView = type;
        {
            for (int i = 0; i < InfoViewObj.Length; i++)
            {
                InfoViewObj[i].SetActive(i==(int)CurInfoView);
            }
        }
    }
    
    public override void Hide()
    {
        //if (IsDuringEffect)
        //{
        //    if (0 < SpawnEffectRoot.childCount)
        //        DestroyImmediate(SpawnEffectRoot.GetChild(0).gameObject);

        //    //Cam3D.rect = new Rect(Cam3DViewport, Cam3D.rect.y, Cam3D.rect.width, Cam3D.rect.height);

        //    TempCoroutine.instance.StopAllCoroutines();
        //    SpawnEffectRoot.parent = transform;
        //    IsDuringEffect = false;
        //}

		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        if (CurInfoView == InfoViewType.ListInfo)
        {
            base.Hide();
            SceneManager.instance.sw.Stop();
            UIMgr.OpenTown();
        }
        else
        {
            if (!OwnPartnerView.CheckClose())
                return;

            //InfoViewType prev = CurInfoView;

            ChangeInfoView(InfoViewType.ListInfo);
            OnTabPartnerList(CurPartnerTabArr);
            uiMgr.SetTopMenuTitleName(7);
            
            ListGridTf.parent.GetComponent<UIScrollView>().ResetPosition();
        }
        
    }

    public override void ObjectHide()
    {
        base.ObjectHide();
        if (IsDuringEffect)//현재로는 업적으로 이동한 것임 꺼지기전에 정리한다.
        {
            if (0 < SpawnEffectRoot.childCount)
                DestroyImmediate(SpawnEffectRoot.GetChild(0).gameObject);

            //Cam3D.rect = new Rect(Cam3DViewport, Cam3D.rect.y, Cam3D.rect.width, Cam3D.rect.height);

            TempCoroutine.instance.StopAllCoroutines();
            SpawnEffectRoot.parent = transform;
            IsDuringEffect = false;
        }
    }

    /// <summary>
    /// 타입에 맞는 InfoViewObj[]리스트를 뱉어준다
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    GameObject GetViewObj(InfoViewType type)
    {
        return InfoViewObj[(uint)type];
    }

    #region 소환 뷰

    /// <summary>
    /// 캐릭터의 좌우 버튼 눌렀을때 이동시키는 것 처리하는 함수
    /// </summary>
    /// <returns>null 일경우 문제가 있는것.</returns>
    NetData._PartnerData MovePartnerView()
    {
        List<NetData._PartnerData> dataList = GetSortPartnerList();
        bool left = true, right = true;
        if (dataList.Count - 1 <= CurPnDataArray)
        {
            CurPnDataArray = dataList.Count - 1;
            right = false;
        }

        if (CurPnDataArray <= 0)
        {
            CurPnDataArray = 0;
            left = false;
        }

        return dataList[CurPnDataArray];
    }


    /// <summary>
    /// 파트너 획득
    /// </summary>
    /// <param name="itemInfo"></param>
    void OnClickPartnerGet(ushort parDataId)
    {
        if (IsDuringEffect)
            return;

        List<NetData._PartnerData> dataList = GetSortPartnerList();

        NetData._PartnerData data = null;//MovePartnerView();
        List<NetData._PartnerData> list = GetSortPartnerList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i]._partnerDataIndex != parDataId)
                continue;

            CurPnDataArray = i;
            break;
        }

        data = dataList[CurPnDataArray];
        if (data.NowShard < data._needShard)
        {
            UIMgr.instance.AddPopup(141, 1176, 117);
            return;
        }

        if (SpawnEffectPrefab == null)
            return;

        NetworkClient.instance.SendPMsgHeroFusionC(data._partnerDataIndex);
    }

    public void OnPartnerFusion(uint lowDataId)
    {
        List<NetData._PartnerData> dataList = GetSortPartnerList();
        NetData._PartnerData partnerData = null;

        int count = dataList.Count;
        for (int i = 0; i < count; i++)
        {
            if (dataList[i]._partnerDataIndex != lowDataId)
                continue;

            CurPnDataArray = i;
            MovePartnerView();
            partnerData = dataList[i];
            break;
        }

        IsDuringEffect = true;
        ChangeInfoView(InfoViewType.SpawnInfo);

        Transform tf = InfoViewObj[(int)InfoViewType.SpawnInfo].transform.FindChild("root");
        GameObject go = UIHelper.CreatePartnerUIModel(tf, partnerData._partnerDataIndex, 1, true, false, "PartnerPanel");
        UIModel model = go.GetComponent<UIModel>();
        
        GameObject effGo = model.PlayIntro(SpawnEffectRoot, ParticleScale);
        if (effGo != null)
            SpawnEffectRoot.localRotation = Quaternion.Euler(Vector3.zero);
        else
        {
            effGo = Instantiate(SpawnEffectPrefab) as GameObject;
            SpawnEffectRoot.localRotation = Quaternion.Euler(new Vector3(20, 0, 0));
        }

        float delay = model.UnitAnim.GetAnimLength(eAnimName.Anim_intro) + 0.1f;
        TempCoroutine.instance.FrameDelay(delay, () =>
        {
            if (!IsDuringEffect)
                return;

            //Cam3D.rect = new Rect(Cam3DViewport, Cam3D.rect.y, Cam3D.rect.width, Cam3D.rect.height);
            SpawnEffectRoot.parent = transform;
            DestroyImmediate(effGo);
            IsDuringEffect = false;

            ///보유하고 있는 뷰로 이동시킨다.
            ChangeInfoView(InfoViewType.OwnInfo);
            OwnPartnerView.SetPartnerInfo(partnerData);
        });

        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(833), partnerData.GetLocName());
        UIMgr.AddLogChat(msg);
    }

    public string GetPartnerSlotName(bool isEmpty)
    {
        int num = 0;
        List<NetData._PartnerData> dataList = CharInven.GetPartnerList();
        for (int i = 0; i < dataList.Count; i++)
        {
            if ((isEmpty && dataList[i]._isOwn) || (!isEmpty && !dataList[i]._isOwn))
                continue;

            num = i;
            break;
        }

        UIScrollView scroll = ListGridTf.parent.GetComponent<UIScrollView>();

        if (11 < num)
            scroll.contentPivot = UIWidget.Pivot.BottomLeft;
        else if (6 < num)
            scroll.contentPivot = UIWidget.Pivot.Center;

        scroll.ResetPosition();

        return num.ToString();
    }

    #endregion 소환뷰 끝

    #region 리스트

    /// <summary>
    /// 파트너 슬롯 생성 함수
    /// </summary>
    void CreatePartnerSlot()
    {
        List<NetData._PartnerData> dataList = CharInven.GetPartnerList();

        ListGridTf.GetChild(0).name = "0";
        ListGridTf.GetChild(0).gameObject.SetActive(false);
        int loopCount = dataList.Count;
        for (int i = 1; i < loopCount; i++)
        {
            Transform newSlotTf = Instantiate(ListGridTf.GetChild(0)) as Transform;
            newSlotTf.parent = ListGridTf;
            newSlotTf.localPosition = Vector3.zero;
            newSlotTf.localScale = Vector3.one;

            newSlotTf.name = string.Format("{0}", i);
        }
    }

    /// <summary>
    /// Partner를 셋팅 및 보여준다.
    /// </summary>
    /// <param name="type">PartnerType으로 타입에 맞게 보여줌</param>
    int OnTabPartnerList(int typeIdx)
    {
        PartnerClassType type = (PartnerClassType)typeIdx;
        int gridSlotCount = 0;

        bool isTuto = SceneManager.instance.CurTutorial == TutorialType.PARTNER;
        List<NetData._PartnerData> dataList = CharInven.GetPartnerList();
        int loopCount = dataList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            ListGridTf.GetChild(i).gameObject.SetActive(false);

            NetData._PartnerData data = dataList[i];
            Partner.PartnerDataInfo info = data.GetLowData();
            if (type != PartnerClassType.None && (PartnerClassType)info.Class != type)
                continue;

            Transform slotTf = ListGridTf.GetChild(gridSlotCount);
            slotTf.gameObject.SetActive(true);
           

            slotTf.FindChild("Mark").GetComponent<UISprite>().spriteName = data.GetClassType();
            //보유조각수
            slotTf.FindChild("Txt_count").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1154), data.NowShard, data._needShard);


            UISprite face = slotTf.FindChild("Itemroot/face").GetComponent<UISprite>();
            UISprite bg = slotTf.FindChild("Itemroot/gradebg").GetComponent<UISprite>();
            face.spriteName = data.GetIcon();
            slotTf.FindChild("Txt_name").GetComponent<UILabel>().text = data.GetLocName();

            UILabel level = slotTf.FindChild("Txt_class").GetComponent<UILabel>();

            //등급은 색상도 고려해줘..
            UILabel gradeLabel = slotTf.FindChild("Txt_type").GetComponent<UILabel>();

            slotTf.FindChild("Itemroot/grade").GetComponent<UISprite>().spriteName = string.Format("Icon_0{0}", data.CurQuality);
            bg.spriteName = string.Format("Icon_bg_0{0}", data.CurQuality);
            
            Transform btnGetTf = slotTf.FindChild("Btn_Get"); //소환버튼
            GameObject btnGetAfter = slotTf.FindChild("Btn_Getafter").gameObject;   //보유중일때나옴
            btnGetAfter.SetActive(data._isOwn);
            btnGetTf.gameObject.SetActive( !data._isOwn);
            slotTf.FindChild("bg_on").gameObject.SetActive(data._isOwn);//배경
            
            if (data._isOwn)
            {
                EventDelegate.Set(btnGetAfter.GetComponent<UIEventTrigger>().onClick, delegate () {
                    UIMgr.instance.AddPopup(141, 1177, 117);    //보유중알림메시지
                });

                if (data._NowLevel >= data._MaxLevel)//최대렙
                    level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1152), data._NowLevel);
                else
                    level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), data._NowLevel);   
                
                //색상도고려
                gradeLabel.text =string.Format("{0}{1}[-]",UIHelper.GetItemGradeColor((int)data.CurQuality), data.GetGradeName());
            }
            else
            {
                if(isTuto && type == PartnerClassType.Attack && data.NowShard >= data._needShard)
                {
                    isTuto = false;
                    TutorialSupport support = btnGetTf.gameObject.AddComponent<TutorialSupport>();
                    support.TutoType = TutorialType.PARTNER;
                    support.SortId = 3;
                    support.IsScroll = true;

                    //support.NextTuto = OwnPartnerView.TabBtn[1].gameObject.GetComponent<TutorialSupport>();
                    support.OnTutoSupportStart();
                }

                btnGetTf.FindChild("Btn_on").gameObject.SetActive(data.NowShard >= data._needShard);
                btnGetTf.FindChild("Btn_off").gameObject.SetActive(data.NowShard < data._needShard);

                level.text = "";
                gradeLabel.text = _LowDataMgr.instance.GetStringCommon(1155); //없으면 미보유
                EventDelegate.Set(btnGetTf.GetComponent<UIEventTrigger>().onClick, delegate () { OnClickPartnerGet(data._partnerDataIndex); });  //소환     
            }
            
            //슬롯 클릭시 실행할 함수 셋팅
            EventDelegate.Set(slotTf.GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickListSlot(data._partnerDataIndex);
            });

            ++gridSlotCount;
        }

        ListGridTf.GetComponent<UIGrid>().repositionNow = true;

        return gridSlotCount;
    }

    

    /// <summary>
    /// 총 공격력 업그레이드 버튼
    /// </summary>
    void OnClickTotalDamageUpgrade()
    {

    }
    /// <summary>
    /// 조각에 대한 도움말
    /// </summary>
    void OnClickHelpSlice()
    {

    }

    /// <summary>
    /// ListSlot을 클릭하면 실행 됨
    /// </summary>
    /// <param name="itemInfo"></param>
    void OnClickListSlot(ushort parDataId)
    {
        //CurPnDataArray = idx;
        NetData._PartnerData data = null;//MovePartnerView();
        List<NetData._PartnerData> list = GetSortPartnerList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i]._partnerDataIndex != parDataId)
                continue;

            CurPnDataArray = i;
            data = list[i];
            break;
        }


        ChangeInfoView(InfoViewType.OwnInfo);
        OwnPartnerView.SetPartnerInfo(data);

    }

    List<NetData._PartnerData> GetSortPartnerList()
    {
        List<NetData._PartnerData> parList = CharInven.GetPartnerList();
        List<NetData._PartnerData> tempList = null;

        PartnerClassType type = (PartnerClassType)CurPartnerTabArr;
        if (type != PartnerClassType.None)
        {
            tempList = new List<NetData._PartnerData>();
            for (int i = 0; i < parList.Count; i++)
            {
                Partner.PartnerDataInfo info = parList[i].GetLowData();
                if ((PartnerClassType)info.Class != type)
                    continue;

                tempList.Add(parList[i]);
            }

            OwnPartnerView.CurTabPartnerList = tempList;
            //return tempList;
        }
        else
        {
            OwnPartnerView.CurTabPartnerList = parList;
            //return parList;

        }

        //Debug.Log("소팅전");
        //for(int i=0;i< OwnPartnerView.CurTabPartnerList.Count;i++)
        //{
        //    Debug.Log(string.Format("idx : {0} isOwn :{1} nedd {2} nox {3}", OwnPartnerView.CurTabPartnerList[i]._partnerDataIndex,
        //        OwnPartnerView.CurTabPartnerList[i]._isOwn, OwnPartnerView.CurTabPartnerList[i]._needShard, OwnPartnerView.CurTabPartnerList[i].NowShard));
        //}

        OwnPartnerView.CurTabPartnerList.Sort(SortPartnerList);

        //Debug.Log("소팅후");
        //for (int i = 0; i < OwnPartnerView.CurTabPartnerList.Count; i++)
        //{
        //    Debug.Log(string.Format("idx : {0} isOwn :{1} nedd {2} nox {3}", OwnPartnerView.CurTabPartnerList[i]._partnerDataIndex,
        //        OwnPartnerView.CurTabPartnerList[i]._isOwn, OwnPartnerView.CurTabPartnerList[i]._needShard, OwnPartnerView.CurTabPartnerList[i].NowShard));
        //}

        return OwnPartnerView.CurTabPartnerList;
    }

    /*
     1순위 소환가능한파트너..
     2순위 보유
     3순위 보유중, 높은등급
     4순위 보유중, 높은등급, 낮은파트너ID
     5순위 미보유
     6순위 미보유 낮은 파트너ID
         */
    int SortPartnerList(NetData._PartnerData a, NetData._PartnerData b)
    {

        if (a._isOwn && b._isOwn)
        {
            //둘다 보유하고있을때체크
            uint aGrade = a.CurQuality, bGrade = b.CurQuality;

            if (a == b)
                return 0;

            if (aGrade < bGrade)
                return 1;
            else if (aGrade == bGrade)
            {
                if (a._partnerDataIndex < b._partnerDataIndex)
                    return 1;
                else
                    return -1;
            }
            else
                return -1;
        }
        else if (a == b)
            return 0;
        else if (a._isOwn && !b._isOwn)
        {
            if (b.NowShard >= b._needShard)
                return 1;
            return -1;
        }
        else if (!a._isOwn && b._isOwn)
        {
            if (a.NowShard >= a._needShard)
                return -1;

            return 1;
        }
        else
        {
            //둘다 보유안하고잇음 , 소환가능한것을 앞으로
            if (a.NowShard >= a._needShard && b.NowShard < b._needShard) //a가능 b불가
                return -1;
            else if (a.NowShard >= a._needShard && b.NowShard >= b._needShard)   //a가능 b가능 -> 인덱스순
            {
                if (a._partnerDataIndex < b._partnerDataIndex)
                    return -1;
                return 1;
            }
            else if (a.NowShard < a._needShard && b.NowShard < b._needShard)    // a불가 b불가 
            {
                //둘다 소환불가능하면 인덱스순으로 
                if (a._partnerDataIndex < b._partnerDataIndex)
                    return -1;

                return 1;
            }
            else    //a불가 b가능?
                return 1;
        }
        

    }

    void CreatePartnerModel(Transform parent, NetData._PartnerData data)
    {
        GameObject go = UIHelper.CreatePartnerUIModel(parent, data._partnerDataIndex, 1, data._isOwn, false, "PartnerPanel");
        //go.SetActive(false);
        parent.localEulerAngles = Vector3.zero;
        //TempCoroutine.instance.FrameDelay(0.01f, () => {
        if (go == null)
            return;

        //go.SetActive(true);
        //go.GetComponent<UIModel>().PlayAnim(eAnimName.Anim_idle);
        Transform modelTf = go.transform;
        int childCount = modelTf.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform tf = modelTf.GetChild(i);
            if (!tf.name.Contains("par_"))
                continue;

            int shaders = tf.renderer.materials.Length;
            for (int j = 0; j < shaders; j++)
            {
                Material m = tf.renderer.materials[j];
                //if (isSpawnView)
                //{
                //    m.SetFloat("_FlashAmount", 0f);
                //}
                //else
                {
                    m.SetFloat("_FlashAmount", data._isOwn ? 0f : 1f);
                    m.SetColor("_FlashColor", Color.black);
                }
            }
        }

        //});
    }

    #endregion 리스트 끝


    #region 서버 응답 처리

       /// <summary> 파트너 진화 별 상승 응답 </summary>
    public void OnPMsgHeroPromoSHandler(uint partnerId)
    {
        OwnPartnerView.OnPMsgHeroPromoSHandler(partnerId);
    }

    /// <summary> 파트너 액티브스킬 레벨업 </summary>
    public void OnPMsgHeroSkillUpgradeSHandler()
    {
        OwnPartnerView.OnPMsgHeroSkillUpgradeSHandler();
    }

    public override byte TabType
    {
        get
        {
            return OwnPartnerView.TabType;
        }
    }

    #endregion

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Cam3D.rect = new Rect(0, Cam3D.rect.y, Cam3D.rect.width, Cam3D.rect.height);

            List<NetData._PartnerData> dataList = GetSortPartnerList();//CharInven.GetPartnerList();
            NetData._PartnerData partnerData = dataList[CurPnDataArray];

            IsDuringEffect = true;
            //ChangeShader(true, false);
            ChangeInfoView(InfoViewType.SpawnInfo);

            Transform tf = InfoViewObj[(int)InfoViewType.SpawnInfo].transform.FindChild("root");
            GameObject go = UIHelper.CreatePartnerUIModel(tf, partnerData._partnerDataIndex, 1, true, false, "PartnerPanel");

            UIModel model = go.GetComponent<UIModel>();
            GameObject effGo = model.PlayIntro(SpawnEffectRoot, ParticleScale);
            if (effGo != null)
                SpawnEffectRoot.localRotation = Quaternion.Euler(Vector3.zero);
            else
            {
                effGo = Instantiate(SpawnEffectPrefab) as GameObject;
                SpawnEffectRoot.localRotation = Quaternion.Euler(new Vector3(20, 0, 0));
            }

            float delay = model.UnitAnim.GetAnimLength(eAnimName.Anim_intro) + 0.1f;
            //RotationTargetList[0].transform.parent.gameObject.SetActive(false);
            TempCoroutine.instance.FrameDelay(delay, () =>
            {
                // Cam3D.rect = new Rect(Cam3DViewport, Cam3D.rect.y, Cam3D.rect.width, Cam3D.rect.height);

                model.SetWeaponEffectActive(true);
                SpawnEffectRoot.parent = transform;
                DestroyImmediate(effGo);
                IsDuringEffect = false;

                ChangeInfoView(InfoViewType.ListInfo);
            });

        }
    }
#endif
}

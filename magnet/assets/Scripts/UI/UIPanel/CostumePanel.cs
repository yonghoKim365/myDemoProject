using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CostumePanel : UIBasePanel {
    
    enum ViewType { None=-1, List=0, Evolve, Socket, Skill, }//ViewObj의 배열값
    public GameObject[] ViewObj;//0 List, 1 Enchant, 2 Socket, 3 Skill View 오브젝트로 되어있다. ViewType이걸로 사용
    public GameObject[] EvolveStars;//승급 별이 몇개인지
    public GameObject CharView;//스킬 뷰일 경우 캐릭터뷰가 꺼져야한다
    public GameObject InvenSlotPrefab;//인벤토리 슬롯 프리팹
    //public GameObject BlockingObj; //이펙트중 막아주기.

    public Transform[] TargetCostume;//0 진화, 1 소켓
    public Transform[] EvolveRootTfs;//0 재료, 1재료, 2재료
    public Transform[] SocketRootTfs;//0 재료, 1재료, 2재료, 3재료

    public UIButton BtnOnceSkillLevelup;//1회 렙업
    public UIButton BtnMaxSkillLevelup;//최대치까지 렙업
    public UIButton BtnEvolve;//승급버튼
    
    public UILabel UserNickAndLevel;//유저 닉네임, 이름
    public UILabel CostumeName;//코스튬 이름
    public UILabel EvolCount;//승급 몇회 째인지.(별이 10개에 1회 상승)
    public UILabel EvolveCost;//승급 가격
    public UILabel[] EvolMaterialAmount;//승급에 필요한 재료 개수 표기
    
    public UITabGroup TabGroup;//우측 탭 리스트 제어해줄 스크립트
    public UIScrollView ListScrollView;
    public UIScrollView JewelScrollView;//보석 아이템 보여주는 슬롯.

    public UIGrid JewelGrid;

    private ViewType CurViewType = ViewType.None;//현재 보고있는 뷰 타입
    private NetData._UserInfo UserInfo;//캐릭터 인벤토리
    private SkillLevelupData SkillLevelup;
    private InvenItemSlotObject[] EvolInvenSlots;//승급 0 재료, 1재료, 2재료
    private InvenItemSlotObject[] JewelInvenSlots;//소켓 0재료, 1재료, 2재료, 3재료
    private NetData.UpgradeMaterialData EvolveMaterialData;//승급에 필요한 재료 정보.
    //private ItemDetailPopup DetailPopup;//코스튬 보석 박기할 경우 사용될 녀석.
    
    private GameObject SkillUpEff;//스킬 업그레이드 이펙트

    private Transform SelectSkillEff;//스킬 선택 이펙트

    public Transform EvoleEffRoot; //Fx_UI_cos_upgrade 
    
    private bool IsChangeCostume=false;//코스튬 변경했는지 여부 했다면 페널 나갈때 마을에 유닛 변경시킴
    private bool IsPreViewCostume;//코스튬을 미리보기했을 경우 다른 뷰에서는 장착한 것으로 변경해주려고 넣은 변수;
    private bool IsMaxSkillup;//스킬 최대 레벨 업
    private int SelectJewelArr;//선택한 빈 보석 슬롯 번호 0~4
    private int SkillMaxLv;
    private string BasePanel;   //난투장패널에서 열경우 

    struct SkillLevelupData
    {
        //public bool IsCanNotLevelup;//강화 가능?
        //public bool IsNoMony;
        public int SelectSkillNum;//선택 스킬
        public int SelectSkillLevel;//선택한 스킬의 레벨
        public uint SkillId;//선택한 스킬의 아이디.

        public void Init()
        {
            //IsNoMony = false;
            //IsCanNotLevelup = false;
            SelectSkillNum = -1;
            SelectSkillLevel = 0;
            SkillId = 0;
        }
    }
    
    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();

		SceneManager.instance.showStopWatchTimer ("costume panel, Init() start");

        base.Init();

        UserInfo = NetData.instance.GetUserInfo();
        SkillMaxLv = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.CharacterMaxSkillLevel);

        Transform gridTf = ViewObj[(uint)ViewType.List].transform.FindChild("ScrollView/Grid");
        if (gridTf.childCount < UserInfo.GetCostumeList().Count)//슬롯에 뿌려줘야 하는 데이터가 더 많다 슬롯 생성한다.
        {
            int createCount = 5;//일단임의로? 이거물어봐야해 //UserInfo.GetCostumeList().Count;
            UIHelper.CreateSlotItem(true, createCount, gridTf.GetChild(0), gridTf, null);
        }
        
        InitEventButton();
        
        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();
        CreateCostume(costumeData, false, false);

        string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
        UserNickAndLevel.text = string.Format("{0} {1}", lvStr, NetData.instance.Nickname);
        CostumeName.text = costumeData.GetLocName();//itemInfo.itemName;

        //승급 초기화
        int loopCount = EvolveRootTfs.Length;
        EvolInvenSlots = new InvenItemSlotObject[loopCount];
        for (int i = 0; i < loopCount; i++)
        {
            GameObject slotGo = Instantiate(InvenSlotPrefab) as GameObject;
            Transform tf = slotGo.transform;
            tf.parent = EvolveRootTfs[i];
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

            EvolInvenSlots[i] = slotGo.GetComponent<InvenItemSlotObject>();
            //EvolInvenSlots[i].SetBackGround("Blod_SlotBg02");
        }

        loopCount = SocketRootTfs.Length;
        JewelInvenSlots = new InvenItemSlotObject[loopCount];
        for(int i=0; i < loopCount; i++)
        {
            GameObject slotGo = Instantiate(InvenSlotPrefab) as GameObject;
            Transform tf = slotGo.transform;
            tf.parent = SocketRootTfs[i].FindChild("target");
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

            JewelInvenSlots[i] = slotGo.GetComponent<InvenItemSlotObject>();
            JewelInvenSlots[i].EmptySlot();
            
            UIEventTrigger uiTri = SocketRootTfs[i].GetComponent<UIEventTrigger>();
            int slotArr = i;
            EventDelegate.Set(uiTri.onClick, delegate () {
                OnClickJewelSlot(slotArr);
            });
        }

        UILabel label = BtnOnceSkillLevelup.transform.FindChild("label_d6").GetComponent<UILabel>();
        label.text = string.Format(_LowDataMgr.instance.GetStringCommon(45), 1);

        SelectSkillEff = UIHelper.CreateEffectInGame(GetViewObj(ViewType.Skill).transform, "Fx_UI_circle_select").transform;
        SkillUpEff = UIHelper.CreateEffectInGame(GetViewObj(ViewType.Skill).transform, "Fx_UI_cos_skillup");
        
        TabGroup.Initialize(OnClickTab);


		SceneManager.instance.showStopWatchTimer ("costume panel, Init() finish");
    }

    public override void LateInit()
    {
        base.LateInit();

        if (parameters.Length == 0)
            return;
        if (parameters[0] != null && parameters[0].ToString().Contains("UIPanel"))
            BasePanel = parameters[0].ToString();
        else
            BasePanel = null;

		SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("costume panel, LateInit() finish");
        
        //OnTutorial();
    }

    void CreateCostume(NetData._CostumeData cos, bool isChange, bool isHideCostume=false)
    {
        //GameObject go = UIHelper.CreatePcUIModel("CostumePanel",RotationTargetList[0].transform, UserInfo.GetCharIdx(), 0, cos._costmeDataIndex, 0, 0, (byte)(isChange ? 5 : 0), isHideCostume, false);
        //if(isChange)
        //    go.GetComponent<UIModel>().CrossFadeAnimation(eAnimName.Anim_skill8, eAnimName.Anim_idle);

        //if (go.name.Contains("pc_f") || go.name.Contains("pc_p"))
        //{
        //    go.transform.localScale = new Vector3(228, 228, 228);
        //}
        //else if(go.name.Contains("pc_d"))
        //{
        //    go.transform.localScale = new Vector3(242, 242, 242);
        //}
    }

    /// <summary>
    /// 뷰타입에 맞는 GameObject를 반환한다.
    /// </summary>
    /// <param name="type">원하는 타입</param>
    /// <returns></returns>
    GameObject GetViewObj(ViewType type)
    {
        GameObject viewObj = ViewObj[(uint)type];
        return viewObj;
    }

    /// <summary>
    /// 버튼 이벤트 처리 함수
    /// </summary>
    void InitEventButton()
    {
        EventDelegate.Set(BtnMaxSkillLevelup.onClick, delegate () { OnClickSkillLevelup(true); });
        EventDelegate.Set(BtnOnceSkillLevelup.onClick, delegate () { OnClickSkillLevelup(false); });

        EventDelegate.Set(BtnEvolve.onClick, OnClickEvolve );
    }
    /*
    /// <summary>
    /// List, Enchant, Socket, Skill 뷰로 전환한다.
    /// </summary>
    /// <param name="type"> 바꾸고자 하는 뷰의 타입</param>
    void ChangeView(ViewType type)
    {
        if (CurViewType == type)//동일하므로 무시
            return;

        for (int i = 0; i < SocketRootTfs.Length; i++)
        {
            GameObject select = SocketRootTfs[i].transform.FindChild("cover").gameObject;
            select.SetActive(false);
        }

        for (int i = 0; i < ViewObj.Length; i++)
        {
            //bool active = false;
            //if (i == (uint)type)
            //    active = true;

            ViewObj[i].SetActive(i==(int)type);
        }

        //ViewType prevType = CurViewType;
        if(CurViewType == ViewType.Evolve)
        {
            Transform effRoot = EvolveRootTfs[0].parent;
            if(EvoleEffRoot.transform.childCount>0)
                Destroy(EvoleEffRoot.GetChild(0).gameObject);   //이펙트지움
            //for (int i = 0; i < effRoot.childCount; i++)
            //{
            //    if(effRoot.GetChild(i).transform.GetChild(i).name.Contains("evoleEffRoot"))
            //        Destroy(EvoleEffRoot.GetChild(0).gameObject);
            //}

            for (int i = 0; i < EvolveStars.Length; i++)
            {
                if (1 < EvolveStars[i].transform.parent.childCount)
                    Destroy(EvolveStars[i].transform.parent.GetChild(1).gameObject);
            }
        }

        CurViewType = type;

        bool charViewActive = false;
        uint titleId = 0;
        switch (type)
        {
            case ViewType.List:
                CostumeListView();
                titleId = 6;
                charViewActive = true;
                break;
            case ViewType.Evolve:
                titleId = 32;
                ViewObj[(int)ViewType.List].SetActive(true);
                CheckEvolve(UserInfo.GetEquipCostume() );
                break;
            case ViewType.Socket:
                titleId = 40;
                InitSocket();
                break;
            case ViewType.Skill:
                titleId = 55;
                SkillViewSetting();
                break;
        }

        uiMgr.SetTopMenuTitleName(titleId);
        CharView.SetActive(charViewActive);
        //charViewActive || 
        if (IsPreViewCostume)
        {
            NetData._CostumeData costumeData = UserInfo.GetEquipCostume();

            CreateCostume(costumeData, false);
            CostumeName.text = costumeData.GetLocName();
            IsPreViewCostume = false;
        }
    }
    */
    public override byte TabType
    {
        get
        {
            return (byte)(CurViewType == ViewType.Evolve ? 2 : 0);
        }
    }

    #region 코스튬 리스트 뷰
    /// <summary>
    /// 코스튬 뷰를 보여준다.
    /// </summary>
    void CostumeListView()
    {
        List<NetData._CostumeData> costimeList = UserInfo.GetCostumeList();
        Transform listTf = ViewObj[(uint)ViewType.List].transform;
        Transform gridTf = listTf.FindChild("ScrollView/Grid");
        for(int i=0; i < gridTf.childCount; /*costimeList.Count;*/ i++)
        {
            Transform slotTf = gridTf.GetChild(i);

            slotTf.FindChild("costume").gameObject.SetActive(false);
            slotTf.FindChild("noncostum_slot").gameObject.SetActive(false);
            if (i >= costimeList.Count)
            {
                slotTf.FindChild("noncostum_slot").gameObject.SetActive(true);
                continue;
            }
            slotTf.FindChild("costume").gameObject.SetActive(true);

            NetData._CostumeData costumeData = costimeList[i];

            //아이템리스트에서 현재의 조각아이템을 찾아서 갯수를 가져온다 - 자주쓸거같으면 _Inventory안에 만들기
            Item.CostumeInfo tempCos = _LowDataMgr.instance.GetLowDataCostumeInfo(costumeData._costmeDataIndex);
            int shardCount = UserInfo.GetItemCountForItemId(tempCos.ShardIdx, (byte)eItemType.USE);

            slotTf.name = string.Format("{0}", costumeData._costmeDataIndex);
            UILabel nameAndStack = slotTf.FindChild("costume/name_and_stack_d1").GetComponent<UILabel>();
            UILabel ability = slotTf.FindChild("costume/ability_d1").GetComponent<UILabel>();
            UILabel info = slotTf.FindChild("costume/info_d1").GetComponent<UILabel>();
            UISprite icon = slotTf.FindChild("costume/icon").GetComponent<UISprite>();
            UISprite bg = slotTf.FindChild("costume/bg").GetComponent<UISprite>();
            NetData.ItemAbilityData abilityData = costumeData.AbilityData;

            nameAndStack.text = string.Format("{0} ({1}/{2})", costumeData.GetLocName(), shardCount, costumeData._needShard);

			float calculatedAbilityValue = _LowDataMgr.instance.GetCostumeAbilityValue (costumeData._Grade,costumeData._MinorGrade, abilityData.Value);
            ability.text = string.Format("{0} {1}", UIMgr.instance.GetAbilityLocName(abilityData.Ability)//이름
			                             , UIMgr.instance.GetAbilityStrValue(abilityData.Ability, calculatedAbilityValue));//지금 능력치

            info.text = costumeData.GetDescription();
            icon.spriteName = costumeData.GetIconName();

            UIButton uiBtnGet = slotTf.FindChild("costume/BtnGet").GetComponent<UIButton>();
            UIEventTrigger uiTriMount = slotTf.FindChild("costume/BtnMount").GetComponent<UIEventTrigger>();
            GameObject mountEff = slotTf.FindChild("costume/mount_effect").gameObject;
            GameObject mountCover = slotTf.FindChild("costume/cover").gameObject;
            bool isGetBtn = false;
            mountEff.SetActive(costumeData._isEquip);
            mountCover.SetActive(costumeData._isEquip);
            ///보유중
            if (costumeData._isOwn)
            {
                string text = null;
                if (costumeData._isEquip)
                {
                    text = _LowDataMgr.instance.GetStringCommon(37);
                    bg.spriteName = "Bod_List04";
                }
                else
                {
                    text = _LowDataMgr.instance.GetStringCommon(38);
                    bg.spriteName = "Bod_List05";

                    if(i != 0 )
                    {
                        Destroy(uiTriMount.gameObject.GetComponent<TutorialSupport>() );
                    }
                }

                EventDelegate.Set(uiTriMount.onClick, delegate ()
                {
                    if (costumeData._isEquip && (IsPreViewCostume ) )
                        OnClickPreView(costumeData);
                    else
                        OnClickMountCostume(costumeData);
                });

                uiTriMount.transform.GetComponent<UILabel>().text = text;
            }
            else//획득하지 못한 아이템
            {
                uiTriMount.transform.GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(38);

                if (costumeData._needShard <= shardCount)//획득 가능.
                {
                    uiBtnGet.collider.enabled = true;
                    slotTf.FindChild("costume/BtnGet/label_d2").GetComponent<UILabel>().color = Color.white;
                    EventDelegate.Set(uiBtnGet.onClick, delegate () { OnClickGetCostume(costumeData._costmeDataIndex); });//, slotTf
                }
                else//불가능
                {
                    uiBtnGet.collider.enabled = false;
                    slotTf.FindChild("costume/BtnGet/label_d2").GetComponent<UILabel>().color = Color.gray;
                }

                EventDelegate.Set(uiTriMount.onClick, delegate () { OnClickPreView(costumeData); });
                
                isGetBtn = true;
            }

            uiBtnGet.gameObject.SetActive(isGetBtn);
        }

        gridTf.GetComponent<UIGrid>().Reposition();

        ListScrollView.enabled = true;
        ListScrollView.transform.SetLocalY(0);
        ListScrollView.transform.GetComponent<UIPanel>().clipOffset = Vector3.zero;
       // ListScrollView.ResetPosition();

        if (costimeList.Count < 4)
            ListScrollView.enabled = false;
    }


    /// <summary> 코스튬 획득 버튼 헨들러 </summary>
    void OnClickGetCostume(uint costumeLowDataId)//, Transform tf)
    {
        if (costumeLowDataId <= 0)
            return;

        //SelectCostumeTfArr = arr;
        NetworkClient.instance.SendPMsgCostumeFusionC((int)costumeLowDataId);
    }
    
    /// <summary> 미리 보기. </summary>
    void OnClickPreView(NetData._CostumeData costumeData)
    {
        if (costumeData == null)
            return;

        CreateCostume(costumeData, true);
        ///코스튬 이름 변경
        CostumeName.text = costumeData.GetLocName();
        IsPreViewCostume = true;
    }

    /// <summary> 코스튬 획득(이미 보유)한 것은 이함수로. 아니면 OnClickGetCostume으로 </summary>
    void OnClickMountCostume(NetData._CostumeData costumeData)
    {
        if (costumeData == null)
            return;
        
        NetworkClient.instance.SendPMsgCostumeUserC((int)costumeData._costumeIndex, 1);
    }

    /// <summary> 코스튬 장착 응답 </summary>
    public void OnReceiveMountCostume(NetData._CostumeData costumeData)
    {
        CreateCostume(costumeData, true);
        ///코스튬 이름 변경
        CostumeName.text = costumeData.GetLocName();
        IsChangeCostume = true;

        //다시 재갱신이 필요하다.
        CostumeListView();
        if (CurViewType == ViewType.Evolve)
            CheckEvolve(costumeData);
    }

    /// <summary> 코스튬 획득 응답. </summary>
    public void OnReceiveGetCostume(NetData._CostumeData costumeData)
    {
        Transform gridTf = ViewObj[(uint)ViewType.List].transform.FindChild("ScrollView/Grid");
        Transform slotTf = gridTf.FindChild(string.Format("{0}", costumeData._costmeDataIndex) );//gridTf.GetChild(SelectCostumeTfArr);
        GameObject eff = UIHelper.CreateEffectInGame(slotTf, "Fx_UI_costume_select");
        Destroy(eff, 2f);
        //TempCoroutine.instance.FrameDelay()
        //CostumeListView();

        //아이템리스트에서 현재의 조각아이템을 찾아서 갯수를 가져온다 - 자주쓸거같으면 _Inventory안에 만들기
        //NetData._CostumeData costumeData = UserInfo.GetCostumeDataForLowDataID(costumeLowDataId);
        Item.CostumeInfo tempCos = costumeData.GetLowData();//_LowDataMgr.instance.GetLowDataCostumeInfo(costumeLowDataId);
        int shardCount = UserInfo.GetItemCountForItemId(tempCos.ShardIdx, (byte)eItemType.USE);

        UILabel nameAndStack = slotTf.FindChild("costume/name_and_stack_d1").GetComponent<UILabel>();
        UILabel ability = slotTf.FindChild("costume/ability_d1").GetComponent<UILabel>();
        UILabel info = slotTf.FindChild("costume/info_d1").GetComponent<UILabel>();
        UISprite icon = slotTf.FindChild("costume/icon").GetComponent<UISprite>();

        NetData.ItemAbilityData abilityData = costumeData.AbilityData;
		float abilityValue = _LowDataMgr.instance.GetCostumeAbilityValue (costumeData._Grade, costumeData._MinorGrade, abilityData.Value);

        nameAndStack.text = string.Format("{0} ({1}/{2})", costumeData.GetLocName(), shardCount, costumeData._needShard);
        ability.text = string.Format ("{0} {1}", UIMgr.instance.GetAbilityLocName (abilityData.Ability)//이름
		                             , UIMgr.instance.GetAbilityStrValue (abilityData.Ability, abilityValue));//abilityData.Value * 0.1f));//지금 능력치

        info.text = costumeData.GetDescription();
        icon.spriteName = costumeData.GetIconName();

        UIButton uiBtnGet = slotTf.FindChild("costume/BtnGet").GetComponent<UIButton>();
        UIEventTrigger uiTriMount = slotTf.FindChild("costume/BtnMount").GetComponent<UIEventTrigger>();
        GameObject mountEff = slotTf.FindChild("costume/mount_effect").gameObject;

        uiBtnGet.gameObject.SetActive(false);
        mountEff.SetActive(false);

        ///보유중
        uiTriMount.collider.enabled = true;
        uiTriMount.transform.GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(38);
        EventDelegate.Set(uiTriMount.onClick, delegate () { OnClickMountCostume(costumeData); });

        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(832), costumeData.GetLocName());
        UIMgr.AddLogChat(msg);
    }
    
    #endregion 코스튬 리스트 뷰 끝

    #region 스킬 뷰
    /*
    /// <summary>
    /// 스킬 뷰 초기화 함수 스킬 뷰로 올때마다 실행해준다
    /// </summary>
    void SkillViewSetting()
    {
        SkillUpEff.SetActive(false);
        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();

        GameObject skillViewObj = GetViewObj(ViewType.Skill);
        if (skillViewObj == null)
            return;

        Transform skillViewTf = skillViewObj.transform.FindChild("Skills");
        UILabel costumeName = skillViewObj.transform.FindChild("CostumeName").GetComponent<UILabel>();
        costumeName.text = costumeData.GetLocName();

        ushort[] skillLevel = costumeData._skillLevel;
        List<uint> skillList = costumeData.GetSkillList();
        int skillCount = skillList.Count;
        for (int i = 0; i < skillCount; i++)
        {
            UIEventTrigger uiTri = skillViewTf.FindChild(string.Format("BtnSkill_{0}", i)).GetComponent<UIEventTrigger>();
            UISprite icon  = skillViewTf.FindChild(string.Format("BtnSkill_{0}/icon_d5", i)).GetComponent<UISprite>();
            UILabel level  = skillViewTf.FindChild(string.Format("BtnSkill_{0}/level_d5", i )).GetComponent<UILabel>();
            UILabel name   = skillViewTf.FindChild( string.Format("BtnSkill_{0}/name_d5", i)).GetComponent<UILabel>();
            
            SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(skillList[i]);
            if (actionLowData == null)
            {
                Debug.LogError("SkillAction index error " + actionLowData.idx);
                continue;
            }

            int arr = i;
            ushort s_level = skillLevel[i];

            uiTri.collider.enabled = true; // ?????
            icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
            level.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), skillLevel[i] );
            name.text = _LowDataMgr.instance.GetStringSkillName(actionLowData.name);

            EventDelegate.Set(uiTri.onClick, delegate() { SetSkillInfo(actionLowData, arr, s_level); } );
        }

        //기본으로 0번 선택해 놓는다.
        SetSkillInfo(_LowDataMgr.instance.GetSkillActionLowData(skillList[0]), 0, skillLevel[0] );
    }
    */
    /// <summary>
    /// 선택한 스킬에 대한 정보를 출력해준다.
    /// </summary>
    void SetSkillInfo(SkillTables.ActionInfo actionLowData, int arr, ushort skillLevel)
    {
        SkillLevelup.Init();//일단 초기화하고 시작.

        SkillTables.SkillLevelInfo nowLevelLowData = _LowDataMgr.GetSkillLevelData(actionLowData.idx, (byte)skillLevel);//지금
        if (nowLevelLowData == null)
        {
            Debug.LogError("skillLevel index error " + actionLowData.idx);
            return;
        }

        uint nextSkillDesc = 0;
        if (skillLevel < SkillMaxLv)//렙업 가능 상황.
        {
            SkillTables.SkillLevelInfo nextSkillLevel = _LowDataMgr.GetSkillLevelData(actionLowData.idx, (byte)(skillLevel + 1));//다음
            nextSkillDesc = nextSkillLevel.skilldesc;//다은꺼는 이거말고 쓸게없음.
            
            {
                SkillLevelup.SelectSkillNum = arr;
                SkillLevelup.SelectSkillLevel = skillLevel;
                SkillLevelup.SkillId = actionLowData.idx;
            }
        }
        else//최대 렙임. 렙업 불가능.
        {
            nextSkillDesc = nowLevelLowData.skilldesc;
            //SkillLevelup.IsCanNotLevelup = true;
        }

        //스킬 정보 셋팅
        GameObject skillViewObj = GetViewObj(ViewType.Skill);
        if (skillViewObj == null)
            return;

        Transform skillInfoTf = skillViewObj.transform.FindChild("Info");
        UILabel skillName = skillInfoTf.FindChild("skill_name_d5").GetComponent<UILabel>();
        UILabel skillDesc = skillInfoTf.FindChild("description_d5").GetComponent<UILabel>();
        UILabel nowData   = skillInfoTf.FindChild("SkillData/Now/now").GetComponent<UILabel>();
        UILabel nextData  = skillInfoTf.FindChild("SkillData/Next/next").GetComponent<UILabel>();
        UILabel needLevel = skillInfoTf.FindChild("Need/Level/value_d5").GetComponent<UILabel>();
        UILabel needGold  = skillInfoTf.FindChild("Need/Gold/value_d5").GetComponent<UILabel>();

        skillName.text = _LowDataMgr.instance.GetStringSkillName(actionLowData.name);
        skillDesc.text = _LowDataMgr.instance.GetStringSkillName(actionLowData.descrpition);
        nowData.text   = _LowDataMgr.instance.GetStringSkillName(nowLevelLowData.skilldesc);
        nextData.text  = _LowDataMgr.instance.GetStringSkillName(nextSkillDesc);
        needLevel.text = string.Format("{0}", nowLevelLowData.LimitLv);//string.Format(_LowDataMgr.instance.GetStringCommon(453), 
		needGold.text = string.Format("{0}", SceneManager.instance.NumberToString(nowLevelLowData.CostGold));//nowLevelLowData.CostGold == 0 ? "0" : nowLevelLowData.CostGold.ToString("#,##") );
        
        Transform btnTf = skillViewObj.transform.FindChild(string.Format("Skills/BtnSkill_{0}", arr) );
        SelectSkillEff.parent = btnTf;
        SelectSkillEff.localPosition = Vector3.zero;
        SelectSkillEff.localScale = Vector3.one;
        
        Transform upEffTf = SkillUpEff.transform;
        upEffTf.parent = btnTf;
        upEffTf.localScale = Vector3.one;
        upEffTf.localPosition = Vector3.zero;
    }
    
    /// <summary> 스킬 강화버튼 </summary>
    void OnClickSkillLevelup(bool isMax)
    {
        int skillNumber = SkillLevelup.SelectSkillNum + 1;
        IsMaxSkillup = isMax;

        //Debug.Log("skill number " + skillNumber);
        if(skillNumber == 0)//최대레벨 
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 635);
            return;
        }

        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();
        NetworkClient.instance.SendPMsgCostumeSkillUpgradeC((int)costumeData._costumeIndex, skillNumber);
    }

    /// <summary> 서버에서 스킬업그레이드 응답. </summary>
    public void OnPMsgCostumeSkillUp(NetData._CostumeData costume, int skillNumber)
    {
        GameObject skillViewObj = GetViewObj(ViewType.Skill);

        int arr = skillNumber - 1;//SkillLevelup.SelectSkillNum;
        Transform skillViewTf = skillViewObj.transform.FindChild("Skills");
        UISprite icon = skillViewTf.FindChild(string.Format("BtnSkill_{0}/icon_d5", arr)).GetComponent<UISprite>();
        UILabel level = skillViewTf.FindChild(string.Format("BtnSkill_{0}/level_d5", arr)).GetComponent<UILabel>();
        UILabel name = skillViewTf.FindChild(string.Format("BtnSkill_{0}/name_d5", arr)).GetComponent<UILabel>();

        SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(SkillLevelup.SkillId);
        if(actionLowData == null)
        {
            Debug.LogError("not found ActionInfo error " + SkillLevelup.SkillId);
            return;
        }
        ushort skillLevel = costume._skillLevel[arr];
        icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
        level.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), skillLevel);
        name.text = _LowDataMgr.instance.GetStringSkillName(actionLowData.name);

        SkillUpEff.SetActive(false);
        SkillUpEff.SetActive(true);

        UIEventTrigger uiTri = skillViewTf.FindChild(string.Format("BtnSkill_{0}", arr)).GetComponent<UIEventTrigger>();
        EventDelegate.Set(uiTri.onClick, delegate () { SetSkillInfo(actionLowData, arr, skillLevel); });

        SetSkillInfo(actionLowData, arr, skillLevel);

        if (IsMaxSkillup)
        {
            SkillTables.SkillLevelInfo nowLevelLowData = _LowDataMgr.GetSkillLevelData(actionLowData.idx, (byte)skillLevel);//지금
            if (SkillMaxLv <= skillLevel
                || NetData.instance.GetAsset(AssetType.Gold) < nowLevelLowData.CostGold
                || NetData.instance.UserLevel < nowLevelLowData.LimitLv)
                IsMaxSkillup = false;
            else
                OnClickSkillLevelup(true);
        }

        SoundManager.instance.PlaySfxSound(eUISfx.UI_cos_skill_up, false);
    }

    /// <summary> 최대 레벨까지 스킬업 정지 </summary>
    public void OnStopMaxSKillup()
    {
        IsMaxSkillup = false;
    }

    #endregion 스킬뷰 끝

    #region 승급 뷰

    void CheckEvolve(NetData._CostumeData costumeData)
    {
        Transform evolveTf = GetViewObj(CurViewType).transform;

        if (costumeData == null || !costumeData._isOwn )//가능하지 않음.
        {
            int length = EvolInvenSlots.Length;
            for (int i = 0; i < length; i++)
            {
                EvolInvenSlots[i].EmptySlot();
            }

            TargetCostume[0].gameObject.SetActive(false);
            EvolCount.text = "";
            EvolveCost.text = string.Format("0 {0}", _LowDataMgr.instance.GetStringCommon(32) );
            length = EvolveStars.Length;
            for (int i = 0; i < length; i++)
            {
                EvolveStars[i].SetActive(false);
            }

            length = EvolMaterialAmount.Length;
            for (int i=0; i < length; i++)
            {
                EvolMaterialAmount[i].text = "";
            }

            BtnEvolve.collider.enabled = false;
            return;
        }

        BtnEvolve.collider.enabled = true;

        Enchant.EvolveInfo evolve = _LowDataMgr.instance.GetLowDataEvolve(costumeData.GetLowData().evolveId );
        EvolveMaterialData = new NetData.UpgradeMaterialData(evolve);
        InitEvolve(costumeData);
    }

    /// <summary> 재료아이템 및 코스튬 데이터 셋팅 사전 검사 필요함.</summary>
    void InitEvolve(NetData._CostumeData costumeData)
    {
        ushort minor = costumeData._MinorGrade;
        int starLength = EvolveStars.Length;
        for (int i = 0; i < starLength; i++)
        {
            EvolveStars[i].SetActive(i < minor);

            //if (i < minor)
            //    EvolveStars[i].SetActive(true);
            //else
            //    EvolveStars[i].SetActive(false);
        }
        
        //재료 셋팅
        bool isCanNotEvolve = false;
        int loopCount = EvolInvenSlots.Length;
        for (int i=0; i < loopCount; i++)
        {
            NetData.MaterialData md = EvolveMaterialData.MaterialList[i];
            int nowAmount = UserInfo.GetItemCountForItemId(md.ItemId, (byte)eItemType.USE);
            ushort amount = (ushort)(md.Amount + (minor*md.AddValue));
            
            if (nowAmount < amount && !isCanNotEvolve)
                isCanNotEvolve = true;

            int arr = i;
            EvolMaterialAmount[i].text = string.Format("{0} / {1}", nowAmount, amount);
            EvolInvenSlots[i].SetLowDataItemSlot(md.ItemId, 0, (lowDataId)=>{
                UIMgr.OpenDetailPopup(this, (uint)lowDataId);
                //UIMgr.OpenClickPopup((uint)lowDataId, EvolInvenSlots[arr].transform.position);
            });
        }
        
        ulong needPrice = EvolveMaterialData.Price + (EvolveMaterialData.AddPrice * costumeData._MinorGrade);
        EvolveCost.text = string.Format("{0:#,#} {1}", needPrice, _LowDataMgr.instance.GetStringCommon(32));

        NetData.ItemAbilityData ability = costumeData.AbilityData;
        string abilityText = null;
		
		float abilityValue = _LowDataMgr.instance.GetCostumeAbilityValue (costumeData._Grade, costumeData._MinorGrade, ability.Value);
		abilityText = string.Format("{0} {1}"
                , UIMgr.instance.GetAbilityLocName(ability.Ability)//이름
		        , UIMgr.instance.GetAbilityStrValue(ability.Ability, abilityValue));//지금 능력치
		
        TargetCostume[0].gameObject.SetActive(true);
        TargetCostume[0].FindChild("icon").GetComponent<UISprite>().spriteName = costumeData.GetIconName();
        TargetCostume[0].FindChild("name").GetComponent<UILabel>().text = costumeData.GetLocName();
        TargetCostume[0].FindChild("info").GetComponent<UILabel>().text = costumeData.GetDescription();
        TargetCostume[0].FindChild("ability").GetComponent<UILabel>().text = abilityText;
        EvolCount.text = string.Format("{0}", costumeData._Grade);
        //TargetCostumeAbility.text = abilityText;
    }

    /// <summary> 코스튬 승급 </summary>
    void OnClickEvolve()
    {
        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();
        NetworkClient.instance.SendPMsgCostumeEvolveC((int)costumeData._costumeIndex, (int)costumeData._costmeDataIndex);
    }

    /// <summary> 승급 응답 </summary>
    public void OnReceiveEvolve(NetData._CostumeData costumeData)
    {
        //BlockingObj.SetActive(true);

        GameObject go = UIHelper.CreateEffectInGame(EvoleEffRoot, "Fx_UI_cos_upgrade");

        int arr = costumeData._MinorGrade - 1;
        if (arr < 0)
            arr = EvolveStars.Length - 1;

        GameObject eff = UIHelper.CreateEffectInGame(EvolveStars[arr].transform.parent, "Fx_UI_star_up_01");
        Destroy(eff, 1.5f);
        Destroy(go, 1.5f);
        SoundManager.instance.PlaySfxSound(eUISfx.UI_cos_upgrade, false);

        //TempCoroutine.instance.FrameDelay(1.5f, delegate () {
            //EvolveEff.SetActive(false);
            
            //Destroy(eff, 0);
            //BlockingObj.SetActive(false);

            //Enchant.EvolveInfo evolve = _LowDataMgr.instance.GetLowDataEvolve(costumeData.GetLowData().evolveId, costumeData._Grade, false);
            //EvolveMaterialData = new NetData.UpgradeMaterialData(evolve);
            //InitEvolve(costumeData);
        //});

        Enchant.EvolveInfo evolve = _LowDataMgr.instance.GetLowDataEvolve(costumeData.GetLowData().evolveId );
        EvolveMaterialData = new NetData.UpgradeMaterialData(evolve);
        InitEvolve(costumeData);
		CostumeListView();

    }

    #endregion 승급뷰 끝
    
    #region 보석 삽입

    void InitSocket()
    {
        Transform socketTf = GetViewObj(CurViewType).transform;
        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();
        NetData.ItemAbilityData ability = costumeData.AbilityData;
        
		float abilityValue = _LowDataMgr.instance.GetCostumeAbilityValue (costumeData._Grade, costumeData._MinorGrade, ability.Value);

        TargetCostume[1].gameObject.SetActive(true);
        TargetCostume[1].FindChild("icon").GetComponent<UISprite>().spriteName = costumeData.GetIconName();
        TargetCostume[1].FindChild("name").GetComponent<UILabel>().text = costumeData.GetLocName();
        TargetCostume[1].FindChild("info").GetComponent<UILabel>().text = costumeData.GetDescription();
        TargetCostume[1].FindChild("ability").GetComponent<UILabel>().text = string.Format("{0} {1}", UIMgr.instance.GetAbilityLocName(ability.Ability)
		                                                                                   , UIMgr.instance.GetAbilityStrValue(ability.Ability, abilityValue));//지금 능력치
        
        //if(DetailPopup == null)
        //    DetailPopup = UIMgr.OpenDetailPopup(this);

        //코스튬에 보석 공간 확인
        for (ushort i = 0; i < SystemDefine.MaxJewelCount; i++)
        {
            string itemName = null;
            string itemAbility = null;
            string itemDesc = null;
            UIEventTrigger uiTri = SocketRootTfs[i].GetComponent<UIEventTrigger>();

            uint jewelLowDataId = costumeData._EquipJewelLowId[i];
            if(0 < jewelLowDataId)
            {
                Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(jewelLowDataId);
                //Item.ItemValueInfo valueLowData = _LowDataMgr.instance.GetLowDataItemValueInfo(useLowData.OptionIndex1);
                //LockImg.SetActive(false);

                itemDesc = _LowDataMgr.instance.GetStringItem(useLowData.DescriptionId);
                JewelInvenSlots[i].SetLowDataItemSlot(jewelLowDataId, 0, null);
                itemName = _LowDataMgr.instance.GetStringItem(useLowData.NameId);
                itemAbility = string.Format("{0} {1}", UIMgr.instance.GetAbilityLocName((AbilityType)useLowData.OptionType),
                    UIMgr.instance.GetAbilityStrValue((AbilityType)useLowData.OptionType, useLowData.value * 0.1f));
            }
            else//그런거 없음 빈 슬롯.
            {
                JewelInvenSlots[i].EmptySlot();
                //LockImg.SetActive(true);
            }
            UILabel nameLbl = SocketRootTfs[i].FindChild("item_name").GetComponent<UILabel>();
            UILabel abilityLbl = SocketRootTfs[i].FindChild("ability").GetComponent<UILabel>();
            UILabel descLbl = SocketRootTfs[i].FindChild("desc").GetComponent<UILabel>();

            //승급에따라 슬롯수가 결정됨, 보석슬롯 락처리
            int maxJewelSlot = costumeData.MaxJewelSlot;
            GameObject LockImg = SocketRootTfs[i].FindChild("Lock").gameObject; // 락이미지
            LockImg.SetActive(maxJewelSlot <= i);



            nameLbl.text = itemName;
            abilityLbl.text = itemAbility;
            descLbl.text = itemDesc;
        }

        if (JewelGrid.transform.childCount <= 0)//처음으로 클릭함. 슬롯 생성 및 갱신해준다.
        {
            int maxInvenSlot = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.InvenMax);
            List<NetData._ItemData> jewelList = UserInfo.GetTypeItemList(eItemType.USE, AssetType.Jewel);
            jewelList.Sort(SortJewel);
            for (int i = 0; i < maxInvenSlot; i++)
            {
                GameObject slotGo = Instantiate(InvenSlotPrefab) as GameObject;
                Transform slotTf = slotGo.transform;
                slotTf.parent = JewelGrid.transform;
                slotTf.localPosition = Vector3.zero;
                slotTf.localScale = Vector3.one;

                InvenItemSlotObject invenSlot = slotGo.GetComponent<InvenItemSlotObject>();
                if (i < jewelList.Count)
                {
                    invenSlot.SetInvenItemSlot(jewelList[i], OnClickInvenItemSlot);
                    //SocketRootTfs[i].FindChild("Lock").gameObject.SetActive(false);
                }
                else
                {
                    invenSlot.EmptySlot();

                }
            }

            JewelScrollView.ResetPosition();
        }

        //선택되어 있는 소켓 기본 0번으로 셋팅
        SelectJewelArr = 0;
        if(costumeData.MaxJewelSlot>0)
        {
            for (int i = 0; i < SocketRootTfs.Length; i++)
            {
                GameObject select = SocketRootTfs[i].transform.FindChild("cover").gameObject;
                select.SetActive(i == SelectJewelArr);
            }
        }
       
    }

    /// <summary> 보석 슬롯 클릭시. 보유한 보석 보여준다. </summary>
    void OnClickJewelSlot(int slotArr)
    {
        SelectJewelArr = slotArr;

        for(int i=0;i< SocketRootTfs.Length;i++)
        {
            GameObject select = SocketRootTfs[i].transform.FindChild("cover").gameObject;
            select.SetActive(i == slotArr);
        }
    }

    /// <summary> 아이템 클릭시 실행. </summary>
    void OnClickInvenItemSlot(ulong itemIdx, byte itemType)
    {
        NetData._ItemData itemData = UserInfo.GetItemDataForIndexAndType(itemIdx, itemType);
        //DetailPopup.SetDetailPopup(itemData);
        UIMgr.OpenDetailPopup(this, itemData);
    }

    /// <summary> 아이템상세팝업에서 보석 삽입 버튼 클릭 했으면 실행. </summary>
    public void OnInsertJewel(NetData._ItemData itemData)
    {
        NetData._CostumeData costumeData = UserInfo.GetEquipCostume();

        NetworkClient.instance.SendPMsgCostumeTokenC((int)costumeData._costumeIndex, 1, (int)itemData._itemIndex, SelectJewelArr+1);
    }
    
    /// <summary> 서버에서 보석박는거 응답 </summary>
    public void OnReceiveJewel(NetData._CostumeData costumeData)
    {
        //DetailPopup.OnClose();
        //DetailPopup.Hide();

        //코스튬에 보석 셋팅
        string itemName = null;
        string itemAbility = null;
        string itemDesc = null;

        uint jewelLowDataId = costumeData._EquipJewelLowId[SelectJewelArr];
        if (0 < jewelLowDataId)
        {
            Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(jewelLowDataId);

            itemDesc = _LowDataMgr.instance.GetStringItem(useLowData.DescriptionId);
            JewelInvenSlots[SelectJewelArr].SetLowDataItemSlot(jewelLowDataId, 0, null);
            itemName = _LowDataMgr.instance.GetStringItem(useLowData.NameId);
            itemAbility = string.Format("{0} {1}", UIMgr.instance.GetAbilityLocName((AbilityType)useLowData.OptionType),
                UIMgr.instance.GetAbilityStrValue((AbilityType)useLowData.OptionType, useLowData.value * 0.1f));
        }
        else//그런거 없음 빈 슬롯. 뭔가 문제가 있는 것임.
        {
            Debug.LogError(string.Format("not found insert jewel item error {0} ", SelectJewelArr));
            JewelInvenSlots[SelectJewelArr].EmptySlot();
        }

        UILabel nameLbl = SocketRootTfs[SelectJewelArr].FindChild("item_name").GetComponent<UILabel>();
        UILabel abilityLbl = SocketRootTfs[SelectJewelArr].FindChild("ability").GetComponent<UILabel>();
        UILabel descLbl = SocketRootTfs[SelectJewelArr].FindChild("desc").GetComponent<UILabel>();

        nameLbl.text = itemName;
        abilityLbl.text = itemAbility;
        descLbl.text = itemDesc;

        //아이템 재셋팅
        Transform slotParent = JewelGrid.transform;
        List<NetData._ItemData> jewelList = UserInfo.GetTypeItemList(eItemType.USE, AssetType.Jewel);
        jewelList.Sort(SortJewel);
        int childCount = slotParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            InvenItemSlotObject invenSlot = slotParent.GetChild(i).GetComponent<InvenItemSlotObject>();
            if (i < jewelList.Count)
                invenSlot.SetInvenItemSlot(jewelList[i], OnClickInvenItemSlot);
            else
                invenSlot.EmptySlot();
        }

        SoundManager.instance.PlaySfxSound(eUISfx.UI_jewel_equip, false);
    }

    int SortJewel(NetData._ItemData a, NetData._ItemData b)
    {
        if (!a.IsUseItem() || !b.IsUseItem())
            return 0;

        Item.ItemInfo aLowData = a.GetUseLowData();
        Item.ItemInfo bLowData = b.GetUseLowData();
        AssetType aType = (AssetType)aLowData.Type;
        AssetType bType = (AssetType)bLowData.Type;

        //보석은 별도
        if (aType == AssetType.Jewel && bType == AssetType.Jewel)
        {
            if (aLowData.Grade < bLowData.Grade)
                return 1;
            else if (bLowData.Grade < aLowData.Grade)
                return -1;
        }
        else if (aType != AssetType.Jewel && bType == AssetType.Jewel)
            return 1;
        else if (aType == AssetType.Jewel && bType != AssetType.Jewel)
            return -1;

        if (a._itemIndex < b._itemIndex)
            return -1;
        else
            return 1;
    }

    #endregion 보석삽입 끝

    #region 공통 버튼 이벤트
    /// <summary> 탭버튼 클릭 View를 ViewType값으로 바꿔준다 </summary>
    void OnClickTab(int viewType)
    {
        //ChangeView((ViewType)viewType);
    }

    public override void Close()
    {

        if (BasePanel != null) //다른패널에서 호출됬을경우
        {
            TempCoroutine.instance.StopAllCoroutines();
            base.Close();
            string[] route = BasePanel.Split(' ');
            UIMgr.Open(route[0].ToString(), true);
            return;
        }

        //if (CurViewType == ViewType.Socket)
        //{
        //    if (DetailPopup != null && DetailPopup.gameObject.activeSelf)
        //    {
        //        DetailPopup.OnClose();
        //        return;
        //    }
        //}
        //else if(CurViewType == ViewType.Evolve)
        //{
        //    if (BlockingObj.activeSelf)
        //        return;
        //}

        if (IsChangeCostume)
        {
            TownState town = SceneManager.instance.GetState<TownState>();
            town.MyHero.SetChangeSkin(true);
        }

        TempCoroutine.instance.StopAllCoroutines();
        base.Close();
        UIMgr.OpenTown();
    }

    #endregion
}

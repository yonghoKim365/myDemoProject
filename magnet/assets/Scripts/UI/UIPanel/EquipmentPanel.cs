using UnityEngine;
using System.Collections.Generic;

public class EquipmentPanel : UIBasePanel {
    enum TabType { Info, Set, Upgrade, Status, SkillUpgrade, Costume }
    enum AttributeLbl { Class, Level, Guild, ID, Status, BattlePoint, Hp, Armor, Exp }

    [Header("0.Default Link")]
    public GameObject CharInfoObj;//캐릭터 모델 뷰
    public GameObject AlramObj;
    public GameObject[] UpgradeObj;//0 강화, 1 승급
    public GameObject[] Objs;//0 속성 정보, 1 장비셋트, 2 강화

    public UITabGroup TabGroup;//Tab버튼 관리하는 클래스

    [Header("0_1.Char Equip Link")]
    public Transform EquipPartsTf;//장착중인 아이템들의 어미 자식들 찾아서 셋팅하려는 의도 ITEM_PARTS_TYPE를 이용하면 됨.
    [Header("0_1.Char CostumeToggle Link")]
    public UISprite[] ToggleObjs;//0 on, 1 off
    
    [Header("1.Attribute Label Link")]
    public UILabel CharAttack;//전투력
    public UILabel[] AbilityList;//속성에서 보여줄 플레이어 속성 정보들. AbilityType-1값으로 배열함.
    public UILabel[] AttributeDefaultList;//속성에서 보여줄 플레이어 기본 정보들 0클래스, 1레벨, 2길드, 3아이디, 4신분, 5 HP, 6 EXP

    [Header("1.Attribute Sprite Link")]
    public UISlider ExpBar;//속성에서 보여주는 Exp진행 막대기
    public UISlider HpBar;//속성에서 보여주는 Hp 막대기
    public UISlider ArmorBar;//속성에서 보여주는 슈퍼아머 막대기

    [Header("2.Enchant GameObject Link")]
    public GameObject[] EnTypeObj;//0 Default, 1 Full

    [Header("2.Enchant Transform Link")]
    public Transform[] BtnEnchantTf;//강화 일회, 풀

    [Header("2.Enchant Label Link")]
    public UILabel[] EnTargetName;//인첸트 대상 아이템 이름 0 Default, 1 Full
    public UILabel[] EnMaterialAmount;//강화용 재료 개수 표기
    public UILabel[] EnCurAbility;//강화용 현재 능력치 표기, 0~3 일회용, 4~7 풀강용
    public UILabel[] EnNextAbility;//강화용 현재 능력치 표기, 0~3 일회용, 4~7 풀강용
    public UILabel[] EnAddAbility;//강화용 현재 능력치에 추가되는 값 표기, 0~3 일회용, 4~7 풀강용
    public UILabel[] EnFullAbility;//강화용 최대 수치일 경우

    [Header("3.EquipmentSet Transform Link")]
    public Transform EquipSetGridTf;
    //public Transform EffSelectSlotTf;//슬롯선택 이미지
    public Transform SetListPopTf;//
    
    [Header("4.Evolve Transform Link")]
    public Transform BtnEvolveTf;

    [Header("4.Evolve Label Link")]
    public UILabel[] EvTargetName;//0~1 Default, 2 Full
    public UILabel[] EvMaterialAmount;//0~1 Default, 2~3 Full
    public UILabel[] EvCurAbility;
    public UILabel[] EvNextAbility;
    public UILabel[] EvAddAbility;

    [Header("5.Status Transform Link")]
    public Transform EquipStatusTf;//내가 장착중인 거 표기용
    public Transform ListGridTf;

    [Header("6.Skill Transform Link")]
    public Transform SkillGridTf;
    public Transform PassiveGridTf;
    public Transform[] BtnUpgradeTf;//0 한번, 1 최대

    [Header("6.Skill Label Link")]
    public UILabel SkillName;
    public UILabel OnceAmount;
    public UILabel[] SkillDesc;//스킬 설명들 0 현재스킬 설명, 1 강화후 설명, 2최대 강화후 설명
    public UILabel[] FullAmounts;

    [Header("6.Skill Sprite Link")]
    public UISprite SelectIcon;

    private GameObject EffectGo;

    private NetData._ItemData UpgradeItem = null;
    private NetData._UserInfo CharInven;//한 캐릭터가 들고있는 인벤토리 정보
    private InvenItemSlotObject[] EnchantTargetSlot = new InvenItemSlotObject[2];
    private InvenItemSlotObject[] EnchantMaterial = new InvenItemSlotObject[2];
    private InvenItemSlotObject[] EvolveTargetSlot = new InvenItemSlotObject[3];//0~1 Default, 2 Full
    private InvenItemSlotObject[] EvolveMaterial = new InvenItemSlotObject[2];//0~1 Defula, 2~3 Full
    private InvenItemSlotObject[] SkillMaterial = new InvenItemSlotObject[5];//0 Defula, 1~5 Full
    private List<NetData.StatusData> StatusList;

    private TabType CurTabType;
    private bool IsUpgrading;//업그래이드 중
    private bool IsLinkTab;
    private uint MaxEnchantCount;

    private UIBasePanel reOpenPanel;

    public override void Init()
    {
        base.Init();

        CharInven = NetData.instance.GetUserInfo();

        EventDelegate.Set(ToggleObjs[0].transform.GetComponent<UIEventTrigger>().onClick, () => {//보이기
            if (CharInven.isHideCostum)
                NetworkClient.instance.SendPMsgCostumeShowFlagC(false);
        });

        EventDelegate.Set(ToggleObjs[1].transform.GetComponent<UIEventTrigger>().onClick, () => {//감추기
            if (!CharInven.isHideCostum)
                NetworkClient.instance.SendPMsgCostumeShowFlagC(true);
        });

        EventDelegate.Set(transform.FindChild("InfoView/BtnTitle").GetComponent<UIEventTrigger>().onClick, delegate () {//칭호 팝업
            UIMgr.OpenSelectTitlePopup();
        });

        EventDelegate.Set(BtnEnchantTf[0].GetComponent<UIButton>().onClick, () => {//1회 강화
            OnClickEnchant(false);
        });
        EventDelegate.Set(BtnEnchantTf[1].GetComponent<UIButton>().onClick, () => {//최대 강화
            OnClickEnchant(true);
        });

        EventDelegate.Set(BtnEvolveTf.GetComponent<UIButton>().onClick, OnClickEvolve);//승급
        EventDelegate.Set(SetListPopTf.FindChild("Btn").GetComponent<UIEventTrigger>().onClick, () => {

            int[] openSlot = new int[] {
                _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen01CharLevel),
                _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen02CharLevel),
                _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen03CharLevel),
                _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen04CharLevel)
            };

            if (CharInven._Level < openSlot[CharInven.GetOwnSetItemData().Count - 1])//레벨 부족
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(945), openSlot[CharInven.GetOwnSetItemData().Count-1]) );
                return;
            }

            List<uint> selIdList = new List<uint>();
            Transform gridTf = SetListPopTf.FindChild("ScrollView/Grid");
            for(int i=0; i < gridTf.childCount; i++)
            {
                if ( !gridTf.GetChild(i).gameObject.activeSelf || !gridTf.GetChild(i).FindChild("on").gameObject.activeSelf)
                    continue;

                uint id = uint.Parse(gridTf.GetChild(i).name);
                selIdList.Add(id);
            }

            if(1 < selIdList.Count)
            {
                if (CharInven._Level < openSlot[selIdList.Count-1])//레벨 부족
                {
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(945), openSlot[selIdList.Count-1]) );
                    return;
                }
                
                for (int i=0; i < selIdList.Count; i++) {
                    NetworkClient.instance.SendPMsgEquipmentSetSelectC(selIdList[i] );
                }
            }
            else if(0 < selIdList.Count)
                NetworkClient.instance.SendPMsgEquipmentSetSelectC(selIdList[0] );
            else//아이템이 선택되어 있지않습니다.
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 9925);
            }

        });//선택 장비들 추가

        SkillMaterial[0] = UIHelper.CreateInvenSlot(OnceAmount.transform.parent).GetComponent<InvenItemSlotObject>();
        for (int i = 0; i < FullAmounts.Length; i++)
        {
            SkillMaterial[i + 1] = UIHelper.CreateInvenSlot(FullAmounts[i].transform.parent).GetComponent<InvenItemSlotObject>();
        }
        
        SetListPopTf.gameObject.SetActive(false);
        TabGroup.Initialize(OnClickTab);
    }

    #region LateInit()
    public override void LateInit()
    {
        base.LateInit();

        if (StatusList == null)
        {
            NetworkClient.instance.SendPMsgRoleIdentifyListC();
            NetworkClient.instance.SendPMsgRoleIdentifyUnlockedListC();
        }

        if (parameters.Length > 0)
            reOpenPanel = (UIBasePanel)parameters[0];
        
        if(mStarted)//최초 호출 후
        {
            TabGroup.CoercionTab(0);
        }
        else//Init 같은 역할 최초 생성일때문 호출될 것이다.
        {
            UIAtlas uiAtlas = AtlasMgr.instance.GetEquipAtlasForCharIdx(CharInven._userCharIndex);
            ///장착중인 아이템 아이콘 셋팅
            for (int i=0; i < EquipPartsTf.childCount; i++)
            {
                Transform tf = EquipPartsTf.GetChild(i);
                if (tf == null)
                    continue;
                
                tf.FindChild("icon").GetComponent<UISprite>().atlas = uiAtlas;
            }
            
            EnchantTargetSlot[0] = UIHelper.CreateInvenSlot(EnTypeObj[0].transform.FindChild("TargetItem")).GetComponent<InvenItemSlotObject>();
            EnchantTargetSlot[1] = UIHelper.CreateInvenSlot(EnTypeObj[1].transform.FindChild("TargetItem")).GetComponent<InvenItemSlotObject>();

            EnchantMaterial[0] = UIHelper.CreateInvenSlot(UpgradeObj[0].transform.FindChild("Default/Material/Item01")).GetComponent<InvenItemSlotObject>();
            EnchantMaterial[1] = UIHelper.CreateInvenSlot(UpgradeObj[0].transform.FindChild("Default/Material/Item02")).GetComponent<InvenItemSlotObject>();

            EvolveTargetSlot[0] = UIHelper.CreateInvenSlot(UpgradeObj[1].transform.FindChild("CurItem")).GetComponent<InvenItemSlotObject>();
            EvolveTargetSlot[1] = UIHelper.CreateInvenSlot(UpgradeObj[1].transform.FindChild("NextItem")).GetComponent<InvenItemSlotObject>();

            EvolveMaterial[0] = UIHelper.CreateInvenSlot(UpgradeObj[1].transform.FindChild("Material/Item01")).GetComponent<InvenItemSlotObject>();
            EvolveMaterial[1] = UIHelper.CreateInvenSlot(UpgradeObj[1].transform.FindChild("Material/Item02")).GetComponent<InvenItemSlotObject>();
        }

        OnSubTutorial();
        CreateCharModel(false);//모델링 변경이 있을 수 있으니 다시 호출
        SetCostumeState();


    }
    #endregion

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    #region 넷트워크 응답 함수
    public override void NetworkData(params object[] proto)
    {
        base.NetworkData(proto);
        switch( (Sw.MSG_DEFINE)proto[0]) {
            #region 코스튬
            case Sw.MSG_DEFINE._MSG_COSTUME_SHOW_FLAG_C://코스튬 감추기, 보이기
                CharInven.isHideCostum = (bool)proto[1];

                SetCostumeState();
                CreateCharModel(true);
                break;
            #endregion
            #region 장비 강화, 승급
            case Sw.MSG_DEFINE._MSG_EQUIPMENT_EVOLVE_C://승급
            case Sw.MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_C://장비아이템 강화
            case Sw.MSG_DEFINE._MSG_EQUIPMENT_ENCHANT_TURBO_C://장비 아이템 연속강화
                SceneManager.instance.SetNoticePanel(NoticeType.PowerUp);
                NetData._ItemData itemData = (NetData._ItemData)proto[1];
                Item.EquipmentInfo equipLow = itemData.GetEquipLowData();
                if (itemData._enchant < equipLow.MaxEnchant || equipLow.NextPartsId <= 0 )//아직더 강화가 가능하다.
                {
                    IsUpgrading = true;
                    EffectGo = UIHelper.CreateEffectInGame(EnchantTargetSlot[0].transform.parent, "Fx_UI_Item_enchant_01");
                    TempCoroutine.instance.KeyDelay("EquipEff", 0.5f, () => {
                        IsUpgrading = false;
                        SetUpgrade(true, itemData);
                    });
                }
                else if(0 < equipLow.NextPartsId )//강화 다함 승급으로 넘어감
                {
                    IsUpgrading = true;
                    EffectGo = UIHelper.CreateEffectInGame(EvolveTargetSlot[0].transform.parent, "Fx_UI_Item_enchant_01");
                    TempCoroutine.instance.KeyDelay("EquipEff", 0.5f, () => {
                        IsUpgrading = false;
                        SetUpgrade(false, itemData);
                    });
                }

                CharAttack.text = string.Format("{0}", CharInven.RefreshTotalAttackPoint(true) );
                ///장착중인 아이템 아이콘 셋팅
                Transform tf = EquipPartsTf.GetChild((int)itemData.EquipPartType-1);
                if (tf != null)
                {
                    UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
                    UISprite icon = tf.FindChild("icon").GetComponent<UISprite>();
                    UISprite grade = tf.FindChild("grade").GetComponent<UISprite>();
                    UILabel Enchant = tf.FindChild("upgrade_lv").GetComponent<UILabel>();
                    UISprite bg = tf.GetComponent<UISprite>();
                    if (itemData == null)//미장착
                    {
                        icon.enabled = false;
                        grade.enabled = false;
                        bg.spriteName = UIHelper.GetDefaultEquipIcon(itemData.EquipPartType);
                        Enchant.gameObject.SetActive(false);
                    }
                    else//장착
                    {
                        icon.enabled = true;
                        grade.enabled = true;
                        
                        icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(equipLow.Icon);
                        grade.spriteName = string.Format("Icon_0{0}", equipLow.Grade);
                        bg.spriteName = string.Format("Icon_bg_0{0}", equipLow.Grade);
                        Enchant.text = itemData._enchant <= 0 ? "" : string.Format("+{0}", itemData._enchant);
                        Enchant.gameObject.SetActive(true);

                    }
                }
                break;
            #endregion
            #region 장비 셋트
            case Sw.MSG_DEFINE._MSG_EQUIPMENT_SET_CHANGE_C://셋트장비 교체
                if (CurTabType == TabType.Set)
                {
                    uint changeId = (uint)proto[1];
                    uint prevId = (uint)proto[2];
                    Transform prefTf = EquipSetGridTf.FindChild(prevId.ToString());
                    Transform mountTf = EquipSetGridTf.FindChild(changeId.ToString());
                    if (prefTf != null)
                    {
                        prefTf.FindChild("Open/Mark").gameObject.SetActive(false);
                        prefTf.FindChild("Open/Btn/Btn_on/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(38);
                        prefTf.FindChild("Open/Btn/Btn_off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(38);
                        prefTf.FindChild("Open/Btn/Btn_on").gameObject.SetActive(true);
                        prefTf.FindChild("Open/Btn/Btn_off").gameObject.SetActive(false);
                        prefTf.FindChild("Open/Btn").collider.enabled = true;
                    }

                    if (mountTf != null)
                    {
                        mountTf.FindChild("Open/Mark").gameObject.SetActive(true);
                        mountTf.FindChild("Open/Btn/Btn_on/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(37);
                        mountTf.FindChild("Open/Btn/Btn_off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(37);
                        mountTf.FindChild("Open/Btn/Btn_on").gameObject.SetActive(false);
                        mountTf.FindChild("Open/Btn/Btn_off").gameObject.SetActive(true);
                        mountTf.FindChild("Open/Btn").collider.enabled = false;
                    }
                }
                else
                {
                    if(CurTabType == TabType.Upgrade)
                    {
                        UpgradeItem = null;
                        OnClickTab((int)TabType.Upgrade);
                    }

                    SetCharView();
                }

                SceneManager.instance.SetNoticePanel(NoticeType.PowerUp);
                TabAttribute();
                CreateCharModel(true);
                break;

            case Sw.MSG_DEFINE._MSG_EQUIPMENT_SET_SELECT_C://셋트장비 추가.
                SetEquipList(false);
                break;
            #endregion
            #region 신분
            case Sw.MSG_DEFINE._MSG_ROLE_IDENTIFY_LIST_S://신분 리스트 받아오기
                if (StatusList == null || StatusList.Count == 0)
                    StatusList = (List<NetData.StatusData>)proto[1];
                else
                {
                    StatusList.AddRange((List<NetData.StatusData>)proto[1]);

                    if (CurTabType == TabType.Status)
                        SetStatus();
                }

                for (int i = 0; i < StatusList.Count; i++)
                {
                    if (!StatusList[i].IsOwn || !StatusList[i].IsEquip)
                        continue;

                    Character.StatusInfo info = _LowDataMgr.instance.GetLowDataCharStatus(StatusList[i].LowDataId);
                    AttributeDefaultList[(uint)AttributeLbl.Status].text = _LowDataMgr.instance.GetStringCommon(info.StatusNameId);
                    break;
                }
                break;

            case Sw.MSG_DEFINE._MSG_ROLE_IDENTIFY_USE_S://신분 장착
                int id = (int)proto[1];
                uint equipLowId =0;
                for(int i=0; i < StatusList.Count; i++)
                {
                    if (!StatusList[i].Id.Equals(id))
                    {
                        if (StatusList[i].IsEquip)
                            StatusList[i].IsEquip = false;

                        continue;
                    }

                    equipLowId = StatusList[i].LowDataId;
                    StatusList[i].IsEquip = true;
                }

                //이전 장착중이던거 장착하기로 변경
                uint prevLowId = uint.Parse(EquipStatusTf.name);
                ListGridTf.FindChild(prevLowId.ToString()).FindChild("BtnEquip/on").gameObject.SetActive(true);
                ListGridTf.FindChild(prevLowId.ToString()).FindChild("BtnEquip/off").gameObject.SetActive(false);

                //나의 정보
                Character.StatusInfo lowData = _LowDataMgr.instance.GetLowDataCharStatus(equipLowId);
                EquipStatusTf.name = equipLowId.ToString();
                EquipStatusTf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} ({1} {2})"
                    , _LowDataMgr.instance.GetStringCommon(lowData.StatusNameId), _LowDataMgr.instance.GetStringCommon(14), lowData.Level);
                EquipStatusTf.FindChild("desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringAchievement(lowData.GoalDescriptionId);
                EquipStatusTf.FindChild("mark").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(lowData.Icon);
        
                //이전 장착하기 던거 장착중으로 변경
                ListGridTf.FindChild(equipLowId.ToString()).FindChild("BtnEquip/on").gameObject.SetActive(false);
                ListGridTf.FindChild(equipLowId.ToString()).FindChild("BtnEquip/off").gameObject.SetActive(true);
                
                AttributeDefaultList[(uint)AttributeLbl.Status].text = _LowDataMgr.instance.GetStringCommon(lowData.StatusNameId);
                break;

            case Sw.MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCK_S://신분 추가
                //int lowDataId = (int)proto[1];
                StatusList.Clear();
                NetworkClient.instance.SendPMsgRoleIdentifyListC();
                NetworkClient.instance.SendPMsgRoleIdentifyUnlockedListC();
                break;

            case Sw.MSG_DEFINE._MSG_ROLE_IDENTIFY_UNLOCKED_LIST_C://미해금 신분 정보 요청
                if (StatusList == null || StatusList.Count == 0)
                    StatusList = (List<NetData.StatusData>)proto[1];
                else
                {
                    StatusList.AddRange((List<NetData.StatusData>)proto[1]);

                    if (CurTabType == TabType.Status)
                        SetStatus();
                }
                break;

            case Sw.MSG_DEFINE._MSG_ROLE_IDENTIFY_UPGRADE_S://신분 레벨업
                int lvId = (int)proto[1];
                StatusList.Clear();
                NetworkClient.instance.SendPMsgRoleIdentifyListC();
                NetworkClient.instance.SendPMsgRoleIdentifyUnlockedListC();
                break;
            #endregion
            #region 스킬
            case Sw.MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_S://레벨
            case Sw.MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_UPGRADE_TURBO_S:
                int skillType = (int)proto[1];
                int skillNum = (int)proto[2];
                NetData.SkillSetData skillSetData = CharInven.GetSkillSetData((uint)skillType);
                SkillGridTf.FindChild(string.Format("{0}/{1}/lv", skillType, skillNum-1)).GetComponent<UILabel>().text =
                    string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), skillSetData.SkillLevel[skillNum-1]);

                OnClickSkillInfo((int)skillSetData.SkillLevel[skillNum-1], skillSetData.SkillSetId, skillSetData.SkillId[skillNum-1], skillNum);
                break;
                
            case Sw.MSG_DEFINE._MSG_ROLE_ACTIVE_SKILL_USE_S://장착
                CharInven.GetEquipSKillSet().IsEquip = false;
                NetData.SkillSetData data = CharInven.GetSkillSetData( (uint)proto[1]);
                data.IsEquip = true;

                SetSkill(false);
                break;

            case Sw.MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_USE_S://패시브 장착
                CharInven.GetEquipPassive().IsEquip = false;
                NetData.PassiveData passData = CharInven.GetPassiveData((int)proto[1]);
                passData.IsEquip = true;

                SetSkill(false);
                break;
            case Sw.MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_S://패시브 레벨업
            case Sw.MSG_DEFINE._MSG_ROLE_PASSIVE_SKILL_UPGRADE_TURBO_S:
                int passId = (int)proto[1];
                NetData.PassiveData passiveData = CharInven.GetPassiveData(passId);
                PassiveGridTf.FindChild(string.Format("{0}/lv", passId)).GetComponent<UILabel>().text =
                    string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), passiveData.Level);

                OnClickSkillInfo((int)passiveData.Level, 0, (uint)passId, -1);
                break;
                #endregion
        }
    }
    #endregion
    
    #region 탭
    void OnClickTab(int arr)
    {
        for (int i = 0; i < Objs.Length; i++)
        {
            Objs[i].SetActive(i == arr);
        }

        if (arr != 2)//강화탭이 아니라면
            UpgradeItem = null;

        //알람상태 체크
        if((TabType)arr == TabType.Set)
        {
            if (AlramObj.activeSelf)
                AlramObj.SetActive(false);//알람 끔
        }
        else
            AlramObj.SetActive(SceneManager.instance.IsAlram(AlramIconType.CHAR));

        uint title = 0;
        switch ((TabType)arr)
        {
            case TabType.Info://속성
                TabAttribute();
                SetCharView();
                title = 1162;
                break;
            case TabType.Set://장비 셋트 뷰
                Objs[0].SetActive(true);//속성뷰 열어줘야함.
                CharInfoObj.SetActive(false);
                TabAttribute();
                SetEquipList(false);

                title = 9906;
                break;
            case TabType.Upgrade://장비 강화
                
                if (UpgradeItem == null ) {
                    bool isEnchant = false, isCheck = false;
                    NetData._ItemData target = null;
                    for (ePartType i = ePartType.HELMET; i < ePartType.PART_MAX; i++)
                    {
                        NetData._ItemData item = CharInven.GetEquipParts(i);
                        if (item == null)
                            continue;

                        if (!isCheck) {
                            if (item._enchant < item.GetEquipLowData().MaxEnchant) {//아직 인첸 가능한지
                                target = item;
                                isEnchant = true;
                                break;
                            }

                            if (!isCheck && i == ePartType.PART_MAX - 1){//풀이네 다음.
                                isCheck = true;
                                i = 0;
                            }
                        }
                        else if (0 < item.GetEquipLowData().NextPartsId ) {//아직 승급가능한지
                            target = item;
                            break;
                        }
                    }

                    if(target == null) {
                        for (ePartType i = ePartType.HELMET; i < ePartType.PART_MAX; i++) {
                            target = CharInven.GetEquipParts(i);
                            if (target == null)
                                continue;

                            break;
                        }

                        isEnchant = true;
                    }

                    SetUpgrade(isEnchant, target);
                }

                SetCharView();
                title = 9907;
                break;

            case TabType.Status://스킬
                title = 9908;
                CharInfoObj.SetActive(false);
                SetStatus();
                
                break;

            case TabType.SkillUpgrade://스킬 강화
                title = 9909;
                CharInfoObj.SetActive(false);
                SetSkill( !IsLinkTab);
                IsLinkTab = false;
                break;

            case TabType.Costume://코스튬
                title = 6;
                break;
        }

        //이전 상태 전검
        if (CurTabType == TabType.Upgrade)
        {
            if (EffectGo != null)
            {
                IsUpgrading = false;
                Destroy(EffectGo);
            }

            if (SceneManager.instance.CurTutorial == TutorialType.ENCHANT)
            {
                uiMgr.TopMenu.EndTutorial();
            }
        }

        CurTabType = (TabType)arr;
        uiMgr.SetTopMenuTitleName(title);
    }
    #endregion

    #region 정보 부분 셋팅
    /// <summary> 정보 부분 셋팅 </summary>
    void TabAttribute()
    {
        Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(CharInven.GetCharIdx());

        AttributeDefaultList[(uint)AttributeLbl.Class].text = _LowDataMgr.instance.GetStringUnit(charLowData.NameId);
        AttributeDefaultList[(uint)AttributeLbl.Level].text = CharInven._Level.ToString();
        AttributeDefaultList[(uint)AttributeLbl.Guild].text = CharInven._GuildId == 0 ? _LowDataMgr.instance.GetStringCommon(133) : CharInven._GuildId.ToString();// 없음 강제로 넣어줌
        AttributeDefaultList[(uint)AttributeLbl.ID].text = CharInven._charName;
        AttributeDefaultList[(uint)AttributeLbl.BattlePoint].text = string.Format("{0}", CharInven._TotalAttack);//전투력
        
        //경험치
        uint curExp = 0, maxExp = 0;
        CharInven.GetCurrentAndMaxExp(ref curExp, ref maxExp);

        ExpBar.value = (float)curExp / (float)maxExp;
        HpBar.value = 1;
        ArmorBar.value = 1;
        
        Dictionary<AbilityType, float> abilityDic = NetData.instance.CalcPlayerStats();
        AttributeDefaultList[(uint)AttributeLbl.Hp].text = string.Format("{0}/{1}", (int)abilityDic[AbilityType.HP], (int)abilityDic[AbilityType.HP]);//체력
        AttributeDefaultList[(uint)AttributeLbl.Exp].text = string.Format("{0}/{1}", curExp, maxExp);//경험치
        AttributeDefaultList[(uint)AttributeLbl.Armor].text = string.Format("{0}/{1}", (int)abilityDic[AbilityType.SUPERARMOR], (int)abilityDic[AbilityType.SUPERARMOR]);//슈퍼아머

        int loopCount = AbilityList.Length;
        for (int i = 0; i < loopCount; i++)
        {
            AbilityType a = (AbilityType)i + 1;
            float value = 0;
            abilityDic.TryGetValue(a, out value);//없으면 0으로 처리함.
            AbilityList[i].text = uiMgr.GetAbilityStrValue(a, value);//체력
        }
    }
    #endregion

    #region 장비 셋트 리스트
    /// <summary> 장비셋 슬롯 셋팅 함수 </summary>
    void SetEquipSetSlot(Transform tf, NetData.SetItemData setItemData)
    {
        tf.FindChild("Open").gameObject.SetActive(true);
        tf.FindChild("Temp").gameObject.SetActive(false);

        Item.EquipmentSetInfo setInfo = _LowDataMgr.instance.GetItemSetLowData(setItemData.LowDataId);
        tf.name = string.Format("{0}", setInfo.Id);

        tf.FindChild("Open/Mark").gameObject.SetActive(setItemData.IsMount);//장착 이미지
        tf.FindChild("Open/SetIcon").GetComponent<UISprite>().spriteName = UIHelper.GetEquipSetIcon(setInfo.Type);

        tf.FindChild("Open/Text/Desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringItem(setInfo.SetDescriptionId);
        tf.FindChild("Open/Text/Name").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringItem(setInfo.SetName);

        string str = null;
        if (setItemData.IsMount)
            str = _LowDataMgr.instance.GetStringCommon(37);
        else
            str = _LowDataMgr.instance.GetStringCommon(38);

        tf.FindChild("Open/Btn/Btn_on/label").GetComponent<UILabel>().text = str;
        tf.FindChild("Open/Btn/Btn_off/label").GetComponent<UILabel>().text = str;
        tf.FindChild("Open/Btn/Btn_on").gameObject.SetActive(!setItemData.IsMount);
        tf.FindChild("Open/Btn/Btn_off").gameObject.SetActive(setItemData.IsMount);
        tf.FindChild("Open/Btn").collider.enabled = !setItemData.IsMount;

        uint[] itemIdx = new uint[] { setInfo.ItemIdx1, setInfo.ItemIdx2, setInfo.ItemIdx3, setInfo.ItemIdx4, setInfo.ItemIdx5, setInfo.ItemIdx6 };
        for (int j = 0; j < itemIdx.Length; j++)
        {
            tf.FindChild(string.Format("Open/SetList/{0}", j + 1)).GetComponent<UISprite>().atlas = AtlasMgr.instance.GetEquipAtlasForCharIdx(CharInven._userCharIndex);
            tf.FindChild(string.Format("Open/SetList/{0}", j + 1)).GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(_LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx[j]).Icon);
        }

        tf.GetComponent<UIEventTrigger>().onClick.Clear();
        EventDelegate.Set(tf.FindChild("Open/Btn").GetComponent<UIEventTrigger>().onClick, () => {
            if (CharInven.IsMountSetItem(setInfo.Id))//이미 장착중인 아이템
                return;

            NetworkClient.instance.SendPMsgEquipmentSetChangeC(setInfo.Id);
        });
    }
    
    /// <summary> 장비셋트 리스트 표기 </summary>
    void SetEquipList(bool isRefresh)
    {
        SetListPopTf.gameObject.SetActive(false);

        //장비 셋트 리스트 셋팅
        List<NetData.SetItemData> setItemList = CharInven.GetOwnSetItemData();
        if (!isRefresh)
        {
            for (int i = 0; i < setItemList.Count; i++)
            {
                Transform tf = null;
                if (i < EquipSetGridTf.childCount)
                    tf = EquipSetGridTf.GetChild(i);
                else
                {
                    tf = Instantiate(EquipSetGridTf.GetChild(0)) as Transform;
                    tf.parent = EquipSetGridTf;
                    tf.localScale = Vector3.one;
                }

                SetEquipSetSlot(tf, setItemList[i]);
            }
        }

        bool isOpenSlot = false;
        int addCount = 0;
        List<Item.EquipmentSetInfo> setList = _LowDataMgr.instance.GetItemSetLowDataList(UIHelper.GetClassType(CharInven._userCharIndex));
        if (setItemList.Count < setList.Count)//빈슬롯 하나 만들어준다.
        {
            int[] openSlot = new int[] {
                    _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen01CharLevel),
                    _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen02CharLevel),
                    _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen03CharLevel),
                    _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen04CharLevel)
                };

            isOpenSlot = openSlot[setItemList.Count - 1] <= CharInven._Level;
            Transform tf = EquipSetGridTf.FindChild("Temp");
            if (tf == null)
            {
                tf = Instantiate(EquipSetGridTf.GetChild(0)) as Transform;
                tf.parent = EquipSetGridTf;
                tf.localScale = Vector3.one;
                tf.name = string.Format("Temp");
            }
            else
            {
                tf.parent = transform;
                tf.parent = EquipSetGridTf;
            }

            tf.FindChild("Open").gameObject.SetActive(false);
            tf.FindChild("Temp").gameObject.SetActive(true);

            tf.FindChild("Temp/Txt_open").gameObject.SetActive(isOpenSlot);
            tf.FindChild("Temp/Txt_lock").gameObject.SetActive(!isOpenSlot);
            tf.FindChild("Temp/eff").gameObject.SetActive(isOpenSlot);
            if (!isOpenSlot)
            {
                tf.FindChild("Temp/Txt_lock").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9905), openSlot[setItemList.Count - 1]);
            }

            addCount = 1;
            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, OnClickSetListPopup);

            if(SceneManager.instance.CurTutorial == TutorialType.EQUIP_SET)
            {
                TutorialSupport tutoSupport = tf.gameObject.AddComponent<TutorialSupport>();
                tutoSupport.TutoType = TutorialType.EQUIP_SET;
                tutoSupport.SortId = 3;
                tutoSupport.IsScroll = true;

                tutoSupport.OnTutoSupportStart();
            }
        }
        else
        {
            Transform tf = EquipSetGridTf.FindChild("Temp");
            if (tf != null)
            {
                tf.parent = transform;
                tf.parent = EquipSetGridTf;
                tf.gameObject.SetActive(false);
            }
        }

        UIScrollView scroll = EquipSetGridTf.parent.GetComponent<UIScrollView>();
        scroll.ResetPosition();

        if (4 <= setItemList.Count+addCount)
            scroll.enabled = true;
        else
            scroll.enabled = false;

        EquipSetGridTf.GetComponent<UIGrid>().repositionNow = true;

        if (setItemList.Count != setList.Count)
        {
            if(!isOpenSlot)
            {
                AlramObj.SetActive(false);
                SceneManager.instance.SetAlram(AlramIconType.CHAR, false);
            }
        }
        else
        {
            AlramObj.SetActive(false);
            SceneManager.instance.SetAlram(AlramIconType.CHAR, false);
        }
    }

    /// <summary> 팝업 오픈 </summary>
    void OnClickSetListPopup()
    {
        SetListPopTf.gameObject.SetActive(true);

        int[] openSlot = new int[] {
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen01CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen02CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen03CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen04CharLevel)
        };

        int activeCount = 0;
        for (int i = CharInven.GetOwnSetItemData().Count - 1; i < openSlot.Length; i++)
        {
            if (CharInven._Level < openSlot[i])
                break;

            ++activeCount;
        }

        UIAtlas uiAtlas = AtlasMgr.instance.GetEquipAtlasForCharIdx(CharInven._userCharIndex);
        int maxCount = 0, slotCount=0;
        Transform gridTf = SetListPopTf.FindChild("ScrollView/Grid");
        List<Item.EquipmentSetInfo> setList = _LowDataMgr.instance.GetItemSetLowDataList(UIHelper.GetClassType(CharInven._userCharIndex));
        bool isTutoSet = SceneManager.instance.CurTutorial == TutorialType.EQUIP_SET;
        for (int i=0; i < setList.Count; i++)
        {
            if (CharInven.IsOwnSetItemData(setList[i].Id))//보유중인거 무시
                continue;

            Transform tf = null;
            if (slotCount < gridTf.childCount)
                tf = gridTf.GetChild(slotCount);
            else
            {
                tf = Instantiate(gridTf.GetChild(0) ) as Transform;
                tf.parent = gridTf;
                tf.localScale = Vector3.one;
            }

            ++slotCount;
            tf.name = setList[i].Id.ToString();

            tf.FindChild("on").gameObject.SetActive(false);
            tf.FindChild("SetIcon").GetComponent<UISprite>().spriteName = UIHelper.GetEquipSetIcon(setList[i].Type);
            tf.FindChild("Text/Desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringItem(setList[i].SetDescriptionId);
            tf.FindChild("Text/Name").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringItem(setList[i].SetName);
            
            uint[] itemIdx = new uint[] { setList[i].ItemIdx1, setList[i].ItemIdx2, setList[i].ItemIdx3, setList[i].ItemIdx4, setList[i].ItemIdx5, setList[i].ItemIdx6 };
            for (int j = 0; j < itemIdx.Length; j++)
            {
                tf.FindChild(string.Format("SetList/{0}", j + 1)).GetComponent<UISprite>().atlas = uiAtlas;
                tf.FindChild(string.Format("SetList/{0}", j + 1)).GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(_LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx[j]).Icon);
            }

            if (isTutoSet)
            {
                isTutoSet = false;
                TutorialSupport tutoSupport = tf.gameObject.AddComponent<TutorialSupport>();
                tutoSupport.TutoType = TutorialType.EQUIP_SET;
                tutoSupport.SortId = 4;
                tutoSupport.IsScroll = true;

                tutoSupport.OnTutoSupportStart();
            }

            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, () => {
                int selectCount = 0;
                tf.FindChild("on").gameObject.SetActive(!tf.FindChild("on").gameObject.activeSelf);
                for (int j = 0; j < gridTf.childCount; j++)
                {
                    if (!gridTf.GetChild(j).gameObject.activeSelf)
                        continue;
                    if (gridTf.GetChild(j).FindChild("on").gameObject.activeSelf)
                        ++selectCount;
                }
                
                if (selectCount <= activeCount)
                {
                    SetListPopTf.FindChild("Btn/Btn_on/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9912), selectCount, activeCount);
                }
                else
                {
                    tf.FindChild("on").gameObject.SetActive(false);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(9922), activeCount));
                }
            });

            ++maxCount;
        }

        for(int i=slotCount; i < gridTf.childCount; i++)
        {
            gridTf.GetChild(i).gameObject.SetActive(false);
        }

        UIScrollView scroll = SetListPopTf.FindChild("ScrollView").GetComponent<UIScrollView>();
        scroll.ResetPosition();
        gridTf.GetComponent<UIGrid>().repositionNow = true;
        if (maxCount < 3)
            scroll.enabled = false;
        else
            scroll.enabled = true;

        if (CharInven._Level < openSlot[CharInven.GetOwnSetItemData().Count-1] )//사용 못함
        {
            SetListPopTf.FindChild("Txt_lock").gameObject.SetActive(true);
            SetListPopTf.FindChild("Txt_lbl").gameObject.SetActive(false);
            SetListPopTf.FindChild("Txt_lock").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9905), openSlot[CharInven.GetOwnSetItemData().Count - 1]);
        }
        else//가능
        {
            SetListPopTf.FindChild("Txt_lock").gameObject.SetActive(false);
            SetListPopTf.FindChild("Txt_lbl").gameObject.SetActive(true);
            SetListPopTf.FindChild("Txt_lbl").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9911), activeCount);
        }


        SetListPopTf.FindChild("Btn/Btn_on/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9912), 0, activeCount);
        SetListPopTf.FindChild("Btn/Btn_off/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9912), 0, activeCount);
    }
    #endregion

    #region 강화 및 승급
    /// <summary> 강화or승급 탭으로 변경(ItemDetailPopup에서 사용) </summary>
    public void OnChangeTabUpgrade(bool isUpgrade, NetData._ItemData equipItem)
    {
        SetUpgrade(isUpgrade, equipItem);
        TabGroup.CoercionTab(2);
    }

    /// <summary> 강화or승급 셋팅 </summary>
    void SetUpgrade(bool isEnchant, NetData._ItemData equipItem)
    {
        UpgradeObj[0].SetActive(isEnchant);//강화
        UpgradeObj[1].SetActive(!isEnchant);//승급

        if (equipItem == null)//장착한 정보가 없다????????
        {
            Debug.LogError("not found equip itemdata error");
            return;
        }
        
        UpgradeItem = equipItem;
        Item.EquipmentInfo equipLowData = equipItem.GetEquipLowData();
        if (equipLowData.MaxEnchant <= UpgradeItem._enchant &&  equipLowData.NextPartsId <= 0)
        {
            #region Full강화
            EnTypeObj[0].SetActive(false);
            EnTypeObj[1].SetActive(true);
            EnchantTargetSlot[1].SetInvenItemSlot(equipItem, null);
            EnTargetName[1].text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(equipItem._Grade), equipItem.GetLocName());

            for (int i = 0; i < 4; i++)//현재 능력치
            {
                if (equipItem.StatList.Count <= i)
                {
                    EnFullAbility[i].transform.parent.gameObject.SetActive(false);
                    EnFullAbility[i + 4].transform.parent.gameObject.SetActive(false);
                    continue;
                }

                EnFullAbility[i].transform.parent.gameObject.SetActive(true);
                EnFullAbility[i + 4].transform.parent.gameObject.SetActive(true);

                AbilityType type = equipItem.StatList[i].Ability;
                int curValue = _LowDataMgr.instance.GetItemAbilityValueToInt(equipItem.StatList[i].Value, equipItem._enchant);

                EnFullAbility[i].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, curValue));
                EnFullAbility[i + 4].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, curValue));
            }
            #endregion
        }
        else if (isEnchant)
        {
            #region Defualt 강화
            EnTypeObj[0].SetActive(true);
            EnTypeObj[1].SetActive(false);
            ulong ownGold = NetData.instance.GetAsset(AssetType.Gold);
            Enchant.EnchantInfo enchantLowData = _LowDataMgr.instance.GetLowDataEnchant(equipLowData.EnchantId, equipItem._enchant);

            int count_2 = 0;
            if (enchantLowData.ItemValue2 <= 0 )
            {
                EnchantMaterial[1].transform.parent.gameObject.SetActive(false);
                EnchantMaterial[0].transform.parent.localPosition = Vector3.zero;
            }
            else
            {
                EnchantMaterial[1].transform.parent.gameObject.SetActive(true);
                Vector3 pos = EnchantMaterial[1].transform.parent.localPosition;
                pos.x = -pos.x;
                EnchantMaterial[0].transform.parent.localPosition = pos;

                count_2 = CharInven.GetItemCountForItemId(enchantLowData.ItemIdx2, (byte)eItemType.USE);
                EnMaterialAmount[1].text = string.Format("{0}/{1}", count_2, enchantLowData.ItemValue2);
                EnMaterialAmount[1].color = count_2 < enchantLowData.ItemValue2 ? Color.red : Color.white;
                EnchantMaterial[1].transform.parent.FindChild("Lock").gameObject.SetActive(count_2 < enchantLowData.ItemValue2);
                EnchantMaterial[1].transform.parent.FindChild("BtnBuy").gameObject.SetActive(count_2 < enchantLowData.ItemValue2);

                EnchantMaterial[1].SetLowDataItemSlot(enchantLowData.ItemIdx2, 0, (key) => {
                    OpenDetailPopup(null, enchantLowData.ItemIdx2);
                });
            }

            int count_1 = CharInven.GetItemCountForItemId(enchantLowData.ItemIdx1, (byte)eItemType.USE);
            EnMaterialAmount[0].text = string.Format("{0}/{1}", count_1, enchantLowData.ItemValue1);
            EnMaterialAmount[0].color = count_1 < enchantLowData.ItemValue1 ? Color.red : Color.white;
            EnchantMaterial[0].transform.parent.FindChild("Lock").gameObject.SetActive(count_1 < enchantLowData.ItemValue1);
            EnchantMaterial[0].transform.parent.FindChild("BtnBuy").gameObject.SetActive(count_1 < enchantLowData.ItemValue1);
            EnchantMaterial[0].SetLowDataItemSlot(enchantLowData.ItemIdx1, 0, (key) => {
                OpenDetailPopup(null, enchantLowData.ItemIdx1);
            });
                
            BtnEnchantTf[0].FindChild("Btn_off/cost").GetComponent<UILabel>().text = string.Format("{0}", enchantLowData.CostGold);//최소 강화 금액
            BtnEnchantTf[0].FindChild("Btn_on/cost").GetComponent<UILabel>().text = string.Format("{0}", enchantLowData.CostGold);

            BtnEnchantTf[0].FindChild("Btn_off").gameObject.SetActive(ownGold < enchantLowData.CostGold || count_1 < enchantLowData.ItemValue1 || count_2 < enchantLowData.ItemValue2 );
            BtnEnchantTf[0].FindChild("Btn_on").gameObject.SetActive(enchantLowData.CostGold <= ownGold && enchantLowData.ItemValue1 <= count_1 && enchantLowData.ItemValue2 <= count_2 );

            EnTargetName[0].text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(equipItem._Grade), equipItem.GetLocName());
            EnchantTargetSlot[0].SetInvenItemSlot(equipItem, null);

            uint nextEnchant = (uint)equipItem._enchant + 1;
            if (equipLowData.MaxEnchant < nextEnchant)//이미 풀임.
            {
                MaxEnchantCount = nextEnchant = equipLowData.MaxEnchant;
            }
            else//일괄 강화 계산
            {
                MaxEnchantCount = 0;
                uint totalCost = 0, totalMaterial_1 = 0, totalMaterial_2 = 0;
                for (int i = equipItem._enchant; i < equipLowData.MaxEnchant; i++)
                {
                    Enchant.EnchantInfo nextEnchantInfo = _LowDataMgr.instance.GetLowDataEnchant(equipLowData.EnchantId, i);
                    if (nextEnchantInfo == null)
                        break;

                    if (ownGold < (totalCost+nextEnchantInfo.CostGold) || count_1 < (totalMaterial_1+nextEnchantInfo.ItemValue1) || count_2 < (totalMaterial_2+nextEnchantInfo.ItemValue2))
                    {
                        if (totalCost == 0)
                        {
                            totalCost = nextEnchantInfo.CostGold;
                            totalMaterial_1 = nextEnchantInfo.ItemValue1;
                            totalMaterial_2 = nextEnchantInfo.ItemValue2;
                        }
                        break;
                    }

                    totalCost += nextEnchantInfo.CostGold;
                    totalMaterial_1 += nextEnchantInfo.ItemValue1;
                    totalMaterial_2 += nextEnchantInfo.ItemValue2;

                    MaxEnchantCount = (uint)i+1;
                }
                    
                BtnEnchantTf[1].FindChild("Btn_off/cost").GetComponent<UILabel>().text = string.Format("{0}", totalCost);//최대 강화 금액
                BtnEnchantTf[1].FindChild("Btn_on/cost").GetComponent<UILabel>().text = string.Format("{0}", totalCost);
                BtnEnchantTf[1].FindChild("Btn_off").gameObject.SetActive(MaxEnchantCount == 0);
                BtnEnchantTf[1].FindChild("Btn_on").gameObject.SetActive(MaxEnchantCount != 0 );
                if (MaxEnchantCount == 0)
                    MaxEnchantCount = 1;
            }

            for (int i = 0; i < 4; i++)//다음, 최대 강화 능력치 표기
            {
                if (equipItem.StatList.Count <= i)
                {
                    EnCurAbility[i].transform.parent.gameObject.SetActive(false);
                    EnCurAbility[i + 4].transform.parent.gameObject.SetActive(false);
                    continue;
                }

                EnCurAbility[i].transform.parent.gameObject.SetActive(true);
                EnCurAbility[i + 4].transform.parent.gameObject.SetActive(true);

                AbilityType type = equipItem.StatList[i].Ability;
                int curValue = _LowDataMgr.instance.GetItemAbilityValueToInt(equipItem.StatList[i].Value, equipItem._enchant);
                int nextValue = _LowDataMgr.instance.GetItemAbilityValueToInt(equipItem.StatList[i].Value, nextEnchant);
                int maxValue = _LowDataMgr.instance.GetItemAbilityValueToInt(equipItem.StatList[i].Value, MaxEnchantCount);

                EnCurAbility[i].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, curValue));
                EnNextAbility[i].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, nextValue));
                EnAddAbility[i].text = string.Format("(+{0})", uiMgr.GetAbilityStrValue(type, (uint)(nextValue - curValue)));

                EnCurAbility[i + 4].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, curValue));
                EnNextAbility[i + 4].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(type), uiMgr.GetAbilityStrValue(type, maxValue));
                EnAddAbility[i + 4].text = string.Format("(+{0})", uiMgr.GetAbilityStrValue(type, (uint)(maxValue - curValue)));
            }
            #endregion
        }
        else
        {
            #region Default 승급
            //EvTypeObj[0].SetActive(true);
            //EvTypeObj[1].SetActive(false);

            EvolveTargetSlot[0].SetInvenItemSlot(equipItem, null);//현재 아이템
            EvolveTargetSlot[1].SetLowDataItemSlot(equipLowData.NextPartsId, 0);//승급 후 아이템
                
            EvTargetName[0].text = equipItem.GetLocName();
            EvTargetName[1].text = equipItem.GetLocName();

            Enchant.EvolveInfo evolveInfo = _LowDataMgr.instance.GetLowDataEvolve(equipLowData.EvolveId);
            EvolveMaterial[0].SetLowDataItemSlot(evolveInfo.ItemIdx1, 0, (key) => {
                OpenDetailPopup(null, evolveInfo.ItemIdx1);
            });

            if(0 < evolveInfo.ItemIdx2)
            { 
                EvolveMaterial[1].SetLowDataItemSlot(evolveInfo.ItemIdx2, 0, (key) => {
                    OpenDetailPopup(null, evolveInfo.ItemIdx2);
                });
            }
            else
            {
                EvolveMaterial[1].EmptySlot();
            }

            int count_1 = CharInven.GetItemCountForItemId(evolveInfo.ItemIdx1, (byte)eItemType.USE);
            int count_2 = CharInven.GetItemCountForItemId(evolveInfo.ItemIdx2, (byte)eItemType.USE);
            EvMaterialAmount[0].text = string.Format("{0}/{1}", count_1, evolveInfo.ItemValue1);
            EvMaterialAmount[1].text = string.Format("{0}/{1}", count_2, evolveInfo.ItemValue2);

            EvMaterialAmount[0].color = count_1 < evolveInfo.ItemValue1 ? Color.red : Color.white;
            EvMaterialAmount[0].transform.parent.transform.FindChild("Lock").gameObject.SetActive(count_1 < evolveInfo.ItemValue1);
            EvMaterialAmount[0].transform.parent.transform.FindChild("BtnBuy").gameObject.SetActive(count_1 < evolveInfo.ItemValue1);

            EvMaterialAmount[1].color = count_2 < evolveInfo.ItemValue2 ? Color.red : Color.white;
            EvMaterialAmount[1].transform.parent.transform.FindChild("Lock").gameObject.SetActive(count_2 < evolveInfo.ItemValue2);
            EvMaterialAmount[1].transform.parent.transform.FindChild("BtnBuy").gameObject.SetActive(count_2 < evolveInfo.ItemValue2);

            for (int i = 0; i < 4; i++)//능력치 셋팅
            {
                AbilityType nowType = AbilityType.NONE;
                int nowValue = 0;
                if (equipItem.StatList.Count <= i)
                {
                    //EvCurAbility[i].transform.parent.gameObject.SetActive(false);
                    EvCurAbility[i].gameObject.SetActive(false);
                }
                else
                {
                    EvCurAbility[i].gameObject.SetActive(true);
                    nowType = equipItem.StatList[i].Ability;
                    nowValue = _LowDataMgr.instance.GetItemAbilityValueToInt(equipItem.StatList[i].Value, equipItem._enchant);
                    EvCurAbility[i].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(nowType), nowValue);
                }
                    
				Item.ItemValueInfo nextItemValueInfo = _LowDataMgr.instance.GetLowDataEquipItemValueInfo(equipLowData.NextPartsId, i);
				if (nextItemValueInfo == null){
					EvNextAbility[i].gameObject.SetActive(false);
				}
				else{
                    EvNextAbility[i].gameObject.SetActive(true);
                    AbilityType nextType = (AbilityType)nextItemValueInfo.OptionId;

                    int nextValue = _LowDataMgr.instance.GetItemAbilityValueToInt(nextItemValueInfo.BasicValue, 0);
                    EvNextAbility[i].text = string.Format("{0}  {1}", uiMgr.GetAbilityLocName(nextType), uiMgr.GetAbilityStrValue(nextType, nextValue));
                    EvAddAbility[i].text = string.Format("({0}{1})", (0 < nextValue - nowValue ? "+" : ""), (nextValue - nowValue));
                }
            }

            BtnEvolveTf.FindChild("Btn_off/cost").GetComponent<UILabel>().text = string.Format("{0}", evolveInfo.CostGold);
            BtnEvolveTf.FindChild("Btn_on/cost").GetComponent<UILabel>().text = string.Format("{0}", evolveInfo.CostGold);

            ulong ownGold = NetData.instance.GetAsset(AssetType.Gold);
            BtnEvolveTf.FindChild("Btn_off").gameObject.SetActive(ownGold < evolveInfo.CostGold || count_1 < evolveInfo.ItemValue1 || count_2 < evolveInfo.ItemValue2);
            BtnEvolveTf.FindChild("Btn_on").gameObject.SetActive(ownGold >= evolveInfo.CostGold && count_1 >= evolveInfo.ItemValue1 && count_2 >= evolveInfo.ItemValue2);

            #endregion
        }
    }

    /// <summary> 강화 </summary>
    void OnClickEnchant(bool isMax)
    {
        if (UpgradeItem == null)
            return;
        
        if(IsUpgrading)//업그레이드 중.
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 9919);
            return;
        }

        Enchant.EnchantInfo info = _LowDataMgr.instance.GetLowDataEnchant(UpgradeItem.GetEquipLowData().EnchantId, UpgradeItem._enchant);
        int count_1 = CharInven.GetItemCountForItemId(info.ItemIdx1, (byte)eItemType.USE);
        int count_2 = CharInven.GetItemCountForItemId(info.ItemIdx2, (byte)eItemType.USE);

        if (NetData.instance.GetAsset(AssetType.Gold) < info.CostGold)//잔액부족
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 443);
        else if (count_1 < info.ItemValue1 || count_2 < info.ItemValue2)//재료부족
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 172);
        else//강화.
        {
            uiMgr.PrevAttack = (int)CharInven.RefreshTotalAttackPoint(false);

            if (isMax)
                NetworkClient.instance.SendPMsgEquipmentEnchantTurboC(UpgradeItem._itemIndex, UpgradeItem._equipitemDataIndex, (int)MaxEnchantCount);
            else
                NetworkClient.instance.SendPMsgEquipmentEnchantC(UpgradeItem._itemIndex, UpgradeItem._equipitemDataIndex);
        }
    }

    /// <summary> 승급 </summary>
    void OnClickEvolve()
    {
        if (UpgradeItem == null)
            return;

        if (IsUpgrading)//업그레이드 중.
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 9920);
            return;
        }

        Item.EquipmentInfo lowData = UpgradeItem.GetEquipLowData();
        Enchant.EvolveInfo info = _LowDataMgr.instance.GetLowDataEvolve(lowData.EvolveId);
        int count_1 = CharInven.GetItemCountForItemId(info.ItemIdx1, (byte)eItemType.USE);
        int count_2 = CharInven.GetItemCountForItemId(info.ItemIdx2, (byte)eItemType.USE);

        if (NetData.instance.GetAsset(AssetType.Gold) < info.CostGold)//잔액부족
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 443);
        else if (count_1 < info.ItemValue1 || count_2 < info.ItemValue2)//재료부족
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 172);
        else if (CharInven._Level < lowData.GradeUpLevel)//레벨 부족
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(945), lowData.GradeUpLevel));
        else//승급.
        {
            uiMgr.PrevAttack = (int)CharInven.RefreshTotalAttackPoint(false);
            NetworkClient.instance.SendPMsgEquipmentEvolveC((int)UpgradeItem._itemIndex, (int)UpgradeItem._equipitemDataIndex);
        }
    }
    #endregion

    #region 신분
    void SetStatus() {

        for (int i = 0; i < ListGridTf.childCount; i++)
        {
            ListGridTf.GetChild(i).gameObject.SetActive(false);
        }

        List<Character.StatusInfo> statusList = _LowDataMgr.instance.GetLowDataCharStatusList();
        int type = -1, count =0;
        for (int i=0; i < statusList.Count; i++)
        {
            NetData.StatusData data = null;
            for (int j = 0; j < StatusList.Count; j++) {//먼저 보유중인건지를 체크한다.

                Character.StatusInfo lowData = _LowDataMgr.instance.GetLowDataCharStatus(StatusList[j].LowDataId);
                if(lowData.Type != statusList[i].Type)
                    continue;
                else if(lowData.Type == statusList[i].Type && lowData.Level != statusList[i].Level)
                {
                    type = lowData.Type;
                    continue;
                }

                data = StatusList[j];
                type = -1;
                break;
            }

            if (statusList[i].Type == type)//같은 종류의 것을 이미 셋팅했다면 무시
                continue;

            Character.StatusInfo info = statusList[i];
            type = info.Type;
            Transform tf = null;
            if (count < ListGridTf.childCount)
                tf = ListGridTf.GetChild(count);
            else
            {
                tf = Instantiate(ListGridTf.GetChild(0) ) as Transform;
                tf.parent = ListGridTf;
                tf.localScale = Vector3.one;
            }

            tf.gameObject.SetActive(true);
            int value = data == null ? 0 : data.Point;
            tf.name = string.Format("{0}", info.Id );
            tf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} ({1} {2})"
                , _LowDataMgr.instance.GetStringCommon(info.StatusNameId), _LowDataMgr.instance.GetStringCommon(14), info.Level);
            tf.FindChild("value").GetComponent<UILabel>().text = string.Format("{0}/{1}", value, info.Clearvalue);
            tf.FindChild("desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(info.GoalDescriptionId);
            tf.FindChild("condition").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(info.GoalNameId);

            tf.FindChild("mark").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(info.Icon);
            tf.FindChild("Lock").gameObject.SetActive(data == null || !data.IsOwn );

            if(0 < info.NextId)
                tf.FindChild("bar").GetComponent<UISprite>().fillAmount = (float)value / (float)info.Clearvalue;

            tf.FindChild("bar").gameObject.SetActive(0 < info.NextId);
            tf.FindChild("max_bar").gameObject.SetActive(info.NextId <= 0);

            EventDelegate.Set(tf.FindChild("BtnLink").GetComponent<UIEventTrigger>().onClick, () => {//바로가기
                OnClickLink(info.ClearType);
            });
            EventDelegate.Set(tf.FindChild("BtnVitality").GetComponent<UIEventTrigger>().onClick, () => {//활성화
                NetworkClient.instance.SendPMsgRoleIdentifyUnlockC(info.Id);
            });
            EventDelegate.Set(tf.FindChild("BtnEquip").GetComponent<UIEventTrigger>().onClick, () => {//장착
                if (data.IsEquip)
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 1063);
                else
                    NetworkClient.instance.SendPMsgRoleIdentifyUseC(data.Id);
            });
            EventDelegate.Set(tf.FindChild("BtnLvup").GetComponent<UIEventTrigger>().onClick, () => {//레벨업
                NetworkClient.instance.SendPMsgRoleIdentifyUpgradeC(data.Id);
            });

            if (data == null || !data.IsOwn) {//미보유시.
                if(info.RewardType != 0 )
                { 
                    uint skillIds = 0;
                    switch (CharInven._userCharIndex) {
                        case 11000: skillIds = info.RewardId01; break;//권사
                        case 12000: skillIds = info.RewardId02; break;//명장
                        case 13000: skillIds = info.RewardId03; break;//협녀
                    }

                    UISprite skillIcon = tf.FindChild("Lock/SkillIcon").GetComponent<UISprite>();
                    tf.FindChild("Lock/SkillIcon").gameObject.SetActive(true);
                    if (info.RewardType == 1) { 
                        SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillIds);
                        skillIcon.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Skill);
                        skillIcon.spriteName = _LowDataMgr.instance.GetLowDataIcon(setInfo.Icon);

                        EventDelegate.Set(skillIcon.gameObject.GetComponent<UIEventTrigger>().onClick, () => {
                            NetData.SkillSetData setData = CharInven.GetSkillSetData(skillIds);
                            uint skillId = 0;
                            if (setData == null)
                                skillId = setInfo.skill1;
                            else
                                skillId = setData.SkillId[0];

                            IsLinkTab = true;
                            TabGroup.CoercionTab((int)TabType.SkillUpgrade);
                            OnClickSkillInfo((int)(setData == null ? 1 : setData.SkillLevel[0]), skillIds, skillId, 1);
                        });
                    }
                    else {//패시브
                        NetData.PassiveData passData = CharInven.GetPassiveData((int)skillIds);
                        byte level = (byte)(passData != null ? passData.Level : 1);
                        SkillTables.SkillLevelInfo lvInfo = _LowDataMgr.GetSkillLevelData(skillIds, level);
                        skillIcon.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Buff);
                        skillIcon.spriteName = _LowDataMgr.instance.GetLowDataIcon(lvInfo.icon);

                        EventDelegate.Set(skillIcon.gameObject.GetComponent<UIEventTrigger>().onClick, () => {
                            IsLinkTab = true;
                            TabGroup.CoercionTab((int)TabType.SkillUpgrade);
                            OnClickSkillInfo(level, 0, skillIds, -1);
                        });
                    }
                }
                else
                    tf.FindChild("Lock/SkillIcon").gameObject.SetActive(false);

                tf.FindChild("BtnVitality").gameObject.SetActive(info.Clearvalue <= value);//활성화
                tf.FindChild("BtnLink").gameObject.SetActive(value < info.Clearvalue);//바로가기

                tf.FindChild("BtnEquip").gameObject.SetActive(false);
                tf.FindChild("BtnMax").gameObject.SetActive(false);
                tf.FindChild("BtnLvup").gameObject.SetActive(false);

                if (info.Clearvalue < value)//획득 못할경우
                {
                    uint skillId = 0;
                    switch (CharInven._userCharIndex)
                    {
                        case 11000: skillId = info.RewardId01; break;//권사
                        case 12000: skillId = info.RewardId02; break;//명장
                        case 13000: skillId = info.RewardId03; break;//협녀
                    }

                    UISprite sp = tf.FindChild("Lock/SkillIcon").GetComponent<UISprite>();
                    SkillTables.SkillSetInfo setInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillId);
                    if (setInfo != null)
                        sp.spriteName = _LowDataMgr.instance.GetLowDataIcon(setInfo.Icon);
                    else//버프
                    {
                        SkillTables.SkillLevelInfo levelInfo = _LowDataMgr.GetSkillLevelData(skillId, 1);
                        sp.spriteName = _LowDataMgr.instance.GetLowDataIcon(levelInfo.icon);
                    }
                }
            }
            else//보유 중일 경우.
            {
                tf.FindChild("Lock/SkillIcon").gameObject.SetActive(false);
                if (data.IsEquip)
                {
                    EquipStatusTf.name = info.Id.ToString();
                    EquipStatusTf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} ({1} {2})"
                        , _LowDataMgr.instance.GetStringCommon(info.StatusNameId), _LowDataMgr.instance.GetStringCommon(14), info.Level);
                    EquipStatusTf.FindChild("desc").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(info.GoalDescriptionId);
                    EquipStatusTf.FindChild("mark").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(info.Icon);
                }

                tf.FindChild("BtnVitality").gameObject.SetActive(false);
                tf.FindChild("BtnEquip").gameObject.SetActive(true);
                tf.FindChild("BtnMax").gameObject.SetActive(info.NextId <= 0);

                tf.FindChild("BtnEquip/on").gameObject.SetActive(!data.IsEquip);
                tf.FindChild("BtnEquip/off").gameObject.SetActive(data.IsEquip);
                if (0 < info.NextId)
                {
                    tf.FindChild("BtnLink").gameObject.SetActive(value < info.Clearvalue);
                    tf.FindChild("BtnLvup").gameObject.SetActive(info.Clearvalue <= value);
                }
                else
                    tf.FindChild("BtnLvup").gameObject.SetActive(false);
            }
            ++count;
        }
        
        ListGridTf.GetComponent<UIGrid>().repositionNow = true;
        ListGridTf.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    /// <summary> 바로가기 기능 </summary>
    void OnClickLink(byte linkType)
    {
        bool isHide = true;
        switch(linkType)
        {
            case 1://모험_일반클리어
                UIMgr.OpenChapter(null, 0);
                break;
            case 2://모험_어려움클리어
                UIMgr.OpenChapter(null, 0);
                break;
            case 3://마계의탑
                UIMgr.OpenTowerPanel();
                break;
            case 4://성망//얘 난투장
            case 5://난투장_몬스터처치
            case 6://난투장_유저처치
                UIMgr.OpenDogFight();
                break;
            case 7://장비재료던전_플레이
                UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_GOLD);
                break;
            case 8://스킬재료던전_플레이
                UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_EXP);
                break;
            case 9://보스레이드_클리어
                UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.RAID);
                break;
            case 10://뽑기
                UIMgr.OpenGachaPanel(SceneManager.instance.GetGachaFreeTime[0], SceneManager.instance.GetGachaFreeTime[1]);
                break;
            case 11://상점
                UIMgr.OpenShopPanel();
                break;
            case 12://VIP
                //아직 없음
                isHide = false;
                break;

            default://최대수치
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 175);
                isHide = false;
                break;
        }

        if(isHide)
        {
            ObjectHide();
        }
    }
    #endregion

    #region 스킬
    void SetSkill(bool isReset)
    {
        List<Character.StatusInfo> statusList = _LowDataMgr.instance.GetLowDataCharStatusList();
        int type = -1;
        int skillCount=0, passCount=0;
        for(int i=0; i < statusList.Count; i++)
        {
            if (statusList[i].Type == type)
                continue;
            
            type = statusList[i].Type;
            Character.StatusInfo info = statusList[i];
            if (info.RewardType == 0)
                continue;

            uint skillId = 0;
            switch (CharInven._userCharIndex) {
                case 11000: skillId = info.RewardId01; break;//권사
                case 12000: skillId = info.RewardId02; break;//명장
                case 13000: skillId = info.RewardId03; break;//협녀
            }
            
            if (info.RewardType == 1) {//스킬셋
                Transform tf = null;
                if (skillCount < SkillGridTf.childCount)
                    tf = SkillGridTf.GetChild(skillCount);
                else {
                    tf = Instantiate(SkillGridTf.GetChild(0)) as Transform;
                    tf.parent = SkillGridTf;
                    tf.localScale = Vector3.one;
                }

                ++skillCount;
                tf.name = skillId.ToString();
                NetData.SkillSetData setData = CharInven.GetSkillSetData(skillId);
                SkillTables.SkillSetInfo skillInfo = _LowDataMgr.instance.GetLowDataSkillSet(skillId);
                uint[] skillIds = new uint[] { skillInfo.skill1, skillInfo.skill2, skillInfo.skill3, skillInfo.skill4 };
                for(int j=0; j < skillIds.Length; j++) {
                    SkillTables.ActionInfo acInfo = _LowDataMgr.instance.GetSkillActionLowData(skillIds[j] );
                    int lv = setData == null ? 1 : (int)setData.SkillLevel[j];

                    tf.FindChild(string.Format("{0}/icon", j)).GetComponent<UISprite>().atlas = AtlasMgr.instance.GetSkillAtlasForCharIdx(CharInven._userCharIndex);
                    tf.FindChild(string.Format("{0}/icon", j)).GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(acInfo.Icon);
                    tf.FindChild(string.Format("{0}/lv", j)).GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), lv);

                    uint skill = skillIds[j];
                    int arr = j + 1;
                    EventDelegate.Set(tf.FindChild(string.Format("{0}", j)).GetComponent<UIEventTrigger>().onClick, () => { OnClickSkillInfo(lv, skillId, skill, arr); });

                    if (isReset && skillCount == 1 && j == 0)//초기 값 셋팅
                        OnClickSkillInfo(lv, skillId, skill, 1);
                }
                
                if (setData == null) {//미보유
                    tf.FindChild("UnEquip").gameObject.SetActive(true);
                    tf.FindChild("Equip").gameObject.SetActive(false);

                    tf.FindChild("BtnEquip/on").gameObject.SetActive(false);
                    tf.FindChild("BtnEquip/off").gameObject.SetActive(true);
                    tf.FindChild("BtnEquip/off/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9926), _LowDataMgr.instance.GetStringCommon(info.StatusNameId));
                }
                else {//보유
                    tf.FindChild("UnEquip").gameObject.SetActive( !setData.IsEquip);
                    tf.FindChild("Equip").gameObject.SetActive( setData.IsEquip);

                    tf.FindChild("BtnEquip/on").gameObject.SetActive(!setData.IsEquip);
                    tf.FindChild("BtnEquip/off").gameObject.SetActive(setData.IsEquip);
                    tf.FindChild("BtnEquip/off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1063);//적용중
                    tf.FindChild("BtnEquip/on/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(657);//적용
                }

                EventDelegate.Set(tf.FindChild("BtnEquip").GetComponent<UIEventTrigger>().onClick, () => {
                    NetData.SkillSetData data = CharInven.GetSkillSetData(skillId);
                    if (data == null)//없는 아이템.
                        SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(9926), _LowDataMgr.instance.GetStringCommon(info.StatusNameId)) );
                    else if (data.IsEquip)//이미 적용 중
                        SceneManager.instance.SetNoticePanel(NoticeType.Message, 1063);
                    else
                        NetworkClient.instance.SendPMsgRoleActiveSkillUseC(skillId);//장착
                });
            }
            else {//패시브
                Transform tf = null;
                if (passCount < PassiveGridTf.childCount)
                    tf = PassiveGridTf.GetChild(passCount);
                else {
                    tf = Instantiate(PassiveGridTf.GetChild(0)) as Transform;
                    tf.parent = PassiveGridTf;
                    tf.localScale = Vector3.one;
                }

                tf.name = skillId.ToString();

                ++passCount;
                SkillTables.SkillLevelInfo lvInfo = _LowDataMgr.GetSkillLevelData(skillId, 1);
                NetData.PassiveData pasData = CharInven.GetPassiveData((int)skillId);
                if(pasData != null )
                {
                    tf.FindChild("BtnEquip/on").gameObject.SetActive( !pasData.IsEquip);
                    tf.FindChild("BtnEquip/off").gameObject.SetActive(pasData.IsEquip);

                    tf.FindChild("BtnEquip/on/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon( (uint)(pasData.IsEquip ? 1063 : 657) );
                    tf.FindChild("BtnEquip/off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon((uint)(pasData.IsEquip ? 1063 : 657));
                }
                else
                {
                    tf.FindChild("BtnEquip/on").gameObject.SetActive(false);
                    tf.FindChild("BtnEquip/off").gameObject.SetActive(true);

                    tf.FindChild("BtnEquip/off/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(9926), _LowDataMgr.instance.GetStringCommon(info.StatusNameId));
                }

                tf.FindChild("Icon").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(lvInfo.icon);
                tf.FindChild("lv").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), 1);

                tf.FindChild("UnEquip").gameObject.SetActive(true);
                tf.FindChild("Equip").gameObject.SetActive(false);
                EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, () => {
                    OnClickSkillInfo(1, 0, skillId, -1);
                });

                EventDelegate.Set(tf.FindChild("BtnEquip").GetComponent<UIEventTrigger>().onClick, () => {//장착
                    if(pasData == null)
                        SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(9926), _LowDataMgr.instance.GetStringCommon(info.StatusNameId)));
                    else if(pasData.IsEquip)
                        SceneManager.instance.SetNoticePanel(NoticeType.Message, 1063);
                    else
                        NetworkClient.instance.SendPMsgRolePassiveSkillUseC(pasData.PassiveId);
                });
            }
        }
    }

    /// <summary> 스킬 강화 정보및 상세정보 </summary>
    void OnClickSkillInfo(int lv, uint skillSetId, uint skillId, int number)
    {
        for (int i = 0; i < FullAmounts.Length; i++)
        {
            FullAmounts[i].text = "";
            SkillMaterial[i + 1].gameObject.SetActive(false);
        }
        
        //현재
        SkillTables.SkillLevelInfo skillLv = _LowDataMgr.GetSkillLevelData(skillId, (byte)lv);
        SkillDesc[0].text = _LowDataMgr.instance.GetStringSkillName(skillLv.skilldesc);

        SkillTables.ActionInfo acInfo = _LowDataMgr.instance.GetSkillActionLowData(skillId);
        if (acInfo != null)
        {
            SelectIcon.atlas = AtlasMgr.instance.GetSkillAtlasForCharIdx(CharInven._userCharIndex);
            SelectIcon.spriteName = _LowDataMgr.instance.GetLowDataIcon(acInfo.Icon);
            SkillName.text = string.Format("{0} ({1} {2})", _LowDataMgr.instance.GetStringSkillName(acInfo.name), _LowDataMgr.instance.GetStringCommon(14), lv);
        }
        else
        {
            SelectIcon.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Buff);
            SelectIcon.spriteName = _LowDataMgr.instance.GetLowDataIcon(skillLv.icon);
            SkillName.text = string.Format("{0} ({1} {2})", _LowDataMgr.instance.GetStringSkillName(skillLv.name), _LowDataMgr.instance.GetStringCommon(14), lv);
        }
        
        int fullLv = lv+1;
        uint fullCost = 0;
        Dictionary<uint, uint> totalDic = new Dictionary<uint, uint>();
        ulong ownGold = NetData.instance.GetAsset(AssetType.Gold);
        
        while (true)//최대 레벨업 구한다.
        {
            skillLv = _LowDataMgr.GetSkillLevelData(skillId, (byte)fullLv);
            if (skillLv == null)//테이블이 존재하지않으면 여기까지가 최대 렙이라는 것.
            {
                fullLv -= 1;
                break;
            }

            if (CharInven._Level < skillLv.LimitLv)//레벨부족
                break;

            bool isNotOwnStatus = true;
            if (skillLv.SkillLevelUpStatusId <= 0)
                isNotOwnStatus = false;
            else { 
                Character.StatusInfo statusInfo = _LowDataMgr.instance.GetLowDataCharStatus(skillLv.SkillLevelUpStatusId);
                for (int i = 0; i < StatusList.Count; i++) {//신분 레벨 체크
                    if (!StatusList[i].IsOwn)
                        continue;

                    Character.StatusInfo info = _LowDataMgr.instance.GetLowDataCharStatus(StatusList[i].LowDataId);
                    if (info.Type == statusInfo.Type && statusInfo.Level <= info.Level) {
                        isNotOwnStatus = false;
                        break;
                    }
                }
            }

            if(isNotOwnStatus)
                break;

            uint totalAmount_0=0, totalAmount_1=0;
            if (!totalDic.TryGetValue(skillLv.SkillLevelUpItem1, out totalAmount_0)) {
                totalDic.Add(skillLv.SkillLevelUpItem1, skillLv.SkillLevelUpItem1Count);
            }

            if (!totalDic.TryGetValue(skillLv.SkillLevelUpItem2, out totalAmount_1)) {
                totalDic.Add(skillLv.SkillLevelUpItem2, skillLv.SkillLevelUpItem2Count);
            }

            int ownAmount_0 = CharInven.GetItemCountForItemId(skillLv.SkillLevelUpItem1, (byte)eItemType.USE);
            int ownAmount_1 = CharInven.GetItemCountForItemId(skillLv.SkillLevelUpItem2, (byte)eItemType.USE);
            if (ownGold < fullCost+skillLv.CostGold || ownAmount_0 < totalAmount_0+skillLv.SkillLevelUpItem1Count || ownAmount_1 < totalAmount_1+skillLv.SkillLevelUpItem2Count)//재화 부족
                break;
            
            ++fullLv;
            fullCost += skillLv.CostGold;
            totalDic[skillLv.SkillLevelUpItem1] += skillLv.SkillLevelUpItem1Count;
            totalDic[skillLv.SkillLevelUpItem2] += skillLv.SkillLevelUpItem2Count;
        }

        //최대 강화
        skillLv = _LowDataMgr.GetSkillLevelData(skillId, (byte)fullLv);
        SkillDesc[2].text = _LowDataMgr.instance.GetStringSkillName(skillLv.skilldesc);
        
        int arr = 1;
        var enumerator = totalDic.GetEnumerator();
        while(enumerator.MoveNext() )
        {
            if (enumerator.Current.Key <= 0)
                break;

            SkillMaterial[arr].gameObject.SetActive(true);
            SkillMaterial[arr].SetLowDataItemSlot(enumerator.Current.Key, 0, (key) => {
                OpenDetailPopup(null, (uint)key);
            });

            int amount = CharInven.GetItemCountForItemId(enumerator.Current.Key, (byte)eItemType.USE);
            FullAmounts[arr - 1].text = string.Format("{0}/{1}", amount, enumerator.Current.Value);//totalMaxAmount[arr-1]);
            ++arr;
        }

        bool isEnchant = OnClickOnceSkillEnchant(true, false, skillSetId, skillId, lv + 1, number);
        for (int i = 0; i < BtnUpgradeTf.Length; i++)
        {
            BtnUpgradeTf[i].FindChild("on").gameObject.SetActive(isEnchant);
            BtnUpgradeTf[i].FindChild("off").gameObject.SetActive(!isEnchant);
        }

        SkillTables.SkillLevelInfo skillLv_1 = _LowDataMgr.GetSkillLevelData(skillId, (byte)(lv + 1));
        BtnUpgradeTf[1].FindChild("on/price").GetComponent<UILabel>().text = string.Format("X{0}", fullCost == 0 ? skillLv_1.CostGold : fullCost);
        BtnUpgradeTf[1].FindChild("off/price").GetComponent<UILabel>().text = string.Format("X{0}", fullCost == 0 ? skillLv_1.CostGold : fullCost);
        EventDelegate.Set(BtnUpgradeTf[1].GetComponent<UIEventTrigger>().onClick, () => {//일괄 강화
            OnClickOnceSkillEnchant(false, true, skillSetId, skillId, lv + 1, number);
        });

        //1강 후
        int ownAmount = CharInven.GetItemCountForItemId(skillLv_1.SkillLevelUpItem1, (byte)eItemType.USE);
        SkillDesc[1].text = _LowDataMgr.instance.GetStringSkillName(skillLv_1.skilldesc);

        OnceAmount.text = string.Format("{0}/{1}", ownAmount, skillLv_1.SkillLevelUpItem1Count);
        SkillMaterial[0].SetLowDataItemSlot(skillLv_1.SkillLevelUpItem1, 0, (key) => {
            OpenDetailPopup(null, skillLv_1.SkillLevelUpItem1);
        });

        BtnUpgradeTf[0].FindChild("on/price").GetComponent<UILabel>().text = string.Format("X{0}", skillLv_1.CostGold);
        BtnUpgradeTf[0].FindChild("off/price").GetComponent<UILabel>().text = string.Format("X{0}", skillLv_1.CostGold);
        EventDelegate.Set(BtnUpgradeTf[0].GetComponent<UIEventTrigger>().onClick, () => {//1회 강화
            OnClickOnceSkillEnchant(false, false, skillSetId, skillId, lv+1, number);
        });
    }

    /// <summary> 스킬 1회 강화 </summary>
    bool OnClickOnceSkillEnchant(bool isCheck, bool isMax, uint skillSetId, uint skillId, int lv, int number)
    {
        bool isOwn = false;
        if (0 < number)//보유한것인지 검사
        {
            if (CharInven.GetSkillSetData(skillSetId) != null)
                isOwn = true;
        }
        else
        {
            NetData.PassiveData passData = CharInven.GetPassiveData((int)skillId);
            if (passData != null)
                isOwn = true;
        }

        if( !isOwn)//보유하지않은 스킬
        {
            if (!isCheck)
            {
                List<Character.StatusInfo> infoList = _LowDataMgr.instance.GetLowDataCharStatusList();
                for (int i = 0; i < infoList.Count; i++)
                {
                    if (infoList[i].RewardType == 0)
                        continue;

                    uint id = infoList[i].RewardType == 1 ? skillSetId : skillId;
                    if (infoList[i].RewardId01.Equals(id) ||
                        infoList[i].RewardId02.Equals(id) ||
                        infoList[i].RewardId03.Equals(id))
                    {
                        SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(9929), _LowDataMgr.instance.GetStringCommon(infoList[i].StatusNameId)));
                        break;
                    }
                }
            }
            return false;
        }

        SkillTables.SkillLevelInfo skillData = _LowDataMgr.GetSkillLevelData(skillId, (byte)lv);
        if (CharInven._Level < skillData.LimitLv)
        {
            if(!isCheck)
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 384);
            return false;
        }

        int amount = CharInven.GetItemCountForItemId(skillData.SkillLevelUpItem1, (byte)eItemType.USE);
        if (amount < skillData.SkillLevelUpItem1Count)
        {
            if (!isCheck)
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 354);
            return false;
        }

        if (0 < skillData.SkillLevelUpItem2)
        {
            amount = CharInven.GetItemCountForItemId(skillData.SkillLevelUpItem2, (byte)eItemType.USE);
            if (amount < skillData.SkillLevelUpItem2Count)
            {
                if (!isCheck)
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 354);
                return false;
            }
        }

        if(0 < skillData.SkillLevelUpStatusId)
        { 
            bool isNotOwnStatus = true;
            Character.StatusInfo statusInfo = _LowDataMgr.instance.GetLowDataCharStatus(skillData.SkillLevelUpStatusId);
            for (int i = 0; i < StatusList.Count; i++) {//신분 레벨 체크
                if (!StatusList[i].IsOwn)
                    continue;

                Character.StatusInfo info = _LowDataMgr.instance.GetLowDataCharStatus(StatusList[i].LowDataId);
                if (info.Type == statusInfo.Type && statusInfo.Level <= info.Level)
                {
                    isNotOwnStatus = false;
                    break;
                }
            }

            if (isNotOwnStatus)
            {
                if (!isCheck)
                {
                    Character.StatusInfo info = _LowDataMgr.instance.GetLowDataCharStatus(skillData.SkillLevelUpStatusId);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(9929), _LowDataMgr.instance.GetStringCommon(info.StatusNameId)));
                }
                return false;
            }
        }

        if (!isCheck)
        {
            if (isMax)
            {
                if (0 < skillSetId)
                    NetworkClient.instance.SendPMsgRoleActiveSkillUpgradeTurboC((int)skillSetId, number);
                else
                    NetworkClient.instance.SendPMsgRolePassiveSkillUpgradeTurboC((int)skillId);
            }
            else
            {
                if (0 < skillSetId)
                    NetworkClient.instance.SendPMsgRoleActiveSkillUpgradeC(skillSetId, number);
                else
                    NetworkClient.instance.SendPMsgRolePassiveSkillUpgradeC(skillId);
            }
        }

        return true;
    }

    #endregion

    #region 공통 및 기타? 사용하는 함수 모음
    /// <summary> 모델 생성 </summary>
    void CreateCharModel(bool isChange)
    {
        NetData._CostumeData costumeData = CharInven.GetEquipCostume();

        if (costumeData != null)
        {
            uint weaponId = 0, clothId = 0, headId = 0;
            if (CharInven.isHideCostum)
            {
                NetData._ItemData head = CharInven.GetEquipParts(ePartType.HELMET);
                NetData._ItemData cloth = CharInven.GetEquipParts(ePartType.CLOTH);
                NetData._ItemData weapon = CharInven.GetEquipParts(ePartType.WEAPON);

                if (head != null)
                    headId = head._equipitemDataIndex;

                if (cloth != null)
                    clothId = cloth._equipitemDataIndex;

                if (weapon != null)
                    weaponId = weapon._equipitemDataIndex;
            }

            if (isChange)
            {
                TownState town = SceneManager.instance.GetState<TownState>();
                town.MyHero.SetChangeSkin(true);
            }

            UIHelper.CreatePcUIModel("InventoryPanel", RotationTargetList[0].transform, CharInven.GetCharIdx(), headId, costumeData._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 3, CharInven.isHideCostum, false);
        }
    }

    /// <summary> 코스튬 정보 갱신 </summary>
    void SetCostumeState()
    {
        string enable = "Btn_Blue01";
        string disable = "Btn_Blue01Dis";

        ToggleObjs[0].spriteName = CharInven.isHideCostum ? enable : disable;
        ToggleObjs[0].collider.enabled = CharInven.isHideCostum;

        ToggleObjs[1].spriteName = CharInven.isHideCostum ? disable : enable;
        ToggleObjs[1].collider.enabled = !CharInven.isHideCostum;

        ToggleObjs[0].gameObject.SetActive(CharInven.isHideCostum ? true : false);
        ToggleObjs[1].gameObject.SetActive(CharInven.isHideCostum ? false : true);
    }

    /// <summary> 아이템의 상세정보 팝업 </summary>
    void OpenDetailPopup(NetData._ItemData itemData, uint itemIdx)
    {
        if(itemData == null)
            UIMgr.OpenDetailPopup(this, itemIdx, 5);
        else
            UIMgr.OpenDetailPopup(this, itemData, 5);
    }

    /// <summary> 전투력, 장착정보 </summary>
    void SetCharView()
    {
        CharInfoObj.SetActive(true);
        CharAttack.text = string.Format("{0}", CharInven.RefreshTotalAttackPoint(true) );

        ///장착중인 아이템 아이콘 셋팅
        for (int i = 0; i < (int)ePartType.RING; i++)
        {
            Transform tf = EquipPartsTf.GetChild(i);
            if (tf == null)
                continue;

            ePartType type = (ePartType)i + 1;
            NetData._ItemData itemData = CharInven.GetEquipParts(type);

            UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
            UISprite icon = tf.FindChild("icon").GetComponent<UISprite>();
            UISprite grade = tf.FindChild("grade").GetComponent<UISprite>();
            UILabel Enchant = tf.FindChild("upgrade_lv").GetComponent<UILabel>();
            UISprite bg = tf.GetComponent<UISprite>();
            if (itemData == null)//미장착
            {
                icon.enabled = false;
                grade.enabled = false;
                bg.spriteName = UIHelper.GetDefaultEquipIcon(type);
                Enchant.gameObject.SetActive(false);
            }
            else//장착
            {
                icon.enabled = true;
                grade.enabled = true;

                Item.EquipmentInfo equipLowData = itemData.GetEquipLowData();
                icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(equipLowData.Icon);
                grade.spriteName = string.Format("Icon_0{0}", equipLowData.Grade);
                bg.spriteName = string.Format("Icon_bg_0{0}", equipLowData.Grade);
                Enchant.text = itemData._enchant <= 0 ? "" : string.Format("+{0}", itemData._enchant);
                Enchant.gameObject.SetActive(true);
            }

            EventDelegate.Set(uiTri.onClick, delegate () {

                NetData._ItemData equipData = CharInven.GetEquipParts(type);
                if (equipData == null)//들어오면 문제가 있는 것.
                    return;

                if(CurTabType == TabType.Upgrade)
                {
                    if (IsUpgrading)
                        return;

                    if (equipData._enchant < equipData.GetEquipLowData().MaxEnchant)
                        SetUpgrade(true, equipData);
                    else if (0 < equipData.GetEquipLowData().NextPartsId)
                        SetUpgrade(false, equipData);
                    else//최대 등급
                    {
                        SetUpgrade(true, equipData);
                        //SceneManager.instance.SetNoticePanel(NoticeType.Message, 1182);
                    }

                    return;
                }

                OpenDetailPopup(equipData, 0);
            });
        }

        bool isCreate = false;
        Transform equipTf = transform.FindChild("CharacterInfo/SimpleEquip");
        List<Item.EquipmentSetInfo> setList = _LowDataMgr.instance.GetItemSetLowDataList(UIHelper.GetClassType(CharInven._userCharIndex));
        for (int i=setList.Count-1; 0 <= i; i--)
        {
            Transform tf = null;
            if ((setList.Count-1)-i < equipTf.childCount)
                tf = equipTf.GetChild((setList.Count-1)-i);
            else
            {
                tf = Instantiate(equipTf.GetChild(0)) as Transform;
                tf.parent = equipTf;
                tf.localScale = Vector3.one;
                isCreate = true;
            }

            uint dataId = setList[i].Id;
            tf.FindChild("Icon").GetComponent<UISprite>().spriteName = UIHelper.GetEquipSetIcon(setList[i].Type);
            tf.FindChild("Mount").gameObject.SetActive(CharInven.IsMountSetItem(dataId));
            tf.FindChild("Lock").gameObject.SetActive(!CharInven.IsOwnSetItemData(dataId));
            
            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, () => {
                if (CharInven.IsMountSetItem(dataId))//이미 장착중인 아이템
                    return;
                else if (!CharInven.IsOwnSetItemData(dataId))//보유하고있지않음
                {
                    Item.EquipmentSetInfo info = _LowDataMgr.instance.GetItemSetLowData(dataId);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0
                        , string.Format(_LowDataMgr.instance.GetStringCommon(9914), _LowDataMgr.instance.GetStringItem(info.SetName)));
                }
                else
                {
                    uiMgr.PrevAttack = (int)CharInven.RefreshTotalAttackPoint(false);
                    NetworkClient.instance.SendPMsgEquipmentSetChangeC(dataId);
                }
            });
        }

        if(isCreate)
            equipTf.GetComponent<UIGrid>().repositionNow = true;
    }

    public override void Hide()
    {
		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        if (SetListPopTf.gameObject.activeSelf)
            SetListPopTf.gameObject.SetActive(false);
        else
        {
            base.Hide();
            if (reOpenPanel != null)
                reOpenPanel.Show(reOpenPanel.GetParams());
            else
                UIMgr.OpenTown();
        }
    }

    public override void ObjectHide()
    {
        base.ObjectHide();

        IsUpgrading = false;
        TempCoroutine.instance.RemoveKeyDelay("EquipEff");
        if (EffectGo != null)
            Destroy(EffectGo);

    }

    #endregion
}

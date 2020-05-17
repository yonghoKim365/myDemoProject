using UnityEngine;
using System.Collections.Generic;

public class PartnerOwnView : MonoBehaviour
{
    //승급, 스킬, 속성 TabList or ViewObj에 사용한다
    enum ViewType { None=-1, Attribute, Skill, Evolve }
    public GameObject InvenItemPrefab;
    public GameObject[] ViewObj;//ViewType으로 배열 관리함
    public GameObject[] BtnMove;

    public UIEventTrigger BtnSkillLevelUp;
    public Transform SkillLevelUpEffRoot;

    public GameObject SkillMaxLevel;

    public Transform ModelRoot;//모델 생성 위치 어미

    public UISlider[] AttStateSlider;//0 hp, 1 exp

    public UISprite[] SkillIcon;//스킬 아이콘들 Active 0~3

    public UILabel Name;
    public UILabel Level;
    public UILabel GradeLabel;
    public UILabel Damage;
    public UILabel[] AttStateValue;//0 hp, 1 슈퍼아머
    public UILabel[] AttAbilitys;
    public UILabel[] EvolveAttAbilitys; //승급용 
    public UILabel[] NextEvolveAttAbilitys; //다음승급용

    public UILabel[] SkillName;//스킬 이름들 Active 0~3, Buff 4~7
    public UILabel[] SkillDesc;//스킬 설명들 Active 0~3, Buff 4~7
    public UILabel[] SkillLevel;//스킬레벨들 

    public UILabel[] CurSelectSkillLevel;   //0현재 ,1다음
    public UILabel[] CurSelectSkilDamage;  //0현재, 1다음
    public UILabel CurSelectSkillDec;   //현재 선택한 스킬 설명

    public UIEventTrigger[] ActiveSkillTrigger;

    public UILabel[] EvolveLevel;   //승급레벨
    public UILabel[] EvolveManLevel;    //승급최대레벨
    public Transform EvolveEffRoot; //승급이펙트부모
    public UIEventTrigger BtnEvolve;  //승급버튼
    public GameObject MaxEvolve;   //최대승급

    //public UITabGroup TabGroup;//우측의 탭 리스트 관리 스크립트
    public UIEventTrigger[] TabBtn;// 0:속성 1:스킬 2:승급

    public Transform EvoleRoot; //진화승급슬롯루트
    public Transform SkillLevelUpRoot;//스킬레벨슬롯 루트

    public List<NetData._PartnerData> CurTabPartnerList;   //탭정렬된 파트너리스트

    public float ModelRootX;    //x좌표

    private NetData._PartnerData CurData;//현재 보여주고 있는 파트너 아이템
    private NetData._UserInfo UserInfo;
    private InvenItemSlotObject SkillLevelUpMaterialSlot;//스킬레벨업 재료 슬롯
    private InvenItemSlotObject EvolveMaterialSlot; //승급 재료 슬롯

    private bool IsUpgrading;

    private int CurPnDataArray;
    private ViewType CurTabIndex = ViewType.None;
    private ViewType PrevTabIndex =ViewType.None;

    /// <summary> OwnPartner 초기화 함수 </summary>
    public void Init()
    {
        //TabGroup.Initialize(OnClickTabList);
        for(int i=0;i<TabBtn.Length;i++)
        {
            int idx = i;
            EventDelegate.Set(TabBtn[idx].onClick, delegate () { OnClickTabList(idx); });

        }

        UserInfo = NetData.instance.GetUserInfo();

        EventDelegate.Set(BtnMove[0].GetComponent<UIButton>().onClick, delegate ()
        {
            OnClickChangePartner(-1);
        });

        EventDelegate.Set(BtnMove[1].GetComponent<UIButton>().onClick, delegate ()
        {
            OnClickChangePartner(1);
        });

        GameObject slotGo = Instantiate(InvenItemPrefab) as GameObject;
        Transform tf = slotGo.transform;
        tf.parent = SkillLevelUpRoot.transform.FindChild("ItemRoot");
        tf.localPosition = Vector3.zero;
        tf.localScale = Vector3.one;

        SkillLevelUpMaterialSlot = slotGo.GetComponent<InvenItemSlotObject>();
        SkillLevelUpMaterialSlot.EmptySlot();

        GameObject slotEvolveGo = Instantiate(InvenItemPrefab) as GameObject;
        Transform tEvolvef = slotEvolveGo.transform;
        tEvolvef.parent = EvoleRoot.transform.FindChild("ItemRoot");
        tEvolvef.localPosition = Vector3.zero;
        tEvolvef.localScale = Vector3.one;

        EvolveMaterialSlot = slotEvolveGo.GetComponent<InvenItemSlotObject>();
        EvolveMaterialSlot.EmptySlot();

        SkillMaxLevel.GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1284), _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.PartnerMaxSkillLevel));
        SkillMaxLevel.SetActive(false);
        MaxEvolve.GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1285);
        MaxEvolve.SetActive(false);
    }

    void OnClickChangePartner(int add)
    {
        CurPnDataArray += add;
        for (int i = 0; i < ViewObj.Length; i++)
        {
            if (!ViewObj[i].activeSelf)
                continue;

            SetPartnerInfo(CurTabPartnerList[CurPnDataArray], i);
            break;
        }
    }

    /// <summary> OwnDetailInfo를 실행하는 함수 </summary>
    public void SetPartnerInfo(NetData._PartnerData data, int openView = (int)ViewType.Attribute)
    {
        if(ModelRoot.childCount>0)
            DestroyImmediate(ModelRoot.GetChild(0).gameObject);

        UIHelper.CreatePartnerUIModel(ModelRoot, data._partnerDataIndex, 3, true, false, "PartnerPanel");
        ModelRoot.localEulerAngles = Vector3.zero;

        string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), data._NowLevel);
        Name.text = data.GetLocName();
        Damage.text = string.Format(_LowDataMgr.instance.GetStringCommon(1163), data._Attack);

        GradeLabel.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor((int)data.CurQuality), data.GetGradeName());

        Transform skillTab = TabBtn[1].transform;
        Transform upTab = TabBtn[2].transform;

        if (data._isOwn)
        {
            if (data._NowLevel >= data._MaxLevel)
                Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1152), data._NowLevel);
            else
                Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), data._NowLevel);
        }
        else
        {
            Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), 1);
        }

        CurData = data;
        CheckMoveState(openView);

        transform.FindChild("Info/Mark").GetComponent<UISprite>().spriteName = CurData.GetClassType();

        OnClickTabList(openView);
        //TabGroup.CoercionTab(openView);

        skillTab.GetComponent<BoxCollider>().enabled = data._isOwn;
        upTab.GetComponent<BoxCollider>().enabled = data._isOwn;

        skillTab.transform.FindChild("tab_off").gameObject.SetActive(data._isOwn);
        skillTab.transform.FindChild("tab_disable").gameObject.SetActive(!data._isOwn);
        upTab.transform.FindChild("tab_off").gameObject.SetActive(data._isOwn);
        upTab.transform.FindChild("tab_disable").gameObject.SetActive(!data._isOwn);

        //중심축때문에.. 
        //파트너 모델링의 x 좌표만큼 상위의 ModelPos에더해줌.
        ModelRoot.SetLocalX(ModelRootX + ModelRoot.GetChild(0).transform.localPosition.x);
        ModelRoot.GetChild(0).transform.SetLocalX(0);

        if (SceneManager.instance.CurTutorial == TutorialType.PARTNER)
            TabBtn[1].gameObject.GetComponent<TutorialSupport>().OnTutoSupportStart();
    }

    /// <summary> 아이템의 상세정보 팝업 </summary>
    void OpenDetailPopup(NetData._ItemData itemData, uint itemIdx)
    {
        UIBasePanel partner = UIMgr.GetUIBasePanel("UIPanel/PartnerPanel");
        if (itemData == null)
            UIMgr.OpenDetailPopup(partner, itemIdx, 5);
        else
            UIMgr.OpenDetailPopup(partner, itemData, 5);
    }

    void CheckMoveState(int openView)
    {
        // 앞서 탭 정렬된 파트너만 보여줌.
        // 정보화면을 제외한 스킬,승급탭에서 미보유 파트너 볼수가없다.
        bool left = true;
        bool right = true;

        int max = 0;

        for (int i = 0; i < CurTabPartnerList.Count; i++)
        {
            if(openView !=(int)ViewType.Attribute)
            {
                if (!CurTabPartnerList[i]._isOwn)//나머지는 없는거임
                    break;
            }

            max = i;

            if (CurTabPartnerList[i] == CurData)
                CurPnDataArray = i;
        }

        if (max <= CurPnDataArray)
        {
            CurPnDataArray = max;
            right = false;
        }
        if (CurPnDataArray <= 0)
        {
            CurPnDataArray = 0;
            left = false;
        }

        BtnMove[0].SetActive(left);
        BtnMove[1].SetActive(right);
    }
    
    #region 속성

    void InitAttribute()
    {

        string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), CurData._NowLevel);
        Name.text = CurData.GetLocName();
        Damage.text = string.Format(_LowDataMgr.instance.GetStringCommon(1163), CurData._Attack);

        GradeLabel.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor((int)CurData.CurQuality), CurData.GetGradeName());

        if (CurData._isOwn)
        {
            if (CurData._NowLevel >= CurData._MaxLevel)
                Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1152), CurData._NowLevel);
            else
                Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), CurData._NowLevel);
        }
        else
        {
            Level.text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), 1);
        }

        transform.FindChild("Info/Mark").GetComponent<UISprite>().spriteName = CurData.GetClassType();

        Dictionary<AbilityType, float> abilityDic = NetData.instance.CalcPartnerStats(CurData._NowLevel, CurData.GetLowData(), CurData.CurQuality);


        float maxExp = CurData._MaxExp;
        AttStateSlider[0].value = 1;
        AttStateSlider[1].value = 1;

        AttStateValue[0].text = string.Format("{0} / {1}", Mathf.FloorToInt(abilityDic[AbilityType.HP]), Mathf.FloorToInt(abilityDic[AbilityType.HP]));
        AttStateValue[1].text = string.Format("{0} / {1}", Mathf.FloorToInt(abilityDic[AbilityType.SUPERARMOR]), Mathf.FloorToInt(abilityDic[AbilityType.SUPERARMOR]));



        //어빌리티 정보 셋팅
        int length = AttAbilitys.Length;
        for (int i = 0; i < length; i++)
        {
            float value = 0;
            AbilityType a = (AbilityType)i + 1;
            abilityDic.TryGetValue(a, out value);

            AttAbilitys[i].text = UIMgr.instance.GetAbilityStrValue(a, value);
        }

    }

    #endregion 속성끝

    #region 승급
    void InitPromo()
    {
        Dictionary<AbilityType, float> nextAbilityDic = null;
        Transform evolveView = ViewObj[(int)ViewType.Evolve].transform;
        Damage.text = string.Format(_LowDataMgr.instance.GetStringCommon(1163), CurData._Attack);

        if (CurData.CurQuality >= 6)
        {
            //최대
            for(int i=0;i<NextEvolveAttAbilitys.Length;i++)
            {
                NextEvolveAttAbilitys[i].text = "";
            }

            evolveView.FindChild("arrow").gameObject.SetActive(false);
            evolveView.FindChild("name").gameObject.SetActive(false);
            EvoleRoot.gameObject.SetActive(false);
            BtnEvolve.transform.gameObject.SetActive(false);
            EvolveLevel[1].transform.parent.gameObject.SetActive(false);
            EvolveLevel[0].transform.parent.transform.localPosition = new Vector3(0, 268.4f, 0);
            MaxEvolve.SetActive(true);
        }
        else
        {
         
            Partner.PartnerDataInfo nextInfo = _LowDataMgr.instance.GetPartnerInfo(CurData.GetLowData().QualityUpId);   //다음승급데이터
            nextAbilityDic = NetData.instance.CalcPartnerStats(CurData._NowLevel, nextInfo, nextInfo.Quality);

            EvolveLevel[1].text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor((int)CurData.CurQuality + 1), GetGradeName((int)(CurData.CurQuality + 1)));
            EvolveManLevel[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(1152), GetMaxEvolveLevel((int)CurData.CurQuality + 1));

            //스킬재료아이템
            EvolveMaterialSlot.SetLowDataItemSlot(CurData.GetLowData().QualityUpItem, 0, (lowDataId) =>
            {
                OpenDetailPopup(null, CurData.GetLowData().QualityUpItem);
            });

            int nowAmount = UserInfo.GetItemCountForItemId(CurData.GetLowData().QualityUpItem, (byte)eItemType.USE);
            bool isSuccess = true;
            if (nowAmount < CurData.GetLowData().QualityUpItemCount || NetData.instance.GetAsset(AssetType.Gold) < CurData.GetLowData().QualityUpNeedGold)
                isSuccess = false;

            EvoleRoot.FindChild("value").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1167), nowAmount, CurData.GetLowData().QualityUpItemCount);
            EvoleRoot.FindChild("value").GetComponent<UILabel>().color = nowAmount < CurData.GetLowData().QualityUpItemCount ? Color.red : Color.white;

            EvoleRoot.FindChild("Lock").gameObject.SetActive(nowAmount < CurData.GetLowData().QualityUpItemCount);

            if (isSuccess)
            {
                BtnEvolve.transform.FindChild("Btn_on").gameObject.SetActive(true);
                BtnEvolve.transform.FindChild("Btn_off").gameObject.SetActive(false);

                BtnEvolve.transform.FindChild("Btn_on/price").GetComponent<UILabel>().text = string.Format("x{0}", CurData.GetLowData().QualityUpNeedGold);
            }
            else
            {
                BtnEvolve.transform.FindChild("Btn_on").gameObject.SetActive(false);
                BtnEvolve.transform.FindChild("Btn_off").gameObject.SetActive(true);

                BtnEvolve.transform.FindChild("Btn_off/price").GetComponent<UILabel>().text = string.Format("x{0}", CurData.GetLowData().QualityUpNeedGold);
            }

            EventDelegate.Set(BtnEvolve.onClick, delegate ()
            {
                if(IsUpgrading)
                {
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 9921);
                    return;
                }

                if (!isSuccess)
                {
                    string msg = "";
                    if (nowAmount < CurData.GetLowData().QualityUpItemCount)
                        msg = _LowDataMgr.instance.GetStringCommon(1173);   //재료부족
                    else if (NetData.instance.GetAsset(AssetType.Gold) < CurData.GetLowData().QualityUpNeedGold)
                        msg = _LowDataMgr.instance.GetStringCommon(1174);   //골드부족

                    UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(141), msg, _LowDataMgr.instance.GetStringCommon(117));
                    return;
                }

                NetworkClient.instance.SendPMsgHeroEvolveC(CurData._partnerIndex);

            });

            evolveView.FindChild("arrow").gameObject.SetActive(true);
            evolveView.FindChild("name").gameObject.SetActive(true);

            EvoleRoot.gameObject.SetActive(true);
            BtnEvolve.transform.gameObject.SetActive(true);
            EvolveLevel[1].transform.parent.gameObject.SetActive(true);
            EvolveLevel[0].transform.parent.transform.localPosition = new Vector3(-145.5f, 268.4f, 0);
            MaxEvolve.SetActive(false);

        }

        EvolveLevel[0].text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor((int)CurData.CurQuality), GetGradeName((int)CurData.CurQuality));
        EvolveManLevel[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(1152), GetMaxEvolveLevel((int)CurData.CurQuality));
        EvolveLevel[1].gameObject.transform.localScale = Vector3.one;

        Dictionary<AbilityType, float> abilityDic = NetData.instance.CalcPartnerStats(CurData._NowLevel, CurData.GetLowData(), CurData.CurQuality);

        //어빌리티 정보 셋팅
        int length = EvolveAttAbilitys.Length;
        for (int i = 0; i < length; i++)
        {
            float value = 0;
            AbilityType a = (AbilityType)i + 1;
            abilityDic.TryGetValue(a, out value);

            if (nextAbilityDic != null)
            {
                float nextValue = 0;
                AbilityType b = (AbilityType)i + 1;
                nextAbilityDic.TryGetValue(b, out nextValue);

                if (nextValue - value > 0)
                {
                    NextEvolveAttAbilitys[i].text = string.Format("▲{0}", nextValue - value);
                }
                else if (nextValue - value == 0)
                {
                    NextEvolveAttAbilitys[i].text = "━";
                }

            }

            EvolveAttAbilitys[i].text = UIMgr.instance.GetAbilityStrValue(a, value);
        }

       

    }

    string GetGradeName(int quality)
    {
        string gradeName = "";
        switch (quality)
        {
            case 1: //일반
                gradeName = _LowDataMgr.instance.GetStringCommon(1145);
                break;
            case 2://우수
                gradeName = _LowDataMgr.instance.GetStringCommon(1146);
                break;
            case 3://희귀
                gradeName = _LowDataMgr.instance.GetStringCommon(1147);
                break;
            case 4://고대
                gradeName = _LowDataMgr.instance.GetStringCommon(1148);
                break;
            case 5://전설
                gradeName = _LowDataMgr.instance.GetStringCommon(1149);
                break;
            case 6://무쌍
                gradeName = _LowDataMgr.instance.GetStringCommon(1150);
                break;
        }

        return gradeName;
    }

    int GetMaxEvolveLevel(int grade)
    {
        int level=0;
        switch (grade)
        {
            case 1:
                level =_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner1GradeMaxLevel);
                break;
            case 2:
                level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner2GradeMaxLevel);
                break;
            case 3:
                level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner3GradeMaxLevel);
                break;
            case 4:
                level =_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner4GradeMaxLevel);
                break;
            case 5:
                level= _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner5GradeMaxLevel);
                break;
            case 6:
                level = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Partner6GradeMaxLevel);
                break;
         
        }
        return level;
    }
    /// <summary> 진화 별 상승 응답 </summary>
    public void OnPMsgHeroPromoSHandler(uint partnerId)
    {
        CurData = UserInfo.GetPartnerForIdx(partnerId);

        UIHelper.CreateEffectInGame(EvolveEffRoot, "Fx_partner_skillup_01");

        EvolveLevel[1].GetComponent<TweenColor>().ResetToBeginning();
        EvolveLevel[1].GetComponent<TweenColor>().PlayForward();

        EvolveLevel[1].GetComponent<TweenScale>().ResetToBeginning();
        EvolveLevel[1].GetComponent<TweenScale>().PlayForward();

        IsUpgrading = true;
        TempCoroutine.instance.KeyDelay("ParEvolve", 1f, delegate ()
        {
            IsUpgrading = false;
            DestroyImmediate(EvolveEffRoot.GetChild(0).gameObject);
            InitPromo();
            SoundManager.instance.PlaySfxSound(eUISfx.UI_par_upgrade_01, false);
        });
           
    }

    #endregion 승급 끝

    #region 스킬

    void SkillInit()
    {

        for (int i = 0; i < SkillIcon.Length; i++)
        {
            GameObject selectEff = SkillIcon[i].transform.parent.transform.FindChild("Selecteff").gameObject;
            selectEff.SetActive(false);
        }
        //액티브 스킬 셋팅
        Dictionary<ushort, NetData._PartnerActiveSkillData> activeSkillList = CurData.ActiveSkillList;
        int activeCount = activeSkillList.Count;
        for (int i = 1; i < activeCount; i++)//0번은 평타.
        {
            uint skillId = activeSkillList[(ushort)i]._skillIndex;
            SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(skillId);
            if (actionLowData == null)
                continue;

            SkillIcon[i - 1].spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
            SkillName[i - 1].text = _LowDataMgr.instance.GetStringSkillName(actionLowData.name);
            SkillLevel[i - 1].text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), CurData.GetBuffSkillToSlot((ushort)i)._skillLevel);

            int idx = i;
            EventDelegate.Set(ActiveSkillTrigger[i - 1].onClick, delegate ()
            {
                OnClickActiveSkill(idx - 1, activeSkillList[(ushort)idx]);
            });
            //SkillDesc[i - 1].text = _LowDataMgr.instance.GetStringSkillName(actionLowData.descrpition);
        }

        //디폴트는 첫번째스킬
        OnClickActiveSkill(0, activeSkillList[1]);

    }

    private int SelectActiveSkillIndex;
    /// <summary>
    /// 액티브스킬칸 누름/// </summary>
    void OnClickActiveSkill(int index, NetData._PartnerActiveSkillData selectSkill)
    {
        SelectActiveSkillIndex = index;
        for (int i = 0; i < SkillIcon.Length; i++)
        {
            GameObject selectEff = SkillIcon[i].transform.parent.transform.FindChild("Selecteff").gameObject;
            selectEff.SetActive(i == index);
        }
        SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(selectSkill._skillIndex);
        if (actionLowData == null)
            return;

        SkillTables.SkillLevelInfo levelLowData = _LowDataMgr.GetSkillLevelData(selectSkill._skillIndex, selectSkill._skillLevel);
        CurSelectSkillDec.text = _LowDataMgr.instance.GetStringSkillName(actionLowData.descrpition);

        Transform SkillView = ViewObj[(int)ViewType.Skill].transform;

        //만렙인지 체크
        SkillTables.SkillLevelInfo nextLevelLowData = null;
        int maxSkillLevel = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.PartnerMaxSkillLevel);
        if (selectSkill._skillLevel < maxSkillLevel)
        {
            SkillView.FindChild("arrow").gameObject.SetActive(true);
            SkillView.FindChild("name").gameObject.SetActive(true);

            nextLevelLowData = _LowDataMgr.GetSkillLevelData(selectSkill._skillIndex, (byte)(selectSkill._skillLevel + 1));
            CurSelectSkillLevel[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), selectSkill._skillLevel + 1);

            //스킬재료아이템
            SkillLevelUpMaterialSlot.SetLowDataItemSlot(nextLevelLowData.SkillLevelUpItem1, 0, (lowDataId) =>
            {
                OpenDetailPopup(null, nextLevelLowData.SkillLevelUpItem1);

                //UIMgr.OpenClickPopup((uint)lowDataId, SkillLevelUpMaterialSlot.transform.position);
            });

            int nowAmount = UserInfo.GetItemCountForItemId(nextLevelLowData.SkillLevelUpItem1, (byte)eItemType.USE);
            bool isSuccess = true;
            if (nowAmount < nextLevelLowData.SkillLevelUpItem1Count || NetData.instance.GetAsset(AssetType.Gold) < nextLevelLowData.CostGold)
                isSuccess = false;

            SkillLevelUpRoot.FindChild("value").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1167), nowAmount, nextLevelLowData.SkillLevelUpItem1Count);
            SkillLevelUpRoot.FindChild("value").GetComponent<UILabel>().color = nowAmount < nextLevelLowData.SkillLevelUpItem1Count ? Color.red : Color.white;

            SkillLevelUpRoot.FindChild("Lock").gameObject.SetActive(nowAmount < nextLevelLowData.SkillLevelUpItem1Count);

            if (isSuccess)
            {
                BtnSkillLevelUp.transform.FindChild("Btn_on").gameObject.SetActive(true);
                BtnSkillLevelUp.transform.FindChild("Btn_off").gameObject.SetActive(false);

                BtnSkillLevelUp.transform.FindChild("Btn_on/price").GetComponent<UILabel>().text = string.Format("x{0}", nextLevelLowData.CostGold);
            }
            else
            {
                BtnSkillLevelUp.transform.FindChild("Btn_on").gameObject.SetActive(false);
                BtnSkillLevelUp.transform.FindChild("Btn_off").gameObject.SetActive(true);

                BtnSkillLevelUp.transform.FindChild("Btn_off/price").GetComponent<UILabel>().text = string.Format("x{0}", nextLevelLowData.CostGold);
            }

            SkillLevelUpRoot.gameObject.SetActive(true);
            BtnSkillLevelUp.transform.gameObject.SetActive(true);
            CurSelectSkillLevel[1].transform.parent.gameObject.SetActive(true);

            CurSelectSkilDamage[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(1166), GetSkillDamage(levelLowData.factorRate.list, (float)levelLowData.factor).ToString());
            CurSelectSkilDamage[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(1166), GetSkillDamage(nextLevelLowData.factorRate.list, (float)nextLevelLowData.factor).ToString());
            SkillMaxLevel.SetActive(false);

            //위치변경도 있어야함
            CurSelectSkillLevel[0].transform.parent.transform.localPosition = new Vector3(-138, -3, 0);
            EventDelegate.Set(BtnSkillLevelUp.onClick, delegate ()
            {
                if (!isSuccess)
                {
                    string msg = "";
                    if (nowAmount < nextLevelLowData.SkillLevelUpItem1Count)
                        msg = _LowDataMgr.instance.GetStringCommon(1173);   //재료부족
                    else if (NetData.instance.GetAsset(AssetType.Gold) < nextLevelLowData.CostGold)
                        msg = _LowDataMgr.instance.GetStringCommon(1174);   //골드부족

                    UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(141), msg, _LowDataMgr.instance.GetStringCommon(117));
                    return;
                }
                NetworkClient.instance.SendPMsgHeroSkillUpgradeC(CurData._partnerIndex, SelectActiveSkillIndex + 1);

            });

        }
        else
        {
            //만렙일경우
            //현재 레벨만 표기
            SkillView.FindChild("arrow").gameObject.SetActive(false);
            SkillView.FindChild("name").gameObject.SetActive(false);
            SkillLevelUpRoot.gameObject.SetActive(false);
            BtnSkillLevelUp.transform.gameObject.SetActive(false);
            CurSelectSkillLevel[1].transform.parent.gameObject.SetActive(false);
            //위치변경도 있어야함
            CurSelectSkilDamage[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(1166), GetSkillDamage(levelLowData.factorRate.list, (float)levelLowData.factor).ToString());
            CurSelectSkillLevel[0].transform.parent.transform.localPosition = new Vector3(0, -3, 0);
            SkillMaxLevel.SetActive(true);
        }

        CurSelectSkillLevel[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), selectSkill._skillLevel);
        CurSelectSkillLevel[1].gameObject.transform.localScale = Vector3.one;


    }

    /// <summary> 파트너 액티브스킬 레벨업 응답 </summary>
    public void OnPMsgHeroSkillUpgradeSHandler()
    {
        NetData._PartnerActiveSkillData selectSkill = CurData.GetBuffSkillToSlot((ushort)(SelectActiveSkillIndex + 1));
        if(selectSkill!=null)
        {
            UIHelper.CreateEffectInGame(SkillLevelUpEffRoot, "Fx_partner_skillup_01");

            CurSelectSkillLevel[1].GetComponent<TweenColor>().ResetToBeginning();
            CurSelectSkillLevel[1].GetComponent<TweenColor>().PlayForward();

            CurSelectSkillLevel[1].GetComponent<TweenScale>().ResetToBeginning();
            CurSelectSkillLevel[1].GetComponent<TweenScale>().PlayForward();

            TempCoroutine.instance.FrameDelay(1f, delegate ()
            {
                DestroyImmediate(SkillLevelUpEffRoot.GetChild(0).gameObject);
                //액티브 스킬 셋팅
                Dictionary<ushort, NetData._PartnerActiveSkillData> activeSkillList = CurData.ActiveSkillList;
                int activeCount = activeSkillList.Count;
                for (int i = 1; i < activeCount; i++)//0번은 평타.
                {
                    uint skillId = activeSkillList[(ushort)i]._skillIndex;
                    SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(skillId);
                    if (actionLowData == null)
                        continue;

                    SkillIcon[i - 1].spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
                    SkillName[i - 1].text = _LowDataMgr.instance.GetStringSkillName(actionLowData.name);
                    SkillLevel[i - 1].text = string.Format(_LowDataMgr.instance.GetStringCommon(1151), CurData.GetBuffSkillToSlot((ushort)i)._skillLevel);

                    int idx = i;
                    EventDelegate.Set(ActiveSkillTrigger[i - 1].onClick, delegate ()
                    {
                        OnClickActiveSkill(idx - 1, activeSkillList[(ushort)idx]);
                    });
                }

                OnClickActiveSkill(SelectActiveSkillIndex, selectSkill);
            });
         
        }
        else
        {
            Debug.LogError("CanNotFound SKillData");
        }
    }

    /// <summary>
    /// 스킬테이블에서 factorRate의평균값을 반환
    /// </summary>
    /// <returns></returns>
    float GetSkillDamage(List<string> factor1, float factor2)
    {
        float Damage = 0;
        for (int i = 0; i < factor1.Count; i++)
        {
            Damage += float.Parse(factor1[i]);
        }
        Damage /= factor1.Count;
        Damage += factor2;

        return Damage * 100;
    }

    /// <summary> 등급에 맞는 이미지를 준다. </summary>
    string GetGradeName(byte grade)
    {
        return string.Format("Icon_0{0}", grade);
    }

    #endregion 스킬 끝

    //원하는 뷰로 바꿔준다
    void ChangedView(ViewType type)
    {
        if(!CurData._isOwn)
        {
            if (CurTabIndex == type)
            {
                InitAttribute();
                return;
            }
        }


        PrevTabIndex = CurTabIndex;
        CurTabIndex = type;

        for (int i = 0; i < ViewObj.Length; i++)
        {
            ViewObj[i].SetActive(i == (int)type);
        }

        for (int i = 0; i < TabBtn.Length; i++)
        {
            TabBtn[i].transform.FindChild("tab_on").gameObject.SetActive(i == (int)type);
            TabBtn[i].transform.FindChild("tab_off").gameObject.SetActive(i != (int)type);
            TabBtn[i].transform.FindChild("tab_disable").gameObject.SetActive(false);
        }

        uint titleId = 0;

        CheckMoveState((int)type);

        switch (type)
        {
            case ViewType.Evolve:
                InitPromo();
                titleId = 32;
                break;
            case ViewType.Skill:
                SkillInit();
                titleId = 55;
                break;
            case ViewType.Attribute:
                InitAttribute();
                titleId = 1162;
                break;
        }

        UIMgr.instance.SetTopMenuTitleName(titleId);
    }

    #region 이벤트 버튼들 정의
    //눌려진 버튼들에 맞춰 뷰 보여주기
    void OnClickTabList(int type)
    {
        ChangedView((ViewType)type);
    }


    #endregion

    GameObject GetViewObj(ViewType type)
    {
        return ViewObj[(uint)type];
    }

    /// <summary> 이펙트중일때 뒤로가기 예외처리 </summary>
    public bool CheckClose()
    {
        if (IsUpgrading)
        {
            TempCoroutine.instance.RemoveKeyDelay("ParEvolve");
            IsUpgrading = false;
            DestroyImmediate(EvolveEffRoot.GetChild(0).gameObject);
        }

        return true;
    }

    public byte TabType
    {
        get
        {
            for (int i = 0; i < ViewObj.Length; i++)
            {
                if (!ViewObj[i].activeSelf)
                    continue;

                if (i == 0)//성장
                    return 4;
                else
                    return (byte)i;
            }

            return 0;
        }
    }
}

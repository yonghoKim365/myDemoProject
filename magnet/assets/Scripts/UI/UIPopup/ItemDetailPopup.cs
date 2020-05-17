using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDetailPopup : UIBasePanel
{
    public UILabel ItemName;//아이템의 이름
    public UILabel Desc;// 아이템 설명
    public UILabel CurCharLv;//현재 캐릭터 레벨
    //public UILabel EquipLv;//장착 가능레벨
    public UILabel[] SetItemName;// 셋트 아이템 이름들
    public UILabel[] SetItemAbility;// 셋트아이템 혜택들
    public UILabel[] Abilitys;// Ability 셋팅하는 라벨들

    public UIScrollView OptionScrollView;// Ability, Set, Description들의 스크롤뷰
    public UIScrollView LinkScroll;//Link Popup 0
    
    //public GameObject InvenItemObj;// ItemRoot하단에 생성될 아이템 오브젝트
    public GameObject[] EquipState;//0 착용가능, 1착용불가능
    public GameObject[] LinkPopup;//0 획득처, 1 조합
    
    public Transform ItemRoot;
    public Transform AbilityTf;
    public Transform SetTf;

    //이벤트 버튼들 정의
    public GameObject MainBtn;
    public GameObject BtnClose;
    
    private NetData._ItemData CurItemData;// 현재 등록되어 있는 아이템 데이터. 일단은 가지고 있어보자.
    private InvenItemSlotObject ItemSlot;// 아이템 슬롯 스크립트
    private InvenItemSlotObject[] LinkItemSlot = new InvenItemSlotObject[2];//소비아이템 아이템 슬롯 스크립트
    private UIBasePanel BasePanel;
    
    /// <summary> 초기화 함수 사용하기전 최초 한번 해야함 </summary>
    public override void LateInit()
    {
        base.LateInit();
        //bool isAutoQ = (bool)parameters[0];
        int depth = (int)parameters[1];

        if( !mStarted)//최초 한번
        {
            ItemSlot = UIHelper.CreateInvenSlot(ItemRoot).GetComponent<InvenItemSlotObject>();
            LinkItemSlot[0] = UIHelper.CreateInvenSlot(LinkPopup[0].transform.FindChild("TargetItem")).GetComponent<InvenItemSlotObject>();
            LinkItemSlot[1] = UIHelper.CreateInvenSlot(LinkPopup[1].transform.FindChild("TargetItem")).GetComponent<InvenItemSlotObject>();

            EventDelegate.Set(MainBtn.GetComponent<UIEventTrigger>().onClick, OnClickMainBtn);
            EventDelegate.Set(BtnClose.GetComponent<UIEventTrigger>().onClick, Hide);
            EventDelegate.Set(LinkPopup[0].transform.FindChild("BtnBack").GetComponent<UIEventTrigger>().onClick, Hide);
            EventDelegate.Set(LinkPopup[1].transform.FindChild("BtnBack").GetComponent<UIEventTrigger>().onClick, Hide);
        }

        if (BasePanel != (UIBasePanel)parameters[2])
        {
            GetComponent<UIPanel>().depth = depth;
            OptionScrollView.GetComponent<UIPanel>().depth = depth + 1;
            LinkScroll.GetComponent<UIPanel>().depth = depth + 1;

            BasePanel = (UIBasePanel)parameters[2];
        }
        
        if(parameters[3] != null )
            SetDetailPopup((NetData._ItemData)parameters[3]);
        else
            ObjectHide();
    }

    /// <summary> 여러군데에서 호출하니까 변환시키는 함수가 있으면 좋을 듯해서 만듦</summary>
    T GetPanel<T>() where T : UIBasePanel
    {
        T t = BasePanel as T;
        if(t == null)
            Debug.LogError(string.Format("can't change data type error" )) ;

        return t;
    }

    /// <summary> 아이템 디테일 팝업 실행 함수. </summary>
    void SetDetailPopup(NetData._ItemData itemData)
    {
        CurItemData = itemData;
        Debug.Log(string.Format("Click Item UseId={0}, equipId={1}", itemData._useitemDataIndex, itemData._equipitemDataIndex) );
        bool isDifferentClass = false;
        NetData._UserInfo charInven = NetData.instance.GetUserInfo();

        if (CurItemData.IsEquipItem())
        {
            LinkPopup[0].SetActive(false);
            LinkPopup[1].SetActive(false);

            int limitLv = CurItemData.GetEquipLowData().LimitLevel;
            //EquipLv.text = string.Format(_LowDataMgr.instance.GetStringCommon(952), limitLv);
            if (limitLv <= charInven._Level && !isDifferentClass)//내 직업이고 레벨이 높다면
            {
                EquipState[0].SetActive(true);
                EquipState[1].SetActive(false);
            }
            else
            {
                EquipState[0].SetActive(false);
                EquipState[1].SetActive(true);
            }

            uint strKey = 0;
            if (BasePanel is EquipmentPanel)
            {
                Item.EquipmentInfo equipLow = CurItemData.GetEquipLowData();
                if (CurItemData._enchant < equipLow.MaxEnchant)
                    strKey = 31;
                else if (0 < equipLow.NextPartsId )
                    strKey = 32;
                else//최대 
                    strKey = 1182;
            }
            else
                strKey = 106;//닫기

            if (0 < strKey)
                SetOnOff(MainBtn.transform, true, _LowDataMgr.instance.GetStringCommon(strKey));

            //텍스트 셋팅
            int grade = itemData.GetEquipLowData().Grade;
            string color = UIHelper.GetItemGradeColor(grade);
            ItemName.text = string.Format("{0}{1}[-] [FFE400]+{2}[-]", color, itemData.GetLocName(), itemData._enchant);
        }
        else
        {
            //EquipLv.text = "";
            EquipState[0].SetActive(false);
            EquipState[1].SetActive(false);
            LinkPopup[0].SetActive(true);
            LinkPopup[1].SetActive(false);

            SetOnOff(MainBtn.transform, true, _LowDataMgr.instance.GetStringCommon(106));
            int count = charInven.GetItemCountForItemId(CurItemData._useitemDataIndex, (byte)eItemType.USE);
            LinkItemSlot[0].SetLowDataItemSlot(CurItemData._useitemDataIndex, (uint)count);
            LinkItemSlot[0].transform.parent.FindChild("Lock").gameObject.SetActive(count <= 0);

            Transform gridTf = LinkScroll.transform.FindChild("Grid");
            List<Item.ContentsListInfo> conList = _LowDataMgr.instance.GetLowDataContentsItemList(CurItemData._useitemDataIndex);
            for (int i = 0; i < conList.Count; i++)
            {
                Item.ContentsListInfo info = conList[i];
                string contentsName = _LowDataMgr.instance.GetStringCommon(info.ContentsName);
                if (info.ContentsParam != null && 0 < info.ContentsParam.Count)
                {
                    if (contentsName.Contains("{0}") && contentsName.Contains("{1}"))
                        contentsName = string.Format(contentsName, info.ContentsParam[0], info.ContentsParam[1]);
                    else if (contentsName.Contains("{0}"))
                        contentsName = string.Format(contentsName, info.ContentsParam[0]);
                }

                Transform tf = null;
                if (i < gridTf.childCount)
                    tf = gridTf.GetChild(i);
                else
                {
                    tf = Instantiate(gridTf.GetChild(0)) as Transform;
                    tf.parent = gridTf;
                    tf.localScale = Vector3.one;
                }

                tf.gameObject.SetActive(true);

                tf.FindChild("txt").GetComponent<UILabel>().text = contentsName;
                bool isEnter = true;
                if (CheckContentsLink(info, false))//조건 만족
                    tf.FindChild("error_txt").GetComponent<UILabel>().text = "";
                else
                {
                    isEnter = false;

                    string contentsError = _LowDataMgr.instance.GetStringCommon(info.ConditionName);
                    if (info.ConditionParam != null && 0 < info.ConditionParam.Count)
                    {
                        if (contentsError.Contains("{0}") && contentsError.Contains("{1}"))
                            contentsError = string.Format(contentsError, info.ConditionParam[0], info.ConditionParam[1]);
                        else if (contentsError.Contains("{0}"))
                            contentsError = string.Format(contentsError, info.ConditionParam[0]);
                    }

                    tf.FindChild("error_txt").GetComponent<UILabel>().text = contentsError;
                }

                EventDelegate.Set(tf.FindChild("Btn").GetComponent<UIEventTrigger>().onClick, () => { OnClickContentsLink(info); });
                if (info.ContentsLinkType == 1)//재료조합
                {
                    tf.FindChild("Btn/On/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(39);
                    tf.FindChild("Btn/Off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(39);
                }
                else if (info.ContentsLinkType == 2)//바로가기(던전)
                {
                    tf.FindChild("Btn/On/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(249);
                    tf.FindChild("Btn/Off/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(249);
                }

                tf.FindChild("Btn/On").gameObject.SetActive(isEnter);
                tf.FindChild("Btn/Off").gameObject.SetActive(!isEnter);
            }

            gridTf.GetComponent<UIGrid>().repositionNow = true;
            for(int j=conList.Count; j < gridTf.childCount; j++)
            {
                gridTf.GetChild(j).gameObject.SetActive(false);
            }

            if (conList.Count < 4)
                LinkScroll.enabled = false;
            else
                LinkScroll.enabled = true;

            Item.ItemInfo data = itemData.GetUseLowData();
            string color = UIHelper.GetItemGradeColor(data == null ? 0 : (int)data.Grade );
            ItemName.text = string.Format("{0}{1}[-] [FFE400][-]", color, itemData.GetLocName() );
        }
        
        //아이템 셋팅
        ItemSlot.SetInvenItemSlot(itemData, null, 0);
        
        //아이템의 옵션들 셋팅
        byte optionCount = 0;
        Desc.transform.localPosition = SetAbility(itemData, ref optionCount);//어빌리티
        SetDesc(itemData, ref optionCount);//아이템 설명 표현
        
        OptionScrollView.ResetPosition();
        if (optionCount <= 1)//스크롤 되면 안됨.
            AbilityTf.parent.collider.enabled = false;
        else
            AbilityTf.parent.collider.enabled = true;
    }

    /// <summary> 조건 체크. </summary>
    bool CheckContentsLink(Item.ContentsListInfo info, bool isClick)
    {
        bool isEnter = false;
        switch (info.ContentsType)
        {
            case 1://조합

                break;
            case 11://차관
                if (_LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvpEnterLv) <= NetData.instance.UserLevel)
                    isEnter = true;
                break;
            case 12://난투장
                List<DungeonTable.FreefightTableInfo> freeList = _LowDataMgr.instance.GetLowDataFreeFightList();
                for (int i = 0; i < freeList.Count; i++)
                {
                    if (NetData.instance.UserLevel < freeList[i].MinenterLv || freeList[i].MaxenterLv < NetData.instance.UserLevel)
                        continue;

                    isEnter = true;
                    break;
                }
                
                break;

            case 2://모험 일반
            case 3://모험 어려움
                uint require = 0;
                NetData.ClearSingleStageData stageData = null;
                if (!NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(info.ContentsIdx, out stageData))
                {
                    DungeonTable.StageInfo stageInfo = _LowDataMgr.instance.GetStageInfo(info.ContentsIdx);
                    if (stageInfo != null && stageInfo.RequireStageId != null && 0 < stageInfo.RequireStageId.list.Count)
                    {
                        List<string> conList = stageInfo.RequireStageId.list;
                        for(int i=0; i < conList.Count; i++)
                        {
                            uint conIdx = uint.Parse(conList[i]);
                            if (NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(conIdx, out stageData))//이전 스테이지 클리어해야함.
                                continue;

                            require = 1;//진행 불가능
                            break;
                        }
                    }
                }

                if (require <= 0)
                    isEnter = true;

                break;
            case 5://보스레이드
                DungeonTable.SingleBossRaidInfo bossRaid = _LowDataMgr.instance.GetSingleBossRaidLimitLevel(info.ContentsIdx);
                if (bossRaid.levelLimit <= NetData.instance.UserLevel)
                    isEnter = true;
                break;
            case 6://멀티 보스레이드
                DungeonTable.MultyBossRaidInfo multyBossRaid = _LowDataMgr.instance.GetLowDataMultyBossInfo(info.ContentsIdx);
                if (multyBossRaid.levelLimit <= NetData.instance.UserLevel)
                    isEnter = true;
                break;
            case 10://길드
                if (0 < NetData.instance.GetUserInfo()._GuildId)
                    isEnter = true;
                break;

            case 4://마계의탑
            case 7://콜로세움
            case 8://골드 던전
            case 9://경험치 던전
                byte idx = 0;
                if (info.ContentsType == 4)
                    idx = (byte)9;
                else if (info.ContentsType == 7)
                    idx = (byte)1;
                else if (info.ContentsType == 8)
                    idx = (byte)3;
                else if (info.ContentsType == 9)
                    idx = (byte)2;

                DungeonTable.ContentsOpenInfo content = _LowDataMgr.instance.GetFirstContentsOpenInfo(idx);
                if (content.ConditionType1 == 2 && content.ConditionValue1 <= NetData.instance.GetUserInfo()._TotalAttack)
                    isEnter = true;
                else if (content.ConditionType1 == 1 && content.ConditionValue1 <= NetData.instance.UserLevel)
                    isEnter = true;
                break;
            }

        if (!isEnter)
        {
            if(isClick)
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(712));
            return false;
        }
        
        return true;
    }

    void OnClickContentsLink(Item.ContentsListInfo info)
    {
        if (info.ContentsLinkType == 1)//재료조합
        {

        }
        else if (info.ContentsLinkType == 2)//바로가기(던전)
        {
            if (!CheckContentsLink(info, true))
                return;

            bool isBasePanelHide = true;
            switch (info.ContentsType)
            {
                case 1://조합

                    break;
                case 11://차관
                    UIMgr.OpenArenaPanel();
                    break;
                case 12://난투장
                    UIMgr.OpenDogFight();
                    break;

                case 2://모험 일반
                case 3://모험 어려움
                    if (BasePanel != null && BasePanel is ChapterPanel)
                    {
                        isBasePanelHide = false;
                        (BasePanel as ChapterPanel).SetStageInfoPopup(info.ContentsIdx);
                    }

                    if(isBasePanelHide)
                        UIMgr.OpenChapter(null,info.ContentsIdx);
                    break;

                case 4://마계의탑
                    UIMgr.OpenTowerPanel(info.ContentsIdx);
                    break;
                case 5://보스레이드
                    DungeonTable.SingleBossRaidInfo bossRaid =_LowDataMgr.instance.GetSingleBossRaidLimitLevel(info.ContentsIdx);
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.RAID, bossRaid.Type);
                    break;
                case 6://멀티 보스레이드
                    DungeonTable.MultyBossRaidInfo multyBossRaid = _LowDataMgr.instance.GetLowDataMultyBossInfo(info.ContentsIdx);
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.MULTI_RAID, multyBossRaid.Type);
                    break;
                case 7://콜로세움
                    UIMgr.OpenColosseumPanel(0);
                    break;
                case 8://골드 던전
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_GOLD, 0);
                    break;
                case 9://경험치 던전
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_EXP, 0);
                    break;
                case 10://길드
                    UIMgr.OpenGuildPanel();
                    break;
            }
            
            if (isBasePanelHide && BasePanel != null)
                BasePanel.ObjectHide();

            Hide();
        }
    }
    
    /// <summary> 종료  </summary>
    public override void Hide()
    {
        if (LinkPopup[1].activeSelf)
        {
            LinkPopup[0].SetActive(true);
            LinkPopup[1].SetActive(false);
        }
        else
        {
            base.Hide();
        }
    }
    public override void ObjectHide()
    {
        for (int i = 0; i < LinkPopup.Length; i++)
        {
            LinkPopup[i].SetActive(false);
        }

        base.ObjectHide();
    }
    
    /// <summary> 아이템의 옵션정보 표기 </summary>
    Vector3 SetAbility(NetData._ItemData itemData, ref byte optionCount)
    {
        for(int i=0; i < Abilitys.Length; i++)
        {
            Abilitys[i].gameObject.SetActive(false);
        }

        int abilityCount = 0;
        GameObject line = AbilityTf.FindChild("line_d0").gameObject;
        if (itemData.IsEquipItem())//장비 아이템
        {
            NetData._ItemData mountItemData = NetData.instance.GetUserInfo().GetEquipParts(itemData.EquipPartType);
            bool isCompare = false;
            if (mountItemData != null)//같은 파츠이면서 착용한 아이템과 다르다면 비교한다.
            {
                if(mountItemData.GetEquipLowData().Class == itemData.GetEquipLowData().Class)//같은 직업군인지
                    isCompare = mountItemData._itemIndex != itemData._itemIndex;
            }

            List<NetData.ItemAbilityData> statList = itemData.StatList;
            abilityCount = statList.Count;
            for(int i=0; i < abilityCount; i++)
            {
                //float value = (statList[i].Value * 0.1f) + (itemData._enchant + ((itemData._Grade * 10) + itemData._MinorGrade));
                int value = _LowDataMgr.instance.GetItemAbilityValueToInt(statList[i].Value, itemData._enchant);
                Abilitys[i].text = string.Format("{0} {1}", uiMgr.GetAbilityLocName(statList[i].Ability), value );
                Abilitys[i].gameObject.SetActive(true);
            }
        }
        else//사용 아이템
        {
            abilityCount = itemData.StatList.Count;
            for(int i=0; i < abilityCount; i++)
            {
                NetData.ItemAbilityData data =  itemData.StatList[i];
                Abilitys[i].text = string.Format("{0} {1}", uiMgr.GetAbilityLocName(data.Ability)
                    , uiMgr.GetAbilityStrValue(data.Ability, data.Value*0.1f));
                Abilitys[i].gameObject.SetActive(true);
            }
        }

        if (abilityCount == 0)//Option이 없다. 꺼놓고 자신의 좌표를 준다.
        {
            AbilityTf.gameObject.SetActive(false);
            return AbilityTf.localPosition;
        }

        ++optionCount;
        ++abilityCount;

        AbilityTf.gameObject.SetActive(true);

        ///Line 그리기
        Transform lineTf = AbilityTf.FindChild("line_d0");
        Vector3 linePos = lineTf.localPosition;
        linePos.y = -(abilityCount * 27);
        lineTf.localPosition = linePos;

        ///다음에 올 객체에 관한 포지션 값
        Vector3 pos = AbilityTf.localPosition;
        pos.y += (linePos.y - 27);
        return pos;
    }
    
    /// <summary> 사용 아이템의 설명문 Equip아이템은 아님. 마지막 셋팅 함수임. Position은 주지 않는다 </summary>
    void SetDesc(NetData._ItemData itemData, ref byte optionCount)
    {
        uint descId = 0;
        if (itemData.IsEquipItem())
        {
            Item.EquipmentInfo equipLowData = itemData.GetEquipLowData();
            descId = equipLowData.DescriptionId;
        }
        else
        {
            Item.ItemInfo useLowData = itemData.GetUseLowData();
            descId = useLowData == null ? 0 : useLowData.DescriptionId;
        }

        if(descId <= 0 )
        {
            Desc.gameObject.SetActive(false);
            return;
        }

        ++optionCount;
        Desc.gameObject.SetActive(true);
        Desc.text = _LowDataMgr.instance.GetStringItem(descId);
    }

    /// <summary> 보고 있던 아이템의 인덱스 및 아이디를 줌. </summary>
    public void CurrentDataIdxAndId(ref ulong idx, ref uint id)
    {
        if (CurItemData == null)
            return;

        idx = CurItemData._itemIndex;
        if (CurItemData.IsEquipItem())
            id = CurItemData._equipitemDataIndex;
        else if (CurItemData.IsUseItem())
            id = CurItemData._useitemDataIndex;
    }
    
    void SetOnOff(Transform tf, bool isOn, string str)
    {
        for(int i=0; i < tf.childCount; i++)
        {
            if (tf.GetChild(i).name.Contains("on"))
            {
                tf.GetChild(i).gameObject.SetActive(isOn);
                if (isOn)
                    tf.GetChild(i).FindChild("lbl").GetComponent<UILabel>().text = str;
            }
            else
            {
                tf.GetChild(i).gameObject.SetActive(!isOn);
                if ( !isOn)
                    tf.GetChild(i).FindChild("lbl").GetComponent<UILabel>().text = str;
            }
        }
    }

    void OnClickMainBtn()
    {
        if (CurItemData == null)
            return;

        if (CurItemData.IsEquipItem() && BasePanel is EquipmentPanel)
        {
            if (CurItemData._enchant < CurItemData.GetEquipLowData().MaxEnchant)
            {
                GetPanel<EquipmentPanel>().OnChangeTabUpgrade(true, CurItemData);
                Hide();
            }
            else if (0 < CurItemData.GetEquipLowData().NextPartsId)
            {
                GetPanel<EquipmentPanel>().OnChangeTabUpgrade(false, CurItemData);
                Hide();
            }
            else
            {
                GetPanel<EquipmentPanel>().OnChangeTabUpgrade(true, CurItemData);
                Hide();
                //SceneManager.instance.SetNoticePanel(NoticeType.Message, 1182);
            }
        }
        else
            Hide();
    }
}

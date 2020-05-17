using UnityEngine;
using System.Collections.Generic;

public class ReadyPopup : UIBasePanel
{

    /// <summary>
    /// 파트너 슬롯 정보 넣거나 뺄때 사용
    /// </summary>
    class PartnerRootInfo
    {
        public bool isInsert;//삽입이 되었는지
        public int insertSlotID = -1;//어떤 슬롯에서 가져온지 알기 위한 값

        public void SetModel(int slotID)
        {
            isInsert = true;
            insertSlotID = slotID;
        }

        public void DeleteSlot()
        {
            isInsert = false;
            insertSlotID = -1;
        }
    }

    public Transform PlayCharRoot;// 플레이 중인 캐릭터 보여줄 곳
    public Transform[] PartnerModelRoot;// 파트너 슬롯, 파트너 삽입하면 보여줄 곳 배열은 0~1
    public Transform CostumGrid;// 코스튬 목록 아이템 셋팅할때 사용한다.
    public Transform CostumMountIconTf;// 코스튬리스트에서 장착 아이콘
    public Transform CostumeOriginalSlotTf;// 코스튬 슬롯 원본
    public Transform PartnerGrid;// 보유중인 파트너 슬롯 보여줌(파트너 슬롯 클릭시)
    public Transform NetInfoTf;//멀티보스레이드, 콜로세움에서 나올 특별 유아이
    
    public GameObject PartnerSlotPrefab;
    public GameObject NotPartner;//파트너 없음
    public GameObject[] PnTouchObj;//파트너의 터치라벨 키고 끄려고 만듦
    public GameObject[] PnTouchEff;

    public UILabel[] TakeParNames;

    public bool IsRoom;

    private GAME_MODE GameMode;

    private PartnerRootInfo[] PnRootInfo = new PartnerRootInfo[2];// 파트너 들어갈 슬롯 데이터 정보 SelectPnRootID 값 사용
    private NetData._UserInfo CharInven;
    private UIBasePanel BasePanel;

    private int SelectPnRootID;// 파트너 자리 아이디 (0, 1 이다.) 서버로는 1, 2로.
    //private bool IsTownInvite;//마을 유저 초대

    public override void Init()
    {
        base.Init();

        //PartnerSlotList.SetActive(false);
        PartnerSlotPrefab.SetActive(false);
        CharInven = NetData.instance.GetUserInfo();

        //이벤트 버튼 설정
        UIButton uiBtnStart = transform.FindChild("BtnStartGame").GetComponent<UIButton>();
        EventDelegate.Set(uiBtnStart.onClick, OnClickStartGame);
        
        EventDelegate.Set(onShow, () => {
            mStarted = false;//onShow에서 두번 실행 못하게 막아버린다
            
            uiMgr.OpenTopMenu(this);
        });


        //PartnerRoot 정보 초기화
        PnRootInfo[0] = new PartnerRootInfo();
        PnRootInfo[1] = new PartnerRootInfo();
        TakeParNames[0].text = "";
        TakeParNames[1].text = "";
        PnTouchObj[0].SetActive(false);
        PnTouchObj[1].SetActive(false);

        CostumeOriginalSlotTf.gameObject.SetActive(false);
        //코스튬 셋팅
        Color colrA = new Color(1, 1, 1, 0.3f);
        List<NetData._CostumeData> dataList = CharInven.GetCostumeList();
        int loopCount = dataList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            NetData._CostumeData data = dataList[i];
            if (!data._isOwn)
                continue;

            Transform slotTf = null;
            if (i < CostumGrid.childCount)
                slotTf = CostumGrid.GetChild(i);
            else
            {
                slotTf = Instantiate(CostumeOriginalSlotTf) as Transform;
                slotTf.gameObject.SetActive(true);
                slotTf.parent = CostumGrid;
                slotTf.localPosition = Vector3.zero;
                slotTf.localScale = Vector3.one;
            }

            slotTf.name = string.Format("{0}", data._costmeDataIndex);
            slotTf.gameObject.SetActive(true);

            UILabel nameLb = slotTf.FindChild("state_d3").GetComponent<UILabel>();
            nameLb.text = data._isEquip ? _LowDataMgr.instance.GetStringCommon(37) : _LowDataMgr.instance.GetStringCommon(38);//("장착중"), ("장착하기")
            /*//2017.11.13 스킬 셋교체 시 작업.
            //스킬 셋팅
            List<uint> skillList = data.GetSkillList();
            int skillCount = skillList.Count;
            for (int j = 1; j < skillCount; j++)
            {
                SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(skillList[j]);
                UISprite sp = slotTf.FindChild(string.Format("skills/{0}/icon_d4", j - 1)).GetComponent<UISprite>();
                sp.spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
            }
            */
            UISprite spIcon = slotTf.FindChild("btn_insert/icon_d4").GetComponent<UISprite>();
            UISprite spBg = slotTf.FindChild("bg").GetComponent<UISprite>();
            spIcon.spriteName = data.GetIconName();

            slotTf.FindChild("mount").gameObject.SetActive(data._isEquip);
            if (data._isEquip)
            {
                CostumMountIconTf.parent = slotTf;
                CostumMountIconTf.localPosition = Vector3.zero;
                CostumMountIconTf.localScale = Vector3.one;

                spBg.color = Color.white;
            }
            else
                spBg.color = colrA;

            slotTf.FindChild("btn_insert").collider.enabled = !data._isEquip;
            UIEventTrigger uiTri = slotTf.FindChild("btn_insert").GetComponent<UIEventTrigger>();
            EventDelegate.Set(uiTri.onClick, delegate () { OnClickChangeCostum((int)data._costumeIndex); });
        }

        CostumGrid.GetComponent<UIGrid>().Reposition();
    }

    public override void LateInit()
    {
        base.LateInit();

        GameMode = (GAME_MODE)parameters[0];
        BasePanel = (UIBasePanel)parameters[1];
        int energe = (int)parameters[2];
        int max = (int)parameters[3];
        float scale = PartnerModelRoot[0].localScale.x;//파트너는 이값으로

        bool isNetwork = false;
        string startBtnLbl = null;
        switch (GameMode)
        {
            case GAME_MODE.SINGLE:
                {
                    //모험모드일때만 체력표시되게해주세요 
                    UIMgr.instance.TopMenu.MenuType[3].transform.parent.gameObject.SetActive(true);
                    startBtnLbl = string.Format("{0} ({1} {2})", _LowDataMgr.instance.GetStringCommon(66), energe, _LowDataMgr.instance.GetStringCommon(1));//("시작"), ("에너지")
                    break;
                }
               

            case GAME_MODE.SPECIAL_EXP:
            case GAME_MODE.SPECIAL_GOLD:
            case GAME_MODE.RAID:
            case GAME_MODE.TOWER:
                startBtnLbl = string.Format("{0}/{1} {2}", max - energe, max, _LowDataMgr.instance.GetStringCommon(66));
                break;

            case GAME_MODE.COLOSSEUM:
            case GAME_MODE.MULTI_RAID:
                isNetwork = true;
                startBtnLbl = string.Format("{0}/{1} {2}", max - energe, max, _LowDataMgr.instance.GetStringCommon(66));
                scale = 0.8f;//플레이어는 이값으로
                break;

            case GAME_MODE.ARENA:
                startBtnLbl = string.Format("{0}", _LowDataMgr.instance.GetStringCommon(1053));
                //transform.FindChild("BtnStartGame").collider.enabled = false;
                break;

            default:
                startBtnLbl = "unDefined 0";
                break;
        }
        
        NetInfoTf.gameObject.SetActive(isNetwork);
        transform.FindChild("Partner").gameObject.SetActive(!isNetwork);
        transform.FindChild("BtnStartGame").gameObject.SetActive(!isNetwork);//상황따라 꺼놓는다.

        PartnerModelRoot[0].localScale = new Vector3(scale, scale, scale);
        PartnerModelRoot[1].localScale = new Vector3(scale, scale, scale);

        IsRoom = isNetwork;
        if (isNetwork)
        {
            InitNetworkDungeon(startBtnLbl);
        }
        else
        {
            transform.FindChild("BtnStartGame/need_food").GetComponent<UILabel>().text = startBtnLbl;

            //캐릭터 닉네임, 레벨 셋팅
            string nickName = NetData.instance.Nickname;
            string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
            transform.FindChild("Character/CharView/CharSlot/player_name").GetComponent<UILabel>().text =
                string.Format("{0} {1}", lvStr, nickName);

            //플레이어 생성
            NetData._CostumeData equipCostumeData = CharInven.GetEquipCostume();
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

            UIHelper.CreatePcUIModel("ReadyPopup",PlayCharRoot, CharInven.GetCharIdx(), headId, equipCostumeData._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 0, CharInven.isHideCostum, false);
        }
        
        if (GameMode != GAME_MODE.COLOSSEUM && GameMode != GAME_MODE.MULTI_RAID)//파트너못대려가는 지역.
        {
            Transform parBtnTf = transform.FindChild("Character/CharView");
            EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_0").GetComponent<UIEventTrigger>().onClick, delegate () { OnClickPartnerRoot(0); });
            EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_1").GetComponent<UIEventTrigger>().onClick, delegate () { OnClickPartnerRoot(1); });
            //CanNotTakePartner.SetActive(false);

            //슬롯 셋팅
            int ownCount = 0;
            List<NetData._PartnerData> dataList = CharInven.GetPartnerList();
            if (PartnerGrid.childCount < dataList.Count)
            {
                int loopCount = dataList.Count;
                for (int i = 0; i < loopCount; i++)
                {
                    Transform slotTf = null;
                    if (i < PartnerGrid.childCount)
                        slotTf = PartnerGrid.GetChild(i);
                    else
                    {
                        GameObject slotGo = Instantiate(PartnerSlotPrefab) as GameObject;
                        slotTf = slotGo.transform;
                        slotTf.parent = PartnerGrid;
                        slotTf.localPosition = Vector3.zero;
                        slotTf.localScale = Vector3.one;
                    }

                    NetData._PartnerData data = dataList[i];
                    if (!data._isOwn)
                    {
                        slotTf.gameObject.SetActive(false);
                        continue;
                    }

                    slotTf.name = string.Format("{0}", data._partnerDataIndex);
                    ++ownCount;
                    slotTf.gameObject.SetActive(true);

                    UIButton uiBtn = slotTf.GetComponent<UIButton>();
                    EventDelegate.Set(uiBtn.onClick, delegate ()
                    {
                        OnClickAddPartner(data);
                    });

                    UISprite faceIcon = slotTf.FindChild("face_d3").GetComponent<UISprite>();
                    UISprite gradeIcon = slotTf.FindChild("grade").GetComponent<UISprite>();
                    UISprite gradebgIcon = slotTf.FindChild("grade_bg").GetComponent<UISprite>();
                    UILabel nameLbl = slotTf.FindChild("partner_name_d4").GetComponent<UILabel>();
                    nameLbl.text = data.GetLocName();
                    faceIcon.spriteName = data.GetIcon();
                    gradeIcon.spriteName = string.Format("Icon_0{0}", data.CurQuality);
                    gradebgIcon.spriteName = string.Format("Icon_bg_0{0}", data.CurQuality);

                    if (!data._isEquip)//장착중이 아니면
                    {
                        slotTf.FindChild("face_d3").GetComponent<UISprite>().color = Color.white;
                        continue;
                    }

                    //장착중인 것 셋팅
                    //등록한 파트너 슬롯 딤처리
                    slotTf.FindChild("face_d3").GetComponent<UISprite>().color = Color.gray;
                    PnRootInfo[data._SlotNumber - 1].SetModel(data._partnerDataIndex);
                }

                UIScrollView scroll = PartnerGrid.parent.GetComponent<UIScrollView>();
                if (4 < ownCount)
                    scroll.enabled = true;
                else
                {
                    if (ownCount <= 0)
                        NotPartner.SetActive(true);
                    else
                        NotPartner.SetActive(false);

                    scroll.enabled = false;
                }

                if (0 < ownCount)
                {
                    GameObject effGo_01 = UIHelper.CreateEffectInGame(PnTouchEff[0].transform, "Fx_UI_partner_select_01", false);
                    GameObject effGo_02 = UIHelper.CreateEffectInGame(PnTouchEff[1].transform, "Fx_UI_partner_select_01", false);
                    effGo_01.transform.localEulerAngles = Vector3.zero;
                    effGo_02.transform.localEulerAngles = Vector3.zero;

                    effGo_01.layer = LayerMask.NameToLayer("UI");
                    effGo_02.layer = LayerMask.NameToLayer("UI");
                    effGo_01.transform.SetChildLayer(LayerMask.NameToLayer("UI"));
                    effGo_02.transform.SetChildLayer(LayerMask.NameToLayer("UI"));

                    PnTouchEff[0].SetActive(true);
                    PnTouchEff[1].SetActive(false);
                }
            }
            
            //장착중인 파트너 생성
            NetData._PartnerData partner_0 = CharInven.GetEquipPartner(1);
            NetData._PartnerData partner_1 = CharInven.GetEquipPartner(2);
            if (partner_0 != null)
            {
                Transform modelRoot = PartnerModelRoot[0];
                PnTouchObj[0].SetActive(false);//터치 라벨 끈다.

                UIHelper.CreatePartnerUIModel(modelRoot, partner_0._partnerDataIndex, 3, true, false, "ReadyPopup");
                string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), partner_0._NowLevel);
                TakeParNames[0].text = string.Format("{0} {1}", lvStr, partner_0.GetLocName());
            }
            else if(0 < ownCount)
                PnTouchObj[0].SetActive(true);//터치 라벨 킨다.
            //else
            //{
            //    PnTouchObj[0].SetActive(true);//터치 라벨 킨다.
            //    TakeParNames[0].text = "";
            //}

            if (partner_1 != null)
            {
                Transform modelRoot = PartnerModelRoot[1];
                PnTouchObj[1].SetActive(false);//터치 라벨 끈다.

                UIHelper.CreatePartnerUIModel(modelRoot, partner_1._partnerDataIndex, 3, true, false , "ReadyPopup");
                string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), partner_1._NowLevel);
                TakeParNames[1].text = string.Format("{0} {1}", lvStr, partner_1.GetLocName());
            }
            else if (0 < ownCount)
                PnTouchObj[1].SetActive(true);//터치 라벨 킨다.

            //else
            //{
            //    PnTouchObj[1].SetActive(true);//터치 라벨 킨다.
            //    TakeParNames[1].text = "";
            //}

        }
        else
        {
            NotPartner.SetActive(false);
        }
        
    }


    public override void Close()
    {
        if (GameMode == GAME_MODE.COLOSSEUM)
        {
            NetworkClient.instance.SendPMsgColosseumLeaveRoomC(NetData.instance.GameRoomData.RoomId );
        }
        else if(GameMode == GAME_MODE.MULTI_RAID)
        {
            NetworkClient.instance.SendPMsgMultiBossLeaveRoomC(NetData.instance.GameRoomData.RoomId);
        }
        else
        {
            base.Close();
            if (BasePanel != null)
            {
                BasePanel.OnCloseReadyPopup();
            }
        }
    }

    public void OnClose()
    {
        base.Close();
    }

    public override int GetUILayer()
    {
        return gameObject.layer;
    }
    
    /// <summary> 콜로세움, 멀티보스레이드 </summary>
    void InitNetworkDungeon(string startBtnLbl)
    {
        NetInfoTf.FindChild("BtnStart/need_food").GetComponent<UILabel>().text = startBtnLbl;

        uint stageId = (uint)parameters[4];
        uint limitPower = 0;
        if (GameMode == GAME_MODE.COLOSSEUM)
        {
            DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo(stageId);
            limitPower = coloInfo.FightingPower;
        }
        else
        {
            DungeonTable.MultyBossRaidInfo multyInfo = _LowDataMgr.instance.GetLowDataMultyBossInfo(stageId);
            limitPower = multyInfo.FightingPower;
        }

        NetInfoTf.FindChild("BattlePoint/my").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(725), CharInven._TotalAttack);
        NetInfoTf.FindChild("BattlePoint/limit").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(724), limitPower);
        
        Transform parBtnTf = transform.FindChild("Character/CharView");
        //if (NetData.instance.GameRoomData.OwnerId == CharInven.GetCharUUID() )//내가 방장
        NetData.RoomData roomData = NetData.instance.GameRoomData;
        if (roomData.IsLeader )//내가 방장
        {
            EventDelegate.Set(NetInfoTf.FindChild("BtnStart").GetComponent<UIButton>().onClick, OnClickStartGame);
            EventDelegate.Set(NetInfoTf.FindChild("BtnTown").GetComponent<UIButton>().onClick, OnInviteTown);
            EventDelegate.Set(NetInfoTf.FindChild("BtnInvite").GetComponent<UIButton>().onClick, () => {
                //IsTownInvite = false;
                UIMgr.OpenInvitePopup(GameMode);
                //Hide();
            });

            //캐릭터 닉네임, 레벨 셋팅
            string nickName = NetData.instance.Nickname;
            string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
            transform.FindChild("Character/CharView/CharSlot/player_name").GetComponent<UILabel>().text =
                string.Format("{0} {1}", lvStr, nickName);

            //플레이어 생성
            NetData._CostumeData equipCostumeData = CharInven.GetEquipCostume();
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

            UIHelper.CreatePcUIModel("ReadyPopup"
                , PlayCharRoot
                , CharInven.GetCharIdx()
                , headId
                , equipCostumeData._costmeDataIndex
                , clothId
                , weaponId
                , CharInven.GetEquipSKillSet().SkillSetId
                , 3
                , CharInven.isHideCostum
                , false);

            EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_0").GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickUserSlot(0);
            });
            EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_1").GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickUserSlot(1);
            });

            PnTouchObj[0].SetActive(true);
            PnTouchObj[1].SetActive(true);
        }
        else//파티 참가자
        {
            NetInfoTf.FindChild("BtnStart").collider.enabled = false;
            NetInfoTf.FindChild("BtnTown").collider.enabled = false;
            NetInfoTf.FindChild("BtnInvite").collider.enabled = false;

            parBtnTf.FindChild("BtnPartnerSlot_0").collider.enabled = false;
            parBtnTf.FindChild("BtnPartnerSlot_1").collider.enabled = false;

            PnTouchObj[0].SetActive(false);
            PnTouchObj[1].SetActive(false);

            //캐릭터 닉네임, 레벨 셋팅
            //방장 셋팅
            string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), roomData.Owner.Lv);
            transform.FindChild("Character/CharView/CharSlot/player_name").GetComponent<UILabel>().text =
                string.Format("{0} {1}", lvStr, roomData.Owner.Name);

            NetworkClient.instance.SendPMsgQueryRoleInfoC((long)roomData.Owner.Id);
            
            for(int i=0; i < roomData.UserList.Count; i++)//파티원들 셋팅
            {
                int arr = roomData.UserList[i].Slot;
                string lv = string.Format(_LowDataMgr.instance.GetStringCommon(453), roomData.UserList[i].Lv);
                TakeParNames[arr].text = string.Format("{0} {1}", lv, roomData.UserList[i].Name);

                if (roomData.UserList[i].Id == CharInven.GetCharUUID())//나
                {
                    //플레이어 생성
                    NetData._CostumeData equipCostumeData = CharInven.GetEquipCostume();
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

                    UIHelper.CreatePcUIModel("ReadyPopup"
                        , PartnerModelRoot[arr]
                        , CharInven.GetCharIdx()
                        , headId
                        , equipCostumeData._costmeDataIndex
                        , clothId
                        , weaponId
                        , CharInven.GetEquipSKillSet().SkillSetId
                        , 3
                        , CharInven.isHideCostum
                        , false);
                }
                else
                {
                    NetworkClient.instance.SendPMsgQueryRoleInfoC((long)roomData.UserList[i].Id);
                }
            }
        }
        
    }

    #region 이벤트 버튼

    /// <summary> 파트너 추가함 </summary>
    void OnClickAddPartner(NetData._PartnerData data)
    {
        int lenght = PnRootInfo.Length;
        for (int i = 0; i < lenght; i++)
        {
            if (PnRootInfo[i] == null)
                continue;

            if (PnRootInfo[i].isInsert && data._partnerDataIndex == PnRootInfo[i].insertSlotID)//장착중이거나 장착중인 녀석과 동일하다면 무시.
                return;
        }

        //파트너 장착
        CharInven.UnEquipPartner(SelectPnRootID + 1);
        CharInven.EquipPartner(data, SelectPnRootID + 1);

        Transform modelRoot = PartnerModelRoot[SelectPnRootID];
        PnTouchObj[SelectPnRootID].SetActive(false);//터치 라벨 끈다.

        UIHelper.CreateEffectInGame(PnTouchObj[SelectPnRootID].transform.parent, "Fx_UI_par_insert");   //교체이펙트

        if (PnRootInfo[SelectPnRootID].isInsert)
        {
            //사용하던 슬롯 다시 사용가능하게 해준다.
            Transform prevSlotTf = PartnerGrid.FindChild(string.Format("{0}", PnRootInfo[SelectPnRootID].insertSlotID));
            prevSlotTf.FindChild("face_d3").GetComponent<UISprite>().color = Color.white;
        }

        //등록한 파트너 슬롯 딤처리
        Transform slotTf = PartnerGrid.FindChild(string.Format("{0}", data._partnerDataIndex));
        slotTf.FindChild("face_d3").GetComponent<UISprite>().color = Color.gray;

        //파트너 이름, level 넣는다.
        string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), data._NowLevel);
        TakeParNames[SelectPnRootID].text = string.Format("{0} {1}", lvStr, data.GetLocName());

        UIHelper.CreatePartnerUIModel(modelRoot, data._partnerDataIndex, 3, true, false, "ReadyPopup");

        PnRootInfo[SelectPnRootID].SetModel(data._partnerDataIndex);
        //PartnerSlotList.SetActive(false);

    }

    /// <summary> 파트너 슬롯 버튼 </summary>
    void OnClickPartnerRoot(int rootID)
    {
        PnTouchEff[SelectPnRootID].SetActive(false);
        PnTouchEff[rootID].SetActive(true);
        SelectPnRootID = rootID;

        if (PnRootInfo[rootID].isInsert)//이미 무언가가 들어가 있음 뺀다.
        {
            NetData._PartnerData partnerData = CharInven.GetEquipPartner(rootID + 1);
            CharInven.UnEquipPartner(rootID + 1);

            //사용하던 슬롯 다시 사용가능하게 해준다.
            Transform slotTf = PartnerGrid.FindChild(string.Format("{0}", PnRootInfo[rootID].insertSlotID));
            slotTf.FindChild("face_d3").GetComponent<UISprite>().color = Color.white;

            Debug.Log("<color=blue/>DisMount slotID " + PnRootInfo[rootID].insertSlotID);

            PnRootInfo[rootID].DeleteSlot();//슬롯 정보 삭제
            PnTouchObj[rootID].SetActive(true);//터치 라벨 킨다.

            if (0 < PartnerModelRoot[rootID].childCount)
                PartnerModelRoot[rootID].GetChild(0).gameObject.SetActive(false);

            TakeParNames[rootID].text = null;
        }
        else//아무것도 없음 추가해준다.
        {
            //PartnerSlotList.SetActive(true);
            UIGrid grid = PartnerGrid.GetComponent<UIGrid>();
            grid.enabled = true;
            grid.Reposition();
        }

    }

    /// <summary> 게임시작 버튼 </summary>
    void OnClickStartGame()
    {
        if (BasePanel == null)
        {
            Debug.LogError(string.Format("Current GameMode {0} is UIBasePanel Null Error!", GameMode));
            return;
        }

        /*
        if (BasePanel.name.Contains("Arena"))  //차관에서는 다시 돌아가야함
        {
            Close();
            return;
        }
        */

        BasePanel.GotoInGame();
        //base.Close();
    }

    /// <summary> 게임 시작전에 코스튬을 변경함 </summary>
    void OnClickChangeCostum(int costumeIdx)
    {
        NetworkClient.instance.SendPMsgCostumeUserC(costumeIdx, 1);
    }

    /// <summary> 마을 유저 초대 </summary>
    public void OnInviteTown()
    {
        TownState town = SceneManager.instance.GetState<TownState>();
        if (town == null)
            return;
        
        if (GameMode == GAME_MODE.COLOSSEUM)
            NetworkClient.instance.SendPMsgColosseumInviteC(1, null);
        else
            NetworkClient.instance.SendPMsgMultiBossInviteC(1, null);
    }

    /// <summary> 방장한테만 권한있는 유저 슬롯 </summary>
    void OnClickUserSlot(int arr)
    {
        //if (CharInven.GetCharUUID() != NetData.instance.GameRoomData.OwnerId)//뭔가 잘못됨
        if(NetData.instance.GameRoomData.Owner != null)//방장이 아님
            return;

        ulong roleId = NetData.instance.GetGameRoomUserRoleId(arr);
        if (roleId == 0)//비었음
        {
            //IsTownInvite = false;
            UIMgr.OpenInvitePopup(GameMode);
            //Hide();
        }
        else
        {
            if (GameMode == GAME_MODE.COLOSSEUM)
                NetworkClient.instance.SendPMsgColosseumKickRoomC(roleId);
            else
                NetworkClient.instance.SendPMsgMultiBossKickRoomC(roleId);
        }
    }

    #endregion


    #region 서버 응답 이벤트

    /// <summary> 코스튬 장착 응답 </summary>
    public void OnPMsgCostume(NetData._CostumeData costume)
    {
        Transform modelTf = null;
        if (GameMode == GAME_MODE.COLOSSEUM || GameMode == GAME_MODE.MULTI_RAID)
        {
            //if (NetData.instance.GameRoomData.OwnerId == CharInven.GetCharUUID())
            if(NetData.instance.GameRoomData.Owner == null)//내가 방장
                modelTf = PlayCharRoot;
            else
            {
                int arr = NetData.instance.GetGameRoomUserArr(CharInven.GetCharUUID());
                modelTf = PartnerModelRoot[arr];
            }

        }
        else
            modelTf = PlayCharRoot;

        CharInven.EquipCostume(costume._costumeIndex);

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

        GameObject go = UIHelper.CreatePcUIModel("ReadyPopup",modelTf, CharInven.GetCharIdx(), headId, costume._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 5, CharInven.isHideCostum, false);
        go.GetComponent<UIModel>().CrossFadeAnimation(eAnimName.Anim_skill8, eAnimName.Anim_idle);

        Transform slotTf = CostumGrid.FindChild(string.Format("{0}", costume._costmeDataIndex));

        slotTf.FindChild("mount").gameObject.SetActive(true);
        slotTf.FindChild("btn_insert").collider.enabled = false;
        UILabel nameLb = slotTf.FindChild("state_d3").GetComponent<UILabel>();
        nameLb.text = _LowDataMgr.instance.GetStringCommon(37); //("장착중");

        //기존꺼 수정하기
        CostumMountIconTf.parent.FindChild("mount").gameObject.SetActive(false);
        CostumMountIconTf.parent.FindChild("btn_insert").collider.enabled = true;
        CostumMountIconTf.parent.FindChild("bg").GetComponent<UISprite>().color = new Color(1, 1, 1, 0.3f);
        UILabel lbl = CostumMountIconTf.parent.FindChild("state_d3").GetComponent<UILabel>();
        lbl.text = _LowDataMgr.instance.GetStringCommon(38); //("장착하기");

        CostumMountIconTf.parent = slotTf;
        CostumMountIconTf.localPosition = Vector3.zero;
        CostumMountIconTf.localScale = Vector3.one;
        slotTf.FindChild("bg").GetComponent<UISprite>().color = Color.white;

        TownState town = SceneManager.instance.GetState<TownState>();
        town.MyHero.SetChangeSkin(true);
    }
    /*
    /// <summary> 파트너 </summary>
    public void OnPMsgPartner(bool isMount, NetData._PartnerData partner)
    {

    }
    */
    /// <summary> 콜로세움, 멀티보스레이드 방에서 내가 나갔다면 </summary>
    public void OnNetworkClose()
    {
        base.Close();
        if (BasePanel != null)
        {
            BasePanel.OnCloseReadyPopup();
        }
    }

    /// <summary> 마을 콜로세움, 멀티보스레이드 다른 유저 입장 </summary>
    public void OnInUser(NetData.RoomUserInfo userInfo)
    {
        int arr = NetData.instance.GetGameRoomUserArr(userInfo.Id);
        string lv = string.Format(_LowDataMgr.instance.GetStringCommon(453), userInfo.Lv);
        TakeParNames[arr].text = string.Format("{0} {1}", lv, userInfo.Name);
        PnTouchObj[arr].SetActive(true);
        
        TownUnit unit = null;
        if(SceneManager.instance.GetState<TownState>().GetTownUnits().TryGetValue((long)userInfo.Id, out unit) )
        {
            UIHelper.CreatePcUIModel("ReadyPopup",PartnerModelRoot[arr]
                , unit.townunitData.character_id
                , unit.townunitData.EquipHeadItemIdx
                , unit.townunitData.costume_id
                , unit.townunitData.EquipClothItemIdx
                , unit.townunitData.EquipWeaponItemIdx
                , unit.townunitData.SkillSetID
                , 3
                , unit.townunitData.isHideCostume
                , false);
                
        }
        else
        {
            NetworkClient.instance.SendPMsgQueryRoleInfoC((long)userInfo.Id);
        }

    }

    /// <summary> 콜로세움, 멀티보스레이드 다른 유저 퇴장or강퇴 </summary>
    public void OnOutUser(int arr)
    {
        if(arr == -1 )
        {
            //Debug.LogError("OutUser error " + arr);
            return;
        }

        if (0 < PartnerModelRoot[arr].childCount )
            Destroy(PartnerModelRoot[arr].GetChild(0).gameObject);

        PnTouchObj[arr].SetActive(false);
        TakeParNames[arr].text = "";
    }

    /// <summary> 친구 or 길드 콜로세움, 멀티보스레이드 다른 유저 입장 </summary>
    public void OnInUser(ulong userID, uint charIdx, uint HeadItemIdx, uint CostumeItemIdx, uint ClothItemIdx, uint WeaponItemIdx, uint skillSetId, bool HideCostume)
    {
        NetData.RoomData roomData = NetData.instance.GameRoomData;
        Transform modelTf = null;

        if (!roomData.IsLeader && roomData.Owner.Id == userID)//방장에 대한 정보 입력
        {
            string lv = string.Format(_LowDataMgr.instance.GetStringCommon(453), roomData.Owner.Lv);
            transform.FindChild("Character/CharView/CharSlot/player_name").GetComponent<UILabel>().text = string.Format("{0} {1}", lv, roomData.Owner.Name);
            modelTf = PlayCharRoot;
        }
        else
        {
            for (int i = 0; i < roomData.UserList.Count; i++)
            {
                if (roomData.UserList[i].Id != (ulong)userID)
                    continue;
                
                int arr = roomData.UserList[i].Slot;
                modelTf = PartnerModelRoot[arr];
                PnTouchObj[arr].SetActive(true);
                
                string lv = string.Format(_LowDataMgr.instance.GetStringCommon(453), roomData.UserList[i].Lv);
                TakeParNames[arr].text = string.Format("{0} {1}", lv, roomData.UserList[i].Name);
                break;
            }
        }

        UIHelper.CreatePcUIModel("ReadyPopup",modelTf
                , charIdx
                , HeadItemIdx
                , CostumeItemIdx
                , ClothItemIdx
                , WeaponItemIdx
                , skillSetId
                , 3
                , HideCostume
                , false);
    }
    
    #endregion
}

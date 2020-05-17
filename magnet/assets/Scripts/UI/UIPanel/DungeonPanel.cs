using UnityEngine;
using System.Collections.Generic;
using Sw;

public class DungeonPanel : UIBasePanel {

    public GameObject[] ViewObjs;//0List, 1Raid, 2Equip,Skill
    public GameObject[] EnterLock;

    public Transform[] RaidDropItem;
    public UILabel RaidDesc;
    //public UISprite[] RaidDropIcons;

    public UITabGroup RaidLvGroup;// 레이드 난이도 선택
    public GameObject[] SelectBoss; //보스레이드난이도 선택이미지
    private Transform EffRaidSelect;

    public UITabGroup EquipLvGroup; //장비재로 난이도선택
    public UITabGroup SkillLvGroup;//스킬재료 난이도선택
    public Transform[] MaterialDungetnDropItem;//재료던전 아이템

    public Transform PlayCharRoot;// 플레이 중인 캐릭터 보여줄 곳
    public Transform[] PartnerModelRoot;// 파트너 슬롯, 파트너 삽입하면 보여줄 곳 배열은 0~1
    public GameObject[] PnTouchObj;//파트너의 터치라벨 키고 끄려고 만듦
    public GameObject[] PnTouchEff;
    public UIButton MaterialDungeonStartBtn;//재료던전 입장버튼

    public UILabel[] TakeParNames;
    public UILabel charNameLv;

    private GAME_MODE CurGameMode;
    private bool IsInvite;
    //private static bool IsSetTextList;
    private int CurSubMode;
    private int CurLevelDifficulty;
    private uint DungeonId;
    private NetData._UserInfo CharInven;

    private int MaterialDungeonLevelDifficulty;

    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("Dungeon panel, Init() start");

        base.Init();
        ViewObjs[0].SetActive(true);
        ViewObjs[1].SetActive(false);
        ViewObjs[2].SetActive(false);

        CharInven = NetData.instance.GetUserInfo();

        UIHelper.CreateEffectInGame(PnTouchEff[0].transform, "Fx_UI_partner_select_01");
        UIHelper.CreateEffectInGame(PnTouchEff[1].transform, "Fx_UI_partner_select_01");
        SetMaterialReadyPopup();

        //컨텐츠 입장레벨
        byte[] enterTypes = new byte[] {
            (byte)ContentType.TOWER,
            0,
            (byte)ContentType.EQUIP_DUNGEON,
            (byte)ContentType.SKILL_DUNGEON,
            (byte)ContentType.BOSS_RAID,
        };

        uint myLv = NetData.instance.GetUserInfo()._Level;
        uint myAttack = NetData.instance.GetUserInfo()._TotalAttack;
        for (int i = 0; i < EnterLock.Length; i++)
        {
            Transform tf = EnterLock[i].gameObject.transform;

            uint openCondition = 0;
            uint stringIdx = 1023;  //{0}렙에열림(1023), {0}전투력에열림(1024) 
            if (enterTypes[i] != 0 )
            {
                DungeonTable.ContentsOpenInfo content = _LowDataMgr.instance.GetFirstContentsOpenInfo(enterTypes[i]);
                if (content.OpenType == 1)//업데이트 컨텐츠
                {
                    UILabel enterLvLabel = tf.FindChild("Txt").GetComponent<UILabel>();
                    enterLvLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1307));
                    EnterLock[i].SetActive(true);
                    continue;
                }

                openCondition = content.ConditionValue1;
                if (content.ConditionType1 == 2)
                    stringIdx = 1024;
            }
            else//차관은 etc에엮여있어서따로뺏음
                openCondition = _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvpEnterLv);

            uint flagValue = stringIdx == 1023 ? myLv : myAttack;
            if (flagValue < openCondition)  //입장렙 or 전투력이 안대
            {
                UILabel enterLvLabel = tf.FindChild("Txt").GetComponent<UILabel>();
                enterLvLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(stringIdx), openCondition);
                EnterLock[i].SetActive(true);
                continue;
            }

            EnterLock[i].SetActive(false);
        }

        Transform listTf = transform.FindChild("List");
        EventDelegate.Set(listTf.FindChild("Gold").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.EQUIP_DUNGEON))
                return;

            CurGameMode = GAME_MODE.SPECIAL_GOLD;
            EquipLvGroup.CoercionTab(0);
            transform.FindChild("Background/bg").gameObject.SetActive(false);

            //NetworkClient.instance.SendPMsgCoinBattleQueryC();

        });

        EventDelegate.Set(listTf.FindChild("Exp").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.SKILL_DUNGEON))
                return;

            CurGameMode = GAME_MODE.SPECIAL_EXP;
            SkillLvGroup.CoercionTab(0);
            transform.FindChild("Background/bg").gameObject.SetActive(false);

            // NetworkClient.instance.SendPMsgExpBattleQueryC();
        });
        /*
        EventDelegate.Set(listTf.FindChild("Raid_0").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.BOSS_RAID_1))
                return;

            //CheckOpenTutorial(OpenTutorialType.BOSS_RAID);

            CurGameMode = GAME_MODE.RAID;
            CurSubMode = 1;
            RaidLvGroup.CoercionTab(0);
        });

        EventDelegate.Set(listTf.FindChild("Raid_1").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.BOSS_RAID_2) )
                return;

            //CheckOpenTutorial(OpenTutorialType.BOSS_RAID);
            CurGameMode = GAME_MODE.RAID;
            CurSubMode = 2;
            RaidLvGroup.CoercionTab(0);
        });

        EventDelegate.Set(listTf.FindChild("Raid_2").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.BOSS_RAID_3) )
                return;

            //CheckOpenTutorial(OpenTutorialType.BOSS_RAID);
            CurGameMode = GAME_MODE.RAID;
            CurSubMode = 3;
            RaidLvGroup.CoercionTab(0);
        });

        EventDelegate.Set(listTf.FindChild("MultyRaid_0").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.MULTY_BOSS_RAID_1))
                return;

            CurGameMode = GAME_MODE.MULTI_RAID;
            CurSubMode = 1;
            RaidLvGroup.CoercionTab(0);
        });
        */
        EventDelegate.Set(listTf.FindChild("MultyRaid_1").GetComponent<UIEventTrigger>().onClick, delegate () {

            SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(174) );
        });

        EventDelegate.Set(listTf.FindChild("MultyRaid_2").GetComponent<UIEventTrigger>().onClick, delegate () {
            
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(174));
        });

        EventDelegate.Set(listTf.FindChild("Pvp").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.ARENA) )
                return;

            base.Hide();
            UIMgr.OpenArenaPanel();
        });

        EventDelegate.Set(listTf.FindChild("Tower").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.TOWER) )
                return;
            
            base.Hide();
            UIMgr.OpenTowerPanel();
        });
        /*
        EventDelegate.Set(listTf.FindChild("Colosseum").GetComponent<UIEventTrigger>().onClick, delegate () {

            if (!CheckEnterCondition(ContentType.COLOSSEUM) )
                return;

            base.Hide();
            UIMgr.OpenColosseumPanel(0);
        });
        */
        EventDelegate.Set(ViewObjs[1].transform.FindChild("BtnReady").GetComponent<UIButton>().onClick, delegate() {
            uint limitLevel = 0, dungeonId = 0;
            if (CurGameMode == GAME_MODE.RAID)
            {
                List<DungeonTable.SingleBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataBossRaidList();

                int count = raidLowList.Count;
                for (int i = 0; i < count; i++)
                {
                    DungeonTable.SingleBossRaidInfo raidLow = raidLowList[i];
                    if (CurSubMode != raidLow.Type || raidLow.level != CurLevelDifficulty)
                        continue;

                    limitLevel = raidLow.levelLimit;
                    break;
                }
            }
            else
            {
                List<DungeonTable.MultyBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataMultyBossInfoList((byte)CurSubMode);

                int count = raidLowList.Count;
                for (int i = 0; i < count; i++)
                {
                    DungeonTable.MultyBossRaidInfo raidLow = raidLowList[i];
                    if (raidLow.level != CurLevelDifficulty)
                        continue;

                    dungeonId = raidLow.raidId;
                    limitLevel = raidLow.EnterLevel;
                    break;
                }
            }

            if (NetData.instance.UserLevel < limitLevel)//레벨 부족 무시한다
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(699), limitLevel);
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
                return;
            }

            if (CurGameMode == GAME_MODE.RAID)
                NetworkClient.instance.SendPMsgBossBattleQueryC(CurSubMode);
            else
            {
                int now = 0, max = 0;
                NetData.instance.GetUserInfo().GetCompleteCount(EtcID.MultyBossRaid1Count, ref now, ref max);
                if (max <= now)
                {
                    uiMgr.AddErrorPopup((int)ErrorCode.ER_MultiBossCreateRoomS_DailyTime_Error);
                    return;
                }

                NetworkClient.instance.SendPMsgMultiBossCreateRoomC(dungeonId);
            }
        });

        RaidLvGroup.Initialize(delegate (int arr)
        {
            CurLevelDifficulty = arr + 1;
            for (int i = 0; i < EffRaidSelect.childCount; i++)
            {
                EffRaidSelect.GetChild(i).gameObject.SetActive(i == arr);
                SelectBoss[i].SetActive(i == arr);
            }

            OnClickRaid(CurGameMode == GAME_MODE.MULTI_RAID, CurSubMode);
        });


        EventDelegate.Set(ViewObjs[2].transform.FindChild("Character/CharView/BtnPartnerSlot_0").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            if (PartnerModelRoot[0].gameObject.activeSelf)
                return;

            base.Hide();
            UIMgr.OpenReadyPopup(CurGameMode, this, 0, 0);
        });
        EventDelegate.Set(ViewObjs[2].transform.FindChild("Character/CharView/BtnPartnerSlot_1").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            if (PartnerModelRoot[1].gameObject.activeSelf)
                return;

            base.Hide();
            UIMgr.OpenReadyPopup(CurGameMode, this, 0, 0);
        });

        EventDelegate.Set(ViewObjs[2].transform.FindChild("Character/BtnStartGame").GetComponent<UIButton>().onClick, delegate ()
        {//준비창으로 변경
            base.Hide();
            //PvpReadyPopup.SetActive(false);
            UIMgr.OpenReadyPopup(GAME_MODE.ARENA, this, 0, 0);
        });


        EquipLvGroup.Initialize(delegate (int arr)
            {
                if (EquipLvGroup.transform.GetChild(arr).FindChild("Lvguide").gameObject.activeSelf)
                {
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 171);
                    return;
                }

                MaterialDungeonLevelDifficulty = arr + 1;

                for(int i=0;i< EquipLvGroup.transform.childCount;i++)
                {
                    GameObject select = EquipLvGroup.transform.GetChild(i).transform.FindChild("cover").gameObject;
                    select.SetActive(i == arr);
                }
                
                OnClickEquipAndSkillDungeon(true);
            });
        SkillLvGroup.Initialize(delegate (int arr)
        {
            if (SkillLvGroup.transform.GetChild(arr).FindChild("Lvguide").gameObject.activeSelf)
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 171);
                return;
            }

            MaterialDungeonLevelDifficulty = arr + 1;

            for (int i = 0; i < SkillLvGroup.transform.childCount; i++)
            {
                GameObject select = SkillLvGroup.transform.GetChild(i).transform.FindChild("cover").gameObject;
                select.SetActive(i == arr);
            }

            OnClickEquipAndSkillDungeon(false);
        });


        EffRaidSelect = UIHelper.CreateEffectInGame(RaidLvGroup.transform, "Fx_UI_Raid_select_set").transform;
        int length = EffRaidSelect.childCount;
        for (int i = 0; i < length; i++)
        {
            EffRaidSelect.GetChild(i).localPosition = RaidLvGroup.TabList[i].transform.localPosition;
        }

        for(int i=0; i < RaidDropItem.Length; i++)
        {
            UIHelper.CreateInvenSlot(RaidDropItem[i]);
        }

        for (int i = 0; i < MaterialDungetnDropItem.Length; i++)
        {
            UIHelper.CreateInvenSlot(MaterialDungetnDropItem[i]);
        }

        MaterialDungeonInit();


        SceneManager.instance.showStopWatchTimer ("Dungeon panel, Init() finish");
    }
    
    bool CheckEnterCondition(ContentType type)
    {
        uint openCondition = 0;
        uint stringIdx = 699;  //해당 던전은 {0}렙부터 입장가능합니다.
        if (type != ContentType.ARENA)
        {
            DungeonTable.ContentsOpenInfo content = _LowDataMgr.instance.GetFirstContentsOpenInfo((byte)type );
            if (content.OpenType == 1)//업데이트 컨텐츠
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1307));
                return false;
            }

            openCondition = content.ConditionValue1;
            if (content.ConditionType1 == 2)//전투력
                stringIdx = 729;//전투력 {0} 이상만 가능합니다.
        }
        else//차관은 etc에엮여있어서따로뺏음
            openCondition = _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvpEnterLv);

        uint flagValue = stringIdx == 699 ? NetData.instance.UserLevel : NetData.instance.GetUserInfo()._TotalAttack;
        if (flagValue < openCondition)  //입장렙 or 전투력이 안대
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(stringIdx), openCondition));
            return false;
        }
        
        return true;
    }

    public override void LateInit()
    {
        base.LateInit();
        bool isMulty = (bool)parameters[0];
        uint dungeonId = (uint)parameters[1];

        GAME_MODE openMode = (GAME_MODE)parameters[2];//다른곳에서 바로 열었다.
        int subType = (int)parameters[3];

        if (isMulty)//초대 받아서 온거임
        {
            OnMultyRaid(dungeonId, true);
        }
        else
        {
            CurGameMode = openMode;
            switch (openMode)
            {
                case GAME_MODE.SPECIAL_EXP:
                    NetworkClient.instance.SendPMsgExpBattleQueryC();
                    break;

                case GAME_MODE.SPECIAL_GOLD:
                    NetworkClient.instance.SendPMsgCoinBattleQueryC();
                    break;

                case GAME_MODE.MULTI_RAID:
                case GAME_MODE.RAID:
                    CurSubMode = subType;
                    RaidLvGroup.CoercionTab(0);
                    break;
            }
        }

        if (SceneManager.instance.CurTutorial == TutorialType.TOWER)
        {
            if (OnSubTutorial() )
            {
                //transform.FindChild("List").GetComponent<UIScrollView>().enabled = false;
                uiMgr.TopMenu.TutoSupport.TutoType = TutorialType.TOWER;
            }
        }
        //else
        //    transform.FindChild("List").GetComponent<UIScrollView>().enabled = true;

        SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("Dungeon panel, LateInit() finish");
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    public override void Hide()
    {
		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        if (ViewObjs[1].activeSelf || ViewObjs[2].activeSelf)
        {
            CloseRaidPanel();
        }
        else 
        {
            base.Hide();
            UIMgr.OpenTown();
        }
    }

    #region 준비화면으로 바로 이동

   

    void OnClickRaid(bool isMulty, int groupId)
    {
        ViewObjs[0].SetActive(false);
        ViewObjs[1].SetActive(true);
        ViewObjs[2].SetActive(false);
        CurGameMode = isMulty ? GAME_MODE.MULTI_RAID : GAME_MODE.RAID;
        CurSubMode = groupId;

        uint bossIdx = 0;
        List<string> rewardList = null;
        List<string> itemList = null;
        if ( !isMulty)
        {
            List<DungeonTable.SingleBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataBossRaidList();
            
            int count = raidLowList.Count;
            for (int i = 0; i < count; i++)
            {
                DungeonTable.SingleBossRaidInfo raidLow = raidLowList[i];
                if (raidLow.Type != groupId || raidLow.level != CurLevelDifficulty)
                    continue;
                
                rewardList = raidLowList[i].RewardId.list;
                bossIdx = raidLowList[i].BossIdx;
                itemList = raidLowList[i].ItemId.list;
                RaidDesc.text = _LowDataMgr.instance.GetStringStageData(raidLowList[i].stageDesc);
                break;
            }
        }
        else
        {
            List<DungeonTable.MultyBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataMultyBossInfoList((byte)groupId);
            int count = raidLowList.Count;
            for (int i = 0; i < count; i++)
            {
                DungeonTable.MultyBossRaidInfo raidLow = raidLowList[i];
                if (raidLow.level != CurLevelDifficulty)
                    continue;

                rewardList = raidLowList[i].RewardId.list;
                bossIdx = raidLow.BossIdx;
                itemList = raidLow.ItemId.list;
                RaidDesc.text = _LowDataMgr.instance.GetStringStageData(raidLow.stageDesc);
                break;
            }
        }

        if(bossIdx <= 0 )
        {
            Debug.LogError("not found boss Index Error " + CurSubMode);
            return;
        }
        
        UIHelper.CreateMonsterUIModel(RotationTargetList[0].transform, bossIdx, false, false, true, "DungeonPanel");

        //드랍 아이템 셋
        //드랍아이템은 나의 클래스에맞춰서 표시해준다
        //장비아이템의경우 가장 등급이 높은것으로 
        int myClass = UIHelper.GetClassType(NetData.instance.GetUserInfo().GetCharIdx() );

        _LowDataMgr lowMgr = _LowDataMgr.instance;
        int dropIconCount = RaidDropItem.Length;
        for (int i = 0; i < dropIconCount; i++)
        {
            if (rewardList.Count <= i)
            {
                RaidDropItem[i].gameObject.SetActive(false);
                continue;
            }

            //uint rewardItemId = uint.Parse(rewardList[i]);
            uint itemId = uint.Parse(itemList[i]);
            RaidDropItem[i].gameObject.SetActive(true);
            InvenItemSlotObject slot = RaidDropItem[i].GetChild(0).GetComponent<InvenItemSlotObject>();
            slot.SetLowDataItemSlot(itemId, 0, delegate (ulong key)
            {
                //UIMgr.OpenClickPopup(itemId, slot.transform.position);
                UIMgr.OpenDetailPopup(this, itemId);
            });
        }

        RaidDropItem[0].parent.GetComponent<UIGrid>().Reposition();
        UIScrollView scroll = RaidDropItem[0].parent.parent.GetComponent<UIScrollView>();
        if(scroll != null )
        {
            scroll.ResetPosition();
            scroll.enabled = rewardList.Count <= 5 ? false : true;
        }
    }

    void CloseRaidPanel()
    {
        ViewObjs[0].SetActive(true);
        ViewObjs[1].SetActive(false);
        ViewObjs[2].SetActive(false);

        transform.FindChild("Background/bg").gameObject.SetActive(true);
    }

    /// <summary> 준비화면으로 바로 이동 </summary>
    public void OnOpenQuery()
    {
        int now = 0, max = 0;
        switch (CurGameMode)
        {
            case GAME_MODE.SPECIAL_EXP:
                NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ExpBattleCount, ref now, ref max);
                break;
            case GAME_MODE.SPECIAL_GOLD:
                NetData.instance.GetUserInfo().GetCompleteCount(EtcID.GoldBattleCount, ref now, ref max);
                break;

            case GAME_MODE.RAID:
                if(CurSubMode == 1)
                    NetData.instance.GetUserInfo().GetCompleteCount(EtcID.BossRaid1Count, ref now, ref max);
                else if (CurSubMode == 2)
                    NetData.instance.GetUserInfo().GetCompleteCount(EtcID.BossRaid2Count, ref now, ref max);
                else if (CurSubMode == 3)
                    NetData.instance.GetUserInfo().GetCompleteCount(EtcID.BossRaid3Count, ref now, ref max);
                break;

            //case GAME_MODE.MULTI_RAID:
            //    ViewObjs[1].SetActive(false);
            //    NetData.instance.GetUserInfo().GetCompleteCount(EtcID.MultyBossRaid1Count, ref now, ref max);
            //    break;
        }

        if(max <= now)//일일 이용 가능횟수 초과
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 396);
            return;
        }

        if (CurGameMode == GAME_MODE.RAID)
            ViewObjs[1].SetActive(false);
       
        if(CurGameMode == GAME_MODE.SPECIAL_EXP || CurGameMode == GAME_MODE.SPECIAL_GOLD)
        {
            OnClickEquipAndSkillDungeon(CurGameMode == GAME_MODE.SPECIAL_GOLD ? true : false);
        }
        else
            UIMgr.OpenReadyPopup(CurGameMode, this, now, max);
    }
    
    public override void GotoInGame()
    {
        base.GotoInGame();
        if (CurGameMode == GAME_MODE.SPECIAL_GOLD)//골드 스테이지로 들어감
        {
            uiMgr.CloseReadyPopup();
            OnCloseReadyPopup();
            //uint level = NetData.instance.UserLevel;
            //byte dungeonId = 1;
            //while (true)
            //{
            //    DungeonTable.EquipInfo materialData = _LowDataMgr.instance.GetLowDataEquipBattle(dungeonId);
            //    if (materialData != null)
            //    {
            //        if (materialData.MinenterLv < level)
            //            break;
            //    }

            //    ++dungeonId;
            //}

            //if (dungeonId == 0)//혹시 모를 경우를 대비해 이렇게 막아놓는다.
            //{
            //    DungeonTable.EquipInfo mData = _LowDataMgr.instance.GetLowDataEquipBattle(1);

            //    string msg = _LowDataMgr.instance.GetStringCommon(699);
            //    msg = string.Format(msg, mData.MinenterLv);
            //    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
            //    return;
            //}

            //OnGoldGameStart(dungeonId);
            //NetworkClient.instance.SendPMsgCoinBattleStartC(dungeonId);
        }
        else if (CurGameMode == GAME_MODE.SPECIAL_EXP)//경험치 스테이지로 들어감
        {
            uiMgr.CloseReadyPopup();
            OnCloseReadyPopup();

            //uint level = NetData.instance.UserLevel;
            //byte dungeonId = 1;
            //while (true)
            //{
            //    DungeonTable.SkillInfo skillData = _LowDataMgr.instance.GetLowDataSkillBattle(dungeonId);
            //    if (skillData != null)
            //    {
            //        if (skillData.MinenterLv <= level)
            //            break;
            //    }


            //    ++dungeonId;
            //}

            //if (dungeonId == 0)//혹시 모를 경우를 대비해 이렇게 막아놓는다.
            //{
            //    DungeonTable.SkillInfo skillData = _LowDataMgr.instance.GetLowDataSkillBattle(1);

            //    string msg = _LowDataMgr.instance.GetStringCommon(699);
            //    msg = string.Format(msg, skillData.MinenterLv);
            //    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
            //    return;
            //}

            //OnExpGameStart(dungeonId);
            //NetworkClient.instance.SendPMsgCoinBattleStartC(dungeonId);
        }
        else if (CurGameMode == GAME_MODE.RAID)//레이드 스테이지로 들어감
        {
            List<DungeonTable.SingleBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataBossRaidList();
            int count = raidLowList.Count;
            for (int i = 0; i < count; i++)
            {
                DungeonTable.SingleBossRaidInfo raidLow = raidLowList[i];
                if (raidLow.Type != CurSubMode ||  raidLow.level != CurLevelDifficulty)
                    continue;

                NetworkClient.instance.SendPMsgBossBattleStartC(raidLow.raidId);
                break;
            }
            
        }
        else if (CurGameMode == GAME_MODE.MULTI_RAID)
        {
            List<DungeonTable.MultyBossRaidInfo> raidLowList = _LowDataMgr.instance.GetLowDataMultyBossInfoList((byte)CurSubMode);
            int count = raidLowList.Count;
            for (int i = 0; i < count; i++)
            {
                DungeonTable.MultyBossRaidInfo raidLow = raidLowList[i];
                if (raidLow.level != CurLevelDifficulty)
                    continue;

                NetworkClient.instance.SendPMsgMultiBossStartC(raidLow.raidId);
                break;
            }
        }
    }

    /// <summary> 멀티 보스레이드 시작 </summary>
    public void OnMultyRaidGameStart()
    {
        uiMgr.CloseReadyPopup();
    }

    /// <summary> 경험치 던전 스타트 응답 </summary>
    public void OnExpGameStart(uint lowDataId)//PMsgExpBattleStartS pmsgExpBattleStartS
    {
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.SPECIAL_EXP, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(true);

            //마지막으로 선택한 스테이지 아이디 저장
            //List<NetData.MonsterData> monList = new List<NetData.MonsterData>();
            //int dropCount = pmsgExpBattleStartS.CDrop.Count;
            //for (int i = 0; i < dropCount; i++)
            //{
            //    ExpDropItem dropData = pmsgExpBattleStartS.CDrop[i];
            //    monList.Add(new NetData.MonsterData(dropData.UnMonsterId, dropData.UnExp));
            //}

            //SpecialGameState.MonList = monList;
            //SpecialGameState.SpecialType = SpecialGameType.SPECIAL_EXP;
            SpecialGameState.lastSelectStageId = lowDataId;//(uint)pmsgExpBattleStartS.UnExpBattleId;
            SpecialGameState.IsGoldStage = false;

            SceneManager.instance.ActionEvent(_ACTION.SPECIAL_STAGE);

        });

        uiMgr.CloseReadyPopup();
    }

    /// <summary> 골드 던전 스타트 응답 </summary>
    public void OnGoldGameStart(uint lowDataId)//PMsgCoinBattleStartS pmsgCoinBattleStartS
    {
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.SPECIAL_GOLD, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(true);

            //마지막으로 선택한 스테이지 아이디 저장
            //List<NetData.MonsterData> monList = new List<NetData.MonsterData>();
            //int dropCount = pmsgCoinBattleStartS.CDrop.Count;
            //for (int i = 0; i < dropCount; i++)
            //{
            //    CoinDropItem dropData = pmsgCoinBattleStartS.CDrop[i];
            //    monList.Add(new NetData.MonsterData(dropData.UnMonsterId, dropData.UnCoin));
            //}

            //SpecialGameState.MonList = monList;
            //SpecialGameState.SpecialType = SpecialGameType.SPECIAL_GOLD;
            SpecialGameState.lastSelectStageId = lowDataId;// (uint)pmsgCoinBattleStartS.UnExpBattleId;
            SpecialGameState.IsGoldStage = true;

            SceneManager.instance.ActionEvent(_ACTION.SPECIAL_STAGE);
        });

        uiMgr.CloseReadyPopup();
    }

    /// <summary> 레이드 던전 스타트 응답 </summary>
    public void OnRaidGameStart(int dungeonId)
    {
        DungeonTable.SingleBossRaidInfo curRaidLowData = null;
        _LowDataMgr.instance.RefLowDataBossRaid((uint)dungeonId, ref curRaidLowData);

        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.RAID, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(true);

            //SpecialGameState.SpecialType = GameType;
            //RaidGameState.RaidType = ;

            //마지막으로 선택한 스테이지 아이디 저장
            RaidGameState.lastSelectStageId = curRaidLowData.raidId;

            SceneManager.instance.ActionEvent(_ACTION.PLAY_RAID);
        });

        uiMgr.CloseReadyPopup();
    }

    public override void OnCloseReadyPopup()
    {
        base.OnCloseReadyPopup();
        if (CurGameMode == GAME_MODE.RAID || CurGameMode == GAME_MODE.MULTI_RAID)
        {
            ViewObjs[0].SetActive(false);
            ViewObjs[1].SetActive(true);
            ViewObjs[2].SetActive(false);

            if(IsInvite)
            {
                if( 0 < DungeonId)
                {
                    DungeonTable.MultyBossRaidInfo multyRaid = _LowDataMgr.instance.GetLowDataMultyBossInfo(DungeonId);

                    CurSubMode = multyRaid.Type;
                    CurLevelDifficulty = multyRaid.level;
                    RaidLvGroup.CoercionTab(CurLevelDifficulty - 1);
                }
            }
        }
        else if (CurGameMode == GAME_MODE.SPECIAL_GOLD || CurGameMode == GAME_MODE.SPECIAL_EXP)
        {
            Show(false, (uint)0, CurGameMode, 0);

            //ViewObjs[0].SetActive(false);
            //ViewObjs[1].SetActive(false);
            //ViewObjs[2].SetActive(true);

            SetMaterialReadyPopup();
        }
        else
        {
            ViewObjs[0].SetActive(true);
            ViewObjs[1].SetActive(false);
        }

        IsInvite = false;
    }

    public void OnMultyRaid(uint dungeonId, bool isInvite)
    {
        DungeonId = dungeonId;

        ViewObjs[0].SetActive(false);
        ViewObjs[1].SetActive(false);

        IsInvite = isInvite;

        CurGameMode = GAME_MODE.MULTI_RAID;
        int now = 0, max = 0;
        
        ViewObjs[1].SetActive(false);
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.MultyBossRaid1Count, ref now, ref max);

        if (max <= now)//일일 이용 가능횟수 초과
        {
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 396);
            return;
        }

        UIMgr.OpenReadyPopup(CurGameMode, this, now, max, dungeonId);
    }

    #endregion


    #region 재료던전


    //최초 생성될것들
    void MaterialDungeonInit()
    {
        //이펙트
        UIHelper.CreateEffectInGame(SkillLvGroup.transform.FindChild("BtnNormal/label"), "Fx_UI_Dungeon_Easy_01");
        UIHelper.CreateEffectInGame(SkillLvGroup.transform.FindChild("BtnHard/label"), "Fx_UI_Dungeon_Normal_01");
        UIHelper.CreateEffectInGame(SkillLvGroup.transform.FindChild("BtnVeryHard/label"), "Fx_UI_Dungeon_Hard_01");
        UIHelper.CreateEffectInGame(SkillLvGroup.transform.FindChild("BtnHell/label"), "Fx_UI_Dungeon_VeryHard_01");

        UIHelper.CreateEffectInGame(EquipLvGroup.transform.FindChild("BtnNormal/label"), "Fx_UI_Dungeon_Easy_01");
        UIHelper.CreateEffectInGame(EquipLvGroup.transform.FindChild("BtnHard/label"), "Fx_UI_Dungeon_Normal_01");
        UIHelper.CreateEffectInGame(EquipLvGroup.transform.FindChild("BtnHell/label"), "Fx_UI_Dungeon_VeryHard_01");

        ViewObjs[2].transform.FindChild("Character/Txt_charname/label").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), CharInven._TotalAttack);


        //던전 난이도
        DungeonTable.SkillInfo skillDungeonInfo = null;
        for (int i = 0; i < SkillLvGroup.transform.childCount; i++)
        {
            UILabel lv = SkillLvGroup.transform.GetChild(i).transform.FindChild("Txt").GetComponent<UILabel>();
            GameObject lockImg = SkillLvGroup.transform.GetChild(i).transform.FindChild("Lvguide").gameObject;
            skillDungeonInfo = _LowDataMgr.instance.GetLowDataSkillBattle((byte)(i + 1));
            if (skillDungeonInfo != null)
            {
                lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(1271), skillDungeonInfo.MinenterLv.ToString());
                lv.color = skillDungeonInfo.MinenterLv > CharInven._Level ? Color.red : Color.white;
                lockImg.SetActive(skillDungeonInfo.MinenterLv > CharInven._Level);
            }
        }

        DungeonTable.EquipInfo equipDungeonInfo = null;
        for (int i = 0; i < EquipLvGroup.transform.childCount; i++)
        {
            UILabel lv = EquipLvGroup.transform.GetChild(i).transform.FindChild("Txt").GetComponent<UILabel>();
            GameObject lockImg = EquipLvGroup.transform.GetChild(i).transform.FindChild("Lvguide").gameObject;
            equipDungeonInfo = _LowDataMgr.instance.GetLowDataEquipBattle((byte)(i + 1));
            if (equipDungeonInfo != null)
            {
                lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(1271), equipDungeonInfo.MinenterLv.ToString());
                lv.color = equipDungeonInfo.MinenterLv > CharInven._Level ? Color.red : Color.white;
                lockImg.SetActive(equipDungeonInfo.MinenterLv > CharInven._Level);
            }
        }



    }


    /// <summary>
    /// 장비강화재료 / 스킬강화재료 던전
    /// </summary>
    void OnClickEquipAndSkillDungeon(bool isEquip)
    {
        _LowDataMgr lowMgr = _LowDataMgr.instance;

        ViewObjs[0].SetActive(false);
        ViewObjs[1].SetActive(false);
        ViewObjs[2].SetActive(true);

        EquipLvGroup.transform.gameObject.SetActive(isEquip);
        SkillLvGroup.transform.gameObject.SetActive(!isEquip);

        byte useEnergy = 0; 
        List<string> itemList = null;
        if (isEquip)
        {
            //장비강화
            DungeonTable.EquipInfo equipDungeonInfo = lowMgr.GetLowDataEquipBattle((byte)MaterialDungeonLevelDifficulty);
            if (equipDungeonInfo != null)
            {
                itemList = equipDungeonInfo.RewardItemId.list;
                useEnergy = equipDungeonInfo.UseEnergy;
            }
        }
        else
        {
            //스킬강화
            DungeonTable.SkillInfo skillDungeonInfo = lowMgr.GetLowDataSkillBattle((byte)MaterialDungeonLevelDifficulty);
            if (skillDungeonInfo != null)
            {
                itemList = skillDungeonInfo.RewardItemId.list;
                useEnergy = skillDungeonInfo.UseEnergy;
            }
        }

        bool canGo = NetData.instance.GetAsset(AssetType.Energy) >= useEnergy;
        MaterialDungeonStartBtn.transform.FindChild("Txt_num").GetComponent<UILabel>().text = useEnergy.ToString();
        MaterialDungeonStartBtn.transform.FindChild("Btn_on").gameObject.SetActive(canGo);
        MaterialDungeonStartBtn.transform.FindChild("Btn_off").gameObject.SetActive(!canGo);

        //드랍아이템
        int dropIconCount = MaterialDungetnDropItem.Length;
        for (int i = 0; i < dropIconCount; i++)
        {
            if (itemList.Count <= i)
            {
                MaterialDungetnDropItem[i].gameObject.SetActive(false);
                continue;
            }

            uint itemId = uint.Parse(itemList[i]);
            MaterialDungetnDropItem[i].gameObject.SetActive(true);
            InvenItemSlotObject slot = MaterialDungetnDropItem[i].GetChild(0).GetComponent<InvenItemSlotObject>();
            slot.SetLowDataItemSlot(itemId, 0, delegate (ulong key)
            {
                UIMgr.OpenDetailPopup(this, itemId);
            });
        }

        MaterialDungetnDropItem[0].parent.GetComponent<UIGrid>().Reposition();
        UIScrollView scroll = MaterialDungetnDropItem[0].parent.parent.GetComponent<UIScrollView>();
        if (scroll != null)
        {
            scroll.ResetPosition();
            scroll.enabled = itemList.Count <= 5 ? false : true;
        }

        EventDelegate.Set(MaterialDungeonStartBtn.onClick, delegate ()
        {
            if(!canGo)
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 390);  //체력부족?
                return;
            }

            //여기서 시작해줌
            if (CurGameMode == GAME_MODE.SPECIAL_GOLD)
            {
                //uint level = NetData.instance.UserLevel;
                //byte dungeonId = 1;
                //while (true)
                //{
                //    DungeonTable.EquipInfo materialData = _LowDataMgr.instance.GetLowDataEquipBattle(dungeonId);
                //    if (materialData != null)
                //    {
                //        if (materialData.MinenterLv < level)
                //            break;
                //    }

                //    ++dungeonId;
                //}

                byte dungeonId = 1;
                DungeonTable.EquipInfo materialData = lowMgr.GetLowDataEquipBattle((byte)MaterialDungeonLevelDifficulty);

                if(materialData!=null)
                {
                    dungeonId = materialData.Index;
                }

                if (dungeonId == 0)//혹시 모를 경우를 대비해 이렇게 막아놓는다.
                {
                    DungeonTable.EquipInfo mData = lowMgr.GetLowDataEquipBattle(1);

                    string msg = _LowDataMgr.instance.GetStringCommon(699);
                    msg = string.Format(msg, mData.MinenterLv);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
                    return;
                }

                OnGoldGameStart(dungeonId);
                //NetworkClient.instance.SendPMsgCoinBattleStartC(dungeonId);
            }

            else if (CurGameMode == GAME_MODE.SPECIAL_EXP)//경험치 스테이지로 들어감
            {
                //uint level = NetData.instance.UserLevel;
                //byte dungeonId = 1;
                //while (true)
                //{
                //    DungeonTable.SkillInfo skillData = _LowDataMgr.instance.GetLowDataSkillBattle(dungeonId);
                //    if (skillData != null)
                //    {
                //        if (skillData.MinenterLv <= level)
                //            break;
                //    }


                //    ++dungeonId;
                //}

                byte dungeonId = 1;
                DungeonTable.SkillInfo materialData = lowMgr.GetLowDataSkillBattle((byte)MaterialDungeonLevelDifficulty);

                if (materialData != null)
                {
                    dungeonId = materialData.Index;
                }

                if (dungeonId == 0)//혹시 모를 경우를 대비해 이렇게 막아놓는다.
                {
                    DungeonTable.SkillInfo skillData = lowMgr.GetLowDataSkillBattle(1);

                    string msg = _LowDataMgr.instance.GetStringCommon(699);
                    msg = string.Format(msg, skillData.MinenterLv);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
                    return;
                }

                OnExpGameStart(dungeonId);
                //NetworkClient.instance.SendPMsgExpBattleStartC(dungeonId);
            }
        });


    }

    void SetMaterialReadyPopup()
    {
        string nickName = NetData.instance.Nickname;
        string _lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
        charNameLv.text = string.Format("{0} {1}", _lvStr, nickName);


        //장착중인 파트너 생성
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

        UIHelper.CreatePcUIModel("DungeonPanel", PlayCharRoot, CharInven.GetCharIdx(), headId, equipCostumeData._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 3, CharInven.isHideCostum, false);

        NetData._PartnerData partner_0 = CharInven.GetEquipPartner(1);
        NetData._PartnerData partner_1 = CharInven.GetEquipPartner(2);

        if (partner_0 != null)
        {
            Transform modelRoot = PartnerModelRoot[0];
            PnTouchObj[0].SetActive(false);//터치 라벨 끈다.
            PnTouchEff[0].SetActive(false);
            PartnerModelRoot[0].gameObject.SetActive(true);

            UIHelper.CreatePartnerUIModel(modelRoot, partner_0._partnerDataIndex, 3, true, false, "DungeonPanel");
            string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), partner_0._NowLevel);
            TakeParNames[0].text = string.Format("{0} {1}", lvStr, partner_0.GetLocName());
        }
        else
        {
            PnTouchObj[0].SetActive(true);//터치 라벨 킨다.
            PnTouchEff[0].SetActive(true);
            PartnerModelRoot[0].gameObject.SetActive(false);
            TakeParNames[0].text = "";
        }

        if (partner_1 != null)
        {
            Transform modelRoot = PartnerModelRoot[1];
            PnTouchObj[1].SetActive(false);//터치 라벨 끈다.
            PnTouchEff[1].SetActive(false);
            PartnerModelRoot[1].gameObject.SetActive(true);

            UIHelper.CreatePartnerUIModel(modelRoot, partner_1._partnerDataIndex, 3, true, false, "DungeonPanel");
            string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), partner_1._NowLevel);
            TakeParNames[1].text = string.Format("{0} {1}", lvStr, partner_1.GetLocName());
        }
        else
        {
            PnTouchObj[1].SetActive(true);//터치 라벨 킨다.
            PnTouchEff[1].SetActive(true);
            PartnerModelRoot[1].gameObject.SetActive(false);
            TakeParNames[1].text = "";
        }

    }

    #endregion
}

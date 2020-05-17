using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
using System.Text;
using WellFired;

public class InGameHUDPanel : UIBasePanel
{
    PlayerController playerCtlr;
    Unit leaderUnit;
    public Unit LeaderUnit
    {
        set
        {
            leaderUnit = value;
        }
        get
        {
            return leaderUnit;
        }
    }

    public List<Pc> AllPartners//모든 파트너 적&나
    {
        get
        {
            if (playerCtlr == null) return null;
            return playerCtlr.Partners;
        }
    }

    public Boss BossUnit
    {
        get
        {
            if (G_GameInfo.GameInfo == null || G_GameInfo.CharacterMgr == null || G_GameInfo.CharacterMgr.BossUnit == null)
                return null;
            return G_GameInfo.CharacterMgr.BossUnit;
        }
    }
    
    public UILabel NowStage;    //현재 스테이지표기

    public Transform JoyGroup;
    private Joystick Joy;

    /// <summary> 보스 체력 게이지 그룹 </summary>
    public GameObject BossGroup;
    /// <summary> 파트너 그룹 배열 </summary>
    public GameObject[] PartnerGroups;
    public GameObject[] SkillCoolTimeEff;//플레이어
    public GameObject[] ParSkillCoolTimeEff;//파트너

    public GameObject CountDownPanel;//카운트 다운 표기용 오브젝트.
    public GameObject EndGameText;//게임 종료 표기 라벨.
    public GameObject ArenaGroup;
    public GameObject NormalGroup;

    public GameObject FreeFightBattleGroup; //난투장관련 UI들
    public GameObject SelectChannelPopup; //난투장 채널선택팝업
    public GameObject RoomSlotPrefab;   //난투장방슬롯프리펩
    public GameObject CurQuest;//현재퀘스트
    public GameObject Warning_Img;  //경고이미지

    public Transform GoldTf;//골드던전에서 사용하는 상단 골드.
    public Transform CoinEffRoot;
    public ParticleSystem[] SubHpEffs;//0 플레이어, 1보스, 2아레나용 적

    public UIGrid RoomSlotRoot; //난투장채널팝업슬롯 부모

    public UIToggle SafetyZoneIcon;

    /// <summary> 공격 누르는 버튼 </summary>
    public UIRepeatButton AtkBtn;
    /// <summary> Btns의 인덱스에 사용될 타입. </summary>
    enum btnT { skill_0 = 0, skill_1, skill_2, skill_3, pskill_0, pskill_1, pauseBtn, AutoBtn }
    /// <summary> UI에서 사용하는 버튼 배열 </summary>
    public UIButton[] Btns;
    public UIButton ChangeChannelBtn; //난투장에서 채널변경할때 쓸버튼

    public Color MissionClearColor;
    public UISprite CountDown;//게임 시작전에 카운트 다운 세는 것
    
    /// <summary> Icons의 인덱스에 사용될 타입. </summary>
    enum iconT { hero = 0, heroClass, boss, partner_0, partner_1, attack, skill_0, skill_1, skill_2, skill_3, pskill_0, pskill_1, autoBtnIcon }
    /// <summary> UI에서 사용하는 아이콘 배열 </summary>
    public UISprite[] Icons;
    public UISprite[] SkillCoolTimeImg;
    public UISprite[] PartnerSkillCoolImg;
    public UISprite[] RoomUserImg;//Hp

    public UILabel[] HPLabel; //영웅, 보스 hp
    public UILabel Lv;  //영웅 LV
    public UILabel AttackLabel; //영웅전투력
    public UILabel CashLabel;   //금화

    /// <summary> 게임에 들어갈 퀘스트 목록 </summary>
    public GameObject CurStageQuestGo;  //스테이지퀘스트 클리어확인창 (별표시된곳)//얘가 움직일거임
    public UILabel[] QuestLabelArray;
    public UISprite[] QuestStartArray;
    public UILabel[] SkillCoolTimer;
    public UILabel[] PartnerSkillCoolTimer;
    public UILabel[] ArenaName;//0 left 1 right
    public UILabel playTimeLbl;

    public UILabel goldLabel;
    public TweenScale GoldTween;

    public GameObject AutoBtnEff;   //오토버튼이펙트
    //public UILabel AutoText;    //오토일시 화면에 뿌려줄 텍스트

    public UILabel FreefightLabel;  //채널변경알림/방변경 쿨타임에러 관련라벨

    /// <summary> Sliders의 인덱스에 사용될 타입. </summary>
    enum sliderT { heroHp = 0, bossHp, partner_0, partner_1, arenaEnemy, subHero, subBoss, subArena, spHero, spBoss, spArena, subSpHero, subSpArena
                    , parArena_0, parArena_1, spParArena_0, spParArena_1, spPar_0, spPar_1, chainTime}

    /// <summary> 각종 게이지에 사용되는 슬라이더이미지 배열 </summary>
    public UISprite[] Sliders;

    /// <summary> 보스이름 표기 라벨 </summary>
    public UILabel BossName;

    public int HpBlockCount = 8;

    public float PathOffset = 40;
    public float HpDownValue = 0.01f;
    public float SkillAlarm = 5f;
    public float[] HpWarningTimes = new float[] {
 0.2f, 0.2f, 0.5f, 0.2f, 0.2f, 0.2f, 0.5f, 0.2f, 0.5f,1f
    };
    
    /// <summary> 스테이지 퀘스트 리스트 </summary>
    private List<NetData.StageClearData> StageQuestList;
    private ChatPopup ChatPop;
    private MapPanel _MapPanel;
    private GameObject[] EffSkillAlarm;
    private TweenAlpha[] HpTweens;
    private TweenScale[] SpTweens;

    private GameObject BossTime;

    private Transform PathArrowTf;

    private Vector2[] PortalPos;
    private string PlayTimeLoc;

    private byte CurAttImgState;

    private float DelayPathEffTime;
    private float CallChainTime;
    private float AutoModeDelayTime = 0f;
    private float ChangeChannelCooltime = 0; //
    private float savedGoldCnt;
    private float[] SkillAlarmTime;
    
    private int MaxGoldCnt;

    private bool IsStopAutoMode = false;
    private bool IsWarning = false;

    private System.DateTime BossAlertTime;

    List<UISkillCoolTimeData> UseSkillList = new List<UISkillCoolTimeData>();//플레이어, 파트너의 사용 스킬 쿨타임 표기용

    struct UISkillCoolTimeData
    {
        public Skill skill;
        public UILabel coolText;
        public UISprite coolPer;
        public Collider col;
        public /*GameObject*/Transform coolEndEff;
    }

    public override void Init()
    {
        playerCtlr = G_GameInfo.PlayerController;

        InitJoystick();

        BtnSetting();

        EventDelegate.Add(onShow, OnShow);

        //EventListner.instance.RegisterListner("HUD_MSG", SetMessage);
        //EventListner.instance.RegisterListner("HUD_MSG_ICON", SetSkillIcon);
        //EventListner.instance.RegisterListner( "HUD_RAIDMSG", SetRaidMessage);
        EventListner.instance.RegisterListner("HUD_SHOWBOSS", ShowBossUI);

        BossTime = FreeFightBattleGroup.transform.FindChild("BosswarringGroup").FindChild("Txt_Deathtime").gameObject;

        SetStageQuestText();

        SettingIcon();

        //자동사냥에 관한 아이콘 설정
        string iconName = G_GameInfo.GameInfo.AutoMode ? "auto_btn_on" : "auto_btn_off";
        Icons[(uint)iconT.autoBtnIcon].spriteName = iconName;
        UIHelper.CreateEffectInGame(AutoBtnEff.transform, "Fx_UI_autoBtn_01");
        AutoBtnEff.SetActive(G_GameInfo.GameInfo.AutoMode);
        //AutoText.gameObject.SetActive(G_GameInfo.GameInfo.AutoMode);


        Btns[(uint)btnT.AutoBtn].normalSprite = iconName;

        GoldTf.gameObject.SetActive(false);
        NormalGroup.SetActive(true);
        ArenaGroup.SetActive(false);
        SafetyZoneIcon.gameObject.SetActive(false);
        //transform.FindChild("ArenaGroup/Start").gameObject.SetActive(false);

        PathArrowTf = UIHelper.CreateEffectInGame(transform, "Fx_IN_direction_001").transform;
        
        if (G_GameInfo.GameMode != GAME_MODE.FREEFIGHT)
        {
            FreeFightBattleGroup.SetActive(false);
            
            if (G_GameInfo.GameMode == GAME_MODE.ARENA)
            {
                GoldTween.gameObject.SetActive(false);
                PlayTimeLoc = "";

                InitArena();
            }
            else if(G_GameInfo.GameMode == GAME_MODE.SPECIAL_GOLD || G_GameInfo.GameMode == GAME_MODE.SPECIAL_EXP)
            {
                PlayTimeLoc = string.Format("{0} : ", _LowDataMgr.instance.GetStringCommon(451));
                GoldTween.gameObject.SetActive(false);
                GoldTf.gameObject.SetActive(true);

                GoldTf.FindChild("Get/get").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon((uint)(SpecialGameState.IsGoldStage ? 4 : 2)), 0);
                GoldTf.FindChild("Item/value").GetComponent<UILabel>().text = string.Format("x0");
                GoldTf.FindChild("Get/slider").GetComponent<UISprite>().fillAmount = 0;

                EventListner.instance.RegisterListner("COIN_LOOTING", COIN_LOOTING);

                DelayPathEffTime = -1f;//골드, 경험치일 경우 일단 경로표기 삭제.
                PathArrowTf.gameObject.SetActive(false);
                GoldTf.FindChild("Item/Gold").gameObject.SetActive(SpecialGameState.IsGoldStage);
                GoldTf.FindChild("Item/Exp").gameObject.SetActive( !SpecialGameState.IsGoldStage);
                GoldTf.FindChild("Get/Gold").gameObject.SetActive(SpecialGameState.IsGoldStage);
                GoldTf.FindChild("Get/Exp").gameObject.SetActive( !SpecialGameState.IsGoldStage);
            }
            else
            {
                PlayTimeLoc = string.Format("{0} : ", _LowDataMgr.instance.GetStringCommon(451));

                EventListner.instance.RegisterListner("COIN_LOOTING", COIN_LOOTING);
                GoldTween.gameObject.SetActive(true);
            }
        }
        else
        {
            //채널당 20개의룸이존재
            for (int i = 0; i < 20; i++)
            {
                GameObject slotGo = Instantiate(RoomSlotPrefab) as GameObject;
                Transform slotTf = slotGo.transform;
                slotTf = slotGo.transform;
                slotTf.FindChild("txt_roomnum").GetComponent<UILabel>().text = (i + 1).ToString();
                slotTf.parent = RoomSlotRoot.transform;
                slotTf.localPosition = Vector3.zero;
                slotTf.localScale = Vector3.one;

            }
            Destroy(RoomSlotPrefab);
            RoomSlotRoot.Reposition();

            if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                SafetyZoneIcon.gameObject.SetActive(true);
                FreeFightBattleGroup.SetActive(true);
                NetworkClient.instance.SendPMsgMessQueryC();
                //NetworkClient.instance.SendPMsgMessRoomQueryC((int)FreeFightGameInfo._SelectLastFreeFightType);

                DelayPathEffTime = -1f;//난투장일 경우 일단 경로표기 삭제.
                PathArrowTf.gameObject.SetActive(false);
            }
            else
            {
                FreeFightBattleGroup.SetActive(false);

            }

            GoldTween.gameObject.SetActive(false);

            playTimeLbl.gameObject.SetActive(false);

            if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
            {
                _MapPanel = UIMgr.OpenMapPanel();

               _MapPanel.SetResenMonster((int)FreeFightGameState.lastSelectStageId);

            }
        }

        TimeUpdate(0);//기본 0초로 시작하게 해준다.

        EffSkillAlarm = new GameObject[] {//스킬 사용하지 않으면 깜빡인다
            UIHelper.CreateEffectInGame(Btns[(int)btnT.skill_0].transform, "Fx_UI_icon_alarm_01"),
            UIHelper.CreateEffectInGame(Btns[(int)btnT.skill_1].transform, "Fx_UI_icon_alarm_01"),
            UIHelper.CreateEffectInGame(Btns[(int)btnT.skill_2].transform, "Fx_UI_icon_alarm_01"),
            UIHelper.CreateEffectInGame(Btns[(int)btnT.skill_3].transform, "Fx_UI_icon_alarm_01"),
            UIHelper.CreateEffectInGame(Btns[(int)btnT.pskill_0].transform, "Fx_UI_icon_alarm_01"),
            UIHelper.CreateEffectInGame(Btns[(int)btnT.pskill_1].transform, "Fx_UI_icon_alarm_01")
        };

        SkillAlarmTime = new float[EffSkillAlarm.Length];
        for (int i = 0; i < SkillAlarmTime.Length; i++)
        {
            SkillAlarmTime[i] = SkillAlarm;
        }

        if(G_GameInfo.GameMode != GAME_MODE.ARENA)
        {
            SpTweens = new TweenScale[] {
                transform.FindChild("PotraitGroup/HeroInfo/Sp/Img_sheild").gameObject.AddComponent<TweenScale>(),
                transform.FindChild("BossHpGroup/super_bar/icon_sp").gameObject.AddComponent<TweenScale>()
            };
        }

        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            NowStage.text = SingleGameState.CurStageName;//.Substring(0,5);
            NowStage.gameObject.SetActive(true);

            //스테이지 클리어 
            CurStageQuestGo.transform.localPosition = new Vector3(-235, 0, 0);   //처음에접은채로시작해줌
            SetStageQuestState(ClearQuestType.UNEQUIP_PARTNER, AllPartners.Count);
            for (int i = 0; i < StageQuestList.Count; i++)//최초 클리어 정보 갱신
            {
                SetQuestStringColor(i, StageQuestList[i].IsClear);
                QuestLabelArray[i].gameObject.SetActive(false); //접혀있는상태에선 퀘스트 내용도 숨겨줘야함.
            }
        }
        else
            NowStage.gameObject.SetActive(false);

        InitUnits();

        HpTweens = new TweenAlpha[3];
        for (int i = 0; i < HpTweens.Length; i++)
        {
            HpTweens[i] = Sliders[i + (int)sliderT.subHero].gameObject.AddComponent<TweenAlpha>();
            HpTweens[i].from = Sliders[i + (int)sliderT.subHero].GetComponent<UISprite>().color.a;
            HpTweens[i].to = 0.2f;
            HpTweens[i].duration = 0.8f;
            HpTweens[i].style = UITweener.Style.PingPong;
        }

        Sliders[(int)sliderT.subHero].fillAmount = 1;
        Sliders[(int)sliderT.subBoss].fillAmount = 1;
        Sliders[(int)sliderT.subArena].fillAmount = 1;
        Sliders[(int)sliderT.chainTime].fillAmount = 0;

        for (int i=0; i < SubHpEffs.Length; i++)
        {
            SubHpEffs[i].transform.parent.gameObject.SetActive(false);
        }
    }

    public override void LateInit()
    {


        IsWarning = false;
        Lv.text = NetData.instance.UserLevel.ToString();
        AttackLabel.text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), NetData.instance._userInfo.RefreshTotalAttackPoint());
        CashLabel.text = NetData.instance.GetAsset(AssetType.Cash) == 0 ? "0" : NetData.instance.GetAsset(AssetType.Cash).ToString(); // ToString("#,##");

        //퀘스트 
        Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
        if (quest != null)
        {
            CurQuest.GetComponent<MissionListObject>().SetupMission(quest.Title, quest.LeftDescription);
            CurQuest.transform.FindChild("Lv").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(14), quest.LimitLevel);

        }
        
        if (G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            if (ChatPop == null)
                ChatPop = UIMgr.OpenChatPopup();//SceneManager.instance.ChatPopup();

            if (ChatPop != null)
                ChatPop.OnShow();

            if (FreeFightGameState.StateActive)
            {
                /*// 보류
                if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                {
                    DungeonTable.FreefightTableInfo freeInfo = _LowDataMgr.instance.GetLowDataDogFight(FreeFightGameState.lastSelectStageId);
                    List<string> posList = freeInfo.Regenposition.list;
                    PortalPos = new Vector2[] {
                        new Vector2( float.Parse(posList[0]), float.Parse(posList[1])),
                        new Vector2( float.Parse(posList[2]), float.Parse(posList[3]))
                    };

                    Btns[(int)btnT.pauseBtn].gameObject.SetActive(false);
                }
                */
            }

        }

        ChangeChannelCooltime = _LowDataMgr.instance.GetEtcTableValue<float>(EtcID.FreeFightChanelResettime);

        NetData._UserInfo charInven = NetData.instance.GetUserInfo();
        NetData._CostumeData costume = charInven.GetEquipCostume();
        uint costumeId = 0;
        
        if (costume == null)
        {
            List<PlayerUnitData> playerData = NetData.instance._playerSyncData.playerSyncDatas;
            for (int i = 0; i < playerData.Count; i++)
            {
                if (playerData[i]._TCPUUID != 0)
                    continue;

                costumeId = playerData[i]._CostumeItem;
                break;
            }
        }
        else
            costumeId = costume._costmeDataIndex;

        if (0 < costumeId)
        {
            uint weaponId = 0, clothId = 0, headId = 0;
            if (charInven.isHideCostum)
            {
                NetData._ItemData head = charInven.GetEquipParts(ePartType.HELMET);
                NetData._ItemData cloth = charInven.GetEquipParts(ePartType.CLOTH);
                NetData._ItemData weapon = charInven.GetEquipParts(ePartType.WEAPON);

                if (head != null)
                    headId = head._equipitemDataIndex;

                if (cloth != null)
                    clothId = cloth._equipitemDataIndex;

                if (weapon != null)
                    weaponId = weapon._equipitemDataIndex;
            }

            uint skillSetId = 0;
            if (charInven.GetEquipSKillSet() == null)
            {
                switch (charInven.GetCharIdx())
                {
                    case 11000:
                        skillSetId = 100;
                        break;
                    case 12000:
                        skillSetId = 200;
                        break;
                    case 13000:
                        skillSetId = 300;
                        break;
                }
            }
            else
            {
                skillSetId = charInven.GetEquipSKillSet().SkillSetId;
            }

            GameObject model = UIHelper.CreatePcUIModel("TownPanel", transform.FindChild("Camera/Rot"), charInven.GetCharIdx()
                , headId, costumeId, clothId, weaponId, skillSetId, 3, charInven.isHideCostum, true, false);
        }

        //스테이지퀘스트 클릭시 펴주거나 접어줘야함.
        EventDelegate.Set(CurStageQuestGo.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            TweenPosition tp = CurStageQuestGo.GetComponent<TweenPosition>();
            if (tp == null)
                tp = CurStageQuestGo.gameObject.AddComponent<TweenPosition>();

            //접혀잇을때 x값 :-235
            //펴있을땐 x값 :0
            float startPos = CurStageQuestGo.transform.localPosition.x < 0 ? -235 : 0;
            float EndPos = CurStageQuestGo.transform.localPosition.x < 0 ? 0 : -235;

            tp.onFinished.Clear();
            tp.ResetToBeginning();
            tp.from = new Vector3(CurStageQuestGo.transform.localPosition.x,
                                      CurStageQuestGo.transform.localPosition.y,
                                      CurStageQuestGo.transform.localPosition.z);

            tp.to = new Vector3(EndPos,
                                     CurStageQuestGo.transform.localPosition.y,
                                     CurStageQuestGo.transform.localPosition.z);


            tp.duration = 0.2f;
            tp.PlayForward();

            for (int i = 0; i < StageQuestList.Count; i++)
            {
                QuestLabelArray[i].gameObject.SetActive(EndPos == -235 ? false : true);
            }

        });

        for(int i=0; i < SpTweens.Length; i++)
        {
            SpTweens[i].to = new Vector3(1.2f, 1.2f, 1.2f);
            SpTweens[i].duration = 0.5f;
        }
    }
    
    void InitArena()
    {
        NormalGroup.SetActive(false);
        ArenaGroup.SetActive(true);
        playTimeLbl.gameObject.SetActive(false);

        Transform arenaTf = ArenaGroup.transform;
        Sliders[(int)sliderT.heroHp] = arenaTf.FindChild("LeftHp/GaugeGroup/HeroHpSlider").GetComponent<UISprite>();
        Sliders[(int)sliderT.subHero] = arenaTf.FindChild("LeftHp/GaugeGroup/SubHeroHpSlider").GetComponent<UISprite>();
        SubHpEffs[0] = arenaTf.FindChild("LeftHp/GaugeGroup/Fx_UI_HP_01/Fx_all/P_glow").GetComponent<ParticleSystem>();
        Sliders[(int)sliderT.spHero] = arenaTf.FindChild("LeftHp/GaugeGroup/HeroSpSlider").GetComponent<UISprite>();
        Sliders[(int)sliderT.subSpHero] = arenaTf.FindChild("LeftHp/GaugeGroup/SubSpSlider").GetComponent<UISprite>();

        Sliders[(int)sliderT.partner_0] = arenaTf.FindChild("LeftHp/Partner00/hp").GetComponent<UISprite>();
        Sliders[(int)sliderT.spPar_0] = arenaTf.FindChild("LeftHp/Partner00/sp").GetComponent<UISprite>();
        Sliders[(int)sliderT.partner_1] = arenaTf.FindChild("LeftHp/Partner01/hp").GetComponent<UISprite>();
        Sliders[(int)sliderT.spPar_1] = arenaTf.FindChild("LeftHp/Partner01/sp").GetComponent<UISprite>();

        arenaTf.FindChild("LeftHp/level").GetComponent<UILabel>().text = NetData.instance.GetUserInfo()._Level.ToString();
        arenaTf.FindChild("LeftHp/Playerame_1").GetComponent<UILabel>().text = NetData.instance.GetUserInfo()._charName;
        arenaTf.FindChild("LeftHp/attack").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), NetData.instance._userInfo.RefreshTotalAttackPoint() );

        SpTweens = new TweenScale[] {
            arenaTf.FindChild("LeftHp/Img_sheild").gameObject.AddComponent<TweenScale>(),
            arenaTf.FindChild("RightHp/Img_sheild").gameObject.AddComponent<TweenScale>()
        };

        arenaTf.FindChild(string.Format("LeftHp/Partner0{0}", 0)).gameObject.SetActive(false);
        arenaTf.FindChild(string.Format("LeftHp/Partner0{0}", 1)).gameObject.SetActive(false);

        int parCount = 0;
        for (int i = 0; i < AllPartners.Count; i++)
        {
            if (AllPartners[i].TeamID != LeaderUnit.TeamID )
                continue;

            arenaTf.FindChild(string.Format("LeftHp/Partner0{0}", parCount)).gameObject.SetActive(true);
            Partner.PartnerDataInfo info = _LowDataMgr.instance.GetPartnerInfo(AllPartners[i].CharInfo.CharIndex);
            arenaTf.FindChild(string.Format("LeftHp/Partner0{0}", parCount)).gameObject.SetActive(true);
            arenaTf.FindChild(string.Format("LeftHp/Partner0{0}/face", parCount)).GetComponent<UISprite>().spriteName = info.PortraitId;
            ++parCount;
        }

        List<PlayerUnitData> syncList = NetData.instance._playerSyncData.playerSyncDatas;
        for (int i = 0; i < syncList.Count; i++)//적유닛.
        {
            if (syncList[i]._TeamID == LeaderUnit.TeamID || syncList[i]._isPartner)//적이니까 나하고 달라야겠지?
                continue;

            arenaTf.FindChild("RightHp/level").GetComponent<UILabel>().text = syncList[i]._Level.ToString();
            arenaTf.FindChild("RightHp/Playerame_1").GetComponent<UILabel>().text = syncList[i]._Name;
            arenaTf.FindChild("RightHp/attack").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(47), ArenaGameState.TargetAttack);
            break;
        }

        List<Unit> unitList = null;//적팀
        if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue((byte)eTeamType.Team2, out unitList))
        {
            arenaTf.FindChild("RightHp/Partner00").gameObject.SetActive(false);
            arenaTf.FindChild("RightHp/Partner01").gameObject.SetActive(false);
            
            int arr = 0;
            for (int i = 0; i < unitList.Count; i++)
            {
                if (unitList[i] == null)
                    continue;
                else if (!unitList[i].IsPartner)//플이어는 아이콘 셋팅
                {
                    GameObject model = UIHelper.CreatePcUIModel("CharacterPanel", arenaTf.FindChild("RightHp/Camera/Rot"), unitList[i].CharInfo.CharIndex
                        , unitList[i].CharInfo.EquipHead, unitList[i].CharInfo.EquipCostume, unitList[i].CharInfo.EquipCloth, unitList[i].CharInfo.EquipWeapon, unitList[i].CharInfo.SkillSetId,
                        3, unitList[i].CharInfo.EquipCostume == 0, true, false);

                    if (model.name.Contains("pc_f"))
                    {
                        model.transform.localPosition = new Vector3(0, -12, 0);
                    }
                    else if (model.name.Contains("pc_p"))
                    {
                        model.transform.localPosition = new Vector3(0, -4, 0);
                    }
                    else if (model.name.Contains("pc_d"))
                    {
                        model.transform.localPosition = new Vector3(0, 10.5f, 0);
                    }
                    
                    continue;
                }

                Partner.PartnerDataInfo info = _LowDataMgr.instance.GetPartnerInfo(unitList[i].CharInfo.CharIndex);
                arenaTf.FindChild(string.Format("RightHp/Partner0{0}", arr)).gameObject.SetActive(true);
                arenaTf.FindChild(string.Format("RightHp/Partner0{0}/face", arr)).GetComponent<UISprite>().spriteName = info.PortraitId;
                ++arr;
            }
        }

        playTimeLbl = ArenaGroup.transform.FindChild("play_time").GetComponent<UILabel>();//바꿔치기.
    }


    void ChargeSkillStart()
    {
        leaderUnit.ChargeStart();
    }

    void ChargeSkillEnd()
    {
        leaderUnit.ChargeEnd();
    }

    void BtnSetting()
    {
        //공격버튼 이벤트 연결
        EventDelegate.Set(AtkBtn.onPressing, AttackBtnPressedEvent);

        //스킬버튼 이벤트 연결
        for (int i = (int)btnT.skill_0; i <= (int)btnT.skill_3; i++)
        {
            if (leaderUnit.CharInfo.CharIndex == 13000 && i == 0)//챠지스킬
            {
                EventDelegate.Set((Btns[i] as UIRepeatButton).onFirstPress, ChargeSkillStart);
                //EventDelegate.Set((Btns[i] as UIRepeatButton).onPressing, delegate () {SkillPressTest();});
                EventDelegate.Set((Btns[i] as UIRepeatButton).onPressOut, ChargeSkillEnd);
            }
            else//일반 스킬
                UIEventListener.Get(Btns[i].gameObject).onClick = SkillBtnClkEvent;
        }

        //파트너스킬 이벤트 연결
        UIEventListener.Get(Btns[(int)btnT.pskill_0].gameObject).onClick = SpecialBtnClkEvent;
        UIEventListener.Get(Btns[(int)btnT.pskill_1].gameObject).onClick = SpecialBtnClkEvent;

        UIEventListener.Get(Btns[(int)btnT.pauseBtn].gameObject).onClick = OnClickPause;

        UIEventListener.Get(Btns[(int)btnT.AutoBtn].gameObject).onClick = OnClickAutoMode;

        // Btns[(int)btnT.AutoBtn].gameObject
        //if (SceneManager.isRTNetworkMode == GAME_MODE.FREEFIGHT) //<----------임시로 난투전은 무조건 수동모드로 진행되게 해 준다.
        //if (SceneManager.instance.IsRTNetwork)
        //{
        //Btns[(int)btnT.AutoBtn].gameObject.SetActive(false);
        //G_GameInfo.GameInfo.AutoMode = false;
        //} //<----------임시로 난투전은 무조건 수동모드로 진행되게 해 준다.

        EventDelegate.Set(ChangeChannelBtn.onClick, delegate ()
        {
//            SelectChannelPopup.SetActive(true);
//            //방정보 다시 불러오기 
//            NetworkClient.instance.SendPMsgMessRoomQueryC((int)FreeFightGameState.lastSelectStageId);

        });

        // 난투장 전투/안전지역 토글 메시지
        EventDelegate.Set(SafetyZoneIcon.onChange, delegate ()
        {
            StopCoroutine("AlertSafetyTxt");
            StartCoroutine("AlertSafetyTxt");
        });
    }

    public void FreeFightChannelLsit(List<NetData.MessInfo> room)
    {
        List<NetData.MessInfo> roomInfo = room;

        for (int i = 0; i < RoomSlotRoot.transform.childCount; i++)
        {
            GameObject slot = RoomSlotRoot.gameObject.transform.GetChild(i).gameObject;
            if (i >= roomInfo.Count)
            {
                slot.SetActive(false);
                continue;
            }

            int idx = i;
            slot.SetActive(true);
            UIEventTrigger tri = slot.GetComponent<UIEventTrigger>();
            EventDelegate.Set(tri.onClick, delegate ()
            {
                // 채널변경
                //NetworkClient.instance.SendPMsgMessChangeRoomC(roomInfo[idx].Id);
            });
        }
        RoomSlotRoot.Reposition();
    }
    public void closeChannelPopup()
    {
        if (FreefightLabel.gameObject.activeSelf)
        {
            FreefightLabel.gameObject.SetActive(false);
            return;
        }
        SelectChannelPopup.SetActive(false);
    }
    // 난투 장 방교체 쿨타임시 알림
    public void AlertFreefightCoolTime()
    {
        SelectChannelPopup.SetActive(false);

        FreefightLabel.gameObject.SetActive(true);

        StartCoroutine("ChangeCoolTime");
    }

    IEnumerator ChangeCoolTime()
    {
        while (true)
        {
            FreefightLabel.text = string.Format("{0} {1:00} : {2:00}", _LowDataMgr.instance.GetStringCommon(332), ChangeChannelCooltime / 60, ChangeChannelCooltime % 60);
            yield return new WaitForSeconds(1f);

            if (ChangeChannelCooltime < 0)
                break;
        }

        yield return new WaitForSeconds(0.1f);
        FreefightLabel.gameObject.SetActive(false);

    }

    /// <summary> 기존에 있던 함수인데 이걸로 플레이 타임 업데이트 해준다. 좀더 채계적으로 관리가 필요 할 듯 하지만... </summary>
    public void TimeUpdate(float time)//(int time)
    {
        float m = Mathf.Floor(time / 60);
        float s = Mathf.Floor(time % 60);

        playTimeLbl.text = string.Format("{0} : {1}", m.ToString("00"), s.ToString("00"));
        //playTimeLbl.text = string.Format("{0}{1} : {2}", PlayTimeLoc, m.ToString("00"), s.ToString("00"));
        SetStageQuestState(ClearQuestType.TIME_LIMIT, time);
    }

    public void COIN_LOOTING(object obj)
    {
        //GoldTween.ResetToBeginning();
        //GoldTween.Play(true);

        savedGoldCnt += (float)obj;
        if (MaxGoldCnt < savedGoldCnt)
            savedGoldCnt = MaxGoldCnt;

        if (G_GameInfo.GameMode != GAME_MODE.SPECIAL_GOLD && G_GameInfo.GameMode != GAME_MODE.SPECIAL_EXP)
        {
            goldLabel.text = Mathf.CeilToInt(savedGoldCnt).ToString();

            GameObject go = UIHelper.CreateEffectInGame(CoinEffRoot, "Fx_IN_coin_01");
            Destroy(go, 1f);
            SoundManager.instance.PlaySfxSound(eUISfx.UI_gold_get, false);
        }
        else
        {
            GoldTf.FindChild("Get/get").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon( (uint)(SpecialGameState.IsGoldStage ? 4 : 2)), Mathf.CeilToInt(savedGoldCnt).ToString() );
            GoldTf.FindChild("Get/slider").GetComponent<UISprite>().fillAmount = savedGoldCnt/(float)MaxGoldCnt;
            //GoldTf.FindChild("Item/value").GetComponent<UILabel>().text = string.Format("x0");
        }
    }
    
    /// <summary> 실제 게임이 시작되면 들어오는 함수. </summary>
    public void GameStart()
    {
        if (G_GameInfo.GameMode == GAME_MODE.SPECIAL_GOLD || G_GameInfo.GameMode == GAME_MODE.SPECIAL_EXP)
        {
            MaxGoldCnt = (int)(G_GameInfo.GameInfo as SpecialGameInfo).MaxValue;
            SpecialKillCount(0);
        }
        else if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode != GAME_MODE.FREEFIGHT)//멀티보스레이드 or 콜로세움
        {
            Transform infoTf = transform.FindChild("ColosseumGroup");
            infoTf.gameObject.SetActive(true);
            infoTf.GetChild(0).gameObject.SetActive(false);
            infoTf.GetChild(1).gameObject.SetActive(false);

            List<Unit> unitList = null;
            if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue((byte)eTeamType.Team1, out unitList))
            {
                int arr = 0;
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i] == null)
                        continue;
                    else if (unitList[i] == LeaderUnit)//나는 뺀다
                        continue;

                    Transform tf = infoTf.GetChild(arr);
                    tf.gameObject.SetActive(true);

                    tf.FindChild("PotraitIcon").GetComponent<UISprite>().spriteName = UIHelper.GetClassPortIcon(unitList[i].CharInfo.CharIndex, 2);
                    tf.FindChild("name").GetComponent<UILabel>().text = unitList[i].CharInfo.UnitName;
                }
            }
        }
        else if (G_GameInfo.GameMode == GAME_MODE.ARENA)
        {

        }
        else
        {
            if (NetData.instance._RewardData != null)
                MaxGoldCnt = NetData.instance._RewardData.GetCoin;
        }

        Unit leader = null;
        if (G_GameInfo.CharacterMgr.allrUUIDDic.TryGetValue(NetData.instance.GetUserInfo().GetCharUUID(), out leader))
            leader.inputCtlr.StartJoystick(Joy, CameraManager.instance.mainCamera);
    }

    public void GameEnd(bool isSuccess, bool isEndGameActive = false)
    {
        SetJoyActive(false);//조이스틱 정지

        Time.timeScale = 1;
        GameDefine.DefaultTimeScale = 1.1f;

        EndGameText.SetActive(isEndGameActive);

        //클리어 했다면 퀘스트 갱신
        SetStageQuestState(ClearQuestType.STAGE_CLEAR, isSuccess ? 1 : -1);

        if (IsStopAutoMode && !G_GameInfo.GameInfo.AutoMode)//조이스틱으로 움직이는 것이 남아있다. 강제로 오토모드 true로 저장시켜준다.
        {
            IsStopAutoMode = false;
            G_GameInfo.GameInfo.AutoMode = true;
        }
    }

    void InitUnits()
    {
        //일단 파트너 존재 여부는 확인되지 않으므로 그룹 비활성화
        PartnerGroups[0].SetActive(false);
        PartnerGroups[1].SetActive(false);
        Unit leader = null;
        if (G_GameInfo.CharacterMgr.allrUUIDDic.TryGetValue(NetData.instance.GetUserInfo().GetCharUUID(), out leader))
        {
            List<Unit> myUnitList = null;
            if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue(leader.TeamID, out myUnitList))
            {
                //파트너 수만큼 그룹 활성화 시키고 체력게이지 세팅
                //if (Partners != null && Partners.Count > 0)
                {
                    int parCount = 0;
                    for (int i = 0; i < myUnitList.Count; i++)
                    {
                        if (myUnitList[i].IsPartner)
                            PartnerGroups[parCount++].SetActive(true);
                    }
                }
            }

        }

        //보스 그룹도 일단 비활성화
        BossGroup.SetActive(false);
    }
    
    void SliderAction(float fill, UISprite fillSp, UISprite subFillSp, ParticleSystem particle, TweenAlpha alpha, TweenScale scale, bool isEffReverse=false)
    {
        if (fillSp.fillAmount != fill)
        {
            if (fillSp.fillAmount != 0 && scale != null && fillSp.fillAmount != fill && fill < fillSp.fillAmount)//다르고 회복이 아닌경우에
            {
                scale.ResetToBeginning();
                scale.PlayForward();
            }
          
            fillSp.fillAmount = fill;
        }

        if (subFillSp.fillAmount < fill)
            subFillSp.fillAmount = fill;

        float minValue = 1 / (float)HpBlockCount;//0.125
        int count = (int)(subFillSp.fillAmount * HpBlockCount);

        if (fill <= 0 || fill < (count * minValue))
        {
            if (alpha != null && alpha.enabled)
            {
                alpha.ResetToBeginning();
                alpha.enabled = false;
            }

            subFillSp.fillAmount -= (HpDownValue * 0.2f);
            if (particle != null && !particle.isPlaying)
                particle.Play();

            if (subFillSp.fillAmount <= fill)
            {

                subFillSp.fillAmount = fill;
                if (particle != null && particle.isPlaying)
                {
                    particle.Stop();

                    if (subFillSp.fillAmount <= 0)
                        particle.transform.parent.gameObject.SetActive(false);
                }
            }

            if (particle != null)
            {
                Vector3 effPos = particle.transform.parent.localPosition;
                effPos.x = (subFillSp.fillAmount * subFillSp.width) + subFillSp.transform.localPosition.x;
                particle.transform.parent.localPosition = isEffReverse ? -effPos : effPos;
            }
        }
        else if (alpha != null && !alpha.enabled)
        {
            alpha.enabled = true;
            alpha.PlayForward();
        }
    }

    public void StopWarning()
    {
        //WarningTime = 0;
        //WarningTimeCount = 0;
        IsWarning = false;
        Warning_Img.SetActive(false);
    }

    /// <summary> 게이지 관련 업데이트 </summary>
    void SliderUpdater()
    {
        if (LeaderUnit != null)
        {
            //영웅 캐릭터 hp게이지 업데이트
            float hpFill = (float)LeaderUnit.CharInfo.Hp / (float)LeaderUnit.CharInfo.MaxHp;

            if (hpFill < 0.1f) {
                Warning_Img.SetActive(true);
            }
            else
            {
                Warning_Img.SetActive(false);
            }

            if (LeaderUnit.ChainEndTime != float.MaxValue)
            {
                float time = LeaderUnit.ChainEndTime-Time.time;
                float fill = time/CallChainTime;
                Sliders[(int)sliderT.chainTime].fillAmount = fill;
            }

            SetStageQuestState(ClearQuestType.HP_PERCENT, hpFill);
            HPLabel[0].text = string.Format("{0}/{1}", (int)LeaderUnit.CharInfo.Hp, (int)LeaderUnit.CharInfo.MaxHp);
            SliderAction(hpFill, Sliders[(int)sliderT.heroHp], Sliders[(int)sliderT.subHero], null, HpTweens[0], null);//SubHpEffs[0]
            if (0 < hpFill)
                SliderAction((float)LeaderUnit.CharInfo.SuperArmor/(float)LeaderUnit.CharInfo.MaxSuperArmor
                    , Sliders[(int)sliderT.spHero], Sliders[(int)sliderT.subSpHero], null, null, SpTweens[0]);
        }

        //파트너 캐릭터들 hp게이지 업데이트
        if (AllPartners != null && AllPartners.Count != 0)
        {
            int dieCount = 0;
            for(int i=0; i < AllPartners.Count; i++)
            {
                if (AllPartners[i].TeamID != LeaderUnit.TeamID)
                    continue;

                int slot = AllPartners[i].syncData._SlotNo;
                if (AllPartners[i].IsDie)
                {
                    Icons[slot == 1 ? (uint)iconT.pskill_0 : (uint)iconT.pskill_1].color = Color.gray;
                    Btns[slot == 1 ? (uint)btnT.pskill_0 : (uint)btnT.pskill_1].collider.enabled = false;
                    SkillAlarmTime[slot == 1 ? (uint)btnT.pskill_0 : (uint)btnT.pskill_1] = -1f;
                    EffSkillAlarm[slot == 1 ? (uint)btnT.pskill_0 : (uint)btnT.pskill_1].SetActive(false);

                    PartnerGroups[slot-1].transform.FindChild("Partner01Icon_d1").GetComponent<UISprite>().color = Color.gray;
                    ++dieCount;
                }

                float parHp = (float)AllPartners[i].CharInfo.Hp / (float)AllPartners[i].CharInfo.MaxHp;
                float parSp = (float)AllPartners[i].CharInfo.SuperArmor / (float)AllPartners[i].CharInfo.MaxSuperArmor;

                Sliders[(int)sliderT.partner_0 + slot-1].fillAmount = parHp;
                Sliders[(int)sliderT.spPar_0 + slot-1].fillAmount = parSp;
            }
            /*
             if (0 < Partners.Count && Partners[0].IsDie)
            {
                Icons[(uint)iconT.pskill_0].color = Color.gray;
                Btns[(uint)btnT.pskill_0].collider.enabled = false;
                //IsPartnerDie[0] = true;
                ++dieCount;

                SkillAlarmTime[(uint)btnT.pskill_0] = -1f;
                EffSkillAlarm[(uint)btnT.pskill_0].SetActive(false);

                PartnerGroups[0].transform.FindChild("Partner01Icon_d1").GetComponent<UISprite>().color = Color.gray;
            }

            if (1 < Partners.Count && Partners[1].IsDie)
            {
                Icons[(uint)iconT.pskill_1].color = Color.gray;
                Btns[(uint)btnT.pskill_1].collider.enabled = false;
                //IsPartnerDie[1] = true;
                ++dieCount;

                SkillAlarmTime[(uint)btnT.pskill_1] = -1f;
                EffSkillAlarm[(uint)btnT.pskill_1].SetActive(false);

                PartnerGroups[1].transform.FindChild("Partner01Icon_d1").GetComponent<UISprite>().color = Color.gray;

            }
            
            for (int i = 0; i < AllPartners.Count; i++)
            {
                float parHp = (float)AllPartners[i].CharInfo.Hp / (float)AllPartners[i].CharInfo.MaxHp;
                float parSp = (float)AllPartners[i].CharInfo.SuperArmor / (float)AllPartners[i].CharInfo.MaxSuperArmor;

                Sliders[(int)sliderT.partner_0 + i].fillAmount = parHp;
                Sliders[(int)sliderT.spPar_0 + i].fillAmount = parSp;
            }
            */

            if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
            {
                SetStageQuestState(ClearQuestType.NO_DIE_PARTNER, dieCount);
            }
        }

        if (G_GameInfo.GameMode == GAME_MODE.ARENA)
        {
            List<Unit> unitList = null;//적팀
            if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue((byte)eTeamType.Team2, out unitList))
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i] == null)
                        continue;
                    else if (unitList[i].IsPartner)
                    {
                        float parHp = (float)unitList[i].CharInfo.Hp / (float)unitList[i].CharInfo.MaxHp;
                        float parSp = (float)unitList[i].CharInfo.SuperArmor / (float)unitList[i].CharInfo.MaxSuperArmor;

                        Sliders[(int)sliderT.parArena_0 + i-1].fillAmount = parHp;
                        Sliders[(int)sliderT.spParArena_0 + i-1].fillAmount = parSp;
                    }
                    else
                    {
                        float hpFill = (float)unitList[i].CharInfo.Hp / (float)unitList[i].CharInfo.MaxHp;
                        SliderAction(hpFill, Sliders[(int)sliderT.arenaEnemy], Sliders[(int)sliderT.subArena], null, HpTweens[2], null, true);//SubHpEffs[2]
                        if (0 < hpFill)
                            SliderAction((float)unitList[i].CharInfo.SuperArmor / (float)unitList[i].CharInfo.MaxSuperArmor
                                , Sliders[(int)sliderT.spArena], Sliders[(int)sliderT.subSpArena], null, null, SpTweens[1], true);
                    }
                }
            }
        }

        //보스 캐릭터 hp게이지 업데이트
        if (BossUnit != null)
        {
            float hpFill = (float)BossUnit.CharInfo.Hp / (float)BossUnit.CharInfo.MaxHp;
            if (Sliders[(int)sliderT.spBoss].fillAmount != 0 && hpFill != Sliders[(int)sliderT.bossHp].fillAmount && hpFill < Sliders[(int)sliderT.bossHp].fillAmount)
            {
                SpTweens[1].ResetToBeginning();
                SpTweens[1].PlayForward();
            }

            SliderAction(hpFill, Sliders[(int)sliderT.bossHp], Sliders[(int)sliderT.subBoss], null, HpTweens[1], null);//SubHpEffs[1]
            HPLabel[1].text = string.Format("{0}/{1}", (int)BossUnit.CharInfo.Hp, (int)BossUnit.CharInfo.MaxHp);

            if (0 < hpFill)
            {
                float armor = (float)BossUnit.CharInfo.SuperArmor / (float)BossUnit.CharInfo.MaxSuperArmor;
                Sliders[(int)sliderT.spBoss].fillAmount = armor;
            }   
        }

        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT)
        {
            if (FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)
                return;

            List<Unit> unitList = null;
            if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue((byte)eTeamType.Team1, out unitList))
            {
                int arr = 0;
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i] == null)
                        continue;
                    else if (unitList[i] == LeaderUnit)//나는 뺀다
                        continue;

                    float fill = (float)unitList[i].CharInfo.Hp / (float)unitList[i].CharInfo.MaxHp;
                    RoomUserImg[arr++].fillAmount = fill;
                }
            }
        }
    }

    void InitJoystick()
    {
        LeaderUnit = playerCtlr.Leader;
        Joy = UIMgr.OpenJoystick(JoyGroup.localPosition);//ResourceMgr.InstAndGetComponent<Joystick>("UI/UIObject/Joystick");//JoystickComponent
        //Joy.transform.SetParent(JoyGroup);
        //Joy.transform.localPosition = Vector3.zero;
    }

    /// <summary> 조이스틱 활성화 여부에 따라서 채팅그룹을 키고 꺼준다. </summary>
    void JoystickCheckedActive()
    {
        if (Joy != null)//ejoy
        {
            //자동모드인데 조이스틱 입력이 들어왔다.
            if (G_GameInfo.GameInfo.AutoMode == true && Joy.IsPress) //ejoy.IsJoystickActive == true)
            {
                //자동 모드 비활성화 시켜주자.
                G_GameInfo.GameInfo.AutoMode = false;
                if (LeaderUnit.CurrentState == UnitState.Attack)
                    LeaderUnit.ChangeState(UnitState.Move);

                //그리고 코루틴 하나 만들어서 조이스틱 입력이 없어지면 몇초후 다시 자동모드로 변경해준다.
                IsStopAutoMode = true;
            }
        }

        if (!IsStopAutoMode || Joy.IsPress)
        {
            AutoModeDelayTime = 0;
            return;
        }

        AutoModeDelayTime += Time.deltaTime;
        if (2.5f <= AutoModeDelayTime)
        {
            //혹시 모르니까 다시 검사
            if (!Joy.IsPress)//(ejoy.IsJoystickActive == false)
            {
                G_GameInfo.GameInfo.AutoMode = true;
                AutoModeDelayTime = 0;
                IsStopAutoMode = false;
            }
        }
    }

    void Update()
    {
        //게이지 관련 업데이트
        SliderUpdater();

        if (!G_GameInfo.GameInfo.IsStartGame)
            return;
        
        //조이스틱 활성화시 채팅그룹 비활성화 관련
        JoystickCheckedActive();

        //스킬 쿨타임
        SkillCoolTimeUpdate();
        
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT/* && BossTime.activeSelf*/ )
        {
            SetSafetyZone();
            //BossAlertTime -= Time.deltaTime;
            if (BossTime.activeSelf)
            {
                System.TimeSpan time = BossAlertTime - System.DateTime.Now;
                if (0 <= time.TotalSeconds)
                    BossTime.GetComponent<UILabel>().text = string.Format("{0:00} : {1:00}", time.Minutes, time.Seconds);
            }


        }

        ////무료?유료부활??
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    NetworkClient.instance.SendPMsgRoleReliveC();
        //}

    }

    private bool IsPossiblePortal;
    void LateUpdate()
    {
        /*// 보류
        if (PortalPos != null)
        {
            if (!IsPossiblePortal)
            {
                if (PortalPos[0].x <= LeaderUnit.BeforePosX && PortalPos[0].y >= LeaderUnit.BeforePosY &&
                    LeaderUnit.BeforePosX <= PortalPos[1].x && LeaderUnit.BeforePosY >= PortalPos[1].y)//포탈 가능
                {
                    OnClickPause();
                    IsPossiblePortal = true;
                }
            }
            else
            {
                if (LeaderUnit.BeforePosX < PortalPos[0].x || LeaderUnit.BeforePosY > PortalPos[0].y ||
                    PortalPos[1].x < LeaderUnit.BeforePosX || PortalPos[1].y > LeaderUnit.BeforePosY)//포탈 조건 불가능
                {
                    UIBasePanel optionPanel = UIMgr.GetUIBasePanel("UIPanel/OptionPanel");
                    if (optionPanel != null)
                    {
                        (optionPanel as OptionPanel).Close();
                    }

                    IsPossiblePortal = false;
                }
            }
        }
        */

        if (DelayPathEffTime == -1)
            return;

        if (0 < DelayPathEffTime)
        {
            DelayPathEffTime -= Time.deltaTime;
            if (DelayPathEffTime <= 0)
                DelayPathEffTime = 0;
            else
                return;
        }

        if (!G_GameInfo.GameInfo.AutoMode || IsStopAutoMode)
        {
            if (playerCtlr.Leader.CurrentState == UnitState.Idle || playerCtlr.Leader.CurrentState == UnitState.Move)
            {
                if( !PathArrowTf.gameObject.activeSelf)
                    PathArrowTf.gameObject.SetActive(true);

                SearchTargetPath();
            }
            else
            {
                PathArrowTf.gameObject.SetActive(false);
                DelayPathEffTime = 0.5f;
            }
        }
        else
        {
            DelayPathEffTime = 0.5f;
            PathArrowTf.gameObject.SetActive(false);
        }
        
    }

    void SearchTargetPath()
    {
        Vector3 endPos = Vector3.zero;

        /*//작업은 해놨지만 난투장일 경우 방향표기 안한다.
        if (G_GameInfo.GameMode == GAME_MODE.FREEFIGHT && FreeFightGameState.GameMode == GAME_MODE.FREEFIGHT)//보스 나와있는지 체크
        {
            FreeFightGameInfo.BossData bossData = (G_GameInfo.GameInfo as FreeFightGameInfo).GetBossData();
            if (bossData == null || bossData.Daed == 1)//없으면 끝점이 목표
                endPos = G_GameInfo.GameInfo.GetSpawnTransform(eTeamType.Team2).position;
            else//있으면 보스로 방향표기.
                endPos = NaviTileInfo.instance.GetTilePos(bossData.PosX, bossData.PosY);
        }
        else*/
        //{
        if (G_GameInfo.GameMode == GAME_MODE.ARENA)//차관일 경우 대상이 목표
        {
            List<Unit> unitList = null;
            if (G_GameInfo.CharacterMgr.allTeamDic.TryGetValue((byte)eTeamType.Team2, out unitList))
            {
                for (int i = 0; i < unitList.Count; i++)
                {
                    if (unitList[i] == null || unitList[i].IsPartner)
                        continue;

                    endPos = unitList[i].cachedTransform.position;
                    break;
                }
            }
            
            if(endPos == Vector3.zero)
            {
                DelayPathEffTime = 0.5f;
                PathArrowTf.gameObject.SetActive(false);
                return;
            }
        }
        else//다른곳들은 목표가 끝점임.
            endPos = G_GameInfo.GameInfo.GetSpawnTransform(eTeamType.Team2).position;

        Vector3 startPos = LeaderUnit.cachedTransform.position;
        NavMeshPath movePath = new NavMeshPath();
        if (!NavMesh.CalculatePath(startPos, endPos, -1, movePath) || movePath.corners.Length <= 1)
        {
            if (PathArrowTf.gameObject.activeSelf)
                PathArrowTf.gameObject.SetActive(false);

            return;
        }

        // 시작점과 끝점은 계산에서 제외시킴
        NavMeshHit navHit;
        for (int i = 1; i < movePath.corners.Length - 1; i++)
        {
            // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
            if (NavMesh.FindClosestEdge(movePath.corners[i], out navHit, 1))
            {
                if ((navHit.position - movePath.corners[i]).sqrMagnitude < 1f)
                {
                    movePath.corners[i] = movePath.corners[i] + navHit.normal * 1f;
                }
            }
        }

        // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
        if (movePath.corners.Length >= 2)
        {
            // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
            for (int i = 1; i < movePath.corners.Length - 1; ++i)
            {
                if ((movePath.corners[i] - movePath.corners[i + 1]).sqrMagnitude < 2f)
                    movePath.corners[i + 1] = movePath.corners[i];
            }
        }

        if (Vector3.Distance(startPos, movePath.corners[movePath.corners.Length - 1]) <= 3f)
        {
            if (PathArrowTf.gameObject.activeSelf)
                PathArrowTf.gameObject.SetActive(false);

            return;
        }

        int arr = 0;
        if (1 < movePath.corners.Length)
            arr = 1;

        endPos = movePath.corners[arr];
        if (!PathArrowTf.gameObject.activeSelf)
            PathArrowTf.gameObject.SetActive(true);

        var newRotation = Quaternion.LookRotation(endPos - startPos).eulerAngles;
        
        newRotation.z = PathOffset - newRotation.y;
        newRotation.x = 0;
        newRotation.y = 0;
        PathArrowTf.eulerAngles = newRotation;

        //var newRotation = Quaternion.LookRotation(startPos - endPos).eulerAngles;

        //newRotation.z = 190 - newRotation.y;
        //newRotation.x = 0;
        //newRotation.y = 0;
        //PathArrowTf.eulerAngles = newRotation;
    }

    public override void OnDestroy()
    {
        Time.timeScale = 1;
        GameDefine.DefaultTimeScale = 1.1f;

        if (EventListner.instance != null)
        {
            //EventListner.instance.RemoveEvent("HUD_MSG", SetMessage);
            //EventListner.instance.RemoveEvent("HUD_RAIDMSG", SetRaidMessage);
            EventListner.instance.RemoveEvent("HUD_SHOWBOSS", ShowBossUI);
            //EventListner.instance.RemoveEvent( "CheckPickUpItem", CheckPickUpItem );
            EventListner.instance.RemoveEvent("COIN_LOOTING", COIN_LOOTING);
        }

        base.OnDestroy();
    }

    #region :: UI Events ::

    /// <summary>
    /// 자동사냥 클릭 이벤트
    /// </summary>
    void OnClickAutoMode(GameObject go)
    {
        if (null == playerCtlr)
            return;

        bool newState = !G_GameInfo.GameInfo.AutoMode;
        string iconName = newState ? "auto_btn_on" : "auto_btn_off";
        Icons[(uint)iconT.autoBtnIcon].spriteName = iconName;
        Btns[(uint)btnT.AutoBtn].normalSprite = iconName;

        AutoBtnEff.SetActive(newState);
        //AutoText.gameObject.SetActive(newState);

        
        ////< 사운드실행
        //SoundHelper.UIClickSound((uint)eUIClickSoundType.AutoGamoeClick);
        /*
        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            if (newState == false)
                G_GameInfo.GameInfo.SearchPathEffect();
            else
            {
                G_GameInfo.GameInfo.ActivePathEffect(false);
            }
        }
        */
        SetAutoMode(newState);
    }

    public void SetAutoMode(bool state)
    {
        if (IsStopAutoMode)
        {
            IsStopAutoMode = false;
            G_GameInfo.GameInfo.AutoMode = false;

            bool newState = G_GameInfo.GameInfo.AutoMode;
            string iconName = newState ? "auto_btn_on" : "auto_btn_off";
            Icons[(uint)iconT.autoBtnIcon].spriteName = iconName;
            Btns[(uint)btnT.AutoBtn].normalSprite = iconName;
            AutoBtnEff.SetActive(newState);

            PlayerController controller = G_GameInfo.PlayerController;
            if (controller != null && controller.Leader != null)
            {
                Unit leader = controller.Leader;
                if (leader.CurrentState == UnitState.Attack || leader.CurrentState == UnitState.Move)
                {
                    leader.ChangeState(UnitState.Idle);
                    leader.ClearPath();
                }

            }

            return;
        }

        G_GameInfo.GameInfo.AutoMode = state;

        //< 리더가 공격상태거나 무브투스킬상태라면 변경
        if (!state)
        {
            PlayerController controller = G_GameInfo.PlayerController;
            if (controller != null && controller.Leader != null)
            {
                Unit leader = controller.Leader;
                if (leader.CurrentState == UnitState.Attack || leader.CurrentState == UnitState.Move)
                {
                    leader.ChangeState(UnitState.Idle);
                    leader.ClearPath();
                }

            }
        }
        else
        {
            IsStopAutoMode = false;
        }
    }

    /// <summary> 일시정지 버튼 클릭 이벤트 </summary>
    void OnClickPause(GameObject go = null)
    {
        if (null == playerCtlr)
            return;

        if (ChatPop != null && ChatPop.IsHideBigPop)
            return;

        if (!G_GameInfo.GameInfo.IsStartGame)
            return;
        
        //옵션창으로 넘어가기
        if (ChatPop != null)
            ChatPop.Hide();

        if(G_GameInfo.GameMode==GAME_MODE.ARENA)
        {
            UIMgr.OpenOptionPanel(false, 2);
        }
        else
        {
            UIMgr.OpenOptionPanel(!SceneManager.instance.IsRTNetwork, 2);
        }

        SetJoyActive(false);
        
    }

    void OnClickPauseBtnOk()
    {
        if (!SceneManager.instance.IsRTNetwork)
            Time.timeScale = 1;

        if(G_GameInfo.GameMode != GAME_MODE.SPECIAL_EXP && G_GameInfo.GameMode != GAME_MODE.SPECIAL_GOLD)//이 둘의경우 강종을 해도 보상받아야함.
            G_GameInfo._GameInfo.IsEndForced = true;//강제종료

        G_GameInfo._GameInfo.ForcedGameEnd();
    }

    void OnClickPauseBtnCancel()
    {
        if (!SceneManager.instance.IsRTNetwork)
            Time.timeScale = 1;

        if (PortalPos != null)
        {
            //IsPossiblePortal = false;
        }
    }

    public override bool Quit()
    {
        if (G_GameInfo._GameInfo == null || !G_GameInfo._GameInfo.GameLive || CutSceneMgr.StartCheck)
            return false;

        if (PortalPos == null)//포탈이 존재한다면 이거 실행하면 안됨
            OnClickPause();

        return false;//무조건 false 종료는 아니고 별도의 지정 메세지만 띄울거니까
    }

    /// <summary> 공격버튼 누르는 이벤트 </summary>
    void AttackBtnPressedEvent()
    {
        // Unit에게 공격하라고 알림.
        if (null != LeaderUnit)
        {
            LeaderUnit.ManualAttack();
        }
    }

    /// <summary>
    /// 스킬버튼클릭이벤트
    /// </summary>
    void SkillBtnClkEvent(GameObject skillBtn)
    {
        int Idx = -1;
        if (int.TryParse(skillBtn.name.Replace("SkillBtn", ""), out Idx))
        {
            //해당 부분에서 넘어온 Idx를 통해 스킬을 호출하는 코드 넣는다.

            //임시로 스킬에서 +1을 해주자
            UseSkill(Idx + 1);
        }
        else
            Debug.LogWarning("2JW : SkillBtn Click Function Error!! - " + skillBtn.name + " : " + Idx);
    }

    void UseSkill(int SkillSlotId)
    {
        if (LeaderUnit.UseSkill(SkillSlotId, true))
        {

        }
    }

    /// <summary>
    /// 파트너스킬클릭이벤트
    /// </summary>
    void SpecialBtnClkEvent(GameObject SpecialBtn)
    {
        int Idx = -1;
        if (int.TryParse(SpecialBtn.name.Replace("SpecialBtn", ""), out Idx))
        {
            //해당 부분에서 넘어온 Idx를 통해 파트너 스킬을 호출하는 코드 넣는다.
            UsePartnerSpecialSkill(Idx);
        }
        else
            Debug.LogWarning("2JW : SpecialBtn Click Function Error!! - " + SpecialBtn.name + " : " + Idx);

    }

    /// <summary> 파트너 필살기를 사용한다. </summary>
    /// <param name="partnerNum">파트너 번호 0, 1</param>
    void UsePartnerSpecialSkill(int partnerNum)
    {
        //파트너 스킬은 액티브 4개와(0~3번) 버프 4개로 이루어지며 0~2번은 자동 발동 액티브 스킬이며, 4번스킬은 필살기임.
        //따라서 3번 스킬만 사용하도록 하면 됨.
        for(int i=0; i < AllPartners.Count; i++)
        {
            if (AllPartners[i].TeamID != LeaderUnit.TeamID)
                continue;

            if (AllPartners[i].syncData._SlotNo-1 != partnerNum)
                continue;

            if (AllPartners[partnerNum].UseSkill(4, true))
            {

            }

            break;
        }
        /*
        if (partnerNum < AllPartners.Count && AllPartners[partnerNum] != null)
        {
            //if (partnerNum == 0)
            if (AllPartners[partnerNum].UseSkill(4, true))
            {

            }
            //else //다르다는걸 보여주기 위해 그냥 일단 임시로 바꿔놓자
            //Partners[partnerNum].UseSkill(2);
        }
        */
    }

    #endregion

    #region :: EventListner ::
    /*
    /// <summary>
    /// 사용 스킬 설명 세팅부분. 트리거이벤트를 통해 들어온다.
    /// UIPlayTween을 통해서 활성화 시킨다.
    /// </summary>
    void SetMessage(object message)
    {
        if (!(message is string))
            return;

        msgLabel.text = (string)message;
        msgInfo.GetComponent<UIPlayTween>().Play(true);
    }
    */
    /// <summary>
    /// 사용 스킬 아이콘 세팅부분. 트리거이벤트를 통해 들어온다.
    /// </summary>
    //void SetSkillIcon(object message)
    //{
    //if (!(message is string))
    //return;
    //    Debug.Log("이거 사용하는 건지?");
    //msgSkillIcon.spriteName = (string)message;
    //}

    //void SetRaidMessage(object message)
    //{
    //    if (!(message is string))
    //        return;

    //    RaidMsgInfoLabel.text = (string)message;
    //    RaidMsgInfo.GetComponent<UIPlayTween>().Play(true);
    //}

    //BuffListPanel BossBuffPanel;
    void ShowBossUI(object show, object unit)
    {
        if (!(show is bool))
            return;

        if (!(unit is Npc))
            return;

        Npc bossUnit = unit as Npc;

        //보스 UI를 보여주는 부분 들어가야함.
        BossGroup.SetActive(true);

        //Transform tf = playTimeLbl.transform;
        //tf.localPosition = new Vector3(-548, tf.localPosition.y);

        //< 이벤트 연출 실행(보스 체크는 중간보스도 호출되기때문)
        if (bossUnit.UnitType == UnitType.Boss)
        {
            if (GameObject.Find("CutSceneCameraMover") != null)
            {
                CutSceneMgr.StartBossCutScene();
            }
            else
            {
                CutSceneMgr.StartCutScene();
            }
        }
        
        //보스나왔으면 꺼준다
        if (G_GameInfo.GameMode != GAME_MODE.FREEFIGHT || FreeFightGameState.GameMode != GAME_MODE.FREEFIGHT)
        {
            DelayPathEffTime = -1f;
            PathArrowTf.gameObject.SetActive(false);
        }

    }
    
    #endregion

    //< 부활했을시 처리
    public void Revive()
    {
        StopWarning();
        SetJoyActive(true);//다시 활성화

        if (null != AllPartners)
        {
            for (int i = 0; i < AllPartners.Count; i++)
            {
                if (AllPartners[i].TeamID != LeaderUnit.TeamID)
                    continue;

                int slot = AllPartners[i].syncData._SlotNo-1;
                Icons[(uint)iconT.pskill_0 + slot].color = Color.white;
                Btns[(uint)btnT.pskill_0 + slot].collider.enabled = true;
				Btns[(uint)btnT.pskill_0 + slot].defaultColor = Color.white;

                PartnerGroups[slot].transform.FindChild("Partner01Icon_d1").GetComponent<UISprite>().color = Color.white;

            }
        }

        Sliders[(int)sliderT.subHero].fillAmount = 1;
        Show();
        //부활 했다면 퀘스트 다시 갱신해줌
        //SetStageQuestState(ClearQuestType.STAGE_CLEAR, 0);
    }

    /// <summary>
    /// 퀘스트 텍스트 넣어준다
    /// </summary>
    void SetStageQuestText()
    {
        if (G_GameInfo.GameMode != GAME_MODE.SINGLE)
        {
            //스테이지?퀘스트 꺼준다
            CurStageQuestGo.transform.parent.gameObject.SetActive(false);
            return;
        }

        StageQuestList = SingleGameState.StageQuestList;
        for (int i = 0; i < StageQuestList.Count; i++)
        {
            QuestLabelArray[i].text = StageQuestList[i].GetTypeString();
        }
    }

    /// <summary>
    /// 퀘스트 클리어, 실패에 관한것 입력한다.
    /// </summary>
    /// <param name="questType">어떤 퀘스트 인지</param>
    /// <param name="value"> 조건의 값 </param>
    public void SetStageQuestState(ClearQuestType questType, float value)
    {
        if (StageQuestList == null)
            return;

        for (int i = 0; i < StageQuestList.Count; i++)
        {
            if (StageQuestList[i].Type != questType)// 현재 진행중인 퀘스트 목록에 없는 타입이면 무시하겠지?
                continue;

            bool isClear = StageQuestList[i].CheckClearQuest(questType, value);

            //이미 같으면 무시.
            if (StageQuestList[i].IsClear == isClear)
                break;

            StageQuestList[i].IsClear = isClear;
            SetQuestStringColor(i, isClear);
            break;
        }

        //if(questType == StageQuestType.Hp)
        //StageQuestHpClear = !StageQuestHpClear;
    }

    public void SetQuestStringColor(int arr, bool isClear)
    {
        QuestLabelArray[arr].color = isClear ? MissionClearColor : Color.white;
        QuestStartArray[arr].spriteName = isClear ? "Img_Star02" : "Img_Star01";
    }
    
    /// <summary>
    /// 아이콘 셋팅하는 곳
    /// </summary>
    void SettingIcon()
    {
        NetData._UserInfo charInven = NetData.instance.GetUserInfo();

        SkillTables.ActionInfo actionLowData_0 = _LowDataMgr.instance.GetSkillActionLowData(playerCtlr.unitSyncData.NormalAttackData[0]._SkillID);
        Icons[(uint)iconT.attack].spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData_0.Icon);

        //스킬 아이콘 넣기
        for (int i = 0; i < 4; i++)
        {
            SkillTables.ActionInfo actionLowData = _LowDataMgr.instance.GetSkillActionLowData(playerCtlr.unitSyncData.SkillData[i]._SkillID);
            Icons[(uint)iconT.skill_0 + i].spriteName = _LowDataMgr.instance.GetLowDataIcon(actionLowData.Icon);
        }

        NetData._PartnerData partner0 = charInven.GetEquipPartner(1);
        NetData._PartnerData partner1 = charInven.GetEquipPartner(2);
        if (G_GameInfo.GameInfo.GameMode == GAME_MODE.FREEFIGHT || (partner0 == null && partner1 == null))
        {//난투장일 경우 파트너 스킬은 안나오게 한다              //착용 하고 있지 않이함
            Icons[(uint)iconT.partner_0].transform.parent.gameObject.SetActive(false);
            Icons[(uint)iconT.partner_1].transform.parent.gameObject.SetActive(false);
            Btns[(uint)btnT.pskill_0].gameObject.SetActive(false);
            Btns[(uint)btnT.pskill_1].gameObject.SetActive(false);
        }
        else if (partner0 != null && partner1 != null)//2마리일 경우
        {
            Icons[(uint)iconT.partner_0].spriteName = partner0.GetIcon();
            Icons[(uint)iconT.partner_1].spriteName = partner1.GetIcon();

            Icons[(uint)iconT.pskill_0].spriteName = partner0.GetStrongSkillIcon();
            Icons[(uint)iconT.pskill_1].spriteName = partner1.GetStrongSkillIcon();
        }
        else if (partner0 != null || partner1 != null)//한마리일 경우
        {
            Icons[(uint)iconT.partner_0].spriteName = (partner0 == null ? partner1 : partner0).GetIcon();
            Icons[(uint)iconT.pskill_0].spriteName = (partner0 == null ? partner1 : partner0).GetStrongSkillIcon();

            Btns[(uint)btnT.pskill_1].gameObject.SetActive(false);
            Icons[(uint)iconT.partner_1].transform.parent.gameObject.SetActive(false);
        }

    }

    //스킬 쿨타임 업데이트
    void SkillCoolTimeUpdate()
    {
        int loopCount = UseSkillList.Count;
        for (int i = 0; i < loopCount; i++)
        {
            UISkillCoolTimeData data = UseSkillList[i];
            float coolTime = data.skill.CoolTime;

            if (1f <= coolTime)
                data.coolText.text = coolTime.ToString("0");
            else
                data.coolText.text = coolTime.ToString("0.0");

            float coolPer = data.skill.IsSkillCoolTimePecent();
            data.coolPer.fillAmount = coolPer;

            if (coolTime <= 0f)
            {
                --loopCount;
                --i;

                UIHelper.CreateEffectInGame(data.coolEndEff, "Fx_IN_cooltime");

                //GameObject eff = data.coolEndEff;
                //eff.SetActive(true);
                //TempCoroutine.instance.FrameDelay(1.2f, () =>
                //{
                //    eff.SetActive(false);
                //});


                bool isEnable = true;
                int parSkillArr = -1;
                if (AllPartners != null)
                {
                    for (int j = 0; j < AllPartners.Count; j++)
                    {
                        if (data.skill.GetCaster() != AllPartners[j])
                            continue;

                        parSkillArr = j;

                        if (AllPartners[j] == null || !AllPartners[j].IsDie)//죽은 파트너라면 예외처리가 필요함
                            continue;

                        isEnable = false;//콜라이더 꺼진체로 둔다

                    }
                }

                if (parSkillArr == -1)
                    SkillAlarmTime[data.skill.slot - 1] = SkillAlarm;
                else
                {
                    if (!isEnable)
                    {
                        SkillAlarmTime[4 + parSkillArr] = -1f;
                        EffSkillAlarm[4 + parSkillArr].SetActive(false);
                    }
                    else
                        SkillAlarmTime[4 + parSkillArr] = SkillAlarm;
                }

                data.coolText.gameObject.SetActive(false);

                data.col.enabled = isEnable;
                UseSkillList.Remove(data);
            }
        }

        for (int i = 0; i < SkillAlarmTime.Length; i++)//스킬 알람 
        {
            if (SkillAlarmTime[i] == -1)
                continue;

            SkillAlarmTime[i] -= Time.deltaTime;

            if (SkillAlarmTime[i] <= 0)
            {
                SkillAlarmTime[i] = SkillAlarm;
                EffSkillAlarm[i].SetActive(false);
                EffSkillAlarm[i].SetActive(true);
            }
        }
    }

    //보스 이름 넣어준다
    public void SetBossInfo(string bossName, string icon)
    {
        // Icons[(uint)iconT.boss].spriteName = icon;
        BossName.text = bossName;
    }

    /// <summary> 플레이어, 파트너가 사용한 스킬의 정보를 받아서 쿨타임 표기한다. </summary>
    public void AddUseSkill(bool isPartner, Skill skill)
    {
        UISkillCoolTimeData data;
        data.skill = skill;
        if (isPartner)
        {
            int arr = 0;//해당 스킬을 사용한 객체를 찾는 것.
            int loopCount = AllPartners.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if (skill.GetCaster() != AllPartners[i])
                    continue;

                arr = i;
                break;
            }

            SkillAlarmTime[4 + arr] = -1;
            EffSkillAlarm[4 + arr].SetActive(false);

            data.coolText = PartnerSkillCoolTimer[arr];
            data.coolPer = PartnerSkillCoolImg[arr];
            data.coolEndEff = ParSkillCoolTimeEff[arr].transform;
        }
        else
        {
            SkillAlarmTime[skill.slot - 1] = -1;
            EffSkillAlarm[skill.slot - 1].SetActive(false);

            data.coolText = SkillCoolTimer[skill.slot - 1];
            data.coolPer = SkillCoolTimeImg[skill.slot - 1];
            data.coolEndEff = SkillCoolTimeEff[skill.slot - 1].transform;
        }

        data.col = data.coolText.transform.parent.collider;

        data.coolText.gameObject.SetActive(true);
        data.col.enabled = false;
        UseSkillList.Add(data);
    }

    /// <summary>
    /// 난투장 드랍아이템 정보화면표기
    /// </summary>
    /// <param name="dropReward"></param>
    public void SetFreeFightReward(FreeFightGameInfo.FreefightDropReward dropReward, bool firstEnter)
    {
        //일일획득가능한최대보상
        int maxHonor = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.FreeBattlePoint);
        int maxItemCnt = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.FreeBattleItem);

        Transform reward = FreeFightBattleGroup.transform.FindChild("Reward").transform;
        UILabel frontHonor = reward.FindChild("honorFront").GetComponent<UILabel>();
        UILabel frontItem = reward.FindChild("itmeFront").GetComponent<UILabel>();
        UILabel BackHonor = reward.FindChild("honorBack").GetComponent<UILabel>();
        UILabel BackItem = reward.FindChild("itemBack").GetComponent<UILabel>();

        BackHonor.text = string.Format("/{0}", maxHonor);
        BackItem.text = string.Format("/{0}", maxItemCnt);

        frontHonor.color = Color.white;
        frontItem.color = Color.white;

        string prevHonor = frontHonor.text;
        string prevItem = frontItem.text;

        frontHonor.text = string.Format("{0}", dropReward.honor > maxHonor ? maxHonor : dropReward.honor, maxHonor);  //dropReward.honor.ToString();
        frontItem.text = string.Format("{0}", dropReward.totalItemCnt > maxItemCnt ? maxItemCnt : dropReward.totalItemCnt, maxItemCnt); //dropReward.totalItemCnt.ToString();

        // 최대상태일시 컬러변경 (컬러는 임시로넣은것)
        BackHonor.color = dropReward.honor >= maxHonor ? Color.blue : Color.white;
        BackItem.color = dropReward.totalItemCnt >= maxItemCnt ? Color.blue : Color.white;


        if (dropReward.honor == maxHonor)
        {
            frontHonor.color = Color.blue;
        }
        else if (dropReward.honor > maxHonor)
        {
            frontHonor.color = Color.red;
        }
        ;
        if (dropReward.totalItemCnt == maxItemCnt)
        {
            frontItem.color = Color.blue;
        }
        else if (dropReward.totalItemCnt > maxItemCnt)
        {
            frontItem.color = Color.red;
        }


        if (!firstEnter)
        {

            // 이떄 트윈연출
            if (prevHonor != dropReward.honor.ToString())
            {
                frontHonor.GetComponent<TweenScale>().to = Vector3.one;
                frontHonor.GetComponent<TweenScale>().from = new Vector3(1.2f, 1.2f, 1.2f);
                frontHonor.GetComponent<TweenScale>().ResetToBeginning();
                frontHonor.GetComponent<TweenScale>().PlayForward();
            }

            if(prevItem != dropReward.item.ToString())
            {
                frontItem.GetComponent<TweenScale>().to = Vector3.one;
                frontItem.GetComponent<TweenScale>().from = new Vector3(1.2f, 1.2f, 1.2f);
                frontItem.GetComponent<TweenScale>().ResetToBeginning();
                frontItem.GetComponent<TweenScale>().PlayForward();
            }

        }
        //else
        //{
        //    //처음에 
           
        //}


        //TweenScale hTween = frontHonor.gameObject.GetComponent<TweenScale>();




        //honor.text = string.Format("{0}/{1}", dropReward.honor > maxHonor ? maxHonor : dropReward.honor, maxHonor);
        //item.text = string.Format("{0}/{1}", dropReward.totalItemCnt > maxItemCnt ? maxItemCnt : dropReward.totalItemCnt, maxItemCnt);

    }

    /// <summary>
    /// 난투장 PK 킬수표시
    /// </summary>
    /// <param name="killData"></param>
    public void SetPKText(FreeFightGameInfo.FreeFightUserKillData killData)
    {
        if (killData.KillCount == 0)
            return;

        StopCoroutine("KillCount");
        FreefightLabel.transform.FindChild("kill_1").gameObject.SetActive(false);
        FreefightLabel.transform.FindChild("kill_2").gameObject.SetActive(false);
        StartCoroutine("KillCount", killData);


    }
    IEnumerator KillCount(FreeFightGameInfo.FreeFightUserKillData killData)
    {
        float delay = 0f;

        // 처치시텍스트 (ETC테이블) 
        EtcID[] indexArr = { EtcID.FreeFightkilltext01, EtcID.FreeFightkilltext02, EtcID.FreeFightkilltext03, EtcID.FreeFightkilltext04, EtcID.FreeFightkilltext05, EtcID.FreeFightkilltext06 };

        GameObject kill;
        kill = FreefightLabel.transform.FindChild("kill_2").gameObject;
        EtcID tableIndex = 0;
        if (killData.KillCount >= 8)
        {
            tableIndex = indexArr[5]; // 119(잔인무도)
        }
        else if (killData.KillCount >= 7)
        {
            tableIndex = indexArr[4];   //118
        }
        else if (killData.KillCount >= 6)
        {
            tableIndex = indexArr[3];
        }
        else if (killData.KillCount >= 4)
        {
            tableIndex = indexArr[2];
        }
        else if (killData.KillCount >= 2)
        {
            tableIndex = indexArr[1];
        }
        else
        {
            tableIndex = indexArr[0];
            kill = FreefightLabel.transform.FindChild("kill_1").gameObject;
        }

        // 한명킬은 단순히 {0}처치!
        // 두명이상부터 {0}처치 무법자 !이렇게들어가므로 구분해줌

        UILabel killName = kill.transform.FindChild("Txt_name").GetComponent<UILabel>();    //처치
        UILabel killText = kill.transform.FindChild("Txt_kill").GetComponent<UILabel>();    //무법자,등등

        string killStr = "";
        if(killData.KillCount>1)
        {
            killStr = string.Format(_LowDataMgr.instance.GetStringCommon(_LowDataMgr.instance.GetEtcTableValue<uint>(tableIndex))/*, killData.UserName*/);
        }
        string killuserName = string.Format(_LowDataMgr.instance.GetStringCommon(1022), killData.UserName);

        killName.text = killuserName;
        killText.text = killStr;

        //페이드인 
        float alpha = 0;
        kill.SetActive(true);
        //float delay = 0;
        while (true)
        {
            alpha += (/*0.1f **/ Time.deltaTime);
            kill.GetComponent<UIPanel>().alpha = alpha;

            if (1 <= alpha)
                break;
        
            yield return null;
        }

        yield return new WaitForSeconds(2f); // 2초간유지 

        delay = 0f;
        //페이드아웃
        while (true)
        {
            alpha -= (/*0.1f*/  Time.deltaTime);
            kill.GetComponent<UIPanel>().alpha = alpha;

            if (0 >= alpha)
                break;

            yield return null;
        }

        kill.SetActive(false);
    }

    ///<summary> 난투장 보스 알림</summary>
    public void AlertBoss(FreeFightGameInfo.BossData bossData)
    {
        //맵에서 
        MapPanel map = null;
        map = UIMgr.Open("UIPanel/MapPanel").GetComponent<MapPanel>();

        if (map != null)
            map.BossDrop();

        FreeFightBattleGroup.transform.FindChild("BosswarringGroup").gameObject.SetActive(true);

        GameObject BossAlert = FreeFightBattleGroup.transform.FindChild("BosswarringGroup").FindChild("BossAlert").gameObject;
        //GameObject BossTime = FreeFightBattleGroup.transform.FindChild("BosswarringGroup").FindChild("Txt_Deathtime").gameObject;
        UISprite img = FreeFightBattleGroup.transform.FindChild("BosswarringGroup").FindChild("img").GetComponent<UISprite>();

        if (bossData.Daed == 0) //생존 이때는 등장알림 켯다꺼줌
        {
            BossAlertTime = System.DateTime.Now;
            // StopCoroutine("BossReviveTime");
            BossTime.SetActive(false);
            StartCoroutine("BossAlertPopup", BossAlert);
            img.spriteName = "Img_pvpBoss";
        }
        else //죽은상태 이때는 등장시간 카운트해줘야함
        {
            BossTime.SetActive(true);
            StopCoroutine("BossAlertPopup");
            BossAlert.SetActive(false);
            BossAlertTime = System.DateTime.Now.AddSeconds(bossData.ReviveTime);
            img.spriteName = "Img_pvpBossDis";
            //  object parms = new object[2] { (GameObject)BossTime, (float)bossData.ReviveTime };
            //  StartCoroutine("BossReviveTime", parms);

        }

    }

    IEnumerator BossAlertPopup(GameObject alertGo)
    {

        float delayTime = 0f;
        alertGo.SetActive(true);

        while (true)
        {
            delayTime += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);

            if (delayTime > 0.5f)
                break;
        }

        //yield return new WaitForSeconds(0.3f);
        alertGo.SetActive(false);
        yield return null;

    }
    //IEnumerator BossReviveTime(object[] parms)
    //{
    //    BossAlertTime = (float)parms[1];
    //    GameObject timeGo = (GameObject)parms[0];
    //    timeGo.SetActive(true);

    //    UILabel timeLabel = timeGo.GetComponent<UILabel>();

    //    while (true)
    //    {
    //        BossAlertTime -= Time.deltaTime;
    //        int min = (int)BossAlertTime / 60;
    //        int sec = (int)BossAlertTime % 60;
    //        timeLabel.text = string.Format("{0:00} : {1:00}", min, sec);
    //        yield return new WaitForFixedUpdate();
    //        //yield return new WaitForSeconds(Time.deltaTime);


    //        if (BossAlertTime < 0)
    //            break;
    //    }

    //    yield return new WaitForSeconds(0.2f);
    //    timeGo.gameObject.SetActive(false);


    //}



    /// <summary> 난투장의 안전지대를 나타내는 아이콘 키고 끄기</summary>
    void SetSafetyZone()
    {

        bool Safety = (G_GameInfo.GameInfo as FreeFightGameInfo).IsSafetyZone;

        if (Safety)//안전지대     
        {
            SafetyZoneIcon.value = true;
            transform.FindChild("BottomBtnGroup").gameObject.SetActive(false);
            transform.FindChild("TopBtnGroup/AutoBtn").gameObject.SetActive(false);

            string iconName = "auto_btn_off";
            Icons[(uint)iconT.autoBtnIcon].spriteName = iconName;
            Btns[(uint)btnT.AutoBtn].normalSprite = iconName;

            AutoBtnEff.SetActive(false);
            //AutoText.gameObject.SetActive(false);

            if (IsStopAutoMode || G_GameInfo.GameInfo.AutoMode)
                SetAutoMode(false);
        }
        else
        {
            SafetyZoneIcon.value = false;
            transform.FindChild("BottomBtnGroup").gameObject.SetActive(true);
            transform.FindChild("TopBtnGroup/AutoBtn").gameObject.SetActive(true);

            //혹시모를 스킬 비활성화이미지 다시 복구?
            for (int i = 0; i < Btns.Length; i++)
            {
                if (Btns[i] == null)
                    continue;

                TweenColor tColor = Btns[i].tweenTarget.GetComponent<TweenColor>();
                if (tColor != null)
                    tColor.ResetToBeginning();
                else
                    Btns[i].ResetDefaultColor();
            }
        }
    }

    IEnumerator AlertSafetyTxt()
    {
        UILabel safetyText = SafetyZoneIcon.transform.FindChild("Txt_Safetyzone").GetComponent<UILabel>();
        safetyText.text = SafetyZoneIcon.value == true ? _LowDataMgr.instance.GetStringCommon(899) : _LowDataMgr.instance.GetStringCommon(900);
        safetyText.applyGradient = SafetyZoneIcon.value == true ? false : true;


        float delay = 0f;

        while (true)
        {
            delay += Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
            safetyText.gameObject.SetActive(true);

            if (delay > 0.3)
                break;
        }

        safetyText.gameObject.SetActive(false);
        yield return null;
    }

    /// <summary> 카운트 다운 표기하는 것. GameInfo에서 실행함. </summary>
    public void SetCountDown(int count)
    {
        CountDown.spriteName = string.Format("{0}", count);
        TweenScale scale = CountDown.gameObject.GetComponent<TweenScale>();
        TweenAlpha alpha = CountDown.gameObject.GetComponent<TweenAlpha>();
        scale.ResetToBeginning();
        scale.PlayForward();

        alpha.ResetToBeginning();
        alpha.PlayForward();
    }

    /// <summary> 카운트 다운 사용할거면 이거 켜주고 시작. 다시 꺼줘야함 . </summary>
    public void SetCountDownActive(bool isActive)
    {
        CountDownPanel.SetActive(isActive);
    }

    /// <summary> 골드, 경험치던전에서 사용하는 카운드다운 이펙트. </summary>
    public void StartEffCountDown()
    {
        UIHelper.CreateEffectInGame(transform, "Fx_IN_countdown_01");
    }
    /*
    /// <summary> 시작 이라는 문구의 트윈 연출. </summary>
    public float StartTweenScale(bool visible)
    {
        Transform startBG = transform.FindChild("ArenaGroup/Start");
        if ( !visible)
        {
            startBG.gameObject.SetActive(false);
            return 0;
        }

        startBG.gameObject.SetActive(true);
        TweenScale tween = startBG.GetComponent<TweenScale>();
        tween.ResetToBeginning();
        tween.PlayForward();
        
        return tween.duration + tween.delay;
    }
    */
    /// <summary> 조이스틱 작동 중지 및 작동 시작  채팅팝업 켜지면 끔 </summary>
    public void SetJoyActive(bool isActive)
    {
        Joy.SetJoyActive(isActive);
        //ejoy.isActivated = isActive;
    }

    public override void Hide()
    {
        base.Hide();
        ChatPop.Hide();
        SetJoyActive(false);

        UIBasePanel optionPanel = UIMgr.GetUIBasePanel("UIPanel/OptionPanel");
        if (optionPanel != null)
            optionPanel.Close();

        if (_MapPanel != null)
            _MapPanel.Hide();
    }

    void OnShow()
    {
        if (G_GameInfo.GameMode != GAME_MODE.TUTORIAL)
        {
            ChatPop.OnShow();
            //PlayModelIdle();//튜토리얼에서는 별도 처리
        }

        for (int i = 0; i < Btns.Length; i++)
        {
            if (Btns[i] == null)
                continue;

            TweenColor tColor = Btns[i].tweenTarget.GetComponent<TweenColor>();
            if (tColor != null)
                tColor.ResetToBeginning();
            else
                Btns[i].ResetDefaultColor();
            //Btns[i].defaultColor = Color.white;
        }

        SetJoyActive(true);

        if (_MapPanel != null)
            _MapPanel.OnShow();

		SetCameraComponent (true);
    }

    public override void Close()
    {
        base.Close();
    }

    public Joystick GetJoystick()
    {
        return Joy;
    }

    /// <summary> 난투장 무료 부활시 다른 팝업들 키거나 끈다. </summary>
    public void SetActiveUI(bool isActive)
    {
        transform.FindChild("TopBtnGroup").gameObject.SetActive(isActive);
        transform.FindChild("PotraitGroup").gameObject.SetActive(isActive);
        transform.FindChild("BottomBtnGroup").gameObject.SetActive(isActive);
        transform.FindChild("TopBtnGroup").gameObject.SetActive(isActive);
        transform.FindChild("JoysticGroup").gameObject.SetActive(isActive);
        //transform.FindChild("JoystickComponent(Clone)").gameObject.SetActive(isActive);
    }

    public void SetActiveTutorial(string[] hideObjs, string[] enableObjs)
    {
        if (hideObjs != null)
        {
            for (int i = 0; i < hideObjs.Length; i++)
            {
                if(transform.FindChild(hideObjs[i]) == null)
                    Debug.LogError("not found child object " + hideObjs[i]);
                else
                    transform.FindChild(hideObjs[i]).gameObject.SetActive(false);
            }
        }

        if (enableObjs != null)
        {
            for (int i = 0; i < enableObjs.Length; i++)
            {
                if (transform.FindChild(enableObjs[i]) == null)
                    Debug.LogError("not found child object " + enableObjs[i]);
                else
                    transform.FindChild(enableObjs[i]).gameObject.SetActive(true);
            }
        }

    }
    /*
    public void SetSearchPath(bool isSearch)
    {
        IsSearchPath = isSearch;
        if (!isSearch)
            G_GameInfo.GameInfo.ActivePathEffect(false);
    }
    */
    public void ChangeAttackImg(byte state, uint skillIdx, float callChain)
    {
        if (CurAttImgState == state)
            return;

        string imgName = null;
        if (state == 1)//스킬사용가능
        {
            UIHelper.CreateEffectInGame(AtkBtn.transform, "Fx_IN_exchange_skill");

            SkillTables.ActionInfo actionLowData_0 = _LowDataMgr.instance.GetSkillActionLowData(skillIdx);
            if (actionLowData_0 != null)
                imgName = _LowDataMgr.instance.GetLowDataIcon(actionLowData_0.Icon);

            CallChainTime = callChain;
        }
        else//일반 용
        {
            if(skillIdx <= 0 )//시간 오버로 채인스킬 이펙트 연출
            {
                UIHelper.CreateEffectInGame(AtkBtn.transform, "Fx_IN_exchange_skill_02");
            }
            else
            {
                if (G_GameInfo.GameMode == GAME_MODE.TUTORIAL)
                {
                    (G_GameInfo.GameInfo as TutorialGameInfo).OnTutorial(InGameTutorialType.Skill_02);
                }
            }

            SkillTables.ActionInfo actionLowData_0 = _LowDataMgr.instance.GetSkillActionLowData(playerCtlr.unitSyncData.NormalAttackData[0]._SkillID);
            if (actionLowData_0 != null)
                imgName = _LowDataMgr.instance.GetLowDataIcon(actionLowData_0.Icon);

            Sliders[(int)sliderT.chainTime].fillAmount = 0;
        }

        CurAttImgState = state;
        AtkBtn.normalSprite = imgName;
        Icons[(uint)iconT.attack].spriteName = imgName;
    }

    /// <summary> 골드던전에서만 사용하지만 혹시 몰라 두는거 </summary>
    public void SpecialKillCount(int count)
    {
        GoldTf.FindChild("Item/value").GetComponent<UILabel>().text = string.Format("x{0}", count);
    }

    public static bool ZeroDamagePlay = false;
    public static bool ZeroDamagePlay_01 = false;
    public static bool ZeroDamagePlay_02 = false;
    public static bool ZeroSkillCoolTime = false;
    float[] tempvals = { 0, 0, 0, 0, 0, 0 };
    bool IsOpenGUI = false;

    void OnGUI()
    {
        if (!Debug.isDebugBuild)
            return;
        
        if (GUI.Button(new Rect(10, 100, 100, 50), string.Format("GUI치트키{0}", IsOpenGUI ? "닫기" : "열기")))
            IsOpenGUI = !IsOpenGUI;

        if (!IsOpenGUI)
            return;

#if UNITY_EDITOR
        if (SceneManager.instance.IsRTNetwork)
        {
            uint count = 0;
            var enumerator = G_GameInfo.CharacterMgr.allrUUIDDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GUI.Label(new Rect(10, 50 * count * 30, 300, 200), string.Format("type: {0}, uuid: {1}", enumerator.Current.Value.UnitType.ToString(), enumerator.Current.Value.m_rUUID));
                count++;
            }

            return;  //난투장에서는 우선 테스트 GUI를 제거했다.
        }
        if (GUI.Button(new Rect(200, 100, 100, 33), string.Format("무적모드\n({0})", ZeroDamagePlay)))
        {
            ZeroDamagePlay = !ZeroDamagePlay;
        }

        if (GUI.Button(new Rect(200, 133, 100, 33), string.Format("나만 무적모드\n({0})", ZeroDamagePlay_01)))
        {
            ZeroDamagePlay_01 = !ZeroDamagePlay_01;
        }

        if (GUI.Button(new Rect(200, 166, 100, 34), string.Format("파트너 무적모드\n({0})", ZeroDamagePlay_02)))
        {
            ZeroDamagePlay_02 = !ZeroDamagePlay_02;
        }
        
        if (GUI.Button(new Rect(300, 150, 100, 50), "마을가기"))
        {
            SceneManager.instance.ActionEvent(_ACTION.GO_TOWN);

            //패널닫기
            Close();
        }

        if (GUI.Button(new Rect(100, 100, 100, 100), string.Format("스피드 {0}", Time.timeScale == 1 ? 1 : 5)))
        {
            Time.timeScale = Time.timeScale == 1 ? 5 : 1;
        }

        if (GUI.Button(new Rect(400, 150, 50, 50), "Clear"))
        {
            //결과 화면이 보고 싶을때 사용.
            if(G_GameInfo.GameMode == GAME_MODE.SINGLE)
                (G_GameInfo._GameInfo as SingleGameInfo).OverrideGameEnd(true);

            G_GameInfo._GameInfo.PrepareEndGame(true);
            G_GameInfo._GameInfo.EndGame(true);
        }

        if (GUI.Button(new Rect(300, 100, 100, 50), string.Format("쿨타임 {0}", ZeroSkillCoolTime ? "없음" : "있음")))
        {
            ZeroSkillCoolTime = !ZeroSkillCoolTime;
        }

        if (GUI.Button(new Rect(400, 100, 50, 50), string.Format("죽음")))
        {
            if (leaderUnit == null || leaderUnit.IsDie)
                return;

            leaderUnit.SetHp(leaderUnit, -leaderUnit.CharInfo.MaxHp);
        }

		if (GUI.Button(new Rect(500, 100, 50, 50), string.Format("partner죽음")))
		{
			if (leaderUnit == null || leaderUnit.IsDie)
				return;

			AllPartners[0].SetHp(leaderUnit, -AllPartners[0].CharInfo.MaxHp);
		}
#else
		if (GUI.Button (new Rect (0, 0, 100, 50), string.Format ("치트팝업"))) {
			string path = "UIPopup/CheatPopup";
			UIBasePanel panel = UIMgr.GetUIBasePanel(path);
			if(panel == null)
			{	
				GameObject chatPop = UIMgr.Open(path, false);
			}
			else
			{
				panel.Close();
			}
		}
#endif
    }

	public void SetCameraComponent(bool b){

		if (transform.FindChild ("Camera") != null) {
			transform.FindChild ("Camera").GetComponent<Camera>().enabled = b;
		}
	}
}


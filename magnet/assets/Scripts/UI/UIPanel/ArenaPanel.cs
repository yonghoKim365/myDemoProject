using UnityEngine;
using System.Collections.Generic;

public class ArenaPanel : UIBasePanel
{
    //public UIEventTrigger[] SelectButton; //첫화면에서 선택버튼 (PVP, 사자왕)

    public Transform PlayCharRoot;// 플레이 중인 캐릭터 보여줄 곳
    public Transform[] PartnerModelRoot;// 파트너 슬롯, 파트너 삽입하면 보여줄 곳 배열은 0~1

    public GameObject PvpReadyPopup;    //pvp전투 준비 팝업창
    public GameObject BattleRecordPopup;    //전투기록
    public GameObject RankPopup;    //전투기록
    public GameObject RewardListPopup;  //보상목록

    public GameObject[] PnTouchObj;//파트너의 터치라벨 키고 끄려고 만듦
    public GameObject[] PnTouchEff;

    public GameObject RecordSlotPrefab; //전투기록슬롯 프리펩
    public GameObject RankingSlotPrefab;    //랭킹슬롯프리펩
    public GameObject RewardDailySlotPrefab;    //일일보상슬롯 프리펩
    //public GameObject RewardMonthlySlotPrefab;    //월간보상슬롯 프리펩
    public GameObject PvpSlotPrefab;

    public UIGrid RecordGrid;
    public UIGrid RankGrid;
    public UIGrid RewardDailyGrid;
    //public UIGrid RewardMonthlyGrid;
    public UIGrid PvpGrid;

    public UIButton RankBtn;
    public UIButton RecordBtn;
    public UIButton RewardBtn;


    public UIButton ShopBtn;
    public UIButton BuyBtn; //추가전투 횟수 구입버튼

    public UIButton ChangeCosutmePartnerBtn;    //파트너와 코스튬번경

    public UILabel[] TakeParNames;
    public UILabel charNameLv;
    public UILabel FightCountLabel; //전투가능횟수
    public UILabel TodayPoints; //오늘획득보상

    private NetData._UserInfo CharInven;
    private List<NetData.ArenaUserInfo> RankList = new List<NetData.ArenaUserInfo>();//랭킹 리스트
    private ObjectPaging RankPaging;
    private ObjectPaging EnemyPaging;
    private List<NetData.ArenaUserInfo> EnemyList = new List<NetData.ArenaUserInfo>();
    private NetData.ArenaUserInfo EnemyInfo;

    private bool IsSendFightList;
    private int DailyCount;
    private int MaxDailyCount;
    private int ResetCount;
    private int[] EnemyPartners;

    private int MyTopRank;  //역대최고순위
    private int ResetMaxCount;  //vip레벨에따른 리셋최대수치
    private string MyGuildName;

    private UIBasePanel reOpenPanel;

    public override void Init()
    {
        base.Init();
        CharInven = NetData.instance.GetUserInfo();

        UIHelper.CreateEffectInGame(PnTouchEff[0].transform, "Fx_UI_partner_select_01");
        UIHelper.CreateEffectInGame(PnTouchEff[1].transform, "Fx_UI_partner_select_01");

        SetPvpReadyPopup();
        PvpReadyPopup.SetActive(true);
        BattleRecordPopup.SetActive(false);
        RewardListPopup.SetActive(false);
        RankPopup.SetActive(false);

        //vip레벨에따른 리셋최대수치
        List<Vip.VipDataInfo> VipInfo = _LowDataMgr.instance.GetLowDataVipData(NetData.instance.GetUserInfo()._VipLevel);
        for (int i = 0; i < VipInfo.Count; i++)
        {
            if (VipInfo[i].type == 12)
            {
                ResetMaxCount = (int)VipInfo[i].Typevalue;
                break;
            }
        }

        for (int i = 0; i < 15; i++)
        {
            GameObject slotGo = Instantiate(PvpSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = PvpGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(PvpSlotPrefab);


        BuyBtn.gameObject.SetActive(false);
        BuyBtn.transform.FindChild("Txt_buy").GetComponent<UILabel>().text = _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPcashvalue).ToString();
        BtnEvents();

        transform.FindChild("PvpPanel/class").GetComponent<UISprite>().spriteName = UIHelper.GetClassIcon(CharInven._userCharIndex);

        RankPaging = ObjectPaging.CreatePagingPanel(RankGrid.transform.parent.gameObject, RankGrid.gameObject, RankingSlotPrefab, 1, 4, 4, 0, OnRankSlotdelegate);
        Destroy(RankingSlotPrefab);

        //EnemyPaging = ObjectPaging.CreatePagingPanel(PvpGrid.transform.parent.gameObject, PvpGrid.gameObject, PvpSlotPrefab, 1, 4, 4, 0, OnMatchListSlotdelegate);
        //Destroy(PvpSlotPrefab);

        //UI바뀌어야함 일단 보류
        List<PVP.PVPAutoRewardInfo> autoList = _LowDataMgr.instance.GetPvpAutoRewardList();
        UIHelper.CreateSlotItem(true, autoList.Count, RewardDailySlotPrefab.transform, RewardDailyGrid.transform, delegate (Transform tf, int arr)
        {
            if (autoList.Count - 1 == arr)//마지막
                tf.FindChild("num").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(193);//순위권 밖
            else
            {
                if (autoList[arr].RankMin == autoList[arr].RankMax)
                    tf.FindChild("num").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(192),/*"{0}"*/ autoList[arr].RankMin);
                else
                {
                    string num = string.Format("{0} ~ {1}", autoList[arr].RankMin, autoList[arr].RankMax);
                    tf.FindChild("num").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(192), num);

                }
            }

            tf.FindChild("value").GetComponent<UILabel>().text = string.Format("{0:#,#}", autoList[arr].RewardValue);
        });

        RewardDailyGrid.repositionNow = true;
        PvpSlotPrefab.SetActive(false);

        NetworkClient.instance.SendPMsgArenaRankInfoC();
        NetworkClient.instance.SendPMsgArenaInfoC();
    }

    public override void LateInit()
    {
        base.LateInit();

        reOpenPanel = (UIBasePanel)parameters[0];
    }

    public override void NetworkData(params object[] proto)
    {
        base.NetworkData(proto);

        switch ((Sw.MSG_DEFINE)proto[0])
        {
            case Sw.MSG_DEFINE._MSG_ARENA_INFO_S://금일 횟수
                DailyCount = (int)proto[1];
                MaxDailyCount = (int)proto[2];
                ResetCount = (int)proto[3];
                MyTopRank = (int)proto[4];

                ArenaGameState.MyTopRank = MyTopRank;

                if (ResetCount >= 1)
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), ResetCount - DailyCount, MaxDailyCount);    //이떄 한개만
                    BuyBtn.gameObject.SetActive(ResetCount - DailyCount == 0);

                }
                else
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), MaxDailyCount - DailyCount, MaxDailyCount);
                    BuyBtn.gameObject.SetActive(MaxDailyCount - DailyCount == 0);

                }



                //FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), MaxDailyCount - DailyCount, MaxDailyCount);

                // BuyBtn.gameObject.SetActive(MaxDailyCount - DailyCount == 0);
                //if (CharInven.ArenaRanking <= 0)//최초 진입일 것이다.
                //{
                //NetworkClient.instance.SendPMsgArenaRankInfoC();
                //}
                //else
                //{
                //    transform.FindChild("PvpPanel/info/rank_label").GetComponent<UILabel>().text =
                //        string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(521), CharInven.ArenaRanking);

                //}

                break;

            case Sw.MSG_DEFINE._MSG_ARENA_RANK_INFO_S://나의 랭킹 정보
                {
                    transform.FindChild("PvpPanel/info/rank_label").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(521), CharInven.ArenaRanking);

                    TodayPoints.text = "0";

                    //오늘획득가능한보상...
                    List<PVP.PVPAutoRewardInfo> autoList = _LowDataMgr.instance.GetPvpAutoRewardList();
                    for (int i = 0; i < autoList.Count; i++)
                    {
                        if (autoList[i].RankMax < CharInven.ArenaRanking || CharInven.ArenaRanking < autoList[i].RankMin)
                            continue;

                        TodayPoints.text = autoList[i].RewardValue.ToString();
                        break;
                    }


                    NetworkClient.instance.SendPMsgArenaMatchListC(0);  //서버에서 알아서보내준다함.

                }
                break;

            case Sw.MSG_DEFINE._MSG_ARENA_MATCH_LIST_S://대상 리스트 받음

                // 상위 유저 8명 , 나 , 하위유저 2명 => 11명 보여줌
                EnemyList = (List<NetData.ArenaUserInfo>)proto[1];

                //내껏도 추가해줭
                NetData.ArenaUserInfo myInfo = new NetData.ArenaUserInfo((long)CharInven._charUUID, CharInven._charName, (int)CharInven._userCharIndex, (int)CharInven._Level, (int)CharInven._VipLevel, (int)CharInven._TotalAttack, CharInven.ArenaRanking, CharInven._GuildId);
                EnemyList.Insert(EnemyList.Count, myInfo);

                //정렬타임
                EnemyList.Sort(delegate (NetData.ArenaUserInfo a, NetData.ArenaUserInfo b)
                {
                    if (a.Rank > b.Rank)
                        return 1;
                    else if (b.Rank > a.Rank)
                        return -1;
                    return 0;
                });

                OnMatchListSlotdelegate();
               
                break;

            case Sw.MSG_DEFINE._MSG_ARENA_MATCH_INFO_S://게임 시작 로딩으로 넘기는 곳
                ArenaGameState.TargetRank = EnemyInfo.Rank;
                ArenaGameState.MyRank = CharInven.ArenaRanking;
                ArenaGameState.TargetAttack = EnemyInfo.Attack;

                SceneManager.instance.SetSubLoadingPanelParams((int)NetData.instance.GetUserInfo()._TotalAttack, EnemyInfo.Attack, MyGuildName, EnemyInfo.GuildName);
                SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.ARENA, () =>
                {
                    SceneManager.instance.ActionEvent(_ACTION.PLAY_ARENA);
                });

                Hide();
                break;

            case Sw.MSG_DEFINE._MSG_ARENA_FIGHT_LIST_S://유아이에 있을 경우 한번만 실행하도록 한다 이후에는 팝업만 껏다 켜줌
                IsSendFightList = true;//막아줄 변수
                List<NetData.ArenaFightInfo> list = (List<NetData.ArenaFightInfo>)proto[1];
                UIHelper.CreateSlotItem(true, list.Count, RecordSlotPrefab.transform, RecordGrid.transform, delegate (Transform tf, int i)
                {
                    if (list.Count <= i)
                    {
                        tf.gameObject.SetActive(false);
                        return;
                    }

                    tf.gameObject.SetActive(true);
                    tf.FindChild("battle").GetComponent<UILabel>().text = SceneManager.instance.NumberToString(list[i].BattlePoint); //list[i].BattlePoint.ToString(); // ToString("#,##");
                    tf.FindChild("name").GetComponent<UILabel>().text = list[i].UserName;
                    //tf.FindChild("rank").GetComponent<UILabel>().text = string.Format("{0}", list[i].Ranking);
                    //tf.FindChild("result").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon((uint)(list[i].FightResult == (int)Sw.FIGHT_RESULT.FIGHT_RESULT_WIN ? 69 : 70));

                    //승리.패배시 슬롯색이달라짐
                    bool isWin = list[i].FightResult == (int)Sw.FIGHT_RESULT.FIGHT_RESULT_WIN;
                    tf.FindChild("winBg").gameObject.SetActive(isWin);
                    tf.FindChild("loseBg").gameObject.SetActive(!isWin);

                    tf.FindChild("winResult").gameObject.SetActive(isWin);
                    tf.FindChild("loseResult").gameObject.SetActive(!isWin);

                    //랭킹변화도 표시해줘야함 
                    //녹색 :3ddb80
                    //빨강: ed353a
                    string rank = "";
                    if (isWin && list[i].Ranking != list[i].WinRanking)
                    {
                        rank = string.Format("{0}[ed353a](-{1})[-]", list[i].WinRanking, list[i].Ranking - list[i].WinRanking);
                    }
                    else
                        rank = string.Format("{0}", list[i].Ranking);

                    //if (CharInven.ArenaRanking == list[i].Ranking)
                    //    rank = string.Format("{0}", list[i].Ranking);
                    //else if (CharInven.ArenaRanking > list[i].Ranking)
                    //    rank = string.Format("{0}[3ddb80](+{1})[-]", list[i].Ranking, CharInven.ArenaRanking - list[i].Ranking);
                    //else
                    //    rank = string.Format("{0}[ed353a](-{1})[-]", list[i].Ranking, list[i].Ranking - CharInven.ArenaRanking);

                    tf.FindChild("rank").GetComponent<UILabel>().text = rank;
                });

                RecordGrid.repositionNow = true;
                BattleRecordPopup.SetActive(true);
                break;

            case Sw.MSG_DEFINE._MSG_ARENA_RANK_LIST_S://랭킹 리스트
                List<NetData.ArenaUserInfo> addList = (List<NetData.ArenaUserInfo>)proto[1];
                List<ulong> guildIdList = new List<ulong>();
                for (int i = 0; i < addList.Count; i++)
                {
                    RankList.Add(addList[i]);
                    guildIdList.Add(addList[i].GuildId);
                }

                NetworkClient.instance.SendPMsgGuildNameQueryC(guildIdList);
                break;

            case Sw.MSG_DEFINE._MSG_GUILD_NAME_QUERY_S:
                Sw.PMsgGuildNameQueryS protocol = (Sw.PMsgGuildNameQueryS)proto[1];
                for (int i = 0; i < protocol.UnCount; i++)
                {
                    ulong guildId = protocol.CGuildInfo[i].UllGuildId;
                    string guild = string.IsNullOrEmpty(protocol.CGuildInfo[i].SzGuildName) ? "" : string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(8), protocol.CGuildInfo[i].SzGuildName);
                    for (int j = 0; j < RankList.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(RankList[j].GuildName) || RankList[j].GuildId != guildId)
                            continue;

                        if (CharInven.ArenaRanking == RankList[j].Rank)//유저의 길드 이름이다.
                            MyGuildName = guild;

                        RankList[j].SetGuildName(guild);
                        break;
                    }
                }

                if (protocol.UnCount != 50 || RankList.Count == 100)
                {
                    RankPaging.RefreshSlot(RankList.Count);
                    if (!RankPopup.activeSelf)
                    {
                        RankPopup.SetActive(true);

                        //유저의 정보
                        transform.FindChild("RankPopup/HeroRank/name").GetComponent<UILabel>().text = CharInven._charName;
                        transform.FindChild("RankPopup/HeroRank/guild_name").GetComponent<UILabel>().text = MyGuildName;
                        transform.FindChild("RankPopup/HeroRank/rank").GetComponent<UILabel>().text = CharInven.ArenaRanking.ToString();//string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(521), CharInven.ArenaRanking);
                        transform.FindChild("RankPopup/HeroRank/class").GetComponent<UISprite>().spriteName = UIHelper.GetClassIcon(CharInven._userCharIndex);
                        transform.FindChild("RankPopup/HeroRank/face").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetCharcterData(CharInven._userCharIndex).PortraitId;
                        transform.FindChild("RankPopup/HeroRank/Bestrank/rank_label").GetComponent<UILabel>().text = MyTopRank.ToString();
                        transform.FindChild("RankPopup/HeroRank/guild_name").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), CharInven._TotalAttack);
                    }
                }
                break;

            case Sw.MSG_DEFINE._MSG_ARENA_RESET_TIMES_S://차관 금일 횟수 초기화
                DailyCount = (int)proto[1];
                MaxDailyCount = (int)proto[2];
                ResetCount = (int)proto[3];

                if (ResetCount >= 1)
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), ResetCount - DailyCount, MaxDailyCount);    //이떄 한개만
                    BuyBtn.gameObject.SetActive(ResetCount - DailyCount == 0);

                }
                else
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), MaxDailyCount - DailyCount, MaxDailyCount);
                    BuyBtn.gameObject.SetActive(MaxDailyCount - DailyCount == 0);

                }


                //NetworkClient.instance.SendPMsgArenaFightStartC(EnemyPartners, EnemyInfo.RoleId, EnemyInfo.Rank);//바로 시작.
                break;
        }

    }

    void OnMatchListSlotdelegate()
    {

        for (int i = 0; i < PvpGrid.transform.childCount; i++)
        {
            if (i >= EnemyList.Count)
            {
                PvpGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = PvpGrid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;
            slotGo.SetActive(true);

            slotTf.FindChild("win_reward").gameObject.SetActive(MyTopRank > EnemyList[i].Rank);  //상대승리시 최고래밸을 달성할수있을시 ..
            if (slotTf.FindChild("win_reward").gameObject.activeSelf)
            {
                slotTf.FindChild("win_reward/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1259), (MyTopRank - EnemyList[i].Rank) * _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPfirstgetcash));
            }
            slotTf.FindChild("lv").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), EnemyList[i].Level);
            slotTf.FindChild("battle").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), EnemyList[i].Attack);
            slotTf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(155), EnemyList[i].Name);
            slotTf.FindChild("rank").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(1020), EnemyList[i].Rank);
            slotTf.FindChild("class").GetComponent<UISprite>().spriteName = UIHelper.GetClassIcon((uint)EnemyList[i].Type);

            if ((ulong)EnemyList[i].RoleId == CharInven.GetCharUUID())
            {
                slotTf.FindChild("win_reward").gameObject.SetActive(false);
                slotTf.FindChild("BtnStart").gameObject.SetActive(false);
            }
            else
                slotTf.FindChild("BtnStart").gameObject.SetActive(true);


            int idx = i;
            EventDelegate.Set(slotTf.FindChild("BtnStart").GetComponent<UIButton>().onClick, delegate ()
            {

                NetData._PartnerData par_1 = CharInven.GetEquipPartner(1);
                NetData._PartnerData par_2 = CharInven.GetEquipPartner(2);
                EnemyPartners = new int[] {
                            (int)(par_1 != null ? par_1._partnerIndex : 0),
                            (int)(par_2 != null ? par_2._partnerIndex : 0)
                        };

                EnemyInfo = EnemyList[idx];

                if (ResetCount >= 1)
                {
                    if (ResetCount - DailyCount == 0)
                    {
                        OnClickBuyBtn();
                        return;
                    }
                }
                else
                {
                    if (MaxDailyCount <= DailyCount)
                    {
                        OnClickBuyBtn();
                        return;
                    }
                }

                NetworkClient.instance.SendPMsgArenaFightStartC(EnemyPartners, EnemyInfo.RoleId, EnemyInfo.Rank);
            });





        }

        PvpGrid.repositionNow = true;


        //if (EnemyList.Count <= arr)
        //{
        //    go.SetActive(false);
        //    return;
        //}
        //go.SetActive(true);

        //Transform tf = go.transform;

        //tf.FindChild("win_reward").gameObject.SetActive(MyTopRank > EnemyList[arr].Rank);  //상대승리시 최고래밸을 달성할수있을시 ..
        //if (tf.FindChild("win_reward").gameObject.activeSelf)
        //{
        //    tf.FindChild("win_reward/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1259), (MyTopRank - EnemyList[arr].Rank) * _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPfirstgetcash));
        //}
        //tf.FindChild("lv").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(14), EnemyList[arr].Level);
        //tf.FindChild("battle").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), EnemyList[arr].Attack);
        //tf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(155), EnemyList[arr].Name);
        //tf.FindChild("rank").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(1020), EnemyList[arr].Rank);
        //tf.FindChild("class").GetComponent<UISprite>().spriteName = UIHelper.GetClassIcon((uint)EnemyList[arr].Type);

        //if ((ulong)EnemyList[arr].RoleId == CharInven.GetCharUUID())
        //{
        //    tf.FindChild("win_reward").gameObject.SetActive(false);
        //    tf.FindChild("BtnStart").gameObject.SetActive(false);
        //}
        //else
        //    tf.FindChild("BtnStart").gameObject.SetActive(true);


        //EventDelegate.Set(tf.FindChild("BtnStart").GetComponent<UIButton>().onClick, delegate () {

        //    NetData._PartnerData par_1 = CharInven.GetEquipPartner(1);
        //    NetData._PartnerData par_2 = CharInven.GetEquipPartner(2);
        //    EnemyPartners = new int[] {
        //                    (int)(par_1 != null ? par_1._partnerIndex : 0),
        //                    (int)(par_2 != null ? par_2._partnerIndex : 0)
        //                };

        //    EnemyInfo = EnemyList[arr];

        //    if (ResetCount >= 1)
        //    {
        //        if(ResetCount - DailyCount == 0)
        //        {
        //            OnClickBuyBtn();
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (MaxDailyCount <= DailyCount)
        //        {
        //            OnClickBuyBtn();
        //            return;
        //        }
        //    }

        //    NetworkClient.instance.SendPMsgArenaFightStartC(EnemyPartners, EnemyInfo.RoleId, EnemyInfo.Rank);
        //});

        /*
         
        
                if (ResetCount >= 1)
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), ResetCount - DailyCount, MaxDailyCount);    //이떄 한개만
                    BuyBtn.gameObject.SetActive(ResetCount - DailyCount == 0);

                }
                else
                {
                    FightCountLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(1255), MaxDailyCount - DailyCount, MaxDailyCount);
                    BuyBtn.ga
         */


    }

    void OnRankSlotdelegate(int arr, GameObject go)
    {
        if (RankList.Count <= arr)
        {
            go.SetActive(false);
            return;
        }

        go.SetActive(true);

        Character.CharacterInfo charInfo = _LowDataMgr.instance.GetCharcterData((uint)RankList[arr].Type);
        Transform tf = go.transform;
        //   tf.FindChild("guild_name").GetComponent<UILabel>().text = RankList[arr].GuildName;
        tf.FindChild("name").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(155), RankList[arr].Name);
        // tf.FindChild("rank").GetComponent<UILabel>().text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(1020), RankList[arr].Rank);
        //tf.FindChild("class").GetComponent<UISprite>().spriteName = UIHelper.GetClassIcon((uint)RankList[arr].Type);
        tf.FindChild("face").GetComponent<UISprite>().spriteName = charInfo.PortraitId;
        tf.FindChild("battle").GetComponent<UILabel>().text = string.Format("{0} {1:#,#}", _LowDataMgr.instance.GetStringCommon(47), RankList[arr].Attack);


        //랭킹 1,2,3등은 명칭이다름 
        GameObject one = tf.FindChild("1").gameObject;
        GameObject two = tf.FindChild("2").gameObject;
        GameObject three = tf.FindChild("3").gameObject;
        GameObject other = tf.FindChild("0").gameObject;

        one.SetActive(false);
        two.SetActive(false);
        three.SetActive(false);
        other.SetActive(false);

        if (RankList[arr].Rank == 1)
            one.SetActive(true);
        else if (RankList[arr].Rank == 2)
            two.SetActive(true);
        else if (RankList[arr].Rank == 3)
            three.SetActive(true);
        else
        {
            other.transform.FindChild("Txt_ranking").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(522), RankList[arr].Rank);
            other.SetActive(true);
        }


        EventDelegate.Set(tf.FindChild("info").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            //정보보기
            ulong uu = (ulong)RankList[arr].RoleId;
            UIMgr.OpenUserInfoPopup(/*(long)RankList[arr].RoleId*/(long)uu, RankList[arr].Name, RankList[arr].Type, RankList[arr].VipLevel, RankList[arr].Level, true);
        });

    }

    public override void OnCloseReadyPopup()
    {
        base.OnCloseReadyPopup();
        Show(GetParams());

        PvpReadyPopup.SetActive(true);
        SetPvpReadyPopup();
    }

    void BtnEvents()
    {
        Transform parBtnTf = transform.FindChild("PvpPanel/Character/CharView");
        EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_0").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            if (PartnerModelRoot[0].gameObject.activeSelf)
                return;

            base.Hide();
            UIMgr.OpenReadyPopup(GAME_MODE.ARENA, this, 0, 0);
        });
        EventDelegate.Set(parBtnTf.FindChild("BtnPartnerSlot_1").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            if (PartnerModelRoot[1].gameObject.activeSelf)
                return;

            base.Hide();
            UIMgr.OpenReadyPopup(GAME_MODE.ARENA, this, 0, 0);
        });

        EventDelegate.Set(RankPopup.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, delegate ()
        {
            RankPopup.SetActive(false);
        });

        EventDelegate.Set(BattleRecordPopup.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, delegate ()
        {
            BattleRecordPopup.SetActive(false);
        });

        EventDelegate.Set(RewardListPopup.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, delegate ()
        {
            RewardListPopup.SetActive(false);
        });

        EventDelegate.Set(RankBtn.onClick, delegate ()
        {//랭킹
            RankList.Clear();
            NetworkClient.instance.SendPMsgArenaRankListC();



        });

        EventDelegate.Set(RecordBtn.onClick, delegate ()
        {//전투 기록

            if (!IsSendFightList)
                NetworkClient.instance.SendPMsgArenaFightListC();
            else
                BattleRecordPopup.SetActive(true);
        });

        EventDelegate.Set(RewardBtn.onClick, delegate ()
        {//보상
            RewardListPopup.SetActive(true);
        });

        EventDelegate.Set(ChangeCosutmePartnerBtn.onClick, delegate ()
        {//준비창으로 변경
            base.Hide();
            //PvpReadyPopup.SetActive(false);
            UIMgr.OpenReadyPopup(GAME_MODE.ARENA, this, 0, 0);
        });


        EventDelegate.Set(ShopBtn.onClick, delegate ()
        {
            base.Hide();
            UIMgr.OpenShopPanel(this);
        });

        EventDelegate.Set(BuyBtn.onClick, delegate ()
        {
            OnClickBuyBtn();
        });
    }

    void OnClickBuyBtn()
    {
        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(974), _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPcashvalue));
        string count = string.Format(string.Format("\n\n{0} ({1}/{2})", _LowDataMgr.instance.GetStringCommon(1298), ResetCount, ResetMaxCount));
        string title = _LowDataMgr.instance.GetStringCommon(141);
        string ok = _LowDataMgr.instance.GetStringCommon(117);
        string cancel = _LowDataMgr.instance.GetStringCommon(76);
        uiMgr.AddPopup(title, msg + count, ok, cancel, null, delegate ()
        {
            if (ResetCount >= ResetMaxCount)
            {
                //횟수소진 알림
                UIMgr.instance.AddPopup(141, 1266, 117);
                return;
            }
            NetworkClient.instance.SendPMsgArenaResetTimesC();
        });
    }

    void SetPvpReadyPopup()
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

        UIHelper.CreatePcUIModel("ArenaPanel", PlayCharRoot, CharInven.GetCharIdx(), headId, equipCostumeData._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 3, CharInven.isHideCostum, false);

        NetData._PartnerData partner_0 = CharInven.GetEquipPartner(1);
        NetData._PartnerData partner_1 = CharInven.GetEquipPartner(2);

        if (partner_0 != null)
        {
            Transform modelRoot = PartnerModelRoot[0];
            PnTouchObj[0].SetActive(false);//터치 라벨 끈다.
            PnTouchEff[0].SetActive(false);
            PartnerModelRoot[0].gameObject.SetActive(true);

            UIHelper.CreatePartnerUIModel(modelRoot, partner_0._partnerDataIndex, 3, true, false, "ArenaPanel");
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

            UIHelper.CreatePartnerUIModel(modelRoot, partner_1._partnerDataIndex, 3, true, false, "ArenaPanel");
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

    public override void Hide()
    {
        base.Hide();

        if (reOpenPanel != null)
            reOpenPanel.Show(reOpenPanel.GetParams());
        else
            UIMgr.OpenDungeonPanel();
    }

    public override void GotoInGame()
    {
        //일단 임시로 게임은 되게
        //SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.ARENA, () =>
        //{
        //    //이상태에서의 데이터를 저장
        //    NetData.instance.MakePlayerSyncData(true);

        //    SceneManager.instance.ActionEvent(_ACTION.PLAY_ARENA);
        //    base.GotoInGame();
        //});

        uiMgr.CloseReadyPopup();
        OnCloseReadyPopup();
        //base.Close();
    }
}

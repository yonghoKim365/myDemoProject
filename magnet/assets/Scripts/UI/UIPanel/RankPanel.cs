using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RankPanel : UIBasePanel
{
    //랭크테이블 타입
    enum eRANK_TABLE_TYPE
    {
        BATTLE_POINT          = 1,// 캐릭터 전투력
        LEVEL                 = 2,// 캐릭터 레벨
        HP                    = 3,// 캐릭터 HP
        DAMAGE                = 4,// 데미지
        TOTAL_LEVEL           = 5,// 레벨 총합 (해당 서버 내 계정 캐릭터 레벨 총합)
        SKILL_ENCAHNT         = 6,// 캐릭터 스킬 강화 누적 수
        PARTNER_SKILL_ENCHANT = 7,// 파트너 스킬 강화 누적 수
        PARTNER_EVOLVE        = 8,// 파트너 승급 누적 수
        //PVP_POINT             = 21,// 차관 누적 점수
        PVP_WIN               = 22,// 차관 누적 승리 수
        PVP_ENTER             = 23,// 차관 누적 참여 수
        FREEFIGHT_POINT       = 41,// 난투장 누적 명예 점수
        FREEFIGHT_KILL        = 42,// 난투장 누적 킬 수
        GUILD_MEMBER          = 61,// 길드 구성원 총합 전투력
        GUILD_GOLD            = 62,// 길드 보유 현재 골드
        ACHIEV_COMPLETE       = 81,// 업적 달성 수
        STAGE_STAR            = 82,// 모험 모드 별 획득 누적 수
        TOWER_CLEAR           = 83,// 마계의 탑 달성 층수
        VIP_POINT             = 84,// VIP 누적 포인트
    }

    public enum eTAB_VIEW_TYPE
    {
        //NONE = 0,
        CHAR = 0,//캐릭터
        PVP = 1,    //1:1차관
        FREEFIGHT=2,// 난투장
        GUILD = 3,  //길드
        ETC = 4, // 기타
    }
    public enum eCHILD_TAB_TYPE { one, tow, three }


    [Header("----Rank Info Link----")]
    public eTAB_VIEW_TYPE CurViewType;// = eTAB_VIEW_TYPE.NONE;//현재 보고있는 뷰 타입
    private eCHILD_TAB_TYPE CurChildTabType = eCHILD_TAB_TYPE.one;
    
    public Transform MySlot;//내 정보 셋팅
    public Transform MinorTabTf; //소분류 탭

    public UIScrollView Scroll;
    public UIGrid Grid;
    public UITabGroup TabGroup;//대분류 탭 리스트 제어해줄 스크립트

    public Color32 oneTop;
    public Color32 oneBtm;
    public Color32 towTop;
    public Color32 towBtm;
    public Color32 threeTop;
    public Color32 threeBtm;
    public Color32 brownEff;
    public Color32 blueEff;

    public GameObject ListEmpty;//빈화면

    [Header("----Guild Info Link----")]
    public GameObject GuildPopGo;

    public UISprite GuildMark;

    public UILabel[] GuildInfo;//0 이름, 1 길드장이름, 2 인원
    public UILabel[] GuildState;//0 길드 생성일, 1 길드 전투력, 2 길드 자금
    public UILabel[] GuildDesc;//0 선언, 1공지


    private Dictionary<eRANK_TABLE_TYPE, MyRankData> MyRankDic = new Dictionary<eRANK_TABLE_TYPE, MyRankData>();
    private Dictionary<eRANK_TABLE_TYPE, List<NetData.RankInfo>> RankingList = new Dictionary<eRANK_TABLE_TYPE, List<NetData.RankInfo>>();
    private NetData._UserInfo CharInven;//한 캐릭터가 들고있는 인벤토리 정보
    private ObjectPaging _ObjectPaging;
    private NetData.GuildSimpleInfo MyGuildInfo;

    private bool IsResetTab;
    private int OpenDay;//데이터 초기화 하기 위해 페널을 열었을때 하루가 지났는지 확인하고자 하는 변수
    
    private string CurChildTapName;
    private eRANK_TABLE_TYPE CurRankType;// = eRANK_TABLE_TYPE.NONE;
    
    private struct MyRankData
    {
        public int MyRank;//내 랭크
        public long MyValue;//내 랭크 탭 데이터 값

        public MyRankData(int rank, long value)
        {
            MyRank = rank;
            MyValue = value;
        }
    }

    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("Ranking panel, Init() start");

        base.Init();

        CharInven = NetData.instance.GetUserInfo();

        _ObjectPaging = ObjectPaging.CreatePagingPanel(Scroll.gameObject, Grid.gameObject, Grid.transform.GetChild(0).gameObject, 1,
             5, 0, 0, (i, go) => {
                 List<NetData.RankInfo> list = null;
                 if(RankingList.TryGetValue(CurRankType, out list) )
                 if (list != null && i < list.Count)
                 {
                     go.SetActive(true);
                     SetSlot(go.transform, list[i] );
                 }
                 else
                     go.SetActive(false);

             });
        
        Destroy(Grid.transform.GetChild(0).gameObject );

        TabGroup.Initialize(OnClickTab);
        //Scroll.enabled = true;
        for (int i = 0; i < MinorTabTf.childCount; i++)
        {
            int type = i;
            EventDelegate.Set(MinorTabTf.GetChild(i).GetComponent<UIEventTrigger>().onClick, delegate () { CallBackMinor(type); });
        }

        EventDelegate.Set(GuildPopGo.transform.FindChild("BtnClose").GetComponent<UIEventTrigger>().onClick, Hide);//길드 정보 팝업 닫기
        SceneManager.instance.showStopWatchTimer ("Ranking panel, Init() finish");
    }


    public override void LateInit()
    {
        base.LateInit();

        if(0 < CharInven._GuildId )//길드랄 새로 가입한걸지도 모르니깐 
            NetworkClient.instance.SendPMsgGuildQueryBaseInfoC(CharInven._GuildId);
        else//길드를 탈퇴당한걸지도 모르니깐
            MyGuildInfo = new NetData.GuildSimpleInfo(0, 0, null, null, 0, 0, 0, 0, 0, 0 );

        if (OpenDay != System.DateTime.Now.Day)
        {
            OpenDay = System.DateTime.Now.Day;
            RankingList.Clear();
            MyRankDic.Clear();
        }

        TabGroup.CoercionTab(0);
        GuildPopGo.SetActive(false);

        SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("Ranking panel, LateInit() finish");
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}
    
    /// <summary> 탭 콜백 </summary>
    void OnClickTab(int viewType)
    {
        //if (viewType == 0)
        //    return;

        CurViewType = (eTAB_VIEW_TYPE)viewType;
        
        for (int i = 0; i < MinorTabTf.childCount; i++)
        {
            MinorTabTf.GetChild(i).gameObject.SetActive(true);
        }
        
        int[] labels = null;
        switch (CurViewType)
        {
            case eTAB_VIEW_TYPE.CHAR:
                labels = new int[] { 47, 14, 18, 19, 1185, 1186, 1187, 1188 };
                break;
            case eTAB_VIEW_TYPE.PVP:
                labels = new int[] { 1190, 1191 };//1189, 
                break;
            case eTAB_VIEW_TYPE.FREEFIGHT:
                labels = new int[] { 1192, 1193 };
                break;
            case eTAB_VIEW_TYPE.GUILD:
                labels = new int[] { 1194};//, 1195 길드 보유자금 탭 일단 보류
                break;
            case eTAB_VIEW_TYPE.ETC:
                labels = new int[] { 1196, 1197, 1198, 1199 };
                break;

            default:
                labels = new int[] { 0 };
                Debug.LogError("unDefined type error " + CurViewType);
                break;
        }

        for (int i = 0; i < MinorTabTf.childCount; i++)
        {
            if (i < labels.Length)
            {
                MinorTabTf.GetChild(i).gameObject.SetActive(true);

                UILabel label = MinorTabTf.GetChild(i).FindChild("tab_on/label").GetComponent<UILabel>();
                UILabel _label = MinorTabTf.GetChild(i).FindChild("tab_off/label").GetComponent<UILabel>();
                label.text = _LowDataMgr.instance.GetStringCommon((uint)labels[i]);
                _label.text = _LowDataMgr.instance.GetStringCommon((uint)labels[i]);
            }
            else
                MinorTabTf.GetChild(i).gameObject.SetActive(false);
        }


        CallBackMinor(0);
    }


    /// <summary> 소분류탭 콜백함수 </summary>
    void CallBackMinor(int idx)
    {
        for (int i = 0; i < MinorTabTf.childCount; i++)
        {
            GameObject on = MinorTabTf.GetChild(i).FindChild("tab_on").gameObject;
            GameObject off = MinorTabTf.GetChild(i).FindChild("tab_off").gameObject;

            on.SetActive(idx == i);
            off.SetActive(idx != i);

            MinorTabTf.GetChild(i).collider.enabled = i != idx;
        }

        CurChildTabType = (eCHILD_TAB_TYPE)idx;

        int type = 0;
        switch (CurViewType)
        {
            case eTAB_VIEW_TYPE.CHAR:
                type = idx+(int)eRANK_TABLE_TYPE.BATTLE_POINT;
                break;
            case eTAB_VIEW_TYPE.PVP:
                type = idx+(int)eRANK_TABLE_TYPE.PVP_WIN;
                break;
            case eTAB_VIEW_TYPE.FREEFIGHT:
                type = idx+(int)eRANK_TABLE_TYPE.FREEFIGHT_POINT;
                break;
            case eTAB_VIEW_TYPE.GUILD:
                type = idx+(int)eRANK_TABLE_TYPE.GUILD_MEMBER;
                break;
            case eTAB_VIEW_TYPE.ETC:
                type = idx+(int)eRANK_TABLE_TYPE.ACHIEV_COMPLETE;
                break;
            default:
                break;
        }

        CurRankType = (eRANK_TABLE_TYPE)type;//현재 랭크타입
        CurChildTapName = MinorTabTf.GetChild(idx).FindChild("tab_on/label").GetComponent<UILabel>().text;
        
        List<NetData.RankInfo> list = null;
        if (RankingList.TryGetValue(CurRankType, out list) && 0 < list.Count)
        {
            ////나의 info
            MySetSlot(new NetData.RankInfo(MyRankDic[CurRankType].MyRank, 0, (int)CharInven._userCharIndex, null, (int)CharInven._Level, MyRankDic[CurRankType].MyValue, (int)CharInven._VipLevel));

            _ObjectPaging.SetPanelActive(true);
            ListEmpty.SetActive(false);
            _ObjectPaging.RefreshObjectPaging(list.Count);
        }
        else
        {
            IsResetTab = true;
            if (CurViewType == eTAB_VIEW_TYPE.GUILD)
            {
                int guildType = 0;
                if ((eRANK_TABLE_TYPE)type == eRANK_TABLE_TYPE.GUILD_MEMBER)
                    guildType = (int)Sw.RANK_TYPE.RANK_TYPE_GUILD_ATTACK_TOTAL;
                else if ((eRANK_TABLE_TYPE)type == eRANK_TABLE_TYPE.GUILD_GOLD)
                    guildType = (int)Sw.RANK_TYPE.RANK_TYPE_GUILD_COIN;

                NetworkClient.instance.SendPMsgRankGuildQueryC(guildType);
            }
            else
                NetworkClient.instance.SendPMsgRankQueryC(type);
        }   
    }

    /// <summary> 캐릭터랭킹, 길드랭킹 정보 응답. </summary>
    public void GetRankList(int type, int myrank, long myValue, List<NetData.RankInfo> rankList)
    {
        if (type != (int)CurRankType)//현재탭과다른 정보가 왔다면 리턴
            return;

        if ( !MyRankDic.ContainsKey((eRANK_TABLE_TYPE)type))
            MyRankDic.Add((eRANK_TABLE_TYPE)type, new MyRankData(myrank, myValue) );

        List<NetData.RankInfo> list = null;
        if (RankingList.TryGetValue((eRANK_TABLE_TYPE)type, out list) )
            list.InsertRange(list.Count, rankList);
        else
        {
            list = rankList;
            RankingList.Add((eRANK_TABLE_TYPE)type, list);
        }

        ////나의 info
        MySetSlot(new NetData.RankInfo(MyRankDic[CurRankType].MyRank, 0, (int)CharInven._userCharIndex, null, (int)CharInven._Level, MyRankDic[CurRankType].MyValue, (int)CharInven._VipLevel));

        //다꺼줘용
        _ObjectPaging.SetPanelActive(list.Count > 0 ? true : false);
        if (list.Count <= 0)
        {
            ListEmpty.SetActive(true);
            SceneManager.instance.SetNoticePanel(NoticeType.Message, 901);
            //_ObjectPaging.SetMaxCount(0);
            return;
        }

        if(type != (int)eRANK_TABLE_TYPE.GUILD_GOLD && type != (int)eRANK_TABLE_TYPE.GUILD_MEMBER)//길드쪽 정보가 아니라면 길드이름 받아온다
        {
            List<ulong> idList = new List<ulong>();
            for (int i = 0; i < list.Count; i++)
            {
                idList.Add(list[i].Id);
            }

            if(0 < idList.Count)
                NetworkClient.instance.SendPMsgGuildIDQueryC(idList);
            else
            {
                if (IsResetTab)
                {
                    IsResetTab = false;
                    ListEmpty.SetActive(false);
                    _ObjectPaging.RefreshObjectPaging(list.Count);
                }
                else
                    _ObjectPaging.SetMaxCount(list.Count);
            }
        }
        else//길드 탭 리스트 받아온것.
        {
            if (IsResetTab)
            {
                IsResetTab = false;
                ListEmpty.SetActive(false);
                _ObjectPaging.RefreshObjectPaging(list.Count);
            }
            else
                _ObjectPaging.SetMaxCount(list.Count);
        }
    }

    /// <summary> 길드 네임 받아옴 </summary>
    public void OnRankerGuildInfoList(Sw.PMsgGuildIDQueryS guildQuery)
    {
        List<NetData.RankInfo> list = RankingList[CurRankType];
        for (int i = 0; i < guildQuery.UnCount; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                if (!list[j].Id.Equals(guildQuery.CGuildInfo[i].UllRoleId))
                    continue;

                list[j].GuildName = string.IsNullOrEmpty(guildQuery.CGuildInfo[i].SzGuildName) ?
                    _LowDataMgr.instance.GetStringCommon(331) : string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(229), guildQuery.CGuildInfo[i].SzGuildName);
                break;
            }
        }

        if (IsResetTab)
        {
            IsResetTab = false;
            ListEmpty.SetActive(false);
            _ObjectPaging.RefreshObjectPaging(list.Count);
        }
        else
            _ObjectPaging.SetMaxCount(list.Count);
    }

    public void OnMyGuildInfo(NetData.GuildSimpleInfo simpleInfo)
    {
        if(simpleInfo.Id.Equals(CharInven._GuildId) )//이렇게 확인한다.
            MyGuildInfo = simpleInfo;
        else//내꺼와 다르다면 상대 길드 정보를 요청한것.
        {
            GuildPopGo.SetActive(true);

            GuildInfo[0].text = simpleInfo.Name;
            GuildInfo[1].text = simpleInfo.LeaderName;
            GuildInfo[2].text = string.Format("{0}/{1}", simpleInfo.Count.ToString(), _LowDataMgr.instance.GetLowdataGuildInfo(simpleInfo.guildLv).GuildJoinLimit); // ToString("#,##");

            GuildMark.spriteName = _LowDataMgr.instance.GetLowDataIcon(simpleInfo.Icon);

            GuildState[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(577), simpleInfo.CreateTime.ToString().Substring(0, 4), simpleInfo.CreateTime.ToString().Substring(4, 2), simpleInfo.CreateTime.ToString().Substring(6, 2));
            GuildState[1].text = simpleInfo.Attack.ToString();
        }
    }


    void SetSlot(Transform slotTf, NetData.RankInfo info)
    {
        //등수
        UISprite rankMark = slotTf.FindChild("Rankmark").GetComponent<UISprite>();
        UILabel rank = slotTf.FindChild("Txt_ranking").GetComponent<UILabel>();
        rank.text = string.Format(_LowDataMgr.instance.GetStringCommon(522), info.Rank);
        slotTf.FindChild("Txt_Name").GetComponent<UILabel>().text = info.Name;//이름

        //얼굴
        UISprite face = slotTf.FindChild("Char").GetComponent<UISprite>();
        if (CurRankType == eRANK_TABLE_TYPE.GUILD_GOLD || CurRankType == eRANK_TABLE_TYPE.GUILD_MEMBER)// 길드는 휘장
        {
            face.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.GuildMark);
            face.spriteName = _LowDataMgr.instance.GetLowDataIcon((uint)info.RoleType);

            slotTf.FindChild("Txt_Guildname").gameObject.SetActive(false);
            Vector3 namePos = slotTf.FindChild("Txt_Name").localPosition;
            namePos.y = 15;//29.2
            slotTf.FindChild("Txt_Name").localPosition = namePos;

            Vector3 lvPos = slotTf.FindChild("Txt_Level").localPosition;
            lvPos.y = -30;//-45.8
            slotTf.FindChild("Txt_Level").localPosition = lvPos;

            slotTf.FindChild("Txt_VIP").GetComponent<UILabel>().text = "";

            if (0 < info.Id)
            {
                EventDelegate.Set(slotTf.GetComponent<UIEventTrigger>().onClick, () => {
                    NetworkClient.instance.SendPMsgGuildQueryBaseInfoC((uint)info.Id);
                    NetworkClient.instance.SendPMsgGuildQueryDetailedInfoC((uint)info.Id);
                });
            }
        }
        else
        {
            face.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Face);
            face.spriteName = UIHelper.GetClassPortIcon((uint)info.RoleType, 1);

            slotTf.FindChild("Txt_Guildname").gameObject.SetActive(true);
            Vector3 namePos = slotTf.FindChild("Txt_Name").localPosition;
            namePos.y = 29.2f;
            slotTf.FindChild("Txt_Name").localPosition = namePos;

            Vector3 lvPos = slotTf.FindChild("Txt_Level").localPosition;
            lvPos.y = -45.8f;
            slotTf.FindChild("Txt_Level").localPosition = lvPos;

            slotTf.FindChild("Txt_VIP").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), info.VipLv);
            slotTf.FindChild("Txt_Guildname").GetComponent<UILabel>().text = info.GuildName;

            if (0 < info.Id)
            {
                EventDelegate.Set(slotTf.GetComponent<UIEventTrigger>().onClick, () => {

                    UIMgr.OpenUserInfoPopup((long)info.Id, info.Name, info.RoleType, 0, info.Level, true);
                });
            }
        }

        if (info.Rank == 1)
        {
            rank.gradientTop = oneTop;
            rank.gradientBottom = oneBtm;
            rank.effectColor = brownEff;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank01";
        }
        else if (info.Rank == 2)
        {
            rank.gradientTop = towTop;
            rank.gradientBottom = towBtm;
            rank.effectColor = blueEff;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank02";
        }
        else if (info.Rank == 3)
        {
            rank.gradientTop = threeTop;
            rank.effectColor = brownEff;
            rank.gradientBottom = threeBtm;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank03";
        }
        else
        {
            rankMark.gameObject.SetActive(false);

            rank.gradientTop = Color.white;
            rank.gradientBottom = Color.white;
            rank.color = Color.white;
            rank.effectColor = blueEff;
        }
        
        slotTf.FindChild("Txt_Info").GetComponent<UILabel>().text = CurChildTapName;//info
        slotTf.FindChild("Txt_Num").GetComponent<UILabel>().text = info.Data.ToString();//info value
        slotTf.FindChild("Txt_Level").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(14), info.Level);
    }

    void MySetSlot(NetData.RankInfo info)
    {
        //얼굴
        bool isGuild = CurRankType == eRANK_TABLE_TYPE.GUILD_GOLD || CurRankType == eRANK_TABLE_TYPE.GUILD_MEMBER;
        UISprite face = MySlot.FindChild("Char").GetComponent<UISprite>();
        if ( !isGuild || MyGuildInfo.Id <= 0)
        {
            face.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Face);
            face.spriteName = UIHelper.GetClassPortIcon((uint)info.RoleType, 1);
        }
        else// 길드는 휘장
        {
            face.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.GuildMark);
            face.spriteName = _LowDataMgr.instance.GetLowDataIcon((uint)MyGuildInfo.Icon);
        }

        //등수
        UISprite rankMark = MySlot.FindChild("Rankmark").GetComponent<UISprite>();
        UILabel rank = MySlot.FindChild("Txt_ranking").GetComponent<UILabel>();
        if (info.Rank > 10000 || info.Rank == 0)
            rank.text = _LowDataMgr.instance.GetStringCommon(193);    //순위권밖
        else
            rank.text = (isGuild && MyGuildInfo.Id <= 0) ? _LowDataMgr.instance.GetStringCommon(133) : string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(521), info.Rank);

        if (info.Rank == 1)
        {
            rank.gradientTop = oneTop;
            rank.gradientBottom = oneBtm;
            rank.effectColor = brownEff;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank01";
        }
        else if (info.Rank == 2)
        {
            rank.gradientTop = towTop;
            rank.gradientBottom = towBtm;
            rank.effectColor = blueEff;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank02";
        }
        else if (info.Rank == 3)
        {
            rank.gradientTop = threeTop;
            rank.effectColor = brownEff;
            rank.gradientBottom = threeBtm;

            rankMark.gameObject.SetActive(true);
            rankMark.spriteName = "Img_Rank03";
        }
        else
        {
            rankMark.gameObject.SetActive(false);

            rank.gradientTop = Color.white;
            rank.gradientBottom = Color.white;
            rank.color = Color.white;
            rank.effectColor = blueEff;
        }

        MySlot.FindChild("Txt_Guildname").GetComponent<UILabel>().text = MyGuildInfo.Id <= 0 ? _LowDataMgr.instance.GetStringCommon(331) : MyGuildInfo.Name;
        MySlot.FindChild("Txt_VIP").GetComponent<UILabel>().text = isGuild ? "" : string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), info.VipLv);

        MySlot.FindChild("Txt_Info").GetComponent<UILabel>().text = CurChildTapName;
        MySlot.FindChild("Txt_Num").GetComponent<UILabel>().text = info.Data.ToString();
        MySlot.FindChild("Txt_Level").GetComponent<UILabel>().text = isGuild ? string.Format("{0}", MyGuildInfo.Id <= 0 ? "" : string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(14), MyGuildInfo.guildLv) ) 
            : string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(14), info.Level);
    }

    /// <summary> 랭킹 길드에서 길드의 상세정보를 요청함 </summary>
    public void OnGuildDetailInfoPopup(uint guildLv, uint blessLv, uint shopLv, string anoun, string notice, ulong money)
    {
        GuildState[2].text = money == 0 ? "0" : money.ToString();//자금

        GuildDesc[0].text = notice;
        GuildDesc[1].text = anoun;
    }

    public override void Hide()
    {
		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        if (GuildPopGo.activeSelf)
        {
            GuildPopGo.SetActive(false);
            return;
        }

        base.Hide();

        UIMgr.OpenTown();
    }
}

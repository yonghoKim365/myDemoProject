using UnityEngine;
using System.Collections.Generic;

public class GuildJoinPanel : UIBasePanel {

    public GameObject[] TabObj;//0 랭킹, 1길드
    public GameObject[] Popup;//0창설, 상세보기(창설용도 있음), 클릭팝업

    public Transform RankTf;
    public Transform ListTf;
    public Transform MarkTf;

    public UIInput CreateName;
    public UIInput CreateDesc;
    public UIInput CreateNoti;
    public UIInput SearchGuild;

    public UIScrollView RankScroll;

    public UILabel NotFoundGuildName;

    public UIToggle ToggleFreeJoin;

    private List<NetData.GuildSimpleInfo> RecommendList = new List<NetData.GuildSimpleInfo>();
    private List<uint> JoinList = new List<uint>();
    private List<NetData.RankInfo> RankingList = new List<NetData.RankInfo>();

    private uint IconID;
    private int slotCnt;
    private int totalCnt;   //여러번호출되니 .. 누적값을 더해줘서 생성되야할 슬롯이 몇개인지 알려줌

    private const int MarkStartId = 40000;
    public bool IsCreateGuild;


    public override void Init()
    {
        base.Init();
        
        EventDelegate.Set(ToggleFreeJoin.onChange, OnToggleFreeJoin);

        Transform prefab = ListTf.GetChild(0);
        prefab.gameObject.SetActive(false);
        for (int i=0; i < 9; i++)
        {
            Transform tf = Instantiate(prefab) as Transform;
            tf.parent = ListTf;
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
        }

        ListTf.GetComponent<UIGrid>().repositionNow = true;

        Transform Rprefab = RankTf.GetChild(0);
        Rprefab.gameObject.SetActive(false);
        for (int i = 0; i <100; i++)
        {
            Transform tf = Instantiate(Rprefab) as Transform;
            tf.parent = RankTf;
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

        }

        RankTf.GetComponent<UIGrid>().repositionNow = true;

        GameObject markPrefab = MarkTf.GetChild(0).gameObject;
        ObjectPaging.CreatePagingPanel(MarkTf.parent.gameObject, MarkTf.gameObject, markPrefab
          , 5, 32, 32, 10, OnCallBackMark);//가라 522개

        Destroy(markPrefab);


        //무료
        string val = string.Format(_LowDataMgr.instance.GetStringCommon(232), SceneManager.instance.NumberToString( _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCost2)) );
		Popup [0].transform.FindChild ("Make/BtnCreateGold/Btn_on/label").GetComponent<UILabel> ().text = val;
		Popup [0].transform.FindChild ("Make/BtnCreateGold/Btn_off/label").GetComponent<UILabel> ().text = val;

        bool create = NetData.instance.GetAsset(AssetType.Gold) < (ulong)_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCost2) ? false: true;
        Popup[0].transform.FindChild("Make/BtnCreateGold/Btn_on").gameObject.SetActive(create);
        Popup[0].transform.FindChild("Make/BtnCreateGold/Btn_off").gameObject.SetActive(!create);


        EventDelegate.Set(Popup[0].transform.FindChild("Make/BtnCreateGold").GetComponent<UIEventTrigger>().onClick, delegate() {//길드 생성 버튼 클릭.

            ulong needValue = _LowDataMgr.instance.GetEtcTableValue<ulong>(EtcID.GuildCost2);
            if (NetData.instance.GetAsset(AssetType.Gold) < needValue)
            {
                SetClickPopup(443, null, null);
            }
            else if (string.IsNullOrEmpty(CreateName.value) )//길드 이름 확인
            {
                SetClickPopup(237, null, null);
            }
            else if (string.IsNullOrEmpty(CreateDesc.value))//선언 확인
            {
                SetClickPopup(238, null, null);
            }
            else if (IconID <= 0)
            {
                SetClickPopup(239, null, null);

            }
            else //모든란 입력했음.
            {
                //금칙어는 XXX로 나옴
                //string name = CreateName.value;
                //_LowDataMgr.instance.IsBanString(ref name);

                if (_LowDataMgr.instance.IsBanString(CreateName.value))
                {
                    uiMgr.AddPopup(141, 898, 117);
                    return;
                }

                string dec = CreateDesc.value;
                _LowDataMgr.instance.IsBanString(ref dec);
                string noti = CreateNoti.value;
                _LowDataMgr.instance.IsBanString(ref noti);

                //if (_LowDataMgr.instance.IsBanString(CreateName.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}
                //if (_LowDataMgr.instance.IsBanString(CreateDesc.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}
                //if (_LowDataMgr.instance.IsBanString(CreateNoti.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}

				SetClickPopup(236, SceneManager.instance.NumberToString(needValue), () => { //SetClickPopup(236, needValue.ToString("#,##"), () => {
                    NetworkClient.instance.SendPMsgGuildCreateNewC(IconID, CreateName.value, dec, 2, 2);
                    //IsCreateGuild = true;
                });
            }

        } );
        //유료
		//string val2 = string.Format(_LowDataMgr.instance.GetStringCommon(231), _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCost1).ToString("#,##"));
		string val2 = string.Format(_LowDataMgr.instance.GetStringCommon(231), SceneManager.instance.NumberToString(_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCost1)));
		Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_on/label").GetComponent<UILabel>().text = val2;
		Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_off/label").GetComponent<UILabel>().text = val2;


        bool create_ = NetData.instance.GetAsset(AssetType.Gold) < (ulong)_LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCost1) ? false : true;
        Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_on").gameObject.SetActive(create_);
        Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_off").gameObject.SetActive(!create_);



        EventDelegate.Set(Popup[0].transform.FindChild("Make/BtnCreateCash").GetComponent<UIEventTrigger>().onClick, delegate () {//길드 생성 버튼 클릭.

            ulong needValue = _LowDataMgr.instance.GetEtcTableValue<ulong>(EtcID.GuildCost1);
            if ( NetData.instance.GetAsset(AssetType.Cash) < needValue)
            {
                SetClickPopup(376, null, null);
            }
            else if (System.Text.ASCIIEncoding.Unicode.GetByteCount(CreateName.value) > 24)  //길드이름 글자수제한
            {

            }
            else if (string.IsNullOrEmpty(CreateName.value))//길드 이름 확인
            {

                SetClickPopup(237, null, null);
            }
            else if (string.IsNullOrEmpty(CreateDesc.value))//선언 확인
            {

                SetClickPopup(238, null, null);
            }
            else if (IconID <= 0)
            {
                SetClickPopup(239, null, null);

            }

            else //모든란 입력했음.
            {
                //금칙어
                //string name = CreateName.value;
                //_LowDataMgr.instance.IsBanString(ref name);

                if (_LowDataMgr.instance.IsBanString(CreateName.value))
                {
                    uiMgr.AddPopup(141, 898, 117);
                    return;
                }
                string dec = CreateDesc.value;
                _LowDataMgr.instance.IsBanString(ref dec);
                string noti = CreateNoti.value;
                _LowDataMgr.instance.IsBanString(ref noti);
                //if (_LowDataMgr.instance.IsBanString(CreateName.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}
                //if (_LowDataMgr.instance.IsBanString(CreateDesc.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}
                //if (_LowDataMgr.instance.IsBanString(CreateNoti.value))
                //{
                //    uiMgr.AddPopup(141, 898, 117);
                //    return;
                //}

				SetClickPopup(235, SceneManager.instance.NumberToString(needValue), ()=> {  //SetClickPopup(235, needValue.ToString("#,##"), ()=> { 
                    NetworkClient.instance.SendPMsgGuildCreateNewC(IconID, CreateName.value, dec, 2, 1);
                    //IsCreateGuild = true;
                });
            }

        });

        //다음버튼 ( 휘장선택후 다시 생성화면으로 넘어감)
        EventDelegate.Set(Popup[0].transform.FindChild("Select/BtnCreate").GetComponent<UIEventTrigger>().onClick, delegate() {

            if (!CheckLevel)
                SetClickPopup(945, _LowDataMgr.instance.GetEtcTableValue<string>(EtcID.GuildCondition), null);
            else
            {
                //CreateGuild();
                if (0 < IconID)
                {
                    CreateGuild();
                }
                else//길드 마크를 선택하지 않음.
                {
                    SetClickPopup(239, null, null);
                }
            }
        } );

        //휘장마크 선택팝업 끄기
        EventDelegate.Set(Popup[0].transform.FindChild("BtnClose").GetComponent<UIEventTrigger>().onClick, () => {
            ActivePopups(99);
            IconID = 0;
        });

        //메인팝업 
        EventDelegate.Set(Popup[1].transform.FindChild("BtnClose").GetComponent<UIEventTrigger>().onClick, () => {
            ActivePopups(99);
            uiMgr.SetTopMenuTitleName(215);
        });

        //메세지 팝업
        EventDelegate.Set(Popup[2].transform.FindChild("BtnCancel").GetComponent<UIEventTrigger>().onClick, () => {
            Popup[2].SetActive(false);
        });

        //길드 생성 버튼
        transform.FindChild("BtnCreate/Btn_on/bg").GetComponent<UISprite>().spriteName = CheckLevel ? "Btn_Blue02" : "Btn_Blue02Dis";
        EventDelegate.Set(transform.FindChild("BtnCreate").GetComponent<UIEventTrigger>().onClick, ()=> {

            if(!CheckLevel)
            {
                SetClickPopup(945, _LowDataMgr.instance.GetEtcTableValue<string>(EtcID.GuildCondition), null);
                return;
            }

            //던에따라..ㅇㄹㅇㄹ
            bool createCash = NetData.instance.GetAsset(AssetType.Cash) >= _LowDataMgr.instance.GetEtcTableValue<ulong>(EtcID.GuildCost1);
            bool createMoney = NetData.instance.GetAsset(AssetType.Gold) >= _LowDataMgr.instance.GetEtcTableValue<ulong>(EtcID.GuildCost2);

            Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_on").gameObject.SetActive(createCash);
            Popup[0].transform.FindChild("Make/BtnCreateCash/Btn_off").gameObject.SetActive(!createCash);

            Popup[0].transform.FindChild("Make/BtnCreateGold/Btn_on").gameObject.SetActive(createMoney);
            Popup[0].transform.FindChild("Make/BtnCreateGold/Btn_off").gameObject.SetActive(!createMoney);

            ActivePopups(0);
        
            //여기서 생성화면으로 ..
            Popup[0].transform.FindChild("Select").gameObject.SetActive(false);
            Popup[0].transform.FindChild("Make").gameObject.SetActive(true);

            CreateName.value = CreateName.label.text = "";
            CreateDesc.value = CreateDesc.label.text = "";
            CreateNoti.value = CreateNoti.label.text = "";

            Popup[0].transform.FindChild("Make/Txt_master").GetComponent<UILabel>().text = NetData.instance.GetUserInfo()._charName;//내 캐릭이 길드장이 됨.
           

            IconID = 0;
            Popup[0].transform.FindChild("Make/Guildmark/icon").GetComponent<UISprite>().spriteName = "";
            UIEventTrigger etri = Popup[0].transform.FindChild("Make/Guildmark/icon").GetComponent<UIEventTrigger>();
            EventDelegate.Set(etri.onClick, delegate ()
            {
                for (int i = 0; i < MarkTf.childCount; i++)
                {
                    GameObject sel = MarkTf.GetChild(i).FindChild("Cover").gameObject;
                    sel.SetActive(false);
                }

                //휘장선택창
                Popup[0].transform.FindChild("Select").gameObject.SetActive(true);
                Popup[0].transform.FindChild("Make").gameObject.SetActive(false);
            });
        });

        //검색
        EventDelegate.Set(transform.FindChild("Search/Btn_search").GetComponent<UIEventTrigger>().onClick, () => {
            Debug.Log("SearchGuild Name = " +  SearchGuild.value);
            RecommendList.Clear();
            NetworkClient.instance.SendPMsgGuildSearchGuildC(SearchGuild.value);
        });

        //다시 재검색
        EventDelegate.Set(transform.FindChild("Search/Btn_reset").GetComponent<UIEventTrigger>().onClick, () => {

            for (int j = 0; j < ListTf.childCount; j++)
            {
                ListTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(false);
            }

            if (TabObj[1].activeSelf)//추천길드 리스트
                NetworkClient.instance.SendPMsgGuildRecommendListC();
        });

        SearchGuild.label.text = SearchGuild.value = "";
        NotFoundGuildName.text = "";
        CreateName.value = CreateName.label.text = "";
        CreateDesc.value = CreateDesc.label.text = "";
        CreateNoti.value = CreateNoti.label.text = "";

        ActivePopups(99);//전부끄기

        if (CheckLevel)//레벨이 부족할 경우 무시.(신청한것이 없을 테니까)
            NetworkClient.instance.SendPMsgGuildQueryGuildListC();

        UITabGroup tabGroup = transform.FindChild("TabBtnList").GetComponent<UITabGroup>();
        tabGroup.Initialize(delegate (int arr) {
            for (int i = 0; i < TabObj.Length; i++)
            {
                TabObj[i].SetActive(i == arr);
            }

            if (arr == 0)//랭킹
            {
                RankingList.Clear();
                NetworkClient.instance.SendPMsgRankGuildQueryC((int)Sw.RANK_TYPE.RANK_TYPE_GUILD_LEVEL);
            }
            else if (arr == 1)//길드 리스트
            {
                NetworkClient.instance.SendPMsgGuildRecommendListC();
            }
        });
    }

    public override void LateInit()
    {
        base.LateInit();

		CameraManager.instance.mainCamera.gameObject.SetActive (false);
    }

	public override void Hide()
	{
		// go here?
		CameraManager.instance.mainCamera.gameObject.SetActive (true);

		base.Hide();
		
		UIMgr.OpenTown();
	}
    
    void OnCallBackMark(int arr, GameObject go)
    {
        if(522 <= arr )
        {
            go.SetActive(false);
            return;
        }
        go.SetActive(true);

        string iconName = _LowDataMgr.instance.GetLowDataIcon((uint)(MarkStartId + arr) );
        if (string.IsNullOrEmpty(iconName) && !iconName.Contains("guild"))
        {
            go.SetActive(false);
            return;
        }

        go.GetComponent<UISprite>().spriteName = iconName;
        
        EventDelegate.Set(go.GetComponent<UIEventTrigger>().onClick, delegate () {

            GameObject select = go.transform.FindChild("Cover").gameObject;
            for (int i = 0; i < MarkTf.childCount; i++)
            {
                GameObject sel = MarkTf.GetChild(i).FindChild("Cover").gameObject;
                sel.SetActive(false);
            }
            if (select.activeSelf)
                select.SetActive(false);
            else
                select.SetActive(true);


            IconID = (uint)(MarkStartId + arr);

        });
    }
    
    /// <summary>   /// 랭킹가져옴 /// </summary>
    public void OnPMsgRankList(int cnt, List<NetData.RankInfo> RankList)
    {

        // 한번에 주지않고 여러번 나눠서 호출해줌

        if (RankingList.Count == 0)
        {
            RankingList = RankList;
            slotCnt = 0;
            totalCnt = 0;
        }
        else
        {

            RankingList.InsertRange(RankingList.Count, RankList);
            slotCnt += RankList.Count;  //다음 슬롯시작인덱스
        }

        RankingView(cnt, RankList);
    }

    public void RankingView(int cnt, List<NetData.RankInfo> RankList)
    {
        RankScroll.gameObject.SetActive(RankList.Count > 0 ? true : false);
        if (!RankScroll.gameObject.activeSelf)
        {
            uiMgr.AddPopup(141, 901, 117);
            return;
        }

        //최초호출시 스크롤뷰를 갱신
        if (totalCnt == 0)
            RankScroll.ResetPosition();

        //여러번 호출되기때문에 누적시켜줌
        totalCnt += cnt;
        // 맨처음 호출될때는 0번째슬롯부터 다음부터는 마지막생성된 슬롯부터~ 시작
        for (int i = slotCnt == 0 ? 0 : slotCnt; i < RankTf.childCount; i++)
        {

            if (i >= totalCnt)
            {
                RankTf.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = RankTf.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;

            slotGo.SetActive(true);

            NetData.RankInfo info = RankingList[i];

            UISprite icon = slotTf.FindChild("Mark").GetComponent<UISprite>();
            UILabel name = slotTf.FindChild("Txt_guildname").GetComponent<UILabel>();
            UILabel rank = slotTf.FindChild("Txt_ranking").GetComponent<UILabel>();

            icon.spriteName = _LowDataMgr.instance.GetLowDataIcon((uint)info.RoleType);
            name.text = info.Name;
            rank.text = string.Format(_LowDataMgr.instance.GetStringCommon(522), info.Rank.ToString());

            //길드 가입신청

            bool isJoinGuild = false;
            for (int j = 0; j < JoinList.Count; j++)
            {
                if (JoinList[j] != info.Id)
                    continue;

                isJoinGuild = true;
                break;
            }

            int idx = i;

            UIEventTrigger etri = slotTf.GetComponent<UIEventTrigger>();
            EventDelegate.Set(etri.onClick, delegate ()
            {
                //선택
                for (int j = 0; j < RankTf.childCount; j++)
                {
                    RankTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }

            });

            //가입신청버튼은 세종류로.
            //가입신청(215)
            //가입취소(가입승인 대기상태)(233)//
            //가입불가능(레벨불만족일경우)
            //slotTf.FindChild("Btn_join").GetComponent<UISprite>().spriteName = isJoinGuild ? "Btn_Blue01Dis" : "Btn_Blue01";
            slotTf.FindChild("Btn_join/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon((uint)(isJoinGuild ? 233 : 215));
            slotTf.FindChild("Btn_join").GetComponent<UISprite>().spriteName = CheckLevel ? "Btn_Blue01" : "Btn_Blue01Dis";

            EventDelegate.Set(slotTf.FindChild("Btn_join").GetComponent<UIButton>().onClick, delegate ()
            {
                //선택
                for (int j = 0; j < RankTf.childCount; j++)
                {
                    RankTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }
                if (CheckLevel)
                    NetworkClient.instance.SendPMsgGuildApplyGuildC((uint)info.Id, (uint)(isJoinGuild ? 2 : 1));//1가입 2 취소
                else
                    SetClickPopup(945, _LowDataMgr.instance.GetEtcTableValue<string>(EtcID.GuildCondition), null);
                //SetClickPopup(171, null, null);
            });

            //길드정보보기
            EventDelegate.Set(slotTf.FindChild("Btn_search").GetComponent<UIEventTrigger>().onClick, delegate () {
                //선택
                for (int j = 0; j < RankTf.childCount; j++)
                {
                    RankTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }

                NetworkClient.instance.SendPMsgGuildQueryBaseInfoC((uint)info.Id);
            });

        }
        RankTf.GetComponent<UIGrid>().Reposition();
    }

    /// <summary> 추천길드 리스트 받아오기 </summary>
    public void OnPMsgRecommendList(List<NetData.GuildSimpleInfo> recommendList)
    {
        RecommendList.Clear();
        RecommendList = recommendList;

        if (RecommendList == null || RecommendList.Count == 0)
            NotFoundGuildName.text = _LowDataMgr.instance.GetStringCommon(579);
        else
            NotFoundGuildName.text = "";

        SetRecommendList(true);
    }
    
    /// <summary> 길드 생성에 필요한 것 셋팅 </summary>
    void CreateGuild()
    {
        uiMgr.SetTopMenuTitleName(216);

        //ActivePopups(1);
        Popup[0].transform.FindChild("Select").gameObject.SetActive(false);
        Popup[0].transform.FindChild("Make").gameObject.SetActive(true);
        //CreateName.value = CreateName.label.text = "";
        //CreateDesc.value = CreateDesc.label.text = "";
        //CreateNoti.value = CreateNoti.label.text = "";

        //Popup[0].transform.FindChild("Make/Txt_master").GetComponent<UILabel>().text = NetData.instance.GetUserInfo()._charName;//내 캐릭이 길드장이 됨.
        Popup[0].transform.FindChild("Make/Guildmark/icon").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(IconID);


    }

    /// <summary> 팝업 키고 끄기 </summary>
    void ActivePopups(byte state)
    {
        for(int i=0; i < Popup.Length; i++)
        {
            Popup[i].SetActive(i == state);
        }
    }

    /// <summary> 자유가입 길드만 보기 </summary>
    void OnToggleFreeJoin()
    {
        if (!mStarted)
            return;

        Debug.Log("Select Free Join List " + ToggleFreeJoin.value);
        SetRecommendList(true);
    }

    void SetRecommendList(bool isResetView)
    {
        if(!TabObj[1].activeSelf)
        {
            TabObj[0].gameObject.SetActive(false);
            TabObj[1].gameObject.SetActive(true);
        }

        int count = ListTf.childCount, setCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (RecommendList.Count <= i)
            {
                ListTf.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            Transform tf = ListTf.GetChild(i);
            NetData.GuildSimpleInfo info = RecommendList[i];

            if (ToggleFreeJoin.value)
            {
                if (info.JoinSet ==1)//심사가입
                {
                    tf.gameObject.SetActive(false);
                    continue;
                }
            }

            tf.FindChild("Mark").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(info.Icon);

            tf.FindChild("Txt_guildname").GetComponent<UILabel>().text = info.Name;
            tf.FindChild("Txt_master").GetComponent<UILabel>().text = info.LeaderName;
            //tf.FindChild("Txt_info").GetComponent<UILabel>().text = info.;
            tf.FindChild("Txt_num").GetComponent<UILabel>().text = string.Format("{0}/{1}", info.Count, _LowDataMgr.instance.GetLowdataGuildInfo(info.guildLv).GuildJoinLimit);


            bool isJoinGuild = false;
            for (int j = 0; j < JoinList.Count; j++)
            {
                if (JoinList[j] != info.Id)
                    continue;

                isJoinGuild = true;
                break;
            }
            
            //tf.FindChild("Btn").GetComponent<UISprite>().spriteName = isJoinGuild ? "Btn_Blue02Dis" : "Btn_Blue02";

            //가입신청버튼은 세종류로.
            //가입신청(215)
            //가입취소(가입승인 대기상태)(233)//
            //가입불가능(레벨불만족일경우)

            tf.FindChild("Btn/label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon((uint)(isJoinGuild ? 233 : 215) );
            tf.FindChild("Btn").GetComponent<UISprite>().spriteName = CheckLevel ? "Btn_Blue01" :"Btn_Blue01Dis";

            int idx = i;

            UIEventTrigger etri = tf.GetComponent<UIEventTrigger>();
            EventDelegate.Set(etri.onClick, delegate ()
            {
                //선택
                for (int j = 0; j < count; j++)
                {
                    ListTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }

            });

            EventDelegate.Set(tf.FindChild("Btn").GetComponent<UIButton>().onClick, delegate () {

                //선택
                for (int j = 0; j < count; j++)
                {
                    ListTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }

                //가입레벨이 길드마다다르므로 체크레벨 비교..
                uint needLv = info.JoinLevel;
                if (NetData.instance.UserLevel >= needLv)
                    NetworkClient.instance.SendPMsgGuildApplyGuildC(info.Id, (uint)(isJoinGuild ? 2 : 1) );//1가입 2 취소
                else
                    SetClickPopup(945, needLv.ToString(), null);
                //SetClickPopup(171, null, null);
            });

            EventDelegate.Set(tf.FindChild("BtnInfo").GetComponent<UIEventTrigger>().onClick, delegate () {
                //선택
                for (int j = 0; j < count; j++)
                {
                    ListTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(j == idx);
                }
                OnGuildDetailInfo(info);
            });

            ++setCount;
            tf.gameObject.SetActive(true);
        }

        if (isResetView)
        {
            ListTf.parent.GetComponent<UIScrollView>().ResetPosition();
            ListTf.GetComponent<UIGrid>().repositionNow = true;
            ListTf.parent.GetComponent<UIPanel>().clipOffset = Vector3.zero;
            ListTf.parent.GetComponent<UIPanel>().transform.localPosition = Vector3.zero;
        }

        ListTf.parent.GetComponent<UIScrollView>().enabled = setCount > 4;

        if (setCount == 0)
        {
            NotFoundGuildName.text = _LowDataMgr.instance.GetStringCommon(579);
        }
        else
        {
            NotFoundGuildName.text = "";
        }
    }

    public override void Close()
    {
		// call here?
		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        if (!IsCreateGuild)
        {
            for (int i = Popup.Length - 1; 0 <= i; i--)
            {
                if (!Popup[i].activeSelf)
                    continue;

                Popup[i].SetActive(false);
                if(i == 1)
                    uiMgr.SetTopMenuTitleName(215);

                return;
            }

            UIMgr.OpenTown();
        }

        base.Close();
    }

    bool CheckLevel//가입 or 창설 레벨 체크
    {
        get {

            int needLevel = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCondition);
            if (NetData.instance.UserLevel < needLevel)
                return false;

            return true;
        }
    }
    
    /// <summary> 클릭 팝업 </summary>
    void SetClickPopup(uint msgLowId, string insert, System.Action ok_callBack)
    {
        Transform tf = Popup[2].transform;
        string msg = _LowDataMgr.instance.GetStringCommon(msgLowId);

        if ( !string.IsNullOrEmpty(insert) )
            msg = string.Format(msg, insert);

        tf.FindChild("msg").GetComponent<UILabel>().text = msg;

        if(ok_callBack != null)
        {
            //tf.FindChild("BtnOk/Lb_d2").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(0);
            //tf.FindChild("BtnCancel/Lb_d2").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(0);

            tf.FindChild("BtnCancel").gameObject.SetActive(true);
            Vector3 pos = tf.FindChild("BtnCancel").localPosition;
            pos.x = -pos.x;
            tf.FindChild("BtnOk").localPosition = pos;

            EventDelegate.Set(tf.FindChild("BtnOk").GetComponent<UIEventTrigger>().onClick, ()=> {
                ok_callBack();
                Popup[2].SetActive(false);
            });
        }
        else
        {
            tf.FindChild("BtnCancel").gameObject.SetActive(false);
            Vector3 pos = tf.FindChild("BtnCancel").localPosition;
            pos.x = 0;
            tf.FindChild("BtnOk").localPosition = pos;

            //tf.FindChild("BtnOk/Lb_d2").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(0);

            //메세지 팝업
            EventDelegate.Set(tf.FindChild("BtnOk").GetComponent<UIEventTrigger>().onClick, () => {
                Popup[2].SetActive(false);
            });
        }

        Popup[2].SetActive(true);
    }

    /// <summary> 길드 생성 완료 </summary>
    public void OnPMsgCreateGuild()
    {
        if( string.IsNullOrEmpty(CreateNoti.value ) )//공지가 비어있다면 바로 내 길드로 넘어감.
        {
            base.Close();//닫고
            UIMgr.OpenGuildPanel();//길드 페이지로 넘긴다.
        }
        else
        {
            string noti = CreateNoti.value;
            _LowDataMgr.instance.IsBanString(ref noti);

            NetworkClient.instance.SendPMsgGuildChangeNameDeclarationAnnouncementC(NetData.instance.GetUserInfo()._GuildId, 3, noti);

        }
        IsCreateGuild = true;
    }

    /// <summary> 길드 정보 </summary>
    public void OnGuildDetailInfo(NetData.GuildSimpleInfo simpleInfo)
    {
        ActivePopups(1);
        Popup[1].transform.FindChild("Main").gameObject.SetActive(true);
        //Popup[1].transform.FindChild("Make").gameObject.SetActive(false);
        
        Transform infoTf = Popup[1].transform.FindChild("Main");
        infoTf.FindChild("Info/guild_name").GetComponent<UILabel>().text = simpleInfo.Name;
        infoTf.FindChild("Info/master_name").GetComponent<UILabel>().text = simpleInfo.LeaderName;
     //   infoTf.FindChild("Info/declaration").GetComponent<UILabel>().text = simpleInfo.Declaration;

        infoTf.FindChild("Info/mark").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(simpleInfo.Icon);

        infoTf.FindChild("Info/guildStr").GetComponent<UILabel>().text = simpleInfo.Attack.ToString();
        infoTf.FindChild("Info/guildmake").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(577), simpleInfo.CreateTime.ToString().Substring(0, 4), simpleInfo.CreateTime.ToString().Substring(4, 2), simpleInfo.CreateTime.ToString().Substring(6, 2));
        //simpleInfo.CreateTime.ToString();
        // infoTf.FindChild("BtnRanking/label").GetComponent<UILabel>().text = string.Format("{0}", _LowDataMgr.instance.GetStringCommon(560) );
        infoTf.FindChild("Info/mamber").GetComponent<UILabel>().text = string.Format("{0}/{1}", simpleInfo.Count.ToString(), _LowDataMgr.instance.GetLowdataGuildInfo(simpleInfo.guildLv).GuildJoinLimit); // ToString("#,##");


        NetworkClient.instance.SendPMsgGuildQueryDetailedInfoC(simpleInfo.Id);
    }

    /// <summary> 길드 상세 정보 응답 </summary>
    public void OnPMsgGuildDetailInfo(uint guildLv, uint blessLv, uint shopLv, string anoun,string notice,ulong money )
    {

        Transform infoTf = Popup[1].transform.FindChild("Main");

        _LowDataMgr.instance.IsBanString(ref anoun);
        infoTf.FindChild("Info/declaration").GetComponent<UILabel>().text = anoun;
        _LowDataMgr.instance.IsBanString(ref notice);
        infoTf.FindChild("Info/notice").GetComponent<UILabel>().text = notice;

        infoTf.FindChild("Info/guildmoney").GetComponent<UILabel>().text = money.ToString();



        //Popup[1].transform.FindChild("Main/Info/notice").GetComponent<UILabel>().text = notice;
        //Popup[1].transform.FindChild("Main/Info/declaration").GetComponent<UILabel>().text = anoun;

        //Transform infoTf = Popup[1].transform.FindChild("Main/Icon");
        //string lvStr = _LowDataMgr.instance.GetStringCommon(453);
        //infoTf.FindChild("Lobby/lv").GetComponent<UILabel>().text = string.Format(lvStr, guildLv);
        //infoTf.FindChild("Shop/lv").GetComponent<UILabel>().text = string.Format(lvStr, shopLv);
        //infoTf.FindChild("Pray/lv").GetComponent<UILabel>().text = string.Format(lvStr, blessLv);

    }

    /// <summary> 길드 가입 응답 </summary>
    public void OnPMsgApply(uint guildId, bool isJoin)
    {
        if(isJoin)
            JoinList.Add(guildId);
        else
            JoinList.Remove(guildId);


        if (TabObj[0].activeSelf/*RankScroll.gameObject.activeSelf*/)
        {
            slotCnt = 0;
            totalCnt = 0;
            RankingView(RankingList.Count, RankingList);
        }
        else
        {
            SetRecommendList(false);

        }
    }

    /// <summary> 길드 이름으로 찾기 결과 응답 </summary>
    public void OnPMsgSearchGuild(List<NetData.GuildSimpleInfo> guildInfo)
    {
        string str = null;
        if (guildInfo == null || guildInfo.Count == 0)
        {
            str = string.Format("({0}) {1}", SearchGuild.value, _LowDataMgr.instance.GetStringCommon(579));
            guildInfo = new List<NetData.GuildSimpleInfo>();
        }
        SearchGuild.label.text = SearchGuild.value = "";

        //RecommendList.Clear();
        RecommendList.InsertRange(RecommendList.Count, guildInfo); //나눠서 호출되므로...       

        for (int j = 0; j < ListTf.childCount; j++)
        {
            ListTf.GetChild(j).transform.FindChild("Cover").gameObject.SetActive(false);
        }

        bool active = guildInfo.Count > 0;
            TabObj[0].SetActive(!active);
            TabObj[1].SetActive(active);

        SetRecommendList(true);

        NotFoundGuildName.text = str;
    }

    /// <summary> 내가 신청한 길드 리스트 응답 </summary>
    public void OnPMsgJoinList(List<uint> joinList)
    {
        JoinList = joinList;

        if (0 < RecommendList.Count )//먼저 추천리스트를 받았다. 재갱신을 시켜준다.
            SetRecommendList(true);
    }

    /// <summary> 길드 신청한 곳에서 응답이 왔다. </summary>
    public void OnPMsgJoinGuild()
    {
        base.Close();
        UIMgr.OpenGuildPanel();
    }
}

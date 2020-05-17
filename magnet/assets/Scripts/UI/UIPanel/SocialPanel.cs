using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SocialPanel : UIBasePanel
{

    enum ViewType { None = -1, List = 0, Recieve, Request, } //친구탭에서 view
// 리스트에서 접속시간이나 요청시간
    enum TimeMsg
    {
        Connect = 1,      //친구 최종접속시간 "{0}일 {1}시간 {2}분 전 접속함"
        Recieve,          //받은 요청시간 "{0}시간 {1}분 전 친구요청옴"
        Send,             //보낸 요청시간 "{0}일 {1}시간 {2}분 전 친구요청함"
        Heart,            //체력 초기화시간 "{0} 시간 or "{0}분" 
        Remove,           //친구 요청유효기간은 3일 , 3일지났을시 삭제요청 보내줘야함
    }
    private ViewType CurViewType = ViewType.None;//현재 보고있는 친구 뷰 타입

    public GameObject MailSlotPref; //메일슬롯
    public GameObject MailNullLabel;    //메일없습니다.라벨

    public Transform MailDetailTf;   //메일상셍정보

    public UIButton BtnMailGetAll;    //모두받기
    public UIButton BtnMailDelAll;    //모두삭제
    public GameObject InvenSlotPref;//
    public UIGrid ContentItemGrid;
    public UIScrollView ContentItemScrollView;

    public UIScrollView MailScroll;
    public UIGrid MailGrid; //메일그리드

    public UILabel MailCount;   //메일수
    
    public Transform[] TabBtnRoot; 
    public GameObject[] ViewObj;    //0 친구 1메일
    public GameObject[] FriendViewObj;  //친구탭에서 뷰

    public GameObject[] TabAlram;   //탭의빨간점(1친구0메일)
    public GameObject RcvFreindAlram; //받은요청알림

    public GameObject FriendListEmpty;
    public GameObject RcvFriendEmpty;
    public GameObject MakeFriendEmpty;

    public UITabGroup FriendTabGroup;   //친구탭그룹(친구/받은요청/보낸요청)

    public GameObject RecomandFrined;   //추천친구창은 두군데서 쓰이니 따로뺴줌
    public GameObject ChatView;         //채팅창
    public GameObject SearchFriendResult;   //친구검색결과창

    public GameObject FriendSlotPref; //친구프리펩
    public GameObject RcvFreindSlotPref;    //받은요청 프리펩
    public GameObject MakeFriendSlotPref;   //보낸요청 프리펩
    public GameObject RecFriendSlotPref;    //추천친구 프리펩
    public GameObject SearchfriendPrefab;       //친구검색 프리펩

    public UIScrollView FriendScrollView;
    public UIScrollView RcvFreinddScrollView;
    public UIScrollView MakeFriendScrollView;
    public UIScrollView RecFriendScrollView;
    public UIScrollView SearchFreidnScrollView; //친구검색

    public GameObject RecommendListEmpty;   //추천친구목록비었습니다.

    public UIGrid FriendGrid;
    public UIGrid RcvFriendGrid;
    public UIGrid MakeFriendGrid;
    public UIGrid RecFriendGrid;
    public UIGrid SearchFriendGrid;       //내가요청한친구

    public UILabel MyFriendCount;   //내친구수
    public UILabel RequestFriendCount;   //내가 요청한 친구수
    public UILabel CurCostumStatus;         //현제 장착중인 코스튬의 신분

    public UIButton BtnSearch;              //친구찾기버튼
    public UIButton BtnSearchSearch;        // 친구검색 창에서 친구찾기버튼
    public UIButton BtnAllGift; //하트모두보내기

    public UIInput InputSearchName;
    public UIInput InputSearchNameSearh;
    public UIInput InputChat;

    public UITextList ChatList;//채팅 로그

    public Transform EquipPartsTf;//장착중인 아이템들의 어미 자식들 찾아서 셋팅하려는 의도 ITEM_PARTS_TYPE를 이용하면 됨.
    public Transform CharModelTf;//캐릭터 모델 어미
    //public GameObject friendinfo;           //캐릭터 정보창
    public UILabel CharNameAndLevel;        //캐릭터 뷰에서 보여줄 캐릭터 이름, 레벨

    private List<NetData.FriendRequestBaseInfo> RecieveList = new List<NetData.FriendRequestBaseInfo>();
    private List<NetData.FriendBaseInfo> RecommendList = new List<NetData.FriendBaseInfo>();
    private List<NetData.FriendBaseInfo> MyfriendList = new List<NetData.FriendBaseInfo>();
    private List<NetData.FriendRequestBaseInfo> SendList = new List<NetData.FriendRequestBaseInfo>();
    private List<NetData.FriendBaseInfo> SearchFriendList= new List<NetData.FriendBaseInfo>();

    //private NetData.FriendFullInfo FriendFullInfoData;
    private uint CharIdx;   //친구 플레이어 생성할때 필요한 값
    //private ItemDetailPopup DetailPopup;//아이템 상세 팝업
    private long WhisperTargetId;//귓말 대상 아이디
    private string WhisperTargetName;//귓말 대상 이름
    private ObjectPaging MailPaging;

    public static Dictionary<string, List<string>> TalkHistory = new Dictionary<string, List<string>>();
    private List<NetData.EmailBaseInfo> EMailDataList = new List<NetData.EmailBaseInfo>();
    private NetData.EamilDetails CurEMailData;
    private Mail.MailInfo mailInfo = new Mail.MailInfo();

    private int totalCnt;   //여러번호출되니 .. 누적값을 더해줘서 생성되야할 슬롯이 몇개인지 알려줌
    private int slotCnt;
    private bool IsDuringDelete;

    private UIBasePanel BasePanel;


    private int DetailArr = -1;    
    class MailItemData
    {
        public string itmeType;
        public string itemLink;
        public string itmeCnt;

        public MailItemData(string type, string link, string cnt)
        {
            itmeType = type;
            itemLink = link;
            itmeCnt = cnt;
        }
    }

    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("Social panel, Init() start");

        base.Init();
        ViewObj[0].SetActive(false);
        ViewObj[1].SetActive(false);

        //슬롯 미리생성
        CreateSlot();
        //FriendFullInfoData = NetData.instance.GetFriendFullInfo();

        for (int i = 0; i < 2; i++)
        {
            int idx = i;
            UIButton tabBtn = TabBtnRoot[i].GetComponent<UIButton>();
            EventDelegate.Set(tabBtn.onClick, delegate ()
            {
                OnclickTabBtn(idx);
            });
        }

        //DetailPopup = UIMgr.OpenDetailPopup(this, 8);

        //UIButton infoClose = friendinfo.transform.FindChild("BtnClose").GetComponent<UIButton>();
        //EventDelegate.Set(infoClose.onClick, delegate ()
        //{
        //    friendinfo.SetActive(false);
        //    FriendFullInfoData.ClearData();
        //});

        EventDelegate.Set(BtnSearch.onClick, OnClickSearch);
        EventDelegate.Set(BtnSearchSearch.onClick, OnClickSearch);
        EventDelegate.Set(BtnAllGift.onClick, delegate () { OnClickAllGift(); });

        UIButton closeDetail = MailDetailTf.FindChild("Btn_0/Btn_up").GetComponent<UIButton>();
        EventDelegate.Set(closeDetail.onClick, delegate ()
        {
            //BtnMailGetAll.isEnabled = true;
            //BtnMailDelAll.isEnabled = true;
            DetailArr = -1;
            MailDetailTf.gameObject.SetActive(false);
            MailPaging.RefreshSlot(totalCnt);
            //MailGrid.Reposition();
        });

        EventDelegate.Set(BtnMailGetAll.onClick, delegate ()
        {
            if (EMailDataList == null || EMailDataList.Count <= 0)
            {
                uiMgr.AddPopup(141, 955, 117);
                return;
            }

            bool isSend = false;
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (EMailDataList[i].IsReceive == 2)
                    continue;

                isSend = true;
                break;
            }

            if (!isSend)
                uiMgr.AddPopup(141, 955, 117);
            else
            {
                SceneManager.instance.ShowNetProcess("GetAllMail");
                NetworkClient.instance.PMsgEmailOneKeyFeatchC();
            }
        });
        EventDelegate.Set(BtnMailDelAll.onClick, delegate ()
        {
            if (EMailDataList == null || EMailDataList.Count == 0)
            {
                uiMgr.AddPopup(141, 955, 117);
                return;
            }

            if (IsDuringDelete)
                return;

            int rcvCnt = 0;

            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (EMailDataList[i].IsReceive != 2)
                {
                    rcvCnt++;
                }
            }

            // 아무것도 수령안했다면 (지울게 없다면) 팝업창 띄움
            if (rcvCnt == EMailDataList.Count)
            {
                //UIMgr.instance.AddPopup(29, null, null, null);
                uiMgr.AddPopup(141, 308, 117);
                return;
            }

            //UIMgr.instance.AddPopup(28, delegate () { NetworkClient.instance.PMsgEmailOnKeyDelC(); }, null, null);//삭제팝업
            uiMgr.AddPopup(141, 307, 117, 76, 0
                , delegate ()
                {
                    SceneManager.instance.ShowNetProcess("DelAllMail");
                    NetworkClient.instance.PMsgEmailOnKeyDelC();
                });

        });
        EventDelegate.Set(ChatView.transform.FindChild("Chat/Btn_request").GetComponent<UIButton>().onClick, delegate ()
        {
            if (0 < WhisperTargetId)
            {
                string msg = InputChat.value;
                msg.Replace("<color=", "");
                msg.Replace("\n", "");
                if (string.IsNullOrEmpty(msg))
                    return;
                else if (_LowDataMgr.instance.IsBanString(msg))
                {
                    string errorMsg = _LowDataMgr.instance.GetStringCommon(898);
                    errorMsg = errorMsg.Replace("\n", "");
                    OnReciveChat(errorMsg, false);
                    return;
                }

                NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_FRIEND, WhisperTargetId, WhisperTargetName, InputChat.value, 1);
            }
            else//대상을 찾을 수 없음.
            {
                //uiMgr.AddPopup(141, 908, 117);
                OnReciveChat("[FF0000]" + _LowDataMgr.instance.GetStringCommon(908) + "[-]", false);
            }
        });


		SceneManager.instance.showStopWatchTimer ("Social panel, Init() finish");
    }

    public override void LateInit()
    {
        base.LateInit();

        //if (parameters !=null && parameters[0] != null)
        //{
        //    int type = (int)parameters[0];
        //    OnclickTabBtn(type);
        //}

        //ViewObj[0].SetActive(false);
        //OnclickTabBtn(0);   
        FriendTabGroup.Initialize(OnClickFriendTab);

        TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));
        TabAlram[1].SetActive(SceneManager.instance.IsAlram(AlramIconType.SOCIAL));
        RcvFreindAlram.SetActive(SceneManager.instance.IsAlram(AlramIconType.SOCIAL)); 



		SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("Social panel, LateInit() finish");

        if(0 < parameters.Length)
        {
            switch( (int)parameters[0])
            {
                case 0:
                    OnclickTabBtn(0);
                    break;
                case 1 ://친구창
                    OnclickTabBtn(1);
                    break;
                case 5://친구와의 대화.
                    UIBasePanel userPop = UIMgr.GetUIBasePanel("UIPopup/UserInfoPopup");
                    long roleId = (userPop as UserInfoPopup).UserRoleId;

                    userPop.Close();//여기서 죽인다.
                    break;
            }

            BasePanel = (UIBasePanel)parameters[1];
        
        }

        if (SceneManager.instance.CurTutorial == TutorialType.SOCIAL)
            OnSubTutorial();
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}


    void CreateSlot()
    {
        // 친구리스트 
        for (int i = 0; i < 50; i++)
        {
            GameObject slotGo = Instantiate(FriendSlotPref) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = FriendGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(FriendSlotPref);
        //추천친구 리스트
        for (int i = 0; i < 50; i++)
        {
            GameObject slotGo = Instantiate(RecFriendSlotPref) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = RecFriendGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(RecFriendSlotPref);

        //요청받은 리스트 
        for (int i = 0; i < 50; i++)
        {
            GameObject slotGo = Instantiate(RcvFreindSlotPref) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = RcvFriendGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(RcvFreindSlotPref);

        // 요청보낸 리스트
        for (int i = 0; i < 50; i++)
        {
            GameObject slotGo = Instantiate(MakeFriendSlotPref) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = MakeFriendGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(MakeFriendSlotPref);

        // 친구검색 리스트
        for (int i = 0; i < 20; i++)
        {
            GameObject slotGo = Instantiate(SearchfriendPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = SearchFriendGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotTf.gameObject.SetActive(false);
        }
        Destroy(SearchfriendPrefab);

        MailPaging = ObjectPaging.CreatePagingPanel(MailScroll.gameObject, MailGrid.gameObject, MailSlotPref, 1, 6, _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MailMaxCount)
            , 0, OnMailSlot);
        Destroy(MailSlotPref);

        for (int i = 0; i < 10; i++)
        {
            GameObject slotGo = Instantiate(InvenSlotPref) as GameObject;
            slotGo.transform.parent = ContentItemGrid.transform;
            slotGo.transform.localPosition = Vector3.zero;
            slotGo.transform.localScale = Vector3.one;
        }
        Destroy(InvenSlotPref);
    }

    // 친구/메일 구분탭버튼
    void OnclickTabBtn(int index)
    {
        //if (ViewObj[index].activeSelf)
        //    return;

        if (IsDuringDelete)
        {
            bool isAlram = false;
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (!isAlram && EMailDataList[i].IsReceive == 0)//안읽은
                    isAlram = true;
            }
            SceneManager.instance.SetAlram(AlramIconType._SOCIAL, isAlram);
            TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));
        }

        slotCnt = 0;
        totalCnt = 0;
        EMailDataList.Clear();
        DetailArr = -1;
        MailDetailTf.gameObject.SetActive(false);

        if (index==0)
        {
            SceneManager.instance.ShowNetProcess("GetMailList");
            NetworkClient.instance.PMsgEmailQueryListC();

        }
        UIMgr.instance.SetTopMenuTitleName(index == 0 ? (uint)207 : (uint)136);

        for (int i=0;i<ViewObj.Length;i++)
        {
            Transform btn = TabBtnRoot[i];
            btn.collider.enabled = i != index;
            ViewObj[i].SetActive(i == index);

            btn.FindChild("tab_on").gameObject.SetActive(i == index);
            btn.FindChild("tab_off").gameObject.SetActive(i != index);

        }
        InputSearchNameSearh.value = "";
        InputSearchName.value = "";
        InputChat.value = "";
    }


    #region 친구

    void OnClickFriendTab(int viewType)
    {
        ChangeView((ViewType)viewType);

    }
    /// <summary>
    /// List, Recive, Request 뷰로 전환한다.
    /// </summary>
    /// <param name="type"> 바꾸고자 하는 뷰의 타입</param>
    void ChangeView(ViewType type)
    {
        if (CurViewType == type)//동일하므로 무시
            return;

        myFirndSlotCnt = 0;
        myFriendTotalCnt = 0;
        MyfriendList.Clear();

        rcvFriendSlotCnt = 0;
        rcvFriendTotalCnt = 0;
        RecieveList.Clear();

        sendFriendSlotCnt = 0;
        sendFriendTotalCnt = 0;
        todaySendCount = 0;
        SendList.Clear();

        InputSearchNameSearh.value = "";
        InputSearchName.value = "";
        InputChat.value = "";

        for (int i = 0; i < FriendViewObj .Length; i++)
        {
            bool active = false;
            if (i == (uint)type)
                active = true;

            FriendViewObj[i].SetActive(active);
        }

        RecomandFrined.SetActive(type != ViewType.Recieve); //추천친구는 친구리스트랑 보낸요청에서만 나옴
        SearchFriendResult.SetActive(false);

        ViewType prevType = CurViewType;
        CurViewType = type;

        switch (type)
        {
            case ViewType.List:
                FriendListView();
                break;
            case ViewType.Recieve:
                RecieveFriendView();
                break;
            case ViewType.Request:
                RequestFriend();
                break;

        }

    }

    /// <summary>
    /// 친구리스트 뷰를 보여준다.
    /// </summary>
    public void FriendListView()
    {

        ChatView.SetActive(false);

        // 친구 리스트 
        SceneManager.instance.ShowNetProcess("GetFriendList");
        NetworkClient.instance.SendPMsgFriendQueryListC();
        SceneManager.instance.ShowNetProcess("GetRecFriendList");
        NetworkClient.instance.SendPMsgFriendRecommendListC();

    }
    /// <summary>
    /// 받은요청 뷰를 보여준다.
    /// </summary>
    void RecieveFriendView()
    {
        RecomandFrined.SetActive(false);//추천친구 꺼놈

        // 받은 요청 리스트
        SceneManager.instance.ShowNetProcess("GetRcvFriendList");
        NetworkClient.instance.SendPMsgFriendRequestFriendListC();

    }

    /// <summary>
    /// 친구맺기 뷰를 보여준다.
    /// </summary>
    void RequestFriend()
    {
        RecomandFrined.SetActive(true);//추천친구 켜놈

        //내가 요청한 친구 리스트
        SceneManager.instance.ShowNetProcess("GetReqFriendList");
        NetworkClient.instance.SendPMsgFriendSelfRequestFriendListC();
        // 추천친구 리스트
        SceneManager.instance.ShowNetProcess("GetRecFriendList");
        NetworkClient.instance.SendPMsgFriendRecommendListC();


    }

    // <summary> 친구찾기  /// </summary>
    void OnClickSearch()
    {

        string msg = "";
        if (SearchFriendResult.activeSelf == true)
            msg = InputSearchNameSearh.value;
        else
            msg = InputSearchName.value;
        if (string.IsNullOrEmpty(msg))
            return;

        // input에 검색할내용이 있을때만 창이열림
        msg.Replace("\n", "");
        SceneManager.instance.ShowNetProcess("SearchFriend");
        NetworkClient.instance.SendPMsgFriendSearchC(msg);

#if UNITY_ANDROID && !UNITY_EDITOR
        NativeHelper.instance.DisableNavUI();
#endif
    }

    /// <summary> 모두에게 하트보내기    /// </summary>
    void OnClickAllGift()
    {
        // 친구리스트에서 체력 안보낸 사람들에게 전부 보내줌
        for (int i = 0; i < MyfriendList.Count; i++)
        {
            if (MyfriendList[i].ullSendPowerTime == 0 || CanGivePower(MyfriendList[i].ullSendPowerTime) == true)
            {
                SceneManager.instance.ShowNetProcess("GiveHeart");
                NetworkClient.instance.SendPMsgFriendGivePowerC(MyfriendList[i].ullRoleId);
            }
        }
    }
    /// <summary> 하트 보내기 가능하냐 체크   /// </summary>
    bool CanGivePower(ulong SendTime)
    {
        //친구추가후 한번이라도 보낸경우 ullSendPowerTime 이 0이 아니게 된다.
        //따라서 ullSendPowerTime 과 오늘날짜를 비교해서 오늘날짜가 아니면 체력보낼수 있는경우로 봐야함
        int year = int.Parse(SendTime.ToString().Substring(0, 4));
        int month = int.Parse(SendTime.ToString().Substring(4, 2));
        int day = int.Parse(SendTime.ToString().Substring(6, 2));

        System.DateTime Now = System.DateTime.Now;    //현재시간
        System.DateTime Time = new System.DateTime(year, month, day, 0, 0, 0);

        System.TimeSpan ts = Now - Time;
        if (ts.TotalDays >= 1)
            return true;
        else
            return false;
    }

    #region 친구 프토로콜 응답

    int myFirndSlotCnt = 0;
    int myFriendTotalCnt = 0;

    public void FriendView(List<NetData.FriendBaseInfo> List, ulong chatUserRoleId = 0)
    {
        MyFriendCount.text = string.Format("{0} / {1}", MyfriendList.Count, _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.maxFriend));
        FriendListEmpty.SetActive(MyfriendList.Count == 0 ? true : false);

        for (int i = myFirndSlotCnt == 0 ? 0 : myFirndSlotCnt; i < FriendGrid.transform.childCount; i++)
        {
            if (i >= myFriendTotalCnt)
            {
                FriendGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = FriendGrid.transform.GetChild(i).gameObject;
            Transform sltTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.FriendBaseInfo rcvData = MyfriendList[i];

            UILabel name = slotGo.transform.FindChild("profile/name").GetComponent<UILabel>();
            name.text = string.Format("{0}", rcvData.szName);
            UILabel lv = slotGo.transform.FindChild("profile/level").GetComponent<UILabel>();
            lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);

            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.nLookId);
            UISprite Faces = slotGo.transform.FindChild("profile/Icon").GetComponent<UISprite>();
            Faces.spriteName = charLowData.PortraitId;

            // 친구 채팅연결 
            UIEventTrigger info = sltTf.FindChild("profile").GetComponent<UIEventTrigger>();
            EventDelegate.Set(info.onClick, delegate () { OnClickChat(rcvData.ullRoleId, rcvData.szName); });

            if (0 < chatUserRoleId && chatUserRoleId.Equals(rcvData.ullRoleId))//1:1 대화로 온것 강제로 연결.
            {
                OnClickChat(rcvData.ullRoleId, rcvData.szName);
            }

            ///친구 아이템정보창
            UIButton uiBtnInfo = slotGo.transform.FindChild("Btn_Info").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnInfo.onClick, delegate ()
            {
                UIMgr.OpenUserInfoPopup((long)rcvData.ullRoleId, rcvData.szName, (int)rcvData.nLookId, 0, (int)rcvData.nLevel, true);
                //friendinfo.SetActive(true);
                //NetworkClient.instance.SendPMsgQueryRoleInfoC((long)rcvData.ullRoleId);
                //CharIdx = rcvData.nLookId;

                //string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);
                //string NameAndLevel = string.Format("{0} {1}", lvStr, rcvData.szName);
                //CharNameAndLevel.text = NameAndLevel;
            });

            //버튼 초기화 시간을 출력해준다(설정값은 24시간, 23시간. .... 40분 이런식)
            UILabel timelabel = slotGo.transform.FindChild("BtnHeart/Label").GetComponent<UILabel>();
            timelabel.text = TransformTime(rcvData.ullSendPowerTime.ToString(), (int)TimeMsg.Heart);
            UIButton heart = slotGo.transform.FindChild("BtnHeart").GetComponent<UIButton>();
            if (string.IsNullOrEmpty(timelabel.text))
            {
                // 초기화완료
                timelabel.gameObject.SetActive(false);
                heart.isEnabled = true;
                EventDelegate.Set(heart.onClick, delegate ()
                {
                    SceneManager.instance.ShowNetProcess("GiveHeart");
                    NetworkClient.instance.SendPMsgFriendGivePowerC(rcvData.ullRoleId);
                });
            }
            else
            {
                // 아직 쿨타임
                timelabel.gameObject.SetActive(true);
                heart.isEnabled = false;
            }

            // 친구삭제  
            UIButton uiBtnDelete = slotGo.transform.FindChild("BtnDelete").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnDelete.onClick, delegate ()
            {
                uiMgr.AddPopup(141, 454, 117, 76, 0, () =>
                {
                    SceneManager.instance.ShowNetProcess("DelFriend");
                    NetworkClient.instance.SendPMsgFriendDelFriendC(rcvData.ullRoleId);
                });
            });

        }
        
        FriendGrid.Reposition();
        FriendScrollView.enabled = MyfriendList.Count >= 4 ? true : false;
    }

    /// <summary>  친구리스트 갱신 </summary>
    public void GetFriendList(List<NetData.FriendBaseInfo> List, ulong chatUserRoleId=0)
    {
        // 여러번올수있기때문에 쳌  
        if(MyfriendList.Count ==0)
        {
            MyfriendList = List;
            myFirndSlotCnt = 0;
            myFriendTotalCnt = 0;
        }
        else
        {
            MyfriendList.InsertRange(MyfriendList.Count, List);
            myFirndSlotCnt += MyfriendList.Count;
        }

        myFriendTotalCnt += List.Count;
        FriendView(List, chatUserRoleId);
    }

    //친구꽉찼을때 추천친구리스트에러주는것 여기서처리
    public void EmptyRecommend()
    {
        //에러팝업대신 UI에표기해준다.
        RecommendListEmpty.SetActive(true);
        RecFriendScrollView.transform.gameObject.SetActive(false);
    }

    // 추천친구 서버에서 받아옴
    public void GetRecommendList(List<NetData.FriendBaseInfo> List)
    {
        RecommendList = List;
        RecommendListEmpty.SetActive(false);
        RecFriendScrollView.transform.gameObject.SetActive(true);

        bool isTutoSet = SceneManager.instance.CurTutorial == TutorialType.SOCIAL;
        for (int i = 0; i < RecFriendGrid.transform.childCount; i++)
        {
            if (i >= List.Count)
            {
                RecFriendGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = RecFriendGrid.transform.GetChild(i).gameObject;
            Transform sltTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.FriendBaseInfo rcvData = RecommendList[i];

            UILabel name = slotGo.transform.FindChild("profile/name").GetComponent<UILabel>();
            name.text = string.Format("{0}", rcvData.szName);
            UILabel lv = slotGo.transform.FindChild("profile/level").GetComponent<UILabel>();
            lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);

            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.nLookId);
            UISprite Faces = slotGo.transform.FindChild("profile/Icon").GetComponent<UISprite>();
            Faces.spriteName = charLowData.PortraitId;

            // 친구요청       
            UIButton uiBtnSend = slotGo.transform.FindChild("BtnSend").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnSend.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("AddFriend");
                NetworkClient.instance.SendPMsgFriendAddC(rcvData.ullRoleId);
            });

            if(isTutoSet)
            {
                isTutoSet = false;
                TutorialSupport tutoSupport = uiBtnSend.gameObject.AddComponent<TutorialSupport>();
                tutoSupport.TutoType = TutorialType.SOCIAL;
                tutoSupport.SortId = 4;
                tutoSupport.IsScroll = true;

                TabBtnRoot[1].GetComponent<TutorialSupport>().NextTuto = tutoSupport;
                //tutoSupport.OnTutoSupportStart();
            }

            RecFriendGrid.Reposition();
            RecFriendScrollView.enabled = RecommendList.Count >= 4 ? true : false;
        }
    }

    int sendFriendSlotCnt = 0;
    int sendFriendTotalCnt = 0;
    int todaySendCount = 0; //당일신청횟수

    // 내가추가한 친구리스트 서버에서 받아옴
    public void GetSendList(List<NetData.FriendRequestBaseInfo> List)
    {

        // 여러번올수있기때문에 쳌  
        if (SendList.Count == 0)
        {
            SendList = List;
            sendFriendSlotCnt = 0;
            sendFriendTotalCnt = 0;
        }
        else
        {
            SendList.InsertRange(SendList.Count, List);
            sendFriendSlotCnt += SendList.Count;
        }

        sendFriendTotalCnt += List.Count;

        //MyfriendList = List;
        SendView();
       
    }


    public void SendView()
    {
        MakeFriendEmpty.SetActive(SendList.Count == 0 ? true : false);

        for (int i = sendFriendSlotCnt ==0 ? 0 : sendFriendSlotCnt; i < MakeFriendGrid.transform.childCount; i++)
        {
            if (i >= sendFriendTotalCnt)
            {
                MakeFriendGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = MakeFriendGrid.transform.GetChild(i).gameObject;
            Transform sltTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.FriendRequestBaseInfo rcvData = SendList[i];

            string cancleTime = TransformTime(rcvData.ullRequestTime.ToString(), (int)TimeMsg.Remove);

            if (cancleTime == null) //3일지나면 삭제 
            {
                // 삭제요청 
                //NetworkClient.instance.SendPMsgFriendSelfRequestInvalidC(rcvData.ullRoleId);
                slotGo.SetActive(false);
                return;
            }

            slotGo.SetActive(true);

            System.DateTime dateTime = System.DateTime.ParseExact(rcvData.ullRequestTime.ToString(), "yyyyMMddHHmmss", null);
            if (dateTime.Year == System.DateTime.Now.Year && dateTime.Month == System.DateTime.Now.Month
                && dateTime.Day == System.DateTime.Now.Day)//오늘신청횟수
            {
                todaySendCount++;
            }

            UILabel name = slotGo.transform.FindChild("profile/name").GetComponent<UILabel>();
            name.text = string.Format("{0}", rcvData.szName);
            UILabel lv = slotGo.transform.FindChild("profile/level").GetComponent<UILabel>();
            lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);

            UILabel elapsed_time = slotGo.transform.FindChild("lastRecommend/label").GetComponent<UILabel>();


            elapsed_time.text = TransformTime(rcvData.ullRequestTime.ToString(), (int)TimeMsg.Send);/*rcvData.ullRequestTime.ToString();*/ //TransformTime((int)rcvData.elapsed_time, (int)TimeMsg.Send);


            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.unRoleType);
            UISprite Faces = slotGo.transform.FindChild("profile/Icon").GetComponent<UISprite>();
            Faces.spriteName = charLowData.PortraitId;

            // 요청취소
            UIButton uiBtnCancel = slotGo.transform.FindChild("BtnCancel").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnCancel.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("CancleFriend");
                NetworkClient.instance.SendPMsgFriendCancleAddC(rcvData.ullRoleId);
            });

        }

        MakeFriendGrid.Reposition();
        MakeFriendScrollView.enabled = SendList.Count >= 4 ? true : false;
        RequestFriendCount.text = string.Format("{0} {1} / {2}", _LowDataMgr.instance.GetStringCommon(1050), todaySendCount, _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.friendInvite));
    }


    int rcvFriendSlotCnt = 0;
    int rcvFriendTotalCnt = 0;
    // 친구요청리스트 
    public void GetRecieveList(List<NetData.FriendRequestBaseInfo> List)
    {

        // 여러번올수있기때문에 쳌  
        if (RecieveList.Count == 0)
        {
            RecieveList = List;
            rcvFriendSlotCnt = 0;
            rcvFriendTotalCnt = 0;
        }
        else
        {
            RecieveList.InsertRange(RecieveList.Count, List);
            rcvFriendSlotCnt += RecieveList.Count;
        }

        rcvFriendTotalCnt += List.Count;

        //MyfriendList = List;
        RecieveView();
    }

    public void RecieveView()
    {
        RcvFriendEmpty.SetActive(RecieveList.Count == 0 ? true : false);

        for (int i = rcvFriendSlotCnt==0? 0:rcvFriendSlotCnt; i < RcvFriendGrid.transform.childCount; i++)
        {
            if (i >= rcvFriendTotalCnt)
            {
                RcvFriendGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = RcvFriendGrid.transform.GetChild(i).gameObject;
            Transform sltTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.FriendRequestBaseInfo rcvData = RecieveList[i];
            string cancleTime = TransformTime(rcvData.ullRequestTime.ToString(), (int)TimeMsg.Remove);
            if (cancleTime == null)
            {
                // 삭제요청 
                //NetworkClient.instance.SendPMsgFriendRequestInvalidC(rcvData.ullRoleId);
                slotGo.SetActive(false);
                return;
            }

            UILabel name = slotGo.transform.FindChild("profile/name").GetComponent<UILabel>();
            name.text = string.Format("{0}", rcvData.szName);
            UILabel lv = slotGo.transform.FindChild("profile/level").GetComponent<UILabel>();
            lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);

            UILabel elapsed_time = slotGo.transform.FindChild("lastConnect/label").GetComponent<UILabel>();
            elapsed_time.text = TransformTime(rcvData.ullRequestTime.ToString(), (int)TimeMsg.Recieve);


            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.unRoleType);
            UISprite Faces = slotGo.transform.FindChild("profile/Icon").GetComponent<UISprite>();
            Faces.spriteName = charLowData.PortraitId;

            //친구요청 수락/ 거절
            UIButton uiBtnAccept = slotGo.transform.FindChild("BtnAccept").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnAccept.onClick, delegate () {
                //친구수 50명이상인데 친구허용하면 팝업
                if (MyfriendList.Count >= _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.maxFriend))
                {
                    uiMgr.AddPopup(141, 240, 117);
                    return;
                }
                SceneManager.instance.ShowNetProcess("ApplyFriend");
                NetworkClient.instance.SendPMsgFriendApplicantC(rcvData.ullRoleId, 1);

            });

            UIButton uiBtnRefus = slotGo.transform.FindChild("BtnDelete").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnRefus.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("ApplyFriend");
                NetworkClient.instance.SendPMsgFriendApplicantC(rcvData.ullRoleId, 2);
            });
        }

        RcvFriendGrid.Reposition();
        RcvFreinddScrollView.enabled = RecieveList.Count >= 4 ? true : false;

        TabAlram[1].SetActive(SceneManager.instance.IsAlram(AlramIconType.SOCIAL));
        RcvFreindAlram.SetActive(SceneManager.instance.IsAlram(AlramIconType.SOCIAL));
    }

    // 친구검색 프리펩생성 
    public void GetSearchList(List<NetData.FriendBaseInfo> List)
    {
        SearchFriendList = List;

        for (int i = 0; i < SearchFriendGrid.transform.childCount; i++)
        {
            if (i >= List.Count)
            {
                SearchFriendGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = SearchFriendGrid.transform.GetChild(i).gameObject;
            Transform sltTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.FriendBaseInfo rcvData = SearchFriendList[i];
            UILabel name = slotGo.transform.FindChild("profile/name").GetComponent<UILabel>();
            name.text = string.Format("{0}", rcvData.szName);
            UILabel lv = slotGo.transform.FindChild("profile/level").GetComponent<UILabel>();
            lv.text = string.Format(_LowDataMgr.instance.GetStringCommon(453), rcvData.nLevel);

            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.nLookId);
            UISprite Faces = slotGo.transform.FindChild("profile/Icon").GetComponent<UISprite>();
            Faces.spriteName = charLowData.PortraitId;

            // 친구요청       
            UIButton uiBtnSend = slotGo.transform.FindChild("BtnSend").GetComponent<UIButton>();
            EventDelegate.Set(uiBtnSend.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("AddFriend");
                NetworkClient.instance.SendPMsgFriendAddC(rcvData.ullRoleId);
            });

            uiBtnSend.isEnabled = true;

            // 친구추가했거나 이미 요청보냈던 상대면 요청버튼 비활성화 시켜주셈용
            for (int j = 0; j < MyfriendList.Count; j++)
            {
                if (MyfriendList[j].ullRoleId == rcvData.ullRoleId)
                {
                    uiBtnSend.isEnabled = false;
                    break;
                }
            }

            if (SendList == null)
                continue;
            for (int k = 0; k < SendList.Count; k++)
            {
                if (SendList[k].ullRoleId == rcvData.ullRoleId)
                {
                    uiBtnSend.isEnabled = false;
                    break;
                }
            }
        }

        SearchFriendGrid.Reposition();
        SearchFreidnScrollView.enabled = SearchFriendList.Count >= 4 ? true : false;
    }



    //  <summary> 친구삭제하고 다시 갱신되는 친구리스트 </summary> 
    public void CancleFriend(ulong Id)
    {
        for (int i = 0; i < MyfriendList.Count; i++)
        {
            if (Id == MyfriendList[i].ullRoleId)
            {
                MyfriendList.RemoveAt(i);
                break;
            }
        }
        
        myFirndSlotCnt = 0;
        myFriendTotalCnt = MyfriendList.Count;
        FriendView(MyfriendList);
        //GetFriendList(MyfriendList);
    }

    /// <summary> 채팅연결 </summary>
    void OnClickChat(ulong uuId, string name)
    {
        // 일단 UI띄우는것만하기 
        RecomandFrined.SetActive(false);
        SearchFriendResult.SetActive(false);
        UILabel chatLabel = ChatView.transform.FindChild("title").GetComponent<UILabel>();
        chatLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(306), name);
        ChatView.SetActive(true);

        WhisperTargetId = (long)uuId;
        WhisperTargetName = name;

        ChatList.ClearHistory();
        TempCoroutine.instance.FrameDelay(0.1f, () =>
        {
            List<string> list = null;
            if (TalkHistory.TryGetValue(name, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ChatList.Add(list[i]);
                }

                //ChatList.gameObject.SetActive(false);
                //ChatList.gameObject.SetActive(true);
            }
            else
                ChatList.Clear();
        });
    }

    /// <summary> 하트보내기 응답    /// </summary>
    public void GiveHeart(ulong Id, ulong time)
    {
        for (int i = 0; i < MyfriendList.Count; i++)
        {
            if (MyfriendList[i].ullRoleId == Id)
            {
                MyfriendList[i].ullSendPowerTime = time;
            }
        }
        //  GetFriendList(MyfriendList);
        myFirndSlotCnt = 0;
        myFriendTotalCnt = MyfriendList.Count;
        FriendView(MyfriendList);
    }

    // 보낸요청에 수락/거절 했을때 갱신
    public void FriendApplicant(ulong id, uint flag)
    {
        // flag : 1 -> 수락 , 요청리스트에서 삭제되고 친구리스트에 추가
        // falg : 2 -> 거절 , 요청리스트 에서만 제거

        for (int i = 0; i < RecieveList.Count; i++)
        {
            if (id == RecieveList[i].ullRoleId)
            {
                RecieveList.RemoveAt(i);
                break;
            }
        }

        rcvFriendSlotCnt = 0;
        rcvFriendTotalCnt = RecieveList.Count;

        bool active = RecieveList.Count > 0 ? true : false;
        SceneManager.instance.SetAlram(AlramIconType.SOCIAL, active);

        RecieveView();

      

        //GetRecieveList(RecieveList);
    }

    //// 신청후에 갱신되는 추천친구/요청보낸 친구 리스트들
    public void AddFriendView(NetData.FriendRequestBaseInfo info)
    {
        // 신청보낸 슬롯을 찾아서 지워줌
        //if (!SearchFriendResult.activeSelf)
        if(RecomandFrined.activeSelf)
        {
            for (int i = 0; i < RecommendList.Count; i++)
            {
                if (info.ullRoleId == RecommendList[i].ullRoleId)
                {
                    RecommendList.RemoveAt(i);
                    break;
                }
            }

            GetRecommendList(RecommendList);
        }

        SendList.Add(info);


        // 친구검색해서 추가 한 경우
        if (SearchFriendResult.activeSelf)
        {
            GetSearchList(SearchFriendList);
        }

        //친구리스트뷰라면 넘어감
        if (!FriendViewObj[2].gameObject.activeSelf)
            return;

        sendFriendSlotCnt = 0;
        sendFriendTotalCnt = SendList.Count;
        todaySendCount = 0;
        SendView();
        //GetSendList(SendList);
    }

    // 요청취소후 갱신되는 요청보낸목록
    public void FriendCancle(ulong id)
    {
        // 취소된 슬롯을 찾아서 목록에서 지워줌
        for (int i = 0; i < SendList.Count; i++)
        {
            if (id == SendList[i].ullRoleId)
            {
                SendList.RemoveAt(i);
            }
        }
        sendFriendSlotCnt = 0;
        sendFriendTotalCnt = SendList.Count;

        todaySendCount = 0;
        SendView();
        //GetSendList(SendList);
    }

    /// <summary> 시간변환  </summary>
    string TransformTime(string time, int type)
    {
        string msg = "";


        if (time == "0" && type == (int)TimeMsg.Heart)
            return null;
        // 시간
        System.DateTime Now = System.DateTime.Now;    //현재시간

        //System.DateTime ReqTime = new System.DateTime();  //경과시간?
        //System.DateTime HeartTime = new System.DateTime();    //하트 쿨타임시간 (하루)
        //System.DateTime CancleTime = new System.DateTime();   //친구유효기간 (3일)

        int year = int.Parse(time.Substring(0, 4));
        int month = int.Parse(time.Substring(4, 2));
        int day = int.Parse(time.Substring(6, 2));
        int hour = 0;
        int min = 0;
        int sec = 0;

        if (time.Length > 9) // yyyymmddhhmmss 
        {
            hour = int.Parse(time.Substring(8, 2));
            min =  int.Parse(time.Substring(10, 2));
            sec =  int.Parse(time.Substring(12, 2));
        }

        System.DateTime reqTime;
        if (time.Length > 9)
            reqTime = new System.DateTime(year, month, day, hour, min, sec);
        else
            reqTime = new System.DateTime(year, month, day);
        
        switch (type)
        {
            case (int)TimeMsg.Recieve:
            case (int)TimeMsg.Send:
                System.TimeSpan ts = Now - reqTime;    // 빼서 계산해줌
                //요청온시간
                //msg = string.Format(_LowDataMgr.instance.GetStringCommon(258), (int)ts.Days, (int)ts.Hours, (int)ts.Minutes);
                //요청보낸시간
                msg = string.Format(_LowDataMgr.instance.GetStringCommon(258), (int)ts.Days, (int)ts.Hours, (int)ts.Minutes);
                break;

            case (int)TimeMsg.Heart:
                System.DateTime heartTime = reqTime.AddDays(1); //체력
                System.TimeSpan ts_heart = heartTime - Now;    //하트쿨타임은 24시간기준 ( 만약 1:40분에 하나, 23:50분에 하나 똑같이 24시가되면 갱신됨)
                if (ts_heart.Minutes < 0)// 하루넘김 
                    return null;
                
                //하트 초기화 시간
                if ((int)ts_heart.Hours >= 1)//1시간 이상이면 시간만출력
                    msg = string.Format(_LowDataMgr.instance.GetStringCommon(259), (int)ts_heart.Hours);
                else //아니면 분만출력
                    msg = string.Format(_LowDataMgr.instance.GetStringCommon(260), (int)ts_heart.Minutes);
                
                break;
            case (int)TimeMsg.Remove:
                System.DateTime CancleTime = new System.DateTime(year, month, day, hour, min, sec);
                if (time.Length > 9 && type == (int)TimeMsg.Remove)// 친구 요청 유효기간3일
                    CancleTime = CancleTime.AddDays(3);//CancleTime = new DateTime(year, month, day + 3, hour, min, sec); // 친구 요청 유효기간3일

                System.TimeSpan ts_Cancle = CancleTime - Now;  //친구유효기간은 3일 (요청일로부터 3일 지났을시 삭제요청들어감)
                if (ts_Cancle.Days < 0)//3일지남 
                    return null;
                break;
            default:
                break;
        }

        return msg;
    }

    /// <summary> 친구검색 팝업창    /// </summary>
    public void SetSearchFriendPopup(List<NetData.FriendBaseInfo> SearchInfo)
    {
        if (SearchInfo.Count == 0)
        {
            uiMgr.AddPopup(141, 270, 117);
            return;
        }

        RecomandFrined.SetActive(false);
        SearchFriendResult.SetActive(true);

        SearchFriendList = SearchInfo;
        GetSearchList(SearchFriendList);

    }
    /*
    public void UserCharInfo(uint charIdx, NetData._ItemData[] equipList, NetData._CostumeData costume, bool hideCostume)
    {

        friendinfo.SetActive(true);

        CurCostumStatus.text = costume.GetLocName();
        
        uint helmet = 0, cloth = 0, weapon = 0;
        for (int i = 0; i < EquipPartsTf.childCount; i++)
        {
            Transform tf = EquipPartsTf.GetChild(i);
            if (tf == null)
                continue;

            UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
            UISprite bg = tf.GetComponent<UISprite>();
            UISprite grade = tf.FindChild("grade").GetComponent<UISprite>();
            UISprite icon = tf.FindChild("icon").GetComponent<UISprite>();
            UILabel enchantLv = tf.FindChild("Num").GetComponent<UILabel>();

            if (equipList[i] == null)
            {
                icon.enabled = false;
                grade.enabled = false;
                bg.spriteName = UIHelper.GetDefaultEquipIcon((ePartType)i + 1);
                enchantLv.gameObject.SetActive(false);

                continue;
            }

            NetData._ItemData equipData = equipList[i];

            if (equipData.EquipPartType == ePartType.HELMET)
                helmet = equipData._equipitemDataIndex;
            else if (equipData.EquipPartType == ePartType.WEAPON)
                weapon = equipData._equipitemDataIndex;
            else if (equipData.EquipPartType == ePartType.CLOTH)
                cloth = equipData._equipitemDataIndex;

            icon.enabled = true;
            grade.enabled = true;

            UIAtlas atlas = null;
            atlas = AtlasMgr.instance.GetEquipAtlasForClassId(equipData.GetEquipLowData().Class);

            icon.atlas = atlas;
            icon.spriteName = _LowDataMgr.instance.GetLowDataIcon(equipData.GetEquipLowData().Icon);
            grade.spriteName = string.Format("Icon_0{0}", equipData.GetEquipLowData().Grade);
            bg.spriteName = string.Format("Icon_bg_0{0}", equipData.GetEquipLowData().Grade);
            enchantLv.text = string.Format("+{0}", equipData._enchant);
            enchantLv.gameObject.SetActive(true);

            EventDelegate.Set(uiTri.onClick, delegate () {
                if (equipData == null)
                    return;
                UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
                if (basePanel == null)
                    return;

                SetItemDetailPopup(equipData);
            });
        }

        //코스튬 화면
        UIHelper.CreatePcUIModel("CharacterPanel", RotationTargetList[0].transform, (uint)CharIdx, helmet, costume._costmeDataIndex
            , cloth, weapon, 3, hideCostume, false);

        friendinfo.SetActive(true);

    }
    */
    /// <summary> ---
    /// 장착아이템 클릭시 호출되는 함수
    /// 아이템 슬롯 클릭했을때와 비슷
    /// </summary>
    /// <param name="partType">ePartType의 인덱스 값</param>
    //void OnClickEquipItem(ePartType type)
    //{
    //    NetData._ItemData itemData = FriendFullInfoData.GetEquipParts(type);
    //    if (itemData == null)//들어오면 문제가 있는 것.
    //    {
    //        return;
    //    }

    //    SetItemDetailPopup(itemData);
    //}
    /// <summary>
    /// 아이템 디테일 팝업 실행 함수---
    /// </summary>
    void SetItemDetailPopup(NetData._ItemData itemData)
    {
        UIMgr.OpenDetailPopup(this, itemData, 8);
    }

    /// <summary> 채팅 받는 함수 </summary>
    public void OnReciveChat(string msg, bool isMe)
    {
        if (isMe)
            InputChat.value = "";

        ChatList.Add(msg);
    }


    public void NotifyAddFriend(NetData.FriendRequestBaseInfo Info)
    {
        // 친구 신청 받았을때 실시간으로 받은요청에 추가해줌
        // 지금 친구창이 열려있다면 받은요청슬롯에 추가해준다.
        if (!FriendViewObj[(int)ViewType.Recieve].activeSelf)
            return;

        RecieveList.Add(Info);

        rcvFriendSlotCnt = 0;
        rcvFriendTotalCnt = RecieveList.Count;
        RecieveView();
        //GetRecieveList(RecieveList);
    }
    public void NotifyCancleFriend(ulong Id)
    {
        // 친구신청한걸 취소 했을경우 실시간으로 받은요청에서 지워준다
        // 지금 친구창이 열리있다면 받은요청 슬롯에서 삭제

        if (!FriendViewObj[(int)ViewType.Recieve].activeSelf)
            return;


        for (int i = 0; i < RecieveList.Count; i++)
        {
            if (Id == RecieveList[i].ullRoleId)
            {
                RecieveList.RemoveAt(i);
                break;
            }
        }


        rcvFriendSlotCnt = 0;
        rcvFriendTotalCnt = RecieveList.Count;

        RecieveView();
       // GetRecieveList(RecieveList);
    }
    public void NotifyResultFriend(ulong id, uint flag)
    {
        // 지금 친구 창이 열려있을때 
        // 신청자에게 처리 결과 통지해줌

        if (flag == 1)  // 수락
        {
            // 현재 RecieveView면 안해줘도됨
            if (FriendViewObj[(int)ViewType.Recieve].activeSelf)
                return;

            // 수락했으면 요청슬롯에서 삭제해주고 내친구리스트에 추가해줌
            for (int i = 0; i < SendList.Count; i++)
            {
                if (id == SendList[i].ullRoleId)
                {
                    NetData.FriendBaseInfo tmp = new NetData.FriendBaseInfo(SendList[i].ullRoleId, SendList[i].szName, SendList[i].nLevel, SendList[i].unRoleType, 0, 0, 0, false);
                    MyfriendList.Add(tmp);
                    SendList.RemoveAt(i);

                    break;
                }
            }

            // GetFriendList(MyfriendList);
            myFirndSlotCnt = 0;
            myFriendTotalCnt = MyfriendList.Count;
            FriendView(MyfriendList);

            sendFriendSlotCnt = 0;
            todaySendCount = 0;
            sendFriendTotalCnt = SendList.Count;
            SendView();
            //GetSendList(SendList);
        }
        else if (flag == 2)   // 거절
        {

            // 현재 RecieveView면 안해줘도됨
            if (FriendViewObj[(int)ViewType.Recieve].activeSelf)
                return;

            // 거절했다면 요청슬롯에서만 삭제
            for (int i = 0; i < SendList.Count; i++)
            {
                if (id == SendList[i].ullRoleId)
                {
                    SendList.RemoveAt(i);
                    break;
                }
            }

            sendFriendSlotCnt = 0;
            todaySendCount = 0;
            sendFriendTotalCnt = SendList.Count;

            SendView();
            //GetSendList(SendList);

        }

    }
    //유효기간(3일)지난것들에 대한 삭제되는 받은요청리스트 
    public void InvalidCancleRecieveFriend(ulong id)
    {
        for (int i = 0; i < RecieveList.Count; i++)
        {
            if (id == RecieveList[i].ullRoleId)
            {
                RecieveList.RemoveAt(i);
                break;
            }
        }

        sendFriendSlotCnt = 0;
        todaySendCount = 0;
        sendFriendTotalCnt = SendList.Count;

        SendView();

        //GetRecieveList(RecieveList);
    }
    //유효기간(3일)지난것들에 대한 삭제되는 친구요청리스트 
    public void InvalidCancleSendFriend(ulong id)
    {
        for (int i = 0; i < SendList.Count; i++)
        {
            if (id == SendList[i].ullRoleId)
            {
                SendList.RemoveAt(i);
                break;
            }
        }

        sendFriendSlotCnt = 0;

        todaySendCount = 0;
        sendFriendTotalCnt = SendList.Count;

        SendView();

       // GetSendList(SendList);
    }
    #endregion


    #endregion

    #region 메일

    ///<summary> 전체 메일리스트 받아옴 </summary> 
    public void GetMailList(List<NetData.EmailBaseInfo> emailInfoList)
    {
        // 최대 10개까지만 받아올수있다 == 여러번 호출된다는 소리 
        if (EMailDataList.Count == 0)
        {
            EMailDataList = emailInfoList;
            slotCnt = 0;
            totalCnt = 0;
        }
        else
        {
            EMailDataList.InsertRange(EMailDataList.Count, emailInfoList);
            slotCnt += emailInfoList.Count;

        }

        if (EMailDataList == null)
        {
            return;
        }

        if (ViewObj[0].activeSelf)
        {
            totalCnt += emailInfoList.Count;
         //   if (emailInfoList.Count != 10)//여러번 받아오니까 10개가 아닌 경우에만 호출한다.
                MailView();
        }
    }

    void MailView()
    {
        //여러번호출 되기떄문에누적
        MailDetailTf.gameObject.SetActive(false);
        DetailArr = -1;

        //BtnMailGetAll.isEnabled = true;
        //BtnMailDelAll.isEnabled = true;
        
        MailPaging.RefreshSlot(totalCnt);
        
        MailNullLabel.SetActive(EMailDataList.Count == 0 ? true : false);
        MailCount.text = string.Format("[ {0} / {1} ]", EMailDataList.Count, _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MailMaxCount));

        TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));

    }

    void OnMailSlot(int arr, GameObject slotGo)
    {
        if (arr >= totalCnt)
        {
            slotGo.SetActive(false);
            return;
        }
        
        slotGo.SetActive(true);
        Transform slotTf = slotGo.transform;
        UIButton Get = slotTf.FindChild("BtnOk").GetComponent<UIButton>();
        UIButton Del = slotTf.FindChild("BtnDell").GetComponent<UIButton>();
        
        if (0 <= DetailArr)
        {
            if (arr < DetailArr-5 || DetailArr+5 < arr)
            {
                if (MailDetailTf.gameObject.activeSelf)
                    MailDetailTf.gameObject.SetActive(false);
            }
            else
            {
                Vector3 pos = slotTf.localPosition;
                if (arr < DetailArr)
                {
                    pos.y = arr * MailGrid.cellHeight;
                }
                else if (arr == DetailArr)
                {
                    MailDetailTf.gameObject.SetActive(true);
                    pos.y = arr * MailGrid.cellHeight;

                    MailDetailTf.parent = slotTf;
                    MailDetailTf.localPosition = new Vector3(0, -140, 0);
                }
                else if (DetailArr < arr)
                {
                    pos.y = 160 + (arr * MailGrid.cellHeight);
                }

                slotTf.localPosition = -pos;
            }
        }
        else//이렇게해도 될지는 모르겠지만 일단 이렇게라도 해놓는다.
        {
            Vector3 pos = slotTf.localPosition;
            pos.y = arr * MailGrid.cellHeight;
            slotTf.localPosition = -pos;
        }
        
        int idx = arr;
        EventDelegate.Set(Get.onClick, delegate ()
        {
            NetworkClient.instance.PMsgEmailFeatchC(EMailDataList[idx].MaidId);
        });
        EventDelegate.Set(Del.onClick, delegate ()
        {
            uiMgr.AddPopup(141, 307, 117, 76, 0, delegate ()
            {
                SceneManager.instance.ShowNetProcess("DelMail");
                NetworkClient.instance.PMsgEmailDelC(EMailDataList[idx].MaidId);
            });
        });

        NetData.EmailBaseInfo info;
        info = EMailDataList[arr];

        UISprite defaultIcon = slotTf.FindChild("IconBg/Icon").GetComponent<UISprite>();
        UILabel title = slotTf.FindChild("Txt_title").GetComponent<UILabel>();
        UILabel content = slotTf.FindChild("Txt_info").GetComponent<UILabel>();
        UILabel expireDate = slotTf.FindChild("Txt_Time").GetComponent<UILabel>();

        // 열람/미열람/수령 체크
        if (info.IsReceive == 0) //미열람상태
        {
            defaultIcon.spriteName = "Icon_MailClosed";
            expireDate.text = TimeTransform(info.OverTime);
            Del.isEnabled = false;
            Get.isEnabled = true;
        }
        else if (info.IsReceive == 1) //열람상태
        {
            //defaultIcon.spriteName = "Icon_MailClosed";
            //expireDate.text = TimeTransform(info.OverTime);
            //Del.isEnabled = false;
            //Get.isEnabled = true;
        }
        else if (info.IsReceive == 2) //열람후 수령상태
        {
            Del.isEnabled = true;
            Get.isEnabled = false;
            defaultIcon.spriteName = "Icon_MailOpen";
            expireDate.text = _LowDataMgr.instance.GetStringCommon(145);  //이거는 무러바야함
        }

        Mail.MailInfo mailTable = _LowDataMgr.instance.GetLowDataMail((uint)info.MailType);

        title.text = mailTable.NameId;
        content.text = mailTable.DescriptionId;

        UIEventTrigger tri = slotGo.GetComponent<UIEventTrigger>();
        EventDelegate.Set(tri.onClick, delegate ()
        {
            mailInfo = mailTable;

            //아이템상세보기
            MailDetailTf.FindChild("Btn_0/Txt_title").GetComponent<UILabel>().text = title.text;
            MailDetailTf.FindChild("Btn_0/Txt_Info").GetComponent<UILabel>().text = content.text;
            DetailArr = idx;
            NetworkClient.instance.PMsgEmailReadDetailC(info.MaidId);    // 메일상세정보 보기
        });
    }

    //<summary> 메일받기했을때 리스트갱신용(일단 다시) </summary>
    public void GetMailItem(List<uint> getMailList)
    {
        for (int i = 0; i < EMailDataList.Count; i++)
        {
            for (int j = 0; j < getMailList.Count; j++)
            {
                if (EMailDataList[i].MaidId == getMailList[j])
                    EMailDataList[i].IsReceive = 2;    //잠시강제로..
            }
        }


        if (TabAlram[0].activeSelf)
        {
            bool isAlram = false;
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (!isAlram && EMailDataList[i].IsReceive == 0)//안읽은
                    isAlram = true;
            }
            SceneManager.instance.SetAlram(AlramIconType._SOCIAL, isAlram);
            TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));
        }

        slotCnt = 0;
        totalCnt = EMailDataList.Count;
        MailView();
    }

    /// <summary> 메일 일괄수령후 리스트랭신</summary>
    public void GetAllMailItem(List<uint> getMailList, List<NetData.EmailAttachmentInfo> mailItem)
    {
        for (int i = 0; i < EMailDataList.Count; i++)
        {
            for (int j = 0; j < getMailList.Count; j++)
            {
                if (EMailDataList[i].MaidId == getMailList[j])
                    EMailDataList[i].IsReceive = 2;    //잠시강제로..
            }
        }



        if (TabAlram[0].activeSelf)
        {
            bool isAlram = false;
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (!isAlram && EMailDataList[i].IsReceive == 0)//안읽은
                    isAlram = true;
            }
            SceneManager.instance.SetAlram(AlramIconType._SOCIAL, isAlram);
            TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));
        }


        slotCnt = 0;
        totalCnt = EMailDataList.Count;
        MailView();
    }



    ///<summary> 상세메일정보 </summary> 
    public void GetDetailMailData(NetData.EamilDetails emDetail)
    {
        for (int i = 0; i < EMailDataList.Count; i++)
        {
            if (EMailDataList[i].MaidId == emDetail.MailId && EMailDataList[i].IsReceive !=2)
                EMailDataList[i].IsReceive = 1;    //잠시강제로..
        }
        CurEMailData = emDetail;
        
        if (TabAlram[0].activeSelf)
        {
            bool isAlram = false;
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (!isAlram && EMailDataList[i].IsReceive == 0)//안읽은
                    isAlram = true;
            }
            SceneManager.instance.SetAlram(AlramIconType._SOCIAL, isAlram);
            TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));
        }
        
        MailDetailTf.gameObject.SetActive(true);
        
        ContentItemScrollView.ResetPosition();
        ContentItemScrollView.enabled = CurEMailData.emAttach.Count > 5 ? true : false;

        //// 상세정보를 볼때는 일괄수령/삭제 버튼 비활성화 해줌       
        //BtnMailGetAll.isEnabled = false;
        //BtnMailDelAll.isEnabled = false;
        
        MailPaging.RefreshSlot(totalCnt);

        int count = ContentItemGrid.transform.childCount;

        //서버에서 아이템을 주고있다면?
        if (emDetail.emAttach.Count > 0)
        {
            //아이템이 있다면 
            for (int i = 0; i < count; i++)
            {
                if (CurEMailData.emAttach.Count <= i)
                {
                    ContentItemGrid.transform.GetChild(i).gameObject.SetActive(false);
                    continue;
                }
                SetItemSlot(ContentItemGrid.transform.GetChild(i).gameObject, CurEMailData.emAttach[i].GoodType, CurEMailData.emAttach[i].Id,CurEMailData.emAttach[i].Count);
               
                ContentItemGrid.transform.GetChild(i).gameObject.SetActive(true);

            }
            ContentItemGrid.Reposition();
            return;
        }
        
        // 테이블에서 값받아오면 ? (체력)
        List<MailItemData> data = new List<MailItemData>();

        for (int i = 0; i < EMailDataList.Count; i++)
        {
            if (emDetail.MailId != EMailDataList[i].MaidId)
                continue;

            Mail.MailInfo info = _LowDataMgr.instance.GetLowDataMail((uint)EMailDataList[i].MailType);
            for (int j = 0; j < mailInfo.itemType.Count; j++)
            {
                //지급아이템타입이 0이면 없음
                if (info.itemType[j].ToString() == "0")
                    continue;
                // 개수가 0개면 넘어가
                if (info.value[j].ToString() == "0")
                    continue;

                MailItemData mail = new MailItemData(info.itemType[j], info.itemLink[j], info.value[j]);
                data.Add(mail);
            }

        }
        
        for (int i = 0; i < count; i++)
        {
            if (data.Count <= i)
            {
                ContentItemGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            // 타입이 있지만 링크가 0 인경우 == 아이템없는겨우
            if (data[i].itmeType != "0" && data[i].itemLink == "0")
                continue;
            
            SetItemSlot(ContentItemGrid.transform.GetChild(i).gameObject, uint.Parse(data[i].itmeType), uint.Parse(data[i].itemLink),uint.Parse(data[i].itmeCnt));
            ContentItemGrid.transform.GetChild(i).gameObject.SetActive(true);

        }

        ContentItemGrid.Reposition();
    }



    /// <summary> 메일함 삭제하고 나서 수행하는 액션 및 삭제</summary>
    public void ActionDeleteMailData(List<int> getMailList)
    {
        SoundManager.instance.PlaySfxSound(eUISfx.UI_mail_delete, false);
        StartCoroutine("ActionDeleteMail", getMailList);
    }

    IEnumerator ActionDeleteMail(List<int> getMailList)
    {
        IsDuringDelete = true;
        
        int RepositionCnt = getMailList.Count;

        //전체삭제
        if (getMailList.Count == 0)
        {
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                getMailList.Add((int)EMailDataList[i].MaidId);
            }
        }

        int getMailCount = getMailList.Count;

        for (int i = 0; i < getMailCount; i++)
        {
            int getMailIdx = getMailList[i];

            int ownMailCount = EMailDataList.Count;
            for (int j = 0; j < ownMailCount; j++)
            {
                NetData.EmailBaseInfo data = EMailDataList[j];
                if (data.MaidId != getMailIdx)
                    continue;

                EMailDataList.Remove(data);
                
                slotCnt = 0;
                totalCnt = EMailDataList.Count;
                MailView();
                yield return new WaitForSeconds(0.15f);
                break;
            }
        }

        IsDuringDelete = false;
        getMailList.Clear();

        bool isAlram = false;
        for(int i=0;i<EMailDataList.Count;i++)
        {
            if (!isAlram && EMailDataList[i].IsReceive == 0)//안읽은
                isAlram = true;
        }
        SceneManager.instance.SetAlram(AlramIconType._SOCIAL, isAlram);
        TabAlram[0].SetActive(SceneManager.instance.IsAlram(AlramIconType._SOCIAL));

     

        // 스크롤뷰가 밖으로 나가는경우가있음 
        if (RepositionCnt > 1 && EMailDataList.Count > 0)
        {
            // 위치랑 오프셋을 전부 0로 바꿔줌
            ContentItemScrollView.transform.localPosition = Vector3.zero;
            //objPaging.NowCreate(EMailDataList.Count);
            ContentItemScrollView.transform.GetComponent<UIPanel>().clipOffset = Vector2.zero;

        }

        yield return null;
    }


    //메일상세보기에서 아이템 아이콘 
    void SetItemSlot(GameObject itemImg, uint type, uint id, uint cnt)
    {
        UILabel CntLabel = itemImg.transform.FindChild("cnt").GetComponent<UILabel>();
        CntLabel.text = cnt.ToString();
        CntLabel.gameObject.SetActive(cnt > 1);
        
        string iconName = "";
        int idx = 0;
        switch ((Sw.UNITE_TYPE)type)
        {
            case Sw.UNITE_TYPE.UNITE_TYPE_COIN:
                iconName = "money";
                idx = 599000;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_GEM:
                iconName = "cash";
                idx = 599001;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_CONTRIBUTION:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_HONOR:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROYAL_BADGE:
                iconName = "badge_A";
                idx = 599002;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_LION_KING_BADGE:

                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_FAME:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_POWER:
                iconName = "stamina";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROLE_EXP:
                iconName = "Img_ap";   //일단임시로넣어둠
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_TITLE:    //임시로 honor를쓰도록함
                iconName = "badge";
                break;
            default:
                break;
        }

        if (iconName == "")
        {
            SetRewardItem(id, itemImg.GetComponent<UISprite>());
        }
        else
        {
            itemImg.transform.FindChild("grade").GetComponent<UISprite>().spriteName = "Icon_01";
            UISprite bg = itemImg.transform.FindChild("bgGrade").GetComponent<UISprite>();
            //if (bg.atlas != BodAltas)
            bg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);
            bg.spriteName = "Bod_IconBg";//"Icon_bg_01";

            itemImg.GetComponent<UISprite>().atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Img);
            itemImg.GetComponent<UISprite>().spriteName = iconName;
            itemImg.GetComponent<BoxCollider>().enabled = idx!=0? true:false;

            if (idx != 0)
            {
                EventDelegate.Set(itemImg.GetComponent<UIEventTrigger>().onClick, delegate ()
                {
                    NetData._ItemData itemData = new NetData._ItemData((uint)idx, (uint)idx, 0, 0, false);
                    SetItemDetailPopup(itemData);
                });
            }


        }

    }
    // 보상 아이템정보 
    void SetRewardItem(uint itemIdx, UISprite itemImg)
    {
        UIEventTrigger tri = itemImg.transform.GetComponent<UIEventTrigger>();

        itemImg.GetComponent<BoxCollider>().enabled = true;

        Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx);
        if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
        {
            itemImg.atlas = AtlasMgr.instance.GetEquipAtlasForClassId(eLowData.Class);

            itemImg.transform.FindChild("grade").GetComponent<UISprite>().spriteName = string.Format("Icon_0{0}", eLowData.Grade);
            UISprite bg = itemImg.transform.FindChild("bgGrade").GetComponent<UISprite>();
            bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);

            bg.spriteName = string.Format("Icon_bg_0{0}", eLowData.Grade);

            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
            EventDelegate.Set(tri.onClick, delegate ()
            {
                NetData._ItemData itemData = new NetData._ItemData(itemIdx, itemIdx, 0, 0, false);
                SetItemDetailPopup(itemData);
            });
        }
        else//소모아이템이 드랍아이템 대표로 등록되어 있음
        {
            Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(itemIdx);
            UISprite bg;
            if (uLowData == null)
            {
                // 초상화일수있으니 다시검ㅅ ㅏ
                if (_LowDataMgr.instance.IsGetRewardType(10, itemIdx))
                {
                    itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);

                    itemImg.spriteName = _LowDataMgr.instance.GetPartnerInfo(itemIdx).PortraitId;
                    itemImg.GetComponent<BoxCollider>().enabled = false;
                    itemImg.transform.FindChild("grade").GetComponent<UISprite>().spriteName = string.Format("Icon_0{0}", _LowDataMgr.instance.GetPartnerInfo(itemIdx).Quality);
                    bg = itemImg.transform.FindChild("bgGrade").GetComponent<UISprite>();
                    //if (bg.atlas != EquipAtlas)
                    bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);
                    bg.spriteName = string.Format("Icon_bg_0{0}", _LowDataMgr.instance.GetPartnerInfo(itemIdx).Quality);

                }
                return;
            }

            if (uLowData.Type == (byte)ItemType.Costum || uLowData.Type == (byte)ItemType.Partner)
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
            else
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.UseItem);

            itemImg.transform.FindChild("grade").GetComponent<UISprite>().spriteName = string.Format("Icon_0{0}", uLowData.Grade);
            bg = itemImg.transform.FindChild("bgGrade").GetComponent<UISprite>();

            if (uLowData.Type == (int)AssetType.Jewel)
            {
                bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);
                bg.spriteName = string.Format("Icon_bg_0{0}", uLowData.Grade);
            }
            else
            {
                bg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);
                bg.spriteName = "Bod_IconBg";
            }

            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);

            EventDelegate.Set(tri.onClick, delegate ()
            {
                NetData._ItemData itemData = new NetData._ItemData(itemIdx, itemIdx, 0, 0, false);
                SetItemDetailPopup(itemData);
            });
        }

    }
    
    /// <summary> 시간변환 </summary>
    string TimeTransform(ulong time)
    {
        // 2016 12 29 12 38 29   <-이런식으로옴 쪼개서 보여주기
        string msg;

        string day = time.ToString().Substring(6, 2);
        string hour = time.ToString().Substring(8, 2);
        string min = time.ToString().Substring(10, 2);

        msg = string.Format(_LowDataMgr.instance.GetStringCommon(295), day, hour, min);

        return msg;
    }

    #endregion


    public override void Hide()
    {
        base.Hide();

        // 받은친구요청이 있다면 알림뜨라
        //SceneManager.instance.SetAlram(AlramIconType.SOCIAL, RecieveList.Count > 0 ? true : false);
        NetworkClient.instance.SendPMsgFriendRequestFriendListC();

        bool mailAlram = false;
        if (EMailDataList.Count > 0)
        {
            for (int i = 0; i < EMailDataList.Count; i++)
            {
                if (EMailDataList[i].IsReceive == 0)
                {
                    mailAlram = true;
                    break;
                }
            }
        }
        SceneManager.instance.SetAlram(AlramIconType._SOCIAL, mailAlram);

        CameraManager.instance.mainCamera.gameObject.SetActive(true);

        if (BasePanel != null)
            BasePanel.Show(BasePanel.GetParams());
        else
            UIMgr.OpenTown();
    }
}

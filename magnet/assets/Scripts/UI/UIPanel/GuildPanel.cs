using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GuildPanel : UIBasePanel
{
    enum eTAB_VIEW_TYPE
    {
        NONE = -1,
        INFO = 0,   //길드정보
        MEMBER,    //길드구성원
        ADMIN,      //길드관리
    }
    private eTAB_VIEW_TYPE CurViewType = eTAB_VIEW_TYPE.NONE;   //현재 보고있는 뷰

    //길드 정보창에서 쓰일 라벨들...
    enum eGuildInfoLabel
    {
        eGuildName = 0,
        eMasterName,
        eMemberCount,
        eAnnounce,
        eNotice,
        eContri,    //공헌도
        eCreateTime,//생성일
        eAttak, //전투력
        eMoney, //보유자금
    }

    public GameObject[] ViewObj;   //탭에따라 변할 오브젝트들
    public UITabGroup TabGroup;//탭 리스트 제어해줄 스크립트

    public UIButton[] BtnGuildInfoChange;    //이름,선언,공지수정버튼
    public GameObject GuildNameChangePopup; //이름변경팝업


    public UIInput DeclareInput;//선언인풋
    public UIInput NoticeInput;//공지인풋
    public UIInput NameInput;   //이름변경인풋
    public UILabel NameErrorLabel;  //이름변경의에러는여기에 적어줌


    public UIButton BtnOut;//탈퇴

    //길드마크팝업관련
    public GameObject GuildIconPopUp; //아이콘 변경
    public Transform MarkTf;
    private const int MarkStartId = 40000;
    private uint IconID = 0;

    public GameObject Alram;    //신청자잇을때알림

    private uint MyGuildId;

    public UIEventTrigger BtnMark;  //길드마크버튼

    public GameObject AdminListEmpty;
    public UIScrollView MemberScroll;
    public UIScrollView AdminSroll;
    public UIGrid MemberGrid;   //멤버그리드
    public UIGrid AdminGrid;    //관리그리드

    public UILabel GuildJoinLevel;  //길드가입레벨
    public UIEventTrigger BtnMinus;
    public UIEventTrigger BtnPlus;

    public UIButton BtnFreeJoin;//자유가입
    public UIButton BtnJudgeJoin;   //심사가입

    public UIEventTrigger BtnApply;   //길드가입레벨, 자유/심사가입 적용버튼


    public GameObject MemberAdminPopup; //멤버관리팝업
    //길드정보들
    private NetData.GuildSimpleInfo MyGuildSimpleInfo;
    private NetData.GuildDetailedInfo MyGuildDetailInfo;
    private NetData.GuildSelfInfo MyInfo;

    //길드 메인에표시될 라벨들
    public UILabel[] GuildInfoLabels;

    private List<NetData.GuildMemberInfo> MyGuildMemberList;
    private List<NetData.ApplyRoleInfo> ApplyList;  //대기신청자들

    private ObjectPaging MarkPaging;
    bool isFreeJoinSet;
    public override void Init()
    {
        base.Init();

        MyGuildId = NetData.instance.GetUserInfo()._GuildId;
        TabGroup.Initialize(OnClickTab);

        for (int i = 0; i < 100; i++)
        {
            GameObject slotGo = Instantiate(MemberGrid.transform.GetChild(0).gameObject) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = MemberGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        //Destroy(MemberGrid.transform.GetChild(0));
        MemberGrid.Reposition();

        for (int i = 0; i < 100; i++)
        {
            GameObject slotGo = Instantiate(AdminGrid.transform.GetChild(0).gameObject) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = AdminGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        //Destroy(AdminGrid.transform.GetChild(0));
        AdminGrid.Reposition();


        EventDelegate.Set(BtnMark.onClick, MarkChange);
        EventDelegate.Set(GuildIconPopUp.transform.FindChild("Markselect/BtnClose").GetComponent<UIButton>().onClick,
            delegate ()
            {
                GuildIconPopUp.SetActive(false);
                IconID = 0;
                for (int i = 0; i < MarkTf.childCount; i++)
                {
                    GameObject sel = MarkTf.GetChild(i).FindChild("select").gameObject;
                    sel.SetActive(false);
                }
            });

        EventDelegate.Set(GuildIconPopUp.transform.FindChild("Markchange/BtnClose").GetComponent<UIButton>().onClick,
           delegate ()
           {
               GuildIconPopUp.transform.FindChild("Markselect").gameObject.SetActive(true);
               GuildIconPopUp.transform.FindChild("Markchange").gameObject.SetActive(false);

           });

        EventDelegate.Set(GuildNameChangePopup.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick,
         delegate ()
         {

             NameErrorLabel.text = "";
             NameInput.value = "";
             NameInput.label.text = "";

             GuildNameChangePopup.SetActive(false);

         });
        EventDelegate.Set(GuildNameChangePopup.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick,
           delegate ()
           {
               GuildNameChangePopup.SetActive(false);
           });

        for (int i=0;i< BtnGuildInfoChange.Length;i++)
        {
            int idx = i;
            EventDelegate.Set(BtnGuildInfoChange[idx].onClick, delegate ()
            {
                ChangeInfo((byte)idx);
            });
        }

        EventDelegate.Set(BtnOut.onClick, delegate ()
        {
            //팝업임시임 등록해줘야함
            uiMgr.AddPopup(141, 1067, 117, 76, 0, delegate () { NetworkClient.instance.SendPMsgGuildSecedeGuildC(MyGuildId); }, null, null);
        });

        EventDelegate.Set(BtnPlus.onClick, delegate () { OnclickUseCount(1); });
        EventDelegate.Set(BtnMinus.onClick, delegate () { OnclickUseCount(-1); });

        EventDelegate.Set(BtnFreeJoin.onClick, delegate () {
            isFreeJoinSet = true;
            BtnFreeJoin.isEnabled = !isFreeJoinSet;
            BtnJudgeJoin.isEnabled = isFreeJoinSet;
        });
        EventDelegate.Set(BtnJudgeJoin.onClick, delegate () {
            isFreeJoinSet = false;
            BtnFreeJoin.isEnabled = !isFreeJoinSet;
            BtnJudgeJoin.isEnabled = isFreeJoinSet;
        });

        EventDelegate.Set(BtnApply.onClick, delegate ()
        {
            //가입레벨

            //심사or자유가입
            int isJoin = isFreeJoinSet ? 2 : 1;
            NetworkClient.instance.SendPMsgSetGuildJoinsetC(MyGuildId, (uint)isJoin );
            NetworkClient.instance.SendPMsgGuildSetRoleLevelForJoinGuildC(MyGuildId, (uint)GuildJoinLv);

            
        });

        //??
        EventDelegate.Set(DeclareInput.onSubmit, delegate ()
        {
            NetworkClient.instance.SendPMsgGuildChangeNameDeclarationAnnouncementC(MyGuildId, 2, DeclareInput.value);
        });
        EventDelegate.Set(NoticeInput.onSubmit, delegate ()
        {
            NetworkClient.instance.SendPMsgGuildChangeNameDeclarationAnnouncementC(MyGuildId, 3, NoticeInput.value);
        });

        //길드마크변경 원보
        GuildIconPopUp.transform.FindChild("Markselect/BtnCreate/num").GetComponent<UILabel>().text = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildMarkChangeCost).ToString();

        //길드이름변경 원보
        GuildNameChangePopup.transform.FindChild("BtnCreate/num").GetComponent<UILabel>().text = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildNameChangeCost).ToString();
    }


    public override void LateInit()
    {
        base.LateInit();
        // 길드기본정보/상세정보를 미리받아놈
        NetworkClient.instance.SendPMsgGuildQueryBaseInfoC(MyGuildId);
        NetworkClient.instance.SendPMsgGuildQueryDetailedInfoC(MyGuildId);
        NetworkClient.instance.SendPMsgGuildQuerySelfInfoC(MyGuildId);

        // 아이콘 
        GameObject markPrefab = MarkTf.GetChild(0).gameObject;
        MarkPaging =  ObjectPaging.CreatePagingPanel(MarkTf.parent.gameObject, MarkTf.gameObject, markPrefab
          , 5, 32, 32, 10, OnCallBackMark);//가라 522개

        Destroy(markPrefab);

        DeclareInput.gameObject.SetActive(false);
        NoticeInput.gameObject.SetActive(false);
        NameInput.gameObject.SetActive(false);

        Alram.SetActive(SceneManager.instance.IsAlram((AlramIconType.GUILD)));
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

	public override void Hide()
	{
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
		
		base.Hide();
		UIMgr.OpenTown();
	}

    void OnCallBackMark(int arr, GameObject go)
    {
        if (522 <= arr)
        {
            go.SetActive(false);
            return;
        }

        go.SetActive(true);

        string iconName = _LowDataMgr.instance.GetLowDataIcon((uint)(MarkStartId + arr));
        if (string.IsNullOrEmpty(iconName) && !iconName.Contains("guild"))
        {
            go.SetActive(false);
            return;
        }

        //착용중도체크해줘여함
        go.transform.FindChild("label").gameObject.SetActive(MarkStartId + arr == MyGuildSimpleInfo.Icon);

        go.GetComponent<UISprite>().spriteName = iconName;
        EventDelegate.Set(go.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            if (go.transform.FindChild("label").gameObject.activeSelf)
                return;

            IconID = (uint)(MarkStartId + arr);
            // Debug.Log(IconID);
            GameObject select = go.transform.FindChild("select").gameObject;
            for (int i = 0; i < MarkTf.childCount; i++)
            {
                GameObject sel = MarkTf.GetChild(i).FindChild("select").gameObject;
                sel.SetActive(false);
            }
            if (select.activeSelf)
                select.SetActive(false);
            else
                select.SetActive(true);
        });
    }

    //길드기본정보
    public void SetGuildInfo(List<NetData.GuildSimpleInfo> SimpleInfoList)
    {
        MyGuildSimpleInfo = SimpleInfoList[0];

        GuildInfoLabels[(int)eGuildInfoLabel.eGuildName].text = MyGuildSimpleInfo.Name;
        GuildInfoLabels[(int)eGuildInfoLabel.eMasterName].text = MyGuildSimpleInfo.LeaderName;
        GuildInfoLabels[(int)eGuildInfoLabel.eMemberCount].text = string.Format("{0}/{1}", MyGuildSimpleInfo.Count.ToString(), _LowDataMgr.instance.GetLowdataGuildInfo(MyGuildSimpleInfo.guildLv).GuildJoinLimit);
        GuildInfoLabels[(int)eGuildInfoLabel.eCreateTime].text = string.Format(_LowDataMgr.instance.GetStringCommon(577), MyGuildSimpleInfo.CreateTime.ToString().Substring(0,4), MyGuildSimpleInfo.CreateTime.ToString().Substring(4, 2), MyGuildSimpleInfo.CreateTime.ToString().Substring(6, 2));
        GuildInfoLabels[(int)eGuildInfoLabel.eAttak].text = MyGuildSimpleInfo.Attack.ToString();

        //아이콘
        BtnMark.GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(MyGuildSimpleInfo.Icon);
        MarkPaging.NowCreate(32);

        isFreeJoinSet = MyGuildSimpleInfo.JoinSet == 2;
        GuildJoinLv = (int)MyGuildSimpleInfo.JoinLevel;
    }
    //길드 자세한정보..
    public void SetGuildDetailInfo(NetData.GuildDetailedInfo guildInfo)
    {
        MyGuildDetailInfo = guildInfo;

        string dec = MyGuildDetailInfo.Declaration;
        _LowDataMgr.instance.IsBanString(ref dec);

        GuildInfoLabels[(int)eGuildInfoLabel.eAnnounce].text = dec;//선언

        string noi = MyGuildDetailInfo.Announce;
        _LowDataMgr.instance.IsBanString(ref noi);

        GuildInfoLabels[(int)eGuildInfoLabel.eNotice].text = noi;//공고
        GuildInfoLabels[(int)eGuildInfoLabel.eMoney].text = MyGuildDetailInfo.Bank.ToString();//돈
    }
    //나의정보..
    public void SetGuildMyInfo(NetData.GuildSelfInfo myInfo)
    {
        MyInfo = myInfo;
        GuildInfoLabels[(int)eGuildInfoLabel.eContri].text = MyInfo.ContriTotal.ToString();//공헌도

        //관리탭
        //이렇게맞는지 물어봐야함..
        TabGroup.transform.FindChild("GuildAdmin").gameObject.SetActive(_LowDataMgr.instance.GetLowdataGuildPositionInfo(myInfo.Position).UserAccept == 1);
    }


    //마크변경
    public void MarkChange()
    {
        //권한쳌
        if (_LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).GuildiconChange == 0)  //0이false
        {
            UIMgr.instance.AddPopup(141, 623, 117);
            return;
        }

        for (int i = 0; i < MarkTf.childCount; i++)
        {
            GameObject sel = MarkTf.GetChild(i).FindChild("select").gameObject;
            sel.SetActive(false);
        }

        IconID = 0;

        GuildIconPopUp.SetActive(true);
        GuildIconPopUp.transform.FindChild("Markselect").gameObject.SetActive(true);
        GuildIconPopUp.transform.FindChild("Markchange").gameObject.SetActive(false);

        //변경시 필요원보체크후 활성화처리, 서버처리후 적용하자
        UIEventTrigger changeBtn = GuildIconPopUp.transform.FindChild("Markselect/BtnCreate").GetComponent<UIEventTrigger>();
        EventDelegate.Set(changeBtn.onClick, delegate ()
        {
            if (IconID == 0)
            {
                //에러팝업
                UIMgr.instance.AddPopup(141, 239, 117);
                return;
            }


            GuildIconPopUp.transform.FindChild("Markselect").gameObject.SetActive(false);
            GuildIconPopUp.transform.FindChild("Markchange").gameObject.SetActive(true);

            //현재적용중
            GuildIconPopUp.transform.FindChild("Markchange/Mark1").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(MyGuildSimpleInfo.Icon);
            //변경될아이콘
            GuildIconPopUp.transform.FindChild("Markchange/Mark2").GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(IconID);

            GuildIconPopUp.transform.FindChild("Markchange/Txt_cash").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1141), _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildMarkChangeCost));//원보가격정해지면수정해라
            GuildIconPopUp.transform.FindChild("Markchange/Txt_lb").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1142);

            EventDelegate.Set(GuildIconPopUp.transform.FindChild("Markchange/BtnCreate").GetComponent<UIEventTrigger>().onClick, delegate ()
            {
             
                if (IconID < MarkStartId)
                    return;
                NetworkClient.instance.SendPMsgGuildChangeIconC(MyGuildId, IconID);
            });

        });
    }

    //마크변경후
    public void RefreshIcon(uint icon)
    {
        GuildIconPopUp.SetActive(false);
        MyGuildSimpleInfo.Icon = icon;

        BtnMark.GetComponent<UISprite>().spriteName = _LowDataMgr.instance.GetLowDataIcon(icon);
        IconID = 0;

        MarkPaging.NowCreate(32);
    }

    //길드이름,선언 공지등..변경버튼
    public void ChangeInfo(byte type)
    {
        //type = 0  길드이름
        //type = 1  길드선언
        //type = 2  길드공지

        byte[] right = { _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).GuildNameChange,
                         _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).GuildWriteChange,
                         _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).Guildnotice  };

        //DeclareInput.gameObject.SetActive(false);
        //NoticeInput.gameObject.SetActive(false);
        //NameInput.gameObject.SetActive(false);

        string dec = MyGuildDetailInfo.Declaration;        _LowDataMgr.instance.IsBanString(ref dec);        GuildInfoLabels[(int)eGuildInfoLabel.eAnnounce].text = dec;//선언
        string noi = MyGuildDetailInfo.Announce;        _LowDataMgr.instance.IsBanString(ref noi);        GuildInfoLabels[(int)eGuildInfoLabel.eNotice].text = noi;//공고

        //권한쳌
        if (right[type] == 0)  //0이false
        {
            UIMgr.instance.AddPopup(141, 623, 117);
            return;
        }

        //길드이름만 팝업으로처리해줌
        if (type == 0)
        {
            NameInput.gameObject.SetActive(true);

            NameInput.value = "";
            NameInput.text = "";
            NameErrorLabel.text = "";
            GuildNameChangePopup.SetActive(true);



            UIEventTrigger changeName = GuildNameChangePopup.transform.FindChild("BtnCreate").GetComponent<UIEventTrigger>();
            EventDelegate.Set(changeName.onClick, ChangeGuildName);
        }
        else
        {
            if (type == 1)
            {
                DeclareInput  .gameObject.SetActive(true);
            }
            else if (type == 2)
                NoticeInput.gameObject.SetActive(true);


        }
    }

    void ChangeGuildName()
    {
        string msg = "";
        msg = NameInput.value;
        if (_LowDataMgr.instance.IsBanString(msg))
        {
            NameErrorLabel.text = _LowDataMgr.instance.GetStringCommon(898);
            return;
        }
        if (string.IsNullOrEmpty(msg))
        {
            NameErrorLabel.text = _LowDataMgr.instance.GetStringCommon(611);
            return;
        }
        if (msg == MyGuildSimpleInfo.Name)
        {
            //전에이름과같으면리턴
            NameErrorLabel.text = _LowDataMgr.instance.GetStringCommon(1216);
            return;
        }

        _LowDataMgr.instance.IsBanString(ref msg);

        string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(662), _LowDataMgr.instance.GetStringCommon(229));
        UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
            , delegate () { NetworkClient.instance.SendPMsgGuildChangeNameDeclarationAnnouncementC(MyGuildId, 1, msg); }, null, null);
    }
    void ChangeDeclareAndAnnounce(byte type)
    {
        if(type==2)
        {
            //선언

        }
        else
        {
            //공고
        }
        
    }

    public void SetGuildNameDeclareAnoount(bool success, uint error, string value)
    {
        if(success)
        {
            if (GuildNameChangePopup.activeSelf)
            {
                NameErrorLabel.text = "";
                NameInput.value = "";
                NameInput.label.text = "";

                GuildNameChangePopup.SetActive(false);

                string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1164), _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildNameChangeCost), value);
                uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117),null, null
                   , null, null, null);
            }
        }
        else
        {
            if (GuildNameChangePopup.activeSelf)
            {
                NameErrorLabel.text = _LowDataMgr.instance.GetLowDataErrorString(error);
            }
            else
            {
                UIMgr.instance.AddErrorPopup((int)error);
            }
        }


    }

    private int GuildJoinLv = 0;
    //길드가입레벨 조정
    void OnclickUseCount(int add)
    {
        //길드최소레벨, 캐릭터최대레벨사이에만 조정가능
        int lv = GuildJoinLv + add;

        if (lv < _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.GuildCondition))
            return;
        if (lv > _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MaxLevel))
            return;

        GuildJoinLevel.text = lv.ToString();
        GuildJoinLv = lv;
    }



    //길드관리뷰
    public void SetAddminView(uint a, List<NetData.ApplyRoleInfo> applyList)
    {
        isFreeJoinSet = MyGuildSimpleInfo.JoinSet == 2;    //1이자유가입 아마
        BtnFreeJoin.isEnabled = !isFreeJoinSet;
        BtnJudgeJoin.isEnabled = isFreeJoinSet;

        GuildJoinLv = (int)MyGuildSimpleInfo.JoinLevel;
        GuildJoinLevel.text = GuildJoinLv.ToString();//GuildJoinLv.ToString();

        ApplyList = applyList;
        AdminListEmpty.SetActive(applyList.Count == 0);
        for(int i=0;i<AdminGrid.transform.childCount;i++)
        {
            if (i >= ApplyList.Count)
            {
                AdminGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = AdminGrid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.ApplyRoleInfo rcvData = ApplyList[i];

            UILabel name = slotTf.FindChild("name").GetComponent<UILabel>();
            name.text = rcvData.Name;

            UILabel lv = slotTf.FindChild("level").GetComponent<UILabel>();
            lv.text = rcvData.Lv.ToString();

            UILabel Attack = slotTf.FindChild("strength").GetComponent<UILabel>();
            Attack.text = rcvData.Power.ToString();

            UISprite Faces = slotGo.transform.FindChild("Face").GetComponent<UISprite>();
            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.Type);
            Faces.spriteName = charLowData.PortraitId;

            //승인
            UIEventTrigger ok = slotGo.transform.FindChild("btn_ok").GetComponent<UIEventTrigger>();
            EventDelegate.Set(ok.onClick, delegate ()
            {
                NetworkClient.instance.SendPMsgGuildExamineApplicantC(MyGuildId, rcvData.Id, 1);

            });
            //거절
            UIEventTrigger no = slotGo.transform.FindChild("btn_cancel").GetComponent<UIEventTrigger>();
            EventDelegate.Set(no.onClick, delegate ()
            {
                NetworkClient.instance.SendPMsgGuildExamineApplicantC(MyGuildId, rcvData.Id, 2);
            });
        }
        AdminGrid.Reposition();
        AdminSroll.enabled = ApplyList.Count > 4;

        Alram.SetActive(SceneManager.instance.IsAlram((AlramIconType.GUILD)));

    }

    //길드 가입신청 승인/거절후
    public void MemberCountChange(uint acc)
    {
        //길드멤버뷰 재갱신해줌
        NetworkClient.instance.SendPMsgGuildQueryApplyListC(MyGuildId);

    }
    //길드멤버뷰
    public void SetGuildMemberList(List<NetData.GuildMemberInfo> memberList)
    {
        MyGuildMemberList = memberList;

        //접속중인길드원 -> 직책 순으로 소팅해줌
        MyGuildMemberList.Sort(SortGuildMemberList);

        for (int i = 0; i < MemberGrid.transform.childCount; i++)
        {
            if (i >= MyGuildMemberList.Count)
            {
                MemberGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = MemberGrid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;
            slotGo.SetActive(true);

            NetData.GuildMemberInfo rcvData = MyGuildMemberList[i];

            UILabel name = slotTf.FindChild("name").GetComponent<UILabel>();
            name.text = rcvData.Name;

            UISprite Faces = slotGo.transform.FindChild("Face").GetComponent<UISprite>();
            Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(rcvData.Type);
            Faces.spriteName = charLowData.PortraitId;

            UILabel lv = slotTf.FindChild("level").GetComponent<UILabel>();
            lv.text = rcvData.Lv.ToString();

            UILabel Attack = slotTf.FindChild("strength").GetComponent<UILabel>();
            Attack.text = rcvData.Power.ToString();

            UILabel position = slotTf.FindChild("rank").GetComponent<UILabel>();
            Guild.GuildPositionInfo myPosition = _LowDataMgr.instance.GetLowdataGuildPositionInfo(rcvData.position);
            position.text = _LowDataMgr.instance.GetStringCommon(myPosition.name);

            UILabel contri = slotTf.FindChild("point").GetComponent<UILabel>();
            contri.text = rcvData.Contribution.ToString();

            UILabel access = slotTf.FindChild("access").GetComponent<UILabel>();
            if (rcvData.Online == 1/*rcvData.LogountTime < rcvData.LoginTime*/)//0오프 1온라인
            {
                //온라인
                access.text = _LowDataMgr.instance.GetStringCommon(1116);
            }
            else
            {
                //오프
                //이거좀더보기
                if (rcvData.LogountTime != 0)
                {
                    System.DateTime logout = System.DateTime.ParseExact(rcvData.LogountTime.ToString(), "yyyyMMddHHmmss", null);
                    System.TimeSpan lastAccess = System.DateTime.Now - logout;

                    // 하루지남
                    if (lastAccess.Days > 0)
                    {
                        access.text = string.Format(_LowDataMgr.instance.GetStringCommon(1119), lastAccess.Days);
                    }
                    else if (lastAccess.Hours > 0)
                    {
                        //24시간미만
                        access.text = string.Format(_LowDataMgr.instance.GetStringCommon(1118), lastAccess.Hours);
                    }
                    else
                    {
                        //60분미만
                        access.text = string.Format(_LowDataMgr.instance.GetStringCommon(1117), lastAccess.Minutes);
                    }
                    
                }
                else
                {
                    //일단임시로....
                    access.text = string.Format(_LowDataMgr.instance.GetStringCommon(1117), 1);

                }

            }

            UIButton Admin = slotTf.FindChild("admin").GetComponent<UIButton>();
            Admin.isEnabled = rcvData.Id != NetData.instance.GetUserInfo()._charUUID;
            //관리탭
            EventDelegate.Set(Admin.onClick, delegate ()
            {
                MyFriendList.Clear();
                MySendList.Clear();
                MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>().isEnabled = true;
                NetworkClient.instance.SendPMsgFriendQueryListC();
                selectMember = rcvData;
                SetMemberPopup(rcvData);
            });

        }
        MemberGrid.Reposition();
        MemberScroll.enabled = MyGuildMemberList.Count > 4;

    }

    int SortGuildMemberPosition(NetData.GuildMemberInfo a, NetData.GuildMemberInfo b)
    {
        if (a.position < b.position)
            return -1;
        return 1;
    }

    int SortGuildMemberList(NetData.GuildMemberInfo a, NetData.GuildMemberInfo b)
    {
        if (a.Online > b.Online)
            return -1;
        else if (b.Online > a.Online)
            return -1;

        return SortGuildMemberPosition(a, b);
        //// 직책(낮은게더높은직책)
        //if (a.position < b.position)
        //    return -1;
        //else if (b.position < a.position)
        //    return 1;

        //return 0;
    }
    private NetData.GuildMemberInfo selectMember;
    //멤퍼관리 팝업
    void SetMemberPopup(NetData.GuildMemberInfo member)
    {
        MemberAdminPopup.SetActive(true);

        EventDelegate.Set(MemberAdminPopup.transform.FindChild("forg").GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            MemberAdminPopup.SetActive(false);
        });

        MemberAdminPopup.transform.FindChild("name").GetComponent<UILabel>().text = member.Name;

        //길마양도
        UIButton master = MemberAdminPopup.transform.FindChild("Btn02").GetComponent<UIButton>();
        master.isEnabled = _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).MasterEntrust == 1;
        EventDelegate.Set(master.onClick, delegate ()
         {
             string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1121),member.Name);
             uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                 , () => {
                     NetworkClient.instance.SendPMsgGuildAppointGuildLeaderC(MyGuildId, member.Id);
                 }, null, null);
         });
        //부길마
        UIButton subMaster = MemberAdminPopup.transform.FindChild("Btn03").GetComponent<UIButton>();
        subMaster.isEnabled = _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).SubMasterEntrust == 1;
        if (member.position == 1)   //길마는안댄다
            subMaster.isEnabled = false;
        EventDelegate.Set(subMaster.onClick, delegate ()
        {
            string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1123),member.Name);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                , () => {
                    NetworkClient.instance.SendPMsgGuildAppointPositionC(MyGuildId, member.Id, 2);
                }, null, null);

        });
        //정예{
        UIButton elite = MemberAdminPopup.transform.FindChild("Btn04").GetComponent<UIButton>();
        elite.isEnabled = _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).EliteMember == 1;
        if (member.position == 1)   //길마는안댄다
            elite.isEnabled = false;
        EventDelegate.Set(elite.onClick, delegate ()
        {

            string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1156), member.Name);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                , () => {
                    NetworkClient.instance.SendPMsgGuildAppointPositionC(MyGuildId, member.Id, 3);
                }, null, null);
        });
        //길원
        UIButton normal = MemberAdminPopup.transform.FindChild("Btn05").GetComponent<UIButton>();
        normal.isEnabled = _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).NormalMember == 1;
        if (member.position == 1)   //길마는안댄다
            normal.isEnabled = false;
        EventDelegate.Set(normal.onClick, delegate ()
        {

            string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1125), member.Name);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                , () => {
                    NetworkClient.instance.SendPMsgGuildAppointPositionC(MyGuildId, member.Id, 4);
                }, null, null);
        });

        //추방
        UIButton outMember = MemberAdminPopup.transform.FindChild("Btn06").GetComponent<UIButton>();
        outMember.isEnabled = _LowDataMgr.instance.GetLowdataGuildPositionInfo(MyInfo.Position).MemberLeave == 1;
        if (member.position == 1)   //길마는안댄다
            outMember.isEnabled = false;
        EventDelegate.Set(outMember.onClick, delegate ()
        {
            string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(1130), member.Name);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                , () => {
                    NetworkClient.instance.SendPMsgGuildKitkMemberC(MyGuildId, member.Id);
                }, null, null);
        });

    }
    public void PositionChange(bool sucess, uint ErrorCode, uint position)
    {
        //성공or실패메시지?입력후
        //새로 갱신

        MemberAdminPopup.SetActive(false);

        if (sucess)
        {
            string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1122), selectMember.Name); //길마
            if (position == 2)//부길마
            {
                msg = string.Format(_LowDataMgr.instance.GetStringCommon(1124), selectMember.Name);//부길마
            }
            else if (position == 3)//정예
            {
                msg = string.Format(_LowDataMgr.instance.GetStringCommon(1157), selectMember.Name);
            }
            else if (position == 4)//일반
            {
                msg = string.Format(_LowDataMgr.instance.GetStringCommon(1126), selectMember.Name);
            }
            else if(position == 5)//추방
            {
                msg = string.Format(_LowDataMgr.instance.GetStringCommon(1131), selectMember.Name);
            }

            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), msg, _LowDataMgr.instance.GetStringCommon(117), null, null
               , () =>
               {
                   if(position==1)
                   {
                       base.Close();
                       UIMgr.OpenGuildPanel();
                   }
                  else
                   {
                       NetworkClient.instance.SendPMsgGuildMemberListC(MyGuildId);

                   }
               }, null, null);
            }

        else
        {
            if (ErrorCode == 1347)
                UIMgr.instance.AddPopup(141, 1132, 117);    //길드장은추방할수없습니다
            else
                UIMgr.instance.AddErrorPopup((int)ErrorCode);
        }
    }

    //멤버관리탭에서 친구신청을해야할경우..
    private List<NetData.FriendBaseInfo> MyFriendList = new List<NetData.FriendBaseInfo>();
    public void CheckMyFrinedList(List<NetData.FriendBaseInfo> FriendList)
    {
        //친구추가
        UIButton friend = MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>();
        EventDelegate.Set(friend.onClick, delegate ()
        {
            NetworkClient.instance.SendPMsgFriendAddC(selectMember.Id);
        });

        // 여러번올수있기때문에 쳌  
        if (MyFriendList.Count == 0)
        {
            MyFriendList = FriendList;
        }
        else
        {
            MyFriendList.InsertRange(MyFriendList.Count, FriendList);
        }

        if (MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>().isEnabled == true)
        {
            bool isRequest = false;

            for (int i = 0; i < MyFriendList.Count; i++)
            {
                if (MyFriendList[i].ullRoleId != selectMember.Id)
                    continue;

                isRequest = true;
            }

            if (!isRequest)
                NetworkClient.instance.SendPMsgFriendSelfRequestFriendListC();
            else
            {
                MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>().isEnabled = false;
                return;
            }
        }

  
    }

    //보낸친구도체크
    private List<NetData.FriendRequestBaseInfo> MySendList = new List<NetData.FriendRequestBaseInfo>();
    public void CheckSendFriendList(List<NetData.FriendRequestBaseInfo> sendList)
    {
        MySendList = sendList;

        // 여러번올수있기때문에 쳌  
        if (MySendList.Count == 0)
        {
            MySendList = sendList;
        }
        else
        {
            MySendList.InsertRange(MySendList.Count, sendList);
        }

        if (MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>().isEnabled == true)
        {
            for (int i = 0; i < MySendList.Count; i++)
            {
               if(MySendList[i].ullRoleId == selectMember.Id)
                {
                    MemberAdminPopup.transform.FindChild("Btn01").GetComponent<UIButton>().isEnabled = false;
                    return;
                }
            }

        }
    }

    //성공시에 보여줌
    public void SuccessFriendAdd()
    {
        MemberAdminPopup.SetActive(false);
        UIMgr.instance.AddPopup(141, 1120, 117);
    }

    //가입레벨변경
    public void SetGuildJoinLevel(uint level)
    {
        GuildJoinLv = (int)level;
        GuildJoinLevel.text = level.ToString();
    }

    // 길드 속성 타입 
    enum GUILD_ATTRIBUTE_TYPE
    {
        GUILD_ATTRIBUTE_TYPE_ID = 1,                    // 길드 id 帮派ID
        GUILD_ATTRIBUTE_TYPE_LEADER_ID = 2,          // 길마 id 帮主id
        GUILD_ATTRIBUTE_TYPE_LEADER_NAME = 3,           // 길마 닉네임 帮主昵称
        GUILD_ATTRIBUTE_TYPE_ICON = 4,                  // 길드 아이콘 帮派图标
        GUILD_ATTRIBUTE_TYPE_NAME = 5,                  // 길드 이름 帮派名称
        GUILD_ATTRIBUTE_TYPE_DECLARATION = 6,               // 길드 선언 帮派宣言
        GUILD_ATTRIBUTE_TYPE_ANNOUNCEMENT = 7,            // 길드 공고 帮派公告
        GUILD_ATTRIBUTE_TYPE_JOIN_SET = 8,              // 길드 자동 가입 1:아무나 허용, 2:심사가입 自动加入帮派 1允许任何人加入;2审核加入
        GUILD_ATTRIBUTE_TYPE_BANKROLL = 9,              // 길드 자금 帮派资金
        GUILD_ATTRIBUTE_TYPE_CREATE_TIME = 10,          // 설림 시간 创建时间
        GUILD_ATTRIBUTE_TYPE_GUILD_LEVEL = 11,          // 길드 로비 레빌 帮派大厅等级
        GUILD_ATTRIBUTE_TYPE_GUILD_BLESS_LEVEL = 12,    // 길드 축원 레벨 帮会祈福等级
        GUILD_ATTRIBUTE_TYPE_GUILD_SHOP_LEVEL = 13,     // 길드 상점 레벨 帮会商店等级		
        GUILD_ATTRIBUTE_TYPE_GUILD_JOIN_LEVEL_MIN = 14,     // 길드 가입레벨
    }
    public void AttributeUpdate(uint type, ulong value, string str)
    {
        GUILD_ATTRIBUTE_TYPE AttriType = (GUILD_ATTRIBUTE_TYPE)type;

        switch (AttriType)
        {
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_ID:
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_LEADER_ID:
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_LEADER_NAME:
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_ICON:
                MyGuildSimpleInfo.Icon = (uint)value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_NAME:
                MyGuildSimpleInfo.Name = str;
                GuildInfoLabels[(int)eGuildInfoLabel.eGuildName].text = MyGuildSimpleInfo.Name;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_DECLARATION:
                MyGuildDetailInfo.Declaration = str;

                string dec = MyGuildDetailInfo.Declaration;
                _LowDataMgr.instance.IsBanString(ref dec);

                GuildInfoLabels[(int)eGuildInfoLabel.eAnnounce].text = dec;//선언
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_ANNOUNCEMENT:
                MyGuildDetailInfo.Announce = str;

                string noi = MyGuildDetailInfo.Announce;
                _LowDataMgr.instance.IsBanString(ref noi);

                GuildInfoLabels[(int)eGuildInfoLabel.eNotice].text = noi;//공고
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_JOIN_SET:
                MyGuildSimpleInfo.JoinSet = (uint)value;
                //    ToggleFreeJoin.value= MyGuildSimpleInfo.JoinSet == 1 ? true : false;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_BANKROLL:
                MyGuildDetailInfo.Bank = value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_CREATE_TIME:
                MyGuildSimpleInfo.CreateTime = value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_GUILD_LEVEL:
                MyGuildDetailInfo.Lv = (uint)value;
                MyGuildSimpleInfo.guildLv = (uint)value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_GUILD_BLESS_LEVEL:
                MyGuildDetailInfo.BlessLv = (uint)value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_GUILD_SHOP_LEVEL:
                MyGuildDetailInfo.ShopLv = (uint)value;
                break;
            case GUILD_ATTRIBUTE_TYPE.GUILD_ATTRIBUTE_TYPE_GUILD_JOIN_LEVEL_MIN:
                MyGuildSimpleInfo.JoinLevel = (uint)value;
                GuildJoinLv = (int)value;
                break;
            default:
                break;
        }
    }

    void OnClickTab(int viewType)
    {
        ChangeView((eTAB_VIEW_TYPE)viewType);
    }
    /// <summary> 탭 버튼에 따라 보고있는 뷰를 바꿔준다  </summary>
    void ChangeView(eTAB_VIEW_TYPE type)
    {
        if (CurViewType == type)//동일하므로 무시
            return;

        for (int i = 0; i < ViewObj.Length; i++)
        {
            bool active = false;
            if (i == (uint)type)
                active = true;

            ViewObj[i].SetActive(active);
        }

        // 관리탭에서 왔을때 설정변경버튼을 안누를경우 팝업으로경고?
        if (CurViewType == eTAB_VIEW_TYPE.ADMIN)
        {
            bool freejoinSet = MyGuildSimpleInfo.JoinSet == 2;

            if (GuildJoinLv != MyGuildSimpleInfo.JoinLevel)
            {
                uiMgr.AddPopup(141, 1201, 117, 76, 0, delegate () { 
                    int isJoin = isFreeJoinSet ? 2 : 1;
                    NetworkClient.instance.SendPMsgSetGuildJoinsetC(MyGuildId, (uint)isJoin);
                    NetworkClient.instance.SendPMsgGuildSetRoleLevelForJoinGuildC(MyGuildId, (uint)GuildJoinLv);
                }, null, null);
            }
            else if(freejoinSet !=isFreeJoinSet)
            {
                uiMgr.AddPopup(141, 1201, 117, 76, 0, delegate () { 
                    int isJoin = isFreeJoinSet ? 2 : 1;
                    NetworkClient.instance.SendPMsgSetGuildJoinsetC(MyGuildId, (uint)isJoin);
                    NetworkClient.instance.SendPMsgGuildSetRoleLevelForJoinGuildC(MyGuildId, (uint)GuildJoinLv);
                }, null, null);
            }

        }


        CurViewType = type;

        switch (type)
        {
            case eTAB_VIEW_TYPE.INFO:
                //SetInfoView();
                break;
            case eTAB_VIEW_TYPE.MEMBER:
                NetworkClient.instance.SendPMsgGuildMemberListC(MyGuildId);
                break;
            case eTAB_VIEW_TYPE.ADMIN:
                NetworkClient.instance.SendPMsgGuildQueryApplyListC(MyGuildId);
                break;
        }

    }

    public override void Close()
    {
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
        base.Close();
        UIMgr.OpenTown();
    }

    //밑에 아직x
    public void OnPMsgRankList(int rank, int cnt, List<NetData.RankInfo> Info)
    {

    }




    public void ResetGuildUserQuest(NetData.GuildUserTaskInfo info)
    {

    }
    public void CompleteQuest(uint id, int i)
    {

    }
    public void GetUserQuestList(List<NetData.GuildUserTaskInfo> taskList)
    {

    }
    public void GetQuestList(List<NetData.GuildTaskInfo> taskList)
    {

    }
    public void GuildMemberInfo(NetData.FriendFullInfo friendFullInfo, List<NetData._ItemData> itemdataList)
    {

    }


    public void BlessResult(string name, List<uint> id, List<uint> type)
    {

    }

    public void OutMemberNotice()
    {

    }
   
    public void GoLobby(uint type, uint typeLv)
    {

    }
   

    public void OnPMsgSearchGuild(List<NetData.GuildSimpleInfo> infoList)
    {

    }
}

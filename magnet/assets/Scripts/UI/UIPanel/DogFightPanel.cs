using UnityEngine;
using System.Collections.Generic;
using System;

public class DogFightPanel : UIBasePanel
{
    //public enum eTab_View_RoomType
    //{
    //    None = -1,
    //    e20_40 = 1, //레벨20~40
    //    e40_60 = 2, //40~60
    //    e60_79 = 3, //60~79
    //    e80 = 4, //80
    //    eFree = 5,  //자유대전
    //}

    public UIButton[] BottomBtn;  //상점, 코스튬, 바로시작
    public Transform CharModelTf;//캐릭터 모델 어미
    public UILabel[] BottomLabels; // 초기화시간, 아이템, 포인트
   // public Transform TabGroup; //Tab버튼 관리하는 클래스

    // public UILabel[] MyInfo; //렙,닉넴 , 전투력
    public UILabel LvName;
    public UILabel Attack;

    public UIScrollView Scroll;
    public UIGrid Grid;
    public GameObject Slot;


    private NetData._UserInfo UserInfo;//캐릭터 인벤토리
    // 위에 적힐 아이템,포인트 
    private int MyitmeCnt;
    private int ItemMaxCnt;
    private int MyPointCnt;
    private int PointMaxCnt;
    private uint QuickRoomId;

    //private eTab_View_RoomType CurViewType = eTab_View_RoomType.None;//현재 보고있는 뷰 타입
    private List<NetData.MessInfo> RoomList; //레벨에맞는 방
    private List<NetData.MessInfo> FreeRoomList;//자유대전방리스트

    public override void Init()
    {
        base.Init();
        UserInfo = NetData.instance.GetUserInfo();
        string lvStr = string.Format(_LowDataMgr.instance.GetStringCommon(453), UserInfo._Level);
        LvName.text = string.Format("{0} {1}", lvStr, NetData.instance.Nickname);
        Attack.text = UserInfo.RefreshTotalAttackPoint().ToString();
        RoomList = null;
        
        //상점
        EventDelegate.Set(BottomBtn[0].onClick, delegate () {
            base.Close();
            UIMgr.OpenShopPanel(this);
        });
        //코스튬
        EventDelegate.Set(BottomBtn[1].onClick, delegate ()
        {
            base.Close();
            UIMgr.OpenCostume(this);
        });

        SetChannelActive();

        BottomBtn[2].isEnabled = 0 < QuickRoomId ? true : false;//최소렙 못넘으면 빠른입장안대
        EventDelegate.Set(BottomBtn[2].onClick, delegate () { EnterGo(0); });//빠른입장

        //내캐릭터
        NetData._CostumeData equipCostume = UserInfo.GetEquipCostume();
        CharModelTf.transform.gameObject.SetActive(true);

        uint weaponId = 0, clothId = 0, headId = 0;
        if (UserInfo.isHideCostum)
        {
            NetData._ItemData head = UserInfo.GetEquipParts(ePartType.HELMET);
            NetData._ItemData cloth = UserInfo.GetEquipParts(ePartType.CLOTH);
            NetData._ItemData weapon = UserInfo.GetEquipParts(ePartType.WEAPON);

            if (head != null)
                headId = head._equipitemDataIndex;

            if (cloth != null)
                clothId = cloth._equipitemDataIndex;

            if (weapon != null)
                weaponId = weapon._equipitemDataIndex;
        }
        

        UIHelper.CreatePcUIModel("FreefightPanel",CharModelTf, UserInfo.GetCharIdx(), headId, equipCostume._costmeDataIndex, clothId, weaponId, UserInfo.GetEquipSKillSet().SkillSetId, 3, UserInfo.isHideCostum, false);
        
        NetworkClient.instance.SendPMsgMessQueryC();//조회
        GetRoomList();
    }

    //레벨에따른 방정보를 받아온다
    void GetRoomList()
    {
        uint lv = NetData.instance.GetUserInfo()._Level;
        int Type = 0;
        //if (lv == 80)
        //{
        //    Type = (int)eTab_View_RoomType.e80;
        //}
        //else if (lv >= 60)   //60~79
        //{
        //    Type = (int)eTab_View_RoomType.e60_79;
        //}
        //else if (lv >= 40)   //40~59
        //{
        //    Type = (int)eTab_View_RoomType.e40_60;
        //}
        //else if (lv >= 20) //20~39
        //{
        //    Type = (int)eTab_View_RoomType.e20_40;
        //}
        List<DungeonTable.FreefightTableInfo> list = _LowDataMgr.instance.GetLowDataFreeFightList();
        for(int i=0; i < list.Count; i++)
        {
            if (list[i].MaxenterLv < lv || lv < list[i].MinenterLv)
                continue;

            Type = list[i].StageGroup;
            break;
        }

        // 내레벨의맞는방과 자대유전방정보를 받아옴
        //NetworkClient.instance.SendPMsgMessRoomQueryC(Type);//방정보
        //NetworkClient.instance.SendPMsgMessRoomQueryC(5);//자유방정보


    }


    /// <summary>
    /// 레벨에따른 채널선택
    /// </summary>
    /// <returns> 채널Number </returns>
    void SetChannelActive()
    {
        //    string.Format(_LowDataMgr.instance.GetStringCommon(543)),//자유

        QuickRoomId = 0;
        uint lv = NetData.instance.GetUserInfo()._Level;
        List<DungeonTable.FreefightTableInfo> list = _LowDataMgr.instance.GetLowDataFreeFightList();
        for (int i = 0; i < list.Count; i++)
        {
            GameObject slotGo = Instantiate(Slot) as GameObject;
            Transform slotTf = slotGo.transform;
            
            UILabel name = slotTf.FindChild("txt_roomnum").GetComponent<UILabel>();
            name.text = string.Format("{0} {1}~{2}", _LowDataMgr.instance.GetStringCommon(14), list[i].MinenterLv, list[i].MaxenterLv);//roomNme[i];

            //이벤트 트리거
            UIEventTrigger etri = slotGo.GetComponent<UIEventTrigger>();
            uint idx = list[i].StageIndex;
            EventDelegate.Set(etri.onClick, delegate ()
            {
                RoomListView(idx);
            });

            UISprite bg = slotTf.GetChild(0).GetComponent<UISprite>();
            if (lv >= list[i].MinenterLv)//Max렙 비교 
            {
                if (lv <= list[i].MaxenterLv)//입장가능
                {
                    slotTf.GetComponent<BoxCollider>().enabled = true;
                    bg.color = Color.white;

                    QuickRoomId = idx;//입장 가능 한곳 아이디 저장
                }
                else//불가
                {
                    slotTf.GetComponent<BoxCollider>().enabled = false;
                    bg.color = Color.gray;
                }
            }
            else//불가
            {
                slotTf.GetComponent<BoxCollider>().enabled = false;
                bg.color = Color.gray;
            }

            slotTf.parent = Grid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(true);
        }

        Destroy(Slot);
        Grid.Reposition();
    }

    /// <summary>
    /// 빠른입장  빈곳으로 바로 입장도와줌
    /// </summary>
    void EnterGo(uint type=0)
    {
        if(type!=0) //선택해서 온경우
        {
            if(type == 5)
            {
                //자유 
                for (int i = 0; i < FreeRoomList.Count; i++)
                {
                    NetData.MessInfo info = FreeRoomList[i];
                    if (info.RoleNum < info.RoleMaxNum) //아직 만원이 아닌곳
                    {
                        UIMgr.instance.AddPopup(141, 550, 158, 76, 0, delegate () { NetworkClient.instance.SendPMsgMessRoomEnterC(info.Id); }, null, null);
                        return;
                    }
                }
            }
            else
            {
                //
                for (int i = 0; i < RoomList.Count; i++)
                {
                    NetData.MessInfo info = RoomList[i];
                    if (info.RoleNum < info.RoleMaxNum) //아직 만원이 아닌곳
                    {
                        UIMgr.instance.AddPopup(141, 550, 158, 76, 0, delegate () { NetworkClient.instance.SendPMsgMessRoomEnterC(info.Id); }, null, null);
                        return;
                    }
                }
            }

        }
        else
        {
            // 빠른입장버튼으로 온경우 
            // 해당레벨 or 자유대전암데나 빈곳으로 감

            List<NetData.MessInfo> totalList = new List<NetData.MessInfo>();
            totalList.AddRange(RoomList);
            totalList.AddRange(FreeRoomList);

            for(int i=0;i< totalList.Count;i++)
            {
                NetData.MessInfo info = totalList[i];
                if (info.RoleNum < info.RoleMaxNum) //아직 만원이 아닌곳
                {
                    UIMgr.instance.AddPopup(141, 550, 158, 76, 0, delegate () { NetworkClient.instance.SendPMsgMessRoomEnterC(info.Id); }, null, null);
                    return;
                }
            }

        }
     
    }

    public void RoomListView(uint type)
    {
        bool CanEnter = false; //입장레벨이 정해져있으므로

        int minLv = _LowDataMgr.instance.GetLowDataDogFight(type).MinenterLv;
        int maxLv = _LowDataMgr.instance.GetLowDataDogFight(type).MaxenterLv;

        if (NetData.instance.GetUserInfo()._Level < minLv)
            CanEnter = false;
        else if (NetData.instance.GetUserInfo()._Level > maxLv)
            CanEnter = false;
        else
            CanEnter = true;
        
        if (CanEnter)
            EnterGo(type);

        else
            UIMgr.instance.AddPopup(141, 890, 117);
    }

    /// <summary>
    /// 상단에 표시될 라벨들
    /// </summary>
    /// <param name="item"> 획득한 아이템수 </param>
    /// <param name="itemMax"> 획득가능한 아이템 Max </param>
    /// <param name="point"> 획득한 포인트 </param>
    /// <param name="pointMax"> 획득가능한 포인트 Max </param>
    public void SetMyInfo(int item, int itemMax, int point, int pointMax)
    {
        MyitmeCnt = item;
        ItemMaxCnt = itemMax;
        MyPointCnt = point;
        PointMaxCnt = pointMax;

        // 획득아이템/획득가능max
        BottomLabels[1].text = string.Format("{0}/{1}", MyitmeCnt, ItemMaxCnt);
        // 획득포인트/획득가능포인트Max
        BottomLabels[2].text = string.Format("{0}/{1}", MyPointCnt, PointMaxCnt);
    }

    public void SetRoomInfo(int type, int cnt, List<NetData.MessInfo> List)
    {
        //자유방
        if(type == 5)
        {
            FreeRoomList = List;
        }
        else
        {
            RoomList = List;

        }

    }
    
    private void Update()
    {
        //초기화시간 
        DateTime Now = DateTime.Now;    //현재시간
        DateTime ResetTiem = new DateTime(Now.Year, Now.Month, Now.Day, 0, 0, 0);
        ResetTiem = ResetTiem.AddDays(1);
        TimeSpan ts = ResetTiem - Now;

        BottomLabels[0].text = string.Format("{0:00} : {1:00} : {2:00}", ts.Hours, ts.Minutes, ts.Seconds);
    }

  
    #region 이벤트 버튼

    public override void Close()
    {
        base.Close();
        UIMgr.OpenTown();
        //UIMgr.OpenDungeonPanel();
    }

    #endregion;

}

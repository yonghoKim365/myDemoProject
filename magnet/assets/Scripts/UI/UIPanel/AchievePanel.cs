using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievePanel : UIBasePanel
{

    public UIAtlas ImgAtals;//잠시임시

    //업적 탭타입
    public enum eAchieve_TAB_VIEW_TYPE
    {
        None = 0,
        CHARACTER = 1,//캐릭터
        BATTLE = 2,    //전투
        EQUIPMENT = 3,    //장비
        ECONOMY = 4,  //경제
        CONTENTS = 5,   //콘텐츠
        SOCIAL = 6, //소셜
        VIP = 7,  //VIP
    }

    private eAchieve_TAB_VIEW_TYPE CurViewType = eAchieve_TAB_VIEW_TYPE.None;//현재 보고있는 뷰 타입

    public GameObject InvenSlotPref;

    public UITabGroup TabGroup; //Tab버튼 관리하는 클래스

    public GameObject[] ViewObj;    //0일일업적 1일반업적
    public Transform[] TabBtns;//왼쪽탭루트
    //public Transform TabBtnRoot;    //왼쪽탭루트

    // 업적스크롤뷰
    public UIScrollView Scroll;
    public UIGrid Grid;
    public GameObject SlotPrefeb;

    //일일업적뷰
    public UIScrollView DailyScroll;
    public UIGrid DailyGrid;
    public GameObject DailySlotPrefab;

    // 업적카테고리 포인트 관련
    public UISlider PointSlider;    //업적포인트 달성률
    public UILabel AchievePointLabel;   //업적포인트 달성률
    public UISprite AchievePointRewardItemImg;  //포인트 달성아이템
    public UILabel AchievePointRewardItemCnt;
    public UIEventTrigger GetPointRewardBtn;  //포인트보상받기


    //일일업적포인트
    public UISlider DailyPointSlider;
    public UILabel DailyPointLabel;
    public UISprite DailyAchievePointRewardItemImg;  //포인트 달성아이템
    public Transform DailyAchievePointRewardGet;  //획득한포인트 달성
    public UILabel DailyAchievePointRewardItemCnt;
    public UIEventTrigger GetDailyPointRewardBtn;  //포인트보상받기
    public Transform VipItem;   //vip보상아이템

    // 버튼 
    public UIEventTrigger AllGet;    //일괄수령(수령가능한것들 한번에)

    public Color GageColor; //업적게이지 컬러값 (미완일때)
    public Transform GageEffRoot;
    public Transform DailyGageEffRoot;

    public GameObject EmptyList;    //빈화면
    public GameObject DailyEmptyList;    //빈화면

    // 업적정보들
    private List<NetData.AchieveLevelInfo> AchieveList = new List<NetData.AchieveLevelInfo>();
    private List<AchieveTmp> DailyAchieveList = new List<AchieveTmp>();

    private uint DailyPoint = 0;        //업적포인트
    private uint DailyPointGrade;   //업적포인트단계

    private Dictionary<eAchieve_TAB_VIEW_TYPE, uint> AchievePoint = new Dictionary<eAchieve_TAB_VIEW_TYPE, uint>();

    public GameObject[] SideTapAlram;   //대분류 알림마크 0일일업적 1일반업적
    public GameObject[] TabAlram;   //소분류탭 알림마크 (업적)
    public GameObject[] PointAlram; //포인트 알림마크 0일일업적 1일반업적
    public GameObject[] AllGetAlram;//일괄수령알림마크 0일일 1일반    

    private UIBasePanel ReOpenPanel;
    private Dictionary<uint, ulong> CurTabData = new Dictionary<uint, ulong>();    //현재탭데이터.. 소분류탭클릭시 쓰임.

    //private List<GameObject> GetEffGameObj;
    //private GameObject GetOneEffGameObj;

    public class AchieveTmp
    {
        public uint SubType;//소분류
        public ulong Value;//현재값
        public uint Level;//렙
        public uint Complete;//완료
        public uint Fetch;//수령

        public AchieveTmp(uint sub, ulong val, uint level, uint com, uint fet)
        {
            SubType = sub;
            Value = val;
            Level = level;
            Complete = com;
            Fetch = fet;
        }

    }

    private bool FirstFlag;
    private bool DailyFirstFlag;
    public override void Init()
    {
        SceneManager.instance.sw.Reset();
        SceneManager.instance.sw.Start();
        SceneManager.instance.showStopWatchTimer("AchievePanel, Init() start");

        base.Init();
        ViewObj[0].SetActive(false);
        ViewObj[1].SetActive(false);
        DailyPointGrade = 1;

        //GetEffGameObj = new List<GameObject>();
        //GetOneEffGameObj = new GameObject();

        DailyFirstFlag = true;
        UISprite vipSprite = VipItem.FindChild("Icon").GetComponent<UISprite>();
        Achievement.DailyRewardInfo pointInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementPointInof(DailyPointGrade);
        SetRewardItemSlot(_LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0], vipSprite);
        GameObject lockImg = VipItem.FindChild("Inbod").gameObject;
        lockImg.SetActive(NetData.instance.GetUserInfo()._VipLevel < 6);

        ViewObj[0].transform.FindChild("GetInfo/Txt_name").GetComponent<UILabel>().text =
            string.Format("{0}\n{1}", _LowDataMgr.instance.GetStringCommon(1300), _LowDataMgr.instance.GetStringCommon(1301));


        CreateSlot();


        SceneManager.instance.ShowNetProcess("GetAchieveList");


        UIHelper.CreateEffectInGame(GageEffRoot, "Fx_UI_Achieve_get_01");
        GageEffRoot.gameObject.SetActive(false);

        UIHelper.CreateEffectInGame(DailyGageEffRoot, "Fx_UI_Achieve_get_01");
        DailyGageEffRoot.gameObject.SetActive(false);

        BtnEvents();
        //uiMgr.SetTopMenuTitleName(247);

        SceneManager.instance.showStopWatchTimer("AchievePanel, Init() finish");
    }

    public override void LateInit()
    {
        base.LateInit();

        DailyAchieveList.Clear();

        NetworkClient.instance.SendPMsgAchieveDailyQueryInfoC(); //일일업적정보조회

        //일일업적단계들 다받아옴 (왜이렇게해야하지..)
        NetworkClient.instance.SendPMsgAchieveDailyFightTotalQueryInfoC();//전투
        NetworkClient.instance.SendPMsgAchieveDailyFriendTotalQueryInfoC();//소셜
        NetworkClient.instance.SendPMsgAchieveDailyMoneyTotalQueryInfoC();//경제
        NetworkClient.instance.SendPMsgAchieveDailyPlayTotalQueryInfoC();//컨텐츠
        NetworkClient.instance.SendPMsgAchieveDailyRoleTotalQueryInfoC();//캐릭터
        NetworkClient.instance.SendPMsgAchieveDailyVipTotalQueryInfoC();//vip



        NetworkClient.instance.SendPMsgAchieveQueryInfoC(); //업적정보조회
        if (parameters.Length == 0)

            return;

        FirstFlag = true;
        
        if (1 < parameters.Length)
            ReOpenPanel = (UIBasePanel)parameters[1];
        else
            ReOpenPanel = null;

        int type = (int)parameters[0];
        OnclickTabBtn(type);
        int achieveType = (int)parameters[2];

        SceneManager.instance.sw.Stop();
        SceneManager.instance.showStopWatchTimer("AchievePanel, LateInit() finish");
		
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    public void BtnEvents()
    {
        for (int i = 0; i < 2; i++)
        {
            int idx = i;
            UIButton tabBtn = TabBtns[i].GetComponent<UIButton>();
            EventDelegate.Set(tabBtn.onClick, delegate ()
            {
                OnclickTabBtn(idx);
            });
        }

        //포인트보상받기
        EventDelegate.Set(GetPointRewardBtn.onClick, delegate ()
        {

            if (GetPointRewardBtn.transform.FindChild("btn_off").gameObject.activeSelf)
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1017));
                return;
            }

            SceneManager.instance.ShowNetProcess("GetAchievePoint");
            NetworkClient.instance.SendPMsgAchieveFetchPointAwardC((uint)CurViewType);
        });


        //포인트보상받기
        EventDelegate.Set(GetDailyPointRewardBtn.onClick, delegate ()
        {

            if (GetDailyPointRewardBtn.transform.FindChild("btn_off").gameObject.activeSelf)
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1017));
                return;
            }

            NetworkClient.instance.SendPMsgAchieveDailyFetchPointAwardC(DailyPointGrade);
        });

    }
    //일일/일반 탭구분
    void OnclickTabBtn(int index)
    {
        if (ViewObj[index].activeSelf)
            return;

        UIMgr.instance.SetTopMenuTitleName(index == 0 ? (uint)1299 : (uint)247);

        if (index == 0)
            DailyFirstFlag = true;

        for (int i = 0; i < ViewObj.Length; i++)
        {
            ViewObj[i].SetActive(i == index);

            Transform btn = TabBtns[i];
            btn.FindChild("tab_on").gameObject.SetActive(i == index);
            btn.FindChild("tab_off").gameObject.SetActive(i != index);

        }

    }

    /// <summary>
    /// 미리슬롯생성
    /// </summary>
    public void CreateSlot()
    {
        //현재 슬롯 100개 
        for (int i = 0; i < 30; i++)
        {
            GameObject slotGo = Instantiate(SlotPrefeb) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = Grid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;


            slotGo.name = string.Format("{0}", i);//튜토리얼 관련해서 이름 찾는 부분이 있어 이름 변경
            slotGo.SetActive(false);
        }
        Destroy(SlotPrefeb);

        //일일업적
        for (int i = 0; i < 30; i++)
        {
            GameObject slotGo = Instantiate(DailySlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = DailyGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;


            slotGo.name = string.Format("{0}", i);//튜토리얼 관련해서 이름 찾는 부분이 있어 이름 변경
            slotGo.SetActive(false);
        }
        //Destroy(DailySlotPrefab);

    }

    #region 일일업적

    public void SetDailyAchieveInfo(uint count, List<NetData.AchieveLevelInfo> list, uint point)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //인덱스비교후 테이블에없으면 리턴
            if (!IsTableData(true, list[i].Type, list[i].level))
                continue;
            DailyAchieveList.Add(new AchieveTmp(list[i].Type, 0, list[i].level, list[i].Complete, list[i].Fetch));

            if (list[i].Complete == 1 && list[i].Fetch == 0)
            {
                //대분류
                SideTapAlram[0].SetActive(true);
            }
        }

        DailyPoint = point;


    }
    /// type => 호출순서.. vip일때 화면갱신
    public void GetDailyAchieveInfo(uint type, Dictionary<uint, uint> infoDic)
    {
        //이미 있는것에 있는지 확인후 추가해줌.
        foreach (uint index in infoDic.Keys)
        {
            //인덱스비교후 테이블에없으면 리턴
            if (!IsTableData(true, index, 0))
                continue;

            //처음에 받아온것에서 있는지검사      
            AchieveTmp info = GetDayilyAchieveData(index);

            if (info == null)// 새로저장해야함.
            {
                DailyAchieveList.Add(new AchieveTmp(index, infoDic[index], 1, 0, 0));
            }
            else
            {
                for (int i = 0; i < DailyAchieveList.Count; i++)
                {
                    AchieveTmp dailyInfo = DailyAchieveList[i];
                    if (dailyInfo.SubType == index)
                    {
                        //비교할 테이블데이터
                        Achievement.DailyInfo tableInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementInfo(index, DailyAchieveList[i].Level);
                        if (infoDic[index] >= tableInfo.MaxCount)
                            DailyAchieveList[i].Complete = 1;
                        DailyAchieveList[i].Value = infoDic[index];

                        if (DailyAchieveList[i].Complete == 1 && DailyAchieveList[i].Fetch == 0)
                        {
                            //대분류
                            SideTapAlram[0].SetActive(true);
                        }

                        continue;
                    }
                }

            }

        }

        if (type == 6)
        {
            //화면갱신
            ShowDayilyAchieveList();
        }

    }

    public void AfterGetDailyAchieve(List<NetData.AchieveLevelInfo> infoList)
    {
        for (int i = 0; i < infoList.Count; i++)
        {
            NetData.AchieveLevelInfo info = infoList[i];

            //인덱스비교후 테이블에없으면 리턴
            if (!IsTableData(true, info.Type, 0))
                continue;

            bool isNot = true;
            for (int j = 0; j < DailyAchieveList.Count; j++)
            {
                AchieveTmp _info = DailyAchieveList[j];
                if (_info.SubType == info.Type)
                {
                    isNot = false;

                    DailyAchieveList[j].Level = info.level;
                    DailyAchieveList[j].Complete = info.Complete;
                    DailyAchieveList[j].Fetch = info.Fetch;

                    if (DailyAchieveList[j].Complete == 1 && DailyAchieveList[j].Fetch == 0)
                    {
                        //대분류
                        SideTapAlram[0].SetActive(true);
                    }


                    break;
                }
            }
            if (isNot)//신규다.
            {
                DailyAchieveList.Add(new AchieveTmp(info.Type, 0, info.level, info.Complete, info.Fetch));
            }
        }

        ShowDayilyAchieveList();


    }

    //일일업적치 동기화
    public void SyncDailyAchievValue(uint type, uint value)
    {
        for (int i = 0; i < DailyAchieveList.Count; i++)
        {
            if (type == DailyAchieveList[i].SubType)
            {
                DailyAchieveList[i].Value = value;
                break;
            }
        }
        ShowDayilyAchieveList();
    }


    //일일업적포인트 동기화
    public void SetDailyPoint(bool isGrade, uint value)
    {
        if (isGrade)
            DailyPointGrade = value;
        else
        {
            DailyFirstFlag = false;
            DailyPoint = value;
        }
           

        Achievement.DailyRewardInfo pointInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementPointInof(DailyPointGrade);
        uint maxPoint = pointInfo.PointValue;

        //업적포인트 보상아이템 셋팅
        SetRewardItemSlot(_LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0], DailyAchievePointRewardItemImg);
        DailyAchievePointRewardItemCnt.text = _LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].Type == 14 ?
                    _LowDataMgr.instance.GetLowDataTitleName(_LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].ItemId) :
                    string.Format("x {0}", _LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].ItemCount.ToString());


        DailyPointLabel.text = string.Format("{0}/{1}", DailyPoint, maxPoint);
        //DailyPointSlider.value = (float)DailyPoint / maxPoint;
        bool isPointActive = DailyPoint >= maxPoint;
        PointAlram[0].SetActive(isPointActive);

        if (isPointActive || AllGetAlram[0].activeSelf)
            SideTapAlram[0].SetActive(true);
        else
            SideTapAlram[0].SetActive(false);


        GetDailyPointRewardBtn.transform.FindChild("btn_on").gameObject.SetActive(isPointActive);
        GetDailyPointRewardBtn.transform.FindChild("btn_off").gameObject.SetActive(!isPointActive);
        GetDailyPointRewardBtn.transform.FindChild("btn_off/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(496);

        if(!isGrade)
        {
            float prevValue = DailyPointSlider.value == 1 ? 0 : DailyPointSlider.value;
            float pointAmoint = value / (float)maxPoint;
            float nextValue = pointAmoint;

            StartCoroutine("GageEff", new object[] { prevValue, nextValue ,true });
        }
    }
    void ShowDayilyAchieveList()
    {
        int canGetCount = 0;

        UIButton AllGetBtn = ViewObj[0].transform.FindChild("GetInfo/Btn_allget").GetComponent<UIButton>();
        bool isPointActive = false;
        if (DailyFirstFlag)
        {
            Achievement.DailyRewardInfo pointInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementPointInof(DailyPointGrade);
            uint maxPoint = pointInfo.PointValue;

            //업적포인트 보상아이템 셋팅
            SetRewardItemSlot(_LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0], DailyAchievePointRewardItemImg);
            DailyAchievePointRewardItemCnt.text = _LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].Type == 14 ?
                        _LowDataMgr.instance.GetLowDataTitleName(_LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].ItemId) :
                        string.Format("x {0}", _LowDataMgr.instance.GetFixedRewardItemGroupList(pointInfo.RewardId)[0].ItemCount.ToString());

            DailyAchievePointRewardGet.gameObject.SetActive(false);
            
            DailyPointLabel.text = string.Format("{0}/{1}", DailyPoint, maxPoint);
            DailyPointSlider.value = (float)DailyPoint / maxPoint;
            isPointActive = DailyPoint >= maxPoint;

            PointAlram[0].SetActive(isPointActive);

            GetDailyPointRewardBtn.transform.FindChild("btn_on").gameObject.SetActive(isPointActive);
            GetDailyPointRewardBtn.transform.FindChild("btn_off").gameObject.SetActive(!isPointActive);
            GetDailyPointRewardBtn.transform.FindChild("btn_off/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(496);
        }

        bool isTutoSet = SceneManager.instance.CurTutorial == TutorialType.ACHIEVE;
        for (int i = 0; i < DailyGrid.transform.childCount; i++)
        {
            if (i >= DailyAchieveList.Count)
            {
                DailyGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slotGo = DailyGrid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;
            slotGo.SetActive(true);

            AchieveTmp info = DailyAchieveList[i];

            UILabel name = slotTf.FindChild("Txt_Name").GetComponent<UILabel>();
            UILabel dec = slotTf.FindChild("Txt_Dec").GetComponent<UILabel>();
            UIButton getBtn = slotTf.FindChild("Btn_allget").GetComponent<UIButton>();
            UISlider gage = slotTf.FindChild("Gage_bg").GetComponent<UISlider>();
            UILabel gageTxt = gage.transform.FindChild("GageTxt").GetComponent<UILabel>();
            UILabel lv = slotTf.FindChild("Level10/lv").GetComponent<UILabel>();
            UILabel expLabel = slotTf.FindChild("exp/exp_Txt").GetComponent<UILabel>(); //활약도
            UISprite item = slotTf.FindChild("Getitem/Itemicon").GetComponent<UISprite>();
            UILabel itemcont = slotTf.FindChild("Getitem/label_d3").GetComponent<UILabel>();
            
            Achievement.DailyInfo tableInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementInfo(info.SubType, info.Level);
            if (tableInfo == null)
            {
                Debug.LogError(string.Format("not found table type = {0} ,levle = {1}", info.SubType, info.Level));
                DailyGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            //최종단계까지 끝낫으면 슬롯삭제
            if (info.Complete == 1 && info.Fetch == 1 && info.Level == _LowDataMgr.instance.GetLowDataDaiylAchievementInfoList(info.SubType).Count)
            {
                slotGo.gameObject.SetActive(false);
                continue;
            }

            name.text = _LowDataMgr.instance.GetStringAchievement(tableInfo.NameId);
            dec.text = _LowDataMgr.instance.GetStringAchievement(tableInfo.DescId);
            lv.text = info.Level.ToString();

            gage.value = info.Value / (float)tableInfo.MaxCount;
            gageTxt.text = string.Format("{0}/{1}", info.Value, tableInfo.MaxCount);
            expLabel.text = string.Format("{0} {1}", _LowDataMgr.instance.GetStringCommon(1302), tableInfo.PointValue.ToString());

            EventDelegate.Set(getBtn.onClick, delegate ()
            {
                //보상받기, or 바로가기
                OnClickDailyGetBtn(info, tableInfo);
            });

            // 보상,경험치 테이블이값이 아직정해지지않아서 테이블수정되면 코드도 같이 수정해줘 (지금임시값)
            List<GatchaReward.FixedRewardInfo> rewardInfo = _LowDataMgr.instance.GetFixedRewardItemGroupList(tableInfo.RewardId);
            if (0 < rewardInfo.Count)
            {
                //보상아이템셋팅
                SetRewardItemSlot(rewardInfo[0], item);
                itemcont.text = rewardInfo[0].Type == 14 ?
                            _LowDataMgr.instance.GetLowDataTitleName(rewardInfo[0].ItemId) :
                            string.Format("x {0}", SceneManager.instance.NumberToString(rewardInfo[0].ItemCount));
            }
            
            //완료
            if (info.Complete == 1)
            {
                if (info.Fetch == 1 && info.Level == _LowDataMgr.instance.GetLowDataDaiylAchievementInfoList(info.SubType).Count)
                {
                    //최종단계까지 다 끈낸경우 일경우 슬롯에서 삭제해줌
                    slotGo.gameObject.SetActive(false);
                    continue;
                }
                else
                {
                    if (isTutoSet)
                    {
                        isTutoSet = false;
                        TutorialSupport support = getBtn.gameObject.AddComponent<TutorialSupport>();
                        support.TutoType = TutorialType.ACHIEVE;
                        support.SortId = 2;

                        support.NextTuto = TabBtns[1].gameObject.GetComponent<TutorialSupport>();
                        support.OnTutoSupportStart();
                    }

                    //보상받기 대기중
                    getBtn.transform.FindChild("Btn_on").gameObject.SetActive(true);
                    getBtn.transform.FindChild("Btn_off").gameObject.SetActive(false);
                    getBtn.transform.FindChild("Btn_on/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(496);
                    canGetCount++;
                }
            }
            else
            {

                //레벨체크후 바로가기버튼
                if (tableInfo.LimitLv > NetData.instance.GetUserInfo()._Level || !CheckContentsLink(tableInfo.Type))
                {
                    if (isTutoSet)
                    {
                        isTutoSet = false;
                        TutorialSupport support = getBtn.gameObject.AddComponent<TutorialSupport>();
                        support.TutoType = TutorialType.ACHIEVE;
                        support.SortId = 2;

                        support.NextTuto = TabBtns[1].gameObject.GetComponent<TutorialSupport>();
                        support.OnTutoSupportStart();
                    }

                    //렙부족일시 
                    getBtn.transform.FindChild("Btn_on").gameObject.SetActive(false);
                    getBtn.transform.FindChild("Btn_off").gameObject.SetActive(true);

                    if (tableInfo.Type == 502)
                        getBtn.transform.FindChild("Btn_off/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1304);
                    else
                        getBtn.transform.FindChild("Btn_off/label_d3").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1303), _LowDataMgr.instance.GetLowDataDaiylAchievementInfo(DailyAchieveList[i].SubType, DailyAchieveList[i].Level).LimitLv);
                }
                else
                {
                    //바로가기
                    getBtn.transform.FindChild("Btn_on").gameObject.SetActive(true);
                    getBtn.transform.FindChild("Btn_off").gameObject.SetActive(false);

                    getBtn.transform.FindChild("Btn_on/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1306);
                }

            }
        }

        bool canGetActive = canGetCount > 0;
        AllGetBtn.transform.FindChild("Btn_on").gameObject.SetActive(canGetActive);
        AllGetBtn.transform.FindChild("Btn_off").gameObject.SetActive(!canGetActive);

        AllGetAlram[0].SetActive(canGetActive);

        EventDelegate.Set(AllGetBtn.onClick, delegate ()
        {
            if (!canGetActive)
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1017));
                return;
            }

            NetworkClient.instance.SendPMsgAchieveDailyOneKeyFetchAchieveAwardC();
        });
        
        DailyGrid.repositionNow = true;
        DailyScroll.ResetPosition();
        DailyScroll.transform.localPosition = Vector3.zero;
        DailyScroll.GetComponent<UIPanel>().clipOffset = Vector3.zero;

        //빈화면일시 출력
        bool flag = true;
        for (int j = 0; j < DailyGrid.transform.childCount; j++)
        {
            if (DailyGrid.transform.GetChild(j).gameObject.activeSelf)
            {
                flag = false;
                break;
            }
        }
        DailyEmptyList.SetActive(flag);

        //모든보상획득한것
        if (flag)
        {
            GetDailyPointRewardBtn.transform.FindChild("btn_off/label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(145);
        }

        if (canGetActive || PointAlram[0].activeSelf)
        {
            SideTapAlram[0].SetActive(true);
        }
        else
            SideTapAlram[0].SetActive(false);
    }

    void OnClickDailyGetBtn(AchieveTmp dailyinfo, Achievement.DailyInfo tableInfo)
    {
        if (dailyinfo.Complete == 1)
        {
            //보상받기
            NetworkClient.instance.SendPMsgAchieveDailyFetchAwardC(dailyinfo.SubType);
        }
        else
        {
            //레벨체크후 바로가기버튼
            if (tableInfo.LimitLv > NetData.instance.GetUserInfo()._Level)
            {
                //암됨 
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(712));
                return;
            }

            //혹시모르니다시체크...
            if (!CheckContentsLink(dailyinfo.SubType))
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(712));
                return;
            }

            if (dailyinfo.SubType==115)
            {
                return;
            }

            base.Hide();

            switch (dailyinfo.SubType)
            {
                case 501://모험
                    UIMgr.OpenChapter(this,101);
                    break;
                case 502: //어려움
                    UIMgr.OpenChapter(this,10101);
                    break;
                case 504://골드
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_GOLD, 0);
                    break;
                case 505://경치
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.SPECIAL_EXP, 0);
                    break;
                case 503://마탑
                    UIMgr.OpenTowerPanel();
                    break;
                case 521://차관
                    UIMgr.OpenArenaPanel(this);
                    break;
                case 517://콜로쎼움
                    UIMgr.OpenColosseumPanel(0);
                    break;
                case 506://오화
                    break;
                case 511://보스레이드
                    DungeonTable.SingleBossRaidInfo bossRaid = _LowDataMgr.instance.GetSingleBossRaidLimitLevel(1001);
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.RAID, bossRaid.Type);
                    break;
                case 516://멀티보스
                    DungeonTable.MultyBossRaidInfo multyBossRaid = _LowDataMgr.instance.GetLowDataMultyBossInfo(1001);
                    UIMgr.OpenDungeonPanel(false, 0, GAME_MODE.MULTI_RAID, multyBossRaid.Type);
                    break;
                case 113: //난투장
                case 114://난투장
                    UIMgr.OpenDogFight();
                    break;
                case 402://골드소모 (상점)
                case 404://원보소모 (상점)
                    UIMgr.OpenShopPanel(this);
                    break;
                case 601: //친구에게체력
                    UIMgr.OpenSocialPanel(1, this);
                    break;
            }

        }
    }

    /// <summary> 조건 체크. </summary>
    bool CheckContentsLink(uint type)
    {
        bool isEnter = true;

        switch (type)
        {
            case 501://모험
                isEnter = true;
                break;
            case 502: //어려움
                uint require = 0;
                NetData.ClearSingleStageData stageData = null;
                if (!NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(10101, out stageData))
                {
                    DungeonTable.StageInfo stageInfo = _LowDataMgr.instance.GetStageInfo(10101);
                    if (stageInfo != null && stageInfo.RequireStageId != null && 0 < stageInfo.RequireStageId.list.Count)
                    {
                        require = uint.Parse(stageInfo.RequireStageId.list[0]);
                        if (NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(require, out stageData))//이전 스테이지 클리어해야함.
                            require = 0;
                    }
                }

                if (require <= 0)
                    isEnter = true;
                else
                    isEnter = false;
                break;

            case 506://오화
                break;
            case 521://차관
                if (_LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvpEnterLv) > NetData.instance.UserLevel)
                    isEnter = false;
                break;
            case 511://보스레이드
                DungeonTable.SingleBossRaidInfo bossRaid = _LowDataMgr.instance.GetSingleBossRaidLimitLevel(1001);
                if (bossRaid.levelLimit > NetData.instance.UserLevel)
                    isEnter = false;
                break;
            case 516://멀티보스
                DungeonTable.MultyBossRaidInfo multyBossRaid = _LowDataMgr.instance.GetLowDataMultyBossInfo(1001);
                if (multyBossRaid.levelLimit > NetData.instance.UserLevel)
                    isEnter = false;
                break;
            case 113: //난투장
            case 114://난투장
                List<DungeonTable.FreefightTableInfo> freeList = _LowDataMgr.instance.GetLowDataFreeFightList();
                for (int i = 0; i < freeList.Count; i++)
                {
                    if (NetData.instance.UserLevel >= freeList[i].MinenterLv || freeList[i].MaxenterLv >= NetData.instance.UserLevel)
                        continue;

                    isEnter = false;
                    break;
                }
                break;
            case 402://골드소모 (상점)
            case 404://원보소모 (상점)
                isEnter = true;
                break;

            case 504://골드
            case 505://경치
            case 503://마탑
            case 517://콜로쎼움
                byte idx = 0;
                if (type == 504)
                    idx = (byte)9;
                else if (type == 505)
                    idx = (byte)1;
                else if (type == 503)
                    idx = (byte)3;
                else if (type == 517)
                    idx = (byte)2;

                DungeonTable.ContentsOpenInfo content = _LowDataMgr.instance.GetFirstContentsOpenInfo(idx);
                if (content.ConditionType1 == 2 && content.ConditionValue1 > NetData.instance.GetUserInfo()._TotalAttack)
                    isEnter = false;
                else if (content.ConditionType1 == 1 && content.ConditionValue1 > NetData.instance.UserLevel)
                    isEnter = false;
                break;
        }

        if (!isEnter)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region 일반업적
    public void SetAchieveInfo(uint count, List<NetData.AchieveLevelInfo> list, uint role, uint fight, uint money, uint play, uint friend, uint vip)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //인덱스비교후 테이블에없으면 리턴
            if (!IsTableData(false, list[i].Type, list[i].SubType))
                continue;
            AchieveList.Add(list[i]);
        }

        AchievePoint[eAchieve_TAB_VIEW_TYPE.CHARACTER] = role;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.BATTLE] = fight;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.EQUIPMENT] = 0;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.ECONOMY] = money;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.CONTENTS] = play;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.SOCIAL] = friend;
        AchievePoint[eAchieve_TAB_VIEW_TYPE.VIP] = vip;

        CheckTabAlram();

        int openType = 1;
        if (1 < parameters.Length)
            openType = (int)parameters[2];

        TabGroup.DefaultInitIndex = openType;
        TabGroup.Initialize(OnClickTab);
    }

    void CheckTabAlram()
    {
        uint[] Alram = new uint[] { 0, 0, 0, 0, 0, 0, 0 };

        SideTapAlram[1].SetActive(false);
        for (int i = 0; i < AchieveList.Count; i++)
        {
            //완료됬고 수령받기전
            if (AchieveList[i].Complete == 1 && AchieveList[i].Fetch == 0)
            {
                if (!SideTapAlram[1].activeSelf)
                    SideTapAlram[1].SetActive(true);
                if (Alram[AchieveList[i].Type - 1] != 1)
                    Alram[AchieveList[i].Type - 1] = 1;
            }
        }
        //TabAlram
        for (int i = 0; i < TabAlram.Length; i++)
        {
            TabAlram[i].SetActive(Alram[i] == 1);
        }

        foreach (eAchieve_TAB_VIEW_TYPE index in AchievePoint.Keys)
        {
            if (AchievePoint[index] >= 10)
            {
                if(!SideTapAlram[1].activeSelf)
                    SideTapAlram[1].SetActive(true);
                int i = (int)index;
                if (!TabAlram[i - 1].activeSelf)
                    TabAlram[i - 1].SetActive(true);
            }
        }

    }

    ///<summar> 업적보상 수령후 재갱신 </summary>
    public void GetAfterAchieveMent(NetData.AchieveLevelInfo info)
    {
        for (int i = 0; i < AchieveList.Count; i++)
        {
            bool isNot = true;
            NetData.AchieveLevelInfo data = AchieveList[i];
            if (data.Type == info.Type && data.SubType == info.SubType)
            {
                isNot = false;
                AchieveList[i] = info;//바뀐값으로저장해준다
                continue;
            }

            if (isNot)//신규다.
            {
                AchieveList.Add(info);
            }
        }

        CheckTabAlram();
        TabGroup.CoercionTab((int)info.Type);

    }

    ///<summar> 업적포인트 수령후 재갱신 </summary>
    public void GetAfterAchieveMentPoint(uint type, uint value)
    {

        //if (GetOneEffGameObj != null)
        //{
        //    GameObject tmp = GetOneEffGameObj;

        //    UIHelper.CreateEffectInGame(tmp.transform, "Fx_UI_achieve_get_02");
        //    TweenTransform tween = tmp.transform.GetChild(0).GetComponentInChildren<TweenTransform>();
        //    if (tween != null)
        //    {
        //        tween.ResetToBeginning();
        //        tween.to = uiMgr.TopMenu.CategoryGo.transform;
        //        tween.PlayForward();
        //    }

        //    TempCoroutine.instance.FrameDelay(1f, () =>
        //    {
        //        if (tmp.transform.childCount > 0)
        //        {
        //            for (int i = 0; i < tmp.transform.childCount; i++)
        //            {
        //                DestroyImmediate(tmp.transform.GetChild(i).gameObject);
        //            }
        //        }
        //    });

        //}
        //else
        //{
        //    List<GameObject> tmpList = GetEffGameObj;

        //    for (int i = 0; i < tmpList.Count; i++)
        //    {
        //        UIHelper.CreateEffectInGame(tmpList[i].transform, "Fx_UI_achieve_get_02");
        //        TweenTransform tween = tmpList[i].transform.GetChild(0).GetComponentInChildren<TweenTransform>();
        //        if (tween != null)
        //        {
        //            tween.ResetToBeginning();
        //            tween.to = uiMgr.TopMenu.CategoryGo.transform;
        //            tween.PlayForward();
        //        }
        //    }
        //    //TempCoroutine.instance.FrameDelay(1f, () =>
        //    //{
        //    for (int i = 0; i < tmpList.Count; i++)
        //    {
        //        if (tmpList[i].transform.childCount > 0)
        //        {
        //            for (int j = 0; j < tmpList[i].transform.childCount; j++)
        //            {
        //                Destroy(tmpList[i].transform.GetChild(j).gameObject, 1f);
        //                //DestroyImmediate(tmpList[i].transform.GetChild(j).gameObject);
        //            }
        //        }
        //    }
        //    // })
        //}


        FirstFlag = false;

        if (type == 3)
            type = 4;

        eAchieve_TAB_VIEW_TYPE eType = (eAchieve_TAB_VIEW_TYPE)type;


        AchievePoint[eType] = value;

        CheckTabAlram();
        uint max = _LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).Clearvalue;

        //업적포인트 보상아이템 셋팅

        SetRewardItemSlot(_LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0], AchievePointRewardItemImg);
        AchievePointRewardItemCnt.text = _LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].Type == 14 ?
                    _LowDataMgr.instance.GetLowDataTitleName(_LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].ItemId) :
                    string.Format("x {0}", _LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].ItemCount.ToString());



        //업적포인트가 최대치일경우만 활성화해준다
        bool isActive = AchievePoint[(eAchieve_TAB_VIEW_TYPE)type] >= max;

        PointAlram[1].SetActive(isActive);

        GetPointRewardBtn.transform.FindChild("btn_on").gameObject.SetActive(isActive);
        GetPointRewardBtn.transform.FindChild("btn_off").gameObject.SetActive(!isActive);

        GetPointRewardBtn.transform.FindChild("btn_off").transform.FindChild("label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(496);


        AchievePointLabel.text = string.Format("{0} / {1}", AchievePoint[eType], max);

        PointSlider.transform.FindChild("Gage_bar").GetComponent<UISprite>().spriteName = "Img_Gage00";

        float prevValue = PointSlider.value == 1 ? 0 : PointSlider.value;
        float pointAmoint = (float)AchievePoint[eType] / (float)max;
        float nextValue = pointAmoint;

        StartCoroutine("GageEff", new object[] { prevValue, nextValue, false });

    }

    IEnumerator GageEff(object[] value)
    {
        //0.5초동안게이지바 이동
        float from = (float)value[0];
        float to = (float)value[1];
        bool isDaily = (bool)value[2];

        float barTime = 0f;
        while (barTime < 0.25f)
        {
            barTime += Time.deltaTime;
            float lerpValue = barTime / 0.25f;
            if (isDaily)
                DailyPointSlider.value = Mathf.Lerp(from, to, lerpValue);
            else
                PointSlider.value = Mathf.Lerp(from, to, lerpValue);
            yield return null;
        }
        //이동뒤에 이펙트

        if(isDaily)
        {
            DailyPointSlider.transform.FindChild("Gage_bar").GetComponent<UISprite>().spriteName =/* GetPointRewardBtn.isEnabled == true ? "Img_Gage03" : */"Img_Gage01";
            GageEffRoot.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            GageEffRoot.gameObject.SetActive(false);
        }
        else
        {
            PointSlider.transform.FindChild("Gage_bar").GetComponent<UISprite>().spriteName =/* GetPointRewardBtn.isEnabled == true ? "Img_Gage03" : */"Img_Gage01";
            DailyGageEffRoot.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            DailyGageEffRoot.gameObject.SetActive(false);
        }

        

        yield return null;
    }


    /// <summary> 탭버튼 클릭시 들어올 콜백함수 </summary>
    void OnClickTab(int viewType)
    {
        if (viewType <= 0)
            return;

        ChangeView((eAchieve_TAB_VIEW_TYPE)viewType);
    }


    /// <summary> 탭 버튼에 따라 보고있는 뷰를 바꿔준다  </summary>
    void ChangeView(eAchieve_TAB_VIEW_TYPE type)
    {
        if (CurViewType != type)//
            FirstFlag = true;

        eAchieve_TAB_VIEW_TYPE prevType = CurViewType;
        CurViewType = type;

        switch (type)
        {
            case eAchieve_TAB_VIEW_TYPE.None:
                break;
            case eAchieve_TAB_VIEW_TYPE.CHARACTER:
                SceneManager.instance.ShowNetProcess("CharAchieve");
                NetworkClient.instance.SendPMsgAchieveRoleTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.BATTLE:
                SceneManager.instance.ShowNetProcess("BattleAchieve");
                NetworkClient.instance.SendPMsgAchieveFightTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.EQUIPMENT:
                //SceneManager.instance.ShowNetProcess("EquipAchieve");
                //NetworkClient.instance.SendPMsgAchieveEquipTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.ECONOMY:
                SceneManager.instance.ShowNetProcess("EconomyAchieve");
                NetworkClient.instance.SendPMsgAchieveMoneyTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.CONTENTS:
                SceneManager.instance.ShowNetProcess("ContentsAchieve");
                NetworkClient.instance.SendPMsgAchievePlayTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.SOCIAL:
                SceneManager.instance.ShowNetProcess("SocialAchieve");
                NetworkClient.instance.SendPMsgAchieveFriendTotalQueryInfoC();
                break;
            case eAchieve_TAB_VIEW_TYPE.VIP:
                SceneManager.instance.ShowNetProcess("VipAchieve");
                NetworkClient.instance.SendPMsgAchieveVipTotalQueryInfoC();
                break;
            default:
                break;
        }

    }

    // 업적테이블에서 업적순서와 단계를
    public Achievement.AchievementInfo GetCompareData(List<Achievement.AchievementInfo> list, uint subType, uint curLv)
    {
        Achievement.AchievementInfo data = new Achievement.AchievementInfo();


        for (int i = 0; i < list.Count; i++)
        {
            Achievement.AchievementInfo info = list[i];
            if (info.Subtype == subType && info.Phase == curLv)
            {
                data = info;
                //return data;
            }
        }

        return data;
    }

    // 일일업적테이블에서 업적순서와 단계를
    public Achievement.DailyInfo GetDailyCompareData(List<Achievement.DailyInfo> list, uint type, uint curLv)
    {
        Achievement.DailyInfo data = new Achievement.DailyInfo();

        for (int i = 0; i < list.Count; i++)
        {
            Achievement.DailyInfo info = list[i];
            if (info.Type == type && info.Phase == curLv)
            {
                data = info;
                break;
            }
        }

        return data;
    }
    public Achievement.AchievementInfo GetTableData(List<Achievement.AchievementInfo> list, uint subType, uint curLv)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Achievement.AchievementInfo info = list[i];
            if (info.Subtype != subType || info.Phase != curLv)
                continue;

            return info;
        }

        Debug.LogError(string.Format("not found TableData error searchType : subType={0}, curLv={1}", subType, curLv));
        return null;
    }

    ////보상아이템정보 (가챠리워드테이블에서..)
    void SetRewardItemSlot(GatchaReward.FixedRewardInfo itemInfo, UISprite itemImg)
    {
        string iconName = "";

        uint itemidx = 0;
        switch ((Sw.UNITE_TYPE)itemInfo.Type)
        {
            case Sw.UNITE_TYPE.UNITE_TYPE_COIN:
                iconName = "money";
                itemidx = 599000;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_GEM:
                iconName = "cash";
                itemidx = 599001;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_CONTRIBUTION:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_HONOR:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROYAL_BADGE:
                iconName = "Img_flag2";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_LION_KING_BADGE:
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_FAME:
                iconName = "starlight";
                itemidx = 599003;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_POWER:
                iconName = "stamina";
                itemidx = 599104;
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROLE_EXP:
                itemidx = 599105;
                iconName = "Img_ap";   //일단임시로넣어둠
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_TITLE:    //임시로 honor를쓰도록함
                iconName = "title";
                break;
            default:
                break;
        }

        if (iconName == "")
        {
            SetRewardItem(itemInfo.ItemId, itemImg);
        }
        else
        {
            itemImg.atlas = ImgAtals;
            itemImg.spriteName = iconName;

            if (itemidx != 0)
            {
                UIEventTrigger etri = itemImg.gameObject.GetComponent<UIEventTrigger>();
                if (etri != null)
                {
                    EventDelegate.Set(etri.onClick, delegate ()
                    {
                        if (itemidx == 0)
                            return;

                        UIMgr.OpenDetailPopup(this, itemidx);
                    });
                }
            }

        }
    }

    // 보상 아이템정보 
    void SetRewardItem(uint itemIdx, UISprite itemImg)
    {
        Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx);
        if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
        {
            itemImg.atlas = AtlasMgr.instance.GetEquipAtlasForClassId(eLowData.Class);

            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
        }
        else//소모아이템이 드랍아이템 대표로 등록되어 있음
        {
            Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(itemIdx);
            if (uLowData == null)
            {
                // 초상화일수있으니 다시검ㅅ ㅏ
                Partner.PartnerDataInfo partner = _LowDataMgr.instance.GetPartnerInfo(itemIdx);
                if (partner != null)
                {
                    itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
                    itemImg.spriteName = partner.PortraitId;

                    return;
                }


            }

            if (uLowData.Type == (byte)ItemType.Costum || uLowData.Type == (byte)ItemType.Partner)
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
            else
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.UseItem);

            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);
        }

        UIEventTrigger etri = itemImg.gameObject.GetComponent<UIEventTrigger>();
        if (etri != null)
        {
            EventDelegate.Set(etri.onClick, delegate ()
            {
                UIMgr.OpenDetailPopup(this, itemIdx);
            });
        }

    }

    //잠시
    public void _AchieveDataTest(byte type, Dictionary<uint, ulong> myData)
    {
        if (myData.Count == 0)
            return;

        CurTabData = myData;
        Debug.Log("ㅈㅣ움ㅇㅇ");

        bool isActivePoint = false;
        if (FirstFlag)
        {
            //포인트 최대치
            uint max = _LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).Clearvalue;
            AchievePointLabel.text = string.Format("{0} / {1}", AchievePoint[(eAchieve_TAB_VIEW_TYPE)type], max);
            //업적포인트 달성률
            float pointAmoint = (float)AchievePoint[(eAchieve_TAB_VIEW_TYPE)type] / (float)max;
            PointSlider.value = pointAmoint;

            //업적포인트 보상아이템 셋팅
            SetRewardItemSlot(_LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0], AchievePointRewardItemImg);
            AchievePointRewardItemCnt.text = _LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].Type == 14 ?
                        _LowDataMgr.instance.GetLowDataTitleName(_LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].ItemId) :
                        string.Format("x {0}", _LowDataMgr.instance.GetFixedRewardItemGroupList(_LowDataMgr.instance.GetLowDataAchievementInfoCategory(type).RewardId)[0].ItemCount.ToString());

            //업적포인트가 최대치일경우만 활성화해준다
            isActivePoint = AchievePoint[(eAchieve_TAB_VIEW_TYPE)type] >= max;

            PointAlram[1].SetActive(isActivePoint);

            GetPointRewardBtn.transform.FindChild("btn_on").gameObject.SetActive(isActivePoint);
            GetPointRewardBtn.transform.FindChild("btn_off").gameObject.SetActive(!isActivePoint);

            PointSlider.transform.FindChild("Gage_bar").GetComponent<UISprite>().spriteName = /*GetPointRewardBtn.isEnabled == true ? "Img_Gage03" : */"Img_Gage01";

        }

        //List<uint> canGetAll = new List<uint>();    //일괄수령 가능한지 체크
        
        //정렬을 위해..List..
        List<AchieveTmp> sortTmpList = new List<AchieveTmp>();
        List<Achievement.AchievementInfo> list = null;

        foreach (uint index in CurTabData.Keys)
        {
            //인덱스비교후 테이블에없으면 리턴
            if (!IsTableData(false, type, index))
                continue;

            //처음에 받아온것에서 있는지검사
            NetData.AchieveLevelInfo info = GetAchieveData(type, index);

            //비교할 테이블데이터

            list = _LowDataMgr.instance.GetLowDataAchievementInfo(type);

            Achievement.AchievementInfo data = info == null ? GetCompareData(list, index, 1) : GetTableData(list, info.SubType, info.level);

            if (data == null)
                continue;

            uint value = (uint)CurTabData[index];
            uint level = info == null ? 1 : info.level;

            if ((info != null && info.Complete == 1) || value >= data.Clearvalue)//완료 
            {
                if (info != null && _LowDataMgr.instance.GetLowDataAchievementInfoSubType(info.Type, info.SubType).Count == info.level && info.Fetch == 1)
                {
                    //업적의 마지막단계까지 달성했다 
                    AchieveTmp tmp = new AchieveTmp(index, CurTabData[index], level, 1, 1);
                    sortTmpList.Add(tmp);
                }
                else //보상받기 대기중
                {
                    AchieveTmp tmp = new AchieveTmp(index, CurTabData[index], level, 1, 0);
                    sortTmpList.Add(tmp);
                }

            }
            else
            {
                //진행중
                AchieveTmp tmp = new AchieveTmp(index, CurTabData[index], level, 0, 0);
                sortTmpList.Add(tmp);
            }
        }

        sortTmpList.Sort(sortAchieveList);
        
        int rewardCount = 0;
        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            if (i >= sortTmpList.Count)
            {
                Grid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }
            
            GameObject slotGo = Grid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;

            slotGo.SetActive(true);
            UILabel slotLv = slotTf.FindChild("Level10/lv").GetComponent<UILabel>();  //현재 몇단계

            UIButton getBtn = slotTf.FindChild("Btn_allget").GetComponent<UIButton>(); //수령버튼      

            //보상달성률
            UISlider gageBar = slotTf.FindChild("Gage_bg").GetComponent<UISlider>();
            UILabel txt = slotTf.FindChild("Gage_bg/GageTxt").GetComponent<UILabel>();

            UISprite exp = slotTf.FindChild("Exp/icon").GetComponent<UISprite>();
            UILabel expTxt = slotTf.FindChild("Exp/expTxt").GetComponent<UILabel>();//경험치

            UILabel name = slotTf.FindChild("Txt_Name").GetComponent<UILabel>();//업적 이름 
            UILabel txts = slotTf.FindChild("Txt_info").GetComponent<UILabel>();//업적 설명

            UISprite itemImg = slotTf.FindChild("Getitem/Itemicon").GetComponent<UISprite>(); //보상이미지
            UILabel itmeCnt = slotTf.FindChild("Getitem/label_d3").GetComponent<UILabel>(); // 갯수

            GameObject complete = slotTf.FindChild("Complete").gameObject;

            Achievement.AchievementInfo data = GetTableData(list, sortTmpList[i].SubType, sortTmpList[i].Level);


            gageBar.transform.FindChild("Gagebar_on").gameObject.SetActive(true);
            gageBar.transform.FindChild("Gagebar_off").gameObject.SetActive(false);

            if (!gageBar.transform.FindChild("Gagebar_on").GetComponent<UISprite>().isActiveAndEnabled)
                gageBar.transform.FindChild("Gagebar_on").GetComponent<UISprite>().enabled = true;

            // 보상,경험치 테이블이값이 아직정해지지않아서 테이블수정되면 코드도 같이 수정해줘 (지금임시값)
            List<GatchaReward.FixedRewardInfo> rewardInfo = _LowDataMgr.instance.GetFixedRewardItemGroupList(data.RewardId);

            if (0 < rewardInfo.Count)
            {
                name.text = _LowDataMgr.instance.GetStringAchievement(data.NameId);
                txts.text = _LowDataMgr.instance.GetStringAchievement(data.DescriptionId);

                float barAmount = sortTmpList[i].Value / (float)data.Clearvalue;
                gageBar.value = barAmount;

                txt.text = string.Format("{0} / {1}", sortTmpList[i].Value, data.Clearvalue);

                SetRewardItemSlot(rewardInfo[0], itemImg);
                itmeCnt.text = rewardInfo[0].Type == 14 ?
                            _LowDataMgr.instance.GetLowDataTitleName(rewardInfo[0].ItemId) :
                            string.Format("x {0}", SceneManager.instance.NumberToString(rewardInfo[0].ItemCount));

                SetRewardItemSlot(rewardInfo[1], exp);
                expTxt.text = rewardInfo[1].Type == 14 ?
                            _LowDataMgr.instance.GetLowDataTitleName(rewardInfo[1].ItemId) :
                            string.Format("x {0}", SceneManager.instance.NumberToString(rewardInfo[1].ItemCount));

                exp.GetComponent<BoxCollider>().enabled = rewardInfo[1].Type == 14 ? false : true;

            }

            slotLv.text = sortTmpList[i].Level.ToString();
            getBtn.transform.gameObject.SetActive(true);
            complete.SetActive(false);

            //수령버튼
            EventDelegate.Set(getBtn.onClick, delegate ()
            {
                if (getBtn.transform.FindChild("Btn_off").gameObject.activeSelf)
                {
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1017));
                    return;
                }
                SceneManager.instance.ShowNetProcess("FetchAchieve");
                NetworkClient.instance.SendPMsgAchieveFetchAwardC(data.Type, data.Subtype);
            });

            //달성 
            if (sortTmpList[i].Complete == 1)//완료 
            {

                if (sortTmpList[i].Fetch == 1)
                {
                    //업적의 마지막단계까지 달성했다  게이지바색상 변경후 버튼숨기고 완료표시 
                    gageBar.transform.FindChild("Gagebar_on").gameObject.SetActive(false);
                    gageBar.transform.FindChild("Gagebar_off").gameObject.SetActive(true);
                    getBtn.transform.gameObject.SetActive(false);
                    complete.SetActive(true);
                }
                else //보상받기 대기중
                {
                    gageBar.transform.FindChild("Gagebar_on").gameObject.SetActive(true);
                    gageBar.transform.FindChild("Gagebar_off").gameObject.SetActive(false);

                    getBtn.transform.FindChild("Btn_on").gameObject.SetActive(true);
                    getBtn.transform.FindChild("Btn_off").gameObject.SetActive(false);

                    //canGetAll.Add((uint)i + 1);
                    ++rewardCount;
                }
            }
            else // 진행중
            {
                if (sortTmpList[i].Value == 0)
                    gageBar.transform.FindChild("Gagebar_on").gameObject.SetActive(false);
                
                getBtn.transform.FindChild("Btn_on").gameObject.SetActive(false);
                getBtn.transform.FindChild("Btn_off").gameObject.SetActive(true);
            }
        }

        bool isActive = rewardCount > 0;
        AllGet.transform.FindChild("Btn_on").gameObject.SetActive(isActive);
        AllGet.transform.FindChild("Btn_off").gameObject.SetActive(!isActive);

        AllGetAlram[1].SetActive(isActive);

        EventDelegate.Set(AllGet.onClick, delegate ()
        {
            if (rewardCount == 0)  //보상받을게 없음
            {
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, _LowDataMgr.instance.GetStringCommon(1017));
                return;
            }
            else
            {
                SceneManager.instance.ShowNetProcess("FetchAllAchieve");
                NetworkClient.instance.SendPMsgAchieveOneKeyFetchAchieveAwardC(type);
            }
        });



        //빈화면일시 출력
        bool flag = true;
        for (int j = 0; j < Grid.transform.childCount; j++)
        {
            if (Grid.transform.GetChild(j).gameObject.activeSelf)
            {
                flag = false;
                break;
            }
        }
        EmptyList.SetActive(flag);


        Grid.Reposition();
        Scroll.ResetPosition();
        Scroll.transform.localPosition = Vector3.zero;/* ResetPosition();*/
        Scroll.GetComponent<UIPanel>().clipOffset = Vector2.zero;

        Scroll.enabled = sortTmpList.Count > 4;
    }

    //업적정렬
    //1순위 완료되지않은업적
    //2순위 완료되지않은업적중 보상가능
    //3순위 나머지는인덱스순
    int sortAchieveList(AchieveTmp a, AchieveTmp b)
    {
        if (a.Complete > b.Complete)
        {
            //완료된것중 수령받은건 뒤로
            if (a.Fetch > b.Fetch)
                return 1;
            else
                return -1;

        }
        else if (a == b)
            return 0;
        else if (a.Complete < b.Complete)
        {
            // a == 완료0 , b == 완료1 수령0 ( a가뒤 b가앞으로 )
            if (b.Fetch == 0)
                return 1;
            else // a == 완료0 , b == 완료1 수령1 ( a가앞  b가뒤로  )
                return -1;
        }
        else 
        {
            //수령받은건 뒤로
            if (a.Fetch > b.Fetch)
                return 1;
            else if (a.Fetch == b.Fetch)
            {
                if (a.SubType > b.SubType)
                    return 1;
                else
                    return -1;
            }
            else
                return -1;
        }
       
        
    }

    public NetData.AchieveLevelInfo GetAchieveData(uint type, uint subType)
    {
        NetData.AchieveLevelInfo data = null;

        for (int i = 0; i < AchieveList.Count; i++)
        {
            NetData.AchieveLevelInfo info = AchieveList[i];
            if (info.Type == type && info.SubType == subType)
            {
                data = info;
                break;
            }
        }

        return data;
    }

    public AchieveTmp GetDayilyAchieveData(uint type)
    {
        AchieveTmp data = null;

        for (int i = 0; i < DailyAchieveList.Count; i++)
        {
            AchieveTmp info = DailyAchieveList[i];
            if (info.SubType == type)
            {
                data = info;
                break;
            }
        }

        return data;
    }

    public bool IsTableData(bool isDaily, uint type, uint subType)
    {
        if (isDaily)
        {
            //타입만일단 비교해줌..
            List<Achievement.DailyInfo> daylist = _LowDataMgr.instance.GetLowDataDaiylAchievementInfoList(type);

            if (daylist.Count > 0)
                return true;

            return false;
        }
        else
        {

            List<Achievement.AchievementInfo> list = _LowDataMgr.instance.GetLowDataAchievementInfo(type);

            for (int i = 0; i < list.Count; i++)
            {
                Achievement.AchievementInfo info = list[i];
                if (info.Subtype != subType)
                    continue;

                return true;
            }

            return false;
        }

    }

    public void AfterGetAllAchieve(List<NetData.AchieveLevelInfo> infoList)
    {
        for (int i = 0; i < infoList.Count; i++)
        {
            NetData.AchieveLevelInfo info = infoList[i];
            bool isNot = true;
            for (int j = 0; j < AchieveList.Count; j++)
            {
                NetData.AchieveLevelInfo _info = AchieveList[j];
                if (_info.Type == info.Type && _info.SubType == info.SubType)
                {
                    isNot = false;
                    AchieveList[j] = info;  //갱신값 바꿔줘
                }
            }

            if (isNot)//신규다.
            {
                AchieveList.Add(info);
            }
        }

        CheckTabAlram();
        TabGroup.CoercionTab((int)infoList[0].Type);

    }

    #endregion 

    public override void Hide()
    {
        bool isAlram = false;
        //닫기전에 체크
        for (int i = 0; i < AchieveList.Count; i++)
        {
            // 달성되있는데 보상안받은경우가 있다면 알림표시
            if (AchieveList[i].Complete == 1 && AchieveList[i].Fetch == 0)
            {
                isAlram = true;
                break;
            }
        }
        if (!isAlram)
        {
            for (int i = 0; i < DailyAchieveList.Count; i++)
            {
                // 달성되있는데 보상안받은경우가 있다면 알림표시
                if (DailyAchieveList[i].Complete == 1 && DailyAchieveList[i].Fetch == 0)
                {
                    isAlram = true;
                    break;
                }
            }
        }

        if (!isAlram)
        {
            foreach (eAchieve_TAB_VIEW_TYPE index in AchievePoint.Keys)
            {
                if (AchievePoint[index] >= 10)
                {
                    isAlram = true;
                    break;
                }
            }
        }
        if (!isAlram)
        {
            Achievement.DailyRewardInfo pointInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementPointInof(DailyPointGrade);
            if (DailyPoint >= pointInfo.PointValue)
                isAlram = true;
        }

        SceneManager.instance.SetAlram(AlramIconType.ACHIEVE, isAlram);
        uiMgr.IsNextUiAchieve = false;

		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        base.Hide();
        //업적 알림팝업에서 넘어온경우 전의패널을 열어줘야함
        if (ReOpenPanel != null)
        {
            //if (UIMgr.instance.PrevPanel.ToString().Contains("Tower"))
            //    UIMgr.OpenTowerPanel();
            //else
            //    UIMgr.instance.PrevPanel.Show();
            ReOpenPanel.Show(ReOpenPanel.GetParams() );
        }
        else
            UIMgr.OpenTown();
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.F9))
        //{
        //    if (GetOneEffGameObj != null)
        //    {
        //        GameObject tmp = GetOneEffGameObj;

        //        UIHelper.CreateEffectInGame(tmp.transform, "Fx_UI_achieve_get_02");
        //        TweenTransform tween = tmp.transform.GetChild(0).GetComponentInChildren<TweenTransform>();
        //        if (tween != null)
        //        {
        //            tween.ResetToBeginning();
        //            tween.to = uiMgr.TopMenu.CategoryGo.transform;
        //            tween.PlayForward();
        //        }

        //        TempCoroutine.instance.FrameDelay(1f, () =>
        //        {
        //            if (tmp.transform.childCount > 0)
        //            {
        //                for (int i = 0; i < tmp.transform.childCount; i++)
        //                {
        //                    DestroyImmediate(tmp.transform.GetChild(i).gameObject);
        //                }
        //            }
        //        });

        //    }
           
        //}
    }


}

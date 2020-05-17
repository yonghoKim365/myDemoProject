using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BenefitPanel : UIBasePanel
{

    public enum eTAB_VIEW_TYPE
    {
        None = -1,
        DAILY = 0,//일일출석
        ACCESSTIME = 1,    //접속시간보상
        WEEK = 2,    //7일연속
        PAYBACK = 3,  //페이백
        LEVELUP = 4,   //레벨업
        GROW = 5, //성장
        MONTH = 6,  //월정액권
        NEWSERVER = 7,//신규서버오픈
    }
    private eTAB_VIEW_TYPE CurViewType = eTAB_VIEW_TYPE.None;//현재 보고있는 뷰 타입
    private eTAB_VIEW_TYPE PrevViewType = eTAB_VIEW_TYPE.None;//현재 보고있는 뷰 타입

    public GameObject TabGroup; //Tab버튼 관리하는 클래스


    public UIGrid Grid;
    public GameObject SlotPrefeb;
    public UIGrid AccessGrid; //접속시간 그리드
    public GameObject AccessSlotPrefeb;//접속시간슬롯 
    public UIGrid NewServerGrid;    //신썸보상
    public GameObject NewServerSlotPrefeb;
    public UIGrid LvGrid;//레벨업
    public GameObject LvSlotPrefab; //레벨업슬롯

    public GameObject PaybackRewardSlotPrefab;  //페이백 보상 슬롯프리펩

    public Transform ConsumerSlotRoot;  //페이백-누적소비 보상 부모
    public Transform TotalSlotRoot;  //페이백-누적충전 보상 부모
    public Transform DailySlotRoot;  //페이백-일간충전 보상 부모

    // 탭화면들
    public GameObject[] TabViewGO;   //
    public GameObject DailyCheckPopup;
    //public GameObject PaybackRewardPopup;   //페이백혜택에서 보상알려주는 팝업 잠시꺼두자
    //
    public GameObject ReturnPopup;

    public UILabel CurSignInCount;  //현재까지 출석한 횟수
    public UILabel GetCouponCount; //보유한 출석쿠폰
    public UILabel CanUseCouponCount;   //사용가능한 출석쿠폰
    public UILabel AccessTimeLabel; // 접속시간보상 라벨 

    public GameObject[] TabAlram;   //소분류탭 알림마크

    public UIButton SignInBtn;  //출첵버튼

    public UIButton BtnCharge;  //충전버튼 (따로뻇음)

    public UIButton BtnClose;

    public UISlider AccessGageBar;  //접속시간 게이지바 
    public Color AccessTimeLabelColor;
    public Transform AccessTimeLabelRoot;

    public UILabel WeekTimeLabel;   //연속출석일수표시
    public Transform SevenDaysSlotRoot; //7일연속 출석 슬롯부모
    public Transform PaybackSlotRoot;   //페이백 슬롯부모
    public UISprite SevenImg;   //7일차 상자이미지(얘만 바껴줘야하므로 따로빼줌)

    private float AccessCountDown = 0;    //로그인접속시간 카운트다운(초)
    private List<Welfare.WelfareInfo> TotalList = new List<Welfare.WelfareInfo>();
    private List<Welfare.WelfareInfo> DailyList = new List<Welfare.WelfareInfo>();
    private List<Welfare.WelfareInfo> ConsumerList = new List<Welfare.WelfareInfo>();

    private List<bool> alram = new List<bool>();

    private string TimeStr749;

    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("BenefitPanel, Init() start");
        base.Init();

        for(int i=0;i<5;i++)
        {
            alram.Add(false);
        }

        TotalList = _LowDataMgr.instance.GetLowDataWalfare(3); //누적충전 
        DailyList = _LowDataMgr.instance.GetLowDataWalfare(4); //일간 
        ConsumerList = _LowDataMgr.instance.GetLowDataWalfare(5); //일간 

        CreateSlot();   //
       // PaybackRewardPopup.SetActive(false);
        //TabGroup.Initialize(OnClickTab);
        for(int i=0;i<TabGroup.transform.childCount;i++)
        {
            UIButton btn = TabGroup.transform.GetChild(i).GetComponent<UIButton>();
            int idx = i;
            EventDelegate.Set(btn.onClick, delegate () {
                OnClickTab(idx);
            });
        }
        //충전은아직
        EventDelegate.Set(BtnCharge.onClick, delegate ()
        {
            UIMgr.instance.AddPopup(141, 174, 117);
        });
        EventDelegate.Set(SignInBtn.onClick, delegate () {
            SceneManager.instance.ShowNetProcess("SignIn");
            NetworkClient.instance.SendPMsgSignInC();
        });    //출첵버튼

        EventDelegate.Set(BtnClose.onClick, Close);
        OnClickTab(0);

        CheckTabAlram();

        TimeStr749 = _LowDataMgr.instance.GetStringCommon(749);

        SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("BenefitPanel, Init() finish");
    }

    public override void LateInit()
    {
        base.LateInit();

        if(SceneManager.instance.CurTutorial == TutorialType.BENEFIT)
            OnSubTutorial();
    }

    void CheckTabAlram()
    {
        //TabAlram
        for (int i = 0; i < TabAlram.Length; i++)
        {
            TabAlram[i].SetActive(SceneManager.instance.IsBenefitAlram(i) );
        }
    }

    /// <summary>
    /// 미리슬롯생성
    /// </summary>
    public void CreateSlot()
    {
        for (int i = 0; i < 32; i++)
        {
            GameObject slotGo = Instantiate(SlotPrefeb) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = Grid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
            slotGo.name = string.Format("{0}", i+1);
        }
       // Destroy(SlotPrefeb);

        for (int i = 0; i < 6; i++)
        {
            GameObject slotGo = Instantiate(AccessSlotPrefeb) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = AccessGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        Destroy(AccessSlotPrefeb);


        for (int i = 0; i < 20; i++)
        {
            GameObject slotGo = Instantiate(NewServerSlotPrefeb) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = NewServerGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        Destroy(NewServerSlotPrefeb);

        for (int i = 0; i < 10; i++)
        {
            GameObject slotGo = Instantiate(LvSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = LvGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        Destroy(LvSlotPrefab);

        for (int i = 0; i < TotalList.Count; i++)
        {
            GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = TotalSlotRoot.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        for (int i = 0; i < DailyList.Count; i++)
        {
            GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = DailySlotRoot.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
        for (int i = 0; i < ConsumerList.Count; i++)
        {
            GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = ConsumerSlotRoot.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }
    }

    public void ShowDailySignIn(uint info, uint time, uint fill)
    {
        DailyCheckPopup.SetActive(false);

        // 2진법으로 변환하믄 100001 <- 이런식으로댐
        // 예를들어 6일째이고(갯수, 0번째자리가 1일로침) 0이미출석 1이출석
        // 1일날 출석하고 6일날 출석 
        string dailySignInfo = "";
        List<string> checkDaily = new List<string>();

        int SignInCountInMonth = 0; // 현재 출첵카운트
        int CurDateCount = System.DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);     //현재 월이 몇일까지있나

        // 출첵첨함
        alram[(int)eTAB_VIEW_TYPE.DAILY] = false;

        if (info == 0)
        {
            for (int i = 0; i < DateTime.Now.Day - 1; i++)
            {
                checkDaily.Add("0");
                dailySignInfo += "0";
            }
        }
        else
        {
            dailySignInfo = Convert.ToString(info, 2);
            for (int i = dailySignInfo.Length - 1; i >= 0; i--)
            {
                checkDaily.Add(dailySignInfo.Substring(i, 1));
            }


            System.DateTime dateTime = System.DateTime.ParseExact(time.ToString(), "yyyyMMdd", null);
            if (dateTime != null && dateTime.Year == System.DateTime.Now.Year && dateTime.Month == System.DateTime.Now.Month
                && dateTime.Day == System.DateTime.Now.Day)
            {
                alram[(int)eTAB_VIEW_TYPE.DAILY] = false;
                //UIMgr.instance.BenefitAlram[0] = 0;
                SceneManager.instance.SetBenefitAlram(0, false);
            }
            else
            {
                alram[(int)eTAB_VIEW_TYPE.DAILY] = true;
                //UIMgr.instance.BenefitAlram[0] = 1;
                SceneManager.instance.SetBenefitAlram(0, true);

                // 오늘출첵안한것
                int idx = DateTime.Now.Day - checkDaily.Count - 1;
                for (int i = 0; i < idx; i++)
                {
                    checkDaily.Add("0");
                    dailySignInfo += "0";
                }
            }
        }

        TabAlram[0].SetActive(SceneManager.instance.IsBenefitAlram(0) );

        for (int i = 0; i < Grid.transform.childCount; i++)
        {
            GameObject slotGo = Grid.transform.GetChild(i).gameObject;
            Transform slotTf = slotGo.transform;
            int idx = i + 1;

            string date = "";
            date += DateTime.Now.Year;
            date += string.Format("{0:00}", DateTime.Now.Month);
            date += string.Format("{0:00}", idx);

            UILabel daybefore = slotTf.FindChild("Txt_daybefore").GetComponent<UILabel>();//앞으로날들
            UILabel dayafter = slotTf.FindChild("Txt_dayafter").GetComponent<UILabel>();// 지나간날들
            UILabel Cnt= slotTf.FindChild("price").GetComponent<UILabel>();

            daybefore.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), idx);
            dayafter.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), idx);

            daybefore.gameObject.SetActive(true);
            dayafter.gameObject.SetActive(true);

            GameObject stemp = slotTf.FindChild("Attend").gameObject;   //출석도장
            GameObject Nonattend = slotTf.FindChild("Nonattend").gameObject;   //결석도장

            UISprite itemImg = slotTf.FindChild("Icon").GetComponent<UISprite>();

            List<GatchaReward.FixedRewardInfo> list = null;
            if (idx <= CurDateCount)
            {
                uint rewardId = _LowDataMgr.instance.GetLowDataDailyRewardId(uint.Parse(date));
                list = _LowDataMgr.instance.GetFixedRewardItemGroupList(rewardId);
                if (list == null || list.Count <= 0)
                {
                    slotGo.SetActive(false);
                    continue;
                }

                SetRewardItemSlot(list[0], itemImg);
                Cnt.text = string.Format("x{0}", list[0].ItemCount);

            }

            slotGo.SetActive(true);
            //남은 일수 처리해줌
            if (i >= dailySignInfo.Length)
            {
                dayafter.gameObject.SetActive(false);

                // 남은 일수를 계산해준다 
                if (i < CurDateCount)
                {
                    // 한달안이면 미출석으로 처리해줌
                    stemp.SetActive(false);
                    Nonattend.SetActive(false);
                    slotGo.SetActive(true);
                    continue;
                }
                else
                {
                    //나머진 다끄기
                    slotGo.SetActive(false);
                    continue;
                }
            }

            if (checkDaily[i] != "1")    //결석처리
            {
                // 아이콘 회색처리해줘용
                stemp.SetActive(false);
                Nonattend.SetActive(true);
                // 결석처리된것은 출석채우기쿠폰을 사용해 출석을채울수있다.
                UIEventTrigger etri = slotTf.FindChild("Nonattend").GetComponent<UIEventTrigger>();
                EventDelegate.Set(etri.onClick, delegate ()
                {
                    if(list != null && 0 < list.Count)
                        DailyCheck(idx, 0, 0, date, Cnt.text, list[0]);
                    //Debug.Log(date);
                });
            }
            else    //출석했당
            {
                stemp.SetActive(true);
                Nonattend.SetActive(false);
                SignInCountInMonth++;
            }
        }

        CurSignInCount.text = string.Format(_LowDataMgr.instance.GetStringCommon(693), SignInCountInMonth);  //지금 출석횟수
        GetCouponCount.text = string.Format(_LowDataMgr.instance.GetStringCommon(694), 0);  //보유횟수
        CanUseCouponCount.text = string.Format(_LowDataMgr.instance.GetStringCommon(695), 0);   //몇개사용가능한지

        Grid.repositionNow=true;
    }

    // 출석채우기쿠폰 사용시
    void DailyCheck(int date, int CurCnt, int CanCnt, string fillDate, string cnt , GatchaReward.FixedRewardInfo itemInfo)
    {
        //몇일
        DailyCheckPopup.transform.FindChild("get").FindChild("Txt_Day").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(683), date);
        DailyCheckPopup.transform.FindChild("Txt_getcoupon").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(694), 0); //몇일 
        DailyCheckPopup.transform.FindChild("Txt_usecoupon").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(695), 0); ; //몇일 

        UISprite img = DailyCheckPopup.transform.FindChild("get").FindChild("Icon").GetComponent<UISprite>();
        SetRewardItemSlot(itemInfo, img);

        UILabel cntLabel = DailyCheckPopup.transform.FindChild("price").GetComponent<UILabel>();
        cntLabel.text = cnt;
        
        DailyCheckPopup.SetActive(true);

        UIButton close = DailyCheckPopup.transform.FindChild("Btn_cancel").GetComponent<UIButton>();
        UIButton fill = DailyCheckPopup.transform.FindChild("Btn_Ok").GetComponent<UIButton>();

        EventDelegate.Set(close.onClick, delegate () { DailyCheckPopup.SetActive(false); });
        EventDelegate.Set(fill.onClick, delegate ()
        {
            SceneManager.instance.ShowNetProcess("FillSignIn");
            NetworkClient.instance.SendPMsgFillInSignInC(uint.Parse(fillDate)); //yyyymmdd로 
        });
    }

    System.TimeSpan CheckAccessTime;
    System.DateTime accTime ;

    public UIButton CanGetBtn;
    void LateUpdate()
    {
        if (CanGetBtn == null)
            return;

        System.TimeSpan CheckAccessTime = accTime - System.DateTime.Now;
        // 초 단위로 온다.
        if (0 <= CheckAccessTime.TotalSeconds)
        {
            AccessTimeLabel.text = string.Format("{0} {1:00} : {2:00}", TimeStr749, CheckAccessTime.Minutes, CheckAccessTime.Seconds);
            //CanGetBtn.isEnabled = false;
        }
        else
        {
            AccessTimeLabel.text = string.Format("{0} {1:00} : {2:00}", TimeStr749, 0, 0);
            CanGetBtn.isEnabled = true;
            TabAlram[1].SetActive(true);

            AccessCountDown = 0;
        }
    }
    
    //접속시간 보상
    public void ShowAccessTime(uint lv, uint countDown)
    {
        TabViewGO[(int)eTAB_VIEW_TYPE.ACCESSTIME].transform.FindChild("ScrollView").GetComponent<UIScrollView>().enabled = false;

        alram[(int)eTAB_VIEW_TYPE.ACCESSTIME] = false;
        //StopCoroutine("EnableAccessBtn");
        List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(1);     //접속시간 보상

        float[] gage = { 0, 0.060f, 0.248f, 0.452f, 0.65f, 0.849f, 1f };
        /*
         lv -> 다음획든단계 (0이면 한번도 획득한적 없다는 의미)
         countDown -> 다음 보상까지의 카운트다운(초) 0이면 획득가능
         */
         
        if (AccessCountDown == 0)
            accTime = System.DateTime.Now.AddSeconds(countDown);
        AccessCountDown = countDown;

        AccessGageBar.value = gage[lv - 1];
        for (int i = 0; i < AccessTimeLabelRoot.transform.childCount; i++)
        {
            UILabel label = AccessTimeLabelRoot.transform.GetChild(i).gameObject.GetComponent<UILabel>();
            //시간
            uint reward = list[i].RewardCondition/* / 60*/;
            if (i > 0)
            {
                reward -= list[i - 1].RewardCondition;
                reward /= 60;
            }
            else
                reward /= 60;

            label.text = string.Format(_LowDataMgr.instance.GetStringCommon(260), reward/*list[i].RewardCondition / 60*/);
            label.color = (lv-1) > i ? AccessTimeLabelColor : Color.white;
        }

        bool isTutoSet = SceneManager.instance.CurTutorial == TutorialType.BENEFIT;
        TabAlram[1].SetActive(false);
        for (int i = 0; i < AccessGrid.transform.childCount; i++)
        {
            GameObject go = AccessGrid.transform.GetChild(i).gameObject;
            go.SetActive(true);


            UIButton CanGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>(); // 보상받기 활성화상태
            UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>(); // 비활성화
            GameObject black = go.transform.FindChild("soldout").gameObject;   //검은화면 (수령완료시에 나온다)
            GameObject Stemp = go.transform.FindChild("Getmark").gameObject;//출석완료
            Stemp.SetActive(false);
            uint idx = (uint)i + 1;
            EventDelegate.Set(CanGet.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("GetAccessWelfare");
                NetworkClient.instance.SendPMsgWelfareOnlineFetchRewardC(idx);
            });   //보상받기


            //보상시간
            UILabel time = go.transform.FindChild("time").GetComponent<UILabel>();
            //보상아이콘
            UISprite itemImg = go.transform.FindChild("icon").GetComponent<UISprite>();
            //보상설명
            UILabel itemName = go.transform.FindChild("price").GetComponent<UILabel>();
            List<GatchaReward.FixedRewardInfo> fixList = _LowDataMgr.instance.GetFixedRewardItemGroupList(list[i].RewardId);

            // 접속유지보상을 다 달성한경우 
            if (lv > list.Count)
            {
                //수령완료
                Stemp.SetActive(true);
                CanGet.transform.gameObject.SetActive(false);
                NotGet.transform.gameObject.SetActive(true);
                
                black.SetActive(false);

                uint reward = list[i].RewardCondition/* / 60*/;
                if (i > 0)
                {
                    reward -= list[i - 1].RewardCondition;
                    reward /= 60;
                }
                else
                    reward /= 60;

                time.text = string.Format(_LowDataMgr.instance.GetStringCommon(260), reward);
                SetRewardItemSlot(fixList[0], itemImg, itemName);
                continue;
            }
            
            if (i < lv)
            {
                if (lv - 1 == i)
                {
                    // 이떄 버튼 활성화상태] 
                    Stemp.SetActive(false);
                    black.SetActive(false);
                    CanGet.transform.gameObject.SetActive(true);
                    NotGet.transform.gameObject.SetActive(false);

                    if(isTutoSet)
                    {
                        isTutoSet = false;
                        TutorialSupport tutoSupport = CanGet.gameObject.AddComponent<TutorialSupport>();
                        tutoSupport.TutoType = TutorialType.BENEFIT;
                        tutoSupport.SortId = 4;
                        tutoSupport.IsScroll = true;
                        tutoSupport.NextTuto = BtnClose.gameObject.GetComponent<TutorialSupport>();

                        tutoSupport.OnTutoSupportStart();
                    }

                    CanGetBtn = CanGet;
                    if (CheckAccessTime.TotalSeconds <= 0)
                        CanGet.isEnabled = false;
                    else
                        CanGet.isEnabled = true;
                }
                else
                {
                    //수령완료
                    Stemp.SetActive(true);
                    CanGet.transform.gameObject.SetActive(false);
                    NotGet.transform.gameObject.SetActive(true);
                    black.SetActive(false);
                }
            }
            else
            {
                // 수령 예정상태 
                CanGet.transform.gameObject.SetActive(false);
                NotGet.transform.gameObject.SetActive(true);
                
                Stemp.SetActive(false);
                black.SetActive(false);
            }
            
            uint rewardTime = list[i].RewardCondition;
            if (i > 0)
            {
                rewardTime -= list[i - 1].RewardCondition;
                rewardTime /= 60;
            }
            else
                rewardTime /= 60;

            time.text = string.Format(_LowDataMgr.instance.GetStringCommon(260), rewardTime);
            SetRewardItemSlot(fixList[0], itemImg, itemName);
        }

        AccessGrid.Reposition();
    }

    //7일연속보상
    public void Show7Days(uint info, uint day)
    {
        /*
         day = 이미 누적된일수
         info = 보상획득정보 <- 2진수로옴 

        처음출석은 1, 0 으로왓슴
         */
        CheckTabAlram();

        List<Welfare.EventCheckInfo> list = _LowDataMgr.instance.GetLowDataEventCheck(1);     //7일연속보상

        uint dayCheck = day;
        int check = 0;
        if (info == 0) //아직 출석 0
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);//2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }
        alram[(int)eTAB_VIEW_TYPE.WEEK] = false;
        WeekTimeLabel.text = string.Format(_LowDataMgr.instance.GetStringCommon(751), check);
        for (int i = 0; i < SevenDaysSlotRoot.childCount; i++)
        {
            GameObject go = SevenDaysSlotRoot.GetChild(i).gameObject;

            GameObject stemp = go.transform.FindChild("Attend").gameObject;  //수령스탬프
            UILabel reward = go.transform.FindChild("price").GetComponent<UILabel>();   //수령아이템정보
            UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
            UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
            GameObject black = go.transform.FindChild("getcover").gameObject;    //검정색판


            UILabel daybefore = go.transform.FindChild("Txt_daybefore").GetComponent<UILabel>();//앞으로날들
            UILabel dayafter = go.transform.FindChild("Txt_dayafter").GetComponent<UILabel>();// 지나간날들

            daybefore.gameObject.SetActive(true);
            dayafter.gameObject.SetActive(true);

            SevenImg.spriteName = "Img_SprBox02";
            int idx = i;
            EventDelegate.Set(canGet.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("GetWeekWelfare");
                NetworkClient.instance.SendPMsgWelfareFetchXDayRewardC((uint)idx + 1);
            });   //보상받기

            if(idx != SevenDaysSlotRoot.childCount-1)
            {
                daybefore.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), idx + 1); //1일2일
                dayafter.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), idx + 1); //1일2일
            }

            List<GatchaReward.FixedRewardInfo> fixList = _LowDataMgr.instance.GetFixedRewardItemGroupList(list[i].RewardId);    //  보상 

            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(fixList[0].ItemId);
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(eLowData.NameId), fixList[0].ItemCount);
            }
            else //소모성
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(fixList[0].ItemId);
                if (uLowData == null)
                {
                    if (list[0].Type == 1)//골드 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(4), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 2)//원보 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(3), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 10)  //파트너
                    {
                        if (_LowDataMgr.instance.IsGetRewardType(10, fixList[0].ItemId))
                        {
                            reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringUnit(_LowDataMgr.instance.GetPartnerInfo(fixList[0].ItemId).NameId), fixList[0].ItemCount);
                        }
                    }
                }
                else
                {
                    reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(uLowData.NameId), fixList[0].ItemCount);
                }
            }
            
            if (i < check)  //수령완료
            {
                daybefore.gameObject.SetActive(false);
                stemp.SetActive(true);
                canGet.transform.gameObject.SetActive(false);
                NotGet.transform.gameObject.SetActive(true);
                black.SetActive(false);

                if (i == 6)    //막날
                    SevenImg.spriteName = "Img_SprBox01";
            }
            else
            {
                if (i == check)
                {
                    if (dayCheck == check)
                    {
                        daybefore.gameObject.SetActive(false);

                        stemp.SetActive(false);
                        canGet.transform.gameObject.SetActive(true);
                        NotGet.transform.gameObject.SetActive(false);
                        black.SetActive(true);

                        if (i == 6)    //막날
                            SevenImg.spriteName = "Img_SprBox02";
                    }
                    else
                    {
                        // 이때 버튼활성화 
                        daybefore.gameObject.SetActive(true);//자시
                        stemp.SetActive(false);
                        canGet.transform.gameObject.SetActive(true);
                        NotGet.transform.gameObject.SetActive(false);
                        black.SetActive(false);
                        alram[(int)eTAB_VIEW_TYPE.WEEK] = true;

                        if (i == 6)    //막날
                            SevenImg.spriteName = "Img_SprBox01";
                    }
                }
                else
                {
                    //보상대기 
                    // 이때 버튼활성화 
                    stemp.SetActive(false);
                    canGet.transform.gameObject.SetActive(true);
                    NotGet.transform.gameObject.SetActive(false);

                    canGet.collider.enabled = true;
                    if (i == 6)    //막날
                        SevenImg.spriteName = "Img_SprBox02";

                    black.SetActive(true);
                }

            }
        }
    }

    //신썹보상
    private uint newServerDay = 0;

    public void ShowNewServer(uint info, uint day)
    {
        CheckTabAlram();
        /*
        day = 이미 누적된일수
        info = 보상획득정보 <- 2진수로옴 

       처음출석은 1, 0 으로왓슴
        */
        List<Welfare.EventCheckInfo> list = _LowDataMgr.instance.GetLowDataEventCheck(2);     //신썹

        UILabel EventString = TabViewGO[(int)eTAB_VIEW_TYPE.NEWSERVER].transform.FindChild("Txt_event").GetComponent<UILabel>();//이벤트기간

        uint start = _LowDataMgr.instance.GetLowDataServerEvent(1, 1).EventStart;
        uint end = _LowDataMgr.instance.GetLowDataServerEvent(1, 1).EventEnd;

        EventString.text = string.Format("{0} ~ {1}", start, end);

        //bool IsEventTime = false;   //현재 이벤트기간인가 체크변수

        DateTime Now = DateTime.Now;
        DateTime Start = DateTime.ParseExact(start.ToString(), "yyyyMMdd", null);
        DateTime End = DateTime.ParseExact(end.ToString(), "yyyyMMdd", null);

        //if (Start > Now)
        //{
        //    IsEventTime = false;
        //}
        //else
        //{
        //    if (End > Now)
        //        IsEventTime = true;
        //}

        uint dayCheck = day;
        newServerDay = dayCheck;
        int check = 0;
        if (info == 0)//아직 한번도 안함
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }

        for (int i = 0; i < NewServerGrid.transform.childCount; i++)
        {
            GameObject go = NewServerGrid.transform.GetChild(i).gameObject;

            if (i >=list.Count)
            {
                go.SetActive(false);
                continue;
            }
            
            go.SetActive(true);
            GameObject stemp = go.transform.FindChild("Attend").gameObject;  //수령스탬프
            UILabel reward = go.transform.FindChild("price").GetComponent<UILabel>();   //수령아이템정보
            UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
            UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
            GameObject black = go.transform.FindChild("getcover").gameObject;    //검정색판
            
            UILabel daybefore = go.transform.FindChild("Txt_daybefore").GetComponent<UILabel>();//앞으로날들
            UILabel dayafter = go.transform.FindChild("Txt_dayafter").GetComponent<UILabel>();// 지나간날들

            daybefore.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), i + 1); //1일2일
            dayafter.text = string.Format(_LowDataMgr.instance.GetStringCommon(683), i + 1); //1일2일

            daybefore.gameObject.SetActive(true);
            dayafter.gameObject.SetActive(true);

            int idx = i;
            EventDelegate.Set(canGet.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("GetNewSvrWelfare");
                NetworkClient.instance.SendPMsgWelfareOpenSvrFetchRewardC((uint)idx + 1);
            });   //보상받기

            List<GatchaReward.FixedRewardInfo> fixList = _LowDataMgr.instance.GetFixedRewardItemGroupList(list[i].RewardId);    //  보상 

            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(fixList[0].ItemId);
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(eLowData.NameId), fixList[0].ItemCount);
            }
            else //소모성
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(fixList[0].ItemId);
                if (uLowData == null)
                {
                    if (list[0].Type == 1)//골드 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(4), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 2)//원보 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(3), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 10)  //파트너
                    {
                        if (_LowDataMgr.instance.IsGetRewardType(10, fixList[0].ItemId))
                        {
                            reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringUnit(_LowDataMgr.instance.GetPartnerInfo(fixList[0].ItemId).NameId), fixList[0].ItemCount);
                        }
                    }
                }
                else
                {
                    reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(uLowData.NameId), fixList[0].ItemCount);
                }
            }

            if (i < check)  //수령완료
            {
                daybefore.gameObject.SetActive(false);
                stemp.SetActive(true);
                canGet.transform.gameObject.SetActive(false);
                NotGet.transform.gameObject.SetActive(true);
                black.SetActive(false);
            }
            else
            {
                if (i == check)
                {
                    if (dayCheck == check)
                    {
                        stemp.SetActive(false);
                        canGet.transform.gameObject.SetActive(true);
                        NotGet.transform.gameObject.SetActive(false);
                        black.SetActive(true);
                    }
                    else
                    {
                        // 이때 버튼활성화 
                        stemp.SetActive(false);
                        canGet.transform.gameObject.SetActive(true);
                        NotGet.transform.gameObject.SetActive(false);
                        black.SetActive(false);
                    }
                }
                else
                {
                    //보상대기 
                    // 이때 버튼활성화 
                    stemp.SetActive(false);
                    canGet.transform.gameObject.SetActive(true);
                    NotGet.transform.gameObject.SetActive(false);
                    canGet.collider.enabled = true;
                    black.SetActive(true);
                }
            }
        }

        NewServerGrid.Reposition();
    }



    //레벨업 패키지 
    public void ShowLevelUp(uint info)
    {
        /*
         * info = 보상획득상대 (2진법)
         */

        CheckTabAlram();
        List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(2); //레벨업보상 

        uint check = 0;  //보상받은회수 
        if (info == 0)   //한번도 안받음
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }

        alram[(int)eTAB_VIEW_TYPE.LEVELUP] = false;

        for (int i = 0; i < LvGrid.transform.childCount; i++)
        {
            GameObject go = LvGrid.transform.GetChild(i).gameObject;
            Transform tf = go.transform;

            if (i >= list.Count)
            {
                go.SetActive(false);
                continue;
            }

            go.SetActive(true);
            //GameObject stemp = tf.FindChild("Stemp").gameObject;    //스템프
            UILabel Lv = tf.FindChild("Txt_Lv").GetComponent<UILabel>();    //레벨
            UILabel reward = tf.FindChild("Txt_price").GetComponent<UILabel>();//보상
            UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
            UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
            GameObject black = go.transform.FindChild("soldout").gameObject;    //검정색판

            Lv.text = string.Format("{0} {1}",_LowDataMgr.instance.GetStringCommon(14), list[i].RewardCondition);

            int idx = i;
            EventDelegate.Set(canGet.onClick, delegate ()
            {
                SceneManager.instance.ShowNetProcess("GetLvWelfare");
                NetworkClient.instance.SendPMsgWelfareFetchRoleUpgradeRewardC((uint)idx + 1);
            });   //보상받기


            List<GatchaReward.FixedRewardInfo> fixList = _LowDataMgr.instance.GetFixedRewardItemGroupList(list[i].RewardId);    //  보상 
            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(fixList[0].ItemId);
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(eLowData.NameId), fixList[0].ItemCount);
            }
            else //소모성
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(fixList[0].ItemId);
                if (uLowData == null)
                {
                    if (list[0].Type == 1)//골드 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(4), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 2)//원보 
                    {
                        reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(3), fixList[0].ItemCount);
                    }
                    if (list[0].Type == 10)  //파트너
                    {
                        if (_LowDataMgr.instance.IsGetRewardType(10, fixList[0].ItemId))
                            reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringUnit(_LowDataMgr.instance.GetPartnerInfo(fixList[0].ItemId).NameId), fixList[0].ItemCount);
                    }
                }
                else
                {
                    reward.text = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(uLowData.NameId), fixList[0].ItemCount);
                }
            }
            //// 보상 다 달성한경우 
            //if (info > list.Count)
            //{
            //    canGet.transform.gameObject.SetActive(false);
            //    NotGet.transform.gameObject.SetActive(true);
            //    NotGet.transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1016);
            //    black.SetActive(false);
            //    continue;
            //}
            
            if (i < check)  //수령완료
            {
                //stemp.SetActive(true);
                canGet.transform.gameObject.SetActive(false);
                NotGet.transform.gameObject.SetActive(true);
                NotGet.transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1016);
                black.SetActive(false);
            }
            else
            {
                if (i == check)
                {
                    //stemp.SetActive(false);
                    canGet.transform.gameObject.SetActive(true);
                    canGet.isEnabled = false;

                    NotGet.transform.gameObject.SetActive(false);
                    black.SetActive(true);
                    canGet.transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1017);
                    if (NetData.instance.UserLevel >= list[i].RewardCondition)    //레벨이달성됬으면 버튼활성화 
                    {
                        canGet.isEnabled = true;

                        black.SetActive(false);
                        alram[(int)eTAB_VIEW_TYPE.LEVELUP] = true;
                        canGet.transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1016);
                    }
                }
                else
                {
                    //보상대기 
                    // 이때 버튼활성화 
                    canGet.isEnabled = false;
                    canGet.transform.gameObject.SetActive(true);
                    canGet.transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(1017);

                    NotGet.transform.gameObject.SetActive(false);
                    canGet.collider.enabled = true;
                    black.SetActive(true);
                }
            }
            
        }

        LvGrid.Reposition();
    }

    // 누적충전 보상
    public void ShowRechargeTotal(ulong total, uint info, uint canFetchInfo, bool IsFinish = false)
    {
        /*
         total = 결제금액의 총누적치
         info = 충전보상 획득상황 (2진법)
         canFecthInfo = 획득가능단계(2진법)
         */

        //List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(3); //누적충전 

        CheckTabAlram();

        GameObject go = PaybackSlotRoot.GetChild(0).gameObject; //누적충전보상 
        UILabel rewardIdx = go.transform.FindChild("Txt_num").GetComponent<UILabel>();  //수령한보상 {0}단계
        UILabel nextReward = go.transform.FindChild("Txt_step").GetComponent<UILabel>();    //다음보상까지 {0}남음
        UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
        UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
        UISprite gage = go.transform.FindChild("gageback/gagebar").GetComponent<UISprite>();
        
        int check = 0;
        if (canFetchInfo == 0)// 획득가능한단계 
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(canFetchInfo, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }

        EventDelegate.Set(canGet.onClick, delegate () {
            SceneManager.instance.ShowNetProcess("GetRechargeTotal");
            NetworkClient.instance.SendPMsgRechargeTotalFetchRewardC((uint)check);
        });
        
        if (check == 0)  //획득가능한보상없으
        {
            canGet.transform.gameObject.SetActive(false);
            NotGet.transform.gameObject.SetActive(true);
        }
        else
        {
            if (TotalList[check - 1].RewardCondition <= total)
            {
                canGet.transform.gameObject.SetActive(true);
                NotGet.transform.gameObject.SetActive(false);
            }
        }

        int rewardLv = 0;
        if (info == 0)// 획득가능한단계 
        {
            rewardLv = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    rewardLv++;
            }
        }

        gage.fillAmount = (float)rewardLv / (float)TotalList.Count;
        rewardIdx.text = string.Format("{0}{1}", rewardLv.ToString(), _LowDataMgr.instance.GetStringCommon(710));
        ulong next = info == TotalList.Count ? 0 : TotalList[(int)info].RewardCondition - total;
        nextReward.text = string.Format(_LowDataMgr.instance.GetStringCommon(767), next);

        //보상수만큼 만들어준다
        //for (int i = 0; i < TotalList.Count; i++)
        //{
        //    GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
        //    Transform slotTf = slotGo.transform;
        //    slotTf.parent = TotalSlotRoot.transform;
        //    slotTf.localPosition = Vector3.zero;
        //    slotTf.localScale = Vector3.one;
        //    slotGo.SetActive(false);
        //}

        int bar = (int)gage.localSize.x / TotalList.Count;

        for (int i = 0; i < TotalSlotRoot.transform.childCount; i++)
        {

            GameObject slot = TotalSlotRoot.transform.GetChild(i).gameObject;
            slot.transform.localPosition = new Vector3(bar * (i + 1), 50, 0);
            slot.SetActive(true);

            GameObject stemp = slot.transform.FindChild("Stemp").gameObject;  //수령스탬프
            UIEventTrigger etri = slot.transform.FindChild("deco").GetComponent<UIEventTrigger>();
            UISprite box = etri.GetComponent<UISprite>();

            int idx = i;
            EventDelegate.Set(etri.onClick, delegate ()
            {
                GatchaReward.FixedRewardInfo FInfo = _LowDataMgr.instance.GetFixedRewardItemGroupList(TotalList[idx].RewardId)[0];
            });
            if (i < rewardLv)  //수령완료
            {
                stemp.SetActive(true);
                box.spriteName = "Img_NorBox02";
            }
            else
            {
                stemp.SetActive(false);
                box.spriteName = "Img_NorBox01";
            }
        }

        //// 이벤트기간이아닐때 암막?처리
        //GameObject Block = go.transform.FindChild("block").gameObject;
        //Block.SetActive(IsFinish);
    }

    // 일간충전 보상
    public void ShowRechargeDaily(ulong total, uint info, uint canFetchInfo)
    {
        /*
         total = 결제금액의 총누적치
         info = 충전보상 획득상황 (2진법)
         canFecthInfo = 획득가능단계(2진법)
         */

        //  List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(4); //일간 
        CheckTabAlram();
        
        GameObject go = PaybackSlotRoot.GetChild(1).gameObject; //누적충전보상 
        UILabel rewardIdx = go.transform.FindChild("Txt_num").GetComponent<UILabel>();  //수령한보상 {0}단계
        UILabel nextReward = go.transform.FindChild("Txt_step").GetComponent<UILabel>();    //다음보상까지 {0}남음
        UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
        UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
        UISlider gage = go.transform.FindChild("gageback").GetComponent<UISlider>();
        
        int check = 0;
        if (canFetchInfo == 0)// 획득가능한단계 
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(canFetchInfo, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }

        EventDelegate.Set(canGet.onClick, delegate () {
            SceneManager.instance.ShowNetProcess("GetDailyTotal");
            NetworkClient.instance.SendPMsgRechargeTotalFetchRewardC((uint)check); });



        if (check == 0)  //획득가능한보상없으
        {
            canGet.transform.gameObject.SetActive(false);
            NotGet.transform.gameObject.SetActive(true);
        }
        else
        {
            if (DailyList[check - 1].RewardCondition <= total)
            {
                canGet.transform.gameObject.SetActive(true);
                NotGet.transform.gameObject.SetActive(false);
            }
        }

        int rewardLv = 0;
        if (info == 0)// 획득가능한단계 
        {
            rewardLv = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    rewardLv++;
            }
        }

        gage.value = (float)rewardLv / (float)DailyList.Count;
        rewardIdx.text = string.Format("{0}{1}", rewardLv.ToString(), _LowDataMgr.instance.GetStringCommon(710));
        ulong next = info == DailyList.Count ? 0 : DailyList[(int)info].RewardCondition - total;
        nextReward.text = string.Format(_LowDataMgr.instance.GetStringCommon(767), next);

        ////보상수만큼 만들어준다
        //for (int i = 0; i < DailyList.Count; i++)
        //{
        //    GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
        //    Transform slotTf = slotGo.transform;
        //    slotTf.parent = DailySlotRoot.transform;
        //    slotTf.localPosition = Vector3.zero;
        //    slotTf.localScale = Vector3.one;
        //    slotGo.SetActive(false);
        //}

        int bar = (int)gage.transform.GetChild(0).GetComponent<UISprite>().localSize.x / DailyList.Count;

        for (int i = 0; i < DailySlotRoot.transform.childCount; i++)
        {

            GameObject slot = DailySlotRoot.transform.GetChild(i).gameObject;
            slot.transform.localPosition = /*ConsumerSlotRoot.localPosition +*/ new Vector3(bar * (i + 1), 50, 0);
            slot.SetActive(true);

            GameObject stemp = slot.transform.FindChild("Stemp").gameObject;  //수령스탬프
            UIEventTrigger etri = slot.transform.FindChild("deco").GetComponent<UIEventTrigger>();
            UISprite box = etri.GetComponent<UISprite>();
            int idx = i;
            EventDelegate.Set(etri.onClick, delegate ()
            {
                GatchaReward.FixedRewardInfo FInfo = _LowDataMgr.instance.GetFixedRewardItemGroupList(DailyList[idx].RewardId)[0];
               // SetPaybackRewardPopup(FInfo);
            });
            if (i < rewardLv)  //수령완료
            {
                stemp.SetActive(true);
                box.spriteName = "Img_NorBox02";
            }
            else
            {
                stemp.SetActive(false);
                box.spriteName = "Img_NorBox01";
            }

        }
    }
    // 누적소비 보상
    public void ShowRechargeConsumer(ulong total, uint info, uint canFetchInfo)
    {
        /*
         total = 결제금액의 총누적치
         info = 충전보상 획득상황 (2진법)
         canFecthInfo = 획득가능단계(2진법)
         */

        //List<Welfare.WelfareInfo> list = _LowDataMgr.instance.GetLowDataWalfare(5); //레벨업보상 
        CheckTabAlram();
        
        GameObject go = PaybackSlotRoot.GetChild(2).gameObject; //누적충전보상 
        UILabel rewardIdx = go.transform.FindChild("Txt_num").GetComponent<UILabel>();  //수령한보상 {0}단계
        UILabel nextReward = go.transform.FindChild("Txt_step").GetComponent<UILabel>();    //다음보상까지 {0}남음
        UIButton canGet = go.transform.FindChild("btn_notyet").GetComponent<UIButton>();    //보상받기버튼
        UIButton NotGet = go.transform.FindChild("btn_get").GetComponent<UIButton>();    //수령완료버튼
        UISlider gage = go.transform.FindChild("gageback").GetComponent<UISlider>();
        
        int check = 0;
        if (canFetchInfo == 0)// 획득가능한단계 
        {
            check = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(canFetchInfo, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    check++;
            }
        }

        EventDelegate.Set(canGet.onClick, delegate () {
            SceneManager.instance.ShowNetProcess("GetConsumerTotal");
            NetworkClient.instance.SendPMsgRechargeConsumerFetchRewardC((uint)check); });
        
        if (check == 0)  //획득가능한보상없으
        {
            canGet.transform.gameObject.SetActive(false);
            NotGet.transform.gameObject.SetActive(true);
        }
        else
        {
            if (ConsumerList[check - 1].RewardCondition <= total)
            {
                canGet.transform.gameObject.SetActive(true);
                NotGet.transform.gameObject.SetActive(false);
            }
        }

        int rewardLv = 0;
        if (info == 0)// 획득가능한단계 
        {
            rewardLv = 0;
        }
        else
        {
            string dayCnt = Convert.ToString(info, 2);  //2진수로옴
            for (int i = 0; i < dayCnt.Length; i++)
            {
                if (dayCnt.Substring(i, 1) == "1")
                    rewardLv++;
            }
        }

        gage.value = (float)rewardLv / (float)ConsumerList.Count;
        rewardIdx.text = string.Format("{0}{1}", rewardLv.ToString(), _LowDataMgr.instance.GetStringCommon(710));
        ulong next = info == ConsumerList.Count ? 0 : ConsumerList[(int)info].RewardCondition - total;
        nextReward.text = string.Format(_LowDataMgr.instance.GetStringCommon(767), next);

        ////보상수만큼 만들어준다
        //for (int i = 0; i < ConsumerList.Count; i++)
        //{
        //    GameObject slotGo = Instantiate(PaybackRewardSlotPrefab) as GameObject;
        //    Transform slotTf = slotGo.transform;
        //    slotTf.parent = ConsumerSlotRoot.transform;
        //    slotTf.localPosition = Vector3.zero;
        //    slotTf.localScale = Vector3.one;
        //    slotGo.SetActive(false);
        //}

        int bar = (int)gage.transform.GetChild(0).GetComponent<UISprite>().localSize.x / ConsumerList.Count;

        for (int i = 0; i < ConsumerSlotRoot.transform.childCount; i++)
        {

            GameObject slot = ConsumerSlotRoot.transform.GetChild(i).gameObject;
            slot.transform.localPosition = /*ConsumerSlotRoot.localPosition +*/ new Vector3(bar * (i + 1), 50, 0);
            slot.SetActive(true);

            UIEventTrigger etri = slot.transform.FindChild("deco").GetComponent<UIEventTrigger>();
            UISprite box = etri.GetComponent<UISprite>();
            int idx = i;
            EventDelegate.Set(etri.onClick, delegate ()
            {
                GatchaReward.FixedRewardInfo FInfo = _LowDataMgr.instance.GetFixedRewardItemGroupList(ConsumerList[idx].RewardId)[0];

                //SetPaybackRewardPopup(FInfo);
            });

            GameObject stemp = slot.transform.FindChild("Stemp").gameObject;  //수령스탬프

            if (i < rewardLv)  //수령완료
            {
                stemp.SetActive(true);
                box.spriteName = "Img_NorBox02";
            }
            else
            {
                stemp.SetActive(false);
                box.spriteName = "Img_NorBox01";
            }

        }
    }

    //public void HidePaybackRewrdPopup()
    //{
    //    PaybackRewardPopup.SetActive(false);

    //}

    ///// <summary>
    ///// 페이백 혜택에서 아이템
    ///// </summary>
    ///// <param name="info"></param>
    //void SetPaybackRewardPopup(GatchaReward.FixedRewardInfo info)
    //{
    //    PaybackRewardPopup.SetActive(true);

    //    UISprite itemImg = PaybackRewardPopup.transform.FindChild("slot01").FindChild("Icon").GetComponent<UISprite>();
    //    UILabel amount = PaybackRewardPopup.transform.FindChild("slot01").FindChild("Txt_gold").GetComponent<UILabel>();

    //    amount.text = info.ItemCount.ToString();

    //    UIEventTrigger triItem = itemImg.transform.GetComponent<UIEventTrigger>();
    //    EventDelegate.Set(triItem.onClick, delegate () { OnclicItemPopup(itemImg.transform, info.ItemId); });

    //    SetRewardItemSlot(info, itemImg);

    //    //Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(info.ItemId);
    //    //if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
    //    //{
    //    //    if (itemImg.atlas != EquipAtlas)
    //    //        itemImg.atlas = EquipAtlas;

    //    //    itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
    //    //}
    //    //else//소모아이템이 드랍아이템 대표로 등록되어 있음
    //    //{
    //    //    Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(info.ItemId);
    //    //    if (uLowData == null)
    //    //    {
    //    //        //Debug.LogError("item id error" + list[0].ItemId);
    //    //        //따로체크 
    //    //        if (itemImg.atlas != ItemAtlas)
    //    //            itemImg.atlas = ItemAtlas;

    //    //        if (info.Type == 1)//골드 
    //    //        {
    //    //            itemImg.spriteName = "Icon_10000";
    //    //        }
    //    //        if (info.Type == 2)//원보 
    //    //        {


    //    //            itemImg.spriteName = "Icon_10001";
    //    //        }
    //    //        if (info.Type == 10)  //파트너
    //    //        {
    //    //            if (_LowDataMgr.instance.IsGetRewardType(10, info.ItemId))
    //    //            {
    //    //                if (itemImg.atlas != Face)
    //    //                    itemImg.atlas = Face;

    //    //                itemImg.spriteName = _LowDataMgr.instance.GetPartnerInfo(info.ItemId).PortraitId;
    //    //            }
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (itemImg.atlas != ItemAtlas)
    //    //            itemImg.atlas = ItemAtlas;

    //    //        itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);
    //    //    }

    //    //}
    //}

    //보상 아이템정보 (가챠리워드테이블에서..)
    public void SetRewardItemSlot(GatchaReward.FixedRewardInfo itemInfo, UISprite itemImg, UILabel itemName = null)
    {
        string iconName = "";
        string name = "";

        int idx = 0;
        switch ((Sw.UNITE_TYPE)itemInfo.Type)
        {
            case Sw.UNITE_TYPE.UNITE_TYPE_COIN:
                iconName = "money";
                idx = 599000;
                name = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(4), itemInfo.ItemCount);
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_GEM:
                iconName = "cash";
                idx = 599001;
                name = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringCommon(3), itemInfo.ItemCount);
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_CONTRIBUTION:
                iconName = "Icon_10006";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_HONOR:
                iconName = "honor";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROYAL_BADGE:
                idx = 599002;
                iconName = "Img_flag2";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_LION_KING_BADGE:
                iconName = "Icon_10008";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_FAME://성망
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_POWER:
                iconName = "Img_ap";
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_ROLE_EXP: // 다시확인
                iconName = "ap_icon";   //일단임시로넣어둠
                break;
            case Sw.UNITE_TYPE.UNITE_TYPE_TITLE:
                iconName = "badge";
                break;
            default:
                break;
        }

        if (iconName == "")
        {
            if (itemName != null)
                SetRewardItem(itemInfo.ItemId, itemInfo.ItemCount, itemImg, itemName);
            else
                SetRewardItem(itemInfo.ItemId, itemInfo.ItemCount, itemImg);
        }
        else
        {
            if (itemName != null && name != "")
            {
                itemName.text = name;
            }

            itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Img);
            itemImg.spriteName = iconName;

            UISprite bg = itemImg.transform.FindChild("grade_bg").GetComponent<UISprite>();
            bg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);//Bod;
            bg.spriteName = "Bod_IconBg";
            UISprite grade = itemImg.transform.FindChild("grade").GetComponent<UISprite>();
            grade.spriteName = "Icon_01";

            if (idx != 0)
            {
                EventDelegate.Set(itemImg.GetComponent<UIEventTrigger>().onClick, delegate ()
                {
                    OnclicItemPopup((uint)idx);
                });
            }
        }
    }

    // 보상 아이템정보 
    void SetRewardItem(uint itemIdx, uint itemCount, UISprite itemImg, UILabel itemName = null)
    {
        string name = "";
        UISprite bg = itemImg.transform.FindChild("grade_bg").GetComponent<UISprite>();
        UISprite grade = itemImg.transform.FindChild("grade").GetComponent<UISprite>();

        BoxCollider bx = itemImg.transform.GetComponent<BoxCollider>();
        bx.enabled = true;
        Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx);
        if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
        {
            itemImg.atlas = AtlasMgr.instance.GetEquipAtlasForClassId(eLowData.Class); //atlas;
            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);

            name = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(eLowData.NameId), itemCount);

            bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);//EquipAtlas;
            bg.spriteName = string.Format("Icon_bg_0{0}", eLowData.Grade);

            grade.spriteName = string.Format("Icon_0{0}", eLowData.Grade);
        }
        else//소모아이템이 드랍아이템 대표로 등록되어 있음
        {
            Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(itemIdx);
            if (uLowData == null)
            {
                Partner.PartnerDataInfo partner = _LowDataMgr.instance.GetPartnerInfo(itemIdx);
                if(partner!=null)
                {
                    itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);

                    itemImg.spriteName = partner.PortraitId;
                    name = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringUnit(partner.NameId), itemCount);
                    if (itemName != null)
                        itemName.text = name;

                    bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);//EquipAtlas;
                    bg.spriteName = string.Format("Icon_bg_0{0}", partner.Quality);

                    grade.spriteName = string.Format("Icon_0{0}", partner.Quality);
                    bx.enabled = false;
                }

                return;
            }

            if (uLowData.Type == (byte)ItemType.Costum || uLowData.Type == (byte)ItemType.Partner)
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
            else
                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.UseItem);

            itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);
            name = string.Format("{0} x{1}", _LowDataMgr.instance.GetStringItem(uLowData.NameId), itemCount);
            grade.spriteName = string.Format("Icon_0{0}", uLowData.Grade);

            if (uLowData.Type == (int)AssetType.Jewel)
            {
                    bg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Item);//EquipAtlas;

                bg.spriteName = string.Format("Icon_bg_0{0}", uLowData.Grade);
            }
            else
            {
                bg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);

                bg.spriteName = "Bod_IconBg";
            }

        }
        if (itemName != null)
            itemName.text = name;


        EventDelegate.Set(itemImg.transform.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            OnclicItemPopup(itemIdx);
        });

    }

    /// <summary> 아이템 상세창 </summary>
    void OnclicItemPopup(uint id)
    {
        UIMgr.OpenDetailPopup(this, id, 6);
    }
    
    /// <summary> 탭버튼 클릭시 들어올 콜백함수 </summary>
    void OnClickTab(int viewType)
    {
        ChangeView((eTAB_VIEW_TYPE)viewType);
    }

    /// <summary> 탭 버튼에 따라 보고있는 뷰를 바꿔준다  </summary>
    void ChangeView(eTAB_VIEW_TYPE type)
    {
        if (CurViewType == type)//동일하므로 무시
            return;

        // eTAB_VIEW_TYPE prevType = CurViewType;
        PrevViewType = CurViewType;
        CurViewType = type;

        for (int i = 0; i < TabViewGO.Length; i++)
        {
            GameObject CurViewTab = TabGroup.transform.GetChild(i).gameObject;
            GameObject On = CurViewTab.transform.FindChild("On").gameObject;
            GameObject Off = CurViewTab.transform.FindChild("Off").gameObject;
         
            if ((int)type == i)
            {
                TabViewGO[i].SetActive(true);
                On.SetActive(true);
                Off.SetActive(false);
            }
            else
            {
                TabViewGO[i].SetActive(false);
                On.SetActive(false);
                Off.SetActive(true);
            }
        }

        switch (type)
        {
            case eTAB_VIEW_TYPE.DAILY:
                SceneManager.instance.ShowNetProcess("SignInInfo");
                NetworkClient.instance.SendPMsgSignInQueryInfoC();   //출첵 
                break;
            case eTAB_VIEW_TYPE.ACCESSTIME:
                SceneManager.instance.ShowNetProcess("AccessWelfareInfo");
                NetworkClient.instance.SendPMsgWelfareOnlineQueryInfoC();   //로그인유지보상
                break;
            case eTAB_VIEW_TYPE.WEEK:
                SceneManager.instance.ShowNetProcess("WeekWelfareInfo");
                NetworkClient.instance.SendPMsgWelfareXDayLoginQueryInfoC();   //7일연속 
                break;
            case eTAB_VIEW_TYPE.PAYBACK:
                SceneManager.instance.ShowNetProcess("RechargeTotal");
                NetworkClient.instance.SendPMsgRechargeTotalQueryInfoC();   //누적결제
                SceneManager.instance.ShowNetProcess("DailyTotal");
                NetworkClient.instance.SendPMsgRechargeDailyQueryInfoC();   //일간결제
                SceneManager.instance.ShowNetProcess("ConsumerTotal");
                NetworkClient.instance.SendPMsgRechargeConsumerQueryInfoC();   //누적소비 
                break;
            case eTAB_VIEW_TYPE.LEVELUP:
                SceneManager.instance.ShowNetProcess("LvWelfareInfo");
                NetworkClient.instance.SendPMsgWelfareRoleUpgradeQueryInfoC();   //레벨업
                TabViewGO[5].SetActive(true);
                break;
            case eTAB_VIEW_TYPE.GROW:
                EndPeriodTime();
                break;
            case eTAB_VIEW_TYPE.MONTH:
                EndPeriodTime();
                break;
            case eTAB_VIEW_TYPE.NEWSERVER:
                SceneManager.instance.ShowNetProcess("newSvrWelfareInfo");
                NetworkClient.instance.SendPMsgWelfareOpenSvrQueryInfoC();
                break;
        }
    }

    // 보상기간이 지났을때, 전의 창을열여줌
    public void EndPeriodTime(int errorId = 0)
    {
        if(errorId ==0)
        {
            UIMgr.instance.AddPopup(141, 174, 117);
        }
        else
        {
            UIMgr.instance.AddErrorPopup((int)errorId);

        }
        //ChangeView(PrevViewType);

        //팝업띄워주고 TabView 빠궈줌
        TabViewGO[(int)CurViewType].SetActive(false);
        TabViewGO[(int)PrevViewType].SetActive(true);

        GameObject CurViewTab = TabGroup.transform.GetChild((int)CurViewType).gameObject;
        CurViewTab.transform.GetChild(0).gameObject.SetActive(false);
        CurViewTab.transform.GetChild(1).gameObject.SetActive(true);

        GameObject PreViewTab = TabGroup.transform.GetChild((int)PrevViewType).gameObject;
        PreViewTab.transform.GetChild(0).gameObject.SetActive(true);
        PreViewTab.transform.GetChild(1).gameObject.SetActive(false);

        eTAB_VIEW_TYPE temp = CurViewType;
        CurViewType = PrevViewType;
        PrevViewType = temp;
    }

    public override void Close()
    {
        bool isAlram = false;
        for(int i=0;i<alram.Count;i++)
        {
            if (!alram[i])
                continue;

            isAlram = true;
            break;
        }

        SceneManager.instance.SetAlram(AlramIconType.BENEFIT, isAlram);

        base.Close();
        UIMgr.OpenTown();
    }
}

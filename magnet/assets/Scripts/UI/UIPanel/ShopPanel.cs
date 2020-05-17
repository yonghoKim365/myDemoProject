using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class ShopPanel : UIBasePanel
{
    public enum eMainTab
    {
        NONE = 0,
        CASH = 1,//원보
        GOLD = 2,// 골드
        CHANGE = 3,//교환
        CHARGE = 4, //충전
    }

    //소분류탭들
    uint[] ChildCashTab = { 101, 102, 103, 104, 105, 106, 107 };
    uint[] ChildGoldTab = { 201, 202, 203, 204, 205 };
    uint[] ChildChangeTab = { 301, 302,/* 303,*/ 304, 305 };
    private eMainTab CurViewType = eMainTab.NONE;//현재 보고있는 뷰 타입
    public UITabGroup TabGroup; //상점탭

    public UIScrollView ScrollView;
    public UIGrid Grid;
    public GameObject ShopSlotPrefab;
    public Transform ChildTabRoot;
    public Transform TimeRoot;

    public UIButton ResetBtn;
    public UIButton BuyBtn;
    public UIButton BtnGacha;

    public UIEventTrigger BtnMax; //최대수량

    public UIEventTrigger BtnPlus;
    public UIEventTrigger BtnMinus;

    public UILabel Money;   //보유화폐
    public UILabel GoodsInfo;   //아이템정보
    public UILabel GoodsName;    //아이템이름
    public GameObject EmptyTxt;

    public UISprite MoneyImg;   //화폐 모양

    public UILabel ResetTime;   //초기화 시간
    public UILabel TotalNum;  //1/100 총갯수중에 몇개인가?
    public UILabel TotalPrice;  //총가
    public UILabel TimeItemInfo;    //시간제한/한정일경우 아이템설명

    public Color MoneyLabelColor;

    private string RefreshTime; //초기화시간
    private UIBasePanel BasePanel;
    private uint CurShopType;  //상점 종류
    private List<Price.PriceInfo> PriceList = new List<Price.PriceInfo>();
    private ulong money;  //재화 비교위해 

    private int UseSellAmount;  //구매수량
    private int UseSellTotalAmount; //물품총술야

    public Color[] GradeNameColor; //등급에따라 이름색도변경 

    private int ChildTabNum;    //다른패널에서 호출시 사용하게될 소분류탭번호
    private bool lateInitLoad = false;

    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("Shop panel, Init() start");

        base.Init();
        CreateSlot();
        RefreshTime = null;

        EventDelegate.Set(BtnGacha.onClick, delegate ()
        {
            base.Hide();
            uiMgr.IsShop = true;
            UIMgr.OpenGachaPanel(SceneManager.instance.GetGachaFreeTime[0], SceneManager.instance.GetGachaFreeTime[1]);
            //NetworkClient.instance.SendPMsgLotteryQueryInfoC();
        });

        TabGroup.Initialize(OnClickTab);

		SceneManager.instance.showStopWatchTimer ("Shop panel, Init() finish");
    }

    public override void LateInit()
    {
        base.LateInit();
        if (parameters.Length == 0)
            return;

        if (0 < parameters.Length)
            BasePanel = (UIBasePanel)parameters[0];
        else
            TabGroup.CoercionTab((int)eMainTab.CASH);

        if (BasePanel != null)
        {
            switch (BasePanel.ToString())
            {
                case "UIPanel/TowerPanel (TowerPanel)": //교환상점에서 마탑
                    lateInitLoad = true;
                    ChildTabNum = 1;
                    // TabGroup.CoercionTab((int)eTAB_VIEW_TYPE.TOWER);
                    break;
                case "UIPanel/ArenaPanel (ArenaPanel)": //교환상점에서 차관
                    lateInitLoad = true;
                    ChildTabNum = 0;
                    // TabGroup.CoercionTab((int)eTAB_VIEW_TYPE.PVP);
                    break;
                case "UIPanel/FreefightPanel (DogFightPanel)"://교환상점에서 난투장
                    //  case "UIPanel/FreefightPanel2 (FreefightPanel)"://교환상점에서 난투장
                    lateInitLoad = true;
                    ChildTabNum = 3;
                    //  TabGroup.CoercionTab((int)eTAB_VIEW_TYPE.FIGHT);
                    break;
                case "UIPanel/GuildPanel (GuildPanel)": // 교환상점에서 길드 
                    lateInitLoad = true;
                    ChildTabNum = 2;
                    // TabGroup.CoercionTab((int)eTAB_VIEW_TYPE.GUILD);
                    break;
                default:
                    break;
            }
            TabGroup.CoercionTab((int)eMainTab.CHANGE);
        }
        else
            TabGroup.CoercionTab((int)eMainTab.CASH);


        BtnGacha.transform.FindChild("alarmmark ").gameObject.SetActive(SceneManager.instance.IsAlram(AlramIconType.SHOP));

		SceneManager.instance.sw.Stop ();
		SceneManager.instance.showStopWatchTimer ("Shop panel, LateInit() finish");
    }

	public override void Hide()
	{
		base.Hide();
		
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
		UIMgr.OpenTown();
	}

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    private GameObject[] SelEff = new GameObject[20];   //선택이펙트
    public void CreateSlot()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject slotGo = Instantiate(ShopSlotPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf = slotGo.transform;
            slotTf.parent = Grid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;

            Transform eff = slotTf.FindChild("cover").gameObject.transform;
            SelEff[i] = eff.gameObject;

            UIHelper.CreateInvenSlot(slotTf.FindChild("root"));
            slotGo.SetActive(false);
        }

        Destroy(ShopSlotPrefab);
    }

    ///// <summary> 구매팝업창에서 판매 개수 갱신  </summary>
    void OnclickUseCount(int add, int price)
    {
        // 1개일때는 수량0개로 불가함
        if (UseSellAmount == 1 && add == -1)
            return;

        UseSellAmount += add;

        if (UseSellTotalAmount < UseSellAmount)//맥스 값으로 갱신
            UseSellAmount = UseSellTotalAmount;

        else if (UseSellAmount < 0)//최대 개수로 갱신
            UseSellAmount = UseSellTotalAmount;

        TotalNum.text = UseSellAmount.ToString();
        TotalPrice.text = (UseSellAmount * price).ToString();//총 판매 금액
        TotalPrice.color = ulong.Parse(TotalPrice.text) <= money ? Color.white : Color.red;  //돈이 모자르면 빨간색

        //UseSellSlider.value = (float)UseSellAmount / (float)UseSellTotalAmount;

    }
    /// <summary> 탭버튼 클릭시 들어올 콜백함수 </summary>
    void OnClickTab(int viewType)
    {
        if (viewType == 0)
            return;

        if (viewType == (int)eMainTab.CHARGE) //충전
        {
            UIMgr.instance.AddPopup(141, 174, 117);
            return;
        }
        ChangeView((eMainTab)viewType);
    }
    /// <summary> 재화 갱신 </summary>
    public void RefreshCash(uint type)
    {
        ulong cash = NetData.instance.GetAsset(AssetType.Cash); //원보
        ulong gold = NetData.instance.GetAsset(AssetType.Gold); //골드
        ulong fame = NetData.instance.GetAsset(AssetType.FAME); //성망
        ulong badge = NetData.instance.GetAsset(AssetType.Badge);   //휘장
        ulong honor = NetData.instance.GetAsset(AssetType.Honor);   //명예
        ulong contri = NetData.instance.GetAsset(AssetType.Contribute); //공헌
        ulong lion = NetData.instance.GetAsset(AssetType.LionBadge); //사자왕휘장?

        byte costType = _LowDataMgr.instance.GetShopCostType(type);

        switch (costType)
        {
            case 1: //골드
				Money.text = string.Format("{0}", (gold == 0 ? "0" : gold.ToString())); //gold.ToString("#,##")));
                MoneyImg.spriteName = "money";
                money = gold;
                break;
            case 2: //원보
			Money.text = string.Format("{0}", (cash == 0 ? "0" : cash.ToString())); //cash.ToString("#,##")));
                MoneyImg.spriteName = "cash";
                money = cash;
                break;
            case 3: //공헌
			Money.text = string.Format("{0}", (contri == 0 ? "0" : contri.ToString())); //contri.ToString("#,##")));
                MoneyImg.spriteName = "guildpoint";
                money = contri;
                break;
            case 4: //명예
			Money.text = string.Format("{0}", (honor == 0 ? "0" : honor.ToString())); //honor.ToString("#,##")));
                MoneyImg.spriteName = "honor";
                money = honor;
                break;
            case 5: //황성휘장
			Money.text = string.Format("{0}", (badge == 0 ? "0" : badge.ToString())); //badge.ToString("#,##")));
                MoneyImg.spriteName = "badge_A";
                money = badge;
                break;
            case 6: //사자왕휘장
			Money.text = string.Format("{0}", (lion == 0 ? "0" : lion.ToString())); //lion.ToString("#,##")));
                MoneyImg.spriteName = "badge_B";
                money = lion;
                break;
            case 7: //성망
			Money.text = string.Format("{0}", (fame == 0 ? "0" : fame.ToString())); //fame.ToString("#,##")));
                MoneyImg.spriteName = "starlight";
                money = fame;
                break;

        }


    }
    public void ViewInfo(uint type, NetData.ShopItemInfoData shopInfo)
    {
        ScrollView.gameObject.SetActive(shopInfo == null ? false : true);
        EmptyTxt.SetActive(shopInfo ==null? true:false);
        if (/*!ScrollView.gameObject.activeSelf*/shopInfo==null)
        {
            return;
        }

        int shopCnt = shopInfo.shopInfo.Count;
        CurShopType = shopInfo.Type;
        RefreshCash(CurShopType);

        bool IsTimeReset = true;
        if (CurShopType == 101)
            IsTimeReset = false;
        if (CurShopType == 201)
            IsTimeReset = false;
        if (CurShopType == 202)
            IsTimeReset = false;
        if (CurShopType == 102)
            IsTimeReset = false;

        TimeRoot.gameObject.SetActive(IsTimeReset);
        TimeItemInfo.gameObject.SetActive(!TimeRoot.gameObject.activeSelf);
        ResetBtn.transform.gameObject.SetActive(IsTimeReset);

        if (CurViewType != eMainTab.CHANGE)
        {
            TimeRoot.gameObject.SetActive(false);
            ResetBtn.transform.gameObject.SetActive(false);

        }

        if (IsTimeReset)
        {
            RefreshTime = shopInfo.RefreshTimer.ToString();
            // 리셋비용을 얻기위해
            PriceList = _LowDataMgr.instance.GetLowDataPrice(shopInfo.Type);
        }

        GoodsName.text = "";
        GoodsInfo.text = "";
        UseSellAmount = 0;
        UseSellTotalAmount = 0;
        TotalNum.text = "0";
        TotalPrice.text = "0";
        TimeItemInfo.text = "";


        bool isFirstSelect = false;

        int childCount = 0;
        for (int i = 0; i < Grid.transform.childCount; i++)
        {

            if (i >= shopCnt)
            {
                Grid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            Transform slotTf = Grid.transform.GetChild(i);
            slotTf.gameObject.SetActive(true);

            NetData.ShopItemInfo info = shopInfo.shopInfo[i];
            Shop.ShopInfo shopTable = _LowDataMgr.instance.GetLowDataShopById(info.DbIndex);

            if (shopTable == null)
            {
                Grid.transform.GetChild(i).gameObject.SetActive(false);
                continue;

            }

            //한정판매 일때는 기간지나면 목록에서 없앰
            if (shopTable.shopopentime != 0 )
            {
                DateTime startTime = DateTime.ParseExact(shopTable.shopopentime.ToString(), "yyyyMMddHHmmss", null);
                DateTime endTime = DateTime.ParseExact(shopTable.shopclosetime.ToString(), "yyyyMMddHHmmss", null);

                if (startTime.CompareTo(DateTime.Now) < 0) // 오늘이 시작날짜보다 빠르다면 
                {
                    slotTf.gameObject.SetActive(false);
                    continue;
                }

                if(endTime.CompareTo(DateTime.Now) > 0) //오늘이 완료날짜보다 늦다면
                {
                    slotTf.gameObject.SetActive(false);
                    continue;
                }
            }

            // 갯수 1개는 표시x , 2개부터 표시해줌
            UILabel Count = slotTf.FindChild("cnt").GetComponent<UILabel>();
            Count.text = info.Account.ToString();
            Count.gameObject.SetActive(info.Account > 1 ? true : false);

            UISprite moneyIcon = slotTf.FindChild("Img_Money").GetComponent<UISprite>();
            moneyIcon.spriteName = MoneyImg.spriteName;
            
            UILabel name = slotTf.FindChild("name").GetComponent<UILabel>();
            UILabel price = slotTf.FindChild("price").GetComponent<UILabel>();

            price.text = shopTable.cost.ToString();
            Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(info.GoodsId);
            if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
            {
                name.color = GradeNameColor[eLowData.Grade];
                name.text = _LowDataMgr.instance.GetStringItem(eLowData.NameId);
            }
            else//소모아이템이 드랍아이템 대표로 등록되어 있음
            {
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(info.GoodsId);
                if (uLowData == null)
                    continue;
                
                name.color = GradeNameColor[uLowData.Grade];
                name.text = _LowDataMgr.instance.GetStringItem(uLowData.NameId);

            }


            int idx = i;

            //슬롯눌럿을경우 선택 
            EventDelegate.Set(slotTf.GetComponent<UIEventTrigger>().onClick, delegate ()
            {
                if (0 < info.Account)
                    OnclicItemPopup(info, int.Parse(price.text), name.color);

                for (int j = 0; j < SelEff.Length; j++)
                    SelEff[j].SetActive(j == idx);
            });

            //아이콘일경우 팝업
            slotTf.FindChild("root").GetChild(0).GetComponent<InvenItemSlotObject>().SetLowDataItemSlot(info.GoodsId, 0, (key) =>
            {
                UIMgr.OpenDetailPopup(this, info.GoodsId);
            });

            //매진팝업
            Transform soldOut = slotTf.FindChild("soldout").GetComponent<Transform>();
            Transform soldOutStemp = slotTf.FindChild("Stemp").GetComponent<Transform>();

            soldOut.gameObject.SetActive(false);
            soldOutStemp.gameObject.SetActive(false);

            // 한개 이상이면 구매팝업창으로
            if (info.Account < 1)
            {
                soldOut.gameObject.SetActive(true);
                soldOutStemp.gameObject.SetActive(true);
            }


            //매진이아닌 아이템이 최초선택된채로 시작.
            if (!isFirstSelect && !soldOutStemp.gameObject.activeSelf && !isResetSelect)
            {
                OnclicItemPopup(info, int.Parse(price.text), name.color);

                for (int j = 0; j < SelEff.Length; j++)
                    SelEff[j].SetActive(j == idx);

                isFirstSelect = true;
            }
            childCount++;
        }

        //초기화
        EventDelegate.Set(ResetBtn.onClick, delegate ()
        {
            OnclickReset((int)shopInfo.ManualRefreshCount, (int)shopInfo.Type);
        });
        EmptyTxt.SetActive(shopInfo==null/*!Grid.transform.GetChild(0).gameObject.activeSelf*/);
        Grid.Reposition();
        ScrollView.enabled = childCount > 10;
        isResetSelect = false;
    }

    public void Update()
    {
        if (RefreshTime != null)
            ResetTime.text = TransformTime(CurShopType, RefreshTime);
    }

    // 초기화시간 계산
    string TransformTime(uint type, string RefreshTime)
    {
        string msg = "";

        // 시간
        DateTime Now = DateTime.Now;    //현재시간
        //DateTime ResetTime;    //초기화시간 
        //DateTime NormalShopResetTime = new DateTime();    //일반상점 초기화시간 (2시간)

        int year = int.Parse(RefreshTime.Substring(0, 4));
        int month = int.Parse(RefreshTime.Substring(4, 2));
        int day = int.Parse(RefreshTime.Substring(6, 2));
        int hour = int.Parse(RefreshTime.Substring(8, 2));

        DateTime ResetTime = new DateTime(year, month, day, 0, 0, 0);

        if (type == 1)
        {
            // 일반상점
            if (hour >= 22) //10시이후에는 hour+2 했을경우 24시를 넘어가기때문에 그냥 하루를 더해줌
                // ResetTime = new DateTime(year, month, day + 1, 0, 0, 0, 0);   //2시간
                ResetTime = ResetTime.AddDays(1);
            else
                //ResetTime = new DateTime(year, month, day, hour + 2, 0, 0);   //2시간
                ResetTime = ResetTime.AddHours(hour + 2);

        }
        else
        {
            // 나머지상점
            if (day + 1 > DateTime.DaysInMonth(year, month))    // day+1 이 그달의 일수를 넘어간다면? ex) 2월 29일인데 +1 해서 30이나왓다
            {
                ResetTime = ResetTime.AddMonths(1);
                //ResetTime = new DateTime(year, month + 1, 0, 0, 0, 0);   //하루
            }
            else
            {
                //ResetTime = new DateTime(year, month, day + 1, 0, 0, 0);   //하루
                ResetTime = ResetTime.AddDays(1);
            }


        }

        TimeSpan ts = ResetTime - Now;

        msg = string.Format("{0} {1:00} : {2:00} : {3:00}", _LowDataMgr.instance.GetStringCommon(463), ts.Hours, ts.Minutes, ts.Seconds);
        if (ts.Seconds < 0)
            msg = string.Format("{0} {1:00} : {2:00} : {3:00}", _LowDataMgr.instance.GetStringCommon(463), 0, 0, 0);

        return msg;
    }
    /// <summary> 초기화버튼  </summary>
    void OnclickReset(int cnt, int type)
    {
        // 1번은 무료 , 2번째부터는 원보차감(팝업으로물어봄)

        uint reset = 0;   //리셋비용
        string resetType = "";  //재구매 비용타입(골드/원보,.,)_

        // 타입에따른 리스트 
        PriceList = _LowDataMgr.instance.GetLowDataPrice((uint)type);


        if (cnt == PriceList[PriceList.Count - 1].ResetCount) // 최대 리셋횟수초과한경우
        {
            UIMgr.instance.AddErrorPopup(1203);
            return;
        }

        // 리셋회수랑 같은 비용 
        for (int i = 0; i < PriceList.Count; i++)
        {
            if (cnt + 1 != PriceList[i].ResetCount)
                continue;
            reset = PriceList[i].ResetValue;
            if (PriceList[i].ResetType == 1)
            {
                //게임골드
                resetType = _LowDataMgr.instance.GetStringCommon(4);
            }
            else if (PriceList[i].ResetType == 2)
            {
                //원보
                resetType = _LowDataMgr.instance.GetStringCommon(3);

            }
        }

        //0원이면 팝업없이가자
        if(reset==0)
        {
            SceneManager.instance.ShowNetProcess("RefreshShop");
            NetworkClient.instance.SendPMsgShopRefreshC((uint)type);
        }
        else
        {
            //UIMgr.instance.AddPopup(40, string.Format("{0}|{1}", reset.ToString(), resetType), delegate () { NetworkClient.instance.SendPMsgShopRefreshC((uint)type); }, null, null);
            string popMsg = string.Format(_LowDataMgr.instance.GetStringCommon(465), reset.ToString(), resetType);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), popMsg, _LowDataMgr.instance.GetStringCommon(117), _LowDataMgr.instance.GetStringCommon(76), null
                , delegate ()
                {
                    SceneManager.instance.ShowNetProcess("RefreshShop");
                    NetworkClient.instance.SendPMsgShopRefreshC((uint)type);
                });
        }

    }


    /// <summary> 아이템클릭시  </summary>
    void OnclicItemPopup(NetData.ShopItemInfo info, int price, Color nameColor)
    {
        //아이템세부정보
        //구매팝업에서는 무조건 1개로시작함

        string dec = "";
        string itemName = "";
        if (_LowDataMgr.instance.GetLowDataEquipItemInfo(info.GoodsId) != null)//장비아이템
        {
            Item.EquipmentInfo equipLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(info.GoodsId);
            Item.ItemValueInfo valueLowData = _LowDataMgr.instance.GetLowDataItemValueInfo(equipLowData.BasicOptionIndex);

            //dec += _LowDataMgr.instance.GetStringItem(equipLowData.NameId);
            itemName = _LowDataMgr.instance.GetStringItem(equipLowData.NameId);
            //dec += "\n\n";
            dec += _LowDataMgr.instance.GetStringItem(equipLowData.DescriptionId);
            dec += "\n\n";
            dec += string.Format("{0} : {1}", uiMgr.GetAbilityLocName((AbilityType)valueLowData.OptionId)
                , uiMgr.GetAbilityStrValue((AbilityType)valueLowData.OptionId, valueLowData.BasicValue)); ;
            dec += "\n\n";
            dec += string.Format(_LowDataMgr.instance.GetStringCommon(179)
                , 0, 3);

        }
        else
        {
            Item.ItemInfo useLowData = _LowDataMgr.instance.GetUseItem(info.GoodsId);
            itemName = _LowDataMgr.instance.GetStringItem(useLowData.NameId);
            //dec += _LowDataMgr.instance.GetStringItem(useLowData.NameId);
            //dec += "\n\n";
            dec += _LowDataMgr.instance.GetStringItem(useLowData.DescriptionId);
        }

        GoodsName.text = itemName;
        GoodsName.color = nameColor;
        GoodsInfo.text = dec;

        //        시간 / 한정판매아이템일경우 초기화시간자리에 정보를 표시해줘야함
        if (CurShopType == 102 || CurShopType == 202)
        {
            //TimeItemInfo.gameObject.SetActive(true);
            ulong start = _LowDataMgr.instance.GetLowDataShopById(info.DbIndex).shopopentime;
            ulong end = _LowDataMgr.instance.GetLowDataShopById(info.DbIndex).shopclosetime;

            //577 {0}년{0}월{1}일 // 260 {0}분
            string Start = string.Format(_LowDataMgr.instance.GetStringCommon(577),
                start.ToString().Substring(0, 4), start.ToString().Substring(4, 2), start.ToString().Substring(6, 2));
            Start += string.Format(_LowDataMgr.instance.GetStringCommon(260), start.ToString().Substring(8, 2));
            //Start += ":";
            //Start += start.ToString().Substring(2, 2);
            //Start += ":";
            //Start += start.ToString().Substring(4, 2);

            string End = string.Format(_LowDataMgr.instance.GetStringCommon(577),
                end.ToString().Substring(0, 4), end.ToString().Substring(4, 2), end.ToString().Substring(6, 2));
            End += string.Format(_LowDataMgr.instance.GetStringCommon(260), end.ToString().Substring(8, 2));


            //string End = end.ToString().Substring(0, 2);
            //End += ":";
            //End += end.ToString().Substring(2, 2);

            //End += ":";
            //End += end.ToString().Substring(4, 2);

            TimeItemInfo.text = string.Format("{0} ~ {1}", Start, End);
        }

        UseSellAmount = 1;
        UseSellTotalAmount = (int)info.Account;

        TotalNum.text = UseSellAmount.ToString();
        TotalPrice.text = (UseSellAmount * price).ToString();

        TotalPrice.color = ulong.Parse(TotalPrice.text) <= money ? Color.white : Color.red;  //돈이 모자르면 빨간색

        EventDelegate.Set(BtnPlus.onClick, delegate () { OnclickUseCount(1, price); });
        EventDelegate.Set(BtnMinus.onClick, delegate () { OnclickUseCount(-1, price); });

        EventDelegate.Set(BtnMax.onClick, delegate () { OnclickUseCount((int)info.Account, price); });


        EventDelegate.Set(BuyBtn.onClick, delegate ()
        {
        if (UseSellAmount != 0)
        {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(472), itemName);
            uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141), msg,_LowDataMgr.instance.GetStringCommon(467), _LowDataMgr.instance.GetStringCommon(76),null,
                ()=>
                {
                    SceneManager.instance.ShowNetProcess("PurchaseShopItme");
                    NetworkClient.instance.SendPMsgShopByItemC(info.Idx, CurShopType, info.GoodsId, (uint)UseSellAmount, info.DbIndex);
                });
            }
        });


        //  TotalPrice.text = ((ulong)UseSellTotalAmount * price) * ToString();//총 판매 금액

    }

    /// <summary> 탭 버튼에 따라 보고있는 뷰를 바꿔준다  </summary>
    void ChangeView(eMainTab type)
    {
        if (CurViewType == type)//동일하므로 무시
            return;

        for (int j = 0; j < SelEff.Length; j++)
            SelEff[j].SetActive(false);

        //eMainTab prevType = CurViewType;
        CurViewType = type;

        // 탭마다 소분류탭이 다르기때문에 셋팅을 해줘야함 
        SetChildTab(CurViewType);
    }


    void SetChildTab(eMainTab CurViewType /*, bool lateInitLoad = false*/ )
    {
        int[] childTabCount = { 8, 5, /*5*/4 };   //자식탭수
        uint[] stringTab1 = { 816, 817, 818, 6, 7, 819, 820, 539 }; //탭의 이른 스트링커먼아이디 (일단임시로 박음)
        uint[] stringTab2 = { 816, 817, 818, 6, 7 };
        uint[] stringTab3 = { 452, 194, /*461,*/ 8, 12 }; //판호를위해 차관이랑 사자왕 지워놓자
        uint[] curChildTab = { };
        switch (CurViewType)
        {
            case eMainTab.CASH:
                curChildTab = stringTab1;
                break;
            case eMainTab.GOLD:
                curChildTab = stringTab2;
                break;
            case eMainTab.CHANGE:
                curChildTab = stringTab3;
                break;
            case eMainTab.CHARGE:
                break;
            default:
                break;
        }

        for (int i = 0; i < ChildTabRoot.childCount; i++)
        {
            GameObject go = ChildTabRoot.GetChild(i).gameObject;

            //현재 상점데이터가 없어서 첫번째 텝에 다몰아넣어놈 , 첫째탭제외하고 다 숨겨놓기 (데이터 들어오면 다시켜줘여함)
            if (i != 0)
            {
                go.SetActive(false);
                continue;
            }
            // for panho build. hide vip tab.
            //if (i == 6)
            //{
            //    go.SetActive(false);
            //    continue;
            //}

            if (i >= childTabCount[(int)CurViewType - 1])    //넘어감
            {
                go.SetActive(false);
                continue;
            }
            else
            {
                go.SetActive(true);
                for (int j = 0; j < go.transform.childCount; j++)
                {
                    //탭이름변경
                    go.transform.GetChild(j).FindChild("label_d3").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(curChildTab[i]);
                }
            }
            int idx = i;
            EventDelegate.Set(go.GetComponent<UIButton>().onClick, delegate () { OnClickChildTab(CurViewType, idx); }); //버튼클릭함수

        }


        if (!lateInitLoad)
        {
            //처음은 무조건 첫번째소분류탭으로 나오게한다.
            OnClickChildTab(CurViewType, 0);
        }
        else
        {
            // 다른패널에서 호출됫음
            OnClickChildTab(CurViewType, ChildTabNum);
            lateInitLoad = false;
        }

        //TimeItemInfo.gameObject.SetActive(false);
    }

    // 소분류탭 함수 (여기서 상점프로토콜 요청)
    void OnClickChildTab(eMainTab curTab, int childTabNumber)
    {
        //UIScrollView scorll = ChildTabRoot.GetComponent<UIScrollView>();
        //scorll.enabled = curTab == eMainTab.CASH ? true : false;

        //On/Off 이미지교체랑 콜라이더 해제 작업
        for (int i = 0; i < ChildTabRoot.childCount; i++)
        {
            GameObject go = ChildTabRoot.GetChild(i).gameObject;
            if (!go.activeSelf)
                continue;

            if (i != childTabNumber)     // Off 켜주고 콜라이더켜줘
            {
                go.transform.FindChild("On").gameObject.SetActive(false);
                go.transform.FindChild("Off").gameObject.SetActive(true);
                go.GetComponent<BoxCollider>().enabled = true;
            }
            else    // On켜주고 콜라이더 ㄲㅓ줘
            {
                
                go.transform.FindChild("On").gameObject.SetActive(true);
                go.transform.FindChild("Off").gameObject.SetActive(false);
                go.GetComponent<BoxCollider>().enabled = false;
            }

        }

        for (int j = 0; j < SelEff.Length; j++)
            SelEff[j].SetActive(false);

        switch (curTab)
        {
            case eMainTab.NONE:
                break;
            case eMainTab.CASH:
                //Debug.Log(ChildCashTab[childTabNumber]);
                //if (childTabNumber == 7)//가챠
                //{
                //    base.Hide();
                //    uiMgr.IsShop = true;
                //    NetworkClient.instance.SendPMsgLotteryQueryInfoC();
                //}
                //else
                //{
                    SceneManager.instance.ShowNetProcess("GetShopInfo");
                    NetworkClient.instance.SendPMsgShopInfoQueryC(ChildCashTab[childTabNumber]);
                //}
                break;
            case eMainTab.GOLD:
                // Debug.Log(ChildCashTab[childTabNumber]);
                SceneManager.instance.ShowNetProcess("GetShopInfo");
                NetworkClient.instance.SendPMsgShopInfoQueryC(ChildGoldTab[childTabNumber]);
                break;
            case eMainTab.CHANGE:
                //Debug.Log(ChildCashTab[childTabNumber]);

                if (childTabNumber == /*3*/2)    //길드상점 , 레벨을 받아와야함
                {
                    //미가입 팝업처리
                    if (NetData.instance.GetUserInfo()._GuildId == 0)
                    {
                        uiMgr.AddPopup(141, 957, 117);
                    }
                    else
                    {
                        NetworkClient.instance.SendPMsgGuildQueryDetailedInfoC(NetData.instance.GetUserInfo()._GuildId);
                    }
                }
                else
                {
                    SceneManager.instance.ShowNetProcess("GetShopInfo");
                    NetworkClient.instance.SendPMsgShopInfoQueryC(ChildChangeTab[childTabNumber]);
                }

                break;
            case eMainTab.CHARGE:   //충전은따로뺄까?
                break;

        }

        GoodsInfo.text = "";
        UseSellAmount = 0;
        UseSellTotalAmount = 0;
        TotalNum.text = "0";
        TotalPrice.text = "0";
        TimeItemInfo.text = "";

    }

    //길드레벨을 거쳐서 와야하기때문에..
    public void GuildShopView(uint lv)
    {
        //길드레벨로 -> 상점 열어줘
        Guild.GuildShopLevelInfo shopInfo = _LowDataMgr.instance.GetLowDataGuildShopLevel(lv);
        NetworkClient.instance.SendPMsgShopInfoQueryC(shopInfo.ShopType);
    }



    bool isResetSelect = false;
    public void ShopView(uint type)
    {
        for (int j = 0; j < SelEff.Length; j++)
            SelEff[j].SetActive(false);

        isResetSelect = true;
        SceneManager.instance.ShowNetProcess("GetShopInfo");
        NetworkClient.instance.SendPMsgShopInfoQueryC(type);
    }

    public override void Close()
    {
		CameraManager.instance.mainCamera.gameObject.SetActive (true);
        base.Close();

        //타운패널을 제외하고 호출됬을경우 종료했을때 다시 호출해 줘야함
        if (BasePanel != null)
        {
            //마계의탑은 정보를 받아와야하므로 따로호출해줘야함
            if (BasePanel.ToString().Contains("Tower"))
                UIMgr.OpenTowerPanel();
            else
            {
                BasePanel.Show(BasePanel.GetParams());

            }
        }
        else
            UIMgr.OpenTown();

    }


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GachaPanel : UIBasePanel {

    enum eRateViewType
    {
        all = 0,//전체
        equip = 1,//장비
        item = 2,//재료
        piece = 3,//조각
        etc = 4,//기타
    }
    
    public GameObject Popup;
    public GameObject AniObj;
    public GameObject ResultText;
    public GameObject RateView;
    public GameObject GachaView;
    public GameObject[] ViewObjs;//0 List, 1 Result

    public UIEventTrigger InfoBtn;
    public UIEventTrigger BtnAgain;
    //public UIEventTrigger BtnOpenCard;
    public UIEventTrigger BtnClose;//카드 이벤트 닫기
    public UIEventTrigger BtnOpenAllCard;
    
    public Transform[] ResultCards;//0~9임
    public Transform ResultOneCard;//1개임
    public Transform RateSideTabBtnTf; //일반뽑기/고급뽑기 구분탭 

    public UILabel PopupMsg;
    public UILabel GoldTimer;
    public UILabel CashTimer;
    public UILabel[] PanhoLbls;//0 골드용 일일 제한횟수, 1 캐쉬용 일일제한 횟수

    public UITabGroup TabGroup;
    public UIGrid RateGrid;
    public UIScrollView RateScroll;

    public TweenAlpha ResultAlpha;

    public string ChardForward = "Img_card_01";
    public string ChardBack = "Img_card_02";

    //public float AlphaSpeed = 1.5f;
    public float CardDuration = 0.5f;//카드 하나당 뒤집는 시간
    public float CardOpenDelay = 0.1f;//카드 뒤집어지는 연출 후 아이템 나오는 시간
    public float CardActionDelay = 0.2f;
	public float BoxEffTime = 4f;//2.8f;
    public float BoxDeleteDelay = 1f;
    public float StayTime = 0.1f;

    private System.DateTime GoldFreeTimer;
    private System.DateTime CashFreeTimer;

    private InvenItemSlotObject OneCardSlot;

    private string TimeStr;
    
    private bool IsChargeGold;//일회 뽑기 무료까지 시간 업데이트 인지
    private bool IsChargeCash;//일회 뽑기 무료까지 시간 업데이트 인지
    private bool IsOnceGacha;//현재 뽑기가 1회 인지 (false면 10회)
    private bool IsGoldGacha;//현재 뽑기가 골드 인지(false면 cash)

    private int CurRateTabIdx;

    public bool IsAni;

    public override void Init()
    {
        base.Init();
        Animation a = CreateBox(true, true, false);
        Animation b = CreateBox(false, true, false);
        TempCoroutine.instance.FrameDelay(0.1f, () => { 
            Destroy(a.transform.parent.gameObject);
			Destroy(b.transform.parent.gameObject);
        });

        TimeStr = _LowDataMgr.instance.GetStringCommon(524);

        EventDelegate.Set(InfoBtn.onClick, SetRateView);

        Transform goldTf = transform.FindChild("GachaView/List/Gold/Scroll/Grid");
        Transform cashTf = transform.FindChild("GachaView/List/Cash/Scroll/Grid");
        //0은 1회(공짜) 1은 10회(유료)
        SetBox(goldTf.GetChild(1), false, true);//골드
        SetBox(cashTf.GetChild(1), false, false);//캐쉬
        
        int length = ResultCards.Length;
        for (int i = 0; i < length; i++)
        {
            Transform tf = ResultCards[i];
            if (tf == null)
            {
                Debug.LogError("is null error child number " + i);
                continue;
            }

            //tf.rotation = new Quaternion(0, 0, 0, 0);//원상 복구
            tf.collider.enabled = true;
            GameObject goSlot = UIHelper.CreateInvenSlot(tf.FindChild("ItemRoot"));
            goSlot.name = "slot";
            goSlot.SetActive(false);

            UIHelper.CreateEffectInGame(tf.FindChild("eff"), "Fx_UI_gacha_card_01", true);
            UIHelper.CreateEffectInGame(tf.FindChild("eff"), "Fx_UI_gacha_cardTrail_01", true);

            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate() {//단독 클릭시.

                int length2 = ResultCards.Length, count = 0;
                for (int j = 0; j < length2; j++)//검사용 해당 객체가 마지막인지 확인
                {
                    Transform tf2 = ResultCards[j];
                    if (tf2 == null)
                    {
                        Debug.LogError("is null error child number " + j);
                        continue;
                    }

                    if (tf2.eulerAngles.y == 0)
                        ++count;
                }
                
                tf.collider.enabled = false;

                TweenRotation.Begin(tf.gameObject, CardDuration, new Quaternion(0, -180, 0, 0));

                SoundManager.instance.PlaySfxSound(IsGoldGacha ? eUISfx.UI_gatcha_normal_open : eUISfx.UI_gatcha_rare_open, false);

                TempCoroutine.instance.FrameDelay(CardDuration-CardOpenDelay, delegate () {

                    if (tf.GetComponent<TweenRotation>() != null)
                        Destroy(tf.GetComponent<TweenRotation>(), CardDuration-(CardDuration-CardOpenDelay) );

                    tf.GetComponent<UISprite>().spriteName = ChardBack;
                    tf.FindChild("ItemRoot/slot").gameObject.SetActive(true);
                });

                if (count == 1)//마지막으로 남은 녀석임.
                {
                    BtnOpenAllCard.gameObject.SetActive(false);

                    TempCoroutine.instance.FrameDelay(CardDuration, delegate() {
                        BtnAgain.gameObject.SetActive(true);//공짜가 아닐 경우
                        BtnClose.gameObject.SetActive(true);
                        
                    } );
                }
            });
        }

        GameObject invenSlot = UIHelper.CreateInvenSlot(ResultOneCard.FindChild("ItemRoot") );
        UIHelper.CreateEffectInGame(ResultOneCard, "Fx_UI_gacha_card_01", true);
        UIHelper.CreateEffectInGame(ResultOneCard, "Fx_UI_gacha_cardTrail_01", true);
        OneCardSlot = invenSlot.GetComponent<InvenItemSlotObject>();
        invenSlot.SetActive(false);
        
        EventDelegate.Set(BtnClose.onClick, delegate () {//결과화면 닫기 버튼
            ChangeView(true);
        });

        EventDelegate.Set(BtnAgain.onClick, delegate () {//카드 한번 더
            //if(PlayGachaAndSaveToDayCount(IsGoldGacha) )
            int toDayCount = GetToDayCount(IsGoldGacha);
            if ( (IsOnceGacha && toDayCount <= 0) || (!IsOnceGacha && toDayCount != 10))//일일 제한 횟수 에러
                SceneManager.instance.SetNoticePanel(NoticeType.Message, 377);
            else
                BtnOpenResult(IsOnceGacha, IsGoldGacha);
        });

        EventDelegate.Set(ResultOneCard.GetComponent<UIEventTrigger>().onClick, delegate() {//하나짜리 카드 뽑음
            ResultOneCard.collider.enabled = false;

            SoundManager.instance.PlaySfxSound(IsGoldGacha ? eUISfx.UI_gatcha_normal_open : eUISfx.UI_gatcha_rare_open, false);
            TweenRotation.Begin(ResultOneCard.gameObject, CardDuration, new Quaternion(0, -180, 0, 0));
            
            TempCoroutine.instance.FrameDelay(CardDuration-CardOpenDelay, delegate () {

                if (ResultOneCard.GetComponent<TweenRotation>() != null)
                    Destroy(ResultOneCard.GetComponent<TweenRotation>(), CardDuration-(CardDuration-CardOpenDelay));

                ResultOneCard.GetComponent<UISprite>().spriteName = ChardBack;
                OneCardSlot.gameObject.SetActive(true);
            });
            
            TempCoroutine.instance.FrameDelay(CardDuration, delegate () {
                BtnAgain.gameObject.SetActive(true);
                BtnClose.gameObject.SetActive(true);
            } );
        });

        EventDelegate.Set(BtnOpenAllCard.onClick, delegate() {//한번에 열기
            //BtnOpenCard.gameObject.SetActive(false);
            BtnOpenAllCard.gameObject.SetActive(false);

            int cardLength = ResultCards.Length;
            for (int i = 0; i < cardLength; i++)
            {
                Transform tf = ResultCards[i];
                if (tf == null)
                {
                    Debug.LogError("is null error child number " + i);
                    continue;
                }

                if (tf.eulerAngles.y != 0)//이미 돌아간 녀석
                    continue;

                tf.collider.enabled = false;

                TweenRotation.Begin(tf.gameObject, CardDuration, new Quaternion(0, -180, 0, 0));
                
                TempCoroutine.instance.FrameDelay(CardDuration - CardOpenDelay, delegate () {

                    if (tf.GetComponent<TweenRotation>() != null)
                        Destroy(tf.GetComponent<TweenRotation>(), CardDuration-(CardDuration-CardOpenDelay));

                    tf.GetComponent<UISprite>().spriteName = ChardBack;
                    tf.FindChild("ItemRoot/slot").gameObject.SetActive(true);
                });
            }

            SoundManager.instance.PlaySfxSound(IsGoldGacha ? eUISfx.UI_gatcha_normal_open : eUISfx.UI_gatcha_rare_open, false);
            TempCoroutine.instance.FrameDelay(CardDuration, delegate () {
                BtnAgain.gameObject.SetActive(true);
                BtnClose.gameObject.SetActive(true);
            });
        } );

        EventDelegate.Set(Popup.transform.FindChild("BtnOk").GetComponent<UIEventTrigger>().onClick, delegate() { Popup.SetActive(false); } );

        ResultAlpha.enabled = false;
        ResultAlpha.gameObject.SetActive(true);

        AniObj.SetActive(false);
        Popup.SetActive(false);

        RateView.SetActive(false);
        GachaView.SetActive(true);
        ChangeView(true);//리스트 뷰로 시작

        PanhoLbls[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(946), GetToDayCount(true) );
        PanhoLbls[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(946), GetToDayCount(false) );
    }

    public override void LateInit()
    {
        base.LateInit();

        if (parameters.Length == 0)
            return;

        uint norTime = (uint)parameters[0];
        uint serTime = (uint)parameters[1];

        OnTimeUpdate(norTime, serTime);
    }

    /// <summary> 리스트에 있는 박스들 셋팅용 </summary>
    void SetBox(Transform tf, bool isOnce, bool isGold, bool bSetDelegateBtn=true)
    {
		if (bSetDelegateBtn) {
			UIEventTrigger uiTri = tf.FindChild ("Btn").GetComponent<UIEventTrigger> ();
			EventDelegate.Set (uiTri.onClick, delegate() {
                int toDayCount = GetToDayCount(isGold);
                if ( (isOnce && toDayCount <= 0) || (!isOnce && toDayCount != 10 ) )//일일 제한 횟수 에러
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 377);
                else
                    BtnOpenResult (isOnce, isGold);
			});
		}

        UILabel price = tf.FindChild("Btn/price").GetComponent<UILabel>();
        tf.FindChild("timer").gameObject.SetActive(isOnce);
		if (tf.FindChild ("amount") != null) {
			tf.FindChild ("amount").gameObject.SetActive (!isOnce);
		}
        
        if (!isOnce)//10개 짜리
        {
            UILabel amount = tf.FindChild("amount").GetComponent<UILabel>();
            amount.text = string.Format("{0}", "X 10");
            
            uint value = 0;
            if (isGold)
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(399);
                value = priceInfo.ResetValue;
            }
            else
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(401);
                value = priceInfo.ResetValue;
            }

			price.text = SceneManager.instance.NumberToString(value);// string.Format("{0}", value.ToString("#,##"));
        }
        else//1개 짜리
        {
            GameObject iconGo = tf.FindChild("Btn/icon").gameObject;
            if (isGold && !IsChargeGold)//골드 무료
            {
                price.text = _LowDataMgr.instance.GetStringCommon(523);
                iconGo.SetActive(false);
            }
            else if (isGold && IsChargeGold)//골드 유료
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(398);
				price.text = SceneManager.instance.NumberToString(priceInfo.ResetValue);// string.Format("{0}", priceInfo.ResetValue.ToString("#,##"));
                iconGo.SetActive(true);
            }

            if (!isGold && !IsChargeCash)//캐쉬 무료
            {
                price.text = _LowDataMgr.instance.GetStringCommon(523);
                iconGo.SetActive(false);
            }
            else if (!isGold && IsChargeCash)//캐쉬 유료
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(400);
				price.text = SceneManager.instance.NumberToString(priceInfo.ResetValue);// string.Format("{0}", priceInfo.ResetValue.ToString("#,##"));
                iconGo.SetActive(true);
            }
        }
    }

    /// <summary> 리스트 박스의 뽑기 클릭시 처리 이벤트 </summary>
    void BtnOpenResult(bool isOnce, bool isGold)
    {
        if (isOnce)//일회 뽑기
        {
            if (isGold)
            {
                if(IsChargeGold)//골드 검사해야함 공짜가 아니므로
                {
                    Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(398);
                    if (NetData.instance.GetAsset(AssetType.Gold) < priceInfo.ResetValue)//돈 부족
                    {
                        //팝업
                        PopupMsg.text = _LowDataMgr.instance.GetStringCommon(443);
                        Popup.SetActive(true);//골드 부족
                    }
                    else{
                        IsAni = true;

                        NetworkClient.instance.SendPMsgLotteryBoxCommonC();
					}
                }
                else //공짜임
                {
                    IsAni = true;
                    NetworkClient.instance.SendPMsgLotteryBoxCommonFreeC();
                }
                //return;
            }
            else
            {
                if(IsChargeCash)//캐쉬 검사해야함 공짜가 아니므로
                {
                    Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(400);
                    if (NetData.instance.GetAsset(AssetType.Cash) < priceInfo.ResetValue)//캐쉬 부족
                    {
                        //팝업
                        PopupMsg.text = _LowDataMgr.instance.GetStringCommon(450);
                        Popup.SetActive(true);//캐쉬 부족
                    }
                    else{
                        IsAni = true;
                        NetworkClient.instance.SendPMsgLotteryBoxSeniorC();
					}
                }
                else //공짜임
                {
                    IsAni = true;
                    NetworkClient.instance.SendPMsgLotteryBoxSeniorFreeC();
                }
                //return;
            }
            
        }
        else//10회 뽑기
        {
            if(isGold)//골드
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(399);
                if (NetData.instance.GetAsset(AssetType.Gold) < priceInfo.ResetValue)//돈 부족
                {
                    //팝업
                    PopupMsg.text = _LowDataMgr.instance.GetStringCommon(443);
                    Popup.SetActive(true);//골드 부족
                }
                else
                {
                    IsAni = true;
                    NetworkClient.instance.SendPMsgLotteryBoxCommonManytimesC();
                }
            }
            else//캐쉬
            {
                Price.PriceInfo priceInfo = _LowDataMgr.instance.GetLowDataPriceInfo(401);
                if (NetData.instance.GetAsset(AssetType.Cash) < priceInfo.ResetValue)//캐쉬 부족
                {
                    //팝업
                    PopupMsg.text = _LowDataMgr.instance.GetStringCommon(450);
                    Popup.SetActive(true);//캐쉬 부족
                }
                else
                {
                    IsAni = true;
                    NetworkClient.instance.SendPMsgLotteryBoxSeniorManytimesC();
                }
            }
        }

    }

    /// <summary> 리스트 뷰 or 결과 뷰 </summary>
    void ChangeView(bool isList)
    {
        ViewObjs[0].SetActive(isList);
        ViewObjs[1].SetActive(!isList);
    }

    public override void Hide()
    {
        //StopAllCoroutines();
        base.Hide();
    }
    
    public bool Check()
    {
        if (!BtnClose.gameObject.activeSelf)
            return true;
        else
           return false;
        
    }

    public override void Close()
    {
        if (Popup.activeSelf)
        {
            Popup.SetActive(false);
            return;
        }

        if(RateView.activeSelf)
        {
            uiMgr.SetTopMenuTitleName(539);
            RateView.SetActive(false);
            GachaView.SetActive(true);
        }
        else
        {
            if (ViewObjs[1].activeSelf)
            {
                if (!BtnClose.gameObject.activeSelf)
                    return;

                ChangeView(true);
            }
            else
            {
                TempCoroutine.instance.StopAllCoroutines();
                base.Close();
                if (uiMgr.IsShop)
                {
                    System.TimeSpan goldTime = System.DateTime.Now.AddSeconds(SceneManager.instance.GetGachaFreeTime[0]) - System.DateTime.Now;
                    System.TimeSpan cashTime = System.DateTime.Now.AddSeconds(SceneManager.instance.GetGachaFreeTime[1]) - System.DateTime.Now;

                    if (0 >= goldTime.TotalSeconds || 0 >= cashTime.TotalSeconds)
                    {
                        //UIMgr.instance.AlramList[(int)AlramIconType.SHOP] = 1;
                        SceneManager.instance.SetAlram(AlramIconType.SHOP, true);
                    }
                    else
                    {
                        SceneManager.instance.SetAlram(AlramIconType.SHOP, false);
                    }

                    UIMgr.OpenShopPanel();
                }
                    
                else
                    UIMgr.OpenTown();
            }
        }
    }

    void LateUpdate()
    {
        if (IsChargeGold)//골드 무료 박스 시간 갱신
        {
            System.TimeSpan goldTime = GoldFreeTimer-System.DateTime.Now;
            if (0 <= goldTime.TotalSeconds)
            {
                if (goldTime.Days <= 0)//시 분 초 단위
                {
                    GoldTimer.text = string.Format("{0:00}:{1:00}:{2:00} {3}",
                        goldTime.Hours,
                        goldTime.Minutes,
                        goldTime.Seconds, 
                        TimeStr );
                }
                else//하루 이상
                {
                    GoldTimer.text = string.Format(_LowDataMgr.instance.GetStringCommon(530), goldTime.Days);
                }
            }
            else//무료가 됨
            {
                Transform goldTf = transform.FindChild("GachaView/List/Gold/Scroll/Grid");

                IsChargeGold = false;
                //GoldTimer.text = string.Format("00:00:00 {0}", TimeStr);
                GoldTimer.text = _LowDataMgr.instance.GetStringCommon(976);
                SetBox(goldTf.GetChild(0), true, true);//골드
            }
        }

        if (IsChargeCash)//캐쉬 무료 박스 시간 갱신
        {
            System.TimeSpan cashTime = CashFreeTimer - System.DateTime.Now;
            if (0 <= cashTime.TotalSeconds)
            {
                if (cashTime.Days <= 0)
                {
                    CashTimer.text = string.Format("{0:00}:{1:00}:{2:00} {3}",
                        cashTime.Hours,
                        cashTime.Minutes,
                        cashTime.Seconds,
                        TimeStr);
                }
                else
                {
                    CashTimer.text = string.Format(_LowDataMgr.instance.GetStringCommon(530), cashTime.Days);
                }
            }
            else//무료가 됨
            {
                Transform cashTf = transform.FindChild("GachaView/List/Cash/Scroll/Grid");

                IsChargeCash = false;
                //CashTimer.text = string.Format("00:00:00 {0}", TimeStr);
                CashTimer.text = _LowDataMgr.instance.GetStringCommon(976);
                SetBox(cashTf.GetChild(0), true, false);//캐쉬
            }
        }
    }

    IEnumerator CardAction( bool isOne)
    {
        ResultAlpha.ResetToBeginning();

        yield return new WaitForSeconds(BoxEffTime);
        
        int childCount = AniObj.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject go = AniObj.transform.GetChild(i).gameObject;
            Destroy(go);
        }
        AniObj.SetActive(false);

        if (isOne)
        {
            ResultOneCard.gameObject.SetActive(true);
            
            UIPlayTween tween = ResultOneCard.GetComponent<UIPlayTween>();
            tween.tweenGroup = 1;
            tween.resetOnPlay = true;
            tween.Play(true);
            //yield return new WaitForSeconds(alpha.duration+ alpha.delay);
        }
        else
        {
            ResultCards[0].parent.gameObject.SetActive(true);
            
            int length = ResultCards.Length;
            for (int i = 0; i < length; i++)
            {
                ResultCards[i].gameObject.SetActive(false);
            }
            
            int index = 0;
            bool[] range = new bool[length];
            while(index < length)
            {
                int count = Random.Range(0, length);
                if (range[count])
                    continue;

                range[count] = true;
                ResultCards[count].gameObject.SetActive(true);
                UIPlayTween tween = ResultCards[count].GetComponent<UIPlayTween>();
                tween.tweenGroup = 1;
                tween.resetOnPlay = true;
                tween.Play(true);

                yield return new WaitForSeconds(StayTime);
                ++index;
            }
        }
        
        yield return new WaitForSeconds(CardActionDelay);

        ResultAlpha.ResetToBeginning();
        ResultAlpha.PlayForward();

        if (isOne)
        {
            ResultOneCard.collider.enabled = true;
            UIPlayTween tween = ResultOneCard.GetComponent<UIPlayTween>();
            tween.tweenGroup = 2;
            tween.resetOnPlay = true;
            tween.Play(true);
        }
        else
        {
            int length = ResultCards.Length;
            for (int i = 0; i < length; i++)
            {
                Transform tf = ResultCards[i];
                tf.collider.enabled = true;

                UIPlayTween tween = tf.GetComponent<UIPlayTween>();
                tween.tweenGroup = 2;
                tween.resetOnPlay = true;
                tween.Play(true);
            }

            //BtnOpenCard.gameObject.SetActive(true);
            BtnOpenAllCard.gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(BoxDeleteDelay);

        ResultText.SetActive(true);
        IsAni = false;
    }

    public bool IsCurResult
    {
        get {
            if (ViewObjs[1].activeSelf)
                return true;

            return false;
        }
    }

    /// <summary> 판호용 일일 제한 카운트. </summary>
    int GetToDayCount(bool gold)
    {
#if !UNITY_EDITOR
        string pathKey = string.Format("GachaToDayCount_{0}.json", NetData.instance.UUID);
        string data = PlayerPrefs.GetString(pathKey, "");
        if (string.IsNullOrEmpty(data))//처음함 데이터가 없다.
        {
            data = "{";
            data += string.Format("\"Day\":{0}", System.DateTime.Today.DayOfYear);
            data += string.Format(",\"GoldCount\":{0}", 10);
            data += string.Format(",\"CashCount\":{0}", 10);
            data += "}";
            
            PlayerPrefs.SetString(pathKey, data);
            return 10;
        }

        JSONObject jsonPar = new JSONObject(data);
        int time = (int)jsonPar["Day"].n;//long.Parse(.ToString());
        //System.TimeSpan day = new System.TimeSpan(time-System.DateTime.Now.Ticks);
        
        if (System.DateTime.Today.DayOfYear != time)//하루 이상 지남 가능함.
        {
            data = "{";
            data += string.Format("\"Day\":{0}", System.DateTime.Today.DayOfYear);
            data += string.Format(",\"GoldCount\":{0}", 10);
            data += string.Format(",\"CashCount\":{0}", 10);
            data += "}";

            PlayerPrefs.SetString(pathKey, data);
            return 10;
        }

        int toDayCount = 0;
        if(gold)
            toDayCount = int.Parse(jsonPar["GoldCount"].ToString());
        else
            toDayCount = int.Parse(jsonPar["CashCount"].ToString());

        return toDayCount;
#else
        return 10;
#endif
    }

    void SaveToDayCount(bool isGold, int addCount)
    {
#if !UNITY_EDITOR
        string pathKey = string.Format("GachaToDayCount_{0}.json", NetData.instance.UUID);
        string data = PlayerPrefs.GetString(pathKey, "");
        JSONObject jsonPar = new JSONObject(data);
        if(isGold)
        {
            int count = int.Parse(jsonPar["GoldCount"].ToString() );
            count -= addCount;

            PanhoLbls[0].text = string.Format(_LowDataMgr.instance.GetStringCommon(946), count);
            jsonPar["GoldCount"].n = count;
        }
        else
        {
            int count = int.Parse(jsonPar["CashCount"].ToString());
            count -= addCount;

            PanhoLbls[1].text = string.Format(_LowDataMgr.instance.GetStringCommon(946), count);
            jsonPar["CashCount"].n = count;
        }

        PlayerPrefs.SetString(pathKey, jsonPar.ToString() );
#endif
        //return true;
    }

#region 넷트워크 프로토콜

    /// <summary> 한번 뽑기 </summary>
    public void OnOnce(bool isFree, bool isSenior, NetData.DropItemData itemData)
    {
        //reduceGachaOppotunity (!isSenior, 1);

        Transform goldTf = transform.FindChild("GachaView/List/Gold/Scroll/Grid");
		Transform cashTf = transform.FindChild("GachaView/List/Cash/Scroll/Grid");

        SetBox(goldTf.GetChild(0), true, true);
        SetBox(goldTf.GetChild(1), false, true);
        SetBox(cashTf.GetChild(0), true, false);
        SetBox(cashTf.GetChild(1), false, false);

        ResultCards[0].parent.gameObject.SetActive(false);
        ResultOneCard.gameObject.SetActive(false);
        ResultText.SetActive(false);
        
        Animation ani = CreateBox(isSenior, false, false);
        ani.Play(isSenior ? "box_gold_01" : "box_wood_01");

        ResultOneCard.rotation = new Quaternion(0, 0, 0, 0);//원상 복구
        ResultOneCard.collider.enabled = false;
        ResultOneCard.GetComponent<UISprite>().spriteName = ChardForward;

        OneCardSlot.gameObject.SetActive(false);
        OneCardSlot.SetLowDataItemSlot(itemData.LowDataId, itemData.Amount);

        EventDelegate.Set(ResultOneCard.FindChild("ItemRoot").transform.GetComponent<UIEventTrigger>().onClick, delegate ()
        {
            uint itemidx = itemData.LowDataId;

            if (itemData.LowDataId == 1)
                itemidx = 599000;
            else if (itemData.LowDataId == 2 || itemData.LowDataId == 8)
                itemidx = 599001;
            else if (itemData.LowDataId == 5)
                itemidx = 599002;
            else
                itemidx = itemData.LowDataId;

            OnclicItemPopup(itemidx);
        });


        StartCoroutine("CardAction", true );

        IsGoldGacha = !isSenior;
        IsOnceGacha = true;

        //BtnOpenCard.gameObject.SetActive(false);
        BtnOpenAllCard.gameObject.SetActive(false);
        BtnAgain.gameObject.SetActive(false);
        BtnClose.gameObject.SetActive(false);
        ChangeView(false);

        if(isFree)//타임 재갱신이 필요함.
        {
            NetworkClient.instance.SendPMsgLotteryQueryInfoC();
        }

        SaveToDayCount(IsGoldGacha, 1);
        AniObj.SetActive(true);
    }

    /// <summary> 아이템 상세창 </summary>
    void OnclicItemPopup(uint id)
    {
        UIMgr.OpenDetailPopup(this, id);
        //Vector2 position = pos.transform.position;
        //UIMgr.OpenClickPopup(id, position);
    }

    /// <summary> 무료 일반, 고급 뽑기시 시간 재갱신용 </summary>
    public void OnTimeUpdate(uint norTime, uint serTime)
    {
        //골드
        GoldFreeTimer = System.DateTime.Now.AddSeconds(norTime);
        IsChargeGold = norTime != 0;
        
        if( !IsChargeGold)//무료
            GoldTimer.text = _LowDataMgr.instance.GetStringCommon(976);
            //GoldTimer.text = string.Format("00:00:00 {0}", TimeStr);

        Transform goldTf = transform.FindChild("GachaView/List/Gold/Scroll/Grid");
        SetBox(goldTf.GetChild(0), true, true);
        
        //캐쉬
        CashFreeTimer = System.DateTime.Now.AddSeconds(serTime);
        IsChargeCash = serTime != 0;

        if (!IsChargeCash)//무료
            CashTimer.text = _LowDataMgr.instance.GetStringCommon(976);
            //CashTimer.text = string.Format("00:00:00 {0}", TimeStr);

        Transform cashTf = transform.FindChild("GachaView/List/Cash/Scroll/Grid");
        SetBox(cashTf.GetChild(0), true, false);
    }
    
    /// <summary> 10회 뽑기 </summary>
    public void OnTen(bool isSenior, NetData.DropItemData[] itemArr)
    {
		//reduceGachaOppotunity (!isSenior, 10);
		// refresh
		Transform goldTf = transform.FindChild("GachaView/List/Gold/Scroll/Grid");
		Transform cashTf = transform.FindChild("GachaView/List/Cash/Scroll/Grid");
		SetBox(goldTf.GetChild(0), true, true);//골드
		SetBox(goldTf.GetChild(1), false, true);//골드
		SetBox(cashTf.GetChild(0), true, false);//캐쉬
		SetBox(cashTf.GetChild(1), false, false);//캐쉬
		/////////
		
		ResultCards[0].parent.gameObject.SetActive(false);
        ResultOneCard.gameObject.SetActive(false);
        ResultText.SetActive(false);

        AniObj.SetActive(true);
        Animation ani = CreateBox(isSenior, false, true);
        ani.Play(isSenior ? "box_gold_01" : "box_wood_01");

        int length = ResultCards.Length;
        for (int i = 0; i < length; i++)
        {
            Transform tf = ResultCards[i];
            if (tf == null)
            {
                Debug.LogError("is null error child number " + i);
                continue;
            }
            else if (itemArr.Length <= i)
                continue;

            tf.rotation = new Quaternion(0, 0, 0, 0);//원상 복구
            tf.collider.enabled = false;
            tf.GetComponent<UISprite>().spriteName = ChardForward;
            InvenItemSlotObject slot = tf.FindChild("ItemRoot/slot").GetComponent<InvenItemSlotObject>();
            slot.SetLowDataItemSlot(itemArr[i].LowDataId, itemArr[i].Amount);
            tf.FindChild("ItemRoot/slot").gameObject.SetActive(false);

            int idx = i;
            EventDelegate.Set(tf.FindChild("ItemRoot").transform.GetComponent<UIEventTrigger>().onClick, delegate ()
            {
                uint itemidx = itemArr[idx].LowDataId;

                if (itemArr[idx].LowDataId == 1)
                    itemidx = 599000;
                else if (itemArr[idx].LowDataId == 2 || itemArr[idx].LowDataId == 8)
                    itemidx = 599001;
                else if(itemArr[idx].LowDataId == 5)
                    itemidx = 599002;
                else
                    itemidx = itemArr[idx].LowDataId;

                OnclicItemPopup(itemidx);
            });
        }
        
        StartCoroutine("CardAction", false);
        IsGoldGacha = !isSenior;
        IsOnceGacha = false;

        SaveToDayCount(IsGoldGacha, 10);
        
        //BtnOpenCard.gameObject.SetActive(false);
        BtnOpenAllCard.gameObject.SetActive(false);
        BtnAgain.gameObject.SetActive(false);
        BtnClose.gameObject.SetActive(false);
        ChangeView(false);
    }

    Animation CreateBox(bool isSenior, bool isDummy, bool isTen)
    {
		/*
        string modelName = isSenior ? "box_gold_01" : "box_wood_01";
        GameObject go = Instantiate(Resources.Load(string.Format("UI/UIObject/{0}", modelName))) as GameObject;
        go.transform.parent = AniObj.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        NGUITools.SetLayer(go, LayerMask.NameToLayer("UILayer") );
        go.transform.SetChildLayer(LayerMask.NameToLayer("UILayer"));
        */

		//UIAtlas atlas = Resources.LoadAssetAtPath("Assets/Sources/UI/UploadAtlas/" + atlasName + ".prefab", typeof(UIAtlas)) as UIAtlas;

		string modelName = isSenior ? "box_gold_cutscene" : "box_wood_cutscene";
		GameObject go = Instantiate(Resources.Load(string.Format("UI/UIObject/{0}", modelName), typeof(GameObject))) as GameObject;
		go.transform.parent = AniObj.transform;
		go.transform.localPosition = new Vector3 (-100, 0, 0);//Vector3.zero;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localScale = Vector3.one;
		
		//NGUITools.SetLayer(go, LayerMask.NameToLayer("UILayer") );
		//go.transform.SetChildLayer(LayerMask.NameToLayer("UILayer"));

        if (!isDummy)
        {
            GameObject boxEff = UIHelper.CreateEffectInGame(AniObj.transform, isSenior ? "Fx_UI_gacha_02" : "Fx_UI_gacha_01", true);
			//GameObject boxEff = UIHelper.CreateEffectInGame(AniObj.transform, isTen ? "Fx_UI_gacha_01" : "Fx_UI_gacha_02");
            boxEff.transform.localEulerAngles = -AniObj.transform.localEulerAngles;

			boxEff.transform.localPosition = new Vector3(-100,0,0);
            
            if( !isTen)
                SoundManager.instance.PlaySfxSound(isSenior ? eUISfx.UI_gatcha_rare_drop : eUISfx.UI_gatcha_normal_drop, false);
            else
                SoundManager.instance.PlaySfxSound(isSenior ? eUISfx.UI_gatcha_rare_ten : eUISfx.UI_gatcha_normal_ten, false);
        }
		string childAnimationObjName = "box_gold_01"; 
		if (!isSenior) {
			childAnimationObjName = "box_wood_01"; 
		}
		return go.transform.FindChild (childAnimationObjName).GetComponent<Animation> ();

		// return go.GetComponent<Animation> ();
    }

#endregion

#region 확률 팝업
    void SetRateView()
    {
        //uiMgr.SetTopMenuTitleName(539);

        if (RateGrid.transform.childCount < 2)//최초 한번.
        {
            Transform OriginSlotTf = RateGrid.transform.GetChild(0);
            for (int i = 0; i < 200; i++)
            {
                Transform slotTf = Instantiate(OriginSlotTf) as Transform;
                slotTf.parent = RateGrid.transform;
                slotTf.localPosition = Vector3.zero;
                slotTf.localScale = Vector3.one;
                slotTf.name = "slot" + i;
                slotTf.gameObject.SetActive(false);
            }

            EventDelegate.Set(RateSideTabBtnTf.GetChild(0).GetComponent<UIButton>().onClick, delegate ()
            {
                CurRateTabIdx = 0;
                for (int i = 0; i < 2; i++)
                {
                    Transform btn = RateSideTabBtnTf.GetChild(i);
                    btn.FindChild("tab_on").gameObject.SetActive(i == 0);
                    btn.FindChild("tab_off").gameObject.SetActive(i != 0);
                }

                TabGroup.CoercionTab(0);
            });
            EventDelegate.Set(RateSideTabBtnTf.GetChild(1).GetComponent<UIButton>().onClick, delegate ()
            {
                CurRateTabIdx = 1;
                for (int i = 0; i < 2; i++)
                {
                    Transform btn = RateSideTabBtnTf.GetChild(i);
                    btn.FindChild("tab_on").gameObject.SetActive(i == 1);
                    btn.FindChild("tab_off").gameObject.SetActive(i != 1);
                }

                TabGroup.CoercionTab(0);
            });

            TabGroup.Initialize(OnClickUpBtn);
            CurRateTabIdx = 0;
            for (int i = 0; i < 2; i++)
            {
                Transform btn = RateSideTabBtnTf.GetChild(i);
                btn.FindChild("tab_on").gameObject.SetActive(i == 0);
                btn.FindChild("tab_off").gameObject.SetActive(i != 0);
            }
        }

        TabGroup.CoercionTab(0);
        GachaView.SetActive(false);
        RateView.SetActive(true);
    }

    void OnClickUpBtn(int viewType)
    {
        List<GatchaReward.RewardInfo> list = null;
        switch ((eRateViewType)viewType)
        {
            case eRateViewType.all:// 12 , 13, 11, 8, 10, 14
                list = _LowDataMgr.instance.GetGachaRewardItemLsit(12, CurRateTabIdx, true);
                break;
            case eRateViewType.equip://12
                list = _LowDataMgr.instance.GetGachaRewardItemLsit(12, CurRateTabIdx);
                break;
            case eRateViewType.item://13
                list = _LowDataMgr.instance.GetGachaRewardItemLsit(13, CurRateTabIdx);
                break;
            case eRateViewType.piece://11
                list = _LowDataMgr.instance.GetGachaRewardItemLsit(11, CurRateTabIdx);
                break;
            case eRateViewType.etc://8 , 10, 14
                list = _LowDataMgr.instance.GetGachaRewardItemLsit(8, CurRateTabIdx);
                list.InsertRange(list.Count, _LowDataMgr.instance.GetGachaRewardItemLsit(10, CurRateTabIdx));
                list.InsertRange(list.Count, _LowDataMgr.instance.GetGachaRewardItemLsit(14, CurRateTabIdx));
                break;
        }

        byte charClass = _LowDataMgr.instance.GetCharcterData(NetData.instance.GetUserInfo()._userCharIndex).Class;
        for (int i = 0; i < RateGrid.transform.childCount; i++)
        {
            if (i >= list.Count || list[i].ClassType != 99 && list[i].ClassType != charClass)
            {
                RateGrid.transform.GetChild(i).gameObject.SetActive(false);
                continue;
            }

            GameObject slot = RateGrid.transform.GetChild(i).gameObject;
            slot.SetActive(true);

            UISprite itemImg = slot.transform.FindChild("imgae").GetComponent<UISprite>();//아이콘
            UILabel name = slot.transform.FindChild("Txt_itemname").GetComponent<UILabel>();//이름
            UILabel percent = slot.transform.FindChild("Txt_percent").GetComponent<UILabel>();
            UILabel amount = slot.transform.FindChild("Txt_amount").GetComponent<UILabel>();//개수
            UISprite grade = itemImg.transform.FindChild("grade").GetComponent<UISprite>();
            UISprite gradeBg = itemImg.transform.FindChild("bg_grade").GetComponent<UISprite>();

            percent.text = string.Format("{0} : {1}%", _LowDataMgr.instance.GetStringCommon(949), (list[i].ItemIdxRate * 0.0001).ToString());//만분률
            amount.text = list[i].ShardMin <= 1 ? "" : list[i].ShardMin.ToString();

            uint itemIdx = list[i].ItemIdx;
            if (0 < itemIdx)
            {
                if (list[i].Type == 12)//장비임
                {
                    Item.EquipmentInfo eLowData = _LowDataMgr.instance.GetLowDataEquipItemInfo(itemIdx);
                    if (eLowData != null)//장비아이템이 드랍아이템 대표로 등록되어 있음
                    {
                        itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.EquipItem);

                        itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(eLowData.Icon);
                        name.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(eLowData.Grade), _LowDataMgr.instance.GetStringItem(eLowData.NameId));

                        grade.spriteName = string.Format("Icon_0{0}", eLowData.Grade);
                        gradeBg.spriteName = string.Format("Icon_bg_0{0}", eLowData.Grade);
                    }
                }
                else if (list[i].Type == 10)// 초상화일수있으니 다시검ㅅ ㅏ&& _LowDataMgr.instance.IsGetRewardType(10, itemIdx)
                {
                    itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Face);

                    Partner.PartnerDataInfo pInfo = _LowDataMgr.instance.GetPartnerInfo(itemIdx);
                    if (pInfo == null)
                    {
                        RateGrid.transform.GetChild(i).gameObject.SetActive(false);
                        continue;
                    }

                    itemImg.spriteName = pInfo.PortraitId;
                    name.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(pInfo.Quality), _LowDataMgr.instance.GetStringUnit(pInfo.NameId));
                    grade.spriteName = string.Format("Icon_0{0}", pInfo.Quality);
                    gradeBg.spriteName = string.Format("Icon_bg_0{0}", pInfo.Quality);
                }
                else//소비아이템
                {
                    Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(itemIdx);
                    name.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(uLowData.Grade), _LowDataMgr.instance.GetStringItem(uLowData.NameId));
                    grade.spriteName = string.Format("Icon_0{0}", uLowData.Grade);

                    if (uLowData.Type == (int)AssetType.Jewel)
                    {
                        gradeBg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.EquipItem);
                        gradeBg.spriteName = string.Format("Icon_bg_0{0}", uLowData.Grade);
                    }
                    else
                    {
                        gradeBg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);
                        gradeBg.spriteName = "Bod_IconBg";
                    }

                    if (uLowData.Type == (int)AssetType.PartnerShard || uLowData.Type == (int)AssetType.CostumeShard)
                    {
                        itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.Shard);
                        //gradeBg.atlas = AtlasMgr.instance.GetLoadAtlas(PoolAtlasType.Bod);
                        //gradeBg.spriteName = "Bod_IconBg";
                    }
                    else
                        itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.UseItem);

                    itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);
                }
            }
            else//아이템 아이디가 없는것들
            {
                switch (list[i].Type)
                {
                    case 1://게임 머니
                        itemIdx = 599000;
                        break;
                    case 8://에너지
                        itemIdx = 599104;
                        break;
                    case 2://게임 캐쉬(원보)
                        itemIdx = 599001;
                        break;
                    case 5://휘장
                        itemIdx = 599002;
                        break;
                    case 7://성망
                        itemIdx = 599003;
                        break;
                    case 3://공헌
                        itemIdx = 599004;
                        break;
                    case 4://명예
                        itemIdx = 599005;
                        break;
                    case 6://사자왕휘장
                        itemIdx = 599006;
                        break;
                }

                if (itemIdx <= 0)
                {
                    Debug.LogError("UnDefined type error " + list[i].Type);
                    continue;
                }
                Item.ItemInfo uLowData = _LowDataMgr.instance.GetUseItem(itemIdx);

                itemImg.atlas = AtlasMgr.instance.GetLoadAtlas(LoadAtlasType.UseItem);
                itemImg.spriteName = _LowDataMgr.instance.GetLowDataIcon(uLowData.Icon);

                name.text = string.Format("{0}{1}[-]", UIHelper.GetItemGradeColor(uLowData.Grade), _LowDataMgr.instance.GetStringItem(uLowData.NameId));
                grade.spriteName = string.Format("Icon_0{0}", uLowData.Grade);
            }

            int idx = i;
            EventDelegate.Set(slot.GetComponent<UIEventTrigger>().onClick, delegate ()
            {
                if (itemIdx <= 0 || list[idx].Type == 10 || list[idx].Type == 14)//list[idx].Type == 1 ||list[idx].Type == 10 || list[idx].Type == 8 ||
                    return;

                Vector2 position = itemImg.transform.position;
                OnclicItemPopup(itemIdx);
            });
        }

        RateGrid.repositionNow = true;
        RateScroll.ResetPosition();
    }

    #endregion

    public bool IsEndAni
    {
        get {
            return !IsAni;
        }
    }
}

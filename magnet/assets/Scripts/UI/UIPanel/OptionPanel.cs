using UnityEngine;
using System.Collections;

public enum TypeSDKUseType
{
    NONE = 0,
    BIND_FACEBOOK = 1,
    LOGIN_FACEBOOK = 2,
    BIND_GOOGLE = 1,
    LOGIN_GOOGLE = 2,
}

public class OptionPanel : UIBasePanel
{
    public GameObject TownOption;
    public GameObject IngameOption;

    public GameObject[] SettingObj; //기기세팅,게임세팅
    
    public UIButton BtnClose;
    public UIButton BtnClosePopup;

    // 화질선택버튼
    public UIButton[] BtnQuality;

    // 음향설정버튼
    public UIButton[] BtnBGM;
    public UIButton[] BtnSoundfx;
    public UISlider SoundSlider;    //음량조절

    //게임세팅 체크
    public UIEventTrigger[] HpCheck;
    public UIEventTrigger[] InviteCheck;
    public UIEventTrigger[] NameCheck;
    public UIEventTrigger[] WhisperCheck;

    public UIEventTrigger[] AlramCheck;

    //인게임에서 옵션리스트버튼
    public UIButton[] IngameListBtn;

    // 하단버튼
    public UIEventTrigger BtnLogin;

    public UIEventTrigger BtnFacebookBind;
    public UIEventTrigger BtnFacebookLogin;
    public TypeSDKUseType SDKUseType;
    public UILabel lbLoginType;

    public string user_id;
    public string user_token;

    //public UIButton BtnChange;
    //public UIButton BtnCommunity;

    // Sprite이미지
    //public UISprite bgmOn;
    //public UISprite soundfxOn;
    public UISprite[] QualSprite;
    //public UISprite ShowHeadOn;

    public UITabGroup Tap;

    // 팝업창
    //public GameObject CustomerServicePopup;
    //public GameObject CouponPopup;

    public UILabel ServerName;

    //public UIInput InputCoupon;

    byte QualityPressed;
    //bool ShowHeadPressed;

    SceneManager.OptionData option;
    //Joystick joystick;

    public override void Init()
    {
        base.Init();

        option = SceneManager.instance.optionData;

        LoginTypeLabelSetting();

#if UNITY_EDITOR || UNITY_IOS
        BtnFacebookBind.gameObject.SetActive(false);
        BtnFacebookLogin.gameObject.SetActive(false);
#else 
        if (NetData.instance.GetLoginType() == eLoginType.GUEST)
        {
            BtnFacebookBind.gameObject.SetActive(true);
            BtnFacebookLogin.gameObject.SetActive(true);
        }
        if (NetData.instance.GetLoginType() == eLoginType.FACEBOOK)
        {
            BtnFacebookBind.gameObject.SetActive(false);
            BtnFacebookLogin.gameObject.SetActive(true);
        }
        if (NetData.instance.GetLoginType() == eLoginType.GOOGLE)
        {
            BtnFacebookBind.gameObject.SetActive(false);
            BtnFacebookLogin.gameObject.SetActive(true);
        }
#endif

        BtnEvents();
        GetBtnState();

        //CustomerServicePopup.SetActive(false);
        //CouponPopup.SetActive(false);
        ServerName.text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(982), option.ServerName);

        Tap.Initialize(delegate (int arr)
        {
            for (int i = 0; i < SettingObj.Length; i++)
                SettingObj[i].SetActive(i == arr);
           
        });

        SDKUseType = TypeSDKUseType.NONE;

        if ( !TownState.TownActive)//인게임으로 인지.
        {
            BtnLogin.collider.enabled = false;
            BtnLogin.transform.GetComponent<UISprite>().color = Color.gray;
            BtnLogin.transform.FindChild("label").GetComponent<UILabel>().color = Color.gray;
        }

        transform.FindChild("Setpup/RoleID").GetComponent<UILabel>().text = string.Format("RoleID : {0}", NetData.instance.GetUserInfo().GetCharUUID());
    }
    byte openType = 0;

    public void LoginTypeLabelSetting()
    {
        if(NetData.instance.GetLoginType() == eLoginType.GUEST)
        {
            lbLoginType.text = "GUEST LOGIN : " + NetData.instance.GetUserID();
        }
        else if (NetData.instance.GetLoginType() == eLoginType.FACEBOOK)
        {
            lbLoginType.text = "FACEBOOK LOGIN : " + NetData.instance.GetUserID();
        }
        else if (NetData.instance.GetLoginType() == eLoginType.GOOGLE)
        {
            lbLoginType.text = "GOOGLE LOGIN : " + NetData.instance.GetUserID();
        }
    }

    public override void LateInit()
    {
        base.LateInit();

        if(parameters !=null && parameters[0] !=null)
        {
            openType = (byte)parameters[0];
        }

        switch (openType)
        {
            case 0:
                //마을에서누른것 
                TownOption.SetActive(true);
                IngameOption.SetActive(false);
                Tap.CoercionTab(0);
                break;
            case 1:
                //채팅에서 누른것
                TownOption.SetActive(true);
                IngameOption.SetActive(false);
                Tap.CoercionTab(1);

                break;
            case 2:
                //인게임에서 누른것
                TownOption.SetActive(false);
                IngameOption.SetActive(true);
                break;
        
        }

        //TownOption.SetActive(!isIngame);
        //IngameOption.SetActive(isIngame);
    }

    /// <summary> SceneManager에서 셋팅값 받아옴 </summary>
    void GetBtnState()
    {
        QualityPressed = option.Quality;

        BtnBGM[0].gameObject.SetActive(option.Bgm);
        BtnBGM[1].gameObject.SetActive(!option.Bgm);

        BtnSoundfx[0].gameObject.SetActive(option.Soundfx);
        BtnSoundfx[1].gameObject.SetActive(!option.Soundfx);

        SoundSlider.value = option.SoundVolume;

        SetCheckOption(InviteCheck, option.BlockInvite);
        SetCheckOption(HpCheck, option.ShowHpBar);
        SetCheckOption(NameCheck, option.ShowName);
        SetCheckOption(WhisperCheck, option.BlockWhisper);
        SetCheckOption(AlramCheck, option.OffAlram);


        OnClickQuality(QualityPressed);

        //if (!BgmPressed)
        //{
        //    bgmOn.gameObject.SetActive(false);
        //    BtnBGM.transform.FindChild("show").gameObject.SetActive(false);
        //}
        //if (!SoundfxPressed)
        //{
        //    soundfxOn.gameObject.SetActive(false);
        //    BtnSoundfx.transform.FindChild("radialoBox").FindChild("show").gameObject.SetActive(false);
        //}
    }
   
    void SetCheckOption(UIEventTrigger[] tri, string optionData)
    {

        /*
         1-> 체크안됨
         2-> 체크된거

        111 => ALL 
        222 => NONE 
         */
         
        string tmp = "";
        for (int i = 0; i < tri.Length; i++)
            tmp += "2";

        for (int i = 0; i < tri.Length; i++)
        {
            if (optionData == tmp)
            {
                tri[i].transform.FindChild("show").gameObject.SetActive(true);
            }
            else
            {
                if (i == 0)
                    tri[i].transform.FindChild("show").gameObject.SetActive(false);
                else
                    tri[i].transform.FindChild("show").gameObject.SetActive(optionData[i] == '2');
            }
        }
    }

    void TypeSdkInit()
    {
        U3DTypeSDK.Instance.AddEventDelegate(TypeEventType.EVENT_INIT_FINISH, InitFinishResult);
        U3DTypeSDK.Instance.AddEventDelegate(TypeEventType.EVENT_ERROR, ErrorEvent);
        U3DTypeSDK.Instance.AddEventDelegate(TypeEventType.EVENT_LOGIN_SUCCESS, LoginResult);
        //U3DTypeSDK.Instance.AddEventDelegate(TypeEventType.EVENT_PAY_RESULT, PayResult);

        //SceneManager.instance.SetConsoleView(true);

        U3DTypeSDK.Instance.InitSDK();
    }

    void TypeSdkFaceBookLogin()
    {
        U3DTypeSDK.Instance.Login(U3DTypeAttName.LOGIN_CHANNEL_FACEBOOK);
    }

    void InitFinishResult(U3DTypeEvent evt)
    {
        Debug.Log("InitFinishResult:" + evt.evtType);

        var enumerator = evt.evtData.attMap().GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log("Key:" + enumerator.Current.Key + " Value:" + enumerator.Current.Value);
        }
    }

    void ErrorEvent(U3DTypeEvent evt)
    {
        Debug.Log("ErrorEvent:" + evt.evtType);

        var enumerator = evt.evtData.attMap().GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log("Key:" + enumerator.Current.Key + " Value:" + enumerator.Current.Value);
        }
    }

    void LoginResult(U3DTypeEvent evt)
    {
        Debug.Log("LoginResult:" + evt.evtType);

        var enumerator = evt.evtData.attMap().GetEnumerator();
        while (enumerator.MoveNext())
        {
            Debug.Log("Key:" + enumerator.Current.Key + " Value:" + enumerator.Current.Value);
        }

        if (evt.evtType == TypeEventType.EVENT_LOGIN_SUCCESS)
        {
            if (SDKUseType == TypeSDKUseType.BIND_FACEBOOK)
            {
                user_id = evt.evtData.attMap()["user_id"].ToString();
                user_token = evt.evtData.attMap()["user_token"].ToString();

                //바인딩시도 - 서버에 바인딩이 이미 되어있는 계정인가 체크
                //NetworkClient.instance.SendPMsgUserBindFacebookC(user_id);
                NetworkClient.instance.SendPMsgUserBindQueryFbGoogleC();
            }
            else if (SDKUseType == TypeSDKUseType.LOGIN_FACEBOOK)
            {
                //로그인시도 - 서버에 로그인가능한지 쿼리
                user_id = evt.evtData.attMap()["user_id"].ToString();
                user_token = evt.evtData.attMap()["user_token"].ToString();

                OnFacebookBind(user_id);
            }
        }
    }

    public void OnCheckBindQuery(Sw.PMsgUserBindQueryFbGoogleS pmsgUserBindQueryFbGoogleS)
    {
        Debug.Log("OnCheckBindQuery :"+pmsgUserBindQueryFbGoogleS);

        if(pmsgUserBindQueryFbGoogleS.SzFacebookAccount.Length != 0)
        {
            //페이스북으로 이미 연동되어있다 - 임시 에러코드
            UIMgr.instance.AddErrorPopup((int)1111111);
            return;
        }
        else if (pmsgUserBindQueryFbGoogleS.SzGoogleAccount.Length != 0)
        {
            //구글로 이미 연동되어있다. - 임시 에러코드 
            UIMgr.instance.AddErrorPopup((int)1111111);
            return;
        }

        NetworkClient.instance.SendPMsgUserBindFacebookC(user_id);
    }

    public void OnFacebookBind(string szFacebookAccount)
    {
        PlayerPrefs.SetString("logintype", "facebook");
        PlayerPrefs.SetString("user_id", user_id);
        PlayerPrefs.SetString("user_token", user_token);

        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
        {
            UITextList.ClearTextList();
            //UIMgr.GetTownBasePanel().Close();
            //UIMgr.instance.Clear(UIMgr.UIType.System);
            UIMgr.ClearUI(true);

            NetworkClient.instance.DisconnectGameServer();//연결 종료
            NetData.instance.InitUserData();
            NetData.instance.ClearCharIdc();

            SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
        });

        //option.SaveOptionData();
        //Close();
        option.SaveOptionData();
        base.Close();
    }

    void OnClickFaceBookBind()
    {
        TypeSdkInit();
        SDKUseType = TypeSDKUseType.BIND_FACEBOOK;
        TypeSdkFaceBookLogin();
    }

    void OnClickFaceBookLogin()
    {
        TypeSdkInit();
        SDKUseType = TypeSDKUseType.LOGIN_FACEBOOK;
        TypeSdkFaceBookLogin();
    }

    void BtnEvents()
    {
        EventDelegate.Set(BtnClose.onClick, Close);

        //퀄리티
        EventDelegate.Set(BtnQuality[0].onClick, delegate () { OnClickQuality((byte)QUALITY.QUALITY_HIGH); });  
        EventDelegate.Set(BtnQuality[1].onClick, delegate () { OnClickQuality((byte)QUALITY.QUALITY_MID); });  
        EventDelegate.Set(BtnQuality[2].onClick, delegate () { OnClickQuality((byte)QUALITY.QUALITY_LOW); });

        EventDelegate.Set(SoundSlider.onChange, OnSoundSliderChange);
        EventDelegate.Set(BtnLogin.onClick, OnClickLogin);

        EventDelegate.Set(BtnFacebookBind.onClick, OnClickFaceBookBind);
        EventDelegate.Set(BtnFacebookLogin.onClick, OnClickFaceBookLogin);

        for (int i = 0; i < BtnBGM.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(BtnBGM[idx].onClick, delegate ()
             {
                 OnClickBGM(idx);
             });
        }

        for (int i = 0; i < BtnSoundfx.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(BtnSoundfx[idx].onClick, delegate ()
            {
                OnClickSound(idx);
            });
        }
        EventDelegate.Set(BtnLogin.onClick, OnClickLogin);

        for (int i = 0; i < HpCheck.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(HpCheck[idx].onClick, delegate ()
            {
                OnClickCheckHp(idx);
            });
        }
        for (int i = 0; i < InviteCheck.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(InviteCheck[idx].onClick, delegate ()
            {
                OnClickCheckInvite(idx);
            });
        }
        for (int i = 0; i < NameCheck.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(NameCheck[idx].onClick, delegate ()
            {
                OnClickCheckName(idx);
            });
        }
        for (int i = 0; i < AlramCheck.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(AlramCheck[idx].onClick, delegate ()
            {
                OnClickCheckAlram(idx);
            });
        }
        for (int i = 0; i < WhisperCheck.Length; i++)
        {
            int idx = i;
            EventDelegate.Set(WhisperCheck[idx].onClick, delegate ()
            {
                OnClickCheckWhisper(idx);
            });
        }
       // EventDelegate.Set(BtnChange.onClick, OnClickChange);

        EventDelegate.Set(IngameListBtn[0].onClick, delegate() {
            //계속하기버튼

            Close();
        });
        EventDelegate.Set(IngameListBtn[1].onClick, delegate() {
            //스테이지 선택

            NetData.instance.ClearRewardData();
            if (G_GameInfo.GameMode != GAME_MODE.SPECIAL_EXP 
                && G_GameInfo.GameMode != GAME_MODE.SPECIAL_GOLD
                && G_GameInfo.GameMode != GAME_MODE.ARENA)//이 경우 강종을 해도 보상받아야함.
            {
                G_GameInfo._GameInfo.IsEndForced = true;//강제종료
                G_GameInfo._GameInfo.IsEndForcedMap = true;//맵으로 이동
            }

            G_GameInfo._GameInfo.ForcedGameEnd();
            Close();

        });
        EventDelegate.Set(IngameListBtn[2].onClick, delegate ()
        {
            //마을로나가기
            if (G_GameInfo.GameMode != GAME_MODE.SPECIAL_EXP
               && G_GameInfo.GameMode != GAME_MODE.SPECIAL_GOLD
               && G_GameInfo.GameMode != GAME_MODE.ARENA)//이 경우 강종을 해도 보상받아야함.
            {
                G_GameInfo._GameInfo.IsEndForced = true;//강제종료
            }

            G_GameInfo._GameInfo.ForcedGameEnd();
            Close();
        });
        EventDelegate.Set(IngameListBtn[3].onClick, delegate() {
            IngameOption.SetActive(false);
            TownOption.SetActive(true);
        });




        //EventDelegate.Set(BtnShowHead.onChange, OnClickShowNpcHead);
        //EventDelegate.Set(BtnShowHead.onChange, OnClickShowNpcHead);



        //EventDelegate.Set(BtnCustomerService.onClick, delegate () { CustomerServicePopup.SetActive(true); });
        //EventDelegate.Set(BtnClosePopup.onClick, delegate () { CustomerServicePopup.SetActive(false); });

        //EventDelegate.Set(BtnCoupon.onClick, OnClickCoupon);

        //EventDelegate.Set(BtnCommunity.onClick, delegate ()
        //{
        //    uniWebViewPopup.SetActive(true);
        //    uni.SetURL("http://cafe.naver.com/mjbox");
        //    uni.Popup();

        //});

    }


    /// <summary> UISlider의 Thumb가 움직이면 실행 </summary>
    void OnSoundSliderChange()
    {
        float value = SoundSlider.value;

        option.SoundVolume = value;
        NGUITools.soundVolume = value;
        SoundManager.instance.SetBgmVolume(value);
        //SoundManager.instance.audio.volume = value;
    }

    /// <summary> 화질 아이콘 변경</summary>
    void SetQualSprite(byte qual)   // 0
    {
        for (int i = 0; i < BtnQuality.Length; i++)
        {
            // 누른거만 켜줌
            if (qual == i)
            {
                QualSprite[i].gameObject.SetActive(true);
                continue;
            }

            QualSprite[i].gameObject.SetActive(false);


        }
    }

    /// <summary> 화질 변경</summary>
    void OnClickQuality(byte qual)
    {
        uint stringIdx = 0;
        switch (qual)
        {
            case (byte)QUALITY.QUALITY_HIGH: //하이
                QualityPressed = qual;
                QualityManager.instance.SetQuality(QUALITY.QUALITY_HIGH);
                stringIdx = 1009;
                break;
            case (byte)QUALITY.QUALITY_MID: //미드
                QualityPressed = qual;
                QualityManager.instance.SetQuality(QUALITY.QUALITY_MID);
                stringIdx = 1008;
                break;
            case (byte)QUALITY.QUALITY_LOW: //로우
                QualityPressed = qual;
                QualityManager.instance.SetQuality(QUALITY.QUALITY_LOW);
                stringIdx = 1007;
                break;
        }

        TownOption.transform.FindChild("Configuresetup/QualityOption/label").GetComponent<UILabel>().text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(983), _LowDataMgr.instance.GetStringCommon(stringIdx));
        option.Quality = QualityPressed;

        //색바꾸기
        for (int i = 0; i < BtnQuality.Length; i++)
        {
            int idx = i;
            GameObject on = BtnQuality[idx].transform.FindChild("on").gameObject;
            GameObject off = BtnQuality[idx].transform.FindChild("off").gameObject;

            if (i == qual)
            {
                on.SetActive(true);
                off.SetActive(false);
            }
            else
            {
                on.SetActive(false);
                off.SetActive(true);
            }
        }
    }

    /// <summary> 체력표기</summary>
    void OnClickCheckHp(int idx)
    {
        // 0->전체
        // 1->나
        // 2->파티원
        // 3->적

        char[] showArr = option.ShowHpBar.ToCharArray();
        //전체 눌럿을경우
        if (idx == 0)
        {
            char ch = showArr[0] == '1' ? '2' : '1';

            for (int i = 0; i < showArr.Length; i++)
            {
                showArr[i] = ch;
            }
        }
        else
        {
            showArr[idx] = showArr[idx] == '1' ? '2' : '1';
        }
        option.ShowHpBar = new string(showArr);

        SetCheckOption(HpCheck, option.ShowHpBar);

        //
        if(TownState.TownActive)
        {

        } 
        else
        {        
            G_GameInfo.GameInfo.BoardPanel.ChangeHeadState((byte)idx);
        }
    }
    /// <summary> 초대거부 </summary>
    void OnClickCheckInvite(int idx)
    {
        // 0->전체
        // 1->친구
        // 2->파티
        // 3->길드

        char[] showArr = option.BlockInvite.ToCharArray();
        //전체 눌럿을경우
        if (idx == 0)
        {
            char ch = showArr[0] == '1' ? '2' : '1';

            for (int i = 0; i < showArr.Length; i++)
            {
                showArr[i] = ch;
            }
        }
        else
        {
            showArr[idx] = showArr[idx] == '1' ? '2' : '1';
        }
        option.BlockInvite = new string(showArr);

        SetCheckOption(InviteCheck, option.BlockInvite);
    }
    ///// <summary> 이름표시 </summary>
    void OnClickCheckName(int idx)
    {
        //0->전체
        // 1->나
        // 2->모든유저

        char[] showArr = option.ShowName.ToCharArray();
        //전체 눌럿을경우
        if (idx == 0)
        {
            char ch = showArr[0] == '1' ? '2' : '1';

            for (int i = 0; i < showArr.Length; i++)
            {
                showArr[i] = ch;
            }
        }
        else
        {
            showArr[idx] = showArr[idx] == '1' ? '2' : '1';
        }
        option.ShowName = new string(showArr);

        SetCheckOption(NameCheck, option.ShowName);
        if (TownState.TownActive)
        {
            UIBasePanel town = UIMgr.GetTownBasePanel();
            if (town != null)
            {
                (town as TownPanel).ChangeHeadStateInTown((byte)idx);
            }
        }
        else
        {
            G_GameInfo.GameInfo.BoardPanel.ChangeHeadState((byte)idx);
        }
    }
    /// <summary> 알림끄기</summary>
    void OnClickCheckAlram(int idx)
    {
        //0->전체
        // 1->친구접속
        // 2->길드원접속
        // 3->시스템알림

        char[] showArr = option.OffAlram.ToCharArray();
        //전체 눌럿을경우
        if (idx == 0)
        {
            char ch = showArr[0] == '1' ? '2' : '1';

            for (int i = 0; i < showArr.Length; i++)
            {
                showArr[i] = ch;
            }
        }
        else
        {
            showArr[idx] = showArr[idx] == '1' ? '2' : '1';
        }
        option.OffAlram = new string(showArr);

        SetCheckOption(AlramCheck, option.OffAlram);
    }
    /// <summary> 귓속말거부 </summary>
    void OnClickCheckWhisper(int idx)
    {
        // 0->전체
        // 1->친구포함
        // 2->길드포함

        char[] showArr = option.BlockWhisper.ToCharArray();
        //전체 눌럿을경우
        if (idx == 0)
        {
            char ch = showArr[0] == '1' ? '2' : '1';

            for (int i = 0; i < showArr.Length; i++)
            {
                showArr[i] = ch;
            }
        }
        else
        {
            showArr[idx] = showArr[idx] == '1' ? '2' : '1';
        }
        option.BlockWhisper = new string(showArr);

        SetCheckOption(WhisperCheck, option.BlockWhisper);
    }


    /// <summary> BGM On/Off </summary>
    void OnClickBGM(int flag)
    {
        // flag = 0 => On
        // flag = 1 -> Off

        bool play = flag == 1 ? true : false;
        option.Bgm = play;

        //BGM 끄거나 켜기
        if (!play)
        {
            SoundManager.instance.BGMStop();
        }
        else
        {
            string bgmName = _LowDataMgr.instance.GetBGMFile(Application.loadedLevelName);
            if (!string.IsNullOrEmpty(bgmName))
                SoundManager.instance.PlayBgmSound(bgmName);
        }


        BtnBGM[0].gameObject.SetActive(play);
        BtnBGM[1].gameObject.SetActive(!play);

    }

    /// <summary> 효과음 On/Off </summary>
    void OnClickSound(int flag)
    {
        // flag = 0 => On
        // flag = 1 -> Off

        bool play = flag == 1 ? true : false;
        option.Soundfx = play;

        //BGM 끄거나 켜기
        if (!play)
        {
            NGUITools.soundVolume = 0;
        }
        else
        {
            NGUITools.soundVolume = 1;
        }


        BtnSoundfx[0].gameObject.SetActive(play);
        BtnSoundfx[1].gameObject.SetActive(!play);
    }

    /// <summary> 캐릭터 선택화면 이동 </summary>
    void OnClickChange()
    {
        /*
        HttpSender.instance.SendCharInfoList(() => {
        TownState.SavePosition = Vector3.zero;//마지막 포지션 초기화.
        NetData.instance.InitUserData();
        UIMgr.ClearAll();
        SceneManager.instance.ActionEvent(_ACTION.GO_SELECT);
        SoundHelper.TempPlayBgmSound(eBGMSoundType.Main);
        SoundManager.instance.PlayBgmSound("BGM_Main");
        iFunClient.instance.TryDisconnect();
        });

        option.SaveOptionData();
        */
    }

    /// <summary> 로그인화면 </summary>
    void OnClickLogin()
    {
        //UIMgr.ClearAll();

        //iFunClient.instance.TryDisconnect();
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.NONE, () =>
        {
            UITextList.ClearTextList();
            //UIMgr.GetTownBasePanel().Close();
            //UIMgr.instance.Clear(UIMgr.UIType.System);
            UIMgr.ClearUI(true);

            NetworkClient.instance.DisconnectGameServer();//연결 종료
            NetData.instance.InitUserData();
            NetData.instance.ClearCharIdc();

            SceneManager.instance.ActionEvent(_ACTION.GO_LOGIN);
        });

        //option.SaveOptionData();
        //Close();
        option.SaveOptionData();
        base.Close();
    }

    // 쿠폰팝업창
    //void OnClickCoupon()
    //{
    //    CouponPopup.SetActive(true);

    //    UIButton CouponAccept = transform.FindChild("CouponPopup/BtnCouponAccept").GetComponent<UIButton>();
    //    UIButton CouponClose = transform.FindChild("CouponPopup/BtnCouponClose").GetComponent<UIButton>();


    //    EventDelegate.Set(CouponAccept.onClick, OnClickCouponAccept);
    //    EventDelegate.Set(CouponClose.onClick, delegate () { CouponPopup.SetActive(false); });
    //}

    /// <summary> 적 유닛 Hp,이름 표기 유무 </summary>
    //void OnClickShowNpcHead()
    //{
    //    option.ShowNpcHead = BtnShowHead.value;
    //}

    //void OnClickInviteFriend()
    //{
    //    option.BlockFriendInvite = BtnInviteFriend.value;
    //}
    //void OnClickParty()
    //{
    //    option.BlockParty = BtnParty.value;
    //}

    //void OnClickCouponAccept()
    //{
    //    string number = InputCoupon.value;
    //    if (string.IsNullOrEmpty(number))
    //        return;

    //    //   Debug.Log(number);
    //}


    public override void Close()
    {
        base.Close();

        Time.timeScale = 1;

        //저장하고 끄기
        option.SaveOptionData();

        //if (openType==1)
        //{
        //    ChatPopup chat = SceneManager.instance.ChatPopup(false);
        //    chat.Show();
        //    return;
        //}

        if (TownState.TownActive)
        {
            UIBasePanel townPanel = UIMgr.GetTownBasePanel();
            if (townPanel != null)
                townPanel.Show();
        }
        else
        {
            UIBasePanel hudPanel = UIMgr.GetHUDBasePanel();
            if (hudPanel != null)
                (hudPanel as InGameHUDPanel).SetJoyActive(true);

            //ChatPopup chat = SceneManager.instance.ChatPopup(false);
            UIBasePanel chat = UIMgr.GetUIBasePanel("UIPopup/ChatPopup");
            if (chat != null)
                (chat as ChatPopup).OnShow();
        }
               
    }



}

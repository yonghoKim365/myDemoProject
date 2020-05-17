using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class LoginPanel : UIBasePanel {
    /// <summary>
    /// 서버 리스트 게임오브젝트
    /// </summary>
    public GameObject ServerListObj;

    enum GroupT
    {
	   Login, Server
    }

    public UIInput IdInput;
    
    public UIButton BtnLogin;
    public UIButton BtnShowServerList;
    public UIButton BtnMovie;

    public UIButton BtnLoginAlter;

    public UILabel MainServerName;//로그인 끝나고 선택되어 있는 서버 표기해주는 것
    public UILabel ListSelectServerName;//서버 리스트에서 보여주는 선택되어 있는 것
    public UILabel StateLbl;
    public UILabel CheckLoadText;
    public UILabel LoadingStr;
    public UISlider Slider;

    public UIToggle ClauseToggle;

    public Transform ServerListTf;
    public Transform Indicator;
    
    //public GameObject LogoGo;
    public GameObject ClauseCheckPop;
    public GameObject ClauseGo;
    public GameObject LogoEff;
    public GameObject[] Objs;//0 로그인, 1 서버

    public GameObject BarEff;

    public System.Action EndCallBack;
    
    private const string DefaultID = "";
    private const string DefaultPW = "";
    
    private ServerData SelectServerInfo;
    private ServerData TempServerInfo;

    private string uuid;

    private long _userID;
    private int _signedKey;
    private bool bEnableInstantLogin;
    private bool bStopInstantLogin;


    private string loginServerIP = "http://52.78.243.250:7108/server/serverlist.php"; // qa
	//private string loginServerIP = "http://hfh.bs.quwangame.net:7109/server/serverlist.php"; // panho

    private struct ServerGroup
    {
        public int Id;
        public string Name;//그룹이름

        public List<ServerData> ServerList;
    }

    private struct ServerData
    {
        public byte Maintain;//점검 여부?
        public byte Busy;//혼잡도

        public int Id;
        public int Port_1;
        public int Port_2;
        
        public string Name;//서버 이름
        public string Ip_1;
        public string Ip_2;
    }

    public override void Init()
    {
        base.Init();

        if (PlayerPrefs.HasKey("Account"))
        {
            string account = PlayerPrefs.GetString("Account", "Input ID...");
            string info = account;
            IdInput.value = info;
        }
        else
        {
            IdInput.value = DefaultID;
        }
        
        ServerListObj.SetActive(false);
        IdInput.transform.parent.gameObject.SetActive(false);
        BtnShowServerList.gameObject.SetActive(false);
        Objs[1].SetActive(true);
        Objs[0].SetActive(false);
        LogoEff.SetActive(false);
        transform.FindChild("Login/logoEff").gameObject.transform.localPosition = transform.FindChild("Login/logo").gameObject.transform.localPosition;
        UIHelper.CreateEffectInGame(transform.FindChild("Login/logoEff"), "Fx_UI_BI_glow" + SystemDefine.LocalEff);

        ClauseGo.SetActive(false);

        transform.FindChild("Login/Clause").gameObject.SetActive(false);
        ClauseCheckPop.SetActive(false);

        EventBtnHandler();

		EventDelegate.Set(IdInput.onChange, OnIdInputChange );
        //EventDelegate.Set(IdInput.onSubmit, OnIdInputSubmit );

        CheckLoadText.text = string.Format(SystemDefine.LocalLoadingPer, 0);
        Slider.value = 0;
        BarEff.transform.SetLocalScaleX(0); //일단영
        Slider.ForceUpdate();

#if UNITY_EDITOR
        BtnLoginAlter.gameObject.SetActive(true);
#else
		BtnLoginAlter.gameObject.SetActive(false);

		if (Debug.isDebugBuild){
        	BtnLoginAlter.gameObject.SetActive(true);
		}
#endif
    }


    public override void LateInit()
    {
        base.LateInit();
        
        bool isLoading = (bool)parameters[0];
        if (isLoading)
        {
            transform.FindChild("Loading/version").GetComponent<UILabel>().text = 
                string.Format(SystemDefine.LocalVersion, BatteryLevel.GetCFBundleVersion() );
            StartCoroutine("Loading");
        }
        else
        {
            OnShow();
            //if( UIMgr.GetUIBasePanel("UIPanel/LogoPanel") == null)
            //{
                TempCoroutine.instance.FrameDelay(0.2f, () => {
                    SoundManager.instance.PlaySfxSound(eUISfx.UI_opening_shine, false);
                });
            //}

        }
    }

    public override bool Quit()
    {
        if (ClauseCheckPop.activeSelf)
        {
            ClauseCheckPop.SetActive(false);
            return false;
        }
        else if (ClauseGo.activeSelf)
        {
            ClauseGo.SetActive(false);
            return false;
        }

        return base.Quit();
    }

    /// <summary> 다시켜질때 </summary>
    void OnShow()
    {
        transform.FindChild("Loading").gameObject.SetActive(false);
        transform.FindChild("Login").gameObject.SetActive(true);
        transform.FindChild("Login/Clause").gameObject.SetActive(true);
        CameraManager.instance.mainCamera.backgroundColor = new Color32(0, 0, 0, 0);
        
        //uuid = "21d4f946ce16a4690376a5b2a5fe0042e0c6ba13";// PlayerPrefs.GetString("PLAYERUUID", "none");
        uuid = PlayerPrefs.GetString("PLAYERUUID_v2", "none");

        if (uuid.Equals("none"))
        {
            //uuid가 없다 - 신규계정
            //existuuid = System.BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);
            uuid = System.Guid.NewGuid().ToString();
            //uuid = SystemInfo.deviceUniqueIdentifier;

            PlayerPrefs.SetString("PLAYERUUID_v2", uuid.ToString());
        }

        Debug.Log("uuid:" + uuid);

        if(LoginState.FirstLogin)
        { 
            //LogoEff.SetActive(true);
            //TweenScale tween = LogoEff.GetComponent<TweenScale>();
            //tween.ResetToBeginning();
            //tween.PlayForward();
            //TempCoroutine.instance.FrameDelay((tween.delay + tween.duration) - 0.1f, () => {
            //    SoundManager.instance.PlaySfxSound(eUISfx.UI_opening_signboard, false);
            //});

#if UNITY_EDITOR
            // auto start for test. only editor
//            if (Objs[1].activeSelf && Objs[0].activeSelf == false)
//            {
//                TempCoroutine.instance.FrameDelay(2.0f, () => {
//                    Objs[1].SetActive(false);
//                    Objs[0].SetActive(true);
//                    bEnableInstantLogin = true;
//                });
//            }
#endif
        }

        TempCoroutine.instance.FrameDelay(0.2f, () => {
            SoundManager.instance.PlaySfxSound(eUISfx.UI_opening_shine, false);
        });

        SoundManager.instance.PlayBgmSound("BGM_Main");
        HttpSender.instance.Get(loginServerIP, (result) =>
        {
            //성공시
            JSONObject s2c = new JSONObject(result);

            if (s2c == null || s2c.Count <= 0)
            {
                return;
            }

            if (s2c["msg"].str.Equals("success"))//인증서버
            {
                NetworkClient.instance.DisconnectAuthServer();

                List<ServerGroup> groupList = new List<ServerGroup>();
                JSONObject groupJson = s2c["group"];
                if (groupJson != null)//그룹 받기
                {
                    for (int i = 0; i < groupJson.Count; i++)
                    {
                        ServerGroup group;
                        group.Id = int.Parse(groupJson[i]["group_id"].str);
                        group.Name = groupJson[i]["group_name"].str;
                        group.ServerList = new List<ServerData>();

                        groupList.Add(group);
                    }
                }

                JSONObject serverJson = s2c["server"];
                if (serverJson != null)//서버들 받기
                {
                    for (int i = 0; i < serverJson.Count; i++)
                    {
                        ServerData data;
                        data.Id = int.Parse(serverJson[i]["server_id"].str);
                        data.Name = serverJson[i]["server_name"].str;
                        data.Ip_1 = serverJson[i]["server_ip_1"].str;
                        data.Port_1 = int.Parse(serverJson[i]["server_port_1"].str);
                        data.Ip_2 = serverJson[i]["server_ip_2"].str;
                        data.Port_2 = int.Parse(serverJson[i]["server_port_2"].str);
                        data.Maintain = byte.Parse(serverJson[i]["maintain"].str);
                        data.Busy = byte.Parse(serverJson[i]["busy"].str);
                        int groupId = int.Parse(serverJson[i]["server_group_id"].str);

                        for (int j = 0; j < groupList.Count; j++)
                        {
                            if (groupList[j].Id != groupId)
                                continue;

                            groupList[j].ServerList.Add(data);
                            break;
                        }

                    }
                }

                Debug.Log("Server Group " + groupJson);
                Debug.Log("Server List " + serverJson);

                Transform groupTf = transform.FindChild("Login/ServerGroup/ServerList/Etc");
                for (int i = 0; i < groupList.Count; i++)
                {
                    Transform group = groupTf.FindChild(string.Format("tab_{0}", i));
                    if(group == null)
                    {
                        group = Instantiate(groupTf.FindChild("tab_0")) as Transform;
                        group.parent = groupTf;
                        group.localPosition = Vector3.zero;
                        group.localScale = Vector3.one;

                        group.name = string.Format("tab_{0}", i);
                    }

                    group.GetComponent<UILabel>().text = groupList[i].Name;

                    List<ServerData> dataList = groupList[i].ServerList;
                    if (i == 0)
                        OnTabServerList(dataList);

                    EventDelegate.Set(group.GetComponent<UIEventTrigger>().onClick, delegate() {
                        OnTabServerList(dataList);
                    });
                }

                groupTf.GetComponent<UIGrid>().repositionNow = true;
                //if (0 < serverList.Count)//이름 표기
                {
                    ServerData data;
					int id = PlayerPrefs.GetInt("SelectServerId", 0);
                    if (id == 0)
                    {
						data = groupList[0].ServerList[0];
						PlayerPrefs.SetInt("SelectServerId", data.Id);
					}
					else{
                        data = new ServerData();
                        for (int i=0; i < groupList.Count; i++)
                        {
                            List<ServerData> list = groupList[i].ServerList;
                            for (int j=0; j < list.Count; j++)
                            {
                                if (list[j].Id != id)
                                    continue;

                                data = list[j];
                                break;
                            }
                        }
                    }

                    MainServerName.text = data.Name;
                    ListSelectServerName.text = data.Name;
                    
                    SelectServerInfo = data;
                    BtnShowServerList.gameObject.SetActive(true);
                }

                StateLbl.text = SystemDefine.LocalLoginState_0;//"获取服务器列表";

                //IsSendLogin = false;
            }

        },
        (error) =>
        {
            //실패시 - 팝업띄우고 다시시도
            if (error.Contains("connect to host"))//서버연결 안되어 있음
            {

            }
            else
                Debug.LogError(error);
        });

        StateLbl.text = _LowDataMgr.instance.GetStringCommon(291);
    }

    void OnTabServerList(List<ServerData> dataList)
    {
        int serverCount = ServerListTf.childCount < dataList.Count ? dataList.Count : ServerListTf.childCount;
        for (int j = 0; j < serverCount; j++)
        {
            Transform tf = null;
            if (j < ServerListTf.childCount)
                tf = ServerListTf.GetChild(j);
            else
            {
                tf = Instantiate(ServerListTf.GetChild(0)) as Transform;
                tf.parent = ServerListTf;
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;
            }

            if (dataList.Count <= j)
            {
                tf.gameObject.SetActive(false);
                continue;
            }

            ServerData data = dataList[j];
            tf.gameObject.SetActive(true);
            tf.FindChild("name").GetComponent<UILabel>().text = data.Name;

            tf.FindChild("1_on").gameObject.SetActive(false);
            tf.FindChild("2_on").gameObject.SetActive(false);
            tf.FindChild("3_on").gameObject.SetActive(false);

            if (data.Maintain == 0)//정상 서버
            {
                tf.FindChild("normal").gameObject.SetActive(data.Busy != 1);
                tf.FindChild("hell").gameObject.SetActive(data.Busy == 1);
                tf.FindChild("inspection").gameObject.SetActive(false);

                EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate () { OnClickServer(data); });
            }
            else//점검중
            {
                tf.FindChild("normal").gameObject.SetActive(false);
                tf.FindChild("hell").gameObject.SetActive(false);
                tf.FindChild("inspection").gameObject.SetActive(true);

                EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate () {
                    //TempServerInfo = null;
                    TempServerInfo.Id = 0;
                });
            }
        }

        ServerListTf.GetComponent<UIGrid>().repositionNow = true;
    }


    void ConnectLoginServer(string szMachineKey)
    {
        if( !ClauseToggle.value)
        {
            ClauseCheckPop.SetActive(true);
            return;
        }
        
        SceneManager.instance.ShowNetProcess("Login");
        NetworkClient.instance.ConnectServer(SelectServerInfo.Ip_1, SelectServerInfo.Port_1, (state) =>
        {
            switch (state)
            {
                case Core.Net.ConnectState.Success:
                    NetworkClient.instance.SendPMsgAccountCertifyC((uint)Sw.PLATFORM_TYPE.PLATFORM_TYPE_SELF, szMachineKey, 00, (uint)Sw.CHANNEL_TYPE.CHANNEL_TYPE_SELF, "", "", "", "");
                    NetData.instance._Ip = SelectServerInfo.Ip_1;
                    break;
                case Core.Net.ConnectState.Error:
                    //2번 IP로 재시도
                    NetworkClient.instance.ConnectServer(SelectServerInfo.Ip_2, SelectServerInfo.Port_2, (state2) =>
                    {
                        switch (state2)
                        {
                            case Core.Net.ConnectState.Success:
                                NetworkClient.instance.SendPMsgAccountCertifyC((uint)Sw.PLATFORM_TYPE.PLATFORM_TYPE_SELF, szMachineKey, 00, (uint)Sw.CHANNEL_TYPE.CHANNEL_TYPE_SELF, "", "", "", "");
                                NetData.instance._Ip = SelectServerInfo.Ip_2;

                                break;
                            case Core.Net.ConnectState.Error:
                                uiMgr.AddPopup(141, 293, 117, 75, 0,
                                () => {
                                    UIMgr.ClearUI();
                                    TempCoroutine.instance.NextFrame(() => {
                                        UIMgr.Open("UIPanel/LoginPanel", false);
                                    });
                                    //SceneManager.instance.ActionEvent(_ACTION.GO_LOGO);//재접속
                                }, () => {//종료
                                          //#if !UNITY_EDITOR
                                    Application.Quit();
                                }, null);

                                SceneManager.instance.EndNetProcess("Login");
                                break;

                            case Core.Net.ConnectState.Close:
                                //에러코드
                                SceneManager.instance.EndNetProcess("Login");
                                break;
                        }
                    });

                    break;

                case Core.Net.ConnectState.Close:
                    //에러코드
                    break;
            }
            
        });
    }

    void LoginProcess()
    {
        string loginType = PlayerPrefs.GetString("logintype", "none");
        string user_id = PlayerPrefs.GetString("user_id", "none");
        string user_token = PlayerPrefs.GetString("user_token", "none");

        if (!loginType.Contains("none") && !user_id.Contains("none") && !user_token.Contains("none"))
        {
            //서버에 페이스북으로 로그인 시도
            if (loginType.Contains("facebook"))
            {
                SceneManager.instance.ShowNetProcess("Login");
                NetworkClient.instance.ConnectServer(SelectServerInfo.Ip_1, SelectServerInfo.Port_1, (state) =>
                {
                    switch (state)
                    {
                        case Core.Net.ConnectState.Success:
                            NetworkClient.instance.SendPMsgFacebookCertifyC(user_id, user_token);
                            NetData.instance._Ip = SelectServerInfo.Ip_1;
                            break;
                        case Core.Net.ConnectState.Error:
                            //2번 IP로 재시도
                            NetworkClient.instance.ConnectServer(SelectServerInfo.Ip_2, SelectServerInfo.Port_2, (state2) =>
                            {
                                switch (state2)
                                {
                                    case Core.Net.ConnectState.Success:
                                        NetworkClient.instance.SendPMsgFacebookCertifyC(user_id, user_token);
                                        NetData.instance._Ip = SelectServerInfo.Ip_2;

                                        break;
                                    case Core.Net.ConnectState.Error:
                                        uiMgr.AddPopup(141, 293, 117, 75, 0,
                                        () => {
                                            UIMgr.ClearUI();
                                            TempCoroutine.instance.NextFrame(() => {
                                                UIMgr.Open("UIPanel/LoginPanel", false);
                                            });
                                                //SceneManager.instance.ActionEvent(_ACTION.GO_LOGO);//재접속
                                            }, () => {//종료
                                                  //#if !UNITY_EDITOR
                                                Application.Quit();
                                        }, null);

                                        SceneManager.instance.EndNetProcess("Login");
                                        break;

                                    case Core.Net.ConnectState.Close:
                                        //에러코드
                                        SceneManager.instance.EndNetProcess("Login");
                                        break;
                                }
                            });

                            break;

                        case Core.Net.ConnectState.Close:
                            //에러코드
                            break;
                    }

                });
            }
        }
        else
        {
            ConnectLoginServer(uuid);
        }
    }

    /// <summary>
    /// 이벤트 함수 설정
    /// </summary>
    void EventBtnHandler()
    {
        EventDelegate.Set(BtnLogin.onClick, CheckLogin );//로그인 확인버튼

        EventDelegate.Set(BtnLoginAlter.onClick, delegate () {
            Objs[1].SetActive(false);
            Objs[0].SetActive(true);
        });

        EventDelegate.Set(BtnShowServerList.onClick, delegate() {
            ServerListObj.SetActive(true);
            TempServerInfo.Id = 0;
        });//현재 서버 교체 버튼

        EventDelegate.Set(ServerListObj.transform.FindChild("Btn_enter").GetComponent<UIButton>().onClick, 
            delegate() {

                if (TempServerInfo.Id == 0)
                    return;

				PlayerPrefs.SetInt ("SelectServerId", TempServerInfo.Id);// TempServerInfo.Id);//접속한 서버 아이디 저장하기.

                MainServerName.text = TempServerInfo.Name;
                ListSelectServerName.text = TempServerInfo.Name;
                
                SelectServerInfo = TempServerInfo;
                ServerListObj.SetActive(false);
        } );

        EventDelegate.Set(ServerListObj.transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, delegate () {
            ServerListObj.SetActive(false);
            TempServerInfo.Id = 0;
        });//서버교체 팝업 끄기

        //if (Debug.isDebugBuild)
        //{
        //    transform.FindChild("ServerGroup/auto").gameObject.SetActive(true);
        //    EventDelegate.Set(transform.FindChild("ServerGroup/auto").GetComponent<UIButton>().onClick, delegate () {
        //        ConnectLoginServer(uuid);
        //    });
        //}
        //else
        {
            transform.FindChild("Login/ServerGroup/auto").gameObject.SetActive(false);
        }

        EventDelegate.Set(BtnMovie.onClick, PlayMovie);

        EventDelegate.Set(transform.FindChild("Login/ServerGroup/BtnStart").GetComponent<UIButton>().onClick, LoginProcess);//서버연결


        EventDelegate.Set(transform.FindChild("Login/Clause/BtnClausePop").GetComponent<UIEventTrigger>().onClick, delegate() {
            ClauseGo.SetActive(true);
        } );

        EventDelegate.Set(ClauseGo.transform.FindChild("Btn").GetComponent<UIEventTrigger>().onClick, delegate () {
            ClauseGo.SetActive(false);
        });

        EventDelegate.Set(ClauseCheckPop.transform.FindChild("Btn").GetComponent<UIEventTrigger>().onClick, delegate () {
            ClauseCheckPop.SetActive(false);
        });
    }

#region 이벤트 버튼
    /// <summary>
    /// 서버 리스트에서 서버 변경누름.
    /// </summary>
    /// <param name="serverName"></param>
    void OnClickServer(ServerData serverInfo)
    {
        ListSelectServerName.text = serverInfo.Name;

        TempServerInfo = serverInfo;

    }
#endregion

    /// <summary> 원래라면 아이디 비밀번호로 로그인 프로토콜 태우고 로그인 아이디 형식이 맞는지 선체크 </summary>
    void CheckLogin()
    {
        string id = IdInput.value;
        
        if ( id.Contains(" ") || string.IsNullOrEmpty(id) || DefaultID.CompareTo(id) == 0)
        {
            //UIMgr.instance.AddPopup(9, id, null, null, null);
            uiMgr.AddPopup(141, 110, 117);
            return;
        }

        if (SelectServerInfo.Id == 0)
        {//서버 목록을 받아오지 못하였다.
            Debug.LogError("server list error");
            uiMgr.AddPopup(141, 293, 117, 75, 0, () => {
                UIMgr.ClearUI();
                TempCoroutine.instance.NextFrame(() => {
                    UIMgr.Open("UIPanel/LoginPanel", false);
                });

            }, () => {//종료
                Application.Quit();
            }, null);
            return;
        }

        ConnectLoginServer(id);
    }

    void PlayMovie()
    {
//#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_IOS && !UNITY_EDITOR)
//        Handheld.PlayFullScreenMovie("movie.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput | FullScreenMovieControlMode.Full, FullScreenMovieScalingMode.Fill);
//#else
        UIMgr.instance.AddPopup(_LowDataMgr.instance.GetStringCommon(141), _LowDataMgr.instance.GetStringCommon(280), _LowDataMgr.instance.GetStringCommon(117));
//#endif
    }

    /// <summary> </summary>
    public void OnSuccessLogin()
    {
        PlayerPrefs.SetString("Account", string.Format("{0}", IdInput.value) );//사용자 계정 저장하기
		PlayerPrefs.SetInt ("SelectServerId", SelectServerInfo.Id);// TempServerInfo.Id);//접속한 서버 아이디 저장하기.

        SceneManager.instance.optionData.ServerName = SelectServerInfo.Name;


        StateLbl.text = SystemDefine.LocalLoginState_1;//"成功连接登陆服务器";
        //TempCoroutine.instance.FrameDelay(0f, () =>
        //{
            Hide();
            LoginState state = SceneManager.instance.GetState<LoginState>();

            if (state != null)
            {
                state.SelectSceneProcess();
            }
        //});
    }

    public void SendGameLogin()
    {

        byte[] ipbyte = System.BitConverter.GetBytes(NetworkClient.instance.GetSelectedServer());
        IPAddress ipAddress = new IPAddress(System.BitConverter.ToUInt32(ipbyte, 0));

        NetworkClient.instance.ConnectGameServer(ipAddress.ToString(), NetworkClient.instance.GetSelectedPort() , (state) =>
        {
            Debug.Log("ConnectGameServer  " + state);
            switch (state)
            {
                case Core.Net.ConnectState.Success:
                    //연결성공
                    StateLbl.text = SystemDefine.LocalLoginState_2;// "正在连接游戏服务器";
                    TempCoroutine.instance.FrameDelay(0f, () =>
                    {
                        Debug.Log("SendPMsgGameLoginC:" + _userID);
                        NetworkClient.instance.SendPMsgGameLoginC(_userID, 1, 65835, NetData.instance.UnCode, 1, 0);
                    });
                    break;

                case Core.Net.ConnectState.Error:
                    //에러코드
                    break;

                case Core.Net.ConnectState.Close:
                    //에러코드
                    break;
            }
        });
    }

    public void OnAccountCertifyComplet(long userID, int signedKey, eLoginType loginType)
    {
        TempCoroutine.instance.FrameDelay(0f, () =>
        {
            _userID = userID;
            _signedKey = signedKey;
            NetData.instance._LoginType = loginType;
            NetworkClient.instance.SendPMsgLoginC(_userID, _signedKey, SelectServerInfo.Id);
        });
    }
    
#if UNITY_EDITOR
	void Update(){
		if (bEnableInstantLogin) {
			bEnableInstantLogin = false;
			TempCoroutine.instance.FrameDelay(2.0f, () => {
				string id = IdInput.value;

				if ( id.Contains(" ") || string.IsNullOrEmpty(id) || DefaultID.CompareTo(id) == 0)
				{
					return;
				}
				
				if (SelectServerInfo.Id == 0)return;

				if (bStopInstantLogin){
					bStopInstantLogin = false;
					return;
				}

				ConnectLoginServer(IdInput.value);
			});
		}
	}
#endif

	void OnIdInputChange(){
		bStopInstantLogin = true;
		bEnableInstantLogin = false;
	}
    
    public override void Close()
    {
        base.Close();
        LoginState.FirstLogin = false;
        SceneManager.instance.EndNetProcess("Login");
    }
    
    IEnumerator Loading()
    {
        transform.FindChild("Loading").gameObject.SetActive(true);
        transform.FindChild("Login").gameObject.SetActive(false);
        
        float startPos = -595;
        float barSize = Slider.foregroundWidget.localSize.x;

        //Slider.value = 0;
        //Slider.ForceUpdate();
        if (LoadingStr != null)
        {
            LoadingStr.text = SystemDefine.LocalLoadingDesc;
        }

        _LowDataMgr.instance.LoadLowDataAllData((ratio, desc) =>
        {
            CheckLoadText.text = string.Format(SystemDefine.LocalLoadingPer, (ratio * 100f).ToString("N0"));//100f
            Slider.value = ratio >= 0.985f ? 1f : ratio;

            if (Indicator != null)
            {
                Indicator.localPosition = new Vector3(ratio * barSize + startPos, Indicator.localPosition.y, Indicator.localPosition.z);

                // 바 이펙트 스케일처리
                //if (Slider.value < 0.08)
                //    BarEff.transform.localScale = new Vector3((Slider.value/0.01f) * 0.13f , BarEff.transform.localScale.y, BarEff.transform.localScale.z);
                if (Slider.value < 0.23)
                    BarEff.transform.localScale = new Vector3((Slider.value / 0.01f) * 0.045f, BarEff.transform.localScale.y, BarEff.transform.localScale.z);
                else
                    BarEff.transform.localScale = new Vector3(1f, BarEff.transform.localScale.y, BarEff.transform.localScale.z);

            }
			//Debug.Log("LoginPanel.loading, 22 time:"+ sw.ElapsedMilliseconds / 1000f);

            if (ratio == 1f)
            {
                //BenchMark.Mark("LOADING_END");
                //BenchMark.BenchTime("LOADING_START", "LOADING_END");

                if (CheckLoadText != null)
                {
                    CheckLoadText.transform.parent.gameObject.SetActive(false);
                }
                if (LoadingStr != null)
                {
                    LoadingStr.text = _LowDataMgr.instance.GetStringCommon(538);
                }

                EndCallBack();
                OnShow();
            }
        });

		//Debug.Log("LoginPanel.loading done time:"+ sw.ElapsedMilliseconds / 1000f);

        yield return null;
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ChatPopup : UIBasePanel
{
    class SlotData
    {
        public Transform Tf;
        public byte Type;//0나, 1다른유저, 2 텍스트
        
        public SlotData(Transform tf, byte type)
        {
            Tf = tf;
            Type = type;
        }

        public float PosY {
            set {
                if (Tf == null)
                    return;

                Tf.localPosition = new Vector3(0, value, 0);
            }
            get {
                if (Tf == null)
                    return 0;

                return Tf.localPosition.y;
            }
        }

        public Transform Parent {
            set {
                if (Tf == null)
                    return;

                Tf.parent = value;
            }

            get {
                return Tf;
            }
        }

    }

    //public GameObject GetMailItemPanel;//메일에서 수령하면 보여지는 것
    public GameObject WhisperBack;
    public GameObject BigChatPopup;
    public GameObject DefaultBg;

    public Transform MySlot;
    public Transform UserSlot;
    public Transform TxtSlot;
    public Transform WhisperTargetSlot;//귓말 대상 슬롯

    public Transform MiniChat;
    //public Transform[] GetMailSlotTf;//메일함에서 수령했을때 연출할 슬롯 5개.
    public Transform BottmInfoRoot;// 하단에 좌측에 표시될 기기상태
    public Transform DefaultGridTf;
    public Transform TempTf;//채팅 슬롯or텍스트들 사용안하는 것들 담는 애.

    public Color GreenColor;
    public Color RedColor;

    public UIScrollView ChatScroll;

    public UISprite Wifi;
    public UISprite Lte;
    public UILabel CurTime;
    public UILabel Ping;
    public UISprite Battery;

    public UITabGroup TabGroup;
    public UIInput InputMsg;

    public UITextList MiniTextList;

    public UIButton BtnSendMsg;
    public UIButton BtnClose;

    public UIEventTrigger BtnOption;
    public UIEventTrigger BtnOptionMini;
    

    /// <summary> ChatType이 귓말일 경우 나와 대화한 대상의 목록이 나옴. </summary>
    private Dictionary<ChatType, List<NetData.ChatData>> ChatDic = new Dictionary<ChatType, List<NetData.ChatData>>();
    private Dictionary<long, List<NetData.ChatData>> WhisperChatDic = new Dictionary<long, List<NetData.ChatData>>();//귓말한 대상과의 목록

    private List<SlotData> ChatSlotList = new List<SlotData>();
    
    private ChatType CurChatType;

    private bool IsMyChat = false;
    private const int MaxChatLine = 50;
    private const int MaxWhisperTarget = 20;
    private NetData.ChatData WhisperData;//귓말 대상정보
    //private ItemDetailPopup DetailPopup;

    private DateTime DeviceTime;

    public override void Init()
    {
        base.Init();

        EventDelegate.Set(BtnSendMsg.onClick, OnClickSendMessage);
        EventDelegate.Set(BtnClose.onClick, OnClickCloseBigChat);
        EventDelegate.Set(WhisperBack.GetComponent<UIEventTrigger>().onClick, () => {

            List<NetData.ChatData> chatList = null;
            if (!ChatDic.TryGetValue(CurChatType, out chatList))
            {
                chatList = new List<NetData.ChatData>();
                ChatDic.Add(CurChatType, chatList);
            }

            WhisperData.Init();
            DefaultBg.SetActive(true);
            WhisperBack.SetActive(false);
            InputMsg.gameObject.SetActive(false);
            BtnSendMsg.gameObject.SetActive(false);

            ChatScroll.verticalScrollBar.transform.localPosition = new Vector2(335, 28);
            ChatScroll.verticalScrollBar.backgroundWidget.height = 570;

            RefreshSlotData(chatList);

            if (ChatScroll.panel != null)
            {
                ChatScroll.panel.baseClipRegion = new Vector4(0, 0, 520, 615);
                ChatScroll.panel.clipOffset = Vector2.zero;
                ChatScroll.transform.localPosition = new Vector2(63, 29);
                ChatScroll.verticalScrollBar.value = 1;
            }

        });

        EventDelegate.Set(BigChatPopup.transform.FindChild("fog").GetComponent<UIEventTrigger>().onClick, OnClickCloseBigChat);

        OnClickCloseBigChat();
        BigChatPopup.SetActive(false);
        MiniChat.gameObject.SetActive(true);

        EventDelegate.Set(MiniTextList.gameObject.GetComponent<UIEventTrigger>().onClick, delegate () { OnClickBroadCast(null); });

        TabGroup.Initialize(OnClickTabGroup);
        TempCoroutine.instance.FrameDelay(0.3f, () =>
        {
            string loginMsg = string.Format(_LowDataMgr.instance.GetStringCommon(966), SceneManager.instance.optionData.ServerName);
            AddLogChat(loginMsg);
        });

        StartCoroutine("PingUpdate");
        TempTf.gameObject.SetActive(false);

         EventDelegate.Set(BtnOption.onClick, delegate () {

             bool isStop = false;
             if (TownState.TownActive)
                 UIMgr.GetTownBasePanel().Hide();
             else
             {
                 //UIMgr.GetHUDBasePanel().Hide();
                 Hide();
                 isStop = !SceneManager.instance.IsRTNetwork;
             }

            UIMgr.OpenOptionPanel(false, 1); });
        EventDelegate.Set(BtnOptionMini.onClick, delegate () {
            bool isStop = false;
            if (TownState.TownActive)
                UIMgr.GetTownBasePanel().Hide();
            else
            {
                //UIMgr.GetHUDBasePanel().Hide();
                Hide();
                isStop = !SceneManager.instance.IsRTNetwork;
            }

            UIMgr.OpenOptionPanel(isStop, 1);
        });

        MySlot.parent.gameObject.SetActive(false);
        MySlot.gameObject.SetActive(true);
        UserSlot.gameObject.SetActive(true);
        TxtSlot.gameObject.SetActive(true);
        WhisperTargetSlot.gameObject.SetActive(true);
    }

    /// <summary> 메세지 보내는 함수 </summary>
    void OnClickSendMessage()
    {
        if (CurChatType == ChatType.System || CurChatType == ChatType.Everything)
            return;

        string msg = InputMsg.value;
        msg.Replace("<color=", "");
        msg.Replace("\n", "");
        if (string.IsNullOrEmpty(msg))
            return;

        _LowDataMgr.instance.IsBanString(ref msg);

        if (CurChatType == ChatType.Whisper)
        {
            if (0 < WhisperData.WhisperUID && !string.IsNullOrEmpty(WhisperData.UserName))
                NetworkClient.instance.SendPMsgTalkCS((int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_PRIVATE, WhisperData.WhisperUID, WhisperData.UserName, msg, 1);
            else
            {
                OnReciveChat(_LowDataMgr.instance.GetStringCommon(908), ChatType.Whisper);//대상을 찾을 수 없음.
                return;
            }
        }
        else
        {
            int channel = 0;
            if (CurChatType == ChatType.Guild)
            {
                channel = (int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_GUILD;
                if (NetData.instance.GetUserInfo()._GuildId <= 0)
                {
                    //길드 없음
                    OnReciveChat(_LowDataMgr.instance.GetStringCommon(579), ChatType.System);
                    return;
                }
            }
            else if (CurChatType == ChatType.Map)
            {
                if (TownState.TownActive)//마을에서는 광역 채팅만 됨.
                {
                    OnReciveChat(_LowDataMgr.instance.GetStringCommon(909), ChatType.Map);//현재 지역에서는 지역채팅을 할 수 없습니다.
                    return;
                }
                else
                    channel = (int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_MAP;
            }
            else
                channel = (int)Sw.TALK_CHANNEL_TYPE.TALK_CHANNEL_WORLD;

            NetworkClient.instance.SendPMsgTalkCS(channel, 0, "", msg, 1);
        }

        IsMyChat = true;

    }

    /// <summary> 서버에서주는 채팅 응답 </summary>
    public void OnReciveChat(NetData.ChatData chatData, ChatType type)
    {
        string miniMsg = null;
        if (!chatData.Msg.Contains("[url=Item/"))
            miniMsg = string.Format("{0} : {1}", chatData.UserName, chatData.Msg);
        else
            miniMsg = chatData.Msg;

        if (type == ChatType.System)
        {
            char[] AlertArr = SceneManager.instance.optionData.OffAlram.ToCharArray(); //알림
            if (AlertArr[3] == '2')
                return;

            chatData.Msg = string.Format(_LowDataMgr.instance.GetStringCommon(913), chatData.Msg);
            miniMsg = string.Format(_LowDataMgr.instance.GetStringCommon(913), miniMsg);
        }
        else if (type == ChatType.Guild)
        {
            chatData.Msg = string.Format(_LowDataMgr.instance.GetStringCommon(910), chatData.Msg);
            miniMsg = string.Format(_LowDataMgr.instance.GetStringCommon(910), miniMsg);
        }
        else if (type == ChatType.Whisper)
        {
            char[] WhisperArr = SceneManager.instance.optionData.BlockWhisper.ToCharArray(); //귓말
            if ( !chatData.UserName.Equals(NetData.instance.Nickname) && WhisperArr[0] == '2')
                return;

            chatData.Msg = string.Format(_LowDataMgr.instance.GetStringCommon(911), chatData.Msg);
            miniMsg = string.Format(_LowDataMgr.instance.GetStringCommon(911), miniMsg);
        }
        else if (type == ChatType.World)
        {
            chatData.Msg = string.Format(_LowDataMgr.instance.GetStringCommon(912), chatData.Msg);
            miniMsg = string.Format(_LowDataMgr.instance.GetStringCommon(912), miniMsg);
        }
        else if (type == ChatType.Map)
        {
            chatData.Msg = string.Format(_LowDataMgr.instance.GetStringCommon(961), chatData.Msg);
            miniMsg = string.Format(_LowDataMgr.instance.GetStringCommon(961), miniMsg);
        }

        if (IsMyChat)//나의 채팅
        {
            InputMsg.value = "";
            IsMyChat = false;
        }

        MiniTextList.Add(miniMsg);
        AddChatData(chatData, type);
    }

    /// <summary> 서버에서주는 채팅 응답 </summary>
    public void OnReciveChat(string msg, ChatType type)
    {

        char[] WhisperArr = SceneManager.instance.optionData.BlockWhisper.ToCharArray(); //귓말
        char[] AlertArr = SceneManager.instance.optionData.OffAlram.ToCharArray(); //알림

        if (type == ChatType.System)
        {
            if (AlertArr[3] == '2')
                return;

            msg = string.Format(_LowDataMgr.instance.GetStringCommon(913), msg);

        }
        else if (type == ChatType.Guild)
            msg = string.Format(_LowDataMgr.instance.GetStringCommon(910), msg);
        else if (type == ChatType.Whisper)
        {
            if (WhisperArr[0] == '2')
                return;
            msg = string.Format(_LowDataMgr.instance.GetStringCommon(911), msg);

        }
        else if (type == ChatType.World)
            msg = string.Format(_LowDataMgr.instance.GetStringCommon(912), msg);
        else if (type == ChatType.Map)
            msg = string.Format(_LowDataMgr.instance.GetStringCommon(961), msg);

        MiniTextList.Add(msg);

        NetData.ChatData chatData = new NetData.ChatData(msg);
        AddChatData(chatData, ChatType.System);
    }

    /// <summary> 그룹 버튼 클릭시 실행할 이벤트 함수</summary>
    void OnClickTabGroup(int arr)
    {
        if (arr == (int)ChatType.Guild && NetData.instance.GetUserInfo()._GuildId <= 0)
        {
            //길드 없음
            //uiMgr.AddPopup(141, 579, 117);
            OnReciveChat(_LowDataMgr.instance.GetStringCommon(579), ChatType.System);
            TabGroup.CoercionTab(0);
            return;
        }
        
        CurChatType = (ChatType)arr;

        if (CurChatType == ChatType.System || CurChatType == ChatType.Everything || 
            (CurChatType == ChatType.Whisper && WhisperData.WhisperUID <= 0 ) )
        {
            InputMsg.gameObject.SetActive(false);
            BtnSendMsg.gameObject.SetActive(false);
        }
        else
        {
            InputMsg.gameObject.SetActive(true);
            BtnSendMsg.gameObject.SetActive(true);
        }

        List<NetData.ChatData> chatList = null;
        if (CurChatType == ChatType.Whisper)
        {
            if (0 < WhisperData.WhisperUID)
                WhisperChatDic.TryGetValue(WhisperData.WhisperUID, out chatList);
            else
                ChatDic.TryGetValue(CurChatType, out chatList);
        }
        else
        {
            WhisperData.Init();
            ChatDic.TryGetValue(CurChatType, out chatList);
        }

        bool isTarget = 0 < WhisperData.WhisperUID;
        WhisperBack.SetActive( isTarget);
        DefaultBg.SetActive( !isTarget);

        if (chatList == null)
            chatList = new List<NetData.ChatData>();

        RefreshSlotData(chatList);

        if (ChatScroll.panel != null)
        {
            ChatScroll.panel.baseClipRegion = new Vector4(0, 0, 520, isTarget ? 558 : 615);
            ChatScroll.panel.clipOffset = isTarget ? new Vector2(0, -20) : Vector2.zero;
            ChatScroll.transform.localPosition = new Vector2(63, 29);
            ChatScroll.verticalScrollBar.value = 1;
        }

        if (ChatScroll.verticalScrollBar != null)
        {
            ChatScroll.verticalScrollBar.transform.localPosition = new Vector2(335, isTarget ? 7 : 28);
            ChatScroll.verticalScrollBar.backgroundWidget.height = isTarget ? 555 : 570;
        }
    }

    /// <summary> 큰 채팅 팝업 나옴 </summary>
    void OnClickBigChatPopup()
    {
        BigChatPopup.SetActive(true);
        MiniChat.localPosition = new Vector2(0, -1500);

        TabGroup.CoercionTab((int)ChatType.Everything);//전체 채팅으로 강제 변경
    }

    /// <summary> 큰 팝업 닫음 </summary>
    void OnClickCloseBigChat()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        NativeHelper.instance.DisableNavUI();
#endif

        BigChatPopup.SetActive(false);
        MiniChat.localPosition = new Vector2(0, -302); // Vector2.zero;//new Vector2(-634, -290);
        BottmInfoRoot.gameObject.SetActive(true);

    }

    public override void Hide()
    {
        MiniChat.localPosition = new Vector2(0, -1500);
        BigChatPopup.SetActive(false);
        BottmInfoRoot.gameObject.SetActive(false);
    }

    /// <summary> 사용하는 곳에서 호출. </summary>
    public void OnShow()
    {
        OnClickCloseBigChat();
        //gameObject.SetActive(true);
    }

    /// <summary> 큰 채팅창 닫기 </summary>
    public bool IsHideBigPop
    {
        get
        {
            if (BigChatPopup.activeSelf)
            {
                OnClickCloseBigChat();
                return true;
            }

            return false;
        }
    }

    //네트워크상태, 배터리,시간등을표시
    public void SetBottomInfo()
    {
        DeviceTime = DateTime.Now;

        // 몇시 몇분
        CurTime.text = string.Format("{0:00}:{1:00}", DeviceTime.ToString("hh"), DeviceTime.ToString("mm"));

        //인터넷연결상태
        bool lte = true;
        bool wifi = true;
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                //3g/lte
                lte = true;
                wifi = false;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                //wifi
                lte = false;
                wifi = true;
                break;
            case NetworkReachability.NotReachable:
                //연결안댐
                lte = false;
                wifi = false;
                break;
        }
        Lte.gameObject.SetActive(lte);
        Wifi.gameObject.SetActive(wifi);


        Battery.fillAmount = BatteryLevel.GetBatteryLevel();


        //Battery.fillAmount = GetBatteryLevel();
        if (Battery.fillAmount <= 0.1) //10%이하일때 붉은색
            Battery.spriteName = "Icon_batterybar02";
        else
            Battery.spriteName = "Icon_batterybar01";
    }

	IEnumerator PingUpdate(){

		if (string.IsNullOrEmpty (NetData.instance._Ip)) {
			// test environment
			yield break;
		}

		while (true) {
			Ping p = new Ping(NetData.instance._Ip); //ip부분은 다시
			
			yield return new WaitForSeconds(0.1f);
			
			while (!p.isDone){
				yield return null;
			}
            
            int ping = p.time;
            if (ping < 0)
                ping = 0;

            Ping.text = string.Format(_LowDataMgr.instance.GetStringCommon(1178), ping);//string.Format("Ping {0}", p.time);
            if (ping >= 200)
                Ping.color = RedColor;
            else if (ping >= 100)
                Ping.color = Color.yellow;
            else
                Ping.color = GreenColor;

            p.DestroyPing();
            p = null;

            if (DeviceTime.Minute != DateTime.Now.Minute)//분단위로 찍으니깐 다르면 다시 갱신하자.
                SetBottomInfo();

            yield return new WaitForSeconds(1f);
        }
	}
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown("enter"))
        {
            OnClickSendMessage();
        }
       
    }

    void OnClickBroadCast(UILabel label)
    {
        string url = null;
        bool isMiniChat = false;
        if (label != null)// && BigChatPopup.localPosition == Vector3.zero //큰 팝업 켜져있는지
        {
            url = label.GetUrlAtPosition(UICamera.lastHit.point);
            //url = BigTextList[(uint)CurChatType].textLabel.GetUrlAtPosition(UICamera.lastHit.point);
        }
        else//작은 팝업이 켜져있음
        {
            url = MiniTextList.textLabel.GetUrlAtPosition(UICamera.lastHit.point);
            isMiniChat = true;
        }

        if (!TownState.TownActive || string.IsNullOrEmpty(url))
        {
            if (isMiniChat)//유알엘이 아니였음 큰 팝업으로 간주함
                OnClickBigChatPopup();

            return;
        }

        //Debug.Log("chat click url " + url);
        if (url.Contains("colosseum"))
        {
            string[] split = url.Split(',');
            long roomId = 0;
            if (split.Length < 1 || !long.TryParse(split[1], out roomId))
            {
                Debug.LogError("Colosseum URL Parse error! " + url);
                return;
            }

            NetworkClient.instance.SendPMsgColosseumEnterRoomC(roomId);
        }
        else if (url.Contains("multyRaid"))
        {
            string[] split = url.Split(',');
            long roomId = 0;
            if (split.Length < 1 || !long.TryParse(split[1], out roomId))
            {
                Debug.LogError("Colosseum URL Parse error! " + url);
                return;
            }

            NetworkClient.instance.SendPMsgMultiBossEnterRoomC(roomId);
        }
        else if(url.Contains("item") )
        {
            string[] split = url.Split('/');
            if (split.Length < 1)
            {
                Debug.LogError("Item URL pase error! " + url);
                return;
            }

            Debug.Log(url);
            
            string[] equipInfo = split[1].Split(',');
            int unId            = int.Parse(equipInfo[0]);
            //int unBasicValue    = int.Parse(equipInfo[1]);
            int unAttack        = int.Parse(equipInfo[1]);
            int unEnchantTime   = int.Parse(equipInfo[2]);

            string[] optionInfo = split[2].Split('|');


            List<NetData.ItemAbilityData> abilityList = new List<NetData.ItemAbilityData>();

            NetData._ItemData itemData = new NetData._ItemData((ulong)unId, (uint)unId, (ushort)unEnchantTime, 0, 0, (uint)unAttack);
            //Item.ItemValueInfo basicInfo = _LowDataMgr.instance.GetLowDataItemValueInfo(itemData.GetEquipLowData().BasicOptionIndex);
            //if (basicInfo != null)
            //{
            //    NetData.ItemAbilityData abilityData = new NetData.ItemAbilityData();
            //    abilityData.Ability = (AbilityType)basicInfo.OptionId;
            //    abilityData.Value = unBasicValue;

            //    abilityList.Add(abilityData);
            //}

            for(int i=0; i < optionInfo.Length; i++)
            {
                string[] ability = optionInfo[i].Split(',');
                uint id     = uint.Parse(ability[0]);
                float value = float.Parse(ability[1]);

                Item.ItemValueInfo itemValue = _LowDataMgr.instance.GetLowDataItemValueInfo(id );
                abilityList.Add(new NetData.ItemAbilityData(itemValue.OptionId, value));
            }

            itemData.StatList = abilityList;
            UIMgr.OpenDetailPopup(this, itemData, GetComponent<UIPanel>().depth + 3);
        }
    }
    
    public void AddLogChat(string chat)
    {
        char[] AlertArr = SceneManager.instance.optionData.OffAlram.ToCharArray(); //시스템알림
        if (AlertArr[3] == '2')
            return;

        chat = string.Format(_LowDataMgr.instance.GetStringCommon(913), chat);
        NetData.ChatData data = new NetData.ChatData(chat);

        AddChatData(data, ChatType.System);
        MiniTextList.Add(chat);
    }
    /*
    public void AddNotice(string msg, uint commonId)
    {
        if(0 < commonId)
            msg = _LowDataMgr.instance.GetStringCommon(commonId);

        for (int i = NoticeLbl.Length - 1; 0 < i; i--)
        {
            int arr = i - 1;
            NoticeLbl[i].color = NoticeLbl[arr].color;
            NoticeLbl[i].text = NoticeLbl[arr].text;
            NoticeDelay[i] = NoticeDelay[arr];
        }

        IsNoticeClear = false;
        NoticeDelay[0] = 1;
        NoticeLbl[0].color = Color.white;
        NoticeLbl[0].text = msg;
    }
    */
    /// <summary> 서버 or 클라에서 채팅 내용 추가하는 함수. </summary>
    void AddChatData(NetData.ChatData chatData, ChatType type)
    {
        List<NetData.ChatData> addChatList = null;
        if (ChatDic.TryGetValue(type, out addChatList))//채팅 리스트 등록
        {
            if (type == ChatType.Whisper)//귓말 대상 슬롯 추가 or 마지막 대사 갱신
            {
                bool isAddTarget = true;
                for (int i = 0; i < addChatList.Count; i++)
                {
                    if (!addChatList[i].WhisperUID.Equals(chatData.WhisperUID))
                        continue;

                    NetData.ChatData data = addChatList[i];
                    data.Msg = chatData.Msg;
                    addChatList.RemoveAt(i);
                    addChatList.Insert(0, data);

                    isAddTarget = false;
                    break;
                }

                if (isAddTarget)
                {
                    if (MaxWhisperTarget <= addChatList.Count)//최대값을 벗어나면 삭제.
                        addChatList.RemoveAt(addChatList.Count - 1);

                    addChatList.Insert(0, chatData);
                }
            }
            else
            {
                if (MaxChatLine <= addChatList.Count)//최대값을 벗어나면 삭제.
                    addChatList.RemoveAt(addChatList.Count - 1);

                addChatList.Insert(0, chatData);
            }
        }
        else
        {
            addChatList = new List<NetData.ChatData>();
            addChatList.Add(chatData);
            ChatDic.Add(type, addChatList);
        }

        if (type == ChatType.Whisper)//귓말 대상과의 대화 추가.
        {
            if (WhisperChatDic.TryGetValue(chatData.WhisperUID, out addChatList))
            {
                if (MaxChatLine <= addChatList.Count)//최대값을 벗어나면 삭제.
                    addChatList.RemoveAt(addChatList.Count - 1);

                addChatList.Insert(0, chatData);
            }
            else//신규
            {
                addChatList = new List<NetData.ChatData>();
                addChatList.Add(chatData);

                WhisperChatDic.Add(chatData.WhisperUID, addChatList);//대상 대화 리스트
            }
        }
        
        if(type == ChatType.System)//시스템일 경우 모든 대화모드에 추가함.
        {
            for(int i=0; i < (int)ChatType.Map+1; i++)
            {
                if ( (ChatType)i == ChatType.System )//여기에는 로그 남기지 않는다.
                    continue;

                List<NetData.ChatData> list = null;
                if(((ChatType)i == ChatType.Whisper))//귓말 대상 슬롯에는 로그 안보이게한다.
                {
                    var enumerator = WhisperChatDic.GetEnumerator();
                    while(enumerator.MoveNext() )//대화 목록에 추가.
                    {
                        if (MaxChatLine <= enumerator.Current.Value.Count)//최대값을 벗어나면 삭제.
                            enumerator.Current.Value.RemoveAt(enumerator.Current.Value.Count - 1);

                        enumerator.Current.Value.Insert(0, chatData);
                    }
                }
                else if (ChatDic.TryGetValue((ChatType)i, out list))
                {
                    if (MaxChatLine <= list.Count)//최대값을 벗어나면 삭제.
                        list.RemoveAt(list.Count - 1);

                    list.Insert(0, chatData);
                }
                else
                {
                    list = new List<NetData.ChatData>();
                    list.Add(chatData);
                    ChatDic.Add( (ChatType)i, list);
                }
            }
        }
        else//전역값에는 모든 채팅 내용을 담아놓는다.
        {
            List<NetData.ChatData> worldData = null;
            if (ChatDic.TryGetValue(ChatType.Everything, out worldData))
            {
                if (MaxChatLine <= worldData.Count)//최대값을 벗어나면 삭제.
                    worldData.RemoveAt(worldData.Count - 1);

                worldData.Insert(0, chatData);
            }
            else
            {
                worldData = new List<NetData.ChatData>();
                worldData.Add(chatData);
                ChatDic.Add(ChatType.Everything, worldData);
            }
        }

        if ( !BigChatPopup.activeSelf || (CurChatType != ChatType.Everything && type != ChatType.System && CurChatType != type) )//갱신하지 않는 조건.
            return;

        ///현재 보고있는 탭 대화목록 가져오기
        List<NetData.ChatData> chatList = null;
        if (CurChatType == ChatType.Whisper && 0 < WhisperData.WhisperUID)//귓속말 대상과의 대화 중이라면.
        {
            if (!WhisperChatDic.TryGetValue(WhisperData.WhisperUID, out chatList))
                chatList = new List<NetData.ChatData>();
        }
        else//그 외.
        {
            if (!ChatDic.TryGetValue(CurChatType, out chatList))
                chatList = new List<NetData.ChatData>();
        }

        RefreshSlotData(chatList);
    }
    
    /// <summary> 슬롯갱신 </summary>
    void RefreshSlotData(List<NetData.ChatData> chatList)
    {
        SlotData lastSlotData = null;
        long myCharUID = (long)NetData.instance.GetUserInfo()._charUUID;
        for (int i = 0; i < chatList.Count; i++)
        {
            bool isSet = false;
            byte slotType = 0;
            if (chatList[i].UserUID != 0)//다른 유저 or 나
            {
                if (CurChatType == ChatType.Whisper)//3 대상 리스트 4 귓말 대상과의 대화
                {
                    if (WhisperData.WhisperUID <= 0)//귓말 대상 리스트 슬롯
                        slotType = 3;
                    else//귓말 대화
                    {
                        if (chatList[i].UserUID.Equals(myCharUID))//내 캐릭터
                            slotType = 0;
                        else
                            slotType = 1;
                    }
                }
                else if (chatList[i].UserUID.Equals(myCharUID))//내 캐릭터
                    slotType = 0;
                else
                    slotType = 1;
            }
            else//메세지
                slotType = 2;

            ///위치 수정
            float y = 0;
            if (lastSlotData != null)//최초 Null일것이다.
            {
                if (lastSlotData.Type == 2)
                {
                    if(slotType != 2)
                        y = -10;
                }
                else
                {
                    if (slotType == 2)
                        y = 5;
                }

                //이전 슬롯 위치
                y = y + (((lastSlotData.Tf.collider as BoxCollider).size.y) + lastSlotData.PosY );
            }
            
            for (int j = i; j < ChatSlotList.Count; j++)//빈 슬롯을 찾는다.
            {
                if (ChatSlotList[j].Type != slotType)//타입이 다르면 무시
                    continue;

                isSet = true;
                lastSlotData = SetChatSlot(ChatSlotList[j], chatList[i], slotType, i);
                lastSlotData.PosY = y;
                break;
            }

            if (!isSet)//생성한다.
            {
                lastSlotData = SetChatSlot(null, chatList[i], slotType, i);
                lastSlotData.PosY = y;
            }
        }

        for (int j = chatList.Count; j < ChatSlotList.Count; j++)
        {
            if (ChatSlotList[j].Parent == TempTf)
                continue;
            
            ChatSlotList[j].Parent = TempTf;
        }
    }
    
    /// <summary> 채팅 슬롯 타입에 맞게 슬롯 셋팅 </summary>
    SlotData SetChatSlot(SlotData slotData, NetData.ChatData chatData, byte type, int num)
    {
        Transform tf = null;
        if (slotData == null)//빈 슬롯이 없다. 생성
        {
            Transform createTf = null;
            switch (type)
            {
                case 0://나
                    createTf = MySlot;
                    break;
                case 1://다른 유저
                //case 4://1:1 귓말
                    createTf = UserSlot;
                    break;
                case 2://텍스트
                    createTf = TxtSlot;
                    break;
                case 3://귓속말 대상 리스트
                    createTf = WhisperTargetSlot;
                    break;

            }

            tf = Instantiate(createTf) as Transform;
            slotData = new SlotData(tf, type);
        }
        else
        {
            tf = slotData.Tf;
            ChatSlotList.Remove(slotData);
        }

        if (type == 2)// 텍스트
        {
            tf.FindChild("txt").GetComponent<UILabel>().text = chatData.Msg;
            
            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickBroadCast(tf.FindChild("txt").GetComponent<UILabel>() );
            });
        }
        else//유저 대화 슬롯.
        {
            tf.FindChild("msg").GetComponent<UILabel>().text = chatData.Msg;
            tf.FindChild("port/icon").GetComponent<UISprite>().spriteName = UIHelper.GetClassPortIcon((uint)chatData.ClassId, 2);
            tf.FindChild("name").GetComponent<UILabel>().text = chatData.UserName;

            if (0 < chatData.VipLv)
            {
                tf.FindChild("vip").gameObject.SetActive(true);
                tf.FindChild("vip/txt").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), chatData.VipLv);
            }
            else
                tf.FindChild("vip").gameObject.SetActive(false);

            if (type != 0)//내가 아니라면 
            {
                EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, ()=> {
                    UIMgr.OpenUserInfoPopup(chatData.UserUID, chatData.UserName, chatData.ClassId, chatData.VipLv, chatData.Lv);
                } );
            }
            
            if (type == 3)
            {
                EventDelegate.Set(tf.FindChild("BtnTalk").GetComponent<UIEventTrigger>().onClick, () => {

                    StartWhisper(chatData);
                });
            }
        }

        tf.parent = DefaultGridTf;//0 < WhisperTargetId ? TargetGridTf : 
        tf.localScale = Vector3.one;

        ChatSlotList.Insert(num, slotData);

        return slotData;
    }

    /// <summary> 귓말 시작. </summary>
    public void StartWhisper(NetData.ChatData whisper)
    {
        if( !BigChatPopup.activeSelf)
            OnClickBigChatPopup();

        WhisperData = whisper;
        TabGroup.CoercionTab((int)ChatType.Whisper );
    }
    
}

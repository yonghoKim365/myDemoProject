using UnityEngine;
using System.Collections.Generic;

public delegate void LoadingCallbackFunction();

/// <summary>
/// Scene에서 공용으로 사용할 UI들 관리 부분.
/// </summary>
public partial class SceneManager
{
    #region :: Loading Panel ::

    //< 로딩할시에 팁 타입
    public enum eLoadingTipType
    {
        TOWN = 0,
        SINGLE_FOREST = 1,
        SINGLE_POISON = 2,
        SINGLE_ICE = 3,
        SINGLE_REMAINS = 4,
        SINGLE_VOLCANO = 5,
        RAID__FOREST = 6,
        RAID__POISON = 7,
        RAID__ICE = 8,
        RAID__REMAINS = 9,
        RAID__VOLCANO = 10,
        MINE = 11,
        PVP = 12,
        INFINITE = 13,
        SECRET_RAID = 14,
        SPARRING = 15,
        GUILDPVP = 16,
        TEAMPVP = 17,
        NONE,
    }

    public struct PopupData
    {
        public int LowDataID;
        public string MainMsg;
        public string TitleMsg;
        public string InputBtn_0;
        public string InputBtn_1;
        public string InputBtn_2;

        public System.Action OnCallBack_1;//Ok버튼 클릭되었을때 호출할 CallBack함수
        public System.Action OnCallBack_2;//Cancel버튼 클릭되었을때 호출할 CallBack함수
        public System.Action OnCallBack_3;//Cancel버튼 클릭되었을때 호출할 CallBack함수
    }

    public System.DateTime RegenPowerTime;//체력 갱신 시간
    public int CurPopupId = -1;
    public UI_OPEN_TYPE UiOpenType;
    public TutorialType _CurTutorial = TutorialType.NONE;//하이락키 확인용 public 외부 참조는 이걸로 하지 말것
    private List<PopupData> SystemPopupList = new List<PopupData>();
    //private List<NetData._ItemData> RecommendItemData = new List<NetData._ItemData>();
    //private List<ulong> NewItemId = new List<ulong>();

    protected GameObject _StaticRoot;
    protected LoadingTipPanel _LoadingTipPanel;
    protected NetProcess _NetProcess;
    protected Camera UICam;
    protected NoticePanel NoticePanel;

    protected List<UIBasePanel> SaveUIList = new List<UIBasePanel>();

    private List<int> AlramList = new List<int>();
    private List<int> BenefitAlram = new List<int>();
    private uint[] GachaFreeTime = new uint[2];

    public int MailCount;//신써아 나쁜넘

    public Transform StaticRoot
    {
        get
        {
            if (_StaticRoot == null)
                return null;

            return _StaticRoot.transform;
        }
    }

    //잠시
    public LoadingTipPanel LoadingTipPanel
    {
        get
        {
            return _LoadingTipPanel;
        }
    }

    void Init_UI()
    {
        _StaticRoot = ResourceMgr.InstantiateUI("UIObject/StaticRoot");
        _StaticRoot.transform.parent = transform;
        UICam = _StaticRoot.transform.FindChild("LoadingCamera").GetComponent<Camera>();

        for (int i = 0; i < (int)AlramIconType._SOCIAL + 1; i++)
        {
            AlramList.Add(0);
        }

        for (int i = 0; i < 5; i++)
        {
            BenefitAlram.Add(0);
        }

        _StaticRoot.SetActive(false);
    }

    public void ShowLoadingTipPanel(bool flag, GAME_MODE gameMode = GAME_MODE.NONE, System.Action callback = null)
    {
        if (flag)
        {
            if (_LoadingTipPanel == null)
            {
                GameObject loadingPanel = ResourceMgr.InstantiateUI("UIPanel/LoadingTipPanel");
                loadingPanel.transform.parent = _StaticRoot.transform;
                loadingPanel.transform.localPosition = Vector3.zero;
                loadingPanel.transform.localScale = Vector3.one;

                _LoadingTipPanel = loadingPanel.GetComponent<LoadingTipPanel>();

            }
            //ResourceMgr.InstantiateUI("UIPanel/LoadingTipPanel").GetComponent<LoadingTipPanel>();

            if (!UICam.enabled)
                UICam.enabled = true;

            _LoadingTipPanel.LoadingCallBack = callback;
            _LoadingTipPanel.Show(gameMode);

            if(gameMode != GAME_MODE.NONE)
                ClearNoticePanel();

            if (!_StaticRoot.activeSelf)
                _StaticRoot.SetActive(true);
        }
        else
        {
            if (_LoadingTipPanel != null)
                _LoadingTipPanel.Hide();
        }

    }

    public void ShowNetProcess(string protoName)
    {
        if (_NetProcess == null)
        {
            GameObject netprocess = ResourceMgr.InstantiateUI("UIObject/NetProcess");
            netprocess.transform.parent = _StaticRoot.transform;
            netprocess.transform.localPosition = Vector3.zero;
            netprocess.transform.localScale = Vector3.one;

            _NetProcess = netprocess.GetComponent<NetProcess>();

        }

        if (!UICam.enabled)
            UICam.enabled = true;

        _NetProcess.Show(protoName);

        if (!_StaticRoot.activeSelf)
            _StaticRoot.SetActive(true);
    }

    public void EndNetProcess(string protoName)
    {
        if (_NetProcess == null)
            return;

        _NetProcess.EndProcess(protoName);
    }

    public bool CheckNetProcess(string protoName)
    {
        if (_NetProcess == null)
            return false;

        return _NetProcess.CheckedProcessName(protoName);
    }

    /// <summary>
    /// 로딩페널이 보여지구 있는지 확인
    /// </summary>
    /// <returns></returns>
    public bool IsShowLoadingPanel()
    {
        if (_LoadingTipPanel == null)
            return false;

        return _LoadingTipPanel.gameObject.activeSelf;
    }

    public bool IsShowNetProcess()
    {
        if (_NetProcess == null)
            return false;

        return 0 < _NetProcess.GetProcessCount();
    }

    public bool IsShowStaticUI
    {
        get
        {
            if (IsShowLoadingPanel() || IsShowNetProcess())
                return true;

            return false;
        }
    }


    #endregion

    #region :: Indicator Panel ::

    public int IndicatorCount = 0;
    GameObject IndicatorPanel;

    public void AddIndicator(bool showBack = true)
    {
        IndicatorCount++;
        UpdateIndicator(showBack);
    }

    public void RemoveIndicator()
    {
        IndicatorCount--;
        UpdateIndicator();
    }

    public void ResetIndicator()
    {
        IndicatorCount = 0;
        UpdateIndicator();
    }

    void UpdateIndicator(bool showBack = true)
    {
        if (IndicatorCount <= 0)
        {
            IndicatorCount = 0;
            if (IndicatorPanel != null)
            {
                IndicatorPanel.SetActive(false);
            }
        }
        else
        {
            if (IndicatorPanel == null)
            {
                IndicatorPanel = ResourceMgr.InstantiateUI("Loading/IndicatorPanel");
            }

            if (IndicatorPanel != null)
            {
                IndicatorPanel.SetActive(true);
                IndicatorPanel.GetComponent<IndicatorPanel>().ShowBackground(showBack);
            }
        }
    }

    #endregion

    #region :: Console LogView ::

    uLinkConsoleGUI consoleGUI;
    public void StartConsoleView()
    {
        if (null == consoleGUI)
        {
            consoleGUI = gameObject.GetComponent<uLinkConsoleGUI>();
            if (null == consoleGUI)
                consoleGUI = gameObject.AddComponent<uLinkConsoleGUI>();
        }
    }

    public void SetConsoleView(bool type)
    {
        consoleGUI.isVisible = type;
    }

    #endregion

    /// <summary> 로딩 팁 페널 서브용 파라메터들. </summary>
    public void SetSubLoadingPanelParams(params object[] param)
    {
        _LoadingTipPanel.SubParams = param;
    }

    public void AddErrorPopup(int errorPopLowId, uint titleId = 141, uint btn_0 = 117)
    {
        if (CurPopupId == errorPopLowId)
            return;

        for (int i = 0; i < SystemPopupList.Count; i++)
        {
            if (SystemPopupList[i].LowDataID == errorPopLowId)
                return;
        }

        AddPopup(errorPopLowId, null, _LowDataMgr.instance.GetStringCommon(titleId), _LowDataMgr.instance.GetStringCommon(btn_0), null, null, null, null, null);
    }

    /// <summary> 팝업 </summary>
    public void AddPopup(int popLowId, string mainMsg, string title, string btn_0, string btn_1, string btn_2, System.Action callBack_1, System.Action callBack_2, System.Action callBack_3)
    {
        PopupData data;
        data.LowDataID = popLowId;
        data.MainMsg = mainMsg;
        data.TitleMsg = title;
        data.InputBtn_0 = btn_0;
        data.InputBtn_1 = btn_1;
        data.InputBtn_2 = btn_2;
        data.OnCallBack_1 = callBack_1;
        data.OnCallBack_2 = callBack_2;
        data.OnCallBack_3 = callBack_3;

        SystemPopupList.Add(data);
    }

    void LateUpdate()
    {
        if (SystemPopupList.Count == 0 || 0 <= CurPopupId || IsShowLoadingPanel())
            return;

        PopupData data = SystemPopupList[0];//맨 앞에있는 것부터 불러온다.
        UIMgr.Open("UIPopup/SystemPopup", data);

        CurPopupId = data.LowDataID;

        SystemPopupList.RemoveAt(0);//실행한 것은 삭제.
    }
    /*
    public void AddRecommendData(NetData._ItemData itemData)
    {
        Item.EquipmentInfo info = itemData.GetEquipLowData();
        if (info == null)//크게 잘못된듯?
            return;

        NewItemId.Add(itemData._itemIndex);

        Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(NetData.instance.GetUserInfo()._userCharIndex);
        if (info.Class != charLowData.Class || NetData.instance.UserLevel < info.LimitLevel)
            return;

        NetData._ItemData equipItem = NetData.instance.GetUserInfo().GetEquipParts((ePartType)info.UseParts);
        if (equipItem != null && itemData._Attack <= equipItem._Attack)//의미 없는 아이템
            return;

        for (int j = 0; j < RecommendItemData.Count; j++)//보유중인 아이템 확인
        {
            if (itemData.EquipPartType != RecommendItemData[j].EquipPartType)
                continue;

            if (itemData._Attack <= RecommendItemData[j]._Attack)//의미 없는 아이템 무시.
                return;

            RecommendItemData.RemoveAt(j);//더 좋은 걸 찾았다. 기존거 삭제
            break;
        }

        RecommendItemData.Add(itemData);
        if (!IsShowLoadingPanel())
            PopRecommendData();
    }

    public bool PopRecommendData()
    {
        if (RecommendItemData == null || RecommendItemData.Count <= 0)
            return false;

        UIBasePanel townPanel = UIMgr.GetTownBasePanel();
        if (!TownState.TownActive || townPanel == null || !townPanel.gameObject.activeSelf)
            return false;

        string path = "UIPopup/ChangePopup";
        UIMgr.Open(path, RecommendItemData);

        return true;
    }
    */
    /*
    public bool IsNewItemForUID(ulong id)
    {
        if (id == 0)
            return NewItemId.Count != 0 ? true : false;

        for (int i = 0; i < NewItemId.Count; i++)
        {
            if (!NewItemId[i].Equals(id))
                continue;

            return true;
        }

        return false;
    }

    public void ClearNewItemList()
    {
        NewItemId.Clear();
    }

    public void AddNewItemId(ulong id)
    {
        if (!NewItemId.Contains(id))
            NewItemId.Add(id);
    }
    */
    public void SetAlram(AlramIconType type, bool isAlram)
    {
        if ((int)type < AlramList.Count)
            AlramList[(int)type] = isAlram ? 1 : 0;
    }

    public bool IsAlram(AlramIconType type)
    {
        if ((int)type < AlramList.Count)
            return AlramList[(int)type] == 1 ? true : false;
        else
            return false;
    }

    public void SetGachaFreeTime(uint gold, uint cash)
    {
        GachaFreeTime[0] = gold;
        GachaFreeTime[1] = cash;
    }

    public uint[] GetGachaFreeTime
    {
        get
        {
            return GachaFreeTime;
        }
    }

    public void SetBenefitAlram(int arr, bool isAlram)
    {
        if (BenefitAlram.Count <= arr)
            return;

        BenefitAlram[arr] = isAlram ? 1 : 0;
    }

    public bool IsBenefitAlram(int arr)
    {
        if (arr == -1)
        {
            for (int i = 0; i < BenefitAlram.Count; i++)
            {
                if (BenefitAlram[i] == 1)
                    return true;
            }

            return false;
        }

        if (BenefitAlram.Count <= arr)
            return false;

        return BenefitAlram[arr] == 1;
    }

    public void SerCameraActive(bool isActive)
    {
        if (UICam == null)
            return;

        UICam.enabled = isActive;
    }

    public void UserLogout()
    {
        for (int i = 0; i < SaveUIList.Count; i++)
        {
            Destroy(SaveUIList[i].gameObject);
        }

        MailCount = 0;
        SaveUIList.Clear();
    }

    public List<UIBasePanel> GetSavePanel()
    {
        return SaveUIList;
    }

    public void AddSavePanel(UIBasePanel basePanel)
    {
        if (SaveUIList.Contains(basePanel))
            return;

        basePanel.transform.parent = StaticRoot;
        SaveUIList.Add(basePanel);
    }

    /// <summary> 내부적으로 메일이 몇개인지 체크 후 에러메세지 보여줌 </summary>
    public void CheckMailCount(int addCount, bool isMsg = false)
    {
        MailCount += addCount;
        if (MailCount < 0)
            MailCount = 0;

        if (isMsg)//더해줄때만 체크함
        {
            float delayTime = 0;
            UIBasePanel firstPanel = UIMgr.instance.GetFirstUI();
            if (firstPanel == null)
                return;

            if (firstPanel.name.Contains("GachaPanel"))
            {
                delayTime = 5f;//초후(뽑기 연출때문에 딜레이준다.
            }

            int warning = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.Postmaxwarningvalue);
            int maxCount = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MailMaxCount);
            if (warning <= MailCount)//경고
            {
                if (maxCount <= MailCount)//초과 에러
                {
                    TempCoroutine.instance.RemoveKeyDelay("GetMailError");
                    TempCoroutine.instance.KeyDelay("GetMailError", delayTime, () => {
                        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1052), maxCount);
                        SetNoticePanel(NoticeType.Message, 0, msg);
                    });
                }
                else//경고 에러
                {
                    TempCoroutine.instance.RemoveKeyDelay("GetMailError");
                    TempCoroutine.instance.KeyDelay("GetMailError", delayTime, () => {
                        string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1051), maxCount);
                        SetNoticePanel(NoticeType.Message, 0, msg);
                    });
                }
            }
        }

    }

    void SetTutorialType(TutorialType type)
    {
        switch (type)
        {
            case TutorialType.INGAME: //퀘스트
                _CurTutorial = TutorialType.QUEST;
                break;
            case TutorialType.QUEST:  //스테이지
                _CurTutorial = TutorialType.STAGE;
                break;
            case TutorialType.STAGE:  //장비 강화
                if (!TownState.TownActive)
                    _CurTutorial = TutorialType.ENCHANT;
                else
                    return;
                break;
            case TutorialType.ENCHANT: //업적
                _CurTutorial = TutorialType.ACHIEVE;
                break;
            case TutorialType.ACHIEVE://칭호
                _CurTutorial = TutorialType.TITLE;
                break;
            case TutorialType.TITLE://챕터보상
                _CurTutorial = TutorialType.CHAPTER_REWARD;
                break;
            case TutorialType.CHAPTER_REWARD://캐릭터 스킬
                //_CurTutorial = TutorialType.CHAR_SKILL;//아직 미구현 상태
                _CurTutorial = TutorialType.BENEFIT;//강제로 혜택으로 변경
                break;
            case TutorialType.CHAR_SKILL://혜택
                _CurTutorial = TutorialType.BENEFIT;
                break;
            case TutorialType.BENEFIT://소셜
                _CurTutorial = TutorialType.SOCIAL;
                break;
            case TutorialType.SOCIAL://마계의탑
                _CurTutorial = TutorialType.TOWER;
                break;
            case TutorialType.TOWER://뽑기
                //_CurTutorial = TutorialType.GACHA;// UI변경예정 보류
                _CurTutorial = TutorialType.PARTNER;//다음걸로 강제셋팅
                break;
            case TutorialType.GACHA://파트너 소환
                _CurTutorial = TutorialType.PARTNER;
                break;
            case TutorialType.PARTNER://상점 설명
                //_CurTutorial = TutorialType.SHOP;//아직 미구현
                _CurTutorial = TutorialType.CATEGORY;//강제로 재화인벤으로 변경
                break;

            case TutorialType.SHOP: //재화 인벤
                _CurTutorial = TutorialType.CATEGORY;
                break;
            case TutorialType.CATEGORY: //난투장
                _CurTutorial = TutorialType.FREEFIGHT;
                break;

            case TutorialType.FREEFIGHT: //장비셋트
                _CurTutorial = TutorialType.EQUIP_SET;
                break;
            case TutorialType.EQUIP_SET: //스테이지 어려움
                _CurTutorial = TutorialType.CHAPTER_HARD;
                break;
            case TutorialType.CHAPTER_HARD: //스킬 재료던전
                _CurTutorial = TutorialType.SKILL_DUNGEON;
                break;

            case TutorialType.SKILL_DUNGEON: //신분 획득 및 스킬셋 선택
                //_CurTutorial = TutorialType.STATUS;//아직 미구현
                _CurTutorial = TutorialType.ARENA;//차관으로 건너뜀
                break;
            case TutorialType.STATUS: //차관 입장
                _CurTutorial = TutorialType.ARENA;
                break;
            case TutorialType.ARENA: //재료던전
                _CurTutorial = TutorialType.EQUIP_DUNGEON;
                break;

            case TutorialType.EQUIP_DUNGEON://끝
                _CurTutorial = TutorialType.ALL_CLEAR;
                break;

            default:
                _CurTutorial = type;
                break;
                
                
        }
    }

    public TutorialType CurTutorial
    {
        set
        {
            SetTutorialType(value);
            ClearTutorial(value);
            //_CurTutorial = value + 1;
        }
        get
        {
            return _CurTutorial;
        }
    }

    /// <summary> 단순 튜토리얼 타입 넣기 </summary>
    public void SetTutoType(TutorialType type)
    {
        _CurTutorial = type;
    }
    
    public TutorialType NextTutorial()
    {
        CurTutorial = CurTutorial;
        return CurTutorial;
    }

    public bool IsClearTutorial(TutorialType type)
    {
        string tutoDataStr = PlayerPrefs.GetString(string.Format("TutorialData_{0}.json", NetData.instance.GetUserInfo().GetCharUUID()));
        JSONObject parseJs = new JSONObject(tutoDataStr);
        for (int i = 0; i < parseJs.Count; i++)
        {
            if (parseJs[i] != null)//이미 존재하는 튜토리얼 타입
            {
                JSONObject json = parseJs[i]["TutorialType"];
                if (json.n == (int)type)//클리어한 것
                    return true;
            }
        }

        //클리어하지 않음
        return false;
    }

    public void ClearTutorial(TutorialType type)
    {
        ulong key = NetData.instance.GetUserInfo().GetCharUUID();
        string tutoDataStr = null;
        if (PlayerPrefs.HasKey(string.Format("TutorialData_{0}.json", key)))
        {
            tutoDataStr = PlayerPrefs.GetString(string.Format("TutorialData_{0}.json", key));

            JSONObject parseJs = new JSONObject(tutoDataStr);
            for (int i = 0; i < parseJs.Count; i++)
            {
                if (parseJs[i] != null)//이미 존재하는 튜토리얼 타입
                {
                    JSONObject json = parseJs[i]["TutorialType"];
                    if (json.n == (int)type)
                        return;
                }
            }

            string newData = "{";
            newData += string.Format("\"TutorialType\":{0}", (int)type); //"\"{";
            newData += "}";

            parseJs.Add(new JSONObject(newData));
            PlayerPrefs.SetString(string.Format("TutorialData_{0}.json", key), parseJs.ToString());

            Debug.Log("Save Tutorial Data " + parseJs.ToString());
        }
        else
        {
            tutoDataStr = "{";
            tutoDataStr += string.Format("\"TutorialType\":{0}", (int)type);
            tutoDataStr += "}";

            JSONObject parseJs = new JSONObject();
            parseJs.Add(new JSONObject(tutoDataStr));
            PlayerPrefs.SetString(string.Format("TutorialData_{0}.json", key), parseJs.ToString());
        }
    }
    
    public void CheckTutorial()
    {
        ulong key = NetData.instance.GetUserInfo().GetCharUUID();
        string tutoDataStr = null;
        if (PlayerPrefs.HasKey(string.Format("TutorialData_{0}.json", key)))
        {
            tutoDataStr = PlayerPrefs.GetString(string.Format("TutorialData_{0}.json", key));
            JSONObject parseJs = new JSONObject(tutoDataStr);
            JSONObject json = parseJs[parseJs.Count-1]["TutorialType"];

            SetTutorialType((TutorialType)json.n);
        }
        else
            _CurTutorial = TutorialType.QUEST;
    }

#if UNITY_EDITOR
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.A) )
        //{
        //    string data = string.Format("{0},{1},{2}",
        //    1,
        //    1,
        //    1);

        //    SetNoticePanel(NoticeType.Achiev, 0, data);
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            SetNoticePanel(NoticeType.LevelUp, 0, null);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            UIMgr.instance.PrevAttack = 0;
            SetNoticePanel(NoticeType.PowerUp, 0, null);
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    SetNoticePanel(NoticeType.Quest, 1, null);
        //}

        if ( Input.GetKeyDown(KeyCode.M))
        {
            SetNoticePanel(NoticeType.Message, 0, "Test Message");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            SetNoticePanel(NoticeType.Game, 0, "Test Game Message");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SetNoticePanel(NoticeType.System, 0, "Test System Message");
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetNoticePanel(NoticeType.GetItem, 20426, null, true);
        }
        
        if (Input.GetKeyUp(KeyCode.F5) )
        {
            SetNoticePanel(NoticeType.Contents, 1);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //List<NetData.EmailAttachmentInfo> info = new List<NetData.EmailAttachmentInfo>();
            //info.Add(new NetData.EmailAttachmentInfo(0, 430035, 1));
            //SetNoticePanel(NoticeType.GetMailItem, 0, null, info);
            
            SetNoticePanel(NoticeType.GetMailItem, 25);
        }

    }
#endif

    public void SetBullewtin(int bulletType, uint lowDataId, uint grade, uint enchant, int opt, string userName, string dataStr)
    {
        string getName = null;
        switch ((Sw.eDynamicBulletinType)bulletType)//아이템의 이름 정의
        {
            case Sw.eDynamicBulletinType.DynamicBulletinType_Equipment://장비
                Item.EquipmentInfo itemInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo(lowDataId);
                if (itemInfo == null)
                    return;

                getName = string.Format("{0}{1}[-]"// [FFE400]+{2}[-]
                    , UIHelper.GetItemGradeColor(itemInfo.Grade)
                    , _LowDataMgr.instance.GetStringItem(itemInfo.NameId));
                //, pmsgDynamicBulletinSynInfoS.UnCount);
                break;
            case Sw.eDynamicBulletinType.DynamicBulletinType_Hero://파트너
                Partner.PartnerDataInfo parInfo = _LowDataMgr.instance.GetPartnerInfo(lowDataId);
                if (parInfo == null)
                    return;

                getName = string.Format("{0}{1}[-]"// [FFE400]+{2}[-]
                    , UIHelper.GetItemGradeColor((int)grade)
                    , _LowDataMgr.instance.GetStringUnit(parInfo.NameId));
                //, pmsgDynamicBulletinSynInfoS.UnCount
                break;

            default:
                Debug.LogError(string.Format("unDefined type = {0}", (Sw.eDynamicBulletinType)bulletType));
                break;
        }

        if (string.IsNullOrEmpty(getName))
            return;

        string broadCasting = null;
        switch ((Sw.eDynamicBulletinOperateType)opt)
        {
            case Sw.eDynamicBulletinOperateType.DynamicBulletinOperateType_Null://획득?

                if (bulletType == (int)Sw.eDynamicBulletinType.DynamicBulletinType_Equipment)//장비
                {
                    uint contantId = 0;
                    switch ((Sw.ITEM_OPT_TYPE)opt)//획득 어디인지 판별
                    {
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_GM_TEST:                 //GM테스트
                            contantId = 673;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_ITEM_FUSION:             //아이템 합성 
                            contantId = 33;
                            break;
                        //case ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_NEWBIE:                  //뉴비 증정
                        //    contantId = 33;
                        //    break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_STAGE_VICTORY:           //시나리오 스테이지 승리
                            contantId = 9;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_EMAIL:                   //메일
                            contantId = 136;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_TOWER_VICTORY:           //마탑 던전 승리
                            contantId = 194;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_SHOP_BY:                 //상점 구매
                            contantId = 462;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_BOSS_VICTORY:            //보스 던전 승리
                            contantId = 917;
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_MESS_VICTORY:            //난투장 드랍
                            contantId = 12;
                            break;

                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_TASK:                    //통상 퀘스트
                            contantId = 245;//퀘스트 : 244
                            break;
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_TASK_DAILY:              //일일 퀘스트
                            contantId = 246;
                            break;

                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_COMMON_POTTERY:          //보통 뽑기 1회
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_COMMON_POTTERY_MANYTIMES://보통 뽑기 10연뽑
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_SENIOR_POTTERY:          //고급뽑기 1회
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_SENIOR_POTTERY_MANYTIMES://고급 뽑기 10연뽑
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_COMMON_POTTERY_FREE:     //무료 보통 뽑기
                        case Sw.ITEM_OPT_TYPE.EQUIPMENT_OPT_ADD_SENIOR_POTTERY_FREE:     //무료 고급 뽑기    
                            contantId = 539;
                            break;

                        default:
                            Debug.LogError(string.Format("unDefined type = {0}", (Sw.ITEM_OPT_TYPE)opt));
                            break;
                    }

                    broadCasting = string.Format(_LowDataMgr.instance.GetStringCommon(1054), userName, _LowDataMgr.instance.GetStringCommon(contantId), getName);
                }
                else//파트너
                    broadCasting = string.Format(_LowDataMgr.instance.GetStringCommon(1055), getName, userName);

                break;
            case Sw.eDynamicBulletinOperateType.DynamicBulletinOperateType_Enchant://강화
                broadCasting = string.Format(_LowDataMgr.instance.GetStringCommon(1056), userName, getName, enchant);
                break;
            case Sw.eDynamicBulletinOperateType.DynamicBulletinOperateType_Evolve://승급
                broadCasting = string.Format(_LowDataMgr.instance.GetStringCommon(1057), userName, getName, grade);
                break;

            default:
                Debug.LogError(string.Format("unDefined type = {0}", (Sw.eDynamicBulletinOperateType)opt));
                break;
        }

        if (string.IsNullOrEmpty(broadCasting))
            return;

        UIBasePanel chatPop = UIMgr.GetUIBasePanel("UIPopup/ChatPopup", false);
        if (chatPop != null)
        {
            if(string.IsNullOrEmpty(dataStr))
                (chatPop as ChatPopup).OnReciveChat(broadCasting, ChatType.World);
            else
                (chatPop as ChatPopup).OnReciveChat(string.Format("[url=item/{0}]{1}[/url]", dataStr, broadCasting), ChatType.World);
        }

        SetNoticePanel(NoticeType.Game, 0, broadCasting);
    }

    public void SetNoticePanel(NoticeType type, uint condition = 0, string str = null, object obj = null)
    {
        if (type == NoticeType.Quest)// || type == NoticeType.Achiev
        {
            if (!TownState.TownActive)
                return;
        }

        if (NoticePanel == null)
        {
            UIBasePanel noticePanel = UIMgr.OpenNoticePanel(type, condition, str, obj);
            NoticePanel = noticePanel as NoticePanel;
        }
        else
        {
            NoticePanel.NoticePop(type, condition, str, obj);
        }
    }

    public void ClearNoticePanel()
    {
        if (NoticePanel == null)
            return;

        NoticePanel.ClearPopups();
    }

    public bool IsActiveNoticeType(NoticeType type)
    {
        if (NoticePanel == null)
            return false;

        return NoticePanel.IsActive(type);
    }

    public void HideRoot()
    {
        if ((_LoadingTipPanel == null || _LoadingTipPanel.IsHidePanel )
            && (_NetProcess == null || _NetProcess.IsHidePanel) )
            _StaticRoot.SetActive(false);
    }
}
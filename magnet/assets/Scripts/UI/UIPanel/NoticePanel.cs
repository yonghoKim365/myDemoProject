using UnityEngine;
using System.Collections.Generic;

public enum NoticeType
{
    System,//게임 시스템 공지
    Game,//게임 알림
    Quest,//퀘스트 완료
    Contents,//컨텐츠 오픈
    Achiev,//업적 알림
    LevelUp,//레벨 상승
    GetItem,//획득 아이템
    PowerUp,//전투력 상승
    Event,//컨텐츠 별 이벤트 이슈 알림
    Message,//가운데 나오는 알림 메세지.
    GetMailItem,//우편함에서 얻은 아이템 표기

    Max
}

public class NoticePanel : UIBasePanel
{

    //NoticeType이 타입에 따른 배열
    public GameObject[] NoticeGo;

    public UILabel SystemLbl;
    public UILabel GameLbl;
    public UILabel AchievLbl;
    public UILabel ContentsLbl;
    public UILabel[] StatUpLbl;//0 움직이는, 1안움직이는
    public UILabel[] QuestLbl;//0 Title, 1 Info
    public UILabel[] NoticeLbl;

    public Transform[] GetMailSlotTf;//메일함에서 수령했을때 연출할 슬롯 5개.

    public TweenPosition SystemPos;
    public TweenAlpha GameAlpha;

    public bool IsStopUpdate
    {
        set
        {
            _IsStopUpdate = value;
        }
        get
        {
            return _IsStopUpdate;
        }
    }

    public float GameOpenDuration = 0.5f;
    public float GameHideDuration = 0.5f;
    public float GameHideDelay = 2.5f;

    private StatUpData _StatUpData;
    private TimeData _AchievData;
    private TimeData _LevelUpData;
    private TimeData _QuestData;
    private TimeData _GetItemData;

    private TownState _Town;
    private TownState _TownState
    {
        get
        {
            if (_Town == null)
                _Town = SceneManager.instance.GetState<TownState>();

            return _Town;
        }
    }

    private List<MessageData> SystemList = new List<MessageData>();
    private List<MessageData> GameList = new List<MessageData>();
    private List<NetData.EmailAttachmentInfo> GetItemList = new List<NetData.EmailAttachmentInfo>();
    private List<GetItemData> GetUseItemList = new List<GetItemData>();

    private bool _IsStopUpdate;
    private bool IsMsgUpdate;
    private float[] NoticeDelay;

    private uint AchieveTabType;    //0 -> 일일업적 1 ->일반업적 
    private uint AchieveType;
    private uint AchieveSubType;
    private uint AchieveLv;

    #region 전투력 상승 연출 데이터
    private struct StatUpData
    {
        private bool IsGetAction;
        private bool IsPlaySound;
        private int NowStatValue;//현재 스텟
        private int MaxStatValue;//올라갈 스텟량
        private float RunTime;//플레이 초
        private float Duration;//몇초 안에 돌릴건지
        private float Delay;//후딜 몇초 줄 건지
        private string Msg;
        private string DecoStr;

        private string ColorStr;
        private Transform Parent;
        private UILabel StatUpLabel;

        public void StatUpInit(int nowStatValue, int maxStatValue, UILabel statUpLbl, Transform effParent)
        {
            NowStatValue = nowStatValue;//이전값
            MaxStatValue = maxStatValue;// 최종값은 여기서 계산
            Duration = 0.5f;
            Msg = _LowDataMgr.instance.GetStringCommon(47);

            bool isPlus = NowStatValue - MaxStatValue < 0;
            DecoStr = isPlus ? "↑" : "↓";
            ColorStr = isPlus ? "[FFFFFF]" : "[e84d29]";

            StatUpLabel = statUpLbl;
            //StatUpLabel.color = isPlus ? Color.white : Color.red;
            StatUpLabel.text = string.Format("{0}{1}{2}{3}[-]", ColorStr, Msg, NowStatValue.ToString("0"), DecoStr);

            Delay = 0.1f;
            RunTime = 0;
            if (isPlus)
                Parent = effParent;
            else
                Parent = null;
            //if (isPlus)
            //{

            //}

            IsGetAction = true;
            IsPlaySound = false;//상승, 하락 관계없이 실행
        }

        public bool Update()
        {
            if (!IsGetAction)
                return false;

            //if (SceneManager.instance.IsShowLoadingPanel())//로딩 패널이다 대기.
            //    return false;

            RunTime += Time.unscaledDeltaTime;
            if (RunTime < Delay)
                return false;

            if (!IsPlaySound)//한번만 실행시키려고 이렇게함.
            {
                if(Parent != null)
                {
                    GameObject effGo = UIHelper.CreateEffectInGame(Parent, "Fx_UI_fightingPower_01");
                    List<MeshRenderer> list = UIHelper.FindComponents<MeshRenderer>(effGo.transform);
                    int renderQ = Parent.parent.GetComponent<UIPanel>().startingRenderQueue - 1;
                    for (int i = 0; i < list.Count; i++)
                    {
                        SetRenderQueue q = list[i].transform.GetComponent<SetRenderQueue>();
                        if (q == null)
                            q = list[i].gameObject.AddComponent<SetRenderQueue>();

                        if (0 < renderQ - q.renderQueue)
                            q.ResetRenderQ(q.renderQueue + (renderQ - q.renderQueue));
                    }

                    Destroy(effGo, 2f);
                }

                SoundManager.instance.PlaySfxSound(eUISfx.UI_fighting_power_up, false);
                IsPlaySound = true;
            }

            float rate = (RunTime - Delay) / Duration;
            rate = Mathf.Clamp01(rate);

            float value = Mathf.Lerp(NowStatValue, MaxStatValue, rate);
            StatUpLabel.text = string.Format("{0}{1}{2}{3}[-]", ColorStr, Msg, value.ToString("0"), DecoStr);

            if (rate >= 1)//종료
            {
                if (1.5f + Duration < RunTime)//바로 사라지면 안되니 딜레이를 줬다
                {
                    NowStatValue = 0;
                    MaxStatValue = 0;

                    RunTime = 0;
                    Duration = 0;
                    IsGetAction = false;

                    return true;
                }
            }

            return false;
        }
    }
    #endregion
    #region 딜레이 및 타임 주는 데이터 알림 
    private struct TimeData
    {
        public bool IsStart;

        public float Duration;
        public float RunTime;//플레이 초
        public float Delay;//후딜 몇초 줄 건지

        public void TimeInit(float delay, float duration)
        {
            IsStart = true;
            Delay = delay;
            RunTime = 0f;
            Duration = duration;
        }

        public void Reset()
        {
            IsStart = false;
            Delay = 0;
            RunTime = 0f;
            Duration = 0;
        }

        public bool Update()
        {
            if (!IsStart)
                return false;

            RunTime += Time.deltaTime;
            if (RunTime < Delay)
                return false;

            float rate = (RunTime - Delay) / Duration;
            rate = Mathf.Clamp01(rate);

            if (rate >= 1)//종료
            {
                //if (Delay + Duration < RunTime)//바로 사라지면 안되니 딜레이를 줬다
                //{
                RunTime = 0;
                Duration = 0;
                IsStart = false;

                return true;
                //}
            }

            return false;
        }
    }
    #endregion
    #region 메세지 시간
    private class MessageData
    {
        private bool IsInit;
        private float AlphaOpen;
        private float AlphaHide;
        private float AlphaHideDelay;
        private string Msg;

        private UILabel Lbl;
        private UITweener Tweener;
        private TimeData _TimeData;

        public void MsgInit(string msg, float duration, UILabel lbl, UITweener tween)
        {
            Lbl = lbl;
            Tweener = tween;

            Msg = msg;
            IsInit = false;

            Tweener.ResetToBeginning();
            _TimeData.TimeInit(0, duration);
        }

        public void SetAlphaData(float open, float hide, float hideDelay)
        {
            AlphaOpen = open;
            AlphaHide = hide;
            AlphaHideDelay = hideDelay;
        }

        public bool TimeDataUpdate()
        {
            if (!IsInit)
            {
                if (Tweener is TweenPosition)
                {
                    Lbl.text = Msg;
                    char[] charArr = Msg.ToCharArray();
                    int strLength = 0;
                    for (int i = 0; i < charArr.Length; i++)
                    {
                        if (charArr[i].Equals(' '))
                            continue;
                        ++strLength;
                    }

                    TweenPosition tweenPos = Tweener as TweenPosition;
                    Vector3 toVec = Lbl.transform.localPosition;
                    toVec.x = -(tweenPos.from.x + strLength * (Lbl.fontSize + Lbl.spacingX));//SystemLbl.width);// 
                    tweenPos.to = toVec;

                    Tweener.PlayForward();
                    IsInit = true;

                }
                else if (Tweener is TweenAlpha)//켜질때 알파 나타나게
                {
                    if (0 < Tweener.delay)
                    {
                        Lbl.text = Msg;
                        TweenAlpha alpha = Tweener as TweenAlpha;
                        alpha.delay = 0;
                        alpha.duration = AlphaOpen;
                        alpha.to = 1;
                        alpha.from = 0;

                        //alpha.ResetToBeginning();
                        alpha.PlayForward();
                    }
                    else
                    {
                        if (Tweener.duration <= _TimeData.RunTime)
                        {
                            IsInit = true;

                            TweenAlpha alpha = Tweener as TweenAlpha;
                            alpha.delay = AlphaHideDelay;
                            alpha.duration = AlphaHide;
                            alpha.to = 0;
                            alpha.from = 1;

                            alpha.ResetToBeginning();
                            alpha.PlayForward();
                        }
                    }
                }
            }

            return _TimeData.Update();
        }
    }
    #endregion
    #region 아이템 획득 연출 데이터
    private struct GetItemData
    {
        public uint IconId;
        public bool IsShard;
    }
    #endregion

    public override void Init()
    {
        base.Init();

        for (int i = 0; i < NoticeGo.Length; i++)
        {
            NoticeGo[i].SetActive(false);
        }

        NoticeDelay = new float[NoticeLbl.Length];
        for (int i = 0; i < NoticeLbl.Length; i++)
        {
            NoticeDelay[i] = 1;
            NoticeLbl[i].color = new Color32(255, 255, 255, 0);
            NoticeLbl[i].text = "";
        }

        UIHelper.CreateEffectInGame(NoticeGo[(int)NoticeType.Contents].transform.FindChild("Effect"), "Fx_UI_new_contents_01");
        EventDelegate.Set(NoticeGo[(int)NoticeType.Achiev].GetComponent<UIEventTrigger>().onClick,//업적페널 오픈.
            delegate () {

                if (AchieveType <= 0 || !TownState.TownActive)
                {
                    AchieveType = 0;
                    AchieveSubType = 0;
                    AchieveLv = 0;
                    NoticeGo[(int)NoticeType.Achiev].SetActive(false);
                    return;
                }

                NoticeGo[(int)NoticeType.Achiev].SetActive(false);
                //AchieveType = 0;
                AchieveSubType = 0;
                AchieveLv = 0;
                if (uiMgr.IsActiveTutorial)//튜토리얼중 무시한다.
                    return;

                UIBasePanel check = uiMgr.GetCurPanel();
                if (check == null)
                    check = UIMgr.instance.GetSecondUI();
                
                if (check == null)
                    return;

                if (check.name.Contains("AchievePanel"))//업적 페널에 있는데 업적이 갱신되서 클릭함 무시한다.
                    return;

                if (check.name.Contains("TownPanel"))
                    check.Hide();
                else
                    check.ObjectHide();
                
                if (AchieveTabType == 0)
                    UIMgr.OpenAchievePanel(check, 0);
                else
                    UIMgr.OpenAchievePanel(check, 1, (int)AchieveType);

                if (_TownState != null && _TownState.MyHero != null)
                    _TownState.MyHero.ResetMoveTarget();
            });

        for (int i = 0; i < GetMailSlotTf.Length; i++)
        {
            UIHelper.CreateInvenSlot(GetMailSlotTf[i].FindChild("root"));
        }

        EventDelegate.Set(NoticeGo[(int)NoticeType.Contents].transform.FindChild("Icon").GetComponent<UIEventTrigger>().onClick,
            () => {

                NoticeGo[(int)NoticeType.Contents].SetActive(false);

                if (TownState.TownActive)
                {
                    UIBasePanel basePanel = uiMgr.GetCurPanel();//UIMgr.UIType.Default
                    if (basePanel == null)
                    {
                        basePanel = uiMgr.GetFirstUI();
                        if (basePanel == null)
                            basePanel = UIMgr.GetTownBasePanel();
                    }

                    if (basePanel != null )
                    {
                        if (!basePanel.name.Contains("TownPanel"))
                        {
                            if (basePanel.ReturnType == PrevReturnType.Close)
                                basePanel.Close();
                            else
                                basePanel.Hide();
                        }
                        else if (!basePanel.IsHidePanel)
                            basePanel.OnSubTutorial();
                    }
                }
                else
                {
                    UIBasePanel resultPanel = UIMgr.GetUIBasePanel("UIPanel/ResultRewardStarPanel");
                    if (resultPanel != null)
                    {
                        (resultPanel as ResultRewardStarPanel).GotoTown();
                    }
                }
            });
    }

    public override void LateInit()
    {
        base.LateInit();

        if (parameters.Length <= 0 || (NoticeType)parameters[0] == NoticeType.Max)
            return;

        NoticePop((NoticeType)parameters[0], (uint)parameters[1], (string)parameters[2], parameters[3]);
    }
    
    public void NoticePop(NoticeType type, uint condition, string str, object obj)
    {
        Debug.Log(string.Format("<color=yellow>NoticePanel RunType={0}</color>", type));
        Transform typeTf = NoticeGo[(int)type].transform;
        bool notActive = false;
        switch (type)
        {
            case NoticeType.PowerUp://전투력
                NetData._UserInfo charInven = NetData.instance.GetUserInfo();
                if (charInven == null)
                    return;

                int maxValue = (int)charInven.RefreshTotalAttackPoint(false);
                if (uiMgr.PrevAttack == maxValue)
                    return;

                notActive = true;
                NoticeGo[(int)type].SetActive(true);

                _StatUpData.StatUpInit(uiMgr.PrevAttack, maxValue,
                    StatUpLbl[0], typeTf.FindChild("effRoot"));

                int value = maxValue - uiMgr.PrevAttack;
                StatUpLbl[1].text = string.Format("{0}{1}[-]", value < 0 ? "[e84d29]" : "[FFFFFF]", value);

                uiMgr.PrevAttack = maxValue;
                break;

            case NoticeType.Achiev://업적

                string[] achievData = str.Split(',');
                AchieveType = uint.Parse(achievData[0]);
                AchieveSubType = uint.Parse(achievData[1]);
                AchieveLv = uint.Parse(achievData[2]);
                AchieveTabType = uint.Parse(achievData[3]);

                if (!TownState.TownActive)
                    return;
                else
                {
                    UIBasePanel gachaPanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
                    if (gachaPanel != null && !(gachaPanel as GachaPanel).IsEndAni)
                        return;
                }

                notActive = true;
                break;

            case NoticeType.LevelUp://레벨업
                GameObject go = UIHelper.CreateEffectInGame(NoticeGo[(int)NoticeType.LevelUp].transform, "Fx_UI_levelup_01" + SystemDefine.LocalEff);
                Destroy(go, 2f);

                _LevelUpData.TimeInit(0, 2f);
                break;

            case NoticeType.Quest://퀘스트 클리어
                Quest.QuestInfo info = _LowDataMgr.instance.GetLowDataQuestData(condition);
                if (info == null)
                    return;

                QuestLbl[0].text = info.Title;
                QuestLbl[1].text = info.LeftDescription;

                float questDuration = 0;
                List<UITweener> tweenList = UIHelper.FindComponents<UITweener>(typeTf);
                for (int i = 0; i < tweenList.Count; i++)
                {
                    tweenList[i].ResetToBeginning();
                    tweenList[i].PlayForward();
                    if (questDuration < tweenList[i].delay + tweenList[i].duration)
                        questDuration = tweenList[i].delay + tweenList[i].duration;
                }

                _QuestData.TimeInit(0, questDuration);
                break;

            case NoticeType.Message://메세지
                string msg = null;
                if (0 < condition)
                    msg = _LowDataMgr.instance.GetStringCommon(condition);
                else
                    msg = str;

                for (int i = NoticeLbl.Length - 1; 0 < i; i--)
                {
                    int arr = i - 1;
                    NoticeLbl[i].color = NoticeLbl[arr].color;
                    NoticeLbl[i].text = NoticeLbl[arr].text;
                    NoticeDelay[i] = NoticeDelay[arr];
                }

                IsMsgUpdate = true;
                NoticeDelay[0] = 1;
                NoticeLbl[0].color = Color.white;
                NoticeLbl[0].text = msg;
                break;

            case NoticeType.System://공지 시스템 메세지 처리
                MessageData systemData = new MessageData();
                systemData.MsgInit(str, SystemPos.duration + SystemPos.delay, SystemLbl, SystemPos);
                SystemList.Add(systemData);

                notActive = true;
                break;

            case NoticeType.Game://게임 알림(장비 습득 등) 메세지 처리
                MessageData gameData = new MessageData();
                gameData.MsgInit(str, GameOpenDuration + GameHideDuration + GameHideDelay, GameLbl, GameAlpha);
                gameData.SetAlphaData(GameOpenDuration, GameHideDuration, GameHideDelay);
                GameList.Add(gameData);

                notActive = true;
                break;

            case NoticeType.GetMailItem://우편함에서 얻은 아이템

                if (0 < condition)
                {
                    Quest.QuestInfo lowData = _LowDataMgr.instance.GetLowDataQuestData(condition);

                    if (0 < lowData.rewardGold)
                        GetItemList.Add(new NetData.EmailAttachmentInfo((int)AssetType.Gold, (int)AssetType.Gold, lowData.rewardGold));

                    if (0 < lowData.rewardEnergy)
                        GetItemList.Add(new NetData.EmailAttachmentInfo((int)AssetType.Energy, (int)AssetType.Energy, lowData.rewardEnergy));

                    if (0 < lowData.rewardExp)
                        GetItemList.Add(new NetData.EmailAttachmentInfo((int)AssetType.Exp, (int)AssetType.Exp, lowData.rewardExp));

                    if (0 < lowData.rewardItem)
                    {
                        GatchaReward.FixedRewardInfo gatcha = _LowDataMgr.instance.GetFixedRewardItem(lowData.rewardItem);
                        if (gatcha != null)
                        {
                            if (gatcha.ClassType != 99)
                            {
                                int classType = UIHelper.GetClassType(NetData.instance.GetUserInfo()._userCharIndex);
                                List<GatchaReward.FixedRewardInfo> gachaList = _LowDataMgr.instance.GetFixedRewardItemGroupList(lowData.rewardItem);
                                for (int i = 0; i < gachaList.Count; i++)
                                {
                                    if (gachaList[i].ClassType != classType)
                                        continue;

                                    GetItemList.Add(new NetData.EmailAttachmentInfo(gachaList[i].Type, gachaList[i].ItemId, gachaList[i].ItemCount));
                                    break;
                                }
                            }
                            else
                                GetItemList.Add(new NetData.EmailAttachmentInfo(gatcha.Type, gatcha.ItemId, gatcha.ItemCount));
                        }
                    }
                }
                else if (obj != null)
                {
                    List<NetData.EmailAttachmentInfo> mailList = (List<NetData.EmailAttachmentInfo>)obj;
                    GetItemList.AddRange(mailList);
                }

                if (GetItemList.Count <= 0)
                    return;

                TempCoroutine.instance.RemoveKeyDelay("GetMailItem");
                SoundManager.instance.PlaySfxSound(eUISfx.UI_reward_popup, false);
                break;

            case NoticeType.GetItem://획득 아이템 연출.
                GetItemData data;
                data.IconId = condition;
                data.IsShard = (bool)obj;
                GetUseItemList.Add(data);
                notActive = true;

                if (_GetItemData.IsStart)//이미 시작중이라면.
                {
                    //NoticeGo[(int)NoticeType.GetItem].SetActive(true);
                    bool create = false;
                    Transform itemTf = null;
                    if (GetUseItemList.Count - 1 < typeTf.childCount)
                        itemTf = typeTf.GetChild(GetUseItemList.Count - 1);
                    else
                    {
                        itemTf = Instantiate(typeTf.GetChild(0)) as Transform;
                        itemTf.parent = typeTf;
                        itemTf.localPosition = Vector3.zero;
                        itemTf.localScale = Vector3.one;

                        create = true;
                    }

                    itemTf.gameObject.SetActive(true);

                    float delay = 0;
                    List<UITweener> tweener = UIHelper.FindComponents<UITweener>(itemTf);
                    for (int j = 0; j < tweener.Count; j++)
                    {
                        if (create)
                            tweener[j].delay += (GetUseItemList.Count - 1) * 0.05f;
                        if (delay < tweener[j].duration + tweener[j].delay)
                            delay = tweener[j].duration + tweener[j].delay;

                        tweener[j].ResetToBeginning();
                        tweener[j].PlayForward();
                    }

                    UISprite sp = itemTf.FindChild("icon").GetComponent<UISprite>();
                    sp.spriteName = _LowDataMgr.instance.GetLowDataIcon(data.IconId);
                    sp.atlas = AtlasMgr.instance.GetLoadAtlas(data.IsShard ? LoadAtlasType.Shard : LoadAtlasType.UseItem);

                    _GetItemData.TimeInit(0, delay);
                }
                break;

            case NoticeType.Contents:
                uint locKey = 0;
                string iconName = null;
                switch ((OpenContentsType)condition)
                {
                    case OpenContentsType.Char: //오픈연출타입	캐릭터
                        iconName = "costume_btn";
                        locKey = 65;
                        break;
                    case OpenContentsType.Achiev: //오픈연출타입	업적	
                        iconName = "achi_btn";
                        locKey = 247;
                        break;
                    case OpenContentsType.Benefit: //오픈연출타입	혜택	
                        iconName = "benefit_btn";
                        locKey = 681;
                        break;
                    case OpenContentsType.Social: //오픈연출타입	소셜	
                        iconName = "social_btn";
                        locKey = 678;
                        break;
                    case OpenContentsType.Dungeon: //오픈연출타입	컨텐츠
                        iconName = "dungeon_btn";
                        locKey = 10;
                        break;
                    case OpenContentsType.Shop: //오픈연출타입	상품	
                        iconName = "pvp_shop";
                        locKey = 462;
                        break;
                    case OpenContentsType.Partner: //오픈연출타입	파트너
                        iconName = "firend_btn";
                        locKey = 7;
                        break;
                    case OpenContentsType.FreeFight: //오픈연출타입	난투장
                        iconName = "free_btn";
                        locKey = 12;
                        break;
                    case OpenContentsType.Rank: //오픈연출타입	랭킹	
                        iconName = "ranking_btn";
                        locKey = 161;
                        break;
                    case OpenContentsType.Guilde: //오픈연출타입	길드	
                        iconName = "guild_btn";
                        locKey = 8;
                        break;
                    case OpenContentsType.Category: //오픈연출타입	재화인벤
                        iconName = "inven_btn";
                        locKey = 1287;
                        break;
                    case OpenContentsType.Chapter: //오픈연출타입	재화인벤
                        iconName = "adventure_btn";
                        locKey = 9;
                        break;
                }

                string systemMsg = string.Format(_LowDataMgr.instance.GetStringCommon(9924), _LowDataMgr.instance.GetStringCommon(locKey));
                typeTf.FindChild("Icon").GetComponent<UISprite>().spriteName = iconName;
                typeTf.FindChild("Icon/Label_01").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(locKey);
                typeTf.FindChild("Icon/Label_02").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(locKey);
                typeTf.FindChild("Txt_Title").GetComponent<UILabel>().text = systemMsg;

                UIMgr.AddLogChat(systemMsg);

                SetRenderQueue renderQ = UIHelper.FindComponent<SetRenderQueue>(typeTf.FindChild("Effect"));
                if (renderQ != null)
                    renderQ.ResetRenderQ(GetComponent<UIPanel>().startingRenderQueue + 1);

                UITweener openTween = typeTf.GetComponent<UITweener>();
                openTween.ResetToBeginning();
                openTween.PlayForward();

                break;
        }

        if (!notActive)
            NoticeGo[(int)type].SetActive(true);
    }

    void Update()
    {
        if (IsStopUpdate || SceneManager.instance.IsShowLoadingPanel())
            return;

        if (TownState.TownActive && _TownState != null && _TownState.IsEndSceneLoad)//마을일 경우 처리할 곳.
        {
            if (0 < SystemList.Count)
            {
                if (!NoticeGo[(int)NoticeType.System].activeSelf)
                    NoticeGo[(int)NoticeType.System].SetActive(true);
                else if (SystemList[0].TimeDataUpdate())//시스템 메세지 업데이트
                {
                    SystemList.RemoveAt(0);
                    if (SystemList.Count <= 0)
                        NoticeGo[(int)NoticeType.System].SetActive(false);
                }
            }

            if (0 < GameList.Count)
            {
                if (!NoticeGo[(int)NoticeType.Game].activeSelf)
                    NoticeGo[(int)NoticeType.Game].SetActive(true);
                else if (GameList[0].TimeDataUpdate())//게임 알림 메세지 업데이트
                {
                    GameList.RemoveAt(0);
                    if (GameList.Count <= 0)
                        NoticeGo[(int)NoticeType.Game].SetActive(false);
                }
            }

            if (NoticeGo[(int)NoticeType.Quest].activeSelf)
            {
                if (_QuestData.Update())//퀘스트
                    NoticeGo[(int)NoticeType.Quest].SetActive(false);

                //이자식은 페널 다 가림 얘 끝나고 나머지 처리하자.
                return;
            }

            if (0 < AchieveType)//업적 알림
            {
                bool run = true;
                if (!_AchievData.IsStart)//초기화 시켜준다.
                {
                    UIBasePanel gachaPanel = UIMgr.GetUIBasePanel("UIPanel/GachaPanel");
                    if (gachaPanel != null && !(gachaPanel as GachaPanel).IsEndAni)
                        run = false;
                    else
                    {
                        if(AchieveTabType == 0)
                        {
                            //일일
                            Achievement.DailyInfo dailyAchievInfo = _LowDataMgr.instance.GetLowDataDaiylAchievementInfo(AchieveType, AchieveLv);
                            if(dailyAchievInfo == null)
                            {

                                AchieveType = 0;
                                AchieveSubType = 0;
                                AchieveLv = 0;
                                return;
                            }

                            AchievLbl.text = _LowDataMgr.instance.GetStringAchievement(dailyAchievInfo.DescId);

                        }
                        else
                        {
                            //일반
                            Achievement.AchievementInfo achievInfo = _LowDataMgr.instance.GetAchieveInfo(AchieveType, AchieveSubType, AchieveLv);
                            if (achievInfo == null)//문제있음.
                            {
                                AchieveType = 0;
                                AchieveSubType = 0;
                                AchieveLv = 0;
                                return;
                            }

                            AchievLbl.text = _LowDataMgr.instance.GetStringAchievement(achievInfo.DescriptionId);
                        }

                        float achievDuration = 0;
                        UITweener[] achievTweens = NoticeGo[(int)NoticeType.Achiev].transform.FindChild("UI_Txt").GetComponents<UITweener>();
                        for (int i = 0; i < achievTweens.Length; i++)
                        {
                            if (achievDuration < achievTweens[i].delay + achievTweens[i].duration)
                                achievDuration = achievTweens[i].delay + achievTweens[i].duration;

                            achievTweens[i].ResetToBeginning();
                            achievTweens[i].PlayForward();
                        }

                        _AchievData.TimeInit(0, achievDuration);
                        SoundManager.instance.PlaySfxSound(eUISfx.UI_achive_attendence_alarm, false);//업적 달성 알림음 / 혜택 달성음        

                        GameObject eff = UIHelper.CreateEffectInGame(NoticeGo[(int)NoticeType.Achiev].transform.FindChild("effect"), "Fx_UI_Achievement_01");
                        Destroy(eff, 2f);

                        NoticeGo[(int)NoticeType.Achiev].SetActive(true);
                    }
                }

                if (run && _AchievData.Update())
                {
                    NoticeGo[(int)NoticeType.Achiev].SetActive(false);
                    AchieveType = 0;
                    AchieveSubType = 0;
                    AchieveLv = 0;
                }
            }

            if (0 < GetUseItemList.Count)
                UpdateGetUseItem();
        }
        else if (!TownState.TownActive)//마을이 아닐 경우 처리할 곳.,
        {
            if (0 < SystemList.Count)
            {
                if (!NoticeGo[(int)NoticeType.System].activeSelf)
                    NoticeGo[(int)NoticeType.System].SetActive(true);
                else if (SystemList[0].TimeDataUpdate())//시스템 메세지 업데이트
                {
                    SystemList.RemoveAt(0);
                    if (SystemList.Count <= 0)
                        NoticeGo[(int)NoticeType.System].SetActive(false);
                }
            }

            if (0 < GameList.Count)
            {
                if (!NoticeGo[(int)NoticeType.Game].activeSelf)
                    NoticeGo[(int)NoticeType.Game].SetActive(true);
                else if (GameList[0].TimeDataUpdate())//게임 알림 메세지 업데이트
                {
                    GameList.RemoveAt(0);
                    if (GameList.Count <= 0)
                        NoticeGo[(int)NoticeType.Game].SetActive(false);
                }
            }
        }

        if (NoticeGo[(int)NoticeType.PowerUp].activeSelf && _StatUpData.Update())//전투력 상승
            NoticeGo[(int)NoticeType.PowerUp].SetActive(false);

        if (NoticeGo[(int)NoticeType.LevelUp].activeSelf && _LevelUpData.Update())//레벨업.
            NoticeGo[(int)NoticeType.LevelUp].SetActive(false);

        if (IsMsgUpdate)//메세지
            UpdateMessage();

        if (0 < GetItemList.Count)
            UpdateGetMailItem();
    }

    /// <summary> 소비 아이템 획득 연출 </summary>
    void UpdateGetUseItem()
    {
        UIBasePanel town = UIMgr.GetTownBasePanel();
        if (town == null || (town as TownPanel).IsHideTown)//획득 아이템 아이콘 연출.
            return;

        if (!_GetItemData.IsStart)
        {
            bool create = false;
            float delay = 0;
            NoticeGo[(int)NoticeType.GetItem].SetActive(true);
            Transform typeTf = NoticeGo[(int)NoticeType.GetItem].transform;
            for (int i = 0; i < GetUseItemList.Count; i++)
            {
                Transform itemTf = null;
                if (i < typeTf.childCount)
                    itemTf = typeTf.GetChild(i);
                else
                {
                    itemTf = Instantiate(typeTf.GetChild(0)) as Transform;
                    itemTf.parent = typeTf;
                    itemTf.localPosition = Vector3.zero;
                    itemTf.localScale = Vector3.one;

                    create = true;
                }

                List<UITweener> tweener = UIHelper.FindComponents<UITweener>(itemTf);
                for (int j = 0; j < tweener.Count; j++)
                {
                    if (create)
                        tweener[j].delay += i * 0.05f;

                    tweener[j].ResetToBeginning();
                    tweener[j].PlayForward();

                    if (delay < tweener[j].duration + tweener[j].delay)
                        delay = tweener[j].duration + tweener[j].delay;
                }

                itemTf.gameObject.SetActive(true);
                UISprite sp = itemTf.FindChild("icon").GetComponent<UISprite>();
                sp.spriteName = _LowDataMgr.instance.GetLowDataIcon(GetUseItemList[i].IconId);
                sp.atlas = AtlasMgr.instance.GetLoadAtlas(GetUseItemList[i].IsShard ? LoadAtlasType.Shard : LoadAtlasType.UseItem);
            }

            for (int i = GetUseItemList.Count; i < typeTf.childCount; i++)
            {
                typeTf.GetChild(i).gameObject.SetActive(false);
            }

            _GetItemData.TimeInit(0, delay);
        }

        if (_GetItemData.Update())
        {
            NoticeGo[(int)NoticeType.GetItem].SetActive(false);
            GetUseItemList.Clear();
        }
    }

    /// <summary> 메일 아이템 획득 연출 </summary>
    void UpdateGetMailItem()
    {
        for (int j = 0; j < GetMailSlotTf.Length; j++)
        {
            Transform tf = GetMailSlotTf[j];
            TweenAlpha alpha = tf.GetComponent<TweenAlpha>();
            if (0 < alpha.value)//아직 사용중.
                continue;

            NetData.EmailAttachmentInfo mailInfo = GetItemList[0];
            GetItemList.RemoveAt(0);

            tf.GetComponent<UIWidget>().enabled = true;
            UILabel amount = tf.FindChild("amount").GetComponent<UILabel>();
            Transform normalTf = tf.FindChild("normal");
            InvenItemSlotObject invenSlot = tf.FindChild("root").GetChild(0).GetComponent<InvenItemSlotObject>();

            Vector3 pos = tf.localPosition;
            pos.y = j * 55;
            tf.localPosition = pos;
            //if (mailInfo.GoodType == (int)AssetType.None || mailInfo.GoodType == (int)AssetType.Everything)//소비, 장비
            if (50 < mailInfo.Id)
            {
                normalTf.gameObject.SetActive(false);
                invenSlot.gameObject.SetActive(true);
                invenSlot.SetLowDataItemSlot(mailInfo.Id == 0 ? mailInfo.GoodType : mailInfo.Id, mailInfo.Count);
            }
            else
            {
                normalTf.gameObject.SetActive(true);
                invenSlot.gameObject.SetActive(false);

                switch ((AssetType)mailInfo.GoodType)
                {
                    case AssetType.Exp:
                        normalTf.FindChild("sprite").gameObject.SetActive(false);
                        normalTf.FindChild("exp").gameObject.SetActive(true);
                        break;
                    default:
                        normalTf.FindChild("sprite").gameObject.SetActive(true);
                        normalTf.FindChild("exp").gameObject.SetActive(false);
                        UISprite sp = normalTf.FindChild("sprite").GetComponent<UISprite>();

                        if (mailInfo.GoodType == (int)AssetType.Energy)
                            sp.spriteName = "stamina";
                        else if (mailInfo.GoodType == (int)AssetType.Gold)
                            sp.spriteName = "money";
                        else if (mailInfo.GoodType == (int)AssetType.Cash)
                            sp.spriteName = "cash";

                        break;
                }
            }

            amount.text = string.Format("{0}", mailInfo.Count);
            alpha.ResetToBeginning();
            alpha.PlayForward();

            break;
        }

        if (GetItemList.Count <= 0)
        {
            TweenAlpha mailAlpha = GetMailSlotTf[0].GetComponent<TweenAlpha>();
            TempCoroutine.instance.KeyDelay("GetMailItem", mailAlpha.delay + mailAlpha.duration, () =>
            {
                NoticeGo[(int)NoticeType.GetMailItem].SetActive(false);
            });
        }
    }

    /// <summary> 메세지 </summary>
    void UpdateMessage()
    {
        int count = 0;
        for (int i = 0; i < NoticeLbl.Length; i++)
        {
            float alpha = NoticeLbl[i].color.a;
            if (alpha <= 0)
            {
                NoticeLbl[i].color = new Color32(255, 255, 255, 0);
                ++count;
                continue;
            }

            NoticeDelay[i] -= Time.deltaTime;
            if (NoticeDelay[i] <= 0)
                alpha -= 0.5f * Time.deltaTime;
            else
                alpha = 1;

            NoticeLbl[i].color = new Color(1, 1, 1, alpha);
        }

        if (count == NoticeLbl.Length)
        {
            IsMsgUpdate = false;
            NoticeGo[(int)NoticeType.Message].SetActive(false);
        }
    }

    public void ClearPopups()
    {
        for (int i = (int)NoticeType.Quest; i < NoticeGo.Length; i++)
        {
            NoticeGo[i].SetActive(false);
        }

        _AchievData.Reset();

        AchieveType = 0;
        AchieveSubType = 0;
        AchieveLv = 0;
        IsMsgUpdate = false;
    }


    public bool IsActive(NoticeType type)
    {
        if (NoticeGo.Length - 1 < (int)type)
            return false;

        return NoticeGo[(int)type].activeSelf;
    }
}

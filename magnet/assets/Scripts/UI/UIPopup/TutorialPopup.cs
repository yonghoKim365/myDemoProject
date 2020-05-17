using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TutorialPopup : UIBasePanel {
    
    public GameObject TouchObj;
    public GameObject BlockObj;
    public GameObject[] GuideObj;

    public Transform OverDepthTf;

    public UIEventTrigger TouchEvent;

    public UILabel[] GuideText;
    public UILabel QuestName;

    public TutorialSupport TutoSupport;

    public InGameTutorialType CurInGameTuto;
    public InGameTutorialType NextInGameTuto;
    
    private Dictionary<InGameTutorialType, System.Action> InGameDic;
    private List<System.Action> TownList;

    private Quest.TutorialInfo IntroTutorial;
    private Quest.MainTutorialInfo Tutorial;

    private List<Quest.SubTalkSceneInfo> SubTalkList;
    private Transform TouchEff;

    private Transform TargetPanel;
    private System.Action TalkCallback;

    private TutorialType CurType;
    private int CurSortId = 1;
    private int GuideTalkCount;

    public bool IsNetworkDelay;
    private int _Condition;

    private TweenAlpha Tween;//true면 조이스틱 터치될때까지 깜빡이게 만든다.
    private List<EventDelegate> SaveDelList;
    private EventDelegate SaveDelegate;
    private List<TargetData> DataList = new List<TargetData>();

    private struct TargetData
    {
        public int Layer;
        public int InstanceID;
        public Transform Parent;//원본 어미
    }

    public override void Init()
    {
        base.Init();

        for (int i = 0; i < GuideObj.Length; i++)
        {
            GuideObj[i].SetActive(false);
            //GuideArrow[i].SetActive(false);
        }

        TouchObj.SetActive(false);
        
        GameObject touchEff = UIHelper.CreateEffectInGame(TouchObj.transform, "Fx_UI_tutorial_selection");
        TouchEff = touchEff.transform;
        //if(Debug.isDebugBuild)
        transform.FindChild("OverDepth/BtnSkip").gameObject.SetActive(true);
        if ( !TownState.TownActive && G_GameInfo.GameInfo is TutorialGameInfo)//인게임 튜토리얼일 경우 ui 스킵 보이게한다.
        {
            UIEventTrigger btnSkip = transform.FindChild("OverDepth/BtnSkip").GetComponent<UIEventTrigger>();
            EventDelegate.Add(btnSkip.onClick, ()=> {

                string msg = _LowDataMgr.instance.GetStringCommon(698);
                string title = _LowDataMgr.instance.GetStringCommon(141);
                string ok = _LowDataMgr.instance.GetStringCommon(117);
                string cancel = _LowDataMgr.instance.GetStringCommon(76);
                uiMgr.AddPopup(title, msg, ok, cancel, null, () => {
                    if (!TownState.TownActive && G_GameInfo.GameInfo != null && G_GameInfo.GameInfo is TutorialGameInfo)
                        (G_GameInfo.GameInfo as TutorialGameInfo).EndTutorial();
                    else
                        Close();
                });
            });
                
        }
        else
        {
            UIEventTrigger btnSkip = transform.FindChild("OverDepth/BtnSkip").GetComponent<UIEventTrigger>();
            EventDelegate.Add(btnSkip.onClick, () => {

                string msg = _LowDataMgr.instance.GetStringCommon(698);
                string title = _LowDataMgr.instance.GetStringCommon(141);
                string ok = _LowDataMgr.instance.GetStringCommon(117);
                string cancel = _LowDataMgr.instance.GetStringCommon(76);
                uiMgr.AddPopup(title, msg, ok, cancel, null, () => {
                    SceneManager.instance.CurTutorial = TutorialType.ALL_CLEAR;
                    Close();
                });
            });
        }
        
        EventDelegate.Set(TouchEvent.onClick, NextGuide);

        TouchEvent.collider.enabled = false;

        uiMgr.IsActiveTutorial = true;
    }

    public override void LateInit()
    {
        base.LateInit();
        uiMgr.IsActiveTutorial = true;
        if (parameters != null && 0 < parameters.Length)
        {
            int tutoType = (int)parameters[0];

            CurType = (TutorialType)tutoType;
            if (CurType == TutorialType.INGAME)
                InitInGame();
            else
            {
                InitOutTutorial((TutorialSupport)parameters[1]);
            }

        }
    }

    void SetTouch(string targetPath)
    {
        float scale = 50;
        if (!string.IsNullOrEmpty(targetPath))
        {
            Transform findTf = null;
            if (targetPath.Equals("this") )
                findTf = TargetPanel.transform;
            else
                findTf = TargetPanel.FindChild(targetPath);

            if (TutoSupport == null || !TutoSupport.IsScroll )
            {
                Vector3 pos = findTf.position;
                pos.z = transform.position.z;
                TouchObj.transform.position = pos;
            }
            else
            {
                TouchObj.transform.SetParent(findTf);
                TouchObj.transform.localPosition = Vector3.zero;
            }
            
            UISprite sp = GetFindComponent<UISprite>(findTf);
            if (sp != null)
            {
                if (sp.height < sp.width)
                    scale = sp.height;
                else
                    scale = sp.width;
            }

            //string guideArrow = "c";
            //Vector3 localPos = findTf.localPosition;
            //if (180 < localPos.y)//Down
            //    guideArrow = "b";
            //else if (localPos.y < -300)//Up
            //    guideArrow = "t";
            //else if (0 < localPos.x)//Left
            //    guideArrow = "l";
            //else 
            //    guideArrow = "r";
            
            //TouchObj.transform.FindChild("arrow_r").gameObject.SetActive(guideArrow.Equals("r"));
            //TouchObj.transform.FindChild("arrow_l").gameObject.SetActive(guideArrow.Equals("l"));
            //TouchObj.transform.FindChild("arrow_t").gameObject.SetActive(guideArrow.Equals("t"));
            //TouchObj.transform.FindChild("arrow_b").gameObject.SetActive(guideArrow.Equals("b"));
        }

        TouchEff.localScale = new Vector2(scale, scale);
        TouchObj.SetActive(true);
    }

    void InitInGame()
    {
        InGameHUDPanel hudPanel = G_GameInfo.GameInfo.HudPanel;
        InGameDic = new Dictionary<InGameTutorialType, System.Action>();
        BlockObj.SetActive(false);
        
        InGameDic.Add(InGameTutorialType.Joystick, delegate () {
            
            hudPanel.SetActiveTutorial(new string[] {
                        "TopBtnGroup" ,
                        "BottomBtnGroup",
                        "PotraitGroup",
                        "PotraitGroup/HeroInfo",
                        "PotraitGroup/PartnersGroup",
                        "PotraitGroup/QuestGroup"
                    }, new string[] {
                        "PotraitGroup",
                        //"JoysticGroup"
                    }
            );

            TargetPanel = hudPanel.GetJoystick().transform;
            SetTouch("Obj");
            IntroDefault(IntroTutorial.SubTalkSceneID);
            
            AddEventDelegate(false, "this", delegate () {
                TouchObj.SetActive(false);
                IntroNextGuide();
            });

            Tween = TweenAlpha.Begin(hudPanel.GetJoystick().gameObject, 1f, 0.7f);
            Tween.style = UITweener.Style.PingPong;
            
        });

        InGameDic.Add(InGameTutorialType.Attack, delegate () {

            TargetPanel = hudPanel.transform;

            hudPanel.SetActiveTutorial(new string[] {
                    "PotraitGroup/HeroInfo/Sp"
                },
                    new string[] {
                    "BottomBtnGroup",
                    "PotraitGroup/HeroInfo"
                }
            );

            SetTouch("BottomBtnGroup/AttackBtn");
            IntroDefault(IntroTutorial.SubTalkSceneID);
            AddEventDelegate(false, "BottomBtnGroup/AttackBtn", delegate () {

                IntroNextGuide();
                TouchObj.SetActive(false);
            });
        });

        InGameDic.Add(InGameTutorialType.Skill_01, delegate () {

            _Condition = 0;
            uint charIdx = NetData.instance.GetUserInfo()._userCharIndex;
            
            SetTouch("BottomBtnGroup/SkillBtn0");
            IntroDefault(IntroTutorial.SubTalkSceneID + charIdx);
            AddEventDelegate(true, "BottomBtnGroup/SkillBtn0", delegate () {
                if (!NextIntroTutorial)
                    return;

                //IntroTutorial = _LowDataMgr.instance.GetLowDataTutorial(IntroTutorial.NextTutorial);
                _Condition = 1;
                IntroDefault(IntroTutorial.SubTalkSceneID + charIdx);

                SetTouch("BottomBtnGroup/AttackBtn");
            });

        });

        InGameDic.Add(InGameTutorialType.Skill_02, delegate () {//평타 체인 스킬
            _Condition = 0;
            TouchObj.SetActive(false);
            IntroNextGuide();
        });
        
        InGameDic.Add(InGameTutorialType.Auto, delegate () {
            _Condition = 0;

            hudPanel.SetActiveTutorial(
                new string[] {
                    "TopBtnGroup/Coin",
                    "TopBtnGroup/SafetyZone",
                    "TopBtnGroup/PlayTime",
                    "TopBtnGroup/NowStageLabel",
                    "TopBtnGroup/PauseBtn"
                },
                new string[] {
                    "TopBtnGroup",
                    "TopBtnGroup/AutoBtn"
                }
            );

            SetTouch("TopBtnGroup/AutoBtn");
            IntroDefault(IntroTutorial.SubTalkSceneID);
            AddEventDelegate(true, "TopBtnGroup/AutoBtn", delegate () {
                IntroNextGuide();
                TouchObj.SetActive(false);
            });
            
        });

        InGameDic.Add(InGameTutorialType.SuperArmor, delegate () {

            hudPanel.SetActiveTutorial(null,
                new string[] {
                    "PotraitGroup/HeroInfo/Sp"
                }
            );
            
            SetTouch("PotraitGroup/HeroInfo/Sp");
            IntroDefault(IntroTutorial.SubTalkSceneID);

            G_GameInfo.PlayerController.Leader.SuperRecoveryTick = 1;
            TempCoroutine.instance.FrameDelay(5f, () => {
                IntroNextGuide();
                TouchObj.SetActive(false);
            });
        });

        InGameDic.Add(InGameTutorialType.EndGame, delegate () {

            if (IntroTutorial != null && 0 < IntroTutorial.SubTalkSceneID)
            {
                IntroDefault(IntroTutorial.SubTalkSceneID);
                TempCoroutine.instance.FrameDelay(2f, () => {
                    IntroNextGuide();
                });
            }

            TouchObj.SetActive(false);
            hudPanel.SetActiveTutorial(new string[] {
                        "TopBtnGroup" ,
                        "BottomBtnGroup",
                        "PotraitGroup",
                    }, null
            );

            hudPanel.SetJoyActive(false);
            G_GameInfo.GameInfo.AutoMode = true;
        });

        OnInGameTutorial(InGameTutorialType.Joystick);
    }
    
    public void InitOutTutorial(TutorialSupport tutoObj)
    {
        if (IsHidePanel)
            Show(null);

        StopAllCoroutines();
        TutoSupport = tutoObj;
        CurSortId = tutoObj.SortId+1;
        Tutorial = _LowDataMgr.instance.GetLowDataFirstMainTutorial((uint)CurType, (byte)tutoObj.SortId);//무조건 첫번째것부터 뽑아온다
        
        if (TutoSupport != null)
        {
            TargetPanel = TutoSupport.transform;
            BlockObj.SetActive(Tutorial.ProgressType == 0);
            //ReturnType = Tutorial.ProgressType == 0 ? PrevReturnType.Not : PrevReturnType.Close;
            DefaultData();
        }
    }

    void AddEventDelegate(bool isOnClick, string path, EventDelegate.Callback callBack)
    {
        if (callBack == null)
            return;
        
        Transform targetTf = null;
        if (path.Equals("this"))
            targetTf = TargetPanel;
        else
            targetTf = TargetPanel.FindChild(path);
        
        SaveDelegate = new EventDelegate(callBack);
        SaveDelegate.oneShot = true;//실행 후 삭제
        
        UIButton uiBtn = targetTf.GetComponent<UIButton>();
        if (uiBtn != null)
        {
            if (!isOnClick && uiBtn is UIRepeatButton)
                SaveDelList = (uiBtn as UIRepeatButton).onPressing;
            else
                SaveDelList = uiBtn.onClick;
        }
        else
        {
            UIEventTrigger uiTri = targetTf.GetComponent<UIEventTrigger>();
            if (uiTri == null)
            {
                uiBtn = GetFindComponent<UIButton>(targetTf);
                if (uiBtn == null)
                    uiTri = GetFindComponent<UIEventTrigger>(targetTf);
            }

            if (uiBtn != null)
            {
                if (!isOnClick && uiBtn is UIRepeatButton)
                    SaveDelList = (uiBtn as UIRepeatButton).onPressing;
                else
                    SaveDelList = uiBtn.onClick;
            }
            else
            {
                if (uiTri == null)
                    uiTri = targetTf.gameObject.AddComponent<UIEventTrigger>();

                if (isOnClick)
                    SaveDelList = uiTri.onClick;
                else
                    SaveDelList = uiTri.onPress;
            }
        }

        //EventDelegate.Set(SaveDelList, SaveDelegate);
        SaveDelList.Insert(0, SaveDelegate);
    }

    void SetLayer(Transform t, int layer)
    {
        if (t.gameObject.layer != LayerMask.NameToLayer("UI"))
            t.gameObject.layer = layer;

        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            if (child.gameObject.layer != LayerMask.NameToLayer("UI"))
                child.gameObject.layer = layer;

            SetLayer(child, layer);
        }
    }

    T GetFindComponent<T>(Transform tf) where T : MonoBehaviour
    {
        T oriT = tf.GetComponent<T>();
        if (oriT != null)
            return oriT;

        for (int i=0; i < tf.childCount; i++)
        {
            T t = tf.GetChild(i).GetComponent<T>();
            if (t != null)
                return t;
            else if(0 < tf.GetChild(i).childCount)
            {
                t = GetFindComponent<T>(tf.GetChild(i));
                if (t != null)
                    return t;
            }
        }

        return null;
    }

    /// <summary> 요것은 인 게임용 </summary>
    public void OnInGameTutorial(InGameTutorialType type)
    {
        if (InGameDic == null || !InGameDic.ContainsKey(type)
            || CurInGameTuto == type)
            return;
        
        System.Action call = null;

        IntroTutorial = _LowDataMgr.instance.GetLowDataFirstTutorial(1, (byte)type);
        InGameDic.TryGetValue(type, out call);
        
        if (call != null)
            call();
        
        CurInGameTuto = type;
        NextInGameTuto = type + 1;
        InGameDic.Remove(type);//사용한 것은 재사용을 방지하기 위해 삭제함.
    }
    
    public override void Hide()
    {
        if(TutoSupport != null)
        {
            TutoSupport.SkipTuto();
        }

        Clear();
        uiMgr.IsActiveTutorial = false;
        base.Hide();
    }

    public override void Close()
    {
        Hide();
        base.Hide();
    }
    
    void NextGuide()
    {
        //Debug.LogError(string.Format("type={0}, ID={1}, count={2}", uiMgr.CurTutorial, Tutorial != null ? Tutorial.ID : 0, SubTalkList.Count));
        if (Tutorial.GuideCount <= GuideTalkCount)
        {
            for (int i = 0; i < GuideObj.Length; i++)
            {
                GuideObj[i].SetActive(false);
            }
            
            //AniRunTime = -1;
            if( TownState.TownActive )
            {
                if(0 < Tutorial.NextTutorial)
                {
                    Quest.MainTutorialInfo tutoInfo = _LowDataMgr.instance.GetLowDataMainTutorial(Tutorial.NextTutorial);
                }
                else
                {
                    SceneManager.instance.CurTutorial = CurType;
                    TutorialType nextType = CurType + 1;
                    StartNextTutorial(nextType);
                }
            }

            return;
        }
        
        string talk = null;
        switch (GuideTalkCount)
        {
            case 0:
                talk = Tutorial.GuideText1;
                break;
            case 1:
                talk = Tutorial.GuideText2;
                break;
            case 2:
                talk = Tutorial.GuideText3;
                break;
            case 3:
                talk = Tutorial.GuideText4;
                break;
            case 4:
                talk = Tutorial.GuideText5;
                break;
        }

        if (talk.Contains("\\\\\n"))
            talk = talk.Replace("\\\\\n", "\n");
        else if (talk.Contains("\\\\n"))
            talk = talk.Replace("\\\\n", "\n");
        else if (talk.Contains("\\\n"))
            talk = talk.Replace("\\\n", "\n");
        else if (talk.Contains("\\n"))
            talk = talk.Replace("\\n", "\n");

        if (talk.Contains("{0}"))
        {
            uint id = 0;
            uint charId = NetData.instance.GetUserInfo()._userCharIndex;
            if (charId == 13000)
                id = 672;
            else
                id = 671;

            talk = string.Format(talk, _LowDataMgr.instance.GetStringCommon(id));
        }

        GuideObj[0].SetActive(Tutorial.GuideType == 0 );//우
        GuideObj[1].SetActive(Tutorial.GuideType == 1 );//좌
        GuideText[Tutorial.GuideType].text = talk;

        ++GuideTalkCount;

        TouchEvent.collider.enabled = GuideTalkCount < Tutorial.GuideCount;
        if(Tutorial.GuideCount <= GuideTalkCount)
        {
            SetTarget();
        }
    }

    /// <summary> 대화가 종료되었음 </summary>
    public void EndTalkScene()
    {
        Show();

        if (TalkCallback != null)
            TalkCallback();
    }

    void LateUpdate()
    {
        if(!TownState.TownActive && Tween != null)
        {
            InGameHUDPanel hud = TargetPanel.GetComponent<InGameHUDPanel>();
            if (hud != null && hud.GetJoystick().IsPress)
            {
                Tween.ResetToBeginning();
                Destroy(Tween);
                hud.GetJoystick().gameObject.GetComponent<UIPanel>().alpha = 1f;
            }
        }
    }
    
    public bool NextIntroTutorial
    {
        get {
            if (IntroTutorial == null || IntroTutorial.NextTutorial <= 0) {

                Hide();
                return false;
            }

            IntroTutorial = _LowDataMgr.instance.GetLowDataTutorial(IntroTutorial.NextTutorial);
            return true;
        }
    }

    public bool NextTutorial
    {
        get
        {
            if (Tutorial == null || Tutorial.NextTutorial <= 0)
            {
                //if (TutoSupport == null || string.IsNullOrEmpty(TutoSupport.StayPanelPath))
                Hide();

                return false;
            }

            //Tutorial = _LowDataMgr.instance.GetLowDataMainTutorial(Tutorial.NextTutorial);
            return true;
        }
    }
    
    IEnumerator StayPanel(string path, string subStay, System.Action sucCallback, System.Action failCallback, float stayTime=2f)
    {
        float time = 0;
        TargetPanel = null;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (stayTime != -1)//-1이면 무한 대기
            {
                time += 0.1f;
                if (stayTime < time)
                {
                    Debug.LogError(string.Format("tutorial not found error {0}, {1}", path, subStay));
                    CurType = TutorialType.ALL_CLEAR-1;
                    //NextTutorialType(TutorialType.ALL_CLEAR);
                    Hide();
                    break;
                }
            }

            if (TargetPanel != null )
            {
                if (string.IsNullOrEmpty(subStay))
                    break;

                if (TargetPanel.FindChild(subStay) != null && TargetPanel.FindChild(subStay).gameObject.activeSelf)
                    break;
            }
            else
            {
                UIBasePanel panel = UIMgr.GetUIBasePanel(path);
                if (panel != null && panel.gameObject.activeSelf)
                    TargetPanel = panel.transform;
            }
        }
        
        if ( (stayTime == -1 || time < stayTime))
        {
            //Debug.Log("Stay " + path);
            if(sucCallback != null)
                sucCallback();
        }
        else if(failCallback != null)
        {
            failCallback();
        }
    }
    
    public void Clear()
    {
        StopAllCoroutines();

        IsNetworkDelay = false;

        TouchObj.SetActive(false);
        if (OverDepthTf != null && OverDepthTf != TouchObj.transform.parent)
        {
            TouchObj.transform.SetParent(OverDepthTf);
        }

        for (int i = 0; i < GuideObj.Length; i++)
        {
            GuideObj[i].SetActive(false);
        }
    }
    
    /// <summary> 공통적으로 확인하는 것들. </summary>
    void DefaultData()
    {
        if (SaveDelList != null)
        {
            if (SaveDelList.Contains(SaveDelegate))//기존의 데이터 남아있을 시 삭제 후 등록한다.
                SaveDelList.Remove(SaveDelegate);
        }

        if (Tutorial.GuideCount <= 0)
        {
            for (int i = 0; i < GuideObj.Length; i++)
            {
                GuideObj[i].SetActive(false);
            }

            SetTarget();
        }
        else
        {
            GuideTalkCount = 0;
            NextGuide();

            List<UIPlayTween> tween = UIHelper.FindComponents<UIPlayTween>(GuideObj[Tutorial.GuideType].transform);
            for(int i=0; i < tween.Count; i++)
            {
                tween[i].resetOnPlay = true;
                tween[i].Play(true);
            }
        }
        
        QuestName.transform.parent.gameObject.SetActive(false);
        
    }
    
    void SetTarget()
    {
        if (Tutorial.ProgressType == 0)//강제진행.
        {
            TargetData data;
            data.Layer = TutoSupport.gameObject.layer;
            data.Parent = TutoSupport.transform.parent;
            data.InstanceID = TutoSupport.GetInstanceID();

            TutoSupport.transform.parent = OverDepthTf;
            TutoSupport.RefreshWidgets();

            DataList.Add(data);
        }

        SetTouch("this");
        AddEventDelegate(true, "this", delegate () {

            //TutoSupport.IsOnClick = true;
            TutoSupport.OnClickSupport();

            TouchObj.SetActive(false);
            for (int i = 0; i < DataList.Count; i++)
            {
                if (!TutoSupport.GetInstanceID().Equals(DataList[i].InstanceID))
                    continue;

                TutoSupport.transform.parent = DataList[i].Parent;
                TutoSupport.gameObject.layer = DataList[i].Layer;
                TutoSupport.RefreshWidgets();
                DataList.RemoveAt(i);
                break;
            }

            if (TouchObj.transform.parent != OverDepthTf)
            {
                TouchObj.transform.SetParent(OverDepthTf);
                TouchObj.transform.localPosition = Vector3.zero;
            }

            if (!string.IsNullOrEmpty(TutoSupport.StayPanelPath))
            {
                StartCoroutine(StayPanel(TutoSupport.StayPanelPath, null, () =>
                {
                    if (!NextTutorial && CurType != TutorialType.QUEST)
                        SceneManager.instance.CurTutorial = CurType;
                    
                    TutoSupport.ChangeTutoType();
                }, null, 15));
            }
            else if (!NextTutorial && CurType != TutorialType.QUEST)
            {
                SceneManager.instance.CurTutorial = CurType;
            }
            else if (TutoSupport != null)
            {
                TutoSupport.ChangeTutoType();
                TutoSupport.CheckNextTuto();
            }
        });
    }

    public int Condition{
        get {
            return _Condition;
        }
    }

    /// <summary> 해당 튜토리얼 부분 끝났으면 정리함 </summary>
    public void EndZone()
    {
        QuestName.transform.parent.gameObject.SetActive(false);
    }

    public int CurrentSortId
    {
        get {
            return CurSortId;
        }
    }

    public bool StartNextTutorial(TutorialType nextType)
    {
        UIBasePanel frontPanel = uiMgr.GetCurPanel();
        if (frontPanel == null)
        {
            Hide();
            return false;
        }
        else
        {
            return frontPanel.OnSubTutorial();
        }

        //return false;
    }

    #region 임시(2017.10.27) 인트로 튜토리얼 함수
    void IntroDefault(uint subTalkId)
    {
        if (SaveDelList != null)
        {
            if (SaveDelList.Contains(SaveDelegate))//기존의 데이터 남아있을 시 삭제 후 등록한다.
                SaveDelList.Remove(SaveDelegate);
        }

        if (subTalkId <= 0)
        {
            for (int i = 0; i < GuideObj.Length; i++)
            {
                GuideObj[i].SetActive(false);
            }
            return;
        }
        
        SubTalkList = _LowDataMgr.instance.GetLowDataSubTalkInfo(subTalkId);
        List<UIPlayTween> tween = UIHelper.FindComponents<UIPlayTween>(GuideObj[SubTalkList[0].Position].transform);
        for (int i = 0; i < tween.Count; i++)
        {
            tween[i].resetOnPlay = true;
            tween[i].Play(true);
        }

        IntroNextGuide();
        
        if (CurType != TutorialType.INGAME || string.IsNullOrEmpty(IntroTutorial.Title))
            QuestName.transform.parent.gameObject.SetActive(false);
        else
        {
            QuestName.transform.parent.gameObject.SetActive(true);
            QuestName.text = IntroTutorial.Title;
        }
    }

    void IntroNextGuide()
    {
        if (SubTalkList == null || SubTalkList.Count <= 0)
        {
            for (int i = 0; i < GuideObj.Length; i++)
            {
                GuideObj[i].SetActive(false);
            }
            
            return;
        }
        
        string talk = SubTalkList[0].TalkString;
        if (talk.Contains("\\\\\n"))
            talk = talk.Replace("\\\\\n", "\n");
        else if (talk.Contains("\\\\n"))
            talk = talk.Replace("\\\\n", "\n");
        else if (talk.Contains("\\\n"))
            talk = talk.Replace("\\\n", "\n");
        else if (talk.Contains("\\n"))
            talk = talk.Replace("\\n", "\n");

        if (talk.Contains("{0}"))
        {
            uint id = 0;
            uint charId = NetData.instance.GetUserInfo()._userCharIndex;
            if (charId == 13000)
                id = 672;
            else
                id = 671;

            talk = string.Format(talk, _LowDataMgr.instance.GetStringCommon(id));
        }
        
        GuideObj[0].SetActive(SubTalkList[0].Position == 0);//우
        GuideObj[1].SetActive(SubTalkList[0].Position == 1);//좌
        GuideText[SubTalkList[0].Position].text = talk;

        SubTalkList.RemoveAt(0);

        if (string.IsNullOrEmpty(IntroTutorial.Title))
            QuestName.transform.parent.gameObject.SetActive(false);
        else
        {
            QuestName.transform.parent.gameObject.SetActive(true);
            QuestName.text = IntroTutorial.Title;
        }
    }
    #endregion
}

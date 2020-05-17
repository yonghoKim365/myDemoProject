using UnityEngine;
using System.Collections.Generic;

public class TopMenuObject : MonoBehaviour {

    public GameObject CategoryGo;

    public UILabel RegenLbl;
    public UILabel Title;
    public UILabel[] MenuType;//0 캐쉬2, 1캐위1, 2 골드, 3 스테미너
    public UILabel[] UpValues;//0 체력, 1 캐쉬, 2 포인트(?), 3골드
    public UILabel[] DownValues;//0 체력, 1 캐쉬, 2 포인트(?), 3골드

    public TutorialSupport TutoSupport;

    public UISprite GoldFame;   

    private bool isFame;
    
    /// <summary> 뒤로가기 버튼 </summary>
    public UIEventTrigger BackButton;
    //public UIButton[] BtnMenus;

    /// <summary> 시작값 </summary>
    public Vector2 StartPos;

    public float AcDelay = 0;
    public float AcDuration = 0.5f;

    /// <summary> 현재 페널 </summary>
    private UIBasePanel CurPanel;
    private Transform SaveParent;
    private UiOpenEvent OpenEvent;
    private System.DateTime RegenTime;

    private List<ValueData> ValueList = new List<ValueData>();

    class ValueData
    {
        private float RunTime;

        public byte Type;
        public ulong NowValue;
        public ulong MaxValue;

        public UILabel Text;

        public ValueData(byte type, ulong now, ulong max, UILabel t)
        {
            RunTime = 0;
            Type = type;
            NowValue = now;
            MaxValue = max;
            Text = t;
            Text.gameObject.SetActive(true);

            TweenAlpha alpha = Text.gameObject.GetComponent<TweenAlpha>();
            alpha.ResetToBeginning();
            Text.alpha = 1;
            alpha.Play(true);

            Text.text = "0";
        }

        public bool Update(float duration, float delay)
        {
            RunTime += Time.unscaledDeltaTime;
            if (RunTime < delay)
                return true;
            
            float rate = (RunTime - delay) / duration;
            rate = Mathf.Clamp01(rate);
            
            float value = Mathf.Lerp(NowValue, MaxValue, rate);
            Text.text = string.Format("{0:0}", value);

            if (1 <= rate)
            {
                if (duration + 1.5f < RunTime)//끝
                {
                    Text.gameObject.SetActive(false);
                    return false;
                }
            }

            return true;
        }
    }

    public void Initialize(Transform saveParent)
    {
        OpenEvent = gameObject.AddComponent<UiOpenEvent>();

        SaveParent = saveParent;
        transform.parent = saveParent;
        transform.localPosition = StartPos;
        transform.localScale = Vector3.one;


        for (int i = 0; i < DownValues.Length; i++)
        {
            UpValues[i].gameObject.GetComponent<TweenAlpha>().delay = AcDelay+AcDuration;
            DownValues[i].gameObject.GetComponent<TweenAlpha>().delay = AcDelay+AcDuration;

            UpValues[i].gameObject.SetActive(false);
            DownValues[i].gameObject.SetActive(false);
        }

        for(int i=0; i < MenuType.Length; i++)
        {
            MenuType[i].text = "0";
        }

        SetPowerTime(SceneManager.instance.RegenPowerTime);

        ///뒤로가기 버튼 눌렀을때 현재 페널에 알려준다.
        EventDelegate.Set(BackButton.onClick, delegate () {
            UIMgr.instance.Prev();
        });

        EventDelegate.Set(CategoryGo.GetComponent<UIEventTrigger>().onClick, () => {

            UIBasePanel cago = UIMgr.GetUIBasePanel("UIPanel/CategoryPanel");
            if (cago != null && cago.gameObject.activeSelf)
                return;
            
            if(CurPanel != null)
                CurPanel.ObjectHide();
            UIMgr.OpenCategoryPanel(CurPanel);
        });

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 탑메뉴 셋팅. UIBasePanel의 Awake 에서 호출
    /// TopMenuType이 None이 아니어야지만 호출한다.
    /// </summary>
    public void ShowTopMenu(UIBasePanel curPanel)
    {
        CurPanel = curPanel;

        isFame = false;


        bool isEnergy = false;
        uint titleId = 0;
        if (CurPanel is TowerPanel)
            titleId = 194;
        else if (CurPanel is PartnerPanel)
            titleId = 7;
        else if (CurPanel is ShopPanel)
            titleId = 462;
        else if (CurPanel is ArenaPanel)
        {
            isFame = true;
            titleId = 823;
        }
        else if (CurPanel is FreefightPanel)
        {
            isFame = true;
            titleId = 12;
        }
        else if (CurPanel is ColosseumPanel)
            titleId = 720;
        else if (CurPanel is GuildJoinPanel)
            titleId = 215;
        else if (CurPanel is GuildPanel)
            titleId = 8;
        else if (CurPanel is QuestPopup)
            titleId = 244;
        else if (CurPanel is GachaPanel)
            titleId = 539;
        else if (CurPanel is RankPanel)
            titleId = 161;
        else if (CurPanel is BenefitPanel)
            titleId = 681;
        else if (CurPanel is VipPopup)
            titleId = 460;
        else if (curPanel is DungeonPanel)
        {
            titleId = 10;
            isEnergy = true;
        }
        else if (curPanel is ReadyPopup)
            titleId = 63;
        //else if (curPanel is ActivityPanel)
        //    titleId = 825;
        else if (curPanel is ChapterPanel)
        {
            titleId = 9;
            isEnergy = true;
        }
        else if(curPanel is CategoryPanel)
            titleId = 1287;
        else//이쪽으로 온다고 버그는 아니다.
            Debug.LogWarning(string.Format("undefined TitleName {0}", CurPanel.name));

        if (0 < titleId)
            SetTitleName(titleId);

        Level.LevelInfo levelInfo = _LowDataMgr.instance.GetLowDataCharLevel(NetData.instance.UserLevel);
        if (levelInfo.EnergyMax < NetData.instance.GetAsset(AssetType.Energy))//무료 충전개수를 초과했는지.
        {
            RegenLbl.gameObject.SetActive(false);
            RegenTime = System.DateTime.Now;
        }
        else if (RegenTime <= System.DateTime.Now)//충전시간 체크
            RegenLbl.gameObject.SetActive(false);

        RefreshCash(AssetType.None);
        MenuType[3].transform.parent.gameObject.SetActive(isEnergy);

        if (SceneManager.instance.IsClearTutorial(TutorialType.CATEGORY))
            CategoryGo.SetActive(!(curPanel is CategoryPanel));
        else
            CategoryGo.SetActive(false);

        TutoSupport.enabled = false;
        gameObject.SetActive(true);
        OpenEvent.SetEvent(true, () => {
            
        });
    }
    
    /// <summary> 재화 갱신 </summary>
    public void RefreshCash(AssetType type)
    {
        ulong gold = NetData.instance.GetAsset(AssetType.Gold);
        ulong cash = NetData.instance.GetAsset(AssetType.Cash);
        ulong energy = NetData.instance.GetAsset(AssetType.Energy);
        ulong fame = NetData.instance.GetAsset(AssetType.FAME); //난투장, 차관에서는 골드대신 성망이보여야함

        ulong prevGold = ulong.Parse(MenuType[1].text);
        ulong prevCash = ulong.Parse(MenuType[2].text);
        ulong prevStam = ulong.Parse(MenuType[3].text);

        switch (type)
        {
            case AssetType.None://모든 타입 갱신.
                MenuType[0].text = string.Format("0");//현재 이것에 관한 재화 없음.
				MenuType[1].text = SceneManager.instance.NumberToString(gold);//  string.Format("{0}", gold == 0 ? "0" : gold.ToString("#,##") );
				MenuType[2].text = SceneManager.instance.NumberToString(cash); //string.Format("{0}", cash == 0 ? "0" : cash.ToString("#,##") );
                MenuType[3].text = SceneManager.instance.NumberToString(energy);
                break;

            case AssetType.Energy://에너지
                MenuType[3].text = SceneManager.instance.NumberToString(energy);//string.Format("{0:#,#}", energy == 0 ? "0" : energy.ToString("#,##") );
                if (prevStam != energy)
                    SetActionValue(0, prevStam, energy);
                break;
            case AssetType.Gold://골드                
                MenuType[1].text = SceneManager.instance.NumberToString(gold);//string.Format("{0:#,#}", gold == 0 ? "0" : gold.ToString("#,##") );
                if (prevGold != gold)
                    SetActionValue(3, prevGold, gold);
                break;
            case AssetType.Cash://캐쉬
                MenuType[2].text = SceneManager.instance.NumberToString(cash);//string.Format("{0:#,#}", cash == 0 ? "0" : cash.ToString("#,##") );
                if (prevCash != cash)
                    SetActionValue(1, prevCash, cash);
                break;

            case AssetType.Badge://휘장
            case AssetType.FAME://성명    //일단  골드자리에?
                MenuType[1].text = SceneManager.instance.NumberToString(fame);//string.Format("{0:#,#}", gold == 0 ? "0" : gold.ToString("#,##") );
                break;
            case AssetType.Contribute://공헌
            case AssetType.Honor://명예
            case AssetType.LionBadge://사자왕휘장
            case AssetType.Sweep://소탕권
                break;
        }

        GoldFame.spriteName = isFame ? "starlight" : "money";
        MenuType[1].text = isFame ? SceneManager.instance.NumberToString(fame) : SceneManager.instance.NumberToString(gold);
    }

    void SetActionValue(byte type, ulong now, ulong max)
    {
        UILabel lbl = null;
        if (now < max)//증가
        {
            lbl = UpValues[type];
            max = max - now;
        }
        else
        {
            lbl = DownValues[type];
            max = now- max;
        }

        now = 0;

        for (int i = 0; i < ValueList.Count; i++)//기존에 있는지 검사
        {
            if (ValueList[i].Type != type)
                continue;

            ValueList[i].Text.gameObject.SetActive(false);
            ValueList.RemoveAt(i);
            break;
        }
        
        ValueList.Add(new ValueData(type, now, max, lbl));
    }

    /// <summary> 현재 팝업이 삭제 될때 호출 UIMgr </summary>
    public void OnHideTopMenu()
    {
        if (TutoSupport != null && !TutoSupport.enabled && UIMgr.instance.IsActiveTutorial)//스킵이 가능한 튜토리얼일것이다.
        {
            if (SceneManager.instance.CurTutorial != TutoSupport.TutoType)
            {
                UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
                if (basePanel != null)
                {
                    SceneManager.instance.CurTutorial = SceneManager.instance.CurTutorial;//다음으로 넘김.
                    basePanel.Hide();
                }
            }
        }

        OpenEvent.SetEvent(false, () => {
            gameObject.SetActive(false);
        });
    }
    
    void OnClickAddEnergy()
    {
        UIMgr.instance.AddPopup(141, 174, 117);
    }

    void OnClickAddGold()
    {
        UIMgr.instance.AddPopup(141, 174, 117);
    }

    void OnClickAddRuby()
    {
        UIMgr.instance.AddPopup(141, 174, 117);
    }
 
    public void SetTitleName(uint commonLowDataId)
    {
        if (Title == null)
            return;

        Title.text = _LowDataMgr.instance.GetStringCommon(commonLowDataId);
    }
    
    void Update()
    {
        if (System.DateTime.Now <= RegenTime)
        {
            System.TimeSpan time = RegenTime - System.DateTime.Now;
            RegenLbl.text = string.Format("{0:00} : {1:00}", time.Minutes, time.Seconds);
            if (time.Minutes <= 0 && time.Seconds < 0)
                RegenLbl.gameObject.SetActive(false);
        }

        if (0 < ValueList.Count)
        {
            for (int i = 0; i < ValueList.Count; i++)
            {
                if (ValueList[i].Update(AcDuration, AcDelay))
                    continue;

                ValueList.RemoveAt(i);
                --i;
            }
        }
    }
    
    public void SetPowerTime(System.DateTime time)
    {
        RegenTime = time;
        RegenLbl.gameObject.SetActive(true);
    }

    public void OnTutorial(TutorialType type, int sort)
    {
        TutoSupport.enabled = true;
        TutoSupport.TutoType = type;
        TutoSupport.SortId = sort;
        TutoSupport.OnTutoSupportStart();
    }

    public void EndTutorial()
    {
        if (TutoSupport.TutoType != SceneManager.instance.CurTutorial)
            return;

        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
        if (basePanel == null)
            return;

        if ((basePanel as TutorialPopup).CurrentSortId-1 != TutoSupport.SortId)
            return;

        //SceneManager.instance.CurTutorial = TutoSupport.TutoType;//다음으로 넘김.

        TutoSupport.enabled = false;
        TutoSupport.IsEnable = false;
        TutoSupport.TutoType = TutorialType.NONE;

        basePanel.Close();
    }
}

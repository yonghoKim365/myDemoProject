using UnityEngine;
using System.Collections.Generic;

public enum NextPanelType
{
    NONE,
    TOPMENU,
    DETAILPOPUP,
}

public class TutorialSupport : MonoBehaviour {

    public GameObject ActiveObj;
    public TutorialSupport NextTuto;
    public TutorialType TutoType;
    public NextPanelType NextPanelType;
    public string StayPanelPath;
    public string ParentPath;
    public int SortId = 1;

    public bool IsScroll = false;
    public bool IsEnable = false;
    public bool IsOnClick;
    
    void Awake()
    {
        if (SceneManager.instance.CurTutorial == TutorialType.ALL_CLEAR )
            this.enabled = false;

        //ChangeTutoType();
    }

    public bool OnTutoSupportStart()
    {
       //if( SceneManager.instance.IsClearTutorial(TutoType))
       //if(SceneManager.instance.CurTutorial != TutoType)
       //     return false;

        if (!CheckTuto)//갱신 조건 미달
            return false;
        
        IsEnable = false;
        IsOnClick = false;
        
        if (SortId == 1)
        {
            if(ActiveObj != null)
                ActiveObj.SetActive(true);

            IsEnable = true;
            UIMgr.OpenTutorialPopup((int)TutoType, this);
            return true;
        }
        else
        {
            UIBasePanel tuto = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
            if (tuto == null)
                tuto = UIMgr.OpenTutorialPopup((int)TutoType, this);
            else
            {
                if ((tuto as TutorialPopup).CurrentSortId == SortId)
                {
                    IsEnable = true;
                    (tuto as TutorialPopup).InitOutTutorial(this);
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckNextTuto()
    {
        if(NextPanelType != NextPanelType.NONE)
        {
            if(NextPanelType == NextPanelType.TOPMENU)
                UIMgr.instance.TopMenu.OnTutorial(TutoType, SortId + 1);
            return true;
        }
        else if (NextTuto == null || NextTuto.TutoType != TutoType || SortId+1 != NextTuto.SortId)
            return false;

        NextTuto.OnTutoSupportStart();
        return true;
    }
    
    public void RefreshWidgets()
    {
        List<UIWidget> spList = UIHelper.FindComponents<UIWidget>(transform);

        for(int i=0; i < spList.Count; i++)
        {
            spList[i].enabled = false;
            spList[i].enabled = true;
        }
    }

    public bool CheckTuto
    {
        get {
            switch (TutoType)
            {
                case TutorialType.PARTNER:
                    List<NetData._PartnerData> list = NetData.instance.GetUserInfo().GetPartnerList();
                    for(int i=0; i < list.Count; i++)
                    {
                        if (list[i]._needShard <= list[i].NowShard)
                        {
                            Partner.PartnerDataInfo info = list[i].GetLowData();
                            if(info.Class == (int)PartnerClassType.Attack)
                                return true;
                        }
                    }

                    return false;
                    
                case TutorialType.QUEST:
                    Quest.QuestInfo questInfo = QuestManager.instance.GetCurrentQuest();
                    if (questInfo != null)
                    {
                        if (questInfo.ID != 1)
                            return false;
                    }
                    break;
                case TutorialType.STAGE:
                    if (NetData.instance.UserLevel != 1)
                        return false;

                    break;

                case TutorialType.ENCHANT:
                    if (NetData.instance.GetUserInfo().GetEquipParts(ePartType.HELMET) == null)
                        return false;
                    break;

                case TutorialType.CHAPTER_REWARD:
                    if(ActiveObj != null && !ActiveObj.activeSelf)
                        return false;

                    break;
                case TutorialType.CHAPTER_HARD:
                    NetData.ClearSingleStageData data = null;
                    if (!NetData.instance.GetUserInfo().ClearSingleStageDic.TryGetValue(110, out data))
                        return false;
                    else if ((data.Clear_0 + data.Clear_1 + data.Clear_2) <= 0)
                        return false;
                        
                    break;
            }

            return true;
        }
    }

    public bool ChangeTutoType()
    {
        if (SortId != 1)
            return false;

        switch (TutoType)
        {
            case TutorialType.QUEST:
                TutoType = TutorialType.STAGE;
                StayPanelPath = null;
                IsOnClick = false;
                IsEnable = false;

                return true;

            case TutorialType.ENCHANT:
                //if( SceneManager.instance.IsClearTutorial(TutorialType.ENCHANT))
                if(SceneManager.instance.IsClearTutorial(TutorialType.TITLE))
                {
                    TutoType = TutorialType.EQUIP_SET;
                    StayPanelPath = null;
                    IsOnClick = false;
                    IsEnable = false;

                    return true;

                }
                else if (SceneManager.instance.IsClearTutorial(TutorialType.ENCHANT))
                {
                    TutoType = TutorialType.TITLE;
                    StayPanelPath = null;
                    IsOnClick = false;
                    IsEnable = false;

                    return true;
                }
                break;
            case TutorialType.GACHA:
                //if( SceneManager.instance.IsClearTutorial(TutorialType.SHOP))
                if (TutoType < SceneManager.instance.CurTutorial)
                {
                    TutoType = TutorialType.SHOP;
                    StayPanelPath = null;
                    IsOnClick = false;
                    IsEnable = false;

                    return true;
                }
                break;

            case TutorialType.CHAPTER_REWARD:
                //if( SceneManager.instance.IsClearTutorial(TutorialType.CHAPTER_HARD))
                if (TutoType < SceneManager.instance.CurTutorial)
                {
                    TutoType = TutorialType.CHAPTER_HARD;
                    StayPanelPath = "UIPanel/ChapterPanel";
                    IsOnClick = false;
                    IsEnable = false;

                    return true;
                }
                break;
        }

        return false;
    }
    
    public void OnClickSupport()
    {
        IsOnClick = true;

        if (ActiveObj != null)
            ActiveObj.SetActive(false);
    }

    public void SkipTuto()
    {
        OnDisable();
    }

    void OnDisable()
    {
        if ( !UIMgr.instance.IsActiveTutorial || !IsEnable || IsOnClick || SceneManager.instance._CurTutorial != TutoType || SceneManager.instance.IsClearTutorial(TutoType) )
            return;

        Quest.MainTutorialInfo info = _LowDataMgr.instance.GetLowDataFirstMainTutorial((uint)TutoType, (byte)SortId);
        if (info.ProgressType != 1)
            return;

        SceneManager.instance.CurTutorial = TutoType;//다음으로 넘김.
        UIBasePanel basePanel = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
        if (basePanel != null && !basePanel.IsHidePanel )
            basePanel.Hide();
    }
}

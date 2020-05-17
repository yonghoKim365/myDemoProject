using UnityEngine;
using System.Collections.Generic;

public class SelectTitlePopup : UIBasePanel {

    public UILabel CurTitle;
    public UILabel CurLeftTitle;
    public UILabel CurRightTitle;
    public UILabel GetCondition;

    public UIGrid LeftGrid;
    public UIGrid RightGrid;

    private _LowDataMgr LowMgr;
    private NetData._UserInfo UserInfo;

    private uint SelectTitleL;//접두
    private uint SelectTitleR;//접미
    private uint AchievId;

    public override void Init()
    {
        base.Init();

        LowMgr = _LowDataMgr.instance;
        UserInfo = NetData.instance.GetUserInfo();
        
        EventDelegate.Set(transform.FindChild("BtnClose").GetComponent<UIButton>().onClick, Close);
        EventDelegate.Set(transform.FindChild("BtnApply").GetComponent<UIButton>().onClick, delegate() {
            
            if (0 < SelectTitleL && (UserInfo._LeftTitle == 0 ? 1 : UserInfo._LeftTitle) != SelectTitleL)
                NetworkClient.instance.SendPMsgUseTitleC(SelectTitleL == 1 ? UserInfo._LeftTitle : SelectTitleL, SelectTitleL != 1);

            if (0 < SelectTitleR && (UserInfo._RightTitle == 0 ? 2 : UserInfo._RightTitle ) != SelectTitleR)
                NetworkClient.instance.SendPMsgUseTitleC(SelectTitleR == 2 ? UserInfo._RightTitle : SelectTitleR, SelectTitleR != 2);
            //Close();
        });

        EventDelegate.Set(GetCondition.gameObject.GetComponent<UIEventTrigger>().onClick, OnClickAchievLink);

        //LeftGrid.GetChild(0).gameObject.SetActive(false);
        //RightGrid.GetChild(0).gameObject.SetActive(false);
    }

    public override void LateInit()
    {
        base.LateInit();

        SelectTitleL = UserInfo._LeftTitle == 0 ? 1 : UserInfo._LeftTitle;
        SelectTitleR = UserInfo._RightTitle == 0 ? 2 : UserInfo._RightTitle;
        GetCondition.text = "";

        string leftTitle = null, rightTitle = null;
        if (0 < SelectTitleL)
        {
            Title.TitleInfo info = LowMgr.GetLowDataTitle(SelectTitleL);
            leftTitle = LowMgr.GetLowDataTitleName(info.TitleName);

            Achievement.AchievementInfo achievInfo = _LowDataMgr.instance.GetLowDataAchievInfo(info.LinkAchievement);
            if (achievInfo != null)
            {
                GetCondition.text = string.Format(_LowDataMgr.instance.GetStringCommon(1419), _LowDataMgr.instance.GetStringAchievement(achievInfo.NameId) );
            }
        }

        if (0 < SelectTitleR)
        {
            Title.TitleInfo info = LowMgr.GetLowDataTitle(SelectTitleR);
            rightTitle = LowMgr.GetLowDataTitleName(info.TitleName);

            if (string.IsNullOrEmpty(GetCondition.text))
            {
                Achievement.AchievementInfo achievInfo = _LowDataMgr.instance.GetLowDataAchievInfo(info.LinkAchievement);
                if (achievInfo != null)
                {
                    GetCondition.text = string.Format(_LowDataMgr.instance.GetStringCommon(1419), _LowDataMgr.instance.GetStringAchievement(achievInfo.NameId));
                }
            }
        }

        CurTitle.text = string.Format("{0} {1}", leftTitle, rightTitle);//합친거
        CurLeftTitle.text = leftTitle;//접두
        CurRightTitle.text = rightTitle;//접미

        NetworkClient.instance.SendPMsgTitleQueryInfoC();//칭호 정보 갱신
    }

    /// <summary> 칭호 슬롯 선택 콜백함수 </summary>
    void OnClickTitleSlot(uint idx, int arr)
    {
        Title.TitleInfo info = LowMgr.GetLowDataTitle(idx);

        string leftTitle = null, rightTitle = null;
        if (info.Type == 1)//접두
        {
            if (0 < SelectTitleR)
            {
                Title.TitleInfo info2 = LowMgr.GetLowDataTitle(SelectTitleR);
                rightTitle = LowMgr.GetLowDataTitleName(info2.TitleName);
            }

            int childCount = LeftGrid.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if(arr.ToString().Equals(LeftGrid.transform.GetChild(i).name) )
                    LeftGrid.transform.GetChild(i).FindChild("select").gameObject.SetActive(true);
                else
                    LeftGrid.transform.GetChild(i).FindChild("select").gameObject.SetActive(false);
            }

            leftTitle = LowMgr.GetLowDataTitleName(info.TitleName);
            SelectTitleL = idx;
        }
        else if (info.Type == 2)//접미
        {
            if (0 < SelectTitleL)
            {
                Title.TitleInfo info2 = LowMgr.GetLowDataTitle(SelectTitleL);
                leftTitle = LowMgr.GetLowDataTitleName(info2.TitleName);
            }

            int childCount = RightGrid.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (arr.ToString().Equals(RightGrid.transform.GetChild(i).name))
                    RightGrid.transform.GetChild(i).FindChild("select").gameObject.SetActive(true);
                else
                    RightGrid.transform.GetChild(i).FindChild("select").gameObject.SetActive(false);
            }

            rightTitle = LowMgr.GetLowDataTitleName(info.TitleName);
            SelectTitleR = idx;
        }

        AchievId = info.LinkAchievement;
        Achievement.AchievementInfo achievInfo = _LowDataMgr.instance.GetLowDataAchievInfo(info.LinkAchievement);
        if (achievInfo != null)
            GetCondition.text = string.Format(_LowDataMgr.instance.GetStringCommon(1419), _LowDataMgr.instance.GetStringAchievement(achievInfo.NameId));

        CurTitle.text = string.Format("{0} {1}", leftTitle, rightTitle);//합친거
        CurLeftTitle.text = leftTitle;//접두
        CurRightTitle.text = rightTitle;//접미
    }

    public override void Hide()
    {
        base.Hide();
    }

    public void OnPMsgChangeTitle(uint id)
    {
        if(uiMgr.IsActiveTutorial)
        {
            UIBasePanel tutoPopup = UIMgr.GetUIBasePanel("UIPopup/TutorialPopup");
            if(tutoPopup != null)
            {
                //(tutoPopup as TutorialPopup).OnEndNetwork("ApplyTitle");
                Close();
                return;
            }
        }

        Title.TitleInfo info = LowMgr.GetLowDataTitle(id);
        string titleName = LowMgr.GetLowDataTitleName(info.TitleName);
        if (info.Type == 1)//접두
        {
            CurLeftTitle.text = titleName;
        }
        else//접미
        {
            CurRightTitle.text = titleName;
        }
        
        SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, string.Format(_LowDataMgr.instance.GetStringCommon(973), titleName));
        CurTitle.text = string.Format("{0} {1}", CurLeftTitle.text, CurRightTitle.text);//합친거
    }

    /// <summary> 칭호 보유중인거 서버에서 받음 </summary>
    public void OnPMsgReciveGetList(List<uint> getList)
    {
        List<Title.TitleInfo> list = LowMgr.GetLowDataTitleList();
        List<Title.TitleInfo> sortList = new List<Title.TitleInfo>();

        int count = list.Count;
        if (getList.Count != list.Count)
        {
            int sortCount = 0;
            for (int i = 0; i < count; i++)
            {
                uint id = list[i].Id;
                bool isAdd = false;
                for (int j = 0; j < getList.Count; j++)
                {
                    if (id != getList[j])
                        continue;

                    isAdd = true;
                    sortList.Insert(sortCount++, list[i] );
                }

                if (isAdd)
                    continue;

                if (list[i].TitleName == 1)
                {
                    sortList.Insert(0, list[i]);
                    sortCount++;
                }
                else
                    sortList.Add(list[i]);
            }
        }
        else
            sortList = list;

        list.Clear();

        int leftCount=0,rightCount=0;
        bool isTutoSet = SceneManager.instance.CurTutorial == TutorialType.TITLE;
        for (int i = 0; i < count; i++)
        {
            Title.TitleInfo info = sortList[i];
            
            Transform parent = null,slotTf=null;
            if (info.Type == 1)//접두
            {
                parent = LeftGrid.transform;
                if (leftCount < parent.childCount)
                    slotTf = parent.GetChild(leftCount);
                ++leftCount;
            }
            else//접미
            {
                parent = RightGrid.transform;
                if (rightCount < parent.childCount)
                    slotTf = parent.GetChild(rightCount);
                ++rightCount;
            }

            if (slotTf == null)
            {
                slotTf = Instantiate(parent.GetChild(0)) as Transform;
                slotTf.parent = parent;
                slotTf.localPosition = Vector3.zero;
                slotTf.localScale = Vector3.one;
                slotTf.gameObject.SetActive(true);
            }

            int arr = i + 1;
            slotTf.name = arr.ToString();

            uint idx = info.Id;
            bool isGet = false;
            if (info.TitleName == 1)
                isGet = true;
            else
            {
                for (int j = 0; j < getList.Count; j++)
                {
                    if (getList[j].CompareTo(idx) != 0)
                        continue;

                    getList.RemoveAt(j);
                    isGet = true;
                    break;
                }
            }
            
            if (isGet && ((info.Type == 1 && SelectTitleL == info.Id) || (info.Type == 2 && SelectTitleR == info.Id)))
                slotTf.FindChild("select").gameObject.SetActive(true);
            else
                slotTf.FindChild("select").gameObject.SetActive(false);

            slotTf.GetComponent<UILabel>().text = LowMgr.GetLowDataTitleName(info.TitleName);
            slotTf.FindChild("on").gameObject.SetActive(isGet);
            slotTf.FindChild("off").gameObject.SetActive(!isGet);
            
            EventDelegate.Set(slotTf.GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickTitleSlot(idx, arr);
            });

            if (isTutoSet && 0 < info.LinkAchievement)//isGet && 
            {
                isTutoSet = false;
                TutorialSupport support = slotTf.gameObject.AddComponent<TutorialSupport>();
                support.TutoType = TutorialType.TITLE;
                support.SortId = 3;
                support.IsScroll = true;

                support.OnTutoSupportStart();
            }
        }

        //Destroy(LeftGrid.transform.GetChild(0).gameObject);
        //Destroy(RightGrid.transform.GetChild(0).gameObject);

        LeftGrid.repositionNow = true;
        RightGrid.repositionNow = true;
        LeftGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
        RightGrid.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }

    void OnClickAchievLink()
    {
        if (AchievId <= 0)
            return;

        UIBasePanel equipPanel = UIMgr.GetUIBasePanel("UIPanel/EquipmentPanel");
        Achievement.AchievementInfo achievInfo = _LowDataMgr.instance.GetLowDataAchievInfo(AchievId);
        UIMgr.OpenAchievePanel(equipPanel, 1, (int)achievInfo.Type);

        Hide();
    }
}

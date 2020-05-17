using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 스테이지를 클릭하면 열어줄 팝업 스크립트 스테이지에 관한 정보를 셋팅해 준다.
/// </summary>
public class ChapterPopup : MonoBehaviour {
    
    /// PopupObj의 배열값으로 사용
    enum POPUP_TYPE
    {
        NONE =-1,
        STAGE_INFO=0,
        SWEEP=1,
    }
    
    //스테이지에서 사용하는 라벨들
    enum STAGE_LABEL_TYPE
    {
        STAGE_NAME,//스테이지 명
        STORY,//스토리

        CLEAR_CONDITION_0,//스테이지 클리어 조건
        CLEAR_CONDITION_1,//스테이지 클리어 조건
        CLEAR_CONDITION_2,//스테이지 클리어 조건
    }
    
    enum StageSpType{//StageInfoView에서 사용하는 스프라이트 타입 정의
        Monster_0, Monster_1, Monster_2, Monster_3, DropItem_0, DropItem_1, DropItem_2, DropItem_3,
    }
    
    public GameObject[] PopupObj;// POPUP_TYPE 값으로 찾아서 사용한다.  0 StageInfoView, 1 ReadyInfoView
    public GameObject[] MoveStage;//0 left, 1 right
    public GameObject BtnSweep;
    public GameObject SweepItemPrefab;
    public GameObject InvenSlotPrefab;

    public GameObject FirstReward;
    public GameObject BasicReward;
    
    public UILabel[] StageLabels;// StageInfoView에서 사용하는 라벨, STAGE_LABEL_TYPE 값으로 배열 사용함
    public UILabel SweepCash;
    public UILabel SweepStamina;
    public UILabel SweepCount;
    public UILabel SweepStageName;

    public UISprite[] StageSprites;// StageInfoView에서 사용하는 스프라이트들 StageSpType사용
    public UISprite[] MissionClearStar;//스테이지의 미션을 클리어 했다면 이미지를 변경해줘야함

    public UIToggle SweepCardToggle;

    private TweenScale[] StageActions;
    private TweenScale[] SweepActions;
    private InvenItemSlotObject[] InvenSlot;
    private ChapterPanel Chapter;
    private ChapterPanel.DataStage CurrentData;//현제 보고 있는 데이터 정보.
    private ObjectPaging SweepPaging;

    private POPUP_TYPE CurPopupType;// 현제 열고 있는 팝의 타입
    
    private NetData._UserInfo CharInven;
    private List<NetData.SweepSlotData> SweepSlotList = new List<NetData.SweepSlotData>();

    private bool IsAutoFlop;
    private bool IsPopAction;

    public bool IsEnable
    {
        get {
           return CurPopupType != POPUP_TYPE.NONE;
        }
    }

    /// <summary> StoryModePanel에서 사용해준다. 초기화 함수 최초 1회만 호출함. </summary>
    public void PopupInit(ChapterPanel panel)
    {
        CharInven = NetData.instance.GetUserInfo();
        Chapter = panel;

        Transform infoTf = PopupObj[(int)POPUP_TYPE.STAGE_INFO].transform;
        EventDelegate.Set(infoTf.FindChild("BtnReady").GetComponent<UIButton>().onClick, delegate () {

            if (0 < CurrentData._StageLowData.DailyEntercount)
            {
                int clearCount = CurrentData._StageLowData.DailyEntercount - CurrentData.DailyClearCount;
                if (clearCount <= 0)//더이상 진행 불가능
                {
                    UIMgr.instance.AddErrorPopup((int)Sw.ErrorCode.ER_StageStartS_Daily_Time_Error);
                    return;
                }
            }


            UIMgr.OpenReadyPopup(GAME_MODE.SINGLE, UIMgr.GetUIBasePanel("UIPanel/ChapterPanel"), CurrentData.NeedEnerge, 0);
            DisablePopup();
        });

        EventDelegate.Set(infoTf.FindChild("BtnStart").GetComponent<UIButton>().onClick, delegate () {
            panel.GotoInGame();
        });

        EventDelegate.Set(transform.FindChild("StageInfoView/BtnClose").GetComponent<UIButton>().onClick, () => {
            OnClosePopup();
        });

        EventDelegate.Set(transform.FindChild("SweepPopup/BtnClose").GetComponent<UIButton>().onClick, () => {
            OnClosePopup();
        });

        EventDelegate.Set(BtnSweep.GetComponent<UIButton>().onClick, OnClickSweep);
        EventDelegate.Set(SweepCardToggle.onChange, OnToggleAutoFlop);

        DisablePopup();

        Transform sweepTf = PopupObj[(int)POPUP_TYPE.SWEEP].transform;
        EventDelegate.Set(sweepTf.FindChild("Info/BtnOneSweep").GetComponent<UIButton>().onClick, delegate () { OnClickBtnSweep(1); });
        EventDelegate.Set(sweepTf.FindChild("Info/BtnFiveSweep").GetComponent<UIButton>().onClick, delegate () { OnClickBtnSweep(5); });

        EventDelegate.Set(MoveStage[0].GetComponent<UIButton>().onClick, delegate () { OnClickMoveState(-1); });
        EventDelegate.Set(MoveStage[1].GetComponent<UIButton>().onClick, delegate () { OnClickMoveState(1); });

        Transform slotParentTf = infoTf.FindChild("Left/DropItem");
        InvenSlot = new InvenItemSlotObject[4];
        for(int i=0; i < 4; i++)
        {
            GameObject go = UIHelper.CreateInvenSlot( slotParentTf.FindChild(string.Format("{0}", i)));
            InvenSlot[i] = go.GetComponent<InvenItemSlotObject>();
        }


        sweepTf.FindChild("Info/BtnOneSweep/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(297), 1 );
        sweepTf.FindChild("Info/BtnFiveSweep/label").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(297), 10);

        Transform[] root = new Transform[] {
            SweepItemPrefab.transform.FindChild("ItemRoot_0"),
            SweepItemPrefab.transform.FindChild("ItemRoot_1"),
            SweepItemPrefab.transform.FindChild("ItemRoot_2"),
            SweepItemPrefab.transform.FindChild("ItemRoot_3"),
            SweepItemPrefab.transform.FindChild("CardItemRoot_0"),
            SweepItemPrefab.transform.FindChild("CardItemRoot_1")
        };

        for(int i=0; i < root.Length; i++)
        {
            GameObject go = Instantiate(InvenSlotPrefab) as GameObject;
            go.transform.parent = root[i];
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            go.collider.enabled = false;
        }

        Transform scrollTf = transform.FindChild("SweepPopup/ScrollParent/Scroll");
        SweepPaging = ObjectPaging.CreatePagingPanel(scrollTf.gameObject, scrollTf.GetChild(0).gameObject, SweepItemPrefab, 1, 4, 4, 0, OnCallBackSweepSlot);
        SweepItemPrefab.SetActive(false);

        //StageActionPanel.clipOffset = new Vector2(ActionFromIdx, 0);
        StageActions = transform.FindChild("StageInfoView").GetComponents<TweenScale>();
        StageActions[0].enabled = false;
        StageActions[1].enabled = false;

        SweepActions = transform.FindChild("SweepPopup").GetComponents<TweenScale>();
        SweepActions[0].enabled = false;
        SweepActions[1].enabled = false;

		if (SceneManager.instance.testData.bQuestTestStart) {
			TempCoroutine.instance.FrameDelay(2.0f, ()=>{
				panel.GotoInGame();
			});
		}
    }
    
    /// <summary>
    /// type에 맞는 객체를 켜주고 다른거는 꺼주는 함수
    /// </summary>
    /// <param name="type">키고자 하는 객체의 타입</param>
    void SetTypeEnablePopup(POPUP_TYPE type)
    {
        CurPopupType = type;
        PopupObj[(uint)type].SetActive(true);
        PopupObj[(uint)(type == POPUP_TYPE.STAGE_INFO ? POPUP_TYPE.SWEEP : POPUP_TYPE.STAGE_INFO) ].SetActive(false);
    }
    
    /// <summary>
    /// 팝업 객체들을 끈다. 
    /// </summary>
    void DisablePopup()
    {
        CurPopupType = POPUP_TYPE.NONE;
        gameObject.SetActive(false);//어미만 끈다.
    }

    /// <summary>
    /// 켜져있는 객체를 끈다. POPUP_TYPE.READY_INFO일 경우에는 이전 뎁스로 이동시켜준다.
    /// ChapterPanel에서 사용한다.
    /// </summary>
    public byte OnClosePopup()
    {
        if (IsPopAction)
            return 3;

        if(CurPopupType == POPUP_TYPE.NONE)
            return 0;//꺼져잇음

        if (CurPopupType == POPUP_TYPE.STAGE_INFO)//스테이지 정보 화면에서는 객체를 끈다.
        {
            DisablePopup();
            Chapter.SetArrow();

            if(SceneManager.instance.CurTutorial == TutorialType.CHAPTER_HARD)
            {
                TutorialSupport su = GetStartTuto();
                if(su.IsEnable)
                {
                    su.SkipTuto();
                }
            }

            return 1;
        }
        else if(CurPopupType == POPUP_TYPE.SWEEP)
        {
            SetTypeEnablePopup(POPUP_TYPE.STAGE_INFO);
            return 2;
        }

        return 0;
    }

    /// <summary>
    /// 이 함수가 시작점임 스테이지 정보 팝업에서 ReadyInfo로 넘어갈 수 있다.
    /// </summary>
    /// <param name="dataStage"></param>
    public void SetStagePopup(ChapterPanel.DataStage dataStage, bool isAction)
    {
        gameObject.SetActive(true);
        SetTypeEnablePopup(POPUP_TYPE.STAGE_INFO);
        CurrentData = dataStage;

        bool isNext = true, isPrev = true;
        ChapterPanel.DataStage prev = Chapter.GetDataStage( CurrentData.TableID-1 );
        ChapterPanel.DataStage next = Chapter.GetDataStage( CurrentData.TableID+1 );
        if (prev == null)
            isPrev = false;
        if (next == null)
            isNext = false;

        MoveStage[0].SetActive(isPrev);
        MoveStage[1].SetActive(isNext);

        //스트링 셋팅
        StageLabels[(uint)STAGE_LABEL_TYPE.STAGE_NAME].text = string.Format("{0}", dataStage.StageName);//_LowDataMgr.instance.GetString
        StageLabels[(uint)STAGE_LABEL_TYPE.STORY].text = dataStage.Story;//("스토리던전")

        //bool isTuto = UIMgr.instance.CurTutorial == TutorialType.CHAPTER;
        BasicReward.SetActive(false);
        FirstReward.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)// 최초
            {
                FirstReward.SetActive(true);

                GameObject fisrtRewardRecieve = FirstReward.transform.FindChild("getMark").gameObject;
                FirstReward.transform.FindChild("getMark").gameObject.SetActive(CurrentData.State == 1);
            }
            if (i == 1 && CurrentData._StageLowData.type == 2)// 기본
                BasicReward.SetActive(true);
            

            uint dropItemId = CurrentData.GetDropItemID(i);
            if (dropItemId <= 0)
            {
                InvenSlot[i].gameObject.SetActive(false);
            }
            else
            {
                int arr = i;
                InvenSlot[i].gameObject.SetActive(true);
                InvenSlot[i].SetLowDataItemSlot(dropItemId, 0, delegate (ulong key)
                {
                    UIMgr.OpenDetailPopup(Chapter, (uint)key);
                });
            }
            
            string appearMon = CurrentData.GetMonsterIcon(i);
            if (string.IsNullOrEmpty(appearMon))
            {
                StageSprites[(uint)StageSpType.Monster_0 + i].gameObject.SetActive(false);
            }
            else
            {
                StageSprites[(uint)StageSpType.Monster_0 + i].gameObject.SetActive(true);
                StageSprites[(uint)StageSpType.Monster_0 + i].spriteName = appearMon;
            }
        }

        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_0].text = dataStage.QuestList[0].GetTypeString();
        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_1].text = dataStage.QuestList[1].GetTypeString();
        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_2].text = dataStage.QuestList[2].GetTypeString();
        
        Color colA = new Color(1, 1, 1, 0.3f);
        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_0].transform.GetChild(0).GetComponent<UISprite>().color = CurrentData.ClearInfo[0] != 0 ? Color.white : colA;
        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_1].transform.GetChild(0).GetComponent<UISprite>().color = CurrentData.ClearInfo[1] != 0 ? Color.white : colA;
        StageLabels[(uint)STAGE_LABEL_TYPE.CLEAR_CONDITION_2].transform.GetChild(0).GetComponent<UISprite>().color = CurrentData.ClearInfo[2] != 0 ? Color.white : colA;

        int clearCount = CurrentData.ClearInfo.Length;
        for (int i = 0; i < clearCount; i++)
        {
            MissionClearStar[i].spriteName = CurrentData.ClearInfo[i] == 0 ? "Img_Star01" : "Img_Star02";
        }
        
        ///당장은 꺼놓는다.
        //소탕권 셋팅
        if (3 <= dataStage.TotalClearGrade)//조건을 만족했다면 소탕권 오픈
            BtnSweep.SetActive(true);
        else//진행해야할 스테이지면 소탕 못함.
            BtnSweep.SetActive(false);

        SweepSlotList.Clear();
        SweepPaging.NowCreate(SweepSlotList.Count);

        IsPopAction = isAction;
        if (IsPopAction)
        {
            StageActions[0].ResetToBeginning();
            StageActions[0].PlayForward();
            TempCoroutine.instance.FrameDelay(StageActions[0].delay + StageActions[0].duration, () => {
                IsPopAction = false;
                Chapter.OnSubTutorial();

                StageActions[1].ResetToBeginning();
                StageActions[1].PlayForward();
            });
        }

        if (CurrentData._StageLowData.DailyEntercount == 0)//남은횟수 : 무제한
            transform.FindChild("StageInfoView/Right/daily_count").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(302), _LowDataMgr.instance.GetStringCommon(305) );
        else//남은횟수 : n
        {
            string str = string.Format("{0} / {1}", CurrentData._StageLowData.DailyEntercount-CurrentData.DailyClearCount, CurrentData._StageLowData.DailyEntercount);
            transform.FindChild("StageInfoView/Right/daily_count").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(302), str);
        }
    }
    
    public ChapterPanel.DataStage GetChapterData()
    {
        return CurrentData;
    }

    /// <summary> 소탕 보상 슬롯 </summary>
    void OnCallBackSweepSlot(int arr, GameObject go)
    {
        if (SweepSlotList.Count <= arr)
        {
            go.SetActive(false);
            return;
        }

        NetData.SweepSlotData data = SweepSlotList[arr];

        go.SetActive(true);
        Transform slotTf = go.transform;
        slotTf.FindChild("title").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(304),(SweepSlotList.Count - arr) );//data.SweepCount
		slotTf.FindChild("get_gold").GetComponent<UILabel>().text = string.Format("{0}", data.GetGold == 0 ? "0" : data.GetGold.ToString()); //ToString("#,##"));
		slotTf.FindChild("get_exp").GetComponent<UILabel>().text = string.Format("{0}", data.GetExp == 0 ? "0" : data.GetExp.ToString()); //ToString("#,##"));
        
        for(int i=0; i < 4; i++)
        {
            Transform itemRoot = slotTf.FindChild(string.Format("ItemRoot_{0}", i) );
            if (itemRoot == null)
                continue;

            if (data.DropList.Count <= i )
            {
                itemRoot.gameObject.SetActive(false);
                continue;
            }

            NetData.DropItemData dropData = data.DropList[i];
            itemRoot.gameObject.SetActive(true);
            InvenItemSlotObject inven = itemRoot.GetChild(0).GetComponent<InvenItemSlotObject>();
            inven.SetLowDataItemSlot(dropData.LowDataId, dropData.Amount);
        }
        
        for (int i = 0; i < 2; i++)
        {
            Transform itemRoot = slotTf.FindChild(string.Format("CardItemRoot_{0}", i));
            if (itemRoot == null)
                continue;

            if (data.CardList.Count <= i )
            {
                itemRoot.gameObject.SetActive(false);
                continue;
            }

            NetData.DropItemData dropData = data.CardList[i];
            itemRoot.gameObject.SetActive(true);
            InvenItemSlotObject inven = itemRoot.GetChild(0).GetComponent<InvenItemSlotObject>();
            inven.SetLowDataItemSlot(dropData.LowDataId, dropData.Amount);
        }

    }
    
    /// <summary> 소탕 보상 아이템 응답 </summary>
    public void OnPMsgSweepReward(NetData.SweepSlotData slotData)
    {
        SweepSlotList.Insert(0, slotData);

        TempCoroutine.instance.FrameDelay(0.2f, ()=> { 
            SweepPaging.RefreshSlot(SweepSlotList.Count);
            SoundManager.instance.PlaySfxSound(eUISfx.UI_reward_popup, false);
        });
    }

    /// <summary> 소탕 끝난거 응답 갱신시켜준다. </summary>
    public void OnPMsgStageSweep(int dailyClearCount)
    {
        CurrentData.DailyClearCount = dailyClearCount;
        string sweepCount = null;
        if (CurrentData._StageLowData.DailyEntercount == 0)
            sweepCount = string.Format(_LowDataMgr.instance.GetStringCommon(302), _LowDataMgr.instance.GetStringCommon(305));//무제한
        else
        {
            int count = CurrentData._StageLowData.DailyEntercount - CurrentData.DailyClearCount;
            if (count < 0)//혹시 모를 예외처리
                count = 0;

            sweepCount = string.Format(_LowDataMgr.instance.GetStringCommon(302), count);//무제한
        }

        SweepCount.text = sweepCount;
    }
    
    public float GetActionTime()
    {
        if ( StageActions == null || StageActions.Length < 1)
            return 0.2f;

        return (StageActions[0].delay + StageActions[0].duration) + (StageActions[1].delay + StageActions[1].duration);
    }

    #region 이벤트 버튼

    /// <summary>
    /// 소탕 버튼
    /// </summary>
    void OnClickSweep()
    {
        SweepActions[0].ResetToBeginning();
        SweepActions[0].PlayForward();
        TempCoroutine.instance.FrameDelay(SweepActions[0].delay + SweepActions[0].duration, () => {
            SweepActions[1].ResetToBeginning();
            SweepActions[1].PlayForward();
        });

        string sweepCount = null;
        if (CurrentData._StageLowData.DailyEntercount == 0)
            sweepCount = string.Format(_LowDataMgr.instance.GetStringCommon(302), _LowDataMgr.instance.GetStringCommon(305));//무제한
        else
        {
            int count = CurrentData._StageLowData.DailyEntercount - CurrentData.DailyClearCount;
            if (count < 0)//혹시 모를 예외처리
                count = 0;

            sweepCount = string.Format(_LowDataMgr.instance.GetStringCommon(302), count);//무제한
        }

        SetTypeEnablePopup(POPUP_TYPE.SWEEP);
        SweepStageName.text = CurrentData.StageName;
		SweepStamina.text = string.Format(_LowDataMgr.instance.GetStringCommon(303), CurrentData.NeedEnerge.ToString() ); //ToString("#,##") );
        SweepCount.text = sweepCount;
        SweepCash.text = _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.PassCard).ToString(); // ToString("#,##");

        SweepSlotList.Clear();
        SweepPaging.NowCreate(SweepSlotList.Count);
    }

    /// <summary> 소탕 </summary>
    void OnClickBtnSweep(int count)
    {
        if(0 < CurrentData._StageLowData.DailyEntercount)
        {
            int clearCount = CurrentData._StageLowData.DailyEntercount - CurrentData.DailyClearCount;
            if (clearCount <= 0)//더이상 진행 불가능
            {
                UIMgr.instance.AddErrorPopup((int)Sw.ErrorCode.ER_StageStartS_Daily_Time_Error);
                return;
            }
        }

        NetworkClient.instance.SendPMsgStageSweepC(CurrentData.TableID, count, IsAutoFlop);
    }

    /// <summary> 소탕 캐쉬 소모해서 카드 뒤집기 체크 </summary>
    void OnToggleAutoFlop()
    {
        IsAutoFlop = SweepCardToggle.isChecked;
    }

    /// <summary> 스테이지 이동 </summary>
    void OnClickMoveState(int dir)
    {
        if (CurrentData == null)
            return;
        
        ChapterPanel.DataStage stage = Chapter.GetDataStage( (uint)(CurrentData.TableID + dir));
        if (stage != null)
            SetStagePopup(stage, false);
    }


    public TutorialSupport GetStartTuto()
    {
        return PopupObj[(int)POPUP_TYPE.STAGE_INFO].transform.FindChild("BtnStart").GetComponent<TutorialSupport>();
    }
    #endregion
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///****************************************************************************************
/// 업적은 빠짐 별도의 프리팹으로 존재함. 여기에는 메인, 일일퀘스트만 있을 예정임.(2017.02.06)
///****************************************************************************************

public class QuestPopup : UIBasePanel {
    enum ViewType {
        Main=0, Daily//, Achievement
    }
    
    public GameObject MainDetailPop;//메인 퀘스트의 상세 팝업
    public GameObject[] ViewObjs;//ViewType
    
    public Transform[] GridTf;//0 Main, 1Daily

    public UIButton BtnClose;

    public UITabGroup TabGroup;

    public GameObject CurMainQuestInfo;//현재퀘스트

    public GameObject Empty;    //비어있을때 나올것
    private Vector2[] RewardPos = new Vector2[3];
    private Vector2[] DetailRewardPos = new Vector2[3];

    private InvenItemSlotObject DetailItemSlot;//상세 팝업에서 보상 아이템으로 사용
    private List<QuestInfo> SubQuestList = new List<QuestInfo>();//메인에 위치할 퀘스트 리스트

    //private ViewType CurViewType;

    public override void Init()
    {
        base.Init();

        EventDelegate.Set(BtnClose.onClick, Close);

        TabGroup.Initialize(OnClickTab);
        Empty.SetActive(false);

        //일단끄고시작
        GridTf[0].gameObject.SetActive(false);


        //메인or서브 퀘스트 찾기.
        var enumerator = QuestManager.instance.QuestList.GetEnumerator();
        while (enumerator.MoveNext() )
        {
            Quest.QuestInfo questInfo = _LowDataMgr.instance.GetLowDataQuestData(enumerator.Current.Value.unTaskId);
            if (questInfo.type != 2)
                continue;

            SubQuestList.Add(enumerator.Current.Value);
        }

        DetailRewardPos[0] = MainDetailPop.transform.FindChild("Reward/gold").localPosition;
        DetailRewardPos[1] = MainDetailPop.transform.FindChild("Reward/power").localPosition;
        DetailRewardPos[2] = MainDetailPop.transform.FindChild("Reward/exp").localPosition;


        // 보상은 항상 있는게 아니기때문에 위치값을 저장해줘서 정렬해줌
        RewardPos[0] = CurMainQuestInfo.transform.FindChild("Reward/item").localPosition;
        RewardPos[1] = CurMainQuestInfo.transform.FindChild("Reward/gold").localPosition;
        RewardPos[2] = CurMainQuestInfo.transform.FindChild("Reward/exp").localPosition;


        //UIEventTrigger goldEtri = CurMainQuestInfo.transform.FindChild("Reward/gold").GetComponent<UIEventTrigger>();
        //EventDelegate.Set(goldEtri.onClick, delegate ()
        //{
            //UIMgr.OpenClickPopup(599000, CurMainQuestInfo.transform.FindChild("Reward/gold").transform.position);
        //});

        int subCount = SubQuestList.Count;//서브 셋팅
        for (int i=0; i < subCount; i++)
        {
            Transform tf = Instantiate(GridTf[0].GetChild(0) ) as Transform;
            tf.parent = GridTf[0];
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

            SetMainItem(tf, SubQuestList[i] );

            QuestInfo info = SubQuestList[i];
            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate() {
                //SetMainDetailPopup(info);
            } );
        }

        QuestInfo curQuestInfo = QuestManager.instance.GetCurrentQuestInfo();
        if(curQuestInfo != null )
        {
            GridTf[0].gameObject.SetActive(true);
            SetMainItem(CurMainQuestInfo.transform, curQuestInfo);//메인 셋팅
            //EventDelegate.Set(GridTf[0].GetChild(0).GetComponent<UIEventTrigger>().onClick, delegate () {
            //    SetMainDetailPopup(curQuestInfo);
            //});

            GridTf[0].GetComponent<UIGrid>().repositionNow = true;
            CurMainQuestInfo.SetActive(true);
        }
        else
        {
            StartCoroutine("DelayCurMainQuest");
            CurMainQuestInfo.SetActive(false);

        }

        GridTf[1].gameObject.SetActive(false);
        
        //메인퀘스트 상세 팝업
        UIEventTrigger uiTriDetailClose = MainDetailPop.transform.FindChild("BtnClose").GetComponent<UIEventTrigger>();
        UIEventTrigger uiTriDetailFog = MainDetailPop.transform.FindChild("fog").GetComponent<UIEventTrigger>();
        EventDelegate.Set(uiTriDetailClose.onClick, delegate () { MainDetailPop.SetActive(false); } );
        EventDelegate.Set(uiTriDetailFog.onClick, delegate () { MainDetailPop.SetActive(false); });

        GameObject slotGo = UIHelper.CreateInvenSlot(MainDetailPop.transform.FindChild("Reward/item") );
        DetailItemSlot = slotGo.GetComponent<InvenItemSlotObject>();
        MainDetailPop.SetActive(false);

        OnClickTab(0);
    }

    IEnumerator DelayCurMainQuest()
    {
        int whileCount = 5;
        while(0 < whileCount)
        {
            if (QuestManager.instance.GetCurrentQuestInfo() != null)
                break;

            yield return new WaitForSeconds(0.1f);
            --whileCount;
        }

        Debug.Log("Delay Current Main Quest ");

        QuestInfo curQuestInfo = QuestManager.instance.GetCurrentQuestInfo();
        if (curQuestInfo != null)
        {
            CurMainQuestInfo.SetActive(true);
            SetMainItem(GridTf[0].GetChild(0), curQuestInfo);//메인 셋팅
            EventDelegate.Set(GridTf[0].GetChild(0).GetComponent<UIEventTrigger>().onClick, delegate () {
                //SetMainDetailPopup(curQuestInfo);
            });
        }
        else
        {
            GridTf[0].gameObject.SetActive(false);
        }

        yield return null;
    }
    
    /// <summary> 메인퀘스트 셋팅 함수 </summary>
    void SetMainItem(Transform tf, QuestInfo questInfo)
    {
        Quest.QuestInfo lowData = _LowDataMgr.instance.GetLowDataQuestData(questInfo.unTaskId);

        tf.FindChild("Title/Txt_title").GetComponent<UILabel>().text = lowData.Title;   //제목
        tf.FindChild("Detail/Txt_info").GetComponent<UILabel>().text = lowData.Description;//내용
       

       int count = 0;
       
        //아이템있다
        if (0 < lowData.rewardItem)
        {
            List<GatchaReward.FixedRewardInfo> infoList = _LowDataMgr.instance.GetFixedRewardItemGroupList(lowData.rewardItem);

            byte myClass = 1;

            //장비만 직업이다르므로
            if (infoList[0].Type == 12)
            {
                if (NetData.instance.GetUserInfo().GetCharIdx() == 12000)
                    myClass = 2;
                else if (NetData.instance.GetUserInfo().GetCharIdx() == 13000)
                    myClass = 3;
            }
            else
                myClass = 99;

            //내 직업과맞는걸 넣어줘라
            GatchaReward.FixedRewardInfo info = new GatchaReward.FixedRewardInfo();
            for (int i = 0; i < infoList.Count; i++)
            {
                if (infoList[i].ClassType != myClass)
                    continue;
                info = infoList[i];
            }

            tf.FindChild("Reward/item").localPosition = RewardPos[count++];
            InvenItemSlotObject slot = null;
            if (0 < tf.FindChild("Reward/item").childCount)
            {
                slot = tf.FindChild("Reward/item").GetChild(0).GetComponent<InvenItemSlotObject>();
            }
            else
            {
                GameObject go = UIHelper.CreateInvenSlot(tf.FindChild("Reward/item"));
                go.collider.enabled = false;
                slot = go.GetComponent<InvenItemSlotObject>();
            }

            slot.SetLowDataItemSlot(info.ItemId == 0 ? info.Type : info.ItemId, info.ItemCount);
            UIEventTrigger etri =  tf.FindChild("Reward/item").GetComponent<UIEventTrigger>();
            EventDelegate.Set(etri.onClick, delegate ()
            {
                //UIMgr.OpenClickPopup(info.ItemId, tf.FindChild("Reward/item").transform.position);
                UIMgr.OpenDetailPopup(this, info.ItemId);
            });

            tf.FindChild("Reward/item").gameObject.SetActive(true);
        }
        else
        {
            tf.FindChild("Reward/item").gameObject.SetActive(false);
        }

        if (SetRewardTf(tf.FindChild("Reward/gold"), lowData.rewardGold,false))
            tf.FindChild("Reward/gold").localPosition = RewardPos[count++];


        if (SetRewardTf(tf.FindChild("Reward/exp"), lowData.rewardExp,true))
            tf.FindChild("Reward/exp").localPosition = RewardPos[count++];

      


        if (QuestManager.instance.GetCurrentQuestInfo().unTaskId != questInfo.unTaskId)
        {
            bool isEndQuest = false, isReward = false, isGoto = false;
            if (questInfo.unComplete == 1)//클리어
            {
                if (questInfo.unFetchBonus == 0)//미수령
                    isReward = true;
                else//수령
                    isEndQuest = true;
            }
            else if (lowData.value != 0)//미 클리어(진행중?)
            {
                isGoto = true;

                //tf.FindChild("slider/value").GetComponent<UILabel>().text = string.Format("{0} / {1}", questInfo.unTargetNum, lowData.value);
                //tf.FindChild("slider/filled").GetComponent<UISprite>().fillAmount = questInfo.unTargetNum / lowData.value;
            }

            //tf.FindChild("Clear").gameObject.SetActive(isEndQuest);
           tf.FindChild("BtnReward").gameObject.SetActive(isReward);
            //tf.FindChild("slider").gameObject.SetActive(isGoto);
            tf.FindChild("BtnGoto").gameObject.SetActive(isGoto);
        }
        else//진행중인 미션임. 이거는 버튼이 활성화 되지 않이함.
        {
            //tf.FindChild("Clear").gameObject.SetActive(false);
            tf.FindChild("BtnReward").gameObject.SetActive(false);
            //tf.FindChild("slider").gameObject.SetActive(false);
            tf.FindChild("BtnGoto").gameObject.SetActive(false);
        }
    }

    #region 디테일팝업은 현재사용x
    // void SetMainDetailPopup(QuestInfo info)
    // {
    //     Transform rewardTf = MainDetailPop.transform.FindChild("Reward");
    //     Quest.QuestInfo lowData = _LowDataMgr.instance.GetLowDataQuestData(info.unTaskId);

    //     int count = 0;
    //     if (SetRewardTf(rewardTf.FindChild("gold"), lowData.rewardGold,))
    //         rewardTf.FindChild("gold").localPosition = DetailRewardPos[count++];

    //     //if (SetRewardTf(rewardTf.FindChild("cash"), lowData.rewardCash))
    //     //    rewardTf.FindChild("cash").localPosition = DetailRewardPos[count++];

    //     if (SetRewardTf(rewardTf.FindChild("power"), lowData.rewardEnergy))
    //         rewardTf.FindChild("power").localPosition = DetailRewardPos[count++];

    //     if (SetRewardTf(rewardTf.FindChild("exp"), lowData.rewardExp))
    //         rewardTf.FindChild("exp").localPosition = DetailRewardPos[count++];

    //     if (0 < lowData.rewardItem)
    //     {
    //         GatchaReward.FixedRewardInfo rewardInfo = _LowDataMgr.instance.GetFixedRewardItem(lowData.rewardItem);
    //         DetailItemSlot.gameObject.SetActive(true);
    //         rewardTf.FindChild("item_value").gameObject.SetActive(true);

    //rewardTf.FindChild("item_value").GetComponent<UILabel>().text = string.Format("x {0}", rewardInfo.ItemCount.ToString()); //ToString("#,##"));

    //         DetailItemSlot.SetLowDataItemSlot(rewardInfo.ItemId == 0 ? rewardInfo.Type : rewardInfo.ItemId, 0, (key) => {
    //             UIMgr.OpenClickPopup((uint)key, DetailItemSlot.transform.position);
    //         });
    //     }
    //     else
    //     {
    //         rewardTf.FindChild("item_value").gameObject.SetActive(false);
    //         DetailItemSlot.gameObject.SetActive(false);
    //     }

    //     MainDetailPop.transform.FindChild("title").GetComponent<UILabel>().text = lowData.Title;
    //     UILabel desc = MainDetailPop.transform.FindChild("Desc/ScrollView/desc").GetComponent<UILabel>();
    //     desc.text = lowData.Description;

    //     UILabel target = MainDetailPop.transform.FindChild("target").GetComponent<UILabel>();
    //     target.text = lowData.Title;

    //     TempCoroutine.instance.FrameDelay(0.1f, ()=> {
    //         desc.ResizeCollider();
    //     } );

    //     MainDetailPop.SetActive(true);

    // }
    #endregion
    /// <summary> 메인용 아이템 셋팅 함수 </summary>
    bool SetRewardTf(Transform tf, uint value ,bool isExp)
    {
        if (0 < value)
        {
            tf.gameObject.SetActive(true);            
            if(isExp)
            {
                tf.FindChild("value").GetComponent<UILabel>().text = string.Format("{0} {1}",_LowDataMgr.instance.GetStringCommon(2), value.ToString()); // ToString("#,##");

            }
            else
            tf.FindChild("value").GetComponent<UILabel>().text = value.ToString(); // ToString("#,##");
            
            return true;
        }
        else
        {
            tf.gameObject.SetActive(false);
        }

        return false;
    }

    //정렬
    int Sort(NetData.Mission a, NetData.Mission b)
    {
        //아이디로 
        if (a.isClear() && b.isClear())
        {
            if (a._MissionInfo.MissionID < b._MissionInfo.MissionID)
                return 1;
            else
                return -1;
        }
        else if (a.isClear() && !b.isClear())
            return 1;
        else if ( !a.isClear() && b.isClear())
            return -1;

        //보상 받을 수 있는 목록을 최상단
        if (a.isComplet() && !b.isComplet())
            return -1;
        else if (!a.isComplet() && b.isComplet())
            return 1;
        else if (a.isComplet() && b.isComplet())
        {
            if (a._MissionInfo.MissionID < b._MissionInfo.MissionID)
                return 1;
            else
                return -1;
        }

        //진행중인 것을 상단
        if (0 < a._MissionValue && b._MissionValue <= 0)
            return -1;
        else if (a._MissionValue <= 0 && 0 < b._MissionValue)
            return 1;

        return 0;
    }

    /// <summary> 탭 클릭 뷰 바꿔준다. </summary>
    void OnClickTab(int arr)
    {
        switch((ViewType)arr)
        {
            case ViewType.Main:
                break;
            case ViewType.Daily:
                break;
        }

        int length = ViewObjs.Length;
        for (int i=0; i < length; i++)
        {
            if(i == arr)
                ViewObjs[i].SetActive(true);
            else
                ViewObjs[i].SetActive(false);
        }

        Empty.SetActive(!GridTf[arr].gameObject.activeSelf);

        //CurViewType = (ViewType)arr;

        //// 선택된 탭 Eff -> None, 선택안된탭 Eff -> Shadow로
        //for (int i = 0; i < TabGroup.TabList.Count; i++)
        //{
        //    TabGroup.TabList[i].gameObject.transform.FindChild("label").GetComponent<UILabel>().effectStyle = UILabel.Effect.Shadow;
        //    if(i == arr)
        //    {
        //        TabGroup.TabList[i].gameObject.transform.FindChild("label").GetComponent<UILabel>().effectStyle = UILabel.Effect.None;
        //    }
        //}
    }

    public override void Close()
    {
        if (TownState.TownActive)
        {
            UIBasePanel basePanel = UIMgr.GetTownBasePanel();
            (basePanel as TownPanel).SetJoystickActive(true);
        }

        StopCoroutine("DelayCurMainQuest");
        base.Close();

        UIMgr.OpenTown();
    }

    /// <summary> 해당 타입에 맞는 곳으로 이동을 한다. </summary>
    void OnClickGotoStage(MissionSubType subType)
    {
        SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
        int npcID = 0;
        switch (subType)
        {
            case MissionSubType.BOSS_RAID:
            case MissionSubType.SPECIAL:
                npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.SPECIAL_NPC);

                if (npcID != int.MaxValue)
                {
                    SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
                }
                break;
            case MissionSubType.DOGFIGHT:
                npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.FREEFIGHT_NPC);

                if (npcID != int.MaxValue)
                {
                    SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
                }
                break;
            case MissionSubType.TOWER:
                npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.TOWER_NPC);

                if (npcID != int.MaxValue)
                {
                    SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
                }
                break;
            case MissionSubType.ARENA:
                npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.ARENA_NPC);

                if (npcID != int.MaxValue)
                {
                    SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
                }
                break;

            case MissionSubType.COLOSSEUM:
                break;

            case MissionSubType.GUILD_OCCUPY:
                break;

            case MissionSubType.SINGLE:
            default:
                SceneManager.instance.GetState<TownState>().MyHero.RunToPotal();
                break;
        }

        Close();
    }
    
}

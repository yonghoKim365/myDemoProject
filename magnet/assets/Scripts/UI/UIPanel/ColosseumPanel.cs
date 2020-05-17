using UnityEngine;
using System.Collections.Generic;

public class ColosseumPanel : UIBasePanel {

    public Transform StageListTf;
    
    private List<InvenItemSlotObject> InvenSlotList = new List<InvenItemSlotObject>();

    private NetData.ColosseumData Info;
    //private bool IsStartUser;
    
    public override void Init()
    {
        base.Init();

        Info = NetData.instance.GetUserInfo()._ColosseumData;

        uint amount = 0;
        int itemData = NetData.instance.GetUserInfo().GetItemCountForItemId(SystemDefine.SweepItemLowDataID_2, (byte)eItemType.USE);
        int now = 0, max = 0;
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ColosseumCount, ref now, ref max);
        if (0 < itemData && now < itemData)//소탕권이 더 많다면 일일 클리어 횟수로 넣어준다.
            amount = (uint)(max-now);
        else//소탕권이 더 작다면 걍 넣어줌.
            amount = (uint)itemData;

        transform.FindChild("Btn_sweep/label").GetComponent<UILabel>().text = string.Format("{0}\n({1})", _LowDataMgr.instance.GetStringCommon(296), amount);//소탕\n(n)

        Transform parentTf = transform.FindChild("ClearItem");
        for(int i=0; i < parentTf.childCount; i++)
        {
            Transform slotTf = parentTf.FindChild(string.Format("{0}", i));
            if (slotTf == null)//끝임
                break;

            GameObject go = UIHelper.CreateInvenSlot(slotTf);
            InvenItemSlotObject slot = go.GetComponent<InvenItemSlotObject>();
            //slot.SetBackGround("Bod_Inbod06");
            InvenSlotList.Add( slot );
        }

        EventDelegate.Set(transform.FindChild("Btn_sweep").GetComponent<UIEventTrigger>().onClick, delegate () {
            //uiMgr.AddPopup(18, null, null, null);
            if (Info.StageId < Info.SelectId)
            {
                return;
            }

            NetworkClient.instance.SendPMsgColosseumSweepC(Info.SelectId, 1);
        });

        EventDelegate.Set(transform.FindChild("Btn_go").GetComponent<UIEventTrigger>().onClick, delegate() {

            if (Info.IsPossible)
            {
                DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo(Info.SelectId );
                if (NetData.instance.GetUserInfo()._TotalAttack < coloInfo.FightingPower)
                {
                    string msg = string.Format(_LowDataMgr.instance.GetStringCommon(729), coloInfo.FightingPower);
                    SceneManager.instance.SetNoticePanel(NoticeType.Message, 0, msg);
                }
                else
                    NetworkClient.instance.SendPMsgColosseumCreateRoomC(Info.SelectId);
            }
            else
                uiMgr.AddErrorPopup((int)Sw.ErrorCode.ER_TowerBattleStartS_Pre_Stage_Error);//uiMgr.AddPopup(2003, null, null, null);
        } );

        List<DungeonTable.ColosseumInfo> list = _LowDataMgr.instance.GetLowDataColosseumList();
        uint startId = 0;
        if (list[list.Count - 1].StageId < Info.StageId )//모든 스테이지 클리어
            startId = list[list.Count-1].StageId;
        else
            startId = Info.StageId;

        int count = list.Count;
        for (int i = 0; i < count; i++)
        {
            if (StageListTf.childCount <= i)//문제가 있다.
            {
                continue;
            }

            DungeonTable.ColosseumInfo colInfo = list[i];
            Transform tf = StageListTf.FindChild(string.Format("slot_0{0}", i + 1));
            if (tf == null)
            {
                Debug.LogError(string.Format("not found slot_0{0} error ", i + 1));
                continue;
            }

            UILabel stageName = tf.FindChild("stage_name").GetComponent<UILabel>();
            stageName.text = _LowDataMgr.instance.GetStringStageData(colInfo.String);
            tf.FindChild("sweep").gameObject.SetActive(false);//소탕 표기 일딴 꺼놓는다

            if (colInfo.StageId <= startId && colInfo.LimitLevel <= NetData.instance.UserLevel)//입장 조건 만족
            {
                tf.GetComponent<UITexture>().color = Color.white;
                stageName.color = Color.white;
            }
            else
            {
                tf.GetComponent<UITexture>().color = Color.gray;
                stageName.color = Color.gray;
            }

            uint colId = colInfo.StageId;
            EventDelegate.Set(tf.GetComponent<UIEventTrigger>().onClick, delegate () {
                OnClickStage(colId);
            });
        }
        
        OnClickStage(startId);
    }

    public override void LateInit()
    {
        base.LateInit();
        if (parameters.Length <= 0)
            return;

        uint inviteDungeonId = (uint)parameters[0];
        if(0 < inviteDungeonId)
        {
            OnReadyPopup(inviteDungeonId);
        }
        //else
        //{
            //CheckOpenTutorial(OpenTutorialType.COLOSSEUM);
        //}
    }

    public override void Close()
    {
        base.Close();
        //if (uiMgr.ReturnPanel.Equals("ActivityPanel"))
        //    UIMgr.OpenActivityPanel();
        //else
            UIMgr.OpenDungeonPanel();
    }

    public override void OnCloseReadyPopup()
    {
        base.OnCloseReadyPopup();
    }

    void OnClickStage(uint id)
    {
        Info.SelectId = id;
        DungeonTable.ColosseumInfo coloInfo = _LowDataMgr.instance.GetLowDataColosseumInfo(id);
        if (coloInfo == null)
            return;

        if (Info.StageId < id)//IsStartUser && 
        {
            transform.FindChild("Btn_sweep").collider.enabled = false;
            transform.FindChild("Btn_sweep").GetComponent<UISprite>().color = Color.gray;
        }
        else
        {
            bool isClear = id < Info.StageId;//클리어
            if (isClear)
            {
                transform.FindChild("Btn_sweep").collider.enabled = true;
                transform.FindChild("Btn_sweep").GetComponent<UISprite>().color = Color.white;
            }
            else
            {
                transform.FindChild("Btn_sweep").collider.enabled = false;
                transform.FindChild("Btn_sweep").GetComponent<UISprite>().color = Color.gray;
            }
        }

        List<string> getItemList = coloInfo.rewardItemId.list;

        if (InvenSlotList.Count < getItemList.Count)
        {
            Debug.LogWarning("Over RewardItem Slot Count " + getItemList.Count);
        }

        for (int i=0; i < InvenSlotList.Count; i++)
        {
            if (getItemList.Count <= i)
            {
                InvenSlotList[i].gameObject.SetActive(false);
                continue;
            }
            
            uint itemId = 0;
            if (!uint.TryParse(getItemList[i], out itemId))
            {
                continue;
            }

            InvenSlotList[i].gameObject.SetActive(true);
            int arr = i;
            InvenSlotList[i].SetLowDataItemSlot(itemId, 0, (ulong key) => {
                UIMgr.OpenDetailPopup(this, (uint)key, 4);
                //UIMgr.OpenClickPopup((uint)key, InvenSlotList[arr].transform.position );
            } );
        }
    }

    public override void GotoInGame()
    {
        //uiMgr.AddPopup(18, null, null, null);
        //base.GotoInGame();
        NetworkClient.instance.SendPMsgColosseumStartC(Info.SelectId);
        //uiMgr.AddPopup(18, null, null, null);
    }
    
    #region 콜로세움 프로토콜 응답
    
    /// <summary> 방 생성 응답 </summary>
    public void OnCreateRoom(uint dungeonId, long roomId)
    {
        if (Info.SelectId != dungeonId)
            Debug.LogError("Select dungeon id != receive dungeon id error");

        int now = 0, max = 0;
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ColosseumCount, ref now, ref max);
        UIMgr.OpenReadyPopup(GAME_MODE.COLOSSEUM, this, now, max, dungeonId);
    }

    /// <summary> 레디 팝업으로 넘어간다 이경우에는 초대에 응했을 경우임. </summary>
    public void OnReadyPopup(uint dungeonId)
    {
        int now = 0, max = 0;
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ColosseumCount, ref now, ref max);
        UIMgr.OpenReadyPopup(GAME_MODE.COLOSSEUM, this, now, max, dungeonId);
    }

    public void OnStartGame(uint dungeonId, long roomId)
    {
        //DungeonTable.ColosseumInfo coloLowData = _LowDataMgr.instance.GetLowDataColosseumInfo(dungeonId);

        //SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.RAID, () =>
        //{
        //    //이상태에서의 데이터를 저장
        //    NetData.instance.MakePlayerSyncData(true);

        //    SceneManager.instance.ActionEvent(_ACTION.PLAY_FREEFIGHT);
        //});

        //Debug.Log("<color=green>Colosseum Log : OnStartGame</color>");
        uiMgr.CloseReadyPopup();
        base.GotoInGame();
    }

    public void OnSweep()
    {
        int now = 0, max = 0;
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.ColosseumCount, ref now, ref max);
        ++now;
        NetData.instance.GetUserInfo().SetCompleteCount(EtcID.ColosseumCount, now, max);

        uint amount = 0;
        int itemData = NetData.instance.GetUserInfo().GetItemCountForItemId(SystemDefine.SweepItemLowDataID_2, (byte)eItemType.USE);
        if (0 < itemData && now < itemData)//소탕권이 더 많다면 일일 클리어 횟수로 넣어준다
            amount = (uint)(max - now);
        else//소탕권이 더 작다면 걍 넣어줌
            amount = (uint)itemData;

        transform.FindChild("Btn_sweep/label").GetComponent<UILabel>().text = string.Format("{0}\n({1})", _LowDataMgr.instance.GetStringCommon(296), amount);//소탕\n(n)
    }

    #endregion
}

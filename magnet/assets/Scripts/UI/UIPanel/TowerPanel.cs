using UnityEngine;
using System.Collections.Generic;

public class TowerPanel : UIBasePanel {

    public GameObject FloorPrefab;
    public GameObject RankingPrefab;
    public GameObject FirstRewardClear;

    public Transform ArrowEff;
    public Transform FloorGrid;//층수 그리드

    public UILabel PlayerRanking;
    public UILabel CurrentFloor;

    public UIGrid RankGrid;//랭킹 어미

    public UIScrollView FloorScroll;
    public UIScrollView ItemScroll;
    public Color32[] SelectColor;

    public int FloorTreeCount = 4;//해당 값 이후 생성
    public int FloorTreeStart = 0;//해당 값 부터 생성
    public int LastClearFloor;//최종 클리어
    public float MinPosY;
    public float MaxPosY;

    private Transform SelectEff;
    private List<DungeonTable.TowerInfo> TowerLowDataList;
    private List<InvenItemSlotObject> InvenSlotList = new List<InvenItemSlotObject>();
    private InvenItemSlotObject FirstReward;

    private string FloorText;//_LowDataMgr.instance.GetStringCommon(189) 이것이 자주 쓰일거 같아서 변수로 뺌.
    private int CurrentLowDataArr;//데이터 배열값 현재 선택된 층수의 배열값임

    public override void Init()
    {
        base.Init();
        //랭킹 슬롯 생성.
        for (int i = 1; i < SystemDefine.MaxTowerRanking; i++)//9개만 생성한다
        {
            GameObject slotGo = Instantiate(RankingPrefab) as GameObject;
            Transform slotTf = slotGo.transform;
            slotTf.parent = RankGrid.transform;
            slotTf.localPosition = Vector3.zero;
            slotTf.localScale = Vector3.one;
            slotGo.SetActive(false);
        }

        Transform rewardTf = transform.FindChild("Info/Reward/Grid");
        if (rewardTf != null)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject slotGo = UIHelper.CreateInvenSlot(rewardTf );
                InvenItemSlotObject invenSlot = slotGo.GetComponent<InvenItemSlotObject>();
                invenSlot.SetBackGround("Bod_Inbod11");
                invenSlot.EmptySlot();

                InvenSlotList.Add(invenSlot);
            }
        }
        
        FirstRewardClear.SetActive(false);
        FloorGrid.parent.gameObject.SetActive(false);
        RankGrid.repositionNow = true;
        TowerLowDataList = _LowDataMgr.instance.GetLowDataTowerList();
        FloorText = _LowDataMgr.instance.GetStringCommon(189);
        int floorCount = TowerLowDataList.Count;
        for (int i = 0; i < floorCount; i++)
        {
            GameObject go = Instantiate(FloorPrefab) as GameObject;
            Transform tf = go.transform;
            tf.parent = FloorGrid.transform;
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
        }

        GameObject firstGo = UIHelper.CreateInvenSlot( transform.FindChild("Info/FirstReward") );
        FirstReward = firstGo.GetComponent<InvenItemSlotObject>();

        UIButton uiBtn = transform.FindChild("Info/BtnEntry").GetComponent<UIButton>();
        EventDelegate.Set(uiBtn.onClick, OnClickEntry);

        //상점 바로가기
        EventDelegate.Set(transform.FindChild("Info/BtnShop").GetComponent<UIButton>().onClick, OnclickShowShopPanel);

        SelectEff = UIHelper.CreateEffectInGame(transform, "Fx_UI_tower_position_01").transform;
        TempCoroutine.instance.FrameDelay(0.1f, () =>
        {
            Material effM_1 = SelectEff.GetChild(0).FindChild("fix_circle_1").renderer.material;
            Material effM_2 = SelectEff.GetChild(0).FindChild("fix_circle_2").renderer.material;
            effM_1.SetColor("_TintColor", SelectColor[0]);
            effM_2.SetColor("_TintColor", SelectColor[1]);
        });

        SelectEff.GetChild(0).FindChild("_particle_set").gameObject.SetActive(false);
        SelectEff.GetChild(0).FindChild("Ray_1").gameObject.SetActive(false);
        SelectEff.GetChild(0).FindChild("Ray_2").gameObject.SetActive(false);

        MinPosY = FloorScroll.transform.localPosition.y;
    }

    void OnclickShowShopPanel()
    {
        //base.Close();
        base.Hide();
        UIMgr.OpenShopPanel(this); 
    }
    
    public override void LateInit()
    {
        base.LateInit();

        if (mStarted)
            return;

        LastClearFloor = NetData.instance.GetUserInfo().TowerFloor + 1;
        int treeCount = 0;
        int floorCount = TowerLowDataList.Count;
        for (int i = 0; i < floorCount; i++)
        {
            Transform tf = FloorGrid.GetChild(i);

            Vector2 floorLbPos;
            if (i % 2 == 0)
            {
                tf.GetComponent<UISprite>().flip = UIBasicSprite.Flip.Nothing;
                floorLbPos = new Vector2(26, 20);
            }
            else
            {
                tf.GetComponent<UISprite>().flip = UIBasicSprite.Flip.Horizontally;
                floorLbPos = new Vector2(-100, 20);
            }

            if (i % 2 == 0)
                tf.FindChild("eff_root").localPosition = new Vector2(-123, 12);
            else
                tf.FindChild("eff_root").localPosition = new Vector2(123, 12);

            if (LastClearFloor - 1 == i)
            {
                ArrowEff.parent = tf.FindChild("eff_root");
                ArrowEff.localPosition = Vector3.zero;
                ArrowEff.localScale = Vector3.one;

                UIHelper.CreateEffectInGame(tf.FindChild("eff_root"), "Fx_UI_tower_position_01");
            }

            if (LastClearFloor - 1 < i)//진행 불가
                tf.FindChild("floor/Box").GetComponent<UISprite>().spriteName = "Img_JewelBox01_off";
            else//진행 가능
                tf.FindChild("floor/Box").GetComponent<UISprite>().spriteName = "Img_JewelBox01_on";

            if (LastClearFloor - 1 <= i)
                tf.FindChild("Top").GetComponent<UISprite>().spriteName = "Img_pve_tower_off";
            else
                tf.FindChild("Top").GetComponent<UISprite>().spriteName = "Img_pve_tower_on";

            bool isTree = false;
            if (i <= 50 && FloorTreeStart <= i)
            {
                ++treeCount;
                if (FloorTreeStart == i)
                {
                    isTree = true;
                    treeCount = 0;
                }

                if (FloorTreeCount <= treeCount)
                {
                    isTree = true;
                    treeCount = 0;
                }
            }

            tf.FindChild("Top/tree").gameObject.SetActive(isTree);

            int arr = i;
            UIEventTrigger uiTri = tf.GetComponent<UIEventTrigger>();
            UILabel floorLbl = tf.FindChild("floor").GetComponent<UILabel>();

            tf.FindChild("floor").localPosition = floorLbPos;
            EventDelegate.Set(uiTri.onClick, delegate () {
                OnClickFloor(arr);
                SoundManager.instance.PlaySfxSound(eUISfx.UI_dark_tower_choice, false);
            });
            floorLbl.text = string.Format(FloorText, i + 1);
        }

        //if (TowerLowDataList.Count <= LastClearFloor)//모든 층 클리어.
        //    LastClearFloor = TowerLowDataList.Count;//99층

        FloorGrid.parent.gameObject.SetActive(true);
        UIGrid grid = FloorGrid.GetComponent<UIGrid>();
        float startY = 0;
        if (93 < LastClearFloor)//층 이상이면 무시
            startY = -grid.cellHeight * 93;//FloorMaxHeight;
        else if (1 < LastClearFloor)
        {
            startY = ((LastClearFloor - 1) * -grid.cellHeight);// + -FloorGrid.cellHeight;
        }

        Destroy(FloorPrefab);

        grid.repositionNow = true;
        FloorScroll.MoveRelative(new Vector3(0, -startY, 0));

        int startFloor = 0;
        if (0 < parameters.Length && 0 < (uint)parameters[0])//지정된 층으로 이동.
        {
            DungeonTable.TowerInfo towerInfo = _LowDataMgr.instance.GetLowDataTower((uint)parameters[0]);
            startFloor = (int)towerInfo.level - 1;
        }
        else
        {
            if (TowerLowDataList.Count <= LastClearFloor)//모든 층 클리어.
                startFloor = TowerLowDataList.Count - 1;//99층
            else
                startFloor = LastClearFloor - 1;
        }

        OnClickFloor(startFloor);//(int)LastClearFloor);//이렇게 넣으면 이 다음층으로 선택됨.

        if (SceneManager.instance.UiOpenType == UI_OPEN_TYPE.NEXT_ZONE)
            OnClickEntry();

        SceneManager.instance.UiOpenType = UI_OPEN_TYPE.NONE;

        if (SceneManager.instance.CurTutorial == TutorialType.TOWER)
        {
            if(OnSubTutorial() )
                uiMgr.TopMenu.TutoSupport.TutoType = TutorialType.NONE;
        }
    }

    public override void Close()
    {
        base.Close();

        //if (uiMgr.ReturnPanel.Equals("ActivityPanel"))
        //    UIMgr.OpenActivityPanel();
        //else
            UIMgr.OpenDungeonPanel();
    }

    /// <summary> 층을 클릭함. </summary>
    void OnClickFloor(int arr)
    {
        if (TowerLowDataList.Count <= arr)
            return;

        CurrentLowDataArr = arr;
        DungeonTable.TowerInfo towerLowData = TowerLowDataList[arr];
        CurrentFloor.text = string.Format(FloorText, towerLowData.level);

        //최초 보상 셋팅
        GatchaReward.FixedRewardInfo fixedReward = _LowDataMgr.instance.GetFixedRewardItem(towerLowData.FirstReward);
        FirstReward.SetLowDataItemSlot(fixedReward.ItemId, 0, (key) => {
            UIMgr.OpenDetailPopup(this, (uint)key);
        });

        //보상아이템 셋팅
        List<string> rewardList = towerLowData.rewarditemId.list;
        int listCount = InvenSlotList.Count;

        uint myClass = NetData.instance.GetUserInfo().GetCharIdx();
        switch (myClass)
        {
            case 11000:
                myClass = 1;
                break;
            case 12000:
                myClass = 2;
                break;
            case 13000:
                myClass = 3;
                break;
        }

        for (int i = 0; i < listCount; i++)
        {
            if (rewardList.Count <= i)
            {
                InvenSlotList[i].gameObject.SetActive(false);
                continue;
            }

            uint itemId = uint.Parse(rewardList[i]);

            InvenSlotList[i].gameObject.SetActive(true);
            Transform slotTf = InvenSlotList[i].transform;
            InvenSlotList[i].SetLowDataItemSlot(itemId, 0, (lowDataId) => {
                UIMgr.OpenDetailPopup(this, (uint)lowDataId);
            });
        }

        ItemScroll.enabled = true;
        ItemScroll.ResetPosition();

        if (rewardList.Count <= 4)
            ItemScroll.enabled = false;
        

        if (towerLowData.level < LastClearFloor)//클리어함
            FirstRewardClear.SetActive(true);
        else//미클리어
            FirstRewardClear.SetActive(false);

        if (LastClearFloor - 1 == arr)
            SelectEff.localPosition = new Vector2(-1000, 1000);
        else
        {
            Transform effTf = FloorGrid.transform.GetChild(arr).FindChild("eff_root");
            SelectEff.parent = effTf;
            SelectEff.localPosition = Vector3.zero;
            SelectEff.localScale = Vector3.one;
        }
        
        //랭킹
        NetworkClient.instance.SendPMsgTowerRankQueryC(towerLowData.level);
    }

    /// <summary> 입장 </summary>
    void OnClickEntry()
    {
        if (TowerLowDataList.Count <= CurrentLowDataArr)//뭔가 잘못되었다.
            return;

        DungeonTable.TowerInfo towerLowData = TowerLowDataList[CurrentLowDataArr];
        if (LastClearFloor+1 <= towerLowData.level)//입장 불가능한 지역.
        {
            uiMgr.AddErrorPopup((int)Sw.ErrorCode.ER_TowerBattleStartS_Pre_Stage_Error);
            return;
        }

        int now = 0, max = 0;
        NetData.instance.GetUserInfo().GetCompleteCount(EtcID.TowerCount, ref now, ref max);

        transform.FindChild("Info").gameObject.SetActive(false);
        transform.FindChild("FloorList").gameObject.SetActive(false);
        UIMgr.OpenReadyPopup(GAME_MODE.TOWER, this, now, max);
    }

    public override void GotoInGame()
    {
        uint stageId = TowerLowDataList[CurrentLowDataArr].StageIndex;
        NetworkClient.instance.SendPMsgTowerBattleStartC(stageId);
    }

    public override void OnCloseReadyPopup()
    {
        base.OnCloseReadyPopup();
        transform.FindChild("Info").gameObject.SetActive(true);
        transform.FindChild("FloorList").gameObject.SetActive(true);
    }
    
    void LateUpdate()
    {
        Vector2 floorPos = FloorScroll.transform.localPosition;
        if (MinPosY < floorPos.y)
        {
            floorPos.y = MinPosY;
            FloorScroll.transform.localPosition = floorPos;
            FloorScroll.panel.clipOffset = Vector2.zero;
        }
        else if(floorPos.y < MaxPosY)
        {
            floorPos.y = MaxPosY;
            FloorScroll.transform.localPosition = floorPos;
            float offY = Mathf.Abs(MaxPosY - MinPosY);
            FloorScroll.panel.clipOffset = new Vector2(0, offY);
        }
    }
  

    #region 서버 응답
    /// <summary> 랭킹 정보 </summary>
    public void OnPMsgTowerRankQuerySHandler(List<NetData.TowerRankData> rankList)
    {
        ulong playerCharIdx = NetData.instance.GetUserInfo().GetCharUUID();
        int playerRank = 0;
        int childCount = RankGrid.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform slotTf = RankGrid.transform.GetChild(i);
            if (rankList.Count <= i)
            {
                slotTf.gameObject.SetActive(false);
                continue;
            }

            slotTf.gameObject.SetActive(true);
            UILabel nickName = slotTf.FindChild("name").GetComponent<UILabel>();
            UILabel clearTime = slotTf.FindChild("clearTime").GetComponent<UILabel>();
            UILabel rankNum = slotTf.FindChild("num").GetComponent<UILabel>();
            UISprite face = slotTf.FindChild("Face/icon").GetComponent<UISprite>();

            NetData.TowerRankData data = rankList[i];
            float m = Mathf.Floor(data.Seconds / 60);
            float s = Mathf.Floor(data.Seconds % 60);

            rankNum.text = string.Format("{0}", data.RankNumber);
            nickName.text = data.Name;
            clearTime.text = string.Format(_LowDataMgr.instance.GetStringCommon(214), m.ToString("00"), s.ToString("00"));

            Character.CharacterInfo charLowDatga = _LowDataMgr.instance.GetCharcterData(data.CharLowData);
            face.spriteName = charLowDatga.PortraitId;

            if (playerCharIdx.CompareTo(data.RoleId) == 0)//나임
            {
                playerRank = data.RankNumber;
            }
        }

        if (0 < playerRank)//순위권
            PlayerRanking.text = string.Format(_LowDataMgr.instance.GetStringCommon(192), playerRank);
        else//순위권 밖
            PlayerRanking.text = _LowDataMgr.instance.GetStringCommon(193);

        RankGrid.repositionNow = true;
    }

    /// <summary> 시작 응답 </summary>
    public void OnPMsgTowerBattleStartSHandler(int dungeonId)
    {
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.TOWER, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(true);

            //마지막으로 선택한 스테이지 아이디 저장
            TowerGameState.lastSelectStageId = (uint)dungeonId;
            SceneManager.instance.ActionEvent(_ACTION.PLAY_TOWER);

            //base.GotoInGame();
        });

        uiMgr.CloseReadyPopup();
        base.Hide();
    }

    public override void NetworkData(params object[] proto)
    {
        base.NetworkData(proto);
        switch((Sw.MSG_DEFINE)proto[0] )
        {
            case Sw.MSG_DEFINE._MSG_TOWER_USE_TIME_QUERY_S:
                int lv = (int)proto[1];
                int clearTime = (int)proto[2];


                break;
        }
    }
    #endregion 서버응답 끝
}

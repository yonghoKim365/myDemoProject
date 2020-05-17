using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChapterPanel : UIBasePanel {

    /// <summary>
    /// 스테이지 타입 일반, 중간보스, 장보스 가 있다.
    /// </summary>
    enum STAGE_TYPE
    {
        NORMAL,//일반
        MIDDLE_BOSS,//중간 보스
        CHAPTER_BOSS,// 장 보스
    }

    /// <summary>
    /// 버튼 배열 값
    /// </summary>
    enum BUTTON_TYPE
    {
        NEXT,//다음챕터
        PREV,//이전 챕터
    }

    /// <summary>
    /// 챕터 가라 테이터(Table)
    /// </summary>
    class DataChapter
    {
        public bool IsHard;//이거 어려움
        public int number;//챕터 값
        public int GetClearGrade;
        public int CurRewardBoxID;//현재 몇번째 박스 보상 받을 수 있는지

        public string name;//이름

        public List<DungeonTable.ChapterRewardInfo> StarReward;
        public List<DataStage> stageList;//보유한 스테이지

        public DataChapter(int number, bool isHard, NetData.StageStarRewardData rewardData, List<DataStage> stageList)//int totalClearGrade, 
        {
            IsHard = isHard;
            this.name = _LowDataMgr.instance.GetStringStageData(stageList[0]._StageLowData.ChapName);
            GetClearGrade = rewardData.Value;

            StarReward = _LowDataMgr.instance.GetLowDataChapterReward((byte)number, (byte)(IsHard ? 2 : 1) );
            CurRewardBoxID = rewardData.BoxID;

            this.number = number;
            this.stageList = stageList;
        }

    }

    /// <summary>
    /// 스테이지의 가라 데이터(Table)
    /// </summary>
    public class DataStage
    {
        public DungeonTable.StageInfo _StageLowData;
        public List<NetData.StageClearData> QuestList = new List<NetData.StageClearData>();

        public string[] AppearMonster;
        public uint[] DropItemId;
        public int DailyClearCount;

        public byte TotalClearGrade {
            get {
                return (byte)(ClearInfo[0] + ClearInfo[1] + ClearInfo[2]);
            }
        }

        public byte[] ClearInfo;

        public short State;//-1은 가지 못하는 지역, 0은 플레이 가능지역, 1은 클리어 지역

        public bool InitStageData(uint tableID, byte[] clearInfo, short state, int dailyClearCount)
        {
            ClearInfo = clearInfo;
            _StageLowData = _LowDataMgr.instance.GetStageInfo(tableID);
            State = state;
            DailyClearCount = dailyClearCount;

            if (_StageLowData == null)//없으면 문제인데..
                return false;


            QuestList.Add(new NetData.StageClearData((ClearQuestType)_StageLowData.ClearType1, _StageLowData.ClearValue1));
            QuestList.Add(new NetData.StageClearData((ClearQuestType)_StageLowData.ClearType2, _StageLowData.ClearValue2));
            QuestList.Add(new NetData.StageClearData((ClearQuestType)_StageLowData.ClearType3, _StageLowData.ClearValue3));

            int monsterCount = _StageLowData.regenMonster.Count;
            AppearMonster = new string[monsterCount];

            for (int i = 0; i < monsterCount; i++)
            {
                uint m_id = uint.Parse(_StageLowData.regenMonster[i]);
                Mob.MobInfo mLowData = _LowDataMgr.instance.GetMonsterInfo(m_id);
                if (mLowData == null)
                {
                    Debug.Log("MobTable id error " + m_id);
                    continue;
                }

                AppearMonster[i] = mLowData.PortraitId;
            }

            //아이템은 나의 클래스에 맞춰서 표시해준다.
            //int myClass = UIHelper.GetClassType(NetData.instance.GetUserInfo().GetCharIdx() );

            int addCount = _StageLowData.type == 2 ? 2 : 1;
            DropItemId = new uint[_StageLowData.rewardItemId.Count+addCount];
            // 최초
            GatchaReward.FixedRewardInfo firstInfo = _LowDataMgr.instance.GetFixedRewardItem(_StageLowData.FirstReward);
            if (firstInfo != null)
                DropItemId[0] = firstInfo.ItemId == 0 ? firstInfo.Type :  firstInfo.ItemId;

            if (_StageLowData.type == 2)//어려움만.
            {
                GatchaReward.FixedRewardInfo basicInfo = _LowDataMgr.instance.GetFixedRewardItem(_StageLowData.FixedReward);
                if(basicInfo != null)
                    DropItemId[1] = basicInfo.ItemId==0? basicInfo.Type : basicInfo.ItemId;
            }
            
            for (int i=addCount; i < DropItemId.Length; i++)
            {
                uint i_id = uint.Parse(_StageLowData.rewardItemId[i-addCount]);  //ui유아이표시 
                DropItemId[i] = i_id;
            }

            return true;
        }



        //대표 출현 몬스터 아이콘 
        public string GetMonsterIcon(int arr)
        {
            if (AppearMonster.Length <= arr)
                return null;

            return AppearMonster[arr];
        }

        //대표 드랍 아이템 아이콘
        public uint GetDropItemID(int arr)
        {
            if (DropItemId.Length <= arr)
                return 0;

            return DropItemId[arr];
        }

        // 스테이지 입장 필요 에너지
        public byte NeedEnerge
        {
            get {
                byte e = _StageLowData.useValue;

                return e;
            }
        }

        //테이블 아이디(게임시작시 저장용)
        public uint TableID
        {
            get {
                return _StageLowData.StageId;
            }
        }

        //스테이지 이름
        public string StageName
        {
            get {
                if (_StageLowData == null)
                    return null;

                return _LowDataMgr.instance.GetStringStageData(_StageLowData.String); // string.Format("{0}-{1}"_StageLowData. );
            }
        }
        //던전 스토리
        public string Story
        {
            get
            {
                return string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(59), _LowDataMgr.instance.GetStringStageData(_StageLowData.AreaExplain));
            }
        }

        public JsonCustomData JSONDropItem
        {
            get
            {
                return _StageLowData.rewardItemId;
            }
        }

    }

    public GameObject InvenItemPrefab;

    public Transform ChapterParent;//챕터들 들어갈 어미 객체
    public Transform HardChapterParent;//하드챕터들 들어갈 어미 객체
    public Transform ChapterClear;
    public Transform[] ChapRewardTfs;//챕터 보상 슬롯들
    //public GameObject GradeListPop;//별 보상 리스트 가티고 있는 객체
    
    public UIButton[] Btns;//BUTTON_TYPE에 맞춰서 버튼이 들어있다.

    public UIEventTrigger BtnNormal;  //일반스테이지버튼
    public UIEventTrigger BtnHard;    //하드스테이지버튼

    public Transform NameRoot;  
    public UILabel ChapterNameLabel;//챕터의 이름을 표기해줄 라벨.
    public UILabel HardChapterNameLabel;    
    public UILabel GetGradeCountLabel;//획득 별 개수 표기


    public GameObject[] BoxEffRoot; //박스이펙트         
    public GameObject[] BoxClickEffRoot; //박스클릭이펙트  
    public Transform[] RewardSlotRoot; 

    public ChapterPopup StagePopup;//Popup 객체에 있는 스크립트 Chapter에서 사용하는 팝업 관리함.

    private DataChapter CurChapter;//현제 보고 있는 챕터 
    private List<DataChapter> ChapterList;//노말 챕터들
    private List<DataChapter> HardChapterList;//어려움 챕터들

    private InvenItemSlotObject[] InvenItemRewardSlot; 
    
    private bool IsLevelHard;
    private bool IsStarted;

    private int CurChapterNum;//진행 단계 챕터 1~ MaxChapterCount 까지

    private NetData._UserInfo CharInven;//한 캐릭터가 들고있는 인벤토리 정보
    public int PreAttack;  //스탯업 이펙트를위해 미리 계산해놈

    private UIBasePanel reOpenPanel;    
    public override void Init()
    {
		SceneManager.instance.sw.Reset ();
		SceneManager.instance.sw.Start ();
		SceneManager.instance.showStopWatchTimer ("ChapterPanel, Init() start");

        ChapterList = new List<DataChapter>();
        HardChapterList = new List<DataChapter>();

        base.Init();
        SetDataTable(ChapterList, 0);
        SetDataTable(HardChapterList, 10000);

        InvenItemRewardSlot = new InvenItemSlotObject[3];
        for (int i=0;i< RewardSlotRoot.Length;i++)
        {
            GameObject slotGo = Instantiate(InvenItemPrefab) as GameObject;
            Transform tf = slotGo.transform;
            tf.parent = RewardSlotRoot[i];
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

            InvenItemRewardSlot[i] = slotGo.GetComponent<InvenItemSlotObject>();
            InvenItemRewardSlot[i].EmptySlot();

        }

        UIHelper.CreateEffectInGame(BtnHard.transform.FindChild("On/eff").transform, "Fx_UI_HardMode_01");

        //이벤트 버튼 핸들러
        EventButton();
        StagePopup.PopupInit(this);
        CharInven = NetData.instance.GetUserInfo();
        PreAttack = (int)CharInven.RefreshTotalAttackPoint();
        
        List<NetData.StageStarRewardData> rewardList = CharInven.StageStarReward;
        for (int i = 0; i < ChapterList.Count; i++)// 갱신시켜주기
        {
            for (int j = 0; j < rewardList.Count; j++)
            {
                if (rewardList[j].ChapterID != ChapterList[i].number)
                    continue;
                if (rewardList[j].StageType != 1)
                    continue;

                ChapterList[i].GetClearGrade = rewardList[j].Value;
                ChapterList[i].CurRewardBoxID = rewardList[j].BoxID;
            }
        }

        List<NetData.StageStarRewardData> hardRewardList = CharInven.HardStageStarReward;
        for (int i = 0; i < HardChapterList.Count; i++)// 갱신시켜주기
        {
            for (int j = 0; j < hardRewardList.Count; j++)
            {
                if (hardRewardList[j].ChapterID != HardChapterList[i].number)
                    continue;
                if (hardRewardList[j].StageType != 2)
                    continue;

                HardChapterList[i].GetClearGrade = hardRewardList[j].Value;
                HardChapterList[i].CurRewardBoxID = hardRewardList[j].BoxID;
            }
        }
        
        for(int i=0; i < BoxClickEffRoot.Length; i++)
        {
            UIHelper.CreateEffectInGame(BoxClickEffRoot[i].transform, "Fx_UI_chapter_box_open");
            BoxClickEffRoot[i].SetActive(false);
        }

		SceneManager.instance.showStopWatchTimer ("ChapterPanel, Init() finish");

        //별보상 이펙트
        for (int i = 0; i < BoxEffRoot.Length; i++)
        {
            UIHelper.CreateEffectInGame(BoxEffRoot[i].transform, "Fx_UI_chapter_box_on");
            //if(i != 0)
            //    ResettingParticle(1.5f, UIHelper.FindComponents<ParticleSystem>(BoxEffRoot[i].transform), 0.5f);
        }

        //버튼글자가 한글자씩이라....따로적용
        //string normal = _LowDataMgr.instance.GetStringCommon(459);
        //string hard = _LowDataMgr.instance.GetStringCommon(9902);

        //BtnNormal.transform.FindChild("label_1").GetComponent<UILabel>().text = normal.Substring(0, 1);
        //BtnNormal.transform.FindChild("label_2").GetComponent<UILabel>().text = normal.Substring(1, 1);

        //BtnHard.transform.FindChild("On/label_1").GetComponent<UILabel>().text = hard.Substring(0, 1);
        //BtnHard.transform.FindChild("On/label_2").GetComponent<UILabel>().text = hard.Substring(1, 1);
        //BtnHard.transform.FindChild("Off/label_1").GetComponent<UILabel>().text = hard.Substring(0, 1);
        //BtnHard.transform.FindChild("Off/label_2").GetComponent<UILabel>().text = hard.Substring(1, 1);
    }

    public override void LateInit()
    {
        base.LateInit();

        UI_OPEN_TYPE openType = SceneManager.instance.UiOpenType;
        SceneManager.instance.UiOpenType = UI_OPEN_TYPE.NONE;
        int openStageArr = -1, openChapterArr = 0;

        uint openStageId = (uint)parameters[1];
        if(openStageId <= 0 )
            openStageId = SceneManager.instance.GetState<TownState>().MyHero.moveTargetStage;
        reOpenPanel = (UIBasePanel)parameters[0];

        SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
        if (openStageId != uint.MaxValue)//미션
        {
            DungeonTable.StageInfo info = _LowDataMgr.instance.GetStageInfo(openStageId);
            IsLevelHard = info.type == 2 ? true : false;
            int chapterNum = info.ChapId;

            List<DataChapter> list = null;
            if (!IsLevelHard)
                list = ChapterList;
            else
                list = HardChapterList;

            if (list.Count <= chapterNum - 1)
                openChapterArr = list.Count;
            else
            {
                openChapterArr = chapterNum;
                openStageArr = (int)(openStageId % 100);
            }
        }
        else if (openType == UI_OPEN_TYPE.NEXT_ZONE)// 다음스테이지 버튼일시 
        {
            uint stageId = SingleGameState.lastSelectStageId;   //405 이렇게
            DungeonTable.StageInfo info = _LowDataMgr.instance.GetStageInfo(stageId);
            IsLevelHard = info.type == 2 ? true : false;
            int chapterNum = info.ChapId;
            int nextId = (int)(stageId % 100); // 스테이지 넘버 (챕터x)
            
            if (nextId < (IsLevelHard ? 3 : 10))//다음 스테이지 로가면된다
            {
                openChapterArr = chapterNum;
                openStageArr = nextId+1;
            }
            else  // 마지막 스테이지가 X-10 인경우는 다음챕터로 이동해야함
            {
                openStageArr = 1;
                openChapterArr = chapterNum + 1;
                
                if(IsLevelHard)//마지막 챕터인지 검사
                {  
                    if(ChapterList[openChapterArr-1].stageList[9].State != 1)
                    {
                        // 하드모드일경우 일반스테이지 클리어 했는지도 검사필요함
                        // 하드모드 1-3에서 2-1 을 가기위해서는 일반스테이지 2-10까지 클리어해야함.
                        IsLevelHard = false;
                    }
                    else if (HardChapterList.Count <= openChapterArr)
                        openChapterArr = HardChapterList.Count;
                }
                else
                {
                    if(ChapterList.Count<=openChapterArr)
                        openChapterArr = ChapterList.Count;
                }
              
            }
        }
        else// if(0 < parameters.Length )   //용 석상 or TownPanel에서 아이콘 터치 
        {//클리어나 패배후 다시 스테이지를 눌렀을시도 고려해서 난이도를 다시 체크해줌
            uint stageId = (uint)parameters[1];
            if (0 == stageId || stageId.Equals(uint.MaxValue))
                stageId = SingleGameState.lastSelectStageId;
            else
                openStageArr = (int)(stageId % 100);
            
            DungeonTable.StageInfo info = _LowDataMgr.instance.GetStageInfo(stageId);
            IsLevelHard = info.type == 2 ? true : false;
            openChapterArr = info.ChapId;

            if (SceneManager.instance.CurTutorial == TutorialType.STAGE)
                openStageArr = (int)(stageId % 100);
            else if (SceneManager.instance.CurTutorial == TutorialType.CHAPTER_HARD)
            {
                IsLevelHard = false;
                openChapterArr = 1;
            }
        }

        SetChapter(openChapterArr, IsLevelHard);
        if (0 <= openStageArr && !SingleGameState.IsChapterClear)
        {
            List<DataStage> stageList = CurChapter.stageList;
            OnClickStage(stageList[openStageArr - 1]);
        }
        else
            OnSubTutorial();

        SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();        
		SceneManager.instance.sw.Stop ();
        SceneManager.instance.showStopWatchTimer("ChapterPanel, LateInit() finish");
        
        if (SingleGameState.IsChapterClear)
        {
            SingleGameState.IsChapterClear = false;

            float delayTime = 0;
            List<UITweener> tweenList = UIHelper.FindComponents<UITweener>(ChapterClear);
            for (int i = 0; i < tweenList.Count; i++)
            {
                tweenList[i].ResetToBeginning();
                tweenList[i].PlayForward();

                if (delayTime < tweenList[i].delay + tweenList[i].duration)
                    delayTime = tweenList[i].delay + tweenList[i].duration;
            }

            DungeonTable.StageInfo lowData = _LowDataMgr.instance.GetStageInfo(SingleGameState.lastSelectStageId);
            string title = string.Format("{0} {1}", _LowDataMgr.instance.GetStringStageData(lowData.ChapName), _LowDataMgr.instance.GetStringStageData(lowData.ChapName + 10));
            ChapterClear.FindChild("Set_main_title/Txt_Title").GetComponent<UILabel>().text = title;

            //두줄로 나눠서 표기
            string info = _LowDataMgr.instance.GetStringStageData(lowData.ChapName + 20);
            string[] result = info.Split(new char[] { ' ' });

            ChapterClear.FindChild("Txt_Info_1").GetComponent<UILabel>().text = result[0];
            ChapterClear.FindChild("Txt_Info_2").GetComponent<UILabel>().text = result[1];
            ChapterClear.gameObject.SetActive(true);
            TempCoroutine.instance.FrameDelay(delayTime, () =>
            {
                if (ChapterClear != null)//혹시 모를 예외처리
                    ChapterClear.gameObject.SetActive(false);
            });
        }
        else
            ChapterClear.gameObject.SetActive(false);
    }

	public override void UIOpenEventCallback(){
		CameraManager.instance.mainCamera.gameObject.SetActive (false);
	}

    /// <summary> 버튼 이벤트 처리함수 </summary>
    void EventButton()
    {
        //UIButton 이벤트 처리
        EventDelegate.Set(Btns[(uint)BUTTON_TYPE.NEXT].onClick, OnClickNextBtn);
        EventDelegate.Set(Btns[(uint)BUTTON_TYPE.PREV].onClick, OnClickPrevBtn);

        //UITrigger 이벤트 처리
        EventDelegate.Set(ChapRewardTfs[0].parent.GetComponent<UIEventTrigger>().onClick,
            delegate () {
                if (!BoxEffRoot[rewardIdx].activeSelf)
                    return;

                BoxClickEffRoot[rewardIdx].SetActive(true);

                TempCoroutine.instance.FrameDelay(0.6f, () =>
                {
                    BoxClickEffRoot[rewardIdx].SetActive(false);
                    OnClickRewardBox();

                });
            }
          );

        EventDelegate.Set(BtnNormal.onClick, delegate ()
        {
            SetChapter(CurChapterNum, false);
        });
        EventDelegate.Set(BtnHard.onClick, delegate ()
        {
            if (CurChapter.stageList[9].State != 1)
            {
                uiMgr.AddPopup(141, 761, 117);
                return;
            }
            
            SetChapter(CurChapterNum, true);
        });

    }


    /// <summary> 난이도에 따라 변경될 챕터들 </summary>
    void SetChapter(int chapterNum, bool leveldifficulty)
    {
        bool isPrevEnable = true;
        bool isNextEnable = true;
        List<DataChapter> chapterList = null;
        Transform ChapterRoot;
       
        if (leveldifficulty)   //true일경우 Hard Chapter로.
        {
            ChapterRoot = HardChapterParent;
            chapterList = HardChapterList;
        }
        else  //false일경우 Normal Chapter로.
        {
            chapterList = ChapterList;
            ChapterRoot = ChapterParent;
        }

        //챕터 개수가 넘어가거나 모자를 경우의 예외처리. 
        if (chapterList.Count <= chapterNum)
        {
            chapterNum = (int)chapterList.Count;
            isNextEnable = false;
        }
        else if (chapterNum <= 1)
        {
            chapterNum = 1;
            isPrevEnable = false;
        }
        ////4챕터까지만.. (판호용)
        //if (chapterNum >= 4)
        //    isNextEnable = false;

        //현제 보려는 챕터를 찾는다.
        for (int i = 0; i < chapterList.Count; i++)
        {
            if (chapterList[i].number != chapterNum)
                continue;
            else if(chapterList[i].stageList[0].State == -1)//진행 불가능한 스테이지
            {
                uiMgr.AddPopup(141, 761, 117);
                return;
            }

            CurChapter = chapterList[i];
            break;
        }

        //처음이거나 마지막 챕터일 경우 버튼을 꺼놓는다
        Btns[(uint)BUTTON_TYPE.PREV].gameObject.SetActive(isPrevEnable);
        Btns[(uint)BUTTON_TYPE.NEXT].gameObject.SetActive(isNextEnable);

        ChapterObject chap = null, changeChap = null;
        if (1 <= ChapterRoot.childCount)//이미 있는 챕터가 있다면 만들지 않는다.
        {
            string chapObj = leveldifficulty ? "HardChapter_0" : "Chapter_0";
            string path = string.Format("UI/UIObject/{0}{1}",chapObj, chapterNum);

            //string path = string.Format("UI/UIObject/Chapter_0{0}", chapterNum);
            GameObject obj = ResourceMgr.Load(path) as GameObject;

            changeChap = obj.GetComponent<ChapterObject>();
            chap = ChapterRoot.GetChild(0).GetComponent<ChapterObject>();
        }
        else//처음 시작했을 경우 실행됨.
        {
            GameObject go = UIHelper.CreateUIChapter(chapterNum, ChapterRoot, leveldifficulty);
            chap = go.GetComponent<ChapterObject>();
        }

        if (chap != null)//없으면 오류
        {
            if (changeChap != null)//없을 수 있음
                chap.ChangeLocalData(changeChap);

            chap.SetChapter(CurChapter.stageList, OnClickStage, chapterNum.ToString());
            chap.gameObject.SetActive(true);
        }

        //별보상 셋팅
        SetGradeInfo();

        //진행중인 값 및 스테이지 셋팅
        CurChapterNum = chapterNum;

        BtnNormal.gameObject.SetActive(leveldifficulty);
        BtnHard.gameObject.SetActive(!leveldifficulty);

        bool activeHardMode = true;

        if (!leveldifficulty && CurChapter.stageList[9].State != 1)
            activeHardMode = false;


        BtnHard.transform.FindChild("On").gameObject.SetActive(activeHardMode);
        BtnHard.transform.FindChild("Off").gameObject.SetActive(!activeHardMode);
        /*
        if (BtnHard.transform.FindChild("On").gameObject.activeSelf)
        {
            //이펙트 껏다키면 안나오므로 다시생성해줌
            if(BtnHard.gameObject.activeSelf && BtnHard.transform.FindChild("On/eff").childCount>0)
            {
                DestroyImmediate(BtnHard.transform.FindChild("On/eff").GetChild(0).gameObject);
                UIHelper.CreateEffectInGame(BtnHard.transform.FindChild("On/eff"), "Fx_UI_HardMode_01");
            }
        }
        */
        ChapterParent.gameObject.SetActive(!leveldifficulty);
        HardChapterParent.gameObject.SetActive(leveldifficulty);

        IsLevelHard = leveldifficulty;

        //챕터의 이름 셋팅
        ChapterNameLabel.text = string.Format("{0}", CurChapter.name);
        HardChapterNameLabel.text = string.Format("{0}", CurChapter.name);

        NameRoot.transform.FindChild("normal").gameObject.SetActive(!leveldifficulty);
        NameRoot.transform.FindChild("Hard").gameObject.SetActive(leveldifficulty);
        /*
        //현재 퀘스트에 던전입장이 걸려있는가?
        Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
        if (quest != null && quest.type == 1)
        {

        }
        */
        if (leveldifficulty && SceneManager.instance.CurTutorial == TutorialType.CHAPTER_HARD)
        {
            TutorialSupport tuto = StagePopup.GetStartTuto();
            TutorialSupport stageTuto = chap.StageObjs[0].GetComponent<TutorialSupport>();
            stageTuto.NextTuto = tuto;
            stageTuto.OnTutoSupportStart();

            tuto.TutoType = TutorialType.CHAPTER_HARD;
            tuto.SortId = 4;
        }
    }

    /// <summary> 테이블 데이터 셋팅 </summary>
    void SetDataTable(List<DataChapter> dataList, uint addStageId)
    {
        Dictionary<uint, NetData.ClearSingleStageData> clearStageDic = NetData.instance.GetUserInfo().ClearSingleStageDic;
        
        bool curStage = false;

        int chapCount = addStageId >= 10000 ? 6 : (int)SystemDefine.MaxChapter;
        int stageCount = addStageId >= 10000 ? 3 : (int)SystemDefine.MaxChapter;

        //일반 스테이지
        for (int i=0; i < /*SystemDefine.MaxChapter*/chapCount; i++)
        {
            int chapterNumber = i+1;
            int totalClearGrade = 0;

            List<DataStage> stageList = new List<DataStage>();

            for (int j=0; j < /*SystemDefine.MaxStage*/stageCount; j++)
            {
                DataStage data = new DataStage();
                
                byte[] clearGrade = new byte[3];
                short state = 0;//1;//클리어한 지역.

                uint stageId = addStageId+(uint)((chapterNumber * 100) + (j+1));
                int clearCount = 0;
                NetData.ClearSingleStageData clearStageData = null;
                if (!clearStageDic.TryGetValue(stageId, out clearStageData))
                {
                    //추후에 살려놔야함 지금은 모든걸 오픈
                    if ( !curStage)
                    {
                        curStage = true;
                        state = 0;
                    }
                    else//활성화 되지 않은 지역.
                        state = -1;
                }
                else
                {
                    state = 1;

                    clearGrade[0] = clearStageData.Clear_0;
                    clearGrade[1] = clearStageData.Clear_1;
                    clearGrade[2] = clearStageData.Clear_2;

                    totalClearGrade += clearStageData.Clear_0 + clearStageData.Clear_1 + clearStageData.Clear_2;
                    clearCount = clearStageData.DailyClearCount;
                }

                if (data.InitStageData(stageId, clearGrade, state, clearCount))
                    stageList.Add(data);
                else//어려움일 경우 이럴수 있다 아니라면 문제가있는것.
                    break;
            }

            if (stageList.Count <= 0)
                break;

            NetData.StageStarRewardData rewardData = new NetData.StageStarRewardData();
            DataChapter dataChap = new DataChapter(chapterNumber, stageList[0]._StageLowData.type == 2, rewardData, stageList);//totalClearGrade, 
            dataList.Add(dataChap);
        }
    }

    int rewardIdx = 0;
    /// <summary> 별보상 관련 셋팅 </summary>
    void SetGradeInfo()
    {
        List<DungeonTable.ChapterRewardInfo> rewardList = CurChapter.StarReward;
        for (int i = 0; i < rewardList.Count; i++)
        {
            ChapRewardTfs[i].FindChild("con_value_d24").GetComponent<UILabel>().text = rewardList[i].NeedStar.ToString();

            GatchaReward.FixedRewardInfo info = _LowDataMgr.instance.GetFixedRewardItem(rewardList[i].Reward);
            if(info!=null)
            {
                uint id = info.ItemId;
                if (id <= 0)
                    id = info.Type;
                InvenItemRewardSlot[i].SetLowDataItemSlot(id, info.ItemCount);
            }
            float process = 0;
            if (0 < i )
            {
                if(rewardList[i-1].NeedStar <= CurChapter.GetClearGrade)//이전꺼를 클리어 했는지
                    process = (float)CurChapter.GetClearGrade / (float)rewardList[i].NeedStar;//했다면 함
            }
            else//첫번째는 걍 함
                process = (float)CurChapter.GetClearGrade / (float)rewardList[i].NeedStar;

            ChapRewardTfs[i].FindChild("bar").GetComponent<UISprite>().fillAmount = process;
            ChapRewardTfs[i].FindChild("boxEff").gameObject.SetActive(false);

            if (rewardList[i].NeedStar <= CurChapter.GetClearGrade)//1단계 클리어 했는지
            {
                ChapRewardTfs[i].FindChild("getMark").gameObject.SetActive(true);

                if (CurChapter.CurRewardBoxID-1 <= i)
                {
                    ChapRewardTfs[i].FindChild("boxEff").gameObject.SetActive(true);
                    ChapRewardTfs[i].FindChild("getMark").gameObject.SetActive(false);
                }

                if (CurChapter.CurRewardBoxID - 1 == i)
                    rewardIdx = i;
            }
            else
                ChapRewardTfs[i].FindChild("getMark").gameObject.SetActive(false);
        }
    }

    /// <summary> 챕터 별 보상 응답 </summary>
    public void OnPMsgStageChapterRewardSHandler(int chapId, int boxId)
    {
        if (CurChapterNum == chapId)
        {
            CurChapter.CurRewardBoxID = boxId;
        }
        else
        {
            for (int i = 0; i < ChapterList.Count; i++)
            {
                if (ChapterList[i].number != chapId)
                    continue;

                ChapterList[i].CurRewardBoxID = boxId;
            }
        }

        SetGradeInfo();
    }

    #region 이벤트 버튼
    /// <summary>
    /// 뒤로가기 시 발생할 이벤트
    /// </summary>
    //public override void Hide()
    public override void Hide()
    {

		CameraManager.instance.mainCamera.gameObject.SetActive (true);

        byte state = StagePopup.OnClosePopup();

        if (state == 0)//팝업 객체가 꺼져있다면 종료. 아니면 무시 시킨다.
	    {
            base.Hide();
            if (reOpenPanel != null)

                reOpenPanel.Show(reOpenPanel.GetParams());
            else
                UIMgr.OpenTown();
        }
        else if (state == 1)
        {
            SetArrow();
        }
    }
	    
    public void SetArrow()
    {
        List<DataChapter> chapterList = null;
        if (IsLevelHard)//true일경우 Hard Chapter로.
            chapterList = HardChapterList;
        else  //false일경우 Normal Chapter로.
            chapterList = ChapterList;

        bool isNextEnable = true, isPrevEnable = true;
        if (chapterList.Count <= CurChapterNum)
        {
            isNextEnable = false;
        }
        else if (CurChapterNum <= 1)
        {
            isPrevEnable = false;
        }

        //if(CurChapterNum >= 4)
        //    isNextEnable = false;

        //처음이거나 마지막 챕터일 경우 버튼을 꺼놓는다
        Btns[(uint)BUTTON_TYPE.PREV].gameObject.SetActive(isPrevEnable);
        Btns[(uint)BUTTON_TYPE.NEXT].gameObject.SetActive(isNextEnable);
    }

    /// <summary> 챕터 다음으로 클릭 </summary>
    void OnClickNextBtn()
    {
        if (IsLevelHard)
        {
            //일반스테이지 체크
            if (ChapterList[CurChapterNum].stageList[9].State != 1)
            {
                uiMgr.AddPopup(141, 761, 117);
                return;
            }
        }

        SetChapter(CurChapterNum + 1, IsLevelHard);

    }

    /// <summary> 챕터 이전으로 클릭 </summary>
    void OnClickPrevBtn()
    {
        SetChapter(CurChapterNum-1, IsLevelHard);
    }
    
    /// <summary> 스테이지 클릭 했을 경우 이벤트 처리. </summary>
    void OnClickStage(DataStage data)
    {
        if (data == null)
            return;
        
        if(data.State == -1)
        {
            //uiMgr.AddPopup((int)sw.ErrorCode.ER_StageStartS_RequireStageId_Error, null, null, null);
            uiMgr.AddErrorPopup((int)Sw.ErrorCode.ER_StageStartS_RequireStageId_Error);
            return;
        }

        Btns[(uint)BUTTON_TYPE.PREV].gameObject.SetActive(false);
        Btns[(uint)BUTTON_TYPE.NEXT].gameObject.SetActive(false);
        StagePopup.SetStagePopup(data, true);
    }

	public void OnClickStageTest(int stageId){

		int chapterIdArr = (stageId/100)-1;
		List<DataStage> stageList = ChapterList[chapterIdArr].stageList;

		int stageIdArr = stageId % 100 - 1;
		OnClickStage(stageList[stageIdArr] );

		TempCoroutine.instance.FrameDelay (1f, () => {
			NetworkClient.instance.SendPMsgStageStartC((int)stageId);
		});
	}
    
    public override void OnCloseReadyPopup()
    {
        base.OnCloseReadyPopup();
        DataStage data = StagePopup.GetChapterData();
        if (data == null)
            return;

        Btns[(uint)BUTTON_TYPE.PREV].gameObject.SetActive(false);
        Btns[(uint)BUTTON_TYPE.NEXT].gameObject.SetActive(false);
        StagePopup.SetStagePopup(data, false);
    }

    /// <summary> 별보상 박스 선택함 </summary>
    void OnClickRewardBox()
    {
        if (CurChapter.StarReward == null || CurChapter.StarReward.Count <= CurChapter.CurRewardBoxID-1 || CurChapter.CurRewardBoxID <= 0)//|| CurChapter.StarReward.Count <= CurChapter.CurRewardBoxID)
            return;
        
        if (CurChapter.StarReward[(int)(CurChapter.CurRewardBoxID-1)].NeedStar <= CurChapter.GetClearGrade)// 조건충족.
        {
            NetworkClient.instance.SendPMsgStageChapterRewardC(CurChapterNum, CurChapter.CurRewardBoxID, (int)(CurChapter.IsHard ? 2 : 1));
        }
    }

    #endregion

    public void SetStageInfoPopup(uint stageId)
    {
        DungeonTable.StageInfo info = _LowDataMgr.instance.GetStageInfo(stageId);
        IsLevelHard = info.type == 2 ? true : false;
        int chapterNum = info.ChapId;

        List<DataChapter> list = null;
        if (!IsLevelHard)
            list = ChapterList;
        else
            list = HardChapterList;

        if (list.Count <= chapterNum - 1)
            chapterNum = list.Count;

        stageId = (stageId % 100)-1;
        if(CurChapterNum != chapterNum)
            SetChapter(chapterNum, IsLevelHard);
        
        if (0 <= stageId && !SingleGameState.IsChapterClear)
        {
            List<DataStage> stageList = CurChapter.stageList;
            if (stageList[(int)stageId].State == -1)
                return;

            Btns[(uint)BUTTON_TYPE.PREV].gameObject.SetActive(false);
            Btns[(uint)BUTTON_TYPE.NEXT].gameObject.SetActive(false);
            StagePopup.SetStagePopup(stageList[(int)stageId], true);
        }
    }

    /// <summary> ChapterPopup에서 게임시작 버튼 누르면 실행 </summary>
    public override void GotoInGame()
    {
        if (IsStarted)
            return;

        IsStarted = true;
        NetworkClient.instance.SendPMsgStageStartC((int)StagePopup.GetChapterData().TableID);
    }

    public void OnErrorGameStart()
    {
        IsStarted = false;
    }


    public void OnStageStart()
    {
        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.SINGLE, () =>
        {
            //이상태에서의 데이터를 저장
            NetData.instance.MakePlayerSyncData(true);

            //마지막으로 선택한 스테이지 아이디 저장
            DataStage data = StagePopup.GetChapterData();
            SingleGameState.lastSelectStageId = data.TableID;
            SingleGameState.StageQuestList = data.QuestList;
            SingleGameState.CurStageName = data.StageName;
            SingleGameState.verifyToken = 0;
            
            SceneManager.instance.ActionEvent(_ACTION.PLAY_SINGLE);
            base.GotoInGame();
        });

        uiMgr.CloseReadyPopup();
        base.Close();
    }


    /// <summary> 소탕 보상 응답 </summary>
    public void OnPMsgSweepReward(NetData.SweepSlotData slotData)
    {
        StagePopup.OnPMsgSweepReward(slotData);
    }

    /// <summary> 소탕 끝난거 응답 갱신시켜준다. </summary>
    public void OnPMsgStageSweep(int dailyClearCount)
    {
        StagePopup.OnPMsgStageSweep(dailyClearCount);
    }

    /// <summary> 스테이지 데이터 넘겨준다. (ChapterPopup) </summary>
    public DataStage GetDataStage(uint lowDataId)
    {
        int count = CurChapter.stageList.Count;
        for(int i=0; i < count; i++)
        {
            if (CurChapter.stageList[i].State == -1)
                continue;

            if (CurChapter.stageList[i].TableID != lowDataId)
                continue;

            return CurChapter.stageList[i];
        }

        return null;
    }
    
    public float GetPopupActionTime()
    {
        return StagePopup.GetActionTime();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            float delayTime = 0;
            List<UITweener> tweenList = UIHelper.FindComponents<UITweener>(ChapterClear);
            for (int i = 0; i < tweenList.Count; i++)
            {
                tweenList[i].ResetToBeginning();
                tweenList[i].PlayForward();

                if (delayTime < tweenList[i].delay + tweenList[i].duration)
                    delayTime = tweenList[i].delay + tweenList[i].duration;
            }


            DungeonTable.StageInfo lowData = _LowDataMgr.instance.GetStageInfo(110);
            string title = string.Format("{0} {1}", _LowDataMgr.instance.GetStringStageData(lowData.ChapName), _LowDataMgr.instance.GetStringStageData(lowData.ChapName + 10));
            ChapterClear.FindChild("Set_main_title/Txt_Title").GetComponent<UILabel>().text = title;
          
            //두줄로 나눠서 표기
            string info = _LowDataMgr.instance.GetStringStageData(lowData.ChapName + 20);
            string[] result = info.Split(new char[] {' '});

            ChapterClear.FindChild("Txt_Info_1").GetComponent<UILabel>().text = result[0];
            ChapterClear.FindChild("Txt_Info_2").GetComponent<UILabel>().text = result[1];
            ChapterClear.gameObject.SetActive(true);


            ChapterClear.gameObject.SetActive(true);
            TempCoroutine.instance.FrameDelay(delayTime, () => {
                ChapterClear.gameObject.SetActive(false);
            });
        }
    }

    /// <summary>
    /// 파티클 사이즈 재조정
    /// </summary>
    /// <param name="go">원하는 객체</param>
    void ResettingParticle(float scale, List<ParticleSystem> parList, float offScale)
    {
        float scaleX = 0;
        //if (parent == null)
        //    scaleX = offScale;
        //else
        {
            float x = scale;
            scaleX = x;// * 0.0027f;
        }

        if (scaleX < 0)//설마 0 이하가 될까...
            scaleX = 1f;

        int listCount = parList.Count;
        for (int i = 0; i < listCount; i++)
        {
            ParticleSystem ps = parList[i];
            if (ps == null)
                continue;

            float startSize = ps.startSize * scaleX;
            float startSpeed = ps.startSpeed * scaleX;
            float startLifeTime = ps.startLifetime * (scaleX + (scaleX * 0.1f));

            ps.startSize = startSize;
            ps.startSpeed = startSpeed;
            ps.startLifetime = startLifeTime;
        }
    }
}

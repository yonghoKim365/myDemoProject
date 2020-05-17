using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TownPanel : UIBasePanel
{

    public GameObject JoystickGroup;// 조이스틱 그룹. 하위에 조이스틱이 생성되도록 구현 필요
    public GameObject HeadObject;//이름표기해줄 오브젝트

    public GameObject PowerUpGo;    //파워업 오브젝트
    public GameObject[] LastGetEff; //마지막선택이펙트

    public Transform HeadParent;//이름표기해줄 객체의 어미
    public Transform FaceParent;//캐릭터 얼굴3D

    public Transform BottomInfo;    //하단에 표시할 배터리,시간, 네트워크상태
    
    public TownCamZoom Zoom;
    
    public UILabel RubyText;
    public UILabel LevelText;
    public UILabel BattlePowerText;
    public UILabel VipLevelText;
    public UILabel HPText;

    public UISlider ExpFilled;
    public UIEventTrigger BtnCharIcon;//캐릭터 아이콘 클릭시
    public UIEventTrigger BtnVip;   //vip팝업 연결
    public UIEventTrigger[] BtnQuest;//퀘스트 버튼들 0~2
    
    public UIButton BtnOption;//옵션
    public UIButton BtnRank;//랭킹
    public UIButton BtnAchieve; //업적
    public UIButton BtnBenefit; //혜택
    public UIButton BtnStory;//스토리던전
    public UIButton BtnDungeon;//던전 모음
    public UIButton BtnFreefight;   //난투장
    
    public UIButton[] BottomLeftBtns;//하단의 왼쪽에 위치한 버튼들

    public Joystick Joystick;

    public GameObject[] AlramMark;//알림아이콘

    public float PathOffset = 220;

    private NetData._UserInfo CharInven;
    private ChatPopup ChatPop;
    private MapPanel Map;
    private Quest.QuestInfo CurQuestInfo;

    private Transform PathArrowTf;
    private MyTownUnit MyUnit;

    enum eSelEff
    {
        none = -1,
        //eStoty = 0,
        //eDungen,
        //ePartner,
        //eCostume,
        //eInven,
        //eQuest,
        //eFriend,
        //eRank,
        eAchieve,
        eBenefit,
       //eGuild
    }

    public bool IsHideTown;

    public override void Init()
    {
        IsHideTown = true;
        SceneManager.instance.showStopWatchTimer("TownPanel init start");

        base.Init();

        CharInven = NetData.instance.GetUserInfo();
        CameraManager.instance.mainCamera.enabled = true;

        uint totalAtt = CharInven.RefreshTotalAttackPoint(false);
        UIMgr.instance.PrevAttack = (int)totalAtt;  // 상승전의 값을 넣어준다    

        BtnEventsConnecting();

        EventDelegate.Set(onShow, OnShow);
        QuestManager.instance.SetTownPanel(this);

        StartCoroutine(JoystickSetting());


        PathArrowTf = UIHelper.CreateEffectInGame(transform, "Fx_IN_direction_001").transform;
        PathArrowTf.localPosition = new Vector3(20, 0, 0);
        DelayedInit();

        setTestBtns();

        SceneManager.instance.showStopWatchTimer("TownPanel init end");

        //길드쪽..
        if (CharInven._GuildId != 0)
        {
            NetworkClient.instance.SendPMsgGuildQueryApplyListC(CharInven._GuildId);
        }
        
    }

    //IEnumerator DelayedInit(){
    void DelayedInit(){

		//yield return new WaitForSeconds (2f);

		TownState town = SceneManager.instance.GetState<TownState>();
		CreateHeadObjet(town.MyHero.gameObject, NetData.instance.Nickname, CharInven._LeftTitle, CharInven._RightTitle, true);
		
		UIHelper.CreateEffectInGame(transform.FindChild("PlayerInfoGroup/Btnstreng"), "Fx_UI_BattlePower_01");
		UIHelper.CreateEffectInGame(transform.FindChild("PlayerInfoGroup/VipInfo"), "Fx_UI_VIP_01");

		//선택이펙트
		for (int i = 0; i < LastGetEff.Length; i++)
		{
            LastGetEff[i] = UIHelper.CreateEffectInGame(LastGetEff[i].transform, "Fx_UI_icon_lastClick_01");
            LastGetEff[i].SetActive(false);
		}
		
		
		UIHelper.CreateEffectInGame(BtnQuest[0].transform.FindChild("open/EffRoot"), "Fx_UI_quest_01");
		Map = UIMgr.OpenMapPanel();
	}

    public override void LateInit()
    {

		if (!mStarted) {
			SceneManager.instance.showStopWatchTimer ("TownPanel LateInit start");
		}

		TownState town = SceneManager.instance.GetState<TownState>();
		MyUnit = town.MyHero;

        //이곳에서 해주면 Hide됬다가 재 실행됬을 때도 실행 되므로 여기서 호출
        base.LateInit();

        RefreshUserInfo();

		// 케릭터생성창에서 shadow light를 끄기때문에 마울진입시 다시 켜준다.
		UIMgr.instance.SetShadowLightActive (true);

        ChatPop = UIMgr.OpenChatPopup();
        if (ChatPop != null)
            ChatPop.OnShow();

        if (!mStarted)//한번만 실행임
        {
            CharInven.RefreshTotalAttackPoint();
            
            int userNickCount = town.TownNickList.Count;
            for (int i = 0; i < userNickCount; i++)
            {
                TownState.TownNickData nickData = town.TownNickList[i];
                CreateHeadObjet(nickData.Obj, nickData.Nick, nickData.Prefix, nickData.Suffix, false);
                town.CreateAccessEff(nickData.Obj);
            }

            town.TownNickList.Clear();
            //CreatePcFaceObj();//겁나 빠르게 왔을 경우 이경우일듯
            //if (!town._ActivityInfo.IsSend)
            //{
            //    ActivityPoint(town._ActivityInfo.GetPoint);
            //}
        }
        else
            CharInven = NetData.instance.GetUserInfo();

        MissionListSetting();

        //CheckPowerUp();
        CheckAlramMark();
        
        if (SceneManager.instance.testData.bSingleSceneTestStart) {
			StartCoroutine(repeatSingleTest());
		}

		if (SceneManager.instance.testData.bQuestTestStart) {
			StartCoroutine(repeatQuestTest());
		}

		if (!mStarted) {
			SceneManager.instance.sw.Stop ();
			SceneManager.instance.showStopWatchTimer ("TownPanel LateInit end");
		}

        CheckOpenContents(false);

        if (!SceneManager.instance.IsYieldAction)//마을 UI로 넘어가는 것이라면.
        {
            if (!IsHideTown && town.IsEndSceneLoad)
            {
                if (!SceneManager.instance.IsActiveNoticeType(NoticeType.Contents))
                    OnTutorial();
            }
        }
    }

    /// <summary> 타운 스테이트의 모든 로드가 끝난 시점이라고 보면된다. </summary>
    public void OnEndStateLoad()
    {
        if (!SceneManager.instance.IsActiveNoticeType(NoticeType.Contents))
            OnTutorial();

        //CheckOpenContents(false);
        IsHideTown = false;
    }
	
	IEnumerator repeatQuestTest(){
		yield return new WaitForSeconds (4f);

		QuestClick ();

	}

	IEnumerator repeatSingleTest(){

		yield return new WaitForSeconds (4f);

		UIBasePanel changePopup = UIMgr.instance.FindInShowing("UIPopup/ChangePopup");

		bool bLoop = true;
		while (bLoop){

			if (changePopup == null)bLoop=false;
			if (changePopup && changePopup.gameObject.activeInHierarchy==false){
				bLoop=false;
			}
			yield return null;
		}

		SceneManager.instance.testData.nTestStageId = SceneManager.instance.testData.nextStageId;

		startSingleTest();
	}

	void startSingleTest(){
		Debug.Log(string.Format("<color=yellow>startSingleTest, stage = {0}</color>", SceneManager.instance.testData.nTestStageId) );

		OpenChapter ();

		TempCoroutine.instance.FrameDelay(2f, () =>{
			
			UIBasePanel chapterPanel = UIMgr.GetUIBasePanel("UIPanel/ChapterPanel");
			if (chapterPanel != null)
			{
				(chapterPanel as ChapterPanel).OnClickStageTest(SceneManager.instance.testData.nTestStageId);
			}
		});
	}

    void CheckAlramMark()
    {
        //뽑기시간 여기서체크
        //System.TimeSpan goldTime = System.DateTime.Now.AddSeconds(SceneManager.instance.GetGachaFreeTime[0]) - System.DateTime.Now;
        //System.TimeSpan cashTime = System.DateTime.Now.AddSeconds(SceneManager.instance.GetGachaFreeTime[1]) - System.DateTime.Now;
        //SceneManager.instance.SetAlram(AlramIconType.SHOP, 0 >= goldTime.TotalSeconds || 0 >= cashTime.TotalSeconds);
        SceneManager.instance.SetAlram(AlramIconType.SHOP, false);  //ㄱㅏ챠 막아서 무조건 꺼주세여

        //혜택
        SceneManager.instance.SetAlram(AlramIconType.BENEFIT, SceneManager.instance.IsBenefitAlram(-1) );

        //장비
        int[] openSlot = new int[] {
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen01CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen02CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen03CharLevel),
            _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.EquipSetOpen04CharLevel)//셋트아이템 3개뿐 나중에 이것도 살리자
        };

        if(openSlot.Length != CharInven.GetOwnSetItemData().Count)
            SceneManager.instance.SetAlram(AlramIconType.CHAR, openSlot[CharInven.GetOwnSetItemData().Count - 1] <= CharInven._Level);
        else
            SceneManager.instance.SetAlram(AlramIconType.CHAR, false);

        for (int i = 0; i < AlramMark.Length - 1; i++)
        {
            bool active = false;
            if (i == (int)AlramIconType.SOCIAL)
            {
                //둘중하나만 해도 점
                if (SceneManager.instance.IsAlram(AlramIconType._SOCIAL) ||SceneManager.instance.IsAlram(AlramIconType.SOCIAL))
                    active = true;
            }
            else if (i == (int)AlramIconType.CATEGORY)
            {
                if (SceneManager.instance.IsAlram(AlramIconType.CATEGORY))
                    active = true;
                //else
                //    active = SceneManager.instance.IsNewItemForUID(0);   // 새아이템이잇다면? 알림
            }
            else if (i == (int)AlramIconType.SINGLE)
            {
                // 상자 획득가능상태도 체크해...
                List<NetData.StageStarRewardData> stage = CharInven.StageStarReward;
                for (int j = 0; j < stage.Count; j++)//노말 검사
                {
                    List<DungeonTable.ChapterRewardInfo> data = _LowDataMgr.instance.GetLowDataChapterReward((byte)stage[j].ChapterID, 1);
                    if (stage[j].BoxID != 4 && data[stage[j].BoxID - 1].NeedStar <= stage[j].Value )
                    {
                        active = true;
                        break;
                    }
                }

                if (!active)//하드 검사
                {
                    List<NetData.StageStarRewardData> hardStage = CharInven.HardStageStarReward;
                    for (int j = 0; j < hardStage.Count; j++)
                    {
                        List<DungeonTable.ChapterRewardInfo> data = _LowDataMgr.instance.GetLowDataChapterReward((byte)hardStage[j].ChapterID, 2);
                        if (hardStage[j].BoxID != 4 && data[hardStage[j].BoxID - 1].NeedStar <= hardStage[j].Value)
                        {
                            active = true;
                            break;
                        }
                    }
                }
            }
            else if(i == (int)AlramIconType.CHAR)//캐릭터에서 셋트장비 뽑을수 있는것이 있는지 확인.
                active = SceneManager.instance.IsAlram(AlramIconType.CHAR);
            else
            {
                active = SceneManager.instance.IsAlram((AlramIconType)i);
            }

            AlramMark[i].SetActive(active);
        }


        //획득유도
        LastGetEff[(int)eSelEff.eAchieve].gameObject.SetActive(SceneManager.instance.IsAlram(AlramIconType.ACHIEVE) );
        LastGetEff[(int)eSelEff.eBenefit].gameObject.SetActive(SceneManager.instance.IsAlram(AlramIconType.BENEFIT) );
    }

    IEnumerator SelectIcon(GameObject Go)
    {
        float duration = 3f;

        while (true)
        {
            Go.transform.localRotation = Quaternion.Lerp(Go.transform.localRotation, Quaternion.Euler(0, 0, 8), duration);
            yield return new WaitForSeconds(0.15f);
            Go.transform.localRotation = Quaternion.Lerp(Go.transform.localRotation, Quaternion.Euler(0, 0, 0), duration);
            yield return new WaitForSeconds(0.15f);
            Go.transform.localRotation = Quaternion.Lerp(Go.transform.localRotation, Quaternion.Euler(0, 0, 8), duration);
            yield return new WaitForSeconds(0.15f);
            Go.transform.localRotation = Quaternion.Lerp(Go.transform.localRotation, Quaternion.Euler(0, 0, 0), duration);

            yield return new WaitForSeconds(2f);

        }

    }
    /*
    /// <summary>
    /// 인벤토리에 있는 장착가능한 무기중에서 전투력을 더 높일수 있는게 존재하는지 체크
    /// </summary>
    public void CheckPowerUp()
    {
        // 파츠별로 분류?
        Dictionary<ePartType, List<uint>> PartsDic = new Dictionary<ePartType, List<uint>>();

        NetData._UserInfo userInfo = NetData.instance.GetUserInfo();
        List<NetData._ItemData> myItemList = userInfo.GetItemList();

        for (int i = (int)ePartType.HELMET; i < (int)ePartType.PART_MAX; i++)
        {
            NetData._ItemData CurEquipData = userInfo.GetEquipParts((ePartType)i); //현재 내파츠

            //PartsDic.Add((ePartType)i,  )
            for (int j = 0; j < myItemList.Count; j++)
            {
                if (CurEquipData == null)
                {
                    //빈파츠 일 경우 파츠타입이 맞으면 바로 딕셔너리에 등록시켜줌

                    if (myItemList[j].EquipPartType == (ePartType)i)
                    {
                        // 더높은 전투력이면 상승할 전투력값을 더해준다
                        if (PartsDic.ContainsKey((ePartType)i))
                        {
                            //착용가능 레벨인지 체크
                            Item.EquipmentInfo equip = _LowDataMgr.instance.GetLowDataEquipItemInfo(myItemList[j]._equipitemDataIndex);
                            if (userInfo._Level < equip.LimitLevel)//레벨이 모자름.
                                continue;
                            NetData._UserInfo _inven = NetData.instance.GetUserInfo();
                            Character.CharacterInfo _charLowData = _LowDataMgr.instance.GetCharcterData(_inven._userCharIndex);
                            if (_charLowData.Class != equip.Class)//장착 불가능
                                continue;

                            PartsDic[(ePartType)i].Add(myItemList[j]._Attack);
                        }
                        else
                        {
                            //착용가능 레벨인지 체크
                            Item.EquipmentInfo equip = _LowDataMgr.instance.GetLowDataEquipItemInfo(myItemList[j]._equipitemDataIndex);
                            if (userInfo._Level < equip.LimitLevel)//레벨이 모자름.
                                continue;

                            NetData._UserInfo _inven = NetData.instance.GetUserInfo();
                            Character.CharacterInfo _charLowData = _LowDataMgr.instance.GetCharcterData(_inven._userCharIndex);
                            if (_charLowData.Class != equip.Class)//장착 불가능
                                continue;

                            PartsDic.Add((ePartType)i, new List<uint> { myItemList[j]._Attack });
                        }
                    }

                    continue;
                }


                //파츠타입 비교해줌
                if (CurEquipData.EquipPartType != myItemList[j].EquipPartType)
                    continue;
                //착용가능 레벨인지 체크
                Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo(myItemList[j]._equipitemDataIndex);
                if (userInfo._Level < equipInfo.LimitLevel)//레벨이 모자름.
                    continue;

                NetData._UserInfo inven = NetData.instance.GetUserInfo();
                Character.CharacterInfo charLowData = _LowDataMgr.instance.GetCharcterData(inven._userCharIndex);
                if (charLowData.Class != equipInfo.Class)//장착 불가능
                    continue;


                //같은 파츠이면 현재 끼고있는 파츠의 전투력과비교
                if (CurEquipData._Attack < myItemList[j]._Attack)
                {
                    // 더높은 전투력이면 상승할 전투력값을 더해준다
                    if (PartsDic.ContainsKey((ePartType)i))
                    {
                        PartsDic[(ePartType)i].Add(myItemList[j]._Attack - CurEquipData._Attack);
                    }
                    else
                    {
                        PartsDic.Add((ePartType)i, new List<uint> { myItemList[j]._Attack - CurEquipData._Attack });
                    }
                }
            }
        }

        //가장 높은전투력의 파츠끼리 더해줌
        foreach (KeyValuePair<ePartType, List<uint>> value in PartsDic)
        {
            value.Value.Sort(); //정렬시켜준다 , 이때 오름차순으로 되기때문에
            value.Value.Reverse();  //Reverse시켜줌
        }

        uint att = 0;
        foreach (KeyValuePair<ePartType, List<uint>> value in PartsDic)
        {
            att += value.Value[0];  //0번째인덱스값이 가장크게 상승할 전투력값
        }

        //        Debug.Log(att);
        GameObject AttLabel = BtnPowerUp.transform.FindChild("attLabel").gameObject;
        AttLabel.GetComponent<UILabel>().text = string.Format("{0}{1}", "↑", att);
        AttLabel.SetActive(att > 0 ? true : false);

        //PowerUpGo.SetActive(AttLabel.activeSelf);   // 전투력올릴수있ㄴ을때만 켜주기

        SceneManager.instance.SetAlram(AlramIconType.INVENTORY, att > 0 );
        AlramMark[(int)AlramIconType.INVENTORY].SetActive(AttLabel.activeSelf);
    }
    */
    /// <summary> 유저 정보 갱신 </summary>
    void RefreshUserInfo()
    {
        for (byte i = 0; i < 6; i++)
            RefreshUserInfo(i);

    }

    /// <summary> state 값에 맞게 갱신함 </summary>
    /// <param name="state">0 exp, 1 level, 2 vipLv, 3 gold, 4 energy, 5 cash</param>
    public void RefreshUserInfo(byte state)
    {
        if (!gameObject.activeSelf)
            return;
        
        switch (state)
        {
            case 0: //exp
                uint curExp = 0, maxExp = 0;
                CharInven.GetCurrentAndMaxExp(ref curExp, ref maxExp);
                ExpFilled.value = ((float)curExp / (float)maxExp);
                break;
            case 1://level
                LevelText.text = NetData.instance.UserLevel.ToString();//string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
                break;
            case 2: //vip
                VipLevelText.text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(460), CharInven._VipLevel);
                VipLevelText.color = SetVipColor(CharInven._VipLevel);
                break;
            //case 3://gold
            //    ulong gold = NetData.instance.GetAsset(AssetType.Gold);
            //    GoldText.text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(4), gold == 0 ? "0" : gold.ToString("#,##"));
            //    break;
            //case 4://energy
            //    ulong energy = NetData.instance.GetAsset(AssetType.Energy);
            //    EnergyText.text = string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(1), energy == 0 ? "0" : energy.ToString("#,##"));
            //    break;
            case 5://cash
                ulong cash = NetData.instance.GetAsset(AssetType.Cash);
                //RubyText.text = cash == 0 ? "0" : cash.ToString(); // ToString("#,##");//string.Format("{0} : {1}", _LowDataMgr.instance.GetStringCommon(3), cash == 0 ? "0" : cash.ToString("#,##"));
				RubyText.text = cash.ToString();
                break;
        }
    }

    public void RefreshGold()
    {

    }

    /// <summary> vip레벨에 따른 색깔 </summary>
    public Color SetVipColor(uint lv)
    {
        Color vipColor;

        // vip레벨따라 색깔표시가 다름
        Color brown = new Color(0.85f, 0.59f, 0.58f);
        Color blue = new Color(0, 0.69f, 0.94f);
        Color Green = new Color(0, 0.69f, 0.31f);
        Color yellow = new Color(1, 1, 0.04f);

        if (lv == 15) //15
        {
            vipColor = yellow;
        }
        else if (lv == 0) //0
        {
            vipColor = Color.white;
        }
        else
        {
            if (lv > 10) //11~14
            {
                vipColor = Green;
            }
            else
            {
                if (lv > 5) //6~10
                    vipColor = blue;
                else    // 1~5
                    vipColor = brown;
            }
        }

        return vipColor;
    }

    IEnumerator JoystickSetting()
    {
        TownState ts = SceneManager.instance.CurrentStateBase() as TownState;
        while (ts == null)
        {
            yield return null;
        }

        if (Joystick == null)
        {
            Joystick = UIMgr.OpenJoystick(JoystickGroup.transform.localPosition);//ResourceMgr.InstAndGetComponent<Joystick>("UI/UIObject/Joystick");//JoystickComponent
            //Joystick.transform.SetParent(JoystickGroup.transform);
            //Joystick.transform.localPosition = JoystickGroup.transform.localPosition;//Vector3.zero;
            ts.MyHero.ControllerSetting(Joystick);
        }

    }

    /// <summary>
    /// 각종 버튼들에 대해서 연결하자
    /// </summary>
    void BtnEventsConnecting()
    {
        //EventDelegate.Set(BtnEnergy.onClick, FoodAddClick);//에너지 충전?
        //EventDelegate.Set(BtnGold.onClick, GoldAddClick);//골드 충전?
        //EventDelegate.Set(BtnRuby.onClick, CashAddClick);//캐쉬 충전?
        //EventDelegate.Set(BtnPowerUp.onClick, PowerupClick);//파워업 이벤트
        EventDelegate.Set(BtnCharIcon.onClick, delegate ()
        {
            if (IsHideTown)
                return;

            Hide();
            UIMgr.OpenEquipPanel(null);
            //UIMgr.OpenCharPanel();
        });//속성탭으로
        EventDelegate.Set(BtnVip.onClick, OnClickGotoVip);//vip 팝업으로

        EventDelegate.Set(BtnQuest[0].onClick, delegate() {
            UIHelper.CreateEffectInGame(BtnQuest[0].transform.FindChild("open/SelEff"), "Fx_UI_quest_02"); //선택이펙트
            TempCoroutine.instance.FrameDelay(0.2f, () =>
            {
                Destroy(BtnQuest[0].transform.FindChild("open/SelEff").GetChild(0).gameObject);
            });
            QuestClick();

        });


        EventDelegate.Set(BtnOption.onClick, OnClickOption);//옵션
        EventDelegate.Set(BtnRank.onClick, OnClickRank );//랭킹
        EventDelegate.Set(BtnAchieve.onClick, OnClickAchieve );//업적
        EventDelegate.Set(BtnBenefit.onClick, OnClickBenefit);//혜택

        //하단 LeftSidePanel의 버튼
        EventDelegate.Set(BottomLeftBtns[0].onClick, OnClickOpenQuestPopup );//퀘스트 팝업
        EventDelegate.Set(BottomLeftBtns[1].onClick, OnClickGotoGuild );//길드
        EventDelegate.Set(BottomLeftBtns[2].onClick, OnClickGotoPartnerPanel );//파트너
        EventDelegate.Set(BottomLeftBtns[3].onClick, OnClickGotoHeroPanel);//캐릭터
        EventDelegate.Set(BottomLeftBtns[4].onClick, OnClickCategory);//재화인벤
        EventDelegate.Set(BottomLeftBtns[5].onClick, OnClickGotoFriendPanel );//친구
        EventDelegate.Set(BottomLeftBtns[6].onClick, OnClickGotoShopPanel);//상점

        //하단 TopSideGroup
        EventDelegate.Set(BtnStory.onClick, OnClickGotoChapterPanel );//모험
        EventDelegate.Set(BtnDungeon.onClick, OnClickDungeonPanelPanel );//Pve

        //실명 인증 버튼
        EventDelegate.Set(transform.FindChild("TopBtnGroup/BtnClause").GetComponent<UIButton>().onClick, delegate ()
        {
            if (IsHideTown)
                return;

            Hide();
            UIMgr.OpenNameCertifyPopup();
        });

        EventDelegate.Set(BtnFreefight.onClick, OnClickGotoDogFightPanel);//보류 추후에 살려주자.
    }

    /// <summary>
    /// 미션 리스트 생성
    /// 이부분은 추후에 다시 바꿔야함. 보유하고있는 미션 수만큼 오브젝트 생성하도록
    /// </summary>
    public void MissionListSetting(object arg = null)
    {
        if (BtnQuest == null || BtnQuest[0] == null)
            return;

        Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
        TownState town = SceneManager.instance.GetState<TownState>();
        
        if (quest != null && quest.LimitLevel <= NetData.instance.UserLevel)
        {
            BtnQuest[0].gameObject.SetActive(true);
            BtnQuest[0].transform.FindChild("open").gameObject.SetActive(true);
            BtnQuest[0].transform.FindChild("lock").gameObject.SetActive(false);
            BtnQuest[0].GetComponent<MissionListObject>().SetupMission(quest.Title, quest.LeftDescription);
            BtnQuest[0].transform.FindChild("open/Lv").GetComponent<UILabel>().text = string.Format("{0}.{1}", _LowDataMgr.instance.GetStringCommon(14), quest.LimitLevel);
            
            //던전입장일때만.
            if(!AlramMark[(int)AlramIconType.SINGLE].activeSelf)
            {
                if (quest.QuestType == 1 || quest.QuestType == 2)
                {
                    SceneManager.instance.SetAlram(AlramIconType.SINGLE, true);
                    AlramMark[(int)AlramIconType.SINGLE].SetActive(true);
                }
                else
                {
                    SceneManager.instance.SetAlram(AlramIconType.SINGLE, false);
                    AlramMark[(int)AlramIconType.SINGLE].SetActive(false);
                }
            }

            CurQuestInfo = quest;
            SearchTargetPath(false);
        }
        else
        {
            Quest.QuestInfo nextInfo = QuestManager.instance.GetCurNextQuestInfo();
            if (nextInfo != null && NetData.instance.UserLevel < nextInfo.LimitLevel )
            {
                BtnQuest[0].gameObject.SetActive(true);
                BtnQuest[0].transform.FindChild("open").gameObject.SetActive(false);
                BtnQuest[0].transform.FindChild("lock").gameObject.SetActive(true);

                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1180), nextInfo.LimitLevel );
                BtnQuest[0].GetComponent<MissionListObject>().SetLock(msg);
            }
            else
                BtnQuest[0].gameObject.SetActive(false);

            CurQuestInfo = null;
        }


    }

    #region 버튼 EventDelegate 모음

    /// <summary>
    /// vip 버튼 클릭. Vip 패널을 호출한다.
    /// </summary>
    void VipClick()
    {
    }
    /// <summary>
    /// 에너지 추가 클릭. 에너지 구매창으로 연결한다.
    /// </summary>
    void FoodAddClick()
    {
        uiMgr.AddPopup(141, 174, 117);
    }
    /// <summary>
    /// 금화 추가 클릭. 금화 구매창으로 연결한다.
    /// </summary>
    void GoldAddClick()
    {
        uiMgr.AddPopup(141, 174, 117);
    }
    /// <summary>
    /// 캐시 추가 클릭. 캐시 구매창으로 연결한다.
    /// </summary>
    void CashAddClick()
    {
        uiMgr.AddPopup(141, 174, 117);
    }
    /*
    /// <summary>
    /// 파워업 클릭. 강해지는 방법들로 연결해주는 창을 호출한다.
    /// </summary>
    void PowerupClick()
    {
        GameObject AttLabel = BtnPowerUp.transform.FindChild("attLabel").gameObject;
        if (!AttLabel.activeSelf)
            return;

        if (IsHideTown)
            return;

        // 인벤토리로 연결
        Hide();
        UIMgr.OpenEquipPanel(false);
    }
    */
    /// <summary>
    /// 퀘스트 클릭시 실행한다 Npc에게 달려가게 함
    /// 현재는 가라이기 때문에 할 것이 없다 추후 수정필요
    /// </summary>
    void QuestClick()
    {
        Quest.QuestInfo quest = QuestManager.instance.GetCurrentQuest();
        if (quest != null)
        {
            if (NetData.instance.UserLevel < quest.LimitLevel)//진행 불가 퀘스트
            {
                string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1026), quest.LimitLevel);//레벨부족 에러 팝업
                uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141)
                    , msg
                    , _LowDataMgr.instance.GetStringCommon(117) );
            }
            else if (quest.QuestType == (byte)QuestSubType.SINGLEGAMEPLAY || quest.QuestType == (byte)QuestSubType.SINGLEGAMECLEAR)
            {
                //이건 포탈로 가야함
                MyUnit.RunToPotal(QuestManager.instance.GetCurrentQuest().ParamId);
            }
            else
            {
                //해당 NPC로 가야함
                MyUnit.ResetMoveTarget();
                MyUnit.RunToCurrentQuestNpc();
            }
        }
        else
        {
            quest = QuestManager.instance.GetCurNextQuestInfo();
            if(quest != null)
            {
                if (NetData.instance.UserLevel < quest.LimitLevel)//진행 불가 퀘스트
                {
                    string msg = string.Format(_LowDataMgr.instance.GetStringCommon(1026), quest.LimitLevel);//레벨부족 에러 팝업
                    uiMgr.AddPopup(_LowDataMgr.instance.GetStringCommon(141)
                        , msg
                        , _LowDataMgr.instance.GetStringCommon(117));
                }
            }
        }
    }

    /// <summary>
    /// 우측상단 버튼그룹 온오프 버튼을 클릭. 상단버튼그룹 보여줬다 숨겨줬다 한다.
    /// </summary>
    void TopShowAndHideClick()
    {
    }

    /// <summary>
    /// 하단의 왼쪽 슬라이딩 객체의 친구 버튼
    /// 친구 페널로 이동해 준다.
    /// </summary>
    void OnClickGotoFriendPanel()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenSocialPanel(0);
    }
    /// <summary>
    /// 하단의 왼쪽 슬라이딩 객체의 상점 버튼
    /// 상점 페널로 이동해 준다.
    /// </summary>
    void OnClickGotoShopPanel()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenShopPanel();
    }


    /// <summary>
    /// 하단의 왼쪽 슬라이딩 객체의 영웅 버튼
    /// 인벤토리로 이동해 준다.
    /// </summary>
    void OnClickGotoHeroPanel()
    {
        if (IsHideTown)
            return;


        Hide();
        UIMgr.OpenEquipPanel(null);
    }

    /// <summary> 재화인벤 </summary>
    void OnClickCategory()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenCategoryPanel(this);
    }

    /// <summary> Vip팝업으로 이동 </summary>
    void OnClickGotoVip()
    {
        //if (IsHideTown)
        //    return;

        //Hide();
        //UIMgr.OpenVipPopup();
    }

    /// <summary>
    /// 하단의 왼쪽 슬라이딩 객체의 파트너 버튼
    /// 파트너 페널로 이동한다.
    /// </summary>
    void OnClickGotoPartnerPanel()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenPartner(true);
    }

    /// <summary> 길드 페널로 이동. </summary>
    void OnClickGotoGuild()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenGuildPanel();
    }

    /// <summary>
    /// 맵선택화면으로 이동한다.
    /// </summary>
    void OnClickGotoChapterPanel()
    {
        if (IsHideTown)
            return;

        //uint stageId = SceneManager.instance.GetState<TownState>().MyHero.moveTargetStage;
        //Hide();
        //UIMgr.OpenChapter(stageId);

        //Npc로이동
        int npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.SINGLE_NPC);

        if (npcID != int.MaxValue)
        {
            MyUnit.ResetMoveTarget();
            MyUnit.RunToNPC((uint)npcID);
        }

    }
    /*
    /// <summary>
    /// 코스튬 페널로 이동한다
    /// </summary>
    void OnClickGotoCotumePanel()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenCostume();
    }
    */
    /// <summary>
    /// 난투장 페널로 이동
    /// </summary>
    void OnClickGotoDogFightPanel()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenDogFight();
    }

    /// <summary> 던전 모여있는곳 </summary>
    void OnClickDungeonPanelPanel()
    {
        if (IsHideTown)
            return;
        
        Hide();
        UIMgr.OpenDungeonPanel();
        //int npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.SPECIAL_NPC);

        //if (npcID != int.MaxValue)
        //{
        //    SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
        //    SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
        //}
    }

    /// <summary> 결투장 </summary>
    void OnClickArena()
    {
        uiMgr.AddPopup(141, 174, 117);
        MyUnit.ResetMoveTarget();
        /*
        int npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.ARENA_NPC);

        if (npcID != int.MaxValue)
        {
            SceneManager.instance.GetState<TownState>().MyHero.ResetMoveTarget();
            SceneManager.instance.GetState<TownState>().MyHero.RunToNPC((uint)npcID);
        }
        */
    }

    /// <summary> 메일 팝업 오픈 </summary>
    void OnClickOpenMailPopup()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenMailPopup();
    }

    /// <summary> 마계의탑 오픈 </summary>
    void OnClickTower()
    {
        int npcID = TownNpcMgr.instance.GetTownNPC(NPCTYPE.TOWER_NPC);

        if (npcID != int.MaxValue)
        {
            MyUnit.ResetMoveTarget();
            MyUnit.RunToNPC((uint)npcID);
        }
    }

    /// <summary> 옵션 팝업 </summary>
    void OnClickOption()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenOptionPanel(false, 0);
    }

    /// <summary> 랭킹 팝업 </summary>
    void OnClickRank()
    {
        if (IsHideTown)
            return;


        Hide();
        UIMgr.OpenRankPanel();
    }

    /// <summary> 업적패널 </summary>
    public void OnClickAchieve()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenAchievePanel(this, 0);
    }
    /// <summary> 혜택패널 </summary>
    void OnClickBenefit()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenBenefitpanel();
    }

    /// <summary> 퀘스트 오픈 </summary>
    void OnClickOpenQuestPopup()
    {
        if (IsHideTown)
            return;

        Hide();
        UIMgr.OpenQuestPopup();
    }

    #endregion //Button Event Delegate

    /// <summary>
    /// 외부에서 사용할 함수 
    /// </summary>
    public void OpenChapter()
    {
        uint stageId = MyUnit.moveTargetStage;

        Hide();
        UIMgr.OpenChapter(null, stageId);
    }

    public override void Hide()
    {
        base.Hide();
        ChatPop.Hide();
        Map.Hide();
        Joystick.SetJoyActive(false);

        IsHideTown = true;
        MyUnit.ResetMoveTarget();

    }

    public override void Close()
    {
        base.Close();
        //ChatPop.Close();
    }

    public override bool Quit()
    {
        if (ChatPop.IsHideBigPop)
            return false;

        return true;
    }
    /// <summary>
    /// TownPanel이 켜질때 사용할 함수
    /// </summary>
    public void OnShow()
    {
        //TownState에 TownUnit이 있어서 접근을 해서 스킨을 바꿔준다. 추후 나은 방식이 있다면 변경해야한다
        TownState town = SceneManager.instance.GetState<TownState>();
        if(town.MyHero.ChangeSkin() )
            CreatePcFaceObj();

        //ChatPop.OnShow();
        Map.OnShow();

        IsHideTown = false;

        Joystick.SetJoyActive(true);
    }
    
    void LateUpdate()
    {
        /*
#if UNITY_EDITOR
        //잠시 업적알림
        if(Input.GetKeyDown(KeyCode.H))
        {
            UIMgr.instance.AchieveMentClearFlag = true;
            UIMgr.instance.AchieveType = 1;
            UIMgr.instance.AchieveSubType = 1;
            UIMgr.instance.AchieveLv = 1;

            SceneManager.instance.AddPopData(PopType.Achievement);//UI
        }

#endif
        */
        if (Zoom != null )
            Zoom.ZoomUpdate(Joystick);

        SearchTargetPath(true);
    }





    public void CreateHeadObjet(GameObject go, string nick, uint prefix, uint suffix, bool isMy)
    {
        GameObject obj = Instantiate(HeadObject) as GameObject;
        Transform objTf = obj.transform;
        objTf.parent = HeadParent;
        objTf.localPosition = Vector3.zero;
        objTf.localScale = Vector3.one;

        HeadObject head = objTf.GetComponent<HeadObject>();

        // MyTownUnit일경우 색상처리해줘야함
        if (isMy)
        {
            head.NickName.color = SetVipColor(CharInven._VipLevel);
        }


        head.ShowOnlyNickName(go, nick, prefix, suffix, isMy);
    }

    public void ChangeHeadStateInTown(byte state)
    {
        char[] showNameArr = SceneManager.instance.optionData.ShowName.ToCharArray();

        if (state == 1)
        {
            int childCount = HeadParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                HeadObject headObj = HeadParent.GetChild(i).GetComponent<HeadObject>();
                if (headObj.GetOwnerUID() != CharInven._charUUID)
                    continue;

                headObj.ShowNickInTown(showNameArr[1].Equals('1') ? false : true);
                break;
            }
        }
        else
        {
            int childCount = HeadParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                HeadObject headObj = HeadParent.GetChild(i).GetComponent<HeadObject>();

                if (state == 2)
                {
                    if (headObj.GetOwnerUID() == CharInven._charUUID)
                        continue;
                }

                headObj.ShowNickInTown(showNameArr[2].Equals('1') ? false : true);
            }
        }

    }

    public void ChangeUnitTitle(ulong charUID, uint pre, uint suf)
    {
        int childCount = HeadParent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            HeadObject headObj = HeadParent.GetChild(i).GetComponent<HeadObject>();
            if (headObj.GetOwnerUID() != charUID)
                continue;

            headObj.RefreshTitle(pre, suf);
            break;
        }
    }

    public void SetJoystickActive(bool isActive)
    {
        //ejoy.isActivated = isActive;
    }

    /// <summary> 마을에서 레벨업 </summary>
    public void LevelUp()
    {
        QuestInfo questInfo = QuestManager.instance.GetCurrentQuestInfo();
        if (questInfo == null)
        {
            Quest.QuestInfo nextQuest = QuestManager.instance.GetCurNextQuestInfo();
            if (nextQuest != null)
                QuestManager.instance.SearchNextQuest(nextQuest.BeforeQuestId);
        }
        else if (questInfo != null && questInfo.unComplete != 1)
        {
            QuestManager.instance.SearchNextQuest(questInfo.unTaskId);
        }

        RefreshUserInfo(1);
        CheckAlramMark();//레벨업했으니 한번더 체크

        CheckOpenContents(true);

        if (!IsHidePanel)
        {
            if (!SceneManager.instance.IsActiveNoticeType(NoticeType.Contents) )
                OnTutorial();
        }
    }

    public void SetUserTotalAtt(uint att)
    {
        //BattlePowerText.text = string.Format("{0} : {1}  {2}", _LowDataMgr.instance.GetStringCommon(47), att == 0 ? "0" : att.ToString("#,##"), _LowDataMgr.instance.GetStringCommon(98));
		BattlePowerText.text = string.Format ("{0} : {1}", _LowDataMgr.instance.GetStringCommon (47), att == 0 ? "0" : att.ToString ()); //att.ToString ("#,##"));
    }

    public void CreatePcFaceObj()
    {
        if (CharInven.GetEquipCostume() == null)
            return;

        uint weaponId = 0, clothId = 0, headId = 0;
        if (CharInven.isHideCostum)
        {
            NetData._ItemData head = CharInven.GetEquipParts(ePartType.HELMET);
            NetData._ItemData cloth = CharInven.GetEquipParts(ePartType.CLOTH);
            NetData._ItemData weapon = CharInven.GetEquipParts(ePartType.WEAPON);

            if (head != null)
                headId = head._equipitemDataIndex;

            if (cloth != null)
                clothId = cloth._equipitemDataIndex;

            if (weapon != null)
                weaponId = weapon._equipitemDataIndex;
        }
        
        Dictionary<AbilityType, float> abilityDic = NetData.instance.CalcPlayerStats();//체력
        HPText.text = ((int)abilityDic[AbilityType.HP]).ToString();
        UIHelper.CreatePcUIModel("TownPanel", FaceParent, CharInven.GetCharIdx(), headId, CharInven.GetEquipCostume()._costmeDataIndex, clothId, weaponId, CharInven.GetEquipSKillSet().SkillSetId, 3, CharInven.isHideCostum, false, false);


    }

    public bool CheckOpenContents(bool isLvUp)
    {
        bool[] check = new bool[(int)OpenContentsType.Max];
        for(int i=0; i < check.Length; i++)
        {
            check[i] = true;
        }

        uint contentsType = 0;
        List<Quest.MainTutorialInfo> tutoList = _LowDataMgr.instance.GetLowDataMainTutorialList(1);
        for(int i=0; i < tutoList.Count; i++)
        {
            if (tutoList[i].OpenType == 0)//일반연출 무시
                continue;

            if (NetData.instance.UserLevel < tutoList[i].OpenLevel)//레벨 낮으면 무시 어차피 못들어가니깐.
            {
                check[tutoList[i].OpenType] = false;
                continue;
            }

            if(0 < tutoList[i].OpenType && NetData.instance.UserLevel == tutoList[i].OpenLevel)
            {
                if (isLvUp)//레벨업으로 인해 컨텐츠가 오픈됨.
                    contentsType = tutoList[i].OpenType;
                else if (!SceneManager.instance.IsActiveNoticeType(NoticeType.Contents))//현재 진행해야하는 튜토리얼의 시작점이 오픈컨텐츠 연출 후 인지.
                {
                    switch(SceneManager.instance.CurTutorial)//현재 진행하는것을 기준으로 체크함
                    {
                        case TutorialType.ACHIEVE://업적확인
                            if(tutoList[i].OpenType == (int)OpenContentsType.Achiev)
                                contentsType = tutoList[i].OpenType;
                            break;
                    }
                }
            }

            //if ((TutorialType)tutoList[i].Group <= SceneManager.instance.CurTutorial)
                check[tutoList[i].OpenType] = true;//조건 충족
            //else
            //    check[tutoList[i].OpenType] = false;
        }

        BtnDungeon.gameObject.SetActive(check[(int)OpenContentsType.Dungeon]);
        BtnFreefight.gameObject.SetActive(check[(int)OpenContentsType.FreeFight]);
        BtnRank.gameObject.SetActive(check[(int)OpenContentsType.Rank]);//랭킹
        BtnAchieve.gameObject.SetActive(check[(int)OpenContentsType.Achiev]);//업적
        BtnBenefit.gameObject.SetActive(check[(int)OpenContentsType.Benefit]);//혜택
        BtnStory.gameObject.SetActive(check[(int)OpenContentsType.Chapter]);//모험모드

        //BottomLeftBtns[0].gameObject.SetActive(check[(int)OpenContentsType.que]);//퀘스트 팝업
        BottomLeftBtns[1].gameObject.SetActive(check[(int)OpenContentsType.Guilde]);//길드
        BottomLeftBtns[2].gameObject.SetActive(check[(int)OpenContentsType.Partner]);//파트너
        BottomLeftBtns[3].gameObject.SetActive(check[(int)OpenContentsType.Char]);//캐릭터
        BottomLeftBtns[4].gameObject.SetActive(check[(int)OpenContentsType.Category]);//재화인벤
        BottomLeftBtns[5].gameObject.SetActive(check[(int)OpenContentsType.Social]);//소셜
        BottomLeftBtns[6].gameObject.SetActive(check[(int)OpenContentsType.Shop]);//상점

        if (0 < contentsType)
            SceneManager.instance.SetNoticePanel(NoticeType.Contents, contentsType);

        return 0 < contentsType;
    }

    void OnGUI()
    {
        //if(GUI.Button(new Rect(200, 100, 100, 50), "콜로세움)") )
        //{
        //    UIMgr.OpenColosseumPanel();
        //    Hide();
        //}

        //#if UNITY_EDITOR
        //        TownState ts = SceneManager.instance.CurrentStateBase() as TownState;
        //        if (ts != null && ts.MyHero != null)
        //        {
        //            GUI.color = Color.black;
        //            GUI.Label(new Rect(500, 50, 300, 200), string.Format("ActiveJoystick - {0}, mouseposition - {1}, {2}", ActiveJoystick, Input.mousePosition, ActiveJoystick));
        //            GUI.Label(new Rect(500, 100, 300, 200), string.Format("LeaderState:{0}", ts.MyHero.CurrentState));
        //        }
        //#endif
    }

	private void setTestBtns(){
		Transform testBtnGroup = transform.FindChild("TestBtnGroup");
		if (testBtnGroup != null)
		{
			if (Debug.isDebugBuild)
			{
				
				Transform btnSceneTest = testBtnGroup.FindChild("BtnSceneTest");
				Transform testTf = transform.FindChild("BtnTest");
				btnSceneTest.gameObject.SetActive(true);
				UIButton btn = btnSceneTest.GetComponent<UIButton>();
				EventDelegate.Set(btn.onClick, () =>
				                  {
					string path = "UIPopup/CheatPopup";
					GameObject chatPop = UIMgr.Open(path, true);
				});
				
				Transform btnSingleTest = testBtnGroup.FindChild("BtnSingleTest");
				if (btnSingleTest != null){
					btnSingleTest.gameObject.SetActive(true);
					UIButton btn2 = btnSingleTest.GetComponent<UIButton>();
					EventDelegate.Set(btn2.onClick, () =>
					                  {
						if (SceneManager.instance.testData.bSingleSceneTestStart == false){
							SceneManager.instance.testData.bSingleSceneTestStart = true;
							startSingleTest();
							btnSingleTest.transform.GetComponentInChildren<UILabel>().text = "single_test ON";
						}
						else{
							SceneManager.instance.testData.bSingleSceneTestStart = false;
							btnSingleTest.transform.GetComponentInChildren<UILabel>().text = "single_test OFF";
						}
					});
				}
				
				Transform btnQuestTest = testBtnGroup.FindChild("BtnQuestTest");
				if (btnQuestTest != null){
					btnQuestTest.gameObject.SetActive(true);
					UIButton btn2 = btnQuestTest.GetComponent<UIButton>();
					EventDelegate.Set(btn2.onClick, () =>
					                  {
						if (SceneManager.instance.testData.bQuestTestStart == false){
							SceneManager.instance.testData.bQuestTestStart = true;
							QuestClick();
							btnQuestTest.transform.GetComponentInChildren<UILabel>().text = "Quest_test ON";
						}
						else{ 
							SceneManager.instance.testData.bQuestTestStart = false;
							btnQuestTest.transform.GetComponentInChildren<UILabel>().text = "Quest_test OFF";
						}

					});
				}
				Transform btnCutSceneTest = testBtnGroup.FindChild("BtnCutSceneTest");
				if (btnCutSceneTest != null){
					btnCutSceneTest.gameObject.SetActive(true);

					if (SceneManager.instance.testData.bCutSceneTest){
						btnCutSceneTest.transform.GetComponentInChildren<UILabel>().text = "CutScene On";
					}
					else{
						btnCutSceneTest.transform.GetComponentInChildren<UILabel>().text = "CutScene Off";
					}

					UIButton btn2 = btnCutSceneTest.GetComponent<UIButton>();
					EventDelegate.Set(btn2.onClick, () =>
					{
						SceneManager.instance.testData.bCutSceneTest = !SceneManager.instance.testData.bCutSceneTest;
						if (SceneManager.instance.testData.bCutSceneTest){
							btnCutSceneTest.transform.GetComponentInChildren<UILabel>().text = "CutScene On";
						}
						else{
							btnCutSceneTest.transform.GetComponentInChildren<UILabel>().text = "CutScene Off";
						}
					});
				}

				Transform btnPanelTest1 = testBtnGroup.FindChild("BtnPanelTest1");
				if (btnPanelTest1 != null){
					UIButton btn2 = btnPanelTest1.GetComponent<UIButton>();
					EventDelegate.Set(btn2.onClick, () =>
					{
						Hide();
						UIMgr.OpenPanelTest1Panel();
					});
				}
				Transform btnPanelTest2 = testBtnGroup.FindChild("BtnPanelTest2");
				if (btnPanelTest2 != null){
					UIButton btn2 = btnPanelTest2.GetComponent<UIButton>();
					EventDelegate.Set(btn2.onClick, () =>
					{
						UIMgr.OpenPanelTest2Panel();
					});
				}
				/*
                Transform btnArena = testBtnGroup.FindChild("BtnTestArena");
                if (btnArena != null)
                {
                    btnArena.gameObject.SetActive(true);

                    EventDelegate.Set(btnArena.GetComponent<UIButton>().onClick, delegate () {
                        SceneManager.instance.ShowLoadingTipPanel(true, GAME_MODE.ARENA, () =>
                        {
                            //이상태에서의 데이터를 저장
                            NetData.instance.MakePlayerSyncData(true);
                            SceneManager.instance.ActionEvent(_ACTION.PLAY_ARENA);
                            base.GotoInGame();
                        });
                    });
                }
                */
				if (testTf != null)
				{
					testTf.gameObject.SetActive(true);
					EventDelegate.Set(testTf.GetComponent<UIButton>().onClick, delegate ()
					                  {
						SceneManager.instance.TutorialAction();
					});
				}
			}
			else
			{
				testBtnGroup.gameObject.SetActive(false);
			}
		}
	}

    void SearchTargetPath(bool isCheckState)
    {
        //if (!PathArrowTf.gameObject.activeSelf)
        //    return;
        if (MyUnit == null)
            return;

        if (CurQuestInfo == null)
        {
            PathArrowTf.gameObject.SetActive(false);
            return;
        }

        if (isCheckState && MyUnit.CurrentState != UnitState.Move)
            return;
        
        uint param = 0;
        if (CurQuestInfo.QuestType == 0)//npc 대화
            param = CurQuestInfo.ParamId;
        else
            param = (uint)TownNpcMgr.instance.GetTownNPC(NPCTYPE.SINGLE_NPC);

        Vector3 startPos = MyUnit.transform.position;
        Vector3 endPos = TownNpcMgr.instance.GetNpcPosition(TownRunTargetType.Npc, param);

        NavMeshPath movePath = new NavMeshPath();
        if (!NavMesh.CalculatePath(startPos, endPos, -1, movePath) || movePath.corners.Length <= 1)
        {
            if (PathArrowTf.gameObject.activeSelf)
                PathArrowTf.gameObject.SetActive(false);

            return;
        }

        // 시작점과 끝점은 계산에서 제외시킴
        NavMeshHit navHit;
        for (int i = 1; i < movePath.corners.Length - 1; i++)
        {
            // 찾아진 패스에 대해서 가장가까운 Edge를 검사해서 너무 가까우면, 거리를 벌리도록 함.
            if (NavMesh.FindClosestEdge(movePath.corners[i], out navHit, 1))
            {
                if ((navHit.position - movePath.corners[i]).sqrMagnitude < 1f)
                {
                    movePath.corners[i] = movePath.corners[i] + navHit.normal * 1f;
                }
            }
        }

        // i>=2 인 이유는 바로 앞에 코너일 수 있으니 바로 앞은 살려 두도록한다
        if (movePath.corners.Length >= 2)
        {
            // 다음 포인트랑 거리가 가까우면 위치를 이동시킨다
            for (int i = 1; i < movePath.corners.Length - 1; ++i)
            {
                if ((movePath.corners[i] - movePath.corners[i + 1]).sqrMagnitude < 2f)
                    movePath.corners[i + 1] = movePath.corners[i];
            }
        }

        if (Vector3.Distance(startPos, movePath.corners[movePath.corners.Length - 1]) <= 3f)
        {
            if (PathArrowTf.gameObject.activeSelf)
                PathArrowTf.gameObject.SetActive(false);

            return;
        }

        int arr = 0;
        if (1 < movePath.corners.Length)
            arr = 1;

        endPos = movePath.corners[arr];
        if (!PathArrowTf.gameObject.activeSelf)
            PathArrowTf.gameObject.SetActive(true);

        var newRotation = Quaternion.LookRotation(endPos-startPos).eulerAngles;
        //var newRotation = Quaternion.Euler(startPos-endPos).eulerAngles;

        //newRotation.z = newRotation.y- PathOffset;
        newRotation.z = PathOffset-newRotation.y;
        newRotation.x = 0;
        newRotation.y = 0;
        PathArrowTf.eulerAngles = newRotation;
        //PathArrowTf.localEulerAngles = Vector3.Lerp(PathArrowTf.eulerAngles, newRotation, Time.deltaTime);
    }

    bool OnTutorial()
    {
        if (SceneManager.instance.CurTutorial == TutorialType.ALL_CLEAR)
            return false;

        if (TutoSupportList == null || TutoSupportList.Count <= 0)
            TutoSupportList = UIHelper.FindComponents<TutorialSupport>(transform);

        if (TutoSupportList.Count <= 0)
            return false;

        int tutoType = 1;
        int arr = 0;
        while (tutoType < (int)TutorialType.MAX)
        {
            if (TutoSupportList.Count <= arr)
            {
                arr = 0;
                ++tutoType;
            }
            TutorialSupport support = TutoSupportList[arr++];

            if (support.TutoType == TutorialType.STATUS ||
                support.TutoType == TutorialType.SHOP ||
                support.TutoType == TutorialType.GACHA ||
                support.TutoType == TutorialType.CHAR_SKILL)
                continue;

            if (support.TutoType != (TutorialType)tutoType)
            {
                support.ChangeTutoType();
                if (support.TutoType != (TutorialType)tutoType)
                    continue;
            }
            else if (SceneManager.instance.IsClearTutorial(support.TutoType))
            {
                support.ChangeTutoType();
                if (SceneManager.instance.IsClearTutorial(support.TutoType))
                    continue;
            }

            Quest.MainTutorialInfo tutoLowData = _LowDataMgr.instance.GetLowDataFirstMainTutorial((uint)support.TutoType, 1);
            if (tutoLowData == null)
                continue;

            if (NetData.instance.UserLevel < tutoLowData.OpenLevel || !support.CheckTuto)//체크
                continue;

            if (support.OnTutoSupportStart())
            {
                SceneManager.instance.SetTutoType(support.TutoType);
                return true;
            }
            else
                Debug.LogError("SPSPSPSPSPSP");
        }

        return false;
    }
    /*
	IEnumerator LoadAndPlayCutScene(){

		SceneManager.instance.ShowNetProcess("maintown_cutscene");
		
		AsyncOperation async = Application.LoadLevelAdditiveAsync("maintown_cutscene");
		
		async.allowSceneActivation = false;
		
		while (async.progress < 0.9f)
		{
			yield return null;
		}
		
		async.allowSceneActivation = true;
		
		while (!async.isDone)
			yield return null;
		
		yield return new WaitForEndOfFrame();
		
		yield return new WaitForSeconds(0.1f);
		
		SceneManager.instance.EndNetProcess("maintown_cutscene");
		
		string[] CharObjNamesForInactive = new string[2];
		
		if (NetData.instance.GetUserInfo ()._userCharIndex == 11000) {
			CharObjNamesForInactive[0] = "pc_p_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 12000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_d_cutscene_01";
		}
		else if (NetData.instance.GetUserInfo ()._userCharIndex == 13000) {
			CharObjNamesForInactive[0] = "pc_f_cutscene_01";
			CharObjNamesForInactive[1] = "pc_p_cutscene_01";
		}
		
		if (GameObject.Find ("TownCutSceneManager")) {
			TownCutSceneManager tcm = GameObject.Find ("TownCutSceneManager").GetComponent<TownCutSceneManager> (); 
			tcm.InActiveObj(CharObjNamesForInactive);
			tcm.playSeq ( ()=>{
				OnLevelWasLoadedPart2();
			});
		}
	}
    */
}

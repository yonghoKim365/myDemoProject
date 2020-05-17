using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//1. 달성률 표시(동시에 달성률 미션 리스트도 같이 나옴)
//2. 다음 버튼을 누르면 캐릭터와 파트너 모델등과 함께 경험치 획득량과 아이템 획득 목록 연출 보여줌
//3. 모험선택화면으로 가거나 마을로 가긱 버튼을 클릭해 해당 장소로 이동
/// <summary>
/// 모험 결과 성공/실패 화면(모험 달성률(★) 표시 및 영웅,파트너 보여주고 획득 아이템 보여주는 화면)
/// </summary>
public class ResultRewardStarPanel : UIBasePanel
{
    enum groupT { star = 0, mission, rigthBtns, HnP_model/*, partner0, partner1*/, DropItem }// Groups 배열에 인덱스로 사용될 타입
    public GameObject[] Groups;// 키고 끄고 해야할 그룹들
    public GameObject[] StarEffs;//별 이펙트들

    enum iconT { tstar0 = 0, tstar1, tstar2, /*mstar0, mstar1, mstar2*/ }// Icons 배열의 인덱스로 사용될 타입
    public UISprite[] Icons;// 스프라이트들.

  //  public Transform HeroRoot, Partner0Root, Partner1Root;// 히어로, 파트너 모델 루트
    public Transform RewardAssetTf;
    public Transform ArenaView;
    public Transform TowerPop;

    public GameObject FirstRewardPopup;

    public GameObject FirstRewardSlotLabel;
    public GameObject BasicRewardSlotLabel;


    enum RewardItemType//6개가 있다. 이보다 많다면 추가가 되어야 함.
    {
        Root_0, Root_1, Root_2, Root_3, Root_4, Root_5
    }

    public UIGrid RewardGrid;

    public UISlider ExpGauge;// 영웅 경험치 게이지
    //public UISprite TitleSprite;// 승리/패배 스프라이트
    public GameObject[] TitleObj;// 승리/패배 스프라이트

    /// <summary> Labels 배열에 인덱스로 사용될 타입 </summary>
    enum labelT { VnD = 0, mission0, mission1, mission2, hLv, p0Lv, p1Lv, p0Name, p1Name }
    public UILabel[] Labels;// 라벨들

    public UILabel RewardGold;
    public UILabel RewardExp;

    enum btnT { goTownBtn = 0, goStageBtn, nextZone, clearQuest }//Btns 배열에 인덱스로 사용될 타입
    public UIButton[] Btns;//버튼들

    public GameObject ItemSlotPrefab;//아이템 슬롯
    
    public UILabel[] LeftName;
    public UILabel[] LeftDamage;
    public UILabel[] LeftScore;
    public UILabel[] RightName;
    public UILabel[] RightDamage;
    public UILabel[] RightScore;

    public UISprite[] LeftFace;
    public UISprite[] RightFace;

    private NetData.RewardData Reward;
    private bool isSuccess;
    private bool IsIgnoreBtn = true;

    //uint stageId;

    public override void Init()
    {
        base.Init();
        BtnSetting();

        for (int i = 0; i < Icons.Length; i++)
        {
            Icons[i].enabled = false;

            if (i < StarEffs.Length)
                StarEffs[i].SetActive(false);
        }

        //Groups[(int)groupT.star].SetActive(false);
        Groups[(int)groupT.mission].SetActive(false);
        Groups[(int)groupT.DropItem].SetActive(false);

        ///꺼놓자.
        TitleObj[0].SetActive(false);
        TitleObj[1].SetActive(false);
        ArenaView.gameObject.SetActive(GAME_MODE.ARENA == G_GameInfo.GameMode );
        TowerPop.gameObject.SetActive(false);

        FirstRewardPopup.SetActive(false);

        UIHelper.CreateEffectInGame(FirstRewardPopup.transform.FindChild("Effect"), "Fx_UI_contents_alarm");

    }

    void BtnSetting()
    {
        EventDelegate.Set(Btns[(int)btnT.nextZone].onClick, delegate () { OnClickGoStage(UI_OPEN_TYPE.NEXT_ZONE); });
        UIEventListener.Get(Btns[(int)btnT.goTownBtn].gameObject).onClick = GotoTown;
        EventDelegate.Set(Btns[(int)btnT.goStageBtn].onClick, delegate () { OnClickGoStage(UI_OPEN_TYPE.NONE); });
    }

    public override void LateInit()
    {
        base.LateInit();
        //파라메터로 넘어오는 데이터 세팅
        isSuccess = (bool)parameters[0];
        GAME_MODE gameMode = G_GameInfo.GameMode;
        if (gameMode == GAME_MODE.SINGLE && (G_GameInfo.GameInfo as SingleGameInfo).IsQuestClear)//이 경우 마을가기만 존재함
        {
            Btns[(int)btnT.nextZone].gameObject.SetActive(false);
            Btns[(int)btnT.goStageBtn].gameObject.SetActive(false);
            Btns[(int)btnT.goTownBtn].gameObject.SetActive(false);

            Btns[(int)btnT.clearQuest].gameObject.SetActive(true);
            EventDelegate.Set(Btns[(int)btnT.clearQuest].onClick, delegate() { GotoTown(null); });
        }
        else//게임모드에 따라서 버튼 추가
        {
            if (gameMode == GAME_MODE.SINGLE || gameMode == GAME_MODE.TOWER)
                Btns[(int)btnT.nextZone].gameObject.SetActive(isSuccess);  //승리했을때만 다음스테이지가 나와야한다?
            else
                Btns[(int)btnT.nextZone].gameObject.SetActive(false);
            
            uint key = 0;
            if (gameMode == GAME_MODE.TOWER)
                key = 540;
            else
                key = 531;

            Btns[(int)btnT.nextZone].transform.FindChild("label").GetComponent<UILabel>().text = _LowDataMgr.instance.GetStringCommon(key);
            Btns[(int)btnT.clearQuest].gameObject.SetActive(false);
        }
        
        RewardAssetTf.parent.gameObject.SetActive(false);
        ResultDungeon();

        if (gameMode == GAME_MODE.ARENA)//등급 상승 or 하락 연출
        {
            //성망획득량.. 승/패와관려없이 나옴
            GameObject go = Instantiate(ItemSlotPrefab) as GameObject;
            Transform tf = go.transform;
            tf.parent = ArenaView.FindChild("RankInfo/item").transform;
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;

            InvenItemSlotObject slotObj = go.GetComponent<InvenItemSlotObject>();
            slotObj.SetLowDataItemSlot(599003, (uint)parameters[1]);

            ArenaView.FindChild("RankPop/BeforeNum").GetComponent<UILabel>().text = string.Format("{0}", ArenaGameState.MyRank - ArenaGameState.TargetRank);
            ArenaView.FindChild("RankPop/AfterNum-").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1267), (ArenaGameState.MyTopRank - ArenaGameState.TargetRank) * _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPfirstgetcash));
            
            StartCoroutine("TextAction", new object[] {
                ArenaGameState.MyRank,
                ArenaGameState.TargetRank,
                _LowDataMgr.instance.GetStringCommon(521),
                ArenaView.FindChild("RankPop/AfterNum").GetComponent<UILabel>()
            });
            if (isSuccess && ArenaGameState.TargetRank < ArenaGameState.MyTopRank)//등급 상승
            {
                ArenaView.FindChild("RankPop/BeforeNum").GetComponent<UILabel>().text = string.Format("{0}", ArenaGameState.MyRank - ArenaGameState.TargetRank);
                ArenaView.FindChild("RankPop/AfterNum-").GetComponent<UILabel>().text = string.Format(_LowDataMgr.instance.GetStringCommon(1267), (ArenaGameState.MyTopRank - ArenaGameState.TargetRank) * _LowDataMgr.instance.GetEtcTableValue<uint>(EtcID.PvPfirstgetcash));
                
                StartCoroutine("TextAction", new object[] {
                    ArenaGameState.MyRank,
                    ArenaGameState.TargetRank,
                    _LowDataMgr.instance.GetStringCommon(521),
                    ArenaView.FindChild("RankPop/AfterNum").GetComponent<UILabel>()
                });

                //최고랭킹달성원보
                GameObject gem = Instantiate(ItemSlotPrefab) as GameObject;
                Transform gemTf = gem.transform;
                gemTf.parent = ArenaView.FindChild("RankInfo/gem").transform;
                gemTf.localPosition = Vector3.zero;
                gemTf.localScale = Vector3.one;

                InvenItemSlotObject gemObj = gem.GetComponent<InvenItemSlotObject>();
                gemObj.SetLowDataItemSlot(599001, (uint)parameters[2]);





                EventDelegate.Set(ArenaView.FindChild("RankPop/BtnClose").GetComponent<UIEventTrigger>().onClick, () => {

                    if (ArenaView.FindChild("RankPop/bg").transform.childCount > 0)
                        DestroyImmediate(ArenaView.FindChild("RankPop/bg").transform.GetChild(0).gameObject);

                    ArenaView.FindChild("RankPop").gameObject.SetActive(false);

                    ArenaResult();

                    //이때켜준다
                    ArenaView.FindChild("RankInfo").gameObject.SetActive(true);
                    transform.FindChild("Normal").gameObject.SetActive(true);
                });
                
                ArenaView.FindChild("RankPop").gameObject.SetActive(true);
                ArenaView.FindChild("RankInfo").gameObject.SetActive(false);
                transform.FindChild("Normal").gameObject.SetActive(false);

                //이펙트 
                UIHelper.CreateEffectInGame(ArenaView.FindChild("RankPop/title").transform, "Fx_UI_result_01");
                TempCoroutine.instance.FrameDelay(1.5f, () => {
                    UIHelper.CreateEffectInGame(ArenaView.FindChild("RankPop/bg").transform, "Fx_UI_result_02");

                });

            }
            else
            {
                ArenaView.FindChild("RankPop").gameObject.SetActive(false);
                ArenaView.FindChild("RankInfo").gameObject.SetActive(true);

                transform.FindChild("Normal").gameObject.SetActive(true);

                ArenaResult();
            }

            ArenaView.gameObject.SetActive(true);
        }

		if (SceneManager.instance.testData.bSingleSceneTestStart) {
			TempCoroutine.instance.FrameDelay(2f, () => {
                GotoTown(null);
			});
		}
		if (SceneManager.instance.testData.bQuestTestStart) {
			TempCoroutine.instance.FrameDelay(2f, () => {
				GotoTown(null);
			});
		}
    }
    
    void ArenaResult()
    {
       
        int before = 0, after = 0;
        if (ArenaGameState.MyRank < ArenaGameState.TargetRank)//나보다 낮은 랭커
        {
            if (isSuccess)//나보다 낮은애 이겨봐야 동일함
            {
                ArenaView.FindChild("RankInfo/arrow").gameObject.SetActive(false);
                after = before = ArenaGameState.MyRank; 
            }
            else//나보다 낮은녀석 이겨봐야 득볼거 없다.
            {
                ArenaView.FindChild("RankInfo/arrow").transform.localEulerAngles = new Vector3(0, 0, 180);
                before = ArenaGameState.MyRank;
                after = ArenaGameState.TargetRank;
            }
        }//동일하면 버그임
        else//나보다 높음
        {
            if (!isSuccess)//나보다 높은 대상은 순위 변경 없음
            {
                ArenaView.FindChild("RankInfo/arrow").gameObject.SetActive(false);
                after = before = ArenaGameState.MyRank;
            }
            else//변경
            {
                ArenaView.FindChild("RankInfo/arrow").transform.localEulerAngles = Vector3.zero;
                before = ArenaGameState.MyRank;
                after = ArenaGameState.TargetRank;
            }
        }

        StartCoroutine("TextAction", new object[] {
                before,
                after,
                _LowDataMgr.instance.GetStringCommon(1020),
                ArenaView.FindChild("RankInfo/AfterNum").GetComponent<UILabel>()
            });

        ArenaView.FindChild("RankInfo/BeforeNum").GetComponent<UILabel>().text = string.Format("{0}", before - after);
        Result();
    }

    /// <summary> 비동기 방식의 게임들 결과화면 </summary>
    void ResultDungeon()
    {
        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
        {
            //현재 최초보상받을수있는지 체크?
            DungeonTable.StageInfo lowData = _LowDataMgr.instance.GetStageInfo(SingleGameState.lastSelectStageId);
            if (lowData != null)
            {
                //if (lowData.type == 2 && isSuccess)
               // {
                    if (SingleGameState.IsFirstReward)
                    {
                        GatchaReward.FixedRewardInfo firstInfo = _LowDataMgr.instance.GetFixedRewardItem(lowData.FirstReward);

                        //최초보상받을시기?
                        GameObject go = Instantiate(ItemSlotPrefab) as GameObject;
                        Transform tf = go.transform;
                        tf.parent = FirstRewardPopup.transform.FindChild("Icon").transform;
                        tf.localPosition = Vector3.zero;
                        tf.localScale = Vector3.one;

                        UILabel name = FirstRewardPopup.transform.FindChild("Txt_Iconname").GetComponent<UILabel>();

                    InvenItemSlotObject slotObj = go.GetComponent<InvenItemSlotObject>();
                    slotObj.SetLowDataItemSlot(firstInfo.ItemId == 0? firstInfo.Type : firstInfo.ItemId, firstInfo.ItemCount);

                    Item.EquipmentInfo equipInfo = _LowDataMgr.instance.GetLowDataEquipItemInfo(firstInfo.ItemId);
                    if (equipInfo != null) //장비
                        name.text = _LowDataMgr.instance.GetStringItem(equipInfo.NameId);
                    else
                    {
                        uint id = 0;
                        //혹시 재화인가....
                        if (firstInfo.ItemId == 0)
                        {
                            switch(firstInfo.Type)
                            {
                                case 1://골드
                                    id = 599000;
                                    break;
                                case 2://원보
                                    id = 599001;
                                    break;
                                case 3://공헌
                                    id = 599004;
                                    break;
                                case 4://명예
                                    id = 599005;
                                    break;
                                case 5://휘장
                                    id = 599002;
                                    break;
                                case 6://사자왕
                                    id = 599006;
                                    break;
                                case 7://성망
                                    id = 599003;
                                    break;
                                case 8://체력
                                    id = 599104;
                                    break;
                                case 9://경험치;
                                    id = 599105;
                                    break;

                            }

                            name.text = _LowDataMgr.instance.GetStringItem(_LowDataMgr.instance.GetUseItem(firstInfo.ItemId==0 ? id : firstInfo.ItemId).NameId);
                        }
                        else
                            name.text = _LowDataMgr.instance.GetStringItem(_LowDataMgr.instance.GetUseItem(firstInfo.ItemId).NameId);

                    }

                    SetRenderQueue renderQ = UIHelper.FindComponent<SetRenderQueue>(FirstRewardPopup.transform.FindChild("Effect"));
                    if (renderQ != null)
                        renderQ.ResetRenderQ(3031);

                        FirstRewardPopup.SetActive(true);

                        TempCoroutine.instance.FrameDelay(0.1f, () =>
                        {
                            FirstRewardPopup.transform.GetComponent<TweenScale>().ResetToBeginning();
                            FirstRewardPopup.transform.GetComponent<TweenScale>().PlayForward();
                        });

                        TempCoroutine.instance.FrameDelay(3f, () =>
                        {
                            FirstRewardPopup.SetActive(false);
                        });

                    }
                }
           // }

            if (isSuccess && SceneManager.instance.CurTutorial == TutorialType.STAGE)//튜토리얼 중 스테이지 클리어했다면
                SceneManager.instance.CurTutorial = TutorialType.STAGE;
        }

        Reward = NetData.instance._RewardData;
        if (Reward == null)//없으면 안돼겠지만. 뻑은 안나게..
            Reward = new NetData.RewardData();
        
        uint curExp = 0, maxExp = 0, level = 0;
        if (Reward.GetExp <= 0)//획득 경험치가 있다면 뭔가 데이터가 있다는 것임.
        {
            level = NetData.instance.UserLevel;
            NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref curExp, ref maxExp);
        }
        else
        {
            curExp = Reward.SaveExp;
            maxExp = Reward.SaveMaxExp;
            level = Reward.SaveLevel;
        }

        ExpGauge.value = (float)curExp / (float)maxExp;
        Labels[(int)labelT.hLv].text = string.Format(_LowDataMgr.instance.GetStringCommon(453), level);

        SettingRewardItem();
        
        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)//싱글게임에만 스테이지 퀘스트가 존재.
            StartCoroutine("MissionResult");
        else if (G_GameInfo.GameMode != GAME_MODE.ARENA)
            Result();

        if (isSuccess)
        {
            TitleObj[0].SetActive(true);
            UIHelper.CreateEffectInGame(TitleObj[0].transform.FindChild("bg"), "Fx_UI_exp_result_01");
        }
        else
            TitleObj[1].SetActive(true);
    }

    /// <summary>
    /// 별 등급 획득 연출 코루틴
    /// </summary>
    IEnumerator MissionResult()
    {
        Groups[(int)groupT.mission].SetActive(true);

        //퀘스트 목록 셋
        List<NetData.StageClearData> questList = SingleGameState.StageQuestList;
        Labels[(uint)labelT.mission0].text = questList[0].GetTypeString();
        Labels[(uint)labelT.mission1].text = questList[1].GetTypeString();
        Labels[(uint)labelT.mission2].text = questList[2].GetTypeString();

        //2. 달성한 목표 별 활성화 해주고 미션 리스트에서도 강조연출 넣어주고
        if (isSuccess)
        {
            for (int i = 0; i < questList.Count; i++)
            {
                if (!questList[i].IsClear)
                    continue;

                StarEffs[i].SetActive(true);

                SoundManager.instance.PlaySfxSound(eUISfx.UI_victory_star, false);
                yield return new WaitForSeconds(0.3f);
                Icons[i].enabled = true;
            }
        }

        //3. 끝나면 플레그 변환
        Result();
        yield return null;
    }

    /// <summary> 로비로 돌아가기 </summary>
    void GotoTown(GameObject go)
    {
        if (IsIgnoreBtn)
            return;

        NetData.instance.ClearRewardData();
        base.Close();

        TempCoroutine.instance.StopAllCoroutines();
        SceneManager.instance.LobbyActionEvent(_STATE.SINGLE_GAME, _ACTION.GO_TOWN, UI_OPEN_TYPE.NONE);
        StopAllCoroutines();
    }

    /// <summary> 모험지역 선택화면으로 돌아가기 </summary>
    void OnClickGoStage(UI_OPEN_TYPE openType)
    {
        if (IsIgnoreBtn)
            return;

        NetData.instance.ClearRewardData();
        base.Close();

        _STATE state;
        GAME_MODE mode = G_GameInfo.GameMode;
        if (mode == GAME_MODE.ARENA)
            state = _STATE.ARENA_GAME;
        else if (mode == GAME_MODE.FREEFIGHT)
        {
            if (FreeFightGameState.GameMode == GAME_MODE.MULTI_RAID)
                state = _STATE.MULTI_RAID;
            else if (FreeFightGameState.GameMode == GAME_MODE.COLOSSEUM)
                state = _STATE.COLOSSEUM;
            else
                state = _STATE.FREEFIGHT_GAME;
        }
        else if (mode == GAME_MODE.SPECIAL_EXP || mode == GAME_MODE.SPECIAL_GOLD)
            state = _STATE.SPECIAL_GAME;
        else if (mode == GAME_MODE.TOWER)
            state = _STATE.TOWER_GAME;
        else if (mode == GAME_MODE.RAID)
            state = _STATE.RAID_GAME;
        else
            state = _STATE.SINGLE_GAME;

        TempCoroutine.instance.StopAllCoroutines();
        SceneManager.instance.LobbyActionEvent(state, _ACTION.GO_MAP, openType);
        StopAllCoroutines();
    }

    public override void Close()
    {
        if (Groups[(int)groupT.HnP_model].activeSelf)
        {
            GotoTown();//빽키로 나가기 했으면 마을로 강제 이동.
        }
    }

    /// <summary> 결과화면 </summary>
    void Result()
    {
        if (isSuccess)
        {
            SoundManager.instance.PlaySfxSound(eUISfx.Success, true);

            if (NetData.instance.UserLevel < _LowDataMgr.instance.GetEtcTableValue<int>(EtcID.MaxLevel))
            {
                if (0 < Reward.GetExp)//획득한 경험치가 있다면 실행
                    StartCoroutine("ExpGaugeUpdate");
                else
                    IsIgnoreBtn = false;
            }
            else
                IsIgnoreBtn = false;
        }
        else
        {
            IsIgnoreBtn = false;
            if (G_GameInfo.GameInfo.IsEndForced)
                SoundManager.instance.PlaySfxSound(eUISfx.Forced, true);
            else
                SoundManager.instance.PlaySfxSound(eUISfx.Fail, true);
        }

        //영웅및파트너 그룹 활성
        Groups[(int)groupT.HnP_model].SetActive(true);
        //획득 그룹 활성
        Groups[(int)groupT.DropItem].SetActive(true);

        //우측 홈가기 던전선택가기 버튼 활성화
        Groups[(int)groupT.rigthBtns].SetActive(true);

        //스테이지 퀘스트 정리.
        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)
            SingleGameState.StageQuestList.Clear();
    }

    /// <summary> 보상아이템 셋팅 </summary>
    void SettingRewardItem()
    {
        if (G_GameInfo.GameMode == GAME_MODE.ARENA)
        {
            RewardAssetTf.parent.gameObject.SetActive(false);
            return;
        }

        RewardAssetTf.parent.gameObject.SetActive(true);
        RewardAssetTf.gameObject.SetActive(false);
        RewardExp.gameObject.SetActive(false);
        RewardGold.gameObject.SetActive(false);

        uint firstRewardId = 0;
        uint fixedRewardId = 0;

        if(G_GameInfo.GameMode == GAME_MODE.SINGLE && isSuccess)
        {
            DungeonTable.StageInfo lowData = _LowDataMgr.instance.GetStageInfo(SingleGameState.lastSelectStageId);

            //최초보상이 있을씨에..
            if (SingleGameState.IsFirstReward )
            {
                GatchaReward.FixedRewardInfo firstInfo = _LowDataMgr.instance.GetFixedRewardItem(lowData.FirstReward);

                GameObject go = Instantiate(ItemSlotPrefab) as GameObject;
                Transform tf = go.transform;
                tf.parent = RewardGrid.transform;
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;

                InvenItemSlotObject firstSlotObj = go.GetComponent<InvenItemSlotObject>();
                firstSlotObj.SetLowDataItemSlot(firstInfo.ItemId == 0 ? firstInfo.Type :  firstInfo.ItemId, firstInfo.ItemCount);

                FirstRewardSlotLabel.transform.parent = go.transform;
                FirstRewardSlotLabel.transform.localPosition = new Vector3(0, 42, 0);
                FirstRewardSlotLabel.transform.localScale = Vector3.one;
                FirstRewardSlotLabel.SetActive(true);

                firstRewardId = firstInfo.ItemId == 0 ? firstInfo.Type : firstInfo.ItemId;

            }

            // 기본보상표시 0이면안준다
            if (lowData.FixedReward != 0)
            {
                GatchaReward.FixedRewardInfo basicInfo = _LowDataMgr.instance.GetFixedRewardItem(lowData.FixedReward);

                GameObject basicGo = Instantiate(ItemSlotPrefab) as GameObject;
                Transform basicTf = basicGo.transform;
                basicTf.parent = RewardGrid.transform;
                basicTf.localPosition = Vector3.zero;
                basicTf.localScale = Vector3.one;

                InvenItemSlotObject basicSlotObj = basicGo.GetComponent<InvenItemSlotObject>();
                basicSlotObj.SetLowDataItemSlot(basicInfo.ItemId == 0 ? basicInfo.Type : basicInfo.ItemId, basicInfo.ItemCount);

                BasicRewardSlotLabel.transform.parent = basicGo.transform;
                BasicRewardSlotLabel.transform.localPosition = new Vector3(0, 42, 0);
                BasicRewardSlotLabel.transform.localScale = Vector3.one;
                BasicRewardSlotLabel.SetActive(true);

                fixedRewardId = basicInfo.ItemId == 0 ? basicInfo.Type : basicInfo.ItemId;
            }

        }


        List<NetData.DropItemData> dropItemList = Reward.GetList;
        if (dropItemList != null)
        {
            int count = dropItemList.Count;
            for (int i = 0; i < count; i++)
            {
                NetData.DropItemData dropData = dropItemList[i];

                //위에 표기됬으므로 표기안해줌
                if (dropData.LowDataId == fixedRewardId)
                    continue;

                if (dropData.LowDataId == firstRewardId)
                    continue;

                GameObject go = Instantiate(ItemSlotPrefab) as GameObject;
                Transform tf = go.transform;
                tf.parent = RewardGrid.transform;
                tf.localPosition = Vector3.zero;
                tf.localScale = Vector3.one;

                InvenItemSlotObject slotObj = go.GetComponent<InvenItemSlotObject>();
                slotObj.SetLowDataItemSlot(dropData.LowDataId, dropData.Amount);
            }
            
            RewardGrid.repositionNow = true;

        }

        if (5 < RewardGrid.transform.childCount)
            RewardGrid.transform.parent.GetComponent<UIScrollView>().enabled = true;
        else
            RewardGrid.transform.parent.GetComponent<UIScrollView>().enabled = false;

        if (G_GameInfo.GameMode == GAME_MODE.SINGLE)//GAME_MODE.SPECIAL)
            return;
        else
        {
            //획득 재화 설정
            bool isGoldDungeon = false, isExpDungeon = false;
            if (G_GameInfo.GameMode == GAME_MODE.SPECIAL_EXP)
                isExpDungeon = true;
            else if (G_GameInfo.GameMode == GAME_MODE.SPECIAL_GOLD)
                isGoldDungeon = true;

            AssetType assetType = AssetType.None;
            if (G_GameInfo.GameMode == GAME_MODE.TOWER)
                assetType = AssetType.Badge;

            if (0 <= Reward.GetCoin && !isExpDungeon)
            {
                RewardGold.gameObject.SetActive(true);
                RewardGold.text = Reward.GetCoin == 0 ? "0" : Reward.GetCoin.ToString(); // ToString("#,##");
            }

            if (0 <= Reward.GetExp && !isGoldDungeon)
            {
                RewardExp.gameObject.SetActive(true);
                RewardExp.text = Reward.GetExp == 0 ? "0" : Reward.GetExp.ToString(); // ToString("#,##");
            }
            
            if (0 <= Reward.GetAsset && assetType != AssetType.None)
            {
                RewardAssetTf.gameObject.SetActive(true);
                RewardAssetTf.GetComponent<UILabel>().text = Reward.GetAsset == 0 ? "0" : Reward.GetAsset.ToString(); // ToString("#,##");

                UISprite sp = RewardAssetTf.FindChild("icon").GetComponent<UISprite>();
                switch (assetType)
                {
                    case AssetType.Badge:
                        sp.spriteName = "badge_A";//Img_flag2
                        break;
                }
            }
        }
        
    }

    /// <summary> 경험치 상승 </summary>
    IEnumerator ExpGaugeUpdate()
    {
        ulong getExp = Reward.SaveExp + Reward.GetExp;

        float runTime = 0, duration = 0.5f;
        float curExpValue = (float)Reward.SaveExp / (float)Reward.SaveMaxExp;
        float maxExpValue = (float)getExp / (float)Reward.SaveMaxExp;
        float saveExpValue = 0;

        if (!gameObject.activeSelf)
        {
            while (!gameObject.activeSelf)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.2f);//0.2f동안 대기 했다가 시작
        //SoundManager.instance.PlaySfxSound(eUISfx.GetExp, false);//경험치 증가 사운드

        bool isNoticePanel = false;
        bool isLevelup = Reward.SaveLevel < NetData.instance.UserLevel ? true : false;///현재 레벨이 높다는 것은 레벨업을 햇다는 소리겠지??;
        runTime = 0;
        if (isLevelup)//레벨업 했을 경우//1 <= maxExpValue && 
        {
            if (1 < maxExpValue)
                saveExpValue = maxExpValue;

            maxExpValue = 1;

            uint level = Reward.SaveLevel;
            while (true)
            {
                runTime += Time.deltaTime;

                float rate = runTime / duration;
                rate = Mathf.Clamp01(rate);

                float value = Mathf.Lerp(curExpValue, maxExpValue, rate);
                ExpGauge.value = value;

                if (1 <= value && 1f <= saveExpValue)//두번이상 렙업함.
                {
                    runTime = 0;
                    curExpValue = 0;
                    Labels[(int)labelT.hLv].text = string.Format(_LowDataMgr.instance.GetStringCommon(453), ++level);

                    Level.LevelInfo levelLowData = _LowDataMgr.instance.GetLowDataCharLevel(level - 1);

                    getExp -= levelLowData.Exp;//누적 경험치
                    levelLowData = _LowDataMgr.instance.GetLowDataCharLevel(level);
                    if (levelLowData == null)
                        break;

                    saveExpValue -= 1;

                    if (saveExpValue < 1)
                        break;

                }
                else if (rate == 1)
                    break;

                yield return null;
            }

            //남은 경험치 계산
            uint realCurExp = 0, realMaxExp = 0;
            NetData.instance.GetUserInfo().GetCurrentAndMaxExp(ref realCurExp, ref realMaxExp);

            maxExpValue = (float)realCurExp / (float)realMaxExp;
            Labels[(int)labelT.hLv].text = string.Format(_LowDataMgr.instance.GetStringCommon(453), NetData.instance.UserLevel);
            
            SceneManager.instance.SetNoticePanel(NoticeType.LevelUp);
            SceneManager.instance.SetNoticePanel(NoticeType.PowerUp);
            
            List<Quest.MainTutorialInfo> tutoList = _LowDataMgr.instance.GetLowDataMainTutorialList(1);
            for (int i = 0; i < tutoList.Count; i++)
            {
                if (tutoList[i].OpenType == 0)//일반연출 무시
                    continue;

                if (NetData.instance.UserLevel != tutoList[i].OpenLevel)
                    continue;
                
                isNoticePanel = true;
                SceneManager.instance.SetNoticePanel(NoticeType.Contents, tutoList[i].OpenType);
                break;
            }
            
            curExpValue = 0;
            runTime = 0;
            while (true)
            {
                runTime += Time.deltaTime;
                float rate = runTime / duration;
                rate = Mathf.Clamp01(rate);

                float value = Mathf.Lerp(curExpValue, maxExpValue, rate);
                ExpGauge.value = value;

                if (rate == 1)
                    break;

                yield return null;
            }
        }
        else//경험치 증가만 함.
        {
            while (true)
            {
                runTime += Time.deltaTime;
                float rate = runTime / duration;
                rate = Mathf.Clamp01(rate);

                float value = Mathf.Lerp(curExpValue, maxExpValue, rate);
                ExpGauge.value = value;

                if (rate == 1)
                    break;

                yield return null;
            }
        }

        if( !isNoticePanel)
            IsIgnoreBtn = false;

        yield return null;
    }
    
    IEnumerator TextAction(object[] parameter)
    {
        int startIdx = (int)parameter[0];
        int endIdx = (int)parameter[1];
        string insertStr = (string)parameter[2];
        UILabel statLbl = (UILabel)parameter[3];
        statLbl.text = string.Format(string.IsNullOrEmpty(insertStr) ? "{0}" : string.Format("{0} {1}", insertStr, startIdx), startIdx);
        
        yield return new WaitForSeconds(0.5f);//연출 대기
        
        float runTime = 0;
        while (true)
        {
            runTime += Time.deltaTime;
            float rate = runTime / 0.5f;
            rate = Mathf.Clamp01(rate);

            float value = Mathf.Lerp(startIdx, endIdx, rate);
            statLbl.text = string.Format(string.IsNullOrEmpty(insertStr) ? "{0}" : string.Format("{0} {1}", insertStr, (int)value), (int)value);

            if (1f <= rate)
                break;

            yield return null;
        }

        statLbl.text = string.Format(string.IsNullOrEmpty(insertStr) ? "{0}" : string.Format("{0} {1}", insertStr, endIdx), endIdx);
    }
    
    public override void NetworkData(params object[] proto)
    {
        base.NetworkData(proto);

        TowerPop.gameObject.SetActive(true);

        switch ((Sw.MSG_DEFINE)proto[0])
        {
            case Sw.MSG_DEFINE._MSG_TOWER_RANK_QUERY_S://랭킹권 받아옴
                TowerPop.FindChild("Best/Txt_Ranking").GetComponent<UILabel>().text = (string)proto[1];
                TowerPop.FindChild("Best").gameObject.SetActive(true);
                break;

            case Sw.MSG_DEFINE._MSG_TOWER_USE_TIME_QUERY_S:
                int towerLv = (int)proto[1];
                int clearTime = (int)proto[2];
                
                float bestM = Mathf.Floor(clearTime / 60);
                float bestS = Mathf.Floor(clearTime % 60);
                float clearM = Mathf.Floor(G_GameInfo.GameInfo.PlayTime / 60);
                float clearS = Mathf.Floor(G_GameInfo.GameInfo.PlayTime % 60);
                
                TowerPop.FindChild("Best/Txt_Best").GetComponent<UILabel>().text = string.Format("{0:00} : {1:00}", bestM, bestS);
                TowerPop.FindChild("Time/Txt_Best").GetComponent<UILabel>().text = string.Format("{0:00} : {1:00}", clearM, clearS);
                TowerPop.FindChild("Time").gameObject.SetActive(true);
                break;
        }
    }

    public void GotoTown()
    {
        IsIgnoreBtn = false;
        GotoTown(null);
    }
}

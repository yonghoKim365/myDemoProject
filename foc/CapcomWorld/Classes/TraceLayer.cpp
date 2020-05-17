//
//  TraceLayer.cpp
//  CapcomWorld
//
//  Created by yongho Kim on 13. 1. 30..
//
//

#include "TraceLayer.h"
#include "MainScene.h"
#include "UserStatLayer.h"
#include "StageLayer.h"
#include "CardDetailViewLayer.h"
#include "Tutorial.h"

#if (CC_TARGET_PLATFORM == CC_PLATFORM_IOS)
using namespace cocos2d::extension;
#endif


enum spr_tag
{
    SPR_TAG_TAB = 20,
    SPR_TAG_GO,
    SPR_TAG_OK,
    SPR_TAG_MAX
};

enum trace_event
{
    TRACE_EVENT_EXP_UP = 0,
    TRACE_EVENT_COIN,
    TRACE_EVENT_CARD,
    TRACE_EVENT_BBQ, //STAMINA,
    TRACE_EVENT_DRUM_CRASH,
    TRACE_EVENT_NORMAL_ENEMY,
    TRACE_EVENT_RIVAL,
    TRACE_EVENT_HIDDEN_RIVAL,
    TRACE_EVENT_BOSS,
    TRACE_EVENT_LEVEL_UP_AFTER_TRACE,
    TRACE_EVENT_LEVEL_UP_AFTER_BATTLE,
    TRACE_EVENT_STAGE_CLEAR,
    TRACE_EVENT_SHOW_RIVAL_BATTLE_RESULT,
    TRACE_EVENT_GO_RIVAL_AFTER_LOSE,
    TRACE_EVENT_GO_BOSS_AFTER_LOSE,
    TRACE_EVENT_BACK_TO_HISTORY,
};

TraceLayer* TraceLayer::instance = NULL;

TraceLayer* TraceLayer::getInstance()
{
    if (!instance)
        return NULL;
    
    return instance;
}


//TraceLayer::TraceLayer(int _questID, int _progress, const char* stageLabel) : hitCounter(0)
TraceLayer::TraceLayer(int _questID, int _progress) : hitCounter(0)
{
    ///////////////////
    //
    // 추적 배경 음악 재생
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
    }
    
    questID = _questID;
    questProgress = _progress;
    
    //this->stageLabel = stageLabel;
    questSp = 0;
    
    for (int i=0;i<PlayerInfo::getInstance()->questList->count();i++){
        QuestInfo *info = (QuestInfo*)PlayerInfo::getInstance()->questList->objectAtIndex(i);
        if (info->questID == questID){
            questSp = info->q_sp;
            break;
        }
    }
    popupCnt = 0;
    
    instance = this;
    this->setTouchEnabled(true);

    MainScene::getInstance()->removeUserStatLayer();
    
    /////////////////////////////////////////////////////////////////////
    // 추적팀으로 설정된 팀이 비어있다면, 비어있지 않은 팀을 추적팀으로 변경하여 준다.
    /////////////////////////////////////////////////////////////////////
    
    PlayerInfo::getInstance()->traceTeam = CCUserDefault::sharedUserDefault()->getIntegerForKey("traceTeam");
    if (PlayerInfo::getInstance()->isTeamEmpty(PlayerInfo::getInstance()->traceTeam)){
        for(int i=0;i<4;i++){
            if (PlayerInfo::getInstance()->isTeamEmpty(i)==false){
                PlayerInfo::getInstance()->traceTeam = i;
                saveTraceTeam();
                break;
            }
        }
    }
    
    InitChaseUI();
    
    setChaseStep(CHASE_TAB);
    
    bOpenLevelUpAfterTrace = false;
    
    bCallFromHistory = false;
    
    bRewardCardAction = false;
}

// TraceHistory부터의 접근
TraceLayer::TraceLayer(int _rival_battle_id, NpcInfo* _npc, int rivalKind, AReceivedRival* rivalInfo)
{
    gQuestID = rivalInfo->quest_id;
    gRid_for_rivalLayer = _rival_battle_id;
    gCurrentNpc = _npc;
    gRivalKind = rivalKind;
    gRivalInfo = rivalInfo;
    
    
    
    addPageLoading();
    bool startTrace = true;
    
    FileManager* fManager = FileManager::sharedFileManager();
    std::vector<std::string> downloads;

    Quest_Data* bgQuestInfo = fManager->GetQuestInfo(rivalInfo->quest_id);
    std::string bgImgPath = FOC_IMAGE_SERV_URL;
    bgImgPath.append("images/bg/");
    if (!fManager->IsFileExist(bgQuestInfo->stageBG_L.c_str()))
    {
        startTrace = false;
        string downPath = bgImgPath + bgQuestInfo->stageBG_L;
        downloads.push_back(downPath);
    }
    
    if (false == startTrace)
    {
        CCHttpRequest* requestActor = CCHttpRequest::sharedHttpRequest();
        requestActor->addDownloadTask(downloads, this, callfuncND_selector(TraceLayer::onHttpRequestCompleted));
    }
    else
    {
        CCLog("downloading trace background images is completed!");
        removePageLoading();
        
        ///////////////////
        //
        // 라이벌 배경 음악 재생
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
            soundBG->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
        }
        
        //unschedule(schedule_selector(TraceHistoryLayer::battleTimeCounter));
        TraceDetailLayer::getInstance()->unschedule(schedule_selector(TraceDetailLayer::timeCounter));
        
        questID = rivalInfo->quest_id;
        popupCnt = 0;
        
        rid_for_rivalLayer = _rival_battle_id;
        
        currentNpc = _npc;
        
        questProgress = -1;

        instance = this;
        
        this->setTouchEnabled(true);
        
        MainScene::getInstance()->removeUserStatLayer();
        
        PlayerInfo::getInstance()->traceTeam = CCUserDefault::sharedUserDefault()->getIntegerForKey("traceTeam");
        
        InitChaseUI();
        
        //setChaseStep(CHASE_TAB);
        
        
        if (rivalKind == 1){
            goRivalFromHistory(_rival_battle_id, rivalInfo);
        }
        else if (rivalKind == 2){
            goHiddenRivalFromHistory(_rival_battle_id, rivalInfo);
        }
        
        bOpenLevelUpAfterTrace = false;
        
        bCallFromHistory = true;
        bRemoveDetail    = false;   // 졌을 경우 상세 이력 레이어가 필요 없으므로 라이벌 레이어에서 시그널을 true로 바꿔준 후 처리
    }
}

void TraceLayer::onHttpRequestCompleted(cocos2d::CCObject* pSender, void* data)
{
    HttpResponsePacket* response = (HttpResponsePacket* )data;
    if (response->request->reqType == kHttpRequestDownloadFile)
    {
/*
        gQuestID = rivalInfo->quest_id;
        gRid_for_rivalLayer = _rival_battle_id;
        gCurrentNpc = _npc;
        gRivalKind = rivalKind;
        gRivalInfo = rivalInfo;
*/

        
        CCLog("downloading trace background images is completed!");
        removePageLoading();
        
        ///////////////////
        //
        // 라이벌 배경 음악 재생
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
            soundBG->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
        }
        
        //unschedule(schedule_selector(TraceHistoryLayer::battleTimeCounter));
        TraceDetailLayer::getInstance()->unschedule(schedule_selector(TraceDetailLayer::timeCounter));
        
        questID = gQuestID;
        popupCnt = 0;
        
        rid_for_rivalLayer = gRid_for_rivalLayer;
        
        currentNpc = gCurrentNpc;
        
        questProgress = -1;
        
        instance = this;
        
        this->setTouchEnabled(true);
        
        MainScene::getInstance()->removeUserStatLayer();
        
        PlayerInfo::getInstance()->traceTeam = CCUserDefault::sharedUserDefault()->getIntegerForKey("traceTeam");
        
        InitChaseUI();
        
        //setChaseStep(CHASE_TAB);
        
        if (gRivalKind == 1){
            goRivalFromHistory(gRid_for_rivalLayer, gRivalInfo);
        }
        else if (gRivalKind == 2){
            goHiddenRivalFromHistory(gRid_for_rivalLayer, gRivalInfo);
        }
        
        bOpenLevelUpAfterTrace = false;
        
        bCallFromHistory = true;
        bRemoveDetail    = false;   // 졌을 경우 상세 이력 레이어가 필요 없으므로 라이벌 레이어에서 시그널을 true로 바꿔준 후 처리
    }
}

TraceLayer::~TraceLayer()
{
    delete frameToHit;
    delete frameToTab;
    delete frameToGo;
    delete frameFindingEnemyEntry;
    delete frameFindingEnemyMain;
    delete frameFindingEnemyExit;
    delete frameToAlert;
    delete frameToSpotlight;
    
    delete pTraceResultLayer;
    
    this->removeAllChildrenWithCleanup(true);
}

void TraceLayer::setChaseStep(int n)
{
    
    // old
    if (nChaseStep != n){
        switch(nChaseStep){
            case CHASE_TAB:
                releaseTab();
                break;
            case CHASE_GO:
                releaseGo();
                break;
            case CHASE_NORMAL_BATTLE:
                if (normalEnemyLayer != NULL){
                    this->removeChild(normalEnemyLayer, true);
                    normalEnemyLayer = NULL;
                }
                break;
            case CHASE_RIVAL:
                if (rivalLayer != NULL){
                    this->removeChild(rivalLayer, true);
                    rivalLayer = NULL;
                }
                break;
            case CHASE_HIDDEN_RIVAL:
                if (hiddenRivalLayer != NULL){
                    this->removeChild(hiddenRivalLayer, true);
                    hiddenRivalLayer = NULL;
                }
                break;
            case CHASE_BOSS:
                this->removeChild(bossLayer, true);
                bossLayer = NULL;
                break;
        }
    }
    
    this->removeChildByTag(500,true);
    // new
    
    nChaseStep = n;
    
    //this->removeAllChildrenWithCleanup(true);
    
    //
    switch(n)
    {
        case CHASE_TAB:
            InitUI1();
            initTeamUI();
            break;
        case CHASE_GO:
            InitUI2();
            break;
        case CHASE_NORMAL_BATTLE:
            {
                currentNpc = MainScene::getInstance()->getNpc(receivedTraceResultInfo->enemy_code);
                normalEnemyLayer = new TraceNormalEnemyLayer(currentNpc);
                normalEnemyLayer->setContentSize(this->getContentSize());
                normalEnemyLayer->setAnchorPoint(ccp(0,0));
                normalEnemyLayer->setPosition(ccp(0,0));
                this->addChild(normalEnemyLayer,5);
                normalEnemyLayer->InitNormalBattle1(currentNpc->sendCoin);
                
                //InitNormalBattle1();
                
                ///////////
                //
                // TUTORIAL
                
                const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_QUESTBATTLE);
                
                if(false == TutorialCompleted)
                {
                    const bool TutorialMode = false;
                    NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                    tutorialProgress = TUTORIAL_QUESTBATTLE_1;
                    basePopUp->InitUI(&tutorialProgress);
                    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                    basePopUp->setPosition(accp(0.0f, 0.0f));
                    basePopUp->setTag(98765);
                    MainScene::getInstance()->addChild(basePopUp, 9000);
                }
            }
            break;
        case CHASE_RIVAL:
            {
                currentNpc = MainScene::getInstance()->getNpc(receivedTraceResultInfo->enemy_code);
                rivalLayer  = new TraceRivalLayer(currentNpc, receivedTraceResultInfo->rid, receivedTraceResultInfo->enemy_hp, receivedTraceResultInfo->enemy_hp_max, receivedTraceResultInfo->enemy_level, receivedTraceResultInfo->battle_limit_time, questID);
                rivalLayer->setContentSize(this->getContentSize());
                rivalLayer->setAnchorPoint(ccp(0,0));
                rivalLayer->setPosition(ccp(0,0));
                this->addChild(rivalLayer,5);
                
                ///////////
                //
                // TUTORIAL
                
                const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_RIVALBATTLE);
                
                if(false == TutorialCompleted)
                {
                    PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_HIDDENRIVAL, false);
                    
                    const bool TutorialMode = false;
                    NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                    tutorialProgress = TUTORIAL_RIVALBATTLE_1;
                    basePopUp->InitUI(&tutorialProgress);
                    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                    basePopUp->setPosition(accp(0.0f, 0.0f));
                    basePopUp->setTag(98765);
                    MainScene::getInstance()->addChild(basePopUp, 9000);
                }
                else
                {
                    rivalLayer->actionRival1();
                }
                
                rivalLevel = receivedTraceResultInfo->enemy_level;
            }
            break;
        case CHASE_HIDDEN_RIVAL:
            {
                //InitHiddenRivalBattle1();
                
                currentNpc = MainScene::getInstance()->getNpc(receivedTraceResultInfo->enemy_code);
                hiddenRivalLayer  = new TraceHiddenRivalLayer(currentNpc,  receivedTraceResultInfo->rid, receivedTraceResultInfo->enemy_hp, receivedTraceResultInfo->enemy_hp_max, receivedTraceResultInfo->enemy_level, receivedTraceResultInfo->battle_limit_time, questID);
                hiddenRivalLayer->setContentSize(this->getContentSize());
                hiddenRivalLayer->setAnchorPoint(ccp(0,0));
                hiddenRivalLayer->setPosition(ccp(0,0));
                this->addChild(hiddenRivalLayer,5);
                
                ///////////
                //
                // TUTORIAL
                
                const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_RIVALBATTLE);
  
                if(false == TutorialCompleted)
                {
                    PlayerInfo::getInstance()->SetTutorialState(TUTORIAL_HIDDENRIVAL, true);
                    
                    const bool TutorialMode = false;
                    NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                    tutorialProgress = TUTORIAL_RIVALBATTLE_1;
                    basePopUp->InitUI(&tutorialProgress);
                    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                    basePopUp->setPosition(accp(0.0f, 0.0f));
                    basePopUp->setTag(98765);
                    MainScene::getInstance()->addChild(basePopUp, 9000);
                }
                else
                {
                    hiddenRivalLayer->actionHiddenRival1();
                }
                
                rivalLevel = receivedTraceResultInfo->enemy_level;
            }
            break;
        case CHASE_BOSS:
            {
                ///////////////////
                //
                // 보스 배경 음악 재생
                
                if (PlayerInfo::getInstance()->getBgmOption()){
                    soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
                    soundBG->playBackgroundMusic("audio/bgm_boss_01.mp3", true);
                }
                
                currentNpc = MainScene::getInstance()->getNpc(receivedTraceResultInfo->enemy_code);
                bossLayer = new TraceBossLayer(currentNpc, BossID, receivedTraceResultInfo->enemy_hp, receivedTraceResultInfo->enemy_hp_max, receivedTraceResultInfo->enemy_level);
                bossLayer->setContentSize(this->getContentSize());
                bossLayer->setAnchorPoint(ccp(0,0));
                bossLayer->setPosition(ccp(0,0));
                this->addChild(bossLayer,5);
                
                ///////////
                //
                // TUTORIAL
                
                const bool TutorialCompleted = PlayerInfo::getInstance()->GetTutorialState(TUTORIAL_BOSSBATTLE);
                
                if(false == TutorialCompleted)
                {
                    const bool TutorialMode = false;
                    NewTutorialPopUp *basePopUp = new NewTutorialPopUp(TutorialMode);
                    tutorialProgress = TUTORIAL_BOSSBATTLE_1;
                    basePopUp->InitUI(&tutorialProgress);
                    basePopUp->setAnchorPoint(ccp(0.0f, 0.0f));
                    basePopUp->setPosition(accp(0.0f, 0.0f));
                    basePopUp->setTag(98765);
                    MainScene::getInstance()->addChild(basePopUp, 9000);
                }
                else
                {
                    bossLayer->actionBoss();
                }
            
                rivalLevel = receivedTraceResultInfo->enemy_level;
            }
            break;
    }
    
    this->setTouchEnabled(true);
    traceUILayer->Refresh();
}


void TraceLayer::RewardEventManager()
{
    if (rewardEventQueue->count()>0){
        rewardEventQueue->removeObjectAtIndex(0);
    }
    
    if (rewardEventQueue->count()>0){
        
        EventObject *obj = (EventObject*)rewardEventQueue->objectAtIndex(0);
        
        switch(obj->eventCode){
            case TRACE_EVENT_DRUM_CRASH:
                actionDrumCrash();
                break;
            case TRACE_EVENT_EXP_UP:
                actionExpUp(0, obj->exp);
                //actionExpUp(0, resultInfo->exp);
                break;
            case TRACE_EVENT_COIN:
                actionGetCoin(0, obj->coin);
                break;
            case TRACE_EVENT_CARD:
                actionGetCard(0);
                break;
            case TRACE_EVENT_BBQ:
                actionGetStamina(obj->sp);
                break;
            case TRACE_EVENT_LEVEL_UP_AFTER_TRACE:
                bOpenLevelUpAfterTrace = true;
                setTouchEnabled(false);
                InitLevelUplayer(receivedTraceResultInfo->user_uPnt);
                break;
            case TRACE_EVENT_LEVEL_UP_AFTER_BATTLE:
                bOpenLevelUpAfterBattle = true;
                setTouchEnabled(false);
                InitLevelUplayer(responseRivalBattleInfo->user_uPnt);
                break;
            case TRACE_EVENT_STAGE_CLEAR:
                {
                StageClearPopup *popup = new StageClearPopup();
                popup->InitUI();
                this->addChild(popup,1000);
                }
                break;
            case TRACE_EVENT_SHOW_RIVAL_BATTLE_RESULT:
                actionRivalBattleResult1();
                break;
            case TRACE_EVENT_GO_RIVAL_AFTER_LOSE:
                {
                    this->schedule(schedule_selector(TraceLayer::goRivalAfterLose), 0.5f, 0, 0);
                }
                break;
            case TRACE_EVENT_GO_BOSS_AFTER_LOSE:
                {
                    this->schedule(schedule_selector(TraceLayer::goBossAfterLose), 0.5f, 0, 0);
                }
                break;
            case TRACE_EVENT_BACK_TO_HISTORY:
                {
                    this->schedule(schedule_selector(TraceLayer::exitLayer), 0.5f, 0, 0);
                }
                break;
        }
    }
    else{
        // event ??
        setTouchEnabled(true);
        setChaseStep(CHASE_TAB);
    }
    
}

void TraceLayer::callBackNormalEnemy()
{
    // 
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    CCSize size = this->getContentSize();
    
    setChaseStep(CHASE_NORMAL_BATTLE);
}

void TraceLayer::goRival()
{
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    
//    this->removeChildByTag(499,true);
    
    //setChaseStep(CHASE_TAB);
    setChaseStep(CHASE_RIVAL);
}

void TraceLayer::goRivalFromHistory(int rid, AReceivedRival *ri)
{
    nChaseStep = CHASE_RIVAL;
    this->removeChildByTag(499,true);
    rivalLayer  = new TraceRivalLayer(currentNpc, rid, ri->cur_hp, ri->max_hp, ri->npc_lv, ri->limit);
    rivalLayer->receivedRivalInfo = ri;
    rivalLayer->setContentSize(this->getContentSize());
    rivalLayer->setAnchorPoint(ccp(0,0));
    rivalLayer->setPosition(ccp(0,0));
    this->addChild(rivalLayer,5);
    rivalLayer->InitUI();
    rivalLevel = ri->npc_lv;
    
    // 카드 보상 받자마자 뒤로 가기 버튼을 눌러 추적 화면에서 나온 후 히스토리로 접근 할 경우 그대로 true로 켜져 있는 문제 발생
    bRewardCardAction = false;
}

void TraceLayer::goRivalAfterLose()
{
    nChaseStep = CHASE_RIVAL;
    this->removeChildByTag(499,true);
    
    rivalLayer  = new TraceRivalLayer(currentNpc, rid_for_rivalLayer, responseRivalBattleInfo->rival_remain_hp, responseRivalBattleInfo->rival_max_hp, responseRivalBattleInfo->rival_level, responseRivalBattleInfo->battle_limit_time);
    rivalLayer->setContentSize(this->getContentSize());
    rivalLayer->setAnchorPoint(ccp(0,0));
    rivalLayer->setPosition(ccp(0,0));
    this->addChild(rivalLayer,5);
    rivalLayer->InitUI();
}

void TraceLayer::goBossAfterLose()
{
    nChaseStep = CHASE_BOSS;
    this->removeChildByTag(499,true);
    
    // Î∞∞Ì? ?????hp receivedTraceResultInfo->rival_origin_hp
    // Î∞∞Ì? Ï¢????hp receivedTraceResultInfo->rival_remain_hp
    CCLog(" boss HP %d %d",receivedTraceResultInfo->rival_hp_before_battle, receivedTraceResultInfo->rival_hp_after_battle);
    
    bossLayer  = new TraceBossLayer(currentNpc, BossID,receivedTraceResultInfo->rival_hp_after_battle, receivedTraceResultInfo->rival_max_hp,receivedTraceResultInfo->enemy_level);
    bossLayer->setContentSize(this->getContentSize());
    bossLayer->setAnchorPoint(ccp(0,0));
    bossLayer->setPosition(ccp(0,0));
    this->addChild(bossLayer,5);
    bossLayer->InitUI();
    
}

void TraceLayer::goHiddenRival()
{
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    
    //setChaseStep(CHASE_TAB);
    setChaseStep(CHASE_HIDDEN_RIVAL);
}

void TraceLayer::goHiddenRivalFromHistory(int rid, AReceivedRival *ri)
{
    nChaseStep = CHASE_HIDDEN_RIVAL;
    hiddenRivalLayer = new TraceHiddenRivalLayer(currentNpc, rid, ri->cur_hp, ri->max_hp, ri->npc_lv, ri->limit);
    hiddenRivalLayer->receivedRivalInfo = ri;
    hiddenRivalLayer->setContentSize(this->getContentSize());
    hiddenRivalLayer->setAnchorPoint(ccp(0,0));
    hiddenRivalLayer->setPosition(ccp(0,0));
    this->addChild(hiddenRivalLayer,5);
    hiddenRivalLayer->InitUI();
    rivalLevel = ri->npc_lv;
    
    // 카드 보상 받자마자 뒤로 가기 버튼을 눌러 추적 화면에서 나온 후 히스토리로 접근 할 경우 그대로 true로 켜져 있는 문제 발생
    bRewardCardAction = false;
}

void TraceLayer::goBoss()
{
    this->removeChildByTag(31, true);
    this->removeChildByTag(32, true);
    this->removeChildByTag(33, true);
    
    setChaseStep(CHASE_BOSS);
}

void TraceLayer::callBackRivalBattle()
{
    this->removeChildByTag(388,true);
    setChaseStep(CHASE_TAB);
    // Rival battle result
}






void TraceLayer::BackBtnCallback(CCObject* pSender)
{
    if (bRewardCardAction)return;
    
    soundButton1();
    exitLayer();
}


void TraceLayer::exitLayer()
{
    ///////////////////
    //
    // 추적 배경 음악 정지
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG->stopBackgroundMusic();
    }
    
    ///////////////////
    //
    // 메인 배경 음악 재생
    
    soundMainBG();
    
    this->removeAllChildrenWithCleanup(true);
    
    if (bCallFromHistory) {
        
        if (bRemoveDetail == true)
        {
            // 라이벌 이력으로 접근

            TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
            TraceDetailLayer::getInstance()->removeFromParentAndCleanup(true);

            MainScene::getInstance()->initUserStatLayer();
            TraceHistoryLayer::getInstance()->callbackFromDetail();
        }
        else
        {
            // 동료들 이력으로 접근
            TraceDetailLayer::getInstance()->callBackFromTraceLayer();
        }
    }
    else {
        // 추적 화면을 빠져나가면서 지금까지의 스테이지 정보를 갱신함으로써 다시 스테이지 리스트에서 접근 시 최신의 정보를 보여줌(스테이지 진행도)
        ARequestSender::getInstance()->requestStageList();
        
        MainScene::getInstance()->initUserStatLayer();
        MainScene::getInstance()->ShowMainMenu();
        
        StageLayer::getInstance()->InitUI();
        StageLayer::getInstance()->setPositionY(StageLayer::getInstance()->lastLayerY);
        StageLayer::getInstance()->setTouchEnabled(true);
        StageLayer::getInstance()->removeChild(this, true);
    }
}





void TraceLayer::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    for (int i=0;i<SPR_TAG_MAX;i++){
        if (GetSpriteTouchCheckByTag(this, i, localPoint)){
            touchBeganTag = i;
        }
    }
    
}

void TraceLayer::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if (popupCnt > 0)return;
    
    if (GetSpriteTouchCheckByTag(this, SPR_TAG_TAB, localPoint)){
        if (touchBeganTag == SPR_TAG_TAB){
            
            
            if (PlayerInfo::getInstance()->isTeamEmpty(PlayerInfo::getInstance()->traceTeam) || PlayerInfo::getInstance()->GetCardInDeck(0,0,2) == NULL){
                
                popupCnt++;
                TeamSettingWarningPopup* popup = new TeamSettingWarningPopup();
                popup->InitUI();
                
                this->addChild(popup, 5000);
                
                this->setTouchEnabled(false);
                
                return;
            }
            
            if (PlayerInfo::getInstance()->getStamina() < questSp){
                // popup stamina buy
                popupStaminaBuy();
                return;
            }
            else{
                setChaseStep(CHASE_GO);
            }
            
        }
    }
    
    if(GetSpriteTouchCheckByTag(this, CARD_CLOSE_BTN, localPoint))
    {
        //CCLOG("confirm");
        
        soundButton1();
        
        this->removeChild(cardDetailViewLayer, true);
        this->removeChildByTag(CARD_CLOSE_BTN, true);
        this->removeChildByTag(CARD_CLOSE_BTN_LABEL, true);
        
        showCardCnt++;
        
        if (showCardCnt >= rewardCards->count()){
            closeCardAction();
        }
        else{
            showRewardCard();
            return;
            // 이 이후의 동작을 못하게 한다. stopallAction되면 안됨.
        }
    }
}



void TraceLayer::InitChaseUI()
{
/*
    ///////////////////
    //
    // 추적 배경 음악 재생
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
        soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
    }
*/    
    int content_h = GameConst::WIN_SIZE.height;// - accp(MAIN_LAYER_BTN_HEIGHT);
    this->setContentSize(CCSize(GameConst::WIN_SIZE.width,content_h));
    
    CCSize size = this->getContentSize();
    
    //CheckLayerSize(this);
    
    int yy = size.height;
    
    CCSprite* pSprTitleBG = CCSprite::create("ui/quest/trace/trace_title.png");
    pSprTitleBG->setAnchorPoint(ccp(0,1));
    pSprTitleBG->setPosition( ccp(0,yy));
    pSprTitleBG->setTag(190);
    this->addChild(pSprTitleBG, 100);
    
    char buffer[64];
    sprintf(buffer, "%d_name", questID);
    if (questID != 0){
        std::string text = LocalizationManager::getInstance()->get(buffer);
        
        CCLabelTTF* pLabel0 = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
        pLabel0->setColor(COLOR_WHITE);
        pLabel0->setTag(191);
        registerLabel( this,ccp(0.5,0.5), ccp(size.width/2,yy-accp(28)), pLabel0, 100);
    }
    
    CCSprite* pSpr1 = CCSprite::create("ui/quest/trace/trace_achieve.png");
    pSpr1->setAnchorPoint(ccp(1,1));
    //pSpr1->setPosition( ccp(size.width,yy-accp(35)));
    pSpr1->setPosition( ccp(size.width,yy));
    pSpr1->setTag(192);
    this->addChild(pSpr1, 100);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create("스테이지 진행도", "HelveticaNeue-Bold", 9);
    pLabel1->setColor(COLOR_BLACK);
    pLabel1->setTag(193);
    registerLabel( this,ccp(1,0.5), ccp(accp(628), yy - accp(16)), pLabel1, 100);
    
    
//    char buf[3];
//    sprintf(buf, "%d", questProgress);
//    CCLabelTTF* pLabel12 = CCLabelTTF::create(buf, "HelveticaNeue-Bold", 13);
//    pLabel12->setColor(COLOR_WHITE);
//    pLabel12->setTag(194);
//    registerLabel( this,ccp(1,0.5), ccp(accp(595), yy - accp(49)), pLabel12, 100);
    
    // 
    
    
    CCMenuItemImage *pSprBackBtn = CCMenuItemImage::create("ui/quest/trace/trace_btn_back_a1.png", "ui/quest/trace/trace_btn_back_a2.png", this, menu_selector(TraceLayer::BackBtnCallback));
    pSprBackBtn->setAnchorPoint( ccp(0,0));
    pSprBackBtn->setPosition( ccp(0,0));    // size.width/5 * 0,0));
    pSprBackBtn->setTag(0);
    
    CCMenu* pMenu = CCMenu::create(pSprBackBtn, NULL);
    
    pMenu->setAnchorPoint(ccp(0,0));
    //pMenu->setPosition( ccp(0, yy - accp(54) - accp(85)));
    pMenu->setPosition( ccp(0, yy - accp(75)));
    pMenu->setTag(199);
    
    this->addChild(pMenu, 100);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create("뒤로 가기", "HelveticaNeue-Bold", 8);
    pLabel2->setColor(COLOR_YELLOW);
    pLabel2->setTag(198);
    //registerLabel( this,ccp(0.5,0.5), ccp(accp(43), yy-accp(112)), pLabel2, 100);
    registerLabel( this,ccp(0.5,0.5), ccp(accp(43), yy-accp(52)), pLabel2, 100);
    
    
    //traceUI(-10);
    
    traceUILayer = new TraceUILayer();
    traceUILayer->setAnchorPoint(ccp(0,0));
    traceUILayer->setPosition(ccp(0,0));
    if (questProgress >= 0){
        traceUILayer->refreshStageProgressRatio(SCREEN_ZOOM_RATE*(GameConst::WIN_SIZE.width-accp(36)), SCREEN_ZOOM_RATE*(GameConst::WIN_SIZE.height-accp(76)));
    }
    this->addChild(traceUILayer, 100);
    
    initTeamUI();//83);
    
    //////////////////////////////
    //
    // 챕터에 맞는 배경 이미지 경로 생성

    string pathBG = CCFileUtils::sharedFileUtils()->getDocumentPath();
    
    // 다운로드 받지 않고 로컬 테스트
//    string pathBG = "ui/main_bg/";

//    if (questID == 0) {  // 접속하자마자 추적 이력에서 접근할 경우 챕터가 없으므로 기본 배경 사용
//        pathBG = pathBG + "bg_9.png";
//    }
//    else {
        Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
        pathBG = pathBG + questInfo->stageBG_L;
//    }
    
    actionZoomInOut(pathBG.c_str());//"ui/main_bg/bg_9.png");
    //actionZoomInOut("ui/main_bg/bg_9.png");
}

void TraceLayer::InitUI1()
{
    /////////////////////////////////////////
    //
    frameToTab = new CCArray();
    
    string path1 = "ui/quest/trace/quest_tap_02/quest_tap1_";
    for(int i=0;i<=27;i++){
        string path2 = "";
        path2.append(path1);
        if (i<10)path2.append("0");
        
        char buf[2];
        sprintf(buf, "%d",i);
        path2.append(buf).append(".png");
        
        //CCLog("path2 :%s", path2.c_str());
        
        CCSpriteFrame* frame0  = CCSpriteFrame::create(path2.c_str(), CCRectMake(0,0,accp(150),accp(150)));
        
        frameToTab->addObject(frame0);
    }
    
    actionTab();
 
    /*
    StageClearPopup *popup = new StageClearPopup();
    popup->InitUI();
    this->addChild(popup,1000);
*/
 
}

void TraceLayer::InitUI2()
{
    ////////////////////////////////////////
    //
    CCSpriteFrame* frame0  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_00.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame1  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_01.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame2  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_02.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame3  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_03.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame4  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_04.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame5  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_05.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame6  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_06.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame7  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_07.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame8  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_08.png", CCRectMake(0,0,accp(320),accp(290)));
    CCSpriteFrame* frame9  = CCSpriteFrame::create("ui/quest/trace/quest_go/quest_click_01_09.png", CCRectMake(0,0,accp(320),accp(290)));

    frameToGo = new CCArray();
    
    frameToGo->addObject(frame0);
    frameToGo->addObject(frame1);
    frameToGo->addObject(frame2);
    frameToGo->addObject(frame3);
    frameToGo->addObject(frame4);
    frameToGo->addObject(frame5);
    frameToGo->addObject(frame6);
    frameToGo->addObject(frame7);
    frameToGo->addObject(frame8);
    frameToGo->addObject(frame9);
    
    actionGo();
}



void TraceLayer::initTeamUI()//int ccp_y)
{
/*
    int ccp_y = 95;
    
    removeTeamUI();
    
    CCSprite* pSpr1 = CCSprite::create("ui/quest/trace/trace_team_bg.png");
    pSpr1->setAnchorPoint(ccp(0,0));
    pSpr1->setPosition( accp(0,ccp_y));
    pSpr1->setTag(79);
    this->addChild(pSpr1, 10);
    
    refreshTeamUI(ccp_y);
    
    /////// menu
    CCMenuItemImage *pSprBtnL = CCMenuItemImage::create("ui/quest/trace/trace_team_btn_next_a1.png", "ui/quest/trace/trace_team_btn_next_a2.png", this, menu_selector(TraceLayer::teamChangeCallback));
    pSprBtnL->setAnchorPoint( ccp(0,0));
    pSprBtnL->setPosition( accp(6,27));    // size.width/5 * 0,0));
    pSprBtnL->setTag(40);
    
    CCMenuItemImage *pSprBtnR = CCMenuItemImage::create("ui/quest/trace/trace_team_btn_r_next_a1.png", "ui/quest/trace/trace_team_btn_r_next_a2.png", this, menu_selector(TraceLayer::teamChangeCallback));
    pSprBtnR->setAnchorPoint( ccp(0,0));
    pSprBtnR->setPosition( accp(574,27));    // size.width/5 * 0,0));
    pSprBtnR->setTag(41);
    
    CCMenuItemImage *pSprBtnSkill = CCMenuItemImage::create("ui/quest/trace/trace_team_btn_skill_a1.png", "ui/quest/trace/trace_team_btn_skill_a2.png", this, menu_selector(TraceLayer::teamChangeCallback));
    pSprBtnSkill->setAnchorPoint( ccp(0,0));
    pSprBtnSkill->setPosition( accp(490,33));    // size.width/5 * 0,0));
    pSprBtnSkill->setTag(42);
    
    CCMenu* pMenu = CCMenu::create(pSprBtnL, pSprBtnR, pSprBtnSkill, NULL);
    pMenu->setAnchorPoint(ccp(0,0));
    pMenu->setPosition( accp(0, ccp_y));
    pMenu->setTag(78);
    
    this->addChild(pMenu, 100);
*/
}

void TraceLayer::removeTeamUI(){
/*
    this->removeChildByTag(70, true); // text
    this->removeChildByTag(71, true); // text
    this->removeChildByTag(72, true); // text
    this->removeChildByTag(73, true); // text
    this->removeChildByTag(78, true); // menu 
    this->removeChildByTag(79, true); // bg
*/
}

void TraceLayer::refreshTeamUI(int ccp_y)
{
    //removeTeamUI();
/*
    int totAp = 0;
    int totDp = 0;
    int totBp = 0;
    for (int i=0;i<5;i++){
        
        CardInfo *card = PlayerInfo::getInstance()->GetCardInDeck(0, PlayerInfo::getInstance()->traceTeam, i);
        
        if (card != NULL){
            totAp += card->getAttack();
            totDp += card->getDefence();
            totBp += card->getBp();
        }
    }
    
    char buf1[10];
    char buf2[10];
    char buf3[10];
    sprintf(buf1, "%d", totDp);
    sprintf(buf2, "%d", totAp);
    sprintf(buf3, "%d", totBp);
    
    CCLabelTTF* pLabel1 = CCLabelTTF::create(buf1, "HelveticaNeue-Bold", 12);
    pLabel1->setAnchorPoint(ccp(0, 0.5f));
    pLabel1->setColor(COLOR_YELLOW);
    pLabel1->setPosition(accp(205,ccp_y+56));
    pLabel1->setTag(70);
    this->addChild(pLabel1,10);
    
    CCLabelTTF* pLabel2 = CCLabelTTF::create(buf2, "HelveticaNeue-Bold", 12);
    pLabel2->setAnchorPoint(ccp(0, 0.5f));
    pLabel2->setColor(COLOR_YELLOW);
    pLabel2->setPosition(accp(314,ccp_y+56));
    pLabel2->setTag(71);
    this->addChild(pLabel2,10);
    
    CCLabelTTF* pLabel3 = CCLabelTTF::create(buf3, "HelveticaNeue-Bold", 12);
    pLabel3->setAnchorPoint(ccp(0, 0.5f));
    pLabel3->setColor(COLOR_YELLOW);
    pLabel3->setPosition(accp(425,ccp_y+56));
    pLabel3->setTag(72);
    this->addChild(pLabel3,10);
    
    string text = "??;
    char buf[2];
    sprintf(buf, "%d", PlayerInfo::getInstance()->traceTeam+1);
    text.append(buf);
    CCLabelTTF* pLabelTeam = CCLabelTTF::create(text.c_str(), "HelveticaNeue-Bold", 12);
    pLabelTeam->setAnchorPoint(ccp(0.5f, 0.5f));
    pLabelTeam->setColor(COLOR_WHITE);
    pLabelTeam->setPosition(accp(116,ccp_y+56));
    pLabelTeam->setTag(73);
    this->addChild(pLabelTeam,10);
 */
}

void TraceLayer::teamChangeCallback(CCObject* pSender)
{
    
    if (popupCnt>0)return;
    
    CCNode* node = (CCNode*) pSender;
    int tag = node->getTag();
    
    //CCMenuItemImage *item = (CCMenuItemImage *)node;
    switch(tag){
        case 40: // prev
            {
            PlayerInfo::getInstance()->traceTeam--;
            if (PlayerInfo::getInstance()->traceTeam<0)PlayerInfo::getInstance()->traceTeam=3;
            
                saveTraceTeam();
                
                initTeamUI();//83);
            }
            break;
        case 41: // next
            {
            PlayerInfo::getInstance()->traceTeam++;
            if (PlayerInfo::getInstance()->traceTeam>3)PlayerInfo::getInstance()->traceTeam=0;
            
                saveTraceTeam();
                
                initTeamUI();
            }
            break;
        case 42: // skill
            break;
    }
}

void TraceLayer::saveTraceTeam()
{
    CCUserDefault *gameData = CCUserDefault::sharedUserDefault();
    gameData->setIntegerForKey("traceTeam", PlayerInfo::getInstance()->traceTeam);
    gameData->flush();
    
}

void TraceLayer::refreshStageCompleteRate()
{
    
}

void TraceLayer::actionZoomInOut(const char* background)
{
    
    CCSize size = this->getContentSize();
    
    // bg_57
    
    CCSprite* pSprBackground = CCSprite::create(background);
    pSprBackground->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprBackground->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprBackground->setTag(-14);
    pSprBackground->setScale(2.1f);//2.5f);    // ??????¬¨¬®????¬¨¬®¬¨¬Æ??????250% ???
    
    CCActionInterval* extension = CCScaleBy::create(1.0f, 1.05f);
    CCActionInterval* scaledown = CCScaleBy::create(1.0f, 1.05f)->reverse();

    pSprBackground->runAction(CCRepeatForever::create((CCSequence* )CCSequence::create(extension, scaledown, NULL)));
    
    // add the sprite as a child to this layer
    this->addChild(pSprBackground, -14);
}

void TraceLayer::actionTab()
{
 
    CCSize size = this->getContentSize();
    
    CCSpriteFrame* frame = (CCSpriteFrame* )frameToTab->objectAtIndex(0);
    spriteToTab = CCSprite::createWithSpriteFrame(frame);
    spriteToTab->setAnchorPoint(ccp(0.5f, 0.5f));
    spriteToTab->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    spriteToTab->setTag(SPR_TAG_TAB);
    spriteToTab->setScale(2.0f);

    CCAnimation* animationToTab = CCAnimation::create();
    for (int i=0; i<frameToTab->count(); i++)
    {
        animationToTab->addSpriteFrame((CCSpriteFrame* )frameToTab->objectAtIndex(i));
    }
    animationToTab->setLoops(1);
    animationToTab->setDelayPerUnit(0.04f);
    
    CCAnimate* animate = CCAnimate::create(animationToTab);
    
    spriteToTab->runAction(CCRepeatForever::create(animate));
    
    this->addChild(spriteToTab, 5);
}

void TraceLayer::releaseTab(){
    spriteToTab->stopAllActions();
    this->removeChild(spriteToTab, true);
    //spriteToTab = NULL;
    //delete spriteToTab;
    
    //spriteToTab->autorelease();
}

void TraceLayer::actionGo()
{
    soundGo();
    
    removeTeamUI();
    
    this->setTouchEnabled(false);
    CCSize size = this->getContentSize();
    
    CCSpriteFrame* frame = (CCSpriteFrame* )frameToGo->objectAtIndex(0);
    spriteToGo = CCSprite::createWithSpriteFrame(frame);
    spriteToGo->setAnchorPoint(ccp(0.5f, 0.5f));
    spriteToGo->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    spriteToGo->setTag(SPR_TAG_GO);
    spriteToGo->setScale(2.0f);
    
    CCAnimation* animationToGo = CCAnimation::create();
    for (int i=0; i<frameToGo->count(); i++)
    {
        animationToGo->addSpriteFrame((CCSpriteFrame* )frameToGo->objectAtIndex(i));
    }
    animationToGo->setLoops(1);
    animationToGo->setDelayPerUnit(0.04f);
    
    CCAnimate* animate = CCAnimate::create(animationToGo);
//    CCActionInterval* delay = CCDelayTime::create(1.0f);
    CCCallFunc* actionCallFunc = CCCallFunc::create(this, callfunc_selector(TraceLayer::cbActionGo));
    
    spriteToGo->runAction((CCActionInterval* )CCSequence::create(CCRepeat::create(animate, 1), actionCallFunc, NULL));
    
    this->addChild(spriteToGo, 5);
}

void TraceLayer::releaseGo(){
    if (spriteToGo != NULL)spriteToGo->stopAllActions();    // 뻗는 경우가 있었음
    this->removeChild(spriteToGo, true);
    //spriteToGo->autorelease();
    //delete spriteToGo;
}

void TraceLayer::cbActionGo()
{

    
    CCSize size = this->getContentSize();
    spriteToGo->setVisible(false);
    
    CCSprite* pSprGo1 = CCSprite::create("ui/quest/trace/quest_go/quest_click_stop.png");
    pSprGo1->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGo1->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprGo1->setTag(998);
    pSprGo1->setScale(2.0f);
    
    this->addChild(pSprGo1, 6);
    
    
    CCSprite* pSprGo2 = CCSprite::create("ui/quest/trace/quest_go/quest_click_go_type.png");
    pSprGo2->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprGo2->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprGo2->setTag(999);
    pSprGo2->setScale(1.0f);
    
    CCActionInterval* extension = CCScaleTo::create(1.0f, 3.0f);
    CCActionInterval* fadeout   = CCFadeTo::create(1.0f, 0.05f);
    CCCallFunc* actionCallFunc = CCCallFunc::create(this, callfunc_selector(TraceLayer::cbActionSurprised));
    
    pSprGo2->runAction(CCSequence::create(CCSpawn::create(extension, fadeout, NULL), actionCallFunc, NULL));
    
    this->addChild(pSprGo2, 7);
    
}

void TraceLayer::cbActionSurprised()
{
    
    spriteToGo->setVisible(false);
    this->removeChildByTag(998, true);
    this->removeChildByTag(999, true);

    // 챕터에 맞는 배경 이미지 경로 생성
    
    string pathBG = CCFileUtils::sharedFileUtils()->getDocumentPath();
    
    // 다운로드 받지 않고 로컬 테스트
//    string pathBG = "ui/main_bg/";

    Quest_Data* questInfo = FileManager::sharedFileManager()->GetQuestInfo(questID);
    pathBG = pathBG + questInfo->stageBG_L;
    
    actionSurprised(pathBG.c_str());//"ui/main_bg/bg_9.png");
}

void TraceLayer::actionSurprised(const char *background)
{
    runEffect();
    CCSize size = this->getContentSize();
    
    
    CCSprite* pSprRunEffect = CCSprite::create("ui/quest/trace/quest_run/quest_run_spotline_00.png");
    pSprRunEffect->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprRunEffect->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(20.0f)));
    pSprRunEffect->setTag(-7);
    
    CCActionInterval* extensionRunEffect = CCScaleTo::create((3.0f - 0.0f) / 30.0f, 1.1f);
    CCActionInterval* scaledownRunEffect = CCScaleTo::create((6.0f - 3.0f) / 30.0f, 1.0f);
    pSprRunEffect->runAction(CCRepeat::create(CCSequence::create(extensionRunEffect, scaledownRunEffect, NULL), 4));
    
    // add the sprite as a child to this layer
    this->addChild(pSprRunEffect, -7);
  
    
    CCSprite* pSprPopBg0 = CCSprite::create(background);
    pSprPopBg0->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopBg0->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprPopBg0->setScale(2.1f);//2.5f);
    pSprPopBg0->setTag(-9);
/*
    CCActionInterval* zoomin = CCScaleTo::create((46.0f - 0.0f) / 30.0f*10, 2.5f * 1.2f);
    CCActionInterval* fadeout = CCFadeTo::create(0.3f, 0.0f);
 */
    CCActionInterval* zoomInSlow = CCScaleTo::create((21.0f -  0.0f) / 30.0f, pSprPopBg0->getScale()*1.1f);
    CCActionInterval* zoomInFast = CCScaleTo::create((46.0f - 21.0f) / 30.0f, pSprPopBg0->getScale()*1.3f);
//    CCActionInterval* jumpGo1 = CCJumpBy::create(0.7f / 30.0f, accp(0.0f, 0.0f), 7.5f, 1);
//    CCActionInterval* jumpGo2 = CCJumpBy::create(0.7f / 30.0f, accp(0.0f, 0.0f), 7.5f, 1);
//    CCActionInterval* jumpGo3 = CCJumpBy::create(0.7f / 30.0f, accp(0.0f, 0.0f), 7.5f, 1);
//    CCActionInterval* jumpGo4 = CCJumpBy::create(0.7f / 30.0f, accp(0.0f, 0.0f), 7.5f, 1);
    CCActionInterval* fadeOutGo = CCFadeTo::create(15.0f / 30.0f, 0.0f);
    CCActionInterval* disappear = CCMoveBy::create(15.0f / 30.0f, accp(0.0f, -100.0f));
    CCCallFunc* callBackActionEntry = CCCallFunc::create(this, callfunc_selector(TraceLayer::cbActionEntry));
    
    pSprPopBg0->runAction(CCSequence::create(zoomInSlow, CCSpawn::create(zoomInFast, /*CCSequence::create(jumpGo1, jumpGo2, jumpGo3, jumpGo4, NULL),*/ NULL), CCSpawn::create(fadeOutGo, disappear, NULL), callBackActionEntry, NULL));
    
    this->addChild(pSprPopBg0, -12);
    
    
    CCSprite* pSprPopBg1 = CCSprite::create(background);
    pSprPopBg1->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopBg1->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprPopBg1->setScale(2.1f);//2.5f);
    pSprPopBg1->setTag(-10);
    pSprPopBg1->setBlendFunc((ccBlendFunc){GL_SRC_ALPHA, GL_ONE});
    
    CCActionInterval* extensionPopBg1 = CCScaleTo::create((10.0f - 0.0f) / 30.0f, 3.5f * 1.6f);
    CCActionInterval* fadeoutPopBg1 = CCFadeTo::create((10.0f - 0.0f) / 30.0f, 0.0f);
    CCActionInterval* tintoutPopBg1 = CCTintTo::create((10.0f - 0.0f) / 30.0f, 255.0f, 255.0f, 255.0f);
    CCActionInterval* moveupPopBg1 = CCMoveBy::create((10.0f - 0.0f) / 30.0f, accp(0.0f, 20.0f));
    pSprPopBg1->runAction(CCSpawn::create(extensionPopBg1, fadeoutPopBg1, tintoutPopBg1, moveupPopBg1, NULL));
    
    // add the sprite as a child to this layer
    this->addChild(pSprPopBg1, -9);
    
    
    CCSprite* pSprPopBg2 = CCSprite::create(background);
    pSprPopBg2->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopBg2->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprPopBg2->setOpacity(0.0f);
    pSprPopBg2->setScale(2.1f);//2.5f);
    pSprPopBg2->setTag(-11);
    pSprPopBg2->setBlendFunc((ccBlendFunc){GL_SRC_ALPHA, GL_ONE});
    
    CCActionInterval* delayPopBg2 = CCDelayTime::create((7.0f - 0.0f) / 30.0f);
    CCActionInterval* showPopBg2 = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* extensionPopBg2 = CCScaleTo::create ((17.0f - 7.0f) / 30.0f, 3.5f * 1.6f);
    CCActionInterval* fadeoutPopBg2 = CCFadeTo::create((17.0f - 7.0f) / 30.0f, 0.0f);
    CCActionInterval* moveupPopBg2 = CCMoveBy::create((17.0f - 7.0f) / 30.0f, accp(0.0f, 20.0f));
    pSprPopBg2->runAction(CCSequence::create(delayPopBg2, showPopBg2, CCSpawn::create(extensionPopBg2, fadeoutPopBg2, moveupPopBg2, NULL), NULL));
    
    // add the sprite as a child to this layer
    this->addChild(pSprPopBg2, -10);
    
    
    CCSprite* pSprPopBg3 = CCSprite::create(background);
    pSprPopBg3->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopBg3->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprPopBg3->setOpacity(0.0f);
    pSprPopBg3->setScale(2.1f);//2.5f);
    pSprPopBg3->setTag(-12);
    pSprPopBg3->setBlendFunc((ccBlendFunc){GL_SRC_ALPHA, GL_ONE});
    
    CCActionInterval* delayPopBg3 = CCDelayTime::create((14.0f - 0.0f) / 30.0f);
    CCActionInterval* showPopBg3 = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* extensionPopBg3 = CCScaleTo::create((24.0f - 14.0f) / 30.0f, 3.5f * 1.6f);
    CCActionInterval* fadeoutPopBg3 = CCFadeTo::create((24.0f - 14.0f) / 30.0f, 0.0f);
    CCActionInterval* moveupPopBg3 = CCMoveBy::create((24.0f - 14.0f) / 30.0f, accp(0.0f, 20.0f));
    pSprPopBg3->runAction(CCSequence::create(delayPopBg3, showPopBg3, CCSpawn::create(extensionPopBg3, fadeoutPopBg3, moveupPopBg3, NULL), NULL));
    
    // add the sprite as a child to this layer
    this->addChild(pSprPopBg3, -11);
    
    
    CCSprite* pSprPopBg4 = CCSprite::create(background);
    pSprPopBg4->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprPopBg4->setPosition(ccp(size.width/2.0f, size.height/2.0f + accp(15.0f)));
    pSprPopBg4->setOpacity(0.0f);
    pSprPopBg4->setScale(2.1f);//2.5f);
    pSprPopBg4->setTag(-13);
    pSprPopBg4->setBlendFunc((ccBlendFunc){GL_SRC_ALPHA, GL_ONE});
    
    CCActionInterval* delayPopBg4 = CCDelayTime::create((21.0f - 0.0f) / 30.0f);
    CCActionInterval* showPopBg4 = CCFadeTo::create(0.0f, 255.0f);
    CCActionInterval* extensionPopBg4 = CCScaleTo::create((46.0f - 21.0f) / 30.0f, 3.5f * 1.6f);
    CCActionInterval* fadeoutPopBg4 = CCFadeTo::create((46.0f - 21.0f) / 30.0f, 0.0f);
    CCActionInterval* moveupPopBg4 = CCMoveBy::create((46.0f - 21.0f) / 30.0f, accp(0.0f, 20.0f));
    pSprPopBg4->runAction(CCSequence::create(delayPopBg4, showPopBg4, CCSpawn::create(extensionPopBg4, fadeoutPopBg4, moveupPopBg4, NULL), NULL));
    
    // add the sprite as a child to this layer
    this->addChild(pSprPopBg4, 12);
    
/*
    CCSprite* pSprBackground = CCSprite::create(background);
    pSprBackground->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprBackground->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprBackground->setTag(-9);
    pSprBackground->setScale(3.5f);
    pSprBackground->setOpacity(35);
    
    CCActionInterval* extension = CCScaleBy::create(0.43f, 1.3f);
    CCActionInterval* scaledown = CCScaleBy::create(0.01f, 1.2f)->reverse();
    CCDelayTime* delayTime = CCDelayTime::create(0.0f);
    CCCallFunc* actionCallFunc = CCCallFunc::create(this, callfunc_selector(TraceLayer::cbActionEntry));
    CCAction* pSpriteAction = CCSequence::create(CCRepeat::create((CCActionInterval* )CCSequence::create(extension, scaledown, NULL), 3), delayTime, actionCallFunc, NULL);
    
    pSprBackground->runAction(pSpriteAction);
    
    // add the sprite as a child to this layer
    this->addChild(pSprBackground, -9);
    
    
    
    
    
    CCSprite* pSprFadeOutGo = CCSprite::create(background);
    pSprFadeOutGo->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprFadeOutGo->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprFadeOutGo->setTag(-13);
    pSprFadeOutGo->setScale(2.5f);
    
//    CCActionInterval* goDelay = CCDelayTime::create(0.44f * 3.0f);
    CCActionInterval* zoomIn = CCScaleBy::create(0.44f*3, 1.3f);
    CCActionInterval* jumpGo1 = CCJumpBy::create(0.44f, accp(0.0f, 0.0f), 1.5f, 1);
    CCActionInterval* jumpGo2 = CCJumpBy::create(0.44f, accp(0.0f, 0.0f), 1.5f, 1);
    CCActionInterval* jumpGo3 = CCJumpBy::create(0.44f, accp(0.0f, 0.0f), 1.5f, 1);
    CCActionInterval* fadeOutGo = CCFadeTo::create(0.44f, 0.0f);
    CCCallFunc* callBackFadeOutGo = CCCallFunc::create(this, callfunc_selector(TraceLayer::cbFadeOutGo));
    
    pSprFadeOutGo->runAction(CCSequence::create(CCSpawn::create(zoomIn, CCSequence::create(jumpGo1, jumpGo2, jumpGo3, NULL), NULL), fadeOutGo, callBackFadeOutGo, NULL));
    
    this->addChild(pSprFadeOutGo, -10);
    
    
    
    
    

    CCLayerColor* whiteLayer = CCLayerColor::create(ccc4(255, 255, 255, 255));
    whiteLayer->setPosition(ccp(0, 0) );
    whiteLayer->setTag(-8);
    pSprBackground->setScale(2.5f);    // ??????¬¨????¬¨¬®??????250% ???
    whiteLayer->setOpacity(0);
    
    CCFadeTo* in  = CCFadeTo::create(0.43f, 60);
    CCFadeTo* out = CCFadeTo::create(0.01f, 0);
    CCAction* whiteLayerAction = CCRepeat::create(CCSequence::create(in, out, NULL), 3);
    
    whiteLayer->runAction(whiteLayerAction);
    
    // add the whiteLayer as a child to this layer
    this->addChild(whiteLayer, -8);

    
    
    
    
    
    CCSprite* pSprRunEffect = CCSprite::create("ui/quest/trace/quest_run/quest_run_spotline_00.png");
    pSprRunEffect->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprRunEffect->setPosition(ccp(size.width/2.0f, size.height/2.0f));
    pSprRunEffect->setScale(1.08f);
    pSprRunEffect->setTag(-7);
    
    // add the sprite as a child to this layer
    this->addChild(pSprRunEffect, -7);
  */
}

void TraceLayer::cbActionEntry()
{
    this->removeChildByTag(-7, true);
    this->removeChildByTag(-9, true);
//    this->removeChildByTag(-8, true);
    for (int i=-10; i>-14; i--)
    {
        this->removeChildByTag(i, true);
    }
        
    ResponseQuestUpdateResultInfo *info = ARequestSender::getInstance()->requestUpdateQuestResult(this->questID, 0, PlayerInfo::getInstance()->traceTeam, true);
    setTraceResult(info);
}


int TraceLayer::getChaseStep()
{
    return nChaseStep;
}

void TraceLayer::setUserStat(ResponseRivalBattle* result)
{
    PlayerInfo::getInstance()->setLevel(result->user_lev);
    PlayerInfo::getInstance()->setXp(result->user_exp);
    PlayerInfo::getInstance()->setCoin(result->user_coin);
    PlayerInfo::getInstance()->setCash(result->user_gold);
    PlayerInfo::getInstance()->setUpgradePoint(result->user_uPnt);
    PlayerInfo::getInstance()->setBattlePoint(result->user_attackPoint);
    //PlayerInfo::getInstance()->setDefensePoint(result->user_defensePoint);
    PlayerInfo::getInstance()->setStamina(result->user_questPoint);
    PlayerInfo::getInstance()->setRevengePoint(result->user_revenge);
    
}

void TraceLayer::setUserStat(ResponseQuestUpdateResultInfo* result)
{
    PlayerInfo::getInstance()->setLevel(result->user_lev);
    PlayerInfo::getInstance()->setXp(result->user_exp);
    PlayerInfo::getInstance()->setMaxXp(result->user_exp_max);
    PlayerInfo::getInstance()->setCoin(result->user_coin);
    PlayerInfo::getInstance()->setUpgradePoint(result->user_uPnt);
    PlayerInfo::getInstance()->setBattlePoint(result->user_attackPoint); // battle point

    //PlayerInfo::getInstance()->setDefensePoint(result->user_defensePoint);
    PlayerInfo::getInstance()->setStamina(result->user_questPoint);  // stamina
    PlayerInfo::getInstance()->setRevengePoint(result->user_revenge);
}

void TraceLayer::setTraceResult(ResponseQuestUpdateResultInfo *resultInfo)
{
    receivedTraceResultInfo = resultInfo;
    
    if(NULL != resultInfo)
    {
        if (atoi(resultInfo->res) != 0){
            popupNetworkError(resultInfo->res, resultInfo->msg, "requestUpdateQuestResult");
            setChaseStep(CHASE_TAB);
            return;
        }
    }
    else{
        popupNetworkError("Network Error", "quest error", "requestUpdateQuestResult");
        return;
    }
    
    rid_for_rivalLayer = receivedTraceResultInfo->rid;
    
    int prevLev = PlayerInfo::getInstance()->getLevel();
    
    
    
    
    
    if(NULL != resultInfo)
    {
        if (atoi(resultInfo->res) != 0) {
            popupNetworkError(resultInfo->res, resultInfo->msg, "requestUpdateQuestResult");
            setChaseStep(CHASE_TAB);
            return;
        }
        
        questProgress =  resultInfo->progress;
        
        // user stat refresh
        setUserStat(resultInfo);
        
        // add card
        if (resultInfo->cards->count() > 0){
            rewardCards = new CCArray();
            for (int i=0;i<resultInfo->cards->count();i++){
                QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)resultInfo->cards->objectAtIndex(i);
                
                CardInfo *cardInfo = new CardInfo();
                cardInfo->autorelease();
                cardInfo->setId(rewardCard->card_id);
                cardInfo->setSrl(rewardCard->card_srl);
                cardInfo->setExp(rewardCard->card_exp);
                cardInfo->setLevel(rewardCard->card_level);
                cardInfo->setAttack(rewardCard->card_attack);
                cardInfo->setDefence(rewardCard->card_defense);
                cardInfo->setSkillEffect(rewardCard->card_skillEffect);
                //cardInfo->getRare()
                
                
                CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
                PlayerInfo::getInstance()->addToMyCardList(newCard);
                rewardCards->addObject(newCard);
                //CCLog("rewardcard, id:%d rare:%d", newCard->getId(), newCard->getRare());
            }
        }
    }
    
    int event = TRACE_EVENT_DRUM_CRASH;
    
    /////////////////////////////////////////////////////////////////////////
    
    ///////////////////////////////
    // for test - bbq test
    //resultInfo->sp = 50;
    //resultInfo->bp = 10;
    ///////////////////////////////
    
    ///////////////////////////////
    // for test - normarl enemy
    /*
    resultInfo->enemy_type = 1;
    resultInfo->enemy_code = 101;
    */
    ///////////////////////////////
    
    ///////////////////////////////
    // for test - rival test
    /*
     resultInfo->enemy_type = 2;
     resultInfo->enemy_code = 101;
     receivedTraceResultInfo = new ResponseQuestUpdateResultInfo();
     receivedTraceResultInfo->enemy_code = 101;
     */
    ///////////////////////////////
    
    ///////////////////////////////
    // for test - hidden rival test
    /*
     resultInfo->enemy_type = 3;
     resultInfo->enemy_code = 1001;
     receivedTraceResultInfo = new ResponseQuestUpdateResultInfo();
     receivedTraceResultInfo->enemy_code = 1001;
     */
    // rid Í∞????ÎØ?? ?±Ï?Îß??????Í∞????
    ///////////////////////////////
    
    ///////////////////////////////
    // for test - boss test
    /*
     resultInfo = new ResponseQuestUpdateResultInfo();
     resultInfo->enemy_type = 4;
     resultInfo->enemy_code = 101;
     resultInfo->coin = 10;
     resultInfo->cards = new CCArray();
     receivedTraceResultInfo = resultInfo;
     */
    ///////////////////////////////
    ///////////////////////////////
    /*
     // for test - exp test
     resultInfo->enemy_type = 0;
     resultInfo = new ResponseQuestUpdateResultInfo();
     //resultInfo->exp = 10;
     //resultInfo->coin = 10;
     resultInfo->sp = 10;
     resultInfo->bp = 10;
     resultInfo->cards = new CCArray();
     */
    ///////////////////////////////
    
    // for test - card test
    /*
     resultInfo = new ResponseQuestUpdateResultInfo();
     resultInfo->enemy_type = 0;
     resultInfo->coin = 0;
     resultInfo->cards = new CCArray();
     resultInfo->res = "0";
     
     QuestRewardCardInfo *card1 = new QuestRewardCardInfo();
     card1->card_id = 30012;
     resultInfo->cards->addObject(card1);
     
     QuestRewardCardInfo *card2 = new QuestRewardCardInfo();
     card2->card_id = 30624;//30013;
     resultInfo->cards->addObject(card2);
     */
    /*
     if (resultInfo->cards->count() > 0){
     rewardCards = new CCArray();
     for (int i=0;i<resultInfo->cards->count();i++){
     QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)resultInfo->cards->objectAtIndex(i);
     
     CardInfo *cardInfo = new CardInfo();
     cardInfo->autorelease();
     cardInfo->setId(rewardCard->card_id);
     cardInfo->setSrl(rewardCard->card_srl);
     cardInfo->setExp(rewardCard->card_exp);
     cardInfo->setLevel(rewardCard->card_level);
     cardInfo->setAttack(rewardCard->card_attack);
     cardInfo->setDefence(rewardCard->card_defense);
     
     CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
     PlayerInfo::getInstance()->addToMyCardList(newCard);
     rewardCards->addObject(newCard);
     }
     }
     */
    
    ///////////////////////////////
    
    ///////////////////////////////
    // for level up test
    //prevLev =  PlayerInfo::getInstance()->getLevel() - 1;
    ///////////////////////////////
    
    /////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////
    
    if (resultInfo->enemy_type != 0){
        switch (resultInfo->enemy_type) {
            case 1:
                event = TRACE_EVENT_NORMAL_ENEMY;
                CCLOG(" event = normal enemy");
                break;
            case 2:
                event = TRACE_EVENT_RIVAL;
                
                // for test
                //event = TRACE_EVENT_HIDDEN_RIVAL;
                //resultInfo->enemy_type = 3;
                CCLOG(" event = rival");
                break;
            case 3:
                event = TRACE_EVENT_HIDDEN_RIVAL;
                CCLOG(" event = hiddle rival");
                break;
            case 4:
                event = TRACE_EVENT_BOSS;
                BossID = questID;
                BossHP = resultInfo->enemy_hp;
                CCLOG(" event = boss");
                break;
        }
        
        actionEnemy(resultInfo->enemy_type);
        return;
    }
    
    
    
    rewardEventQueue = new CCArray();
    
    if (resultInfo->coin == 0 && resultInfo->cards->count() == 0 && resultInfo->sp==0){
        //event = TRACE_EVENT_EXP_UP;
        //CCLOG(" event = exp up");
        
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_EXP_UP;
        eventObj->exp = resultInfo->exp;
        rewardEventQueue->addObject(eventObj);
    }
    else{
        
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_DRUM_CRASH;
        rewardEventQueue->addObject(eventObj);
        
        if (resultInfo->coin > 0){
            // coin - card - stamina event
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_COIN;
            eventObj->coin = resultInfo->coin;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (resultInfo->cards->count()>0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_CARD;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (resultInfo->bp > 0 && resultInfo->sp > 0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_BBQ;
            eventObj->sp = resultInfo->sp;
            eventObj->bp = resultInfo->bp;
            rewardEventQueue->addObject(eventObj);
        }
    }
    
    if (prevLev < PlayerInfo::getInstance()->getLevel()){
        // user level up
        EventObject *eventObj1 = new EventObject();
        eventObj1->eventCode = TRACE_EVENT_LEVEL_UP_AFTER_TRACE;
        rewardEventQueue->addObject(eventObj1);
    }
    
    
    CCLog(" resultInfo->action :%d resultInfo->enemy:%d battleResult:%d", resultInfo->action, resultInfo->enemy, resultInfo->questBattleResult);
    
    //1,0,1
    // action == battle(1) & enemy == boss(2) & res == win(1)
    if (resultInfo->action == 1 && resultInfo->enemy == 2 && resultInfo->questBattleResult == 1){
        EventObject *eventObj1 = new EventObject();
        eventObj1->eventCode = TRACE_EVENT_STAGE_CLEAR;
        rewardEventQueue->addObject(eventObj1);
    }
    
    
    for(int i=0;i<rewardEventQueue->count();i++){
        EventObject *eventObj = (EventObject*)rewardEventQueue->objectAtIndex(i);
        CCLog(" reward event : %d", eventObj->eventCode);
    }
    
    //CCLog("setTraceResult 100");
    ///////////////////////////////////
    EventObject *eventObj = (EventObject*)rewardEventQueue->objectAtIndex(0);
    
    //CCLog("setTraceResult 200, eventObj->eventCode:%d",eventObj->eventCode);
    
    //switch(event){
    switch(eventObj->eventCode){
        case TRACE_EVENT_DRUM_CRASH:
            actionDrumCrash();
            break;
        case TRACE_EVENT_EXP_UP:
            actionExpUp(0, eventObj->exp);
            //actionExpUp(0, resultInfo->exp);
            break;
         /*
          // ??? ?¬•??????????????¬∞??¬®??????? ???¬∂¬®??? ????? 
          // ????????????????¬∞????¬∂¬®??? ?????? ????????
          // ??? - ??? - ??¬•?? - ?¬ß?????????????¬∞?????????¬∞??¬•????run???.
        case TRACE_EVENT_COIN:
            actionGetCoin(0, eventObj->coin);
            //actionGetCoin(0, resultInfo->coin);
            break;
        case TRACE_EVENT_CARD:
            actionGetCard(0);
            break;
        case TRACE_EVENT_STAMINA:
            actionGetStamina(eventObj->stamina);
            //actionGetStamina(resultInfo->stamina);
            break;
            */
//        case TRACE_EVENT_NORMAL_ENEMY:
//            actionNormalEnemy();
//            break;
//        case TRACE_EVENT_RIVAL:
//            actionRival();
//            break;
//        case TRACE_EVENT_HIDDEN_RIVAL:
//            actionHiddenRival();
//            break;
//        case TRACE_EVENT_BOSS:
//            actionBoss();
//            break;
    }
    
    //CCLog("setTraceResult 300");
}


void TraceLayer::InitLevelUplayer(int uPoint)
{
    //int uPoint = 0;
    //if (receivedTraceResultInfo != NULL){
    //    uPoint = receivedTraceResultInfo->user_uPnt;
    //}
    
    actionLevelUP = new LevelUpAction(this->getContentSize(), uPoint); //receivedTraceResultInfo->user_uPnt);
    actionLevelUP->setAnchorPoint(ccp(0.0f, 0.0f));
    actionLevelUP->setTag(9999);
    actionLevelUP->setPosition(accp(0.0f, 0.0f));
    MainScene::getInstance()->addChild(actionLevelUP, 13000);
    
}


void TraceLayer::CloseLevelUpLayer(){
    
    this->setTouchEnabled(true);
    
    if (bOpenLevelUpAfterTrace){
        bOpenLevelUpAfterTrace = false;
        setChaseStep(CHASE_TAB);
    }
    else if (bOpenLevelUpAfterBattle){
        bOpenLevelUpAfterBattle = false;
        RewardEventManager();
    }
    else{
        
    }
}

void TraceLayer::actionExpUp(int nCallBack, int val)
{
    CCSize size = this->getContentSize();

    
    // size == 320,480
    

    /////////////////////////////////////////
    //
    // ?????? ?????????? ?¬¨¬®???????¬¨¬®????????????? ?¬¨¬®???
    
    //CCSprite* sparkle = CCSprite::create("ui/quest/trace/quest_item/quest_mid_bg.png");
    CCSprite* sparkle = CCSprite::create("ui/quest/trace/message_bg.png");
    sparkle->setAnchorPoint(ccp(0.5f, 0.5f));
    sparkle->setPosition(ccp(size.width/2, size.height/2));
    sparkle->setScale(0.2);
    sparkle->setTag(494);
    
    //CCActionInterval* sparkleExtension = CCScaleTo::create(0.5f, 0.8f);
    CCActionInterval* sparkleExtension = CCScaleTo::create(0.5f, 2.0f);
    CCActionInterval* sparkleDelay = CCDelayTime::create(1.2f);
    CCActionInterval* sparkleDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* sparkleFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    sparkle->runAction(CCSequence::create(sparkleExtension, sparkleDelay, CCSpawn::create(sparkleDrop, sparkleFadeOut, NULL), NULL));
    
    this->addChild(sparkle, 0);
    
    /////////////////////////////////////////
    //
    // ??????¬¨¬®???????¬¨¬®???????????????????¬¨¬®???¬¨¬®¬¨¬µ?? ?¬¨¬®???
    
    CCSprite* expUp = CCSprite::create("ui/quest/trace/quest_item/icon_exp.png");
    expUp->setAnchorPoint(ccp(0.5f, 0.0f));
    expUp->setPosition(ccp(size.width/2, size.height/2));
    expUp->setScale(2.0f * 0.2f);
    expUp->setTag(495);
    
    CCCallFunc* cbPlayExpUp = CCCallFunc::create(this, callfunc_selector(MyUtil::soundExpUp));
    CCActionInterval* expUpExtension = CCScaleTo::create(0.5f, 2.0f * 0.6f);
    CCActionInterval* expUpJumping1 = CCJumpBy::create(0.4f, accp(0.0f, 0.0f), 10.0f, 1);
    CCActionInterval* expUpJumping2 = CCJumpBy::create(0.2f, accp(0.0f, 0.0f), 6.0f, 1);
    CCActionInterval* expUpJumping3 = CCJumpBy::create(0.1f, accp(0.0f, 0.0f), 3.0f, 1);
    CCActionInterval* expUpDelay1 = CCDelayTime::create(0.5f);
    CCActionInterval* expUpDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* expUpFadeOut = CCFadeTo::create(0.3f, 0.05f);
    CCActionInterval* expUpDelay2 = CCDelayTime::create(1.0f);
    
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackExpUp));

    expUp->runAction(CCSequence::create(expUpExtension, cbPlayExpUp, expUpJumping1, expUpJumping2, expUpJumping3, expUpDelay1, CCSpawn::create(expUpDrop, expUpFadeOut, expUpDelay2, NULL), cb, NULL));
    
    this->addChild(expUp, 0);
    
    std::string text_sign1 = "+ ";
    
    char buffer1[10];
    sprintf(buffer1, "%d", val);
    text_sign1.append(buffer1);
    
    std::string text_type1 = " EXP";
    text_sign1.append(text_type1);
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 20);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_WHITE);//COLOR_ORANGE);
    pLblSp->setScale(0.2f);
    pLblSp->setTag(496);
    
    CCActionInterval* spExtension = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    pLblSp->runAction(CCSequence::create(spExtension, spDelay, CCSpawn::create(spDrop, spFadeOut, NULL), NULL));
    
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, size.height/2-accp(60)), pLblSp, 6);
    
    ///
    CCSprite* pSprTextBg = CCSprite::create("ui/quest/trace/message_exp.png");
    pSprTextBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTextBg->setPosition(ccp(size.width/2, size.height/2-accp(60)));
    pSprTextBg->setScale(0.2f);
    pSprTextBg->setTag(497);
    this->addChild(pSprTextBg,5);
    
    CCActionInterval* spExtension2 = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay2 = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop2 = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut2 = CCFadeTo::create(0.3f, 0.05f);
    pSprTextBg->runAction(CCSequence::create(spExtension2, spDelay2, CCSpawn::create(spDrop2, spFadeOut2, NULL), NULL));
}

void TraceLayer::callBackExpUp(cocos2d::CCSprite *sprite)
{
    this->removeChildByTag(494, true);
    this->removeChildByTag(495, true);
    this->removeChildByTag(496, true);
    this->removeChildByTag(497, true);
    removeChild(sprite, true);
    
    RewardEventManager();
}

void TraceLayer::actionGetCoin(int nCallBack, int val)         // ??¬¨¬©??
{
    CCSize size = this->getContentSize();
    
    PlayerInfo::getInstance()->addCoin(val);
    /////////////////////////////////////////
    //
    // ?????? ?????????? ?¬¨¬®???????¬¨¬®????????????? ?¬¨¬®???
    
    //CCSprite* sparkle = CCSprite::create("ui/quest/trace/quest_item/quest_mid_bg.png");
    CCSprite* sparkle = CCSprite::create("ui/quest/trace/message_bg.png");
    sparkle->setAnchorPoint(ccp(0.5f, 0.5f));
    sparkle->setPosition(ccp(size.width/2, size.height/2));
    sparkle->setScale(0.2);
    sparkle->setTag(497);
    
    CCActionInterval* sparkleExtension = CCScaleTo::create(0.5f, 2.0f);//0.8f);
    CCActionInterval* sparkleDelay = CCDelayTime::create(1.2f);
    CCActionInterval* sparkleDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* sparkleFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    sparkle->runAction(CCSequence::create(sparkleExtension, sparkleDelay, CCSpawn::create(sparkleDrop, sparkleFadeOut, NULL), NULL));
    
    this->addChild(sparkle, 0);
    
    /////////////////////////////////////////
    //
    // ??????¬¨¬®???????¬¨¬®???????????????????¬¨¬®???¬¨¬®¬¨¬µ?? ?¬¨¬®???
    
    CCSprite* chicken = CCSprite::create("ui/quest/trace/quest_item/item_coin_b.png");
    chicken->setAnchorPoint(ccp(0.5f, 0.0f));
    chicken->setPosition(ccp(size.width/2, size.height/2));
    chicken->setScale(2.0f * 0.2f);
    chicken->setTag(498);
    
    CCCallFunc* cbPlayCoin = CCCallFunc::create(this, callfunc_selector(MyUtil::soundCoin));
    CCActionInterval* chickenExtension = CCScaleTo::create(0.5f, 2.0f * 0.6f);
    CCActionInterval* chickenJumping1 = CCJumpBy::create(0.4f, accp(0.0f, 0.0f), 10.0f, 1);
    CCActionInterval* chickenJumping2 = CCJumpBy::create(0.2f, accp(0.0f, 0.0f), 6.0f, 1);
    CCActionInterval* chickenJumping3 = CCJumpBy::create(0.1f, accp(0.0f, 0.0f), 3.0f, 1);
    CCActionInterval* chickenDelay = CCDelayTime::create(0.5f);
    CCActionInterval* chickenDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* chickenFadeOut = CCFadeTo::create(0.3f, 0.05f);
    CCActionInterval* delay2 = CCDelayTime::create(1.0f);
    
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackCoin0));
    /*
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackCoin0));
    if (nCallBack==1)cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackCoin1));
    else if (nCallBack==2)cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackCoin2));
    */
    chicken->runAction(CCSequence::create(chickenExtension, cbPlayCoin, chickenJumping1,chickenJumping2, chickenJumping3, chickenDelay, CCSpawn::create(chickenDrop, chickenFadeOut, delay2, NULL), cb, NULL));
    
    this->addChild(chicken, 0);
    
    
    std::string text_sign1 = "+ ";
    
    char buffer1[10];
    sprintf(buffer1, "%d", val);
    text_sign1.append(buffer1);
    
    std::string text_type1 = " COINS";
    text_sign1.append(text_type1);
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 20);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_WHITE);//COLOR_ORANGE);
    pLblSp->setScale(0.2f);
    pLblSp->setTag(499);
    
    CCActionInterval* spExtension = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    pLblSp->runAction(CCSequence::create(spExtension, spDelay, CCSpawn::create(spDrop, spFadeOut, NULL), NULL));
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, size.height/2-accp(60)), pLblSp, 6);
    
    ///
    CCSprite* pSprTextBg = CCSprite::create("ui/quest/trace/message_coin.png");
    pSprTextBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTextBg->setPosition(ccp(size.width/2, size.height/2-accp(60)));
    pSprTextBg->setScale(0.2f);
    pSprTextBg->setTag(496);
    this->addChild(pSprTextBg,5);
    
    CCActionInterval* spExtension2 = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay2 = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop2 = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut2 = CCFadeTo::create(0.3f, 0.05f);
    pSprTextBg->runAction(CCSequence::create(spExtension2, spDelay2, CCSpawn::create(spDrop2, spFadeOut2, NULL), NULL));
}

void TraceLayer::callBackCoin0()
{
    // remove coin sprite, action,
    this->removeChildByTag(496, true);
    this->removeChildByTag(497, true);
    this->removeChildByTag(498, true);
    this->removeChildByTag(499, true);
    RewardEventManager();
}

void TraceLayer::actionGetCard(int nCallBack)         // ??¬¨¬®???
{
    // test
    
    CCSize size = this->getContentSize();
    
    /*
    std::string text_sign1 = "NEW CARD";
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 20);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_ORANGE);
    pLblSp->setScale(0.2f);
    pLblSp->setTag(499);
    */
    
    CCSprite* pSpr = CCSprite::create("ui/quest/trace/message_newcard.png");
    pSpr->setAnchorPoint(ccp(0.5f, 1.0f));
    pSpr->setScale(0.2f);
    pSpr->setTag(499);
    pSpr->setPosition(ccp(size.width/2, size.height/2));//-accp(20+20)));
    
    
    CCActionInterval* spExtension = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    //CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::actionShowCard));
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::startCardAction));
    
    pSpr->runAction(CCSequence::create(spExtension, spDelay, CCSpawn::create(spDrop, spFadeOut, NULL), cb, NULL));
    
    this->addChild(pSpr,5);
    //registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, size.height/2-accp(20+20)), pLblSp, 5);
    
    actionCallFrom = nCallBack;
    
}

void TraceLayer::actionShowCard()
{
    
    if (rewardCards->count() > 0){
        CCSize size = this->getContentSize();
        CardInfo *newCard = (CardInfo*)rewardCards->objectAtIndex(0);
        CardDetailViewLayer *_cardDetailView = new CardDetailViewLayer(CCSize(size.width, size.height), newCard, NULL, DIRECTION_NEWCARD);
        _cardDetailView->setTag(1010);
        this->addChild(_cardDetailView,1000);
        rewardCards->removeObjectAtIndex(0);
        
        this->schedule(schedule_selector(TraceLayer::callBackShowCard), 3.0f);
    }
    else{
        callBackRewardCard();
    }
}

void TraceLayer::callBackShowCard()
{
    this->removeChildByTag(1010,true);
    
    this->unschedule(schedule_selector(TraceLayer::callBackShowCard));
    
    actionShowCard();
    
}

void TraceLayer::callBackRewardCard()
{
    this->removeChildByTag(1010,true);
    this->removeChildByTag(499, true);
    RewardEventManager();
}


void TraceLayer::actionGetStamina(int val)      // ?¬¨¬®????????
{
    
    PlayerInfo::getInstance()->addStamina(val);
    
    CCSize size = this->getContentSize();
    
    //CCSprite* sparkle = CCSprite::create("ui/quest/trace/quest_item/quest_mid_bg.png");
    CCSprite* sparkle = CCSprite::create("ui/quest/trace/message_bg.png");
    sparkle->setAnchorPoint(ccp(0.5f, 0.5f));
    sparkle->setPosition(ccp(size.width/2, size.height/2));
    sparkle->setScale(0.2);
    sparkle->setTag(485);
    
    CCActionInterval* sparkleExtension = CCScaleTo::create(0.5f, 2.0f);//0.8f);
    CCActionInterval* sparkleDelay = CCDelayTime::create(1.2f);
    CCActionInterval* sparkleDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* sparkleFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    sparkle->runAction(CCSequence::create(sparkleExtension, sparkleDelay, CCSpawn::create(sparkleDrop, sparkleFadeOut, NULL), NULL));
    
    this->addChild(sparkle, 0);
    
    /////////////////////////////////////////
    //
    // ??????¬¨¬®???????¬¨¬®???????????????????¬¨¬®???¬¨¬®¬¨¬µ?? ?¬¨¬®???
    
    CCSprite* chicken = CCSprite::create("ui/quest/trace/quest_item/item_bar_2.png");
    chicken->setAnchorPoint(ccp(0.5f, 0.0f));
    chicken->setPosition(ccp(size.width/2, size.height/2));
    chicken->setScale(0.2f);
    chicken->setTag(486);
    
    CCCallFunc* cbPlayChicken = CCCallFunc::create(this, callfunc_selector(MyUtil::soundExpUp));
    CCActionInterval* chickenExtension = CCScaleTo::create(0.5f, 0.6f);
    CCActionInterval* chickenJumping1 = CCJumpBy::create(0.4f, accp(0.0f, 0.0f), 10.0f, 1);
    CCActionInterval* chickenJumping2 = CCJumpBy::create(0.2f, accp(0.0f, 0.0f), 6.0f, 1);
    CCActionInterval* chickenJumping3 = CCJumpBy::create(0.1f, accp(0.0f, 0.0f), 3.0f, 1);
    CCActionInterval* chickenDelay = CCDelayTime::create(0.5f);
    CCActionInterval* chickenDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* chickenFadeOut = CCFadeTo::create(0.3f, 0.05f);
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackStamina));
    
    chicken->runAction(CCSequence::create(chickenExtension, cbPlayChicken, chickenJumping1, chickenJumping2, chickenJumping3, chickenDelay, CCSpawn::create(chickenDrop, chickenFadeOut, NULL), cb, NULL));
    
    this->addChild(chicken, 0);
    
    
    std::string text_sign1 = "+ ";
    std::string text_sign2 = "\n+ ";
    
    char buffer1[10];
    sprintf(buffer1, "%d", receivedTraceResultInfo->sp);
    text_sign1.append(buffer1);
    
    char buffer2[10];
    sprintf(buffer2, "%d", receivedTraceResultInfo->bp);
    text_sign2.append(buffer2);
    
    std::string text_type1 = " 스테미나";//" SP";
    text_sign1.append(text_type1);
    
    std::string text_type2 = " 배틀포인트";//" BP";
    text_sign2.append(text_type2);
    
    text_sign1.append(text_sign2);
    
    CCLabelTTF* pLblSp = CCLabelTTF::create(text_sign1.c_str(), "HelveticaNeue-Bold", 18);
    pLblSp->setAnchorPoint(ccp(0.5f, 1.0f));
    pLblSp->setColor(COLOR_WHITE);//COLOR_ORANGE);
    pLblSp->setScale(0.2f);
    pLblSp->setTag(487);
    
    CCActionInterval* spExtension = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut = CCFadeTo::create(0.3f, 0.05f);
    
    pLblSp->runAction(CCSequence::create(spExtension, spDelay, CCSpawn::create(spDrop, spFadeOut, NULL), NULL));
    
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, size.height/2-accp(80)), pLblSp, 5);
    
    ///////
    CCSprite* pSprTextBg = CCSprite::create("ui/quest/trace/message_sp.png");
    pSprTextBg->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTextBg->setPosition(ccp(size.width/2, size.height/2-accp(60)));
    pSprTextBg->setScale(0.2f);
    pSprTextBg->setTag(488);
    this->addChild(pSprTextBg,4);
    
    CCActionInterval* spExtension2 = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay2 = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop2 = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut2 = CCFadeTo::create(0.3f, 0.05f);
    pSprTextBg->runAction(CCSequence::create(spExtension2, spDelay2, CCSpawn::create(spDrop2, spFadeOut2, NULL), NULL));
    
    CCSprite* pSprTextBg2 = CCSprite::create("ui/quest/trace/message_bp.png");
    pSprTextBg2->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprTextBg2->setPosition(ccp(size.width/2, size.height/2-accp(100)));
    pSprTextBg2->setScale(0.2f);
    pSprTextBg2->setTag(489);
    this->addChild(pSprTextBg2,4);
    
    CCActionInterval* spExtension3 = CCScaleTo::create(0.5f, 1.0f);
    CCActionInterval* spDelay3 = CCDelayTime::create(1.2f);
    CCActionInterval* spDrop3 = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* spFadeOut3 = CCFadeTo::create(0.3f, 0.05f);
    pSprTextBg2->runAction(CCSequence::create(spExtension3, spDelay3, CCSpawn::create(spDrop3, spFadeOut3, NULL), NULL));
}

void TraceLayer::callBackStamina(CCSprite* sprite)
{
    this->removeChildByTag(485, true);
    this->removeChildByTag(486, true);
    this->removeChildByTag(487, true);
    this->removeChildByTag(488, true);
    this->removeChildByTag(489, true);
    
    traceUILayer->refreshStaminaGauge(-10);

    //setChaseStep(CHASE_TAB);
    
    RewardEventManager();
}

void TraceLayer::actionDrumCrash()
{
    CCSize size = this->getContentSize();
    CCSprite* sprite = CCSprite::create("ui/quest/trace/quest_drum/quest_drum_00_05.png");
    sprite->setAnchorPoint(ccp(0.5f, 0.5f));
    sprite->setPosition(ccp(size.width/2, size.height/2 + accp(500)));
    sprite->setScale(1.3f);
    sprite->setTag(491);
    
    CCActionInterval* movingDown = CCMoveBy::create(0.5f, accp(0.0f, -500.0f));
    CCActionInterval* jumping1 = CCJumpBy::create(0.8f, accp(0.0f, 0.0f), 20.0f, 1);
    CCActionInterval* jumping2 = CCJumpBy::create(0.4f, accp(0.0f, 0.0f), 10.0f, 1);
    CCActionInterval* jumping3 = CCJumpBy::create(0.2f, accp(0.0f, 0.0f), 5.0f, 1);
    CCCallFunc* cb = CCCallFuncN::create(this, callfuncN_selector(TraceLayer::callBackDrumCrash1));
    CCActionInterval* delay = CCDelayTime::create(0.5f);
    
    sprite->runAction(CCSequence::create(movingDown, jumping1, jumping2, jumping3, delay, cb, NULL));
    
    this->addChild(sprite, 0);
    
}

void TraceLayer::callBackDrumCrash1(CCSprite* sprite)
{
 
    sprite->setVisible(false);
    CCSize size = this->getContentSize();
        
    //CCSprite* sparkle = CCSprite::create("ui/quest/trace/quest_item/quest_mid_bg.png");
    CCSprite* sparkle = CCSprite::create("ui/quest/trace/message_bg.png");
    sparkle->setAnchorPoint(ccp(0.5f, 0.5f));
    sparkle->setPosition(ccp(size.width/2, size.height/2));
    sparkle->setScale(0.2);
    sparkle->setTag(492);
    
    CCCallFunc* cbPlayCrash = CCCallFunc::create(this, callfunc_selector(MyUtil::soundBreakDrum));
    CCActionInterval* sparkleExtension = CCScaleTo::create(0.5f, 2.0f);//0.8f);
    CCActionInterval* sparkleDelay = CCDelayTime::create(1.2f);
    CCActionInterval* sparkleDrop = CCMoveBy::create(0.3f, accp(0.0f, -100.0f));
    CCActionInterval* sparkleFadeOut = CCFadeTo::create(0.3f, 0.05f);
    CCCallFunc* cb = CCCallFunc::create(this, callfunc_selector(TraceLayer::callBackDrumCrash2));
    
    
    sparkle->runAction(CCSequence::create(cbPlayCrash, sparkleExtension, sparkleDelay, CCSpawn::create(sparkleDrop, sparkleFadeOut, cb, NULL), NULL));
    
    
    this->addChild(sparkle, 0);
    
    // broken drum
    CCSprite* breakedDrum = CCSprite::create("ui/quest/trace/quest_drum/quest_drum2_40.png");
    breakedDrum->setAnchorPoint(ccp(0.5f, 0.5f));
    breakedDrum->setPosition(ccp(size.width/2, size.height/2));
    breakedDrum->setScale(1.3f);
    breakedDrum->setTag(493);
    
    CCActionInterval* jumpingR1 = CCJumpBy::create(0.6, accp(280.0f, 0.0f), accp(70.0f), 1);
    CCActionInterval* jumpingR2 = CCJumpBy::create(0.3, accp(150.0f, 0.0f), accp(35.0f), 1);
    
    
    breakedDrum->runAction(CCSequence::create(jumpingR1, jumpingR2, NULL));
    this->addChild(breakedDrum, 0);
    
    
    RewardEventManager();
    
}

void TraceLayer::callBackDrumCrash2()
{
    removeChildByTag(491, true);
    removeChildByTag(492, true);
    removeChildByTag(493, true);
}




void TraceLayer::callBackMoneySendReward()
{
    
}

void TraceLayer::popupStaminaBuy()
{
    NoStaminaPopup* popup = new NoStaminaPopup();
    popup->InitUI();
    this->addChild(popup, 5000);
}



const char* TraceLayer::textAdjust(const char *input)
{
    std::string text = input;
    do {
        int pos = text.find("\\n");
        if (pos >= text.length())
            return text.c_str();
        text = text.replace(pos, 2, "\n");
    } while (1);
    return text.c_str();
}

void TraceLayer::startFight()
{
    CCSize size = this->getContentSize();
    
    pTraceResultLayer = new TraceResultLayer;
    pTraceResultLayer->setPosition(size.width/2.0f, size.height/2.0f);
    pTraceResultLayer->getInstance()->startFight();
    this->addChild(pTraceResultLayer, 10);
}

void TraceLayer::startKo()
{
    CCSize size = this->getContentSize();
    
    pTraceResultLayer = new TraceResultLayer;
    pTraceResultLayer->setPosition(size.width/2.0f, size.height/2.0f);
    pTraceResultLayer->getInstance()->startKo();
    this->addChild(pTraceResultLayer, 10);
    
    ////////////////////////////////////////////////////
    //
    // KO 연출 시 뒤에 남아있는 일반적과 게이지 보이지 않도록 처리
    
    TraceNormalEnemyLayer::getInstance()->getChildByTag(90)->setVisible(false);
    TraceNormalEnemyLayer::getInstance()->getChildByTag(91)->setVisible(false);
    TraceNormalEnemyLayer::getInstance()->getChildByTag(92)->setVisible(false);
    TraceNormalEnemyLayer::getInstance()->getChildByTag(93)->setVisible(false);
    TraceNormalEnemyLayer::getInstance()->getChildByTag(193)->setVisible(false);
    TraceNormalEnemyLayer::getInstance()->getChildByTag(200)->setVisible(false);
    for (int i=0; i<3; i++)
    {
        TraceNormalEnemyLayer::getInstance()->removeChild(TraceNormalEnemyLayer::getInstance()->hitGauge[i], true);
    }
}


void TraceLayer::actionEnemy(int _enemyType)
{
    
    ////////////////////////////////////
    //
    // ??????§¬????¬£¬∫???????????§Œ?????¬©?????¬•??????    
    CCSize size = this->getContentSize();
    
    // ???? ????¬©????
    CCSpriteFrame* entryFrame0 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_bk_01.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* entryFrame1 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_bk_02.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* entryFrame2 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_bk_03.png", CCRectMake(0,0,accp(640),accp(280)));
    
    frameFindingEnemyEntry = new CCArray();
    frameFindingEnemyEntry->addObject(entryFrame0);
    frameFindingEnemyEntry->addObject(entryFrame1);
    frameFindingEnemyEntry->addObject(entryFrame2);
    
    CCAnimation* entryAnimation = CCAnimation::create();
    for (int i=0; i<frameFindingEnemyEntry->count(); i++)
    {
        entryAnimation->addSpriteFrame((CCSpriteFrame* )frameFindingEnemyEntry->objectAtIndex(i));
    }
    entryAnimation->setLoops(1);
    entryAnimation->setDelayPerUnit(0.05f);
    
    CCAnimate* entryAnimate = CCAnimate::create(entryAnimation);
    
    // ??????§Œ?????¬©????
    CCSpriteFrame* mainFrame0 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_01.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* mainFrame1 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_02.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* mainFrame2 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_03.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* mainFrame3 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_04.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* mainFrame4 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_05.png", CCRectMake(0,0,accp(640),accp(280)));
    
    frameFindingEnemyMain = new CCArray();
    frameFindingEnemyMain->addObject(mainFrame0);
    frameFindingEnemyMain->addObject(mainFrame1);
    frameFindingEnemyMain->addObject(mainFrame2);
    frameFindingEnemyMain->addObject(mainFrame3);
    frameFindingEnemyMain->addObject(mainFrame4);
    
    CCAnimation* mainAnimation = CCAnimation::create();
    for (int i=0; i<frameFindingEnemyMain->count(); i++)
    {
        mainAnimation->addSpriteFrame((CCSpriteFrame* )frameFindingEnemyMain->objectAtIndex(i));
    }
    mainAnimation->setLoops(1);
    mainAnimation->setDelayPerUnit(0.05);
    
    CCAnimate* mainAnimate = CCAnimate::create(mainAnimation);
    
    // ?¬•?? ????¬©????
    CCSpriteFrame* exitFrame0 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_01.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame1 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_02.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame2 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_03.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame3 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_04.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame4 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_05.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame5 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_06.png", CCRectMake(0,0,accp(640),accp(280)));
    CCSpriteFrame* exitFrame6 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_bg_end_07.png", CCRectMake(0,0,accp(640),accp(280)));
    
    frameFindingEnemyExit = new CCArray();
    frameFindingEnemyExit->addObject(exitFrame0);
    frameFindingEnemyExit->addObject(exitFrame1);
    frameFindingEnemyExit->addObject(exitFrame2);
    frameFindingEnemyExit->addObject(exitFrame3);
    frameFindingEnemyExit->addObject(exitFrame4);
    frameFindingEnemyExit->addObject(exitFrame5);
    frameFindingEnemyExit->addObject(exitFrame6);
    
    CCAnimation* exitAnimation = CCAnimation::create();
    for (int i=0; i<frameFindingEnemyExit->count(); i++)
    {
        exitAnimation->addSpriteFrame((CCSpriteFrame* )frameFindingEnemyExit->objectAtIndex(i));
    }
    exitAnimation->setLoops(1);
    exitAnimation->setDelayPerUnit(0.05f);
    
    CCAnimate* exitAnimate = CCAnimate::create(exitAnimation);
    
    //////////////////////////////////
    //
    // ??????§¬????¬£¬∫???????????§Œ?????¬©?????¬®??
    
    CCSpriteFrame* entryFrame = (CCSpriteFrame* )frameFindingEnemyEntry->objectAtIndex(0);
    spriteFindingEnemyEntry = CCSprite::createWithSpriteFrame(entryFrame);
    spriteFindingEnemyEntry->setAnchorPoint(ccp(0.5f, 0.0f));
    spriteFindingEnemyEntry->setPosition(ccp(size.width/2.0f, 0.0f+accp(85)));
    spriteFindingEnemyEntry->setTag(31);
    //CCActionInterval* moveToOut1 = CCMoveBy::create(0.1f, accp(-500.0f, 0.0f));
    CCActionInterval* moveToOut1 = CCMoveBy::create(0.1f, ccp(-(GameConst::WIN_SIZE.width/2), 0.0f));
    
    ////////////////////////////////////
    //
    // ???Œ©???¬∞???????????????Œ©?????? ???¬∂¬®
    
    if (_enemyType == 1)           // NORMAL
    {
        CCCallFunc* cbNormalEnemy = CCCallFunc::create(this, callfunc_selector(TraceLayer::callBackNormalEnemy));
        
        spriteFindingEnemyEntry->runAction(CCSequence::create(entryAnimate, CCRepeat::create(mainAnimate, 8), exitAnimate, moveToOut1, cbNormalEnemy, NULL));
    }
    else if (_enemyType == 2)      // RIVAL
    {
        CCCallFunc* cbRival1 = CCCallFunc::create(this, callfunc_selector(TraceLayer::goRival));
        
        spriteFindingEnemyEntry->runAction(CCSequence::create(entryAnimate, CCRepeat::create(mainAnimate, 8), exitAnimate, moveToOut1, cbRival1, NULL));
    }
    else if (_enemyType == 3)      // HIDDEN_RIVAL
    {
        
        //CCCallFunc* cbHiddenRival = CCCallFunc::create(this, callfunc_selector(TraceLayer::goHiddenRival));
        //CCCallFunc* cbRival1 = CCCallFunc::create(this, callfunc_selector(TraceLayer::goRival));
        //spriteFindingEnemyEntry->runAction(CCSequence::create(entryAnimate, CCRepeat::create(mainAnimate, 8), exitAnimate, moveToOut1, cbHiddenRival, NULL));
        
        CCCallFunc* cbRival1 = CCCallFunc::create(this, callfunc_selector(TraceLayer::goHiddenRival));
        spriteFindingEnemyEntry->runAction(CCSequence::create(entryAnimate, CCRepeat::create(mainAnimate, 8), exitAnimate, moveToOut1, cbRival1, NULL));
    }
    else if (_enemyType == 4)      // BOSS
    {
        CCCallFunc* cbBoss = CCCallFunc::create(this, callfunc_selector(TraceLayer::goBoss));
        
        spriteFindingEnemyEntry->runAction(CCSequence::create(entryAnimate, CCRepeat::create(mainAnimate, 8), exitAnimate, moveToOut1, cbBoss, NULL));
    }
    
    this->addChild(spriteFindingEnemyEntry, -5);
    
    ///////////////////////////////
    //
    // ??????§¬????????????¬©?????¬•??????    
    CCSpriteFrame* alertFrame0 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_11.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame1 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_12.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame2 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_13.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame3 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_14.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame4 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_15.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame5 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_16.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame6 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_17.png", CCRectMake(0, 0, accp(428), accp(230)));
    CCSpriteFrame* alertFrame7 = CCSpriteFrame::create("ui/quest/trace/quest_find/quest_enemy_shoc_18.png", CCRectMake(0, 0, accp(428), accp(230)));
    
    frameToAlert = new CCArray();
    frameToAlert->addObject(alertFrame0);
    frameToAlert->addObject(alertFrame1);
    frameToAlert->addObject(alertFrame2);
    frameToAlert->addObject(alertFrame3);
    frameToAlert->addObject(alertFrame4);
    frameToAlert->addObject(alertFrame5);
    frameToAlert->addObject(alertFrame6);
    frameToAlert->addObject(alertFrame7);
    
    CCSpriteFrame* alertFrame = (CCSpriteFrame* )frameToAlert->objectAtIndex(0);
    spriteToAlert = CCSprite::createWithSpriteFrame(alertFrame);
    spriteToAlert->setAnchorPoint(ccp(1.0f, 0.5f));
    spriteToAlert->setPosition(ccp(size.width-accp(50), 0.0f+accp(300)));
    spriteToAlert->setTag(32);
    
    CCAnimation* animationToAlert = CCAnimation::create();
    for (int i=0; i<frameToAlert->count(); i++)
    {
        animationToAlert->addSpriteFrame((CCSpriteFrame* )frameToAlert->objectAtIndex(i));
    }
    animationToAlert->setLoops(1);
    animationToAlert->setDelayPerUnit(0.05f);
    
    CCAnimate* animateToAlert = CCAnimate::create(animationToAlert);
    CCActionInterval* delay = CCDelayTime::create(2.0f);
    CCActionInterval* fadeOut = CCFadeTo::create(0.0f, 0);
    
    spriteToAlert->runAction(CCSequence::create(animateToAlert, delay, fadeOut, NULL));
    
    this->addChild(spriteToAlert, -3);
    
    ////////////////
    //
    // ?¬£¬∫??????¬±?? ?¬∞??
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    // 리더 카드에 맞는 배경 이미지 경로 생성
    CardListInfo* leader = FileManager::sharedFileManager()->GetCardInfo(PlayerInfo::getInstance()->GetCardInDeck(0, 0, 2)->cardId);
//    string pathChar = "ui/cha/";
    string pathChar = CCFileUtils::sharedFileUtils()->getDocumentPath();
    pathChar = pathChar + leader->GetSmallBattleImg();
    
    CCSprite* hero = CCSprite::create(pathChar.c_str());//"ui/cha/s_ff_guy.png");
    hero->setAnchorPoint(ccp(0.0f, 0.0f));
    hero->setPosition(ccp(size.width, 0.0f+accp(98)));
    hero->setTag(33);
    
    CCActionInterval* moveFastToLeft = CCMoveTo::create(0.1f, accp(0.0f, 98.0f));
    CCActionInterval* moveSlowToLeft = CCMoveBy::create(2.3f, accp(-20.0f, 0.0f));
    CCCallFunc* cbPlayRingSound = CCCallFunc::create(this, callfunc_selector(MyUtil::soundRing));
    CCActionInterval* moveToOut2 = CCMoveBy::create(0.1f, accp(-500.0f, 0.0f));
    
    hero->runAction(CCSequence::create(moveFastToLeft, cbPlayRingSound, moveSlowToLeft, moveToOut2, NULL));
    
    this->addChild(hero, -5);
}



TraceUILayer::TraceUILayer(){
    
    int ccp_y = -10;
    
    CCSprite* pSprTraceStat = CCSprite::create("ui/quest/trace/trace_level_bg.png");
    pSprTraceStat->setAnchorPoint(ccp(0,0));
    pSprTraceStat->setPosition( accp(0,ccp_y));
    this->addChild(pSprTraceStat, 0);
    
    
    CCSprite* pSprXpGageSlash = CCSprite::create("ui/quest/trace/trace_level_gage_slash.png");
    pSprXpGageSlash->setAnchorPoint(ccp(0.0f, 0.5f));
    pSprXpGageSlash->setPosition(accp(XP_GAUGE_LENGTH / 2.0f, ccp_y + XP_SPLASH_Y_POS-1.5f));
    this->addChild(pSprXpGageSlash, 11);
    
    refreshXp(ccp_y-3);
    refreshMaxXp(ccp_y-3);
    loadXpGauge();
    refreshXpGauge(ccp_y);
    
    refreshStamina(ccp_y);
    refreshMaxStamina(ccp_y);
    loadStaminaGauge();
    refreshStaminaGauge(ccp_y);
    
    refreshBattlePoints(ccp_y);
    refreshMaxBattlePoints(ccp_y);
    loadBattlePointsGauge();
    refreshBattlePointsGauge(ccp_y);
    
    refreshLevel(ccp_y);
}

TraceUILayer::~TraceUILayer(){
}

void TraceUILayer::Refresh()
{
    int ccp_y = -10;
    refreshXp(ccp_y-3);
    refreshMaxXp(ccp_y-3);
    refreshXpGauge(ccp_y);
    
    refreshStamina(ccp_y);
    refreshMaxStamina(ccp_y);
    refreshStaminaGauge(ccp_y);
    
    refreshBattlePoints(ccp_y);
    refreshMaxBattlePoints(ccp_y);
    refreshBattlePointsGauge(ccp_y);
    refreshLevel(ccp_y);
    
    
    refreshStageProgressRatio(SCREEN_ZOOM_RATE*(GameConst::WIN_SIZE.width-accp(36)), SCREEN_ZOOM_RATE*(GameConst::WIN_SIZE.height-accp(76)));//38));
    
    
}



void TraceUILayer::refreshXp(int yy)
{
    removeXp();
    int x = XP_GAUGE_LENGTH / 2.0f - 10.0f;
    float scale = 0.7f;
    
    char buffer[10];
    sprintf(buffer, "%d", PlayerInfo::getInstance()->xp);
    int value = atoi(buffer);
    int length = strlen(buffer);
//    CCLog(" refreshXp : %d", PlayerInfo::getInstance()->xp);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan+1))) / (int)(pow(10, scan));
//        CCLog("scan:%d num :%d",scan, number);
        xp[scan] = createNumber(number, accp(x, yy + XP_SPLASH_Y_POS - 5.0f), scale);
        this->addChild(xp[scan], 11);
        CCSize size = xp[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 1;
        xp[scan]->setPosition(accp(x, yy + XP_SPLASH_Y_POS - 5.0f));
    }
}

void TraceUILayer::refreshMaxXp(int yy)
{
    removeMaxXp();
    bool skip = true;
    int x = XP_GAUGE_LENGTH / 2.0f + 21.0f;
    float scale = 0.7f;
    
    char buffer[10];
    sprintf(buffer, "%d", PlayerInfo::getInstance()->getMaxXp());
//    CCLog(" refreshMaxXp : %d", PlayerInfo::getInstance()->getMaxXp());
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan=length-1; scan>-1; scan--)
    {
        int number = value % ((int)powf(10, scan+1)) / (int)(pow(10, scan));
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxXp[scan] = createNumber(number, accp(x, yy + XP_SPLASH_Y_POS - 5.0f), scale);
        this->addChild(maxXp[scan], 11);
        CCSize size = maxXp[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceUILayer::removeXp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (xp[scan] != NULL)
            this->removeChild(xp[scan], true);
        xp[scan] = NULL;
    }
}

void TraceUILayer::removeMaxXp()
{
    for (int scan=0; scan<10; scan++)
    {
        if (maxXp[scan] != NULL)
            this->removeChild(maxXp[scan], true);
        maxXp[scan] = NULL;
    }
}

void TraceUILayer::loadXpGauge()
{
    //char buffer[64];
    for (int scan=0; scan<2; scan++)
    {
        
        //sprintf(buffer, "ui/quest/trace/trace_gage_red%d.png", scan+2);
        //xpGauge[scan] = CCSprite::create(buffer);
        
        string path = "ui/quest/trace/trace_gage_red";
        
        char buf[1];
        sprintf(buf, "%d", scan+2);
        path.append(buf).append(".png");
        xpGauge[scan] = CCSprite::create(path.c_str());
        
        xpGauge[scan]->setAnchorPoint(ccp(0.0f, 0.0f));
        this->addChild(xpGauge[scan], 10);
        //}
    }
}


void TraceUILayer::refreshXpGauge(int yy)
{
    PlayerInfo* playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->xp / (float)playerInfo->getMaxXp();
    if (ratio > 1)ratio = 1;
    
    for (int scan=0; scan<2; scan++)
        xpGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = 0;
    xpGauge[0]->setPosition(accp(x, yy + XP_GAUGE_Y_POS));
    xpGauge[0]->setScaleX(ratio * XP_GAUGE_LENGTH);
    x += ratio * XP_GAUGE_LENGTH;
    xpGauge[1]->setPosition(accp(x, yy + XP_GAUGE_Y_POS));
}





void TraceUILayer::refreshStamina(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeStamina();
    int x = STAMINA_GAUGE_X_POS + 234;
    float scale = 1.3f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->getStamina());     // stamina ?¬¨¬®??????¬¨¬®??? ??
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan=0; scan<length; scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        stamina[scan] = createNumber(number, accp(x, yy + STAMINA_GAUGE_Y_POS), scale);
        this->addChild(stamina[scan], 10);
        CCSize size = stamina[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        stamina[scan]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS + 15.0f));
    }
}

void TraceUILayer::refreshMaxStamina(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeMaxStamina();
    bool skip = true;
    int x = STAMINA_GAUGE_X_POS + 189;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        
        int number = (playerInfo->getMaxStamina() % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));   // maxStamina ?¬¨¬®??????¬¨¬®??? ??
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxStamina[scan] = createNumber(number, accp(x, yy + STAMINA_GAUGE_Y_POS - 2.0f), scale);
        this->addChild(maxStamina[scan], 10);
        CCSize size = maxStamina[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceUILayer::removeStamina()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (stamina[scan] != NULL)
            this->removeChild(stamina[scan], true);
        stamina[scan] = NULL;
    }
}

void TraceUILayer::refreshStageProgressRatio(int xx, int yy)
{
    if (TraceLayer::getInstance()->questProgress >=0){
        int spVal = TraceLayer::getInstance()->questProgress;
        
        removeStageProgressRatio(spVal);
        int x = xx;
        float scale = 1.3f;
        char buffer[5];
        sprintf(buffer, "%d", spVal);
        int value = atoi(buffer);
        int length = strlen(buffer);
        for (int scan=0; scan<length; scan++)
        {
            int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
            sprStageProgressRatio[scan] = createNumber(number, accp(x, yy), scale);
            this->addChild(sprStageProgressRatio[scan], 10);
            CCSize size = sprStageProgressRatio[scan]->getTexture()->getContentSizeInPixels();
            x -= size.width * scale - 2;
            sprStageProgressRatio[scan]->setPosition(accp(x, yy + 15.0f));
        }
    }
}


void TraceUILayer::removeStageProgressRatio(int progress)
{
    char buffer[5];
    sprintf(buffer, "%d", progress);
    
    for (int scan=0; scan<strlen(buffer); scan++)
    {
        if (sprStageProgressRatio[scan] != NULL)
            this->removeChild(sprStageProgressRatio[scan], true);
        sprStageProgressRatio[scan] = NULL;
    }
    
}


void TraceUILayer::removeMaxStamina()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxStamina[scan] != NULL)
            this->removeChild(maxStamina[scan], true);
        maxStamina[scan] = NULL;
    }
}

void TraceUILayer::loadStaminaGauge()
{
    char buffer[64];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_stamina0%d.png", scan + 1);
        staminaGauge[scan] = CCSprite::create(buffer);
        staminaGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(staminaGauge[scan], 10);
    }
}

void TraceUILayer::refreshStaminaGauge(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    float ratio = (float)playerInfo->getStamina() / (float)playerInfo->getMaxStamina();   // stamina ?¬¨¬®??? ?????¬¨¬®??????¬¨¬®??? ??
    for (int scan = 0;scan < 3;scan++)
        staminaGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = STAMINA_GAUGE_X_POS;
    staminaGauge[0]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
    x += 5;
    staminaGauge[1]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
    staminaGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    staminaGauge[2]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
}

////////////////////
//
// ?????? ?¬¨¬®¬¨??????¬¨¬®??? ?¬¨¬®???

void TraceUILayer::refreshBattlePoints(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeBattlePoints();
    int x = BATTLE_GAUGE_X_POS + 231;
    float scale = 1.3f;
    char buffer[3];
    sprintf(buffer, "%d", playerInfo->getBattlePoint());// defenceBP);   // battlePoints ?¬¨¬®??????¬¨¬®??? ??
    int value = atoi(buffer);
    int length = strlen(buffer);
    for (int scan = 0;scan < length;scan++)
    {
        int number = (value % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));
        battlePoints[scan] = createNumber(number, accp(x, yy + STAMINA_GAUGE_Y_POS), scale);
        this->addChild(battlePoints[scan], 10);
        CCSize size = battlePoints[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        battlePoints[scan]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS + 15.0f));
    }
}

void TraceUILayer::refreshMaxBattlePoints(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeMaxBattlePoints();
    bool skip = true;
    int x = BATTLE_GAUGE_X_POS + 186;
    float scale = 0.7f;
    for (int scan = 2;scan >= 0;scan--)
    {
        //int number = (playerInfo->maxDefensePoints % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));     // maxBattlePoints ?¬¨¬®??????¬¨¬®??? ??
        int number = (playerInfo->getMaxBattlePoint() % ((int)powf(10, scan + 1))) / (int)(pow(10, scan));     // maxBattlePoints ?¬¨¬®??????¬¨¬®??? ??
        if (number > 0)
            skip = false;
        if (skip)
            continue;
        maxBattlePoints[scan] = createNumber(number, accp(x, yy + STAMINA_GAUGE_Y_POS - 2.0f), scale);
        this->addChild(maxBattlePoints[scan], 10);
        CCSize size = maxBattlePoints[scan]->getTexture()->getContentSizeInPixels();
        x += size.width * scale - 1;
    }
}

void TraceUILayer::removeBattlePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (battlePoints[scan] != NULL)
            this->removeChild(battlePoints[scan], true);
        battlePoints[scan] = NULL;
    }
}

void TraceUILayer::removeMaxBattlePoints()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (maxBattlePoints[scan] != NULL)
            this->removeChild(maxBattlePoints[scan], true);
        maxBattlePoints[scan] = NULL;
    }
}

void TraceUILayer::loadBattlePointsGauge()
{
    char buffer[64];
    for (int scan = 0;scan < 3;scan++)
    {
        sprintf(buffer, "ui/home/gage_bp0%d.png", scan + 1);
        battlePointsGauge[scan] = CCSprite::create(buffer);
        battlePointsGauge[scan]->setAnchorPoint(ccp(0, 0));
        this->addChild(battlePointsGauge[scan], 10);
    }
}

void TraceUILayer::refreshBattlePointsGauge(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    //float ratio = (float)playerInfo->defenceBP / (float)playerInfo->maxDefensePoints;   // battlePoints ?¬¨¬®??? ?????¬¨¬®??????¬¨¬®??? ??
    float ratio = (float)playerInfo->getBattlePoint() / (float)playerInfo->getMaxBattlePoint();   // battlePoints ?¬¨¬®??? ?????¬¨¬®??????¬¨¬®??? ??
    for (int scan = 0;scan < 3;scan++)
        battlePointsGauge[scan]->setVisible((ratio <= 0.0f) ? false : true);
    int x = BATTLE_GAUGE_X_POS;
    battlePointsGauge[0]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
    x += 5;
    battlePointsGauge[1]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
    battlePointsGauge[1]->setScaleX(ratio * 127);
    x += (ratio * 127);
    battlePointsGauge[2]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS));
}

void TraceUILayer::refreshLevel(int yy)
{
    PlayerInfo *playerInfo = PlayerInfo::getInstance();
    removeLevel();
    int x = BATTLE_GAUGE_X_POS + 343;
    float scale = 2.0f;
    char buffer[10];
    sprintf(buffer, "%d", playerInfo->myLevel);
    int length = strlen(buffer);
    for (int scan = length - 1;scan >= 0;scan--)
    {
        int number = buffer[scan] - '0';
        level[scan] = createNumber(number, accp(x, yy + STAMINA_GAUGE_Y_POS + 30.0f), scale);
        this->addChild(level[scan], 2000);
        CCSize size = level[scan]->getTexture()->getContentSizeInPixels();
        x -= size.width * scale - 2;
        level[scan]->setPosition(accp(x, yy + STAMINA_GAUGE_Y_POS + 32.0f));
    }
}

void TraceUILayer::removeLevel()
{
    for (int scan = 0;scan < 3;scan++)
    {
        if (level[scan] != NULL)
            this->removeChild(level[scan], true);
        level[scan] = NULL;
    }
}



///////////////////////////////////////////////////////////////////


StageClearPopup::StageClearPopup()
{
    InitUI();
}

StageClearPopup::~StageClearPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void StageClearPopup::InitUI()
{
    
    CCSize size = GameConst::WIN_SIZE;
    
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.5f, 0.5f));
    
    int yy = size.height/2 - accp(186);//accp(500);
    popupBG->setPosition(ccp(size.width/2, size.height/2));// accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
//    const char* text2 = "다음 스테이지에 입장할 수 있습니다.";
    CCLabelTTF* Label2 = CCLabelTTF::create(LocalizationManager::getInstance()->get("quest_nextStage_msg"), "Thonburi", 11);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, yy + accp(186) - accp(60)), Label2, 160);
    
    CCSprite* pSprc = CCSprite::create("ui/quest/trace/trace_popup_complete.png");
    pSprc->setAnchorPoint(ccp(0.5,0.5));
    //pSprc->setPosition(ccp(accp(89 + 194), yy - accp(130)));
    pSprc->setPosition(ccp( size.width/2 + accp(10.0f), yy + accp(186) + accp(40)));
    this->addChild(pSprc);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(101);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(ccp(accp(93.0f), yy + accp(10)));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(ccp(accp(342.0f), yy + accp(10)));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(194.0f), yy + accp(15)), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), ccp(accp(443.0f), yy + accp(15)), RightLabel, 160);

}


void StageClearPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI ?¬¢?????GL?¬¢???¬∞?????§Œ?¬©????
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        // ok
        
        ARequestSender::getInstance()->requestStageList();
        
        /*
        <?xml version="1.0" encoding="utf-8"?><response><res>0</res><message></message><quests><quest id="20011" begin="1360998220" end="1361002914" progress="100" clear="1" max_progress="999" enemy="0"></quest></quests></response>
        */
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->exitLayer();
        
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG->stopBackgroundMusic();
        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
        soundMainBG();
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
//        TraceLayer::getInstance()->exitLayer();
        TraceLayer::getInstance()->setChaseStep(CHASE_TAB);
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
//        if (PlayerInfo::getInstance()->getBgmOption()){
//            soundBG->stopBackgroundMusic();
//        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
//        soundMainBG();
    }
}


///////////////////////////////////////////////////////////////////


void TraceLayer::setRivalBattleResult(ResponseRivalBattle *result)
{
    //result
    
    // 결과 먼저 받은 후 결과 연출
    this->responseRivalBattleInfo = result;
    if (rivalLayer != NULL)this->removeChild(rivalLayer, true);
    if (hiddenRivalLayer != NULL)this->removeChild(hiddenRivalLayer, true);
    if (bossLayer != NULL)this->removeChild(bossLayer, true);
    
    prevUserLevelBeforeRivalBattle = PlayerInfo::getInstance()->getLevel();
    
    /*
    // 배틀 이후에 stat이 갱신되도록 callbackFromBattle로 이동. 2013-03-08 김용호
    setUserStat(result);
    
    if (result->reward_cards->count()>0){
        rewardCards = new CCArray();
        for (int i=0;i<result->reward_cards->count();i++){
            QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)result->reward_cards->objectAtIndex(i);
            
            CardInfo *cardInfo = new CardInfo();
            cardInfo->autorelease();
            cardInfo->setId(rewardCard->card_id);
            cardInfo->setSrl(rewardCard->card_srl);
            cardInfo->setExp(rewardCard->card_exp);
            cardInfo->setLevel(rewardCard->card_level);
            cardInfo->setAttack(rewardCard->card_attack);
            cardInfo->setDefence(rewardCard->card_defense);
            cardInfo->setSkillEffect(rewardCard->card_skillEffect);
            
            
            CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
            PlayerInfo::getInstance()->addToMyCardList(newCard);
            rewardCards->addObject(newCard);
        }
    }
    */
    
    goBattleLayerFromRival(result,currentNpc,PlayerInfo::getInstance()->traceTeam);
    
}

void TraceLayer::setBossBattleResult(ResponseQuestUpdateResultInfo *result)
{
    // 결과 먼저 받은 후 결과 연출
    this->receivedTraceResultInfo = result;
    if (rivalLayer != NULL)this->removeChild(rivalLayer, true);
    if (hiddenRivalLayer != NULL)this->removeChild(hiddenRivalLayer, true);
    if (bossLayer != NULL)this->removeChild(bossLayer, true);
    
    prevUserLevelBeforeRivalBattle = PlayerInfo::getInstance()->getLevel();
    
    /*
     // 배틀 이후에 stat이 갱신되도록 callbackFromBattle로 이동. 2013-03-08 김용호
    setUserStat(result);
    
    if (result->cards->count()>0){
        rewardCards = new CCArray();
        for (int i=0;i<result->cards->count();i++){
            QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)result->cards->objectAtIndex(i);
            
            CardInfo *cardInfo = new CardInfo();
            
            cardInfo->autorelease();
            cardInfo->setId(rewardCard->card_id);
            cardInfo->setSrl(rewardCard->card_srl);
            cardInfo->setExp(rewardCard->card_exp);
            cardInfo->setLevel(rewardCard->card_level);
            cardInfo->setAttack(rewardCard->card_attack);
            cardInfo->setDefence(rewardCard->card_defense);
            cardInfo->setSkillEffect(rewardCard->card_skillEffect);
            
            CCLog("reward card id:%d srl:%d lv:%d",rewardCard->card_id, rewardCard->card_srl, rewardCard->card_level);
            
            CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
            
            PlayerInfo::getInstance()->addToMyCardList(newCard);
            
            rewardCards->addObject(newCard);
        }
    }
    */
    goBattleLayerFromQuest(result, currentNpc, PlayerInfo::getInstance()->traceTeam);
    
}

void TraceLayer::goBattleLayerFromRival(ResponseRivalBattle* _rivalBattleResult, NpcInfo* _npcInfo, int _nTeam)
{
    // ResponseRivalBattle를 ResponseBattleInfo로 convert
    
    PlayerInfo::getInstance()->battleResponseInfo = new ResponseBattleInfo();
    
    battleInfoForBattleScene = PlayerInfo::getInstance()->battleResponseInfo;
    
    battleInfoForBattleScene->battleResult        = _rivalBattleResult->battleResult; // 0 == lose, 1 == win, 2 == na
    
    battleInfoForBattleScene->reward_coin         = _rivalBattleResult->reward_coin;
    battleInfoForBattleScene->used_battle_point   = _rivalBattleResult->used_battle_point;
    battleInfoForBattleScene->opponent_card[2]    = _npcInfo->cardCode;
    battleInfoForBattleScene->enemy_nick          = _npcInfo->npcName;
    /*
    // 이 부분들은 배틀에서는 필요없을듯?
    battleInfoForBattleScene->enemy_appFriends    = _rivalBattleResult->defend_friend;
    battleInfoForBattleScene->enemy_defense_pnt   = _rivalBattleResult->rival_hp;// 0;//rivalBattleResult->defend_point;
    battleInfoForBattleScene->enemy_battle_pnt    = 0; // attack point
    */
    
    battleInfoForBattleScene->attackerSkillPoint  = _rivalBattleResult->user_ext_tot;
    battleInfoForBattleScene->defenderSkillPoint  = _rivalBattleResult->rival_ext_tot;
    battleInfoForBattleScene->enemy_level         = rivalLevel;//_rivalBattleResult->rival_level;
    
//    int rivalHp             = _rivalBattleResult->rival_origin_hp;
//    int rivalHpMax          = _rivalBattleResult->rival_max_hp;
//    int rivalAttackPoint    = _rivalBattleResult->rival_attack_tot;
//    int myHp                = _rivalBattleResult->user_max_hp;
//    int myHpMax             = _rivalBattleResult->user_max_hp;
//    int userAttackPoint     = _rivalBattleResult->user_attack_tot;
    
    
    CCSize size = GameConst::WIN_SIZE;
    battleLayer = new BattleFullScreen(size, PlayerInfo::getInstance()->traceTeam, 1, _rivalBattleResult, questID);
    battleLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    battleLayer->setPosition(accp(0.0f, 0.0f));
    battleLayer->SetBattleResult(battleInfoForBattleScene);
    battleLayer->InitUI();
    battleLayer->InitGame();
    battleLayer->setTag(388);
    this->addChild(battleLayer, 100);
    
}

// 배틀의 결과를 셋팅하고 배틀레이어를 call 한다.
void TraceLayer::goBattleLayerFromQuest(ResponseQuestUpdateResultInfo* _rivalBattleResult, NpcInfo* _npcInfo, int _nTeam)
{
    PlayerInfo::getInstance()->battleResponseInfo = new ResponseBattleInfo();
    
    battleInfoForBattleScene = PlayerInfo::getInstance()->battleResponseInfo;
    
    battleInfoForBattleScene->battleResult = _rivalBattleResult->questBattleResult; // 0 == lose, 1 == win, 2 == na
    battleInfoForBattleScene->reward_coin = _rivalBattleResult->coin;
    
    battleInfoForBattleScene->opponent_card[2] = _npcInfo->cardCode;
    battleInfoForBattleScene->enemy_nick          = _npcInfo->npcName;
    battleInfoForBattleScene->attackerSkillPoint  = 0;//_rivalBattleResult->attack_ext;
    battleInfoForBattleScene->defenderSkillPoint  = 0;//_rivalBattleResult->defend_ext;
    battleInfoForBattleScene->enemy_level         = rivalLevel; //_rivalBattleResult->enemy_level;
        
//    int rivalHp             = _rivalBattleResult->rival_hp_before_battle;
//    int rivalHpMax          = _rivalBattleResult->rival_max_hp;
//    int rivalAttackPoint    = _rivalBattleResult->rival_attack_tot;
//    int myHp                = _rivalBattleResult->user_max_hp;
//    int myHpMax             = _rivalBattleResult->user_max_hp;
//    int myAttackPoint     = _rivalBattleResult->user_attack_tot;
    
    
    CCSize size = GameConst::WIN_SIZE;
    battleLayer = new BattleFullScreen(size, PlayerInfo::getInstance()->traceTeam, 2, _rivalBattleResult, questID);
    battleLayer->setAnchorPoint(ccp(0.0f, 0.0f));
    battleLayer->setPosition(accp(0.0f, 0.0f));
    battleLayer->SetBattleResult(battleInfoForBattleScene);
    battleLayer->InitUI();
    battleLayer->InitGame();
    battleLayer->setTag(388);
    this->addChild(battleLayer, 100);
    
}


// 배틀 연출 후 TraceLayer로 돌아옴
void TraceLayer::callBackFromBattle(int _nFrom)
{
    ///////////////////
    //
    // 배틀 배경 음악 정지
    
    if (PlayerInfo::getInstance()->getBgmOption()){
        soundBG->stopBackgroundMusic();
    }
    
    //BattleFullScreen::
    
    //rivalBattleInfoFromTrace
    
    if (_nFrom == 1){ // from rival battle
        
        ///////////////////////
        //
        // 라이벌 배경 음악 다시 재생
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
            //        soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
            soundBG->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
        }
        
        this->removeChild(battleLayer, true);
        
        // 배틀 이후에 stat이 갱신되도록 callbackFromBattle로 이동. 2013-03-08 김용호
        ////////////////////////////////////////////// update user stat and add card
        setUserStat(responseRivalBattleInfo);
        
        if (responseRivalBattleInfo->reward_cards->count()>0){
            rewardCards = new CCArray();
            for (int i=0;i<responseRivalBattleInfo->reward_cards->count();i++){
                QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)responseRivalBattleInfo->reward_cards->objectAtIndex(i);
                
                CardInfo *cardInfo = new CardInfo();
                cardInfo->autorelease();
                cardInfo->setId(rewardCard->card_id);
                cardInfo->setSrl(rewardCard->card_srl);
                cardInfo->setExp(rewardCard->card_exp);
                cardInfo->setLevel(rewardCard->card_level);
                cardInfo->setAttack(rewardCard->card_attack);
                cardInfo->setDefence(rewardCard->card_defense);
                cardInfo->setSkillEffect(rewardCard->card_skillEffect);
                
                CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
                PlayerInfo::getInstance()->addToMyCardList(newCard);
                rewardCards->addObject(newCard);
            }
        }
        //////////////////////////////////////////////////
    }
    else if (_nFrom == 2){ // from boss battle
        
        ///////////////////////
        //
        // 보스 배경 음악 다시 재생
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
            //        soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
            soundBG->playBackgroundMusic("audio/bgm_boss_01.mp3", true);
        }
        
        this->removeChild(bossLayer, true);
        
        
        ////////////////////////////////////////////// update user stat and add card
        setUserStat(receivedTraceResultInfo);
        
        if (receivedTraceResultInfo->cards->count()>0){
            rewardCards = new CCArray();
            for (int i=0;i<receivedTraceResultInfo->cards->count();i++){
                QuestRewardCardInfo *rewardCard = (QuestRewardCardInfo*)receivedTraceResultInfo->cards->objectAtIndex(i);
                
                CardInfo *cardInfo = new CardInfo();
                
                cardInfo->autorelease();
                cardInfo->setId(rewardCard->card_id);
                cardInfo->setSrl(rewardCard->card_srl);
                cardInfo->setExp(rewardCard->card_exp);
                cardInfo->setLevel(rewardCard->card_level);
                cardInfo->setAttack(rewardCard->card_attack);
                cardInfo->setDefence(rewardCard->card_defense);
                cardInfo->setSkillEffect(rewardCard->card_skillEffect);
                
                //CCLog("reward card id:%d srl:%d lv:%d",rewardCard->card_id, rewardCard->card_srl, rewardCard->card_level);
                
                CardInfo* newCard = PlayerInfo::getInstance()->makeCard(rewardCard->card_id, cardInfo);
                
                PlayerInfo::getInstance()->addToMyCardList(newCard);
                
                rewardCards->addObject(newCard);
            }
        }
        ///////////////////////////////////////////
        
        responseRivalBattleInfo = new ResponseRivalBattle();
        responseRivalBattleInfo->battleResult = receivedTraceResultInfo->questBattleResult;
    }
    
    ////////////////////////////////////////////////////////////////////
    // reward event
    ////////////////////////////////////////////////////////////////////
    
    // new
    rewardEventQueue = new CCArray();
    
    EventObject *dummyEvent = new EventObject();
    rewardEventQueue->addObject(dummyEvent); // dummy event;
    
    EventObject *event2 = new EventObject();
    event2->eventCode = TRACE_EVENT_SHOW_RIVAL_BATTLE_RESULT;
    rewardEventQueue->addObject(event2);
    
    //////////////////////////////////// for test
    //prevUserLevelBeforeRivalBattle = PlayerInfo::getInstance()->getLevel() -1;
    //////////////////////////////////// for test
    
    if (_nFrom == 1){ // from rival battle
        
        if (responseRivalBattleInfo->reward_exp > 0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_EXP_UP;
            eventObj->exp = responseRivalBattleInfo->reward_exp;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (responseRivalBattleInfo->reward_coin > 0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_COIN;
            eventObj->coin = responseRivalBattleInfo->reward_coin;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (responseRivalBattleInfo->reward_cards->count()>0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_CARD;
            rewardEventQueue->addObject(eventObj);
        }
        

        if (prevUserLevelBeforeRivalBattle < PlayerInfo::getInstance()->getLevel()){
            // user level up
            EventObject *eventObj1 = new EventObject();
            eventObj1->eventCode = TRACE_EVENT_LEVEL_UP_AFTER_BATTLE;
            rewardEventQueue->addObject(eventObj1);
        }
        
        if (responseRivalBattleInfo->battleResult == 0){ // lose
            
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_GO_RIVAL_AFTER_LOSE;
            rewardEventQueue->addObject(eventObj);
        }
        else if (responseRivalBattleInfo->battleResult == 1){ // win
            
            ///////////////////
            //
            // 현재 배경 음악 정지
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG->stopBackgroundMusic();
            }
            
            ///////////////////////
            //
            // 추적 배경 음악 다시 재생
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
                soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
//                soundBG->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
            }
            
            if (bCallFromHistory){
                EventObject *eventObj = new EventObject();
                eventObj->eventCode = TRACE_EVENT_BACK_TO_HISTORY;
                rewardEventQueue->addObject(eventObj);   
            }
        }
    }
    else if (_nFrom == 2){ // from boss battle
        
        if (receivedTraceResultInfo->exp > 0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_EXP_UP;
            eventObj->exp = receivedTraceResultInfo->exp;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (receivedTraceResultInfo->coin > 0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_COIN;
            eventObj->coin = receivedTraceResultInfo->coin;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (receivedTraceResultInfo->cards->count()>0){
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_CARD;
            rewardEventQueue->addObject(eventObj);
        }
        
        if (prevUserLevelBeforeRivalBattle < PlayerInfo::getInstance()->getLevel()){
            // user level up
            EventObject *eventObj1 = new EventObject();
            eventObj1->eventCode = TRACE_EVENT_LEVEL_UP_AFTER_BATTLE;
            rewardEventQueue->addObject(eventObj1);
        }
        
        if (receivedTraceResultInfo->questBattleResult == 0){ // lose
            
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_GO_BOSS_AFTER_LOSE;
            rewardEventQueue->addObject(eventObj);
            
            BossHP = receivedTraceResultInfo->rival_hp_after_battle;
        }
        else if (receivedTraceResultInfo->questBattleResult == 1){ // win
            
            ///////////////////
            //
            // 현재 배경 음악 정지
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG->stopBackgroundMusic();
            }
            
            ///////////////////////
            //
            // 추적 배경 음악 다시 재생
            
            if (PlayerInfo::getInstance()->getBgmOption()){
                soundBG = CocosDenshion::SimpleAudioEngine::sharedEngine();
                soundBG->playBackgroundMusic("audio/bgm_quest_01.mp3", true);
//                soundBG->playBackgroundMusic("audio/bgm_rival_01_brief.mp3", true);
            }
            
            EventObject *eventObj = new EventObject();
            eventObj->eventCode = TRACE_EVENT_STAGE_CLEAR;
            rewardEventQueue->addObject(eventObj);
        }
    }
    
    RewardEventManager();
    
    traceUILayer->Refresh();
}
/*
void TraceLayer::callBackFromBoss()
{
    
    this->removeChild(bossLayer, true);
    
    responseRivalBattleInfo = new ResponseRivalBattle();
    responseRivalBattleInfo->battleResult = receivedTraceResultInfo->questBattleResult;
    
    // new
    rewardEventQueue = new CCArray();
    
    EventObject *dummyEvent = new EventObject();
    rewardEventQueue->addObject(dummyEvent); // dummy event;
    
    EventObject *event2 = new EventObject();
    event2->eventCode = TRACE_EVENT_SHOW_RIVAL_BATTLE_RESULT;
    rewardEventQueue->addObject(event2);
    
    
    
    if (receivedTraceResultInfo->coin > 0){
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_COIN;
        eventObj->coin = receivedTraceResultInfo->coin;
        rewardEventQueue->addObject(eventObj);
    }
    
    if (receivedTraceResultInfo->cards->count()>0){
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_CARD;
        rewardEventQueue->addObject(eventObj);
    }
    
    if (receivedTraceResultInfo->questBattleResult == 0){ // lose
        
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_GO_BOSS_AFTER_LOSE;
        rewardEventQueue->addObject(eventObj);
        
        RewardEventManager();
    }
    else if (receivedTraceResultInfo->questBattleResult == 1){ // win
        
        EventObject *eventObj = new EventObject();
        eventObj->eventCode = TRACE_EVENT_STAGE_CLEAR;
        rewardEventQueue->addObject(eventObj);
        
        
        // ?¥Î≤§?????
        RewardEventManager();
    }
}
*/

void TraceLayer::actionRivalBattleResult1(){
    
    CCSize size = GameConst::WIN_SIZE;
    
    this->removeChildByTag(500,true);
    
    CCSprite* pSpr11 = NULL;
    CCLog(" rivalBattleResult->battleResult :%d", responseRivalBattleInfo->battleResult);
    
    if (responseRivalBattleInfo->battleResult == 0){ // lose
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_lose.png");
    }
    else if (responseRivalBattleInfo->battleResult == 1){ // win
        pSpr11 = CCSprite::create("ui/battle/battle_duel_result_win.png");
    }
    pSpr11->setAnchorPoint(ccp(0.5,0.5));
    pSpr11->setPosition( ccp(size.width/2, size.height/2));
    pSpr11->setTag(500);
    this->addChild(pSpr11, 1);
    
    this->schedule(schedule_selector(TraceLayer::actionRivalBattleResult2), 2.0f, 0, 0);
}

void TraceLayer::actionRivalBattleResult2(){
    this->removeChildByTag(500,true);
    
    RewardEventManager();
}

void TraceLayer::backToHistoryDetail(){
    TraceDetailLayer::getInstance()->callBackFromTraceLayer();
}




void TraceLayer::startCardAction()
{
    bRewardCardAction = true;
    CCSprite *pSpr0 = CCSprite::create("ui/home/ui_BG.png");
    pSpr0->setAnchorPoint(ccp(0,0));
    pSpr0->setPosition( ccp(0,0) );
    pSpr0->setTag(88);
    this->addChild(pSpr0, 90+100);
    
    showCardCnt = 0;
    showRewardCard();
}

void TraceLayer::closeCardAction()
{
    restoreTouchDisable();
    //UserStatLayer::getInstance()->setVisible(true);
    this->removeChildByTag(88, true);
    callBackRewardCard();
    bRewardCardAction = false;
}

void TraceLayer::showRewardCard()
{
    //UserStatLayer::getInstance()->setVisible(false);
    
    CCSize size = GameConst::WIN_SIZE;
    
    CardInfo* card = (CardInfo*)rewardCards->objectAtIndex(showCardCnt);
    
    cardDetailViewLayer = new CardDetailViewLayer(CCSize(size.width, size.height), card, NULL, DIRECTION_CARDPACK_OPEN);
    cardDetailViewLayer->setScale(0.0f);
    this->addChild(cardDetailViewLayer, 100+100);
    
    //CC_SAFE_DELETE(card);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 0.80f);
    cardDetailViewLayer->runAction(CCSequence::actions(actionScale1, NULL));
    
    CCCallFunc* call_Fade4 = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceLayer::FadeWhite04));
    this->runAction(call_Fade4);
    
    CCDelayTime *delay = CCDelayTime::actionWithDuration(0.5f);
    CCCallFunc* call_btn = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceLayer::CreateBtn));
    this->runAction(CCSequence::actions(delay, call_btn, NULL));
}

void TraceLayer::FadeWhite04()
{
    playEffect("audio/card_show.mp3");
    
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* whiteBG = CCSprite::create("ui/card_tab/cardpack/card_white.png");
    whiteBG->setAnchorPoint(ccp(0.5f, 0.5f));
    whiteBG->setPosition(ccp(size.width/2, size.height/2));
    whiteBG->setScale(0.0f);
    whiteBG->setOpacity(255);
    whiteBG->setTag(FADE_04);
    this->addChild(whiteBG, 200+100);
    
    CCFiniteTimeAction* actionScale1 = CCScaleTo::actionWithDuration(0.20f, 1.6f);
    whiteBG->runAction(CCSequence::actions(actionScale1, NULL));
    
    CCFadeTo* fadeout = CCFadeTo::actionWithDuration(0.33f, 0);
    CCCallFunc* callWhite = CCCallFunc::actionWithTarget(this, callfunc_selector(TraceLayer::RemoveFade04));
    whiteBG->runAction(CCSequence::actions(fadeout, callWhite, NULL));
}
void TraceLayer::RemoveFade04()
{
    this->removeChildByTag(FADE_04, true);
}

void TraceLayer::CreateBtn()
{
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* btn = CCSprite::create("ui/shop/popup_btn_c1.png");
    btn->setAnchorPoint(ccp(0.5f, 0.5f));
    btn->setPosition((ccp(size.width/2, accp(60.0f))));
    btn->setTag(CARD_CLOSE_BTN);
    this->addChild(btn, 200+100);
    
    CCLabelTTF* confirm = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue", 13);
    confirm->setColor(COLOR_YELLOW);
    confirm->setTag(CARD_CLOSE_BTN_LABEL);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, accp(60.0f)), confirm, 201+100);
}



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// MoneySpendNotiPopup
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


MoneySpendNotiPopup::MoneySpendNotiPopup(int coin)
{
    setTouchEnabled(true);
    InitUI(coin);
}

MoneySpendNotiPopup::~MoneySpendNotiPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void MoneySpendNotiPopup::InitUI(int coin)
{
    
    TraceLayer::getInstance()->popupCnt++;
    
    CCSize size = GameConst::WIN_SIZE;
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.5f, 0.5f));
    
    int yy = size.height/2 - accp(186);//accp(500);
    popupBG->setPosition(ccp(size.width/2, size.height/2));// accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    string text = "Coin을 사용하시겠습니까?\n";
    char buf[10];
    sprintf(buf, "%d", coin);
    text.append(buf).append("의 Coin이 차감됩니다.");
    
    CCLabelTTF* Label2 = CCLabelTTF::create(text.c_str(), "Thonburi", 11);
    Label2->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f, size.height/2.0f + accp(20.0f)), Label2, 160);
    //    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2, yy + accp(186) - accp(50)), Label2, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(101);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(ccp(accp(93.0f), yy + accp(10)));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(ccp(accp(342.0f), yy + accp(10)));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(194.0f), yy + accp(15)), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), ccp(accp(443.0f), yy + accp(15)), RightLabel, 160);
    
    /*
     CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
     popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
     
     int yy = accp(500);
     popupBG->setPosition(accp(89.0f, yy));//220.0f));
     
     this->addChild(popupBG);
     
     string text = "Coin???¨Ï????Í≤??????\n";
     char buf[10];
     sprintf(buf, "%d", coin);
     text.append(buf).append("??Coin??Ï∞®Í??©Î???");
     
     CCLabelTTF* buyLabel = CCLabelTTF::create(text.c_str(), "Thonburi", 13);
     buyLabel->setColor(COLOR_WHITE);
     registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
     
     CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
     LeftBtn->setTag(101);
     LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
     LeftBtn->setPosition(accp(93.0f, yy + 5));
     this->addChild(LeftBtn, 10);
     
     CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
     RightBtn->setTag(102);
     RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
     RightBtn->setPosition(accp(342.0f, yy + 5));
     this->addChild(RightBtn, 10);
     
     CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
     LeftLabel->setColor(COLOR_YELLOW);
     registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
     
     CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
     RightLabel->setColor(COLOR_YELLOW);
     registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
     */
    
}

void MoneySpendNotiPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void MoneySpendNotiPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI ?¬¢?????GL?¬¢???¬∞?????§Œ?¬©????
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        // ok
        //TraceLayer::getInstance()->normalMoneyResult = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 2, PlayerInfo::getInstance()->traceTeam, true);
        
        ResponseQuestUpdateResultInfo *info = ARequestSender::getInstance()->requestUpdateQuestResult(TraceLayer::getInstance()->questID, 2, PlayerInfo::getInstance()->traceTeam, true);
        TraceNormalEnemyLayer::getInstance()->removeAllChildrenWithCleanup(true);
        
        TraceLayer::getInstance()->removeChild(TraceLayer::getInstance()->normalEnemyLayer,true);
        TraceLayer::getInstance()->removeChildByTag(400, true);
        TraceLayer::getInstance()->removeChildByTag(401, true);
        TraceLayer::getInstance()->removeChildByTag(402, true);
        TraceLayer::getInstance()->removeChildByTag(403, true);
        TraceLayer::getInstance()->removeChildByTag(404, true);
        
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->popupCnt--;
        
        TraceLayer::getInstance()->setTraceResult(info);//TraceLayer::getInstance()->normalMoneyResult);
        //TraceLayer::getInstance()->actionMoneySendReward();
        
        // coin ?????.
        //
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->popupCnt--;
        TraceLayer::getInstance()->setTouchEnabled(true);
    }
}

/////////////////////////////////////////////////////////////////////////////////////////
// No money popup
/////////////////////////////////////////////////////////////////////////////////////////

NoMoneyPopup::NoMoneyPopup()
{
    InitUI();
}

NoMoneyPopup::~NoMoneyPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void NoMoneyPopup::InitUI()
{
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    this->setTouchEnabled(true);
    
    int yy = 250;
    popupBG->setPosition(accp(89.0f, yy));
    
    this->addChild(popupBG);
    
    const char* text = "Coin이 부족합니다.\nCoin 충전을 위해 상점으로\n이동하시겠습니까?";
    
    CCLabelTTF* buyLabel = CCLabelTTF::create(text, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
    
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(101);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
    
}

void NoMoneyPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void NoMoneyPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    //: UI ?¬¢?????GL?¬¢???¬∞?????§Œ?¬©????
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        // ok
        // ????¬∫?? ?¬•??
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NoStaminaPopup
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


NoStaminaPopup::NoStaminaPopup()
{
    InitUI();
}

NoStaminaPopup::~NoStaminaPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void NoStaminaPopup::InitUI()
{
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 250;
    popupBG->setPosition(accp(89.0f, yy));
    
    this->addChild(popupBG);
    
    // 그린 젬 또는 넉넉한 그린 젬의 수량을 조사
    int counter = 0;
    ResponseItemInfo* itemInfo = ARequestSender::getInstance()->requestItemList();
    for (int i=0; i<itemInfo->itemList->count(); i++)
    {
        ItemInfo* tempItem = (ItemInfo* )itemInfo->itemList->objectAtIndex(i);
        if(10005 == tempItem->itemID)       // 그린 젬 ID == 10005
        {
            counter = tempItem->count;
            break;
        }
        else if (10007 == tempItem->itemID) // 넉넉한 그린 젬 ID == 10007
        {
            counter = tempItem->count;
            break;
        }
    }

    CCLabelTTF* buyLabel = NULL;
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    if (0 < counter)
    {
//        const char* text = "스테미나가 부족합니다.\n충전을 위해 인벤토리로\n이동하시겠습니까?";
        buyLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("more_sp_msg_toInven"), "Thonburi", 13);
        CCLog("Rich Green Gem Counter : %d", counter);
        
        LeftBtn->setTag(100);
    }
    else
    {
//        const char* text = "스테미나가 부족합니다.\n충전을 위해 상점으로\n이동하시겠습니까?";
        buyLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("more_sp_msg_toShop"), "Thonburi", 13);
        CCLog("Green Gem Counter : %d", counter);
        
        LeftBtn->setTag(101);
    }
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
    
    // 확인 버튼도 조건에 따라 태그를 다르게 하여 이동 루트를 나눔
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
    
}

void NoStaminaPopup::ccTouchesBegan(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    
}

void NoStaminaPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 100, localPoint))
    {
        MainScene::getInstance()->ShowMainMenu();
        MainScene::getInstance()->initUserStatLayer();
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_DOJO);
        
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG->stopBackgroundMusic();
        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
        soundMainBG();
    }
    else if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        MainScene::getInstance()->ShowMainMenu();
        MainScene::getInstance()->initUserStatLayer();
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_SHOP);
        // ok
        // ????¬∫?? ?¬•??
        //this->removeAllChildrenWithCleanup(true);
        //this->removeFromParentAndCleanup(true);
        
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG->stopBackgroundMusic();
        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
        soundMainBG();
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
    }
}

///////////////////////////////////////////////////////////////////////////////

NoBattlePointPopup::NoBattlePointPopup()
{
    InitUI();
}

NoBattlePointPopup::~NoBattlePointPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void NoBattlePointPopup::InitUI()
{
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 250;
    popupBG->setPosition(accp(89.0f, yy));
    
    this->addChild(popupBG);
    
    
    // 블루 젬 또는 넉넉한 블루 젬의 수량을 조사
    int counter = 0;
    ResponseItemInfo* itemInfo = ARequestSender::getInstance()->requestItemList();
    for (int i=0; i<itemInfo->itemList->count(); i++)
    {
        ItemInfo* tempItem = (ItemInfo* )itemInfo->itemList->objectAtIndex(i);
        if (10004 == tempItem->itemID)      // 블루 젬 ID == 10004
        {
            counter = tempItem->count;
            break;
        }
        else if (10006 == tempItem->itemID) // 넉넉한 블루 젬 ID == 10006
        {
            counter = tempItem->count;
            break;
        }
    }
    
    CCLabelTTF* buyLabel = NULL;
    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    if (0 < counter)
    {
//        const char* text = "배틀포인트가 부족합니다.\n충전을 위해 인벤토리로\n이동하시겠습니까?";
        buyLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("more_bp_msg_toInven"), "Thonburi", 13);
        CCLog("Rich Blue Gem Counter : %d", counter);
        
        LeftBtn->setTag(100);
    }
    else
    {
//        const char* text = "배틀포인트가 부족합니다.\n충전을 위해 상점으로\n이동하시겠습니까?";
        buyLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("more_bp_msg_toShop"), "Thonburi", 13);
        CCLog("Blue Gem Counter : %d", counter);
        
        LeftBtn->setTag(101);
    }

    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);
    
    // 확인 버튼도 조건에 따라 태그를 다르게 하여 이동 루트를 나눔
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
    
}


void NoBattlePointPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    //: ?¬¢?????????????? ????????????¬©????
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    location = CCDirector::sharedDirector()->convertToGL(location);
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    
    if(GetSpriteTouchCheckByTag(this, 100, localPoint))
    {
        MainScene::getInstance()->ShowMainMenu();
        MainScene::getInstance()->initUserStatLayer();
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_DOJO);
        
        MainScene::getInstance()->removeChildByTag(586,true);
        
        if (TraceLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceLayer::getInstance(),true);
            TraceLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        if (TraceHistoryLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceHistoryLayer::getInstance(),true);
            TraceHistoryLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        if (TraceDetailLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(),true);
            TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        // ok
        // ????¬∫?? ?¬•??
        //this->removeAllChildrenWithCleanup(true);
        //this->removeFromParentAndCleanup(true);
        
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG->stopBackgroundMusic();
        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
        soundMainBG();
    }
    else if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        MainScene::getInstance()->ShowMainMenu();
        MainScene::getInstance()->initUserStatLayer();
        MainScene::getInstance()->goMainScene(MainScene::MAIN_LAYER_SHOP);
        
        MainScene::getInstance()->removeChildByTag(586,true);
        
        if (TraceLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceLayer::getInstance(),true);
            TraceLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        if (TraceHistoryLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceHistoryLayer::getInstance(),true);
            TraceHistoryLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        if (TraceDetailLayer::getInstance()){
            MainScene::getInstance()->removeChild(TraceDetailLayer::getInstance(),true);
            TraceDetailLayer::getInstance()->removeAllChildrenWithCleanup(true);
        }
        // ok
        // ????¬∫?? ?¬•??
        //this->removeAllChildrenWithCleanup(true);
        //this->removeFromParentAndCleanup(true);
        
        
        
        
        
        ///////////////////
        //
        // 추적 배경 음악 정지
        
        if (PlayerInfo::getInstance()->getBgmOption()){
            soundBG->stopBackgroundMusic();
        }
        
        ///////////////////
        //
        // 메인 배경 음악 재생
        
        soundMainBG();
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        if (TraceRivalLayer::getInstance()){
            TraceRivalLayer::getInstance()->setTouchEnabled(true);
        }
        if (TraceHiddenRivalLayer::getInstance()){
            TraceHiddenRivalLayer::getInstance()->setTouchEnabled(true);
        }
        if (TraceBossLayer::getInstance()){
            TraceBossLayer::getInstance()->setTouchEnabled(true);
        }
        
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////
// Trace fail popup
/////////////////////////////////////////////////////////////////////////////////////////////////////

TraceFailPopup::TraceFailPopup()
{
    initUI();
}

TraceFailPopup::~TraceFailPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void TraceFailPopup::initUI()
{
    CCSize size = GameConst::WIN_SIZE;
    
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_a.png");
    popupBG->setAnchorPoint(ccp(0.5f, 0.5f));
    popupBG->setPosition(ccp(size.width/2, size.height/2));// accp(89.0f, yy));//220.0f));
    popupBG->setOpacity(100.0f);
    popupBG->setScale(0.8f);
    //    popupBG->setTag(180);
    
    this->addChild(popupBG, 10);
    
    const char* text2 = "적이 도망쳤지만\n보상은 획득하지 못했습니다.";
    CCLabelTTF* Label2 = CCLabelTTF::create(text2, "Thonburi", 13);
    Label2->setColor(COLOR_WHITE);
    //    Label2->setTag(181);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f, size.height/2.0f + accp(50.0f)), Label2, 15);
    /*
     CCSprite* pSprc = CCSprite::create("ui/quest/trace/trace_popup_complete.png");
     pSprc->setAnchorPoint(ccp(0.5,0.5));
     //pSprc->setPosition(ccp(accp(89 + 194), yy - accp(130)));
     pSprc->setPosition(ccp( size.width/2, yy + accp(186)));
     this->addChild(pSprc);
     
     CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
     LeftBtn->setTag(101);
     LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
     LeftBtn->setPosition(ccp(accp(93.0f), yy + accp(10)));
     this->addChild(LeftBtn, 10);
     */
    
    CCMenuItemImage* pSprConfirm = CCMenuItemImage::create("ui/shop/popup_btn_c1.png", "ui/shop/popup_btn_c2.png", this, menu_selector(TraceNormalEnemyLayer::callBackRemoveTimeOut));
    pSprConfirm->setAnchorPoint(ccp(0.5f, 0.5f));
    pSprConfirm->setPosition(ccp(size.width/2.0f, size.height/2.0f - accp(65.0f)));
    pSprConfirm->setScaleX(0.8f);
    pSprConfirm->setScaleY(1.2f);
    
    //    pSprConfirm->setTag(182);
    
    CCLabelTTF* pLblConfirm = CCLabelTTF::create("확인", "HelveticaNeue-Bold", 13);
    pLblConfirm->setColor(COLOR_WHITE);
    //    pLblConfirm->setTag(183);
    registerLabel(this, ccp(0.5f, 0.5f), ccp(size.width/2.0f, size.height/2.0f - accp(65.0f)), pLblConfirm, 15);
    
    CCMenu* pMenu = CCMenu::create(pSprConfirm, NULL);
    pMenu->setAnchorPoint(ccp(0.0f, 0.0f));
    pMenu->setPosition(ccp(0.0f, 0.0f));
    //    pMenu->setTag(184);
    this->addChild(pMenu, 13);
    
    /*
     CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
     RightBtn->setTag(102);
     RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
     RightBtn->setPosition(ccp(accp(342.0f), yy + accp(10)));
     this->addChild(RightBtn, 10);
     
     CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
     LeftLabel->setColor(COLOR_YELLOW);
     registerLabel(this, ccp(0.5f, 0.0f), ccp(accp(194.0f), yy + accp(15)), LeftLabel, 160);
     
     CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
     RightLabel->setColor(COLOR_YELLOW);
     registerLabel(this, ccp(0.5f, 0), ccp(accp(443.0f), yy + accp(15)), RightLabel, 160);
     */
}


void TraceFailPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        ARequestSender::getInstance()->requestStageList();
        
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->exitLayer();
        /*
         ///////////////////
         //
         // 추적 배경 음악 정지
         
         if (PlayerInfo::getInstance()->getBgmOption()){
         soundBG->stopBackgroundMusic();
         }
         
         ///////////////////
         //
         // 메인 배경 음악 재생
         
         soundMainBG();
         */
    }
    /*
     if(GetSpriteTouchCheckByTag(this, 102, localPoint))
     {
     this->removeAllChildrenWithCleanup(true);
     this->removeFromParentAndCleanup(true);
     TraceLayer::getInstance()->exitLayer();
     
     ///////////////////
     //
     // 추적 배경 음악 재생
     
     if (PlayerInfo::getInstance()->getBgmOption()){
     soundBG->stopBackgroundMusic();
     }
     
     ///////////////////
     //
     // 메인 배경 음악 재생
     
     soundMainBG();
     }
     */
}

/////////////////////////////////////////

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


TeamSettingWarningPopup::TeamSettingWarningPopup()
{
    InitUI();
}

TeamSettingWarningPopup::~TeamSettingWarningPopup()
{
    this->removeAllChildrenWithCleanup(true);
}

void TeamSettingWarningPopup::InitUI()
{
    this->setTouchEnabled(true);
    
    CCSprite* popupBG = CCSprite::create("ui/shop/popup_bg_s.png");
    popupBG->setAnchorPoint(ccp(0.0f, 0.0f));
    
    int yy = 250;//accp(250);//250;// * SCREEN_ZOOM_RATE;// accp(500);
    popupBG->setPosition(accp(89.0f, yy));//220.0f));
    
    this->addChild(popupBG);
    
    const char* text = "팀에 카드가 설정되어 있지 않거나\n리더카드가 설정되어 있지 않아\n퀘스트를 진행할 수 없습니다.\n카드-팀 에서 카드를 설정해 주세요.";
    
    CCLabelTTF* buyLabel = CCLabelTTF::create(text, "Thonburi", 13);
    buyLabel->setColor(COLOR_WHITE);
    registerLabel(this, ccp(0.5f, 0.5f), accp(319.0f, yy + 200), buyLabel, 160);

    CCSprite* LeftBtn = CCSprite::create("ui/shop/popup_btn_a1.png");
    LeftBtn->setTag(101);
    LeftBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    LeftBtn->setPosition(accp(93.0f, yy + 5));
    this->addChild(LeftBtn, 10);
    
    CCSprite* RightBtn = CCSprite::create("ui/shop/popup_btn_b1.png");
    RightBtn->setTag(102);
    RightBtn->setAnchorPoint(ccp(0.0f, 0.0f));
    RightBtn->setPosition(accp(342.0f, yy + 5));
    this->addChild(RightBtn, 10);
    
    CCLabelTTF* LeftLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("Confirm_btn"), "HelveticaNeue-Bold", 12);
    LeftLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0.0f), accp(194.0f, yy + 15), LeftLabel, 160);
    
    CCLabelTTF* RightLabel = CCLabelTTF::create(LocalizationManager::getInstance()->get("cancel_btn"), "HelveticaNeue-Bold", 12);
    RightLabel->setColor(COLOR_YELLOW);
    registerLabel(this, ccp(0.5f, 0), accp(443.0f, yy + 15), RightLabel, 160);
}


void TeamSettingWarningPopup::ccTouchesEnded(cocos2d::CCSet* touches, cocos2d::CCEvent* event)
{
    CCTouch *touch = (CCTouch*)(touches->anyObject());
    CCPoint location = touch->getLocationInView();
    
    location = CCDirector::sharedDirector()->convertToGL(location);
    
    CCPoint localPoint = this->convertToNodeSpace(location);
    
    if(GetSpriteTouchCheckByTag(this, 101, localPoint))
    {
        // ok
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->popupCnt--;
        TraceLayer::getInstance()->setTouchEnabled(true);
    }
    
    if(GetSpriteTouchCheckByTag(this, 102, localPoint))
    {
        // cancel
        
        this->removeAllChildrenWithCleanup(true);
        this->removeFromParentAndCleanup(true);
        TraceLayer::getInstance()->popupCnt--;
        TraceLayer::getInstance()->setTouchEnabled(true);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////